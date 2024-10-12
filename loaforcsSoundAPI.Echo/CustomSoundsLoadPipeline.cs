using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
			PopulateSoundReplacementCollection(collection, Path.Combine(modFolder, "CustomSounds"));
            
			SoundAPI.RegisterSoundPack(pack);
			CustomSoundsPacks.Add(pack);
		}
		
		completeLoadingTimer.Stop();
		LoadTime = completeLoadingTimer.ElapsedMilliseconds;
		EchoPreloader.Logger.LogInfo($"All done! Took {LoadTime}ms :3");
	}

	static void PopulateSoundReplacementCollection(SoundReplacementCollection parent, string path) {
		foreach (string audioFile in Directory.GetFiles(path, "*.wav", SearchOption.TopDirectoryOnly)) {
			AudioClip clip = LoadAudioClip(audioFile);
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
				AudioClip clip = LoadAudioClip(audioFile);
				(string clipName, int weight) = ParseFileName(Path.GetFileNameWithoutExtension(audioFile));

				SoundReplacementGroup group = new(parent, [$"*:{sourceObjectNameMatch}:{clipName}"]);
				SoundInstance sound = new(group, weight, clip);
			}
		}
	}

	// TODO: This should 100% be replaced with a method from SoundAPI when I finally get around to it.
	static AudioClip LoadAudioClip(string path) {
		using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
		request.SendWebRequest();

		try {
			while (!request.isDone) { Thread.Sleep(100); }

			if (request.result != UnityWebRequest.Result.Success)
				EchoPreloader.Logger.LogError($"AudioClip load failure on: {Path.GetFileName(path)}");
			else {
				return DownloadHandlerAudioClip.GetContent(request);
			}
		} catch (Exception) {
			EchoPreloader.Logger.LogError($"AudioClip load failure on: {Path.GetFileName(path)}");
		}

		return null;
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