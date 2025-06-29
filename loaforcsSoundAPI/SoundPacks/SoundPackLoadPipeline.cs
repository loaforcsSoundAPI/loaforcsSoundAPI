using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.Core.JSON;
using loaforcsSoundAPI.Core.Util;
using loaforcsSoundAPI.Reporting;
using loaforcsSoundAPI.SoundPacks.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace loaforcsSoundAPI.SoundPacks;

static class SoundPackLoadPipeline {
	static volatile int _activeThreads;

	// todo: maybe remove
	internal static event Action OnFinishedPipeline = delegate { };
	static Dictionary<string, List<string>> mappings = [];

	// todo: probably change this to be else where? soundinstance needs this for validation.
	internal static Dictionary<string, AudioType> audioExtensions = new Dictionary<string, AudioType> {
		{ ".ogg", AudioType.OGGVORBIS },
		{ ".wav", AudioType.WAV },
		{ ".mp3", AudioType.MPEG }
	};

	class SkippedResults {
		public int Collections;
		public int Groups;
		public int Sounds;
	}

	// todo: clip sharing/single-loading
	internal static async void StartPipeline() {
		Stopwatch completeLoadingTimer = Stopwatch.StartNew();
		Stopwatch timer = Stopwatch.StartNew();

		// Step 1: Find and load packs
		List<SoundPack> packs = FindAndLoadPacks();
		loaforcsSoundAPI.Logger.LogInfo($"(Step 1) Loading Sound-pack definitions took {timer.ElapsedMilliseconds}ms");

		if(packs.Count == 0) loaforcsSoundAPI.Logger.LogWarning("No sound-packs were found to load! This can be ignorable if you're doing testing or using SoundAPI for another purpose, but if you expected sound-packs to load you may have set it up incorrectly.");

		timer.Restart();

		// Step 2: Load mappings
		List<LoadSoundOperation> webRequestOperations = [];

		// todo: function
		foreach(SoundPack pack in packs) {
			string mappingFile = Path.Combine(pack.PackFolder, "soundapi_mappings.json");
			if(File.Exists(mappingFile)) {
				Dictionary<string, List<string>> packDefinedMappings = JSONDataLoader.LoadFromFile<Dictionary<string, List<string>>>(mappingFile);

				foreach(KeyValuePair<string, List<string>> entry in packDefinedMappings) {
					if(mappings.ContainsKey(entry.Key))
						mappings[entry.Key].AddRange(entry.Value);
					else
						mappings[entry.Key] = entry.Value;
				}
			}
		}

		loaforcsSoundAPI.Logger.LogInfo($"(Step 2) Loading Sound-pack mappings ('{mappings.Count}') took {timer.ElapsedMilliseconds}ms");
		timer.Restart();

		SkippedResults skippedStats = new SkippedResults();
		// Step 3: Load sound replacement collections data and begin loading audio
		foreach(SoundPack pack in packs) {
			// Step 4: Enter foreach hell and fire async methods to begin UWR calls to load sounds.
			foreach(SoundReplacementCollection collection in LoadSoundReplacementCollections(pack, ref skippedStats)) {
				foreach(SoundReplacementGroup replacementGroup in collection.Replacements) {
					SoundPackDataHandler.AddReplacement(replacementGroup);

					// finally actually load sounds!
					foreach(SoundInstance soundReplacement in replacementGroup.Sounds) {
						if(soundReplacement.Condition is ConstantCondition constant && constant.Value == false) {
							Debuggers.SoundReplacementLoader?.Log($"skipping a sound in '{LogFormats.FormatFilePath(collection.FilePath)}' because sound is marked as constant and has a value of false.");
							skippedStats.Sounds++;
							continue;
						}

						webRequestOperations.Add(StartWebRequestOperation(pack, soundReplacement, audioExtensions[Path.GetExtension(soundReplacement.Sound)]));
					}
				}
			}
		}

		#region boring other stuff

		int amountOfOperations = webRequestOperations.Count;
		loaforcsSoundAPI.Logger.LogInfo($"(Step 3) Skipped {skippedStats.Collections} collection(s), {skippedStats.Groups} replacement(s), {skippedStats.Sounds} sound(s)");
		loaforcsSoundAPI.Logger.LogInfo($"(Step 3) Loading sound replacement collections took {timer.ElapsedMilliseconds}ms");
		if(SoundReportHandler.CurrentReport != null) SoundReportHandler.CurrentReport.AudioClipsLoaded = amountOfOperations;
		loaforcsSoundAPI.Logger.LogInfo($"(Step 4) Started loading {amountOfOperations} audio file(s)");

		// Delay until splash screens are done and we can check in on the unity web requests
		loaforcsSoundAPI.Logger.LogInfo("Waiting for splash screens to complete to continue...");
		completeLoadingTimer.Stop();
		await Task.Delay(1);
		loaforcsSoundAPI.Logger.LogInfo("Splash screens done! Continuing pipeline");
		loaforcsSoundAPI.Logger.LogWarning("The game will freeze for a moment!");
		timer.Restart();
		completeLoadingTimer.Start(); // unpause

		// Display some info to the users so they know their game hasn't crashed.
		bool displayedHalfwayMessage = false;
		bool threadsShouldExit = false;

		ConcurrentQueue<LoadSoundOperation> queuedOperations = new ConcurrentQueue<LoadSoundOperation>();
		ConcurrentBag<Exception> threadPoolExceptions = [];

		// TODO: fix me, this is not good logic.
		for(int i = 0; i < 16; i++) {
			new Thread(() => {
				LoadSoundOperation operation;
				while(queuedOperations.Count == 0 && !threadsShouldExit) {
					Thread.Yield();
				}

				Interlocked.Increment(ref _activeThreads);
				Debuggers.SoundReplacementLoader?.Log($"active threads at {_activeThreads}");

				while(queuedOperations.TryDequeue(out operation)) {
					try {
						AudioClip clip = DownloadHandlerAudioClip.GetContent(operation.WebRequest);
						operation.Sound.Clip = clip;
						operation.WebRequest.Dispose();
						Debuggers.SoundReplacementLoader?.Log("clip generated");

						operation.IsDone = true;
					} catch(Exception exception) {
						threadPoolExceptions.Add(exception);
					}
				}

				Interlocked.Decrement(ref _activeThreads);
			}).Start();
		}

		// Step 5: Block game from progressing until all audio is loaded
		while(webRequestOperations.Count > 0) {
			foreach(LoadSoundOperation operation in webRequestOperations.ToList().Where(operation => operation.IsReady)) { // .ToList() here is so we can modify the current list without causing an exception
				queuedOperations.Enqueue(operation); // give to threads to do work
				webRequestOperations.Remove(operation);
			}

			if(!displayedHalfwayMessage && webRequestOperations.Count < amountOfOperations / 2) {
				displayedHalfwayMessage = true;
				loaforcsSoundAPI.Logger.LogInfo($"(Step 5) Queued half of the needed operations!");
			}

			Thread.Yield(); // this has to be Thread.Sleep instead of Task.Delay because this needs to be blocking
		}

		loaforcsSoundAPI.Logger.LogInfo($"(Step 5) All file reads are done, waiting for the audio clips conversions.");
		threadsShouldExit = true;

		// Step 6: Wait.
		while(_activeThreads > 0 || webRequestOperations.Any(operation => !operation.IsDone)) {
			Thread.Yield();
		}

		loaforcsSoundAPI.Logger.LogInfo($"(Step 6) Took {timer.ElapsedMilliseconds}ms to finish loading audio clips from files");
		if(threadPoolExceptions.Count != 0) {
			loaforcsSoundAPI.Logger.LogError($"(Step 6) {threadPoolExceptions.Count} internal error(s) happened while loading:");
			foreach(Exception poolException in threadPoolExceptions) {
				loaforcsSoundAPI.Logger.LogError(poolException.ToString());
			}
		}

		#endregion

		// Step 7: Fire event and final cleanup
		OnFinishedPipeline();
		mappings = null;

		loaforcsSoundAPI.Logger.LogDebug($"Active Threads that are left over: {_activeThreads}");
		loaforcsSoundAPI.Logger.LogInfo($"Entire load process took an effective {completeLoadingTimer.ElapsedMilliseconds}ms");
	}

