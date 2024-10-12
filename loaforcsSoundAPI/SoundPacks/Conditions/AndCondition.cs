using System.Collections.Generic;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

[SoundAPICondition("and")]
class AndCondition : Condition<DefaultConditionContext> {
	public Condition[] Conditions { get; private set; }

	protected internal override void OnRegistered() {
		foreach (Condition condition in Conditions) {
			condition.Parent = Parent;
			condition.OnRegistered();
		}
	}

	protected override bool EvaluateWithContext(DefaultConditionContext context) {
		foreach (Condition condition in Conditions) {
			if (condition is InvalidCondition) return false;
			if (!condition.Evaluate(context))
				return false; // short-cut
		}

		return true;
	}

	public override List<IValidatable.ValidationResult> Validate() {
		if (Conditions.Length == 0) {
			return [
				new IValidatable.ValidationResult(IValidatable.ResultType.WARN, "'and' condition has no conditions and will always return true!")
			];
		}

		return [];
	}
}