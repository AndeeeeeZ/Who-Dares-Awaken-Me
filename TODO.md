# Graveyard game TODO

The number of waves comes from the `RoundController` waves array. Tombstones are lives: the player loses when all eight are broken at once and wins after clearing the final configured wave.

**Tags:** `[Code]` can be done in C# scripts. `[Unity]` requires scene, prefab, UI, or other asset setup in the Unity Editor. Agent instructions keep those Unity files read-only unless the user authorizes an exception. `[Test]` is a Play Mode or build check.

## 1. Finish tombstone repair and round pacing

- [ ] **[Unity]** Increase the tombstone prefab's `fixPerInteraction` from its current `2` durability per click so repairs are meaningful during preparation.
- [ ] **[Unity]** Set the desired preparation durations and starting tombstone durability in the Inspector. The current scene uses 15 seconds before the first wave, 10 seconds between waves, and seven tombstones starting broken at 1 durability for the survivor.
- [ ] **[Test]** Confirm a zero-durability tombstone can be repaired while another survives, and that all eight at zero immediately show defeat and stop spawning and player interactions.
- [ ] **[Test]** Run a short waves array and confirm the final configured wave ends in victory. Check that enemy deaths, impacts, and destruction cannot leave a wave stuck.

## 2. Make threats and defenses matter

- [ ] **[Code]** Make board and chain defenses distinct: boards are fast to place but weaker; chains take longer but last longer.
- [ ] **[Unity]** Add and wire the second lockable door. The scene currently has one lockable `Door` and one board-repair `Wall`; decorative door meshes are not interactive.
- [ ] **[Unity]** Make each `Door` object's interaction collider a trigger so it can detect enemy impacts without blocking the doorway after the chain breaks. Keep the closed-door visual's blocking collider.
- [ ] **[Test]** Check that both enemy types prefer secured doors and placed boards, damage them on impact, retarget after a defense breaks, and fall back to standing tombstones. Verify chain and board replacement, and validate the leg enemy impact effect and damage in Play Mode.

## 3. Give the player useful information

- [ ] **[Code]** Feed a minimap with player, enemy, tombstone, and door positions and states.
- [ ] **[Unity]** Build a readable minimap in the Canvas: player direction, eight tombstones, two doors, and active enemy markers. Color tombstones by health and pulse one under attack.
- [ ] **[Code]** Show the current wave number and total configured waves in the HUD. Preparation time, remaining enemies, and standing tombstone count already have display scripts.
- [ ] **[Unity]** Add and assign a wave-number TextMeshPro label in the Canvas.
- [ ] **[Unity]** Add directional audio or visual warnings for attacks so the player does not have to watch only the minimap.

## 4. Polish and validate

- [ ] **[Code]** Enable and disable the `CursorHider` input action correctly so Escape can release the cursor.
- [ ] **[Test]** Play every configured wave from the title screen. Check prep timing, repair pacing, both doors, minimap alerts, all-tombstones-broken defeat, and final-wave victory.
- [ ] **[Test]** Check the Unity Console for errors, then build and play on the intended target platform (including browser if the itch.io version is the target).
- [ ] **[Test]** Tune enemy budgets, spawn spacing, damage, repair speed, and preparation length based on full-run playtests.
