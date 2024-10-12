using System;
using System.Collections.Generic;
using System.IO;

namespace loaforcsSoundAPI.Reporting.Data;

/// <summary>so
/// A Sound-report instance.
/// </summary>
public class SoundReport {
	public DateTime StartedAt { get; private set; } = DateTime.Now;

	public List<string> AllMatchStrings { get; private set; } = [];
	public List<string> RawMatchStrings { get; private set; } = [];
    
	public List<string> SoundPackNames { get; private set; } = [];

	public int AudioClipsLoaded { get; set; }
}