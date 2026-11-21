# Multi #885 Stage I — MA-1 integration and technical qualification result

**Verdict: NEEDS CHANGES — technical integration qualification only.**

The exact candidate was fast-forwarded into its separate integration lane and pushed unchanged. Both builds and all three executable test suites passed. Stage I is **not fully qualified**: the required P0 isolation harness cannot lawfully run under the current no-rebuild grant, and the N1 naturally-empty carry-forward presentation branch was not reached through the shipped HTTP API. These are outstanding gates, not waived tests or proven product defects. No source fix was attempted. Stage Q execution remains blocked pending qualification closure and its separate approved executable packet.

## Identity, authority and custody

- Actor: separate local OpenAI GPT-6.1 Sol / Codex integrator, high effort; neither Claude Opus author, D-091 custodian, orchestrator, nor independent reviewer. No subagents.
- Integrated and delivered SHA: `4cecf87339bea4d32907955a8d5625d1f27f96bd`.
- Integration branch: `integration/multi-885-ma1-two-area-20261121-v1`, pushed with upstream; remote ref read back at the same SHA. No integration commit was made.
- Worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121`.
- Base: `e11dd5192d7a89abf859613ce92814bdd6338ea2`; true `git merge --ff-only` to the candidate. HEAD has one parent, not a merge commit. `ac4c7b51582283fb125f6b747a76b31282451696` is not reachable.
- Host reverified: `DESKTOP-DL1BQI7`, WSL2 Ubuntu, `jer` uid 1000, 880G available on `/` at preflight.
- SDK actually invoked: `/home/jer/.dotnet/dotnet`, **8.0.134**. The project's 8.0.133 / latestPatch pin admits it. System SDK was never invoked.
- TMPDIR: `/tmp/ccf-885-integration-20261121/tmp`; all evidence outside the repository at `/tmp/ccf-885-integration-20261121/evidence`.
- Separate docs scratch clone: `/tmp/ccf-885-integration-20261121/docs-clone`, based on `cbf3a6185754a33f4a75b96f8fe775d0041b72a1`. The result commit is exclusively on `docs/multi-885-stage1-integration-result-20261121-v1`, not the integration branch.
- Parent clone remained clean on main at `cbf3a6185754a33f4a75b96f8fe775d0041b72a1`; no worktree-file writes there.

Binding authority read: [Manager dispatch](https://github.com/jermur788/CCF/blob/docs/multi-885-largest-safe-simulation-programme-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-LARGEST-SAFE-END-TO-END-MA1-INTEGRATION-MULTIYEAR-QUALIFICATION-ORCHESTRATOR-20261121.md) and [D-091 admission, sections 5 and 7](https://github.com/jermur788/CCF/blob/docs/multi-885-d091-admission-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-D091-PHYSICAL-ADMISSION-RECEIPT-AND-INTEGRATOR-SCOPE-20261121.md), narrowed by the direct Stage I task's copy-only P0 prohibition. Repository context inspected; local context stamp `3f4e852e9579de7335e6d71ca660ecd2b0f8b561`. Live main coordination and issue #885 were fetched. The live main board has older reservations and no fresh Stage I row; the explicit D-091 receipt and direct dispatch grant this disjoint lane. No board, lease or custody ownership was edited or self-reassigned. Q0 was not repeated.

Fresh preflight passed: path and branch absent locally and on origin, base/candidate commit objects present, required ancestry true and excluded successor ancestry false, clean pinned parent, protected listeners alive, #867 absent, and ports 33811/33841 free.

## Byte identity and input pins

`git diff 4cecf87339bea4d32907955a8d5625d1f27f96bd HEAD --stat` was empty. Full candidate-to-HEAD diff and final HEAD-to-working-tree diff were empty; final tracked/untracked Git status was clean. Thus all tracked product, tests and compile-linked dependencies match the exact candidate, not merely the changed-file list. All recorded product/input SHA-256 values were rechecked after execution and passed. No source, test, fixture, golden, Assets, package, save or P0 reference content was edited.

The delta from base was exactly these **16 paths**: the following 15 product/test paths plus `Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-MA1-TWO-AREA-SIMULATOR-SOURCE-IMPLEMENTATION-HANDOFF-20261121.md`. The carried author handoff is historical author evidence; its earlier SHA statements do not replace this result's exact candidate identity.

| Product/test path | SHA-256 |
|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` | `c8428379d49a5d695296a3da0ec44c755f4f44204abb877ff5bc5c83a61e804e` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContracts.cs` | `28fa7cb65885ecb44892955d734d30f2d80644a90739aee1db9a0c998a32404f` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaExperiment.cs` | `bf03a641908f7780a8844ae0099b3ecfc51e2250bdbbaa3b92c4dade1c36ac0e` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaGenerator.cs` | `12b1a2871d2288c55eafe933b4e63770dc12e86b417f6c5259029608314aa3a5` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaGeometry.cs` | `55850db4f33bc0b57aa65bbb1ceb92b2cf2486cb311fa00ff41be8767052ba79` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaProjection.cs` | `b83fdbd7acebd81e9c810599d3423b34592fa8e60d0dc936539afdda7db826fa` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs` | `b4b43808677487bc5e964c9d2753711df5cfe79841c5489b74584147d64e757a` |
| `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs` | `4013474f5e2334b121f6131c7ab9aead9edafe45b9799ef5bf4b493977714173` |
| `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` | `840c2dd33aee0acd383aedb88e160b88cbb8e5276ebfd9b92f1a0463337faced` |
| `Tools/MultiRackSitkaP1Lab/src/Ui/Program.cs` | `3b85f67a563d832e34c81e21fc1fd22f8671f5133a7b2eef1568452e72c679f4` |
| `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js` | `0a26d41dd97016a4d44440a94e8aaf4ec913e0d15fb81b295d130ce0abbb2c7b` |
| `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/MultiAreaTests.cs` | `8434451cec5ebd44ddd51c2f895c2cb303928213887047dd8ec7233757b1d380` |
| `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs` | `4923a48d7815ab11951e414427311981f6699621f533cdd169fb5e5726748fa7` |
| `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` | `6135ac5028e3e6fbc180baa9dadd1165ffbddd0257ccef82737bd54a3c82c85e` |
| `Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh` | `d9b99b5f9add8e7cae1af8b87fb1be4899e3b7000152b05ae52d5c183c94992e` |

