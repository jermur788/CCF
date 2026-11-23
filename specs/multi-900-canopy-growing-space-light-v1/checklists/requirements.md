# Multi #900 — Approved requirements checklist

**Status: APPROVED (D-094, 2026-11-23).** Mark from observed evidence only.

## Decision gates (satisfied)

- [x] G01 #899 crown model fixed (C2) and fed into this layer.
- [x] G02 Light model fixed as `Modelled Light Index (Unity k10a10 proxy — not calibrated PAR)`, version `unity-k10a10-decoupled-v1`.
- [x] G03 Grid/buffer fixed: 2.5 m default, 5.0 m parity, `b = 2.5 m` independent of cell size.
- [x] G04 Boundary policy fixed: connected-woodland neighbours, open-grassland outer edge, per-rack summaries.
- [x] G05 Engine-first: no overlay/heatmap/inspector this pass; session-only outputs; zero save-schema change.

## Implementation evidence (unchecked until observed)

- [ ] R01 Empty stand returns light 1.000 and cover 0.
- [ ] R02 Single mature overhead returns 0.000; single 4 m young overhead returns 0.500.
- [ ] R03 Drip-line edge `r = 1.73 m` returns 0.409 identically at 5.0 m and 2.5 m spacing.
- [ ] R04 Two-tree narrow gap ≈ 0.349 with 0 % geometric union overlap; dense lattice shows no 13× grid artefact.
- [ ] R05 Stump and biologically dead trees contribute no cover or shade (result 1.000 in isolation).
- [ ] R06 5.0 m mode matches the Unity equation exactly; 2.5 m mode is decoupled and differs only by sampling/buffer.
- [ ] R07 Geometric cover is polygon-clipped **union**, not summed disks; uncovered m² and opening dimensions reported.
- [ ] R08 Cross-border living neighbours within 8 m influence the adjacent rack's cells; stems beyond the outer perimeter do not.
- [ ] R09 Per-rack summaries are area-weighted by clipped cell area and never blended across racks.
- [ ] R10 Approve alone changes no canopy or light value; Execute changes exactly the removed IDs; Year +1/+3 follows crown evolution.
- [ ] R11 Matched untreated control and treated run reported at years 0/1/3 with exact deltas and measured runtime on 171/73 trees.
- [ ] R12 Values finite and bounded 0–1; deterministic TreeId order; repeated runs identical.
- [ ] R13 `LightModelMetadata` present with version, kind, caveat and uncertainties.
- [ ] R14 P0/Legacy40/Reference Future v1, frozen fixtures, Work Plan fences and session-only semantics preserved.
- [ ] R15 No recruitment, planting, browsing, LAI/PAR fit, species curve, save change, Unity or UI change.
- [ ] R16 Final proof distinguishes implemented, numerically qualified, reviewed and owner-accepted; overlay deferred to a separate approved slice.
