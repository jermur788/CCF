# S1-D T-04 — Manager Review

**Task:** S1-D T-04 — Guide monotonicity  
**Claim:** #124  
**Current verdict:** **BLOCKED PENDING REPAIRED D-066 MANAGER REVIEW #158**  
**Prior verdict:** browser review #144 = NEEDS CHANGES to amendment #136  
**Review basis:** approved S1-D product intent including D-066; pushed candidate `b9d204cbae687da71d32a9da42575ca3f298a511` as unqualified evidence only; exact inspected teaching/hash lineage `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; browser repair #145.

## Prior candidate finding retained

`b9d204c` corrected rejected surrogate proofs but cannot satisfy full FR-B1 / AC-3 by derivation alone. Reload resets session-only rack review and later world drift can expose an earlier lesson. It remains useful evidence only: not accepted, not Unity-qualified and not an integration base.

## D-066 repair — binding proposed implementation contract

The user-selected Option A remains unchanged: preserve full FR-B1 / AC-3 with the **smallest durable teaching-completion fact**. Browser repair #145 narrows the implementation contract to exactly one monotone historical latch for the existing first guided rack sample/review.

The latch means only:

> **The Scenario One first guided teaching sample/review has historically completed at least once.**

It does **not** identify a management cycle, store counters, remember individual `[J]/[K]` choices, replay inspected trees/stand portions, cache a guide index, cache readiness, or create a second intervention/history store.

Old/pre-amendment saves with the field absent read exactly like `false`: historical teaching completion is not recorded. It is never reconstructed retrospectively from planting, felling, Annual Review records, Crop Tree count, current rack geometry, `PlayerPrefs`, save age or another heuristic.

## Exact authoritative write transition

The latch may change `false → true` exactly once, and only when the **pre-amendment rack-world guide would genuinely advance from lesson `response` to lesson `recruitment` in the current session**.

Static inspection of the qualified lineage establishes the required semantics:

1. all earlier `ScenarioOneTeachingSequence.Current(Facts)` guards have already passed: access is open/reviewed, the teaching rack has enough distributed future trees, canopy is kept, the release has been planned/executed and mixed deadwood/sold outcomes satisfy the existing lesson path;
2. the release Annual Review is acknowledged (`ReleaseReviewed == true`);
3. the current rack-world session review itself is complete (`RackReviewComplete == true`), as computed from the live `ScenarioOneRackReview` session state;
4. evaluating the **unlatched/pre-amendment** sequence would therefore return `recruitment` rather than `response`.

`ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` alone is **not** a write condition because it deliberately omits session `[J]/[K]` review completion. A later lesson, planting event, later review, or other surrogate is also insufficient.

Implementation may extract a pure helper for this transition, but verification must prove that helper matches the pre-amendment `response → recruitment` boundary before the historical latch is applied.

## One shared T-03 / T-04 read meaning

There are two phases and one historical fact:

- **Before latch:** the historical completion API returns false. T-03 retains its accepted current-world/session teaching-cap classification and deadlock safeguard; T-04 retains pre-completion guide behaviour. No durable state fabricates session review completion.
- **At transition:** the authoritative current-session `response → recruitment` condition above records the latch once.
- **After latch:** both T-03 and T-04 consume the same read-only historical-completion API/property. It remains true through save/load, Crop Tree loss/count drift, later in-footprint felling and executed-share drift. T-03 therefore treats the first teaching sample as historically closed; T-04 may use that same fact as its monotone floor.

There must not be a second T-03 historical helper with a different durable meaning. Existing current-world calculations remain available for pre-latch teaching safeguards; the shared latch is the sole persisted historical truth.

## Save/schema and hash identity contract

#143 remains the only prospective save/schema/hash writer for this slice. At the inspected production umbrella, `ForestSaveData.CurrentVersion = 23`; v24 is only the **expected** next slot if a fresh task-start refresh still finds v23 current/free and no competing writer. #145/#158 do not reserve it.

The persisted representation is one boolean-like Scenario One save field/latch. Exact C# field/helper names are implementation details, but the representation may not broaden into per-cycle keys, counters, lists or replay records without a new Manager decision.

The teaching latch is **save state but not world identity**. It must round-trip in saves while being excluded from every `ScenarioReferenceArchive` identity/hash projection, including:

- `WorldHash` / geometry-selected full identity;
- `CurrentWorldHash`;
- `LegacyCompatibleWorldHash`;
- `HashLayout` on both default-later-field and non-default v23 prescription-state branches;
- `MinimalWorldHash` or any equivalent minimal/protected projection used by verification;
- protected hash extraction/comparison and Reference Future/anchor checks that consume those projections.

The implementation must use the smallest compatibility edit that removes only this teaching-only field from identity inputs. **No accepted anchor, protected fingerprint or reference hash may be rebaselined.** Full save serialization must still preserve the latch.

A single boolean latch introduces no per-record key/counter validation problem. Existing schema/version and pre-mutation validation guarantees remain binding; tests must prove old/missing-field false behavior, true round-trip, one-way write semantics and no world mutation on rejected malformed saves generally. Do not invent a complex validation model solely to justify persistence.

## #124 continuation contract

#124 remains blocked until #158 returns APPROVED and #143 is then implemented, independently reviewed, qualified, Manager-approved and integrated on one exact lineage. When re-released, #124 must:

1. rebase/replay useful logic from `b9d204c` against the exact integrated #143 lineage rather than stacking it blindly;
2. consume the single shared historical-completion read API;
3. preserve pre-latch lesson behaviour and all stable lesson IDs;
4. prove full AC-3 across save/load and later world drift;
5. own no save/schema/hash files;
6. rerun T-03 shared-semantics checks plus TeachingCopy/MenuTutorial/T-02/S1-C/N3 with no rebaseline;
7. complete Unity qualification and independent review on the exact candidate.

## Sequencing / gate state

1. #145 browser repair publishes the repaired documents/result and hands off.
2. **#158 independently reviews the repaired contract: APPROVED / NEEDS CHANGES / REJECTED.**
3. Only an APPROVED #158 may cause Manager to repin/release #143 after a fresh live schema/ownership check.
4. #143 implements/qualifies/integrates the one-way latch and all-hash exclusion as the sole save/schema/hash writer.
5. #124 curriculum replay follows on the exact #143 integration and owns no persistence/hash surface.
6. #126 PARITY-08 remains blocked until that exact combined S1-D lineage is known and repinned.
7. #128 remains review-only behind #126.

Current states:

- `b9d204c`: unqualified evidence only.
- #143: **BLOCKED PENDING #158 APPROVAL; no ownership**.
- #124: **BLOCKED PENDING #158 then #143 integration; no active production release**.
- #126: **BLOCKED** on repaired/approved S1-D lineage.
- #128: **BLOCKED** behind future #126 handoff.
- No Windows build is authorised.

## Next Manager decision

Run browser Manager review #158 against the repaired #145 package. This file records the proposed corrected contract; it does not itself release production implementation.