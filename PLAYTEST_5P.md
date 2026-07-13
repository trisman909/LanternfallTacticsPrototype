# Phase 5P - Threat Readability and Clutter Reduction Notes

Date: 2026-07-13

This phase responds to real playtest feedback after the Phase 5O tactical threat upgrade. Difficulty, boss phases, AP/MP pressure, line-of-sight blockers, healing pickups, classes, biomes, enemies, boss, and room count were preserved.

## Feedback addressed

- The game felt better and more tactical.
- Boss difficulty and AP/MP pressure were worth keeping.
- The main issue became board clutter: too many AP/MP labels, danger labels, icons, and intent markers were visible at once.

## Clutter reduction

- Delayed/AP/MP threat tiles no longer print repeated `AP` or `MP` text across the board.
- Delayed threat tiles now use compact symbols with threat-coloured outlines.
- Enemy intent text above units was replaced by one compact badge per enemy.
- Board detail is now colour/border/symbol first, text second.

## Threat hierarchy

- Red remains immediate danger.
- Purple remains delayed/casting/AP/MP pressure.
- Gold remains selected skill/targeting.
- Cyan remains movement.
- Biome hazard symbols remain biome-specific.
- Detailed explanation is shown in the HUD/message area instead of stamped repeatedly on tiles.

## HUD threat detail

The HUD can explain the currently focused/tapped tile:

- Enemy name
- HP/AP/MP threat type
- Whether it triggers now or next turn
- Hazard, healing, or blocker details when relevant

Examples:

- `Gloom Archer: AP drain next turn`
- `Stone Sentinel: MP bind now`
- `Lantern Warden: HP + AP/MP threat next turn`

## Validation notes

- Automated tests check compact threat markers, enemy badges, HUD threat detail, distinct danger colours, mobile controls, boss phases, AP/MP pressure, classes, biomes, and WebGL docs payload.
- Further clarity tuning should use screenshots or phone/desktop playtest feedback.

## Known issues

- Threat icons are still prototype symbols, not final art.
- Tile focus/tooltip behavior is intentionally simple and based on tapped/current tiles.
- More sophisticated hover/tap inspection can be considered later if playtesters still miss threat meanings.
