# Understorey Dynamics — calibration diagnostic

**Status: diagnostic only.** No runtime understorey mechanic changed. Task: Regeneration Bottlenecks v1A, optional Phase P. Branch: `task/regeneration-browsing-protection-v1`.

Source: `Understorey_Dynamics_Irish_CCF_Report.pdf` (1 Oct 2026), coupled to Browsing & Protection v1 (`Docs/BrowsingProtectionV1.md`).

Tool: `Tools/Diagnostics/understorey_dynamics_diagnostic.py`. It is deterministic, uses only the Python standard library, and is run with `python3` (no Unity). It reproduces every figure below.

Tags: [E] empirical (Irish unless "transferred"), [G] guidance, [I] inference, [S] simulation abstraction, [C] calibration.

## Current runtime state (inspected at `bb314a4`)

`ScenarioOneUnderstorey` (Scenario One only) holds six provisional [D] groups per 5 m cell: mosses, ferns, grasses, forbs, shrubs and fungi.

- Each group's target comes from light, site, stability and recent opening.
- The annual step uses Lerp toward the target with colonisation rate 0.3 and loss rate 0.45 (`ScenarioOneDefinition`).
- The groups are saved (`ScenarioUnderstoreyCell`). They drive habitat presentation and score.
- They do **not** feed tree establishment, growth, survival or browsing.
- There is no bramble or bracken group.
- `ForestEcologyController` (natural cohorts) cannot see them, because they live in `ScenarioOneManager`.

## Candidate causal relationships tested

Model forms are taken from the report (§10–12). Coefficients are candidates [S]/[C] unless marked.

| Relationship | Candidate form / values | Tag |
|---|---|---|
| Target cover by relative light | Piecewise anchors per group. Bramble ≈0.20 at RLI 0.06 (transferred empirical anchor: ~20% cover at 5–7% light); bracken strong at 0.15–0.30; graminoids minor below 0.15, strong above 0.30 | [E] bramble anchor; [E transferred] bracken shape; [G]/[S] graminoids |
| Approach to target | `cover += rate × (target − cover)`. Rise: bramble 0.35, bracken 0.30, graminoid 0.50. Fall: 0.15 / 0.08 / 0.30 | [C]. Fall < rise gives the required hysteresis |
| Source gate | Bracken needs its own rhizome source or neighbour influx; graminoids/bramble assume a local source | [E transferred] for bracken persistence; [S] |
| Neighbour spread | Bracken 0.25, bramble 0.10 × neighbour cover excess × 0.5 | [E transferred] 1–2 m/yr rhizome spread; coefficient [C] |
| Shared competition | `Σ cover × strength × clamp(1 − h/overtop)`, capped at 0.8. Strength: bramble 0.6, bracken 0.7, graminoid 0.5. Overtop: 1.2 / 1.5 / 0.5 m | Ordering per report table; values [C]/[S] |
| Competition route | (a) multiply height growth (report equation), or (b) reduce the juvenile's effective light, feeding the existing light growth and survival responses | (b) is [I]: bramble light interception [E transferred]; mortality "only via prolonged suppression through existing survival" |
| Bramble concealment | `vegetationExposure = 1 − min(cap, cover × min(1, brambleHeight/treeHeight))`, with derived bramble height 0.2 + 0.8 × cover m. Cap 0.5 (report default) or 0.78 (Killarney ratio) | Direction [E]; Killarney 11% vs 49% browsed [E] ⇒ multiplier ≈0.22; value [C] |
| Spot control | 1 m² spot cleared to 0 for the season, regrowing to cell cover over 2 years | [G] 1 m-diameter spot treatment; regrowth [C] |

Browsing terms use the implemented v1 values: cap 0.95, increment loss 0.9, extra mortality 0.04, oak palatability 1.0, taper 1.2→1.8 m.

## Results

### A. Closed spruce vs small gap (cover at years 5 / 20 / 50)

