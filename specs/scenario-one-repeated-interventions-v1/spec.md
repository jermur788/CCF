# Scenario One Second and Subsequent Interventions (S1-D) Specification

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Status:** **APPROVED PRODUCT INTENT; D-066 IMPLEMENTATION CONTRACT REPAIRED BY #145 AND PENDING MANAGER REVIEW #158. NO T-04P/T-04 PRODUCTION RELEASE UNTIL #158 APPROVES.**
- **Owner:** Overall Manager (ChatGPT Game Dev)
- **Created:** 2026-10-27
- **Last updated:** 2026-10-29 — browser repair #145
- **Decision authority:** D-004, D-008, D-009, D-010 as amended, D-013, D-014, D-042, D-056, D-060, D-063, D-064, D-065, **D-066**; resolved decisions RD-1…RD-12.
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness; S1-D is pre-demo.
- **Repair evidence:** browser Manager review #144 and browser repair #145. Exact static code evidence base: `c8cfae8c4025cec6e960a050edf4bfde2a981de5`.
- **Task class:** SPEC REQUIRED. This document authorises no production work while the repaired D-066 contract is pending #158.

## 1. Product intent

The player must be able to repeat:

`assess → decide/mark/plan → Work Plan → execute → advance time → observe/review → decide again`

in the same Scenario One forest, without resetting the scenario or turning the first intervention into a permanent tutorial gate. Later management remains player-directed and advisory. Completion rules, objectives, economy/ecology and the protected Scenario One reference remain unchanged.

D-066 preserves full FR-B1 / AC-3 by adding the **smallest durable fact needed to remember that the existing first guided rack teaching sample/review has genuinely completed**. It does not authorise a repeated cycle-history store, current readiness cache or individual `[J]/[K]` persistence.

## 2. Inspection basis and evidence discipline

Historical implementation findings remain evidence, not acceptance. Browser repair #145 statically inspected the exact qualified post-MFM-07 lineage `c8cfae8c4025cec6e960a050edf4bfde2a981de5`:

- `ForestSaveData.CurrentVersion = 23` on that lineage; v24 is only an expected next slot if still free at a later released task start.
- `ScenarioOneTeachingSequence.Current(Facts)` leaves `response` only after all earlier lesson guards pass and, in a rack world, `ReleaseReviewed && RackReviewComplete` is true.
- `LearningObjectivesView.Sequence.TeachingFacts()` computes `RackReviewComplete = area && rackReview.Complete(bay)` from current-session rack review state; inspection/decision methods only feed that session review while the teaching sequence is at `response`.
- `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` proves only current-world executed release/review conditions and deliberately omits session `[J]/[K]` review completion; it is not sufficient to write the D-066 latch.
- `ScenarioReferenceArchive.HashLayout(...)` has a non-default v23 prescription-state branch that retains current JSON except fields explicitly stripped from identity. A new teaching-only field would therefore alter world identity unless deliberately excluded on every hash path.

Static inspection is not Unity verification. Production truth remains the exact later inspected/qualified candidate.

## 3. First-cycle assumption register

| ID | Finding | Required treatment |
|---|---|---|
| F-1 | Teaching-footprint cap could become a lifetime Approve/Advance block. | RD-5 / FR-A3: hard only while the owning teaching activity is active; advisory afterwards. |
| F-2 | Guide can regress when current Crop Tree facts change. | D-066 / FR-B1: durable historical floor after genuine completion. |
| F-3 | Later in-footprint felling can reopen the release/review lesson from mutable facts. | D-066 / FR-B1. |
| F-4 | `[J]/[K]` rack-review decisions are session-only; load can return to `response`. | Persist one completion latch, not the choices/transcript. |
| F-5 | First Annual Review acknowledgement gates play; later acknowledgements should not. | RD-3. |
| F-6 | Readiness existed mainly as teaching text. | RD-4: advisory derived components/reasons. |
| F-7 | Marks lack designation time; orders/events have chronology. | RD-6: derive prior/proposed presentation; no designation-year field. |
| F-8 | Practice steps may use device-wide `PlayerPrefs`. | Never progression authority after durable completion. |
| F-9 | Completion chronology relies on valid management-event IDs. | RD-12: reject duplicate/non-positive IDs before mutation. |
| F-10 | Economy already distinguishes previous thinning history. | Unchanged. |
| F-11 | `Completed` does not stop management before existing horizon; `Failed` is terminal. | RD-8. |
| F-12 | Rack V2 persistence and Objective 1 teaching are separate concerns. | RD-9. |

