# Storms — player teaching (Workstream K)

**Status:** teaching proposal. Builds on `task/windthrow-readiness` @ `9a9f4fb` [STORM-DESIGN] (`PlayerFeedback.md`, `TutorialHandoff.md`, `GameplayCases.md`). **Storms are not implemented. D-044 still defers them.** Lifting the deferral is a product decision (storm decision matrix #1).

## 1. What must be taught

| # | Lesson | Mechanism in the proposed model | Player evidence |
|---|---|---|---|
| 1 | **Thinning can change exposure** | Recent-opening term in the vulnerability index V (3 × 3 cells, decays over a few years) | Stability band moves to "Exposed — neighbours removed recently" after marking/felling |
| 2 | **Tall, slender trees can be vulnerable** | Height/load and H/D terms in V | Reason line "Tall for its diameter (H/D 86)" |
| 3 | **Risk is contextual** | Storm severity × direction × edge × recent opening × tree | Same tree "Watch" in one decade, "Stable" later; edge factor only when the storm comes from the open side |
| 4 | **One storm does not prove a treatment was wrong** | Two-layer model: storm years are random; damage is probabilistic per tree | Review wording "Trees near recent thinning were among the damaged", never "your thinning caused" |
| 5 | **Disturbance creates deadwood and openings** | Windthrown stems become fallen deadwood records; gap light rises; regeneration may follow | Storm gap visible, later seedlings in it; deadwood counts |

## 2. A finding that changes the teaching plan [PROTO]

Every new Scenario One game uses the **same seed** (`ForestEcologyController.simulationSeed = 20260914` [REPO]). The proposed storm roll is keyed only by seed and year (`SimulationRandom.Roll`, "STORM-OCCURS-v1"). So **every player gets the same storm years**, whatever they do.

Reproducing `SimulationRandom.Roll` (model 1) offline with the proposed defaults (p = 0.08, grace 3 years, M/S/X 0.70/0.25/0.05) gives (`storm_timeline.py`, `Evidence/storm-timeline-output.txt`):

| Proposed p | Storms in Years 3–25 | Later |
|---|---|---|
| 0.05 | **Year 3 (severe)** | 38, 100 |
| **0.08 (default)** | **Year 3 (severe)**, Year 13 (moderate) | 31, 38, 56, 100 |
| 0.12 | Year 3 (severe), Year 13 (moderate) | 31, 38, 56, 74, 86, 100 |

Severity order is an assumption about the future implementation; occurrence years are not. Across 200 alternative seeds the first storm's median year is 11 (10th–90th percentile 4–25).

**Implications:**

- With the default seed, a **severe storm arrives in Year 3 of every game**: the first year storms are allowed, and 1–2 years after the typical first thinning, when recent-opening exposure peaks. The storm design's own check #4 ("Year-0 stand: negligible damage") covers an age-20 stand. It does not cover a stand heavily thinned in Year 1.
- This is either a teaching hazard (an early loss the player cannot yet understand) or a deliberate authored moment. **It must be decided, not discovered** (`DecisionMatrix.md` PD-K1).
- Options: (a) keep the fixed seed and calibrate so a Year-3 severe storm on a thinned age-23 stand does modest, explicable damage; (b) a longer grace period (e.g. first storm not before Year 8, after the first Annual Review cycle and planting); (c) a per-game seed (variety, but loses cross-player comparability for playtests and changes every anchor); (d) different roll ids. **Recommendation: (a) + (b) with grace = 6** [C]: storms enter after the player has completed one full cycle. With the same roll, grace 6 makes the first default-seed storm **Year 13 (moderate)**. That is after a first intervention and its regeneration response, and usually close to a second-intervention window (`SecondInterventionDesign.md`). Re-run this script on the final implementation to confirm the first storm year.

## 3. Explicit tutorial versus organic discovery

| Criterion | Explicit storm tutorial | **Organic discovery + Annual Review explanation (recommended)** |
|---|---|---|
| Fidelity to the brief | Scripts an event; risks implying storms follow thinning | Storms happen when the model says; the review explains |
| Learning value | Guaranteed exposure | High *if* a storm happens. With the fixed seed it always does (§2) |
| "One storm does not prove…" lesson | Hard: a scripted storm *is* a prescribed outcome | Natural: the explanation stresses chance and context |
| Cost | Scripted event path = second code path, harness exceptions | Uses the real event path only |
| Predictability for testing | High | High (deterministic seed) |

**Recommendation: organic discovery with a pre-storm framing and an event-driven explanation.** No forced storm in normal play (the `ForceStorm` harness API stays editor-only).

Sequence:

1. **Before any storm** (intro help, one sentence): "Storms are part of Irish forestry. Some years bring damaging winds; you cannot predict which."
2. **First tree shown Watch/Exposed** (Tree Inspection help, one-time card ≤ 3 sentences): what the band and its reason line mean.
3. **First marking that moves trees to Exposed** (forecast line): "6 trees will be more exposed to wind for a few years."
4. **First storm** (Annual Review DISTURBANCE section + one-time explanation card): what happened, where (co-occurrence wording), what windthrown stems are, what salvage is, Set waypoint to the largest new gap.
5. **First salvage decision** (Work Plan): money vs deadwood vs doing nothing; all legitimate.
6. **Later**: when regeneration appears in a storm gap, PLACES TO INSPECT can name it ("Seedlings in the Year 13 storm opening").

## 4. Draft copy

| Moment | Copy |
|---|---|
| Stability band help | "Stability describes how exposed a tree is to strong wind: Stable, Watch or Exposed. Tall, slender trees and trees whose neighbours were recently removed are more exposed. Exposure fades over a few years as trees adapt. It is not a forecast — storms are rare and unpredictable." |
| Forecast | "After this thinning: 6 trees move to Exposed for a few years (mostly C3, D3)." |
| Storm review lead | "STORM — Year 13. A moderate storm from the south-west. 9 trees blew down (2.1 m³). Most damage: C3, D3 and the south edge. Trees near recent thinning were among the damaged." |
| Not-proof line | "A storm shows what happened this time. It does not prove a different plan would have avoided damage: unthinned, tall, slender stands are also vulnerable." |
| Opportunity line | "Fallen trees are now deadwood, and the new opening lets light reach the ground." |
| Salvage | "You can sell or keep windthrown stems for a few years, or leave them as deadwood. Salvage costs more than normal harvesting, and value falls as stems decay." |

Never: a storm probability; "safe"; "your thinning caused"; a correct thinning percentage.

## 5. Dependencies

Stability bands, storm events, salvage and the DISTURBANCE section all come from the storm packet (Sol). Teaching work starts only after it integrates. The Annual Review v2 structure (P4) should land **before** storms, so the storm section has a home.
