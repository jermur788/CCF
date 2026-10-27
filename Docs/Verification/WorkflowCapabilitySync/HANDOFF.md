# WORKFLOW-CAPABILITY-SYNC-01 — Manager/GitHub workflow and project-context publication

**Date:** 2026-10-27
**Task:** `WORKFLOW-CAPABILITY-SYNC-01 — PUBLISH UPDATED MANAGER/GITHUB WORKFLOW AND PROJECT CONTEXT`
**Routing:** SOL/CODEX (executed as T3 Code / `opencode-go/deepseek-v4.1-flash`)
**Verdict:** `WORKFLOW CAPABILITY SYNC COMPLETE — PROJECT ATTACHMENT REFRESH REQUIRED`

## Final context SHA

**`eb218eab8b8e200a81cc99bfef7e8906bc21a88d`**

This is the Overall Manager's `docs: reconcile ops context lineage with current main` commit on `origin/main` (2026-10-27T09:54:16+00:00). It reconciles the accepted D-063/D-064/D-065 context, the specification framework, Rack V2/MFM specs and the accepted research records from `ops/workflow-foundation-v1` (`9f06398`) onto current `main` while retaining the 2026-10-27 Manager/GitHub workflow wording.

`6eed963912f276681fa88b3af52714b2e952fdbb` is the later `origin/main` tip; it adds only a GitHub handoff and does not change the context commit. `Docs/Project/coordination/active-tasks.md` remains live coordination outside this SHA.

An earlier local merge, `716042c330abf81154e1755555bd70557d5dfce5`, was generated and briefly written to Drive before the Manager published `eb218ea`. It is **not** the final publication; its outputs are retained only as audit history under `superseded-716042c/`.

## Publication branch

`ops/workflow-capability-sync-v2`, based on `origin/main` `6eed963` (final context `eb218ea`) plus the Manager's task-registration commit `7d9a664` (live coordination only). The branch adds only this GitHub handoff after the context commit.

**Handoff content commit:** `a7409039454c27571a18307a5e4c288bb25d5d9e` (the commit that added this file). The live-coordination closure is the later tip of `ops/workflow-capability-sync-v2`; the exact branch tip is reported in the task completion record (`git log -1 ops/workflow-capability-sync-v2`).

## What changed

Canonical Git context now includes the accepted Manager/GitHub workflow capability and the accepted MFM/Rack V2 canonical documentation:

1. **Manager GitHub capability** — when connected GitHub write access is available, the Overall Manager may directly perform repository inspection, live task/lock maintenance, bounded coordination edits, Decision Log / milestone / Research Index / specification updates, durable GitHub handoff publication, and small reviewable text/source edits that do not require local Unity/build verification.
2. **Routing** — `FREE HERE` includes GitHub-native management work. `SOL/CODEX` remains the route for local worktree implementation, Unity/C# production changes, local tooling, integration, debugging, and compiler/Editor/runtime verification. A paid/local worker is not routed merely for a safe GitHub documentation/coordination edit.
3. **Verification boundary** — a GitHub commit proves repository state only. It does not prove Unity import, compilation, runtime behaviour, rendered output, local worktree cleanliness or machine-wide Unity lock state.
4. **Live coordination** — workers with live GitHub access must refresh the authoritative `Docs/Project/coordination/active-tasks.md` before claiming a task or lock is absent. A stale or dirty local copy does not override the newer GitHub record.
5. **GitHub handoffs** — substantive implementation, qualification, review, integration and research handoffs must be readable from GitHub. Local evidence archives may hold large logs, screenshots and raw data, but a local-only handoff is administratively incomplete.
6. **Accepted MFM canonical context** — D-064/D-065, the rack-planning and multi-scale forestry specifications, the specification framework and the accepted research records are included in the final context. D-065 remains confirmed for planning; no MFM mechanics were implemented or verified here.

Authority precedence is unchanged: user instruction → Decision Log → Current Milestone → Game Brief; implementation truth is the inspected repository / verified Unity state.

No gameplay, forestry simulation, save schema/policy, Unity asset, `ProjectSettings`, package or Rack V2 implementation file was changed.

## Generated context

Generated from a clean checkout at `eb218ea` with:

```sh
python3 Tools/ProjectContext/generate_project_instructions.py
```

Output: 12 stamped source mirrors, 2 role composites and `context-manifest.json`. Manifest: `context = eb218ea...`, `preview = false`, snapshot timestamp `2026-10-27T09:54:16+00:00`. Hash verification passed for all source, generated and mirror entries.

