# Progressive Web App audit

Lanternfall Tactics uses the existing HTTPS GitHub Pages origin and `/LanternfallTacticsPrototype/` scope.

## Install contract

- Manifest: `manifest.webmanifest`, served as `application/manifest+json`.
- Identity: explicit relative `id`, `start_url`, and `scope` so the contract remains correct under the GitHub Pages project path.
- Display: `fullscreen`, with `standalone` and `minimal-ui` fallbacks.
- Icons: PNG icons at 192x192 and 512x512 plus dedicated maskable variants; iOS receives a 180x180 Apple touch icon.
- Service worker: same-scope registration, lightweight shell precache, navigation network-first fallback, and cache-first immutable Unity/PWA assets.
- Offline behavior: the shell is installed immediately; Unity loader, framework, data, and WASM responses are cached during the first successful online play session.
- iOS: Apple mobile-web-app metadata and touch icon support Safari Add to Home Screen.

The service worker deliberately does not precache the full 22 MB Unity player during installation. This keeps service-worker activation reliable on mobile browsers while ensuring the complete game becomes offline-capable after its first successful load.
