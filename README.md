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
- Phase 4 - mobile build readiness and touch validation (complete; physical mobile-device test pending)
- Phase 4.5 - iOS export preparation (complete; Xcode export and device test pending)
- Phase 5 - playable candidate polish and final prototype verification (complete)
- Phase 5B - tactical combat foundation with AP/MP and class skills (complete)
- Phase 5C - WebGL preview and GitHub Pages preparation (complete)
- Phase 5D - playtest balance and fun pass (complete)
- Phase 5E - browser/mobile playtest QA and usability fixes (complete)
- Phase 5F - visual identity polish (complete)
- Phase 5G - external playtest release prep (complete)
- Phase 5H - share-ready playtest polish (complete)
- Phase 5I - lightweight game-feel polish (complete)
- Phase 5J - first feedback fix pass setup (complete)
- Phase 5K HUD clarity rework - combat HUD/skill panel fix from live WebGL playtest feedback (complete)
- Phase 5K.1 - live browser HUD fix for cropped desktop WebGL right panel text (complete)
- Phase 5K.2 - desktop WebGL board scale and HUD width tuning (complete)
- Phase 5L - first external playtest feedback package and safe clarity fix pass (complete)
- Phase 5M - first external feedback response for onboarding, rewards, AI, and tactical difficulty (complete)
- Phase 5N - optional healing pickups, reward layout fix, and slight difficulty tightening (complete)
- Phase 5O - tactical threat upgrade with smarter enemy intent, boss phases, blockers, and healing visibility (complete)
- Phase 5O.1 - mobile WebGL HUD playability fix for phone portrait and landscape (complete)
- Phase 5P - mobile HUD playability and threat clarity cleanup (complete)
- Phase 5Q - real phone HUD readability and normal enemy AI regression fix (complete)
- Phase 5Q.3 - mobile landscape-first HUD redesign (complete)
- Phase 6C - visual language and lightweight icon foundation (complete)
- Phase 6D - authored icon and UI art pass (complete)
- Phase 6E - authored player, enemy, and boss sprite pass (complete)
- Phase 6F - authored five-biome environment pass (complete)
- Phase 6G - lightweight combat effects and animation pass (complete)
- Phase 6H - visual readability and art-direction polish (complete)
- Phase 6I - presentation integration and cleanup (complete)
- Phase 6I.2 - gameplay polish, AI fixes, hazard audit, and range readability (complete)
- Phase 6J - group AI coordination and hazard containment polish (complete)
- Phase 6K - combat fairness, telegraph accuracy, and gameplay readability (complete)
- Phase 6K.1 - tactical clarity, environmental hazard integration, and boss-room decluttering (complete)
- Phase 6L - mobile polish, Sentinel AI correction, terrain integration, status clarity, and audio foundation (complete)
- Phase 6M - Ashling preview parity, Sentinel movement correction, and mobile tactical threat HUD (complete)
- Phase 6N.1 - readability-first phone HUD and larger-board layout (complete)
- Phase 6N.2 - phone HUD fit, board scale, and spacing polish (complete)
- Phase 6N.3 - real-device phone HUD correction (local review candidate; deployment pending physical-device approval)

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

Best played first on a desktop browser. Mobile browser testing is supported but still experimental.

For first external testers, use [PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md).

For recording first tester notes, use [PLAYTEST_FEEDBACK_LOG.md](PLAYTEST_FEEDBACK_LOG.md).

## Playable candidate notes

Phase 5 adds the start screen, help panel, clearer invalid-tap feedback, stronger tile/readability highlights, reward/end-state polish, and a small balance pass while preserving the original content cap. See [PROTOTYPE_NOTES.md](PROTOTYPE_NOTES.md).

Phase 5B adds an original class-based tactical foundation inspired by classic grid tactics: AP for skills, MP for movement, line-of-sight checks, reusable effects, and five small class kits. It does not add rooms, biomes, bosses, online features, monetization, or final art.

Phase 5C adds a browser-playable WebGL preview path and GitHub Pages-ready static files when Unity WebGL Build Support is installed. Compression is disabled for simple static hosting.

Phase 5D tunes the existing classes, skills, enemies, rewards, and hazards using deterministic seeded playtest notes. No new gameplay content was added. See [PLAYTEST_5D.md](PLAYTEST_5D.md).

Phase 5E verifies the WebGL preview in a local browser, tests phone-sized viewports, and fixes usability/layout issues found during QA. See [QA_5E.md](QA_5E.md).

