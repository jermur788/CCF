# Scenario One sequential objectives V1

Task: CCF-S1-SEQUENTIAL-OBJECTIVES-IMPLEMENTATION-V1. Implementation base: `01ce9a4835888e80ebd8f25a8d88c47b518565b6`; locked context: `36cd791b329189e0ae38931d06ae51af1a1173ce`. Implementation/design record, not a new canonical Decision Log ruling.

## Curriculum and ordering

Eight teaching objectives, with explicit phases within them, sit above the existing eight success goals. Equal counts are coincidental: these are different sets. Initial assessment belongs in the short introduction (option A). Rack stays Objective 1. This preserves the approved onboarding, avoids a compulsory inspection chore, and explains what to look at before controls. More detailed assessment follows the first intervention and uses the changed stand to make observation purposeful.

Final sequence: introduction/assessment → 1 access racks and annual execution → 2 review access results and choose future trees → 3 plan selective release → 4 retain fallen wood and resolve the intervention → 5 review response/repeated management → 6 next generation with protection decision → 7 observe establishment after another year → 8 continue transformation against the scenario goals.

The browser proposal informed the curriculum; it is not an accepted silvicultural recipe. Differences: assessment is introductory, the access observation is part of future-tree selection, habitat is an explicit lesson at the felling-material decision, and second-cycle teaching follows the selective release rather than requiring a gratuitous third harvest after planting. Planting/protection share one lesson because the existing work order stores that decision before execution. Pruning and clearance remain optional contextual practice, avoiding an unsuitable mandatory treatment.

## Lesson specifications

### Introduction — Your forest, your goal

- Concept/why: transformation of an even-aged Sitka plantation toward continuous cover; assess before acting.
- Action: optionally compare rows, tree size and ground light. No numbered assessment gate.
- Completion: dismiss the short premise; this does not count as forest management or scenario success.
- Time/observe: no advance. Later visits compare the starting stand with openings and recruitment.
- Player copy: even-aged premise, transformation goal, brief assessment cue, access as the first objective.
- Visual: original three-panel plantation → retained trees/openings → varied woodland; schematic rather than guaranteed model outcome.
- Dependencies/classification: existing introduction and inspection; **PRESENTATION CHANGE**.

### 1 — Create access racks (`access`)

- Main concept: deliberate permanent access, before later management of retained trees.
- Action: choose origin, aim forward, confirm; finish authored routes with map assistance; review costs, approve and execute.
- Completion: existing `RackLayoutTracker.Harvested` with ecological year > 0. Legacy geometry uses completed felling as its available access proxy; no new authored layout is invented.
- Time: explicit annual resolution, normally year 0 → 1. No next concept before execution. Pending/approved phases retain rack guidance.
- Observe afterward: opened route and standing trees either side, compared with annual work/cost results in lesson 2.
- Explanation: permanent routes let later work reach trees while retaining the forest between them.
- Visual: preserve independently approved original rack plan view, fixed origin and forward arrow.
- Dependencies: rack planner/layout, Work Plan, approval, annual execution. **EXISTING + PRESENTATION CHANGE**.
- Preserve partial/off-layout explanation, qualification, cancellation and reselection unchanged.

### 2 — Choose future trees (`future-trees`)

- Main concept: positive selection, paired with returning to assess the first intervention.
- Why here: the player can compare access work with what remains before deciding what to favour.
- Action: read/acknowledge actual annual results; walk back, inspect and compare; mark at least one living Crop Tree.
- Completion: saved review acknowledgement after the last completed access-route felling year plus a living Crop Tree. Current marks reconstruct selection after load.
- Time: none required. Early marks do not bypass the access review phase.
- Observe: tree quality/competition and ground light alongside the newly opened route.
- Explanation: a Crop Tree is chosen to retain and favour for future development; no arbitrary three-tree quota.
- Visual: look at stand → compare neighbours → highlighted future tree.
- Dependencies: tree inspection, crop marks, saved review chronology. **NEW OBJECTIVE LOGIC** over existing mechanics.

### 3 — Release a future tree (`release`)

- Main concept: selective competitor removal, retaining forest structure.
- Why here: positive selection gives a reason for removal, rather than a target felling percentage.
- Action: inspect crop-tree competitors, mark a relevant neighbour and bring it into Work Plan; read predicted release and residual stand.
- Completion: a valid open felling order created in/after the last resolved access-route felling year, within the existing Hegyi competition cutoff of a living Crop Tree; or completed later felling within that same competition cutoff. Access chronology uses the existing authored corridor coordinates/width to exclude split-year rack work from later release evidence.
- Time: no advance yet. The following material/habitat decision belongs before execution.
- Observe afterward: review predicted versus actual response after execution; do not promise growth in one year.
- Explanation: a selected neighbour may restrict growing space; more felling is not automatically better.
- Visual: crowded crowns → selected competitor → retained tree and growing space.
- Dependencies: existing crop/competitor analysis, marks, validation and work orders. **NEW OBJECTIVE LOGIC**. The cutoff is used only as a teaching relevance test, not a new felling qualification rule or prescription.

