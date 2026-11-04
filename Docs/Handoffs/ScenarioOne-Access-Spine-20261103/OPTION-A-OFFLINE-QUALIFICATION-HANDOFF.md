# #253 — Option A offline qualification: ownership, reach, validity lifecycle, compatibility

**Status:** `DRAFT` research return for Overall Manager #200 / DPC #201/#203. **Not reviewed. Not integrated. Not accepted.** No production edit, no Unity, no Player build, no Windows build.
**Worker:** Claude Sonnet 5.5, Medium (`claudeAgent / claude-sonnet-5-5`), role `research`, journal **EVENT 005**.
**Branch / lane:** `docs/mfm-option-a-offline-20261104-v1`, `/media/jer/Files/CCF/Operations/CCF-MFM-OptionA-Docs-20261104`, from `origin/main` `8f0f2301`.
**Source read (read-only, `git show`):** `72b4efbd1c653fa2231f632a79030a83ff76f833` (`integration/scenario-one-player-test-ready-v1`). Every `file:line` below is at that commit unless stated. `Assets/ForestPrototype` is abbreviated `AFP`.
**Controlling, unchanged:** Rack V2 / D-064, MFM V1 / D-065. D-067/D-068/D-069 are *direction only*. The four-file access-spine spec stays DRAFT / STOP IMPLEMENTATION.

---

## 0. Verdict

> **PARTIALLY QUALIFIED. Research recommendation: NEEDS CHANGES to Option A as drafted in #251. Not a NO-GO.**

**What survived testing (on the proposed rules, in an independent Python model, section 7):**

1. A deterministic **at-most-one-owner** partition is achievable. Every point in the stand maps to exactly one of `OUT / COR / UNIT / REM`. Over 108,000 seeded points on 40 random two-family plans, including points ±1e-9 either side of every strip edge, the map was total and repeatable. The exact interval scan closes to 6400 within 5.6e-11.
2. The published #251 numbers were reproduced exactly from the published endpoints: 1575.0 m² naive double-claim, the 5461.25 / 414.5 / 524.25 owner-rule close, 2246.0 / 1718.25 / 830.0 m² beyond 9 / 12 / 20 m, and 169.5 m² for the suggested layout. The model matches #251's arithmetic. That is evidence the model is a faithful port of the rules, not evidence about the C#.
3. **Input-order invariance** holds: 120 rack orders × 32 entrance/far flips (3,840 runs) gave an identical signature, and the 6,400-cell label map was identical across all 120 orders.
4. The legacy path is left untouched by construction. A plan `Derive` accepts returns the legacy result unchanged, and an unrelated legacy failure (duplicate id, near-zero length, wrong count, 4+1) is never "cured" by grouping.

**Why it is not qualified as drafted (each is a finding, not a style point):**

| # | Finding | Evidence |
|---|---|---|
| F-1 | **The #251 lateral-distance owner rule is unsafe without a reach cap.** It measures distance to an *infinite* centreline, so it hands a rack ground up to **69.17 m** away. Ground beyond 9 / 12 / 20 m of its own owner is **2,466 / 2,216 / 1,715 m²** (lattice values; section 7). The nearest-finite-segment owner gives 2,246 / 1,718 / 830 m² and a 41.18 m maximum. The two rules disagree on **2,312 m²** of owner. | §3.2, results.json `reach_*`, `owner_rule_disagreement_m2` |
| F-2 | **Conservative reach removes a large share of the ground whichever owner rule is used.** Within 9 m of its owner: 51% (lateral) or 62% (nearest) of the 5,875.75 m² non-corridor ground. The suggested parallel layout is 96.9% (5,286.75 of 5,456.25 m²). At 20 m: 64% / 86%. | §3.2 |
| F-3 | **The inherited anchor puts the one demonstrated sample on the junction, on top of the other family.** Of the 432 m² footprint: 70.8 m² is corridor and **150.6 m² lies in other racks' units, 143.0 m² of it in R4's Centre**. R4C is 216.8 m² unmasked, so about **66% of it** would be the sample, not an independent comparison. | §3.3 |
| F-4 | **Single-link grouping chains.** With a 4° link, A–B 3.9° and B–C 3.9° form one group though A–C is 7.8°. It needs a span guard. | §2.3, E3/E3b |
| F-5 | **A direction group of one rack is UNSPECIFIED.** The legacy strip rule halves the distance to a neighbour, and a lone rack has none. A 4+1 plan cannot be derived by "run the existing logic per group". | `RackSampleGeometry.cs:280-281`; E9 |
| F-6 | **The validity gate changes accepted Rack V2 behaviour.** Plans that confirm today but fail `Derive` (for example a third rack under 24 m) would stop confirming. At least 13 verification files call `ConfirmRackPlan`, and their fixtures are exposed. | §4.4 |
| F-7 | **#251's lattice values are biased by 0.1–1.0% of the stand.** Exact scan: units 5,454.26, remainder 418.68, corridor 527.06 m². The 0.5 m lattice gave 5,461.25 / 414.50 / 524.25. The bias does not shrink monotonically with finer lattices. | C-block, results.json `lattice_vs_exact_summary` |
| F-8 | **Correction to #251:** the R1/R2 corridor *rectangles* do not overlap. The separating-axis gap is **0.214 m**. #251's "corridors overlap true" is the editor's 2.4 m centreline-clearance rule (`RackEditing.cs:102`), under which R1/R2 at 1.414 m is already refused. #251's T-10 needs a different fixture (§7 F-block). | F1 |

