# S1-D T-04 — Guide Monotonicity — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-04 — Guide monotonicity
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Claim:** #124
- **Status:** **BLOCKED PENDING #158 APPROVAL AND THEN #143 MANAGER-APPROVED / INTEGRATED PERSISTENCE PREREQUISITE**
- **Historical base:** `b971f4da0540e606bfee9702b0c99f81a0a89355`
- **Historical pushed candidate:** `b9d204cbae687da71d32a9da42575ca3f298a511` — useful evidence only; not accepted, not Unity-qualified, not an integration base.
- **Purpose:** finish guide monotonicity after the repaired D-066 one-way teaching-completion latch exists on an exact integrated lineage.
- **Difficulty:** 4/5 — bounded curriculum surface with subtle historical/current-state separation.
- **Preferred executor:** existing #124 curriculum owner when explicitly re-released; one primary writer only.
- **Independent review:** required on the exact final candidate.

Suggested continuation lane:

- branch `feature/s1-d-t04-guide-monotonicity-v1`
- worktree `/media/jer/Videos/CCF-S1D-T04`

Do **not** resume #124 until Manager explicitly releases it on the exact integrated #143 lineage.

## Repaired D-066 authority consumed by T-04

#143 is to persist exactly one boolean-like historical fact:

> **The existing first guided rack teaching sample/review has historically completed at least once.**

That latch is set only when the unlatched/pre-amendment rack-world guide genuinely crosses `response → recruitment` in-session after all earlier lesson guards, `ReleaseReviewed`, and actual session `RackReviewComplete` are satisfied.

T-04 does not decide, reconstruct, write or validate that fact. It consumes the #143 read-only API after #143 has been qualified, Manager-approved and integrated.

Old/pre-amendment saves with no latch read false until the player actually completes the authoritative session review and the new latch is recorded. No planting/felling/review surrogate or retrospective reconstruction is permitted.

## One shared T-03 / T-04 read meaning

T-04 and T-03 must share exactly one durable historical meaning after completion:

- **Before latch:** shared historical read = false. T-03 retains its accepted current-world/session cap logic and deadlock safeguard; T-04 preserves current pre-completion guide behavior. No durable fact fabricates session review completion.
- **At the authoritative current-session boundary:** #143 records the latch once.
- **After latch:** shared historical read = true forever. T-03 treats the first teaching sample as historically closed and T-04 uses the same fact as the monotone historical floor.

Do not build a T-04-only historical interpretation, and do not treat `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` as equivalent to the durable latch: that current-world helper deliberately omits session `RackReviewComplete`.

## Required execution order

1. #145 publishes repaired documents/result.
2. #158 independently reviews the repaired contract.
3. Only if #158 is **APPROVED**, Manager may repin/release #143 after live schema/ownership refresh.
4. #143 implements, independently reviews, qualifies, Manager-approves and integrates the one-way latch + all-world-hash exclusion.
5. Manager repins #124 to that exact integration and releases curriculum ownership.
6. #124 rebases/replays useful logic from `b9d204c`; do not stack blindly.
7. #124 completes offline + Unity qualification + independent review.
8. PARITY-08 #126 remains blocked until the exact post-#143/#124 lineage is known; #128 remains behind #126.

## Required behavior

Implement FR-B1 / FR-B2 / FR-B3 / AC-3 using the integrated #143 API:

- preserve every pre-latch/current-session lesson behavior;
- when durable historical completion is true, never regress behind the completed `response` boundary because of later Crop Tree count increase/decrease, Crop Tree loss, later in-footprint felling, executed-share drift, or save/load;
- use the durable fact as historical teaching proof only, not as forestry prescription, current-world observation, Scenario One completion input, MFM state or Rack V2 state;
- do not read `PlayerPrefs` or session-only `[J]/[K]` state as durable authority;
- do not serialize `[J]/[K]` or any session transcript;
- preserve all stable lesson IDs exactly: `access`, `future-trees`, `release`, `habitat`, `response`, `recruitment`, `establishment`, `transformation`;
- do not rename/reuse persisted/teaching identities.

The simplest acceptable shape is a monotone guide floor that cannot return behind `recruitment` once the shared historical latch is true, while later derived/current facts continue to drive `recruitment`, `establishment` and `transformation` normally. Do not persist the current guide index.

## Expected owned production surface when released