| Pinned read-only input | SHA-256 |
|---|---|
| `Assets/ForestPrototype/SitkaGrowthModel.cs` | `291601041876d83451259990f82219c6ccd3fb704ba55e6dcb8189f13b446463` |
| `Assets/ForestPrototype/SimulationRandom.cs` | `d1b52a604d2593c833e80f4c20bce1ead56f2ad51f19ce12625e4a6bb2ec927b` |
| `Assets/ForestPrototype/TreeSpeciesDefinition.cs` | `0206b64be5c18dee1fc7db8b8a399b92d3d5915be5e31965646ff7dfc784ba48` |
| `Assets/ForestPrototype/Species/SitkaSpruce.asset` | `a6634c76644e5fec31d728f1edbdd6cd2b23e85281d4dced288980eda6251563` |
| `Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` | `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` |
| `Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh` | `e4e00479544d98cc2738ff2d96ee30ea74cf4a9f620ed3ef93c0dd25f6f05853` |
| `Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh` | `b2b990a3a01f994ffca0f08cc3ffac15639f5293016c92dd12789497121db41f` |

The four Scenario One source/asset pins, immutable fixture, reference-builder script and isolation script above were unchanged throughout. The builder script was inspected but never executed.

### P0 reference: four-value agreement, copy only

All three required files were copied read-only from `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-MA1/Tools/MultiRackSitkaP1Lab/.p0ref/` into this lane's ignored `.p0ref/`: `SitkaLab.Core.dll`, `SitkaLab.Core.dll.sha256`, `provenance.txt`. Fallback was not needed. No other author/reviewer/#882 worktree access was performed.

Each of these independently read values was exactly `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`:

1. `P0CoreExpectedSha256` in `src/Core/SitkaP1.Core.csproj`.
2. Actual `sha256sum` of the copied DLL, also rechecked after all runs.
3. First field of `SitkaLab.Core.dll.sha256`.
4. `dllSha256=` in `provenance.txt`.

**P0 pin agreement: PASS (4/4). This is a copied reference hash, not a generated-reference/isolation result.** No reference was generated during this task; no actual generated/reference comparison can honestly be reported.

## Actual build and test gates

All commands ran from the integration lane's `Tools/MultiRackSitkaP1Lab` with the external TMPDIR above.

