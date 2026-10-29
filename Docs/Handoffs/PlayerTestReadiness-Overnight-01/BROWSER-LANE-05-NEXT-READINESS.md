# PLAYER-TEST-BROWSER-LANE-05 — Next Readiness / Return Review

Parent: #91 `PLAYER-TEST-OVERNIGHT-01`
Claim: #122
Lane: separate Game Dev browser chat / GitHub + project-source access only
Status: PREPARED FOR DISPATCH — NOT RUNNING UNTIL A LIVE CHAT RECEIVES THIS HANDOFF

## Purpose

Use a separate browser chat to complete all useful non-local work that can run in parallel with MFM-07 #121 and immediately consume its return. The browser lane should reduce idle time by making the next Code tasks exact, ownership-safe and dependency-correct without claiming C#/Unity/runtime evidence.

## Current accepted state

- MFM-06 v23 save/provenance foundation is integrated and bounded-qualified.
- Exact Unity-qualified post-MFM-06 production candidate: `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- Published integration branch has documentation-only successor: `7608f02cb8da1fabd8b78fd5aaf45b0c32086f67`.
- MFM-07 claim #121 is released for Claude Code, preferred Sonnet 5.5 Ultra Code, rooted from exact production base `b971f4d...`.
- MFM-07 mandatory inherited rule: before any prescription-generated order is written, validate referenced prescription + coverage unit + Crop Tree; mismatch/staleness fails closed with no partial mutation.
- No Windows build is authorised.

Always refresh live state before acting. Do not assume this snapshot is still current once #121 advances.

## Difficulty

Medium–High. This is not implementation, but it spans several approved specs and shared ownership surfaces. The main risk is releasing overlapping writers or preparing packets against stale save/provenance interfaces.

## Model / provider

Primary: current capable browser Manager model.

If a delegated independent browser review is materially useful:
- default Claude Sonnet 5.5 Medium;
- escalate to Claude Opus 5.5 Low only when complexity or reviewer independence justifies it;
- OpenAI review may use the strongest suitable OpenAI browser model.

No model choice grants production ownership.

## Mandatory preflight

Before substantive work:

1. Refresh #91, #106, #113, #120, #121 and this claim #122.
2. Refresh `Docs/Project/coordination/active-tasks.md`.
3. Refresh current `main` and published `integration/scenario-one-player-test-ready-v1`.
4. Read the approved specs and task/checklist files for:
   - `specs/multiscale-forestry-management-v1/`;
   - `specs/scenario-one-three-species-parity-p0/`;
   - `specs/scenario-one-review-history-v1/`;
   - `specs/scenario-one-repeated-interventions-v1/`.
5. Refresh the current packets for PARITY-08, S1-C T-04 and S1-D T-04 rather than relying on historical wording.
6. Mark #122 `[RUNNING]` with actual model/provider before substantive work.

## Workstream A — PARITY-08 readiness

Goal: make the three-species save/determinism compatibility task executable without colliding with MFM-07.

Inspect:
- current PARITY-08 task packet and approved requirements;
- MFM-06 v23 save/provenance API and validator ownership visible in published sources/handoff;
- likely save/hash/validator surfaces PARITY-08 needs;
- MFM-07 #121 scope and any returned diff once available.

Required output:
- readiness verdict: READY / WAIT / NEEDS PACKET REPAIR;
- exact prerequisite list;
- exact likely shared files/subsystems;
- whether it can run in parallel with MFM-07 or must wait;
- exact recommended model/provider and difficulty 1–5;
- a bounded Code claim/packet only when ownership is clear.

Do not release PARITY-08 if MFM-07 unexpectedly edits save schema/validation/hash surfaces. The approved expectation is that MFM-07 consumes v23 without a schema bump; any deviation must be reconciled before parallel execution.

## Workstream B — S1-C T-04 readiness

Goal: prepare provenance labelling / Annual Review-history work against the actual v23 provenance model.

Inspect:
- approved S1-C T-04 scope and acceptance criteria;
- existing Annual Review/history ownership;
- MFM-06 v23 provenance terminology/fields that are actually supported;
- likely overlap with S1-C T-06/T-07 and MFM-07.

Required output:
- source/spec mapping for every user-facing provenance label;
- exact likely files/interfaces;
- single-writer conflict analysis;
- readiness verdict and exact base dependency;
- recommended difficulty 1–5 and agent/provider;
- exact Code handoff/claim if genuinely unblocked.

Do not invent new provenance semantics. Use only accepted MFM-06 state and S1-C authority.

## Workstream C — S1-D T-04 readiness

Goal: prepare repeated-intervention teaching-sequence work so it can execute independently where safe and land before MFM-09.

Inspect:
- approved S1-D T-04 packet/spec;
- current teaching sequence and learning-objective interfaces visible in repo;
- exact relationship to MFM-09;
- whether it overlaps the MFM-07 proposal/pending-work surface.

Required output:
- exact files/interfaces likely owned by T-04;
- confirmation that it does not expand into MFM logic;
- sequencing recommendation relative to MFM-07, S1-D T-05 and MFM-09;
- readiness verdict;
- difficulty 1–5 + best agent/provider;
- exact Code packet/claim when safe.

## Workstream D — post-MFM-07 sequencing

Maintain the cross-feature dependency graph after #121 lands.

At minimum reconcile:
- MFM-08;
- S1-D T-05;
- PARITY-08;
- S1-C T-04/T-06/T-07;
- MFM-09;
- any save/validator/history/Work Plan/ScenarioOneManager shared files.

Rules:
- one primary writer per subsystem/file set;
- if a later task needs a file currently owned by #121, it waits;
- S1-D T-05 waits for the MFM-07 pending-work/provenance API to be stable;
- MFM-09 waits for MFM-07/08 and the required S1-D teaching/Work Plan seams;
- PARITY-08 must not overlap an unexpected MFM-07 save-path edit;
- S1-C work may proceed in parallel only when files/interfaces are proven non-overlapping.

Publish a concise sequencing table or amendment and update `active-tasks.md` only if this browser lane is the authorised coordination writer at that moment.

## Workstream E — review MFM-07 return

When #121 returns:

1. Refresh #121, its handoff, actual published diff/commit identity and live coordination.
2. Verify the exact starting base is `b971f4d...` or an explicitly approved successor.
3. Check the implementation against the approved MFM-07 invariants:
   - proposal state separate from `TreeMarkType.Fell`;
   - opening/approving Work Plan does not import unapproved proposals;
   - prescription approval creates pending ordinary work only;
   - Work Plan approval remains the commitment gate;
   - manual state wins conflicts;
   - invalid/stale prescription + coverage unit + Crop Tree linkage fails closed before any order/world mutation;
   - MFM-06 v23 provenance is reused rather than duplicated;
   - no unauthorised schema bump/broadleaf extrapolation/hidden automation.
4. Inspect available offline/Unity/runtime evidence without claiming anything not actually evidenced.
5. Issue one verdict: `APPROVED`, `NEEDS CHANGES`, or `REJECTED`.
6. Update/close #121 as appropriate.
7. Immediately prepare the next executable Code handoff(s) that are truly unblocked, with exact base, difficulty, preferred agent/provider, branch/worktree guidance, success token and dispatch line.

## Browser hard boundary

Do not:
- edit production C# or serialized Unity assets;
- assert local dirty/clean/lock/process state unless published evidence supports it;
- launch Unity or perform save execution;
- integrate branches/worktrees;
- claim compiler/runtime/rendered/Tier-3 PASS from browser inspection alone;
- invent product decisions;
- weaken tests or rebaseline protected hashes;
- build Windows.

## Publication requirements

For every substantive result:
- record it on #122;
- keep #91 ledger current where appropriate;
- publish durable Markdown under `Docs/Handoffs/...` when the result is a review, sequencing amendment or task packet;
- distinguish PREPARED / DISPATCHED / RUNNING / HANDED-OFF truthfully;
- remember GitHub is a coordination record, not a live message bus.

## Completion condition

Continue through all browser-safe work above until one of these is true:
- waiting only on local Code/Unity execution;
- blocked on a genuine product decision;
- blocked on unresolved ownership conflict;
- waiting on user playtest.

Close #122 as `[HANDED-OFF]` with a concise result file summarising what was prepared/reviewed and the next executable Code tasks.

Suggested success token:

`BROWSER LANE 05 COMPLETE — NEXT CODE TASKS RELEASED OR GATED`

## Copy/paste dispatch

Run claim #122 using `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-05-NEXT-READINESS.md`.

Work continuously through all browser-safe readiness work while MFM-07 #121 executes: PARITY-08 readiness, S1-C T-04 readiness, S1-D T-04 readiness, post-MFM-07 single-writer sequencing, and review of the MFM-07 return when it appears.

Refresh #91/#106/#113/#120/#121/#122, live `Docs/Project/coordination/active-tasks.md`, current main/integration refs and the approved specs before acting. Mark #122 `[RUNNING]` with your actual model/provider.

Do not perform C#/Unity/save/integration work. Prepare exact Code claims/packets only when dependencies and ownership are genuinely clear. When #121 returns, issue APPROVED / NEEDS CHANGES / REJECTED and immediately prepare the next executable Code work.

No Windows build.