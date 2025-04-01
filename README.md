# Boulders Begone

**A BepInEx mod for Aska**  
Removes all those pointless decorative boulders that get in the way of building. Because you're a Viking, not a landscaper.

## Features

- Automatically deletes decorative boulders across the entire explored map upon loading into the world
- Additionally delete them on command by pressing F6
- Lightweight and performance-friendly

## Compatibility

To remain compatible:
- Avoid modifying the same GameObjects (I used `GameObject.FindObjectsOfType<>()` then filtered by `rock` and `boulder` and that happened to work.)
- Avoid repurposing the F6 key without coordination
- Avoid mass-deleting scene objects without filtering types
- Any large mods that affect scene loading should try to preserve late-stage hooks (e.g., camera movement or HUD loading) for the automatic trigger.

## Timer-Based Trigger

Because ASKA loads a full world and character model in the main menu, I could not find a clean way to detect when the player is actually in world. Thus, the "automatic" activation is a simple timer: after you click load game, it waits 10 seconds, then triggers the mod. This works out to be about one second after I load into the world in my game. If that's too fast for your save file and computer I apologize, but for now just press F6.

## Planned Features

- Customize timer activation delay and activation key (easy)
- Automatically detect length of time from load game to first F6 press and save that as the timer for the future (medium)
- Possibly find a better way to detect loading into the world (hard)

## Requirements

- BepInEx 6 (BepInExPack for IL2CPP)
- Aska (latest Early Access version)

## Installation

1. Install BepInEx 6 into your Aska directory  
2. Drop the compiled `MaoBouldersBegone.dll` into `BepInEx/plugins`  
3. Launch the game and enjoy a clutter-free map!

## License

Licensed under [Creative Commons Attribution 4.0 International](https://creativecommons.org/licenses/by/4.0/).  
**TL;DR:** *Do whatever you want, just give me full credit.*

---

Have fun!
— Maoman
