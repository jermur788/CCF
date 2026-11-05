# Access Spine & Diagonal Rack Network — DRAFT Requirements and QA Checklist

**Status: DRAFT / STOP IMPLEMENTATION. Initial #286 NEEDS CHANGES resolved in independent #288 APPROVED research-quality re-review, with six non-blocking editorial corrections applied, 5 November 2026.** Checked documentation items denote completed source audit/recording only; no Unity/feature qualification is implied. Decisions remain open until accepted by user/Manager and written into approved spec/Decision Log.

## A. Authority and research quality

- [x] **AS-R01** — D-064/D-065 accepted rack/MFM rules preserved; D-055/D-056 historic site/ID policy and D-067–D-070 design intent cited. E/P/F, T-F/T-C/T-A and Option A remain unapproved.
- [x] **AS-R02** — #283 four DRAFT docs and source handoff published to isolated draft PR #285; no C#/Unity/saves changed by browser.
- [x] **AS-R03** — Current scene/source route, headland, RackPlan/entrance, Stand Map/minimap, MFM masks and save semantics inventoried and corrected evidence distinguished from runtime tests.
- [x] **AS-R04** — Corrected #248 45° best sampled **18.274 m** distinguished from actual #250 candidate same-side **16.623/17.510/17.855 m**; R1/R2 physical rectangle **0.214214 m gap** vs centreline 1.414 m failing editor **2.4 m**; ~120 impacted stems are not Fell instructions.
- [x] **AS-R05** — Independent #286 source review returned **NEEDS CHANGES B1–B7**, no approval of production, E or the spec.
- [x] **AS-R06** — Independent #288 source review returned **APPROVED FOR RESEARCH QUALITY** for B1–B7 with C1–C6 editorial items (applied to PR #285 docs); Manager disposition recorded separately. This **does not** approve the DRAFT spec or feature.
- [ ] **AS-R07** — User and Manager explicitly approve combined choices only after two separate axes, acceptable manual share, historic-save and geometric stop gates are considered.

## B. Starting infrastructure and topology — B1, B5

- [ ] **AS-R08** — Q1 selects E (entire central feeder already existed for *new* Enlarged80), P (proposed northern extension with actual route-work process), or F (current headland baseline), independently of rack layout.
- [ ] **AS-R09** — Q4 selects T-F headland parallel, T-C spine-entry single-family or T-A two-sided 3+2/45° reference. The spine counts as **zero** of five branch racks. No claimed viable 45° layout yet.
- [ ] **AS-R10** — Decision matrix shows experimental 20→9 m outside-cap shares: current F **0→3.11%**, T-A nearest owner **14.13→38.22%**, lateral owner **36.25→49.03%**. These are **unapproved reach/owner sensitivities**, not certified Covered/Manual counts; PR #274 is unmerged.
- [ ] **AS-R11** — Establish acceptable minimum actual Covered vs Manual/NotCovered area/stems (and sample credibility) through product decision before T-A selected. A provisional C-S45 spine candidate may have small modeled beyond-cap share but needs different entrance and spacing policy.
- [ ] **AS-R12** — E initial track is actually connected, physically usable on terrain, turning/exit access proven, start-tree exclusions and Track/Site/Headland/Clearing masked truthfully, and 3D/Stand Map/minimap status agree.
- [ ] **AS-R13** — P remains a nonmasking ghost with no tree marks/orders until real route work, cost and Work Plan approval/execution are specified; F does not falsely create a new northern road.
- [ ] **AS-R14** — Q6 selects northern original-tree-ID occupancy inference **I-N** or explicit saved initial-road field/model **I-V** with justified ambiguity vs JSON schema/hash cost. Test old Enlarged80 with north original trees, old felled north, naturally empty north, fresh E, Legacy40 and frozen reference; never retrofit E to an old saved world accidentally.
- [ ] **AS-R15** — New E spine masks/clearance apply to *new-world authored geometry*, not retroactive treatment changes for old-world saves. Model status must survive save/load/year, not depend on path plank name.

