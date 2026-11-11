# #383 — Manager continuation after incomplete read-only storage preflight

**Date:** 11 November 2026  
**Status:** INCOMPLETE / READY TO RESUME, NOT RUNNING; **no cleanup action authorised**.  
**Manager:** #200. **Worker:** OpenAI Sol/Codex local T3 Code, same agent if available; no additional reviewer.  
**Difficulty:** HIGH — cross-volume and cross-project audit with live T3 operations, unique evidence and protected personal work.  
**Binding full-scope packet:** [Pre-ZX20 storage cleanup and migration audit](MANAGER-383-READ-ONLY-STORAGE-AND-MIGRATION-CLEANUP-AUDIT.md). This continuation **adds preflight findings and solves a report-branch availability problem**, without weakening any of that packet's safety rules.

## 1. What has actually been reported (not an audit result)

The prior local operator reported a **read-only preflight only** and no published manifest or report. These are **operator observations**, not manager-measured filesystem facts:

| Item | Observed | Implication / qualification |
|---|---|---|
| ZX20 | `/media/jer/ZX20`, exFAT on `/dev/sdd2`; ~477 GiB total, ~450 GiB used, ~28 GiB available | Very constrained migration staging; **zero defensible recoverable-space estimate so far**. Values are time-specific and require remeasurement. Do not automatically conclude all 450 GiB is project-owned. |
| Files | mounted ext4 on `/dev/sdb4`; ~465 GiB total, ~249 GiB used, ~192 GiB available | Available space and used space need not sum to total on ext4 (reserved blocks/metadata). Do not treat the difference as reclaimable. |
| CCF Primary | `/media/jer/Files/CCF-Primary/AGENTS.md` and `Docs/Operations/StorageArchitecture.md` already modified | **PROTECTED DIRTY; do not checkout, restore, stash, reset, clean, edit, copy over or use Primary as the report writer**. Read relevant storage architecture carefully; report actual ownership. |
| Git worktrees | Numerous registered CCF worktrees on Files, ZX20 and home | No obsolete verdict without per-worktree source ref, dirty/untracked/ignored, ownership/process and independent reconstruction evidence. |
| T3 | T3 processes active | Respect **T3 Fork Integration HOLD** and actual owners/leases; no lock release inferred from silent/nonrunning workers. |
| Report branch | Worker could not find the prepared docs-only branch in local refs | **Manager independently verified that the named branch and binding packet exist on GitHub.** The absence of a *local tracking ref* is not a reason to modify Primary's working tree or abandon inventory. |
| Completion | No report, measured per-path sizes, protected register, space estimates, GitHub handoff | **AUDIT NOT COMPLETE**. Do not mark #383 HANDED-OFF or publish “cleanup ready”. |

