using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace loaforcsSoundAPI.Core;

class SoundAPIAudioManager : MonoBehaviour {
	internal static readonly Dictionary<AudioSource, AudioSourceAdditionalData> audioSourceData = [];

	static SoundAPIAudioManager Instance;

	void Awake() {
		SceneManager.sceneLoaded += (_, _) => {
			if(!Instance)
				SpawnManager();
			
			RunCleanup();
		};
		
	}

	internal static void SpawnManager() {
		loaforcsSoundAPI.Logger.LogInfo("Starting AudioManager.");
		GameObject manager = new("SoundAPI_AudioManager");
		DontDestroyOnLoad(manager);
		Instance = manager.AddComponent<SoundAPIAudioManager>();
		
	}
	
	// this seems icky but i do not care
	void Update() {
		Debuggers.UpdateEveryFrame?.Log($"sanity check: soundapi audio manager is running!");

		
		foreach (AudioSourceAdditionalData data in audioSourceData.Values) {
			data.Update();
		}
	}

	void OnDisable() {
		loaforcsSoundAPI.Logger.LogDebug("manager disabled");
	}

	void OnDestroy() {
		loaforcsSoundAPI.Logger.LogDebug("manager destroyed");
	}


	// This isn't particularly good, but because AudioSourceAdditionalData is not a behaviour it can't keep track of OnEnable, OnDisable itself
	// maybe putting this on a background thread that runs every few seconds would work but really i don't care too much, i want this project done
	static void RunCleanup() {
		loaforcsSoundAPI.Logger.LogDebug("cleaning up old audio source entries");
		foreach (AudioSource source in audioSourceData.Keys.ToArray()) {
			if (!source) {
				audioSourceData.Remove(source);
			}
		}
	}
}