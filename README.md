# Lanternfall Tactics Prototype

A separate Unity 6 vertical slice for a mobile-friendly, turn-based tactical roguelite.

Open this folder in Unity 6000.5.1f1 and press Play. The runtime creates the prototype automatically; no scene setup is required. Mouse clicks emulate touch. Clear four generated encounters, choose one reward after each, then defeat the Lantern Warden in room five.

## Controls

- Tap/click a cyan tile to move.
- Tap a skill button, then a highlighted tile or enemy.
- Red floor overlays show attacks enemies have committed to for their next turn.
- `Wait` spends the turn without moving.

## Scope

One Warden class, three skills, one connected crypt biome generator, three enemy types, one boss, one reward choice, five rooms, win/loss states, and EditMode tests.

## Milestones

- Phase 1 — playable vertical slice (complete)
- Phase 2 — core-loop and mobile-readability polish (complete)
- Phase 3 — playtesting and numerical tuning (complete)
- Phase 3.5 — original Lanternfall biome identity reuse (complete)
- Phase 4 — mobile build readiness and touch validation (complete; physical Android device pending)
- Phase 4.5 — iOS export preparation (complete; Xcode export and device test pending)
- Phase 5 — final prototype verification

Each milestone preserves the original content limit until playtesting proves the core loop is fun.

## Five-room biome rotation

The run now visits The Drowned Narthex, Siltglass Observatory, The Ember Ossuary, The Gloam Orchard, and Stormvault Foundry in order. Each room uses a lightweight palette adapted from the original Lanternfall production materials and one readable tactical hazard. See [BIOME_IDENTITY.md](BIOME_IDENTITY.md) for reuse details and manual theme notes.

## Mobile readiness

Portrait and short-landscape layouts are safe-area aware, use 48-pixel-or-larger interaction targets, and retain mouse support for editor testing. The runtime targets 30 FPS and uses no expensive visual effects. Android export settings and a build method are prepared, but this workstation does not have Unity Android Build Support installed; see [MOBILE_READINESS.md](MOBILE_READINESS.md).

iPhone settings, notch/Dynamic Island layout checks, low-end quality defaults, and an Xcode export method are prepared. Unity iOS Build Support is not installed here, and a Mac with Xcode is still required to compile, sign, and install the application. See [IOS_TESTING.md](IOS_TESTING.md).
