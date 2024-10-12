using System;
using loaforcsSoundAPI.LethalCompany.Conditions.Contexts;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions;

[SoundAPICondition("LethalCompany:player:standing_on")]
public class PlayerStandingOnCondition : Condition<PlayerContext> {
	public string Value { get; internal set; }
    
	protected override bool EvaluateWithContext(PlayerContext context) {
		if (!context.Player) return false;

		return string.Equals(Value, StartOfRound.Instance.footstepSurfaces[context.Player.currentFootstepSurfaceIndex].surfaceTag, StringComparison.InvariantCultureIgnoreCase);
	}
	
	protected override bool EvaluateFallback(IContext context) {
		if (!GameNetworkManager.Instance) return false;
		return EvaluateWithContext(new PlayerContext(GameNetworkManager.Instance.localPlayerController));
	}
}