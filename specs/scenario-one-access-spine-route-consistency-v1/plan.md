# Access Spine & Diagonal Rack Network — Draft Delivery and Qualification Plan

**Status: DRAFT / NO PRODUCTION AUTHORITY. Revised 2026-11-05 under browser claim #283.** Authoritative until a further approved amendment only as planning material; D-064 Rack V2, D-065 MFM V1, Legacy40 and frozen Reference Future v1 remain binding.

## 1. Outcome and current evidence

Evaluate a coherent *existing* full central feeder track for future new Enlarged80 starts, with **five separate diagonal extraction racks** planned and physically marked by the player. The 45° off north / 3+2 herringbone is a research reference, not an accepted geometry. The user-approved D-070 retirement of the unused invisible Forestry Workbench is separate from the unapproved full-spine decision. Do not present the currently approved headland-only Rack V2 as already replaced.

Research already available: #246 source inventory; corrected #248 measurements; corrected and reviewed #250 candidate (spacing pass, entrance/MFM/clearance/turn/render fails); #251 architecture; #253 partly qualified offline owner/reach study; #271 / open PR #274 experimental Option A–C comparison (not policy). The complete design/source matrix and options E/P/F are in `spec.md` `6. Source and derived research must not be conflated with Unity proof.

## 2. Browser/management sequence (no Unity)

1. **#283 research/source reconciliation — this lane:** read main and ownership; compare E (established complete spine), P (present route and prospective extension), F (constrained Rack V2 fallback). Record initial-stand masks, physical access, MFM, Rack V2 and save semantics, with evidence labels.
2. **#283 four-file DRAFT update — this lane:** revise `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` only on `docs/access-spine-start-condition-20261105-v1`. Publish `Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-SPEC-RESEARCH-RESULT.md` there; mark the claim HANDED-OFF, not feature-done. The result goes to Manager #200.
3. **Review of the DRAFT:** independent source/design review must explicitly test the 3D/spine-entrance interaction, MFM one-owner/reach proposal, workbench retirement and historical-save/version policy. A separate review claim and verdict precede Manager spec disposition; DO NOT self-approve.
4. **Product gates:** explicit user/Manager approval for E vs P/F, initial-stand rebaseline policy, spine entrance authority, final five-rack topology, MFM owner/reach/sample policy, and paid-access implications. Document decisions in Decision Log as appropriate. If accepted rules conflict with D-064/D-065, update their approved specs through authorised review rather than silently bypassing code.
5. **Spec transition:** only after decision/independent review/criteria closure, change this four-file status to APPROVED, or retain DRAFT and STOP. Approved plan/tasks then authorise separately claimed implementation.

## 3. Proposed independent production lanes — not released by this plan

| Phase | Target change and single writer | Capability / difficulty / entry |
|---|---|---|
| W — unused workbench retirement | #284 isolated local T3 lane; scene roots and buildable prereqs, new-world Site mask, save IDs; **no full-spine implementation** | Local GPT-6.1 Sol Medium/High or equivalent C#/Unity worker. Medium–High: scene/save/coverage cross-system. Requires independent narrow Manager contract, checked branch/worktree, user session release and exclusive Unity lease. |
| G — authoritative start infrastructure | New claim and isolated local lane: one route definition consumed by starting stand, scenery, masks and safe old-save load; E **only after accepted** | High: deterministic starting trees/path/save. Separate write owner for `StandApproach*` / `ForestStartingStand` / world geometry. |
| R — rack editor/entrances | New claim and isolated local lane: legal spine/headland entries, robust clearance, five valid branches, physical target selection and existing reopening/locked orders | High: cross-UI and work-state constraints. Own rack plan/editing and entrance marking only after G contract stabilises. |
| M — MFM topology/reach | New claim and isolated local lane: strictly additive branch-local selector with unique ownership, finite reach, safe legacy/confirmed-invalid preservation, sample calibration | High: coverage/save integrity. Own sample/coverage/prescription paths; cannot concurrently write R or G overlap. |
| V — three-view/lesson/status fidelity | New UI/render lane after authoritative network and statuses stabilise; coordinate separately with #258/#282 objective guidance | Medium–High: map/minimap/3D consistency and UI test; do not conflate lesson diagram with actual route. |
| Q — independent review/qualification/integration | Reviewer separate from implementation; Manager owns canonical integration and coordination | Tier 1 during coding; Tier 2 exact-head scene/save/3D/UI; Tier 3 only milestone candidate; no premature Windows build. |

