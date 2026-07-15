# Phase 6K.1 — Tactical Clarity

This presentation-only pass preserves the existing combat pipeline, classes, enemies, skills, hazards, progression, layouts, and mobile HUD architecture.

## Readability changes

- Biome-authored hazard surfaces now occupy 94% of their tile and use darker, slightly desaturated biome grading. The separate floating hazard icon and bright perimeter were removed.
- Authored floor textures receive a consistent 12% value reduction; Siltglass remains more strongly normalized to match the other biomes.
- Adjacent current and future enemy warnings share one outer boundary instead of stacking icons and per-tile frames. Future warnings are deliberately subdued, especially while aiming a skill.
- Selecting a skill dims unrelated immediate warnings and hazards while preserving gold legal targets, muted maximum range, blocked tiles, out-of-range floor, and final impact outlines.
- The right HUD wraps selected-skill guidance and expresses the colour sequence as `Current`, `Next`, and `Then`.
- Boss rooms use three hazards, one edge/dead-end prop, fewer decorative accent tiles, and a clear radius around the boss spawn. Connectivity and reachability remain guaranteed.

## Performance

No new textures, shaders, particles, post-processing, lights, animations, or gameplay systems were added. The pass removes repeated hazard and warning icon draws and continues to reuse the existing atlases and IMGUI primitives.

## Validation

Closeout requires the complete automated suite, WebGL build and generated-doc checks, plus desktop, phone-landscape, and portrait-rotate browser checks. Windows builds are intentionally excluded at the user's request.
