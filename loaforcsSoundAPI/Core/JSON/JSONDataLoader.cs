using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.SoundPacks;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace loaforcsSoundAPI.Core.JSON;

/// <summary>
/// Handles SoundAPI's JSON loading operations.
/// </summary>
public static class JSONDataLoader {
	static readonly JsonSerializerSettings _settings = new() {
		ContractResolver = new IncludePrivatePropertiesContractResolver(),
		Converters = [
			new MatchesJSONConverter(),
			new ConditionConverter()
		]
	};

	/// <summary>
	/// Load JSON file as specified type T
	/// </summary>
	/// <param name="path">File path</param>
	/// <typeparam name="T">Type</typeparam>
	/// <returns>An instance of T or null if an error occured while loading</returns>
	public static T LoadFromFile<T>(string path) {
		string input = File.ReadAllText(path);
		try {
			T result = JsonConvert.DeserializeObject<T>(input, _settings);

			if (result is IFilePathAware dataFile) {
				dataFile.FilePath = path;
			}

			if (result is Conditional conditional && conditional.Condition != null) {
				conditional.Condition.Parent = conditional;
				conditional.Condition.OnRegistered();
			}
            
			return result;
		} catch (JsonReaderException exception) {
			loaforcsSoundAPI.Logger.LogError($"Failed to read json file: 'plugins{Path.DirectorySeparatorChar}{Path.GetRelativePath(Paths.PluginPath, path)}'");
			loaforcsSoundAPI.Logger.LogError(exception.Message);

			// handle showing context around the error.
			string[] lines = input.Split("\n");
			
			int minLeadingSpaces = int.MaxValue;

			// Count leading spaces
			for (int i = Mathf.Max(0, exception.LineNumber - 3); i < Mathf.Min(lines.Length, exception.LineNumber + 3); i++) {
				int leadingSpaces = lines[i].TakeWhile(char.IsWhiteSpace).Count();
				minLeadingSpaces = Mathf.Min(minLeadingSpaces, leadingSpaces);
			}
            
			for (int i = Mathf.Max(0, exception.LineNumber - 3); i < Mathf.Min(lines.Length, exception.LineNumber + 3); i++) {
				string lineContent = $"{(i + 1).ToString(),-5}|  " + lines[i][Mathf.Min(lines[i].Length, minLeadingSpaces)..].TrimEnd();
				
				if (i + 1 == exception.LineNumber) {
					lineContent += " // <- HERE";
				}
				
				loaforcsSoundAPI.Logger.LogError(lineContent);
			}
		}

		return default;
	}
	
	class MatchesJSONConverter : JsonConverter {
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(List<string>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			JToken token = JToken.Load(reader);
			if (token.Type == JTokenType.Array) {
				return token.ToObject<List<string>>();
			}

			return new List<string> { token.ToString() };
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			serializer.Serialize(writer, value);
		}
	}
	
	class IncludePrivatePropertiesContractResolver : DefaultContractResolver {
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
			JsonProperty property = base.CreateProperty(member, memberSerialization);
 
			if (!property.Writable && member is PropertyInfo propInfo) {
				property.Writable = propInfo.GetSetMethod(true) != null;
			}

			return property;
		}
	}
	
	class ConditionConverter : JsonConverter<Condition> {
		public override Condition ReadJson(JsonReader reader, Type objectType, Condition existingValue, bool hasExistingValue, JsonSerializer serializer) {
			// load the json object
			JObject jsonObject = JObject.Load(reader);

			// get the "type" field to determine which condition class to use
			string conditionType = jsonObject["type"]?.ToString();

			if (string.IsNullOrEmpty(conditionType)) return new InvalidCondition(null);
			
			Condition condition = SoundPackDataHandler.CreateCondition(conditionType);
			if (condition == null) return null;
			
			serializer.Populate(jsonObject.CreateReader(), condition);
			
			return condition;
		}

		public override void WriteJson(JsonWriter writer, Condition value, JsonSerializer serializer) {
			throw new NotImplementedException("no.");
		}
	}
}