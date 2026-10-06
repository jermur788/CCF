# Annual Review as the entry point to history (Workstream E4)

**Status:** NEW PROPOSAL [INF]. Phase 1 needs no save change.

## 1. Should the Annual Review be the main entry point to history?

**Yes.** Reasons:

- It already opens automatically after every advance. It is the one screen every player sees at the moment history is most meaningful (D-004 "understand why").
- It already holds WORK DONE / MONEY / FOREST plus two trend strips (`AnnualReviewView`).
- The Work Plan is for the *next* job (D-010); the map is for *where*. Neither is a natural home for *when*.

A secondary entry point, **"History of this place"**, belongs on the ground report and the map side panel, so that history stays spatial.

## 2. Proposed structure

```
ANNUAL REVIEW · Year 6 complete                 [This year] [History] [Help F1]

THIS YEAR
  WORK DONE      (existing)
  MONEY          (existing)
  FOREST         (existing lines; jargon cleaned per ForestryTerminologyAudit)

WHAT CHANGED SINCE YOUR LAST INTERVENTION (Year 1)          ← new, derivable
  Basal area 38.3 → 48.9 m²/ha · regenerating cells 17 → 27 · light in the cells you opened 0.11 → 0.09

WHAT TO INSPECT NEXT                                          ← new, derivable, non-prescriptive
  · Cell C4: the oak you sheltered in Year 2 — shelter has 4 years left
  · Your Crop Trees P0707, P1614: neighbours have grown since Year 1
  · Cells B6–C7: Sitka seedlings now 1.2 m

OBJECTIVES (existing) · CENTURY REVIEW (existing)

[History tab]
  Timeline (intervention years + every 5th year)
  Compare: [Year 1 ▾] vs [Year 6 ▾]  → two-column table of the saved snapshot fields
```

### Rules for "What to inspect next"

- Each item must name a **place** (cell or tree id) and a **fact**. It must never name an action.
- Sources: management events (places of past work), shelters (expiry), regeneration diagnosis (limits), Crop Trees (marks), snapshot deltas.
- Maximum 3 items, so it guides without becoming a to-do list.
- Ordering is deterministic (by year of past work, then cell index), so the same save always shows the same list.

### Rules for "What changed"

- Compare against the last *intervention* year if there is one; otherwise against last year.
- Use only snapshot and report fields (authoritative now).

## 3. What stays out

- No score, grade or "forest health" index.
- No projection of the future (that belongs to practice mode, `LongTermTrainingFlow.md`).
- No claims of causation beyond `ForestDiaryConcept.md` §4.

## 4. Dependencies and ownership

| Item | Files | Save | Notes |
|---|---|---|---|
| History tab, compare, timeline | `UI/AnnualReviewView.cs` (+ a small `ForestHistory` read-only helper) | none | UI Toolkit owner |
| "What changed" / "inspect next" | same + read-only access to events, snapshots, shelters, marks | none | |
| Place history on the ground report / map | `UI/WalkingHudView.cs`, `UI/StandMapView.cs` | none | |
| Crop-Tree growth since designation | — | **new field** | Phase 2 (after Sol) |

## 5. Tests

1. History renders identically after save/load (derived from saved data).
2. "What to inspect next" never contains an imperative verb from a deny-list ("cut", "fell", "plant", "clear", "remove", "thin").
3. "What changed" uses the last intervention year when one exists.
4. The Reference Future preview shows no player history (isolation; precedent `referenceClean=true`).
5. Performance: building the timeline for 100 years takes under one frame budget on the reference hardware (100 snapshots; trivial, but test it).
