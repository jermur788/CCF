# Natural Old-Age Tree Death and Smooth Late-Life Growth V1 — Bounded Tasks

- **Spec ID** `SPEC-multi-2d-senescence-late-growth-v1`
- **Spec status** APPROVED / IMPLEMENTING (Part 1); Part 2 DEFERRED
- **Task list owner** Overall Manager (Playable Milestone 8 Part 1 packet, 2026-11-28)
- **Last updated** 2026-11-28

| Task | Scope | Completion evidence |
| --- | --- | --- |
| T-01 | Author the four-file spec quartet (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) with Track A provenance/evidence table and explicit Part 2 deferrals | `specs/multi-2d-senescence-late-growth-v1/*` |
| T-02 | Implement immutable contracts, version gate (`Disabled` default / `OldAgeV1`), stream prefix, biological-death record, and plain player wording | Console cases `GATE-01`..`GATE-06`, `ROLL-06`, `ROLL-07`, `ROLL-09` |
| T-03 | Implement six-species smoothstep old-age hazard profiles, pure `AnnualHazard` function, and survival helper | Console cases `HAZ-01`..`HAZ-10` |
| T-04 | Implement the stateless collision-hardened keyed roll (64-bit FNV-1a key hash + SplitMix64), validation guards, order independence, and residual cohort behaviour | Console cases `ROLL-01`..`ROLL-05`, `ROLL-08`, `ROLL-10`, `DET-02` (collision-hardened roll, named 32-bit collision regression) |
| T-05 | Implement Wave 2 smooth late-life DBH growth continuation (`scots-pine`, `common-alder`, `downy-birch`) with exact in-window delegation and refusals for Sitka/Oak/Beech | Console cases `GROW-01`..`GROW-13` |
| T-06 | Build and run the standalone offline console gate under pinned SDK 8.0.133, confirm byte-identical cross-process output, pin the canonical SHA-256 digest, verify source hygiene and the write fence, and publish Part 1 handoff | Console cases `DET-01`, `HYG-01` (added: module sources pass the existing NaturalProcesses JW-T63 banned list and G6 tokens), `IMPLEMENTATION-HANDOFF-PART1.md`, `git diff --name-only` receipts |
| T-07 | **DEFERRED (Part 2):** wire into `AnnualPipeline.cs`, register senescence salt(s) in `RngSalts.cs`, live Wave 2 late-life growth wiring, Version 3 six-species deadwood save validation (Version 2 Phase1 untouched), History/Habitat UI | **DEFERRED** — requires a separate approved Part 2 packet and integrated acceptance gates |

T-01 precedes source implementation. T-02 through T-05 feed T-06. T-07 is explicitly deferred out of this branch.

## Stop conditions

Stop and return to the Manager if any task requires editing a file outside the four authorised prefixes, modifying `RngSalts.cs` / `AnnualPipeline.cs` / existing Wave 2 or NaturalProcesses files / saves / Unity assets / `Docs/Project/**`, adding a dependency, or making an unapproved Part 2 wiring decision.
