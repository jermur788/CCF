# MFM-08 — Tier-2 Manager Review

**Date:** 2026-10-30  
**Claim:** #156  
**Verdict:** **NEEDS CHANGES**  
**Returned result:** `BLOCKED — GEOMETRY CONFLICT`

## Reviewed lineage

- Static-approved production candidate: `d55ec86a3f0bf13820f2080197b6b0f43d832e06`
- Unity verification branch: `qual/mfm-08-unity-v1`
- Returned handoff / verification tip: `eefba5eaf35d23e66af9b8ba7304f10b0cb3aa47`
- Unity: 6000.6.0f1
- Final evidence root: `/media/jer/Files/CCF/Verification/MFM08-Tier2-20261030T203629Z-run8-final/`

GitHub compare confirms `d55ec86a...` -> `eefba5ea...` adds only the MFM-08 Unity verification harness and handoff. No production file was changed during qualification.

## What is accepted from the returned qualification

The worker correctly stopped rather than repairing production under a verification-only claim. The final gate records 351 passing checks plus two deliberately failing geometry defect checks, with rendered evidence at both 1280x720 and 1920x1080. The following areas are credible positive evidence and should be reused on the repaired candidate, but do not by themselves qualify MFM-08:

- Unity import/compile of the exact production candidate;
- bounded/non-wrapping Previous/Next candidate flow;
- real 3D `[C]` confirmation and session-only `[N]` reject/reopen;
- no map bulk-confirm path;
- Prepare creates prescription/snapshot only;
- proposal wording remains non-committal and not a Fell mark;
- manual Fell conflict is held without overwriting player state;
- a waypoint alone does not satisfy the 3D spot-check;
- real 3D inspection satisfies the spot-check;
- prescription approval creates ordinary Pending work with provenance and Work Plan remains the commitment gate;
- repeated approval is refused;
- offline MFM-04/05/06/07/08A/08 suites are green.

The worker also returned the Unity lock normally, left U1 clean at the exact production candidate, and recorded the Editor-normalisation rewrites rather than absorbing them.

## Confirmed production defect

The Tier-2 blocker is real and violates the prepared Manager checklist's requirement that source-sample and coverage footprints align with authoritative geometry.

### Authoritative model

`ScenarioOneCoverage` defines every rack-served coverage unit as three areas, `Low / Centre / High`, split along the common rack axis at the sample anchor +/- `SegmentHalfLengthM`. `Area` stores `UnitIndex`, `RackId` and `Segment`. The teaching rack's Centre area is the source sample and is never a target.

`ScenarioOneRackSampleGeometry` intentionally keeps `CoverageUnit.Polygon` as one full-length strip per rack. It separately exposes the fixed teaching footprint through `Result.Teaching`.

### Fault in MFM-08 presentation

`StandMapTreatmentReview.Footprints()` and `TryAreaCentre()` currently find a `CoverageUnit` only by `UnitIndex + RackId` and assign/use `unit.Polygon`. They do not use `Area.Segment`.

Therefore all three Low/Centre/High areas of a rack unit receive one identical whole-unit polygon and one identical centroid. The final Unity run measured all five units this way. Consequences include:

1. the source sample is rendered as the whole strip instead of the teaching/sample footprint;
2. multiple per-area tags stack at one position, hiding `SAMPLE` and `NOT COVERED` under later tags;
3. per-area states cannot be read spatially;
4. `Inspect in forest` for Low/Centre/High of the same unit routes to the same point.

This is a production presentation/geometry defect, not a harness-only failure.

## Required repair

Use a separate bounded production claim. Preserve the authoritative coverage/classification/calibration behavior; do **not** change `ScenarioOneCoverage` tolerances, classification, save/schema, prescriptions, Work Plan authority, rack persistence or world state merely to make the map pass.

The repaired MFM-08 map representation must:

1. derive the polygon for each non-remainder area from its authoritative `Area.Segment` plus the existing rack/sample geometry and accepted segment boundaries;
2. represent the source using the actual fixed teaching/sample footprint rather than the whole rack-served unit;
3. make Low/Centre/High footprints spatially distinct and correctly aligned for rotated as well as axis-aligned rack plans;
4. make `TryAreaCentre` / `Inspect in forest` use the same area-specific authoritative footprint, so distinct areas do not collapse to one centroid;
5. keep remainders on their existing authoritative remainder polygons;
6. keep candidate/proposal glyph positions and map transform behavior unchanged except insofar as correct area footprints affect containment;
7. preserve all noncommitment, proposal, Pending-work, Work Plan, manual-conflict and no-bulk-confirm semantics already evidenced.

Prefer one shared pure footprint derivation used by both drawing and waypoint-centre calculation so the two cannot drift. Do not key geometry by parsing the area id when `Area.Segment` is already authoritative.

## Requalification required after repair

The repaired candidate must rerun the final MFM-08 Tier-2 gate, including the independent segment/footprint assertions and rendered review at 1280x720 + 1920x1080. The two defect checks must become genuine passing checks, not be removed or weakened.

Also run the authoritative regressions deferred by the returned candidate: Rack V2 map/runtime/field behavior, Reference Future v1/protected compatibility, applicable MFM-06/07 Unity gates and the relevant general UI gate. Exercise the open-work conflict separately in addition to the already-tested manual-Fell conflict.

## Verdict

**NEEDS CHANGES.** The broader MFM-08 interaction and commitment design is retained; the current production candidate is not eligible for integration because its coverage/sample geometry presentation is wrong.

After the bounded geometry repair returns, perform fresh Manager source review and then rerun Tier-2 on the repaired exact production candidate. Do not integrate `d55ec86a...` as-is.

No Windows build is authorised.
