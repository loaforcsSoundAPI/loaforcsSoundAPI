using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BepInEx;
using loaforcsSoundAPI.Core.JSON;
using loaforcsSoundAPI.Echo.Data;
using loaforcsSoundAPI.SoundPacks.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace loaforcsSoundAPI.Echo;

static class CustomSoundsLoadPipeline {
	internal static List<SoundPack> CustomSoundsPacks { get; private set; } = [];
	internal static long LoadTime { get; private set; }
	
	internal static void StartPipeline() {
		Stopwatch completeLoadingTimer = Stopwatch.StartNew();
		EchoPreloader.Logger.LogInfo("Starting CustomSounds Sound-Pack loading pipeline.");
		
		// Step 1: Find mods
		List<string> modFolders = FindCustomSoundsSoundPacks();
		EchoPreloader.Logger.LogInfo($"Found '{modFolders.Count}' sound-pack(s) to load from CustomSounds!");
		
		// Step 2: Process
		foreach (string modFolder in modFolders) {
			// Setup data
			ThunderstoreManifest manifest = JSONDataLoader.LoadFromFile<ThunderstoreManifest>(Path.Combine(modFolder, "manifest.json"));
			SoundPack pack = new(manifest.Name, modFolder);
			SoundReplacementCollection collection = new(pack);
			
			EchoPreloader.Logger.LogInfo($"Loading Sound-Pack: {pack.Name}");
			// technically i believe this is called after the splash screens are done, but it should be fine because it's done asynchronously and not on another thread?
			PopulateSoundReplacementCollection(collection, Path.Combine(modFolder, "CustomSounds")).ContinueWith(_ => {
				SoundAPI.RegisterSoundPack(pack);
				CustomSoundsPacks.Add(pack);
			});
		}
		
		completeLoadingTimer.Stop();
		LoadTime = completeLoadingTimer.ElapsedMilliseconds;
		EchoPreloader.Logger.LogInfo($"All done! Took {LoadTime}ms :3");
	}

	async static Task PopulateSoundReplacementCollection(SoundReplacementCollection parent, string path) {
		foreach (string audioFile in Directory.GetFiles(path, "*.wav", SearchOption.TopDirectoryOnly)) {
			AudioClip clip = await SoundAPI.LoadAudioFileAsync(audioFile);
			(string clipName, int weight) = ParseFileName(Path.GetFileNameWithoutExtension(audioFile));

			SoundReplacementGroup group = new(parent, [$"*:*:{clipName}"]);
			SoundInstance sound = new(group, weight, clip);
		}

		foreach (string subDir in Directory.GetDirectories(path)) {
			string sourceObjectNameMatch = "*";
			string directoryName = Path.GetFileName(subDir);
			EchoPreloader.Logger.LogDebug($"directoryName: {directoryName}");
			if (directoryName.EndsWith("-AS")) sourceObjectNameMatch = directoryName.Substring(0, directoryName.Length - 3);
			EchoPreloader.Logger.LogDebug($"sourceObjectNameMatch: {sourceObjectNameMatch}");
			
			foreach (string audioFile in Directory.GetFiles(subDir, "*.wav", SearchOption.TopDirectoryOnly)) {
				AudioClip clip = await SoundAPI.LoadAudioFileAsync(audioFile);
				(string clipName, int weight) = ParseFileName(Path.GetFileNameWithoutExtension(audioFile));

				SoundReplacementGroup group = new(parent, [$"*:{sourceObjectNameMatch}:{clipName}"]);
				SoundInstance sound = new(group, weight, clip);
			}
		}
	}
	
	static List<string> FindCustomSoundsSoundPacks() {
		List<string> directories = Directory
		   .GetDirectories(Paths.PluginPath)
		   .Where(modFolder => Directory.Exists(Path.Combine(modFolder, "CustomSounds")))
		   .ToList();

		return directories;
	}
	
	static (string clipName, int weight) ParseFileName(string fileName) {
		string[] stringParts = fileName.Split("-");
		if (!int.TryParse(stringParts.Last(), out int weight)) weight = 1; // if failed to parse
		return (stringParts[0], weight);
	}
}