using System;
using System.Linq;
using BepInEx.Configuration;

namespace loaforcsSoundAPI.Core.Util;

// todo: generalize maybe??
public class AdaptiveConfigEntry {
    public AdaptiveConfigEntry(AdaptiveBool state, bool defaultValue) {
        State = state;
        DefaultValue = defaultValue;
    }

    public AdaptiveBool State { get; private set; }
    public bool DefaultValue { get; private set; }
    
    public bool? OverrideValue { get; set; } // todo: add log message when two overrides happen.
    
    public bool Value {
        get {
            switch (State) {
                case AdaptiveBool.Enabled:
                    return true;
                case AdaptiveBool.Disabled:
                    return false;
                default:
                case AdaptiveBool.Automatic:
                    return OverrideValue ?? DefaultValue;
            }
        }
    }
}

public enum AdaptiveBool {
    Automatic,
    Enabled,
    Disabled
}