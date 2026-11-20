# CCF — Read-only inventory of all laptop drives

**Status:** COMPLETE for mounted volumes / PARTIAL overall (offline and unmounted media not inspected). **Cleanup approved: ZERO bytes.**
**Date/time of measurement:** 2026-11-20, 20:40–21:35 UTC.
**Worker:** T3 Code / OpenCode, `opencode-go/deepseek-v4.1-flash`, native local session on `jer-laptop` (Linux 7.0.0-42-generic, x86_64).
**Authority:** DeepSeek inspection handoff `CCF-ALL-DRIVES-READONLY-DEEPSEEK-INVENTORY-HANDOFF-20261120`; registered RUNNING on Manager #425 at 20:47Z.
**Task class:** PACKET ONLY, read-only storage audit. No Unity, source, Git-worktree or cleanup lane granted.
**Companion register:** `CCF-ALL-DRIVES-INVENTORY-REGISTER-20261120.tsv` (306 itemised rows).

---

## 1. Answer to the owner's question

| Question | Answer |
|---|---|
| Which folders are in use? | 8 active lanes/worktrees, 10 protected historical/personal copies, `CCF-Ops`, the shared Git store, `CCF-Primary`, and roughly 20 worktrees tied to currently open issues. Detailed rows in §6 and the register. |
| Which may be redundant? | 224 of 284 registered worktrees are completely clean, and 276 have their HEAD commit already inside origin history. 91 of those are also older than 13 November and referenced by no open issue: **114.2 GB upper bound**. |
| How much could eventually be recovered? | **About 75 GB is strongly evidenced** (30.2 GB of clean, pushed, unclaimed old worktrees plus 22.2 GB of standalone builds that need archiving, plus 4 GB of duplicated legacy Git store, plus ~8 GB of root-side worktrees, plus ~5 GB Unity scratch). **Up to ~180 GB** if evidence compaction and T3-tooling decisions also go in favour. Nothing is approved for deletion today. |
| Where is the pressure? | **Files is the critical volume: 95 % used, 24.3 GB free.** Root is 90 % used, 24.8 GB free. ZX20 and Videos have 281 GB and 436 GB free respectively, so recovering space there is housekeeping, not relief. |

The dominant cause is not a few old projects: it is **checkout multiplication**. Every CCF worktree carries a complete copy of a roughly 1.24 GB Unity source tree, and 240 worktrees are currently registered plus 45 in the legacy store. On Files alone, `CCF/Operations` holds 143 registered worktrees (167.2 GB) and `CCF/Verification` holds evidence that itself embeds dozens of full source copies (120.3 GB).

---

## 2. Scope, method and limits

**Verified before any inspection**

- Host is the intended laptop (`jer-laptop`, user `jer`), not a remote or WSL host.
- Four user-data filesystems mounted read-write by the user; no attempt was made to mount anything.
- Live coordination refreshed from the coordination anchor `ops/workflow-foundation-v1`; `main` observed at `c94f2a2b0dee439cbe2a1269fc47d7421a3e1be2` (20:07Z).
- No Unity Editor process at measurement start; Unity Hub (user-owned flatpak) was already open.
- No Unity lock held: last lease event was a `Codex-886-native` release at 20:35:36Z.
- Prior work reused: #383 audit, its Manager ruling, its residual TSV and #385 execution result, plus the two untracked 2026-11-18 storage-recovery notes already present in `CCF-Primary`.

**Method**

- `findmnt`, `lsblk -f`, `df -B1` for device identity and capacity.
- `du -x -s -B1` (allocated blocks) for size, bounded per volume and per candidate root; symlinks not followed, nested mounts not crossed.
- `git worktree list --porcelain` on the canonical and legacy stores; `git status --porcelain` per worktree with `GIT_OPTIONAL_LOCKS=0`; `git for-each-ref` / `rev-list` for ref and reachability comparison; GitHub REST API (read-only) for branch-head existence.
- No `fetch`, `checkout`, `reset`, `stash`, `clean`, `gc`, `prune`, `worktree remove`, no Unity launch, no writes into any audited folder.

**Limits**

