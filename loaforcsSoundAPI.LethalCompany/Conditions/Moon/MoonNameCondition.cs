using System;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions;

[SoundAPICondition("LethalCompany:moon_name", true, "Use 'LethalCompany:moon:name' instead. Will be removed on full release.")]
[SoundAPICondition("LethalCompany:moon:name")]
public class MoonNameCondition : Condition {
	public string Value { get; internal set; }
	
	public override bool Evaluate(IContext context) {
		if (!StartOfRound.Instance) return false;
		string moonName = StartOfRound.Instance.currentLevel.name;
		
		return string.Equals(Value, moonName, StringComparison.InvariantCultureIgnoreCase);
	}
}