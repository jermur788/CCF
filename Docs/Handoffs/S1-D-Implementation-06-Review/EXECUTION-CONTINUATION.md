# S1-D T-06 INDEPENDENT REVIEW — EXECUTION CONTINUATION

## Task

**S1-D T-06 — finish independent executable review of management-event ID save hardening.**

Browser/static independent review is complete on branch `review/s1-d-management-event-id-integrity-v1` at review handoff commit `5702fc67e69a97ed0f19ac83e4dda6447327fa9e`.

Static verdict: **PASS** for the production source. No production defect was found. Production remains read-only in this continuation.

The browser reviewer could not run the packet's C#/.NET suites because that environment has no .NET/C# compiler. This continuation exists to execute that evidence, apply the Manager-approved U4 verification-only repair, and publish a complete executable independent-review result.

## Authority and reviewed candidate

- Original implementation tip: `55787c8a3e99e415c89366fd98c9aa31f4c7096c`
- Production implementation: `8e05b5a24d7c3c74d063feb28002a6d7f28db438`
- Browser/source-review handoff commit: `5702fc67e69a97ed0f19ac83e4dda6447327fa9e`
- Review handoff: `Docs/Handoffs/S1-D-Implementation-06-Review/HANDOFF.md` on `review/s1-d-management-event-id-integrity-v1`
- Manager disposition: `Docs/Handoffs/S1-D-Implementation-06/MANAGER-DISPOSITION.md`
- Original review packet: `Docs/Handoffs/S1-D-Implementation-06-Review/TASK-PACKET.md`

Target reviewer/executor: **OpenAI Codex / strongest available Sol-class local code-review model**. The implementation was authored by Claude Sonnet 5.5 / Anthropic, so this preserves provider/model independence.

## Preflight

Before changing verification code or executing local tools:

1. Refresh current `main`, this continuation, claim #86, and live `Docs/Project/coordination/active-tasks.md`.
2. Use a dedicated clean worktree for `review/s1-d-management-event-id-integrity-v1` at `5702fc67...` or a clean descendant containing only authorised review work.
3. Mark claim #86 `[RUNNING]` with actual agent/model/provider, worktree, branch and starting HEAD.
4. Verify production blob `Assets/ForestPrototype/ForestSaveValidation.cs` is still exactly the reviewed implementation unless the Manager has explicitly authorised a new production revision.
5. Do not acquire Unity. This continuation is the executable **offline/static** independent-review phase only; Unity qualification remains separately queued behind RACK-V2-08 #49.

## A. Apply the authorised U4 verification-contract repair

File allowed to change:

`Tools/Verification/U4/U4Qualification.cs`

The existing `SYN duplicate event IDs` case is a deliberately-invalid synthetic fixture, not a shipped/accepted save. RD-12 now requires it to be rejected before mutation. Do not delete the case and do not bypass `ForestSaveValidation`.

Implement the smallest equivalent of this source-review design:

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

Route only `SYN duplicate event IDs` through the rejection helper. Keep S1, S2 and S4 on the existing accepted-save helper. Do not advance rejected S3 to Y25 or ask the completion evaluator to interpret a save that never loaded.

Preserve N3/rules-level duplicate chronology coverage. This is not permission to weaken or remove duplicate-ID logic tests.

If a smaller implementation proves the same rejection/no-mutation contract, it is acceptable. Report the exact diff and reasoning.

## B. Required executable independent-review runs

Run from the exact review candidate after the U4 verification repair:

1. `python3 Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/run_event_id_integrity_offline.py --mutations`
   - require baseline PASS;
   - require all seven mutants caught.
2. `python3 Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py`
   - require PASS with protected completion-rule source/hash unchanged.
3. T-02 cycle-state offline suite:
   - use the current canonical runner recorded by the implementation handoff;
   - require PASS and mutation controls where that runner normally includes them.
4. Bounded compile/type/static checks for the edited U4 source and the T-06 verification sources, using the repository's established method. A source-only grep is not enough if an existing local type-check path is available.

Do not modify production merely to make these tests pass. If production needs a change, stop and return:

`S1-D T-06 EXECUTABLE REVIEW FOUND PRODUCTION DEFECT — MANAGER DECISION REQUIRED`

## C. Explicitly preserve the source-review findings

The browser review additionally inspected `nextManagementEventId`. Existing `ScenarioOneManager.LoadScenario` clamps it to at least `max(existing eventId)+1`, and `RecordEvent` assigns from that counter. Do not add a redundant production rule for a stale serialized counter unless new executable evidence disproves that inspected behavior.

## D. No Unity in this continuation

Do **not** run:

- `SaveEventIdIntegrityVerification.cs`;
- `SAVE_HARDENING_VERIFY_PASS`;
- save-cell-index Unity hardening;
- any other Editor/runtime qualification.

Those remain a later machine-wide Unity qualification and may not bypass RACK-V2-08 claim #49.

No Windows build.

## Output

Update/publish:

`Docs/Handoffs/S1-D-Implementation-06-Review/HANDOFF.md`

on the review branch with an executable-review addendum containing:

- actual reviewer/model/provider;
- worktree and starting HEAD;
- U4 diff and rationale;
- exact commands and outputs for all offline suites;
- mutation results;
- compile/type-check results;
- confirmation production source/blob was unchanged;
- final candidate SHA and worktree cleanliness;
- Unity explicitly not run;
- unresolved risks.

On full success return:

`S1-D T-06 INDEPENDENT STATIC REVIEW PASS / UNITY QUALIFICATION WAITING — RACK UNITY PRIORITY`

Do not open an integration PR. Manager review follows the handoff.
