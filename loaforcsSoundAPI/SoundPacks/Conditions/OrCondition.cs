using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.Core.Util;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

[SoundAPICondition("or")]
class OrCondition : LogicGateCondition {
	protected override string ValidateWarnMessage => "'or' condition has no conditions and will always return false!";

	public override bool Evaluate(IContext context) {
		return Or(Conditions, context);
	}
}

[SoundAPICondition("nor")]
class NorCondition : LogicGateCondition {
	protected override string ValidateWarnMessage => "'nor' condition has no conditions and will always return true!";

	public override bool Evaluate(IContext context) {
		return !Or(Conditions, context);
	}
}