# Phase 5E Browser/Mobile QA Notes

## Browser/WebGL QA

Local WebGL preview was served from `docs/` at `http://127.0.0.1:8787/`.

Verified:

- WebGL page loaded locally.
- Unity canvas was created.
- Loading bar hid after startup.
- Start screen was visible.
- How to Play opened and displayed AP/MP, movement, targeting, enemy preview, reward, and boss goals.
- Start Run opened the tactical board.
- No browser console errors were observed during startup or basic interaction.

Observed warning:

- Unity logs a deprecation warning about manual persistent data path synchronization in WebGL. This is a Unity runtime warning, not a gameplay crash.

## Mobile viewport QA

Tested with browser viewport overrides:

- iPhone portrait: `390x844`
- iPhone landscape: `844x390`

Findings:

- Initial WebGL template used a fixed 960x600 desktop canvas and cropped on narrow viewports.
- The generated WebGL output now patches desktop WebGL sizing to full viewport, hides the Unity footer, and prevents horizontal overflow.
- Portrait start screen and in-game HUD are readable after the patch.
- Short landscape start screen needed a compact layout because Start Run was below the viewport; it now uses a compact CLASS / START / HELP row.

## Usability fixes made

- Shortened all skill hint text to avoid wrapping/clipping in the WebGL side panel.
- Simplified landscape skill button copy.
- Made WebGL canvas responsive for all browser viewport sizes, not only mobile user agents.
- Added compact short-landscape start/help layout.

## What was not fully verified

- Full win/loss/restart flow was covered by automated tests, not manually played to completion in browser.
- Physical iPhone Safari testing was not performed.
- Public GitHub Pages URL was not verified live in this environment.
