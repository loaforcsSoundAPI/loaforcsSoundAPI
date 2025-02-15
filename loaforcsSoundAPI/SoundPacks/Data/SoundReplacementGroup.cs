using System;
using System.Collections.Generic;
using System.Linq;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using Newtonsoft.Json;

namespace loaforcsSoundAPI.SoundPacks.Data;

public class SoundReplacementGroup : Conditional {
	[JsonConstructor]
	internal SoundReplacementGroup() { }

	public SoundReplacementGroup(SoundReplacementCollection parent, List<string> matches) {
		Parent = parent;
		Matches = matches;

		if (SoundPackDataHandler.LoadedPacks.Contains(parent.Pack)) {
			throw new InvalidOperationException("SoundPack has already been registered, trying to add a new SoundReplacementGroup does not work!");
		}
		
		parent.AddSoundReplacementGroup(this);
	}

	internal void AddSoundReplacement(SoundInstance sound) {
		Sounds.Add(sound);
	}
	
	[field:NonSerialized]
	public SoundReplacementCollection Parent { get; internal set; }
    
	public List<string> Matches { get; private set; }
	public List<SoundInstance> Sounds { get; private set; } = [];
	
	
	public override List<IValidatable.ValidationResult> Validate() {
		List<IValidatable.ValidationResult> results = base.Validate();

		foreach (string match in Matches) {
			if (string.IsNullOrEmpty(match)) {
				results.Add(new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, "Match string can not be empty!"));
				continue;
			}
            
			string[] processed = match.Split(":");

			if (processed.Length == 1) {
				// could maybe swap to a warning? but like it won't work at all with only one part to the match string so i think it should be a fail.
				results.Add(new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"'{match}' is not valid! If you mean to match to all Audio clips with this name you must explicitly do '*:{match}'."));
			}

			if (processed.Length > 3) {
				results.Add(new IValidatable.ValidationResult(IValidatable.ResultType.WARN, $"'{match}' has more than 3 parts! SoundAPI will handle this as '{match[0]}:{match[1]}:{match[2]}', discarding the rest!"));
			}
		}
		
		return results;
	}

	public override SoundPack Pack {
		get => Parent.Pack;
		set {
			if (Parent.Pack != null) throw new InvalidOperationException("Pack has already been set.");
			Parent.Pack = value;
		}
	}
}