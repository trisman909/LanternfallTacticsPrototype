# Phase 6N.2 — phone HUD fit, board scale, and spacing polish

Phase 6N.2 keeps the established L-shaped phone-landscape HUD and changes presentation geometry only. The top and skill bars use less vertical space, the threat rail uses a responsive 200–228 pixel clamp, and phone boards explicitly use the headerless occupied-grid fit. This removes the unintended duplicate turn/room title inside the board and materially increases tile scale while keeping the complete grid visible.

Each skill retains its full touch rectangle while its two-line label is confined to a matching inset content rectangle. End Turn similarly retains the full action hit target while its authored frame is aspect-fitted and its label is inset independently. HP/AP/MP chips are generated as one aligned row with consistent height and spacing.

No combat, AI, skills, threat calculations, balance, controls, art, audio, or desktop layout behavior changed.
