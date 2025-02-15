using System;
using System.Collections.Generic;
using System.Linq;
using loaforcsSoundAPI.SoundPacks;
using loaforcsSoundAPI.SoundPacks.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using UnityEngine;

namespace loaforcsSoundAPI.Core;

/// <summary>
/// Contains additional data for a specific audio source.
/// </summary>
public class AudioSourceAdditionalData {
	internal AudioSourceAdditionalData(AudioSource source) {
		Source = source;
	}
	
	/// <summary>
	/// AudioSource that this AdditonalData is describing.
	/// </summary>
	public AudioSource Source { get; private set; }
	
	internal SoundReplacementGroup ReplacedWith { get; set; }

	/// <summary>
	/// Should SoundAPI ignore replacing for this Audio Source?
	/// </summary>
	public bool DisableReplacing { get; private set; }
	
	/// <summary>
	/// Current Context, may be null.
	/// </summary>
	public IContext CurrentContext { get; set; }
	
	internal void Update() {
		if(ReplacedWith == null) return;
		if (!Source) {
			Debuggers.UpdateEveryFrame?.Log($"err: source is not valid!!");
			return;
		}
		
		if (!Source.isPlaying) {
			ReplacedWith = null;
			Debuggers.UpdateEveryFrame?.Log("source stopped playing, setting replaced with = null");

			return;
		}

		if (!ReplacedWith.Parent.UpdateEveryFrame) {
			Debuggers.UpdateEveryFrame?.Log($"replaced; but not update every frame: {Source.name}");

			return;
		}
		
		if (!Source.enabled) {
			Debuggers.UpdateEveryFrame?.Log($"err: source is disabled!");
			return;
		}
		
		Debuggers.UpdateEveryFrame?.Log($"success: updating every frame for {Source.name}");

		IContext context = CurrentContext ?? SoundReplacementHandler.DEFAULT_CONTEXT;
		
		SoundInstance sound = ReplacedWith.Sounds.FirstOrDefault(x => x.Evaluate(context));
		if(sound == null) return;
		if(sound.Clip == Source.clip) return;
		Debuggers.UpdateEveryFrame?.Log("new clip found, swapping!!");

		
		float currentTime = Source.time;
		Source.clip = sound.Clip;
		Source.Play();
		Source.time = currentTime;
		
		Debuggers.UpdateEveryFrame?.Log("new clip found, swapped");

	}
	
	public static AudioSourceAdditionalData GetOrCreate(AudioSource source) {
		if (SoundAPIAudioManager.audioSourceData.TryGetValue(source, out AudioSourceAdditionalData sourceData)) {
			return sourceData;
		}
		
		sourceData = new AudioSourceAdditionalData(source);
		SoundAPIAudioManager.audioSourceData[source] = sourceData;
        
		return sourceData; 
	}
}