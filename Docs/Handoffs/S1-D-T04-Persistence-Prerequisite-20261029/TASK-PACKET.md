# S1-D T-04P — Minimal Teaching-Completion Persistence — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-04P — D-066 minimum durable teaching-history persistence prerequisite
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Claim:** #143
- **Status:** **PREPARED — DO NOT START UNTIL BROWSER MANAGER REVIEW APPROVES AND RELEASES THIS PACKET**
- **Exact production base:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5` — post-MFM-07 qualified production umbrella
- **Inspected save schema on that base:** `ForestSaveData.CurrentVersion = 23`
- **Expected next schema:** **v24 only if live start refresh still confirms v23 is current/free and no competing save/schema writer exists.** Otherwise STOP and return to Manager.
- **Purpose:** implement the smallest durable aggregate historical record required by D-066 so an actually completed Scenario One teaching sample/review remains historically complete across save/load and later world drift, without serializing individual `[J]/[K]` choices.
- **Difficulty:** **High** — small data surface, but shared save/load/validation, backward compatibility, deterministic/reference protection and exact curriculum semantics make incorrect implementation costly.
- **Best-suited model/provider:** OpenAI Sol/Codex local code lane or equivalent strong repository/Unity implementation model with save/validation discipline.
- **Independent review:** mandatory before Manager acceptance because the task changes the shared save/load contract.

Suggested lane:

- branch `feature/s1-d-t04-teaching-persistence-v1`
- worktree `/media/jer/Videos/CCF-S1D-T04-Persistence`

Planned handoff:

- `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md`

## Start protocol

Before any edit:

1. refresh `Docs/Project/coordination/active-tasks.md` from GitHub;
2. refresh claims #91, #106, #124, #126, #128 and #143;
3. verify the intended branch/worktree is clean and starts from exact `c8cfae8c4025cec6e960a050edf4bfde2a981de5` unless a later Manager packet explicitly repins it;
4. inspect live `ForestSaveData.CurrentVersion` and current save/validator ownership;
5. confirm no other live writer owns `ForestSaveData`, save controller, save validation, reference/hash compatibility or the same Scenario One save fields;
6. only then mark #143 `[RUNNING]` with model/provider, branch, worktree and starting HEAD and register ownership in live coordination.

If `CurrentVersion != 23`, v24 has been consumed/reserved, or another save/schema writer is active, **STOP before editing** and return to Manager. Do not improvise a version or overlap ownership.

## Product authority — D-066

The user selected Option A: preserve full S1-D FR-B1 / AC-3.

The task may persist **only** the minimum aggregate historical teaching-completion evidence required to distinguish incomplete from completed teaching progression. The durable record is authority only for historical teaching progression and the T-03/T-04 shared `sample historically complete` interpretation.

It is not:

- a forestry prescription;
- a Scenario One completion predicate;
- an MFM prescription/coverage/work-order record;
- a Rack V2 lifecycle record;
- a current readiness/recommendation cache;
- a replay of the session review;
- a store of individual player `[J]/[K]` decisions.

## Authoritative completion transition

The record may become complete **only when the existing pre-amendment in-session teaching sample/review has actually completed under its authoritative lesson rules**.

It MUST NOT infer completion from:

- successful planting;
- arbitrary or merely in-footprint felling;
- a later Annual Review acknowledgement by itself;
- current Crop Tree count or loss;
- current executed-share values after drift;
- current rack geometry alone;
- `PlayerPrefs`;
- save age/version;
- a heuristic reconstruction from incomplete historical records.

The implementation must identify the exact authoritative old in-session transition from the inspected code and hook persistence to that transition with the smallest safe surface.

Once established, historical completion is monotone and cannot be unset by later world changes.

## Required persisted shape

Concrete C# names are implementation choices, but the record must be no broader than necessary. It may include:

- a deterministic stable teaching/management-cycle key;
- sample-complete state;
- review-complete state;
- only the aggregate counters/outcomes strictly needed to distinguish an incomplete teaching sample/review from a completed one and to validate the record.

Do not persist:

- individual `[J]/[K]` choices;
- inspected tree IDs;
- stand-portion choices/details;
- review transcript/order;
- current guide index if derivable from the aggregate + current facts;
- current readiness/advisory state;
- Crop Tree designation year;
- a broad `ScenarioOneManagementCycleState` or equivalent.

Prefer append-only/monotone historical records if multiple teaching cycles must be distinguished; do not overwrite historical completion with current-session state.

## Expected owned production surface while RUNNING

Inspect actual files first. Expected bounded ownership:

- `Assets/ForestPrototype/ForestSaveData.cs` — version + minimum Scenario One teaching-history field/record only;
- smallest Scenario One save capture/restore adapter required to round-trip the new record;
- `ForestSaveValidation*.cs` / existing pre-mutation validation path only as required for the new record;
- smallest authoritative teaching-completion write hook/helper needed to record the aggregate;
- focused verification files under `Tools/Verification/`.

Do **not** absorb #124 curriculum/guide replay. Do not edit MFM proposal/prescription logic, Rack V2 geometry/lifecycle, completion rules, scenario objective predicates/targets, ecology/economy/growth/regeneration, S1-C presentation, scenes/prefabs/packages/ProjectSettings or Reference Future archive content.

If the authoritative completion hook sits in a file reserved by #124, coordinate one API boundary through Manager; do not make #143 and #124 concurrent writers.

## Schema and migration contract

On the inspected base `c8cfae8c...`, save v23 is owned by MFM-06 prescription/provenance state and qualified through MFM-07 integration. If live start inspection still confirms this exact state, T-04P owns the single next schema increment, expected **v24**.

Requirements:

- v23 and all supported earlier saves remain loadable under existing policy;
- a missing D-066 field/list means **not historically complete / not recorded** for the durable teaching latch;
- do not backfill completion from session state, `PlayerPrefs`, current world facts or historical heuristics;
- preserve MFM-06/07 prescription/provenance fields exactly;
- preserve objective-achievement history, Rack V2 state and existing older-version defaults;
- Reference Future v1 remains frozen; do not rewrite its archive or reserialize it to establish integrity;
- Legacy40 and pre-v20 paths retain their accepted compatibility semantics; no Enlarged80-only teaching record is fabricated into Legacy40.

## Validation contract

For the new owning schema version, malformed or ambiguous D-066 data must be rejected before world or disk mutation through the same safe load contract used by `CanLoad` and `LoadData`.

Define and test at least:

- duplicate stable teaching-cycle keys;
- invalid/empty key where key is required;
- impossible completion combinations (for example review complete while the required sample completion is false, if the chosen representation permits such a state);
- negative/invalid aggregate counts;
- duplicate/conflicting records for the same cycle;
- unknown enum/state values if any are introduced.

Do not silently repair, renumber, merge or reinterpret malformed new-version history.

## Required verification

### Offline / static

- exact schema/version assertion;
- serialize → deserialize → serialize round-trip of incomplete and completed records;
- monotonic write semantics: completion cannot revert;
- repeated calls/reads in one state do not append/consume another sample;
- authoritative completion transition records exactly once;
- negative validation matrix for every malformed-data case;
- pre-v24 / missing-field default is false/not-recorded;
- grep/serialization proof that individual `[J]/[K]` choices and inspected-tree/session-review detail are absent from the save model;
- N3 normalised `ScenarioOneCompletionRules` source/hash unchanged — **no rebaseline**;
- protected save/reference/hash compatibility checks relevant to the touched path;
- MFM-06/07 save/provenance round-trip remains green.

### Unity qualification

Under the machine-wide Unity lock only after offline gates pass:

1. import/compile exact candidate;
2. create an incomplete teaching-sample state, save/load, prove it remains incomplete and current in-session behaviour is unchanged;
3. complete the authoritative in-session sample/review, prove one durable aggregate is recorded;
4. save/load and prove historical completion remains true;
5. repeat view refresh / read path / same-state calls and prove no second sample/record is consumed;
6. mutate later world state (Crop Tree count/loss, later in-footprint felling or executed-share drift where possible without #124 guide changes) and prove the stored historical aggregate remains unchanged;
7. load representative v23 and earlier supported saves with no field and prove conservative false/not-recorded default;
8. exercise malformed v24 fixtures through both `CanLoad` and `LoadData` and prove pre-mutation rejection parity;
9. rerun narrow SaveHardening/reference/compatibility gates required by the existing project harness.

The curriculum guide itself is not claimed qualified by this task; #124 consumes the aggregate later.

### Mandatory independent review

Independent reviewer on the exact candidate must inspect:

- diff is bounded to D-066 persistence/validation/write hook;
- completion write hook is the authoritative old in-session teaching transition, not a surrogate;
- persisted shape is minimal and contains no individual `[J]/[K]`/session transcript;
- old saves default conservatively and remain loadable;
- MFM-06/07 and protected reference/hash contracts are preserved;
- validation rejects malformed new records before mutation;
- tests were not weakened and no hash/reference was rebaselined.

Verdict must be APPROVED before Manager integration/release to #124.

## Stop conditions

STOP and return to Manager if:

- live schema is no longer v23 or v24 is reserved/consumed;
- another save/schema/validator writer is active;
- exact production base cannot be established safely;
- faithful completion would require persisting individual `[J]/[K]`, inspected-tree detail or substantially more session transcript;
- implementation would alter `ScenarioOneCompletionRules`, objective predicates/targets, economy/ecology, Rack V2 lifecycle, MFM prescription semantics or Reference Future v1;
- a legitimate accepted old save cannot default/load conservatively without a new compatibility decision;
- a protected test/hash would have to be weakened/rebaselined merely to pass.

## Handoff requirements

Publish `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md` with:

- actual model/provider;
- exact starting production SHA and ending production SHA;
- actual schema version used and proof it was free at start;
- branch/worktree;
- changed files;
- exact persisted record shape and authoritative completion write hook;
- old-save/default policy;
- malformed-data validation rules;
- offline + Unity evidence and tokens;
- independent-review verdict/evidence;
- proof `[J]/[K]` and session detail are not serialized;
- N3/reference/hash/MFM save compatibility results with no rebaseline;
- exact candidate/integration recommendation for #124.

At handoff, rename #143 `[HANDED-OFF]`, link the handoff, close the claim and return:

`S1-D T-04P PERSISTENCE QUALIFIED — READY FOR MANAGER REVIEW`

or the precise blocker.

## Copy/paste dispatch line

`Run claim #143 from Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/TASK-PACKET.md after Manager release. Start from exact qualified production umbrella c8cfae8c4025cec6e960a050edf4bfde2a981de5 only if live refresh still confirms save v23 and no competing save/schema writer; then v24 is the expected single next schema. Implement only D-066's minimum aggregate historical teaching-sample/review completion, triggered by the authoritative pre-amendment in-session completion transition. Do not serialize individual J/K or session-review detail, do not implement #124 curriculum, preserve v23/older/Legacy40/Reference/MFM compatibility, enforce CanLoad/LoadData pre-mutation validation, run offline + Unity + independent review, and do not build Windows.`