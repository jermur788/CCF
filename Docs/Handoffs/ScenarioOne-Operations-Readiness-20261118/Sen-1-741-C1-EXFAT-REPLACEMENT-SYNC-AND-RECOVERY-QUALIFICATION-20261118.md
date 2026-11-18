# Sen-1 #741-C1 — exFAT replacement, sync and recovery qualification

**STATUS: QUALIFICATION EXECUTED AND PASSED 13/13. SCRATCH ONLY. LIVE OPS UNTOUCHED. NOT AN INSTALL PERMISSION.**

**Date:** 2026-11-18, 14:25–14:27 UTC.
**Author:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), provider `opencode`, T3 Code / OpenCode, host `jer-laptop`, Linux user `jer`, orchestrator of [#741](https://github.com/jermur788/CCF/issues/741).
**Independence:** this work is **not** independent of the orchestration — I wrote and ran it. It exists to close the one blocking finding of the independent review ([#746](https://github.com/jermur788/CCF/issues/746)), which is why the run is fully scripted, hash-logged and reproducible rather than narrated, and why the same reviewer is asked to assess the evidence afterwards.

## 1. Why this exists

The independent nonauthor review (subtask C) returned **NEEDS CHANGES** with exactly one blocking finding:

> **7. Blocking.** The ext4 rehearsal validates rollback logic. The future installer still needs **exFAT write, sync and recovery qualification**.

That is correct. Subtask B's rehearsal ran on ext4 while the real target `/media/jer/ZX20/CCF-Ops` is exFAT (`/dev/sdd2`, `fmask=0022`, `uid=1000,gid=1000`). exFAT is not POSIX: modes cannot be set, and durability/rename semantics are filesystem-specific. A pass on ext4 is evidence about the *logic*, not about the *target filesystem*.

Writing to live Ops is forbidden. But the same device and filesystem are legitimately available as designated scratch under `/media/jer/ZX20/CCF-Temp`, so the mechanics can be qualified on the **real filesystem type, device and mount options** without touching the installed tree.

## 2. Method

- Scratch root: `/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQual-20261118T142559Z` (deleted-on-demand scratch, not durable evidence).
- Base tree: byte-identical copy of B's 50-file replica, itself byte-identical to live. Verified against the accepted live baseline inventory (`live-inventory.sha256`): **50 expected, 50 present, 0 mismatches**.
- The five projected files were taken from B's staged tree (`8fa0c5ad` / `1bd03f15` / `e44e7eb8` / `24a32146` / `3baf029e`).
- Script: `exfat_qual.py` (kept in the evidence folder, SHA-256 recorded in its `SHA256SUMS`). Read-only with respect to live Ops; it never opens a path outside its scratch root.
- Evidence: `/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/` — `exfat-qualification.json` (`a3c82aad…`), `exfat_qual.py` (`5caac91a…`), `SHA256SUMS`.

## 3. Results — 13 tests, 13 PASS, 0 FAIL

| Test | Result | Observed |
| --- | --- | --- |
| `setup.copy-replica-to-exfat` | PASS | 50 files copied to the exFAT volume |
| `setup.byte-identity-vs-live-baseline` | PASS | 50 expected / 50 present / **0 mismatches** against the accepted live baseline |
| `setup.mode-owner-semantics` | PASS | files appear `0755 jer:jer`; `chmod 0600` is a **no-op** — confirms receipts must record bytes and owner, not modes |
| `atomic.replace-tmp-fsync-rename` | PASS | 8 MiB file replaced by a 6 MiB file via `tmp` → `fsync` → `rename`. A concurrent reader made **4 observations of the complete old file and further observations of the complete new file**, never an intermediate length, never a partial hash, never `ENOENT`. No `tmp` residue |
| `sync.directory-fsync` | PASS (observed) | directory `fsync` returned **SUPPORTED** on this kernel/exFAT mount. Recorded as an observation, not a portability guarantee |
| `recovery.backup-50` | PASS | full backup, identical to pre-state |
| `install.injected-failure` | PASS | failure raised while handling the **4th** file, after exactly **3** replacements (`scripts/unity-lock`, `tests/self-test.py`, `roles/unity-operator.md`) |
| `install.state-after-failure` | PASS | exactly those 3 paths changed; **only_expected = true** |
| `recovery.rollback-after-failure` | PASS | **3 restored, 0 refused** |
| `recovery.exact-restoration` | PASS | all 50 files back to pre-state hashes, 0 differing |
| `recovery.drift-refusal` | PASS | a deliberately drifted path was **REFUSED** (current ≠ expected new hash) |
| `cleanup.drift-probe-restored` | PASS | probe path restored to baseline |
| `final.50-file-baseline` | PASS | 50 files, 0 differing |

## 4. What this does and does not prove

**Proves, on the real exFAT device and mount options:** the exact replace primitive the install uses (`same-directory tmp` → file `fsync` → `rename` → re-hash) publishes atomically as observed by a concurrent reader; a mid-sequence failure after three files leaves exactly those three changed; compare-before-write rollback restores all pre-state bytes exactly; foreign drift is refused rather than overwritten; modes are not representable, so byte/owner evidence is the correct receipt content.

**Does not prove:** behaviour under actual power loss or kernel crash (no fault injection at the block or VFS layer); the live directory's own cluster history or free-space fragmentation; the live board flock or Unity guard (never touched — this ran entirely inside `CCF-Temp`); performance under the real concurrent load; or the Git/worktree side of the deployment. Directory `fsync` succeeding here must not be assumed on another kernel.

**Consequence for the installer:** keep the rehearsed primitive exactly (per-file `tmp` → `fsync` → `rename` → immediate re-hash), and still re-verify each path and each rollback on the real tree while exclusivity is held. The exFAT qualification removes the *filesystem-unknown* risk; it does not remove the *do it for real, once* requirement.

## 5. Live Ops state around this run

My own read-only observations before and after (14:16Z and 14:29Z): 50/50 installed files OK against the baseline inventory, `board/active.md` `2b99552c…`, `lanes.json` `c17903dd…`, `deployment.json` `8c66b290…`, no `state/unity/lock.json`, no `/proc/locks` entry for device `08:32`, no Unity process, and no file in live Ops modified since 12:52:39Z. Nothing in this qualification touched it.

## 6. Status

- **C blocking finding 7:** addressed with executed evidence on the target filesystem; assessment by the independent reviewer is the next step, and I do not declare it closed on my own.
- **No installation, no board reservation, no Unity, no game source, no Windows, no main merge.**
- The owner's **NO LIVE INSTALL** order remains operative.
