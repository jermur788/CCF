# PARITY-06 — Independent review (Claude A)

**Verdict: `PARITY-06 REVIEW PASS — APPROVED FOR UNITY QUALIFICATION AND G4`**

This approves the production bytes for the queued Unity-only gates and, once those gates pass, for G4 integration. **NOT UNITY-VERIFIED.** It does not grant G2 acceptance of Unity behaviour, G3 acceptance of any broadleaf-containing anchor movement, G4 integration itself or whole-parity completion.

## Identity

- Reviewer: **Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness, in T3 Code.** I worked independently and read-only. I did not implement any part of PARITY-06. The implementer was OpenAI `openai/gpt-6.1-sol` (OpenCode harness).
- Task: PARITY-06, umbrella #91, claim #92.
- Worktree: `/media/jer/Videos/CCF-Parity06`, branch `feature/parity-06-broadleaf-timber-pruning-v1`.
- **Exact reviewed production candidate: `4fb0d925afed5a2fd50d7d7973c0c9ce0384687b`**. Base: `72dbee8b8eab66f58046abfd49e77ead4c29d357`, which is an ancestor of and equal to `origin/main` at review time. The docs tip is `9ab3e8205eb0695897f9ffe7a8e3474fc407b59f` (local = origin). `4fb0d92..9ab3e82` changes only `HANDOFF.md`.
- No Unity, no machine lock, no Windows build. No production, verification, spec or coordination file was edited. This file is the only change.

## Commands run and results

All commands ran in the assigned worktree with `python3 -I -B`, so no `__pycache__` was created. Afterwards `git status` was clean. Output went to `/media/jer/Files/CCF/Verification/Parity06-Review-ClaudeA-20261029/`, separate from the implementer's evidence. Toolchain: dotnet 10.0.112.

| Command | Result |
|---|---|
| `git diff --stat/--name-only 72dbee8..4fb0d92` | 10 files: 5 production, 5 under `Tools/Verification/ThreeSpecies/`. No other `Assets/` path changed (`git diff --quiet` with the five production files excluded). No scene, prefab, asset, `.meta`, package or ProjectSettings file changed. |
| `git diff --check 72dbee8 4fb0d92` | PASS |
| `run_pruning_check.py --out <review>/pruning --mutations` | `PARITY06_PRESERVATION_PASS protected_files=17 manager_callsites=exact legacy_methods=exact`; `PARITY06_LEGACY_DIFFERENTIAL_PASS cases=1296`; `PARITY06_ADAPTER_MEASUREMENT unprunedOakCents=46845 prunedOakCents=58388 totalCostCents=250000`; **`PARITY06_PRUNING_PASS checks=3946 failures=0 NOT_UNITY_VERIFIED`**; 8/8 mutants killed (interval-two, interval-four, third-lift, no-species-gate, no-model-gate, wrong-target, wrong-crown-loss, height-cap); exit 0 |
| `run_offline_check.py --suite timber --out <review>/timber` | **`THREE_SPECIES_TIMBER_PASS checks=73`**, 73 PASS lines, 0 FAIL, exit 0 |
| `compile_runtime_sources.py --src <worktree> --managed <L01 Linux Player Managed>` | **`sources=173 refs=116 exit=0 errors=0`**. This is a syntax/type check only. It is not a Unity import or compile. |
| `sha256sum` frozen archive / schedule / SitkaGrowthModel | `789e3edd…41fc` / `91cf5059…98f0` / `29160104…6463`. All exact. |
| Protected manifest diff (review run vs implementer `pruning/protected-sha256.json`) | Identical, 17/17 |

The reproduced numbers match every count claimed in the handoff.

## Scope conformance (review item 1)

