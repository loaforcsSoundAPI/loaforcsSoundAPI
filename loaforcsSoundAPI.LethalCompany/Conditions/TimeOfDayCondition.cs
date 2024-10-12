using System;
using GameNetcodeStuff;
using loaforcsSoundAPI.LethalCompany.Conditions.Contexts;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions;

[SoundAPICondition("LethalCompany:time_of_day")]
public class TimeOfDayCondition : Condition {
	public string Value { get; internal set; }

	public override bool Evaluate(IContext context) {
		if (!TimeOfDay.Instance) return false;
		return string.Equals(Value, TimeOfDay.Instance.dayMode.ToString(), StringComparison.InvariantCultureIgnoreCase);
	}
}