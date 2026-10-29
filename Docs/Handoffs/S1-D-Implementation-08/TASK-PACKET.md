# S1-D T-08 — Multi-cycle witness and compatibility check — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-08 — Multi-cycle witness and compatibility check
- **Parent:** #91 PLAYER-TEST-OVERNIGHT-01
- **Purpose:** run the approved three-cycle Unity witness that proves repeated intervention remains reachable, understandable, save/load compatible and solvent without changing coefficients or protected anchors.
- **Difficulty:** **High** — integrated multi-cycle Unity/save/compatibility qualification across S1-D, S1-C history and ordinary Work Plan execution.
- **Recommended executor:** **OpenAI 6.1 Sol** in the T3 Code local lane; this leaves Claude available as the independent T-09 reviewer.
- **Suggested branch:** `verify/s1-d-multicycle-witness-v1`
- **Suggested worktree:** `/media/jer/Videos/CCF-S1D-T08`
- **Claim:** do not create/start until every dependency gate is satisfied. At release create `[RUNNING] S1-D T-08 — multi-cycle witness and compatibility` with actual model/provider, branch/worktree and starting HEAD.
- **User action required:** none.

## Hard start gate

T-08 remains BLOCKED until all are true and published on one GitHub-refreshable integration lineage:

1. T-02 integrated.
2. T-03 published-integrated and its qualification evidence accepted.
3. T-04 guide monotonicity integrated and independently reviewed.
4. T-05 previous/proposed presentation integrated and rendered-qualified.
5. T-06 management-event ID integrity integrated/qualified.
6. Any S1-C save/history work required by the current lineage, including #104 v22 closure, is published-integrated and stable.
7. Live `active-tasks.md`, Unity storage floor, lock and processes are refreshed immediately before execution.

Do not treat local-only merge reports as satisfying this gate.

## Required witness

Run one controlled Scenario One witness with **three real management cycles**, including:

- idle year(s) between interventions;
- at least one stale/carried pending order;
- save/load at each required stage;
- Annual Review/history observation at each cycle as required by the integrated UI;
- completion achieved on the third cycle under the ordinary completion rules;
- continued management after `Completed`, proving completion is not a terminal lockout;
- D-057 solvent/economy witness re-run on the exact integrated candidate;
- ordinary Work Plan approval/execution remains authoritative.

Use the exact integrated teaching/guide behaviour. Do not bypass T-03/T-04/T-05 UI semantics with direct state mutation except where a verification harness needs deterministic setup; any such setup must be documented and must not replace the real user-path assertions.

## Required acceptance questions

The witness must answer, with recorded evidence:

1. Can the player perform a second, third and later intervention without first-cycle teaching residue blocking Approve/Advance?
2. Does the guide remain monotone across drift and reload while preserving pre-completion teaching behaviour?
3. Are previous, approved, new-pending and carried-pending work understandable through the existing S1-C/S1-D surfaces?
4. Do save/load and event chronology remain deterministic/non-mutating where required?
5. Is completion still reachable on repeated management and does management remain possible afterward?
6. Does the default repeated-intervention path remain solvent under the accepted D-057 witness without coefficient changes?

## Protected regressions / anchors

Run the narrow regression set applicable to the integrated candidate, including:

- Scenario One completion / N3 normalised-rule hash;
- S1-D T-02 cycle-state checks;
- T-03 cap / RackLesson / MenuTutorial / TeachingCopy where still applicable;
- T-04 guide monotonicity checks;
- T-05 presentation/history checks;
- T-06 save-hardening/event-ID checks;
- S1-C history/query/Annual Review regressions;
- Legacy40 / Reference Future v1 / protected deterministic hashes required by the integrated save lineage.

Never rebaseline an anchor merely because the integrated run differs. Attribute first and stop on a genuine protected-contract change.

## Editing rule

This is a qualification task, not a feature-implementation task.

- Verification-harness additions needed to express the witness are allowed only if they preserve accepted assertions/thresholds and are reviewed as verification changes.
- **No production fix is authorised by this packet.** If the witness exposes a production defect, stop and return a bounded defect packet to the Manager/#91.
- Do not add thresholds to make the gate pass.

## Evidence

Record:

- exact candidate SHA and ancestry;
- exact Unity version/lane/preflight;
- commands and exit codes;
- save checkpoints and relevant hashes;
- cycle-by-cycle state table;
- screenshots where presentation/reachability matters;
- cash/solvency witness values with existing evidence classification;
- protected-anchor comparison;
- any uncertainty and whether it was resolved.

## Stop conditions

Stop with a precise blocker if:

- any T-02..T-06 dependency is not published-integrated;
- completion becomes unreachable/unrecoverable without changing coefficients or product scope;
- a protected save/reference/determinism contract moves unexpectedly;
- the only way forward would weaken a gate or bypass normal Work Plan execution;
- Unity lock/storage ownership is unsafe.

## Handoff

Publish:

`Docs/Handoffs/S1-D-Implementation-08/HANDOFF.md`

Success token:

`S1-D T-08 MULTI-CYCLE WITNESS PASS — READY FOR INDEPENDENT SPEC REVIEW`

Failure token:

`S1-D T-08 NEEDS CHANGES — BOUNDED BLOCKER PUBLISHED`

## Copy/paste dispatch line

`Run S1-D T-08 from Docs/Handoffs/S1-D-Implementation-08/TASK-PACKET.md only after T-02..T-06 are all published-integrated on one exact candidate. Use OpenAI 6.1 Sol in the local T3 lane. Run the three-cycle + idle-year + stale-order + save/load + completion + post-completion + D-057 solvent witness and protected regressions. Verification-only; no production fixes, no rebaseline, no Windows build.`
