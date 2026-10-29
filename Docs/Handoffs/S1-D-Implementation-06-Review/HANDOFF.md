# S1-D T-06 INDEPENDENT REVIEW — HANDOFF

**Task:** S1-D T-06 — Reject ambiguous management-event IDs before save-load mutation  
**Review claim:** #85  
**Reviewer:** GPT-5.6 Sol / OpenAI  
**Review branch:** `review/s1-d-management-event-id-integrity-v1`  
**Reviewed implementation tip:** `55787c8a3e99e415c89366fd98c9aa31f4c7096c`  
**Production implementation:** `8e05b5a24d7c3c74d063feb28002a6d7f28db438`  
**Implementer:** Claude Sonnet 5.5 / Anthropic  
**Unity:** not run  
**Windows build:** not run

## Verdict

**S1-D T-06 INDEPENDENT SOURCE REVIEW PASS / EXECUTABLE REVIEW CONTINUATION REQUIRED — RACK UNITY PRIORITY**

The production change passes independent source review. I found no source-level defect requiring production changes. The change remains **not independently execution-qualified, not Unity-verified, not integrated and not accepted as a completed T-06 slice** because this browser lane has no .NET/C# compiler and cannot run the packet's required executable offline suites. Unity also remains queued behind RACK-V2-08 claim #49.

This handoff is therefore a completed browser/static review result plus an exact continuation requirement, not the packet's full success token.

## 1. Production review

### R1 — scope and authority: PASS

The production diff is confined to `Assets/ForestPrototype/ForestSaveValidation.cs` (+21, no deletions). It adds one shared validation rule for `ScenarioOneSaveData.managementEvents`:

- `eventId <= 0` is rejected;
- repeated positive `eventId` is rejected;
- null/empty management-event lists remain accepted;
- no event is repaired, renumbered, deduplicated or reordered;
- `ForestSaveData.CurrentVersion` remains 21;
- no persisted field was added.

That matches S1-D RD-12 / FR-E3 / AC-7. The preflight fixture audit reported no ambiguous shipped/accepted fixture, so the spec STOP condition was not triggered.

### R2 — one shared pre-mutation authority: PASS by source inspection

`ForestSaveController` still centralises parsed-save validation through `FindLoadProblem`:

- `CanLoad(out problem)` reads through `TryReadSave`, which calls `FindLoadProblem`;
- `LoadData(...)` calls `FindLoadProblem` directly;
- `FindLoadProblem` calls `ForestSaveValidation.Validate`;
- `LoadData` returns `false` before the source's explicit `First change to the world` boundary when validation reports a problem;
- `Load()` reads through `TryReadSave` and then calls `LoadData`.

No second event-ID rule was added to the controller. The T-06 rule is therefore one shared pre-mutation validation authority rather than duplicated `CanLoad`/`LoadData` logic.

### R3 — reason precedence and null handling: PASS

`ValidateManagementEventIds` is the final validation step in `ForestSaveValidation.Validate`. Existing geometry, scenario-cell, tree, storm, model, ecology-cell, v15 scenario and Rack V2 validation therefore retain their earlier precedence. A null management-event record is still rejected earlier by `ValidateScenarioCellIndices`; the new helper's null-record skip does not create an acceptance path. A null `scenarioOne`, null list or empty list remains accepted where prior compatibility semantics allowed it.

### R4 — future ID-counter safety: PASS after additional review

I investigated a possible gap not called out in the implementation handoff: a save could theoretically contain unique positive historical event IDs but a stale `nextManagementEventId` that points backward into that history.

The existing load path already makes this safe. `ScenarioOneManager.LoadScenario` sets:

- `nextManagementEventId = Max(1, data.nextManagementEventId)`; then
- when events exist, `nextManagementEventId = Max(nextManagementEventId, Max(eventId) + 1)`.

`RecordEvent` then assigns `entry.eventId = nextManagementEventId++`.

Therefore an otherwise-valid loaded history cannot create a duplicate merely because the serialized counter is stale. This is existing counter restoration behaviour; T-06 does not need a second production rule for it.

## 2. Verification-suite review

### T-06 offline suite: design review PASS; execution NOT REPEATED HERE

`run_event_id_integrity_offline.py` copies the actual production `ForestSaveValidation.cs` into a temporary compilation area with the required production dependencies rather than reimplementing the validator. `EventIdIntegrityOfflineCheck.cs` covers:

