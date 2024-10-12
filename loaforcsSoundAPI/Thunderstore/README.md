 This is the v2 version of SoundAPI. The entire mod has been rewritten to be more stable and (hopefully) performant.

NOTE: It is NOT recommened that you depend on this specific package. Once it is stable enough, this package will be deprecated and the main loaforcsSoundAPI package will receive future updates instead.

## Major Fixes
- Multithreading crashes (fr this time, I spent days on just this alone lol)
- Certain sounds would never be picked up (e.g. Meltdown audio after an update)

## Known Issues
- The game sometimes crashes when closing.
- Only `.ogg` is supported currently.
- When using a lot of mods it will sometimes just not load.

## Breaking Changes
- Certain game object names didn't trim out numbers correctly. I don't think this should be an issue but it may come up.
- Conditions specific to Lethal Company have moved into a seperate package to better facilitate SoundAPI's cross-game compatibility

## New Features
- Way better error handling and validation systems.
- Contextual conditions
- Config option to generate sound reports, 
- Custom match string mappings. Will allow mod developers to provide mappings so sound replacement mods will continue to work even if the mod updates how it handles audio.