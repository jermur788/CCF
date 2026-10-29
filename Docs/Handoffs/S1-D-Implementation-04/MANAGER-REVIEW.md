# S1-D T-04 — Manager Review

**Task:** S1-D T-04 — Guide monotonicity  
**Claim:** #124  
**Verdict:** **BLOCKED — PRODUCT / SPEC DECISION REQUIRED BEFORE UNITY**  
**Review basis:** approved S1-D specification, plan, requirements checklist and task packet; pushed candidate `b9d204c` on `feature/s1-d-t04-guide-monotonicity-v1`; branch handoff revision 2. The superseded local candidate `18e33d4` is not accepted.

## Inspected candidate

- Exact base: `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- Exact pushed candidate: `b9d204c` (two commits ahead of the base).
- Production changes are limited to the teaching/current-state surface plus the shared teaching-cap helper; no save/schema/validator, completion-rule, objective, Rack, MFM, ecology/economy, scene, package or ProjectSettings file is changed in the GitHub comparison.
- The worker reports the revised offline gates green and correctly withholds Unity qualification and independent-review success tokens.

## Findings

### 1. Prior Manager findings are corrected

**PASS for static design review.** The candidate removes planting as a surrogate for `[J]/[K]` completion and removes arbitrary in-footprint felling from the history-floor proof. `ScenarioOneTeachingHistory.Floor` uses successful beech/oak planting events plus recorded objective-achievement presence and later Annual Review acknowledgement only for the later lesson boundaries. No `PlayerPrefs` or session `[J]/[K]` state is persisted.

### 2. Full FR-B1 / AC-3 is not met

**BLOCKER.** The approved checklist requires the guide to remain stable under Crop Tree count change/loss, later in-footprint felling, executed-share drift **and save/load at lessons 6–8**. The released packet likewise requires a save at lessons 6–8 to reload with the same guide index.

The candidate deliberately applies its historical floor only when `Raw(...) >= recruitment` (lesson 6). Consequently a reload that resets the session-only rack review can make `Raw` return lesson 5, bypassing the floor; the branch handoff explicitly expects a lesson-7/8 save to return to lesson 5 until `[J]/[K]` is repeated. Crop Tree loss can similarly push `Raw` below lesson 6 and bypass the floor.

That behaviour is cautious and avoids inventing history, but it is narrower than the accepted requirement. It cannot be approved as T-04 completion without an explicit spec/product change.

### 3. The implementation has reached the packet's stop condition

The task packet says that when no persisted fact faithfully establishes a required historical-completion state, the worker must preserve existing behaviour and report the gap rather than invent a latch. That is exactly what the revised candidate has now demonstrated for the release/review boundary.

Therefore this is no longer an ordinary coding defect. Continuing to iterate locally without changing authority would force either fabricated inference or an unauthorised persistence change.

### 4. T-03/T-04 do not literally consume one completion predicate

**REQUIRES RESOLUTION WITH THE PRODUCT DECISION.** `ScenarioOneTeachingCap` delegates to `ScenarioOneTeachingHistory.SampleHistoricallyComplete`, but `ScenarioOneTeachingSequence.Current` does not call that predicate; it gates a separate planting/review `Floor` behind `Raw >= recruitment`. The semantics are related, but this is not literally the single persisted-history completion condition described by plan §2.1 / the independent-review requirement.

Do not spend the independent-review lane resolving this while the larger AC-3 product decision is open.

## Product decision required

Choose one direction before further implementation or Unity qualification:

### Option A — preserve the approved AC-3 behaviour

Keep the requirement that a historically completed teaching sample does not regress after reload or later Crop Tree/world drift. Authorise a minimal **durable aggregate completion record** that faithfully records that the teaching sample/review was completed, without persisting the individual `[J]/[K]` decisions themselves. The exact representation must be specified before coding (for example, a purpose-built record in existing persisted history versus a new save field). This reopens persistence/save-compatibility scope and must be coordinated with save/validator ownership and PARITY-08; no implementation is authorised by this review alone.

### Option B — accept the narrower no-new-persistence behaviour

Amend the approved S1-D spec/checklist/packet so FR-B1/AC-3 no longer promises stability when the current guide falls below lesson 6. Explicitly accept that loading a lesson-7/8 save may return the player to lesson 5 until the session inspection is repeated, and that Crop Tree loss can similarly expose earlier current-state lessons. This is a product-scope reduction, not an implementation verification result.

## Recommendation

**Recommend Option A.** T-04 exists specifically to prevent repeated-intervention teaching from regressing after historical completion, including across load. Requiring a player who has already reached lessons 7/8 to repeat lesson 5 after every load undermines that purpose. A single durable aggregate completion fact is preferable to inferring completion from unrelated planting/felling and preferable to weakening AC-3, provided the save-compatibility work is explicitly specified and serialized against PARITY-08 ownership.

## Gate state

- Candidate `b9d204c`: **not approved for Unity qualification yet**.
- Unity lock: do not acquire for #124 until the product/spec decision is resolved and the resulting candidate is statically accepted.
- Independent review: wait until after that corrected candidate exists.
- MFM-09 remains blocked on accepted S1-D T-04/T-05.
- No Windows build is authorised.
