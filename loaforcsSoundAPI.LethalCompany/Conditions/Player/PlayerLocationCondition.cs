using GameNetcodeStuff;
using loaforcsSoundAPI.LethalCompany.Conditions.Contexts;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Player;

[SoundAPICondition("LethalCompany:player:location")]
public class PlayerLocationCondition : Condition<PlayerContext> {
	public enum LocationType {
		INSIDE,
		ON_SHIP,
		OUTSIDE
	}
    
	public LocationType Value { get; internal set; }

	protected override bool EvaluateWithContext(PlayerContext context) {
		if(!context.Player) return false;
		if(context.Player.isPlayerDead) return false;
		if(context.Player.isInsideFactory) {
			return Value == LocationType.INSIDE;
		}
		if(StartOfRound.Instance.shipBounds.bounds.Contains(context.Player.transform.position)) {
			return Value == LocationType.ON_SHIP;
		}
		return Value == LocationType.OUTSIDE;
	}

	protected override bool EvaluateFallback(IContext context) {
		if (!GameNetworkManager.Instance) return false;
		return EvaluateWithContext(new PlayerContext(GameNetworkManager.Instance.localPlayerController));
	}
}