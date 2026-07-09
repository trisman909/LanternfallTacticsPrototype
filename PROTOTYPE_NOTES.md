# Lanternfall Tactics Prototype Notes

## Phase 5 playable candidate notes

Phase 5 keeps the original prototype content limits: one class, five existing biome identities, three enemy types, one boss, three skills, one reward choice flow, and a five-room run.

### First-time clarity

- Added a start screen before the run begins.
- Added a short in-game "How to Play" panel.
- Clarified the core loop: move or skill, then enemies resolve their previews.
- Added clear reward, boss-room, victory, defeat, and restart wording.

### UI and readability

- Retained touch-first portrait and landscape layouts.
- Kept large buttons and mobile-readable text.
- Improved board messaging for boss room, room clear, invalid taps, and skill targeting.
- Added clearer visual states for selected tiles, rejected tiles, valid move tiles, skill targets, enemy preview tiles, hazards, hit feedback, rewards, and end screens.

### Game feel

- Enemy turns now call out strikes and advances more clearly.
- Hit tiles flash with a bright readable highlight.
- Invalid taps are explicitly labeled as invalid.
- Reward selection and boss arrival use stronger wording.

### Balance

- Between-room recovery increased from 2 to 3 HP.
- Ember Bolt cooldown reduced from 2 to 1 for snappier mobile pacing.
- Lantern Warden health reduced from 15 to 14 to avoid a slow final-room stall.
- Enemy count, room count, biome order, enemy roster, boss identity, and reward choices are unchanged.

### Mobile readiness

- Safe-area support from Phase 4.5 is preserved.
- Runtime still targets 30 FPS and low-cost visual effects.
- iPhone export remains prepared, but this Windows machine does not have Unity iOS Build Support installed.

### Remaining known issues

- Visuals are still code-driven placeholder art.
- No physical iPhone validation has been performed.
- No Xcode project or signed iPhone build exists yet.
- The standalone repository still has no configured Git remote.

## Phase 5B tactical combat foundation notes

Phase 5B adds a small original tactics foundation inspired by classic grid combat without copying class names, spell names, formulas, UI, art, or exact mechanics from any reference game.

### AP / MP

- Player turns now start with Action Points for skills and Movement Points for tile movement.
- Moving spends MP by path length and no longer automatically ends the turn.
- Skills spend AP and can be combined in one turn if AP remains.
- End Turn explicitly hands control to enemies.
- HUD copy now shows HP, AP, MP, cooldowns, and AP costs.

### Classes

- Vanguard / Sun Spear: durable close-range kit with thrust, guard, and charge.
- Wayfinder / Prism Bow: long-range line attacker with shot, mark, and piercing hit.
- Cantor / Cinder Staff: ember spell kit with bolt, area burn, and delayed blast.
- Gloamstep / Echo Blades: mobility kit with diagonal dash, backstab, and swap.
- Artificer / Lenscaster: control kit with root trap, redirect shot, and shield gadget.

### Tactical mechanics

- Added reusable push, swap, shield, burn, root, mark, and delayed-area preview behavior.
- Added line-of-sight checks for line-style skills.
- Added affected-area preview tiles for area skills.
- Enemy previews remain simple and readable.

### Mobile readability

- Class selection is available from the start screen.
- Skill buttons show AP cost, readiness, and cooldown state.
- The board continues to use large color-coded overlays for move, target, danger, hazard, hit, and invalid feedback.

### Known Phase 5B limits

- Class kits are first-pass prototypes and need hands-on balance testing.
- No new enemy roster was added, so enemy variety is still intentionally small.
- Line-of-sight feedback is color/validity based; it does not yet draw an explicit blocked ray.
- Visuals remain placeholder IMGUI/code-driven art.
