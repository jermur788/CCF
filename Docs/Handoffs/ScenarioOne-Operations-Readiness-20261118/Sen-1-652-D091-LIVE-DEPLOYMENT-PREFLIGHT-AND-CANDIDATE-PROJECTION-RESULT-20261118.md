# Sen-1 #652 — D-091 live deployment preflight and candidate projection (READ-ONLY)

**CONCLUSION FOR FUTURE DEPLOYMENT: NEEDS CHANGES.** The installer projection and provenance need changes. The four reviewed runtime/source files do not.
The physical state is clean today. The exact projection is still undecided, the post-install receipt does not exist yet, the projected bytes have not been rehearsed, and there is no installer reservation. Section 9 lists what must close before a Manager can consider **[APPROVED TO DEPLOY OPS]**.
**Nothing was installed. No board, lanes, state, guard, history, source or Unity file was written. No flock was taken on live Ops.**

- **Date:** 2026-11-18, about 12:58–13:16 UTC.
- **Packet:** Manager #731 `Sen-1-731-MANAGER-652-D091-SOURCE-ACCEPTANCE-AND-DEPLOYMENT-PREFLIGHT-GATE-20261118.md` (branch `docs/sen-1-731-chas-manager-intake-20261118-v1`).

## 1. Actor, separation and custody

- **Actor:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code.
  - Session `a368d8d5-e9bd-4304-8d7e-59963cfd4b75`, host `jer-laptop`, Linux user `jer` uid 1000.
  - Owner USER jermur788 dispatched it directly.
  - Registered before any physical inspection: https://github.com/jermur788/CCF/issues/652#issuecomment-6979268813
- **Separation:**
  - This session is not the source writer (Codex `9bcdfde6…`), not the reviewer (Sonnet `5b89dd08…`), and not the earlier one-row board custodian (Opus `81a16309…`).
  - It authored and reviewed none of the #652 source. It claims no review and no deploy approval.
- **Custody observed (read-only):**
  - Live Ops is owned by `jer` on an exFAT mount (`uid=1000,gid=1000,fmask=0022`).
  - `/proc/locks` has **no** lock on device 08:32 at 13:14:17Z. That covers the board dir (inode 1399769), the guard (1598961) and anything else on ZX20, so no custodian or writer held the board or the Unity guard.
  - There is no Unity `lock.json`.
  - No board row reserves live Ops installation. The `CCF-652-OPS-READINESS` row is the only #652 row; it is still `READY | RESERVED ONLY / NOT RUNNING / SOURCE BLOCKED` and names the source writer.
  - **So no exclusive installer custody exists today.** This preflight did not need any and created none.

## 2. Storage identity (fresh, not inherited)

