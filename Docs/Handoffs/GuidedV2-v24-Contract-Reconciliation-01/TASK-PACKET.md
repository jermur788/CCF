# Guided-v2 v24 / D-066 Contract Reconciliation 01 — Task Packet

Claim: #198

## Purpose
Reconcile stale `ScenarioOneRackUx` / `SequentialObjectives` verification expectations with the accepted current v24 / D-066 teaching-latch lineage, then rerun full `guided-v2`.

## Exact start
`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

## Authority
Parent diagnosis: `Docs/Handoffs/RackV2-Current-Lineage-Red-Gate-Diagnosis-01/HANDOFF.md`.

Known stale expectations include:
- `ForestSaveData.CurrentVersion == 23` in `SequentialObjectivesVerification.cs`;
- hard-coded `Stage(4, "save/load ... requires fresh rack review")`;
- session-only rack-review assumptions that #195 observes as lesson index 6/7 and review counters 0/0 on current v24 lineage.

Read the accepted D-066/v24 persistence/curriculum handoffs before editing. Do not infer the replacement expectation.

## Scope
Verification-only. Minimum files under `Tools/Verification/ScenarioOneRackUx/` and `Tools/Verification/SequentialObjectives/` plus handoff/docs.

Preserve:
- real OS-input intent;
- explicit Rack V2/model-0 split;
- three distributed future-tree/release/review coverage where still part of accepted teaching;
- save/load determinism;
- protected Rack assertions.

Forbidden: production edits, schema/anchor changes, broad fixture changes, accepting arbitrary lesson indices, skipping review assertions, Windows build.

## Execution
- Difficulty: Medium.
- Preferred: Claude Code / Sonnet 5.5 Medium or equivalent verification-capable local worker.
- Branch: `verify/guided-v2-v24-contract-reconcile-v1`.
- Unity: serialized; fresh live preflight and lock acquisition required.
- Before edits/Unity: refresh `active-tasks.md`, issue #198, exact HEAD, worktree/lane cleanliness, storage, Editor processes and lock; post `[RUNNING]` with provider/model/worktree/start SHA.

## Required verification
1. Inventory stale assertions and map each to current accepted authority.
2. Make the minimum verification-only patch.
3. Offline/static check.
4. Run full `guided-v2` on exact start + patch.
5. Run proportionate model-0/legacy companion coverage required to prove compatibility is not weakened.
6. If a correctly reconciled gate still exposes production failure, stop and hand it back; no production repair under #198.
7. Return Editor/lane/lock clean.

## Handoff
`Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/HANDOFF.md`

Success token: `GUIDED-V2 V24 CONTRACT RECONCILED — READY FOR MANAGER REVIEW`

Copy/paste:
`Run #198 from Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/TASK-PACKET.md starting at exact ce6737146a7aa9ea953d7db0ca93b103f6c2c996. Reconcile only stale guided/SequentialObjectives verification expectations to accepted v24/D-066 teaching-latch authority; preserve real-input, Rack V2/model-0, future-tree/release/review and save/load coverage. No production/schema/anchor edits or test weakening. Fresh-preflight/acquire Unity, run full guided-v2 and required compatibility coverage, publish HANDOFF.md, and return the exact success token or blocker.`
