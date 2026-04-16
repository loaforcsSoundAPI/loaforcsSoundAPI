using System;

namespace loaforcsSoundAPI;

/// <summary>
/// <see cref="SoundAPI"/>
/// </summary>
[Flags]
public enum AudioSourceCopyFlags {
	DontCopyPlayOnAwake = 1 << 0
}