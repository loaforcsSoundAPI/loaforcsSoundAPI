using System.Collections.Generic;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.Core.Util;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

/// <summary>
/// Checks if all conditions are true.
/// </summary>
/// <soundapi>
///		<type>condition</type>
///		<id>and</id>
/// </soundapi>
[SoundAPICondition("and")]
class AndCondition : LogicGateCondition {
	/// <summary>
	/// Collection of conditions
	/// </summary>
	/// <value><see cref="Condition"/></value>
	public override Condition[] Conditions { get; protected set; }

	protected override string ValidateWarnMessage => "'and' condition has no conditions and will always return true!";

	public override bool Evaluate(IContext context) {
		return And(Conditions, context);
	}
}

/// <summary>
/// Checks if all conditions are false.
/// </summary>
/// <soundapi>
///		<type>condition</type>
///		<id>nand</id>
/// </soundapi>
[SoundAPICondition("nand")]
class NandCondition : LogicGateCondition {
	/// <summary>
	/// Collection of conditions
	/// </summary>
	/// <value><see cref="Condition"/></value>
	public override Condition[] Conditions { get; protected set; }

	protected override string ValidateWarnMessage => "'nand' condition has no conditions and will always return false!";

	public override bool Evaluate(IContext context) {
		return !And(Conditions, context);
	}
}