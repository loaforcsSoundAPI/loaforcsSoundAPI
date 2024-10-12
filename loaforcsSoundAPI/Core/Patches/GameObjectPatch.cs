using System;
using HarmonyLib;
using UnityEngine;

namespace loaforcsSoundAPI.Core.Patches;

[HarmonyPatch(typeof(GameObject))]
static class GameObjectPatch {
	[HarmonyPostfix, HarmonyPatch(nameof(GameObject.AddComponent), [ typeof(Type) ])]
	internal static void NewAudioSource(GameObject __instance, ref Component __result) {
		if (__result is not AudioSource source) return;
		
		AudioSourceAdditionalData.GetOrCreate(source);
	}
}