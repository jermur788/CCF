# Scenario One Second and Subsequent Interventions (S1-D) Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Status:** repaired D-066 contract pending independent Manager review #158; implementation not released.
- **Checklist owner:** Overall Manager / later independent T-09 reviewer
- **Last updated:** 2026-10-29 — browser repair #145

A checked implementation item requires exact evidence. Browser #145 provides specification/static-code evidence only; it does not prove C#/Unity implementation.

## Preconditions / authority

- [x] RD-1…RD-12 and accepted D-066 Option A exist as product authority.
- [x] Exact static evidence base inspected: `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, save v23.
- [x] Pre-amendment sequence evidence confirms rack-world `response → recruitment` requires earlier lesson guards + `ReleaseReviewed` + actual session `RackReviewComplete`.
- [x] T-03 `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` is confirmed insufficient by itself to prove session rack review.
- [x] `ScenarioReferenceArchive.HashLayout` non-default prescription-state branch requires explicit exclusion of any new teaching-only field.
- [ ] Manager review #158 = APPROVED — evidence:
- [ ] #143 released after fresh live schema/ownership refresh — evidence:
- [ ] #143 exact implementation integrated/accepted before #124 release — evidence:

## D-066 repaired contract

- [ ] Exactly **one boolean-like monotone historical latch** is added; no per-cycle key/list/counter/history structure — evidence:
- [ ] Latch meaning is exactly “first guided rack teaching sample/review historically completed at least once” — evidence:
- [ ] False→true write occurs exactly once only when the **unlatched/pre-amendment** rack-world sequence genuinely crosses `response → recruitment` — evidence:
- [ ] All earlier lesson guards are satisfied at the write, including `ReleaseReviewed` and actual session `RackReviewComplete` — evidence:
- [ ] `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` alone cannot write the latch — evidence:
- [ ] Planting, arbitrary/in-footprint felling, later review, Crop Tree count/loss, geometry, executed-share drift, `PlayerPrefs`, save version/age and heuristic reconstruction cannot write it — evidence:
- [ ] Load/read/display/rebuild paths do not infer/write the latch — evidence:
- [ ] Once true, latch never returns false — evidence:
- [ ] Individual `[J]/[K]`, inspected-tree/stand-portion detail and session transcript are not persisted — evidence:
- [ ] No guide-index/readiness/advisory cache or broad `ScenarioOneManagementCycleState` is added — evidence:

## T-03 / T-04 shared semantics

- [ ] Before latch, T-03 retains accepted current-world/session cap classification and deadlock safeguard — evidence:
- [ ] Before latch, T-04 preserves existing pre-completion teaching behavior — evidence:
- [ ] After latch, T-03 and T-04 consume exactly one shared historical completion read meaning — evidence:
- [ ] Later Crop Tree count/loss, in-footprint felling, executed-share drift and save/load cannot unset historical completion — evidence:
- [ ] No second durable T-03/T-04 completion interpretation exists — evidence:

## Functional requirements

### A. Re-entry / actions
- [ ] FR-A1: later cycles work in same save without reset/lost records — evidence:
- [ ] FR-A2: first-cycle management actions remain available later without lesson/PlayerPrefs/session dependence — evidence:
- [ ] FR-A3: teaching footprint hard only while first guided activity open; after latch true it is advisory/non-blocking; MFM ceilings unchanged — evidence:

### B. Progression
- [ ] FR-B1/AC-3: exact authoritative latch transition and post-latch monotonic guide behavior pass — evidence:
- [ ] FR-B2: stable teaching IDs unchanged: `access`, `future-trees`, `release`, `habitat`, `response`, `recruitment`, `establishment`, `transformation` — evidence:
- [ ] FR-B3: no individual J/K or PlayerPrefs durable authority — evidence:

### C. Previous/proposed work
- [ ] FR-C1: Executed / Approved / Pending / Pending carried over / Fell mark distinct — evidence:
- [ ] FR-C2: stale carried-over work revalidated/surfaced, never auto-cancelled/approved — evidence:
- [ ] FR-C3: executed work not shown as current proposal; failures retain reason — evidence:
- [ ] FR-C4/C5: no invented designation time/year and no dated past-work map layer — evidence:

### D. Advisory state
- [ ] FR-D1: derived components/reasons from authoritative records — evidence:
- [ ] FR-D2: no readiness gate/timetable/fixed interval — evidence:
- [ ] FR-D3: later review acknowledgement advisory, not Approve/Advance gate — evidence:
- [ ] FR-D4: later management player-directed; no per-cycle prescriptive objective — evidence:

### E. Completion / integrity
- [ ] FR-E1: completion remains reachable; no transient rack-review session state required after latch — evidence:
- [ ] FR-E2: completion/objectives/economy/ecology protected behavior unchanged — evidence:
- [ ] FR-E3: duplicate/non-positive management-event IDs reject pre-mutation without repair — evidence:
- [ ] FR-E4: two reviewed successful cycles minimum unchanged; continued management after Completed — evidence:

### F. Save / backward compatibility
- [ ] FR-F1: save/load preserves latch and other derived/history state; reads are non-mutating — evidence:
- [ ] FR-F2: save diff contains only one latch + minimum version/capture/restore/hash compatibility plumbing — evidence:
- [ ] Live start used next free schema version; v24 only if v23 remained current/free — evidence:
- [ ] Old/missing field loads as false/not-recorded with no backfill — evidence:
- [ ] False and true latch states round-trip deterministically — evidence:
- [ ] MFM-06/07 prescription/provenance, objective-achievement history, Rack V2 and supported old-save defaults remain intact — evidence:

### World identity / hashes
- [ ] Latch round-trips in full saves but is excluded from `WorldHash` — evidence:
- [ ] excluded from `CurrentWorldHash` — evidence:
- [ ] excluded from `LegacyCompatibleWorldHash` — evidence:
- [ ] excluded from **every `HashLayout` branch**, including non-default v23 prescription state — evidence:
- [ ] excluded from any `MinimalWorldHash`/equivalent protected-minimal identity projection — evidence:
- [ ] protected reference/hash extraction/comparison sees no false-vs-true latch difference — evidence:
- [ ] otherwise-identical latch=false/latch=true worlds produce identical accepted identity hashes, including a real prescription-state case — evidence:
- [ ] no reference archive, accepted anchor or protected fingerprint rebaselined — evidence:

### G/H neighbouring compatibility
- [ ] MFM orders remain ordinary; no second prescription — evidence:
- [ ] Objective 1 rack workflow does not automatically repeat; no rack reopening added — evidence:
- [ ] no new species coefficient/Sitka-only assumption — evidence:
- [ ] S1-C remains sole intervention/history authority — evidence:
- [ ] Legacy40/pre-v20/Reference Future compatibility preserved — evidence:
- [ ] pre-amendment Enlarged80 saves load without fabricated teaching completion — evidence:

## T-04P verification matrix

- [ ] response reached, `ReleaseReviewed=false` → latch false.
- [ ] `ReleaseReviewed=true`, `RackReviewComplete=false` → latch false.
- [ ] actual session rack review completes and unlatched sequence reaches recruitment → exactly one false→true write.
- [ ] repeated update/view/read in same state → no second write.
- [ ] save/load true → remains true.
- [ ] later Crop Tree count/loss/felling/share drift → remains true.
- [ ] representative old saves → false/default, no heuristic backfill.
- [ ] T-03 pre-latch decision matrix unchanged.
- [ ] T-03 and T-04 post-latch shared read returns same true value.
- [ ] N3 normalized completion source/hash unchanged.
- [ ] MFM save/provenance regression green.
- [ ] all identity/hash invariance tests green with no rebaseline.
- [ ] Unity compile/runtime witness green on exact candidate.
- [ ] independent #143 implementation review APPROVED.

## T-04 verification matrix

- [ ] pre-latch response behavior unchanged.
- [ ] review acknowledgement without RackReviewComplete does not skip response.
- [ ] after latch, guide cannot regress behind completed response boundary after Crop Tree count/loss.
- [ ] after latch, later in-footprint felling/executed-share drift does not regress it.
- [ ] after latch, save/load at later lessons preserves guide floor.
- [ ] all eight stable lesson IDs unchanged.
- [ ] TeachingCopy/MenuTutorial/T-03/T-02/S1-C/N3 regressions green.
- [ ] Unity qualification green on exact replayed candidate.
- [ ] independent T-04 review APPROVED.

## Integrated acceptance criteria

- [ ] AC-1: ≥3 real cycles with idle years and ordinary management actions.
- [ ] AC-2: teaching cap scoped correctly before/after latch.
- [ ] AC-3: authoritative one-way latch + guide monotonicity under world drift/save-load.
- [ ] AC-4: same valid save loaded twice yields identical history/advisory/latch/text; reads mutate nothing.
- [ ] AC-5: five work classes visibly distinct; no invented provenance/date.
- [ ] AC-6: ≥3-cycle completion/solvency witness, objectives/targets/N3 unchanged.
- [ ] AC-7: event-ID integrity rejection before mutation.
- [ ] AC-8: protected anchors/reference/hash behavior unchanged; no rebaseline; MFM save/provenance green.
- [ ] AC-9: readiness advisory only.
- [ ] AC-10: independent review finds no hidden automation/causal overclaim.
- [ ] AC-11: one intervention/history authority; latch is not history store.
- [ ] AC-12: rack teaching not automatically repeated.
- [ ] AC-13: Legacy/pre-amendment compatibility preserved.
- [ ] AC-14: management continues after Completed under existing rules.
- [ ] AC-15: MFM ordinary-order behavior unchanged.
- [ ] AC-16: old saves default latch false with no fabrication.
- [ ] AC-17: one shared T-03/T-04 post-latch historical meaning and unchanged T-03 pre-latch semantics.

## Sequencing / ownership

- [x] #143 remains blocked pending #158.
- [x] #124 remains blocked pending #158 then accepted/integrated #143.
- [x] #126 remains blocked until exact post-#143/#124 lineage.
- [x] #128 remains review-only behind #126.
- [ ] #158 APPROVED and Manager explicitly released #143 — evidence:
- [ ] one save/schema/hash writer only during #143 — evidence:
- [ ] #124 owned no save/schema/hash code — evidence:

## Verdict

- **Spec conformance:** NOT VERIFIED
- **Implementation correctness:** NOT VERIFIED
- **Browser repair state:** ready for independent Manager review #158; no production release.
