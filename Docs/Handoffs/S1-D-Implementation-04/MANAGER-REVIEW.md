# S1-D T-04 — Manager Review

**Task:** S1-D T-04 — Guide monotonicity  
**Claim:** #124  
**Current verdict:** **BLOCKED ON AUTHORISED D-066 PERSISTENCE PREREQUISITE #143**  
**Prior verdict:** BLOCKED — PRODUCT / SPEC DECISION REQUIRED BEFORE UNITY  
**Review basis:** approved S1-D specification/package; pushed candidate `b9d204cbae687da71d32a9da42575ca3f298a511`; the user-selected Option A now recorded as Decision Log **D-066**; browser amendment #136.

## Prior candidate finding retained

`b9d204c` corrected the earlier rejected surrogate proofs and correctly withheld Unity qualification, but it still cannot satisfy full FR-B1 / AC-3 by derivation alone. Reload can reset session-only rack review so the raw guide falls behind the intended historical boundary; Crop Tree loss/current-state drift can likewise expose earlier lessons. The packet's stop condition was therefore reached honestly.

The candidate remains **useful implementation evidence only**. It is not accepted, qualified or an integration base and must not be stacked blindly after the persistence prerequisite.

## Product decision — resolved

The user selected **Option A**. D-066 now authorises the smallest durable aggregate historical record needed to remember that the authoritative teaching sample/review has actually completed.

Binding boundaries:

- preserve full FR-B1 / AC-3; do not weaken save/load or world-drift monotonicity;
- individual `[J] Retain here` / `[K] Release further` decisions remain session-only and are not serialized;
- inspected-tree lists, stand-portion detail and replayable session-review transcript remain non-persistent;
- the aggregate is authority only for historical teaching progression and the shared T-03 sample-complete meaning;
- the aggregate becomes complete only from the **existing authoritative pre-amendment in-session teaching sample/review completion path**, not from planting, arbitrary felling, a later Annual Review, current Crop Tree count, current geometry or another surrogate;
- once complete, it is monotone historical state;
- older saves with the field absent default conservatively to not historically complete;
- current readiness/recommendation state remains recomputable; broad T-07 persistence stays cancelled;
- Scenario One completion rules/objectives, economy/ecology, Rack V2, MFM and Reference Future v1 are unchanged.

## Required prerequisite

The amended task structure introduces **T-04P / claim #143** as the bounded save/load prerequisite.

#143 owns the minimum aggregate teaching-history persistence, schema/version plumbing, save capture/restore, pre-mutation validation and focused compatibility qualification. It must be the sole save/schema writer while RUNNING and requires independent review because it changes the shared save/load contract.

At browser amendment inspection, the exact post-MFM-07 qualified production umbrella is `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, whose `ForestSaveData.CurrentVersion` is **23**. The expected next schema is v24 **only if** the #143 worker refreshes live ownership/source at start and still finds v23 free. If the lineage/version/ownership changed, the worker stops and returns to Manager rather than reusing a consumed version.

## #124 continuation contract

After #143 is integrated and qualified:

1. refresh live ownership and the exact #143 handoff/integration SHA;
2. rebase/replay useful T-04 logic from `b9d204c` against that exact lineage;
3. consume the #143 durable teaching-completion API rather than creating another store;
4. preserve pre-completion teaching behaviour and all existing lesson IDs;
5. prove full AC-3 across Crop Tree count/loss, later in-footprint felling, executed-share drift and save/load;
6. prove repeated reads/calls do not consume another sample;
7. rerun TeachingCopy/MenuTutorial/T-03/T-02/S1-C/N3 checks with no protected-hash rebaseline;
8. run Unity qualification under the machine-wide lock;
9. obtain independent review on the exact persistence + curriculum candidate.

## Save/schema sequencing with PARITY-08

Safe single-writer order:

1. #143 S1-D minimal persistence owns save/schema first.
2. #124 curriculum replay follows after #143 integration/qualification; #124 does **not** own schema/save files.
3. PARITY-08 #126 remains blocked until the exact post-#143/#124 lineage is known, then is repinned so its compatibility/hash/reference checks cover the new field rather than obsolete v23-only state.
4. #128 remains the mandatory independent PARITY-08 review after #126 hands off.

No concurrent save/schema writers are authorised.

## Gate state

- Candidate `b9d204c`: **not accepted / not Unity-qualified**.
- #143: next local persistence prerequisite once released by browser amendment/Manager review.
- #124: **BLOCKED on #143 integration/qualification**.
- Unity lock: do not acquire for #124 before #143 is accepted and #124 is explicitly re-released.
- Independent review: required after the corrected exact candidate exists.
- MFM-09 remains blocked on accepted S1-D T-04/T-05.
- PARITY-08 #126/#128 remain blocked pending exact amended lineage.
- No Windows build is authorised.

## Next Manager decision

The browser amendment itself must receive an independent browser Manager review before production release. That review will issue **APPROVED / NEEDS CHANGES / REJECTED** against the exact #136 result and, if approved, release the next executable local packet and repin #126/#128 safely.