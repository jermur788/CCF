# RACK-V2-08 — G3 blocker handoff

**BLOCKED — Manager G3 disposition required for the unmapped guided first-person rack assertions.** No production or protected-test changes were made. This is an inspection/coordination handoff, not a qualified implementation or an integration request.

```text
IMPLEMENTATION HANDOFF
Task: RACK-V2-08 — remove competing first-person rack authoring; claim #49
Worker: T3 Code / OpenAI Codex gpt-6.1-sol, xhigh; primary and single writer
Watcher: OpenCode / opencode-go/deepseek-v4.1-flash high, read-only/plan mode
Role: Primary
Branch: feature/scenario-one-rack-v2-no-competing-author-v1
Base / starting main: 3b103626d5499aaa968c10ee3c6a78c0139c29d6
Context: task packet and canonical context at starting main; live coordination refreshed separately
Coordination commit / inspected HEAD: 59ef118e2e6713b7cb7db128f6794550ad977ed8
Implementation commits: NONE
Exact tested candidate: NONE — stopped before production edits and runtime gates
Final tip: the documentation-only commit publishing this handoff and the RACK-V2-08 blocked row;
           its full SHA is recorded in claim #49 and the worker's final report
GITHUB HANDOFF PATH: Docs/Handoffs/RACK-V2-08/HANDOFF.md
GITHUB HANDOFF COMMIT: publication commit containing this file
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/RackV2-08-20261028T120834Z/
Ready for review: No — G3 disposition first
```

## Blocking evidence and required decision

The task packet's G3 rule says: **“If an existing assertion explicitly encodes the superseded first-person-authoring requirement, stop and flag it for Manager G3 review before changing that protected assertion.”** The direct dispatch repeats the stop requirement.

`Tools/Verification/SequentialObjectives/README.md:7–15` prescribes the `GuidedOnboardingVerification.Begin` gate plus the sequential companion, with `CCF_STAND_GEOMETRY=1`. It explicitly instructs native B → choose start → aim → reselect → cancel → B → confirm. In a fresh world that geometry selects Rack V2 (`ScenarioOneManager.RackPlan.cs:83–94`). There is no model-0 initialization in the guided fixture.

`Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs:182–186` then waits for `RackTool.ConfirmedCount > 0` and asserts all of:

- “rack confirmed through real OS input”;
- at least three origin selections, one reselection and one cancellation;
- a swivelled first-person preview;
- ordinary rack work created by that confirmation.

These assertions cannot qualify the required V2 absence contract. `VerifyRackFeedback` and later phases also exercise the old tool. They were inspected, not executed or modified. This conflict already exists at the starting production bytes: `ConfirmRack` refuses V2 (see below); the task did not introduce it.

The prior Manager accepted the `RackUxRack` F-10 retained/replaced/retired record in `Docs/Handoffs/RACK-V2-Combined-Integration/MANAGER-REVIEW.md`. `F10-MAPPING.md` covers the named `RackUxRack` assertions, including `B_OPENS_THE_RACK_TOOL`; it does not disposition the separate `GuidedOnboardingVerification` assertions. That accepted record is therefore not a new blocker by itself, and it is not treated as authority to retire the guided gate silently.

Related unmapped legacy premise: `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs:508–530` requires the map to wait for a first forest rack and then calls legacy `ConfirmRack` to record it. This also needs explicit V2/model-0 disposition before using that gate for current V2 qualification. Its source is unchanged.

**Requested Manager action:** issue a G3 disposition for the guided and combined-HUD first-person premises: specify their protected model-0/Legacy40 coverage and the V2 replacement absence/map coverage. No waiver of marking, progression, map, save or confirmation/minimap invariants is requested. Then resume RACK-V2-08 in this same branch/worktree with one writer. Do not merge this branch as a completed feature.

## Coordination completed