## 4. Definitions

- **Management cycle:** one pass of the repeat loop; it may legitimately contain no intervention.
- **Intervention:** a resolved work plan / S1-C intervention grouping; order-level records are drill-down.
- **Previous work:** executed/resolved work with persisted chronology/outcome.
- **Current commitment:** approved orders awaiting resolution.
- **Proposed work:** pending orders and Fell marks not yet committed. Crop Tree designations are standing designations, not work proposals.
- **Re-entry:** beginning any later cycle in the same save, including after save/load and idle years.
- **Readiness:** advisory derived components/reasons, never a gate or timetable.
- **D-066 historical teaching-completion latch:** one monotone boolean-like Scenario One save fact whose sole meaning is: **the existing first guided rack teaching sample/review has historically completed at least once.**

The latch has no cycle key, counter, list or repeated record. It is not intervention history, a guide-index cache, readiness, biological/world state or a forestry prescription.

## 5. Goals

1. Re-enter and repeat management cycles in one forest until the existing horizon/terminal rules end management.
2. Distinguish previous work from current commitments/proposals using S1-C and existing order/event records.
3. Keep completion reachable and understandable across repeated cycles.
4. Keep current recommendation/readiness derived; persist only the D-066 first-guided-sample/review completion latch.
5. Keep later management player-directed and advisory.
6. Save/load must not erase a legitimately recorded first teaching completion or fabricate one retrospectively.

## 6. Non-goals

- New ecology/economy/objective/calibration coefficients or altered completion policy.
- A fixed intervention interval, compulsory second treatment or per-cycle prescriptive objective.
- Broad persisted management-cycle/readiness state; T-07 remains cancelled.
- Per-cycle teaching keys, counters, sample histories or repeated review records.
- Persisting individual `[J]/[K]`, inspected-tree/stand-portion detail, session transcript, `PlayerPrefs`, guide index or Crop Tree designation year.
- A dated past-work map layer or duplicate S1-C history store/UI.
- Rack reopening/additional-rack authoring, a second MFM prescription, Scenario Two or sandbox work.
- Production C#/Unity implementation in this browser repair.

## 7. Player workflow

1. **Assess:** walk, inspect, map layers, review history.
2. **Decide/mark/plan:** use ordinary management actions without later-cycle lesson locking.
3. **Work Plan:** distinguish proposed/approved work; teaching safeguards are scoped to their activity.
4. **Execute/advance:** approved work resolves; idle advance remains legitimate.
5. **Observe/review:** Annual Review/history/walk changed forest; later acknowledgement is advisory.
6. **Decide again:** guide never regresses behind a genuinely completed first teaching review.

The next cycle is available after advance while the scenario remains manageable under existing rules. No recurring acknowledgement or minimum observation interval is introduced.

## 8. Interface to S1-C

S1-D consumes S1-C's read-only history capability and stable records. It must not create a competing intervention/history authority.

The D-066 latch is allowed only because the session rack-review completion boundary is not faithfully reconstructible from existing persistent history. It stores **only that missing historical teaching-completion fact** and no intervention detail.

A read-only cycle-state adapter may derive advisory components from existing records. It remains non-persistent and distinct from the D-066 latch.

## 9. Functional requirements

### A. Re-entry and action availability

- **FR-A1** Further cycles MUST remain possible in the same save without resetting forest/scenario or losing prior records.
- **FR-A2** First-cycle management actions MUST remain available in later manageable years and MUST NOT depend on lesson, `PlayerPrefs` or session-only state. Rack reopening remains Rack V2 scope.
- **FR-A3** The teaching-footprint safeguard MUST be hard only while the first guided sample activity is still open. After the D-066 historical latch is true it MAY remain advisory but MUST NOT disable Approve Work Plan or Advance. MFM-owned ceilings are unchanged.

### B. Progression state

