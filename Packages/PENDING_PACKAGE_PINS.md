# Pending package pins

Four entries in `manifest.json` are pinned by local `file:` path instead of a
real git URL, because the sibling package repos they point at
(`C:\Dev\addressables-toolkit-unity\addressables-toolkit\Packages\...`) have
no GitHub remote yet. This means the template is **not yet clone-portable**
to another machine or CI runner — anyone who clones
`kidzdev-unity-project-template` elsewhere needs that sibling checkout at the
same relative path, or these four entries will fail to resolve.

| Package | Current pin | Blocker | What it should become |
|---|---|---|---|
| `com.kidzdev.unity.project-conventions` | `file:` path | Local-only git repo, never pushed. | `https://github.com/knabsiraphop/kidzdev-unity-project-conventions.git#v0.1.0` (or matching tag) once a GitHub remote exists and a tag is cut. |
| `com.kidzdev.unity.async-utils` | `file:` path | Local-only git repo, never pushed. | Same as above once pushed + tagged. |
| `com.kidzdev.unity.cache` | `file:` path | Local-only git repo, never pushed. | Same as above once pushed + tagged. |
| `com.kidzdev.unity.object-pool` | `file:` path | Local-only git repo, never pushed. | Same as above once pushed + tagged. |

Creating each of these repos is a public, hard-to-reverse action and stays
gated on explicit user confirmation per standing convention — do not create
them silently while resolving this file.

## Resolved during this pass (kept here for the record)

- `com.kidzdev.unity.addressables-toolkit` looked like it had the same
  problem: its git remote has mismatched fetch/push URLs
  (`fetch: kidzdev-unity-addressables-toolkit.git`,
  `push: kidzdev-addressables-toolkit.git`). Verified via `git ls-remote`
  that **both URLs resolve to the same repo** — GitHub 301-redirects the
  `push` URL (the old, pre-rename name) to the `fetch` URL (current name).
  Tag `v1.5.1` exists on both and matches the package's `package.json`
  version, so this package **is** pinned by a real git URL above, not a
  `file:` path. The sibling repo's local `git remote` config still points
  partly at the stale pre-rename URL — worth fixing there at some point
  (`git remote set-url --push origin https://github.com/knabsiraphop/kidzdev-unity-addressables-toolkit.git`),
  but that's a mutation of a different repo and out of scope for this template
  scaffolding pass.
