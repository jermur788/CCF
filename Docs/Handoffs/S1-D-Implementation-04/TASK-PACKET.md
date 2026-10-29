# S1-D T-04 — Guide Monotonicity — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-04 — Guide monotonicity
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Claim:** #124
- **Status:** **BLOCKED ON D-066 PERSISTENCE PREREQUISITE #143**
- **Historical base:** `b971f4da0540e606bfee9702b0c99f81a0a89355`
- **Historical pushed candidate:** `b9d204cbae687da71d32a9da42575ca3f298a511` — useful evidence only; **not accepted, not Unity-qualified, not an integration base**.
- **Purpose:** finish Scenario One guide monotonicity after the minimum durable aggregate teaching-sample/review-completion record authorised by D-066 exists on the exact implementation lineage.
- **Difficulty:** **4/5** — bounded curriculum surface, but it consumes save-backed historical teaching completion and must preserve the pre-completion lesson behaviour exactly.
- **Preferred implementation model/provider:** existing #124 owner, Claude Sonnet 5.5 / Anthropic, when re-released; equivalent strong local code model is acceptable if ownership is reassigned.
- **Independent review:** required on the final exact candidate after persistence + curriculum are both present.

Suggested continuation lane remains:

- branch `feature/s1-d-t04-guide-monotonicity-v1`
- worktree `/media/jer/Videos/CCF-S1D-T04`

Do **not** resume #124 yet. Refresh live coordination, claim #143 and the exact integrated prerequisite handoff first. The claim itself never grants ownership.

## Authority amendment — D-066

The user selected Option A from the prior Manager review. Full FR-B1 / AC-3 remains binding.

The authorised durable proof is now the **minimum aggregate historical teaching-sample/review-completion record** implemented by prerequisite #143. Individual `[J]/[K]` choices, inspected-tree lists, stand-portion detail and session-review transcript remain non-persistent.

The aggregate may become historically complete **only when the existing authoritative in-session teaching sample/review has actually completed under the pre-amendment lesson rules**. Planting, arbitrary felling, a later Annual Review, current Crop Tree count, current rack geometry or any other surrogate must not fabricate completion. Once established, completion is monotone historical state.

Pre-amendment saves that lack the record default conservatively to **not historically complete** for this latch.

## Required execution order

1. Browser amendment #136 publishes D-066 and the amended spec/package.
2. **#143 / T-04P** implements, independently reviews and qualifies the minimum persistence/save-load slice from the exact production lineage.
3. #143 is integrated to the exact lineage named in its handoff.
4. #124 is re-released and **rebased/replayed**, not blindly stacked, against that exact integrated prerequisite.
5. #124 completes offline + Unity qualification and independent review.
6. Manager reviews the exact #124 candidate before any later teaching insertion.
7. PARITY-08 #126 remains blocked until the resulting save/schema lineage is exact; #128 remains blocked behind #126.

## Released start gate for the continuation

#124 may resume only when all are true:

- #143 is `[HANDED-OFF]`, Manager-approved and integrated/qualified on one exact lineage;
- live coordination gives #124 sole curriculum ownership;
- no MFM teaching writer owns `ScenarioOneTeachingSequence` / `LearningObjectivesView.Sequence`;
- the #143 handoff identifies the durable teaching-completion read API and exact schema/version;
- the worker rebases/replays from the exact integrated prerequisite, preserving useful logic from `b9d204c` only where it still conforms.

If any gate is false, stop before editing.

## Required behaviour

Implement approved FR-B1 / FR-B2 / FR-B3 / AC-3 with D-066:

- preserve existing current guide behaviour before historical sample/review completion;
- once the durable #143 record establishes the relevant teaching boundary, later Crop Tree count increase/decrease, Crop Tree loss, later in-footprint felling, executed-share drift and save/load must not move the guide behind that boundary;
- consume the #143 aggregate as historical teaching proof only; do not make it a forestry prescription, Scenario One completion input, MFM state or current-world inspection substitute;
- do **not** read `PlayerPrefs` or session-only `[J]/[K]` state as durable progression authority;
- do not serialize `[J]/[K]` or any session-review transcript;
- T-03 and T-04 must consume one shared meaning/API for `sample historically complete`; do not implement competing definitions;
- preserve every stable lesson ID: `access`, `future-trees`, `release`, `habitat`, `response`, `recruitment`, `establishment`, `transformation`;
- no rename/reuse of saved IDs.

## Expected owned production surface while #124 RUNNING

