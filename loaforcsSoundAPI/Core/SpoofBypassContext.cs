using System;
using loaforcsSoundAPI.Core.Patches;

namespace loaforcsSoundAPI.Core;

class SpoofBypassContext : IDisposable {
	public SpoofBypassContext() {
		AudioSourcePatch.bypassSpoofing = true;
	}

	public void Dispose() {
		AudioSourcePatch.bypassSpoofing = false;
	}
}