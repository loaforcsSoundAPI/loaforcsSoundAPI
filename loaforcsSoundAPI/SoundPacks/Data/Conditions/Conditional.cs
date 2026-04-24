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

		// todo: fixme, this is dumb
		// i have no clue why this isn't working properly, it should be set the moment it gets loaded in JSONDataLoader but for some reason it's still null
		Condition.Parent = this;
		Condition.OnRegistered();
		return Condition.Validate();
	}

	public abstract SoundPack Pack { get; set; }

	public void OnRegistered() {
		Condition.OnRegistered();
		if(Condition.ShouldBeMadeConstant(Condition)) {
			Condition = Condition.Evaluate(DefaultConditionContext.DEFAULT) ? ConstantCondition.TRUE : ConstantCondition.FALSE;
		}
	}
}