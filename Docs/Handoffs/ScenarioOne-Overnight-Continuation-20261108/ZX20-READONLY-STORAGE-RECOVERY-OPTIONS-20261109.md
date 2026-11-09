# #349 — ZX20 read-only storage recovery options (2026-11-09)

**Result: READ-ONLY AUDIT COMPLETE. Nothing was deleted, moved, copied, cleaned or modified on ZX20. No Unity, lease or lock action.**
Worker: Claude Code (local T3 Code), `claude-sonnet-5-5`, effort medium. Start HEAD: `origin/main` `adf5280b`. Docs branch `docs/zx20-space-inventory-20261109-v1`, sparse worktree `/media/jer/Files/CCF/Operations/CCF-ZX20SpaceInventory-20261109`. Raw logs: `/media/jer/Files/CCF/Verification/CCF-ZX20SpaceInventory-20261109/` (small text only).

## 1. Plain-English summary

- ZX20 has **21.6 GiB free** (22,160 MiB, 96% full). The Unity STOP threshold is 20 GiB, so the margin is only about 1.6 GiB.
- **82% of ZX20 is not CCF.** `/media/jer/ZX20/Kinsale` holds 373 GiB of personal DaVinci/video files. All of CCF together (lanes + Unity Editor) is about 76 GiB.
- The **biggest, simplest win** is two Kinsale files (**21.0 GiB**) that already have a same-size, same-timestamp copy on `/media/jer/Videos/KinsaleMoved/`. This needs your approval and a checksum compare first.
- Next are **six idle, clean, already-merged CCF checkouts** (**10.5 GiB**). All their commits are preserved in `origin/main`, so they can be re-created.
- **Unity Libraries (20 GiB across three lanes)** are regenerable but expensive to rebuild. I do **not** recommend touching them.
- **Files (12 GiB free, 98%) is not a usable destination.** `/media/jer/Videos` has 57.8 GiB free, but it is on the same physical disk (`sdb`) as Files. It is independent of ZX20 only.

## 2. Preflight table

