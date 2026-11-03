# #251 — Design a safe MFM sample/coverage contract for a branched access spine

**Status: READY / READ-ONLY SPEC ANALYSIS ONLY. No Unity or production code.**  
**Issue:** https://github.com/jermur788/CCF/issues/251  
**Date:** 2026-11-03.  
**Purpose:** investigate the smallest safe model change, if one is justified, that permits a player to use a full central access spine and diagonally branched extraction racks while retaining trustworthy sample-to-comparable-area coverage and continuous-cover management.  
**Difficulty: HIGH** — coupled rack topology, spatial allocation, sample teaching, MFM comparability, Work Plan, persistence, test qualification and backward compatibility.  
**Overall Manager:** #200; **parent orchestrator:** **DeepSeek DPC** (#201/#203).  
**Worker:** **Claude Sonnet 5.5 Medium** or **OpenAI GPT-6.1 Medium**, selected by live observed capacity, task fit and sufficient context; route independent review to the other provider. Pool enabled is not a quota promise.  
**Canonical return:** `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MFM-TOPOLOGY-OPTIONS-HANDOFF.md`, published as GitHub Markdown with independent review and V2 Orchestration Decision Appendix. Link #251 and mark [HANDED-OFF] on return.

## Authority and evidence

- User preference: central spine plus diagonal branches, Decision D-067. Manager current direction: full existing central in-stand route plus northern extension and 45°/3+2 as a *reference proposal*, D-068. **The draft access-spine four-file spec remains DRAFT.**
- Rack V2 / D-064 is **currently controlling**. Map plans geometry, physically marked entrances, confirmed rack records, locked executed removals, Work Plan is the commitment gate. MFM V1 / D-065 preserves tree-level demonstrated judgement and comparable-area review, not automated blanket approval.
- [#248 corrected handoff](UNITY-GEOMETRY-HANDOFF.md), [#250 corrected reference handoff](FIVE-RACK-REFERENCE-HANDOFF.md) (both on `main` after docs-only PR #249), and [Manager verdict](MANAGER-FIVE-RACK-REVIEW-20261103.md).
- **Actual STOP:** `ScenarioOneRackSampleGeometry.Derive` returns `Valid=false` for the specific 3+2 five-rack candidate because projected R4/R5 intervals overlap on the global normal. This invalidates current candidate-specific MFM sample/coverage derivation. Do **not** claim every herringbone arrangement is impossible.
- Other blockers are separate: continuation passes near Forestry Workbench, current entrance authoring is headland-only, R1/R2 corridors overlap, R1 far end intersects a trunk collider, and actual map/minimap/3D rendering is **UNMEASURED**. This packet does **not** implement repairs for any.
- External evidence folders `/media/jer/Files/CCF/Verification/ScenarioOneAccessSpineMeasure-20261103T2110Z/` and `/media/jer/Files/CCF/Verification/ScenarioOneFiveRackReference-20261103T2210Z/` are **local-only**; never substitute inferred bytes for inaccessible raw data. Request safe evidence access when needed, and mark source unverified if unavailable.

## Mandatory source audit (read-only)

Inspect published candidate `integration/scenario-one-player-test-ready-v1` (freshly resolve current ref), in particular `Assets/ForestPrototype/ScenarioOne/ScenarioOneRackSampleGeometry.cs`, `ScenarioOneCoverage.cs`, relevant `ScenarioOneManager` rack/MFM prescription and confirmation paths, `ForestSaveData` and validation, `ScenarioReferenceArchive`, Stand Map/minimap and Work Plan bridge, and the approved Rack V2 and MFM specs. Cite exact file/line and distinguish source fact, inference, gameplay calibration and unsupported policy. Do not alter accepted MFM tolerances or the existing parallel-rack sample semantics.

## Options to evaluate, not preapprove

**A. Branch/side-local frames (preferred investigation hypothesis):** treat the three west and two east branches as disjoint, identifiable rack-side groups; derive local geometry per side (not a single global common normal), assign each cell/stem to exactly one group or explicit unassigned/manual remainder, and retain transparent treatment-comparability checks. Determine whether **one demonstrated 3D sample** can validly inform both sides, or whether separate sample/approval is necessary; do not silently change the user's demonstrated judgement. Explain how shared spine/overlaps, edge cells, excluded masks and competing groups are resolved deterministically.

**B. General topology/graph-aware coverage:** build stable areas using a graph of confirmed route/rack connections or verified spatial partition, with clear assignment and overlap behaviour. Evaluate engineering, explainability, persistence, replay determinism, future complex stands and risk of adding a second authoritative spatial model. No proposed geometry should immediately change masks.

**C. Constrained parallel-rack fallback:** keep the existing `Derive` and adjust permitted layout/topology to one ordered rack family while providing an access spine. Estimate what the user loses (diagonal branches, real-world map consistency, freedom to make poor proposals) and whether a feasible forestry/geometry layout exists; do not claim feasibility without measured evidence.

**Compare** determinism, sample pedagogy, number of manual demonstrations, unit/area remainders, mask exclusions, performance, risk, save evolution, testing cost, map/UI interpretation, future-proofing and simplest reliable implementation. Explicitly say if all options have unresolved unacceptable cost.

## Contract invariants and test design

1. **One authoritative forest**: all stem/cell biology, marks, orders, history and progress remain authoritative in existing world; no second biological state or hidden treatment execution.
2. **Coverage assignments cannot double-count.** Every applicable tree/area maps to one and only one covered/not-covered/excluded/manual result, or a documented conservative nonapplicable state. Shared boundaries/overlapping corridor geometry cannot result in two work orders or duplicate stem removal. Stable deterministic ordering, ties and hashes are mandatory.
3. **Confirmed-vs-proposed semantics:** Draft route strokes, poorly located or disconnected starts, suggestion overlays and painted entrance cues are *not* Fell marks, executed corridors or MFM exclusions. Distinguish `Proposed`, `Confirmed`, `EntranceMarked`, `WorkPlanApproved` and `Executed/Cleared`; reconcile with current authoritative states before prescribing new ones. Only correct committed/physical ground participates in actual exclusion, per accepted policy; do not invent real silvicultural practice.
4. **Sample demonstration and fit:** player demonstrates 3D judgement; comparable areas are derived transparently with recorded reasons and appropriate conservative fallback for mismatches. Retain manual override, non-Sitka adult exclusions, browsing/protection and species safeguards. Reopening or editing racks cannot rewrite historic work or executed trees.
5. **Compatibility:** baseline parallel Rack V2/MFM on the already accepted candidate must stay qualified; no changed sample units, completion semantics, user-save corruption, silent v24/schema jump or Reference Future v1 / Legacy40 drift. Enumerate any required schema/compatibility amendment with testable migration/default rules; do not implement one.
6. **Adversarial cases:** two opposing branches touching the spine, intersecting corridor widths, branch crossing common normal but not intersecting physically, short northern branch, interior entrance, incomplete five-rack plan, overlapping cells, unmarked entrance, tree already Fell-marked, uncovered remainder and player manual area. List expected stable error or conservative result rather than forcing a green status.
7. **Qualify only affected change later:** propose offline deterministic/unit/property tests and a bounded eventual Unity/renders test, including Stand Map/minimap/3D/Work Plan and load/save. Do not weaken current `Derive` or existing baselines merely to accept a candidate.

## DeepSeek orchestration and publication discipline

DPC checks live #200/#203/#251 and `Docs/Project/coordination/active-tasks.md` before start; #200 remains sole writer of active-tasks.md. **Read-only source lane:** inspect the published `integration/scenario-one-player-test-ready-v1` ref through GitHub or `git show` in an existing clean workspace; no Unity, new Unity Library or exclusive Unity lease needed. **Docs-writing lane reservation:** `docs/mfm-topology-options-20261103-v1` under an isolated **docs-only sparse** worktree `/media/jer/Files/CCF/Operations/CCF-MFM-TopologyDocs-20261103`, **only if path/branch unused and ownership free**. If conflicting, STOP and ask Manager for reassignment; do not touch another owner's worktree. At real worker start mark #251 [RUNNING] with actual model/provider/Medium effort, inspected candidate ref/HEAD and docs lane. Use own PR for durable docs.

Every substantive dispatch, provider/context/capacity check, retry, block, review, correction and handoff goes as a timestamped append to the [live V2 ledger](ORCHESTRATION-LEDGER.md). Keep `OBSERVED FACT`, `ORCHESTRATOR CLAIM`, `RETROSPECTIVE INFERENCE` distinct. Unknown quota/context/token metrics stay **UNKNOWN**. Provide alternatives and why routed; independent cross-provider review is mandatory.

**Publish-time numerical provenance rule:** re-extract every count, spacing, mask delta, review tally and other numerical claim from its actual raw record at publication; cite path/key/row and reconcile totals. Do not summarise from memory. Preserve superseded calculations and correction traces; explicitly identify any claim that lacked source access. Treat errors as evidence to correct, never conceal. Do not mistake an environmental/quota failure for model incapacity.

## Completion and authority

Return a concise options matrix, minimum viable interface/contract proposal, explicit MFM/Rack V2 spec clauses requiring amendment, test matrix, no-go risks, proof gaps, review verdict and recommended next spec decision. **Do not** change the four-file draft status to APPROVED or implement source/UI/scene/save/MFM/Workbench changes. DeepSeek may continue another independently READY/approved, non-overlapping task while this is blocked, but cannot manufacture work rights.

**No Windows/Player build** before user completes personal Scenario One playtest and explicitly authorises the build afterward. User playtest branch and separately staged intro branch unchanged.

## Paste-ready dispatch to DPC

> DeepSeek DPC: run the **read-only MFM topology-aware sample/coverage specification study #251** from https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MFM-TOPOLOGY-OPTIONS-PACKET.md . Compare branch/side-local sample regions, a graph-aware model and retaining parallel racks, against the actual `ScenarioOneRackSampleGeometry.Derive` stop in #250. Preserve player-demonstrated judgement, non-overlap, MFM safety, existing saves and D-064/D-065. Use Sonnet 5.5 Medium or GPT-6.1 Medium per observed capacity; independently review with the other provider. Keep V2 routing ledger and re-extract all published numbers from raw evidence. Publish MFM-TOPOLOGY-OPTIONS-HANDOFF.md; no Unity, production implementation, felling, or Windows build.

**Exact user action:** forward the dispatch to the active DeepSeek DPC session. **No product authorisation** is required to research design alternatives; new gameplay/forestry rules require subsequent Manager/user approval.