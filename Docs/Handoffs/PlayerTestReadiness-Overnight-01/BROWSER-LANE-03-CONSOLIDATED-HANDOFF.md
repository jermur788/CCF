# PLAYER-TEST-BROWSER-LANE-03 — Consolidated browser-safe continuation

## Task identity

- **Parent orchestration:** #91 — PLAYER-TEST-OVERNIGHT-01
- **Browser continuation claim:** #118
- **Execution surface:** separate ChatGPT chat inside the Game Dev project, using browser/GitHub/project-source tools only
- **Purpose:** execute every remaining Scenario One player-test-readiness task that can be completed honestly without local C#/Unity execution, and keep converting local-execution residuals into exact T3 Code handoffs.
- **Difficulty:** Medium–High. The work is mostly review/orchestration rather than coding, but it crosses multiple approved specs and must preserve save/reference/ownership boundaries.
- **Parent orchestrator:** DeepSeek #91 remains the programme orchestrator. This browser lane does not replace it.
- **User model routing:** default Claude Sonnet 5.5 Medium for delegated bounded review; escalate to Claude Opus 5.5 Low when complexity or reviewer independence warrants; OpenAI assignments/cross-provider review use OpenAI 6.1 Sol.
- **No Windows build.**

## Starting-state rule

Do not trust this handoff as a frozen status snapshot. At task start and before every substantive disposition, refresh:

1. issue #91 and its latest ledger entries;
2. issue #106 and active T3 child claims;
3. `Docs/Project/coordination/active-tasks.md`;
4. the relevant approved spec/task/checklist files;
5. the GitHub integration ref and any exact candidate SHA being reviewed;
6. claims #111/#112/#113 for MFM-06 and any newer claims created after this packet.

The latest reported local umbrella candidate when this packet was written was:

`ac876a64969752a3864034521f42559671264b30`

It was explicitly recorded in #91 as **clean, local and unpushed**. Therefore:

- treat it as orchestration/local execution evidence only until GitHub can fetch the exact SHA/ref;
- never say the browser independently inspected or verified an unpushed SHA;
- when local and published states differ, state both explicitly.

## Browser-lane authority

This lane may:

- inspect GitHub branches, commits, diffs, files, issues, PRs and handoffs;
- inspect the approved product/spec authority and Project sources;
- reconcile stale task-list wording with newer verified evidence;
- maintain child-claim hygiene and dependency sequencing under #91;
- prepare and publish bounded task packets, review packets and continuation handoffs;
- issue technical `APPROVED / NEEDS CHANGES / REJECTED` verdicts when the evidence needed for that verdict is browser-inspectable;
- review source/static changes and wording/evidence-label conformance;
- coordinate required independent reviews and model/provider routing;
- update live coordination where safe and where no active owner conflict exists;
- prepare final Tier-3 requirements/checklists and the eventual user-playtest handoff structure.

This lane may **not**:

- claim local filesystem cleanliness from chat text alone;
- edit production or verification C#/Unity assets;
- run save files, compilers, Unity, runtime tests or rendered qualification;
- acquire the Unity lock;
- create/merge local worktrees or perform executable integration;
- claim a runtime/rendered/save gate passed without published evidence;
- infer implementation truth from a worker summary when the actual candidate cannot be inspected;
- weaken tests or protected anchors;
- change accepted product direction;
- build Windows.

## Operating rule

Continue automatically through all browser-safe work. Do not stop merely because one T3 task is waiting. Work ahead on independent reviews, audits, dependency reconciliation and packet preparation where that does not create stale ownership or premature implementation claims.

Only stop the whole browser lane for:

- a genuine unresolved product decision;
- an ownership conflict that requires Manager/user disposition;
- missing/unpublished evidence that makes the requested review impossible;
- a required independent reviewer that is unavailable and cannot be substituted under user routing;
- the point where all remaining work is local T3 execution or the user's personal playtest.

## Work programme

### A. Live-state reconciliation and claim hygiene

Maintain a current browser-visible programme picture.

