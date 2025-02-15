using BepInEx.Configuration;

namespace loaforcsSoundAPI.Core.Util.Extensions;

public static class ConfigFileExtensions {
    public static AdaptiveConfigEntry BindAdaptive(this ConfigFile file, string section, string key, bool defaultValue, string description) {
        AdaptiveBool state = file.Bind(section, key, AdaptiveBool.Automatic, new ConfigDescription(
                $"{description}\nAutomatic default: {defaultValue}"
            )).Value;

        return new AdaptiveConfigEntry(state, defaultValue);
    }
}