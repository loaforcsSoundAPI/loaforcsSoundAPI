using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Dungeon;

[SoundAPICondition("LethalCompany:facility_power_state", 
	true, 
	"renamed to 'LethalCompany:dungeon:power_state' to better fit."
)]
[SoundAPICondition("LethalCompany:dungeon:power_state")]
public class DungeonPowerStateCondition : Condition {
	internal static bool CurrentPowerState = false;

	public bool? Value { get; internal set; }

	public override bool Evaluate(IContext context) {
		return CurrentPowerState == (Value ?? true);
	}
}