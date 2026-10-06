# Decision paper 3 — Forest Diary / permanent plot

**PRODUCT DECISION REQUIRED** for Phase 2 only. Detail: `ForestDiaryConcept.md`, `PermanentPlotDecision.md`, `AnnualReviewHistoryIntegration.md`.

## Question

How should the game let the player connect past management to the current forest?

## Options (summary)

| | A. Whole-property diary | B. Permanent sample plots only | C. Hybrid (A + optional plots) |
|---|---|---|---|
| Save change | **none** (derived from existing snapshots, reports, events) | yes (new records) | Phase 1 none; Phase 2 yes |
| Teaching value | trends, money, structure | individual-tree and place-level response; the real AFI/ANW method | both |
| UI weight | one History tab + place history | plot placement + measurement view | both, plots optional |

## Recommendation

**C, phased.**

- **Phase 1 now (no save change):** History tab in the Annual Review, "What changed since your last intervention", "What to inspect next" (place + fact, never an action), and "History of this place" on the ground report and map.
- **Phase 2 after Sol's save/growth integration:** up to 3 permanent sample plots (one cell each), measured every 5 years, copying authoritative tree and regeneration state. This also closes the missing Crop-Tree growth-history gap for places the player cares about.

## Save implication

Phase 1: none. Phase 2: a new save version with `samplePlots[]`, validation bounds, and Reference-preview isolation. **Single-writer save schema: must be sequenced after Sol's growth-model versioning.**

## What the user needs to decide

1. Accept Phase 1 (recommended: yes)?
2. Accept Phase 2 in principle (plots, 5-year interval as a definition setting)?
3. Should the diary also record the player's stated purpose for each intervention (a small event field)?
