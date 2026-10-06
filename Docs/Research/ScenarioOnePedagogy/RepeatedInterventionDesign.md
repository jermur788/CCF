# Scenario One — repeated intervention design (Workstream B6)

**Status:** NEW PROPOSAL [INF], supported by harness evidence [HARNESS] and practitioner guidance [PRAC]. Not accepted.

## 1. Why this is the central lesson

Practitioner sources agree that CCF is a *process*: light, repeated selective interventions with reassessment in between [PRAC §2.2–2.3, §11.1, §14.3]. Irish guidance describes a transformation of roughly 40 years from second thinning, made of many interventions [PRAC §11.1]. That figure is context, not a game timer.

The current build never asks for a second intervention (audit A2 §1 item 9). A player can complete the scenario with one thinning of one tree.

## 2. What the simulation already shows (harness evidence)

The read-only harness (`ResidualStandEvaluationPrototype.md`) applied each treatment once at Year 0 through the real Work Plan cycle, then advanced 20 years with no further work. Selected rows (RNG model 1, regeneration model 1; full CSV in `Evidence/`):

| Treatment | Crop-tree increment Y1 (cm/yr) | … Y20 | Crop DBH Y20 (cm) | Mean light Y1 | … Y10 | … Y20 | Regen cells Y1 → Y20 |
|---|---|---|---|---|---|---|---|
| T0 none | 0.478 | 0.423 | 28.72 | 0.055 | 0.094 | 0.058 | 14 → 37 |
| T4 conservative (15 trees) | 0.508 | 0.446 | 29.27 | 0.061 | 0.097 | 0.060 | 15 → 39 |
| T2 crop-tree release (30) | 0.533 | 0.467 | 29.73 | 0.066 | 0.103 | 0.063 | 17 → 40 |
| T5 concentrated gap (48, same volume as T2) | 0.540 | 0.463 | 29.79 | 0.138 | 0.174 | 0.084 | 23 → 40 |
| T3 heavy (87) | 0.625 | 0.536 | 31.33 | 0.101 | 0.137 | 0.092 | 25 → 46 |

Observations relevant to teaching [HARNESS + INF]:

1. **The release fades.** Every treatment's Crop-Tree increment falls year on year, because retained neighbours keep growing (Hegyi terms rise with neighbour DBH).
2. **The opening closes.** Mean light peaks around Year 5–10 and falls again by Year 20 in every treatment, as crowns relax outward (`crownRelaxationPerYear` 0.15) and trees grow.
3. **One light intervention changes the Crop Trees only a little over 20 years** (T2 vs T0: about +1.0 cm DBH). A second intervention is what turns a small advantage into a structural one. This is the game's own argument for repeated management, and it needs no new ecology.
4. **Regeneration appears with or without thinning.** Untreated T0 has 14 regenerating cells by Year 1, because the Sitka begin seeding at age 21. The current "regeneration ≥ 3 cells" objective is therefore met by doing nothing (see `ObjectiveGraph.md`).
5. **Stocking keeps rising without adult mortality.** T0 basal area reaches about 93 m²/ha by Year 20 (living trees 336 → 353). Adult suppression mortality is deferred (D-044) and is being addressed in Sol's Sitka work. **Second-intervention numbers in this document must be re-run after Sol's growth-model integration.**

## 3. Proposed design

### 3.1 The cycle the player experiences

```
inspect → intervene → advance → review → walk back → inspect changed forest → intervene again
```

| Phase | Player action | Authoritative state used | Feedback |
|---|---|---|---|
| First intervention (S4–S7) | Mark and approve a light treatment around Crop Trees | marks, Work Plan quote, residual metrics | Residual-stand block; Annual Review |
| Watch (S8, S9) | Walk back; revisit Crop Trees; branch lessons | per-year snapshots and events (saved); last-year DBH growth (not saved, see below) | "What to inspect next" list |
| Reassessment prompt | Opens ≥ N years after the first intervention | intervention year from saved management events | One line, not a command: "It is 5 years since your first thinning. Your Crop Trees' neighbours have grown; light in the cells you opened has fallen from 0.10 to 0.08." |
| Second intervention (S10) | Re-inspect, mark, approve | same as first | Residual block compares with the first intervention |
| Compare (S11) | Open the history for both intervention years | saved annual snapshots + events | Two-row comparison |

### 3.2 What counts as a second intervention

- A **later year** with ≥1 completed FellTree order, at least **N** annual advances after the first intervention year.
- N = 5 is recommended as a *tutorial pacing* value. It is not a forestry return interval, and no Swiss/Finnish/Walloon interval is imported [PRAC §5.3, §16.2]. **PRODUCT DECISION REQUIRED** on N.
- The first intervention can never satisfy S10. Two jobs approved in the same year count once.

### 3.3 What makes the second intervention different (teaching content)

These are true in the current simulation [REPO] and need no new state:

| Change by Year 5–10 | Authoritative source | Teaching point |
|---|---|---|
| Plantation trees become seed-bearing (Sitka maturity 20 → 30 years; age 30 at Year 10) | `TreeSpeciesDefinition.Maturity` | Removing a big tree now also removes seed. "Seed source" becomes a real retention consideration |
| Competitor ranks around each Crop Tree change | Hegyi terms with new DBH | Re-inspect; last time's competitor list is out of date |
| Light in opened cells falls again | cell light | The opening was temporary |
| Regeneration may now exist under or near Crop Trees | cohort bands | Felling now affects young trees too (felling damage is not simulated — see limitation) |
| The contractor minimum is the same | `minimumHarvestJobCents` | Bigger trees now give more revenue per tree, so a second thinning may pay where the first did not (to be measured after Sol's growth integration) |

### 3.4 Missing history that limits feedback

- **Per-tree growth history is not saved.** Only last year's DBH increment exists, and it is cleared on load [REPO; `Docs/Scenario1UiRedesignAudit.md` §4]. "Your Crop Tree grew X cm since the thinning" needs either:
  - (a) DBH at the intervention year, recorded in the management event for Crop Trees within 8 m of a felled tree — small save addition; or
  - (b) a diary snapshot of Crop-Tree DBH per year — see `ForestDiaryConcept.md`.

  Both need save-schema review. **PRODUCT DECISION REQUIRED** (the Forest Diary decision paper covers it).
- **Harvest damage to regeneration** in a second intervention is not simulated. Do not claim it.

## 4. Success and failure, without a universal score

| Situation | Feedback (facts only) |
|---|---|
| Second intervention released Crop Trees again | "Competition on your Crop Trees fell by 16 % (first thinning: 20 %)." |
| Second intervention removed seed-bearing trees | "This plan removes 9 seed-bearing trees; 214 remain." |
| Player never intervened again | The review at Year 25 states: "One intervention in 25 years. Light in the opened cells returned to 0.06." No penalty text |
| Heavy second cut | "Retained basal area falls from 46 to 31 m²/ha; 3 cells now above 0.25 light; wind exposure: 12 retained trees high." |

## 5. Tests (for the future implementation)

1. S10 cannot be completed by the first intervention year (year arithmetic from saved events).
2. Two approvals in one year count as one intervention.
3. The reassessment prompt appears only ≥ N years after the first completed FellTree event, and only once.
4. Loading a save between interventions preserves the first intervention year (it is derived from saved events, not from session state).
5. Determinism: a scripted two-intervention run gives identical world hashes in two processes (pattern: `ResidualStandEvaluation` determinism block).
