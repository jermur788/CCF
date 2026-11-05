# #289 — CCF project-context snapshot refresh after D-071 parallel-rack decision

**Date:** 2026-11-05. **Status:** READY / NOT RUNNING.  
**Purpose:** publish an internally consistent new clean context snapshot after the user selected the existing five parallel racks for Scenario One, recorded as D-071. The old uploaded context remains historical until replaced; do not claim it current.  
**Classification:** **PACKET ONLY** — bounded safe local context generation/publication. No feature design, no Unity/C#/scene/source gameplay, no saves or Windows build.  
**Difficulty:** MEDIUM — clean Git ancestry/manifest provenance plus permanent Drive mirror publication and platform attachment handoff.  
**Best-suited worker:** T3 GPT-6.1 Sol Medium for local Git/Python/verification; Overall Manager can perform Drive connector publication if the worker does not have Drive write access. Provider availability UNKNOWN.  
**Claim:** [#289](https://github.com/jermur788/CCF/issues/289), READY / NOT RUNNING.  
**Local lane:** proposed disposable isolated clean clone `/media/jer/ZX20/CCF-ContextRefresh-D071-20261105`; **do not assume it exists**. If path is in use, dirty, inaccessible or disk unsafe, STOP and report; never overwrite or clean others.  
**Result branch:** `docs/context-publication-d071-20261105`, created as GitHub branch for docs handoff; recheck actual local branch/HEAD on arrival. Do not write to main or user's in-progress `integration/scenario-one-player-test-ready-v1` checkout.  
**Expected result:** `Docs/Handoffs/ProjectContext-20261105/CONTEXT-PUBLICATION-D071-RESULT.md` on result branch (durable handoff, not local-only).  
**Ownership:** worker owns only its isolated clone and generated Build/ProjectContext output + result handoff; Manager #200 solely edits `Docs/Project/coordination/active-tasks.md`.

## 1. Input authority and why refresh is mandatory

D-071 in [Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md) and the updated [Current Milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md) are both canonical context sources. The [Manager decision handoff](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/MANAGER-SCENARIO-ONE-PARALLEL-RACK-DIRECTION-20261105.md) states the authority transition; these context sources changed on main. The old composite stamped `d05b27ad376ce02748ee4703429ad3ff66a65542` is not current.

The repository generator is `Tools/ProjectContext/generate_project_instructions.py`, Python 3.10+ standard library. Its exact CONTEXT_PATHS are:
- `Docs/Project/game-brief.md`, `decision-log.md`, `current-milestone.md`, `unity-project-overview.md`, `AgentWorkflow.md` and `research-index.md`;
- `Docs/Project/instructions/shared-project-instructions.md`, `chatgpt-role.md` and `claude-role.md`;
- root `AGENTS.md` and `CLAUDE.md`;
- `Tools/ProjectContext/generate_project_instructions.py`.

**Expected current snapshot input (verify, do not blindly reuse):** the full main commit that last changed a context path is `81867c8940d7d07c952e8e49ca4a0cb93498e790` (D-071 Current Milestone). The worker must run `git log -1 --format=%H -- <all CONTEXT_PATHS>` at refreshed main and compare. If more recent source changes exist, take that newest **one full commit SHA** for *all* source reads; do not mix SHA from different files. Active tasks is live/excluded, not a context source. The later D-071 handoff/spec/coord commits alone do not advance the generator's stamp.

## 2. Local clean source generation

1. Verify safe location, mount/device identity, sufficient disk space and no existing worktree ownership overlap. Only create/clone the isolated repo if safe; do not change any existing checkout. This task never runs Unity and **does not require or consume the Unity machine lease**. If the report of a mismatched ZX20 device identity affects the clone's disk safety, STOP and report rather than bypass checks.
2. Refresh `main`, check exact branch and current Git HEAD. Ensure all generator-listed canonical context paths exist, are committed and have clean/untracked status. Do not use `--preview-dirty` for publication. Fail closed on any dirty/untracked source, source drift or stale context.
3. Run e.g. `python3 Tools/ProjectContext/generate_project_instructions.py --repo <clean-repo> --out <ignored-output>`, standard clean mode, preferably into `Build/ProjectContext/`. The script should produce **both** `chatgpt-project-instructions.md` and `claude-project-instructions.md`, `context-manifest.json` and stamped `mirrors/` of every context source.
4. Independently verify: all files stamp the **same** full context SHA, timestamp comes from `git show --format=%cI` for that commit, `preview=false`, SHA-256 hashes for each source/composite/mirror match the manifest, every mirror begins with `MIRROR — DO NOT EDIT` and points to its exact repo path, and regenerated output at same source SHA is byte-identical. No hand-edited composite or fabricated manifest.
5. Identify permanent Drive mirror document IDs/URLs from the existing publication registry (if needed, ask Manager to discover connector-side IDs). **One-way Git→Drive only:** update existing permanent mirrors **in place**, never create orphan duplicate copies or overwrite canonical Git from Drive. Publisher must verify a sample read-back and all mirror/document IDs, stamps and hashes. If Drive write is unavailable, label `LOCAL GENERATION VERIFIED / DRIVE PUBLICATION PENDING` and provide safe exported generated text/files or exact transfer plan for Manager. Do not mark the full refresh complete.
6. Platform knowledge/project attachments do **not** follow Drive automatically. Preserve and report old-to-new attachment replacement requirements for ChatGPT, Claude and other configured platforms; don't claim success without actual platform confirmations. Avoid exposing generated composites as alternate canonical sources.

## 3. Publication, handoff and stop rules

- Before starting work, mark #289 **[RUNNING]** with actual model/provider, isolated branch/worktree, starting HEAD and no Unity; if inaccessible, mark **[BLOCKED]** with specific reason instead.
- Publish exactly `Docs/Handoffs/ProjectContext-20261105/CONTEXT-PUBLICATION-D071-RESULT.md` as durable Markdown to the review/result branch. Include the chosen context commit, generator command, manifest SHA-256 proof, outputs list, old vs new stamps, whether Drive permanent IDs were updated and read back, platform replacement status and any outstanding user action; protect local/Drive credentials. If it cannot publish to GitHub, do not claim handoff complete.
- When source/manifest verified and result published, add a [HANDED-OFF] claim comment, link exact handoff and close #289; Manager #200 then inspects and publishes any remaining Drive mirrors and updates status. No claim that source-only merge advances the user's current playtest.
- No Windows/Player build; no Unity Editor; no user save, reference archive or gameplay edit; no forced locks, mount hacks, unrelated dirty tree changes, automatic context-file edits or Git merge into the player-test branch.

**Acceptance:** consistent clean context stamp and every hash match; Drive/permanent mirror and platform status independently labelled (PASS or PENDING); durable handoff on GitHub. No further provider review required for pure deterministic file generation if hashes and read-back are conclusive, but Manager makes publication verdict.

**Copy/paste dispatch:**

> T3 GPT-6.1 Sol Medium — take CCF context-publication claim #289. Read the exact handoff: https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ProjectContext-20261105/CONTEXT-PUBLICATION-D071-PACKET.md . Use only a **fresh, isolated clean clone** at the packet's conditional worktree/path and result branch, inspect main and the canonical generator, regenerate both composites, all mirrors and manifest at one verified full source SHA after D-071, verify hashes and deterministic stamps. Update permanent Drive mirrors in place **only if safely authorised and verifiable**; otherwise report that publication as PENDING with exact generated outputs for Manager. Publish the required `CONTEXT-PUBLICATION-D071-RESULT.md` handoff on the isolated result branch and record actual claim state. No Unity, current playtest checkout, saves, gameplay files or Windows build.

**Immediate user action:** send this dispatch to the isolated T3 local worker if available. No user playtest/Windows permission is granted by the task.
