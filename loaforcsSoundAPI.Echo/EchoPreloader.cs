using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using loaforcsSoundAPI.Echo.Patches;
using loaforcsSoundAPI.Reporting;
using loaforcsSoundAPI.Reporting.Data;
using Mono.Cecil;

namespace loaforcsSoundAPI.Echo;

public static class EchoPreloader {
	public static IEnumerable<string> TargetDLLs { get; } = [];
	
	internal static ManualLogSource Logger { get; } = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);

	public static void Patch(AssemblyDefinition assembly) { }

	internal static bool CustomSoundsPatched = false;

	static Harmony _harmony = null!;
	
	public static void Finish() {
		Logger.LogInfo($"Loading Echo v{MyPluginInfo.PLUGIN_VERSION}");

		Logger.LogInfo("Running patches.");
		_harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);

		// this is horrible but i really don't care actually
		AppDomain.CurrentDomain.AssemblyLoad += (sender, e) => {
			if(e.LoadedAssembly.GetName().Name == "CustomSounds" && !CustomSoundsPatched)
				TryPatchCustomSounds();
		};
        
		//TryPatchCustomSounds();
        
		Logger.LogInfo("Finished loading :3");
	}
	
	internal static void AddReportSection() {
		SoundReportHandler.AddReportSection("Echo Loader", (stream, _) => {
			stream.WriteLine($"Echo version: `{MyPluginInfo.PLUGIN_VERSION}` <br/><br/>");
			
			stream.WriteLine($"Patched CustomSounds: `{CustomSoundsPatched}` <br/>");
			stream.WriteLine($"Load time: `{CustomSoundsLoadPipeline.LoadTime}ms`");
			
			SoundReportHandler.WriteList("CustomSounds packs", stream, CustomSoundsLoadPipeline.CustomSoundsPacks.Select(it => it.Name).ToList());
		});
		
		
	}

	internal static void TryPatchCustomSounds() {
		Logger.LogInfo("Attempting to patch CustomSounds.");
		
		// i don't think there's a cleaner way to handle patching custom sounds like this lol
		try {
			_harmony.PatchAll(typeof(CustomSoundsPatches));
			CustomSoundsPatched = true;
		} catch (TypeLoadException) {
			Logger.LogInfo("Failed this time, but echo will still work.");
		} // custom sounds is not installed
	}
}