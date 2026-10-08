# Training Stand v2 — specification (Workstream R)

**Status:** content specification for a future authored training stand (`MarteloscopeFinalDesign.md` Q2). Supersedes `TrainingStandSpecification.md` (pedagogy branch), which could use only cases occurring naturally in the Scenario One stand.

## 1. Form

| Property | Specification |
|---|---|
| Area | 40 × 40 m (same cell grid as Scenario One: 8 × 8 cells of 5 m), so every existing system, map and UI works unchanged |
| Base stand | Irish Sitka, age 30 at start (seed-bearing, so seed-source cases work), Growth Model 1 heights. Authored positions and DBH per tree |
| Mixture | 2 mature broadleaf seed trees (Sessile Oak, age ≥ 40) in Zone C, for the seed-source case |
| Seed | Fixed; documented in the definition |
| Models | RNG 1, regeneration model current (2 once integrated), growth 1, storms per mode rules |
| Delivery | A separate definition + starting-stand asset (serialized: high-conflict, single writer) |
| IDs | Stable authored IDs `T-A01…`, so exercises and copy can name trees |
| Determinism | Two processes → identical world hash at load and after N advances |

## 2. Zones and cases

| # | Case | Zone (cells) | Authored state | Authoritative mechanism | Available |
|---|---|---|---|---|---|
| 1 | **Excellent Crop Tree** | A (B6–C7) | Sitka DBH 26 cm, H/D ≈ 65, interior, 6–8 neighbours within 8 m | Hegyi CI, growth | **NOW** |
| 2 | **Harmless suppressed neighbour** | A | DBH 11 cm at 6.5 m from #1: Hegyi share ≤ 5 % (tuned by harness) | Hegyi term small because DBH ratio and distance | **NOW** |
| 3 | **Real competitor** | A | DBH 24 cm at 1.8 m from #1: share 15–30 % (tuned by harness) | Hegyi term large | **NOW**. *Named "competitor", not "crown competitor"*: crowns do not enter competition (`CropTreeCompetitorDesign.md` §1) |
| 4 | **Large poor-form competitor** | C (E6–F7) | DBH 32 cm, forked/leaning *visual* variant, 2.5 m from a Crop Tree | Size and distance only. **Form has no authoritative state** | **UNSUPPORTED** as a form case. Options: (a) present only as a "large competitor and seed source" case (NOW); (b) add a saved stem-quality flag with timber-yield downgrading (`StemQualityFlags` exists in TimberYield) → **product decision + save field** |
| 5 | **Seed source** | C | 2 Sessile Oak age 45 at the zone edge; Sitka all seed-bearing | Seed rain by species and distance | **NOW** (with authored oak) |
| 6 | **Unstable exposed tree** | E (G2–H3) | Sitka H/D ≈ 90, 24 m, on the open south edge, neighbours removed in the authored "Year −1" (recent opening set) | Vulnerability index V: Stable/Watch/Exposed | **AFTER STORMS** |
| 7 | **Regeneration patch** | D (A1–B2) | Bright cell (light ≥ 0.3) with Sitka cohorts 0.3–1.2 m and one oak cohort | Regeneration bands, light diagnosis | **NOW** (authored bands) |
| 8 | **Dense bramble/bracken** | D (C1–C2) | Cell covers bramble 0.7, bracken 0.6, short juveniles present | M2 survival loss | **AFTER REGEN MODEL 2** |
| 9 | **Browse pressure** | D | Oak juveniles 0.5 m unprotected; pressure 0.4 in the training definition | Shared browse response | **NOW** |
| 10 | **Deadwood** | B (D4) | 3 fallen logs at decay classes 0, 2, 4 | Deadwood records, habitat display | **NOW** |
| 11 | **Gap edge** | B (D3–E4) | 2-cell authored gap (recent opening 0 → old gap) with edge trees | Light; under storms, edge exposure only with recent opening | **NOW** (light); edge-stability lesson **AFTER STORMS** |
| 12 | Uniform pole zone | B (D5–H8) | 30 trees of similar size for the pattern exercise | Pattern descriptors | **NOW** |
| 13 | Dense group | A (A8–B8) | 9 stems per cell | CI, light | **NOW** |

## 3. Summary by dependency

| Dependency | Cases |
|---|---|
| Available now (with an authored stand) | 1, 2, 3, 5, 7, 9, 10, 11 (light), 12, 13 |
| After Regeneration Model 2 | 8 (and clearance contrasts in 7) |
| After Storms v1 | 6, 11 (edge exposure), training storm |
| Unsupported without a new state decision | 4 (stem form) |

## 4. Acceptance criteria (content packet)

- Every case's key value checked by a harness. Example: case 3’s Hegyi share for #1 is 15–30 %; case 2’s is ≤ 5 %.
- Two-process determinism at load and after 5/10 advances.
- Rendered review: each zone legible at walking height (`PlayerFacingAssetGaps.md`).
- No case depends on a visual without matching state (D-020). The forked variant in case 4 is used **only** if the form flag decision is taken.
- Performance within the Scenario One budget (same tree count order).