Phase 5F keeps the prototype code-driven and lightweight while making the board, units, effects, biome hazards, reward cards, and panels feel more intentional and readable. See [VISUAL_POLISH_5F.md](VISUAL_POLISH_5F.md).

Phase 5G prepares the WebGL build for first external playtesting, adds the visible `Prototype v0.5G` label, verifies GitHub Pages-ready files, and adds a short tester guide/checklist. See [PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md).

Phase 5H improves first-minute share clarity without adding content: clearer start/help instructions, a WebGL loading hint, known limitations for testers, and a feedback checklist suitable for friends trying the browser build.

Phase 5I improves lightweight game feel through clearer transition, invalid-action, reward, skill-result, victory, and defeat feedback while keeping the prototype asset-light. See [GAME_FEEL_5I.md](GAME_FEEL_5I.md).

Phase 5J prepares the project to receive first external playtest feedback. No external feedback is recorded yet, so it adds a feedback log/template and preserves scope for future triage. See [PLAYTEST_FEEDBACK_LOG.md](PLAYTEST_FEEDBACK_LOG.md).

Phase 5K adds safe offline polish and automated QA hardening: clearer AP/MP wording, centralized reward labels, stronger WebGL troubleshooting notes, and additional tests for share/readability contracts.

Phase 5L adds release-hardening instrumentation without analytics: `Prototype v0.5L`, a local Playtest Info panel, safer WebGL loading/recovery text, and additional generated-folder/runtime-log QA checks.

Phase 5M strengthens automated regression coverage for invalid reward choices, cooldown rejection, invalid targets, class start/action labels, enemy preview tile validity, and future-safe WebGL/mobile contracts.

Phase 5K HUD clarity rework responds to live WebGL playtest feedback by replacing the cramped in-combat status block with clear HP/AP/MP chips, contained combat messages, cleaner skill cards, collapsed Help/Info access, and layout tests for desktop WebGL plus mobile landscape.

Phase 5K.1 tightens the live desktop WebGL combat HUD after screenshot feedback: safer top padding, smaller right-panel fonts, clearer vertical section order, taller skill cards, compact skill summaries, and stricter layout tests to prevent cropped header/skill text.

Phase 5K.2 gives the tactics board more desktop/WebGL priority without changing combat: the right HUD panel is slightly narrower, board fitting uses the actual playable floor footprint instead of the full generator rectangle, and tests preserve HUD readability while requiring stronger desktop board sizing.

Phase 5L prepares the prototype for real external testers: shorter first-player instructions, an in-game playtest prompt, a focused feedback guide/checklist, and a feedback log template. No external feedback has been invented or recorded yet.

Phase 5M responds to the first real external playtest: clearer onboarding, readable reward cards, lower passive recovery, stronger room-depth scaling, smarter enemy repositioning, and a tougher existing boss. See [PLAYTEST_5M.md](PLAYTEST_5M.md).

Phase 5N responds to follow-up live feedback: automatic room-clear healing is removed, optional modest board healing pickups can appear in some rooms, reward layout spacing is protected against header/card overlap, and later-room enemy pressure is nudged up slightly. See [PLAYTEST_5N.md](PLAYTEST_5N.md).

Phase 5O responds to another live playtest: enemies now show immediate and delayed/AP/MP intent, the existing boss has health-based phases, later rooms gain light line-of-sight blockers, healing pickups are more visible, and WebGL repeat-load caching is enabled while preserving GitHub Pages compatibility. See [PLAYTEST_5O.md](PLAYTEST_5O.md).

Phase 5O.1 responds to phone WebGL playtest feedback by giving portrait and phone-landscape layouts dedicated readable HUD sizing: all three skills, HP/AP/MP, and End Turn are kept visible and finger-sized, with mobile viewport CSS adjusted for browser chrome and safe areas.

Phase 5P preserves the improved Phase 5O difficulty while fixing the highest-priority phone WebGL usability feedback: portrait gets a larger action panel, phone landscape gets a wider readable HUD, all skills/End Turn/HP/AP/MP are protected by layout tests, repeated AP/MP tile text is replaced by compact markers, enemy intent labels become small badges, and detailed threat explanations move into the HUD/message panel. See [PLAYTEST_5P.md](PLAYTEST_5P.md).

