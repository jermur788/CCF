# Scenario One — final learning outcomes (Workstream B1)

**Status:** proposal [INF] built on supporting evidence [PRAC]/[EMP]. **Not an accepted decision.**

By the end of Scenario One, a complete novice should be able to explain each outcome below *in their own words*, and should have **done** something in the game that demonstrates it.

Each outcome records:

- **Basis** — the evidence class behind the claim.
- **Sim support** — can the *current* simulation (`3e4ee40`) demonstrate it truthfully?
  - **SUPPORTED** — authoritative state already shows it.
  - **PARTIAL** — shows part of it; the rest needs honest wording.
  - **NOT SUPPORTED** — the tutorial must not claim it causally yet.
- **Demonstration** — what the player does.
- **Observable evidence** — what the game can check without a quiz.

Where the sim does not support an outcome, the tutorial should say so in plain words ("this version does not simulate X"). It must not fake it (D-020).

---

| # | Outcome (player can explain…) | Basis | Sim support | Demonstration | Observable evidence |
|---|---|---|---|---|---|
| O1 | **What CCF is:** managing by repeated selective work on trees and small groups, keeping cover and letting renewal happen | [PRAC §2.2, §11.1] | SUPPORTED (as framing) | Read the opening framing. Later, see cover maintained through two interventions | Two separated interventions completed while mean canopy stays above the scenario threshold |
| O2 | **Why CCF is repeated management, not one conversion cut** | [PRAC §2.3, §11.1 "about 40 years"; Swiss/Irish "little and often"] | SUPPORTED | Make a light first intervention; return years later and intervene again on the changed stand | ≥2 intervention years with ≥3 annual advances between them (see `RepeatedInterventionDesign.md`) |
| O3 | **Why the forest left behind matters** | [PRAC §2.1, §5.2A, §11.5] | SUPPORTED (derivable) | Compare the "what you are leaving" summary of two marking plans | Player opened the residual-stand summary before approving (requires Packet 3) |
| O4 | **What a Crop Tree (quality tree) is** | [PRAC §11.5 Q-tree] | PARTIAL: vigour (DBH, crown, competition) exists; **stem form/quality does not** | Choose Crop Trees after inspecting them | Crop Tree designated on a tree the player had inspected |
| O5 | **Why a suppressed tree is not automatically a tree to remove** | [PRAC §11.4, §12.2 "do not clean up"] | SUPPORTED: Hegyi term ∝ neighbour DBH / distance, so a small stem adds little competition to a large neighbour. Every felled tree carries a work cost | "Do not clean up" teaching case (`PositiveSelectionTeaching.md`) | Player compares removing a small stem with removing a large neighbour, using the Crop-Tree release readout |
| O6 | **How to identify a real competitor** | [PRAC §11.5 step 2; §12.2] | SUPPORTED for size/distance competition (Hegyi). **NOT** for crown overlap or crown class as such (no crown-position state) | Inspect neighbours of a Crop Tree. Identify the one with the largest competitive effect | Fell mark placed on a neighbour whose Hegyi term on a Crop Tree is among its top 3 (diagnostic only, never a pass/fail gate) |
| O7 | **Why thinning creates benefits and risks** | [PRAC §12.2 "thinning too heavily"; EMP §6 gap-edge quality] | PARTIAL. Benefits: DBH growth response, timber income. Risks shown: the wind diagnostic rises with slenderness and opening; productive capital falls; light rises (enabling regeneration). **Not simulated:** windthrow (D-044), gap-edge timber-quality loss, epicormic growth | Compare a light and a heavy plan | Player viewed both a light and a heavy plan's summary (marteloscope) |
| O8 | **How regeneration enters the system** | [EMP §4–5; Overview annual sequence] | SUPPORTED: seed rain from mature trees → establishment, depending on light → growth → promotion | Watch Sitka regeneration appear once the plantation trees mature (age 21+) and light rises | Player inspected a regeneration cell. Diary shows the first regeneration year |
| O9 | **When natural regeneration may be sufficient** | [PRAC §11.8, §2.7] | SUPPORTED for Sitka (abundant seed once mature). The key nuance is honest: natural regeneration here can renew *cover*, but **cannot add species without a seed source** | Compare regeneration by species on the map | Player opened the Regeneration layer after Year 5 |
| O10 | **When enrichment planting may be useful** | [PRAC §11.8 Irish Quick Guide 07] | SUPPORTED: no Oak/Beech seed source exists on the property, so planting is the only route for these species | Plant a few broadleaves where light allows | Planting placed in a cell with light ≥ the species' poor-light threshold (diagnostic, not a gate) |
| O11 | **How browsing can stop recruitment** | [EMP §4 Welch: height delay and form damage; PRAC §2.8 Austria] | PARTIAL: browsing suppresses height growth of palatable species (Oak, Beech; Sitka palatability 0.15). Background pressure is fixed at 0.2 ("low"); form damage and browse history are not persisted (D-044) | Compare a sheltered and an unsheltered planted pair over several years | Diary shows the height difference. Player visited both |
| O12 | **Why protection may be justified** | [PRAC §2.8; EMP §5.2 non-monotonic exclusion effects] | PARTIAL: shelters protect individual stems for 8 years. Fencing is not gameplay. Long-term exclusion side effects are not simulated | Decide which planted trees to shelter given cost | Shelter choice made, with cost shown |
| O13 | **Why vegetation competition may require clearance** | [EMP §5.1 Harmer & Morgan; research-index D-024] | **NOT SUPPORTED causally.** Understorey cover has no causal effect on juvenile survival or growth (D-047 open item; `BrowsingConditions` concealment hook = 1). Clearance removes ground plants *and* tree regeneration in its footprint | Teach clearance as a *tool with costs* and state honestly that this version does not yet simulate weed competition | Player read the clearance explanation before planning. **Do not** assess "clearance helped seedlings" |
| O14 | **Why pruning is used for timber quality** | [EMP §6 Macdonald: not a guaranteed premium] | PARTIAL: lifts and clear-stem height are tracked. No price premium is implemented (D-044) | Prune Crop Trees after reading why | Pruning applied to Crop Trees only (already enforced) |
| O15 | **Why management is reassessed after time advances** | [PRAC §2.6, §14.3 adaptive monitoring; §17 AFI] | SUPPORTED: annual snapshots and events are saved. Per-tree growth history is not (only last year's increment) | Use the Annual Review "what changed" list and revisit the site | Player re-inspected a Crop Tree after ≥3 years |
| O16 | **Why several different resulting stands can all be credible CCF** | [PRAC §2.2, §12.1, §19] | SUPPORTED by design: no universal score | See two or three marteloscope treatments with different, defensible trade-offs | Player viewed the multi-dimensional comparison |

---

## Simulation support summary

| Support | Outcomes |
|---|---|
| SUPPORTED | O1, O2, O3, O5, O8, O9, O10, O15, O16 |
| PARTIAL | O4 (no stem form), O6 (no crown position), O7 (no windthrow/quality loss), O11 (fixed pressure, no form damage), O12 (no fencing), O14 (no premium) |
| NOT SUPPORTED causally | O13 (vegetation competition) |

## Implication for design

1. **Teach O13 as honest uncertainty, not a mechanic.** Until a causal understorey model is accepted (D-024 is still "current direction"), the clearance lesson must say what clearance *does* in the game: it removes ground plants and young trees in the square, for a cost; plants regrow. It must not say that clearing makes seedlings grow better.
2. **O4 and O6 are the biggest positive-selection risk.** Without stem form, a "Crop Tree" is chosen on vigour and position only. The tutorial should say: "In a real forest you would also judge straightness and branch size. This game does not yet model stem form, so choose on vigour and space." This is a deliberate simplification [INF]. A **future** form/quality attribute is a product decision, not proposed here.
3. **Assessment is observational.** Every "observable evidence" item above is something the game already knows or can derive. None is a quiz, and none uses a universal score (see `TutorialScoringDecision` in `DecisionMatrix.md`).
