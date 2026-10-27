# S1-C Implementation 02 — incomplete qualification handoff

**Verdict: OWNERSHIP CONFLICT — IMPLEMENTED, T-02 REQUALIFICATION INCOMPLETE.**

Do not issue the ready-for-Manager-review exit token yet. The behavioural harness passed, but its Editor shutdown crashed; the repaired shutdown has not been qualified, and the baseline comparison has not run. No integration, full S1-C qualification, spec verification or acceptance is claimed.

## Identity and scope

- Task: `S1-C-IMPL-02`; primary worker continuation in T3 Code / Codex `gpt-6.1-sol`, following the interrupted Claude implementation.
- Branch/worktree: `feature/s1-c-history-ui-v1`, `/media/jer/ZX20/CCF-S1C-Impl02`.
- Dependency base: `808d8165d242da7632f37fea5c1d7355fa59279c`.
- Actual starting packet HEAD: `cf7eb613594d742dab778008e7f41156c0152eda`.
- Context: `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`, the current canonical-context commit on inspected `origin/main`; the feature lineage itself retains older context files. Task authority is the approved S1-C spec, T-02 packet and reroute handoff on main. Live coordination was checked separately; it assigns this task and Rack V2 distinct file ownership.
- Production implementation: `46370f6e4159da12d69f226fb63377723e63c5e8`.
- Behaviourally tested HEAD: `863d8317a385353cc6694a8f08f54d83dc4f567f`.
- Final implementation/tooling HEAD: `0d3ac9208d4a36813ae0bef3d9064157a69511e6`. Production is byte-identical to the tested HEAD; later changes repair harness shutdown and enforce launcher ownership checks. **Those later changes have not had a successful Unity run.**
- GitHub handoff path: `Docs/Handoffs/S1-C-Implementation-02/HANDOFF.md` on this branch. The documentation commit and remote readback SHA are recorded in the external publication receipt and worker response.
- External evidence archive: `/media/jer/Files/CCF/Verification/S1C-T02-YearBrowsing-20261027/`.

## Implementation

The inherited Earlier/Later buttons, `viewYear == -1` follow-latest state, opening on latest, reset on a new completed year, card hierarchy, compact trends, completion/intervention presentation and historical live-state omissions were adopted.

`AnnualReviewView.Refresh`, `StepYear` and `FindYear` now select completed years through `ScenarioOneManager.HistoryYears`. The selected `YearHistoryView` supplies the report, resolved event/order pairs, annual-report-authoritative money, snapshots and previous snapshot. `BuildWork`, `BuildMoney`, `BuildForest`, `BuildStorm` and the selected-year part of `BuildHistory` consume that view. Missing reports/snapshots show explicit absence; an unknown remembered year falls back to latest. The cheap refresh key avoids rebuilding the history projection every frame.

The existing higher-level `Interventions`, completion and `StormDamageInYear` reconstruction remain. The storm card is selected using the year's `Storms`; its existing victim/deadwood/salvage reconstruction is preserved. Full provenance wording belongs to T-04. The three remaining raw-list references are change-key counts and the inherited all-year trend strip, not selected-year joins.

The acknowledgement footer predicate is unchanged:

```csharp
(ui.NeedsAnnualReview || m.NeedsCompletionReviewAcknowledgement || teachingReview) && !historical
```

The button callback additionally refuses while `showingEarlierYear` is true. `ScenarioOneUiRoot`, manager acknowledgement logic, first-review advancement gate, completion/teaching semantics and persistence were not modified. Historical objectives remain explicitly labelled current state.

### Changed files

| File | Change |
| --- | --- |
| `Assets/ForestPrototype/UI/AnnualReviewView.cs` | Only production file changed: query-layer selection/cards, deterministic missing-record handling and hidden-callback guard. |
| `Tools/Verification/S1CManagementHistory/AnnualReviewYearBrowsingVerification.cs` | Disposable four-year UI fixture, navigation/save-state/gate checks, teaching-only footer fixtures, missing-record checks and captures; later deferred Editor shutdown repair. |
| `Tools/Verification/S1CManagementHistory/run_t02_gate.py` | Private display/config driver, cleanup, socket-capable `/tmp/` with trailing slash, and fail-closed exact task-lock/HEAD/clean-lane checks. |
| `Tools/Verification/S1CManagementHistory/compare_reviews.py` | Baseline text/gate comparison, identical fixture-hash requirement, required-state and successful-process-receipt checks. |
| This handoff | Evidence, incidents and continuation boundary. |

No scene/prefab/ScriptableObject, package, ProjectSettings, pre-existing metadata, ecology, economy, curriculum, objective persistence, Rack V2 production file or frozen Reference Future content changed on this task branch. Save schema remains the inherited v21. T-01 production sources are unchanged. No T-03–T-07 implementation, dependency or binary asset was added.

## Verification obtained

