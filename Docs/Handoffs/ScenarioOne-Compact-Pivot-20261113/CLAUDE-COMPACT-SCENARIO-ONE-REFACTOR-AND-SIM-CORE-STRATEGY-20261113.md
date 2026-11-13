# Compact Scenario One — targeted refactoring and engine-independent simulation core strategy

**13 November 2026 | Author: Anthropic Claude Opus 5.5 (`claude-opus-5-5`, Claude Code in T3 Code, HP host) | Requested directly by the project owner**
**Related review:** [#459 independent DELTA review and refactoring addendum](https://github.com/jermur788/CCF/blob/docs/compact-one-rack-458-independent-delta-review-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/CLAUDE-ONE-RACK-458-INDEPENDENT-DELTA-REVIEW-20261113.md). This document **supersedes that addendum** by combining it with the engine-independence strategy.
**Subject:** [compact one-rack Scenario One DRAFT (#458 repair)](https://github.com/jermur788/CCF/blob/docs/compact-one-rack-457-review-repair-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/ONE-RACK-457-NEEDS-CHANGES-DRAFT-REPAIR-HANDOFF-20261113.md).
**Evidence base:** read-only static inspection of `main` @ `6c13a9e182774945bf391bc8d4845ce74d3e5e88`. The #443 Objective 1 branch was **not** inspected.

## Status

**RECOMMENDATION ONLY — NOT AUTHORISED.**
- It does not approve the compact DRAFT, change any product decision, release any source/Unity lane or permit refactoring production code.
- Adopting it needs Manager #425 and owner decisions (section 9), then incorporation into the amended DRAFT and the normal claim/lease process under #424.
- No source, Unity, save, package, project-structure, `main` or coordination file was changed to write it.

## Summary recommendation

1. **Keep Unity.** Do not change engines before the compact demo. The friction comes from code structure, not the engine. Moving engines would rewrite most of the project and endanger the protected saves and hashes.
2. **Refactor only what blocks the compact scenario:**
   - geometry lookup;
   - save slot and identity preflight;
   - learning/help key inputs;
   - one small active-profile record.

   Do this once, in shared form that Scenario Two can reuse.
3. **Write the logic of each prerequisite and new compact feature as plain C# that does not reference `UnityEngine`.** Unity scripts then become thin adapters that read the scene, call the plain logic and display the result. Apply this **only to code being written or changed anyway**. No standalone rewrite of working systems.
4. **Give the plain-C# code one shared offline test project.** This generalises the copy-to-temp `dotnet` harness pattern the repo already uses. Fast checks then run in seconds without the Unity Editor or the machine-wide Editor lease.
5. **Defer** large splits (`ForestEcologyController`, `ScenarioOneManager`), generic scenario frameworks, project-wide assembly restructuring and any engine evaluation until the compact demo is playable and accepted.

## 1. Inspected evidence

### 1a. Architecture seams

| System | Fact on `main` | Implication |
| --- | --- | --- |
| Scenario config | `ScenarioOne/ScenarioOneDefinition.cs` is a ScriptableObject with `scenarioId`, `definitionVersion` (`scenario-one-v13`), economy (€12,000 start, €2,500 minimum per contractor visit, rates) and completion thresholds. `Editor/ScenarioOneSetup.cs` wires **one** asset into the scene. | Compact calibration can be a **second asset** (data). The missing piece is choosing the active asset at New Game/Continue. |
| Save slot | `ForestSaveController.cs:9-19` has `const SaveFileName = "forest-save.json"`, with `.tmp`/`.bak` derived. Shared pre-load path: `CanLoad` :240, `TryReadSave` :261, `FindLoadProblem` :296. Future versions load best-effort at :346-347. `ForestSaveData` v24 is one class with nested `ScenarioOneSaveData`. | The slot must become a parameter, with an identity check added to the existing pre-load path. #436 needs the same change for O2. |
| Geometry | `StandGeometryModel` is an `int` referenced from 29 files. Legacy40/Enlarged80 either/or checks: `StandGeometryModel.cs:34,62`, `ForestSaveValidation.Geometry.cs:15`, `ForestStartingStand.cs:83`, `ScenarioOneManager.RackPlan.cs:40,92`, `ScenarioOneObjectives.cs:104`, `ScenarioOneRackLayout.cs:18`, `ScenarioReferenceArchive.cs:337`, `StandWorldBoundary.cs:76`. | Needs explicit lookup with unknown IDs refused (M1). `ScenarioOneRackLayout.AppliesTo` and `RackPlanningV2Active` (59 references) are already Enlarged80-only, so a compact geometry turns off five-rack V2/MFM **with no new conditionals**. |
| Learning/help | `UI/LearningObjectivesView.Curriculum.cs`: `const PreferencePrefix = "CCF.Learning.v1."`, one static `Topics` array (48 steps, five-rack text), explicitly "kept free of UI Toolkit and PlayerPrefs … checked offline on plain .NET". `UI/ScenarioOneTeachingSequence.cs` (136 lines) is a five-lesson stage machine. `MenuHelpView.cs:11` has a constant prefix. | Make the prefix and curriculum inputs. Compact gets its own small sequence. |
| Scenario manager | `ScenarioOneManager.cs` ~141 KB plus 9 partial files, referenced from 40+ files outside `ScenarioOne/`. Shared `ForestEcologyController` finds it via `FindFirstObjectByType` (:44, :307, :485). | High-conflict hotspot. **Do not split now.** |
| Economy | `WorkEconomy/` (6 files) is a separate namespace with **zero** `UnityEngine` references. `ScenarioOneEconomyAdapter` maps the definition onto it. | Already shared and engine-independent. Reuse as is. |
| Species | ScriptableObject species assets; `Species/ScenarioTwo/` assets and `ScenarioTwoSpeciesSiteResponses` already exist. | Shared; leave alone. |
| Reference identity | `ScenarioReferenceArchive.cs:248-272` strips named JSON fields by text pattern. | Any shared-schema change falls under M2/OPEN-C4. A separate compact root avoids the risk. |

No scenario interface, base class or registry exists, and `ScenarioExecutionMode` has one value. There is nothing to unwind, and none should be added.

### 1b. How tied to Unity the simulation is

| Fact | Evidence |
| --- | --- |
| Runtime (non-Editor) C# files under `Assets/ForestPrototype` | 188 |
| …containing `using UnityEngine` | 110 |
| …without it | 78; of these, 5 use fully qualified `UnityEngine.` names (`ForestSaveData`, `ScenarioOneCropTreeCandidates`, `ScenarioOnePrescription`, `ScenarioOnePrescriptionCalibration`, `ScenarioOneTreatmentReview`) |
| MonoBehaviour/ScriptableObject classes | 36 files |
| Already engine-free (examples) | `SitkaGrowthModel`, `SimulationRandom`, `RegenerationModel`, `MixedDensityModel`, `AdultMortalityAccount`, `ForestSiteProfile`, `ForestSaveValidation*`, `ScenarioOneCompletionRules`, `ScenarioManagementHistory`, `CompetitionShare`, `Racks/RackGeometry`, `ScenarioOneRackSampleGeometry`, `ScenarioOneCoverage`, `ScenarioOnePrescription*` (one fully qualified reference), all of `WorkEconomy/` |
| Strongly Unity-bound | `ForestEcologyController` (~106 KB MonoBehaviour; ~89 `Mathf`/`Vector`/`Transform`/`GameObject`/`FindFirstObjectByType` uses), `ForestStartingStand` (`UnityEngine`, scene lookups), `ScenarioOneManager`, `StandGeometryModel` (6 Unity references, e.g. `Mathf`) |
| Assembly definitions (`.asmdef`) | **None.** Everything compiles into Unity's default assembly. |
| Unity Test Framework | `com.unity.test-framework` 1.8.0 is in `Packages/manifest.json`, but there is **no** `Assets/**/Tests` folder |
| Existing offline test pattern | e.g. `Tools/Verification/MfmSaveProvenance/README.md`: "No Unity required; needs `dotnet` (SDK 8+)". A Python script copies selected C# sources into a temporary `dotnet` project under `/tmp`, reproduces baseline hashes byte for byte and runs 23 deliberate-defect mutation controls. The README states its limit: it does **not** prove Unity import/compile, real `ForestSaveController` load order or `JsonUtility` byte layout (a separate `MfmSaveProvenanceUnity` harness covers that). No `.csproj` is committed; each harness keeps its own file list. |

**Reading of the evidence:** about 40% of runtime files are already engine-free, and the team has a working, proven offline harness pattern. The gap is that it is ad hoc: each feature has its own copied file list, and nothing shared covers the simulation core. The heaviest code (annual ecology tick, stand generation, scenario manager) is still Unity-bound.

## 2. Why move logic into plain C#: benefits

**Testing cost and speed (main benefit).**
- Fast checks (AgentWorkflow Tier 1) for pure logic run in seconds with `dotnet`. There is no Editor start, no asset import and no Library rebuild.
- They need **no machine-wide Unity Editor lease**. Several workers, including code-only and cloud sessions, can run them in parallel, instead of queuing behind Objective 1 for the one Editor.
- Mutation controls (deliberately broken copies that the tests must catch) are cheap, as `MfmSaveProvenance` already shows. Tests can then be trusted to catch real defects.
- Fewer focused Unity (Tier 2) runs are needed. They only have to cover what truly needs Unity (section 5).

**Earlier, cheaper feasibility evidence.** The C0-03/C0-04 study needs to generate candidate compact stands and cash traces. A plain-C# stand planner and the already-pure `WorkEconomy` let it run headless (without graphics) and in batches. This resolves review item **E-1** (Unity Editor or managed lane) in favour of a managed lane for most of the study.

**Protecting old saves and hashes.** Logic separated from the scene can be compared byte for byte against stored baselines (as the MFM-06 harness does) every time a shared file changes. This makes "no rebaselining" cheap to enforce rather than an expensive Unity rerun.

**Long simulations.** 25- and 100-year runs, parity oracles and the D-077 skip comparisons are far cheaper headless.

**Research calibration.** External model comparisons (for example the #456 Samsara2/CAPSIS spike) can drive the plain-C# rules directly in offline experiments, never as a runtime dependency.

**Maintainability for a learner.**
- Rules ("what happens to a tree this year", "does this save belong to this world") read as ordinary C# without scene plumbing.
- Smaller files mean fewer merge conflicts between lanes.

**Scenario Two and future scenarios** reuse the same tested core instead of copying Scenario One's scene-bound code.

**Future engine flexibility.** If an engine or platform change is ever justified, the plain-C# core moves with you and only presentation and input need rebuilding. This is a side benefit, not a reason to start.

## 3. Guardrails (so this does not become a rewrite)

1. **Move logic only when you touch it.** Pure-C# extraction happens only inside a task that already has to change that code. There is no standalone "extract the ecology engine" task now.
2. **Behaviour must stay byte-identical** for every protected class: Legacy40, Enlarged80, v1–v24 saves, Reference Future v1, pure-Sitka model-0 anchors, five-rack/MFM tests. Proof is a before/after comparison. **No rebaselining, no weakened tests.**
3. **Do not swap Unity maths types casually.** Replacing `Mathf` (float) with `System.Math` (double), or changing operation order, can change results and drift hashes. When moving code, keep identical float arithmetic. Where `Mathf` is unavoidable, use a tiny local equivalent proven identical by tests. Offline .NET results are evidence; final parity on Unity's own runtime is still confirmed at the focused Unity check.
4. **Keep Unity `JsonUtility` as the save serializer.** Offline tests can check save *decisions* (identity, version, classification, graph validity). Byte layout and load order still need the existing Unity harness pattern.
5. **No new packages, no Git LFS, no CI service** without approval. `.asmdef` files are a built-in Unity feature, not a package, but they change project structure and compile order. They are a **decision** (section 9), not a silent change.
6. **No frameworks.** No dependency injection, service locator, plugin registry or generic `IScenario` engine. Use plain data (definition assets, a fixed slot table, one profile record) plus plain static or instance classes.
7. **Thin adapters.** A MonoBehaviour gathers scene data, calls the plain class and applies or displays the result. The plain class must never call `FindFirstObjectByType`, `PlayerPrefs`, `Debug` or file I/O. Pass in values or simple delegates instead.
8. **Profile checks only at entry points:** New Game/Continue/load, lesson selection, completion predicate and starting-stand selection. A compact `if` inside shared ecology, economy or history code is a STOP-and-report signal.

## 4. What stays shared vs scenario-specific

- **Shared, one implementation (plain C# where it already is or becomes so on touch):**
  - annual ecology and species engine;
  - `WorkEconomy` and Work Plan resolution;
  - work orders, management events/history and Annual Review (D-073 L1–L4);
  - `TreeMarkType` marking;
  - save validation graph (`FindLoadProblem`/`ForestSaveValidation*`);
  - map/minimap rendering;
  - one-year advance and the approved D-077 skip.
- **Scenario-specific, as data where possible:**
  - definition asset;
  - slot name and world/geometry/content IDs;
  - starting-stand planner;
  - curriculum, learning/help prefix and teaching sequence;
  - completion/classifier policy (a new compact class; `HasReviewedTwoCycleSequence` untouched);
  - access lesson (five-rack legacy vs one-rack compact);
  - MFM (legacy only).
- **Scenario Two** reuses the slot table, geometry lookup, prefix/curriculum inputs and offline test project. Nothing O2-specific is built now beyond #436's proposed SI-03 slot entry.

## 5. Which tests become cheap, and which still need Unity

| Moves to fast offline checks (`dotnet`, no Editor) | Still needs Unity (focused Tier 2, or Tier 3 once) |
| --- | --- |
| Geometry lookup, refusal of unknown IDs, cell counts | Scene/prefab wiring, asset import, URP materials |
| Save identity/version/classification decision table; two-way wrong-world matrix logic; slot-name mapping | `JsonUtility` byte layout; real `ForestSaveController` load order against a live scene; actual disk `.tmp`/`.bak` behaviour |
| Learning/help key building; "legacy keys cannot complete compact steps" | Real `PlayerPrefs` read/write on device |
| Compact stand planner: positions, IDs, species, counts, BA, two-sided parcel, corridor geometry | Walking/collision, physical reachability, 3D vs map agreement |
| Residual BA / opening-set membership and geometry; Work Plan quote arithmetic via `WorkEconomy` | Rendering, legibility at 1280×720/1920×1080, colour-independent cues |
| Compact completion/substantive classifier; teaching-sequence state machine | Input System focus and modal ownership |
| Multi-year runs and parity oracle A **for logic already engine-free**; hash comparisons vs baselines | Ecology tick paths still inside `ForestEcologyController`, until extracted on touch; frame time, draw calls, memory |

**Retest rule:**
- Any task touching the foundation files reruns the offline legacy suite: seconds to minutes, no lease.
- Each feature runs its own offline checks plus one focused Unity check.
- **Tier 3 runs once** at the pinned pre-demo candidate (F-01), as the DRAFT already requires.

## 6. Prioritised plan with task boundaries

**P0 — decisions and documents (now; no code).**
- **D-1:** Manager/owner decide whether to adopt this direction and fold it into the amended DRAFT: plan Stage A–D stop rules and a new foundation task.
- **D-2:** choose the home of the shared offline test project. Section 9 explains why the recommended option is the Tools project.

**P1 — foundation (after the S-21 gates and #424 release; before compact content).**

| ID | Scope | Pure-C# part | Unity part | Depends on | Lane | Gate |
| --- | --- | --- | --- | --- | --- | --- |
| **T-0** Shared offline test project | One committed test project under `Tools/Verification/` (e.g. `SimCore/`). It references **one maintained list** of engine-free production sources and generalises the copy-to-temp pattern. It hosts the A-02 legacy/Reference/pure-Sitka baselines. No production code changes. | All | None | D-2 decision | Any capable worker; no Editor lease | Builds with `dotnet`; reproduces existing baseline hashes; mutation controls fail as expected |
| **F-1** Geometry lookup | Replace the 10 either/or sites with an explicit known-ID lookup that refuses unknown IDs. Keep Legacy40/Enlarged80 results and `Label` strings identical. Leave `RackLayout.AppliesTo`/`RackPlanningV2Active` Enlarged80-only. | Lookup table and refusal logic (no `Mathf`, or proven-identical arithmetic) | Call sites updated | T-0; #443 handoff landed (shared files `ScenarioOneManager.RackPlan.cs`, `ScenarioOneObjectives.cs`) | One writer, local; short Editor lease for the compile/focused check only | Offline negatives plus before/after protected hashes identical; focused Unity load of Legacy40 and Enlarged80 |
| **F-2** Slot table, identity preflight, active-profile record | Replace the `SaveFileName` constant with a fixed slot table (legacy O1 / compact / O2). A plain `SaveIdentityCheck`-style class decides accept/refuse from the parsed data. `TryReadSave`/`FindLoadProblem`/`CanLoad` call it **before any mutation**. One small serializable profile record (definition asset, slot, geometry ID, curriculum key), chosen at the title menu and passed by reference, never a singleton. | Decision table, slot mapping, profile data | `ForestSaveController` wiring, title-menu selection, `JsonUtility` layout check | F-1; **single router owner decision with #436** (do it once) | The one approved router writer | R-013–R-018 and R-084–R-086 two-way zero-checksum negatives offline; focused Unity load-order/byte-layout check; review item E-2 ordering honoured |
| **F-3** Learning/help inputs | The prefix and curriculum become inputs supplied by the profile. A pure key builder. The `PlayerPrefs` call stays in the view. Legacy `CCF.Learning.v1.`/`CCF.MenuHelp.v1.` exact. | Key builder; "legacy keys cannot complete compact" check | View wiring; real `PlayerPrefs` | F-2 (same claim recommended) | Same router writer | R-087–R-089 offline; one focused device `PlayerPrefs` check |

**P2 — inside each feature task (extract logic only for code the feature touches).**

| Feature task | Plain-C# piece | Thin Unity adapter |
| --- | --- | --- |
| **C0-03/C0-04 study** (before approval; disposable) | Disposable stand-planner and cash-trace code under `Tools/Verification/` only, using T-0's project if it exists, and `WorkEconomy` as is. **No production edits.** | None for most measurements. A short leased Unity check only if a measurement truly needs the scene. |
| **B-01** compact stand and geometry | Production compact stand **planner** (seed + geometry → tree positions/IDs/species/BA), informed by, but not copied blindly from, the study code | A `ForestStartingStand` branch that spawns the planner's output. The Enlarged80 generator is untouched. |
| **B-02** compact Objective 1 / teaching | Separate compact teaching sequence and curriculum data, following the existing offline-checkable curriculum style | Reuse lesson views via F-3 inputs; no branches in `ScenarioOneTeachingSequence` |
| **C-01** marks and Work Plan | Pending/Approved/OPEN-C8 edit-policy rules as pure functions over orders | Existing views call them |
| **C-02/C-03** residual BA and opening | Pure calculator: current trees minus one named removal set; crown-opening geometry | `WorkPlanView` and `StandMapCanopyRaster` read the results; no view rewrite |
| **D-02** compact economy | Values in the second definition asset; `WorkEconomy` unchanged | Existing adapter |
| **D-03** substantive classifier and completion | New pure class beside `ScenarioOneCompletionRules`; old predicate untouched | Manager calls it at the completion entry point |
| **D-04** D-077 skip | Parity oracle A logic headless where already engine-free | UI acknowledgement path (oracle B) in Unity |
| Any task forced to edit `ForestEcologyController` | Move **only** the touched rule into a pure helper with byte-identical arithmetic | The controller keeps orchestration |

**P3 — defer until the compact demo is playable and accepted.**
- Splitting `ForestEcologyController` into a plain engine plus adapter.
- Splitting or renaming `ScenarioOneManager` and the `ScenarioOne*` types.
- Removing `FindFirstObjectByType<ScenarioOneManager>` coupling from ecology.
- Generic multi-scenario engine.
- Restructuring `ForestSaveData`, unless the save owner chooses the separate-root option (then it belongs to F-2).
- Generalising MFM to N racks (OPEN-1–6).
- Scene split.
- Project-wide `.asmdef` restructure.
- Continuous integration service.
- **Any engine evaluation** (section 8).

## 7. Sequencing to minimise duplication, conflicts and repeated testing

1. **Existing gates are unchanged:** #459 delta review → separately claimed C0-03/C0-04 study → amended DRAFT plus bounded delta review → owner/save decisions with **one** router owner → Manager APPROVED → #424 release.
2. **T-0 can be prepared early.** It touches only `Tools/Verification/`, and the study can be its first user (if D-2 approves). This is the only item that may precede spec approval, because it changes no production code. It still needs its own claim and #424's lane check.
3. **F-1**, after the #443 handoff lands (it shares files with that lane).
4. **F-2 + F-3** as one claim by the single router owner, combined with the #436 amendment, so slot work is never done twice.
5. **A-02** legacy baselines live in T-0 and become the permanent fast regression suite.
6. **B → C → D** features. C (Work Plan/map files) and D (completion/economy files) may run in parallel **only** with disjoint file allowlists. `ScenarioOneManager*`, `ForestSaveController` and `ForestEcologyController` stay single-writer hotspots.
7. **Tier 3 once** at F-01. Offline suites run continuously; focused Unity checks run per feature; the Editor lease is held only for those focused checks.

## 8. Engine choice

**Recommendation: keep Unity (6000.6.0f1, URP, New Input System) for the compact demo and the foreseeable roadmap.** Reasons:
- the size of the working investment (section 1);
- protected save/hash continuity that a port would almost certainly break;
- timing (the demo pivot is underway);
- the observed friction is structural and would follow the project to any engine.

Revisit only after the compact demo, and only for a concrete trigger, such as:
- Unity licensing or terms becoming a real problem (check the then-current terms);
- a required platform Unity serves poorly;
- the long-term maintainer's strong preference.

Any later comparison (for example Godot with C#, after checking its C# export support for the target platforms at that time) is far cheaper once the plain-C# core from this plan exists.

Separately, **replacing the forest model** (as opposed to the engine) is a different question. The #456 Samsara2/CAPSIS spike informs it. External models should serve as **offline calibration references**, not runtime replacements, for the same save/hash reasons.

## 9. Decisions needed (not taken here)

| ID | Decision | Owner | Recommendation (not binding) |
| --- | --- | --- | --- |
| D-1 | Adopt "targeted refactor + plain-C# logic on touch" as a stated architecture rule for compact implementation | Manager #425 / product owner | Adopt; add it to the amended DRAFT and plan stop rules |
| D-2 | Home of the shared offline tests: (a) committed `dotnet` test project under `Tools/Verification/` referencing a maintained source list, or (b) `.asmdef` assemblies plus Unity Test Framework EditMode tests | Manager + project owner (project-structure change) | **(a) first.** It changes no Unity project structure, matches the proven MFM-06 pattern and runs without the Editor. Consider (b) after the demo, as a deliberate project-structure decision. |
| D-3 | May T-0 be claimed before spec approval so the C0-03/C0-04 study can use it? | Manager #425, with #424 lane check | Yes: verification-only, no production writes |
| D-4 | Router ownership (#436 amended vs compact owns O1) | Manager/save owner (OPEN-C4) | Unchanged from review; required before F-2 |

## 10. Risks and mitigations

| Risk | Mitigation |
| --- | --- |
| Hash/behaviour drift while moving logic (float maths, operation order, `Mathf` swaps) | Byte-identical before/after baselines in T-0 for every protected class; no rebaseline; focused Unity confirmation on the Unity runtime |
| Offline .NET result differs from Unity runtime in an edge case | Treat offline as fast evidence, not final proof; keep one focused Unity check per foundation/feature and Tier 3 once |
| "Extract on touch" creeps into a rewrite | Guardrail 1; each task's allowlist names the exact files; extraction outside the feature's need is out of scope |
| Profile record becomes a service locator | Plain data record passed by explicit reference; no static access |
| Conflicts with #443 Objective 1 lane | F-1 waits for #443 handoff; #424 controls leases; hotspot files single-writer |
| Slot work duplicated across compact and #436 | Single router decision (D-4) before F-2 |
| Offline test list goes stale as files move | One maintained source list in T-0; build fails if a listed file is missing |

## Protections (non-negotiable, restated)

Preserve:
- Legacy40 and Enlarged80 saves, all v1–v24 history;
- frozen Reference Future v1;
- pure-Sitka model-0 anchors;
- five-rack Rack V2 and MFM functionality and tests;
- the annual three-species Sitka/Oak/Beech simulation;
- economy, Work Plan and history behaviour;
- the v24 latch;
- approved specs (D-073 V2, D-077 cadence);
- the Windows-build gate.

**No hash rebaselining, no weakened or deleted tests, no package/LFS/CI additions without approval.**

## Evidence limits

- **Static inspection only**, at `main` `6c13a9e1`, through read-only `git show`/`git grep`/`git ls-tree` on fetched refs.
- **Not run:** no Unity, compilation, `dotnet` build or tests.
- **Approximations:** file counts use a `using UnityEngine` text search and are approximate. Fully qualified references were checked only for files without the `using` line.
- **Not inspected:** the #443 branch (may change F-1/F-2 files); the full contents of `ForestEcologyController`.
- **Not measured:** time savings, which are qualitative. The first T-0 run should record actual offline vs Unity check durations.
- Dashboard/Drive mirrors **NOT VERIFIED**.

**Requested next action:** Manager #425 to consider D-1–D-3 when planning the C0-03/C0-04 study packet and the amended DRAFT. **User action now:** none required. D-1 and D-2 are the user-visible decisions when the Manager brings them forward.
