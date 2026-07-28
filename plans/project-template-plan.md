# `kidzdev-unity-project-template` — Plan

Date: 2026-07-28. Companion to `project-conventions-plan.md` (lives in the `addressables-toolkit` host project repo at `Packages/com.kidzdev.unity.addressables-toolkit`'s sibling plan doc — see that repo's `plans/project-conventions-plan.md`). Origin: `/fable-advisor` consult on the user's "base project structure for every future Unity project" idea. This is the second half of a two-artifact answer.

## What this is

A **separate GitHub template repo** (public, per user decision), not another `com.kidzdev.unity.*` embedded package. Rationale: a project's `Assets/` layout, `ProjectSettings/`, and `Packages/manifest.json` are owned by the *consuming project*, not something a UPM package can generate-and-own without drifting from what it wrote. "Clone this to start every future project" is mechanically a GitHub template repo (`git clone` / "Use this template" button), not a scaffolder script.

Accepted tradeoff: a template repo doesn't retroactively update projects already cloned from it — same accepted-drift-over-coupling tradeoff the rest of the KidzDev ecosystem already takes (e.g. package duplication instead of cross-coupling). Future convention changes only reach new projects unless someone manually re-pulls.

## Decisions (locked by user, 2026-07-28)

- Repo name literal: `kidzdev-unity-project-template`.
- **Public** on GitHub, `knabsiraphop` profile (same as shipped packages) — confirm exact repo visibility/creation with the user before actually running `gh repo create`, per standing rule to confirm before public-facing/hard-to-reverse actions.
- Depends on `com.kidzdev.unity.project-conventions` — **already built and committed** (own local git repo, not pushed yet) in the addressables-toolkit host project at `Packages/com.kidzdev.unity.project-conventions`. This package still needs its own GitHub remote created before the template can pin it via git URL — confirm with user when ready (same public-repo-creation gate).
- Default pinned `manifest.json` set: `addressables-toolkit`, `extensions`, `singleton`, `state-machine`, `async-utils`, `cache`, `object-pool`, `ui-overlay`, `popup`, `screen-navigator`, `audio`, `local-save`. Deliberately excludes game-specific packages (`joystick`, `grid`, `chat-view`, `voice-chat`, `nakama-client`, etc.) — those get added per-project as needed.

## Folder / assembly layout

```
_Project/
  Runtime/
    KidzGame.Core.asmdef        # shared DTOs + small interfaces only. Features may reference this; features never reference each other.
    Bootstrap/
      GameBootstrap.cs          # the ONLY MonoBehaviour in the first scene. Awaits AddressablesService.Default.InitializeAsync(ct), gates on Ready, hands off via SceneLoader (never raw SceneManager).
    Features/
      <FeatureName>/
        KidzGame.Features.<FeatureName>.asmdef   # one asmdef per feature, explicit minimal references (Core + whichever KidzDev packages it needs) — no feature-to-feature asmdef references
  Editor/
    KidzGame.Editor.asmdef
  Tests/
    EditMode/
    PlayMode/
Packages/
  manifest.json                 # pre-pinned with the substrate list above + project-conventions
ProjectSettings/                # IL2CPP, .NET Standard 2.1, Enter Play Mode Options pre-enabled (disable domain reload + scene reload), one AddressableAssetSettings asset + one default group already created
Assets/
  Art/ Audio/ Prefabs/ ScriptableObjects/ Scenes/
ARCHITECTURE.md                 # human-facing rule list incl. the two checks the validator can't automate (struct sizing, pre-sized collections)
CLAUDE.md                       # mirrors this ecosystem's own conventions doc, tuned for agent-driven work on a *consumer* game project (not a package repo)
README.md                       # "Use this template" instructions, CI wiring for the two project-conventions menu commands via -executeMethod
LICENSE                         # MIT, KidzDev
```

## No-spaghetti mechanism

The actual mechanical cause of spaghetti in Unity projects is an assembly boundary that lets any class reference any other. Per-feature asmdefs with an **explicit, minimal reference list** make illegal cross-feature coupling a **compile error**, not a code-review nit. Cross-feature communication goes through `KidzGame.Core` DTOs/interfaces + events/mediator pattern — consistent with the ecosystem's existing "inject persistence/IO rather than couple directly" rule.

## Addressables-toolkit as mandatory, not optional

Three layers, so it's structural rather than a convention someone can quietly ignore:

1. **Pinned at clone time** — already in `manifest.json`; you cannot not have it.
2. **`GameBootstrap.cs` is the only entry point** — awaits `AddressablesService.Default.InitializeAsync(ct)`, gates on `Ready`, hands scene transitions to `SceneLoader`.
3. **`AddressablesUsageValidator`** (from the `project-conventions` package) — CI-fails on any direct `Addressables.*`/`SceneManager.Load*` call outside the toolkit's own assembly or `_Project/Runtime/Bootstrap/`.

## GC / memory discipline

- Mechanically checkable rules (hot-path `new`, LINQ, `Instantiate`/`Destroy`, `GetComponent`/`Find*`, string concat/interpolation) — enforced by `HotPathAllocationValidator`, CI-gateable.
- Non-automatable rules (struct-vs-class field sizing judgment, pre-sized collection capacity) — listed in `ARCHITECTURE.md` as a **human-reviewed checklist**, explicitly labeled "not automated" so the doc never overclaims a guarantee the tooling doesn't provide.

## Relationship to `utilities` (still-unbuilt backlog candidate in the addressables-toolkit host project)

Kept separate, not merged. `utilities` is a runtime-consumable grab-bag of math/file/rendering functions meant to be `using`'d by game code. This template + `project-conventions` is Editor-only inspection tooling plus a project skeleton — never ships in a player build, not gated on `utilities` existing.

## Build steps

1. ~~Build `com.kidzdev.unity.project-conventions` first~~ — **done**. Package built, 49 EditMode tests green, committed locally in its own repo at `Packages/com.kidzdev.unity.project-conventions` inside the addressables-toolkit host project. Not yet pushed to a GitHub remote. Design note from that build: both validators skip everything under any `Packages/` folder (embedded/local/registry/git alike) to avoid noisy false positives on sibling KidzDev packages' own `Update()` methods.
2. Scaffold the `_Project/` layout + `GameBootstrap.cs` + `ARCHITECTURE.md`/`CLAUDE.md`/`README.md` in **this** project — can be built and reviewed locally before any GitHub repo exists. **This is the current step.**
3. **Confirm with user** before running `gh repo create` / first push — new public repo creation is a hard-to-reverse, externally-visible action, do not do this silently even though visibility was pre-approved as "public." Note: `project-conventions` also needs its GitHub remote created before this template's `manifest.json` can pin it via a real git URL — until then, reference it locally (e.g. a `file:` path to the sibling checkout, or a placeholder git URL with a TODO) so scaffolding work isn't blocked on that separate confirmation.
4. Mark repo as a GitHub template (`Settings > Template repository`) once pushed.

## Open items still needing a human (carried from Fable's brief, not yet resolved)

- Whether `project-conventions` should eventually live inside a future `utilities` package's `Editor/` tier instead of standing alone — backlog-sequencing call, not architecture; deferred, not blocking.
