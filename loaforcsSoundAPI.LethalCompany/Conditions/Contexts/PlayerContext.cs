using GameNetcodeStuff;
using loaforcsSoundAPI.SoundPacks.Data.Conditions;

namespace loaforcsSoundAPI.LethalCompany.Conditions.Contexts;

public class PlayerContext(PlayerControllerB player) : IContext {
	public PlayerControllerB Player => player;
}