| File | Finding |
|---|---|
| `ForestTree.cs` | Only `CanPrune` and `TryPrune` changed. The runner's scrub contract confirms this, and I re-read the diff myself. The new optional `speciesAware = false` parameter keeps every existing two-argument caller on legacy rules. |
| `ScenarioOneDefinition.cs` | Two changes. (1) New overload `NextPruningTargetHeightM(ForestTree, bool)`; the `int` overload is byte-exact. (2) The serialized field `broadleafTimberValueCentsPerCubicMetre` was removed, and `TimberValueCentsPerCubicMetre` now throws `InvalidOperationException` for any species other than Sitka. `using System;` is already present. Nothing else changed. |
| `ScenarioOneManager.cs` | Exactly the six approved pruning call-site substitutions. The runner reconstructs the expected file by text replacement and asserts equality. |
| `TimberYieldConfiguration.cs` | Evidence labels, comments and source strings only. Dimensions, priorities, flags and allowed lengths are unchanged. |
| `Stage1EconomyDefaults.cs` | Evidence labels and source strings only. Every cent value, the 890 proxy, rounding and costs are unchanged. |
| Verification | Bounded. It sits under `Tools/Verification/` and writes output outside the repository. Mutations apply to generated copies only. |

I searched all callers of `TimberValueCentsPerCubicMetre`. The only one outside the definition is `Tools/Verification/ScenarioOneDeadwoodVerification.cs:174`, which passes `"sitka-spruce"` and is unaffected. No production caller reaches the throw.

## Policy dispositions (review item 2)

| Policy | Disposition |
|---|---|
| Oak five rungs | **PASS.** veneer/planking (10) → beam (20) → fencing (30) → second planking → firewood (50), in both the yield and price tables. |
| Beech two rungs | **PASS.** second planking → firewood only. |
| Quality = size + pruning/crown history only | **PASS.** Live quality comes from `ScenarioOneEconomyAdapter` pruned sections, which are unchanged. The generic allocator still accepts defect flags, but they are relabelled as synthetic robustness tests. No living-tree form state was added. |
| Teagasc typical quotations = `ManagementGuidance` | **PASS.** Covers the five non-firewood prices and the Oak veneer/beam dimensions. A new offline assertion pins this. |
| Selected references / roadside basis = gameplay calibration | **PASS.** `SelectedEvidence = GameplayCalibration` on every price, and the source string says so explicitly. Both are asserted. |
| 890 kg/m³ = work proxy only | **PASS.** The comment and source string say "unit convention, not a green density"; the existing density-label assertions remain. |
| No cost multiplier | **PASS.** `HARVEST_COST_IS_GENERIC_NOT_PER_SPECIES` gives 250000 for each species; no multiplier was introduced. |
| Broadleaf pruning 3.5/6.0 m, two lifts, ≥3 years | **PASS.** The rule is gated on model 2 *and* species ∈ {sessile-oak, beech}. The target is exact, the lift cap is 2 and the minimum interval is 3 years. The checks probe the exact year-3 boundary and years 0 and 2, and mutants at 2 and 4 years are both killed. |
| Sitka schedule exact | **PASS.** 2.5/5/6.5 m, three lifts, 5 years, unchanged with the gate on or off (differential test). |
| Dormant flat broadleaf value removed or blocked | **PASS (blocked).** The field is no longer read, the API throws for non-Sitka species, and `NO_FLAT_BROADLEAF_RATE` passes. See N4 for the orphan YAML key. |

## Compatibility (review item 3)

- **Models 0/1 and Sitka unchanged: PASS (offline).** `BroadleafMarketActive` is `GrowthModelVersion >= SpeciesAware`. When it is false, every manager path calls the legacy rule. The 1296-case differential compares against the exact base method bodies: rejection text, crown state, lift/year state and crown-target factor all match.
- **Unknown species does not inherit broadleaf rules: PASS.** The species check is an explicit ID match; a null or unknown species falls to the legacy branch. This is covered by the differential test and the `no-species-gate` mutant.
- **No new persisted state or schema: PASS.** `ForestSaveData`, `ForestSaveController` and `ForestSaveValidation` are byte-exact; the Unity harness asserts save version 21.
- **No UI copy: PASS.** No UI file changed. See N2 for an existing string that PARITY-07 must handle.
- **No fork/form state, no premium-Oak guarantee, no time-varying price deck: PASS.**
- Failed or obsolete orders: a model-2 broadleaf order with an obsolete 2.5/5 m target is rejected by validation. If it is resolved anyway, `ResolvePruning` → `Fail` runs, which spends no cash (cash is deducted only after `TryPrune` succeeds). I confirmed this from the source.
- Pruning-order targets persist as `float` and are compared exactly with `3.5f`/`6f`. Both values are exactly representable, so they round-trip through JSON without loss. No float-equality risk.