## C. D-055 seven sites, D-056 identities and D-070 workbench — B2, B3

- [x] **AS-R16** — All **seven** hidden buildables inventoried: Workbench, Saw Pit, Timber Sledge, Basic Shelter, Log Rack I, Log Rack II and Grinding Stone; four direct Workbench prerequisites and two indirect via Saw Pit; old missing reference can fail open.
- [x] **AS-R17** — Distinguish **actual #284** retained-scene-root/runtime policy flag with saved-ID reconciliation (source-reviewed; not Unity-qualified or merged) from unsafe proposed root deletion (shared Legacy40/Enlarged80 core).
- [ ] **AS-R18** — #287 Unity must explicitly prove **four** lifecycle assertions: (1) fresh game omits `workbench-01` from saved `buildables`, Site and clearance; (2) old save *containing* that ID restores it; (3) Reference Future preview enter→exit restores new-world retirement; (4) a new-game save round-trip stays retired. Negative case: **empty/pre-`buildables` old saves retire** because the ID is absent, not automatically restored. Align `Tools/Verification/StandGeometry/Enlarged80Verification.cs`'s old workbench-exclusion superset oracle and add **independent expected six-site inventory** to MFM-08A, whose gate currently shares the production retired predicate. No dangling prereqs, UI prompt or ghost collider; six other hidden sites remain without separate approval.
- [ ] **AS-R19** — Q2 decides whether to retain six other Site circles near the northern route terminus or separately retire them under new specification. Current #284 **does not** grant the latter.
- [ ] **AS-R20** — Regression inventories `ScenarioOneInteractionVerification`, `StandGeometry/Enlarged80Verification`, `MfmCoverageWorldUnityGate`, `CCFIntegrationVerificationTemp`, `MoveToReferenceView`, `ForestSceneBuilder`, `MixedSpeciesTest` before altering their site/source assumptions. A purported root `README.md` workbench reference remains unconfirmed by #288; locate an actual path before treating it as a consumer.
- [ ] **AS-R21** — Starting tree equality requires **exact 336 central tree IDs/positions**, 1,008 deterministic outer IDs, sorted total 1,344 IDs, DBH/neighbours and tree/mask hash; count alone is insufficient. #284 offline zero-freed-slot finding does not substitute for Unity ID-set verification.
- [ ] **AS-R22** — Protect Legacy40 start `EBE7A228F630AE41`, old Enlarged80 start `04A78188A6F8E6F9` and year 3 `081DB58CE3034648`, Reference Y100 `7AD177B3CC2F73C7` and continuation `9CDF21A541C5968D`. Establish P2/P3/Model2 fixture loaded-vs-regenerated provenance. New-world snapshots may require separately labelled hashes from intentional `buildables` delta; no old-anchor overwrite.

## D. Rack editing, clearance, MFM and work — B4, B6

