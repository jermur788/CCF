# Downy Birch and Common Alder V1 Bounded Tasks

- **Spec ID** `SPEC-multi-2d-birch-alder-v1`
- **Spec status** APPROVED / IMPLEMENTING
- **Task list owner** Overall Manager (implementation packet 2026-11-27)
- **Last updated** 2026-11-27

| Task | Scope | Completion evidence |
| --- | --- | --- |
| T-01 | Stamp the approved Option A spec quartet and the scope fence | `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` |
| T-02 | Implement species access, result records and the Wet Valley & Slope two-zone helper | Console cases SITE-01 and ZONE-01/02 |
| T-03 | Implement Downy birch growth and local-gap regeneration | Console cases BIRCH-01, BIRCH-GROWTH-01/02, BIRCH-REGEN-01/02 |
| T-04 | Implement Common alder growth, local-gap regeneration and heart-rot inspection | Console cases ALDER-01, ALDER-GROWTH-01/02, ALDER-REGEN-01/02, ROT-01/02 |
| T-05 | Implement bulk grading, density, sapling cost, quality-stem flag and alder downgrade | Console cases TIMBER-01/02/03 and ROT-03 |
| T-06 | Run the pinned standalone console gate, repeat it for the determinism digest and check the diff fence | Console case DET-01 plus handoff command receipts |
| T-07 | Wire `AnnualPipeline`, `MultiAreaContinuation`, `ForestMaker`, Version 3 saves and `/multi`, Downy birch first and then Common alder | **QUEUED** behind Playable Milestone 1 and Scots Pine Part 2; separate packet and integrated acceptance |

T-01 precedes source work. T-02 through T-05 feed T-06. T-07 requires a new implementation packet and the relevant review gates; it is not part of this branch.

## Stop conditions

Any task that needs a file outside the four authorised prefixes, a change to an existing Wave 2 file or RNG salt, a new dependency, or a Part 2 wiring decision stops and returns to the Manager.
