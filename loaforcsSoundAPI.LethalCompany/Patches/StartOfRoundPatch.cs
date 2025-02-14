using HarmonyLib;
using loaforcsSoundAPI.LethalCompany.Conditions;
using loaforcsSoundAPI.LethalCompany.Reporting;
using loaforcsSoundAPI.Reporting;
using loaforcsSoundAPI.Core.Util.Extensions;

namespace loaforcsSoundAPI.LethalCompany.Patches;

[HarmonyPatch(typeof(StartOfRound))]
static class StartOfRoundPatch {
	[HarmonyPrefix, HarmonyPatch(nameof(StartOfRound.EndOfGame)), HarmonyWrapSafe]
	static void ResetApparatusState() {
		ApparatusStateCondition.CurrentApparatusPulled = false;
	}

	[HarmonyPostfix, HarmonyPatch(nameof(StartOfRound.Awake)), HarmonyWrapSafe]
	static void ReportFootstepSurfaces() {
		if(SoundReportHandler.CurrentReport == null) return;

		foreach (FootstepSurface surface in StartOfRound.Instance.footstepSurfaces) {
			LethalCompanySoundReport.foundFootstepSurfaces.AddUnique(surface);
		}
	}

	// todo
	/*
	[HarmonyPostfix, HarmonyPatch(nameof(StartOfRound.OnPlayerConnectedClientRpc)), HarmonyWrapSafe]
	static void PlayShipSpeakerOnClientJoin(StartOfRound __instance) {
		if(!SoundFixesConfig.PlayShipSpeakerOnClientJoin.Value) return;
		if(__instance.IsServer || __instance.IsHost) return;
		if (__instance.gameStats.daysSpent == 0) {
			__instance.speakerAudioSource.PlayOneShot(__instance.shipIntroSpeechSFX);
		}
	}
	*/
}