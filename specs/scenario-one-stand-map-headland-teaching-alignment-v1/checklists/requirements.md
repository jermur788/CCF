# D-072 requirements and verification checklist

- **Status:** D-072 source-only specification APPROVED by Manager after independent Review 2 PASS WITH NON-BLOCKING NOTES, zero blockers. #297 implementation READY for actual local preflight, not running. All code/Unity/integration checks NOT RUN.
**Authority:** [D-072 user-confirmed Option B](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md), under D-071 five parallel Rack V2 family; [#297 map repair](https://github.com/jermur788/CCF/issues/297) precedes [#296 diagram](https://github.com/jermur788/CCF/issues/296) in [#294 overnight chain](https://github.com/jermur788/CCF/issues/294). [#295 T2 cues](https://github.com/jermur788/CCF/issues/295) already implemented and independently reviewed **SOURCE ONLY**; its code is preserved but not integrated or Unity verified. `Docs/Project/coordination/active-tasks.md` is live single-writer authority.

## Design and ownership
- [x] User directly chose Option B, superseding temporary T3 deferral; no new forest geometry, racks or saves.
- [x] Current-source discrepancy identified: `Dirt Path` planks-only map paths versus separate 3D V2 headland/feeder mesh.
- [x] Exact authoritative geometry and five authentic source-entrance identities documented; no invented rack-spacing calibration.
- [x] #298 original independent review returned **NEEDS CHANGES (B1–B4)**; original evidence retained.
- [x] Manager revised all four canonical specs, with B1–B4 mandatory mechanisms and no changed game design.
- [x] Independent [Review 2](https://github.com/jermur788/CCF/blob/docs/d072-independent-spec-review-20261106-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-INDEPENDENT-SPEC-REVIEW-2.md) returned PASS WITH NON-BLOCKING NOTES, zero blockers, on amended four-file HEAD.
- [x] Manager #200 APPROVED #297 source-only start after independent review; conditional single-writer reservation in live coordination, with actual branch/worktree/clean/HEAD preflight and claim start still mandatory.
- [ ] #297 branch/worktree/one-writer/starting HEAD and protected local user checkout/disk preflight verified by actual worker.
## Map source (not yet implemented)
- [ ] **B1 mandatory data mechanism:** new separate `StandMapModel.ExistingHeadlandSurfaces` pure collection **never** appended to `model.Paths`. Existing Paths count, indices, point sequences and widths unchanged in all states; historic path-count tests NOT weakened.
- [ ] **B1 FULL-only draw:** new surface rendered **only when `detail == MapDetail.Full`**, HUD Mini and Stand Map embedded Mini overview lists element-for-element identical with/without new collection.
- [ ] **B1 model version/legend:** `StaticContentHash` mixes surface visibility + both segments + both pads so `StaticVersion` invalidation/repaint works; only a separate presence-gated full-map “Existing V2 headland/feeder — not a planned rack” legend row added.
- [ ] **B2 fail-closed gate:** pure static predicate `V2 && Enlarged80 && surfaceActive`; `RackPlanningV2Active` read from manager, positive proof cached `ScenarioOneHeadlandPresentation` reference and active named child `Scenario One V2 Headland`/renderer. Unknown, absent, hidden or disabled ⇒ **draw nothing**.
- [ ] **B2 cache/load transitions:** 0.25s rack-read cadence or invalidation detects V2↔non-V2 load on unchanged scenery geometry key, no property, absent child and reappearance; no per-tick global scan/mesh read.
- [ ] **B3 exact source geometry:** **two new** 2.6m-width centrelines only (western headland south→north and single south feeder west→existing approach `PathX`), plus **two 2.2m-radius pads** at authored junction points, no extra road or extension.
- [ ] **B3 visual union/order (NB-1/NB-2):** new surface immediately before existing Paths after rack corridor bands; first all new casing strokes and both separate pad-outline items, then all tan strokes and both separate pad-fill items, then existing Paths. Later rack centre lines/entrances visible; no junction ring cut-through, dark bar or false road widening.
- [ ] Negative state/compat cases: HUD minimap and embedded Mini overview unchanged, Legacy40/frozen Reference Future v1/pre-V2 Enlarged80 unchanged; no phantom surface in missing-template case; original `Dirt Path`, existing rack plan and save state intact.
- [ ] No scene/prefab/physics, collision, clearance, RackPlan/manager, MFM/Work Plan, new road, saved schema or actual rack topology changed.
- [ ] **B4 Unity-free offline seam:** actual `ScenarioOneHeadlandGeometry` consts projected only in Unity-side `StandMapSource` to plain model data; pure `StandMapModel`, draw plan and eligibility predicate compile without UnityEngine.
- [ ] **B4 source-constant parity:** source-text regex reads exact named `const float` fields in `ScenarioOneHeadlandGeometry.cs`; verifies endpoints/width/pads versus projected shape (do not compile Unity file into offline harness or invent independent constants).
- [ ] **B4 offline matrix:** V2+Enlarged80+surface show; Legacy40, frozen Reference Future preview, non-V2 Enlarged80, V2 with absent surface, no-property hide; Paths/racks byte/equivalent unchanged; Mini draw lists exactly equal; Full static count/ordering, hash/repaint tested.
- [ ] **B4 measurable refresh:** read-only `HeadlandProjectionReads` increments only when projection actually recomputes; steady walking/view refresh leave counter unchanged. No new scene-wide scan on each 0.25s cadence.
- [ ] #297 source/offline checks PASS, separate independent Sonnet source review zero blockers, Manager approval for #296. Existing `StandApproachVerification`/`StandMapParity` full Unity gates remain **NOT RUN until eligible Editor qualification** and must not be relaxed.
## Diagram source (not yet implemented)
- [ ] #296 starts only after map source approved and writer ownership transferred cleanly.
- [ ] Exactly 5 authentic parallel racks/5 entrances, source-derived nonuniform spacing, existing west headland/one southern feeder; six retained standing bands. **Rubric FR-5 / AC-D / AC-V pinned to approved substantive PR #265 commit `e5fe1761744d880798fd7cc685362f1e28b7c60b`**, no wholesale divergent PR merge.
- [ ] Static example clearly not the player's live/saved plan, planned != marked != confirmed != executed, no false cleared corridor/road.
- [ ] Labels/legend readable in grayscale and all required meaning without colour; site-specific 16–20 m guidance not hard-rule.
- [ ] Intro/other lessons unchanged; diagram implementation/source handoff and independent review/Manager verdict.
## Unity and release (not yet run)
- [ ] #287 existing Workbench candidate exact-head, save/history/Legacy40/frozen reference, true exclusive Unity lock and Tier-2 passes.
- [ ] Integrated exact-head Unity import/compile, native input/prompt focus/Help/drag and Work Plan commitment tests PASS (old #295 source acceptance is NOT a runtime PASS).
- [ ] Full Stand Map and teaching diagram at 1280×720 and 1920×1080, colour and grayscale, geometry/source overlay, text size, full recognition binary rubric PASS. Minimap negative-case and 3D world agreement checked natively.
- [ ] Historical feedback checker anchor repaired in independently reviewed **non-weakened** bounded claim; old six checks and FAIL 08 semantic test preserved; do NOT count its current exit 2 as PASS.
- [ ] No protected save/schema/Reference Future v1/Legacy40/parity/MFM/rack regressions; no edits to user personal saves.
- [ ] Separate Manager integrated verdict on exact candidate and next focused playtest handback, user personally tests it.
- [ ] Windows/Player build remains prohibited until explicit post-playtest user permission; no implicit build authorisation.

Unchecked is **NOT RUN / NOT VERIFIED**; source-only assumptions and previously reported historical fixture numbers are not substitutes for actual Unity/native/render input proof.
