# Fable #402 — Scenario One cadence V1 end-to-end orchestration result

**Date:** 12 November 2026. **Parent claim:** [#402](https://github.com/jermur788/CCF/issues/402). **Return to:** Manager [#200](https://github.com/jermur788/CCF/issues/200).

**Verdict: APPROVED.** **Integration state: INTEGRATED into the new isolated branch only.**

- Integrated branch: `integration/scenario-one-cadence-v1-qualified-20261112-v1` @ `e759193354b4613f88e3370a62ccaccf02c556cf`, tree `00c736b933c09e3c25ac347d13657e5372821007`.
- This is the exact commit and tree that passed the Unity qualification in claim #408.
- Not merged to `main`. Not merged into the personal playtest candidate. No Windows or Player build. No Tier 3 run. Not user-accepted.

**Immediate Manager next step:** review this isolated integration evidence and decide whether the cadence feature goes forward to the user's personal playtest candidate. The disclosed coverage limits in section 6 are the items to weigh.

## 1. Who did what

| Role | Model / provider / effort | Claim | Result |
|---|---|---|---|
| Orchestrator, reviewer of child diffs, integrator | Claude Fable 5.2 (`claude-fable-5-2`), Claude Code harness in T3 Code, provider instance "Claude B — Pro", low effort | #402 | this document |
| Harness repair | GPT-6.1 Sol, medium, provider "Codex" | [#401](https://github.com/jermur788/CCF/issues/401) | HANDED-OFF, feature → `9ce4ab6b` |
| Unity qualification, attempt 1 | GPT-6.1 Sol, medium, "Codex" | [#403](https://github.com/jermur788/CCF/issues/403) | HANDED-OFF / BLOCKED (fixture) |
| Unity qualification, attempt 2 | GPT-6.1 Sol, medium, "Codex" | [#404](https://github.com/jermur788/CCF/issues/404) | HANDED-OFF / FAIL (F-8 reproduced) |
| Bounded production UI fix | GPT-6.1 Sol, medium, "Codex" | [#407](https://github.com/jermur788/CCF/issues/407) | HANDED-OFF, feature → `e7591933` |
| Full Unity requalification | GPT-6.1 Sol, medium; started on "Codex", cut off by that provider's usage limit at about 10:42 UTC; resumed on "ChatGPT – OpenAI B" at 10:47 UTC on the user's instruction | [#408](https://github.com/jermur788/CCF/issues/408) | HANDED-OFF, ALL PASS |

The orchestrator held no source worktree and never started Unity. Its coordinator checkout `/media/jer/Files/CCF-Primary` (`ops/workflow-foundation-v1` @ 46247486) was used read-only; its two pre-existing dirty files were left untouched.

One writer at a time: the feature branch was written only by #401 and then #407, both in the Files clone `/media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397`. The qualify branch was written only by the Unity workers in lane `ResearchCopyQual391`.

## 2. Commits

| Item | Commit |
|---|---|
| Accepted C-01 base (unchanged) | `a314d308315b5894263d30461a1e8b52ca0f3605` |
| Source as independently reviewed in #398 | `8c1efc74` |
| After harness repair #401 (harness commit `f2f3e136`) | `9ce4ab6b` — superseded, not qualified |
| After UI fix #407 (production commit `a8471a74`) | `e759193354b4613f88e3370a62ccaccf02c556cf` — **qualified and integrated** |
| Qualify branch tip (candidate + runner + reports) | `dcb319f9eb93b16d60a739bd907c65de1d4204e7` |
| `main` at integration time (not touched by any merge) | `861b61bb` |
| Personal playtest candidate (not touched) | `b62446ca` |

## 3. Sequence of events

1. **Phase 0 preflight.** Source clean at `8c1efc74`, C-01 base an ancestor. Unity lock FREE, no Editor. Lane `ResearchCopyQual391` clean at `a314d308` with its warm Library; prior holders #391 and #393 closed; no row for the lane on the Ops board.
2. **#401 harness repair.** Changed only `CadenceParity.cs`, `OfflineLogic/Check.cs`, the README and its report. Orchestrator diff review: `git diff 8c1efc74..9ce4ab6b -- Assets` empty; the new oracle strings match production `FinishSkip`/`SkipStopReason` text exactly. Accepted for Unity use.
3. **Lane records.** Ops board row `CCF-403-CADENCE-V1-UNITY-QUAL` added (lint PASS) and a matching entry prepended to live `active-tasks.md` on `main`, before any lock was taken.
4. **#403.** Candidate compiled in Unity. The worker's fixture world only reached lesson stage 4, so the skip correctly refused. No skip ran. BLOCKED on the fixture, not on the product.
5. **#404.** Genuine stage-7 world reached through real handlers. Quiet-state parity A and B passed 5/5. **F-8 reproduced twice:** after a skip to Year 6, loading a Year 0 world still showed "Session only: advanced Years 5–6" in the Annual Review. The orchestrator opened the screenshot and confirmed it.
6. **#407 production fix.** 15 changed lines in `AnnualReviewView.cs` and `WorkPlanView.cs`. See section 5.
7. **#408 full requalification** on `e7591933`. 24 Editor runs under 14 leases. Final result ALL PASS, 143 sub-cases.
8. **Integration** by fast-forward. See section 7.

## 4. Review findings F-1 to F-10: disposition

| Finding | Disposition | Evidence |
|---|---|---|
| F-1 early stop false green | Fixed in harness (#401): quiet run requires exactly `k == n` and the exact reassessment summary; injected stops take explicit expected k and reason. Exercised in Unity: ten parity runs logged `requested=N actual=N`. | `f2f3e136`; #408 `resume/runtime18/cadence.log` |
| F-2 stop reason and input locks | Fixed in harness: nine input types, Review screen, exact reason text, `RejectWrongStopReason`. Exercised in Unity for every stop case. | same |
| F-3 different parity entry states | Fixed in harness: both legs open the Work Plan and assert identical pre-tick JSON and hash. | same |
| F-4 five-tick cap cannot bind | Disclosed, no change. Real skips are at most 4 years; N=5 ran only as a harness stress override. Not a five-year game promise. | #408 timing and parity rows |
| F-5 surviving calculator mutants | Fixed offline: four mutants killed, 19 assertions. | #401 report |
| F-6 fault exits untested | Not written by #401. Covered by the #408 runner with test-side injection: exception in a tick, `AdvanceYear` false on tick 2, `OnDisable`, manager and ecology reference loss, each with nine-type restore. | #408 row 4d |
| F-7 silent refused confirm | Fixed in production (#407). Confirmed in Unity at both resolutions. | `a8471a74`; `1280x720-F7-refused-current-reason.png` |
| F-8 stale skip summary after load | Reproduced (#404), fixed in production (#407), confirmed fixed in Unity at both resolutions and after a same-year disk reload. | `a8471a74`; `*-loaded-world-hidden-summary.png` |
| F-9 Crop Tree mortality with marking disabled | Run in Unity: identical forced `ApplyMortality` on both legs, equal JSON and hash (`9F04788D7A569F07`), dead tree's mark gone after the stale sweep. Mortality was forced through an existing game API, not a natural death. | #408 `F9/crop-mortality` |
| F-10 Legacy40 eligibility | Run in Unity: the unmodified archived v12 Year 20 world computes lesson stage 0 and the skip is refused with the lesson reason. No stage-7 Legacy40 world exists, so the "eligible" side was not exercised on Legacy40. | #408 `compat/Legacy40` |

## 5. Production change made during this orchestration

Commit `a8471a74`, reviewed by the orchestrator (a different model family from the implementer). It has not had a separate full independent review.

- `AnnualReviewView.cs`: the session-only skip summary remembers the manager and its `TeachingWorldVersion` at the time it was set. On each refresh, if either differs, the summary is cleared. The empty label is hidden so it takes no layout space.
- `WorkPlanView.cs`: "Confirm skip" now redraws the Work Plan when `BeginSkip` returns false.
- `TeachingWorldVersion` already existed on `ScenarioOneManager`. It is session-only, excluded from the save schema, and increments only in `InitializeNewScenario` and `RestoreSaveData`, never on a yearly tick.
- No change to `ScenarioOneManager`, save data, hash layout, events, MFM, the cadence calculator, tick order, or any stop or summary text. The #408 worker confirmed the save-version and hash-layout source files are byte-identical to base `a314d308`.

Total production delta from the C-01 base to the integrated commit: six files under `Assets/` (new `ScenarioManagementCadence.cs` and its `.meta`, one accessor line in `ScenarioOneManager.cs`, `ScenarioOneUiRoot.cs`, `AnnualReviewView.cs`, `WorkPlanView.cs`).

## 6. Unity qualification result (#408) and what the orchestrator checked

Worker report: [SOL-CADENCE-V1-FIXED-CANDIDATE-UNITY-QUALIFICATION-RESULT.md](https://github.com/jermur788/CCF/blob/dcb319f9eb93b16d60a739bd907c65de1d4204e7/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-CADENCE-V1-FIXED-CANDIDATE-UNITY-QUALIFICATION-RESULT.md). Evidence: `/media/jer/Files/CCF/Verification/CadenceV1-FixedQual-408-20261112T095220Z/`.

| Row | PASS | FAIL | NOT RUN |
|---|---:|---:|---:|
| 1 Compile/import | 2 | 0 | 0 |
| 2 Parity A strict (quiet and post-felling, N=1..5) | 10 | 0 | 0 |
| 3 Parity B semantic (same) | 10 | 0 | 0 |
| 4 Input locks, entry blockers, stops, faults | 71 | 0 | 0 |
| 5 MFM and rack safety | 6 | 0 | 0 |
| 6 Compatibility | 14 | 0 | 0 |
| 7 UI at 1280×720 and 1920×1080, timings | 24 | 0 | 0 |
| 8 Safety | 6 | 0 | 0 |

Key figures from the worker report:

- Per-year `CurrentWorldHash`, direct versus skip, identical at every tick. Quiet: Y5 `D5C2997D4B56176F`, Y6 `8B0BDF75F9BC3816`, Y7 `1984FFF2B7579D27`, Y8 `26C98E82B0BE2D8C`, Y9 `F4516798470C4FB7`. Post-felling: Y6 `BC5F18E0283EDF2B`, Y7 `7B2A9B73FA119189`, Y8 `FFB99D03EF9B29D8`, Y9 `5B4DB7305AF51AC2`, Y10 `3180A80F8EC4D3F9`. All 60 records are in the worker report.
- Frozen Reference Future v1: embedded anchors Y0 `A564039D9B7CE31D`, Y20 `F7DF7DAB53B6FD32`, Y50 `D5E75D6D21D631AC`, Y100 `7AD177B3CC2F73C7`, all pinned; archive file identical by SHA-256 to base `a314d308`; current-layout projections equal the C-01 values recorded in #393.
- Timing in the Editor: normal 4-year skip 6.56 s; 5-tick stress 8.26 s.
- Year 24→25 completion inside a skip: direct and skip equal (`DC5E883C1BC6DC6E`), outcome Completed.

**Checked directly by the orchestrator, not taken from the worker's summary:**

- `Assets` on the tested qualify tip `dcb319f9` is identical to candidate `e7591933` (empty diff). `CadenceParity.cs` on the qualify branch is identical to the feature branch.
- The parity runs used a copy, `Cadence408Parity.cs`. Its diff against `CadenceParity.cs` is the class name plus two added `Debug.Log` lines that print the hashes. No assertion differs.
- Core run log `resume/runtime18/cadence.log`: 72 `CADENCE408_CASE` markers, all PASS; no line matching a standalone `FAIL`, `error CS` or `NOT_RUN`; ten `CADENCE PARITY A+B PASS requested=N actual=N` lines; final `CADENCE403_PASS checks=418`.
- The driver's own `result.json` for that run says FAIL with `fail_lines: 1`. The one line is the positive diagnostic `CADENCE408_FAILED condition=…`, matched by a substring search. The orchestrator's independent grep agrees with the worker's reclassification.
- Stage-7 fixture construction read in the runner source: racks, entrance marks, Crop Trees, release, the real `InspectTree` / `ObserveRackInspection` / `DecideRackInspection` handlers, planting and establishment. No lesson field is written.
- Four captures opened: destination summary (720p), loaded-world with summary gone (1080p), F-7 refused confirm (720p), progress overlay with Cancel (1080p). All rendered and showing what they claim.
- Lock history: every lease for #403, #404 and #408 shows a normal, unforced release. Lock FREE and no Editor process at the end. Lane clean on the qualify branch.

**Coverage limits disclosed by the worker and accepted with this verdict. Manager should weigh these:**

1. Several entry blockers and stop conditions were produced by test-side injection into a disposable world, not by player action: reference-preview flag, rack-tool active/intro flags, consent/reset/reopen/drag/ghost flags, advance warning, zero or negative cash, missing references, forced storm, forced mortality, a replanting order resolved at the current year. Each is labelled as an injection in the worker report.
2. The "Review boundary reached" stop at Year 25 was isolated by setting the UI's private `skipObjectives` observation counter, because a genuine Year 24 world also achieves an objective or completes at 25. The genuine Year 24→25 case (reason "Scenario outcome changed", Completed) and the Year 99→100 boundary both ran without that override.
3. Rack-plan and rack-approval blockers cannot occur at lesson stage 7 in this build; an earlier guard always wins. They were shown present in genuine earlier-stage worlds, where the lesson guard refuses the skip. Those two branches of `SkipProblem` were therefore not executed as the deciding reason.
4. Legacy40 computes stage 0, so only refusal was shown for it.
5. UI evidence is from the Editor Game view. Native F10 delivery needed a private GTK setting in the run's own config folder, because the Linux Editor otherwise takes F10 for its menu. No Player build was made or tested.
6. The Editor Search-indexing `ArgumentOutOfRangeException` appears in every run. The same stack is in Unity logs from 16 October onward on other candidates; it is not attributed to this feature.
7. Tier 3 integrated milestone tests were not run, by design.

**Orchestrator observations, not blocking:**

- The skip progress overlay renders as a full-size, mostly empty modal with one line and a wide Cancel button. It works, but it is a candidate for a presentation pass.
- After a skip, the Annual Review's sub-heading still reads "Read and acknowledge the Annual Review: open the Work Plan [Tab]." alongside the new summary. This wording predates the feature.

## 7. Integration

Preconditions checked at 12:14 UTC: feature branch still `e7591933`; C-01 base still `a314d308`; target branch absent; base is an ancestor of the candidate; eight commits in range, no merge commits; Unity lock FREE; #400 and #406 closed and never touched these files; #382 untouched.

Method: fast-forward only, no merge commit, no force.

1. `git push origin a314d308…:refs/heads/integration/scenario-one-cadence-v1-qualified-20261112-v1` (new branch at the accepted base).
2. `git push origin e7591933…:refs/heads/integration/scenario-one-cadence-v1-qualified-20261112-v1` (plain push; accepted as `a314d308..e7591933`).

Result:

- Integration head `e759193354b4613f88e3370a62ccaccf02c556cf`, tree `00c736b933c09e3c25ac347d13657e5372821007`: the same commit and tree as the qualified candidate.
- GitHub compare against the C-01 base: ahead 8, behind 0, 13 files. Against the feature branch: identical.
- Diff of `Assets`, `ProjectSettings` and `Packages` between the integration head and the Unity-tested qualify tip: empty.
- C-01 base branch, `main` and the personal playtest candidate were not changed by this integration.
- Post-merge smoke check: the integration head is byte-identical to the tested commit, so no new Unity run was needed; the offline calculator on that commit passes 19 assertions, exit 0.

The runner and qualification reports stay on the qualify branch and are not part of the integration branch.

## 8. Custody, locks, lane, disk

- **Lane assignment.** `ResearchCopyQual391` was reassigned for this work under the packet's conditional disposition after confirming prior holders #391/#393 closed, last lock released normally, lane clean, lock FREE, no Editor. Recorded on the Ops board and in `active-tasks.md` before the first lease. The lane now sits clean on the qualify branch at `dcb319f9`; the assignment is ended (see section 9).
- **Leases.** 16 in total: one for #403, one for #404, 14 for #408. All normal releases after Editor exit.
- **One exception to "released by the worker".** Lease `9b90eca38be24ffdb963fcb617721415` was still held when the first #408 worker was cut off by its provider limit. The orchestrator confirmed no Editor process at 10:43:37Z and released it normally at 10:43:44Z using the lease's own owner, task and lock id, with the reason recorded in the lock history. It was not a forced release. The lock-history "actor" field therefore reads "GPT-6.1 Sol Medium" for a release the orchestrator performed.
- **Host saves and Prefs.** The same 37 files hashed before #403 and after #408: unchanged.
- **Protected items.** `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` (#382), U1, U2, U3, the #400 branch, `main` source, the C-01 base and the frozen archives were not touched.
- **Editor drift.** Each Editor run dirtied `.vscode/settings.json` and three Forb materials in the lane. The workers saved a patch each time and restored the files from the candidate; none was committed.
- **Disk.** ZX20 free space was 20.4 GB through 10:10 UTC and 72.75 GB from 10:28 UTC onward. That change was not made by this orchestration or its workers; its cause was not investigated. The 15 GiB floor held throughout the sampled points.
- **Writes to `main`.** Coordination entries only, each a single prepended entry to `Docs/Project/coordination/active-tasks.md` through the Contents API with a blob-SHA precondition. One write was correctly refused when the Manager had written in between, and was redone on the new content.

## 9. Status words

| Status | Value |
|---|---|
| IMPLEMENTED | Yes, `e7591933` |
| REVIEWED | Independent source review #398 at `8c1efc74`. Later changes reviewed by the orchestrator only: harness `f2f3e136`, production `a8471a74`. |
| VERIFIED | Tier 1/Tier 2 feature qualification in the Unity Editor on the exact commit, with the limits in section 6. Not Tier 3. Not a Player build. |
| INTEGRATED | Yes, into `integration/scenario-one-cadence-v1-qualified-20261112-v1` only |
| ACCEPTED | No. The user has not playtested it and the Manager has not ruled. |

## 10. Deferred and open

- D-074 spaced major-intervention learning objective: not part of this feature; its spec remains DRAFT.
- D-076 repeated representative rack sampling: out of scope.
- A separate independent review of `a8471a74`, if the Manager wants one before wider integration.
- The presentation observations in section 6.
- Overall-Dev dashboard source, published dashboard and Drive mirrors: see the closing comment on #402 for what was and was not updated; downstream rendering is UNVERIFIED.

## 11. Links

- Orchestration packet: [MANAGER-FABLE-CADENCE-V1-END-TO-END-ORCHESTRATION-PACKET.md](https://github.com/jermur788/CCF/blob/docs/manager-cadence-review-disposition-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-FABLE-CADENCE-V1-END-TO-END-ORCHESTRATION-PACKET.md)
- #401 result: [SOL-CADENCE-V1-VERIFICATION-HARNESS-FIX-RESULT.md](https://github.com/jermur788/CCF/blob/e759193354b4613f88e3370a62ccaccf02c556cf/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-CADENCE-V1-VERIFICATION-HARNESS-FIX-RESULT.md)
- #403 result: [SOL-CADENCE-V1-EXACT-UNITY-QUALIFICATION-RESULT.md](https://github.com/jermur788/CCF/blob/dcb319f9eb93b16d60a739bd907c65de1d4204e7/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-CADENCE-V1-EXACT-UNITY-QUALIFICATION-RESULT.md)
- #404 result: [SOL-CADENCE-V1-EXACT-UNITY-REQUALIFICATION-RESULT.md](https://github.com/jermur788/CCF/blob/dcb319f9eb93b16d60a739bd907c65de1d4204e7/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-CADENCE-V1-EXACT-UNITY-REQUALIFICATION-RESULT.md)
- #407 result: [SOL-CADENCE-V1-F7-F8-UI-FIX-RESULT.md](https://github.com/jermur788/CCF/blob/e759193354b4613f88e3370a62ccaccf02c556cf/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-CADENCE-V1-F7-F8-UI-FIX-RESULT.md)
- #408 result: linked in section 6.
- Orchestrator evidence (board copies, lock and merge receipts): `/media/jer/Files/CCF/Verification/CadenceV1-Fable402-*/`
