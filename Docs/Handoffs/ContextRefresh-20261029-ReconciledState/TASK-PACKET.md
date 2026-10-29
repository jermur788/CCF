# PROJECT CONTEXT REFRESH — RECONCILED SCENARIO ONE STATE

**Task ID:** CONTEXT-REFRESH-20261029-RECONCILED-STATE  
**Class:** Context publication / infrastructure only  
**Overall Manager:** ChatGPT Game Dev  
**Supersedes:** `Docs/Handoffs/ContextRefresh-20261027-ApprovedSpecs/TASK-PACKET.md`

## Why this task exists

The currently published project-context snapshot is still `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` from 2026-10-27. It is materially stale.

Canonical Git has since advanced through substantial accepted authority and status changes, including:

- Rack V2 confirmation/minimap, OPEN-3 authority, Combined HUD repair, and the current RACK-V2-08 continuation boundary;
- S1-C query/history integration and T-03 static Manager disposition;
- S1-D T-01/T-02 integration and the authorised T-06 save-history hardening packet;
- PARITY-03 qualification/review plus Q11 evidence-record repair and Q10 strengthening packet;
- PARITY-04/PARITY-05 accepted/integrated work and current PARITY-05 Unity boundary;
- MFM-02 integration and corrected MFM-03 queue state;
- the 2026-10-28/29 rewrite of `Docs/Project/current-milestone.md` from stale historical prose into current complete-loop three-species demo authority;
- the cleanup of `Docs/Project/coordination/active-tasks.md` back to live ownership/queue semantics. (`active-tasks.md` remains excluded from the snapshot and must be read live.)

The old context-refresh packet is therefore superseded. Do not publish another `eb218ea`-era snapshot.

## Goal

Generate and publish one fresh, internally consistent project-context snapshot from a single clean current `main` SHA using the established `Tools/ProjectContext/generate_project_instructions.py` workflow, then verify permanent Drive readback and prepare the platform attachment set.

This is publication only. It does not alter product authority.

## Target lane

SOL/CODEX / strongest reliable local infrastructure-capable lane. A bounded OpenCode worker is acceptable if it can execute the established generator, hash/readback verification and Drive publication workflow without changing account/privacy/billing settings.

Suggested branch: `ops/context-refresh-20261029-reconciled-state` in a dedicated clean worktree.

No Unity and no machine-wide Unity lock are required.

## Preconditions

1. Refresh `origin/main` and `Docs/Project/coordination/active-tasks.md` immediately before starting.
2. Mark the task claim `[RUNNING]` with actual agent/model/provider, branch/worktree and starting HEAD.
3. Choose **one clean current `main` SHA** after refresh. The task packet does not pin the source SHA because `main` may advance between dispatch and execution; the handoff must record the exact source SHA actually used.
4. Confirm the chosen SHA contains at least:
   - the reconciled `Docs/Project/current-milestone.md` commit `135e7f77bc481c52b81b6575d94afd779326eb8a` or descendant;
   - Rack layout integration PR #80 / merge `b118d8e63f60d54c9f9a3bed6de40af83b35efc1` in ancestry;
   - PARITY-03 Q11 handoff commit `8920e927f40de2360cb3e34ced4c346236823b2b` or descendant;
   - current Decision Log/spec authority.
5. `Docs/Project/coordination/active-tasks.md` is **not** part of the context snapshot. Do not bake its current rows into generated context as static authority; workers continue to read it live.
6. Verify the checkout is clean. Do not use or absorb a dirty primary worktree.
7. If canonical source documents materially conflict with latest accepted user/Manager authority, STOP rather than silently “fixing” them during generation:

`CONTEXT REFRESH BLOCKED — CANONICAL AUTHORITY CONFLICT`

## Required generation

Run the established generator exactly from the selected clean context SHA:

```sh
python3 Tools/ProjectContext/generate_project_instructions.py
```

Use the generator's canonical output locations/workflow. Do not hand-edit generated composites or mirrors.

Required checks:

1. generator reports clean committed mode / `preview=false`;
2. expected source mirrors, ChatGPT composite, Claude composite and context manifest are present;
3. every generated stamp/reference uses the selected context SHA;
4. manifest/hash consistency passes;
5. a second clean generator run is byte-identical to the first;
6. no `DIRTY PREVIEW — NOT A CONTEXT COMMIT` marker appears in the publication set;
7. generated context includes the reconciled current milestone and current authority hierarchy;
8. generated context continues to state the GitHub-vs-local/Unity verification boundary and live `active-tasks.md` rule.

## Permanent Drive publication

Follow the established one-way **Git canonical → Drive mirror** process from `Docs/Verification/WorkflowCapabilitySync/HANDOFF.md`.

- Replace the permanent Drive context mirrors/composites **in place** rather than creating a competing “current” folder.
- Keep historical snapshot folders historical.
- Publish only bytes generated from the one selected source SHA.
- Download/read back every permanent file that was replaced and compare byte-for-byte against the generated source.
- Verify both permanent manifests agree and record the selected source SHA with `preview=false`.
- Update the Project Context Guide only for factual current-SHA/publication status if the established workflow requires it; do not create a competing authority ladder.

Do not change Drive permissions, account/privacy settings or billing.

## Platform attachment set

Prepare a stamped attachment bundle for the current ChatGPT Game Dev project and Claude project matching the generated source SHA. Include the same canonical role/source set used by the established workflow.

Record:

- exact file names;
- SHA-256 hashes;
- ChatGPT composite character/byte count;
- whether direct project-instruction replacement is supported in the execution environment;
- any user action still required to replace project instructions/static context attachments.

Do not hand-trim the generated composite merely to fit a platform field. If a platform limit rejects it, preserve the generated artifact unchanged and report the supported attachment/accepted-alternative route.

## Repository publication

Publish a GitHub Markdown handoff:

`Docs/Verification/ContextRefresh-20261029-ReconciledState/HANDOFF.md`

The handoff must include:

- actual agent/model/provider;
- branch/worktree;
- starting HEAD;
- exact selected context SHA;
- generator command/version/path;
- generated mirror/composite count;
- ChatGPT/Claude composite hashes;
- manifest hash and deterministic rerun result;
- Drive file count and byte-identical readback result;
- Project Context Guide update/readback status;
- platform attachment bundle path/list/hashes;
- ChatGPT platform refresh status;
- Claude platform refresh status;
- repository cleanliness;
- dashboard status;
- exact remaining user action, if any.

## Hard restrictions

No gameplay/Unity code.  
No Unity execution.  
No save/schema change.  
No Rack/S1-C/S1-D/parity/MFM mechanic or calibration change.  
No Decision Log/spec redesign while generating.  
No manual edits to generated context composites/mirrors.  
No manual edits to generated dashboard-state files.  
No account/privacy/billing changes.  
No Windows build.

## Exit

Success:

`PROJECT CONTEXT REFRESH COMPLETE — RECONCILED STATE PUBLISHED`

If Git/Drive is complete but platform project attachments require user action:

`PROJECT CONTEXT REFRESH COMPLETE — PLATFORM ATTACHMENT UPDATE REQUIRED`

If canonical authority conflicts:

`CONTEXT REFRESH BLOCKED — CANONICAL AUTHORITY CONFLICT`
