# Phase 5Q.2 - Emergency Mobile HUD Regression Fix

Date: 2026-07-14

This pass treats the previous live `Prototype v0.5Q.2` phone HUD as a failed regression. Real phone screenshots showed the board was too small, the portrait layout wasted space, and phone landscape still behaved like a cramped desktop side panel.

## What changed

- Phone portrait was rebuilt around a larger top board plus compact bottom controls.
- Phone landscape now uses a full-width bottom action bar instead of a narrow right-side panel.
- The portrait skill layout is now 2+1: two half-width skills, then one full-width skill.
- HP/AP/MP remain always visible in a compact row.
- Help and Info are small but still finger-sized in portrait.
- Landscape shows HP/AP/MP and one short status line above the action row.
- Long biome/hazard/debug/prototype text is not always visible in the phone combat HUD.
- The layout tests now reject the failed tiny-board strategy.

## Phone portrait expected layout

- Board uses the top ~40-45% of the usable phone viewport.
- Bottom HUD uses the remaining space without a giant blank footer.
- HP/AP/MP are shown at the top of the HUD.
- One short status line is visible.
- Skill 1 and Skill 2 sit side by side.
- Skill 3 is full width below them.
- End Turn is large at the bottom.

## Phone landscape expected layout

- Board uses the full width above the controls.
- Bottom action bar contains HP/AP/MP, one short status line, three skills, Cancel when needed, and End Turn.
- The old tiny right-side phone panel is no longer used.

## What to check on phone next

- Portrait: is the board large enough again?
- Portrait: is the big empty black area gone?
- Portrait: are HP/AP/MP, skills, and End Turn comfortable without zooming?
- Landscape: does the board feel much larger than the failed side-panel layout?
- Landscape: are the bottom action buttons readable and tappable?
- Confirm the visible build label/loading copy says `Prototype v0.5Q.2c`.

## Known issues

- This is still Unity IMGUI prototype UI, not final responsive UI tech.
- Real-phone screenshots remain the source of truth for further tuning.
