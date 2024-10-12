using System.IO;
using BepInEx;

namespace loaforcsSoundAPI.Core.Util;

static class LogFormats {
	internal static string FormatFilePath(string path) {
		return $"plugins{Path.DirectorySeparatorChar}{Path.Combine(Path.GetRelativePath(Paths.PluginPath, path))}";
	}
}