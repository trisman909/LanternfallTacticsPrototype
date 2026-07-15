# Phase 6J — Group AI Coordination and Hazard Polish

Phase 6J adds no enemies, classes, skills, rooms, relics, maps, or progression. It coordinates the existing enemy roles and contains the existing hazard art.

## Hazard containment

Every hazard icon uses the existing authored atlas at an 82% square footprint with 9% padding on all four sides. Drawing is explicitly clipped to the owning tile, and the tile border is redrawn above the icon. This preserves large silhouettes without cropping, border overlap, or bleed into neighbouring cells.

## Squad planning

Enemy movement is planned once at the start of the enemy turn. The deterministic role order is frontline Stone Sentinel, fast Ashling, ranged Gloom Archer, then Lantern Warden. Every living enemy reserves one destination and its combined immediate/delayed attack footprint before movement resolves.

- Reserved destinations cannot be selected by later squad members.
- Existing occupied tiles remain blocked, so unplanned swaps and pass-through collisions are not possible.
- Later enemies reward threat coverage over player escape tiles that is not already reserved.
- Duplicate attack coverage is allowed when tactically useful but receives less value than new zoning.
- Distinct approach sectors receive a flank bonus; repeating an occupied flank receives a penalty.
- At most three adjacent escape tiles may be reserved, leaving at least one neighbouring route open on a fully open board.
- Sentinels favour front-line escape denial and protection of ranged allies.
- Ashlings retain fast medium-range flanking behavior.
- Gloom Archers preserve distance and must use a non-adjacent position to reserved frontliners whenever one is reachable.
- The existing boss behavior and phase rules remain intact.

## Pathing and fairness

The squad planner retains Phase 6I.2 weighted hazard costs and movement memory. Holding is preferred over a collision or traffic jam. Tests cover unique reservations, reserved threat coverage, multi-flank spread, archer/frontline separation, adjacent escape limits, seeded collision-free turns, and hazard icon containment.

## Performance

Planning runs once per enemy turn on the existing 9×11 board and reuses the existing movement and threat calculations. No assets, shaders, particles, dynamic lighting, or post-processing were added.
