﻿using System;
using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

[SoundAPICondition("mod_installed")]
class ModInstalledCondition : Condition<DefaultConditionContext> {
	public string Value { get; private set; }
	
	protected override bool EvaluateWithContext(DefaultConditionContext context) {
		return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(Value);
	}
	
	public override List<IValidatable.ValidationResult> Validate() {
		if (string.IsNullOrEmpty(Value))
			return [
				new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"Value on 'mod_installed' must be there and must not be empty.")
			];

		return [];

	}
}