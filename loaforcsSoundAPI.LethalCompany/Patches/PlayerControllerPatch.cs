using GameNetcodeStuff;
using HarmonyLib;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.LethalCompany.Conditions.Contexts;

namespace loaforcsSoundAPI.LethalCompany.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
static class PlayerControllerPatch {
	[HarmonyPatch(nameof(PlayerControllerB.Start))]
	static void UpdatePlayerContexts(PlayerControllerB __instance) {
		PlayerContext context = new(__instance);
		AudioSourceAdditionalData.GetOrCreate(__instance.movementAudio).CurrentContext = context;
		AudioSourceAdditionalData.GetOrCreate(__instance.statusEffectAudio).CurrentContext = context;
		AudioSourceAdditionalData.GetOrCreate(__instance.waterBubblesAudio).CurrentContext = context;
	}
}