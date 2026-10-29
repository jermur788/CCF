# Browser Result — D-066 Minimum Authoritative Teaching-Completion Latch Repair

## Result identity

- **Claim:** #145
- **Parent:** #91
- **Trigger:** Manager review #144 of browser amendment #136
- **Mode:** browser/GitHub only
- **Starting `main`:** `e2cb2bd1cb2ce95018408145d409e0a226b7980a`
- **Exact implementation evidence inspected:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5`
- **Result:** **REPAIRED — READY FOR INDEPENDENT MANAGER REVIEW #158; NO PRODUCTION RELEASE**
- **Readiness token:** `S1-D MINIMAL PERSISTENCE AMENDMENT REPAIRED — READY FOR MANAGER REVIEW`
- **No C#/Unity/save-schema implementation, Unity run or Windows build was performed.**

## 1. Executive result

D-066's accepted Option-A product decision is preserved, but the implementation contract is now narrowed to the demonstrated minimum:

> **One monotone boolean-like Scenario One latch recording that the existing first guided rack teaching sample/review has historically completed at least once.**

The #136 per-cycle key/list/counter model is removed from the repaired authority. No repeated teaching-history records, guide-index cache, readiness cache or broad cycle-state persistence are authorised. Individual `[J]/[K]`, inspected-tree/stand-portion details and the rack-review session transcript remain non-persistent.

The latch is written exactly once at the genuine pre-amendment in-session `response → recruitment` boundary, shared by T-03/T-04 only after it is true, defaults false on older saves, and is excluded from **every world/reference/protected hash identity path** while remaining present in full save serialization.

#143/#124/#126/#128 remain blocked. Independent browser Manager review **#158** is prepared; only an APPROVED #158 can lead to a fresh Manager repin/release of #143.

## 2. Source inspection findings

### 2.1 Authoritative guide transition

At exact qualified lineage `c8cfae8...`, `ScenarioOneTeachingSequence.Current(Facts)` checks the teaching lessons in order. In a rack-world it stays at `response` while:

`!ReleaseReviewed || !RackReviewComplete`

and reaches `recruitment` only after the earlier access/future-tree/release/habitat guards have also passed.

`LearningObjectivesView.Sequence.TeachingFacts()` computes `ReleaseReviewed` from the Annual Review chronology and computes:

`RackReviewComplete = area && rackReview.Complete(bay)`

from the live `ScenarioOneRackReview` session object. `ObserveRackInspection` and `DecideRackInspection` only feed this rack review while the current teaching stage is `response`. The session evidence therefore belongs to the actual current-session review, not to persisted intervention history.

**Repair consequence:** the D-066 write event is fixed to the **unlatched/pre-amendment sequence crossing from `response` to `recruitment` in-session**. Implementation may extract a pure helper, but must prove equivalence to this old boundary before applying the historical latch.

### 2.2 T-03 is deliberately weaker than the D-066 write event

`ScenarioOneTeachingCap.SampleHistoricallyComplete(bay, releaseReviewed)` currently proves that the rack sample applies, the release review exists, release work executed with mixed outcomes and the minimum executed share was reached. Its source explicitly states that session `[J]/[K]` decisions are not used.

That helper remains valid for accepted T-03 pre-latch/current-world safeguard semantics, but **cannot by itself set the D-066 durable latch**. The repair explicitly prevents that conflation.

### 2.3 Save/load architecture

`ForestSaveController.CaptureData()` delegates Scenario One serialization to `ScenarioOneManager.CaptureSaveData()`. Disk load first parses JSON and runs the version-specific `ForestSaveValidation.Validate*Json` chain; both `CanLoad` and `LoadData` then use the shared `FindLoadProblem(...)` validation path. `LoadData` performs its first world mutation only after that shared validation succeeds.

**Repair consequence:** #143 may add only the minimum one-field capture/restore/version plumbing and any validation genuinely required by that field/schema. A boolean latch does not justify the previous duplicate-key/counter/list validation model.

### 2.4 World/hash identity architecture

`ScenarioReferenceArchive.WorldHash` selects legacy-compatible or current identity. `CurrentWorldHash` and `LegacyCompatibleWorldHash` both pass serialized data through `HashLayout(...)`.

On v23, `HashLayout` has an important branch:

- if prescription state is non-default, it returns current JSON with objective-achievement history removed;
- otherwise it strips later default fields and applies the older rack/layout compatibility rules.

Therefore a new teaching field would enter world identity for a real prescription-state world unless it is explicitly stripped from that branch as well as the historical/default branches.

**Repair consequence:** the latch is canonical **save progression but not world identity**. #143 must round-trip it in full saves while stripping/ignoring it in every identity projection: `WorldHash`, `CurrentWorldHash`, `LegacyCompatibleWorldHash`, all `HashLayout` branches, any `MinimalWorldHash`/equivalent protected projection, and protected/reference hash comparisons that consume those layouts. No accepted anchor/fingerprint/reference is rebaselined.

## 3. Repaired D-066 contract

### Persisted shape

Exactly one boolean-like latch in Scenario One save state. Exact C# field/helper names remain implementation details.

Not authorised:

- teaching/management cycle IDs or keys;
- per-cycle records/lists;
- aggregate counters/outcomes used only for teaching history;
- `[J]/[K]` decisions;
- inspected-tree/stand-portion IDs/detail;
- session review transcript/order;
- current guide index;
- current readiness/recommendation state;
- broad cycle-state objects.

If later exact implementation inspection proves one latch materially insufficient, the worker must STOP and return to Manager rather than broaden it.

### Write event

Initial/default value: false.

Permitted mutation: false → true exactly once when all are true in the current rack-world session:

1. the unlatched/pre-amendment teaching sequence has passed all lessons before `response`;
2. the release/review prerequisites that make lesson 5 current are satisfied;
3. `ReleaseReviewed == true`;
4. actual session `RackReviewComplete == true`;
5. evaluating the pre-latch sequence would now return `recruitment` rather than `response`.

Never set from T-03 helper alone, planting, felling, later review, Crop Tree/world drift, load, `PlayerPrefs` or retrospective history inference.

Never reset true → false.

### Old-save meaning

Field absent/default false = historical completion **not recorded**. No backfill or heuristic reconstruction. A legitimate old save remains loadable under existing compatibility policy; the player may have to complete the session review again before the new latch can be earned.

### Shared T-03/T-04 meaning

- Before latch: durable read false. T-03 keeps accepted current-world/session classification; T-04 keeps pre-completion guide behavior.
- At boundary: #143 records true once.
- After latch: both consume the same read-only historical completion API/property. Later world drift/save-load cannot unset it.

The current T-03 helper may remain useful for pre-latch/current-world classification, but is not a second durable history authority.

## 4. Repaired files

The following canonical/task documents were repaired on `main`:

- `Docs/Handoffs/S1-D-Implementation-04/MANAGER-REVIEW.md` — commit `01b7c8526a0accd90dfce2b980bc194437344c75`;
- `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/TASK-PACKET.md` — `e7a4160b255fc9e668b3cd2265a24adca803aa02`;
- `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md` — `616df9305599fe3947c9267ce38cc9db122d2a71`;
- `specs/scenario-one-repeated-interventions-v1/spec.md` — `c7234bb970be1ea437377d0b62b1bc8bf06bf0c9`;
- `specs/scenario-one-repeated-interventions-v1/plan.md` — `20a0bc4673af1688cfb9184606824f93f25ad4af`;
- `specs/scenario-one-repeated-interventions-v1/tasks.md` — `78ae15117e458943871b16fe04a4d57af50fdb27`;
- `specs/scenario-one-repeated-interventions-v1/checklists/requirements.md` — `e1f2d9cf840550279d108b3b3d46edd6e0c573cf`;
- Manager-review handoff for #158 — `8fd27fa3aa7daf42cff76a6d58673533e577c911`.

Decision Log D-066 was **not changed**. The accepted product decision did not need amendment; the defect was in its implementation contract.

## 5. #143 implementation packet — now architecture-complete but still blocked

The repaired packet no longer delegates product design to the local worker. It fixes:

- representation: one boolean-like latch;
- write event: exact unlatched `response → recruitment` transition with real session `RackReviewComplete`;
- old-save default: false/no-backfill;
- post-latch read: one shared T-03/T-04 meaning;
- hash policy: saved but stripped from every world identity path;
- v24: only expected after later live refresh, not reserved;
- verification: explicit incomplete-review, complete-review, one-write, save/load, world-drift and all-hash invariance cases including non-default prescription state.

#143 remains **BLOCKED** until #158 APPROVED plus explicit Manager release/repin.

## 6. #124 continuation boundary

#124 remains a separate curriculum/read-only consumer lane:

- own no save/schema/hash files;
- rebase/replay `b9d204c` only after exact #143 integration;
- before latch preserve existing teaching behavior;
- after latch use the shared historical read as the monotone floor;
- preserve all eight lesson IDs;
- prove T-03/T-04 shared semantics and AC-3 under world drift/save-load.

`b9d204c` remains unqualified evidence only.

## 7. Schema / hash disposition

- Inspected production schema: v23 at `c8cfae8...`.
- v24: **not reserved or released**; merely expected if still next/free when #143 is later released.
- Full save round-trip: latch must be preserved.
- World/reference/protected identity: latch must be absent from identity inputs everywhere.
- Rebaseline: **forbidden**.
- Frozen Reference Future v1 content: unchanged.
- MFM-06/07 prescription/provenance: unchanged.

## 8. Claim / dependency disposition

- **#145:** repair complete; hand off/close after coordination sync.
- **#158:** PREPARED independent Manager review; no production ownership.
- **#143:** BLOCKED pending #158 APPROVED + explicit Manager release; no save/schema/hash ownership.
- **#124:** BLOCKED pending #158 and then exact accepted/integrated #143; no production release.
- **#126:** BLOCKED until exact post-#143/#124 S1-D lineage; old v23-only basis remains obsolete.
- **#128:** BLOCKED behind future repinned #126; review-only.

No local save/schema writer was started by this repair.

## 9. Independent Manager review

Prepared claim: **#158**  
Handoff: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-Manager-Review-20261029/BROWSER-HANDOFF.md`

Required verdict: APPROVED / NEEDS CHANGES / REJECTED.

Only APPROVED allows the Manager to refresh live schema/ownership and issue a new exact release/repin for #143. It does not directly release #124/#126/#128.

## 10. User action

None. The next executable task is browser Manager review #158; local production remains blocked.

`S1-D MINIMAL PERSISTENCE AMENDMENT REPAIRED — READY FOR MANAGER REVIEW`
