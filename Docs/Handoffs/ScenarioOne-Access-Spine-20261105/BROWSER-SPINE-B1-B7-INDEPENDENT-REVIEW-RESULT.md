# #288 — Independent B1–B7 re-review of access-spine DRAFT corrections (draft PR #285)

**Date:** 2026-11-05. **Status:** **[HANDED-OFF] — REVIEW ONLY.** Documentation/research-quality review. No spec approval, no E/P/F, T-F/T-C/T-A, Option A, clearance, save-identity or #284 integration approval. No Unity, code, scene, save, build or merge.
**Claim:** [#288](https://github.com/jermur788/CCF/issues/288). **Reviewed:** [draft PR #285](https://github.com/jermur788/CCF/pull/285), branch `docs/access-spine-start-condition-20261105-v1`, head `4f8dd10ef6daa460105d3e079d16b5f4f288c1d0` (OPEN / draft / unmerged; not modified by this review).
**Reviewer / provider:** Claude Sonnet 5.5 Medium (`claude-sonnet-5-5`). Quota/capacity not observable; no switch.
**Original findings:** [#286 NEEDS CHANGES](https://github.com/jermur788/CCF/blob/docs/access-spine-independent-review-20261105-v1/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-INDEPENDENT-REVIEW-RESULT.md). **Author correction:** [BROWSER-SPINE-REVIEW-CORRECTION-RESULT.md](https://github.com/jermur788/CCF/blob/docs/access-spine-start-condition-20261105-v1/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/BROWSER-SPINE-REVIEW-CORRECTION-RESULT.md).

## Independence disclosure

Claude Sonnet 5.5 Medium (the same model family as this reviewer) is recorded as author of #246 and #251, of the #286 review being re-checked here, and of the source review that approved #284. This session had no access to those sessions. Nothing below rests on their conclusions: each claim was re-checked against `main` file bytes, the PR #285 files, the #284 branch handoff, the Decision Log, or the published #253/PR #274 records. Manager should still weigh same-family risk, particularly for B2 (where the same family reviewed #284) and B1 numerics (same family wrote #251).

## Method and what was actually read

| Class | Items |
|---|---|
| **READ (bytes)** | PR #285's four spec files and both handoffs at `4f8dd10e`; #286 result; packet; Manager disposition; `Docs/Project/decision-log.md` D-055, D-056, D-070 on `main`; #284 branch implementation handoff; source on `main`: `ForestBuildable.cs`, `ForestStartingStand.cs`, `ForestSaveController.cs`, `StandApproachRoute.cs`, `RackEditing.cs`, `ScenarioOneRackSampleGeometry.cs`, `ScenarioOneManager.PrescriptionApproval.cs`, `LearningObjectivesView.Sequence.cs`, `StandMapTreatmentReview.cs`; `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md` (#253); PR #274 comparison handoff (head `4d0abd1b`). |
| **COMPUTED here** | `ForestTest.unity` on `main` parsed with throwaway stdlib Python (outside any repo): seven `ForestBuildable`s, positions, `requiredBuildable` links, no `retiresFromStartingWorld` on `main`. Link-resolution and backtick-parity scripts over the PR files. |
| **CITED only (not re-derived)** | All #248/#250/#251/#253/#271 numbers (checked for faithful transcription against #253 and PR #274 text, **not remeasured**); #284's offline checker output (5 slots, 0.0969 m², zero changed slots) is cited from its handoff, not re-run. |
| **NOT done** | Unity, any harness, any save; `Tools/Verification/CCFIntegrationVerificationTemp.cs` and `MfmCoverageWorldUnityGate.cs` read only as far as named by the spec; `MixedSpeciesTest.unity` not opened; I could not locate a root `README.md` documenting the workbench (the spec/original review name "README.md" without a path), so that reader is unconfirmed. |

---

## Verdict (research quality only): **APPROVED — with six non-blocking editorial corrections (C1–C6)**

All seven original blockers are closed on substance. Remaining items are wording, one internal inconsistency in §6.3 vs §6.4, and Markdown/link hygiene; none changes a number, a decision authority or a safety claim, and none needs Unity. I would apply C1–C6 before Manager merges PR #285 as a record, but I do not require another full review for them. **APPROVED means the DRAFT is honest and decision-ready as research. It does not approve the spec, E/P/F, T-A, Option A, any clearance change, #284 production integration, old-save migration or a build.**

## B1–B7 matrix

| # | Blocker | Result | Basis |
|---|---|---|---|
| B1 | Two axes + MFM cost | **PASS WITH CORRECTION** (C1) | §6.2/§6.7a/plan §1 separate feeder (E/P/F) from topology (T-F/T-C/T-A). Numbers verified below. |
| B2 | #284 source vs hypothetical deletion; D-056 | **PASS WITH CORRECTION** (C2) | §6.6 matches the #284 handoff; D-056 identities and hashes correct. |
| B3 | D-055 seven-site inventory | **PASS** | Independently reproduced from the scene. |
| B4 | Testable conditional A | **PASS WITH CORRECTION** (C3) | Predicate variables, recorded identity, anchor and fixtures present; one inconsistency and two oracle-less fixtures. |
| B5 | I-N vs I-V | **PASS** | |
| B6 | 2.4 m rule is a decision | **PASS** | |
| B7 | Status/history/links | **PASS WITH CORRECTION** (C4–C6) | Status text now consistent; malformed backticks and three PR-local dangling links remain. |

---

## B1 — two axes and evidence: PASS WITH CORRECTION

**Axes.** `spec.md` §6.2 ("E is the *starting feeder condition*, not an automatic two-family herringbone decision"), §6.7 Q1 ("independently of rack arrangement") and Q4, §6.7a, `plan.md` §1 table, and AS-R08–11 all separate E/P/F from T-F/T-C/T-A. The recommendation is now explicitly conditional on selected topology and an accepted Manual/NotCovered share, and Q5 lists that share as a product choice. **Closed.** C-S45 is described as experimental, needing a spine-entrance rule change and conflicting with 16–20 m spacing (§6.2, §6.7a, plan). **Closed.**

**Numeric transcription, each re-checked against the published text:**

| Spec figure | Source | Result |
|---|---|---|
| F 0.00% beyond 20 m / 3.11% beyond 9 m | PR #274 V-3 and T1 (lines 23, 82–83) | match |
| A-nearest 14.13→38.22%; A-lateral 36.25→49.03% (20→9 m) | #253 F-2 (line 30); PR #274 V-2/T1 | match |
| Denominator 5,875.75 m² non-corridor lattice area | #253 line 128; PR #274 line 202 | match (stated in §6.4) |
| 69.17 m lateral max, 41.18 m nearest max | #253 F-1 / reach table | match |
| 150.6 m² sample in other units, 143.0 m² in R4C | #253 F-3 (line 31) | match |
| 14/14 F vs at most 9/14 A units at 20 m; "ceiling" | PR #274 T2 (cap 20 m rows) | match; A-nearest and A-lateral are both 9/14 at 20 m, 6/14 at 9 m |
| C-S45 3.06% beyond 20 m, 13.75 m spacing | PR #274 V-4 | match |

**No "E automatically costs this coverage" inference remains**; §6.7a states the costs "attach to two-family T-A … do not prove any E road automatically loses MFM coverage." **Closed.**

**C1 (qualifiers a decision-maker needs; non-blocking).**
- (a) PR #274 states "Non-corridor ground differs by layout (A 5,875.75 m²)" (line 76). §6.2's table and §6.7a put F's 0.00→3.11% beside A's figures without saying the denominators differ and that the F percentages are of a different (legacy-layout) non-corridor area. Add: "Denominators differ by layout; shares are of each layout's own unmasked non-corridor lattice area (A: 5,875.75 m²)."
- (b) §6.7a says F is "14/14" as a ceiling but omits PR #274's own calibration point: the **recorded C# run on the legacy layout is 12/14**, because competition fails two units (V-7, line 27/133). Add that the F-versus-A unit gap is therefore a ceiling comparison in which the F ceiling is already 2 units optimistic.
- (c) Accepting a Manual/NotCovered share is an explicit user choice: Q5 and AS-R11 say so. **PASS**, no change.

## B2 — #284 source versus hypothetical deletion: PASS WITH CORRECTION

Checked against the #284 branch handoff (implementation commit `57f10ff0`):

- Retained scene instance, ID `workbench-01`, and **four** direct `requiredBuildable` references: handoff §2/§4/§6.3 and §6.1 (`prerequisite_refs=4`). Spec §1a/§6.6/tasks AS-W match. A single `retiresFromStartingWorld` flag on the workbench instance; scene diff is one line. Spec correctly says this is "runtime saved-state scoped, not strictly a geometry-only Enlarged80 scene deletion" and that the root-deletion alternative is "unapproved."
- Suppresses new-world Site/clearance/save registration: handoff §3/§4 (`CollectClearancePoints`, `ReadCoverageWorld`, `CaptureData`) and spec §6.6 and plan §4 match. I confirmed on `main` that all three enumerations use `FindObjectsInactive.Include` (`ForestStartingStand.cs:339`, `PrescriptionApproval.cs:348`, `ForestSaveController.cs:106,332`) and that the scene on `main` carries no retirement flag, so the draft correctly says "existing `main` scene/source still has the old Site footprint."
- D-056 cited; "336 central IDs and exact positions, 1,008 outer IDs, 1,344 total" match D-056 text; ID-set/position/DBH/neighbour (not count) required in §6.2, §6.6, AS-R21, plan §4. **Closed.**
- Protected hashes transcribed correctly against the D-056 text and #284 handoff: Enlarged80 `04A78188A6F8E6F9` / `081DB58CE3034648`; Reference Future Y100 `7AD177B3CC2F73C7`; new world hash explicitly "expected to change from an omitted `buildables` entry." Legacy40 `EBE7A228F630AE41`, continuation `9CDF21A541C5968D` and P2/P3/Model2 are cited from the #286 review and the Manager record; I did not re-find them in a primary file. P2/P3/Model2 are correctly flagged for loaded-versus-regenerated provenance.
- The "0 changed slots" claim is labelled offline-predicted and Unity-unrun (§6.6 "Real Unity ID-set … checks remain mandatory"; tasks AS-W; AS-R21). The handoff's own numbers are consistent with that labelling (5 workbench slots, all inside `Forest Clearing`; 0.0969 m² crescent; 0 stems). No implied #284 integration: "NOT MERGED / UNITY NOT RUN," #287 WAITING. **Closed.**

**C2 (#284 facts the draft omits; non-blocking).** The #284 handoff itself records items §6.6/AS-R18 should carry so #287's gate is not under-specified:
- an **empty or pre-`buildables` save** is treated as retired (handoff §5 last row), which is a behaviour difference from "old saves restore the workbench";
- the four in-repo test gaps named by the #284 source review (new game omits `workbench-01`; old save keeps it; Reference preview enter→exit ends retired; new-game reload stays retired);
- `Enlarged80Verification.cs:159/260` still model the workbench exclusion, which is a superset and passes but no longer mirrors production;
- the MFM-08A gate now shares the production predicate and needs an explicit expected-site assertion.
Add one sentence or checklist line for each under AS-R18.

## B3 — D-055 seven-site inventory: PASS

Independently parsed `ForestTest.unity` on `main`. Result (all `m_IsActive: 1`, none retired):

| buildId | x, z | requiredBuildable |
|---|---|---|
| workbench-01 | −2.75, 11 | none |
| saw-pit-01 | −4.9, 9.3 | workbench-01 |
| timber-sledge-01 | −0.2, 9 | workbench-01 |
| shelter-01 | −5.6, 12.2 | workbench-01 |
| log-rack-build-01 | −0.3, 11.5 | workbench-01 |
| log-rack-build-02 | 2.2, 12.8 | saw-pit-01 |
| grinding-stone-01 | −2.5, 9.4 | saw-pit-01 |

This is the exact §6.6 table; four direct plus two indirect (via Saw Pit) is correct. `IsPrerequisiteMet => requiredBuildable == null || requiredBuildable.IsBuilt` (`ForestBuildable.cs:203`) confirms fail-open. Six other Site masks remain after #284 and Q2 states retirement is a separate decision. D-055 is cited and its "latent mechanic" guard (`ScenarioOneInteractionVerification`) named. Named readers in §6.6/plan §4/AS-R20: `ScenarioOneInteractionVerification`, `Enlarged80Verification`, `MfmCoverageWorldUnityGate`, `CCFIntegrationVerificationTemp`, `MoveToReferenceView`, `ForestSceneBuilder`, `MixedSpeciesTest.unity`, README: all present (I confirmed by path that the first seven exist on `main`; `ForestSceneBuilder.cs:99-101` creates the workbench; README unconfirmed, see Method). The saved-`workbench-01` risk is no longer "Critical": §6.6 "Risk prioritisation" says it is a compatibility/round-trip risk because `LoadData` skips unmatched ids, which agrees with `ForestSaveController.cs:464-473`. **Closed.**

## B4 — testable conditional A: PASS WITH CORRECTION

Verified present in §6.4, plan §3–4, AS-R26–32: legacy `Derive` is not a parallel-only filter (matches `ScenarioOneRackSampleGeometry.cs:182` exactly-five and `:244-246` interleave-only refusal); named variables `A_intra`, `A_inter`, `A_chain`, `N_group` with finite-input validation, quantisation/tie rule and singleton/third-family handling; numeric values explicitly OPEN; the legacy-valid-but-fanned ruling explicitly a Manager/D-064 decision; **Confirm-time stored model/rule identity** or "independently proven equivalent," with the 1 cm read-time switch forbidden; a separate A-specific sample anchor by stable rack identity (the legacy `TeachingRackIndex = 2` "third rack" is correctly called undefined for two families); old Confirm-positive (including MFM-invalid) fixtures protected; invalid historical confirmed plans load without destructive rewrite and suspend only new dependent work; executed locks preserved. The three read-time call sites named in §6.4 are real (`PrescriptionApproval.cs:261`, `LearningObjectivesView.Sequence.cs:69`, `StandMapTreatmentReview.cs:145`), and AS-11/AS-12 now list the UI consumers, `ScenarioOneRackBay.Frame`, `StandMapAreaFootprints`, the map source and minimap, with a one-writer rule. **Closed.**

**C3 (internal inconsistency and fixtures with no oracle; non-blocking).**
- **Confirm-gate wording conflict.** §6.3 (last bullet but one) says Confirm and confirmed edits "must fail closed when either the selected legacy derivation or … branch-local derivation is invalid," unscoped. §6.4 and AS-R28/R31 say a universal gate would change accepted Rack V2 behaviour and must be limited to the "new governed topology." Source confirms today's Confirm does not call `Derive` (only the three read-time sites above). Reword §6.3 to: "…for the new governed topology only; plans that confirm today continue to confirm unless a D-064 amendment says otherwise."
- **Fixtures that cannot yet be written.** (i) "strongly fanned legacy-valid input" has no expected result until the OPEN legacy-valid-fanned ruling is made; (ii) "1 cm boundary flip" has no pass condition unless it states that stored selection must be unchanged after the move. Mark both "oracle pending Q5" and state (ii)'s expected result.
- The A **reach/sample** fixtures are well-formed but depend on OPEN caps and a calibration source; §6.4 and AS-R29/R30 already say so. No change.

## B5 — E save identity: PASS

§6.8 compares I-N and I-V with failure modes. I read `StandApproachRoute.WorldHasApproach` (`:77-95`): it scans original-plantation tree records (living, dead or stump; `IsOriginalPlantationId` accepts `P####`/`PO####`) within the 1.5 m corridor of the extension and returns "has approach" when none is found. The spec's description is accurate, including that it is "original-tree occupancy," not south-approach art or a plank name, and that its failure mode is a false positive when a corridor was cleared or naturally empty. I-V correctly carries the schema/JSON-WorldHash cost. Fixture lists (old Enlarged80 with north trees, fully felled north, naturally empty, new E, Legacy40, Reference Future) match the #286 requirement. The spec correctly treats the south heuristic as precedent, not proof (§6.2, plan §1). Q6 remains OPEN with no selection. **Closed.**

## B6 — clearance rule: PASS

§6.3 and Q4a: the 2.4 m centreline rule is "a Rack V2 gameplay rule, not immutable engineering authority," with two options (retain, or a reviewed D-064 amendment to rectangle separation plus a specified machine-clearance minimum), "including opposite-side entrances," "never silently weaken," and the 0.214214 m R1/R2 physical gap "is not itself adequate turning/access proof." Source: `RackEditing.cs:102` (`OverlapClearanceM = RackGeometry.DefaultWidthM`) and `:176` (`SegmentDistance` between centreline segments, `< OverlapClearanceM`). Terminology (physical rectangle gap versus centreline distance) is correct everywhere I checked, including AS-R04/R24. Minor optional hardening: say "nonzero" in the machine-clearance minimum. **Closed.**

## B7 — history, status and references: PASS WITH CORRECTION

Verified:
- Four files stay DRAFT/STOP. One `## 6` heading remains (line 95); old text is now `## 5a … superseded`; `6.1`–`6.9` unique.
- Live states agree with the text: #283, #284 and #286 closed; #287 open WAITING; #285 open draft; #288 open. `tasks.md` has #283 HANDED-OFF/CLOSED, #286 NEEDS CHANGES, #284 source-approved/Unity-pending, #287 WAITING; the stale "#283 RUNNING" and "#248 blocked" text is gone. `plan.md` §1 malformed backtick line is gone (all four spec files have even backtick parity).
- The original #283 handoff remains historical and the correction handoff supersedes it (correction §1–2, tasks AS-06/AS-07R).

**Defects found:**
- **C4 — malformed Markdown in the correction handoff.** `BROWSER-SPINE-REVIEW-CORRECTION-RESULT.md` lines 20, 22 and 26 (the B1, B3 and B7 matrix rows) have odd backtick counts (7, 5, 9): fragments such as `` `spec.md` `6.2 and `6.7/6.7a; `plan.md` `1; `` are unclosed code spans. The packet explicitly asks for no malformed Markdown. Rewrite as `` `spec.md` §6.2, §6.7 and §6.7a; `plan.md` §1 `` etc.
- **C5 — links that resolve only after merge.** `spec.md` and `tasks.md` link to `Docs/Handoffs/ScenarioOne-Access-Spine-20261105/MANAGER-WORKBENCH-RETIREMENT-REVIEW-20261105.md` and `T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-PACKET.md`. Both exist on `main` but not on the PR branch (the branch is diverged, 15 ahead / 10 behind `main`). All other relative links (the 20261103 Manager/packet files and the correction handoff's links to the four spec files) resolve on both. Either rebase/merge `main` into the PR branch or use absolute `main` URLs.
- **C6 — one stale clause.** `spec.md` §5 says the corrected #248 report "resides in unmerged docs-only PR #249 at the review point." PR #249 is now closed and merged. The "at the review point" qualifier makes it historically true but easy to misread; change to "merged PR #249."

---

## Required text corrections (summary)

| ID | File | Change |
|---|---|---|
| C1 | `spec.md` §6.2 table, §6.7a | State differing denominators; add that the legacy C# run is 12/14 versus a 14/14 ceiling (PR #274 V-7). |
| C2 | `spec.md` §6.6, `requirements.md` AS-R18 | Add the empty/pre-`buildables` save retirement case; the four Phase-3 test gaps; the older verifiers' superset model; the MFM-08A expected-site assertion. |
| C3 | `spec.md` §6.3, §6.4/§6.9 | Scope the fail-closed Confirm statement to the new governed topology; mark the fanned-legacy-valid and 1 cm fixtures "oracle pending Q5" with the 1 cm expected outcome (selection unchanged). |
| C4 | `BROWSER-SPINE-REVIEW-CORRECTION-RESULT.md` lines 20, 22, 26 | Close the code spans. |
| C5 | `spec.md`, `tasks.md` | Fix the three PR-local dangling links (rebase or absolute URLs). |
| C6 | `spec.md` §5 | "PR #249" is merged. |

## Still unproven (unchanged by this review)

Everything needing Unity or a human: #284 runtime behaviour and the new labelled Enlarged80 hashes (#287); exact 336/1,008 ID-set equality; a physically traversable northern corridor; machine turning; legal spine entrances; any C# Option A coverage or sample calibration; real Covered/Manual share on a mask-adjusted world; old-save, Legacy40 and Reference round-trips; 720p/1080p map/minimap/3D parity; any Windows build. The external forestry sources in spec header line 8 were not re-fetched.

## Limits of this review

Cited published figures were checked for faithful transcription, not remeasured. #284's offline numbers were read from its handoff, not rerun. GitHub code search was unavailable, so the call-site and consumer lists come from file reads on `main` and may miss files outside those read. No file other than this one was written; PR #285 and `main` were not touched.

*Sources:* [spec.md](https://github.com/jermur788/CCF/blob/4f8dd10ef6daa460105d3e079d16b5f4f288c1d0/specs/scenario-one-access-spine-route-consistency-v1/spec.md) · [plan.md](https://github.com/jermur788/CCF/blob/4f8dd10ef6daa460105d3e079d16b5f4f288c1d0/specs/scenario-one-access-spine-route-consistency-v1/plan.md) · [tasks.md](https://github.com/jermur788/CCF/blob/4f8dd10ef6daa460105d3e079d16b5f4f288c1d0/specs/scenario-one-access-spine-route-consistency-v1/tasks.md) · [requirements.md](https://github.com/jermur788/CCF/blob/4f8dd10ef6daa460105d3e079d16b5f4f288c1d0/specs/scenario-one-access-spine-route-consistency-v1/checklists/requirements.md) · [#284 implementation handoff](https://github.com/jermur788/CCF/blob/feature/scenario-one-workbench-retirement-20261105/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-RETIREMENT-IMPLEMENTATION-HANDOFF.md) · [decision log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md) · [#253](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Access-Spine-20261103/OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md) · [PR #274 handoff](https://github.com/jermur788/CCF/blob/4d0abd1b3bc2c69f6d1d3c836951a04e9bf0c9d9/Docs/Handoffs/Overnight-Continuation-20261104/MFM-A-VS-C-COMPARISON-HANDOFF.md)
