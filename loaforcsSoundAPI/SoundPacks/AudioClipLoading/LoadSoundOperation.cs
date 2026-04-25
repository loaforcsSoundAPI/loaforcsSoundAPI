using System.Threading.Tasks;
using loaforcsSoundAPI.Core.Util.Extensions;
using loaforcsSoundAPI.SoundPacks.Data;
using UnityEngine.Networking;

namespace loaforcsSoundAPI.SoundPacks.AudioClipLoading;

class LoadSoundOperation(
	SoundInstance soundInstance,
	UnityWebRequestAsyncOperation webRequest
) {
	public readonly UnityWebRequest WebRequest = webRequest.webRequest;
	public bool IsReady => WebRequest.isDone;
	public bool IsDone { get; set; }
	public readonly SoundInstance Sound = soundInstance;

	public async Task WaitUntilReadyAsync() {
		await webRequest;
	}
}