Phase 5Q responds to follow-up real phone screenshots by prioritizing comfort over merely fitting: portrait uses stacked full-width skill cards, larger AP/MP/HP chips, larger End Turn, and reduced board height; phone landscape uses a substantially wider action HUD. It also fixes a normal-enemy AI regression so non-boss enemies reposition toward useful pressure/line-of-sight positions instead of drifting left or idling. See [PLAYTEST_5Q.md](PLAYTEST_5Q.md).

Phase 5Q.4 is an emergency mobile layout hard fix from real phone screenshots: phone portrait is blocked in both Unity and the WebGL page itself, and phone landscape uses more robust viewport detection so it gets the real bottom command HUD instead of a tiny desktop side panel. See [PLAYTEST_5Q4.md](PLAYTEST_5Q4.md).

Phase 6A focuses on mobile UX, tactical AI, and first-time polish without adding content: phone landscape uses a dedicated bottom command bar, default combat HUD information is reduced, enemies score repositioning by role/cooperation, and the Lantern Warden Phase 2 transition gets a readable banner/presentation beat.

Phase 6B is a real iPhone landscape layout fix: phone landscape now uses explicit responsive modes and a stacked bottom command HUD, while the WebGL shell avoids `100vw` plus safe-area overflow so the board and HUD stay inside the visible iPhone browser viewport.

Phase 6B.1 keeps that accepted layout and improves real-phone readability with larger mobile landscape HUD text, slightly taller stat/skill/action rows, and compact two-line skill labels.

Phase 6C replaces repeated board text with a 20-icon, code-drawn visual language for stats, statuses, threats, blockers, healing, and all five biome hazards. Unit tokens gain class/enemy symbols and a distinct Lantern Warden frame without changing silhouettes, colors, gameplay, or the accepted Phase 6B.1 layouts. See [PLAYTEST_6C.md](PLAYTEST_6C.md).

Phase 6D replaces the visible procedural icon set and plain combat frames with two original, lightweight hand-painted atlases inspired by the official Lanternfall concept sheet. Authored icons cover resources, statuses, targeting, danger, pickups, obstacles, and biome hazards; authored frames cover skills, stat chips, command buttons, utility buttons, selected-skill information, rewards, outcomes, and tooltips. Procedural drawing remains only as a load-failure fallback. See [ART_ASSETS_6D.md](ART_ASSETS_6D.md).

Phase 6E replaces visible letter/abstract unit bodies with a single original nine-cell top-down production-placeholder atlas covering all five player classes, Ashling, Gloom Archer, Stone Sentinel, and Lantern Warden. Existing selection, health, shield, hit, and status communication remains layered above the sprites; Lantern Warden overcharge gains a distinct magenta frame and overcharge badge. See [ART_ASSETS_6E.md](ART_ASSETS_6E.md).

Phase 6F replaces procedural board surfaces and prop glyphs with five compact original production-placeholder biome atlases. Every biome now has distinct floor variants, blockers, hazard treatment, healing pickup, props, background palette, and a sparse boss-room accent while all tactical overlays and rules remain unchanged. See [ART_ASSETS_6F.md](ART_ASSETS_6F.md).

Phase 6G adds a presentation-only combat-effects layer: short unit interpolation, movement trails, destination pulses, action-family attack cues, status auras, death fades, threat breathing, and room/outcome banners. A persisted Full/Reduced Motion toggle is available in How to Play. The layer uses existing IMGUI primitives and cached art, with no particles, shaders, gameplay waits, or balance changes. See [EFFECTS_6G.md](EFFECTS_6G.md).

Phase 6H is a strict readability and art-direction polish pass. Its post-integration refinement normalizes the five authored biome atlases with per-biome contrast control, a consistent fixed painted-light direction, quieter floors beneath tactical states, sparse structural props, and a clearer player/enemy/boss scale hierarchy. No content, effects, mechanics, maps, shaders, particles, animations, or gameplay systems were added. See [READABILITY_6H.md](READABILITY_6H.md).

Phase 6I integrates and cleans the complete presentation stack. Obsolete letter, prop-glyph, floor-glyph, and mojibake fallbacks are removed; reliability fallbacks now use the authored icon language. First-time instructions explicitly cover hero identity, movement, selected skills, enemy HP/AP/MP intent, danger timing, End Turn, rewards, and boss Phase 2. The WebGL shell gains a themed loading card and landscape install manifest. See [INTEGRATION_6I.md](INTEGRATION_6I.md).