Current [#383 issue](https://github.com/jermur788/CCF/issues/383) is the single execution claim; **no new task or paid review**. The next phase is **read-only inventory and original-report publication**, not cleanup or transfer.

## 2. Safe route around the local report-branch problem

The GitHub documentation branch **`docs/ccf-pre-zx20-storage-migration-audit-20261111-v1`** exists on remote GitHub (read back by Manager). First confirm using a **read-only remote query** such as `git ls-remote --heads origin 'refs/heads/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1'` from a known CCF repository or an authenticated GitHub API call. Do not assume the local refs must already have been fetched.

The local worker is authorised to **write only newly created report/manifest files** in a clean, separately owned audit-output lane. Choose one safe route based on actual tools:
- Prefer a new, lightweight **isolated docs-only worktree or clone** at proposed `/media/jer/Files/CCF/Operations/CCF-Pre-ZX20-Storage-Audit-Sol`, solely after checking that the path is absent/safely empty and no owner is registered. A *targeted, non-force fetch* of just the remote report branch, when required, is allowed **as report-workspace setup only**; it must not checkout/merge/reset any active/dirty repository, change production source, or fetch over a valued local ref. Never copy a large working Unity project merely to get a docs branch.
- If authenticated GitHub contents/API publication is actually available to the local worker, it may **publish report files directly on the pre-existing docs branch** and avoid a new local checkout. Preserve concurrent edits with normal expected-version checks. Do not assert API access until verified.
- If neither route is safe, conduct non-destructive **read-only inventory** and report this as a specific **REPORT PUBLICATION BLOCKER**; do not misuse CCF Primary or another active worktree. Ask Manager for a safer reports-only publication route instead of abandoning the audit due solely to a missing local branch.

Refreshing a remote tracking ref in a safe, owned reports-only repository is a limited report setup action, **not permission to alter active Primary's Git index/worktree**. Keep generated audit output out of all measured storage totals or explicitly deduct it; record newly-created report-lane bytes so the baseline is auditable.

Before real audit execution, refresh live `Docs/Project/coordination/active-tasks.md`, #383 and T3/dashboard ownership. Register **report writer only** in that isolated lane and mark #383 [RUNNING] with actual model/provider, branch, worktree and starting commit. For metadata of active workspaces, remain read-only regardless of issue title.

## 3. Finish the missing measurements in a bounded sequence

**A. Volumes and owned roots:** Verify current mounts/UUIDs/filesystems/space (`findmnt`, `lsblk -f`, `df -hT`) without mounting or fixing anything. Locate CCF, T3 Fork, dashboard/Overall-Dev and shared project roots from actual current ownership documents and process references. Mark inaccessible/unmounted roots **UNKNOWN**, not zero. Limit scans to verified project-owned roots and avoid following symlinks into unrelated data.

**B. High-value, low-impact size census:** Use targeted `du -x` and `stat`/top-level directory counts to measure allocated vs apparent sizes, grouped by **physical volume**. Prioritise the largest Unity `Library/Temp/obj`, standalone builds, archived verification captures, disposable worktrees, package caches, T3 runtime outputs, and dashboard data. Measure heavy subtrees only when their owner and mount are confirmed; protect live processes. Do not run costly whole-drive hashing.

**C. Git and active dependency audit:** For each proposed duplicate/stale Git worktree, collect read-only refs/branch/full HEAD, tracked+untracked+ignored status, local-only or unpushed commits, object-store sharing, owner/lock, unique licensed binaries, saves and evidence. Demonstrate theoretical reconstruction from preserved refs/objects/remote **without actually removing or recreating** it. Worktrees with uncertainty are **ARCHIVE BEFORE REMOVAL / UNCERTAIN**, never unconditionally disposable.

**D. T3 Fork and dashboard:** Inspect their project-specific latest Current State/Roadmap, Coordination/Handoff Protocol, runtime/evidence ledgers and **Integration HOLD**. Active processes and databases are **PROTECTED**. Distinguish caches from authoritative source/runtime/ledger/DB and Windows migration prerequisites. Do not touch other project files or operational locks.

**E. Duplicate-aware manifest:** Fill the five classification bins and exact-path manifest required in the original packet. Do not double-count parent+child, symlink targets, hardlinks, shared Git object stores, reflinks, compressed copies or data stored on different volumes. Record **low/high unique reclaimable byte estimates only after measurements**; confidence `HIGH / MEDIUM / LOW / UNKNOWN`; provide per-volume conservative and possible reclaim. Uncertain/protected categories contribute **zero to committed cleanup savings** until proof/user approval.

**F. Backup & Windows migration:** Enumerate mandatory INCLUDE (canonical Git history and all local-only commits, source/`.meta`, ignored licensed and unique assets, user saves/preferences, frozen Reference, approved specs/decision logs, unique verification evidence, T3 runtime data, dashboard source/schema/state, secure credential recovery plan); REBUILD, ARCHIVE, CONDITIONAL EXCLUDE. Include documented rollback and owner signoff per proposed item. **No backup copy, migration or transfer yet.**

Suggested **stop budget**: one read-only mount/owner pass, one coarse per-root size census, then targeted deeper inspection of the 10–20 largest candidate storage roots. Do not spend a whole local session proving tiny cache rows while the destination has only roughly 28 GiB free. If scope is too broad, publish a **PARTIAL with measured coverage and explicit unknowns**, not a false complete/recoverable figure.

## 4. Deliverables and admission/stop rules

**Primary report** on docs-only branch:  
`Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-INVENTORY-AND-MIGRATION-MANIFEST.md`.

**Adjacent outputs:** structured TSV/CSV, mount-and-space summary, protected-file register, Windows include/exclude/rebuild schedule, rollback and exact user approval list. All actions are **PROPOSED ONLY / NOT EXECUTED**. If no measured reclaim can be supported, report `UNKNOWN — insufficient inventory`, not a fabricated range.

**Nonnegotiable restrictions:** no deletion, file moves, overwrites, reset/stash, clean/gc/worktree prune, history rewrite, modifying Primary's two pre-existing dirty files, T3/database/dashboard edits, integration, credential disclosure, Unity, backup transfer/migration, active-owner interference or removal of unique historical evidence. T3 Fork Integration HOLD stays in force. Original personal Scenario One U3 and #382 Editor test remain protected.

At finish, commit/publish only docs audit outputs, release report ownership, mark #383 **[HANDED-OFF]** and close only for a **substantive completed measured report**; if partial, say PARTIAL and let Manager decide whether to continue or close. Return a direct GitHub report URL to [Manager #200](https://github.com/jermur788/CCF/issues/200). Manager reviews before any user approval request for destruction.

**Single copy/paste continuation:**

> Sol / T3 Code (Codex High): Resume the **INCOMPLETE, READ-ONLY** pre-ZX20 CCF/T3 Fork/dashboard cleanup audit under the existing #383 claim using this Manager continuation: https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-CONTINUATION-AFTER-INCOMPLETE-PREFLIGHT-20261111.md . The remote docs-only branch exists on GitHub; its absence from local refs does not authorise checkout/reset of dirty Primary. Use a safe isolated report-only lane or verified GitHub API publication, read live owners and active T3 HOLD, then measure real project-owned roots/per-path sizes, unique evidence, Git worktrees/unpushed commits, dependencies and non-double-counted reclaim by volume. Publish exact Markdown/TSV manifest, protected register, migration INCLUDE/REBUILD/ARCHIVE and rollback plan. **No deletion, migration, backup transfer, stash, reset, Unity, database write or peer review.** If blocked, publish factual partial coverage rather than an invented recoverable estimate and return to Manager #200.
