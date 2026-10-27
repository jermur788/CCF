# CONTEXT-RECONCILE-01 — GitHub Handoff

**Verdict:** `CONTEXT LINEAGES RECONCILED — FINAL CONTEXT READY FOR PUBLICATION`

**Task:** `CONTEXT-RECONCILE-01 — RECONCILE OPS CONTEXT LINEAGE WITH CURRENT MAIN`
**Routing:** deepseek / repository integration
**Role:** Integrator — documentation/context only
**Branch:** `ops/context-reconcile-01`
**Worktree:** `/media/jer/ZX20/CCF-Reconcile`

## Lineage

| Item | SHA / reference |
|---|---|
| Current authoritative `main` base | `b07dd8a627b4fab2db55e96145270170eeee4712` |
| Ops/context source HEAD | `9f06398c068cbaed5b307229241c3640c5646d6d` |
| Common ancestor | `c1564b857972ee18917e8a461c457533a54cc3f1` |
| Context-integration commit | `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` |
| **Final context SHA** | **`eb218eab8b8e200a81cc99bfef7e8906bc21a88d`** |
| Final reconciliation HEAD | `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` (the handoff commit that follows changes no context source) |

Current main's 2026-10-27 workflow authority is preserved from `587c2eb`, `5efcc49`, `fb2f127`, `64da8c7`, `afb70d8` and `b07dd8a`.

## Method

The reconciliation branch was started from current `origin/main` and the required ops material was brought forward by path. `ops/workflow-foundation-v1` was **not** merged wholesale. The pre-existing local branch `ops/workflow-capability-sync-v1` at `716042c` is a wholesale merge of the two lineages; it was neither used nor modified.

## Files brought forward from `9f06398`

### Canonical decisions and context

- `Docs/Project/decision-log.md` — D-063, D-064 and D-065 present; D-065 is dated `2026-10-27`; D-065 preserves the D-010 amendment and the D-064 prerequisite, requires MFM before beginner/external Scenario One testing, and states that MFM does not automatically satisfy S1-D.
- `Docs/Project/current-milestone.md` — D-059–D-065 scope, complete-loop external-demo sequence and MFM pre-external-test requirement.
- `Docs/Project/research-index.md` — CCF book reference, rack/tree-marking extraction, delegated-treatment/sample-to-comparable-area evidence, V1B Sitka evidence roles and the later research context.
- `Docs/Project/unity-project-overview.md` — species-parity audit facts, D-060 supersession of the earlier Stage-A boundary, and the current release boundary.

### Specification framework

- `specs/README.md`
- `specs/_template/spec.md`
- `specs/_template/plan.md`
- `specs/_template/tasks.md`
- `specs/_template/checklists/requirements.md`

### Rack V2 specification

- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md`
- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/plan.md`
- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/tasks.md`
- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/checklists/requirements.md`

### Multi-Scale Forestry Management V1 specification

- `specs/multiscale-forestry-management-v1/spec.md`
- `specs/multiscale-forestry-management-v1/plan.md`
- `specs/multiscale-forestry-management-v1/tasks.md`
- `specs/multiscale-forestry-management-v1/checklists/requirements.md`

### Research records

- `Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md`
- `Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`
- `Docs/Research/RacksAndTreeMarking/CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md`
- `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md`
- `Docs/Research/SitkaGrowthMortality/SitkaGrowthModel1.md`

No missing delegated-treatment source location was invented. The research index retains its explicit note that the audit's source documents and primary citations are not reproduced in the committed package.

## Files deliberately kept from current `main`

- `AGENTS.md` — unchanged; current main's live-GitHub coordination, Manager GitHub-native capability and GitHub-tracked handoff wording retained.
- `Docs/Project/AgentWorkflow.md` — current main text retained as the base; only the missing accepted semantics listed below were added.
- `Docs/Project/instructions/chatgpt-role.md`
- `Docs/Project/instructions/shared-project-instructions.md`
- `Docs/Project/instructions/claude-role.md`
- `Docs/Verification/ManagerGitHubRoutingUpdate/HANDOFF.md`
- `Docs/Verification/RackV2LaneRegistration/HANDOFF.md`
- `Docs/Project/coordination/active-tasks.md` — live single-writer state; **not touched**.

## Reconciliations performed

