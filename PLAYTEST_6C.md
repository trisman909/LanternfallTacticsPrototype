# Phase 6C — Visual Language and Icon Foundation

Phase 6C keeps the accepted Phase 6B.1 phone-landscape HUD and makes one contained correction: the End Turn button is centered in its existing command-bar row.

## Icon vocabulary

The prototype now draws 20 original lightweight icons in code: HP, AP, MP, shield, burn, root, mark, heal, AP drain, MP drain, immediate danger, delayed danger, boss danger, blocked line of sight, healing pickup, shallow water, prism glass, ember vent, grasping roots, and charged floor.

No textures, atlases, shaders, materials, or runtime effects were added. Each icon is assembled from a few solid rectangles and outlines in the existing IMGUI pass.

## Board and tokens

- AP/MP enemy badges are now compact drain icons; exact meanings remain in selected-tile threat details.
- Immediate, delayed, boss, blocker, healing, and five hazard markers use the new icons.
- The healing pickup no longer repeats `HEAL +3` across the board.
- Player tokens use class-specific symbols, normal enemies use role-specific symbols, and the Lantern Warden keeps its silhouette/color while gaining a double boss frame.
- Statuses use compact shield, burn, root, and mark icons.

## Preserved scope

Gameplay, balance, five classes, five biomes, enemy AI, boss phases, portrait rotation screen, desktop layout, mobile landscape layout, and the WebGL deployment path are unchanged.