- positive/unique acceptance;
- zero and negative rejection;
- adjacent/non-adjacent duplicate rejection;
- deterministic first-offender messages;
- null/empty/absent-list compatibility;
- version spread;
- earlier-reason precedence;
- unchanged unrelated validation;
- Reference Future/storm fixture validation;
- JSON-round-trip cases;
- no schema bump;
- no repair of event history.

The mutation driver targets seven relevant weaknesses: zero allowed, negatives allowed, duplicates allowed, rule unwired, rule moved before older validation, null list rejected, and repair-by-sorting.

Worker-reported result was 75/75 PASS and 7/7 mutants caught. I reviewed the source but **did not independently execute those results** because this browser environment has no `dotnet`, `csc`, `mcs` or `mono` toolchain.

### N3 and T-02 preservation: source/hand-off reviewed; execution NOT REPEATED HERE

Worker reported N3 88 PASS and T-02 cycle-state 93 PASS. This review did not independently execute them for the same environment reason.

### Unity runtime harness: design reviewed; NOT COMPILED / NOT RUN

`SaveEventIdIntegrityVerification.cs` is appropriately aimed at the missing runtime evidence: Legacy40/Enlarged80, `CanLoad`/`LoadData`/disk `Load()` parity, exact rejection reasons, valid-case acceptance, and world/disk non-mutation. It remains uncompiled and unrun. Existing `SAVE_HARDENING_VERIFY_PASS` and save-cell-index gates also remain unrun.

No Unity lock was requested or acquired. RACK-V2-08 #49 retains Unity priority.

## 3. U4 synthetic duplicate-ID contract

### Manager disposition confirmed: NOT a STOP condition

`Tools/Verification/U4/U4Qualification.cs` has a deliberately edited synthetic fixture named `SYN duplicate event IDs`. Its shared `SyntheticFromS24` helper currently assumes the edited save passes `LoadData`, then advances to inspect completion chronology.

RD-12 supersedes that acceptance assumption: duplicate IDs must now be rejected before mutation. The spec STOP condition applies to a **shipped or accepted save fixture**, not to a deliberately-invalid in-memory synthetic negative fixture.

### Required verification-only repair

Do not delete the duplicate-ID case, weaken validation, or bypass the loader. Replace only that S3 contract with a rejection-path helper. The minimal intended shape is:

```csharp
private IEnumerator SyntheticRejectedFromS24(string s24, string label, Action<ScenarioOneSaveData> edit)
{
    yield return RestoreJson(s24, label);
    string before = Snapshot();
    int yearBefore = Year;
    ForestSaveData full = saves.CaptureData();
    edit(full.scenarioOne);

    Expect(!saves.LoadData(full, false), label + ": invalid edited save refused");
    yield return null; yield return null;

    Expect(Year == yearBefore, label + ": rejection preserves year");
    Expect(Snapshot() == before, label + ": rejection preserves Scenario One/world snapshot");
}
```

Then route only `SYN duplicate event IDs` through `SyntheticRejectedFromS24(...)`; keep S1, S2 and S4 on `SyntheticFromS24(...)`. Do not advance the rejected S3 to Y25 or assert completion state because the invalid save never becomes live state.

N3/rules-level duplicate chronology coverage remains the place that proves ambiguous IDs cannot establish completion chronology independently of save-load acceptance.

This is a verification-contract repair caused by enforcement of an already-approved rule, not a production relaxation or rebaseline.

## 4. Findings

| ID | Finding | Disposition |
|---|---|---|
| F-1 | Production rule is narrow and matches RD-12 | PASS |
| F-2 | `CanLoad` and `LoadData` share one pre-mutation validator | PASS |
| F-3 | Existing validation reason precedence is preserved | PASS |
| F-4 | Null event records remain rejected by earlier validation | PASS |
| F-5 | Stale serialized `nextManagementEventId` cannot generate a duplicate after load because load clamps it above the maximum existing event ID | PASS |
| F-6 | Worker offline suite has meaningful direct-production and mutation coverage | PASS by design review; not independently executed here |
| F-7 | Existing U4 duplicate-ID synthetic load-acceptance assertion is stale under RD-12 | REPAIR REQUIRED in verification only |
| F-8 | Unity runtime parity/non-mutation and existing save-hardening gates are outstanding | WAITING behind Rack #49 |

No source-level production change is requested from the implementer.

## 5. Required continuation before Manager integration

A local OpenAI/Codex review continuation must:

1. start from this review lineage containing `55787c8...` plus the review handoff;
2. apply the bounded U4 verification-only repair above;
3. run the T-06 offline check with all seven mutation controls;
4. run N3 and T-02 cycle-state preservation suites;
5. perform bounded compile/type checks appropriate to the changed verification code;
6. publish exact command/output evidence and branch SHA;
7. **not** open an integration PR yet;
8. leave Unity gates waiting behind #49 unless the Manager explicitly releases the machine-wide lane.

