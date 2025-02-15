using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

[SoundAPICondition("counter")]
public class CounterCondition : Condition {
    public string Value { get; private set; }
    public int? ResetsAt { get; private set; }

    int _count;
    
    public override bool Evaluate(IContext context) {
        LogDebug("counter", $"counting: {_count} -> {_count + 1}");
        _count++;
        bool result = EvaluateRangeOperator(_count, Value);
        LogDebug("counter", $"is {_count} in range ({Value})? {result}");
        if (_count >= ResetsAt) {
            _count = 0;
            LogDebug("counter", $"reset count to 0.");
        }
        return result;
    }
    
    public override List<IValidatable.ValidationResult> Validate() {
        if (!ValidateRangeOperator(Value, out IValidatable.ValidationResult result))
            return [result];
        return [];
    }
}