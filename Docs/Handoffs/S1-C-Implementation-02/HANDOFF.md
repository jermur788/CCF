# S1-C Implementation 02 — Annual Review year browsing: requalified handoff

**Verdict: IMPLEMENTED / REQUALIFIED — READY FOR MANAGER REVIEW.**

The final tooling and production bytes passed a full Unity process gate (exit 0, harness PASS, cleanup PASS), and the same-harness baseline comparison at `cf7eb61` is identical for every shared state. This closes **T-02 only**. No integration, full S1-C qualification, spec conformance, T-03+ work or acceptance is claimed.

## Identity and scope

- Task: `S1-C-IMPL-02`; claim issue #6 (`[RUNNING]` before any Unity step, closed `[HANDED-OFF]` at publication).
- Workers across the task: interrupted Claude implementation → T3 Code / Codex `gpt-6.1-sol` (implementation and first qualification attempts, stopped at an ownership collision) → **this requalification continuation: T3 Code / Claude Code, Claude Sonnet 5.5**, resumed from `REROUTE-HANDOFF.md`. The packet prefers DeepSeek and the reroute names the TTree Code local lane; the actual model is recorded here and on the claim.
- Branch/worktree: `feature/s1-c-history-ui-v1`, `/media/jer/ZX20/CCF-S1C-Impl02` (clean at resume, equal to `origin`).
- Dependency base: `808d8165d242da7632f37fea5c1d7355fa59279c`. Packet HEAD: `cf7eb613594d742dab778008e7f41156c0152eda`. Resume HEAD: `a23a34c7cc313edf82f41bb2ae0ee37d05345ec9`.
- Production implementation: `46370f6e4159da12d69f226fb63377723e63c5e8`.
- **Final tested HEAD: `0d3ac9208d4a36813ae0bef3d9064157a69511e6`.** `Assets/ForestPrototype/UI/AnnualReviewView.cs` is byte-identical (sha1 prefix `55c3fd91c14f`) at `46370f6`, `863d831`, `0d3ac92` and the resume HEAD. `863d831..0d3ac92` changed only the verification harness and driver.
- Published branch tip: this documentation commit, above `a23a34c`. It changes only this file; `0d3ac92..a23a34c` was this file alone. The commit SHA and the remote readback are in the worker response and `publication-requal.json` in the evidence archive, because a commit cannot name itself.
- Authority: `origin/main` `fe81994` at resume (live coordination row: *READY TO RESUME / QUALIFICATION INCOMPLETE*, no other writer on `AnnualReviewView.cs`), approved S1-C spec, T-02 packet, `REROUTE-HANDOFF.md`.
- External evidence archive: `/media/jer/Files/CCF/Verification/S1C-T02-YearBrowsing-20261027/`.

## Implementation (unchanged by this continuation)

Inherited Earlier/Later buttons, `viewYear == -1` follow-latest state, opening on latest, reset on a new completed year, the card hierarchy, compact trends, completion/intervention presentation and the historical live-state omissions were **adopted**.

Changed: `AnnualReviewView.Refresh`, `StepYear` and `FindYear` select completed years through `ScenarioOneManager.HistoryYears`. The selected `YearHistoryView` supplies the report, resolved event/order pairs, annual-report-authoritative money, snapshot and previous snapshot. `BuildWork`, `BuildMoney`, `BuildForest`, `BuildStorm` and the selected-year part of `BuildHistory` consume that view. A missing report or snapshot shows explicit absence, and an unknown remembered year falls back to latest. A cheap change key avoids rebuilding the history projection every frame.

Source audit repeated on the tested bytes: the only remaining references to `AnnualReports` / `ManagementEvents` / `EcologicalSnapshots` in the view are the change key (count comparison) and the inherited all-year trend strips. No selected-year join uses a raw list. `BuildStorm` still calls the existing `StormDamageInYear` after a `year.Storms` gate, and `BuildHistory` still lists `m.Interventions`; both are deliberate (T-03/T-04 own them).

