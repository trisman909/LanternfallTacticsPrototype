# Phase 6E — Player, Enemy, and Boss Sprite Pass

## Authored atlas

`Assets/Resources/Units/phase6e_unit_atlas.png` is a 768×768 RGBA, 3×3 atlas containing nine original hand-painted production-placeholder sprites:

1. Vanguard / Sun Spear
2. Wayfinder / Prism Bow
3. Cantor / Cinder Staff
4. Gloamstep / Echo Blades
5. Artificer / Lenscaster
6. Ashling
7. Gloom Archer
8. Stone Sentinel
9. Lantern Warden

The official concept sheet was used only as an art-direction reference. The sprites are new original designs with distinct silhouettes, recognizable weapons, class colour accents, and a consistent top-down combat orientation. No third-party game assets were copied.

## Runtime states

- Player selection retains the existing high-contrast white outline.
- Hit units receive a warm damage tint and thicker gold hit outline.
- Shield, burn, root, and mark remain authored compact status markers.
- Enemy health and shield values remain visible.
- Lantern Warden is larger than normal enemies and retains its gold boss frame.
- Phase 2 and Phase 3 add a magenta overcharge frame and authored overcharge badge without changing boss mechanics.
- Procedural class/enemy symbols remain only as an atlas-load fallback.

## Performance

The atlas loads once through `Resources` and uses UV selection in the existing IMGUI pass. It adds no animation controller, shader, material, particle system, or per-unit texture. Mipmaps are disabled to prevent atlas bleed; WebGL uses normal texture compression.

## Generation record and remaining placeholders

Built-in image generation created an exact isolated 3×3 top-down unit sheet on a flat magenta chroma background, using the concept sheet as a style reference. The installed chroma-removal helper produced alpha, and the result was Lanczos-downscaled and optimized to 768×768.

This is a coherent high-quality production-placeholder set, not claimed final character art. Bespoke animation frames, directional variants, death frames, biome environment art, combat effects, and audio remain for later milestones.
