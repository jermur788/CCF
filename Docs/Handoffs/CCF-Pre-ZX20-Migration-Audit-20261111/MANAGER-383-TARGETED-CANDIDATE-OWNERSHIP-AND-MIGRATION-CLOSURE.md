# #383 — Bounded READ-ONLY completion of the partial pre-ZX20 cleanup audit

**Manager verdict:** [partial measured audit accepted, but NO removal/migration approved](MANAGER-383-PARTIAL-MEASURED-AUDIT-DISPOSITION.md).  
**Execution claim:** [#383](https://github.com/jermur788/CCF/issues/383), OPEN / PARTIAL / NOT RUNNING (reuse same task; no new claim).  
**Purpose:** fill the few missing ownership, rebuild, backup and dependency facts that can change an *actual* cleanup/migration decision; not another unbounded full-disk scan.  
**Difficulty:** HIGH — three project authorities, active T3, local-only Git refs, exFAT allocation, user data and database consistency.  
**Best local worker:** OpenAI Sol/Codex High T3 Code, **one** read-only agent, no peer reviewer.  
**Permission:** metadata/read-only observation and **new audit report files only**. **No deletion, move, archive creation, backup transfer, database write, Unity run, reset, stash, Git prune/gc, credentials or integration.** T3 Fork Integration HOLD remains active.

## Authoritative starting evidence

- [Sol #383 partial report](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-INVENTORY-AND-MIGRATION-MANIFEST.md)
- [Current candidate TSV](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CLEANUP-MANIFEST.tsv)
- [Protected register](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-PROTECTED-REGISTER.md)
- [Volumes/mount observations](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-MOUNT-AND-STORAGE-SUMMARY.md)
- Live CCF `Docs/Project/coordination/active-tasks.md`, T3 Current State/Roadmap/Coordination/Handoff Protocol, active process registry and owner confirmation. The report says T3/B2.1 and dashboard have nontrivial dependencies: do not infer owners from absence of Unix processes.

**Baseline:** root `/` only 1,267,175,424 bytes available at 20:34 UTC; ZX20 29,587,668,992 available; Files 205,980,205,056. **These are occupancy, not reclaim.** Unknown totals must remain unknown when proof is missing. First 16 TSV rows were a sampled inventory, not complete per-worktree/backup verification.

## 0. Safe setup; no collision with personal U3 or active T3

Use **GitHub report publication** (previous #383 worker did so without a local worktree) if available, or a fresh isolated **docs-only** audit lane after actual live ownership/dirty preflight. Reuse the same #383 claim. Do not check out or rebase the dirty CCF Primary, or any active source/Unity/T3 workspace. Set #383 [RUNNING] with actual provider, report branch/commit and writer lane only once a report writer is truly admitted. Source code, machine and T3 runtime ownership remain with their current owners. No Unity machine lock requested.

Capture `findmnt`/`df` delta as read-only; if root space has declined, restrict discovery to quick targeted metadata and notify Manager about critical capacity without deleting files. Do not create caches or compute huge hashes on root.

## 1. Four targeted high-value questions in order

### A. ZX20: two most promising *non-personal* Unity Library caches

Audit **separately**:
- `/media/jer/ZX20/CCF-MainIntegration-20261104/Library`, measured 7,216,824,320 allocated;
- `/media/jer/ZX20/CCF-UIQualification-20261104/Library`, measured 7,210,270,720 allocated.

For each: current worktree branch/full HEAD, tracked/untracked/ignored parent state, real owner/last use/lease, dependencies on `Library`, exact Unity version/package/import/licensed assets and whether a *clean source reconstruction* is possible without removing unique data. **MainIntegration parent is DIRTY with eight files: never remove or alter parent.** `Library` might contain import state costly to regenerate. If actual owner cannot release or licences/source cannot be confirmed, stay UNCERTAIN. Do not scan every cache in U1/U3/#382: all remain PROTECTED as of this audit.

Measure apparent vs allocated at cache root and note exFAT allocation unit; if independent directory trees are physically separate and owned, a conditional upper byte range may be estimated with caveats, **not user-approved reclaim**. No subfolder removal or temp restart.

### B. CCF: builds, Git stores, worktree uniqueness and evidence

Review individual leaf directories within `/media/jer/Files/CCF/Builds` (3,053,748,224 allocated) and linked capture/verification provenance. Identify exact build receipts/versions, accepted tests that reference them, whether any are sole executable proof and copies in other stores. List top 10 or all if fewer. **Do not call any unpushed Git source or local-only object redundant**.

Inventory CCF Main Git object store and the separate Legacy Git store with **read-only** refs/worktree lists: each worktree actual HEAD, branch/detached, dirty/untracked/ignored status, whether source/data are reconstructed from preserved reachable refs; list especially candidates carrying unique changes or ignored licensed assets. Do not enumerate 1,855 “local-only commits” as 1,855 independent branches or assume remote backup. For any worktree candidate, record exact **rebuild proof and preservation plan** before suggesting removal; never delete a worktree or prune metadata.

The size of `/media/jer/Files/CCF/Repository.git` is not countable as savings from removing one worktree. Do not sum builds, verification parent archives and nested captures.

### C. System root: T3 package dependencies and live runtime

Root free space is critically low. Record only **nonintrusive** T3 owner/lock and dependency/rebuild metadata for `/home/jer/dev/t3code/node_modules` (~3,831,660,544 allocated), any linked pnpm/cache locations and installed toolchain/lockfile. Confirm whether actual dependency files hardlink to a shared store or are separate, whether a full offline reinstall is possible, and whether any active process uses them. No removing dependencies.

Live `/home/jer/.t3/userdata` (~5,842,542,592 allocated) **PROTECTED**: identify current dashboard/T3 owner, whether log retention or browser files are truly disposable, and required database/WAL/sessions/identity preservation and consistent backup protocol from Current State. No live DB read that requires a write lock, no compact/checkpoint, no state mutations, no revealing tokens or credentials. Logs (~747,188,224) and server-browser (~200,589,312) are **children** of `userdata`; never sum as independent reclaim. If owner unavailable, record **OWNER APPROVAL PENDING**, not “rebuildable”.

### D. Migration destination and cross-project backup dependencies

Record actual destination **Windows PC drive capacity / filesystem and Windows-vs-WSL execution choice** only from verifiable available owner sources. If not accessible, label **NOT OBSERVED — USER CAPACITY DATA REQUIRED** rather than assuming available space. Determine whether ZX20 is intended as transfer medium, permanent home or single interim backup; existing free space does not establish final backup capacity. Map dashboard source/ingestion owner, T3 runtime SQLite/WAL consistency and any shared Git refs, licence and configuration/secret handback to the proposed migration. Full T3 Fork Integration HOLD stays binding; do not start migration.

## 2. Candidate-grade evidence and estimate policy

For each of the above candidates record exact path, size timestamp, allocated/apparent bytes, volume, unique inode/link status if meaningful, parent-child overlap, current owner/lease, local-only/ignored data, active dependency, rebuilding from retained source/licences, independent validated backup status, migration impact, retention class, proposed next user approval, rollback and confidence.

Use five original classes: `REGENERABLE — LOW RISK`, `LIKELY OBSOLETE — USER APPROVAL REQUIRED`, `ARCHIVE BEFORE REMOVAL`, `PROTECTED — RETAIN`, `UNCERTAIN — INVESTIGATE`. Keep all previous measurements and add new rows **without overwriting any original report**. Every positive savings estimate must be **conditional** on actual owner release, reimport/reinstall feasibility, no unique data, and separate user cleanup approval. Distinguish allocated space possibly freed on ZX20 from root bytes possibly freed on system volume and from future migration copy volume. If no candidate meets evidence, **UNKNOWN** remains the correct conclusion.

No destructive reconstruction test is allowed. Worktree reconstructibility may be proven by remote/full ref/object availability and an explicit **untested replay recipe**; if proof cannot include ignored/untracked user files, classify ARCHIVE/PROTECTED. For live DB, plans must include an owner-approved *future* quiescent consistent backup and restore test.

## 3. Budget and stop

One limited metadata pass on these four targets, not another entire disk scan. Don't hash all of ZX20 or recursively inspect every git object. Avoid repeated T3 process refreshes and full-file hashing; prefer existing receipts. If an item cannot be resolved without user/owner approval, stop it at UNCERTAIN with exact missing permission.

**Early stop:** if any unexpected active owner, incorrect drive, read-only/open permission failure, low root space risk or current T3 HOLD uncertainty would make inspection intrusive, stop that item and report BLOCKED/PARTIAL without touching its data. Do not start any Unity or destructive operation to prove rebuildability.

## 4. Exact publication and Manager return

Publish **new additive docs only** on the existing docs-only audit report branch `docs/ccf-pre-zx20-storage-migration-audit-20261111-v1`, either by safe GitHub API or isolated report writer:

- `Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-TARGETED-OWNER-AND-RECOVERY-ADDENDUM.md` — measured results, candidate decisions and missing owner/user approval;
- `Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-TARGETED-CANDIDATE-ADDENDUM.tsv` — exact paths, evidence, byte ranges, class, rollback and signature;
- short migration capacity/dependency blockers in the addendum, avoiding credential/path-secret exposure.

Do not replace or edit the original four Sol files. Return direct links to [Manager #200](https://github.com/jermur788/CCF/issues/200) and [#383](https://github.com/jermur788/CCF/issues/383). Mark claim **HANDED-OFF/CLOSED** only if user-relevant ownership/rebuild/backup decisions are adequately evidenced and report writer released; otherwise **PARTIAL OPEN / NOT RUNNING** with exact missing permissions and no speculative deletion approvals.

**User action:** none needed for *this read-only investigation* while access remains safe; for a final Windows migration capacity calculation the user may later need to supply Windows destination capacity and approve a backup location, and exact source owners may need to release particular lanes. **No cleanup or migration action is part of this handoff.**

## ONE COPY-PASTE DISPATCH

> Sol / T3 Code (Codex High): Continue existing #383 read-only Pre-ZX20 cleanup audit using Manager's targeted packet at https://github.com/jermur788/CCF/blob/docs/manager-383-partial-audit-review-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-TARGETED-CANDIDATE-OWNERSHIP-AND-MIGRATION-CLOSURE.md . Existing four audit artifacts are accepted as a PARTIAL baseline; **recoverable bytes UNKNOWN / no deletion approved**. Inspect only the two non-personal ZX20 Unity Libraries, CCF Builds/Git worktree reconstructibility, root T3 node_modules/live runtime owner/dependencies and Windows/dashboard migration backup capacity, with live lock/owner safety and no full scans. Distinguish unique allocated bytes from approved reclaim and preserve dirty MainIntegration, personal U3/#382, licensed/reference assets, all Git refs, live T3 SQLite/WAL. Publish an additive measured GitHub Markdown/TSV addendum and return Manager #200. **NO deletion, movement, backup transfer, migration, Unity, database writes, cleanup, stash/reset/prune, credential exposure or second reviewer; T3 Integration HOLD stays in force.**
