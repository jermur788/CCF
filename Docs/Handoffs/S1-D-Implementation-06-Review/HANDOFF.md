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
