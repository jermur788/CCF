# S1-D T-02 — Advisory cycle/readiness state from existing history — Task Packet

**Task:** `S1-D-T02 — derive advisory cycle/readiness state from existing persisted records`  
**Spec:** `specs/scenario-one-repeated-interventions-v1/` — **APPROVED**  
**Prerequisite:** S1-D T-01 Manager-approved; handoff integrated by PR #71 / merge `4d6864515c32c87445bdff42819fb358905a10a0`  
**Manager review:** `Docs/Handoffs/S1-D-Implementation-01/MANAGER-REVIEW.md`  
**Mode:** code + offline verification only; **NO UNITY REQUIRED**  
**Windows build:** prohibited

## Goal

Implement the S1-D V1 **pure derived cycle-state adapter** required by FR-D1..D3 and FR-F1. The adapter describes the player's current management-cycle context from records already persisted by Scenario One. It does not make a treatment decision, block play, prescribe an intervention interval, or add save state.

The output must expose named advisory components **with reasons**, including at minimum:

- approved work awaiting execution;
- pending work, including carried-over/open work;
- stale or invalid open orders;
- latest resolved intervention and whether its later Annual Review acknowledgement is recorded;
- years since the latest resolved intervention, descriptive only;
- enough explanatory text/data that every state has a non-empty reason/hint.

This is an L3 derived/game interpretation layer. It is not forestry evidence and must not be presented as an empirical intervention schedule.

## Starting authority

Refresh before work:

1. current `main` (must descend from `4d6864515c32c87445bdff42819fb358905a10a0` and contain this packet);
2. `Docs/Project/coordination/active-tasks.md`;
3. this task claim;
4. S1-C T-03 claim #19.

Manager sequencing gives **S1-D T-02 the history-query file set first**. S1-C T-03 is held from history-query edits until T-02 integrates. If live coordination disagrees, STOP before editing and return `OWNERSHIP CONFLICT — MANAGER REQUIRED`.

## Required implementation boundary

Expected production files:

- **new:** `Assets/ForestPrototype/ScenarioOne/ScenarioCycleState.cs` (working name may change only if a materially clearer name is needed);
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs` — read accessor only;
- new focused offline verification under `Tools/Verification/ScenarioOneRepeatedInterventions/` or the closest existing plain-.NET offline structure.

Do **not** edit `UI/AnnualReviewView.cs`, `ScenarioManagementHistory.cs` or `ScenarioInterventionHistory.cs` unless the current source makes the approved contract impossible without a small shared helper. If one of those edits becomes necessary, stop before writing and report the exact API need because S1-C T-03 is queued against the same history surface.

## Binding design decisions

### One history authority

Do not create another history store or duplicate intervention parser. Use the integrated S1-C/S1-D derived-history architecture. For the term **latest resolved intervention**, use the same intervention unit exposed by `ScenarioInterventionHistory.Build` / `ScenarioOneManager.Interventions`: one resolved work-plan year with at least one succeeded commissioned task. Do not silently substitute the completion-rule plan parser.

The T-01 review found that these definitions agree on normal saves but can differ on odd/malformed data. T-02 must state the chosen definition explicitly in code comments/tests and stay consistent with the intervention history surface.

### Review acknowledgement

Derive "latest resolved intervention reviewed" from existing `AnnualReviewAcknowledged` management events. No new persisted acknowledgement field is allowed. Put the derivation in one named cycle-state helper/API rather than scattering a third anonymous interpretation through UI code.

The adapter must distinguish **advisory review state** from gameplay gating: an outstanding acknowledgement for a later intervention must never disable Approve Work Plan or Advance.

### No persisted state

No new field, schema version, `PlayerPrefs`, session latch, hidden cache or serialized component. Rebuilding from a field-for-field copy of the persisted inputs must return an equivalent result and mutate nothing.

### No prescriptive state machine

Do not return a single `ShouldIntervene`, `ReadyForIntervention`, fixed interval, recommended treatment or automatic action. Named components may describe facts such as `yearsSinceLatestIntervention`; the reason must make clear this is context for player judgement, not a universal schedule.

### Repetition symmetry

The fourth intervention/cycle is treated by the same derivation as the first/second/third. No hard-coded two-cycle or three-cycle terminal branch. Scenario completion may already be achieved; management-cycle context must still be derivable afterwards.

## Required verification — offline only

Add a focused plain-.NET suite and run the existing relevant offline checks. At minimum prove:

1. no-intervention state has a non-empty explanation;
2. one, two, three and four resolved interventions use the same derivation rules;
3. idle years do not create interventions;
4. approved work awaiting execution is distinguishable from pending/unapproved work;
5. carried-over work remains visible as pending context;
6. stale/invalid open orders are surfaced, not expired or deleted;
7. latest-intervention acknowledgement is derived by year/event record and does not become an Approve/Advance gate;
8. an outstanding acknowledgement after cycle two remains advisory;
9. `yearsSinceLatestIntervention` is descriptive and does not produce a fixed treatment recommendation;
10. rebuilding from a deep/field-for-field copy returns equivalent state and does not mutate inputs;
11. Completed outcome still yields cycle context and does not imply the player can no longer manage;
12. no new save field/schema change and no `UnityEngine` dependency in the cycle-state implementation.

Also rerun proportionate existing checks covering S1-C history/query and Scenario history. The N3 normalised `ScenarioOneCompletionRules` hash must remain unchanged. Pre-existing unrelated failures are reported with base reproduction, never fixed or rebaselined here.

## Must not change

- `ScenarioOneCompletionRules.cs` or objective predicates/targets;
- `ForestSaveData` / save schema;
- Rack V2 files or rack reopening/locking behaviour;
- MFM files;
- economy, ecology, growth, regeneration, mortality or species values;
- scenes/prefabs/USS/UXML/ProjectSettings/packages;
- Reference Future v1;
- Annual Review presentation or Work Plan presentation;
- any Unity gate, Player build or Windows build.

## Handoff requirements

Publish `Docs/Handoffs/S1-D-Implementation-02/HANDOFF.md` containing:

- actual model/provider;
- branch/worktree and starting/ending SHAs;
- changed files;
- exact cycle-state API and the chosen definitions for intervention and acknowledgement;
- offline commands/results and negative controls where useful;
- proof of no schema/Unity/product-value change;
- repo cleanliness/unrelated-work preservation;
- any ownership/API conflict;
- exact remaining gate.

At publication, mark the task claim `[HANDED-OFF]`, link the handoff, close it, and return:

`S1-D ADVISORY CYCLE STATE IMPLEMENTED — READY FOR MANAGER REVIEW`

If a new persisted field or competing history authority is required, stop instead with:

`S1-D T-02 BLOCKED — PRODUCT/ARCHITECTURE DECISION REQUIRED`

## Dashboard

Dashboard publisher is not currently available. Report `DASHBOARD SYNC: NOT AVAILABLE` / `Published dashboard reflects this task: NO` unless that changes.
