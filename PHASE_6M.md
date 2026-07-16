# Phase 6M — Tactical correctness and mobile threat clarity

Phase 6M separates two unrelated enemy issues without adding content.

## Ashling

The reported HP loss was the Ashling's existing delayed HP-pressure path. At Manhattan range two, `Flame Sigil` marks the player's tile and its adjacent floor tiles in purple, then resolves once during the next enemy phase after End Turn. It is not an immediate ranged attack, a hazard, Burning, or damage from hypothetical movement. The old generic `rush strike` label and generic AP/MP warning made this valid delayed damage look unexplained.

Immediate `Claw Strike` remains melee-only. Its red preview is exactly the adjacent floor tiles from the Ashling's actual position. Immediate and delayed committed tile sets are now captured and validated separately. Damage records its named source and amount, delayed HUD text states its timing and avoidance rule, and player movement rebuilds the intent before End Turn.

## Stone Sentinel

The Sentinel stalled because a distance-two delayed MP preview was resolved before movement, while the broad control preview also served as a hold reason. Open floor therefore looked like a tactical hold even though no chokepoint or ally-protection condition existed.

`MP Bind` is now selected only as an explicit distance-two chokepoint control action. In ordinary open space the preview is absent and the Sentinel advances along a legal distance-reducing route. Adjacent `Shield Bash` remains melee-only and resolves from the actual final position. The deterministic fallback refuses the current tile when a safe reachable tile improves path distance.

## Mobile tactical HUD

Phone landscape keeps its accepted bottom command bar and board share. HP/AP/MP occupy the left side of the top command row; a structured threat strip occupies the right. It shows only relevant categories: incoming damage, delayed threats, active Burning, incoming control, and enemy movement. On phones the strip uses one priority-ordered, non-wrapping tactical line and a `+N` count for additional simultaneous alerts; empty categories reserve no space and detailed board information remains available from tile focus. The existing readable message font and line height are unchanged. All three skills remain simultaneously visible at 60 pixels or taller, and End Turn remains 56 pixels or taller. Portrait continues to show the rotation screen.

## Validation

Deterministic coverage verifies Ashling preview/resolution parity and named damage, stale preview removal, Sentinel open-space advancement and chokepoint control, final-position melee, structured threat text, and representative iPhone/Android landscape layouts. Phase 6L's platform-neutral audio implementation is unchanged. Native builds remain excluded because the project owner is testing WebGL only.
