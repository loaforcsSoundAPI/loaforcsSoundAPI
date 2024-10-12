using HarmonyLib;
using loaforcsSoundAPI.LethalCompany.Conditions;
using loaforcsSoundAPI.Reporting;

namespace loaforcsSoundAPI.LethalCompany.Patches;

[HarmonyPatch(typeof(RoundManager))]
static class RoundManagerPatch {
	[HarmonyPatch(nameof(RoundManager.GenerateNewFloor)), HarmonyPostfix, HarmonyWrapSafe]
	static void Reporting() {
		if(SoundReportHandler.CurrentReport == null) return;
        
		string dungeonName = RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow.name;
		string moonName = StartOfRound.Instance.currentLevel.name;
		
		
		
		if(!loaforcsSoundAPILethalCompany.foundDungeonTypes.Contains(dungeonName))
			loaforcsSoundAPILethalCompany.foundDungeonTypes.Add(dungeonName);
		
		if(!loaforcsSoundAPILethalCompany.foundMoonNames.Contains(moonName))
			loaforcsSoundAPILethalCompany.foundMoonNames.Add(moonName);
	}

	[HarmonyPatch(nameof(RoundManager.Awake)), HarmonyPostfix, HarmonyWrapSafe]
	static void ListenForPowerChanges() {
		RoundManager.Instance.onPowerSwitch.AddListener(power => {
			FacilityPowerStateCondition.CurrentPowerState = power;
		});
	}
}