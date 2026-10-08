# Player feedback before and after storms

**Status:** proposal. Production UI is not changed by this task.

## 1. Replace "Wind exposure: high"

The current label is saturated: every tree reads "high" (`CurrentWindSystemAudit.md` §4). Replace it with a **three-band stability state from the new vulnerability index V**, plus the reason. No percentage.

| Band | Rule (relative to V; [C] thresholds set after calibration) | Inspection wording |
|---|---|---|
| **Stable** | V below the 50th percentile of a calibration reference stand (e.g. unthinned dominant at Year 15) | "Stability: stable" |
| **Watch** | between the reference and 2 × reference | "Stability: watch — tall and slender" |
| **Exposed** | above 2 × reference | "Stability: exposed — neighbours removed recently" |

Bands are anchored to a **fixed reference value** (not the current stand's own distribution), so a whole stand *can* become "watch" as it grows tall and slender. That is honest: the IRL report says risk rises continuously with height.

**Reason line:** the largest factor, translated:

| Dominant factor | Text |
|---|---|
| Recent | "Neighbours were removed in the last few years. Exposure fades as trees adapt." |
| Slender | "Tall for its diameter (H/D 86). Stouter trees are more stable." |
| Load/height | "Among the tallest trees here." |
| Edge | "On an open edge facing the prevailing wind." |
| (stable) | "Sheltered by neighbours." |

H/D is shown as a number only next to the tree's height and DBH ("Height 23 m · DBH 27 cm · H/D 85"), as context, never as a verdict.

## 2. Marking forecast

Replace "wind peak 29.7 (high)" with:

- "Stability after this thinning: 6 trees move to **exposed** for a few years (mostly in cells C3, D3)."
- No probability, and no claim that a storm will occur.

## 3. Stand Map

Optional fifth layer, **"Stability"**: per cell, the count of watch/exposed trees, plus a marker on cells opened in the last 3 years. Same rule as other layers: it shows where to look, not what to do.

## 4. Seasonal context (no false precision)

The HUD never shows a storm chance. The scenario intro and help can say: "Storms are part of Irish forestry. Some years bring damaging winds; you cannot predict which."

## 5. Annual Review after a storm

See `AnnualReview` section below (packet requirement 13).

### Storm section (only in a storm year)

```
STORM — Year 17
A severe storm from the south-west.
26 trees blew down (8.4 m³; 3 were Crop Trees: P0707, P1103, P1614).
Most damage: cells C3, D3, D4 — where you thinned in Year 15 — and the south edge.
New openings: 2 (largest about 6 cells). Light on the ground rose to 0.31 in D3.
Windthrown stems are lying deadwood until you decide otherwise.
You can salvage them for the next 5 years: mark stems with X in the forest,
or review them in the Work Plan.
Worth inspecting: D3 (largest new gap), P1614's neighbours, the south edge.
```

Rules:

- Facts from authoritative state: storm record, victims (cause/year), deadwood records, cell light, events. "Where you thinned in Year 15" only when management events exist in those cells within the last 5 years (co-occurrence wording, not causation).
- "Worth inspecting" names places, never actions (same rule as the pedagogy history proposal).
- **No line says "your thinning caused this".** At most: "Trees near recent thinning were among the damaged." This is true and does not over-claim.
- Salvaged / left as deadwood counts appear in later years' WORK DONE and FOREST columns.

## 6. Physical revisit

A waypoint button on the storm section ("Set waypoint to the largest new gap") uses the existing map waypoint mechanism. The player walks there and sees fallen stems, root plates, the light change and, later, regeneration.
