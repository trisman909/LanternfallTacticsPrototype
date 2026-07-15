# Phase 6L — Mobile Polish and Audio Foundation

Phase 6L is a focused polish and bug-fix pass. It adds no classes, enemies, bosses, skills, hazards, rooms, progression, monetization, or large gameplay systems.

## Tactical readability

- Purple future-threat boundaries are modestly thicker and brighter on phone landscape while remaining quieter than current red attacks.
- Merged threat edges are calculated from affected walkable tiles and stop at walls, gaps, and disconnected shapes.
- Biome props use darker biome grading, larger irregular art coverage, and a restrained contact shadow without tactical borders.
- Phone status badges are approximately twice the previous perceived size. Shield, burn, root, and mark use dark backings, coloured outlines, duration/stack numbers, and the existing status pulse.
- Invalid skill attempts retain the selected skill and show one wrapped reason: out of range, no valid target, blocked line of sight, occupied, insufficient AP, or invalid destination.
- The accepted responsive HUD architecture remains unchanged. Audio controls use a separate phone-safe row in the existing Help overlay.

## Stone Sentinel

The Sentinel no longer treats every delayed preview as permission to remain idle. It holds only when already attacking, controlling close space, occupying a chokepoint, or protecting a nearby Gloom Archer. Otherwise it receives a strong deterministic incentive to advance and cannot repeatedly idle at long range.

Base HP increases from 6 to 7. This is the smallest integer adjustment available and compensates for the Sentinel's one-tile movement and required frontline exposure. Damage, range, status pressure, and hazard path costs are unchanged.

## Original audio foundation

Twenty-one short procedural cues cover UI, targeting, movement, attacks, statuses, healing, phase/room flow, rewards, End Turn, victory, and defeat. One exploration loop and one boss loop are synthesized at runtime from original tone sequences. No imported or copyrighted audio files are used.

Saved controls cover master, SFX, music, and mute. WebGL playback remains locked until the player's first interaction. The implementation uses Unity's built-in audio module, two AudioSources, mono 22.05 kHz clips, and no streaming or external requests.

## Validation scope

Targeted and complete EditMode suites cover future-threat visibility, environmental prop hierarchy, Sentinel progress, invalid-target reasons, phone status sizing, wall/gap boundary clipping, boss threat validity, audio persistence, and WebGL interaction gating. Closeout includes WebGL build, generated-doc validation, live asset checks, responsive browser checks, and console inspection. A Windows build is intentionally excluded because the project owner tests only WebGL.
