using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using Newtonsoft.Json;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

[SoundAPICondition("not")]
class NotCondition : Condition<DefaultConditionContext> {
	[JsonProperty("condition")]
	public Condition Condition { get; private set; }
    
	protected internal override void OnRegistered() {
		if (Condition != null) {
			Condition.Parent = Parent;
			Condition.OnRegistered();
		}
	}
    
	protected override bool EvaluateWithContext(DefaultConditionContext context) {
		if (Condition is InvalidCondition) return false;
		return !Condition.Evaluate(context);
	}

	public override List<IValidatable.ValidationResult> Validate() {
		if (Condition == null) {
			return [
				new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, "'not' condition has no valid condition to invert!")
			];
		}

		return [];
	}
}