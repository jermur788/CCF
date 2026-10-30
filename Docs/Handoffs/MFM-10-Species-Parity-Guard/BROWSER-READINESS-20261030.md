# MFM-10 — Species-Parity Integration Guard — Browser Readiness

**Date:** 2026-10-30  
**Browser claim:** #181  
**Parent:** #91 / browser lane #162  
**Authority:** D-065; approved `specs/multiscale-forestry-management-v1/` MFM-10  
**Status:** PREPARED — NOT RELEASED FOR LOCAL EXECUTION

## Purpose

Prepare the MFM-10 qualification so it can start immediately after MFM-09 is Manager-approved and integrated. This browser preparation grants no production ownership and makes no runtime/Unity claim.

## Release gate

Do not create or release the substantive local MFM-10 claim until all of the following are true:

1. #151 S1-D T-05 has completed Unity qualification, final Manager review and safe integration.
2. #156 MFM-08 has completed Tier-2 Unity qualification, final Manager review and safe integration.
3. MFM-09 `Extend your treatment` has returned `SCENARIO ONE EXTENSION FLOW QUALIFIED`, passed Manager review and been integrated.
4. The then-current exact integration SHA has been refreshed and pinned.
5. `Docs/Project/coordination/active-tasks.md` has been refreshed and no conflicting writer owns the verification or species-rule surfaces.

The current pre-MFM-09 integration lineage is preparation context only and must not be used as the eventual MFM-10 candidate pin.

## Required guard

The approved task requires proof that:

- accepted Sitka prescription/proposal behaviour remains unchanged unless a separately accepted revision explicitly says otherwise;
- Oak and Beech are never processed through Sitka-only quantitative candidate/release/proposal rules merely because their species IDs are present in the same stand;
- Oak/Beech exclusions and player-readable reasons remain accurate across coverage, candidate, proposal and Work Plan-facing flows;
- no code path silently treats broadleaf adults as Sitka equivalents or transfers Sitka competition/release coefficients to them;
- no UI copy claims species-aware quantitative treatment that the implementation does not provide;
- the guard is fail-closed: an unexpected species/rule combination must not create treatment proposals or committed work;
- any desire to extend quantitative treatment to Oak/Beech stops for an accepted spec/rule-version amendment instead of being folded into this qualification.

Exact success token: `SPECIES GUARD QUALIFIED`.

## Minimum qualification design

The released local packet must test one exact integrated candidate and cover, at minimum:

- pure Sitka control: accepted proposal/candidate outputs and deterministic fingerprints remain unchanged on the same authoritative state;
- mixed Sitka/Oak/Beech state: Sitka-eligible stems may follow the accepted Sitka rule while Oak/Beech remain explicitly excluded from quantitative release generation;
- pure Oak and pure Beech negative cases: zero Sitka-calibrated candidates/releases/proposals and clear exclusion reasons;
- broadleaf as local competitor/neighbour: presence must not silently convert the broadleaf stem into a Sitka release target;
- manual marks/work/orders: existing player/manual state continues to win conflicts and cannot be overwritten by the guard;
- prescription provenance: any surviving Sitka-generated Pending work preserves the accepted prescription/version/source/coverage provenance and remains Pending until ordinary Work Plan approval;
- save/load continuation on the accepted schema: species exclusions and valid Sitka behaviour remain deterministic after reload without Reference Future or protected-anchor rebaselining;
- Legacy40 / frozen Reference Future compatibility required by the governing specs remains unchanged;
- relevant MFM-04/05/06/07/08/09 and parity regressions remain green.

If a Unity/runtime path is needed to prove the integrated species guard, it must use the machine-wide lock and exact candidate. Static evidence alone must not be relabelled as Unity verification.

## Production-change policy

MFM-10 is **qualification-only by default**. A genuine defect must stop the run, publish the failing witness, and return for a separate bounded repair assignment. Do not opportunistically edit species rules under the qualification claim.

## Relationship to PARITY-09

The authoritative PARITY-09 task packet explicitly requires MFM-10 `SPECIES GUARD QUALIFIED` before PARITY-09 starts and before its substantive local claim is created. Therefore:

`MFM-09 accepted/integrated → MFM-10 SPECIES GUARD QUALIFIED → fresh PARITY-09 browser repin → new PARITY-09 local qualification claim`.

MFM-11 and MFM-12 remain later required MFM V1 gates for the full player-test candidate, but they are not substituted for this explicit PARITY-09 start gate.

## Handoff requirements on return

Publish `Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md` with:

- exact candidate SHA and ancestry;
- branch/worktree/starting HEAD and claim status;
- changed-file list (expected verification/docs only unless a separately assigned repair occurs);
- exact commands and pass/fail counts;
- deterministic pure-Sitka comparison evidence;
- Oak/Beech negative/exclusion evidence;
- save/load/reference/protected-anchor evidence where applicable;
- Unity lock/cleanup evidence for any Unity phase;
- explicit `SPECIES GUARD QUALIFIED` or precise blocker.

No Windows build.
