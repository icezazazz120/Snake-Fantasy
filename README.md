Snake-Fantasy

This game is a Snake-like game but has a Fantasy RPG mixed. The player have to control a head to collect a line of heroes and fight a monsters on a map.

---

## Technical Decision Document (TDD)

---

### Problem Statement

- Make an arena that the player can move.
- Make a heros and monsters randomly spawn in area.
- Collect player position to make a other heroes have a correct position in line.
- Make a battle and stats system like a turn-base system.
- When hero die make a another hero in line become a head and adjust all heroes position.

---

### Options

1.Player movement and arena
- Use a Tilemap from Unity and make a player move only in vector2D.
- Make a arena form Camera display and make a player move using pushing force.

2.Heros and monsters spawn
- Make both spawn randomly in arena.
- check for both if it have a spawnable position.

3.Player and heroes positioning
- Make a hero in line move toward player head position.
- Use List<Vector3> name previousPositions for position reconding and make a heroes use index.

4.Battle and stats system for turn-base
- Use a same fixed stats prefab for heroes and monsters for a battle.
- Make a adjustable stats separate prefabs for each heroes and monsters for a battle.

5.Positioning after head hero die.
- Remove form list and update position later.
- Make a function name UpdateHeroLineAfterDeath for position update.

---

### Final Decisions

- Use a Tilemap from Unity and make a player move only in vector2D.
- check for both if it have a spawnable position by checking tilemap.
- Use List<Vector3> name previousPositions for position reconding and make a heroes use index.
- Make a adjustable stats separate prefabs for each heroes and monsters for a battle.
- Make a function name UpdateHeroLineAfterDeath for position update.

---

### Consequences

- Easy to use a vector but can only use it for a positioning
- Need to check every tile and if there are a much larger area it might causing performance problem.
- Hero positioning needs to be managed every time a player moves or the first hero dies.
- Stats like Health/Attack/Defense need to be synced every time the first hero die.
- If there are too many heroes, May need to consider make it into a separate system like HeroManager.

---

## Developer
- Nuttakit Sanbor
- 22/06/2025

