using BepInEx.Configuration;

namespace loaforcsSoundAPI.Core.Networking;

/// <summary>
/// Implements cross-platform syncing for SoundAPI.
/// </summary>
public abstract class NetworkAdapter {
	/// <summary>
	/// Name for this Sync Adapater.
	/// </summary>
	public abstract string Name { get; }

	public abstract void OnRegister();
}