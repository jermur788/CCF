# CCF #434 — species variation and tree-life-cycle learning across eight accepted forestry scenarios

**Date:** 12 November 2026  
**Difficulty:** HIGH: Irish species evidence and ecological constraints, teaching progression across eight scenarios, source-level species readiness, and protecting the cheapest safe second-scenario proof.  
**Best suited:** GPT-6 High or equivalent forestry-research capable browser/GitHub worker. Record actual model/provider/effort as known; do not claim a setting not independently attestable.  
**Issue:** https://github.com/jermur788/CCF/issues/434 — READY/NOT RUNNING until actual researcher starts.  
**Exact GitHub packet:** https://github.com/jermur788/CCF/blob/docs/overall-manager-d079-d080-eight-case-intake-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/MANAGER-EIGHT-SCENARIO-TREE-SPECIES-LIFECYCLE-RESEARCH-PACKET-20261112.md  
**Manager:** Overall Development Manager [#425](https://github.com/jermur788/CCF/issues/425). Scenario One Delivery Manager [#424](https://github.com/jermur788/CCF/issues/424) retains all S1 integration, live coordination and Unity ownership. No new local worktree, Editor/build rights or physical machine lease.

## 1. Why this task now, and what is already accepted

The user **accepted** the complete long-term eight-scenario *order* and specific concept-level additions on 12 November 2026. GitHub main `Docs/Project/decision-log.md` **D-079** and `game-brief.md` record:
1. **O1 Inherited Sitka Plantation** — current full Scenario One.
2. **O2 Working Mixed Forest** — established irregular multi-species continuous-cover forest.
3. **O4 New Farm Woodland** — site-before-species, plant/no-plant choice.
4. **O3 Neglected Broadleaf Wood** — tree quality/health, disease, habitat.
5. **O7 Exposed Upland Forest** — exposure and risk, deferred access concepts for future study.
6. **O6 Atlantic Oakwood** — native woodland continuity, browsing and invasives.
7. **O5 Wet Woodland and River Valley** — soil/water/access.
8. **O8 Complete Irish Forest Estate** — finite estate-local compartments and budget.

The user then added **D-080**: later chapters should keep adding tree variety, ideally **at least one newly encountered species already present or a new suitable species to plant**, teaching *tree development and lifecycle* as well as environmental differences. The spoken end of the lifecycle sentence was incomplete, so do not pretend that an exact scoring/required-life-stage rubric has been approved. A new species presented as mere visual decoration cannot be claimed to teach a full simulated lifecycle. Any proposed minimum one new species per case is **curriculum direction**, not approval of a specific species asset or new growth formula.

Accepted separate scope:
- **O4 simultaneous year-zero temporary nurse and long-term target planted together** = OPTIONAL LATER challenge, not required in first O4 release; Irish observed pine/larch risks are conditional. Another mode is pioneers planted first and crop underplanted later. No instant mature shelter or magic benefit.
- **O3** old coppice history/recognition, not full stool-regrowth mechanic in the core eight; optional later forestry extension subject to separate decision.
- **Stage 1** fruit/nut trees can be recognised as forest species but have no edible produce yields, processing or orchard economy. Full fruit/nuts are first **Stage 2** slice (D-054).
- **O7** may evaluate historic diagonal-rack/access-spine research, but Scenario One's existing five parallel rack workflow is NOT changed.
- **O8** one finite authored world, 3–4 proposed compartments and an estate-local budget; no money/trees shared across separate scenario saves.
- **First external demo is complete Scenario One** and stays entirely under #424 (Sitka/Oak/Beech parity, Rack V2, MFM, annual Work Plan and repeated interventions). Do not move its scope or start O2 code. No Windows build before the user's personal playtest and explicit authorisation.
- **Before any O2 code:** independent scenario save-slot/identity and pre-mutation wrong-scenario load rejection, preserving old `forest-save.json`, old saves, Legacy40 and frozen Reference Future v1. Requires an approved four-part spec and qualified tests; this research packet does not authorise it.

## 2. Sources: distinguish authority, implementation and ecological research

First read live main D-051–D-054, D-059/062, D-078–D-080, Game Brief, Current Milestone, Research Index and Unity Overview, plus [accepted Manager intake](https://github.com/jermur788/CCF/blob/docs/overall-manager-d079-d080-eight-case-intake-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/OVERALL-MANAGER-425-EIGHT-SCENARIO-APPROVAL-AND-TREE-VARIETY-INTAKE-20261112.md). Research reports are proposal/evidence, not implementation:
- [Original eight-case study](https://github.com/jermur788/CCF/blob/docs/independent-eight-irish-forest-scenarios-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/INDEPENDENT-EIGHT-IRISH-FOREST-MANAGEMENT-SCENARIOS-20261112.md).
- [Three-order research and engineering gap synthesis](https://github.com/jermur788/CCF/blob/docs/eight-scenario-curriculum-refinement-result-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-CURRICULUM-DEPENDENCY-REFINEMENT-RESULT.md).
- [Simultaneous nurse research](https://github.com/jermur788/CCF/blob/docs/pommerening-instant-new-ccf-research-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/POMMERENING-INSTANT-NEW-CCF-BARE-LAND-RESEARCH-20261112.md).
- [Recent dual-Claude critique / Manager source correction](https://github.com/jermur788/CCF/blob/docs/overall-manager-432-review-disposition-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/MANAGER-425-INDEPENDENT-CLAUDE-432-REVIEW-DISPOSITION-AND-BLIND-RERUN-DECISION.md).

**Inspected existing game source (read-only):** `Docs/ScenarioTwoSpeciesFoundation.md`, `Assets/ForestPrototype/Species/ScenarioTwo/{ScotsPine,DownyBirch,CommonAlder}.asset`, `Assets/ForestPrototype/ForestTreeSpawner.cs`, current species definitions/registries, saved tree structure, growth and regeneration model rules. Existing three-species current Scenario One has Sitka/Oak/Beech at accepted comparable depth but not equal precision; pine/birch/alder exist as **HOLD / not simulation-ready**, and evidence summaries must not be mistaken for released models. Verify actual source before repeating any claim. Check exact spelling and source.

**Evidence sources:** project's Irish woodland ecology, Species Evidence Pack, Scenario Two Species Evidence Closure, GROWFOR extraction, five composite reports (growth/density, CCF economics/practice, ecology/regeneration, browsing/protection), Irish COFORD nurse oak research, NPWS native woodland management guidance, primary Teagasc/Forest Research/Irish forestry source papers as material. Prefer existing credible coverage rather than new unscopeable literature survey. Flag location/geography, empirical observations, ordinal transfer, gameplay calibration and untested model design separately. Sources that are inaccessible are NOT independently verified.

## 3. Mandatory deliverables — one useful, bounded matrix

**A. Eight-case species and lifecycle progression table**, in **accepted order** (O1,O2,O4,O3,O7,O6,O5,O8), one row per case including:
1. Existing active species / species already present; which one is new to the **player's previous chapters** (avoid counting O1 Sitka/Oak/Beech as new again in O2).
2. Candidate genuinely *new* tree species encountered **in a stand** versus one that is **site-suitably plantable**, with a conservative alternate if Irish evidence/readiness fails. Do not force a species onto an unsuitable wet/peat/native habitat.
3. Tree age/size stages that can be shown honestly; one **species-specific learning dilemma** and at least one observable real outcome after years of growth/management, including seedling-to-sapling versus genuine crop recruitment distinctions.
4. Present source/asset/readiness: simulation-ready, HOLD, partial parameter research, unimplemented, visual-only candidate. Identify actual gameplay treatment plus persistence and scene art implications.
5. Site suitability/conservation and Irish evidence strength, time/verification risks, and first needed feature spec or research decision. Include real 'do not plant' options.
6. Distinguish **new species learning** from **new mechanism** (e.g. O2 crown/seed tree decisions; O4 area planting; O3 ash health; O7 wind; O6 invasion; O5 water; O8 estate trade-offs).

**B. Explicit first post-O1 tension**: Previous O2 'cheap independent second forest' starts with Sitka/Oak/Beech and no new species. D-080 asks for a fresh species/lifecycle opportunity in every later scenario. Compare at least three honest strategies:
- introduce a suitable new **simulation-ready** species already in O2 at first playable release (research/calibration cost and risk);
- phase delivery via a separately **approved** initial O2 forest/save proof and subsequent required new species before calling the **complete O2 scenario** accepted (this is a risk-managed staging plan, not silent scope removal);
- provide tree-variety learning via a genuinely *new* plantable species/seed-to-sapling pathway rather than mature adult stands; must still be evidence-suitable, authoritative, saved and visually observable.
For each, show species candidate, why it fits a working mixed Irish CCF forest, the minimum model required, cross-save compatibility, educational value, **Low/Medium/High** relative development/research risk (no invented duration), and what the owner would need to decide. Do not quietly resolve against user intent or claim a static label satisfies longitudinal tree biology.

**C. Life-stage teaching rubric proposal (NOT acceptance):** Show a concise shared educational vocabulary for *plant or germinate → establish/juvenile → compete/grow → recruit/reproduce → mature/manage/retain → replace/regenerate*, but adapt to species and historic pre-existing stands. Which stages/decisions must be causal/persistent vs observational? What is a credible stop point in one scenario? Explain uncertainty and avoid guaranteeing that a 100-year tree completes its full lifecycle within a demo.

**D. Targeted research gaps** (top 5, not 30), with exact next sources/needed data and which scenario depends on them. Distinguish present pine/birch/alder HOLD from absent ash/larch/hazel/other species and from unvalidated nursery/vegetative regrowth. Do not assume larch or willow are approved planting species for a particular site.

**E. Manager decision recommendation:** minimal species/pacing matrix for each of eight; single preferred O2 approach plus a credible alternate, a couple of required owner choices, and a clear implementation-safety gate. If evidence cannot support a new species for one particular case, report it as a blocker/research gap **rather than declaring the education target impossible or swapping in fantasy numbers**.

## 4. Exact branch, claim, review and handoff

- **Source branch:** `docs/overall-manager-d079-d080-eight-case-intake-20261112-v1`, GitHub manager docs packet; main has D-079/D-080 already.
- **New worker result branch:** `docs/eight-scenario-species-lifecycle-evidence-result-20261112-v1`, create only if verified absent. Based on packet branch after inspecting latest HEAD and live owner/claim.
- **Exact single writable result file:** `Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-TREE-SPECIES-LIFECYCLE-MATRIX-RESULT.md`. The worker MUST NOT overwrite existing documentation or create new active specs. No source/Unity/system-wide lock, no local worktree implied.
- **Task claim:** [#434](https://github.com/jermur788/CCF/issues/434). At actual start mark **[RUNNING]** with provider/model, branch, browser-only lane and starting HEAD. Before writing read current GitHub `Docs/Project/coordination/active-tasks.md` READ ONLY and confirm no conflicting ownership; if can't establish safe path mark **[BLOCKED]**. #424 alone may edit the coordination file; the Manager can arrange the registration separately when appropriate.
- **Finish:** publish the new report, fetch/read back, compare against packet base to prove only the new result Markdown was added; link exact result; mark **[HANDED-OFF]** and close claim if actual deliverable is complete. Report source verification, zero Unity/coding, dashboard/Drive UNKNOWN unless verified.
- **Manager next:** overall Manager #425 reviews the report, gives APPROVED / NEEDS CHANGES / REJECTED research disposition, and if material O2 species strategy or release policy remains open, returns it to the user; no O2 code or new specimen scheduled from research alone.
- **No work on:** Current Scenario One source, Unity, environment/world/save, existing approved specs, `current-milestone.md`, `active-tasks.md`, main, parent #424, game build, user test or Stage 2. Do not fabricate a new species model or grant/species fidelity.

**Paste-ready research dispatch:**

> Continue as CCF eight-scenario tree-species and lifecycle evidence researcher, claim #434, HIGH difficulty, preferred GPT-6 High browser/GitHub research. The owner approved the eight-case order but additionally wants genuine new tree variety and learning about tree development across successive scenarios. Read the entire GitHub packet: https://github.com/jermur788/CCF/blob/docs/overall-manager-d079-d080-eight-case-intake-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/MANAGER-EIGHT-SCENARIO-TREE-SPECIES-LIFECYCLE-RESEARCH-PACKET-20261112.md . Produce one evidence-classified eight-case species/stages/plantable-vs-present matrix, inspect current species readiness, and explicitly compare options for adding a real new species in O2 without breaking its safe second-forest proof. Work docs-only on the exact new result branch/file; register #434 RUNNING before work and HANDED-OFF/CLOSED after GitHub readback. No source code, Unity, save modifications, product scope changes or Scenario One interference. Return the direct published result URL and the few real product decisions.