| Generated file | SHA-256 |
|---|---|
| `chatgpt-project-instructions.md` | `a951e0b9b1bd06148b412155fd6babe91fe379b683a9f742ad725cdadcec5060` |
| `claude-project-instructions.md` | `5bb37c3bc5913b86d601e839dc54d5cdd1e19c8cabfa0579fcbd225918e73a6a` |
| `context-manifest.json` | `28fc820bae330ca40e793540e2fadb8a43196207e9523cf1a72ef567e3d87fd8` |
| `mirrors/AGENTS.md` | `aed6c614ec4d775f6dce26b4cb27cf677774b17cd5a06d89394865ee71655f7c` |
| `mirrors/Docs/Project/AgentWorkflow.md` | `3edad7e3dc80f38eb0e5ab67811744eba6a94072d6d8430a08e79ea9620af4cd` |
| `mirrors/Docs/Project/instructions/chatgpt-role.md` | `ebd41872597cda3239f5e0f4a3e940fd7cb86e6e9ad1a7112e74ff61716a16c2` |
| `mirrors/Docs/Project/instructions/claude-role.md` | `c919cde6bb1cd07e769698632d9b23a1836b59042b23f96f9fc2480e6b3c0fc5` |
| `mirrors/CLAUDE.md` | `98e80ef8e11dac9f05b2d7bb2db8843a37173b17ac9e989ccce5eeb4708e7ab3` |
| `mirrors/Docs/Project/current-milestone.md` | `6c7479ec21a1e9a37498c7703aa42466d2f360224c3970329cd7f92bbc4c04da` |
| `mirrors/Docs/Project/decision-log.md` | `ce70a014063c7949f7920bc23413d39430ad484611e289614712b7c2b1df2619` |
| `mirrors/Docs/Project/game-brief.md` | `c036d3b54d7fe1d2402d255a3fa0c39053b37c7885fcf7fe1d88e644fc8f4bba` |
| `mirrors/Tools/ProjectContext/generate_project_instructions.py` | `3d645a6b6d5593865d633c695865c73af2096ec9b5cce39e073e11095f000b9c` |
| `mirrors/Docs/Project/research-index.md` | `765b99f11de7b04b5292611806b8eb18ac7b71f600cd4ede7764939fe1c88ead` |
| `mirrors/Docs/Project/instructions/shared-project-instructions.md` | `3d0193acbdb3683152003bb1638e74cd731f6b6908c60d83b288ac3036cba57a` |
| `mirrors/Docs/Project/unity-project-overview.md` | `9b72b36ed8b81fc5e1753c3052607b30c79ee8fdc81a10a7477a86b6f191d014` |

The complete generator manifest and readback hashes are in the external evidence folder; the manifest is authoritative for the full set.

## Drive publication and readback