The acknowledgement footer predicate is unchanged and verified identical at base and head:

```csharp
(ui.NeedsAnnualReview || m.NeedsCompletionReviewAcknowledgement || teachingReview) && !historical
```

The button callback additionally refuses while `showingEarlierYear` is true. `ScenarioOneUiRoot`, manager acknowledgement logic, the first-review advancement gate, completion/teaching semantics and persistence were not modified.

### Changed files on the task branch (since base `808d816`)

| File | Change |
| --- | --- |
| `Assets/ForestPrototype/UI/AnnualReviewView.cs` | Only production file: query-layer selection and cards, deterministic missing-record handling, hidden-callback guard. |
| `Tools/Verification/S1CManagementHistory/AnnualReviewYearBrowsingVerification.cs` | Disposable four-year UI fixture; navigation, save-state, gate, teaching-footer, defensive-record and capture checks; deferred Editor shutdown. |
| `Tools/Verification/S1CManagementHistory/run_t02_gate.py` | Private display/config driver, cleanup, socket-capable `/tmp/`, fail-closed exact task-lock/HEAD/clean-lane checks. |
| `Tools/Verification/S1CManagementHistory/compare_reviews.py` | Baseline text/gate comparison with fixture-hash, required-state and process-receipt checks. |
| this handoff | Evidence and boundaries. |

Scope check on the final tree: nothing under ProjectSettings, Packages, scenes, prefabs, ScriptableObjects, `.meta`, `Racks/`, `ForestSaveData` or Rack planning changed; the only non-doc, non-tooling path changed since the base is `AnnualReviewView.cs`. Save schema remains the inherited v21. `ScenarioManagementHistory.cs`, `ScenarioOneManager.History.cs`, `ScenarioInterventionHistory.cs` and the T-01 offline check are unchanged since `808d816` (empty `git diff`), so the T-01 offline check was not rerun; the retained T-01 evidence (`S1C_HISTORY_QUERY_PASS` 55 checks, `THREE_SPECIES_HISTORY_PASS` 74 checks) stands. No T-03–T-07 work, packages or binary assets were added.

## Verification on exact `0d3ac92` (and baseline `cf7eb61`)

| Run | Lane / lock | Result |
| --- | --- | --- |
| Candidate `0d3ac92` | U1 at `0d3ac92`; lock `f471c04ed595441ea540b3ef18fb8a49`, task `S1-C-IMPL-02` | Unity exit **0**, `S1C_T02_HARNESS_PASS mode=candidate checks=246`, 0 failures, 12 captures, 247 s, harness/.meta/licence copy removed, cleanup PASS, driver preflight PASS first |
| Baseline `cf7eb61`, same harness bytes, `--mode baseline` | U1 at `cf7eb61`; lock `9e5c7739a4014cebbb993983f89090c1`, task `S1-C-IMPL-02` | Unity exit **0**, `checks=212`, 0 failures, 12 captures, 167 s, cleanup PASS (the 34 extra candidate checks need the new behaviour) |
| `compare_reviews.py` | — | Fixture world hash `EBE48D41AAA615EB` in both; 16 shared review states identical (16/16); gate table identical; acknowledgement-footer rule line identical at base and head; **RESULT: IDENTICAL** |

Both runs emitted no `error CS`. The only `AnnualReviewView.cs` warning is the inherited `FindFirstObjectByType` obsolescence (CS0618), present in baseline and candidate. The only error-level line in the candidate log is the isolated config's Unity licensing token notice (`Access token is unavailable`). The earlier `863d831` signal-11 on exit did **not** recur: the deferred Edit-Mode exit completed with exit code 0. The candidate's 18 captured review texts are also byte-identical to the earlier `863d831` run.

### Packet proof table