1. **`Docs/Project/AgentWorkflow.md`** — kept current main's 2026-10-27 GitHub capability/routing, live-GitHub lock and durable-handoff model. Added the accepted D-063 semantics that the reconciled specs depend on:
   - spec-driven development (`PACKET ONLY` / `SPEC REQUIRED`, SDD lifecycle, spec authority and status vocabulary);
   - canonical `specs/` artifacts and scoped spec loading;
   - verification cost tiers (Tier 1/2/3) and G1–G5 review gates;
   - dashboard synchronisation and the no-manual-generated-state rule;
   - lane/worktree safety and the machine-wide Unity resource policy;
   - spec fields in the task packet, worker handoff and integration decision.
   The ops transitional claim that `ops/workflow-foundation-v1:Docs/Project/coordination/active-tasks.md` was the temporary live record was **not** imported; current main's live GitHub/main coordination model remains authoritative.
2. **Role instruction files** — kept current main's GitHub wording and added only the D-063 classification/spec-authority semantics.
3. **`Docs/Project/current-milestone.md`** — brought forward the ops version, then corrected one stale sentence that said no spec was `APPROVED`. It now names the two current specs: Rack V2 (D-064, `APPROVED`) and MFM (D-065, approved for implementation planning, production coding still gated by Rack V2 persistence). No scope or decision changed.
4. **MFM `spec.md` header** — metadata only. The repository's lifecycle vocabulary is `DRAFT / APPROVED / IMPLEMENTING / VERIFYING / VERIFIED / SUPERSEDED`; the user's exact label `APPROVED FOR IMPLEMENTATION PLANNING` is preserved verbatim inside the mapped `APPROVED` status, together with the production-coding gate. Added the factual feature ID, owner/authority, created/accepted dates, D-065/D-010/D-064 references, milestone, evidence and supersession fields. No requirement, acceptance criterion, calibration or decision was changed.

## Deliberately not imported

- **`Docs/Project/coordination/active-tasks.md`** — live single-writer state. Current main/GitHub state wins; the historical ops version was not imported. The Overall Manager maintains current locks separately.
- **`CCF-Ops/board/active.md` and the `CCF-Ops/` tree** — inspected before action. The board is a manually maintained supporting working view, explicitly non-authoritative, last reconciled 2026-10-16 with row states through 2026-10-21. It is stale/superseded, not a generated dashboard, and was not imported. No board or dashboard file was manually rewritten.
- **`Docs/Project/context-manifest.md`** — the ops artifact map was not copied forward. The generated manifest (`Build/ProjectContext/context-manifest.json`) must be regenerated from the final context SHA by the publication pass. No manifest was hand-edited.
- **`Docs/Operations/WorkflowFoundation*.md`, `Docs/Operations/ActiveTasksTransition.md`, `Docs/Operations/AgentWorkflowProposedChanges.md`, `CCF-Ops/` tooling, roles, tests and pilot material** — operational workflow-foundation material, not the requested context/spec/research reconciliation. Its own transition text records that its G4 review was still awaiting assignment.
- **Ops versions of `AgentWorkflow.md`, the role files and `AGENTS.md`** — superseded by current main's newer routing model.

## MFM-00 status

`9f06398` is treated as **MFM-00 CONTENT COMPLETE — GITHUB PUBLISHED**, and its canonical changes are now reconciled into the authoritative lineage. Dashboard/context publication remains deliberately deferred to `WORKFLOW-CAPABILITY-SYNC-01`.

MFM-00 does **not** authorise implementation. MFM production coding remains gated by Rack V2 persistence being integrated and verified, and MFM does not automatically satisfy S1-D.

## Verification

