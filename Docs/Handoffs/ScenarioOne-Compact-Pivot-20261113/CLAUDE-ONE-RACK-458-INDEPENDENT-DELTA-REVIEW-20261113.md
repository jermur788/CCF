# #459 — Independent 20-finding DELTA review of the #458 compact one-rack DRAFT repair

**13 November 2026 | Reviewer: Anthropic Claude Opus 5.5 (`claude-opus-5-5`, Claude Code in T3 Code, HP host) | Claim: [#459](https://github.com/jermur788/CCF/issues/459)**
**Not the author.** #454 and #458 were written by OpenAI GPT-6.
**Reviewed candidate:** `docs/compact-one-rack-457-review-repair-20261113-v1` @ `94278186740c7f32c08f3b06deb00ea0fa7896bf`, compared with the reviewed #454 author baseline `0d857afa58bfe120de6460cbcd18097861437684`.
**Findings under test:** [#457 independent review](https://github.com/jermur788/CCF/blob/docs/compact-one-rack-independent-review-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/CLAUDE-ONE-RACK-454-INDEPENDENT-DRAFT-REVIEW-20261113.md) B1–B3, M1–M7, m1–m10, as bound by the [Manager #425 repair packet](https://github.com/jermur788/CCF/blob/docs/manager-425-457-one-rack-draft-disposition-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/MANAGER-425-457-NEEDS-CHANGES-DRAFT-REPAIR-PACKET-20261113.md).
**Repair handoff reviewed:** [#458 handoff](https://github.com/jermur788/CCF/blob/docs/compact-one-rack-457-review-repair-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/ONE-RACK-457-NEEDS-CHANGES-DRAFT-REPAIR-HANDOFF-20261113.md).
**Reviewer branch:** `docs/compact-one-rack-458-independent-delta-review-20261113-v1`, starting HEAD `94278186740c7f32c08f3b06deb00ea0fa7896bf`. Worktree: NONE.

## Verdict

# APPROVED FOR NEXT NATIVE FEASIBILITY STUDY — DOCUMENTATION DELTA QUALITY ONLY

This verdict covers **only** whether the #458 documents fix the #457 findings well enough for Manager #425 to commission the separate native/offline C0-03/C0-04 study. It is **not** spec APPROVED status, not user or save-owner approval, not implementation authority, not Unity verification and not acceptance. No checklist box was checked. No OPEN-C1–C8 or OPEN-1–OPEN-6 item was decided.

**Tally:** 18 RESOLVED, 2 PARTIAL (M5, M7), 0 UNRESOLVED.

Neither PARTIAL item blocks the feasibility study:
- **M5** is a naming defect in the future Work Plan display (Stage C). The study does not depend on it.
- **M7**'s gap is the study's own lane type (Unity Editor or plain managed code) and lease check. The Manager's study packet must state these anyway, so it can close the gap there (see E-1).

The remaining edits in "Edits still required" can wait for the amended DRAFT that the plan already schedules after the study.

## Change accounting (verified)

- `git merge-base --is-ancestor 0d857afa 94278186` passes. The repair branch is 8 commits ahead and 0 behind.
- `git diff --name-status 0d857afa 94278186` gives exactly five paths:
  - `A` `Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/ONE-RACK-457-NEEDS-CHANGES-DRAFT-REPAIR-HANDOFF-20261113.md`
  - `M` `specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md`
  - `M` `specs/scenario-one-compact-complete-loop-v1/plan.md`
  - `M` `specs/scenario-one-compact-complete-loop-v1/spec.md`
  - `M` `specs/scenario-one-compact-complete-loop-v1/tasks.md`
- Checklist: **104** `- [ ] R-nnn` lines, 104 distinct IDs (each appears once), R-001–R-104 contiguous, **zero** `[x]`. All original R-001–R-082 IDs are still present.
- All four DRAFT files still carry DRAFT / NOT APPROVED / NOT AUTHORISED status lines.
- `main` is still `6c13a9e182774945bf391bc8d4845ce74d3e5e88`, the same base #457 audited.

## Source facts independently re-checked (read-only, `main` @ `6c13a9e1`)

The #458 handoff honestly says its source facts were inherited from #457. This reviewer re-checked the ones the repair relies on:

| Claim in repaired DRAFT | Verified at |
| --- | --- |
| Future-version legacy save loads "best-effort" after the reject check and before the first world mutation | `ForestSaveController.cs:346-347`; first mutation (`ecology.StandApproachExpected`, `ApplyStandGeometry`) follows at :355-356 |
| `CanLoad`, `TryReadSave`, `FindLoadProblem` are the pre-load checks | `ForestSaveController.cs:240, 261, 296` |
| Scenario-ID mismatch only warns | `ScenarioOne/ScenarioOneManager.cs:1481-1482` |
| `StandSizeMeters` maps every non-Enlarged80 value to 40 m; `IsKnown` accepts only two IDs; `ForSave` maps pre-v20 to Legacy40 | `StandGeometryModel.cs:32, 34, 60` |
| Global PlayerPrefs prefixes | `UI/LearningObjectivesView.Curriculum.cs:9` `CCF.Learning.v1.`; `UI/MenuHelpView.cs:11` `CCF.MenuHelp.v1.` |
| €2,500 is a minimum **per contractor visit** | `ScenarioOneDefinition.cs:39` `minimumHarvestJobCents = 250000`; `UI/WorkPlanOverview.cs:129` "minimum charge per contractor visit" |
| Old two-cycle predicate | `ScenarioOne/ScenarioOneCompletionRules.cs:36` |
| Reference identity strips named fields | `ScenarioOne/ScenarioReferenceArchive.cs:248-272` |
| Marks are None/Fell/CropTree only | `ForestTree.cs:18-23` |
| Marks join the Work Plan when it opens | `UI/LearningObjectivesView.Curriculum.cs:37`, `UI/WorkPlanView.cs:50` |
| Fixture directories exist | `Tools/Verification/MfmSaveProvenance` (7 files), `S1CObjectiveHistory` (5), `TeachingPersistence` (5), `RackV2Save` (1) |

**Cross-spec citations checked:**
- **#436** @ `50496929` (status DRAFT — NOT APPROVED) contains SI-02 (O1 stays at `forest-save.json`), SI-03 (`forest-save-scenario-two.json`), SI-05 (legacy classification; "a foreign/explicit conflicting ID always rejects"), FR-02 ("reject unknown/newer versions unless separately supported") and FR-06 (backup recovery only after an independent preflight and an explicit user choice).
- **D-074 DRAFT** @ `61f6fb09` contains FR-02, FR-04 (Option A three elapsed years vs Option B), FR-06, MD-02 and MD-03.
- **D-073 feedback V2** reads "APPROVED — BOUNDED D-073 ANNUAL REVIEW V2".
- **Cadence spec** @ `43e00a6b` reads "APPROVED DESIGN / RELEASEABLE FOR BOUNDED IMPLEMENTATION; NOT IMPLEMENTED / VERIFIED". Line 22 says "stage 7 is the conservative first-release boundary". Line 31 counts rack-access felling "solely as a calendar/navigation anchor".
- **Decision Log D-077** on `main` contains P-1 (two-cycle predicate unchanged) and P-2 (any successful `FellTree` year, including rack access, is a navigation-only anchor).

**No factual claim in the repaired DRAFT was found to be wrong.**

## 20-row finding matrix

Ratings: **RESOLVED** = every element #457 and the Manager packet asked for is present and consistent. **PARTIAL** = the substance is present, but a named element is missing or a new inconsistency was introduced. **UNRESOLVED** = not addressed.

| # | Finding | Rating | Exact repaired anchors | Fixed in document | Still open for authority (correctly not decided) |
| --- | --- | --- | --- | --- | --- |
| 1 | **B1** pre-mutation guard vs legacy best-effort | **RESOLVED** (clarification E-2 recommended) | spec §2 row `ForestSaveController…`; S-01, S-02, S-22; plan Stage A; tasks A-01; R-013–R-018, R-084–R-086 | Both source facts recorded. One shared **pre-load** check covers direct `LoadData`, Start/Continue, F9/F10, reference preview entry/exit and every backup route. It runs before any scene/ecology/tree/cash/order/allocator/PlayerPrefs/file change. `JsonUtility` default IDs are not proof. A missing or foreign compact ID is rejected **even with valid geometry** (R-014, R-018, R-086). Continue is checked per profile and again at click time (R-013). Legacy future-version best-effort stays as shipped (S-02, R-018, R-084). World, PlayerPrefs and disk checksums must be unchanged. | Compact version policy, exact IDs and the legacy exception all sit under the save owner (OPEN-C4). |
| 2 | **B2** competing #436 router | **RESOLVED** | spec §2 "Intersecting specifications", S-04, S-21.4, OPEN-C4; plan "Read-only design intersections"; tasks A-01, C0-02, "Binding sequencing"; R-083, R-080 | #436 cited by branch, with SI-02/03/05 and FR-02/06. Its five-rack first-demo text is flagged as superseded **for new compact only**. Exactly **one** APPROVED router must exist: (a) amend #436 to add a third profile, or (b) compact owns O1 and #436 is formally amended. "No parallel A-01 source writers." O2 independence and the six-species ambition are kept. S-04 explicitly avoids claiming #436's stricter policy has shipped. | Who owns the router: Manager/save owner. |
| 3 | **B3** device-wide learning/help PlayerPrefs | **RESOLVED** | spec §2 "Verified cross-profile hazards", S-02, S-06, S-09, S-22, OPEN-C4; plan Stage B; tasks B-02, A-02; R-087–R-089, R-025, R-093 | Both prefixes, the `rack.*` five-rack step IDs and the never-reuse rule are recorded as globals that live outside any save. Compact gets a disjoint namespace or an explicitly approved alternative. Legacy keys are never renamed, cleared, overwritten or backfilled. Two-way negatives: compact play does not change legacy keys (R-087), and legacy keys cannot complete compact steps (R-088). Rejected loads leave both namespaces unchanged. The v24 latch cannot seed compact progress. | Exact key policy: OPEN-C4. |
| 4 | **M1** geometry fallthrough | **RESOLVED** | spec §2 `StandGeometryModel` row, S-01, S-05, S-22; plan Stage B; tasks B-01; R-090, R-011 | `StandSizeMeters`, `IsKnown` and `ForSave` facts recorded. Inventory required of `ForestSaveValidation.Geometry`, `ScenarioOneObjectives`, `ScenarioReferenceArchive`, `StandWorldBoundary`, map/minimap and starting stand. Explicit dispatch; unknown IDs **fail closed** and never fall back to 40 m/64 cells or 80 m/256 cells. Pre-v20 saves still Legacy40. | Footprint: OPEN-C2. |
| 5 | **M2** schema append vs frozen hashes | **RESOLVED** | spec §2 hazards, S-03, S-22, OPEN-C4; plan Stage A; tasks A-02; R-022, R-023, R-091 | The contradictory "do not alter serial field layouts" sentence is replaced by two **unapproved** options: (a) a separate compact root/envelope, or (b) additive shared fields with audited identity-neutral stripping through the three named functions. Exact before/after legacy, pure-Sitka model-0 and Reference Future v1 world/schedule hash comparison is required. "STOP without rebaseline" (R-091). | Option (a) or (b): save owner. |
| 6 | **M3** substantive-intervention classifier | **RESOLVED** (consistency edit E-4) | spec S-13, S-15, OPEN-C5, §9; plan "Read-only design intersections", Stage D; tasks D-03, C0-02; R-051–R-054, R-094, R-095 | D-074 DRAFT FR-02/04/06 and MD-02/03 cited, plus the #413 NEEDS CHANGES review. The classifier and interval are an explicit OPEN (OPEN-C5) and a dependency of D-03. It covers bounded area, several real choices, linked succeeded resolution, mixed plans, manual one-rack equivalence, origin, elapsed years and reassessment. Rack clearance, planting, pruning, protection and isolated Fell "never automatically qualify". The old `HasReviewedTwoCycleSequence` is unchanged under D-077 P-1. "Three" is labelled the **Manager's DRAFT interpretation**, not a verbatim owner number (S-13, R-052, R-094, plan, §9). No old-world retroactive grading. | Classifier, interval and whether to ratify three: owner. |
| 7 | **M4** cadence state and compact teaching-complete | **RESOLVED** (optional E-6) | spec §2 last row, S-06, S-14, OPEN-C7; plan Stage B, Stage D; tasks B-02, D-04; R-027, R-057–R-061, R-093, R-096 | Cadence spec cited as APPROVED design, kept distinct from isolated candidate E (#402/#408) and from compact integration. Old stage-7 gate verified and not reused. **Separate** teaching-complete predicates are required for P and for A. P-2 FellTree (including access) is **navigation advice only**, never S-13 credit. 4-year advice and a 5-tick cap are labelled calibration. Stops cover Pending/Approved, unread review, preview/modal, storm/outcome/objective/lesson and Year 25/100. Quiet years are recorded with no synthetic acknowledgements. **Two** parity oracles: (A) direct ticks with no acknowledgement, hash-equal per year; (B) UI click-through compared semantically, not by raw hash. | Teaching predicate and whether skip is in the compact slice: OPEN-C7. |
| 8 | **M5** residual BA/canopy/access presented as existing | **PARTIAL** | spec §2 Work Plan row, S-10; plan Stage C; tasks C-02; R-039, R-097 | Done: #457's grep is "not proof of universal absence". Residual BA, retained canopy and access feasibility are **NEW/UNVERIFIED Stage C** unless a source location is traced. Method is current individual-tree DBH BA minus one chosen removal set, same-year crowns, with no growth/relaxation/forecast. L2 current vs L3 derived vs L1 actual. Expected costs are kept separate from settlements. **Gap (new inconsistency):** S-10 introduces an undefined term, "the **user-approved SelectedProjectedRemovalSet**". It is not defined anywhere and differs from S-17's defined `SelectedOpeningRemovalSet`. R-039 says "one chosen projected removal set". R-097 names both without saying whether they are the same set. "User-approved" could mean the player or the owner. This recreates, for the Work Plan display, the ambiguity M6 removed for the overlay. | Whether to add the outputs at all: owner (new scope). Which status set feeds them: OPEN-C5/C6. |
| 9 | **M6** opening overlay input set | **RESOLVED** (wording E-5) | spec S-17, OPEN-C6; plan Stage C; tasks C-03; R-067, R-068 | One defined term, `SelectedOpeningRemovalSet`, used consistently across S-17, R-067, R-068, plan and tasks. It is exactly one of draft paint, Pending, Approved, or a single-status selector. No union; completed Fell and MFM proposals excluded. Label must name the chosen source. Undo/cancel/approval/withdraw/selector changes recompute the overlay. Read-only and deterministic, current crowns only, never a forecast. Deferral allowed. | Which status: OPEN-C6. |
| 10 | **M7** gate order and study lane | **PARTIAL** (E-1 in study packet; E-3) | spec header "Implementation gate", S-21.1–6; plan Phase 0 steps 1–5; tasks C0-01–C0-04, "Binding sequencing"; R-099, R-100 | Done: the sequence matches the Manager packet exactly. #458 DRAFT → independent DELTA → separately claimed native/offline study → amended DRAFT → bounded delta review → owner/save decisions with one router → Manager APPROVED → #424 release → A–E → **one** Tier 3. Measured vs unmeasured is explicit (route length is not human walking or marking time). Browser-only generation is excluded. **Gaps:** (1) #457 M7 asked for C0-03 to have its own claim **and a #424 lease check**. Neither C0-03 nor Phase 0 step 2 says whether the study runs in the Unity Editor (which needs the machine-wide Editor lease) or as a plain managed harness. The docs cite the `Tools/Verification/` copy/run/remove pattern, which AGENTS.md defines as copying into `Assets/ForestPrototype/` (a Unity compile). `ForestStartingStand.cs` uses `UnityEngine`. `Tools/Verification` holds both Unity and non-Unity harnesses (for example `MfmSaveProvenance` vs `MfmSaveProvenanceUnity`). The lane is therefore under-specified. (2) Unchanged tasks.md "Handoff contract" still says "Do not dispatch Stage A prior to **DRAFT approval**". It should say Manager-APPROVED spec, per the new sequence. | Study authorisation and lane: Manager. |
| 11 | **m1** €2,500 wording | **RESOLVED** | spec §2 economy row, S-11; tasks C0-04, D-02; R-040 | "Minimum per commissioned contractor harvest visit/job", not annual. Matches `WorkPlanOverview.cs:129`. €12,000 and year 25 labelled gameplay calibration. | Compact calibration: OPEN-C5. |
| 12 | **m2** reviewer only identifies | **RESOLVED** | R-002; spec S-21.1; plan Phase 0.1 | The reviewer "identifies" blockers; only owner/Manager/save owner resolve them and promote status. | — |
| 13 | **m3** R-010 vs R-082 | **RESOLVED** | R-010, R-082 | R-010 is the canonical-context/Drive mirror read-back after an authorised authority change. R-082 is the final dashboard/status/ingestion receipts. Each cross-references the other; neither is verified. | — |
| 14 | **m4** MFM pinned tests | **RESOLVED** | R-036, R-103; spec S-22 | "Original pinned old-world MFM tests" continue unchanged, with no claim that MFM-08/09/10 passed. | — |
| 15 | **m5** v1–v24 fixture provenance | **RESOLVED** | R-021, R-101; spec S-22; plan Stage A; tasks A-02 | Inventory of the four directories (verified to exist), with origin and hashes. A freshly generated round trip is explicitly **not** old-save proof. | — |
| 16 | **m6** no fourth habitat mark | **RESOLVED** (coverage regression E-7) | spec S-07; plan Stage B; R-031 | Only None/Fell/CropTree (verified in `ForestTree.cs`). Habitat retention only "where the authoritative current game supports" it. "No invented fourth habitat-retain mark." | — |
| 17 | **m7** Approved-order edit policy | **RESOLVED** | spec S-08, **OPEN-C8** (new row); plan Stage C; tasks C-01; R-033, R-034, R-098 | Current behaviour (marks join on Work Plan open) cited. Approved-order edits are an explicit OPEN-C8 owner choice (withdraw/requote/reapprove or safe block); the implementer must not choose. | OPEN-C8. |
| 18 | **m8** vague acceptance terms | **RESOLVED** | spec S-06, S-15, S-17; plan Stage B, Stage E; R-006, R-042, R-053, R-072, R-074, R-104 | "Believable", "intuitive", "credible", "functioning", "meaningful" and "useful history" are replaced either by measured native evidence or by explicit **owner/user playtest judgement**. The performance budget is set after measurement. R-104 is a catch-all. No numbers invented. | Owner playtest. |
| 19 | **m9** D-073 V2 cite | **RESOLVED** | spec §2, S-09; plan intersections; tasks "External spec cross-links"; R-055 | APPROVED feedback V2 linked (status verified). L1 recorded / L2 observed / L3 derived / L4 authored, with ambiguous links labelled. | — |
| 20 | **m10** D-081 O2 novelty | **RESOLVED** | spec OPEN-C3, S-04; R-102, R-016 | Older compact Oak/Beech cohorts could dilute O2's irregular-cohort chapter. No cohort authorised. | OPEN-C3 owner. |

## Answers to the packet's ten particular stops

1. **B1: PASS.** Legacy future-version best-effort is not silently changed (S-02, R-018, R-084). A wrong or missing explicit compact ID is refused before all mutation on direct `LoadData`, Continue, F9/F10, reference and backup routes (S-02, R-013, R-085, R-086). `CanLoad` is per profile and checksums are required. See E-2 for a precedence clarification.
2. **B2: PASS.** One future approved router, citing SI-02/03/05 and FR-02/06. The old five-rack save stays, O2 stays independent, and there is no concurrent A-01 (S-04, R-083, tasks A-01).
3. **B3: PASS.** Legacy `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*` and five-rack IDs are untouched. Compact gets its own namespace or an explicit owner policy, with two-way zero-change tests (R-087–R-089).
4. **M1/M2: PASS.** Explicit dispatch and fail-closed handling, with no 40 m/64 or 80 m/256 fallback. Reference stripping and exact hash equivalence are required. Root vs schema append is left undecided (S-03, OPEN-C4, R-091).
5. **M3: PASS.** D-074 FR-02/04/06 and MD-02/03 are cited with #413 NEEDS CHANGES. The mixed/manual classifier and interval/reassessment stay OPEN. "Three" is labelled Manager DRAFT interpretation. The old two-cycle predicate is unchanged. One unlabelled residue remains in R-008 (E-4).
6. **M4: PASS.** The approved design, isolated candidate E and compact integration are kept distinct. P and A each need their own teaching-complete signal. P-2 is navigation only. There are two oracles, quiet-year handling and Year 25/100 stops.
7. **M5: PARTIAL. M6: PASS.** NEW/UNVERIFIED labelling, L1/L2/L3 separation and the per-visit €2,500 are correct. The overlay uses exactly one status-selected immediate set or is deferred, and is not a forecast. **But** the Work Plan's projected-removal set has an undefined second name (E-5).
8. **M7: PARTIAL.** The sequence is exactly right, with no fake tests and no repeated Tier 3. The study lane does not say whether it is Unity or plain managed code, and has no #424 lease check (E-1). There is also one stale "DRAFT approval" phrase (E-3).
9. **Minors: PASS.** m2 identifies only; m3 is separated; m4 has no MFM-08/09/10 PASS claim; m5 has provenance; m6 has no fourth mark; m7 is OPEN-C8; m8 is measurable or owner playtest; m9 cites the approved D-073 V2; m10 cites D-081 novelty.
10. **PASS.** OPEN-C1–C8 (spec §8, plan Phase 0.4, handoff) and OPEN-1–OPEN-6 (spec §8 footer) remain **undecided**. There is no old-world, Reference or pure-Sitka hash rebaseline (S-03, S-22, R-022, R-023, R-091) and no test is weakened ("repair profile selection, not the old tests", tasks "Risk escalation"). OPEN-C1 keeps a *recommendation* ("prefer abbreviated real map-first plan only if … measured feasible"); it is clearly labelled as a recommendation, not a decision.

## Edits still required

These are document edits only. None needs a product or save decision from the editor.

**E-1 (M7) — for the Manager's C0-03/C0-04 study packet; mirror it into the amended DRAFT.** State whether the study is:
- **(a)** a Unity Editor harness using the `Tools/Verification/` → `Assets/ForestPrototype/` copy/run/remove pattern. This needs the **machine-wide Unity Editor lease via #424**, a clean dedicated worktree and the AGENTS.md "no temporary script or `.meta` left behind" check; or
- **(b)** a plain managed harness (like `Tools/Verification/MfmSaveProvenance`) that does not need the Editor. In that case it must say how `UnityEngine`-dependent stand generation (`ForestStartingStand.cs`) is exercised without changing production source.

In both cases, add the "#424 lease/lock check" #457 asked for to C0-03. This choice is about the study lane, not a product decision. **This is the only edit that must be settled before the study starts.**

**E-2 (B1) — clarification, amended DRAFT.** S-02, R-015 and R-085 require compact→legacy rejection before mutation on *every* route. Today the legacy route only warns on an ID mismatch, after mutation. Applying the shared preflight to the legacy route is therefore a (desirable) change to legacy load behaviour, beyond the protected future-version rule. State two things:
- that this legacy-route identity check is itself inside the save-owner sign-off (OPEN-C4);
- the **order of checks**: an explicit foreign or compact identity must be rejected **before** legacy newer-version best-effort applies. Otherwise a compact payload with a higher schema number reaching the legacy route could be "best-effort" loaded, contradicting R-015 while still satisfying R-084.

#436 SI-05 ("foreign/explicit conflicting ID always rejects") already points this way. Cite it as DRAFT, not as shipped.

**E-3 (M7) — amended DRAFT.** In the tasks.md "Handoff contract", replace "prior to DRAFT approval" with "prior to Manager-APPROVED spec (after the S-21 gates)".

**E-4 (M3) — amended DRAFT.** R-008 still reads "Economic feasibility across three cycles". Relabel it, for example: "across the candidate substantive-cycle count (Manager DRAFT interpretation: three; owner ratification pending)". This matches S-13, R-052 and R-094.

**E-5 (M5, M6) — amended DRAFT.**
- In S-10, either replace "user-approved SelectedProjectedRemovalSet" with the defined `SelectedOpeningRemovalSet`, or define a second named set and add its status choice to OPEN-C5/C6. Use the same term in R-039, R-097 and tasks C-02. Say whether "approved" means the player's in-game choice or the owner's design choice.
- In S-17, the phrase "one explicitly chosen status source only **after approval**" can be misread as "Approved orders only". Reword it to "one status source, chosen by owner decision OPEN-C6".

**E-6 (M4) — optional.** Pin isolated cadence candidate E by its SHA (`e759193354b4613f88e3370a62ccaccf02c556cf`, as cited by #457/#413) wherever "candidate E (#402/#408)" appears.

**E-7 (new delta observation; coverage regression, m6/S-07 area) — amended DRAFT.**
- Original R-030 "**Player can reach/inspect** eligible standing trees throughout actual treatment polygon on both sides" was rewritten as an eligibility-rule check only. Physical reachability and inspection of the whole two-sided parcel is a core product requirement, still stated in S-07 ("allow player to reach and inspect every eligible tree"). No checklist item now tests it directly (R-006 measures route length, not reach). Restore the reach/inspect wording in R-030, or add it as a new checklist item.
- Similarly, original R-039 "without double counting corridor" was dropped. Restore it, or state that "declared area/unit/masks" covers it.

The #458 handoff's statement "original R-001–R-082 all preserved" is true for **IDs**, not for every requirement's substance.

## Evidence limits

- **Method:** read-only `git fetch` of remote-tracking refs, then `git show`, `git diff` and `git grep` against objects in the user's existing clean `main` clone (no checkout, no branch, no file change in that clone), plus `gh` reads of #424, #458, #459 and live `active-tasks.md`. The **only write** is this file, through the GitHub Contents API on the reviewer branch.
- **Live coordination:** read, **not edited**. `active-tasks.md` content SHA `5dda6a8c63bc5d54bee429ab439a9572e795e4e6`; no #458/#459/compact-review overlap. Claim registered [RUNNING] on #459 before this write.
- **Not run or not assessed:** no Unity/Editor, compilation, tests, harnesses, fixture generation, PlayerPrefs or save access, hash replay, rendering or performance work. No full audit of Work Plan fields (M5 remains "not found by targeted search" plus #458's NEW/UNVERIFIED label). No audit of the #443 feature branch (owned by #424). #436 and D-074 were checked only for the cited IDs and wording, not reviewed in full. Each spec's internal correctness beyond the 20 findings was not re-reviewed, apart from the rewritten checklist lines compared above. Dashboard and Drive mirrors are **NOT VERIFIED**.
- **No checklist box checked. No owner, save-owner, router, classifier, cohort, overlay-status or Approved-edit decision made.**

## Requested next action

Manager #425 decides what to do with this verdict. A satisfactory delta review only lets the Manager **commission a separately claimed native/offline C0-03/C0-04 study**, whose packet should settle E-1. E-2 to E-7 go into the amended DRAFT and its bounded delta review. This verdict does not approve the spec or authorise implementation. **User action now:** none required by this review.

---

## Addendum (user instruction, 13 Nov 2026): targeted refactoring recommendation

**Status:** architectural recommendation only. This addendum does **not** authorise refactoring production code, change any accepted product decision, approve the DRAFT or alter the verdict above. Evidence is a read-only static inspection of `main` @ `6c13a9e1`. The #443 Objective 1 branch was **not** inspected and may already change some of these files.

### What the code shows

| System | Inspected fact | Refactor implication |
| --- | --- | --- |
| Scenario configuration | `ScenarioOne/ScenarioOneDefinition.cs` is already a ScriptableObject. It holds `scenarioId`, `definitionVersion` (`scenario-one-v13`), economy (€12,000 start, €2,500 minimum per visit, rates), completion thresholds (year 25/100, canopy, regeneration, deadwood) and reference values. The editor script `Editor/ScenarioOneSetup.cs` wires **one** asset into the scene. | **Existing seam; reuse it.** Most compact calibration can be a *second definition asset* (data, not code). The missing piece is choosing which asset is active at New Game/Continue. |
| Save slot | `ForestSaveController.cs:9-19`: `const SaveFileName = "forest-save.json"`, with `.tmp`/`.bak` derived from it. `CanLoad`, `TryReadSave` and `FindLoadProblem` are already a shared pre-load path (:240, :261, :296). `ForestSaveData` is one class (v24) with a nested `ScenarioOneSaveData`. | **Prerequisite.** The slot must become a parameter, and an identity check must join the existing pre-load path. #436 needs exactly the same change for O2, so it should be done once. |
| Geometry | `StandGeometryModel` is a plain `int` referenced by 29 files. Binary checks against Legacy40/Enlarged80 appear in `StandGeometryModel.cs:34,62`, `ForestSaveValidation.Geometry.cs:15`, `ForestStartingStand.cs:83`, `ScenarioOneManager.RackPlan.cs:40,92`, `ScenarioOneObjectives.cs:104`, `ScenarioOneRackLayout.cs:18`, `ScenarioReferenceArchive.cs:337` and `StandWorldBoundary.cs:76`. | **Prerequisite.** Use explicit dispatch with fail-closed handling (M1). Useful property: `ScenarioOneRackLayout.AppliesTo` and `RackPlanningV2Active` (59 references) are already true **only for Enlarged80**. A compact geometry ID therefore switches off the five-rack V2 and MFM paths *without new conditionals*, as long as those gates are left alone. |
| Learning/help | `UI/LearningObjectivesView.Curriculum.cs` has a `const PreferencePrefix` and one static `Topics` array (48 steps, five-rack wording). It is "kept free of UI Toolkit and PlayerPrefs … checked offline". `UI/ScenarioOneTeachingSequence.cs` (136 lines) hard-codes a five-lesson stage machine (`Current(f) == 5`, rack-bay review). `MenuHelpView` has its own constant prefix. | **Prerequisite (small):** pass the prefix and the curriculum in as inputs instead of using constants; legacy values stay byte-identical. **Within the feature:** write a *separate* small compact lesson sequence rather than adding compact branches to the five-rack stage machine. |
| Scenario manager | `ScenarioOneManager.cs` is ~141 KB plus 9 partial files (racks, rack plan/editing, prescriptions, approval, history, storms, salvage). It is referenced from 40+ files outside `ScenarioOne/`, including shared `ForestEcologyController` (competition/clearance owner through `FindFirstObjectByType<ScenarioOneManager>`, :44/:307/:485). | **Defer.** Splitting or renaming it is broad, high-conflict, and touches files #443 owns. Compact can reuse it with its definition, slot and geometry seams. |
| Economy | `WorkEconomy/` (`ForestryPriceBook`, `ForestryWorkDomain`) is a separate namespace. `ScenarioOneEconomyAdapter` maps the definition onto it. | **Already shared.** Compact needs a different definition asset, not engine changes. |
| Species/ecology | Species are ScriptableObject assets. `Species/ScenarioTwo/` assets and `ScenarioTwoSpeciesSiteResponses` already exist. The annual engine is `ForestEcologyController` (~106 KB). | **Keep shared and untouched.** No refactoring is needed for compact. |
| Stand generation | `ForestStartingStand.Generate` takes an Enlarged80 branch or a legacy lattice branch, and uses `UnityEngine`. | **Within the feature:** add a compact generator branch alongside. Extract only the helpers the compact generator actually reuses. Do not rewrite the Enlarged80 generator, whose output is protected by hashes. |
| Reference identity | `ScenarioReferenceArchive.cs:248-272` strips fields by text pattern. | Any shared-schema change falls under M2/OPEN-C4. Choosing a separate compact root removes this risk entirely. |

No scenario interface, base class or registry exists, and `ScenarioExecutionMode` has one value. This is a good thing: there is nothing to unwind, and none should be introduced now.

### What stays shared vs scenario-specific

- **Shared, one implementation, no compact forks:**
  - annual ecology and species engine (growth, competition, regeneration, browse, deadwood, mortality, storms);
  - WorkEconomy domain, work orders, Work Plan resolution;
  - management events/history and Annual Review (D-073 L1–L4);
  - tree marking (`TreeMarkType`);
  - the pre-load save validation graph (`FindLoadProblem`);
  - map/minimap rendering;
  - one-year advance and the approved D-077 skip.
- **Scenario-specific, as data where possible:**
  - definition asset (economy and completion values);
  - save slot name and world/geometry/content IDs;
  - starting-stand generator and geometry;
  - curriculum, learning/help prefix and teaching sequence;
  - objective/completion policy (the compact predicate is a new class; `HasReviewedTwoCycleSequence` is untouched);
  - access lesson (five-rack V2 legacy vs one-rack compact);
  - MFM (legacy only).
- **Scenario Two:** it reuses the same slot parameter, geometry dispatch and prefix/curriculum inputs. Nothing O2-specific should be built now beyond the fixed slot table entry that #436 already proposes (SI-03).

### Prioritised recommendation

**P1 — Prerequisites (foundation). Before compact content, after the S-21 gates and #424 release.**

1. **F-1 Geometry dispatch, fail-closed.**
   - Change `StandGeometryModel` plus the 10 binary sites listed above to an explicit known-ID lookup.
   - Unknown IDs fail closed; Legacy40/Enlarged80 behaviour is unchanged; leave `RackLayout.AppliesTo` and `RackPlanningV2Active` as Enlarged80-only.
   - **Independent of the router decision.** It can land first as a pure, identity-neutral refactor once the compact ID is decided, or even before that, with the unknown-ID path only.
   - **Risk:** hash or label drift; `StandGeometryModel.Label` strings must stay identical.
   - **Gate:** Tier-1 dispatch negatives plus before/after Legacy40, Enlarged80, Reference Future v1 and pure-Sitka hash equality (A-02 fixtures).
2. **F-2 Slot-parameterised save path and identity preflight, plus a minimal active-profile record.**
   - Replace the `SaveFileName` constant with a small fixed slot table (legacy O1 / compact / O2).
   - Add the explicit-ID check inside the existing `TryReadSave`/`FindLoadProblem`/`CanLoad` path.
   - Add one small serializable **profile record**, chosen at the title menu. It names the definition asset, slot, geometry ID and curriculum key. It is passed through existing scene/manager references, **not** a singleton or service locator.
   - **Owner:** the single approved router writer only (resolves B2; do this together with #436, never twice).
   - **Risk:** changing legacy load behaviour (see E-2).
   - **Gate:** the two-way, zero-checksum negatives R-013–R-018 and R-084–R-086.
3. **F-3 Learning/help inputs.**
   - Make the prefix and curriculum instance inputs supplied by the profile. Legacy `CCF.Learning.v1.` and `CCF.MenuHelp.v1.` stay exact.
   - Fold this into the F-2 claim, because the rejected-load oracle includes PlayerPrefs.
   - **Gate:** R-087–R-089.

**P2 — Refactor inside each feature task. Touch only what the feature needs.**

- **B-01:** compact generator branch in `ForestStartingStand`; map/minimap extent comes for free from F-1.
- **B-02:** new compact teaching sequence class and curriculum data. Reuse lesson rendering; do not branch inside `ScenarioOneTeachingSequence`.
- **C-02:** residual BA/opening as a new **pure calculator** class, testable offline, read by `WorkPlanView`/`StandMapCanopyRaster`. No rewrite of either view.
- **D-02/D-03:** compact values in the second definition asset; compact completion/classifier as a new static class beside `ScenarioOneCompletionRules`. Objective area ratios come from the definition rather than new `if (compact)` checks.
- **Rule for every feature task:** profile checks only at the entry points listed above. A new compact check deep inside shared ecology, economy or history code is a STOP-and-report signal, not a quick fix.

**P3 — Defer until the compact scenario is playable and accepted.**

- Splitting or renaming `ScenarioOneManager` and its partials.
- Removing `ForestEcologyController`'s `FindFirstObjectByType<ScenarioOneManager>` coupling.
- A generic `IScenario`/eight-scenario engine.
- Restructuring `ForestSaveData`, unless the save owner chooses the separate-root option, which then belongs to F-2.
- Generalising MFM to N racks (OPEN-1–6).
- Splitting `ForestTest.unity` or extending `ScenarioExecutionMode`.
- Renaming `ScenarioOne*` types.

Each of these touches protected or #443-owned files and offers no compact benefit.

### Avoiding unnecessary abstraction

Yes, it can be avoided. Compact needs:
- **one** extra data asset (definition);
- **one** small profile record;
- **one** slot table;
- **one** geometry lookup;
- **two** inputs on existing views (prefix and curriculum);
- a few **new**, separate classes (generator branch, compact lesson sequence, compact completion predicate, residual-BA calculator).

None of the working forestry systems needs rewriting. Generalise further only when O2 *actually* needs something the slot table and definition assets cannot express.

### Sequencing to minimise duplication, conflicts and retesting

1. Existing gates unchanged: native study → amended DRAFT → decisions, including a single router → Manager APPROVED → #424 release. **Suggested DRAFT amendment (not made here):** add F-1 as an explicit foundation task (for example "A-00") ahead of A-01, and add the "entry-point-only profile checks" rule to plan Stage A–D stop rules.
2. **F-1** (geometry): one writer, after #443's handoff lands so the shared files `ScenarioOneManager.RackPlan.cs` and `ScenarioOneObjectives.cs` aren't edited by two lanes.
3. **F-2 + F-3** (router, profile, prefs): the single router owner, combined with the #436 amendment.
4. **A-02** legacy/Reference/pure-Sitka regression suite is established **once** here as the offline Tier-1 baseline.
5. **B → C → D** features. C (Work Plan/map files) and D (completion/economy files) may run in parallel **only** with disjoint file allowlists. `ScenarioOneManager*` partials and `ForestSaveController` stay single-writer hotspots.
6. **Retest rule:** any task touching F-1/F-2 files reruns the cheap A-02 Tier-1 legacy suite. Other tasks run their own Tier-1 plus focused Tier-2. **Tier 3 runs once** at F-01, as the DRAFT already requires.

**Main risks:**
- foundation refactors drifting protected hashes (mitigated by the A-02 before/after gate);
- the profile record growing into a service locator (keep it as a plain data record passed explicitly);
- in-feature refactors creeping into #443-owned or shared-engine files (STOP rule);
- doing the slot work twice (once here, once for #436); avoid this through the single-router decision.

**No hash rebaselining or test weakening is implied by any item above.**
