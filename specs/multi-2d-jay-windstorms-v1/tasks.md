# CCF Multi 2D — Jay Acorn Dispersal, Nearby Oak Woodland and Windstorms V1 — Task Checklist

- **Spec ID:** `SPEC-multi-2d-jay-windstorms-V1`
- **Branch:** `feat/multi-2d-jay-windstorms-engine-v1`
- **Base Commit:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`)

## Part 1 — ACTIVE (pure engine, not wired, not merged)

| Task | Title | Acceptance | Status |
|---|---|---|---|
| **JW-T00** | Preflight | Base and `origin/main` SHAs verified after fetch; SDK 8.0.133; clean tree; no overlapping NaturalProcesses writer | `DONE` |
| **JW-T01** | Contracts (`NaturalProcessContracts.cs`) | Immutable records/enums, append-only refusal codes, OFF-by-default settings | `DONE` |
| **JW-T02** | Keyed RNG + stream IDs (`NaturalProcessRandom.cs`) | Model-1 port bit-identical to frozen Unity; per-call draw count; fixed versioned IDs | `DONE` |
| **JW-T03** | Woodland footprint (`WoodlandFootprint.cs`) | Shoelace hectares; boundary-inclusive containment; invalid outline refused | `DONE` |
| **JW-T04** | Jay transport (`JayAcornTransport.cs`) | `r = 5 + 195u²`, uniform bearing, export without redraw/clamp | `DONE` |
| **JW-T05** | Jay Acorn Dispersal (`JayAcornDispersal.cs`) | OFF/non-mast zero draws; 10/ha cap diverted not added; source ID kept; no microsite bias | `DONE` |
| **JW-T06** | Nearby Oak Woodland (`NearbyOakWoodland.cs`) | One edge; 6/ha cap; no fabricated tree ID; same transport | `DONE` |
| **JW-T07** | Storm vulnerability (`StormVulnerability.cs`) | Frozen D-050 constants; ReducedProposal V; BoundedRational p; Stable/Watch/Exposed bands | `DONE` |
| **JW-T08** | Storm adapter (`StormAdapter.cs`) | Model 0 zero draws; one woodland-wide event; u·360 blow-towards (D-050); glyph = direction; two-pass victims; one deadwood + one opening | `DONE` |
| **JW-T09** | Salvage accounting (`SalvageAccounting.cs`) | Default leave; Work Plan gate; revenue only for Sell; Keep = retained, no revenue; duplicate-deadwood guards | `DONE` |
| **JW-T10** | Text constants (`NaturalProcessText.cs`) | Qualitative labels; glyph convention; dated non-causal teaching note | `DONE` |
| **JW-T11** | Standalone offline tests | 41 named tests across jay/storm/salvage/hygiene-parity; non-zero exit on failure | `DONE` |
| **JW-T12** | Verification gates + fence proof | restore/build/run on 8.0.133: 0 warnings, 0 errors, exit 0; fence diff within four prefixes | `DONE` |
| **JW-T13** | Handoff + push | `IMPLEMENTATION-HANDOFF-PART1.md`; commit and push `feat/multi-2d-jay-windstorms-engine-v1` only (no review branch) | `DONE` |

## Part 2 — QUEUED behind Playable Milestone 1 (must not rewrite Part 1)

| Task | Title | Status |
|---|---|---|
| **JW-P2-01** | `AnnualPipeline` wiring (storm after management, before growth; jay with existing mast/candidates) | `QUEUED` |
| **JW-P2-02** | `MultiAreaContinuation` state, Work Plan salvage orders, grouped harvest settlement | `QUEUED` |
| **JW-P2-03** | `RngSalts` registration + prefix-freeness tests | `QUEUED` |
| **JW-P2-04** | ForestMaker world options (all OFF / StormModel 0 by default) | `QUEUED` |
| **JW-P2-05** | Save v3 persistence and old-save defaults | `QUEUED` |
| **JW-P2-06** | `/multi` UI readouts, glyph, explanations, salvage choices, teaching note | `QUEUED` |
