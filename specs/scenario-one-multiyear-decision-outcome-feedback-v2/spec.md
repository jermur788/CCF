# Scenario One — Multi-Year Decision and Outcome Feedback V2

**Status:** **DRAFT — REQUIRED PRODUCT OUTCOME (D-073), NOT APPROVED FOR IMPLEMENTATION.**  
**Date:** 2026-11-12. **Owner:** Overall Manager #200. **Depends on:** approved S1-C Review/Management History, S1-D repeated interventions, D-004 gameplay loop, D-059/D-060 first complete three-species demo. **Not dependent on:** optional retained-neighbour reasons #386.

## 1. Player purpose and concrete outcome

A player who has completed one or more management interventions must be able to understand **what they chose, what was actually approved and executed, what the game recorded afterward, and what to consider in the next intervention** across different years. This **is required even if the optional "Retain — why?" interaction is never built**. The source and current UI may already meet part of the outcome; this specification requires a **gap audit first**, not a redundant replacement for S1-C.

Use the current physical forest / Work Plan / Annual Review / management-history flow as the core. Do not create a competing timeline. Include Sitka, Oak and Beech in credible observed cases. Product result is better player understanding, not a richer but unexplained data dump.

## 2. Authority, evidence and unresolved baseline

- **Direct user instruction / D-073:** prioritise essential multi-year decision↔outcome feedback ahead of uncertain tree-retention intention UX.
- **S1-C:** four separate content layers: L1 *recorded fact*, L2 *current observed state*, L3 *derived comparison* only where inputs available, L4 *explanatory consideration* explicitly nonauthoritative.
- **S1-D:** advisory repeated management, existing legitimate teaching-completion latch, no new per-cycle state/compulsory treatment interval. Its original approved spec may contain historical implementation notes; use actual updated branch source as implementation truth.
- **Existing model and UI** on the updated Scenario One lineage must be inspected **before** specifying missing screens or data. Suggested inventory targets: `ScenarioManagementHistory`, `ScenarioManagementEvent`, `ScenarioOneManager` annual reports/ecology, `AnnualReviewView`, `WorkPlanView`, `ScenarioOneUiFacts`, current annual ecological snapshot/save structures, existing objectives and recorded work order chronology. These are targets, not verified present-day claims.
- **Research #388:** natural young-growth cohorts are not individual seedling census or guaranteed recruitment, and planted juveniles have different tracking. Light/browse/understorey discussion needs source truth; no invented consequences.
- **Opus #389:** new persistent retention reason/event fields can alter protected world hash. This V2 needs **no per-neighbour reason or new event store**.

## 3. Draft functional requirements — minimum deliverable pending source audit

**FR-M1 — One accessible multi-year question.** From an existing Annual Review / management-history surface, the player can identify and browse a completed year or intervention, then return to the current decision context. Reuse existing year browser and navigation if present; do not add an additional menu path without a demonstrated UX gap.

**FR-M2 — Decisions versus work actually done.** For the selected period, distinguish player-selected management/marks or plans *when historically recorded*, **proposals**, approved orders, completed/failed/cancelled work and costs. Never infer an approval, Crop Tree designation date, fell mark or prior motive from current state. Retained-neighbour reasons are not required.

**FR-M3 — Observable later outcome.** Present selected year’s recorded annual results alongside a **clearly dated** present condition where existing S1-C data provides it, or a documented same-measure longitudinal comparison where two saved observations truly exist. Separate the time of the intervention and the time of an observation. Never claim an intervention caused a change merely because it preceded it.

**FR-M4 — Decision-ready feedback, not new mechanics.** Provide a concise contextual “what to consider next” message in the S1-C explanatory layer (L4), identifying recorded history and current observed model signals but no unique correct action, new objective gate, forestry score or automated mark. Keep the Work Plan approval/commitment boundary unchanged.

**FR-M5 — Young growth where evidence supports it.** Where the saved annual data permits, distinguish presence of represented natural-origin cohorts from exact planted juveniles, and discuss whether recorded presence/condition appears to persist across years. **Do not equate cohort-occupied portions with seedling counts, hectares, mature establishment or survival proof.** Where a year/species/origin field is missing or model versions differ, say “not recorded” or omit the comparison. Gap-edge/open-ground spatial labels are not authorized without real geometry.

