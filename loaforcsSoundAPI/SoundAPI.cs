using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using loaforcsSoundAPI.Core.Networking;
using loaforcsSoundAPI.SoundPacks;
using loaforcsSoundAPI.SoundPacks.Data;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using loaforcsSoundAPI.Core.Util.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace loaforcsSoundAPI;

/// <summary>
/// Main SoundAPI functionality
/// </summary>
public static class SoundAPI {
	/// <summary>
	/// BepInEx plugin GUID for dependency. It is fine to reference as a soft-dependency.
	/// </summary>
	public const string PLUGIN_GUID = MyPluginInfo.PLUGIN_GUID;
	
	internal static NetworkAdapter CurrentNetworkAdapter { get; private set; }

	/// <summary>
	/// Loads an audio clip from a given file. 
	/// </summary>
	/// <exception cref="NotImplementedException">Thrown if an unsupported audio file extension is passed.</exception>
	/// <exception cref="FileNotFoundException">File not found</exception>
	/// <remarks></remarks>
	/// <returns>AudioClip</returns>
	public static async Task<AudioClip> LoadAudioFileAsync(string fullPath) {
		// todo: this is effectively duplicated code from the soundpack load pipeline, look at combining the code from there into this.

		if (!File.Exists(fullPath)) throw new FileNotFoundException($"'{fullPath}' not found.");
		if (!SoundPackLoadPipeline.audioExtensions.ContainsKey(Path.GetExtension(fullPath))) throw new NotImplementedException($"Audio file extension: '{Path.GetExtension(fullPath)}' is not implemented.");
		
		UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(fullPath, SoundPackLoadPipeline.audioExtensions[Path.GetExtension(fullPath)]);
		await request.SendWebRequest();
		
		AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
		request.Dispose();
		return clip;
	}
	
	/// <summary>
	/// Finds all types marked with [SoundAPICondition] and registers them with a default factory method.
	/// </summary>
	/// <seealso cref="SoundAPIConditionAttribute"/>
	/// <seealso cref="Condition{ContextType}"/>
	/// <param name="assembly">Assembly to search</param>
	public static void RegisterAll(Assembly assembly) {
		foreach (Type type in assembly.GetLoadableTypes()) {
			if(type.IsNested) continue;

			foreach (SoundAPIConditionAttribute conditionAttribute in type.GetCustomAttributes<SoundAPIConditionAttribute>()) {
				if (!typeof(Condition).IsAssignableFrom(type)) {
					loaforcsSoundAPI.Logger.LogError($"Condition: '{type.FullName}' has been marked with [SoundAPICondition] but does not extend Condition!");
					continue;
				}
			
				ConstructorInfo info = type.GetConstructor([]);
				if (info == null) {
					loaforcsSoundAPI.Logger.LogError(
						$"Condition: '{type.FullName}' has no valid constructor! It must have a constructor with no parameters! " +
						$"If you need extra parameters do not mark it with [SoundAPICondition] and register it manually."
					);
					continue;
				}
				RegisterCondition(conditionAttribute.ID, () => {
					if (conditionAttribute.IsDeprecated) { // todo: change this to be like InvalidCondition so that it can correctly trigger during validation
						if (conditionAttribute.DeprecationReason == null) {
							loaforcsSoundAPI.Logger.LogWarning($"Condition: '{conditionAttribute.ID}' is deprecated and may be removed in future.");
						} else {
							loaforcsSoundAPI.Logger.LogWarning($"Condition: '{conditionAttribute.ID}' is deprecated. {conditionAttribute.DeprecationReason}");
						}
					
					}
                
					return (Condition)info.Invoke([]);
				});
			}
		}
	}
	
	// this seems a bit dumb because it just surfaces an internal method, but i want to keep all public methods in SoundAPI
	/// <summary>
	/// Register a condition
	/// </summary>
	/// <seealso cref="Condition{ContextType}"/>
	/// <param name="id">A unique ID for this condition</param>
	/// <param name="factory">The method to create a condition per instance.</param>
	public static void RegisterCondition(string id, Func<Condition> factory) {
		SoundPackDataHandler.Register(id, factory);
	}

	
	/// <summary>
	/// Registers a network adapter.
	/// </summary>
	/// <param name="adapter"></param>
	public static void RegisterNetworkAdapter(NetworkAdapter adapter) {
		CurrentNetworkAdapter = adapter;
		loaforcsSoundAPI.Logger.LogInfo($"Registered network adapter: '{CurrentNetworkAdapter.Name}'");
		CurrentNetworkAdapter.OnRegister();
	}
	
	/// <summary>
	/// Registers a Sound-pack for use by SoundAPI.
	/// </summary>
	/// <param name="pack">Pack to register</param>
	/// <exception cref="InvalidOperationException">Sound-pack is already registered.</exception>
	public static void RegisterSoundPack(SoundPack pack) {
		if (SoundPackDataHandler.LoadedPacks.Contains(pack)) {
			throw new InvalidOperationException($"Already registered sound-pack: '{pack.Name}'!");
		}
		
		SoundPackDataHandler.AddLoadedPack(pack);
		foreach (SoundReplacementCollection collection in pack.ReplacementCollections) {
			foreach (SoundReplacementGroup group in collection.Replacements) {
				SoundPackDataHandler.AddReplacement(group);
			}
		}
	}
}