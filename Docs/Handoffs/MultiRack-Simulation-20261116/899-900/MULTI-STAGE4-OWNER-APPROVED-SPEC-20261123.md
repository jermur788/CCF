# MULTI Stage 4 — Owner-approved spec and lane plan

**Status:** `APPROVED` quartet published; implementation not started by the orchestrator.
**Owner decision:** D-095 (2026-11-23).
**Base:** `4e60c8ab806df56441497b682ccf783badc6e0ea` (Stage 1–3, not merged).

## What was decided

1. Adult DBH growth uses woodland-wide Hegyi CI; adult density mortality uses per-rack RD only.
2. Natural death is a distinct `BiologicallyDead` state with one fallen-deadwood record per death and zero timber.
3. Pruning mirrors Unity: 0.92 immediate cut, 0.92^lifts target, 3 lifts, 5-year interval, 60 % height guard.
4. Age-phase seam mirrored; Sitka has no phases; site productivity defaults to 1.0.
5. Model 0/P0 frozen; Stage 4 behind explicit versions; session-only.

## Verified Unity facts used

- `RelativeDensity = N/ha * (Dq/25)^2.063 / 1868`, onset 0.6, strength 0.08, exponent 4, probability cap 0.5, RD cap 1.0.
- `GrowAdults`: `1.2 * site * clamp01(1 - DBH/120) / (1 + CI/Ci50)`; Sitka height = `NextHeight` envelope; age +1.
- Boundary victims: suppression descending, TreeId ascending.
- `ForestTree`: max 3 lifts, 5-year interval, target < 60 % height, immediate `r * 0.92`.
- `SitkaSpruce.asset`: DBH max 120, Ci50 5, crown 0.9415 + 0.07635·DBH, form height 0.5; **no growth age phases**.
- `ScenarioOneManager` deadwood record: tree/species/position/volume/height/DBH/fallen year.

## Files

- `specs/multi-stage4-adult-sitka-parity-v1/{spec,plan,tasks,checklists/requirements}.md` — APPROVED.
- `Docs/Project/decision-log.md` — D-095.
- This handoff.

No source, test, fixture, Unity, save or package file was touched.

## Next

One D-091 lane on base `4e60c8ab`, one writer, `Tools/MultiRackSitkaP1Lab` only, Tier 1 during build and Tier 2 bounded qualification with an independent review by a different provider.
