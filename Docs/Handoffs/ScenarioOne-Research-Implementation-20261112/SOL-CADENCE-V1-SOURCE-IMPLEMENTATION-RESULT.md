# Cadence V1 source rerun — incomplete, Manager amendment required

**Assessment: NEEDS CHANGES to writable scope; implementation NOT STARTED. Not Unity-verified, integrated or accepted.**

Task: #397. Worker: GPT-6.1 Sol, reasoningEffort medium, T3 Code Codex. Sole implementation worker; no delegates launched. Date: 2026-11-12.

## Verified preflight

- Worktree: `/media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397`; launch/coordination anchor was not modified.
- Branch: `feature/scenario-one-cadence-dual-advance-v1-20261112`, created from refreshed `origin/integration/scenario-one-c01-research-copy-candidate-20261112-v1` at **a314d308315b5894263d30461a1e8b52ca0f3605**. Starting checkout clean. Source remains that base; only this report added.
- Live `origin/main:Docs/Project/coordination/active-tasks.md` inspected; newest entry releases #397 and identifies no competing UI writer. Claim was open/BLOCKED from previous storage attempt, then changed to RUNNING for this rerun.
- `df -h .`: Files ext4 `/dev/sdb4`, 176 GiB available. `df -h /media/jer/ZX20`: 20 GiB available. Both exceed 15 GiB floor.
- User explicitly selected this Files clone after first attempt on `/` had 12.77 GiB free. This is an authorized code-only storage exception/reroute; no Library created or copied.
- Packet and corrected four-part spec on `docs/manager-cadence-review-disposition-20261112-v1`, Manager disposition, original disposed review and repository workflow inspected. Local base's latest canonical-context commit is `63044dfbbeb869996ceecec8a47619dcba048b0b`; live newer Manager packet remains task authority.

## Exact scope conflict found before source writes

The packet requires skip to reject **reference-authoring** state (spec section 4), while its exclusive writable set excludes `ScenarioOneManager.cs` and expressly says not to edit that manager without STOP/Manager new permission.

At the exact approved source base:

- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs:56` declares **private bool referenceAuthoring**.
- Lines 427–430 expose `SetReferenceAuthoring(bool active)` only; there is no public read accessor.
- `ReferencePreviewActive` is a different flag. `CanAdvanceYearNow` and `CanEditWorkPlan` do not check reference-authoring. `CaptureSaveData` does not persist this transient flag.
- Therefore a helper restricted to public read-only state cannot implement the mandatory authoring guard. Guessing from preview/management eligibility would miss this case. Reflection into private state would be an inappropriate workaround for a learner-maintained production feature.

**Smallest recommended amendment:** authorize adding a read-only `public bool ReferenceAuthoringActive => referenceAuthoring;` property beside `ReferencePreviewActive` in `ScenarioOneManager.cs`. No annual method, simulation, serializer or completion-rule change is needed for that accessor. This report requests scope authority, not a change to accepted product design.

The packet's explicit STOP rule applies. The rerun is **not labeled BLOCKED**, as the user restricted BLOCKED to real ownership/base conflicts; those checks passed. This is an incomplete diagnostic handoff for a Manager scope amendment, not the requested finished source feature.

## Additional inspection for continuation

`ForestSaveController.Update` owns F5/F9 input, while public `Save`/`Load` are also called through `StandaloneSessionMenu`. A skip UI must lock both surfaces and restore their prior enabled states on every exit, as well as locking forest/other UI input. Disabling only Work Plan buttons is insufficient. This inspection does not establish that such locking has been implemented or verified.

## Changes and verification

Changed files: this Markdown report only. No C# source, tests, serialized Unity assets, meta, packages, ProjectSettings, `.vscode`, save schema, HashLayout, Reference Future v1 or completion rules changed. No user #382 lane, old save or Prefs accessed or modified. No Unity, Player or Windows build, merge or cleanup performed.

Executed: Git fetch/status/base/branch/ownership inspection, GitHub claim read/update, filesystem free-space checks, static source inspection. **Compiler/typecheck: NOT RUN (no source change). Parity A: NOT RUN / NOT WRITTEN. Parity B: NOT RUN / NOT WRITTEN. Negative tests: NOT RUN / NOT WRITTEN.** No behavioural PASS claimed. All packet implementation and runtime gates remain outstanding.

After amendment: implement helper/UI/session summary; execute documented offline runtime-source typecheck; write and run eligible offline tests; provide authentic-state strict per-tick parity A and acknowledgment-normalized parity B fixtures; qualify all negative guards/stops. Unity remains separately authorized only. No test may claim exact full-world hash equality for acknowledged UI-manual stepping.

## Routing and operational handoff

Routing as supplied by dispatch: Claude Opus 5.5 (low, T3 parent) orchestrated; GPT-6.1 Sol Medium dispatched via T3 `delegate_task`; initial run stopped at `/` disk floor; user selected Files clone reroute. Independent review by another model family should follow implementation. Cost/quota unknown.

`active-tasks.md` is Manager-exclusive under AgentWorkflow. Routing and reroute are recorded here and in #397 rather than editing that ledger. Manager must update authoritative coordination and verify dashboard ingestion/audit/published state; dashboard/Drive synchronization **NOT VERIFIED** here.

Recommended next step: Manager authorize the single read-only accessor and reopen/release source claim for continuation on this branch. Do not treat this diagnostic handoff as implemented cadence or G2/G4 acceptance. No product-choice amendment is requested.
