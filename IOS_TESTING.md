# Phase 4.5 — iPhone Export and Testing

## Ready in Unity

- Product name: **Lanternfall Tactics Prototype**
- Placeholder bundle identifier: `com.yourstudio.lanternfalltactics`
- Version: `0.4.5`; iOS build number: `1`
- iPhone-only target with iOS 15.0 minimum
- Autorotation supports portrait and both landscape directions; upside-down portrait is disabled
- Dynamic safe-area layout for notches, Dynamic Island, and the home indicator
- Single-touch input, 30 FPS target, Gamma color space, no anti-aliasing, shadows, soft particles, or realtime reflection probes
- `Lanternfall.EditorTools.BuildPrototype.ExportIOS` exports an Xcode project to `Builds/iOS` when Unity iOS Build Support is available

## Not available on this workstation

Unity iOS Build Support is not installed for Unity 6000.5.1f1. The expected `Editor/Data/PlaybackEngines/iOSSupport` directory is absent, so this milestone does not contain an exported Xcode project. Install the matching **iOS Build Support** module through Unity Hub before running the export method.

Windows can prepare and export the Unity Xcode project after that module is installed, but it cannot compile, sign, or install the app. Those final steps require macOS and Xcode.

## Run on an iPhone later

1. Install Unity iOS Build Support for the exact Unity editor version.
2. Run `Lanternfall.EditorTools.BuildPrototype.ExportIOS` in Unity to create `Builds/iOS`.
3. Copy the complete exported folder to a Mac.
4. Install the current Xcode release and open `Unity-iPhone.xcodeproj`.
5. Select the `Unity-iPhone` target, open **Signing & Capabilities**, choose your Apple team, and replace the placeholder bundle identifier with one unique to you if necessary.
6. Connect the iPhone to the Mac, trust the computer, and enable Developer Mode on the phone if iOS requests it.
7. Select the connected iPhone as the Xcode run destination, then press **Run**.
8. Test portrait rotation, both landscape directions, notch/Dynamic Island clearance, home-indicator clearance, every tap flow, hazards, rewards, boss completion, defeat, and restart.

An ordinary Apple Account can be used with Xcode's free personal provisioning for testing on your own device, subject to Apple's limitations and short provisioning lifetime. Membership in the paid Apple Developer Program is required for normal TestFlight/App Store distribution and broader production signing workflows.

## Device checks still required

- Exact font rendering and wrapping on the target iPhone model
- Real finger comfort and accidental double-tap behavior
- Safe areas during rotation and system overlays
- Sustained frame pacing, temperature, and battery use
- Audio interruption, suspend/resume, and incoming-call behavior

No signed iPhone build or physical-device result is claimed until these steps are completed on a Mac with Xcode.
