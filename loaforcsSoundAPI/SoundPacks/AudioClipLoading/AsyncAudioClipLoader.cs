using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.Core.Util.Extensions;
using loaforcsSoundAPI.SoundPacks.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace loaforcsSoundAPI.SoundPacks.AudioClipLoading;

class AsyncAudioClipLoader : IAudioClipLoader {
	List<LoadSoundOperation> _webRequestOperations = [ ];

	public void Queue(SoundInstance sound) {
		_webRequestOperations.Add(StartWebRequestOperation(sound));
	}

	public async Task LoadAllAsync() {
		foreach(LoadSoundOperation operation in _webRequestOperations) {
			await operation.WaitUntilReadyAsync();
			AudioClip clip = DownloadHandlerAudioClip.GetContent(operation.WebRequest);
			operation.Sound.Clip = clip;
			operation.WebRequest.Dispose();
			operation.IsDone = true;
		}
	}

	LoadSoundOperation StartWebRequestOperation(SoundInstance sound) {
		string fullPath = Path.Combine(sound.Pack.PackFolder, "sounds", sound.Sound);

		UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
			fullPath,
			IAudioClipLoader.audioExtensions[Path.GetExtension(sound.Sound)]
		);

		return new LoadSoundOperation(sound, www.SendWebRequest());
	}
}