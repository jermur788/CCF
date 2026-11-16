# Sen-1 — Manager graphics audit verdict and DRAFT specification gate

**Date:** 16 November 2026. **Manager:** Scenario One Implementation Manager [#424](https://github.com/jermur788/CCF/issues/424), actual OpenAI ChatGPT GPT-6, browser/GitHub only (effort selector not observed). **Manager docs task:** [#572](https://github.com/jermur788/CCF/issues/572). **Verdict: APPROVED AS A DOCUMENTATION AUDIT AND DRAFT-SPEC INPUT ONLY.** Not independently proven Unity state; NOT an APPROVED spec, game visual acceptance, new player-test candidate or implementation release.

## Reviewed exact evidence

- Worker deliverable: [Sen-1 source-graded ground graphics audit](https://github.com/jermur788/CCF/blob/docs/sen-1-graphics-parity-audit-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-SCENARIO-ONE-GRAPHICS-PARITY-AUDIT-AND-SPEC-HANDOFF.md), authored by Claude Code Opus 5.5 on independent docs branch at user-provided source tip. Direct GitHub compare against main at intake was **one added Markdown file, no source/scene/asset/meta changes**, 1 ahead / 0 behind, no PR/merge. This Manager inspected the full report and section 7–9. I did NOT run Unity, visit original local external screenshot files, profile the game, or independently rerun its exact source-parsing script. Source findings remain **worker-reported inspection evidence** unless independently verified later.
- Report distinguishes seven route consumers (source names/positions), a shared track/ground soil texture, excessive track tint, no normal map, missing soft edges, grid-centred vegetation without constructed-surface exclusion, unlit fallback plants, disabled fog/unset tonemapping/default sky and a bare Year-0 floor. Confidence caveats are appropriately preserved: five **historical** screenshots, one from an unrelated 30m test arena; E80-specific overlap and distorted UV must be captured in an actual E80 baseline, not asserted from visual memory.
- Actual source-coupling risk governs implementation: **52 named authored path GameObjects and related runtime segments** are used by starting stand, Stand Map, coverage, management prescriptions, headland/approach renderers and Editor builder. Renderer hiding only is a **hypothesis pending a full caller inventory and native regression**, not authorisation to delete/move/rename objects.
- The worker's earlier E80 median frame estimate (about 40ms) is historical Overview context, not a fresh benchmark. The desired +15% cap must be tested against current same hardware/seed/pose.
- The safe benchmark is the genuine E80 **spawn clearing → main track → southern feeder junction**, including the prior junction complaint. No topology/rack/forest-floor ecology/terrain relief editing.

## Initial forestry material evidence for Q1 (external, supporting rather than accepted design)

Published Irish COFORD 2004 *Forest Road Manual* describes **engineered tracks** as prepared formed corridors not normally surface dressed and **harvest tracks/racks** as clearances or rows used for extraction. UK Forest Research 2006 CCF case studies distinguish stone-surfaced forest roads, lower-spec tracks and unsurfaced/brash racks. Hence a **worn compacted earth main track with some stone**, a dirt/organically edged lower-spec headland/feeder and unsurfaced/brash extraction racks is an **evidence-informed visual design hypothesis**, not a known specification for the game's particular site. Do not misdescribe the 2.6m main game plank geometry as proven to be a 4m engineered forestry road. Example sources:
- [COFORD Forest Road Manual (2004)](https://limerickandtipperarywoodlandowners.ie/wp-content/uploads/2024/04/ForestRoadManual-1-.pdf)
- [Forest Research, Operational Experience of CCF: UK Case Studies, Appendix 1](https://cdn.forestresearch.gov.uk/2022/02/ipin_1306_ccf_case_studies.pdf)
- [Irish Sitka track location photo, Geograph](https://www.geograph.ie/photo/3441512) — **reference only; not permission to commit/reuse image assets**.

## Q1–Q7 Manager disposition — status matters

| Q | Disposition | Authority / limitation |
|---|---|---|
| Q1 track appearance | **OPEN OWNER LOOK CHOICE.** Recommend compacted worn earth main track, lower-spec organic headland/feeder, unsurfaced racks; research and visual reference alternatives to present first. | Forestry evidence informs art, does not prove game property's actual surface. No production material locked. |
| Q2 vegetation | **MANAGER DRAFT DEFAULT:** relocate a *visual anchor* to safe space **within the same ecological cell and existing budget**; suppress visually if none. Preserve game state, no cross-cell movement. Multi-instance clustering deferred. | Needs exact source/coverage regression and discussion of high-cover cells with no displayable area; not ecology policy. |
| Q3 tone/sky/fog | **COMPARE CANDIDATES, NOT FINAL ACCEPTANCE:** author a proposed E80-only restrained tonemap/fog/sky visual style; no global URP default or scene edits before approval. | Final rendered comparison and user visual acceptance still required; current baseline photos not enough. |
| Q4 new textures | **D-032 OPEN.** Zero unapproved new texture binaries for any initial prototype; licensed existing materials/procedural sources may be compared in a future authorised slice. Proposed four 2048² sets/~60 MB is NOT an accepted budget. | Owner must approve binary budget/storage/licence before production image import. |
| Q5 exterior scenery | **DEFER/EXCLUDE** from first benchmark. Keep boundary as is; outside-forest scenic dressing needs explicit owner acceptance and clear non-simulation attribution. | Does not reverse authored boundary semantics. |
| Q6 80m versus 40m | **MANAGER SCOPE: E80 only** for new graphics; mandatory Legacy40/Reference visual-behaviour regression. | Consistent with D-056 historical Legacy40. |
| Q7 development route | **SPEC REQUIRED** for implementation. Audit alone was PACKET ONLY; actual lighting/material/vegetation/track mesh work is cross-system. | DRAFT must be approved before production. |

## Canonical four-file draft created, still DRAFT

New files on separate Manager docs branch only:
- [spec.md](https://github.com/jermur788/CCF/blob/docs/sen-1-ground-graphics-manager-draft-20261116-v1/specs/scenario-one-ground-graphics-parity-v1/spec.md)
- [plan.md](https://github.com/jermur788/CCF/blob/docs/sen-1-ground-graphics-manager-draft-20261116-v1/specs/scenario-one-ground-graphics-parity-v1/plan.md)
- [tasks.md](https://github.com/jermur788/CCF/blob/docs/sen-1-ground-graphics-manager-draft-20261116-v1/specs/scenario-one-ground-graphics-parity-v1/tasks.md)
- [checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/sen-1-ground-graphics-manager-draft-20261116-v1/specs/scenario-one-ground-graphics-parity-v1/checklists/requirements.md)

The files describe P0 docs and G-03 before-only baseline, later G-04–G-08 bounded visual work and 18 **unchecked** requirement gates. They do not edit any accepted spec or scene. Before any implementation: independently review current DRAFT for source/evidence/acceptance accuracy; obtain actual owner Q1/Q3/Q4 look/storage choices, Manager explicit APPROVED spec status and narrow worker packet. **No Unity lease granted here.**

## Immediate next packet and status

**Task:** G-01/G-02 nonauthor reference + four-file spec quality review [#573](https://github.com/jermur788/CCF/issues/573), **READY / NOT RUNNING**. **Difficulty MEDIUM:** niche track-surface interpretation, asset provenance and cross-system no-regression. **Recommended provider:** independently assigned Claude Sonnet 5.5 Medium or other capable nonauthor browser reviewer. **Exact docs-only review lane:** new isolated branch `review/sen-1-ground-graphics-spec-independent-20261116-v1`, starting from the exact DRAFT tip; no Unity, physical worktree lease, source, Assets, canon, binary files or main edits. **Result path:** `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-AND-REFERENCE-REVIEW-20261116.md`. [Full review dispatch](https://github.com/jermur788/CCF/blob/docs/sen-1-ground-graphics-manager-draft-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-REVIEW-PACKET-20261116.md).

**Paste-ready dispatch:**
> Take CCF Sen-1 #573 as an independent docs-only reviewer: https://github.com/jermur788/CCF/blob/docs/sen-1-ground-graphics-manager-draft-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-REVIEW-PACKET-20261116.md . Register actual provider/model/effort, new isolated read-only review branch/worktree and initial HEAD. Review the four-file DRAFT, Q1 source provenance, route-object/Legacy40/Reference/save non-regression, UI lighting/readability, author visual gate and performance baseline. Publish the exact one-file GitHub review and mark #573 HANDED-OFF. No Unity or production changes.

## Current programme and user action

Existing DeepSeek save-system #544 remains an independent active critical source sequence. #443 Objective 1 overlap remains frozen; G-03 Unity cannot compete with physical Editor operations. Preserve real user saves, frozen Reference Future, D-071/072 existing five racks and D-032 OPEN. **Current user action:** none for continued docs research/review. Before production, user will need to choose the surface look and approve actual lighting/texture budget; later the user must inspect before/after rendered E80 visuals. **No Windows build.** Dashboard ingestion and Drive publication **NOT VERIFIED**; don't call either synced.
