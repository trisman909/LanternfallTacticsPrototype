# Phase 6K — Combat Fairness, Readability, and AI Polish

Phase 6K is a polish and correctness pass. It adds no enemies, skills, rooms, relics, classes, progression, or encounter content.

## Telegraph-accurate damage

The complete player-damage pipeline now has one guarded entry point. Enemy immediate previews, delayed HP attacks, ember vents, and charged-floor splash tiles are snapshotted when End Turn is pressed. Before HP changes, `CombatTelegraphValidator` verifies that the player's tile was present in that committed set. A mismatch logs `TELEGRAPH_MISMATCH`, increments an internal counter, and suppresses the unannounced damage.

Enemies no longer move and attack from a preview generated after End Turn. They may move into a threatening position, but the regenerated pattern is displayed for the next End Turn before it can resolve. Attack range, line-of-sight, and damage execution therefore use the same `Preview`/`DelayedPreview` sets the player saw.

Charged floor now exposes every adjacent damage tile with the armed-hazard visual state. Ember vents expose their exact tile. Tooltips on affected tiles state that two damage will resolve after End Turn.

## AI reliability

Phase 6J destination, lane, flank, and escape reservations remain intact. Immediate attack positions receive stronger commitment. Non-ranged enemies that make no progress accumulate a stall count; after two idle turns stale movement memory is cleared and closing distance receives extra weight. Explicit roots and valid ranged spacing remain legitimate reasons to hold.

## Targeting and biome feedback

- Hazard icons use a quieter 68% footprint with 16% equal padding and the existing hard tile clip.
- Skill selection continues to separate maximum range, legal gold targets, blocked tiles, and dark out-of-range floor.
- Area skills additionally outline every possible splash tile before casting.
- Standing on a biome hazard adds a strong player-tile frame and compact biome icon.
- Entering a hazard writes its name and exact active rule into the combat message, so water, roots, prism bonuses, vents, and charged-floor behavior do not require counter watching.

## Performance

The validator uses small hash sets already produced by the threat system. All planning and checks operate on the existing 9×11 board. No assets, shaders, particles, lighting, or post-processing were added.
