# Forest Diary v1 (Workstream H)

**Status:** final Phase 1 proposal. Finalises `ForestDiaryConcept.md`, `MonitoringDecision.md` and `PermanentPlotDecision.md` (pedagogy branch) for `a8596df`. **No new save state.**

**Player question:** *"What happened here over the last 20 years?"*

## 1. Source data at `a8596df` (all saved per forest) [REPO]

| Record | Cadence | Diary use |
|---|---|---|
| `ScenarioEcologicalSnapshot` | every year from Year 0 | living trees, basal area, mean DBH, DBH CV, light, canopy, regenerating cells, deadwood count/volume/decay, per-species trees/BA/regeneration cells/planted juveniles |
| `ScenarioAnnualReport` | every advanced year | work counts, costs, revenue, harvested/kept/deadwood volumes, minimum adjustment, owner minutes, sales by product |
| `ScenarioManagementEvent` | every order/approval/resolution/purchase | year, type, outcome, **cell**, position, tree, species, executor, costs |
| Trees | current | **mortality cause + year** (Growth Model 1 self-thinning), marks, pruning lifts/year |
| `ScenarioDeadwoodRecord` | per log | cell, fallen year, volume, decay |
| Planted juveniles | per stem | species, position, alive, promoted id, browse flag last year, shelter |

New since the pedagogy concept: **natural deaths per year are derivable** (tree `mortalityYear`), and each death is linked to a fallen-deadwood record.

## 2. What v1 shows

| Item | Class | Shown as |
|---|---|---|
| Year | saved | row key |
| Work | saved (events) | "Thinning 30 trees · Planted 12 oak (sheltered) · Pruned 8 Crop Trees" |
| Money | saved (reports) | net for the year, closing cash |
| Basal area | saved (snapshot) | value plus change |
| **Standing volume** | **not saved per year** | **Omitted from history**. Current volume only. Adding it needs a snapshot field (deferred) |
| Regeneration | saved (snapshot) | regenerating cells by species; planted growing/lost (per-year planted loss is derivable from juvenile records only as a total, not by year) |
| Deadwood | saved | volume on site; new this year (felling choice vs natural death, separated) |
| Notable disturbance | derived | self-thinning deaths (count); storms later |
| Management notes | **not supported** | No player purpose field. Do not invent one |

Year-row rule: show **intervention years plus every 5th year** by default; "Show all years" expands.

```
FOREST DIARY                                           whole property · 0.16 ha
Year  Work                              Money       Basal area   Regeneration          Deadwood
 0    —                                 €12,000     41.1         none (no seed yet)    0.0 m³
 1    Thinned 30 trees (release)        −€2,250     36.2→38.3    14 cells Sitka        0.0
 5    —                                 —           47.0         21 cells Sitka        0.0
 6    Planted 12 oak, sheltered          −€216      49.2         24 cells · oak 12P     0.0
10    —                                 —           58.9         32 cells               0.3 (2 natural deaths)
11    Thinned 41 trees                  −€1,620     …
```

Figures illustrative (format only); every column maps to a saved field above.

## 3. Whole-forest vs sample plots vs hybrid

| Option | What it is | Learning value | Save impact | Effort | Risk |
|---|---|---|---|---|---|
| **Whole-forest diary** | One row per year from snapshots/reports/events | High for "what did I do and how did the forest respond overall" | **None** | Small (read-only view + helper) | Aggregates hide local effects |
| Sample plots | Player places 2–4 permanent plots; each records tree list/DBH/regeneration at intervals (AFI/ANW-style) | High for local cause and effect; real practitioner method | **Yes**: plot definitions plus per-plot periodic records | Medium–large | Another mechanic to teach; save single-writer queue |
| Hybrid | Whole-forest diary plus derived *place history* for any cell (no plots) | High: aggregate trends plus "what happened here" from events | **None** | Small–medium | Place history lacks per-cell ecological history (regeneration, light), see §4 |

**Recommendation: Hybrid v1 with no save change.** Whole-forest timeline plus place history built from events, deadwood records, mortality and planted records (`MapHistoryDesign.md`). Sample plots stay a Phase 2 product decision, after Model 2 (v18) and storms (v19) have landed in the save queue.

## 4. Known limits (state honestly in help)

- **Per-cell ecological history is not saved.** The diary can say "Year 6: oak planted in D6" (event) and "D6 now: oak 1.4 m, Sitka seedlings" (current state). It **cannot** say "regeneration appeared in D6 in Year 9".
- Standing volume and Crop Tree DBH are current-only.
- Browsing per year: only last year's flag per planted stem.

If playtests show these matter, the smallest save addition is: per-year `standingVolumeM3`, `cropTreeMeanCi`, `cropTreeMeanDbh` in the snapshot (3 floats per year), plus optional per-cell regeneration presence bits (64 bits per year). That is a deliberate future schema decision, not part of v1.

## 5. Causality rules

Keep the pedagogy rules (`ForestDiaryConcept.md` §4): co-occurrence wording; the simulation's own diagnoses; Hegyi-based release. Updates:

- **Allowed now:** "2 trees died from crowding" (Growth Model 1 cause `self-thinning`).
- **Allowed after M2 integration:** "Clearance in E4 removed dense bramble; young trees there were then less limited by vegetation" — only when the M2 cause ledger supports it for that place and year. The ledger is runtime only in M2, so the diary may say this only for the current year unless ledger history is saved. **v1: no clearance-benefit statements in history.**
- **Allowed after storms:** "Year 17 storm: 26 trees blew down; most damage where you thinned in Year 15" (co-occurrence wording from the storm design). Never "your thinning caused".

## 6. Where it lives

A **History** tab in the Annual Review (also reachable from the Work Plan header). No new key binding (D-021). Rows link to the map with the relevant cells outlined.

## 7. Tests

Identical before and after save/load; a v15/v16/v17 legacy save renders (missing fields tolerated); Reference preview shows no player history; string deny-list for forbidden causal patterns; deterministic ordering.
