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

GitHub will publish the preview at a URL similar to:

`https://trisman909.github.io/LanternfallTacticsPrototype/`

## WebGL settings

The build is configured for simple static hosting:

- WebGL compression disabled to avoid requiring special server headers.
- Data caching disabled for simpler refresh behavior while prototyping.
- Threads disabled for broad browser compatibility.
- 128 MB WebGL memory target.
- Mouse and touch-style input are preserved.

## Known limitations

- Browser performance can vary, especially on older phones.
- iPhone Safari may behave differently from desktop browsers; physical-device testing is still needed.
- Full iPhone app testing still requires Unity iOS Build Support, a Mac, and Xcode.
- The prototype uses placeholder IMGUI/code-driven visuals.
- Audio and final art are not part of this preview milestone.
