# Phase 5Q.4 - Mobile Layout Hard Fix

Date: 2026-07-14

This pass treats the real-phone screenshots as the source of truth. Phone portrait is no longer a playable layout, and phone landscape gets the dedicated mobile command layout rather than falling back to a cramped desktop side panel.

## Portrait behavior

- Phone portrait is blocked by Unity and by the WebGL page HTML/CSS/JS.
- The browser page uses real `window.innerWidth` / `window.innerHeight` checks and shows a full-screen rotate prompt before the Unity UI can leak through.
- Expected text:
  - `Rotate your phone to play`
  - `Lanternfall Tactics is best played in landscape`
  - `Add to Home Screen for more space`
- The board, combat HUD, skill cards, and End Turn button should not be visible in phone portrait.

## Landscape behavior

- Phone landscape uses a full-width board above a bottom action HUD.
- The bottom HUD shows only the essentials:
  - HP / AP / MP
  - one short turn/status line
  - three large skill buttons
  - Cancel when relevant
  - large End Turn
- Long biome explanations, help/details, prototype footer text, and large log areas stay hidden or collapsed during active phone play.
- Detection now covers wider/taller real browser landscape viewports up to 1200 x 620 CSS pixels.

## Desktop preservation

- Desktop WebGL still uses the richer board plus right-side HUD layout.
- The portrait blocker is limited to likely phone/touch portrait dimensions.

## What to check on phone next

- Portrait: confirm only the rotate-device prompt appears.
- Landscape: confirm the game does not show the skinny desktop right panel.
- Landscape: confirm the board is playable and centered above the bottom controls.
- Landscape: confirm HP/AP/MP, all three skills, and End Turn are readable and comfortable to tap.
- If the old version appears, refresh the page or clear browser cache; the WebGL cache key should show `v=5Q4`.

## Known issues

- This is still Unity IMGUI prototype UI, not final responsive UI tech.
- Real-phone screenshots remain the source of truth for further tuning.
