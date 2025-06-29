using System;
using System.Collections.Generic;
using System.Linq;
using loaforcsSoundAPI.Core.Patches;
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

	SoundReplacementGroup _replacedWith;

	public AudioClip OriginalClip { get; internal set; }

	public AudioClip RealClip {
		get => AudioSourcePatch.GetRealClip(Source);
		set => AudioSourcePatch.SetRealClip(Source, value);
	}

	internal SoundReplacementGroup ReplacedWith {
		get => _replacedWith;
		set {
			_replacedWith = value;

			// todo: kind of icky just modifying the list raw
			if(RequiresUpdateFunction()) {
				if(SoundAPIAudioManager.liveAudioSourceData.Contains(this)) return; // dont add to list twice

				SoundAPIAudioManager.liveAudioSourceData.Add(this);
			} else if(SoundAPIAudioManager.liveAudioSourceData.Contains(this)) {
				SoundAPIAudioManager.liveAudioSourceData.Remove(this);
			}
		}
	}

	/// <summary>
	/// Should SoundAPI ignore replacing for this Audio Source?
	/// </summary>
	public bool DisableReplacing { get; private set; }

	/// <summary>
	/// Current Context, may be null.
	/// </summary>
	public IContext CurrentContext { get; set; }

	internal void Update() {
		if(!RequiresUpdateFunction() || !AudioSourceIsPlaying()) return;

		Debuggers.UpdateEveryFrame?.Log($"success: updating every frame for {Source.name}");

		IContext context = CurrentContext ?? DefaultConditionContext.DEFAULT;

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

	bool RequiresUpdateFunction() {
		return ReplacedWith != null && ReplacedWith.Parent.UpdateEveryFrame;
	}

	bool AudioSourceIsPlaying() {
		return Source && Source.enabled && Source.isPlaying;
	}

	public static AudioSourceAdditionalData GetOrCreate(AudioSource source) {
		if(SoundAPIAudioManager.audioSourceData.TryGetValue(source, out AudioSourceAdditionalData sourceData)) return sourceData;

		sourceData = new AudioSourceAdditionalData(source);
		SoundAPIAudioManager.audioSourceData[source] = sourceData;

		return sourceData;
	}
}