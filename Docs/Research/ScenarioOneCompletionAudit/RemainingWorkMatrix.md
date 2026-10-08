# Remaining work matrix — Scenario One

Base: `main` @ `869ee92`, assuming the P3 candidate `0ab7399` integrates without material change. Size: **XS** < ½ day · **S** ≈ 1 day · **M** 2–4 days · **L** ≥ 1 week (one agent, including gates). "Blocks external test" means the **private beginner test** (`ReleaseGates.md` B).

Order column: **0** = before any packet; **A–D** = packet S1-A … S1-D (`ImplementationSequence.md`); **H1** = first beginner test; **later** = after the gates named in `ImplementationSequence.md` §4.

## 1. Integration, verification and decisions

| # | Item | Current state | Authority / evidence | Player impact | Size | Dependencies | Test requirement | Blocks external test? | Order |
|---|---|---|---|---|---|---|---|---|---|
| 1 | Integrate P3 | CANDIDATE, Unity-verified on branch | `P3ImplementationRecord.md` | Sees what remains, money, dead-end warning | XS (integration) | MenuTutorial classified (#2) | P3 gates on the merge result; anchors unchanged | **YES** | 0 |
| 2 | Classify MenuTutorial "Help Escape also closed inspection" | Unclassified failure (P3 regression 23/24; P2 run 24/24) | P3 record "Tutorial/Help failure" | Possibly Esc closes more than expected | XS–S | — | Reproduce on main `869ee92` interactive ×2; harness vs production verdict | **YES** (an unexplained red gate) | 0 |
| 3 | P2 human smoke | NOT PERFORMED | P2 record "Manual play smoke" | Readability of the competitor panel and rings | XS (human, 20 min) | — | 15-step manual list at 1280/1600/1920 | **YES** | 0 |
| 4 | P3 human review | NOT PERFORMED | P3 record "Manual review" | Does any line read as a verdict? | XS (human) | #1 | Manual list in P3 record | **YES** | 0 |
| 5 | Record P2 (and P3 on integration) in Overview / Decision Log; refresh milestone header and live locks | Drift | `CurrentStateAudit.md` §12 | None directly | XS (docs) | #1 | Generator run on a clean context commit | NO | 0 |
| 6 | Decision E1: no single forestry score | PROPOSED | Readiness `DecisionMatrix.md` E1 | Tone of all feedback | XS (decision) | — | Decision Log entry | NO (holds in code today) | 0 |
| 7 | Decision J3: clearance preview only after U | PROPOSED | Readiness S9 | Removes a constant "clear vegetation" invitation | XS decision + S code | — | `ClearanceVerification` updated | NO | A (if accepted) |
| 8 | Decisions N1/N2/L3/O1/M1 (completion model, deadwood objective, broadleaf requirement, hard fail, trigger thresholds) | PROPOSED | Readiness `DecisionMatrix.md` | Defines what "completing" Scenario One means | S (decision session) | Beginner test 1 evidence preferred | Decision Log entries | NO | before D |

## 2. Teaching, terminology and objective visibility (packet S1-A)

| # | Item | Current state | Authority / evidence | Player impact | Size | Dependencies | Test requirement | Blocks external test? | Order |
|---|---|---|---|---|---|---|---|---|---|
| 9 | P1 copy ported onto v19 + P2 + P3 | CANDIDATE (v16) | `1a36ea9`; P2 overlap notes | CCF defined; positive selection framed; prompt order C before X | M | #1, #2 | `ScenarioOneTeachingCopyVerification` re-based to v19, batch + rendered | **YES** (CCF is never defined on main) | A |
| 10 | Replace P1's Model-1 clearance sentence with the model-aware explanation everywhere | P1 text false for new games | D-049; `LearningObjectivesView.cs:131–136` | Prevents a false causal claim | XS (inside #9) | #9 | String gate per regeneration model (0/1 vs 2) | **YES** | A |
| 11 | Land corrected harnesses + `ScenarioOneGateModes.md` on main; drop runner overlay | Overlay from `1a36ea9` | `run_regression.py:8` | None | S | #9 | Full regression from tracked sources only | NO (technical) | A |
| 12 | Species display names in objectives, review lines, century review (render-time map; stored ids unchanged) | Raw ids shown | `ScenarioOneObjectives.cs:92`; `ScenarioEcologyReviewLines.cs:40` | "Establish planted sessile-oak" | S | #9 | String sweep: no raw species id in rendered text | **YES** (first objective a tester reads) | A |
| 13 | "cohort"/"juvenile" → plain terms; "Approve all pending jobs (n)" | Jargon | Readiness `TerminologyAudit.md` | Comprehension | S | #1 | String sweep | NO (but cheap; do with #12) | A |
| 14 | Forest objectives visible from HUD/`[O]`; Year-25 horizon and "why" stated at start; why broadleaves must be planted (no seed source) | Count only; `[O]` opens learning list | Stall points S2, S3, S4 | Players know what they are working toward | S | #12 | Rendered check; MenuTutorial updated | **YES** | A |
| 15 | Readability V2: no information-bearing text below 13 px | Not done | Readiness `ReadabilityAudit.md` | Legibility at 1280×720 | XS | — | Static `.uss` check + rendered | NO (recommended) | A |
| 16 | Pre-purchase cash warning on nursery buttons | Missing (P3 shows it after purchase) | P3 "Known limitations" | Closes the last unwarned route into the dead end | XS | #1 | Fixture: buy to below €2,500 before first thinning → warning before the purchase | **YES** (otherwise the trap is still reachable unwarned) | A |

## 3. Private test build (packet S1-B)

| # | Item | Current state | Authority / evidence | Player impact | Size | Dependencies | Test requirement | Blocks external test? | Order |
|---|---|---|---|---|---|---|---|---|---|
| 17 | Reproducible standalone build (script/procedure, build SHA shown on screen) | Never built | `CurrentStateAudit.md` §11 | Testers can run it at all | S | Ultimate Nature present on the build machine | Build from a clean checkout; launches; ForestTest loads | **YES** | B |
| 18 | Visible save/load (Pause/Esc menu or HUD entries for Save, Load), quit, "Start a new forest" with confirmation | F5/F9 only; no quit; no new game | `ForestSaveController.cs:33–36` | Testers can stop, resume, restart after a dead end | S–M | #17 | Save → quit → relaunch → load equals saved world hash; new forest resets scenario | **YES** | B |
| 19 | Per-tester reset of learning/help preferences (menu action or launch flag) | Per device PlayerPrefs | Stall S14 | Each tester sees introductions | XS | #18 | Reset → all five first-use screens reappear | **YES** (for shared test machines) | B |
| 20 | Standalone Player profiling, Year 0 → 25 Scenario One (frame time, annual step, memory) | Editor-only evidence | D-045 (Editor), D-050 open items | Smoothness on testers' hardware | S | #17 | Recorded numbers on the reference machine; no hitch > 1 s on annual advance | **YES** (unknown, not necessarily failing) | B |
| 21 | Confirm Asset Store licence covers a private test build; record pack version/GUIDs used | Unrecorded | `UltimateNatureSetup.md` | None | XS (check) | — | Recorded in the build record | **YES** (legal precondition) | B |
| 22 | Playtest protocol updated for this build (session plan, consent, save backup, findings form) | PROPOSED on readiness branch | `BeginnerPlaytestProtocol.md` | — | XS (docs) | #17 | — | **YES** | B |

## 4. Review and history (packet S1-C)

| # | Item | Current state | Authority / evidence | Player impact | Size | Dependencies | Test requirement | Blocks external test? | Order |
|---|---|---|---|---|---|---|---|---|---|
| 23 | Annual Review v2 sections (WORK DONE · MONEY · FOREST · REGENERATION · DISTURBANCE · PLACES TO INSPECT) | PROPOSED | `AnnualReviewV2.md` | Each year points back into the forest | M | A integrated; H1 findings | Derivation equals saved data; identical across save/load; deny-list for causal/prescriptive wording | NO | C |
| 24 | Self-thinning deaths named under DISTURBANCE (cause from mortality records) | MISSING in UI | D-048 cause `self-thinning` | Explains fallen logs the player did not create | XS (inside #23) | #23 | Fixture year with deaths | NO | C |
| 25 | PLACES TO INSPECT (≤ 3, deterministic, Set waypoint) | MISSING | `AnnualReviewV2.md` §4 | The "observe" step gets a destination | S (inside #23) | #23; waypoint API | Deterministic order; arrival works | NO | C |
| 26 | Forest Diary v1 (whole-forest timeline) | PROPOSED | `ForestDiaryV1.md` | "What happened over 20 years?" | S | #23 | Timeline equals saved snapshots/reports/events; legacy saves render | NO | C |
| 27 | Stand Map cell history list | PROPOSED | `MapHistoryDesign.md` | "What happened here?" | S | #26 helper | Events for thinned cells listed | NO | C |
| 28 | Per-cause planted-loss line (light/browse/vegetation) in REGENERATION, current year only | Runtime accounts exist; review aggregates | Stall S10; D-049 separate accounts | Understand why saplings died | S | #23 | Fixture with each cause | NO | C |

## 5. Second cycle and completion (packet S1-D)

| # | Item | Current state | Authority / evidence | Player impact | Size | Dependencies | Test requirement | Blocks external test? | Order |
|---|---|---|---|---|---|---|---|---|---|
| 29 | "Forest has changed — look again" trigger (teaching only, reason string; never "thin now") | MISSING; PROPOSED | `SecondInterventionDesign.md` §2 | A reason to come back | S | C (PLACES) | Fires in Years 6–20 for T0–T5 plans under the current stack | NO | D |
| 30 | Completion = Year ≥ 25 + first release + second resolved cycle (any work type, ≥ 5 years later) + guardrails (cover every year, standing capital) + one deliberate broadleaf surviving 5 years | PROPOSED (model D) | `ScenarioCompletionDesign.md` §3 | Completion means repeated management | M | #8 decisions | New completion anchor (authorised); negative controls (unmanaged, one thinning, clear-fell, planting with no survival) fail with the named reason; two processes identical | NO | D |
| 31 | No hard fail: Year-100 "not completed" wording; remove/reword cash-0 failure; old `Failed` saves display correctly | PROPOSED (O1) | `FailureRecoveryDesign.md` §3 | No "Failed" for a recoverable learning path | S | #8 | Old saves with `Failed` load and display; anchor recorded | NO | D |
| 32 | Completion view (multidimensional, no total, "one way the forest can develop") | PROPOSED | `ScenarioCompletionDesign.md` §4 | A clear end state that is not a grade | S | #30 | Rendered; deny-list (score/grade/best) | NO | D |
| 33 | Re-verify lowest cash and completion year under the save-19 stack | Not recorded for v19 | Storm handoff (hash only) | Solvency claim | XS (inside #30) | — | Logged in the completion gate | NO | D |

## 6. Separate lanes (not UI packets)

| # | Item | Current state | Authority / evidence | Player impact | Size | Dependencies | Test requirement | Blocks external test? | Order |
|---|---|---|---|---|---|---|---|---|---|
| 34 | Mixed-species SDI mortality: diagnose whether promoted oak/beech die under the Sitka density line by Year 25–40 | CONFIRMED in code; impact unmeasured | `ForestEcologyController.cs:1010–1056`; P3 record | Planted broadleaves may die "from crowding" | S (diagnostic) → M (fix, if needed) | Ecology owner | Mixed-stand fixture; lifecycle anchors | NO (beginner); **YES for forester review** | Diagnostic before D; fix per ecology decision |
| 35 | Sapling price authority: document definition price as authority; adapter override | Inert conflict | P3 `SaplingPriceAuthorityAudit.md` | None now | XS | Economy owner | Economy gate (120), no cash change | NO | with D |
| 36 | Cash recovery route (standing sale / net settlement) | PROPOSED, decision O2 | `FailureRecoveryDesign.md` §3 | Recovery from the dead end | M (economy) | Decision; test evidence that players hit it | Economy anchors change | NO | later |
| 37 | Model 2 bramble/bracken visual recognition + human clearance playtest | Open (D-049) | Asset packet 03 | Can players see the competitor? | Human + possibly S | H1 | Recognition check inside H1 | NO (observe in H1) | H1 |
| 38 | Asset packets 01 (juvenile vs woody understorey) and 07 (mark contrast) | PROPOSED | `ScenarioOneAssets/CreationPackets` | Recognising young trees and marks | M (art) | — | Rendered review | NO (observe in H1; fix if testers fail) | after H1 |
