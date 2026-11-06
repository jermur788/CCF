# Next-action guidance repair and conditional implementation plan

- **Spec ID:** SPEC-SCENARIO-ONE-NEXT-ACTION-GUIDANCE-V1
- **Status:** DRAFT — all implementation planning below is conditional, not approved.
- **Source base:** main `1e3e159b768dc4b0bfe6e410a7728a412d34e5db`.
- **Context:** `81867c8940d7d07c952e8e49ca4a0cb93498e790`.
- **Authority/evidence:** spec S1–S14 and R disposition table. Static source only; no Unity/runtime/build/tests run by #282.

## Smallest change and trade-offs

This repair changes only the four DRAFT artifacts plus `Docs/Handoffs/ScenarioOne-UX-Expectation-Reconciliation-20261105/OBJECTIVE-GUIDANCE-SPEC-REPAIR-HANDOFF.md`. No production ownership is granted. Old P1/P2/P3 FIXED claims are withdrawn; revised prose does not close #270 findings.

After approval, prefer a small transient presentation projection alongside existing UI state: reuse CurrentInstruction/RackV2Instruction, rack-owned progress, current/presented lesson facts and actual validators. No new manager, progression engine, save field, package or geometry system. The helper is on current main, not a hypothetical candidate. Recompute after view rebuild/load and actual prompt lifecycle changes; rackMode/session facts are not saved guidance state.

Safety is not merely styling. Scoped focus/filtering (D-07) changes input behaviour and safe prompt emphasis (D-08) changes consent UX; approve and assign both explicitly. Guidance ownership does not prove input capture. Retain Consent ∥ Reset → no-primary fallback (D-09); separate lifecycle repair requires another claim. Do not quietly add modal locks or rewrite map-pointer dismissal.

A static teaching map is the smallest truthful diagram: approved five parallel racks, existing headland and southern entrance, retained stand, state/scale caveats. D-071 supersedes D-067–D-069 as Scenario One targets; no northern spine, diagonal 3+2 or MFM two-family algorithm. A live saved-plan inset has additional state/representation costs and is not the recommendation. Existing Stand Map/3D/minimap network and D-070 are outside scope.

## Conditional sequence / standalone ownership

1. **T1:** Manager decides Q1–Q4/D-07–D-13, approves source/geometry fixture, gets independent spec review and registers source/base and task lanes. No implementation while DRAFT.
2. **T2:** separately claimed cues, safe prompt styles and approved focus/filtering; source/offline mapping first. Candidate files only after registration: `Assets/ForestPrototype/UI/LearningObjectivesView.cs`, `.Curriculum.cs`, `.Sequence.cs`, `StandMapView.cs`, `WorkPlanView.cs`, `ScenarioOneUiRoot.cs`, `UiKit.cs`, `Resources/ScenarioOneUi.uss`. Minimise exact set after T1; shared USS/UiKit/Root single-writer. No consent lifecycle fix implied.
3. **T3:** separately claimed faithful Objective 1 teaching picture and reference record; candidate `TeachingDiagram.cs`, `TeachingDiagramPlan.cs`, approved label/legend styles. Coordinate Curriculum/USS with T2; preserve introduction/later artwork. No binary asset family or geometry edits.
4. **T4:** only if Manager includes later-objective scope, separately claim lesson→validator→action mapping before coding. Unknown targets remain REVIEW NEEDED; no guessed ecology/completion rules. Any later implementation needs explicitly approved file ownership.
5. **T5:** independent exact-head Tier-2 qualification at 1280×720 and 1920×1080, native pointer/keyboard input, FR-5 colour/grayscale rubric and opposite-provider review. No build. Integration/user acceptance remain Manager gates.

Each task must have its **own GitHub claim, named owner, registered owned lane/branch, exact starting HEAD, bounded file list and handoff after approval**. None is registered by this author. T2/T3 must serialize shared-file work; do not make competing writes. Docs repair lane is not an implementation or Unity lane. #200 alone edits live coordination; orchestrator alone publishes claim/ledger updates.

## Verification and stop rules

For #282, review source citations and five-file diff, whitespace/scope/DRAFT consistency, normal commit/push only. These are documentation integrity checks, not production tests or acceptance. Preserve historical FAIL 08.

Future tasks run bounded Tier 1 compiler/source/offline gates, then T5's native-input/rendered checks on the same exact implementation HEAD/configuration. Record AC-G/P/X/H/K/D/V/S and AC-L if released; no high gate waived by prose. In particular, reproduce Consent ∥ Reset and Help over retained prompts/advance warning; verify first-use Help, forced close, pointer dismissal, and Work Plan Tab behaviour. Measure actual panel scale, do not infer 720p/1080p appearance from USS. Screen-reader mechanism is deferred, not qualified.

Use authorised disposable saves; protected Reference Future/Legacy40 gates remain unchanged. No tests run against user playtest state, no auto-start script left in Assets, no broad Tier-3 rerun without concrete reason. Durable evidence belongs under `/media/jer/Files/CCF/`, not new ZX20 storage.

Stop for dirty/occupied lane, source/helper contract change affecting behaviour, new user representation choice, blocked access geometry, save/schema changes or focus/consent routing wider than approval. Return to Manager rather than silently extending scope. This plan ends DRAFT / HANDED-OFF, not implemented, verified or integrated.