| Gate | Actual output/result |
|---|---|
| `/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln` | `Build succeeded.`; `0 Warning(s)`; `0 Error(s)`; elapsed 00:00:09.74 |
| `/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **`135/135 passed, 0 failed`** |
| `/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj` | `Build succeeded.`; `0 Warning(s)`; `0 Error(s)`; elapsed 00:00:02.49 |
| `tests/ui-smoke.sh /tmp/ccf-885-integration-20261121/evidence` | **`OVERALL: PASS (287 passed, 0 failed)`** |
| `tests/ui-smoke-multiarea.sh /tmp/ccf-885-integration-20261121/evidence` | **`OVERALL: PASS (127 passed, 0 failed)`** |
| `tests/p0-isolation.sh` | **BLOCKED / NOT RUN; 0 checks executed.** No generated hash or overall isolation verdict produced. |

The first single-area smoke invocation refused before any check or listener: `STOP: node is required for the page witness (set NODE)`. No runtime was installed. A bounded read-only search located existing Node, then both unchanged smokes ran using `NODE=/home/jer/.local/share/vite-plus/js_runtime/node/24.22.2/bin/node` (v24.22.2) and `DOTNET=/home/jer/.dotnet/dotnet`. This tool version differs from the author's recorded Node; actual integrator version is disclosed here. No test, fixture, threshold or expected count was modified. The prerequisite-stop output is retained separately.

The inspected isolation harness deletes `.p0ref` and P1 Core/CLI/test outputs before its cold run, then invokes `build/build-p0-core-ref.sh`; it also invokes bare `dotnet`. Executing it unchanged would violate this task's explicit no-reference-rebuild rule. It was stopped at inspection, not patched, wrapped to bypass its work, or credited as passing. Resolve this contract conflict with a bounded Manager grant before closing Stage I; pin agreement and smoke no-write checks do not substitute for L0–L4 isolation.

## N1 actual HTTP and presentation witness

Served the built exact-candidate UI from this worktree against the unchanged `fixtures/derived`, using the pinned dotnet executable. `Program.cs` accepts only the fixture directory and hard-codes `Listen(IPAddress.Loopback, 0)` with an empty configuration builder; it exposes no fixed-port option and ignores environment URL configuration. Consequently **33811 could not be used**. The task's explicit fallback was used: actual listener **127.0.0.1:45113**, PID **1075349**, recorded from its printed capability URL and `ss`. No second listener, proxy or source edit was introduced.

Curl used the printed capability path and explicit `Host: 127.0.0.1:45113` and `Origin: http://127.0.0.1:45113` (no localhost alias). Captured raw HTTP headers and bodies for `multi`, `multi.js`, `api/multi/view` before/after, and `api/multi/year/advance`; the request used the real returned fence and `consentToCancelWork: false`. Exactly one no-work ecological advance was made in this dedicated witness, not a Stage Q experiment.

Actual history was year **0 → 1**: MA-A **171 → 166** living trees, MA-B **73 → 73**, and both area's `noLivingStems` values false. Executed the unchanged `renderHistory` and its helper functions from the **served** JavaScript in a minimal Node DOM stand-in against the **unaltered actual API response**. Exact observed latest-year string:

> Latest year 0 → 1. Each area ran the unchanged annual rules on its own trees.

This is an executable presentation witness, not grep; the raw API responses and generated `n1-rendered-history.html` are retained. It is **not** a browser/layout/native-input review.

**Naturally-empty branch: NOT REACHED through the shipped HTTP API in this Stage I witness; its exact rendered string is NOT runtime-verified.** Both shipped areas begin populated. The HTTP process constructs only `MultiAreaWorkPlanAdapter.OpenSynthetic()` and exposes no custom-experiment/import/seed endpoint; work execution refuses to fell a populated area to zero. No source/fixture/payload was edited to manufacture emptiness, and no long-horizon search was run outside Stage Q. This does not prove the branch can never occur after natural mortality at some later horizon.

The passed Core suite includes `MA-E05`, which reaches genuine natural extinction and subsequent empty carry-forward using a **test-only custom experiment** and real annual rules; it checks a null carried-empty annual report, not the UI history wording. That engine test is not a shipped-API N1 presentation witness. The unexercised served conditional contains the text:

> Each populated area ran the unchanged annual rules on its own trees; an area with no living stems was carried forward unchanged.

