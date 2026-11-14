# scen1 — Manager #424: early Oak/Beech planting accepted; later planting lesson deferred to player test

**Date:** 14 November 2026 UTC. **Manager:** actual OpenAI GPT-6, GitHub-only, no local worktree/Unity/physical lock. **Execution claim:** [#506](https://github.com/jermur788/CCF/issues/506), docs-only. **Status:** owner product choice accepted in DRAFT; **not** a formally approved implementation spec or native qualification. **Dashboard/Drive/context mirrors:** NOT VERIFIED.

## 1. Exact owner decision and its limits

At 16:43 UTC the owner selected **Option 1 of the two planting workflows** offered in the prior Manager response: **initial early establishment, rather than a preset staggered second planting episode**. The owner said to **decide during player test whether to add later planting**.

This is the third linked, narrow product choice, after:
1. **Option A:** first-demo Oak/Beech establishment is via real player planting/protection/management, with naturally recruited Oak/Beech seedlings **not required by Year40**.
2. **Initial stand Option 1:** Scenario One starts **all Sitka spruce**; no pre-existing Oak/Beech at any stage.
3. **Now accepted:** after real assessment, player introduces **both Oak and Beech through an early planting episode**. **No scripted/compulsory later planting or replanting lesson is currently required.** The owner will judge its value in a **personal player test**. Existing voluntary later planting, where current code legitimately supports it, should not be disabled solely to enforce this initial teaching plan.

The word “early” does **not** establish an exact ecological year, placement coordinate, fixed sapling number, protected/unsheltered mix, guaranteed success, nursery stock, cost, plot dimensions or level-completion threshold. These remain OPEN-C2/OPEN-C5/OPEN-C9 and related spec gates. **OPEN-C9 is partly resolved for sequencing**, not closed for cohort/site design. It does **not** approve identical-start guided/independent dual save slots (OPEN-C10), a permanent 30m geometry, a compulsory thinning schedule, outside hedgerow seed donors or a Windows build.

**Do not weaken D-060 complete-loop scope:** later annual reviews, honest protection/care and meaningful selective forestry management choices must still occur. They cannot be replaced by a forced second planting just to reach a “three-cycle” milestone, nor does removing required later planting exempt genuine later management. Existing D-059 comparable three-species biological depth also remains.

## 2. Source/evidence boundary

- Current-source `ScenarioOneManager.TryDesignateExactPlanting` requires a viable stocked Oak/Beech shop species, unreserved nursery stock, exact site inside a stand ecology cell, no duplicate open planting marker there and a minimum **0.7m distance from living stems** on designation. It generates a **Pending** Work Plan planting order, not a planted juvenile until legitimately resolved. The stem-distance guard **does not** prove adequate light, browsing safety, long-term survival, good 3D legibility or financial suitability.
- [#490 original 30m 40-year research](https://github.com/jermur788/CCF/blob/research/scen1-three-species-long-horizon-native-20261114-v1/Docs/Research/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-RESULT-20261114.md) showed one planted Oak and one planted Beech could survive/promote in a finite all-Sitka research stand. It did **not** validate a sufficiently large early cohort, exact cost, learner discoverability or any specific later planting or management-policy outcome. No new Unity or numerical results follow from this owner choice.
- #499 approved the previous #498 DRAFT quality only; #505 made the all-Sitka owner choice explicit; this #506 decision adds initial planting **sequence**, not measured growth or a new product geometry. The normal-manager death→deadwood event path remains a later targeted native witness, not proved by disabled-manager #490.

## 3. Specification update and player-test observation gate

DRAFT files updated on `docs/scen1-early-planting-player-test-gate-20261114-v1` from previous #505 authored branch:
- `specs/scenario-one-compact-complete-loop-v1/spec.md`: owner early planting, later-planting lesson deferred, OPEN-C9 PARTLY RESOLVED sequencing; no other OPEN-C decision.
- `specs/scenario-one-compact-complete-loop-v1/plan.md`: start with both species through early legitimate planting; then review, care, optional appropriate later forestry choices. No mandatory new second planting episode.
- `specs/scenario-one-compact-complete-loop-v1/tasks.md`: narrow early placement/Work Plan/site design and explicit personal player-test gate; no duplicate 40-year research.
- `specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md`: update **existing** R-105, R-107 and R-113; 115 R-IDs stay unique, contiguous and all unchecked. Requirements do not count documentation decisions as verified gameplay.

**Suggested personal player-test observations after a real qualified candidate exists:** Was the first Oak/Beech planting choice clear and genuinely player-controlled? Could the player understand site-light, browsing and shelter implications and find the planted trees afterward? Were later annual consequences and forestry choices interesting without another scripted planting? Did losses or available gaps naturally motivate voluntary replanting? Only after seeing these results should the owner decide whether an additional **taught** later planting episode improves the complete loop. Do not invent success thresholds before tests.

**Publication/scope:** only the four modified DRAFT Markdown files plus this single NEW `scen1-` handoff; no edits to `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/decision-log.md`, milestone/brief, live `active-tasks.md`, other specs, source tests, saved worlds or Reference Future v1. Do not alter paused [#471 D-082 canonical review](https://github.com/jermur788/CCF/issues/471) owned by Overall Manager #425. Future independent DRAFT review may be bundled with actual OPEN-C9 cohort/rack/economy details; **do not launch an immediate redundant review just to restate a direct owner instruction**. Full feature spec still requires independent approval and normal source/Unity ownership gates.

## 4. Closure / next action

**Manager disposition:** accepted direct product scope, recorded in DRAFT; not product implementation, integration or user acceptance. **Next:** prepare source-/geometry-grounded first-planting site and cohort options once forest footprint, rack and costs are sufficiently defined; protect user playtest as the gate for later-planting lesson. No new external worker is being dispatched in this decision-record task. The #506 author closes this docs claim [HANDED-OFF] after GitHub readback and exact diff, notifies #424 and #425/#471. **User action required now: NONE.** Dashboard/Drive mirrors not verified.
