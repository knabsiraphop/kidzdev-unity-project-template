# CLAUDE.md

This is a **consumer game project** cloned from `kidzdev-unity-project-template`
— application code, not a `com.kidzdev.unity.*` UPM package. No
package.json/CHANGELOG discipline here; that applies to the packages this
project depends on, not to this repo itself.

`ARCHITECTURE.md` is the source of truth for the rules below — read it before
making structural changes. This file is the short version + agent-specific
notes.

## Non-negotiable rules

- **Never call `Addressables.*` or `SceneManager.Load*` directly** outside
  `Assets/_Project/Runtime/Bootstrap/` or a KidzDev package's own assembly.
  Use `AssetLoader`/`AddressablePool`/`SceneLoader` from
  `com.kidzdev.unity.addressables-toolkit` instead. CI enforces this via
  `AddressablesUsageValidator` — the exemption is an exact path-string match
  on `/_Project/Runtime/Bootstrap/`, so don't relocate or rename that folder.
- **No feature asmdef references another feature's asmdef.** Route
  cross-feature communication through `KidzGame.Core` DTOs/interfaces +
  events. See `ARCHITECTURE.md` → "Assembly boundaries."
- **Everything must live under `Assets/` or `Packages/`.** Unity's
  AssetDatabase does not see anything outside those two roots — a script
  placed at the repo root will silently never compile.
- Avoid hot-path allocation (`new` on reference types, LINQ, `Instantiate`/
  `Destroy`, `GetComponent`/`FindObjectOfType`/`Camera.main`, string
  concat/interpolation) inside `Update`/`FixedUpdate`/`LateUpdate`/`OnGUI`.
  `HotPathAllocationValidator` CI-gates this for everything under `Assets/`.
- **Confirm with the user before any public-facing or hard-to-reverse GitHub
  action** — creating a repo, first push, force-push, tagging a release.
  This applies to this project too, not just the template it came from.

## CI entry points

Both validators come from `com.kidzdev.unity.project-conventions` and are
`-executeMethod` callable in batch mode (they call `EditorApplication.Exit(1)`
only when `Application.isBatchMode && violationCount > 0`):

```
KidzDev.Unity.ProjectConventions.Editor.HotPathAllocationValidator.Validate
KidzDev.Unity.ProjectConventions.Editor.AddressablesUsageValidator.Validate
```

## Project structure quick reference

```
Assets/_Project/Runtime/KidzGame.Core.asmdef            # DTOs/interfaces, zero references
Assets/_Project/Runtime/Bootstrap/                       # GameBootstrap.cs lives here — see ARCHITECTURE.md
Assets/_Project/Runtime/Features/<Name>/                 # one asmdef per feature
Assets/_Project/Editor/, Assets/_Project/Tests/          # empty placeholders, ship with no scripts by design
Assets/Scenes/Bootstrap.unity                            # Build Settings index 0, the only build-list scene
Assets/Resources/AddressablesToolkitSettings.asset       # autoInitializeOnLaunch must stay false
Packages/manifest.json                                   # see Packages/PENDING_PACKAGE_PINS.md for pin status
```

## What NOT to assume

- Don't assume every `com.kidzdev.*` package in `manifest.json` is pinned by a
  real git URL — some are local `file:` paths pointing at a sibling checkout.
  Check `Packages/PENDING_PACKAGE_PINS.md` before recommending a clone of this
  repo onto another machine or CI runner.
- Don't assume `AddressablesToolkitSettings.contentSource` is `Remote` — the
  template baseline is `Local`, which skips CDN/catalog/predownload entirely.
  `progress`/`confirm` hooks on `InitializeAsync` are inert until a project
  switches to `Remote`.
