# Sen-1 #741-B — five-file staging, prospective receipt and rollback rehearsal (ISOLATED SCRATCH ONLY)

**RESULT: 11/11 PROCEDURES PASS. Harness limits are listed as NOT TESTED in §13. NO STOP CONDITION HIT.**
**NOTHING WAS INSTALLED. No write, chmod, copy or flock touched `/media/jer/ZX20/CCF-Ops`. No board, lanes, state, ledger, guard or history was written anywhere live. No Unity, no Windows, no source edit.**
**The prospective receipt is a TEST_FIXTURE_ONLY artefact. It is not installed provenance.** This is author evidence. The independent review is subtask C's.

- **Date:** 2026-11-18, about 13:48–14:15 UTC.
- **Issue:** [#745](https://github.com/jermur788/CCF/issues/745), child of [#741](https://github.com/jermur788/CCF/issues/741), subtask B. [RUNNING registration](https://github.com/jermur788/CCF/issues/745#issuecomment-6980492597) was posted before any write.
- **Controls read in full:**
  - Manager #731 five-file staging decision.
  - #652 read-only preflight `8506db60`.
  - #652 accepted source result `4c7acc05`.
  - Orchestrator dossier §4 and §7.

## 1. Actor, separation and custody

- **Actor:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code (Agent SDK) inside T3 Code, acting as the implementation sub-agent. Session `3ed601e7-2f1e-4a7f-85e4-330d41763715`, host `jer-laptop`, user `jer`.
- **Separation:**
  - This session is not the #652 source writer (Codex `9bcdfde6…`), not its reviewer (Sonnet `5b89dd08…`), and not the preflight custodian (`a368d8d5…`). It did not write subtask A's README.
  - It reviews nothing of its own.
- **Branch:** `docs/sen-1-741-b-stage-rehearsal-and-rollback-20261118-v1`, starting HEAD `e8e84027faa45d42316cd01bda34ad36393fd069` (= `origin/main`).
- **Checkout deviation (disclosed in RUNNING):** the dispatch said the worktree was pre-created. At 13:48Z neither the path nor the branch existed. I created exactly the declared worktree, `/media/jer/Files/CCF/Operations/CCF-741-B-StageRehearsal-20261118`, from `e8e84027`. No other checkout was made.
- **Scratch/evidence root (mine alone):** `/media/jer/Files/CCF/Verification/Sen1-741-B-StageRehearsal-20261118/`. `SHA256SUMS` there covers every file.

## 2. Inputs consumed (exact bytes)

| Input | Hash | Notes |
|---|---|---|
| A2 attachment `Sen-1-741-A2-MERGED-INSTALLED-README-v2-ORCHESTRATOR-REPAIR.proposed-NOT-DEPLOYED.md` @ `052ae112a1707830dd2ed482f936d3b6cff878c4` | `233071cb5c218e6bd9fccd8d69771ba0544580fd1acf5688518f29a827635a98` | Read from the git object. Not modified |
| **Merged README v2 bytes**, extracted with `sed '1,/^<!-- BEGIN-PROPOSED-README-BYTES -->$/d'` | **`3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123`** (16,549 B) | Equals the required hash. The same extraction from A's checkout gives the same hash. A's v1 (`bde3c6cf…`) was not used or touched |
| Source `ops/sen-1-652-unity-lease-readiness-20261116-v1` @ `4c7acc051e262c3fe301049b51ee7fc48a9f99a2` | see §4 | `git archive` into scratch. The worktree is clean (including ignored files) and HEAD = origin. `4d03f5b5..4c7acc05` changes nothing under `CCF-Ops/` |

## 3. Procedure results (PASS/FAIL)

| # | Procedure | Result | Key observed numbers |
|---|---|---|---|
| 1 | Identity and provenance | **PASS** | ZX20 `/dev/sdd2` exfat UUID `18D6-896E` rw at `/media/jer/ZX20`, by-id pin → `/dev/sdd2`, 53,034,745,856 B free. Live `sha256sum -c` against the preflight inventory: **50/50 OK**. board `2b99552c…`, lanes `c17903dd…`, receipt `8c66b290…`. All 50 files `755 jer:jer`. No Unity argv0 process, no lock on dev `08:32` in `/proc/locks`, no `lock.json`, no process with cwd or open fd in live Ops. Source clean at `4c7acc05` = origin |
| 2 | Isolated replica | **PASS** | `replica/CCF-Ops`: 50 files, bytes and mode identical to live. Live inventory identical before and after the copy |
| 3 | Exact five-file projection | **PASS** | staged vs replica: **5 changed + receipt, 0 added, 0 removed, 44 unchanged**. `environment.sh` `3775cba7…` and `AGENTS.md` `d1ce3ad7…` retained. `scripts/verify-deployment` and `DeploymentProvenance.md` ABSENT. 50 files before and after (no sixth executable). Import delta is only stdlib names (`runpy`, `shutil`, `unittest.mock.patch`) plus `_ccf` names in self-test |
| 4 | Retained live `environment.sh` compatibility | **PASS** | Every name the staged scripts and `environment.sh` import from candidate `_ccf` exists: unity-lock 8, lane-status 10, packet-lint 5, self-test 11, environment.sh 3; **0 missing**. `source environment.sh` gives `PASS: /dev/sdd2 rw …`, exit 0. `lane-status --storage-only --json` PASS, exit 0. `lane-status --json`: 23 lanes, exit 0. `packet-lint valid.md` PASS. `unity-lock status --test-state-dir <absent CCF-Temp path>` gives `free`, exit 0, nothing created. Staged tree combined hash unchanged (`ce9d4818…`). **No incompatibility; no STOP** |
| 5 | Original full suite from staged root | **PASS** | `cd <staged>/CCF-Ops && python3 -B tests/self-test.py` 14:01:23Z–14:08:32Z: **234 PASS, 0 FAIL, exit 0**, `result: PASS`, `production_lock_acquired: false`, `unity_launched: false`. **234, not 233:** the PASS-label diff against the accepted source run `files-full-1.log` (`b9c59507…`, 233) differs only in random fixture names and a timestamp, plus **one extra data-driven check**, `stored ledger schema: 2026-11-12-CCF-412-D073-FR10-ONE-GO.md`. That live-only ledger entry is absent from the source tree, and the suite adds one check per stored ledger file. Both race checks pass (`exactly one guarded winner`, `exactly one winner`). Staged tree identical before and after (50 files) |
| 6 | Original board lint on replica board | **PASS** | Staged original `packet-lint` (`a776ec42…`, retained byte-identical) `--board <staged>/board/active.md` (`2b99552c…`, replica): **`PASS: board schema and duplicate checks (42 live tasks)`**, exit 0 |
| 7 | Prospective receipt verifier | **PASS** | Source `verify-deployment` (`ee001216…`) `--root <staged> --manifest <receipt> --candidate-root <staged>`: **PASS (30 source hashes, 5 pinned overrides, 1 config projection, 5 live-state), candidate PASS (5)**, exit 0. Negatives all FAIL exit 1 as expected (§6) |
| 8 | Backup, atomic replace, injected failure, rollback | **PASS** | 50-file byte/mode/owner backup, readback identical. Failure injected **after file 3** (`tests/self-test.py`). Rollback **restored 3**, already-old-no-write 3, **refused 0**, installer `.bak` removed. Post-rollback **exact**: 50/50 bytes, mode, uid, gid and size; protected inode/mtime/sha unchanged; 0 `.tmp` left. Full 6-path install, then verifier PASS, then full rollback: restored 6, exact |
| 9 | Negatives | **PASS** | Foreign drift on `roles/unity-operator.md` → **REFUSED_FOREIGN_DRIFT**, the other 5 restored, exit 5, verify reports exactly that path. Replica board-dir flock held → install and rollback both refused, exit 1, tree hash unchanged. Candidate guard held → `status` blocks (timeout exit 124) and completes after release. Suite race checks pass (procedure 5) |
| 10 | Lock compatibility and rollback refusal rule | **PASS** | Reader matrix identical to the preflight (§8). Held `no_expiry:true` record → rollback **WITHHELD** (exit 4), 0 writes, tree hash unchanged. Wrong-owner release refused. Explicit matching release → `released`, `forced:false`. Rollback then restored 6. History = exact pre-state prefix + 1 `release` line. Legacy ISO record → rollback allowed, record byte-identical, old reader READ_OK |
| 11 | Risk register | **PASS (stated)** | §12 |

## 4. Exact projection (old live → new staged)

| Path | Old/live SHA-256 | New staged SHA-256 | Origin | Previous receipt class |
|---|---|---|---|---|
| `scripts/unity-lock` | `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5` | `8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8` | `4c7acc05:CCF-Ops/scripts/unity-lock` | source_hashes |
| `tests/self-test.py` | `2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3` | `1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50` | `4c7acc05:CCF-Ops/tests/self-test.py` | source_hashes |
| `roles/unity-operator.md` | `120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815` | `e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db` | `4c7acc05:CCF-Ops/roles/unity-operator.md` | source_hashes |
| `scripts/_ccf.py` | `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001` | `24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435` | `4c7acc05:CCF-Ops/scripts/_ccf.py` | deployed_overrides |
| `README.md` | `249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517` | `3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123` | A2 @ `052ae112`, bytes after marker | deployed_overrides |
| `deployment.json` (receipt, sixth mutable path) | `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18` | `ee13a811304c4cbfced23c4969b2a94d970d22fc5d424b72fa24dd2a943ad216` | generated (§5) | — |

- **Retained, byte-identical in staged:** `environment.sh` `3775cba7…`, `AGENTS.md` `d1ce3ad7…`, `lanes.json` `c17903dd…`, `board/active.md` `2b99552c…`, `scripts/lane-status` `e7102a83…`, `scripts/packet-lint` `a776ec42…`, every ledger/state file, and the other 34 unchanged files.
- **Not installed:** `scripts/verify-deployment`, `DeploymentProvenance.md`.
- **Never copied:** source `deployment.json` `be9fbcd0…`.
- **Modes:** every replica/staged file is `0755`, mirroring live exFAT `fmask=0022`. Rehearsal writes keep the existing mode.

## 5. Prospective receipt

- **Attachment:** `attachments/Sen-1-741-B-PROSPECTIVE-deployment.proposed-NOT-INSTALLED.json`, SHA-256 **`ee13a811304c4cbfced23c4969b2a94d970d22fc5d424b72fa24dd2a943ad216`**.
- **Generated by:** `tools/make_receipt.py` from the **live** receipt bytes `8c66b290…` (asserted). It was **not** generated from the source receipt.

What it contains:

- **Historical baseline kept:** `source_head` `dfe445e54d099a0e20c9c7d5a9e3e6f195968b4e`, `deployed_at` `2026-10-15T00:20:00+00:00`, `source_branch`, `source_repository`, `deployment_target`, schema 2.
- **Five projected files:**
  - `_ccf.py` and `README.md` moved from `deployed_overrides` into `source_hashes`.
  - `unity-lock`, `self-test.py` and `unity-operator.md` updated in place in `source_hashes`.
  - All five are pinned to the exact installed bytes. Result: 28 → **30** source hashes and 7 → **5** overrides.
- **Still-pinned overrides:** `AGENTS.md` `d1ce3ad7…`, `environment.sh` `3775cba7…`, `scripts/lane-status` `e7102a83…`, `G4WorkflowFoundationReviewPacket.md`, `WorkflowFoundationHandoff.md`. Their reasons are unchanged.
- **Config:** `configuration_hashes.lanes.json.sha256_without_lanes` = `ed642a52…` is unchanged (asserted equal).
- **Fixture markers:**
  - `record_type: PROSPECTIVE-RECEIPT-TEST_FIXTURE_ONLY-NOT-INSTALLED`
  - `test_fixture_only: true`
  - `provenance_status: TEST_FIXTURE_ONLY / NOT_DEPLOYED / …`
  - `recorded_at: null`
  - `projection.deployment_status: NOT_DEPLOYED`, `projection.deployed_at: null`, `projection.install_record: null`
  - `candidate.deployment_status: NOT_DEPLOYED`, `candidate.deployed_at: null`
- **Only timestamp in the file:** the historical `2026-10-15T00:20:00+00:00` (regex scan). **No future or current time is presented as an install time.**
- **New `projection` block:** records per-file origin, previous class, retained overrides, not-installed and never-copied files.
- **`candidate` block:** names source head `4c7acc05`, reviewed commit `4d03f5b5`, README commit `052ae112`, and the five hashes.
- **`previous_manifest_sha256`:** `8c66b290…`, with a note that a real installer must back it up as `deployment.json.bak-<UTC>`.
- **Text change (disclosed):** the `live_state["lanes.json#/lanes"]` reason no longer says "16 entries", which is stale. It now says 23 were observed and that lane registrations are operational custody state. The key set is unchanged, as the verifier requires.
- **Judgement for review:** the README is a deployment-only document, not source. Putting it in `source_hashes` follows the #745 brief. Its true origin is stated in `projection.files["README.md"].origin`.

## 6. Verifier evidence (`p7/`)

| Run | Exit | Output |
|---|---|---|
| V1 prospective → staged, candidate-root staged | 0 | PASS 30/5/1/5; candidate PASS (5) |
| N1 prospective → pre-install replica | 1 | FAIL on exactly the 5 projected paths |
| N2 live receipt `8c66b290` → staged | 1 | FAIL on exactly the 5 projected paths |
| N3 stale source receipt `be9fbcd0` → staged | 1 | FAIL on the 5 paths plus 6 candidate mismatches (it expects Option A bytes, `environment.sh` fc1744b2, `verify-deployment` and `DeploymentProvenance.md`) |
| C1 live receipt → replica (control) | 0 | PASS 28/7/1/5, reproducing the preflight live audit on the replica |
| Installed replica (procedure 8 full install), manifest = installed receipt | 0 | PASS 30/5/1/5; candidate PASS (5) |

## 7. Rollback rehearsal design and results (`p8/`, `p9/`, `p10/`)

The driver is `tools/rehearse.py`. Every path is resolved and refused unless it is inside this task's scratch. Live Ops is refused explicitly.

1. **Backup.** Copy all 50 files, then record sha/mode/uid/gid/size/inode/mtime_ns in `manifest.json` and read them back.
   - p8 manifest `9b25cb74…`; p10 manifest `2f9970d9…`.
2. **Exclusivity.** Take a non-blocking `flock -x` on the **replica** `board/` directory. If it is held, refuse with no write.
3. **Precheck.** All six targets must hash to the expected old value. Otherwise STOP.
4. **Receipt backup.** `deployment.json.bak-<stamp>` is written atomically and must hash to `8c66b290…`.
5. **Atomic replace,** in order `_ccf.py`, `unity-lock`, `self-test.py`, `unity-operator.md`, `README.md`, then the receipt:
   - write `NAME.tmp` with `O_EXCL`, write, fchmod to the original mode, fsync;
   - rename, then fsync the directory;
   - re-hash, which must equal the new hash.
6. **Rollback.** Under the same flock, check the lock-state rule (§8). Then, per path:
   - current == new → atomic restore from backup with the original mode;
   - current == old → no write;
   - otherwise → **REFUSE that path (foreign drift)**.
   - Installer-created `.bak` files are removed only if their bytes equal the old receipt.
   - board, lanes, state and ledger are never in the restore set.
7. **Verify.**
   - Full inventory vs backup manifest: bytes, mode, uid, gid, size; no missing or extra files.
   - The 10 protected board/lanes/state/ledger paths keep the same inode, mtime_ns and sha.

Results:

- **Injected failure (p8):**
  - The failure was raised after replace #3 (`tests/self-test.py`). Mid-state: 3 changed plus one `.bak`; protected paths unchanged.
  - Rollback restored `_ccf.py`, `unity-lock` and `self-test.py`, left `unity-operator.md`, `README.md` and `deployment.json` alone (already old) and removed the `.bak`.
  - Final verify **exact** (0 diffs, 0 protected diffs, 0 `.tmp`).
- **Full cycle (p8):** 6 replaced, verifier PASS, rollback restored 6, verify exact.
- **Foreign drift (p9):**
  - After a full install, one byte line was appended to `roles/unity-operator.md`. Rollback refused it (current `287977d7…` ≠ new `e44e7eb8…` ≠ old `120f1deb…`), restored the other 5, and returned exit 5.
  - Verify reported only that path.
  - Cleanup of the deliberate mutation was byte-checked. Afterwards verify was exact.
- **Contention (p9):**
  - Replica board flock held by `flock -x -n … sleep 6`: install and rollback refused, exit 1, replica combined hash `f45385bf…` before = after.
  - Candidate `unity-lock` guard held on a scratch state dir: `status` blocked until the 2 s timeout (exit 124) and returned `free` after the holder exited.

## 8. Lock compatibility (synthetic records only, `p10/`)

`tools/lockread.py` loads `read_state()` from each script without running `main()`. Old = backup `54cf0a95`, candidate = staged `8fa0c5ad`.

| Record | Old reader | Candidate reader |
|---|---|---|
| legacy ISO `eta` | READ_OK | READ_OK |
| `eta:null`, `no_expiry:true` | **MALFORMED: invalid string metadata** | READ_OK |
| `eta:null` unmarked | MALFORMED | MALFORMED: null ETA requires explicit no-expiry mode |
| ISO `eta` + `no_expiry:true` | READ_OK | MALFORMED (the candidate never writes this) |

**Rollback refusal rule as implemented:**

- Restoring the old set is SAFE only if `lock.json` is absent, or the **old** reader reads the current record (legacy). Otherwise rollback is WITHHELD with zero writes.
- **Shown on `p10/replica10`:**
  1. Install the candidate.
  2. Place a synthetic held `no_expiry:true` record, `pid:null`. Candidate status: `held`, eta `None`, no_expiry `True`.
  3. Rollback **WITHHELD** (exit 4), tree hash `fb940e97…` before = after.
  4. Release with the wrong owner: refused, exit 2.
  5. Explicit matching release via the candidate: `released`, `forced:false`. History is the pre-state prefix plus exactly 1 `release` event.
  6. Rollback: SAFE (no `lock.json`), restored 6. Verify exact except the allowed history append.
- **Legacy case:** the rule allowed rollback and the legacy record stayed byte-identical (`4b41df7a…`). The restored old reader returned READ_OK.
- **Never** were force, deletion of a held record, or migration used.
- **How the candidate CLI ran on scratch state:** `unity-lock` hardcodes `DEFAULT=/media/jer/ZX20/CCF-Ops/state/unity` and confines state to ZX20. So it ran via `tools/scratch_unitylock.py`, which replaces `DEFAULT` and `on_storage` in `main.__globals__` with a confinement that refuses live Ops and anything outside this scratch. This is the same patching technique the suite uses for `editors`. Self-check: the wrapper refused `/media/jer/ZX20/CCF-Ops/state/unity` (exit 1).

## 9. Suite side effects (procedure 5)

- **CCF-Temp:** the suite created exactly one directory, `/media/jer/ZX20/CCF-Temp/foundation-test-xhol659x` (26 MB). It does not remove it itself.
  - I preserved `result.json`, `default-caller-matrix.json` and `pilot/` in `p5/suite-scratch-receipts/`.
  - I then removed **only that directory**. The CCF-Temp listing afterwards equals the pre-suite listing (19 entries). No other worker's entry was touched.
- **Files:** the suite created and removed its own `/media/jer/Files/CCF/Verification/ops652-files-fixture-ojg98lxn` in its `finally` block. 0 such fixtures before and 0 after.
- **What it read:**
  - live `lanes.json` and `board/active.md`, copied into its own scratch;
  - live lane worktrees via read-only `lane-status` (`GIT_OPTIONAL_LOCKS=0`, `du`).
- **What it never touched:** every lock command used `--test-state-dir` under its own scratch, and the live guard was never opened.
- **Live tree:** combined hash identical before and after the suite.

## 10. Live tree before and after this task

- **Final read-only recheck at 14:09:24Z:**
  - `sha256sum -c` against the 13:50Z inventory: **50/50 OK**, 50 files, all `755 jer:jer`.
  - No lock on dev `08:32`, no `lock.json` (only `guard`, `history.jsonl`), no Unity process.
  - `scripts/__pycache__/_ccf.cpython-312.pyc` still dated 2026-11-11.
- **Source worktree:** clean (0 entries including ignored), `4c7acc05`.
- **Subtask A checkout:** clean at `052ae112`.
- **Live access summary:** this task only read live bytes. No `unity-lock` was run against live state.

## 11. Observations for the independent reviewer (non-blocking, not STOP)

- **O-1, README (inherited, not introduced):**
  - Merged README line 84 says `self-test.py` "creates fixtures only under ZX20/CCF-Temp".
  - The suite also creates and removes `/media/jer/Files/CCF/Verification/ops652-files-fixture-*` (procedure 5). Its `foundation-test-*` scratch under CCF-Temp is not removed by the suite.
  - The same sentence is in the live README (`249be391…`, line 79) and the reviewed source README. A's bytes were not edited.
- **O-2, receipt:** README sits in `source_hashes` per the brief, although it is a deployment-only document (see §5). The reviewer should confirm this classification is acceptable.
- **O-3, receipt:** the `live_state` lanes reason text was refreshed (16 → 23 observed, non-numeric wording). The key set is unchanged.

## 12. Risk register (procedure 11)

- **R-1, Editor PID binding (known, M-4):**
  - Candidate normal release refuses while the recorded PID exists, including after PID reuse.
  - A launcher or terminal shell PID recorded via `--pid` would block normal release until that shell exits. The only remaining path would be the Manager-approved force route.
  - Operators must pass the **actual Unity Editor PID**, once it is verified alive, or omit `--pid`.
  - The merged README v2 line 48 carries this rule (A2 repair R-1).
  - The personal launcher passes no `--pid` (per preflight §6).
- **R-2, hardcoded live state default:**
  - `unity-lock` `DEFAULT=/media/jer/ZX20/CCF-Ops/state/unity` is absolute, not relative to the script. Running **any** copied or staged `unity-lock` without `--test-state-dir` acts on **live** state. `status` opens the guard with `a+` and takes a flock.
  - Future installers and reviewers must never run a staged copy's `unity-lock` bare.
  - This rehearsal always used `--test-state-dir` (absent CCF-Temp path) or the confinement wrapper.
- **R-3, rollback with a no-expiry lock:** after install, any `no_expiry:true` record makes the old script read MALFORMED. Rollback must be withheld until the Editor/PID has exited and an explicit normal release is done (rule shown in §8). Never force.
- **R-4, partial-failure mixed state:** after the injected failure, the tree briefly held candidate `_ccf.py`/`unity-lock`/`self-test.py` with old docs. Rollback restored it exactly. A real installer must keep exclusivity until rollback or completion, and must not leave a mixed set. No suite run qualified a mixed set.
- **R-5, filesystem fidelity:** the replica is on ext4 (Files), as the scratch grant requires. Live is exFAT. No exFAT rename/fsync atomicity or mode behaviour was rehearsed here. The deploy-day installer must re-hash after each rename, as the procedure does.
- **R-6, old `_ccf` board confinement:** live `_ccf.py` (`ab4343ef`) refuses boards and replica paths outside ZX20. After a rollback, the old tools work only on the live ZX20 tree. This is expected, but it means post-rollback CLI readback cannot be rehearsed on a Files replica (§13).
- **R-7, observation expiry:** all procedure 1 live observations are point-in-time (13:50Z). They must be repeated at actual installer admission, together with a custodian correction of the stale `CCF-652` board row and an exclusive installer reservation.
- **R-8, untested callers:** caller compatibility relies on preflight §6 (tracked Tools and Operations depth 3). Ad-hoc scripts were not covered.

## 13. Not tested / limits

- **Old `packet-lint`/`unity-lock` CLI against the replica** (old `_ccf` refuses non-ZX20 paths: "path escapes ZX20"). Not run. That covers:
  - old-tool board lint on the pre-install replica;
  - the old CLI releasing a preserved legacy record after rollback.

  The old reader's acceptance of the legacy record **was** shown with `read_state` (READ_OK). The live old lint was PASS 42 in the preflight.
- **exFAT behaviour of atomic replace (R-5).**
- No live `unity-lock status/acquire/release`, no live flock, no Unity, no Windows, no installation, and no dashboard/Drive sync.

## 14. Evidence

- **Root:** `/media/jer/Files/CCF/Verification/Sen1-741-B-StageRehearsal-20261118/`, with `SHA256SUMS` over every file.
- **Directories:**
  - `inputs/`: A2 attachment, extracted README, source archive.
  - `p1/`–`p10/`: per-procedure evidence.
  - `tools/`: `build_trees.py`, `compare_trees.py`, `make_receipt.py`, `rehearse.py`, `lockread.py`, `scratch_unitylock.py`.
  - `replica/`, `staged/`: the staged tree, containing the merged README `3baf029e…` and the receipt `ee13a811…`.

`SHA256SUMS` (406 files) SHA-256: `88f9756bce7fcabc48b868d4056e7b2adf2724dee5f5ce906193f3653ade64f7`.

Key artefacts:

| File (under evidence root) | SHA-256 |
|---|---|
| `p5/self-test-staged.log` | `c56484b061c2c1cf9270388fd268e749791ebfa05fdb9d2cb0fd916108fc3cf2` |
| `p5/label-diff-vs-source-run.txt` | `994b3dae99dde49b65e2959f7a755736de2ee1ec152ac490447914988860c1e0` |
| `p7/V1-prospective-vs-staged.txt` | `f5efe35f97c14a5f1ba90747dd525936f9e17d8732308301e988c0e8b09c45c4` |
| `p8/backup/manifest.json` | `9b25cb7493bba0f02f7ca6a268be708751b538f14c3dc33aa31ab4e964e04ad6` |
| `p8/install-fail3.json` | `85f11a848ced4aa39bb0ff76ae56c6b074f677ff7bc4637093fee8bc2728ebe6` |
| `p8/rollback-fail3.json` | `8d59d639f61f98334ed5313f18b066d4955fe798c08f6756920ee47ae894c80e` |
| `p8/verify-after-rollback.json` | `205934d77bf0307a230b68a5538644d872961117aedc110dd6b35318f8f8c685` |
| `p9/rollback-drift.json` | `c668d3682fffe039da82bceb0fd6fab2577d739c42d7758396927c54402138a4` |
| `p10/reader-matrix.txt` | `80242712f8ce6360de2fcea88b4fd942516f1089504e9d8e99a165c1a27f6b6c` |
| `p10/A-rollback-held.json` | `3512e3a3c60ea20d230eaba17c4bdf9274dfed4440a206b07daf14c1966ae928` |
| `p10/A-rollback-after-release.json` | `58f72cea278f61cd8a241e20d714e5545a853d1bc8fac481697cc44df940adf3` |
| `p10/A-verify.json` | `d19b55f40bfe6319bea71ca33bc3802fc5217d60ef152d0a049ddd913e18931a` |
| `p3/projection.json` | `d5e07535c6aa2ab900ba2a3834e9315f2f3a2b7d82a2cebbe49d6527ea1444c9` |
| `p3/replica-vs-staged.json` | `4c59462312beec069939faf1e26c7aa90c7171794c3852ec2a4bba6f52480562` |
| `p1/live-inventory-before.sha256` | `f45385bfb3fb50e30233a979f17d38fd96679ffdb1f5071e07fb23f5e3ca3301` |
| `p-final/live-after.txt` | `599cc34242966d7774b47056086171f8337ca772de4501436ffff288cabf5aff` |
| `tools/rehearse.py` | `6b28dd56b8575a8a109dcc76df840fa73b5dfd5c0d4dad9d4aca375728671185` |
| `tools/make_receipt.py` | `3ff4af135e5766b8f7e73515863e0dae4337b36cd7f7ec5131dc6cfce148b450` |
| `tools/scratch_unitylock.py` | `bb412a0f3b82c01f88aac63db4f4c2e8272ac20e72649ca3d1314ab27728782e` |
| `tools/lockread.py` | `9b9b60f6fda264dbba17e935c9c9e37f6c975729484b545c0a3dbf487b50176e` |
| `inputs/README.md.merged-v2` | `3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123` |
| `staged/CCF-Ops/README.md` | `3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123` |
| `staged/CCF-Ops/deployment.json` | `ee13a811304c4cbfced23c4969b2a94d970d22fc5d424b72fa24dd2a943ad216` |

