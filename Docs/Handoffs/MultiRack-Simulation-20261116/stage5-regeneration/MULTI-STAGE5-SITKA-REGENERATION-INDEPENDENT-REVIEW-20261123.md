# MULTI Stage 5 Sitka regeneration engine — independent review

**Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code in T3 Code. Not the implementer: the implementation was written by Claude Opus 5.5 in a different session. The reviewer modified nothing on the candidate branch or in the candidate worktree.
**Date:** 2026-11-23. **Scope:** read-only review of a finished candidate, plus a build, a suite run and reviewer-only scratch experiments in a `/tmp` copy that was never committed.

```text
INTEGRATION DECISION
Status: NEEDS CHANGES
Gate: G2 (acceptance): FAIL. Criterion S5-07 failed on reproduction (80 of 80 promotion seed-checks outside [0.75, 1.33]).
      Every other criterion S5-01..S5-06 and S5-08..S5-10 is PASS. A green count cannot compensate for a failed criterion.
Gate: G3 (measurement integrity): PASS. The failure is reported, not hidden: the S5-07 test is left red on purpose, every ratio is
      printed before any assertion, and the three disclosed test-criteria changes are legitimate measurements (S5-02 carries a MINOR caveat).
Reviewed branch: feat/multi-stage5-sitka-regeneration-20261123-v1
Reviewed HEAD: cb739cbde06d293aa76ac1b0d4dc14b76ed02489
Context commit / spec: specs/multi-stage5-sitka-regeneration-v1 @ 85d85a8d (APPROVED)
Spec conformance: FAIL. Only S5-07 / spec §4 with owner resolution Q4 is unmet. Every other spec part (§2, §3, §3.1, §3.2, §3.3, §5, §6, §7, §8, §9, Regeneration 0 byte-identity) is PASS.
Reasons: see §2 (numbered findings F1..F11)
Required fixes / remaining gates: see §9
Integration target: none — no merge authority exists for this task
```

---

## 1. Verdict in plain language

- **The code is a faithful port of the approved rules.** I found no coding defect in the block cap, the tie-breaker, the band accumulator or the Unity browsing arithmetic. Regeneration 0 is byte-identical, and the evidence is a real assertion against pinned constants that were not edited.
- **The one failed gate is a spec-level problem, not a code defect.** The approved §4 / Q5 rule limits promotions to one per aligned 5 m block **per year**. It does not limit them **per cohort**. Four 2.5 m cells each hold a band of the same cohort. Those bands reach 3.5 m together, and the cap only spreads their promotions over the next years. A 5.0 m cell turns the same cohort into one tree. The worker's diagnosis is sound, and my own probe confirms the mechanism (§4).
- **Adopting the worker's recommended option (a) alone would probably not make S5-07 pass as written.** In a reviewer-only probe, a block-level export removed the interior excess (ratio 0.90–1.08 under the shared-light control). The edge-block approximation then became the dominant residual, and under model light the interior ratio stayed far above the tolerance. The owner needs this before choosing between the options (§4.4, F2).
- **Do not integrate and do not call this numerically qualified.** `IMPLEMENTED ON TASK BRANCH` is the correct state word. `NUMERICALLY QUALIFIED` is correctly *not* claimed. The independent-review word `REVIEWED` is this document, with the verdict above.

---

## 2. Findings

Severity words: BLOCKING, MAJOR, MINOR, OBSERVATION. All paths are relative to `Tools/MultiRackSitkaP1Lab/` unless stated.

