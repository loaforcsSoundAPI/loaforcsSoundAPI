using BepInEx.Configuration;

namespace loaforcsSoundAPI.Core.Patches;

static class PatchConfig {
	internal static bool AudioClipSpoofing { get; private set; }

	internal static void Bind(ConfigFile file) {
		AudioClipSpoofing = file.Bind("Experiments", nameof(AudioClipSpoofing), false, "Should SoundAPI spoof the return value of AudioSource.clip? This can improve compatibility but is also fairly invasive.").Value;
	}
}