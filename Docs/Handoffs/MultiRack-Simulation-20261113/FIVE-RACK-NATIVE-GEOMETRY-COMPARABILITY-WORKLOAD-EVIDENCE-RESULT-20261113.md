# Five-rack native geometry, comparability and workload evidence — #466 result

**Date:** 13 November 2026 (work 20:09–20:40 UTC)
**Claim:** [#466](https://github.com/jermur788/CCF/issues/466).
**Outcome in one line:** Phases A and B are complete offline. Phase C (Unity witness) was **NOT RUN**, for the reasons in §8.

**Worker (actual):** Anthropic **Claude Opus 5.5** (`--model=claude-opus-5-5`, `--effort=medium`, read from its own process command line), Claude Code in local T3 Code, "Claude B — Pro". Native sub-agent under a Claude Fable 5.2 T3 orchestrator, on the owner's Linux host.

**Authority:**
- live board `origin/main` @ `fc18190b` line 3;
- [owner delegation](https://github.com/jermur788/CCF/issues/466#issuecomment-6863557282);
- [written release](https://github.com/jermur788/CCF/issues/466#issuecomment-6863588469);
- binding packet `MANAGER-FIVE-RACK-NATIVE-EVIDENCE-ACQUISITION-PACKET-20261113.md` and supplemental dispatch `MANAGER-466-T3-CODE-NATIVE-UNITY-AGENT-DISPATCH-20261113.md`.

**Lane and evidence locations:**
- **Research branch:** `research/multirack-five-rack-native-evidence-20261113-v1`, start HEAD `ee457db344c8d3618e34f61dc5694550fc482ecc`.
- **Worktree:** `/media/jer/ZX20/CCF-MultiRackNativeEvidence-20261113`.
- **Evidence root (off-repo):** `/media/jer/Files/CCF/Verification/MultiRackNativeEvidence466-20261113T201043Z/`.

**Scope of this report:**
- This is research evidence only. Nothing here is an accepted specification, gameplay calibration, Unity feature qualification or integration verdict.
- **Objective 2 OPEN-1..OPEN-6 and compact OPEN-C1..OPEN-C8 all remain undecided.**
- No source, Unity asset, scene, spec, Decision Log, milestone, coordination file or save was edited.

**Labels used for every material number:**
- **[obs]** observed: read directly from a save file.
- **[der]** pure-derived: deterministic arithmetic on observed data plus source constants.
- **[hyp]** hypothetical: a projection under a stated rule the game does not implement.
- **[syn]** synthetic: authored test input.
- **[cal]** old gameplay calibration: a candidate setting only, not a forestry fact.

---

## 0. Headline results

1. **The world and candidate are identified and their provenance is checked [obs].**
   - The pinned fixture `D073-FR10-427-20261112/candidate1/.../forest-save.json` has sha256 `1a32513112e1fd43dd29af8c8ffbe4ca40cdb5738e7af89a260fbf9707dcb670`, which matches the reported prefix and suffix.
   - It is v24, Enlarged80, Year 6, with 1,344 tree records: 1,171 living Sitka spruce and 173 stumps.
   - It has five confirmed Rack V2 racks (ids 1–5), all entrances marked, 175 resolved orders, 537 events, six annual reports and three Crop Trees (P0706, P0714, P1109).
   - It is a **disposable verification fixture, not the owner's personal world.** The personal D-073 playtest world was never saved: the personal profile holds no `forest-save.json`, re-checked at 20:10Z and 20:27Z.
2. **Five racks, net standing area and density [obs, der].**
   - The racks are parallel and east–west, running x = −40 → +40 m, with a 2.4 m corridor (source constant; the save holds no width).
   - Under the source's own midpoint-strip rule (S0), net standing area per whole rack is **870 / 984 / 1,004 / 960 / 997 m²** for racks 1–5.
   - Living stems per rack are **203 / 216 / 234 / 249 / 217**, giving **2,334 / 2,194 / 2,332 / 2,594 / 2,176 stems per net ha** and **62.2 / 56.4 / 59.1 / 67.0 / 57.3 m² BA per net ha**.
   - The exclusion union (corridors, headland/feeder/pads, 104 track circles, clearing, 6 sites) covers 1,435.7 m² (22.4 % of the 6,400 m² property). Counting the masks without taking their union would double-count 99.2 m².
3. **The disjoint ledger reconciles exactly [der].**
   - In each of five schemes, all 1,171 living trees fall in exactly one bucket: rack + side (+ tile), outer remainder, or end remainder.
   - There are **0 duplicates, 0 unassigned and 0 exact boundary ties.** **0 living stems** lie inside any mask.
   - Areas sum to 6,400.000 m² with zero error.
   - Recomputed property BA/ha is 47.751045, against the save's Year-6 snapshot of 47.75103 (float32).
4. **Executed-work provenance holds for all 175 orders, but #418 cannot be exercised here [obs].**
   - Every order is traced through workOrderId → OrderCreated → OrderApproved → WorkResolved (Succeeded) → the target's current stump state.
   - The 175 orders are 159 rack-access orders, 14 release fellings and 2 plantings. 159/159 rack orders target a tree in that rack's frozen removal list.
   - The fixture holds **no prescriptions** (`prescriptions=[]`, `nextPrescriptionId=1`), so `prescriptionId=0` on every order and event is the correct value here. This fixture **cannot demonstrate #418 either way.**
   - Static reading of source at `55863f1b` confirms the defect path is still present: `RecordOrderEvent` (ScenarioOneManager.cs:2466-2487) never copies `prescriptionId`, and approval (:1258) and resolution (:1325) events go through it. §6.1 has the details.
5. **Positive executed release is proven for the teaching bay only, not for a whole source rack [obs, der].**
   - The Year-6 fixture holds **no** pre-treatment state.
   - However, the #417 run that produced it captured `stage-3.json` (Year 1, Crop Trees marked, the 14 release orders Pending). This capture is proven to be a strict history prefix of the fixture: identical tree ids and positions, events 1–493 byte-equal, the same racks and Crop Trees.
   - With those authentic pre-treatment denominators (Hegyi, 8 m, recomputed), the in-bay release shares are **P0706 0.3424, P1109 0.3435, P0714 0.3562**: r = 3, min 0.3424, lower quartile 0.3430, median 0.3435.
   - The 14 executed stems are counted once. Two of them (P0807, P0908) served two Crop Trees each.
   - The release covers only the **24 × 18 m teaching bay on rack 3**, not rack 3 as a whole. Under the packet's "one whole source rack" requirement, every destination is therefore **InsufficientSourceEvidence** (§6.3).
6. **Similarity-only sensitivity (candidate thresholds, not policy) [der, cal]:**
   - Against rack 3 as source, under the old MFM-03 settings (20 % density / 15 % mean DBH / 20 % mean competition) with a 20-stem minimum, **every destination rack is Partial** in both Year 1 and Year 6.
   - With the old 30-stem minimum, **most 20 m tiles are underpowered (Manual)**: 20 of the 32 destination tiles hold fewer than 30 stems.
   - Two tiles change classification between Year 1 and Year 6, so classification is time-dependent.
7. **Hypothetical queues and BA caps (projection only; no engine exists) [hyp].**
   - Copying the source's share (≈0.343) to destination tiles with a largest-DBH greedy candidate rule gives:
     - **10–11 Crop Tree candidates and 58–73 unique removals per rack** at 16 m spacing, i.e. 69–84 individual confirmations;
     - **21–22 candidates and 112–137 removals** at 8 m spacing.
   - At 16 m spacing **2–3 tiles per rack exceed the 40 % BA safeguard and all 8 exceed 20 %**. At 8 m spacing 6–8 tiles exceed 40 %.
   - By contrast, the actual source release removed 23.8 % of the bay's pre-treatment BA and 7.2 % of rack 3's.
   - These are not observed workloads and not gameplay-tested.
8. **Phase C NOT RUN.** A/B left no gap that a minimal disposable normal-work witness could answer without manufacturing player choices (§8).

---

## 1. Inputs, identity and provenance

| Item | Value | Label |
|---|---|---|
| Pinned source baseline | D-073 `55863f1b7719011bba16b3f3ab377e379c0b988a` (read via `git show` / `git grep` only; clean lane `CCF-S1-ResearchCopy-V1-Qual` untouched) | — |
| Fixture (original, never opened for write) | `/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/candidate1/config/unity3d/DefaultCompany/CCF/forest-save.json`, 2,242,732 B, sha256 `1a32513112e1fd43dd29af8c8ffbe4ca40cdb5738e7af89a260fbf9707dcb670` (pre and post identical) | obs |
| Working copy | `fixtures/primary/forest-save.candidate1.json` in the evidence root, read-only mode, same sha256 | obs |
| #427 runner identity | `source-identity.json`: runner `87405a562f1973de80e1c1830df1765368ca72b4`, candidate tip `55863f1b`. **Re-verified:** `git diff 87405a56 55863f1b -- Assets` is **empty (0 lines)** | obs |
| How the world was actually made | The #427 FR10 harness (`Fr10RenderedOracle.cs` @ 87405a56) **loads** `CCF_D073_RESUME_HISTORY` = #417 `runtime1/evidence/skip-final.json` (sha256 `cae00068…25b0`, recorded in `fixture-provenance.json`). The candidate1 save is world-identical to that file: all 1,344 tree records and every scenarioOne history list are equal; only list order in `trees`, `buildables` and `storages` differs. | obs |
| Origin of that history | #417 `D073Verification.cs` harness at source **`77a56a44`**. `git diff 77a56a44 55863f1b -- Assets` touches only `UI/AnnualReviewView.cs` (+42 lines, UI). The harness drove the **real** manager and UI API (`AddRack`, `ConfirmRackPlan`, `MarkRackEntrance`, `ApproveWorkPlan`, `AdvanceFromWorkPlan`). **The choices were scripted, not made by a human:** layout lines from `RackPlanner.LayoutLines`; Crop Trees hard-coded `P0714`, `P1109`, `P0706`; competitors added greedily by Hegyi term until the bay minimum share was ≥ 0.30. | obs (source read) |
| Pre-treatment capture | #417 `runtime1/evidence/stage-3.json`, sha256 `116a7acf08dbd7c53cce32a90bda6abd949535a0da08d53e109b3ecd2232c27c`. Year 1, 1,185 living, crop marks set, 14 release orders status Pending. **Lineage proof:** same tree ids, identical positions, events 1–493 byte-equal to candidate1's, same racks, crops and seed, and every stage-3 order id has the same target in candidate1. | obs |
| World facts | v24; `standGeometryModel` 1 (Enlarged80, 80 × 80 m, 256 cells of 5 m); `growthModel` 2; `regenerationModel` 2; seed 20260914; `rackPlanningModel` 1 (V2); `rackPlanConfirmed` true; cash 7,867.78 (cents 786,778) | obs |
| Unity version | No Editor was run. The project pins 6000.6.0f1; this is not observed here. | — |

**Not the user's world:** the fixture is the disposable #427/#417 verification world. It must not be described as the owner's personal playtest. That personal world was never saved and cannot be reconstructed or attributed.

---

## 2. Rack geometry and authoritative masks (Phase A)

### 2.1 Racks [obs]

| Rack id | Entrance (x, z) | Far (x, z) | Length m | Entrance tree | Entrance marked | Frozen removal list |
|---:|---|---|---:|---|---|---:|
| 1 | (−40, −31.400) | (40, −31.400) | 80 | PO0400 | yes | 30 |
| 2 | (−40, −16.038) | (40, −16.038) | 80 | PO1200 | yes | 20 |
| 3 | (−40, 0.028) | (40, 0.028) | 80 | PO2000 | yes | 36 |
| 4 | (−40, 16.017) | (40, 16.017) | 80 | PO2800 | yes | 38 |
| 5 | (−40, 31.346) | (40, 31.346) | 80 | PO3601 | yes | 35 |

**Derived spacing [der]:**
- Interior centreline gaps are 15.362 / 16.066 / 15.989 / 15.329 m.
- The midpoints between racks are at z = −23.719 / −8.005 / 8.022 / 23.682.

**Corridor width:**
- The width is **2.4 m**: `RackGeometry.DefaultWidthM` (RackGeometry.cs:53), used by every V2 order path (ScenarioOneManager.RackPlan.cs:383/455/528).
- `RackSaveData` (ForestSaveData.cs:129-145) stores no width.

**Display ordinals:** none are used. Identity is the stable `rackId`, ordered by z, which equals id order here.

### 2.2 Masks (union subtracted once) [obs source + der areas]

The mask definitions follow the MFM-08A world adapter (`ScenarioOneCoverageWorld.cs:7-11`) and its authoritative owners:

| Mask | Authority (55863f1b) | Geometry used | Area in property m² |
|---|---|---|---:|
| Corridors | RackGeometry.cs:53 | 5 × (80 × 2.4 m) | 960.000 |
| Headland strip, feeder and pads | ScenarioOneHeadlandGeometry.cs:7-23 (stem-centre rule, guard 0.15 m) | strip x = −35.4, z ±37.7, radius 1.45; feeder z = −27.4 from x −35.4 to −2.436, radius 1.45; pads radius 2.35 | 330.144 |
| Track | `Dirt Path*` planks, radius `pathCorridorRadiusMeters` 1.5 (ForestTest.unity:5940) | 104 circles | 161.822 |
| Clearing | "Forest Clearing", radius `clearingRadiusMeters` 2.2 (ForestTest.unity:5941) | 1 circle at (−2.772, 11.000) | 15.205 |
| Sites | active, non-retired `ForestBuildable`, radius 2.2 | 6 circles (Forestry Workbench excluded: `retiresFromStartingWorld: 1`, ForestTest.unity:5680) | 67.778 |
| **Sum if double-counted** | | | **1,534.949** |
| **Union (used)** | | | **1,435.716** |
| Union with visual (unguarded) headland width | sensitivity only | | 1,406.145 |

**Where the plank, clearing and site positions come from:**
- They are the #248 live-scene measurement `ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/scenery.json` (sha256 `38db90ff…039eed`), taken at source `72b4efbd`.
- They carry over to `55863f1b` because `git diff 72b4efbd 55863f1b` on `ForestTest.unity` adds only that one `retiresFromStartingWorld: 1` line. `StandApproachRoute.cs` and `StandApproachPath.cs` are unchanged.
- The generator's clearance predicate now skips retired buildables (ForestStartingStand.cs:340). The retired workbench circle lies almost entirely inside the clearing circle.
- **Empirical cross-check:** zero of the 1,171 living stem centres lie in any mask.
- This carry-over is **derived, not re-measured in a live 55863f1b scene**.

**Absent from data and source (treated as UNKNOWN, not similar):**
- **soil, moisture, wind exposure, slope, machine access and legal access:** no such field in `ForestSaveData.cs`, and no source producer;
- **protected areas:** `protectedAreas=[]`, and Scenario One has no live producer (ScenarioOneCoverageWorld.cs:38);
- **shelters, clearance patches and planted juveniles:** all empty.

Walkability was not treated as machine access.

### 2.3 Candidate schemes (sensitivity variants; OPEN-1 not accepted)

Common rules for all schemes:
- **Side naming:** S = right of travel (south, −z) and N = left (north, +z). Direction of travel is entrance (x = −40) → far (x = +40).
- **Half-open ties:** z bands are `[lo, hi)` and x tiles are `[a, b)`, with the last tile closed at +40.
- **Membership order:** masks first, with fixed precedence corridor > headland > track > clearing > site.

The schemes:
- **S0, midpoint strips with explicit outer remainders.** This is **identical to the source's own coverage-strip rule** (ScenarioOneRackSampleGeometry.cs:275-296). Outer sides mirror the adjacent half-gap. The strips beyond them are **OUTER-S** (z −40 → −39.081) and **OUTER-N** (z 39.011 → 40), both Manual/UNKNOWN.
- **S1, outer sides run to the property edge** (the nearest rack owns the perimeter strip). It also has an explicit **west end cap**: everything west of the guarded headland edge (x < −33.95) is an **END-W** remainder (UNKNOWN), because the strip west of the headland is reached only across the headland.
- **Tilings:**
  - **T4:** four 20 m along-rack bands × 2 sides = 8 tiles per rack. This is the #464 illustration, *not accepted*.
  - **T3:** three 26.67 m bands.
  - **T1:** whole side.
- **The historical 15-area MFM sample ids are not used or relabelled.** The teaching bay below is described only as the treated footprint.

**Area self-check [der]:** in every scheme (S0T4, S0T3, S0T1, S1T4, S1T1), units + remainders + MASKED sum to **6,400.000 m²** with error 0.000 m². Integration is exact in x, with 0.0025 m slices in z.

---

## 3. Disjoint tree membership ledger (Phase A)

| Check | Result | Label |
|---|---|---|
| Tree records | 1,344 = 1,171 living (stage 0, not dead) + 173 stumps (stage 1) | obs |
| Save Year-6 snapshot `livingTrees` | 1,171 (matches) | obs |
| Living trees assigned per scheme | 1,171 in each of S0T4, S0T3, S0T1, S1T4, S1T1 | der |
| Duplicate memberships / duplicate ids | 0 / 0 | der |
| Unassigned | 0 | der |
| Exact boundary ties (\|d\| < 1e-9 m) | 0 | der |
| Living stems inside any mask | 0 | der |
| S0 split | 1,119 in racks + 52 outer remainder (OUTER-S 24, OUTER-N 28) | der |
| S1 split | 1,171 in racks, of which 64 are END-W remainder (5–9 per rack-side) | der |
| Stumps by S0T4 unit | 159 corridor (MASKED); release stumps R3-S-1 6, R3-S-2 5, R3-N-1 2, R2-N-1 1 | der |

**Full per-tree ledger:**
- `phaseA/phaseA-tree-ledger.csv` (scheme, treeId, x, z, unit, mask type);
- `phaseA/phaseA-trees-year6.csv` (all 1,344 records with DBH, BA, crown, mark, crop, recomputed Hegyi, cell, unit).

**Cells:**
- Ecology cells are not split or duplicated; the ledger is by stem centre only.
- The save stores 149 regeneration cells: cohorts are Sitka in 326 cohort entries, beech 1, sessile oak 1.
- Regeneration cohorts are reported stand-wide only, not per rack.

---

## 4. Per-rack, per-side and per-tile inventory (Phase A, Year 6)

**How these values are produced:**
- BA uses π·(DBH_cm/200)² per living stem. Density and BA/ha use **net standing hectares** after the mask union.
- **Hegyi competition is recomputed in float64 from the source formula, not saved:** ForestEcologyController.cs:912 (8 m cutoff), :923-926 (term), and :953-955 (stumps excluded).
- It is computed over **all living neighbours stand-wide, never cut at ownership boundaries**.

**Uniform across every unit:**
- species is Sitka spruce only;
- age is 26 years;
- there are no Fell marks and no pending or approved work.

**Juveniles, deadwood and protection:**
- juveniles 0; protection 0;
- deadwood 1 record (DW0001 from P0707, fallen Year 2, in R3-S-1).

### T-A2 Per rack and side (S0, midpoint strips = source coverage-strip rule), Year 6

| Unit | Gross m² | Masked m² | Net m² | Net ha | Living stems | Stems / net ha | BA m² | BA m²/net ha | DBH p10 / median / p90 cm | Mean DBH cm | Recomputed Hegyi CI mean / p90 | Crop Trees |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---|---:|---|---|
| OUTER-N | 79.2 | 0.1 | 79.1 | 0.0079 | 28 | 3541 | 0.855 | 108.13 | 14.4 / 19.1 / 24.0 | 19.38 | 4.87 / 6.69 | — |
| OUTER-S | 73.4 | 2.8 | 70.6 | 0.0071 | 24 | 3398 | 0.670 | 94.84 | 16.9 / 18.8 / 20.1 | 18.80 | 4.65 / 5.48 | — |
| R1-N-0 | 614.4 | 226.0 | 388.4 | 0.0388 | 96 | 2472 | 2.614 | 67.30 | 13.9 / 18.1 / 22.4 | 18.33 | 7.26 / 10.00 | — |
| R1-S-0 | 614.6 | 133.4 | 481.2 | 0.0481 | 107 | 2224 | 2.792 | 58.02 | 13.9 / 17.8 / 22.3 | 17.93 | 6.44 / 8.58 | — |
| R2-N-0 | 642.6 | 138.5 | 504.1 | 0.0504 | 113 | 2242 | 2.938 | 58.27 | 14.5 / 18.1 / 21.3 | 18.04 | 7.39 / 9.59 | — |
| R2-S-0 | 614.6 | 134.3 | 480.3 | 0.0480 | 103 | 2144 | 2.616 | 54.46 | 14.7 / 18.1 / 20.9 | 17.82 | 7.38 / 9.67 | — |
| R3-N-0 | 639.6 | 142.1 | 497.5 | 0.0497 | 117 | 2352 | 2.910 | 58.50 | 13.2 / 17.5 / 21.9 | 17.55 | 8.08 / 10.67 | P1109 |
| R3-S-0 | 642.6 | 136.6 | 506.0 | 0.0506 | 117 | 2312 | 3.016 | 59.61 | 14.1 / 17.7 / 22.0 | 17.88 | 7.64 / 10.68 | P0706, P0714 |
| R4-N-0 | 613.2 | 114.7 | 498.5 | 0.0498 | 134 | 2688 | 3.441 | 69.03 | 13.6 / 18.0 / 21.8 | 17.83 | 8.51 / 10.87 | — |
| R4-S-0 | 639.6 | 178.3 | 461.3 | 0.0461 | 115 | 2493 | 2.991 | 64.84 | 13.4 / 17.7 / 22.3 | 17.89 | 8.39 / 11.79 | — |
| R5-N-0 | 613.0 | 114.1 | 498.9 | 0.0499 | 104 | 2085 | 2.775 | 55.63 | 15.0 / 18.4 / 21.4 | 18.29 | 6.68 / 8.17 | — |
| R5-S-0 | 613.2 | 114.7 | 498.5 | 0.0498 | 113 | 2267 | 2.943 | 59.05 | 15.0 / 18.0 / 21.2 | 18.06 | 7.91 / 9.51 | — |
| MASKED (union, once) | — | 1435.7 | 0 | — | 0 | — | — | — | — | — | — | — |

### T-A3 Whole rack (both sides, S0), Year 6

| Rack (id) | Centreline z | Net m² | Net ha | Stems | Stems / net ha | BA m² | BA m²/net ha |
|---|---:|---:|---:|---:|---:|---:|---:|
| 1 | -31.400 | 869.6 | 0.0870 | 203 | 2334 | 5.406 | 62.16 |
| 2 | -16.038 | 984.4 | 0.0984 | 216 | 2194 | 5.553 | 56.41 |
| 3 | 0.028 | 1003.5 | 0.1003 | 234 | 2332 | 5.927 | 59.06 |
| 4 | 16.017 | 959.7 | 0.0960 | 249 | 2594 | 6.432 | 67.02 |
| 5 | 31.346 | 997.3 | 0.0997 | 217 | 2176 | 5.718 | 57.34 |

### T-A4 Per tile (S0 × T4: four 20 m along-rack bands × 2 sides), Year 6

| Tile | Gross m² | Net m² | Net/gross | Stems | Stems / net ha | BA m²/net ha | DBH median cm | Hegyi CI mean | Masks in tile (m²) | Crop |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---|---|
| R1-N-0 | 153.6 | 67.9 | 0.44 | 19 | 2800 | 84.51 | 19.4 | 5.34 | corridor 24.0; headland 65.2 | — |
| R1-N-1 | 153.6 | 61.3 | 0.40 | 17 | 2771 | 74.49 | 18.1 | 5.92 | corridor 24.0; headland 61.9; track 22.9 | — |
| R1-N-2 | 153.6 | 129.6 | 0.84 | 32 | 2469 | 62.08 | 17.7 | 8.52 | corridor 24.0 | — |
| R1-N-3 | 153.6 | 129.6 | 0.84 | 28 | 2160 | 60.10 | 19.2 | 7.94 | corridor 24.0 | — |
| R1-S-0 | 153.7 | 111.6 | 0.73 | 27 | 2419 | 61.26 | 17.8 | 6.21 | corridor 24.0; headland 21.5 | — |
| R1-S-1 | 153.7 | 110.3 | 0.72 | 24 | 2176 | 56.84 | 17.4 | 5.95 | corridor 24.0; track 22.9 | — |
| R1-S-2 | 153.7 | 129.7 | 0.84 | 30 | 2314 | 66.03 | 19.5 | 6.85 | corridor 24.0 | — |
| R1-S-3 | 153.7 | 129.7 | 0.84 | 26 | 2005 | 48.23 | 16.5 | 6.67 | corridor 24.0 | — |
| R2-N-0 | 160.7 | 116.8 | 0.73 | 28 | 2397 | 61.37 | 17.9 | 7.71 | corridor 24.0; headland 23.3 | — |
| R2-N-1 | 160.7 | 130.0 | 0.81 | 28 | 2154 | 57.16 | 18.4 | 6.87 | corridor 24.0; track 10.2 | — |
| R2-N-2 | 160.7 | 120.7 | 0.75 | 24 | 1989 | 53.62 | 18.1 | 6.18 | corridor 24.0; track 16.5 | — |
| R2-N-3 | 160.7 | 136.7 | 0.85 | 33 | 2415 | 60.80 | 17.9 | 8.43 | corridor 24.0 | — |
| R2-S-0 | 153.7 | 110.9 | 0.72 | 30 | 2706 | 66.85 | 18.0 | 7.30 | corridor 24.0; headland 22.3 | — |
| R2-S-1 | 153.7 | 110.2 | 0.72 | 20 | 1815 | 47.72 | 17.8 | 6.25 | corridor 24.0; track 23.3 | — |
| R2-S-2 | 153.7 | 129.7 | 0.84 | 20 | 1543 | 39.50 | 18.3 | 7.27 | corridor 24.0 | — |
| R2-S-3 | 153.7 | 129.7 | 0.84 | 33 | 2545 | 64.55 | 18.1 | 8.21 | corridor 24.0 | — |
| R3-N-0 | 159.9 | 116.2 | 0.73 | 28 | 2410 | 58.41 | 17.2 | 7.72 | corridor 24.0; headland 23.2 | — |
| R3-N-1 | 159.9 | 121.6 | 0.76 | 25 | 2056 | 56.06 | 18.2 | 7.75 | corridor 24.0; site 5.4; track 11.9 | P1109 |
| R3-N-2 | 159.9 | 123.8 | 0.77 | 33 | 2666 | 66.12 | 18.1 | 8.74 | corridor 24.0; site 1.5; track 14.7 | — |
| R3-N-3 | 159.9 | 135.9 | 0.85 | 31 | 2281 | 53.81 | 17.0 | 7.98 | corridor 24.0 | — |
| R3-S-0 | 160.7 | 116.8 | 0.73 | 29 | 2482 | 70.77 | 18.3 | 6.96 | corridor 24.0; headland 23.3 | — |
| R3-S-1 | 160.7 | 136.7 | 0.85 | 30 | 2195 | 52.93 | 17.3 | 7.51 | corridor 24.0 | P0706 |
| R3-S-2 | 160.7 | 115.9 | 0.72 | 26 | 2243 | 50.49 | 17.2 | 8.14 | corridor 24.0; track 24.5 | P0714 |
| R3-S-3 | 160.7 | 136.7 | 0.85 | 32 | 2342 | 64.48 | 18.1 | 7.99 | corridor 24.0 | — |
| R4-N-0 | 153.3 | 110.6 | 0.72 | 30 | 2714 | 67.14 | 17.1 | 7.61 | corridor 24.0; headland 22.2 | — |
| R4-N-1 | 153.3 | 129.3 | 0.84 | 34 | 2630 | 67.88 | 18.2 | 9.20 | corridor 24.0 | — |
| R4-N-2 | 153.3 | 129.3 | 0.84 | 38 | 2939 | 80.08 | 18.9 | 9.11 | corridor 24.0 | — |
| R4-N-3 | 153.3 | 129.3 | 0.84 | 32 | 2475 | 60.73 | 17.2 | 7.89 | corridor 24.0 | — |
| R4-S-0 | 159.9 | 116.2 | 0.73 | 26 | 2238 | 62.38 | 17.8 | 6.92 | corridor 24.0; headland 23.2 | — |
| R4-S-1 | 159.9 | 94.2 | 0.59 | 28 | 2974 | 70.43 | 16.7 | 9.25 | clearing 15.2; corridor 24.0; site 39.8; track 12.1 | — |
| R4-S-2 | 159.9 | 115.0 | 0.72 | 32 | 2782 | 69.76 | 17.3 | 9.39 | corridor 24.0; site 21.1 | — |
| R4-S-3 | 159.9 | 135.9 | 0.85 | 29 | 2134 | 58.92 | 19.1 | 7.77 | corridor 24.0 | — |
| R5-N-0 | 153.2 | 111.1 | 0.73 | 21 | 1890 | 52.93 | 18.5 | 5.74 | corridor 24.0; headland 21.6 | — |
| R5-N-1 | 153.2 | 129.2 | 0.84 | 28 | 2166 | 58.24 | 18.6 | 7.30 | corridor 24.0 | — |
| R5-N-2 | 153.2 | 129.2 | 0.84 | 27 | 2089 | 53.41 | 18.2 | 6.90 | corridor 24.0 | — |
| R5-N-3 | 153.2 | 129.2 | 0.84 | 28 | 2166 | 57.56 | 18.3 | 6.56 | corridor 24.0 | — |
| R5-S-0 | 153.3 | 110.6 | 0.72 | 26 | 2352 | 66.33 | 18.6 | 6.91 | corridor 24.0; headland 22.2 | — |
| R5-S-1 | 153.3 | 129.3 | 0.84 | 30 | 2320 | 58.23 | 18.2 | 8.71 | corridor 24.0 | — |
| R5-S-2 | 153.3 | 129.3 | 0.84 | 31 | 2398 | 57.71 | 17.3 | 8.57 | corridor 24.0 | — |
| R5-S-3 | 153.3 | 129.3 | 0.84 | 26 | 2011 | 54.98 | 18.4 | 7.21 | corridor 24.0 | — |
| OUTER-S (remainder, Manual/UNKNOWN) | 73.4 | 70.6 | 0.96 | 24 | 3398 | 94.84 | 18.8 | 4.65 | — | — |
| OUTER-N (remainder, Manual/UNKNOWN) | 79.2 | 79.1 | 1.00 | 28 | 3541 | 108.13 | 19.1 | 4.87 | — | — |

## 5. Alternative end and tile schemes (sensitivity, Phase A)

### T-A5 Alternative scheme S1 (outer sides to property edge; west end cap beyond headland = END-W remainder), per rack-side

| Unit | Net m² | Stems | Stems / net ha | BA m²/net ha |
|---|---:|---:|---:|---:|
| R1-N-0 | 370.3 | 91 | 2457 | 64.90 |
| R1-N-END-W | 18.1 | 5 | 2764 | 116.32 |
| R1-S-0 | 525.1 | 122 | 2323 | 62.65 |
| R1-S-END-W | 26.7 | 9 | 3374 | 64.34 |
| R2-N-0 | 482.6 | 108 | 2238 | 57.75 |
| R2-N-END-W | 21.5 | 5 | 2323 | 70.09 |
| R2-S-0 | 459.9 | 96 | 2087 | 53.24 |
| R2-S-END-W | 20.4 | 7 | 3428 | 81.95 |
| R3-N-0 | 476.1 | 112 | 2353 | 58.89 |
| R3-N-END-W | 21.4 | 5 | 2336 | 49.90 |
| R3-S-0 | 484.5 | 110 | 2270 | 56.89 |
| R3-S-END-W | 21.5 | 7 | 3252 | 120.79 |
| R4-N-0 | 478.1 | 126 | 2636 | 69.04 |
| R4-N-END-W | 20.4 | 8 | 3928 | 68.82 |
| R4-S-0 | 439.9 | 109 | 2478 | 62.67 |
| R4-S-END-W | 21.4 | 6 | 2803 | 109.57 |
| R5-N-0 | 551.1 | 126 | 2286 | 62.61 |
| R5-N-END-W | 26.8 | 6 | 2235 | 67.02 |
| R5-S-0 | 478.1 | 107 | 2238 | 57.37 |
| R5-S-END-W | 20.4 | 6 | 2946 | 98.48 |

### T-A6 Alternative tiling S0 × T3 (three 26.67 m bands), per tile

| Tile | Net m² | Stems | Stems / net ha | BA m²/net ha | DBH median |
|---|---:|---:|---:|---:|---:|
| R1-N-0 | 91.7 | 27 | 2944 | 82.46 | 17.9 |
| R1-N-1 | 123.9 | 31 | 2503 | 71.64 | 18.4 |
| R1-N-2 | 172.8 | 38 | 2199 | 56.13 | 17.3 |
| R1-S-0 | 154.8 | 38 | 2455 | 66.98 | 18.3 |
| R1-S-1 | 153.5 | 32 | 2085 | 49.37 | 16.4 |
| R1-S-2 | 172.9 | 37 | 2140 | 57.68 | 18.1 |
| R2-N-0 | 162.4 | 37 | 2279 | 60.46 | 18.3 |
| R2-N-1 | 159.5 | 35 | 2194 | 57.52 | 18.3 |
| R2-N-2 | 182.2 | 41 | 2250 | 56.99 | 17.9 |
| R2-S-0 | 154.1 | 39 | 2531 | 62.45 | 17.7 |
| R2-S-1 | 153.4 | 22 | 1434 | 39.21 | 18.5 |
| R2-S-2 | 172.9 | 42 | 2430 | 60.87 | 18.1 |
| R3-N-0 | 161.5 | 39 | 2415 | 63.85 | 17.7 |
| R3-N-1 | 154.8 | 33 | 2132 | 47.05 | 17.0 |
| R3-N-2 | 181.2 | 45 | 2483 | 63.52 | 17.7 |
| R3-S-0 | 162.4 | 42 | 2586 | 68.42 | 17.8 |
| R3-S-1 | 161.4 | 29 | 1796 | 46.77 | 17.7 |
| R3-S-2 | 182.2 | 46 | 2525 | 63.13 | 17.5 |
| R4-N-0 | 153.7 | 44 | 2864 | 76.65 | 18.5 |
| R4-N-1 | 172.4 | 45 | 2610 | 61.91 | 17.1 |
| R4-N-2 | 172.4 | 45 | 2610 | 69.35 | 18.4 |
| R4-S-0 | 161.5 | 40 | 2477 | 61.60 | 16.8 |
| R4-S-1 | 118.6 | 32 | 2698 | 80.02 | 19.5 |
| R4-S-2 | 181.2 | 43 | 2373 | 57.80 | 17.4 |
| R5-N-0 | 154.2 | 31 | 2010 | 55.28 | 18.5 |
| R5-N-1 | 172.3 | 38 | 2205 | 58.03 | 18.3 |
| R5-N-2 | 172.3 | 35 | 2031 | 53.55 | 18.4 |
| R5-S-0 | 153.7 | 39 | 2538 | 67.62 | 18.1 |
| R5-S-1 | 172.4 | 38 | 2204 | 56.57 | 18.1 |
| R5-S-2 | 172.4 | 36 | 2088 | 53.87 | 17.7 |

**Notes on the tables above:**
- **Edge remainders look denser [der].**
  - OUTER-S/N and END-W show inflated density and BA/ha (3,400–3,900 stems/ha; 64–121 m²/ha) on tiny areas.
  - The cause is that the generator clamps outer-lattice stems to the ±39.4 m margin (ForestStartingStand.cs:179-300, `OuterAxis`). These are geometry artefacts of small denominators, not ecological edge effects.
- **Rack 1's north side is infrastructure-dominated.** The southern feeder (z = −27.4) and its pads fall inside the R1-N strip, so R1-N carries 127 m² of headland/feeder mask.
- **Rack 4's south side holds the work-area clearing and sites.**
- **Hegyi values are for comparison only.** They are relative within this world, not calibrated competition facts.

---

## 6. Phase B — provenance, release, comparability, workload and caps

### 6.1 Executed-work provenance and the #418 audit [obs]

All 175 orders have a complete chain: OrderCreated (event 1) → OrderApproved (2) → one WorkResolved (4) with outcome Succeeded. Each target's current state is consistent with that chain (`phaseB/provenance-orders.csv`).

| Class | Orders | Created → resolved | Detail |
|---|---:|---|---|
| Rack access (rackId 1–5) | 159 | Year 0 → Year 1 | 159/159 targets in that rack's frozen `removalTreeIds`; all are now stumps; sold (`TreeFelledAndExtracted`) |
| Release fell (rackId 0) | 14 | Year 1 → Year 2 | 13 sold, 1 retained as fallen deadwood (P0707 → DW0001); all 14 targets are now stumps |
| Planting | 2 | Year 2 → Year 3 | beech and sessile oak, one each |
| Open (Pending/Approved) | 0 | — | no committed future work |

**#418 finding (stated as observed):**
- Every order `prescriptionId` = 0 and every event `prescriptionId` = 0.
- The save has **no prescriptions at all** (`prescriptions=[]`, `nextPrescriptionId=1`), so 0 is the correct value in this fixture.
- **The fixture therefore cannot show whether #418 is fixed or present.**
- Static source reading at `55863f1b`:
  - `RecordOrderEvent` (ScenarioOneManager.cs:2466-2487) builds the event without `prescriptionId`.
  - `OrderApproved` (:1258) and `WorkResolved` (:1325) are recorded through it.
  - Only the OrderCreated event of a prescription-generated order is patched afterwards (ScenarioOneManager.PrescriptionApproval.cs:196).
  - So approval and resolution events of prescription work would carry 0. This is a **static reading, not exercised**.
- Lineage in this report uses `workOrderId` throughout. #418 is **not asserted fixed**.

### 6.2 Positive executed release with authentic pre-treatment denominators [obs, der]

**Source of the denominators:**
- They come from #417 `stage-3.json`, which is proven to be in the fixture's own lineage (§1).
- The capture was taken in Year 1, with the Crop Trees marked and the 14 release orders Pending.
- Hegyi is recomputed from the source formula over all living neighbours within 8 m.

**Teaching-bay footprint:** the third rack (rack 3), anchor (0, 0.028) as the point nearest the origin (ScenarioOneRackSampleGeometry.cs:262-271), ±12 m along, ±9 m across, minus all corridors (ScenarioOneRackBay.cs:35-38, 141-171).

| Crop Tree | Unit (S0T4) | DBH at stage-3 cm | Pre-treatment CI | Executed competitor terms | Release share (in bay = physical) | Executed competitors (distance m) | DBH Year 6 cm |
|---|---|---:|---:|---:|---:|---|---:|
| P0706 | R3-S-1 | 20.945 | 7.4517 | 2.5515 | **0.3424** | P0707 (1.54), P0806 (1.37), P0606 (2.51), P0807 (2.22), P0705 (2.43), P0908 (5.72) | 23.412 |
| P1109 | R3-N-1 | 17.932 | 3.7071 | 1.2734 | **0.3435** | P1108 (1.67), P1208 (3.40), P0908 (4.01), P0910 (4.14), P0807 (7.34) | 21.328 |
| P0714 | R3-S-2 | 21.026 | 6.4226 | 2.2880 | **0.3562** | P0715 (1.67), P0614 (1.87), P0713 (2.01), P0814 (2.33), P0815 (2.38) | 23.699 |

**Release statistics:**
- Positive releases **r = 3**. The candidate r ≥ 3 minimum is **unratified** and only just met.
- Min 0.3424 / lower quartile 0.3430 / median 0.3435 / max 0.3562. The spread of the three statistics is 0.0011.
- Leave-one-out: the min stays within 0.3424–0.3435 and the median within 0.3430–0.3499. With N = 3, small-N sensitivity cannot be estimated meaningfully.
- **Check against the harness:** recomputing the source's PlannedShare at stage-3 gives a minimum of 0.3424 ≥ 0.30, which reproduces the #417 harness stop rule (`PlanRelease` loops until `MinPlannedAndExecuted ≥ FirstReleaseShare 0.30`).

**Physical ledger, each stem counted once:**
- There are **14 unique executed stems, with 0.3609 m² BA at stage-3 DBH**.
- There are 16 Crop-Tree links, because **P0807 and P0908 each served both P0706 and P1109**. They enter both share explanations, but the BA, the stem and the order count only once.

**Ownership conflict (geometry finding) [der]:**
- P0807 lies inside the ±9 m teaching bay but in **rack 2's** S0 midpoint strip (R2-N-1).
- The cause is that the bay's fixed 9 m half-width exceeds the ~8.0 m half-gap between racks 2 and 3.
- Any whole-rack source rule must decide which unit such a stem belongs to (OPEN-1/OPEN-3).

**Why stage-3 is needed:** the live formula drifts on the Year-6 state.
- The source's executed-share account uses the *historical* denominator: stump DBH frozen at felling plus grown neighbours (ScenarioOneRackBay.cs:27-31, 280-317).
- Recomputed on Year 6, it reads 0.3136 / 0.3098 / 0.3269 for P0706 / P1109 / P0714, because the Crop Trees grew 2.5–3.4 cm.
- **A Year-6 save alone would understate the release and could not supply authentic pre-treatment denominators.** This is why the stage-3 capture is required.

### 6.3 Comparability: source rack 3 against finite portions of racks 1, 2, 4 and 5 [der, cal]

**Source metrics** (rack 3, both sides, S0):

| Phase | Stems / net ha | Mean DBH cm | BA m²/net ha | Mean CI |
|---|---:|---:|---:|---:|
| Year 1, pre-release (stage-3) | 2,461 | 15.77 | 49.62 | 8.16 |
| Year 6 (candidate1) | 2,332 | 17.72 | 59.06 | 7.86 |

The destinations are the eight 20 m tiles of each other rack (T4). T3 is also run (all four racks Partial under the old grid at every stem minimum).

**Classification rules (candidate only):**
- **Manual** if net/gross < 0.50 (infrastructure-dominated) or stems < minimum (underpowered).
- **NotCovered** if any of the following fails:
  - |Δdensity| / src > tolerance;
  - |ΔmeanDBH| / src > tolerance;
  - |ΔmeanCI| / src > tolerance;
  - species set differs.
- **Covered** otherwise.
- **Site, soil, wind and machine access are UNKNOWN in every tile.** They are not treated as similar. The packet allows Manual/RequestInspection where site is material; that decision is left open.

**Release-evidence verdict for all four destinations: InsufficientSourceEvidence.**
- The packet requires one **whole** source rack.
- The fixture's executed release lies only in the teaching bay. Rack 3 tiles R3-S-1, R3-S-2 and R3-N-1 hold the executed stems; the rest of rack 3 is untreated.
- One stem, P0807, lies in rack 2's strip.
- The similarity tables below are therefore **sensitivity only**: they show where a recipe *could* apply if a whole-rack source existed.

#### T-B3a Rack-level similarity-only verdicts (S0 × T4), all grids

| Phase | Grid (density / mean DBH / mean CI tolerance) | Min stems | Rack 1 | Rack 2 | Rack 4 | Rack 5 |
|---|---|---:|---|---|---|---|
| Year 1 pre-release | x0.5 (10/7.5/10 %) | 15 | Partial (1C/5N/2M) | Partial (4C/4N/0M) | Partial (1C/7N/0M) | Partial (2C/6N/0M) |
| Year 1 pre-release | x0.5 (10/7.5/10 %) | 20 | Partial (1C/5N/2M) | Partial (4C/4N/0M) | Partial (1C/7N/0M) | Partial (2C/6N/0M) |
| Year 1 pre-release | x0.5 (10/7.5/10 %) | 30 | Partial (1C/1N/6M) | Partial (2C/1N/5M) | Partial (1C/4N/3M) | Partial (2C/0N/6M) |
| Year 1 pre-release | x1.0 old MFM-03 (20/15/20 %) [cal] | 15 | Partial (4C/2N/2M) | Partial (6C/2N/0M) | Partial (7C/1N/0M) | Partial (7C/1N/0M) |
| Year 1 pre-release | x1.0 old MFM-03 (20/15/20 %) [cal] | 20 | Partial (4C/2N/2M) | Partial (6C/2N/0M) | Partial (7C/1N/0M) | Partial (7C/1N/0M) |
| Year 1 pre-release | x1.0 old MFM-03 (20/15/20 %) [cal] | 30 | Partial (2C/0N/6M) | Partial (3C/0N/5M) | Partial (5C/0N/3M) | Partial (2C/0N/6M) |
| Year 1 pre-release | x1.5 (30/22.5/30 %) | 15 | Partial (6C/0N/2M) | Partial (7C/1N/0M) | Covered (8C/0N/0M) | Covered (8C/0N/0M) |
| Year 1 pre-release | x1.5 (30/22.5/30 %) | 20 | Partial (6C/0N/2M) | Partial (7C/1N/0M) | Covered (8C/0N/0M) | Covered (8C/0N/0M) |
| Year 1 pre-release | x1.5 (30/22.5/30 %) | 30 | Partial (2C/0N/6M) | Partial (3C/0N/5M) | Partial (5C/0N/3M) | Partial (2C/0N/6M) |
| Year 6 current | x0.5 (10/7.5/10 %) | 15 | Partial (2C/4N/2M) | Partial (3C/5N/0M) | Partial (2C/6N/0M) | Partial (2C/6N/0M) |
| Year 6 current | x0.5 (10/7.5/10 %) | 20 | Partial (2C/4N/2M) | Partial (3C/5N/0M) | Partial (2C/6N/0M) | Partial (2C/6N/0M) |
| Year 6 current | x0.5 (10/7.5/10 %) | 30 | Partial (1C/1N/6M) | Partial (2C/1N/5M) | Partial (1C/4N/3M) | Partial (1C/1N/6M) |
| Year 6 current | x1.0 old MFM-03 (20/15/20 %) [cal] | 15 | Partial (4C/2N/2M) | Partial (5C/3N/0M) | Partial (6C/2N/0M) | Partial (7C/1N/0M) |
| Year 6 current | x1.0 old MFM-03 (20/15/20 %) [cal] | 20 | Partial (4C/2N/2M) | Partial (5C/3N/0M) | Partial (6C/2N/0M) | Partial (7C/1N/0M) |
| Year 6 current | x1.0 old MFM-03 (20/15/20 %) [cal] | 30 | Partial (2C/0N/6M) | Partial (3C/0N/5M) | Partial (4C/1N/3M) | Partial (2C/0N/6M) |
| Year 6 current | x1.5 (30/22.5/30 %) | 15 | Partial (6C/0N/2M) | Partial (7C/1N/0M) | Covered (8C/0N/0M) | Covered (8C/0N/0M) |
| Year 6 current | x1.5 (30/22.5/30 %) | 20 | Partial (6C/0N/2M) | Partial (7C/1N/0M) | Covered (8C/0N/0M) | Covered (8C/0N/0M) |
| Year 6 current | x1.5 (30/22.5/30 %) | 30 | Partial (2C/0N/6M) | Partial (3C/0N/5M) | Partial (5C/0N/3M) | Partial (2C/0N/6M) |

#### T-B3b Per-tile detail, Year 6, old MFM-03 grid, min 20 stems [der, cal]

| Tile | Verdict | Stems | Stems/net ha | Mean DBH | Mean CI | KS D (DBH vs source) | Failing metric / reason |
|---|---|---:|---:|---:|---:|---:|---|
| R1-S-0 | NotCovered | 27 | 2419 | 17.65 | 6.21 | 0.170 | meanCI 0.211>0.2 |
| R1-S-1 | NotCovered | 24 | 2176 | 17.96 | 5.95 | 0.139 | meanCI 0.244>0.2 |
| R1-S-2 | Covered | 30 | 2314 | 18.80 | 6.85 | 0.269 | — |
| R1-S-3 | Covered | 26 | 2005 | 17.18 | 6.67 | 0.244 | — |
| R1-N-0 | Manual | 19 | 2800 | 19.29 | 5.34 | 0.281 | net/gross 0.44 < 0.50 (infrastructure-dominated tile) |
| R1-N-1 | Manual | 17 | 2771 | 18.16 | 5.92 | 0.144 | net/gross 0.40 < 0.50 (infrastructure-dominated tile) |
| R1-N-2 | Covered | 32 | 2469 | 17.66 | 8.52 | 0.106 | — |
| R1-N-3 | Covered | 28 | 2160 | 18.57 | 7.94 | 0.269 | — |
| R2-S-0 | Covered | 30 | 2706 | 17.54 | 7.30 | 0.103 | — |
| R2-S-1 | NotCovered | 20 | 1815 | 18.18 | 6.25 | 0.188 | density 0.221>0.2; meanCI 0.205>0.2 |
| R2-S-2 | NotCovered | 20 | 1543 | 17.94 | 7.27 | 0.194 | density 0.338>0.2 |
| R2-S-3 | Covered | 33 | 2545 | 17.79 | 8.21 | 0.131 | — |
| R2-N-0 | Covered | 28 | 2397 | 17.91 | 7.71 | 0.144 | — |
| R2-N-1 | Covered | 28 | 2154 | 18.28 | 6.87 | 0.281 | — |
| R2-N-2 | NotCovered | 24 | 1989 | 18.34 | 6.18 | 0.125 | meanCI 0.214>0.2 |
| R2-N-3 | Covered | 33 | 2415 | 17.72 | 8.43 | 0.108 | — |
| R4-S-0 | Covered | 26 | 2238 | 18.52 | 6.92 | 0.244 | — |
| R4-S-1 | NotCovered | 28 | 2974 | 17.07 | 9.25 | 0.205 | density 0.275>0.2 |
| R4-S-2 | Covered | 32 | 2782 | 17.60 | 9.39 | 0.127 | — |
| R4-S-3 | Covered | 29 | 2134 | 18.45 | 7.77 | 0.245 | — |
| R4-N-0 | Covered | 30 | 2714 | 17.48 | 7.61 | 0.176 | — |
| R4-N-1 | Covered | 34 | 2630 | 17.92 | 9.20 | 0.115 | — |
| R4-N-2 | NotCovered | 38 | 2939 | 18.40 | 9.11 | 0.197 | density 0.260>0.2 |
| R4-N-3 | Covered | 32 | 2475 | 17.39 | 7.89 | 0.151 | — |
| R5-S-0 | Covered | 26 | 2352 | 18.76 | 6.91 | 0.192 | — |
| R5-S-1 | Covered | 30 | 2320 | 17.77 | 8.71 | 0.156 | — |
| R5-S-2 | Covered | 31 | 2398 | 17.36 | 8.57 | 0.162 | — |
| R5-S-3 | Covered | 26 | 2011 | 18.53 | 7.21 | 0.197 | — |
| R5-N-0 | NotCovered | 21 | 1890 | 18.64 | 5.74 | 0.195 | meanCI 0.270>0.2 |
| R5-N-1 | Covered | 28 | 2166 | 18.41 | 7.30 | 0.266 | — |
| R5-N-2 | Covered | 27 | 2089 | 17.92 | 6.90 | 0.238 | — |
| R5-N-3 | Covered | 28 | 2166 | 18.25 | 6.56 | 0.154 | — |

**Change over time:** between Year 1 and Year 6 (old grid, min 20 stems), **R2-N-2 and R4-N-2 change from Covered to NotCovered**. Classification is time-dependent, which matters for the open source-staleness policy (OPEN-6).



### 6.4 Hypothetical destination queues [hyp]

**This is not an implemented engine and was not gameplay-tested.** The rule, authored here:
- **Crop candidates:** largest-DBH-first greedy selection inside each tile, with ≥ S m between candidates.
- **Removals:** per candidate, add the highest-Hegyi-term non-candidate neighbour *inside the same tile*, until the candidate's share ≥ the target statistic. This is the same shape as the #417 harness rule.
- **Denominators:** whole-stand neighbours, not cut at tiles.
- **Excluded:** in game the player chooses Crop Trees individually, and no automated tree-quality judgement is implied.

The three source statistics (min, lower quartile, median) are within 0.0011 of each other. They therefore produced **identical** queues; the aggregation choice is not distinguishable with this source.

#### T-B4 Hypothetical per-destination queues (Year 6 state; share target = source in-bay statistic) [hyp]

| Spacing | Share statistic | Rack | Crop candidates | Unique removals | Individual confirmations (crop + fell) | Rack removed-BA share | Tiles > 40 % / > 30 % / > 20 % |
|---:|---|---:|---:|---:|---:|---:|---|
| 16 m | min = p25 = median (identical result) | 1 | 11 | 58 | 69 | 0.370 | 3 / 5 / 8 of 8 |
| 16 m | min = p25 = median (identical result) | 2 | 11 | 73 | 84 | 0.353 | 3 / 3 / 8 of 8 |
| 16 m | min = p25 = median (identical result) | 4 | 11 | 72 | 83 | 0.373 | 3 / 3 / 8 of 8 |
| 16 m | min = p25 = median (identical result) | 5 | 10 | 65 | 75 | 0.325 | 2 / 2 / 8 of 8 |
| 8 m | min = p25 = median (identical result) | 1 | 22 | 112 | 134 | 0.571 | 6 / 8 / 8 of 8 |
| 8 m | min = p25 = median (identical result) | 2 | 21 | 120 | 141 | 0.577 | 8 / 8 / 8 of 8 |
| 8 m | min = p25 = median (identical result) | 4 | 21 | 137 | 158 | 0.566 | 8 / 8 / 8 of 8 |
| 8 m | min = p25 = median (identical result) | 5 | 21 | 125 | 146 | 0.586 | 8 / 8 / 8 of 8 |

**Workload counts:**
- These are proposal and confirmation counts derived from a graph-free projection.
- **They are not observed human minutes.** No player time was observed. Graph-derived walking distance was not computed because no route model is in scope.
- Per-tile rows are in `phaseB/hypothetical-queues.csv`.

### 6.5 Unique BA ledgers and cap stress [der; one case syn]

**Per-tile ledger** (`phaseB/ba-ledger-tiles.csv`):
- Columns: A1 baseline (Year 1 pre-source, stage-3), A2 baseline (Year 6), executed non-access removal, Pending 0, Approved 0.
- Destinations had **no** non-access work between Years 1 and 6, so A1 and A2 differ only by growth. This is relevant to the open A1/A2/B1/B2 window choice; nothing is chosen here.

**Actual source release:**
- **Teaching bay:** 0.3609 m² of 1.5171 m² pre-treatment BA = **23.8 %**. This passes a 30 % or 40 % cap and fails a 20 % cap. The 40 % figure is the old sample safeguard (ScenarioOneRackBay.cs:28), sensitivity only.
- **Rack 3 whole (A1):** 0.3609 of 4.9790 m² = **7.2 %**.
- **Crop Tree 8 m neighbourhood severity:**

  | Crop Tree | BA removed / neighbour BA | Share |
  |---|---|---:|
  | P0706 | 0.163 / 0.778 m² | 21.0 % |
  | P0714 | 0.147 / 0.658 m² | 22.3 % |
  | P1109 | 0.096 / 0.402 m² | 23.8 % |

  A good rack-wide figure (7 %) coexists with ~24 % local concentration.
- **Shared competitor (observed):** P0807 and P0908 each appear in two Crop-Tree neighbourhoods. They are counted once in the BA and stem ledgers.

**Already-committed overcap (SYNTHETIC):**
- Tile R4-N-2 (Year-6 BA 1.0354 m²) was given a script-authored committed set of 12 stems, largest DBH first, reaching 42.2 %.
- Under a 40 % cap, no new proposal is admissible.
- The fixture itself has **0** open orders. This case tests the refusal path only.

**Projection stress (hypothetical):** see T-B4. At 16 m spacing, 2–3 tiles per rack would exceed 40 % and every tile would exceed 20 %. The projection therefore fails closed under every cap variant tested. This is evidence that a naive share copy is unsafe; it is not a policy result.

### 6.6 Negative cases

**Authentic negatives (copied and hashed, read-only):**

| Fixture | sha256 | Facts [obs] | Expected refusal [der] |
|---|---|---|---|
| CCF-376 run save (`CCF-376-UIQualification-Retry-20261110/run/config/.../forest-save.json`) | `c715ea5b…ae2` | v24 Enlarged80, Year 2. Five confirmed racks, but the **geometry differs**: entrances at x = −35.4, z = −35.27 / −16.42 / 0.24 / 15.79 / 35.23, gaps 18.85 / 16.66 / 15.56 / 19.44 m. 3 Crop Trees, 149 executed, 0 open orders. | Refuse: changed rack geometry and different world identity, so the candidate1 source recipe cannot be applied |
| CCF-357 persist-cold save (`CCF-357-MaterialDriftTriage-20261109/run/persist-cold/.../forest-save.json`) | `51cacbc7…a53` | Same geometry as CCF-376 (rack ids 1, 3, 4, 5, 6), Year 0. **149 Pending orders**, **3 entrances unmarked**. | Refuse: committed work takes precedence (Excluded/Manual); access not established (Manual); different world |

- The source commits that produced these two fixtures were **not re-verified** here. They are used only as refusal-logic negatives, not as measured comparators.
- No authentic mixed Oak/Beech or juvenile/protection tile exists in any five-rack fixture found:
  - the candidate1 plantings exist only as single beech and oak regeneration cohorts;
  - `plantedJuveniles=[]`.

**Synthetic negatives [syn], labelled, not world evidence:**
- **Mixed Oak/Beech tile** → Manual. Quantitative transfer is adult Sitka only, per #464 §4.1.
- **Protected juvenile tile** → Excluded.
- **Two competing prescriptions for one stem** → Manual / refuse.
- **12-stem tile** → Manual (underpowered).

### 6.7 Annual economics and later-year readings present in the fixture [obs]

| Year | Completed tasks | Contractor cost | Timber revenue | Harvest-minimum adjustment | Harvested m³ | Closing cash | Living trees | BA m²/ha (property) | Mean light | Occupied regen cells |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 0 | — | — | — | — | — | 12,000.00 | 1,344 | 40.68 | 0.015 | 0 |
| 1 | 159 | 2,500.00 | 774.58 | 2,157.61 | 18.32 | 10,274.58 | 1,185 | 37.89 | 0.025 | 51 |
| 2 | 14 | 2,500.00 | 104.70 | 2,449.12 | 2.21 | 7,879.28 | 1,171 | 39.32 | 0.031 | 56 |
| 3 | 2 | 1.50 | 0 | 0 | 0 | 7,867.78 | 1,171 | 41.36 | 0.036 | 91 |
| 4–6 | 0 | 0 | 0 | 0 | 0 | 7,867.78 | 1,171 | 43.45 / 45.58 / 47.75 | 0.041 / 0.045 / 0.049 | 92 / 100 / 120 |

Currency is game units. The table is from `annualReports` and `ecologicalSnapshots`.

**Grouped job minimum:**
- The €2,500 minimum applies **once per grouped annual job, not per rack**.
- In Year 1 the 159 access fellings across all five racks cost 2,500.00 in total, including a 2,157.61 minimum top-up.
- In Year 2 the 14 release fellings again cost 2,500.00, including a 2,449.12 top-up.

**Deadwood:** DW0001 has 0.278 m³ original volume and 0.246 m³ remaining in Year 6.

**Objectives achieved:** retained-canopy, continuous-canopy, regeneration and managed-opening (Year 1); fallen-deadwood (Year 2); introduced beech and introduced sessile oak (Year 3).

**Destination remeasurement:** Years 1→6 are both available offline (stage-3 and candidate1), so the "classification can change" check (§6.3) needed no new run.

---

## 7. Comparability, refusal and queue summary by destination

| Destination | Net ha (S0) | Stems | BA m²/net ha | Release-evidence verdict | Similarity, old grid, min 20 (Y6) | Similarity, old grid, min 30 (Y6) | Hypothetical queue at 16 m (candidates / removals) |
|---|---:|---:|---:|---|---|---|---|
| Rack 1 | 0.0870 | 203 | 62.16 | **InsufficientSourceEvidence** | Partial (4C / 2N / 2M) | Partial (2C / 0N / 6M) | 11 / 58 |
| Rack 2 | 0.0984 | 216 | 56.41 | **InsufficientSourceEvidence** | Partial (5C / 3N / 0M) | Partial (3C / 0N / 5M) | 11 / 73 |
| Rack 4 | 0.0960 | 249 | 67.02 | **InsufficientSourceEvidence** | Partial (6C / 2N / 0M) | Partial (4C / 1N / 3M) | 11 / 72 |
| Rack 5 | 0.0997 | 217 | 57.34 | **InsufficientSourceEvidence** | Partial (7C / 1N / 0M) | Partial (2C / 0N / 6M) | 10 / 65 |

No destination is Excluded:
- there is no protection, juvenile or committed work in any destination tile;
- R1-N-0 and R1-N-1 are Manual (infrastructure-dominated);
- outer remainders and END-W strips are Manual/UNKNOWN by construction.

---

## 8. Phase C — NOT RUN (precise gap statement)

**Conditions at decision time (20:27Z), all available:**
- no Unity process;
- lock `free`;
- #443 lane clean at `acb4e111` and D-073 lane clean at `55863f1b`;
- ZX20 57 GiB free.

**Phase C was not run because no concrete A/B gap is answerable by a minimal disposable witness of the existing normal route:**
1. **Missing whole-rack source release.** Producing one would require choosing Crop Trees and competitors along all of rack 3 in a scratch world. Those choices would be manufactured by the worker, not observed player judgement. The packet forbids forcing marks to manufacture a transferable rule (#464 §4.2, packet Gate B.3). This is a product or packet decision, not a measurement gap.
2. **Pre-treatment denominators.** Already supplied by the authentic, lineage-proven #417 stage-3 capture (§6.2).
3. **Later-year remeasurement and annual economics.** Already present in the fixture (Years 0–6) and the stage-3 capture (§6.3, §6.7).
4. **Live mask positions at `55863f1b`.**
   - The scene diff from the measured `72b4efbd` is one flag on a buildable whose circle lies inside the clearing.
   - The 0-stems-in-masks cross-check holds.
   - A live re-read would refine a derived input, not answer an open evidence question. It is recorded as a limitation instead.
5. **Human marking time and site, soil, wind and access data.** These cannot be produced by a scripted witness at all.

**No physical lock was acquired, no Editor was launched and no harness was staged.** There is no Editor exit code, profile probe or capture.

---

## 9. Physical safety, resources and release

| Check | Pre (20:09–20:13Z) | Post (20:27Z) |
|---|---|---|
| Unity / Hub / licensing / bwrap / runner processes | none | none |
| `unity-lock status --json` | `free`, metadata null, `unity_editor_pids` [] (not acquired) | `free`, metadata null, [] |
| Lock acquired by this task | — | **never** |
| Ops lane `MultiRackEvidence466` in `lanes.json` | registered (sha256 `1b6d70eb…cf21` → `339df8ac…5ecc`, one added entry) | **unregistered**; `lanes.json` byte-identical to pre-registration (`1b6d70eb…cf21`) |
| Ops `board/active.md` | not edited | unchanged (byte-compare); note its `lane-status` parse already failed before this task (`Unity required? must be Yes/No`, #460 row column shift) |
| Protected save/profile manifest (Gate-0, 61 hashed + 4 ABSENT) | 61 same / 0 changed / 0 missing / 4 still absent | 61 / 0 / 0 / 4 |
| #437 `host37.sha256` | 37/37 OK | 37/37 OK |
| Original fixture sha256 | `1a32513…dcb670` | identical |
| #443 lane / D-073 lane | clean `acb4e111` / clean `55863f1b` | clean / clean |
| Worktree `Temp/UnityLockfile` | absent | absent (no `Temp/`) |
| Disk | ZX20 57.15 GiB free (floor 15 GiB); Files 155 GB; `/` 5.7 GB | unchanged in practice (19 MB evidence on Files) |

The protected files were only hashed (opened read-only). No personal save, Legacy40 save or Reference Future was opened for writing. Frozen Reference Future v1 is a tracked file and is unchanged by this branch.

---

## 10. Evidence manifest

**Location and integrity:**
- Evidence root: `/media/jer/Files/CCF/Verification/MultiRackNativeEvidence466-20261113T201043Z/` (19 MB).
- `evidence-manifest-phaseAB.sha256` lists 49 files; its own sha256 is `2d590eb5fb4d6a545677a00565ab5da2a7909dc9f7e19e15dfcf54dc6081a360`.
- Close-out receipts are under `closeout/`.

| File | sha256 |
|---|---|
| `scripts/phase_a.py` | `4019a0e8562d2a40a9b18203bdc888bdecbce26b22580abd02f40c0bfa59b53f` |
| `scripts/phase_b.py` | `05d28c45f3ac129396302b132a09fdba8c2d9dd799c3be7f02454c723b25faff` |
| `scripts/rehash_protected.py` | in manifest |
| `fixtures/primary/forest-save.candidate1.json` | `1a32513112e1fd43dd29af8c8ffbe4ca40cdb5738e7af89a260fbf9707dcb670` |
| `fixtures/history417/stage-3.json` | `116a7acf08dbd7c53cce32a90bda6abd949535a0da08d53e109b3ecd2232c27c` |
| `fixtures/scenery-248.json` | `38db90ffcd578d1c1123ff1f41c4845ff5cb85e0a08593aca406216b95039eed` |
| `fixtures/negative/ccf376-run-forest-save.json` | `c715ea5bfada226ec737386df4f089817eaddec531e84f2cf84f96099ff15ae2` |
| `fixtures/negative/ccf357-persist-cold-forest-save.json` | `51cacbc782ddf03fe1d0f612d544c478d2a30fa6fd540eb523bab63383f31a53` |
| `source/ForestTest.unity.55863f1b` (git blob `acbb98cb`) | `6b1c4760a419d7e61fd3979dc1fb99f632a6ceca5ef0316b7330949b62c9c785` |
| `phaseA/phaseA-summary.json` | `690b243d93307537744233607c6fbac04aa6724c75d71c7ab45059fe836f38c2` |
| `phaseB/phaseB-summary.json` | `139d24d0536fc0580461705886f47718c35c35853031deed9cc11f8f8258f8c2` |

**Other outputs:**
- `phaseA/`: tree ledger, Year-6 tree CSV, boundary ties (empty).
- `phaseB/`: provenance, comparability, queue and BA-ledger CSVs.
- `preflight/` and `closeout/`: lock status, lanes before/after, protected re-hash, host37 checks, the RUNNING comment text.

**Determinism:**
- The full rerun into `rerun/` gives **byte-identical** Phase A outputs.
- Phase B outputs are identical apart from the embedded evidence-root path.

**Reproduce:**

```bash
cd /media/jer/Files/CCF/Verification/MultiRackNativeEvidence466-20261113T201043Z
python3 -I -B scripts/phase_a.py fixtures/primary/forest-save.candidate1.json fixtures/scenery-248.json phaseA
python3 -I -B scripts/phase_b.py "$PWD"
sha256sum -c evidence-manifest-phaseAB.sha256
```

---

## 11. Limitations

- **One world, one scripted history.**
  - The fixture and its stage-3 capture come from one deterministic harness run, with hard-coded Crop Trees and an algorithmic competitor choice.
  - This is authentic normal-route *execution* but not human silvicultural judgement.
  - The release history was made at source `77a56a44` (UI-only difference from `55863f1b`) and re-saved under `87405a56`.
- **The source is a bay, not a rack.** r = 3, all within 24 × 18 m. Small-N statistics are reported but not meaningful.
- **Masks are derived, not re-measured live** at `55863f1b` (§2.2).
- **Recomputed numbers are float64, not saved values.** Hegyi values are not the saved competition index; the Unity annual step uses float32. Property-level BA matches the save to 1.5e-5 m²/ha.
- **Geometry schemes and tilings are candidates.** Results depend on them (e.g. S1 removes the outer remainders but creates END-W remainders of 64 stems). The 0.50 net/gross Manual rule and the KS distance are worker-chosen diagnostics, not rules.
- **The queue projection is a hypothetical rule** with tile-confined removals. A different rule (e.g. allowing cross-tile removals) would change the counts.
- **No observed player time, no rendering, no Unity version observation.**
- **No site, soil, wind, machine-access or legal data exists**; every such input is UNKNOWN.
- **Negatives.** The CCF-376 and CCF-357 source commits were not re-verified. No authentic mixed-species five-rack world was found, so those cases are synthetic only.
- **Ops board.** `lane-status` could not parse `board/active.md` (pre-existing #460 row defect), so `--preflight` was not run. Admission relied on direct worktree, branch, lock and disk checks.

---

## 12. Next product decisions (data presented; nothing ratified)

1. **OPEN-1, area boundaries.**
   - S0 (source strip rule; 52 stems and 149.7 m² in outer remainders) versus S1 (perimeter to the nearest rack; 64 stems west of the headland as an END-W remainder).
   - How to treat the feeder-dominated R1-N tiles.
   - Whether the 9 m teaching half-width may cross a ~8 m midpoint (P0807).
2. **OPEN-2/OPEN-3, source sufficiency.**
   - Whether a bay-only executed release (r = 3, spread 0.0011) may ever stand for a whole rack. The data here say a whole-rack source does not exist yet.
   - The choice among min, lower quartile and median is indistinguishable with this source.
3. **OPEN-3, caps.**
   - Tile versus rack versus Crop-Tree-neighbourhood caps.
   - The real source shows 7 % (rack), 24 % (bay) and 21–24 % (neighbourhood) for the same work.
   - A naive share copy breaches 40 % in 2–8 tiles per rack.
4. **OPEN-5, confirmation load.** Hypothetical 69–84 (16 m) or 134–158 (8 m) individual confirmations per destination rack under the projection. Observed time is still unmeasured.
5. **OPEN-6, staleness.** Two tiles flip classification between Year 1 and Year 6 with no intervening work.
6. **Separate engineering question for #424/#418.** The prescription event-linkage defect path is still present in source. This fixture cannot test it; a prescription-bearing fixture is needed.

OPEN-4 and OPEN-C1..OPEN-C8 are untouched.

---

*Only this file is tracked. Raw evidence is off-repo with the hashes above.*
