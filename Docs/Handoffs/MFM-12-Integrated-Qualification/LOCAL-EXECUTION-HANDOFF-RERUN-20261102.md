# MFM-12 — Full Qualification Rerun — Local Execution Handoff — 2026-11-02

**Claim:** #225  
**Parent:** #91 / Browser Overall #200 / local orchestration #203  
**Purpose:** rerun the complete Multi-Scale Forestry Management V1 qualification after the approved #230 fit-replay repair.  
**Difficulty:** High — end-to-end Unity qualification, save/persistence compatibility, rendered UX, complete-loop witness, repeated-cycle provenance and independent review.  
**Best-suited primary:** SOL/CODEX high-reasoning local qualification lane.  
**Branch:** `qual/mfm-12-integrated-v2`  
**Worktree:** dedicated clean lane assigned by #203.  
**Exact starting HEAD:** use the live exact repin recorded on issue #225 after this handoff and the Manager coordination update are published. Do not use the stopped-run candidate or resume its lane state.  
**User action required:** paste the Browser Overall dispatch into the CCF local orchestrator.

## Read first

1. `Docs/Handoffs/MFM-12-Integrated-Qualification/TASK-PACKET.md` — full requirements/evidence authority.
2. `Docs/Handoffs/MFM-12-Integrated-Qualification/RERUN-ADDENDUM-20261102.md` — supersedes only the old blocked/repin status.
3. `Docs/Handoffs/MFM-12-Fit-Replay-Repair/MANAGER-REVIEW-20261102.md` — accepted repair boundary.
4. Issue #225 — exact starting integration HEAD and live status.
5. `Docs/Project/coordination/active-tasks.md` — current ownership/lock authority.

## Preflight

Before any Unity or verification work:

- refresh `Docs/Project/coordination/active-tasks.md` and issue #225;
- verify #225 is READY/REPinned for the full rerun;
- verify the exact starting HEAD equals the repin on #225;
- create/use one dedicated clean worktree and the branch above;
- inspect current Editor processes and machine-wide Unity lock state;
- mark #225 `[RUNNING]` with provider/model, branch, worktree and starting HEAD.

If the exact integration candidate has moved since the Browser repin, STOP and return for a fresh repin rather than qualifying a different lineage.

## Execution rule

This is a **fresh full qualification**, not continuation of the stopped run. Start the MFM-12 matrix from the beginning. The stopped run remains historical diagnostic evidence only.

#225 is qualification-only. Do not make production repairs under this claim.

## Required fresh evidence

Follow the complete A–P checklist and task packet. Fresh exact-candidate evidence must include at minimum:

- live Rack-derived source sample and coverage geometry/reasons;
- deterministic prescription derivation and the repaired source-sample fit replay;
- agreement/disagreement inspectability and Share/removal consequences before wider approval;
- candidate queue, confirm/reject/manual alternative and 3D player-judgement boundary;
- proposal/Fell separation, prescription approval, Pending work and Work Plan commitment boundary;
- ordinary contractor/economy execution and durable prescription provenance;
- save/load, pre-feature saves, malformed-save rejection-before-mutation, Legacy40 and frozen Reference Future protection;
- rendered 1280×720 and 1920×1080 evidence for sample/coverage/candidate/proposal/fit/approved/executed distinctions and no critical clipping;
- BroadleafTimber Legacy40 + Enlarged80, MixedDensity, pruning, ThreeSpecies history, guided-v2 and applicable PARITY-07/Combined/Rack regressions;
- current safeguard/economy/ecology/timber/deadwood/regeneration/light integration checks;
- provenance/history through at least two management cycles;
- one explicit complete Scenario One witness: initial forest → assess/access → sample teaching → extend treatment → review/fit/player judgement → prescription approval → Work Plan approval → execute → advance → Annual Review/history → later intervention → adapt/repeat → completion;
- fresh independent cross-provider architecture/spec-conformance review of the exact candidate and evidence package.

## Carried boundaries

- MFM-11 extension-state calibration remains accepted within its gameplay-calibration boundary.
- Post-treatment source-vs-current-Covered CI parity remains `NOT DEMONSTRATED` unless separately and explicitly proven.
- The three pure-Beech R² misses remain an accepted C-level limitation against the unchanged 0.40 criterion.
- The #230 fit repair reuses the existing `CheckFit` core; do not broaden or retune it during qualification.
- Verification pins repaired under #230 must remain on the accepted v24/current lineage.

## Stop conditions

Stop immediately and return to Browser Overall if:

- any production/spec defect appears;
- any repair would be required;
- protected Reference Future / Legacy40 / pure-Sitka anchors move unexpectedly;
- malformed/old saves mutate before required rejection;
- player judgement can be bypassed;
- unapproved proposals reach committed work;
- the repaired fit replay is missing, stale, bypassable or misleading in the live flow;
- a required current-product rendered gate remains red;
- complete-loop completion is unreachable;
- the exact candidate changes materially;
- independent review returns a blocker.

Do not weaken tests, retune production, silently rebaseline, touch #194, or make a Windows build.

## Durable return

Publish exactly:

- `Docs/Handoffs/MFM-12-Integrated-Qualification/HANDOFF.md`
- `Docs/Handoffs/MFM-12-Integrated-Qualification/INDEPENDENT-REVIEW.md`

At handoff mark #225 `[HANDED-OFF]` and link both durable files.

Successful local return:

`MFM-12 QUALIFICATION PASS / INDEPENDENT REVIEW APPROVED / READY FOR MANAGER ACCEPTANCE`

Do not self-issue:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

That remains Browser Overall's decision after review.
