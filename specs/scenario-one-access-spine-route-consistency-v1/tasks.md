# Access Spine & Diagonal Rack Network — DRAFT Task List

**D-071 PRODUCT DISPOSITION (2026-11-05) — SUPERSEDED FOR SCENARIO ONE.** The user selected the **existing southern-headland five parallel racks** as sufficient for Scenario One (illustration Option A, equivalent to the current parallel/headland fallback in this research). No full northern spine, diagonal/herringbone 3+2 racks, new single-family spine-entry network, player-built northern extension, new two-family MFM derivation or modified rack-clearance rule is required or authorised for Scenario One. D-064 Rack V2 and D-065 MFM V1 remain controlling. This file preserves independently reviewed research and **is not an implementation licence**; later scenarios require their own product decisions and specs. D-070 hidden workbench retirement remains separate, source-reviewed but Unity-unverified. See [Decision Log D-071](../../Docs/Project/decision-log.md).

**AS-09–AS-13 below are historical not-dispatched implementation sketches, not ready task packets.** Do not open work claims for them as Scenario One deliverables; new scenario selection requires fresh scope and ownership.

**Status: SUPERSEDED FOR SCENARIO ONE / STOP NEW ACCESS-SPINE PRODUCTION after D-071, 5 November 2026.** Each task status is execution truth only; accepted product direction, actual source implementation, Unity verification and external-demo acceptance remain distinct.

## Historical evidence and browser work

| Task | Current status and output | Meaning |
|---|---|---|
| AS-01 / #246 | **CLOSED — source reconnaissance**; `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/RECONNAISSANCE-HANDOFF.md` | Existing bowed path, fixed headland/feeder, rack and map source inventory; no runtime layout proof |
| AS-02 / #248 | **CLOSED — corrected Unity measurement research**; `MANAGER-MEASUREMENT-REVIEW-20261103.md` | 45° best-sampled **18.274 m** maximum-of-minimum spacing is optimistic and not the real #250 candidate |
| AS-03 / #250 | **CLOSED AS RESEARCH / STOP IMPLEMENTATION**; `MANAGER-FIVE-RACK-REVIEW-20261103.md` | Actual 3+2 candidate minimum **16.623 m** same-side; illegal entrances, 1.414 m centreline failing 2.4 m editor rule, R1 trunk, turning and MFM invalid. Physical rectangles **do not intersect**, 0.214214 m gap |
| AS-04 / #251 | **CLOSED AS CORRECTED RESEARCH**; `MANAGER-MFM-TOPOLOGY-REVIEW-20261104.md` | Conditional side-local A hypothesis, no production owner/reach/sample policy |
| AS-05 / #253 and #271 | **CLOSED / PARTIALLY QUALIFIED OFFLINE RESEARCH**, experimental A/C PR #274 remains unmerged | Reproducible owner partition but finite reach/Manual cost, 150.6 m² contaminated sample and layout C comparison unresolved |
| AS-06 / #283 | **HANDED-OFF / CLOSED — BROWSER SOURCE/DRAFT**; [result](https://github.com/jermur788/CCF/blob/docs/access-spine-start-condition-20261105-v1/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-SPEC-RESEARCH-RESULT.md), draft PR #285 | First four-file spec revision published. No code/Unity |
| AS-07 / #286 | **HANDED-OFF / CLOSED — INDEPENDENT REVIEW: NEEDS CHANGES**; [review result](https://github.com/jermur788/CCF/blob/docs/access-spine-independent-review-20261105-v1/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-INDEPENDENT-REVIEW-RESULT.md) | Seven docs blockers B1–B7, not a feature verdict or approval |
| **AS-07R / #288** | **HANDED-OFF / CLOSED — independent re-review APPROVED FOR RESEARCH QUALITY with C1–C6 minor edits.** [Result](https://github.com/jermur788/CCF/blob/docs/access-spine-b1-b7-review-20261105-v1/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-B1-B7-INDEPENDENT-REVIEW-RESULT.md) | C1–C6 editorial fixes applied to the four-file DRAFT and correction handoff; no E/P/F, MFM, Unity or spec adoption. |
| **AS-08 / D-071** | **RESOLVED FOR SCENARIO ONE — EXISTING PARALLEL RACKS RETAINED.** No new full-spine/herringbone specification approval is needed or allowed in this milestone without another explicit user change. | User chose current five parallel rack/headland arrangement (illustration Option A; historical F + T-F). Complex alternatives retained as research only for future scenarios. D-064/D-065 unchanged. |

