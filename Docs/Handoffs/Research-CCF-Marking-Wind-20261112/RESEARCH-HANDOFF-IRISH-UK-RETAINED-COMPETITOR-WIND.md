# Browser research handoff — Irish tree selection, competitor retention, and two-way wind/stability trade-offs

**Status:** READY / NOT RUNNING — targeted evidence research, not implementation  
**Research claim:** [#387](https://github.com/jermur788/CCF/issues/387) — update to [RUNNING] only when personally starting  
**Manager:** Overall Manager / #200  
**Exact handoff name/path:** \`Docs/Handoffs/Research-CCF-Marking-Wind-20261112/RESEARCH-HANDOFF-IRISH-UK-RETAINED-COMPETITOR-WIND.md\`  
**Purpose in plain English:** Establish how to explain competing tree-retention and selective crown-release choices in an Irish small-owner CCF simulation without pretending that the game's Hegyi Share is an expert's complete forestry judgement, and identify accurate language about the default absence of simulated wind damage.  
**Difficulty:** **High** — Irish versus British practical guidance can differ from published theory; the need is source-context interpretation, not more generic CCF summaries.  
**Best suited model/provider:** **ChatGPT Deep Research with live browsing**, or a browser-capable high-reasoning research agent. Do **not** use local T3 Code/Unity/Claude Ultra Code for this web-only research.  
**Lane:** GitHub **remote documentation only**, branch \`docs/research-ccf-marking-wind-20261112-v1\`. **No local worktree** (not a Unity or source lane). **Single primary writer** owns only the output files below.  
**Starting baseline:** use the branch's existing main-derived content; verify actual current source baseline and \`Docs/Project/coordination/active-tasks.md\` live before first write. Do not assume any local machine/Unity lock.  
**Report output:** \`Docs/Research/ResearchIntake20261112/IRISH-UK-TREE-RETENTION-RELEASE-WIND-EVIDENCE.md\`. A small separate reference table or links appendix is allowed inside the same directory.  
**Claim end state:** [HANDED-OFF], report link, close #387. No auto-merge; Manager reviews. If GitHub app unavailable, return complete Markdown with all exact citations and **mark the claim [BLOCKED] rather than claim a durable GitHub handoff**.

## Research question (bounded)

For an Irish continuous-cover transformation of a dense ~20-year Sitka plantation toward a mixed Sitka/Oak/Beech woodland, which **observable, defensible reasons** can a player have to **keep a neighbour that still contributes to competition around a Crop Tree**, and how should the game describe the possibility that **both retaining and removing trees** can carry wind/stability trade-offs?

Answer in a way suitable for **educational copy**, not rules or Irish risk coefficients.

## What is already decided and not under review

- Scenario One's first demo remains a complete multi-cycle game under accepted milestone/decisions D-059/060, Rack V2, MFM V1 and S1-C/S1-D.
- Lesson 3's current goal is **at least 30% planned Hegyi Share reduction per selected Crop Tree**, with a **40% basal-area safeguard**. This is explicitly **GAMEPLAY CALIBRATION**, not an Irish stand prescription.
- Player-confirmed Crop Trees, player manual precedence, MFM's "proposal not Fell mark" separation and Work Plan approval remain mandatory.
- The player can already keep a neighbour by leaving it unmarked. A future optional **recorded intention** must not imply the game demonstrates shelter or habitat improvement, turn competition off, or substitute for the 30% Share requirement.
- New Scenario One games run **storm model 0**, so the current default simulation provides **no wind-damage consequence** for removing or retaining neighbours. Do not propose a numerical release-risk indicator or silently enable the dormant wind model.
- **No pre-demo gameplay/lesson/save/UI changes** from the six-paper review. This research may proceed now because it is browser-only documentation; future use needs explicit product approval.

## Start with project sources (do not reinvent baseline)

1. The six-paper extraction and original PDFs in the Game Dev project sources:
   - **P1:** Pommerening, Maleki & Haufe (2021), individual-based forest management, DOI 10.1016/j.foreco.2021.119677. The reported crop-tree-vs-competitor marking agreement is P1, **not** the P2 low-vs-crown comparison.
   - **P2:** 2018 rating experiments in forestry, DOI 10.1371/journal.pone.0194747. Agreement does **not** score forest quality; its Table 3 uses the study's cited category scale.
   - **P6:** Pommerening, Szmyt & Duchiron (2024), regeneration-method taxonomy, DOI 10.1016/j.tfp.2024.100597, for shelter/edge/open distinctions only.
2. \`Docs/Project/research-index.md\`, specifically the existing **Irish photographed tree-marking pages 84–87** and **D-065 delegated-treatment** caution on machine-cab visibility. Do not call the photographed page excerpt a proven official Irish national standard: its containing book metadata remain unresolved.
3. \`Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md\`, \`Docs/Research/RacksAndTreeMarking/CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md\`.
4. Attached \`CCF_CCF_Practice_Economics_and_Transformation_Evidence_Composite.md\` (Irish transformation trials/wind/economics), plus \`Delegated Marking and Operator Selection in Irish, UK and European Forestry.pdf\`.
5. Accepted \`specs/multiscale-forestry-management-v1/spec.md\` and the [Manager independent review disposition](https://github.com/jermur788/CCF/blob/docs/scenario-one-objectives-six-paper-review-20261111-v1/Docs/Handoffs/ScenarioOne-Objectives-Six-Paper-Review-20261111/MANAGER-INDEPENDENT-REVIEW-DISPOSITION-AND-DOCUMENTATION-FREEZE-20261111.md).

Project index/composites **do not substitute for checking the original primary source** when making a new claim.

## Browser evidence to collect

Search and inspect **official Irish forestry guidance first** (relevant Department of Agriculture, Food and Marine forestry guidance; Teagasc forestry; Coillte/professional Irish CCF materials where public), then **UK Forest Research/Forestry Commission/Pro Silva practice**, then independent peer-reviewed windfirmness/stability research where directly relevant. The goal is *a few strong sources*, not an exhaustive bibliography. Use dated and versioned sources; check whether policy guidance currently applies to 2026 and whether a technical paper is about newly planted stands, post-thinning stands, mature stands or conversions.

Answer specifically:

1. What are credible **reasons to retain** a spatially competing Sitka or broadleaf neighbour (e.g. canopy shelter, gradual exposure, habitat, timber, seed supply, site constraints, stability/risks)? For each: distinguish **player intent**, **modelled effect CCF can actually observe**, and **real-world function that CCF does not currently model**.
2. What source-specific examples show **when removal may benefit** a Crop Tree and **when very heavy/sudden release may be unwise**? What about the stability risks of **leaving dense unthinned stands**? Never assume a monotone wind rule.
3. Is evidence strong enough to justify a broad, non-numerical caveat in the game's Help/teaching copy? Propose exact 1–3-sentence wording with sources and an alternative conservative option if not.
4. Does current practice evidence suggest a **single 'correct' competitor** or a range of decisions? Do **not** use Fleiss kappa as a player-quality coefficient.
5. What is the actual limited disagreement between **future-crop-tree/manager selection and operator/contractor selection of competitors**, specifically where cab visibility limits crown assessment? Existing game uses player review; say whether sources reinforce this or identify a new safety problem.
6. Are "canopy shelter", "potential habitat", "later timber" and "reassess later" appropriate **player-intention reason labels**? Flag any labels that wrongly promise measured ecological value.
7. What evidence would be needed before a modelled wind/stability consequence, dominance-dependent release target or habitat decision *could* be proposed? Identify missing Irish data/implementation state, **not** invented coefficients.

## Required evidence table and output

The report must contain:

- **Executive decision (≤250 words):** what copy/selection rationale is safe with current project evidence; what is not.
- A **claim-by-claim table** with claim, primary source, link/DOI, exact pages/paragraphs, geography/stand context, confidence, whether **direct Irish**, **transferred/foreign**, **model derivation**, **gameplay calibration** or **unsupported**.
- **2–4 short alternative player copy options**, including an accurate caveat that the **default game does not simulate wind damage**.
- A design **agreement/disagreement** note explaining what P1 and UK practical guidance actually contradict, if anything; do **not** manufacture "contested science".
- A minimal **do-not-import** list of thresholds/coefficient claims.
- **Conclusions for Manager:** REUSE CURRENT RULE / COPY ONLY / MAY SUPPORT LATER FEATURE / NEEDS NEW EMPIRICAL EVIDENCE.
- Publication URL and a **readback** check that the whole report is in the requested branch.

No invented practice, no exact unverified Irish thresholds, no model coefficients, no new feature spec and no gameplay implementation.

## Operational/safety instructions

- Refresh live \`Docs/Project/coordination/active-tasks.md\` and #387 before any authoring. This handoff is a **READY claim**, not a lock grant. Do not overlap an actual writer; stop if conflict.
- Only write the named report/appendix on the **exact assigned branch**. **No edits to** Decision Log, Current Milestone, Research Index, accepted specs, source code or the other research worker's files.
- At start comment [RUNNING] with actual model/provider, GitHub branch, **remote docs-only / no worktree**, and observed base. If blocked, comment [BLOCKED].
- Publish via GitHub; verify remote readback; comment final handoff link and concise status; mark [HANDED-OFF] and close #387.
- The receiving worker may do ordinary web browsing. It **must not** open local machine accounts, use Unity, change policies, alter frozen archives or run builds.

## Copy/paste dispatch line

> Perform browser-only Irish/UK forestry source research for Scenario One **tree competition, optional retained-neighbour rationale and two-way wind limitations**, strictly following the complete GitHub handoff: https://github.com/jermur788/CCF/blob/docs/research-ccf-marking-wind-20261112-v1/Docs/Handoffs/Research-CCF-Marking-Wind-20261112/RESEARCH-HANDOFF-IRISH-UK-RETAINED-COMPETITOR-WIND.md . Claim https://github.com/jermur788/CCF/issues/387 and use only the named remote docs branch; publish the cited evidence Markdown, read it back and return its URL. Do not touch Unity, gameplay, or canonical decisions.

**User action to dispatch:** paste that line into a separate browser/Deep Research chat. **No other user action now.**
