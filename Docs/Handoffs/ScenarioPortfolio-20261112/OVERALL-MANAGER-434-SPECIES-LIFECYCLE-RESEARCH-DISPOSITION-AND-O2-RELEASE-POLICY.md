# Overall Manager #425 — formal #434 research disposition, Scenario Two species and release policy

**Date:** 12 November 2026  
**Disposition:** **APPROVED AS RESEARCH INPUT WITH CONSTRAINTS / RECOMMENDED STAGING MODIFIED TO HONOUR EXISTING ACCEPTED ROSTER**  
**Canonical decision:** main `Docs/Project/decision-log.md` **D-081** records Manager resolution within the owner's D-079 eight-case progression and D-080 new-tree learning direction. **No source code, Unity, game/spec implementation or qualification authorised.**

## 1. What the research delivered and what passed

**Research result:** https://github.com/jermur788/CCF/blob/docs/eight-scenario-species-lifecycle-evidence-result-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-TREE-SPECIES-LIFECYCLE-MATRIX-RESULT.md  
**Original task:** https://github.com/jermur788/CCF/issues/434 — HANDED-OFF/CLOSED.  
**Manager:** https://github.com/jermur788/CCF/issues/425 — continuing strategy and source-authority owner.

The research satisfies its principal task: all eight cases in user-accepted order; a per-case new encountered-vs-plantable species ledger, learning stages, real annual causal outcomes and alternative site paths; explicit three-way O2 first playable species-versus-save-safety comparison; a non-scoring lifecycle rubric; five prioritized evidence gaps; primary/professional source pointers and honest no-Unity/no-calibration qualifications. It correctly resists decorative novelty, seedling count as recruitment, unearned universal 'nurse bonuses', and fruit/nut Stage 2 early drift.

**Formal research verdict: APPROVED AS QUALIFIED STRATEGIC RESEARCH INPUT.** This approves evidence organisation and design reasoning **only**; it does not approve eight new species, growth coefficients, scenes, game logic, O2 public release or the researcher’s four-species completion preference.

**Readback/branch integrity:** the result branch was compared against its Manager packet base: exactly **one added Markdown result**, one commit ahead, no game/source/spec/canon/coordination changes. Actual current game code was separately checked for the material safeguards below. Neither qualification nor Unity rendered behaviour was tested by this Manager review.

## 2. Manager findings grounded in accepted history and inspected code

| Topic | Evidence finding | Interpretation/risk |
|---|---|---|
| **Previously accepted O2 roster** | `Docs/Project/research-index.md` §Scenario Two species evidence explicitly states **'Scenario Two additions are Sitka spruce, oak, beech, Scots pine, downy birch and common alder (accepted roster)'**. Research Index is **supporting research, not decision authority**, but faithfully records this prior accepted selection. Neither D-079's **order** nor D-080's **novelty target** expressly superseded that roster. | #434's recommended four-species complete-O2 route would implicitly drop the two remaining earlier additions. **Manager does NOT approve that scope reduction.** Retain the existing six until an explicit new owner decision revises it. The D-081 strategic ruling makes that preservation clear in the higher-authority Decision Log. |
| **Real species readiness** | `Docs/ScenarioTwoSpeciesFoundation.md` and actual `ScotsPine.asset`, `DownyBirch.asset`, `CommonAlder.asset` show **HOLD** juvenile/growth parameters; `ForestTreeSpawner.cs` requires `IsSimulationReady` for registration. | No new species is currently a playable O2 feature. Research's pine 'ready for initial parameter design' (Irish stand-model reference) must not be misreported as an implemented individual-tree, full-life-cycle simulator. Birch/alder likewise not ready. |
| **World/save safety** | `Assets/ForestPrototype/ForestSaveController.cs` uses single `forest-save.json`; `ScenarioOne/ScenarioOneManager.cs` warns on wrong `scenarioId` but continues restore. | **Before any independent O2 production** require approved save identity/routing and fail-before-mutation policy. These are static source findings; no corrupting cross-load reproduced. |
| **Evidence fidelity** | Scots pine adult model reference is stand-scale Irish COFORD/GROWFOR; current source lacks a validated individual-tree mixed-age pine/juvenile/browse model. Other candidates use UK or continental transferred ecology where Irish observed quantification is weak. | Treat every model as its own evidence-bound calibration; no automatic Sitka equations or manufactured precision. Noncommercial small tree may have a smaller **causal ecology** model if the lesson needs no timber yield. |
| **Novelty ledger conflict** | #434 proposed O4 first new **downy birch** and O5 first new **common alder** conditional on O2 shipping only Scots pine. | If full O2 includes accepted birch/alder, those **cannot** be counted as fresh encounters in O4/O5. Their deeper learning is useful, but D-080 calls for an additional novel species opportunity. This is **major required research/lesson correction**, not grounds to reject the report entirely. |

## 3. Binding pre-spec release resolution (D-081, no new game work)

**Adopt the researcher's Strategy B as internal sequencing ONLY, with a stricter completed-O2 definition preserving six species:**

**Gate 1 — private engineering qualification, NOT a playable release:** first use the three Scenario One species (Sitka, Oak, Beech) to establish an independent **O2 world identity/save boundary** and credible authored irregular multi-cohort Work Plan / annual-review loop. This isolates loading, save, objective/session selection, source-to-scene state and old-world compatibility without the complexity of new-species behaviour. Label exactly **INTERNAL PROOF**, not O2 implemented/verified/accepted. It is explicitly **not user-facing complete O2** and it must not be conflated with the user’s own required Scenario One personal playtest.

