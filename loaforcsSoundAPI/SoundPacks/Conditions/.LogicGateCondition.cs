using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

public abstract class LogicGateCondition : Condition {
    public Condition[] Conditions { get; private set; }
    protected abstract string ValidateWarnMessage { get; }
    
    protected internal override void OnRegistered() {
        foreach (Condition condition in Conditions) {
            condition.Parent = Parent;
            condition.OnRegistered();
        }
    }
    
    public override List<IValidatable.ValidationResult> Validate() {
        if (Conditions.Length == 0) {
            return [
                new IValidatable.ValidationResult(IValidatable.ResultType.WARN, ValidateWarnMessage)
            ];
        }

        return [];
    }
    
    protected static bool And(Condition[] conditions, IContext context) {
        foreach (Condition condition in conditions) {
            if (condition is InvalidCondition) return false;
            if (!condition.Evaluate(context))
                return false; // short-cut
        }

        return true;
    }
    
    protected static bool Or(Condition[] conditions, IContext context) {
        foreach (Condition condition in conditions) {
            if (condition is InvalidCondition) return false;
            if (condition.Evaluate(context))
                return true; // short-cut
        }

        return false;
    }
}