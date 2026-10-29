# Scenario One Second and Subsequent Interventions (S1-D) Implementation Plan

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Status:** **D-066 repair proposed by browser #145; production blocked pending Manager review #158.**
- **Plan owner:** Overall Manager
- **Last updated:** 2026-10-29

## 1. Architectural rule

S1-D remains **derived-first**. Current advisory/readiness state, previous/proposed classification and later-cycle guidance continue to derive from authoritative persisted world/management records.

The only D-066 persistence exception is **one monotone boolean-like historical teaching-completion latch** for the existing first guided rack teaching sample/review. Its meaning is exactly:

> The first guided rack sample/review has historically completed at least once.

No per-cycle key, counter, list, aggregate record set, guide-index cache or broad `ScenarioOneManagementCycleState` is authorised. Individual `[J]/[K]`, inspected-tree/stand-portion detail and session transcript remain non-persistent.

## 2. Exact current-session write transition

Static inspection of exact qualified lineage `c8cfae8c4025cec6e960a050edf4bfde2a981de5` pins the authoritative boundary.

`ScenarioOneTeachingSequence.Current(Facts)` reaches `recruitment` from `response` only when all earlier lesson guards have passed and, in the rack-world path:

- the existing access/future-tree/release/habitat prerequisites are satisfied;
- `ReleaseReviewed == true`;
- `RackReviewComplete == true`, where `LearningObjectivesView.Sequence.TeachingFacts()` obtains it from live `ScenarioOneRackReview` session state.

Therefore T-04P writes the latch exactly once only when the **unlatched/pre-amendment sequence would cross `response → recruitment` in the current session**. The implementation may extract a helper, but verification must prove semantic equivalence before any historical floor is applied.

`ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` alone is not a write witness: its accepted T-03 purpose deliberately omits session rack-review completion. Planting, arbitrary felling, later Annual Review acknowledgement, Crop Tree count/loss, geometry, executed-share drift, `PlayerPrefs`, save age or heuristic reconstruction are also insufficient.

## 3. T-03 / T-04 shared meaning

Use one historical read API after T-04P:

- **Before latch:** read false. T-03 keeps its existing current-world/session cap classification and deadlock safeguard. T-04 keeps pre-completion sequence behavior.
- **At boundary:** T-04P records true once.
- **After latch:** read true forever. T-03 treats the first sample as historically closed; T-04 uses the same fact as its monotone floor. Later world drift and save/load cannot unset it.

Do not create two durable interpretations. T-03 may retain current-world helpers for pre-latch classification, but the latch is the sole persisted historical truth.

## 4. Other implementation slices

### 4.1 T-02 advisory cycle state

Keep one pure `UnityEngine`-free derived adapter over existing persisted records. Report named components/reasons: approved work, pending/carried-over work, stale/invalid orders, latest resolved intervention review state and descriptive years since intervention. No persistence, gate or prescribed interval.

### 4.2 T-03 teaching cap

Keep the accepted current-world/session safeguard behavior before D-066 completion, including its deadlock avoidance. After the shared historical latch is true, the historical teaching activity is closed and the footprint fact may only be advisory/non-blocking. MFM ceilings are unrelated and unchanged.

### 4.3 T-04 guide monotonicity

After T-04P is integrated, replay/rebase the old T-04 candidate rather than stacking it. Consume the shared historical read only. The smallest acceptable guide change prevents regression behind `recruitment` after true while leaving `recruitment`/`establishment`/`transformation` driven by current facts. Persist no guide index.

### 4.4 T-05 previous vs proposed

Reuse S1-C history. Work Plan may derive Approved / Pending new / Pending carried over from existing order status/year. No new date/provenance field and no duplicate history UI.

### 4.5 T-06 event-ID integrity

Reject duplicate/non-positive management-event IDs before world/disk mutation; no repair/renumbering. Sequence so T-06 and T-04P are never concurrent save/validator writers.

### 4.6 T-07 broad persistence

**Cancelled.** D-066 does not reopen broad cycle state.

## 5. T-04P persistence architecture

At the inspected base, save schema is v23. v24 is only an expected next slot if the later released worker refresh still finds v23 current/free and no competing writer. Browser #145/#158 reserves nothing.

Expected production surface when released:

- one boolean-like Scenario One save field;
- minimum save capture/restore plumbing;
- one-way authoritative write hook + read-only historical API;
- minimum save-version/validation changes genuinely required;
- minimum `ScenarioReferenceArchive` hash-layout compatibility edit needed to exclude the latch everywhere;
- focused verification.

Old/missing field = false/not-recorded. Never backfill completion.

