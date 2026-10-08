# Stand boundary / edge decision for Scenario One

**Status:** analysis and recommendation. **PRODUCT DECISION REQUIRED** on what surrounds the property. Nothing implemented.

## 1. The situation today [REPO]

- The simulated stand is 40 × 40 m (0.16 ha, 8 × 8 cells of 5 m). The ecology knows only trees inside it. The scene has terrain ridges ("West Ridge", "South Ridge") and a forest road along the south, but **no trees or state outside**.
- D-041 (Confirmed): Scenario One is a **genuinely bounded property**.
- Competition uses an 8 m Hegyi cutoff, so trees within 8 m of the boundary see only part of their neighbourhood. About **64 % of the stand area** lies within 8 m of an edge [I: (40² − 24²)/40²].
- `Docs/EdgeBias.md` (an earlier calibration with a 20 m cutoff and Ci50 3) measured edge trees at about 55 % of their interior competition. **That study predates C8 and must be re-run** before its magnitude is quoted again; the direction stands.
- Canopy light (`RecomputeCanopy`) and seed rain sum only trees inside the stand, so cells at the stand edge receive no shade or seed from outside.
- Wind (`GetWindRisk`) has no edge term.

## 2. Options

| | A. Plot = interior of a larger forest | B. Genuinely bounded forest with defined surroundings | C. Explicit buffer / surrounding state |
|---|---|---|---|
| Meaning | The 40 m plot is a window; outside is "more of the same" stand | The property ends here; outside is something specific (road, field, neighbour's forest) | A ring of simulated or parametric trees/land around the property |
| Competition | Needs periodic/mirrored neighbours, or a correction term | Edge trees really are less crowded on open sides; current behaviour is *correct* for open sides and wrong for forested ones | Correct by construction |
| Canopy / light | Mirror shading needed | Open sides: correct. Forested sides: too bright | Correct |
| Seed | Outside seed rain like inside | Depends on surroundings (a neighbour's Sitka forest = seed source; a field = none) | Correct |
| Wind exposure | No outer edge effect | **Outer edges are long-established edges**: open sides are exposed but acclimated [I]; forested sides are sheltered | Correct, including which edges are newly exposed |
| Storm vulnerability | Only management-created openings matter | Depends on side; the road side and an open field side differ from a neighbour's forest | Most faithful |
| Fit with D-041 | **Conflicts** (D-041 says bounded) | Matches | Matches; heavier |
| Cost | Changes competition/light/seed anchors | For wind only: small (per-side descriptor). For competition/light/seed: anchor changes | Large: new state, save, performance, anchors |

## 3. Recommendation for Scenario One

**B, applied to wind only in Storms v1, with a fixed per-side surroundings descriptor.**

1. Add a **scenario-authored, unsaved descriptor of the four sides** (definition data, not world state). For example: `north: neighbouring conifer forest (sheltered)`, `east: …`, `south: forest road + open ground (open)`, `west: ridge / open pasture (open)`. Values are a product/authoring choice.
2. In v1 the descriptor affects **only storm vulnerability**:
   - **Outer edges on "open" sides are established edges:** a small, constant exposure term, acclimated (no recent-opening term). Rationale [I]: trees that grew on a long-standing edge are generally windfirmer than trees suddenly exposed. Edge exposure matters mainly when it is *new*.
   - **"Sheltered" sides** add no exposure.
   - **New internal edges** come from management or storms and are handled by the recent-opening term (`RecentThinningAssessment.md`).
3. **Do not change competition, light or seed boundary behaviour in the storm packet.** Those edge effects are real model biases, but fixing them changes every lifecycle anchor. They need their own packet (re-run the edge-bias diagnostic under C8 + Growth Model 1 first).
4. If the descriptor decision is not made in time, **default every side to "sheltered" (neutral)**. Storm v1 then depends only on height, H/D, dominance and management-created openings. This is the safest default: it adds no exposure the player cannot see or influence.

## 4. Decision needed

**PRODUCT DECISION REQUIRED — "What surrounds Scenario One's property on each side?"** It is a content question with a gameplay consequence:

- An exposed south/west side makes the road edge and the west boundary the natural first storm-damage locations. That is legible, because the player walks the road, but it is a pre-set disadvantage.
- A fully sheltered property makes storm damage purely a consequence of stand development and the player's own openings.

Recommended starting point: **south = open (road + field), other sides = sheltered**. It matches the visible scene (road and open ground to the south) and gives one legible, pre-existing exposed edge.