- Assigned worktree was clean on the named branch at the exact starting main. CCF-Primary was never written, stashed, checked out, cleaned or reset.
- Fetched origin; main remained `3b103626d5499aaa968c10ee3c6a78c0139c29d6`; both dispatch and minimum-base ancestry checks passed.
- Read live `origin/main:Docs/Project/coordination/active-tasks.md` and claim #49 through REST before the edit.
- Changed only the RACK-V2-07 status cell to record PR #48, merge `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9` and its Manager review, and added the RACK-V2-08 row. Every byte outside those two rows was compared and unchanged.
- Set #49 RUNNING through REST, recording executor, watcher, branch, worktree and starting HEAD. Committed coordination separately with the Codex attribution trailer and explicitly pushed the same-named feature branch.
- On finding the G3 conflict, stopped production work. Claim remains open for the Manager; no HANDED-OFF success status or closure is claimed. The task-branch RACK-V2-08 row records the blocker. Main coordination remains the Manager's integration decision.
- Publication refresh: main advanced to `c15df9f916f4dab5fca66ad8348a620370841911` (PR #51, OPEN-3 authority documentation). It descends from starting main and changes only documentation/spec authority; production and protected verification inputs are unchanged. The two target coordination rows have no incompatible concurrent edit. No Decision Log/spec edit or task-branch rebase was performed.

## Former route inventory — inspected, not remediated

`RackPlanningV2Active` is the existing identity guard: explicit rack model 1 **and** Enlarged80. Geometry alone is insufficient; historical model-0 Enlarged80 must also retain its path. `RackFirstStageActive` is an Enlarged80 teaching-stage predicate across models; it must not be removed or repurposed as a V2 identity.

| Surface | Inspected entry/copy | V2 / legacy classification and current result |
|---|---|---|
| B input, cancellation | `Racks/RackTool.cs:64–145`: `Update`, B at 92/98, mouse left/right, Enter/numpad Enter, wheel | Shared, currently live in V2 and model 0. Tool previews/claims availability, although backend refuses V2 confirmation. |
| First-use flow | `RackTool.cs:147–159`: `Request`, `introPending`, help close → `Enter` | Shared; deferred intro can enter tool without a new B press. Needs the same V2 guard as direct entry. |
| Direct tool API | `RackTool.cs:160–177`, `SelectOrigin`, `ReselectOrigin`, `Turn`, `Confirm` at 218–238 | Shared API route. `Confirm` calls only `manager.ConfirmRack(proposal, "forest")`; no authoritative V2 AddRack call. V2 entry/confirmation must be inert while legacy stays usable. |
| Tool lifetime / input suppression | `UI/ScenarioOneUiRoot.cs:103–111`, `RackTool.AnyActive` | Runtime AddComponent in both models. Keep planner available for legacy/map consumers; V2 must never hold AnyActive and suppress tree input. No serialized asset edit identified as necessary. |
| Preview panel / figures | `UI/WalkingHudView.cs`: `hud-rack-panel`, `racking` at 244, panel display at 265, `FillRackPanel` at 298 | Shared, driven by tool.Active. V2 panel should remain hidden under tool guard. |
| Tree interaction prompt | `ForestPlayer.cs:920–929` | Shared rack-stage branch advertises `[B] Create rack` in V2; replace V2 copy with Stand Map direction without disabling E/inspection. |
| Refined HUD prompt | `UI/WalkingHudView.cs:241–250` | Shared teaching branch advertises B in V2. Keep ordinary marks/interaction intact. |
| Lower HUD summary | `UI/WalkingHudView.cs:281–293` | Shared summary advertises B/Create rack; V2 needs derived plan/mark status plus `[M]` direction. |
| Startup controls / help keys | `UI/ControlsReference.cs:14,31`; consumed by `StandaloneSessionMenu.Startup.cs:34` and Walking HUD help | Pure shared copy advertises B globally. Needs narrowly selected V2/default map instructions while preserving model-0 controls in context. |
| Rack first-use/help | `UI/MenuHelpView.Text.cs:30–31,63–67`; `MenuHelpView.cs:104–135` renders Summary/Explanation | Shared origin/preview/B instructions. V2 render substitution/availability guard needed; static legacy API can stay for compatibility. |
| Full Walking HUD help | `MenuHelpView.Text.cs:94,100` | Shared `racks (B)` and ControlsReference.KeysLine advertise the old route; V2 rendered help must point to `[M] → Plan Racks`. |
| Fresh scenario feedback | `ScenarioOne/ScenarioOneManager.cs:87–91` | StartGuidance uses RackFirstStageActive, so advertises B in fresh V2. Needs the existing V2 identity guard. |
| Objective 1 current instruction / summary | `UI/LearningObjectivesView.cs:34–53` | Already uses V2ObjectiveInstruction/V2CounterText before legacy B fallback. Retain these guards. |
| Objective 1 HUD action | `UI/LearningObjectivesView.Sequence.cs:214–223`; `UI/RackLayoutTracker.cs` V2 properties | Already V2-scoped to authoritative plan/entrance counters; legacy B text only in fallback. |
| Optional curriculum | `UI/LearningObjectivesView.Curriculum.cs:25–28` (`rack.create`, `rack.layout`), `rack.map`; rendered by `LearningObjectivesView.cs:173–191` | Shared static B / common-tool copy remains discoverable in V2 optional topics. Prefer a narrow V2 render substitution; retain stable ids/titles and legacy text. |
| Shared lesson data | `UI/ScenarioOneTeachingSequence.cs:32–33` Action/HudAction | Static B copy. Current Objective 1 action consumers already select V2 instructions; preserve shared ids/titles and avoid a global legacy rewrite. |
| Legacy confirmation handler | `ScenarioOneManager.Racks.cs:103–113` | Already returns 0 for V2 before any work/group mutation. Preserve. Legacy creates pending ordinary orders/session RackGroups and fires RackConfirmed. |
| Lesson event / derived progress | `UI/ScenarioOneUiRoot.cs:146–152`; `LearningObjectivesView.cs:200–211` | Event follows successful legacy confirmation; V2 ObserveRacks reads authoritative PlannedRackCount. No independent rack-plan counter found. |
| Legacy Stand Map proposal route | `UI/StandMapView.cs:507–513` | Calls legacy ConfirmRack with source map (V2 backend rejects); current V2 map uses separate editor. Preserve model-0 route. |
| Authoritative V2 map authoring | `UI/Map/StandMapRackEditor.cs:394`; `ScenarioOneManager.RackEditing.cs:239–247,330`; `StandMapView.cs:70–71` | Create/reshape/delete/reset/undo/confirm flow to existing RackPlan runtime. V2 map planning available immediately. No replacement rack state proposed. |
| Fell/Red, Crop Tree/Blue and inspection | `ForestTreeMarkingManager.cs:107–174,199–245`; `ForestPlayer.cs` inspection | X/C gated by view/cursor/RackTool.AnyActive, with independent marking calls; no V2 identity block. Must preserve, along with E and later entrance-tree interaction. Not runtime-verified here. |

**Smallest proposed production change after G3 clearance:** guard the existing first-person RackTool by `RackPlanningV2Active`, including deferred/direct entry and confirmation; select V2-specific map-only copy at existing presentation points. Expected files: `Racks/RackTool.cs`, `ForestPlayer.cs`, `UI/WalkingHudView.cs`, `UI/ControlsReference.cs`, `UI/MenuHelpView.cs` and/or `.Text.cs`, `UI/LearningObjectivesView.cs` and/or `.Curriculum.cs`, and `ScenarioOne/ScenarioOneManager.cs`; new focused verification under `Tools/Verification/RackV2NoCompetingAuthor/`. No production edit was started. Reuse RackPlan; preserve static legacy lesson ids/titles, ordinary marking and all serialized assets.

## Verification actually performed

Read-only source inspection, Git identity/cleanliness, live coordination/claim, ancestry checks, two-row byte-preservation check, and empty production/test/package/settings diff against starting main passed. SHA-256 digests of nine relevant source/authority inputs and exact blocker excerpts are in `inspection-manifest.json` and `g3-source-excerpts.txt`. These establish the blocker and unchanged bytes; they do not prove the requested absence contract.

| Required runtime check | Result |
|---|---|
| 1. Fresh V2: attempt every former route; unchanged rack count | NOT RUN — G3 stop before production |
| 2. Stand Map create/edit preserved | NOT RUN |
| 3. V2 obsolete hint/control copy absent, rendered evidence | NOT RUN; static inspection confirms obsolete copy is still present |
| 4. Ordinary Fell/Crop marking | NOT RUN; source path inspected and unchanged |
| 5. RACK-V2-07 confirmation/minimap preservation | NOT RUN; production and protected gate bytes unchanged |
| 6. Legacy40 viable progression | NOT RUN; legacy code/gates unchanged |

Runtime source compile, focused absence tests, offline suites, Unity import/runtime, rendered UI, save/load and Player builds were not run. No pass from earlier tasks is attributed to this branch. No assertions were weakened/deleted/rebaselined. No temporary harness or `.meta` was staged under Assets.

## Unity ownership, limitations and continuation

`unity-lock status --json` returned `free`, no metadata and no Editor PIDs; receipt retained in `unity-lock-status.json`. This task never acquired a lock, placed CCF-U1, launched Unity or touched a save. No lock is held by this worker.

Changed files are only `Docs/Project/coordination/active-tasks.md` and this handoff. Serialized Unity files, assets, packages, simulation, save/schema, ecology, economy, frozen Reference Future and all Tools verification sources are unchanged. No model/provider transition occurred. The nested read-only watcher was stopped after the user supplied the completed watcher advisory and the primary independently confirmed the blocker; its unfinished audit is not relied on as evidence.

The task is incomplete. Dashboard publication/readback is unavailable/unverified and no synchronization is claimed. Claim #49 stays open with the G3 blocker and exact publication tip so the Manager can continue in the same worktree/branch. Remaining work is implementation, all static/compiler gates, six locked Unity checks, successful handoff, and only then HANDED-OFF/closure. No PR or integration was created.

BLOCKED — Manager G3 disposition required for unmapped guided first-person rack assertions
