using LethalLevelLoader;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.OtherMods.LethalLevelLoader;

public class LLLMoonTagCondition : Condition {
	public string Value { get; internal set; }
	
	public override bool Evaluate(IContext context) {
		if (!StartOfRound.Instance) return false;
		if (!PatchedContent.TryGetExtendedContent(
			StartOfRound.Instance.currentLevel, 
			out ExtendedLevel lllMoon)
		) return false;

		return lllMoon.TryGetTag(Value);
	}
}