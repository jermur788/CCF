# Sen-1 #741-C1 — exFAT replacement, sync and recovery qualification (v5)

**STATUS: QUALIFICATION EXECUTED AND PASSED 20/20. SCRATCH ONLY. LIVE OPS UNTOUCHED. NOT AN INSTALL PERMISSION.**

**Run:** `/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV5-20261118T145959Z`, **2026-11-18 14:59:59–15:00:03 UTC** (from the log's own `root`/`started_utc`/`finished_utc`).
**Author:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), provider `opencode`, T3 Code / OpenCode, host `jer-laptop`, user `jer`, orchestrator of [#741](https://github.com/jermur788/CCF/issues/741).
**Independence:** **not** independent — I wrote and ran it. The reviewer supplies the verdict.

## 1. Revision history — every rejection kept visible

| Rev | Result | Rejected because |
| --- | --- | --- |
| v1 | 13/13 | dir sync a side probe; drift never ran rollback code; no I/O-error path; PASS ignored samples; over-broad claim; no mount snapshot |
| v2 | 14/14 | post-rename dir-sync `EIO` still passed; publication recovery unqualified; receipt not same primitive; drift used an unchanged file; sampler pathname race + truncated digest |
| v3 | 16/16 | batch control-flow: `EIO` in the loop left files published and skipped rollback; a renamed path not yet booked could be missed |
| v4 | 19/19 | **post-rename I/O error could still escape**: a readback `EIO` or a directory-`open` `EIO` raised out of the publish function after the rename, so a published path went unbooked and recovery never ran; a failing restore could stop recovery |
| **v5 (this run)** | **20/20** | — |

## 2. What v5 fixes — the class, not one instance

1. **Expected hash is computed from the source bytes before publishing.** Rollback comparison therefore never depends on a successful readback of the installed file.
2. **The publish function cannot raise after `rename`.** Readback, directory `open` and directory `fsync` errors are all captured into the returned record.
3. **Every renamed path is booked immediately**, before any success decision, so no published file can fall outside the recovery set.
4. **Recovery is exception-tolerant.** If one restore fails, the remaining paths are still restored, the failure is reported as `RESTORE-ERROR` with the live hash, and the partial state is stated explicitly rather than presented as success.

## 3. Results — 20 tests, 20 PASS, 0 FAIL

| Test | Result | Observed |
| --- | --- | --- |
| `environment.same-device-as-live-ops` | PASS | `exfat`; scratch `st_dev` 2098 = live Ops `st_dev` 2098 |
| `setup.replica-byte-identical-to-live-baseline` | PASS | 50/50, 0 mismatches |
| `recovery.backup-50` | PASS | identical to pre-state |
| `atomic.replace-with-directory-sync` | PASS | 17 samples = 4× complete old + 13× complete new, no other pair, no `ENOENT` |
| `batch.receipt-dir-sync-eio` | PASS | 6 booked, failure `post-rename-io` (dir-fsync, EIO) at the receipt, 6/6 `RESTORED`, pre-state exact |
| `batch.receipt-dir-open-eio` | PASS | same, with the directory **open** failing |
| `batch.receipt-readback-eio` | PASS | same, with the **readback** failing — booking survives because the expected hash was precomputed |
| `*.no-tmp` (×3) | PASS | no `.tmp` residue after any scenario |
| `batch.restore-failure-is-reported-not-swallowed` | PASS | 1 `RESTORE-ERROR`, 5 `RESTORED`; recovery did not stop |
| `batch.partial-state-is-explicit` | PASS | the un-restored path's live hash recorded, not hidden |
| `batch.repair-after-failed-restore` | PASS | explicit repair returns 50/50 baseline |
| `batch.synthetic-abort-after-three-recovers` | PASS | 3 published → 3 restored |
| `install.full-six-path-success` | PASS | 6/6 published |
| `install.protected-paths-unchanged` | PASS | board/lanes/`environment.sh`/`AGENTS.md` untouched |
| `recovery.full-install-rollback` | PASS | 6/6 `RESTORED`, pre-state exact |
| `recovery.drift-refusal` | PASS | drifted `_ccf.py` refused, not overwritten |
| `final.50-file-baseline` | PASS | 50 files, 0 differing |
| `custody.guard-exclusion-mechanism` | PASS | held flock blocks a second acquirer |

## 4. Limits — unchanged and stated plainly

Cannot prove real power loss or kernel crash (block-layer fault injection would risk the live tree), the live directory's cluster history or free space, the live board flock or Unity lease, Git/worktree behaviour, or behaviour under real load. Directory `fsync` succeeding here must not be assumed on another kernel.

**Accepted residual risk:** power loss or crash during the deploy window is carried as a **deploy-window condition** — a verified off-volume backup before the first write, and reconciliation of filesystem and installed hashes under renewed custody after any interruption.

**Scope note, stated honestly:** this harness now enumerates the failure classes a single-user install can plausibly meet (write error before publication, drift, post-rename directory sync, directory open, readback, restore failure, synthetic abort). It is not a proof of universal fault tolerance, and it is not the installer — the installer is a human-run procedure whose steps the dossier fixes.

## 5. Live Ops state

Read-only at 14:16Z, 14:28Z, 14:34Z, 14:45Z, 14:53Z and 15:00Z: 50/50 hashes OK, board `2b99552c…`, lanes `c17903dd…`, receipt `8c66b290…`, no `lock.json`, no flock on `08:32`, no Unity process, nothing modified since 12:52:39Z.

## 6. Evidence

`/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/` — `exfat-qualification-v5.json` (`ea9ce18713043eaf4e8cd3a2bc538463c30c750e1df705b81e72ffb296a5ad1a`), `exfat_qual_v5.py` (`8707300948e172479f9a88560967131091a0835dd842cab4d5f98d63d1d31f8a`), `SHA256SUMS`. v1–v4 retained.

## 7. Status

No installation, no board reservation, no Unity, no game source, no Windows, no main merge. The owner's **NO LIVE INSTALL** order remains operative.