**Is A still the least-risk route?** I cannot establish that. I did not model C. What I can say: the reach finding (F-2) is a property of the *reference layout*, so it would bind B too. A's advantage is only that it can represent the geometry at all. After a conservative reach rule, A is a bounded-reach model that leaves a large Manual/NotCovered share, and that is a product trade-off to approve, not a quiet default.

---

## 1. Scope map

Questions are answered in the order of the packet. Section 7 is the executable evidence, section 8 the test matrix, and section 10 the decisions.

---

## 2. (a) Deterministic at-most-one-owner assignment

### 2.1 Established code facts (legacy)

| Fact | Citation |
|---|---|
| `Derive` sorts racks by id first, so caller order never matters | `AFP/ScenarioOne/ScenarioOneRackSampleGeometry.cs:187-189` |
| Common axis is the length-weighted mean of the **doubled** angle; refuses below 1e-6 of total length | `:201-216` |
| Order by midpoint offset rounded to 1 mm, ties by rack id (`OffsetResolutionPerM=1000`) | `:30, 221-239` |
| Refusal: `High[k] > Low[k+1] + 1e-3`, first failing pair only | `:241-246` (`CrossingToleranceM`, `:31`) |
| Strips halfway between midpoint offsets; outermost mirrors its inner half gap; **needs ≥2 racks** (`offsets[1]`) | `:273-282` |
| `Classify` precedence: outside stand > rack corridor (loop in offset order) > outer remainder > unit (right-closed last strip) | `:131-151` |
| Two outer remainders always reported, even if empty | `:293-295` |
| Coverage `Locate` precedence: Outside > Corridor > Remainder > **Masked** > Sample > Area | `AFP/ScenarioOne/ScenarioOneCoverage.cs:435-458` |
| One global axis drives segmentation and the area scan: `anchorAlong` from `geometry.AxisX/Z`; `along` at `:454`; scan lines at `:469-472` | `ScenarioOneCoverage.cs:432, 454-455, 466-530` |
| `StandMapAreaFootprints` also assumes one `AxisX/AxisZ` and one unit family | `AFP/UI/Map/StandMapAreaFootprints.cs:8, 37-48` |

### 2.2 Proposed rule, with the gaps this study found filled

