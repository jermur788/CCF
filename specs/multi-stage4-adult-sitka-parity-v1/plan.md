# Plan — Stage 4 adult parity, mortality and deadwood

**Status: APPROVED (D-095, 2026-11-23).** One D-091 lane, one writer, Core and tests only.

1. **Read-only audit — done (2026-11-23).** Exact Unity RD/hazard/boundary arithmetic, growth order, pruning guards, deadwood record fields and the absence of Sitka growth age phases were read from `origin/main` and recorded in `spec.md`.
2. **Versioning first.** Add `AdultModelVersion` and `MortalityModelVersion`; keep every default at 0. Prove model 0 fingerprints and existing Stage 1–3 model 1 output unchanged before adding behaviour.
3. **Growth.** Wire adult DBH to woodland CI, height to `NextHeight` × neutral site seam, and the age-phase seam. Default site 1.0.
4. **Mortality and deadwood.** Keep per-rack RD arithmetic and hazard; add the explicit `BiologicallyDead` transition and one deadwood record per death; ensure dead stems leave CI, growth, cover and light.
5. **Pruning.** Add per-tree pruning state, a fenced `Prune` order, the verified guards, immediate 0.92 cut and the `0.92^lifts` target factor.
6. **Contracts.** Extend read-only views and metadata without changing existing record shapes used by the UI; keep the UI on model 0.
7. **Evidence.** Unit oracles for RD/hazard/boundary, cross-border growth, matched overstocked vs thinned Area A, pruning lifecycle, neutral site, determinism, zero drift.
8. **Handoff.** Commit, tests, honest statuses; independent review by a different provider; no merge.

**Stop conditions:** any model 0 hash or P0 pin change; a public contract break that forces a UI edit; a need for Unity; an unspecified product choice; overlapping writer.
