# S1-C T-07 — Data Contract Stability Handoff

**Result:** `S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW`

**Claim:** #187

**Parent:** #91 / local umbrella #106

**Mode:** offline verification/docs only; no Unity, no production repair
**Date:** 2026-10-30

## Lane

| Field | Value |
|---|---|
| Start SHA | `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` |
| Verification/implementation HEAD | `e64b337efd3b758fe6bc21636822ad178f0df0ca` |
| Handoff commit | `8228a266` (docs-only child of `e64b337e`) |
| Branch | `verify/s1-c-t07-data-contract-v1` |
| Worktree | `/media/jer/Videos/CCF-S1C-T07` |
| Provider/model | opencode-go / deepseek-v4.1-flash |
| Unity | not used; no Unity lock taken |
| Worktree return | clean at the verification run; handoff commit is the only later change |

The start SHA is the exact released repin. The handoff publication commit is `8228a266`. The verification run recorded `lane_status_before = lane_status_after = ""` at harness commit `e64b337e`, so the gate wrote nothing inside the repository.

## Outcome

The existing S1-C management-history reader and the persisted identity contract are stable, read-only and deterministic on:

1. a genuine pre-S1-C **v20 year-5** save;
2. a genuine recorded **v22 year-3** S1-C save;
3. a deliberately constructed cancellation/vanished-order semantic case (not presented as a save).

Passed **304/304 checks**. All **8/8** negative/mutation controls were caught. No production, save/schema, validator, UI, MFM or reference-future file changed.

## Genuine fixtures and provenance

| Case | Path | Bytes | SHA-256 | Classification |
|---|---|---:|---|---|
| old v20-y5 | `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures/v20-y5.json` | 1,334,812 | `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00` | genuine pre-S1-C save |
| current v22-y3 | `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/02-objective/evidence/recorded-v22-year3-save.json` | 1,301,407 | `95e5dd37d7f546d78cb94cda8e5af1641b576c925cc37313ce88aa0a54167b9e` | genuine recorded S1-C save |

The runner pins both digests and refuses a differently-labelled file.

### Old-save evidence (the care point)

The old save is **not** a synthetic fixture renamed `v20-records`.

- It was copied byte-for-byte from the real rendered/player save at
  `/media/jer/Files/CCF/Verification/ScenarioOneRackFirstUx-20261017/ux-run1/config/unity3d/DefaultCompany/CCF/forest-save.json`.
  That origin is recorded in `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures/manifest.json`.
