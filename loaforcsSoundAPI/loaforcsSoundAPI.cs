using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.Core.Patches;
using loaforcsSoundAPI.Reporting;
using loaforcsSoundAPI.SoundPacks;

namespace loaforcsSoundAPI;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
class loaforcsSoundAPI : BaseUnityPlugin {
	internal new static ManualLogSource Logger { get; private set; }

	static loaforcsSoundAPI _instance;

	void Awake() {
		_instance = this;
		Logger = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_GUID);
		Config.SaveOnConfigSet = false;

		Logger.LogInfo("Setting up config");
		Debuggers.Bind(Config);
		SoundReportHandler.Bind(Config);
		SoundReplacementHandler.Bind(Config);

		Logger.LogInfo("Running patches");
		Harmony harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID);
		UnityObjectPatch.Init(harmony);

		Logger.LogInfo("Registering data");
		SoundAPI.RegisterAll(Assembly.GetExecutingAssembly());

		SoundAPIAudioManager.SpawnManager();

		SoundReplacementHandler.Register();
		Config.Save();

		Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} by loaforc has loaded :3");
	}

	internal static ConfigFile GenerateConfigFile(string name) {
		return new ConfigFile(Utility.CombinePaths(Paths.ConfigPath, name + ".cfg"), false, MetadataHelper.GetMetadata(_instance));
	}
}