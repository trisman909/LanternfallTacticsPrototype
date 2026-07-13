# Lanternfall Tactics Prototype

A separate Unity 6 vertical slice for a mobile-friendly, turn-based tactical roguelite.

Open this folder in Unity 6000.5.1f1 and press Play. The runtime creates the prototype automatically; no scene setup is required. Mouse clicks emulate touch. Use the start screen, read the short How to Play panel, clear four generated encounters, choose one reward after each, then defeat the Lantern Warden in room five.

## Controls

- Tap/click a cyan tile to move.
- Tap a skill button, then a highlighted tile or enemy.
- Red floor overlays show attacks enemies have committed to for their next turn.
- Tap `How to Play` in-game if the color rules are unclear.
- `End Turn` resolves enemy previews when you are done spending AP/MP.

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
- Phase 5C - WebGL preview and GitHub Pages preparation (complete)
- Phase 5D - playtest balance and fun pass (complete)
- Phase 5E - browser/mobile playtest QA and usability fixes (complete)
- Phase 5F - visual identity polish (complete)
- Phase 5G - external playtest release prep (complete)

Each milestone preserves the original content limit until playtesting proves the core loop is fun.

## Five-room biome rotation

The run visits The Drowned Narthex, Siltglass Observatory, The Ember Ossuary, The Gloam Orchard, and Stormvault Foundry in order. Each room uses a lightweight palette adapted from the original Lanternfall production materials and one readable tactical hazard. See [BIOME_IDENTITY.md](BIOME_IDENTITY.md) for reuse details and manual theme notes.

## Mobile readiness

Portrait and short-landscape layouts are safe-area aware, use 48-pixel-or-larger interaction targets, and retain mouse support for editor testing. The runtime targets 30 FPS and uses no expensive visual effects. Android export settings and a build method are prepared, but this workstation does not have Unity Android Build Support installed; see [MOBILE_READINESS.md](MOBILE_READINESS.md).

iPhone settings, notch/Dynamic Island layout checks, low-end quality defaults, and an Xcode export method are prepared. Unity iOS Build Support is not installed here, and a Mac with Xcode is still required to compile, sign, and install the application. See [IOS_TESTING.md](IOS_TESTING.md).

## Build and test commands

Use Unity 6000.5.1f1.

- Open in Unity: open this project folder and press Play.
- Run EditMode tests: use Unity Test Runner, or run Unity batchmode with `-runTests -testPlatform EditMode`.
- Build Windows: run editor method `Lanternfall.EditorTools.BuildPrototype.BuildWindows`.
- Build WebGL: run editor method `Lanternfall.EditorTools.BuildPrototype.BuildWebGL`.

Current local Windows build path:

`Builds/Windows/LanternfallTactics.exe`

Current local WebGL build path:

`Builds/WebGL/LanternfallTactics`

GitHub Pages-ready WebGL files are prepared under:

`docs`

For GitHub Pages setup and WebGL limitations, see [WEBGL_PREVIEW.md](WEBGL_PREVIEW.md).

Expected GitHub Pages playtest URL after Pages is enabled:

`https://trisman909.github.io/LanternfallTacticsPrototype/`

For first external testers, use [PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md).

## Playable candidate notes

Phase 5 adds the start screen, help panel, clearer invalid-tap feedback, stronger tile/readability highlights, reward/end-state polish, and a small balance pass while preserving the original content cap. See [PROTOTYPE_NOTES.md](PROTOTYPE_NOTES.md).

Phase 5B adds an original class-based tactical foundation inspired by classic grid tactics: AP for skills, MP for movement, line-of-sight checks, reusable effects, and five small class kits. It does not add rooms, biomes, bosses, online features, monetization, or final art.

Phase 5C adds a browser-playable WebGL preview path and GitHub Pages-ready static files when Unity WebGL Build Support is installed. Compression is disabled for simple static hosting.

Phase 5D tunes the existing classes, skills, enemies, rewards, and hazards using deterministic seeded playtest notes. No new gameplay content was added. See [PLAYTEST_5D.md](PLAYTEST_5D.md).

Phase 5E verifies the WebGL preview in a local browser, tests phone-sized viewports, and fixes usability/layout issues found during QA. See [QA_5E.md](QA_5E.md).

Phase 5F keeps the prototype code-driven and lightweight while making the board, units, effects, biome hazards, reward cards, and panels feel more intentional and readable. See [VISUAL_POLISH_5F.md](VISUAL_POLISH_5F.md).

Phase 5G prepares the WebGL build for first external playtesting, adds the visible `Prototype v0.5G` label, verifies GitHub Pages-ready files, and adds a short tester guide/checklist. See [PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md).
