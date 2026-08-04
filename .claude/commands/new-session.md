---
description: "Read CHANGELOG.md + PROGRESS.md + current git/repo state, rebuild context, and prep this session to continue from where the last one left off."
argument-hint: "[optional focus for this session]"
allowed-tools: Bash, Read, Glob, Grep
---

`$ARGUMENTS` — optional focus for this session (what the user wants to work on). If empty, infer from PROGRESS.md's latest "Next" line.

Goal: fast-track this session's context from the logs, without re-deriving everything from scratch. Read-only reconnaissance — do not edit files or run destructive git commands in this command.

## Step 1 — Read the logs

Read `CHANGELOG.md` and `PROGRESS.md` (repo root). Focus on the most recent (top) entries.

## Step 2 — Check reality against the logs

Run in parallel:
- `git status --porcelain=v1`
- `git log -5 --oneline`
- `git diff --stat`

Compare against what PROGRESS.md's last entry expected (e.g. "pending commit", "next: X"). Flag mismatches — e.g. logged as pending but tree is now clean (someone committed since), or new uncommitted changes not reflected in the logs (last session's `/update-doc` never ran).

## Step 3 — Surface open threads

From PROGRESS.md's latest "Next" line(s) and CHANGELOG.md's `## Unreleased` section, list what's outstanding. Note anything that looks stale (references a file that no longer exists — check with Glob/Grep) or already done (check git log for a matching commit).

## Step 4 — Report

Give the user a short brief:
- Where the last session left off (1-3 lines).
- Current repo state vs. what was logged (clean / dirty, matches or diverges).
- Open "Next" items, oldest-relevant first.
- If `$ARGUMENTS` given, confirm it aligns with logged next-steps (or flag if it's a new direction not in the logs).

Do not take further action (no edits, no commits, no `/update-doc`) — just brief and wait for the user's go-ahead. If the logs are missing or empty, say so and fall back to `git log` for context.
