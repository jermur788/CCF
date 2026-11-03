# #251 — MFM topology / sample-coverage options: read-only specification analysis

**Status: READ-ONLY SPEC ANALYSIS / NOT IMPLEMENTED / REVIEWED AND PUBLISHED WITH CORRECTIONS.** No Unity, no production code, no save/schema change, no spec status change. Independent cross-provider review returned **MATERIAL ERROR** on the first draft; the four FAIL findings and eight corrections are applied below and recorded in Appendix C.
**Issue:** [#251](https://github.com/jermur788/CCF/issues/251). **Date:** 2026-11-03. **Author:** design worker (Claude Sonnet 5.5, Medium), under DPC; **independent reviewer:** GPT-6.1 (`gpt-6.1-sol`, medium effort).
**Candidate audited:** `origin/integration/scenario-one-player-test-ready-v1` = `72b4efbd1c653fa2231f632a79030a83ff76f833`. DPC resolved that ref with `git ls-remote` at 23:07Z; the worker read it as the local remote-tracking ref and did not contact the remote. All source was read with `git show 72b4efbd…:path` from the clean checkout `/media/jer/ZX20/CCF-Primary`; nothing was fetched, switched or modified there.
**Docs base:** `origin/main` `19245388`, branch `docs/mfm-topology-options-20261103-v1`, worktree `/media/jer/Files/CCF/Operations/CCF-MFM-TopologyDocs-20261103`. The worktree also carries the DPC append to `ORCHESTRATION-LEDGER.md` for this task; no other handoff was touched.
**Controlling documents:** Rack V2 (D-064) and MFM V1 (D-065) stay controlling. The four-file `specs/scenario-one-access-spine-route-consistency-v1/` is **DRAFT and is not authority** anywhere below.

**Evidence labels.** `OBSERVED FACT` = read from source, a spec/decision, or raw data in this session. `INFERENCE` = my reasoning from observed facts. `GAMEPLAY CALIBRATION` = a number or rule that is a game choice, not forestry fact. `UNSUPPORTED POLICY` = a rule I had to name to make a design complete that nobody has approved. `UNMEASURED` = not measured by anyone; renders are UNMEASURED in this programme. Numbers carry an `[Nxx]` tag that points to the Appendix table, which says where each came from and whether I re-extracted it.

## Summary (ten lines)

1. `Derive` refuses the 3+2 reference because its single common normal cannot order two groups of racks that are 90° apart. R4/R5 is only the first pair it reports; all three cross-group neighbour pairs overlap, and forcing the other axis also fails (§1).
2. The refusal is structural, not a rounding accident. It is not proof that every herringbone fails (§1.3).
3. The same refusal also takes out the Lessons 2–5 rack area, the prescription path and the Stand Map coverage footprints (§1.4).
4. **New finding:** nothing at Confirm, in the editor, or in save validation checks that `Derive` is valid, and `UpdateRack` can still reshape an unlocked rack of a confirmed plan. Invalid geometry can therefore reach confirmation and execution while MFM has no valid sample; completed rack work locks that rack's geometry, so repair may be blocked (§1.5, §9). No end-to-end player reproduction or proof of plan-wide permanent failure was performed. This is the most urgent item and is independent of which option is chosen.
5. **Option A** (side-local frames, added as a second derivation, `Derive` untouched) is the least expensive way evaluated to deliver 3+2 at 45°. A graph model (B) could also represent it at materially higher cost. A needs no new serialized field, but that does not by itself prove hash or semantic compatibility (§7). It needs several new decisions and an offline re-qualification before it could be called safe (§3A, §5).
6. **Option B** (graph) needs a persisted network that does not exist and would be a second spatial model. Not justified for five racks (§3B).
7. **Option C** (one parallel family) cannot hold 45°. Pure geometry needs about 90.5 m of spine for four 16 m gaps (five racks); the spine has 77.5 m. Roughly 55.7° or steeper is the geometric floor. Feasibility is not claimed (§3C).
8. **One demonstrated sample can validly inform both sides under the current comparability test**, because that test never looks at rack direction. That is a product decision, not a forestry finding, and it does not establish that the transfer is silviculturally sound. I recommend it, with a visible explanation (§3A.2).
9. No option is free of unresolved cost. None has an established save-corruption path, and none has demonstrated compatibility or calibrated coverage either. A is ready for a spec decision, not for build.
10. Recommendation: one spec decision, in §10.

---

## 1. The actual stop, restated from source

### 1.1 What `Derive` refuses

`ScenarioOneRackSampleGeometry.Derive` (`Assets/ForestPrototype/ScenarioOne/ScenarioOneRackSampleGeometry.cs`) runs these steps, in order:

| Step | Lines | Behaviour |
|---|---|---|
| Exactly five racks | 182–183 | else `"A rack-served sample needs exactly 5 confirmed racks (found N)."` |
| Ids positive, unique; finite endpoints; width and length > 0 | 188–199 | calling order never matters: racks are sorted by id first |
| Common axis | 201–216 | length-weighted mean of the **doubled** direction angle, so east/west of the same line are one direction. Refuses only if the resultant is below 1e-6 of total length (211–212) |
| Offsets | 221–239 | each rack's **midpoint** offset along the normal; sorted by offset rounded to the millimetre (OffsetResolutionPerM = 1000, line 30), ties by rack id (line 238) |
| **Refusal** | 241–246 | for each adjacent pair in that order: `High[k] > Low[k+1] + 1e-3` (CrossingToleranceM, line 31) ⇒ fail with `"Racks {a} and {b} cross or overlap across the common normal, so they have no defensible order."`. The loop returns at the first failing pair |
| Teaching rack | 260–271 | `measured[TeachingRackIndex]`, index 2 (line 28), i.e. the third in that order; must be ≥ 24 m (262–263); anchor = closest point to (0,0), clamped so ±12 m stays on the rack |
| Coverage strips | 273–298 | boundaries halfway between neighbouring midpoint offsets; the two outermost strips mirror the half-gap on the open side (280–281); two outer remainders are always reported |

`Low`/`High` are the minimum/maximum offsets of the rack's two endpoints (lines 225–231). A rack of length L whose direction deviates φ from the common axis therefore occupies L·sin φ of normal range. `MaxAxisDeviationDegrees` is only recorded after the refusal (lines 250–258), which is why the raw run shows 0 [N03]. #250 review item C10 said the same; it is a default, not evidence of parallelism.

### 1.2 Why the 3+2 reference fails (re-derived from the raw rack endpoints)

I re-implemented lines 201–246 in a scratch script (Appendix B) and fed it the junction/far endpoints from `design_candidate.json` [N10]. Orientation of entrance versus far does not matter (angles are doubled).

- All five racks are exactly 45° or 135° off the x-axis. West racks R2, R3, R5 are 135° and total 115.946 m; east racks R1, R4 are 45° and total 103.642 m [N20].
- Doubled angles of the two groups are opposite, so they cancel. The resultant is only 12.304 m out of 219.588 m (5.6%) [N21]. **The common axis is decided by the 12.3 m length difference.** Here it comes out at 135°, the west direction; Unity reported 135.0000153° [N03], my double-precision run 134.99999999999999° [N22].
- On the 135° normal the east racks run perpendicular to the axis, so each spans its full length. R1 covers offsets [−37.997, 18.481] and R4 covers [−50.214, −3.050], while R5's single midpoint (−17.066) lies inside R4's span [N23].

| Adjacent pair in sorted order | High[k] − Low[k+1] (m) | Result |
|---|---|---|
| R4 → R5 | +14.016 | **fail** (the pair the engine reports) |
| R5 → R1 | +20.931 | fail |
| R1 → R3 | +18.037 | fail |
| R3 → R2 | −16.623 | ok (this is the west same-side gap) |

[N23]. The engine never shows pairs after the first failure; the other two are my re-computation.

- **The other axis fails too.** Forcing 45° (counterfactual, not what `Derive` computes) gives R1→R4 −17.855 (ok), then R4→R2 +16.441, R2→R3 +37.644, R3→R5 +28.714 (all fail) [N24]. If the east total ever exceeded the west total the axis would flip to 45°, and the plan would still be refused, so the stop does not depend on which side wins.
- **Within each side the legacy test would pass.** Run on its own, west orders R5, R3, R2 with gaps 17.510 and 16.623, and east orders R1, R4 with gap 17.855. These equal the design JSON's same-side gaps 16.623 / 17.510 / 17.855 [N11, N25]. The two groups are individually valid parallel families and invalid only together.
- **It is not a physical crossing.** R4 and R5 centrelines are 18.087 m apart [N26]. The refusal is about overlapping projections on a single normal (as the Manager review and #250 review C10 already say).

### 1.3 What is NOT proven

- **Not proven:** that every herringbone fails. The failing condition is "adjacent midpoint-ordered racks' normal ranges overlap" (line 244). Short or near-parallel branches can pass. I did not map the boundary of the valid class and do not claim to.
- **Not proven:** that the mixed-direction class is rare in play. The editor lets a player draw non-parallel racks (§1.5).
- **Not proven:** anything about renders, on-foot entrance marking, turning room, or the user's playtest checkout (#250 §7).
- The #250 MFM mask table (156 newly masked, −111 stems, −459.82 m²) was run on the five **suggested east–west** racks as coverage geometry, not on the candidate, because `Derive` rejects the candidate [N12, N27]. The raw note says so. The numbers are mask sensitivity, not a herringbone coverage result.

### 1.4 What else the refusal switches off in production

| Path | Evidence |
|---|---|
| Prescription world read | `ScenarioOneManager.PrescriptionApproval.cs:261–266`: `Derive(ConfirmedRacks, half)`; invalid ⇒ `"The confirmed rack plan gives no valid sample: …"`, so `ReadCoverage`, `CreatePrescription`, `PreviewPrescription`, `ApprovePrescription` all refuse |
| Coverage classification | `ScenarioOneCoverage.cs:223–224` refuses an invalid geometry |
| Lessons 2–5 rack area | `UI/LearningObjectivesView.Sequence.cs:63–71` passes the derived sample to `ScenarioOneRackBayReader.Read`, which returns `null` when `!sample.Valid` (`ScenarioOneRackBayReader.cs:21`); the sequence facts at lines 94–98 then use the non-area expressions, so `Bay` is null and `area` is false. Whether Objectives 2–5 remain completable on such a plan is UNMEASURED |
| Stand Map coverage | `UI/Map/StandMapAreaFootprints.cs:36` `"rack/sample geometry is not valid"`; shown as `"Coverage cannot be shown: …"` (`UI/StandMapTreatmentReview.cs:194`) |
| Not affected | Map rack drawing reads `Manager.RackPlan` directly (`UI/Map/StandMapSource.cs:279`), so diagonal racks would still draw while their coverage is missing. Minimap rack drawing was not audited |

### 1.5 The refusal is not checked on every authoritative mutation (OBSERVED FACT, new)

I `git grep`-ed every use of `ScenarioOneRackSampleGeometry` under `Assets/ForestPrototype` (callers: prescription world read, lessons sequence, Stand Map footprints and treatment review; none in rack authoring or Confirm) and read the authoring and save paths:

- `ConfirmRackPlan` (`ScenarioOneManager.RackPlan.cs:239–343`) checks five racks, per-rack geometry (finite, length, inside stand: 462–485), per-rack entrance tree, and tree-claim exclusivity (285–293). It does **not** call `Derive`.
- `UpdateRack` (`RackPlan.cs:145–193`) accepts an **unlocked** rack of a **confirmed** plan, rewrites its geometry and reconciles its orders **without clearing `rackPlanConfirmed`**. The map UI normally requires reopening a confirmed plan first, but the manager API itself does not. A Confirm-only check would therefore not cover every authoritative mutation.
- `RackEditing.Validate` (`Racks/RackEditing.cs:149–181`) checks length ≥ 5 m, inside stand, entrance on headland, duplicates and overlap. It checks no direction relationship. Parallel snapping is a soft 4° pull (`AngleSnapDegrees`, line 104; spec §13 "enough freedom to deliberately deviate").
- `ValidateRackV2` (`ForestSaveValidation.RackV2.cs:58–61, 81–85`) checks count ≤ 5, confirmed ⇒ exactly 5, finite, non-degenerate. No direction or ordering check.
- A rack with one completed order is locked: `IsLocked => Completed > 0` (`Racks/RackGeometry.cs:356`); `UpdateRack` and `DeleteRack` refuse locked racks (`RackPlan.cs:155–156`, `205–206`). `ReopenRackPlan` changes nothing about locked racks.
- Rack V2 flow executes the rack work **before** the sample lessons (MFM spec §6 steps 1–5; Rack V2 D-064 sequence).

**INFERENCE:** a player can confirm a mixed-direction plan, approve and execute it, and only afterwards reach Lesson 2, at which point every executed rack's geometry is locked and cannot be moved or deleted. I have not demonstrated this in a running game, and I have **not** shown that the whole plan becomes unrepairable: if only some work is executed, the remaining racks may still be editable, and whether a repairable remainder exists depends on the specific plan. This is a hazard for any plan `Derive` rejects, not only the herringbone, and it already exists in the shipped candidate for anyone who fans racks. The specific 3+2 reference also fails current entrance and overlap rules (§9), so it could not be confirmed as drawn today.

**Lifecycle policy this exposes (needs a Manager decision, not a code detail):** validity should be checked at Confirm **and** at every authoritative operation that changes a confirmed plan — or those operations should be forced through a reopen that suspends the work until reconfirmation. Separately, behaviour for an already-loaded confirmed plan whose geometry is invalid must be defined: fail closed, show the exact reason, and allow repair without erasing executed history.

---

## 2. Current model mechanics (what exists today)

### 2.1 Placement precedence (`Result.Classify`, lines 131–151)

One ground point maps to exactly one placement, in this order: outside the stand wins over everything (133); then the **first** rack in ordered-rack order whose corridor contains the point (135–140); then outer remainders (142–143); then the coverage-unit strip (144–149). A corridor wins over the strip it lies in. Coverage then adds masks before the sample rectangle and before the segment split (`ScenarioOneCoverage.cs:441–456`), so a masked tree is never a sample or target stem.

### 2.2 Teaching sample

- `Frame.Anchored(anchor, along, halfWidth 9 m, corridors)` (`Derive` line 271; `ScenarioOneRackBay.cs:159–160`). Half-length 12 m, half-width 9 m [N40]. The frame carries **all five** corridors, and `Contains` excludes them (`RackBay.cs:164–173`), so access ground is excluded wherever it lies.
- The only way the global ordering enters the sample is *which rack is third*. The rectangle itself is built from that one rack's own direction.
- The prescription rule derived from the sample is two scalars: least executed release share and removed basal share (`ScenarioOnePrescription.cs:112`, `ReleaseRule.Create`). It contains no direction.

### 2.3 Coverage (`ScenarioOneCoverage.Classify`, lines 216–357)

- Three segments per unit, Low / Centre / High, split along the **common axis** at the teaching anchor ± 12 m (`Locate`, lines 454–456). The footprint code does the same (`StandMapAreaFootprints.cs:37–39, 53–56`). This is the second place the single global axis is baked in.
- Source = the teaching rack's Centre segment (lines 256–261); the sample is the fixed rectangle, not the whole segment.
- State per area, in precedence: Source ⇒ Manual; player-chosen manual ⇒ Manual; any exclusion reason ⇒ Excluded; fewer than 30 stems ⇒ NotCovered/`TooFewStems`; else compare density (20%), mean DBH (15%), mean competition (20%) ⇒ Covered or NotCovered (lines 337–346, 360–380; calibration `ScenarioOnePrescriptionCalibration.cs:73–75`) [N41].
- Exclusion reasons are per tree or feature: player Fell mark, open order, completed work, non-Sitka adult, planted juvenile, shelter, clearance patch, protected area, deadwood record, active management work (lines 286–304). **No reason mentions rack direction, side or geometry.**
- The two outer remainders are always `Manual` and never compared (155–165). A tree is either in a corridor, a remainder, masked, in the sample, or in exactly one area (`Spot`, lines 417, 435–458).
- Net area is integrated **numerically along the axis**: exact interval cuts across each 0.1 m strip (`AreaStepM`), with each strip classified once at its midpoint (`Coverage.cs:460–530`). Each shape must be convex along a scan line for the interval cut to hold. A new boundary that curves needs explicit crossing handling and convergence qualification; curvature is not by itself a proof that the method cannot be used.

### 2.4 States and the Work Plan gate (all existing, unchanged by this study)

| Concept | Where it lives |
|---|---|
| Draft rack (Proposed) | `RackSaveData` in a draft plan; `ConfirmedRacks` is empty until confirmed (`RackPlan.cs:50–51`) |
| Confirmed | `rackPlanConfirmed`; Confirm freezes membership and creates/refreshes **Pending** orders only (239–343) |
| EntranceMarked | `entranceMarked` on the rack; separate from Fell/Crop marks (`ForestSaveValidation.RackV2.cs:107–120`) |
| WorkPlanApproved | ordinary order status; prescription approval makes Pending orders only (`PrescriptionApproval.cs:142–222`) |
| Executed / locked | `Completed > 0` ⇒ geometry and membership locked (above) |
| Prescription proposal | never stored, recomputed each call (`PrescriptionApproval.cs:11–12, 92–101`) |

Confirmed-but-unexecuted racks already exclude their corridor ground from coverage today (corridors come from `ConfirmedRacks`). That is existing accepted behaviour (D-064: confirmed is authoritative); the "proposal must not mask" rule in #250 §4 concerns unbuilt spine/overlay routes, which are not rack records.

### 2.5 Persistence: stored versus recomputed

| Stored (save v24, `ForestSaveData.cs:7`) | Recomputed every call, no cache |
|---|---|
| `RackSaveData`: id, entrance/far, frozen removal ids, entrance tree, mark flag (`ForestSaveData.cs:124–146`) | Axis, offsets, order, teaching rack, anchor, strips, remainders, segments (`Derive`) |
| `PrescriptionSaveData`: rule version text, calibration version text, `sourceRackId`, source anchor, sample crop/removal ids, coverage decisions by **unit id string**, approval state/year (`ForestSaveData.cs:274–343`) | Coverage states, areas, proposals, fit |
| Work orders carry `prescriptionCoverageUnitId` | Footprint polygons |

Save validation checks prescription fields for form and internal consistency only, never against geometry: rule/calibration versions must be `[A-Za-z0-9._-]`, ≤ 64 chars; unit ids printable ASCII ≤ 64; `sourceRackId` must be an id the allocator has handed out (`ForestSaveValidation.cs:538, 555, 609–628`). A stored prescription is re-validated against the live derivation on every use: rule version must equal the current constant (`ScenarioOneManager.PrescriptionApproval.cs:152`), and teaching rack id and anchor (±1e-3) must match (`:165–168`) else `SampleChanged`; an accepted unit that is no longer covered is surfaced as a conflict (`:215–220`); a target not in the unit is refused (`:282`). All of these **fail closed**.

### 2.6 Frozen boundaries

`RackPlanningV2Active` requires model 1 **and** Enlarged80 (`RackPlan.cs:37–40`), and prescription reads require it (`PrescriptionApproval.cs:255`). Reference Future v1 requires Legacy40 and save version 12 (`ScenarioReferenceArchive.cs:108–110, 115–116, 126`). World identity hashes strip default-valued later fields (`:224–259`), so **any new serialized field would change the hash layout for worlds that carry it** and needs an explicit strip rule. A change that adds no field avoids a *layout* change; it does not by itself prove that serialized **values** stay identical, because the hash covers the stored prescription state and version text as well (§7).

---

## 3. The three options

All three are evaluated on the 3+2 reference (§1.2) and on the stem data in `ScenarioOneAccessSpineMeasure…/measurements/trees.json` (1344 living Sitka, no stumps [N42]). Those stems are the #248 fresh Enlarged80 world; #250 states the same count. That they are the same world instance is INFERENCE.

### Option A — branch/side-local frames

**A.1 Mechanism (smallest form I could make complete).** Add a *second* derivation next to `Derive`. `Derive` is never edited. The new one runs only when `Derive` is invalid for a reason that grouping can cure.

1. **Group** racks by undirected direction with a tolerance (single-link on the doubled angle). Groups are named by their smallest rack id. At most two groups for Scenario One.
2. **Per group**, run exactly the existing ordering and strip logic (lines 201–246, 273–298) on that group's own axis and normal. Each group must be valid on its own.
3. **Own every ground point exactly once.** A point inside a strip of both groups goes to the group whose own rack centreline is *laterally* closer; exact ties go to the group holding the lowest rack id. A point inside a strip of only one group (the other group's outer remainder) goes to that one group. A point outside both groups' outer strips is an outer remainder (always Manual).
4. **Teaching rack:** the rack at index ⌊n/2⌋ of the largest group, and must be ≥ 24 m. For a single group of five this is index 2, the same as today.
5. **Segments:** Low / Centre / High along the *owning group's* axis, split at the projection of the teaching anchor onto that axis ± 12 m.
6. Corridors win over strips and tree ownership stays one-rack-per-tree, both as today.

*Why lateral distance and not nearest rack segment.* Both are implementable. Nearest-finite-segment (Voronoi) ownership adds curved boundaries wherever a point-end meets a line; the existing interval cutter can be extended to them, but each new edge type needs its crossings found and its convergence qualified before the areas can be trusted. Lateral distance to infinite centrelines adds only straight boundaries, which the existing cutter already handles. That is a lead-time and qualification-cost argument, INFERENCE from `Coverage.cs:460–465`, not a claim that nearest-segment measurement is impossible.

**A.2 The central pedagogy question — can one demonstrated 3D sample inform both sides?**

*Direct answer: yes under the current comparability definition; whether it should is a product decision.*

OBSERVED FACT supporting "yes":
- Comparability compares three scalars only: density, mean DBH, mean competition (`Compare`, lines 360–380). The `Reason` vocabulary (100–109) has no direction, side or geometry reason. Rack direction is not a comparability variable today.
- The derived release rule is two scalars (§2.2). Proposals are built around the player's own confirmed Crop Trees within the 8 m competition neighbourhood and contain no distance-to-rack or direction term (`ScenarioOnePrescription.cs`: no `distance`/`corridor` hit).
- MFM spec §6 step 6 already lists "another sample where needed" as an optional manual path; §10.4 says a unit that differs is `Not covered`, never silently covered.

OBSERVED FACT against "a second sample is cheap":
- The code supports exactly **one** source sample per confirmed plan: every prescription is re-checked against the single teaching rack (`ScenarioOneManager.PrescriptionApproval.cs:165–168`). A second source would need multi-source support, a teaching-rack selector and a curriculum change to Lessons 2–5. That is a feature, not a geometry fix.

INFERENCE and what it does to the player's demonstrated judgement:
- **Manual demonstrations: one**, unchanged from today. The player judges inside one 24 × 18 m footprint beside R3, and the same judgement is *tested* against east-group areas by the unchanged three-metric rule. Where an east area differs, it becomes `NotCovered` and the player manages it manually or demonstrates again; no area is made Covered by a side rule.
- The pedagogical risk is that "the sample stands for the east branches" is a claim about **conditions**, which MFM spec §10.1 says is the right basis. The model does not check conditions that differ *because of* orientation (for example how a 45° rack crosses the planting rows). Whether such a difference matters is UNMEASURED and I will not assert it.
- Required honesty rule (GAMEPLAY CALIBRATION): every east-group area explanation must say it is compared by stand conditions to a sample in the west group, name the group, and not imply the sample was taken beside an east rack.
- If qualification later shows poor transfer, the fallback is the optional second sample the spec already names, accepting the feature cost above. I do not recommend building it now.

*Worked sample on the reference.* The rule above picks R3 (west group, middle of R5, R3, R2). Its anchor is clamped: the closest point to the origin is 4.647 m along, clamped to 12 m, giving (−5.513, 4.885) [N50]. The footprint therefore starts at the spine junction and lies about 6.70 m from the Forest Clearing centre [N50, N38]. Before masks it holds 68 stems outside corridors and 358.25 m² net, against the suggested-layout sample's 75 stems and 314.005 m² after masks [N50, N35]. Masks cannot be applied because the Track/Clearing/Site plank geometry is not in these files. R5 (15.061 m [N10]) cannot be a teaching rack: it is under the 24 m limit (line 262).

**A.3 Worked behaviour on the 3+2 reference (unmasked, my computation).**

| Quantity | Value |
|---|---|
| Stand area / union of five corridors / non-corridor | 6400.0 / 524.25 / 5875.75 m² [N51] |
| **Naive per-side strips, no owner rule:** area claimed by both a west and an east strip | **1575.0 m² (26.8% of non-corridor)**, i.e. 311 of 1241 non-corridor stems [N52] |
| Same, claimed by west only / east only / neither | 2472.5 / 1413.75 / 414.5 m²; stems 551 / 294 / 85 [N52] |
| **With the lateral-distance owner rule:** owned units + outer remainder + corridors | 5461.25 + 414.5 + 524.25 = 6400.0 m²; stems 1156 + 85 + 103 = 1344 [N53] |
| Smallest unit by unmasked stems | R1C, 31 stems against the 30-stem minimum; every other of the 15 units has at least 39 [N53] |
| Units under 30 unmasked stems | none; masks will lower these, so TooFewStems is plausible for R1C (UNMEASURED) |

So a naive "give each side its own frame" **double counts a quarter of the stand**. A is only safe with an explicit single-owner rule, and the conservation sums above (area and stems both close exactly) are the property the tests in §8 must hold.

*Shared spine.* The spine is not a rack record. Today it enters coverage only as Track masks from `Dirt Path*` planks (`PrescriptionApproval.cs:338–340`, `CoverageWorld.cs:73–74`). A leaves that unchanged; nothing in A needs the spine as data.

*Edge cells.* Strip membership uses `offset >= lower && offset < upper` with the last strip closed (`Classify` 147), unchanged per group. Ownership ties use a 1e-3 m band (the same constant as `CrossingToleranceM`) [Q02].

*Excluded masks.* Masks and corridors are removed identically from sample and every area (`Coverage.cs:450–453, 466–530`). Masks come from world objects, not rack geometry (`CoverageWorld.cs:7–12`), so A inherits them without change.

*Competing groups.* Resolved by the owner rule. Both groups' outer remainders are explicit and Manual, so no region is silently given to the nearest rack. New remainder ids are needed (legacy `REM-Right`/`REM-Left` are one pair) [§5.3].

**A.4 What A preserves and breaks.**
- Preserves: `Derive` bytes; every plan `Derive` accepts (the new path never runs for them); unit-id syntax `R{id}{L|C|H}`; the Work Plan gate; corridor/tree exclusivity; masks; Reference Future v1/Legacy40 (not reachable).
- Breaks / needs decision: the spec text "third rack … common normal" (MFM §4.1) for branched plans; calibration transfer (see §9); the single global axis in `Locate` and the footprint code, which must become per-group.
- **Unresolved cost:** none of the thresholds that make A complete is approved: the grouping tolerance, the 2-group cap, the owner rule, the teaching-rack rule, segment anchoring for the non-teaching group, and whether a reach bound is wanted (A.5). Each is `UNSUPPORTED POLICY` until a Manager/user decision.

**A.5 The reach question A exposes (not caused by A).** The legacy model has no distance-to-rack criterion anywhere in coverage or proposals (§A.2). It never needed one: on the suggested east–west layout the sampled maximum distance from any non-corridor point to a rack centreline is 10.43 m (exact ≈10.75 m at the west corners beyond the line ends), and only 169.5 m² is farther than 9 m [N54]. On the reference, 2246.0 m² (38.2%) is farther than 9 m, 1718.25 m² farther than 12 m, and 830.0 m² farther than 20 m, which lies in the south (z from −39.75 to −10.25), with a maximum of 41.18 m [N55]. By stems: 480 / 365 / 176 of 1241 [N55]. Any rule that assigns that ground to a rack's unit stretches "rack-served" far beyond anything the model was calibrated on. No bound is approved. I propose none in the first cut but that the explanation shows distance to the owning rack; the Manager should decide (§10).

### Option B — graph / topology-aware coverage

**B.1 Two different things hide under this name.**
- **B1: stable areas from a persisted network** (spine, junctions, rack edges). The spine is not a record today: it exists only as `Dirt Path*` planks that feed Track masks. A graph needs a persisted route/junction entity, a new save field and version (v25), a hash strip rule for default-valued worlds (§2.6), load validation, an editor for it, and a rule linking route masks to proposals that D-068 says must not be a cosmetic patch. It is a second authoritative spatial model beside the rack plan.
- **B2: a verified spatial partition derived only from racks** (for example nearest finite segment). It needs no persistence, but its boundaries are curved, so the interval cutter needs extra crossing handling and convergence qualification (§2.3); that is cost, not impossibility. It also becomes A's owner rule in a different geometry.

**B.2 Behaviour on the reference.** With B2 and no distance bound, the five owners would hold R1 2242.2 m², R2 1409.8, R3 864.2, R4 855.8, R5 503.8 m² (sum 5875.8, rounding; I used a 0.5 m raster, not the exact method) and 475 / 274 / 181 / 194 / 117 stems [N56]. R5, 15.061 m long, owns 117 stems but nearly all of them are far from it: that is the same reach question as A.5. B1 has no worked geometry because nobody has defined the graph.

**B.3 Determinism, explainability, future.** B1 can be deterministic if the network is persisted canonically, but "which face is this tree in" needs a planar-graph rule that has not been designed. Its future-proofing is the best of the three (complex stands, property level) and it is exactly what MFM V1 says not to build now (§18 non-goals: property-level multi-stand gameplay). The risk named in the packet, becoming a second authoritative spatial model, is real for B1.

### Option C — constrained parallel-rack fallback

**C.1 Mechanism.** `Derive` and the coverage code stay unchanged. The permitted topology is one ordered family: all five racks parallel in one direction, plus an access spine.

**C.2 What the user loses.** (a) The 45° 3W+2E herringbone, which is the D-068 reference target. (b) By construction, mirrored V-shaped branching: on a spine, one family means the east branches point the opposite way along the spine from the west branches (a single set of parallel lines crossing the spine). (c) Freedom to place a poor or fanned proposal: it would have to be refused or warned, not merely drawn (spec §13 currently promises "enough freedom to deliberately deviate").

**C.3 Is there a feasible layout? Not established.** What I can prove is a floor from pure geometry (INFERENCE, straight-spine approximation):

- Five parallel lines meeting a straight spine need four gaps. With junctions on the spine separated by Δz, the perpendicular gap is Δz·sin θ (θ = angle off the spine).
- For gaps of at least 16 m, the junctions need at least 64/sin θ of spine. At 45° that is 90.51 m (113.14 m for 20 m gaps). The spine's z-extent is 77.489 m (−39.989 to 37.5) [N60].
- **A single parallel family therefore cannot hold 45° with five racks on this spine.** The floor is θ ≥ asin(64/77.489) = 55.68° [N60], and that uses every metre of spine with a junction at each extreme.

The 3+2 herringbone works precisely because it is two families that interleave offsets. Entrance legality (headland-only), the yard conflict, tree impact, corridor overlap and turning room were not measured for any one-family layout. C is not shown to be feasible.

**C.4 Variants.** *C0* (status quo: suggested east–west racks from the headland, spine is path only) is safe and measured (baseline 12 of 14 covered [N30]) but delivers none of D-067. *C1* (one family attached to the spine at about 56° or steeper) still needs the entrance-on-spine amendment and the yard decision (§6), so it saves MFM work only.

---

## 4. Options matrix

| Criterion | A: side-local frames (additive) | B: graph (B1) / partition (B2) | C: one parallel family |
|---|---|---|---|
| Determinism | Good if the owner rule, tie band and group naming are fixed (§5). Reduces to today's output for parallel plans | B1 needs a canonical persisted graph and a face rule not yet designed. B2 deterministic; curved edges need extra crossing handling | Unchanged and proven by existing gates |
| Sample pedagogy | One sample compared by the unchanged metrics; needs explicit explanation that it is in the other group | Same as A for the sample; areas explained by graph faces, harder to show | Unchanged and simplest to teach |
| Manual demonstrations | 1 (second sample is a separate feature, not built) | 1 | 1 |
| Unit / area remainders | Explicit single-owner units plus outer remainders; reach unbounded by default (A.5) | B1: new remainder concept per face. B2: as A | Unchanged strips; remainders as today |
| Mask exclusions | Inherited unchanged; masks do not depend on rack geometry | Inherited, but a persisted route invites a route-to-mask rule D-068 forbids as a cosmetic patch | Unchanged |
| Performance | Roughly two scan-line passes; per call, no cache today. UNMEASURED | B1 unknown; B2 raster or exact-arc work heavier. UNMEASURED | Unchanged |
| Risk | Medium: new rules, calibration non-transfer, UI changes | High: new save field and authoritative model | Low technically; product risk is the lost request |
| Save evolution | **None needed** (rule/calibration versions are existing text fields; unit ids unchanged) §7 | v25, hash strip rule, validation | None |
| Testing cost | Medium: offline matrix §8, a calibration re-qualification, one bounded Unity run | High: graph invariants plus the above | Low: existing gates plus a spacing check |
| Map / UI interpretation | Needs per-group segment axes and a group legend | Needs a face/network legend | No change |
| Future-proofing | Handles two groups; three or more groups would need a policy change | Best | Poorest: forbids branching |
| Simplest reliable implementation | **Yes among the three that deliver 45°** | No | Yes, but does not deliver 45° |

**Plain verdict.** A is the simplest of the three that could deliver the user's 3+2 request. B is disproportionate to a five-rack scenario. C is the safest but cannot hold 45° on this spine. None of the three is approved by this study: A has not demonstrated safe coverage, compatible semantics or calibration transfer, and B could also represent 45° branches at higher cost.

**Does every option have an unresolved unacceptable cost?** **No.** C has no unresolved safety cost; its cost is that it fails the stated request (§3C.3). A's unresolved items are decisions plus a re-qualification; none corrupts saves or changes accepted semantics for existing plans. B1's cost is unacceptable at this scope. One cost is common to all and is not optional: the unguarded lock hazard in §1.5.

---

## 5. Minimum viable interface / contract (specification only; no C#)

### 5.1 Inputs

A confirmed plan: exactly five rack records (`RackPlanRack`: id, entrance, far, width), the stand half-extent, and the same teaching constants as today. No new persisted input. Candidate parameters (all `UNSUPPORTED POLICY` until approved, labelled with their provenance in Appendix F):

- `GroupToleranceDeg` — suggested to reuse the editor's 4° snap window [Q01]. It is a snap, not a grouping rule, so reuse is a convenience, not evidence.
- `OwnerTieBandM` — reuse 1e-3 m [Q02].
- `MaxGroups` — 2 [Q03].

Single-link clustering has a chaining failure mode: two racks at the extremes of a group may exceed the tolerance while every adjacent pair is inside it. The rule must therefore either cap the group's extreme-to-extreme spread as well as the neighbour step, or declare chaining acceptable and cover it with a test. This is a decision [Q05], not a free implementation detail.

### 5.2 Outputs

The existing `Result` shape, extended: per group (axis, normal, ordered racks, strips), per unit the owning group, per remainder its group and side, a `TeachingGroup`, and `Signature()` that lists group membership first. Existing consumers keep reading `Racks`, `Units`, `Teaching`, `Anchor`; `Classify` keeps its precedence and gains the owner rule. Segment cuts become `(axis_g, anchorAlong_g)` per unit instead of one `(Axis, anchorAlong)`.

### 5.3 Validity rules and stable error strings

| Condition | Result | Text |
|---|---|---|
| Not five racks | invalid | existing, unchanged: `A rack-served sample needs exactly 5 confirmed racks (found N).` |
| `Derive` valid | use it unchanged | none (the new path is never entered) |
| `Derive` invalid and the cause is not groupable (non-finite, duplicate id, zero length) | invalid | the existing string from `Derive` |
| More than `MaxGroups` groups | invalid | proposal: `The racks point in {n} different directions; at most 2 are supported.` |
| A group fails its own ordering | invalid | proposal: `Group {g}: Racks {a} and {b} cross or overlap across the group normal, so they have no defensible order.` |
| Teaching rack shorter than 24 m | invalid | existing wording, unchanged |
| Otherwise | valid | none |

Where the check runs (a lifecycle rule, not one call site):

| Moment | Required behaviour |
|---|---|
| Confirm | refuse a plan neither derivation can read (recommended) |
| Any authoritative mutation of a confirmed plan (`UpdateRack`, `DeleteRack`, add/remove) | either validate the resulting plan before applying it, or force a reopen that suspends reliance on the sample until reconfirmation |
| Load of an already-confirmed invalid plan | fail closed with the intended reason; permit repair without erasing executed history; do not silently re-derive a different sample |

An invalid result is never repaired and never replaced by a fallback layout, as `ScenarioOneCoverage.Classify` already requires (`Coverage.cs:83, 223–224`). The visible message is the same text path as today (`"The confirmed rack plan gives no valid sample: …"`).

### 5.4 Pseudocode (informative)

```
DeriveBranched(racks, half):
    legacy = Derive(racks, half)
    if legacy.Valid: return legacy                    # parallel plans never change
    if not groupable(legacy): return legacy           # keep the existing error
    sort racks by id
    groups = single_link_cluster(doubled direction angle, GroupToleranceDeg)
    name each group by its smallest rack id
    if count(groups) > MaxGroups: return Fail(too many directions)
    for g in groups:                                  # existing steps, per group
        axis_g, normal_g = length_weighted_doubled_mean(g)
        order g by midpoint offset (mm rounding, tie by id)
        if any adjacent pair has High[k] > Low[k+1] + 1e-3: return Fail(group g)
        strips_g = half-distance, mirrored outer edges
    teachingGroup = largest group (tie: group with smallest rack id)
    teaching = teachingGroup.ordered[floor(n/2)]      # n=5 -> index 2, same as today
    if length(teaching) < 24: return Fail(teaching too short)
    anchor, frame = as today on the teaching rack, with ALL five corridors
    Classify(p):
        outside stand -> OutsideStand
        first corridor by rack id -> RackCorridor
        for each group: strip index k_g and lateral = |offset_g(p) - offset_g(rack k)|, or none
        both none -> OuterRemainder(of the group whose outer edge is nearer by offset)
        one -> that group's unit
        both -> lower lateral wins; |difference| <= tie band -> group with smaller group id
        segment = Low/Centre/High by (along_g(p) - along_g(anchor)) at +/-12
```

The exact owner for a both-none point (which group's remainder it joins) is a detail that must be decided; any fixed rule works so long as it is total, and the point is reported once.

### 5.5 Rules that must hold regardless

1. Single owner: each ground point maps to exactly one of OutsideStand, one corridor, one unit, one remainder. Each stem has exactly one of those results.
2. Conservation: unit areas + remainder areas + corridor union = stand area, and stems sum to the world total (the reference above closes to 6400.0 m² and 1344 stems [N53]).
3. Input order never changes the answer (existing shuffle gate).
4. A plan that `Derive` already accepts produces a byte-identical `Signature()` through the new entry point.

---

## 6. Spec clauses requiring amendment

The draft access-spine spec is **not** authority; every row names an approved spec.

| Clause | Conflict | Minimal amendment | Why |
|---|---|---|---|
| MFM V1 §4.1 | Teaching rack defined as "third … ordered by perpendicular offset along the plan's common normal"; plan.md:45 and tasks.md MFM-02 repeat it | Add: for plans with more than one direction group, the teaching rack is the middle rack of the largest group; third-by-common-normal remains the rule for single-group plans | Without it, no teaching rack is defined for the reference |
| MFM V1 §4.2 | Strips "halfway toward neighbouring racks"; edge treatment left to implementation; fixed 9 m must not become the boundary | State the cross-group owner rule and per-group outer remainders; state that the 9 m teaching width stays unchanged | A new partition rule is a spec-level choice, not an implementation detail |
| MFM V1 §10.3, §10.4 | Four states and exclusion list assume one rack-served partition | Add: units are labelled by group; remainders per group are Manual | Keeps the four-state vocabulary and its explanation duty |
| MFM V1 §7.4, §14 | Determinism and "new rule version strategy" for algorithm changes | Add a rule version for branched geometry stored in the existing `ruleVersion` text, and a calibration version if tolerances change | Uses the versioning §7.4 already requires; no schema change |
| MFM V1 §6 step 6 | "another sample where needed" is optional and unspecified | Decide: one sample serves both groups (recommended) or a second teaching sample is required | The central pedagogy decision (A.2) |
| MFM V1 §18 non-goals | "changing Rack V2 geometry/editing rules" is a non-goal | Keep it. Do **not** solve this in MFM; the Confirm gate and entrance rules belong to Rack V2 | Respects ownership |
| MFM V1 §21 AC 2, 5, 13, 15, 16, 17 and §22 stop 1 | §22 stop "Rack V2 geometry/persistence cannot provide stable rack-served strips" has literally fired for this plan | Record the stop as fired and cured only by the amendment above | Spec-conformant route out of the stop |
| Rack V2 §13 | Helps the player make "practical, parallel rack layouts" and requires "enough freedom to deliberately deviate" | Add a plan-validity consequence: free deviation is allowed, but a plan that neither derivation can read cannot be confirmed (or confirms only with explicit consent) | Closes the unguarded-mutation hazard (§1.5) |
| Rack V2 §16 | Plan validity lists no MFM requirement; "show why a plan needs adjustment" | Add: the geometry the MFM sample needs is a validity condition, with the §5.3 message | Same |
| Rack V2 §17, §18 | Confirm and reopen say nothing about derivation validity; `UpdateRack` can reshape a confirmed rack directly | Confirm refuses (or warns) on invalid derivation; any authoritative mutation of a confirmed plan re-checks or reopens; reopen itself unchanged | Same |
| Rack V2 §6 | 16–20 m band and "STOP — PRODUCT/GEOMETRY CONFLICT" | Say which distance the band applies to between non-parallel groups (the reference's gaps are same-side only) | Cross-group spacing is not defined |
| Rack V2 §11, §16, §23 and code `RackEditing.EntranceOnHeadland` (`RackEditing.cs:123–125, 162–163`) | Entrances headland-only in code; spec says "connect validly with the path/headland" | Decide whether a spine junction is an allowed anchor and how the entrance tree is chosen there | Needed by every spine layout, in all three options |
| Rack V2 §16 and `RackEditing.Validate` overlap rule (`RackEditing.cs:102, 176–177`) | Racks closer than 2.4 m are refused; R1/R2 centrelines are 1.414 m apart [N13] | Define an allowed overlap near a shared spine junction, or re-place R2 | Corridor-overlap conflict from #250 |
| Rack V2 §34, §37, §39.4, §39.6.2 | Persistence additive and old-save safe | No layout change if A adds no field; stored values and round-trip hashes still require qualification (§7); if B1 is chosen, a new §34 field set and hash rule | Option-dependent |

---

## 7. Save / compatibility analysis

**Persisted today versus recomputed:** see §2.5.

**Option A, minimal form: no serialized field is added and the save version stays 24** (`ForestSaveData.CurrentVersion = 24`). A rule version string such as a branched variant of `MFM-V1-RULE-1`, and a calibration version string if tolerances change, are existing free-text fields whose validator accepts letters, digits, `.`, `_`, `-` up to 64 characters (`ForestSaveValidation.cs:620–628`). Unit ids keep their syntax.

**This avoids a schema-layout change. It does not by itself prove hash or semantic stability.** `ScenarioReferenceArchive.CurrentWorldHash` hashes the serialized **values**, and `HashLayout` branches on the preserved prescription state (`ScenarioReferenceArchive.cs:224–259`); a changed version string or a changed stored decision is a changed value even when the field set is identical. Stable area names (`R{rackId}{L|C|H}`) preserve *references*, not *meaning*: the same id can denote a different polygon once the partition changes. The compatibility claim must therefore be qualified, not asserted:

1. For every plan `Derive` already accepts, the branched path never runs, and every stored value that a legacy save carries must be reproduced unchanged — derive, never rewrite.
2. A prescription recorded under one rule version must keep that interpretation; the expected version must be chosen from the derivation the live plan actually uses, not from a new global constant.
3. Round-trip hashes, coverage-decision resolution and work-order provenance must be qualified on a real save matrix, not inferred from "no new field".

Consequences to design on purpose (OBSERVED FACT about the checks, INFERENCE about their effect):
- A prescription made under the legacy rule version, then a plan reshaped to branched geometry (or the reverse), is refused with `RuleVersionMismatch` (`ScenarioOneManager.PrescriptionApproval.cs:152–153`); source rack id and anchor mismatches give `SampleChanged`. No order is created. This is the intended fail-closed behaviour.
- The rule-version constant is currently global. To avoid stranding existing accepted prescriptions, the *expected* version must depend on which derivation the live plan uses. That choice is the main place a code change could affect how an old save is read, and it must be covered by a test (T-17).
- A plan that was valid under `Derive` keeps `Derive`'s numbers; reading an old save never calls the new path.

**Defaults and migration rules (all testable):**
1. Missing or absent branched marker means the legacy derivation. There is no marker to migrate.
2. A pre-v23 save has no prescription state (`Prescriptions.cs:24–29`); nothing is synthesized.
3. A save loaded with a confirmed branched plan and no prescriptions needs no migration.
4. Newer-than-supported saves already load "best-effort" with a warning (`ForestSaveController.cs:346–347`); no change.

**Reference Future v1 / Legacy40.** Unreachable **by the V2 geometry path**: Reference requires Legacy40 and save version 12 (`ScenarioReferenceArchive.cs:110, 126`), and V2 requires Enlarged80 (`RackPlan.cs:37–40`). The shared serialization and hash code is still exercised, so "unreachable" is not a substitute for regression checks. Testable assertions: the reference archive loads, its hash verifies, and the Legacy40 progression path still completes with the new code present.

**Option B1** would add a persisted network: a new v25, new default-valued fields, a hash strip rule modelled on `WithoutRackV2Fields`/`WithoutPrescriptionState` (`ScenarioReferenceArchive.cs:246–300`), and a validator. **Option C** adds nothing.

---

## 8. Test matrix

**Offline (plain .NET, `Tools/Verification/ScenarioOneNonUi`-style, as `Derive`'s own header describes).** Expected results are stable, not "green".

| ID | Scenario | Expected stable result |
|---|---|---|
| T-01 | Single group: suggested east–west layout and three other parallel fixtures through the new entry point | `Signature()` byte-identical to `Derive` |
| T-02 | The 3+2 reference racks | `Derive` invalid with the exact R4/R5 string; new path valid; groups {R2,R3,R5} and {R1,R4}; axes 135° and 45°; orders R5,R3,R2 and R1,R4; gaps 17.510 / 16.623 / 17.855 within 1e-3 |
| T-03 | All 120 input orders of the reference | identical `Signature()` |
| T-04 | Tie handling: equal midpoint offsets, equal lateral distance on a group boundary | lower rack id / lower group id wins; reported once |
| T-05 | Conservation property over seeded random valid two-group plans | unit + remainder + corridor areas = stand area (tolerance stated); stems sum to total |
| T-06 | Single-owner property: 10⁵ seeded points including boundary points ±1e-9 | exactly one of {Outside, Corridor, Unit, Remainder}; `Classify` is a function |
| T-07 | Grouping at tolerance −ε, 0, +ε, plus a chaining case where two group extremes exceed the tolerance while every adjacent pair is inside it | same grouping both runs; chaining case either refused or documented by the chosen Q05 rule |
| T-08 | Three direction groups | invalid, `…at most 2 are supported.` |
| T-09 | Branch crossing the common normal without physical intersection (reference R4/R5) | valid via grouping; no refusal |
| T-10 | Two opposing branches whose corridors overlap near the spine (reference R1/R2, 1.414 m) | corridor ground owned by lowest rack id; area counted once; no stem has two results; removal-set exclusivity unchanged (`RackPlan.cs:285–293`) |
| T-11 | Short northern branch (R5, 15.061 m) as non-teaching rack | valid; its units may be `NotCovered/TooFewStems`, never forced Covered |
| T-12 | Teaching rack under 24 m | invalid, existing wording |
| T-13 | Incomplete plan (fewer than five; unconfirmed ⇒ none) | invalid, `…exactly 5 … (found N)` |
| T-14 | Interior entrance, unmarked entrance | geometry result identical to marked-headland case: entrance state is not an MFM input; authoring refusal is a Rack V2 matter |
| T-15 | Tree already Fell-marked in an area; player manual area; non-Sitka adult | area `Excluded`/`Manual` exactly as today; tree in a corridor ignored |
| T-16 | Adding an unbuilt route overlay (not a mask, not a rack) | coverage `Signature()` unchanged |
| T-17 | Prescription made under legacy rule, plan reshaped to branched (and reverse) | refusal, zero orders, no id consumed |
| T-18 | Reopen with one executed rack | executed rack unchanged; others reconcile; no historic order rewritten |
| T-19 | Reference Future v1 and Legacy40 gates | unchanged results with new code present |
| T-20 | Save v24 round trip with branched confirmed plan | loads; world JSON identical on re-save; hash gates unchanged |

**Calibration re-qualification (offline, before any build).** Re-run the MFM-03 style comparison on the reference with real masks and the unchanged tolerances. `ScenarioOnePrescriptionCalibration.cs:5` says the V1 values were measured on one stand and one rack layout. State whether the same tolerances give a defensible Covered/NotCovered spread, with 12 of 14 covered on the suggested layout [N30] as the only existing baseline and *not transferable*.

**One bounded Unity run (later, separate task, new lease).** A throwaway copy of a fresh Enlarged80 world; no user save; the reference plan confirmed. Check: Stand Map shows five diagonal racks with coverage footprints per group and the rack-served legend; minimap shows the same racks; 3D entrance cue for each rack; prescription accept ⇒ Pending orders only (no Fell mark, Work Plan still gates); advance a year; save and load; reopening leaves an executed rack locked. **Renders are UNMEASURED and #250's attempt was killed under an 8 GB cap; rerun with at least 12 GB or on the real display with the user's agreement [N73].** No Windows build.

---

## 9. No-go risks and proof gaps

**No-go until resolved**
1. **Unguarded invalid geometry (§1.5).** Confirm, `UpdateRack` on a confirmed plan, and save validation never check `Derive`. Invalid geometry can reach confirmation and execution, and completed rack work locks that rack's geometry. No end-to-end player reproduction was performed. Needs a lifecycle gate (block recommended at Confirm and at every authoritative mutation, with a defined load behaviour) before any herringbone is playable — independent of A/B/C.
2. **Calibration non-transfer.** The tolerances were measured on one layout; the reference's coverage spread is UNMEASURED.
3. **Entrance headland-only, and entrance trees on a spine.** All five entrances are illegal today (`RackEditing.cs:123–125, 162–163`); #250 recorded real entrance trees but not legal ones. Applies to every spine layout.
4. **Yard conflict.** The straight continuation passes 0.035 m from the Forestry Workbench [N14]; the sample footprint sits about 6.70 m from the Clearing centre [N50]. A bypass or relocation decision is not designed anywhere.
5. **Corridor overlap and the editor's overlap rule.** R1/R2 centrelines 1.414 m apart [N13] against the editor's 2.4 m clearance [N43].
6. **Reach.** Up to 41.18 m from the nearest rack [N55]; no policy.

**Proof gaps (each is UNMEASURED or UNVERIFIED)**
- Renders, Stand Map/minimap/3D coherence, on-foot reachability, machine turning room.
- Real coverage states for the reference (no run applies the true masks and metrics to this geometry).
- Whether Objectives 2–5 complete on a plan with an invalid sample (§1.4).
- Whether rack orientation relative to planting rows matters to comparability.
- The class boundary of plans `Derive` accepts (§1.3).
- Minimap rack drawing path, Work Plan unit-label display and `ScenarioOneTreatmentReview` text for group-labelled units (not audited).
- Performance of `ReadCoverage`: it has no cache (`PrescriptionApproval.cs:242`); the cost of two group passes is not measured.
- The `trees.json` stems are from #248; sameness with #250's world is INFERENCE.
- My raster and stem analyses ignore the Track/Clearing/Site/Headland masks; they are upper bounds on stems and areas.
- The suggested east–west lines remain the minimap guidance (`map_model.json` `miniGuidance`) and contradict the reference unless replaced (#250 §7).

---

## 10. Recommended next spec decision for the Manager

**Recommendation: adopt Option A as the preferred hypothesis, by a spec amendment that comes with a validity gate at Confirm and at every authoritative mutation, and commission offline qualification before any build.** In one sentence: *keep `Derive` as it is, add a branched derivation that only runs for plans `Derive` rejects, make every authoritative plan change refuse geometry neither derivation can read, and let one demonstrated sample serve both groups through the unchanged comparability test — subject to qualification.*

**What this study has and has not established.** It has established the geometry of the stop, the impossibility of one global normal for the reference, the mechanics a second derivation would need, and a source-derived enforcement gap. It has **not** demonstrated that A is safe, that its coverage is calibrated, that its save semantics are unchanged, or that one sample transfers acceptably. B could also represent 45° branches at materially higher cost; C's spine-attached variant is unqualified and its 55.68° floor is a straight-spine approximation. **No option is approved here.**

Decisions the Manager/user must make before an implementation task can be written:

| # | Decision | My recommendation |
|---|---|---|
| D-A | One sample serves both groups, or a second sample is required | One sample, with the explanation rule in §A.2 |
| D-B | Group tolerance and the two-group cap | Start from the existing snap window; qualify; approve a number |
| D-C | Reach bound for rack-served units | No bound in the first cut; show distance to the owning rack; revisit after qualification |
| D-D | Validity gate: block or warn, and where it applies | Block at Confirm and at every authoritative mutation of a confirmed plan; define load behaviour for an already-invalid confirmed plan |
| D-E | Entrance anchor on a spine, yard bypass, corridor overlap near junctions | Separate Rack V2 decisions, not MFM; all three options need them |

**Alternatives and why rejected**
- **Loosen `Derive` or fabricate coverage:** forbidden by the packet and by D-065; it would hand the player a green status the geometry cannot support.
- **Option B1 (graph):** needs a persisted network and a second spatial model for a five-rack scenario; MFM V1 §18 excludes property-level scope. Revisit only if multi-stand play is approved.
- **Option B2 (nearest-segment partition):** needs additional curved-edge intersection handling and convergence qualification, and gives no advantage over A's owner rule at this scope. It is not rejected for being unmeasurable.
- **Option C:** cannot hold 45° (needs at least 55.68° on this spine) and still needs the entrance and yard decisions. Keep as the fallback if D-A to D-D are not accepted; C0 is already what ships.
- **A separate sample per side:** not supported by the code (one source sample per plan) and changes the Lessons 2–5 curriculum. Hold in reserve if qualification shows poor transfer.
- **Do nothing:** leaves the lock hazard in the shipped candidate. The Confirm-time gate alone is worth specifying now.

This study authorises nothing: no source, spec status, save field, Windows build or playtest change.

---

# Appendix A — Numerical claim table

**Basis key.** RAW = re-extracted by me from the raw file in this session. RECOMPUTED = computed by me from raw data with the scratch scripts described in Appendix B. SRC = read from the source at 72b4efbd (path:line). DOC = read from a document. PROPOSAL = a number I propose and nobody has approved.

**Raw files and hashes read.** `ScenarioOneFiveRackReference-20261103T2210Z/measurements/mfm_candidate.json` sha256 `67a24ac0f1288949f0dbfa149ae5538ed25faef465f403f9c1cd32ffc8aa15a2`; `…/design_candidate.json` sha256 `f9ca1d1977427ad78f882a58b09088a4f2b7346578b845dca4b40e67fe07975c`; `…/map_model.json`; `ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/trees.json` sha256 `822959038ffd2f4a0379e1b6c77bfff465ede76ec3141b675ab3035c42a5cbf0`. I did not compare these hashes with the READMEs; I only recorded them.

### A1. mfm_candidate.json (RAW)

| ID | Claim | Value | Source / key | Basis |
|---|---|---|---|---|
| N01 | Derive on the candidate's own racks | `valid=false`, `rackCount=5` | `geometryB_candidateRacks.valid`, `.rackCount` | RAW |
| N02 | Refusal text | `Racks 4 and 5 cross or overlap across the common normal, so they have no defensible order.` | `geometryB_candidateRacks.problem` | RAW |
| N03 | Axis and deviation reported | axis `135.00001525878906`; max deviation `0` | `geometryB_candidateRacks.axisAngleDegrees`, `.maxAxisDeviationDegrees` | RAW |
| N27 | Coverage geometry used for the mask table | five **suggested** east–west racks; plan not confirmed | root `note`; `manager_rackPlanConfirmed=false`; `geometryA_valid=true` | RAW |
| N28 | Baseline masks | 116 = 104 Track + 1 Clearing + 7 Site + 4 Headland (104+1+7+4=116) | `baselineMasks` | RAW |
| N29 | Scenario entries | 16 (8 scenarios × Track/Headland); `coveredCount` 12→12; `signature_changed` is the JSON text `"True"` in all 16 | `scenarios[0..15]` | RAW |
| N30 | Baseline coverage | covered 12 of 14 non-source (0.8571428571428571); 15 areas = 14 + source; 2 NotCovered (R1L, R2C, `MeanCompetitionDiffers`, BoundaryFail); 1 Manual (R3C source); second sample not required | `baseline.coveredCount/.nonSourceCount/.coveredFraction/.areas[]/.secondSampleRequired` | RAW |
| N35 | Baseline sample | 75 stems; 314.0050195986773 m²; 2388.496849376988 /ha | `baseline.sourceStems/.sourceNetAreaM2/.sourceDensityPerHa` | RAW |
| N31 | Baseline treatable | 994 stems; 4141.990353031232 m² | `scenarios[*].treatableStems_before`, `.treatableNetAreaM2_before` | RAW |
| N12 | C3 spine + five racks, Track | 156 newly masked; 883 stems after (−111, 994−883=111); 3682.1748589640792 m² after (−459.8155, 4141.990353031232−3682.1748589640792=459.8154940671528, rounds to 459.82) | `scenarios[4]` | RAW |
| N12b | C3, Headland | 150 newly masked; 888 stems (−106); 3693.959077977418 m² (−448.0313, rounds to 448.03) | `scenarios[5]` | RAW |
| N12c | C2 five racks only | Track 138 newly masked, 897 stems; Headland 133, 901 | `scenarios[2]`, `[3]` | RAW |
| N12d | Per-rack newly masked (Track / Headland) | R1 34/32; R2 25/24; R3 33/32; R4 36/35; R5 10/10; sums 138 and 133 (34+25+33+36+10=138; 32+24+32+35+10=133) equal C2 | `scenarios[6..15]` | RAW |
| N32 | Hypothetical sample cut | `sourceStems` 75→62 in C2/C3, `rawInSampleRect` 13 | `scenarios[2..5].sourceStems_after`, `.rawInSampleRect` | RAW |
| N33 | Compared-area stems and net area, baseline | stems 53 (R1C) to 100 (R4L); net area 221.392 to 429.932 m² | `baseline.areas[].stems/.netAreaM2` | RAW |

### A2. design_candidate.json, map_model.json, candidate_lines.csv, trees.json (RAW)

| ID | Claim | Value | Source / key | Basis |
|---|---|---|---|---|
| N10 | Rack endpoints (junction ⇒ far) and usable length | R1 (−2.436166286468506, −23.7) ⇒ (37.5, 16.236166286468514), 56.478267991510926 m; R2 (−2.436166286468506, −21.7) ⇒ (−35.4, 11.2638337135315), 46.617900705487706; R3 (2.9724510669708253, −3.6) ⇒ (−35.4, 34.77245106697083), 54.26684072040809; R4 (0.16338980197906475, 4.15) ⇒ (33.51338980197906, 37.5), 47.164022305142716; R5 (−2.7148311138153076, 26.85) ⇒ (−13.364831113815304, 37.5), 15.06137443927346; width 2.4 m | `racks[].junction/.far/.usable_length_m/.width_m` | RAW |
| N11 | Same-side gaps | 16.623102746936695, 17.50988569057508 (west), 17.854760117829294 (east) | `gaps.three_side`, `gaps.two_side` | RAW |
| N13 | R1–R2 centreline minimum distance | 1.414213562373095 m; corridors overlap true | `racks[0].vs_other_racks.R2` | RAW |
| N14 | Forestry Workbench to the continuation axis | 0.03516888618469238 m | `spine.yard_conflict[Forestry Workbench].dist_from_spine_axis_x_m` | RAW |
| N15 | Corridor trees per rack; totals | 24, 20, 29, 22, 8; sum 103 = distinct 103; spine continuation 18; union 120 = 103 + 18 − 1 (shared tree PO3518) | `racks[].corridor_tree_count`, `totals.*` | RAW |
| N16 | Spine lengths | existing 53.02025798867924; continuation 26.75; total 79.77025798867923 (53.02+26.75=79.77) | `spine.*_length_m` | RAW |
| N17 | Spine z-extent | south end z −39.989181518554688 (existing_polyline / map paths), north end 37.5; extent 77.489 | `map_model.json.paths[1].lastZ`; `design_candidate.json.spine.continuation_end` | RAW; extent RECOMPUTED (37.5+39.989181518554688=77.489181518554688) |
| N36 | Suggested layout lines | z = −35.271175384521484, −16.421764373779297, 0.23823529481887817, 15.793529510498047, 35.232353210449219, x from −35.4 to 40; gaps 18.849, 16.660, 15.555, 19.439 | `map_model.json.miniGuidance[]` | RAW; gaps RECOMPUTED |
| N37 | Stand size; clearing | sizeM 80; Clearing at (−2.7719941139221191, 11), r 4.5 | `map_model.json.sizeM`, `.landmarks[0]` | RAW |
| N38 | Clearing centre distance used in §A.2 | (−2.772, 11) (see N37) | `map_model.json` | RAW |
| N42 | Stem file | 1344 trees, all living Sitka, 0 stumps | `trees.json.trees[]` (`living`, `stump`, `sp`) | RAW |

### A3. Recomputed by me (scratch, Appendix B)

| ID | Claim | Value | Method | Basis |
|---|---|---|---|---|
| N20 | Lengths by group | west R2+R3+R5 = 115.94611586516926 m; east R1+R4 = 103.64229029665364 m; difference 12.30382556851562 | sum of N10 lengths | RECOMPUTED |
| N21 | Common-axis resultant | 12.303825568515608 m of 219.5884061618229 m = 0.05603130777063184 (equals N20 difference) | `Derive` lines 203–213 re-implemented | RECOMPUTED |
| N22 | Axis | 134.9999999999999° (double); Unity float N03 differs by float noise | same | RECOMPUTED |
| N23 | Normal ranges and overlaps | R4 [−50.214, −3.050]; R5 −17.066; R1 [−37.997, 18.481]; R3 0.444; R2 17.067. High[k]−Low[k+1]: R4→R5 +14.016; R5→R1 +20.931; R1→R3 +18.037; R3→R2 −16.623 | `Derive` lines 221–246 | RECOMPUTED |
| N24 | Forced 45° axis | order R1,R4,R2,R3,R5; R1→R4 −17.855; R4→R2 +16.441; R2→R3 +37.644; R3→R5 +28.714 | same, axis fixed | RECOMPUTED |
| N25 | Per-group valid | west order R5,R3,R2 gaps 17.510, 16.623; east R1,R4 gap 17.855; axes 135°, 45° | same per group | RECOMPUTED; equals N11 |
| N26 | R4–R5 centreline distance | 18.087 m; R1–R2 1.414; R1–R5 35.941 | dense segment sampling | RECOMPUTED; R1–R2, R1–R5 match N13 and `vs_other_racks.R5` 35.941293578075076 |
| N50 | R3 teaching anchor | unclamped along 4.647; clamped 12; anchor (−5.513, 4.885); 6.70 m to Clearing centre; 68 stems outside corridors, 15 in corridors; net 358.25 of 432 m² before masks | `Derive` lines 265–270; 0.25 m world-aligned grid centres from −39.875; trees.json. Independent reviewer grid gave 361.44 m² (+0.89%) | RECOMPUTED |
| N51 | Raster, 0.5 m cells | stand 6400.0; corridor union 524.25; non-corridor 5875.75 (6400.0−524.25) | grid centres, corridor = `Corridor.Contains` (half-width 1.2) | RECOMPUTED |
| N52 | Naive per-side strips | both 1575.0 m² (0.2680508871207931 of non-corridor); west only 2472.5; east only 1413.75; neither 414.5 (1575.0+2472.5+1413.75+414.5=5875.75). Stems outside corridors 1241 = 1344−103; both 311; west only 551; east only 294; neither 85 (311+551+294+85=1241) | strips per group as `Derive` 280–281 | RECOMPUTED |
| N53 | Lateral-distance owner rule | units 5461.25 + remainder 414.5 + corridors 524.25 = 6400.0 m²; stems 1156 + 85 + 103 = 1344. Unmasked unit stems: R1C 31, R1H 76, R1L 64, R2C 43, R2H 59, R2L 87, R3C 39, R3H 110, R3L 139, R4C 42, R4H 110, R4L 108, R5C 69, R5H 71, R5L 108 (sum 1156) | owner rule §5.4; segments at ±12 m about the anchor projection | RECOMPUTED |
| N54 | Suggested layout reach | non-corridor **5456.25** m² (raster-quantised; exact rectangle union would be ≈904.8 m² of corridor over the 75.4 m lines); farther than 9 m: 169.5; farther than 12 m: 0; farther than 20 m: 0; **sampled** max 10.43 m | 0.5 m world-aligned grid centres from −39.75; **rectangular** corridor test (production `Corridor.Contains`, inclusive longitudinal ends, half-width 1.2) on N36 lines. A capsule-ended test gives 188.3 m² and 10.6 m because it over-classifies corridor near the ends; exact geometric max ≈10.75 m at the west corners beyond the line ends | RECOMPUTED |
| N55 | Reference reach | farther than 9 m: 2246.0 m² (38.2% of 5875.75); 12 m: 1718.25; 20 m: 830.0, z in [−39.75, −10.25]; max 41.18 m. Stems farther than 9/12/20 m: 480 / 365 / 176 of 1241 | nearest finite centreline segment | RECOMPUTED |
| N56 | Nearest-segment owners | areas R1 2242.25, R2 1409.75, R3 864.25, R4 855.75, R5 503.75 m² (sum 5875.75; exact raster values, so rounding is exact to 0.25 m²); stems 475, 274, 181, 194, 117 (sum 1241) | 0.5 m raster and stems; nearest finite centreline segment | RECOMPUTED |
| N60 | One-family spine bound | 64/sin 45° = 90.51 m; 80/sin 45° = 113.14 m; spine z-extent 77.489 (N17); θ_min = asin(64/77.489) = 55.68° | straight-spine geometry, 4 gaps | RECOMPUTED (INFERENCE as to feasibility) |
| N61 | Corridor reconciliation | per-rack counts 24/20/29/22/8 sum 103 re-derived from trees.json with centre in [0,L]×[−1.2,1.2]; continuation within 1.3 m of x=−2.7148, z 10.75–37.5 = 18; overlap PO3518; union 120 | trees.json | RECOMPUTED; agrees with N15 |

### A4. Read from source at 72b4efbd (SRC)

| ID | Claim | Value | Source |
|---|---|---|---|
| N40 | Teaching footprint | half-length 12 m, half-width 9 m, minimum teaching length 24 m | `ScenarioOneRackBay.cs:35, 38`; `Derive` line 262 (2 × 12) |
| N41 | Calibration V1 | segment half-length 12; tolerances density 0.20, mean DBH 0.15, mean competition 0.20; minimum stems 30; boundary margins 0.65 and 1.5; candidate spacing 16 m; area step 0.1 m; second sample not required, limit 1.0 | `ScenarioOnePrescriptionCalibration.cs:73–75` |
| N43 | Editor constants | min length 5 m; entrance tolerance 0.01; overlap clearance 2.4 m (= `RackGeometry.DefaultWidthM`, `RackGeometry.cs:53`); angle snap 4°; boundary snap 2.5 m | `Racks/RackEditing.cs:96, 98, 102, 104, 105` |
| N44 | Derive constants | five racks (line 27); teaching index 2 (28); offset resolution 1000 per m (30); crossing tolerance 1e-3 m (31); axis refusal 1e-6 of total length (211) | `ScenarioOneRackSampleGeometry.cs` |
| N45 | Headland | x −35.4; z −37.7 to 37.7 | `ScenarioOneHeadlandGeometry.cs:7–9` |
| N46 | Save | version 24; at most 5 racks; confirmed ⇒ 5; prescriptions cap 1024; id and version text ≤ 64 chars | `ForestSaveData.cs:7`; `ForestSaveValidation.RackV2.cs:58–61`; `ForestSaveValidation.cs:466–467` |
| N47 | Link check tolerance | anchor match within 1e-3 | `ScenarioOneManager.PrescriptionApproval.cs:166` |
| N48 | Reference archive | Legacy40; save version 12 | `ScenarioReferenceArchive.cs:110, 126` |
| N49 | Lock | `IsLocked => Completed > 0` | `Racks/RackGeometry.cs:356` |

### A5. Read from documents (DOC)

| ID | Claim | Value | Source |
|---|---|---|---|
| N70 | Rack count and spacing band | five racks; 16–20 m band | Rack V2 spec §6, §7 |
| N71 | Protected mechanics | 30% first release, 40% basal-area safeguard, 8 m teaching spacing, 8 m Hegyi neighbourhood | MFM spec §5 |
| N72 | Stop conditions | MFM §22 first bullet; Rack V2 §39.6 items 1–2 | specs |

### A6. Proposals (PROPOSAL, unapproved)

| ID | Proposal | Value | Provenance |
|---|---|---|---|
| Q01 | `GroupToleranceDeg` | 4° (reuse of `AngleSnapDegrees`, N43) | convenience reuse, not evidence |
| Q02 | `OwnerTieBandM` | 1e-3 m (reuse of N44) | convenience reuse |
| Q03 | `MaxGroups` | 2 | smallest value that covers 3+2 |
| Q04 | Test parameters in §8 | 120 input orders (5! = 120) in T-03; 10⁵ seeded points and ±1e-9 boundary offsets in T-06 | test design, not measurements |
| Q05 | Group-chaining rule | either cap the group's extreme-to-extreme angular spread as well as the neighbour step, or declare chaining acceptable and test it | single-link clustering failure mode; unapproved |

### A7. Read from other handoffs (DOC)

| ID | Claim | Value | Source |
|---|---|---|---|
| N73 | #250 render attempt | killed under an 8 GB cgroup cap; next step suggests at least 12 GB | `FIVE-RACK-REFERENCE-HANDOFF.md` §5 |

**Reconciliations.** Corridor union: 103 + 18 − 1 = 120 [N15, N61]. Masks: 104 + 1 + 7 + 4 = 116 [N28]. C2 per-rack sums: 138 and 133 [N12d]. Naive-strip areas sum to 5875.75 and stems to 1241 [N52]; owner-rule areas sum to 6400.0 and stems to 1344 [N53]. Spine: 53.02 + 26.75 = 79.77 [N16]. The #250 handoff's 156/−111/−459.82 and 150/−106/−448.03 match the raw file [N12, N12b]; I did not carry them from prose.

# Appendix B — How the recomputations were done

Scratch scripts live in `/tmp/opencode` and are **not committed** (repo write scope is this one file). In outline, each re-reads the raw JSON above and does the following in pure Python with no third-party library:

1. `derive_check`: for each rack, accumulate `(dx²−dz²)/L` and `2·dx·dz/L`, take half the `atan2` for the axis (wrapped to [0,180)), compute entrance and far offsets on the normal `(−sin a, cos a)`, take midpoint, low and high, sort by millimetre-rounded midpoint with id tie-break, then test `High[k] > Low[k+1] + 1e-3` for every adjacent pair (the engine stops at the first).
2. `side_check`: run step 1 per group; build half-distance strips with mirrored outer edges; rasterise the 80 × 80 m stand at 0.5 m; classify each cell centre as corridor (`Corridor.Contains` rule), then by strip membership per group, then by nearest finite centreline segment.
3. `stem_check`, `resolved_check`: apply the same rules to the 1344 stems in `trees.json`; the owner rule compares lateral distance to the rack centreline of the strip each group puts the point in.
4. `sample_check`: the teaching anchor from `Derive` lines 265–270, then counts stems in the 24 × 18 m rectangle outside corridors.

Float32 versus double accounts for the 135.0000153° versus 134.99999999999999° difference [N22]; no result depends on it. The scripts' printed outputs are the values in A3. They were reviewed independently under Appendix C; the reviewer re-ran them from the raw files and its agreement or correction is recorded there.

---

# Appendix C — Independent cross-provider review record

**Reviewer:** `codex` / `gpt-6.1-sol`, `reasoningEffort: medium`, role `review`, read-only, task `node:delegated-task:…:delegate-task:28b40f93…`, 2026-11-03. **Verdict: MATERIAL ERROR** on first submission — "The core STOP and most numerical evidence reproduce, but the proposed gate and compatibility guarantees are incomplete and must be corrected before publication."

**Claim tally (reviewer's own table, parsed from its output): 40 rows = 28 PASS + 8 PASS-WITH-CORRECTION + 4 FAIL.**

The four FAIL rows and their disposition:

| Row | Failed claim | Correction applied in this document |
|---|---|---|
| C24 | A Confirm-only gate closes the irreversibility hazard | §1.5 now names `UpdateRack` (`RackPlan.cs:145–193`) as a confirmed-plan mutation that does not clear confirmation, states the lifecycle rule (Confirm + every authoritative mutation, or forced reopen), and requires a defined behaviour for already-invalid loaded plans; §5.3, §6, §9 and D-D follow |
| C28 | "No serialized field" implies hashes cannot move | §7 now states that hashes cover serialized **values** (`ScenarioReferenceArchive.cs:224–259`), that no new field only avoids a layout change, and that round-trip hashes, coverage decisions and provenance must be qualified |
| C30 | Stable unit ids preserve coverage-decision meaning | §7 now separates reference stability from meaning stability: the same `R{id}{L|C|H}` can denote a different polygon after a partition change, so decisions must be re-resolved against the live derivation |
| C36 | Curved boundaries make B2 unusable, establishing A | §2.3, §A.1 and §B.2 now describe numerical midpoint integration with exact cross-line intervals; curvature adds crossing/convergence work rather than impossibility, and B2 is rejected on cost, not measurability |

**The eight PASS-WITH-CORRECTION items, as applied:** sample raster convention stated and reviewer value disclosed (N50); suggested-layout non-corridor area corrected to 5456.25 m² and reach labelled a sampled maximum with its corridor convention (N54); nearest-segment areas restated as exact raster values to 0.25 m² (N56); straight-spine approximation retained beside every categorical C conclusion (N60); `signature_changed` recorded as JSON text `"True"` (N29); sequence `bay = null` now cites `ScenarioOneRackBayReader.cs:21`; Reference Future/Legacy40 "unreachable" limited to the V2 geometry path with shared hash code still regression-tested (§7); full path for the prescription-link tolerance (N47). Two further wording corrections were accepted and applied: "permanently disables MFM" softened to the evidenced hazard (§1.5, summary), and the "A is the only/simplest option that delivers the request" claim reduced to a preferred hypothesis with no option approved (§3, §4, §10).

**Numerical checks the reviewer confirmed independently** (its own Python, same raw files): group totals 115.946116 + 103.642290 = 219.588406 m and resultant 12.303826 m (ratio 0.0560313); overlaps +14.016114 / +20.931066 / +18.037316 / −16.623103 with R4/R5 reported first; forced-45° +16.440547 / +37.643738 / +28.713923; same-side 17.509886 / 16.623103 / 17.854760; R1/R2 1.414214 m; anchor (−5.512830, 4.885281) and unclamped 4.647425; 68 net and 15 corridor stems; 103 corridor trees with 311/551/294/85 naive split and 1156/85/103 owner split; reach 2246 / 1718.25 / 830 m² and 480/365/176 stems; nearest-segment stems 475/274/181/194/117; mask and baseline scenarios reconcile; `Derive` line ranges and every enforcement citation checked. Its raster convention: world-aligned cell centres over [−40, 40]², 0.5 m spacing from −39.75, rectangular corridors with inclusive longitudinal ends and half-width 1.2 m, no masks; sample check at 0.25 m from −39.875. It notes the reported maxima are sampled, not continuous-domain proofs.

**Judgement questions returned (Manager's, not errors):**
- **J1.** Should one demonstrated sample inform both groups? Existing scalar comparability permits the hypothesis; evidence does not establish forestry validity or acceptable teaching outcomes.
- **J2.** What geometry classes, grouping tolerance, remainder ownership and reach limits should be accepted? Single-link clustering also needs a chaining case where group extremes exceed the tolerance.
- **J3.** Where should validity be enforced, and how should already-confirmed invalid saves behave? Blocking Confirm is a recommendation, not the complete lifecycle policy.
- **J4.** Does preserving the 45° reference justify A's new partition and qualification cost, or should C remain the fallback? B is more expensive, but its rejection cannot rest on the incorrect exact-area argument.

**What the reviewer could not verify:** runtime execution, rendering, on-foot marking, turning room, masked reference coverage, save round trips and historical hash stability — all require qualification that this review was forbidden to run. It also did not establish a runnable, currently legal plan demonstrating permanent MFM loss, and did not re-derive every ancillary citation outside its listed checks.

**Disclosure.** This review is a source-and-arithmetic review. It is **not** a Unity run, not an acceptance of any option, and not a substitute for the offline re-qualification in §8 or the bounded Unity test that would follow a spec decision.

---

# Appendix D — V2 Orchestration Decision Appendix

**Decision unit:** #251 read-only specification study, under Overall Manager #200 and DPC #203. **Decision class: RESEARCH / SPECIFICATION INPUT ONLY.** This appendix records what the study did, what it verified, what it corrected and what it deliberately did not decide. It approves no model, no code, no save change and no spec status.

## D.1 Routing and execution (observed)

| Item | Record |
|---|---|
| Worker | `claudeAgent` / `claude-sonnet-5-5` / `effort: medium`, role `design` — Sonnet 5.5 Medium, per the packet's first option |
| Reviewer | `codex` / `gpt-6.1-sol` / `reasoningEffort: medium`, role `review` — the other provider, as the packet requires |
| Candidate | `origin/integration/scenario-one-player-test-ready-v1` = `72b4efbd1c653fa2231f632a79030a83ff76f833`, read via `git show`; no checkout, no Unity, no Library |
| Docs lane | `docs/mfm-topology-options-20261103-v1` in `/media/jer/Files/CCF/Operations/CCF-MFM-TopologyDocs-20261103`, based on `origin/main` `19245388`; branch and path were unused before this task |
| Unity lease | not requested and not used — the packet waives it for a read-only lane |
| Quota / context / tokens | **UNKNOWN** for all three agents; no telemetry exposed and none estimated |
| Retries / failures | none; both dispatches completed normally |

## D.2 Numerical provenance (the publication rule)

Every figure in this document was re-extracted from the raw artefact at publication time, not carried from an earlier prose summary:

- **DPC** independently recomputed the STOP arithmetic, overlaps, forced-45° counterfactual, per-group gaps, teaching anchor, corridor reconciliation, mask/baseline/C3 rows, reach and owner-rule conservation from `mfm_candidate.json`, `design_candidate.json`, `candidate_lines.csv`, `map_model.json` and `trees.json`.
- **Reviewer** recomputed the same set independently and corrected or qualified eight items (C10–C13, C17, C19, C22, C34) and failed four claims (C24, C28, C30, C36).
- Where DPC's and the reviewer's raster conventions disagreed (capsule-ended versus rectangular corridors), the **production** convention (`ScenarioOneRackBay.Corridor.Contains`) was adopted and the alternative disclosed at N54.
- Corrections are visible in the document rather than silently overwritten: the previous draft's 5446.25 m², its "exact area" argument and its "hashes cannot move" claim are all replaced here with a stated correction.

## D.3 What is established, and what is not

**Established:** the geometry of the stop (two families 90° apart; one length-weighted normal decided by a 12.304 m length difference); that the refusal is structural but does not prove all herringbones fail; that production has no `Derive` check on Confirm, on a confirmed-plan edit, or in save validation, and that completed rack work locks geometry; and that A, B and C differ in the ways §4 sets out.

**Not established (must not be published as fact):** that A is safe or calibrated; that one sample transfers acceptably to the other group; that any option preserves hash and coverage-decision semantics without qualification; that renders, minimap, 3D entrances, turning room or on-foot access work; that the reference plan is confirmable today (it is not — entrance and overlap rules); and the boundary of the plan class `Derive` accepts.

## D.4 Effect on controlling decisions and invariants

| Authority | Effect of this study |
|---|---|
| D-064 Rack V2 | **Unchanged and controlling.** No rack geometry, edit rule, confirmation rule, save field or mask was changed. The study recommends, but does not enact, a validity gate at Confirm and at authoritative mutations |
| D-065 MFM V1 | **Unchanged and controlling.** `Derive`, `Classify`, tolerances, states, remainders, safeguards and the Work Plan gate are untouched; the study only proposes spec clauses to amend later |
| D-067 / D-068 | Treated as direction, not approved geometry; the 3+2 reference is used as evidence, not as a design |
| Access-spine four-file spec | **Still DRAFT.** This document is not authority and does not raise its status |
| Reference Future v1 / Legacy40 | Not modified, not migrated; compatibility argued only as a testable requirement (§7) |
| User playtest / Windows build | Unaffected; no build was attempted |

## D.5 Manager's next decision

One decision is requested, with the alternatives recorded in §10: **whether to direct a spec amendment along option A** (branched derivation + validity gate at Confirm and at authoritative mutations), **keep option C as the fallback**, or **defer** until the Rack V2 entrance/yard/overlap questions are settled. D-A to D-E and J1–J4 are the inputs that decision needs. Nothing in this study authorises implementation; a separate, scoped implementation task and its qualification gates would still be required.

**Handoff status:** [HANDED-OFF] on #251. No Unity, production implementation, felling or Windows build was performed.
