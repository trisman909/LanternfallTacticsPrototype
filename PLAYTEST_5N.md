# Phase 5N - Healing Pickups, Reward Layout, and Difficulty Notes

Date: 2026-07-13

Scope stayed limited to the existing prototype content: no new classes, enemies, bosses, biomes, rooms, combat systems, monetization, online features, or final art.

## Player feedback addressed

- The live WebGL build felt much better overall.
- The tactical combat direction still felt worth continuing.
- Limited health between rooms felt good and should not become a full free refill.
- Damaged players may feel forced into HP rewards, so small optional board healing was requested.
- The right-side `ROOM CLEAR / CHOOSE ONE` reward area could visually crop or collide with reward cards.
- Difficulty could become slightly tighter again, as long as room 1 stayed fair and enemy previews remained readable.

## Reward layout notes

- The room-clear heading now uses a two-line compact header with fixed spacing before cards.
- Side-panel reward cards use shared layout constants so tests can verify header/cards do not overlap.
- Portrait reward cards keep the compact three-card row but now share the same spacing contract.
- Reward card copy from Phase 5M is preserved.

## Healing pickup notes

- Automatic between-room recovery is now `0`; room clears do not heal for free.
- A single `Lantern bloom` healing pickup can appear in some non-boss rooms.
- Pickup chance is intentionally modest: about 35% for rooms 2-4 only.
- No pickup appears in room 1 or the boss room.
- Pickup heals `3 HP`, cannot exceed max HP, and disappears when collected.
- Generator rules prevent pickups from spawning on the player start, enemies, hazards, props, blocked tiles, or unreachable tiles.
- The pickup is drawn as a clear green board marker with a `+` glyph.

## Difficulty notes

- Room 1 is unchanged.
- Later normal rooms apply a small additional damage bump from room 4 onward.
- The existing boss receives a small health increase through room scaling.
- Enemy count, enemy roster, room count, class kits, AP/MP values, rewards, and biome hazards are otherwise unchanged.

## Manual QA notes

- Reward layout should be checked in the live WebGL build after deployment at a normal desktop browser size.
- Healing pickup behavior should be checked by playing multiple seeded/random runs; because pickups are optional, not every run will show one before the boss.
- Balance is intentionally a light nudge, not a full rebalance. Further difficulty tuning should wait for more human playtest data.

## Known issues

- Healing pickup placement is simple and uses existing board-generation data; it is readable, not visually final.
- Mobile browser play remains experimental.
- No additional external playtest beyond the feedback above is recorded in this file.