- **FR-B1** Guide/active teaching state MUST be monotone after the first guided rack sample/review has genuinely completed. The single D-066 latch is the durable authority for that historical boundary.
- **FR-B1a — exact write boundary.** The latch MUST change `false → true` exactly once only when the **unlatched/pre-amendment rack-world sequence would genuinely transition from `response` to `recruitment` in the current session**. All earlier lesson guards must already pass, `ReleaseReviewed` MUST be true, and actual session `RackReviewComplete` MUST be true. A helper/refactor is allowed only if its behavior is proven equivalent to this pre-amendment transition.
- **FR-B1b — no surrogate.** `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` alone, planting, arbitrary/in-footprint felling, later review acknowledgement, Crop Tree count/loss, current geometry, executed-share drift, `PlayerPrefs`, save age/version or heuristic reconstruction MUST NOT set the latch.
- **FR-B1c — two-phase shared semantics.** Before latch, T-03 keeps accepted current-world/session cap semantics and T-04 keeps existing pre-completion guide behavior. After latch, T-03 and T-04 MUST consume one shared read meaning: the first guided sample/review is historically complete. Later world drift/load cannot unset it.
- **FR-B1d — no write-on-read/load.** Read/display/rebuild/load paths MUST NOT infer or write completion. The authoritative current-session transition is the only false→true write event.
- **FR-B2** Stable lesson/objective/order/event IDs MUST NOT be renamed/reused. Existing teaching IDs remain `access`, `future-trees`, `release`, `habitat`, `response`, `recruitment`, `establishment`, `transformation`.
- **FR-B3** Individual `[J]/[K]`, inspected-tree/stand-portion detail and `PlayerPrefs` MUST NOT be persisted or used as post-latch progression authority.

### C. Previous work versus current proposals

- **FR-C1** Presentation MUST distinguish Executed, Approved, Pending, Pending carried over and Fell marks not yet in Work Plan.
- **FR-C2** Carried-over orders MUST be revalidated and surfaced if stale; never auto-cancelled/approved.
- **FR-C3** Executed work MUST NOT appear as current proposal; failures keep their reason.
- **FR-C4** UI MUST NOT invent a designation time where none is persisted.
- **FR-C5** No Crop Tree designation-year field or dated past-work map layer in S1-D V1.

### D. Advisory cycle state

- **FR-D1** Derived cycle state MUST report named components/reasons from authoritative records.
- **FR-D2** Readiness MUST NEVER be a gate/timetable; waiting remains legitimate.
- **FR-D3** After the first advance, outstanding Annual Review acknowledgement is advisory and MUST NOT block Approve/Advance.
- **FR-D4** Later interventions remain ordinary player-directed management; no per-cycle objective or single correct treatment.

### E. Completion and validation

- **FR-E1** Completion MUST remain reachable after repeated cycles; after D-066 completion no transient session review may be required to preserve that historical teaching boundary.
- **FR-E2** Protected behavior in §10 MUST NOT change.
- **FR-E3** Duplicate/non-positive management-event IDs MUST reject before world/disk mutation with no repair. The one-field D-066 latch does not authorise a new key/counter/list validation model; existing schema/version and pre-mutation validation guarantees remain binding.
- **FR-E4** Existing minimum of two reviewed successful intervention cycles in distinct years is unchanged; management may continue after `Completed` until existing terminal/horizon conditions.

### F. Save and history re-entry

- **FR-F1** Save/load MUST reproduce authoritative history/advisory state and the D-066 latch. Reads MUST NOT write. Latch writes only at FR-B1a.
- **FR-F2** S1-D V1 MAY add exactly one boolean-like D-066 teaching-completion field plus minimum version/capture/restore/hash compatibility plumbing. No per-cycle key/list/counters or broad cycle blob.
- **FR-F2a — schema.** Use the next free version on the actual Manager-released implementation base. v24 is expected only if live inspection still shows v23 current/free. No version is reserved by #145/#158.
- **FR-F2b — old saves.** Field absent/default false means historical completion **not recorded**. Do not backfill from existing world/history/session facts.
- **FR-F3** Reference Future v1, Legacy40/pre-v20 compatibility, objective-achievement history and MFM-06/07 prescription/provenance round-trip MUST remain unchanged.
- **FR-F4 — world identity exclusion.** The teaching latch MUST round-trip in saves but MUST NEVER participate in world identity. Every `ScenarioReferenceArchive` identity/hash projection MUST strip/ignore it, including `WorldHash`, `CurrentWorldHash`, `LegacyCompatibleWorldHash`, every `HashLayout` branch (including non-default v23 prescription state), any `MinimalWorldHash`/protected projection, and protected hash/reference comparisons that consume those layouts. No accepted anchor/fingerprint/reference may be rebaselined.

