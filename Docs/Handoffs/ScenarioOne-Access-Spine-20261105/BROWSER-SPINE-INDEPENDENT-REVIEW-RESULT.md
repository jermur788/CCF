# #286 — Independent review of #283 access-spine DRAFT research (draft PR #285)

**Date:** 2026-11-05. **Status:** **[HANDED-OFF] — REVIEW ONLY.** No spec approval, no implementation approval, no Unity, no game/scene/save/build change.
**Claim:** [#286](https://github.com/jermur788/CCF/issues/286). **Reviewed:** [draft PR #285](https://github.com/jermur788/CCF/pull/285) (`docs/access-spine-start-condition-20261105-v1`, head `4128757b9188cb3325ed87ef3b94b35d02044949`, OPEN / draft / unmerged) and its [research result](https://github.com/jermur788/CCF/blob/4128757b9188cb3325ed87ef3b94b35d02044949/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-SPEC-RESEARCH-RESULT.md).
**Reviewer / provider:** Claude Sonnet 5.5 Medium (`claude-sonnet-5-5`), independent of the GPT-6 author of #283. Account/quota capacity was not observable and no switch occurred.
**Starting state:** `main` `50042bbe97734b031210ddebb603b5ec00ff459d`; review branch `docs/access-spine-independent-review-20261105-v1` base `49021323ad8129957fd20f79c21aea77159c4254` (packet commit only; 1 behind `main` by the #286 registration commit — docs-only, no overlap). PR #285 is 6 ahead / 4 behind `main`; the four newer `main` commits touch only `active-tasks.md` and the #286 packet, so there is no path overlap with the PR.
**Method:** GitHub API reads of the five PR files; a read-only sparse clone of `main` `50042bbe` in a scratch directory outside any CCF worktree; disposable stdlib-Python arithmetic and scene-YAML parsing (nothing written to the repo); published #248/#250/#251/#253/#271 records read as documents. **Not run:** Unity, any game test, any harness.
**Independence disclosure:** `active-tasks.md` records that #246 and #251 were authored by Claude Sonnet 5.5 Medium. This session has no access to those sessions and treated their outputs as published documents; every #251-derived claim below was checked against current source or the independently corrected Manager records, not taken from #251's own conclusions. Manager should weigh same-family risk on that subset.

Source references are `path:line` on `main` `50042bbe` unless stated (permalink base: `https://github.com/jermur788/CCF/blob/50042bbe97734b031210ddebb603b5ec00ff459d/`). Paths under `Assets/ForestPrototype/` are abbreviated **AFP**.

---

## 1. Verdict (research and draft quality only): **NEEDS CHANGES**

The draft is honest where it matters most. It stays DRAFT/STOP, does not edit D-064, D-065 or D-070, does not present E as approved, keeps proposal/confirm/mark/approve/execute separate, and transcribes the corrected #248–#253 figures faithfully (§4). None of the seven required amendments below needs Unity.

It is **not yet safe for a product decision on E**, for five reasons:

1. The E-vs-F comparison leaves out the largest measured cost of E, the MFM coverage loss. It also folds the rack-topology choice into the start-condition choice (**B1**).
2. The workbench-retirement contract promises "Enlarged80 only" but the proposed mechanism cannot deliver it. It also omits the decision, anchors and tests that constrain it (**B2, B3**).
3. The MFM Option A eligibility and selection contract is not yet testable (**B4**).
4. The save/start-identity design for E is stated as a requirement but not as a choice between feasible designs (**B5**).
5. One validation rule is presented as immutable when it is a decision (**B6**), and the four files contain stale status text that contradicts the result file (**B7**).

**What is solid:** the central-route and headland source map, the rack-editing and headland-only claim, the Confirm/reopen/lock description, the Track/Headland/Site/Clearing mask description, the hidden-workbench collider and Site-mask finding, the "no evidence of an invisible colliding box" restraint, the evidence classification, and the honest runtime-unproven list.

---

## 2. Review matrix (packet items 2–8)

| # | Packet item | Result | Notes (details in §3–§7) |
|---|---|---|---|
| 2 | All four changed files plus result read; remain DRAFT/STOP; no silent D-064/D-065/D-070 change; E not presented as approved | **PASS** | PR touches exactly five files (4 spec + 1 handoff); none in `Docs/Project/` or approved specs. Headers are "DRAFT — STOP". Q1 "E" is labelled a recommendation and OPEN. §1a states the user "has not yet explicitly approved" E. |
| 3 | Independent source verification on `main` | **PASS WITH CORRECTIONS** | 11 PASS, 4 PASS-with-correction, 0 FAIL; see §3. |
| 4 | Numeric claims vs published corrected records | **PASS WITH QUALIFIER GAPS** | Every figure traces to a published record; none unsupported. Several lack the qualifier a decision-maker needs (§4). |
| 5 | E / P / F comparison honesty | **NEEDS CHANGES** | E's recommendation does not disclose its coupled MFM cost, and its start-baseline, save-identity and workbench dependencies are under-specified (B1, B2, B3, B5). D-070 is correctly separated from E approval and scoped to #284. |
| 6 | MFM conditional A | **NEEDS CHANGES** | Safety intent right; eligibility, selection point and testability gaps (B4). |
| 7 | State vocabulary, test matrix, task/file ownership | **PASS WITH CORRECTIONS** | Vocabulary coherent. Task ownership omits real consumers (B4, B3). Status text stale (B7). |
| 8 | Verdict, numbered blockers, runtime-unproven list, further user choices | **Delivered** | §1, §6, §8, §9. |

---

## 3. Independent source verification (packet item 3)

Legend: **D** direct source or scene read; **O** disposable offline Python reproduction (not Unity); **P** published corrected record; **U** unverified (needs Unity).

| # | Claim in #285 | Verdict | Evidence |
|---|---|---|---|
| S1 | Scene-authored central `Dirt Path 00…51` ends near the work clearing at z≈+10.75; bowed, not due north | **PASS (D)** | `ForestTest.unity`: 52 planks, z −14.75 → +10.75, x −2.715 → +3.00, all active. `Dirt Path 51` = (−2.7148, 10.75). `Forest Clearing` = (−2.772, 11.0). |
| S2 | Plank supplies art only; ground supplies collision | **PASS (D)** | `Dirt Path 00` has Transform, MeshFilter, MeshRenderer only; no collider component. |
| S3 | South approach is procedural, Enlarged80-only, extends to the property edge; old saves use occupancy inference | **PASS (D)** | `StandApproachPath.cs:23-72`; `StandApproachRoute.cs:292-307` (`WorldHasApproach` reads original-plantation tree ids within the 1.5 m corridor — no save field); `ForestSaveController.cs:355-357`. |
| S4 | Headland/feeder constants are separate and duplicate route coordinates | **PASS (D, O)** | `ScenarioOneHeadlandGeometry.cs:7-14` (HeadlandX −35.4, FeederZ −27.4, PathX −2.436166, width 2.6). **O:** replaying `StandApproachRoute.Build` from the scene's `Dirt Path 00` pose (yaw 27.169°) gives route x at z=−27.4 of **−2.4361656**, equal to the literal `PathX`. The literal is a hand copy of a computed route. |
| S5 | Stand Map reads `Dirt Path*` by name; headland is not a map "track" | **PASS (D)** | `UI/Map/StandMapSource.cs:348-360`: name-prefix filter, **ordinal name sort**, a >2 m jump starts a new line. No headland read in this file. |
| S6 | Rack editing and entrances are headland-only | **PASS (D)** | `Racks/RackEditing.cs:123-124` (`EntranceOnHeadland`), `:162-163` (`EntranceOffHeadland`). `ScenarioOneSuggestedRackLayout.cs` also consumes the headland constants (not mentioned in #285). |
| S7 | Confirm creates pending work only; reopen path; completed removals lock geometry | **PASS (D)** | `ScenarioOneManager.RackPlan.cs:239` (`ConfirmRackPlan`), `:346-354` (reopen), D-064 OPEN-3. |
| S8 | Confirm / edit do not validate MFM derivation (the "lifecycle gap") | **PASS (D)** | The only `ScenarioOneRackSampleGeometry.Derive` call sites are `ScenarioOneManager.PrescriptionApproval.cs:261`, `UI/LearningObjectivesView.Sequence.cs:69`, `UI/StandMapTreatmentReview.cs:145`. None is in the rack-plan or rack-editing path. `ForestSaveValidation.RackV2.cs:35-36` requires Enlarged80 but checks neither headland entrance nor derivation. |
| S9 | Legacy `Derive` rejects the two-family candidate | **PASS WITH CORRECTION (D)** | `ScenarioOneRackSampleGeometry.cs:182` (exactly 5 racks), `:244-246` (refuses only when projected intervals "cross or overlap across the common normal"). The test is **not** a parallel-only test; there is no deviation cap (`MaxAxisDeviationDegrees` is reported, never gated). See B4. |
| S10 | Masks: `Dirt Path*` → Track (1.5 m circles), clearing → Clearing, headland → Headland, **every `ForestBuildable` incl. inactive → Site (2.2 m)** | **PASS (D)** | `ScenarioOneCoverageWorld.cs:73-79, 95-101`; `ScenarioOneManager.PrescriptionApproval.cs:338-350` (`FindObjectsInactive.Include` for buildables). |
| S11 | Starting stand reserves clearance for inactive buildables | **PASS (D)** | `ForestStartingStand.cs:325-344`. |
| S12 | Hidden workbench collider is disabled at runtime; no invisible colliding box demonstrated | **PASS (D)** | `ForestBuildable.cs:182-194` (`solid = built \|\| constructionAvailable`). Corroboration #285 does not cite: `Tools/Verification/StandGeometry/Enlarged80Verification.cs:260-265` asserts every site has no enabled collider; D-055. |
| S13 | Other buildables require the workbench; destroying only the scene root can leave dangling references | **PASS WITH CORRECTION (D)** | See B3: **seven** sites, two more dependants than #285 lists, and the dangling reference **fails open** (no exception). |
| S14 | Saved `workbench-01` identity is a compatibility risk | **PASS WITH CORRECTION (D)** | Severity overstated. `ForestSaveController.cs:163-169` writes one entry per scene buildable (all `built:false`); load `:464-473` iterates saved ids against live buildables and **silently skips any id with no live match**. No save validation references buildables (`ForestSaveData.cs:36,231`). A missing `workbench-01` is benign on load. |
| S15 | Old Enlarged80, Legacy40 and Reference Future v1 need explicit protection | **PASS (D)** | D-042, D-056, `Docs/Project/unity-project-overview.md:343,368`. See B2 for what #285 fails to name. |

---

## 4. Numeric audit (packet item 4)

All figures below trace to a **published** record. I did not rerun Unity, and I treat the 18.274, 120 and 38.22 figures as published corrected measurements, not as findings of this review.

| Figure | Where used in #285 | Published source | Status |
|---|---|---|---|
| **0.214214 m physical gap** between R1/R2 corridor rectangles vs **1.414 m centreline** failing the **2.4 m** rule | result §2, spec §6.1/6.3, AS-R04 | `MANAGER-FIVE-RACK-REVIEW-20261103.md:3,11`; `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md:12`; rule is `Racks/RackEditing.cs:102,176` (`OverlapClearanceM = DefaultWidthM = 2.4`, tested with `SegmentDistance` between **centreline segments**) | **PASS.** Terminology is correct and the withdrawn "rectangles overlap" is not reintroduced. **O:** 0.214214 equals √2 − 1.2 (0.2142136), consistent with the stated separation and a 1.2 m half-width. I did not rederive the rectangle geometry. |
| **16.623 / 17.510 / 17.855 m** same-side spacing (3W+2E) | result §2, spec §6.1 | `MANAGER-FIVE-RACK-REVIEW-20261103.md:11` | **PASS.** Note these are one concrete candidate's gaps, not the 18.274 best case (below). |
| **~18.274 m** best-sampled 45° minimum gap | spec §5, AS-R04 | `MANAGER-MEASUREMENT-REVIEW-20261103.md:14` (worker 18.294, independent 18.274) | **PASS.** Lower independent value used. Qualifier gap: a reader may assume the five-rack candidate attains 18.274; it attains 16.623 minimum. State that 18.274 is a best-case maximum-of-minima over sampled placements. |
| **~120 distinct trees** in proposed corridors | result §2, spec §6.1 | `MANAGER-FIVE-RACK-REVIEW-20261103.md:14` (103 rack + 18 spine − 1 duplicate) | **PASS.** Correctly labelled "not Fell instructions". |
| **18 stems / 65.80 m²** (spine alone), **31 / 125.96 m²** (spine + one branch) lost to Track mask | spec §5 (D-068) | `MANAGER-MEASUREMENT-REVIEW-20261103.md:16` | **PASS** as hypothetical-spine sensitivity. Under E the stems are not planted at all, so this is a start-baseline change, not a treatment-exclusion change; spec §6.2 half-says this. |
| **3–38 trees per branch corridor** | spec §5 | `MANAGER-MEASUREMENT-REVIEW-20261103.md:17` | **PASS.** |
| **24.3 m** northern extension-only zone vs 32 m needed | spec §5 (D-068) | `MANAGER-MEASUREMENT-REVIEW-20261103.md:13` | **PASS.** Scoped to 3+2 parallel branches; the spec preserves that scope. |
| **1,575 m²** naive double ownership; **2,246 / 1,718.25 / 830 m²** beyond 9 / 12 / 20 m (nearest-segment owner) | spec §5 (D-069) | `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md:21,29`; `MANAGER-MFM-TOPOLOGY-REVIEW-20261104.md:19-20` | **PASS.** |
| **38.22% (9 m) / 14.13% (20 m)** uncovered, nearest-segment owner | spec §6.4(3) | `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md:30`; reproduced in `Docs/Handoffs/Overnight-Continuation-20261104/MFM-A-VS-C-COMPARISON-HANDOFF.md` V-2 (PR #274, unmerged) | **PASS WITH QUALIFIERS.** Denominator is the **5,875.75 m² unmasked non-corridor lattice area** on one layout (#253 F-2, F-7); not stated in #285. 2,246/5,875.75 = 38.22%, 830/5,875.75 = 14.13% (checked). |
| Lateral owner **36.25–49.03%** uncovered; **69.17 m** maximum reach; nearest-segment **41.18 m** | spec §6.4(3) says only "infinite-line owner gives unbounded reach" | `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md:29-30` | **Omitted, not wrong.** Add: the packet asked for nearest *and* lateral ratios. |
| Sample footprint straddles the other group | spec §6.4(4) "contaminates" | #253 F-3: **150.6 m²** in other units (143.0 m² in R4C) | **Omitted number.** Add it. |
| **0.035 m** workbench-to-spine; Grinding Stone **1.367 m** | spec §1a, result §2 | `MANAGER-FIVE-RACK-REVIEW-20261103.md:13` | **PASS (O).** From scene coordinates: workbench (−2.75, 11) to the line x = −2.7148 is 0.035 m; Grinding Stone (−2.5, 9.4) to the `Dirt Path 51` end is **1.367 m**. |

**Unsupported numeric assertions found:** none. **Missing qualifiers:** the three flagged above.

**#271 / PR #274.** #285 treats it as experimental, unmerged, and not a fair single-layout C substitute. That matches the PR's own verdict ("NO-COMPARISON", V-4/V-5), PR state OPEN/unmerged, and `active-tasks.md`. **PASS.** The data it holds that matters to B1 is in §6.

---

## 5. E / P / F assessment (packet item 5)

**Done well:** E, P and F are distinguished; P is correctly refused a fake approval button; F is named an explicit fallback, not a hidden success; D-070 is scoped to #284 and explicitly not E approval (spec §6.6, result §4); new-start masks, persistent infrastructure identity, old-save protection and corridor/turn/access are listed as requirements, not assumed.

**Not yet honest enough to recommend E to a decision-maker:** see B1, B2, B3, B5.

---

## 6. Numbered blockers and precise amendments

Severity: **High** blocks Manager's product decision on E; **Medium** blocks spec disposition.

### B1 (High) — The E-vs-F decision omits E's MFM coverage cost and conflates two decision axes

**Finding.** The §6.2 decision table has rows for Objective 1, masks, physical representation, economics, old saves and verification burden. It has **no row for MFM coverage**, although the published data show it is the largest measured difference:

- The legacy/parallel layout (F) leaves **0.00%** of non-corridor ground beyond 20 m and **3.11%** beyond 9 m, with 14/14 units covered and 100.0% (cap 20 m) / 95.4% (cap 9 m) of treatable stems covered (PR #274 V-3, T2; the 96.9%-within-9 m figure for the suggested layout is in #253 F-2).
- The two-family herringbone under Option A leaves **14.13–38.22%** (nearest owner) or **36.25–49.03%** (lateral owner) beyond 20–9 m. Under cap 20 m only 9 of 14 units are covered and covered stems are ≤81.2% (nearest) / 54.0% (lateral); cap 9 m gives 47.1% / 38.5% (PR #274 T2; the "covered" count is a ceiling).

These A figures are unmerged-PR sensitivities on one layout with unapproved caps. They are still the only published evidence on what E costs the lesson, and #285 shows none of it.

Also, Q1 (E) and Q5 (Option A) are listed as separate decisions. They are **coupled**: the herringbone makes legacy `Derive` return `Valid=false`, so adopting the E herringbone commits to Option A, or to a C-variant that, per PR #274, either needs an accepted Rack V2 change or breaks the 16–20 m guidance (C-S45 at 13.75 m). There is also an unlisted middle option: an E spine with a **single direction family** entering it (C-S45 leaves 3.06% beyond 20 m, nearest owner). The E/P/F axis is the *start condition*; the second axis is *rack topology* (headland-parallel, spine single-family, two-family A). #285 mixes them in one table.

**Amendment.** (a) Add an "MFM coverage / Manual share" row to the §6.2 table showing the F and A ranges above with their provenance (PR #274 unmerged; #253 merged) and the "unapproved cap, unmasked, ceiling" caveats. (b) Add a second axis for rack topology, and state in Q1/Q5 that E+herringbone is conditional on A or a C-variant. (c) Reword "Recommendation… E is the most coherent teaching route" as conditional on an accepted minimum Manual/NotCovered share. That share is a product decision (§8 D6).

### B2 (High) — The workbench retirement contract (#284) cannot be "Enlarged80 only" as proposed, and it omits the constraints that apply

**Finding.** §6.6 proposes, "for future new Enlarged80 starts only, remove workbench registration/site exclusion and authored placement." But the workbench is a **scene-authored root** in `ForestTest.unity`, the same scene Legacy40 uses ("the scene stays the authored Legacy40 baseline on disk", `StandApproachPath.cs:5-8`). `ForestStartingStand.CollectClearancePoints()` (`:325-344`) feeds the Legacy40 `Generate()` (`:100`) and the Enlarged80 **core** ("the Legacy40 generation, step for step", `:193`). Deleting the root therefore changes both. The only repo precedent for an Enlarged80-only change to a Legacy40 scene is the **runtime** `Apply`/`Reconcile` pattern of `StandApproachPath` and `StandWorldBoundary`.

Constraints #285 does not name:

- **D-056** accepts "the central 336 legacy tree IDs and exact positions are retained." Workbench removal frees a 2.2 m clearance circle at the clearing centre, so previously omitted core slots become eligible and the set of 336 IDs changes.
- **Anchors** (`Docs/Project/unity-project-overview.md:343,368`): Enlarged80 start `04A78188A6F8E6F9`, year 3 `081DB58CE3034648`; Legacy40 starting world `EBE7A228F630AE41`. Whether P2 `F58FB0B1A421D28B`, P3 `F7C2FC966816BBAD` and Model2 `702766DECE591E21` are regenerated or loaded from embedded saves must be confirmed. Reference Y100 `7AD177B3CC2F73C7` and continuation `9CDF21A541C5968D` are archive-based and expected to be unaffected, which the matrix already says.
- **Count vs identity.** Clearance omissions are subtracted from the mortality budget (`ForestStartingStand.cs:126-131`; the comment at `:121` says the opposite). So until clearance exceeds the budget the **count stays 336 + 1,008**, which `Enlarged80Verification.cs:151` hard-asserts, while **which ids exist changes**. Both E and workbench retirement can pass a count gate while changing every downstream identity-dependent hash, DBH/neighbour field and competition value. #285's "publish the precise accepted baseline… tree count" would miss this.

**Amendment.** In §6.6, §6.8 and AS-W: (a) name the two implementation choices — runtime geometry-conditional suppression (precedent) vs scene-root removal — and state that scene-root removal changes Legacy40 and Enlarged80-core generation and therefore the anchors above, so it needs a recorded re-baseline decision; (b) cite D-056 and list the anchors as the protected set; (c) require gates to report **id-set difference, not only count**.

### B3 (High) — The hidden-site and dependency inventory is incomplete, and D-055 is not cited

**Finding.** `ForestTest.unity` holds **seven** `ForestBuildable`s, all hidden, all clustered at the clearing (x −5.6…+2.2, z 9.0…12.8):

| Site (buildId) | Position | `requiredBuildable` | 
|---|---|---|
| Forestry Workbench (`workbench-01`) | (−2.75, 11) | none |
| Saw Pit (`saw-pit-01`) | (−4.9, 9.3) | Workbench |
| Timber Sledge (`timber-sledge-01`) | (−0.2, 9) | Workbench |
| Basic Shelter (`shelter-01`) | (−5.6, 12.2) | Workbench |
| Log Rack (`log-rack-build-01`) | (−0.3, 11.5) | Workbench |
| Log Rack II (`log-rack-build-02`) | (2.2, 12.8) | **Saw Pit** |
| Grinding Stone (`grinding-stone-01`) | (−2.5, 9.4) | **Saw Pit** |

#285 names four dependants and omits Log Rack II and Grinding Stone (transitive via Saw Pit). **D-055** (2026-10-10) is the controlling decision for these seven sites ("a site's flag is switched on only when its structure is genuinely buildable"; code/prefabs retained; `ScenarioOneInteractionVerification` keeps the latent mechanic covered) and is not cited.

Dependencies #285 does not list:

- **Fail-open prerequisite.** `IsPrerequisiteMet => requiredBuildable == null || requiredBuildable.IsBuilt` (`ForestBuildable.cs:203`). A deleted workbench leaves four sites with a null reference that reads as **prerequisite met**, not an exception. Harmless while `constructionAvailable` is false everywhere, but the failure mode is silent unlock, not a crash.
- **Protected tests that read buildables:** `ScenarioOneInteractionVerification.cs:150` (probe picks the first unbuilt site with `IsPrerequisiteMet` and `plankCost == 0`), `Enlarged80Verification.cs:151,159-160,259-265` (no tree on a "(hidden) site footprint"; every site unavailable, uncollidable), `Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs:231`, `Tools/Verification/CCFIntegrationVerificationTemp.cs:116` (inactive-inclusive buildable enumeration; not read in full). Retiring the workbench changes what these find.
- **Other runtime consumer:** `ScenarioOneManager.cs:747-775` (`MoveToReferenceView`, presentation-only) rejects viewpoints within 3 m of any buildable.
- **Tooling and scenes:** `Editor/ForestSceneBuilder.cs:99-103,319` recreates the workbench for any freshly built scene; `MixedSpeciesTest.unity` contains its own workbench; `README.md` documents the workbench.
- **After workbench-only retirement, six hidden Site circles remain on the spine terminus.** From scene coordinates (disposable arithmetic): Grinding Stone is 1.367 m from the `Dirt Path 51` end, Saw Pit 2.62 m, Log Rack 2.41 m from the northward line, Shelter 2.89 m, Timber Sledge 3.06 m. Each carries a 2.2 m Site/clearance circle, so Q2 "other sites unchanged" leaves excluded area on the very stretch the spine would use. This does not block walking (colliders are off) but it does affect Site masks and start-stand clearance, which is the footprint D-070 aims to remove.

**Correction to a #285 severity.** The "saved `workbench-01` identity" risk is **lower than "Critical"**: load silently skips unmatched ids (S14). The real risks are the fail-open reference, the protected-test dependencies and the start-baseline change. Keep a save round-trip test, but do not rank it as the lead risk.

**Amendment.** Replace the four-item list in §1a/§6.6/AS-R11 with the seven-site table; cite D-055; add the protected-test, `MoveToReferenceView`, `ForestSceneBuilder`, `MixedSpeciesTest` and fail-open items to the #284 contract; and ask Q2 as an explicit seven-site inventory choice (retire workbench only / retire the whole cluster / keep), with the six remaining Site circles' effect on the spine terminus quantified.

### B4 (High) — Option A's eligibility, selection point and lifecycle are not yet testable

**Finding.** The safety intent in §6.4 is right. These gaps would stop an implementer or reviewer writing a deterministic test:

1. **Eligibility is defined by "legacy refuses".** Legacy `Derive` is not parallel-only (S9): it accepts any five racks whose projected intervals do not interleave and gates no angular deviation. So (i) a strongly fanned layout can pass legacy and silently receive legacy strip semantics that no one designed for it, and (ii) a legacy refusal can mean several different things (count ≠ 5, non-finite, no common direction, interleave, short teaching rack). #285 says "narrowly recognised two-direction-family failure" but gives no predicate (angle separation, intra-family tolerance, minimum group size, chaining bound).
2. **Selection at read time.** `Derive` is called at read time from **three** places (S8). If A is chosen by "legacy returned invalid", moving one rack a centimetre across the validity boundary flips the whole coverage model, and unit identity and meaning change with it. §6.4(7) notes unit identity but not that selection must be **recorded** (a rule-version tag fixed at Confirm) rather than recomputed at each read.
3. **Consumers not in the ownership plan.** AS-11 does not list `UI/LearningObjectivesView.Sequence.cs`, `UI/StandMapTreatmentReview.cs` or `StandMapAreaFootprints`, whose coverage footprints #253 found single-axis dependent, nor `ScenarioOneRackBay.Frame`, which takes the teaching sample. AS-11 and AS-12 therefore overlap on files.
4. **Sample anchor under A.** The teaching sample is "the third rack in common-normal order" (`ScenarioOneRackSampleGeometry.cs:28`). "Third" has no meaning in a two-family order. The spec says the sample is a hypothesis but does not require that the anchor rule itself be re-specified.
5. **The lifecycle gate changes accepted behaviour (#253 F-6).** Adding a Confirm-time derivation gate would newly refuse plans that confirm today. §6.4(6) lists the gate but not the fixture set needed to prove old-valid plans still confirm.

**Amendment.** Add to §6.4: a formal eligibility predicate with named parameters (even if values are OPEN); an explicit statement that selection is stored at Confirm and that A and legacy never switch by re-evaluation; a ruling on legacy-valid but strongly fanned layouts (cap, or accept and document); a sample-anchor rule for A; a full consumer list for AS-11/AS-12 with a single writer per file; and a Confirm-gate fixture list (every layout that confirms today must still confirm).

### B5 (Medium-High) — E's start-state identity is required but the feasible designs are not enumerated

**Finding.** §6.2 says "do not infer new northern infrastructure from old save version, an existing plank name or the old south-approach occupancy heuristic" and asks for "an unambiguous scenario-start/infrastructure identity." The repo already has **two** working precedents, and the spec should choose between them as a decision:

- **Inferred, no save field** (`StandApproachRoute.WorldHasApproach`): reads original-plantation tree records inside the extension's 1.5 m corridor. It is keyed on `P####`/`PO####` ids that the game keeps for living, dead and stump trees, so it does not depend on the south approach. A parallel north check is feasible without a schema change. Its limit is that it infers from absence, and a corridor with no original ids by chance would read as "has north".
- **Explicit field:** unambiguous, but the project's own history says a new serialized field changes JSON world hashes even for identical biology (`unity-project-overview.md:151`, `ScenarioReferenceArchive.LegacyV15WorldHash`), so it carries a hash-compatibility burden that #285 only mentions generally.

The spec's dismissal is also slightly imprecise: the existing heuristic is not "south-approach occupancy". It is original-tree occupancy of the extension corridor.

**Amendment.** Add the two designs as options under Q6 with their costs, and list the save fixtures to be built (pre-feature Enlarged80 with trees in the north corridor; new-start-with-north; Legacy40; the unmodified Reference archive; a pre-feature save with a felled north corridor).

### B6 (Medium) — The 2.4 m clearance rule is presented as immutable; it is a decision

**Finding.** Spec §6.3 and plan §5 say not to waive 2.4 m "merely because rectangles do not intersect" and "never fake by weakening validation." Source: `OverlapClearanceM = RackGeometry.DefaultWidthM` (2.4), tested with `SegmentDistance` between centreline segments (`RackEditing.cs:102,176`), and its comment says "corridors that come closer than their own width overlap materially." That is a sound proxy for **parallel** racks. For angled racks it rejects physically disjoint rectangles: R1/R2 have a positive 0.214214 m gap. Equally, 0.214 m is not machine clearance, so the rectangles-disjoint test is not enough either.

The practical consequence for any spine design: **every opposite-side pair of branches must enter the spine at least 2.4 m apart, whatever the angle.** That binds the entrance layout, not just R1/R2.

**Amendment.** Reframe as decision Q4a: (i) keep centreline ≥ 2.4 m (conservative, forces entrance spacing), or (ii) amend D-064 editor validation to a rectangle-separation test plus a stated machine-clearance minimum. Either needs a reviewed D-064 amendment. Remove "never weaken" as a rule; keep "never silently weaken."

### B7 (Medium) — Stale and contradictory status text in the four files

**Finding** (PR head `4128757b`):

- `tasks.md` AS-06: "**RUNNING BROWSER RESEARCH**" and "**Current next action:** Finish #283 on the docs branch…" while the result file and `active-tasks.md` record #283 HANDED-OFF/CLOSED. AS-07 is still "new claim" though #286 now exists.
- `spec.md` old §6 (line ~90): "#248 real Unity geometry/mask measurement remains blocked until the user's playtest and Unity lease gates clear" while the header and the new §6 record #248 accepted. Line 8: "#246 … when returned".
- Two sections are numbered `## 6` (old "Approval / stop conditions" and the new controlling section). The result says the new §6 "controls", but a reader hitting the older text first is told contradictory things.
- `plan.md` §1 contains a malformed reference ("`spec.md` `6. Source and derived research…" with an unclosed backtick).
- Requirements: 4 of 34 items are `[x]`; AS-R05 (this review) will need updating.

**Amendment.** Mark superseded sections explicitly, renumber, and bring AS-06/AS-07 and the "next action" line in line with HANDED-OFF and #286.

---

## 7. MFM Option A audit (packet item 6) — summary

| Required property | In #285? | Testable as written? |
|---|---|---|
| Existing-valid legacy `Derive` unchanged | Yes (§6.4(1), AS-R20) | Partly: needs the selection-recorded rule (B4.2) |
| Narrow two-family eligibility | Yes, in words | **No** — no predicate (B4.1) |
| Singleton / chaining / ties | Yes (§6.4(2),(4)) | Partly: tolerances OPEN, correctly |
| Finite access, unique owner | Yes (§6.4(2),(3)); nearest-finite-segment, no infinite lines | Yes once the reach cap is chosen |
| One sample for both families **unqualified** | Yes, as hypothesis (§6.4(4)) | Yes; add the 150.6 m² contamination figure |
| Confirm / edit / work fail-closed | Yes (§6.4(6)) | Needs the "every layout that confirms today still confirms" fixtures (B4.5) |
| Previously saved invalid confirmed plans load without destructive rewrite | Yes | Yes; matches today's behaviour (no load-time derivation check) |
| Rule-version / unit identity / hash history | Yes (§6.4(7)) | Yes; add that selection is recorded (B4.2) |

**Internal inconsistency found:** §6.4(1) forbids routing "malformed inputs, third-family plans, missing IDs or invalid count" to A, but defines eligibility only by what legacy refuses; those are all legacy refusals. The predicate in B4.1 resolves it.

---

## 8. Vocabulary, test matrix, ownership (packet item 7), and further user/product choices

**Vocabulary.** Existing / Proposed / Confirmed / Marked / Approved / Executed is coherent and consistent across 3D, Stand Map, minimap, Work Plan and Objective guidance. The rule that Proposed never masks or marks, and that an orange entrance mark does not execute work, matches D-064/D-065. Source-to-presentation duplication is acknowledged. **PASS.** Two additions:

- The Stand Map orders planks by **ordinal name** and splits lines on a >2 m jump (S5). Any north-continuation plank names must sort between `Dirt Path 51` and `Dirt Path X…` (so `N…` sorts, but a name starting `Dirt Path X…` for a north plank would sort after the south approach and break the line). State this naming constraint or replace name-scraping with the single route definition #285 already wants.
- `ScenarioOneSuggestedRackLayout.cs` reads the headland constants (S6) and will need a defined behaviour under E.

**Test matrix (§6.8).** Coverage is good. Missing rows: the anchors and id-set difference (B2), the protected buildable-reading gates (B3), a Confirm-gate regression set (B4.5), and the save fixtures in B5.

**Further user/product choices needed** (none is made by this review):

| # | Choice | Why now |
|---|---|---|
| D1 | E / P / F *and* rack topology (headland-parallel / spine single-family / two-family A) | B1 |
| D2 | Workbench mechanism: runtime geometry-conditional vs scene edit, and whether Legacy40 start anchors may move | B2 |
| D3 | The other six hidden sites: retire the cluster, keep, or workbench only | B3 |
| D4 | Start-identity design for E: inferred vs explicit field | B5 |
| D4a | 2.4 m centreline rule: keep, or amend to rectangle separation plus machine-clearance minimum | B6 |
| D5 | Reach cap and the minimum acceptable Manual/NotCovered share before the MFM lesson loses meaning | B1, B4 |
| D6 | Whether any layout that confirms today may stop confirming (Confirm-time derivation gate) | B4.5 |

---

## 9. Remains runtime-unproven (not claimed by #285, restated for the record)

Everything requiring Unity or a human: a cleared, traversable northern corridor on real terrain; machine and forwarder turning; legal spine entrance trees and reachable spray interaction; five-rack field marking; new-start tree population, ids, masks and WorldHash under E and under workbench retirement; Stand Map / minimap / 3D parity at 720p and 1080p; save/load of every fixture in B5; the C# Option A (only a disposable Python port exists, #253); calibration on real stems and masks; completion-gate and Tier-3 results on any new baseline; the PR #274 comparison values. The external forestry sources cited in spec header line 8 (Pro Silva Ireland, Teagasc) were **not** re-fetched in this review; they support "planned permanent access" in general only and the spec already labels the layout as game design.

---

## 10. Acceptance-gate judgment

**Research quality: NEEDS CHANGES.** The route to APPROVED research quality is the seven amendments above, all doc-only: B1 (add MFM cost row, split axes), B2 (mechanism, D-056, anchors, id-set gate), B3 (seven-site inventory, D-055, protected tests, fail-open), B4 (eligibility predicate, recorded selection, consumer list, Confirm fixtures), B5 (two start-identity designs), B6 (clearance rule as a decision), B7 (status cleanup). I would expect a re-review limited to those sections, without re-verifying the unchanged source map.

No Unity is needed to close any blocker. **This review does not set the spec to APPROVED, does not approve E, P, F or Option A, does not approve #284 or any code, and does not say #283 is Unity-qualified.** Draft PR #285 remains unmerged; whether to merge it as a documentation record, request amendments first, or hold, is for Manager #200.

---

## 11. Provenance and limits

- Source reads at `main` `50042bbe`; PR files at `4128757b`; scene parsed from `Assets/Scenes/ForestTest.unity` (plank, clearing and buildable transforms and `requiredBuildable` fileIDs) with throwaway Python; route formula replayed from `StandApproachRoute.Build` (S4). All scratch files stayed outside CCF worktrees; nothing was committed except this file.
- `Tools/` was grepped only for `ForestBuildable` / workbench names; I did not read those harnesses in full and did not run any. `MixedSpeciesTest.unity` was only grepped for the workbench name.
- GitHub code search returned no results for the workbench terms, so the consumer list in B3 comes from the sparse clone and may miss files outside `Assets/`, `Tools/`, `Docs/` and `specs/`.
- I did not check `Docs/Handoffs` ledgers or the T3 handoff beyond what is cited.
- #246/#251 same-family disclosure: see header.

*Sources:* [spec.md](https://github.com/jermur788/CCF/blob/4128757b9188cb3325ed87ef3b94b35d02044949/specs/scenario-one-access-spine-route-consistency-v1/spec.md) · [plan.md](https://github.com/jermur788/CCF/blob/4128757b9188cb3325ed87ef3b94b35d02044949/specs/scenario-one-access-spine-route-consistency-v1/plan.md) · [tasks.md](https://github.com/jermur788/CCF/blob/4128757b9188cb3325ed87ef3b94b35d02044949/specs/scenario-one-access-spine-route-consistency-v1/tasks.md) · [requirements.md](https://github.com/jermur788/CCF/blob/4128757b9188cb3325ed87ef3b94b35d02044949/specs/scenario-one-access-spine-route-consistency-v1/checklists/requirements.md) · [decision log](https://github.com/jermur788/CCF/blob/50042bbe97734b031210ddebb603b5ec00ff459d/Docs/Project/decision-log.md) (D-042, D-055, D-056, D-064, D-065, D-067–D-070) · [#248 Manager](https://github.com/jermur788/CCF/blob/50042bbe97734b031210ddebb603b5ec00ff459d/Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-MEASUREMENT-REVIEW-20261103.md) · [#250 Manager](https://github.com/jermur788/CCF/blob/50042bbe97734b031210ddebb603b5ec00ff459d/Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-FIVE-RACK-REVIEW-20261103.md) · [#251 Manager](https://github.com/jermur788/CCF/blob/50042bbe97734b031210ddebb603b5ec00ff459d/Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-MFM-TOPOLOGY-REVIEW-20261104.md) · [#253 handoff](https://github.com/jermur788/CCF/blob/50042bbe97734b031210ddebb603b5ec00ff459d/Docs/Handoffs/ScenarioOne-Access-Spine-20261103/OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md) · [PR #274 handoff](https://github.com/jermur788/CCF/blob/4d0abd1b3bc2c69f6d1d3c836951a04e9bf0c9d9/Docs/Handoffs/Overnight-Continuation-20261104/MFM-A-VS-C-COMPARISON-HANDOFF.md).
