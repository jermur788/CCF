# Manager #425 → context-publication worker #429: D-078 one-context-SHA Drive/attachment refresh

**Date:** 12 November 2026  
**Difficulty:** MEDIUM — exact Git snapshot and hash integrity across generator output, existing permanent mirrors, instruction composites and platform Project attachments.  
**Best suited:** Codex/T3 source-capable worker with a clean isolated checkout and authenticated Drive/attachment publication, or another verified capable browser/connector worker. **No Unity required.**  
**Issue / status:** https://github.com/jermur788/CCF/issues/429 — READY / NOT RUNNING until actual worker start.  
**Parent manager:** https://github.com/jermur788/CCF/issues/425.  
**Context source:** `main` at `bc38ff2461c6aa004dde6a73b84718c313f41c44` as of publication; source has approved D-078 in `Docs/Project/decision-log.md` and aligned `Docs/Project/game-brief.md`. **Refresh the live main and context commit at start**; context may have moved for unrelated commits. Read `Docs/Project/AgentWorkflow.md` and generator first. This is a browser/DevOps follow-up, not production forestry work.

## Purpose

The user approved the **eight independent Irish forest-management scenarios** as the current long-term Stage 1 direction with **sequential playable delivery and safe overlap of later research/design/disjoint work**. Two canonical context sources changed and were read back on main. Previous generated Project attachments and Drive mirrors are now stale relative to those edits. Refresh the **entire canonical one-SHA context** rather than trying to edit a couple of stale Drive documents. Keep Git canonical and Drive strictly one-way read-only mirror.

## Required steps

1. **Ownership/environment preflight:** inspect newest `main`, the real live `Docs/Project/coordination/active-tasks.md` **READ ONLY** and #429 claim; check the single-writer coordination owner #424 and any existing context publication process. Use a dedicated, clean writable **context publication lane** and confirm no conflicting writer/process. Do not borrow a Unity/Editor lease; no Unity needed. If work must be done on a local machine, use its preexisting safe Git/Drive publication tooling and avoid changing unrelated dirty work.
2. **Determine one clean source commit:** the newest commit changing any generator `CONTEXT_PATHS`, not a coordination-only commit. Confirm all 12 source paths exist and are committed cleanly. Generate using `Tools/ProjectContext/generate_project_instructions.py` **without `--preview-dirty`**, normally under ignored `Build/ProjectContext/`.
3. **Verify output integrity:** inspect `context-manifest.json`, ChatGPT/Claude composites and all mirrors. Check the single full context ID, context timestamp and SHA-256 of every canonical source, both composites and every mirror. Ensure output is reproducible from the exact same source commit. Detect/record any generator validation failure; never replace it with hand-edited output.
4. **Publish existing permanent Drive objects IN PLACE:** locate/confirm the preconfigured `CCF — Shared Project Context` permanent mirror set and instruction composites using existing manifest/Drive IDs and the accepted publication workflow. **Never create duplicate 'current' copies** or silently overwrite any unrelated file. Publish all intended files from the same source commit, read back their bytes/hashes (or explicitly name any unsupported readback), and preserve Drive's role as distribution mirror only.
5. **Refresh platform knowledge:** update/re-attach ChatGPT Game Dev and other platform instruction/knowledge attachments from that exact validated context where authenticated and permitted. Verify actual platform update/readback when possible. If current tools cannot change platform attachments, **state the exact manual user step and affected files**, instead of claiming completion. Older stamped attachments must not be presented as current.
6. **Dashboard sync:** investigate the existing automated ingestion/audit mechanism only insofar as needed to verify the new substantial project decision; never manually edit generated dashboard-state files. Report current dashboard ingestion and published audit evidence or **NOT VERIFIED** if unavailable.
7. **Document:** publish a concise, auditable GitHub Markdown result with the context ID, source/manifest/hash counts, exact Drive objects updated, readback results, any unresolved platform attachments/dashboard, and honest status. Do not embed credentials, private user data or external secrets. No code/Unity or product decision changes.

## Locked output lane and claim lifecycle

**Source branch (read-only packet):** `docs/overall-manager-d078-context-sync-20261112-v1` based on the accepted D-078 main context.  
**Worker result branch:** NEW `docs/overall-manager-d078-context-sync-result-20261112-v1` derived from the packet branch, after explicit absence check.  
**Worker writable GitHub file:** one new `Docs/Handoffs/Management-RoleSplit-20261112/D078-CANONICAL-CONTEXT-MIRROR-REFRESH-RESULT.md`.  
**Local lane/worktree:** an independently verified **clean context-publication checkout/worktree**, exact path chosen and registered by the authorised execution owner before any writes; **no known preallocated local worktree** under this browser Manager. No pretend local lock.

Before production publication, update issue #429 to **[RUNNING]** with the actual provider/model, branch, exact local worktree or authenticated browser lane, starting HEAD, context commit and read-only/writable scope. If a credential, publication API, owner lock or clean Git precondition fails, mark **[BLOCKED]** and publish a useful blocker/status record instead of modifying another owner's working state.

At handoff, push result Markdown, read it back, compare result branch to packet base and verify only the intended Markdown was added, link the durable result, mark issue **[HANDED-OFF]** and close only if publication is genuinely finished; otherwise record **BLOCKED/PARTIAL** and leave clear follow-up. Include dashboard status and whether Platform attachments were **UPDATED / PARTIAL / NOT VERIFIED** separately.

**Forbidden:** modifying game code, Unity, source/specs, current approved decisions, the current Scenario One milestone, `main`, live `active-tasks.md`, physical Predator Ops board or user protected playtest; Windows/Player build; destroying dirty local work; inventing missing Drive IDs or fabricating successful attachment readback. Do not relax manifest/hash guards for convenience.

## Handoff summary and user action

- **Purpose:** refresh previously generated project knowledge to the accepted eight-scenario context, not change forestry features.
- **Difficulty:** MEDIUM because many files and publication/readback steps require exact identity but no game implementation.
- **Preferred provider:** Codex/T3, source-capable + permitted Google Drive/project knowledge access, alternatively an appropriately authenticated connector worker.
- **Exact claim:** [#429](https://github.com/jermur788/CCF/issues/429), READY / NOT RUNNING.
- **Exact GitHub outcome:** `Docs/Handoffs/Management-RoleSplit-20261112/D078-CANONICAL-CONTEXT-MIRROR-REFRESH-RESULT.md` on new `docs/overall-manager-d078-context-sync-result-20261112-v1`.
- **Required user action:** a worker/session with valid file-system and connected Drive/Project attachment publication capabilities must receive this packet. It may report PARTIAL if platform attachment controls are unavailable; no automatic authorisation to edit other files.

**Paste-ready dispatch:**

> CCF context-publication task **#429**, difficulty MEDIUM, for a source-capable Codex/T3 worker with authorized Google Drive/Project-attachment access. Read the complete GitHub handoff: https://github.com/jermur788/CCF/blob/docs/overall-manager-d078-context-sync-20261112-v1/Docs/Handoffs/Management-RoleSplit-20261112/MANAGER-D078-CANONICAL-CONTEXT-MIRROR-REFRESH-PACKET-20261112.md . User approved D-078 eight-scenario direction; regenerate all canonical context mirrors and ChatGPT/Claude composites at one CLEAN Git SHA, verify manifest/hashes, publish the permanent Drive mirrors in place, refresh platform attachments when supported, report dashboard status, and publish the exact GitHub Markdown result. No Unity/game-code/main/coordination edits. Refresh live ownership, claim [RUNNING] before publication, and stop/report BLOCKED on any credential, clean-worktree or publication conflict.
