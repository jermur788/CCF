# PARITY-05 Unity harness reconciliation — claim #101

**PARITY-05 UNITY QUALIFICATION BLOCKED — MANAGER REVIEW REQUIRED**

G1/G2 pass. G3 completes with all behavioural checks passing, but **both protected browse hashes fail**. This is a stopped, unqualified diagnostic candidate, not permission to integrate or rebaseline. #101 remains open; #70 cannot close.

- Executor: OpenAI / `gpt-6.1-sol`, Codex through T3 Code, reasoning effort `high` (live orchestration catalog confirmed).
- Task: bounded verification-only diagnosis/repair under #101, parent #70, umbrella #91. Latest direct task authorises verification repair and requires #101 to remain open.
- Source worktree/branch: `/media/jer/Videos/CCF-Parity05Fix`, `fix/parity-05-unity-harness-v1`.
- Base: `8b6c3b03a9929ffd3d55119384c3a5a88c409650` (umbrella integration, containing PR #69).
- Context commit: `63e95ba2175f7e239a278c616681b286719821f8`; live GitHub coordination/claims refreshed at start. The latest direct task assigns U1 under #91 despite the older queue text.
- Exact committed Unity candidate: `18b31ada647f177c8190d74740141d3ad8753fcf`.
- Unity: **6000.6.0f1**, U1 `/media/jer/ZX20/CCF-U1`, detached at that exact SHA; own existing Library; Linux batchmode/nographics; no Windows or Player build.
- Durable evidence: `/media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/`.
- Original failed evidence: `/media/jer/Files/CCF/Verification/Parity05-Unity-20261029/`.

## Diagnosis and bounded repair

1. **Juvenile Legacy save — test fixture/configuration defect.** The failed run started Enlarged80 (1,344 stems, 256 cells). Its `livingSave` retains the current stand geometry and the fresh Enlarged80 Rack model 1 through the original scenario snapshot. Relabelling that clone v13 does not make it a valid historical save. The actual first refusal is `a save older than version 20 cannot carry a stand geometry model`; the pre-v21 Rack validator also prohibits retained model-1 state. This is the same stale-clone class as T-06, with geometry detected first. The unchanged base harness passes in **Legacy40 using `CCF_STAND_GEOMETRY=0`**, including every mortality/load assertion (baseline-juvenile receipt, exit 0). Therefore there is no reason to alter production validation, delete fixture records, or mechanically down-convert a 256-cell world. The repair documents the required historical configuration and explicitly asserts Legacy40/model-0, empty/unconfirmed rack plan, initial allocator, no rack-linked orders and `ValidateRackV2 == null` before exercising the v13 load. The originating fixture is now required to be valid; no mortality assertion was weakened.

2. **Canonical drift — test configuration defect, not Rack/hash-layout drift.** `ScenarioOneInteractionGate.LifecycleHash` hashes ordered tree state and ecology cells directly, not `ForestSaveData` or `ScenarioReferenceArchive.WorldHash`. It cannot include v21 Rack JSON fields. The deliberate v21 compatibility strip already exists in `ScenarioReferenceArchive` and remains untouched. The failed command had no `CCF_STAND_GEOMETRY=0` override and its log proves Enlarged80 startup. `ForestStandScenarios.ApplyLifecycleFixture` resets ecology but preserves the live geometry: it therefore ran the 68-tree historical fixture on a different 256-cell grid. On the **unchanged base SHA**, the explicit Legacy40 control reproduces `BFC55473C1506067` and `3485B6630C9EA448`, exit 0. The repaired candidate also reproduces both. Added a Legacy40/64-cell precondition; the hash algorithm, fixture tree data, model selections and protected expected values are untouched. No canonical production movement is evidenced under the required configuration.

3. **Browsing probability — narrow numeric comparison repair under qualification.** The original comparison contrasts a returned float field with a float product expression. Mono can retain extra precision in the expression. The candidate logs actual/product values and the original strict-expression comparison through `Debug.Log` (never `Emit`, which feeds the protected measurement hash). Only this comparison uses `abs(actual - expected) <= expected * 1.2e-7`, a relative single-precision rounding bound. Added exact checks for pressure, palatability, vulnerability, vegetation exposure and protection access; exposed reason, species ordering, protected probability exactly zero, all outcome assertions and both protected hashes remain mandatory. Runtime confirms the original Sitka expression comparison is false even though both rounded values print `0.0300000012` (the double product is `0.030000001639127749`, stored-float value approximately `0.030000001192092896`; the difference is about `4.47e-10`). Oak/Beech strict comparisons are true. **The protected matrix/measurement hash condition fails below, so the tolerance remains a diagnostic proposal, not an accepted/qualified repair. Do not adopt the candidate as qualified.** No further repair experiment was run after discovering hash movement.

4. **Shelter scan — PASS, no repair required.** The existing binary protection scan is unchanged: 21 years × 4 positions = 84 cases, checking exact 0/1 access and the correspondence with intact-fence/effective-shelter state. No scan case, threshold or assertion was removed. `BROWSE_EARLY_LIFE_CONFORMANCE_PASS` confirms the entire existing scan completed, including its effective-state/access equivalence checks.

5. **Protected browsing hash drift — unresolved, Manager blocker.** The first complete post-float diagnostic run produces matrix `92A0DA50D6F5D3D9` instead of `BEC549ED9EBA18B7`, and measurement `A3541E0EDF86DE66` instead of `32224555690DD824`. All 144 matrix conditions and the behavioural assertions pass; the canonical hashes inside the same browsing process remain exact. The untouched original failed before generating a matrix, so there is no comparable full matrix on the task base to establish whether this is pre-existing harness/measurement compatibility or integrated production drift. Production bytes are unchanged by this task, which is **not proof** that the historical browsing anchors still match the integrated production. No production defect is asserted and no harness-only classification is claimed for this remaining drift. Per the stop condition, further diagnosis/repair/rebaseline needs Manager disposition.

## Files changed

- `Tools/Verification/JuvenileMortalityFoundationVerification.cs` — historical-configuration comments and explicit geometry/Rack fixture preconditions.
- `Tools/Verification/BrowsingProtectionVerification.cs` — historical-configuration comment/precondition, float diagnostics, bounded product comparison and exact factor assertions.
- This handoff, published in a subsequent documentation-only commit; the tested candidate above is the code commit.

No production, species, scene, prefab, material, save/schema, ecology/economy, Rack/MFM/S1-C/S1-D, package, project-setting, spec or Reference Future file changed. No new asset or dependency.

## Protected-state proof

`protected-state-proof.json` records equal Git tree objects at base and candidate:

| Tree | Exact object at both SHAs |
|---|---|
| Assets | `235b1b45fb94088029f975897a02a87df8376ff6` |
| Packages | `fa9bf0d3c9463a0a5a8a98e87e92bf66883ba27c` |
| ProjectSettings | `aa08457b80d4299389ef387fd6d9afc655a63e7b` |
| specs | `cedd11eb7e54c91fc86920a811131355da6c25c5` |

All existing 16-hex expected literals are identical. Source `Check(` occurrences increase from 49 to 52 (juvenile) and 117 to 119 (browsing); no existing check is removed. The one intentionally changed predicate is the authorised bounded probability comparison described above. The old 30-combination juvenile count, 144-condition browse matrix, 60-individual fixture size and 84-case scan remain. Production and the offline conformance checker are read-only. Static conformance: `EARLY_LIFE_CONFORMANCE_STATIC_PASS checks=30 NOT_UNITY_VERIFIED`; runtime evidence is reported separately.

## Exact gate commands and expected observations

Run from `/media/jer/ZX20/CCF-U1` at the candidate, under its normal Unity lock. Before each run, copy the current Hub `unity3d/Unity/licenses` directory into that run's isolated `config/unity3d/Unity/licenses`; never edit the host licence. The generic runner otherwise selects the stale host licence. The initial diagnostic attempt exited **198 before project loading**; the isolated current-Hub retry succeeded. This is recorded licence-operation debt, not a gate waiver.

```bash
XDG_CONFIG_HOME=/media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/g1-compile/config CCF_STAND_GEOMETRY=0 systemd-run --user --scope --quiet -p MemoryMax=8G -p MemorySwapMax=256M /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -projectPath /media/jer/ZX20/CCF-U1 -job-worker-count 2 -batchmode -nographics -quit -logFile /media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/g1-compile/compile.log

python3 -B Tools/Verification/Qualification/run_qualification.py --label juvenile --out /media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/g2-juvenile --method JuvenileMortalityFoundationVerification.Begin --file Tools/Verification/JuvenileMortalityFoundationVerification.cs --marker JUVENILE_MORTALITY_FOUNDATION_VERIFY_PASS --env CCF_STAND_GEOMETRY=0 --timeout 900

python3 -B Tools/Verification/Qualification/run_qualification.py --label canonical --out /media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/g2-canonical --method JuvenileMortalityFoundationVerification.BeginCanonical --file Tools/Verification/JuvenileMortalityFoundationVerification.cs --marker JUVENILE_CANONICAL_VERIFY_PASS --env CCF_STAND_GEOMETRY=0 --timeout 900

python3 -B Tools/Verification/Qualification/run_qualification.py --label browsing --out /media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/g3-browsing --method BrowsingProtectionVerification.Begin --file Tools/Verification/BrowsingProtectionVerification.cs --marker BROWSING_PROTECTION_VERIFY_PASS --env CCF_STAND_GEOMETRY=0 --timeout 1200
```

The runner stages each source into `Assets/ForestPrototype`, runs one explicit method in an isolated configuration and removes the copy and `.meta` in `finally`. No real save slot is written by these harnesses. Gate PASS also requires normal exit, no compiler/assertion failure and no unexpected tree changes. The packet's exact hash checks are additionally enforced against the logs; a runner PASS alone does not waive them.

## Gate results

- G1: **PASS**, normal exit 0, zero C# compilation errors (`g1-compile/result.json`).
- G2 Begin: **PASS**, exit 0, 60.9 s; `EARLY_LIFE_CONFORMANCE_VERIFY_PASS`, `SHARED_JUVENILE_RULES_VERIFY_PASS combinations=30`, `JUVENILE_PROMOTION_PERSISTENCE_VERIFY_PASS`, `TREE_MORTALITY_FOUNDATION_VERIFY_PASS`, `JUVENILE_MORTALITY_FOUNDATION_VERIFY_PASS`.
- G2 BeginCanonical: **PASS**, exit 0, 60.0 s; `JUVENILE_CANONICAL_VERIFY_PASS`, `CANONICAL_ISOLATION_HASH BFC55473C1506067`, `SCENARIO_ONE_LIFECYCLE_HASH pressure=0.20 hash=3485B6630C9EA448`.
- G3: **FAIL against packet**, despite runner PASS and normal exit 0 (138.0 s). `BROWSE_EARLY_LIFE_CONFORMANCE_PASS`, `BROWSING_PROTECTION_VERIFY_PASS` and `BROWSE_SCENE_RESTORED_PASS` all appear; no `_FAIL` or C# error appears. All **144** matrix conditions ran. **Required hashes are not satisfied**:

  | Measurement | Required (read-only) | Actual |
  |---|---|---|
  | `BROWSE_MATRIX_HASH` | `BEC549ED9EBA18B7` | `92A0DA50D6F5D3D9` |
  | `BROWSE_MEASUREMENT_HASH` | `32224555690DD824` | `A3541E0EDF86DE66` |

  Same-process `BROWSE_LIFECYCLE neutral=BFC55473C1506067`; browsing 0.6 A/B both `5863F0BF89C817B5`; `BROWSE_SCENARIO_LIFECYCLE pressure=0.200 A=3485B6630C9EA448 B=3485B6630C9EA448`. The runner checks terminal markers but does not enforce these two browse hashes; `packet-results.json` records the explicit **FAIL** and both missing expected tokens. No acceptance follows from its generic PASS receipt.
- G4: **PASS**. Both disposable harnesses and `.meta` files absent; only attributable `.vscode/settings.json` normalisation occurred (recorded patch, then restored byte-exactly). No other tracked drift. All **8** real user save/preference fingerprints match before/after. `BROWSE_SCENE_RESTORED_PASS` confirms in-memory captured-world restoration. U1 and source worktree were initially clean; unrelated Primary/other-lane work was not touched. U1's own Library/import cache is the expected local Editor side effect; it was neither copied nor shared. The run configurations/logs are isolated durable evidence.

Baseline lock `ddb0813bbaae460f9365b2b3ceba7481` acquired 07:20:09 UTC and released normally before switching candidates. Candidate lock `83e84c5de14649b6b0a196723f38a47b` acquired 07:23:49 UTC with exact HEAD and clean U1; released normally after cleanup (`lock-release.json`). Final lock status **free**, no Unity Editor PIDs. U1 restored to its original detached `8b6c3b03a9929ffd3d55119384c3a5a88c409650`, final porcelain status empty. `cleanup.json` records the checks. No Windows build.

## Review and integration boundary

- IMPLEMENTED: diagnostic verification-only candidate committed; this handoff is published in a documentation-only descendant with byte-identical harnesses/production. The float proposal is not qualified because its protected-hash condition failed.
- INTEGRATED: **No**; no merge into main or the umbrella branch.
- VERIFIED: **G1/G2/G4 only**; G3 protected-hash gate **FAIL**, so PARITY-05 is not Unity-qualified.
- ACCEPTED: **No**; independent/Manager G2 acceptance, G3 measurement-integrity review and G4 integration remain with Manager/integrator.
- #101 remains open with a linked handoff comment. **#70 cannot close**: the protected browse hashes still fail, and the diagnostic tolerance cannot be accepted under the current conditional authorisation. Manager must assign bounded anchor attribution and review before any further repair/integration.
- Operational follow-up: no dashboard ingestion/publisher capability was identified in this task’s tools/checkout. GitHub claim/handoff are updated, but the published dashboard and audit-snapshot advance are **not independently verified**. Umbrella Manager must confirm synchronization; no generated dashboard state was edited.
