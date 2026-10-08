# P2 — Crop Tree and competitor reasoning in Tree Inspection (Workstream B)

**Status:** design proposal. Not decision authority. Builds on the pedagogy-branch proposal (`PositiveSelectionTeaching.md`, `60674f1`), re-checked against `a8596df`.

**Teaching core:** *Start with what you want to keep. Then find which neighbours actually compete with it.*

## 1. What is authoritative today [REPO]

| Quantity | Authoritative? | Where |
|---|---|---|
| Competition index CI = Σ over living neighbours within **8 m** of `(DBH_neighbour / max(1, DBH_target)) / max(0.5, distance)` | **Yes**; drives DBH growth (`1/(1+CI/Ci50)`), Sitka Ci50 = 5 | `ForestEcologyController.HegyiTerm`, `HegyiCutoffMeters = 8` |
| Each neighbour's term in that sum | **Yes** (same function, deterministic) | `HegyiTerm` is public and static |
| Share of potential DBH growth withheld | **Yes** | `GetCurrentSuppression` |
| Distance, DBH, height of each neighbour | Yes | tree state |
| Crown overlap / crown contact | **No.** Crown radius drives cell light only, never competition | `CanopyShadeReachPerCrownRadius` is cell-light only |
| Light at a tree's own crown | **No.** Light is per 5 m ground cell | cell `Light` |
| Height relationship (dominant / suppressed) | Height is authoritative, but **not** an input to CI | Growth Model 1: height follows a site curve; CI uses DBH |

**Consequence:** the only honest "competition contribution" is the Hegyi term. A crown-relationship display would invent a mechanism the simulation does not use (D-020). Height may be shown as **context**, labelled as such, never as the reason.

## 2. What the Year-0 stand actually looks like [PROTO], [HARNESS-Y0]

`year0_release_and_pattern.py` recomputes CI for all 336 trees from positions and DBH. It reproduces Unity's CI exactly for all 16 fixture Crop Trees (mean 6.0925 vs 6.0924). 9 non-crop trees differ by one boundary neighbour (0.07–0.21), because positions are rounded to 2 dp at the 8 m cutoff.

| Fact (16 fixture Crop Trees, Year 0) | Value |
|---|---|
| Neighbours inside 8 m | min 10, **median 39**, max 48 |
| Largest single neighbour's share of CI | min 7.0 %, **median 8.8 %**, max 22.9 % |
| Top three neighbours' combined share | min 18.2 %, **median 22.6 %**, max 47.7 % |
| Neighbours with ≥ 10 % share each | **median 0**, max 3 |

**Design consequence:** in this dense 2,100 stems/ha plantation, competition around a Crop Tree is **diffuse**. Usually no single neighbour dominates. A design that says "this is *the* competitor" would be false for most trees. The display must show the *distribution*: a few larger contributors plus a long tail. That distribution is itself the lesson: *release usually means removing several neighbours, and closer, larger ones count most.*

## 3. What the player should see

When the inspected tree is a **Crop Tree** (or any tree, from a "Show neighbours" toggle):

Real Year-0 values for fixture Crop Tree **P0707** (DBH 20.7 cm, CI 8.15) [PROTO, `Evidence/crop-neighbour-ranking.csv`]; the mark column is illustrative:

```
NEIGHBOURS COMPETING WITH THIS CROP TREE
Competition here comes from 48 trees within 8 m. Closer and larger
neighbours count most. No single neighbour dominates this tree.

  #  Tree   Distance  DBH      Share of competition   Mark
  1  P0706  1.5 m     20.5 cm  ████   8 %             —
  2  P0607  1.6 m     19.9 cm  ████   7 %             ■ Fell
  3  P0807  2.2 m     17.8 cm  ██     5 %             —
  4  P0806  2.5 m     18.2 cm  ██     4 %             —
  5  P0708  2.4 m     17.1 cm  ██     4 %             —
     43 other trees together                72 %
  Marked to fell: 1 neighbour, 7 % of this tree's competition.
```

P0707 is a deliberately hard case: its competition is very diffuse. The fixture Crop Tree with the most concentrated competition has one neighbour at 23 % and three neighbours with ≥ 10 % each.

