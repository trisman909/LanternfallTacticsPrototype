# Phase 6D — Authored Icon and UI Art Pass

## Authored assets

- `Assets/Resources/UI/phase6d_icon_atlas.png` — 640×640 RGBA, 5×5 cells. Covers HP, AP, MP, shield, burn, roots, mark, healing, AP/MP drain, delayed cast, shielded, boss overcharge, move/target states, invalid/blocked states, immediate/delayed/boss danger, healing pickup, obstacle, and all five biome hazards.
- `Assets/Resources/UI/phase6d_ui_atlas.png` — 768×768 RGBA, 3×3 cells. Covers skill cards, stat chips, End Turn, utility buttons, selected-skill information, reward cards, victory, defeat, and tooltip/message framing.

The official Lanternfall concept sheet was used as a style reference only. Both atlases are newly generated original production-placeholder artwork. They use dark slate interiors, antique brass framing, lantern accents, strong silhouettes, and the established cyan/violet/ember/healing palette. No external or copyrighted game assets were used.

## Integration and fallback

The atlases load once through Unity `Resources` and render as atlas UV regions in the existing IMGUI pass. No new shaders, materials, particle systems, canvases, or layout containers were added. Phase 6C procedural marks remain only as a fallback if an atlas fails to load.

## Visual review notes

### Desktop

- Resource chips now read as a coherent framed set while retaining exact HP/AP/MP text.
- Skill and reward cards use warmer brass hierarchy than tooltip/message panels.
- End Turn uses a distinct amber command frame; Help/Info and Cancel remain visually quieter.
- Danger and target icons retain their existing red, purple, gold, and cyan overlay hierarchy.

### Phone landscape

- The accepted Phase 6B.1 three-row command HUD geometry is unchanged.
- Atlas silhouettes remain legible at the existing 44–68 pixel row heights.
- End Turn remains centered in the existing command row.
- Secondary frames use dark interiors so large mobile text retains contrast.

## Generation record

Built-in image generation was used with the concept sheet as a style reference. The icon prompt requested an exact isolated 5×5 grid of small dark-fantasy symbols on a flat magenta chroma background. The UI prompt requested an exact isolated 3×3 grid of empty dark-slate/brass frames on the same chroma background. Chroma removal used the installed image-generation helper, then the outputs were downscaled with Lanczos resampling and saved as optimized RGBA PNGs.

## Remaining placeholders

Unit tokens, biome floor/prop art, combat effects, animation, and audio remain production placeholders for Phases 6E–6H. Phase 6D artwork is intentionally described as a coherent production-placeholder set, not final production art.
