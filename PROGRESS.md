# Progress Log

Session log for fast-tracking context across sessions. Newest on top.

## 2026-08-02
- Added CLAUDE.md rule: log all repo changes to CHANGELOG.md + PROGRESS.md each session.
- Created `/update-doc` slash command (`.claude/commands/update-doc.md`) to automate this log.
- Bootstrap scene load reworked: `GameBootstrap` now calls new `SceneTransition.LoadAsync` with fade in/out durations, replacing direct `SceneLoader.LoadAsync` call.
- Bumped `ui-overlay` package to v1.2.0, added `ui-animation` v1.0.0 package + asmdef ref.
- Imported TextMesh Pro assets (untracked in working tree).
- Used `/fable-advisor` to work out Addressable group strategy: recommendation was group-per-feature (mirroring the existing asmdef-per-feature boundary), MainMenu/shared UI staying in the shared/default group rather than its own feature group.
- Followed up with a second `/fable-advisor` consult on centralizing Addressables group schema settings (Local vs Remote) + a CI validator to catch schema drift across groups, matching the existing `AddressablesUsageValidator`/`HotPathAllocationValidator` pattern in `project-conventions`. Full plan exported to `addressables-group-schema-baseline-plan.md` (scratchpad, not checked into repo) — user is implementing this themselves in the `addressables-toolkit`/`project-conventions` sibling repos, not in this repo.
- Separately (outside this session's direct edits — found via git diff): Addressables flipped from Local to Remote content source, CDN URLs pointed at GitHub Pages, remote catalog build enabled; Addressable groups restructured (`Default Local Group` deleted, new `MainMenu` group created); `ConfirmPopup` prefab moved to `Assets/Resources/Prefabs/` and now loaded via a new `PopupKeys.cs` Resources-path constant instead of a direct prefab reference; `GameBootstrap.cs` further cleaned up (inlined scene-transition call, removed dead helper).
- Flagged: CLAUDE.md's "What NOT to assume" section still states the template baseline is `Local` content source — now stale since this project is on `Remote`. Needs reconciling.
- Renamed `Assets/_Project/` → `Assets/Scripts/` (naming preference, via `AssetDatabase.RenameAsset`, GUIDs preserved). Updated path references in `CLAUDE.md`/`ARCHITECTURE.md`.
- Fixed the resulting CI-exemption break: sibling `com.kidzdev.unity.project-conventions` package (`C:\Dev\addressables-toolkit-unity\...`, local `file:`-pinned checkout, not this repo) — `AddressablesUsageValidator.BootstrapPathMarker` updated `_Project/Runtime/Bootstrap/` → `Scripts/Runtime/Bootstrap/`, plus its README + a new Unreleased CHANGELOG entry there.
- Rebuilt Addressables content locally after the MainMenu-group/ConfirmPopup rework (stale `RemoteDemo` bundle artifact cleaned from `ServerData/`).
- Next: reconcile CLAUDE.md's Local/Remote baseline note with the new Remote config. Decide whether to check the Fable-advised schema-baseline plan into the repo as a doc or keep it scratchpad-only. Push `gh-pages` branch + enable GitHub Pages (user gated this explicitly — not yet done). Stage/commit all pending changes when ready (large pending diff — Addressables Remote flip, group restructure, popup Resources-path migration, `_Project`→`Scripts` rename, bootstrap cleanup, doc updates).
