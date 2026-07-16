# Phase 6L.2 WebGL class ID 115 diagnosis

> Historical diagnostic record. Phase 6L.6 subsequently proved that the class-ID serialization investigation was not the runtime audio cause. After a clean project-state regeneration, the production player loaded correctly; the remaining Web Audio exception was independently traced to the uninitialized ambient step documented in `POLISH_6L.md` and fixed with finite-value guards. The earlier blocked status below is retained as investigation history rather than current release status.

## Baseline

- Branch: `master`
- Starting HEAD: `ad6a0586dce2b09a4b1b3e1a3077e9bf9439434b`
- Phase 6L implementation: `810fe75bbed09fa02465454b7529da1525a1f...`
- Live rollback: `ad6a058` with the validated Phase 6K.1 `docs` content from `722e973`
- Unity: `6000.5.1f1 (0d9463e84828)`
- WebGL backend: IL2CPP, non-development, engine-code stripping enabled
- Project serialization settings: `stripEngineCode: 1`, no explicit managed-stripping override, no preloaded assets, no configured Editor build scenes
- The build method creates one empty `Assets/Scenes/Main.unity` scene and passes it directly to `BuildPipeline.BuildPlayer`.
- Runtime assembly: `Lanternfall.Core.dll`; test assembly is Editor-only.

## Exact runtime stage and evidence

The clean Phase 6L production output was served from:

`http://127.0.0.1:8766/index.html?phase6l2=baseline2`

The Codex in-app Chromium browser reached `document.readyState=complete`, created a 1920x1080 Unity canvas, hid the loading bar, and reported 100% loading progress. The loader, framework, data archive, WASM, and data decompression therefore completed before the failure.

The first captured runtime messages were repeated class-instantiation failures followed by Unity startup/deserialization messages:

```text
Could not produce class with ID 115.
This could be caused by a class being stripped from the build even though it is needed.
Try disabling 'Strip Engine Code' in Player Settings.
Input Manager initialize...
UnloadTime: 0.299999 ms
The referenced script (Unknown) on this Behaviour is missing!
The referenced script on this Behaviour (Game Object '<null>') is missing!
A scripted object (script unknown or not yet loaded) has a different serialization layout when loading. (Read 40 bytes but expected 8580 bytes)
Did you #if UNITY_EDITOR a section of your serialized properties in any of your scripts?
```

No scene name or project asset path appeared. Normal game initialization and `LanternfallView.Boot` never completed. No Web Audio plug-in call occurred. The earliest failure is player-data/global-manager deserialization, before the first frame, runtime audio initialization, Resources loading, or gameplay startup.

Principal clean-output SHA-256 values captured before the diagnostic build were:

- `LanternfallTactics.data`: `08F701452E0D4AD910631ECEE985BC6129BE1267C0198F3FDD4FD243AE229C205`
- `LanternfallTactics.wasm`: `95B070C0B961F7E9043C81153A71AB46329123EF5115B755C1220C344922F1E7`
- `LanternfallTactics.framework.js`: `BB7634048C978F127A184A66FCDBAC4DD76CA70508D3D663B5ECC1F44953885D`

## Serialized-file comparison

The Phase 6K.1 and Phase 6L WebGL data archives were unpacked read-only. Both `data.unity3d` bundles contain:

- `globalgamemanagers`
- `Resources/unity_builtin_extra`
- `globalgamemanagers.assets`
- `sharedassets0.assets`
- `level0`
- `resources.assets`

`level0`, `sharedassets0.assets`, and `resources.assets` did not gain Phase 6L serialized content. The only new object in `globalgamemanagers.assets` is:

```text
serialized file: globalgamemanagers.assets
path ID:         25
class ID:        115 (MonoScript)
size:            96 bytes
name:            LanternfallAudio
namespace:       Lanternfall
assembly:        Lanternfall.Core
script GUID:     de1a4b551ff6b9247bece72e9dfa3e5b
payload SHA-256: 73F51DDCBB94425331328412C694D1843F341F6D4F5034BBA3E8093E9EEBC0C4
```

Phase 6K.1 has 30 objects in this file; Phase 6L has 31. The metadata grows by exactly one 24-byte object-table entry and the data section gains the aligned 96-byte `LanternfallAudio` record. Its GUID matches `Assets/Scripts/LanternfallAudio.cs.meta`. There is no broken, duplicated, or unresolved GUID.

