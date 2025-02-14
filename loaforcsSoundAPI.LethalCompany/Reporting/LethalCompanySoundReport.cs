using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using LethalLevelLoader;
using loaforcsSoundAPI.LethalCompany.Conditions;
using loaforcsSoundAPI.LethalCompany.Conditions.Player;
using loaforcsSoundAPI.LethalCompany.Conditions.Ship;
using loaforcsSoundAPI.Reporting;

namespace loaforcsSoundAPI.LethalCompany.Reporting;

static class LethalCompanySoundReport {
	internal static readonly List<string> foundDungeonTypes = [];
	internal static readonly List<string> foundMoonNames = [];
	internal static readonly List<ReverbPreset> foundReverbPresets = [];
	internal static readonly List<FootstepSurface> foundFootstepSurfaces = [];
    
	internal static void Init() {
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

			if (loaforcsSoundAPILethalCompany.CheckSoftDep(LethalLevelLoader.Plugin.ModGUID)) {
				WriteLLLDataToReport(stream);
			}
		});
	}
	
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	static void WriteLLLDataToReport(StreamWriter stream) {
		List<string> tags = [];
		// i kinda really don't like this
		loaforcsSoundAPILethalCompany.Logger.LogDebug("writing lll data!!");
		List<ExtendedMod> mods = PatchedContent.ExtendedMods.ToList();
		mods.Add(PatchedContent.VanillaMod);
		foreach (ExtendedMod mod in mods) {
			foreach (ExtendedContent content in mod.ExtendedContents) {
				tags.AddRange(
					content.ContentTags.Select(
						it => it.contentTagName
					).Where(
						it => !tags.Contains(it)
					)
				);
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