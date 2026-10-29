# Scenario One Player-Test Readiness — Cross-Feature Ownership and Sequencing

**Parent:** #91  
**Browser lane:** #118  
**Date:** 2026-10-29  
**Status:** Manager sequencing disposition; no production ownership granted

## Purpose

Prevent remaining S1-C, S1-D, three-species parity and Multi-Scale Forestry Management work from creating parallel writers on the same save, teaching and Work Plan surfaces.

This document does not change product requirements. It orders already-approved work so each implementation task sees one stable API/base.

## Publication boundary

At this disposition, GitHub publishes `integration/scenario-one-player-test-ready-v1` at `aca74acacc565f72ab802177cf69ca6ebb54d637`. Parent #91 reports a later `ac876a64969752a3864034521f42559671264b30` umbrella candidate as clean/local/unpushed. The latter is local execution evidence only until published.

## Single-writer sequencing

1. **MFM-06 / #111 → #112 → #113** owns the active save/provenance change. No PARITY-08 save-path writer starts in parallel. MFM-06 must publish/integrate first.
2. **PARITY-08** follows accepted MFM-06 integration so unknown-species/old-save/determinism checks exercise the current v23 lineage rather than a superseded v22 save path.
3. **S1-C T-04 / T-06 / T-07** may be implemented after the exact post-MFM-06 base is published and ownership is free. T-04/T-06 share history/review presentation; use one S1-C writer or explicit non-overlapping file ownership. T-07 is contract/verification work and must not create a second identity/history store.
4. **MFM-07 → MFM-08** remains sequential under the approved MFM task order.
5. **S1-D T-04 must land before MFM-09** on teaching-sequence/current-state files, as the S1-D approved task explicitly requires.
6. **S1-D T-05 should land after MFM-07's pending-work/provenance API is stable and before MFM-09's Work Plan teaching integration.** It owns the Approved / Pending new / Pending carried-over presentation and links to the S1-C history surface; MFM-09 must consume that presentation rather than invent a parallel grouping/history UI.
7. **MFM-09** then adds the new stable `Extend your treatment` teaching IDs and explains prescription-generated pending work through the ordinary Work Plan without renaming existing IDs.
8. **MFM-10** follows PARITY-08 and MFM-09. It is the Sitka-only species guard. It must complete before PARITY-09 so the parity qualification can inspect the actual MFM guard and avoid a dependency cycle.
9. **PARITY-09** follows PARITY-08 + MFM-10 and independently qualifies the combined three-species slice.
10. **S1-C T-08** follows S1-C T-04/T-06/T-07 and uses the current integrated save/UI lineage.
11. **S1-D T-08 → T-09** follows S1-D T-04/T-05 plus already accepted T-02/T-03/T-06. T-09 remains an independent review.
12. **MFM-11** calibration-impact qualification follows the completed repeated-intervention/player-facing flow so its long-run completion/economy comparison measures the intended final teaching path rather than an interim one.
13. **MFM-12** integrated MFM verification/review follows MFM-11, PARITY-09 and the relevant S1-C/S1-D feature qualifications.
14. **Final Tier-3** follows all feature-level gates and reruns the S1-C T-05 save/history chain on one exact final combined candidate.

## Shared surfaces

- **Save/load/validation:** MFM-06 → PARITY-08 → final Tier-3. No parallel writers.
- **Teaching sequence/current state:** S1-D T-04 → MFM-09. One primary writer.
- **Work Plan / Annual Review / history:** S1-C T-04/T-06 establish history semantics; MFM-07 establishes proposal→pending-work API; S1-D T-05 establishes previous-vs-proposed grouping; MFM-09 integrates its generated pending work into that existing surface.
- **Species truth:** PARITY-08 → MFM-10 → PARITY-09.
- **Reference/Legacy compatibility:** every save/determinism gate preserves frozen Reference Future v1 and Legacy40; no task may silently rebaseline them.

## Dispatch rule

Packet publication does not grant ownership. Create a child claim and a live `active-tasks.md` row only when the task's start gates are true and the exact current base can be recorded.

No Windows build.