- [ ] **AS-R23** — Q3 allows new spine/headland rack entries only if physically connected with a valid reachable first removal trunk; current headland-only validator remains controlling until reviewed D-064 amendment.
- [ ] **AS-R24** — Q4a explicitly chooses retaining ≥2.4 m **centreline** clearance or approving actual corridor rectangle separation plus justified machine-clearance bound. Nonintersecting rectangles do not prove machine access or 0.214 m workable gap.
- [ ] **AS-R25** — Five legal branch corridors checked for true length, finite width, boundary, headland/feeder, crossing, north turning, trunk collisions, suitable removal cost and suggested-spacing explanation; no automatic Fell marks.
- [ ] **AS-R26** — Option A **explicit eligibility predicate** names `A_intra`, `A_inter`, `A_chain`, `N_group`, bearing quantisation, finite input validation, unique IDs, exactly two families. Actual parameters OPEN. Legacy `Derive` is not a parallel-only test.
- [ ] **AS-R27** — Confirm-time **stable saved derivation and rule identity** (or proven equivalent) prevents 1 cm geometry/read/load boundary from switching legacy and A or changing source/unit identity; schema/migration/hashes explicitly qualified.
- [ ] **AS-R28** — Protect every old Rack V2 Confirm-PASS fixture (including legacy-MFM-invalid) unless user explicitly approves a D-064 behaviour change. Test structural invalid, strongly fanned legacy-valid (**expected disposition PENDING Q5**), eligible A, singleton/third-family/chaining, input order/endpoint flips, 1 cm boundary edit (**expected: stored derivation kind unchanged until explicitly reselected; precise policy oracle PENDING Q5**), mutation/reopen consent and executed work lock.
- [ ] **AS-R29** — Every cell/tree belongs to at most one owner or is explicitly Excluded/Manual/NotCovered; deterministic tie/finite-segment reach, masks first, user marks/work precedence, no duplicate prescription/order. Experimental 9/12/15/20 m caps remain unapproved.
- [ ] **AS-R30** — Define separate A sample anchor (legacy third rack of common normal does not apply): #253 measured **150.6 m²** inherited sample in other units, **143.0 m²** in R4 Centre. One cross-family demonstrated sample remains unqualified; test source overlap and fit.
- [ ] **AS-R31** — Confirm, authoritative confirmed edit, new prescription and work approval fail closed for *new governed topology*; a pre-existing confirmed-invalid plan loads, preserves executed history and suspends unsafe new work with explicit repair path.
- [ ] **AS-R32** — File ownership spans `ScenarioOneRackSampleGeometry`, `ScenarioOneCoverage`, `ScenarioOneManager.RackPlan/PrescriptionApproval`, `ForestSaveValidation`, `ScenarioOneRackBay.Frame`, `LearningObjectivesView.Sequence`, `StandMapTreatmentReview`, `StandMapAreaFootprints`, map source and minimap; assign one writer/API ownership before code and do not overlap AS-11/AS-12.
- [ ] **AS-R33** — Proposals never cause Fell marks, Track/Site/Headland exclusions, approved work or paid route construction; Confirm creates pending orders only, orange entrance Mark is separate, Work Plan Approval commits, Execution changes trees/history.

## E. Rendering, verification and release

- [ ] **AS-R34** — Existing / Proposed / Confirmed / Marked / Approved / Executed derived from one persisted status/geometry truth across 3D, Stand Map, minimap, Objectives and Work Plan; the #258/#282 teaching illustration is not real route implementation.
- [ ] **AS-R35** — Actual Unity 720p/1080p renders, native mouse/keyboard and 3D physical access confirmation; SVG/source-only audit cannot pass this gate. Off-path slowdown is gameplay calibration not accepted speed/engineering physics.
- [ ] **AS-R36** — Exact runtime candidate branch/HEAD, one writable lane per source file set, user playtest release, safe lock/mount/disk preflight and exclusive Unity lease verified. No forced release or shared dirty-work cleanup.
- [ ] **AS-R37** — Proportionate Tier 1/2 then milestone Tier 3 only if warranted; protect Legacy40/frozen reference, deterministic tree/WorldHash and actual completed-work histories, no weakened tests or fabricated PASS.
- [ ] **AS-R38** — Independent review and Manager integration verdict on exact candidate, GitHub Markdown handoff and dashboard sync. Keep IMPLEMENTED, INTEGRATED, VERIFIED and USER-ACCEPTED distinct.
- [ ] **AS-R39** — User personally completes Scenario One playtest and separately explicitly authorises any Windows build; no Windows/Player build under this DRAFT.

**Research boundary:** #283 documentation delivered, #286 original review NEEDS CHANGES, #288 B1–B7 re-review APPROVED FOR RESEARCH QUALITY with C1–C6 editorial fixes applied on docs branch. #284 is source-reviewed but not integrated/Unity qualified; #287 awaits user release/valid lease. Neither E/P/F nor T-A, Q4a, Q6 or any old-save migration has been approved.
