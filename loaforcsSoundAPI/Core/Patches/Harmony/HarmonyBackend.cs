using HarmonyLib;
using loaforcsSoundAPI.SoundPacks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace loaforcsSoundAPI.Core.Patches.Harmony;

// Used if NativeBackend is not avaliable
static class HarmonyBackend {
	internal static void Init(HarmonyLib.Harmony harmony) {
		harmony.PatchAll(typeof(HarmonyBackend));
		//harmony.Patch(AccessTools.Method(typeof(AudioSource), nameof(AudioSource.Play)), new HarmonyMethod(AccessTools.Method(typeof(HarmonyBackend), nameof(AudioSource.Play))));
		UnityObjectPatch.Init(harmony);
	
		SceneManager.sceneLoaded += (scene, _) => {
			// run goofy loop on everything
			SoundAPIAudioManager.RunCleanup();

			foreach(AudioSource source in Object.FindObjectsOfType<AudioSource>(true)) {
				if(source.gameObject.scene != scene) continue; // already processed
				CheckAudioSource(source);
			}
		};
	}

	internal static void CheckAudioSource(AudioSource source) {
		AudioSourceAdditionalData data = AudioSourceAdditionalData.GetOrCreate(source);

		if(!source.playOnAwake) return;
		if(!source.isActiveAndEnabled) return;
		if(data.RealClip) {
			source.Play();
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), [ ])]
	[HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), typeof(ulong))]
	[HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), typeof(double))]
	public static bool Play(AudioSource __instance) {
		Debuggers.SoundReplacementHandler?.Log("HarmonyX Backend: AudioSource.Play patch");
		AudioSourceAdditionalData data = AudioSourceAdditionalData.GetOrCreate(__instance);

		if(SoundReplacementHandler.TryReplaceAudio(__instance, data.OriginalClip, out AudioClip replacement)) {
			if(replacement == null) return false;
			data.RealClip = replacement;
		}

		return true;
	}
}