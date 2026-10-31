# MFM-08 Integration 01 — Task Packet

**Claim:** #205  
**Parent feature:** #156  
**Browser Manager:** #200 / resume #202  
**Local orchestrator:** #201 / resume #203  
**Classification:** PACKET ONLY — bounded integration/verification repair, no new product behavior

## Purpose

Integrate the Manager-approved MFM-08 production delta onto the exact current Scenario One integration lineage and prove the replay still composes with the later accepted v24/D-066/parity/S1-D state.

This task does not authorize new MFM behavior, UI redesign, schema changes, calibration changes or protected-gate relaxation.

## Exact inputs

- Current published integration: `integration/scenario-one-player-test-ready-v1` @ `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.
- Approved MFM-08 production candidate: `e5fb424635f16182748efcb0539e8a49539495a9`.
- Common merge base: `cfdc93bf3943125d21570bc5749a9338f52d1cce`.
- Approved production range: exact delta `cfdc93bf3943125d21570bc5749a9338f52d1cce..e5fb424635f16182748efcb0539e8a49539495a9` (3 commits).
- #196 verification-only patch: `eef1bdf9` — **not part of the production replay by default**.
- #196 Manager review: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.

## Browser overlap evidence

The current integration is 29 commits ahead of the common base. Browser compare shows those later integration commits do not modify the two MFM-08 pre-existing production paths:

- `Assets/ForestPrototype/UI/Map/StandMapView.cs`
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`

The other MFM-08 production files are additions. This reduces expected merge risk but does not substitute for local replay/import/compile evidence.

## Ownership / sequencing

Before starting:

1. refresh live `Docs/Project/coordination/active-tasks.md` and #156/#196/#199/#203/#205;
2. refresh the actual published integration ref and confirm it still equals `ce673714...`; if it moved, STOP and return to #200 for repin;
3. establish a dedicated clean writable integration worktree/branch;
4. confirm no overlapping MFM-08/StandMap/TreeInspection/UI writer is active;
5. fresh-preflight local cleanliness, free space, Editor processes and the authoritative Unity lock before any Unity phase;
6. mark #205 `[RUNNING]` with actual provider/model, branch, worktree and starting HEAD.

#199 Option 1 is Manager-authorised but must not become an overlapping production UI writer while #205 owns the integration surface.

## Integration work

Start from exact `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.

Replay only the exact approved MFM-08 delta from `cfdc93bf...` through `e5fb4246...` onto that lineage. A clean cherry-pick/replay of the three approved commits is preferred when it preserves the candidate delta exactly. Equivalent integration is acceptable only if the resulting production/offline-verification diff is demonstrated equivalent.

Do not silently absorb:

- #196 verification-only commit `eef1bdf9`;
- unrelated handoff/docs commits from verification branches;
- #199 partial fixture branch `42742a8a`;
- new product/UI/schema/calibration changes.

If any source conflict, semantic conflict, duplicate type/API or unexpected overlap appears, stop with the exact conflict. Do not guess-resolve.

## Post-replay verification

On the exact integrated candidate:

1. inspect the final diff against `ce673714...` and prove it matches the approved MFM-08 delta;
2. Unity import/compile — no compile errors;
3. run the focused MFM-08 treatment-review qualification sufficient to prove sample/coverage/candidate/proposal/3D review behavior still works on the later integration lineage;
4. run the complete authoritative `RackV2MapPlanning` gate using the Manager-approved #196 contract (original five tabs + additive Treatment review in V2; historical five-tab branch retained);
5. run proportionate save/Reference Future/Legacy compatibility checks affected by the integration, with no rebaseline;
6. if the replay touches or exposes a later current-lineage behavior conflict, stop and report it rather than modifying unrelated systems;
7. restore disposable/editor side effects, release the Unity lock normally and return the lane clean.

The already accepted 353/373 MFM-08 feature evidence need not be mechanically rerun in full unless the integration changes or failures make that necessary. The purpose here is post-replay composition, not redoing all prior feature qualification.

## Protected constraints

- Frozen Reference Future v1 bytes/semantics unchanged.
- Legacy40 compatibility unchanged where applicable.
- No save/schema/anchor/calibration/economy/ecology changes.
- No test weakening or protected expected-value rebaseline.
- No MFM-09 teaching implementation.
- No #199 production layout repair in this ownership window.
- No Windows build.

## Handoff

Publish:

`Docs/Handoffs/MFM-08-Integration-01/HANDOFF.md`

Include:

- exact pre/input/post SHAs;
- integration method and exact commit/range replayed;
- conflict status;
- final changed-file scope/equivalence evidence;
- Unity import/compile result;
- focused MFM-08 result;
- RackV2MapPlanning result/check count;
- compatibility checks;
- lock/Editor/worktree cleanup state;
- any residuals;
- explicit statement that #196 verification-only and #199 partial branches were not silently absorbed.

At handoff mark #205 `[HANDED-OFF]` and return one exact token:

Success: `MFM-08 INTEGRATED ON CURRENT SCENARIO ONE LINEAGE — READY FOR MANAGER REVIEW`

Blocker: `BLOCKED — MFM-08 INTEGRATION CONFLICT OR REGRESSION`

## Difficulty / routing

**Difficulty: Medium.** The Git overlap appears clean, but Unity composition against 29 later commits must be observed.

**Best-suited provider:** DeepSeek may orchestrate/perform the bounded mechanical integration and verification where capable. Escalate to Claude Sonnet 5.5 Medium only for a genuine conflict/architecture-sensitive issue or if DeepSeek cannot complete the local Unity/code work. Do not use Opus for a mechanical clean replay.

## Copy/paste dispatch line

`Run MFM-08 Integration 01 under claim #205 using Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md. Refresh live active-tasks and exact integration first. Start from ce6737146a7aa9ea953d7db0ca93b103f6c2c996, replay only the approved cfdc93bf3943125d21570bc5749a9338f52d1cce..e5fb424635f16182748efcb0539e8a49539495a9 MFM-08 delta, and do not absorb #196 eef1bdf9 or #199 partial work. Stop on unexpected conflict. Fresh-preflight/acquire Unity, run import/compile plus focused MFM-08 and full RackV2MapPlanning post-replay qualification and proportionate compatibility checks, publish Docs/Handoffs/MFM-08-Integration-01/HANDOFF.md, and return the exact success/blocker token. No Windows build.`
