# CCF #895 — seven exact Files worktree conditional retirement: manager verdict and binding execution packet

**Date:** 2026-11-20 after 23:01 UTC.  
**Status:** **MANAGER APPROVED PER PATH / EXECUTION READY WITH HARD NATIVE SAFETY GATES / NOT RUNNING / ZERO DELETED.**  
**Authority:** the owner approved only read-only preflight on 2026-11-20 22:34 UTC (implemented under #893), then at 23:01 UTC presented the completed #893 7/7 PASS result and directly instructed the Overall Manager, “Next step is yours: approve or reject each exact path individually.” This document records the Manager's conditional per-path **APPROVE** decisions in response. Approval **does not** waive a fresh immediate final check, physical Ops/Git shared-registry authority or exact path identity; no runtime action is claimed. No other folder is approved.  
**Class:** PACKET ONLY — bounded retirement of seven clean historical worktree checkouts.  
**Difficulty:** MEDIUM–HIGH (registered shared Git store and potential active process/owner races).  
**Provider:** **one** native GPT-6.1 Sol/Codex local Linux Git metadata writer (or authorised equivalently capable worker). GitHub-only manager cannot execute removal.  
**Issue:** https://github.com/jermur788/CCF/issues/895. Historical research #892; preflight #893.  
**Physical host:** verified at source preflight `jer-laptop`, current mount expected `/media/jer/Files`, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`. Must be rechecked live. Shared Git metadata store **protected except normal Git side effect on seven approved worktree registry entries**: `/media/jer/Files/CCF/Repository.git`.

## 1. Verified evidence and confidence

- [#893 local preflight Markdown](https://github.com/jermur788/CCF/blob/docs/ccf-893-seven-files-preflight-results-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/893-seven-files-preflight/CCF-893-SEVEN-FILES-LOCAL-READONLY-PREFLIGHT-RESULT.md), fetched GitHub blob `8faaf1b66ee3094e281cf0775ec098a4b04478d5`.
- [#893 exact seven results TSV](https://github.com/jermur788/CCF/blob/docs/ccf-893-seven-files-preflight-results-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/893-seven-files-preflight/CCF-893-EXACT-SEVEN-PATH-PREFLIGHT-RESULTS.tsv), fetched blob `939e2d525e6d2c41c0dad46974403938ae6e020b`. GitHub base→source compare observed **only these two new files**, ahead3/behind0.
- Single-read local worker was DeepSeek V4.1 Flash via T3 Code/OpenCode. Its reports state **all seven** exact mount UUID, path/registered Git identities, branch/HEAD and allocated bytes match; 0 staged/unstaged/untracked/ignored; no Library, Build, Temp, UserSettings, saves, prefs, archives/backups/logs; no named live task/board claim, process/cwd/FD/mmap visible under user `jer`; no live Unity lease; exact GitHub commit objects exist; no local-only commits. No deletes/moves/archives happened. **Manager independently read and compared the GitHub report and row evidence, not the physical machine.**
- Historical originating issues #255, #256, #260, #269, #275, #277, #195, #205, #213 are all **CLOSED** as checked in GitHub; closure does **not** itself release physical ownership.
- The live `active-tasks.md` was refreshed in this Manager review (blob `e99380bd243b66b8fd0a4197dd5b5599aee7cab2`). #802 remains OPEN with its own one-orchestrator Unity-lock/obsolete-folder scope, and no overlap with these seven was reported.
- **Limitations** from worker: 281 root/kernel `/proc` FD entries unreadable; ignored-file tool behavior and new files/processes can change after the snapshot; no repository-wide stashes re-audited; origin remote was read via cached origin refs and GitHub API. No claim of future absence of consumers. A fresh targeted final check is mandatory.

## 2. Individually approved exact targets

**Manager disposition: CONDITIONAL APPROVE for each exact registered worktree root below, and nothing else.** Each removal MUST be skipped if current branch/full HEAD/owner/contents differ, not reinterpreted as a broader folder approval. The local worktree currently has one linked `.git` file; use only normal non-force Git removal, never manual directory deletion. The shared repository itself, remote Git branches/tags/objects and all other worktree entries are NOT removal targets.

| Inventory ID | Exact FULL local worktree directory | Expected full HEAD | Expected branch | Previously allocated bytes | Manager verdict |
|---|---|---|---|---:|---|
| `CCF-INV-0102` | `/media/jer/Files/CCF/Operations/CCF-MFM08Closure` | `911a8a34ffbaa2004697ffeabdfe21edcb82eb2d` | `verify/mfm-08-tier2-closure-v1` | 1,230,462,976 | **APPROVE / gated** |
| `CCF-INV-0103` | `/media/jer/Files/CCF/Operations/CCF-MFM08Qual` | `d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727` | `qual/mfm-08-unity-v1` | 1,231,458,304 | **APPROVE / gated** |
| `CCF-INV-0104` | `/media/jer/Files/CCF/Operations/CCF-MFM10Residual-20261101` | `43e263708d8f517337181c92d0d78ed442803950` | `verify/mfm-10-species-guard-v1` | 1,231,798,272 | **APPROVE / gated** |
| `CCF-INV-0128` | `/media/jer/Files/CCF/Operations/CCF-Overnight-FeedbackVerify-20261104` | `6efba8fdc35f22098faf205a5f7d14ccf65a3e1a` | `verify/overnight-playtest-feedback-20261104` | 1,232,261,120 | **APPROVE / gated** |
| `CCF-INV-0129` | `/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveCopy-20261104` | `693e24c8891637470052031d5023883b922e9334` | `feature/overnight-objective-copy-20261104` | 1,232,629,760 | **APPROVE / gated** |
| `CCF-INV-0131` | `/media/jer/Files/CCF/Operations/CCF-Overnight-RackHandle-20261104` | `9304d318f346b281a19cb805feebc6dc1a95008e` | `feature/overnight-rack-handle-20261104` | 1,232,502,784 | **APPROVE / gated** |
| `CCF-INV-0132` | `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104` | `d3ba4fce3994d5c59b51786c6671dd654791e7d1` | `integration/scenario-one-overnight-ui-qualification-v1` | 1,233,047,552 | **APPROVE / gated / SHA PIN** |

**Aggregate estimated maximum:** **8,624,160,768 bytes / ~8.62 decimal GB** on Files, from measured allocated bytes. It is not a guarantee of actual `df` free delta. Do not double-count shared Git objects.

**Special #0132 check:** Manager independently compared exact local `d3ba4fce3994d5c59b51786c6671dd654791e7d1` to remote `cddf3ed6`; GitHub shows **six newer commits, local is an ancestor**. Preserve/reconstruct the exact old tree via full SHA, **not by checking out the branch's current tip**. No deletion of the six newer commits or branch is authorised. The Files `CCF-Overnight-UIQualification-20261104` is **not** the protected ZX20 `CCF-UIQualification-20261104` with unconfirmed warm Library.

## 3. Required native admission and hard stops

1. **Claim one actual shared Git-worktree registry writer, not Unity:** refresh latest GitHub `Docs/Project/coordination/active-tasks.md` and current local CCF-Ops board/physical leases; confirm #802 orchestrator neither owns nor is writing the shared `Repository.git/worktrees` metadata, including process/claim checks. Actual lane = single native CCF #895 **Git metadata writer** with explicit local reservation/receipt. If registry writer cannot be safely serialised, **STOP ALL**. This issue/packet is not a physical lock. No Unity Editor/process launch or lock acquisition necessary. No unrelated source or private checkout writes.
2. **Confirm host/storage and exact identity immediately before each item:** `jer-laptop`, Files UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`, exact `realpath` equal to approved path, no symlink/nested mount, worktree registered once in `Repository.git`; verify expected `.git` backlink. Missing/different means **SKIP**, not prune/repair.
3. **Check actual Git status:** branch and full HEAD must match table; no staged, unstaged, untracked or ignored changes; no unique files, saves, prefs, licence files, asset downloads, packages, verification logs, independent binary archives, stashes/refs/reflog-only commits or untracked/nested submodules. Use `GIT_OPTIONAL_LOCKS=0` for read-only checks. Any mismatch = **SKIP**; never force/reset/clean.
4. **Check current ownership/process:** live tasks, active CLI jobs, Unity/editor/agent/file watchers with `cwd`, open file handles or mmap; attempt reliable process inspection sufficient to exclude plausible consumers of target path. The #893 281 unreadable root/kernel FD entries are a known confidence limit. If evidence suggests a relevant root-owned process or inspection cannot safely resolve an actual live-use risk, **SKIP/STOP** rather than assume none. No active owner or physical lease is allowed.
5. **Recovery:** confirm each exact full SHA accessible on GitHub and reachable from remote advertised ref, that no local-only history or relevant stash depends on it, and record safe reconstruction by **pinned SHA** to a different path. For #0132 explicitly verify old local HEAD's ancestor relationship and pin SHA, never branch name. No fetch/reset/branch/reflog pruning as a preparatory action.
6. **No destructive broad cleanup:** no `rm`, recursive shell delete, `--force`, glob/wildcard path, `git clean`, `git gc`, `git worktree prune`, manual `Repository.git/worktrees` edits, branch/tag/ref removal, unmount, permissions, source/scene/asset/save edits, archive, move, Unity or Windows build. Do not touch root T3, ZX20, #891 art, #890 onboarding, #894, shared Ops history or owner checkouts.

## 4. Permitted narrow execution

**Only** after all six gates pass **for that path**, and the actual exclusive Git registry lease is held, perform the normal registered-worktree removal:

```bash
git --git-dir=/media/jer/Files/CCF/Repository.git worktree remove "/media/jer/Files/CCF/Operations/<ONE EXACT APPROVED BASENAME>"
```

The placeholder is explanatory only; **do not execute with a placeholder or substitute broad parent**. One call per exact list item, without `--force`. Do not use Git's force flags even if it declines to remove. Record exit status, verify precise directory disappearance and matching worktree registration disappearance; ensure all other worktrees/refs remain intact. Capture per-item `df -B1` before/after and actual delta. If remove fails or state differs, **STOP on that path and keep it**, without manual deletion or pruning; safe non-conflicting items may proceed only if exclusive metadata control remains sound. If shared-store error or lease conflict, STOP entire batch.

Actual file-system effects are restricted to the seven approved checkout directories and the seven associated Git worktree registry entries that Git itself removes. **Source branches, remote commits, tags, Git objects and shared repository remain**. No Unity Library impact expected; do not infer product gameplay changes.

## 5. Result handoff and next status

At completion (including partial), publish one unique, GitHub readback-verified documentation result on an isolated branch from current main:

- `Docs/Handoffs/CCF-Storage-Inventory-20261120/895-seven-files-removal/CCF-895-EXACT-SEVEN-REGISTERED-WORKTREE-REMOVAL-RESULT.md`.
- `Docs/Handoffs/CCF-Storage-Inventory-20261120/895-seven-files-removal/CCF-895-SEVEN-ITEM-REMOVAL-RECEIPTS.tsv`.

Include real model/provider and local claim/lease receipt, exact host/UUID, source main/context, start/end, seven path identities/HEAD/branch, final preflight observations, actual per-path deletion YES/NO/WHY, Git exit codes, before/after bytes, verified Git registry remaining, any skipped/held item, proof of branches/objects preserved, release of exclusive local registry claim, zero Unity/main/source/asset/build changes. If partial, say **PARTIAL / NEEDS OWNER DECISION**, not PASS. Links to output must be placed in [#895](https://github.com/jermur788/CCF/issues/895), #893 and #425. Notify #802 only for actual contention/overlap. No Dashboard/Drive sync inferred.

**Phase labels:** #893 G0 preflight is complete; #895 is **CONDITIONALLY APPROVED, NOT STARTED** until physical registry lock/claims and fresh path-level safety checks. A local removal receipt proves directory disposition and free-space change, **not Unity gameplay acceptance**.

## 6. Complete paste-ready dispatch for the owner to launch one local worker

> **CCF #895 — EXACT-SEVEN FILES WORKTREE RETIREMENT, CONDITIONAL MANAGER APPROVAL / NATIVE SINGLE WRITER.** The owner delegated an individual yes/no decision after the approved read-only #893 inspection; Manager approves exactly seven IDs `0102,0103,0104,0128,0129,0131,0132` conditionally. **READ COMPLETE DIRECT GITHUB HANDOFF FIRST: https://github.com/jermur788/CCF/blob/docs/ccf-895-seven-worktree-exact-conditional-retirement-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/CCF-895-MANAGER-SEVEN-EXACT-WORKTREE-RETIREMENT-VERDICT-AND-EXECUTION-PACKET-20261120.md**. Provider GPT-6.1 Sol/Codex native on `jer-laptop` (or approved equivalent); difficulty MEDIUM–HIGH; phase READY, not claimed. Scope: exactly the seven fully qualified Files worktree roots/HEADs in the packet, shared Git `Repository.git/worktrees` mutation **only as non-force normal `git worktree remove` side effect**, no other writes to source/assets/saves/branches/refs/objects. Before starting, refresh live `active-tasks.md`, CCF-Ops physical registry, #802 separate orchestrator and agent/process ownership, and acquire exclusive native shared Git registry writer control; do NOT run with active #802 writer or unclear locks. Immediate final per-path check realpath/UUID, branch/full SHA, no staged/unstaged/untracked/**ignored** files, no user/unique evidence, no process/cwd/FD/mmap, remote SHA recovery. **#0132 local HEAD d3ba4fce is six commits behind remote tip; restore by exact SHA, never branch**. For each passed exact path, use `git worktree remove <exact path>` without force; skip every ambiguous, occupied or changed path. Never `rm -rf`, clean/prune/gc/reset/checkout, delete branches, alter main, or run Unity/build. Measure actual `df` delta; publish Markdown report and seven-row receipts TSV on new docs-only GitHub branch, read back, link #895/#893/#425, release registry ownership. Claims and execution must be real; this dispatch text does not start the worker. **Owner action:** launch one local worker for this exact packet; no additional folders are approved.
