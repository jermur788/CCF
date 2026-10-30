# S1-D T-05 / #151 — NEEDS WORK: exact-year API blocker

Provider/model: OpenAI / gpt-6.1-sol, high reasoning, T3 Code Codex harness (dispatch provider label OpenAI B).

Branch: `feature/s1-d-t05-previous-proposed-v1`  
Worktree: `/media/jer/Videos/CCF-S1D-T05`  
Exact start and unchanged production/verification end: `cfdc93bf3943125d21570bc5749a9338f52d1cce`. First blocker publication commit: `c3a254c66ce5dce2639f33055e3040b8e8824146`; final documentation successor SHA is reported on #151 (a document cannot contain its own Git commit hash). Context commit observed in this lane: `63e95ba2175f7e239a278c616681b286719821f8`.

## Disposition

**NEEDS WORK — stopped before production edits under the explicit #151 release/user stop condition.** The integrated S1-C surface does not expose exact-year navigation to Work Plan. No implementation qualification or success token is claimed. No Unity, merge, integration or Windows build was run.

Only this handoff is changed. No production, serialized assets, persistence, save/schema/validator/hash/reference, economy, ecology or teaching changes. Initial worktree was clean and on the exact required branch/start. Live GitHub coordination and #91/#106/#143/#151/#156 were refreshed; #151 has the dispatch RUNNING — OFFLINE record. #156 owns Stand Map/inspection; no MFM-09 Work Plan writer is released. The integration remote still points to the required start.

## Exact missing API / overlap

At the required start:

- `AnnualReviewView.cs:33`: selected `viewYear` is private.
- `AnnualReviewView.cs:73`: the only public selection entry is `ShowLatest()`; `Refresh(bool)` does not accept a year.
- `AnnualReviewView.cs:149`: `FindYear` is private; `StepYear` at line 675 is also private.
- `AnnualReviewView.cs:723`: existing exact-year history-button callback directly assigns private `viewYear`. It is internal navigation, not an external API.
- `AnnualReviewView.cs:87`: first/new-latest refresh resets `viewYear` to latest.
- `ScenarioOneUiRoot.cs:367–384`: `SetScreen(Review)` calls `review.ShowLatest()` then `review.Refresh(true)`.
- `ScenarioOneUiRoot.cs:473`: `ShowReview()` accepts no year and opens that latest route. Its `review` reference is private.

**Requested Manager disposition:** explicitly release/assign the tiny `AnnualReviewView` read-only exact-year selection API, e.g. `bool TryShowYear(int year)`, plus the smallest root route, e.g. `bool TryShowReviewYear(int year)`. These names are proposals, not implemented API. The view API must use existing `Manager.HistoryYears`, return false without switching to an invented/latest year if absent, preserve selection across first/new-latest refresh, and never acknowledge/mutate history. Routing must avoid `ShowLatest()` overwriting the requested selection. The root route alone cannot access the current private view selection through a supported API.

No reflection, synthetic button clicks, duplicate history UI/query/store, or indirect stepping workaround was introduced. `AnnualReviewView.cs` and `ScenarioOneUiRoot.cs` remain untouched. Latest direct user instruction and release handoff override the older packet permission to add the view hook without stopping.

## Binding classification decision table (pending implementation)

| Open ordinary record | Class / handling |
|---|---|
| `status == Approved` | Approved; Work Plan commitment only |
| `Pending && createdYear == CurrentEcologicalYear` | Pending new |
| `Pending && createdYear < CurrentEcologicalYear` | Pending carried over, independent of stale-attention threshold |
| `createdYear > CurrentEcologicalYear` | Surface inconsistent record; no invented class or history year |
| Closed record | Not a current open-work proposal; existing S1-C retains executed/failed history |

MFM prescription approval still creates ordinary Pending work with existing provenance; it must follow the same classification until Work Plan approval. No T-05 implementation or new MFM provenance witness was produced because the required seam fails preflight.

For carried work, only an existing `HistoryYears` entry matching `createdYear` may be linked. Approved work may link its recorded creation/origin year, never assert an unrecorded approval year. Current/missing history years get no fabricated completed-year destination. These are retained requirements, not verified new behaviour.

## Verification

Existing offline baselines all passed on the unchanged production tree, exit code 0 each (257 checks total). Exact commands from the assigned worktree:

```bash
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/S1CManagementHistory/run_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1c-t02
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/S1CManagementHistory/run_t04_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1c-t04
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/S1CManagementHistory/run_t06_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1c-t06
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1d-t02
```

- S1-C T-02: `S1C_HISTORY_QUERY_PASS checks=55`.
- S1-C T-04: `S1C_T04_PROVENANCE_PASS checks=56`.
- S1-C T-06: `S1C_T06_GUIDANCE_PASS checks=53 failures=0`.
- S1-D T-02: `CYCLE_STATE_OFFLINE_PASS checks=93 NOT_UNITY_VERIFIED`.

Harnesses were inspected before running; outputs/build projects and full `.log` files are under the Files evidence directory above, never under Assets or another worktree. Initial log redirection attempts failed before Python execution because the evidence parent did not exist; creating it resolved the invocation and all four commands then passed. The first push returned `remote rejected ... (failed)` without a reason; remote absence was checked and a bounded retry succeeded. No auth/quota failure was reported.

`git diff cfdc93bf3943125d21570bc5749a9338f52d1cce -- Assets Tools ProjectSettings Packages` is empty; no protected implementation or reference was changed/rebaselined. Baseline checks do not establish new T-05 behaviour or qualify the frozen Unity anchors. T-05 classification/navigation/provenance tests and rendered three-cycle qualification are not implemented/run because production stopped at the missing API. No duplicate history store was created; production diff is empty.

## T-08 dependency / next action

T-08 remains blocked on accepted T-02..T-06, including incomplete T-05. Manager must dispose the exact AnnualReview/root API overlap and authorise resumption before implementation. After release: implement Work Plan grouping and focused offline checks, publish an offline candidate, then #91 schedules serialized Unity import/runtime and 1280×720 / 1920×1080 rendered review. No claim of integration, acceptance or dashboard closure; #151 comment and this handoff are the operational return to Manager/browser #162.
