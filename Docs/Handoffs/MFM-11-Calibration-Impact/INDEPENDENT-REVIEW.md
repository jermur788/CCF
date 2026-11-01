# MFM-11 — Calibration Impact Qualification — Independent Review

**Claim:** #220
**Verdict:** `NEEDS CHANGES`
**Reviewer:** Claude (Sonnet 5.5), review sub-agent. I did not implement or qualify this task.
**Date:** 2026-11-01

I did not edit production, tests, thresholds, baselines, the handoff or the coordination ledger. I did not run Unity and did not self-accept the task. I did not touch #194 or build for Windows. The only file I wrote is this review.

## Identity and artifacts inspected

- Exact candidate: `3acdf9cb635cdca79606335801e97f9d7dcce8b0`.
- Branch `verify/mfm-11-calibration-impact-v1` at `1249c782`. `git diff --stat 3acdf9cb HEAD` shows only the handoff, measurement design and `Tools/Verification/Mfm11CalibrationImpact/*`. Production bytes are unchanged.
- Read in full: `HANDOFF.md` and `Mfm11CalibrationImpactGate.cs`.
- Read: `MEASUREMENT-DESIGN.md` (grepped for the completion and repeated-intervention rows only), and the task packet (`9d26846a:.../TASK-PACKET.md`, grepped for the pass conditions and the MFM-03 residual text).
- Not read: `run_gate.py`, `run_offline.py`, the offline logs and fixtures, the Release Repin, the Browser-readiness note and the MFM-03 Manager review. Offline claims in the handoff, and the `run1` result, are unreviewed here.
- `unity/run2/evidence/{comparison-table,notes}.txt` and `unity.log`, plus `process-result.json` for both runs. Run 2 passed with 54 checks and `MFM11_UNITY_PASS`.
- Completion logic in `ScenarioOneManager.EvaluateProgress`, `ScenarioOneCompletionRules.cs` and `ScenarioOneObjectives.Evaluate`.
- Scenario definition `ScenarioOne.asset`.
- Cleanup: U1 clean at `1249c782`, `unity-lock` reports free, no Unity PIDs, no `mfm11*` file under `Assets/ForestPrototype/`.

## 1. Three-path comparison validity

**Partly valid. It is not like-for-like.**

What holds:
- All paths restore from the same `baselineState` through `saves.LoadData`. The baseline is identical across paths: 1,177 living stems, 25.4715 m² BA, cash 776,854, canopy 0.970165.
- Path A, B and C are genuinely distinct. A has no treatment, B has an 84-order prescription with provenance, and C has 9 manual marks with no provenance.
- Species split and provenance counts are consistent with the paths.

What does not hold:
- **Unequal time horizons.** Path A advances 3 years (`for y<3`). Paths B and C advance 1 year. A's BA (25.47→29.50) and canopy/light/regen are three years of growth. B's and C's are one year.
  - The B-vs-A and C-vs-B "treatment impact" lines in the handoff therefore mix treatment with a 2-year growth difference.
  - A's regeneration of 51→86 and B's 51→101 are not comparable.
- **Arithmetic error in the handoff.** "B vs A: canopy +0.002 lower" is wrong. The table gives 0.958−0.950 ≈ 0.0075 lower. Light +0.008 and regen +15 are correct.
- **Unreconciled economy.**
  - B removes 84 stems for −182,544 cash.
  - C removes 9 stems for −245,801 cash. The cheaper job removes ten times fewer trees.
  - A's cash is unchanged over 3 years, so annual fixed cost is not the explanation.
  - The handoff calls this "PASS (cash non-negative)" with no decomposition (contractor hours, mobilisation or minimum-job fee, revenue). The check only proves cash ≥ 0.
- Labels are mostly honest. Stems, BA, cash, canopy, light and regen are DIRECT. Species splits and deltas are DERIVED. The 20% CI tolerance and the 40% BA ceiling are GAMEPLAY CALIBRATION. The handoff does not show that "Path B response consistent with thinning" is a caveated reading rather than a result.

## 2. Completion gap: measurement limitation, not an accepted-outcome break

**Evidence for "gate limitation":**
- `ScenarioOneObjectives.Evaluate` adds `introduced-beech` and `introduced-sessile-oak` objectives for every `shopEntries` species. `ScenarioOne.asset` lists both beech and sessile-oak.
- Each one needs a completed `PlantJuvenile` order for that species, plus presence.
- `EvaluateProgress` completes only if `objectives.All(achieved)` and `HasReviewedTwoCycleSequence`.
- The gate never plants anything. The fixture is Sitka-only (species 1093/0/0).
- Completion is therefore unreachable by construction. This is independent of calibration and of any wider treatment.
- Other objectives are probably met but were never logged: min year 25, ≥60 retained original trees, canopy ≥0.35 (measured about 0.95), regen ≥3 cells (86–101), deadwood ≥0.02 m³ (0.128), and managed opening (done).
- The two-cycle review sequence is likely met already: rack plan year 1, release year 2, and each year is acknowledged.
- The second intervention ("10 trees at year 17") was not the missing ingredient. The gate also never logs `ScenarioOneObjectives.Evaluate` results, so the real missing objectives are unproven.
- The handoff text ("objectives may require more interventions") misattributes the cause.

**Conclusion:** this is not evidence of a material break. The handoff's "not a blocker" claim is directionally right but unsupported: the gate cannot discriminate between the two readings. `BLOCKED — CALIBRATION IMPACT` is not warranted. The pass condition "long-run complete-loop Scenario One remains reachable" is **unmeasured**, not failed.

## 3. Zero timber quote: a real gap

