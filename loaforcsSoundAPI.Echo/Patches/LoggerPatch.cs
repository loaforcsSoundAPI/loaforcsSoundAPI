using HarmonyLib;

namespace loaforcsSoundAPI.Echo.Patches;

[HarmonyPatch(typeof(BepInEx.Logging.Logger))]
static class LoggerPatch {
	[HarmonyPrefix, HarmonyPatch("LogMessage")]
	static void RunPipeline(object data) {
		if (data is not "Chainloader startup complete") return; // this is icky, but patching Chainloader.Start just borks it lmao.
		CustomSoundsLoadPipeline.StartPipeline();
		EchoPreloader.AddReportSection();
	}
}