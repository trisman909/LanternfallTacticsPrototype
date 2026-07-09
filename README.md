# Lanternfall Tactics Prototype

A separate Unity 6 vertical slice for a mobile-friendly, turn-based tactical roguelite.

Open this folder in Unity 6000.5.1f1 and press Play. The runtime creates the prototype automatically; no scene setup is required. Mouse clicks emulate touch. Use the start screen, read the short How to Play panel, clear four generated encounters, choose one reward after each, then defeat the Lantern Warden in room five.

## Controls

- Tap/click a cyan tile to move.
- Tap a skill button, then a highlighted tile or enemy.
- Red floor overlays show attacks enemies have committed to for their next turn.
- Tap `How to Play` in-game if the color rules are unclear.
- `Wait` spends the turn without moving.

## Scope

Five original class frameworks, three skills per class, AP/MP turn economy, one connected room generator, five reused Lanternfall biome identities, three enemy types, one boss, one reward choice flow, five rooms, win/loss states, and EditMode tests.

## Milestones

- Phase 1 - playable vertical slice (complete)
- Phase 2 - core-loop and mobile-readability polish (complete)
- Phase 3 - playtesting and numerical tuning (complete)
- Phase 3.5 - original Lanternfall biome identity reuse (complete)
- Phase 4 - mobile build readiness and touch validation (complete; physical Android device pending)
- Phase 4.5 - iOS export preparation (complete; Xcode export and device test pending)
- Phase 5 - playable candidate polish and final prototype verification (complete)
- Phase 5B - tactical combat foundation with AP/MP and class skills (complete)

Each milestone preserves the original content limit until playtesting proves the core loop is fun.

## Five-room biome rotation

The run visits The Drowned Narthex, Siltglass Observatory, The Ember Ossuary, The Gloam Orchard, and Stormvault Foundry in order. Each room uses a lightweight palette adapted from the original Lanternfall production materials and one readable tactical hazard. See [BIOME_IDENTITY.md](BIOME_IDENTITY.md) for reuse details and manual theme notes.

## Mobile readiness

Portrait and short-landscape layouts are safe-area aware, use 48-pixel-or-larger interaction targets, and retain mouse support for editor testing. The runtime targets 30 FPS and uses no expensive visual effects. Android export settings and a build method are prepared, but this workstation does not have Unity Android Build Support installed; see [MOBILE_READINESS.md](MOBILE_READINESS.md).

iPhone settings, notch/Dynamic Island layout checks, low-end quality defaults, and an Xcode export method are prepared. Unity iOS Build Support is not installed here, and a Mac with Xcode is still required to compile, sign, and install the application. See [IOS_TESTING.md](IOS_TESTING.md).

## Playable candidate notes

Phase 5 adds the start screen, help panel, clearer invalid-tap feedback, stronger tile/readability highlights, reward/end-state polish, and a small balance pass while preserving the original content cap. See [PROTOTYPE_NOTES.md](PROTOTYPE_NOTES.md).

Phase 5B adds an original class-based tactical foundation inspired by classic grid tactics: AP for skills, MP for movement, line-of-sight checks, reusable effects, and five small class kits. It does not add rooms, biomes, bosses, online features, monetization, or final art.
