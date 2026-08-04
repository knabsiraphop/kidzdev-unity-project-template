# Architecture

Rules for this project. Companion to `CLAUDE.md` (agent-facing) — this file is
the human-facing source of truth; `CLAUDE.md` points back here rather than
duplicating it.

## Assembly boundaries — the no-spaghetti mechanism

The mechanical cause of spaghetti in Unity projects is an assembly boundary
that lets any class reference any other. This project makes illegal
cross-feature coupling a **compile error**, not a code-review nit:

```
Assets/Scripts/
  Runtime/
    KidzGame.Core.asmdef        # shared DTOs + small interfaces only, zero references
    Bootstrap/
      KidzGame.Bootstrap.asmdef # references Core + AddressablesToolkit + UniTask + UnityEngine.UI
      GameBootstrap.cs
    Features/
      <FeatureName>/
        KidzGame.Features.<FeatureName>.asmdef   # one asmdef per feature, explicit minimal references
  Editor/
    KidzGame.Editor.asmdef
  Tests/
    EditMode/KidzGame.Tests.EditMode.asmdef
    PlayMode/KidzGame.Tests.PlayMode.asmdef
```

Rules:
- One asmdef per feature, explicit minimal reference list (Core + whichever
  KidzDev packages that feature needs).
- **No feature asmdef references another feature's asmdef.** Cross-feature
  communication goes through `KidzGame.Core` DTOs/interfaces + events, not
  direct references — same "inject rather than couple directly" rule the rest
  of the KidzDev ecosystem already uses.
- `Core`, `Editor`, `Tests/EditMode`, `Tests/PlayMode` ship with **no scripts**
  in this template — that's intentional. Unity will warn
  "will not be compiled, because it has no scripts associated with it" for
  each on first open; ignore it. The asmdefs exist so the reference graph is
  already in place when a real project starts adding files.
- **Everything must live under `Assets/` (or `Packages/`).** A folder at the
  repo root next to `Assets/` is invisible to Unity's AssetDatabase — it will
  not compile, no matter how correct the C# inside it is. `Scripts/` lives at
  `Assets/Scripts/`, not at the repo root, for exactly this reason.

## Addressables-toolkit as mandatory, not optional

Three layers, so it's structural rather than a convention someone can quietly
ignore:

1. **Pinned at clone time** — already in `Packages/manifest.json`; you cannot
   not have it. See `Packages/PENDING_PACKAGE_PINS.md` for which entries are
   still local `file:` pins vs. real git URLs.
2. **`GameBootstrap.cs`** (`Assets/Scripts/Runtime/Bootstrap/GameBootstrap.cs`)
   is the only MonoBehaviour in the Bootstrap scene (`Assets/Scenes/Bootstrap.unity`,
   Build Settings index 0). It calls `AddressablesService.InitializeAsync`,
   gates on the result, then hands off via `SceneLoader.LoadAsync` — never a
   raw `SceneManager.Load*`/`Addressables.*` call outside this folder or the
   toolkit's own assembly.
3. **`AddressablesUsageValidator`** (from `com.kidzdev.unity.project-conventions`)
   CI-fails on any direct `Addressables.*`/`SceneManager.Load*` call outside
   the toolkit's own assembly or `Scripts/Runtime/Bootstrap/`. The exemption
   is a **literal path-string match** on `/Scripts/Runtime/Bootstrap/` —
   exact case, exact slashes. Move `GameBootstrap.cs` or rename the folder and
   the exemption silently stops applying; CI will then fail on the file
   itself.

### Bootstrap scene contract

- `Assets/Scenes/Bootstrap.unity`, Build Settings index 0, enabled. No other
  scene needs to be in Build Settings — every scene after Bootstrap loads via
  an Addressable key through `SceneLoader`, not the build list.
- `GameBootstrap` sequence: `InitializeAddressablesAsync` → (fail: log + stop,
  do not proceed) → `LoadFirstSceneAsync(firstSceneAddress)`.
- `AddressablesToolkitSettings.autoInitializeOnLaunch` **must stay `false`**
  (`Assets/Resources/AddressablesToolkitSettings.asset`). `GameBootstrap` owns
  initialization explicitly; the toolkit's own auto-init-at-launch path would
  race/duplicate it. It's idempotent so nothing breaks if both fire, but it's
  confusing and pointless — leave it off.
- `progress`/`confirm` on `AddressablesService.InitializeAsync` only matter
  once `AddressablesToolkitSettings.contentSource` is `Remote` and
  `predownloadPreloadContent` is on — for the template's `Local` baseline that
  whole branch is skipped, so passing null/optional hooks is not lying about
  behavior, it's dead-until-configured.
- `GameBootstrap` exposes two optional hooks, both null-safe if unassigned:
  - `progressReporter` (`BootstrapProgressReporter`) — drives a filled
    `Image.fillAmount` directly and/or raises a `UnityEvent<float>` for
    anything fancier. The template's own Bootstrap scene wires this to a
    simple loading bar (`Canvas/LoadingBarBackground/LoadingBarFill`).
  - `downloadConfirmGate` (`BootstrapDownloadConfirmGate`, abstract) — subclass
    it in a project that needs consent before large downloads (e.g. a
    "Download 40MB?" popup). Unassigned = auto-proceed.
- **Sign-in-before-Addressables** (or any other pre-init step) is a per-project
  extension, not template baseline — insert it as an extra awaited step at the
  top of `GameBootstrap`'s orchestrator method, ideally behind an interface in
  `Core` rather than a raw call to a specific backend package (e.g.
  `com.kidzdev.unity.nakama-client`), so `Bootstrap` stays a thin orchestrator.

## GC / memory discipline

- **Mechanically checkable** (hot-path `new`, LINQ, `Instantiate`/`Destroy`,
  `GetComponent`/`FindObjectOfType`/`Camera.main`, string concat/interpolation
  inside `Update`/`FixedUpdate`/`LateUpdate`/`OnGUI`) — enforced by
  `HotPathAllocationValidator`, CI-gateable. Only scans `Assets/`; everything
  under any `Packages/` folder is skipped.
- **Not automatable — human-reviewed checklist**:
  - Struct-vs-class field sizing judgment.
  - Pre-sized collection capacity (`new List<T>(expectedCount)` instead of
    growing from empty in a hot path).
  
  These need profiling judgment a line-scanner can't honestly claim. Review
  them by hand; don't assume CI is catching them.

## Relationship to `utilities` (backlog, not yet built)

Kept separate, not merged. `utilities` (if/when built) is a
runtime-consumable grab-bag of math/file/rendering functions meant to be
`using`'d by game code. This template + `project-conventions` is Editor-only
inspection tooling plus a project skeleton — never ships in a player build,
not gated on `utilities` existing.

## Package pinning

See `Packages/PENDING_PACKAGE_PINS.md` for the current state of every
`com.kidzdev.*` entry in `manifest.json` — which are real git URL + tag pins
vs. local `file:` pins waiting on a GitHub remote for the sibling package.
