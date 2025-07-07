using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

/// <summary>
/// Increments a counter by one every time this condition is evaluated.
///
/// For the following example, it will trigger once every 5 times.
/// Be careful when using with `and`, `nand`, `or` or `nor` as these have performance optimizations that may skip increasing the counter in some cases.
/// </summary>
/// <soundapi>
///		<type>condition</type>
///		<id>counter</id>
/// </soundapi>
[SoundAPICondition("counter")]
public class CounterCondition : Condition {
	/// <summary>
	/// Range of values to check against
	/// </summary>
	/// <value>ValueRange</value>
	/// <example>1</example>
	public string Value { get; private set; }

	/// <summary>
	/// Resets after reaching this number. Inclusive.
	/// </summary>
	/// <value><see cref="int"/></value>
	/// <example>5</example>
	public int? ResetsAt { get; private set; }

	int _count;

	public override bool Evaluate(IContext context) {
		LogDebug("counter", $"counting: {_count} -> {_count + 1}");
		_count++;
		bool result = EvaluateRangeOperator(_count, Value);
		LogDebug("counter", $"is {_count} in range ({Value})? {result}");
		if(_count >= ResetsAt) {
			_count = 0;
			LogDebug("counter", $"reset count to 0.");
		}

		return result;
	}

	public override List<IValidatable.ValidationResult> Validate() {
		if(!ValidateRangeOperator(Value, out IValidatable.ValidationResult result))
			return [result];
		return [];
	}
}