For each substantive refresh:

- distinguish `IMPLEMENTED`, `REVIEWED`, `INTEGRATED`, `VERIFIED`, and `ACCEPTED`;
- distinguish local-only integration from GitHub-published integration;
- retire stale live coordination rows only when ownership is demonstrably released;
- do not close feature claims merely because a parent summary says the work is done;
- ensure new substantive T3 tasks have claims before execution;
- ensure claims never override `active-tasks.md` ownership.

Keep #91 updated with concise ledger entries after material browser reviews/dispatches.

### B. MFM V1 browser work

Current sequence from the approved spec is MFM-06 → MFM-07 → MFM-08 → MFM-09 → MFM-10 → MFM-11 → MFM-12.

#### MFM-06

Implementation claim #111, independent review #112, integration gate #113 are already prepared.

Browser responsibilities:

- inspect the published MFM-06 implementation diff/handoff when it exists;
- verify that the exact reviewed lineage really owned the next save version after S1-C T-05 v22;
- audit persisted fields against MFM-06 task/checklist authority;
- verify no old marks/orders are fabricated into prescriptions;
- verify malformed new data is rejected before mutation in source/verification logic where inspectable;
- verify Reference Future / Legacy40 / pre-feature compatibility evidence is present and not silently rebaselined;
- coordinate the required independent Claude Opus 5.5 Low review; do not silently substitute this browser chat for that reviewer if independence/model routing requires Claude;
- after #111/#112 return, publish a Manager integration disposition for #113 based on the available evidence;
- hand any unresolved executable gate back to T3 Code exactly.

#### MFM-07

Prepare the just-in-time packet only once MFM-06 is genuinely integrated on the active lineage.

Packet must preserve:

- proposals are not `TreeMarkType.Fell`;
- opening/approving Work Plan cannot import unapproved proposals;
- prescription approval creates pending ordinary work only;
- Work Plan approval remains the commitment gate;
- player manual state wins conflicts;
- provenance from MFM-06 survives into generated pending work.

#### MFM-08

Browser-safe preparation/review:

- map/3D information architecture review;
- terminology and evidence-label review;
- explicit distinction between sample, coverage, candidate, proposal, approved work and executed result;
- check planned presentation remains distinct from Red Fell, Blue Crop Tree and Orange Rack marks;
- prepare rendered 720p/1080p acceptance checklist for T3.

No rendered acceptance claim from the browser.

#### MFM-09

Coordinate sequencing with S1-D T-04 before dispatching overlapping curriculum writers.

Browser responsibilities:

- inspect existing teaching step IDs and accepted sequence authority;
- ensure new `Extend your treatment` step uses new stable IDs;
- ensure Work Plan copy/grouping preserves the approval boundary;
- prepare the joint ownership/API packet if S1-D T-04 and MFM-09 touch the same guide/current-state files.

#### MFM-10

Prepare/review the species-parity guard after PARITY-08/09 state is sufficiently stable.

Verify the task does not silently generalise Sitka-only quantitative rules to Oak/Beech.

#### MFM-11

Browser side:

- prepare calibration-impact evidence matrix;
- inspect returned measurements and compare sample-only / representative-prescription / plausible manual-override paths;
- coordinate required independent review;
- stop and flag `BLOCKED — CALIBRATION IMPACT` if returned evidence shows material accepted-outcome breakage rather than inventing new coefficients.

#### MFM-12

Browser side:

- run/coordinate the independent spec-conformance review using `checklists/requirements.md` after T3 executable evidence exists;
- verify save/load, map/3D, Work Plan boundary, old saves, provenance/history, completion evidence and no hidden automation are all evidenced;
- issue `APPROVED / NEEDS CHANGES / REJECTED`;
- only after full evidence support may the MFM slice receive:
  `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`.

### C. Three-species parity browser work

#### PARITY-08 — save / deterministic compatibility

Prepare the T3 packet and source-review returned implementation/verification.

Required scope:

