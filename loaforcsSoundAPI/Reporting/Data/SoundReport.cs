using System;
using System.Collections.Generic;
using System.IO;

namespace loaforcsSoundAPI.Reporting.Data;

/// <summary>so
/// A Sound-report instance.
/// </summary>
public class SoundReport {
	public class PlayedSound(string matchString, string caller, bool isPlayOnAwake) {
		public string MatchString { get; private set; } = matchString;
		public string Caller { get; private set; } = caller;
		public bool IsPlayOnAwake { get; private set; } = isPlayOnAwake;
		
		public override bool Equals(object obj) {
			if (obj is not PlayedSound other) return false;
			return Equals(other);
		}

		protected bool Equals(PlayedSound other) {
			return MatchString == other.MatchString && Caller == other.Caller && IsPlayOnAwake == other.IsPlayOnAwake;
		}

		public override int GetHashCode() {
			return HashCode.Combine(MatchString, Caller, IsPlayOnAwake);
		}

		public string FormatForReport() {
			return $"Match String: {MatchString}, Caller: {Caller}, IsPlayOnAwake: {IsPlayOnAwake}";
		}
	}
	
	public DateTime StartedAt { get; private set; } = DateTime.Now;

	public List<PlayedSound> PlayedSounds { get; private set; } = [];
    
	public List<string> SoundPackNames { get; private set; } = [];

	public int AudioClipsLoaded { get; set; }
}