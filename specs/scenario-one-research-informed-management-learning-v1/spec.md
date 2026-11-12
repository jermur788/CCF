# Scenario One — Research-Informed Management Learning V1: Truthful Copy

**Status:** DRAFT REVISION — NOT APPROVED FOR IMPLEMENTATION  
**Date:** 2026-11-12  
**Owner:** Overall Manager  
**Scope:** a **small post-demo copy-only change** correcting the remaining misleading or ambiguous forestry language. Product owner can expressly approve a different timing, but nothing in this draft changes the current pre-demo programme.  
**Original broad draft:** preserved at `docs/scenario-one-research-informed-spec-20261112-v1`; its event-persistence design is **WITHDRAWN**, not ready for implementation.  
**Four-part structure:** [spec.md](spec.md), [plan.md](plan.md), [tasks.md](tasks.md), [checklists/requirements.md](checklists/requirements.md).

## 1. Authority, scientific evidence and review disposition

Accepted authority: `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md`, `Docs/Project/game-brief.md`, and the approved MFM V1, S1-C and S1-D specs. All source-line observations must be rechecked on the **actual local candidate** before a production patch.

**Review evidence:**
- [Claude independent review #389 — NEEDS CHANGES](https://github.com/jermur788/CCF/blob/docs/claude-review-research-management-spec-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md): source code inspected without Unity; blocking hash/persistence conflict; extensive unnecessary work and tests.
- [Browser research #387 — Irish/UK competitor retention and wind](https://github.com/jermur788/CCF/blob/docs/research-ccf-marking-wind-20261112-v1/Docs/Research/ResearchIntake20261112/IRISH-UK-TREE-RETENTION-RELEASE-WIND-EVIDENCE.md): COMPLETE, HANDED-OFF, CLOSED; Manager-reviewed as supporting research, not a rule approval.
- [Browser research #388 — young-tree/regeneration language](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Research/ResearchIntake20261112/YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md): COMPLETE, HANDED-OFF, CLOSED; Manager-reviewed with source-access limits.
- [Manager disposition](../../Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-CLAUDE-REVIEW-389-AND-RESEARCH-387-388-DISPOSITION.md): original draft NEEDS CHANGES; copy-only recommendation pending product approval.

P1–P6 overlap in authorship and provide useful methods and selected empirical observations, not six independent Irish validations. No new Irish coefficients, forestry prescriptions, scientific score or simulated wind effect may be derived from these papers.

## 2. Source-level baseline: do not reimplement what already exists

- Accepted Lesson 3: **30% local Hegyi Share reduction** and **40% basal-area safeguard**, with existing teaching calibration and source evaluation unchanged.
- `ScenarioOneUiFacts` (actual file path to locate at implementation) already maps current objective ID `regeneration` to **“Areas with young trees”**, with analogous Year-100 friendly labelling. This was reported live on main by independent reviewer and must be source-verified before editing. Do **not** claim the entire jargon-removal feature is absent.
- Internal `ScenarioOneObjectives` displayName strings “Maintain regenerating cells” and “Regenerating cells” may be **persisted** in the Century Review record. **Do not rename, rewrite or normalise those internal strings**, stable objective ID or snapshot fields; presentation-by-ID is the accepted safe mechanism.
- `ScenarioOneRackReview` already has session-only **Objective 5: Retain here / Release further** decisions per Crop Tree. The player can leave a competing neighbour unmarked. **No new retention UI, personal reason field, event, persistence, grade or history drill-down belongs to this V1.**
- The ground/HUD interface already shows model light-band and present regeneration diagnostics. There is **no accepted site-specific “forest edge” classification**, and the existing light label “open” is a different game concept from P6's open regeneration environment.
- The game default **storm model 0** does not simulate wind damage, whether neighbours are removed or retained.

## 3. Functional requirements — bounded copy only

### FR-C1 — Competition Share explanation (short)

Use existing text/help placement to convey that `Share` is an **8 m local DBH/size-and-distance based Hegyi indicator**. Its calculation **does not itself assess species, crown shape, tree height, shade tolerance, light direction, timber quality, habitat, or wind stability**. Removing different neighbours can satisfy the game's **specific crown-release exercise**; it is not a universal “right tree” answer.

**Candidate concise Lesson wording** (requires final screen review):
> This lesson teaches a game-calibrated local competition target, not the only acceptable way to manage a forest.

**Optional fuller Help wording from #387 option A (candidate, not mandatory if it crowds Help):**
> Competition Share is a size-and-distance-based measure of pressure from nearby trees, not a verdict that one particular neighbour must be felled. Keeping a neighbour can serve other aims, while thinning and leaving dense areas both involve real-world stability trade-offs. The default Scenario One does not simulate wind damage, and this lesson uses a defined crown-release target rather than a universal forestry rule.

Rewrite an existing generic line **“being small is not a reason to fell”** as a narrow statement about this crown-release exercise, for example:
> In this crown-release exercise, smaller neighbours usually contribute less to the measured Share.

**Boundaries:** no change to `CompetitionShare` code, 8 m neighbourhood, eligible neighbour sets, 30%/40% rules, Crop Tree designation, grade, marking, MFM proposal/approval or Work Plan. Do not introduce a competing explanation engine.

### FR-C2 — Residual regeneration vocabulary only

Keep the existing player-facing **“Areas with young trees”** unless explicitly accepted change chooses the slightly safer #388 recommendation **“Areas with young growth”**. Neither phrase asserts exact young stems; count is tracked locations with natural modelled young-growth cohorts and/or qualifying living planted juveniles.

Check only remaining **player-visible** string cases, including “regeneration cohorts” in `WorkPlanOverview`, Oak/Beech “Establish planted ...” mappings and Help copy. Replace with truthful, natural language in **presentation mapping only**. A candidate current-purpose explanation:
> Young growth is recorded in parts of the woodland where natural tree regeneration is represented or living planted young trees are present. This does not guarantee their survival.

Oak/Beech current predicate is successful planting work **plus species presence somewhere**. Any revised player label must not promise survival of the exact originally planted trees. Keep the predicate and internal `displayName` byte-identical.

**Do not add** separate natural/planted **aggregate counts**, newly occupied-location histories, cohort survival gauges, hectares, sapling densities or new objective thresholds; existing aggregates do not support those claims without new model calculations.

### FR-C3 — General-only regeneration environments

Allow **one general Help sentence** based on #388, not a geographic/local class:
> Beneath canopy, shade can limit growth. At gap edges, light and shelter vary; open spaces may get more light but also more competing plants. Check young trees and site conditions over time.

Do **not** label specific mapped locations “edge”, “open gap”, “sheltered patch”, or infer microclimate or survival rates. Do not confuse P6 silvicultural regeneration **strips** with extraction racks. Do not place this text beside the existing light-band **“open”** in a way that equates their meanings. If no readable Help location exists, **omit rather than create a new screen**.

### FR-C4 — Default wind limitation

State once in **Help/limitations**:
> The default Scenario One does not simulate wind damage.

The external-test release/limitation note may repeat this exact known fact outside the game. Do not crowd the existing 1280×720 competitor panel with wind caveats. P1/#387 document that both retained dense stands and altered exposure after thinning can have context-dependent real-world stability implications, but this V1 makes **no numeric risk prediction or one-direction assertion**, and does not activate dormant wind settings.

## 4. Non-goals and protected contracts

Absolutely no edits under this V1 to:
- Scenario One objective values, boolean rules, Year-25 completion chronology or Year-100 comparison data;
- the 30% Share lesson rule, 40% cap, tree marking/Crop Tree designation, Rack V2, MFM proposal/player confirmation or Work Plan commitment;
- ecology, tree growth, species, regeneration biology, cash/economy, storm profiles or planting success;
- management events, any save or serializer fields, event ID logic, saved objective `displayName` or schema version;
- S1-C/S1-D history and first teaching-completion latch, current / derived source provenance;
- old saves, Legacy40 and frozen Reference Future v1 identity/layout;
- new tree inspection controls or a persistent “Retain — why?” feature;
- a biodiversity/structural-forest score, imported Swiss crown/Finnish ingrowth/competition parameters, or Bayesian forecasts.

**Known blocking architecture for the former draft:** the old FR-07 idea of new `ScenarioManagementEvent` fields would modify `JsonUtility.ToJson(save)` data used by `ScenarioReferenceArchive.HashLayout` and protected world identity. This is **removed**, not quietly replaced by a separate saved list. Future persistent annotations need their **own new spec** with explicit protected identity-hash testing and product choice.

## 5. Scope completion / acceptance

One small bounded UI-copy patch after **product approval and an available owner/lane**; no standalone major feature programme. A single implementer verifies:
1. The exact target strings exist in **current candidate source**, and which friendly objective labels already ship; only actual residual strings changed.
2. External source-backed wording is limited to supported general statements. No unexplained scientific precision or unmodelled effect.
3. No changed internal `displayName`, objective ID, stable persisted fields, event/save/schema, Share/lesson predicates or MFM/Rack logic.
4. Source-level/world identity check: identical representative save before and after text-only patch, including frozen Reference Future and Legacy40 where safe existing fixtures apply; **hash should remain identical** for same world.
5. Actual affected panel screenshots at existing supported 1280×720 and 1920×1080 sizes when a Unity/UI lane is available; no overlapping/hidden text, especially pre-existing competitor label gate.
6. Minimal source/copy verification and existing integration qualification when that release candidate is tested. **No separate full Tier-2/3 qualification**, no new Windows build.

This is a **DRAFT**. No implementation, tests, merge, Unity machine lease or acceptance is claimed by publishing it.

## 6. Deferred ideas, distinct from V1

- Optional reasoning: compare existing Objective 5 **[J] Retain here / [K] Release further** to a **non-production mock-up** of a short optional rationale, first. Do not make an entirely new neighbour-level action unless user value established and exact 8 m association and broadleaf cases are defined. No new feature automatically follows.
- Session-only rationale: a possible separate small UX spec after player feedback; no lasting history.
- Durable rationale: only with **separate SPEC REQUIRED** plus explicit product approval; a separate hash-excluded save list is a **candidate**, not accepted design. Verify exact serializer/hash strip, old saves, identity equality and S1-C adapter before proceeding. Preserve one history authority.
- Offline optional P3 structural mingling/diameter differences and Hegyi/RBA comparison: authentic fixtures first, no biodiversity interpretation; a meaningful negative result may close the idea without UI.

**Product choices still open:** exact label (“young trees” vs “young growth”), whether even a small copy change belongs after or during demo, and whether later retention UX merits a prototype. The present recommendation is **post-demo copy-only; no new retention feature until evidence**.