	static List<SoundPack> FindAndLoadPacks(string entryPoint = "sound_pack.json") {
		Dictionary<string, SoundPack> packs = [];

		foreach(string file in Directory.GetFiles(Paths.PluginPath, entryPoint, SearchOption.AllDirectories)) {
			Debuggers.SoundReplacementLoader?.Log($"found entry point: '{file}'!");

			SoundPack pack = JSONDataLoader.LoadFromFile<SoundPack>(file);
			if(pack == null) continue; // json error
			pack.PackFolder = Path.GetDirectoryName(file);

			if(packs.TryGetValue(pack.Name, out SoundPack existingPack)) {
				IValidatable.LogAndCheckValidationResult($"loading '{file}'", [
					new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"A sound-pack with name '{pack.Name}' was already loaded from '{LogFormats.FormatFilePath(Path.Combine(existingPack.PackFolder, "sound_pack.json"))}'. Skipping loading the duplicate!!")
				], pack.Logger);
				continue;
			}

			Debuggers.SoundReplacementLoader?.Log("json loaded, validating");
			List<IValidatable.ValidationResult> validationResult = pack.Validate();

			if(IValidatable.LogAndCheckValidationResult($"loading '{file}'", validationResult, pack.Logger)) {
				BepInPlugin metadata;

				if(PackLoadingConfig.MetadataSpoofing)
					metadata = new BepInPlugin(pack.GUID, pack.Name, pack.Version ?? "1.0.0");
				else
					metadata = MetadataHelper.GetMetadata(typeof(loaforcsSoundAPI));

				ConfigFile configFile = loaforcsSoundAPI.GenerateConfigFile(pack.GUID, metadata);
				configFile.SaveOnConfigSet = false; // dumb setting that's enabled by default
				pack.Bind(configFile);
				configFile.Save();
				packs[pack.Name] = pack;
				SoundPackDataHandler.AddLoadedPack(pack);
				Debuggers.SoundReplacementLoader?.Log($"pack folder: {pack.PackFolder}");
			}
		}

