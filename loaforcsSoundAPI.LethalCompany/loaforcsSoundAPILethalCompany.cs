using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using loaforcsSoundAPI.LethalCompany.Conditions;
using loaforcsSoundAPI.LethalCompany.Conditions.OtherMods.LethalLevelLoader;
using loaforcsSoundAPI.LethalCompany.Conditions.Player;
using loaforcsSoundAPI.LethalCompany.Conditions.Ship;
using loaforcsSoundAPI.Reporting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace loaforcsSoundAPI.LethalCompany;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SoundAPI.PLUGIN_GUID)]

// Integrations
[BepInDependency(LethalLevelLoader.Plugin.ModGUID, BepInDependency.DependencyFlags.SoftDependency)]
public class loaforcsSoundAPILethalCompany : BaseUnityPlugin {
	internal static readonly List<string> foundDungeonTypes = [];
	internal static readonly List<string> foundMoonNames = [];
	internal static readonly List<ReverbPreset> foundReverbPresets = [];
	internal static readonly List<FootstepSurface> foundFootstepSurfaces = [];
	
	internal new static ManualLogSource Logger { get; private set; }
	
	private void Awake() {
		//SoundAPI.RegisterNetworkAdapter(new NGONetworkAdapter());
		Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
		SoundAPI.RegisterAll(Assembly.GetExecutingAssembly());

		if (CheckSoftDep(LethalLevelLoader.Plugin.ModGUID)) {
			Logger.LogInfo("LethalLevelLoader found, registering conditions on SoundAPI side.");
			SoundAPI.RegisterCondition("LethalLevelLoader:dungeon:has_tag", () => new LLLDungeonTagCondition());
			SoundAPI.RegisterCondition("LethalLevelLoader:moon:has_tag", () => new LLLMoonTagCondition());
		}
        
		SoundReportHandler.AddReportSection("Lethal Company", (stream, _) => {
			stream.WriteLine($"Version: `{MyPluginInfo.PLUGIN_VERSION}` <br/>");
			
			SoundReportHandler.WriteList("Found Dungeon Types", stream, foundDungeonTypes);
			SoundReportHandler.WriteList("Found Moon Names", stream, foundMoonNames);
			SoundReportHandler.WriteList("Found Reverb Presets", stream, foundReverbPresets.Select(ReverbPresetToHumanReadable).ToList());
			SoundReportHandler.WriteList("Found Footstep Surfaces", stream, foundFootstepSurfaces.Select(it => it.surfaceTag).ToList());
			
			SoundReportHandler.WriteEnum<PlayerLocationCondition.LocationType>("Player Location Types", stream);
			SoundReportHandler.WriteEnum<ApparatusStateCondition.StateType>("Apparatus State Types", stream);
			SoundReportHandler.WriteEnum<DayMode>("Time Of Day Types", stream); // :skull:
			SoundReportHandler.WriteEnum<ShipStateCondition.ShipStateType>("Ship State Types", stream);

			if (CheckSoftDep(LethalLevelLoader.Plugin.ModGUID)) {
				WriteLLLDataToReport(stream);
			}
		});
        
		Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
	}

	bool CheckSoftDep(string guid) {
		return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(guid);
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	void WriteLLLDataToReport(StreamWriter stream) {
		List<string> tags = [];
		// i kinda really don't like this
		foreach (ExtendedMod mod in PatchedContent.ExtendedMods) {
			foreach (ExtendedContent content in mod.ExtendedContents) {
				tags.AddRange(content.ContentTagStrings.Where(tagString => !tag.Contains(tagString)));
			}
		}
		
		SoundReportHandler.WriteList("Found Lethal Level Loader Tags (CASE-SENSITIVE)", stream, tags);
	}
	
	static string ReverbPresetToHumanReadable(ReverbPreset preset) {
		string result = preset.name + "<br/>\n";
		result += $"hasEcho: {preset.hasEcho} <br/>\n";
		result += $"changeRoom: {preset.changeRoom}, room: {preset.room} <br/>\n";
		result += $"changeDecayTime: {preset.changeDecayTime}, decayTime: {preset.decayTime} <br/>\n";
		result += $"changeDryLevel: {preset.changeDryLevel}, dryLevel: {preset.dryLevel} <br/>\n";
		result += $"changeHighFreq: {preset.changeHighFreq}, highFreq: {preset.highFreq} <br/>\n";
		result += $"changeLowFreq: {preset.changeLowFreq}, lowFreq: {preset.lowFreq} <br/>\n";
		
		return result;
	}
}