using HarmonyLib;
using loaforcsSoundAPI.LethalCompany.Conditions;
using loaforcsSoundAPI.Reporting;

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
			if(!loaforcsSoundAPILethalCompany.foundFootstepSurfaces.Contains(surface))
				loaforcsSoundAPILethalCompany.foundFootstepSurfaces.Add(surface);
		}
	}
}