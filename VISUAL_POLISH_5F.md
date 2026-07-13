# Phase 5F — Visual Identity Polish

Phase 5F keeps Lanternfall Tactics lightweight and prototype-safe. The visuals are still code-driven placeholder art, but the board now reads more like an intentional dark mystical tactics game instead of a debug grid.

## What changed

- Tile states now use a shared readability palette for floor, hazard, armed hazard, enemy preview, move target, skill target, area preview, hit, and invalid feedback.
- Tiles have dark borders and subtle biome glyphs to reduce the plain-square look without adding noisy texture detail.
- Hazards have stronger outlines and clear symbols.
- Enemy preview tiles now combine red fill, red outline, and a white warning symbol.
- Valid movement and skill targets now use stronger outlines, not just fill color.
- Invalid taps now show a bright red outline and `X`.
- Player/enemy tokens have darker silhouettes, outlines, clearer glyphs, boss emphasis, and compact status markers.
- Skill buttons and reward cards now have simple framed card styling.
- Start, help, HUD, and board panels now use stronger framing and a small gold trim.

## Biome visual notes

### Drowned Narthex

- Palette remains cyan/blue-green, wet stone, moss, and oxidized-brass inspired.
- Floor glyphs use subtle water marks.
- Shallow water hazards use `~`, bright cyan outlines, and high contrast against floor tiles.
- Readability check: valid move cyan remains distinct from shallow-water cyan because valid moves use a stronger fill and outline.

### Siltglass Observatory

- Palette remains sandstone, prism violet, brass, and gold.
- Floor glyphs use prism/diamond marks.
- Prism tiles use `<>` and bright accent outlines.
- Readability check: gold skill targets remain distinct from violet prism hazards.

### Ember Ossuary

- Palette remains ash, charred stone, bone, ember red/orange/black.
- Floor glyphs use small ember/crack marks.
- Ember vents use `!`; armed vents use the brighter warning palette and white outline.
- Readability check: enemy danger red and ember warning orange are close in mood but separated by warning glyphs and outlines.

### Gloam Orchard

- Palette remains root, fungus, moonlit green/violet.
- Floor glyphs use organic marks.
- Root hazards use `#` and luminous green-violet contrast.
- Readability check: green class/status colors remain readable because hazard tiles use strong tile outlines.

### Stormvault Foundry

- Palette remains metal, copper, coils, and blue-white electricity.
- Floor glyphs use plate/coil-like marks.
- Charged floor hazards use `Z`, blue-white warning color, and hard outlines.
- Readability check: electric hazard warnings remain visible against darker foundry plates.

## Manual QA notes

- Portrait layout: framed HUD and board remain readable; skill/reward buttons keep large touch targets.
- Landscape layout: compact mode preserves board visibility and keeps AP/MP/HP, End Turn, and skill buttons visible.
- Browser/WebGL: visual changes are IMGUI rectangles/labels only, with no new shaders, textures, or heavy assets.
- Mobile safety: no expensive effects, no particle systems, no imported art packs, and no large textures.

## Remaining known issues

- Visuals are still prototype IMGUI art, not final production art.
- No physical iPhone playtest has been performed on this machine.
- Effects are readable but still simple; there are no authored animations or audio polish passes yet.
