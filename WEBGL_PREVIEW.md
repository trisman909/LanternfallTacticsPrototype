# Lanternfall Tactics WebGL Preview

## Phase 5C status

Unity WebGL Build Support is installed for Unity 6000.5.1f1 on this machine.

The WebGL preview is intended as a lightweight browser-playable build for sharing the current prototype. It does not add gameplay content and should preserve the Phase 5B AP/MP, class, biome, room, boss, reward, and win/loss systems.

## Build output

Local WebGL build output:

`Builds/WebGL/LanternfallTactics`

GitHub Pages-ready copy:

`docs`

The Pages copy should contain:

- `index.html`
- `Build/`
- `TemplateData/`

## GitHub Pages setup

After the `docs` folder is pushed to GitHub:

1. Open the GitHub repository.
2. Go to `Settings`.
3. Open `Pages`.
4. Set `Source` to `Deploy from a branch`.
5. Select branch `master`.
6. Select folder `/docs`.
7. Save.

For this repository, GitHub Pages should publish the preview at:

`https://trisman909.github.io/LanternfallTacticsPrototype/`

Do not treat that URL as verified until it has been opened and the Unity start screen appears.

## WebGL settings

The build is configured for simple static hosting:

- WebGL compression disabled to avoid requiring special server headers.
- Data caching disabled for simpler refresh behavior while prototyping.
- Threads disabled for broad browser compatibility.
- 128 MB WebGL memory target.
- Mouse and touch-style input are preserved.
- The generated template is patched to use a full-viewport responsive canvas for desktop, phone portrait, and phone landscape browser previews.

## Known limitations

- Browser performance can vary, especially on older phones.
- Best played first on a desktop browser. Mobile browser play is prepared for testing but still experimental.
- iPhone Safari may behave differently from desktop browsers; physical-device testing is still needed.
- GitHub Pages must be enabled manually from repository settings before the public URL can be claimed as live.
- Full iPhone app testing still requires Unity iOS Build Support, a Mac, and Xcode.
- The prototype uses placeholder IMGUI/code-driven visuals.
- Audio and final art are not part of this preview milestone.

## GitHub Pages troubleshooting

- If the page shows an old version immediately after a push, wait a minute and hard refresh.
- If the page 404s, confirm repository `Settings > Pages` uses branch `master` and folder `/docs`.
- If loading stalls, try a desktop Chromium-based browser first, then retest on mobile.
- If the canvas is cropped, rotate the device or reload after the browser address bar collapses.
