# PARITY-08 — Save / Deterministic Compatibility

**Parent:** #91  
**Spec:** `specs/scenario-one-three-species-parity-p0/` PARITY-08  
**Status:** PREPARED / NOT DISPATCHED  
**Purpose:** prove species-parity save/model gating and reject unknown saved species safely on the current post-MFM-06 save lineage, without adding any parity-specific persisted state.

## Difficulty and routing

- **Difficulty:** High — shared save validation, deterministic anchors and historical compatibility.
- **Preferred executor:** Claude Opus 5.5 Low / Anthropic because this is a high-risk save/reference slice.
- **Mandatory independent reviewer:** OpenAI 6.1 Sol, cross-provider.

## Start gate / base

Do not start while MFM-06 owns save/validation. Start only after MFM-06 is accepted/integrated and its exact GitHub-resolvable integration SHA is published. Refresh #91/#106/active-tasks and record that exact base before edits.

Suggested branch/worktree: `verify/parity-08-save-compatibility-v1` / `/media/jer/Videos/CCF-Parity08`.

Create implementation/review claims only when this gate is satisfied.

## Required scope

Verify and, only where a demonstrated conformance defect exists, repair:

- unknown saved species IDs are rejected before world mutation and never resolve to Sitka;
- `CanLoad` and `LoadData` reject/accept consistently;
- growth models 0/1 preserve historical behaviour;
- growth model 2 preserves accepted parity behaviour;
- current MFM v23 save fields coexist without parity-specific reinterpretation;
- pre-v23/pre-parity saves retain their accepted semantics;
- Legacy40 remains exact under its original configuration;
- frozen Reference Future v1 archive bytes/integrity remain exact;
- protected pure-Sitka lifecycle/density and Sitka timber anchors remain exact;
- species-less timber-sale records remain accepted under the current P0 pricing assumptions;
- no parity-specific save field or schema bump is introduced.

## Stop conditions

STOP and return to the Manager if:

- a legitimate shipped/accepted historical fixture contains an unknown species ID that current authority requires loading;
- a compatibility repair would require a parity schema field/version bump;
- a protected Sitka/Legacy/Reference anchor would move;
- a broadleaf-containing anchor moves materially without attributable accepted behaviour and explicit Manager review.

## Verification

- negative unknown-species fixtures with pre-mutation/no-mutation observation;
- positive model-0/model-1/model-2 save fixtures;
- current v23 round-trip after MFM-06;
- Legacy40 and frozen Reference gates;
- deterministic anchor/fingerprint comparisons;
- CanLoad/LoadData parity;
- exact key-set/hash evidence with no hidden exclusions or silent comparator weakening;
- Unity import/compile/save/load qualification on the exact candidate under the machine-wide lock.

No Windows build.

## Handoff / review

Publish `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/HANDOFF.md` with exact base/end SHA, changed files, fixture inventory, commands, hashes and residuals.

Independent OpenAI 6.1 Sol review must publish `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW.md` with `APPROVED / NEEDS CHANGES / REJECTED` before PARITY-09.

Success token: `SPECIES PARITY COMPATIBILITY PASS`

## Copy/paste dispatch line

`Run PARITY-08 from Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/TASK-PACKET.md only after MFM-06 is accepted and its exact integration SHA is published. Use Claude Opus 5.5 Low for the high-risk save/determinism task and OpenAI 6.1 Sol for independent cross-provider review. Prove unknown-species pre-mutation rejection, model-0/1/2 compatibility, current v23 coexistence, Legacy40, frozen Reference and protected anchors with no parity schema bump. Stop on any legitimate historical-fixture conflict. No Windows build.`

**User action required now:** none; blocked on MFM-06 acceptance/publication.
