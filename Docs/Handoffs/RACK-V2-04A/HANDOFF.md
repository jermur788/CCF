# RACK-V2-04A — Suggested-layout read-only foundation API

**RACK V2 SUGGESTED-LAYOUT API QUALIFIED — READY FOR RACK-V2-04 RESUME**

Worker recommendation: **APPROVE for RACK-V2-04A only**. This is bounded source/API qualification, not Manager acceptance or integration. RACK-V2-04 must consume this API after its integration; no map UI was implemented here.

## Identity and ownership

- Worker: T3 Code through Codex harness / `gpt-6.1-sol` / xhigh reasoning / configured Codex provider; Primary and focused Unity Operator.
- Starting current main: `73a840b7526d856721a1db3f36adb3433c13fa24`, verified descendant of both required integrations `f23549b18b17b3542ec8f633d7309b11020990d4` and `169970f2d6b0268d53a4077e0e97934d290bf7eb`.
- Context commit on that lineage: `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`. The approved spec, task packet and fetched live coordination were also read; coordination remains outside the context snapshot.
- Branch: `feature/scenario-one-rack-v2-suggested-layout-api-v1`.
- Dedicated clean sparse implementation worktree: `/media/jer/ZX20/CCF-RackV2-04A`; no Library created or shared there. Docs/media were excluded from the sparse working set where unnecessary to preserve SSD space.
- **Exact production/static/Unity-tested HEAD: `840475103c33be9f168e55078c1ef56b7530d049`.** This handoff is the only subsequent task-branch change.
- Validation lane: registered `/media/jer/ZX20/CCF-U1`, detached at that exact candidate. Other workers' branches were not moved.
- [Claim #20](https://github.com/jermur788/CCF/issues/20) was marked `[RUNNING]` and read back with the actual worker/model/provider, branch, worktree and starting HEAD **before production edits or Unity execution**.
- Live main/coordination was refreshed during preflight and before qualification. Main advanced to `31f8cd6` during implementation with PARITY-04 documentation and a comment-only `ScenarioSiteProfiles.cs` change; the relevant rack/headland/model foundations remained unchanged. That unrelated work was not absorbed into this branch.

## API and the production change

New API in [ScenarioOneSuggestedRackLayout.cs](../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs):

```csharp
IReadOnlyList<SuggestedRackSegment> ScenarioOneSuggestedRackLayout.Get(
    int rackPlanningModel, int standGeometryModel, IReadOnlyList<RackTree> plantationTrees)
```

The caller supplies the manager's explicit saved `RackPlanningModelVersion`, ecology's `StandGeometryModelVersion` and a detached plantation-tree snapshot, including stumps. `SuggestedRackSegment` is a readonly value with `Entrance`, `Far` and `WidthM`; the result collection is read-only and independent of its input. Order is the qualified south-to-north route identity, independent of enumeration order.

The foundation owns the qualified entrance IDs `PO0203`, `PO1204`, `PO2003`, `PO2803`, `PO3803`. This is deliberately a V2-specific foundation rather than an alteration of the legacy every-eighth-row `ScenarioOneRackLayout.Centres()` contract. The packet expressly permits foundation ownership of these identifiers; UI consumers need neither the IDs nor route-selection policy.

Geometry follows the existing combined gate's `AddQualifiedHeadlandRacks()` exactly: each qualified entrance tree's **live Z**, `ScenarioOneHeadlandGeometry.HeadlandX`, `StandGeometryModel.StandSizeMeters(...)/2` at the far end, and `RackGeometry.DefaultWidthM`. No new route coordinates, spacing target, headland dimension or starting-tree generation rule were introduced. Live Z is intentional: historical sample decimals are not new fixed targets. The live gate independently reconstructed the original qualified recipe and compared endpoints using the existing `RackGeometry.SameLine` millimetre tolerance.

Only rack model 1 **and** Enlarged80 can produce V2 guidance. Model 0, Legacy40 and unknown identities return empty. Missing/duplicate qualified tree IDs, non-finite Z or Z outside the existing headland return empty rather than fabricate another layout. The consumer must provide a complete snapshot; a complete applicable fresh world returns exactly five. Tree felling/marking eligibility does not redefine the suggestions.

The getter takes only identity numbers and readonly tree-position values. It has no manager/work-order/save/UI reference, does not call any rack mutation operation, and cannot allocate authoritative rack IDs or modify their collections/counters. Its temporary segment array is display data, not another persisted plan.

Files added:

- `Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs` and its new `.meta` (GUID checked unique).
- `Tools/Verification/RackV2SuggestedLayout/OfflineCheck.cs`.
- `Tools/Verification/RackV2SuggestedLayout/run_offline.py`.
- `Tools/Verification/RackV2SuggestedLayout/SuggestedLayoutVerification.cs`.
- `Tools/Verification/RackV2SuggestedLayout/README.md`.
- `Docs/Handoffs/RACK-V2-04A/HANDOFF.md`.

No existing production file, scene, prefab, material, package, settings, save/schema, work-order lifecycle, reference asset, legacy layout, UI, OPEN-3 or protected anchor definition was changed. No dependencies or subagents were added.

## Verification

Commands, detailed copy/run/remove instructions and the API usage contract are in [the verification README](../../../Tools/Verification/RackV2SuggestedLayout/README.md).

Executed on exact `8404751`:

```bash
python3 Tools/Verification/RackV2SuggestedLayout/run_offline.py --out /media/jer/Files/CCF/Verification/RackV2-04A-20261027/offline
python3 Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src /media/jer/ZX20/CCF-RackV2-04A --out /media/jer/Files/CCF/Verification/RackV2-04A-20261027/harness-typecheck --extra Tools/Verification/RackV2SuggestedLayout/SuggestedLayoutVerification.cs
```

- Offline: **147 checks PASS**, plus static no-UI/no-rack-mutation-call check; build 0 warnings / 0 errors. Tests cover five independent qualified sample segments, reordered/repeated reads, unchanged input, immutable nested values, collection mutation refusal, detached results, legacy/unknown identity rejection and incomplete/ambiguous/invalid input. Production geometry/headland/model source is compiled; only the unused `ForestSaveData` type needed by `StandGeometryModel.ForSave` is stubbed.
- Exact-candidate runtime plus disposable gate type check: **169 sources, 116 references, 0 errors**. An earlier pre-commit runtime-only check on the same production bytes also passed (168 sources / 116 references / 0 errors). These are syntax/type checks against an existing Player's Unity assemblies, not substitutes for the Editor run.
- Unity **6000.6.0f1**, focused Tier 1: **`SUGGESTED_LAYOUT_UNITY_PASS checks=48`**, **normal Editor exit 0**, about **139 seconds**. No C# compile error or unexpected import exception. No render, Player or native-input requirement for this API task.

The inspected harness was staged into `Assets/ForestPrototype/`, executed with `SuggestedLayoutVerification.Begin`, then removed together with its generated `.meta`. It ran through the existing `ScenarioOneProfessionalMap/run_unity_harness.py` runner on a private display, explicit Enlarged80/direct entry, isolated `XDG_CONFIG_HOME`, and a task-local Files/ext4 temp directory for Unity licensing IPC. It never called `Save()`, saved a scene, wrote a reference archive or advanced ecology. The temporary licence copy was removed after shutdown.

The first pre-Unity harness type check caught a fixture member typo (`RackPlanRack.RackId` instead of `Id`); it was corrected before the candidate commit. No production or expectation change was needed, and no failed Unity attempt occurred.

## Non-mutation and compatibility evidence

In each live fixture, 100 repeated reads alternated input enumeration order. Complete `CaptureData()` JSON was identical before/after in the same frame, as were the world hash, planned/marked counters and next rack ID. Thus rack geometry/IDs, confirmation/marks, linked work-order IDs/status and other captured world state were preserved.

- Fresh V2: five suggestions; **0 player racks / 0 marks / next ID 1 / 0 orders**, unconfirmed. Before/after SHA-256: `030f3235937ae1a0170bbf91fac750d2a37a71fa0ff33fb0801b3a879ce0afa0`.
- Confirmed/marked V2 fixture: five suggestions; **5 player racks / 1 mark / next ID 6 / 149 orders**, confirmed. These racks/orders/mark were created explicitly outside the getter. Before/after SHA-256: `bff6ec0fb5dc976d4583706b1ec93a02d18d8f43f15744b913c6e0c71cbacaf1`.
- Model-0 Enlarged80 identity fixture: **0 suggestions**. Before/after SHA-256: `f44455aa21ab29a5637cd9a76a38b1184224e1c86ba8c9c155f18a5091513979`.
- Genuine frozen Year-0 Reference preview / Legacy40 / rack model 0: **0 suggestions**. Before/after SHA-256 of captured live state: `5954d1354dc9b2c49437c425ee4e7856dace417c0a6a291e72b9eb49836006ff`.

The production loader verified the original embedded reference worlds; existing Year 0/20/50/100 anchors `A564039D9B7CE31D`, `F7DF7DAB53B6FD32`, `D5E75D6D21D631AC`, `7AD177B3CC2F73C7` and schedule `56C8B99FA1E8DDD1` were exact. Archive file bytes were unchanged, and ending the genuine preview restored the model-0 Enlarged80 identity. Archived worlds were not reserialized to establish their frozen integrity. The captured-live-state hashes above are non-mutation evidence, **not new protected anchors**.

Limit: model-0 Enlarged80 was supplied by the production Scenario One reset. This checks applicability/non-mutation; it does not requalify historical starting-tree generation. No century simulation, reference continuation, old-save migration matrix, broader regression, map rendering or whole Objective-1 playthrough was required or claimed.

## Unity queue, cleanup and preservation

PARITY-03 #10 retained priority. Its normal release event at `2026-10-27T20:26:54.784500+00:00` explicitly recorded both focused gates PASS, normal exits, unchanged user data, absent harness and clean U1. Only **after** that completed release, and after verifying free lock/no Editor/clean lane, was U1 placed at the API candidate. Claim #10's administrative handoff was still in progress; its actual Unity execution and cleanup had finished. No lock was forced or bypassed.

RACK-V2-04A then acquired normal lock **`c80785e5af214244bff5c77ba2e8a4a0`**, owner `T3-Code-Codex-gpt-6.1-sol-RackV2-04A`, task `RACK-V2-04A`, at `2026-10-27T20:30:20.694549+00:00`. It was released normally after PASS and cleanup. Acquisition/release/final-state receipts are retained.

The clean candidate incurred the familiar Editor workspace normalization and three Forb material import side effects. Their exact diffs and changed bytes were preserved externally, then only those four paths were restored byte-exact to the clean candidate. No serialized change is in the branch. U1 ended clean, no disposable harness/meta and no Editor remained. All eight pre-existing user save/preferences files retained exact SHA-256 hashes. The implementation worktree is also clean after publication.

Primary's unrelated modified `AGENTS.md` and `Docs/Project/coordination/active-tasks.md`, plus untracked `Docs/Operations/StorageArchitecture.md` and `Docs/ScenarioTwoPreProductionAudit.md`, were preserved untouched. No other lane was reset or cleaned, and no Library was copied/shared.

## Evidence and Manager continuation

Durable archive: `/media/jer/Files/CCF/Verification/RackV2-04A-20261027/`.

- `static-results.json`, `offline.log`, `harness-typecheck.log`: exact candidate commands and results.
- `preflight.json`, `claim-running-readback.json`, `live-coordination.md`: base/context/ownership and claim-before-edit evidence.
- `parity03-normal-completed-release.json`, `lock-acquire.json`, `lock-release.json`, `lock-final.json`: queue and lock lifecycle.
- `unity/result.json`, `unity/assertions.txt`, `unity/SuggestedLayoutVerification.log`, `unity/driver.log`: gate results, independent route observations and four exact before/after state pairs.
- `unity/editor-side-effects.patch`, `unity/editor-side-effects/`, `unity/restoration.json`, `user-save-prefs-before.json`, `user-save-prefs-after.json`: byte-exact preservation and cleanup.

GitHub handoff path: `Docs/Handoffs/RACK-V2-04A/HANDOFF.md` on the task branch; the publication commit is recorded in claim #20 and obtainable with `git log -1 --format=%H -- Docs/Handoffs/RACK-V2-04A/HANDOFF.md`. Claim #20 is handed off/closed per packet; this does not grant integration authority.

Recommend Manager acceptance/integration of this bounded API, then redispatch RACK-V2-04 under a new claim using this API. Independent G2/G3 review remains a Manager decision; this worker does not self-approve integration. No UI completion, whole Rack V2/Objective-1 verification, demo/MFM readiness or Windows build is implied.

**DASHBOARD SYNC: NOT AVAILABLE / NOT DONE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** Manager-owned live coordination and dashboard publication must be updated for this qualified handoff when available. No generated dashboard state was hand-edited.
