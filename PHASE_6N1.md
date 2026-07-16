# Phase 6N.1 — Mobile HUD and board layout redesign

Phase 6N.1 is presentation-only. It changes no combat, AI, balance, audio, animation, camera, movement, or pathfinding behavior.

## Phone landscape architecture

The old bottom-only command stack constrained the board vertically. Phone landscape now uses an L-shaped arrangement built entirely from existing Lanternfall UI skins:

- a wide top bar for large HP/AP/MP chips and the centered turn/biome title;
- the board between the top and bottom bars, with no redundant internal header allowance;
- three wide skill cards in a shallow bottom bar;
- a 210–250 pixel right rail for relevant threat categories, Help/Info, Cancel, and a prominent End Turn button.

At 844×390 the board height increases by more than 20%. At 932×430 it increases by more than 12%. Tiles remain fully contained; the change comes from reclaiming HUD padding and relocating End Turn rather than camera zoom.

## Readability hierarchy

HP/AP/MP retain the existing large phone font and authored icons. Threats use compact category headings and one concise action per relevant category, ordered as immediate, delayed, active, control, then movement. Empty categories collapse. Skills remain at least 56 pixels tall and End Turn remains at least 56 pixels tall. Portrait continues to show the rotation prompt.

Desktop retains the existing side-panel layout at 1280×720 and 1920×1080. No new textures, fonts, shaders, or UI artwork were added.
