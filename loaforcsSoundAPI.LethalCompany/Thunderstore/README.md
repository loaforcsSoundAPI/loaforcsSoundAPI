## loaforcsSoundAPI LethalCompany
Lethal Company bindings for SoundAPI.

## Conditions
- `LethalCompany:apparatus_state`
- `LethalCompany:dungeon_name`
- `LethalCompany:facility_power_state`
- `LethalCompany:moon_name`
- `LethalCompany:player:standing_on`
- `LethalCompany:time_of_day`
- `LethalCompany:player:audio_reverb`
- `LethalCompany:player:is_alone`
- `LethalCompany:player:health` 
- `LethalCompany:player:insanity`
- `LethalCompany:player:location`

### Sound Report Sections
- Found Dungeon Types
- Found Moon Names
- Found Footstep Surfaces
- Player Location Types
- Apparatus State Types
- Time Of Day Types
- Found Reverb Presets

### Updating from SoundAPI v1
- BREAKING: Condition types changed from `LethalCompany:player_health` to `LethalCompany:player:health`
- Make sure to list this package as an explicit dependency.