| # | Required proof | Evidence (check IDs in `final-candidate/evidence/t02-result.txt`) |
| --- | --- | --- |
| 1 | At least three completed years exposed | `FIXTURE_HAS_AT_LEAST_THREE_COMPLETED_YEARS count=4`, `HISTORY_YEARS_OLDEST_FIRST_UNIQUE`, `ALL_COMPLETED_YEARS_VISITED 4,3,2,1` |
| 2 | Opens on latest | `OPENS_ON_LATEST_YEAR`, `REOPEN_RETURNS_TO_LATEST`, `LATEST_HAS_EARLIER_BUT_NO_LATER_CONTROL` |
| 3 | Earlier and back without changing simulation/save | `STEP_EARLIER_*`/`STEP_LATER_*`, `BACK_ON_LATEST_AFTER_STEPPING_BACK`, `LATEST_CONTENT_UNCHANGED_AFTER_ROUND_TRIP`, `SIM_AND_SAVE_STATE_UNCHANGED_AFTER_{BROWSING,CLOSE_REOPEN,OLDER_YEAR_GATE_PROBES,RENDER_PASS}` (hash `EBE48D41AAA615EB`, 27/27 events, 4/4 reports, year 4/4, `seen=False`) |
| 4 | Content sourced through `HistoryYears` / `YearHistoryView` | Source audit above; `YEAR_n_SHOWS_ITS_OWN_CLOSING_CASH`, `DEFENSIVE_NO_SNAPSHOT_DOES_NOT_BORROW_LATEST` |
| 5 | Older year has no acknowledgement action | `OLDER_YEAR_HAS_NO_ACK_FOOTER`, `ACK_FOOTER_HIDDEN_ON_YEAR_{1,2,3}`, `OLDER_YEAR_ACK_CALLBACK_DOES_NOTHING seen=False events=27/27` |
| 6 | Latest acknowledgement still gates exactly as before | `ADVANCE_BLOCKED_WHILE_REVIEW_UNACKNOWLEDGED_{OLDER_VIEW,LATEST_VIEW}`, `LATEST_ACK_MARKS_REVIEW_SEEN`, `LATEST_ACK_RECORDS_EXACTLY_ONE_ACKNOWLEDGED_EVENT events=28`, `ACK_CLEARS_BOTH_PENDING_FLAGS`, `ADVANCE_ALLOWED_AFTER_ACK`, `NO_RECURRING_REVIEW_GATE_AFTER_FIRST_ACK`; baseline run shows the same states |
| 7 | Completion-review / teaching-review not regressed | `COMPLETION_ACK_FOOTER_VISIBLE_ON_LATEST`, `COMPLETION_ACK_FOOTER_HIDDEN_ON_OLDER_YEAR`, `TEACHING_ONLY_FOOTER_ON_LATEST_{1,4,6}`, `TEACHING_ONLY_FOOTER_HIDDEN_ON_OLDER_{1,4,6}`, `TEACHING_OLDER_CALLBACK_DOES_NOT_ACK_{1,4,6}`, `TEACHING_LATEST_CALLBACK_RECORDS_ONE_ACK`; identical text/gate in baseline for all six teaching states and both completion states |
| 8 | No-work and work years render coherently | `WORK_YEAR_{1,3,4}_LISTS_ITS_WORK`, `NO_WORK_YEAR_2_SAYS_SO`, `FIXTURE_COVERS_BOTH_WORK_AND_NO_WORK_YEARS`; defensive: `DEFENSIVE_NO_REPORT_SAYS_SO`, `DEFENSIVE_NO_SNAPSHOT_SAYS_SO`, `DEFENSIVE_UNKNOWN_YEAR_FALLS_BACK_TO_LATEST`, `DEFENSIVE_CASES_RAISE_NO_ERRORS`; new-year reset: `NEW_COMPLETED_YEAR_RETURNS_VIEW_TO_LATEST` |
| 9 | Legible at 1280×720 and 1920×1080 | 12 captures; per-shot `CAPTURE_SIZE`, `SHOT_NOT_BLANK`, `TEXT_INSIDE_PANEL`, `TITLE_CLEAR_OF_HEADER_BUTTONS` all pass; **all 12 opened and inspected for this tested HEAD** (below) |
| 10 | Compile/import clean, no baseline weakened | 0 `error CS` in both runs; no hash rebaselined; baseline run uses the same harness bytes and fixture hash |