- Allocated bytes are filesystem blocks, not exclusive bytes. Shared Git objects are counted once per checkout only as working-tree files.
- `atime` may be updated by reads; physical zero-write cannot be promised.
- Ignored files were not enumerated per worktree. Instead, Unity `Library/` presence was tested separately: **no `Library` directory exists anywhere under `/media/jer/Files/CCF` (max depth 4)**, and 17 warm Libraries exist on ZX20.
- Remote backup evidence uses the local `refs/remotes/origin/*` set. That set can be stale if the last fetch is old; branch-head API checks were used to confirm the disputed cases.
- Unmounted/offline media (including the separate "HP" workstation path `/home/jer/Development/CCF-HP-Worktrees/...` referenced by the Multi 2D handoff) is **NOT INSPECTED**.

---

## 3. Drive-by-drive table

Measured 2026-11-20 20:47Z.

| Mount | Device | UUID | FS | Capacity | Used | Free | Use % | CCF-related content | Coverage |
|---|---|---|---|---|---|---|---|---|---|
| `/` | `/dev/sda2` | `d393b49a-24aa-4f88-b8c4-5121b96c240d` | ext4 | 250.4 GB | 212.8 GB | **24.78 GB** | 90 % | 33.3 GB worktrees + 0.9 GB builds/quals ≈ **34.2 GB** | Full metadata pass of `/home/jer`; system dirs excluded as non-CCF |
| `/media/jer/Files` | `/dev/sdb4` | `2283b135-bf37-42d6-921f-2fb861d0a8d4` | ext4 | 498.7 GB | 449.0 GB | **24.27 GB** | 95 % | **430.7 GB** (CCF tree 353.7 + legacy estate 60.1 + T3 16.9) | Full top-level `du`, per-worktree `du`, targeted sub-tree `du` |
| `/media/jer/Videos` | `/dev/sdb1` | `60e276b9-4abb-4325-a913-f4c43c14294b` | ext4 | 467.3 GB | 7.9 GB | 435.6 GB | 2 % | **7.45 GB** (6 CCF worktrees) | Full |
| `/media/jer/ZX20` | `/dev/sdc2` | `18D6-896E` | exFAT | 512.1 GB | 231.3 GB | 280.8 GB | 46 % | **206.5 GB** CCF + 19.0 GB Unity Editor | Full top-level `du`; all 42 CCF roots measured |

No fifth user-data volume is mounted. `/dev/sdd` has 0 B and `sr0` is optical. **Disconnected drives are NOT INSPECTED.**

> **Record-fidelity defect (pre-existing, not repaired):** `Docs/Operations/StorageArchitecture.md` and `CCF-Ops/lanes.json` still pin older `/dev` names (`ZX20 = /dev/sda2`, `Files = /dev/sdc4`). The live mapping is `sda2` = root, `sdb4` = Files, `sdc2` = ZX20. `active-tasks.md` already records this as blocking the Unity lock tool. This report uses UUIDs, not device names.

---

## 4. Where the 430 GB of CCF content on Files actually is

| Path | Allocated | Share of Files | What it is |
|---|---:|---:|---|
| `CCF/Operations` | 187.0 GB | 42 % | 143 registered worktrees (167.2 GB) + 27 task containers (19.8 GB) |
| `CCF/Verification` | 120.3 GB | 27 % | 619 evidence directories, policy-protected |
| Top-level `CCF-*` (Legacy store) | 60.1 GB | 13 % | 45 legacy worktrees incl. 19 embedded player builds (19.1 GB) |
| `CCF/Worktrees` | 21.4 GB | 5 % | 11 registered worktrees (13.9 GB) + T3 evidence subdirectories (5.0 GB) |
| `CCF/Orchestration` | 12.4 GB | 3 % | 10 registered orchestration worktrees |
| `CCF/Archives` + `Reporting` + `Builds` + `Media` + `worktrees` | 14.6 GB | 3 % | Archives, 3 reporting worktrees, 3.1 GB builds, 2.6 GB media |
| `CCF/Repository.git` | 4.5 GB | 1 % | Shared bare store, 240 worktrees, 32 pack files |
| `CCF-Legacy` (Files root) | 5.5 GB | 1 % | Legacy store working tree + 4.0 GB `.git`; see §7 |
| **T3-adjacent on Files** (`Files/Worktrees` 9.5 GB, `t3-migration-stage` 7.35 GB, `Documents/CCF Game` 0.18 GB) | 17.0 GB | 4 % | T3 Fork evidence/trial checkouts and a staged migration backup; not CCF game source |

Single largest directory on the drive: `CCF/Verification/SharedSaveT03A-571-Review-20261116` at **34.6 GB**, of which 30.6 GB is a mutation-test matrix (`mut/`) containing repeated full 1.24 GB source copies.

---

