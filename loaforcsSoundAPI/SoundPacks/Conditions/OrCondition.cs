using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

[SoundAPICondition("or")]
class OrCondition : Condition<DefaultConditionContext> {
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
			if (condition.Evaluate(context))
				return true; // short-cut
		}

		return false;
	}

	public override List<IValidatable.ValidationResult> Validate() {
		if (Conditions.Length == 0) {
			return [
				new IValidatable.ValidationResult(IValidatable.ResultType.WARN, "'or' condition has no conditions and will always return false!")
			];
		}

		return [];
	}
}