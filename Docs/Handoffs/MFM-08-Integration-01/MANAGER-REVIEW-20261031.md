# MFM-08 Integration 01 — Manager Review — 2026-10-31

**Claim:** #205  
**Feature:** #156 MFM-08  
**Verdict:** **APPROVED — INTEGRATION CANDIDATE MAY BECOME THE PUBLISHED SCENARIO ONE INTEGRATION HEAD**

## Reviewed exact lineage

- Published integration input: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`
- Approved MFM-08 source candidate: `e5fb424635f16182748efcb0539e8a49539495a9`
- Integrated production candidate: `34e360f85835afd15f5323f287372e0f2ee83dd9`
- Verification-only post-integration harness: `c9336186781022f44b9c2e53eebb6764da080204`
- Worker handoff tip: `3920ef14`

GitHub compare confirms `34e360f8...` is exactly three commits ahead of `ce673714...`, with merge base equal to `ce673714...`. The changed-file set is the expected 15-file MFM-08 production/offline-verification delta only.

The worker additionally reports the replay diff against `ce673714...` is byte-identical to the approved source delta `cfdc93bf...e5fb4246`, with no conflicts. #196 verification-only `eef1bdf9` and #199 partial `42742a8a` were not absorbed.

## Qualification accepted

Returned local evidence is accepted as sufficient post-integration qualification:

- Unity import/compile clean; 0 `error CS`.
- Focused MFM-08: `MFM08_UNITY_PASS checks=373 shots=25` at 1280x720 and 1920x1080.
- RackV2MapPlanning under the Manager-approved #196 contract: `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`.
- PARITY-08 save compatibility: `PARITY08_SPECIES_OFFLINE_PASS checks=23`, including the v24/D-066 teaching-latch WorldHash invariant.
- MFM-08 offline/static logic: `MFM08_TREATMENT_REVIEW_PASS checks=297 failed=0` plus repeat determinism PASS.
- Lane/Editor/Unity lock returned clean; no Windows build.

The disclosed first MFM-08 run failure is accepted as a stale verification-contract pin (`ForestSaveData.CurrentVersion == 23`) against the already accepted v24/D-066 lineage. The bounded verification-only reconciliation retains an exact version assertion and mirrors the already approved #198 class of repair. It is not a production defect and is not part of the production integration candidate.

The offline `MFM08_BOUNDARY_FAIL` is also accepted as expected verification-policy behaviour on a later integration lineage because that boundary check is pinned to the original MFM-08 start commit; the exact replay-equivalence proof is the applicable scope evidence for #205.

## Compatibility / protected gates

No save/schema/reference/anchor/ecology/economy path is changed by the production replay. Frozen Reference Future v1 and Legacy40 compatibility remain protected. #199 Combined HUD/focus debt remains separate and is not silently treated as resolved by this approval.

## Integration decision

**APPROVED.** Fast-forward `integration/scenario-one-player-test-ready-v1` from exact expected head `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` to exact reviewed production candidate `34e360f85835afd15f5323f287372e0f2ee83dd9` using an expected-head lease. Do not move the branch if its head changed unexpectedly.

After the branch move:

1. #205 may close as APPROVED / INTEGRATED.
2. #156 may close as MANAGER APPROVED / INTEGRATED.
3. Downstream work must be repinned to exact `34e360f8...` before execution where the prior packet was based on `ce673714...`.
4. Preserve the established critical path: S1-C T-08 -> S1-D T-08 -> S1-D T-09, while MFM-09 can be repinned/released on the new exact integration lineage. #199 Option 1 must also be repinned before production edits and remains required before final Tier-3.

No Windows build. User playtest and acceptance remain later distinct gates.