| Group | Closed (RLI 0.02) | Small gap (0.25, closing to 0.10 from year 10) | Gap, no bracken source |
|---|---|---|---|
| Bramble | 0.09 / 0.10 / 0.10 | 0.53 / 0.43 / 0.29 | same |
| Bracken | 0.05 / 0.05 / 0.05 | 0.48 / 0.42 / 0.21 | 0 / 0 / 0 |
| Graminoid | 0.01 / 0.01 / 0.01 | 0.26 / 0.10 / 0.06 | same |

Closed cells stay sparse, consistent with the Irish WL5A Sitka baseline [E]. Gaps respond only where a source exists, and vegetation declines with a lag as the gap closes. This passes the report's experiment A (no convergence to light-only cover).

### E. Canopy-closure hysteresis (RLI 0.40 for 8 years, then 0.05)

| Group | Peak | Years to half after closure | Cover 10 years after closure |
|---|---:|---:|---:|
| Bramble | 0.73 | 7 | 0.28 |
| Bracken | 0.71 | 10 | 0.35 |
| Graminoid | 0.48 | 3 | 0.03 |

Bracken persists longest (rhizome), bramble lingers (shade tolerant), and graminoids drop quickly. The current runtime loss rate of 0.45/yr on every group would halve any group in about 1.2 years. That is far less memory than the report requires.

### D. Bracken spread (RLI 0.35, source in cell 0 of a six-cell row)

Year 3: 0.68 0.50 0.05 0 0 0. Year 10: 0.72 0.72 0.70 0.66 0.56 0.30. Year 20: all 0.72.

Spread is gradual, not instant. The front moves about 2.5 m/yr, slightly faster than the transferred 1–2 m/yr. **Recommend a spread coefficient of ~0.15**, not 0.25.

### B/C. Oak juvenile at RLI 0.35 with browsing (expected-value juvenile)

| Case (planted 0.6 m) | Pressure | Fenced | Promotion yr | Survival | Browse events (30 yr) |
|---|---:|---|---:|---:|---:|
| No vegetation | 0 | – | 14 | 1.00 | 0 |
| Uncleared vegetation | 0 | – | 14 | 1.00 | 0 |
| No vegetation | 0.85 | no | 26 | 0.58 | 13.2 |
| Uncleared vegetation | 0.85 | no | 18 | 0.84 | 4.2 |
| Spot-cleared every 2 years | 0.85 | no | 23 | 0.66 | 10.4 |
| Uncleared vegetation | 0.85 | yes | 14 | 1.00 | 0 |

For natural seedlings (0.18 m), vegetation costs about one year when fenced (16 vs 15). Under pressure 0.85, unvegetated seedlings never recruit (survival 0.44), while vegetated ones recruit at year 21 (survival 0.80).

**Finding.** With the report's candidate magnitudes, bramble concealment dominates and competition is weak: the trade-off sign is "vegetation helps" whenever browsing is meaningful. Killarney's longer-term result points the other way. There, weeded oak was browsed far more, yet later survived better because competition outweighed concealment [E]. The candidate magnitudes do **not** reproduce that.

### Killarney sweep (seedling 0.18 m, dense bramble 0.8 + graminoid 0.5, light-interception route, 25 years)

| Light | Concealment cap | Competition × | Fenced survival: weeded / vegetated | Unfenced (0.85) survival: weeded / vegetated | Direction |
|---|---:|---:|---|---|---|
| 0.20 | 0.50 or 0.78 | 1.0 | 1.00 / 0.26 | 0.42 / 0.07–0.17 | weeded better |
| 0.35 | 0.50 | 1.0 | 1.00 / 1.00 | 0.44 / 0.79 | vegetated better |
| 0.35 | 0.50 | 1.5 | 1.00 / 0.26 | 0.44 / 0.09 | weeded better |
| 0.35 | 0.78 | 1.0 | 1.00 / 1.00 | 0.44 / 0.90 | vegetated better |
| 0.35 | 0.78 | 1.5 | 1.00 / 0.26 | 0.44 / 0.18 | weeded better |

