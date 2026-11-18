# Sen-1 #754 — Independent review of exFAT scratch harness v7 (publication-to-booking gap)

**VERDICT: APPROVED** for the stated scope: a **scratch deployment-harness fix** closing the two #752 post-rename windows. **0 BLOCKER / 0 MAJOR in scope; 1 MAJOR-class residual carried forward (R-1, not a regression and not one of the two #752 windows); 3 MINOR; 2 NIT.** This approves *no* deployment and nothing live (see §8 status table).

- **Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), provider Anthropic, Claude Code (Agent SDK) in T3 Code, session `3b5c687d-00bb-4e74-a0c9-19ab9e572fce`, host `jer-laptop`, Linux 7.0.0-42-generic, Python 3.12.3.
- **Author under review:** Claude Opus 5.5, session `05912f35-360b-4806-aec1-4d694ed2a154` (#754 comment 6985686760). I wrote none of v6/v7.
- **Independence limitation (stated, not buried):** reviewer and author are **different models but the same family (Anthropic Claude)**. Shared-family blind spots are possible. Mitigation: my fault-injection driver was written from scratch (I did not read the author's driver code before writing mine; I only looked at the author's log summaries), runs on my own fixture, and was validated against the *unmodified v6* (it reproduces the #752 defect there, §4.1).
- **Child claim:** [#754 comment 6985945065](https://github.com/jermur788/CCF/issues/754#issuecomment-6985945065). Main HEAD at registration and at review time: `34185b8b71ba280c89091290345e537ef107bdf4`.
- **Scratch (ZX20 exFAT, st_dev 2098 = live):** `/media/jer/ZX20/CCF-Temp/Sen1-754-Review-20261118T171545Z/`. **Durable evidence:** `/media/jer/Files/CCF/Verification/Sen1-754-BookingGapFix-20261118/review/` (SHA256SUMS inside).

## 1. Subject identity (re-hashed by me)

| Artifact | SHA-256 | Result |
|---|---|---|
| v6 original `…/Sen1-741-C1-ExFATQual-20261118/v6/exfat_qual_v6.py` | `d75a218212cedca21f7fa5b93b81181c965aa91487afb782a37b751c61e8eec3` | matches packet |
| v6 original JSON | `77e5ed4ef4816f8855067a6b7d91d8eed083964a35c27319161337e8b2eb017c` | matches packet |
| v7 subject `…/author/exfat_qual_v7.py` | `02606febc26f8bda286387010ca2a16f25dc24b78fe3b993122b9be1e74f004c` | matches (450 lines) |
| author `exfat_qual_v6.py` copy | `d75a2182…8eec3` | identical to original |
| author dir `sha256sum -c SHA256SUMS` | 17/17 OK | author evidence intact; I wrote nothing there |

Fixture inputs (all matched the packet): unity-lock `8fa0c5ad…`, self-test.py `1bd03f15…`, unity-operator.md `e44e7eb8…`, `_ccf.py` `24a32146…`, README merged-v2 `3baf029e…`, receipt (TEST_FIXTURE_ONLY) `ee13a811…`. My replica = byte copy of live Ops; **50/50 verified against a fresh live manifest** (`cmp` equal; same content as the author's manifest, ordering differs only). Live-manifest-before sha256 `0d3a93c9ae9ef63326d815276d3fb5141a26ea5e00c6111f411d7ee6d776c94b`.

## 2. Code read (v6 → v7, `reviewer-v6-to-v7.diff`)

Changes are confined to the module docstring, `publish()`, new `classify_interrupted_step()`, new `forward_phase()`, `batch()`, and the output filename on the last line. I independently compared the main body (from `mount = subprocess.run(` to EOF) and the helpers `sha, safe_sha, sha_bytes, record, inventory, fsync_dir, restore_one, run`: **all byte-identical to v6 except `exfat-qualification-v6.json` → `-v7.json`**. No assertion removed or weakened (neither file contains any `assert`; all 21 `record()` checks are unchanged). See `main-body-comparison.txt`.

Design claims checked against code and tests: stage `"replace"` is set before `os.replace` and `"replaced"` after (v7:125–127); the temp is created with `O_EXCL` (118) and only removed when owned (176–190); a may-be-published path is booked first and un-booked only when the re-read equals the pre-state (194–208); unreadable → `destination-state-unverifiable`, foreign bytes → `destination-foreign-drift`, both stay booked so `restore_one` returns `UNVERIFIABLE` / `REFUSED` and never overwrites (149–154). `forward_phase()` being a separate function does make an exception from an uncovered instruction of the forward loop surface at the covered call in `batch()` (277–292).

## 3. Full v7 harness on my fixture

```
cd $S && python3 -I -B src/exfat_qual_v7.py $S/run21 $S/fixture/replica $S/fixture/live-manifest-before.sha256 $S/fixture/staged $S/fixture/receipt/deployment.json
exit=0   {"total": 21, "pass": 21, "fail": 0}   stderr empty
```
`device {scratch: 2098, live: 2098}`, mount `exfat rw … errors=remount-ro`. `run21-v7.stdout` is byte-identical (`c2af83953bec…`) to the author's own run21 stdout — the gate is deterministic and reproduced independently.

## 4. Independent fault injection (my own driver `review_driver.py`, sha256 `12d81a2462387a49a6ae4d7c210df0da93b04e93db7234ec97a866c003b32ad6`)

The driver `exec`s the subject source up to `\nmount = subprocess.run(` into a private namespace (definitions only; the subject is never edited) and injects from outside via `sys.settrace` (line- and opcode-level), namespace wrappers, and filesystem manipulation. Every case uses a **fresh byte-copied fixture** (`copytree` of the replica) and my own 50-file hash check against the manifest.

### 4.1 Driver validity control — unmodified v6 (`driver-v6-control-named.*`)
Same driver, v6 source: the post-`os.replace` injection yields `error-before-publication` with **booked 0 / recovered 0 and the file left published** (inventory ≠ 50/50) for indices 1–6; the publish-return/`booked.append` injection **escapes `batch()`** (`KeyboardInterrupt`) with nothing booked. 9/12 FAIL on v6, exactly the #752 defect; the 3 original cases PASS on v6. So the driver detects the defect and is not vacuous.

### 4.2 Required cases on v7 (`driver-v7-named-negatives-extra.*`: 37 cases, 32 PASS, 5 INFO, 0 FAIL)

| Case | Result on v7 |
|---|---|
| (a) KeyboardInterrupt at the line right after `os.replace` (`progress["stage"]="replaced"`), **every index 1–6** (incl. 1 unity-lock and 6 receipt). Publication confirmed at the instant of the signal (`dst == expected`) | each: k booked, k RESTORED, **50/50 byte-exact**, no `.tmp`, kind `interrupted-after-publication` (never `error-before-publication`) |
| (b) KeyboardInterrupt between `publish()` returning and `booked.append(rec)`, idx 1, 3, 6 | same as (a); publication confirmed at fire |
| (c-a) `fail="interrupt_readback", fail_at=6` | 6 booked / 6 RESTORED / 50/50 |
| (c-b) `fail_prestate_at=4` | 3 booked / 3 RESTORED, idx 4 never published, kind `error-before-publication` (correctly, destination == pre-state) |
| (c-c) `fail="dirsync", fail_at=6, hostile_recovery_path="_ccf.py"` | 5 RESTORED + 1 UNVERIFIABLE (`_ccf.py`), still holds the new bytes, **inventory ≠ baseline — baseline not claimed** |
| (d1) destination unreadable after rename (real: path replaced by a directory → `IsADirectoryError`; and `sha()` raising EIO), idx 3 and 6; also at the append boundary | kind `destination-state-unverifiable`, path booked, result `UNVERIFIABLE`, not labelled pre-publication, baseline not claimed, other paths RESTORED |
| (d2) foreign drift after rename (idx 3, 6; also at append boundary) | kind `destination-foreign-drift`, result `REFUSED`, foreign bytes still on disk (not overwritten), others RESTORED |
| e1 signal just **before** `os.replace` (idx 1, 4, 6) | provably unpublished → `error-before-publication`, k−1 booked and restored, temp removed, 50/50 |
| e2 signal while writing the temp | temp removed, `error-before-publication`, 50/50 |
| e3 pre-existing **foreign** `.tmp` at idx 3 | not truncated/adopted/removed (bytes intact), nothing published at idx 3, earlier two restored |
| e7 unreadable destination **before** publication | `pre-state-unverifiable`, recorded in `uncertain`, not booked, not mislabelled |
| e8 foreign drift **before** rename | `pre-state-drift`, foreign bytes untouched |
| e9 **genuine `os.kill(SIGINT)`** queued at the post-replace and pre-append lines (idx 1, 6) | CPython picks the delivery point; every case k booked / k RESTORED / 50/50 |

### 4.3 Exhaustive sweeps on v7 (final driver revision)

| Sweep | Points | Outcome |
|---|---|---|
| Opcode-level KeyboardInterrupt before **every instruction** of `publish`, `forward_phase` (k = 1 and 6; 229 / 189 instructions) — a superset of the author's 406 line-level points, including the CALL→STORE gap after `publish()` returns | 836 (435 reached; the rest are instructions on paths not executed in that run) | **every reached point: returned normally, all RESTORED, 50/50, no `.tmp`** |
| Second signal at every instruction of `classify_interrupted_step` (280) after a first signal at post-replace k=6, post-replace k=1, and pre-append k=6 | 840 | **840/840 ok-restored** (author claimed 27) |
| **Real SIGINT storm** (`pthread_kill` from a timer thread at uniform random offsets across the ~35 ms `batch()` run, default int handler) | 2000 | 1982 restored 50/50, 15 completed as full installs (signal landed after the last step), 3 not delivered, **0 escapes, 0 dirty** (an earlier 800-trial run: 0 flagged) |

### 4.4 Probes beyond the author (findings in §5)
Opcode sweep of the `batch()` frame itself, with the forward phase succeeding (`fail=None`, k=1; 262 instr.) and with a genuine forward failure (`dirsync` EIO at idx 3 and 6; occurrences 1–3; 1572 points). Restore-phase signals (publish call 7 and 9, before and after `os.replace`) and stale-temp retry. Results below.

## 5. Findings

### In-scope blockers: **none.**

### R-1 (residual, MAJOR-class, **not** an in-scope #754 blocker): signals in `batch()`'s own frame after the forward phase can escape without completing rollback
`batch()` lines 294–317 (the `if hostile_recovery_path:` test, the `try:` at 300, `if out["failure"]:` 301, the recovery `for` 302, `rel = None` 303, the `try:` 304, the append of the result 306, and the `finally` 316 / `return out` 317) sit **outside** any guard. A KeyboardInterrupt delivered at those instructions escapes `batch()` as an exception.
- *Single-signal variant:* forward phase fully succeeded (6/6 published, consistent), signal lands at 294–317 → `KeyboardInterrupt` escapes, **6 files stay installed, no rollback, no result** (the caller-owned `evidence` dict holds booked, `failure=None`). 14 of 262 instruction points in the `fail=None` sweep.
- *Second-signal / recovery variant:* forward failed (e.g. dirsync EIO, 3 or 6 booked); a signal at 302–304/306 escapes mid-rollback with some paths still new (`evidence["booked"]` and the partial `recovered` list are retained). 65 escape points of 1572. Signals inside the per-record `try` (the common case) are handled (`RESTORE-ERROR` and the loop continues).
- *Why not a blocker:* identical structure existed in v6 (not a regression); neither of the two #752 windows; these positions are reachable by a real signal only at eval-breaker checks after a call, and **0/2000 real SIGINT trials hit it**; it is the same class as the packet's accepted "signal during recovery / power loss → off-volume backup + post-interruption full hash reconciliation" residual. The author's claims are limited to the forward step, and I did not find a claim of recovery-phase coverage.
- *Required carry-forward:* the real deployer must not rely on in-process `try/except` for the batch+rollback critical section. Recommended minimal fix: block SIGINT/SIGTERM/SIGHUP with `signal.pthread_sigmask(SIG_BLOCK, …)` (or a deferring handler that sets a flag checked after rollback) from before the first publish until after `batch()` returns/rollback completes; and keep the mandated post-interruption hash reconciliation against the off-volume backup. This closes R-1 and the settrace-only windows together.

### F-2 (MINOR): restore-phase interrupt leaves a `.tmp` that blocks a retry of the same restore
`restore_one()` calls `publish()` (v7:157), which now uses `O_EXCL` (118) but has no temp cleanup outside the forward classifier. A signal before `os.replace` inside a *restore* leaves `<dst>.tmp`; `batch()` reports `RESTORE-ERROR` (fail-closed, correct) but a second `restore_one` on that path raises `FileExistsError` until the temp is removed by hand (v6 truncated and reused it). Reproduced: `e6-restore-retry-after-stale-tmp`, `retry_restore_one: FileExistsError`. No data risk; an operator-recovery usability regression. Minimal fix: in `publish()`, `try/finally`-unlink the temp it created (`progress["tmp_created"]`) on any exception before `os.replace` returns; or have the recovery path remove only a temp it created.

### F-3 (MINOR): `RESTORE-ERROR` entry can carry `rel: None`
If the signal lands at line 305 (`rel = Path(rec["path"]).relative_to(base)…`) the per-record `except` runs with `rel` still `None`, so `recovered` gets `{"rel": None, "result": "RESTORE-ERROR", "current": None}`; the path is not restored and the entry does not name it (66 of 1572 points). Safe (explicitly an error; `evidence["booked"]` still lists the path) but weakens the "exact evidence" requirement. Fix: derive `rel` from `rec["path"]` inside the `except`, or compute it before the `try`.

### F-4 (MINOR): author evidence does not exercise the `batch()` frame
The author's 406-point and 27-point sweeps cover `forward_phase`/`publish`/classifier only, so R-1 is not visible in them. Add a note to the result handoff that recovery-phase signals are out of the proven scope.

### N-1 (NIT): a restore-phase signal *after* its `os.replace` reports `RESTORE-ERROR` although the file is in fact restored (conservative mislabel; state is correct, 50/50). Seen in `e5-recovery-after-replace-call7/9`.
### N-2 (NIT): `os.fdopen(fd)` after `os.open` has an uncovered instruction gap (fd could leak until process exit on a signal in that gap; the temp itself is cleaned by the classifier).

## 6. Residuals and out of scope (never claimed closed)

SIGKILL, kernel crash, power loss and SIGSTOP are **not testable in-process** and were not tested; they remain under the packet's off-volume backup + full post-interruption hash reconciliation + explicit owner risk acceptance. A TOCTOU between the pre-state `sha(dst)` and `os.replace` (no flock in this harness) is unchanged from v6. The opcode-level models are *stricter* than real signal delivery (3.12 raises only at eval-breaker checks); the real-signal storm is statistical, not exhaustive.

## 7. Live read-only recheck (step 5)

- Live Ops still **50 files**. **No pre-existing file I read changed except `board/active.md` and `lanes.json`**: both were rewritten at **17:28:31Z** (my replica was taken ~17:16Z) by another worker — the diff vs my replica is two new `CCF-755-H0A-*` lanes created "by temporary G1 physical setup custodian … under Manager #730" (new board rows + two `MultiH0A*` lane entries). I never opened either file for writing; this is foreign legitimate churn, not mine, and not related to #754. Board now `4fa750eb…` (was `2b99552c…`), lanes `c1d14719…` (was `c17903dd…`). `deployment.json` unchanged `8c66b290…`.
- Everything else in the live manifest byte-identical (`diff` shows exactly those two lines). Receipt fixture `ee13a811…`, v6 original `d75a2182…`/`77e5ed4e…`, v7 `02606feb…` re-hashed **unchanged**. My replica re-hashed pristine (my cases only touched per-case copies). Author dir 17/17 OK.
- My worktree creation did not touch the primary checkout; its pre-existing uncommitted changes are untouched.

## 8. Status (this review only)

| SOURCE (v7 scratch harness) | REVIEWED | STAGED | DEPLOYED | UNITY |
|---|---|---|---|---|
| authored by Opus 5.5 | **APPROVED (scratch harness fix, scope above)** with R-1/F-2/F-3 carried forward | not changed by this review | **NOT DEPLOYED — no live install authorised** | **NOT RUN** |

## 9. Not established

Not an approval of any live install, the D-091 Unity-lock runtime, the five deploy paths, the merged README or the receipt (those are separate reviews). Not tested: any real deployment script (this is the qualification harness only), Windows, Unity, power-loss/SIGKILL/SIGSTOP, other filesystems, concurrent writers/flock behaviour. Not independent of the Claude model family (§ top). R-1 is untested against a real production driver. The 5 INFO cases in the driver output are informational probes, not PASS claims.

## 10. Commands and exit codes

All Python as `python3 -I -B`. Exit codes: v7 21/21 harness `0`; `review_driver.py … named,negatives,extra,extra2` `0` (37 cases); v6 control `0` (9 FAIL by design); opcode sweep, double-signal sweep, batch-recovery sweep and storm each `0`. Driver usage: `review_driver.py SUBJECT WORKDIR FIXTURE OUT.json PHASES` (phases `named,negatives,extra,extra2,sweep,sweep_double,sweep_batch_fail,storm`). The "BLOCK-…" labels inside the sweep JSONs are my driver's *internal* triage tags (state not restored and not reported by `batch()`), mapped to R-1/F-3 above — not verdicts. Preliminary earlier-revision runs are kept under `review/preliminary/` (same results, earlier driver revision).

## 11. Artifact SHA-256 (durable dir `…/Sen1-754-BookingGapFix-20261118/review/`)

| SHA-256 | file |
|---|---|
| `13b041f301b7b66beebe1b02691f64d187e929f977af6d16910b851942781b43` | ./driver-v6-control-named.json |
| `c76ee9e217ef8c49335ffa09e44ff52a39ac8654bf60f886ee9ada91aa2fe370` | ./driver-v6-control-named.log |
| `a169ad94d2b82e32813c3f3304e721fe70579b8d6f0d00b3e3dbbeed472ec61c` | ./driver-v7-batch-recovery-sweep.json |
| `4170b4eca2c9989aa0928723ec578ae041b2dc202b2e93db1b30ca1d1325cd19` | ./driver-v7-batch-recovery-sweep.log |
| `ad4d59b32a5d77e8af6fb2d11f57e9f7da8633d066bc525909aec2ee5f1def29` | ./driver-v7-double-signal.json |
| `1538d5f425aaff49051edbc4f14dde2015e10a5ae304b7a4b455fbe181667648` | ./driver-v7-double-signal.log |
| `be7b0e4766bdb7c00de3fbf36f7b571ef450b9358debb8b5374bc5e8aa381810` | ./driver-v7-named-negatives-extra.json |
| `314187394096d91e731520b58be446d683cb16cb90345b30688bb4230b788c6b` | ./driver-v7-named-negatives-extra.log |
| `f386c84b5e9f6c98f49313e946c65e017188a179725bd51f815cb0d2609e9570` | ./driver-v7-opcode-sweep.json |
| `730f115c8e97a4bd27dc0ca472088b250657a44f4a346a457b2d399204d39b35` | ./driver-v7-opcode-sweep.log |
| `11360355c03de1bc3f65a9da8bfe72ac52f0b28f8c09eb7a5179bf4bc5d5ec1b` | ./driver-v7-real-sigint-storm.json |
| `df8a00c2238d92353e4c7732f47b87ba7059fceaf98d048642309f0c64c1a747` | ./driver-v7-real-sigint-storm.log |
| `95e2f70264fe9978449497f362dcae7dad7eea3b422bca13661c7f7469032f90` | ./fixture-manifests/live-manifest-after.sha256 |
| `0d3a93c9ae9ef63326d815276d3fb5141a26ea5e00c6111f411d7ee6d776c94b` | ./fixture-manifests/live-manifest-before.sha256 |
| `0d3a93c9ae9ef63326d815276d3fb5141a26ea5e00c6111f411d7ee6d776c94b` | ./fixture-manifests/replica-inventory.sha256 |
| `0b59c40c687dc37b7efcf4b095cf37419fafca5ceb8584bb9b734aee53140639` | ./main-body-comparison.txt |
| `90679aff7d2bcad07a25b5ff59bed581e5f8ca1b08d7566332bcdb04f7c43b78` | ./preliminary/driver-v6-control.json |
| `c76ee9e217ef8c49335ffa09e44ff52a39ac8654bf60f886ee9ada91aa2fe370` | ./preliminary/driver-v6-control.log |
| `aaeed10654bce81cdf6a84771deceb8a2295c99985b16fd35734757a5358b389` | ./preliminary/driver-v7-batchfail.json |
| `4170b4eca2c9989aa0928723ec578ae041b2dc202b2e93db1b30ca1d1325cd19` | ./preliminary/driver-v7-batchfail.log |
| `8ee01d46ae85bdaf93373056968833ba2659aaed728035b9077f564f401ec672` | ./preliminary/driver-v7-double.json |
| `1538d5f425aaff49051edbc4f14dde2015e10a5ae304b7a4b455fbe181667648` | ./preliminary/driver-v7-double.log |
| `c08618a03a3d1a3bf607dee1e6a0327791b44d98cdc80e96f86dee3bedd915d5` | ./preliminary/driver-v7-extra2.json |
| `6a83c99be3ee6e589b97d7c7e6257179711095bad46480b92aeef325394913d7` | ./preliminary/driver-v7-extra2.log |
| `8bac0da4ecccfa0a1dbd0742eea544180c3ce5565d025b6b463567fc38787d16` | ./preliminary/driver-v7-named.json |
| `011ea69d638c940e66e101d52a7e014f91eece694149536a80af67041985edcd` | ./preliminary/driver-v7-named.log |
| `ff14f60b1d6f7d9ce0fc626be9306d824f3904922710d152796b8a8d9c7a1d91` | ./preliminary/driver-v7-storm-run1.json |
| `9e6103e3c4c5fd3bf0e1f3dcb5a23ec03a7279588286860c76679b1f80cb425e` | ./preliminary/driver-v7-storm-run1.log |
| `995ca941d8bf74651e5e07dd0d3bc5792d896e620edbf4cee98623dc43b5b187` | ./preliminary/driver-v7-sweep.json |
| `730f115c8e97a4bd27dc0ca472088b250657a44f4a346a457b2d399204d39b35` | ./preliminary/driver-v7-sweep.log |
| `12d81a2462387a49a6ae4d7c210df0da93b04e93db7234ec97a866c003b32ad6` | ./review_driver.py |
| `f68913d0618851a129f60353287d8c657a1fb753ac94987affe68e23cb64ddce` | ./reviewer-v6-to-v7.diff |
| `7c5770f3817a6ba894c0666a2873d3639568c36dd44324f6429238a77705a78e` | ./run21-v7-result.json |
| `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855` | ./run21-v7.stderr |
| `c2af83953bec0c738451c0fc87b04dcd38a111be5421c3863bd65e48bc1ca2d3` | ./run21-v7.stdout |

The review document itself is not self-hashed; its SHA-256 is recorded in the #754 hand-off comment and in `review/SHA256SUMS-doc`.