Phase 6I.2 keeps the existing content while making its rules more predictable. Skill selection always exposes maximum reach, legal targets, blocked spaces, and out-of-range floor. Enemies use deterministic hazard costs, remember their previous position, commit toward useful destinations, and attack after repositioning when the player is in range. Hazard timing, procedural connectivity, and the widened late-phase boss telegraph are covered by regression tests. See [GAMEPLAY_POLISH_6I.md](GAMEPLAY_POLISH_6I.md).

Phase 6J plans each enemy turn as a coordinated squad. Role-ordered destination and attack reservations prevent collisions, reward distinct flanks and uncovered escape pressure, keep archers separated behind the front line, and limit adjacent escape denial to preserve fairness. Hazard icons retain an 82% footprint with equal padding and an explicit tile clip. See [AI_COORDINATION_6J.md](AI_COORDINATION_6J.md).

Phase 6K makes the combat pipeline fail closed against invisible damage. Enemy and hazard telegraphs are snapshotted when End Turn is pressed; every player HP loss is validated against that committed tile set. Same-turn move-and-hit is removed, charged-floor splash tiles are visibly armed, hazards use a quieter 68% footprint, area skills expose their potential impact, and active biome effects receive an explicit player frame and message. See [COMBAT_FAIRNESS_6K.md](COMBAT_FAIRNESS_6K.md).

Phase 6K.1 removes the floating hazard-icon layer and blends biome-authored hazard art into 94% of each tile with darker, desaturated grading. Current and future enemy regions use clean merged boundaries, selected-skill mode suppresses unrelated warnings, the right HUD uses wrapped range guidance and a simple Current/Next/Then sequence, and boss rooms reserve more clear floor around the Lantern Warden. No new content or combat rules were added. See [TACTICAL_CLARITY_6K1.md](TACTICAL_CLARITY_6K1.md).

Phase 6L modestly strengthens future-threat borders on phones, grades props into their biome floor, clips merged threat boundaries at walls and gaps, enlarges backed status badges, and replaces invalid-target art with concise wrapped reasons. Stone Sentinels now advance unless they are genuinely controlling close space, a chokepoint, or a ranged ally; their HP rises from 6 to 7 without a damage increase. An original procedural audio foundation adds short cues, two lightweight loops, first-interaction WebGL unlock, saved master/SFX/music/mute controls, and finite-value guards at both the saved-setting and browser-audio boundaries. See [POLISH_6L.md](POLISH_6L.md).

Phase 6M identifies the Ashling's confusing damage as its existing delayed Flame Sigil path, separates its purple committed tiles from immediate melee damage, records named damage sources, and gives the phone HUD structured immediate/delayed/status/control/movement sections. Stone Sentinels no longer treat a generic distance-two MP preview as permission to stall in open space; MP Bind is reserved for a real chokepoint action and ordinary frontline plans advance. See [PHASE_6M.md](PHASE_6M.md).

Phase 6N.1 reorganizes phone landscape into a wide top status/title bar, a bottom three-skill bar, and a right threat/End Turn rail. This reclaims vertical board space while keeping HP/AP/MP, threats, skills, and End Turn comfortably readable with the existing Lanternfall frames, icons, colours, and typography. Desktop and portrait behavior are unchanged. See [PHASE_6N1.md](PHASE_6N1.md).

The Phase 6N.1 provenance correction pins WebGL to CSS-pixel resolution so high-density phones reliably select that phone layout. See [PHASE_6N1_PROVENANCE.md](PHASE_6N1_PROVENANCE.md).

Phase 6N.2 preserves the L-shaped phone HUD while making its bars slimmer, removing the duplicate board header, fitting the occupied room at the largest safe scale, containing skill labels inside consistent inner bounds, and aspect-fitting End Turn artwork inside its full-size touch target. Desktop and gameplay behavior are unchanged. See [PHASE_6N2.md](PHASE_6N2.md).

Phase 6N.3 responds directly to physical iPhone 16 Pro screenshots. Phone skill cards use fixed name, AP-cost, and state baselines; selected state no longer changes text geometry; the intentional targeting-cancel X has a fixed slot; End Turn uses separate artwork, label-safe, and hit rectangles; the HUD footprint is reduced again; and phone board fitting uses occupied floor bounds with a small effect margin. Deployment remains gated on local screenshot review. See [PHASE_6N3.md](PHASE_6N3.md).





