# CCF #858 — independent nonauthor blank-starter design-quality review

Date: 20 November 2026. Requested model: **GPT-6.1 Sol**. Selected/executed model: **gpt-6.1-sol**, provider **OpenAI**, Codex harness through T3 Code on jer-laptop. Session/thread ID: UNKNOWN (not supplied); agent identity `/root`. This is a distinct review session from the OpenAI ChatGPT GPT-6 browser/docs author. I did not author or amend the reviewed documents and have no implementation role. No child agents were used.

**Verdict: NEEDS CHANGES — DOCUMENT QUALITY ONLY.**

This verdict is not implementation authorization. It confers **no G4/G5 approval, no source/Unity grant and no merge right**. The specification remains DRAFT / NOT APPROVED. There are two blocking and three non-blocking findings. Bounded document corrections are needed; no replacement architecture is requested.

## Exact reviewed bytes

Commit existence/tree verified through fetched Git objects: `4ea5a10db061b9fe69601118649bb77b698877cf`, author branch `docs/ccf-blank-starter-856-gpt6-g0-20261120-v1`. Only these pinned bytes were reviewed. Prefix S below means `specs/ccf-blank-scenario-starter-v1/`; H means `Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/`.

| File | Verified Git blob SHA |
|---|---|
| S/spec.md | `73c7c7779df980e37d414e6f18c66b5932dfc9c3` |
| S/plan.md | `59901649cdc59926b78349b9af370ce0e8fc312c` |
| S/tasks.md | `a29609055756e4ed7657dae22251e588895bcfff` |
| S/checklists/requirements.md | `e98eec259af90f4a03319144d4136156da2c5128` |
| H/Dashboard-CCF-BLANK-STARTER-G0-FOUR-FILE-SPEC-AUTHOR-RESULT-20261120.md | `337240cdba50eada58e3c1614c0bb66e19d42b69` |

Author base-to-candidate diff is exactly five added Markdown files. Freshly fetched origin/main and reviewer branch base: `cbcb3850c3952a8d74e54606279165b2f4be9add`. Source claims were checked at the author's actual baseline `7758e17c882fad01970d2f2fb8c09586bbcab5c1`, not inferred from main or Compact branches.

Review branch: `review/ccf-858-blank-starter-spec-review-20261120-v1`. Isolated worktree: `/media/jer/ZX20/CCF-858-SpecReview-20261120`, initially clean at main. Supplied worktree was clean and preserved. Backing devices checked: ZX20 exFAT `/dev/sdc2`, Files ext4 `/dev/sdb4`. No Unity cache or evidence archive was created.

## Context read