| # | Severity | Finding | Evidence |
|---|---|---|---|
| F1 | **BLOCKING** | **S5-07 FAILS.** Accepted-abundance ratios are in tolerance (0.815–0.870 model light, 0.830–0.956 shared control; 0/80 outside). Multi-year promotion ratios are outside `[0.75, 1.33]` in **all 80 seed-checks** (4 metric groups × 20 seeds): managed thinning 1.923–2.211 (model) and 2.506–2.959 (shared control); no work 2.057–3.217 and 3.000–4.304. Pooled: 2.072, 2.652, 2.412, 3.467. I reproduced every per-seed line in the handoff appendix, for example seed 1 managed thinning `159/76 = 2.092`, shared `207/76 = 2.724`. Zero out-of-polygon trees held exactly. | `tests/Core.Tests/Stage5GridStudy.cs:72` (exact out-of-polygon assert), `:73-74` (throws on tolerance miss); suite log lines for `S5-07 SUMMARY`; `tests/Core.Tests/Stage5GridStudy.cs:18` (30-year horizon) |
| F2 | **MAJOR** (owner decision input; reviewer-scratch evidence, not candidate evidence) | The worker's recommended option (a), a block-level export of sibling bands, would remove the interior excess but **would not by itself make S5-07 pass as written.** In my probe over the 77 interior 5 m blocks (every one of its four 2.5 m cell centres in polygon, and a 5.0 m cell in the same block): shared-light ratio fell from 2.35 / 2.12 to 1.08 / 0.90, but over the whole woodland it stayed 1.58 (thinning) and 2.07 (no work). Almost all of the remaining excess sits in the 36 edge blocks (fewer than four in-polygon centres, still given a full budget of 1). Under model light the interior ratio stayed 2.2–3.8 even after the export, which means the finer 2.5 m light sampling is a second, separate effect. Q4 does not say whether model light or the shared-light control is the acceptance basis. | §4.4 and Appendix A (probe patch, commands, numbers) |
| F3 | MINOR | **`Stage5Cell.Light` is a pre-promotion snapshot.** Spec §3 step 10 says "then refresh canopy/light cache". The engine's canopy cache (`ReadCanopyLight`) is refreshed from the committed roster, but the Stage 5 state stores the light used *before* promotion. In a thinned 30-year run, cells whose stored light differs from the canopy read-out: 97/383 in year 13 (23 promotions), 54, 70, 55, 70, 46, 63, 36, … down to 13/383 in year 30. Years 1–12 (no promotions) show 0 differing cells, and genesis shows 0/383. No rule reads the stored value, since the next step recomputes light, so no numerical result is affected. A UI that displays `Stage5Cell.Light` beside the canopy overlay would disagree in promotion years. | `src/Core/MultiArea/Stage5Annual.cs:87` (promotion), `:97-103` (next-state cell takes `light[i][c]`, pre-promotion); `src/Core/MultiArea/Stage5Records.cs:19` |
| F4 | MINOR | **S5-02 criterion was replaced after the first run failed** (G3-1). Verdict PASS, with a caveat: the poor-light check uses the 15-year *mean* light (0.102), which hides a rising trend. The dense interior's tallest band reached 2.48 m, so "suppressed" is only comparative (shorter and sparser than the gap, never promoted). The owner may want an absolute definition. | `tests/Core.Tests/Stage5Tests.cs:130-140`, printed line `S5-02 cells 12: … dense max over 15 y 2.48 m` |
| F5 | MINOR | **A test-only seam lives in a production options type.** `Stage5LightSampling.SharedFiveMetreBlockCentre` and `Stage5Options.LightSampling` exist only for the S5-07 control. Default is `CellCentre`; no UI or save path can reach it. It is documented as a control and not a cheat, but it is a test affordance in `src/Core`. | `src/Core/MultiArea/Stage5Models.cs:30-34`, `:51-52`; `src/Core/MultiArea/Stage5Annual.cs:192-206` |
| F6 | MINOR | **Untested corners.** (a) The last tie-break key `(Column, Row)` is never exercised: every competing pair in P-02 differs in establishment year or height. (b) The refusal matrix tests 4 of the 7 incomplete Regeneration-1 selections (`0/0/0`, `0/0/1`, `1/0/1`, `1/1/0`; missing `0/1/0`, `0/1/1`, `1/0/0`), although the code path is one generic branch. (c) A woodland with **no** living adult anywhere but juvenile abundance (the `HasAbundance` bypass of `ContinuationNoLivingStems`) has no test; S5-09 empties only one rack. (d) The A/B-straddling block is not unit-tested; my real-run check (§4.2) shows the cap holds woodland-wide. | `tests/Core.Tests/Stage5Tests.cs:476-500`, `:72-79`, `:539-574`; `src/Core/MultiArea/MultiAreaContinuation.cs:743-745` |
| F7 | MINOR (spec housekeeping) | The approved spec text still carries draft wording in §9 Q4 ("mean relative occupancy … over 10 years") while the binding approval record says "accepted abundance and multi-year promotion ratios". The implementation follows the binding record. A 10-year promotion ratio would be 0/0, because nothing can promote before year 13 (§5, G3-3). | `specs/multi-stage5-sitka-regeneration-v1/spec.md:11` (binding), `:142` (draft Q4 text) |
| F8 | OBSERVATION | **The approval commit rewrote one rule line, not only status/records** (§6). The §4 block-winner row changed from "greatest height, then oldest, then lowest (col,row)" to the owner's Q5 order. The approval record states this openly and the packet lists the same Q5 order. I treat it as owner-sourced, not as a silent change. | `git diff 6c74588e 85d85a8d`; `spec.md:15`, `:111` |
| F9 | OBSERVATION | **Disclosed interpretations that differ in float rounding from Unity, not in rule.** Survival is one double product `light·(1−0.04p)·(1−p_veg)` (spec §3.2 governs); Unity applies the vegetation loss first as a separate float step. Seed rain uses post-adult-step ages and radii and sums in ordinal TreeId order, not Unity's scene order. The mast stream is the new isolated `STAGE5-MAST` stream (owner Q1). None is a rule change. | `src/Core/MultiArea/Stage5Juvenile.cs:84-90` vs Unity `ForestEcologyController.cs:1404-1409` @ `5e79e986` |
| F10 | OBSERVATION | **The shared-light control equalises light *sampling position*, not the light *field* over time.** Each 2.5 m cell reads the light at its aligned 5 m block centre, which is the 5.0 m light field exactly only while both runs have the same roster. Once promotions differ (from year 13) the two canopies diverge. It still isolates the dominant effect: my interior probe with sibling export reaches 0.90–1.08. | `src/Core/MultiArea/Stage5Annual.cs:192-206`; handoff §5.3 |
| F11 | OBSERVATION | **The suite exits non-zero while S5-07 is red.** That is intended and honest. Any CI step that requires exit 0 for this suite will fail until the S5-07 decision is made. | suite log: `201/202 passed, 1 failed` |