## Protected anchors (review item 4)

| Anchor | Status |
|---|---|
| Sitka 3000-stem / 11592-log fingerprint `CAA0388524E0D0E5` | Recomputed: exact against the base, and exact under the scenario configuration |
| Sitka 400-stem operation `2976F3575E0446B3` | Recomputed: PASS. The suite asserts the fingerprint against this constant and prints sale 169245486 / residual 130542728 / 12 summaries. The fingerprint itself is asserted, not printed. |
| Frozen archive `789e3edd…b041fc` | Recomputed: exact |
| Schedule `91cf5059…98f0` | Recomputed: exact |
| SitkaGrowthModel `29160104…6463` | Recomputed: exact |
| 17/17 protected files | Recomputed: exact; manifest identical to the implementer's |
| Unity lifecycle / density-controller / Legacy40 / Reference load-continuation fingerprints | **Not rerun. Unity residual.** |

## Evidence audit (review item 5)

Changes to assertions, compared with the base:

- `NON_FIREWOOD_PRICES_ARE_LABELLED_EVIDENCE_INFERENCE` was replaced by `TEAGASC_TYPICAL_QUOTES_ARE_MANAGEMENT_GUIDANCE`. This is the policy-required relabel, not a weakening. Two new assertions were added: the selected-reference/roadside calibration check and the dimension-versus-selection evidence split.
- `MORE_PRUNING_LIFTS_UNLOCK_MORE_CLEAR_LOGS` (2.5/5.0/6.5, inequalities) was replaced by `BROADLEAF_HIGH_PRUNING_UNLOCKS_ONLY_COVERED_LOGS` (3.5 m → exactly 1 veneer log, 6.0 m → exactly 2). The old monotonic comparison against 6.5 m is gone, but the new check uses the approved heights and exact equality, so it is **not weakened**.
- Unity harness: `RestorePruningHistory(3, 6.5f, 0)` was replaced by two real lifts at years 0 and 3 through `TryPrune(..., true)`. This is stronger because it exercises the real rule.
- Negative controls are genuine. Each mutant must compile, and a build failure counts as a runner error, not as a kill. Each must also emit `PARITY06_PRUNING_FAIL`. The handoff's disclosure about strengthening the third-lift fixture matches the current fixture (the restored two-lift / 5 m history). No pass threshold was relaxed.
- Synthetic economy measurement: 46845 → 58388 cents for a 60 cm / 24 m Oak, with job cost 250000 unchanged. It reproduced exactly. The handoff correctly calls it a large-stem fixture, not a solvency or completion anchor. No claim goes beyond this evidence.
- The stubs (`PruningOfflineStubs.cs`) are limited to Unity data and attributes. Visual hooks are inert and salvage throws. The production pruning block is extracted verbatim, not re-implemented.

The handoff's claims match the evidence. It is correctly marked NOT UNITY-VERIFIED, and the exit token is correctly scoped to static/offline work.

## Findings

### Blocking

None.

### Non-blocking