### G. Neighbouring work

- **FR-G1** MFM-generated work remains ordinary work; no second prescription.
- **FR-G2** Objective 1 rack teaching does not automatically repeat; rack reopening is Rack V2 scope.
- **FR-G3** No Sitka-only assumption or new species coefficient.
- **FR-G4** Consume S1-C history; no competing store/identity/UI. The D-066 latch is teaching progression only.

### H. Compatibility

- **FR-H1** Geometry-independent derived presentation/re-entry may work for Legacy40 where records exist; Enlarged80 rack/sample teaching is not migrated into Legacy40. Reference Future v1 remains frozen.
- **FR-H2** Pre-amendment saves MUST load with no fabricated teaching completion/history/provenance.

## 10. Protected behavior

D-066 MUST NOT change:

- `ScenarioOneCompletionRules` normalised source or pinned N3 hash;
- eight objective predicates/targets or D-057 effective targets;
- economy/ecology/work-economy/thinning coefficients;
- Rack V2 lifecycle/geometry;
- MFM-06/07 prescription/provenance semantics;
- frozen Reference Future v1 content;
- accepted Legacy40/Enlarged80 world anchors/fingerprints;
- no-Windows-build-before-user-playtest rule;
- no fixed intervention interval or automatic treatment choice.

The D-066 latch is **save progression state, not world identity**. Excluding it from world hashes preserves existing identity semantics; it does not create a new hash policy.

## 11. Authority / source of truth

Accepted product intent remains Decision Log through D-066, Current Milestone and Game Brief. The repaired implementation contract in this spec is **proposed by #145 and not released until Manager review #158 returns APPROVED**. Implementation truth remains inspected repository/Unity evidence.

## 12. Resolved decisions

- **RD-1:** historical implementations are partial evidence, not feature completion.
- **RD-2:** later cycles are bounded hybrid; player-directed with advisory guidance.
- **RD-3:** later Annual Review acknowledgement advisory, not a recurring hard gate.
- **RD-4:** readiness is advisory derived components/reasons, no gate/timetable.
- **RD-5:** teaching-footprint cap hard only while owning activity is active; advisory afterwards.
- **RD-6:** previous/new provenance derives from existing records; no designation-year field/detailed past-work map.
- **RD-7 narrowed by D-066:** current/recomputable facts remain derived; individual `[J]/[K]` and `PlayerPrefs` are not persistent authority; only the single historical completion latch is newly persisted.
- **RD-8:** two reviewed successful cycles minimum unchanged; management may continue after `Completed`.
- **RD-9:** rack reopening/additional racks remain Rack V2; Objective 1 teaching does not automatically repeat.
- **RD-10:** geometry-independent features may work for Legacy40 where evidence exists; no Enlarged80 teaching migration; Reference frozen.
- **RD-11:** no second MFM prescription in S1-D V1.
- **RD-12:** ambiguous management-event IDs reject before mutation; no repair.
- **D-066 repaired implementation meaning:** one monotone first-guided-sample/review completion latch, written only at FR-B1a; no per-cycle history/counters; old/missing false; all world/hash identities exclude it.

## 13. Evidence classification

- Accepted authority: D-060, D-066, RD-1…RD-12.
- Direct code evidence for the repair: exact Git inspection at `c8cfae8...`; no Unity claim.
- Model derivation: advisory/current-state comparisons.
- Gameplay calibration: existing teaching release/cap values and two-cycle minimum; unchanged.
- Product/technical design: the one-way latch, old-save default and world-identity exclusion.

## 14. Save / backward compatibility

