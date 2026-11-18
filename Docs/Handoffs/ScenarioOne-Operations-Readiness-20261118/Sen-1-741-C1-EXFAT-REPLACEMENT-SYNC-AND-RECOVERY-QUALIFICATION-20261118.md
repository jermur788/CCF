# Sen-1 #741-C1 — exFAT replacement, sync and recovery qualification (v3)

**STATUS: QUALIFICATION EXECUTED AND PASSED 16/16. SCRATCH ONLY. LIVE OPS UNTOUCHED. NOT AN INSTALL PERMISSION.**

**Run:** `/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV3-20261118T144412Z`, 2026-11-18 **14:44:12–14:44:17 UTC** (exact values taken from the log's own `root`/`started_utc`/`finished_utc` fields, not from memory).
**Author:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), provider `opencode`, T3 Code / OpenCode, host `jer-laptop`, Linux user `jer`, orchestrator of [#741](https://github.com/jermur788/CCF/issues/741).
**Independence:** **not** independent — I wrote and ran it. The independent reviewer supplies the verdict; this document supplies reproducible evidence and states its own limits.

## 1. History of this evidence (kept visible, not rewritten)

| Revision | Result | Rejected for |
| --- | --- | --- |
| v1 (`T142559Z`) | 13/13 PASS | dir sync only a side probe; drift probe never ran the rollback code; no I/O-error path; PASS ignored reader samples; over-broad "scratch-only" claim; no mount snapshot |
| v2 (`T143503Z`) | 14/14 PASS | a directory-sync `EIO` **after rename still produced PASS**; recovery after publication unqualified; receipt not written with the same primitive; drift probe used a file whose staged bytes equalled baseline; sampler used a separate `stat` and a truncated digest |
| **v3 (this run)** | **16/16 PASS** | — |

Both earlier logs and scripts remain in the evidence folder. Nothing was deleted, and no earlier claim is presented as having been true.

## 2. What v3 adds, mapped to the objections

1. **Directory-sync failure after rename is now fatal, not silent.** `replace_file()` returns `dir_sync_ok`; on `False` it raises `PublicationUnproven`. The install must treat that as *published-but-not-durable* and recover.
2. **Recovery after publication is exercised**: a file is renamed into place, the directory `fsync` is forced to fail with `EIO`, and the tree is then restored byte-exactly from backup.
3. **The receipt is written and rolled back with the same primitive** as the five code files.
4. **Drift refusal uses a file that genuinely changes** (`scripts/_ccf.py`: baseline `ab4343ef…` → projected `24a32146…`), so the refusal cannot be satisfied by an unchanged file.
5. **Reader sampling opens once**, `fstat()`s that descriptor and hashes the full stream with a full 64-hex digest — no separate pathname `stat`, no truncation.
6. **Mount identity is proven in the log**: `findmnt` output plus `st_dev` of the scratch root and of live Ops.

## 3. Results — 16 tests, 16 PASS, 0 FAIL

| Test | Result | Observed |
| --- | --- | --- |
| `environment.same-device-as-live-ops` | PASS | `exfat`; scratch `st_dev` **2098** = live Ops `st_dev` **2098** |
| `setup.replica-byte-identical-to-live-baseline` | PASS | 50 expected / 50 present / 0 mismatches |
| `atomic.replace-with-directory-sync` | PASS | 8 MiB → 6 MiB; `dir_sync_ok = true`; 18 samples = 4× complete old + 14× complete new, **no other pair**, no `ENOENT` |
| `sync.dir-sync-eio-detected` | PASS | forced `EIO` after rename → `PublicationUnproven` raised; the file **had** changed (`e44e7eb8…`), so the failure was detected after real publication, not before |
| `recovery.after-unproven-publication` | PASS | restored to `120f1deb…` (baseline) |
| `recovery.after-unproven-publication-tree` | PASS | all 50 files back to baseline |
| `receipt.write-with-same-primitive` | PASS | receipt `ee13a811…`, `dir_sync_ok = true` |
| `receipt.rollback-exact` | PASS | restored to `8c66b290…` |
| `install.injected-failure` | PASS | failure after exactly 3 replacements |
| `install.only-three-changed` | PASS | exactly those 3 paths changed |
| `recovery.rollback-restores-exactly-three` | PASS | 3 `RESTORED` |
| `recovery.exact-restoration` | PASS | 50 files, 0 differing |
| `recovery.drift-refusal-changed-file` | PASS | `_ccf.py` installed as `24a32146…`, then drifted → rollback **REFUSED**, drifted file left in place |
| `cleanup.drift-probe-restored` | PASS | probe restored |
| `final.50-file-baseline` | PASS | 50 files, 0 differing |
| `custody.guard-exclusion-mechanism` | PASS | held `flock` blocks a second acquirer; succeeds after release |

## 4. What this does and does not prove

**Proves on the real exFAT device and mount:** atomic publication to a concurrent reader; directory `fsync` accepted; a post-rename directory-sync failure is detectable and recoverable to byte-exact pre-state; a failed write never reaches `rename` (and leaves a `.tmp` that must be deleted); compare-before-write rollback restores exactly and refuses drift through the same code path; the receipt obeys the same primitive; the guard lock genuinely excludes a second holder.

**Does not prove:** real power loss or kernel crash (no block-layer fault injection, and doing so safely is impossible without jeopardising the live tree); the live directory's cluster history or free space; the live board flock or live Unity lease; Git/worktree behaviour; real concurrent load. Directory `fsync` succeeding here must not be assumed on another kernel or mount.

**Accepted residual risk, stated as such:** power loss or kernel crash during the deploy window is a **residual risk carried as a deploy-window condition**, not a pre-deploy gate — an off-volume verified backup must exist before the first write, and after any interruption the filesystem and installed hashes must be reconciled under renewed custody before anything continues. This is the reviewer's own framing and I am not waiving it: it is recorded in the dossier as a mandatory condition.

## 5. Live Ops state around these runs

Read-only observations at 14:16Z, 14:28Z, 14:34Z and 14:45Z: 50/50 installed files OK against the baseline inventory, `board/active.md` `2b99552c…`, `lanes.json` `c17903dd…`, `deployment.json` `8c66b290…`, no `state/unity/lock.json`, no `/proc/locks` entry for device `08:32`, no Unity process, and no file in live Ops modified since 12:52:39Z. The only access to live Ops was `stat()`, `findmnt` and hash reads.

## 6. Evidence

`/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v3/`
`exfat-qualification-v3.json` (`d00735dd3ab56cc307efb48db51f3d0d75345396ba550d09119ff5b1628f7de6`), `exfat_qual_v3.py` (`486e4b7ca137ae231ca3a8bd77313a5bcff48ba45fad93dfadd1a2c9b3556fe3`), `SHA256SUMS`. v1 and v2 retained in sibling folders.

## 7. Status

- Evidence supplied for the reviewer's blocking finding and its named gap; only the reviewer can close it.
- **No installation, no board reservation, no Unity, no game source, no Windows, no main merge.**
- The owner's **NO LIVE INSTALL** order remains operative.
