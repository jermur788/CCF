Routing: REGULAR CLAUDE VALUE
Role: Independent G4 Reviewer
Context: c6dc136b7f1f8668d9f67f95f48968d290ad5067
Base: c6dc136b7f1f8668d9f67f95f48968d290ad5067
Branch: ops/workflow-foundation-v1
Review target HEAD: Use the exact full SHA in /media/jer/ZX20/CCF-Ops/WorkflowFoundationHandoff.md; do not review a moving branch.
Scope: Independently review foundation implementation, deployment provenance, storage placement, offline evidence, lock behaviour and authority-transition proposal.
Exclusions: No merge, canonical writes, Unity launch, cleanup/migration, account configuration, new worker dispatch, production content or threshold changes.
Ownership: Read-only review of CCF-Ops and Docs/Operations plus raw receipts; no implementation ownership.
Verification tier: Tier 1
Time budget: 15 minutes; report unresolved criteria rather than expanding to Unity qualification.
Acceptance criteria:
- Every new lane, Git store, scratch/build/verification/ops path resolves to ZX20 UUID 18D6-896E; old live work is preserved.
- Four persistent lanes use distinct branches; cold Libraries are declared honestly; no Library copy or Unity run is claimed.
- lane-status is read-only, handles missing/clean/dirty lanes and config errors; packet-lint distinguishes PASS/WARN/FAIL mechanically.
- Unity ownership is machine-wide across clones, acquisition atomic, contention rejected, stale records inspected, force-release audited; no process is killed.
- Board/ledger fields and persistent role limits match the Manager packet; simulated pilot is not production acceptance.
- Exact changed files are operations only; Assets/Packages/ProjectSettings/scenes/prefabs and canonical context are unchanged; diff check and offline receipts support claims.
- active-tasks remains authoritative; later G4-reviewed canonical update is separately scoped; no autonomous T3 orchestration or credential handling is introduced.
Stop conditions: Missing/contradictory raw evidence, wrong exact HEAD, ownership conflict, storage mismatch or any need to relax requirements/change canon. Return NEEDS CHANGES or BLOCKED with exact criterion.
Expected handoff: Exact reviewed HEAD/context; APPROVED / NEEDS CHANGES / REJECTED per criterion; raw evidence paths; limitations and bounded follow-up packet recommendation. Do not merge or self-assign product acceptance.
Unity required: No
