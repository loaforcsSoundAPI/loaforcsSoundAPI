using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using loaforcsSoundAPI.LethalCompany.Conditions;
using loaforcsSoundAPI.LethalCompany.Conditions.OtherMods.LethalLevelLoader;
using loaforcsSoundAPI.LethalCompany.Conditions.Player;
using loaforcsSoundAPI.LethalCompany.Conditions.Ship;
using loaforcsSoundAPI.LethalCompany.Reporting;
using loaforcsSoundAPI.Reporting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace loaforcsSoundAPI.LethalCompany;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SoundAPI.PLUGIN_GUID)]

// Integrations
[BepInDependency(LethalLevelLoader.Plugin.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
public class loaforcsSoundAPILethalCompany : BaseUnityPlugin {
	internal new static ManualLogSource Logger { get; private set; }
	
	private void Awake() {
		//SoundAPI.RegisterNetworkAdapter(new NGONetworkAdapter());
		Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
		Config.SaveOnConfigSet = false;
		SoundAPI.RegisterAll(Assembly.GetExecutingAssembly());

		if (CheckSoftDep(LethalLevelLoader.Plugin.ModGUID)) {
			RegisterLLLConditions();
		}

		if (SoundReportHandler.CurrentReport != null) {
			LethalCompanySoundReport.Init();
		}
		
		// todo
		// SoundFixesConfig.Bind(Config);
		
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
		Config.Save();
		Logger.LogInfo("Done.");
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	static void RegisterLLLConditions() {
		Logger.LogInfo("LethalLevelLoader found, registering conditions on SoundAPI side.");
		SoundAPI.RegisterCondition("LethalLevelLoader:dungeon:has_tag", () => new LLLTagCondition<ExtendedDungeonFlow>(() => {
			if (!RoundManager.Instance) return null;
			if (!RoundManager.Instance.dungeonGenerator) return null;
			if (!PatchedContent.TryGetExtendedContent(
				    RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow, 
				    out ExtendedDungeonFlow lllDungeon)
			   ) return null;
			return lllDungeon;
		}));
		SoundAPI.RegisterCondition("LethalLevelLoader:moon:has_tag", () => new LLLTagCondition<ExtendedLevel>(() => {
			if (!StartOfRound.Instance) return null;
			if (!PatchedContent.TryGetExtendedContent(
					StartOfRound.Instance.currentLevel, 
					out ExtendedLevel lllMoon)
			   ) return null;
			return lllMoon;
		}));
	}
	
	internal static bool CheckSoftDep(string guid) {
		return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(guid);
	}
}