Copy rules:

- Use: "This neighbour contributes strongly to competition around your Crop Tree." "Smaller and more distant trees contribute little."
- **Never:** "cut this", "remove", "should", "best", "correct", or any ranking label such as "worst competitor".
- Show **another Crop Tree** among the neighbours explicitly (◆). Two Crop Trees competing is a real decision (choose, or accept).
- Show "Marked to fell: n neighbours, x % of this tree's competition" as feedback on the player's *own* marks.
- Height can appear as a context column ("taller / similar / shorter") **only if** it is labelled "context; not part of the competition calculation". Recommendation: omit it from v1 and avoid the confusion.

## 4. Presentation options compared

| Criterion | A. Ranked list in the inspection card | B. World-space highlighting only | **C. List + temporary in-world tags (recommended)** |
|---|---|---|---|
| **Learning value** | Medium. IDs ("P0706") are hard to find among 336 identical spruce. The player must match an ID to a trunk | Medium. Spatial, but without numbers the player cannot tell 12 % from 5 % | **High.** The list gives magnitude; the tag shows *which trunk*. Direct link between reasoning and the 3D forest (D-004) |
| **Clutter** | Low. Inside an existing card | Medium–high if all 39 neighbours are tagged | Low–medium. Tag **only the listed top 5** while inspection is open; remove on close |
| **Prescriptiveness risk** | Low | **Higher.** Highlighted trees read as "targets" | Medium. Mitigated by neutral colour (not red), numbers instead of colour alone, and the "share" framing |
| **Implementation effort** | Small. New card section in `TreeInspectionView`; static helper using `HegyiTerm` | Medium. World markers, lifecycle, occlusion | Medium. A + one world-marker helper reusing the `ClearancePreview` LineRenderer pattern (raised numbered stem marker) |
| **Maintenance** | Low | Medium | Medium. One helper and one lifecycle rule ("only while inspecting") |
| **Performance** | O(n) over living trees per inspection refresh. Negligible (336 trees; the card rebuilds only when its key changes) | Same plus ≤ 5–39 line renderers | Same plus ≤ 5 line renderers. Negligible |
| **Accessibility** | Text with bars; numeric | Colour-dependent unless numbered | Numbered tags plus bars. Not colour-only |

**Recommendation: C**, constrained as follows:

1. Tags appear **only while Tree Inspection is open on a Crop Tree** and only on the listed top five neighbours. Numbers 1–5 match the list.
2. Tag colour is neutral (pale gold), distinct from red Fell and blue Crop marks. No tag says "fell".
3. A neighbour that is already Fell-marked keeps its red mark; its tag number still shows.
4. The tags vanish on close, on any modal screen, and in the Reference preview (same lifecycle rules as the clearance preview).
5. **Ship A first** inside the same packet and gate B on a manual review. If the review finds tags read as instructions, ship A alone. (This replaces pedagogy decision #14, "defer highlighting", with a concrete, reversible test.)

## 5. Also in this packet (small, same files)

- **Release line on the inspection card** for a Crop Tree with Fell-marked neighbours: "If your marked neighbours are felled: competition around this tree falls from 6.1 to 4.9 (−20 %). About 23 % → 19 % of its potential diameter growth would be withheld." Same formula as `GetCurrentSuppression`, recomputed without the marked trees. Labelled "estimate, if nothing else changes" (`CropTreeReleaseMetric.md`).
- **Forecast split** in the marking forecast line: Crop Tree figure separate from the stand average (the pedagogy harness found the two can invert: T1 novice clean-up gives the larger *stand* gain, T2 crop-tree release the larger *Crop Tree* gain [HARNESS-Y0]).

## 6. Not in this packet

- Wind labels: wait for the storm packet's Stable/Watch/Exposed bands.
- Competition label thresholds: replaced in the card by the numeric "% growth withheld" sentence that already exists. Thresholds are untouched.
- Form/quality-based Crop Tree choice: no stem-form state exists (UNSUPPORTED). One honest sentence only: "This scenario does not model stem form; in a real forest you would also judge straightness and crown health."
