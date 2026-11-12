# Overall Manager disposition — independent specification review and returned browser research

**Date:** 2026-11-12  
**Review result:** **APPROVED AS SUBSTANTIVE REVIEW EVIDENCE**; original four-part DRAFT remains **NEEDS CHANGES / NOT APPROVED**.  
**Revision:** the original broad spec is preserved on branch `docs/scenario-one-research-informed-spec-20261112-v1`; this branch carries a **much narrower, DRAFT copy V1**.  
**Independent reviewer:** Claude Opus 5.5; [durable report](https://github.com/jermur788/CCF/blob/docs/claude-review-research-management-spec-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md); [claim #389](https://github.com/jermur788/CCF/issues/389) reported [HANDED-OFF] and closed.  
**Other research:** [#387 Irish/UK marking and wind](https://github.com/jermur788/CCF/blob/docs/research-ccf-marking-wind-20261112-v1/Docs/Research/ResearchIntake20261112/IRISH-UK-TREE-RETENTION-RELEASE-WIND-EVIDENCE.md) and [#388 Irish regeneration terms](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Research/ResearchIntake20261112/YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md), claims reported [HANDED-OFF] and closed.

## Source inspection and limits

Manager directly retrieved/read the published Claude review, both research reports, and inspected `ScenarioManagementEvent.cs`, `ScenarioReferenceArchive.cs`, `ScenarioOneRackReview.cs`, `ScenarioOneObjectives.cs` on GitHub main. The relevant code confirms (i) the management event has no dual-tree/reason fields; (ii) the world hash uses serialised save data with version-specific layout stripping; (iii) first guided retain/release decisions are session-only; and (iv) internal objective display labels remain technical. Claude also directly inspected UI mapping and lesson text on main; Manager accepts this as **reviewer-observed evidence**, not independent Unity runtime verification. No Editor or compilation was run.

The browser reports are **accepted as supporting qualitative research and copy options**, not new forestry coefficients or validation of current model output. Manager did **not** independently re-fetch every external Irish citation or read every primary page cited; unusual or specific practice claims require checking the cited source before publication. Their bounded general copy is usable for drafting subject to normal product approval.

## Findings and dispositions

| Finding | Manager verdict | Exact correction |
|---|---|---|
| **B-1: new `ScenarioManagementEvent` fields and appended annotation events enter hashed save layout** | **ACCEPT — blocking** | Remove FR-07's event-based preferred architecture from implementable V1; no persistence in V1. A separately approved future feature spec must test world/reference identity with AND without annotations and all old-version/frozen fixtures. A separate save list is **one candidate, not a proven harmless solution**: it also needs protected hash-layout exclusion, migration and explicit user approval. |
| **M-1: regeneration wording partly already shipped** | **ACCEPT** | Confirm existing `ScenarioOneUiFacts` maps metric to “Areas with young trees” and Year-100 presentation; only residual player strings and truthfulness are candidates. Do not edit internal `displayName`, stable objective ID or persisted Century Review label. Distinct natural/planted global counts need new aggregation and are excluded. |
| **M-2: research claims stale** | **ACCEPT** | #387/#388 reports are published/closed, now Manager-reviewed as support; no new browser research needed for conservative copy V1. Source-specific claims still require evidence check if actually quoted. |
| **M-3: retention prompt duplicates existing Objective 5 retain/release** | **ACCEPT** | No new control in V1. Later design-only comparison must begin with current [J] Retain here / [K] Release further interaction. Current choice is per Crop Tree, not a persistent per-neighbour decision. If designing any neighbour-specific UX, define the genuine model competitor set and visible subset, including broadleaf neighbours. |
| **M-4: Share explanation omits key limitations** | **ACCEPT** | Explain that local Share uses DBH and distance and **does not itself evaluate species, height, crown architecture, shade tolerance or wind stability**. Reword the bare “being small is not a reason to fell” statement to specify that this refers to the **current crown-release exercise**, not forestry as a whole. Preserve grade, 30% and 40%. |
| **M-5: shelter/edge/open location classification not implemented** | **ACCEPT** | Use #388 qualitative text only in general Help; no new spot class, no conflation of silvicultural open ground with existing numeric light band “open”. |
| **M-6: 69 checks/extra Unity lanes excessive** | **ACCEPT** | Draft one bounded Low-difficulty copy task with concise tests (literal inventory, unchanged IDs/hashes, snapshots at existing supported resolutions, focused readback). Do not commission a separate full-feature qualification or Tier-3 for copy. Use next already-planned integration qualification if eventually merged. |
| **Minor: reason vocabulary/historic statements** | **ACCEPT** | No auto-generated forestry reasons, no “no reason recorded” noise in pre-feature history; canopy cover/shade safer than asserted wind shelter. |
| **Optional P3/P5 metrics** | **KEEP OUTSIDE V1** | Offline authentic-fixture analysis only; no player panel unless later evidence and separate decision. |

## Manager's narrowed recommendation

**Copy V1, post-current-demo by default**:
1. Correct a short Lesson 3 explanation of what local `Share` measures and omits; avoid unique “correct neighbour” inference. Use #387 **Option D** for a short low-claim lesson sentence; **Option A** as general Help only if screen and source review permit.
2. Narrow the existing “being small is not a reason to fell” phrasing explicitly to this crown-release teaching method.
3. Keep the *already existing* “Areas with young trees” UI label unless product owner elects one safe presentation-only swap to “Areas with young growth.” Audit and fix residual player jargon (e.g. “regeneration cohorts”) and consider a truth-preserving Oak/Beech label where current predicate is only planting plus species presence; do not edit persisted objective labels.
4. Add one **general Help** sentence from #388 on shade/edge/open conditions; do not add location-specific classification.
5. State “The default Scenario One does not simulate wind damage” in Help/limitations and external-test documentation, not the already crowded competitor panel.
6. Make **no changed grades, objectives, world data, save schema, annotations, model coefficients, MFM work order or history functions**.

**Retained competitor reasons:** user had earlier asked to document the idea and defer implementation. Existing #386 remains a post-demo research/UX concept. First compare no extra UI with a mock-up extending the current Crop Tree retain/release decision; **no new mandatory control, event or save**. If users later demonstrate a need for **cross-year intention recall**, propose a separate full persistence spec with explicit world-hash/Reference Future tests, not an additive shortcut in copy V1.

## What has and has not changed

- **Accepted as review evidence:** all major findings supported by reviewer source inspection; #387/#388 qualitative copy and provenance with stated limitations.
- **Changed here:** Manager documentation; copy-only four-part **DRAFT** revision for later product review.
- **Not changed:** production C#, UI, Unity, scenario completion, lesson credit, MFM, event/save schema, hashes, historical reviews, Rack V2, source anchors, Legacy40/frozen Reference Future, current milestone, Windows build eligibility or player acceptance.
- **No implementation authorisation:** the new copy draft is not an APPROVED spec. No new local worktree, Unity machine lease, code task claim or source writer assigned.
- **Potential conflicts:** existing complete-loop demo qualification, D-072 and MFM UI owners have priority. Do not interrupt or commandeer their files.
- **Context/Drive/Dashboard:** no canonical content package merged; no manifest/Drive/dashboard sync readback verified.

## Next gate

Product owner/Manager reviews the reduced copy V1 against accepted no-research-change-before-demo boundary. If explicitly approved when a safe source lane is available, issue one small Low-difficulty UI/copy implementation packet with exact current branch/HEAD, confirmed file ownership, a new [READY] claim and Tier-1 focused checks, plus an existing-release qualification screenshot. The optional feature requires fresh product evidence and its own four-part spec.

**Verdict on original draft:** NEEDS CHANGES. **Verdict on revised spec:** DRAFT, pending acceptance. **Immediate user action:** none to preserve current demo.
