# Scenario One — research follow-on implementation roadmap

**Prepared:** 2026-11-12 by Overall Manager #200. **Updated:** 2026-11-12 following direct user priority decision D-073. **Status:** multi-year decision/outcome feedback priority CONFIRMED; feature design/implementation remains DRAFT / NOT RELEASED. **Product aim:** convert worthwhile research into choices and visible outcomes, while keeping the approved complete-loop three-species Irish CCF reference scenario stable. **Scope distinction:** approved copy-only FR-C1–FR-C4 is implemented, source-reviewed and Unity-checked on a feature branch, **not yet integrated**; focused integration gate [#393](https://github.com/jermur788/CCF/issues/393) is READY/NOT RUNNING.

## LATEST — Confirmed multi-year pacing and two advance modes (D-074 / D-075)

The user confirmed Scenario One should teach spaced substantive CCF interventions over multiple years rather than encourage consecutive-year thinnings, **while keeping necessary planting/protection/maintenance legitimate between major interventions**. The user expressly chose **two controls**: `Advance 1 Year` or `Skip to Next Management Year`. The skip must loop real annual simulation, keep each annual history/snapshot and stop for user judgment and pending/approved work; the destination is a suggested **management review year**, not an automatic felling order or player-action objective completion.

[Separate four-part DRAFT](../../specs/scenario-one-management-cadence-and-advance-v1/spec.md) defines this cross-system design; [independent design review #396](https://github.com/jermur788/CCF/issues/396) is READY/NOT RUNNING with [full review packet](MANAGER-CCF-CADENCE-TWO-ADVANCE-OPTIONS-DESIGN-REVIEW-PACKET.md). The exact review-year algorithm, significant intervention distinction, objective/pacing interaction and old-save compatibility require review and explicit approval before production coding. Product direction is confirmed; feature is **not yet implemented or source authorised**.

[Source audit #394](https://github.com/jermur788/CCF/issues/394) is **HANDED-OFF / CLOSED**, with [readback source report](https://github.com/jermur788/CCF/blob/docs/scenario-one-multiyear-feedback-source-audit-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MULTIYEAR-FEEDBACK-SOURCE-AND-UX-DELTA-AUDIT.md) confirming existing history/year browser and a genuine decision→subsequent-outcome connection gap. [Copy-only integration qualification #393](https://github.com/jermur788/CCF/issues/393) is also HANDED-OFF/CLOSED: source-equivalent world identity PASS and nursery warning FAIL on both baseline/candidate; final integration still requires Manager decision. Optional retained-neighbour reasons #386 remain deferred.

## What the evidence actually supports

- The six-paper extraction and [#387 Irish/UK tree-retention/crown-release/wind research](https://github.com/jermur788/CCF/blob/docs/research-ccf-marking-wind-20261112-v1/Docs/Research/ResearchIntake20261112/IRISH-UK-TREE-RETENTION-RELEASE-WIND-EVIDENCE.md) support *choice-dependent management rationale* and the idea that one Share ranking is not a uniquely correct forestry prescription. Retained neighbours can have timber, canopy, possible habitat or future-management value; the game does **not** model quantitative wind protection.
- [#388 Irish regeneration/young-growth research](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Research/ResearchIntake20261112/YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md) supports distinguishing *young growth observed* from *successful recruitment*, and natural versus planted origins where genuinely recorded. Light, browsing, understorey and gap-edge context matter qualitatively; the game has abstract natural cohorts and exact planted juveniles, not a universal seedling-to-sapling census.
- P2's human marking comparisons discourage teaching one uniquely correct competitor selection. P3 offers local species mingling and diameter-differentiation **methods**; P5 offers competition-method alternatives; P4/P5 and Finnish/Swiss coefficients do not validate new Irish growth or wind parameters. These are proposals until scenario-specific fixtures and separate decisions.
- [Opus/Claude independent critique #389](https://github.com/jermur788/CCF/blob/docs/claude-review-research-management-spec-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md) **blocks** the old proposed `ScenarioManagementEvent` retention-reason design: appending new serialized fields/events changes protected world-hash identity. The earlier broad four-file draft remains historical, not an approved spec. Current [J] Retain here / [K] Release further is already available **per Crop Tree and session-only**, not durable per retained neighbour.

## Sequenced opportunities — UPDATED by confirmed D-073 (2026-11-12)

**Priority decision:** The **multi-year management decision → outcome review** is required whether or not the optional tree-retention-reason feature exists. Therefore move former Order 2 to **Order 1 / required** and make **young-growth trajectory an important next case**, while former Order 1 retention reasons are **optional and deferred**, not a blocker. The concise summary in chat previously used different row numbers for regeneration; this plan names each feature to avoid mistaken dispatch.

| New order | Player value | First safe slice | Gate / actual status |
|---|---|---|---|
| **0 — Finish C-01** | Integrate corrected forestry educational copy on the updated UI source | [#393](https://github.com/jermur788/CCF/issues/393): saved-world hash and nursery visibility, Manager exact-lineage merge decision | READY / NOT RUNNING; no disruptive new Unity task |
| **1 — REQUIRED: Management decision ↔ later results** | The player can distinguish what they decided, what the Work Plan actually executed, what the game observed in subsequent years, and what remains a consideration for the next intervention | **DRAFT four-part specification and evidence delta audit first**, based on already-approved S1-C annual history and S1-D repeated interventions. Use current recorded events, work orders, annual ecological snapshots, objective milestones and in-world/current model facts. One concise timeline or linked review may suffice; no second history store | Confirmed product priority D-073, **not yet approved implementation design**; inspect latest source, user-facing gaps, saved/current fields and no duplication. Feature scope SPEC REQUIRED |
| **2 — Young-growth changes over time** | Explicitly compare recorded natural/cohort presence and planted juveniles, where measurable, so users can see persistence versus transient growth without fabricated species success | Source/data inventory on authentic two-year fixture; integrate into Order 1's review *if* genuine year data and surface fit; otherwise a later separate approved four-part spec | Never treat represented cohort portions as seedling numbers or recruitment success. Browser #388 supports conservative wording, not extra empirical coefficients |
| **OPTIONAL LATER — Tree-retention intentions** | Some players may want to record why they intentionally keep a high-Share neighbour and revisit it years later | [#386](https://github.com/jermur788/CCF/issues/386) optional post-demo mock-up: compare existing [J]/[K] and no-extra-UI with contextual “Retain — why?”; user has **not** chosen this feature | **Deferred / uncertain**. Do not inject reason input or persistence into Order 1. Prior proposed `ScenarioManagementEvent` extension breaks protected world hash; needs new spec/compatibility proof if ever selected |
| **Research-only — Structure and mixture** | Potential future stand-structure information (size differentiation, species mingling) | Offline P3 boundary-aware metrics on authentic saved same-world years; compare Hegyi versus P5 relascope alternative without parameter import | No player panel/score without meaningful signal and separately approved spec |
| **Longer-term — Model enhancements** | Richer growth, canopy, regeneration or wind processes only if justified | Dedicated Irish/UK applicability and quantitative parameter review; model impact and frozen-reference/saves threat analysis | P4/P5 Swiss/Finnish coefficients not accepted Irish calibration. No new default storm model or invented wind odds |

## Updated release gates

**Phase A — immediately:** finish current C-01 integration gate #393. **In parallel, documentation only**, inspect the latest actual S1-C/S1-D annual reports, history/provenance and existing UI before drafting what feedback is genuinely missing. Publish a four-part **DRAFT** research-informed multi-year feedback V2 spec with exact data-source and UX boundaries, not an automatic Unity implementation dispatch. The required player goal is approved; the new UI/data architecture is not.

**Phase B — after explicit approval of the feature spec and safe current UI source/owner:** implement the smallest truthful linked review using existing recorded work and annual observed outcomes; independently review where history/source validity requires; use focused 720p/1080p and real save/Reference Future compatibility tests. Reuse the updated UI lineage only after C-01 integration decision. Do not add a new event/history persistence model unless new proof and explicit user decision justify it.

**Phase C — young growth:** use actual multi-year natural-cohort and planted-juvenile data to include progression honestly in the required review. Where current saved data cannot justify a longitudinal claim, show **not recorded** or keep it out. Write separate spec only if meaningful new aggregation or workflow is needed.

**Phase D — optional and research-only:** leave #386 tree-retention reasons deferred. Structural metrics and more ambitious canopy/wind/economy simulations remain research prototypes until authentic fixture evidence and separate product authorisation. Do not open a source claim for these because Order 1 is required.

**Qualification discipline:** proportionate source, dataset and Unity checks; no new full milestone Tier-3 campaign for every presentation fragment. Keep the real current complete-loop demo qualification distinct, retain user #382 protected lane, one source writer, no Windows build without personal playtest and direct user authorisation.

## Authority and decisions

- **Confirmed required follow-on (D-073):** management decisions and subsequent recorded outcomes must be more intelligible to the player, **regardless of whether retained-neighbour reasons are ever added**.
- **Not yet approved:** exact UI journey, new history summaries, further aggregation, source edits or save changes; first inspect the actual S1-C/S1-D evidence and write a four-part DRAFT.
- **Deferred:** retained-neighbour intention UX [#386](https://github.com/jermur788/CCF/issues/386), especially persistence. No new player reason is required for multi-year history to work.
- **Planned research:** young-growth chronology, structure metrics and wind/crown modelling remain evidence-gated as above.
- **Current work stays separate:** [#393](https://github.com/jermur788/CCF/issues/393) research-copy integration qualification. No change to current candidate, Legacy40/frozen Reference Future v1 or user playtest by this roadmap.
- **Next Manager action:** publish the design/source-audit packet and four-part DRAFT on this own docs-only branch; Manager review and later implementation acceptance gate follow evidence. There is **no local branch/worktree or Unity lock** for the design-only phase.
- **User action:** none for updating this plan; new substantive implementation beyond current demo requires approved spec and release into a safe lane.

concept](https://github.com/jermur788/CCF/issues/386).
- **Proposed, not approved:** a new review-linked intention feature; regeneration lifecycle data view; structure panel; alternate competition index; wind/crown or ecology-model changes.
- **No automatic implication:** a retained neighbour is not actually wind-safe; light-band "open" is not an inferred silvicultural gap class; species mix does not guarantee growth gains; Share or an annotation is not a professional tree-quality score.
- **Integrate only on updated UI lineage** after real locks/review/tests; protect frozen Reference Future v1, Legacy40, personal #382 tests/saves, current demo and explicit Windows-build gate.

## Suggested next Manager action

1. Keep C-01 focused integration #393 as the **sole current source/Unity follow-up**.
2. Prepare one **research-informed retention/decision-feedback V2 DRAFT** (not an implementation dispatch) after reading the existing [J]/[K] interaction and exact current S1-C fields. Start with two annotated screen sketches and a decision table, avoiding all new persistence.
3. Take a product decision from user evidence: pursue retained-competitor reasons, existing-history better feedback, regeneration trajectory, or no extra interface. After selection, approve the corresponding new four-file spec and create bounded worker claims.
4. Do not use the deferred #386 as a writer lock or label new concepts IMPLEMENTING/VERIFIED before actual evidence.

**User action now:** none required merely to preserve this draft roadmap. Any shift of significant gameplay work into the current demo, persistent intent storage, new tree/ecology parameters or budget escalation requires explicit product approval.

**Publication:** GitHub-only docs branch; not merged into main. Dashboard and Drive mirrors for this draft not synced; authoritative coordination remains `Docs/Project/coordination/active-tasks.md`.
