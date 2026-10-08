# Annual Review v2 — information architecture (Workstream G)

**Status:** design proposal. Supersedes the history parts of `AnnualReviewHistoryIntegration.md` (pedagogy branch). Current view: `UI/AnnualReviewView.cs` at `a8596df`.

## 1. What is wrong with the current structure [REPO]

| Issue | Evidence |
|---|---|
| Three equal columns (WORK DONE · MONEY · FOREST) answer *what happened*, but not *where* or *so what* | `AnnualReviewView.Refresh` |
| FOREST is up to 6 generic lines, some with raw IDs ("sitka-spruce 14") | `ScenarioEcologyReviewLines` |
| No pointer back into the forest: nothing names a place to walk to | — |
| Only the first review is acknowledged; later years are skimmed | `NeedsAnnualReview` |
| Objectives footer falls back to the learning summary (`TutorialHint`) | `BuildObjectives` |
| Planted losses are aggregated with no cause | review line 4 |
| Self-thinning deaths (Growth Model 1) appear only indirectly, as deadwood | snapshot |

## 2. Organising questions

1. **WHAT I DID** — work done and failed, by type and place.
2. **WHAT IT COST / EARNED** — money, small-job minimum, owner hours.
3. **WHAT CHANGED** — forest, regeneration, disturbance. Facts only.
4. **WHERE SHOULD I LOOK?** — places worth walking to. **Never** what to do there.

## 3. Sections

```
ANNUAL REVIEW · YEAR 7                                  [History] [Map] [Walk the forest]

WORK DONE
  Thinning: 30 trees felled in 23 cells (contractor)       · Sold
  Planting: 12 Sessile oak in D6, E6 (you), 12 with shelters
  Nothing failed.

MONEY
  Timber sold        +€250   (pulp 3.1 m³, pallet 2.0 m³)
  Harvesting work    −€97
  Small-job minimum  −€2,403   (one contractor visit costs at least €2,500)
  Planting/shelters  −€216
  Cash               €9,534 → €7,068
  Your time          4.0 h of 40 h

FOREST
  Trees standing     306 (−30 felled, −0 natural deaths)
  Basal area         38.3 m²/ha (was 36.2 after felling; grew +2.1)
  Canopy             0.93 · ground light 0.07
  Crop Trees         competition around them 4.9 (was 6.1 before Year 6 thinning)

REGENERATION
  Sitka seedlings in 17 cells (was 14). Tallest 0.6 m.
  Planted oak: 12 growing, 0 lost. 12 protected by shelters (7 years left).
  [M2] Dense bramble/bracken in 3 cells with young trees.

DISTURBANCE                                  (only when something happened)
  2 trees died from crowding (self-thinning); now fallen deadwood.
  [Storms] STORM — 26 trees blew down …

PLACES TO INSPECT
  ▸ D6  Your new oak planting                         [Set waypoint]
  ▸ C4  Largest opening from this year's thinning       [Set waypoint]
  ▸ F2  Two trees died from crowding                    [Set waypoint]

OBJECTIVES / PROGRESS   (the scenario progress panel; ScenarioCompletionDesign.md)
```

## 4. Rules

| Rule | Why |
|---|---|
| Sections appear in fixed order; empty sections collapse to one line ("No disturbance this year.") | Predictable reading |
| Every number is a **change** or a **before → after** | The review is about the year |
| PLACES TO INSPECT has at most **3** entries, ranked deterministically by the fixed priority below. Each has a Set waypoint button (existing map waypoint) | Points back to the 3D forest (D-004) |
| PLACES entries describe **what is there**, never an action ("Your new oak planting", not "Check your oak needs clearing") | Non-prescriptive |
| Causal wording only where the simulation computed the cause ("died from crowding" = mortality cause `self-thinning`) | D-020 |
| Species display names everywhere (no raw IDs) | Terminology |
| The acknowledgement gate stays for the first review. Later reviews add a soft prompt: PLACES must be scrolled into view before "Walk the forest" highlights. Nothing blocks | Reduce skimming without nagging |

### Place priority (deterministic)

1. Storm damage (largest new opening) — after storms.
2. Places with work done this year: largest felled cluster, each planting cluster, clearance cells.
3. Planted stock with losses this year.
4. Natural deaths (self-thinning) clusters.
5. Newly regenerating cells (needs the previous year's cell state; see `ForestDiaryV1.md` §4, so v1 uses only "regenerating now in a cell where you worked").

Ties: lowest cell index. All inputs are saved data or deterministic derivations.

## 5. How storms fit

The storm section from `PlayerFeedback.md` [STORM-DESIGN] drops into **DISTURBANCE** unchanged. Its "Worth inspecting" items join PLACES TO INSPECT at priority 1. No layout change is needed when storms arrive. This is the main reason to restructure *before* storms.

## 6. Derivability

| Section | Source | Save change |
|---|---|---|
| WORK DONE | `ScenarioManagementEvent` (type, outcome, cell, executor) | None |
| MONEY | `ScenarioAnnualReport` | None |
| FOREST | `ScenarioEcologicalSnapshot` (this and last year); trees with `mortalityCause`/`mortalityYear` | None |
| Crop Trees CI "was" | Live current CI only in v1. "Was" needs history (`CropTreeReleaseMetric.md` §5) | None in v1 (omit "was") |
| REGENERATION | snapshot per species; planted juvenile records; M2 cell covers | None (M2 adds its own fields) |
| DISTURBANCE | mortality cause/year; storms: `stormEvents[]` (storm packet) | None here |
| PLACES | the above, plus cell indices | None |

## 7. Not in v2

- Recommendations of any kind.
- Projections of next year.
- A score, a grade, or "good year / bad year".
