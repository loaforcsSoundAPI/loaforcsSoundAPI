using System;
using System.Reflection;
using HarmonyLib;
using loaforcsSoundAPI.SoundPacks;
using UnityEngine;

namespace loaforcsSoundAPI.Core.Patches;

//[HarmonyPatch(typeof(UnityEngine.Object))]
static class UnityObjectPatch {
	static void InstantiatePatch(UnityEngine.Object __result) {
		Debuggers.AudioSourceAdditionalData?.Log($"aghuobr: {__result.name}");
		if (__result is not GameObject gameObject) return;
		CheckInstantiationRecursively(gameObject);
	}
    
	internal static void Init(Harmony harmony) {
		HarmonyMethod postfixPatch = new(typeof(UnityObjectPatch).GetMethod(nameof(InstantiatePatch), BindingFlags.Static | BindingFlags.NonPublic));
		foreach (MethodInfo method in typeof(UnityEngine.Object).GetMethods()) {
			if(method.Name != nameof(UnityEngine.Object.Instantiate)) continue;
			Debuggers.AudioSourceAdditionalData?.Log($"patching {method}");

			if (method.IsGenericMethod) {
				harmony.Patch(method.MakeGenericMethod(typeof(UnityEngine.Object)), postfix: postfixPatch);
			} else {
				harmony.Patch(method, postfix: postfixPatch);
			}
		}
	}

	static void CheckInstantiationRecursively(GameObject gameObject) {
		//Debuggers.AudioSourceAdditionalData?.Log($"recursively: {gameObject.name}");
		if (gameObject.TryGetComponent(out AudioSourceAdditionalData _)) return; // already processed
		
		foreach (AudioSource source in gameObject.GetComponents<AudioSource>()) {
			AudioSourceAdditionalData.GetOrCreate(source);
			
			if(source.playOnAwake) {
				AudioClip originalClip = source.clip;
				if(SoundReplacementHandler.TryReplaceAudio(source, originalClip, out AudioClip replacement)) {
					source.Stop();
					if (replacement == null) continue;
					source.clip = replacement;
					source.Play();
				} else {
					source.clip = originalClip;
					source.Play();
				}
			}
		}

		foreach (Transform transform in gameObject.transform) {
			CheckInstantiationRecursively(transform.gameObject);
		}
	}
}