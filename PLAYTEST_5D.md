# Phase 5D Playtest Balance and Fun Pass

## Pre-change audit

The audit used the Phase 5C/5B tactical foundation as the baseline: five-room run, five existing classes, five existing biomes, three enemy types, one boss, AP/MP turns, rewards, and the existing UI.

### Seeded audit runs

These are deterministic review seeds used for repeatable room generation and class checks.

| Seed | Class | Notes before tuning |
| --- | --- | --- |
| 1101 | Vanguard / Sun Spear | Durable and understandable, but Sun Charge felt low-impact for a 4 AP tactical move. Guard was useful but easy to spam defensively. |
| 2202 | Cantor / Cinder Staff | Ember Bolt was clear and reliable. Cinder Bloom/Delayed Blast had good identity, but delayed blast cooldown made the class feel slightly stall-heavy in short rooms. |
| 3303 | Wayfinder / Prism Bow | Strong class identity, but strict line targeting made some generated rooms feel like the class had no opening play. Mark was readable but needed clearer payoff. |
| 4404 | Gloamstep / Echo Blades | Mobility identity was good, but low health plus close-range pressure made mistakes feel harsher than other classes. |
| 5505 | Artificer / Lenscaster | Control identity was readable. Lens Trap felt useful but weak as a damage/control button against the boss. |

### Findings before changing values

- AP/MP decisions are meaningful: moving no longer automatically ends the turn, and skill combinations matter.
- The run is understandable after the help panel, but some skill invalid states are still explained only after tapping.
- Enemy previews are readable and fair; the player generally has time to react.
- The boss is reachable and beatable, but can feel slightly flat because its pressure is similar to a normal enemy with more health.
- Rewards are easy to understand; +1 MP is currently the most universally exciting reward.
- Hazards are readable, but Drowned/Gloam movement penalties can be harsh for melee classes when they start on hazard tiles.
- WebGL and mobile layouts are preserved, but no physical mobile browser playtest has been completed.

## Phase 5D tuning decisions

- Keep the content count unchanged.
- Make every class have a clearer basic / tactical / utility role.
- Slightly reduce frustration from movement-locking hazards.
- Make the boss a little more threatening without making the run unfair.
- Keep rewards simple and readable.

## Changes made

### Class and skill balance

- Vanguard: Sun Charge damage increased from 2 to 3 so the 4 AP mobility/push option feels worth using.
- Wayfinder: Straight Shot range increased from 5 to 6; Marked Target now deals 1 chip damage and no longer requires line of sight; Piercing Prism damage increased from 4 to 5.
- Cantor: Delayed Blast cooldown reduced from 3 to 2 to better fit short rooms.
- Gloamstep: health increased from 10 to 11; Backstab AP cost reduced from 4 to 3; Shadow Swap cooldown reduced from 2 to 1.
- Artificer: Lens Trap damage increased from 1 to 2 and root duration increased to 2 turns for the Artificer kit.

### Enemy and boss balance

- Stone Sentinel health reduced from 6 to 5 to reduce room pacing drag.
- Lantern Warden health increased from 14 to 15 so the boss has slightly more final-room presence.

### Reward and hazard balance

- Swift Flame reward text now says +1 MP to match the AP/MP economy.
- Siltglass Prism now empowers all damage skills with +1 range and +1 damage instead of only Ember Bolt.
- Gloam roots now reduce movement to 2 tiles instead of 1, keeping the hazard meaningful without hard-locking mobile movement.
- Ember vents and Stormvault electricity remain delayed, previewed, and 2 damage.

## Post-change seeded notes

| Seed | Class | Result after tuning |
| --- | --- | --- |
| 1101 | Vanguard / Sun Spear | Charge now creates a clearer engage/push decision; guard remains a safe utility option. |
| 2202 | Cantor / Cinder Staff | Bolt remains reliable; shorter Delayed Blast cooldown makes area play feel less once-per-room. |
| 3303 | Wayfinder / Prism Bow | Mark gives a useful fallback when line shots are blocked; Piercing Prism better rewards setup. |
| 4404 | Gloamstep / Echo Blades | Slight health bump and cheaper Backstab make the close-range style less brittle. |
| 5505 | Artificer / Lenscaster | Lens Trap now feels like a real control button, especially against larger targets. |

## Remaining playtest questions

- Physical phone/browser testing is still needed for WebGL and iPhone readability.
- Class balance needs human playtesting; automated checks only prove viability and rules integrity.
- Enemy AI is still intentionally simple.
- Line-of-sight invalid feedback is textual/color-based, not a drawn ray.
