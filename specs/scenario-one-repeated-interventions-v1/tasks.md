# Scenario One Second and Subsequent Interventions (S1-D) Bounded Tasks

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Status:** **D-066 repaired contract pending Manager review #158; #143/#124/#126/#128 remain blocked.**
- **Owner:** Overall Manager
- **Last updated:** 2026-10-29 — browser repair #145

Common exclusions: no change to `ScenarioOneCompletionRules.cs`, objective predicates/targets, economy/ecology coefficients, Rack V2 lifecycle/geometry, MFM prescription semantics, frozen Reference Future content, scenes/prefabs/packages/ProjectSettings or protected hashes/anchors. No Windows build. The only newly authorised S1-D persistence proposed by the repaired D-066 contract is **one boolean-like first-guided-sample/review historical completion latch**, and it is not released until #158 approves.

## T-00 — Product decisions

- **Status:** COMPLETE.
- Original decisions RD-1…RD-12 and user-selected D-066 Option A are canonical.
- Browser #145 repairs implementation authority only; it does not change the accepted product decision.

## T-01 — Baseline re-verification

- **Status:** COMPLETE / Manager-accepted for current programme lineage.
- Implementation workers must still refresh exact lineage/ownership at task start.

## T-02 — Pure derived cycle state/readiness

- **Status:** authorised on its accepted dependencies.
- Derive named advisory components/reasons from authoritative records.
- No persisted readiness/cache, no gate, no interval, no prescribed treatment.

## T-03 — Teaching-footprint cap decoupling

- **Status:** accepted implementation slice exists; its durable-history interaction is constrained by repaired D-066.
- Before D-066 latch: preserve accepted current-world/session classification and deadlock safeguard.
- After latch: consume the same read-only historical completion meaning as T-04; the first teaching sample is historically closed and the cap is at most advisory/non-blocking.
- `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` may remain a pre-latch/current-world helper but is **not** sufficient to write the durable latch because it omits actual session `RackReviewComplete`.
- No second durable T-03 interpretation/store.

## T-04P — One-way teaching-completion persistence prerequisite

- **Claim:** #143
- **Status:** **BLOCKED PENDING MANAGER REVIEW #158 — NO OWNERSHIP / DO NOT START.**
- **Purpose:** persist one monotone boolean-like fact: the first guided rack teaching sample/review has historically completed at least once.
- **Inspected base:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, save v23.
- **Expected next schema:** v24 only if #158 approves, Manager later releases #143, and live start refresh still finds v23 current/free with no competing writer.

### Required write semantics

Write false→true exactly once only when the **unlatched/pre-amendment rack-world `ScenarioOneTeachingSequence` would cross `response → recruitment` in the current session**. This requires all earlier lesson guards to pass and specifically `ReleaseReviewed == true` and actual session `RackReviewComplete == true`.

Do not infer/write from T-03's `SampleHistoricallyComplete` alone, planting, arbitrary/in-footprint felling, later Annual Review, Crop Tree count/loss, current geometry, executed-share drift, `PlayerPrefs`, load or historical heuristic.

### Persisted shape

Allowed:

- one boolean-like Scenario One save field/latch;
- minimum version/capture/restore plumbing;
- one-way write/read helper;
- minimum hash-layout compatibility edit to exclude the field from identity everywhere;
- validation only as genuinely required by the one-field schema/version change;
- focused verification.

Forbidden:

- per-cycle key/list/history records;
- sample/review counters or replay state;
- individual `[J]/[K]`, inspected-tree/stand-portion/session transcript;
- guide index, readiness/advisory cache or broad cycle-state object.

### Old-save / hash contract

Field absent/default false = not historically complete; no backfill. Save serialization round-trips the field, but every world/hash identity path strips/ignores it, including `WorldHash`, `CurrentWorldHash`, `LegacyCompatibleWorldHash`, all `HashLayout` branches including non-default v23 prescription state, any minimal/protected projection and protected reference/hash comparisons. **No rebaseline.**

### Verification

Offline + Unity + independent review must prove: exact pre-amendment transition equivalence; incomplete rack review does not latch; one write only; save/load persistence; old-save false default; T-03 pre-latch behavior unchanged; shared post-latch read semantics; no J/K detail serialized; all world/protected hashes invariant between otherwise-identical false/true latch worlds including prescription-state case; N3/MFM/reference regressions green with no rebaseline.

### Stop conditions

Stop on #158 non-approval, changed schema/ownership/lineage, need for broader persistence, need for J/K/session detail, inability to use actual RackReviewComplete boundary, or inability to exclude field from identity without changing accepted hash semantics.

## T-04 — Guide monotonicity

- **Claim:** #124
- **Status:** **BLOCKED PENDING #158, THEN #143 MANAGER-APPROVED / INTEGRATED.**
- Existing `b9d204c` is unqualified evidence only; replay/rebase after exact #143 integration.
- Own curriculum/read-only consumption only; no save/schema/hash files.
- Before latch: preserve current teaching behavior.
- After latch: consume #143's single shared historical read; never regress behind completed `response` boundary because of Crop Tree count/loss, later in-footprint felling, executed-share drift or save/load.
- Preserve all eight lesson IDs.
- Verify shared T-03/T-04 meaning, TeachingCopy/MenuTutorial/T-02/S1-C/N3, Unity save/load/world-drift matrix and independent review.

## T-05 — Previous versus proposed presentation

- **Status:** separately sequenced with S1-C history/review ownership.
- Derive Executed / Approved / Pending new / Pending carried over / unplanned Fell marks from existing records.
- Reuse S1-C history; no designation-year field, dated map layer or duplicate history store/UI.

## T-06 — Management-event ID integrity

- **Status:** authorised on its accepted dependencies but must not overlap T-04P save/validator ownership.
- Reject duplicate/non-positive management-event IDs before mutation; no repair/renumbering.
- Independent review required for shared load path.

## T-07 — Broad persisted cycle state

- **Status:** **CANCELLED.**
- D-066 does not reopen it. Any proposal for per-cycle teaching persistence, broad cycle/readiness state or repeated teaching-history records returns to Manager.

## T-08 — Multi-cycle integrated witness

- **Status:** BLOCKED on required accepted/integrated S1-D tasks, including T-04P/T-04.
- Tier-2 Unity witness: ≥3 real cycles, idle years, stale pending order, save/load at each stage, completion, continued management, D-057 solvent witness and protected anchor/hash comparison with no rebaseline.

## T-09 — Independent S1-D conformance review

- **Status:** BLOCKED on T-08.
- Review finished implementation against `checklists/requirements.md`; verdict APPROVED / NEEDS CHANGES / REJECTED.

## Required D-066 sequence

1. #145 browser repair publishes/hands off; no production release.
2. **#158** independently reviews repaired authority.
3. Only if #158 = **APPROVED** may Manager repin/release #143 after fresh live schema/ownership check.
4. #143 implements/qualifies/independent-reviews; Manager then reviews/integrates exact candidate.
5. Manager repins/releases #124 against exact #143 integration; #124 owns curriculum only.
6. #124 qualifies/independent-reviews/Manager reviews.
7. #126 PARITY-08 remains blocked until exact post-#143/#124 lineage is known and repinned.
8. #128 remains review-only behind #126.
9. T-08/T-09 follow the complete accepted S1-D lineage.

No concurrent save/schema/hash writers. No Windows build.