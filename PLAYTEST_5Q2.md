# Phase 5Q.2 - True Real-Phone HUD Redesign

Date: 2026-07-14

This pass responds to another real-phone playtest where the board was playable but the combat HUD was still too small for comfortable use.

## What changed

- Phone-sized screens now use a true phone HUD mode instead of a squeezed desktop HUD.
- Phone portrait prioritizes a large lower action panel and accepts a smaller board.
- Phone landscape gives most of the width to the action HUD instead of a narrow desktop-style side strip.
- Secondary combat details are no longer always visible on phone combat HUD.
- Tiny footer/version text is hidden during phone combat.
- Skill cards use compact mobile labels such as `BOLT`, `BLOOM`, `BLAST`, `SPEAR`, `GUARD`, and `CHARGE`.
- Long skill descriptions remain available in the richer desktop HUD / help context, not on phone action cards.

## Phone portrait expected layout

- Board stays in the upper section.
- Lower action area dominates the screen.
- HP/AP/MP chips are large.
- Three skills are stacked full-width.
- End Turn is a large button.
- One compact status/danger message is shown.
- Help/Info/details do not consume combat action space.

## Phone landscape expected layout

- Board remains playable on the left.
- Right action area is much wider than before.
- Three large skill cards sit in one readable row.
- End Turn is large.
- HP/AP/MP are large.
- Help/Info/details are not always visible in the action area.

## Manual phone verification

- Portrait: can both players read HP/AP/MP without zooming?
- Portrait: are all three skills comfortable to read and tap?
- Portrait: is End Turn obvious and comfortable?
- Portrait: is the smaller board still playable?
- Landscape: does the HUD finally stop feeling like a tiny right strip?
- Landscape: are skill cards and End Turn readable at normal phone distance?
- Browser mode: confirm it is usable even with the address bar visible.
- Optional: compare normal browser mode against landscape/fullscreen/Add to Home Screen.

## Known issues

- This is still Unity IMGUI prototype UI, not a final responsive UI framework.
- Phone browser chrome can still vary by device and browser.
- Real-phone screenshots remain the source of truth for further HUD tuning.

