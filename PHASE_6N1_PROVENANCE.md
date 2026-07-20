# Phase 6N.1 deployment provenance correction

The original Phase 6N.1 source, local WebGL output, committed `docs`, and live GitHub Pages files were proven byte-identical. GitHub Pages deployed commit `c0755988073beef30ac303c2ef7b49b5a0d96e6a` from `master` through the repository's `pages-build-deployment` workflow.

The failure was layout reachability, not stale deployment content. Unity WebGL was allowed to scale its canvas backing buffer by `devicePixelRatio`, while `MobileLayout.Compute` classified a phone only when its internal width was at most 1200 pixels. High-density phones could therefore present an 844–932 CSS-pixel viewport to the page but a Unity viewport wider than 1200, selecting the preserved desktop/tablet sidebar.

The correction pins the WebGL player to CSS-pixel resolution with `config.devicePixelRatio = 1`. A temporary `Phase 6N.1 — L HUD` proof label appears on the menu and gameplay screen. Playtest Info reports the build source version, selected layout mode, and Unity viewport dimensions for deployment verification.

No combat, AI, audio, balance, board generation, or HUD geometry was changed.