After that executable independent-review continuation passes, the Manager may accept the independent static review stage while still withholding integration until the required Unity save-hardening qualification is green.

## Status

- IMPLEMENTED: **YES — static production change**
- WORKER OFFLINE VERIFIED: **YES — reported evidence**
- INDEPENDENT SOURCE REVIEWED: **YES — PASS**
- INDEPENDENT EXECUTABLE OFFLINE REVIEWED: **NO**
- U4 VERIFICATION CONTRACT REPAIRED: **NO — exact repair specified for continuation**
- UNITY VERIFIED: **NO**
- INTEGRATED: **NO**
- ACCEPTED AS COMPLETE T-06 SLICE: **NO**

**No Windows build was made or authorised.**

---

# EXECUTABLE REVIEW ADDENDUM (claim #86)

**Date:** 2026-10-29  
**Executor:** Claude Sonnet 5.5 / Anthropic (Claude Code, local), at the user's direct instruction.  
**Independence statement:** the packet and continuation require an executor from a different provider/model than the Claude Sonnet 5.5 / Anthropic implementer. **This executor is the same provider/model as the implementer, so this addendum does NOT satisfy that requirement.** It is mechanical execution evidence plus the prescribed verification-only repair. Source-level independence for the production change still rests on the GPT-5.6 Sol / OpenAI browser review above. The Manager decides whether to accept this execution or have a non-Anthropic executor re-run it; every command below is reproducible.  
**Worktree:** `/media/jer/ZX20/CCF-S1D-T06Review` (dedicated, clean at start)  
**Branch:** `review/s1-d-management-event-id-integrity-v1`  
**Starting HEAD:** `5702fc67e69a97ed0f19ac83e4dda6447327fa9e`  
**Evidence directory (durable):** `/media/jer/Files/CCF/Verification/S1D-T06-ExecReview-20261029/`  
**Unity:** not run. **Windows build:** not made. **Integration PR:** not opened.

## A. Production unchanged

`Assets/ForestPrototype/ForestSaveValidation.cs` blob is `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb` at starting HEAD, at `8e05b5a` and after the repair. The only changed file in this addendum's code change is `Tools/Verification/U4/U4Qualification.cs` (`git diff --name-only` outside that file: none). No production file, schema, completion rule or objective logic was touched.

## B. U4 verification-only repair

Diff: `u4-repair.diff` in the evidence directory (+20 / −3).

- Added `SyntheticRejectedFromS24(s24, label, edit)`, exactly the shape specified in section 3 above: restore the Y24 save, snapshot, edit a captured save, then assert `!saves.LoadData(full, false)`, same year, and an identical `Snapshot()` afterwards.
- Routed only `SYN duplicate event IDs` (S3) through it and dropped its `expectComplete: false` argument. S1, S2 and S4 still use `SyntheticFromS24`, unchanged.
- Updated the S3 comment to say why.

**Why this is not a weakening:** the fixture is still present and still invalid. Instead of an assertion that holds only because a loaded duplicate-ID history fails to complete, it now asserts the stronger approved boundary (RD-12): refusal before any mutation, with a before/after snapshot comparison. The validator is not bypassed, N3/rules-level duplicate chronology coverage is untouched, and S4 still proves the negative fixtures are not vacuous.

**Not proven here:** the helper asserts refusal and non-mutation, not the specific rejection reason. The fixture contains two records sharing one positive ID, so the T-06 rule should be the one that fires, but that is only confirmed when the U4 gate runs in Unity (offline T-06 proves the rule and reason text, not this fixture).

## C. Executed offline suites (all from this worktree, exit 0)

| # | Command (run from worktree root) | Result | Log |
|---|---|---|---|
| 1 | `python3 -I Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/run_event_id_integrity_offline.py --out <EV>/t06-work --mutations` | `EVENT_ID_OFFLINE_PASS checks=75 failures=0`; `EVENT_ID_MUTATIONS_PASS caught=7 survived=0` | `01-t06-offline.log` |
| 2 | `python3 -I Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py --out <EV>/n3-work` | `N3_OFFLINE_PASS checks=88 NOT_UNITY_VERIFIED` (baseline 88) | `02-n3.log` |
| 3 | `python3 -I Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py --out <EV>/t02-work --mutations` | `CYCLE_STATE_OFFLINE_PASS checks=93 NOT_UNITY_VERIFIED`; `CYCLE_STATE_MUTATIONS_PASS caught=7 survived=0` | `03-t02.log` |

