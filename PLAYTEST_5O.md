# Phase 5O - Tactical Threat Upgrade Notes

Date: 2026-07-13

This phase responds to real live WebGL playtest feedback after Phase 5N. Scope stayed within the existing content: no new classes, enemies, bosses, biomes, rooms, monetization, online features, or final art overhaul.

## Feedback addressed

- Vanguard / spear run was won with 7 HP remaining.
- Rewards, reward cards, AP/MP, HUD width, board size, and clipping were reported as good.
- Healing pickups were not noticed.
- Enemy previews were too easy to avoid by stepping away and attacking safely.
- The Lantern Warden boss mostly followed the player and needed health-based phases.
- Line of sight existed, but rooms did not have enough blockers for it to matter.
- WebGL worked on phone, but phone loading was slow.

## Boss changes

- The existing Lantern Warden now has health phases:
  - Phase 1 above 66% HP: basic ward strike pressure.
  - Phase 2 from 66% to 33% HP: larger immediate pressure plus line/AP intent.
  - Phase 3 below 33% HP: storm blast intent with mixed HP/AP/MP pressure.
- Boss behavior remains previewed and readable; it should be harder to trivialize by simply backing away.

## Enemy threat changes

- Enemies now expose an intent label and threat type: HP, AP, MP, or mixed.
- Red tiles remain immediate danger.
- Purple intent tiles show delayed or AP/MP pressure.
- Gloom Archer can pressure AP.
- Stone Sentinel can pressure MP.
- Ashling threatens likely nearby escape spaces.
- Enemy repositioning now values delayed coverage as well as immediate preview coverage.

## Line-of-sight and blockers

- Later rooms can include a small number of connected tactical blockers.
- Blockers are not added to room 1.
- Blockers avoid player spawn, enemy spawns, hazards, props, and healing.
- All generated rooms remain connected, and enemies remain reachable.
- These blockers create simple line-of-sight choices without cluttering the board.

## Healing visibility

- Healing pickups are now drawn larger with a heart symbol and `HEAL +3` label.
- The playtest guide now calls out the green healing tile.
- Spawn/heal rules from Phase 5N remain: modest heal, not every room, no overheal, disappears on pickup.

## WebGL loading notes

- WebGL payload files are limited to the required Unity WebGL output under `docs/`.
- Compression remains disabled for simple GitHub Pages compatibility.
- WebGL data caching is enabled for repeat loads where supported.
- First mobile browser load may still be slow because Unity WebGL must download a sizeable `.wasm` payload.

## Manual seeded notes

- Seeded QA should compare Vanguard survivability before/after this phase.
- Expected outcome: guided players can still win, but boss phases and AP/MP pressure should force more defensive choices.
- Further difficulty changes should wait for more human playtest data.

## Known issues

- Enemy intent visuals are still lightweight prototype UI.
- AP/MP pressure is intentionally simple and may need tuning after external playtests.
- Mobile browser loading remains limited by Unity WebGL payload size and device performance.
