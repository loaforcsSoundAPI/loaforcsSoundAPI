﻿using System.Collections.Generic;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using UnityEngine.UIElements;

namespace loaforcsSoundAPI.SoundPacks.Conditions;

/// <summary>
/// Checks if the provided config option matches the provided value
/// </summary>
/// <soundapi>
///		<type>condition</type>
///		<id>config</id>
/// </soundapi>
[SoundAPICondition("config")]
class ConfigCondition : Condition {
	/// <summary>
	/// Config name
	/// </summary>
	/// <value><see cref="string"/></value>
	/// <example>Replacements:replace_spider_sounds</example>
	public string Config { get; private set; }

	/// <summary>
	/// Value to check against.
	/// </summary>
	/// <value>matches config</value>
	/// <example>true</example>
	/// <default>defaults to `true` if bool, defaults to empty if string</default>
	public object Value { get; private set; }

	public override bool Evaluate(IContext context) {
		if(!Pack.TryGetConfigValue(Config, out object data)) return false;

		// this is bad
		if(Value == null) {
			if(data is bool booleanData) return booleanData;
			if(data is string stringData) return string.IsNullOrEmpty(stringData);
			return false;
		} else {
			if(data is bool booleanData) return booleanData == (bool)Value;
			if(data is string stringData) return stringData == (string)Value;
			return false;
		}
	}

	public override List<IValidatable.ValidationResult> Validate() {
		if(!Pack.TryGetConfigValue(Config, out object data))
			return [
				new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"Config '{Config}' does not exist on SoundPack '{Pack.Name}'")
			];

		if(Value != null && data.GetType() != Value.GetType())
			return [
				new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"Config '{Config}' has a type of: '{data.GetType()}' but the Value type is '{Value.GetType()}'!")
			];

		return [];
	}
}