# loaforcsSoundAPI
A portable sound-replacement mod, built to give sound pack creators more control and a more maintainable code base.

## For Sound-Pack creators
A [simple tutorial](https://github.com/loaforcsSoundAPI/loaforcsSoundAPI/wiki/Making-a-simple-Sound%E2%80%90Pack) to make a sound pack is available in the github wiki.
[Examples](https://github.com/loaforcsSoundAPI/loaforcsSoundAPI.Examples) are available as well.

- Custom File Structure Hierarchy
- Mapping multiple sounds to the same sounds
- Sound-packs can create config options just like regular mods
- Swap out audio clips live based on what is happening in game.

---

## For developers
SoundAPI will eventually contain multiple utilities to help with loading custom sounds. All common use methods are under the `SoundAPI` class.

### Custom Conditions
Here is an example condition.
```cs
[SoundAPICondition("LethalCompany:apparatus_state")] // attribute for SoundAPI.RegisterAll function
public class ApparatusStateCondition : Condition {
    public enum StateType {
    	PULLED,
    	PLUGGED_IN
    }
    internal static bool CurrentApparatusPulled = false;

    public StateType? Value { get; internal set; }

    public override bool Evaluate(IContext context) {
    	StateType state = CurrentApparatusPulled ? StateType.PULLED : StateType.PLUGGED_IN;
		
    	return state == (Value ?? StateType.PULLED);
    }
}

// To register all in your assembly (this will respect other soft-dependencies in your mod)
SoundAPI.RegisterAll(Assembly.GetExecutingAssembly());

// Or manually (if you need more complex logic)
SoundAPI.RegisterCondition("LethalLevelLoader:dungeon:has_tag", () => new LLLTagCondition<ExtendedDungeonFlow>(() => {
    if (!RoundManager.Instance) return null;
    if (!RoundManager.Instance.dungeonGenerator) return null;
    if (!PatchedContent.TryGetExtendedContent(
        RoundManager.Instance.dungeonGenerator.Generator.DungeonFlow, 
        out ExtendedDungeonFlow lllDungeon)
    ) return null;
    return lllDungeon;
}));
```
Sound-pack creators will use
```json
{
    "condition": {
        "type": "LethalCompany:apparatus_state"
    }
}
```
Because `Value` is nullable, the sound pack creator does not need to supply a value, and it will default to true. 
Otherwise, they can manually set it to `PLUGGED_IN` to check if the apparatus is plugged in.
It's recommended to use an enum even if there are currently only two options for forward compatibility.

### Mappings
Mappings are for a mod developer to write a shortcut to one or more sound matches.
It's recommended sound pack creators use mappings where possible for forward compatibility (as the mod dev can update the mapping file with a mod update, without the sound pack needing to update.)

To use mappings you need to include a `sound_pack.json` in your mod zip (this is so SoundAPI loads it automatically, an option to load them via the `SoundAPI` class may come later). 
You do not need to include a `replacers` or `sounds` folder.
A `soundapi_mappings.json` file then goes in the same folder.

Example `soundapi_mappings.json` file:
```json
{
    "FacilityMeltdown:music": [
        "MeltdownHandler:MeltdownMusic:meltdownMusic"
    ],
    "FacilityMeltdown:warning_voice": [
        "MeltdownHandler:WarningVoice:warning1",
        "MeltdownHandler:WarningVoice:warning2",
        "MeltdownHandler:WarningVoice:warning3",
        "MeltdownHandler:WarningVoice:warning4"
    ]
}
```

**NOTE: The `soundapi_mappings.json` may change file name later, but compatibility will be retained.**

Sound-pack creators will then use your mappings in their replacers. Mappings in matches are prepended with a `#`.
```json
{
    "replacements": [
        {
            "matches": "#FacilityMeltdown:music",
            "sounds": [
                {
                    "sound": "meltdown.ogg"
                }
            ]
        }
    ]
}

```

### Naming Convention
It's recommended to follow a naming convention for your conditions and mappings. 
Don't have 1 'part' or more than 3. Each part should be seperated with a colon.
- `NAME OF YOUR MOD:NAME` or
- `NAME OF YOUR MOD:SUB CATEGORY:NAME`