# Sen-1 #754 — Delta re-check of exFAT scratch harness v7.1 (F-2 / F-3 resolution)

**VERDICT: APPROVED** for the stated scope: **v7.1 (`exfat_qual_v7_1.py`) resolves F-3 and resolves F-2 for the scenario the author claims and tested, with one new MINOR residual (D-1: F-2 is not closed for a 6-instruction window) and R-1 unchanged.** **0 BLOCKER / 0 MAJOR / 1 MINOR (D-1) / 3 NIT.** This approves a **scratch qualification harness only**; it approves no deployment and nothing live (§9). R-1 is **not closed** and is not presented as closed (§5).

- **Re-checker:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), provider Anthropic, Claude Code (Agent SDK) in T3 Code, session `eefc57ed-276c-478c-94ec-ac4cbd9e29f4`, host `jer-laptop`, Linux 7.0.0-42-generic, Python 3.12.3. I wrote none of v6/v7/v7.1 and I did not write the prior review (I did not read its session; I read its published document and evidence).
- **Independence limitation (stated, not buried):** same Anthropic Claude family as the author (Opus 5.5, session `05912f35`); the **same model as the prior #754 reviewer (session `3b5c687d`) but a new session**. My main driver is the prior reviewer's `review_driver.py` with one hunk adapted (§4.1), so that driver's blind spots are inherited; my own probe (`delta_probe.py`) is new but written by the same model family. Shared-family blind spots are possible.
- **Child claim:** [#754 comment 6987434505](https://github.com/jermur788/CCF/issues/754#issuecomment-6987434505). `git ls-remote origin main` was `34185b8b71ba280c89091290345e537ef107bdf4` at registration and `7d24af7963a5d1e35e5aca24a9f9b02466f1ba59` at the final check (moved by other workers; unrelated to this work).
- **Scratch (ZX20 exFAT, st_dev 2098 = live):** `/media/jer/ZX20/CCF-Temp/Sen1-754-Delta-20261118T181603Z/`. **Durable evidence:** `/media/jer/Files/CCF/Verification/Sen1-754-BookingGapFix-20261118/delta/` (`SHA256SUMS` inside).

## 1. Subject identity (re-hashed by me)

| Artifact | SHA-256 | Result |
|---|---|---|
| v7.1 subject `author/exfat_qual_v7_1.py` | `52d4c6aedbecd47c4cf912f8903fe07547cb24c9b743aa9c65f15fdf35c60eab` | matches packet |
| v7 (APPROVED earlier, left intact) `author/exfat_qual_v7.py` | `02606febc26f8bda286387010ca2a16f25dc24b78fe3b993122b9be1e74f004c` | matches |
| v6 original `Sen1-741-C1-ExFATQual-20261118/v6/exfat_qual_v6.py` | `d75a218212cedca21f7fa5b93b81181c965aa91487afb782a37b751c61e8eec3` | matches |
| `author/SHA256SUMS` (`sha256sum -c`) | all OK | author evidence intact; I wrote nothing there |
| prior `review/SHA256SUMS` (`sha256sum -c`) | all OK | read-only evidence intact |
| prior review document | `c0163e2b4bca08da8da65c96e962fe91364f8f0a5c7edc92515dffe87398afcb` | matches `review/SHA256SUMS-doc` |

**Fixture (fresh, mine).** Live Ops `/media/jer/ZX20/CCF-Ops` copied byte-for-byte to `fixture/replica` (50 files) with a **fresh manifest** (sha256 of the manifest file `934ccba7014ebafcf3ca51eec3ffd66bdf5d37a2f4976ce3491bc31534847fd2`, equal to the author's post-run manifest, i.e. it includes the 17:28Z #755 board/lanes change). Staged inputs hashed equal to the packet: unity-lock `8fa0c5ad…`, self-test.py `1bd03f15…`, unity-operator.md `e44e7eb8…`, `_ccf.py` `24a32146…`, README merged-v2 `3baf029e…`, receipt (TEST_FIXTURE_ONLY) `ee13a811304c4cbfced23c4969b2a94d970d22fc5d424b72fa24dd2a943ad216`. The replica re-hashed pristine at the end (my cases only touch per-case copies).

## 2. Delta is confined to what is claimed (code read)

I regenerated `diff -u v7 v7.1` myself (`v7-to-v7_1.reviewer.diff`); it is **identical** to the author's `v7-to-v7_1.diff` apart from the two header lines. Hunks: (1) module docstring, (2) `publish()`: the write + `os.replace` block is wrapped in `try … except BaseException:` that unlinks `tmp` if `os.path.lexists(tmp)`, sets `progress["tmp_removed_by_publish"]`, swallows any error from the cleanup itself, and re-raises; (3) `batch()` recovery `except`: if `rel is None`, derive it from `rec["path"]` (fallback `rec.get("path")`); (4) output JSON name `v7_1`.

AST comparison (`results/main-body-comparison.txt`): `classify_interrupted_step` and `forward_phase` are **byte-identical to v7**; `sha, safe_sha, sha_bytes, record, inventory, fsync_dir, restore_one, run, now` identical to v6 and v7; **the main test body (from `mount = subprocess.run(` to EOF) is identical to v6 and to v7 apart from the JSON filename**; no `assert`/`record()` weakened. The new `except` can only reach `os.unlink(tmp)` **after** `os.open(tmp, O_CREAT|O_EXCL)` has succeeded (the `try:` begins after that call), so a foreign pre-existing temp makes `os.open` raise before the handler exists and is never unlinked.

## 3. Full harness on my fixture

`python3 -I -B src/exfat_qual_v7_1.py $S/run21 …` → **exit 0, `{"total": 21, "pass": 21, "fail": 0}`**, stderr empty, no `.tmp`, replica unchanged. `run21-v7_1.stdout` is **byte-identical** (`c2af83953bec0c738451c0fc87b04dcd38a111be5421c3863bd65e48bc1ca2d3`) to the author's own `run21-v7_1.stdout` and to v7's. (My first invocation exited 1 because I had not pre-created `ROOT`; harness requires it to exist — operator error, rerun is the one recorded.)

## 4. Independent testing

### 4.1 Prior reviewer's driver, re-run against v7.1 (no regression)
`review_driver.py` as published **cannot be run unmodified on v7.1**: its `line_info()` finds `os.replace` by matching a *top-level* statement whose source contains the string, which in v7.1 matches publish()'s **docstring** (the call is now nested in a `try`). My first unmodified run therefore reported 15 spurious FAILs (it injected at the wrong lines). I discarded that run and made `review_driver_d71.py` (one hunk, `drivers/review_driver-to-d71.diff`: walk the function and match the exact statements `os.replace(tmp, dst)` and `progress["stage"] = "replaced"`). I confirmed it computes **identical line anchors on v7** (126/127) as the original and the correct ones on v7.1 (134/145). I also ran the adapted driver on v7 as a control and compared case by case with the prior reviewer's published v7 results: **37/37 cases same verdict and kind**.

| Phase (adapted driver, my fixture) | v7.1 result | vs v7 |
|---|---|---|
| named,negatives,extra,extra2 (37 cases: both #752 windows idx 1–6, append-boundary, unreadable/foreign dst, signal before replace/during write, foreign pre-existing `.tmp`, real `os.kill(SIGINT)`) | **32 PASS, 5 INFO, 0 FAIL** | same verdicts as v7; only difference: the 3 restore-phase probes (e5-before-replace ×2, e6) now leave **no `.tmp`** (v7 left one) |
| sweep (opcode KeyboardInterrupt before every instruction of `publish`, `forward_phase`, `batch`; k=1,6) | 1205 pts: publish 127+127 ok-restored; forward_phase 99/84 ok-restored; **batch frame 14 "escaped-dirty" (R-1), 25 escaped-clean, 12 ok** | publish/forward_phase identical; batch frame distribution **identical to v7 (14)** → R-1 unchanged |
| sweep_double (second signal at every instruction of the classifier after a first signal, 3 anchors) | **840/840 ok-restored**, 0 flagged | identical to v7 |
| sweep_batch_fail (batch frame while a forward failure forces recovery; dirsync EIO at idx 3, 6; 3 occurrences) | 1758 pts: **65 escape points, 0 `rel: None` entries** | v7: 65 escape points **and 66 `rel: None`** (F-3) of 1572 |
| storm (real SIGINT via `pthread_kill` at random offsets, default handler, forward path) | **2000 trials: 1618 restored 50/50, 188 completed full installs, 194 signal-not-delivered, 0 flagged, 0 escapes** | prior review 0 flagged |

### 4.2 My own probe (`delta_probe.py`, new; os-shim + `sys.settrace`; subject never edited) — v7.1 vs v7 as a validity control
`probe-v7_1`: **24 cases, 22 PASS, 2 INFO, 0 FAIL**. Same probe on v7: 6 PASS, 2 INFO, **16 FAIL** (the F-2 and F-3 defects, plus the new publish-cleanup checks) — so the probe detects the defects and is not vacuous.

- **F-2 (resolved for the claimed scenario):** interrupt (`KeyboardInterrupt`) / `OSError(EIO)` at `os.replace`, and `KeyboardInterrupt` in the temp file's `fsync`, inside restore publish calls #7, #9, #12 (first, middle, last restore): in every one `batch()` returns (no escape), that path is `RESTORE-ERROR` (target still holds the new bytes — explicit, not hidden), the other five are `RESTORED`, **no `.tmp` left**, a **retry `restore_one` returns `RESTORED`**, and the tree is **50/50** after the retry. v7: `.tmp` left, retry `FileExistsError`, not 50/50 (all 9).
- **F-3 (resolved):** `KeyboardInterrupt` at the `rel = Path(rec["path"])…` recovery line, occurrences 1, 2, 3, 6: the `RESTORE-ERROR` entry **names its path** each time (`deployment.json`, `README.md`, `scripts/_ccf.py`, `scripts/unity-lock`); v7 gives `rel: null` (4/4). Corroborated by the 1758-point batch sweep (66 → 0).
- **New `except` cannot unlink what it must not (publish() called directly):**
  - pre-existing **foreign** `unity-lock.tmp` (bytes `FOREIGN-BYTES`) → `FileExistsError`, bytes intact, destination baseline, no `tmp_created`, no `tmp_removed_by_publish`; same with a foreign `.tmp` **directory** (contents intact) — **PASS**;
  - interrupt **after** the real `os.replace` returns (shim does the real rename, then raises `KeyboardInterrupt`): destination holds the **new** bytes, no `.tmp`, `tmp_removed_by_publish` absent — nothing published is unlinked; through `batch()` at idx 3: 3 booked, all RESTORED, 50/50, kind `interrupted-after-publication` — **PASS**;
  - `os.replace` EIO before rename: own temp removed, destination baseline, original `OSError(EIO)` propagates — **PASS**; destination is a directory (real `IsADirectoryError`), `write_fn` fails mid-write (ENOSPC): temp removed, destination untouched — **PASS**;
  - `os.unlink` itself fails with `OSError`: the **original** exception is preserved, temp left — **PASS**; success path leaves no temp and no cleanup flag — **PASS**.
  - INFO (not PASS claims): (i) a **second** `KeyboardInterrupt` raised inside the cleanup `unlink` is swallowed by `except BaseException: pass`; the original is re-raised and the temp stays (the forward classifier removes its own temp; in a restore this is a D-1-class leftover). (ii) *Theoretical TOCTOU:* if a different process creates a file at exactly `<dst>.tmp` between the rename and the handler, the handler would unlink it. It needs a concurrent writer using that exact name in a microsecond window, and such a writer already collides with this harness's `O_EXCL`; no flock exists in the harness (unchanged from v6).

### 4.3 Restore-phase sweeps and a restore-targeted real-signal storm (new; this is where D-1 comes from)
`delta_probe.py restore_sweep`: opcode-level `KeyboardInterrupt` before **every instruction of `publish()`** during restore call #7 and call #12 (dirsync EIO at idx 6 forces recovery), then retry of every `RESTORE-ERROR`:

| | reached points / call | `.tmp` left, retry `FileExistsError` | other |
|---|---|---|---|
| **v7.1** | 129 | **6** (bytecode offsets **288, 290, 292, 294, 296, 300**) | 98 RESTORE-ERROR→retry reconciled 50/50; 25 restored with no error; 0 escapes |
| v7 (control) | 128 | **46** | 57 reconciled; 25 clean |

`storm_restore`: real SIGINT (`pthread_kill`) at uniform random offsets across a batch forced into recovery, 1500 trials, then retry of `RESTORE-ERROR`/`UNVERIFIABLE` entries (`restore_one`'s `safe_sha` swallows a signal and reports `UNVERIFIABLE`, fail-closed; retry reconciles it):

| | clean / reconciled after retry | `.tmp` left, retry blocked | escapes |
|---|---|---|---|
| **v7.1** (run in parallel with the control) | 1311 + 174 | **15 (1.0 %)** | 0 |
| **v7.1** (earlier solo run, kept in `old-first-storm-run/`; its `NOT-RECONCILED` 40 were `UNVERIFIABLE` entries my first categoriser did not retry) | 1272 + 138 (+40 UNVERIFIABLE) | **50 (3.3 %)** | 0 |
| v7 (control) | 1224 + 47 | **229 (15.3 %)** | 0 |

Offsets 288–300 are the instructions between `os.open(tmp, O_CREAT|O_EXCL)` **returning** and the `try:` that owns the cleanup (`STORE_FAST fd`, `progress["tmp_created"] = True`, the `NOP` of `try:`). CPython 3.12 runs Python signal handlers at the eval-breaker check after a call completes, so a signal that arrives *during* the `os.open` system call is delivered exactly at offset 288 — the most likely landing point, which matches the observed 1–3 % storm incidence. No state other than a stale `<dst>.tmp` results.

## 5. Findings

### In-scope blockers / majors: **none.**

### F-3 — RESOLVED. F-2 — RESOLVED for the claimed scenario, **PARTIAL overall** (see D-1)
Evidence in §4.2–4.3. The author's own driver (`findings-v7_1.json` 19/19, `boundary-v7_1.json` 91/91, `run21-v7_1` 21/21) is consistent with my results; its F-2 case fires only at the `os.replace` line (inside the guarded region), so it cannot see D-1.

### D-1 (MINOR, new): the F-2 fix leaves a six-instruction window between creating the temp and entering `try:`
A signal landing at bytecode offsets 288–300 of `publish()` leaves `<dst>.tmp` behind; in a **restore** that yields an explicit `RESTORE-ERROR`, and a retry of `restore_one` raises `FileExistsError` until an operator removes the temp (v7 behaviour for that instruction range). Fail-closed and explicit: no data is lost or hidden, the tree is recoverable, and in the **forward** phase the classifier still removes its own temp. Measured: 6 of 129 reached instruction points per restore call (v7: 46), 1.0–3.3 % of real restore-phase signals in my storms (v7: 15.3 %). The v7.1 docstring ("removes a temp file it created if anything interrupts it before the rename") and the packet wording ("interrupted before `os.replace` completes") therefore over-claim by this window. It cannot be closed by moving the `try:` alone (a gap remains between `open` returning and recording ownership). Suggested closure, either is sufficient and neither blocks this approval: **per-call unique temp names** (so a stale temp can never block a retry), and/or the deploy-time signal mask recommended for R-1. At minimum, the result handoff should say "F-2 mitigated, not eliminated".

### R-1 — unchanged, **still open** (carried forward, not closed)
`batch()`'s own frame after the forward phase / during rollback is unguarded exactly as in v6/v7: the sweeps reproduce it identically (14 single-signal "escaped-dirty" points in `batch/k1`; 65 recovery-variant escape points of 1758). 0 escapes in 2000 forward-path and 0 in 1500 restore-targeted real-signal trials. **Decision on whether leaving it as a documented residual is acceptable for a scratch-harness fix under the packet: yes, with conditions** — (a) every artifact that reports this work (result handoff, status line, README) must list R-1 as **OPEN**, never "closed"/"all windows closed"; I could read only the v7.1 docstring, which **does not mention R-1**, and I did not see the author's final handoff, so (a) must be confirmed at hand-off review; (b) it is carried as a **hard requirement into the deploy design**: block SIGINT/SIGTERM/SIGHUP (`signal.pthread_sigmask`, or a deferring handler checked after rollback) from before the first publish until rollback completes, plus the packet's off-volume backup and post-interruption full hash reconciliation (this also closes D-1's window); (c) no claim that the scratch harness proves a production deployer's behaviour. Rationale: not one of the two #752 windows, not a regression (same structure in v6 per the prior review; I did not re-run v6), unhit by any of the 3500 real-signal trials, and a test harness is not the deploy critical section.

### NIT
- **N-3:** the cleanup `except BaseException: pass` swallows a *second* `KeyboardInterrupt` once (INFO case §4.2). Intentional fail-closed, but a user's second Ctrl-C is eaten.
- **N-4:** `classify_interrupted_step` is unchanged, so `info["tmp_removed"]` is no longer populated when `publish()` removed the temp first (the new `progress["tmp_removed_by_publish"]` is not copied into the evidence). Cosmetic.
- **N-5:** `review_driver.py` is layout-coupled to v7 (docstring anchoring); use the adapted copy `drivers/review_driver_d71.py` for v7.1 or later.
- Prior NITs N-1 (restore-phase signal after `os.replace` reports `RESTORE-ERROR` for a file that is restored; seen again in `e5-after-replace` INFO cases) and N-2 (`os.fdopen` gap) stand; N-2's gap is the same region as D-1.

## 6. Residuals and out of scope (never claimed closed)
R-1 and D-1 above. SIGKILL, kernel crash, power loss, SIGSTOP (not testable in-process); TOCTOU between pre-state read and rename (no flock; unchanged from v6); concurrent writers; non-exFAT, Windows, Unity; any real deploy script, D-091 runtime, the merged README, the five paths. The opcode models are stricter than real delivery; storms are statistical, not exhaustive (storm random offsets use a fixed seed `754`).

## 7. Live read-only recheck
`/media/jer/ZX20/CCF-Ops`: **50 files**; manifests taken at fixture time (18:16Z), mid-run (`live-manifest-recheck1`) and at the end (`live-manifest-final`) are **byte-identical** to each other and to my replica manifest; `board/active.md` and `lanes.json` mtime `17:28:31Z` (the #755 change recorded by the prior review), `deployment.json` unchanged. I opened nothing under live Ops for writing. Author dir and prior review dir `sha256sum -c` clean; v7.1, v7, v6 original and the receipt fixture re-hashed unchanged. The primary working tree `/media/jer/Files/CCF-Primary` was not touched (publish used a separate worktree). No Unity run; no deployment.

## 8. Status (this delta re-check only)

| SOURCE (v7.1 scratch harness) | REVIEWED | STAGED | DEPLOYED | UNITY |
|---|---|---|---|---|
| authored by Opus 5.5 (new file; v7 left intact) | **APPROVED (scratch harness, scope above)** with **D-1 (MINOR) and R-1 (open) carried forward** | not changed by this review | **NOT DEPLOYED — no live install authorised** | **NOT RUN** |

## 9. Not established
Not an approval of any live install, deploy script, D-091 Unity-lock runtime, the five deploy paths, the merged README or the receipt. Not tested: power loss/SIGKILL/SIGSTOP, other filesystems, concurrent writers, Windows, Unity. Not independent of the Claude model family. D-1 and R-1 are open.

## 10. Commands and exit codes
All Python as `python3 -I -B`; `S=/media/jer/ZX20/CCF-Temp/Sen1-754-Delta-20261118T181603Z`, `F=$S/fixture`. `gh issue view` not used (`gh api` only).

| Command | Exit |
|---|---|
| `exfat_qual_v7_1.py $S/run21 $F/replica $F/replica-inventory.sha256 $F/staged $F/receipt/deployment.json` | **0**, 21/21 |
| `review_driver_d71.py src/exfat_qual_v7_1.py W $F OUT named,negatives,extra,extra2` (and same on v7) | 0 / 0 |
| `… sweep` / `… sweep_double` / `… sweep_batch_fail` / `STORM_TRIALS=2000 … storm` (v7.1) | 0 / 0 / 0 / 0 |
| `delta_probe.py src/exfat_qual_v7_1.py W $F OUT f2,f3,safety` (v7.1 / v7 control) | 0 / 0 (22P-2I-0F / 6P-2I-16F) |
| `delta_probe.py … restore_sweep` (v7.1 / v7) | 0 / 0 |
| `STORM_TRIALS=1500 delta_probe.py … storm_restore` (v7.1 / v7) | 0 / 0 |

Process notes (disclosed): the first unmodified-driver run on v7.1 (15 spurious FAILs, §4.1) was discarded; an initial batch of background jobs was killed by my own `pkill`/self-match and rerun from clean work dirs; the first v7.1 restore storm categoriser did not retry `UNVERIFIABLE` entries and was rerun (both kept). Exact invocations: `delta/COMMANDS.txt`.

## 11. Artifact SHA-256 (durable dir `…/Sen1-754-BookingGapFix-20261118/delta/`; `SHA256SUMS` itself ``d1846eb1fb43a71b3c872679f2b2729ec7118055c86d7ed25d98ff486894e8a6``)

| SHA-256 | file |
|---|---|
| `37c93f362b19ff8e2df56ebdbc7ca5eb06d3ff62272b40e485bb12c907e15450` | ./COMMANDS.txt |
| `3aaeb99a1e98e7a1305a71e363de3940527c1ff15b9765cf30498d245b5494e1` | ./drivers/delta_probe.py |
| `f33e3615cda95e26845c37165cdab80c75ef0954ebcf16e131971422d7b9ff6f` | ./drivers/review_driver_d71.py |
| `67fbea56448f30abe83dc20277b6e293b5b4f012174534d1d1c5978ef75ee47c` | ./drivers/review_driver-to-d71.diff |
| `934ccba7014ebafcf3ca51eec3ffd66bdf5d37a2f4976ce3491bc31534847fd2` | ./fixture-manifests/live-manifest-before.sha256 |
| `934ccba7014ebafcf3ca51eec3ffd66bdf5d37a2f4976ce3491bc31534847fd2` | ./fixture-manifests/live-manifest-final.sha256 |
| `934ccba7014ebafcf3ca51eec3ffd66bdf5d37a2f4976ce3491bc31534847fd2` | ./fixture-manifests/live-manifest-recheck1.sha256 |
| `934ccba7014ebafcf3ca51eec3ffd66bdf5d37a2f4976ce3491bc31534847fd2` | ./fixture-manifests/replica-inventory.sha256 |
| `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855` | ./old-first-storm-run/probe-v7_1-storm-restore.err |
| `6dbf06fef82b2d107ca187e14d87f0c0dc8704cbe668c120b490c4b3ae233e92` | ./old-first-storm-run/probe-v7_1-storm-restore.json |
| `1969df009b8a19e0f427270300264e11173bfe0c4c1b2abd68efa0028a3502f9` | ./old-first-storm-run/probe-v7_1-storm-restore.log |
| `d078716d0722418c5f1c1f39e0c52a9a2cfae6be4d0df8895ba640043a129233` | ./results/driver-v7_1-named-negatives-extra.json |
| `5c60ec9c561e5d1a91c4a405fb517814d550b16add85b5dc1f4c2d82255d48f3` | ./results/driver-v7_1-named-negatives-extra.log |
| `f5c56fc84fc9c304f0fad3ad1e627af0af1144f8d488ede7bb3cab9ccf98bc6f` | ./results/driver-v7_1-storm.json |
| `3ed2c516dbc1122292bfcd8bd4fa9607765481b95eda2b5bd7e771900b32f2e1` | ./results/driver-v7_1-storm.log |
| `560e7d023c4f4185c94859359a727d4c1812185e0ae03dabf82ec0dadd56c81d` | ./results/driver-v7_1-sweep-batch-fail.json |
| `33ac22a2ccf88ca83f2080b7ab68b9792a3c420e9db662a1fb4d27997f9f39b9` | ./results/driver-v7_1-sweep-batch-fail.log |
| `5be37f1f2e6a858259682f182c063e292f990e6cd15df709a7e1eb64957116c6` | ./results/driver-v7_1-sweep-double.json |
| `1538d5f425aaff49051edbc4f14dde2015e10a5ae304b7a4b455fbe181667648` | ./results/driver-v7_1-sweep-double.log |
| `bdd978e5b0feed4676b8685ac1c2fd657c66f5b48b0fde8fe6f61c8b06515c4f` | ./results/driver-v7_1-sweep.json |
| `199c9cc7bbb8ed810ce7e2b56adfee5fca235cdd98e7c5baa2e3c64976b966da` | ./results/driver-v7_1-sweep.log |
| `f95335cc546b5f3714a19e314177fea5e975b45cbe83d6c182edfe5afa96c01c` | ./results/driver-v7-control-named-negatives-extra.json |
| `314187394096d91e731520b58be446d683cb16cb90345b30688bb4230b788c6b` | ./results/driver-v7-control-named-negatives-extra.log |
| `ae7e23f154c3f0696516917692d9492e69cf4399fedfd4032baf2cf09496950d` | ./results/main-body-comparison.txt |
| `ad77a323512591ca54f4b5019233e3b587f3a44309c59c9293a0ca6a701eb9b1` | ./results/probe-v7_1.json |
| `22040e14226207f79cdef45a8e398db75d214ad3448e890eff777a706a43eeda` | ./results/probe-v7_1.log |
| `f600d45d289d965912eb8ef194f13fdcb53f05d014f3eee9a291f53afda57bb6` | ./results/probe-v7_1-restore-sweep.json |
| `5767c94488aeb1c10403fd1e2cbc7d41e2ad0729c2000cb00f8f530d81a85861` | ./results/probe-v7_1-restore-sweep.log |
| `4ab3c20d2bd87761534aec241bc75e51e83b3544d5fc75c5c369842a9d1db0a3` | ./results/probe-v7_1-storm-restore.json |
| `610530990683aa4b2ad1315ea4d1d6f58ffb993934023218de6fb53922f750b6` | ./results/probe-v7_1-storm-restore.log |
| `3cca45565828e6a9ea617b3b9e3c0caaf58a3ab99b5f262a8fb4b0aeb78ee71b` | ./results/probe-v7.json |
| `a05af6603a64adaac817dcb9cef7b4e58f63628984f506f5711a99ed7141b32c` | ./results/probe-v7.log |
| `11bc3c82586fa86d4128167ba447fabe8f1515ecd68dbef1e44fa67b2221803c` | ./results/probe-v7-restore-sweep.json |
| `78ab2989d120f714f1d93547b9b1774beea17f69b79c9f1a859d228644f56189` | ./results/probe-v7-restore-sweep.log |
| `bc7ba5400563c760d43e64a967b6880f803b1a7e09f7df0e035a249f2466a52b` | ./results/probe-v7-storm-restore.json |
| `8ade4aa44b9c553ea583c537b3f9e4f3066162adf3a2789a5636ad13b2b81968` | ./results/probe-v7-storm-restore.log |
| `05b4c523f651f6f7aea36b4f555492bed927398d97d62dc6acc9a5c5a6d75762` | ./results/run21-v7_1-result.json |
| `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855` | ./results/run21-v7_1.stderr |
| `c2af83953bec0c738451c0fc87b04dcd38a111be5421c3863bd65e48bc1ca2d3` | ./results/run21-v7_1.stdout |
| `267ccb47a52931248423751c7ab98040f30612be636454a3bfd7f082b149268a` | ./v7-to-v7_1.reviewer.diff |

The review document itself is not self-hashed; its SHA-256 is recorded in the #754 hand-off comment and in `delta/SHA256SUMS-doc`.
