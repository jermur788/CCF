# Unity SSD Headroom Remediation
**Label: LOCAL/CODE REQUIRED — STORAGE/OPERATIONS**
**Claim #152**

Parent: #91 / #106.

## Purpose
Restore enough free space on `/media/jer/ZX20` for safe Unity qualification after PARITY-03 Q10 #74 crossed the mandatory 15 GiB machine floor during a fresh Unity import.

This task is storage remediation only. It grants no production-code ownership and must not alter gameplay, verification assertions, save/schema state, branches, commits, or user evidence.

## Difficulty
**2/5** — operationally simple, but unsafe deletion could destroy unrelated work.

## Best-suited executor
T2/T3 Code local shell/filesystem lane. No Unity run is required or permitted during remediation.

## Exact lane
No new Git worktree is required. Operate read-only across `/media/jer/ZX20` and `/media/jer/Videos` except for the specifically authorised generated-cache deletions below.

## Preflight
1. Refresh live `Docs/Project/coordination/active-tasks.md` and claims #74, #129, #147, #152 plus every currently RUNNING local claim.
2. Verify no Unity Editor/process owns the machine-wide lock.
3. Record free bytes on `/media/jer/ZX20`.
4. Inventory the largest CCF-related directories on `/media/jer/ZX20` and `/media/jer/Videos` before deleting anything.
5. For each cleanup candidate, inspect Git worktree status and claim ownership.

## Classification
Classify each candidate as:
- **ACTIVE / PROTECTED** — current RUNNING ownership, current candidate lane, active evidence or uncertain dependency. Read-only.
- **DIRTY / UNKNOWN** — any modified/untracked state or ownership uncertainty. Read-only and report.
- **DISPOSABLE-GENERATED** — regenerable Unity cache/import output belonging to a clean, inactive/released worktree whose source and required evidence are already preserved.

## Pre-authorised deletion boundary
The only deletions authorised without another Manager/user decision are regenerable generated directories such as `Library/`, `Temp/`, `obj/` or equivalent Unity/import caches when ALL of the following are true:
- the owning worktree is clean, including untracked files;
- no RUNNING claim owns that worktree or its Unity state;
- the source branch/commit is preserved;
- required verification evidence/captures/handoffs are already preserved outside that cache;
- deleting the cache cannot remove a unique user file, save, screenshot, log, handoff or source artifact.

Prefer completed/released historical worktrees' generated caches first.

Do **not** delete the current Q10 partial-import cache merely because it is large if retaining it is expected to reduce the next Q10 import cost. Classify it and leave it unless fresh inspection proves removal is the safer choice and another suitable retained Unity lane remains available.

Never delete or move:
- Git worktrees themselves;
- `.git` metadata, branches or commits;
- dirty/untracked files;
- current active candidate lanes (#129/#147/etc.);
- evidence directories under `/media/jer/Files/CCF/Verification/`;
- user saves, screenshots/captures, logs required by a handoff, or project source;
- anything outside CCF-related generated caches.

## Target
After each deletion, re-measure `/media/jer/ZX20` free bytes.

Stop cleanup as soon as at least **20 GiB free** is available. This is a Manager operating target to provide headroom above the mandatory 15 GiB Unity-lock floor; it is not a new gameplay/product requirement.

Do not launch Unity during this task.

## Handoff evidence
Publish `Docs/Handoffs/Unity-SSD-Headroom-Remediation-20261029/HANDOFF.md` containing:
- executor/model/provider;
- starting and final free bytes;
- inventory summary;
- exact paths removed and their sizes;
- proof each removed cache belonged to a clean inactive/released worktree;
- any skipped DIRTY/UNKNOWN candidates;
- final machine lock/process state;
- whether the 20 GiB target was met.

## Stop conditions
Stop and return to Manager if:
- every large candidate is active, dirty or ownership-uncertain;
- reaching 20 GiB would require deleting source/evidence/user files or a worktree;
- free-space accounting is ambiguous;
- another worker acquires Unity or changes a candidate while inventory is in progress.

## Success
`UNITY SSD HEADROOM RESTORED — UNITY QUEUE MAY RESUME`

## Blocked
`UNITY SSD HEADROOM REMEDIATION BLOCKED — MANAGER/USER STORAGE DECISION REQUIRED`

No Windows build.
