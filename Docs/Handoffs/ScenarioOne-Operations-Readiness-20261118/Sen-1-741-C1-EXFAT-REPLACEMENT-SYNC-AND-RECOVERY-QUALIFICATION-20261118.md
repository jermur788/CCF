# Sen-1 #741-C1 — exFAT replacement, sync and recovery qualification (v6, final harness)

**STATUS: QUALIFICATION EXECUTED AND PASSED 21/21. SCRATCH ONLY. LIVE OPS UNTOUCHED. NOT AN INSTALL PERMISSION. NOT RE-REVIEWED AFTER v6.**

**Run:** `/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV6-20261118T150753Z`, **2026-11-18 15:07:53–15:07:57 UTC** (from the log's own fields).
**Author:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), provider `opencode`, T3 Code / OpenCode, host `jer-laptop`, user `jer`, orchestrator of [#741](https://github.com/jermur788/CCF/issues/741).
**Independence:** **not** independent. The reviewer's verdict governs; this document supplies evidence and states its own limits.

## 1. Revision history — every rejection kept visible

| Rev | Result | Rejected because |
| --- | --- | --- |
| v1 | 13/13 | dir sync a side probe; drift never ran rollback code; no I/O-error path; PASS ignored samples; over-broad claim; no mount snapshot |
| v2 | 14/14 | post-rename dir-sync `EIO` still passed; publication recovery unqualified; receipt not same primitive; drift used an unchanged file; sampler pathname race |
| v3 | 16/16 | batch control flow: `EIO` in the loop left files published and skipped rollback |
| v4 | 19/19 | post-rename I/O error could escape via readback or directory-open; a failing restore could stop recovery |
| v5 | 20/20 | a `KeyboardInterrupt` during readback could still escape `except OSError`; a pre-state read error could bypass recovery; a diagnostic re-read error could stop it |
| **v6 (final)** | **21/21** | — |

**Independent review status of v6: NOT RE-REVIEWED.** The reviewer's last verdict (on v5) was `STILL-BLOCKING-WITH-NAMED-GAP`. v6 closes the two closable items in that verdict (pre-state read error, diagnostic re-read error) and the signal class it named (`KeyboardInterrupt`), and it does **not** claim to close the uncatchable class. This is stated here rather than glossed.

## 2. What v6 fixes

1. **Booking precedes every risky step.** The published record is created immediately after `rename`, before readback or directory sync, so any later exception — including a signal — cannot leave a published path unbooked.
2. **All per-file steps are guarded by `except BaseException`**, not only `OSError`, so `KeyboardInterrupt` during readback routes into the same recovery.
3. **The pre-state read is inside the guard**, so a read error fails closed *before* publication instead of bypassing recovery.
4. **Recovery's diagnostic re-read is best-effort** (`safe_sha`, never raises); a failure there is recorded as `UNVERIFIABLE` and recovery continues with the remaining paths.

## 3. Results — 21 tests, 21 PASS, 0 FAIL

| Test | Result | Observed |
| --- | --- | --- |
| `environment.same-device-as-live-ops` | PASS | `exfat`; scratch `st_dev` = live Ops `st_dev` |
| `setup.replica-byte-identical` | PASS | 50/50, 0 mismatches |
| `recovery.backup-50` | PASS | identical |
| `batch.receipt-dir-sync-eio` (+ no-tmp) | PASS | 6 booked, 6 `RESTORED`, pre-state exact |
| `batch.receipt-readback-eio` (+ no-tmp) | PASS | 6 booked, 6 `RESTORED` |
| **`batch.receipt-readback-SIGINT`** (+ no-tmp) | PASS | injected `KeyboardInterrupt` during readback → 6 booked, 6 `RESTORED` |
| **`batch.pre-state-read-eio-does-not-bypass-recovery`** | PASS | fails closed at index 4; prior 3 restored; tree exact |
| `batch.restore-failure-reported-and-continues` | PASS | 1 `RESTORE-ERROR`, 5 `RESTORED` |
| `batch.restore-failure-state-explicit` | PASS | failed path keeps installed bytes `24a32146…`; recorded, not hidden |
| **`batch.diagnostic-re-read-eio-does-not-stop-recovery`** | PASS | 6 attempts recorded; 5 restored; 1 `UNVERIFIABLE` |
| `batch.repair` | PASS | explicit repair returns baseline |
| `batch.synthetic-abort-recovers` | PASS | 3 → 3 restored |
| `install.full-six-success` | PASS | 6/6 |
| `install.protected-unchanged` | PASS | board/lanes/`environment.sh`/`AGENTS.md` untouched |
| `recovery.full-rollback` | PASS | 6/6 `RESTORED` |
| `recovery.drift-refusal` | PASS | drifted path refused |
| `final.50-file-baseline` | PASS | 50/50 |
| `custody.guard-exclusion` | PASS | held flock blocks a second acquirer |

## 4. What cannot be closed, stated plainly

**SIGKILL, `SIGSTOP`, kernel panic and power loss cannot be handled by any in-process code.** No further harness iteration can close that class; claiming otherwise would be dishonest. The mitigation is procedural, and the dossier already mandates it:

- a verified off-volume backup of all 50 files **before** the first write;
- after any uncatchable interruption, treat published state as unknown, re-observe the filesystem, and reconcile installed hashes against both the backup and the projected set under renewed exclusive custody before continuing or restoring;
- never resume a partially-known install from memory.

Also unproven here: the live directory's cluster history and free space, the live board flock and Unity lease, Git/worktree behaviour, and real load. Directory `fsync` succeeding on this kernel must not be assumed elsewhere.

## 5. Live Ops state

Read-only at 14:16Z, 14:28Z, 14:34Z, 14:45Z, 14:53Z, 15:00Z and 15:08Z: 50/50 hashes OK, board `2b99552c…`, lanes `c17903dd…`, receipt `8c66b290…`, no `lock.json`, no flock on `08:32`, no Unity process, nothing modified since 12:52:39Z.

## 6. Evidence

`/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v6/` — `exfat-qualification-v6.json` (`77e5ed4ef4816f8855067a6b7d91d8eed083964a35c27319161337e8b2eb017c`), `exfat_qual_v6.py` (`d75a218212cedca21f7fa5b93b81181c965aa91487afb782a37b751c61e8eec3`), `SHA256SUMS`. v1–v5 retained.

## 7. Status

No installation, no board reservation, no Unity, no game source, no Windows, no main merge. The owner's **NO LIVE INSTALL** order remains operative.
