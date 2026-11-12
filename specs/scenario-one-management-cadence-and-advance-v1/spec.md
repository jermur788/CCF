# Scenario One — CCF Management Cadence and Two Advance Options V1

**Status:** **DRAFT TECHNICAL SPEC / APPROVED PRODUCT INTENT (D-074 and D-075)**. No source implementation authorised until exact policy and safety gates are approved.  
**Owner:** Overall Manager #200. **Prepared:** 2026-11-12.  
**Companion:** [plan](plan.md), [tasks](tasks.md), [requirements](checklists/requirements.md).
**Related:** [required multi-year decision/outcome feedback V2](../scenario-one-multiyear-decision-outcome-feedback-v2/spec.md); historical S1-C Review/History and S1-D repeated-intervention specs.

## Product intent

Scenario One should model continuous-cover forestry as **a management intervention → years of forest development/observation → reassessment → another justified major intervention**, rather than teaching repeated major thinnings every consecutive year. The player controls when work is carried out; **annual simulation remains the authority**.

The player may choose either:
- **Advance 1 Year** — current familiar Work Plan action, retains every existing guard and review.
- **Skip to Next Management Year** — one controlled batch over several ordinary annual steps until the *next indicated management review point* (or an earlier real stop condition). On arrival, inspect the forest, history and recommendations; **no work is automatically chosen or performed**.

The phrase *management year* means a suggested year to **reassess whether intervention is justified**, not a prescribed year in which felling must occur. A year may have no work. Do not promise it is safe to thin again.

## Directly inspected baseline (latest post-Fable C-01 feature source, not yet integrated)

- `ScenarioOneManager.AdvanceYear()` (approx lines 1264–1387) handles approved work, then exactly one `ecology.AdvanceOneYear()`, juveniles/understorey/deadwood, annual report, `YearAdvanced` event, snapshot and `EvaluateProgress`; it marks the latest review unread after every year.
- `WorkPlanView` (approx lines 48 and 170–174) offers one **Advance one year** action. `ScenarioOneUiRoot.AdvanceFromWorkPlan()` (approx 663–670) respects Rack problem and outstanding Annual Review and then opens Review.
- `ScenarioOneCompletionRules.HasReviewedTwoCycleSequence()` currently accepts **any two distinct successful-work years** with an intervening review acknowledgment, not necessarily spaced stand interventions. `ScenarioInterventionHistory` defines an intervention as **any successful resolved Work Plan**, not exclusively substantial thinning.
- Saved annual reports, ecological snapshots, management events and historical years already support longitudinal summaries. A previous review has historical/year controls; a new timeline store is not required.
- These are source observations, not evidence that an automated batch already exists or is safe. **No current Unity batch runs/tests have been performed.**

## Requirements

**FR-A1 — Both choices visible and honest.** Retain `Advance 1 Year` and add an adjacent/nearby `Skip to Next Management Year` action only in an appropriate Work Plan or review decision context. Before activating a skip, show **the target year, estimated elapsed years and why it is proposed**. If there is no safe target, disable it with a plain explanation and preserve one-year mode. No compulsory use of the skip, extra key binding, or misleading “automatically does next objective.”

**FR-A2 — Context-derived review point, not a blanket prescription.** The candidate target derives read-only from **the last qualifying major silvicultural intervention**, the current year, relevant objective/lesson state, and an expressly approved teaching window. A provisional **four-year review point** can be considered as a *gameplay calibration*, informed by cited indicative 3–6-year CCF practice ranges, but it is **not yet approved as the universal target**. The rule must distinguish first rack/access and planting/protection/maintenance from a qualifying stand-scale thinning/release. If a stage requires marking, inspection or an Objective completion action now, the skip must not bypass it. A targeted year asks the player to review; it does not auto-complete an objective.

**FR-A3 — Annual parity.** Internally run the **existing** authoritative single-year path repeatedly for each elapsed year, preserving exact ordering of committed work (if any, under strictly defined eligibility), growth, death, regeneration, ecology, cost, events, summaries, objective checks, completion and saved world state. Do not jump Year counter, fast-forward a different model, remove history entries, aggregate away yearly records or alter RNG draw order. **Year-by-year manual and skipped runs with identical decisions and seed must result in equivalent annual histories and final world**, allowing only explicitly documented UI acknowledgment presentation differences.

