# MFM-08 — Final Manager Review Checklist After Unity Tier-2

**Prepared:** 2026-10-30; refreshed by browser lane #162 after #151 integration  
**Static-reviewed production candidate:** `d55ec86a3f0bf13820f2080197b6b0f43d832e06`  
**Static-reviewed docs/handoff tip:** `d70cec1781c104794a6f225c8609847c55df1510`  
**Current Scenario One integration preparation reference:** `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`  
**Static browser verdict:** `APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED`  
**Source-overlap preflight:** `Docs/Handoffs/MFM-08-Map-3D-Review-UX/BROWSER-INTEGRATION-PREFLIGHT-20261030.md` at `58bc9778d36658fef20799e5c9a038f431bd5589` — no production-file overlap from the exact shared base  
**Status:** REVIEW CHECKLIST ONLY — NO UNITY VERDICT YET

Use immediately after #156 publishes the completed Tier-2 Unity handoff. Static review and source-overlap preflight do not authorise integration.

## 1. Exact candidate / scope

- Confirm Unity tests the exact accepted production candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06`, with verification-only successors clearly separated.
- Inspect any post-static-review diff. Unexpected production repair requires its own review before approval.
- Preserve the accepted bounded surface: Stand Map / MFM review / tree-inspection presentation plus focused verification.
- Reject unassigned edits to `ScenarioOneUiRoot`, Annual Review/history, generic S1-D Work Plan presentation, save/schema/hash/reference, MFM-09 curriculum, coverage calibration/world adapter, proposal→Fell conversion or bulk Crop Tree confirmation.
- Before integration, refresh the Scenario One integration head and repeat the overlap check if it has moved from `ce673714...`.

## 2. Unity import / compile

- Unity 6000.6.0f1 import/compile is green.
- `Painter2D`, ScrollView and all new `.meta`/UI paths compile/import in Unity; external standalone type-check is not accepted as a substitute.
- Lock/U1/save/world cleanup evidence is durable.

## 3. Rendered review at both resolutions

At **1280×720** and **1920×1080** verify:

- treatment-review card is readable and scrollable;
- tab/surface placement does not obscure ordinary Scenario One navigation;
- sample / target coverage / candidate / proposal states remain legible;
- card height/layout works with real content, including long reasons/conflicts;
- no important meaning relies on hue alone.

## 4. Map geometry / layer behavior

Prove with live state:

- source-sample footprint aligns with authoritative Rack/sample geometry;
- coverage polygons correspond to authoritative `ScenarioOneManager.ReadCoverage` results and identities;
- rack/sample/coverage overlay does not corrupt or obscure ordinary Rack V2/Fell/Crop marks beyond accepted presentation;
- overlay ordering and map-square interactions are usable;
- `Covered` means comparable/available, not selected/approved/committed;
- Not Covered / Excluded / Manual and explicit remainders are accurate and readable.

## 5. Candidate flow

With real living-tree data:

- candidate queue contains the intended eligible current stems and does not become stale/incomplete;
- Previous / Next are bounded/non-wrapping as designed;
- Inspect navigates to the real tree;
- ordinary 3D `[C]` confirmation remains the authoritative Crop Tree action;
- `[N]` reject/reopen is review-session behavior only and works without input collision;
- map review cannot bulk-confirm or silently designate candidates;
- candidate remains visibly/textually an inspection prompt, not a quality judgement.

## 6. Proposal / spot-check / commitment boundary

Directly witness:

- preview proposals are read-only and visibly `PROPOSAL — not a Fell mark and not yet work` or accepted equivalent;
- Prepare creates the prescription/snapshot only; it does not create Fell marks or committed work;
- map/waypoint navigation cannot satisfy the required 3D spot-check by itself;
- actual inspected-tree / extrapolated-unit interaction records the required spot-check using fresh authoritative coverage;
- prescription approval creates ordinary **Pending** work with provenance;
- Work Plan remains unapproved immediately afterward and is still the commitment gate;
- repeated prescription approval is blocked/fails safely;
- held/conflict/manual-state cases remain visible and do not overwrite player state.

## 7. Visual distinction

Confirm in captures/runtime, not source design alone:

- candidate symbol is distinguishable from Blue Crop Tree, Red Fell and Orange Rack marks by shape/form/text;
- proposal symbol is distinguishable from candidate and management marks;
- sample, Covered, Not Covered, Excluded and Manual coverage treatments remain distinguishable without relying solely on color;
- conflict marking remains clear.

## 8. Regressions / compatibility

Require the smallest authoritative current regressions for Stand Map, Rack V2, MFM-04/05/06/07 and relevant save/protected/reference compatibility. Frozen Reference Future v1 and protected anchors must remain unchanged.

## 9. Final verdict

After inspecting the exact completed handoff/evidence, issue one of:

- `APPROVED` — Tier-2 runtime/rendered residuals are all evidenced and scope is clean;
- `NEEDS CHANGES` — bounded defect/gap with exact next action;
- `REJECTED` — fundamental semantic/runtime regression.

If APPROVED, safe integration remains a separate Manager action after fresh integration-head/ownership checks. Use the exact approved production head, not a disposable verification branch. Record exact merge and docs successor.

No Windows build.
