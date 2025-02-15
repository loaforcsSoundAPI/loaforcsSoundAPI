using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace loaforcsSoundAPI.SoundPacks.Data.Conditions;

/// <summary>
/// Marks a class with a condition ID for use in SoundAPI.RegisterAll
/// </summary>
/// <seealso cref="SoundAPI"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
[MeansImplicitUse]
public class SoundAPIConditionAttribute(string id, bool deprecated = false, string deprecationReason = null) : Attribute {
	/// <summary>
	/// Unique condition ID
	/// </summary>
	public string ID { get; private set; } = id;

	/// <summary>
	/// Is this condition deprecated and should throw a warning when used?
	/// </summary>
	public bool IsDeprecated { get; private set; } = deprecated;

	/// <summary>
	/// Optional property to provide more reason for a condition to be deprecated.
	/// </summary>
	public string DeprecationReason { get; private set; } = deprecationReason;
}