- All path tables show `quoteSold=0`, `quoteRevenue=0`, `quoteCost=0` and `retainedTimberM3=0`.
- In Path B the quote is taken after `Approve` generated 84 orders but before `ApprovePendingWork`. It reports `total=0`.
- Path C takes it after `AddMarkedTreesToWorkPlan`, before approval, and also gets 0. Either `GetHarvestQuote(true)` only counts approved orders, or the quote does not apply to this order set.
- Deadwood volume (0.128 m³) is identical in all three paths and equals the baseline's single deadwood record. Treatment therefore changed neither deadwood nor timber as measured. 84 removed stems left no timber or deadwood trace.
- No cumulative sold volume or revenue was read after execution. "Timber/deadwood outcomes coherent" is **not supported** by the data. The only observation is "unchanged from baseline", and that holds for every path.
- **Smallest fix:** read `GetHarvestQuote(true)` (and `(false)`) after `ApprovePendingWork`, before `AdvanceYear`. After execution, read the cumulative sold, retained and deadwood volumes and the revenue from the management-event or `WorkResolved` records. Report a cash decomposition (revenue, contractor cost, fees) for B and C. This is a gate-only change.

## 4. MFM-03 residual state

**Mostly correct, but incomplete as a "cross-year" check.**
- The task packet requires "actual extension timing/state … post-Lesson-5/cross-year robustness". The gate measures at the baseline: year 2, after sample release, before wider treatment. That is a defensible live post-Lesson-5 state.
- It is the **only** state measured. Growth drift of 0.96 cm is just baseline mean DBH over 2 years. CI parity is 4.17% (source 8.059 vs covered 7.723).
- No CI parity or drift was re-measured at the post-extension states: after B at year 3, or in the 25-year run. The "cross-year" half is therefore not demonstrated.
- The gate also did not check that CI parity holds on the post-treatment state. Natural cohorts and shelters are 0, as expected at year 2, so those are untested rather than clean.
- The `mfm03-ci-parity` note exists at baseline only. It is also guarded by `if (coveredAreas.Count > 0 && coveredCi > 0)`, which would silently skip on an empty set.

## 5. Protected compatibility

**Adequate for save/load, not for the full condition as the gate stands.**
- `WorldHash` is computed and the save/load round-trip preserves it. `479B689F7E12910C` is the hash of a post-25-year Path B state.
- It is never compared with any expected pin. `Check(!IsNullOrEmpty)` plus `==` after a round-trip proves only self-consistency.
- `LegacyV15` and `LegacyV16` are null in that state. That is expected.
- The handoff cites PARITY-09 `REFERENCE_FUTURE_V1_PASS` at `3acdf9cb` for Reference and Legacy40 and does not claim this gate verified them. That is honest. Because production bytes are unchanged, the citation is acceptable as supporting evidence if the Manager accepts a cited external gate.
- The gate adds no new frozen-pin comparison. It has a fixture mismatch with the Reference, so it could not have produced one anyway.
- I did not independently open the PARITY-09 evidence, so I rely on the packet statement that it passed at the exact candidate.
- Recommendation: state this explicitly in the handoff as "protected compatibility = PARITY-09 evidence (cited) plus save/load self-consistency only".

## 6. Handoff completeness and honesty

Unsupported or omitted items:
1. "Timber/deadwood outcomes coherent: PASS". Unsupported (see §3).
2. The B-vs-A canopy figure is wrong (0.002 instead of about 0.0075).
3. The unequal horizon (A 3 years vs B and C 1 year) is not disclosed.
4. The completion gap is attributed to "more interventions". The shop-species planting objectives were not driven or logged.
5. The cash anomaly (B −182k vs C −246k) is not noted.
6. "Wider treatment does not undermine canopy objectives: PASS" rests on a canopy of 0.950 against a threshold of 0.35 at year 3. It is not tested against the completion objectives.
7. Missing: "GAP-4 / protected hashes" are computed but not compared to any pin.
8. "Growth drift negligible" is a single-state measurement.
9. Verdict status "COMPLETE WITH GAP" and the header `Gate HEAD 822e474c` vs delivered `1249c782` differ in the doc text. This is minor.

The offline results in the handoff (per-unit BA ≤40%, species guard) are unreviewed here. I did not open the offline logs.

## 7. Cleanup

Confirmed: U1 clean at `1249c782`, branch `verify/mfm-11-calibration-impact-v1`, lock free, no Editor, no staged harness or `.meta` under `Assets/ForestPrototype/`.

## Can `MULTI-SCALE CALIBRATION QUALIFIED` be issued?

**No, not yet.** The honest state is `QUALIFICATION INCOMPLETE — GAP`, and the handoff's remaining gaps are larger than it admits. A Manager decision on the `BLOCKED` token is not needed. Nothing measured shows a material break.

**Bounded follow-up (gate and docs only, no production change, one more Unity run on U1):**
1. **Equal horizons.** Advance A, B and C the same number of years (3 or 4) from the same baseline, and report all three at the same year.
2. **Timber and economy.** Read the quote after `ApprovePendingWork`. Report cumulative sold, retained and deadwood volumes and revenue after execution, with a cash decomposition for B and C that explains the 182k vs 246k difference.
3. **Completion.** Log the full `ScenarioOneObjectives.Evaluate` results at year 25. Plant beech and sessile-oak via the ordinary shop/work-order path (approved, executed, established), then re-run the two-cycle sequence. Report `Outcome.Completed` or the exact failing objective.
4. **MFM-03 cross-year.** Repeat CI parity and growth drift at the post-extension state (after B, and at a later year). Report cohorts, shelters and deadwood there.
5. **Handoff corrections.** Fix the canopy delta, the "timber coherent" claim and the gap list. Label protected compatibility as PARITY-09 cited plus save/load self-consistency.
