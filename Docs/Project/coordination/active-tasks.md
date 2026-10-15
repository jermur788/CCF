# Active Task Coordination

Live coordination, **outside the project context commit**. Workers read this file at task start; packets carry locks for workers without live Git access. Lock changes do not advance context.

Temporary authoritative physical record designated by Overall Manager/user for this publication:
`ops/workflow-foundation-v1:Docs/Project/coordination/active-tasks.md`.
Only Overall Manager/assigned integrator or the explicitly assigned T1 editor maintains these locks. This designation names one physical record; it does not promote the CCF-Ops board. Future designation is a live coordination change, not a permanent branch requirement.

## Live tasks / temporary locks

HEAD pins identify the execution/reviewed implementation snapshot, not the later commit containing this live record. READY is readiness for the stated next action, never integration, verification or acceptance. NONE means no assigned worker; no branch/worktree presence grants ownership.

| Task ID | State | Owner | Harness | Model | Quota source | Lane | Branch | HEAD | Subsystem | Locks | Unity required? | Unity queue position | Verification tier | Open gate | Dependency | Next action |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CCF-CANONICAL-WORKFLOW | ACTIVE | Canonical workflow editor (current user packet) | Codex | GPT-6.1 Sol | OpenAI allowance | NonUnity | ops/workflow-foundation-v1 | 3614783e1a9fb7a4b4d934dca62c94c4555f19c1 | Canonical workflow / live coordination | AgentWorkflow; active-tasks; dependent routing/review wording; generated publication only | No | None | Tier 1 | G4 implementation of reviewed E1-E10 | Manager T1 designation and live ownership ruling | Refresh T1 then apply E1-E10 and publish one stamped context |
| CCF-DENSITY-INTEGRATION-INPUT | READY | NONE | Unassigned | Unassigned | Unassigned | None | fix/density-v3-canopy-pulses-v1 | 271193c786e256e4f55017d34d07209eccb6af3c | Accepted density integration input | No active worker; preserve historical dirty checkout | No | None | Tier 1 | G4 integration | Accepted production 639ae2c3ac85474368c173118bd2718743346bd9; tested oracle hashes matched pushed repair | DONE publication / READY-FOR-INTEGRATION; Manager assigns integration owner |
| CCF-CANDIDATE-V2 | READY | NONE | Unassigned | Unassigned | Unassigned | None | integration/scenario-one-final-candidate-v2 | c20c6dc242e4463b3de72e6c2d06b4ea9fa1fbc2 | Shared candidate provenance / integration | No active worker; no acceptance or new Unity execution implied | No | None | Tier 1 | G4 candidate review and handoff | Density repair published; candidate contains accepted density and exact F10 input | Manager obtains candidate handoff and assigns bounded G4; candidate not accepted/verified by existence |

## Unity ownership

IDLE; OWNER NONE. Production unity-lock status FREE, metadata null, no observed Editor PIDs at reconciliation. Do not acquire until a real Unity packet is assigned. No Unity qualification was rerun.

## Reconciliation and preservation holds

Manager ruling: no current density/integration worker ownership is established from conversation state. Historical packets, dirty files, checkouts and evidence branches grant none.

- `task/mixed-density-architecture-v3` @ `9c736dc52717302b39faf61c75da958c81d8c1af`: INACTIVE / REFERENCE, OWNER NONE. No new production assignment here.
- Historical density verification branches, including `verify/final-density-a395e3f` @ `4c00dd89b5e9fdd4ab34cd406d698d8a082d84e7` and `verify/final-density-v2-a7a759d` @ `81126ee093b0db345112af17bcb2842a95fe1009`: INACTIVE / EVIDENCE, OWNER NONE. Preserve refs/evidence; not live task locks.
- Density repair publication is resolved: origin repair HEAD is `271193c786e256e4f55017d34d07209eccb6af3c`; committed harness/probe/runner hashes match the tested receipt. The historical receipt's uncommitted wording describes its original run, not current publication.
- Candidate-v2 unexpectedly exists remotely at `c20c6dc242e4463b3de72e6c2d06b4ea9fa1fbc2`. It descends from accepted production `639ae2c3ac85474368c173118bd2718743346bd9` and repair `271193c786e256e4f55017d34d07209eccb6af3c`; its F10 changed files equal triage input `5d69b7bf58eb961bad3127432e5372b1d2bdb9f9`. There is no production delta after the accepted density commit. This establishes source prerequisites only: Manager creation handoff/G4 and remaining runtime/native-input gates are not inferred complete.
- Last tested density checkout `/media/jer/Files/CCF-a1-density-e5cfce3` is detached at candidate HEAD with unrelated tracked settings/material changes and untracked ClearanceVerification source/meta. No current owner is assigned; preserve, do not clean, reset or dispatch into it. It is not a writable lane assignment in the table.
- Existing dirty/local-unique legacy estate remains preserved, with no new ownership claims or closure inferred. This includes `/home/jer/CCF`, CCF-main, CCF-claude, CCF-p2-bimodal, CCF-openai; legacy density/profiling/understorey/integration checkouts and unique verification/research branches. Full per-path status/HEAD/store evidence is linked below. No preservation hold authorises work, deletion or migration.
- Old completed coordination rows are removed as stale live-table noise; their Git history remains intact. The generic unassigned local setup row is superseded by the observed persistent-lane foundation, not a claim that every historical task was accepted. The old standing G4-assignment-pending board row is reconciled to this explicitly assigned reviewed-corrections publication task, without inventing a reviewer verdict.

Evidence (2026-10-15): `/media/jer/ZX20/CCF-Verification/CanonicalWorkflowPublication-20261015T221131Z/preflight.json` and `ownership-reconciliation/repository-checks.json`. These are local receipts, not a cloud dependency; exact live task locks above travel in packets. Designated storage/device/capacity settings remain CCF-Ops machine configuration.
