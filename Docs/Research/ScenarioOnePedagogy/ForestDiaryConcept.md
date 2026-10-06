# Forest Diary — concept (Workstreams E1, E3)

**Status:** NEW PROPOSAL [INF]. Practitioner basis: AFI two-scale monitoring, ANW Dauerwald permanent plots with recorded interventions, French permanent plots, Pro Silva monitoring [PRAC §2.6, §14.5, §17].

**Purpose:** help the player connect **past management → current forest**, without turning the game into a spreadsheet. The world stays the primary evidence; the diary points back to it (D-004, game brief).

## 1. What already exists in the save (inspected at `3e4ee40`)

| Saved record | Cadence | Fields relevant to a diary |
|---|---|---|
| `ScenarioEcologicalSnapshot` | every year from Year 0 | living trees; basal area; mean DBH; DBH coefficient of variation; mean light; mean canopy; regenerating cells; recent opening; understorey group means; deadwood count, volume, decay; **per species:** living trees, basal area, regeneration cells, regeneration density, planted cells, planted juveniles |
| `ScenarioAnnualReport` | every advanced year | tasks completed/failed; contractor cost; timber revenue; harvested volume; regeneration removal; deadwood created/decayed; kept-for-use; closing cash; minimum adjustment; owner minutes; timber sales by product |
| `ScenarioManagementEvent` | every order, approval, resolution, purchase | year; type; outcome; **target tree id; cell index; world position**; species; volume; costs; revenue; executor; failure reason |
| Deadwood records | per log | position, creation year, decay |
| Planted juveniles | per stem | species, position, height, alive, promoted tree id, last browse assessment and whether browsed last year, shelter |
| Trees | current state only | DBH, height, crown, marks, pruning lifts and year, suppression history (equivalent years), mortality cause and year |

**Conclusion:** a whole-property annual diary is **almost entirely derivable now**, with no save change.

## 2. Minimum useful history (E1)

Each item is classified AUTHORITATIVE NOW (saved), DERIVABLE NOW (computed from saved data), or FUTURE ONLY (needs new saved data).

| Category | Item | Class |
|---|---|---|
| Year | year | AUTHORITATIVE NOW |
| Management | interventions by type and count | AUTHORITATIVE NOW (events) |
| | marked/removed volume | AUTHORITATIVE NOW (report: harvested volume; events: per-tree volume) |
| | cost, revenue, net | AUTHORITATIVE NOW |
| | where the work happened (cells) | DERIVABLE NOW (event cell index / position) |
| | the player's stated purpose for an intervention | FUTURE ONLY (see `RegenerationTeaching.md` §1.3) |
| Forest capital | basal area per year | AUTHORITATIVE NOW |
| | standing volume per year | **FUTURE ONLY** (not in the snapshot; current year derivable) |
| | timber value per year | FUTURE ONLY (current year derivable by quote) |
| Structure | DBH mean and variability | AUTHORITATIVE NOW (mean, CV) |
| | full DBH distribution per year | FUTURE ONLY (current year derivable) |
| | species composition | AUTHORITATIVE NOW (per-species BA and stems) |
| | canopy layers | UNSUPPORTED (no layer state) |
| Regeneration | regenerating cells by species per year | AUTHORITATIVE NOW |
| | recruitment (new canopy trees) | DERIVABLE NOW (planted: promoted ids; natural: R-prefixed ids by year in tree list) |
| | browse outcome per year | **PARTIAL**: only last year's flag per planted stem; cohort browse fraction for last year |
| | where regeneration is, per year | FUTURE ONLY (cell-level history not saved) |
| Habitat | deadwood count/volume/decay | AUTHORITATIVE NOW |
| | understorey cover by group | AUTHORITATIVE NOW (provisional, non-causal state; label it as such) |
| Crop Trees | DBH/increment history per Crop Tree | **FUTURE ONLY** (only last year's increment exists, cleared on load) |

**Do not invent state solely for the diary.** The FUTURE ONLY items are listed for the decision paper, not proposed by default.

## 3. Diary views (proposal)

### 3.1 Stand timeline (Phase 1, no save change)

One row per year, collapsed by default to **intervention years and every 5th year**:

```
Year 0   Plantation, 336 Sitka, basal area 41.1 m²/ha. No regeneration (trees not yet seeding; ground light 0.05).
Year 1   Thinning: 30 trees (5.2 m³), net −€2,250. Light in opened cells rose.        [cells: B3 C4 …]
Year 5   Basal area 47.0 m²/ha. Regenerating cells: 24 (Sitka).
Year 6   Planted 4 sessile oak (2 sheltered). Cost €46.
Year 10  …
```

Values come from snapshots and reports. The location chips open the Stand Map at those cells, so they tie back to walking.

### 3.2 "What happened here?" (E3) — place history

Aim at the ground or open a map cell → **"History of this place"**:

```
Cell C4
Year 1  2 trees felled (P0706, P0805) — thinning
Year 6  1 oak planted with shelter (PJ-…)
Year 9  shelter on PJ-… still effective
Now     Sitka seedlings 0.6 m; oak PJ-… 1.4 m, protected
```

Derivable now from events (cell index/position), planted juvenile records and current state. **Limitation:** "regeneration began here in Year N" is not derivable per cell, because cell-level regeneration history is not saved. The diary may say "regeneration present now". It may not say when it began here unless a future cell-history record exists.

### 3.3 Tree history (Crop Trees)

```
P0707 — Crop Tree since Year 0
Year 1  2 neighbours felled (P0706 …)
Year 6  pruned to 2.5 m
Now     DBH 23.4 cm (Year 0: 20.7 cm)
```

Needs the Crop Tree's Year-0 DBH, so it needs either a saved designation-year DBH or Crop-Tree snapshots: **FUTURE ONLY**. Current DBH and events are available now.

## 4. Causality rules for diary text (E3)

The diary reports **OBSERVED CHANGE** and **KNOWN MANAGEMENT HISTORY** separately. It joins them only through language the simulation supports.

| Allowed | Example | Basis |
|---|---|---|
| Co-occurrence | "Light in C4 rose after the Year 1 thinning there." | Both facts recorded; the light model is driven by crowns |
| Simulation's own diagnosis | "This oak is still within browsing height and unprotected." | `RegenerationDiagnosis` limit |
| Competition-driven growth | "P0707's competition fell by 8 % when P0706 was felled." | Hegyi recomputation |

| Forbidden | Why |
|---|---|
| "Clearing C4 helped the seedlings grow." | Understorey has no causal effect (D-047 open item) |
| "The thinning caused windthrow risk to fall/rise" with a probability | Wind is a diagnostic; no windthrow exists |
| "Browsing killed N trees" | Browsing mostly delays growth; mortality has other causes |
| "Regeneration began because of the thinning" | Seed onset (age 20–30) also changes over time; both drivers co-occur |

## 5. UI weight

- Default: a single **History** tab in the Annual Review (`AnnualReviewHistoryIntegration.md`) and a **History of this place** line on the ground report / map side panel.
- No charts by default except the existing trend bars, plus basal area and regenerating cells.
- No table with more than six columns on screen.

## 6. Tests

1. The timeline for a saved forest is identical before and after save/load (it is derived from saved data only).
2. Place history lists exactly the events whose cell index matches.
3. No diary string uses a forbidden causal pattern (string test against the §4 list).
4. A legacy v15 save (no regeneration model field) still renders a timeline (derivation must tolerate missing fields).
