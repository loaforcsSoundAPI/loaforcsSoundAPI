using System;
using GameNetcodeStuff;
using loaforcsSoundAPI.LethalCompany.Conditions.Contexts;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Moon;

[SoundAPICondition("LethalCompany:moon:time_of_day")]
[SoundAPICondition("LethalCompany:time_of_day", 
	true, 
	"Use 'LethalCompany:moon:time_of_day' instead. Will be removed on full release."
)]
public class TimeOfDayCondition : Condition {
	public string Value { get; internal set; }

	public override bool Evaluate(IContext context) {
		if (!TimeOfDay.Instance) return false;
		return string.Equals(Value, TimeOfDay.Instance.dayMode.ToString(), StringComparison.InvariantCultureIgnoreCase);
	}
	
	// todo: validate
}