# Management history on the Stand Map (Workstream I)

**Status:** design proposal. **The map stays diagnostic and navigational.** It never marks, plans or approves (D-010, `StandMapView` header comment).

## 1. Proposal

The selected cell's side panel gains a short **History** list under the existing diagnosis:

```
CELL D6
Light 0.31 (moderate) · Regeneration: Sitka seedlings 0.6 m · Sessile oak ×11 (planted)
Why: light is sufficient for continued growth.

HISTORY
  Year 1   2 trees felled (thinning)
  Year 6   12 oak planted, 12 shelters
  Year 10  1 tree died from crowding (fallen deadwood)
  Year 12  Vegetation cleared (contractor)
  [Storms] Year 17  Storm: 4 trees blew down; new opening
  Year 20  3 trees felled (thinning)

[Set waypoint]  — go and look
```

## 2. Derivability per example line

| Example | Source | Available |
|---|---|---|
| "Year 5 thinning" | `WorkResolved` FellTree events with `cellIndex` | **Now** |
| "Year 6 planting / shelters" | PlantJuvenile events with cell; order `installShelter` | **Now** |
| "Year 9 regeneration appeared" | Per-cell regeneration history | **Not saved.** Show "Regeneration present now" only. A per-cell first-seen year needs a new saved field (deferred, `ForestDiaryV1.md` §4) |
| "Year 12 clearance" | RemoveRegeneration events with cell | **Now** |
| "Year 10 died from crowding" | Tree `mortalityCause`/`mortalityYear` + position → cell; deadwood record cell | **Now** (Growth Model 1) |
| "Year 17 storm opening" | Tree cause `windthrow` + year → cell; `stormEvents[]` | **After storms** |
| "Year 20 second intervention" | Events | **Now** |

## 3. Rules

| Rule | Reason |
|---|---|
| Facts only, newest last, at most 8 lines with "older…" collapse | Readable at 1280 |
| No buttons except **Set waypoint** | The player must still walk there |
| Optional **History** map layer: cells with any management event get a small dot. The number is the count of years with work. Colour plus numeral | Shows *where* the player has worked without making the map an editor |
| No "time slider" that re-renders the past forest | Reference preview already exists for frozen comparisons; a replay of the player's own past would need saved per-year world states (not available) |
| Derived per open of the panel; nothing cached in save | No save impact |

## 4. Why not more

- A map timeline scrubber or past-state overlays would need per-year cell state (not saved) and would shift learning from walking to the map.
- Ordering work from the map (e.g. "re-plant this cell") breaks D-010.

## 5. Implementation shape

A read-only `ForestHistory` helper (shared with Forest Diary and Annual Review PLACES) returns `IReadOnlyList<HistoryEntry>` for a cell or for the property. `StandMapView.BuildSide` appends entries. Tests: entries equal the events whose cell matches; identical across save/load; empty for the Reference preview.
