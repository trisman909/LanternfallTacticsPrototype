# Phase 5I - Lightweight Game Feel Polish

Phase 5I improves feedback and flow without adding content, systems, rooms, classes, enemies, bosses, or heavy assets.

## What changed

- End Turn now clearly says enemy previews are resolving.
- New player turns call out `PLAYER TURN` before the danger/hazard summary.
- Invalid actions now repeat the core color language: cyan move, gold skill target, red danger.
- Reward choices now carry into the next room message instead of being immediately overwritten.
- Victory and defeat messages now tell players they can start a new run.
- Skill feedback is a little sharper:
  - push skills mention push feedback
  - mark feedback says the next hit gets bonus damage
  - area skills report affected tile count
  - root and swap feedback uses clearer action language
  - defeated enemies are called out as defeated
- No audio, particles, imported art, new content, or complex animation system was added.

## Manual notes

- Movement, skills, enemy previews, rewards, boss reachability, win/loss, and restart flow remain the same.
- The changes are intentionally text-and-highlight driven so the WebGL and low-end mobile footprint stays small.
- The most important improvement is reward feedback persistence: testers now see which blessing they picked as the next room begins.

## Remaining known issues

- Enemy actions are still stepwise rather than animated.
- There is still no authored audio or final VFX pass.
- Full feel tuning still needs human playtest feedback from the public WebGL link.
