# Sen-1 #741-C1 — exFAT replacement, sync and recovery qualification (v4)

**STATUS: QUALIFICATION EXECUTED AND PASSED 19/19. SCRATCH ONLY. LIVE OPS UNTOUCHED. NOT AN INSTALL PERMISSION.**

**Run:** `/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV4-20261118T145302Z`, **2026-11-18 14:53:02–14:53:06 UTC** (values read from the log's own `root`/`started_utc`/`finished_utc`).
**Author:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), provider `opencode`, T3 Code / OpenCode, host `jer-laptop`, user `jer`, orchestrator of [#741](https://github.com/jermur788/CCF/issues/741).
**Independence:** **not** independent — I wrote and ran it. The reviewer supplies the verdict; this document supplies reproducible evidence and states its own limits.

## 1. Revision history — kept visible, never rewritten

| Rev | Result | Rejected because |
| --- | --- | --- |
| v1 `T142559Z` | 13/13 | dir sync only a side probe; drift probe never ran rollback code; no I/O-error path; PASS ignored reader samples; over-broad "scratch-only" claim; no mount snapshot |
| v2 `T143503Z` | 14/14 | post-rename directory-sync `EIO` still produced PASS; recovery after publication unqualified; receipt not written with the same primitive; drift probe used an unchanged file; sampler used a separate `stat` + truncated digest |
| v3 `T144412Z` | 16/16 | **batch control-flow defect**: a dir-sync `EIO` inside the loop left published files behind and skipped rollback; a path renamed but not yet booked could be missed; no shared abort-and-recover procedure |
| **v4 (this run)** | **19/19** | — |

All logs and scripts are retained. Nothing earlier was deleted or retro-actively described as correct.

## 2. What v4 fixes

1. **`replace_one()` never raises across a bookkeeping boundary.** It returns `{path, sha256, dir_sync_ok}`; the caller records the published path *before* deciding. A path that was renamed can therefore never escape the recovery set.
2. **One `batch_install()` owns both failure modes** — a synthetic mid-batch abort and a post-rename directory-sync `EIO` — through a single reverse-order, compare-before-write, directory-synced recovery path.
3. **The reviewer's named gap is executed exactly:** five replacements succeed, then a directory-sync `EIO` is forced on the **already-renamed receipt**, and all **six** paths are restored with sync to full pre-state.
4. Recovery is verified to leave **no `.tmp` residue** and to restore `board/active.md`, `lanes.json`, `environment.sh` and `AGENTS.md` untouched.

## 3. Results — 19 tests, 19 PASS, 0 FAIL

| Test | Result | Observed |
| --- | --- | --- |
| `environment.same-device-as-live-ops` | PASS | `exfat`; scratch `st_dev` **2098** = live Ops `st_dev` **2098** |
| `setup.replica-byte-identical-to-live-baseline` | PASS | 50 expected / 50 present / 0 mismatches |
| `recovery.backup-50` | PASS | backup identical to pre-state |
| `atomic.replace-with-directory-sync` | PASS | 8 MiB → 6 MiB; `dir_sync_ok`; 18 samples = 4× complete old + 14× complete new, no other pair, no `ENOENT` |
| `recovery.io-error-before-publication` | PASS | injected `ENOSPC`; target unchanged; **partial `.tmp` left**, as expected |
| `recovery.io-error-cleanup` | PASS | `.tmp` removed |
| **`batch.named-gap-five-then-receipt-eio`** | PASS | 6 published; failure `post-rename-dirsync-eio` at `deployment.json` (index 6); first five all `dir_sync_ok` |
| **`batch.published-renamed-receipt-is-bookkept`** | PASS | the renamed receipt is in the recovery set at `ee13a811…` |
| **`batch.synced-restoration-of-all-six`** | PASS | 6/6 `RESTORED`, every restore `dir_sync_ok = true` |
| **`batch.full-pre-state-restored`** | PASS | 50 files, 0 differing |
| `batch.no-tmp-residue` | PASS | no `*.tmp` anywhere in the tree |
| `batch.synthetic-abort-after-three-recovers` | PASS | 3 published → 3 restored, pre-state exact |
| `install.full-six-path-success` | PASS | all six published, all `dir_sync_ok` |
| `install.protected-paths-unchanged` | PASS | board / lanes / `environment.sh` / `AGENTS.md` untouched |
| `recovery.full-install-rollback` | PASS | 6/6 `RESTORED`, pre-state exact |
| `recovery.drift-refusal-through-real-function` | PASS | `_ccf.py` installed as `24a32146…`, drifted, rollback **REFUSED**, file left drifted |
| `cleanup.drift-probe-restored` | PASS | probe restored |
| `final.50-file-baseline` | PASS | 50 files, 0 differing |
| `custody.guard-exclusion-mechanism` | PASS | held `flock` blocks a second acquirer; succeeds after release |

## 4. What this proves, and what it cannot

**Proves on the real exFAT device and mount:** atomic publication to a concurrent reader; directory `fsync` accepted; a post-rename directory-sync failure is detected and forces a **whole-batch** synced recovery of every published path including the failed one; no path can be published without being booked; recovery returns byte-exact pre-state with no `.tmp` residue; the receipt obeys the same primitive; a failed write never reaches `rename`; drift is refused by the same function; the guard lock genuinely excludes.

**Cannot prove:** real power loss or kernel crash (block-layer fault injection would risk the live tree); the live directory's cluster history or free space; the live board flock or Unity lease; Git/worktree behaviour; real load. Directory `fsync` succeeding here must not be assumed on another kernel.

**Accepted residual risk (the reviewer's own framing):** power loss or crash during the deploy window is carried as a **deploy-window condition** — a verified off-volume backup must exist before the first write, and after any interruption the filesystem and installed hashes must be reconciled under renewed custody before continuing.

## 5. Live Ops state

Read-only observation at 14:16Z, 14:28Z, 14:34Z, 14:45Z and 14:53Z: 50/50 installed files OK, `board/active.md` `2b99552c…`, `lanes.json` `c17903dd…`, `deployment.json` `8c66b290…`, no `state/unity/lock.json`, no `/proc/locks` entry for `08:32`, no Unity process, nothing in live Ops modified since 12:52:39Z. Only `stat()`/`findmnt`/hash reads touched it.

## 6. Evidence

`/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v4/` — `exfat-qualification-v4.json` (`e04922f8b5a7e5c29d0d9b186b3a2fb99d70196a5ba88104538c0ac03db39552`), `exfat_qual_v4.py` (`4a72149a97b31f0704534c8603646cc0d37a6026b394263a276f9ddf3c0386fc`), `SHA256SUMS`. v1–v3 retained in sibling folders.

## 7. Status

Evidence supplied; only the reviewer can close the finding. No installation, no board reservation, no Unity, no game source, no Windows, no main merge. The owner's **NO LIVE INSTALL** order remains operative.
