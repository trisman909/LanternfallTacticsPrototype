# Phase 6I — Presentation Integration and Cleanup

Phase 6I closes the requested presentation sequence without adding content or changing gameplay, balance, AI, boss phases, maps, classes, biomes, or the accepted Phase 6B.1 mobile layout.

## Integration audit

- Visual hierarchy: the Phase 6H order remains player > enemies > danger/targets > board > background.
- Icon consistency: HP, AP, MP, shield, statuses, threats, line-of-sight, pickup, obstacle, and five hazards all use the Phase 6D atlas.
- Unit consistency: all five classes, three normal enemies, and Lantern Warden use the Phase 6E atlas; selection, hit, status, boss overcharge, HP, and shield states remain above the sprites.
- Environment consistency: all five biomes use Phase 6F atlases under the quiet-board treatment; blockers, hazards, pickups, props, and boss accent remain distinct.
- Effects consistency: Phase 6G movement, action, status, death, boss, room, victory, and defeat cues retain the reduced-motion contract.
- HUD consistency: authored frames remain on stats, utilities, skill cards, selected skill, End Turn, rewards, tooltips, victory, and defeat with restrained Phase 6H tinting.
- Mobile/desktop: phone landscape retains its centered End Turn command bar; portrait retains the rotate screen; desktop retains the board/right-HUD split.

## Cleanup completed

- Removed obsolete class-letter, enemy-letter, biome-prop-letter, procedural floor-glyph, status-glyph, and mojibake fallback paths.
- Reliability fallbacks use the authored icon language and flat biome colour rather than visible debug symbols.
- Replaced outdated WebGL documentation that described the presentation as code-only placeholder visuals.
- Updated all visible/current build markers to Prototype v0.6I.
- Added a dark-fantasy themed WebGL loading card, page description/theme metadata, and a landscape fullscreen web-app manifest.
- Kept only concise tactical copy; first-time instructions now identify the controlled hero, movement, skill selection, HP/AP/MP intent, danger timing, End Turn, rewards, and boss Phase 2.

## Remaining placeholders

- The original authored atlases are coherent production-placeholder art, not final production character/environment/UI art.
- Combat VFX are lightweight production-placeholder motion graphics.
- There is no audio layer because the revised Phase 6H brief replaced the earlier audio milestone with visual-readability work.
- A physical iPhone Safari screenshot pass remains external-device validation; automated and browser-emulated phone-landscape/portrait checks remain in place.

## Performance

Phase 6I adds no runtime textures, shaders, particles, post-processing, dynamic lighting, gameplay systems, or audio. The manifest and loading CSS are static text. Runtime cleanup removes obsolete glyph paths and otherwise reuses existing atlases and presentation code.