		Debuggers.SoundReplacementLoader?.Log($"loaded '{packs.Count}' packs.");
		return packs.Values.ToList();
	}

	static List<SoundReplacementCollection> LoadSoundReplacementCollections(SoundPack pack, ref SkippedResults skippedStats) {
		List<SoundReplacementCollection> collections = [];
		if(!Directory.Exists(Path.Combine(pack.PackFolder, "replacers"))) return collections; // purely for mods that only have mappings

		Debuggers.SoundReplacementLoader?.Log($"start loading '{pack.Name}'!");

		foreach(string file in Directory.GetFiles(Path.Combine(pack.PackFolder, "replacers"), "*.json", SearchOption.AllDirectories)) {
			Debuggers.SoundReplacementLoader?.Log($"found replacer: '{file}'!");

			SoundReplacementCollection collection = JSONDataLoader.LoadFromFile<SoundReplacementCollection>(file);
			if(collection == null) continue; // json error
			collection.Pack = pack;

			if(collection.Condition is ConstantCondition constant && constant.Value == false) {
				Debuggers.SoundReplacementLoader?.Log($"skipping '{LogFormats.FormatFilePath(collection.FilePath)}' because collection is marked as constant and has a value of false.");
				skippedStats.Collections++;
				continue;
			}

			if(!IValidatable.LogAndCheckValidationResult($"loading '{LogFormats.FormatFilePath(file)}'", collection.Validate(), pack.Logger)) continue;

			List<IValidatable.ValidationResult> groupValidations = [];
			// not the cleanest
			foreach(SoundReplacementGroup replacementGroup in collection.Replacements) {
				replacementGroup.Parent = collection; // !!! - Setting data while doing validation. If this ever breaks it's here!

				if(replacementGroup.Condition is ConstantCondition constantGroup && constantGroup.Value == false) {
					Debuggers.SoundReplacementLoader?.Log($"skipping a replacement in '{LogFormats.FormatFilePath(collection.FilePath)}' because group is marked as constant and has a value of false.");
					skippedStats.Groups++;
					continue;
				}

				// validate match strings
				List<IValidatable.ValidationResult> validationResults = replacementGroup.Validate();

				// Convert mappings
				foreach(string match in replacementGroup.Matches.ToList()) { // .ToList to avoid list exception or whatever
					if(!match.StartsWith("#")) continue;

					replacementGroup.Matches.Remove(match);

					if(mappings.TryGetValue(match[1..], out List<string> mappedStrings))
						replacementGroup.Matches.AddRange(mappedStrings);
					else
						validationResults.Add(new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"Mapping: '{match}' has not been found. If it's part of a soft dependency, make sure to use a 'mod_installed' condition with 'constant' enabled."));
				}

				if(validationResults.Count != 0) {
					groupValidations.AddRange(validationResults);
					continue;
				}

				// validate sounds exist.
				foreach(SoundInstance sound in replacementGroup.Sounds) {
					sound.Parent = replacementGroup; // !!! - Setting data while doing validation. If this ever breaks it's here!
					validationResults.AddRange(sound.Validate());
				}

				if(validationResults.Count != 0) {
					groupValidations.AddRange(validationResults);
					continue;
				}


				// Imply "*:object:clip" from "object:clip"
				List<string> corrected = replacementGroup.Matches.Select(match => match.Split(":").Length == 2 ? $"*:{match}" : match).ToList();

				replacementGroup.Matches.Clear();
				replacementGroup.Matches.AddRange(corrected);
			}

			if(!IValidatable.LogAndCheckValidationResult($"loading '{LogFormats.FormatFilePath(file)}'", groupValidations, pack.Logger)) continue;

			collections.Add(collection);
		}

		return collections;
	}

	static LoadSoundOperation StartWebRequestOperation(SoundPack pack, SoundInstance sound, AudioType type) {
		string fullPath = Path.Combine(pack.PackFolder, "sounds", sound.Sound);

		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fullPath, type);

		return new LoadSoundOperation(sound, www.SendWebRequest());
	}
}