## Parallel but separate D-070 workbench retirement

| Task | Status | Boundary |
|---|---|---|
| **AS-W / #284** | **HANDED-OFF / CLOSED, SOURCE/OFFLINE REVIEW APPROVED; NOT UNITY VERIFIED / NOT MERGED**. [Implementation handoff](https://github.com/jermur788/CCF/blob/feature/scenario-one-workbench-retirement-20261105/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-RETIREMENT-IMPLEMENTATION-HANDOFF.md), [Manager review](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/MANAGER-WORKBENCH-RETIREMENT-REVIEW-20261105.md) | Scene **retained**; only workbench instance marked new-world retired at runtime; historical ID, 4 direct prerequisite refs and source/assets retained; all 7 site objects in scene |
| **AS-WQ / #287** | **WAITING — USER PLAYTEST RELEASE + SAFE UNITY-LOCK DEVICE/LEASE**. [Exact packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-PACKET.md) | Verify fresh/old saves, site masks, 336 central tree IDs, full 1,344 count and exact ID sets, hash/reference, UI and scene. Isolated local clone/branch; no full spine work, merge or Windows build |

## Historical access-spine production proposal — DEFERRED TO FUTURE SCENARIOS, NOT SCENARIO ONE

| Task | Proposed narrow deliverable | Ownership and qualification dependency |
|---|---|---|
| **AS-09 — feeder route and save identity** | Register E/P/F status-bearing geometry shared by 3D/Stand Map/minimap, starting stand/Track masks, historical save version or north-original-ID inference | New local claim, exact branch/worktree, Manager lock. Qualify old fully felled/naturally empty northern corridors, new E, Legacy40 and frozen reference. No simultaneous shared Rack/MFM writer |
| **AS-10 — five-rack editing and entrances** | Entry legality from real registered spine/headland, accessible first removal trunk, 16–20 m recommendation and Q4a (retain 2.4 m centreline or approved rectangle+machine clearance) | Only after approved D-064 amendment if changed; ensure old Confirm and executed histories. Source RackPlan/Edit/Entrance writer negotiated with MFM |
| **AS-11 — deterministic conditional MFM** | Eligibility parameters `A_intra`, `A_inter`, `A_chain`, `N_group`; Confirm-time stable derivation version, unique owner, finite reach, explicit Manual/NotCovered, A sample anchor and safe load | Own `ScenarioOneRackSampleGeometry` / `ScenarioOneCoverage` / `ScenarioOneManager.PrescriptionApproval` / `ScenarioOneRackBay.Frame` / rack-save rule contract; MFM owner must supply one stable derived API to UI. No concurrent edits on shared RackPlan or save files |
| **AS-12 — representations and guidance** | Shared status across Stand Map, minimap, 3D, entrance marker, Objectives and Work Plan; coordinate with separate #258/#282 draft | Consumers: `UI/LearningObjectivesView.Sequence.cs`, `UI/StandMapTreatmentReview.cs`, `UI/Map/StandMapAreaFootprints.cs`, `UI/Map/StandMapSource.cs`, `UI/HudMinimapView.cs`. Only after M API stable; coordinate file authorship rather than overlap |
| **AS-13 — independent review, qualification and integration** | Exact-head static/property tests, Tier 2 Unity field/render/save and then milestone Tier 3 only when warranted | Independent reviewer, Manager integration verdict, old hash/Reference Future and new hash labels, no test weakening or Windows build without completed personal playtest and explicit authorisation |

**D-055/D-056 source guardrails:** 7 hidden buildables (Workbench, Saw Pit, Timber Sledge, Shelter, Log Racks I/II and Grinding Stone); after #284 six other Site circles remain, and Workbench's 4 direct references must not fail open by removal. Preserve 336 central Legacy40 IDs and locations, 1,008 outer IDs and 1,344 total. Source-/offline-derived claim of 0 changed slots for #284 must be **confirmed with exact Unity ID sets**.

**Research review status:** #288 approved B1–B7 for research quality; six editorial changes are applied to PR #285 source docs. Final Manager research disposition and user product choices still follow. **Local task:** #287 remains WAITING for playtest release and valid Unity lease. No feature implementation based on this DRAFT.

All substantive workers mark claims [RUNNING] only when genuinely started, record provider/model, branch, worktree, exact starting HEAD, preserve other lanes and publish GitHub Markdown handoff before [HANDED-OFF]. #200 alone writes live `active-tasks.md` and accepts integration. No Windows build.
