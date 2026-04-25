using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.SoundPacks.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace loaforcsSoundAPI.SoundPacks.AudioClipLoading;

// TODO: fix me, this is not good logic.
// im going to be so real i no longer understand whats happening here
class MultithreadedAudioClipLoader : IAudioClipLoader {
	List<LoadSoundOperation> _webRequestOperations = [ ];
	static volatile int _activeThreads;

	ConcurrentBag<Exception> _threadPoolExceptions = [ ];
	ConcurrentQueue<LoadSoundOperation> _queuedOperations = new ConcurrentQueue<LoadSoundOperation>();


	bool _threadsShouldExit, _displayedHalfwayMessage = false;

	public int Count => _webRequestOperations.Count;

	public void LoadAllBlocking() {
		Stopwatch timer = Stopwatch.StartNew();

		for(int i = 0; i < 16; i++) {
			new Thread(() => {
				LoadSoundOperation operation;
				while(_queuedOperations.Count == 0 && !_threadsShouldExit) {
					Thread.Yield();
				}

				Interlocked.Increment(ref _activeThreads);
				Debuggers.SoundReplacementLoader?.Log($"active threads at {_activeThreads}");

				while(_queuedOperations.TryDequeue(out operation)) {
					try {
						AudioClip clip = DownloadHandlerAudioClip.GetContent(operation.WebRequest);
						operation.Sound.Clip = clip;
						operation.WebRequest.Dispose();
						Debuggers.SoundReplacementLoader?.Log("clip generated");

						operation.IsDone = true;
					} catch(Exception exception) {
						_threadPoolExceptions.Add(exception);
					}
				}

				Interlocked.Decrement(ref _activeThreads);
			}).Start();
		}

		while(_webRequestOperations.Count > 0) {
			foreach(LoadSoundOperation operation in _webRequestOperations.ToList().Where(operation => operation.IsReady)) { // .ToList() here is so we can modify the current list without causing an exception
				_queuedOperations.Enqueue(operation); // give to threads to do work
				_webRequestOperations.Remove(operation);
			}

			if(!_displayedHalfwayMessage && _webRequestOperations.Count < Count / 2) {
				_displayedHalfwayMessage = true;
				loaforcsSoundAPI.Logger.LogInfo($"(Step 5) Queued half of the needed operations!");
			}

			Thread.Yield(); // this has to be Thread.Sleep instead of Task.Delay because this needs to be blocking
		}

		loaforcsSoundAPI.Logger.LogInfo($"(Step 5) All file reads are done, waiting for the audio clips conversions.");

		while(_activeThreads > 0 || _webRequestOperations.Any(operation => !operation.IsDone)) {
			Thread.Yield();
		}

		loaforcsSoundAPI.Logger.LogInfo($"(Step 6) Took {timer.ElapsedMilliseconds}ms to finish loading audio clips from files");
		if(_threadPoolExceptions.Count != 0) {
			loaforcsSoundAPI.Logger.LogError($"(Step 6) {_threadPoolExceptions.Count} internal error(s) happened while loading:");
			foreach(Exception poolException in _threadPoolExceptions) {
				loaforcsSoundAPI.Logger.LogError(poolException.ToString());
			}
		}
	}

	public void Queue(SoundInstance sound) {
		_webRequestOperations.Add(StartWebRequestOperation(sound));
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