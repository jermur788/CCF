# S1-C T-07 — S1-D Data Contract and Stability Checks

**Parent:** #91 / local umbrella #106 / browser prep #186  
**Spec:** `specs/scenario-one-review-history-v1/` T-07 / AC-10  
**Status:** RELEASED FOR OFFLINE VERIFICATION  
**Exact released base:** `8fa83bff22ea1b4943689bee1074eed8eae65d2f`

## Purpose

Close the remaining S1-C prerequisite by documenting and testing the identity/data contract that later repeated-intervention work relies on. This is a bounded verification task, not new history architecture.

The integrated history helper already documents important contract semantics, including read-only history, stable ID pass-through and `harvestJobId` as a resolution-year label rather than unique job identity. The missing evidence is the explicit T-07 offline stability suite on current and genuine old-save fixtures.

## Difficulty and routing

- **Difficulty:** Low/Medium — focused offline contract verification with old-save fixture coverage; identity drift would be high consequence, but no new architecture is expected.
- **Preferred worker:** SOL/CODEX high-reasoning verification lane or Claude Sonnet-class Code lane.
- **Unity:** not required by default.

## Start gate / lane

Suggested branch/worktree: `verify/s1-c-t07-data-contract-v1` / `/media/jer/Videos/CCF-S1C-T07`.

Before edits:

1. refresh #91/#106/#151, live `Docs/Project/coordination/active-tasks.md` and this packet;
2. prove clean exact start `8fa83bff22ea1b4943689bee1074eed8eae65d2f` or a docs-only descendant with identical production;
3. confirm no writer owns the focused verification paths;
4. mark the substantive claim `[RUNNING — OFFLINE]` with actual model/provider/branch/worktree/start SHA.

Do not touch #151-owned `WorkPlanView.cs`, `AnnualReviewView.cs` or `ScenarioOneUiRoot.cs`.

## Writable surface

Prefer verification/docs only:

- a focused gate under `Tools/Verification/S1CManagementHistory/` or a narrow sibling directory;
- `Docs/Handoffs/S1-C-Implementation-07/HANDOFF.md`.

`ScenarioManagementHistory.cs` should remain runtime-read-only unless a genuinely missing **comment-only** contract note is necessary and live ownership is free. No runtime behaviour change is expected.

## Required contract checks

### Management-event identity

- positive/unique management-event IDs on valid current and old fixtures;
- S1-C history preserves authoritative event IDs and never renumbers them;
- accepted S1-D T-06 validation remains the integrity authority rather than a duplicate validator.

### Work-order identity and retention

- persisted work-order IDs remain stable across history query/reload fixtures;
- resolved/event linkage uses existing IDs;
- historical records required by later-year browsing remain available;
- cancellation is represented by the surviving cancellation event when the order object no longer exists.

### Read purity / counters

- repeated history queries do not alter events, orders, next-ID counters or fixture state;
- no reader-side identity generator/cache/store appears.

### Grouped-harvest limitation

- `harvestJobId` is the accepted resolution-year grouping label, **not** a globally unique job ID;
- grouped-harvest settlement remains job/year-level; no authoritative per-order money is invented.

### Broader existing identity links where exposed by the history fixture

Do not invent a new requirement, but verify any identities the existing S1-C query actually consumes remain pass-through rather than re-keyed: recorded tree IDs, cell indices/positions, objective-achievement IDs/keys and relevant provenance references. Cell identity/position must not be promoted into a stronger geometry-independent claim than the records support.

### Genuine old save

Use at least one genuine supported pre-S1-C fixture:

- existing identities remain unchanged;
- missing newer provenance/identity data stays absent / `not recorded`, never synthesized;
- query remains deterministic and non-mutating.

## Required negative / mutation controls

Catch at least:

1. duplicate event ID;
2. reader-side event/work-order renumbering;
3. treating `harvestJobId` as unique identity or per-order settlement authority;
4. query mutation of input collections/counters.

Use accepted validation semantics. Never weaken validators/tests to make the check green.

## Exclusions / invariants

- no new S1-D feature behaviour;
- no ID renumbering/repair;
- no save/schema/version/validator/hash/reference change;
- no fabricated old-save data;
- no second history implementation or identity namespace;
- no Annual Review/Work Plan UI change;
- no MFM provenance architecture change;
- preserve current save compatibility, Legacy40 and frozen Reference Future;
- no Windows build.

If faithful verification unexpectedly requires Unity, stop and return the exact reason instead of taking the serialized machine lane automatically.

## Handoff

Publish `Docs/Handoffs/S1-C-Implementation-07/HANDOFF.md` with exact start/head, model/provider, branch/worktree, changed files, identity-contract table, current/old fixtures, commands/results, negative-control counts, limitations and clean return state.

Success token:

`S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch line

`Run S1-C T-07 from Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md starting from exact 8fa83bff22ea1b4943689bee1074eed8eae65d2f (or a proven docs-only descendant after fresh preflight). Verification/docs only by default: prove stable event/work-order identities, append-only history meaning, harvestJobId's resolution-year limitation, genuine old-save behaviour and read purity with negative controls. Do not change save/schema/UI or take Unity automatically. Publish HANDOFF.md and return S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW or the exact blocker. No Windows build.`
