using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.Core.Data;
using loaforcsSoundAPI.Core.JSON;
using loaforcsSoundAPI.Core.Util;
using loaforcsSoundAPI.SoundPacks.AudioClipLoading;

namespace loaforcsSoundAPI.SoundPacks.Data;

public class Registry<T> : IEnumerable<T> where T : class, IFilePathAware {
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

	T TryLoadFile(string filePath) {
		T item = JSONDataLoader.LoadFromFile<T>(filePath);
		if(item == null) return default; // json error

		if(item is IPackData pd) {
			pd.Pack = Pack;
		}

		if(item is IValidatable validatable) {
			if(!IValidatable.LogAndCheckValidationResult(
				   $"loading '{LogFormats.FormatFilePath(filePath)}",
				   validatable.Validate(),
				   Pack.Logger
			   )) {
				return default;
			}
		}

		if(item is IRegistrationCallback callback) {
			callback.OnRegistered();
		}

		_items.Add(item);
		return item;
	}

	internal void Load() {
		if(!Directory.Exists(AbsolutePath)) return; // nothing to load!

		foreach(string file in Directory.GetFiles(AbsolutePath, "*.json", SearchOption.AllDirectories)) {
			TryLoadFile(file);
		}
	}

	protected virtual void HotLoadAdd(T item) { }
	protected virtual void HotLoadRemove(T item) { }

	internal void EnableHotReload() {
		FileSystemWatcher watcher = new FileSystemWatcher(AbsolutePath) {
			NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite,
			Filter = "*.json",
			IncludeSubdirectories = true
		};

		void UnloadOldFile(string fullPath) {
			T existing = _items.FirstOrDefault(it => it.FilePath == fullPath);
			if(existing != null) {
				Debuggers.HotReload?.Log("\twas in registry, removing!");
				_items.Remove(existing);
				HotLoadRemove(existing);
			}
		}

		void LoadNewFile(string fullPath) {
			T item = TryLoadFile(fullPath);
			if(item != null) {
				HotLoadAdd(item);
			}
		}

		watcher.Created += (s, e) => {
			Debuggers.HotReload?.Log($"Created: {LogFormats.FormatFilePath(e.FullPath)}");
			LoadNewFile(e.FullPath);
		};
		watcher.Deleted += (s, e) => {
			Debuggers.HotReload?.Log($"Deleted: {LogFormats.FormatFilePath(e.FullPath)}");
			UnloadOldFile(e.FullPath);
		};
		watcher.Changed += (s, e) => {
			Debuggers.HotReload?.Log($"Changed: {LogFormats.FormatFilePath(e.FullPath)}");
			UnloadOldFile(e.FullPath);
			LoadNewFile(e.FullPath);
		};
		watcher.Renamed += (s, e) => {
			Debuggers.HotReload?.Log($"Renamed: {LogFormats.FormatFilePath(e.OldName)} -> {LogFormats.FormatFilePath(e.Name)}");
			UnloadOldFile(e.OldFullPath);
			LoadNewFile(e.FullPath);
		};

		watcher.EnableRaisingEvents = true;
	}
}