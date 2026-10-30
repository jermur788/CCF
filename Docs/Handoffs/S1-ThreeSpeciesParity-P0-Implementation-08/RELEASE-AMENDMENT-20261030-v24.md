# PARITY-08 Release Amendment — v24 lineage — 2026-10-30

Parent: #91 `PLAYER-TEST-OVERNIGHT-01`  
Claim: #126  
Status: **RELEASED FOR IMPLEMENTATION ON THE v24 LINEAGE**

This amendment supersedes the older `2026-10-29` release amendment and any v23-only base wording in the issue body/packet. All technical scope, exclusions, verification and stop conditions in `TASK-PACKET.md` remain binding unless explicitly changed below.

## Exact lineage

- Integration production merge for S1-D T-04P: `2f476a52`
- S1-D T-04P docs successor: `bccd2b90`
- S1-D T-04 offline review + Unity harness reconciliation integrated at **`185200f5`** (merges `99b7d4b4` and `edba1c35`; production tree unchanged from `bccd2b90`/`2f476a52`, runtime compile 184 sources / 0 errors).
- **Exact production/integration base for #126: `185200f5`**
- Current save schema: **v24**. v24 adds exactly one persisted teaching-completion boolean (`ScenarioOneSaveData.firstGuidedRackReviewComplete`), which every world/reference/protected identity/hash projection strips. No parity-specific field may be added.

## Why released now

#143 is independently reviewed and integrated; #124 is offline-reviewed, Unity-qualified through the #164 harness reconciliation, and integrated at `185200f5` with production unchanged. The S1-D persistence/curriculum lineage is therefore exact and accepted for this release. #128 remains the independent review and starts only after #126 publishes one exact candidate.

## Changed wording

- Where the older packet says "current v23 MFM save/provenance coexistence", read **"current v24 MFM save/provenance coexistence"**: v24 adds only the stripped teaching latch, and must not be reinterpreted as parity state.
- Existing v23/v24/v25 compatibility paths must be proven, not assumed. A v24 save that carries the teaching latch true must load without parity reinterpretation, and the latch must not enter any parity hash/anchor.
- No parity-specific schema bump. The base schema is v24; if a genuine new field were required, that is a stop condition.

## Routing

- Difficulty: **5/5** — high-risk save validation/determinism/historical compatibility.
- Preferred executor: **Claude Opus 5.5 Low** (justified by 5/5 save/reference/determinism consequence; user policy permits Opus Low/Medium when justified).
- Mandatory independent reviewer before PARITY-09: **DeepSeek V4.1 Flash** (cross-provider from Claude) or OpenAI when its quota is restored. Reviewer is read-only; repairs return to #126.
- Unity remains machine-serialized; fresh lock/process/free-space/priority preflight. The lane is currently free after #164.

## Lane

- Branch: `verify/parity-08-save-compatibility-v1` (create from `185200f5`; do not reuse the stale v23 branch blindly)
- Worktree: `/media/jer/Videos/CCF-Parity08`

## Hard boundaries (unchanged)

No parity-specific save field or schema bump. No protected anchor rebaseline. No validator/fixture weakening. Preserve Legacy40 and frozen Reference Future. Unknown species rejection before world mutation. No MFM-08/S1-D/MFM-09/UI ownership. No Windows build.

Success token remains:

`SPECIES PARITY COMPATIBILITY PASS`

No Windows build.
