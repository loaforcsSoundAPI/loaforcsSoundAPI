using loaforcsSoundAPI.LethalCompany.Conditions.Contexts;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Player;

[SoundAPICondition("LethalCompany:player:is_alone")]
public class PlayerAloneCondition : Condition<PlayerContext> {
	public bool? Value { get; internal set; }
    
	protected override bool EvaluateWithContext(PlayerContext context) {
		if (!context.Player) return false;

		return context.Player.isPlayerAlone == (Value ?? true);
	}
	
	protected override bool EvaluateFallback(IContext context) {
		if (!GameNetworkManager.Instance) return false;
		return EvaluateWithContext(new PlayerContext(GameNetworkManager.Instance.localPlayerController));
	}
}