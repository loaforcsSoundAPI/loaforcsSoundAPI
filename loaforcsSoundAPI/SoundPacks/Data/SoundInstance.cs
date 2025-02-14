using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using Newtonsoft.Json;
using UnityEngine;

namespace loaforcsSoundAPI.SoundPacks.Data;

public class SoundInstance : Conditional {
	[JsonConstructor]
	internal SoundInstance() { }

	public SoundInstance(SoundReplacementGroup parent, int weight, AudioClip clip) {
		Parent = parent;
		Weight = weight;
		Clip = clip;
		parent.AddSoundReplacement(this);
	}

	[field:NonSerialized]
	public SoundReplacementGroup Parent { get; internal set; }
    
	public string Sound { get; private set; }
	
	public int Weight { get; private set; }
	
	[field:NonSerialized]
	public AudioClip Clip { get; internal set; }

	public override List<IValidatable.ValidationResult> Validate() {
		List<IValidatable.ValidationResult> results = base.Validate();
		
		if (!File.Exists(Path.Combine(Pack.PackFolder, "sounds", Sound))) {
			results.Add(new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"Sound '{Sound}' couldn't be found or doesn't exist!"));
		} else if (!SoundPackLoadPipeline.audioExtensions.ContainsKey(Path.GetExtension(Sound))) {
			results.Add(new IValidatable.ValidationResult(IValidatable.ResultType.FAIL, $"Audio type: '{Path.GetExtension(Sound)}' is not supported!"));
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