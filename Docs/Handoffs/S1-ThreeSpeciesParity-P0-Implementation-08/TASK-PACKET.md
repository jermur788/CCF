# PARITY-08 — Save / Deterministic Compatibility

**Parent:** #91  
**Spec:** `specs/scenario-one-three-species-parity-p0/` PARITY-08  
**Status:** **PREPARED / WAIT FOR MFM-07 RETURN REVIEW — DO NOT DISPATCH YET**  
**Prepared by:** #122 browser lane / Overall Manager

## Readiness verdict

**WAIT.** The former MFM-06 dependency is satisfied: exact qualified production base is

`b971f4da0540e606bfee9702b0c99f81a0a89355`

with save schema v23 and protected hash/Reference compatibility qualified.

The only remaining release gate is MFM-07 #121's actual published diff/handoff. MFM-07 is explicitly **not authorised** to bump schema or edit save validation, so PARITY-08 is expected to become executable immediately if #121 returns within scope. Do not create/release the production claim until the Manager confirms the returned MFM-07 candidate did not unexpectedly take save/validator/hash ownership.

If MFM-07 changes any of `ForestSaveData`, `ForestSaveValidation`, `ForestSaveController`, save migration, reference/hash layout, or materially changes species-bearing persisted records, reconcile that change first and keep PARITY-08 waiting.

## Purpose

Prove species-parity save/model gating and reject unknown saved species safely on the current v23 lineage, without adding any parity-specific persisted state or moving protected deterministic/reference anchors.

## Difficulty and routing

- **Difficulty:** **5/5** — high-risk shared save validation, deterministic anchors and historical compatibility.
- **Preferred executor:** **Claude Opus 5.5 Low / Anthropic** for the save/reference slice, or the strongest equivalent local Code lane if availability differs.
- **Mandatory independent reviewer:** independent **OpenAI Sol/Codex** reviewer, cross-provider, before PARITY-09.

Suggested lane after release:

- branch `verify/parity-08-save-compatibility-v1`
- worktree `/media/jer/Videos/CCF-Parity08`

## Exact prerequisites at dispatch

All must be true:

1. MFM-06 remains integrated/qualified at `b971f4d...` or an explicitly Manager-approved successor.
2. MFM-07 #121 has a GitHub-published handoff and exact candidate SHA.
3. Browser/Manager review of MFM-07 is `APPROVED`.
4. MFM-07 diff confirms no unauthorised schema/save-validator/hash/reference change; otherwise this packet is repaired before dispatch.
5. Live `active-tasks.md` grants PARITY-08 the single-writer save/validation/determinism surface.
6. No other live task owns save validation or protected hash/reference fixtures.

At actual start, refresh #91/#106/#121/#122 plus the new PARITY-08 claim, current integration ref and live coordination; record actual model/provider, branch/worktree and starting SHA before edits.

## Likely owned surface while RUNNING

Exact files must be confirmed against the post-MFM-07 candidate. Expected primary surfaces:

- `Assets/ForestPrototype/ForestSaveValidation.cs` and any existing split validation partials used by species checks;
- `Assets/ForestPrototype/ForestSaveController.cs` only if required to preserve `CanLoad`/`LoadData` pre-mutation parity;
- existing species-bearing save DTOs only as read inputs — **no new field/schema change**;
- deterministic/reference compatibility helpers only where needed to prove existing layout behaviour, never to rebaseline it;
- focused verification under existing save-hardening/three-species/reference/hash suites.

Treat `ScenarioReferenceArchive.cs`, protected hash comparators and pin files as high-risk review surfaces. Do not change expected hashes/fixtures to make a failure pass.

## Required scope

Verify and, only where a demonstrated conformance defect exists, repair:

- unknown saved species IDs are rejected **before world mutation** and never resolve/fall back to Sitka;
- every persisted species-bearing location relevant to Scenario One P0 is covered consistently, including living tree records and regeneration/juvenile/history/order records where species is authoritative;
- `CanLoad` and `LoadData` reject/accept consistently;
- growth models 0/1 preserve historical behaviour;
- growth model 2 preserves accepted three-species parity behaviour;
- current MFM v23 save/provenance fields coexist without parity-specific reinterpretation;
- pre-v23/pre-parity saves retain accepted semantics;
- Legacy40 remains exact under its original configuration;
- frozen Reference Future v1 archive bytes/integrity remain exact;
- protected pure-Sitka lifecycle/density and Sitka timber anchors remain exact;
- species-less timber-sale records remain accepted under current P0 pricing assumptions;
- no parity-specific save field or schema bump is introduced.

## Required verification

- negative unknown-species fixtures for every authoritative species-bearing save path, with explicit no-mutation observation;
- positive model-0/model-1/model-2 fixtures;
- current v23 MFM save/provenance round-trip;
- genuine pre-v23/pre-parity fixtures;
- Legacy40 and frozen Reference gates;
- protected deterministic anchor/fingerprint comparisons;
- `CanLoad`/`LoadData` parity;
- exact key-set/hash evidence with no hidden exclusions or comparator weakening;
- Unity import/compile/save/load qualification on the exact candidate under the machine-wide lock;
- independent review of source, fixtures, mutation ordering and hash/reference preservation.

## Stop conditions

STOP and return to the Manager if:

- a legitimate shipped/accepted historical fixture contains an unknown species ID that current authority requires loading;
- a compatibility repair would require a parity-specific schema field/version bump;
- a protected Sitka/Legacy/Reference anchor would move;
- MFM-07 unexpectedly owns/changes the same save/validation/hash surface and has not been reconciled;
- a broadleaf-containing anchor moves materially without attributable accepted behaviour and explicit Manager review;
- the only apparent fix is to weaken a validator, fixture, deterministic comparison or protected hash.

## Handoff / review

After release, the implementation claim will be named in this packet and issue body. Publish:

- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/HANDOFF.md`
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW.md`

The implementation handoff must identify exact base/end SHA, changed files, species-bearing fixture inventory, mutation/no-mutation evidence, commands/hashes and residuals. Independent review verdict: `APPROVED / NEEDS CHANGES / REJECTED`.

Success token:

`SPECIES PARITY COMPATIBILITY PASS`

## Copy/paste dispatch line — use only after Manager release

`Run PARITY-08 from Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/TASK-PACKET.md only after #121 MFM-07 is Manager-APPROVED and its exact published diff confirms no unreconciled save/validator/hash ownership. Root from the Manager-approved post-MFM-07 candidate, use Claude Opus 5.5 Low (or strongest equivalent local Code lane) for this 5/5 save/determinism slice and an independent OpenAI Sol/Codex reviewer. Prove pre-mutation unknown-species rejection, model-0/1/2 compatibility, v23 MFM coexistence, Legacy40, frozen Reference and protected anchors with no parity schema bump or rebaseline. No Windows build.`

**User action required now:** none. Browser #122 will release the claim immediately after #121 returns APPROVED and the save ownership gate is clear.
