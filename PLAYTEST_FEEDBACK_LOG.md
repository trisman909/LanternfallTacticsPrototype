# Lanternfall Tactics - Playtest Feedback Log

Status: first external tester feedback has been recorded and addressed through Phase 5O.

Live playtest link:

`https://trisman909.github.io/LanternfallTacticsPrototype/`

Use this log to capture real tester notes. Keep raw wording where possible. Do not invent feedback.

## Entry template

### YYYY-MM-DD - Tester name or initials

- Did the game load:
- Device/browser:
- Class tried:
- Furthest room reached:
- Understood what to do in first minute:
- AP and MP clarity:
- Skill target clarity:
- Enemy danger preview clarity:
- Board large enough:
- HUD readable:
- Anything too small or clipped:
- Difficulty/pacing:
  - Too easy / Fair / Too hard / Too slow / Too fast
- Class that felt best:
- Class that felt worst:
- Most confusing moment:
- Most fun moment:
- Bug or broken behavior:
- Screenshot/video link:
- Severity:
  - Blocker / High / Medium / Low
- Follow-up action:
  - None / Needs reproduction / Fix planned / Fixed in commit:

## Current triage

### 2026-07-13 - First external browser tester

- Did the game load: Yes
- Device/browser: Live WebGL browser build, exact browser not recorded
- Class tried: Not recorded
- Furthest room reached: Won / beat room 5
- Understood what to do in first minute: No, needed in-person explanation
- AP and MP clarity: Needed explanation
- Skill target clarity: Needed explanation
- Enemy danger preview clarity: Understandable after explanation, but too easy to avoid
- Board large enough: No specific complaint after Phase 5K.2
- HUD readable: Rewards and upgrade cards unclear
- Anything too small or clipped: Reward card text wrapped/cropped badly
- Difficulty/pacing:
  - Too easy
- Class that felt best: Not recorded
- Class that felt worst: Not recorded
- Most confusing moment: First-minute rules and reward choices
- Most fun moment: Combat felt fun once understood
- Bug or broken behavior: No crash reported
- Screenshot/video link: Not included in repo
- Severity:
  - High
- Follow-up action:
  - Fixed in commit: Phase 5M response

### 2026-07-13 - Follow-up live browser tester

- Did the game load: Yes
- Device/browser: Live WebGL browser build, exact browser not recorded
- Class tried: Not recorded
- Furthest room reached: Not recorded
- Understood what to do in first minute: Improved after Phase 5M
- AP and MP clarity: Improved after Phase 5M
- Skill target clarity: Not specifically flagged
- Enemy danger preview clarity: Not specifically flagged
- Board large enough: Not flagged after Phase 5K.2
- HUD readable: Reward clear panel still needed spacing cleanup
- Anything too small or clipped: `ROOM CLEAR / CHOOSE ONE` and reward cards could visually overlap/crop
- Difficulty/pacing:
  - Better, but can be slightly harder
- Class that felt best: Not recorded
- Class that felt worst: Not recorded
- Most confusing moment: Health scarcity may make HP reward feel mandatory
- Most fun moment: Combat direction felt good
- Bug or broken behavior: No crash reported
- Screenshot/video link: Not included in repo
- Severity:
  - Medium
- Follow-up action:
  - Fixed in commit: Phase 5N response

### 2026-07-13 - Second follow-up live browser tester

- Did the game load: Yes
- Device/browser: Live WebGL, also phone browser; exact browsers not recorded
- Class tried: Vanguard / spear
- Furthest room reached: Won
- Understood what to do in first minute: Improved
- AP and MP clarity: Clear
- Skill target clarity: Clear
- Enemy danger preview clarity: Clear but too easy to avoid
- Board large enough: Yes
- HUD readable: Yes
- Anything too small or clipped: No
- Difficulty/pacing:
  - Fair and fun, but too easy with guidance
- Class that felt best: Vanguard / spear used
- Class that felt worst: Not recorded
- Most confusing moment: Healing pickups were not noticed
- Most fun moment: Core tactical combat direction felt good
- Bug or broken behavior: None reported
- Screenshot/video link: Not included in repo
- Severity:
  - Medium
- Follow-up action:
  - Fixed in commit: Phase 5O response

## First-feedback rules

- Fix blockers first.
- Fix repeated confusion before adding content.
- Treat one-off subjective balance comments as notes, not marching orders.
- Keep first fixes small: clarity, layout, controls, invalid-action feedback, reward/restart wording.
- Do not add classes, enemies, bosses, biomes, rooms, monetization, online features, or large systems during this pass.
- Preserve WebGL, Windows build support, mobile readability, AP/MP combat, five classes, five biomes, and the five-room run.
