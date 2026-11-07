# #324 independent Workbench runner R1–R5 review

**Reviewer opinion: NEEDS CHANGES — SOURCE ONLY.** R1's major admission/provenance defect is repaired; R3–R5 pass this bounded source/mock review. R2 fixes the original stale JSON bug, but still leaves a misleading surviving markers receipt when JSON creation and its fallback marker append both fail. This is **MINOR R324-01**, within the existing R2 contract, independently reproduced with simulated ENOSPC and actual filesystem permissions. The process correctly returns nonzero; this is a durable-evidence defect, not a new process-exit false green.

This is the sole independent #324 review after the earlier instance exhausted provider quota before doing review work. No earlier review state was reused. This opinion grants neither Manager #200 approval nor integration. **#287 remains NOT QUALIFIED.** No Unity qualification is claimed.

## Identity and authority

| Item | Exact identity |
|---|---|
| Reviewed branch | `fix/workbench-runner-integrity-20261107-v1` |
| Base | `6b7d2d45a21824f4325bc3c176f7e276dc7299e6` |
| Tested/reviewed source | `e94267695b4cfef05ba012010ddbf5db625be0ec` |
| Published source head | `2c4aea1c94a0768c684376bc027ba15c5af5146f` |
| Publication relationship | Head's only parent is the tested source; head adds only the author report. All five tested source files match their exact Git blobs. HTTPS origin and GitHub commit API independently read back this head. |
| Source context commit | `63044dfbbeb869996ceecec8a47619dcba048b0b` (latest canonical context-set change in the source history) |
| Author | Anthropic `claude-opus-5-5`, Claude B–Pro, from the #323 handoff |
| Reviewer self-report | **T3 Code, Codex harness, gpt-6.1-sol, reasoningEffort high, OpenAI provider family (ChatGPT–OpenAI B instance); account/quota UNKNOWN.** Sole reviewer; no subagents. I did not author the reviewed code. |
| Reviewer branch | `docs/workbench-runner-r1-r5-independent-20261107-v1` |
| Reviewer lane | `/media/jer/Files/CCF/Operations/CCF-WBRunnerReview-20261107` |
| Reviewer branch base | Exact origin/main `93f7abc0f290cab61509222c237fab513c6337ea` at creation; its context-set commit is `4790c20918ca2a667b04d80262142d9b4151b60e` |
| Evidence root, abbreviated `E` | `/media/jer/Files/CCF/Verification/WorkbenchRunnerR1R5Review-20261107/` |
| Execution root, abbreviated `C` | `E/clone`, shared-object sparse disposable clone, detached at the tested source; no Library and no `.unity` files |
| Date | 2026-11-07 |