- unknown-species rejection before mutation;
- model 0/1 old saves;
- model-2 saves;
- Legacy40;
- Reference archive;
- deterministic anchors;
- no parity-specific save field/schema bump;
- stop if a legitimate historical fixture contains an unknown species that accepted authority says must load.

Browser may inspect source and review diffs, but runtime/save execution remains T3.

#### PARITY-09 — integrated parity qualification

After PARITY-08 and all prerequisite slices settle:

- prepare Tier-2 parity qualification matrix;
- independently review combined parity evidence;
- check MFM Sitka-only guard;
- verify no fabricated empirical precision or protected Sitka anchor movement;
- issue the final parity conformance verdict.

Only after complete evidence support may the slice receive:
`THREE-SPECIES PARITY P0 VERIFIED`.

### D. S1-C browser work

Remaining tasks are T-04, T-06, T-07, T-08.

#### T-04 — provenance labelling

Browser can do substantial preflight/source/copy work:

- map every history/review fact to recorded/current/derived/reconstructed/session-only/not-recorded;
- inspect copy for unsupported causal wording;
- identify old-save states that must say `not recorded` rather than inventing values;
- prepare exact UI/rendered examples T3 must demonstrate.

Any code/UI edits and rendered validation go to T3.

#### T-06 — prior-treatment guidance

Browser can:

- audit source records available to support each guidance statement;
- review wording against FR-6/FR-7 evidence rules;
- ensure guidance remains bounded and non-prescriptive;
- prepare the two-intervention rendered walkthrough requirements;
- ensure waypoint guidance uses only recorded locations.

Implementation/runtime/rendered work goes to T3.

#### T-07 — S1-D data contract/stability

Browser can:

- reconcile the identity/data contract from current source/spec/handoffs;
- document expected stable IDs/counters/append-only semantics;
- inspect returned verification diffs;
- prepare genuine-old-save fixture requirements.

Any new assertions/tooling execution belong to T3.

#### T-08 — S1-C feature qualification/spec conformance

After T-04/T-06/T-07 executable work returns:

- prepare/check the Tier-2 evidence matrix;
- inspect results for multi-year session, two-resolution review, old-save load, round-trip, grouped-harvest year and no-work year;
- complete independent checklist/spec-conformance review where browser evidence is sufficient;
- issue verdict.

### E. S1-D browser work

Remaining tasks are T-04, T-05, T-08, T-09.

#### T-04 — guide monotonicity

Before dispatch:

- sequence with MFM-09 ownership;
- inspect current `ScenarioOneTeachingSequence.Current` and guide authority once the candidate is published;
- prepare an exact packet preserving stable IDs and forbidding new persistence;
- require reload monotonicity evidence across lessons 6–8.

#### T-05 — previous versus proposed presentation

Browser can:

- reconcile with S1-C history/UI ownership so no second history UI appears;
- define the required Approved / Pending new / Pending carried over presentation from the approved spec;
- review copy for non-causal/non-prescriptive wording;
- prepare the three-cycle rendered fixture and 720p/1080p acceptance matrix.

#### T-08 — multi-cycle witness

Browser prepares the exact T3 qualification packet:

- three real cycles;
- idle years;
- stale pending order;
- save/load at each stage;
- completion on the third cycle;
- continued management after Completed;
- D-057 solvent witness;
- protected anchor comparison;
- one stated uncertainty and stop when answered.

Execution remains T3.

#### T-09 — independent S1-D conformance review

After T-08:

- perform or coordinate a genuinely independent review;
- complete `checklists/requirements.md`;
- verify no hidden automation and no unlabelled coefficient;
- issue `APPROVED / NEEDS CHANGES / REJECTED`.

### F. Cross-feature integration review

Throughout the programme, check overlaps before dispatching writers:

- `AnnualReviewView` / `WorkPlanView` / history surfaces across S1-C, S1-D and MFM;
- teaching sequence/current-state files across S1-D T-04 and MFM-09;
- save/schema surfaces across S1-C v22, MFM-06 next-version work, PARITY-08 compatibility and final Tier-3;
- species truth vs MFM Sitka-only guard;
- Rack/Reference/Legacy compatibility.