| # | Check | Result |
|---|---|---|
| 1 | Final branch descends from current `main` workflow authority | PASS — branch created from `origin/main` `b07dd8a`; `git merge-base --is-ancestor origin/main HEAD` succeeds |
| 2 | D-063, D-064, D-065 present | PASS — all three rows present; D-065 dated `2026-10-27` |
| 3 | MFM four-file package present | PASS — `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` |
| 4 | Rack V2 spec present | PASS — four-file package present |
| 5 | Spec framework/templates present | PASS — `specs/README.md` plus four template files |
| 6 | Accepted research docs present | PASS — all five named records present and non-empty |
| 7 | Latest Manager GitHub-routing wording still present | PASS — `AGENTS.md` unchanged; GitHub wording retained in all three role files and `AgentWorkflow.md` |
| 8 | Live `active-tasks.md` not replaced | PASS — identical to `origin/main`; `git diff origin/main -- Docs/Project/coordination/active-tasks.md` is empty |
| 9 | No production `Assets/`, save, Unity scene, prefab, package or `ProjectSettings` change | PASS — changed paths are documentation, research and specification only |
| 10 | No generated dashboard file manually rewritten | PASS — no `CCF-Ops/board` or generated dashboard state added or edited |
| 11 | No Drive mirrors regenerated | PASS — generator/publication not run; no Drive output produced |
| 12 | Working tree clean after commit | PASS — context integration committed; the handoff commit leaves no uncommitted changes |

Additional checks:

- `git diff --check` passes after the metadata edit.
- Every backticked `Docs/Research/...` path in the reconciled research index resolves on the branch.
- The only unresolved backticked `specs/...` path is `specs/scenario-one-ui-visual-system-v2/`, which the Rack V2 spec itself marks as living on `feature/scenario-one-ui-visual-system-v2` and not on every checkout. No placeholder was invented.
- No Unity Editor, build, Player, save file, package or `ProjectSettings` action was taken.

## Changed-file list

```text
M  Docs/Project/AgentWorkflow.md
M  Docs/Project/current-milestone.md
M  Docs/Project/decision-log.md
M  Docs/Project/instructions/chatgpt-role.md
M  Docs/Project/instructions/claude-role.md
M  Docs/Project/instructions/shared-project-instructions.md
M  Docs/Project/research-index.md
M  Docs/Project/unity-project-overview.md
A  Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md
A  Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md
A  Docs/Research/RacksAndTreeMarking/CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md
A  Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md
M  Docs/Research/SitkaGrowthMortality/SitkaGrowthModel1.md
A  specs/README.md
A  specs/_template/checklists/requirements.md
A  specs/_template/plan.md
A  specs/_template/spec.md
A  specs/_template/tasks.md
A  specs/multiscale-forestry-management-v1/checklists/requirements.md
A  specs/multiscale-forestry-management-v1/plan.md
A  specs/multiscale-forestry-management-v1/spec.md
A  specs/multiscale-forestry-management-v1/tasks.md
A  specs/scenario-one-objective-1-rack-planning-field-marking-v2/checklists/requirements.md
A  specs/scenario-one-objective-1-rack-planning-field-marking-v2/plan.md
A  specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md
A  specs/scenario-one-objective-1-rack-planning-field-marking-v2/tasks.md
```

The handoff file itself (`Docs/Verification/ContextReconciliation/HANDOFF.md`) is added by the publication commit after `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`.

## Generated files deliberately deferred

- `Build/ProjectContext/context-manifest.json`
- stamped instruction composites and Drive mirrors
- dashboard ingestion/publication

These must be produced once from **`eb218eab8b8e200a81cc99bfef7e8906bc21a88d`** by `WORKFLOW-CAPABILITY-SYNC-01`. Do not use `afb70d8`; the context advanced.

## User decisions required

None blocking.

The only judgement call is the MFM header metadata mapping. The exact user label `APPROVED FOR IMPLEMENTATION PLANNING` and the production-coding gate are preserved; only the lifecycle token was mapped to the repository's `APPROVED` vocabulary. If the Manager prefers the raw label without the mapping, revert only the `Status:` line; no substantive spec content depends on it.

## Next step

Proceed to `WORKFLOW-CAPABILITY-SYNC-01` using the final context SHA:

```text
eb218eab8b8e200a81cc99bfef7e8906bc21a88d
```

## Unrelated work preserved

The pre-existing uncommitted changes in `/media/jer/ZX20/CCF-Primary` (including its local `AGENTS.md` edits, `Docs/Project/coordination/active-tasks.md` edits and the untracked `Docs/Operations/StorageArchitecture.md` and `Docs/ScenarioTwoPreProductionAudit.md`) were treated as unrelated user work. They were not imported, committed or modified. The reconciliation was performed in the separate clean worktree `/media/jer/ZX20/CCF-Reconcile`.
