# MFM-03 — Coverage Calibration Measurement: Independent Review

**Task reviewed:** `MFM-03 — Coverage calibration measurement` (claim #39, umbrella PLAYER-TEST-OVERNIGHT-01 #91)
**Review role:** independent read-only review sub-agent ("Claude A"). I did not implement MFM-03 (implementer: Claude B, separate thread).
**Reviewer model / provider:** Claude Opus 5.5 (`claude-opus-5-5`), Anthropic, running in Claude Code under the T3 Code harness. No child agents.
**Reviewed SHA:** `60792dc0a70a26af6d43603d9992bc477b9782f5` (branch `research/mfm-coverage-calibration-v1`, matches `origin`), base `72dbee8b8eab66f58046abfd49e77ead4c29d357`; tooling commit `139b1f9edebfcda4c0bbed2b195f0e6b0ed01f8c`.
**Worktree:** `/media/jer/Videos/CCF-MFM03`
**Unity:** not used. No lock acquired. No Windows build. Apart from this file, nothing was written to the repo or to the evidence directory.

## Verdict

`MFM-03 CALIBRATION REVIEW PASS — ACCEPT FOR MFM-04`

The measurement reproduces byte for byte from the committed tooling and frozen inputs. Fixture identity matches the live MFM-02 Unity receipt. The method is sound, and the recommended metric set and tolerances rest on measured data. None of the four §14 product items blocks MFM-04. All four fall within authority the approved spec delegates, so the Manager can accept them. The non-blocking findings below (N1–N6) correct some overstated precision in the supporting arguments. They do not change any recommended value or classification. MFM-04 and the Manager should carry them forward and should not repeat the overstated claims.

## 1. Commands and results

| # | Command / check | Result |
|---|---|---|
| 1 | `git -C /media/jer/Videos/CCF-MFM03 status --short`; `git log 72dbee8..HEAD`; `git ls-remote origin research/mfm-coverage-calibration-v1` | Clean tree. Two commits (`139b1f9`, `60792dc`). Remote tip is `60792dc`. |
| 2 | `git diff --name-only 72dbee8..HEAD` | Only `Tools/Verification/MfmCoverageCalibration/**` (10 files) and `Docs/Handoffs/MFM-Implementation-03/HANDOFF.md`. Nothing under `Assets/` and no `.meta`. |
| 3 | `git diff 8f9be4f 72dbee8 --stat --` on `ForestStartingStand.cs`, `ScenarioOneHeadlandGeometry.cs`, `ScenarioOneRackSampleGeometry.cs`, `ScenarioOneRackBay.cs`, `CompetitionShare.cs`, `RackGeometry.cs`, `ForestEcologyController.cs`, `OfflineStubs.cs` | Empty. The handoff's claim that these files are unchanged since the MFM-02 Unity-tested HEAD holds. |
| 4 | `sha256sum -c MANIFEST.sha256` in `/media/jer/Files/CCF/Verification/MFM03-CoverageCalibration-20261029/` | All 5 files OK. |
| 5 | `python3 -I -B Tools/Verification/MfmCoverageCalibration/run_coverage_calibration.py --out /tmp/mfm03-review-rerun-ZJvU` (≈8 s, .NET SDK + Python 3.12) | 16 × `MFM03_CHECK_PASS`, `MFM03_ANALYSIS_PASS checks=16 failed=0`, `MFM03_TABLES_WRITTEN`, `MFM03_EXPORT footprint=75 ba=1.4417530581468396 sha=c2f2fd5418794915 rackRemovals=149 postLiving=1195`, `MFM03_EXPORT_PASS`. |
| 6 | `cmp` of the rerun against the evidence | `export.json` `149ff1b4…679c`, `results.json` `aa73c75f…7c31` and `tables.md` `76c5c6c6…86ba` are **byte-identical**. The run is deterministic. |
| 7 | Independent negative controls: exporter built separately from the same `SOURCES` in `/tmp/mfm03-review-neg-*` and run on perturbed copies of `fixture-stand.csv` | P0605 moved to z = 99 → count 74, SHA `56785985197f1a2b`, BA FAIL, **rc = 1**. +0.5 cm DBH on P0605 → BA FAIL, **rc = 1**. Unperturbed run → rc = 0 and output byte-identical to the evidence `export.json`. |
| 8 | Cross-check against `/media/jer/Files/CCF/Verification/MFM02-RackSampleGeometry-20261027T202415Z/unity-v2/evidence/mfm02-v2-measurements.txt` | See §2. All match. |
| 9 | Spot recomputation of handoff numbers from `results.json` (per-unit table, sweeps, plateau ends, windows, variants, band, edge bins, probes, spacing rows) | All handoff figures checked agree with `results.json` to the stated rounding. |

The `/tmp` rerun and negative-control directories are scratch copies only. They are not durable evidence and are not cited as such.

## 2. Fixture identity (task item 2)

The live MFM-02 V2 Unity receipt (`…/unity-v2/evidence/mfm02-v2-measurements.txt`, HEAD `8f9be4f`) records:

- `footprintTrees=75 footprintBA=1.441753070295678 corridorExcluded=10 treesBetween8.00385and9m=11 livingOutsideNineMetreBandWithin10m=8 footprintSha=c2f2fd5418794915`. This is the same after reload, after reconfirm and with permuted rack IDs.
- Rack z values −35.271175 / −16.421764 / 0.238235 / 15.793530 / 35.232353, width 2.4, x −35.4 → 40.
- Unit areas 1132.28247 / 1420.37646 / 1288.61182 / 1399.76477 / 1158.96472 m². Both remainders area 0, edges −44.69588 / 44.95177.
- Crop Trees P0810 / P1206 / P1214. Initial release `baShare=0.17898431420326233`.

The MFM-03 exporter and analysis reproduce every one of these, digit for digit. The exporter checks count, SHA and BA (|ΔBA| ≤ 1e-6). The analysis checks the 10/11/8 band counts, the five units and the empty remainders, the 6400 m² tiling, and that the Crop Trees are living footprint stems. The exporter exits 1 on any mismatch, and the analysis fails if `problems` is non-empty, so a silent identity drift cannot pass (§1 row 7).

**Scope of identity (correctly stated in the handoff):** it is proven for the 75-stem footprint, the band counts and the 1,344 total. Positions and DBH of the other ~1,269 stems are not individually verified against a live dump. They come from the accepted RACK-V2-02A reconstruction, whose provenance hashes are listed in `inputs/PROVENANCE.md`. This is residual §12.2.

## 3. Method audit (task item 3)

**Sound:**

- **Competition.** `CompetitionShare.Measure(...).CurrentTotal` is computed from the unmodified production source. `SourceParity` text-checks the production Hegyi term, the inline annual-step copy and the 8 m cutoff. A 9 m pre-filter only shortens the candidate list, and `Measure` applies the exact cutoff. Samples are passed as living, unplanned and not-in-footprint, which matches a post-access, pre-treatment state.
- **Masks and denominators.** The same masks apply to source and targets, for both stems and area. Corridors, masks and the footprint are combined as an exact interval union per 0.1 m column. Source-area convergence is 309.885 / 309.810 / 309.817 m². Stem-partition conservation is checked for all three partitions (`every living post-access stem is in exactly one region, the footprint or a mask`).
- **Per-unit partition.** All partitions are sub-partitions of the unchanged MFM-02 units. Footprint stems that spill into R2 and R4 (2 and 3 stems) are excluded from those targets, and their area is subtracted.
- **Regeneration diagnostic.** The cell indexing (`row = floor((z+40)/5)`, 16 per axis) matches production `ForestEcologyController.GetCellIndex` (`z * cellsPerAxis + x`). The diagnostic is correctly labelled historical, pre-V2 and not V2 state.
- **Candidate spacing.** Order uses position only (distance to region mean, then x, z, ID). No DBH, CI or dominance value enters ordering or "rejection", and the rejection hash is keyed on tree ID. This is consistent with spec §8.1 and U5.
- **Determinism.** Confirmed by the byte-identical rerun.

**Arithmetic checks that hold:** 13/14 covered. The CI plateau is (16.6, 33.2) = (|R1C|, |R1W|). The density plateau starts at 12.3% (R2C) and the DBH plateau at 3.3% (R5C). Guard sweep 5 / 9 / 11 / 13. Window fractions 52.8 / 90.4 / 96.0%. Tail exceedance at +10 pp is 42.4%. Former-edge effect on ΔCI is at most 2.2 pp (R4C 13.7 → 11.5). Growth √1.05226 = 1.0258/step. 16 m row: 17 candidates, 295 m tour, 17/17 at 14.9%. 17 × 30 s + 295 m / 4 m·s⁻¹ = 9.7 min.

### Findings

**Blocking: none.**

**Non-blocking (carry forward; do not repeat the overstated wording):**

- **N1. The SE is an under-estimate; "about 4–6 SE" overstates the separation.** `seDCi` treats source and target CI values as independent samples. Hegyi CI values are spatially autocorrelated, because neighbouring stems share neighbours and the same openings, so the effective n is smaller. The true uncertainty is larger than ±3.1–4.7 pp. Treat the "4–6 SE" and "about 11 SE" (R1W) wording as an indicative MODEL DERIVATION only. R1W's failure is robust either way (−33.2%).
- **N2. "Clear gap to both the nearest covered and the nearest failed unit" is true only on the failure side.** R1C (−16.6%, SE ±4.3) sits 3.4 pp inside the 20% CI tolerance, which is under one even-optimistic SE. R2C (−15.1%) and R4C (+13.7%) are also near the edge. The handoff already lists these as boundary units in §8. The plateau is really "R1W is an outlier": with 14 regions on one fixture, any tolerance between adjacent sorted |Δ| values forms a "plateau". So 20% is a defensible GAMEPLAY CALIBRATION point within the measured gap, not a statistically separated threshold.
- **N3. The "clean interior window" evidence is thin.** Only **7** of 125 windows qualify as clean. All 7 are on rack 3 at centre x −6 … +6 m, step 2 m on a 24 m window, so they overlap the source heavily, and one *is* the source window (`isSource`, ΔCI 0.0). "100% within ±10%" therefore says little about position noise elsewhere. The 125 windows also overlap, so the 53 / 90 / 96% fractions are not independent observations. The argument against 10–15% CI tolerances mixes genuine edge differences with "noise". It still holds qualitatively, because tighter tolerances flip R1C, R2C, R4C and R5E.
- **N4. `negative-controls.txt` records `rc=0` for both failing controls.** The exporter returns 1 on any problem, and my independent reruns gave rc = 1 (§1 row 7). The recorded `rc` was almost certainly the exit status of a pipeline stage, not the exporter. The FAIL lines themselves are correct. Do not cite those `rc` values. This review's row 7 supersedes them. The evidence was not rewritten.
- **N5. The pre-access variant label is imprecise.** "Both sides measured pre-access" in handoff §7 actually uses **post-access stem sets and areas with pre-access CI** (`variant(..., "ciPre")` over `lv` = post-access living). The conclusion still stands: mixing access states changes R2C. The wording should say "pre-access competition values".
- **N6. The same-year offline state differs from the live chain timing.** The live receipt shows footprint BA 1.441753 → 1.517099 with the same 75 trees across the access step (`v2.after-access`), so the live post-access snapshot is one growth step later than the fixture's year 0. The offline comparison is same-year and growth-free. Cross-year robustness (source snapshot at Lesson 2, targets after Lesson 5) is **not** measured. The DBH tolerance headroom rests on an assumed uniform growth rate. The handoff correctly puts this in residual §12.1 with a 7.5% drift criterion. The Hegyi ratio is invariant only under *uniform* growth; competition-dependent growth and mortality are unmeasured.

**Minor / editorial:**

- Spec §10.4 defines `Manual` as "player has chosen to manage it manually", while §4.2 allows a remainder to be "left for manual management". MFM-04 should show a non-empty remainder as *left for manual management* (not covered/compared), without implying that the player chose it.
- The tooling cannot check species: the CSV has no species column. "All Sitka" rests on the spawner default and the historical save. MFM-04 must enforce the Sitka-only rule from live species state, not assume it.
- 4 m/s is a run, not a walk. It is already labelled ASSUMPTION, and the minutes are not player measurements.
- `run.log` interleaves .NET output after the Python output because of stdout buffering. This is cosmetic.

## 4. Policy boundaries (task item 4)

| Boundary | Status |
|---|---|
| No production change | **Holds.** Diff limited to `Tools/Verification/MfmCoverageCalibration/` and the handoff. Production sources compile unmodified in a temp dir outside the repo. |
| No density / Sitka retune | **Holds.** No species, growth or stand-generation file touched. Frozen CSV copied verbatim from the accepted RACK-V2-02A output. |
| No anchor / fixture rebaseline | **Holds.** The exporter *asserts* the MFM-02 live values and does not redefine them. Model-0, Legacy40 and Reference Future are untouched. |
| No persisting state / save | **Holds.** No save, schema or serialized-asset change. Output only in the Files evidence tree. |
| No prescription implementation | **Holds.** The classifier and greedy release indicator are analysis-only Python, explicitly not production. Nothing lands in `Assets/`. |
| No Oak/Beech quantitative rules | **Holds.** No Oak/Beech logic in tooling. The fixture has no broadleaf adults, and the handoff keeps the Sitka-only exclusion in force. |
| Unity not used | **Holds** per the handoff, with no lock receipt or Unity evidence present. This review used none either. |
| Storage policy | **Holds.** Evidence is under `/media/jer/Files/CCF/Verification/`. Worktree on `/media/jer/Videos` (not ZX20), with nothing created on ZX20. |

## 5. Handoff §14 decisions: authority classification (task item 5)

| # | Item | Classification | Basis | Recommendation |
|---|---|---|---|---|
| 1 | Anchor-aligned thirds sub-partition | **Delegated technical / calibration authority (Manager accepts)** | Spec §4.2: "Exact segmentation and edge treatment remain implementation/calibration details to be qualified from the final Rack V2 geometry before production logic is frozen." MFM-02 unit semantics are unchanged. | Accept as GAMEPLAY CALIBRATION. Whole strips (R1 margin 1.03) and equal thirds (R2C 1.01) were measured as fragile. MFM-04 must derive the split from the live anchor, not hard-code ±12 m about x = 0. |
| 2 | No crowded-tail (P90) guard | **Delegated calibration authority** | Spec §10.2: Stage C candidates "are not automatically accepted"; the task "must qualify chosen tolerances". The guard was never accepted. Measured as non-robust (5 / 9 / 11 / 13) and not band-selective. | Accept. Spec §10.3's "former-edge dense band" guard is satisfied by flag-not-exclude, given the measured ≤ 2.2 pp effect. |
| 3 | ~93% (13/14) of thirds covered after one sample | **Delegated as a calibration outcome; not a gating user decision.** It has a user-experience dimension that should be *surfaced*, not decided here. | U1 / U2 / P1 accept applying the demonstrated sample to comparable areas. §10.1 requires that no proposal assume *every* unit is represented (met: R1W fails, for a stated, observable reason). U4 keeps manual management. The spec sets no target coverage fraction. Coverage ≠ treatment: every candidate and prescription is still confirmed individually. | Accept for MFM-04. Put the smallest question to the user at the next product checkpoint (non-blocking): *"In the default even-aged Sitka stand, one demonstrated sample makes roughly 13 of 14 areas eligible for the same treatment, and only the stand-edge corner differs. Is that acceptable, or should the game deliberately require a second sample somewhere?"* |
| 4 | Batch vs queue re-suggestion after rejection | **Delegated (conservative default), revisit in UX qualification** | Spec §8.2 requires individual confirmation, queue plus Next/Previous and no bulk action, and makes workload a UX qualification item ("stop for a product decision" only if tedious). Batch adds no system-initiated suggestions after a player judgement and keeps §8.3 manual override. Queue re-suggestion is the more expansive option. | Accept **batch** as the V1 default. This is not MFM-04 core logic. Escalate to the user only if user testing finds rejected positions leave unacceptable gaps. |
| 5 | 16 m candidate spacing, unit-centre-first | **Delegated calibration authority** | Spec §8.1: candidate spacing "is its own parameter and must be qualified separately". Listed under GAMEPLAY CALIBRATION in spec §19. | Accept as a **fixture-specific, versioned** parameter. The feasibility basis is the live sample's 17.9% BA ceiling, and a player's sample sets a different ceiling. MFM-04/11 must re-check feasibility at runtime rather than trust 16 m. |

**None of the five items needs a user product decision before MFM-04.** Item 3 is the only one I would put to the user, and only as an informational, non-gating question. That is why the verdict is PASS rather than "USER PRODUCT DECISION REQUIRED".

## 6. Unity-only residual (task item 6)

**Correctly scoped:**

- Live post-Lesson-5 state: growth drift, mortality, cohorts, juveniles, shelters, clearance, deadwood, marks and orders.
- A whole-stand live identity dump.
- Live `GetCompetitionIndex` versus offline `CompetitionShare` parity.
- Real player-authored Rack V2 requalification, which is explicitly *not* satisfied by this fixture.
- Human confirmation UX.

The acceptance criteria (footprint SHA, CI parity ≤ 1e-4, classification unchanged or changed for a stated live reason, DBH drift < 7.5%) are concrete and testable. The handoff correctly calls the proposed harness a *copy* of the MFM-02 harness, so it needs its own task, lock and stage/run/remove cycle. It does not authorise editing the MFM-02 harness.

**Gap to add:** N6. The residual should also measure same-year versus cross-year CI change under live, non-uniform growth between the source snapshot and extension time. Criterion (b) covers this only implicitly.

**Does it block MFM-04? No.** MFM-04 is pure deterministic core with no UI and no Work Plan mutation. It can implement the metric definitions (net-area density, mean DBH, mean authoritative CI, n ≥ 30, same-state comparison, partition, remainder and exclusion semantics) and take the tolerances and spacing as **versioned GAMEPLAY CALIBRATION parameters**. Spec §10.2 says tolerances must be qualified "before they become production constants". The residual must therefore pass before the MFM integration or qualification gate freezes those constants and before beginner/external testing (U3). It can be queued behind the serialized Unity lane.

## 7. Conditions on acceptance (for the Manager / MFM-04 packet)

1. Treat 20% / 15% / 20% / n ≥ 30, the thirds partition and 16 m spacing as **GAMEPLAY CALIBRATION, one fixture, one rack layout**, versioned per spec §7. Do not present them to players as forestry standards.
2. Do not carry the N1–N3 precision wording ("4–6 SE", "clear gap" on the covered side, "100% of clean windows") into specs or player copy.
3. Keep the two measurement definitions that are part of the metric: same post-access state on both sides, and masked net-area density.
4. Queue the §12 Unity residual (plus N6) before freezing constants. The real player-authored Rack V2 requalification remains open.

## 8. Evidence paths

- Implementer evidence (read-only, manifest verified): `/media/jer/Files/CCF/Verification/MFM03-CoverageCalibration-20261029/` (`run.log`, `export.json`, `results.json`, `tables.md`, `negative-controls.txt`, `MANIFEST.sha256`).
- MFM-02 live receipt used for identity: `/media/jer/Files/CCF/Verification/MFM02-RackSampleGeometry-20261027T202415Z/unity-v2/evidence/mfm02-v2-measurements.txt`.
- Reviewer scratch reruns (not durable): `/tmp/mfm03-review-rerun-ZJvU/`, `/tmp/mfm03-review-neg-*/`.

`MFM-03 CALIBRATION REVIEW PASS — ACCEPT FOR MFM-04`