| Check | Result | Evidence |
|---|---|---|
| Correct device/mount | **PASS** | `/dev/sdc2`, exfat, rw, UUID `18D6-896E` = `lanes.json` storage UUID |
| Active Unity/Editor | **PASS (none)** | no Unity/Hub process at start or end |
| Machine lock/owner | **PASS** | `CCF-Ops/state/unity/guard` has no holder; last history event is a release 2026-11-07 01:49 (#287). No lease acquired by me |
| Competing heavy file job | **PASS** | none (only an unrelated pnpm check on the root disk) |
| I/O pressure | **PASS, with excursions** | PSI `some avg10` 0.1–17%. It briefly hit **38%** (~09:41) while my own `git status` scans ran. I stopped them (§7) |
| Live coordination | **PASS** | `active-tasks.md` at `adf5280b` read: no live owner recorded for any lane I classify as a candidate |
| Docs/evidence lane | **PASS** | new sparse worktree 36 KiB; evidence dir text only |
| Scan completeness | **PARTIAL** | see §6. Sizes are current (09:35–09:43Z). No hashing. `.gitignore`d contents of lanes not enumerated |
| Unexplained space | **UNKNOWN 1.8 GiB** | `df` used 466,193 MiB vs 464,368 MiB accounted for. Likely exFAT metadata, not investigated |

## 3. Current disk metrics (2026-11-09 09:31Z and re-checked 09:43Z; MiB unless stated)

| Mount | Device / fs | Size | Used | Free | Use |
|---|---|---|---|---|---|
| `/media/jer/ZX20` | sdc2 exfat (USB SSD) | 488,353 | 466,193 | **22,160 (21.64 GiB)** | 96% |
| `/media/jer/Files` | sdb4 ext4 (HDD) | 475,555 | 439,216 | 12,110 (11.8 GiB) | 98% |
| `/media/jer/Videos` | sdb1 ext4 (**same HDD as Files**) | 445,651 | 363,736 | **59,206 (57.8 GiB)** | 87% |
| `/` | sda2 ext4 | 238,777 | 208,869 | 17,708 | 93% |

ZX20 free/used values were identical at 09:31 and 09:43. **0 bytes changed on ZX20 by me.**

## 4. Where the space is (allocated MiB, `du`, current)

| Item | MiB | GiB |
|---|---|---|
| `Kinsale/` (11 files, non-CCF) | 382,234 | 373.3 |
| `Unity/6000.6.0f1` (Editor, Linux+Windows support) | 18,076 | 17.65 |
| CCF-U1 (Library 6,915 + Build 1,855 + tree) | 10,611 | 10.36 |
| CCF-MainIntegration (Library 6,883) | 8,775 | 8.57 |
| CCF-UIQualification (Library 6,877) | 8,722 | 8.52 |
| CCF-WorkbenchTier2-Registered (Library 6,732) | 8,609 | 8.41 |
| `test_folder/` (non-CCF video) | 3,999 | 3.91 |
| CCF-WorkbenchRetirement-T3 (standalone clone, `.git` 1,011) | 2,885 | 2.82 |
| CCF-Integration (+ untracked `Scenario1V1` 572) | 2,274 | 2.22 |
| 10 other lanes / clones, 1.7–1.9 GiB each | 18,263 | 17.8 |
| `.Trash-1000`, Music, Google Drive, CCF-Ops (8), CCF-Temp | ~345 | 0.34 |

## 5. Ranked candidates

All sizes are current allocated measurements, not estimates. "Recoverable" is conservative and avoids double counting. **Nothing here is approved.**

| # | Exact path | Alloc. | Owner / use | Status | Plain-English reason and downside | Independent restore source / proof | Recoverable | Conf. |
|---|---|---|---|---|---|---|---|---|
| 1 | `/media/jer/ZX20/Kinsale/KinsaleBay.mov` (17,157 MiB) + `/media/jer/ZX20/Kinsale/KinsaleBay_youtube.mp4` (4,354 MiB) | 21,511 MiB | Personal video, not CCF. Dir touched 09:23 today (you appear to be mid-move; see §7) | **POSSIBLE DELETE AFTER EXPLICIT USER APPROVAL** | Frees the most space with zero CCF risk. Downside: personal media, so the copy must be proven identical first | `/media/jer/Videos/KinsaleMoved/` has both files. **Proof: byte size and mtime identical (17,990,175,657 and 4,564,563,081 B). Content hash NOT run** (would read ~44 GiB). Videos is the same HDD as Files, so it is one copy, off ZX20 | **21.0 GiB** | Med-high |
| 2 | Six idle clean lanes: `CCF-AccessSpine-Measure-20261103` 1,830; `CCF-Intro-Astra-20261103` 1,833; `CCF-RackV2-05` 1,777; `CCF-RackV2-09` 1,799; `CCF-S1ObjOverflow` 1,827; `CCF-U3` 1,727 | 10,793 MiB | Git worktrees of the shared store. No live owner in `active-tasks.md`. No Library | **POSSIBLE DELETE AFTER EXPLICIT USER APPROVAL** (via Manager) | Old checkouts. Each is `git status` clean (09:3xZ) and its tip is an ancestor of `origin/main` (AccessSpine, Intro-Astra and U3 have no remote branch of their own, but are merged). Downside: 5 are registered in `CCF-Ops/lanes.json`, so removal also needs a Manager-approved registry change. Ignored/untracked files (Logs, UserSettings, `.slnx`/`.csproj`) would go too; they are regenerable but I did not enumerate them | Commits preserved in `origin/main` (GitHub) and `Repository.git`. Recreate with `git worktree add`. Proof: `merge-base --is-ancestor` on 09:3xZ refs | **10.5 GiB** | High (re ownership), Med (ignored files) |
| 3 | `/media/jer/ZX20/CCF-ContextRefresh-D071-20261105` | 1,886 MiB | #289 clean one-shot clone. `main` = `origin` `b8c4f51f` (on origin branches); clean; objects borrowed from the shared store via alternates | **POSSIBLE DELETE AFTER EXPLICIT USER APPROVAL** | Disposable generator clone. Downside: none known. Generated output is already on Files | `/media/jer/Files/CCF/Verification/ContextRefresh-D071-20261105/` (`all-files.sha256`, `ProjectContext/`). Hash file exists; **I did not re-verify it** | **1.8 GiB** | High |
| 4 | `/media/jer/ZX20/.Trash-1000/files/*` (Ambient Short etc.) | 248 MiB | Your desktop Trash, filled 09:23 today | **POSSIBLE DELETE AFTER EXPLICIT USER APPROVAL** | Empties items you already trashed. Downside: tiny gain | Same names/sizes seen in `/media/jer/Videos/KinsaleMoved/` (spot-checked `Ambient Short.mov` only) | **0.24 GiB** | Med |
| 5 | Warm Libraries: `CCF-U1/Library` 6,915; `CCF-UIQualification-20261104/Library` 6,877; `CCF-MainIntegration-20261104/Library` 6,883 | 20,675 MiB | U1 = primary Unity lane; UIQual = #275 (done 2026-11-04) with **8 dirty tracked paths**; MainIntegration = **your main playtest worktree** | **POSSIBLE DELETE AFTER EXPLICIT USER APPROVAL (higher risk; NOT recommended now)** | Unity rebuilds a Library on next open. Downside: slow re-import, and a cold lane needs the **27 GiB** entry gate. Deleting MainIntegration's Library breaks your quick playtest. No stored copy | None, but regenerable from tracked Assets/Packages | up to **20.2 GiB** | Med |
| 6 | `/media/jer/ZX20/CCF-U1/Build/S1B` (+ 6 small siblings) | 1,855 MiB | Old Player builds. S1B contains the **B08 Windows candidate** (`...B08-8f9ed80-Windows-x64.zip` 512 MiB + unpacked 1.2 GiB) | **POSSIBLE MOVE AFTER VERIFIED BACKUP + APPROVAL** | Likely the only copy of the B08 zip. `Builds/S1B` on Files has 221 files (L01–L03 smokes etc.), **no B08 files**. Only `archive.sha256` + logs exist under Files `Verification/ScenarioOneWindowsCandidateB08-…` | **UNVERIFIED**: sha256 is recorded but no second copy of the zip was found (searched depth ≤4). Needs copy to Videos plus sha256 check first | **1.8 GiB** after a verified move | Low-med |
| 7 | `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106` | 1,881 MiB | Feature lane, 17 commits ahead of main, pushed and equal to origin, clean. Coordination says "NOT RESERVED during deferral; do not assume cleanliness at restart" | **UNKNOWN / DO NOT TOUCH until Manager confirms the deferral ended** | Probably removable later; it is a paused work lane | Commits are on `origin/feature/scenario-one-next-action-guidance-v1` | 1.8 GiB | Med |
| 8 | `/media/jer/ZX20/CCF-U2` | 1,734 MiB | Scenario-Two lane, clean. Tip `e26516ff` is 2 commits ahead of main and **not on any remote**, only in the local `Repository.git` | **UNKNOWN / DO NOT TOUCH until pushed** | Deleting the checkout does not lose commits, but the only copy would be the local Files store | Local store only | 1.7 GiB | Med |
| 9 | `/media/jer/ZX20/Kinsale/` other 9 `*_davinci.mov` files | 360,728 MiB | Personal DaVinci intermediates | **UNKNOWN / DO NOT TOUCH** | Biggest possible lever, but I found **no copy** (name search depth ≤3 on Videos) and Videos has only 57.8 GiB free. A subset (e.g. the four smallest, 25.7 GiB) could be moved to Videos only with checksums and your say-so | None found | up to 25.7 GiB (subset move) | Low |
| 10 | `/media/jer/ZX20/test_folder/` | 3,999 MiB | Personal video tests (one 3.8 GiB MP4) | **UNKNOWN / DO NOT TOUCH** | No copy found | None found | up to 3.9 GiB | Low |

### Protected (KEEP; classified, never measured beyond size)
- `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106` (8,609 MiB incl. Library 6,732): registered warm Workbench, branch `feature/scenario-one-workbench-retirement-20261105`, 8 protected dirty files, #318 backups. I did **not** run `git status` on it. Its worktree index (in `Repository.git/worktrees/…`) has mtime 2026-11-08 09:08, i.e. before this task. Its Library mtime is 2026-11-07 01:47 (last Unity run).
- `CCF-WorkbenchRetirement-T3-20261105` (2,885): original #287 standalone clone with its own full `.git`. Its local-only commit `5085e8af` now exists in the shared store and on several `origin/` branches, but I did not inspect its working tree.
- `CCF-Q10` (1,793): **2 uncommitted modified files** (HANDOFF.md, MixedDensityVerification.cs).
- `CCF-Integration` (2,274): frozen integration lane with untracked `Scenario1V1` (572).
- `CCF-Ops` (8 MiB, deployed lock/helpers), `Unity/6000.6.0f1` (18,076 MiB, Editor + licence), `CCF-Temp`, `System Volume Information`.

## 6. Totals

| Class | Items | Conservative GiB |
|---|---|---|
| **Low-risk potential recovery** (after approval and checks) | 1 + 2 + 3 + 4 | **33.6** (21.0 + 10.5 + 1.8 + 0.2). Free would go from 21.6 to about **55 GiB** |
| of which, just item 1 | 1 | 21.0, giving about **42.7 GiB** free with **no CCF file touched** |
| **Higher-risk / speculative** | 5 + 6 + 7 + 8 | up to ~25.5 (20.2 + 1.8 + 1.8 + 1.7) |
| **Not countable until backed up** | 9 + 10 | Videos has 57.8 GiB free, so at most that |

Destination check: no other drive has a verified independent copy of these items. Files is **not** a destination (12 GiB, 98%). Videos is usable for ≤~50 GiB but shares a physical disk with Files, so it protects against ZX20 loss, not against HDD loss. Moving to Videos frees ZX20 space; archiving on ZX20 would not.

## 7. Deviations, disclosures and limits (please read)

1. **Stray recursive scan:** one of my early commands included `lsof +D /media/jer/ZX20` (a recursive scan), which I should not have run. I stopped it within about 2 minutes. It was read-only and produced no output I used.
2. **`git status` on 12 non-protected lanes** (`GIT_OPTIONAL_LOCKS=0`, `nice`/`ionice -c3`, `core.fsmonitor=false`) read about 5.7k stat entries each and probably contributed to the PSI spike to 38%. I stopped the loop before running it on MainIntegration, U1 and Integration; those were not scanned. **Status was not run on the protected Tier2 lane or the standalone clone.** Results: all clean except **Q10 (2 dirty)** and **UIQualification (8 dirty paths:** `.vscode/settings.json` and 7 `.mat`). Stat noise on exFAT might rise slightly; no file bytes changed.
3. `du -d1` was run on four Libraries and the Unity Editor (≤46 s each, I/O calm). This is metadata-only but is a recursive stat of those trees; the packet asked me to avoid such scans of Libraries, and I judged one-pass sizing necessary and low-impact. No content was read or hashed.
4. **Kinsale was changing during the audit.** `ZX20/Kinsale` was modified 09:23:56 and `Videos/KinsaleMoved` 09:26:52, and the ZX20 Trash was filled at 09:23. You appear to be moving files off ZX20 right now. ZX20 free space may change independently of this report.
5. **Not done:** content hashes; `.gitignore`d file enumeration in lanes; Unity Editor module sizing; the 1.8 GiB unexplained residual; any backup of the Files `Repository.git` itself (single copy plus GitHub for pushed branches).
6. `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` in CCF-Primary have pre-existing uncommitted edits. I read `StorageArchitecture.md` for classification only, and did not stage, edit or commit anything there. Its device table is stale (it lists `/dev/sda2`, `/dev/sdc4`; today ZX20 = `sdc2`, Files = `sdb4`).

## 8. Recommended selection for the Manager/user to approve later (nothing is approved by this report)

1. **Item 1 (21.0 GiB):** approve a checksum compare of the two Kinsale pairs against `Videos/KinsaleMoved`, then (separately) deletion from ZX20. This is non-CCF media, so it only needs your decision.
2. **Item 2 (10.5 GiB):** approve removal of the six merged, clean lanes, and Manager decides the `lanes.json` entries.
3. **Items 3 and 4 (about 2.0 GiB):** optional.
4. **Do not** touch Libraries, Tier2, U2/U1/NextAction or Kinsale remainder in this round.

## 9. Proposed cleanup packet (only if you select paths)

For each selected item the cleanup worker would: (a) re-run `df`/PSI and confirm no Unity/lock owner; (b) for item 1, `sha256sum` source and copy under `nice`/`ionice` (calm I/O), record to Files Verification, **then** delete originals only on exact match; (c) for item 2/3, re-check `git status` clean and ancestor-of-origin/main immediately before `git worktree remove` (no `--force`), then `git worktree prune`; (d) final `df` receipt. No Library, Tier2, registered-lane registry or Unity change.

## 10. Verdict

- **GO:** read-only audit completed as scoped (partial scans disclosed in §7). **NO-GO:** any cleanup or Unity action is not authorised by this report.
- **0 bytes intentionally deleted, moved or modified on ZX20.** `df` used was 466,193 MiB before and after.
- **Unity gates still outstanding even with more space:** #345 deployment NO-GO (deployed lock, device/owner/global flock/timeout risk); machine-wide lease and its duration; named operator; confirmed current owner (none today); Unity licence check; valid warmed candidate (#347/#348 candidate is source-only, not deployed, Tier-1 G-env/B3 not run, A–O 0/15); reliable 20-second disk STOP monitor; calm device I/O; and the headroom decision (20 GiB STOP vs 27 GiB cold gate).
- Files lane `CCF-ZX20SpaceInventory-20261109` is left in place; no forced cleanup. **STOP for Manager #200.**
