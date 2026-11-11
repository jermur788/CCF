# Post-demo design opportunity — recording a reason to retain a competing neighbour

**Product-owner direction:** 2026-11-11 — "Okay, do that", following Manager recommendation to document this idea now, preserve the current Scenario One demo and assess the richer interaction afterwards.  
**Status:** **DOCUMENTED / DEFERRED UNTIL AFTER THE COMPLETE SCENARIO ONE DEMO.** This is **NOT** an approved implementation specification, a ready task packet, a mandated feature, an accepted lesson-rule change or an active ownership claim.  
**Owner:** Overall Manager (research/design backlog).  
**Deferred backlog tracker:** [GitHub issue #386](https://github.com/jermur788/CCF/issues/386) — not running, not an active task claim.  
**Canonical authority:** Latest user direction; `Docs/Project/decision-log.md`; `Docs/Project/current-milestone.md`; approved `specs/multiscale-forestry-management-v1/spec.md`, S1-C and S1-D.  
**Evidence:** Six-paper research extraction P1/P2; existing Irish tree-marking source and CCF practice/wind composite, with source-transfer limitations in `Docs/Research/PommereningSixPapers/Originals-Hash-and-Evidence-Register-20261111.md`.

## The problem this might solve

The current lesson teaches a specific form of crown release: selected Crop Trees have their local competitor `Share` reduced by at least the approved 30% teaching threshold. The underlying Hegyi/Share model estimates neighbour pressure using observable geometry; it cannot answer all of the player's legitimate forestry questions: whether to keep shelter, biodiversity-related structure, future timber, or a neighbour for reassessment in later interventions.

**The player can already refrain from marking a neighbour for removal.** The proposal is **not** permission to leave a tree standing. It is an optional way to **make the player's retention intention explicit, inspectable and historically explainable**, while admitting the actual competition remains.

For example: after selecting a promising Crop Tree, the player might leave a high-Share neighbouring Sitka standing because they wish to retain canopy cover or examine that choice at a later thinning. A future review could accurately say: "You chose to retain this neighbour for canopy cover", then show subsequently recorded competition and young-tree response. It **must not** assert that the retained tree provided proven shelter or that it caused a later ecological result unless the authoritative simulation actually supports that claim.

## The smallest post-demo experiment

Run an **offline/UX design evaluation first**, using a visual mock-up or a non-production prototype with an existing tree inspection context. No new physical marking colour required. The concept is a small, optional **"Retain — why?"** annotation alongside a competing neighbour.

Suggested **player-reported intentions**, not simulated condition flags:
- Preserve canopy cover / potential shelter.
- Keep for future timber or planned later intervention.
- Preserve an observed tree/species or possible habitat feature (only use "observed" if current tree state supports it; "potential habitat" is not proven ecological value).
- Reassess on a later visit.

These are **working hypotheses**, not final selectable options. Evidence-dependent wording and UI space need UX review.

**Crucial interaction rule:** recording an intention does **not** reduce Hegyi `Share`; does **not** satisfy the current 30% release requirement by itself; does **not** create a protected biological status, new Fell mark or silently override work approvals. Existing MFM proposals must remain distinct from committed player marks, and an unapproved annotation should not become a veto on approved work without an explicit product decision.

The player still chooses which trees to mark; accepted 3D manual judgement, sample-to-comparable-area rules, Work Plan commitment, economics and annual ecology remain unchanged.

## Alternatives to compare after the demo

A. **No extra interface** — player leaves the neighbour unmarked; brief teaching text explains trade-offs. Simplest, but no recorded intention for future review.

B. **Optional session-only explanation** — panel can acknowledge a reason but it is not saved. Low-cost UX probe, but later reviews cannot honestly claim to remember it across years/save-load.

C. **Persistent player-authored reason** — attaches a durable, explicit statement of the player's intention to a stable tree/crop-tree/management decision and year. Enables future historical review but requires a new authoritative save/event contract, old-save behaviour, cancellation/edit policy, consistency with felling and MFM, deterministic persistence and independent qualification.

D. **Reason contributes to lesson credit** — would modify the meaning of the existing 30% Share training rule and could introduce an ungrounded equivalence between reducing competition and articulating a rationale. **Not preferred as a first implementation.** Requires separate explicit product acceptance and evidence.

**Initial preferred probe:** compare A with a mock-up of B, gathering user comprehension and workload evidence. Do **not** build C or D until the usefulness is shown and their scope is accepted.

## Design/evidence questions to resolve before a specification

1. Does the player actually want to remember **why a particular competitor was retained**, rather than simply knowing that they left it standing?
2. Can they correctly distinguish a simulated measurable effect (`Share`, current canopy/light) from a judgement or intention ("for potential shelter")?
3. Does the optional reason improve the **second and later management cycle**: can players revisit the decision, see observed consequences and decide again?
4. Will an annotation create confusing conflicts with MFM's proposed removals, Crop Tree confirmation, manual precedence, or an eventual felling decision?
5. Is there enough grounded tree/stand state to offer each reason without implying hidden quality, unmodelled habitat value or wind response?
6. How much extra tree-by-tree clicking would this add? The feature should not increase the mandatory burden of the already-approved MFM candidate-confirmation flow.
7. If persistence is useful, who owns the annotation (tree ID, managed relationship to the target crop tree, year, intervention, source)? How to represent deletion/retirement and previous facts without rewriting append-only history?
8. What if the tree dies or is cut later? The historical reason must remain a historical **player intention**, not mutate into a claim that its benefit was achieved.
9. Is a short general lesson caveat enough to teach the same idea without another UI workflow?

## Measurable post-demo decision gate

Before agreeing to a new SPEC REQUIRED feature, show **at least two distinct defensible neighbour choices** around the same target Crop Tree in a non-production exercise, then demonstrate that a small reason prompt improves the player's explanation or later recall without falsely awarding 30% Share credit. Assess workload and whether existing S1-C/S1-D history already answers the question. No numerical ecological improvement threshold is assumed. A negative or inconclusive user result supports **Option A: no feature**.

**Decision output:** one of (1) reject extra UI and retain optional explanatory copy only; (2) approve a bounded non-persistent UX iteration; (3) request a new, separately accepted four-part spec for persisted player intentions and read-only historical review; (4) consider a distinct lesson-credit revision with its own evidence. The test does not pre-approve any outcome.

## Non-negotiable current-demo boundary

Do not change or re-qualify because of this idea:
- current Lesson 3 30% Share and existing basal-area safeguard;
- crop-tree eligibility, competitor ranking, five-rack teaching / Rack V2;
- MFM transparent proposals, individual player confirmation, manual precedence and Work Plan commitment;
- Scenario One objectives, Year-25 completion, three-species parity, economy, growth/regeneration or storm model 0;
- any save fields, event history, old-save handling, original protected anchors, Legacy40 or frozen Reference Future v1;
- S1-C/S1-D review gates, D-072 UI qualification, personal playtest, Windows build authorisation.

**Default wind limitation:** neither crown release nor retaining dense neighbours carries modelled storm consequences in the default Scenario One; do not call retention wind-safe or wind-dangerous based on absent model output.

**Project vocabulary:** player-facing discussions use natural forestry concepts such as "young trees", "woodland regeneration", "Crop Tree" and "neighbour"; internal grid bookkeeping remains technical only.

## Reopen trigger, route and qualification

**Trigger:** the complete Scenario One is qualified and the user has completed the intended personal/external demo stage, or the user explicitly authorises an earlier separate design-only review without touching the release candidate. Do not infer a demo has happened from GitHub state.

**First subsequent task:** Overall Manager reads actual beginner/player feedback and present MFM/S1-C/S1-D implementation, then commissions a light **design-only comparison A vs B**. No Unity or persistent code by default. A live substantive worker needs an individual GitHub claim, active-task lock, precise lane/worktree and a separate dispatch packet. Do not reserve a lane now.

**If a feature is later approved:** SPEC REQUIRED (new player workflow, potential persistence, cross-system interaction). Specify `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` and independent review. Use Tier-1 for design/calculation, Tier-2 for actual new feature, Tier-3 only when integrated into a milestone candidate. Protect existing objective rule and verified saves unless user explicitly approves a separate migration/design decision.

## Status of this document

This proposal is published on a documentation branch; it does not alter canonical `main`, Unity or current milestone. No active implementation, qualification or later automatic reminder is created. Dashboard ingestion/readback not verified.
