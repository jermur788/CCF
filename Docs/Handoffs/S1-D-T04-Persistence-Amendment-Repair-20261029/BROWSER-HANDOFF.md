# Browser Handoff — Repair D-066 to the Minimum Authoritative Teaching-Completion Latch

## Task identity

- **Claim:** #145
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Trigger:** browser Manager review #144 of amendment #136
- **Task class:** browser/GitHub-only specification/packet repair; **no production C#/Unity**
- **Status at publication:** **PREPARED FOR DISPATCH**
- **Purpose:** correct the Option-A persistence amendment before any local save/schema work starts. Narrow the persistence authority to the actual minimum durable teaching-completion fact, pin the authoritative transition that writes it, and make save/hash/ownership sequencing exact.
- **Difficulty:** **4/5** — browser-only, but requires code-level static inspection of the current qualified lineage and careful authority repair.
- **Best-suited model/provider:** OpenAI GPT-5.6 Sol browser/GitHub lane or stronger browser-capable reasoning model.
- **Lane / branch / worktree:** canonical GitHub browser lane on `main`; no local worktree.
- **User action required:** none.

## Manager-review findings to repair

The accepted product decision D-066 is sound. The #136 amendment implementation contract is not yet safe to release.

### Finding 1 — persisted shape is broader than demonstrated need

The current amended spec/plan/tasks/checklist requires, for each teaching/management cycle, a stable cycle key, separate sample/review completion flags and aggregate counters/outcomes. No inspected evidence demonstrates that this structure is necessary to satisfy the user's selected Option A.

The direct user decision authorises the **smallest durable aggregate fact** needed to remember that the authoritative teaching sample/review has historically completed. Broad per-cycle state risks recreating the cancelled T-07 management-cycle persistence under another name.

**Required repair:** default to one monotone Scenario-One teaching-completion latch for the existing first guided teaching sample/review. Do not authorise per-cycle keys, counters, replay records or repeated sample-history records unless fresh repository inspection proves they are strictly required for the accepted current curriculum. If such proof appears, STOP and return to Manager rather than broadening silently.

### Finding 2 — authoritative write transition is not pinned

On the pre-amendment qualified teaching lineage, `ScenarioOneTeachingSequence.Current(Facts)` advances past the `response` lesson only when the real rack-world session review is complete. The relevant condition includes `ReleaseReviewed` **and** `RackReviewComplete` (with the preceding release/deadwood/current lesson prerequisites already satisfied).

By contrast, the existing T-03 `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` helper only proves the executed release/review geometry/outcome condition and deliberately does **not** include the session `[J]/[K]` review completion. It therefore cannot by itself authorise the D-066 durable latch.

**Required repair:** define the write transition as the authoritative pre-amendment transition from lesson `response` to `recruitment` in an Enlarged80/rack-world session — i.e. the current-state teaching rules have genuinely passed all lessons 1–5, including the real `RackReviewComplete` session review. A later review acknowledgement or T-03 helper alone is insufficient.

The specification/packet must say how this condition remains testable after extracting/refactoring a helper. The implementation may use a pure helper, but its semantics must match the pre-amendment guide transition before applying any historical latch.

### Finding 3 — T-03/T-04 shared semantics need two phases

D-066 says the durable fact is shared with the T-03 teaching-cap interpretation. The current amendment can be read as replacing T-03's existing pre-completion logic with the new durable latch, which would be wrong before the player has actually finished the session review.

**Required repair:**

- **before durable completion:** T-03 keeps its existing in-session cap classification and current-world safeguards; the durable fact is false and does not fabricate lesson completion;
- **at the authoritative response→recruitment transition:** write the durable fact once;
- **after durable completion:** both T-03 and T-04 may consume the same durable true value as the historical `sample/review complete` fact; later world drift cannot unset it.

This creates one historical meaning without pretending the earlier T-03 helper alone proves the session review occurred.

### Finding 4 — world/hash identity contract is underspecified

On the qualified v23 lineage, `ScenarioReferenceArchive.HashLayout(...)` strips v22 objective-achievement history from world identity, but a world carrying real v23 prescription state hashes its full current JSON except for that explicit history field. A new teaching-only field would therefore alter world identity unless explicitly excluded.

Teaching progression is not authoritative biological/world state. D-066 also forbids silent protected-hash rebaseline.

**Required repair:** state canonically that the D-066 teaching-completion latch is **never part of world identity**. The local implementation packet must require the existing hash canonicalisation/compatibility path to strip the new teaching-only field on every relevant hash path, including worlds with non-default v23 prescription state, while preserving full save round-trip. No accepted anchor is rebaselined.

The repair may authorise the smallest required hash-layout edit as part of the single #143 save writer; #124 still owns no hash/save code.

## Required browser work

### 1. Refresh live authority/ownership

Before edits:

- refresh `main` and `Docs/Project/coordination/active-tasks.md`;
- refresh #143/#124/#126/#128/#144/#145;
- verify no production save/schema writer has started;
- mark #145 `[RUNNING]` with actual model/provider and starting main SHA;
- register browser ownership only for the canonical spec/packet/review/coordination files being repaired.

