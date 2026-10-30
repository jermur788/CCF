# S1-C T-07 — S1-D Data Contract and Stability Checks — Browser Readiness

**Date:** 2026-10-30  
**Browser prep claim:** #186  
**Authority:** approved `specs/scenario-one-review-history-v1/` T-07 / AC-10  
**Published integration base inspected:** `8fa83bff22ea1b4943689bee1074eed8eae65d2f`  
**Verdict:** READY FOR BOUNDED OFFLINE VERIFICATION — NO UNITY REQUIRED

## Audit result

T-07 is not durably complete in the published programme records. No dedicated T-07 claim/handoff/qualification was found.

The current integrated `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs` already carries several required code-facing identity notes, including:

- read-only/on-demand history over persisted records;
- stable IDs passed through rather than recreated;
- cancellation represented by the surviving event when the order object is gone;
- grouped-harvest settlement is a derived grouping and `harvestJobId` is a **resolution-year label, not a unique job identity**;
- history views order events by authoritative `eventId`;
- no history query writes a second identity store.

That is useful implementation evidence, but T-07 also explicitly requires **offline stability assertions on fixture saves, including a genuine old save**. No durable T-07-specific gate proving those requirements was found, so T-07 must be completed rather than inferred from comments or later feature success.

## Safe execution boundary

This task can run concurrently with #151 Unity because it requires no Unity and need not touch #151's owned `WorkPlanView.cs`, `AnnualReviewView.cs` or `ScenarioOneUiRoot.cs` surfaces.

Preferred implementation is **verification/docs only**. If current code-facing notes are already sufficient, do not edit production. If a tiny comment-only contract note is genuinely missing in `ScenarioManagementHistory.cs`, stop before editing if live ownership has changed; comments must not alter runtime behavior.

No save/schema/validator/hash/reference edits are authorised.

## Required T-07 checks

On exact released lineage, prove at minimum:

1. **Event identity** — management-event IDs are positive/unique on valid current and old fixtures; the history query preserves the authoritative IDs and does not renumber.
2. **Work-order identity** — work-order IDs are stable across query/reload fixtures; history does not create a competing ID namespace.
3. **Append-only historical meaning** — resolved work and management events required by the history layer remain available after later years; cancellation is represented by the surviving cancellation event rather than a fabricated replacement order.
4. **Counter stability** — next-ID counters / stable IDs are not mutated by history reads. Run the same query twice and prove inputs/counters are byte/logically unchanged.
5. **Grouped harvest identity limitation** — assert that `harvestJobId` is treated as the resolution-year label/grouping aid and is never promoted to a globally unique job identity or per-order settlement authority.
6. **Old save** — run against a genuine supported pre-S1-C save fixture. Missing newer identity/provenance data must remain missing/labelled, not synthesized; existing IDs that are present must survive unchanged.
7. **Round-trip/read purity** — history-query/contract checks do not dirty the save structures, world, or fixture records.
8. **No competing scheme** — scan/negative assertion that the S1-C query layer does not introduce a second event/work-order identity generator/store.

Where the current validator already rejects duplicate/non-positive management-event IDs because of accepted S1-D T-06, T-07 may reuse that as underlying integrity authority, but must still prove the S1-C reader preserves/consumes the contract rather than duplicating it.

## Verification / mutation controls

Use a focused plain-.NET/offline gate if practical. Include negative controls for at least:

- duplicate event ID;
- renumbered event or work-order ID in the reader;
- treating `harvestJobId` as unique identity;
- query mutation of an input/counter.

Do not weaken existing save/history gates. No Unity import/compile is required unless the worker proves the contract cannot be exercised faithfully offline; in that case return to Manager rather than taking the machine lock automatically.

## Required handoff

`Docs/Handoffs/S1-C-Implementation-07/HANDOFF.md`

Record exact base/head, branch/worktree/claim/model/provider, changed files, commands/counts, old-save fixture identity, mutation-control results, and explicit scope statement.

Success token:

`S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW`

No Windows build.
