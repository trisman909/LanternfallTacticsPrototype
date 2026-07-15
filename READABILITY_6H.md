# Phase 6H — Visual Readability and Art-Direction Polish

Phase 6H is a presentation polish pass only. It adds no enemies, classes, props, skills, effects, mechanics, shaders, particles, animations, maps, or gameplay systems.

## Readability hierarchy

The intended scan order is player → enemies → danger/targets → board → background.

- Environment: every authored floor tile is blended toward a slightly desaturated biome colour at 36% opacity, reducing local texture contrast by roughly 36% while keeping each realm's palette. Hazards use an 18% veil so their identity remains clear.
- Patterns: alternate floor art appears on about one quarter of tiles rather than every other tile, avoiding a strong checkerboard rhythm.
- Props: rooms now place at most one decorative prop in rooms 1–3 and two in rooms 4–5. Candidates are limited to edges, entrances, corners, or dead ends; prop art is rendered at 72% tile size and slightly dimmed.
- Units: normal unit frames increase from 86% to 98% of a tile (about 14% larger). Sprites remain inside their tiles. The player receives a white/cyan double frame and stronger idle colour; the boss receives a gold double frame and 99% footprint.
- Overlays: movement, skill, area, enemy danger, boss danger, and hit fills use 64–80% opacity with thicker borders, remaining substantially stronger than the environment veil.
- HUD: existing premium gothic atlas frames are reused with restrained neutral tinting for utility, chip, tooltip, selected-skill, and card surfaces. The End Turn control and tactical state colours retain their stronger emphasis.

## Performance and scope

No textures or runtime systems were added. Phase 6H reuses the existing icon, UI, unit, and biome atlases. The changes are fixed colour blends, sizes, placement rules, and outline weights in the existing IMGUI path. There are no shaders, post-processing, dynamic lights, new effects, or gameplay changes.

## Validation contract

Automated tests require props to remain sparse and structurally anchored across generated rooms, environment saturation to decrease, tactical overlays to remain more opaque than floor muting, and unit frames to stay within a 98–99% tile footprint. Desktop and phone-landscape visual checks verify immediate player/enemy/danger recognition.