- The origin file still exists today and its SHA-256 was re-checked as `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- The supported-load status is established by prior Unity evidence, not assumed here:
  `Docs/Handoffs/S1-C-Implementation-05/UNITY-HANDOFF-V22.md` (genuine old-save section) records that genuine v20/v21 saves passed both the typed and disk load routes, migrated to an empty in-memory history, made no retrospective write, and re-saved as v22 with an explicit empty list. The v20-y5 row and digest are listed there.
- This gate re-parses the exact same bytes offline, applies the real production `ScenarioOneSaveMigration` normalizers for version 20, and runs the real `ScenarioManagementHistory` reader. It does **not** re-run Unity load. That distinction is deliberate: T-07 is the offline contract gate, and no Unity lane was requested.

Old-save facts proved:

- save version 20, ecological year 5; events `1..17` with next counter `18`; orders `1..3` with next counter `4`;
- completed-year views are years `1..5`; their 15 events are exactly the raw events of those years, in event order. Year-0 order events `1..2` are correctly outside a completed-year view because year 0 has no annual report or `YearAdvanced` event;
- resolved events are `3 → order 1` and `15 → order 2`; both order records still exist and link by reference; pending order `3` remains outside the resolved set;
- year-1 settlement is event `3`, label `1`, grouped orders `[1]`; year-5 settlement is event `15`, label `5`, grouped orders `[2]`;
- `harvestJobId = 5` is shared by two orders (resolved order 2 and pending order 3), proving it is a resolution-year label rather than a globally unique job ID; pending order 3 is not fabricated into the settled group;
- `objectiveAchievements` is absent from the source JSON and parses as an empty list; every view reports no achievement; nothing is synthesized;
- `prescriptionId`, prescription references, `rackId` and prescription records are absent from the source; after production migration they remain `0`/empty and `nextPrescriptionId` is the explicit default `1`;
- no deadwood or storm records exist in this save; none are invented.

### Current save facts

The v22-y3 save is the genuine recorded save used by the accepted T-05/T-06 evidence. Its source role is recorded in `Tools/Verification/S1CManagementHistory/AnnualReviewGuidanceVerification.cs` (real pre-feature and recorded v22 saves loaded through the production typed route).

- save version 22, ecological year 3; events `1..10`, next counter `11`; orders `1..2`, next counter `3`;
- completed-year views are years `1..3`; all 10 events appear exactly once;
- year-2 settlement is event `4`, label `2`, grouped orders `[1]`; year 3 has no harvest settlement;
- order 2 is a completed planting with `harvestJobId = -1`; its own event money passes through and it is not marked grouped;
- five objective achievements pass through by reference (`retained-canopy@1`, `continuous-canopy@1`, `regeneration@1`, `managed-opening@2`, `introduced-sessile-oak@3`);
- no prescription state is present; after migration it remains `0`/empty.

The current integration is v24, but v23/v24 only append prescription and guided-review fields. The history reader last changed for T-05/T-03/T-01, and no later commit changes `ScenarioManagementHistory.cs` or the event/work-order identity fields. The v22 recorded save is therefore the appropriate genuine S1-C-era current case. The constructed case below uses the latest-shaped defaults but is not claimed as a genuine save.

## Constructed semantic case

The genuine saves contain no cancellation, so this case supplies only the missing cancellation/vanished-order semantics. It is constructed in the check and labelled as such.

- cancelled order `900` survives only as event `2`; no order object exists; drill-down uses `EventId = 2`, `WorkOrderId = 900`, kept tree/cell/task, no execution method and no money;
- resolved order `901` has no order record; its resolution event `3` survives with `WorkOrderId = 901` and no fabricated order;
- resolved order `902` links by reference to the surviving order;
- grouped settlement is event `4`, label `1`, grouped orders `[901, 902]`; the earlier grouped event `3` keeps zero money;
- one objective achievement, deadwood record, storm record and both snapshots pass through by reference.

## Identity and contract evidence

| Case | Event IDs | Next | Order IDs | Next | Views | Resolved linkage |
|---|---|---|---|---:|---|---|
| old v20-y5 | `1..17` | 18 | `1..3` | 4 | 1..5 | event 3 → order 1; event 15 → order 2; pending order 3 unresolved |
| current v22-y3 | `1..10` | 11 | `1..2` | 3 | 1..3 | event 4 → order 1; event 9 → order 2 |
| constructed | `1..6` | 7 | `902,903` | 904 | 1 | event 3 → vanished 901; event 4 → 902; cancelled 900 event-only |

Proved by the check:

- all event and order IDs are positive and unique; next counters are past every used ID;
- every view event/order/cancellation/achievement/deadwood/storm/snapshot is the same object reference as its input record (no cloning, renumbering or re-keying);
- drill-down `EventId`, `WorkOrderId`, tree ID, species ID, cell index (including `-1`), position and position-presence equal the recorded event fields;
- existing resolved orders link by reference; missing orders stay missing rather than being fabricated; cancellations keep the surviving event's authoritative work-order ID;
- repeated queries on a freshly parsed copy mutate no record, collection, view input or next-ID counter;
- each query builds fresh lists/views while still returning the same pass-through event references;
- the query types own no static field or property cache/store/generator; their source contains no counter writes, `Guid`/`Random` generator, or duplicate-ID validator;
- the production `ForestSaveValidation` remains the sole management-event ID validator;
- grouped harvest items carry no own money; the settlement event carries the whole recorded job; non-harvest successful items keep their own recorded money; failed/cancelled items invent none.

## Negative and mutation controls

In-gate data control (production validator):

- distinct positive IDs accepted;
- duplicate ID rejected with `management event ID 2 appears twice (records 1 and 2)`;
- non-positive ID rejected;
- rejection does not renumber, repair or reorder the event list.

Source mutations, all compiled from deliberately broken copies outside the repository:

| Mutation | Result | Failing checks |
|---|---|---:|
| reader renumbers existing events | caught | 3 |
| reader re-keys work orders | caught | 9 |
| cancellation fabricates a replacement ID | caught | 1 |
| `harvestJobId` treated as a unique job identity | caught | 3 |
| grouped harvest invents per-order money | caught | 9 |
| query mutates an input order | caught | 3 |
| harvest label becomes the order identity | caught | 3 |
| duplicate event ID accepted by the validator | caught | 1 |

**8/8 caught.** No mutation survived.

## Read purity / counter fingerprints

Fingerprint scope: first 16 hex of SHA-256 over canonical `System.Text.Json` serialization (public fields included) of the parsed `scenarioOne` record graph, before/after two complete `BuildYearViews` + `DescribeWork` passes. Purity is measured on a fresh parse so a reader that mutates on every call cannot hide behind earlier queries. Next-ID counters, collection sizes and identity sets are asserted separately.

| Case | Purity fingerprint | Counter/identity result |
|---|---|---|
| old v20-y5 | `7b1ef25837784c8d` | `nextManagementEventId` 18, `nextWorkOrderId` 4 unchanged; second parse returns the same IDs |
| current v22-y3 | `fa5272f0cc9b8027` | `nextManagementEventId` 11, `nextWorkOrderId` 3 unchanged; second parse returns the same IDs |
| constructed | `df7fe118db02936a` | counters and record graph unchanged |

The identity dump also survived a genuine second parse of each genuine file, including the per-year history output and resolved-order ID sets.

## Commands and results

Run from `/media/jer/Videos/CCF-S1C-T07` at `e64b337e`:

```bash
python3 Tools/Verification/S1CManagementHistory/run_t07_gate.py \
  --old-save /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures/v20-y5.json \
  --current-save /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/02-objective/evidence/recorded-v22-year3-save.json \
  --out /media/jer/Files/CCF/Verification/S1C-T07-Offline-20261030 \
  --mutations
