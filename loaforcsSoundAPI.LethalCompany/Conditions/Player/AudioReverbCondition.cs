using System;
using GameNetcodeStuff;
using JetBrains.Annotations;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Player;

[SoundAPICondition("LethalCompany:player:audio_reverb")]
public class AudioReverbCondition : Condition {
	public bool? HasEcho { get; private set; } = null;

	[CanBeNull]
	public string Name { get; private set; } = null;
	
	public override bool Evaluate(IContext context) {
		if (!GameNetworkManager.Instance) return false;
		PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
		if (!player) return false;
		if (!player.reverbPreset) return false;
		
		if (HasEcho != null) {
			if (HasEcho == player.reverbPreset.hasEcho) return true;
		}

		if (Name != null) {
			if (string.Equals(Name, player.reverbPreset.name, StringComparison.InvariantCultureIgnoreCase)) return true;
		}

		return false;
	}
}