A boolean latch does not justify per-record keys/counters or a new aggregate-history validator. Preserve existing pre-mutation load guarantees and add only validation required by the actual one-field schema/version change.

## 6. World identity / hash policy

The latch is persisted progression but **not world identity**.

Save JSON must round-trip it; every identity projection must remove/ignore it:

- `ScenarioReferenceArchive.WorldHash`;
- `CurrentWorldHash`;
- `LegacyCompatibleWorldHash`;
- every `HashLayout` branch, including the branch for non-default v23 prescription state;
- any `MinimalWorldHash` or equivalent protected/minimal projection;
- protected reference/hash extraction/comparison consuming those layouts.

The current v23 non-default prescription-state branch strips only explicitly named history before hashing, so the new teaching field must be explicitly excluded there too. Use the smallest compatibility edit. **No reference archive, anchor or protected fingerprint is rebaselined.**

## 7. Integration boundaries

Potential T-04P surfaces: `ForestSaveData.cs`, the smallest Scenario One capture/restore/helper, existing validation only as needed, `ScenarioReferenceArchive` hash-layout compatibility, focused verification.

Potential T-04 surfaces: `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`, `LearningObjectivesView.Sequence.cs`, read-only #143 adapter if needed, focused verification.

Do not change `ScenarioOneCompletionRules.cs`, objective predicates/targets, economy/ecology/growth/regeneration, Rack V2 lifecycle/geometry, MFM prescription semantics, S1-C history storage, scenes/prefabs/packages/ProjectSettings or Reference Future archive content.

## 8. Ownership and execution sequence

1. #145 publishes repaired contract/result.
2. #158 performs independent browser Manager review.
3. Only **APPROVED** #158 allows Manager to refresh/repin/release #143.
4. #143 becomes sole save/schema/hash writer for the one-way latch, qualifies + independent-reviews, then Manager approves/integrates it.
5. Manager repins/releases #124 on exact #143 integration. #124 owns curriculum only and no save/hash code.
6. #124 qualifies + independent-reviews.
7. #126 PARITY-08 remains blocked until exact combined lineage is known; repin then.
8. #128 remains independent review behind #126.
9. T-05 coordinates with S1-C owner; T-06 never overlaps save/validator ownership.
10. T-08 integrated multi-cycle witness and T-09 independent spec review follow required implementation completion.

No concurrent writer on overlapping files. Heavy Unity remains machine-serialized. No Windows build before user playtest authorisation.

## 9. Verification strategy

### T-04P

Offline/static must prove:

- false/default old-save behavior;
- false→true exactly once; no true→false path;
- write helper equivalent to unlatched `response → recruitment`, including `ReleaseReviewed` and actual `RackReviewComplete`;
- T-03 current helper alone cannot set latch;
- repeated read/refresh/load does not write;
- save/load round-trip false and true;
- no `[J]/[K]`/inspection/session details serialized;
- T-03 pre-latch behavior unchanged;
- shared post-latch read semantics;
- N3 source/hash unchanged;
- otherwise-identical false-vs-true worlds have identical world/protected hashes on every path, including non-default prescription state;
- no baseline/reference rebaseline;
- MFM-06/07 save/provenance regressions green.

Unity must witness response incomplete → review acknowledged but rack review incomplete → actual rack review complete → one latch write → save/load true → later world drift still true; plus old-save false defaults.

Independent review mandatory.

### T-04

Offline/Unity matrix: pre-latch response unchanged; post-latch no regression under Crop Tree count/loss, later felling, executed-share drift, save/load at later lessons; repeated reads non-mutating; all stable lesson IDs unchanged; T-03 shared-semantics regression; TeachingCopy/MenuTutorial/T-02/S1-C/N3 green.

Independent review mandatory.

### Integrated S1-D

T-08 covers ≥3 real cycles, idle years, stale pending order, save/load at each stage, completion and continued management. T-09 closes requirements independently.

## 10. Risks / stop conditions

STOP if:

- #158 does not approve;
- live schema/ownership changes before #143;
- one boolean latch proves insufficient and broader persistence appears necessary;
- faithful completion would require individual `[J]/[K]`/session detail;
- `RackReviewComplete` cannot serve the actual current-session boundary without changing product meaning;
- the latch cannot be excluded from every world identity path without changing accepted hash semantics;
- any accepted old save cannot default false under current compatibility policy;
- protected completion/objectives/economy/ecology/Rack/MFM/Reference behavior would change or a protected hash/reference would need rebaseline.

Return material ambiguity to Manager rather than broadening locally.