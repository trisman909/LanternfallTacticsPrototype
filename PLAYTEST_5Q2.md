# Phase 5Q.3 - Mobile Landscape-First HUD Redesign

Date: 2026-07-14

This pass accepts the real-phone finding that portrait is not a good fit for Lanternfall Tactics. The game is now mobile landscape-first: phone portrait shows a clean rotate-device screen instead of attempting to show the full board and combat HUD.

## What changed

- Phone portrait no longer shows the playable board/HUD.
- Phone portrait shows a centered rotate instruction: `Rotate your phone to play` and `Landscape mode recommended`.
- Phone landscape is the real mobile layout.
- Phone landscape uses a full-width board above a bottom command bar.
- The bottom command bar keeps HP/AP/MP, one short status line, three skills, Cancel when relevant, and End Turn visible.
- Long biome/hazard/debug/prototype text is hidden from the always-visible mobile combat HUD.
- Skill cards use short mobile labels and show AP plus READY/CD/blocked state.

## Phone portrait expected behavior

- No tiny board.
- No tiny combat HUD.
- Clean rotate-device screen only.
- Optional fullscreen/Add to Home Screen guidance is visible.

## Phone landscape expected layout

- Board takes the full width above the controls.
- Bottom action bar contains HP/AP/MP and a short turn/status line.
- Three large skill cards sit in the action row.
- End Turn is a large, readable button.
- Help/Info/details are hidden/collapsed during combat.

## What to check on phone next

- Portrait: does it show only the rotate-device instruction?
- Landscape: is the board large enough to play comfortably?
- Landscape: are HP/AP/MP readable without zooming?
- Landscape: are all three skills readable and tappable?
- Landscape: is End Turn readable and tappable?
- Confirm the visible build/loading copy says `Prototype v0.5Q.3`.

## Known issues

- This is still Unity IMGUI prototype UI, not final responsive UI tech.
- Real-phone screenshots remain the source of truth for further tuning.
