using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using Newtonsoft.Json;

namespace loaforcsSoundAPI.SoundPacks.Data.Conditions;

public abstract class Conditional : IValidatable, IPackData, IRegistrationCallback {
	public Condition Condition { get; private set; }

	public bool Evaluate(IContext context) {
		if(Condition == null) return true;
		return Condition.Evaluate(context);
	}

	public virtual List<IValidatable.ValidationResult> Validate() {
		if(Condition == null) return [ ];

		// todo: this is still necessary somehow
		Condition.Parent = this;
		Condition.OnRegistered();

		return Condition.Validate();
	}

	public abstract SoundPack Pack { get; set; }

	public virtual void OnRegistered() {
		if(Condition == null) return;
		Condition.Parent = this;
		Condition.OnRegistered();
		if(Condition.ShouldBeMadeConstant(Condition)) {
			Condition = Condition.Evaluate(DefaultConditionContext.DEFAULT) ? ConstantCondition.TRUE : ConstantCondition.FALSE;
		}
	}

	internal bool ShouldSkip() {
		if(Condition is ConstantCondition constant) {
			return !constant.Value;
		}

		return false;
	}
}