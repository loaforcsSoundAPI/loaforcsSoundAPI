using GameNetcodeStuff;
using HarmonyLib;
using loaforcsSoundAPI.Reporting;

namespace loaforcsSoundAPI.LethalCompany.Patches;

[HarmonyPatch(typeof(AudioReverbTrigger))]
static class AudioReverbTriggerPatch {
	[HarmonyPatch(nameof(AudioReverbTrigger.ChangeAudioReverbForPlayer)), HarmonyPostfix, HarmonyWrapSafe]
	static void LogFoundReverbPreset(AudioReverbTrigger __instance) {
		if(SoundReportHandler.CurrentReport == null) return;
		if(__instance.reverbPreset == null) return;
		
		if(!loaforcsSoundAPILethalCompany.foundReverbPresets.Contains(__instance.reverbPreset))
			loaforcsSoundAPILethalCompany.foundReverbPresets.Add(__instance.reverbPreset);
	}
}