This last quotation is **inspected source text only, not an observed empty-branch runtime result**. Therefore the N1 two-branch requirement remains incomplete, without claiming a product defect or inventing a test-only UI pass.

## Processes, ports and cleanup

| Own listener | PID | Actual bound endpoint | End state |
|---|---|---|---|
| Single-area smoke | 1060852 | 127.0.0.1:38653 | Harness SIGINT, exit 0; absent afterward |
| Multi-area API smoke | 1074088 | 127.0.0.1:36165 | Harness SIGINT, exit 0; absent afterward |
| Multi-area page smoke | 1075184 | 127.0.0.1:36129 | Harness SIGINT, exit 0; absent afterward |
| N1 dedicated listener | 1075349 | 127.0.0.1:45113 | Integrator SIGTERM to this PID only, waited for exit 0; absent afterward |

These are **all actual listener ports bound** by this task. The smoke scripts set hostile URL configuration for 5187 as a refusal/isolation check; **5187 was not bound**. Granted 33811 and separate Stage Q 33841 stayed unbound. No 33767/43649 bind was attempted. All four listeners ended cleanly with the session-only shutdown message; server stderr checks passed. Build/test, Node, curl, Git, Python and shell helper processes were transient and completed; a final process cwd/command audit found no remaining integration-lane process. No broad kill command was used.

Protected processes were reverified before/after: **1750123 (#877), 127.0.0.1:33767**, original start Fri Nov 20 18:08:17 2026; **1806361 (#882), 127.0.0.1:43649**, original start Fri Nov 20 21:00:23 2026. Both remain alive and listening with unchanged command/start identity, untouched and never signalled. **390578 (#867) remained absent and untouched**, never restarted. No Unity, Windows build or save-file operation occurred.

One scratch process-audit attempt flagged its own concurrently running `tee`, then stopped before pushing. The audit was narrowed only to exclude that exact evidence-output helper, rerun successfully, and no process was killed or product gate altered. The first read-only discovery script also stopped because WSL lacked `rg`; ordinary `grep` was used thereafter. The fresh scratch clone initially refused its docs commit because no Git author identity was configured; set the actual integrator name and existing repository email locally in that clone, without changing global or parent configuration, then committed successfully.

## Evidence, limitations and remaining gates

External raw evidence: `/tmp/ccf-885-integration-20261121/evidence` contains preflight, path/hash manifests, both build logs, Core output, initial prerequisite refusal, full and harness smoke logs, P0 pin agreement, raw N1 HTTP responses/headers and request, served page/script, executed DOM output, listener/process and final hash audits. Scratch scripts are under `/home/jer/orch-scratch/885-*.sh` and `885-render-history.cjs`. Evidence is local temporary storage, not a permanent shared archive; this tracked prose preserves the decision and identities. Capability URLs are kept only in local raw evidence, not published here.

No PR, main merge, force push, tag, history rewrite, code/test/fixture/golden change, Unity operation or cross-lane writer takeover was performed. The integration ref is delivered unchanged despite the incomplete qualification verdict. Only this one new result Markdown is committed to the separate docs branch. The lane and generated outputs are preserved for Manager follow-up; this report does not release another owner's custody or grant Stage Q execution.

Required before Stage I can become APPROVED: reconcile the P0 isolation no-rebuild conflict and execute the lawful gate; establish an honest executable N1 carried-empty presentation witness or obtain a documented Manager disposition. Independent review of this integrator result and owner **G2** remain separate. Live board/dashboard/Drive synchronization was not performed or verified under this one-document grant and remains an operational handoff to the orchestrator/custodian.

**Most important Stage Q risk:** extinct-area carry-forward can be misreported as an annual simulation result if null annual reports, extinction-year reports and later empty histories are not distinguished. The UI's empty wording lacks this Stage I shipped-API witness. Stage Q must not convert null to zero or claim the empty area ran a year, and must run only after a separate exact-SHA/harness/physical execution grant.

**Scope disclaimer:** This is technical qualification of a **SYNTHETIC** model. It is **NOT ecological validation**, **NOT integrated to main**, and **NOT owner-accepted (G2 pending)**. The model does not simulate **regeneration, windthrow, browsing, canopy light, timber grades, economics or cross-area ecological interactions**. No empirical comparability, MA-2 treatment transfer, scientific calibration or wider game readiness is claimed.
