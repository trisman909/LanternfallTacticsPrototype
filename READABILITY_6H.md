# Phase 6H — Visual Readability and Art-Direction Polish

Phase 6H is a presentation-only pass. The post-6I refinement is released as `Prototype v0.6I.1` so the integrated presentation stack remains intact. It adds no enemies, classes, props, skills, effects, mechanics, shaders, particles, animations, maps, or gameplay systems.

## Readability hierarchy

The player-facing scan remains player > enemies > danger/targets > board > background. The enforced layer order is units > tactical overlays > HUD > props > floor > background.

- Floors: all five existing biome atlases are reused. Their authored texture is blended toward a slightly desaturated dominant biome colour. Per-biome opacity corrects the measured brightness and contrast spread instead of applying one grade indiscriminately; Siltglass receives an additional neutral tint because its source floor was more than twice as bright as most other realms.
- Atmosphere: one subtle, fixed diagonal painted-light grade runs across every biome. It is a pair of inexpensive colour rectangles, not a light, shader, effect, or post-process.
- Patterns: alternate floor art remains sparse, avoiding a strong checkerboard rhythm or repeated high-frequency detail.
- Props: rooms place at most one decorative prop in rooms 1–3 and two in rooms 4–5. Candidates are limited to edges, entrances, corners, or dead ends; art is rendered at 72% tile size and slightly dimmed.
- Units: enemy, player, and boss frames use a deliberate 96% / 98% / 99% footprint. Sprites stay within tile boundaries. The player retains its cyan-white double frame and the Lantern Warden retains its distinct gold boss frame.
- Overlays: whenever movement, selection, targeting, danger, boss danger, or a skill preview occupies a tile, a restrained dark separator makes the floor recede before the 64–80% tactical colour is drawn. Hazards retain more of their identifying art while overlays remain dominant.
- HUD: the existing premium gothic frames and authored icons remain unchanged. No UI architecture or accepted desktop/mobile layout was restructured.

## Quantitative audit

The pre-refinement source-floor audit showed large cross-biome variation: mean brightness ranged from about 36 to 105 and local contrast from about 5 to 13. Siltglass was the clear outlier. The new per-biome grade uses quieting alpha values from 24% to 66%, chosen to normalize that source variation while retaining the dominant hue and realm identity. Tactical enemy preview opacity is 72%, above every environment grade; hit emphasis remains 80%.

## Performance and scope

No textures or runtime systems were added. No assets, atlases, shaders, particles, post-processing, or dynamic lights were added either. The implementation reuses the current biome, icon, UI, and unit atlases and adds only fixed colour blends in the existing IMGUI draw pass. Gameplay, balance, generation rules, AI, boss phases, desktop layout, phone-landscape layout, and portrait rotation screen are unchanged.

## Validation contract

Automated tests cover sparse structurally anchored props across generated rooms, reduced environment saturation, per-biome normalization, tactical-overlay dominance, and bounded unit scale hierarchy. Closeout also requires all edit-mode tests, a clean WebGL build, deployed-doc validation, desktop and phone-landscape visual checks, portrait rotation behavior, loading-shell verification, and a live-page check.
