# Objective redesign (Workstream L)

**Status:** design proposal. **Changing forest objectives changes the completion anchor** (`D7C4DDD36B53FCCE` for the current stack) and is a **PRODUCT DECISION** (`DecisionMatrix.md`).

## 1. Audit of the current eight objectives

Test: **does meeting it prove the player understands the mechanic?**

| Objective | Measures | Met by inaction? | Proves understanding? | Verdict |
|---|---|---|---|---|
| Reach the management review year (≥ 25) | Time | Yes | No | Keep as a horizon, not an objective. Explain why (CCF is a process) |
| Retain original canopy trees (≥ 60 living Sitka) | Count of *all* living Sitka, including recruits | Yes (Year 0) | No. A guardrail against liquidation only | Keep as a guardrail; count original-plantation trees (P-prefixed), as the Century Review already does |
| Keep continuous canopy (mean ≥ 0.35) | Final-year mean canopy | Yes (Year 0) | No. A guardrail | Keep as a guardrail, checked **every** year, not only the final one |
| Maintain regenerating cells (≥ 3) | Occupied cells, any species | Yes (Year 1) | No | Replace with a diagnosis stage plus a renewal guardrail |
| Retain fallen deadwood (≥ 0.02 m³) | Volume on site | **Yes under Growth Model 1** (self-thinning logs) [INF] | No | Remove from completion. Report under HABITAT |
| Carry out a commissioned thinning (≥ 1 felled) | One tree | No | No: any tree counts | Replace with "Crop Tree release" plus a second cycle |
| Establish planted beech (≥ 1 present) | One sapling | No | Weakly: one sapling anywhere | Merge into "introduce a species the plantation lacks", judged by survival after time |
| Establish planted sessile oak (≥ 1 present) | One sapling | No | Weakly | As above |

**Summary:** five objectives are state the forest already has or reaches alone. The other three each count a single act. None needs a decision that could be made badly.

## 2. Design principles for the replacement

1. **Stages complete on observed decisions in the forest**, not on reading or counts.
2. **No stage judges the decision.** Any thinning that touches a Crop Tree's neighbours is "a release"; how much is the player's choice, and the consequences are reviewed.
3. **Guardrails, not targets**, for forest state: continuous cover, standing capital, renewal. No inverse-J, no fixed species %, no fixed basal area.
4. **Counts only where the count is the meaning** (e.g. "two cycles").
5. **Learning stages and completion share one progress panel.** Retire the separate 42-step device checklist as the main path. Keep its texts as per-stage help.
6. **Stage progress is per forest** (pedagogy decision #8). Interim before the save field exists: derive from saved events where possible (§4).

## 3. Proposed progression (stages)

| # | Stage | What the player does | Evidence that completes it | Derivable now? | Avoids |
|---|---|---|---|---|---|
| S1 | **Explore** | Use the map, walk to a bright place and a dark place, read the ground report at both | Arrived at two cells with ground light differing by ≥ one band, ground report shown at each | UI observation (session) | Clicking map layers N times |
| S2 | **Inspect** | Inspect a tree **and one of its neighbours** | Two inspections within 8 m of each other | UI observation | "Inspect 5 trees" |
| S3 | **Choose what to keep** (positive selection) | Designate Crop Trees after inspecting them; open one Crop Tree's neighbour list (P2) | ≥ 1 Crop Tree that was inspected before designation; neighbour list viewed | UI observation; Crop Tree mark is saved | "Mark 10 Crop Trees" |
| S4 | **Release** (selective thinning) | Fell-mark neighbours of Crop Trees; review "What you are leaving"; approve | A resolved thinning in which ≥ 1 felled tree was within 8 m of a living Crop Tree | **Yes** (events + positions + marks) | A volume or % target |
| S5 | **Count the cost** (economic review) | See the job's net and the small-job minimum before approving | Approval happened with the Work Plan harvest card shown (session); the following review's MONEY read | Partly | Quiz questions |
| S6 | **Let time pass and look back** | Advance; read the review; follow a PLACES waypoint and arrive | Arrival at a PLACES cell after a review | UI observation | Acknowledge-only |
| S7 | **Diagnose regeneration** | Visit a place with young trees and one without; read "Why" at both | Ground report at a cell with juveniles and one without, in the same visit window | UI observation | "Regeneration ≥ 3 cells" |
| S8 | **Plant where it makes sense** | Decide whether and where to plant broadleaves (no seed source here) | ≥ 1 planted broadleaf **alive 5 years after planting** | **Yes** (juvenile records) | "Plant both species", "plant N" |
| S9 | **Protect or accept browsing** | Choose shelters on or off for a planting; later read its browse result | Planting resolved with a shelter decision, and a later review with that stock's browse line | Yes (orders + review) | "Install shelters" |
| S10 | **Vegetation and clearance** *(Model 2)* | Find a place where vegetation limits short young trees; decide | Ground report showed the vegetation limit at a cell with short juveniles. Clearance optional | After M2 | "Clear N cells" |
| S11 | **Prune (optional)** | Prune an eligible Crop Tree, then inspect it | Resolved lift on a Crop Tree | Yes | Batch-prune counts |
| S12 | **Second look, second cycle** | After the trigger (`SecondInterventionDesign.md`), re-inspect and carry out any management work | Resolved work ≥ 5 years after the first release, after the trigger, plus the following review read | Yes (events), except re-inspection | Calendar-only "thin again" |

Optional (not required for completion): S10 before Model 2 integrates (clearance has no causal effect on main), S11.

## 4. Implementation tiers

| Tier | Scope | Save | Anchor |
|---|---|---|---|
| **A — Teaching only** | Progress panel shows S1–S12 with per-stage help. UI-observed stages are session-scoped. Completion is unchanged | None | Unchanged |
| **B — Per-forest progress** | A saved `scenarioProgress` record (stage ids + year reached). Survives reload and machine changes | **Schema change** (after M2 v18 and storms v19) | Unchanged if completion is untouched |
| **C — Completion uses stages** | `ScenarioCompletionDesign.md` model D | Uses B | **Changes the completion anchor**; explicit authorisation |

**Recommendation:** ship **Tier A** in P5 (no save), playtest, then decide B + C together in P6.

## 5. What happens to "plant Beech **and** Oak"

Keep a broadleaf requirement in some form. It is the only way the player can begin a species change the plantation cannot make on its own. Its form is a product decision:

- (i) keep both species (current);
- (ii) **at least one broadleaf species established after 5 years (recommended)**;
- (iii) no requirement; report only.

(ii) keeps the lesson "species the site lacks must be introduced deliberately" without dictating a mixture.
