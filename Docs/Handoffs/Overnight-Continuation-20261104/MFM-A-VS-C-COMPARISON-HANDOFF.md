# #271 — MFM Option A versus constrained parallel fallback C: matched offline comparison

> **EXPERIMENTAL / OFFLINE — not a policy default.** This document and the script under `Tools/Verification/MFMAvsC/` are research input only. **This document grants no production approval, no spec approval, no status change to any DRAFT spec, and no approval of any reach cap, group tolerance, owner rule, layout, Confirm gate or Windows/Player build.** D-064, D-065 stay controlling; D-067/D-068/D-069 stay direction only. Independent review is still owed (see §12) and is required before any Option A production spec approval.

**Status:** `DRAFT` research return for Overall Manager #200 / DPC #201/#203, claim #271. Not reviewed. Not integrated. Not accepted.
**Worker:** Claude Sonnet 5.5, medium effort (`claude-sonnet-5-5`), role `research`. Token/quota telemetry: **UNKNOWN** (not exposed to the worker).
**Lane:** branch `docs/mfm-a-vs-c-offline-20261104-v1`, worktree `/media/jer/Files/CCF/Operations/CCF-MFM-A-vs-C-Docs-20261104`, from `origin/main` `db43dbe5`.
**Source read (read-only, `git show`):** `72b4efbd` (`integration/scenario-one-player-test-ready-v1`) for C#; `Docs/Project/decision-log.md` D-064…D-069 on `db43dbe5`. `Assets/ForestPrototype` is written `AFP`.
**Raw evidence read (read-only, durable store):** `/media/jer/Files/CCF/Verification/ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/{trees,mfm,suggested_racks}.json` and `…/ScenarioOneFiveRackReference-20261103T2210Z/measurements/{design_candidate,mfm_candidate}.json`. sha256 of `trees.json`, `design_candidate.json`, `mfm_candidate.json` equal the values recorded in #251 Appendix A (checked by the script before it runs; it aborts on mismatch). `mfm.json` sha256 `2e23ca24d7f5c52dac4c498440c7ec27c3db722b242b6e8bf59f065a71a4a8b3` (#251 did not record one).

> **Correction carried forward (never restate the old error).** The R1/R2 corridor **rectangles do not overlap**: minimum rectangle-to-rectangle distance **0.214214 m** (re-derived in §8, R1). A separate **2.4 m editor centreline-clearance rule** (`AFP/Racks/RackEditing.cs:102` `OverlapClearanceM`, enforced at `:176`) **still fails** for R1/R2, whose centrelines are **1.414214 m** apart.

---

## 1. Verdict

> **NO-COMPARISON on "which option is better". A matched, reproducible measurement of reach and stem outcomes exists for Option A and for the legacy/parallel baseline. Fallback C has no single evidence-grounded geometry at 45°, so it is reported as a bracket of explicitly-labelled constructions (C-J45, C-S45 with a 55.7° bound, C-H45) that disagree widely with each other. The 45° 3+2 target cannot be met by C by definition and must be scoped or redefined if C is chosen.**

