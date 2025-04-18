﻿namespace loaforcsSoundAPI.SoundPacks.Data.Conditions;

/// <summary>
/// Context interface.
/// </summary>
public interface IContext;

class DefaultConditionContext : IContext {
    DefaultConditionContext() { }

    internal static readonly DefaultConditionContext DEFAULT = new();
}