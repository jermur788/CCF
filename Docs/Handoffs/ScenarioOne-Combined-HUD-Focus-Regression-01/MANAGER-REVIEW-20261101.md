# Scenario One Combined HUD / Focus Regression Repair 01 — Manager Review — 2026-11-01

**Claim:** #199  
**Exact base:** `6784ab17cac922a16815f4898a30737482101018`  
**Tested production commit:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`  
**Worker handoff commit:** `0d8898980b192d651360a10c55cde7e73bc4ba31`  
**Independent review commit:** `48e96ebd5195a874e4d3920774424697b1f3e939`  
**Verdict:** **APPROVED / INTEGRATED**

## Scope review

The production candidate is exactly one commit ahead of the authorised base. The production diff is confined to:

- `Assets/ForestPrototype/UI/TreeInspectionView.cs`;
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`;
- `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss`;
- `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs`.

The first three are the authorised presentation/focus surfaces. The fourth replays the already-accepted Rack-owned-tree fixture correction; no protected assertion is removed, weakened or rebaselined.

No Rack geometry/marking/persistence, MFM semantics, save/schema/anchor/reference, ecology or economy change is present.

## Qualification evidence

Accepted exact-candidate evidence:

- `combined-v2`: **465 pass / 0 fail / 24 shots**;
- `combined-legacy`: **450 pass / 0 fail / 24 shots**;
- PARITY-07: **1031 / 0 / 36 shots**;
- RackV2ConfirmMinimap: **213 checks / 21 shots — PASS**.

The protected failure families are closed: ordinary estimate inspection retains the minimap; estimate/minimap overlap is cleared; FOCUS_04/06/07 are green; the 1920 V2 competitor list is not clipped.

The independent reviewer reran `guided-v2` on exact base `6784ab17...` and on the repaired candidate. Both produced **553 checks / 47 shots / the same 11 failures**, with an empty set difference. Those failures are therefore pre-existing and are not caused by #199.

## Visual trade-offs — accepted for this repair

The following bounded presentation trade-offs are accepted for Scenario One demo readiness:

- estimate text scale: 15 / 14 / 13 px, heading 18 px;
- competitor panel/row spacing tightened;
- right-column side-panel gap reduced from 8 to 2 units.

The accepted copy is unchanged, the protected layout/focus contracts are green at both target resolutions, and the rendered review reports the result functional/readable. These choices remain valid playtest-observation items; this approval does not establish them as a universal UI scale standard.

## Process deviation

The worker updated the machine lock JSON HEAD while retaining the same owner/task without a release/acquire history event. This is a process deviation and must not become normal practice. No competing writer existed, subsequent gate preflight observed the updated HEAD, the lock was later released normally, and the independent baseline used a separate normal lock cycle. The deviation does not invalidate the qualification evidence.

## Guided-v2 residual

The independently reproduced 11-failure guided-v2 set is separate pre-existing verification/UI debt: stale v23 pins, Objectives-window overflow, teaching-sequence index expectations and the V2 rack lesson expectation. It is **not** silently declared green by this review and must be reconciled before any later gate that requires guided-v2 green. It is not grounds to reject #199 because exact-base reproduction proves non-causation.

## Integration

`integration/scenario-one-player-test-ready-v1` was fast-forwarded with lease from exact `6784ab17cac922a16815f4898a30737482101018` to the tested production commit:

`3acdf9cb635cdca79606335801e97f9d7dcce8b0`

The later handoff/review commits on the worker branch were **not** moved into the production integration ref.

Exact success disposition:

`SCENARIO ONE COMBINED HUD/FOCUS REGRESSION REPAIRED — MANAGER APPROVED / INTEGRATED`

#199 may close completed. No Windows build. Research Source Consolidation #194 remains NO ACTION.
