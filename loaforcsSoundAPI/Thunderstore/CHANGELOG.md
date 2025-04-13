## 2.0.1
- Replacements and sounds with a condition that are labelled as `constant` are now skipped when loading, as opposed to just entire replacer files
- Conditional objects marked with `constant` also now skip validation and mapping (meaning custom conditions/mappings from mods can be properly used like a soft-dependency)
- Fixed an issue where it was impossible for some sounds to be randomly chosen
- Added validation error for missing mapping
- When logging validation errors, the log source now has `soundpack.` infront of the sound pack name
- Changed `resetsat` to `resets_at` for the `counter` condition
- Added logs for when SoundAPI handles DebugLogSources
- Added smarter internal logic to handle SoundAPI updating some AudioSources. Should maybe be more performant
- Fixed an issue where the `MatchStrings` DebugLogSource did not log anything unless `GenerateReports` was also enabled.

## 2.0.2
- woops

## 2.0.3
- fixed an issue where if SoundAPI had nothing to load, it would spin infinitely.

## 2.0.4
- bleh 

## 2.0.5
- upload to R.E.P.O.

## 2.0.6
- fixed an issue where SoundAPI would cause many issues if a loaded sound-pack tried to reference files that didn't exist.
- fixed an issue where SoundAPI would stop audio sources from playing if they were marked as `playOnAwake` and loaded with a scene.
    - note: this has a temporary config to disable it, as i'm not entirely sure if this will fully work
- added an extra check to prevent duplicate sound-packs from loading.
- added a warning log if no sound-packs loaded.