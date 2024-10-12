using System;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions;

[SoundAPICondition("LethalCompany:dungeon:name")]
[SoundAPICondition(
	"LethalCompany:dungeon_name", 
	true, 
	"Use 'LethalCompany:dungeon:name' instead. Will be removed on full release."
)]
public class DungeonNameCondition : Condition {
	public string Value { get; internal set; }
	
	public override bool Evaluate(IContext context) {
		if (!RoundManager.Instance) return false;
		string dungeonName = RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow.name;
		return string.Equals(Value, dungeonName, StringComparison.InvariantCultureIgnoreCase);
	}
}