If a local save writer has started despite the blocks, STOP and return to Manager.

### 2. Inspect exact authoritative code

At minimum inspect the exact qualified post-MFM-07 lineage `c8cfae8c4025cec6e960a050edf4bfde2a981de5` and the last accepted pre-amendment teaching code inherited from the S1-D T-03 base. Confirm:

- the exact `ScenarioOneTeachingSequence.Current` conditions for leaving `response` and reaching `recruitment`;
- how `RackReviewComplete` is computed and when it becomes true in-session;
- the current T-03 `ScenarioOneTeachingCap` semantics;
- save capture/restore and pre-mutation validation architecture;
- `ScenarioReferenceArchive` hash stripping/canonicalisation, especially the v23 prescription-state branch.

Static inspection is evidence only; do not claim Unity verification.

### 3. Repair canonical D-066 text without changing the user decision

Update at minimum:

- `specs/scenario-one-repeated-interventions-v1/spec.md`
- `plan.md`
- `tasks.md`
- `checklists/requirements.md`
- `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md`
- `Docs/Handoffs/S1-D-Implementation-04/MANAGER-REVIEW.md`
- `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/TASK-PACKET.md`

Do **not** change Decision Log D-066 unless wording must be clarified to remove an actual contradiction. Its accepted product decision is not under review.

Required semantic end state:

- one minimal monotone durable fact for the existing first teaching sample/review, unless inspected code proves more is strictly needed;
- no per-cycle key/counter/history structure by default;
- field absent/false on old saves = not historically complete, never reconstructed retrospectively;
- write exactly once at the authoritative pre-amendment response→recruitment transition after actual `RackReviewComplete`/lesson-1–5 success;
- individual `[J]/[K]`/session detail still non-persistent;
- T-03 before latch keeps existing session/current-world semantics; after latch it and T-04 consume the same durable historical fact;
- #124 curriculum remains a separate serialized follow-on and owns no save/hash code;
- teaching latch excluded from all world/hash identity paths, no rebaseline;
- MFM/Rack/completion/objectives/ecology/economy/Reference Future content unchanged.

### 4. Make the #143 packet implementation-ready

The repaired #143 packet must no longer ask the local worker to rediscover product architecture.

It must name the intended representation in product terms (one monotone teaching-sample/review-complete boolean/latch in Scenario One save state; exact C# field naming may be fixed by the repair after source inspection), the exact authoritative write condition, old-save default, validation expectations, hash exclusion rule, expected schema sequence, and the narrow API consumed later by #124/T-03.

The packet may still allow ordinary implementation choices such as helper method names, but not whether the record is a bool versus per-cycle history or what event makes it true.

### 5. Keep the schema sequence blocked until repaired approval

Current inspected production schema remains v23 at `c8cfae8c...`. v24 is only an **expected next version**, not released ownership, until the repaired amendment is Manager-approved and the local worker rechecks live state.

#143/#124/#126/#128 remain blocked during this browser repair.

### 6. Publish repair result for another Manager gate

Publish:

`Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-RESULT.md`

State exact commits, repaired semantics, inspected code evidence, whether one bool/fact was sufficient, exact hash rule, schema expectation, claim states and any blocker.

Do not directly release local production from #145. Return the repaired amendment to Manager review (reuse/new review claim as coordination requires).

## Stop conditions

Stop and return to Manager if:

- exact current code shows one monotone first-sample completion fact is insufficient and requires a materially broader persisted model;
- `RackReviewComplete` cannot be cleanly identified as the authoritative current-session boundary without persisting individual choices;
- old accepted saves cannot default false without a new compatibility decision;
- the teaching-only field cannot be excluded from world identity without changing accepted reference/hash semantics rather than merely preserving them;
- save/schema ownership changed or v24 became occupied;
- any repair would alter completion rules/objectives, ecology/economy, Rack V2, MFM prescription semantics or frozen Reference Future v1.

## Handoff protocol

At start comment on #145:

`[RUNNING] Browser repair — D-066 minimum authoritative teaching-completion latch — <model/provider>, canonical main start <SHA>, no local worktree/Unity.`

At completion publish the repair result, mark #145 `[HANDED-OFF]`, close it and return to Manager review. Do not release #143 locally from this repair claim.

## Copy/paste dispatch line

`Run browser repair claim #145 from Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-HANDOFF.md. Repair #136's D-066 amendment without changing the accepted Option-A product decision: narrow persistence to the actual minimum one-way historical teaching-sample/review completion latch, pin the write transition to the genuine pre-amendment response→recruitment session boundary including RackReviewComplete, keep T-03's pre-completion semantics but have T-03/T-04 share the durable fact after it is set, and explicitly exclude the teaching-only field from every world/hash identity path with no rebaseline. Keep #143/#124/#126/#128 blocked, browser/GitHub only, no C#/Unity, no Windows build; publish the repair result for another Manager gate.`