On exact `863d831`, Unity 6000.6.0f1 emitted **246 checks passed, 0 failed, 12 captures**, with no C# compile errors. This is behavioural/render evidence, **not a successful full process gate**, because native shutdown subsequently crashed.

| Required proof | Observed evidence |
| --- | --- |
| At least three completed years | Real fixture completes years 1–4; all four reached in both directions. |
| Latest default/reopen/new year | Latest on opening/reopening; a new completed year resets an older selection to latest. |
| Browsing is read-only | World/save hash `EBE48D41AAA615EB`, 27 events, four reports, cash and year unchanged through browsing, reopen, gate probes and rendering. |
| Selected-year source | Source audit confirms `HistoryYears` / `YearHistoryView`; T-01 sources unchanged. |
| Older-year acknowledgement | Footer hidden; invoking its registered callback leaves acknowledgement/events unchanged. |
| Latest gate | UI advancement blocked before acknowledgement from both older/latest views; latest callback records exactly one acknowledgement and advancement then succeeds. |
| Completion/teaching review | Completion-only footer latest/older checks pass. Teaching-only fixtures at boundaries 1/4/6 show footer only on latest, reject older callbacks, preserve save state, and route the latest acknowledgement normally. |
| Work/no-work | Years 1/3 thinning, year 2 no work and year 4 planting-only render coherently. |
| Defensive records | Missing annual report/snapshot and unknown selected year pass without errors or borrowing live/latest history. |
| Rendered legibility | All 12 captures inspected at 1280×720 and 1920×1080: readable cards, wrapping, navigation, footer and scroll access. A suspected stale heading was correct on reinspection; no production/render fix was made for it. |

Teaching cases deliberately inject cached teaching facts to isolate the UI footer predicate. They do not prove real curriculum progression or Work Plan unlocks, and never call the APIs that correctly invalidate such facts. Callback/API driving does not prove OS-native mouse/key delivery. No disk save/load qualification or Tier-3/frozen-anchor rerun is claimed for this bounded presentation task.

Retained offline evidence: `offline/s1c-history-query-offline.log` (**55 checks PASS**) and `offline/threespecies-history-offline.log` (**74 checks PASS**). The old broad offline typecheck logs have stub/tooling failures and are not clean compile receipts; the real Unity compilation supersedes their limited type-check evidence. No baseline was weakened or hashes rebaselined.

Rendered evidence: `candidate-7c/evidence/shots/` (the folder name is incidental; its exact tested revision is `863d831`). Files include `year4-latest-{top,lower}`, `year{1,2,3}-earlier-top` and `year1-earlier-lower`, each with `-1280x720.png` / `-1920x1080.png`. Texts, `gate-states.tsv` and `t02-result.txt` are beside the shots. Revision/hash metadata is in `qualification-revisions.json`.

## Failures and operational incidents

1. Inherited attempt 1 stalled at licensing because TMPDIR was on exFAT; preserved in `candidate-a15f067-ATTEMPT1-TMPDIR-STALL`. Attempt 2 licensed but timed out before the harness; preserved in `candidate-a15f067` (process receipt FAIL).
2. Inspection of attempt 3's private display found **Corrupted Library Detected**. Unity's Rebuild Library action regenerated U1's cache. That run then exposed the harness's missing boolean argument at its acknowledgement call, fixed in `863d831`. Attempt 3 remains in `candidate-a15f067-attempt3`.
3. On `863d831`, all 246 behavioural checks and 12 captures passed, but same-frame Play Mode exit / Editor exit was followed by native signal 11 (`MonoPathRemapper` / `__longjmp`). The driver received SIGTERM and its receipt is FAIL despite the harness PASS marker. `10906c9` changes shutdown to wait for Edit Mode across domain reload using SessionState; its effectiveness remains **UNVERIFIED**.
4. **Worker-caused lane collision:** after releasing lock `3753ff87908a41acb726bcd9c1a65561`, Rack V2 legitimately acquired U1 at `8763623c7b8a65c357e136a94ded117ff42492e3` under lock `1195d0f3107c4f90bbd1524a3362c5a1` at 15:04:49 UTC. This worker failed to stop its dependent operations after the subsequent lock acquisition failed, briefly checked out `10906c9` and launched its own retry. The retry was terminated using only its own driver PID 419966; its own Editor 420042 and temporary harness/licence were removed by its driver. U1 was restored to the other owner's exact `8763623`. Their Editor 419679, Rack harness and lock were left untouched.

**Rack V2's overlapping gate may have observed transient source/harness changes. Its owner must treat that run as contaminated and rerun it. Do not accept it based on its eventual marker or exit code.** Evidence: `lane-collision.json`, `transient-checkout-changed-paths.txt` and `candidate-10906c9/process-result.json`. This handoff and the user-facing warning report the collision; no independent message to another thread was sent.

