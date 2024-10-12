using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.LethalCompany.Conditions.Contexts;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Player;

[SoundAPICondition("LethalCompany:player:insanity")]
public class PlayerInsanityCondition : Condition<PlayerContext> {
	public string Value { get; internal set; }
	
	protected override bool EvaluateWithContext(PlayerContext context) {
		if(!context.Player) return false;
		if(context.Player.isPlayerDead) return false;
		
		return EvaluateRangeOperator(context.Player.insanityLevel, Value);
	}

	protected override bool EvaluateFallback(IContext context) {
		if (!GameNetworkManager.Instance) return false;
		return EvaluateWithContext(new PlayerContext(GameNetworkManager.Instance.localPlayerController));
	}

	public override List<IValidatable.ValidationResult> Validate() {
		if (!ValidateRangeOperator(Value, out IValidatable.ValidationResult result))
			return [result];
		return [];
	}
}