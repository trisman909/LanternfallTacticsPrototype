# Phase 5P - Mobile HUD Playability and Threat Clarity Notes

Date: 2026-07-13

This phase responds to real playtest feedback after the Phase 5O tactical threat upgrade. Difficulty, boss phases, AP/MP pressure, line-of-sight blockers, healing pickups, classes, biomes, enemies, boss, and room count were preserved.

## Feedback addressed

- The game felt better and more tactical.
- Boss difficulty and AP/MP pressure were worth keeping.
- The main issue became board clutter: too many AP/MP labels, danger labels, icons, and intent markers were visible at once.
- Phone portrait loaded but squeezed the HUD into the bottom, making skills, End Turn, and HP/AP/MP hard to use.
- Phone landscape technically fit, but the right HUD, skill cards, and stat chips were too small.

## Phone layout fixes

- Portrait now reserves a larger lower action panel for combat controls.
- Phone landscape uses a wider HUD than desktop/large-tablet landscape.
- All three skill cards, HP/AP/MP chips, Help/Info, End Turn, and the message area are required to fit without overlap.
- Short-landscape tests now require full 48px touch targets instead of accepting smaller emergency buttons.
- The WebGL template keeps the canvas on dynamic/small viewport height and safe-area-aware padding for mobile browser chrome.

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
- Additional layout tests cover common phone portrait and phone landscape sizes, requiring all three skills, End Turn, and HP/AP/MP to remain readable and tappable.
- Further clarity tuning should use screenshots or phone/desktop playtest feedback.

## Known issues

- Threat icons are still prototype symbols, not final art.
- Tile focus/tooltip behavior is intentionally simple and based on tapped/current tiles.
- More sophisticated hover/tap inspection can be considered later if playtesters still miss threat meanings.
- Physical phone confirmation is still useful after the live GitHub Pages cache updates on each browser.
