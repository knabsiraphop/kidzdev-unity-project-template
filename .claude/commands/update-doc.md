---
description: "Manually update CHANGELOG.md and PROGRESS.md at repo root from current session's work."
argument-hint: "[optional note on what was done / what's next]"
allowed-tools: Bash, Read, Edit, Write
---

`$ARGUMENTS` — optional note from the user on what was done or what's next. If empty, derive it yourself.

Update the repo's `CHANGELOG.md` and `PROGRESS.md` (root of `C:\Dev\kidzdev-unity-project-template`) per the CLAUDE.md rule: every session's changes get logged there.

## Step 1 — Gather what changed

Run in parallel:
- `git status`
- `git diff`
- `git log -5 --oneline`

If nothing changed (clean tree, no new commits since last log entry) and `$ARGUMENTS` is empty, say so and stop — nothing to log.

## Step 2 — Update CHANGELOG.md

Read `CHANGELOG.md`. Add entries under `## Unreleased` (or today's date section if the user asks to cut one) — one line per meaningful change, what changed not why. Group by file/feature area if there are several. Terse, factual, no fluff.

## Step 3 — Update PROGRESS.md

Read `PROGRESS.md`. Add a new dated section at the top (newest-first) with:
- What was done this session (bullet list, derived from git diff/status + `$ARGUMENTS`)
- What's next (only if known — from `$ARGUMENTS`, open TODOs, or an obvious follow-up; otherwise omit)

Use today's date (check current date from context, format `YYYY-MM-DD`).

## Step 4 — Confirm

Report back in 1-2 lines what was logged. Do not commit — leave staging/commit to the user unless they explicitly ask.
