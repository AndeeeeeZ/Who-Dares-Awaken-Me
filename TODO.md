# Graveyard game TODO

This checklist turns the plan in `agent.md` into implementation work. The game has five waves; each wave starts after a 30-second preparation phase. All eight tombstones are lives: the player loses when all eight are broken at once and wins after clearing wave five.

**Tags:** `[Code]` can be done in C# scripts. `[Unity]` requires you to set up or save scenes, prefabs, UI, or other assets in the Unity Editor. Agent instructions currently keep those Unity files read-only. `[Test]` is a Play Mode or build check.

## 1. Make the tombstone rules work with preparation

- [x] **[Code]** Stop passive tombstone durability loss. `Tombstone.Start` now disables the serialized decay setting without changing the prefab.
- [x] **[Code]** Make the tombstone progress ring change from red at zero health to green at full health and face the main camera.
- [x] **[Code]** Track the number of tombstones above zero durability and expose a change event and `tombstones: current/total` display script.
- [x] **[Code]** Randomize starting tombstone durability from GameController: a configurable number start at zero and the rest use an Inspector-configurable range.
- [ ] **[Unity]** Tune GameController's Starting Broken Tombstones, Minimum Starting Durability, and Maximum Starting Durability fields in the Inspector; confirm all eight Tombstone references remain assigned.
- [ ] **[Code]** Make repair meaningful within 30 seconds. Current repair is 2 health per click; choose a faster amount or a short hold-to-repair action.
- [ ] **[Code]** Give each tombstone a clear broken state at zero health. Allow repair from zero while at least one tombstone remains unbroken.
- [ ] **[Code]** Trigger defeat immediately when all eight tombstones are at zero; stop spawning and player interactions after the result.
- [ ] **[Unity]** Configure the tombstone visuals, prompts, and health display for healthy, damaged, broken, and under-attack states.

## 2. Build the five-wave loop

- [ ] **[Code]** Add game states for preparation, active wave, victory, and defeat. Start each of the five waves after a 30-second preparation countdown; spawn nothing during preparation.
- [ ] **[Code]** Give each wave a fixed **total** small/leg pumpkin budget, a spawn interval, and a separate cap on enemies alive simultaneously. The current `EnemySpawner.maxEnemyCount` only limits simultaneous enemies and will otherwise spawn forever.
- [ ] **[Code]** Track remaining-to-spawn and alive enemies separately. Count an enemy as resolved exactly once whether killed by the player or exploded against a target.
- [ ] **[Code]** End a wave only when both counts reach zero. Start the next preparation phase, or show victory after wave five.
- [ ] **[Code]** Add restart and return-to-title actions to the victory/defeat flow.
- [ ] **[Unity]** Wire the game controller, two spawners, eight tombstones, and result UI in `GameScene`.
- [ ] **[Test]** Confirm wave transitions cannot hang if an enemy dies, impacts a target, or is otherwise destroyed.

### Initial wave budgets for playtesting

| Wave | Small pumpkins | Leg pumpkins |
| --- | ---: | ---: |
| 1 | 4 | 0 |
| 2 | 5 | 1 |
| 3 | 7 | 1 |
| 4 | 8 | 2 |
| 5 | 10 | 3 |

These are starting values to tune after playing a full run.

## 3. Make threats and defenses matter

- [ ] **[Code]** Replace the spawners' fixed tombstone target with valid target selection across live tombstones and appropriate defenses. Retarget when a target breaks or becomes unavailable.
- [ ] **[Code]** Define what happens when a locked door or boarded defense reaches zero durability. Support repair or replacement and make enemy behavior follow the actual state.
- [ ] **[Code]** Make board and chain defenses distinct: boards are fast to place but weaker; chains take longer but last longer.
- [ ] **[Unity]** Add and wire the second lockable door. The scene currently has one lockable `Door` and one board-repair `Wall`; decorative door meshes are not interactive.
- [ ] **[Test]** Check both enemy types against tombstones and each defense. The current uncommitted leg enemy prefab change assigns an impact effect and sets damage to `-50`; confirm it works in Play Mode.

## 4. Give the player useful information

- [ ] **[Code]** Feed a minimap with player, enemy, tombstone, and door positions and states.
- [ ] **[Unity]** Build a readable minimap in the Canvas: player direction, eight tombstones, two doors, and active enemy markers. Color tombstones by health and pulse one under attack.
- [ ] **[Code]** Expose wave number (1–5), preparation time, and enemy count to the HUD. Surviving tombstone count is now exposed in code.
- [ ] **[Unity]** Assign all eight Tombstone components on `GameController`. Add a TextMeshPro UI label under `GameScene`'s `UI Canvas`, attach `TombstoneCountDisplay`, and assign its GameController and Count Text fields.
- [ ] **[Unity]** Add HUD text/icons and victory/defeat panels. Add directional audio or visual warnings for attacks so the player does not have to watch only the minimap.

## 5. Polish and validate

- [ ] **[Code]** Remove per-frame and per-collision debug logging from enemy spawning/combat.
- [ ] **[Code]** Enable and disable the `CursorHider` input action correctly so Escape can release the cursor.
- [ ] **[Test]** Play all five waves from the title screen. Check prep timing, repair pacing, both doors, minimap alerts, all-tombstones-broken defeat, and wave-five victory.
- [ ] **[Test]** Check the Unity Console for errors, then build and play on the intended target platform (including browser if the itch.io version is the target).
- [ ] **[Test]** Tune enemy budgets, spawn spacing, damage, repair speed, and preparation length based on full-run playtests.
