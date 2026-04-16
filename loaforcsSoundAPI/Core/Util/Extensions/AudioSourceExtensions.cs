using UnityEngine;

namespace loaforcsSoundAPI.Core.Util.Extensions;

public static class AudioSourceExtensions {
	public static void PlayThenDestroy(this AudioSource source) {
		source.Play();
		Object.Destroy(source.gameObject, source.clip.length);
	}

	public static void PlayWithoutReplacement(this AudioSource source) {
		AudioSourceAdditionalData data = AudioSourceAdditionalData.GetOrCreate(source);
		data.DisableReplacing = true;
		source.Play();
		data.DisableReplacing = false;
	}
}