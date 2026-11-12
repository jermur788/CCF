# Cadence V1 — independent source review

**Verdict: NEEDS CHANGES (narrow, verification-harness only).** I found no blocking defect in the production source. On static reading, the production diff meets the corrected approved spec and the Manager packet, and it is fit to enter the separately authorised Unity qualification. But the runtime harness `Tools/Verification/CadenceDualAdvance/CadenceParity.cs` would not, as written, prove everything its README claims. A green run could hide a stop-condition regression (F-1, F-2). Fix the harness before Manager treats a qualification run that uses it as P-A/P-B/P-D evidence.

- **Target:** `feature/scenario-one-cadence-dual-advance-v1-20261112` @ `8c1efc744722943f5119d358e5d836b372e5e454` (base `a314d308`). Author: GPT-6.1 Sol Medium.
- **Reviewer:** Claude Opus 5.5 (`claude-opus-5-5`), reasoning effort **low**, Claude Code in T3 Code. This is a different model family from the author. I did it myself, with no subagents. Claim: [#398](https://github.com/jermur788/CCF/issues/398). Date: 2026-11-12.
- **Inputs read:** Manager packet and corrected four-file spec on `docs/manager-cadence-review-disposition-20261112-v1` @ `194ce84f`; Manager #396 disposition; Sol handoff `SOL-CADENCE-V1-SOURCE-IMPLEMENTATION-RESULT.md` on the target; prior review (#396) on `docs/claude-b-cadence-time-advance-review-20261112-v1`; AGENTS.md.
- **Method:** `git fetch` plus `git show`/`git diff` from `/media/jer/Files/CCF-Primary`, with no checkout. Offline checks ran on a `git archive` export in a scratch directory. **No Unity, Editor, save load, or runtime execution.** No production file was edited. I did not touch any Unity lane or the parallel Sol `qualify/` work.

## 1. Contract checklist (static)

| Requirement | Result | Evidence |
|---|---|---|
| One-year route unchanged | PASS | `AdvanceFromWorkPlan` only gains `review.SetSkipSummary("")` (`UI/ScenarioOneUiRoot.cs:783`). The advance button, warning and confirmation in `WorkPlanView` are untouched. |
| 4-year target / 5-tick cap / Year-25 / Year-100 | PASS (see F-4) | `ScenarioOne/ScenarioManagementCadence.cs:30-33` follows spec §3 exactly: latest unique, validated `Succeeded` `WorkResolved` linked to a `Completed` `FellTree`; `candidate<=current` → `current+4`; `min(candidate, current+5, ReviewYear)`; clamp to 25 when `current<25`. |
| Stop: outcome change | PASS | `SkipStopReason` `UI/ScenarioOneUiRoot.cs:182-191` |
| Stop: objective achievement count | PASS | `ObjectiveAchievements.Count > skipObjectives` |
| Stop: storm | PASS | `StormEvents.Count` growth, or `LastStormPerformance.Victims>0` after the first tick (`:187`). `LastStormPerformance` is reset every year in `ResolveStormForYear` and on load, so a stale pre-skip storm cannot trigger a stop. |
| Stop: lesson change, Year 25, Year 100, target | PASS | `:188-190` |
| Stop: `AdvanceYear` false / exception | PASS | `TickSkip` `:193-208`. Failure feedback comes from `manager.Feedback`. Years already completed are kept, with no rollback claimed. |
| Cancel only between ticks | PASS | The `cancelSkip` flag is checked only before each tick. One `AdvanceYear` per frame is guarded by `skipFrame` (`:195-196`). |
| Pre-checks: stage 7, no Pending/Approved (incl. invalid/MFM), unread review, rack problem/approval, preview, reference authoring, Work Plan open, modals/help/session menu/rack editor, controller, `CanAdvanceYearNow`/`CanEditWorkPlan` | PASS | `SkipProblem` `:108-129`. It is re-run in `BeginSkip` together with a target re-match (`:134`), and re-run between ticks with `betweenTicks=true`, which ignores only the skip's own modal and the expected unread intermediate review. |
| Never calls `MarkAnnualReviewSeen`; never creates/approves/imports work; never touches MFM prescriptions | PASS | `MarkAnnualReviewSeen` is called only from the existing `AcknowledgeAnnualReview` (`:763`). The diff contains no `ApprovePendingWork`, `OpenWorkPlan(true)` or MFM calls. `ShowReview` only closes the Work Plan and sets the screen. |
| Save/load + input locks restored on every exit | PASS (static) | Hotkey owners (`ForestSaveController` F5/F9 live in `Update`), `StandaloneSessionMenu`, player, marking, rack tool, construction and asset review are disabled, keeping each one's prior enabled state (`:143-149`). Manager `ModalOpen` blocks Tab/Esc. Root children are disabled. Restore happens on target, stop, cancel, failure, exception (`:207`), `BeginSkip` exception (`:158`), `OnDisable` (`:210`) and controller loss (`SkipStopReason` null branch). `UiRoot.Update` returns early during a skip (`:315`). Ecology T-key time-lapse is already inert under annual control. |
| `ScenarioOneManager` diff only the authorised line | PASS | Single added line: `public bool ReferenceAuthoringActive => referenceAuthoring;` |
| No completion-rule / save / HashLayout / scene / prefab / ProjectSettings / Packages / `.vscode` change | PASS | `git diff --stat a314d308 8c1efc74` over those paths is empty. The 11 changed files are listed in the handoff. |

## 2. Offline checks I ran (scratch export of `8c1efc74`)

1. `python3 Tools/Verification/CadenceDualAdvance/run_offline.py` → **PASS, 15 assertions** (`CADENCE CALCULATOR PASS`).
2. `run_typecheck.py --mode prod --replace Assets/ForestPrototype/UI/Map/StandMapSource.cs <compat>`. My own compat copy changes only `SceneHandle`→`int`, and is byte-identical in effect to Sol's `StandMapSource.compat.cs` (3-line diff). Result: **TYPECHECK_CLEAN**, 6 distinct errors = baseline 6.
3. **Sensitivity check:** changing `manager.ReferenceAuthoringActive` to `...ActiveX` in a replaced `ScenarioOneUiRoot.cs` gave **TYPECHECK_NEW_ERRORS** (CS1061). So the typecheck really compiles the new UI code.
4. **Harness binding:** with the `CadenceParity.cs` body (Editor guard stripped) appended to the cadence file via `--replace`, the result was **TYPECHECK_CLEAN** (6 = baseline). The harness API calls bind. This is not Unity compilation.
5. **Calculator mutants** (run_offline.py as the oracle): Year-25 clamp removed → killed. **Survived:** `item.year != order.resolvedYear` disabled; `eventId <= 0` guard removed; empty `targetTreeId` guard removed; `resolvedYear > current` guard removed; 5-tick cap removed (see F-4).

## 3. Findings (ranked)

**F-1 — MEDIUM, harness can false-green on spurious early stops.** `CadenceParity.cs:59` accepts any `0 < k <= n`. In quiet fixtures nothing asserts `k == n`, and nothing asserts the stop reason. Suppose a regression made a guard fire after the first tick, such as a stale storm-victim check, or `SkipProblem(true)` failing between ticks. Then all ten `Run` cases would pass, logging `actual=1`, and both A and B would compare only one year. The README asks a human to "confirm that every expected quiet run actually reaches N". That is a manual inspection, not a test. *Fix:* add an `expectedK`/`expectFull` parameter, and expose the last stop reason (for example a read-only `LastSkipStopReason`) so the test can assert both.

**F-2 — MEDIUM, `RunStop` does not prove which guard fired.** `CadenceParity.cs:95-110` uses `reason` only in failure messages. It asserts "no further tick" and "inputs restored", so if an injection trips another guard (or the controller-unavailable branch), the guard under test can be deleted and the test still passes. That defeats the "effective negative mutation" goal in the packet. It also snapshots only 5 of the 9 locked component types (`:97-99` vs `ScenarioOneUiRoot.cs:145-147`), and never checks that the destination review opened with the summary. *Fix:* assert the exact stop reason, snapshot all locked types, and assert `screen==Review` and the summary text.

**F-3 — LOW-MEDIUM, parity-A legs do not start from the same in-memory state.** The skip leg calls `ui.ShowWorkPlan(); ui.Learning.Refresh(true)` before ticking (`:43`), but the direct leg does not (`:38-41`). Opening the plan runs `AddMarkedTreesToWorkPlan(false)` and panel-state code. If that ever changes saved state, A fails falsely (red, not green). It also means A does not strictly compare from one identical pre-tick state as spec §6 requires. *Fix:* open the Work Plan (or capture the post-open state as the baseline) in both legs.

**F-4 — LOW (spec-level, disclose), the 5-tick cap can never bind in production.** A qualifying felling year `q` satisfies `q <= current` (`ScenarioManagementCadence.cs:19` filter). So `candidate <= current+4 < current+5`, and every real skip is at most **4** years. The `MaxTicksPerSkip` term is dead in practice; the cap5 mutant survives. This faithfully implements spec §3 and is not a code defect. But "five-tick cap" and N=5 parity are reachable only through the harness's reflection override of `skipTarget` (`CadenceParity.cs:47`). Manager should note this in the product status, or the spec should change if a 5-year case was intended.

**F-5 — LOW, offline calculator coverage gaps.** Four validation guards in `ScenarioManagementCadence.cs:17-27` survive mutation (year mismatch, `eventId<=0`, empty target tree, future `resolvedYear`). Add one assertion each to `OfflineLogic/Check.cs`.

**F-6 — LOW, untested exit paths.** No harness case drives the `TickSkip` exception catch (`ScenarioOneUiRoot.cs:207`), `OnDisable` (`:210`) or controller loss through to a restore assertion. All three are left as manual README items. The code is correct on reading, but the packet's "restored on every exit" claim is not tested.

**F-7 — LOW, UX: stale skip state in the Work Plan footer.** `WorkPlanView.Refresh` is gated by a signature (`WorkPlanView.cs:125`) that excludes most `SkipProblem` inputs (help/modal/rack tool/session menu). So the skip button's enabled state and reason can lag. Safety is preserved because `BeginSkip` re-validates. But if `BeginSkip` returns false after Confirm (`:186`), nothing is shown and the old confirmation text stays until the next refresh. *Fix:* call `Refresh(true)` when `BeginSkip` fails.

**F-8 — LOW, UX: the summary label is always present and is never cleared on load.** `AnnualReviewView.cs:50` adds an empty `body` Label at the top of every Annual Review modal, which may shift the layout or screenshot baselines even when no skip happened. The text is cleared only by the one-year route (`ScenarioOneUiRoot.cs:783`), so after loading another save the old "advanced Years X–Y" summary can appear against the wrong world. *Fix:* set `display:none` when the text is empty, and clear it on load/session reset.

**F-9 — INFO, marking manager unsubscribed during ticks.** Disabling `ForestTreeMarkingManager` removes its `ForestTree.Felled`/`MortalityApplied` handlers (`ForestTreeMarkingManager.cs:94-98`), so Crop Trees that die in skipped years are not `Unmark`ed through the event. This has no save effect, because `cropTreeIds` is computed live and markers are children of the hidden tree, and the 0.5 s stale sweep repairs the counts. It is still a behaviour difference from the one-year path. Include a Crop Tree mortality year in the Unity parity-A fixture.

**F-10 — INFO, Legacy40 fails closed only if its facts compute below 7.** Eligibility relies on `ScenarioOneTeachingSequence.Current(facts)==7`. Whether the authentic Legacy40 world computes 7 "genuinely" is not provable statically. It needs the README's Unity fixture.

## 4. What remains unproven

Runtime parity A (byte-identical JSON + `CurrentWorldHash` per tick), parity B (ack-normalised semantics), every stop/negative fixture, input-lock behaviour, 720p/1080p UI, save/reload and Legacy40/Reference Future compatibility are all **NOT RUN** by Sol or by me. Sol's handoff discloses this accurately. Nothing here is a Unity PASS.

## 5. Recommended disposition

Manager: keep the production source as the qualification candidate. Before relying on a `CadenceParity` run as evidence, have the source owner fix F-1 and F-2 (assert k and the stop reason), plus F-3. Then add F-5 and F-6 if cheap. Disclose F-4 in the status. F-7 and F-8 are small UI follow-ups that do not block qualification. No production re-review is needed for harness-only changes, but the harness diff should be inspected.
