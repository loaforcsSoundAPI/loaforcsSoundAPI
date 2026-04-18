using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace loaforcsSoundAPI.Core.Patches.Harmony;

static class UnityObjectPatch {
	static void InstantiatePatch(Object __result) {
		Debuggers.AudioSourceAdditionalData?.Log($"aghuobr: {__result.name}");
		if(__result is not GameObject gameObject) return;
		CheckInstantiationRecursively(gameObject);
	}

	internal static void Init(HarmonyLib.Harmony harmony) {
		HarmonyMethod postfixPatch = new HarmonyMethod(typeof(UnityObjectPatch).GetMethod(nameof(InstantiatePatch), BindingFlags.Static | BindingFlags.NonPublic));
		foreach(MethodInfo method in typeof(Object).GetMethods()) {
			if(method.Name != nameof(Object.Instantiate)) continue;
			Debuggers.AudioSourceAdditionalData?.Log($"patching {method}");

			if(method.IsGenericMethod) {
				harmony.Patch(method.MakeGenericMethod(typeof(Object)), postfix: postfixPatch);
			} else {
				harmony.Patch(method, postfix: postfixPatch);
			}
		}
	}

	static void CheckInstantiationRecursively(GameObject gameObject) {
		//Debuggers.AudioSourceAdditionalData?.Log($"recursively: {gameObject.name}");

		foreach(AudioSource source in gameObject.GetComponents<AudioSource>()) {
			HarmonyBackend.CheckAudioSource(source);
		}

		foreach(Transform transform in gameObject.transform) {
			CheckInstantiationRecursively(transform.gameObject);
		}
	}
}