Binding authority: the [independent review packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-WORKBENCH-RUNNER-R1-R5-INDEPENDENT-REVIEW-PACKET-20261107.md), [correction packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-WORKBENCH-RUNNER-R1-R5-CORRECTION-PACKET-20261107.md), [#319 findings](https://github.com/jermur788/CCF/blob/docs/workbench-287-verifier-closure-independent-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-287-VERIFIER-CLOSURE-INDEPENDENT-REVIEW-RESULT.md), and [Manager closeout ruling](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-322-CLOSURE-DISPOSITION-AND-NEXT-GATES-20261107.md). Complete copies and hashes are in `E/inputs/` and `E/gates/13-audit.stdout`. The assignment is PACKET ONLY, bounded G2/G3 source review; G4 integration is not exercised.

Preflight: branch absent locally and on HTTPS origin, lane and evidence path absent, no existing worktree for this branch, store/origin reachable, #323 writer explicitly released in its handoff, no competing #324 review publication/comment found. Files is `/dev/sdb4`, ext4, initially about 36 GiB available. New docs worktree was clean at the exact main above. Primary's unrelated `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` modifications remained present and untouched. The live coordination record was read; it was not edited. [RUNNING comment](https://github.com/jermur788/CCF/issues/324#issuecomment-6707956138) followed preflight and lane creation.

All execution and scratch writes used the new evidence root. The author/#318 lanes and all older evidence were read only. No ZX20 path was written; no Editor, import, lease, new Library, package restore/update, live save, Reference, production or CCF-Ops modification occurred. I did not rerun the protected eight-file live ZX20 hash audit and do not claim a new such measurement.

## Diff scope and preserved gates — PASS

The base-to-publication diff has exactly these six paths:

1. `Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py`
2. `Tools/Verification/WorkbenchRetirement/test_runner.py`
3. `Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs`
4. `Tools/Verification/RackV2Persistence/test_historical_projection.py`
5. `Tools/Verification/StandGeometry/Enlarged80Verification.cs`
6. `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RUNNER-R1-R5-CORRECTION-RESULT.md`

The tested source diff is five files, **628 insertions / 54 deletions**. Enlarged80 changes one label and its comment. `git diff --check` exits 0. Assets, Packages, ProjectSettings, `.vscode`, CCF-Ops and live coordination have an empty diff; no material, scene, save fixture, Reference or baseline change exists. `E/audit/full.diff` and `commands.json` retain the full audit and individual command exits.

The **13 original test method bodies are byte-identical**. Both `HistoricalLayout` methods, the **28-control C# block**, and its historical-schema audit are byte-identical to #318. `WorkbenchRetirementVerification.cs` is entirely unchanged by #323, preserving the B2 lifecycle repair, exact tree/hash equality and C–G assertions. The frozen Reference Year-100 hash `7AD177B3CC2F73C7` and Enlarged80 final equality are retained. No prior gate was silently removed.

## R1 — PASS for fail-closed drift enforcement, source/mock only

I ran the verbatim #319 runner probes against the old runner extracted from the exact base. **P7 returns 0, failures `[]`**, reproducing the old false green. With only the mock plumbing adapted for the new status/blob calls, **P7 returns 1, `postrun_lane_drift:2`**. Probe scripts, their adaptation and complete outputs are in `E/scripts/probe_{old,new}.py` and gates 07–08.

The repaired runner checks porcelain v1 `-uall -z` after removing its staged harness and `.meta`; any remaining tracked/untracked entry fails. Paths, status, worktree kind, SHA-256 and HEAD blob appear in JSON and extracted markers. A status-query exception also fails. It performs no reset, restore or automatic cleanup of drifted user files.

Independent real-Git fixtures in gate 16 test **one clean success (0)** and **seven drift failures (1 each)**: manifest, lockfile, material, staged rename with newline/Unicode, untracked newline/Unicode path, tracked symlink substitution, and executable-bit-only change. The mock Editor mutates only after clean admission; actual Git supplies identity/status/blob queries, and only the lease and Editor are mocked. Every drift's worktree hash is independently recomputed, both receipts are inspected, harness/meta/licence cleanup is asserted, and drift remains on disk. These minimal synthetic repositories have their own commits; they are not real Unity candidate worlds. The executed runner bytes come from the exact reviewed candidate.

The 35-test suite additionally covers status failure, deletion and multi-path drift. A runner failure is established by nonzero exit and JSON failure. Extracted markers retain the Editor's raw terminal PASS as evidence alongside `POSTRUN_LANE_STATUS drift` and hashes; **a raw gate PASS is not a successful runner verdict**. This review does not claim literal erasure of all PASS strings from diagnostic evidence. The marker-only finalization weakness is covered under R2 below.

## R2 — NEEDS CHANGES, MINOR R324-01

**The original #319 defect is fixed.** Marker-write failure now yields JSON with `exit_code:1` and the marker error. Ordinary JSON-write failure leaves no JSON/partial and appends `RUNNER_RECEIPT_ERROR ... runner_exit=1` to markers. Wholly unwritable evidence creates no invented receipts. The author tests and my independent cases confirm those results, exit 1, cleanup preserved.

**Residual finding:** the markers receipt is published before the JSON attempt. If JSON creation fails and the fallback append also fails, the surviving marker file does not disclose the runner failure. See [reviewed source lines 253–270](https://github.com/jermur788/CCF/blob/e94267695b4cfef05ba012010ddbf5db625be0ec/Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py#L253). The fallback exception is swallowed.

I independently reproduced this twice:

- Simulated ENOSPC starts after markers are written; both the JSON write and fallback append receive the same ENOSPC condition.
- **Actual permission failure**, without mocking either failed filesystem operation: after the real marker write, set the synthetic evidence directory to `0555` and its marker file to `0444`. The real JSON temporary-file write and real marker append both raise PermissionError. After execution, only those synthetic fixture permissions are restored for evidence retention; no receipt is changed.

Both yield process return **1**, stdout `exit_code:1`, no JSON, no partial, cleanup complete. The surviving marker receipt is exactly:

```text
WORKBENCH_RETIREMENT_VERIFY_PASS checks=1
POSTRUN_LANE_STATUS clean entries=0
```

Actual-permissions stdout contains `failures: ["receipt_write_error:json:PermissionError"]`. The durable marker has no failure, no `runner_exit=1` and no pending/incomplete indicator. Evidence: `E/fresh-v2/{full_filesystem_after_markers,real_readonly_after_markers}/`, especially `stdout.json` and the marker receipt. `E/gates/16-fresh-refined.stdout` reports both. Its shared finding-description string mentions ENOSPC for both cases; the real-permissions case's exception and stdout are **PermissionError**, as shown in its actual receipt.

This violates the binding R2 requirement that on-disk receipts, if written, agree with the final exit. It is narrower than the old R1 false green: the process fails correctly and there is no false-success JSON. A consumer requiring both JSON and successful process exit would reject it, but that consumer policy is not stated in the surviving marker artifact.

**Required bounded correction:** make the initial durable marker state explicitly incomplete/provisional until receipt finalization, and make a successful runner verdict distinguishable from raw Editor PASS markers. Preserve truthful failure when final writes cannot occur; do not rely on being able to rewrite a full/read-only filesystem. Add the two negatives above. I made no source correction and grant no waiver.

## R3 — PASS, source/mock only

The three known gates are bound to their exact terminal tokens. Unknown gates and caller-supplied spoof/cross-gate/intermediate tokens are refused by argparse, **exit 2 before launch/evidence creation**. Logs require exactly one start-of-line terminal token with the allowed boundary. Missing, intermediate-only, duplicate, look-alike, prefixed/indented and gate FAIL/FAILED cases fail. Each exact terminal token passes its clean control.

The full author suite covers all three gate names and token-table/source agreement. My fresh truth table adds **27 verdict controls**: per gate, one valid positive and eight negatives, including punctuation and nonbreaking-space look-alikes. All expected outcomes hold. P8 changes from old **0** to new **argparse 2**, a correct early refusal.

## R4 — PASS, source only

The Rack v20/v21 and Enlarged80 v19 projection labels now identify synthetic projections. No fixture becomes an archived historical capture. The existing check finds **0** misleading candidate labels, and its #318 negative control finds **24 Rack + 1 Enlarged80**. Independent mutants reverting either label or adding a new misleading v21 claim are all caught (**3/3**). Frozen genuine Reference checks and real historical field boundaries remain intact.

The output-path allow-list in `test_historical_projection.py` does not include this new review root. I ran a scratch adapter changing only that guard and setting ROOT to the unchanged candidate clone. No candidate file was edited; original and adapter remain available. This is a relocation for review evidence, not a behavioral-test relaxation.

## R5 — PASS for strengthened/narrowed source claims; Unity NOT RUN

The absent-flag check now loads a disk v24 true positive control, verifies the session is true, writes a genuinely absent-key file, then exercises Continue/CanLoad/Load and requires false over that prior true state. It accurately says missing means false by schema policy; it does not claim production validates key presence. Cold start explicitly checks the seed boolean, loads its opposite from disk, then reloads the original seed. Both changes must discriminate from an unchanged session; the signature/world equality is rechecked.

Production still has `CurrentVersion = 24`, and restoration is `saveVersion >= 24 && data.firstGuidedRackReviewComplete`. The v24 true/false round trip and stray-v23-true control remain. The schema boundaries and synthetic projection policy are unchanged.

The actual extracted helper executes offline with **six controls (two positive, four negative)**. My three source mutants (memory-only absent load, missing true precondition, missing opposite-load discrimination) are all rejected. Two separately compiled helper mutants—constant false and the old tautological equality shape—compile with exit 0 and fail execution with **-6/SIGABRT**, with core dumps disabled. Thus source assertions and the helper can fail; this is not proof that Unity's disk load paths have executed. Gates 19–20 preserve the first mutation-driver error and its corrected run.

## Preserved tests, old adversarial notes and reviewer instrumentation

- **Runner suite: 35/35**, exit 0; separately **13/13 prior**, exit 0, including N1–N3. N1 no terminal marker fails, N2 FAIL+PASS fails, N3 nested licence directory succeeds with two hash prefixes; N3 hashing exception still cleans up and fails. No tests skipped in this non-root session.
- **Projection suite: 28/28**, compiler/run exits 0; historical schema boundaries pass; teaching helper **6/6**.
- **#319 runner probes: 35 each**, script exit 0. Old has 8 expectation mismatches; new has 7. New's five known N1 diagnostic-spelling/exception blind spots remain; P8 is now correctly refused with 2 rather than the old probe's expected 1; the licence symlink loop fails closed, as #319 already accepted. These are not seven new repair failures. P7 and the original P4 JSON discrepancy are fixed. N2 stderr/stdout exclusion and N3 caller environment inheritance remain documented outside R1–R5. Keep `CCF_TEST_DIRECT_ENTRY=1` explicit in any future admitted runtime protocol.
- **#319 extra projection controls: 14**, compile 0/run 2; **13 expected, 1 unchanged Note N4**. A pre-v21 empty-plan model-1 projection deliberately drops the model. It was already classified as the retained synthetic policy, not a regression. It still needs clearer policy documentation/positive control in a later allowed task.
- **Fresh controls: 13 real-Git/filesystem scenarios + 27 token cases**. One clean run, seven drift negatives, three ordinary receipt-failure scenarios and two residual R2 reproductions. Gate 12 was the initial 12-scenario run. Its rename fixture used `git add -A`, unintentionally staging the disposable harness/meta and reporting three drift entries. Gate 16 narrows the add to the two rename paths, gives one rename entry, and adds the actual-permissions reproduction. Both runs are retained.
- **Fresh R4/R5 mutants: 8/8 caught** (six source checks, two executable helper negatives). Gate 19 initially exited 1 because my driver caught AssertionError but not the check's expected missing-substring ValueError; corrected gate 20 exits 0. This was reviewer instrumentation, not a candidate failure.
- **Offline Roslyn type check:** 189 production non-Editor files + 3 harnesses, **192 sources / 320 SHA-verified Files reference DLLs / 0 errors / 45 warnings**, exit 0. Deliberate undefined-name negative: **1 error CS0103**, exit 1. References were read from retained Files evidence, never from ZX20. This is not Unity compilation or runtime qualification.
- Python syntax check exits 0 with explicit `-X pycache_prefix=E/pycache`; final candidate clone status including ignored entries is empty, no Library or scenes.

## Assessment of the author's three flagged items

1. **“Every real Editor run will FAIL while packages/materials keep rewriting”: truthful as a conditional source-behavior claim.** Independent manifest, lockfile and material mutations cause R1 failure. The runner must reject changed candidate bytes. I did not establish that every future Editor invocation will rewrite these files; no real Editor ran. Canonical tracked 1.1.0 versus Editor-resolved 1.1.1, the transitive sysroot, five material rewrites, and the protected eight dirty files remain Manager provenance/policy items. No package/material equivalence, update, restoration or cleanup is authorised by this review.
2. **README wording: confirmed.** At the reviewed SHA, `RackV2Persistence/README.md` still describes “v20 Enlarged80 save” and “v21 and v20 Legacy40 saves” without synthetic qualification. The file is unchanged and outside the allowed repair list. Carry forward a bounded docs correction; it does not undo the repaired runtime label checks.
3. **Author pycache incident: recorded and consistent with current evidence, not retrospectively witnessed.** The retained incident lists exactly three pyc files in the author's disposable clone. Its final clean receipt and my read-only current status of that clone are empty. `-I` ignores PYTHONPYCACHEPREFIX; explicit `-X pycache_prefix` is the correct replacement. No evidence here places those files in an author/registered Unity lane. I cannot independently prove all historical creation/removal events; the disclosure is appropriately limited to a throwaway clone and is not grounds for a new source defect.

## Remaining gates and authority limits

**B3 remains BLOCKED:** lost P0604/gained P1209 tree substitution is unresolved; live-player-pose sensitivity is a plausible mechanism, not an established cause. No tree/hash baseline is changed or waived.

**NOT RUN:** repaired Unity C–G; native former-site witnesses at 1280×720 and 1920×1080; RackV2 v2/legacy40/coldstart runtime including the new teaching disk controls; Enlarged80 F/runtime; Legacy40/Reference save/load and rendered material equivalence; full integrated A–O; Player/native Windows profiling or build. No lease was acquired. No source/mock PASS changes these classifications or qualifies #287.

**Decision:** NEEDS CHANGES, G2/G3 source evidence only, for MINOR R324-01. Required next step is the bounded receipt-finalization correction and its source negative controls, then the Manager's separate source disposition. Integration target/head: **NONE / NOT INTEGRATED**. No production change is requested by this opinion.

**DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** `command -v ccf-ops` returned 127. This docs-only reviewer did not edit the forbidden live coordination file or generated dashboard state. GitHub #324 comments and this result are the authorised publication; Manager/orchestrator must reconcile coordination/dashboard ingestion and verify published readback. This remains an operational follow-up, not completed synchronization.

## Command and evidence ledger

`E/scripts/gate.py <id> <cwd> <command...>` records each command's argv/cwd/exit in `E/gates/<id>.json`, stdout/stderr beside it. It sets `CCF_TEST_SCRATCH=E/tmp`, `TMPDIR=E/tmp`, `GIT_OPTIONAL_LOCKS=0` and `PYTHONDONTWRITEBYTECODE=1`. Child negative exits are recorded without disguising them as success. Nested compiler/run command arrays and exits are in the corresponding projection/Roslyn/mutation folders; individual audit commands are in `E/audit/commands.json` and `E/final-checks.json`.

Initial read-only preflight commands (`git status`, `worktree list`, local show-ref, `ls-remote`, `findmnt`, `df`, GitHub comments/auth) established the facts above. Absent local branch produced the expected show-ref failure. Two reconnaissance commands were replaced: bare-store `git status` correctly rejects worktree-only use, and `gh issue view --comments` fails on deprecated classic-project GraphQL, so authenticated `gh api .../issues/{323,324}/comments` supplied the handoffs. A head-limited curl read ended with curl's broken-pipe diagnostic; complete binding inputs were subsequently retrieved and stored. None was counted as a passing verification gate.

The worktree creation command was exactly `git -C /media/jer/Files/CCF/Repository.git worktree add -b docs/workbench-runner-r1-r5-independent-20261107-v1 /media/jer/Files/CCF/Operations/CCF-WBRunnerReview-20261107 origin/main`; completed successfully, with empty post-creation porcelain status. No force operation was used.

Completed verification manifest: `E/VERIFICATION-SHA256SUMS.txt`, **196 entries**, SHA-256 **`7044ace46d2dd6ce20b7840bad8520778c3e3244a8608e8cb8642ea59f749072`**. It covers scripts, completed gate receipts and selected raw evidence; excludes disposable Git repos, DLLs, caches and later publication receipts. The final-check script's earlier intermediate manifest was regenerated after that gate closed, so its self-stream was not used as the final checksum. Only this result document is committed to the reviewer branch.

Exact executed gate commands (`E` and `C` expand to the absolute roots above):

| Gate | Command | Exit |
|---|---|---|
| 00-clone | `git clone --shared --no-checkout /media/jer/Files/CCF/Repository.git clone` (cwd `E`) | 0 |
| 01-sparse | `git sparse-checkout set --no-cone /Tools/Verification/ '/Assets/**/*.cs'` (cwd `C`) | 0 |
| 02-checkout | `git checkout --detach e94267695b4cfef05ba012010ddbf5db625be0ec` (cwd `C`) | 0 |
| 03-running-comment | `gh issue comment 324 -R jermur788/CCF --body-file E/inputs/running.md` (cwd `/media/jer/Files/CCF/Operations/CCF-WBRunnerReview-20261107`) | 0 |
| 04-runner35 | `python3 -I -B test_runner.py -v` (cwd `C/Tools/Verification/WorkbenchRetirement`) | 0 |
| 05-prior13 | `python3 -I -B test_runner.py -v RunnerControls` (cwd `C/Tools/Verification/WorkbenchRetirement`) | 0 |
| 06-prepare | `python3 -I -B scripts/prepare.py` (cwd `E`) | 0 |
| 07-probe-old | `python3 -I -B scripts/probe_old.py old/run_workbench_retirement.py tmp/probe-old` (cwd `E`) | 0 |
| 08-probe-new | `python3 -I -B scripts/probe_new.py clone/Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py tmp/probe-new` (cwd `E`) | 0 |
| 09-projection | `python3 -I -B projection-source/Tools/Verification/RackV2Persistence/test_historical_projection.py --out E/projection` (cwd `E`) | 0 |
| 10-prior-identity | `python3 -I -B ../scripts/compare_prior.py 6b7d2d45a21824f4325bc3c176f7e276dc7299e6` (cwd `C`) | 0 |
| 11-projection-identity | `python3 -I -B ../scripts/compare_projection.py 6b7d2d45a21824f4325bc3c176f7e276dc7299e6` (cwd `C`) | 0 |
| 12-fresh | `python3 -I -B scripts/fresh.py` (cwd `E`) | 0 |
| 13-audit | `python3 -I -B scripts/audit.py` (cwd `E`) | 0 |
| 14-roslyn | `python3 -I -B scripts/roslyn_check.py clone roslyn` (cwd `E`) | 0 |
| 15-roslyn-negative | `python3 -I -B scripts/roslyn_check.py clone roslyn-negative inputs/negative.cs` (cwd `E`) | 1 |
| 16-fresh-refined | `python3 -I -B scripts/fresh_v2.py` (cwd `E`) | 0 |
| 17-prior-projection-probes | `python3 -I -B scripts/prior_projection_probes.py C E/prior-projection` (cwd `E`) | 2 |
| 18-pycompile | `python3 -I -X pycache_prefix=E/pycache -m py_compile Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py Tools/Verification/WorkbenchRetirement/test_runner.py Tools/Verification/RackV2Persistence/test_historical_projection.py` (cwd `C`) | 0 |
| 19-r4-r5-falsification | `python3 -I -B scripts/falsify_csharp.py` (cwd `E`) | 1 |
| 20-r4-r5-falsification-rerun | `python3 -I -B scripts/falsify_csharp_v2.py` (cwd `E`) | 0 |
| 21-final-checks | `python3 -I -B scripts/final_checks.py` (cwd `E`) | 0 |
