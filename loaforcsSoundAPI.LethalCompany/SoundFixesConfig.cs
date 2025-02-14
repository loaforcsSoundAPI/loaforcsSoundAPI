using BepInEx.Configuration;
using loaforcsSoundAPI.Core.Util;
using loaforcsSoundAPI.Core.Util.Extensions;

namespace loaforcsSoundAPI.LethalCompany;

public static class SoundFixesConfig {
    const string SECTION = "SoundFixes";
    
    public static AdaptiveConfigEntry PlayShipSpeakerOnClientJoin { get; private set; }
    
    public static void Bind(ConfigFile config) {
        PlayShipSpeakerOnClientJoin = config.BindAdaptive(
            SECTION, 
            "PlayShipSpeakerOnClientJoin", 
            false,
            "Vanilla does not play the ship speaker when clients join a lobby for the first time."
        );
    }
    
}