Primary:

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`
- focused verification files.

Allowed only if strictly necessary:

- the smallest read-only adapter needed to consume #143's already-integrated historical completion API.

**Not owned by #124:** `ForestSaveData.cs`, save capture/restore, validators, schema version, `ScenarioReferenceArchive` hash code, #143 persistence implementation, completion rules/objectives, economy/ecology, Rack V2, MFM implementation, S1-C history storage, scenes/prefabs/packages/ProjectSettings.

If the #143 API is insufficient, STOP and return to Manager. Do not expand #124 into a second persistence/hash writer.

## Required verification

### Offline/static matrix

Cover at least:

- response lesson with durable latch false and incomplete session review;
- response lesson with `ReleaseReviewed` true but `RackReviewComplete` false — guide stays at `response`, durable read remains false;
- authoritative transition case where the unlatched sequence reaches `recruitment` and #143 historical read is true;
- after latch: Crop Tree count increase/decrease;
- after latch: Crop Tree loss;
- after latch: later in-footprint felling;
- after latch: executed-share drift;
- after latch: save/load/rebuild at lessons 6–8 without regression;
- pre-amendment/missing-latch save remains conservative false until authoritative completion is newly earned;
- repeated reads/calls never write/consume another sample;
- all eight lesson IDs unchanged.

Regression requirements:

- TeachingCopy / relevant MenuTutorial;
- T-03 teaching-cap decision tests proving the same post-latch historical read and unchanged pre-latch current-world/session behavior;
- T-02 cycle-state checks;
- #143 save/load/hash regression suite;
- S1-C history/query checks;
- N3 normalised completion-rule source/hash unchanged — no rebaseline.

### Unity qualification

Under the machine-wide Unity lock on the exact candidate:

- compile/import;
- prove pre-latch `response` behavior is unchanged;
- prove `ReleaseReviewed` without complete rack review does not skip `response`;
- complete the actual session rack review and prove progression reaches `recruitment`;
- save/load at later lessons and prove no regression behind the historical boundary;
- exercise Crop Tree loss/count drift, later felling and executed-share drift after historical completion;
- prove view refreshes/repeated calls do not mutate persistence.

### Independent review

Reviewer must confirm:

- T-04 consumes exactly the #143 shared historical latch and owns no persistence/hash code;
- T-03/T-04 post-latch historical meaning is identical;
- pre-latch current-world/session behavior is preserved;
- no surrogate proof or retrospective inference exists;
- no lesson ID rename/reuse;
- no completion/objective/economy/ecology/Rack/MFM/Reference change;
- no weakened test or hash/reference rebaseline.

## Stop conditions

Stop and return to Manager if:

- #158 is not APPROVED;
- #143 is not Manager-approved/integrated on the exact starting lineage;
- live coordination has another curriculum writer on the same files;
- the #143 shared read cannot satisfy AC-3 without another persisted field;
- any save/schema/hash edit is requested from #124;
- completion rules/objectives or other protected systems would need to change;
- MFM has already rewritten the same curriculum seam and requires a fresh ownership/integration decision;
- a protected hash/reference would require rebaseline.

## Handoff

Publish `Docs/Handoffs/S1-D-Implementation-04/HANDOFF.md` with actual model/provider, exact #143 integration start SHA and ending production SHA, branch/worktree, changed files, exact shared historical read API consumed, lesson-floor mapping, pre-latch behavior, offline + Unity evidence, independent-review verdict, stable-ID proof, T-03 shared-semantics regression and N3/hash preservation.

On successful handoff mark #124 `[HANDED-OFF]`, link the handoff, close the execution claim and return:

`S1-D GUIDE MONOTONICITY QUALIFIED — READY FOR MANAGER REVIEW`

or a precise blocker.

## Copy/paste dispatch line — only after exact #143 integration + Manager release

`Resume claim #124 only after #143 is Manager-approved/integrated and this packet is repinned. Rebase/replay T-04 on that exact lineage and consume #143's single durable first-guided-sample/review completion read. Preserve pre-latch response/RackReviewComplete behavior; after latch never regress behind recruitment because of Crop Tree drift/loss, later felling, executed-share drift or save/load. T-03/T-04 must share the same post-latch historical meaning. Own no save/schema/hash code, persist no J/K/session detail, preserve all lesson IDs, rerun offline + Unity + independent review, and do not build Windows.`