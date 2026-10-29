# S1-D T-06 — Independent Review Task Packet

## Task

**S1-D T-06 Independent Review — review management-event ID save hardening and repair the stale U4 synthetic contract.**

**Authority:** approved `specs/scenario-one-repeated-interventions-v1/` (RD-12 / FR-E3 / AC-7), implementation handoff `Docs/Handoffs/S1-D-Implementation-06/HANDOFF.md`, and Manager disposition `Docs/Handoffs/S1-D-Implementation-06/MANAGER-DISPOSITION.md`.

**Production implementation to review:** `8e05b5a24d7c3c74d063feb28002a6d7f28db438` on `feature/s1-d-management-event-id-integrity-v1`; published handoff tip `55787c8a3e99e415c89366fd98c9aa31f4c7096c`.

**Required reviewer independence:** use a different provider/model from Claude Sonnet 5.5 / Anthropic. Recommended: OpenAI Codex / strongest available Sol-class code-review model.

**Suggested review branch:** `review/s1-d-management-event-id-integrity-v1`, based on `55787c8a3e99e415c89366fd98c9aa31f4c7096c`, in a dedicated clean worktree.

**Unity:** do not acquire Unity ahead of RACK-V2-08 claim #49. Static review and verification-only U4 repair may proceed now.  
**Windows build:** prohibited.

## Before work

1. Refresh current `main`, live `Docs/Project/coordination/active-tasks.md`, the implementation claim, the review claim and this packet.
2. Create/use an isolated review branch/worktree and record reviewer model/provider, branch/worktree and starting HEAD in the review claim before edits.
3. Treat production files as read-only during review. If a production defect is found, return `NEEDS CHANGES`; do not silently repair production as the reviewer.

## Required review questions

### R1 — Authority and compatibility

Confirm the implementation matches RD-12/FR-E3/AC-7:
- duplicate or non-positive management-event IDs are rejected before mutation;
- no repair, renumber, reorder or schema bump;
- null/empty lists remain compatible where previously allowed;
- the rule is not accidentally limited to only recent save versions;
- no shipped/accepted fixture in the published audit contradicts the rule.

### R2 — Shared load boundary

Verify from source that `CanLoad`, `LoadData` and disk `Load()` still converge on one shared pre-mutation validation authority and that T-06 does not create a second event-ID validator in `ForestSaveController`.

### R3 — Validation ordering and null records

Review the decision to call `ValidateManagementEventIds` last in `ForestSaveValidation.Validate`, including:
- existing rejection reason precedence remains unchanged;
- a null management-event record is still rejected earlier by the existing cell-index validation path;
- skipping null records inside the new helper cannot allow a malformed save through.

### R4 — Offline verification quality

Inspect the new offline harness and mutation controls rather than trusting the handoff summary. Confirm the tests genuinely execute the production validator and that mutation cases would catch removal/weakening/reordering of the new rule.

### R5 — U4 stale-contract repair

Manager decision is binding: `Tools/Verification/U4/U4Qualification.cs` case `SYN duplicate event IDs` is a synthetic negative fixture, not a shipped/accepted save, so it is **not** the STOP condition.

The reviewer is authorised to make the smallest **verification-only** repair needed so U4 reflects the approved save boundary:
- the deliberately duplicate-ID edited save must be expected to fail load before mutation rather than be accepted and later remain incomplete;
- preserve the protected N3/rules-level duplicate-ID chronology coverage; do not delete or weaken it;
- do not bypass the validator to inject invalid history into live state;
- do not change production code, save schema, completion rules or objective logic.

A narrow helper/assertion in `U4Qualification.cs` is acceptable. Any broader harness redesign returns to the Manager.

## Static/offline verification during review

Run at minimum:
- T-06 offline check, including mutation mode;
- N3 offline suite;
- S1-D T-02 cycle-state offline suite;
- any bounded static/type-check available for the changed verification code without requiring Unity.

Record exact commands/results and distinguish executed evidence from source inspection.

## Unity status

Do not run Unity while #49 has priority. The independent review may conclude static PASS with Unity qualification outstanding.

When the Manager later releases the Unity lane, the exact reviewed candidate must run:
- T-06 runtime save-load parity/no-mutation gate;
- existing `SAVE_HARDENING_VERIFY_PASS`;
- save-cell-index hardening suite;
- U4 relevant gate after the verification-only repair;
- clean import/compile with no new errors.

No assertion may be weakened or rebaselined merely to obtain green.

## Handoff

Publish:

`Docs/Handoffs/S1-D-Implementation-06-Review/HANDOFF.md`

Include:
- reviewer/model/provider and independence statement;
- branch/worktree and starting/reviewed/published SHAs;
- production review findings R1–R4;
- exact U4 repair and why it is not a test weakening;
- executed offline/static results;
- Unity status and exact remaining gates;
- repo cleanliness/unrelated-work accounting;
- verdict `APPROVED` / `NEEDS CHANGES` / `REJECTED`.

### Success token while Unity remains queued

`S1-D T-06 INDEPENDENT STATIC REVIEW PASS / UNITY QUALIFICATION WAITING — RACK UNITY PRIORITY`

### Failure token

`S1-D T-06 INDEPENDENT REVIEW NEEDS CHANGES — RETURN TO MANAGER`

Do not open an integration PR and do not merge T-06 from this review task.
