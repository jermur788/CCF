# PARITY-08 Release Amendment — 2026-10-29

Parent: #91 PLAYER-TEST-OVERNIGHT-01  
Claim: #126  
Status: RELEASED FOR DISPATCH / NOT YET RUNNING

This amendment supersedes the `WAIT FOR MFM-07 RETURN REVIEW` status at the top of `TASK-PACKET.md`. The rest of that packet remains authoritative unless explicitly changed below.

## Release basis

MFM-07 #121 is Manager-APPROVED on exact qualified production candidate:

`d4188b33e58b0fe67504a87167692d01b226c84e`

Its exact base is MFM-06-qualified v23:

`b971f4da0540e606bfee9702b0c99f81a0a89355`

Published MFM-07 source review confirms the production delta adds only:

- `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescriptionApproval.cs` (+ meta)
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs` (+ meta)

MFM-07 does not modify `ForestSaveData`, `ForestSaveValidation`, `ForestSaveController`, save migration, hash/reference layout, or species-bearing persisted DTOs. The save/validation/determinism ownership gate that previously blocked PARITY-08 is therefore clear.

## Exact task base

Root PARITY-08 production work from:

`d4188b33e58b0fe67504a87167692d01b226c84e`

Do not root from the docs-only MFM-07 successor unless you explicitly prove no production/verification change beyond the qualified candidate.

## Claim / routing

Use claim #126.

- Difficulty: **5/5**.
- Preferred executor: **Claude Opus 5.5 Low / Anthropic** for the high-risk save/reference/determinism slice.
- Mandatory independent reviewer before PARITY-09: **OpenAI 6.1 Sol/Codex**, preserving cross-provider independence.
- Suggested branch: `verify/parity-08-save-compatibility-v1`.
- Suggested worktree: `/media/jer/Videos/CCF-Parity08`.

PARITY-08 may perform offline work in parallel with MFM-07 integration #125 because MFM-07 owns no save/validator/hash/reference production surface. Heavy Unity work remains serialized under the machine-wide lock.

## Unchanged hard boundaries

No parity-specific save field or schema bump. No protected anchor rebaseline. No validator/fixture weakening. Preserve Legacy40 and frozen Reference Future. Unknown species rejection must occur before world mutation. No Windows build.

Success token remains:

`SPECIES PARITY COMPATIBILITY PASS`