```

Observed:

- main gate: `S1C_T07_OFFLINE_PASS checks=304 failures=0`;
- mutations: `8/8` caught;
- runner: `S1C_T07_OFFLINE_PASS ... mutations=8 caught=8`;
- no build/test writes in the repository; no Unity process; no disk save touched.

Evidence:

- `/media/jer/Files/CCF/Verification/S1C-T07-Offline-20261030/t07-offline-result.json`
- `/media/jer/Files/CCF/Verification/S1C-T07-Offline-20261030/t07-offline-stdout.txt`

The result JSON records the exact fixture digests, branch/HEAD, lane status before/after, source SHA-256 table and each mutation's failing checks.

## Changed files

Added:

- `Tools/Verification/S1CManagementHistory/run_t07_gate.py`
- `Tools/Verification/S1CManagementHistory/S1CDataContractStabilityOfflineCheck.cs`
- `Tools/Verification/S1CManagementHistory/T07OfflineStubs.cs`
- `Docs/Handoffs/S1-C-Implementation-07/HANDOFF.md` (this file)

Not changed: production C#, save version/schema/migration/validator/hash/reference behavior, `WorkPlanView.cs`, `AnnualReviewView.cs`, `ScenarioOneUiRoot.cs`, MFM provenance architecture, history UI/query/store architecture, Unity assets/scenes/prefabs, Windows build.

## Verification classification and limits

Directly verified offline:

- identity pass-through/positive uniqueness, linkage, cancellation/vanished-order meaning, reload stability;
- read purity/counter stability and absence of a second static identity store;
- `harvestJobId` as a resolution-year label and grouped-settlement money honesty;
- genuine old-save record identities and absent newer provenance;
- production duplicate/non-positive event-ID rejection and eight mutation controls.

Not verified here:

- Unity import, scene/runtime manager wiring, `JsonUtility` or the disk save/load path;
- rendered Annual Review/Work Plan UI;
- full-save validation/hash/protected anchors;
- live gameplay or new save writes.

The genuine v20/v22 load support is prior Unity evidence cited above; this offline lane uses the same real bytes and production normalizers but does not claim to have repeated that Unity run. If Manager requires a fresh Unity old-save load for T-07 specifically, that is a separate Unity gate (normally S1-C T-08), not this offline task.

## Manager review focus

1. Accept the old-save provenance and the offline/Unity evidence boundary.
2. Inspect the mutation table and the two genuine fixture digests.
3. Confirm the diff is verification/docs only and the worktree is clean.
4. If accepted, T-08 can be repinned/released against the current integration; this handoff does not release it by itself.