### 4 — Keep some fallen wood (`habitat`)

- Main concept: ecological and material consequences of retaining felled timber.
- Why here: its disposition is selected before the same release work is approved.
- Action: choose **Leave as deadwood** for a felling job if needed; compare sale forgone, cost and retained canopy; approve and advance.
- Completion: later felling successfully resolved and fallen wood goal achieved or successful historical retention recorded. Previously present habitat is credited; no extra felling is required merely for a button exercise. Later decay does not erase a learned retention action; current success targets still reflect decay.
- Time: normally one annual resolution after planning. Failed/unapproved orders do not count.
- Observe: retained canopy and the fallen log; compare work, habitat and money in Annual Review.
- Explanation: felled timber can remain as habitat rather than be extracted, at the cost of its sale.
- Visual: selected felling → material choice → log under retained trees.
- Dependencies: existing material outcomes, orders/events and deadwood goal. **NEW OBJECTIVE LOGIC**.

### 5 — Review and manage again (`response`)

- Main concept: adaptive repeated management, through evidence of response.
- Why here: access and selective release form two real interventions in distinct years; no artificial extra harvest.
- Action: read/acknowledge latest release results, compare intervention history, return to the opening, crop trees and fallen wood.
- Completion: acknowledgement after the latest later felling plus existing `HasReviewedTwoCycleSequence`. That protected rule requires an acknowledgement between two successfully resolved plans in distinct years. A nonstandard save missing that chronology is told to review before a later justified plan.
- Time: no prescribed additional interval; if chronology is missing, another actual later plan is needed under existing rules.
- Observe: work executed versus intention, cost/revenue, canopy/light, growth, regeneration present or absent, other deaths. Neither regeneration nor visible growth is a gate.
- Explanation: decision → work → time → response → reassessment; an opening is an opportunity, not a promise of regeneration.
- Visual: explicit repeating management loop.
- Dependencies: implemented S1-C history, existing S1-D repeated work and v20 review events. **NEW OBJECTIVE LOGIC**, no new S1-C/D system.

### 6 — Secure the next generation (`recruitment`)

- Main concept: recruitment and its closely coupled protection decision.
- Why here: assess openings and responses before choosing establishment sites.
- Action: map diagnosis then visit sites, inspect light/regeneration/browsing/vegetation; use nursery stock to introduce beech and sessile oak, review optional shelters and executor, approve and advance.
- Completion: both existing introduced-species goals achieved. Those already require successful commissioned planting plus present species; neither target nor predicate changes. Failed/dead establishment leads back here with the missing species named.
- Time: normal annual planting resolution. Previously completed introductions are recognised.
- Observe: actual work failures/successes, surviving juveniles, browsing/protection and vegetation; use naturally present regeneration if useful but do not wait for it as an unavoidable gate.
- Explanation: suitable light/site and protection matter; planting is player controlled, survival is not guaranteed. Shelters are an optional management choice, not mandatory CCF.
- Visual: site/light → juvenile → browsing and shelter; no implied survival guarantee.
- Dependencies: accepted map/ground inspection, nursery, exact planting/cohort legacy pathway, shelters/work execution. **NEW OBJECTIVE LOGIC**; planting/protection **COMBINED**. No new protection mechanic.

### 7 — Return to the young trees (`establishment`)

- Main concept: establishment versus recruitment; observe and adapt.
- Why here: a planted work order alone does not demonstrate continuing success.
- Action: read planting results, revisit sites; advance at least one additional year, read/acknowledge that year's results and compare the sites again.
- Completion: ecological year greater than latest successful planting year and saved AnnualReviewAcknowledged in a later year. Actual live introduced-species goals remain a prerequisite through lesson 6.
- Time: one extra year minimum; extra advances do not break progression. This is a teaching observation interval, not a universal forestry intervention interval. No multi-year jump or fixed thinning cycle is introduced.
- Observe: survival or failure, browse damage, vegetation and light; no required random regeneration, size or growth threshold.
- Explanation: establishment is not recruitment; failure also informs later management.
- Visual: planting year → later observation → surviving or failed young trees.
- Dependencies: annual advance, saved acknowledgement, existing planted juveniles. **NEW OBJECTIVE LOGIC**.

### 8 — Continue the transformation (`transformation`)

- Main concept: synthesis and ongoing adaptive management, not a magically finished forest.
- Why here: the basic management loop has been demonstrated; long-term forest success can still be incomplete.
- Action: use overall goals and history to identify remaining work, revisit sites and decide whether intervention or observation is justified.
- Completion: no additional tutorial requirement. This remains the current ongoing lesson until/after the unmodified scenario outcome becomes Completed; its action then points to the completion summary.
- Time: as needed toward the existing minimum year, current canopy/recruitment/habitat goals and review horizon. No universal interval.
- Observe: retained canopy, mixed age/species structure, recruitment, deadwood, finances and consequences across interventions.
- Explanation: continued assessment and adaptation, preserving tree cover.
- Visual: retained forest → younger generation → continued adaptation.
- Dependencies: all existing goals, outcome and completion/history UI. **PRESENTATION CHANGE**.

