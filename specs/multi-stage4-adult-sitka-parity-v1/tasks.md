# Tasks — Stage 4

- [x] S0 Unity source audit recorded (RD, hazard, boundary, growth order, pruning guards, deadwood fields, no Sitka age phases).
- [x] S1 Owner decisions recorded (D-095) and quartet marked APPROVED.
- [ ] S2 Add `AdultModelVersion` / `MortalityModelVersion`; defaults 0; prove model 0 and Stage 1–3 model 1 output unchanged.
- [ ] S3 Adult DBH growth uses woodland CI; height uses `NextHeight` with neutral site seam; age-phase seam present and inert for Sitka.
- [ ] S4 Per-rack RD/hazard/boundary mortality with deterministic rolls; explicit `BiologicallyDead` cause/year; one deadwood record per death.
- [ ] S5 Dead stems excluded from CI, growth, crown union and light; canopy/light invalidated after mortality.
- [ ] S6 Pruning state and fenced `Prune` order with guards, immediate 0.92 cut, 0.92^lifts target factor.
- [ ] S7 Extend views/metadata; no existing record shape change; UI stays model 0.
- [ ] S8 Tests: RD oracle, boundary cap, cross-border growth, overstocked vs thinned A, Area B isolation, pruning lifecycle, neutral site, determinism, zero drift.
- [ ] S9 Handoff and independent review; no merge.
