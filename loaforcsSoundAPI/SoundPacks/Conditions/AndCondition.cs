using System.Collections.Generic;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.Core.Util;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

[SoundAPICondition("and")]
class AndCondition : LogicGateCondition {
	protected override string ValidateWarnMessage => "'and' condition has no conditions and will always return true!";

	public override bool Evaluate(IContext context) {
		return And(Conditions, context);
	}
}

[SoundAPICondition("nand")]
class NandCondition : LogicGateCondition {
	protected override string ValidateWarnMessage => "'nand' condition has no conditions and will always return false!";

	public override bool Evaluate(IContext context) {
		return !And(Conditions, context);
	}
}