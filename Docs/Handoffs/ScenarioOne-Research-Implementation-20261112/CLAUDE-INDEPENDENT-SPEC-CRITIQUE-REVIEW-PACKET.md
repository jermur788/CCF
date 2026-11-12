# CLAUDE INDEPENDENT REVIEW HANDOFF — Research-informed Scenario One management-learning V1 draft specification

**Prepared:** 2026-11-12 by CCF Overall Manager  
**Status:** READY / NOT RUNNING — **INDEPENDENT REVIEW ONLY**, not specification approval, production implementation, Unity testing or integration  
**GitHub task claim:** [#389 — independent four-part specification review](https://github.com/jermur788/CCF/issues/389)  
**Priority:** High for design quality, **non-blocking to existing complete-loop Scenario One demo development**  
**Difficulty:** **High** — must reconcile scientific evidence/transfer limits with four-part cross-system requirements, current implementation contracts, user-facing pedagogical goals, persistent history and proportionate testing.  
**Best-suited model/provider:** **Claude Opus 5.5, Medium thinking**, independent of the GPT-6 Manager-authored draft (or an equivalently capable Claude reviewer if that exact configuration is unavailable). No coding agent is needed for a docs-only review.  
**Plain-English purpose:** Critique the full proposed package of research-informed learning improvements before anything is authorised for implementation; identify incorrect science, ungrounded player claims, architectural hazards, missing product decisions, excessive scope and unnecessary verification work. Do not just endorse the author's recommendations.  
**Exact ownership lane:** GitHub remote documentation-only branch \`docs/claude-review-research-management-spec-20261112-v1\`, **created from the specification branch** so it contains the same four spec files. **No local worktree is created/owned**; no Unity machine lease. Write **only** \`Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md\` on the assigned branch.  
**Canonical report path/name (required):** \`Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md\`  
**User action:** Copy the dispatch line at the end into a Claude conversation with GitHub access. No other action required to prepare review.  
**Dashboard:** no sync/readback assumed.

## 1. Read these exact four files FIRST — this is the object of review

**Source branch:** \`docs/scenario-one-research-informed-spec-20261112-v1\` (the draft has **not** been merged to \`main\`).

1. **Feature specification \`spec.md\`** — https://github.com/jermur788/CCF/blob/docs/scenario-one-research-informed-spec-20261112-v1/specs/scenario-one-research-informed-management-learning-v1/spec.md
2. **Delivery plan \`plan.md\`** — https://github.com/jermur788/CCF/blob/docs/scenario-one-research-informed-spec-20261112-v1/specs/scenario-one-research-informed-management-learning-v1/plan.md
3. **Task decomposition \`tasks.md\`** — https://github.com/jermur788/CCF/blob/docs/scenario-one-research-informed-spec-20261112-v1/specs/scenario-one-research-informed-management-learning-v1/tasks.md
4. **Requirements and verification checklist** — https://github.com/jermur788/CCF/blob/docs/scenario-one-research-informed-spec-20261112-v1/specs/scenario-one-research-informed-management-learning-v1/checklists/requirements.md

You **must inspect all four**, compare cross-references and verify that proposed acceptance checks actually prove their requirements. Do not mistake proposed tasks for already dispatched/implemented work.

## 2. Compare with controlling canonical project authority

Read live \`main\` (not just the reviewer branch's snapshot) and record the exact date/source state without exposing opaque commits in user-facing prose:

- \`Docs/Project/decision-log.md\`: accepted D-042 frozen Reference Future; D-059 three-species parity; D-060 complete-loop first demo; D-064 Rack V2; D-065 Multi-Scale Forestry Management V1; D-066 historical latch; D-071 existing five-parallel access layout; D-072 map/headland teaching presentation.
- \`Docs/Project/current-milestone.md\`, \`Docs/Project/game-brief.md\`.
- **Live** \`Docs/Project/coordination/active-tasks.md\` before writing or declaring any source lane free (this file is *not* a static snapshot).
- Approved \`specs/multiscale-forestry-management-v1/spec.md\`, \`specs/scenario-one-review-history-v1/spec.md\`, \`specs/scenario-one-repeated-interventions-v1/spec.md\` and Rack V2 spec.
- Relevant implementation paths, **read-only**: \`Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs\`, \`ScenarioOneManager.cs\`, \`ScenarioOneCompletionRules.cs\`, \`ScenarioOneDefinition.cs\`, \`ForestTree.cs\`. Find/read the **current actual** \`ScenarioManagementEvent\`, save serialization/migration, S1-C history consumer and MFM code before asserting how a new event would behave. Never infer implementation from an approved spec or prior chat.
- Existing [six-paper independent-review Manager disposition](https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Handoffs/ScenarioOne-Objectives-Six-Paper-Review-20261111/MANAGER-INDEPENDENT-REVIEW-DISPOSITION-AND-DOCUMENTATION-FREEZE-20261111.md) and [post-demo optional-retention design opportunity](https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Handoffs/ScenarioOne-Objectives-Six-Paper-Review-20261111/POST-DEMO-COMPETITOR-RETENTION-RATIONALE-DESIGN-OPPORTUNITY.md).

**Important source distinction:** The Manager-authored implementation spec is DRAFT. The earlier Opus review accepted the **research intake/extraction with qualifications**, NOT all proposed game mechanics. The latest user asked for comprehensive implementation specifications and separate research handoffs — not for immediate production code that overrides pre-demo boundaries.

## 3. Research basis and specific evidence checks

Consult the exact original six-paper project-source PDFs where accessible, plus the existing four CCF composites and Irish marking evidence. The primary source/extraction register is:

https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Research/PommereningSixPapers/Originals-Hash-and-Evidence-Register-20261111.md

Also inspect:
- \`Docs/Project/research-index.md\` and \`Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md\`, including photographed Irish marking pages 84–87 and their *uncertain publication metadata*.
- \`CCF_CCF_Practice_Economics_and_Transformation_Evidence_Composite.md\`, \`CCF_Ecology_and_Regeneration_Evidence_Composite.md\` and \`CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md\`.
- Two already-published **browser-research handoffs** are **READY, not yet accepted evidence**:
  - [#387 — Irish marking/competitor retention/wind](https://github.com/jermur788/CCF/blob/docs/research-ccf-marking-wind-20261112-v1/Docs/Handoffs/Research-CCF-Marking-Wind-20261112/RESEARCH-HANDOFF-IRISH-UK-RETAINED-COMPETITOR-WIND.md).
  - [#388 — Irish young-tree/regeneration language](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Handoffs/Research-CCF-Young-Growth-20261112/RESEARCH-HANDOFF-YOUNG-TREES-REGENERATION-TERMINOLOGY.md).

**If original PDFs are not accessible in your Claude environment, explicitly list what you did not read and limit scientific-confidence claims accordingly.** Never invent page references, citations, numeric model results, current branch state or an Irish forestry standard.

The six papers overlap in authorship (all have Pommerening). They do not amount to six independent Irish validations. Relevant known boundaries:
- P1 (individual-based management) is the source for greater agreement on retained crop/frame trees than competitors; P2 (rating experiments) compares thinning approaches and tree-selection agreement. The initial attribution and agreement-scale objections in the earlier Opus review were resolved by primary-source spot checks.
- Default Scenario One runs **Storm Model 0**: neither crown release nor keeping dense neighbours has a simulated wind-damage consequence. Risk language must reflect two-sided real-world considerations and model omissions without implying one fixed answer.
- Existing Lesson 3 **30% Hegyi Share release** and **40% basal-area safeguard** are protected educational/gameplay calibration, not universal Irish empirical prescriptions.
- P3 neighbourhood structure metrics cannot become scientifically validated habitat/biodiversity success scores by assertion.
- No transfer of P4 Swiss Beech canopy parameters, P5 Finnish ingrowth or competition coefficients, or foreign gap/strip prescriptions into Irish gameplay from this packet.
- "Regenerating cells" is an internal model term the user rejected for player-facing language. The existing occupancy count still reflects artificial map sections, not hectares or an established individual-seedling inventory.

## 4. Review questions — make findings concrete, challenge every assumption

### A. Product scope / decision authority
1. Does the proposed draft **silently promote** research evidence into a mandatory new gameplay workflow or accepted mechanic? Identify specific FR/AC/task lines.
2. Is the separation **documentation now / unchanged first demo / post-demo implementation by explicit approval** consistent across spec, plan, tasks and checklist? Are there contradictions with previous user instructions?
3. Which features are small enough to implement via existing systems versus an optional new feature? Would a smaller V1 offer most of the educational value with much lower risk?
4. Does the draft correctly distinguish **informational explanation**, **recorded player intention**, **reviewable historical fact**, **objective score**, and **biological consequence**?

### B. Forestry/science and learning design
5. Is Share/Hegyi described precisely, including what it does *not* model? Is the claim that many acceptable tree sets can satisfy the lesson correctly bounded?
6. Could an optional reason to retain a competing neighbour **reward unverifiable intentions**, teach that narration cancels competition, or create misleading claims of shelter/habitat/wind value? What safeguards or alternative wording would you require?
7. Does "areas with young trees" correctly represent both abstract regeneration cohorts and exactly planted juveniles? Would "areas where young growth is recorded" be safer? Are any stage/survival claims in the spec unsupported?
8. Can the existing model actually support **local "shelter/edge/open" labels** without a new hidden spatial classifier? Suggest a copy-only fallback if not.
9. Does the wind-caveat design adequately disclose **storm effects absent in default model** without overclaiming stability benefits? Should it be release-note only rather than UI?
10. Is the document overdependent on one research school; which independent Irish, UK practice or contrary evidence is necessary before player-facing assertions? Identify exact gaps rather than ordering broad redundant research.

### C. Implementation architecture, persistence and safe compatibility
11. **Challenge FR-07's preferred append-only \`ScenarioManagementEvent\` approach.** Inspect current enum/serialization, saved IDs/versions, S1-C event filters, grouping/annual reports, S1-D two-cycle logic, MFM prescribed work and potential accidental misclassification. Does the proposal work unchanged? If not, say exactly what breaks or what evidence is missing.
12. Would **player intention** be better implemented as a thin UI note, another record linked to a work/management intervention, or no persisted feature? Compare effort and interoperability honestly; don't assume "reuse existing event" is automatically safest.
13. What happens when Crop Tree designations change, neighbours die or are felled, work orders carry over, reasons are revised/cleared, a save is loaded, and old references are absent? Does the checklist adequately cover identity, provenance, event ordering and old-save semantics?
14. Would optional reasons complicate MFM's **proposal-not-mark / player manual precedence / Work Plan approval** contract, or incorrectly promise that a "keep" note creates a protection/veto? Is the spec explicit enough to prevent this?
15. Identify where current UI ownership, competition panels and 1280×720 overlaps make the plan hazardous. Are there duplicate explanation systems or repeated tree-by-tree clicks that reduce playability?

### D. Tests, costs, handoffs and sequencing
16. Assess each required validation tier. Are the **43 acceptance checks plus other gates** proportionate, testable and non-duplicative? Which should be combined, deleted or made conditional? Do not require Tier 3 for a copy-only or offline feature.
17. Are Wave A and Wave B separated appropriately or does Wave B introduce a hidden dependency that blocks simple copy? Is it feasible to keep existing objective IDs and internal names while changing wording without changing saved history?
18. Are the browser handoffs #387/#388 asking only genuinely unresolved scientific questions? Which facts can be verified directly from the six papers or current repository and don't warrant more expense?
19. Are proposed model/provider assignments, writer file boundaries and local Unity-serialization gates realistic? Do **not** claim any agent or Unity machine lock is active based only on this packet.
20. Rank by risk-adjusted value: changes to **implement**, **tighten before approval**, **prototype only**, **defer**, **reject**. Provide the smallest safe executable follow-on sequence. Include a case for **doing nothing beyond truthful copy** if justified.

## 5. Required review output

Publish one Markdown document on the exact assigned branch and path.

### Mandatory report structure
1. **Executive verdict** — choose exactly **APPROVED / NEEDS CHANGES / REJECTED** for the four-part DRAFT, with scope of that verdict. **APPROVED is not implementation acceptance.**
2. **Source-access and method register** — list what you actually read (each spec file, controlling docs, source files, original papers), what was unavailable, and uncertainty level for each class of claim.
3. **Ranked objections** — severity **BLOCKING / MAJOR / MINOR / NOTE**, exact file+section/line or quote, concrete risk, evidence, and a proposed fix or explicit alternative. Do not invent defects.
4. **Requirement-level disposition table** — FR-01–FR-09 (and any split/merge proposed), each one of **KEEP / REVISE / SPLIT / DEFER / REMOVE**, plus rationales and dependencies.
5. **Data/architecture audit** — direct evidence for or against the \`ScenarioManagementEvent\` retention-intention design; do not merely repeat the spec's assumptions.
6. **Forestry/evidence crosswalk** — source-backed versus transferred versus gameplay calibration, with bounded source references; explicit gaps awaiting #387/#388 if those reports are not returned.
7. **Minimal revised implementation sequence** — recommend one economical set of source/UX tasks, separate optional historical-persistence work, and sensible test-tier gates. Explain the cost of extra tests/lanes.
8. **Product decisions for Manager/user**, limited to real choices. Give preferred options and consequences, avoid redispatching unresolved questions already answered in the spec.
9. **Orchestration evidence summary** — provider/model/effort if observable, actual tools, time/cost only if observable, exact GitHub branch/report path, readback and whether any edits exceeded this report. Use UNKNOWN rather than invented metrics.
10. **Hand-off status** — exact direct GitHub review URL, claim #389 marked [HANDED-OFF] and closed only after report publication/readback.

**Standard of reasoning:** Be independent, not adversarial for its own sake. Prefer evidence-backed objections and the least costly correction. A clean PASS is acceptable only if the draft withstands the requested checks. If a critical source is unavailable, qualify scope rather than giving unwarranted approval.

## 6. Ownership, safe workflow and restrictions

1. Read the full packet, four-part spec, controlling main docs and **live** \`Docs/Project/coordination/active-tasks.md\` at task start, including before declaring no writer conflict.
2. Read/update [#389](https://github.com/jermur788/CCF/issues/389): mark **[RUNNING]** with *actual* Claude model/provider, branch \`docs/claude-review-research-management-spec-20261112-v1\`, **remote GitHub docs-only / no local worktree**, and observed spec baseline. If unable to start, mark **[BLOCKED]**.
3. Do not edit any of the four spec files, canonical Research Index, Decision Log, Current Milestone, coordination file, source code, Unity content or files owned by existing browser research #387/#388. Only create/update **your independent review Markdown** at \`Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md\` in the assigned review branch.
4. If this Claude session cannot connect to GitHub, return the report body and explicitly flag **PUBLICATION BLOCKED**; do not claim the handoff is durable or complete. No local scratchpad path is an acceptable substitute for GitHub publication.
5. Once published, **fetch/read back** the full report; update #389 [HANDED-OFF], link it and close. Overall Manager will inspect and issue final APPROVED / NEEDS CHANGES / REJECTED disposition. The reviewer cannot self-authorise implementation or merge.
6. **No local Unity, C# production work, builds, tests, backups, Git reset/stash/clean/prune, or actions against user playtest worktrees**. Inspection may use GitHub read tools, never conflict with owners.
7. **Demo boundary:** no new pre-demo lesson grading, objective, biological, wind, save/anchor, Rack V2, MFM or frozen Reference Future changes. Windows build remains prohibited until personal playtest + explicit user permission.

## 7. Exact dispatch message (copy/paste)

> **Claude Opus 5.5 Medium — independent critical specification review only.** Review the entire four-part *Scenario One Research-Informed Management Learning V1* DRAFT against current CCF decisions/specs, actual relevant repository code and the six-paper evidence, using this binding GitHub handoff: https://github.com/jermur788/CCF/blob/docs/claude-review-research-management-spec-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-SPEC-CRITIQUE-REVIEW-PACKET.md . Primary spec: https://github.com/jermur788/CCF/blob/docs/scenario-one-research-informed-spec-20261112-v1/specs/scenario-one-research-informed-management-learning-v1/spec.md . Use issue https://github.com/jermur788/CCF/issues/389, only the named remote reviewer branch, and publish the severity-ranked, evidence-cited verdict at the specified Markdown report path. Read back the published report, hand off and close claim. Do not edit the spec, code, Unity, canonical decisions or current demo scope.

**User action now:** paste this dispatch into a Claude session that can access the GitHub repository. The reviewer is NOT automatically running.