**Gate 2 — true new-tree learning and roster completion:** qualify **Scots pine first as main O2 new-species lesson**, then qualify **downy birch and common alder** with appropriate adult/juvenile/site/browse/seed/visual/management behaviour at the **depth needed for their actual roles**. At least one new species must contribute a genuinely demonstrated real stage change and intervention/hold contrast across annual steps and save/load. Since the existing roster names all six, **complete O2 acceptance requires all three added species genuinely usable** where the authored site supports them. Do not require all to be planted on every soil. A declared planting-only pine model may omit **natural pine recruitment**, but then do NOT teach retain-pine-seed-tree→natural-pine-recruitment as a simulated mechanic. Scenarios are not a species collection game, and stage teaching must not imply every species completes its entire life within a chapter.

**Gate 3 — complete O2 scenario qualification and acceptance:** only after Gate 1 and Gate 2, and genuine O2 unique multi-cohort maintenance (not just Scenario One's later forest with a new label), plus bounded candidate-specific source/Unity/save/visual/annual-observation verification, can anyone ask for **complete O2** acceptance. This requires user-facing qualification as appropriate, not approval from research. No calendar estimates or automatic Tier-3 repetition assigned.

**Do NOT create, approve or implement any four-file feature spec in this #434 Manager review.** The earliest future spec must address the fundamental per-scenario save/identity policy first (four parts: `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`); separate species/lesson specs follow only under correct ownership. Old `forest-save.json`, all old scenario versions including v1–v24, Legacy40 and frozen Reference Future v1 must be preserved. Cross-load refusal must occur before any forest mutation. Scenario One remains under Manager #424 and no Windows build before user's personal playtest + explicit permission.

**Trade-off acknowledged:** a six-species O2 **will cost more and take broader source/visual qualification** than the researcher’s minimum four-species O2. That is the price of preserving earlier owner acceptance instead of silently revising scope. If the user wants a four-species complete-O2 path, request a direct superseding product decision *before* changing D-081 or commissioning the species release spec.

## 4. Lifecycle learning acceptance principle and future roster consequences

Adopt a **teaching criterion, not a universal biological age formula**: in each later scenario, identify an actual new species **encountered or genuinely plantable** on that case's site; demonstrate at least two meaningfully distinguishable growth/health/size/cover conditions *where applicable*, one defensible owner judgement/intervention-versus-hold, and one resulting authoritative **persistent** observable change after ecological advances or supported authored-stage review. A species' full biological seed-to-death cycle is not required. Different species have different roles. A holly understorey/cover species need not model merchantable timber to teach juvenile shade/browse competition, but it **must** have real saved ecology state if its behaviour is used in the learning outcome. Authored past history, conservation continuity or a mature tree's seed status may be educational observations **only**, not fabricated new regeneration.

**Revision needed for later scenarios, not yet species approvals:**

| Chapter | #434 primary proposal | After retaining O2 six | Status |
|---|---|---|---|
| O1 Inherited Sitka | Sitka/Oak/Beech | Unchanged, current demo | Existing accepted scope |
| O2 Working Mixed | Scots pine, birch/alder contingent | **Pine, downy birch and common alder in complete O2** | Owner roster preserved, model/scene/qualifications HOLD |
| O4 New Farm | Birch first encounter | Birch already introduced; consider **site-suitable hazel or silver birch**, or another scientifically justified novel forestry tree | **Research candidate only**, do not plant inappropriate open habitat |
| O3 Neglected Broadleaf | Ash health stage | Could teach **pre-existing diseased ash** condition/professional management, no routine ash crop replant | Research candidate, not ash approval |
| O7 Exposed Upland | Lodgepole pine | Conditional legacy lodgepole component without universal bog restocking | Research candidate |
| O6 Atlantic Oakwood | Holly | Holly shade/cover and oak recruitment alongside invasive/browse | Research candidate; full commercial model unnecessary if honest ecology-only |
| O5 Wet Valley | Alder first encounter | Alder already introduced; consider **site-suitable grey willow** or another credible native wet-woodland species, while deepening alder's wet-site lesson | **Research candidate only**, hydrology and willow readiness missing |
| O8 Complete Estate | Wild cherry | Cherry in genuinely suitable well-drained estate patch, no fruit-sale economy | Research candidate, Stage 2 food production unchanged |

No newly named tree above is accepted as game-ready, auto-planted or universal to the habitat. Forestry conservation constraints, protected open habitats, Irish species evidence and the chosen scenario site must be respected. Species can be revealed by observation, genuine natural recruitment (only if modelled) or site-suitable planting; this is not a score/unlock system.

## 5. Status, downstream dependencies and user actions

- **What passed:** research source organization, full eight-case matrix, truthful HOLD status, safe O2 staged strategy, causal life-stage principle. Branch diff and management source preflight.
- **What needed change:** report's suggested **four-species finished O2** conflicts with earlier six-roster record; its O4/O5 'first encounter' ledger must be revised if six delivered in O2. Research remains valid input; not a blocker to Manager's policy ruling.
- **Manager decision D-081 published on main:** stage internal save proof then preserve **six-model O2** completion; accept causal life-stage teaching criterion; no features/implementation released.
- **Next when product and delivery gates allow:** separately author an **unapproved DRAFT save/identity specification** for review, then review/approve before any code. New-species model/lesson specs **after** save gate policy; no multi-agent source/code dispatch from this intake.
- **User action now:** **none required for retaining the existing six-roster promise**; if reducing complete O2 to only pine beyond S1's trio is preferred, user must expressly supersede previous acceptance. Any future change to D-080's novel-species-every-later-case goal also needs user approval.
- **No changes:** game code, Unity, specs, scenario save/build, protected world, current milestone, live active-tasks (only #424 writes), Scenario One #424 ownership or user playtest. **Dashboard and Drive mirrors: NOT VERIFIED.** The existing #429 context refresh must use newest source including D-081, not stale earlier full-SHA snapshots. 
