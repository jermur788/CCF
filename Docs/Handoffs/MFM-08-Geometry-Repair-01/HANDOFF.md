# MFM-08 Geometry Repair 01 — Handoff (claim #192, parent #156)

**Result: `MFM-08 SEGMENT FOOTPRINT REPAIR IMPLEMENTED — READY FOR MANAGER REVIEW`**

This is a bounded bug fix with focused verification. It is not the successor #156 Tier-2 qualification, which must still be rerun on the repaired candidate.

- **Provider/model:** Claude Code, Sonnet 5.5.
- **Exact start:** `d55ec86a3f0bf13820f2080197b6b0f43d832e06`.
- **Repair production head:** `e5fb424635f16182748efcb0539e8a49539495a9`. This is the only commit on the branch. This handoff is a docs-only successor commit on top of it.
- **Branch / worktree:** `fix/mfm-08-segment-footprints-v1`, `/media/jer/Videos/CCF-MFM08-FIX`.

## Changed files

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/Map/StandMapAreaFootprints.cs` + `.meta` | New pure helper (no UnityEngine): the one derivation of every area's footprint and centre. |
| `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs` | `Footprints()` and `TryAreaCentre()` now read that derivation; the whole-unit-by-`UnitIndex+RackId` logic is removed. |
| `Tools/Verification/MfmTreatmentReview/OfflineLogic/AreaFootprintChecks.cs` | New focused checks (axis-aligned and rotated plus negative controls). |
| `Tools/Verification/MfmTreatmentReview/OfflineLogic/TreatmentReviewOfflineCheck.cs` | `Check` made `internal` and `AreaFootprintChecks.Run` called. No existing check was changed or relaxed. |
| `Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py` | Compiles the two new sources. The offline boundary allow-list gained the helper and this handoff directory. |

Not touched: coverage classification and calibration, the world adapter, `ScenarioOneRackSampleGeometry`, Rack V2 persistence, save, schema, hash and reference files, prescriptions and approval, the Work Plan, history, teaching, scenes and prefabs. Candidate and proposal glyph code, the card and the approval flow are unchanged.

## Derivation

Coverage assigns a point to a segment with `along = x*AxisX + z*AxisZ - anchorAlong`. Low is `along < -S`, Centre is `-S <= along < S`, High is `along >= S`, with `S = Calibration.SegmentHalfLengthM`. `StandMapAreaFootprints.Derive(coverage, geometry)` cuts each coverage unit's own polygon (`CoverageUnit.Polygon`) with exactly those two lines, one half-plane at a time (Sutherland–Hodgman). The inputs are the existing `Geometry.AxisX/AxisZ`, `Geometry.Anchor` and the area's `Area.Segment` enum. It never parses the `R1L/R1C/R1H` id and does not assume an east-west plan.

- **Source:** the area flagged `IsSource` takes the fixed teaching rectangle from `geometry.Teaching` (±`ScenarioOneRackBay.HalfLengthM` along the rack, ±`HalfWidth` across, about the anchor). It is neither enlarged nor redefined.
- **Remainders:** these keep their authoritative `OuterRemainder.Polygon`.
- **Centre:** the area-weighted centroid of the same polygon, or the vertex mean if it is degenerate.
- **One shared instance:** `StandMapTreatmentReview` derives footprints once per `ReadAll()`. The map layer, its printed tags and Inspect in forest all read that instance, so the waypoint is exactly the centre of the shape the map draws.

## Verification

**Offline MFM-08 gate:** `MFM08_TREATMENT_REVIEW_PASS checks=297 failed=0`, repeat byte-identical, and boundary PASS with nothing outside the owned surface. The previous 106 checks are unchanged. 191 checks were added, run for an east-west plan and rotated plans of 35°, -20° and 90° on the real `ScenarioOneRackSampleGeometry`. Per plan they check:
- Low, Centre and High of every unit are distinct polygons.
- Segment vertices stay inside the along-axis boundaries, with the boundaries recomputed independently from anchor and axis.
- The segments tile the unit strip exactly (area sum).
- Each footprint centre falls in the segment the coverage rule assigns it.
- The source is the 24 m × 18 m (432 m²) teaching rectangle, with its corners on the teaching frame's own limits and its waypoint at the teaching anchor.
- Waypoints are the polygon centroids, ordered Low < Centre < High along the axis and at least 5 m apart.
- No two tags share a position or overlap.
- Remainder polygons are unchanged.

Also checked: invalid input produces nothing guessed, and the review card has exactly one derivation site, with no separate unit or remainder geometry left in it.

**Negative controls:**
- The d55ec86a whole-unit derivation, re-implemented in the check, fails the distinctness predicate in every plan.
- Mutation A (helper ignores the segment) fails the distinctness, boundary, tiling and centre checks.
- Mutation B (source falls back to the whole unit) fails the three source checks.
- Both mutations were reverted and the helper is byte-identical to the committed file.

**Regression** (`/media/jer/Files/CCF/Verification/MFM08-GeoRepair-regression/`, all with repeat PASS): MFM-04 240/0, MFM-05 95/0, MFM-06 172 pass, MFM-07 125/0, MFM-08A 102/0, MFM-08 297/0.

**Unity (used; Tier-1 compile and footprint smoke only):** the lock was acquired normally for task `MFM-08-192-REPAIR-SMOKE` (lock_id `5de64dd5bbdf4100b4777d19bb6a8e97`) with the repaired head clean and detached in `CCF-U1`, then released normally (forced: False), and status is `free`. Unity 6000.6.0f1 compiled the repair. The Tier-2 harness from `qual/mfm-08-unity-v1` was run unchanged, except that its "footprint equals the unit polygon" check was reformulated to expect the segment cut (or the teaching rectangle for the source). The two defect assertions from the returned Tier-2 run were not touched. The result was **`MFM08_UNITY_PASS checks=353`**:
- The live footprints equal the independently expected segment cuts, worst deviation 0.00 m.
- Both defect assertions pass (distinct footprints in all 5 units; the sample drawn as the teaching rectangle, 24 m of an 80 m unit).
- Tag overlaps are 0 of 15 at both 1280×720 and 1920×1080 (they were 15 of 15).
- The rest of the 351-check matrix (candidate flow with real [E]/[C]/[N], proposals, spot-check, approval to Pending, Work Plan gate, repeated approval refused) passes unchanged.
- Visual check of the 1280×720 capture: SAMPLE is its own crosshatched rectangle, the dashed NOT COVERED area is visible, and tags are distinct.

Evidence: `/media/jer/Files/CCF/Verification/MFM08-GeoRepair-unity-20261030T210141Z/`. The updated harness is committed on `qual/mfm-08-unity-v1`.

This does not replace the successor Tier-2. The deferred Rack V2, Reference Future, MFM-06/07 Unity and UI regressions, and the open-work conflict, remain to be run on the repaired candidate.

## Clean return

- The repair worktree is clean at `e5fb4246`, apart from this handoff, and was pushed.
- `CCF-U1` is clean (detached at `e5fb4246`). Editor normalisation diffs (`.vscode/settings.json`, SectionFive materials) were saved and reverted. No staged files remain and no Editor is running.
- The Unity lock is free. No Windows build.
