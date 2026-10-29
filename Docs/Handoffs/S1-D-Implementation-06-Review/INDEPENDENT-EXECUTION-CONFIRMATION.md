# S1-D T-06 — Independent Execution Confirmation

## Task

Confirm the executable/static review evidence for **S1-D T-06 — management-event ID save hardening** using a provider/model different from Claude Sonnet 5.5 / Anthropic.

This is a confirmation run, not a new implementation task.

## Exact candidate

Review and execute against exactly:

`29bb6cbd6bf5dd81499a1e45bb68666f34969309`

This candidate is on `review/s1-d-management-event-id-integrity-v1` and contains:

- the already-reviewed T-06 production implementation unchanged;
- the Manager-authorised verification-only U4 repair;
- the executable-review addendum produced by Claude Sonnet 5.5.

Do not silently advance to another candidate.

## Independence requirement

Executor must be a different provider/model from Claude Sonnet 5.5 / Anthropic.

Preferred: **OpenAI Codex / strongest available Sol-class local code-review model**.

If the actual executor is Anthropic/Claude Sonnet 5.5, stop and mark the claim BLOCKED rather than rerunning.

## Lane

Use a dedicated clean worktree/branch rooted at exact candidate `29bb6cb`, for example:

- branch: `review/s1-d-management-event-id-integrity-independent-confirm-v1`
- worktree: choose a dedicated clean local path and record it in the claim.

No production or verification-code edits are authorised. The only repository write permitted is the final result handoff.

Before execution:

1. refresh current `main`, live `Docs/Project/coordination/active-tasks.md`, the claim for this confirmation, this packet, and `Docs/Handoffs/S1-D-Implementation-06-Review/MANAGER-DISPOSITION-EXECUTION.md`;
2. record actual model/provider, branch/worktree and starting HEAD in the claim;
3. verify starting candidate is exactly `29bb6cbd6bf5dd81499a1e45bb68666f34969309`;
4. verify `Assets/ForestPrototype/ForestSaveValidation.cs` blob is exactly `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb`;
5. verify the only code delta from source-review handoff `5702fc67e69a97ed0f19ac83e4dda6447327fa9e` is `Tools/Verification/U4/U4Qualification.cs`.

## A. Inspect the U4 repair independently

Confirm the diff does all of the following:

- introduces an invalid-save rejection/no-mutation path for the deliberately duplicate-ID synthetic fixture;
- routes only `SYN duplicate event IDs` through that path;
- asserts `LoadData` refusal;
- asserts year unchanged;
- asserts the saved Scenario One/world snapshot is unchanged;
- leaves S1, S2 and S4 semantics unchanged;
- does not bypass `ForestSaveValidation`;
- does not weaken N3/rules-level duplicate chronology coverage.

Do not edit the code if you disagree. Return to the Manager with the exact finding.

## B. Re-run independent executable evidence

Run the same canonical suites on exact candidate `29bb6cb`:

1. T-06 event-ID integrity offline suite with mutation mode. Require baseline PASS and all 7 mutants caught.
2. N3 offline suite. Require PASS.
3. S1-D T-02 cycle-state offline suite with its canonical mutation controls. Require PASS and all expected mutants caught.
4. Repository stock tools type-check using the existing command recorded in the addendum. Do not rebaseline. Record whether it reproduces the same unrelated/stale-baseline failure profile.
5. Focused method-body type-check using the addendum's throwaway-stub approach outside the repo, including a deliberate negative control. Require the negative control to fail and the unmodified candidate's `U4Qualification.cs` plus `SaveEventIdIntegrityVerification.cs` to report zero focused errors. Record unrelated production/API-gap errors separately.

The previous Claude logs are reference evidence only. The independent executor must run the commands, not merely inspect the prior logs.

## C. Boundaries

- Production: read-only.
- Verification code: read-only.
- No Unity in this confirmation.
- No Windows build.
- No integration PR.
- Do not modify type-check baselines.
- Do not absorb unrelated dirty work.

Unity qualification remains a later machine-wide gate and may not bypass RACK-V2-08 claim #49.

## Output

Publish a GitHub Markdown result at:

`Docs/Handoffs/S1-D-Implementation-06-Review/INDEPENDENT-EXECUTION-CONFIRMATION-RESULT.md`

Include:

- actual executor/model/provider and independence statement;
- exact starting candidate, branch/worktree and final handoff commit;
- production blob identity;
- U4 diff review verdict;
- exact commands and results for T-06, N3 and T-02;
- mutation counts;
- stock and focused type-check results, including the negative control;
- confirmation no code was changed;
- Unity/Windows-build status;
- unresolved risks.

### Success token

`S1-D T-06 INDEPENDENT EXECUTION CONFIRMED / UNITY QUALIFICATION WAITING — RACK UNITY PRIORITY`

### Failure token

`S1-D T-06 INDEPENDENT EXECUTION MISMATCH — MANAGER DECISION REQUIRED`

Do not integrate T-06 from this task. Manager review follows.