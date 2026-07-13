# Phase 5Q - Real Phone HUD Fix and Normal Enemy AI Regression Fix

Date: 2026-07-14

This phase responds to real phone WebGL screenshots after Phase 5P. The previous automated viewport checks passed, but the phone HUD was still too small and cramped in actual browser use.

## Phone HUD changes

- Portrait now prioritizes a comfortable lower action area over maximum board size.
- Portrait skill cards are stacked full-width instead of squeezed into three narrow columns.
- HP/AP/MP chips, End Turn, Help/Info, selected skill, and the short status message have larger minimum heights.
- Phone landscape uses a much wider right-side action panel instead of desktop-style proportions.
- Mobile font sizing now has a stronger phone scale so HUD text cannot shrink as far on small screens.
- WebGL cache bust is updated to `v=5Q` so the live page can pull the new layout.

## Minimum mobile layout targets

- Phone portrait font baseline: at least 22.
- Phone landscape font baseline: at least 24.
- Portrait skill cards: at least 64 px high and nearly full panel width.
- Portrait End Turn: at least 68 px high.
- Landscape HUD width: at least 450 px in tested phone-browser viewports.
- Landscape End Turn: at least 56 px high.
- Critical controls must remain within the visible safe area.

## Normal enemy AI fix

- Normal enemies now score repositioning by useful pressure instead of arbitrary reachable-tile order.
- Ashlings prefer closing toward melee pressure.
- Gloom Archers prefer clear line-of-sight firing positions.
- Stone Sentinels prefer positions that keep MP-bind pressure relevant.
- Idling is penalized unless the enemy already has a direct preview on the player.
- Enemy turn messaging now correctly reports when an enemy actually repositions.

## Boss fairness follow-up

- Low-health boss delayed AP/MP pressure no longer sneaks in hidden HP damage.
- Phase 2 is now a clear transition beat: the Warden pauses, announces overcharge, and gains `Overcharge Shield +4`.
- The Phase 2 message explains that range and AP/MP pressure increased.
- Phase 3 announces `HEAVY BLAST` before acting.
- Heavy boss preview tiles use a stronger `!!` marker and brighter outline.
- Boss HUD threat details now include timing, damage, and phase summary.

## Manual phone verification needed

On the live GitHub Pages build:

- Portrait: confirm HP/AP/MP are readable without zooming.
- Portrait: confirm all three skills are readable and comfortable to tap.
- Portrait: confirm End Turn is large enough to tap confidently.
- Portrait: confirm board is still playable even though it is smaller.
- Landscape: confirm the right HUD panel is no longer a tiny strip.
- Landscape: confirm skill cards and End Turn are comfortable.
- Combat: confirm normal enemies move toward useful positions instead of sliding left or doing nothing.
- Combat: confirm boss pressure still feels like Phase 5O/5P.
- Boss: confirm Phase 2 clearly announces the shield/range/AP-MP change before the boss attacks.
- Boss: confirm low-health attacks feel telegraphed and do not produce surprise hidden lethal bursts.

## Known issues

- Mobile browser play is still constrained by Safari/Chrome address bars.
- This is still IMGUI prototype UI, not a final mobile UI framework.
- Real-device confirmation is still required because screenshots already proved idealized viewport tests were not enough.
- Boss fairness still needs another real playtest pass because the original issue came from in-fight perception, not only numbers.
