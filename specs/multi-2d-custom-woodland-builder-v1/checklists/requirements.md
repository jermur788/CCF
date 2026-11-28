# Full Custom Woodland Builder V1 Requirements Checklist

- **Spec ID** `SPEC-multi-2d-custom-woodland-builder-v1`
- **Spec status** APPROVED / IMPLEMENTING (Part 1)
- **Checklist owner** Overall Manager
- **Implementation under test** `feat/multi-2d-custom-woodland-builder-engine-v1`, Part 1 only
- **Date** 2026-11-28

## Part 1 worker evidence

| Requirement | Acceptance check | Evidence |
| --- | --- | --- |
| FR-1 | Four templates, expected area and vertex count, counter-clockwise normalisation, canonical start vertex | AC-1, BOUND-01 to BOUND-05 |
| FR-1 | Corner chamfers applied and bounded; negative, over-long and non-finite adjustments refused | AC-1, BOUND-06/07 |
| FR-1 | Footprint, area, duplicate vertex, spike and self-intersection refusals | AC-1, BOUND-08/09 |
| FR-2 | 2–8 lanes, south-first indices, exact clipping, areas sum to the polygon | AC-2, LANE-01/02 |
| FR-2 | Every lane vertex inside the boundary; no tree or grid cell outside | AC-2, LANE-03, GEN-02, MOIST-02 |
| FR-2 | Narrow or degenerate lane refuses the whole plan with a named reason | AC-2, LANE-04/05 |
| FR-3 | 2.5 m lattice-aligned grid, active cells only inside the polygon | AC-3, MOIST-01/02 |
| FR-3 | All four patterns deterministic and produce their declared classes | AC-3, MOIST-03 to MOIST-06 |
| FR-3 | No active `NeutralLegacy`; table identity and caution unchanged | AC-3/AC-4, MOIST-07 |
| FR-4 | Overrides applied after the base, later wins, invalid targets refused, caution recomputed | AC-4, MOIST-08/09 |
| FR-5 | Exactly four presets at 0.0 / 0.1 / 0.2 / 0.4 with `[E]` calibration provenance and the not-a-density note | AC-5, DEER-01 to DEER-03 |
| FR-6 | Six-species mix, sum 100, exact integer allocation, refusals | AC-6, MIX-01 to MIX-04 |
| FR-7 | Start age and stocking ranges enforced at both ends | AC-7, MIX-03 |
| FR-7 | Exact planted count, every tree inside the polygon and its lane | AC-7, GEN-01/02 |
| FR-7 | Edge seed trees flagged, mature, 0.5 m clear, refused when the ring cannot seat them | AC-7, GEN-03/04 |
| FR-7 | 3,000 cap refuses; 2,500 warning reported | AC-7, CAP-01/02 |
| FR-7 | Deterministic species shuffle; identical output across processes | AC-7, GEN-05, DET-01/02 |
| FR-8 | Standalone console gate, fenced source additions, no I/O, clock or `System.Random` | AC-8, DET-02 and worker handoff |
| D-059 | Evidence classes and calibration parameters documented, no invented Irish empirical claims | Spec evidence table and public evidence tags |
| Save and write-back | Part 1 is a pure computation API with no save impact | Source and file-fence inspection |

## Remaining acceptance

- [ ] Independent reviewer checks Part 1 conformance, calibration wording, determinism and the file fence against the final feature commit.
- [ ] Part 2 AC-9 is **QUEUED** behind the Milestones 2–5 Part 2 wiring wave.
- [ ] The coordinator creates `review/multi-2d-custom-woodland-builder-engine-v1` after review; this worker does not.
- [ ] `2D_Simulator_Strategic_Roadmap.md` v1.23 GitHub sync is **not** part of this branch and must be bundled into a single-integrator combine by its own owner.

## Verdict

- **Spec conformance:** NOT VERIFIED (worker self-check only)
- **Implementation correctness:** NOT VERIFIED (worker self-check only)
