# Lab file format v1 (`format = "ccf-sitka-lab"`, `labSchemaVersion = 1`)

Applies to both checked-in fixtures (`fixtures/synthetic/*.sitkalab.json`) and lab saves
(`.lab-data/*.sitkalab.json`). **ENGINEERING TEST FIXTURE data only — never a measured Irish stand.**
Machine-readable shape: [`lab-file-v1.schema.json`](lab-file-v1.schema.json). JSON Schema cannot express
key *order* or the hash rules, so this document is the authority; the reader enforces everything below.

## Canonical bytes

* UTF-8, **no BOM**, **no whitespace and no newline anywhere** (compact), fixed key order, integers only.
* A file is accepted only if re-serialising what was parsed reproduces **the exact input bytes**
  (`NotCanonical` otherwise). `.gitattributes` marks `*.sitkalab.json` as `-text` so Git never changes line endings.
* Strings are restricted to `[A-Za-z0-9_.-]`, so no JSON escaping is ever needed. IDs (`treeId`, `planId`,
  `forestId`, `fixtureId`) are 1–64 characters of `[A-Za-z0-9_-]`.
* Numbers are integers only. Fractions, exponents, `-0`, leading zeros, `NaN`, values that overflow int64 and
  booleans are rejected. Duplicate keys and unknown keys are rejected.
* Numeric-safety bounds (not biological claims): |coordinate| ≤ 1,000,000,000 mm; DBH and height 1…1,000,000,000 mm;
  years ≤ 1,000,000; ≤ 20,000 trees; ≤ 256 polygon vertices; ≤ 5,000 plans; ≤ 200,000 events; ≤ 64 MiB per file.

## Root (property order is fixed)

`format, labSchemaVersion, sourceClass, fixtureId, snapshot, eventLog, snapshotHash`

* The first three properties are read **alone** by the header gate. Anything that is not exactly
  `"ccf-sitka-lab"`, `1`, `"Synthetic"` in that order is rejected as `WrongFormat` **before** `snapshot` or
  `eventLog` is parsed (so CCF game-save shapes and broken bodies never reach the parser).
* `snapshotHash` = lowercase hex SHA-256 over the canonical bytes of the `snapshot` **object only**
  (not the root, not `eventLog`).
* `fixtureId` names the checked-in fixture the history started from. Loading replays `eventLog` from that fixture.

## `snapshot` (order fixed)

`forestId, inventoryYear, experimentStep, forestRevision, adultThresholdMm, standAreaPolygon, siteProfile, trees, plans`

| Field | Rule |
|---|---|
| `standAreaPolygon` | array of `{xMm,yMm}`; one simple ring, ≥ 3 distinct vertices, no holes, non-zero area, **either winding**; self-crossing, self-touching, repeated-vertex and fold-back rings are rejected |
| `siteProfile` | `{slopeDeg:0}` — flat only |
| `trees` | sorted by case-sensitive ordinal `treeId`; each is `treeId, species, xMm, yMm, zMm, dbhMm, heightMm, ageAtInventoryYears, status, markState` |
| tree fields | `species` is `"Sitka"`; `zMm` is `0`; stems **on the boundary count as inside**; any stem outside rejects the whole file; `status` ∈ `live\|dead\|felled`; `markState` ∈ `UNMARKED\|PLAYER_MARKED` |
| mark rule | only a living stem with `dbhMm >= adultThresholdMm` may be `PLAYER_MARKED` |
| `plans` | sorted by ordinal `planId`; each is `planId, planState, treeIds, baseForestRevision, approvalSessionEpoch` |
| plan fields | `treeIds` unique, non-empty, ordinal-sorted, existing trees; `baseForestRevision`/`approvalSessionEpoch` are `null` while `DRAFT`/`PROPOSED`/`REJECTED` and set (both) once approved; a tree is in at most one *open* plan (`DRAFT`,`PROPOSED`,`APPROVED`,`PENDING_WORK`) |

No derived metric (BA, N/ha, Dq, area) is ever persisted.

## Plan state machine (spec A5)

