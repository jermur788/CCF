# Scenario One — Multi-Year Decision and Outcome Feedback V2 Implementation Plan

- **Spec ID:** SPEC-scenario-one-multiyear-decision-outcome-feedback-V2
- **Spec status:** DRAFT — MANAGER REVIEW REQUESTED. This plan is a proposal and releases no implementation.
- **Plan owner:** Overall Manager
- **Created:** 2026-11-12
- **Last updated:** 2026-11-12 (HP #409 refinement)

Authority: [spec.md](spec.md). Tasks: [tasks.md](tasks.md). Checklist: [checklists/requirements.md](checklists/requirements.md). Requirement IDs (FR-n, AC-n) are the spec's.

## Architecture / current-state findings

Read at `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4`. Verified by source reading only; nothing here was compiled or run by the plan's author.

- **`AnnualReviewView.Refresh`** rebuilds the scroll content when a change key differs. `ScenarioOneUiRoot` calls it every frame while the review is open, and `HistoryYears` rebuilds every completed year, so anything expensive must sit behind that key.
- **Year selection** is `viewYear` (−1 means latest) with `StepYear`, `ShowLatest` and `TryShowYear`. `showingEarlierYear` hides the acknowledgement footer and makes the acknowledge button a no-op on earlier years.
- **Existing comparison** is previous snapshot → viewed year only.
- **`ScenarioOneManagementFollowonReadout.Build(IReadOnlyList<YearHistoryView>, int)`** is a static, non-mutating function. Its `Result` carries `SelectedYear`, `LatestCompletedYear`, three `ObservationState` values, copied `Selected` and `Latest` observations, a `Comparison`, and `RecordedWork` for **all** completed years.
- **Text helpers** already follow a pattern of pure static classes without UI types (`ManagementHistoryText`, `HistoryProvenanceText`).
- **Cadence candidate** `e759193354b4613f88e3370a62ccaccf02c556cf` changes the same view: it adds a `skipSummary` label above the header and a reset at the top of `Refresh`. It does not touch the scroll content builders.

Superseded statement: the earlier draft said the helper was uncompiled. That is no longer true for offline type binding and 53 offline assertions (#406). It remains true for Unity.

## Proposed technical approach

Two small pieces, so that most of the behaviour can be checked without Unity:

1. **A pure text mapper** — one new static class beside `ManagementHistoryText` (working name `ScenarioOneYearComparisonText`). Input: a `Result` and the viewed year. Output: an ordered list of plain rows (provenance kind, text, optional three column values). It holds every FR-4 to FR-12 rule and every FR-9 wording. It references no `UnityEngine` type, so the existing offline runner can assert each state. Species display names are passed in as a function, because `ScenarioOneUiFacts.SpeciesName` looks up a scene object.
2. **A thin view method** — one new private `BuildYearComparison` in `AnnualReviewView`, called from the guarded part of `Refresh`. It calls `Build` once, passes the result to the mapper and adds the rows with the existing `AddTagged` / `TaggedLine` helpers and one `Foldout` for species. No new styles unless the 720p check requires one.

Rejected: putting the rules straight into the view (only checkable in Unity); extending the helper (it is approved as-is and FR-3 needs nothing more); a new screen (Option C, out of scope).

## Integration boundaries

- **May add:** one new text-mapper source file and its `.meta` under `Assets/ForestPrototype/ScenarioOne/`; new offline fixture files under `Tools/Verification/`.
- **May edit:** `Assets/ForestPrototype/UI/AnnualReviewView.cs` only, and only to add the method, its call and the foldout-state key.
- **Must not edit:** `ScenarioOneManagementFollowonReadout.cs`, `ScenarioManagementHistory.cs`, `ScenarioInterventionHistory.cs`, `ScenarioEcologicalSnapshot.cs`, any `ScenarioOneManager*` file, `WorkPlanView.cs`, `ScenarioOneUiRoot.cs`, save, migration, hash or reference files, MFM files, scenes, prefabs, existing tests or fixtures.
- **Interfaces used:** `ScenarioOneManager.HistoryYears`, `ScenarioOneManagementFollowonReadout.Build`, `ScenarioOneUiFacts.SpeciesName`, `HistoryProvenance`, existing `UiKit` helpers.

## Affected systems / files

- `Assets/ForestPrototype/UI/AnnualReviewView.cs` (edit, bounded)
- `Assets/ForestPrototype/ScenarioOne/<text mapper>.cs` and `.meta` (new)
- `Tools/Verification/...` offline fixture for the mapper (new)

## Ownership / concurrency considerations

- `AnnualReviewView.cs`, `WorkPlanView.cs` and `ScenarioOneUiRoot.cs` belong to the Fable cadence lane ([#402](https://github.com/jermur788/CCF/issues/402) / [#408](https://github.com/jermur788/CCF/issues/408)) until the Manager records its release. **No task that touches `AnnualReviewView.cs` may start before that.**
- The implementation base must contain both the released cadence result and helper blob `2c7bdb3c6b6f13790a325ebabc187c497b335b52`. Today those live on different branches; choosing or building that base is a Manager/integrator step (T-01), not an implementer's.
- One UI writer, one worktree, one branch. Unity work takes the single machine-wide lease; the mapper and its offline fixture need no lease.
- The personal playtest lane ([#382](https://github.com/jermur788/CCF/issues/382)) and `main` are not targets.

## Migration / save implications

None. No schema, version, event, hash or archive change. Old saves are read as they are.

## Verification strategy

- **Implementation correctness:** offline fixture over the mapper for every FR-9 state, FR-5 exclusion, FR-8 species cases and FR-12; existing 53 helper assertions re-run unchanged on the chosen base; production type-binding check with zero errors.
- **Spec conformance:** the checklist in `checklists/requirements.md`, each line with evidence from the exact candidate head.
- **Verification tier:** Tier 1 for the mapper (offline). Tier 2 once, on the wired candidate, for AC-2, AC-7 to AC-10 and AC-12. No Tier 3 for this feature alone.
- **Unity-only items:** real compile and import; rendered 1280×720 and 1920×1080; scroll and footer priority; real-input year stepping; acknowledgement event count; one-year versus long-advance card equality; a genuine older save, Legacy40 and frozen Reference Future v1.
- **Not acceptable as evidence:** synthetic legacy records for AC-10; a screenshot from a different head; offline results presented as Unity results.

## Risks

- **720p fit.** The table plus caveat may be too tall beside the headlines. Mitigation is already in scope: species closed by default, and the stop condition if the footer is displaced.
- **Two counts on one screen.** The card's work counts and the Work column's are computed by different code. AC-12 makes a disagreement visible; open decision 4 removes the risk by dropping the counts.
- **Per-frame cost.** A `Build` call outside the change-key guard would rebuild every frame. FR-14 forbids it; the review step checks the call site.
- **Base drift.** If the cadence result changes `Refresh` again, the call site moves. T-01 pins the base before T-03 starts.
- **Wording unreviewed in game.** All copy here is proposed text that no player has seen.

## Implementation sequence

1. Manager decision on the spec and its six open decisions — **T-00**.
2. Cadence release recorded; integration base chosen and pinned; single UI owner assigned — **T-01**.
3. Re-run the existing helper checks on that base, no source change — **T-02**.
4. Text mapper and offline fixture — **T-03**.
5. View wiring — **T-04**.
6. Unity Tier 2 qualification by a worker other than the implementer — **T-05**.
7. Independent review, Manager verdict, integration decision, dashboard — **T-06**.

T-02 and T-03 touch no cadence-owned file and may be released before T-04 if the Manager wishes; they still require the spec to be `APPROVED`.
