using JetBrains.Annotations;
using UnityEngine;

namespace loaforcsSoundAPI.Core.Patches.Native;

public record NativeBackendSettings(
	// Fully qualified unity version
	string UnityVersion,
	NativeOffsets WindowsReleaseOffsets
) {
	public bool CurrentVersionMatches => Application.unityVersion == UnityVersion;
}

// also ghidra offsets are typically `180xxxxx`, just remove the 180 from the address
// offsets can be found easily by getting the pdb for the build (i think debug builds have this built in)
// AudioSource::Play -> AudioSource_Play
public record NativeOffsets(
	int AudioSource_Play,
	int? AudioSource_RemoveFromManager = null
);