# Phase 6I.2 — Gameplay Polish, AI Fixes, and Readability

This milestone adds no enemies, classes, relics, skills, rooms, or progression. It refines the existing tactical rules and presentation.

## Range readability

Selecting a skill now keeps its full tactical reach visible even when no legal target exists. Gold tiles are legal targets, muted gold tiles are within maximum range but blocked or invalid for that skill, outlined blockers interrupt the cast, and darkened floor is out of range. The HUD explicitly explains a zero-target selection instead of hiding the preview.

## Enemy commitment and engagement

- Enemies remember their previous tile and strongly reject reversing onto it unless that reversal creates an immediate attack.
- A committed destination receives a stable preference, removing equal-score left/right indecision.
- Weighted route evaluation uses safe 1, water 2, roots 2, charged floor 3, ember vent 4, and prism 1. Hazards remain traversable when they are the only useful route.
- After repositioning, intent is recalculated once. If the player is then in the enemy's immediate attack pattern, the enemy follows through during that turn.
- HP attack intents now deal their documented damage when resolved; they no longer consume an enemy turn with an empty pressure action.
- Only the existing Gloom Archer behavior is allowed to deliberately retreat from melee.

## Hazard audit

- Shallow Water: beginning a turn on water reduces effective movement by one.
- Grasping Roots: beginning a turn on roots caps movement at two. This is a movement allowance, not a separate MP-drain status.
- Prism Glass: damage skills cast while standing on prism gain one range and one damage.
- Ember Vent: vents visibly arm, then damage an occupying player on resolution.
- Charged Floor: plates visibly arm, then damage adjacent occupants on resolution.

The displayed MP is now capped at player-turn reset to match water/root targeting, removing the previous discrepancy between the HUD and reachable tiles. Entering, leaving, or remaining on a passive movement hazard is evaluated deterministically from the tile occupied when the next player turn begins.

## Boss and rooms

Lantern Warden Phase 1 remains unchanged. Later immediate threat expands from radius three to radius four, while phase lines extend farther and remain bounded by connected floor. Generated rooms retain the existing corridor-and-pocket style. Generation is still seeded and procedural; connectivity, enemy reachability, floor-backed hazards, and lack of isolated islands are tested across all five rooms and many seeds.

## Performance

Hazard icons use the existing icon atlas at an 80% tile footprint. AI additions are small deterministic searches on the existing 9×11 board. No assets, shaders, particles, post-processing, dynamic lights, or gameplay content were added.
