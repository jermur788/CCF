# PARITY-06 — Broadleaf timber/economy/pruning gap closure

**Exit token:** `BROADLEAF TIMBER ECONOMY AND PRUNING QUALIFIED`

**Scope of this result: bounded static/offline implementation qualification only. NOT UNITY-VERIFIED.** The token does not grant G2 acceptance, G3 acceptance of world-anchor movement, G4 integration, whole-parity completion or external-demo readiness. Unity execution was explicitly excluded from this dispatch; the residual queue below remains mandatory.

## Identity and authority

- Task: PARITY-06, umbrella [#91](https://github.com/jermur788/CCF/issues/91), execution claim [#92](https://github.com/jermur788/CCF/issues/92), left open for integrator review.
- Actual executor: **OpenAI `openai/gpt-6.1-sol`, OpenCode harness/provider instance `opencode`, in T3 Code**. Sole implementation owner; no subagents or independent reviewers spawned. The claim's dispatch description is not a substitute for this actual executor record.
- Worktree: `/media/jer/Videos/CCF-Parity06` only.
- Branch: `feature/parity-06-broadleaf-timber-pruning-v1`.
- Base: `72dbee8b8eab66f58046abfd49e77ead4c29d357`.
- **Exact production + verification candidate: `4fb0d925afed5a2fd50d7d7973c0c9ce0384687b`.** This handoff is published in a subsequent documentation-only commit on the same branch; its exact tip is reported on claim #92 and to the parent. It does not change the tested code.
- Context snapshot: `63e95ba2175f7e239a278c616681b286719821f8` (latest context-source commit in the base). Live coordination was fetched separately through GitHub.
- Approved authority: `SPEC-scenario-one-three-species-parity-p0-V1`, especially FR-6/7/8/10/11/12, plan timber/economy/pruning strategy, task PARITY-06, evidence closure Q-10/Q-11/Q-12. PARITY-04 accepted Unity evidence was read; accepted calibration was retained.

### Ownership preflight and release

The assigned branch/base were exact and the worktree was clean, with no unrelated changes. Live `active-tasks.md`, claims #90/#91/#92 and the Rack packet were inspected before writing. The live file still described Rack as WAITING, while claim #90 documented its subsequent release and active execution; this discrepancy was surfaced to #91.

The worker initially held `ForestTree.cs` and `ScenarioOneManager.cs` edits. The umbrella integrator first released non-overlapping timber/economy/verification work, then explicitly released **CanPrune/TryPrune only**, **manager pruning call sites only**, and the definition's pruning/flat-rate surface after Rack confirmed it did not claim those files. All edits respect that disposition. Rack's marking, RackPlan partial, player, HUD, controls and verification files were not touched. No Unity lock was acquired and no Unity process was launched.

## Production diff inventory and policy mapping

All paths below are relative to `Assets/ForestPrototype/`.

| File | Actual change / policy |
|---|---|
| `ForestTree.cs` | Only `CanPrune` / `TryPrune` changed. Added an explicit optional species-aware gate, defaulting to legacy for existing two-argument callers. Model-2 Oak/Beech enforce targets 3.5 m then 6.0 m, two-lift cap and minimum three-year recovery. Sitka and models 0/1 use the old constants/conditions/results. Existing radius reduction and crown-target arithmetic are unchanged. |
| `ScenarioOne/ScenarioOneDefinition.cs` | Added a tree/species/model-aware target overload; retained the original configured Sitka/legacy target method unchanged. Removed the runtime serialized flat broadleaf field and made the dormant flat-value API reject every non-Sitka species. Preserved its Sitka result. |
| `ScenarioOne/ScenarioOneManager.cs` | Only pruning target/eligibility/designation/validation/resolution call sites now pass the existing model-2 `BroadleafMarketActive` gate. No wallet, approval, event/history, rack, ecology or UI flow changed. |
| `TimberYield/TimberYieldConfiguration.cs` | Evidence/provenance only: Teagasc Oak veneer/beam dimensions now `ManagementGuidance`, selected lengths/guards/pruning-only proxy clearly calibration. All ladder numeric values, priorities, flags and allocation rules retained. |
| `WorkEconomy/Stage1EconomyDefaults.cs` | Evidence/provenance only: five non-firewood Oak/Beech quotations now `ManagementGuidance`; selected references/roadside interpretation explicitly `GAMEPLAY CALIBRATION`. No numeric price, density proxy, rounding or harvest cost changed. |

### Implemented evidence boundaries

- **DIRECT / runtime `ManagementGuidance`:** Teagasc typical hardwood quotations; Oak >2.7 m / 45 cm mid-log and >2.5 m / 30 cm mid-log guidance. These are practitioner guidance, not a guaranteed tariff or measured individual-tree quality model.
- **GAMEPLAY CALIBRATION / runtime `GameplayCalibration`:** selected prices, firewood outlet/range, roadside basis, selected discrete lengths and engine guards, pruning-only veneer proxy, three-year recovery, 8% crown-radius loss and existing 60% height cap. High-pruning two-lift heights follow the accepted Teagasc guidance; the selected interval is calibration within its guidance, not formative shaping.
- **MODEL DERIVATION:** deterministic geometry/allocation and the price-unit conversion. The inherited 890 kg/m³ is a work-equivalent proxy only, never a measured Oak/Beech green density. Price conversion still rounds to whole euro/tonne; €230/m³ corresponds to approximately €229.62/m³ after conversion rounding.
- Oak remains **veneer/planking → beam → fencing → second planking → firewood**. Beech remains **second planking → firewood only**. Neither uses Sitka boundaries/prices or a species price/cost multiplier.
- Live living-tree quality inputs remain size and recorded pruning/crown history. Existing generic allocator defect flags and synthetic robustness tests are not newly implemented fork/form biology. No persisted fork, defect, browse-form, shaping, price-deck or other state was added.
- No premium-Oak size/growth target was changed or guaranteed. Large synthetic stems exercise eligibility; they are not a promise that a player tree will reach it.

### Compatibility details

Growth models 0/1 retain their prior pruning and market behaviour. Sitka retains three configured lifts at 2.5/5/6.5 m and five-year recovery even with the model-2 gate enabled. The direct tree API's default remains legacy; the production Scenario One manager supplies the explicit gate consistently at all pruning consumers.

Existing model-2 history is not rewritten: a one-lift tree proceeds to the second 6 m lift, whereas two- or three-lift trees cannot acquire another lift. Old completed counts, crown bases and years are retained. A pending model-2 broadleaf order with an obsolete 2.5/5 m target is rejected by ordinary validation rather than silently retargeted or charged; no migration was introduced. This is a deliberate effect of enforcing the approved schedule and should be inspected in the queued Unity gate.

`ScenarioOne.asset` still contains its historical serialized flat-value key, **but no runtime field consumes it and the API throws for non-Sitka**. This blocks the prohibited value without taking the serialized lane or changing asset bytes. The API has no production caller beyond its declaration in the inspected base. There is no fallback to a different invented broadleaf value.

## Verification files

- Updated `Tools/Verification/ThreeSpecies/OfflineLogic/BroadleafTimberOfflineCheck.cs`: evidence classification assertions, actual 3.5/6 m pruning-section fixtures, explicit distinction between synthetic allocator flags and live form state.
- Added `Tools/Verification/ThreeSpecies/OfflineLogic/run_pruning_check.py`: compiles the exact production pruning block/restore method in fixture wrappers, the exact base block alongside it, the unchanged complete production definition/economy adapter/yield/work modules, preservation contracts and eight negative controls. Output is required outside the repository. Mutations affect generated copies only.
- Added `Tools/Verification/ThreeSpecies/OfflineLogic/BroadleafPruningOfflineCheck.cs` and `PruningOfflineStubs.cs`: executable fixtures and explicitly bounded Unity/collaborator stubs. Visual hooks are inert; salvage throws if accidentally entered. This is not a fake Unity runtime.
- Strengthened `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs` for the later real-Unity queue: both legacy growth models, manager/batch targets and recovery timing, two-lift cap, history save/load, real high-pruning before grading, pricing/settlement and new-product report/save checks. **This strengthened Editor harness has not been run or separately Editor-compiled here.**

## Checks actually run

Evidence root: `/media/jer/Files/CCF/Verification/Parity06-Offline-20261029/`.

| Check | Result |
|---|---|
| Base timber suite, before edits | `THREE_SPECIES_TIMBER_PASS checks=71` |
| Candidate timber suite | **`THREE_SPECIES_TIMBER_PASS checks=73`** |
| Full runtime-source external type check against existing L01 Player managed assemblies | **173 sources, 116 references, exit 0, errors 0**. Syntax/type evidence only, not Unity import/compile. |
| Candidate pruning/adapter executable | **`PARITY06_PRUNING_PASS checks=3946 failures=0 NOT_UNITY_VERIFIED`** |
| Differential legacy pruning | **1,296 cases** against the exact base method bodies: Sitka gate off/on, Oak/Beech gate off, and unknown-species no broadleaf inheritance; rejection text, crown state, lift/year state and crown-target factor exact |
| Negative controls | **8/8 killed**, all compiled successfully: two-/four-year interval, third lift, missing species gate, missing model gate, wrong target, changed crown loss and changed height cap |
| Source/preservation contracts | **17 protected files SHA-256 exact against base**, original CreateSitka / Sitka price-book factory / legacy target method unchanged; manager diff exactly approved call-site substitutions; ForestTree unchanged outside CanPrune/TryPrune |
| Git whitespace | `git diff --check` PASS |

The first negative-control pass exposed that testing a third lift after an already-complete 6 m crown also triggered the target/height checks, masking the cap mutation. The fixture was strengthened with an existing **two-lift / 5 m** model-2 history; the cap mutation is now detected. No production rule or pass threshold was weakened. An initial runner path typo for ScenarioOne.asset was corrected before the complete passing run.

### Observed synthetic economy measurement

Through the real production Scenario One economy adapter, a 60 cm DBH / 24 m Oak earned **46,845 cents unpruned**, **58,388 cents after the two high lifts**. The three-species job cost remained **250,000 cents** with or without the market. Products, work proxy, volume conservation, market opt-in, Beech restrictions, Sitka revenue and re-resolution/ledger balance were checked. This is a bounded large-stem fixture, not a Scenario One completion/solvency witness or a rebaselined world anchor.

### Reproduce offline

Run in the assigned checkout (or exact byte-equivalent descendant), with output on the durable Files tree:

```bash
python3 -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite timber --out /media/jer/Files/CCF/Verification/Parity06-Offline-20261029/candidate
python3 -B Tools/Verification/ThreeSpecies/OfflineLogic/run_pruning_check.py --out /media/jer/Files/CCF/Verification/Parity06-Offline-20261029/pruning --mutations
python3 -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src /media/jer/Videos/CCF-Parity06 --out /media/jer/Files/CCF/Verification/Parity06-Offline-20261029/runtime-typecheck --managed /media/jer/Files/CCF/Builds/S1B/CCF-S1-20261019-L01-8f9ed80-SMOKE-Linux-x64/CCF_Data/Managed
git diff --check
```

The external pruning evidence includes `results.json`, `protected-sha256.json`, candidate build/run logs and a separate build/run log for each killed mutant. The older timber runner prints its results to stdout; its generated projects/binaries are in `baseline/` and `candidate/`. The exact commands and terminal counts above are the durable summary; no raw Unity evidence is implied.

## Protected hash status

- Sitka 3,000-stem / 11,592-log fingerprint: **`CAA0388524E0D0E5`**, exact against the inherited base fixture.
- Sitka 400-stem operation fingerprint: **`2976F3575E0446B3`**, exact; sold **169,245,486 cm³**, residual **130,542,728 cm³**, **12** assortment summaries, exact.
- Frozen archive SHA-256: **`789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc`**, exact.
- Frozen schedule SHA-256: **`91cf50595e9b3337438323ef09609a70d7de11374569dd9fb3c6359fbdb798f0`**, exact.
- SitkaGrowthModel SHA-256: **`291601041876d83451259990f82219c6ccd3fb704ba55e6dcb8189f13b446463`**, exact. Full density/controller/species/save/adapter/data manifest: external `pruning/protected-sha256.json` (17/17).
- **Unity lifecycle, density-controller, Legacy40 world and Reference load/continuation fingerprints were NOT rerun.** Static source protection and offline fingerprints do not substitute for them. No expected world hash, reference byte or anchor was changed/rebaselined.
- No broadleaf-containing model-2 world/completion/economy anchor was measured or accepted here. Any later movement must be attributed under G3 and explicitly accepted by the Manager; protected pure-Sitka/Legacy/archive anchors must not be rebaselined.

## Unity-only residual queue and expected observations

After #91 releases an exact candidate and a Unity lane, with the normal storage preflight and machine lock:

1. Import/compile on **Unity 6000.6.0f1**, including the strengthened disposable harness; no candidate-caused compile/import errors.
2. Run `BroadleafTimberEconomyVerification.Begin` in both explicit Legacy40 and Enlarged80 configurations. Expected: terminal `BROADLEAF_TIMBER_VERIFY_PASS`, two manager/batch high-pruning targets 3.5/6 m for model-2 Oak/Beech, first lift at year 1 and second no earlier than year 4, no third lift, exact saved history/crown bases, generic costs, species-explicit products and identical Sitka prices/quotes. Models 0/1 keep prior targets/recovery and no broadleaf sale market.
3. Inspect obsolete pending model-2 broadleaf target rejection and restore existing one-/two-/three-lift histories without mutation; verify ordinary approval/resolution spends nothing for an ineligible order. Confirm real crown/visual refresh and subsequent ecology relaxation use the recorded two-lift state. Offline visual hooks do not prove this.
4. Run the existing focused pruning/Sitka compatibility and protected legacy/reference gates under their original configurations. Measure a model-2 completion/economy/solvency witness on the combined candidate; report any broadleaf-containing movement for G3/Manager acceptance. Do not change protected expected hashes.

Suggested runner (only **after** queue release; not executed by this worker):

```bash
CCF_TEST_DIRECT_ENTRY=1 CCF_STAND_GEOMETRY=0 python3 -B Tools/Verification/ThreeSpecies/run_unity_gates.py --unity <Unity-6000.6.0f1> --gate BroadleafTimberEconomyVerification --out <FILES_EVIDENCE>/legacy40
CCF_TEST_DIRECT_ENTRY=1 CCF_STAND_GEOMETRY=1 python3 -B Tools/Verification/ThreeSpecies/run_unity_gates.py --unity <Unity-6000.6.0f1> --gate BroadleafTimberEconomyVerification --out <FILES_EVIDENCE>/enlarged80
```

Inspect runner side effects before use, isolate preferences/save paths and apply the documented copy/run/remove workflow. No temporary harness or generated `.meta` is present in Assets from this task. PARITY-07 owns wording/rendered inspection/Work Plan truth; this task does not claim those gates.

## Preservation, uncertainties and disposition

- No change to density, growth, species biology outside pruning, Sitka parameters, species assets, scenes/prefabs/materials/meta, frozen resources, packages/ProjectSettings, save schema/fields, Rack, S1-C/S1-D, MFM, or player-facing UI files. No asset deletion, Unity launch, lock acquisition or Windows build.
- No other worktree or user save was changed. Initially there was no dirty/user work in this checkout. All verification artifacts are outside Assets and outside the repository on the Files storage tree.
- Accepted limitations remain: no living-tree fork/defect state; no measured broadleaf density; generic harvest costs; coarse calibrated roadside references; no time-varying market or premium-Oak guarantee. Existing model-2 pending orders can be newly ineligible under the approved corrected schedule, as described above.
- **Blocking design/ownership issue: none after explicit #91 release.** Remaining execution gates: Unity-only checks above, independent applicable G2/G3 review and G4 integration. Ready for static review and queueing; not integrated or finally accepted.
- `DASHBOARD SYNC: NOT AVAILABLE`. The worker updates claim #92 with this result; Manager-owned live coordination/dashboard ingestion and published-state readback remain outstanding. No generated dashboard state or canonical coordination/spec status was edited.