Primary:

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`

Allowed only if strictly necessary:

- smallest read-only adapter needed to consume the #143 aggregate API;
- focused verification files.

**Not owned by #124:** `ForestSaveData.cs`, save controller/validation/schema, #143 persistence implementation, completion rules, objective predicates/targets, Rack V2 lifecycle/geometry, ecology/economy, MFM implementation, scenes/prefabs/packages/ProjectSettings, Reference Future v1.

If the #143 API is insufficient, return to Manager/#143; do not expand #124 into a second save writer.

## MFM sequencing boundary

#124 does not implement Multi-Scale Forestry Management. MFM-09 teaching insertion remains later and must wait for accepted S1-D T-04/T-05. If MFM has rewritten the same curriculum seam before #124 resumes, stop and repin/rebase through Manager ownership rather than merging assumptions.

## Required verification

### Offline/static matrix

Cover at least:

- current guide state before durable completion;
- authoritative transition that records sample/review completion;
- Crop Tree count increase/decrease after completion;
- Crop Tree loss after completion;
- later in-footprint felling after completion;
- executed-share drift after completion;
- save/load/rebuild from the persisted aggregate at lessons 6–8 / the exact unchanged IDs;
- repeated reads/calls in one state do not consume another sample or create another record;
- pre-amendment save with aggregate absent does not fabricate completion;
- all stable lesson IDs unchanged.

Regression requirements:

- TeachingCopy / relevant MenuTutorial checks;
- T-03 teaching-cap decision checks using the same historical-completion interpretation;
- T-02 cycle-state checks;
- N3 normalised completion-rule hash unchanged — **no rebaseline**;
- S1-C history/query checks;
- #143 save/load/validation regression remains green.

### Unity qualification

Under the machine-wide Unity lock:

- import/compile exact candidate;
- save at lessons 6–8 / exact corresponding IDs, reload, prove guide index does not regress after historical completion;
- exercise Crop Tree loss/count change, later felling and executed-share drift after historical completion;
- prove pre-completion teaching behaviour remains unchanged;
- prove repeated view refreshes/calls do not consume another teaching sample.

### Independent review

Independent reviewer on the exact persistence + curriculum candidate must confirm:

- durable aggregate is the only new S1-D persistence and is read as historical teaching proof only;
- individual `[J]/[K]` decisions are not serialized;
- the completion transition is the authoritative old in-session teaching completion, not a surrogate;
- T-03/T-04 share one completion meaning/API;
- no completion-rule/objective/economy/ecology/MFM/Rack/Reference change;
- no stable-ID rename/reuse and no weakened tests/hash rebaseline.

## Stop conditions

Stop and return to Manager if:

- #143 is not integrated/qualified on the exact base;
- #143 aggregate cannot express the required historical completion without persisting individual `[J]/[K]` or substantially more session detail;
- a new live writer owns the same curriculum files;
- another schema/save field is requested by #124;
- `ScenarioOneCompletionRules` or objective predicates would need to change;
- MFM has already rewritten `Current()` on the target lineage;
- any protected hash/reference would need rebaseline.

## Handoff

Publish `Docs/Handoffs/S1-D-Implementation-04/HANDOFF.md` with:

- actual model/provider;
- exact prerequisite/integration starting SHA and ending production SHA;
- branch/worktree;
- changed files;
- exact #143 durable-proof API consumed;
- lesson → historical-proof mapping;
- pre-completion fallback behaviour;
- offline + Unity evidence;
- independent-review verdict;
- proof `[J]/[K]` are not serialized;
- stable-ID and N3-hash preservation;
- residuals / next dependency.

On successful handoff, rename #124 `[HANDED-OFF]`, link the handoff, close the claim and return:

`S1-D GUIDE MONOTONICITY QUALIFIED — READY FOR MANAGER REVIEW`

or a precise blocker.

## Copy/paste dispatch line

`Resume claim #124 only after prerequisite #143 is integrated and qualified. Rebase/replay S1-D T-04 guide monotonicity on the exact #143 integration SHA; consume its D-066 aggregate teaching-completion API so full FR-B1/AC-3 survives Crop Tree drift/loss, later felling, executed-share drift and save/load. Preserve pre-completion lesson behaviour and all stable lesson IDs. Do not persist J/K, do not edit save/schema/validator, do not change completion/objectives/ecology/economy/Rack/MFM/Reference, rerun offline + Unity + independent review, and do not build Windows.`