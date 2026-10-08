# Economy teaching audit (Workstream P)

**Status:** audit at `a8596df` [REPO] against the economics report [ECON] (*Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1*, local PDF, SHA-256 `fc51dc48e675951f345005aec4bf1c296954de61cc4f82dbf7b9566c34808741`). **No economy value is changed or proposed as a national rate.**

[ECON] principles used: Irish costs are **operation- and site-specific**; there is **no robust published Irish minimum call-out tariff** (the €2,500 is [C]); economies of scale make **small jobs expensive**; roadside accounting separates revenue from intervention cost; pruning premiums are **not guaranteed**; grants are a **policy layer**; owner labour is **time, not wage**; salvage costs more (×1.20 [I]) and windblown timber can **retain value**.

## 1. What the game models and what it teaches

| Topic | Model at `a8596df` [REPO] | Evidence class | What the player is taught now | Gap |
|---|---|---|---|---|
| **Contractor minimum** | `max(variable, €2,500)` once per year's harvest job (`MinimumHarvestJobCents`), sensitivity €1,500–4,000 in the price book | [C] ([ECON]: no Irish tariff) | Work Plan: "Small-job minimum" line; help: "A contractor's minimum job charge can make a small harvest expensive"; card note "This visit is small… Combining more trees into one visit spreads that cost" (P1 removes the "combine more trees" nudge) | Never said: it is a calibration, not a tariff; **the property is far below the scale where the minimum stops binding**; how that should shape *how often* to intervene |
| **Small-job inefficiency** | One job per year groups all fellings | [ECON] principle | Only through the minimum line | The real lesson ("frequent small visits multiply fixed costs") is never stated. On 0.16 ha it can only be experienced as "every thinning loses money" |
| **Timber assortment** | `TimberYieldCalculator`: pulp / stake / pallet / sawlog by small-end diameter and length; roadside €/t from IFA 2024 (€38 / €47 / €68 / €95 reference) | [E] prices, [C] selection | Work Plan table by assortment; inspection "If felled now" | Not explained *why* small Year-1 stems are mostly pulp, or that value rises with diameter. That is the economic reason CCF keeps good trees growing |
| **Sale basis** | Roadside (owner pays harvesting; buyer pays at roadside) | [ECON] recommended core | Implicit ("roadside value") | Standing sale, the common Irish arrangement for small owners, is absent (`FailureRecoveryDesign.md` §3) |
| **Harvest rates by intervention** | First / second / later thinning €21 / €23 / €20 per t (IFA) | [E] | Not shown | Fine to keep hidden |
| **Planting cost** | Sapling **€4.50 / €5.50** at purchase (definition, [D]) + 10 min labour (€7.50 contractor or owner time) | [D] | Nursery and planting card | **Inconsistency:** the price book holds the report's wholesale €0.95 / €1.00 for the same item ids, but the purchase uses the definition's €4.50 / €5.50. Neither is documented as "delivered small-order price". Teaching must not present either as an Irish rate. **Economy-owner decision** |
| **Shelter cost** | €5 material per shelter [D] ("NOT €2.56 grant allowance") + same executor as planting | [D] | Card line "€5.00 each (material…)" | Honest. Never framed against browse pressure (bands only) |
| **Clearance cost** | Contractor €45/h × (6 + 3 × cohort density) min, about €4.50+ per cell | [D] prototype hourly rate | Work Plan card | Cost is small: the lesson must come from **what is removed** (young trees), not money. Under Model 2: useful vs wasted vs harmful (€36 useful, €31.50 wasted, €7,586 repeated) [M2-CAND] |
| **Pruning** | €45/h × (8 + 2 × target height) min, about €9.75 / €13.50 / €15.75 per lift; **no premium** | [D]; [ECON] no guaranteed premium | P1 copy: "no timber price change in this scenario" (candidate) | Main says nothing about the missing premium. Pruning is cost with no visible return; honest, but it must be *said* |
| **Owner labour** | 40 h/yr, no wage; planting and shelters only; harvest contractor-only | [ECON] safety ([G] HSA) | Work Plan "Your time"; help | Good. Not stated *why* felling is contractor-only (safety) |
| **Deadwood opportunity cost** | Leave-as-deadwood removes that stem's sale revenue; work still costs | [ECON] "foregone revenue − avoided cost" | Deadwood lesson text | Good. Not shown as a number in the plan (could be: "leaving these 2 stems forgoes about €15 of timber") |
| **Salvage** | Not implemented | — | — | Storm packet: ×1.25 site multiplier proposed vs [ECON] ×1.20 [I]; `WindDamage` downgrading; decay window. Teaching later |
| **Grants** | Not implemented | [ECON] policy layer | — | Keep out of Scenario One unless a policy-layer decision is made. On 0.16 ha the WIS €1,200/ha is €192 |
| **Capital (value of standing trees)** | Not shown | Derivable (timber yield over standing trees) | — | The most important missing economic concept for CCF: *keeping* trees is keeping capital (`ResidualStandReview.md`) |

## 2. Key quantitative facts for teaching [REPO][HARNESS-Y0][ECON]

| Fact | Value |
|---|---|
| Notional roadside value of the whole Year-0 stand | ≈ €1,702 |
| Year-0 thinning outcomes, 15–87 trees | −€1,871 to −€2,366 net; the minimum binds in every case |
| Thinning profitability (Growth Model 1, 30 % from below) | Negative through scenario Year 25; ≈ +€495 at Year 40 (`EconomyTutorialImpact.md`) |
| [ECON] scale at which a €2,500 minimum stops binding (first thinning, 70 m³/ha) | ≈ 119 t ≈ **1.9 ha** |
| Scenario One area | **0.16 ha** (≈ 1/12 of that) |

**Interpretation:** inside Scenario One the player will *never* see a thinning pay for itself. That is defensible ([ECON]: "first thinning can be economically marginal"), but only if the game **says so** and frames early thinning as an **investment in the trees left**, not as income.

## 3. Findings

| # | Finding | Severity | Owner |
|---|---|---|---|
| E1 | The game never says why every thinning loses money (scale, not player error) | IMPORTANT | Teaching (copy) |
| E2 | Sapling price inconsistency between definition (€4.50/€5.50) and price book (€0.95/€1.00) | IMPORTANT (credibility) | **Economy owner** decision; teaching avoids quoting either as Irish |
| E3 | No representation of the value of what is kept | IMPORTANT | P3 (derivable) |
| E4 | Pruning has no stated payoff on main | POLISH | Copy (P1 has it) |
| E5 | The cash dead end (S1) is economic in origin | BLOCKER | Product + economy |
| E6 | Standing sale missing; it would be both a recovery route and a sale-basis lesson | PRODUCT DECISION | Economy owner |
| E7 | Deadwood opportunity cost not shown as a number | POLISH | P3 |
| E8 | The minimum is [C] but shown as fact | IMPORTANT | Copy: "In this scenario a contractor visit costs at least €2,500" (scenario rule, not national rate) |
