using System.Threading.Tasks;
using loaforcsSoundAPI.Core;
using loaforcsSoundAPI.SoundPacks.AudioClipLoading;

namespace loaforcsSoundAPI.SoundPacks.Data;

public class ReplacementsRegistry(SoundPack pack, string relativePath) : Registry<SoundReplacementCollection>(pack, relativePath) {
	protected override void HotLoadAdd(SoundReplacementCollection item) {
		AsyncAudioClipLoader clipLoader = new AsyncAudioClipLoader();
		foreach(SoundReplacementGroup group in item.Replacements) {
			group.QueueSounds(clipLoader);
		}

		Task.Run(async () => {
			Debuggers.HotReload?.Log("start async clip loader!");
			await clipLoader.LoadAllAsync();
			Debuggers.HotReload?.Log("done! registering new replacements");

			foreach(SoundReplacementGroup group in item.Replacements) {
				SoundPackDataHandler.AddReplacement(group);
			}
		});
	}

	protected override void HotLoadRemove(SoundReplacementCollection item) {
		Debuggers.HotReload?.Log("removing old replacements");

		foreach(SoundReplacementGroup group in item.Replacements) {
			SoundPackDataHandler.RemoveReplacement(group);
			group.DestroySounds();
		}
	}
}