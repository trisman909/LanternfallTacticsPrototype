# Phase 3.5 — Lanternfall Biome Identity

The original Lanternfall project was inspected read-only. The tactics prototype reuses its five canonical biome names, stable IDs, fog/ambient colors, and the key colors from its Surface, Accent, Hazard, Emissive, and particle materials.

The original URP materials, 3D mesh assets, particle systems, volume profiles, prefabs, enemies, guardians, scenes, and gameplay assemblies were deliberately not copied. They are heavier than this mobile board needs and several are coupled to the action game. Instead, each theme is represented by a compact code-driven palette, alternating top-down tiles, five high-contrast hazard tiles, and three large dressing symbols. This adds no texture memory and keeps props from obscuring selectable cells.

## Manual theme notes

- **The Drowned Narthex** — blue-green wet stone, oxidized cyan water and cathedral-tower symbols. Shallow water reduces the next movement allowance by one tile.
- **Siltglass Observatory** — dark sandstone, brass-gold alternate slabs and violet prism glass. Standing on a prism gives Ember Bolt +2 range and +1 damage.
- **The Ember Ossuary** — charred brown stone, bone-orange accents, furnace vents and large ossuary marks. Vents become bright warnings, then deal 2 damage to a unit occupying them on the next enemy phase.
- **The Gloam Orchard** — moonlit green floors, violet roots and large fruit/tree silhouettes. Root tiles bind the player's next movement allowance to one tile.
- **Stormvault Foundry** — steel plates, copper alternates and electric-blue charged panels with gear dressing. Charged panels warn, then deal 2 damage to all adjacent units, including enemies.

The five-room run rotates through the themes in this order, leaving Stormvault Foundry as the boss room. Hazard and prop placement never removes floor connectivity or occupies initial unit spawns.
