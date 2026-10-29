# PLAYER-TEST-BROWSER-LANE-01 — S1-D T-03 RackLesson residual reconciliation

## Identity

- Parent: #91 `PLAYER-TEST-OVERNIGHT-01`
- Browser lane: #105
- Feature claim: #103
- Execution/reconciliation lane: #106
- Browser inspection model/provider: OpenAI GPT-5.6 Sol, GitHub/browser-only manager lane.
- Integration inspected: `integration/scenario-one-player-test-ready-v1` = `aca74acacc565f72ab802177cf69ca6ebb54d637`.
- T-03 qualification head inspected: `8a92a6eddba7a98ca64dd10dc1db636bfe2a49ee`.

## Sources inspected

- #103 body/comments and the published T-03 handoff.
- GitHub compare `aca74ac..8a92a6e`.
- Approved Rack V2 specification and current D-064 authority context.
- The qualification evidence that the same RackLesson mismatch reproduces on clean integration base `aca74ac`.

## Static findings

1. The remaining RackLesson failure is `middle rack half-width mismatch: expected 8.00385, actual 7.995, tolerance 0.005`.
2. The same failure reproduces on clean integration base `aca74ac`; therefore it is not evidence that the T-03 teaching-footprint-cap production change caused a rack-geometry regression.
3. The `aca74ac..8a92a6e` compare contains the teaching-footprint-cap decoupling, verification helper, usable-bounds/history ratio work and null-guard qualification repair. It does not introduce a rack-geometry production change.
4. The approved Rack V2/D-064 product authority protects the Stand Map workflow, compatibility and physical entrance-marking behavior, but the inspected authority does not establish the literal `8.00385` middle-rack half-width as a protected accepted numeric anchor.
5. The #103 qualification handoff reports the live integration behavior as `7.995` and characterises `8.00385` as a stale expected geometry constant. That characterization is consistent with the base reproduction and source diff, but the exact current source expression still needs execution-lane confirmation before any harness edit.

## Browser verdict

**NEEDS T3 VERIFICATION-HARNESS RECONCILIATION; NO T-03 PRODUCTION CHANGE IS INDICATED BY THE AVAILABLE EVIDENCE.**

This does not waive RackLesson, does not authorise assertion weakening and does not classify T-03 as integrated/verified.

## Exact bounded packet for T3 Code claim #106

1. Refresh the exact integration branch, #91/#103/#105/#106 and live `active-tasks.md`; respect the Unity/execution lock.
2. On exact integration `aca74ac`, locate the authoritative production source expression used to compute the live middle-rack half-width. Independently recompute/observe its expected value. The current reproduced runtime value is `7.995`; do not assume that value is correct merely because the old harness fails.
3. Check the approved Rack V2 spec, D-064 compatibility rules, frozen Reference Future v1 rules and any protected rack-verification anchors for an explicit requirement that the value remain `8.00385`.
4. If the production expression and accepted authority support `7.995` and no protected `8.00385` anchor exists, patch **only** the stale RackLesson verification expectation (or its exact derived expected-value calculation) as narrowly as possible. Do not change rack geometry production code and do not broaden tolerance to hide the discrepancy.
5. If the source/authority instead shows that `8.00385` is the required value, **stop** and report a pre-existing production defect on the integration base. Do not silently rewrite the test.
6. Rerun RackLesson on the clean integration base after the reconciliation, then rerun it on the exact T-03 candidate/descendant.
7. Rerun the T-03 focused teaching-footprint verification plus the relevant Rack V2 Guided/Sequential/Combined-HUD/geometry assertions required by the current qualification contract. Preserve existing assertion strength.
8. Only after the pre-existing RackLesson residual is correctly classified/reconciled and the required candidate gates are green may T-03 proceed to integration review. No Windows build.

## Next executable action

#106 performs the exact source-expression/authority check first, then either makes a verification-only RackLesson expectation repair and reruns the gates, or reports a pre-existing production defect if `8.00385` is actually protected.