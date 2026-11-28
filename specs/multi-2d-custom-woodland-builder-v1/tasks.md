# Full Custom Woodland Builder V1 Bounded Tasks

- **Spec ID** `SPEC-multi-2d-custom-woodland-builder-v1`
- **Spec status** APPROVED / IMPLEMENTING (Part 1)
- **Task list owner** Overall Manager (implementation packet 2026-11-28)
- **Last updated** 2026-11-28

| Task | Scope | Completion evidence |
| --- | --- | --- |
| T-01 | Stamp the approved Option A spec quartet and the scope fence | `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` |
| T-02 | Implement the polygon layer and the four boundary templates with validated corner chamfers | Console cases BOUND-01 to BOUND-09 |
| T-03 | Implement exact parallel lane clipping and sliver refusal | Console cases LANE-01 to LANE-06 |
| T-04 | Implement the 2.5 m moisture grid, four patterns and per-cell paint overrides | Console cases MOIST-01 to MOIST-09 |
| T-05 | Implement the four deer browsing presets with `[E]` provenance | Console cases DEER-01 to DEER-03 |
| T-06 | Implement the six-species mix allocator and the deterministic stand generator | Console cases MIX-01 to MIX-04, GEN-01 to GEN-06 |
| T-07 | Implement recipe validation, the fail-closed living-tree cap and the canonical build result | Console cases CAP-01/02, DET-01/02 |
| T-08 | Run the pinned standalone console gate, repeat it for the determinism digest and check the diff fence | Console case DET-01 plus handoff command receipts |
| T-09 | Wire the New Woodland picker, `ForestMaker`, `AnnualPipeline`, Version 3 `genesisRecipe` and `/multi` | **QUEUED** behind the Milestones 2–5 Part 2 wiring wave; separate packet and integrated acceptance |

T-01 precedes source work. T-02 through T-07 feed T-08. T-09 requires a new implementation packet and the relevant review gates; it is not part of this branch.

## Stop conditions

Any task that needs a file outside the four authorised prefixes, a change to an existing Wave 2 file or RNG salt, an edit to `ForestMaker`/`Saves`/`src/Ui`, a new dependency, or a Part 2 wiring decision stops and returns to the Manager.
