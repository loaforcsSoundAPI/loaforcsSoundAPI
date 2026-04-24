using System.Collections.Generic;
using System.Linq;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

public abstract class LogicGateCondition : Condition {
	public abstract Condition[] Conditions { get; protected set; }

	protected abstract string ValidateWarnMessage { get; }

	public override void OnRegistered() {
		for(int i = 0; i < Conditions.Length; i++) {
			Condition condition = Conditions[i];
			condition.Parent = Parent;
			condition.OnRegistered();
			if(ShouldBeMadeConstant(condition)) {
				Conditions[i] = condition.Evaluate(DefaultConditionContext.DEFAULT) ? ConstantCondition.TRUE : ConstantCondition.FALSE;
			}
		}
	}

	/// <inheritdoc/>
	public override bool CanBeImpliedConstant() {
		return Conditions.All(ShouldBeMadeConstant);
	}

	public override List<IValidatable.ValidationResult> Validate() {
		if(Conditions.Length == 0) {
			return [
				new IValidatable.ValidationResult(IValidatable.ResultType.WARN, ValidateWarnMessage)
			];
		}

		List<IValidatable.ValidationResult> results = [ ];
		foreach(Condition condition in Conditions) {
			results.AddRange(condition.Validate());
		}

		return results;
	}

	protected static bool And(Condition[] conditions, IContext context) {
		foreach(Condition condition in conditions) {
			if(condition is InvalidCondition) return false;
			if(!condition.Evaluate(context)) {
				return false; // short-cut
			}
		}

		return true;
	}

	protected static bool Or(Condition[] conditions, IContext context) {
		foreach(Condition condition in conditions) {
			if(condition is InvalidCondition) return false;
			if(condition.Evaluate(context)) {
				return true; // short-cut
			}
		}

		return false;
	}
}