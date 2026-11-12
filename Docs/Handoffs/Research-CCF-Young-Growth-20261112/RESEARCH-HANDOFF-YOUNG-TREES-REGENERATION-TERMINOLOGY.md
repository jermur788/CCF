# Browser research handoff — young woodland growth, regeneration terminology and honest explanatory copy

**Status:** READY / NOT RUNNING — focused public-source research, not implementation  
**Claim:** [#388](https://github.com/jermur788/CCF/issues/388)  
**Manager:** Overall Manager / #200  
**Exact handoff name/path:** \`Docs/Handoffs/Research-CCF-Young-Growth-20261112/RESEARCH-HANDOFF-YOUNG-TREES-REGENERATION-TERMINOLOGY.md\`  
**Purpose:** Establish a scientifically and practically accurate player vocabulary for **where young trees are appearing and developing** in an Irish woodland, clarifying what can honestly be inferred from the game's spatial occupancy model, planting records, light, browsing and vegetation competition.  
**Difficulty:** **Medium–High**, because the accepted simulation distinguishes actual individually planted trees, abstract natural cohorts and occupied map regions; public forestry terminology must not erase those differences.  
**Best suited:** ChatGPT Deep Research / browser-connected high-reasoning model. **No local T3/Unity work.**  
**Exact writing lane:** GitHub docs-only branch \`docs/research-ccf-young-growth-language-20261112-v1\`, **no local worktree**, sole author of the named report.  
**Report output:** \`Docs/Research/ResearchIntake20261112/YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md\`; optional short reference appendix in same folder only.  
**End status:** verified report handoff, #388 [HANDED-OFF] then closed.  
**User action:** paste the dispatch line below into a separate browser research chat. No gameplay edits authorised.

## Current confirmed project contract

The user rejected the phrase **"regenerating cells"** as player-facing forestry language. It is an internal model/data term, not a physical site unit. Manager proposes **"Encourage woodland regeneration"** for objective purpose and **"Areas with young trees"** for a progress display — both are **working wording**, not a redefinition of the target.

Inspect live source and avoid relying only on the friendly phrase:

- \`Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs\` uses the \`regeneration\` stable objective ID; its display currently says "Maintain regenerating cells" and uses \`snapshot.occupiedRegenerationCells\`.
- \`Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs\`, in \`RecordEcologicalSnapshot\`, counts map areas with regeneration cohort occupancy and certain distinct locations with living exactly planted juveniles. **The counted unit is a simulation grid element**, not measured hectares, seedling stems/ha or a successful future generation.
- Enlarged80 grid: 256 cells of 5 m × 5 m; completion currently requires 12 occupied units. **No change** to the count, area-calibration policy or persisted IDs/fields is authorised.
- Natural regeneration has representation thresholds/cohorts; planted exact juveniles have distinct provenance. Presence is not a guarantee of survival, and a current snapshot is not proof of progression across time.
- Accepted S1-C history L1–L4 separates recorded fact/current observation/derived comparison/explanation. Do not invent earlier-year stage or browse records where they are session-only.
- The existing first external demo has a complete repeated-management loop, three species, Rack V2/MFM; no new regeneration system or threshold needed.

This research is **documentation only**, and the original research extraction is supporting methods material, not proof of Irish establishment percentages.

## Existing project evidence to consult first

- \`Docs/Project/research-index.md\`; \`Docs/Project/decision-log.md\` (D-047, D-049, D-057 and related decisions).
- \`CCF_Ecology_and_Regeneration_Evidence_Composite.md\` (growth stages, light, browsing/competition, Irish oak regeneration evidence); \`CCF_Browsing_Protection_and_Management_Evidence_Composite.md\`.
- \`Irish Atlantic Woodland Ecology for a Continuous-Cover Forestry Game Simulation.pdf\`.
- The six-paper originals/extraction, especially P6 Pommerening, Szmyt & Duchiron (2024), *Revisiting silvicultural systems* (DOI 10.1016/j.tfp.2024.100597), P6's **shelter / forest margin / open** conceptual categories. Do not adopt its Polish gap sizes or treat a harvesting rack as a regeneration strip.
- [Manager vocabulary proposal](https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Handoffs/ScenarioOne-Objectives-Six-Paper-Review-20261111/MANAGER-PLAYER-FACING-REGENERATION-TERMINOLOGY-ADDENDUM.md).

## Browser task

Use authoritative Irish forestry/woodland sources first: current Department/Forest Service, Teagasc, Coillte, NPWS and Irish peer-reviewed regeneration studies, with UK/European science as **labelled transfer** only. Do not assume a web site is a current Irish regulatory authority. Retrieve original pages/publications, not only summaries or snippets. Cross-check scope: Sitka plantation transformation, Oak and Beech introductions, native woodland claims, planted versus natural young growth.

Answer:

1. What terms do practical Irish forest managers/owners use for **natural regeneration, advance regeneration, planted stock, seedlings, saplings, young growth, recruitment/ingrowth and successful establishment**? Which terms have stage/survival implications?
2. Is **"Areas with young trees"** defensible for the game's count when some included records are abstract modeled cohorts rather than individual sampled young trees? Compare **"Areas where young growth is recorded"**, **"Parts of woodland with young trees"**, **"Woodland regeneration present"** and more precise alternatives. Give a recommended shortest label, optional explanatory sentence and a less-obtrusive fallback.
3. Should the player see **natural and planted presence** separated where recorded? Which labels must not imply equal countable individuals or equivalent origin?
4. Which relationships can be accurately described from authoritative current light, browsing/vegetation competition and young-tree stage without claiming future survival? Do **not** invent direct causal claims or Irish coefficients.
5. In a 0.64 ha, mixed-crop / transitioning Sitka forest, how should **beneath shelter**, **near a gap edge** and **more open area** be framed as qualitative conditions? Which differences are supported and which require local measurements/model checks? Acknowledge that environmental gradients and gap geometry are continuous rather than universal bands.
6. Which wording should describe **Annual Review** changes: raw count, newly recorded locations, older cohorts and protection status? Distinguish directly recorded yearly facts from derivations that require checking year-to-year presence.
7. Critique whether "establish planted Oak" correctly describes the game's existing predicate (successful planting event + species presence), without proposing a new completion rule in this task.
8. Are any official Irish terminology or guidance claims in the existing research composites stale, insufficiently sourced, or context-specific? Return bounded corrections, with evidence.

## Required deliverables

- **Decision summary (≤300 words)**, stating the recommended player-facing terminology and its limits.
- A **source-provenance table**: exact statement / source link / publication date / page or paragraph / geography / tree species / source class / certainty / CCF relevance.
- A **vocabulary comparison** table with recommended/allowed/avoid and why.
- Three short copy sets ready for a later **design-only** approval:
  - Objective/progress (headline, metric label, one-sentence explanation).
  - Annual Review (one grounded current-year statement + an optional comparison only when recorded).
  - Inspect/Help (natural vs planted origin and shelter/edge/open guidance).
- A **cannot claim** list: no seedling density, no hectare coverage, no proven forest renewal, no automatically beneficial planting, no survival probability without modelled support.
- A list of **source-level facts the later implementation worker must inspect** before finalising copy, including actual screen references and persisted history fields.
- An explicit verdict **COPY READY / NEEDS GAME DATA / NEEDS PRODUCT DECISION** for each copy option.
- GitHub report URL plus remote readback confirmation.

No novel biological coefficients, no new objective thresholds, no new authoring/gameplay of gaps or strips, no change to placement/Work Plan, no eligibility or save changes.

## Coordination and safety

- Read \`Docs/Project/coordination/active-tasks.md\` and #388 live before writing; branch is remote/docs-only, not a local ownership lock.
- At start, change claim [RUNNING] with model/provider, branch, remote-only lane and observed starting baseline. Mark [BLOCKED] if cannot start.
- Own **only** \`Docs/Research/ResearchIntake20261112/YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md\` and appendix files on your branch. Do not touch the tree-marking/wind research branch or production specs and decisions.
- Publish, read back, link returned report in #388, mark [HANDED-OFF] and close.
- Do not assume Drive mirroring, dashboard integration, local Editor verification or expert endorsement.
- If no authenticated GitHub access, return report text but flag **publication blocked**; do not pretend it was durably handed off.

## Copy/paste dispatch line

> Research **Irish forestry language for young trees and natural versus planted woodland regeneration**, and validate wording that accurately describes CCF's existing presence metric, strictly following https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Handoffs/Research-CCF-Young-Growth-20261112/RESEARCH-HANDOFF-YOUNG-TREES-REGENERATION-TERMINOLOGY.md . Use https://github.com/jermur788/CCF/issues/388 and only the stated remote docs branch; publish an evidence-based Markdown report with source links and page references, verify readback and return its URL. No Unity or gameplay edits.

**User action to dispatch:** paste this line into a separate browser-research conversation. No other action required.
