# Phase 5O.1 - Mobile HUD Playability Fix Notes

Date: 2026-07-13

This pass responds to real phone WebGL feedback. Scope stayed limited to mobile/WebGL HUD layout, text sizing, touch targets, and browser viewport handling. No gameplay balance, AI, boss, class, biome, reward, healing, or room content changed.

## Phone portrait changes

- The bottom HUD/action panel is taller so controls are not pushed off-screen.
- The board uses compact header spacing in portrait so the HUD gets enough room.
- HP/AP/MP chips are taller and use larger mobile text.
- All three skill buttons are visible in one row.
- Narrow mobile skill buttons show only the essentials: skill label, AP cost, and ready/cooldown/AP state.
- End Turn remains visible and finger-sized.

## Phone landscape changes

- Phone landscape now gets its own wider right HUD instead of a narrow desktop-style strip.
- Skill cards use a horizontal row and larger touch targets.
- HP/AP/MP chips and End Turn remain visible inside short browser viewports such as 800x360.
- The message strip is shorter in phone landscape to keep important controls above the bottom edge.

## WebGL/mobile viewport changes

- WebGL template now uses `100dvh` where practical to better match dynamic mobile browser viewport height.
- Mobile viewport meta includes `viewport-fit=cover`.
- CSS includes safe-area padding hints for notches/home indicators.
- Loading copy suggests rotating or using browser fullscreen/Add to Home Screen if browser chrome makes the game feel cramped.

## Validation notes

- Automated layout tests cover iPhone portrait, iPhone landscape, short Android-like landscape, skill visibility, End Turn visibility, stat-chip readability, reward card clipping, desktop preservation, and WebGL docs payload strings.
- Physical phone confirmation is still needed after the live GitHub Pages build updates.

## Known issues

- WebGL cannot force true fullscreen from normal browser play.
- Phone browser address bars can still reduce usable height depending on browser and scroll state.
- Desktop remains the best first-test experience, but phone portrait/landscape should now be playable enough for targeted mobile QA.
