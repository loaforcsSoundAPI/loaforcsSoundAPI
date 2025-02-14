using GameNetcodeStuff;
using HarmonyLib;
using loaforcsSoundAPI.LethalCompany.Reporting;
using loaforcsSoundAPI.Reporting;
using loaforcsSoundAPI.Core.Util.Extensions;

namespace loaforcsSoundAPI.LethalCompany.Patches;

[HarmonyPatch(typeof(AudioReverbTrigger))]
static class AudioReverbTriggerPatch {
	[HarmonyPatch(nameof(AudioReverbTrigger.ChangeAudioReverbForPlayer)), HarmonyPostfix, HarmonyWrapSafe]
	static void LogFoundReverbPreset(AudioReverbTrigger __instance) {
		if(SoundReportHandler.CurrentReport == null) return;
		if(__instance.reverbPreset == null) return;
		
		LethalCompanySoundReport.foundReverbPresets.AddUnique(__instance.reverbPreset);
	}
}