- **N1. The Reference runner's candidate filter is not species-aware.** `ScenarioReferenceRunner.cs:475` still calls `NextPruningTargetHeightM(t.PruningLifts)`. Under model 2, a two-lift Oak/Beech remains a "candidate" there. The manager then rejects it correctly, the runner moves to the next tree, and nothing wrong is applied. The selection order of a model-2 Reference run can still move, though. For the frozen Reference configuration the gate is off, so behaviour is unchanged. Any model-2 Reference or witness movement belongs to G3. This file was not in the released surface, so not changing it was correct. Record it as a possible follow-up only.
- **N2. Existing UI copy for PARITY-07.** `UI/TreeInspectionView.cs` (Oak branch) says logs "can reach first-grade veneer/planking if they are large and straight enough" and "Premium Oak needs a large, straight log". FR-9 forbids "straight enough"/form claims. This was not introduced here and PARITY-07 owns it, but it must be fixed there.
- **N3. Model-2 broadleaf pruning cost will move.** Pruning minutes and cost scale with target height (`PruningBaseMinutes + target × PruningMinutesPerMetre`). Model-2 Oak/Beech now pay for 3.5 + 6.0 m over two lifts instead of 2.5 + 5 + 6.5 m over three. This is generic and contains no species multiplier, but any model-2 economy or solvency witness with broadleaf pruning will move. The G3 witness in residual step 4 must attribute it.
- **N4. Orphan serialized key.** `ScenarioOne.asset:24` still contains `broadleafTimberValueCentsPerCubicMetre: 6500`. Nothing reads it. However, any later intentional Editor save of that asset will drop the line. The G4 integrator and the serialized-lane owner should expect that one-line deletion, recognise it as caused by PARITY-06 rather than unrelated churn, and remove it deliberately when the asset next has a single writer.
- **N5. The strengthened Editor harness is unrun.** Its timing expectations need Unity: `LastPruningYear == 4` after load, the three-year wait expressed as two `AdvanceYear` calls, and whether the spawned fixtures are tall enough for 3.5/6 m (target < 0.6 × height). I traced the timing from source and found it consistent, but only Unity confirms it.
- **N6. A few timber fixtures still use `Pruned(6.5, …)` for Oak.** Examples are `PRUNING_RAISES_VALUE…` and `GRADING_IS_DETERMINISTIC`. These represent a pre-existing three-lift history that remains loadable and is deliberately preserved, so the fixtures are valid allocator tests rather than a policy error.

## Unity-only residual assessment (review item 6)

The handoff queue (steps 1–4) is **complete and correctly scoped**:

- **G2 behaviour:** Step 1 is import/compile on 6000.6.0f1. Step 2 is `BroadleafTimberEconomyVerification` under Legacy40 and Enlarged80, covering models 0/1/2, manager/batch targets, recovery, cap, save/load, pricing and settlement. Step 3 covers obsolete pending orders, histories restoring without mutation, zero spend on ineligible orders, crown/visual refresh, and ecology relaxation from the two-lift state.
- **G3 measurement integrity:** Step 4 reruns the protected Sitka, Legacy and Reference gates under their original configurations without changing expected hashes. It also measures a model-2 completion/economy/solvency witness and attributes any broadleaf-containing movement for Manager acceptance. N1 and N3 should be named explicitly in that attribution.
- **G4:** after G2 and G3, follows the existing schema/integration order.

Two **additions** I recommend inside the existing steps. They are not new gates:

1. Step 4: also run the existing `ScenarioOneInteractionVerification` (`Assets/ForestPrototype/ScenarioOneInteractionVerification.cs:95`, which uses `CanPrune(2.5f, 1)` on the legacy default API) and `ScenarioHabitatPresentationVerification` (which reads `PruningLifts`), to confirm the legacy default path still holds in the live scene.
2. Step 1: report the `ScenarioOne.asset` load without warnings about the orphan field, and confirm the asset bytes are unchanged after the run (N4).

The disposable-harness copy/run/remove rule and the `forest-save.json` isolation from the handoff apply. No PARITY-06 gate is missing; rendered UI and wording correctly stay with PARITY-07.

## Integration risk (review item 7)

- I scanned every local and remote branch for changes to the five production files and `ScenarioOne.asset` relative to its merge-base with `72dbee8`. The only recent writer is this branch. All other hits are stale October 2–18 branches whose divergence predates current main. **None of the active Rack V2, S1-C, S1-D or MFM branches touch these files.** This is consistent with Rack #90's release statement.
- **No new save field or schema bump.** PARITY-06 does not consume a slot in the G2 schema order.
- Merge hazard: `ScenarioOneManager.cs` is a high-traffic file. The six call-site edits are small, single-line substitutions, so a later S1-C/S1-D/Rack rebase should resolve mechanically. The integrator should re-run `run_pruning_check.py` on the integrated tree, keeping in mind that its manager-exactness contract is base-relative and will need the base updated or will intentionally fail.
- Species-less sale records stay valid. The Oak and Beech second-planking and firewood selected prices remain equal, so plan line 147 is still satisfied and no G5 trigger is raised.

## Disposition

- **Production bytes `4fb0d92` can be queued as-is** for the Unity residual queue (G2/G3) and then G4.
- **Required repair before queueing: none.**
- Carry forward: N2 → PARITY-07; N1/N3 → G3 attribution; N4 → G4 / serialized-lane note; N5 → Unity step 2/3 observation.
