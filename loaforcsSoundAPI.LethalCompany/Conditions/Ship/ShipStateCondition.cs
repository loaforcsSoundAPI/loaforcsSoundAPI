using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Ship;

[SoundAPICondition("LethalCompany:ship:state")]
public class ShipStateCondition : Condition {
	public enum ShipStateType {
		IN_ORBIT,
		LANDED
	}
	
	public ShipStateType Value { get; internal set; }


	public override bool Evaluate(IContext context) {
		if (!StartOfRound.Instance) return false;
		if (StartOfRound.Instance.inShipPhase) {
			return Value == ShipStateType.IN_ORBIT;
		} else {
			return Value == ShipStateType.LANDED;
		}
	}
	// todo: validate
}