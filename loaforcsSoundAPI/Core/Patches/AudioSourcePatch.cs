using System;
using HarmonyLib;
using loaforcsSoundAPI.SoundPacks;
using UnityEngine;
using UnityEngine.Experimental.Audio;

namespace loaforcsSoundAPI.Core.Patches;

[HarmonyPatch(typeof(AudioSource))]
static class AudioSourcePatch {
	[HarmonyPrefix,
	 HarmonyPatch(nameof(AudioSource.Play), new Type[] { }),
	 HarmonyPatch(nameof(AudioSource.Play), [ typeof(ulong) ]),
	 HarmonyPatch(nameof(AudioSource.Play), [ typeof(double) ] )
	]
	static bool Play(AudioSource __instance) {
		if(SoundReplacementHandler.TryReplaceAudio(__instance, __instance.clip, out AudioClip replacement)) {
			if (replacement == null) return false;
			__instance.clip = replacement;
		}
		
		return true;
	}

	[HarmonyPrefix, HarmonyPatch(nameof(AudioSource.PlayOneShot), [ typeof(AudioClip), typeof(float) ])]
	static bool PlayOneShot(AudioSource __instance, ref AudioClip clip) {
		if (SoundReplacementHandler.TryReplaceAudio(__instance, clip, out AudioClip replacement)) {
			if (replacement == null) return false;
			clip = replacement;
		}
		
		return true;
	}
}