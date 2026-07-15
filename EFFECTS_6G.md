# Phase 6G — Combat Effects and Animation Pass

Phase 6G is a presentation-only layer driven by existing game-state changes. It does not alter damage, resources, cooldowns, AI, turn order, hazard timing, movement costs, or input availability.

## Visual effect checklist

- Movement: short player/enemy tile interpolation, path sparks, destination pulse, restrained four-speck dust burst.
- Player attacks: compact slash, spear/thrust, projectile, fire/blast, prism, shadow movement, and gadget/root cue families selected from existing action messages.
- Enemy attacks: breathing preparation outlines, source-to-target strike/cast trail, distinct AP and MP drain colours, root cue, stronger heavy/boss pulse.
- Statuses: burn, shield, mark, and root retain authored markers and gain subtle colour-matched auras; healing gets a green pulse; delayed danger keeps its authored icon and breathing outline; boss overcharge retains its frame, badge, phase banner, and transition pulse.
- Deaths: defeated enemy art briefly expands and fades instead of disappearing without feedback.
- Room flow: room arrival, room clear/reward, boss arrival, victory, and defeat banners; Phase 2/3 retain dedicated boss banners.
- Accessibility: How to Play includes a persisted Full/Reduced Motion toggle. Reduced mode removes shake and aura breathing and reduces animation durations to 60–120 ms.

## Performance

Effects use a small bounded in-memory list, cached unit/icon textures, and simple IMGUI rectangles/outlines. There are no new texture assets, particle systems, materials, shaders, physics objects, coroutines, or gameplay delays. Finished effects are removed immediately. The target remains 30 fps for low-end phone WebGL.

## Remaining presentation placeholders

Effects are lightweight production-placeholder motion graphics rather than final VFX animation sheets. Final art may later replace individual cue shapes without changing `CombatEffectLanguage`, gameplay code, or the reduced-motion contract.
