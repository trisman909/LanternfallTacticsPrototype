# Phase 6F — Biome Environment Pass

Phase 6F adds five original, lightweight 576×576 RGBA production-placeholder atlases under `Assets/Resources/Biomes`. Each fixed 3×3 atlas contains a base floor, alternate floor, obstacle, biome hazard, healing pickup treatment, three props, and a boss-room floor accent.

The board renderer selects the atlas from the existing biome ID. Authored floors sit beneath the established movement, skill-target, area, hit, enemy-preview, danger, selection, and hazard-readability layers. Missing assets retain the prior procedural fallback. No generation, connectivity, hazard behavior, combat, balance, AI, or layout logic changes.

The source PNGs total roughly 2.6 MB before Unity import compression. Atlases have no mipmaps, use bilinear sampling and clamp wrapping, and require no materials, shaders, animation, or per-frame texture allocation. They are intentionally production-placeholder environment art, not final production art.

Atlas order for every biome is: base floor, alternate floor, obstacle / hazard, healing pickup, prop A / prop B, prop C, boss accent.

Biome treatments:

- Drowned Narthex: wet cathedral stone, shallow water, oxidized lantern architecture.
- Siltglass Observatory: sandstone and brass, prism glass, astronomical instruments.
- Ember Ossuary: charred stone, ember vent, bone and furnace fragments.
- Gloam Orchard: roots and moonlit soil, grasping roots, fungi and thorn props.
- Stormvault Foundry: riveted steel and copper, charged plate, coils and insulators.
