using BepInEx.Configuration;

namespace loaforcsSoundAPI.Core.Patches;

static class PatchConfig {
	public enum Backend {
		HarmonyX,
		NativeBackend
	}

	internal static bool AudioClipSpoofing { get; private set; }
	internal static bool UEFOneShotWorkaround { get; private set; }
	internal static Backend PreferredBackend { get; private set; }

	const string ExperimentsHeader = "Experiments";

	internal static void Bind(ConfigFile file) {
		AudioClipSpoofing = file.Bind("Advanced", nameof(AudioClipSpoofing), true, "Should SoundAPI spoof the return value of AudioSource.clip? This improves compatibility in most edge cases.").Value;
		PreferredBackend = file.Bind("Advanced", nameof(PreferredBackend), Backend.NativeBackend, "What backend should SoundAPI try to use? You should only use this option if you know what it means. Note: NativeBackend needs to be supported on a per unity version basis, if the current version is not supported it will fallback to the old HarmonyX backend automatically.").Value;
		UEFOneShotWorkaround = file.Bind(ExperimentsHeader, nameof(UEFOneShotWorkaround), false, "update_every_frame by default does not work on `.PlayOneshot()`. This experiment works around the issue by instead playing the clip on a duplicate AudioSource, allowing UEF to work as intended.").Value;
	}
}