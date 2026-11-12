# Scenario One — Spaced Major-Intervention Learning V1

**STATUS: DRAFT / READY FOR MANAGER DECISION — NOT APPROVED FOR IMPLEMENTATION.** 12 November 2026. D-074 accepts the educational direction; the exact new-world objective, evidence requirement and save policy are *proposals*. The narrow [read-only follow-on helper](../scenario-one-management-followon-browser-readonly-v1/spec.md) is separately approved but is NOT an objective predicate.

## Product requirement and source truth

Teach **deliberate stand management separated by years of observation and reassessment**, followed by a new player-directed substantive management decision where appropriate. Work in the intervening years (planting, pruning, protection, deadwood, salvage, necessary felling) stays permitted. The player is not required to cut when the calendar reaches a suggested reassessment year.

Inspected actual C-01 source:
- [ScenarioOneCompletionRules.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs#L36) currently accepts any TWO distinct years containing qualifying succeeded orders with an intervening AnnualReviewAcknowledged event in event-ID order. Planting or year-to-next-year maintenance can count. This is the accepted historical S1-D completion semantics, **not** proof of periodic major CCF thinning. Do not replace it silently.
- [ScenarioInterventionHistory.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs#L80) similarly calls any year with a succeeded commissioned task an intervention, regardless of silvicultural weight. Its presentation ordinal is not a major-treatment classification.
- [ScenarioOneWorkOrder.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs) carries type, outcome status, rackId and MFM unit provenance when available. rackId>0 is rack-owned access work, not a general stand-thinning count; old orders may have no prescription/unit.
- [ScenarioOneManagementFollowonReadout.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs) can surface only **possible** non-rack FellTree years and genuine exact-year observations. It does not prove that one or many removals were a justified stand-scale operation, nor that a player reviewed evolving local conditions.
- [ScenarioOneManager.PrescriptionApproval.cs](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs#L144) only generates Pending orders following accepted prescription approval; subsequent Work Plan approval and annual resolution are distinct gates. The first fixed third-rack teaching sample is protected by D-065/D-076.
- D-077 time advance source (#397) supplies a **4-year suggested review point and at most five sequential annual ticks**, not a minimum CCF treatment interval, a completion gate or approval of new management. The source candidate has not been Unity-qualified in this browser task.

## Proposed two-layer learning model

**L1 — Historical Scenario One objective remains authoritative.** Preserve HasReviewedTwoCycleSequence, existing completed outcomes, Year25/100 and S1-D teaching credits exactly for current and historical worlds. This V1 must not make an already-completed save become incomplete. No silent reinterpretation of annualReviewSeen or old outcome records.

**L2 — Additional non-gating learning demonstration for explicitly new worlds (Manager approval required).** Show a progress narrative, not an invisible mandatory win condition: (i) first *substantiated* stand-scale treatment, (ii) two or more genuinely completed observation years and review of recorded changes, (iii) player reassesses current live forest and resolves a later appropriate *substantiated* stand-scale treatment. Label the elapsed interval *gameplay calibration*, not national forestry practice; do not require a cut when no treatment is justified. Proposed **minimum three elapsed years** between the two example treatments would guarantee intervening observation, but must be explicitly accepted or replaced by evidence-driven reassessment with no fixed minimum. It is distinct from the D-077 four-year navigation suggestion.

**No retroactive reconstruction.** An old saved game can show “historical sequence preserved; new learning evidence not tracked” and still complete under its prior rules. Resaving an old file under the latest serializer does not turn it into a new-world teaching cohort. If no reliable durable world-origin/new-world flag is available, keep L2 **non-gating and advisory for all worlds** until a hash-safe identity design is accepted. World creation version alone is NOT a safe discriminator without source proof.

## Proposed strict evidence vocabulary (not an implemented predicate)

| Class | Direct surviving evidence | Meaning / limitation |
|---|---|---|
| Recorded work | Succeeded/failed/cancelled WorkResolved events and retained order when valid | Actual work, not proof of management judgement |
| Rack access | Succeeded FellTree with matching retained order and rackId>0 | Access clearance, explicitly **not** stand thinning achievement |
| Candidate stand management | Succeeded FellTree, valid non-rack order, possibly with prescription/unit IDs | Candidate only; single arbitrary tree never automatically qualifies |
| Credible substantive treatment | **Not fully persisted today**: a player-reviewed stand/rack-area treatment episode with meaningful spatial scope, deliberate release/retention decision and successful resolved Work Plan | Requires explicit product acceptance and likely new episode/provenance or evidence aggregation; do NOT infer from event count alone |
| Reassessment | Recorded annual review and on-site current decision context | Acknowledgment proves review interface event, not physical field inspection, tree choice rationale or condition-specific judgement |
| Observed change | Exact annual stand/species snapshot years A and B | Aggregate model difference, not causality or per-rack biological history |

**Proposal for eventual qualification:** require at least one meaningful comparable rack-area management unit actually treated (or equivalent verifiable manual area), NOT a compulsory five-rack count. Define treatment size/geometry in a separate approved objective contract only after authentic fixture testing, with a manual route. No unsupported numeric hectares/BA percentage as proof of “major”; the 30% release and 40% BA lesson safeguards are local protected teaching rules, not universal episode thresholds.

## Player experience proposal

In the existing Objectives/Annual Review teaching language, after the first executed substantive treatment: “Return in later years; inspect the stand, keep necessary maintenance going, and check recorded outcomes.” Show dated observational years and acknowledge changes may also arise from growth, competition, weather and browsing. After a later genuine player choice: “You have demonstrated another considered intervention after years of observation.” Never auto-mark, auto-propose, auto-approve, auto-advance or force a treatment. Idle years legitimately count as observation years when recorded; the player still needs real independent management judgement for the next treatment.

D-076 representative-area logic may supply treatment provenance **only where it has actually executed**. Current MFM is Sitka-adult-only for automated proposals; Oak and Beech must be managed with species-appropriate decisions and cannot inherit its coefficients.

## Blocking product and compatibility decisions (must resolve before APPROVED)

1. **Objective type:** Accept recommendation for a *non-gating learning objective for new worlds first*, keeping old completion intact, or request a new gated completion path with a separate explicit save/schema/identity version and frozen-reference test programme.
2. **Treatment qualifier:** Decide an inspectable manual-vs-MFM definition of significant stand treatment and how much area/selection evidence must survive save. Current FellTree+non-rack is insufficient; do not promote source-helper candidate count into grading.
3. **Observation cadence:** Accept a proposed 3-year elapsed learning minimum **only as gameplay calibration**, change it, or use a qualitative reinspection requirement with authentic proof. Neither choice forces management every 3/4 years.
4. **New-world marker and saved progress:** Specify reliable origin/eligibility and irreversible learning evidence without reclassifying old/Legacy40/frozen worlds or world/event hashes. Historical objectiveAchievements v22 alone does not authorise another event type.
5. **Spatial/species coverage:** Decide meaningful whole-stand example including manual areas and heterogeneous units without compulsory all-rack sampling or Sitka rules on broadleaves.

## Exclusions and future verification

No write to CompletionRules, save/version/hash, achievements, MFM source identity, cadence UI or automatic management under this DRAFT. Future tests must contrast consecutive-year planting/one-tree felling (not new-credit) with genuinely resolved treatment → intervening exact-year observations → later separate treatment; also early necessary operations, failed orders, hidden/absent old metadata, save/load, Oak/Beech, Year25/100, Legacy40/frozen Reference Future and two advance modes. Exact-head source/tests and appropriate Unity/visual gate with Manager verdict needed; no source integration accepted here.