Permanent Drive folder: [CCF — Shared Project Context](https://drive.google.com/drive/folders/1AuRJ334q4-2VFz-U5aziGhNJKWSCDiLS).

All 16 permanent files were replaced in place from the `eb218ea` generator outputs and downloaded again for byte comparison:

- `01 — Authoritative Context`: 12 stamped mirrors + `context-manifest.json` (`1OmPFnMJLIxHbJDvo4aaeuKqPLoKTTeau`).
- `02 — Project Instructions`: `chatgpt-project-instructions.md` (`1cF_bqhK4tpMJrnEg9F7_5FD3Z6v7SvFQ`), `claude-project-instructions.md` (`1VMycqdd8RD3bzAVU15lLjUiporj7vKnn`), `context-manifest.json` (`1L5UQW5a2kCaxYw1TcbTZpM7z0tbFZiFw`).

Readback: **16/16 byte-identical**; all headers/stamps use `eb218ea...`; both manifests record `preview = false`; no generated header or manifest carries `DIRTY PREVIEW — NOT A CONTEXT COMMIT`. The previously inconsistent `01` manifest was replaced at the same time.

Historical `07`–`12` Context Snapshot folders were left untouched. They are historical and are not current merely because they exist.

## Project Context Guide

The Drive Google Doc **Project Context Guide** (`1sWQn5ZRT6QQ5d-3sotsckSvimIDeQ0LcCRb9pkF5mmo`) was updated in place and read back. It now states that Git canonical context exists, permanent publication is operational, the Overall Manager is ChatGPT Game Dev unless changed, GitHub-native Manager coordination is supported when connected, local Unity/runtime verification remains separate, generated composites must not be hand-edited, and `active-tasks.md` remains live Git coordination outside the context commit. It records the current SHA as `eb218ea...` and does not create a competing authority ladder. Stale 2 October setup-status claims were removed.

## Platform / project knowledge

**`PROJECT ATTACHMENT REFRESH REQUIRES USER ACTION`**

The permanent Drive mirrors and both generated composites are current, but this environment has no ChatGPT or Claude project-authoring tool. The platform copies were not claimed current.

Attach or replace from:

`/media/jer/Files/CCF/Verification/WorkflowCapabilitySync-20261027/platform-attachments/`

Required files (all stamped `eb218ea...`; hashes in `SHA256SUMS.txt`):

- `CCF-chatgpt-project-instructions-eb218ea.md`
- `CCF-claude-project-instructions-eb218ea.md`
- `CCF-AgentWorkflow-eb218ea.md`
- `CCF-current-milestone-eb218ea.md`
- `CCF-decision-log-eb218ea.md`
- `CCF-game-brief-eb218ea.md`
- `CCF-research-index-eb218ea.md`
- `CCF-unity-project-overview-eb218ea.md`
- `CCF-context-manifest-eb218ea.json` and `CCF-context-manifest-eb218ea.md` (same manifest content)
- `CCF-shared-project-instructions-eb218ea.md`, `CCF-chatgpt-role-eb218ea.md`, `CCF-claude-role-eb218ea.md`, `CCF-AGENTS-eb218ea.md`, `CCF-CLAUDE-eb218ea.md`

The ChatGPT composite is **12,023 characters**. A previously recorded ChatGPT project-instruction limit was 8,000 characters, so the full generated composite cannot be assumed to fit the project instruction field. Do not hand-edit the generated composite; if the platform rejects it, attach it as a project source file or request a separately accepted platform-fitting instruction.

## Verification

| Check | Required | Result |
|---|---|---|
| Final context SHA identified | PASS | `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` |
| Generator clean run | PASS | clean committed mode, `preview=false` |
| ChatGPT composite generated | PASS | `a951e0b9...` |
| Claude composite generated | PASS | `5bb37c3b...` |
| Manifest/hash verification | PASS | 12 sources, 2 composites, 12 mirrors |
| No dirty-preview marker | PASS | no generated header/manifest uses it |
| Drive ChatGPT copy readback | PASS | byte-identical |
| Drive Claude copy readback | PASS | byte-identical |
| Other permanent mirrors synchronized | PASS | 16/16 files read back |
| Project Context Guide updated | PASS | in place; readback verified |
| Platform/project attachments refreshed or explicitly blocked | BLOCKED | `PROJECT ATTACHMENT REFRESH REQUIRES USER ACTION` |
| Gameplay/runtime files changed | MUST BE NO | NO (no `Assets/`, `Packages/`, `ProjectSettings/` or `Tools/Verification/` changes) |
| Unity required | NO | not required, not run |
| Windows build | NO | not run |

Additional checks:

- Final source set contains the Manager GitHub capability, `FREE HERE`/`SOL/CODEX` boundary, GitHub verification boundary, live-lock refresh requirement and durable GitHub handoff requirement.
- Claude composite instructs workers to refresh the GitHub-authoritative lock record.
- Shared instructions retain the GitHub-only vs local/Unity verification boundary.
- Authority block is unchanged from `9f06398`/`eb218ea`.
- MFM D-064/D-065 and the accepted research index entries are present in the generated mirrors.
- No generated file uses the old `4a68d52`, `9f06398`, `afb70d8`, `716042c` or other obsolete context stamp as its publication stamp.

## Dashboard

`DASHBOARD SYNC: NOT AVAILABLE`

No dashboard publisher/listener was available in this environment (`127.0.0.1:8090` not listening; no established publication process was running). Generated dashboard-state files were not manually edited.

## User action still required

1. Replace/attach the ChatGPT Game Dev project instructions and static context files listed above. The full ChatGPT composite is 12,023 characters; confirm the platform limit before pasting.
2. Replace/attach the Claude project instruction copy if that project still uses an older composite.
3. Merge `ops/workflow-capability-sync-v2` into the desired integration/main line if branch publication alone is not sufficient. The final context SHA is `eb218ea...`; the handoff commit does not change it.
4. Record the live coordination row for this task if the Manager wants it in `active-tasks.md`. That file is live coordination, outside the context commit, and was not advanced by this publication.

## Evidence

External evidence root:

`/media/jer/Files/CCF/Verification/WorkflowCapabilitySync-20261027/`

- `generated/` — exact `eb218ea` generator outputs.
- `drive-before/` — previous permanent Drive bytes (context `4a68d52`; the `01` manifest was already inconsistent at `341ccbf`).
- `drive-after/` — final Drive readback (context `eb218ea`).
- `platform-attachments/` — stamped platform attachment set and `SHA256SUMS.txt`.
- `drive-map.tsv` — permanent Drive file IDs and generated-source mapping.
- `superseded-716042c/` — the earlier local merge outputs, retained for audit only.
- `drive-after/ProjectContextGuide__export.txt` — Guide readback.

GitHub handoff:

`Docs/Verification/WorkflowCapabilitySync/HANDOFF.md`