The generated `TypesInScenes.xml` explicitly lists `UnityEditor.MonoScript`, while `UnityLinkerToEditorData.json` classifies native `MonoScript` as a dependency. The stripped managed output contains `Lanternfall.Core.dll`, and generated IL2CPP code contains `LanternfallAudio` and its WebGL backend. This rules out removal of the managed audio facade itself.

No `SerializeReference`, serialized `IAudioService`, reflection-based construction, `Activator.CreateInstance`, `Type.GetType`, assembly scanning, editor assembly leak, preloaded asset, Resources script object, or runtime test assembly reference was found. Backend selection uses compile guards and explicit constructors. The `.jslib` is correctly placed under `Assets/Plugins/WebGL`; the runtime error precedes its first call.

## Focused Phase 6K.1 to Phase 6L differences

Runtime-relevant additions are the `LanternfallAudio.cs` source/meta pair, the WebGL `.jslib` source/meta pair, explicit audio calls and controls in `LanternfallView`, the `com.unity.modules.audio` manifest entry, and normal gameplay-polish changes unrelated to serialization. ProjectSettings changed only the bundle version. The generated scene remained empty.

## Ranked hypotheses

1. **Unity 6000.5.1f1 engine-stripping defect triggered by the additional serialized `MonoScript` record.** Strongest evidence: the exact new class-115 object and immediate class-115 deserialization failure; all managed code and GUID data are valid.
2. **Invalid explicit `MonoScript`/editor object in runtime data.** Partially supported because `MonoScript` is explicitly serialized, but its payload and GUID are valid and identical in layout to the other working script records.
3. **Missing or invalid script GUID.** Evidence against: the serialized GUID exactly matches the source `.meta`, and no missing reference exists in project YAML.
4. **SerializeReference or reflection-created backend stripped.** Evidence against: neither mechanism is used; IL2CPP generated the concrete WebGL types.
5. **Runtime/editor assembly placement error.** Evidence against: runtime audio is in `Lanternfall.Core`; editor and test assemblies are excluded correctly.
6. **Preloaded/Resources asset with a script object.** Evidence against: preloaded assets are empty and Resources contains only authored PNG atlases.
7. **Audio package interaction.** Possible but weaker: the package enables audio compilation, while both the known-good and failing player-data assembly manifests already enumerate `UnityEngine.AudioModule`.
8. **Unrelated WebGL linker defect.** Possible, but the serialized delta identifies a more specific trigger.

## Controlled diagnostic experiment

One variable was tested without changing the audio-service contract:

- moved the existing audio facade/backends into `LanternfallView.cs`;
- changed the facade from `MonoBehaviour` attachment to explicit plain-object construction;
- removed only the standalone `LanternfallAudio.cs`/`.meta` script asset;
- retained `IAudioService`, Web Audio, the native backend, explicit backend constructors, saved settings, and all UI/gameplay call sites;
- retained engine-code stripping and forced clean player-data generation.

The newly generated partial data archive contains zero `LanternfallAudio` strings, confirming that the suspected serialized record was removed. The WebGL build did not complete: Unity repeatedly lost its asset-worker/player-build connection, eventually reached native compiler activity, then stalled with zero Unity CPU delta, zero build-backend CPU delta, and zero log growth for 45 seconds. The process tree was terminated after approximately 13 minutes. No browser runtime result was produced, so the hypothesis is not yet proven as a release fix.

The experiment and temporary `CleanBuildCache` option were reverted. Diagnostic builds used: **1 of 2**. No `link.xml`, `[Preserve]`, broad assembly retention, package change, stripping change, or publish occurred.

## Status and smallest next step

Phase 6L remains blocked. The live Phase 6K.1 build remains untouched. The previous 150/150 passing test baseline is retained; the Phase 6L.2 test invocation completed script import but did not emit a test-results XML file, so it is not counted as a new test run.

The smallest next step is to open the project once in Unity 6000.5.1f1, allow asset workers to become fully ready, and run the same single serialization-neutral diagnostic build without clearing caches or changing source. If it loads locally, the exact corrective workaround is to keep the audio implementation in an existing runtime source asset (or another arrangement that does not add the standalone `LanternfallAudio` MonoScript record). If it still fails, use the remaining diagnostic-build allowance to remove only `com.unity.modules.audio` while keeping the WebGL backend, which would isolate the package interaction.
