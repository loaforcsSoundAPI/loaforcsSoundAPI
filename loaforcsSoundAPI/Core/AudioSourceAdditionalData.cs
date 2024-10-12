using System;
using System.Collections.Generic;
using System.Linq;
using loaforcsSoundAPI.SoundPacks;
using loaforcsSoundAPI.SoundPacks.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using UnityEngine;

namespace loaforcsSoundAPI.Core;

public class AudioSourceAdditionalData(AudioSource source) {
	public AudioSource Source { get; } = source;
	
	internal SoundReplacementGroup ReplacedWith { get; set; }

	public IContext CurrentContext { get; set; }
	
	internal void Update() {
		if(!Source) return;
		if(!Source.enabled) return;
		if(ReplacedWith == null) return;
		if (!Source.isPlaying) {
			ReplacedWith = null;
			return;
		}
		
		if(!ReplacedWith.Parent.UpdateEveryFrame) return;

		IContext context = CurrentContext ?? SoundReplacementHandler.DEFAULT_CONTEXT;
		
		SoundInstance sound = ReplacedWith.Sounds.FirstOrDefault(x => x.Evaluate(context));
		if(sound == null) return;
		if(sound.Clip == Source.clip) return;
		
		float currentTime = Source.time;
		Source.clip = sound.Clip;
		Source.Play();
		Source.time = currentTime;
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