All future implementation handoffs need exact own claim, branch/worktree, starting commit, file ownership, non-overlap checks, explicit source-change boundary, Unity lease conditions, protected tests and GitHub Markdown return. Do not open Unity during user's active playtest; do not touch their existing main checkout, current saves or frozen archive.

## 4. Design dependency graph

`D-070 approved intent → #284 safe workbench/source contract → independently reviewed baseline`. This can proceed separately from the access-spine feature.

`E/P/F decision + corridor/entrance/turn measurement + new-start/save policy → APPROVED revised spec → authoritative route model (G) → rack entrance/corridor (R) + MFM valid ownership (M) → unified views/teaching (V) → review / feature qualification (Q)`.

If E is rejected or cannot pass feasibility and historical-save gates, choose F explicitly; do not fake a P work-order mechanism. Option A cannot be selected solely because the existing global `Derive` rejects herringbone. A distinct safe implementation remains conditional on independently accepted bounded reach and unique ownership.

## 5. Mandatory checks

- **Site and start:** quantify tree count, stable IDs/positions and generation determinism for new Enlarged80; distinguish `Track`, `Site`, `Headland`, `Clearing` masks and omitted tree/corridor areas. No hidden workbench `Site` in new world; do not retire other hidden construction footprints without product choice. Compare pre/post state; accept a changed baseline only through a recorded decision.
- **Routing and entrances:** reproduce five branches in real stand-space with nonoverlapping permissible corridors, separate spine count, on-spine legal entrance trees, reachable spray interaction, safe crossing, boundary and turning. Re-test R1/R2 2.4 m centreline rejection and far-end trunk; do not assume geometry was fixed by removing the workbench.
- **Save correctness:** new-world infrastructure identity cannot come from old stand-approach occupancy inference alone. Load pre-feature Enlarged80, current valid Rack V2, confirmed-invalid legacy, Legacy40, Reference Future v1 and historical workbench-ID saves safely; keep executed work/order IDs and immutable archive bytes.
- **MFM:** legacy `Derive` unchanged for all accepted layouts; branch-local two-family eligibility bounded, one owner per tree/cell, conservative finite-segment reach and exposed Manual/NotCovered, no double-counting; independent representative-sample test before cross-side transfer is allowed.
- **Transitions:** proposed vs existing vs confirmed vs marked vs approved vs executed remain truthful in maps/minimap/3D, masks, Work Plan and guided objectives. No proposed Fell marks; approval remains commitment gate.
- **Testing:** offline geometry and save/oracle fixtures, actual Unity Editor import/compile and focussed rendered/input tests in an exclusive local lane; no test weakening, no claim of verified until exact candidate qualifies. Published SVG or static draft is not runtime proof.

## 6. Stop conditions and reporting

STOP implementation if a controlling spec is DRAFT, a root/save ID cannot be safely mapped, an old saved world would be silently rebaselined, MFM ownership/coverage is ambiguous, Rack V2 allows irreversible invalid confirmation, no real spine entrance/turn is possible, other writers overlap, active playtest/Unity lock is unavailable, or full three-view capture is missing at feature qualification. Return partial evidence, state the precise blocker; do not force success.

Manager's eventual report distinguishes RESEARCHED, DRAFTED, APPROVED, IMPLEMENTED, INTEGRATED, VERIFIED and USER-ACCEPTED. Update canonical dashboard after substantial status changes; Drive mirrors update only if authoritative project-context sources change and a clean snapshot is regenerated. No Windows build before completed personal playtest and explicit user authorisation.
