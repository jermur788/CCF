# #317 independent review of #287 Workbench Tier-2 qualification

**Verdict: NEEDS CHANGES — review opinion for Manager #200; NOT acceptance, integration approval, or Unity qualification.**

T3 delegate reviewer, provider ChatGPT - OpenAI B (codex_c75d8114), model gpt-6.1-sol, reasoningEffort HIGH (set by Mimo orchestrator).

Cross-provider independent review: #287 author was Claude Sonnet 5.5 (`claudeAgent`). Reviewer worked serially, without helpers, Unity execution, a Unity lease, Library creation, production edits, or modifications to the evidence/Unity lanes. Review date: 2026-11-07 UTC. **Finding tally: 4 blocking, 2 major, 1 minor, 2 notes. No fabricated-count blocker found.** Logged gate groups and individual assertions are distinguished below; they must not be added into a fictitious uniform assertion total.

The submission is reviewable as failed qualification evidence. It is insufficient for acceptance. A verifier-only repair remains a plausible route, but **a version repin and immediate short rerun alone do not suffice**: exact-package provenance, the final-reset tree substitution, complete persistence coverage, runner integrity, and missing workbench-specific native evidence must be addressed. No production regression is established by this review; neither is the unresolved tree substitution proven to be exclusively a verifier defect.

## Scope, authority, and receipts

Read the exact independent-review packet, original Tier-2 packet, #312/#287 admission amendment, published worker result, AgentWorkflow, Decision Log, Current Milestone, Game Brief, Unity Project Overview, and live `origin/main` coordination. The review follows all seven packet points and dispatch items A–G. Research/ecological calibration was outside scope; no ecological claim or rebaseline is proposed.

