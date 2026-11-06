# 3D Zombie Runner - Military Base Edition

Unity 3D endless runner / zombie survival. Written for **Unity 2022.3 LTS** (built-in render pipeline, legacy Input Manager).

## Open and play
1. Extract the ZIP.
2. Unity Hub -> Add -> select the `ZombieRunner_MilitaryBase` folder -> open with any 2022.3 LTS editor.
3. First open: wait for compilation. An editor script creates `Assets/Scenes/MainMenu.unity` and `Gameplay.unity`, adds them to Build Settings and opens the menu (also available via menu **Zombie Runner > Setup Project**).
4. Press **Play**. Click START GAME.
5. Optional: **Zombie Runner > Build Windows EXE** (needs the Windows Build Support module).

If the console shows compile errors, send me the first error and I will fix it (see "Testing status").

## Controls
A/Left, D/Right: lanes - W/Up: jump - S/Down: slide (fast-drop in air) - Space or Left Mouse: shoot (hold) - Esc: pause - R: restart after game over.

## Design notes
- All models, particles, UI and audio are **generated procedurally in code** (Assets/Scripts/Gen, AudioManager); there are no external or licensed assets. The scenes only contain a bootstrap object that builds the game at runtime.
- Zones (40 m modular segments, pooled): Base Entrance, Supply Depot, Barracks, Runway/Vehicle Zone.
- Zombies: Normal, Fast, Armored, Horde patterns. Obstacles: blocks, jump, overhead (slide), hazards (mine, explosive barrel - shootable, fire, electric).
- Fairness: every spawn pattern leaves a safe lane or is a full-width wall that is jumpable/slidable.
- Power-ups: Health Kit, Ammo, Coin Magnet, Shield, Speed Boost, Double Coins. All values live on `GameConfig` (Inspector, play mode) / defaults in `GameConfig.cs`.
- Save: best score, total coins, volumes, quality, difficulty via PlayerPrefs.
