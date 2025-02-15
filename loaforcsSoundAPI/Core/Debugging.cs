using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx.Configuration;

namespace loaforcsSoundAPI.Core;

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
static class Debuggers {

	internal static DebugLogSource AudioSourceAdditionalData;

	internal static DebugLogSource SoundReplacementLoader;

	internal static DebugLogSource SoundReplacementHandler;

	internal static DebugLogSource MatchStrings;

	internal static DebugLogSource ConditionsInfo;
	
	internal static void Bind(ConfigFile file) {
		foreach(FieldInfo fieldInfo in typeof(Debuggers).GetFields(BindingFlags.Static | BindingFlags.NonPublic)) {
			if (file.Bind("InternalDebugging", fieldInfo.Name, false, "Enable/Disable this DebugLogSource. Should only be true if you know what you are doing or have been asked to.").Value) {
				fieldInfo.SetValue(null, new DebugLogSource(fieldInfo.Name));
			} else {
				fieldInfo.SetValue(null, null);
			}
		}
	}
}

#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

class DebugLogSource(string title) {
	internal void Log(object message) {
		loaforcsSoundAPI.Logger.LogDebug($"[Debug-{title}] {message}");
	}
}