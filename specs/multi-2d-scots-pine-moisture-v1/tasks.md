# Scots Pine and Site Moisture V1 Bounded Tasks

| Task | Scope | Completion evidence |
| --- | --- | --- |
| T-01 | Stamp the approved Option A spec quartet and scope fence | `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` |
| T-02 | Implement weighted site selection, table-based modifiers, two-zone map and bog warning | Console cases MOISTURE-01 through MOISTURE-04 |
| T-03 | Implement Scots pine adult growth and local gap regeneration | Console cases PINE-01, GROWTH-01/02 and REGEN-01/02 |
| T-04 | Implement Scots pine grading, price and material cost | Console cases TIMBER-01 and TIMBER-02 |
| T-05 | Run pinned standalone console gate and verify same-output SHA-256 across processes | Console case DET-01 and handoff command receipts |
| T-06 | Check the diff fence, commit and push the implementation handoff | Four feature-branch commits and remote verification |
| T-07 | Wire AnnualPipeline, MultiAreaContinuation, ForestMaker, Version 3 saves and `/multi` | **QUEUED behind Playable Milestone 1**, separate scope and integrated acceptance |

T-01 precedes source work. T-02 through T-04 feed T-05 and T-06. T-07 requires a new implementation packet after the milestone and the relevant review gates.