### Rendered review

Opened `final-candidate/evidence/shots/year{4-latest,3-earlier,2-earlier,1-earlier}-top` and `year4-latest-lower`, `year1-earlier-lower`, each at `-1280x720.png` and `-1920x1080.png`. At both sizes the three columns, header controls, scroll area and wrapped text are readable with no clipped controls. Year 4 (latest) shows `Later year` disabled and the acknowledgement footer. Years 1–3 show `(earlier year; latest is Year N)`, no footer, `Later year` enabled, and the Objectives card labelled `(current state)`. Year 2 reads "No work was resolved this year." with closing cash only; years 1 and 3 show their own timber, harvesting, small-job-minimum and shelter lines; years 1–3 omit the stock-on-hand and browsing/protection lines. The lower Year 1 capture shows trends and objectives, with the first completed year's `Earlier year` disabled.

One minor observation, **not changed**: the Objectives card on an older year still ends with the inherited hint "Read WORK DONE, MONEY and FOREST in Annual Review, then acknowledge the results to continue." while the footer is hidden. It is existing text, identical in the baseline run, and already marked `(current state)`; T-04/T-06 may want to reword it.

## Failures and operational incidents

Earlier attempts (kept for the record):

1. Attempt 1 stalled at licensing because TMPDIR was on exFAT (`candidate-a15f067-ATTEMPT1-TMPDIR-STALL`); attempt 2 timed out before the harness (`candidate-a15f067`).
2. Attempt 3's private display showed *Corrupted Library Detected*; Rebuild Library regenerated U1's cache, which then exposed the harness's missing boolean argument (fixed in `863d831`). Preserved in `candidate-a15f067-attempt3`.
3. On `863d831`: 246 checks and 12 captures passed, but native signal 11 followed the same-frame Play Mode / Editor exit (`MonoPathRemapper` / `__longjmp`). `10906c9` deferred the exit to Edit Mode via SessionState. **Qualified now:** the `0d3ac92` run exited 0.
4. Prior worker-caused lane collision (Rack V2 acquired U1 at `8763623` under lock `1195d0f3…` while this task's retry was briefly checked out at `10906c9`; its overlapping gate may have observed transient source). Evidence: `lane-collision.json`, `transient-checkout-changed-paths.txt`. The Rack V2 lane has since been re-gated on U1 under its own lock (live coordination records Rack core integrated at `f23549b`); T-02 holds no open action for that owner.

This continuation:

5. First launch of the stage script stopped cleanly at lock acquisition: the lock tool verifies the lane's actual HEAD, so the exact head must be placed first and the lock taken immediately after (my script had the order reversed). The message was `lane branch/HEAD mismatch; worktree was not changed`; U1 was unchanged at `8763623`. The script was corrected (and now undoes only its own placement if acquisition fails) and the stage reran. Both receipts are in `requal-chain-status.txt`.
6. Each run left the same four Editor-generated modifications in U1 (`.vscode/settings.json` and three `Woodland_Forbs_01` materials). They were archived byte-exact (`final-candidate/editor-side-effects.patch`, `baseline-cf7eb61/editor-side-effects.patch`) and restored, and U1 was clean afterwards. No routine shared-settings change is committed.

## Ownership, cleanup and preserved work

- Both locks above were taken normally and released with the matching owner/task/lock-id. No force-release was used, and the reaper loop never had to act (it logged nothing).
- U1 was restored to the other owner's `8763623c7b8a65c357e136a94ded117ff42492e3`, clean, with no Editor and no staged harness/.meta. The machine lock is free.
- The player's real `forest-save.json` last changed on 2026-10-17, so the isolated `XDG_CONFIG_HOME` kept it out of both runs.
- Primary's unrelated modified `AGENTS.md` / `active-tasks.md` and untracked files, and the Rack V2 worktrees, were not touched. `active-tasks.md` is Manager-maintained and was not edited.

## Not verified

- Teaching cases inject cached teaching facts to isolate the footer predicate; they do not prove real curriculum progression or Work Plan unlocks.
- Input was callback/API-driven in the Editor; OS-native pointer/key delivery was not exercised, and no player hand-test was done.
- No disk save/load qualification, Tier-3 or frozen-anchor rerun, built Player, or Windows build was run; this is a bounded presentation task.
- The T-01 offline check was not rerun (T-01 sources unchanged).
- No full S1-C feature, spec, or integration claim.

## Continuation and recommendation

**Recommend the Manager approve T-02 for continuation.** T-03 can proceed from this branch after approval; it should keep the selected-`YearHistoryView` boundary and add aggregate-first intervention detail on top (the `Interventions` list and `Read year N` buttons are still the inherited reconstruction). T-04 can reuse the missing-report/missing-snapshot wording and decide on the older-year Objectives hint above. T-07 must retain this exact candidate/baseline evidence, reconcile the remaining legacy reconstruction (`StormDamageInYear`, `Interventions`, all-year trend lists) and cover integrated regressions; it must not treat T-02 as full S1-C conformance.

### Reproduce

```bash
# lane at the exact head, lock held with task S1-C-IMPL-02, lane U1, matching head
python3 -B Tools/Verification/S1CManagementHistory/run_t02_gate.py --project /media/jer/ZX20/CCF-U1 --out <out> --mode candidate --lock-id <ID> --expected-head 0d3ac9208d4a36813ae0bef3d9064157a69511e6 --timeout 1500
python3 -B Tools/Verification/S1CManagementHistory/run_t02_gate.py --project /media/jer/ZX20/CCF-U1 --out <out-baseline> --mode baseline --lock-id <ID2> --expected-head cf7eb613594d742dab778008e7f41156c0152eda --timeout 1500
python3 -B Tools/Verification/S1CManagementHistory/compare_reviews.py --baseline <out-baseline> --candidate <out> --repo /media/jer/ZX20/CCF-S1C-Impl02 --base-rev cf7eb61 --head-rev 0d3ac92
```

The exact stage script used here is `requal-driver/stage.sh` in the evidence archive (place head, take lock, preflight, run, restore Editor side effects, release).

Manual review: open Annual Review on the four-year fixture and confirm Year 4 first; step back through 3, 2 and 1 and forward again; Year 2 says no work; older years hide the stock/browsing lines and the footer; reopen on latest and acknowledge. Advancement is blocked before acknowledgement and works afterwards with one acknowledgement event.

### Evidence index (under the archive root)

`final-candidate/` (`process-result.json`, `unity.log`, `evidence/{t02-result.txt,gate-states.tsv,review-texts/,shots/}`, `editor-side-effects.patch`); `baseline-cf7eb61/` (same layout); `compare-reviews.txt`; `requal-chain-status.txt`; `requal-unity-lock-{acquire,release}.txt`; `requal-driver/`; earlier attempts `candidate-*`; `offline/`; `qualification-revisions.json`; `lane-collision.json`.

**SPEC CONFORMANCE: NOT VERIFIED** (bounded T-02 behaviour verified; full S1-C spec not assessed). **Ready for review: Yes.**

**DASHBOARD SYNC: NOT ATTEMPTED by this continuation. PUBLISHED DASHBOARD REFLECTS THIS TASK: NO.** The published state is still the prior worker's BLOCKED entry (GitHub audit export, sequence 104, task `review-history-years`), which is now stale; the Manager or an authorised publisher needs to refresh it. Only the claim issue and this handoff were updated.

## Exit

`S1-C YEAR BROWSING IMPLEMENTED / REQUALIFIED — READY FOR MANAGER REVIEW`