| Check | Observed |
|---|---|
| ZX20 | `/dev/sdd2`, exfat, UUID `18D6-896E`, rw, mounted at `/media/jer/ZX20`. lanes.json pins `/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2`; the kernel name is now sdd, not sda |
| ZX20 free | 53,034,745,856 B, above the 16,106,127,360 B minimum (90% used) |
| Files | `/dev/sdb4`, ext4, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`, 44,068,823,040 B free (91% used) |
| Live receipt audit | Source `verify-deployment --root /media/jer/ZX20/CCF-Ops --manifest live deployment.json` gives **PASS (28 source hashes, 7 pinned overrides, 1 config projection, 5 live-state entries), exit 0**. The live tree's combined hash was unchanged before and after |
| Original board lint | Live `packet-lint --board`: **PASS, 42 live tasks, exit 0**. Board and lanes hashes were unchanged and `scripts/__pycache__` was untouched (still dated Nov 11) |

## 3. Board/lanes drift: reconciled and attributed, not altered

The complete hash chain is proven by each actor's own before/after copies:

| Step | Board `active.md` | `lanes.json` | Actor / authority | Change |
|---|---|---|---|---|
| #652 custodian refresh, 12:22Z | `8d3c17e1…` → `d98c3156…` | `b16bfcad…` (unchanged) | Opus `81a16309`, custodian | #652 row only |
| **#734**, 12:41:21Z | `d98c3156…` → `ae80e190…` | `b16bfcad…` → `51850d74…` | USER jermur788 custodian via Opus `dd9143ff-82d8-4eb7-a74d-8b413009cda2`, under `flock -x -n` on the board dir | Row `CCF-732-MULTI-YEAR1-NATIVE-QUAL` went WAITING → DONE-PENDING-LEDGER after Manager #730 APPROVED. Lane `MultiYear1Qual732` was **removed**. One ledger paragraph was added. Receipts are in `/media/jer/Files/CCF/Verification/CCF-732-Year1Qual-20261118/custody/release/` |
| **#735**, 12:52:39Z | `ae80e190…` → **`2b99552c…`** | `51850d74…` → **`c17903dd…`** | Owner direct dispatch via Codex gpt-6.1-sol `01a1eeb5-2429-78e1-9783-3923470314f0`, under the exclusive board flock | Row `CCF-725` owner text changed to immutable custody. The `MultiAnnualF0Child725` lane `purpose` changed (custody extended to 2026-11-27T18:00Z). One paragraph was added. The receipt is `/media/jer/Files/CCF/Verification/CCF-735-SourceRetention-20261118/RETENTION.md` |

- The Manager packet saw only the first drift (12:41, `ae80e190` / `51850d74`). **A second lawful drift (#735, 12:52) happened after the source handoff.**
- Diffing `d98c3156` against the current `2b99552c` shows exactly those two rows plus two paragraphs. **The `CCF-652-OPS-READINESS` row is byte-identical.**
- No lane related to #652 exists in lanes.json (23 lanes now).
- Both actors were separate, owner-authorised custodians who wrote their own receipts. I found no conflicting source, installer or Unity reservation.
- The live receipt's `sha256_without_lanes` pin still passes, so neither change touched storage, repository or git_stores config.
- **Nothing was restored, rewritten or rolled back.**
- The stale `CCF-652` row still says SOURCE BLOCKED and names the source writer. That needs a separately authorised custodian-only correction. It is not done here.

## 4. Source provenance

- **Checkout:** `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116`.
  - Clean, at HEAD `4c7acc05…`, which equals origin and the local store ref.
  - `4d03f5b5..4c7acc05` changes no file under `CCF-Ops/` (reports only).
- **Git stores:**
  - Files `Repository.git` is bare. Its only checkout of `ops/sen-1-652-unity-lease-readiness-20261116-v1` is that worktree.
  - Legacy `/media/jer/Files/CCF-Legacy/.git` has no local branch of that name and no checkout. No dual-store writer conflict.
- **Four D-091 files:** the bytes at `4d03f5b5` and in the worktree match the packet exactly.
  - unity-lock `8fa0c5ad…`
  - self-test.py `1bd03f15…`
  - README `a1911b01…`
  - unity-operator.md `e44e7eb8…`

## 5. Per-file deployment projection (proposed, NOT executed)

- **Modes and owners:** ZX20 is exFAT, mounted `uid=1000,gid=1000,fmask=0022`. Every live file is `0755 jer:jer`. chmod/chown have no effect there, so per-file mode steps are moot. Git modes (100644/100755) cannot be represented.
- **Copy step for every COPY row:** the historical rehearsal recipe. Under the exclusive board-dir flock, compare live bytes to the expected old hash, write `NAME.tmp` in the same directory, fsync, rename, then re-hash.

| Path | Class | New (source `4d03f5b5`) SHA-256 | Old/live SHA-256 | Action |
|---|---|---|---|---|
| `scripts/unity-lock` | D-091, required | `8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8` | `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5` | COPY |
| `tests/self-test.py` | D-091, required (the qualified 233 suite; not runtime) | `1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50` | `2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3` | COPY |
| `roles/unity-operator.md` | D-091, required (operator instructions) | `e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db` | `120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815` | COPY |
| `scripts/_ccf.py` | Option A (approved at `0ea3ca4c`), **required**: the 233 suite qualified D-091 only on top of it | `24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435` | `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001` (installed override) | COPY. The diff shows the candidate is the live override plus Option A, so no live-only semantics are lost |
| `README.md` | D-091 docs over the live override | `a1911b01ece918d4eb52bfde230fe197e34f839a8c41d79f67555d9a560598e4` | `249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517` (installed override) | **DECISION (P-1)** |
| `environment.sh` | Option A, not required for D-091 | `fc1744b25c82fd136c176c8f678a3245dd718d7e98985a1ac5a9f86de51c3409` | `3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954` (installed override) | DECISION (P-2). Recommend COPY: the candidate is exactly live plus a bounded `on_durable_ccf` check, it was in the 5-file rehearsal, and live and candidate imports both resolve |
| `scripts/verify-deployment` | Option A, absent live | `ee001216fb020ae33ed54f4c317e2c8e19ce4bdb407f4e6bb8ae457248a02386` | ABSENT | OPTIONAL. It can run from source (as here). Installing it adds a new receipt entry |
| `DeploymentProvenance.md` | Option A docs, absent live | `a08fb8a5d38e82ab3dad1b6b1e0df7280cf28e1751fd1359b61cc2947b288db4` | ABSENT | OPTIONAL. The candidate README names it, so install it together with the candidate README |
| `AGENTS.md` | installed-only override | source `a7d8aa32…` is older | `d1ce3ad775e16d80fea6779c91bfa66adbd2e019edfbbbb9280be563be60e08f` | **RETAIN live; DO NOT COPY.** The source would revert the live storage-policy paragraph |
| `deployment.json` | receipt | source `be9fbcd0…` must NOT be copied | `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18` | Write a NEW reviewed receipt after install (P-3) |
| `lanes.json`, `board/active.md`, `state/unity/{guard,history.jsonl}`, `ledger/`, `state/board-history.jsonl` | live state/config | — | `c17903dd…`, `2b99552c…`, `e3b0c442…` (empty guard), `dfb50644…`, unchanged | **NEVER COPY**. They must hash-match before and after |
| `scripts/lane-status`, `scripts/packet-lint`, and the 26 other identical files | unchanged | equal | equal | no action |

Notes on the table:

- **P-1 README.md.** The candidate was rebuilt from the old source README, not from the live override. Copying it would:
  - drop the accurate live text about the persistent `/dev/disk/by-id` path and rejection of duplicate visible UUIDs;
  - change `Full receipt belongs in /media/jer/Files/CCF/Verification/WorkflowFoundation` into the retired `CCF-Verification/WorkflowFoundation`;
  - correctly replace the stale legacy store path `/home/jer/CCF/.git` with `/media/jer/Files/CCF-Legacy/.git`, which matches lanes.json.

  The options are: (a) copy the reviewed bytes and record the two regressions as an accepted docs debt, or (b) install merged installed-only bytes, which are new and need review. **Docs only; runtime is unaffected.**
- **P-3 receipt.** The source `deployment.json` candidate block is **stale for D-091**:
  - Its unity-lock (`e4653034…`), self-test (`d4ebd922…`) and README (`5b7abb83…`) hashes are the Option A bytes.
  - It omits `roles/unity-operator.md` entirely.
  - Proof: source `verify-deployment --manifest <source deployment.json> --candidate-root <source>` gives **FAIL, exit 1**, on exactly those three files.
  - The live receipt's `candidate` block names the even older `60f97b06` and also FAILs against `4d03f5b5`.
  - The installer must therefore generate a new receipt. It should move the copied files from `deployed_overrides` into `source_hashes` with source commit `4d03f5b5`, and keep AGENTS.md and the other retained overrides pinned.
  - It must keep historical `source_head`/`deployed_at`, add a separate install record with the real install timestamp, and back up the old receipt as `deployment.json.bak-<UTC>`.
  - That new manifest needs independent review before or with the deploy permit.
- **Stale bytecode.** `scripts/__pycache__/_ccf.cpython-312.pyc` (Nov 11) goes stale after `_ccf.py` changes. Python's mtime/size check invalidates it, and every tool sets `dont_write_bytecode`, so no correctness risk. Leave it, as the receipt already classifies it.

**Backup and restore plan (for the future installer):**

- Take a full verified backup of all 50 live files (bytes + SHA-256) into a new Files verification folder before the first write.
- Restore applies **only** to paths whose current bytes equal the expected new hash, while exclusivity is still held. Any foreign drift blocks restore of that path.
- The board, lanes, state and ledger are never restored from backup.

## 6. Caller compatibility

- **Gate scripts.** `git grep` over all **921 origin branches**, limited to `Tools/*`, `*.sh` and `*.py`, finds **no** `unity-lock status|release` call that passes `--eta` or `--no-expiry`.
  - Every tracked gate (Mfm09/12, MfmApprovalBoundary, MfmSaveProvenance, MfmTreatmentReview, RackV2FieldMarking, S1C t02–t06, S1CObjectiveHistory, ScenarioOneRepeatedInterventions) calls `unity-lock status --json`. That still parses under the new mutually-exclusive group. **Compatible.**
- **Personal launcher.** `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh`, `1d43336e…`, the only untracked caller found at maxdepth 3 of Operations:
  - It acquires with legacy `--eta "+4h"`. Still accepted; it writes a legacy ISO record, which is rollback-safe.
  - It passes **no `--pid`**, so the lock never binds a shell PID.
  - It records the Editor PID as `$!` of `env … Unity &` in `run/unity.pid`. env execs Unity, so that is the Editor PID.
  - It releases normally only after waiting for the Editor and confirming `unity_editor_pids` is empty. That matches the new release refusal. **Compatible, no change required.**
  - Optional: move it to `--no-expiry` under D-091. That is the user's file and is not done here.
  - Known pre-existing risk (2026-11-12): if the terminal closes before the EXIT trap runs, the lease stays held. D-091 makes this no worse.
  - Board row `CCF-412` (user personal playtest on ResearchCopyQual391) is **WAITING, not started**. A deploy must confirm it has not started.
- **Default consumers.** Both the live `_ccf` and candidate `_ccf` `config_read` parse the **current** 23-lane lanes.json and resolve the live board.
  - Import surfaces: every name that live `lane-status`, `packet-lint`, live `environment.sh` and candidate `unity-lock` import exists in candidate `_ccf.py`.
- **Untested caller limit.** I searched only tracked Tools/scripts on origin and Operations to maxdepth 3. Ad-hoc shell history or deeper untracked scripts are **not** covered.

## 7. Editor PID, release and force policy (operator instructions for the deploy packet)

- **Record the actual Editor PID** with `--pid`, only once the Editor is verified alive. Never pass a launcher or terminal shell PID.
  - Candidate release refuses while *any* Unity Editor or the recorded PID exists (reviewer M-4). A long-lived shell PID would block normal release until that shell exits.
  - If the PID is unknown, omit `--pid`. The editors() scan still guards release.
- **Normal release:** first confirm `unity-lock status --json` shows `unity_editor_pids: []` and the recorded PID is gone. Then release with exact `--owner --task-id --lock-id`.
- **Force release** is unchanged: explicit prior Overall Manager approval referenced through `--manager-approval`, plus `--force --owner --reason`. The complete previous record goes to history. Never use force to work around a live Editor.
- **Side effect.** `unity-lock status` is not strictly write-free when the state dir exists: it opens `guard` with `a+` and takes a flock. For that reason this preflight did not run it against live and read `state/unity` and `/proc` directly.

## 8. Live Unity state and rollback safety

- **At 13:12:20Z:** no process with argv[0] basename `Unity` (the same rule as `editors()`), and no `state/unity/lock.json`.
  - guard: empty, `e3b0c442…`.
  - history: 299,478 B, `dfb50644…`. Last event 2026-11-15T22:44Z was #548's metadata-only release.
  - **The machine-wide Unity lock is free and no Editor is running.**
- **Old reader against the new null shape.** Proven by loading `read_state()` without `main()` against synthetic records in this session's scratchpad (`probe_reader_compat.py`; no live state read):

| Record | Live (old) `54cf0a95` | Candidate `8fa0c5ad` |
|---|---|---|
| legacy ISO eta | READ_OK | READ_OK |
| `eta:null, no_expiry:true` | **MALFORMED: invalid string metadata** | READ_OK |
| `eta:null` unmarked | MALFORMED | MALFORMED (requires mode) |
| ISO eta + `no_expiry:true` | READ_OK | MALFORMED |

- **Rollback rule:**
  1. Before restoring the old `unity-lock`, read `state/unity/lock.json`.
  2. **Absent:** restoring is safe.
  3. **Legacy ISO record:** the old reader reads it; restore the scripts and preserve the record unchanged.
  4. **`no_expiry:true` record:** **DO NOT restore the old script.** It would see a malformed lock, so acquire is refused and normal release is impossible. The only way out would be force, which is not a rollback path.
     - Instead, keep the candidate script, verify the Editor and recorded PID have exited, and have the owner normally release.
     - Alternatively, obtain a separately reviewed compatible fallback.
  5. Never edit or migrate held lock JSON. An existing Unity process or any unrelated live Ops change means STOP, not force.
- The asymmetric last row is harmless because the candidate never writes it.

## 9. Required before any deploy permit (why NEEDS CHANGES)

1. **P-1/P-2 decisions** (README form; environment.sh, verify-deployment, DeploymentProvenance.md included or not). This yields one exact projection list.
2. **P-3:** a new post-install `deployment.json` manifest, generated for exactly that projection with honest timestamps, **independently reviewed**. The source receipt is stale and must not be copied.
3. **Fresh scratch rehearsal on the exact projected bytes.**
   - The historical #652 rehearsals (5-file, partial failure after 3 of 5, 50-file backup/restore, drift refusal; `ScenarioOne652OptionA-20261116/rollback-*`) used the **Option A** `unity-lock` `e4653034…`, README and self-test, and never included `roles/unity-operator.md`.
   - They are historical evidence only, not evidence for this projection. They are **author-run and not independently re-executed**.
4. **Board:** a separately authorised custodian must correct the stale `CCF-652` row and reserve an exclusive installer scope (paths, actor) **before** any installer write.
5. At deploy time, recheck: storage, all 50 hashes, `/proc/locks` empty, no Unity process, no lock.json, CCF-412 not started.
6. Owner's explicit release of the earlier **NO LIVE INSTALL** instruction, then Manager **[APPROVED TO DEPLOY OPS — EXACT CANDIDATE/INSTALLER]**.

No source runtime change is needed. Nothing found requires editing code outside the four reviewed files.

Non-blocking pre-existing note: both live and candidate `roles/unity-operator.md` still say receipts belong on `ZX20/CCF-Verification` and builds on `ZX20/CCF-Builds`. That contradicts the storage policy and was not introduced by D-091. Record it for a later docs task.

## 10. Tested vs not tested

**Observed (read-only, this session):**
- storage identity and capacity
- full 50-file live inventory with hashes and modes
- board/lanes hash chain and diffs
- live receipt verify PASS
- original board lint PASS 42
- source and candidate receipt verify FAIL (stale receipt)
- old/candidate `config_read` on live config
- import surfaces
- old/candidate reader compatibility on synthetic records
- `/proc` Unity scan, `/proc/locks`, Git store and worktree checkout state
- caller grep and launcher inspection

**NOT run:**
- the self-test suite (it writes `ZX20/CCF-Temp` and `Files/CCF/Verification` fixtures)
- `unity-lock status/acquire/release` on live
- any rehearsal or copy
- Unity, Windows
- dashboard/Drive sync

## 11. Evidence

Folder: `/media/jer/Files/CCF/Verification/Sen1-652-D091-LiveDeployPreflight-20261118/` (new, disjoint; no other custodian's receipt was edited). `SHA256SUMS` lists every file in it. Main files:

- `storage.txt`, `ops-ls.txt`
- `live-inventory.sha256`, `live-inventory-ls.txt`
- `active.md.observed-2b99552c.md`, `lanes.json.observed-c17903dd.json`, `deployment.json.live`
- `board-d98c3156-to-2b99552c.diff`
- `candidate-vs-live.txt`, `diffs/`
- `config-read-*.json`, `reader-compat-*.json`, `probe_*.py`
- `verify-deployment-*.txt`, `lint-live-board.txt`
- `unity-proc-state.txt`, `flock-observation.txt`, `git-stores.txt`

Statuses:
- **#652:** SOURCE APPROVED; LIVE INSTALL HELD; installer preflight **NEEDS CHANGES (projection/provenance)**.
- **#718, #690, #730:** untouched.
