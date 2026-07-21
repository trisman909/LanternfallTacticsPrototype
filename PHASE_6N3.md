# Phase 6N.3 - Real-device phone HUD correction

Status: local review candidate. Do not publish until the required screenshot matrix has been reviewed against the supplied iPhone 16 Pro photographs.

## Real-device root causes

- Phone skill cards used one word-wrapped string containing the selected prefix, abbreviated name, AP cost, and state. Adding `SEL` changed line wrapping and produced a third line outside the frame.
- The intentional targeting-cancel X consumed a proportional quarter of the action rail. End Turn then received only the remainder, was aspect-fitted a second time, and used a wrapping skill-label style inside a narrow safe region.
- Decorated controls generally exposed only their outer rectangles; artwork ornament zones were treated as text space.
- Phase 6N.2 reserved 50-58 pixels at the top, 63-70 at the bottom, and 200-228 at the right even when threat content was short.
- The WebGL shell rendered short phone screens at an artificial internal width of 1199 and downscaled the result. That made logical viewport tests underrepresent real text and ornament pressure.

## Correction

- Each skill has invariant outer and inner geometry plus dedicated name, AP, and state rectangles. Selection changes only colour and state text.
- Short, medium, and long skill names use three deterministic font tiers; wrapping is disabled.
- Cancel has a fixed 42-48 pixel slot. End Turn uses the remaining selected-state slot or the full action slot when targeting is inactive. Both variants have explicit aspect-fitted artwork and ornament-safe label rectangles.
- Stats, title, threat content, Help, and Info expose asset-specific safe rectangles.
- Phone bars are 42-46 pixels at the top and 52-56 at the bottom. The threat rail is clamped to 174-194 pixels.
- Phone boards fit actual floor extents with an 0.11-tile effect margin and a small horizontal balance bias.
- WebGL uses the real logical viewport at device-pixel-ratio 1 for deterministic phone geometry.

Gameplay, balance, AI, audio, content, and desktop layout are unchanged.
