# Scenario One completion (Workstream N)

**Status:** design options and a recommendation. **PRODUCT DECISION REQUIRED.** Any change alters the completion anchor (`D7C4DDD36B53FCCE`, current stack) and `ScenarioOneCompletionVerification`.

**What completion should mean:** the player has demonstrated repeated, selective, reasoned management, and the forest is still a continuous-cover forest with renewal under way, whatever exact structure they chose.

**Must not require:** an inverse-J distribution, fixed species percentages, a fixed basal area, or one ideal CCF pattern.

## 1. Facts that constrain the options [REPO][INF]

| Fact | Consequence |
|---|---|
| Canopy, Sitka count, natural Sitka regeneration and (Growth Model 1) deadwood are met **without management** by Year 25 | State thresholds alone cannot evidence competence |
| Natural Sitka **recruitment** to tree size happens by Year 25 even unthinned (Regeneration Model 1 verification: 20 / 33 / 75 recruited trees at Year 25 for unthinned / 20 % / 60 % removal, under growth 0) | "Successful later recruitment" is also near-automatic. It is usable only as a guardrail |
| Broadleaves have no seed source; planted Beech/Oak bear seed only from age 40 | Broadleaf *natural* recruitment cannot be a Year-25 criterion |
| Multiple age classes exist by default (age-20 plantation + Sitka seedlings) | "Multiple age classes" is not evidence either |
| Every thinning to about Year 25–40 loses money (€2,500 minimum, 0.16 ha) | "Positive finances" cannot mean profit. At most "not stuck" |
| The completion gate completes at Year 25 with lowest cash €6,139.75 | A solvency rule must not break the reference path |

## 2. Alternative completion models

| Model | Definition | Strengths | Weaknesses |
|---|---|---|---|
| **A. Current counters** | 8 objectives (`ScenarioOneObjectives`) | Simple; anchored; verified | 5 auto-complete; others count one act; mandatory two-species planting; no repetition |
| **B. Competence checklist** | All learning stages S1–S12 completed | Directly rewards learning actions | Rewards *doing steps*, not forest outcomes; many stages are session-observed; can be gamed by rote |
| **C. Forest-state targets** | Thresholds on canopy, regeneration, broadleaf presence, size diversity (e.g. DBH CV), deadwood | Outcome-focused | Mostly auto-met (§1). Raising thresholds turns them into a prescribed structure: exactly what the brief forbids |
| **D. Two cycles + guardrails + renewal (recommended)** | Demonstrated management (two release cycles) **and** forest guardrails held throughout **and** a deliberate species introduction surviving | Evidence of repeated selective management; no prescribed structure; uses saved data; storm-compatible | Needs the release definition (§3) and per-year guardrail checks; changes the anchor |
| **E. Player-declared goals** | The player chooses an emphasis (timber / habitat / regeneration) and is evaluated against their own goals | Strongest agency; matches Pro Silva's "judge by your own objectives" | Needs a goal-setting UI, a purpose-note save field, and per-goal evaluation; large; risks a hidden score. Future (Scenario Two or practice mode) |

## 3. Recommended model D — definition

Completion at the first year-end where **all** hold:

| # | Condition | Definition (all from saved data) | Why |
|---|---|---|---|
| D1 | **Horizon** | Year ≥ 25 | Consequences of the first cycle are visible; existing horizon |
| D2 | **First release** | A resolved thinning in which ≥ 1 felled tree stood within 8 m of a then-living Crop Tree | Positive selection, demonstrated. No amount specified |
| D3 | **Second cycle** | Resolved management work (any type) ≥ 5 years after D2, after the second-look trigger (`SecondInterventionDesign.md`), followed by a read Annual Review | Repetition |
| D4 | **Continuous cover held** | Mean canopy ≥ 0.35 in **every** recorded year (snapshots) | Guardrail against clear-fell or collapse. The same threshold as now, checked throughout |
| D5 | **Standing capital held** | ≥ 60 original-plantation trees (P-prefixed) living **or** basal area ≥ 20 m²/ha [C]; whichever product prefers | Guardrail against liquidation. The P-prefix matches the Century Review's definition |
| D6 | **Renewal under way** | Regeneration present in ≥ 3 cells (any origin) **and** ≥ 1 player-introduced broadleaf alive ≥ 5 years after planting | Natural renewal plus a deliberate species introduction; no species list |
| D7 | **Reviews used** | ≥ 2 Annual Reviews read (first acknowledgement plus the review after D3) | Teaching; minimal |

**Removed:** the deadwood volume objective (auto-met; reported under HABITAT) and "both Beech and Oak" (replaced by D6).

**Storm compatibility:** a storm may reduce canopy or original trees. D4 and D5 must be calibrated so a severe storm does not by itself fail a reasonable plan. Storm decision #22 recommends calibrating storms, not changing objectives. If a storm breaks D4 in a year, D4 should count years with canopy below 0.35 **not caused by player felling** as held (derivable: canopy loss from windthrow deaths that year). Decide with the storm packet.

## 4. Completion presentation (no score)

```
SCENARIO ONE — YOUR FOREST AT YEAR 25
You have managed this forest through two cycles, kept continuous cover and
started a new generation. This is one way the forest can develop — not the only one.

ECONOMY       Cash €6,140 · 2 harvest visits · timber sold 14.2 m³ · minimums paid €4,800
SILVICULTURE  16 Crop Trees · competition around them 6.1 → 4.6 · mean DBH 20.2 → 29.1 cm
REGENERATION  Sitka in 41 cells · 9 of 12 planted oak alive, 3 above browsing height
STRUCTURE     336 → 288 trees · DBH variation 0.17 → 0.24 · 2 age layers present
HABITAT       Fallen deadwood 3.1 m³ (12 logs; 4 left by you, 8 from crowding)
STABILITY     [after storms] One moderate storm, Year 13: 9 trees fell

[Compare with Reference Future at Year 20 / 50 / 100]   [Continue managing to Year 100]
```

Figures illustrative (format). Each line is a dimension from `MultidimensionalFeedback.md`. There is no total and no "grade". The Reference Future comparison stays explicitly "not an optimum" (D-018).

## 5. Verification for the implementing packet

- Completion gate re-run under the current stack with a reference plan that satisfies D. Record the **new** anchor, with explicit authorisation.
- Negative controls: unmanaged (fails D2/D3/D6); one thinning only (fails D3); clear-fell (fails D4/D5); planting with no survival (fails D6).
- Calibration: the share of 5-year planted broadleaf survival under browse 0.2 with and without shelters (D6 feasibility). If unsheltered survival is near zero, D6 implicitly requires shelters. That is acceptable only if stated.
- Under storms: completion viability across seeds or storm schedules (storm decision #22).