---

## 3. Per-criterion table S5-01 … S5-10

| Criterion | Result | Evidence I actually observed |
|---|---|---|
| **S5-01** frozen paths | **PASS** | `S5-01 F-01/F-02/F-03` PASS in my run. F-01 asserts equality of strings against constants that sit in files the diff does not touch: `PinnedMultiAreaHash` (`a692a6d5…`), `PinnedStage3CrownModel1Hash` (`814e8165…`), plus a new `PinnedStage4FullRunHash` `8a926776…` for `1/1/1/0`. The new constant is not self-derived: the base commit's own Stage 4 handoffs and `specs/multi-stage4-ui-catchup-v1/spec.md:99` already record `8a926776…`. `7e3a2a86…`, `c66d566c…` and `9a99f781…` definitions are byte-unchanged between base and head (`git grep` at both commits). `.p0ref/SitkaLab.Core.dll` sha256 = `9a99f78131a3…e1934b3`. Token for 1/1/1/1 is `…stage4-adult-mortality-v1+regeneration-v1`; earlier tokens unchanged. Build 0 warnings, 0 errors (incremental on the candidate; clean `--no-incremental` of a `/tmp` copy of the lab; the UI project also compiles 0/0 there, compile only, not run). |
| **S5-02** dark interior vs managed gap | **PASS** (see F4, G3-1) | Light at year 1: dense 0.003, gap 0.980. Year 12: abundance dense 1.5691 vs gap 4.5000 (full capacity 12 × 0.375); tallest dense 1.74 m vs gap 3.24 m. The dense interior never reached 3.5 m and never promoted in 15 years (max 2.48 m). |
| **S5-03** bramble/bracken | **PASS** | Genesis: 383/383 cells at exactly `0.25·T`, bramble = bracken bitwise. Year 1: gain branch 383/383; loss branch proven on a pure step (`0.9 → 0.45` branch). `p_veg` oracle exact at 0.2, 0.3, 0.9 and ≥1.5 m. Matches Unity `UnderstoreyCompetition` float order (`Target`, `Step`, `LossProbability`). |
| **S5-04** ClearUndergrowth | **PASS** | Pending/Approve leave `Areas` the same reference; Execute zeroes the target and keeps `Bands` by reference; no year or annual-revision change; replay returns a receipt and publishes nothing (`ReferenceEquals` on the root); four drifted batches refuse whole; consented Year+1 cancels clears. Code reads as specified (`MultiAreaContinuation.cs:444`, `:467`, `:696`). |
| **S5-05** browsing | **PASS** (see G3-2) | Pressure 0: height and survival bit-identical to light-only at 21 light values, and no browse loss in whole sessions. Pressure 0.2: `p = 0.2f·0.15f`; increment × `(1−0.9p)`; survival × `(1−0.04p)`; 0.9 m band identical under 0 and 0.2. Matches Unity `JuvenileEcologyRules` factor order (`bg × palatability × V(h)`; vegetation exposure and protection access are 1 here). |
| **S5-06** cross-rack seed rain | **PASS** | A→B and B→A match a double-precision oracle at 1e-5 relative; only immature stems give zero seed and zero bands; mast per year equals the isolated stream with thresholds 0.15 / 0.50 over 12 years; reversed input order gives an identical hash. The reversed-order test calls the pure `Stage5Annual.Step` directly; the continuation always passes areas in definition order. |
| **S5-07** grid consistency | **FAIL** | See F1 and §4. Accepted abundance in tolerance; promotions outside in 80/80 seed-checks; zero out-of-polygon trees exact. |
| **S5-08** promotion | **PASS** | P-01: oldest represented band ≥3.5 m, sub-threshold accumulator never promotes, `Age = max(1, year − establishYear)`, `DBH = clamp(1.5·H, 2, 20)`, `r_0 = max(0.1, r_pot/√(1+CI/5))` checked against an oracle and `r_0 < r_pot`. P-02: one per aligned block, winner oldest then tallest, deferred cells promote in later years, deterministic in-polygon placement. P-03: promoted tree is in the roster, canopy/light woodland and next adult step; IDs unique. Gap: the `(Column, Row)` key is untested (F6). |
| **S5-09** adult-empty rack | **PASS** | Rack B's only stem is injected dead. B's adult step is skipped (`Report is null`), its own ecological year still advances, it receives A's seed, its undergrowth changes, and it promotes in year 14 (printed). The promoted adult then runs the adult step. |
| **S5-10** determinism and atomicity | **PASS** | Two 30-year `1/1/1/1` runs hash identically (`b48aa8fc…`, reproduced). The `1/1/1/1` adult output over 10 years equals `8a926776…`, so Stage 5 does not draw from or reorder `ADULT-MORT-*` (those rolls are stateless per `TreeId`: `AnnualRules.cs:87`; Stage 5 uses only the new keys `STAGE5-MAST` and `STAGE5-PROMO-…`). A forced `beforePublish` failure in Execute and a forced `beforeCommit` failure in Year+1 leave `ReferenceEquals` roots; the retried year equals an uninterrupted twin. |
| S5-11 independent review | this document | — |

