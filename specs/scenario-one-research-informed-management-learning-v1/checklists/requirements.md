# Requirements and acceptance checklist — Scenario One Research-Informed Management Learning V1

**Status:** DRAFT / NOT VERIFIED. No checkbox below is evidence of a pass.  
**Authority:** [spec.md](../spec.md), [plan.md](../plan.md), [tasks.md](../tasks.md)  
**Date:** 2026-11-12. The complete-loop Scenario One demo has its own existing gates and is not automatically blocked or modified by this future improvement scope.

## A. Design and source gates — before implementation

- [ ] **REQ-01 — Product acceptance.** Product owner has explicitly approved scope, post-demo versus any pre-demo exceptions, and if applicable persisted retention-intent semantics; status no longer DRAFT.
- [ ] **REQ-02 — Evidence independence.** Research classification states P1–P6 share one co-author, do not independently validate Irish coefficients, and distinguishes P1's crop-tree-vs-competitor study from P2's low-vs-crown study.
- [ ] **REQ-03 — Irish and UK marking/wind evidence.** #387 returned a public-primary-source, page-linked report; Manager reviewed/accepted or specifically narrowed any unsupported copy.
- [ ] **REQ-04 — Young-tree terminology.** #388 returned evidence/semantic caveats; copy approved for abstract modeled natural cohorts and exact planted juveniles.
- [ ] **REQ-05 — Canonical authority unchanged.** No change to accepted Scenario One pre-demo objectives, MFM, Rack V2, D-072, S1-C/D or frozen Reference Future is introduced without separate explicit user decision.
- [ ] **REQ-06 — Live source/ownership.** Implementing worker inspected the exact code/serializer/UI baseline; \`Docs/Project/coordination/active-tasks.md\` and actual current machine/Unity locks, branch and worktree; claim set [RUNNING] before production edits.
- [ ] **REQ-07 — Approved four-part spec.** \`spec.md\`, \`plan.md\`, \`tasks.md\`, this checklist cross-consistent with independent review where needed. No implied approval from Opus research review.
- [ ] **REQ-08 — No redundant infrastructure.** Current MFM Why/coverage, S1-C history and S1-D advisory UI have been inspected; new features reuse their APIs rather than creating a second system.
- [ ] **REQ-09 — Evidence class labelled.** Each sourced claim used in player copy identified as direct evidence, transferred methods, model result, calibration, or unsupported and excluded.

## B. Competition explanation and marking logic (FR-01/02)

- [ ] **AC-01 — Exact Share.** Competitor panel displays the authoritative current \`CompetitionShare\` value; there is no duplicate or altered Hegyi calculation.
- [ ] **AC-02 — Clear scope.** Copy says Share reflects local size/distance-based competition and does not guarantee unique expert advice or tree quality.
- [ ] **AC-03 — Multiple defensible sets.** In a controlled fixture, two different removal sets that both satisfy existing 30% Share/40% safeguards are not graded by specific chosen tree IDs. Any other validation failure is reported independently.
- [ ] **AC-04 — Competition remains.** Optional "retain" language or action does not reduce computed Share, mark trees or award release credit. A legitimate failure to meet 30% still fails.
- [ ] **AC-05 — Distinct action states.** Crop Tree designation, manual Fell mark, MFM proposal, approved work order and executed result are not confused by labels, state or display.

**Evidence required:** exact baseline/candidate rule comparisons, one tree-neighbour fixture with two alternative marking sets, focused UI captures and explanation-source readout. **Not:** new scoring formula or imported thinning density.

## C. Woodland regeneration and natural language (FR-03/04)

- [ ] **AC-06 — No internal grid jargon in player copy.** Objective, Annual Review, Century Review, Help and inspected tutorial progress do not say "regenerating cells" or "regeneration cells" as the player unit. Technical C# and diagnostics retain legitimate internal names.
- [ ] **AC-07 — Objective unchanged.** Stable objective ID, current and target values, achieved flag, spatial occupancy counting and historic data are byte/behaviour-equivalent before/after pure-copy update on accepted baseline.
- [ ] **AC-08 — Honest presence unit.** New text describes where young growth is recorded, not the number of established individual saplings, area in hectares, verified survival, or species-specific stems/ha.
- [ ] **AC-09 — Old/save/review.** Legacy40, Enlarged80, historical and Year-100 aspirational review labels render correctly without reinterpreting or rewriting recorded numeric values. Frozen Reference Future remains unmodified.
- [ ] **AC-10 — Recorded origin only.** Natural regeneration vs planted young trees displayed only where source data genuinely preserve their origin/stage; no silent inference from species or tree location.
- [ ] **AC-11 — Environmental explanation faithful.** Qualitative shelter/edge/open guidance does not invent geometric bands or local microclimate/wind measures; where no legitimate local classification exists, wording is general rather than asserting a measured condition.
- [ ] **AC-12 — Separate regeneration and access.** No screen treats extraction racks or operational machine tracks as P6 regeneration strips, and no new opening/planting/seed rules exist.

**Evidence required:** copy/string inventory, approved wording source, screenshots at two resolutions, stable save/snapshot/goal predicates, an example with natural cohorts and exactly planted young trees plus a case where only aggregate presence can honestly be shown.

## D. Wind/stability wording (FR-05)

- [ ] **AC-13 — Default simulation truth.** The limitations message accurately says the default Scenario One simulation does not model wind damage; no player-visible wind-risk number is invented.
- [ ] **AC-14 — Two-sided caution.** If mentioning field forestry wind stability, both dense unthinned conditions and abrupt release/exposure are handled contextually with sourced wording; no direction claimed universally safe/unsafe.
- [ ] **AC-15 — No activation or benefit.** Storm Model 0 stays default; no model/risk coeff or delayed mortality rule is created, and a player retention reason cannot claim modelled wind benefit.

**Evidence required:** code/config view of default model profile and source-cited copy; not a forced-storm full simulator qualification.

## E. Optional recorded intention — event, data and migration (FR-06/07)

**Only apply E if the user explicitly approves the durable form of the optional decision. If session-only or no feature is chosen, document the alternative instead; do not mark unused checks as passing.**

- [ ] **AC-16 — Existing authority.** Single current Scenario One event timeline is used or a Manager-approved spec amendment proves another design; no parallel authoritative history store.
- [ ] **AC-17 — Stable relationship.** Event records the actual target Crop Tree ID and the actual neighbouring tree ID, authentic year, stable event ID, player-authored flag/intention type and versioned reason code.
- [ ] **AC-18 — Limited reasons.** Accepted labels represent intention (possible shelter, potential habitat, future timber, later reassessment), not asserted ecological fact. No hidden tree quality/real habitat/wind certainty.
- [ ] **AC-19 — Append-only amendments.** Record/revise/clear append new valid events, never overwrite past recorded intention. Latest active state derived deterministically.
- [ ] **AC-20 — Idempotence.** Clicking same reason twice without an intervening meaningful state change does not create duplicate authoritative history.
- [ ] **AC-21 — Life-cycle truth.** Tree later felled/dead or crop-tree mark removed: prior intention remains in historical record, but is not displayed as live protection/required action.
- [ ] **AC-22 — Old saves conservative.** Pre-feature, Legacy40 and Reference Future worlds do not fabricate player retention reasons; missing fields mean "not recorded".
- [ ] **AC-23 — Current schema.** Next available save version and migration chosen after inspecting actual implementation; no reserved enum collision or stale copied version.
- [ ] **AC-24 — Roundtrip.** Save/load/advance/reload yields identical surviving intent history and valid event IDs; history query/inspection does not mutate saves.
- [ ] **AC-25 — No chronology corruption.** Annual report counts, successful work events, grouped-harvest money, scenario review acknowledgement, plan grouping and two-reviewed-cycle completion behave identically in equivalent states with/without intent annotations.

**Evidence required:** deterministic add/revise/clear/reload/old-save fixtures; malformed/duplicate event IDs fail safe; comparison against original S1-C and S1-D verification anchors. **Independent review mandatory** for new save/event code.

## F. Optional player action and historical review (FR-06/08)

- [ ] **AC-26 — Optional action.** Real inspected neighbour of a confirmed Crop Tree can be intentionally retained with a reason, or left unannotated with no penalty.
- [ ] **AC-27 — No new physical mark.** No paint, permanent key or separate management mode added. Existing tree selection and Work Plan marking remain as before.
- [ ] **AC-28 — Current pair and reason.** UI shows correct Crop Tree, neighbour and selected player reason; no reason can be assigned to unrelated/stale IDs.
- [ ] **AC-29 — Accessible re-entry.** Optional control retains focus/navigation compatibility with existing map, Work Plan and modal Help; no 1280×720 overlap or hidden CTA.
- [ ] **AC-30 — Neutral to biology.** Same world/annual step with intent event and without intent produces identical world-state biology, competition, costs, Work Plan and objectives given same actual work.
- [ ] **AC-31 — L1 player history.** Historical view labels the reason as "you chose" with correct year/IDs and no pretence that the game observed shelter/habitat value.
- [ ] **AC-32 — L2 present state.** Later review can accurately tell whether the neighbour is still alive/present using authoritative current state; stale/missing data label "not recorded" or "not available".
- [ ] **AC-33 — L3 derived honestly.** Current vs past Share/canopy/light comparisons appear only where past values really exist or are exactly reconstructable; never infer unrecorded original Share from current geometry.
- [ ] **AC-34 — L4 context.** Advisory text prompts reassessment but never forces felling, rescoring, no-work-year penalty or invented ecology.
- [ ] **AC-35 — Multiple decisions.** Retained tree later felled as ordinary approved work; first intention remains discoverable historically, no false live annotation, and later year remains valid.
- [ ] **AC-36 — Existing S1-C history.** A pre-feature intervention without player intention shows no reason recorded, not a fabricated past rationale or incorrect "Player retained" assertion.
- [ ] **AC-37 — No extra screen infrastructure.** S1-C aggregate-first/year browsing and S1-D advisory re-entry continue working without a second dashboard, map-history mode or export feature.

**Evidence required:** two distinct complete interaction traces, history in different years, one no-event old save, one revised reason and one later removal, plus captured UI on actual candidate.

## G. MFM / Work Plan / teaching non-interference (FR-02/06–08)

- [ ] **AC-38 — Nonproposal.** Recorded retention is not a Fell mark, Crop Tree mark, MFM proposed removal, Work Plan order or independent prescription approval.
- [ ] **AC-39 — Manual precedence and normal Work Plan.** Existing manual marking, candidate confirmation, proposal review, exclusion, approval and payment follow accepted MFM rules with or without optional intent.
- [ ] **AC-40 — No implicit veto.** A stored intention does not silently block an MFM proposal, accepted Work Plan or later felling; any future alert needs a separate product rule.
- [ ] **AC-41 — Protected thresholds.** 30% sample release, 40% removal ceiling, 8 m relevant constants, Year-25 objective values and two-cycle chronology are unchanged.
- [ ] **AC-42 — Candidate quality honesty.** Spacing-only MFM suggestions remain candidate-inspect only, never automatic proof of Crop Tree quality or broadleaf species equivalence.
- [ ] **AC-43 — Stable save/reference.** Legacy40, pre-feature Enlarged80, Reference Future v1 and current saved tutorial steps remain meaningfully compatible and unmodified.

**Evidence required:** source+runtime comparators for identical work with/without annotation; explicit no-work/multiple-year cases; preserved immutable reference and approved MFM tests.

## H. Optional offline structure/method work (FR-09)

- [ ] **RESEARCH-AC-01 — Authentic data.** Tree positions/species/DBH/live status and saved years extracted from identified real fixture worlds; source/revision/time noted.
- [ ] **RESEARCH-AC-02 — Spatial edge robustness.** K-nearest neighbour tie-breaking, out-of-estate neighbours, sample region and edge fraction addressed; no false stand-wide score from a small central sample.
- [ ] **RESEARCH-AC-03 — Cohort boundary.** Abstract natural regeneration and exact planted juveniles not treated as fully measurable adult tree stems without a supported identity/DBH measurement.
- [ ] **RESEARCH-AC-04 — Determinism.** Same saved state yields same mingling and DBH differentiation results; formulas and units documented; no in-game world state changed.
- [ ] **RESEARCH-AC-05 — Descriptive honesty.** No biodiversity index claim, Irish conservation threshold, success grade or player reward; if near-zero/uninformative, recommendation is "do not build panel".
- [ ] **RESEARCH-AC-06 — Competition model comparison.** Hegyi/RBA comparison explicitly reports foreign-data scope and that P5 does not establish superior calibration for Irish Scenario One; no P5 coefficient enters production.
- [ ] **RESEARCH-AC-07 — Publication.** Research worker posts exact fixture/algorithm/outputs and uncertainty, with a Manager disposition; no Tier-3 Unity gate for a standalone offline result.

## I. Qualification discipline and release integrity

- [ ] **QUAL-01 — Exact candidate.** All Tier-1/Tier-2 tests and screenshots refer to one inspected source branch/HEAD and qualified Unity project, not a different checkout or historical passed build.
- [ ] **QUAL-02 — Proportionate suite.** Copy-only checks do not trigger full integration suite. Event/save changes receive focused deterministic/compatibility qualification. Tier-3 is reserved for accepted integrated milestone/release candidates.
- [ ] **QUAL-03 — Render & input.** UI tested for actual 1280×720 and 1920×1080 layout and focus; test output documents actual input type and gaps.
- [ ] **QUAL-04 — No test weakening.** Existing protected scenario, history, MFM and input assertions not edited merely for green; any stale assertion has an explicit superseding accepted product decision.
- [ ] **QUAL-05 — Independent review.** New persistence/cross-system feature gets a separate qualified reviewer and clear Manager APPROVED/NEEDS CHANGES/REJECTED verdict; source-only copy may get lighter review.
- [ ] **QUAL-06 — Safe local ownership.** No cross-writer/file overlap, unrelated dirty changes, forced worktree removal, killed Unity process, or reused unverified local owner lease.
- [ ] **QUAL-07 — Immutable reference.** Frozen Reference Future v1 remains byte/integrity-identical and Legacy40 compatibility preserved.
- [ ] **QUAL-08 — Manual test and Windows.** Player personally tests and authorises any Windows build when appropriate; successful Unity Editor tests alone do not imply acceptance.
- [ ] **QUAL-09 — Research vs game truth.** Final handoff identifies exactly implemented/tested, reviewed, integrated, verified and accepted states without merging them into one status.
- [ ] **QUAL-10 — Publication.** Each dispatched work result has a durable GitHub Markdown handoff; issue claim set [HANDED-OFF] and closed on report acceptance; research and docs context/Drive mirror status accurately marked.

## J. Blockers and sign-off table

| Decision/gate | Current truth | Required action before closing |
|---|---|---|
| Scope and timing | DRAFT; prior research-review freeze protects pre-demo gameplay | Explicit product-owner acceptance, no inferred blanket approval |
| Retention persistence | Preferred append-only event architecture only | Product owner chooses durability; inspect live event/serializer and approve full contract |
| Irish source copy | Browser reports not returned | #387/#388 evidence or narrower conservative copy accepted |
| Actual UI/source filenames | GitHub source reviewed only in part; no current local UI candidate inspected | M-02 exact file/owner inventory |
| Default wind damage | Storm Model 0 for new games per accepted context | Limit claims to the actual default profile |
| Model compatibility | Frozen Reference v1 / Legacy40 protected | Current exact-candidate tests before implementation approval |
| Review/approval | No independent approved spec handoff yet | Reviewer verdict and Manager acceptance after user scope approval |
| Dashboard/mirrors | Unverified | Do not claim synced until ingestion/readback performed |

**Current result:** Complete draft checklist, no checks executed and no production implementation started.