## Exact eight-goal mapping and protected rules

| Existing goal ID | Actual predicate (unchanged) | Teaching connection |
|---|---|---|
| minimum-year | snapshot year >= definition MinimumCompletionYear (25) | time in 1/4/6/7, ongoing 8 |
| retained-canopy | current living original-species count >= geometry-adjusted definition target | positive selection 2, residual stand 3/4, synthesis 8 |
| continuous-canopy | current mean canopy >= definition minimum (.35), not historical never-below | retain cover in 3/4, observe 5, ongoing 8 |
| regeneration | occupied regeneration cells >= geometry-adjusted target; includes accepted individual planted juveniles in otherwise empty cells | diagnosis/planting 6, observe 7, ongoing 8 |
| fallen-deadwood | current deadwood volume >= geometry-adjusted target | material decision 4, visit/review 5, maintain 8 |
| managed-opening | successful commissioned felling event or completed felling order | access 1 already contributes; selective release 3/4 teaches a further principle |
| introduced-beech | successful historical beech planting plus current species presence | 6/7/8 |
| introduced-sessile-oak | successful historical sessile oak planting plus current species presence | 6/7/8 |

The manager additionally requires the existing reviewed two-cycle chronology for scenario completion (separate from the eight displayed goals). Lesson 5 reads that exact rule. Current canopy/species/wood predicates remain current-state tests; the guide cannot silently make failed work count. Actual values scale through the existing definition/geometry; no teaching constant replaces them.

All lesson gates are pedagogical projections, never new scenario completion requirements. Finishing the teaching path does not complete the scenario. The existing minimum review year, failure rules, finances and century review are untouched.

## UI, visuals, persistence and boundaries

- Primary card: current title, WHY, immediate action, meaningful world progress and original supporting diagram. No opaque substep counter.
- Completed titles: collapsed secondary list, inferred from world prerequisites rather than permanent achievement claims.
- Future lessons: one collapsed short preview, not a numbered wall. Optional detailed topics retain all original stable practice IDs and device preferences. Overall scenario goals remain independently discoverable and collapsed initially.
- Visual production: original procedural introduction, retained approved rack diagram, plus future-tree, release, fallen-wood, repeated-cycle, recruitment/protection, later-observation and transformation diagrams. No imported or copyrighted imagery, licensing dependency, new package, scene or prefab. Existing rack-source distinction/provenance remains in Docs/Verification/ScenarioOneGuidedOnboardingRack/SourceReview.md; no engineering dimensions added.
- Annual Review exposes its existing acknowledgement action when the current lesson needs results, including after the protected two-cycle rule is already met. It records the same v20 event; earlier-year historical views cannot acknowledge current results.
- Existing preferences are device-wide optional practice, unsuitable as per-forest curriculum authority. The new guide does not use them to skip lessons. It reconstructs from existing saved world marks, orders, events, year and goals; save v20 unchanged. UI projection cache lasts at most .25 seconds and forced refresh invalidates it.
- Physical return/inspection is explicitly prompted, not independently persisted or made a button-count gate. Saved review acknowledgements are the durable observation/review gate. Acknowledgement cannot prove a player compared every visual consequence; human playtest remains necessary.
- Advanced/nonstandard saves reconstruct from meaningful current state and chronology. Completed later felling near a current living Crop Tree is evidence of a later intervention, not proof of the exact historical crop selection. Crop loss can require reassessment and a new selection. Early completed actions may already satisfy later lessons; the guide does not erase or redo valid forestry work. Missing/current failed requirements return the relevant guidance.
- No new S1-C/S1-D development needed: actual base has intervention comparison/history, stale-plan revalidation and repeatable work. Older context status describes an earlier implementation snapshot. No new forestry, economy, ecology, deterministic/reference, schema or outcome logic.
- Optional pruning (eligibility/lifts) and vegetation clearance (including risk to existing juveniles) remain available in the original help/practice library; neither is compulsory.

## Verification and remaining calibration

Tier 2 only: pure sequence/wording checks, real-world transition and save/load reconstruction checks, 720p/1080p visuals, native rack origin/aim/cancel/reselect smoke, teaching copy, MenuTutorial and unmodified completion verification. Raw receipts/captures and exact source HEAD are linked in the implementation handoff; completion of this file alone is not verification.

No product approval is required to implement this bounded V1 under the packet. Independent review/user playtest should judge copy length, the one-year post-planting observation interval, and whether the optional return-to-site instruction warrants a future durable visit system. That last feature would be separately scoped if it needs save changes; none is introduced here. No canonical Decision Log edit.
