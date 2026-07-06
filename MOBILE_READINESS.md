# Phase 4 — Mobile Readiness Notes

## Portrait

The 360×800 reference layout reserves a non-overlapping lower control panel and keeps the complete nine-by-eleven board above it. Skill controls are arranged in one row with 64-pixel height, followed by separate Cancel and Wait targets. Room, health, biome, message, hazard rule, cooldowns, rewards, and outcome controls remain in the safe area.

## Landscape

The 800×360 reference layout uses a compact right panel instead of the desktop-height panel. Skill controls remain at least 50 pixels high, rewards use three large columns, and the board keeps an estimated minimum tile size above 24 pixels. Taller landscape windows retain the more descriptive panel.

## Touch flow

Unity IMGUI buttons and board cells accept primary touch as pointer input while continuing to support an editor mouse. The tested flow covers tile movement, invalid-tile rejection, skill selection, cancellation, reward selection, room advancement, and restart. Selected skills receive an arrow marker; invalid taps retain position and display a leading cross message.

## Tactical readability

Move tiles remain cyan, skill targets gold, enemy attacks red, and delayed hazards use their biome warning color plus a persistent glyph. This lets warnings remain identifiable when overlays share a tile. Turn state remains above the board in both orientations.

## Performance

The prototype uses code-drawn solid rectangles and text with no runtime particle systems, post-processing, animated materials, or imported 3D biome assets. Runtime defaults cap rendering at 30 FPS, disable VSync, and disable multitouch because gameplay accepts one deliberate tap at a time. Android settings use Gamma color space, ARM64, and Android API 26 minimum.

## Android export status

Unity Android Build Support is not installed for Unity 6000.5.1f1 on this machine. The expected `Editor/Data/PlaybackEngines/AndroidPlayer` directory is absent, so no APK build was attempted or claimed. `BuildPrototype.BuildAndroid` and the Android player settings are prepared for use after installing Android Build Support, the SDK, NDK, and OpenJDK through Unity Hub.

## Remaining device work

Physical-device validation is still required for safe-area cutouts, manufacturer font rendering, thermal behavior, and touch feel. The Windows development player remains the verified fallback.
