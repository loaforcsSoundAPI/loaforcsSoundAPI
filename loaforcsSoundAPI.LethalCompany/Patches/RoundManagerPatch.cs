using HarmonyLib;
using loaforcsSoundAPI.LethalCompany.Conditions;
using loaforcsSoundAPI.LethalCompany.Reporting;
using loaforcsSoundAPI.Reporting;
using loaforcsSoundAPI.Core.Util.Extensions;
using loaforcsSoundAPI.LethalCompany.Conditions.Dungeon;

namespace loaforcsSoundAPI.LethalCompany.Patches;

[HarmonyPatch(typeof(RoundManager))]
static class RoundManagerPatch {
	[HarmonyPatch(nameof(RoundManager.GenerateNewFloor)), HarmonyPostfix, HarmonyWrapSafe]
	static void Reporting() {
		if(SoundReportHandler.CurrentReport == null) return;
        
		string dungeonName = RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow.name;
		string moonName = StartOfRound.Instance.currentLevel.name;
		
		LethalCompanySoundReport.foundDungeonTypes.AddUnique(dungeonName);
		LethalCompanySoundReport.foundMoonNames.AddUnique(moonName);
	}

	[HarmonyPatch(nameof(RoundManager.Awake)), HarmonyPostfix, HarmonyWrapSafe]
	static void ListenForPowerChanges() {
		RoundManager.Instance.onPowerSwitch.AddListener(power => {
			DungeonPowerStateCondition.CurrentPowerState = power;
		});
	}
}