using System;
using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Moon;

[SoundAPICondition("LethalCompany:moon:current_time")]
public class CurrentTimeCondition : Condition {
    public string Value { get; internal set; }

    public override bool Evaluate(IContext context) {
        if (!TimeOfDay.Instance) return false;
        return false; // todo: do this lol
    }
    
    public override List<IValidatable.ValidationResult> Validate() {
        if (!ValidateRangeOperator(Value, out IValidatable.ValidationResult result))
            return [result];
        return [];
    }
}