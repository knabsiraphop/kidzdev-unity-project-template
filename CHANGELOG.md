# Changelog

Format: date, then change list. Newest on top.

## Unreleased
- Bootstrap: scene load now routes through new `SceneTransition.LoadAsync` (fade-in/out, configurable duration) instead of direct `SceneLoader.LoadAsync`. Added `sceneFadeInDuration`/`sceneFadeOutDuration` fields to `GameBootstrap`.
- Added `Assets/Scripts/Runtime/Bootstrap/SceneTransition.cs`.
- `KidzGame.Bootstrap.asmdef`: added reference to `KidzDev.Unity.UIOverlay`.
- Packages: bumped `com.kidzdev.unity.ui-overlay` to v1.2.0, added `com.kidzdev.unity.ui-animation` v1.0.0.
- README: package list updated with `ui-animation`.
- Added TextMesh Pro assets (untracked, now present in `Assets/TextMesh Pro/`).
- CLAUDE.md: added rule to log every session's changes to CHANGELOG.md + PROGRESS.md.
- Added `.claude/commands/update-doc.md` — manual `/update-doc` command to run this log update.
- `GameBootstrap.cs`: removed now-unused `LoadFirstSceneAsync` helper, inlined direct `SceneTransition.LoadAsync` call; dropped unused `UnityEngine.SceneManagement` using.
- **Addressables flipped Local → Remote**: `AddressablesToolkitSettings.asset` `contentSource` 0→1, `overrideRemoteUrl` on, all environment `CdnBaseUrl`s point to `https://knabsiraphop.github.io/kidzdev-unity-project-template/Addressables`, `contentVersion` set to `1.0.0`, `verboseLogging` on. `AddressableAssetSettings.asset`: `m_BuildRemoteCatalog` on, remote catalog build/load path IDs set, remote catalog build path URL set to the same GitHub Pages URL, `mainmenu-scene` added to preload labels. **Note: this contradicts the current CLAUDE.md "What NOT to assume" line stating the template baseline is `Local` — that doc note is now stale for this project and should be reconciled.**
- Addressable group restructure: deleted `Default Local Group` (+ its `BundledAssetGroupSchema`/`ContentUpdateGroupSchema`), added new `MainMenu` group (+ its own two schemas) — moves toward the per-feature/per-scene group naming discussed this session.
- Added `Assets/AddressableAssetsData/ProfileDataSourceSettings.asset` and `Windows.meta` (remote build output folder marker).
- Added `ProjectSettings/ScriptableBuildPipeline.json` (Addressables Scriptable Build Pipeline settings).
- `.gitignore`: added `/ServerData/` (Addressables remote content build output, not source).
- Moved `ConfirmPopup` prefab from `Assets/Prefabs/UI/ConfirmPopup.prefab` to `Assets/Resources/Prefabs/` — now loaded via Resources path instead of a direct serialized prefab reference.
- Added `Assets/Scripts/Runtime/PopupKeys.cs` — centralizes popup Resources-path constants.
- `BootstrapDownloadConfirmPopup.cs`: switched from `PopupRef.Direct(confirmPopupPrefab)` to `PopupRef.Resources(confirmPopupResourcesPath)` using new `PopupKeys.ConfirmPopup` constant; dropped the serialized `confirmPopupPrefab` field.
- `Assets/Settings/UniversalRP.asset`: shader-variant-stripping prefiltering flags changed (33 fields) — looks Editor-regenerated (e.g. from a build/validation pass), not a deliberate hand-edit; verify intentional before committing.
- Renamed `Assets/_Project/` → `Assets/Scripts/` (naming preference). Updated `CLAUDE.md`/`ARCHITECTURE.md` path references accordingly. Also updated the sibling `com.kidzdev.unity.project-conventions` package's `AddressablesUsageValidator.BootstrapPathMarker` (and its README/CHANGELOG) from `_Project/Runtime/Bootstrap/` to `Scripts/Runtime/Bootstrap/` so the CI exemption still matches — that package is a local `file:`-pinned sibling checkout, not this repo, so this edit lives outside `kidzdev-unity-project-template` itself.