1. Inspected base `c8cfae8...` is v23; v24 is expected only if still free when #143 is later released.
2. Add one boolean-like Scenario One teaching-completion field only.
3. Missing/false on older saves = not historically complete; no retrospective reconstruction.
4. True can never return false.
5. `[J]/[K]`, inspected-tree/stand-portion detail, session transcript, cycle keys/counters and current guide/readiness state remain non-persistent.
6. Full save serialization preserves the latch.
7. Every world/hash identity layout strips/ignores the latch, including non-default prescription-state paths; no rebaseline.
8. Preserve MFM-06/07, objective-achievement history, Rack V2, Legacy40/pre-v20 and frozen Reference Future behavior.

## 15. Security / write-back

Single-player local save; load validation remains an integrity boundary. No read/display/load inference may mutate the latch. Existing rejection paths must remain pre-mutation and consistent between CanLoad/LoadData where applicable.

## 16. Acceptance criteria

- **AC-1** Three real cycles with idle years preserve ordinary management availability and records.
- **AC-2** Before historical completion, existing teaching-cap safeguards remain; after latch true, exceeded historical footprint is advisory/non-blocking.
- **AC-3** The latch is written exactly once only at the genuine unlatched `response → recruitment` boundary including `ReleaseReviewed` + actual session `RackReviewComplete`; after true, guide does not regress behind that boundary due to Crop Tree count/loss, later in-footprint felling, executed-share drift or save/load; before true behavior is unchanged; repeated reads/calls do not write.
- **AC-4** Loading the same valid save twice yields identical history, advisory cycle state, latch and text; reads are non-mutating.
- **AC-5** Five work classes are distinct; no unrecorded designation date or dated map layer.
- **AC-6** ≥3-cycle path remains completable/solvent under existing rules; objectives/targets/N3 hash unchanged.
- **AC-7** Duplicate/non-positive event IDs reject before mutation, no repair.
- **AC-8** Save diff is limited to the D-066 one-field latch + required version/capture/restore/hash compatibility plumbing. Otherwise identical false-vs-true latch worlds have identical world/reference/protected hashes on **all** identity paths, including non-default prescription state. No rebaseline. MFM-06/07 round-trip remains green.
- **AC-9** Readiness remains advisory, never a gate/timetable.
- **AC-10** Independent review finds no hidden automation/causal overclaim.
- **AC-11** One intervention/history authority; D-066 is not a second history store.
- **AC-12** Objective 1 rack teaching does not automatically retrigger; no rack-reopening behavior added.
- **AC-13** Legacy/pre-amendment compatibility and Reference Future remain valid.
- **AC-14** Management continues after `Completed` to existing horizon/terminal conditions.
- **AC-15** MFM orders remain ordinary orders; no second prescription.
- **AC-16** Pre-amendment supported saves load with latch false/not recorded and no fabricated completion.
- **AC-17** T-03 and T-04 consume one shared post-latch historical meaning while T-03 pre-latch current-world/session classification remains unchanged.

## 17. Stop conditions

Return to Manager rather than assume if:

- #158 does not approve the repaired contract;
- live save/schema/hash ownership or version changes before #143 start;
- one boolean latch is insufficient and a materially broader persisted model appears necessary;
- faithful completion would require persisting individual `[J]/[K]`/session detail;
- exact `RackReviewComplete` current-session boundary cannot be preserved;
- the teaching field cannot be excluded from all world identity paths without changing accepted identity semantics;
- an accepted old save cannot default false under existing compatibility policy;
- any fix would alter protected completion/objectives/economy/ecology/Rack/MFM/Reference behavior or require a hash/reference rebaseline;
- repeated cycles make completion unreachable without coefficient change.

## 18. D-066 repaired execution order — 2026-10-29

1. #145 publishes this repaired contract and its browser result; no production release.
2. **#158** independently reviews the repair and returns APPROVED / NEEDS CHANGES / REJECTED.
3. Only if APPROVED may Manager repin/release **#143 / T-04P** after fresh live schema/ownership inspection.
4. #143 implements/qualifies/independently reviews and integrates the one-way latch + all-hash exclusion as the sole save/schema/hash writer.
5. Manager repins/releases **#124 / T-04** against that exact integration; #124 owns curriculum/read-only consumption only.
6. #124 completes qualification/review.
7. **#126 PARITY-08** remains blocked until the exact post-#143/#124 lineage is known and repinned.
8. **#128** independent PARITY review follows #126.

No Windows build is authorised by D-066 or this repair.