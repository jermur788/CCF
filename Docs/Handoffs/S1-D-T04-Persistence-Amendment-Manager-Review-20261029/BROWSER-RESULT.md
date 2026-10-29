# Browser Manager Review Result — S1-D T-04 Persistence Amendment

## Review identity

- **Review claim:** #144
- **Reviewed browser amendment:** #136
- **Amendment result SHA reviewed:** `ae518ee9c1029eaabbc83c8369c20f02a6bf655a`
- **Qualified production lineage inspected for save/schema facts:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5`
- **Current save schema on that production lineage:** v23
- **Verdict:** **NEEDS CHANGES**
- **Review lane:** OpenAI GPT-5.6 Sol / browser-GitHub only
- **No C#/Unity, no Unity lock, no Windows build.**

## Executive finding

The **product decision D-066 is approved and remains canonical**. The #136 amendment correctly chose the user's Option A, removed the blanket no-persistence prohibition, preserved individual `[J]/[K]` as session-only, established conservative old-save defaults, blocked competing save writers and sequenced PARITY-08 behind the S1-D lineage.

However, the **implementation authority encoded by #136 is not yet safe to release**. Three material contract defects remain:

1. the amended spec mandates a broader per-cycle/key/counter history structure than the user-authorised minimum fact and than inspected evidence currently requires;
2. it does not pin the authoritative write transition tightly enough — the local packet still asks the code worker to discover whether/when the durable fact becomes true, while the inspected pre-amendment guide shows that the genuine rack-world transition requires the session `RackReviewComplete` condition that the existing T-03 helper does not include;
3. it does not explicitly state that this teaching-only persistence is excluded from **every** world/hash identity path. On the current v23 implementation, worlds with real prescription state bypass the later-field stripping path and retain all current JSON except objective-achievement history, so a new teaching field would move current world hashes unless deliberately stripped.

These are specification/packet defects, not reasons to weaken D-066. Local #143 must remain blocked until the browser repair fixes them.

## Review matrix

| Review question | Verdict | Finding |
|---|---|---|
| Product decision recorded in Decision Log | **PASS** | D-066 is canonical and accurately preserves full FR-B1/AC-3 while forbidding individual `[J]/[K]` persistence. |
| Old blanket “no persisted field” authority removed/narrowed | **PASS** | Spec, plan, tasks and checklist consistently recognise the narrow D-066 exception and keep broad T-07 persistence cancelled. |
| One **minimal** aggregate fact defined | **FAIL** | FR-B1/plan/T-04P currently require “for each applicable teaching/management cycle” a stable key, separate sample/review flags and aggregate counters/outcomes. No reviewed evidence establishes that this broader repeated-history structure is necessary. |
| Exact authoritative write transition defined | **FAIL** | D-066 says “authoritative pre-amendment teaching completion”, but #143 still delegates the exact representation/write hook to the local worker. Static code shows the rack-world guide advances from `response` only after real `RackReviewComplete`, whereas T-03's current `SampleHistoricallyComplete` helper does not prove that session review occurred. |
| No retrospective fabrication / honest old-save default | **PASS** | Missing field defaults to not-recorded/incomplete; heuristic reconstruction is prohibited. |
| Eight teaching IDs preserved; J/K non-persistent | **PASS** | #124 continuation packet preserves all existing IDs and explicitly forbids J/K/session transcript persistence. |
| v23 / next-schema sequencing safe | **PASS WITH HOLD** | Qualified production umbrella is v23; v24 is correctly treated as conditional on a fresh live check. **v24 is not released or reserved by this review.** |
| Reference Future / protected hashes preserved | **FAIL — contract incomplete** | “No rebaseline” is required, but the amendment does not explicitly define the new teaching-only field as outside world identity or require it to be stripped from all `ScenarioReferenceArchive` hash layouts, including non-default v23 prescription worlds. |
| Exactly one save/schema writer | **PASS** | #143 is the only contemplated save/schema lane; #124 owns curriculum only; #126/#128 remain blocked. |
| T-03/T-04 faithful shared historical completion input | **FAIL — semantics need repair** | Before durable completion, T-03 must retain its current-world/session cap semantics. The durable latch may be written only at the genuine guide response→recruitment transition, then both T-03/T-04 can consume the same historical true fact. The current amendment does not state this two-phase contract precisely enough. |
| MFM proposal/Work Plan semantics unchanged | **PASS** | No MFM production semantics are authorised to change. |

## Source finding — authoritative transition

On the accepted pre-amendment teaching code, `ScenarioOneTeachingSequence.Current(Facts)` leaves lesson `response` only when the earlier lesson conditions have passed and, in a rack-world, both `ReleaseReviewed` and `RackReviewComplete` are true. Therefore a player has not historically completed that teaching sample/review merely because the release work and Annual Review exist.

The current T-03 `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` helper instead tests the executed release/review geometry/outcome condition (`releaseReviewed`, release year, mixed sold/deadwood outcome and minimum executed share) and deliberately omits session `[J]/[K]` review completion. That helper is valid for T-03's existing in-session cap behaviour but is **not sufficient by itself** to write the D-066 historical latch.

The repaired authority must therefore pin the durable write event to the **pre-amendment guide's real response→recruitment transition in the current session**, including `RackReviewComplete`, rather than to the T-03 helper or any later surrogate.

## Source finding — hash identity

On qualified v23, `ScenarioReferenceArchive.HashLayout(...)` treats v22 `objectiveAchievements` as history and always excludes it from world identity. For a world with non-default v23 prescription state, the current path returns the full current JSON **minus only objective-achievement history**. A new teaching-completion field would therefore enter the world hash unless implementation explicitly strips it on that path as well as the older/default paths.

D-066 teaching progression is not biological/world identity. The repaired spec/packet must state that the teaching latch is **never part of world identity**, and #143 must make the smallest hash-layout compatibility edit necessary to strip it everywhere while leaving save round-trip intact. No accepted hash/reference may be rebaselined.

## Required amendment repair

A browser-only repair is prepared as **claim #145**:

`Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-HANDOFF.md`

The repair must preserve D-066 but change the implementation contract to:

1. **one monotone durable historical teaching-sample/review-complete latch for the existing first guided sample**, unless fresh exact-code inspection proves a broader persisted model strictly necessary; any such broader need returns to Manager;
2. **no per-cycle key, aggregate counter set or repeated history record by default**;
3. old/missing field = false/not-recorded; no retrospective reconstruction;
4. set the latch exactly once when the **pre-amendment rack-world guide would genuinely advance from `response` to `recruitment` in-session**, including the real `RackReviewComplete` condition and prior lesson requirements;
5. before the latch, T-03 keeps existing current-world/session safeguard semantics; after the latch, T-03 and T-04 consume the same durable historical true fact;
6. individual `[J]/[K]`, inspected-tree/stand-portion detail and session review transcript remain non-persistent;
7. the teaching latch is always stripped from world/hash identity, including worlds with non-default v23 prescription state; save serialization still round-trips it; no rebaseline;
8. #124 remains a separate curriculum lane with no save/schema/hash ownership.

The repaired #143 packet must specify these product/architecture choices before local implementation. It must not ask the local code worker to choose between a bool/latch and per-cycle historical model or to decide what constitutes authoritative completion.

## Exact save/schema sequence

**Current accepted state:**

- production umbrella `c8cfae8c4025cec6e960a050edf4bfde2a981de5`;
- save schema v23;
- v23 contains MFM-06/07 prescription/provenance semantics;
- no S1-D D-066 schema ownership exists yet.

**Sequence after this review:**

1. #145 repairs the browser amendment; no production ownership.
2. repaired amendment returns to Manager review.
3. only if that review is **APPROVED**, release the corrected #143 save/schema packet. At its actual start, refresh live ownership and `ForestSaveData.CurrentVersion`.
4. if v23 is still current and the next slot is free, **v24 is the expected single S1-D schema increment**; otherwise STOP and repin through Manager. This review does **not** reserve v24.
5. #143 implements/qualifies the one-way teaching latch, save/load/validation and hash exclusion as the sole save/schema/hash writer for this slice, with mandatory independent review.
6. integrate/accept the exact #143 candidate.
7. re-release #124 against that exact lineage; #124 owns curriculum/read-only consumption only and does not touch save/schema/hash.
8. only after the exact post-#143/#124 lineage is known, repin/release PARITY-08 #126 so its compatibility/determinism/reference matrix covers the actual new schema.
9. #128 stays review-only and starts only on the exact #126 handed-off candidate.

No concurrent S1-D/PARITY save writers are authorised.

## Claim and coordination disposition

- **#136:** HANDED-OFF / CLOSED; amendment result reviewed at `ae518ee9c1029eaabbc83c8369c20f02a6bf655a`.
- **#144:** Manager review verdict **NEEDS CHANGES**; browser handoff only.
- **#145:** **PREPARED FOR DISPATCH** browser repair; owns nothing until RUNNING.
- **#143:** **BLOCKED ON #145**; current local packet is not released; no save/schema/hash ownership.
- **#124:** **BLOCKED ON #145**; `b9d204c` remains unqualified evidence.
- **#126:** **BLOCKED ON #145 and later exact S1-D lineage**; old v23-only release basis remains obsolete.
- **#128:** blocked on future repinned #126; review-only.

Live coordination was reconciled at commit **`fa4c561eb080a1816d812407de2d341341d8dcec`**.

## Next executable packet

**Browser repair only:**

- handoff: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-HANDOFF.md`
- claim: #145 — `[PREPARED] Browser repair — narrow D-066 to one authoritative teaching-completion latch`
- purpose: fix the amendment contract; no production code
- difficulty: 4/5
- best model/provider: OpenAI GPT-5.6 Sol browser/GitHub lane or stronger browser-capable reasoning model
- lane: canonical `main`, no local worktree
- user action required: none

Copy/paste dispatch:

`Run browser repair claim #145 from Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-HANDOFF.md. Repair #136's D-066 amendment without changing the accepted Option-A product decision: narrow persistence to the actual minimum one-way historical teaching-sample/review completion latch, pin the write transition to the genuine pre-amendment response→recruitment session boundary including RackReviewComplete, keep T-03's pre-completion semantics but have T-03/T-04 share the durable fact after it is set, and explicitly exclude the teaching-only field from every world/hash identity path with no rebaseline. Keep #143/#124/#126/#128 blocked, browser/GitHub only, no C#/Unity, no Windows build; publish the repair result for another Manager gate.`

## User action required

**None.** The next step is the prepared browser repair #145. No local C#/Unity task is released from this review.

## Dashboard / coordination sync

- live `active-tasks.md`: **SYNCED** at `fa4c561eb080a1816d812407de2d341341d8dcec`;
- #143/#124/#126 issue titles/states: **SYNCED to blocked-on-repair**;
- #128 remains safely downstream/review-only;
- no separate dashboard mutation was required.
