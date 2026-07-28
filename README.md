# kidzdev-unity-project-template

Base project structure for every new KidzDev Unity game. Unity 6000.0, URP
2D, Addressables-first, per-feature assembly boundaries enforced at compile
time. See `ARCHITECTURE.md` for the rules and why they exist, `CLAUDE.md` for
the short agent-facing version.

## Use this template

Once this repo is pushed and marked as a GitHub template
(`Settings > Template repository`): click **Use this template** on GitHub, or

```
git clone --depth 1 <this-repo-url> my-new-game
cd my-new-game
rm -rf .git && git init
```

## Step 0 — first time you open the project

1. Let Unity resolve packages on first open (Package Manager will show
   resolution progress). Check `Packages/PENDING_PACKAGE_PINS.md` — some
   `com.kidzdev.*` packages are pinned by local `file:` path rather than a git
   URL until their sibling repos get a GitHub remote; those need a sibling
   checkout at the matching relative path until resolved.
2. Verify `Assets/Resources/AddressablesToolkitSettings.asset` exists with
   `autoInitializeOnLaunch = false` (already true in this template — don't
   flip it on; `GameBootstrap.cs` owns initialization explicitly).
3. Verify `ProjectSettings > Editor > Enter Play Mode Settings` has both
   "Reload Domain" and "Reload Scene" unchecked (already set in this
   template — this is `EnterPlayModeOptions = 3` in
   `ProjectSettings/EditorSettings.asset`).
4. Confirm `Assets/Scenes/Bootstrap.unity` is Build Settings index 0 and
   enabled (already set in this template).
5. If your game needs remote content: open
   `Assets/Resources/AddressablesToolkitSettings.asset`, set
   `contentSource = Remote`, configure your CDN environments, and wire a
   `BootstrapProgressReporter`/`BootstrapDownloadConfirmGate` on the
   `GameBootstrap` GameObject in the Bootstrap scene if you want a loading UI
   or a download-consent prompt (template ships with a simple loading bar
   already wired — replace or remove it as needed).

## Default pinned packages

`addressables-toolkit`, `extensions`, `singleton`, `state-machine`,
`async-utils`, `cache`, `object-pool`, `ui-overlay`, `popup`,
`screen-navigator`, `audio`, `local-save`, plus `project-conventions`
(Editor-only tooling). Deliberately excludes game-specific packages
(`joystick`, `grid`, `chat-view`, `voice-chat`, `nakama-client`, etc.) — add
those per-project as needed via Package Manager.

## CI

Two validators from `com.kidzdev.unity.project-conventions`, batch-mode
callable, exit code 1 on violations:

```sh
Unity -batchmode -nographics -projectPath . -quit \
  -executeMethod KidzDev.Unity.ProjectConventions.Editor.HotPathAllocationValidator.Validate

Unity -batchmode -nographics -projectPath . -quit \
  -executeMethod KidzDev.Unity.ProjectConventions.Editor.AddressablesUsageValidator.Validate
```

Wire both into your CI pipeline as separate steps so a failure in one doesn't
mask the other.

## License

MIT — see `LICENSE`.
