# S1-C T-08 — Manager Review — 2026-10-31

**Claim:** #206  
**Feature:** Scenario One Review / Management History V1  
**Exact production candidate:** `34e360f85835afd15f5323f287372e0f2ee83dd9`  
**Returned handoff:** `Docs/Handoffs/S1-C-Implementation-08/HANDOFF.md` at `f84214764afacbc77ffb329cfb0f7ba7d6815b24`  
**Verdict:** **APPROVED — FEATURE QUALIFIED**

## Manager finding

The returned Tier-2 evidence is sufficient to accept S1-C T-08 on the exact published Scenario One candidate `34e360f8...`. The worker changed verification tooling only; production bytes remained unchanged.

The accepted result token is:

`S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`

## Accepted evidence

The qualification exercised the complete S1-C review/history matrix and protected compatibility chain:

- S1-C T-07 offline data-contract/stability check: **304/304**, with **8/8** deliberately broken mutations caught;
- objective/history/save/old-save gate: **544 checks / 34 cases**;
- rendered objective-history gate: PASS;
- T-02 year browsing: **246** checks;
- T-03 drill-down: **279** checks;
- T-04 provenance: **529** checks;
- T-06 guidance/waypoint: **170** checks;
- frozen Reference Future v1: PASS;
- SaveHardening: PASS;
- ScenarioOneProgress: PASS;
- baseline-hash: **30/30** accepted identities reproduced, with no anchor relabelling;
- interaction: PASS;
- density/save compatibility: PASS;
- Scenario One completion: PASS;
- 84 final 1280x720 / 1920x1080 captures, with a representative set directly inspected and the remainder covered by in-engine fit/clipping assertions;
- current schema v24 save/load round trip exercised;
- genuine supported v20-y5 save SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00` loaded through the current v24 path; the rejected synthetic `v20-records` fixture was not substituted.

The returned cleanup evidence records normal lock release, a clean U1 lane, no remaining Editor/Xephyr process, restored Editor side effects and no Windows build.

## Deviation review

### 1. Baseline-hash verification reconciliation — ACCEPTED

The original live pre-v22 derivation became stale on the v23/v24 lineage because its `PreV22SaveTypes` shares current work-order/event types, which now serialise later prescription fields. The verification-only reconciliation at `e0f0437c...` does **not** establish a new baseline. It uses the already accepted T-05 pre-v22 identity table produced from the real historical hashing source and requires:

- every accepted row to be internally `candidate == baseline`;
- the accepted table row count to match the executed case count;
- every current candidate identity to equal that accepted historical identity exactly;
- missing/malformed rows or any mismatch to fail the gate.

The live derived value is retained diagnostically. This is a bounded harness repair restoring the intended protected-identity comparison, not an anchor rebaseline.

### 2. Mixed verification heads — ACCEPTED

The main final gate chain ran at `f3470edb...`; the final verification head `e0f0437c...` changed only:

1. `Tools/Verification/S1CObjectiveHistoryUnity/S1CHashBaselineGate.cs`; and
2. a gate-specific runner environment pass-through used only for `baseline-hash`.

Compile and the baseline-hash gate were rerun and passed at `e0f0437c...`. The harnesses for gates 02–10 and 12–14 were unchanged between the two heads, and their recorded SHA-256 identities remained the same. A full rerun of unrelated gates at the final verification head is therefore not required for this Tier-2 verdict.

### 3. Runner direct-entry repair — ACCEPTED

The `f3470edb...` runner change prevents T-02/T-03/T-04/T-06 from inheriting `CCF_TEST_DIRECT_ENTRY`, because those fixtures exercise their own startup/new-game path. This is execution-environment correction only; no assertion or production behaviour was weakened.

## Scope and compatibility

No production, save-schema, anchor, Reference Future, ecology, economy, MFM, Rack or acceptance-threshold change is accepted by this verdict. Frozen Reference Future v1 and Legacy compatibility remain protected.

This verdict qualifies S1-C Review / Management History V1 at Tier 2. It does not constitute final Scenario One Tier-3 qualification or user acceptance.

## Downstream decision

The S1-C dependency that blocked S1-D T-08 is cleared. S1-D T-08 may now be released on the same exact published candidate `34e360f8...`, subject to a fresh local ownership/free-space/Editor/Unity-lock preflight. It remains qualification-only with no production-repair authority.

S1-D T-09 remains a separate independent conformance review after T-08 PASS.

No Windows build.