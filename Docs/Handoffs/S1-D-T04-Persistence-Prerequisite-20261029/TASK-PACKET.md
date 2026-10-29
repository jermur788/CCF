# S1-D T-04P — Minimal Teaching-Completion Persistence — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-04P — D-066 minimum durable teaching-completion persistence prerequisite
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Claim:** #143
- **Status:** **BLOCKED — DO NOT START UNTIL REPAIRED D-066 MANAGER REVIEW #158 RETURNS APPROVED AND MANAGER REPINS/RELEASES THIS PACKET**
- **Exact inspected production base:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5` — post-MFM-07 qualified production umbrella
- **Inspected save schema:** `ForestSaveData.CurrentVersion = 23`
- **Expected next schema:** v24 only if a fresh released-task start still confirms v23 current/free and no competing save/schema writer. Otherwise STOP.
- **Difficulty:** High — tiny state surface, high-consequence shared save/hash contract.
- **Best-suited model/provider:** local SOL/Codex high-reasoning save/validation lane or equivalent strong repository/Unity implementation model.
- **Independent review:** mandatory before Manager acceptance.

Suggested lane after release:

- branch `feature/s1-d-t04-teaching-persistence-v1`
- worktree `/media/jer/Videos/CCF-S1D-T04-Persistence`

Planned handoff:

- `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md`

## Purpose

Implement exactly one durable, monotone historical fact for Scenario One:

> **The existing first guided rack teaching sample/review has historically completed at least once.**

The persisted representation is one boolean-like Scenario One save latch. Exact C# field/helper names are implementation choices. **Do not** replace it with a per-cycle key, list, counter set, aggregate history record, guide-index cache or broad management-cycle object.

This task exists only because the current `[J]/[K]` rack-review evidence is session-only and cannot be reconstructed faithfully after save/load. Individual decisions remain session-only.

## Hard release gate

Before any production edit, all must be true:

1. browser Manager review #158 returned **APPROVED** for the repaired D-066 contract;
2. Manager explicitly repinned/released #143;
3. live `Docs/Project/coordination/active-tasks.md` gives #143 sole save/schema/hash ownership for this slice;
4. exact start lineage and `ForestSaveData.CurrentVersion` are refreshed;
5. no #126/PARITY or other save/schema/validator writer is active;
6. branch/worktree are clean at the Manager-pinned start SHA.

Then mark #143 `[RUNNING]` with model/provider, branch, worktree and starting HEAD. A claim alone does not grant ownership.

If v23 is no longer current/free, v24 is occupied, or the exact production lineage changed without a Manager repin, **STOP before editing**.

## Authoritative write transition — product architecture is fixed

The latch starts false and may change **false → true exactly once**. It is never reset.

Write it only when the **unlatched/pre-amendment rack-world teaching sequence would genuinely advance from `response` to `recruitment` in the current session**.

At the inspected qualified lineage, this means the existing `ScenarioOneTeachingSequence.Current(Facts)` conditions for lessons 1–5 have all passed and, specifically for leaving `response`:

- the release has actually reached the existing executed/mixed-outcome lesson state under the current rack-bay rules;
- the release Annual Review has been acknowledged (`ReleaseReviewed`);
- the real session rack review is complete (`RackReviewComplete`), derived from `ScenarioOneRackReview` after the player has performed the current-session review;
- applying the **pre-latch** sequence rules would therefore return `recruitment` (lesson index 5) rather than `response` (lesson index 4).

The implementation may extract/refactor a pure helper, but it must preserve this exact pre-amendment transition. Verification must compare the helper against the old sequence boundary before the latch is applied.

### Explicitly insufficient write evidence

Do **not** set the latch from any of these alone or in combination unless they are part of the exact transition above:

- `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` by itself — it deliberately omits the session `[J]/[K]` review;
- planting or later lesson progress;
- arbitrary/in-footprint felling;
- a later Annual Review acknowledgement;
- current Crop Tree count or loss;
- current rack geometry;
- current executed-share drift;
- `PlayerPrefs`;
- save version/age;
- heuristic reconstruction from older persisted records.

Do not write during load/restore, display/read refresh, or from a post-load inference.

## One shared T-03 / T-04 historical read contract

Expose the smallest read-only API/property whose meaning is exactly:

> `true` iff the first guided rack teaching sample/review historical completion latch has been durably recorded in this save lineage.

Two-phase semantics are mandatory:

- **Before latch:** read = false. T-03 keeps its accepted current-world/session cap classification and deadlock safeguard; T-04 keeps existing pre-completion guide behaviour. The durable field must not pretend session review occurred.
- **At transition:** record true once.
- **After latch:** read = true forever. T-03 and T-04 both consume this same historical fact. Later world drift/save-load cannot unset it.

Do not create a second durable T-03 definition. `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` may remain as a current-world helper if needed before the latch, but it is not the persisted historical authority after D-066.

## Persisted shape / schema contract

The save impact is limited to:

- one boolean-like field in Scenario One save state;
- the smallest capture/restore plumbing needed to round-trip it;
- expected one-version schema increment only if the live slot is free;
- the smallest validation/hash compatibility changes necessary to preserve existing contracts.

Do not persist:

- cycle IDs/keys or per-cycle records;
- counters/outcomes solely for teaching history;
- individual `[J]/[K]` choices;
- inspected tree IDs or stand-portion details;
- session review transcript/order;
- current guide index;
- readiness/advisory state;
- Crop Tree designation year;
- a broad `ScenarioOneManagementCycleState` or equivalent.

### Old-save behavior

For every supported pre-amendment save, field absent/default false means **historical completion not recorded**. Do not backfill. Existing game state still loads under existing compatibility policy; the teaching review may need to be completed again in-session before the new latch can be earned.

A boolean latch does not justify a new key/counter/list validation model. Preserve existing CanLoad/LoadData pre-mutation guarantees and add only validation genuinely required by the chosen one-field schema/version plumbing.

## World identity / hash contract — mandatory no-rebaseline rule

The teaching latch is saved progression state but **never part of biological/world identity**.

Full save serialization must round-trip it, while every world/hash projection must strip/ignore it. Inspect and cover at least:

- `ScenarioReferenceArchive.WorldHash`;
- `CurrentWorldHash`;
- `LegacyCompatibleWorldHash`;
- `HashLayout` for both its historical/default-later-field paths **and the non-default v23 prescription-state branch**;
- any `MinimalWorldHash` or equivalent minimal/protected identity projection used by verification;
- protected hash extraction/comparison and Reference Future/anchor checks that consume those projections.

At v23, a world with real prescription state currently reaches a `HashLayout` branch that keeps full current JSON except explicitly stripped history. The new field must be explicitly excluded there too. Do not rely on it being default/false.

Use the smallest compatibility edit that removes only this teaching-only field from identity inputs. **Do not rewrite Reference Future v1, move accepted anchors, change hash semantics for authoritative world state, or rebaseline any protected fingerprint.**

## Expected production ownership after release

Inspect actual files first. Expected bounded surface:

- `Assets/ForestPrototype/ForestSaveData.cs` — one field + version only if the live next slot is free;
- smallest Scenario One manager/save capture/restore surface;
- smallest teaching write/read helper surface needed to set/read the latch;
- `ScenarioReferenceArchive` hash-layout compatibility code only as required to strip the field everywhere;
- existing save validation only where genuinely required by the one-field version contract;
- focused verification under `Tools/Verification/`.

Do **not** absorb #124 curriculum replay. Do not change completion rules/objectives, economy/ecology, Rack V2 lifecycle/geometry, MFM prescription semantics, S1-C history presentation, scenes/prefabs/packages/ProjectSettings or Reference Future archive content.

If the authoritative write hook overlaps an active owner, STOP and obtain a Manager API/ownership disposition rather than racing it.

## Required verification

### Offline/static

Prove all of the following on the exact candidate:

- exact live schema/version selection and no collision;
- false/default old-save behavior;
- false → true one-way semantics; no true → false path;
- the write helper matches the **pre-amendment `response → recruitment`** boundary including `ReleaseReviewed` and actual session `RackReviewComplete`;
- `ScenarioOneTeachingCap.SampleHistoricallyComplete` alone cannot set the latch;
- repeated reads/refreshes/same-state calls do not perform a second write;
- save → load → save round-trip preserves false and true states;
- individual `[J]/[K]`, inspected-tree and stand-portion/session-review detail are absent from save data;
- T-03 pre-latch tests retain existing current-world/session behavior;
- the shared historical read returns true after latch for both T-03/T-04 consumption;
- N3 normalised `ScenarioOneCompletionRules` source/hash is unchanged;
- **every world/hash identity projection is byte-for-byte unchanged for otherwise identical false-vs-true latch worlds**;
- include a non-default v23 prescription-state world in that hash comparison;
- protected reference/hash baselines remain unchanged — no rebaseline;
- MFM-06/07 prescription/provenance save round-trip remains green.

### Unity qualification

Under the machine-wide Unity lock only after offline gates pass:

1. import/compile exact candidate;
2. reach `response` with incomplete session rack review; prove latch remains false;
3. satisfy Annual Review but leave `RackReviewComplete` false; prove latch remains false;
4. complete the actual rack review so the pre-latch sequence crosses `response → recruitment`; prove exactly one latch write;
5. save/load and prove latch remains true;
6. reload/refresh repeatedly and prove no extra write/side effect;
7. mutate later Crop Tree count/loss, in-footprint felling and executed-share state where possible; prove latch remains true;
8. representative pre-amendment saves load with false/default state and no heuristic backfill;
9. rerun SaveHardening/reference/hash/MFM compatibility gates required by the existing harness.

This task does **not** claim #124 guide behavior qualified; it supplies the durable fact that #124 later consumes.

### Independent review

An independent reviewer must inspect the exact candidate and return APPROVED before Manager integration. Review must confirm:

- exactly one boolean-like latch; no per-cycle history/counters;
- exact authoritative current-session write boundary;
- no individual `[J]/[K]`/session detail persisted;
- conservative old-save false default;
- one shared post-latch T-03/T-04 read meaning;
- all world/hash projections exclude the latch, including non-default prescription worlds;
- save round-trip still preserves the latch;
- no tests/hashes/references were weakened or rebaselined.

## Stop conditions

STOP and return to Manager if:

- #158 is not APPROVED or #143 has not been explicitly released;
- live schema/version/ownership differs from the repinned packet;
- one boolean latch is insufficient and a materially broader persisted model appears necessary;
- faithful completion would require persisting individual `[J]/[K]`, inspected-tree/stand-portion detail or a replay transcript;
- the exact `RackReviewComplete` session boundary cannot be used without changing product meaning;
- the latch cannot be excluded from identity without changing accepted world/hash semantics rather than preserving them;
- another save/schema/hash writer is active;
- implementation would alter completion/objectives, economy/ecology, Rack V2, MFM, frozen Reference Future content or accepted anchors;
- a protected hash/test would require weakening/rebaseline merely to pass.

## Handoff requirements

Publish `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md` with model/provider, branch/worktree, exact start/end SHAs, actual schema version, changed files, exact latch field/read API/write hook, old-save behavior, all offline/Unity evidence, independent-review verdict, proof no `[J]/[K]` detail is serialized, and all-hash/no-rebaseline results including a non-default prescription-state case.

On successful handoff mark #143 `[HANDED-OFF]`, link it, close the execution claim and return:

`S1-D T-04P PERSISTENCE QUALIFIED — READY FOR MANAGER REVIEW`

or the precise blocker.

## Copy/paste dispatch line — only after #158 APPROVED + Manager release

`Run claim #143 from Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/TASK-PACKET.md only after Manager release. Implement one monotone first-guided-sample/review completion latch, written exactly once when the unlatched pre-amendment rack-world teaching sequence genuinely crosses response→recruitment with ReleaseReviewed and actual session RackReviewComplete after all earlier lesson guards. Old/missing = false; no retrospective reconstruction; no per-cycle keys/counters or J/K/session detail. Expose one shared historical read for T-03/T-04 after the latch while preserving T-03 pre-latch current-world/session safeguards. Round-trip the latch in saves but strip it from every world/hash identity path including non-default prescription worlds, with no rebaseline. Run offline + Unity + independent review; no Windows build.`