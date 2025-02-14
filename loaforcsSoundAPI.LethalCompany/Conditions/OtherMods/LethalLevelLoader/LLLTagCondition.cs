using System;
using LethalLevelLoader;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.OtherMods.LethalLevelLoader;

public class LLLTagCondition<T>(Func<T> generator) : Condition where T : ExtendedContent {
	[field: NonSerialized]
	Func<T> _generator = generator;
	
	public string Value { get; internal set; }
	
	public override bool Evaluate(IContext context) {
		T content = _generator();
		return content && content.TryGetTag(Value);
	}
}