**FR-A4 — No unattended work execution.** At a minimum, skip is **not enabled** with pending/approved open Work Plan orders, unacknowledged mandatory review, outstanding rack/teaching/Objective action, invalid save, missing ecology controller, protected reference-preview state, insufficient disk/test conditions, terminal or incompatible scenario. Do not auto-approve/mark, never silently leave planned work unattended. If new risks/events appear during iteration, stop **after the completed annual step** and open that year’s Review. A single annual action remains available where valid. Failure must not roll back or relabel already completed annual steps.

**FR-A5 — Bounded and interruptible at safe boundaries.** Hard-cap a single request to a finite approved interval (e.g. **maximum five annual ticks** proposed, final cap pending audit) and clamp at first required interaction, special outcome, Year 25/100 or any other authoritative boundary. Show elapsed years and a truthful stop reason; allow further player-directed steps. No UI freeze or uncontrolled high-speed simulation. Never open a new Editor or separate Unity simulation context.

**FR-A6 — Intermediate annual reports preserved, review legible.** Generate normal annual records and snapshots for **every** simulated year, including quiet years. On destination/early stop, open a concise review of elapsed years and link to the existing earlier-year browser. Do not silently acknowledge a **mandatory** review gate; separate optional observational years from first guided/required acknowledgments. Avoid overwriting saved evidence by using a single `annualReviewSeen` flag as proof all skipped years were acknowledged. Stay within S1-C L1/L2/L3/L4 provenance and D-073 required decision↔outcome learning.

**FR-A7 — Legitimate work between major interventions.** Planting, protection and site maintenance remain permitted in annual mode. A player may choose to stop skipping and do earlier warranted work; no imposed “minimum years before any work.” A major repeat-intervention **learning/completion condition** may require elapsed time and a reviewed intervening forest assessment *only if separately approved* after authentic old-save/goal audits. CCF timing is not a universal legal/biological rule.

**FR-A8 — Compatibility/acceptance.** Preserve Legacy40/pre-feature/frozen Reference Future v1 semantics and world/reference identity, historical saves, Rack V2/MFM Work Plan commitment and three-species parity. No extra persistence just to remember a proposed review target if it is derivable. If a saved timetable becomes necessary, stop for a new spec and hash-safe compatibility proof. No new CCF ecology coefficients, no automatic felling, no Windows build prior to personal test and explicit authorization.

## Adjacent product decision D-076 — Sample and treat, then observe

The target-management-year review may be the moment to consider a **new representative rack-area sample** and apply a new MFM prescription to comparable units if justified. Neither advance mode automatically opens a sample, generates a new prescription, confirms a Crop Tree or executes proposed work. Each annual step updates all **actually treated rack areas** through the same authoritative ecology. The separate [Repeated Representative Rack Management V1 DRAFT](../scenario-one-multicycle-representative-rack-management-v1/spec.md) owns any later-cycle rack selection and new multi-sample logic. This time-advance feature must not turn a suggested review date into a compulsory felling cycle, nor require every rack to be manually sampled.

## Design decisions pending source audit

1. Which existing learner states actually have a safe next temporal management point, and which (e.g. early Rack Objective 1) demand same-year player interaction?
2. Should provisional four-year gap / five-step cap be used for *learning calendar guidance*, or should an evidence-backed scenario schedule or stand-state condition choose the next year? Avoid a universal “always 4 years” claim.
3. Should the spacing requirement modify current completion predicate, add a separate educational objective, or remain guidance until user tests the pacing? Old saves may already have completed consecutive-year cycles; do not retroactively invalidate them.
4. How to preserve mandatory review acknowledgment while allowing one-click observational intervals, including after a new Year25 completion event or unexpected objective transition?
5. Whether a 1-to-5-year batch is safe for existing runtime time budget, Unity visuals, save/reload and deterministic current RNG without a new async simulation layer.

**Approval contract:** User approved both choices and multi-year CCF cadence at the product level; **this draft's implementation mechanics, numeric calendar policy and completion-predicate changes are not yet separately released**.
