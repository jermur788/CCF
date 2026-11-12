# SCENARIO ONE — RESEARCH-INFORMED MANAGEMENT LEARNING V1

**Spec status:** **DRAFT — proposed product scope and implementation contract; NOT APPROVED FOR PRODUCTION**  
**Spec ID:** \`SCENARIO-ONE-RESEARCH-INFORMED-MANAGEMENT-LEARNING-V1\`  
**Author / owner:** Overall Manager  
**Created:** 2026-11-12  
**Product request:** Produce a concrete implementation specification for all worthwhile Scenario One improvements that can be defined from the six papers, the independent review, the existing model and accepted forestry research. Publish separate browser research handoffs for matters requiring more source corroboration.  
**Milestone:** **Post-current-demo improvement package** by default. **Specifications/research can be prepared now**; no change to accepted pre-demo production scope unless user explicitly revises that boundary.  
**Status authority:** Product owner still must approve this four-part spec and any changes to protected design/retention persistence.  
**Companion files:** [plan.md](plan.md), [tasks.md](tasks.md), [checklists/requirements.md](checklists/requirements.md).  
**Research handoffs:** [Irish/UK marking/wind](https://github.com/jermur788/CCF/blob/docs/research-ccf-marking-wind-20261112-v1/Docs/Handoffs/Research-CCF-Marking-Wind-20261112/RESEARCH-HANDOFF-IRISH-UK-RETAINED-COMPETITOR-WIND.md) and [Irish young-tree/woodland language](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Handoffs/Research-CCF-Young-Growth-20261112/RESEARCH-HANDOFF-YOUNG-TREES-REGENERATION-TERMINOLOGY.md). Claims #387 and #388 are **READY/NOT RUNNING** until user dispatches.  
**Existing proposal:** [Six-paper source register](https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Research/PommereningSixPapers/Originals-Hash-and-Evidence-Register-20261111.md); [independent review ruling](https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Handoffs/ScenarioOne-Objectives-Six-Paper-Review-20261111/MANAGER-INDEPENDENT-REVIEW-DISPOSITION-AND-DOCUMENTATION-FREEZE-20261111.md).

## 1. Product purpose

Help players understand **why a forestry decision might be justified**, what the authoritative game actually measures, why alternative defensible choices exist, and how earlier decisions relate to later recorded outcomes. Preserve the established **decide → advance → walk → observe → review → decide again** loop.

The specific educational improvements are:
1. Teach Crop Tree selection separately from competitor removal.
2. State that \`CompetitionShare\` is a local size/distance-based indicator, **not a complete verdict on the value of a neighbour** or a unique answer key.
3. Allow an **optional recorded player intention** when deliberately keeping a competing neighbour, without making a non-removal biologically equivalent to removal.
4. Reuse recorded work/current state in Annual Review and later cycles to explain the player's earlier intentions and observed changes honestly.
5. Eliminate internal spatial-grid jargon from player-facing regeneration language; distinguish natural young growth from planting where recorded.
6. Explain broad light/shelter/edge/open regeneration situations conservatively using existing observable data; avoid inventing new microclimate or habitat effects.
7. Communicate the **default lack of simulated wind damage**, with no numeric wind-risk invention.

These are not eight new lessons, an expert marking grader or a new simulation model.

## 2. Source/authority and independent review

**Accepted product authority:** latest user instructions; \`Docs/Project/decision-log.md\` (D-003/004/010/042/047/049/057/059–066/071/072); \`Docs/Project/current-milestone.md\`; \`Docs/Project/game-brief.md\`.

**Accepted task-level authority:** \`specs/multiscale-forestry-management-v1/spec.md\`, \`specs/scenario-one-review-history-v1/spec.md\`, \`specs/scenario-one-repeated-interventions-v1/spec.md\`, Rack V2 spec.

**Implementation source inspected for planning:** \`Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs\`, \`ScenarioOneManager.cs\`, \`ScenarioOneCompletionRules.cs\`, \`ScenarioOneDefinition.cs\`, \`ForestTree.cs\`. Exact latest integration branch, UI owners, newest save schema and current Unity state must be **reinspected before local implementation**; the Game Dev documentation baseline is not enough to assert they pass.

**Research sources (supporting, not accepted parameters):**
- P1 individual-based management / Crop Tree and competitor-selection evidence (Pommerening, Maleki & Haufe 2021).
- P2 rating experiments / low versus crown thinning and human marking disagreement (2018).
- P3 local structural indices (2002): research-only descriptive tools.
- P4 CanopyShotNoise crown model (2021): no parameter import.
- P5 Bayesian forest dynamics (2024): no Finnish coefficients or predicted probabilities.
- P6 regeneration-method taxonomy (Pommerening, Szmyt & Duchiron 2024): shelter/edge/open concepts, not Irish numerical gap rules.
- Existing Irish photographed marking pages, practice/transformation/wind and ecology/regeneration composites, Irish Sitka growth evidence, UK cab-visibility limitations.

**Independence:** all six papers share Pommerening; they are **one overlapping research network**, not six independent validations. The independent reviewer accepted the methods/extraction with limitations and withdrew its two initial P1/P2 attribution/category objections after primary spot checks. No disagreement is to be invented for narrative value.

## 3. Existing system contract — protected

These cannot change under this V1 without a separate explicit product decision/spec amendment:

- **Scenario One completion:** Year 25 minimum, numerical objective predicates/targets, Oak/Beech presence conditions and two reviewed management cycles; Year 100 review; no new pass requirements.
- **Lesson 3:** at least **30% planned Share reduction per selected Crop Tree**, **40% basal-area safeguarding**; teaching sample of three Crop Trees, 8 m teaching spacing and separate 8 m Hegyi neighbourhood.
- **Multi-Scale Forestry Management V1 (MFM):** proposals are distinct from Fell marks, player confirms Crop Trees and reviews wider proposals, manual state takes precedence, Work Plan approves committed work, comparison/coverage remain explainable.
- **Rack V2 / D-071/072:** five-parallel-rack design and true map/headland/entrance cues, old-save compatibility.
- **S1-C history:** recorded fact (L1), observed present (L2), derived comparison (L3) and explanation (L4) remain distinct; no invented past, no second history authority.
- **S1-D:** repeated player-directed management remains possible and advisory, no mandatory new cycle or fixed intervention interval.
- **Ecology, economy, growth, origin, browsing, understorey and storm profiles:** no changed coefficients, default Storm Model 0, no forest-quality or biodiversity score.
- **Persistence/compatibility:** no changes to frozen Reference Future v1, no rewrite of historic events or anchors, no silent alteration of Legacy40 or pre-feature save meaning.

A new **retention-intention event** is a *proposed* additive post-demo history feature. If this spec is explicitly approved, it may require the next available save schema with an old-save-safe migration. This does **not** retroactively authorise a pre-demo schema edit.

## 4. Functional scope by workstream

### FR-01 — Explicitly explain what Share measures

Existing Crop Tree/competitor inspection and relevant Lesson 3 screens MUST explain:
- \`Share\` is a model-derived local tree-competition indicator, based on neighbour size/distance and the current authorised model;
- removing one neighbour can reduce the computed burden, but a high-ranking neighbour is **not inherently the only right one to remove**;
- the current 30% teaching target is a bounded exercise in **crown release**, not a universal management requirement;
- a kept neighbour still contributes competition where the current model says so.

Use the existing authoritative \`CompetitionShare\` result; **no duplicate algorithm** and no model parameter changes.

**Not permitted:** assigning hidden stem quality, habitat value, windfirmness, shade-protection probability or expert correctness from Share; using agreement-statistic scores or thinning-from-below preferences as a lesson replacement.

### FR-02 — Preserve diverse marking decisions and meaningful feedback

The player MUST continue to choose removal marks manually within existing rules. Guidance MUST NOT claim a specific named neighbour is the unique valid choice. Two alternative sets of removals meeting the same accepted safety/Share rules MUST produce consistent rule credit and may have different explainable consequences. The UI MUST separate:
- chosen Crop Tree (player judgement);
- considered competitor (model-derived Share);
- manual Fell mark (player intention to remove);
- MFM proposal (not a Fell mark);
- approved Work Plan work;
- executed/observed result.

The feature MUST NOT allow an optional reason to evade existing lesson credit or safeguards.

### FR-03 — Natural player-facing vocabulary for regeneration

Player-facing objective: **"Encourage woodland regeneration"**, progress working label **"Areas with young trees: [current]/[target]"**, with one accessible explanation such as **"Young growth is recorded in these parts of the woodland; some began naturally and some were planted. Presence does not guarantee survival."**

**The counted unit remains the existing internal spatial cell**. The UI MAY describe a counted part/area/location but MUST NOT imply measured contiguous land area, stems/ha, individual established young trees, native forest restoration or future recruitment success. The count continues to include tracked natural cohorts and qualifying exact planted juveniles according to **current authoritative code**, which must be inspected before final copy.

Natural vs planted origin and named species can be shown only where actually stored and read correctly. Maintain stable objective ID \`regeneration\`, persisted field names and all existing target values. Copy must work with Legacy40 and Enlarged80, including existing Year-100 aspirational presentation and old saves.

**Copy nuance:** If browser research or code inspection shows "young trees" overstates the abstract cohort representation, prefer **"Areas where young growth is recorded"** while retaining natural language. Browser handoff #388 refines wording, not metric authority.

### FR-04 — Grounded regeneration explanation: shelter, margin and more open conditions

Teach the three **conceptual environments** in context:
- beneath retained mature canopy / sheltered by canopy;
- beside a canopy opening (edge/margin influence);
- more open ground.

When current UI shows local light, understorey competition, browsing/protection or juvenile stage, it MAY use those **recorded current states** to explain why young growth might differ. It MUST distinguish **model-observed** states from general forestry considerations. An edge/open classification requires **existing source-supported spatial geometry or a separately qualified read-only derivation**; do not show a false numeric class based only on arbitrary thresholds.

No new prescriptions for gap length, opening sizes, tree density, planting success, exposure or regeneration methods. **Extraction racks are not regeneration strips**. No new seasonal/Irish probabilistic model.

### FR-05 — Accurate wind/stability limitation

The **minimum unambiguous fact** to expose through appropriate Help/limitations text (after approved copy gate) is: **"The default Scenario One simulation does not model wind damage."**

Optional context MAY explain that real tree stability can depend on both retained stand density and exposure after selective removal, **if source-checked** in browser handoff #387. No one-direction rule that retaining or removing a large neighbour is always stable/unstable; no unimplemented wind-risk indicator, protective benefit or activation of dormant StormModel 1.

The initial docs/external-test limitation note MAY be prepared **now** without production changes. In-game placement after the demo follows bounded UI approval.

### FR-06 — Optional retained-competitor intention (new feature, **post-demo**, not a lesson workaround)

A player inspecting a live neighbour associated with a confirmed player Crop Tree MAY choose **"Retain — why?"**, without any added physical paint colour or persistent protected-tree mark. The action is optional and does not become a mandatory step.

**Reason vocabulary (proposed for spec approval):**
- \`canopy-shelter\`: "Keep canopy cover / potential shelter".
- \`later-timber\`: "Keep for future timber or later management".
- \`potential-habitat\`: "Retain for possible habitat or species value" (do not assert observed habitat or wildlife presence).
- \`reassess\`: "Reassess at a later intervention".

The player records a **management intention**, not a validated model outcome. User selection cannot alter ecology, Share, marks, MFM proposal state, approval, costs or grading. A record has no power to prevent a later Fell mark, prescription proposal or resolved fell. If the player later changes their mind, the history must preserve the original decision as historical fact.

**Starting policy:** begin with a very small UX mock-up comparing no prompt vs optional prompt. If product owner approves *durable later-year review* as the actual goal, implement FR-07; a session-only prototype alone cannot satisfy FR-07. This spec makes the durable design explicit so it can be reviewed now; implementation remains gated.

### FR-07 — One append-only history authority for retention intentions

**Preferred contract for formal spec approval (pending repo-source verification):** represent a recorded retention intention as a new typed **\`ScenarioManagementEvent\`**, reusing the existing monotone \`eventId\` and existing append-only S1-C event timeline, **not** a new separate authoritative decision store.

Minimal required persisted semantics, using existing fields where compatible and adding explicit named fields when absent:
- stable existing event ID;
- recorded scenario year and explicitly recorded player-authored **intention** type;
- stable **selected Crop Tree ID** and **kept neighbour ID** pair (do not invent tree identities);
- closed-vocabulary reason code and version;
- action **record**, **revise** or **clear**; later actions append new events, never edit/remove old ones;
- enough provenance to distinguish authoring from model prediction and point to an authentic 3D tree pair.

An **active intention** is derived read-only from the latest valid event for the pair. Exact same-state repeat clicks should be idempotent. Clearing appends a clear event. If either tree later dies, is felled or loses Crop Tree designation, retain historical intent but do not present it as a current protection/active requirement. Old saves with no new fields/events mean **no recorded intention**; never fabricate records from current tree marks or absent event years.

The authoring/reader implementation MUST:
- verify the latest current \`ScenarioManagementEvent\` enum and serializer, reserved numeric IDs, next schema version, S1-C FR-8/FR-10/FR-12 and S1-D chronology; STOP for spec amendment if additive event entries would disturb other existing consumers;
- keep this event out of \`WorkResolved\` and plan-count/financial work semantics;
- preserve event ordering, valid nonduplicated IDs, save-load roundtrip and history append-only semantics;
- distinguish L1 **"Player recorded intent to keep X for Y"** from L2 live condition and any L3 derived competition comparison; L4 teaching never invents a causal past;
- not infer exact original decision dates for old saves; show **"not recorded"**.

The persistence design is **specified as a preferred plan, not claimed compatible with the newest uninspected source**. If direct source inspection reveals an incompatibility, stop and revise the spec before code changes.

### FR-08 — Annual Review and repeated management

Reuse existing S1-C **aggregate-first with drill-down** and S1-D history/advisory surfaces. When a new persisted retention intention exists, the player MUST be able to locate:
1. **past decision** (who/what/when, reason explicitly player-authored), tied to tree IDs;
2. **present status** (tree still standing or no longer live; where current state supports it);
3. **measured current Share** and other available model outputs as L2;
4. **derived change** only if original and current values are truly recorded or reconstructable without invented history;
5. short next-step consideration as L4, not a compulsory new cycle.

For existing historical interventions without intentions, say **"No retention reason was recorded"** rather than interpreting absence of Fell marks as explicit retention. Wording must be concise and legible on existing 1280×720 and 1920×1080 screens. No new map-history product, export system, graphs or player proficiency grade.

### FR-09 — Read-only research computations, separate from player feature

Offline, non-production research may compute:
- local species mingling, e.g. number of different-species neighbours / 3;
- a size-difference function such as \`1 − min(DBH_i,DBH_j)/max(DBH_i,DBH_j)\`;
- a **comparison** of current Hegyi with the Finnish P5 relascope-based area alternative, **without** importing P5 coefficients.

Tests must handle property edge, sample denominator, tie order, live/dead distinction, changes in spatial footprint, younger abstract cohorts versus exact stems and matched saved years. Only if the same-fixture output is meaningful and comprehensible may Manager propose a **separate** descriptive panel. **Nothing in FR-09 authorises a new scenario score, objective, or canopy/competition engine.**

## 5. Non-goals / explicit exclusions

No new timber/felling commands, tree paint/mark colour, automated marker, operator freedom without player review, precise tree-quality classifier, biodiversity score, marteloscope exercise, future-year prediction engine, Finnish ingrowth, Swiss crown parameters, 95% Bayesian interval, Irish calibration based solely on P1–P6, new real estate or early Windows build. Do not change the default storm model.

**No universal judgement rule:** Retaining a neighbour can be a defensible decision, but the game MUST NOT claim it is always better than release. Likewise reaching 30% Share is **lesson credit**, not a professional silvicultural certification.

## 6. UX / interaction contract

- The player walks/inspects the existing world; the new interaction belongs inside the current inspection/lesson UI, not another permanent key or third management mode.
- All optional reason choices must be keyboard/controller accessible at the level supported by existing screens, and avoid focus conflict with map, Work Plan or modal help.
- Show the target Crop Tree and neighbour identity clearly before authoring any record. A tree inspection alone is not enough to infer that relationship.
- If another writer/feature changes Crop Tree designation/marks, refresh current display; player manual authority and Work Plan remain superior.
- Avoid exaggerated/misleading statements ("correct removal", "shelter proven", "wind-safe", "successful natural establishment", "biodiversity improved").
- Term use: player copy says **young trees, young growth, woodland regeneration**; internal \`cell\` vocabulary is developer only.
- No paid independent reviewer for trivial copy-only diffs. Full multi-system review **is** warranted for new event persistence.

## 7. Acceptance gates

- **Gate G0 — scope approval:** Product owner approves post-demo implementation package and explicitly permits or declines the retention-intent persistence extension. Approval of this DRAFT alone does not override an existing demo freeze unless stated.
- **Gate G1 — research:** Browser reports #387/#388 returned and Manager-reviewed **for claims that actually depend on them**. Safe copy and known code-source claims may be specified without waiting for every research question. Missing research cannot justify new coefficients.
- **Gate G2 — implementation readiness:** inspect latest repo and live locks, pick one writer per affected subsystem, identify clean branch/worktree and next available save version, preserve existing UI and MFM owned lanes. One task claim per substantive work packet.
- **Gate G3 — feature correctness:** targeted copy/content visual checks; optional intent life cycle and source provenance if approved; no change in sample lesson credit, MFM order lifecycle or annual work.
- **Gate G4 — integration:** only after current complete-loop demo programme and the approved feature candidate, proportionate Tier-2, with Tier-3 reserved for full milestone/release qualification.
- **Gate G5 — acceptance:** independent review if persistence/cross-system; Manager verdict APPROVED/NEEDS CHANGES/REJECTED; player acceptance separately. No Windows build before personal playtest and explicit permission.

## 8. Open decisions (do not silently assume approval)

1. Does the product owner explicitly approve the **persistent** retention-intention feature, or only the optional design prototype? **Manager recommends persistence if later review is the real educational benefit.**
2. Should this package become a **post-demo V1** as recommended, or does the user want a **specific limited, source-only copy subset** advanced into the current demo? This is a scope decision, not implied by request to write the spec.
3. Is "Areas with young trees" final, or would source/mode semantics support "Areas where young growth is recorded" better? Browser report #388 + source copy review.
4. Should any wind wording go into the game after #387, or remain an explicit test limitation outside the game? **Known base fact (storm 0) may be stated; broader risk descriptions need source support.**
5. Shall a descriptive species-structure panel ever be pursued? **Recommend no until a meaningful same-fixture signal is demonstrated offline.**

**Implementation state:** Zero source/Unity changes are authorised or claimed here. This document is a complete draft scope for product review, not a passing implementation or formal acceptance.
