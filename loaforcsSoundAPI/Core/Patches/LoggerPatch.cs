using HarmonyLib;
using loaforcsSoundAPI.SoundPacks;

namespace loaforcsSoundAPI.Core.Patches;

// i still hate that this stupid dumb ahh patch is still necessary
[HarmonyPatch(typeof(BepInEx.Logging.Logger))]
static class LoggerPatch {
	[HarmonyPrefix, HarmonyPatch("LogMessage")]
	static void ReenableAndSaveConfigs(object data) {
		if (data is not "Chainloader startup complete") return;
		loaforcsSoundAPI.Logger.LogInfo("Starting Sound-pack loading pipeline");
		SoundPackLoadPipeline.StartPipeline();
	}
}