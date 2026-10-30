# S1-C T-07 — Data Contract Stability Local Dispatch Handoff

**Date:** 2026-10-30  
**Claim:** #187  
**Parent:** #91 / local umbrella #106  
**Status:** READY — OFFLINE; NOT YET RUNNING  
**Mode:** focused verification/docs only by default; no Unity

## Purpose

Close the remaining S1-C prerequisite by proving that the existing management-history identity/data contract is stable, read-only and deterministic across current state and a genuine supported old-save source, without adding a second history authority or changing save semantics.

## Difficulty / model fit

**Difficulty: 2/5.**

Reason: this is a bounded offline verification task with no expected production implementation or Unity work. The main care points are evidence discipline: identify a genuinely supported old-save source rather than a synthetic fixture, prove read purity/counter stability, and catch identity misuse/renumbering with negative controls. Consequence is high if the contract is misread, but the technical execution surface is narrow.

**Ultra Code suitability: YES — very good fit.** This is probably the better of the two jobs for Ultra Code because it is deterministic repository/offline verification, can run without Unity, and should require mostly test/harness work plus evidence inspection. A strong code agent can complete it independently in parallel with #156.

Best-suited lane: Ultra Code / SOL-Codex-class high-reasoning verification worker; Claude Sonnet-class code worker is also suitable. Overall Manager/browser performs the final evidence review after handoff.

## Exact base / lane

- Exact released start: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`
- Branch: `verify/s1-c-t07-data-contract-v1`
- Worktree: `/media/jer/Videos/CCF-S1C-T07`
- Canonical task packet: `Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md`
- Repin handoff: `Docs/Handoffs/S1-C-Implementation-07/REPIN-HANDOFF-20261030.md`
- Claim #187 status at publication: `READY — OFFLINE; NOT RUNNING`

The previous `8fa83bff22ea1b4943689bee1074eed8eae65d2f` pin is superseded. The current released base includes accepted S1-D T-05 work but does not change the S1-C identity/query or save/schema/validator/hash contract under this task.

## Mandatory start preflight

Before editing verification/docs:

1. Refresh #91, #106, #187 and `Docs/Project/coordination/active-tasks.md`.
2. Confirm the branch/worktree is dedicated and clean.
3. Confirm exact starting HEAD `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` (or stop for Manager repin if integration has materially moved).
4. Confirm no other worker owns the same verification files.
5. Post `[RUNNING — OFFLINE]` on #187 with provider/model, branch/worktree and exact starting HEAD.

Do not take the Unity lock; this task does not require Unity by default.

## Required verification

Prove:

- management-event IDs are positive and unique for valid current and supported-old-save cases;
- existing event IDs pass through S1-C history and are never regenerated or renumbered by readers;
- work-order IDs remain stable across query/reload;
- resolution/event linkage uses existing authoritative identities;
- when an order object no longer exists, cancellation/history meaning is represented by the surviving authoritative event rather than a fabricated replacement identity;
- repeated history queries do not mutate events, work orders, next-ID counters, collections or fixture state;
- the history reader does not create a second ID generator/cache/store;
- `harvestJobId` remains a **resolution-year grouping label**, not a globally unique job identity and not an authority for fabricated per-order money;
- broader identities consumed by history remain pass-through rather than re-keyed;
- at least one **genuine supported pre-S1-C old save** preserves existing identities, leaves newer unavailable provenance absent/`not recorded`, and remains deterministic/read-only.

## Old-save evidence requirement

Do not assume a fixture merely named `v20-records` is genuine. Earlier S1-C qualification found a constructed fixture with that label carrying Rack V2 state and corrected the verification fixture rather than production behavior.

The handoff must name the exact genuine supported old-save source/identity and distinguish it from any constructed semantic test cases. If no genuine supported source can be established from the repository/evidence, stop and report that exact gap; do not invent one.

## Required negative controls

At minimum, tests must fail/catch:

- duplicate management-event ID;
- reader-side event renumbering/re-keying;
- treating `harvestJobId` as a unique job/per-order settlement authority;
- history/query mutation of counters/collections/state.

Use additional focused mutations if they materially strengthen AC-10 without broadening scope.

## Scope / stop conditions

Expected owned surface: focused verification under `Tools/Verification/S1CManagementHistory/` or the narrowest existing sibling plus the T-07 handoff/docs.

Do not change:

- save version/schema/migration/validator/hash/reference behavior;
- `WorkPlanView.cs`, `AnnualReviewView.cs` or `ScenarioOneUiRoot.cs`;
- MFM provenance architecture;
- production identity generation/repair rules;
- history UI/query/store architecture;
- Windows build.

If faithful verification reveals a genuine production defect or would require Unity/runtime production changes, stop and publish the exact failing witness for a separate bounded repair assignment.

## Required handoff

Publish:

`Docs/Handoffs/S1-C-Implementation-07/HANDOFF.md`

Include exact start/end SHAs, model/provider, branch/worktree, changed files, commands/pass-fail counts, current and genuine-old-save identities, negative-control results, read-purity/counter fingerprints, evidence classification and clean worktree return.

Expected success token:

`S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW`

This does not itself release S1-C T-08; Overall Manager must first inspect and accept the evidence.

## Copy/paste dispatch line

`Run S1-C T-07 from Docs/Handoffs/S1-C-Implementation-07/LOCAL-DISPATCH-HANDOFF-20261030.md under claim #187, starting from exact ce6737146a7aa9ea953d7db0ca93b103f6c2c996 after fresh active-tasks/claim/worktree preflight. Mark [RUNNING — OFFLINE] with provider/model/branch/worktree/start. Verification/docs only by default: prove stable event/work-order identities, append-only/pass-through history meaning, read purity and counter stability, harvestJobId as resolution-year grouping only, a genuine supported old-save case, and negative controls for duplicate IDs, reader renumbering, harvestJob misuse and query mutation. Do not change save/schema/UI/MFM architecture or take Unity automatically. Publish HANDOFF.md and return S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW or the exact blocker. No Windows build.`

## User action required

Dispatch the copy/paste line to one offline code/verification worker. It can run concurrently with #156 MFM-08 Tier-2.