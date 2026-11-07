# #328 Stage B — Workbench R324-01 receipt finalisation — author result

**AUTHOR HANDOFF — SOURCE AND MOCK ONLY. Not approved, not integrated, not Unity-qualified.** Independent review is #329. This worker does not accept its own work. No Unity Editor, import, lease or Library was used; the real Workbench gates C–G remain **NOT RUN**.

Binding authority: [Manager packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-327-NEXT-SOURCE-CLOSURE-PACKET-20261107.md), [ruling B](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-327-DISPOSITION-AND-VERIFIER-ARCHITECTURE-20261107.md), and the [#324 finding R324-01](https://github.com/jermur788/CCF/blob/docs/workbench-runner-r1-r5-independent-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RUNNER-R1-R5-INDEPENDENT-REVIEW-RESULT.md). Claim: [#328 Stage B RUNNING receipt](https://github.com/jermur788/CCF/issues/328#issuecomment-6710778562).

## Identity

| Item | Value |
|---|---|
| Branch (new) | `fix/workbench-receipt-finalization-20261107-v1` |
| Base (exact published #323 head) | `2c4aea1c94a0768c684376bc027ba15c5af5146f` (author-tested source there: `e94267695b4cfef05ba012010ddbf5db625be0ec`) |
| **Tested commit** | **`a4019e6b96b50e03d2b71d92c44fc30a0f7ace29`** (the two source files) |
| Published head | the commit that adds this document; it changes no tested byte (`git diff a4019e6b <head>` is this file only) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-WBReceipt-20261107` (new, sole author; store `/media/jer/Files/CCF/Repository.git`) |
| Evidence (`E/`) | `/media/jer/Files/CCF/Verification/WorkbenchReceipt-328-20261107/` |
| Worker | T3 Code / Claude Code harness, model **`claude-sonnet-5-5` (Sonnet 5.5)**; reasoning effort not observable by me. Not the packet's recommended Sol High. Same owner as Stage A, run serially after Stage A was published and released. |

**Preflight (before any edit):** live `active-tasks.md` re-read from `origin/main` `a37089e1` (no owner of the #323 lane); `git ls-remote` showed `fix/workbench-runner-integrity-20261107-v1` at `2c4aea1c` (= packet); target branch and path did not exist; no Git locks; no Unity/Editor/runner/compiler-server process; Files 34 GiB free. The worktree was clean (tracked/untracked/ignored) at the start and after every gate. The previous author lane `CCF-WBRunnerClosure-20261107` is at that head and clean; it was not touched.

## What changed (exact inventory vs `2c4aea1c`)

`git diff --name-status 2c4aea1c a4019e6b`: **2 files**, +212/−12. `git diff --check` clean.

| Path | SHA-256 | Blob |
|---|---|---|
| `Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py` | `d7df6401…6d1a` | `9352503a26656b20c209ceed851a77ade4a19690` |
| `Tools/Verification/WorkbenchRetirement/test_runner.py` | `1b683522…c8b9` | `c00b43b353c5f8b94c3f543b31f32ff13a1a2ac6` |

The diff against `Assets`, `Packages`, `ProjectSettings`, `.vscode`, `Tools/Verification/RackV2Persistence`, `Tools/Verification/StandGeometry` and `WorkbenchRetirementVerification.cs` is **0 bytes**. No Unity/historical source, package, save, Reference Future v1, Legacy40, production or protected-file change. Nothing was read from or written to the registered ZX20 Workbench lane; its eight protected dirty files were not opened.

## The receipt protocol

Before, the markers file was written first and contained the raw Editor `…_PASS` line and `POSTRUN_LANE_STATUS clean`. If the JSON write then failed and the fallback append failed too, that file survived with no failure indicator (R324-01; process exit was already 1). Now there are three writes, each atomic on its own (temp file + `os.replace`). **No atomicity across the two files is claimed.**

1. **`<gate>.markers.txt` PROVISIONAL.** First line `RUNNER_RECEIPT_STATE PROVISIONAL INCOMPLETE - NOT A RUNNER VERDICT …`, last line `RUNNER_RECEIPT_STATE PROVISIONAL END - no FINAL line was committed; …`. Every quoted line (raw Editor output, git-status snapshot) is prefixed `PROVISIONAL> `, so no bare `…_PASS` or `POSTRUN_LANE_STATUS clean` line exists in that state.
2. **`<gate>.json`** is written. It is the committed verdict (exit code, failures, drift). It now also carries a `receipt_authority` sentence stating the rule below.
3. **`<gate>.markers.txt` FINAL.** Rewritten with the first line `RUNNER_RECEIPT_STATE FINAL runner_exit=<n> json=<gate>.json json_sha256=<SHA-256 of that JSON file>`, followed by the original marker lines in their old, unquoted form.

### Authoritative success (all three, nothing less)

- **(a) process exit status 0;**
- **(b)** `<gate>.json` exists, `gate` matches, `exit_code` is 0 and `failures` is empty;
- **(c)** the first line of `<gate>.markers.txt` is exactly the FINAL line with `runner_exit=0` and the SHA-256 of **that** JSON file's bytes.

`authoritative_success(out_dir, gate)` in the runner implements (b) and (c); the exit status (a) is the caller's. Raw Editor `…_PASS` plus a clean status never satisfies it, because the raw lines only appear unquoted inside a file whose first line is FINAL.

### Failure and provisional-state semantics

| Situation | Process exit | JSON | Markers on disk | Authoritative success |
|---|---|---|---|---|
| Clean success | 0 | `exit_code 0`, no failures | FINAL `runner_exit=0`, hash matches | **yes** |
| Gate / drift / terminal-token failure, all receipts written | 1 | `exit_code 1`, failures listed | FINAL `runner_exit=1` (a completed, recorded failure) | no |
| Provisional markers write fails | 1 | written, lists `receipt_write_error:markers:<Type>` | absent | no |
| **JSON write fails, fallback append works** | 1 | absent, no `.partial` | PROVISIONAL + `…INCOMPLETE json_not_written` + `RUNNER_RECEIPT_ERROR json:<Type> runner_exit=1` (still the last line) | no |
| **JSON write fails AND append fails (R324-01)** | 1 | absent, no `.partial` | **PROVISIONAL, first and last line say INCOMPLETE** | no |
| JSON written, FINAL markers write fails (new third write) | 1 | present, says `exit_code 0` (committed earlier, cannot be amended) | PROVISIONAL (+ appended `INCOMPLETE markers_not_finalised`, best effort) | **no: condition (c) fails; markers decide** |
| Evidence directory unwritable from the start | 1 | absent | absent | no (unchanged R2 control) |
| Process killed between writes | non-zero or unknown | absent or present | PROVISIONAL | no (follows from (a)–(c); not simulated) |

Stdout and the process exit are always the current verdict; in the "JSON written, FINAL failed" row the stdout carries `receipt_write_error:markers_final:<Type>` and exit 1 while the JSON file still says 0. That is the cost of not claiming cross-file atomicity, and (c) is what resolves it.

## Results at the tested commit `a4019e6b`

All in a disposable sparse `--shared` clone detached at the tested commit (no Library, no `.unity`, `git status` empty), `python3 -I -B`, `PYTHONPYCACHEPREFIX`/`-X pycache_prefix` outside the lane, `CCF_TEST_SCRATCH` under `E/`. Raw output: `E/out/proof/`. Driver: `E/proof_stage_b.py` (evidence tooling, not in the repository). Two earlier runs of the same driver, which had mistakes in my own allow-list substitution (disclosure 3), are retained under `E/out/superseded-first-run/`.

### Required results

| Requirement | Result |
|---|---|
| Early marker explicitly INCOMPLETE / PROVISIONAL until a final receipt is committed | Done; asserted line by line (`assert_only_provisional`). |
| Raw Editor `…_PASS` + clean status alone never reads as a verified PASS | Done: quoted in the provisional file, unquoted only under a FINAL first line. |
| JSON write failure **and** fallback append failure, **simulated ENOSPC**: exit nonzero, old marker incomplete, no fabricated JSON or partial artifact | `test_R324_01_ENOSPC_json_and_fallback_append_both_fail` — **ok**. The FINAL write is never attempted (call log asserted). |
| Same with **real permission failure**, no mocked operation after the real markers write | `test_R324_01_real_permission_failure_json_and_fallback_append_both_fail` — **ok**. After the real provisional write the test sets the evidence dir `0555` and the markers file `0444`; the failure recorded is a genuine `receipt_write_error:json:PermissionError` (exact list asserted). Skipped for root only. |
| State authoritative success conditions without impossible atomicity | Above, in the module docstring, in each JSON (`receipt_authority`), and in `authoritative_success()`. |
| R1 post-run dirty fail/diagnostics; R3 exact terminal PASS; R4 labels; R5 teaching negatives; 13 mocks N1–N3; 28 projections; P7 old false green → current fail; historical equality; earlier controls | All preserved, see below. |
| No test weakened; package drift not waved through | No existing test body changed; the R1 drift check is untouched. |

### Test counts

| Gate | Result |
|---|---|
| Full runner suite | **41/41 ok** (35 prior + 6 new), exit 0 |
| `RunnerControls` (the 13 original #318 mocks, N1–N3 included) | **13/13 ok**; the 13 bodies are **byte-identical** to `6b7d2d45` |
| **All 46 prior test-class members** (every class, incl. helpers) vs the published #323 file | **46/46 byte-identical**; additions are only the `R324FinalReceipt` members |
| `R2TruthfulReceipts` (the four R2 controls) | 4/4 ok, unmodified (including the one that requires the file to end with `RUNNER_RECEIPT_ERROR json:OSError runner_exit=1`) |
| New `R324FinalReceipt` | **6/6 ok** (below) |
| `test_historical_projection.py` (28 synthetic projections, schema boundaries v19/v20/v21, R4 labels, R5 teaching controls) | exit 0: `HISTORICAL_PROJECTION_PASS checks=28` (28 `PROJECTION_PASS`), `HISTORICAL_SCHEMA_FIELD_BOUNDARIES_PASS`, `SYNTHETIC_LABELS_PASS rack=0 enlarged80=0 misleading; negative control #318 rack=24 enlarged80=1 found`, `TEACHING_CONTROLS_PASS checks=6`, wiring PASS |
| `compare_projection.py 6b7d2d45` | `HistoricalLayout` identical in both harnesses, 28-control C# block identical, schema audit identical, `PROJECTION_IDENTITY_PASS` |
| `py_compile` of the three Python files | exit 0 |
| Scope | 2 files; 0-byte diff on all forbidden paths; `git diff --check` exit 0 |

The projection test's `--out` guard accepts only the two earlier authors' evidence folders, and editing it is outside this packet. I therefore ran it in the **disposable clone only**, with a one-line allow-list swap to this evidence folder (recorded in `E/out/proof/08-allowlist-diff.txt`: one line, the tuple of allowed folders). The repository file is unchanged and every control ran as written.

### The six new controls (`R324FinalReceipt`)

1. `test_R324_clean_run_ends_with_FINAL_markers_bound_to_the_JSON` — positive control: FINAL line with the exact SHA-256 of the JSON bytes, raw Editor line present unquoted, no `PROVISIONAL`, `authoritative_success` true.
2. `test_R324_failed_gate_is_a_FINAL_failure_not_a_success` — a failed gate is a FINAL `runner_exit=1` and not authoritative.
3. **`test_R324_01_ENOSPC_json_and_fallback_append_both_fail`** — R324-01, simulated ENOSPC (negative).
4. **`test_R324_01_real_permission_failure_json_and_fallback_append_both_fail`** — R324-01, real permissions (negative).
5. `test_R324_final_markers_write_failure_exits_nonzero_and_stays_provisional` — the one extra control I judged necessary: my design adds a third write, so its own failure had to be covered. Exit 1, `receipt_write_error:markers_final:OSError`, markers PROVISIONAL, not authoritative.
6. `test_R324_authoritative_success_needs_matching_JSON_and_FINAL_markers` — JSON bytes changed, FINAL downgraded to PROVISIONAL, old-style file with no state line, and missing file: all not authoritative.

Controls 3 and 4 are the packet's "two precise R324-01 negatives"; 1 and 2 are the "necessary clean positives"; 5 and 6 are additions required by the design, which you may reject.

### The controls can fail

- **Against the published #323 runner** (new test file, old runner, copied outside the tree): the 6 new controls are red (5 failures + 1 error); the other 34 pass. A seventh error, `test_R3_terminal_table_matches_the_harness_sources`, appears only because that test reads sibling harness sources and the copy is outside the repository tree; it is an artefact, not a product signal.
- **The actual defect, same faults, old vs new runner** (`E/out/proof/repro/`). Surviving markers file after JSON and append both fail with ENOSPC:
  - published #323 runner: `WORKBENCH_RETIREMENT_VERIFY_PASS checks=1` / `POSTRUN_LANE_STATUS clean entries=0` — two bare lines, no failure indicator; process exit 1, no JSON.
  - tested runner: `RUNNER_RECEIPT_STATE PROVISIONAL INCOMPLETE - NOT A RUNNER VERDICT …`, the two lines quoted as `PROVISIONAL> …`, then `RUNNER_RECEIPT_STATE PROVISIONAL END - no FINAL line was committed; …`; process exit 1, no JSON.
- **Mutation matrix, 8/8 killed** (`E/out/proof/11-mutant-*`, each mutant is the tested runner with one behaviour removed, run against the full suite): M1 no provisional state (the #324 shape); M2 a FINAL-looking line written before the JSON; M3 header says PROVISIONAL but lines unquoted; M4 failed FINAL write ignored; M5 FINAL not bound to the JSON bytes; M6 `authoritative_success` ignores a recorded failure; M7 JSON failure no longer forces exit 1; M8 FINAL written even when the JSON was not committed.
- **#319 probe P7 (post-run tracked drift)**: verbatim probe vs the #318 runner `6b7d2d45`: `actual_exit 0`, `failures []` (the old false green); adapted probe vs the #323 runner and vs the tested runner: `actual_exit 1`, `postrun_lane_drift:2`.

## Limits and what changes for consumers

- **Format change.** Success markers now start with one `RUNNER_RECEIPT_STATE FINAL …` line (body otherwise as before). Failure/incomplete files and the stdout/JSON gained text. The only readers I found in the repository are this runner and its tests; any external reader of `*.markers.txt` from this runner must be updated. Other qualification runners have their own formats and are untouched.
- **No cross-file atomicity** (stated above). A kill between writes leaves a PROVISIONAL file; this is not simulated.
- `authoritative_success` is an importable function, not a CLI. Example: `python3 -I -B -c "import importlib.util as u; s=u.spec_from_file_location('r','Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py'); m=u.module_from_spec(s); s.loader.exec_module(m); print(m.authoritative_success('<evidence dir>','WorkbenchRetirementVerification'))"`.
- Real runs: the known Editor rewrite of Packages (1.1.0 to 1.1.1) and materials still makes every real run FAIL by the R1 provenance check until the Manager decides the package/material policy. Not changed or waived here.

## Process disclosures

1. **Model:** Sonnet 5.5, not the recommended Sol High.
2. I wrote a `__pycache__` directory into my author worktree once, by running `py_compile` without a cache prefix before the commit; I deleted exactly that directory, and the lane was clean at commit time and at the end of every gate.
3. **A mistake in my evidence driver (not product code):** my first substitution for the projection test's allow-list was malformed twice (first a bare string that made the guard pass vacuously, then an extra parenthesis that stopped the script parsing). The controls themselves were unaffected in the first case and did not run in the second. Both runs are kept under `E/out/superseded-first-run/`; the reported run uses the exact one-line tuple swap shown in `08-allowlist-diff.txt`.
4. All three proof runs (the two superseded ones and the reported one) were made at this same tested commit; no source file changed between them.

## NOT RUN / still open

- Unity Workbench gates **C–G** on the repaired verifier, native former-site 720p/1080p witness, Legacy40 / Reference save/load, RackV2 v2/legacy40/coldstart runtime including the teaching controls, Enlarged80 runtime: **NOT RUN**. No Unity, import, lease or Library was used.
- **B3** tree identity (P0604 lost / P1209 gained): **BLOCKED**, cause unproven; nothing re-baselined.
- Package 1.1.0 vs 1.1.1 and material provenance; the eight protected dirty files; ZX20 storage floors: unchanged and untouched.
- Independent review **#329** (two separate verdicts) and the Manager #200 G2/G3 verdict are required. #287 remains NOT QUALIFIED. No PR, merge, integration, cleanup or Windows/Player build.

## Reproduce

```bash
cd /media/jer/Files/CCF/Operations/CCF-WBReceipt-20261107            # at a4019e6b (or any checkout of this branch)
export PYTHONPYCACHEPREFIX=<outside the worktree> CCF_TEST_SCRATCH=<outside the worktree>
python3 -I -B Tools/Verification/WorkbenchRetirement/test_runner.py -v                       # 41 tests, exit 0
python3 -I -B Tools/Verification/WorkbenchRetirement/test_runner.py -v RunnerControls         # the 13 original mocks
python3 -I -B Tools/Verification/WorkbenchRetirement/test_runner.py -v R324FinalReceipt       # the 6 new controls
python3 -B /media/jer/Files/CCF/Verification/WorkbenchReceipt-328-20261107/proof_stage_b.py a4019e6b96b50e03d2b71d92c44fc30a0f7ace29   # clones, old-vs-new repro, P7, projection, mutants (~15 s)
```

## Status

**Author claim: SOURCE/MOCK candidate for R324-01, qualified by the evidence above at `a4019e6b`.** A surviving early markers file now says INCOMPLETE / PROVISIONAL until a FINAL line bound to the committed JSON is written; JSON-plus-append failure under both simulated ENOSPC and real permissions leaves only that incomplete file and exit 1. Not approved. Not Unity-qualified.