| # | Finding | Evidence |
|---|---|---|
| V-1 | One experimental reach policy and one sample policy were applied unchanged to Legacy, A and every C construction (§2). Eight published #251/#253 geometry figures were **re-derived exactly** by independent code and a second set of one-line commands from raw files (§8). | `run.log` V1 lines; `rederivation.log` |
| V-2 | **Option A, reproduced under the matched policy:** **36.25 / 40.82 / 44.77 / 49.03 %** of non-corridor ground is beyond reach or Manual at caps **20 / 15 / 12 / 9 m** with the lateral owner, **14.13 / 22.86 / 29.24 / 38.22 %** with the nearest-segment owner. (#253 tabulated 20 and 9; **15 m is new here.**) | §4 T1 |
| V-3 | **Legacy/parallel baseline:** **0.00 % at 20/15/12 m and 3.11 % at 9 m**, either owner rule. | §4 T1 |
| V-4 | **C is not one thing at 45°.** Its uncovered share at 20 m ranges from **3.06 % (C-S45, nearest)** to **56.31 % (C-J45, lateral)** depending only on which layout I construct. The best case (C-S45) beats A on reach, but it needs an **accepted-Rack-V2 change** (two of five chords have no legal headland entrance) and breaks D-068's 16–20 m spacing guidance (13.75 m). The only 45° construction that changes **no** accepted Rack V2 rule (C-H45) is **not better than A on lateral ownership (41.1–42.2 % vs 36.3–49.0 %)**. | §4, §6 |
| V-5 | **The user-requested full-spine 45° 3+2 herringbone cannot be met by C**: C is one parallel direction family; the reference is two families 90° apart (R1,R4 at 45°; R2,R3,R5 at 135°). What C can offer is a **single-direction 45° family**, a different target. | §6 |
| V-6 | **#251's "C cannot hold 45°" is conditional.** It rests on a 16 m minimum gap. That value is D-068 Manager *guidance* ("site-specific 16–20 m"), **not an enforced rule**; the only enforced clearance is 2.4 m (`RackEditing.cs:102`). At 45° the spine allows **4** racks at ≥16 m, or **5** at a maximum equal gap of **13.70 m**. | §6 |
| V-7 | Every "covered" figure is a **ceiling**: density and mean DBH only, competition not computed. On the legacy layout the same ceiling says 14/14 while the recorded C# run says 12/14. | §7 |

**I did not manufacture a winner.** The honest summary for the Manager is in §10.

---

## 2. One experimental policy, applied identically (every constant with its source)

`EXPERIMENTAL` = chosen for this comparison only; not an approved value.

| Item | Value | Source | Status |
|---|---|---|---|
| Reach caps | **20 / 15 / 12 / 9 m** | task brief; #253 `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md` §3.2 tabulates 9/10/12/16/20 | **15 m is my addition** (not in #253). Experimental, unapproved (Manager D-069 forbids unbounded reach and does not approve any cap) |
| Reach distance | distance from ground point to the **owning rack's finite centreline segment** | #253 §9 A5 | carried; a real access model could differ |
| Beyond-reach ground | `NotCovered/Manual`, conservative | D-069 | policy as stated |
| Ownership variant 1 | **lateral**: strips per direction group (unchanged `Derive` logic), point in two groups' strips goes to the smaller lateral distance to a rack centreline, tie band 1e-3 m to the smaller group name; outside every strip = outer remainder, always Manual | #251 §3A.A.1; #253 §2.2 item 4 | carried |
| Ownership variant 2 | **nearest finite segment** to any rack, ties to lowest id; no remainder | #253 §3.2 | carried |
| Sample placement | teaching rack = index ⌊n/2⌋ of the **largest** direction group (single group of five → index 2 = legacy `TeachingRackIndex=2`, `AFP/ScenarioOne/ScenarioOneRackSampleGeometry.cs:28`); anchor = point on it nearest the stand origin, clamped ≥12 m from each end (`:268`); footprint |along|≤12, |across|≤9, minus all corridors (`AFP/ScenarioOne/ScenarioOneRackBay.cs:35,38,171`) | A.1 item 4 (generalisation of legacy); C# as cited | the same rule for every option; for A it is the #251 proposal |
| Units | rack × {Low, Centre, High}, split along the **owning group's own axis** at the anchor's projection ±12 m | A.1 item 5; `ScenarioOnePrescriptionCalibration.cs:74` `SegmentHalfLengthM` | all 15 units always exist; 14 non-source |
| Reach applied to units | a unit is evaluated on its **within-cap ground and stems only** | **my assumption** | #253 defined only ground share; this is how I carry the cap into unit outcomes |
| Precedence | outside > corridor > remainder > mask > sample > unit | `AFP/ScenarioOne/ScenarioOneCoverage.cs:435-458` | carried |
| Masks | production fresh-world baseline masks, 116 capsules (Track 104, Site 7, Headland 4, Clearing 1), inclusive `Contains` | `mfm.json` `baselineMaskList`; `ScenarioOneCoverage.cs:60-66` | geometry of the *proposed* spine masks is **not available** (§7) |
| Fit test | density within **20 %**, mean DBH within **15 %** of the sample; competition (20 %) **not computed**; minimum **30** stems | `ScenarioOnePrescriptionCalibration.cs:74` (`MFM03-CAL-1`); `ScenarioOneCoverage.cs:340,360` | "covered" is therefore a **ceiling** |
| Stand / lattice | 80×80 m, 0.5 m cell centres from −39.75 | #251 raster convention; #253 | lattice bias up to ~0.1–1 % of the stand (#253 F-7); 0.25 m spot check in `results.json` |
| Group link tolerance (A) | 4° undirected | **my assumption**, irrelevant here (racks are exactly 45°/135°) | not a recommendation (#253 left it open) |
| South window for C chords | z = −37.5 | **my assumption** (east/north 37.5 and headland −35.4 are read from `design_candidate.json` far-end labels) | |

---

## 3. What was modelled

**Legacy / parallel baseline.** The five suggested east–west racks from the headland (`suggested_racks.json`), through the unchanged single-family `Derive`. This is the layout the recorded C# run measured.

**Option A (as drafted in #251, with #253's fixes).** The 3+2 reference (`design_candidate.json`): east group {R1,R4} at 45°, west group {R2,R3,R5} at 135°. Legacy `Derive` on all five is **INVALID** ("Racks 4 and 5 cross or overlap across the common normal", reproduced). A runs the unchanged strip logic per group and applies the owner rule. Not modelled: eligibility gate, validity gate, versions, save/load (not relevant to coverage numbers, see #253).

**Fallback C — precisely defined.** *C = the unchanged legacy `Derive`, a single parallel direction family of exactly five racks, no new derivation, no new owner rule.* Because that definition fixes no geometry, four constructions are reported. **All are my constructions, not evidence-derived layouts:**

| Name | Construction | Why it exists |
|---|---|---|
| **C-J45** | The reference's **own five junctions and sides**; every rack gets the same 45° bearing (east racks keep +dir, west racks run −dir). East racks R1,R4 are unchanged; R2,R3,R5 are mirrored NW→SW. **No junction is moved.** | The least-invented C: no new anchor positions |
| **C-S45 / C-S55.7** | Five **full window chords** of a 45° (resp. 55.68°) family whose perpendicular offsets are **equally spaced across the offset range the spine spans** (junctions at the spine's two extremes) | A best-case **envelope** for a spine-crossing family: the #251 §3C.3 bound applied |
| **C-H45** | Five 45° racks whose **entrances are on the headland** (legal today) at equally spaced z from the south window to the last z that still gives the editor's 5 m minimum | The only 45° family that keeps every accepted Rack V2 entrance rule |

C-S60 and C-S70 are in `results.json`/`run.log` as a sweep only.

---

## 4. Matched-policy results

Lattice 0.5 m, 1344 living Sitka stems, unmasked-geometry shares (the #253-comparable quantity). "Uncovered" = non-corridor ground that is beyond the cap of its owner, **plus** outer remainder (lateral rule). Non-corridor ground differs by layout (A 5,875.75 m²).

**T1 — uncovered / Manual ground share (geometry only)**

| Option | Owner | >20 m | >15 m | >12 m | >9 m | Max owner distance (m) |
|---|---|---|---|---|---|---|
| Legacy parallel E–W | lateral | 0.00 % | 0.00 % | 0.00 % | 3.11 % | 10.43 |
| Legacy parallel E–W | nearest | 0.00 % | 0.00 % | 0.00 % | 3.11 % | 10.43 |
| **A** 3+2 herringbone | lateral | 36.25 % | 40.82 % | 44.77 % | 49.03 % | 69.17 |
| **A** 3+2 herringbone | nearest | 14.13 % | 22.86 % | 29.24 % | 38.22 % | 41.18 |
| C-J45 (ref junctions, one family) | lateral | 56.31 % | 59.62 % | 62.13 % | 65.17 % | 64.24 |
| C-J45 (ref junctions, one family) | nearest | 11.99 % | 20.06 % | 28.18 % | 40.80 % | 41.18 |
| C-S45 (spine chords, max spread) | lateral | 17.85 % | 17.85 % | 18.02 % | 19.05 % | 14.72 |
| C-S45 (spine chords, max spread) | nearest | 3.06 % | 7.21 % | 10.38 % | 14.77 % | 29.66 |
| C-S55.7 (bound) | lateral | 9.16 % | 9.28 % | 9.88 % | 11.91 % | 17.61 |
| C-S55.7 (bound) | nearest | 0.47 % | 2.77 % | 5.25 % | 10.07 % | 23.59 |
| C-H45 (headland entrances) | lateral | 41.13 % | 41.13 % | 41.29 % | 42.21 % | 13.91 |
| C-H45 (headland entrances) | nearest | 21.53 % | 28.01 % | 32.10 % | 37.12 % | 54.73 |

**T2 — unit and stem outcomes under the policy, cap 20 m, and cap 9 m** (masks + sample precedence; **covered is a CEILING**; "treatable" = stems not in a corridor, not masked, not in the sample; Manual/NotCovered = treatable − covered)

**Cap 20 m**

| Option | Owner | Units covered (ceiling) | Stems covered (ceiling) / treatable | Manual / NotCovered stems | beyond reach | remainder | too few (<30) | differs | source-unit residue |
|---|---|---|---|---|---|---|---|---|---|
| Legacy parallel E–W | lateral | 14 / 14 | 1120 / 1120 (100.0 %) | 0 | 0 | 0 | 0 | 0 | 0 |
| Legacy parallel E–W | nearest | 14 / 14 | 1120 / 1120 (100.0 %) | 0 | 0 | 0 | 0 | 0 | 0 |
| **A** 3+2 herringbone | lateral | 9 / 14 | 633 / 1173 (54.0 %) | 540 | 377 | 85 | 47 | 31 | 0 |
| **A** 3+2 herringbone | nearest | 9 / 14 | 952 / 1173 (81.2 %) | 221 | 176 | 0 | 44 | 0 | 1 |
| C-J45 (ref junctions, one family) | lateral | 1 / 14 | 30 / 1182 (2.5 %) | 1152 | 196 | 502 | 51 | 403 | 0 |
| C-J45 (ref junctions, one family) | nearest | 2 / 14 | 126 / 1182 (10.7 %) | 1056 | 152 | 0 | 17 | 875 | 12 |
| C-S45 (spine chords, max spread) | lateral | 14 / 14 | 874 / 1089 (80.3 %) | 215 | 0 | 215 | 0 | 0 | 0 |
| C-S45 (spine chords, max spread) | nearest | 14 / 14 | 1053 / 1089 (96.7 %) | 36 | 36 | 0 | 0 | 0 | 0 |
| C-S55.7 (bound) | lateral | 13 / 14 | 950 / 1104 (86.1 %) | 154 | 0 | 109 | 0 | 45 | 0 |
| C-S55.7 (bound) | nearest | 13 / 14 | 1052 / 1104 (95.3 %) | 52 | 7 | 0 | 0 | 45 | 0 |
| C-H45 (headland entrances) | lateral | 10 / 14 | 601 / 1141 (52.7 %) | 540 | 0 | 495 | 45 | 0 | 0 |
| C-H45 (headland entrances) | nearest | 9 / 14 | 801 / 1141 (70.2 %) | 340 | 274 | 0 | 66 | 0 | 0 |

**Cap 9 m**

| Option | Owner | Units covered (ceiling) | Stems covered (ceiling) / treatable | Manual / NotCovered stems | beyond reach | remainder | too few (<30) | differs | source-unit residue |
|---|---|---|---|---|---|---|---|---|---|
| Legacy parallel E–W | lateral | 14 / 14 | 1068 / 1120 (95.4 %) | 52 | 52 | 0 | 0 | 0 | 0 |
| Legacy parallel E–W | nearest | 14 / 14 | 1068 / 1120 (95.4 %) | 52 | 52 | 0 | 0 | 0 | 0 |
| **A** 3+2 herringbone | lateral | 6 / 14 | 452 / 1173 (38.5 %) | 721 | 523 | 85 | 82 | 31 | 0 |
| **A** 3+2 herringbone | nearest | 6 / 14 | 553 / 1173 (47.1 %) | 620 | 480 | 0 | 83 | 57 | 0 |
| C-J45 (ref junctions, one family) | lateral | 1 / 14 | 30 / 1182 (2.5 %) | 1152 | 293 | 502 | 35 | 322 | 0 |
| C-J45 (ref junctions, one family) | nearest | 2 / 14 | 178 / 1182 (15.1 %) | 1004 | 520 | 0 | 40 | 444 | 0 |
| C-S45 (spine chords, max spread) | lateral | 14 / 14 | 862 / 1089 (79.2 %) | 227 | 12 | 215 | 0 | 0 | 0 |
| C-S45 (spine chords, max spread) | nearest | 14 / 14 | 909 / 1089 (83.5 %) | 180 | 180 | 0 | 0 | 0 | 0 |
| C-S55.7 (bound) | lateral | 12 / 14 | 884 / 1104 (80.1 %) | 220 | 38 | 109 | 28 | 45 | 0 |
| C-S55.7 (bound) | nearest | 12 / 14 | 905 / 1104 (82.0 %) | 199 | 126 | 0 | 28 | 45 | 0 |
| C-H45 (headland entrances) | lateral | 10 / 14 | 592 / 1141 (51.9 %) | 549 | 14 | 495 | 40 | 0 | 0 |
| C-H45 (headland entrances) | nearest | 9 / 14 | 626 / 1141 (54.9 %) | 515 | 449 | 0 | 66 | 0 | 0 |

(Caps 15 and 12 m are in `output/results.json` and `output/run.log`.)

Reading T2 carefully: the **Legacy** row is the only one whose unit states can be compared with a recorded C# result (§8, R5): its stems per area match the C# run exactly, but its "14/14" ceiling is **above** the C#'s 12/14 because competition fails two units. Expect the same optimism in every other row.

**T3 — layout facts that decide whether the C row is allowed to exist**

| Option | Rack lengths R1..R5 (m) | Perpendicular gaps (m) | Legal headland entrances | Min centreline distance (m) vs editor 2.4 | Corridor stems | Notes |
|---|---|---|---|---|---|---|
| Legacy | 75, 75, 75, 75, 75 | 15.555–19.439 (#251 N36; raw `suggested_racks.json` shows 18.849 / 16.660 / 15.555) | 5 of 5 | ≥15.6 ✔ | 149 | All five entrances on the headland; accepted today |
| A | 56, 47, 54, 47, 15 | 17.51, 16.62 (west); 17.85 (east); R1/R2 are perpendicular (45° vs 135°), not parallel | 0 of 5 (spine junctions) | 1.414 ✘ (R1/R2) | 103 | R1/R2 rectangles apart 0.214 m; clearance rule fails; entrance rule fails for all five (#251 §9) |
| C-J45 | 56, 22, 48, 47, 46 | 1.414, 8.974, 7.466, 18.087 | 0 of 5 | 1.414 ✘ | 101 | R1/R2 parallel 1.414 m apart: refused by the editor |
| C-S45 | 53, 80, 101, 74, 46 | 13.748 ×4 | 3 of 5 | 13.748 ✔ | 190 | 2 chords start on the south edge; gap < D-068 16 m guidance |
| C-S55.7 | 44, 78, 88, 74, 40 | 16.039 ×4 | 3 of 5 | 16.039 ✔ | 170 | the 16 m geometric floor; junctions at the stand extremes |
| C-H45 | 103, 81, 56, 30, 5 | 12.633 ×4 | 5 of 5 | 12.633 ✔ | 119 | one rack is the 5 m minimum (artefact of max spread); not spine-attached |

---

## 5. Ownership of disputed ground

| Option | Double-claimed ground (m²) | Lateral owner split by group | Nearest owner split by group | Lateral vs nearest disagree (m²) | Stems with a different owner |
|---|---|---|---|---|---|
| Legacy parallel E–W | 0.00 | — | — | 0.00 | 0 |
| **A** 3+2 herringbone | 1575.00 | grp0 729.25, grp1 845.75 | grp0 1020.50, grp1 554.50 | 2726.25 | 575 |
| C-J45 (ref junctions, one family) | 0.00 | — | — | 3751.75 | 770 |
| C-S45 (spine chords, max spread) | 0.00 | — | — | 1088.50 | 231 |
| C-S55.7 (bound) | 0.00 | — | — | 607.75 | 128 |
| C-H45 (headland entrances) | 0.00 | — | — | 2466.00 | 519 |

(group 0 = {R1,R4}, group 1 = {R2,R3,R5}; "disagree" counts a lateral remainder cell as a disagreement.)

Reading: A's naive double claim is **1,575.0 m²** (reproduced). Which group wins it flips with the owner rule (lateral 729.25 / 845.75; nearest 1,020.5 / 554.5 m² for group 0 {R1,R4} / group 1 {R2,R3,R5}), and the two rules disagree on **2,726.25 m²** (including remainder) or **575 stems** for A. C has **no double claim by construction** (one family), but the same two rules still disagree on **607.75 m² (C-S55.7) to 3,751.75 m² (C-J45)** depending on the construction (1,088.5 m² for C-S45, 2,466.0 m² for C-H45); the disagreement is a **reach/ownership-rule** issue, not an A-only one. The owner rule is therefore a first-order product decision for any option (D-069 leaves owner/remainder ties OPEN).

---

## 6. Can C meet the full-spine 45° target?

**Short answer: not the target as the Manager recorded it, and only partly the weaker "45°" reading.**

1. **By definition, C cannot be the 3+2 herringbone.** D-069 names the target "the full-spine 45° 3+2 reference". That reference has two families 90° apart; a single parallel family has one. No geometry choice changes that. (Logical, not numeric.)
2. **A single 45° direction family is geometrically possible.** Spine z-extent **77.489 m** (raw `design_candidate.json`). Racks fitting at 45° for a minimum gap g: g = 2.4 m (editor) → 23; g = 16 m (D-068 guidance) → **4**; five racks → max equal gap **13.70 m**; 64/sin 45° = **90.51 m**; asin(64/77.489) = **55.68°** (all re-derived in §8, R2).
3. **#251's "cannot hold 45°" holds only if 16 m is binding.** I could not find 16 m as an enforced rule: D-068 (`Docs/Project/decision-log.md:115`) calls it "site-specific 16–20 m spacing guidance" (direction, spec not approved). At the enforced 2.4 m, five 45° racks fit trivially. So 45° "feasibility" is a **gap-policy question the Manager owns**, not a geometric impossibility.
4. **Meeting 45° with C changes something accepted, or drops the spine.**
   - **C-S45 (spine-crossing, max spread)** has two of five chords (k=0,1) starting on the south window edge, where **no legal entrance exists** (`RackEditing.EntranceOnHeadland`, `AFP/Racks/RackEditing.cs:162`). It therefore requires an entrance-rule amendment, i.e. **changes accepted Rack V2 behaviour**. Its chords are 46–101 m long and consume **190** corridor stems (A: 103; legacy: 149), and its junctions sit at the stand's extreme ends, so it is a bound, not a design.
   - **C-H45 (legal headland entrances)** changes no accepted rule (entrance on headland, ≥5 m, clearance 12.63 m ≥ 2.4 m, five racks, parallel = legacy `Derive` valid), but it is **not spine-attached**, so "full-spine" is dropped, one rack is exactly the 5 m minimum (a construction artefact of max spread), and its reach result is no better than A on lateral ownership.
   - **C-J45 (reference junctions kept)** puts R1 and R2 **1.414 m** apart, **below the 2.4 m editor clearance**, so it would be refused by `RackEditing` at `:176`. It is not a legal layout.
5. **Conclusion.** If the target stays "3+2 herringbone at 45°", only A (or B) can carry it. If C is the fallback, the target must be **redefined** to one of: *a single 45° direction family on a spine that Rack V2 first learns to accept as an entrance* (C-S), or *45° racks from the headland with the spine as a path only* (C-H). Neither is the herringbone. This is a scope decision for the Manager/user; I recommend nothing beyond stating it.

---

## 7. Stem and mask source limitations

- **Stems:** `trees.json`, 1344 living Sitka, none outside ±40 m, from the #248 fresh Enlarged80 world. That it is the same world instance as #250 is #251's **INFERENCE**. A fresh world has no Crop/Fell marks, planted juveniles or other features, so none are modelled.
- **Masks:** only the **production fresh-world baseline** masks (116) are in geometry. Under them **0 stems are masked** in every layout (baseline masks lie on already-cleared path), which matches the recorded C# run. The **proposed spine continuation / rack-track masks are not available as geometry**; `mfm_candidate.json` holds only counts for them (C3: +490 masks, 156 stems newly masked, −111 treatable stems). So **every spine-bearing result here is an upper bound on stems and on covered units**. This applies to A, C-S and C-J alike; it is not a differential.
- **Competition** (Hegyi) is not computed. The ceiling gap on the only layout with a recorded C# result is **2 of 14 units** (14/14 vs 12/14).
- **Area precision:** lattice, not the production interval scan. Sample net area 313.000 m² (lattice) vs 314.005 m² (C#); corridor union 0.5 m: 524.25 vs 0.25 m: 525.06 m². Stem counts are exact, not lattice-based.
- **Sample transfer:** whether one west-group sample is a valid comparison for east-group ground is still **UNMEASURED** (needs competition and the production code). #253 F-3 (150.6 m² of the sample footprint lies over other units) is carried, **not re-derived**.

---

## 8. Numbers re-derived from raw files (not copied)

`Tools/Verification/MFMAvsC/rederive_from_raw.sh` reads only raw JSON, uses neither my main script nor #253's, and was run in the lane (`output/rederivation.log`). Commands and raw result lines:

```
$ Tools/Verification/MFMAvsC/rederive_from_raw.sh
R1 rectangle-rectangle min distance (R1,R2) = 0.214214 m (positive => NOT overlapping)
R1 junction-to-junction distance R1-R2     = 2.000000 m
R1 centreline segment distance R1-R2       = 1.414214 m  (< 2.4 => editor OverlapClearanceM refuses)
R2 spine z-extent = 77.489182 m
R2 64/sin45 = 90.50967 m ; asin(64/extent) = 55.68205 deg ; extent*sin45/4 = 13.69828 m
R2 racks fitting at 45 deg for gap g -> g=16: 4 ; g=13.7: 4 ; g=2.4: 23
R3 corridor union = 524.25 m2 ; non-corridor = 5875.75 m2
R3 nearest-rack ground beyond 9/12/20 m = 2246.00 / 1718.25 / 830.00 m2
R4 living stems = 1344 ; stems with centre inside any of the five reference corridors = 103 (published #251: 103)
R4 recorded C# baseline: covered 12/14, sourceStems 75, sum of area stems = 1120
```

| # | Re-derived | From | Result | Published | Match |
|---|---|---|---|---|---|
| R1 | R1/R2 rectangle gap and centreline distance | raw rack endpoints | 0.214214 m / 1.414214 m | #253 correction | yes |
| R2 | spine extent, 90.51 m, 55.68°, 13.70 m | raw spine polyline ends | as above | #251 N60 (90.51, 55.68°, 77.489) | yes (13.70 m is new) |
| R3 | corridor union, non-corridor, nearest ground beyond 9/12/20 m | raw rack endpoints | 524.25 / 5,875.75; 2,246 / 1,718.25 / 830 | #251 N51/N55 | yes |
| R4 | stems in reference corridors | raw `trees.json` + endpoints | 103 | #251 | yes |
| R5 | per-area stems of the legacy port vs the recorded **C#** baseline | `mfm_candidate.json` `baseline` | **15 of 15 areas equal; sample 75 = 75** | recorded C# run | yes |
| R6 | lateral-owned, remainder, double claim, lateral max reach | main script `V1` lines | 5,461.25 / 414.5 / 1,575.0 / 69.17 m | #251/#253 | yes |

The main script's own V1 block independently reproduces R3 and R6 (`output/run.log` lines 16–24): **8 of 8 published areas equal to 1e-9 m²**. Items not re-derived (carried from #253, flagged): sample-over-other-units 150.6 m², Confirm-gate counts (30 calls / 16 files), grouping-tolerance sweep.

---

## 9. Fidelity limits (carried from #253, and what is new)

- **A Python port is not production C#.** I ported `Derive`, `Corridor.Contains`, `Frame.Contains`, `Mask.Contains`, `Locate` precedence and the `Compare` tolerances from `72b4efbd`. **No C# was compiled or run. No Unity, Library, Player or Windows build.**
- **Algebraic equivalence is not executable equivalence.** The legacy port matching one recorded C# run (R5) is evidence about the **legacy path on one layout**. **It says nothing about the A or C paths: neither exists in C#.**
- The Python float path, abbreviated types and 0.5 m lattice differ from the C# float path and interval scan.
- C is a family of **my constructions** (§3). I selected none as the proposal.
- Nothing here exercises Confirm, `UpdateRack`, save/load, hashes, rule versions, rendering, minimap, turning, yard or entrance trees.

---

## 10. Manager decision inputs (no approval is requested or implied)

1. **Is the 45° 3+2 herringbone still the target?** If yes, C cannot serve it; the fallback decision (D-069) then changes the product, not just the implementation. If C is the fallback, choose the redefined target (§6.5): C-S-like (needs an entrance-rule amendment) or C-H-like (no spine).
2. **Is 16 m a binding gap or guidance?** It flips C at 45° between 4 racks and 5 racks (13.70 m) (§6.3).
3. **Owner rule (lateral vs nearest) and reach cap are first-order.** For A the owner rule moves the uncovered share by 22.1 → 10.8 points (20 → 9 m); the cap moves it 14.1 → 38.2 % (nearest) or 36.3 → 49.0 % (lateral). Neither is evidence-backed; D-069 leaves both open.
4. **Is a "covered" ceiling acceptable?** Competition is the missing test; the legacy case shows it can remove 2 of 14 units.
5. **What does "conservative NotCovered/Manual" cost the player?** For A at cap 20 m, 540 (lateral) or 221 (nearest) of 1,173 treatable stems are left Manual/NotCovered (T2).
6. **Do not rank on this table.** C-S45's reach advantage is a best-case synthetic envelope; the legal-entrance C-H45 is not better than A. A fair ranking needs a Manager-chosen C layout first.

## 11. What is NOT proven

- That A or any C is safe, calibrated, correct in C#, or approvable. **No option is approved.**
- Any real Covered/NotCovered outcome (competition, features, proposed-spine masks missing).
- That C-S/C-H/C-J are buildable: entrances, turning, yard, tree impact, rendering, map/minimap consistency are unmeasured for all of them.
- That C-S45's equal-offset construction is an upper bound on coverage for C at 45° (it is intended as best-case; not proven).
- Sample transfer across groups; calibration under any cap; Q-14/Q-15/Q-26 of #253.
- Lifecycle gate, save/load, hash, version behaviour (not in scope here; #253 stands).
- Anything about a second sample or graph option B.

## 12. Review and handoff state

**Independent review owed** (GPT-6.1 reviewer to be dispatched by DPC). Suggested focus: the unit-level reach assumption (§2), the three C constructions (§3), and the V2 comparison to the recorded C# run.

**Files (in this lane):**

| File | sha256 |
|---|---|
| `Tools/Verification/MFMAvsC/mfm_a_vs_c_offline.py` | `3893ee382aa8a55ac29119344cf4c933aac56fbe329b471bf92db19299146d5b` |
| `Tools/Verification/MFMAvsC/rederive_from_raw.sh` | `6b22dd2f47bd97c1a2cf700405e2c49bc3f3890686063b022365a1467261cd59` |
| `Tools/Verification/MFMAvsC/output/results.json` | `92e5ab3b19587740c2969cac0cb8c9e55bf67876a5a62e895f7785f41ada5d8a` |
| `Tools/Verification/MFMAvsC/output/run.log` | `2deb90cfcfa27a1b23ae6af93246a6c4d261e073254e3eb64bc9969e7ed6c5e1` |
| `Tools/Verification/MFMAvsC/output/rederivation.log` | `f199591a47392e21c7d5f398e04d640361875cccd0c56e2a9e99858a49859d73` |

Reproduce: `python3 -I -B Tools/Verification/MFMAvsC/mfm_a_vs_c_offline.py <outdir>` (stdlib only, no random numbers, ~20 s; two runs gave byte-identical `results.json`). The `-B` matters: it avoids a `__pycache__` that would dirty a lane worktree. The directory is **disposable research tooling**: not a test, not a gate, do not copy to `Assets/`, delete after the Manager has consumed this handoff.

**For the DPC to append (the worker edits neither `ROUTING-LEDGER.md`, `ORCHESTRATION-LEDGER.md` nor `active-tasks.md`):**

```
EVENT — #271 A-vs-C matched offline comparison — RESEARCH WORKER RETURN
pool: claudeAgent / claude-sonnet-5-5 / effort medium / role research
lane: docs/mfm-a-vs-c-offline-20261104-v1 @ /media/jer/Files/CCF/Operations/CCF-MFM-A-vs-C-Docs-20261104 (from origin/main db43dbe5)
source: 72b4efbd (git show only). Unity: none. Player/Windows build: none. No production, active-tasks or ledger edit.
verdict: NO-COMPARISON on "which is better"; matched measurement for A and legacy; C reported as bracket of three constructions
key numbers: A uncovered 36.25-49.03% (lateral) / 14.13-38.22% (nearest) at 20-9 m; legacy 0-3.11%; C 45deg 3.06%..56.31% at 20 m by construction; spine 77.489 m, 55.68 deg, 13.70 m max equal gap, 4 racks at >=16 m
fidelity: Python port; legacy path matches recorded C# stems 15/15 areas + 75 sample; A and C paths have no C#
quota/context/tokens: UNKNOWN
needs: independent review (GPT-6.1) before any Option A production spec approval; Manager decisions in section 10
```