| From | Action | To | Forest effect |
|---|---|---|---|
| tree `UNMARKED` | mark (live adult) | `PLAYER_MARKED` | none |
| tree `PLAYER_MARKED` | unmark (no open plan) | `UNMARKED` | none |
| marked trees | create plan | plan `DRAFT` | none |
| `DRAFT` | propose | `PROPOSED` | none |
| `PROPOSED` | approve (records `baseForestRevision` + `approvalSessionEpoch`) | `APPROVED` | none |
| `APPROVED` | queue test work | `PENDING_WORK` | none |
| `PENDING_WORK` | execute test work | `EXECUTED` | **felled: exactly the listed trees, all or none** |
| `PROPOSED` | reject | `REJECTED` | none, trees released |
| open plan | cancel | `CANCELLED` | none, trees released |
| `APPROVED`/`PENDING_WORK` | revision change, calendar step, or load | `STALE` | none, trees released |

Editing the ID list of an open plan does not exist; cancel/reject and create a new plan (new `planId`).

## Events

Each snapshot-changing action appends exactly one event, in this property order:
`eventSeq, eventType, treeIds, payload, prevStateHash, postStateHash`. `eventSeq` starts at 1 and is gap-free.
`prevStateHash` equals the previous event's `postStateHash` (the first equals the fixture's snapshot hash).
`postStateHash` is the SHA-256 of the canonical snapshot after the event.

| `eventType` | `treeIds` | payload (fixed order) |
|---|---|---|
| `TreeMarked`, `TreeUnmarked` | trees acted on | `{}` |
| `PlanCreated`, `PlanProposed`, `PlanQueued`, `PlanRejected`, `PlanCancelled` | the plan's trees | `{planId}` |
| `PlanApproved` | the plan's trees | `{planId, baseForestRevision, approvalSessionEpoch}` |
| `PlanStale` | the plan's trees | `{planId, reason}` — `ForestRevisionChanged`, `ExperimentStepAdvanced` or `SessionLoaded` |
| `TestWorkExecuted` | felled trees | `{planId, forestRevisionAfter}` |
| `PlanExecutionRejected` | offending trees | `{planId, reason}` — **audit-only: `prevStateHash == postStateHash`** |
| `ExperimentStepAdvanced` | `[]` | `{experimentStepAfter, forestRevisionAfter}` |

Marks/proposals/approvals/queueing never change `trees[].status`. Only `TestWorkExecuted` fells trees, and
`ExperimentStepAdvanced` is a calendar counter: `ageAtInventoryYears`, `dbhMm` and `heightMm` never change.

Rejection reasons: `PlanNotPending`, `PlanAlreadyExecuted`, `PlanStale`, `PlanCancelledOrRejected`,
`RevisionMismatch`, `EpochMismatch`, `TreeDuplicate`, `TreeMissing`, `TreeNotLive`, `TreeBelowThreshold`, `TreeNotReserved`.

## Session epoch, staleness and replay

* `sessionEpoch` is an **opaque runtime string** (128 random bits as hex), fresh on every startup, reset and load.
  It is *not* stored at root level. An approval stores the epoch it happened under in its own event payload and plan.
* Replay takes the historical epoch **from the `PlanApproved` payload**, never from the live runtime.
* On load, every `APPROVED`/`PENDING_WORK` plan becomes `STALE` by a recorded `PlanStale` (`SessionLoaded`) event —
  even if the numeric revision still matches. This is an intentional state change, so the hash after loading a file
  with outstanding approvals differs from the saved hash. Files with no outstanding approvals reload byte-identically.
* Execute checks, **before any mutation**: plan is `PENDING_WORK`; revision and epoch tokens match; every ID is unique,
  exists, is alive, is at/above the adult threshold and is reserved to this plan. One failure ⇒ nothing is felled and
  one audit-only `PlanExecutionRejected` event is appended.

## Error codes (stable tokens)

`WrongFormat, JsonInvalid, DuplicateKey, UnsupportedJsonValue, NonIntegerNumber, NumberOverflow, SchemaInvalid, NotCanonical,
HashMismatch, InvalidId, InvalidValue, InvalidPolygon, OutsidePolygon, DuplicateId, NotSorted, InvalidMark, InvalidPlan,
UnknownFixture, ReplayMismatch, PathRejected, NotListed, SymlinkRejected, NotRegularFile, FileTooLarge, UnsafeString`.