(`PROPOSAL`, unapproved; it is the #251 §A.1/§5.4 rule plus fixes.)

1. **Eligibility gate.** The branched path runs only if legacy `Derive` is invalid *and* the legacy error is one of: `"…cross or overlap across the common normal…"` or `"The racks do not share a direction…"`. The second is new: **two equal-total perpendicular families cancel in the axis sum and fail with that message, not the crossing one** (E10). #251 only considered the crossing message. All other legacy failures (count, ids, finite, width, length, stand size, teaching rack too short) are returned untouched.
2. **Group** by quantised undirected direction (period 180°, 1e-3° quantisation so a tolerance edge cannot flicker with float noise; E2). Group name = smallest rack id. At most 2 groups; ≥3 refused.
3. **Per group**, run the unchanged ordering and strip logic. A group of one rack is **UNSPECIFIED** (F-5) and must be refused or specified.
4. **Owner.** Corridor first (lowest rack id where corridors genuinely overlap). Then the unit with the smallest **lateral** distance; ties within a band go to the smaller group id. A point in neither group's range goes to the remainder of the group whose outer edge is **nearer** (smaller overshoot); tie, smaller group id. *This is the #251 rule. §3.2 shows it needs a reach cap, or replacement by nearest-finite-segment.*
5. **Segments** along the owning group's own axis, split at the teaching anchor's projection ± 12 m.

### 2.3 Established by the offline model

- **Identity is stable.** `R{id}{L|C|H}` ids survive; the partition behind an id changes only for branched plans (see §6 for the version consequence).
- **No duplicated work.** Owner is a pure function of the point and the geometry (B2, D1); a stem takes the label of its position, so no stem can carry two unit ids.
- **Input order.** B1, B2 (sections above). Re-labelling rack ids (1↔5, 2↔4) changed no unit area at the 1 m lattice (0.000 m²); ids matter only on tie bands.
- **Remainder strips.** The reference has 418.68 m² outside both groups' strips (exact scan). It is assigned one owner; it is always `Manual` (legacy `Remainder` state is fixed `Manual`, `ScenarioOneCoverage.cs:155-165`).
- **Segment areas (unmasked, exact scan):** R1C 184.8, R2C 206.8, R3C 210.3, R4C 216.8, R5C 265.6 m²; the largest, R3L, is 663.5 m².

### 2.4 Chaining and tolerance (question 1)

| Item | Result |
|---|---|
| Wraparound 179.9° and 0.1° | same group (E1) |
| Tolerance edge 3.9999999999 / 4.0 / 4.0000000001 / 4.002 | 1 / 1 / 1 / 2 groups: stable only because of the 1e-3° quantisation (E2) |
| Chain A–B 3.9°, B–C 3.9°, tol 4° | single-link accepts it; the span guard refuses it with a precise message (E3/E3b) |
| Order of the chain case | identical outcome over all 6 orders (E5) |
| Three families | refused (E4) |
| Duplicate id / near-zero length / 4 racks | legacy message returned unchanged (E6–E8) |
| Same-family plan that legacy refuses | the legacy message is returned, not rescued (E11) |

**Provenance of an angular tolerance.** *I do not select one.* The 4° `AngleSnapDegrees` (`AFP/Racks/RackEditing.cs:104`) is a soft UI pull, not a geometric limit, and the packet forbids copying it. The only first-principles bound I can derive: a family member is refused anyway by the within-group ordering test once two adjacent racks (rotated opposite ways about their midpoints) satisfy `(La+Lb)/2·sinφ > gap`. For the reference's own lengths and the closest gap this is φ ≈ asin(2·16.6/(54.3+46.6)) ≈ **19°** (`ASSUMPTION`: worst-case symmetric rotation; not a forestry standard). So any link tolerance above ~19° is dead code for this stand. Below that, it is a policy about when two directions are "different families".

Sweep (200 seeded perturbations per row, racks of the reference jittered ±δ about their midpoints, span guard = link tolerance):

| link/span tol | ±4° | ±6° | ±8° | ±10° | ±15° |
|---|---|---|---|---|---|
| 4° | 73 valid / 127 refused | 34 / 166 | 14 / 186 | 3 / 197 | 2 / 198 |
| 10° | **200 / 0** | 175 / 25 | 129 / 71 | 73 / 127 | 22 / 178 |

In this sweep every refusal was a **grouping** refusal; none was a within-group ordering refusal. A ≈4° tolerance with a span guard would reject about two-thirds of plans whose racks are merely hand-drawn ±4° off the intended 45°/135°. Whether that is desired is a product decision (D-2).

---

## 3. (b) Reach, mask and sample semantics

### 3.1 Actual MFM masks (established)

- `Mask` is a capsule (segment + radius); a circle is a capsule with equal ends. `Contains` is inclusive: `ex²+ez² ≤ r²` (`ScenarioOneCoverage.cs:51-67`). Kinds: `Headland, Track, Clearing, Site` (`:49`).
- Masks remove ground and stems from **both the sample and every area, identically** (`:450-453`, `:466-530`, header comment `:17-22`).
- **Precedence consequence:** corridor and remainder are tested before masks (`:444-452`). Remainder *stem counts and areas are therefore gross of masks* (`:283`, `:525`). A masked stem in an outer remainder still counts.
- Player judgements (fell mark, open order, non-Sitka, crop) ignore masks on purpose (`:285-297`, `Locate(..., false)`).
- Small-area cut-off is a **stem** count, not an area: `MinimumStems = 30`, then `NotCovered/TooFewStems` (`:340`; calibration `ScenarioOnePrescriptionCalibration.cs:73-75`). There is no minimum-area rule.
- `Compare` is three scalars (density, mean DBH, mean competition), inclusive tolerances 0.20 / 0.15 / 0.20, **no direction or side term** (`:360-380`, `Reason` enum `:100-109`).
- **No transitive chaining exists in Coverage today.** Each area is compared to the one source (`:343`); there is no area-to-area propagation.
- Input-order invariance: stems sorted by ordinal id (`:404-413`), racks by id (`RackSampleGeometry.cs:188-189`).

### 3.2 Reach (question 4)

Raw numbers (0.5 m lattice, `UNIT` ground only, distance to the **owning rack's finite centreline**; lattice bias per F-7):

| Owner rule | Owned m² | > 9 m | > 12 m | > 20 m | Max |
|---|---|---|---|---|---|
| Lateral (#251) | 5,461.25 | 2,466.25 | 2,216.25 | 1,715.25 | **69.17 m** |
| Nearest finite segment | 5,875.75 | 2,246.00 | 1,718.25 | 830.00 | 41.18 m |

Ground **within a cap of its own owner** (eligible to be *Covered*; everything else would be `NotCovered/Manual`), as a share of 5,875.75 m² non-corridor ground:

| Cap | 9 m | 10 m | 12 m | 16 m | 20 m |
|---|---|---|---|---|---|
| Lateral owner | 2,995 (51.0%) | 3,080 (52.4%) | 3,245 (55.2%) | 3,541 (60.3%) | 3,746 (63.8%) |
| Nearest owner | 3,630 (61.8%) | 3,843 (65.4%) | 4,158 (70.8%) | 4,658 (79.3%) | 5,046 (85.9%) |

Reading: using those cells as "rack-served" without a cap would be misleading, since up to 69 m of walking from the owner is invisible to the model. The 9 m teaching half-width is not evidence for a reach (`ScenarioOneRackBay.cs:38`, a teaching footprint). *I invent no empirical Irish practice limit.* The cap value is open (D-1). Any cap changes the `Covered` area; calibration under it is UNMEASURED.

### 3.3 Sample (question 3)

- **Established:** the sample footprint is a fixed 24 × 18 m rectangle anchored on the third rack in the common-normal order (`TeachingRackIndex=2`, `RackSampleGeometry.cs:28, 260-271`), width 9 m each side (`ScenarioOneRackBay.cs:35, 38`). It overrides area membership in `Locate` (`:453`).
- **Offline geometry result (F-3):** footprint at the reference's R3 anchor (-5.513, 4.885), 432.0 m² total: corridor 70.8, R3C 210.6, **R4C 143.0**, R2C 6.0, R5C 1.6 m². The anchor is the spine junction. One demonstrated sample therefore physically sits next to, and largely *over*, the east family's first Centre area.
- **Not measurable offline:** whether one sample's density / mean DBH / competition fits the other side. That needs trees and masks. `trees.json` was in another lane under `Operations/` and was **not read**. Result: **UNMEASURED**.
- **Orientation-dependent sample-transfer issue the code does not check:** the comparison ignores row orientation relative to the 45° rack. The honesty rule from #251 stands: every east-group explanation must name the sample's group.

---

## 4. (c) Fail-closed validity: Confirm, mutation, load/reopen

### 4.1 Established code facts

| Path | Behaviour | Citation |
|---|---|---|
| `ConfirmRackPlan` | checks 5 racks, per-rack geometry, entrance tree, tree-claim exclusivity; **no `Derive` call**; atomic (writes after all checks) | `AFP/ScenarioOne/ScenarioOneManager.RackPlan.cs:239-343`, writes begin `:313` |
| `UpdateRack` | on an **unlocked** rack of a **confirmed** plan: rewrites geometry and reconciles orders **without clearing `rackPlanConfirmed`** | `:146-194`, reconcile `:187-189` |
| `AddRack` / `DeleteRack` | un-confirm the plan when count ≠ 5 | `:139-140`, `:230-231` |
| `ReopenRackPlan` | clears `rackPlanConfirmed`; touches no work or marks | `:347-356` |
| Locked rack | `IsLocked => Completed > 0`; `UpdateRack`/`DeleteRack` refuse | `AFP/Racks/RackGeometry.cs:356`; `RackPlan.cs:155-156, 205-206` |
| `ConfirmedRacks` | returns **empty** when unconfirmed, so every MFM path suspends on reopen | `RackPlan.cs:50-51` |
| MFM world read | `Derive(ConfirmedRacks)` invalid ⇒ `"The confirmed rack plan gives no valid sample: …"`; `ReadCoverage`, create, preview, approve all refuse | `AFP/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs:255-266` |
| Save validation | count ≤ 5; confirmed ⇒ exactly 5; finite; non-degenerate; **no direction or ordering check** | `AFP/ForestSaveValidation.RackV2.cs:58-61, 81-85` |
| Load | `ForestSaveController` calls `ValidateRackV2Json` and `Validate` | `AFP/ForestSaveController.cs:272, 303` |

So the lifecycle hazard #251 reported is **confirmed from source**. A plan that `Derive` rejects can reach Confirm, can be edited while confirmed (`UpdateRack`), can be saved, and its racks lock once work completes. Also confirmed: the **only existing fail-closed behaviour is read-side** (MFM refuses). Nothing is read-side-silent: `Derive` invalid already suspends prescriptions without erasing anything. *No end-to-end irreversible reproduction was run (no Unity); it stays a source-backed risk.*

### 4.2 Proposed policy (`PROPOSAL`; consistent with the Manager review)

One pure predicate, `RackSampleValidity.Check(racks, half) → (ok, problem)`, defined as *legacy `Derive` valid OR branched derivation valid*, with no other side effects.

| Point | Rule | Where (insertion point, not an edit) |
|---|---|---|
| Confirm | Refuse with the precise `problem` text when the predicate fails | after the tree-claim check, before the first write (`RackPlan.cs:293`→`:313`), keeping the "validation before mutation" atomicity (`:111`) |
| `UpdateRack` while `rackPlanConfirmed` | Evaluate the **hypothetical** plan with the rack replaced; refuse (or require explicit reopen) before `:174` | between the existing refusals (`:169-172`) and the first write (`:175`) |
| `AddRack` / `DeleteRack` | No change: already un-confirm | n/a |
| Load | **Never reject on this predicate.** Load; expose `problem`; leave MFM suspended (it already is, via `:261-266`) | `ValidateRackV2` must not be extended to call the predicate |
| Reopen | Already suspends all MFM (`ConfirmedRacks` empty). Reconfirm runs the Confirm gate | existing |
| Executed history | The predicate is read-only; completed orders and locked racks are never rewritten | by construction |

**Irreducible case (needs a decision, D-7):** a loaded confirmed plan that is invalid under both derivations *and* has all five racks locked cannot be repaired. Locked racks cannot be changed or deleted. Reopen leaves them as-is. The only honest outcome is **MFM permanently suspended with a precise error**, with the rest of the game continuing. Rewriting geometry or bypassing the red status is forbidden by the packet.

### 4.3 Not testable offline

Whether Objectives 2–5 remain completable on such a plan (#251 §1.4, `LearningObjectivesView.Sequence.cs:63-71`, `ScenarioOneRackBayReader.cs:21`) is **UNMEASURED**.

### 4.4 Collateral

`ConfirmRackPlan` appears in at least 13 files under `Tools/Verification` at `72b4efbd` (for example `RackV2ConfirmMinimapVerification.cs` ×18, `RackV2EditingVerification.cs` ×4, `MfmRackSampleGeometryVerification.cs` ×3). Any fixture that confirms a plan failing both derivations would break. This is **UNMEASURED** (needs fixture-by-fixture inspection) and is F-6. Derive-valid also does not mean sensible: legacy `Derive` accepts two parallel racks **0.5 m apart** (a 0.5 m strip; E12). The gate must not claim to enforce spacing.

---

## 5. (d) Legacy behaviour, old saves, versions, hashes

**Established:**

- `RuleVersion = "MFM-V1-RULE-1"` is a **global constant** (`AFP/ScenarioOne/ScenarioOnePrescriptionCalibration.cs:26`), used at `ScenarioOnePrescription.cs:47, 340`, `ScenarioOneCropTreeCandidates.cs:93`, and the validator `ScenarioOnePrescriptionApproval.cs:152`. Calibration version is `"MFM03-CAL-1"` (`:73-74`).
- A record stores both (`ScenarioOneManager.PrescriptionApproval.cs:66-67`); mismatch ⇒ `RuleVersionMismatch` / `CalibrationMismatch` / `SampleChanged` (`ScenarioOnePrescriptionApproval.cs:152-168`). Fail-closed already.
- Version text: letters, digits, `.`, `_`, `-`, ≤ 64 chars (`AFP/ForestSaveValidation.cs:619-628`). So **`+` is illegal**; a suffix such as `-BR1` is legal.
- `ForestSaveData.CurrentVersion = 24` (`AFP/ForestSaveData.cs:7`). No new field is required by A.
- Hash: `CurrentWorldHash` hashes the serialized **values** (`AFP/ScenarioOne/ScenarioReferenceArchive.cs:224-228`); `HashLayout` branches on prescription and rack-V2 state (`:246-261`).
- Reference Future v1 requires Legacy40 and save v12; V2 requires Enlarged80 (`ScenarioReferenceArchive.cs:110, 126`; `RackPlan.cs:37-40`).

**Consequences (reasoned, not run):**

1. *No new field avoids a schema-layout change, not a semantic change.* A branched-plan prescription's stored values differ from any legacy one, so it hashes differently. That is correct and expected.
2. **Legacy saves keep legacy values only if the legacy path is byte-identical.** The offline model returns the legacy result object unchanged (D4). The C# must be tested for the same: `Signature()` equality through the new entry point (#251 T-01).
3. **The expected rule version must be chosen from the derivation the live plan uses** (legacy ⇒ exactly `MFM-V1-RULE-1`; branched ⇒ a distinct legal string, for example `MFM-V1-RULE-1-BR1`). A global bump would strand every old prescription (D-8).
4. **A plan can flip derivation** across `UpdateRack`/reopen/reconfirm. Prescriptions made under one derivation then fail closed with `RuleVersionMismatch`. This is the desired behaviour but needs its own test (#251 T-17).
5. **Same unit id, different meaning** across derivations is real: `R3C` means different ground. Fail-closed on version is what protects it.
6. Pre-v23 saves have no prescription state and nothing is synthesised (#251 §7; not re-verified by me).
7. **Round-trip hashes, previously approved prescriptions, Legacy40 and the frozen Reference Future v1 were not run.** They need the plain-.NET harness (`Tools/Verification/MfmSaveProvenance`, present at `72b4efbd`) on a candidate that contains the change, which does not exist. **UNMEASURED.** No protected archive was touched.

---

## 6. (e) What the offline script proves, and what it does not

`Docs/Handoffs/ScenarioOne-Access-Spine-20261103/OptionA-Offline-20261104/optionA_offline_analysis.py` — disposable, standalone, no packages, seeded, writes `results.json` and `run.log`. Run: `python3 -I -B optionA_offline_analysis.py [outdir]`. Two consecutive runs produced a byte-identical `results.json`.

**Proves (for the *modelled rules*, in doubles):**

- A total, repeatable single-owner map; conservation of unit + remainder + corridor + outside = 6400 on the exact scan (5.6e-11 worst case over 40 random plans).
- A production-style **interval scan** extended with every discontinuity (strip edges, lateral-tie lines ± tie band, outer-overshoot lines, corridor ends, per-group segment lines) classifies each piece once, and zero random points inside any piece disagree. So the existing `MeasureAreas` technique (`ScenarioOneCoverage.cs:460-530`) *can* be extended to two axes in principle. This is a feasibility argument, **not** a statement about the C#.
- Order invariance, orientation (entrance/far) invariance, tolerance-edge stability under quantisation, chaining refusal, and non-rescue of unrelated failures.

**Does not prove:**

- That the C# behaves the same (no C# was compiled or run).
- Any stem count, density, DBH, competition or Covered/NotCovered outcome. No tree data was read. The segment areas above are **geometry only**. A rough density of 2,100 stems/ha would give R1C ≈ 39 stems unmasked, against #251's 31 for its segmentation (that figure is #251's and is **not** re-derived here). Masks will lower it. Whether R1C falls under the 30-stem minimum is **UNMEASURED**.
- Nearest-finite-segment ownership in the interval scan. That rule adds curved boundaries (point-end meets line) and I **did not implement its crossing set**. Its areas above are lattice only.
- Rendering, minimap, Stand Map footprints, 3D marking, turning, or any Unity behaviour.

**Scope/accuracy limits:** lattice values carry up to ~0.1% of stand (F-7); I used the exact scan for conservation and the lattice only to reproduce #251 and for reach tables. Endpoints are #251 Appendix A row N10 as published in `MFM-TOPOLOGY-OPTIONS-HANDOFF.md`; the raw `design_candidate.json` lives in another lane and was **not read**, so "reviewed against raw JSON/CSV at publication" is still owed by the independent reviewer.

---

## 7. Executed results (from `run.log`, 41 checks, 0 failed)

| Block | Checks | Outcome |
|---|---|---|
| A reproduce #251 | A1–A6 | legacy refuses with the R4/R5 string; groups {R2,R3,R5}/{R1,R4}; per-group gaps 17.510/16.623/17.855; teaching R3, anchor (-5.513, 4.885); 1575.0 naive; 5461.25 + 414.5 + 524.25 |
| B invariance | B1–B3 | 3,840 runs and 120 label maps identical; id-relabel max change 0.000 m² |
| C exact scan | C1–C5 | piece-constant; total 6400 within 1e-6; step 0.1 vs 0.05 differ ≤ 0.505 m² per unit; Monte Carlo (400k) agrees within 1.12σ; **exact: units 5,454.262, remainder 418.680, corridor 527.059 m²** |
| D properties | D1–D4 | 40 random plans, 108,000 points; parallel plan returns the legacy result unchanged |
| E edge cases | E1–E12 | see §2.4; E10, E12 are new observations |
| F corridors | F1–F3 | R1/R2 rectangles separated by 0.214 m; a synthetic perpendicular crossing overlaps ≈5.72 m² (exact 5.76) and is counted once, owned by the lowest id |
| G reach | G1–G2 | #251 N54/N55 reproduced; reach tables above |
| H tolerance | H1 | sweep in §2.4 |
| I sample | I1 | footprint split in §3.3 |

Exact per-rack unit area (scan, all segments): R1 879.2, R2 925.0, R3 1,363.4, R4 1,251.7, R5 1,034.8 m².

**Raw-result index (preserved in the lane, not the production tree):**

| File | sha256 |
|---|---|
| `OptionA-Offline-20261104/optionA_offline_analysis.py` | `459bb4649ece9f01a96b45b8220afc08615c51010eaf6a27f17e25dc3e456152` |
| `OptionA-Offline-20261104/results.json` | `80f371e946f13ad7f7724571abb58f7852a06d4b5a9c94d06731508c52e3a80c` |
| `OptionA-Offline-20261104/run.log` | `ea01ebd47d08b8c8f971bbb36ef21db09e3692f91adc313da90af260a423260e` |

(`run.log` contains wall-clock timings and will differ on re-run; `results.json` does not. The script hash above is of the file as committed; if it is edited, regenerate and re-hash.)

---

## 8. Qualification / test matrix

**Legend:** ✔ executed offline here (model only) · ○ required, not run · ✘ cannot be done offline.

| ID | Case | Expected stable result | Status |
|---|---|---|---|
| Q-01 | Parallel plans (suggested layout + ≥3 fixtures) through the new entry point | `Signature()` byte-identical to legacy | ✔ model (D4); ○ C# |
| Q-02 | 3+2 reference | legacy invalid, exact R4/R5 text; groups, axes, gaps as in §7 | ✔ model; ○ C# |
| Q-03 | All 120 orders × 32 flips | identical signature and label map | ✔ model; ○ C# |
| Q-04 | Ownership ties: equal lateral, equal offset | lower group / rack id; once | ✔ model totality at ±1e-9 edge points only; tie *policy* not asserted separately (○); ○ C# |
| Q-05 | Conservation over random plans | sum = 6400 within stated tolerance | ✔ model; ○ C# polygons and `MeasureAreas` |
| Q-06 | Totality at ±1e-9 edge points | one label each | ✔ model; ○ C# |
| Q-07 | Grouping tolerance −ε/0/+ε, wrap, chaining | stable; chaining refused | ✔ model |
| Q-08 | Three families | refused, precise text | ✔ model |
| Q-09 | Group of one rack (4+1) | refused or specified | ✔ model shows UNSPECIFIED; ○ decision D-3 |
| Q-10 | Equal-total families (axis cancellation) | covered by eligibility rule | ✔ model (E10) |
| Q-11 | Genuinely overlapping corridors (synthetic crossing) | lowest id; area once | ✔ model (F2/F3); replaces #251 T-10 |
| Q-12 | Teaching rack < 24 m, < 5 racks | legacy wording | ✔ model for the < 5 case (E8); teaching < 24 m ○; ○ C# |
| Q-13 | Reach cap sweep (9/10/12/16/20 m), both owner rules | tables §3.2; NotCovered/Manual beyond cap | ✔ model (lattice); ○ nearest-owner exact scan |
| Q-14 | Real stems + real masks: Covered/NotCovered spread, R1C vs 30-stem minimum | defensible spread | ✘ UNMEASURED (needs trees + masks) |
| Q-15 | Sample at alternative anchors; share of sample over other units | measured per anchor rule | ✔ model one anchor (I1); ○ alternatives |
| Q-16 | Confirm refuses a plan failing both derivations; no write | `Invalid`, state unchanged | ○ (C#) |
| Q-17 | `UpdateRack` on confirmed plan that would break validity | refused before `:175`; orders untouched | ○ (C#) |
| Q-18 | Load a confirmed invalid plan | loads; error shown; MFM suspended; nothing rewritten | ○ (C#) |
| Q-19 | Load with one executed rack, others invalid | executed rack and history unchanged; repair path works | ○ (C#) |
| Q-20 | Load with all five racks locked and invalid | MFM suspended permanently; game continues | ○ (C#), D-7 |
| Q-21 | Prescription made under legacy, plan reshaped to branched, and reverse | `RuleVersionMismatch`, zero orders, no id consumed | ○ (C#) |
| Q-22 | v24 round trip, branched confirmed plan | world JSON identical on re-save; hash gates unchanged | ○ (C#) |
| Q-23 | Pre-v23, Legacy40, Reference Future v1 gates with change present | unchanged results | ○ (C#) |
| Q-24 | Existing `ConfirmRackPlan` fixtures vs the new gate | list of fixtures that now refuse | ○ inspection |
| Q-25 | Adding an unbuilt route overlay | coverage `Signature()` unchanged | ○ (C#) |
| Q-26 | Rendered Stand Map, minimap, 3D entrance cues | consistent with partition | ✘ UNMEASURED (needs a Unity lane) |

**Hard stops (qualification fails, production blocked) if any of these occurs:**

1. Any legacy-valid plan's `Signature()` changes through the new entry point.
2. Any point or stem receives two owners, or conservation fails beyond the stated tolerance.
3. Any result depends on input order or entrance/far orientation.
4. Group tolerance admits a chain, or the rule silently accepts a case invalid for an unrelated legacy cause.
5. A load rejects an otherwise loadable save, or any executed order, history or locked rack is rewritten.
6. A reach rule is adopted without a product-approved cap and a stated NotCovered/Manual fallback.
7. A prescription made under one derivation is readable under the other without `RuleVersionMismatch`.
8. A protected archive (Legacy40, Reference Future v1) hash or result changes.

---

## 9. Assumptions and uncertainty (kept apart from code facts)

**Assumptions:**

- A1. The Python model is a faithful port of `Derive` lines 179-298 and the cited helper semantics. Supported by exact reproduction of six #251 figures; not proven against the C#.
- A2. Published N10 endpoints are accurate; raw JSON was not read (machine rule).
- A3. The ≈19° ordering ceiling in §2.4 uses symmetric opposite rotations (worst case) and the reference's lengths; other layouts differ.
- A4. Density 2,100 stems/ha is a crude 1,344 / 6,400 m² average, used once for an order-of-magnitude remark only.
- A5. "Within cap of its owner" uses distance to the owner's finite centreline. A real access model could weight terrain, corridors or the spine; that is not modelled.

**Unmeasured / open:** calibration on real stems and masks; whether Objectives 2–5 complete on a branched plan; renders; the C# itself; save and hash round trips; the nearest-owner exact areas; fixtures exposed by a Confirm gate; the spine-entrance, workbench-yard, R1/R2 clearance, turning and layout questions, which are **carried forward unsolved** (question 7). Drawing a route must not change MFM masks or the world; I verified nothing about that offline. No new path planks, blanket tree removal or Track→Headland relabelling are proposed.

**Independent review:** none done. The packet requires the other provider (GPT-6.1) to review fixed claims, especially F-1, F-3, F-8 and the lifecycle citations. DPC to arrange.

---

## 10. Decisions needing Manager / user approval

("D-n" elsewhere in this document means decision n below.)

1. **Reach policy.** Is a cap required (this study says yes), what value, and what basis? Neither 9 m nor 41.18 m is a threshold. Choose between a lateral owner **with** a cap and a nearest-finite-segment owner **with** a cap (the latter needs its curved-boundary crossings qualified).
2. **Group tolerance and chaining.** Fixed constant versus derived; link and span tolerance values; whether ≈4° (rejects ~⅔ of hand-drawn ±4° plans) or ≈10° is acceptable; adopt the span guard (this study recommends yes) and the 1e-3° quantisation.
3. **Minimum group size.** Refuse a group of one rack (this study recommends refusing until specified), or define a strip rule for it. Same for the equal-total ("no shared direction") eligibility.
4. **Teaching rack and anchor for a branched plan.** The inherited closest-to-origin anchor puts the sample on the junction over R4C. Options: a player-chosen anchor along the teaching rack, a different anchor rule, or requiring the footprint to avoid the other family's units.
5. **One-sample hypothesis.** Accept as a labelled research hypothesis only, require the east-group explanations to name the sample's group, and authorise a later, separate measurement using real stems and masks.
6. **Validity gate shape.** Hard refusal at Confirm and at `UpdateRack`, versus forced reopen/reconfirm; and acceptance that plans which confirm today but fail `Derive` will stop confirming (F-6). Whether to also refuse degenerate-but-`Derive`-valid spacing (E12).
7. **Fully locked invalid load.** Accept "MFM permanently suspended with a precise error" as the outcome when no repair is possible.
8. **Rule/calibration version scheme.** Per-derivation expected rule version (legacy unchanged at `MFM-V1-RULE-1`, branched a distinct legal string) versus a global bump; remainder ids for two groups (legacy is one `REM-Right`/`REM-Left` pair); which version string covers a reach cap.
9. **Segment axis for the non-teaching group.** Owning group's axis with the teaching anchor projected onto it, or another rule.
10. **Scope of the UI/footprint change.** `StandMapAreaFootprints` and the area scan assume one axis. Confirm that group-aware polygons are in scope of any later spec.
11. **Remainder owner rule.** The nearer-outer-edge rule is a placeholder; confirm or replace.
12. **Next step.** Whether to commission (a) the independent GPT-6.1 review, (b) a C#-level plain-.NET qualification of Q-01…Q-25 on an unmerged branch, and (c) an Option C re-qualification under the same reach rule, since this study cannot say A beats C.

Carried forward unresolved (no approval requested here): illegal spine entrance trees, Workbench yard conflict, R1/R2 clearance, turning room, rendered/map/minimap consistency, and honest proposal-vs-cleared mask timing.

---

## Appendix — V2 Orchestration Decision Appendix

**Decision unit:** #253 offline Option A qualification. **Class: RESEARCH / SPECIFICATION INPUT ONLY.** It approves no model, code, save change, spec status, Fell/Work Plan execution or build.

| Item | Record |
|---|---|
| Journal entry | **EVENT 005** |
| Worker | `claudeAgent` / `claude-sonnet-5-5` / effort **medium**, role `research` (Sonnet 5.5 Medium per the packet's first option) |
| Parent | DeepSeek DPC (#201/#203) under Overall Manager #200 |
| Lane | `docs/mfm-option-a-offline-20261104-v1` at `/media/jer/Files/CCF/Operations/CCF-MFM-OptionA-Docs-20261104`, from `origin/main` `8f0f2301`; verified clean at start |
| Source | `72b4efbd` read via `git show` only; no checkout |
| Unity / Library / Player / Windows | none started, none created |
| Other lanes | none read or modified. `Operations/` and `ZX20/` other than this lane untouched, so `trees.json` and raw measurement JSON were not read |
| Quota / context / tokens | **UNKNOWN** (no telemetry exposed to the worker) |
| Retries / failures | two analysis-script test criteria were mis-specified and corrected before the final run (a step-stability threshold, a corridor-overlap assertion); one earlier raster comparison failed to converge, which exposed the lattice-bias finding F-7. No dispatch failure. |
| Files written | this handoff; the three files in `OptionA-Offline-20261104/`. No production source, `.meta`, scene, save, test, ledger or `active-tasks.md` edit |
| Review state | **DRAFT** — unreviewed; cross-provider review outstanding |

**Established vs not:** *Established* — the lifecycle gap and its insertion points; the global rule version and the per-derivation consequence; the single-axis dependence in coverage and map footprints; the model's exact reproduction of six #251 figures; the single-owner, conservation and order-invariance properties of the proposed rule *in the model*; F-1…F-8. *Not established* — anything about the C#, real stems, calibration, hashes, renders, or A versus C.

**Effect on controlling decisions:** D-064, D-065 unchanged and controlling. D-067/68/69 treated as direction only. The access-spine spec stays DRAFT. Legacy40, Reference Future v1 and the player's playtest branch untouched.

### Event packet for the DPC to append (the worker does not edit `ORCHESTRATION-LEDGER.md`)

```
EVENT 005 — #253 Option A offline qualification — RESEARCH WORKER RETURN
pool: claudeAgent / claude-sonnet-5-5 / effort medium / role research
lane: docs/mfm-option-a-offline-20261104-v1 @ /media/jer/Files/CCF/Operations/CCF-MFM-OptionA-Docs-20261104 (from origin/main 8f0f2301)
source: 72b4efbd (git show only). Unity: none. Player/Windows build: none.
output: Docs/Handoffs/ScenarioOne-Access-Spine-20261103/OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md + OptionA-Offline-20261104/{optionA_offline_analysis.py,results.json,run.log}
status: DRAFT (IMPLEMENTED as research only). Not reviewed / integrated / accepted.
verdict: PARTIALLY QUALIFIED; recommendation NEEDS CHANGES (not NO-GO)
checks: 41 offline model checks, 0 failed (model only; C# unrun)
key findings: F-1 lateral owner reaches 69.17 m; F-2 reach cap leaves 38% (9 m) to 14% (20 m) of ground beyond reach on the nearest-owner basis; F-3 sample footprint 150.6 m2 in other units (143.0 m2 in R4C); F-4 chaining; F-5 group-of-one unspecified; F-6 Confirm gate changes accepted behaviour; F-7 lattice bias; F-8 R1/R2 rectangles do not overlap (0.214 m gap)
quota/context/tokens: UNKNOWN
routing evidence: not an escalation; no retries of dispatch; two script criteria corrected in-session
needs: independent GPT-6.1 review of F-1, F-3, F-8 and lifecycle citations; Manager decisions 1-12 in the handoff
```
