# MFM-07 Integration Task Packet

Parent: #91 PLAYER-TEST-OVERNIGHT-01  
Claim: #125  
Lane: local T3 / Code integration  
Status: PREPARED FOR DISPATCH

## Purpose

Integrate the Manager-approved MFM-07 proposal-state / prescription-approval boundary onto the current Scenario One umbrella integration lineage, then prove the merged candidate preserves the qualified approval boundary and MFM-06 v23 compatibility.

## Difficulty

**3/5.** The implementation itself is already reviewed and qualified. This is a bounded integration plus focused Unity/save/hash regression task. Unexpected merge or ownership conflicts are a stop/escalation condition, not permission to broaden scope.

## Recommended agent/provider

Claude Sonnet 5.5 Medium or OpenAI 6.1 Sol in the local Code lane. Escalate to Claude Opus 5.5 Low only if a real merge/architecture conflict appears.

## Exact reviewed input

- Qualified production candidate: `d4188b33e58b0fe67504a87167692d01b226c84e`
- Source branch: `feature/mfm-v1-proposal-approval-boundary-v1`
- Published docs-only successor: `01755c47d537202e9cf479545be527d43121298f`
- Qualified starting base: `b971f4da0540e606bfee9702b0c99f81a0a89355`

Integrate the exact production candidate. If using a descendant, prove it is documentation-only and contains no production or verification changes beyond the reviewed candidate.

## Before work

1. Refresh #91, #106, #121, #122 and #125.
2. Refresh live `Docs/Project/coordination/active-tasks.md`.
3. Refresh the actual current `integration/scenario-one-player-test-ready-v1` HEAD; do not assume `b971f4d` is still the live integration head.
4. Verify clean integration lane and no overlapping production writer.
5. Mark #125 `[RUNNING]` with actual model/provider, integration branch/worktree, starting HEAD and exact input SHA.
6. Refresh Unity lock/free-space/process state immediately before Unity.

## Protected MFM-07 behaviour

Preserve exactly:

- proposals are transient plain data, never `TreeMarkType.Fell`, work orders or persisted proposal state;
- manual Fell marks, open orders and confirmed Crop Tree state take precedence;
- prescription approval creates **Pending** ordinary felling work only;
- Work Plan approval remains the separate commitment gate;
- before any prescription-generated order is written, prescription + coverage unit + Crop Tree linkage is validated and then revalidated against a fresh world immediately before the first write;
- stale/invalid linkage fails closed with no partial mutation;
- unexpected write failure rolls back orders, management events, allocators and approval state/year;
- v23 provenance fields are reused; no schema/version bump or save-validator change;
- no hidden auto-approval or execution.

Do not implement MFM-08 UX, MFM-09 curriculum, broadleaf extrapolation or a new approval event type in this task.

## Post-merge qualification

Run on the exact merged umbrella candidate:

- Unity import/compile;
- focused MFM-07 approval-boundary runtime gate;
- Work Plan leakage/bypass negatives;
- MFM-06 real save/load/no-mutation regression;
- protected 18-line hash comparison / no rebaseline;
- Scenario One interaction/Legacy regression;
- any narrow additional check required to diagnose an actual merge effect.

Do not rerun the whole final Tier-3 suite merely because this is an integration task. N3/Frozen Reference/full sweep remain later programme gates unless a bounded failure makes them diagnostically necessary.

## Accepted downstream residuals

These do not block MFM-07 integration but must remain visible:

1. `TryReadPrescriptionWorld` still omits infrastructure masks and management features. Close this before shipped MFM-08/09 coverage UX relies on it.
2. Approval itself has no dedicated management-event enum; approval state/year plus per-order provenance is accepted for MFM-07 unless later history review proves otherwise.
3. A single player Fell mark can cause coverage to exclude the whole unit; safe/conflict-surfaced, but MFM-08 owns player-facing treatment/decision if needed.

## Handoff

Publish `Docs/Handoffs/MFM-07/INTEGRATION-HANDOFF.md` with:

- exact pre-integration umbrella SHA;
- exact reviewed input SHA;
- exact post-integration umbrella SHA;
- merge/cherry-pick method and conflict statement;
- changed-file summary;
- post-merge gate results;
- clean/Unity-lock state;
- residuals and next dependency.

Success token:

`MFM-07 INTEGRATED — READY FOR MFM-08 / S1-D T-05 DEPENDENCIES`

No Windows build.

## Copy/paste dispatch

Run claim #125 from `Docs/Handoffs/MFM-07/INTEGRATION-TASK-PACKET.md`. Integrate exact reviewed MFM-07 production candidate `d4188b33e58b0fe67504a87167692d01b226c84e` onto the freshly inspected Scenario One umbrella integration head. Preserve the proposal/non-mark boundary, fresh pre-write prescription+coverage-unit+Crop-Tree validation, rollback semantics and separate Work Plan commitment gate. Run the bounded MFM-07 runtime + MFM-06 save/hash + interaction regressions on the exact merged candidate and publish `Docs/Handoffs/MFM-07/INTEGRATION-HANDOFF.md`. No MFM-08/MFM-09 scope and no Windows build.