---

## 4. The S5-07 failure

### 4.1 Verdict on the diagnosis: **SOUND on the question asked; INCOMPLETE as a full explanation and as a remedy**

The worker's claim: the excess is caused by the approved promotion rule (one 5 m band becomes one tree, whereas four 2.5 m cells each hold a band of the same cohort), and not by a coding defect. I checked this four ways and found it **sound**: no coding defect, and the named mechanism is real and sufficient for the interior under the shared-light control. It is **not the whole story**: edge blocks (§4.4) and the finer light sampling under model light add excess that the handoff's option list does not address (F2).

### 4.2 The block cap really enforces Q5

- **Code.** `Stage5Promotion.Winners` groups candidates by `(BlockX, BlockZ)` across the **whole woodland**, orders groups by block, and takes `OrderBy(EstablishYear).ThenByDescending(Height).ThenBy(Column).ThenBy(Row).First()` per group (`src/Core/MultiArea/Stage5Promotion.cs:46-55`). That is exactly Q5: oldest `EstablishYear`, then tallest, then `(Column, Row)`. `BlockOf` uses the cell centre in the world lattice, so 2.5 m cells 0–1 are block 0 (`:38-40`). Non-winners keep their bands (`Stage5Annual.cs:161-186` removes only the winner's band).
- **Real runs.** In my scratch run (seeds 1–5, 30 years, managed thinning and no work, both grids), the maximum number of promotions in any one `(year, block)` was **1 in all four configurations**. The grouping spans both racks, so a straddling block is covered.
- **Unit test.** P-02 shows the year-by-year deferral: year 20 promotes the oldest-then-tallest cell, year 21 the oldest deferred band, year 22 the younger one.
- **Tie-break.** The first two keys are tested; the third `(Column, Row)` is read in code only (F6).

### 4.3 The excess comes from the approved rule, not a defect

- **Mechanism measured directly.** For 2.5 m, managed thinning, seeds 1–5: 806 promotions; **254 of them (31.5 %) are a second or later tree from a 5 m block whose establishment year was already used by another tree from the same block** (no work: 33 of 373, 8.8 %). At 5.0 m that number is **0 by construction**. Those are the sibling-cell duplicates of one cohort. This count is a lower bound for the splitting effect, because band merges round establishment years, so sibling bands can differ by a year.
- **Timing is not an artefact.** Promotion ratios (pooled over 20 seeds) at horizons y14 / y16 / y18 / y20 / y25 / y30 were 2.12 / 2.70 / 2.58 / 2.61 / 2.20 / 2.07 (managed thinning) and 2.75 / 3.12 / 2.26 / 2.46 / 2.18 / 2.41 (no work). There are no promotions before year 13, so a 10-year horizon is vacuous. The failure is robust to the horizon choice.
- **Causal test.** A scratch variant that lets a block's winner also consume its siblings' qualifying bands (Appendix A) brings the interior shared-light ratio from 2.35 / 2.12 to **1.08 / 0.90**. A coding defect in the cap, tie-break or accumulator would not respond that cleanly to a change in the promotion rule.
- **Accumulator.** Accepted abundance sits in tolerance in every seed (0.82–0.96), and the band logic is a line-by-line match of Unity `NormalizeBands`, `MergeBands`, `AdmitDensity` and `SharedOccupancy` with only the §4 scaling added. No defect found.
- **Why the approved rule cannot meet Q4.** The cap limits trees **per block per year**. It does not limit trees **per cohort**. The spec §4 rationale ("caps trees/ha/yr at Unity's ceiling") holds for the annual ceiling, which is rarely binding at 5.0 m, but not for the total. Q5 plus "non-winners stay and can promote next year" lets one cohort yield up to four trees at 2.5 m.

### 4.4 The edge-block effect is disclosed honestly, with one more thing the owner should know

- **The disclosure is accurate.** I reproduced the figures: 36 edge blocks of 113; 1213 of 3170 (38.3 %) managed-thinning promotions and 674 of 1440 (46.8 %) no-work promotions fall in them. The subtraction `(3170 − 1213)/1530 = 1.28` reproduces the lower end of the worker's "1.3–2.3" statement. I could not reproduce the upper end from the printed numbers (the shared-light edge counts are not printed).
- **"Edge blocks do not explain the excess" holds like-for-like.** Over the 77 interior blocks that exist at both grids, 30 years, 20 seeds: managed thinning model 2.31 (1957 vs 848), shared 2.35; no work model 3.95 (766 vs 194), shared 2.12. All are far outside tolerance even with edge blocks removed on both sides.
- **But the edge blocks become the main residual once the interior duplication is removed** (reviewer scratch, F2). Whole-woodland shared-light ratio with the sibling export is 1.58 and 2.07; interior only, 1.08 and 0.90. The remaining excess is almost entirely in the 36 edge blocks.
- **The finer light sampling appears to be a separate effect.** Under model light, even the exported interior stays at 2.23 (thinning) and 3.76 (no work), against 1.08 and 0.90 under the shared-light control. In the unexported baseline, the no-work interior promotes 766 trees under model light against 412 under shared light, at the same blocks and seeds. A different promotion rule cannot fix that part. I measured the size of the effect, not its mechanism.
- **A property of the control, not of the engine.** In the shared-light control, an edge block can sample light at a block centre outside the polygon, where there are fewer neighbouring crowns, so the light may be brighter (I did not measure how many blocks do). This may inflate the control's edge-block result. It is a Q4 clarification for the owner: which basis is the acceptance basis, and is an edge-block normalisation allowed?

---

## 5. The three G3 test-criteria changes

| # | Change | Verdict | Reasoning |
|---|---|---|---|
| G3-1 | **S5-02:** a self-authored "dense interior stays under 1.5 m" cap was replaced by the spec's reading of "suppressed". | **PASS** (MINOR caveat, F4) | The 1.5 m number was never in the spec (`checklists/requirements.md` S5-02 says only "stays suppressed"). The failing run exposed a real model behaviour: Area A self-thins, so interior light rises from 0.003 to a 15-year mean of 0.102 and the tallest band reaches 2.48 m. The test now asserts spec-derived, comparative criteria (never reaches 3.5 m or promotes, poor-light band, shorter and sparser than the gap at year 12, gap grows past 1.5 m) and prints all values. The change is disclosed with the first-run numbers. It does not hide a failure of a spec criterion. Caveat: the poor-light assertion is on the 15-year mean, which is a weak statement of "suppressed", and a 2.48 m band is not suppressed in an absolute sense. |
| G3-2 | **S5-05:** `p ≤ 0.03` carries a `1e-7` tolerance. | **PASS** | Unity's float expression `0.2f × 0.15f` is 0.030000001; the spec's 0.03 is the real-number product. The line before the tolerance asserts the **exact bits** of `0.2f * 0.15f * 1f` (`tests/Core.Tests/Stage5Tests.cs:321`), so the tolerance at `:323` cannot hide a deviation. `1e-7` is about 50 float ulps at 0.03 and about 3e-6 relative. |
| G3-3 | **S5-07:** horizon set to 30 years. | **PASS** | A 10-year horizon cannot produce a promotion: `0.15 + 10 × 0.35 × 0.9 = 3.30 m < 3.5 m`, and real runs show the first promotion in year 13. The binding Q4 record has no horizon; "10 years" appears only in the draft Q4 text (F7). My horizon sweep shows the failure at every horizon with promotions (2.07–3.12). The test is stricter than the record in one respect, because every seed must be inside the tolerance, but pooled ratios fail too, so the verdict does not depend on that. |

**Measurement-integrity checks that also passed.** The S5-07 test prints every ratio before it asserts. It asserts zero out-of-polygon trees exactly and asserts the 5.0 m control equals the 5.0 m model. The study uses the same woodland, 20 deterministic seeds, and two scenarios, and it reports accepted abundance and promotions separately. Nothing was deleted, skipped or loosened to turn the test green.

---

## 6. Spec-approval integrity: `6c74588e → 85d85a8d`

**Result: PASS, with one flagged line (F8).** The diff touches only the four quartet files. It adds the owner approval record and the Q1–Q6 resolutions, changes the status banners to APPROVED, ticks G02/G03/G04/T0d, rewrites two "open question" phrases as "resolved", and updates the plan trade-off sentence. **One rule line changed:** the §4 table row "Block winner" went from "greatest height, then oldest, then lowest `(col,row)`" to "oldest `EstablishYear`, then tallest height, then lowest `(Column, Row)`". The approval record says in so many words that the earlier §4 block-winner order is governed by the Q5 resolution, and the task packet lists the same Q5 order. I therefore treat it as owner-sourced and disclosed, not as a silent substantive change. No other rule, constant or formula changed.

The four spec blobs on the feature branch are **byte-identical** to the approved docs-branch blobs at `85d85a8d` (`git rev-parse` of all four paths matches).

---

## 7. Port fidelity against Unity `main` @ `5e79e986`

I read `JuvenileEcologyRules.cs`, `UnderstoreyCompetition.cs`, `ForestEcologyController.cs` (annual order, `UpdateMast`, `ComputeSpeciesSeedRain`, `GrowExistingRegeneration`, `EstablishNewBands`, `AdmitRecruitment`, `NormalizeBands`, `MergeBands`, `PromoteSpeciesCohorts`, `OldestQualifyingBand`), `ForestEcologyCell.cs`, `RegenerationModel.cs`, `TreeSpeciesDefinition.cs` and `SitkaSpruce.asset`.

| Rule | Port | Unity | Result |
|---|---|---|---|
| Annual order 6 → 7 → 8 → 9 → 10 | `Stage5Annual.Step` | `ForestEcologyController.cs` Update, steps 6–10 | match |
| Light response (piecewise, clamp, InverseLerp, Lerp) | `Stage5Juvenile.LightResponse` | `TreeSpeciesDefinition.InterpolateResponse` | match |
| Light survival `1d − 0.2f` (unrounded double complement) | `LightSurvival` | `JuvenileEcologyRules.SurvivalResponse` | match |
| Height growth, browsed and light-only paths | `GrowHeight` | `JuvenileEcologyRules.GrowHeight` | match, same float order |
| `V(h)`, `p = min(0.95, bg·0.15·V)` | `BrowseProbability` | `AssessBrowse` | match (vegetation exposure and protection access are 1) |
| Browse height | pre-growth | `AssessCohortBrowse(…, cohort.Height)` before growth | match (Q6) |
| Bramble/bracken `Target`, `Step`, `Initial`, `LossProbability` | `Stage5Undergrowth` | `UnderstoreyCompetition` | match, same float order |
| Mast thresholds, good 1.5 / poor 0.4 / normal 1.0 | `Stage5Mast.FromRoll` | `UpdateMast` | match; stream differs by owner Q1 |
| Seed rain: maturity, `clamp01(r/4)`, `exp(−d/20)`, cutoff 150 | `Stage5SeedRain` | `ComputeSpeciesSeedRain` | match; source order is ordinal TreeId by design |
| Establishment `1 − exp(−S/S50)`, skip ≤ 0.01, `×0.5` | `RequestedEstablishment` | `EstablishNewBands` | match, plus the §4 `×a` |
| `AdmitDensity`, `SharedOccupancy`, capacity floor 0.01 | `Stage5Bands` | `ForestEcologyCell` | match, capacity scaled |
| `AdmitRecruitment`, `Normalize`, `MergeBands`, `InsertBand` | `Stage5Bands` | `ForestEcologyController` | match, single species and origin |
| Promotion: oldest qualifying band, `CanPromote`, DBH clamp, age | `Stage5Promotion` | `PromoteSpeciesCohorts`, `OldestQualifyingBand` | match; `r_0` from CrownModel 1 and the ID format are spec changes |

**No invented variable and no silently changed constant.** Every constant in `Stage5Constants` equals the Unity asset value or code literal, or is a spec-stated value. The only value the worker had to choose is `PromotionPlacementAttempts = 8`, which the spec leaves as "a fixed number" and the code tags as a simulation abstraction. The port deliberately omits only diagnostic counters (`SubThresholdRecruitment`) and the origin tie-break (one origin exists). Stage 5 adds no place where the port reinterprets a rule.

---

## 8. Hygiene

- **Allowlist.** All 24 changed paths lie under `src/Core/**`, `tests/Core.Tests/**`, `specs/multi-stage5-sitka-regeneration-v1/**` or the Stage 5 handoff folder. There is nothing under `Assets/`, no `.meta`, no `.sln`, no `.csproj`, no `.p0ref`, no `src/Ui/**` and no `wwwroot`. `.p0ref` sha256 is unchanged.
- **Pinned constants.** The files that define the pins (`Model0FingerprintTests.cs`, `Stage4Tests.cs`, `MultiAreaTests.cs`, `SitkaP1.Core.csproj`, the shell smoke scripts) are not in the diff. The only 64-hex literal added in `src/` and `tests/` is `PinnedStage4FullRunHash`.
- **Pre-existing tests.** The only edits to existing test code are `Program.cs` (+1 registration line) and `Stage4BehaviourTests.RunFingerprint` (made `internal` with an optional selection; its default path is unchanged and still reproduces `8a926776…`). All 184 pre-existing tests pass.
- **Save schema and `/multi`.** No change. `MultiAreaModelSelection` gains a defaulted fourth member; `src/Ui` never mentions it, and the UI project compiles 0/0 in my scratch copy.
- **Cheat scan.** No `Environment`, `#if`, skip or test-detection branch in the Stage 5 production files. No constant tuned toward a test. The one test-affordance seam is F5.
- **Worktree cleanliness.** The candidate worktree had 0 `git status` lines before and after my build; `HEAD` stayed `cb739cbd`; the base `e7c1a57d` is an ancestor of `HEAD`. The remote head of the candidate branch is `cb739cbd`.

---

## 9. Required fixes and remaining gates

**Blocking the integration decision**

1. **Owner decision on the promotion budget** (spec §4 / Q5). The candidate implements the approved text correctly, so this is a spec revision, not a code fix. Options on the table: (a) block-level export, (b) fractional-promotion accumulator, (c) accept a different tolerance, (d) run Stage 5 on the 5.0 m grid only. Use §4.4: option (a) alone removed the interior excess in my probe but left the whole-woodland ratio at 1.58–2.07.
2. **Clarify Q4 before the next implementation pass:** the acceptance basis (model light, or the shared-light control that isolates the policy), and how the edge-block approximation is to be treated or normalised.
3. **After a revised, approved spec:** a new implementation pass, S5-07 green on the agreed basis, then a **delta review** of the changed promotion code and the new test.

**Recommended but not blocking** (F3, F4, F5, F6, F7): refresh or document the stored `Stage5Cell.Light`; add tests for the `(Column, Row)` tie-break, the remaining three refusal combinations and the all-racks-empty path; decide whether the `LightSampling` seam stays in `src/Core`; clean the draft Q4 wording in the spec.

**Remaining gates (none can be claimed from this review):** G2 re-run after the fix; owner acceptance; D-091 integration; any Unity/UI work in a later stage. No merge authority exists for this task.

---

## 10. What I did not verify

- **Unity runtime parity.** I compared source text at `5e79e986` with the port. Nothing was run side by side, and no Unity process was started.
- **Ecological validity.** Every constant is gameplay calibration or transfer; light is a modelled index, not PAR. I claim nothing about real Irish regeneration.
- **The owner's own words.** I took Q1–Q6 from the approved spec text and the task packet. I could not hear the owner decision.
- **UI runtime and `/multi`.** The UI project was compiled in my scratch copy only. No page or smoke script ran.
- **The base build.** I did not rebuild the base. Byte-identity relies on the unchanged pinned-constant files, the base's recorded `8a926776…`, and the passing assertions, not on a fresh base run.
- **Windows host, other OS and CI.** I ran the lab under WSL with .NET 8.0.134 only.
- **Cross-process determinism.** The runs I compared were in one process; the hashes I reproduced equal the worker's, which is weak cross-process evidence only.
- **The upper end of the worker's "1.3–2.3" edge-block statement** (§4.4).
- **Performance and memory** beyond the observed suite runtime.
- **Any code outside the 24-file diff**, except where I read it to check a call site (`MultiAreaContinuation`, `AnnualRules`, `CrownModel`, `CanopyLightEvaluator`, `SimulationRandom`).
- **GitHub rendering** of this document.

---

## Appendix A — what I ran

All commands used WSL git and `/home/jer/.dotnet/dotnet` 8.0.134. Windows `git.exe` was not used. Nothing touched Unity, `Assets/`, the saves or the protected listeners.

1. `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` on the candidate: **0 Warning(s), 0 Error(s)** (incremental). `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll`: **201/202 passed, 1 failed (S5-07)**. `1/1/1/0` fingerprint `8a926776…`; `1/1/1/1` fingerprint `b48aa8fc…` (both equal the handoff).
2. A `/tmp` copy of `Tools/MultiRackSitkaP1Lab` (plus the two linked Unity source files it compiles), built with `--no-incremental` before any scratch edit: **0 / 0**. `src/Ui/SitkaP1.Ui.csproj` compiled in the same copy: **0 / 0** (this build ran after my env-gated probe edit to `Stage5Annual.cs`, which the UI project does not touch). It is compile evidence only, not run, and not a build of the candidate worktree itself.
3. Reviewer-only scratch diagnostics in that copy, never committed:
   - **D1** block cap in real runs, seeds 1–5, 30 years, both scenarios, both grids: max promotions per `(year, block)` = 1 in all four configurations; sibling-cohort duplicates 254/806 (2.5 m thinning), 33/373 (2.5 m no work), 0 at 5.0 m.
   - **D2** horizon sweep of the pooled promotion ratio (§4.3).
   - **D3 / D3b** Stage 5 cell light against the engine's canopy read-out: 0/383 differing at genesis; 97/383 in year 13, falling to 13/383 in year 30 (F3).
   - **D4** like-for-like promotions over the 77 interior blocks, baseline: thinning 2.308 (model) / 2.354 (shared); no work 3.948 / 2.124.
   - **Causal probe (not a fix, not endorsed).** In `Stage5Annual.Promote`, immediately after `bands.Remove(band);`, when `REVIEW_VARIANT` is set, every other candidate cell of the winner's block also removes its own oldest qualifying band (`ALL`) or only if that band has the winner's `EstablishYear` (`SAMEYEAR`). Results over the 77 interior blocks (thinning / no work): shared-light ratio `ALL` 1.077 / 0.897, `SAMEYEAR` 1.080 / 0.912; model-light ratio `ALL` 2.228 / 3.758, `SAMEYEAR` 2.254 / 3.892. Whole-woodland S5-07 harness with `ALL`: promotions model 2.002 / 2.335, shared 1.575 / 2.067; accepted abundance unchanged in tolerance. The probe breaks other unit tests by design and proves only the mechanism.
4. `git diff`, `git grep` and `git rev-parse` checks listed in §6 and §8.

The temporary review worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage5-SitkaRegen-Review-20261123` was created from `cb739cbd` on the new branch `review/multi-stage5-sitka-regen-20261123-v1`, holds only this file as its change, and is removed after the push with a normal `git worktree remove`.

---

PR none, reviewed head cb739cbde06d293aa76ac1b0d4dc14b76ed02489, review verdict NEEDS CHANGES, integration none
