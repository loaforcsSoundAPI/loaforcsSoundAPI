namespace loaforcsSoundAPI.SoundPacks.Data;

/// <summary>
/// Interface for data that gets loaded by a Sound-pack that needs to know what pack it was loaded by.
/// </summary>
/// <seealso cref="SoundPack"/>
public interface IPackData {
	/// <summary>
	/// Parent pack
	/// </summary>
	SoundPack Pack { get; internal set; }
}