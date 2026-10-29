# Browser Lane 06 — Next-wave reviews, MFM-08 / S1-D T-05 readiness and sequencing

Parent: #91 PLAYER-TEST-OVERNIGHT-01  
Browser claim: #127  
Execution surface: separate ChatGPT Game Dev browser chat with GitHub/project-source access  
Status at publication: PREPARED FOR DISPATCH — NOT RUNNING  
No production ownership. No Windows build.

## Purpose

Continue all browser-safe Scenario One player-test-readiness work after MFM-07 Manager approval. This lane should keep the next local Code workers unblocked by reviewing returned handoffs, reconciling ownership/dependencies, and preparing exact just-in-time packets for MFM-08, S1-D T-05 and later tasks.

Do not perform C#/Unity/save/rendered/integration execution here. Convert every local-execution residual into a precise Code packet/claim instead.

## Starting accepted state

Refresh everything before relying on this snapshot.

- Parent orchestrator: #91.
- Local Code umbrella: #106.
- Browser Lane 05 #122 is completed and superseded by this lane.
- MFM-06 is integrated/qualified on exact production candidate `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- MFM-07 #121 is Manager-APPROVED on exact production candidate `d4188b33e58b0fe67504a87167692d01b226c84e`.
- MFM-07 pushed branch `feature/mfm-v1-proposal-approval-boundary-v1` has docs-only successor `01755c47d537202e9cf479545be527d43121298f`.
- MFM-07 is not yet integrated into the umbrella.
- #125 MFM-07 integration is PREPARED FOR DISPATCH.
- #126 PARITY-08 save/deterministic compatibility is PREPARED FOR DISPATCH.
- #123 S1-C T-04 provenance labelling is PREPARED FOR DISPATCH.
- #124 S1-D T-04 guide monotonicity is PREPARED FOR DISPATCH.

Never treat a prepared claim as a live worker. A live executor must mark the matching claim RUNNING with actual model/provider/branch/worktree/start SHA.

## First refresh

Before substantive work, inspect:

1. #91 latest ledger.
2. #106 local execution lane.
3. #121, #122, #123, #124, #125, #126 and #127.
4. `Docs/Project/coordination/active-tasks.md`.
5. Current `main` and `integration/scenario-one-player-test-ready-v1` refs.
6. Approved specs and task/checklist files for MFM V1, three-species parity P0, S1-C and S1-D.
7. Any newer published feature handoffs/branches that appeared after this packet.

Mark #127 `[RUNNING]` with the actual browser model/provider before substantive edits to coordination/docs.

## Browser authority

You may:

- inspect GitHub branches, commits, diffs, issues, handoffs and approved specs;
- review returned local Code work where the evidence is GitHub-readable;
- issue APPROVED / NEEDS CHANGES / REJECTED for bounded handoffs when evidence is sufficient;
- maintain child-claim and dependency sequencing;
- reconcile `active-tasks.md` when you are the current authorised browser/Manager writer;
- publish browser-safe task packets, review packets and coordination amendments;
- audit source/static ownership/API boundaries;
- coordinate independent reviews and model routing;
- prepare final Tier-3/final-review packets when dependencies are actually ready.

You may not:

- edit production C# or serialized Unity assets;
- run Unity, compiler, saves, runtime or rendered qualification;
- acquire the Unity lock;
- integrate branches locally;
- claim local cleanliness/runtime/save/rendered success from chat summaries alone;
- invent product decisions or empirical precision;
- weaken tests or protected anchors;
- build Windows.

## Workstream A — return review for #125 MFM-07 integration

When #125 returns:

1. Inspect the published `Docs/Handoffs/MFM-07/INTEGRATION-HANDOFF.md` and exact pre/input/post SHAs.
2. Confirm the integrated tree contains the Manager-approved MFM-07 production candidate and no unrelated feature absorption.
3. Check the reported post-merge compile, MFM-07 runtime, MFM-06 save, hash/no-rebaseline and interaction results.
4. Preserve the distinction between MFM-07 IMPLEMENTED/REVIEWED and the post-#125 INTEGRATED/VERIFIED state.
5. Issue APPROVED / NEEDS CHANGES / REJECTED.
6. If APPROVED, pin the exact post-MFM-07 integrated production SHA for every downstream packet.

Do not require a broader gate solely because it exists elsewhere; use the bounded #125 integration packet unless evidence reveals a real integration risk.

## Workstream B — PARITY-08 coordination/review

Claim #126 and `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/` own the next high-risk save/determinism slice once a live worker starts.

Browser responsibilities:

- keep single-writer ownership of `ForestSaveData` / save validation / load ordering / hash/reference surfaces isolated from other live tasks;
- ensure no parity-specific schema field/version bump is introduced;
- preserve unknown-species rejection-before-mutation, model 0/1/2 compatibility, v23 MFM coexistence, Legacy40, frozen Reference Future and protected Sitka/deterministic anchors;
- never accept a changed expected hash/fixture merely to make a test pass;
- review the implementation handoff when published;
- coordinate the required independent OpenAI 6.1 Sol/Codex review if the implementer is Anthropic;
- issue the Manager verdict before PARITY-09 is prepared/released.

Do not allow PARITY-08 and another save/validator writer to overlap.

## Workstream C — S1-C T-04 provenance labelling

Claim #123 may run independently if live ownership remains file-separable.

Browser responsibilities:

- review provenance semantics against S1-C authority and actual v22/v23 records;
- require labels to distinguish Recorded / Current / Derived / Reconstructed / Session-only / Not recorded honestly;
- reject invented historical values or current-state backfill into old years;
- inspect returned source diff and rendered-evidence handoff when published;
- keep S1-C history/presentation ownership separate from MFM work-order/proposal logic;
- issue Manager verdict and prepare T-06/T-07 only when the actual result is accepted.

## Workstream D — S1-D T-04 guide monotonicity

Claim #124 may run independently if teaching-sequence ownership remains exclusive.

Browser responsibilities:

- preserve existing stable lesson IDs;
- require monotonic progression only where persisted facts faithfully prove historical completion;
- reject PlayerPrefs/session-only review state as durable progression authority;
- keep this task ahead of MFM-09, which will touch the same curriculum/current-state seam;
- coordinate the independent OpenAI review required by the approved plan if the implementation is Anthropic;
- issue Manager verdict before MFM-09 is allowed to own the teaching sequence.

## Workstream E — MFM-08 readiness and the partial coverage-adapter residual

MFM-07 was accepted with an explicit residual: `TryReadPrescriptionWorld` currently supplies live stems, orders, confirmed rack geometry and stored coverage choices, but omits infrastructure masks and management features. That was acceptable for the bounded MFM-07 approval boundary. It must not silently become the shipped MFM-08/09 coverage truth.

Browser task:

1. Inspect the approved MFM spec/plan/tasks/checklist and MFM-03/04/05/07 handoffs for the accepted meaning of infrastructure masks and management features.
2. Inspect current source/API definitions for coverage masks/features and the existing adapters that already populate them elsewhere.
3. Determine whether MFM-08 can consume an existing authoritative adapter/API or whether a bounded Code prerequisite is required.
4. Distinguish implementation omission from product ambiguity:
   - if existing accepted rules clearly define what masks/features should be supplied, prepare a bounded Code prerequisite or include it in MFM-08;
   - if the accepted product sources do not define the intended exclusions, stop and surface a product decision rather than inventing one.
5. Preserve the accepted gameplay rule that proposals are not Fell marks and the player must inspect/judge in 3D.
6. Preserve visual distinction from Red Fell, Blue Crop Tree and Orange Rack.
7. Require rendered 1280x720 and 1920x1080 review plus interaction qualification in the local Code lane.

When ready, publish/update the exact MFM-08 claim and task packet with:

- purpose;
- difficulty 1–5 and reason;
- best model/provider;
- exact accepted base SHA;
- branch/worktree guidance;
- owned file/subsystem boundary;
- adapter prerequisite/resolution;
- protected invariants;
- verification matrix;
- success token;
- one copy/paste dispatch.

Do not start the production worker from this browser chat.

## Workstream F — S1-D T-05 readiness after MFM-07 integration

S1-D T-05 must consume the accepted MFM-07 pending-work/provenance API rather than create a second work/history system.

After #125 is APPROVED:

1. Pin the exact integrated MFM-07 base.
2. Re-read the S1-D T-05 packet and approved S1-D spec.
3. Inspect the current `WorkPlanView` / Annual Review / history ownership split.
4. Preserve presentation of at least Approved / Pending new / Pending carried over states as authorised by the spec/packet.
5. Keep copy non-causal and non-prescriptive where the evidence does not support causality.
6. Ensure S1-D T-05 does not absorb MFM-09 teaching insertion or S1-C history internals.
7. Prepare/release an exact Code claim only after ownership is clean.

## Workstream G — downstream sequencing

Continuously reconcile the shortest safe path through:

- MFM-08;
- S1-D T-05;
- S1-C T-06 and T-07;
- PARITY-08;
- MFM-09;
- MFM-10 species guard;
- PARITY-09;
- S1-C T-08;
- S1-D T-08/T-09;
- MFM-11 calibration impact;
- MFM-12 integrated verification/review;
- final Scenario One Tier-3 and final independent review.

Use just-in-time claims. Do not grant production ownership to far-ahead tasks merely because their packet exists.

Shared surfaces requiring explicit single-writer sequencing include:

- save/schema/validation/hash/reference;
- `ScenarioOneManager` work-order/prescription APIs;
- Work Plan presentation;
- Annual Review/history presentation;
- Scenario One teaching sequence/current lesson state;
- Stand Map/3D proposal/candidate presentation.

Overlap means stop and publish an ownership/API disposition before allowing parallel writers.

## Returned handoff rule

For every returned substantive Code handoff:

1. confirm the exact published branch/SHA and handoff path;
2. inspect the diff/source evidence that is browser-visible;
3. distinguish IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED;
4. issue APPROVED / NEEDS CHANGES / REJECTED;
5. update/close the matching claim as appropriate;
6. update #91 with a concise ledger entry;
7. immediately prepare the next executable packet when the dependency graph is clear.

Do not claim Unity/local cleanliness from GitHub alone; rely on the published handoff for local execution evidence and label it as such.

## Model routing

Use the user-approved prospective routing unless a newer direct instruction supersedes it:

- default bounded implementation/review: Claude Sonnet 5.5 Medium;
- complex cross-surface implementation: Sonnet 5.5 Ultra Code where available;
- difficult/high-risk reasoning or save/reference work: Claude Opus 5.5 Low when justified;
- OpenAI implementation/review: OpenAI 6.1 Sol;
- preserve independent/cross-provider review where the approved task or risk level requires it.

Do not add a new mandatory cross-provider gate unless the approved task/plan or a documented Manager risk decision actually requires one.

## Required closeout

When no further browser-safe work can proceed without local Code evidence, mandatory independent review, a product decision or user playtest, publish:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-06-RESULT.md`

Include:

- starting and ending published state;
- all reviews/verdicts;
- packets/claims created or updated;
- exact integrated/approved SHAs that became authoritative;
- unresolved product/ownership decisions;
- remaining local Code residuals;
- shortest remaining path to the Scenario One personal playtest candidate;
- exact user action required, including any live dispatches still needed;
- `DASHBOARD SYNC: NOT AVAILABLE` unless a supported dashboard publisher was actually used and verified.

Close #127 `[HANDED-OFF]` only when its browser-safe work is exhausted.

Terminal wording when appropriate:

`BROWSER NEXT-WAVE WORK EXHAUSTED — WAITING ON CODE EXECUTION / INDEPENDENT REVIEW / USER PLAYTEST`
