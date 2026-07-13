# Phase 5M - First External Playtest Response Notes

Source: first real external browser playtest feedback.

## Reported issues

- Tester beat the game easily and barely took damage.
- Boss was too easy.
- Enemy strength did not ramp enough across rooms.
- Always-previewed enemy attacks were too easy to avoid.
- Game became fun once explained, but first-minute onboarding was not clear enough.
- Reward cards were confusing and could wrap/crop badly.

## Response summary

- Onboarding copy now explicitly explains AP, MP, movement, skill targeting, danger previews, rewards, and restart.
- Start screen now asks testers to note confusion, fun moments, and breakage.
- Reward cards now use short name/effect/detail lines instead of cramped all-caps labels.
- Room 4 now has 4 enemies instead of 3.
- Between-room passive recovery is reduced from 3 HP to 2 HP.
- Base enemy health was raised slightly.
- Room-depth scaling increases late-room health/damage/mobility.
- Existing boss health, damage, movement, and low-health preview pressure were increased.
- Enemy repositioning now scores threatening positions and escape-tile coverage instead of only walking straight toward the player.

## Seeded/manual check notes

- Seed family `7201-7205`: previews remain on playable tiles for all rooms.
- Seed family `9001-9005`: all biomes and boss room remain connected/reachable.
- Seed family `9101-9105`: board fitting still keeps generated maps visible after Phase 5K.2 scaling.
- Deterministic balance checks now confirm room-depth scaling and stronger boss pressure.

## Expected feel after this pass

- Room 1 should still teach with low danger.
- Room 2 should introduce mild pressure.
- Room 3 should encourage skill use.
- Room 4 should punish careless kiting more often.
- Room 5 boss should require planning around stronger preview zones, not only basic attacks.

## Still needs real tester confirmation

- Whether the new reward cards are understandable without explanation.
- Whether the stronger boss is fair rather than frustrating.
- Whether smarter enemy repositioning creates good tactical pressure.
- Whether the first-minute help copy is enough for a brand-new player.
