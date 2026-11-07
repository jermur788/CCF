# #323 Workbench runner R1–R5 bounded verifier repair — author result

**Status: AUTHOR HANDOFF. This is source and mock verification only; it is not accepted, integrated or Unity-qualified.**

- All five #319 findings are repaired in the runner and harness source. Each repair has a negative case that the old #318 runner or bytes fail and the new code passes.
- All prior #318 gates are preserved byte-for-byte or by count.
- No Unity Editor, import, lease or Library was used. Gates C–G, the native former-site 720p/1080p witness and Legacy40/Reference save/load are **NOT RUN**. B3 (lost P0604 / gained P1209 tree) stays **BLOCKED**.
- Independent review [#324](https://github.com/jermur788/CCF/issues/324) and the Manager #200 verdict are still required.

Binding packet: [`MANAGER-WORKBENCH-RUNNER-R1-R5-CORRECTION-PACKET-20261107.md`](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-WORKBENCH-RUNNER-R1-R5-CORRECTION-PACKET-20261107.md). Governing ruling: [`MANAGER-322-CLOSURE-DISPOSITION-AND-NEXT-GATES-20261107.md`](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-322-CLOSURE-DISPOSITION-AND-NEXT-GATES-20261107.md). Findings: [#319 review](https://github.com/jermur788/CCF/blob/docs/workbench-287-verifier-closure-independent-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-287-VERIFIER-CLOSURE-INDEPENDENT-REVIEW-RESULT.md).

## Identity

| Item | Value |
|---|---|
| Branch | `fix/workbench-runner-integrity-20261107-v1` (new) |
| Base (exact published #318 result) | `6b7d2d45a21824f4325bc3c176f7e276dc7299e6` |
| Tested source commit | `e94267695b4cfef05ba012010ddbf5db625be0ec` (the five source files) |
| Published head | the commit that adds this document; its only parent is the tested commit, and it changes no tested byte |
| Lane | `/media/jer/Files/CCF/Operations/CCF-WBRunnerClosure-20261107` (new Files canonical-store worktree) |
| Evidence (`E/`) | `/media/jer/Files/CCF/Verification/WorkbenchRunnerR1R5-20261107/` |
| Disposable non-Unity clone | `E/clone`: a `--shared` sparse clone (`/Tools/Verification/`, `/Assets/**/*.cs`) detached at the tested commit. It has no `Library`, no `.unity` files and no Unity process. |
| Worker (self-report) | T3 Code, Claude harness, `claude-opus-5-5` (Claude Opus 5.5), effort high as dispatched (I cannot observe the effort level myself), Anthropic provider family; account/quota UNKNOWN. Sole author; no subagents; no review or acceptance of my own work. Dispatched by the #327 Mimo orchestrator (ledger Entry 004). |
| Claim | [#323 RUNNING](https://github.com/jermur788/CCF/issues/323#issuecomment-6707683144) |

**Preflight (before any write):**
- The target branch was absent locally and on origin. The target path did not exist, and no existing worktree used it.
- There were no Git lock files in the bare store. The store is `/media/jer/Files/CCF/Repository.git`, with origin HTTPS reachable.
- `git ls-remote` showed the #318 branch at `6b7d2d45`. Files is `/dev/sdb4`, ext4, with 37 GiB available.
- Competing writers: #322 is CLOSED; #325 and #326 are HANDED-OFF; #324 is WAITING; no runner process was running.
- After creation: HEAD equals the base, and `status --porcelain=v1 -uall --ignored` was empty.

## Changed files (exact inventory vs `6b7d2d45`)

`git diff --name-status 6b7d2d45 HEAD`:

| Path | Why |
|---|---|
| `Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py` | R1, R2, R3 |
| `Tools/Verification/WorkbenchRetirement/test_runner.py` | 22 new R1–R3 controls. Shared helpers moved into the test-free `RunnerHarness`. The 13 prior test bodies are unchanged. N5: the default scratch folder is now the system temp dir, not #318 evidence. |
| `Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs` | R4 labels; R5 falsifiable teaching controls |
| `Tools/Verification/RackV2Persistence/test_historical_projection.py` | R4 label assertions with a negative control; R5 helper controls and wiring checks; the `--out` guard also accepts the #323 evidence dir |
| `Tools/Verification/StandGeometry/Enlarged80Verification.cs` | R4: the one Pass label (and its comment) that #319 named |
| `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RUNNER-R1-R5-CORRECTION-RESULT.md` | this report |

The tested-source diff is 5 files, +628/−54. `git diff --check` is clean. `git diff 6b7d2d45 HEAD -- Assets Packages ProjectSettings .vscode` is **0 lines**. There is no production, scene, package, material, save fixture, Reference Future, baseline, CCF-Ops, `active-tasks.md` or `main` change. `HistoricalLayout` is byte-identical in both harnesses.

## Per-fix evidence

The gate files are under `E/gates/<id>.{cmd,stdout,stderr,exit}`, run by `E/scripts/gates.sh e9426769…`.

### R1 MAJOR — post-run clean-candidate provenance: FIXED (source/mock)

**Change:**
- After the Editor exits, the runner removes the harness and `.meta` it staged and the licence copy.
- If the lane was admitted clean, the runner then runs `git status --porcelain=v1 -uall -z`. The `-z` form is the same porcelain v1 data and keeps the leading status column intact.
- **Any** entry is a FAIL, recorded as `postrun_lane_drift:<n>`. Each drifted path is recorded in **both** receipts with:
  - its status;
  - the rename source, if any;
  - the worktree kind: file, symlink, directory or absent;
  - the worktree SHA-256;
  - its `HEAD:` blob id.
- The JSON receipt gets `postrun_lane_status` (`clean` / `drift` / `error` / `not_checked_no_clean_admission`) and `postrun_drift[]`. The markers receipt gets one `POSTRUN_LANE_STATUS` line and one `POSTRUN_LANE_DRIFT` line per path, with the path JSON-quoted.
- A failure of the status command is itself a FAIL (`postrun_status_error:<Type>`).
- Receipts hold hashes only, never file contents. Licence handling is unchanged: SHA prefixes only, and the copy is deleted.
- Nothing is restored, cleaned or waived. The tests assert that the drift is still in the lane after the run.

**P7 reproduction:**

| Run | Result |
|---|---|
| Verbatim #319 `probe_runner.py` vs the **old** runner (gate 06) | `P7 Editor rewrites tracked Packages during run`: **exit 0, failures `[]`**. False green reproduced. |
| Adapted probe vs the **new** runner (gate 07) | P7: **exit 1, `postrun_lane_drift:2`** |

The adaptation is shown in gate 07b. It is one block in the Editor mock that answers the new runner's `git status -z` and `rev-parse` subprocess calls. The probe logic is unchanged.

**New negative and positive controls:** 11 tests, all `ok` at the tested commit.

| Kind | Tests |
|---|---|
| Mocked status | P7 manifest + lock rewrite → 1, both paths, hashes in both receipts, no private bytes; clean → 0 (`clean`); status command error → 1; no clean admission → `not_checked…` and no launch |
| **Real temporary git repository at the exact committed HEAD** (only the lease query and the Editor are mocked) | clean → **0**; Editor rewrites `Packages/manifest.json` → **1**; rewrites `Packages/packages-lock.json` → **1**; rewrites material `Assets/Materials/Bark.mat` → **1**; leaves an untracked asset → **1**; deletes a tracked file → **1** (`absent`); staged + unstaged multi-path → **1** with 2 entries |

In each real-repo case the receipt's `head_blob` equals the real `git rev-parse HEAD:<path>`, `worktree_sha256` equals the SHA-256 of the drifted bytes, and `identity.head` equals the real HEAD.

### R2 MINOR — truthful receipts on write failure: FIXED

**Change:**
- The markers receipt is attempted first. The exit code is then recomputed, and only then is the JSON serialized. The durable JSON therefore always carries the final exit code and every failure, including `receipt_write_error:markers:<Type>`.
- Both receipts are written atomically: write `<name>.partial`, then `os.replace`. A failed write leaves no partial or stale receipt.
- If the JSON write fails, the printed result says `exit_code: 1` with `receipt_write_error:json:<Type>`. A best-effort `RUNNER_RECEIPT_ERROR json:<Type> runner_exit=1` line is appended to an existing markers file. No other receipt is invented.
- Harness, `.meta` and licence cleanup still happen first.

**Negative cases:** 4 tests, all `ok`.
1. The markers write fails → the on-disk JSON has `exit_code: 1`, the markers failure is listed, and there is no markers file and no `.partial`.
2. The JSON write fails → there is no JSON file and no `.partial`; the markers file ends with the error line; stdout says exit 1.
3. The markers write fails on a gate that also failed → every failure is kept in the JSON.
4. The evidence directory really becomes unwritable (`chmod 0555` during the run, as a non-root user) → **no durable receipt exists**. Only the log and the config dir remain. Stdout has exit 1 with both write errors, and cleanup was still completed.

With the reviewer's P4/P4b against the new runner, the on-disk JSON now lists `receipt_write_error:markers:IsADirectoryError`. For the old runner it listed `[]`.

### R3 MINOR — exact terminal PASS per gate: FIXED

**Change:** `TERMINAL_PASS` maps the three known gates to their exact tokens:
- `WorkbenchRetirementVerification` → `WORKBENCH_RETIREMENT_VERIFY_PASS`
- `Enlarged80Verification` → `ENLARGED80_VERIFY_PASS`
- `RackV2PersistenceVerification` → `RACKV2PERSIST_VERIFY_PASS`

An unknown gate, or a `--pass-token` that differs from the gate's own token, is refused by argparse with exit 2. The refusal happens before any staging or launch, and before the evidence directory is created. The original Workbench-specific check is kept.

In the log, **exactly one** line must start with the token followed by whitespace or end of line:
- none → `missing_terminal_pass`;
- more than one → `duplicate_terminal_pass`;
- the gate's own `_FAIL` token anywhere, including `…_FAILED` → `gate_fail_marker`.

The original generic FAIL, compiler-error and editor-exit checks are unchanged.

"Exactly one" matches real Editor logs. A read-only scan of retained Verification logs found exactly one start-of-line terminal PASS in each Unity log: 1 Workbench, 18 Enlarged80 and 6 RackV2 logs. Two driver logs only echo the token inside a longer line, so they do not start a line with it.

**Negative cases:** 7 tests, all `ok`.
- P8 `RACKV2PERSIST_CHECK` as the token is refused, with no launch.
- Arbitrary tokens and every cross-gate token are refused, for all 3 gates.
- An unknown gate is refused.
- A Rack log holding only `RACKV2PERSIST_CHECK` lines fails.
- Per gate, 9 verdict cases each: duplicate PASS, empty log, `…PASSED`, `…PASS_X`, `…PASS.x`, indented, prefixed, FAIL+PASS and PASS+`…FAILED`.
- Positive control: each gate passes with its own token.
- The table is cross-checked against the PASS/FAIL literals in the three harness sources.

### R4 MINOR — honest synthetic labels: FIXED

**Change:**
- RackV2: every label for a projected v20/v21 file now says "synthetic projected …". This covers sections 7–8 in v2 mode, sections 1–3 in legacy40 mode and the header comment. The header now states that the projections are not archived genuine saves.
- Three section-6 refusal labels that act on v24 files now say so: "current v24 file missing nextRackId (required since v21)", "… missing racks …" and "current v24 V2 file relabelled with a pre-v21 header".
- Enlarged80: `Pass("synthetic v19-layout Legacy40 save into an Enlarged80 world …")`.

The real Legacy40 and frozen Reference Future v1 requirements, the explicit refusals and the archive/hash checks are unchanged.

**Assertion and negative control (gate 08):** `SYNTHETIC_LABELS_PASS rack=0 enlarged80=0 misleading; negative control #318 rack=24 enlarged80=1 found`.
- Every string literal outside comments that names v19/v20/v21 must contain "synthetic" or appear in a 7-entry allow-list. The allow-list holds projection rules, "required since", the relabelled-header refusal and one explicitly "projected" diagnostic.
- The same scan over the #318 bytes finds the 24 + 1 misleading labels that #319 reported, so the assertion can fail.

### R5 MINOR — falsifiable teaching-history checks: FIXED with narrowed claims

**Absent flag (v2 mode):**
- The old in-memory check ("absent teaching history stays false") is removed. It could not fail: `JsonUtility` gives a missing bool the value false.
- New positive control: a compact v24 file recording `true` is written to disk and loaded through the real Continue path (`CanLoad` + `Load`), which must give `true`.
- New check: the absent-flag file goes through the same disk path and must load `false` **over a session that held true**. The fixture is asserted to really omit the key.
- **Narrowed claim (stated in code):** production has no JSON-presence validator for this key. "Missing means false" is its schema policy (`ForestSaveData` v24 comment; `ScenarioOneManager.cs:1484`). So this proves the absent flag is applied as false and never inferred. It does not prove that a validator inspected the key.

**Cold start:**
- The equality check is kept but narrowed to "session matches the seeded file's **explicit** boolean". The seeded file must state the key as a JSON boolean.
- A new control then loads the opposite flag from disk, then the seeded file again. Each load must change the session to the file's value. The control also re-checks the expected signature and world hash, and the existing "Continue does not rewrite the save file" check stays before it.

**Shared helper:**
- `TeachingLoadDiscriminates(sessionBefore, expectedAfter, actualAfter)` passes only when `sessionBefore != expectedAfter && actualAfter == expectedAfter`.
- `test_historical_projection.py` compiles the **actual** helper from the harness with Roslyn and runs it: `TEACHING_CONTROLS_PASS checks=6`. That is 2 positive controls and 4 negative controls (two trivial same-value comparisons, two ignored loads).
- Wiring is checked too: `TEACHING_CONTROL_WIRING_PASS`. The disk-true precondition comes before the absent-flag disk load, and cold start makes two discriminating calls.

The 28 prior synthetic projection controls, the historical schema field-boundary audit and the v24 round-trip and stray-v23-flag controls are unchanged. The C–G end-to-end Unity obligations are unchanged; nothing was deleted.

## Preserved prior gates

| Prior gate | Result at the tested commit |
|---|---|
| #318 runner mocks, 13 including N1–N3 | **13/13 ok** (gate 03). The 13 test method bodies are **byte-identical** to `6b7d2d45` (gate 04, `PRIOR13_IDENTICAL count=13`). |
| 28 C# synthetic projection controls | **`HISTORICAL_PROJECTION_PASS checks=28`** (gate 08); 28 `PROJECTION_PASS` lines. The C# control block, the schema audit and `HistoricalLayout` in both harnesses are byte-identical to #318 (gate 09). |
| Historical schema field boundaries v19/v20/v21 | `HISTORICAL_SCHEMA_FIELD_BOUNDARIES_PASS` (gate 08) |
| Old-runner behaviour the new tests must not demand | With the new test file loaded against the **old** runner (gate 05): the 13 prior tests pass and 20/22 new tests fail or error, so they detect R1–R3. The 2 that pass are the positive controls and the missing-terminal case, which the old runner already handled. |
| Tree/hash equality, Reference hash `7AD177B3CC2F73C7`, Enlarged80 final `CurrentHash() == startAnchor`, frozen v1 archive checks | Unchanged source; nothing in the diff touches them. Runtime **NOT RUN**. |
| Current baseline / production | No production byte changed (0-line production diff) |
| Pre-run dirty-lane refusal, identity, lease, licence and cleanup checks | Unchanged and still covered by the 13 prior controls. The new post-run check is additive. |

## Mutation matrix — every new control can fail

Gate 11 (`E/scripts/mutations.py`, `E/tmp/mutants/mutations.json`) killed **16/16** mutants. Each mutation removes the behaviour that one control guards:

| Fix | Mutants |
|---|---|
| R1 | no post-run check; untracked entries ignored; drift found but not failed |
| R2 | JSON serialized before the markers attempt (the #318 shape); non-atomic partial write |
| R3 | duplicates accepted; token map bypassed; `\b` instead of whitespace/end; unknown gate allowed |
| R4 | Rack label reverted; Enlarged80 label reverted; a new unlabelled v20 claim |
| R5 | trivial helper; absent flag back in memory; no true precondition; cold-start opposite flag dropped |

Each mutant exits nonzero for its intended cause. For example, M4 fails at "false-seeded comparison with a false session is rejected as trivial". The R4 and R5 mutants M5–M7 are caught by source assertions; the R5 runtime behaviour itself needs Unity.

## Commands and exits

All run in `E/clone` at `e9426769`, except gate 14.

| Gate | Command (summary) | Exit | Counts |
|---|---|---|---|
| 00 | clone identity | 0 | HEAD `e9426769`, 0 status entries, no `Library`, 0 `.unity` files |
| 01 | `python3 -I -X pycache_prefix=E/pycache -m py_compile` (3 changed .py) | 0 | — |
| 02 | `CCF_TEST_SCRATCH=E/tmp python3 -I -B test_runner.py -v` | **0** | **35/35 ok** (13 prior + 22 new) |
| 03 | `… test_runner.py -v RunnerControls` | **0** | 13/13 |
| 04 | `compare_prior.py 6b7d2d45` | 0 | 13 identical |
| 05 | new `test_runner.py` vs the **old** runner (expected red) | **1** (expected) | 13 prior ok; 26 failures + 5 errors, including subtests; 20/22 new red |
| 06 | verbatim #319 probe vs the old runner | 0 (script) | P7 **0**, P8 **0**, P4 JSON `[]`; 8/35 mismatches as #319 reported |
| 07 | adapted probe vs the new runner | 0 (script) | P7 **1** (`postrun_lane_drift:2`), P8 refused (argparse exit 2), P4/P4b record the write error; 7/35 mismatches (see the note below) |
| 08 | `test_historical_projection.py --out E/projection` | **0** | 28 + 6; labels 0 misleading (negative control 24 + 1); wiring PASS |
| 09 | `compare_projection.py 6b7d2d45` | 0 | 4 identical |
| 10a | Roslyn (`csc` from SDK 8.0.133, C# 9, `UNITY_EDITOR` + Unity/Input/Standalone defines): 189 production non-Editor files + the 3 tested harnesses | **0** | 0 errors, 45 warnings, none in `Tools/Verification`. Warning codes: CS0649 ×39, CS0169 ×3, CS0108, CS0219, CS0414, CS1701; CS0618 suppressed. |
| 10b | the same with the `6b7d2d45` harnesses | 0 | 0 errors, 45 warnings |
| 10c | 10a plus a file with an undefined name (negative control) | **1** (expected) | 1 error, CS0103 |
| 11 | `mutations.py` | 0 | 16/16 killed |
| 12 | `git diff --name-status/--check/--stat 6b7d2d45 HEAD`; production-path diff | 0 | 5 source files, diffcheck ok, 0 production lines |
| 13 | `git status --porcelain=v1 -uall --ignored` in the clone | 0 | empty (see the note below) |
| 14 | read-only protected readback (`GIT_OPTIONAL_LOCKS=0`) | 0 | #318 lane `6b7d2d45` with 0 entries; ZX20 lane `4d95e452` with exactly its 8 dirty paths; lane = dirty backup 8/8; `sha256sum -c` 16/16 OK; lock `free`, PIDs `[]`; no Editor process |

Roslyn references are the 320 Unity/.NET DLL copies on Files from #325. All 320 were re-verified against #325's SHA-256 list before compiling. Nothing was read from the ZX20 Library for compilation.

Notes on gate 07 and gate 13:
- **Gate 07:** of the 7 remaining probe mismatches, 5 are #319 Note N1 (FAIL spellings or exceptions printed alongside a PASS). That is unchanged and outside the R1–R5 scope. One is P8, now refused earlier by argparse (exit 2) instead of the probe's expected 1. One is the P12 symlink loop, which fails closed as #319 accepted.
- **Gate 13:** the first run of gate 01 wrote 3 `__pycache__` files into the **disposable clone**, because `-I` ignores `PYTHONPYCACHEPREFIX`. They are recorded in `13-first-run-pycache-in-disposable-clone.txt`. I removed them, switched to `-X pycache_prefix`, and reran gates 01 and 13 (`E/scripts/rerun_01_13.sh`). No registered or author lane was affected; my author lane's status stayed empty.

## Honest limitations

1. **No Unity at any point.** The new C# teaching controls, the cold-start control and the relabelled runtime paths compile, but have **not executed in Unity**.
   - They write Newtonsoft compact text to the isolated slot. From source reading, production's JSON validators parse with `JObject`, so they do not depend on formatting. That is not runtime proof.
   - `RACKV2PERSIST_VERIFY_PASS … checks=N` will rise by 1 in the v2 and coldstart modes; I found no fixed expectation for N.
2. **R1 in a real Editor run:**
   - The check uses porcelain without `--ignored`, so gitignored Editor output such as `Library/` is out of scope by design. Any non-ignored file the Editor creates is a FAIL, which is strict on purpose.
   - **The known Editor rewrite of Packages from 1.1.0 to 1.1.1 and of five materials will now make every real run FAIL** until Manager decides the package/material policy. That is the intended provenance result, not a defect.
3. R1–R3 are proven with mocked Editor and lease processes. The real-git tests use a temporary repository, not the CCF Unity project.
4. R4 is a label rule over string literals, with an explicit allow-list. `Tools/Verification/RackV2Persistence/README.md` (lines 20 and 22) still describes "v20 Enlarged80 save" and "v21 and v20 Legacy40 saves" without "synthetic". It is **outside this packet's allowed files**, so I left it unchanged and flag it for a later docs edit.
5. R5's source wiring (M5–M7) is checked by text assertions. The helper is executed for real, but only offline.
6. Small behaviour changes for review:
   - the receipt failure label is now `receipt_write_error:markers|json:<Type>` (it was `receipt_write_error:<Type>`);
   - unknown gate names are now refused, so a future gate must be added to `TERMINAL_PASS`;
   - the test scratch default is now the system temp dir.
7. #319 notes N1 (FAIL spellings and exceptions alongside a PASS), N2 (Editor stdout/stderr not captured), N3 (`CCF_TEST_DIRECT_ENTRY` inheritance), N4 (no documentation or positive control for the empty-plan model-1 drop) and N6 are **not addressed**. They are outside R1–R5. N5 is addressed (scratch default). N7 needs no action.

## NOT RUN / still open

- Unity Workbench gates **C–G** on the repaired verifier: NOT RUN.
- Native former-site Workbench witness at **720p/1080p**: NOT RUN.
- **Legacy40 / Reference** save/load runtime and the RackV2 v2/legacy40/coldstart modes, including the new teaching controls: NOT RUN.
- Enlarged80 runtime, including F: NOT RUN.
- **B3** tree substitution (P0604 lost, P1209 gained): **BLOCKED**, cause unproven. Nothing was rebaselined; production is unchanged.
- Package 1.1.0 vs 1.1.1 and material provenance: no restore, update or commit. It waits on a Manager decision. The eight protected dirty files are untouched (8/8 equal to their backup, 16/16 SHA OK).
- Independent review **#324** and the **Manager #200** source verdict are pending. #287 remains NOT QUALIFIED. No PR, merge or Windows/Player build.
