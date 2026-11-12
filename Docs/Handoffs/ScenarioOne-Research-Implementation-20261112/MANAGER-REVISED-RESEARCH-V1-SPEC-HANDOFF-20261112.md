# Manager handoff — revised Scenario One research-learning V1 after Claude and two browser reports

**Date:** 2026-11-12  
**Verdict:** **Original broad spec NEEDS CHANGES; narrowed V1 DRAFT published, NOT YET APPROVED FOR IMPLEMENTATION.**  
**Manager:** CCF Overall Manager  
**Reason for revision:** Claude's independent review identified a blocking world-identity/persistence conflict and excessive scope/testing. Both targeted browser research tasks returned sufficient qualitative evidence for conservative education copy without new mechanics.  
**No Unity/code changes, no new local worktree or source lease, no playtest or build, no acceptance claimed.**

## Evidence received and disposition

| Work | Status | Durable result | Manager disposition |
|---|---|---|---|
| Independent Claude four-part spec review, issue [#389](https://github.com/jermur788/CCF/issues/389) | HANDED-OFF, CLOSED | [Full critique](https://github.com/jermur788/CCF/blob/docs/claude-review-research-management-spec-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md) | **APPROVED AS REVIEW EVIDENCE**; blocking saved-event approach rejected; other major scope corrections accepted |
| Irish/UK marking and two-sided wind research, issue [#387](https://github.com/jermur788/CCF/issues/387) | HANDED-OFF, CLOSED | [Primary-source research report](https://github.com/jermur788/CCF/blob/docs/research-ccf-marking-wind-20261112-v1/Docs/Research/ResearchIntake20261112/IRISH-UK-TREE-RETENTION-RELEASE-WIND-EVIDENCE.md) | **ACCEPTED AS SUPPORTING QUALITATIVE EVIDENCE** for explanation/caveats, not coefficients, player score or feature approval |
| Irish regeneration terminology and young-growth research, issue [#388](https://github.com/jermur788/CCF/issues/388) | HANDED-OFF, CLOSED | [Primary-source research report](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Research/ResearchIntake20261112/YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md) | **ACCEPTED AS SUPPORTING QUALITATIVE/WORDING EVIDENCE**, not permission to add new aggregation |
| Earlier retained-competitor concept, [#386](https://github.com/jermur788/CCF/issues/386) | DEFERRED | [Design opportunity](https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Handoffs/ScenarioOne-Objectives-Six-Paper-Review-20261111/POST-DEMO-COMPETITOR-RETENTION-RATIONALE-DESIGN-OPPORTUNITY.md) | Keep as optional design-only experiment; no implementation claim |

**Source limits:** The Claude reviewer examined source but did not compile/Unity-verify; the Manager checked the critical save/identity and session-only decision contracts directly via GitHub, not the live Editor. The two browser reports cite Irish and UK sources; the Manager has not individually reopened every external primary document, so they are supporting evidence for bounded text, not authority to introduce new factual precision.

## Revised four-part spec — direct GitHub links

**Exact branch:** `docs/scenario-one-research-learning-v1-copy-revision-20261112`. This branch remains separate from canonical `main`.

1. [spec.md — actual proposed feature boundary](https://github.com/jermur788/CCF/blob/docs/scenario-one-research-learning-v1-copy-revision-20261112/specs/scenario-one-research-informed-management-learning-v1/spec.md)
2. [plan.md — minimal source patch and proportional testing](https://github.com/jermur788/CCF/blob/docs/scenario-one-research-learning-v1-copy-revision-20261112/specs/scenario-one-research-informed-management-learning-v1/plan.md)
3. [tasks.md — one proposed Low-difficulty copy task, plus separately deferred features](https://github.com/jermur788/CCF/blob/docs/scenario-one-research-learning-v1-copy-revision-20261112/specs/scenario-one-research-informed-management-learning-v1/tasks.md)
4. [checklists/requirements.md — three gates and eight focused acceptance checks](https://github.com/jermur788/CCF/blob/docs/scenario-one-research-learning-v1-copy-revision-20261112/specs/scenario-one-research-informed-management-learning-v1/checklists/requirements.md)

**Formal Manager review:** [#389 + #387/#388 disposition](https://github.com/jermur788/CCF/blob/docs/scenario-one-research-learning-v1-copy-revision-20261112/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-CLAUDE-REVIEW-389-AND-RESEARCH-387-388-DISPOSITION.md).

The original complete draft remains preserved (and **not approved**) at `docs/scenario-one-research-informed-spec-20261112-v1`. Do **not** dispatch from its superseded event/persistence recommendation.

## Proposed V1 — small post-demo copy, not a new system

1. **Share/Crop Tree guidance:** say Share reflects current local DBH/distance competition, not species, height, crown/light, habitat, timber quality or wind. Explain that 30% is the existing crown-release **teaching target**, not the only acceptable forestry approach; narrow the generic "being small is not a reason to fell" sentence to this exercise.
2. **Young growth:** do **not** reimplement the already-present "Areas with young trees" player label; possibly change presentation mapping to "Areas with young growth" after product wording decision. Fix only residual jargon and misleading Oak/Beech establishment claims. **Do not change persisted internal display names or objective IDs.**
3. **General canopy/help:** describe shelter/edge/open as **conceptual conditions**, not named labels at specific game locations; existing light-band "open" is a distinct game-defined category.
4. **Wind:** one Help/limitations sentence, "The default Scenario One does not simulate wind damage." Do not add an unsupported wind coefficient or crowd the existing competitor UI.
5. **No new input, persistence or game scoring.** Existing Objective 5 retain/release is session-only. Player can retain neighbours by not marking them.

## Specifically rejected from V1

- FR-07's suggested new `ScenarioManagementEvent` with added fields or appended intent events: **blocked by protected `HashLayout`/world identity and insufficient product evidence**.
- Separate new retention UI competing with Objective 5; new save fields; rewritten S1-C management history; new natural/planted aggregate counts; site-specific "forest edge" tagging; forest-structure player scores; any Swiss/Finnish imported parameters.
- Overlong 69-item checklist, new duplicated Unity test campaigns and multiple overlapping writers for a text change.

**Future persistence note:** A separate hash-excluded intentions list is **one option**, not an approved or automatically safe implementation. A new spec would need exact source/serialization checks, old saves, protected world identity and Reference Future equal-hash tests, plus explicit product authority. The less costly option is to stop with copy.

## Next steps and gates

1. **Now (Manager):** record this revision and review status; no new browser research needed for conservative copy.
2. **Product decision:** approve or modify **post-demo copy-only V1**, including whether to keep "Areas with young trees" or use "Areas with young growth". This is not implied by reviewing this document.
3. **Once approved and safely schedulable:** Manager inspects latest `Docs/Project/coordination/active-tasks.md`, verifies current integrated source branch/HEAD and actual UI/Unity lock, then publishes **one** bounded, Low-difficulty Sol/Codex implementation packet with exact **new** GitHub issue claim, local branch/worktree, permitted files, Tier-1/limited visual acceptance and durable result path. No worktree or claim is allocated now.
4. **Existing first-demo milestone:** continues unchanged; no Windows build without personal candidate playtest and explicit approval. No premature Tier-3 on the copy task.
5. **Later optional UX research:** if user observes an unmet teaching need, compare current Objective 5 retain/release against a non-production reason mock-up. If negative, do nothing. Durable history needs a separate spec.

**Current statuses:** research COMPLETE (two reports); Claude review COMPLETE (NEEDS CHANGES original); revised copy V1 DOCUMENTED/DRAFT (unapproved); Unity implementation NOT STARTED; dashboard/Drive context sync NOT VERIFIED.

**User action required now:** none to continue existing demo. To schedule future V1 gameplay copy, user accepts its scope/timing and optionally chooses final young-growth term.