If overlap exists, publish one ownership/API disposition rather than allowing parallel writers.

### G. Final Tier-3 preparation and final independent review

Before Tier-3 execution, browser lane must prepare a final candidate checklist that requires one exact clean candidate and covers:

- Rack V2 complete flow;
- three-species parity;
- S1-C history/review;
- S1-D repeated interventions;
- MFM sample → coverage → proposals → prescription approval → Work Plan → execution;
- save/load and old-save compatibility;
- Legacy40 and frozen Reference Future;
- complete Scenario One reachability/understandability;
- rendered 1280×720 and 1920×1080 states;
- completion/solvency witness;
- **rerun S1-C T-05 save/history chain on the final combined candidate**, not only its earlier isolated qualification.

After T3 returns the exact final candidate evidence:

1. inspect the published candidate SHA/ref;
2. verify all required child claims/handoffs are closed or explicitly non-blocking;
3. coordinate an independent final reviewer not responsible for the dominant final fixes;
4. issue final Manager verdict;
5. prepare `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF.md` with exact launch instructions and user action.

Only if all accepted gates are met may the programme emit:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

This is not user acceptance and does not authorise a Windows build.

## Packet/claim rules for every new T3 handoff

Every substantive local task handed to T3 must state:

- exact handoff path;
- plain-English purpose;
- difficulty: Low / Medium / High, with reason;
- best-suited model/provider under the user's ladder;
- exact current dependency/base lineage as far as known;
- branch/worktree suggestion where useful;
- child claim and status;
- owned files/subsystem if known;
- exclusions and protected invariants;
- exact verification/exit token;
- one copy/paste dispatch line;
- exact user action required, normally none except actually launching the worker because GitHub is not a live message bus.

Do not create implementation claims far in advance when dependencies may change. Prefer just-in-time claims/packets.

## Model routing

Use the user's prospective ladder:

1. **Claude Sonnet 5.5 Medium** — default bounded review/copy/spec work when an external model is actually needed.
2. **Claude Opus 5.5 Low** — escalate for difficult architecture, save/reference review, or when independent-review quality requires it.
3. Escalate further only with a recorded reason.
4. **OpenAI 6.1 Sol** — code/Unity-oriented review or cross-provider independence when appropriate.

Do not describe a model as having been dispatched merely because a GitHub packet/issue was published. State `prepared for dispatch` until a live chat/agent has actually received it.

## Required browser closeout

Publish:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-03-RESULT.md`

with:

- starting and ending GitHub-published integration state;
- local-only states reported but not independently inspectable;
- browser reviews completed and their verdicts;
- packets/claims created;
- executable residuals handed to T3;
- blocked items and why;
- remaining path to the user playtest;
- `USER ACTION REQUIRED` — including any live-agent dispatches still needing manual delivery;
- `DASHBOARD SYNC: NOT AVAILABLE` unless a real publisher becomes available.

Close #118 as `[HANDED-OFF]` only when no further browser-safe work can proceed without new T3 evidence or user action.

Browser-lane terminal token:

`BROWSER READINESS WORK EXHAUSTED — WAITING ONLY ON T3 EXECUTION / USER PLAYTEST`

## Copy/paste dispatch line

`Run claim #118 using Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-03-CONSOLIDATED-HANDOFF.md. Work continuously through every browser-safe Scenario One readiness review, audit, claim/coordination update and just-in-time T3 packet you can complete. Refresh #91, #106, active-tasks.md and the relevant approved specs before each substantive action; never treat local-only/unpushed SHAs as independently inspected truth; never claim C#/Unity/save/rendered/integration execution from browser evidence alone. Default delegated model routing is Sonnet 5.5 Medium, escalate to Opus 5.5 Low when justified, and use OpenAI 6.1 Sol for suitable cross-provider review. Keep #91 updated, hand every executable residual to T3 with an exact packet, and continue until only T3 execution or the user's personal playtest remains. No Windows build.`
