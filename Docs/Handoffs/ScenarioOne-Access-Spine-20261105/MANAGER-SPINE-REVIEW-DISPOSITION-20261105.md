# Manager disposition — #286 independent Scenario One access-spine research review

**Date:** 2026-11-05.  
**Verdict:** **NEEDS CHANGES — ACCEPTED FOR RESEARCH QUALITY.** No approved access-spine specification, no approved E/P/F start status, no topology/Option A/rack-clearance/infra-save choice, no Unity/production approval.  
**Manager lane:** #200 browser/GitHub only; D-064/D-065 and D-055/D-056/D-070 remain authority.  
**Independent read-only review:** [#286 review result](https://github.com/jermur788/CCF/blob/docs/access-spine-independent-review-20261105-v1/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-INDEPENDENT-REVIEW-RESULT.md); [claim](https://github.com/jermur788/CCF/issues/286) is HANDED-OFF/CLOSED.  
**Author branch:** `docs/access-spine-start-condition-20261105-v1`, [draft PR #285](https://github.com/jermur788/CCF/pull/285), **not merged**.  
**Corrected response:** [BROWSER-SPINE-REVIEW-CORRECTION-RESULT.md](https://github.com/jermur788/CCF/blob/docs/access-spine-start-condition-20261105-v1/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-REVIEW-CORRECTION-RESULT.md).  
**State:** all seven corrections drafted in the same four DRAFT files, but **independent re-review still required**. This Manager disposition does not mark the corrections verified or the spec APPROVED.

## Inspected evidence

1. Refreshed #286's source-backed reviewer handoff and current Decision Log D-055, D-056 and D-064–D-070; compared its B1–B7 to the four PR #285 documents and current #284 source-reviewed candidate. Independent reviewer reported no unsupported copied numeric claims, correctly differentiated 0.214214 m physical R1/R2 gap from 1.414 m centreline failing 2.4 m editor rule, and preserved experimental PR #274's NO-COMPARISON status.
2. **B1 accepted:** original E/P/F comparison omitted material conservative MFM manual/reach consequence and conflated full route at start with two-sided racks. Revisions now define feeder E/P/F and rack T-F/T-C/T-A axes. Experimental outside-cap area figures: F 0–3.11%, A-nearest 14.13–38.22%, A-lateral 36.25–49.03% across unapproved 20→9 m caps. This is not actual C# Covered/Manual measurement and is not automatically an E-only loss.
3. **B2/B3 accepted with implementation nuance:** deleting the hidden workbench's scene root from shared Legacy40/Enlarged80 core would threaten D-056 IDs and fail open four prerequisite refs. The actual separate #284 candidate instead retains the scene root and uses a runtime retirement flag saved-ID reconciliation; source/offline reviewed, Unity NOT RUN. #284 worker reports zero changed planting slots because the workbench's five excluded slots lie within Clearing, but that expectation still requires exact 336 legacy central ID and 1,008 outer ID tests (not merely 1,344 count). All **seven** D-055 hidden buildables and six remaining Site masks are now inventoried, with indirect dependencies and source/verifier consumers. A separate user decision is required before additional hidden-site retirement.
4. **B4 accepted:** legacy `ScenarioOneRackSampleGeometry.Derive` is not a parallel-only check. Option A eligibility needs explicit named family tolerances/cardinality/quantisation, stable derivation rule identity at Confirm (or proven equivalent), source-sample anchor, old Confirm-positive preservation and one writer for every MFM/UI/save consumer. Parameter values and field/schema selection remain OPEN.
5. **B5 accepted:** E new-start identity should compare I-N northern original-plantation occupancy inference (southern approach as precedent, not proof) with I-V explicit versioned infrastructure field. Old felled/naturally empty north, new E, Legacy40 and frozen Reference Future fixtures are mandatory; hash/semantic costs explicit.
6. **B6 accepted:** present ≥2.4 m editor centreline clearance is D-064 gameplay validation, not immutable machinery clearance. Decide to retain it or approve a reviewed rectangle-separation-plus-real-machine-clearance alternative; the 0.214214 m positive gap alone proves no practical access.
7. **B7 accepted:** historical task #283, independent #286, #284 implementation and #287 Unity qualification statuses corrected, malformed plan reference removed, duplicate spec section numbering resolved. All written changes remain on the docs branch; no accepted production spec was changed.

## Exact remaining blockers

**Independent research re-review** must verify that the B1–B7 amendments are both complete and internally consistent; corrections prepared by the original author are not independent acceptance. **Product choices** still require user/Manager approval: E/P/F feeder, T-F/T-C/T-A racks, acceptable Manual/NotCovered share, D-064 entrance and clearance, other six sites' Site footprint, conditional MFM model/sample, and I-N/I-V save identity. **Runtime evidence remains absent** for full north corridor/turning/3D views, save round-trips, MFM C# and new-world IDs/hashes. User's personal playtest occupancy and Unity lease gate remain outside this browser task.

## Next executable task and ownership

Issue a **new bounded docs-only re-review claim** for independent examination of PR #285 correction handoff; do not reopen #286 or self-approve. Reviewer must read the exact #286 result, verify all four revised DRAFT files and return an APPROVED / NEEDS CHANGES / REJECTED verdict *for documentation research quality only*. A future APPROVED research verdict does not approve E, Rack V2 amendment, Option A or any game implementation.

Separately, #284 workbench source implementation is HANDED-OFF/CLOSED and source-review approved, but **#287 Unity verification is WAITING** for explicit personal-playtest release and a valid exclusive lease. Do not let the access-spine reviewer change that separate integration status.

**Dashboard:** Manager shall reconcile live `Docs/Project/coordination/active-tasks.md` and `jermur788/Overall-Dev/updates/ccf/latest.md` after the new re-review claim is published; verify GitHub read-back, not automatic downstream ingestion.

**User action now:** none for the documentation response. Later, explicit product decisions are needed; no Windows build until the separate user playtest and authorisation gates.
