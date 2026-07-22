# Phase 6N.4 - Final mobile HUD polish

Phase 6N.4 is a layout-only pass. Gameplay, AI, balance, content, audio, and board visuals are unchanged.

## Combat layout

- Phone top bars use 34-39 pixels and bottom skill bars use 48-52 pixels.
- The threat rail is clamped to 152-174 pixels and returns its recovered width to the board.
- Occupied-board fitting uses a 0.04-tile effect margin and centres the playable floor within the larger viewport.
- Skill cards retain identical selected and unselected geometry with separate name, AP-cost, and state regions.
- Cancel and End Turn use the same premium framed-control treatment as skill cards, a shared baseline, two-pixel borders, contained labels, and at least 44-point hit targets.

## Reward and outcome layout

- Phone reward, victory, and defeat states use one safe-area-aware modal grid instead of an empty overlay shell.
- `ROOM CLEAR`, `CHOOSE ONE REWARD`, and `SAFE` each receive dedicated rectangles.
- Reward name, effect, and description use separate contained regions inside three equal cards.
- Help, Info, and the status/restart control share one aligned 44-point utility row.

## Validation contract

- Target phone sizes: 844x390 and 932x430.
- Desktop behavior remains on the existing desktop HUD path.
- Deterministic tests cover occupied-board growth, modal containment, touch targets, premium action grouping, and skill selection stability.
