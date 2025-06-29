using BepInEx.Configuration;

namespace loaforcsSoundAPI.SoundPacks;

static class PackLoadingConfig {
	internal static bool MetadataSpoofing { get; private set; }

	internal static void Bind(ConfigFile file) {
		MetadataSpoofing = file.Bind("PackLoading", nameof(MetadataSpoofing), true, "Should SoundAPI use a fake BepInPlugin attribute when generating configs? This can fix some issues with mod managers, notably with Gale displaying the config file name, instead of the sound-pack name.").Value;
	}
}