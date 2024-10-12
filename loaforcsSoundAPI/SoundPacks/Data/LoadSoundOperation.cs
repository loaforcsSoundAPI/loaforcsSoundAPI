using System.Runtime.CompilerServices;
using loaforcsSoundAPI.SoundPacks.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace loaforcsSoundAPI.SoundPacks;

class LoadSoundOperation(
	SoundInstance soundInstance,
	UnityWebRequestAsyncOperation webRequest
) {
	public readonly UnityWebRequest WebRequest = webRequest.webRequest;
	public bool IsReady => WebRequest.isDone;
	public bool IsDone { get; set; }
	public readonly SoundInstance Sound = soundInstance;
}