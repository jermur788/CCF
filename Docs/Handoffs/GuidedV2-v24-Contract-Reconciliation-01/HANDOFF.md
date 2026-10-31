# Guided-v2 v24 / D-066 Contract Reconciliation 01 — Handoff (#198)

**Success token:** `GUIDED-V2 V24 CONTRACT RECONCILED — READY FOR MANAGER REVIEW`

## Claim and execution

| Field | Value |
|---|---|
| Claim | #198 (parent #201/#91) |
| Executed by | #201 long-running lane; DeepSeek V4.1 Flash as orchestrator/default worker |
| Branch | `verify/guided-v2-v24-contract-reconcile-v1` |
| Exact start | `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` |
| Verification commits | `bac20456` (v24/latch), `1b8127a2` (V2 entrance-marking step) |
| Unity lane | `/media/jer/ZX20/CCF-U1` |
| Lock | `4953262b6e1942538e34e6c0029dc78a` (first run), `41efe1bde2ac4d0c852e33f756c7528b` (final runs), both released normally |
| Evidence root | `/media/jer/Files/CCF/Verification/ScenarioOneGuidedV2V24Reconcile-20261031T093000Z/` |

## Scope

Verification-only reconciliation. No production, schema, anchor, scene, prefab, asset, calibration, ecology, economy or Reference Future change. The exact production candidate remains `ce673714`.

Changed files (2, +22/-7):

- `Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs`
- `Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs`

## Stale-contract inventory and authority mapping

Authority used: Decision Log D-066; `specs/scenario-one-repeated-interventions-v1/spec.md` FR-B1/FR-B1a…c, AC-3, AC-16, AC-17; `plan.md` §2–§4; the accepted RACK-V2-09 field-marking flow already present in `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs` and `ScenarioOneManager.RackPlan.cs`.

1. **`ForestSaveData.CurrentVersion == 23` (two assertions)** → **v24**. D-066 added the single durable `firstGuidedRackReviewComplete` latch and the save version is v24 on this lineage. Messages updated to say v24.
2. **`RoundTripReviewed` expected `Stage(4, "requires fresh rack review")` after save/load and then repeated the session review.** → **stale pre-D-066 acceptance.** Under D-066 the first guided sample/review completion is monotone and survives save/load; the guide must not regress to `response`. The revised check:
   - proves the latch is captured in the save (`data.scenarioOne.firstGuidedRackReviewComplete`);
   - loads and proves the latch survives;
   - proves session review detail is still non-persistent (`RackReviewComplete == false` after `ReloadProgress`, which resets it);
   - asserts the expected later stage directly, with no fresh review.
   This preserves save/load determinism and does not accept an arbitrary index: the stage index is the same one derived from the accepted facts at that point in the test.
3. **"V2 confirmed rack lesson clearly requests annual advance" failed on a fresh V2 world.** → **stale harness state, not a production defect.** The accepted V2 flow (RACK-V2-09) requires all five confirmed rack entrances to be marked before approving the rack work, and the live V2 instruction correctly asks for that marking while `MarkedRackEntranceCount < 5`. The harness confirmed the plan but never performed the accepted field step. It now marks each entrance through the authoritative `ScenarioOneManager.MarkRackEntrance` API and asserts `MarkedRackEntranceCount == RackPlanTargetCount` before `ApprovePendingWork()`; the opening lesson then correctly requests the annual advance. This adds coverage; it does not relax the assertion.

No assertion was deleted, skipped, rebaselined or relaxed. The original real-input/native intent, the explicit V2/model-0 split, future-tree/release/review coverage and protected Rack assertions remain.

## Verification results

### guided-v2 — PASS

- Run `guided-v2-r2/guided-v2.log`
- **535 `GUIDED_PASS`, 0 `GUIDED_FAIL`, 0 `error CS`**
- Terminal marker `GUIDED_ONBOARDING_PASS`
- 48 evidence captures
- SHA-256 `3e27e4750c5d873d7017cf22a15d7829373c2b443df44ed5c662f1881839af58`
- New contract evidence observed: `all five V2 rack entrances marked before approval`, `V2 confirmed rack lesson clearly requests annual advance`, and `D-066 historical teaching latch captured/survives` at all three post-latch round trips.

### guided-legacy (model-0 companion) — PASS

- Run `guided-legacy-r2/guided-legacy.log`
- **569 `GUIDED_PASS`, 0 `GUIDED_FAIL`, 0 `error CS`**
- Terminal marker `GUIDED_ONBOARDING_PASS`
- 62 evidence captures
- SHA-256 `b764de92dec5bef7d5f62db67f391f2c288a47e66f9e023899228bd00258c715`

### Failed attempts, classified

- **guided-v2 r1:** stopped on the genuinely stale `V2 confirmed rack lesson clearly requests annual advance` state (one `GUIDED_FAIL`); this produced the second verification-only patch (`1b8127a2`). Not a production failure.
- **guided-legacy r1:** the real-input driver failed before confirmation: `RuntimeError: State wait failed` while waiting for the pointer swivel (`selections=1`, `reselections=0`, `cancellations=0`, `confirmed=0`). Classified **ENVIRONMENT/RUNNER (input-driver timing)** — no model escalation. Re-run alone (`guided-legacy-r2`) passed with the same tree. The failed run and driver log are preserved under `guided-legacy/`; this is the same order/timing sensitivity already carried as workflow debt for the guided real-input gate.

## Lane / lock cleanup

- Staged disposable sources and generated `.meta` files removed by both runners; verified absent from `Assets/ForestPrototype/`.
- Unity/URP Editor side effects (`.vscode/settings.json` and five SectionFive materials: `Bilberry_Leaves`, `Bilberry_Stems`, `Forb_Flower_Centre`, `Forb_Leaves`, `Forb_Stems`) were restored byte-exactly with `git checkout -- <paths>`; lane clean at `1b8127a2`.
- Final lock released normally by matching owner/task/lock id; live status `free`, metadata `null`, no Unity Editor PIDs. No Unity/Xephyr/openbox process remains.
- No Windows build.

## Manager decision requested

#200 to review the exact verification diff and the two passing gate results. #198 execution is complete: `[HANDED-OFF]`. If the Manager accepts the reconciliation, #199 remains the separate production HUD/focus repair.
