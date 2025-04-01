# Boulders Begone

**A BepInEx mod for Aska**  
Removes all those pointless decorative boulders that get in the way of building. Because you're a Viking, not a landscaper.

## DISCLAIMER

This mod is in no way officially endorsed by the developers. You are downloading and using this mod with the understanding that you are modifying the game outside of what the devs intended, and that this may result in unexpected behavrior, potentially even game breaking bugs.

## Features

- Automatically deletes decorative boulders across the entire explored map upon loading into the world
- Additionally delete them on command by pressing F6
- Lightweight and performance-friendly

## Compatibility

To remain compatible:
- Avoid modifying the same GameObjects (e.g. "rock_shoreline_medium" and "VFX_VFXVegetation_Rock4_38+0")
- Avoid repurposing the F6 key without coordination
- Avoid mass-deleting scene objects without filtering types
- Any large mods that affect scene loading should try to preserve late-stage hooks (e.g., camera movement or HUD loading) for the automatic trigger.

## Timer-Based Trigger

Because ASKA loads a full world and character model in the main menu, I could not find a clean way to detect when the player is actually in world. Thus, the "automatic" activation is a simple timer: after you click load game, it waits 10 seconds, then triggers the mod. This works out to be about one second after I load into the world in my game. On slower computers, the load screen *should* halt the process from finishing until the world loads, even if the timer runs out mid-load. If you have any problems, simply press F6 to trigger the mod manually.

## Planned Features

- Customize timer activation delay and activation key (easy)
- Automatically detect length of time from load game to first F6 press and save that as the timer for the future (medium)
- Possibly find a better way to detect loading into the world (hard)

## Requirements

- BepInEx 6 (BepInExPack for IL2CPP)
- Aska (latest Early Access version)

## Installation

1. Go to https://builds.bepinex.dev/projects/bepinex_be
2. Download the **BepInEx Unity (IL2CPP)** file that matches your OS. **It MUST be the IL2CPP version, NOT Mono.**
3. Extract the BepInEx .zip into your main ASKA directory (where ASKA.exe is)
4. Run the game once to generate BepInEx files
5. Extract maoBouldersBegone.zip into the BepInEx\plugins folder
6. Launch the game
7. ??????
8. Profit!

## License

Licensed under [Creative Commons Attribution 4.0 International](https://creativecommons.org/licenses/by/4.0/).  
**TL;DR:** *Do whatever you want, just give me full credit.*

---

Have fun!
— Maoman
