# S1-C T-07 — Final Manager Review

**Date:** 2026-10-30  
**Claim:** #187  
**Verdict:** **APPROVED**  
**Accepted result:** `S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW`

## Reviewed lineage

- Released base: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`
- Verification HEAD: `e64b337efd3b758fe6bc21636822ad178f0df0ca`
- Worker handoff commit: `8228a266` (docs-only descendant)
- Reported branch tip: `d104a3b0`
- Branch/worktree: `verify/s1-c-t07-data-contract-v1` / `/media/jer/Videos/CCF-S1C-T07`
- Worker: opencode-go / deepseek-v4.1-flash
- Unity: not used; not required by the released T-07 packet

GitHub compare confirms `ce673714...` → `e64b337e...` is one commit adding only the three focused verification files under `Tools/Verification/S1CManagementHistory/`. Compare `e64b337e...` → `d104a3b0` adds only `Docs/Handoffs/S1-C-Implementation-07/HANDOFF.md`. No production/save/schema/UI/MFM file changed.

## Evidence reviewed

The handoff records `304/304` passing checks and `8/8` mutation controls caught.

The gate is not a token-only harness. The runner compiles the real production `ScenarioManagementHistory`, persisted record types, `ScenarioOneSaveMigration`, `ForestSaveValidation`, related validation/model dependencies, and then runs the focused check in a temporary project outside the repository. The runner pins the exact SHA-256 values of both genuine save inputs and refuses mismatches. It records lane status before/after and requires equality for a passing result.

The eight mutation controls deliberately break reader/event-ID behavior for:

1. event renumbering;
2. work-order re-keying;
3. fabricated cancellation replacement identity;
4. `harvestJobId` treated as unique job identity;
5. invented grouped-harvest per-order money;
6. query mutation of an input order;
7. harvest label substituted for order identity;
8. duplicate event IDs accepted by the validator.

All eight were reported caught.

## Genuine-save boundary

The previously rejected synthetic `v20-records` fixture is **not** used as the genuine old-save witness.

Accepted old-save witness:

- v20 year-5 save: `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures/v20-y5.json`
- SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`
- handoff traces it byte-for-byte to the real rendered/player save under the 2026-10-17 Rack-first UX evidence and cites prior accepted T-05 Unity typed/disk-load evidence.

Accepted current S1-C witness:

- recorded v22 year-3 save: `/media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/02-objective/evidence/recorded-v22-year3-save.json`
- SHA-256: `95e5dd37d7f546d78cb94cda8e5af1641b576c925cc37313ce88aa0a54167b9e`

T-07 does not claim to have repeated Unity load. It re-parses the same pinned bytes offline through the production normalizers and history reader. This is the correct boundary for the released offline T-07 task. Fresh current-schema and genuine-old-save Unity load remain part of S1-C T-08.

## Contract findings accepted

The evidence is sufficient for T-07 / AC-10:

- management-event and work-order identities remain positive/unique and pass through without reader renumbering/re-keying;
- next-ID counters and record graphs remain stable across repeated reads/reparse;
- existing resolved orders link by authoritative IDs/references, while vanished/cancelled orders remain represented by surviving events rather than fabricated records;
- the reader introduces no second static identity generator/cache/store;
- `harvestJobId` remains a resolution-year grouping label, not a globally unique job identity;
- grouped harvest money remains job/year-level and no authoritative per-order harvest money is invented;
- older saves do not synthesize newer objective/prescription/rack provenance that was not recorded;
- broader identities actually consumed by the reader are pass-through rather than re-keyed;
- accepted `ForestSaveValidation` remains the management-event duplicate/non-positive-ID authority.

## Limitations carried forward

This approval is **offline contract qualification only**. It does not independently verify Unity import/runtime wiring, rendered Annual Review/Work Plan states, current-schema disk round-trip, protected hashes, or live gameplay. Those are explicit T-08 Tier-2 responsibilities.

The verification harness additions are accepted evidence tooling. They do not alter the Scenario One production candidate, which remains current integration `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` pending later accepted integrations.

## Manager verdict

**APPROVED.**

`S1-C T-07 DATA CONTRACT STABILITY — MANAGER ACCEPTED`

#187 may close. S1-C T-08 is now dependency-satisfied on T-07 and may be repinned/released against the current exact integration candidate, but actual Unity execution remains serialized behind the current higher-priority MFM-08 #156 lane and the machine-wide Unity-lock governance repair #189. No absence of a live lock is inferred.

No Windows build is authorised.