**FR-M6 — Honest provenance and compatibility.** All displayed historical L1 and observation L2 facts come from authoritative existing records. L3 computes only defensible same-unit comparisons with clear source/years; L4 remains general guidance. Historical saves, Legacy40, current Enlarged80 and frozen Reference Future v1 remain valid; no new historical backfills. World hash, save/version/event IDs, current models, ecology, cash and objective scoring unchanged unless a later separately approved spec amendment proves necessary.

**FR-M7 — Existing feature parity.** The UI remains usable at 1280×720 and 1920×1080, without breaking Annual Review acknowledgment, S1-D repeated cycles, readable money/work, the player's currently improved UI or #382 personally owned playtest lane. One navigational summary or limited inline link may be sufficient.

## 4. Explicit exclusions

No optional “Retain — why?” button, reason capture, retained-neighbour event, crop-tree judgement grader, world-save schema, unrecorded motive, new timeline authority, MFM proposal override, false cause-effect inference, 30% Share/40% basal-area/Year25/100 rule changes, automatic treatment, new forest calculation, storm damage, exact site-specific shade/edge classification or Windows build.

## 5. Scope gates and design decisions still open

**D-073 approves the NEED, not this UI design.** Before local source work the Manager must see a source/UX gap matrix stating which S1-C/S1-D abilities already work on the **current updated UI source**, and which exact missing link this V2 would fill. If no gap is found, the requirement may be satisfied by documenting/validating existing capability, rather than manufacturing a duplicate control.

Decide after audit: (A) existing history is sufficient but discoverability/copy needs work; (B) one small linked comparison is needed with existing data; (C) multi-year origin/stage aggregation is missing and requires a separate data/feature spec. Prefer A or B when truthful. Do not silently choose C.

**Integration:** existing Research Copy V1 gate [#393](https://github.com/jermur788/CCF/issues/393) remains independent. After Manager approves a revised four-part spec, release one clean source owner/feature claim on a verified up-to-date UI candidate; focused independent evidence review when history/provenance changes; no full Tier-3 except on milestone candidate. No producer can waive save, Reference Future or Unity lock gates.

## 6. Completion evidence (future)

At least two authentic successive annual reviews with an intervention and later decision, plus one idle/no-work year; a normal versus older save gap; positive Sitka and Oak/Beech cases where genuinely present. Verify navigation, player provenance, zero fabricated events, user-visible differences and honest unrecorded fallbacks. No PASS/IMPLEMENTED assertion from this DRAFT.

## D-076 — Treatment provenance and rack-wide observed consequences

Future decision/outcome feedback should let players distinguish their chosen **representative source area** from other areas where they later **approved and executed** a corresponding MFM treatment. Treat the original sample as L1 demonstrative player decisions, approved Work Plan work as separate L1 recorded action, and later forest state as L2 observation; use L3 only for comparable saved year and location metrics, and L4 for cautious advice. Existing annual ecological snapshots are stand totals and per-species aggregates, **not per-rack annual ecological histories**. Therefore do not claim per-rack Year A → Year B canopy/regeneration figures unless a new source-authentic longitudinal measurement contract is separately approved. D-076's [multi-cycle rack sample DRAFT](../scenario-one-multicycle-representative-rack-management-v1/spec.md) controls later-cycle sample selection and overlap; this UI feature owns honest explanatory visibility. The optional retained-neighbour reason feature #386 is still not needed.

## 7. Required CCF calendar/advance connection — D-074 and D-075

The user's subsequent product direction requires **periodic management and two selectable time actions**, not merely additional review text. The independent [Management Cadence and Two Advance Options V1](../scenario-one-management-cadence-and-advance-v1/spec.md) draft owns that **new player workflow and annual-step engine**; this feedback V2 owns only the selected-year recorded-decision/outcomes presentation contract. The two features can reuse S1-C historical records but must not each implement a new time controller or history store.

The player can choose **Advance 1 Year** or **Skip to Next Management Year**. During a skip, the separate cadence feature must run each authoritative annual tick and stop on real decisions, preserve individual annual records, and on arrival open the same truthful Review supported here. A requested “management year” is a suggested **reassessment point**, not mandatory thinning, and an objective requiring same-year player action must not be auto-completed by passage of time. The exact target-year policy and any completion predicate amendment remain DRAFT until tested/approved. This V2 neither implements nor blocks the two-button UI by itself.
