using BepInEx.Configuration;

namespace loaforcsSoundAPI.SoundPacks;

static class PackLoadingConfig {
	internal static bool MetadataSpoofing { get; private set; }
	internal static bool SkipUnusedSounds { get; private set; }

	internal static void Bind(ConfigFile file) {
		MetadataSpoofing = file.Bind(
			"PackLoading",
			nameof(MetadataSpoofing),
			true,
			"Should SoundAPI use a fake BepInPlugin attribute when generating configs? This can fix some issues with mod managers, notably with Gale displaying the config file name, instead of the sound-pack name."
		).Value;
		SkipUnusedSounds = file.Bind(
			"PackLoading",
			nameof(SkipUnusedSounds),
			true,
			"Should SoundAPI attempt to skip loading sounds that use conditions that change very little? Most notably sounds that are enabled through config settings. Using an external mod to toggle these configs will not work properly."
		).Value;
	}
}