- Review packet: `/media/jer/Files/CCF/Operations/CCF-MimoOvernight-20261107/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MANAGER-MIMO-WORKBENCH-INDEPENDENT-REVIEW-PACKET-20261107.md`.
- Binding execution packet: same operations tree, `Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-PACKET.md`.
- Admission amendment: same operations tree, `Docs/Handoffs/UnityLock-20261107/MANAGER-312-OPERATIONS-ACCEPTANCE-AND-287-UNITY-ADMISSION-20261107.md`.
- Worker result: [immutable submitted document](https://github.com/jermur788/CCF/blob/4d95e45245d660e24b91e26754d9c57949bf3384/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-RESULT.md).
- Review worktree: `/media/jer/Files/CCF/Operations/CCF-MimoWb287Review-20261107`; branch `docs/mimo-workbench-287-independent-review-20261107-v1`; base/fetched `origin/main` `b5c89dfc133913786590ffa9ecfee3c134278906`. Most recent context-source commit identified across the core sources/instructions/generator paths: `4790c20918ca2a667b04d80262142d9b4151b60e`. Live coordination is outside that snapshot; latest Mimo serial override controls the older status rows.
- Preflight confirmed branch and path absent before the specified canonical-store `worktree add`. Origin fetched; new worktree clean before report. `gh` authenticated as `jermur788`; [RUNNING] posted on #317 at actual start. Only this report is authored/committed here. Primary's pre-existing `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` modifications were left untouched.

**Citation convention:** `E/` means `/media/jer/Files/CCF/Verification/Workbench287Tier2-20261107/`. Source line numbers refer to `5085e8afd5abb90533e834dd6bbe7444b23973a4` unless another SHA is explicit. Evidence paths remain read-only and external; this report preserves reproducible summaries and locations, not copies of their contents.

## Seven mandatory review points

| Packet point | Disposition / severity | Independently established result and citations |
|---|---|---|
| 1. Submitted commit, ancestry, four verifier files, later differences, exact provenance | **PASS source scope; FAIL exact runtime provenance — blocking (B1)** | Exact four-file +328/−2 diff; zero production/scene/asset/package commits in `8b1946dc..5085e8af`. Result-only child `4d95e452`. Runtime package bytes differ: `E/editor-rewrites-after-cold-import.diff`, `E/compile.log:266–270`, B1 below. |
| 2. Independent Site inventory, six/new vs seven/historical, save/reference identity | **PASS source and observed early runtime checks — note** | Literal build-ID oracle, independent of retirement flag. `WorkbenchRetirementVerification.cs:124–156,167–176`; `MfmCoverageWorldUnityGate.cs:231–267`; `E/workbench-gate/WorkbenchRetirementVerification.log:1241,1370,1457,1512,1643,1757`. Later reference/reload sequences remain NOT RUN on pinned harness. |
| 3. Complete lifecycle, old worlds, Year 0/3, presentation/native checks | **FAIL / NOT RUN — blocking (B2–B4), major (M2)** | Lifecycle stops after 35 PASS; Enlarged80 stops after six logged groups; persistence entry fails. Diagnostic reset swaps one tree. Dedicated native workbench-site/prompt witness absent. See raw tally and findings. |
| 4. Lease, storage identity, thresholds, compile/import | **PASS operational trail — note; exact-candidate compile qualification FAIL under B1** | Genuine held lease and normal release, history line 338; current status free/no Editor. Observed free 28.20→21.62 GiB, never recorded below 20 GiB. Real successful second compile, zero `error CS`; first licence failure retained. |
| 5. Error handling, negative controls, false-green risk, independence, frozen writes | **FAIL runner integrity — major (M1); PASS reviewer independence/frozen-byte checks** | Runner loses exit receipts and accepts exit 0 without PASS or despite FAIL text. Three bounded in-memory negatives below. C# lifecycle exceptions are caught and reported FAIL, not green. Frozen bytes match evidence and Git. |
| 6. Itemized status/severity and historical/process exceptions | **PASS reporting — note** | Source, runtime, storage and diagnostic scopes separated in this report; historical labels and publication exception explicitly retained as N1/N2. |
| 7. Independent verdict and precise Manager recommendation | **PASS handoff — note** | NEEDS CHANGES, not acceptance. Manager #200 must dispose findings, repin scope/capacity, and evaluate repaired exact-candidate evidence. No integration/release action performed. |

## Source identity and four-file audit (dispatch A)

Observed exact ancestry, all single-parent edges:

```text
50042bbe97734b031210ddebb603b5ec00ff459d  original base
  -> 57f10ff09a5e1b4e1e776ed7be2b0d46d6f7c411  production implementation
  -> 4d0c6091ccd9b38e0afa398584f7ec8e7862b15b  implementation handoff
  -> debf4d6c27291b3d291c2292fbb3d5fc97ccb0b6  prior review record
  -> 8b1946dcec00587971d1f7c80004a47ebaa88515  source-approved parent
  -> 5085e8afd5abb90533e834dd6bbe7444b23973a4  runtime verifier candidate
  -> 4d95e45245d660e24b91e26754d9c57949bf3384  worker result only
```

Fetched `origin/feature/scenario-one-workbench-retirement-20261105` and registered lane HEAD both resolve to `4d95e45245d660e24b91e26754d9c57949bf3384` at review. The old standalone clone is clean at `5085e8afd5abb90533e834dd6bbe7444b23973a4`. The registered lane's post-publication HEAD is therefore no longer the runtime HEAD; this is an observed docs-only child, not evidence of a second runtime candidate. `git diff --name-status 5085e8af 4d95e452` contains only the worker result document.

`git diff --stat 8b1946dc 5085e8af`: exactly four verifier files, **328 insertions, 2 deletions**. `git diff --name-only ... -- Assets Packages ProjectSettings` is empty. The full path inventory contains no production or serialized asset file. Thus **no unreviewed production commit is present in this verifier delta**. This does not erase the uncommitted runtime modifications in B1.

| File under `Tools/Verification/` | Audit |
|---|---|
| `MfmCoverageWorld/MfmCoverageWorldUnityGate.cs` | +4/−1: replaces expected Site filtering by production `IsRetiredFromStartingWorld` with literal `BuildId != "workbench-01"`; adds seven-scene/six-expected count. Independent mask construction and remove-each-mask-kind negative controls retained. |
| `StandGeometry/Enlarged80Verification.cs` | +2/−1: footprint exclusion oracle skips literal workbench ID. Existing default-hidden renderer/collider assertions retained. Existing stale version pin remains. |
| `WorkbenchRetirement/WorkbenchRetirementVerification.cs` | New 262-line lifecycle verifier. Independent oracle and early witnesses valid; three execution defects identified in B2; later sections not reached on committed harness. |
| `WorkbenchRetirement/run_workbench_retirement.py` | New 60-line disposable runner. Audited isolation, staging, timeout, removal, licence cleanup, exit and marker handling. Major integrity defects M1. |

At review all four files' registered-lane bytes equal their Git blobs. Independently computed SHA-256s, in the order above: `5c2cf9a437479fde4b257ffd6e8d3770499476859c3d344d1ccd83e7e62a9ddc`, `99b2e9537e1b3269c7a70bd68a205029c03b198b61fab60f03b585d6c60e7eec`, `7bc37e20492326f19a0f149855e4c34c9157a3f8c10764c02130c5135a30073a`, `b39f74f4858ab23f892477d2fbff44681ebc88e1ff753459aeb5c3f35c457a22`.

## Raw runtime tally (dispatch B)

These are **observed behavioral log outcomes under the drifted runtime configuration**, not qualification of the clean tracked package candidate. Counts below were recomputed from line-start markers in the raw `.log` files, not accepted from manual marker extracts or worker prose. Stack-trace repetitions were excluded. No claimed numeric count disagrees with its raw emitted marker count.

| Run | PASS / FAIL / NOT RUN and exact evidence |
|---|---|
| Cold import/compile | **PASS**, second attempt `E/compile.exit:1` = `exit=0`; `E/compile.start:1` 00:47:38Z to `compile.end:1` 00:57:35Z. Zero regex `error CS[0-9]+` matches in 15,963-line `compile.log`; engine `6000.6.0f1 (f7f8ed4d1e24)` at line 203, refresh 535.657 s at 15861, successful batch exit at 15962. First attempt **FAIL exit 198**, retained in `compile-attempt1.exit` / stale-home-licence log. Zero compiler errors does not mean no Editor exceptions across all runs. |
| WorkbenchRetirementVerification | **35 PASS / 1 FAIL**, 35 `WORKBENCH_PASS` markers; first failed assertion at `E/workbench-gate/WorkbenchRetirementVerification.log:1785`; terminal `WORKBENCH_RETIREMENT_VERIFY_FAIL checks=35` at 1799. “Check 36” is correct here. Sections **C–G NOT RUN** on committed file: disk reload, empty/null negative, reference enter/exit, Year-3, Legacy40. |
| MfmCoverageWorldUnityGate | **57 PASS / 0 gate FAIL**, 57 `MFM08A_CHECK`; terminal `MFM08A_UNITY_PASS checks=57` at `E/mfm-coverage-world-gate/MfmCoverageWorldUnityGate.log:2098`. Seven scene/six expected at 1653; track=104, clearing=1, site=6, headland=4 at 1681; independent mask comparison at 1694; per-kind negative controls at 1722/1736/1750/1764; player-move invariance at 1778. Approval/execution of 149 rack tasks at 1425/1455. |
| Enlarged80Verification | **6 emitted PASS groups / 1 failure / later groups NOT RUN**. `ENLARGED80_CHECK` occurs six times; terminal failure `an Enlarged80 capture is v23 + geometry 1` at `E/enlarged80-gate/Enlarged80Verification.log:1524`. “Check 7” is the seventh reporting stage, not the seventh individual `Check(...)`: many atomic checks precede it. Tree accounting 1,344=336+1,008 at 1257; 335/336 core ID/position preservation with documented P0009/P0600 exception at 1300; start hash `71481A025AB269B2` at 1374; hidden sites at 1480. No pinned Year-3 execution. |
| ScenarioReferenceVerification, default Enlarged80 | **FAIL**, after archive-integrity success. Raw `E/scenario-reference-gate-run1-default-enlarged80-fails-Matches-by-design/ScenarioReferenceVerification.log` contains the archive-loader verification failure. Production `ScenarioReferenceArchive.cs:115–116` requires Legacy40 for `Matches`; this is the wrong environment for this standalone harness, not proof of broken preview-from-Enlarged80. That separate cycle remains unqualified. |
| ScenarioReferenceVerification, Legacy40 override | **PASS behavioral markers** under documented `CCF_STAND_GEOMETRY=0`. `E/scenario-reference-gate-legacy40-env/ScenarioReferenceVerification.log:575,590,605,620` has four exact archive hashes: Y0 `A564039D9B7CE31D`, Y20 `F7DF7DAB53B6FD32`, Y50 `D5E75D6D21D631AC`, Y100 `7AD177B3CC2F73C7`. Archive integrity at 635 and tamper negative control at 649. Preview PASS Y0/20/50/100 at 682/697/710/723 with 336/352/401/517 trees. Continuation at 5691: deterministic, `EBD49ADEF18C3F36`, v15-layout `BFFF66015A6619DA`; contract at 5704, terminal `REFERENCE_FUTURE_V1_PASS` at 5717. |
| RackV2PersistenceVerification mode v2 | **0 emitted PASS groups / 1 FAIL at schema entry**. `E/rackv2-persist-v2/RackV2PersistenceVerification.log:628`: `CurrentVersion == 23` fails. Earlier missing-`CCF_DIAG_DIR` attempt preserved separately. Successful setup checks are not emitted PASS groups. **legacy40/coldstart NOT RUN**; no qualification credit from failing entry. |
| RackV2Objective1EndToEnd | **173 PASS / 0 gate FAIL**, counted 173 `O1E2E_CHECK`; `E/o1e2e/RackV2Objective1EndToEnd.log:5252` = `RACKV2_O1E2E_PASS checks=173 shots=24`. `result.json` says exit 0, 310.6 s, no reaper, runtime SHA `5085e8af`; `candidate.txt`, `lock.json`, and `staged-sources.json` corroborate source/lease. Real W movement at 2152; five native spray checks at 3809/3907/3991/4075/4159; disk Save/CanLoad/load state checks at 5169/5197/5211/5225. |
| Dedicated workbench native location/prompt check | **NOT RUN**. Early renderer/collider/availability assertions are not a native aim/interact/no-prompt witness. O1E2E shots do not specifically exercise this location. |

**Process-exit limitation:** `E/workbench-gate.stdout` and the corresponding other batch stdout files end in `IsADirectoryError` from the runner. Their root `.exit` files contain **wrapper exit 1**, including the MFM and Legacy40 Reference runs that emit PASS. These are not trustworthy Editor exit-code receipts. The worker's `RUNNER-CRASH-NOTE.txt` explicitly says the Workbench Editor exit code is unknown. Thus MFM/Reference semantic PASS is grounded in completed log markers, not a successful runner or independently preserved Editor return code. Compilation and O1E2E have separate valid exit-0 evidence.

O1E2E PNG headers independently confirm **24 files, 12 at 1280×720 and 12 at 1920×1080**. Reviewer visually inspected `c01-objective-1-before-planning-1280x720.png` and `c09-five-entrances-marked-1920x1080.png`: real Objective/UI/forest renders, five-entrance status and an aimed entrance-tree prompt. No workbench-specific visual claim follows. Native-action JSONL plus `Tools/Verification/RackV2Objective1/native_input.py:2–31` supports OS `xdotool` events on a private display; this is Linux windowed Editor evidence, not a standalone Player/Windows test or a human playtest. Remaining 22 captures were counted/dimension-checked, not individually visually approved by this reviewer. Search-index `ArgumentOutOfRangeException` appears in the O1E2E log at 1701 and also MFM at 1177 / Reference at 562; gate tokens do not certify a universally clean console.

## Findings and verifier-correctness audit (dispatch C–D)

### B1 — FAIL, blocking: exact candidate/package attribution is broken

The logged C# behavior is attributable to the inspected production/verifier sources, **with disclosed environmental drift**. It cannot be called qualification of the clean tracked `5085e8af` project. `E/editor-rewrites-after-cold-import.diff` proves both package files changed three direct dependencies from **1.1.0 to 1.1.1** (`com.unity.sdk.linux-arm64`, `com.unity.sdk.linux-x86_64`, `com.unity.toolchain.linux-x86_64-linux`) and the lock's transitive `com.unity.sysroot.base` likewise. `E/compile.log:266–270` proves actual 1.1.1 PackageCache resolution during cold compile. This affects compilation provenance even if these toolchain packages plausibly do not alter forestry arithmetic; no equivalence test proves that assumption here.

Live registered-lane `git status --short` confirms **eight modified tracked paths**: those two package files, `.vscode/settings.json`, and five SectionFive materials (`Bilberry_Leaves`, `Bilberry_Stems`, `Forb_Flower_Centre`, `Forb_Leaves`, `Forb_Stems`). `E/editor-rewrites-after-windowed-run-materials.diff` includes `_MainTex` assignment and `_AlphaToMask` 0→1 on leaf materials as well as small color float rewrites. They must not all be dismissed as formatting or declared rendered-byte equivalent. No source/scene modifications or staged disposable harness were observed in that status.

The O1E2E external-runner adaptation explicitly removes the blanket clean-lane refusal in favor of a three-path allowlist (`E/run_gate_287.adaptation.diff`, hunk at original lines 44–47). It preserves the held task/lane/HEAD check, but HEAD equality alone cannot prove package equality. The allowlist is by path, not by expected diff/hash. Calling this run “HEAD ... unmodified” without the package qualifier is too strong, even though the worker later discloses the exception.

Operational consequence: installed `/media/jer/ZX20/CCF-Ops/scripts/unity-lock:126–128` verifies HEAD/branch and then rejects a dirty lane. The current lane would fail that condition. **No acquire was attempted by this reviewer.** Manager must authorize a preservation/reconciliation plan for these eight existing modifications, settle actual package versions and pin any authorized package change separately; otherwise make the runtime honor the committed versions. A future run must record clean tracked bytes, resolved package versions and per-phase drift rather than allow arbitrary contents at known dirty paths. This is not permission to reset or commit the existing side effects.

### B2 — FAIL, blocking: committed lifecycle gate is false-red and cannot qualify sections C–G

**Check 36 is a VERIFIER BUG, not a demonstrated product regression.** [Actual assertion, line 181](https://github.com/jermur788/CCF/blob/5085e8afd5abb90533e834dd6bbe7444b23973a4/Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs#L181) filters `Buildables().Where(b => b.HasPrerequisite)` and requires **all** prerequisites met, while the fixture builds only the workbench. Production `ForestBuildable.cs:207–208` defines `HasPrerequisite` as non-null `requiredBuildable` and `IsPrerequisiteMet` as that referenced structure's built state. Correct graph, independently read from scene file IDs:

| Dependents | Required scene component | Scene evidence |
|---|---|---|
| `saw-pit-01`, `timber-sledge-01`, `shelter-01`, `log-rack-build-01` | Workbench fileID `778820503` | `Assets/Scenes/ForestTest.unity:893–894,5271–5272,8774–8775,11387–11388` |
| `log-rack-build-02`, `grinding-stone-01` | Saw Pit fileID `113386332` | same scene: `2036–2037,3597–3598` |

The latter two correctly stay unmet while the Saw Pit is unbuilt. Diagnostic raw log corroborates four met workbench dependents and unmet Saw Pit dependents at `E/DIAG-workbench-gate-modified-harness-run3/WorkbenchRetirementVerification.log:1869,1883`. Narrow the predicate to actual workbench references and independently assert the exact four identities/count, plus the exact two Saw Pit dependents still unmet. Do not make all buildables built to satisfy the faulty assertion.

Two further committed verifier defects block a simple one-line repair:

- **Lines 183–190 / helper 84–93:** section B leaves the saved workbench restored/built. `ResetWorld` destroys trees and regenerates ecology; it does not reload scene components or rerun `Awake`. Therefore section C's supposed fresh game inherits an old workbench. Production fresh start calls `StandaloneSessionMenu.Restart` → `SceneManager.LoadScene` (`UI/StandaloneSessionMenu.cs:146–148`), while `ForestBuildable.Awake:47–49` reapplies retirement/built presentation. Reproduce that fresh-state setup in the verifier, with assertions showing it happened; preferably witness the actual restart path where feasible. The diagnostic proposal's explicit retirement/unbuilt reset is justified as fixture setup, not evidence the production reset path was exercised.
- **Lines 46–50:** `Check` dereferences `detail.Length`; `CanLoad(out problem)` can succeed with null problem (`ForestSaveController.cs:240` and its read path), passed at verifier line 190. Add null normalization before using length; retain failure semantics and do not increment/log a success before a potentially throwing detail operation.

Independent Site inventory is nevertheless valid: verifier lines 124–126 derive expected IDs by **literal workbench ID**, not production retirement status; lines 137–156 compare saved inventory and six/seven Site masks. MFM expected sites likewise use literal ID at lines 231–242. This is an independent retirement oracle; shared geometry/classifier primitives remain shared and do not establish an independent forestry model. Count equality alone is weaker than a full immutable scene inventory, but the actual inspected scene graph and saved IDs provide the required bounded policy witness. Baseline/historical seven-mask cases really occur in raw pinned log, not only in a diagram. Track/Clearing/Headland independent-construction comparisons and remove-kind controls pass; a historical before/after count matrix was not recorded.

Diagnostic proposed code is **not approved code for a future candidate**. The external lifecycle copy emits **103 PASS** (`...run3/...log:3188`, 103 line-start `WORKBENCH_PASS` records), including disk omission/CanLoad at 1927/1956, empty/null cases at 2056–2100, four reference enter/exit cycles at 2144–2572 and Legacy40 at 3089. It was run as a modified disposable script, not the committed verifier. The archived proposal and its diff must be reviewed and committed under a new pin before any qualification credit transfers.

### B3 — FAIL, blocking: final-reset tree substitution remains unexplained

`E/DIAG-enlarged80-gate-modified-harness-run4/Enlarged80Verification.log:1988` prints start `71481A025AB269B2`, final `23CF72B3FDD59D2D`; line 2027 fails the final reset assertion. Reviewer parsed the paired `config/unity3d/DefaultCompany/CCF/diag-start.json` and `diag-final.json` in that directory, keyed trees by ID, and compared records:

- Both have 1,344 trees and identical buildables (six, workbench absent).
- Lost `P0604` at (−12.146471, −8.072353); added `P1209` at (−2.054706, 3.910000), with different dimensions.
- All **1,343 shared tree records are identical**. Top-level differing values are `trees` and `scenarioOne`; list order also differs. This is a real membership substitution, not merely order or buildable hashing noise.

Production `ForestStartingStand.cs:325–342` collects the **current player transform** as a clearance anchor, making leaked player pose a concrete source-level hypothesis. The diagnostic `ResetWorld` does not restore player pose. That is not a proven cause without paired pose/clearance observations and a controlled rerun. The existing proposal's workbench reset does not solve the failure; the worker's extra approach-state attempt did not close it either. **Do not remove the final equality assertion, retune omissions, move trees, or choose a replacement hash.**

The independent Year-3 diagnostic agreement is useful, but it covers a particular fixture state. It cannot establish stable start/Year-3 identities across all required resets while this state dependence remains unresolved. A bounded verifier-state repair may suffice if it reproduces clean scene initial state and proves why the substitution happened; if the problem reproduces through the genuine production restart path, return a production defect for separate authority/review. The present evidence does not decide that branch.

### B4 — FAIL, blocking qualification: historical persistence is incomplete; stale pins pre-exist

Both stale entry pins demonstrably exist on **source-approved parent `8b1946dc`**; `ForestSaveData.CurrentVersion` is already 24 there as well as at `5085e8af`:

| Verifier | Parent → candidate lines | Classification |
|---|---|---|
| Enlarged80 | `saved.version == 23` at 273 → 274 | **Pre-existing stale fixture**, not introduced by the four-file verifier commit. It still blocks its qualification run. |
| RackV2Persistence | `CurrentVersion == 23` at 174 → 174; entire file unchanged | **Pre-existing stale fixture**, not a workbench regression. Its mandatory historical/coldstart coverage remains absent. |

**An entry-pin-only Rack repair is insufficient.** The same file has further current-file version 23 assertions at **329, 476, 527**. `ToV20` at **258–270**, especially replacement at **268**, rewrites only `"version":23,`; it will not convert a v24 capture. The v24-only teaching history field also requires an explicit reviewed historical-projection policy, not a renamed label. Preserve genuine v20/v21 fixture semantics, strict refusal tests, frozen bytes and coldstart checks; do not simply accept any schema. Manager scope extension is required before editing this fifth verifier file.

The worker's §6 description **“real v19/v20 saves diag PASS only” overstates the evidence**. Inspected `Enlarged80Verification.cs:272–288` captures the current save version, validates/loads it, then emits a stale “v20” group label. Lines **291–299** create a current Legacy40 capture, remove `standGeometryModel`, and retag it v19. They are synthetic compatibility fixtures, not untouched historical v19/v20 saves. The diagnostic pass does not change that provenance. Genuine frozen v12 reference coverage does exist separately; it cannot replace the unrun Rack historical-save/legacy40/coldstart modes. The early lifecycle cases likewise construct synthetic old-style workbench entries from a current capture; accurately label those witnesses.

### M1 — FAIL, major: runner can false-green and loses gate exit receipts

`run_workbench_retirement.py:43–49` removes the staged `.cs` and `.meta`, then calls `read_bytes()` on **every entry**, including the Editor-created `licenses/packages/` directory. The resulting exception occurs **before licence-directory removal and before marker/JSON writes at 50–54**. `E/workbench-gate.stdout:1–19` and the crash note prove the actual failure. Gate logic has already run, so the crash cannot change its logged PASS/FAIL assertions, but **it loses the Editor return-code receipt** and conflates wrapper failure with gate failure. It also skips the runner's normal licence cleanup; do not assume all finally operations completed merely because the harness was removed.

Additional integrity defect: **lines 50–57 do not require a gate-specific terminal PASS token and do not reject a FAIL token**; they merely record/filter text and return the subprocess status. The runner also documents rather than enforces caller lease ownership (line 7). Genuine external lease evidence exists for this run, so that is not evidence of lock misuse; a future qualification wrapper should fail closed on owner/lane/HEAD/configuration as well.

Reviewer executed the unchanged candidate Python source **in memory**, with all `main()` filesystem and subprocess calls replaced by virtual objects/functions. No Unity process, clone, test file or lease was created. Three bounded negative controls produced:

| Negative | Input | Actual candidate-runner behavior |
|---|---|---|
| N1 | Mocked Editor exit 0, log has no verifier verdict | **Returns 0**, writes `exit_code:0` receipt. False-green risk if runner exit is consumed as the gate. |
| N2 | Mocked Editor exit 0, log contains `WORKBENCH_RETIREMENT_VERIFY_FAIL checks=0` | **Returns 0**, writes `exit_code:0` receipt. Does not enforce FAIL markers. |
| N3 | Mocked terminal PASS, licence glob includes directory | Raises `IsADirectoryError`; harness/meta removal happened; licence cleanup and JSON receipt did **not** happen. |

Method: `git show 5085e8af:<runner path>` → Python `compile/exec` with non-main module name; replace `Path`, `ROOT`, `sys`, `subprocess`, `shutil` in module globals, then invoke `main()`. This is a mocked Python orchestration test only, not C#/Unity qualification. Repair should preserve actual Editor return code independently, hash only regular licence files or safely handle subdirectories, guarantee cleanup/receipt persistence even if hashing fails, and require the expected terminal PASS with no gate FAIL/compiler-error markers. Missing/ambiguous verdict must fail, even on Editor exit 0.

### M2 — NOT RUN, major: no dedicated native hidden-workbench/prompt evidence

Pinned early assertions prove `IsRetiredFromStartingWorld`, unavailable construction, zero active renderers and zero enabled colliders. Production `ForestBuildable.cs:58–59,92–95` guards interaction/building. These are worthwhile source/API witnesses; they do not substitute for actually approaching/aiming/interacting at the former site. The 24 O1E2E captures serve rack regression, not this specific requirement. Add a bounded native 720p/1080p former-site witness showing movement/aim/use causes no workbench selection/build prompt and no invisible obstruction, alongside assertions for no Site mask and unchanged world/save. No Windows/Player build is implied.

### m1 — FAIL, minor: plural hash-delta claim exceeds the implemented comparison

Worker §5.2 says the new-game “hashes differ only” by the omitted buildable entry. The committed strip-and-compare proof at `WorkbenchRetirementVerification.cs:154–156` is specifically **the starting world**. The diagnostic Year-3 baseline addition only advances and prints baseline hashes; it does not perform the same paired Year-3 capture-minus-entry proof. Diagnostic new Year-3 `044C4578E3E8DBA9` agrees between two harnesses, and unretired Year-3 `92F9635566BF6AFE` matches an accepted historical label. That supports a bounded comparison, not the stronger paired-state proof. Narrow the statement or add the explicit comparison with no anchor retuning.

### N1 — PASS evidence / note: historical frozen identity and labels remain Manager-owned

Independent SHA-256 verification matched **all three** before/after hash rows, the current read-only lane files, and both parent/candidate Git blobs. The before/after text headers differ in timestamp, as expected; the hash records agree:

| Protected path | SHA-256 |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes` | `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc` |
| `Assets/ForestPrototype/Docs/ScenarioOneReferenceFutureV1.md` | `fd9e3c79d81484a0360d75e6c96cc32d0bf3bfecf735a991e08e5811f0adf9ec` |
| `Docs/ReferenceFutureContract.md` | `239ed039b8aeaacdbc42aa37fbd4dc49c0929f30325a8681660504fca05e27b9` |

Diagnostic lifecycle raw log `...run3/WorkbenchRetirementVerification.log:1349,1464,2870,2883,3018,3031` contains:

| Measured diagnostic label | Hash |
|---|---|
| Retired start / Year 0 | `71481A025AB269B2` |
| Retired Year 3, continuous = resumed | `044C4578E3E8DBA9` |
| Unretired baseline-equivalent start / Year 0 | `94589C749852F359` |
| Unretired baseline-equivalent Year 3 | `92F9635566BF6AFE` |

The unretired start is also observed in the pinned early lifecycle run; the Year-3 values are diagnostic only. The newer unretired anchors agree with `Docs/Handoffs/RACK-V2-03C/HANDOFF.md:99,104` and `Docs/Handoffs/RACK-V2-Combined-Integration/HANDOFF.md:88–89` at parent `8b1946dc`; older #284/80B labels `04A78188A6F8E6F9` / `081DB58CE3034648` are historical configuration labels, not a universal baseline. Matching newer labels is supporting evidence, not authority to replace old ones. Frozen Y100 `7AD177B3CC2F73C7` and archive previews match exactly. Current continuation `EBD49ADEF18C3F36` and v15-layout `BFFF66015A6619DA` are different contracts from old `9CDF21A541C5968D`; retain their labelled history. **No rebaseline approved here; Manager #200 decides label reconciliation.**

### N2 — process observation / note: unqualified candidate was pushed

The remote now includes `5085e8af` and result child `4d95e452`. The Manager admission amendment §capacity item 6 conditions publication authority on verifier/source tests truly passing and a qualified exact feature. The worker instead published a candid NOT QUALIFIED result plus the unqualified verifier for provenance. This review records the mismatch/exception for Manager #200 and **does not rule on publication authority, acceptance, or retrospective waiver**. Fast-forward ancestry alone does not satisfy qualification. Historical #311 blocked admission and #312 operations-only acceptance also remain distinct and are not retroactively Unity passes.

## Lease and storage audit (dispatch E)

**PASS, note: no evidence of lock misuse or recorded capacity breach.** `E/lease-acquire.txt` records genuine held lease `fa8b4b65bc1b4dda8f21f4b6bcb8c82d`, owner `mimo-287-claude-sonnet-5-5`, task `287`, registered path `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106`, branch and full runtime HEAD `5085e8af...`, acquired **2026-11-07T00:45:32.178021Z**. O1E2E's copied `lock.json` corroborates it. `E/lease-release.txt` records matching normal release, `forced: False`, no Editor PIDs. Installed history `/media/jer/ZX20/CCF-Ops/state/unity/history.jsonl:338` records release **01:49:05.746890Z**, with the full previous metadata and acquisition timestamp. The installed acquire implementation does not append a separate acquire history row; absence of such a row is not a missing lease when the actual held receipt and release metadata exist.

Reviewer independently ran the required read-only `python3 /media/jer/ZX20/CCF-Ops/scripts/unity-lock status`: **free, metadata None, unity_editor_pids []**. No acquire/release/force operation was executed. Current mount inspection confirms ZX20 `/dev/sdc2`, exFAT, rw; Files `/dev/sdb4`, ext4. These are current device observations, not a device-string rewrite or independent replay of the worker's historical hardware preflight.

`E/disk-log.txt:1` records pre-acquire **30,277,238,784 bytes = 28.20 GiB**, above the Manager's 27 GiB cold-entry margin. At line 4 post-compile **23,237,754,880 = 21.64 GiB**; final line **23,216,128,000 = 21.62 GiB**. `disk-monitor-compile.log` contains 30 samples roughly 20 s apart, plus per-run before/after checks in `disk-log.txt`. The recorded decline is consistent with one cold Library of roughly 6.6 GiB; it shows no second cold import and no crossing of **20 GiB STOP / 15 GiB hard floor**. These are sampled observations, not continuous proof of every instant or an exhaustive all-lane disk inventory. No broad disk scan or cache manipulation was performed by the reviewer.

The retained Library can potentially support a warm rerun, but current free space has only about **1.62 GiB above the extra STOP margin**. The original amendment forbids unapproved changed-HEAD import and requires Manager repinning/capacity approval. Warm-cache availability alone is not fresh permission. Do not wipe/recreate/share Libraries or use the old clone as a shortcut.

## Required repair and bounded rerun recommendation to Manager #200

**Keep #287 NOT QUALIFIED / NEEDS CHANGES. Do not integrate on this evidence.** A verifier-only route can plausibly suffice without a production rewrite, conditional on resolving B3 and exact-package provenance. The reviewed diagnostic copies remain proposals for a new candidate; none is approved for acceptance, and no proposed commit has been runtime-tested as a committed clean candidate by this reviewer.

1. **Reconcile provenance/ownership first.** Preserve the eight current lane modifications through a separately authorized byte-exact plan. Decide whether the intended candidate truly uses tracked 1.1.0 packages or an explicitly approved committed package update. No incidental package policy change is authorized by this review. Confirm exact HEAD/branch/clean status/resolved versions and spare capacity; obtain Manager authorization for the new pin and warm import, then a fresh genuine lease by the assigned operator.
2. **Repair and independently inspect exact verifier lines**, retaining all assertions and negative controls:
   - `WorkbenchRetirementVerification.cs:46–50` null detail; `178–181` exact prerequisite edges/count/identities and unmet negative; `183–190` fresh-state setup consistent with scene restart. Retain C–G, save isolation and all original hash/tree checks; add paired Year-3 entry-only comparison if claiming it.
   - `Enlarged80Verification.cs:274` current-schema expectation and truthful label; `81–94,394–396` reset state. Instrument pose, clearance anchors and tree membership, establish the cause of the one-tree substitution, and retain the final equality assertion. The archived diagnostic diff is not a solution to that final failure. Do not turn a schema check into accepting arbitrary versions; explicitly assert the approved current version where the contract needs a pin.
   - `RackV2PersistenceVerification.cs:174,258–270,329,476,527`: current-schema and genuinely historical fixture projection/version rules, including v24 fields, under explicit scope extension. Preserve rejection/legacy/coldstart paths; identify genuine archived save fixtures separately from projections.
   - `run_workbench_retirement.py:38–57`: durable Editor-exit/marker/cleanup receipts even on post-run errors; directory-safe licence handling; terminal-PASS/no-FAIL enforcement. Fail-closed metadata/source/config checks must cover actual bytes, not simply the same Git HEAD.
3. **Rerun the focused Tier-2 matrix on the approved exact clean pin**, with no speculative extra cold Library: compile/import as needed; lifecycle A–G fully; MFM parity; complete Enlarged80 including final reset and paired Year-0/Year-3 continuous/resumed identity; Rack persistence `v2`, `legacy40`, then fresh-process `coldstart`; frozen Reference integrity/previews/continuation under the documented Legacy40 override plus the lifecycle preview-return from Enlarged80. Keep diagnostic/default-environment failures in history. Record real Editor exits separately from runner exits.
4. **Close the presentation gap:** bounded native workbench former-site/no-prompt/no-collider witness at 720p/1080p, plus narrow O1E2E regression on that pin (24 existing frames may guide the operator; this review does not demand a broader Tier-3 programme). Inspect relevant rendered output; retain asset/package drift receipts rather than calling dirty material bytes exact-candidate visuals.
5. **Return to independent review/Manager.** If the cause is only incomplete verifier reset and all bounded gates pass with clean provenance, a verifier-only repin plus focused warm rerun should be enough for this feature gate. If genuine production restart/tree identity or package equivalence cannot be established, verifier edits alone are insufficient; return the precise production/package question for authority. Manager alone settles historical hash labels, publication exception, source/runtime acceptance, and any later integrated qualification/release.

**Closure/operations:** report publication and the #317 [HANDED-OFF] receipt are the only requested external writes. No source/scene/checker/save/reference/Manager/ledger file was edited. Dashboard/coordination changes are outside this reviewer's allowed writes; **dashboard synchronization and published dashboard reflection are NOT VERIFIED** and remain Manager/Mimo follow-up. This review does not close #317 administratively or self-accept #287.
