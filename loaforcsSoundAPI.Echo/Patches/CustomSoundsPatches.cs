using System.Collections.Generic;
using CustomSounds;
using HarmonyLib;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace loaforcsSoundAPI.Echo.Patches;

static class CustomSoundsPatches {
	// i probably don't need to patch all these but better safe than sorry
	[
		HarmonyTranspiler,
		HarmonyPatch(typeof(Plugin), nameof(Plugin.Awake)), 
		HarmonyPatch(typeof(Plugin), nameof(Plugin.Start)),
		HarmonyPatch(typeof(Plugin), nameof(Plugin.OnDestroy)),
		HarmonyPatch(typeof(Plugin), nameof(Plugin.Initialize)),
		//HarmonyPatch(nameof(Plugin.OnApplicationQuit))
	]
	static IEnumerable<CodeInstruction> DisableCustomSounds(IEnumerable<CodeInstruction> instructions) {
		return new CodeMatcher(instructions)
			   .Start()
			   .Insert(new CodeInstruction(OpCodes.Ret))
			   .InstructionEnumeration();
	}
}