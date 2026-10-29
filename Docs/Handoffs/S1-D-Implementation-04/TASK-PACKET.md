# S1-D T-04 — Guide Monotonicity — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-04 — Guide monotonicity
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Claim:** #124
- **Status:** **RELEASED FOR CODE EXECUTION**
- **Exact base:** `b971f4da0540e606bfee9702b0c99f81a0a89355`
- **Purpose:** make the Scenario One teaching guide monotone from faithful persisted-history facts so later felling, Crop Tree-count changes, executed-share drift or save/load cannot regress a historically completed teaching stage.
- **Difficulty:** **4/5** — bounded production surface, but subtle derived-history/monotonicity logic must remain faithful across reload and later interventions.
- **Preferred implementation model/provider:** **Claude Sonnet 5.5 Ultra Code / Anthropic**.
- **Independent review:** required by the approved S1-D plan for the guide change; use an independent OpenAI reviewer suitable for the repository lane (project routing currently identifies OpenAI Sol/Codex as appropriate).

Suggested lane:

- branch `feature/s1-d-t04-guide-monotonicity-v1`
- worktree `/media/jer/Videos/CCF-S1D-T04`

Before edits, refresh #91/#106/#121/#122/#124 and live `Docs/Project/coordination/active-tasks.md`; verify exact base/clean lane and mark #124 `[RUNNING]` with actual model/provider, branch, worktree and starting HEAD. The claim itself does not grant ownership.

## Released start gate

The earlier preparation gates are now satisfied on the published production lineage:

- T-01/T-02/T-03 are present in the qualified base;
- the current T-03 implementation already scopes the teaching-footprint hard cap to the active sample and uses advisory behaviour afterwards;
- PARITY-07 no longer owns the curriculum surface;
- intended MFM-07 #121 explicitly excludes MFM-09 teaching-sequence insertion;
- MFM-09 has not yet started and must wait for T-04/T-05.

If live coordination shows a new writer on the curriculum files at actual start, stop before editing and return to Manager.

## Authority / required behaviour

Implement approved FR-B1 / FR-B2 / AC-3 and `plan.md` §2.2 only.

The current base still computes guide state from mutable current facts such as `Bay.HasEnoughFuture`, current executed/review state and session-only rack review. T-04 adds a **history-only monotonic floor/latch** only where persisted facts faithfully prove historical completion.

Required rules:

- compute/retain the existing current guide behaviour before historical completion;
- after a lesson boundary is faithfully established as historically complete, later Crop Tree count changes, later in-footprint felling, executed-share drift or load must not move the guide behind that boundary;
- use persisted facts such as completed/resolved orders, durable Annual Review acknowledgements and successful planting only where they faithfully prove completion;
- do **not** use `PlayerPrefs` or session-only `[J]/[K]` rack-review state as durable progression authority;
- do not persist `[J]/[K]` decisions;
- where no persisted fact faithfully proves a state, preserve existing behaviour there and report the uncovered state rather than inventing a latch;
- preserve every existing stable lesson ID: `access`, `future-trees`, `release`, `habitat`, `response`, `recruitment`, `establishment`, `transformation`;
- new IDs only if strictly required and authorised; no rename or reuse.

## Expected owned production surface while RUNNING

Primary:

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`

Allowed only if strictly necessary:

- smallest adjacent pure helper/read accessor required to consume authoritative history facts;
- focused verification files.

Do not expand into broad curriculum/UI files unless unavoidable and live ownership is explicitly reconciled. Do not edit save/schema/validator, completion rules, objective predicates/targets, Rack V2 lifecycle/geometry, ecology/economy/growth/regeneration, MFM implementation, scenes/prefabs/packages/ProjectSettings or Reference Future v1.

## MFM sequencing boundary

This task does **not** implement Multi-Scale Forestry Management. MFM-07 #121 owns proposal → validated pending-work only and excludes teaching insertion. MFM-09 later inserts `Extend your treatment` after existing Lesson 5 and edits this same teaching/current-state seam.

Therefore:

1. T-04 may execute in parallel with intended MFM-07 because their production file sets are non-overlapping.
2. T-04 must be integrated/accepted **before MFM-09**.
3. If MFM-07 unexpectedly changes these curriculum files, stop and return to Manager rather than merging assumptions.

## Required verification

### Offline/static matrix

Cover at least:

- normal current guide state before each historical boundary;
- Crop Tree count increase/decrease after historical completion;
- later in-footprint felling after historical completion;
- executed-share drift after historical completion;
- load/rebuild from persisted history;
- repeated/fourth-cycle symmetry where applicable;
- ambiguous/unfaithful persisted evidence falls back to current behaviour rather than over-claiming completion;
- all stable lesson IDs unchanged.

Regression requirements:

- TeachingCopy / relevant MenuTutorial checks;
- T-03 teaching-cap decision checks;
- T-02 cycle-state checks;
- N3 normalised completion-rule hash remains unchanged;
- S1-C history/query checks remain green.

### Unity qualification

Under the machine-wide Unity lock:

- import/compile exact candidate;
- save at Lessons 6–8 (or their exact unchanged IDs), reload, and prove guide index does not regress;
- exercise at least one post-completion world-drift trigger;
- prove pre-completion teaching behaviour remains unchanged.

### Independent review

An independent reviewer must confirm on the exact candidate:

- latch is derived only from persisted history and is non-mutating;
- no over-claim of lesson completion;
- T-03 and T-04 share one faithful historical-completion interpretation rather than competing definitions;
- no hidden persistence, ID rename/reuse or completion-rule change;
- tests were not weakened.

## Stop conditions

Stop and return to Manager if:

- no persisted fact faithfully establishes completion for a required state;
- T-03's current integrated behaviour materially conflicts with the planned latch;
- a new live writer owns the same teaching/current-state files;
- a schema field appears necessary;
- `ScenarioOneCompletionRules` or objective predicates would need to change;
- MFM has already rewritten `Current()` before this task lands.

## Handoff

Publish `Docs/Handoffs/S1-D-Implementation-04/HANDOFF.md` with:

- actual model/provider;
- exact starting/ending SHA;
- branch/worktree;
- changed files;
- lesson → persisted-proof latch mapping;
- uncovered/fallback states;
- offline + Unity evidence;
- independent-review verdict;
- stable-ID and N3-hash preservation;
- residuals / next dependency.

On handoff, rename #124 `[HANDED-OFF]`, link the handoff, close the claim and return:

`S1-D GUIDE MONOTONICITY QUALIFIED — READY FOR MANAGER REVIEW`

or a precise blocker.

## Copy/paste dispatch line

`Run claim #124 from Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md. Root from exact qualified base b971f4da0540e606bfee9702b0c99f81a0a89355. Implement only S1-D T-04 guide monotonicity using faithful persisted-history facts; preserve all existing lesson IDs, do not persist J/K, add no schema field, and do not implement MFM. Refresh live ownership before edits, mark #124 RUNNING with the actual lane, serialize Unity normally, obtain the required independent review, and publish the exact handoff. This must land before MFM-09. No Windows build.`