The final launcher now refuses another task's lock, wrong HEAD, dirty lane or running Editor before creating output/staging anything. Its read-only preflight was tested against the actual conflicting lock: rejected with `OWNERSHIP CONFLICT`, exit 1; no output folder created. Python syntax and `git diff --check` pass. No further Unity work was attempted after detecting the conflict.

## Ownership, cleanup and preserved work

- Earlier T-02 locks `e66d9b051d78434892c3dbbb03714a1a`, `d64c4a5f27d945068b1cbbdf29192fa5` and `3753ff87908a41acb726bcd9c1a65561` were released normally, with no Editor remaining at each release.
- T-02 now owns **no Unity lock/lane**. U1 belongs to Rack V2 at exact `8763623`; only its own Rack harness remains. Its live lock was not released or altered.
- The original planned restore to `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd` is deferred to coordination with the current U1 owner; doing it now would overwrite that owner's active checkout.
- U1's single Editor-generated `.vscode/settings.json` change from the `863d831` run was archived/restored byte-exact before releasing that lock (`side-effects-candidate-863d831/`). No routine shared-settings change is committed.
- Primary's unrelated modified `AGENTS.md` / `active-tasks.md` and untracked Storage Architecture / Scenario Two audit files were not edited. The dashboard code checkout's pre-existing dirty files were also preserved.

## Required continuation

1. Manager/current U1 owner resolves the collision and reruns the potentially contaminated Rack V2 gate.
2. After an explicit clean lane assignment and the other owner relinquishes it, qualify final T-02 tooling/production on exact `0d3ac9208d4a36813ae0bef3d9064157a69511e6` (or a deliberately documented successor) under a new matching lock. Do not change U1 while another task owns it.
3. Require normal Editor exit 0, harness PASS, zero failed checks and cleanup PASS. Then run baseline `cf7eb61` with the **same final harness bytes**, in baseline mode under its own exact matching lock, and compare. Latest-content non-regression remains unproved until this passes.

Commands from the implementation worktree, only after those ownership prerequisites:

```bash
python3 -B Tools/Verification/S1CManagementHistory/run_t02_gate.py --project /media/jer/ZX20/CCF-U1 --out /media/jer/Files/CCF/Verification/S1C-T02-YearBrowsing-20261027/final-candidate --mode candidate --lock-id ACTUAL_CANDIDATE_LOCK_ID --expected-head 0d3ac9208d4a36813ae0bef3d9064157a69511e6 --timeout 1500
python3 -B Tools/Verification/S1CManagementHistory/run_t02_gate.py --project /media/jer/ZX20/CCF-U1 --out /media/jer/Files/CCF/Verification/S1C-T02-YearBrowsing-20261027/baseline-cf7eb61 --mode baseline --lock-id ACTUAL_BASELINE_LOCK_ID --expected-head cf7eb613594d742dab778008e7f41156c0152eda --timeout 1500
python3 -B Tools/Verification/S1CManagementHistory/compare_reviews.py --baseline /media/jer/Files/CCF/Verification/S1C-T02-YearBrowsing-20261027/baseline-cf7eb61 --candidate /media/jer/Files/CCF/Verification/S1C-T02-YearBrowsing-20261027/final-candidate --repo /media/jer/ZX20/CCF-S1C-Impl02 --base-rev cf7eb61 --head-rev 0d3ac92
```

For a manual review after qualification: open Annual Review on the four-year fixture, verify Year 4 initially, step back through 3/2/1 and forward, check Year 2 says no work, check older views omit current browsing/stock lines and the acknowledgement footer, then reopen latest and acknowledge. Before acknowledgement advancement stays blocked; afterwards one acknowledgement event is recorded and advancement works. Check top/bottom scroll positions at both supported resolutions.

T-03 should continue from this branch only after T-02 is requalified and Manager approves continuation. Preserve the selected `YearHistoryView` boundary when adding aggregate-first intervention detail. T-07 must retain the exact successful candidate/baseline evidence, cover integrated regressions and reconcile the protected legacy reconstruction/provenance boundaries; it must not treat this partial result as full S1-C conformance.

**SPEC CONFORMANCE: NOT VERIFIED** (bounded T-02 behaviour obtained; final process gate/baseline outstanding). **Ready for review: No — blocked qualification handoff.** No product decision is requested; lane coordination and verification remain.

**DASHBOARD SYNC: PASS. PUBLISHED DASHBOARD REFLECTS THIS TASK: YES (GitHub audit export).** Verifying-state audit `d9308db1be77d89e812e9c723fe14ce790c23786` (sequence 103) advanced to blocked-state audit `8f6385d74690d81bdc14a6727dccb4a970911c91` (sequence 104). Remote round trip and the published task's BLOCKED state / exact implementation SHA were verified; receipts are `dashboard-sync.json` and `dashboard-published-task.json` in the evidence archive. The local HTTP dashboard service is not running. Only supported state/publisher tools were used; generated dashboard files were never hand-edited. The authoritative Manager-maintained live coordination rows were not rewritten by this worker.
