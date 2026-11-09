# #350 — Post-cleanup Unity re-entry preflight (2026-11-09)

Read-only, one pass. Worker: Claude Code (local T3), `claude-sonnet-5-5`, effort medium. Start HEAD `origin/main` `c6cc3e9c`; docs branch `docs/post-cleanup-unity-reentry-20261109-v1`; worktree `/media/jer/Files/CCF/Operations/CCF-PostCleanupUnityReentry-20261109`. Evidence: `/media/jer/Files/CCF/Verification/CCF-PostCleanupUnityReentry-20261109/`.

## Verdict

**Machine, protected workspace and candidate: READY (GO).**
**Starting Unity today: NO-GO**, pending one explicit Manager/user decision (§4). The blocker is not disk space. It is the unchanged #345 risk: the shared `unity-lock` refuses this lane (8 dirty files), and installing the reviewed replacement changes a global tool that can block on a stalled USB read with no timeout (O-3).

Nothing was deleted, moved, staged or written outside my own docs lane and evidence directory. No Unity, lease, flock, lock install or `git status`/refresh on the warm Workbench.

## 1. PASS / FAIL / UNKNOWN

| # | Check | Result | Fresh observation (2026-11-09) |
|---|---|---|---|
| 1 | ZX20 device/mount | **PASS** | `/dev/sdc2` exfat rw, UUID `18D6-896E` (= `lanes.json`) |
| 2 | ZX20 free space | **PASS** | **43,918 MiB = 42.9 GiB** free (92% used) at 09:57Z; re-read 10:00Z identical. Up from 22,160 MiB at #349 (a gain of 21.7 GiB). Measured, not assumed |
| 3 | Other disks | note | Files 12,110 MiB (98%), root 17,673 MiB (93%). Not usable for new work |
| 4 | Headroom vs thresholds | **PASS** | 22.9 GiB above the 20 GiB STOP, 27.9 GiB above the 15 GiB floor, and 15.9 GiB above the 27 GiB cold-entry gate (cold gate not required for the warm lane) |
| 5 | KinsaleBay originals | note | Absent from `ZX20/Kinsale`; two `KinsaleBay*` files present in `/media/jer/Videos/KinsaleMoved`. Not hashed or opened. This is not a verified backup claim |
| 6 | Unity / Hub / lock holder | **PASS** | no Unity, Hub, `unity-lock`, `flock` process; `state/unity/guard` has no holder (lsof); last history event is a release 2026-11-07 01:49 (#287). No lease exists. I did not take a flock |
| 7 | Machine activity / I/O | **PASS** | PSI io `some avg10` 2.7% (avg60 12.97, a transient from my own hashing; avg300 9%); CPU idle. Nothing heavy running (desktop only). #349's 60% pressure is gone |
| 8 | Coordination | **PASS** | `active-tasks.md` @ `c6cc3e9c` read: #350 mine; no other RUNNING owner of the warm lane or ZX20 |
| 9 | Warm Workbench identity | **PASS** | `WorkbenchTier2Reg287` at `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106`; HEAD ref `feature/scenario-one-workbench-retirement-20261105` → tip **`4d95e45245d660e24b91e26754d9c57949bf3384`**; gitfile → canonical `Repository.git/worktrees/CCF-WorkbenchTier2-Registered-20261106` |
| 10 | Library present | **PASS** | `Library/` present, mtime 2026-11-07 01:47 (last Unity run). Not scanned |
| 11 | 8 protected files | **PASS 8/8** | sha256, size and mode (755) all match the #337 table (`protected-8-hashes.txt`) |
| 12 | Index unchanged | **PASS (mtime) / baseline only (hash)** | Index (in the common store) mtime `2026-11-08 09:08:02.700` is identical to the #349 observation, so unchanged since before #349. sha256 `afd75813…ed458b` recorded as the new baseline. No earlier hash exists to compare, so the content claim rests on the mtime |
| 13 | Backup chain | **PASS** | `Verification/Workbench287MorningClosure-20261107/`: `SHA256SUMS.txt` sha256 = `9fb80596…f74` (matches the pinned value), `sha256sum -c` **16/16 OK**; `dirty-backup/` holds 8 files with matching sizes. Second before-image in `CCF-Workbench111GenvB3-20261108/`. No multi-GiB rehash |
| 14 | Standalone recovery anchor | **PASS (present)** | `ZX20/CCF-WorkbenchRetirement-T3-20261105` present, `.git/HEAD` mtime 2026-11-05. Contents not re-inspected (as in #349) |
| 15 | Deployed lock unchanged | **PASS** | `CCF-Ops/scripts/unity-lock` sha256 `54cf0a95…48e5` = the #337 pinned copy; scripts and `lanes.json` mtimes unchanged (2026-10-15 / 11-05 / 11-07) |
| 16 | Candidate C | **PASS** | `69c52906…a2412` exists locally, on `origin/feat/workbench-111-b3-evidence-candidate-20261108-v1`. 13 paths from the warm HEAD `4d95e452`; 3 paths from Option B `46bc1fd6`, all under `Tools/Verification/StandGeometry/` |
| 17 | Lease proposal source | **PASS** | `26e32b9cf8579a97e4988dcab5f674ef03c1cdd9` on `origin/fix/unity-lease-post-transition-pins-20261108-v1`; 4 paths from the N-1 source `11cee0c2`, all under `Tools/Verification/UnityLeaseExactAdmission/` (unity-lock.proposed, patch, 2 tests). Not deployed |
| 18 | Runner takes `--candidate-sha` | **PASS** | `run_enlarged80.py` accepts `--candidate-sha <40-hex>` and `--b3-mode plain\|authored-pin`; rejects non-40-hex |
| 19 | F1 confirmed | **PASS (as a known gap)** | Source check: `status` is `EVIDENCE_COMPLETE` unless the log has `kind=UNAVAILABLE` lines or a required kind is missing. Omitting `--candidate-sha` writes `candidate=UNAVAILABLE` into the JSON but does **not** downgrade `status`. So an operator-side check of candidate, HEAD, scene, package pins and archive fields is mandatory; any UNAVAILABLE ⇒ **B3 EVIDENCE INCOMPLETE / NOT QUALIFIED**. F2 observer effect remains an interpretation risk |
| 20 | Unity Editor binary | **PASS** | `ZX20/Unity/6000.6.0f1/Editor/Unity` present (Sep 13) |
| 21 | Licence | **UNKNOWN** | `UnityEntitlementLicense.xml` present (2026-09-15), only a `StartDate` field; expiry cannot be read offline. Last known good Editor run was #287 on 2026-11-07. Only a real launch proves it (memory: a stale host licence can exit with 198) |
| 22 | Reliable disk-stop monitor | **UNKNOWN (feasible, not built)** | No monitor exists. A plain shell loop (`df` every 20 s, alert at 28 GiB, stop at 25 GiB, absolute 20 GiB) is feasible; nothing ran. See §3 |
| 23 | Runner side effect | note | The runner copies `Enlarged80Verification.cs` into `<lane>/Assets/ForestPrototype/` (and writes `Build/Enlarged80`), then removes it and its `.meta`. In the warm lane this must be checked: no stray file may remain, and the 8 bytes must be re-hashed after |

## 2. What remains before a real run

1. **Lock admission.** The deployed `unity-lock` runs a `git status` clean-lane check and refuses a lane with 8 dirty files. Source proposal `26e32b9c` fixes that but is **not deployed**, and neither the 2026-11-11 expiry nor "+6 h" is approved. (#345, #347/#348)
2. **Global exclusive `flock` with no timeout** can stall `status`/`release` and other owners on a hung USB read (O-3). Disk space does not change this.
3. **Transition not rehearsed on the real lane.** The warm HEAD is 13 paths behind C. A non-forced fast-forward needs the two package files staged (8 dirty → 6 dirty). Only a toy fixture exists.
4. **Disk monitor, licence and operator presence** are not yet in place or proven (items 21–22).
5. **Never done:** Tier-1 G-env compile and B3 (2 plain + 1 authored-pin). Integrated A–O is still 0/15.

## 3. Proposed headroom thresholds (for the Manager to choose)

Start needs ≥ **35 GiB** free (today 42.9). Alert at 28 GiB. Hard STOP at 25 GiB, Manager-visible (the 20 GiB mandatory STOP stays as the absolute limit). Poll `df` every 20 s. Plus PSI io `some avg60` < 10% and no non-session heavy jobs. No consumption data exists for a warm compile and runs; the Library was 6.6 GiB at last use. These are proposals only.

## 4. The ONE recommended next action (needs Manager #200 and user)

**Decide whether to authorise the supervised global-lock window (workflow below), or choose Option B, which avoids the global lock.**

- **Option A (the §5 workflow):** keeps the warm Library and the original Workbench provenance. Cost: installing a modified shared `unity-lock` for a short, human-attended window, the unrehearsed exFAT transition and the O-3 stall risk.
- **Option B (for the Manager to rule on; I did not test it):** run the same Tier-1 G-env and B3 sequence in an existing clean registered lane instead (for example `CCF-U3`, clean, merged, no Library) checked out at candidate C. The deployed lock then works unmodified and the 8 protected files and warm Library are never touched. Cost: a cold import (≈6–7 GiB, takes time), a lane/branch change on a different lane, and a different provenance than "the warm Workbench". Space now clears the 27 GiB cold gate (42.9 GiB). This is a new-Library decision that the packet and #345 deferred.

I recommend A only if the user can be at the machine for the whole window. Otherwise recommend B.

## 5. Proposed attended workflow (Option A; nothing here is approved or run)

Every step has a **STOP** if its condition fails. Stop = no further steps; roll back what that step changed; report.

| Step | Action | STOP if |
|---|---|---|
| S0 | Operator at machine. Manager/user approve in writing: exact script bytes, a fixed window (start/end), rollback, who is present. No silent 11 Nov expiry | Any approval missing |
| S1 | Fresh checks: `df` ZX20 ≥ 35 GiB, PSI, no Unity/Hub, guard unheld, no other RUNNING owner, mount UUID | Any red |
| S2 | Confirm sources/backups: C `69c52906…`, profile `26e32b9c…` available; `SHA256SUMS` 16/16; 8 files 8/8 (sha256+size+mode); HEAD `4d95e452`; index mtime/sha256 vs baseline; deployed lock sha256 `54cf0a95…`. Copy the deployed `unity-lock` + `_ccf.py` to a rollback file first | Any mismatch |
| S3 | **(Separate permission)** Install `unity-lock.proposed` @ `26e32b9c` with the fixed expiry. Read-only `status` smoke only | `status` hangs (use `timeout`; do not kill foreign tasks); hash differs from the reviewed bytes |
| S4 | **(Separate permission)** Operator stages the two package files at 1.1.1, then **non-forced** fast-forward `4d95e452 → 69c52906` on the warm lane. Prove 8 → 6 dirty, 2 clean pins, 13 changed paths, digest `eda2bfe3…d7cd`, Legacy40 anchors untouched. No `--force`, reset, stash, clean | Any other path changes; any of the 6 remaining dirty files differs from the backup; git errors. Roll back from the before-image only after Manager approval |
| S5 | Start the disk monitor; then acquire the real exclusive lease through the installed lock with post-transition admission; record lease id/owner | Admission refuses; monitor not running; free < 35 GiB |
| S6 | ONE Editor session: Tier-1 G-env compile, then B3 via `run_enlarged80.py --candidate-sha 69c52906… --b3-mode plain` ×2 and `--b3-mode authored-pin` ×1 | Free < 25 GiB (alert) / < 20 GiB (hard stop); licence error; compile error; Editor hung. Operator ends the Editor, no `kill -9` of anything else |
| S7 | Inspect each JSON: candidate, HEAD, scene, package pins, archive fields, Legacy40 before/after. Any `UNAVAILABLE` or mismatch ⇒ **B3 EVIDENCE INCOMPLETE / NOT QUALIFIED** regardless of "EVIDENCE_COMPLETE". Note F2/F3/F4 limits. `EVIDENCE_COMPLETE` ≠ `B3 PASS` | n/a (classification) |
| S8 | Release the lease (owner-safe). Confirm no Editor. Verify no stray `Enlarged80Verification.cs`/`.meta` in the lane's Assets | Release fails |
| S9 | Restore the original `unity-lock` + `_ccf.py` from the S2 copy; verify sha256 `54cf0a95…`. Re-hash the 6 remaining dirty files, the 2 clean pins, the index, and the backup chain. Final `df`. Report. Library and 8 originals stay preserved | Hash mismatch ⇒ BLOCKED, no fixes without Manager |

The 6-file post-transition set is: the 5 `.mat` files and `.vscode/settings.json` (still dirty); `Packages/manifest.json` and `packages-lock.json` become clean pins at 1.1.1 (per #346/#347). **A transition that changes the real lane's bytes must be observed, not inferred from the toy fixture.**

## 6. Not done (by design)

No storage audit, security or mutation tests, 77+40+13, mini fixture, Unity stub compile, cleanup, cache purge, video action, lock install or lease, `git status`, or any write to the warm Workbench, CCF-Ops, or other lanes. Docs lane left in place.

## 7. Outstanding explicit decisions

1. Manager/user: Option A (global-lock window; needs fixed expiry, rollback, presence) or Option B (cold clean lane).
2. If A: separate permission for S3 and for S4.
3. Threshold values in §3.
4. A Windows build still needs the user's personal Scenario One playtest and separate approval.
