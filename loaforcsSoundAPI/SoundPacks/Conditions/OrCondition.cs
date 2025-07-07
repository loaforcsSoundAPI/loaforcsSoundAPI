using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.Core.Util;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

/// <summary>
/// Checks if any conditions are true.
/// </summary>
/// <soundapi>
///		<type>condition</type>
///		<id>or</id>
/// </soundapi>
[SoundAPICondition("or")]
class OrCondition : LogicGateCondition {
	/// <summary>
	/// Collection of conditions
	/// </summary>
	/// <value><see cref="Condition"/></value>
	public override Condition[] Conditions { get; protected set; }

	protected override string ValidateWarnMessage => "'or' condition has no conditions and will always return false!";

	public override bool Evaluate(IContext context) {
		return Or(Conditions, context);
	}
}

/// <summary>
/// Checks if any conditions are false.
/// </summary>
/// <soundapi>
///		<type>condition</type>
///		<id>nor</id>
/// </soundapi>
[SoundAPICondition("nor")]
class NorCondition : LogicGateCondition {
	/// <summary>
	/// Collection of conditions
	/// </summary>
	/// <value><see cref="Condition"/></value>
	public override Condition[] Conditions { get; protected set; }

	protected override string ValidateWarnMessage => "'nor' condition has no conditions and will always return true!";

	public override bool Evaluate(IContext context) {
		return !Or(Conditions, context);
	}
}