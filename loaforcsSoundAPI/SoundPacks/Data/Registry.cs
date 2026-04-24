using System.Collections;
using System.Collections.Generic;
using System.IO;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.Core.JSON;
using loaforcsSoundAPI.Core.Util;

namespace loaforcsSoundAPI.SoundPacks.Data;

public class Registry<T> : IEnumerable<T> {
	List<T> _items = [ ];

	public Registry(SoundPack pack, string relativePath) {
		Pack = pack;
		RelativePath = relativePath;
		AbsolutePath = Path.Combine(pack.PackFolder, relativePath);
	}

	public SoundPack Pack { get; }
	public string RelativePath { get; }
	public string AbsolutePath { get; }

	public IEnumerator<T> GetEnumerator() {
		return _items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	internal void Load() {
		if(!Directory.Exists(AbsolutePath)) return; // nothing to load!

		foreach(string file in Directory.GetFiles(AbsolutePath, "*.json", SearchOption.AllDirectories)) {
			T item = JSONDataLoader.LoadFromFile<T>(file);
			if(item == null) continue; // json error

			if(item is IPackData pd) {
				pd.Pack = Pack;
			}

			if(item is IValidatable validatable) {
				if(!IValidatable.LogAndCheckValidationResult(
					   $"loading '{LogFormats.FormatFilePath(file)}",
					   validatable.Validate(),
					   Pack.Logger
				   )) {
					continue;
				}
			}

			if(item is IRegistrationCallback callback) {
				callback.OnRegistered();
			}

			_items.Add(item);
		}
	}
}