## 5. What is protected, and why

| Protected | Reason | Evidence |
|---|---|---|
| `/media/jer/ZX20/CCF-U1` | Named Unity lane U1; 8 known orphaned Editor artifacts | active-tasks.md; direct status |
| `/media/jer/ZX20/CCF-U2` | Named Unity lane U2; **HEAD `e26516ff` is not on any origin ref** | active-tasks.md; reachability test |
| `/media/jer/ZX20/CCF-U3` | #361 personal playtest (older Scenario One) | #383 ruling; open issue #361 |
| `/media/jer/ZX20/CCF-Integration` | Named integration lane; untracked `Scenario1V1/` expressly preserved | active-tasks.md |
| `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` | #382 personal UI playtest | #383 ruling; open issue #382 |
| `/media/jer/Files/CCF-Primary` | Coordination anchor, live T3 workspace, 1 unpushed local commit `a42eaad` | direct status |
| `/media/jer/Files/CCF/Repository.git` | Canonical shared store for all 240 worktrees | direct inspection |
| `/media/jer/ZX20/CCF-Ops` | Live machine-local lock/board state | direct inspection |
| `CCF/Verification`, `CCF/Archives`, `CCF/Media` | Evidence, licensed/recorded assets, durable policy | StorageArchitecture.md |
| `CCF-Legacy` store | Contains 1 unique stash and 1 unique annotated tag | §7 |
| Open-issue worktrees (#857, #858, #862, #873, #876, #886, #813, #820, …) | Live work in the current week | open-issue cross-reference |

The two older ZX20 Libraries (#383 "Z1/Z2") remain **owner-unconfirmed, not cleanup candidates**. New direct evidence strengthens the hold: the running Unity Hub's own last-opened list contains `CCF-MainIntegration-20261104` and `CCF-UIQualification-20261104`, so they are still one click from the user.

---

## 6. Worktree estate — the core finding

**240 worktrees are registered in the canonical store; 45 more are registered in the legacy store. All 240 main-store paths exist on disk; one legacy registration is stale.**

| Location | Worktrees | Allocated | Notes |
|---|---:|---:|---|
| `/media/jer/Files/CCF/Operations` | 143 | 167.2 GB | One per task; ~1.24 GB each |
| `/media/jer/ZX20` | 36 | 194.8 GB | 17 carry warm `Library/` (~7.2 GB each) |
| `/home/jer/.t3/worktrees/CCF-Primary` | 24 | 29.6 GB | Root volume; T3 session worktrees |
| `/media/jer/Files/CCF/Worktrees` | 11 | 13.9 GB | |
| `/media/jer/Files/CCF/Orchestration` | 10 | 12.4 GB | |
| `/media/jer/Videos` | 6 | 7.4 GB | Created 19 Nov; volume has 436 GB free |
| `/media/jer/Files/CCF/Reporting` | 3 | 3.8 GB | |
| `/home/jer/ccf-worktrees` | 3 | 3.8 GB | |
| `/media/jer/Files/CCF/worktrees` | 1 | 1.2 GB | |
| `/media/jer/Files` root | 2 | 1.2 GB | `CCF-Primary`, `CCF-OrchestrationPolicy` |
| Legacy top-level `CCF-*` | 45 (+1 stale) | 59.0 GB | Second store, see §7 |

**Health of the estate**

- **Main store: 224 of 239** worktrees have zero tracked and zero untracked changes; **15** have working-tree changes. Eight of those carry the same known orphaned Editor artifacts (`.vscode/settings.json` plus seven SectionFive materials) already documented in `active-tasks.md`.
- **Legacy store: 40 of 44** existing worktrees are clean. Four have changes: the `CCF-Legacy` working tree itself (25 modified / 24 untracked), `CCF-a1-density-e5cfce3` (12 modified / 2 untracked — expressly named in `active-tasks.md` as preserve-and-do-not-clean), `CCF-tier2-smoke-bc81627` (10 modified) and `CCF-player-performance-profiling` (1 untracked).
- **276 of 284** main-store and legacy rows have their HEAD already reachable from an `origin/*` ref, i.e. the committed content exists in pushed history.
- **Only 4 main-store worktree HEADs are local-only:** `CCF-Primary` (`a42eaad`, known unpushed ops doc), `CCF-U2` (`e26516f`, the Scenario Two candidate), `CCF-D072-CombinedReview-20261106` (`531ff3e`), and the T3 blank-starter review worktree (same `a42eaad`).
- **19 worktree branches have no same-named GitHub branch**, but for 16 of them the commits are still reachable from other pushed refs; this is ordinary post-merge branch tidying, not data loss.
- **1 stale registration:** `/home/jer/Documents/ChatGPT/Local Game Dev/CCF-context-save-hardening-publication` (directory absent, marked `prunable` by Git). Metadata only.

**Why this matters:** a 1.24 GB clean worktree whose branch is pushed and whose task is closed is fully reconstructible with `git worktree add <path> <branch>` at the recorded HEAD. Its removal returns 1.24 GB immediately. The reimport of a Unity `Library/` is the only expensive part, and that applies to 17 ZX20 lanes only.

---

## 7. Duplicate and related-version map

### 7.1 Canonical store vs legacy store

The legacy store `CCF-Legacy` (121 branches, 287 refs, 1,602 reachable commits) was compared object-by-object with the canonical store:

- **Every legacy ref tip exists as an object in the canonical store.**
- **285 of 287 legacy refs are reachable from canonical refs.**
- **Two are genuinely unique:** `refs/stash` = `7b1c808d` ("Survival WIP preserved before Sitka migration", 2026-09-15) and the annotated tag object `871e9591` for `prototype-baseline` (its target commit `db22b092` *is* in the canonical store).
- The three legacy branch names absent from the canonical store all exist on GitHub at the same SHA.

**Conclusion:** the legacy store is *not* a separate body of unique source work — but it is the **only local holder of a real stash and an annotated tag**. Preserve the store until those two refs are preserved elsewhere; after that, its 4.0 GB `.git` is a redundancy candidate.

### 7.2 Legacy worktrees versus current worktrees

The 45 legacy worktrees are older checkouts (October, ~1.2 GB each, 114 MB for the early `CCF-serial-sol`/`CCF-overnight-sol` lanes). 42 of 45 HEADs are inside origin history; 40 are clean. Three have local-only HEADs:

- `CCF-verify-final-density-a395e3f` @ `4c00dd89` — explicitly named in `active-tasks.md` as a preserved evidence ref.
- `CCF-f10-triage-v1` @ `5d69b7bf` — the triage input SHA named in `active-tasks.md`.
- `CCF-perf-dense-stand-v1` @ `e788b09f` — local-only perf commit **and** a 9.18 GB Build directory.

These three are the only legacy worktrees that carry information not already in pushed history; all three stay protected.

### 7.3 Version relationships that are *not* duplicates

- U3 (#361) is an ancestor of #382's HEAD (5 commits behind). Ancestry is not equivalence: U3 has local save/preferences exposure and is a personal playtest. Both remain protected.
- `CCF-UIQualification-20261104` is a different, older lane from the #382 personal qualifier.
- `CCF-MainIntegration-20261104` is `main` at `9a6ed000`, 300 commits behind origin at the time of #383. It is a stale main checkout, not a copy of the current branch.

### 7.4 Evidence and builds

- `CCF/Verification` (120.3 GB) is evidence, not source. However, several harnesses embed whole project copies: the 34.6 GB mutation-test directory alone contains ~25 full checkouts (30.6 GB in `mut/`) plus the scripts, logs and receipts that are the actual witness. The reproducible checkouts are the only defensible reduction there, and only after an evidence-retention review.
- **19 embedded `Build/` directories total 19.1 GB** (largest: 9.18 GB perf-dense, 3.74 GB player profiling, 2.31 GB Stage A Windows pilot) plus 3.05 GB in `CCF/Builds`. These are standalone binaries; #383 established that the S1B binary on disk is *not* the one cited by the smoke summaries and has no independent archive. Treat as **archive before any consideration**, not as duplicates.

---

## 8. Storage ranking per disk

**Files (24.3 GB free — act here first)**

| Rank | Item | Size | Recovery posture |
|---|---|---:|---|
| 1 | Clean, pushed, unclaimed worktrees older than 13 Nov (77 paths) | **84.8 GB** | Review queue, per-path proof |
| 2 | `CCF/Verification` embedded source copies | ~30 GB (of 120.3 GB) | Evidence review; not a deletion proposal |
| 3 | Embedded Build outputs in legacy worktrees + `CCF/Builds` | 22.2 GB | Archive required |
| 4 | Legacy store `.git` (after stash/tag preservation) | 4.0 GB | Redundancy candidate with a precondition |
| 5 | T3 evidence/trial + migration stage | 16.9 GB | Separate T3 programme |

**Root (24.8 GB free)**

| Rank | Item | Size | Recovery posture |
|---|---|---:|---|
| 1 | T3 `t3code` worktrees (not CCF game source) | 21.7 GB | Already partially cleaned on 18 Nov; T3 owner decision |
| 2 | `~/.t3/worktrees/CCF-Primary` (24 sessions) | 29.6 GB | 7 clean/pushed/unclaimed older than 13 Nov = 8.6 GB |
| 3 | `/home/jer/dev` T3 stage checkouts | 19.7 GB | T3 owner decision |
| 4 | `~/ccf-worktrees` | 3.8 GB | Hold; recent art/review lanes |
| 5 | Three standalone Linux player builds (`ccf-*-player`) | 0.39 GB | Archive |

**ZX20 (280.8 GB free — housekeeping only)**

| Rank | Item | Size | Recovery posture |
|---|---|---:|---|
| 1 | 17 warm `Library/` caches | ~122 GB (implied) | Reimport cost; only old lanes |
| 2 | 7 clean/pushed/unclaimed old worktrees incl. ResearchCopy 9.37 GB | 20.8 GB | Review queue |
| 3 | `CCF-Temp` (Unity temp, historical Ops copies) | 5.35 GB | Separate #802 lane |
| 4 | 19.0 GB Unity Editor | 19.0 GB | Required tooling, retain |

**Videos (435.6 GB free)** — six recent worktrees (7.4 GB) for issues #792/#802/#809/#813/#815. No space pressure; do not treat as a cleanup target.

---

## 9. Reconciliation with #383

| #383 finding (11 Nov) | Current observation (20 Nov) | Change |
|---|---|---|
| Z1 `MainIntegration` Library 7,216,824,320 B; Z2 `UIQualification` Library 7,210,270,720 B; owner unconfirmed | Both Libraries **byte-identical in size**, both still present, both still owner-unconfirmed. Unity Hub lists both as last-opened projects. | **Unchanged; hold strengthened** |
| 10 clean docs worktrees, 12,330,786,816 B, conditionally removable | Executed under #385; 12,339,671,040 B recovered; registered worktrees fell 107→97 | **Closed and confirmed by #385** |
| 97 registered worktrees | **240 registered worktrees** (+143 in 8 days, ≈180 GB of new checkouts) | **Major adverse change** |
| Files free 182.95 GB (11 Nov) → 195.29 GB (12 Nov, post-#385) | **24.27 GB free** | **≈171 GB consumed in 8 days** |
| ZX20 free 26.8 GB | **280.8 GB free** | Large non-CCF media (Kinsale) relocated/removed; `Kinsale` now ~0 B |
| S1B binary 1,225,150,464 B, not the smoke-summary build, no independent archive | Still present, unchanged; `Bundles/` still empty | **Unchanged** |
| `S1-80m-local-output` 1,260,412,928 B, uncertain | Still present, unchanged | **Unchanged** |
| Legacy store 121 branches / 45 worktrees, dirty work | All 45 present; 1 stale registration; 2 unique refs identified (stash + tag); 42/45 heads pushed | **Refined** |
| CCF worktrees 185 at 18 Nov (per Stage-1 note) | 240 now | Consistent with rapid growth |

**Skipped expensive rescans (deliberate):** no re-hash of Libraries, no content hashing of builds, no full `CCF/Verification` re-read, no re-audit of the ten B1 paths (already executed and closed).

**Current blockers carried forward:** owner attestation for the two old ZX20 lanes; archive proof for S1B/S1-80m; evidence-retention policy for Verification; T3 ownership of `~/.t3` and `~/dev`; Windows/offline destination unknown.

---

## 10. Future approval queue (not a deletion plan)

**Nothing here is authorised. Every row needs an explicit owner decision and a per-path preflight.**

### Tier A — strongest evidence, smallest risk (30.2 GB)

22 paths that are clean, pushed, unclaimed and older than two weeks.

| Volume | Paths | Bytes |
|---|---:|---:|
| Files | 12 | 14.7 GB |
| Root | 5 | 6.0 GB |
| ZX20 | 5 | 9.5 GB |

### Tier B — review queue ceiling (114.2 GB)

Adds paths up to 13 November. Files 84.8 GB / 77 paths; root 8.6 GB / 7; ZX20 20.8 GB / 7.
Each needs, before approval: task-closure confirmation, live-owner check, `git status` re-check, confirmation that ignored/untracked data is absent, and a recorded reconstruction command.

### Tier C — needs an archive or evidence decision (not counted above)

| Item | Upper bound | Precondition |
|---|---:|---|
| 19 embedded player builds + `CCF/Builds` | 22.2 GB | Verified independent archive; S1B specifically has no archive today |
| `CCF-Legacy` `.git` | 4.0 GB | Preserve stash `7b1c808d` and tag `prototype-baseline` first |
| Verification embedded checkouts | ~30 GB | Evidence-retention review; receipts/logs must be kept |
| `ZX20/CCF-Temp` | 5.35 GB | #802 one-orchestrator lane only |
| T3 tooling (`~/.t3/t3code`, `~/dev`, `Files/Worktrees`, `t3-migration-stage`) | ~58 GB | Separate T3 programme, not this packet |

### Explicitly excluded from every tier

Personal copies #361/#382, U1/U2/U3/Integration, `CCF-Primary`, `Repository.git`, `CCF-Ops`, `CCF/Verification` as a whole, `CCF/Archives`, `CCF/Media`, the three local-only legacy evidence worktrees, all 15 dirty worktrees, and every worktree referenced by an open issue.

---

## 11. Non-mutation attestation

**Read operations only.** `findmnt`, `lsblk`, `df`, `du`, `find`, `stat`, `git worktree list`, `git status`, `git for-each-ref`, `git rev-list`, `git cat-file -e`, `git log`, and GitHub REST GET requests. All Git commands ran with `GIT_OPTIONAL_LOCKS=0`. No `fetch`, `checkout`, `reset`, `stash`, `clean`, `gc`, `repack`, `prune`, `worktree add/remove`, branch creation or deletion, Unity launch, package restore, cache eviction, save/PlayerPrefs access, or file move/rename/delete inside any audited folder.

**One accidental write, disclosed.** At 20:41Z a shell operator-precedence error sent the ZX20 `du` output to the then-current directory instead of `/tmp/opencode`, creating `zx20_top.txt` and `zx20_top.err` inside `/media/jer/Files/CCF-Primary`. They were detected at 20:49Z, copied to `/tmp/opencode` as evidence, and removed at 20:53Z. `git status` of `CCF-Primary` was then verified to match its pre-existing state exactly: 3 modified tracked files (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`, `Docs/Project/coordination/active-tasks.md`) and 4 pre-existing untracked items. No project file, Git object, Unity state, save or cache was altered.

**State during the audit:** at preflight 20:47Z — `/` 24.78 GB free, Files 24.27 GB free, Videos 435.6 GB free, ZX20 280.8 GB free. At handback 21:52Z — `/` **21.70 GB** free, Files 24.27 GB, Videos 435.6 GB, ZX20 280.7 GB. The 3.07 GB root change occurred while other user/agent processes were active; this audit wrote only ~230 KB of local temp output plus GitHub content, and wrote no CCF data. No Unity lock was held; last lease release 20:35:36Z. Unity Hub remained open (user-owned) throughout; no Editor started.

---

## 12. Unknowns and recommended next steps

**Unknown until separately inspected**

1. Offline/other-machine CCF roots, including `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-CompetitionUI` (Multi 2D) — **not on this laptop**.
2. Whether the last `origin` fetch is current; branch-head API checks covered the 19 disputed names but not all 240.
3. Per-worktree ignored-file inventory (only `Library/` presence was tested).
4. Real Unity reimport time per Library on this hardware.
5. Independent archive existence for every standalone build.
6. Windows/destination capacity for any future migration.

**Recommended order for the owner's review (no action taken here)**

1. Decide the two old ZX20 lanes (MainIntegration / UIQualification): still wanted as playtest copies, or retire the warm Library only?
2. Approve or reject a bounded Tier A worktree-retirement batch on **Files** (14.7 GB), with per-path manifests and `git worktree remove` without `--force`.
3. Commission the archive verification for the 22.2 GB of builds before deciding anything about them.
4. Decide the evidence-retention policy for `CCF/Verification` (120.3 GB) — the only route to the further ~30 GB there.
5. Treat T3 tooling (`~/.t3`, `~/dev`) as its own programme; do not mix it into a CCF folder cleanup.
6. Preserve the legacy stash and annotated tag, then re-evaluate the 4.0 GB legacy `.git`.

---

*Prepared as read-only inspection. No folder was deleted, moved, renamed, archived, compressed or cleaned. Approval for any future change remains with the owner, folder by folder.*
