using LethalLevelLoader;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.OtherMods.LethalLevelLoader;

public class LLLDungeonTagCondition : Condition {
	public string Value { get; internal set; }
	
	public override bool Evaluate(IContext context) {
		if (!RoundManager.Instance) return false;
		if (!PatchedContent.TryGetExtendedContent(
			RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow, 
			out ExtendedDungeonFlow lllDungeon)
		) return false;

		return lllDungeon.TryGetTag(Value);
	}
}