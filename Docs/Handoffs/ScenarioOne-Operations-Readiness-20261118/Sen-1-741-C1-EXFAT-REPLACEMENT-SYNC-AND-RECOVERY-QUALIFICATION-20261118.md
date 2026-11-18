# Sen-1 #741-C1 — exFAT replacement, sync and recovery qualification (v2)

**STATUS: QUALIFICATION EXECUTED AND PASSED 14/14. SCRATCH ONLY. LIVE OPS UNTOUCHED. NOT AN INSTALL PERMISSION.**

**Date:** 2026-11-18, 14:32–14:34 UTC (v2 run). v1 ran at 14:25Z and is superseded.
**Author:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), provider `opencode`, T3 Code / OpenCode, host `jer-laptop`, Linux user `jer`, orchestrator of [#741](https://github.com/jermur788/CCF/issues/741).
**Independence:** this work is **not** independent of the orchestration — I wrote and ran it. It exists to supply evidence for the independent reviewer's blocking finding, which is why the run is scripted, self-asserting and reproducible, and why the reviewer is asked to assess it. I do not declare the finding closed on my own authority.

## 1. Why this exists

The independent nonauthor review (subtask C) blocked on: *"the future installer still needs exFAT write, sync and recovery qualification"*. Subtask B rehearsed on ext4, but the real target `/media/jer/ZX20/CCF-Ops` is exFAT, where modes are not representable and rename/durability semantics are filesystem-specific. Live Ops may not be written, but the **same device and mount** are available as designated scratch under `/media/jer/ZX20/CCF-Temp`.

A first run (v1, `Sen-1-741-C1` at commit `98223fd4`) was rejected on re-check for four fair reasons, all fixed here:

| v1 defect | v2 correction |
| --- | --- |
| changed-directory `sync` was a side probe, not part of the install path | every replace now does `tmp` → file `fsync` → `rename` → **directory `fsync`** → re-hash |
| the drift probe did not execute the rollback code, it re-stated the comparison inline | one shared `rollback_path()` function is used by failure recovery **and** drift refusal |
| recovery from an ordinary I/O error was not exercised | an injected `ENOSPC` during the write phase is now tested |
| the PASS predicate ignored the concurrent-reader observations | PASS now *requires* both complete versions to be observed, no other size/hash pair, no `ENOENT`, ≥4 samples |
| C1 claimed the script "never opens a path outside its scratch root" | corrected: it reads the replica, the staged tree and the baseline inventory, read-only, and `stat()`s live Ops only to prove device identity |
| the log carried no contemporaneous mount snapshot | the log records `findmnt` output and the `st_dev` of both the scratch root and live Ops |

## 2. Method

- Scratch root: `/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV2-20261118T143224Z` (disposable scratch, not durable evidence).
- **Same filesystem proven, not assumed:** `findmnt` line `/media/jer/ZX20 exfat rw,relatime,uid=1000,gid=1000,fmask=0022,dmask=0022,iocharset=utf8,errors=remount-ro`; `st_dev` of the scratch root **2098** equals `st_dev` of `/media/jer/ZX20/CCF-Ops` **2098**.
- Base tree: byte-identical copy of B's 50-file replica; verified against the accepted live baseline inventory — **50 expected / 50 present / 0 mismatches**.
- Projected bytes from B's staged tree: `8fa0c5ad` / `1bd03f15` / `e44e7eb8` / `24a32146` / `3baf029e`.
- Evidence: `/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v2/` — `exfat-qualification-v2.json` (`4d059654…`), `exfat_qual_v2.py` (`ff4b178c…`), `SHA256SUMS`. The v1 run is retained alongside for audit.

## 3. Results — 14 tests, 14 PASS, 0 FAIL

| Test | Result | Observed |
| --- | --- | --- |
| `environment.same-device-as-live-ops` | PASS | exfat mount; scratch `st_dev` 2098 = live Ops `st_dev` 2098 |
| `setup.replica-byte-identical-to-live-baseline` | PASS | 50 / 50, 0 mismatches |
| `atomic.replace-with-directory-sync` | PASS | 8 MiB → 6 MiB via tmp → fsync → rename → **dir fsync** → re-hash. 18 reader samples: 4 of the complete old version, 14 of the complete new one, **no other size/hash pair**, no `ENOENT`, no tmp residue |
| `recovery.io-error-during-write-target-unchanged` | PASS | injected `ENOSPC` (errno 28) during write: target bytes **unchanged**, rename never reached. **A `.tmp` is left behind by the failed write** — the installer must delete it and abort |
| `recovery.io-error-cleanup` | PASS | tmp removed |
| `recovery.backup-50` | PASS | backup identical to pre-state |
| `install.injected-failure` | PASS | failure after exactly **3** replacements (`scripts/unity-lock`, `tests/self-test.py`, `roles/unity-operator.md`) |
| `install.only-three-changed` | PASS | exactly those 3 paths changed |
| `recovery.rollback-restores-exactly-three` | PASS | 3 `RESTORED`, 0 refused, via the shared rollback function |
| `recovery.exact-restoration` | PASS | all 50 files back to pre-state hashes |
| `recovery.drift-refusal-through-real-function` | PASS | installed bytes then drifted: the **function itself returned `REFUSED`** and left the drifted file untouched |
| `cleanup.drift-probe-restored` | PASS | probe restored |
| `final.50-file-baseline` | PASS | 50 files, 0 differing |
| `custody.guard-exclusion-mechanism` | PASS | with an exclusive `flock` held on a guard file, a second non-blocking acquirer **failed**; after release it **succeeded** — the same lock primitive the Unity guard uses |

## 4. What this does and does not prove

**Proves, on the real exFAT device and mount options:** the exact install primitive publishes atomically (a concurrent reader never observes a partial or missing file); a directory `fsync` after `rename` is accepted here; a failed write never reaches `rename` and leaves a `.tmp` that must be cleaned; a mid-install failure after three files is recoverable to byte-exact pre-state; the rollback function refuses a drifted path instead of overwriting it; and the guard-flock primitive genuinely excludes a second holder.

**Does not prove:** behaviour under real power loss or kernel crash (no block-layer fault injection); the live directory's own cluster history or free space; the live board flock or the live Unity lease (never touched); Git/worktree behaviour; performance under real load. Directory `fsync` succeeding here must not be assumed on a different kernel or mount.

**Consequences for the installer, now evidence-based:**
1. Keep the primitive: same-directory `tmp` → file `fsync` → `rename` → directory `fsync` → immediate re-hash.
2. On any write error, delete the `.tmp` and abort; never rename a partially written file.
3. Rollback only when current bytes equal the expected new hash; otherwise refuse that path and stop.
4. Hold an exclusive `flock` on `state/unity/guard` for the whole install-and-recovery window — this is enforced exclusion, not a check, because a second holder cannot acquire it.
5. Modes are not representable on exFAT (`chmod` is a no-op): the receipt must record bytes and owner.

## 5. Live Ops state around these runs

Read-only observations at 14:16Z, 14:28Z and 14:34Z: 50/50 installed files OK against the baseline inventory, `board/active.md` `2b99552c…`, `lanes.json` `c17903dd…`, `deployment.json` `8c66b290…`, no `state/unity/lock.json`, no `/proc/locks` entry for device `08:32`, no Unity process, and no file in live Ops modified since 12:52:39Z. The only access to live Ops from this work was `stat()`/`findmnt`/hash reads.

## 6. Status

- The reviewer's blocking finding is addressed **with executed evidence**; the reviewer's own assessment is the next step and is the only thing that can close it.
- **No installation, no board reservation, no Unity, no game source, no Windows, no main merge.**
- The owner's **NO LIVE INSTALL** order remains operative.
