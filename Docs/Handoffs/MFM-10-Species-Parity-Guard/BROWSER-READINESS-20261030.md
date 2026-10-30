# MFM-10 — Species-Parity Integration Guard — Browser Readiness

**Date:** 2026-10-30  
**Browser claim:** #181  
**Parent:** #91 / browser lane #162  
**Authority:** D-065; approved `specs/multiscale-forestry-management-v1/` MFM-10  
**Status:** **PREPARED — NOT RELEASED FOR LOCAL EXECUTION**

## Purpose

Prepare the MFM-10 qualification so it can start immediately after MFM-09 is Manager-approved and integrated. This browser preparation grants no production ownership and makes no runtime/Unity claim.

## Current dependency reconciliation

Already satisfied:

- #151 S1-D T-05 completed Unity qualification, Manager review and safe integration. Exact production integration merge `46fc6d55315bbddcdfa7667f0da490378d584023`; current Scenario One integration/docs successor `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.
- PARITY-03 through PARITY-08 are durably accepted/integrated on the published lineage. PARITY-08 exact integration merge `7d2049e862df5c9311748167423b981e713d1fa9` / docs successor `8fa83bff22ea1b4943689bee1074eed8eae65d2f`, now an ancestor of `ce673714...`.

Still required before MFM-10 release:

1. #156 MFM-08 completes Tier-2 Unity qualification, final Manager review and safe integration.
2. MFM-09 `Extend your treatment` returns `SCENARIO ONE EXTENSION FLOW QUALIFIED`, receives Manager `APPROVED` and is integrated.
3. The then-current exact post-MFM-09 integration SHA is refreshed and pinned.
4. `Docs/Project/coordination/active-tasks.md` is refreshed and no conflicting writer owns verification/species-rule surfaces.
5. A substantive MFM-10 local claim is created/marked `[RUNNING]` only at actual execution start.

The current `ce673714...` lineage is preparation context only, not the eventual MFM-10 test candidate.

## Required guard

Prove that:

- accepted Sitka prescription/proposal behaviour remains unchanged unless a separately accepted revision explicitly says otherwise;
- Oak and Beech are never processed through Sitka-only quantitative candidate/release/proposal rules merely because they exist in the same stand;
- Oak/Beech exclusions and player-readable reasons remain accurate across coverage, candidate, proposal and Work Plan-facing flows;
- no code path silently treats broadleaf adults as Sitka equivalents or transfers Sitka competition/release coefficients to them;
- no UI copy claims species-aware quantitative treatment that is not implemented;
- the guard is fail-closed: unexpected species/rule combinations must not create treatment proposals or committed work;
- any desire to extend quantitative treatment to Oak/Beech stops for accepted spec/rule-version amendment rather than silent generalisation.

Exact success token: `SPECIES GUARD QUALIFIED`.

## Minimum qualification design

The released local packet must test one exact integrated candidate and cover at minimum:

- pure Sitka control: accepted proposal/candidate outputs and deterministic fingerprints remain unchanged on the same authoritative state;
- mixed Sitka/Oak/Beech state: Sitka-eligible stems may follow accepted Sitka rules while Oak/Beech remain explicitly excluded from quantitative release generation;
- pure Oak and pure Beech negative cases: zero Sitka-calibrated candidates/releases/proposals and clear exclusion reasons;
- broadleaf as local competitor/neighbour: presence must not silently convert the broadleaf stem into a Sitka release target;
- manual marks/work/orders: existing player/manual state continues to win conflicts and cannot be overwritten by the guard;
- prescription provenance: any surviving Sitka-generated Pending work preserves accepted prescription/version/source/coverage provenance and remains Pending until ordinary Work Plan approval;
- save/load continuation on the accepted schema: exclusions and valid Sitka behaviour remain deterministic after reload without Reference Future/protected-anchor rebaseline;
- Legacy40 / frozen Reference Future compatibility required by governing specs remains unchanged;
- relevant MFM-04/05/06/07/08/09 and parity regressions remain green.

If Unity/runtime is needed, use the machine-wide lock and exact candidate. Static evidence must not be relabelled as Unity verification.

## Production-change policy

MFM-10 is **qualification-only by default**. A genuine defect stops the run, publishes the failing witness and returns for a separate bounded repair assignment. Do not opportunistically edit species rules under the qualification claim.

## Relationship to PARITY-09

The authoritative PARITY-09 packet requires MFM-10 `SPECIES GUARD QUALIFIED` **before PARITY-09 starts and before its substantive local claim is created**.

Therefore the mandatory chain is:

`#156 accepted/integrated → MFM-09 accepted/integrated → MFM-10 SPECIES GUARD QUALIFIED → fresh PARITY-09 browser repin → new PARITY-09 local qualification claim`.

MFM-11 and MFM-12 remain later required MFM V1 gates for the full player-test candidate, but they do not substitute for this explicit PARITY-09 start gate.

## Handoff requirements on return

Publish `Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md` with:

- exact candidate SHA and ancestry;
- branch/worktree/starting HEAD and claim status;
- changed-file list (expected verification/docs only unless separately assigned repair occurs);
- exact commands and pass/fail counts;
- deterministic pure-Sitka comparison evidence;
- Oak/Beech negative/exclusion evidence;
- save/load/reference/protected-anchor evidence where applicable;
- Unity lock/cleanup evidence for any Unity phase;
- explicit `SPECIES GUARD QUALIFIED` or precise blocker.

No Windows build.