The suite 3 command is the canonical T-02 runner recorded in `Docs/Handoffs/S1-D-Implementation-02/HANDOFF.md`. No `FAIL` line in any of the three logs.

Seven T-06 mutants, all caught: zero id allowed (8 failing checks), negative id allowed (2), duplicate id allowed (9), rule never wired into `Validate` (19), rule runs before older checks (4), null list rejected (1), repairs by sorting the list (13).

N3: the suite passes, but its log prints no explicit completion-rule source hash line. Protected completion-rule files are unchanged by construction, because the only changed file is `Tools/Verification/U4/U4Qualification.cs`.

Running the N3 runner left an untracked `Tools/Verification/__pycache__/` inside the repo. I deleted it; it is not committed.

## D. Compile / type checks

Method: the repository's `Tools/Verification/CloudTypeCheck` approximation (UnityEngine 2021.3 reference assemblies, not Unity's compiler).

1. **Stock runner:** `python3 -I Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools --out <EV>/typecheck-tools-work` exited 1 with `TYPECHECK_NEW_ERRORS`, log `04-typecheck-tools.log`. **Not a clean result.** The 22 "NEW" entries are all in other files (Painter2D declarations; duplicate-definition clashes among other branches' offline-stub harnesses), and 43 baseline entries are "GONE", so `baseline-tools.txt` looks stale for this lineage. None of the entries is in `U4Qualification.cs` or `SaveEventIdIntegrityVerification.cs`. I did not update the baseline.
2. **That run does not prove method bodies.** Roslyn stops before binding bodies while declaration errors exist, and the stock project has the Painter2D declaration errors. A negative control confirmed it: an injected undefined identifier in U4 was *not* reported.
3. **Focused check (outside the repo):** production `Assets/**` (non-Editor) + `InputSystemStub.cs` + `UnityEditorStub.cs` + throwaway `Painter2DStub.cs` and `SessionStateStub.cs` + the edited `U4Qualification.cs` + `SaveEventIdIntegrityVerification.cs`. With the stubs, declarations resolve and bodies are bound.
   - **Negative control** (`U4Qualification.control.cs` with `undefinedSymbolForControl` injected into the new helper): reported `error CS0103 ... U4Qualification.control.cs(652,75)`. The check can fail.
   - **Edited U4 + T-06 harness:** **zero errors in `U4Qualification.cs` and zero in `SaveEventIdIntegrityVerification.cs`.**
   - 15 other errors remain, all in unmodified production files and attributable to the 2021.3 vs Unity 6 API gap (`FindObjectsByType`, `FindObjectsInactive`, `Mouse.WarpCursorPosition`, `Keyboard.shiftKey`/`numpadEnterKey`, `MeshGenerationContext.painter2D`, `GetEntityId`). Same set with and without my change.
   - Logs: `05-focused-typecheck-edited.log`, `06-focused-typecheck-negative-control.log`. The stubs and csproj files are in the evidence directory, not in the repo.

This is a 2021.3 approximation, not Unity's compiler. It does not show that Unity 6000.6 compiles the files.

## E. Source-review findings re-checked

I did not add a rule for a stale serialized `nextManagementEventId`. No executed evidence contradicted the browser review's finding that `ScenarioOneManager.LoadScenario` clamps it above the maximum existing event ID. This was not re-derived by an executable test here.

## F. Unresolved risks

- Executor is not provider-independent (see top of addendum).
- Unity not run: `SaveEventIdIntegrityVerification.cs` is type-checked only, never run. `SAVE_HARDENING_VERIFY_PASS`, save-cell-index hardening and the U4 gate (including the new `SYN duplicate event IDs` refusal/no-mutation assertions) still wait behind RACK-V2-08 #49.
- The U4 helper does not assert the specific rejection reason (section B).
- The stock tools-mode type-check baseline is stale; recommend a separate baseline-refresh task (not done here).

## Status after addendum

- INDEPENDENT SOURCE REVIEWED: **YES — PASS** (GPT-5.6 Sol / OpenAI, #85)
- EXECUTABLE OFFLINE SUITES RUN: **YES — all PASS, 7/7 + 7/7 mutants caught**
- U4 VERIFICATION CONTRACT REPAIRED: **YES — verification-only, type-checked, not run in Unity**
- EXECUTOR PROVIDER-INDEPENDENT: **NO**
- UNITY VERIFIED / INTEGRATED / ACCEPTED AS COMPLETE T-06 SLICE: **NO**

**No Windows build was made or authorised.**