**Finding.** The Killarney direction appears in marginal light, or once competition is about 1.5× the candidate strengths. The response is close to a **switch**: between 1.0× and 1.5× at RLI 0.35, vegetated survival drops from 0.79–0.90 to 0.09–0.26. The cause is the binary poor-light survival rule used for the approximation (20%/yr below light response 0.15). Routing competition through light, as the report intends, will be calibration-fragile unless the survival response is continuous near the threshold. Oak already has a continuous distinct survival curve; Sitka and beech use the legacy binary rule.

### Concealment and competition shapes

Exposure multiplier, with cap 0.5:
- bramble cover 0.4: 0.60 at h 0.3 m, 0.65 at 0.6 m, 0.79 at 1.0 m, 0.86 at 1.5 m;
- bramble cover ≥0.7: capped at 0.50 up to 1.0 m.

Growth removed by dense bramble 0.8 + graminoid 0.6:
- 0.58 at 0.2 m, 0.28 at 0.5 m, 0.16 at 0.8 m;
- 0 from 1.2 m.

The continuous overtop taper behaves as the report requires.

## Bounded recommendation for the next production task

**Understorey v1: three causal covers + shared juvenile competition and concealment.**

1. **State.** Add `brambleCover`, `brackenCover` and `graminoidCover` per 5 m cell, persisted, alongside the existing provisional groups. The provisional groups stay presentation/habitat. This is a **save-schema change (v15)**. It should be bundled with the browsing v15 fields (browse history, form damage, protection records) so there is one approved bump.
2. **Ownership.** The covers must be readable by `ForestEcologyController` for natural cohorts, not only by `ScenarioOneManager`. Expose per-cell `competition` and `vegetationExposure` inputs to the ecology the same way `BrowsingConditions` does, so both juvenile adapters call one shared rule.
3. **Dynamics.** Use the target-cover anchors above, rise/fall rates (bramble 0.35/0.15, bracken 0.30/0.08, graminoid 0.50/0.30), a bracken source gate, and bracken neighbour spread ≈0.15. Do not reuse the current 0.45/yr loss rate for causal groups.
4. **Competition route.** Reduce the juvenile's effective light by `min(0.8, Σ cover × strength × overtop taper)`. This feeds the existing growth and survival responses, so there is no new mortality roll. **Before production, replace the binary poor-light survival** used by Sitka and beech with a continuous response near the threshold, or the trade-off will act as a switch.
5. **Concealment.** Bramble only. `vegetationExposure` cap is calibrated between 0.5 (report default) and 0.78 (Killarney 11/49). It plugs into the existing Browsing v1 `vegetationExposure` input; no browsing change is needed.
6. **Spot control.** Keep the 1 m² clearance as "spot vegetation control". It lowers local cover for one season with regrowth over ~2 years, and also removes local concealment. Labour, cost and herbicide policy belong to work/economy and scenario policy data.
7. **Acceptance experiments**, before tuning beyond the bounds above:
   - report A, C, D and E as shown here;
   - Killarney B: weeded browsed more, but surviving better in the long term under the chosen calibration at marginal light;
   - an explicit check that a fenced, uncleared cohort shows competition only.

**Out of scope for that task:** rush/sedge causal state, bilberry, holly/hazel as cover, moss effects, invasive module, explicit litter classes, bracken concealment.

## Limits of this diagnostic

- It is an expected-value juvenile with fixed light. No canopy feedback, no cohort density, and no per-individual stochasticity.
- Oak survival is approximated by the legacy binary poor-light rule, not oak's distinct curve.
- All cover-response numbers except the bramble 5–7% anchor are band-derived starting points. Irish annual response curves do not exist in a form suitable for direct calibration (report §18).
