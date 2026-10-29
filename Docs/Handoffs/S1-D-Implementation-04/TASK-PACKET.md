# S1-D T-04 — Guide monotonicity — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-04 — Guide monotonicity
- **Parent:** #91 PLAYER-TEST-OVERNIGHT-01
- **Purpose:** make the Scenario One teaching guide monotone from faithful persisted-history facts so later felling, Crop Tree-count changes, executed-share drift or save/load cannot regress a historically completed teaching stage.
- **Difficulty:** **Medium** — bounded derived/UI logic, but it touches hot teaching files and must prove a faithful history latch without inventing persistence.
- **Default implementation model/provider:** **Claude Sonnet 5.5 Medium / Anthropic**.
- **Mandatory independent review:** **OpenAI 6.1 Sol** after implementation because the approved S1-D plan requires independent review for the guide change.
- **Suggested branch:** `feature/s1-d-guide-monotonicity-v1`
- **Suggested worktree:** `/media/jer/Videos/CCF-S1D-T04`
- **Claim:** **do not create/start the substantive claim yet**. At release, create `[RUNNING] S1-D T-04 — guide monotonicity` and record actual model/provider, branch/worktree and starting HEAD. Packet preparation grants no production ownership.
- **User action required:** none.

## Start gate — all required

Do not start production edits until all are true:

1. T-03's exact integrated descendant is **published and GitHub-refreshable**; do not base this task on the local-only `22ba458...` claim.
2. T-03's faithful teaching-sample completion definition is inspected and reused; T-04 must not create a competing definition.
3. PARITY-07 #102 has released any overlapping teaching/curriculum ownership and its exact accepted candidate is published.
4. Live `Docs/Project/coordination/active-tasks.md` is refreshed and grants this task the single-writer teaching/current-state file set.
5. Starting integration HEAD is recorded exactly.

If any gate is false, mark the eventual task claim `[BLOCKED]` with the exact dependency; do not edit.

## Authority / required behaviour

Implement approved FR-B1 / FR-B2 / AC-3 and plan §2.2 only.

- Compute the existing current guide state as before, then apply a **history-only monotonic floor/latch** for lessons whose prerequisite facts cannot un-happen.
- Use persisted facts such as completed/resolved orders, durable Annual Review/history acknowledgement and successful juvenile planting only where they faithfully prove historical completion.
- Before historical completion, preserve existing in-lesson behaviour, including session-only practice/review behaviour.
- After historical completion, later world drift or load must not move the guide backwards.
- Where no persisted fact faithfully proves completion, preserve existing behaviour and report the uncovered state; **do not invent a latch**.
- Keep existing stable step/lesson IDs unchanged. New IDs only if strictly necessary and authorised by the approved spec.

## Expected owned production surface

Reconfirm exact files after refreshing the integrated base. Expected bounded surface:

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`
- only the minimum adjacent helper/read accessor needed to consume the already-authoritative history fact.

Do not expand into `LearningObjectivesView.Curriculum.cs` unless an unavoidable ID-only addition is required and #102/MFM ownership is clear. Stop for Manager API/ownership review if broader curriculum work is needed.

## Hard exclusions

- no new persisted field or schema/version change;
- no `PlayerPrefs` or session-only `[J]/[K]` state as progression authority;
- no persistence of `[J]/[K]` decisions;
- no `ScenarioOneCompletionRules.cs` or objective predicate/target change;
- no economy/ecology/growth/regeneration changes;
- no Rack lifecycle/geometry changes;
- no MFM implementation;
- no duplicate history store/UI;
- no Reference Future v1 change;
- no Windows build.

## Required verification

### Offline / static

Create a focused matrix covering at least:

- current guide before each historically completed stage;
- Crop Tree count increases/decreases after completion;
- later in-footprint felling after completion;
- executed-share drift after completion;
- save/load reconstruction;
- fourth-cycle/repeated-management symmetry where applicable;
- ambiguous/unfaithful persisted fact falls back to existing behaviour rather than over-claiming completion;
- stable step IDs unchanged.

Preserve/regress:

- TeachingCopy / MenuTutorial relevant offline checks;
- T-03 cap decision checks;
- T-02 cycle-state checks;
- N3 normalised completion-rule hash unchanged;
- S1-C history/query regressions.

### Unity qualification

Under the machine-wide lock, after static review:

- save at Lessons 6–8 (or exact current equivalent), reload, and prove guide index does not regress;
- exercise at least one world-drift trigger after historical completion;
- prove pre-completion teaching behaviour remains unchanged;
- import/compile cleanly on the exact candidate.

### Independent review

OpenAI 6.1 Sol reviews the exact candidate after implementation and before Manager integration. Reviewer must confirm:

- latch is history-derived and non-mutating;
- no over-claim of completion;
- no second completion definition diverging from T-03;
- no hidden persistence or ID reuse;
- tests do not weaken existing gates.

## Stop conditions

Stop and return to Manager if:

- no persisted fact faithfully establishes completion for a required state;
- T-03's published implementation exposes a materially different completion contract than this packet assumes;
- PARITY-07 or MFM owns the same file set at start;
- a schema field appears necessary;
- completion rules/objective predicates would need to change.

## Handoff

Publish:

`Docs/Handoffs/S1-D-Implementation-04/HANDOFF.md`

Include exact SHA, files changed, latch mapping (lesson → persisted proof), uncovered/fallback states, offline and Unity evidence, independent-review verdict, preserved IDs/hashes and remaining dependencies.

Success token:

`S1-D T-04 GUIDE MONOTONICITY QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

## Copy/paste dispatch line

`Run S1-D T-04 from Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md. Refresh published integration, #91, #102, #103 and live active-tasks first. Start only when T-03 is published-integrated and PARITY-07 has released overlapping teaching ownership. Use Claude Sonnet 5.5 Medium by default; require OpenAI 6.1 Sol independent review of the exact candidate. No schema, no completion-rule change, no Windows build.`