- [Binding parent dispatch](https://github.com/jermur788/CCF/blob/Dashboard-CCF-Blank-Starter-End-To-End-Orchestrator-Dispatch-20261120-v1/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-BLANK-STARTER-END-TO-END-ORCHESTRATOR-AFTER-SPEC-DISPATCH-20261120.md), including O0–O5 and historical protections.
- Issue bodies/comments: [#856](https://github.com/jermur788/CCF/issues/856), [#857](https://github.com/jermur788/CCF/issues/857), [#858](https://github.com/jermur788/CCF/issues/858), including author registration/final handoff and parent registration. [Reviewer registration](https://github.com/jermur788/CCF/issues/858#issuecomment-7029616073). Direct user packet controls the exact output filename.
- Main `cbcb3850c3952a8d74e54606279165b2f4be9add`: AgentWorkflow, Decision Log, Current Milestone, Game Brief, active-tasks and specs/README. Relevant rules include current routing/independence, DRAFT authority, historical reference protection, Compact scope/rights distinctions, D-091 and nontransferable D-092. Unity Project Overview was also consulted as supporting context; inspected code controls implementation facts.
- All five pinned candidate documents and every required baseline source file listed below. This is static source inspection, never native qualification.

## Findings and required corrections

All document locations are at the reviewed commit; S is expanded above. BLOCKING means correct before document-quality approval. NON-BLOCKING clarifications do not invalidate the central design.

| ID | Severity | File + line | Evidence and impact | Exact required correction |
|---|---|---|---|---|
| F-01 | **BLOCKING** | S/spec.md:62,83; S/checklists/requirements.md:15,34; S/tasks.md:29,33 | FR-09 calls scenario copy optional and forbids a new resolver. AC-08/checklist additionally mandate a **production-content validation path** without identifying required fields, invocation, failure behavior or task owner. This Editor-only fixture has no specified production-entry operation. A disconnected validator or invented publication framework could be called PASS. | Define which copy is required in which context, the existing or narrowly approved operation invoking validation, and observable rejection. Assign its implementation/test in plan/tasks. If production validation is deferred, explicitly reconcile FR-09, AC-08, checklist and parent adoption. Do not leave a mandatory undefined gate or build a second editor/resolver. |
| F-02 | **BLOCKING** | S/spec.md:85,93; S/plan.md:30; S/tasks.md:55,63 | AC-10 mandates independent source/serialized review and T7 names an independent reviewer, matching parent O4. Plan instead says “if implementation reviewer mandated”; spec ownership says “where needed”; tasks T6/worker dispatch say “when/as required.” Conditional handoffs allow bypass of a mandatory gate. Independence from the **document author** does not establish independence from the implementation author. | State uniformly that independent review of the implementation candidate and evidence is mandatory before acceptance/integration and independent of its implementation author/session. Remove conditional review wording for this task in spec ownership, plan G2 and tasks T6/dispatch. Qualification may still be performed by the authorised implementer; subsequent review must be independent. |
| F-03 | **NON-BLOCKING** | S/tasks.md:27 versus S/spec.md:20 | Tasks says “constructs rack helpers when manager exists.” UI root constructs RackLayoutTracker and StandMapSource unconditionally (baseline lines 104–105); only RackTool/PlannedFellMarkers are manager-conditional (106–110). Spec table is accurate. | Make tasks distinguish unconditional tracker/source from conditional tool/markers. Removing manager must not imply removal of all rack helpers. |
| F-04 | **NON-BLOCKING** | S/spec.md:24 | “Otherwise it clears map layers” overstates the no-ecology branch. StandMapSource.Refresh:123–133 clears Racks/Trees/Juveniles/Shelters and player/waypoint flags, but not Paths, Landmarks, MiniGuidance or cached bounds/grid fields. HasProperty becomes false; cache erasure is not established. | State exactly which layers/fields clear and which remain cached. Future preview test should cover populated-to-no-ecology transition if reachable under the selected seam. Do not claim an observed rendered ghost without runtime evidence. |
| F-05 | **NON-BLOCKING** | S/spec.md:54,76; S/checklists/requirements.md:7 | Every FR/AC ID occurs in checklist mappings, but FR-01's positive finite configurable X/Z and safe spawn inside boundary are not fully reproduced as assertions. Two prescribed footprint screenshots alone do not test invalid input or resized spawn safety. | Add positive/finite configuration and safe-spawn assertions for both footprints; define rejection or bounded behavior for zero, negative/nonfinite input where accepted by the chosen config path. Record measurement tolerance in the future native test setup. |

## Source accuracy — checked and passed, subject to F-03/F-04

All following references are at source base `7758e17c882fad01970d2f2fb8c09586bbcab5c1`. Paths are under `Assets/ForestPrototype/` except ProjectSettings.

| File | Direct source evidence |
|---|---|
| UI/ScenarioOneUiRoot.cs:76–177 | Composes existing views, unconditional tracker/source, conditional rack tool/markers; Update returns for absent manager/document. Start can still construct UI objects: the missing supported refresh/input lifecycle is the problem, not proof that nothing renders at all. |
| UI/WorkPlanView.cs:114–169 | GetWorkPlanSummary, year, cash, cost/revenue and approve/advance controls depend on real manager state. Zero orders alone is insufficient. |
| UI/AnnualReviewView.cs:88–144 | Reads HistoryYears; no-year message still calls BuildObjectives. Existing empty history is not a managerless blank-preview implementation. |
| UI/LearningObjectivesView.cs:24–114,118–180 | CurrentTopicIndex/Summary and teaching sequence are rack-first; constructor reloads optional device progress through PlayerPrefs; Refresh uses manager objectives. Optional practice credits and scenario facts must remain distinct. ScenarioOneTeachingSequence.cs was additionally opened to confirm the rack-first lesson. |
| UI/Map/StandMapSource.cs:123–155,195–242,340–385 | HasProperty requires ecology cells/count; bounds use square width; trees are scanned and scenery includes Dirt Path/Forest Clearing. No authored blank-ground rectangle projection exists. Cached-layer qualification is F-04. |
| UI/Map/StandMapModel.cs:122–163 | MinX/MinZ/SizeM; area is SizeM squared; TransformFor passes one size. |
| UI/Map/StandMapTransform.cs:18–66 | Same SizeM/SidePx drives both axes and inverse mapping. Cannot represent independent 45×60 extents unchanged. |
| UI/StandaloneSessionMenu.cs:28–149 | Global AfterSceneLoad bootstrap runs in normal Editor Play unless explicit test bypass is set. Open requires player/save/UI. MainCard probes File.Exists before Load is offered; save callback probes too. SaveAndClose calls Save. Restart closes and loads active buildIndex. |
| UI/StandaloneSessionMenu.Startup.cs:35–185 | StartupCard calls RefreshContinueState/CanLoad automatically before Continue click; ContinueSavedForest rechecks then Load. StartNewScenarioClicked separately probes File.Exists. ControlsHelp's ControlRows also dereferences forestUi.Manager. Direct scene-open alone does not establish save/null safety. |
| ForestSaveController.cs:15–101, Load/CanLoad/TryReadSave | F5/F9 guard is reference-preview-only in Update. Public Save captures before writing; Load/CanLoad probe File.Exists then read/validate. .tmp/.bak support atomic writes; read-error handling can probe backup. Public calls do not inherit the Update guard. |
| ForestSaveData.cs:7 | CurrentVersion = 24; historical geometry/management schema, no starter save identity. |
| ForestEcologyController.cs:69–113 | StandBounds/area use one size; ApplyStandGeometry applies named geometry, approach state and possible grid rebuild. Scaling ground is not a new persisted geometry. |
| StandGeometryModel.cs:14–61 | Legacy40=0, Enlarged80=1, only known models; square counts; pre-v20 saves select Legacy40. |
| ProjectSettings/EditorBuildSettings.asset:7–10 | Only enabled scene is Assets/Scenes/ForestTest.unity; additional config object binds Input actions. |

Core source claims are supported within this inspected scope, with the two narrow factual qualifications above. Restart's verified fact is the build-index call; an actual wrong-world transition in an unlisted Editor scene was **not** observed. The draft generally frames it as a conditional risk/stop.

## FR/AC coverage and testability

| FR | AC / task coverage | Assessment |
|---|---|---|
| FR-01 | AC-01, T3/T6 | Real ground/collider/walk/hierarchy required. F-05 fills configuration/spawn checks. |
| FR-02 | AC-02/07, T2/T5 | Single implementation/GUID comparison plus asset-only copy prevents cloned engines. AC-02 “no second C# script” is interpreted in this shared-implementation context. |
| FR-03 | AC-04, T2, N-01/03 | No fake economy, cells, trees, results or gameplay authority. Source/lifecycle and hierarchy evidence are required alongside counters; frozen counters after dummy initialization are insufficient. |
| FR-04 | AC-02, T2 | Four headings/common HUD/menu and no tree inspection observable. |
| FR-05 | AC-03/07, T5, N-01/02/10 | Actual ground bounds and no ghost layers; square-map limitation acknowledged. F-04 improves precision. |
| FR-06 | AC-04, T2/T6, N-03 | Exact empty message, hotkeys/clicks and no work/year/economy mutation. Fake zero treasury cannot pass. |
| FR-07 | AC-05, T2/T6, N-04 | Empty review/objectives, reopen/persisted-preference negatives; no imported lesson completion. |
| FR-08 | AC-06, T4, N-05–09 | Universal pre-probe refusal including direct methods, startup/menu/keys, sidecars and access tracing. Hash-only proof expressly insufficient. |
| FR-09 | AC-08, N-11 | DEV text policy clear; production rejection witness undefined: F-01. |
| FR-10 | AC-07, T5 | Both 45×60 world/map and original 30×30, owned asset-only copies, same scripts. Dimensions must be measured, not merely printed. |
| FR-11 | AC-10, T3/T4, N-08 | Direct Editor entry/restart safety and no settings/packages/picker; Start New automatic probe is included by the universal fence. |
| FR-12 | AC-09, T6/T7, N-12 | Original five-rack/Rack V2/MFM, v1–v24, Legacy40/Enlarged80, frozen Reference Future and RNG/growth/regeneration/wind/save anchors protected on final integrated candidate. |
| FR-13 | AC-10, T3/T7 | Unsaved authoring label, no finished Compact/Multi/gameplay/build claim. |

All FR-01…FR-13 and AC-01…AC-10 are mapped by the checklist. ID coverage is not complete assertion coverage (F-05) or a native PASS. Mandatory review needs F-02 reconciliation. AC-09 uses existing expected anchors rather than inventing new values; future native preflight must pin fixtures, gates, configuration and expected-value manifest. No rebaseline is authorised.

## Hard boundaries, scope and ownership — checked and passed

- **Save fence:** FR-08/T4 genuinely require refusal before capture, file-existence probe, validation or mutation through every reachable entry point. This includes automatic startup/menu display and Start New, not just Save/Load clicks. Future trace must cover existence/stat/open/read/write attempts from scene startup, not start after title display or rely on atime/hash. Distinguish test preparation of preserved fixtures from preview-process access. No existing implementation safety is claimed.
- **No active manager/ecology:** absent versus inert components is expressly a pre-G1 G4 decision, not a hidden grant or a requirement to fabricate a forest. It is operationally decidable through source/lifecycle evidence, hierarchy and mutation negatives from scene load. Initializing a dummy economy/grid then disabling it is not compliance. Exact component mechanism can remain open at DRAFT review; no class/API is selected here.
- **Rectangle:** unchanged square transform or a fabricated 60×60 ecology stand cannot satisfy 45×60 true ground/map. Narrow preview projection/source approval and historical square/save invariance remain explicit; no silent model extension is authorised.
- **Scope:** Compact30/#820/#836 rebuild, H1/path/fence/floor, Multi 2D transplant, #543/#544 router implementation, save ID/schema/profile, registry/picker, packages, ProjectSettings and Windows builds excluded. FR-11's softer “no requirement” is constrained by AC-10 and the explicit worker prohibition. Diverged Compact evidence is correctly not promoted to combined qualification.
- **Status/custody:** all artifacts DRAFT/NOT APPROVED, native NOT RUN, worker/lease unassigned. #844 shared-source ownership, #425/product gates and physical custody are not inferred from read/compile access or owner-reported idle state. Save-principle approval is distinguished from router implementation. Earlier author handoff readback receipts identify their earlier heads; #856 final comment supplies the final six-commit receipt, so those are not false current-head assertions.
- **Historical protection:** original five-rack scene/UI/GUID identity, Rack V2/MFM, Legacy40/Enlarged80, v1–v24 and frozen Reference Future v1 are named across requirements/qualification. No anchor change or retrospective no-rack behavior is permitted. These are contractual protections, not native verification.

## NOT CHECKED / NOT RUN

- No Unity launch/import, compilation, runtime, scene/prefab authoring, build, rendered review, performance measurement or save experiment. No unity-lock acquired; no user forest-save opened or changed.
- No implemented blank scene or approved final seam exists in this artifact set. Complete transitive UI null/side-effect safety, lifecycle feasibility, rectangular render/minimap parity and exhaustive runtime call graph remain future source/native gates.
- No historical v1–v24 fixture matrix or deterministic/reference harness executed; baseline limitations/expected anchor manifests not independently qualified.
- No physical Ops lease, #690/save-worker custody or Compact candidate correctness certified. This reviewer owns the report only.
- External #855/#854 full adoption bytes, #543/#544 complete save contract and owner G5 receipt were not independently audited in full; status was corroborated through supplied parent/issue context. They confer no approval here. No ecological fact was added.
- Dashboard/Drive synchronization not checked; parent Manager may ingest this review. No PR, merge, main push, rebase or force push. Reviewed files were not fixed or rewritten.

## Publication and next action

Publication is limited to this one added Markdown file from main `cbcb3850c3952a8d74e54606279165b2f4be9add`. Commit/push and GitHub exact-byte readback are checked after content freeze; final return records resulting commit/readback (a file cannot embed its own commit SHA).

Next: bounded author/Manager corrections for F-01/F-02 and independent delta review. Formal product/spec adoption, source grants, physical Unity custody and integration remain separate gates.
