using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions;

[SoundAPICondition("LethalCompany:facility_power_state")]
public class FacilityPowerStateCondition : Condition {
	internal static bool CurrentPowerState = false;

	public bool? Value { get; internal set; }

	public override bool Evaluate(IContext context) {
		return CurrentPowerState == (Value ?? true);
	}
}