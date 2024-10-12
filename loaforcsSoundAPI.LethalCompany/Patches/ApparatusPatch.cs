using HarmonyLib;
using loaforcsSoundAPI.LethalCompany.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Patches;

[HarmonyPatch(typeof(LungProp))]
static class ApparatusPatch {
	[HarmonyPatch(nameof(LungProp.EquipItem)), HarmonyPrefix, HarmonyWrapSafe]
	static void OnApparatusPull(LungProp __instance) {
		if (__instance.isLungDocked) {
			ApparatusStateCondition.CurrentApparatusPulled = true;
		}
	}
}