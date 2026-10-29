# CONTEXT-REFRESH-20261029 — reconciled project-context snapshot

**Task:** `CONTEXT-REFRESH-20261029-RECONCILED-STATE` (claim #84)
**Status:** `CONTEXT REFRESH GIT/LOCAL QUALIFIED — PERMANENT DRIVE IN-PLACE PUBLICATION BLOCKED BY SESSION CAPABILITY`

This is **not** `PROJECT CONTEXT REFRESH COMPLETE`. Generation, determinism, hash checks and the attachment bundle are done. The permanent Drive publication and byte-identical readback have **not** been performed and are **not** claimed as PASS.

## Execution record

| Item | Value |
|---|---|
| Agent / model / provider | Claude Code, Claude Sonnet 5.5, Anthropic (via T3 Code) |
| Branch / worktree | `ops/context-refresh-20261029-reconciled-state` / `/media/jer/ZX20/CCF-ContextRefresh20261029` |
| Starting HEAD (= selected `origin/main` tip) | `c5a269004a6b1efc3b6ffb134ca20070785f8e5e` |
| **Context SHA stamped by generator** | **`d05b27ad376ce02748ee4703429ad3ff66a65542`** (2026-10-29T00:07:22+00:00, `docs: correct milestone reconciliation date`) |
| Generator | `python3 Tools/ProjectContext/generate_project_instructions.py --out <evidence>/generated` (Python 3.12.3); script SHA-256 `9e4ffa96b5969572db2816200c33e027a7e5d7ba4c4dc98e909701947b438411` |
| Dirty primary checkout | Not used. A fresh clean worktree was created from `origin/main`; `git status --porcelain` empty before and after generation |

The generator stamps the **last commit that touched a context source file**, not the checkout HEAD. `c5a2690` is the checked-out tip, and it adds only coordination and packet documents outside the context set. Everything published is stamped `d05b27a`. `git diff d05b27a c5a2690` over the context paths touches only `Docs/Project/coordination/active-tasks.md`, which is excluded.

### Precondition checks (at the stamped SHA `d05b27a`)

- `135e7f77bc481c52b81b6575d94afd779326eb8a` (reconciled current milestone) in ancestry: **yes**
- `b118d8e63f60d54c9f9a3bed6de40af83b35efc1` (PR #80 Rack layout repair merge) in ancestry: **yes**
- `8920e927f40de2360cb3e34ced4c346236823b2b` (PARITY-03 Q11 handoff) in ancestry: **yes**
- `origin/main` re-read with `git ls-remote` after fetch: `c5a2690…` (tip had not moved)
- No canonical-authority conflict was found; no source was edited.

## Generation and determinism (PASS)

- Clean committed mode; manifest `preview = false`; `snapshot_timestamp = 2026-10-29T00:07:22+00:00`.
- Output: 12 stamped source mirrors, 2 role composites, `context-manifest.json`.
- Manifest/hash verification: all 12 source hashes (re-read with `git show d05b27a:<path>`), 2 composite hashes and 12 mirror hashes match the files on disk (0 mismatches).
- A second clean generator run is **byte-identical** to the first (`diff -r` empty).
- Every mirror header and both composites carry `Context: d05b27ad376ce02748ee4703429ad3ff66a65542`.
- No `DIRTY PREVIEW — NOT A CONTEXT COMMIT` marker in any header or manifest. The phrase appears only as descriptive text inside the mirrored generator script and AgentWorkflow prose.
- The generated context includes the reconciled current milestone ("Scenario One — Complete-Loop Three-Species Demo Readiness", last reconciled 2026-10-29). It keeps the authority hierarchy, the live `active-tasks.md` rule (both composites) and the "compilation/GitHub commit does not prove Unity/runtime behaviour" boundary.

| Generated file | SHA-256 |
|---|---|
| `chatgpt-project-instructions.md` (12,019 chars / 12,023 bytes) | `50a93899fb8d7e08a7c4b096400c2baaaf53783c89b7e0508e3afba8a738e0a6` |
| `claude-project-instructions.md` (9,554 chars / 9,558 bytes) | `0c71c479bc4a736e800c544d0d8fab8f8684a86aff6fb6c9aa74d3d731ee2901` |
| `context-manifest.json` | `87bf9d9ce4f5e37955c24477d0958ad5da5e24373ffa742837e6c1fdc0eabde5` |

Per-mirror hashes are in the manifest and the table below. The artifacts are deterministic. Rerun the generator at `d05b27a` to reproduce them byte-for-byte.

## Permanent Drive publication — BLOCKED (connector capability only)

The Drive connector in this session exposes `create_file`, `copy_file`, `trash_file`, `read/download`, and `update_file` as **metadata-only**. None can overwrite the bytes of an existing file, so in-place replacement was not possible. By Manager direction, **no workaround was used**: no `create_file`, no new IDs, no renames, no trashing, no competing "current" copies. No Drive file, permission or Project Context Guide content was modified.

Verified live Drive state at the time of stopping: all 16 permanent files still have `modifiedTime` 2026-10-27 (10:06–10:07Z) and the sizes and stamp from the older `eb218ea` publication. Permanent Drive is therefore **still stale** (`eb218ea`). The historical `07`–`12` snapshot folders were not touched.

Owner of all 16 files: `inseparablefromnature@gmail.com`. Folder parents: `01 — Authoritative Context` = `1AcY1ymmiwze7vMKOvLX7msrCgGboMlgS`; `02 — Project Instructions` = `1o8vwzUnePWqP0ICKeIXBjRh3j-vbw9SL`.

### Publication manifest (source context SHA for every row: `d05b27ad376ce02748ee4703429ad3ff66a65542`)

Artifact root `<A>` = `/media/jer/Files/CCF/Verification/ContextRefresh-20261029-ReconciledState/generated/`. Machine-readable copies: `publication-manifest.tsv` and `.json` in the parent evidence directory (SHA-256 `eb3717ba442cc65b2c244234471f62013989f926e1e89752dfed7ccfcbea31fa` and `ceaf1863f80df5d6ba94b506e49e0d744647e7505ddb1001d6343e5e5f47f339`).

| Drive file ID | Existing filename | Replacement artifact (relative to `generated/`) | SHA-256 | MIME type | Bytes |
|---|---|---|---|---|---|
| `1OmPFnMJLIxHbJDvo4aaeuKqPLoKTTeau` | `context-manifest.json` (01 — Authoritative Context) | `context-manifest.json` | `87bf9d9ce4f5e37955c24477d0958ad5da5e24373ffa742837e6c1fdc0eabde5` | application/json | 3125 |
| `1u-ge5a2gWhRg5wf8sQvkJMJsymh9Owu2` | `MIRROR_AGENTS.md` (01 — Authoritative Context) | `mirrors/AGENTS.md` | `e2cc4ddde3fbb978bf634db54225e8995795c7db32a9089be1fdfed03816b524` | text/markdown | 9296 |
| `1vFhEJwyTBC3ZEDpAaatsrrvONekDM-AP` | `MIRROR_AgentWorkflow.md` (01 — Authoritative Context) | `mirrors/Docs/Project/AgentWorkflow.md` | `55ccab790ce53624704b6bd68758cd18a5aade76c5729397592f635f463ee34b` | text/markdown | 28475 |
| `1jb_4l9ZzYAH8A_gRvFHe2HHcOIG-ikNO` | `MIRROR_chatgpt-role.md` (01 — Authoritative Context) | `mirrors/Docs/Project/instructions/chatgpt-role.md` | `13d2cfe143a8d4c2dda96f3aaf14a629a7a48686e80a2a43e1c446ddba16c559` | text/markdown | 5130 |
| `1mZ6uTiSzi0cgHJeb6MHohbXzPA_AtcNS` | `MIRROR_claude-role.md` (01 — Authoritative Context) | `mirrors/Docs/Project/instructions/claude-role.md` | `ce35339b4d65c14206ad97a19d5c27dc12a7283d7cb7cdf94699680fdb5dd410` | text/markdown | 2675 |
| `11OSbUwDMfbbcZjYPMp_v3FxLmzPLN5pv` | `MIRROR_CLAUDE.md` (01 — Authoritative Context) | `mirrors/CLAUDE.md` | `6f9b0c09fdfe1b3844c5f6b5d792528340f6a9272d0e35b1f7cc75c66ec39f41` | text/markdown | 667 |
| `1Z9p9Fyl1BgsA7JlBOOO7aUfK47jUvo69` | `MIRROR_current-milestone.md` (01 — Authoritative Context) | `mirrors/Docs/Project/current-milestone.md` | `e8c943de8d8f5e2b3cdfdf56ec974cbd310a386a9117e64f2f1208ab63d5e24c` | text/markdown | 11504 |
| `1UccCz_G524NGtAKbEuVSW2OzgqiP2IQs` | `MIRROR_decision-log.md` (01 — Authoritative Context) | `mirrors/Docs/Project/decision-log.md` | `07de613b92ac879f4ac348a761f719baa630574f6511f49e6fae800596819504` | text/markdown | 45359 |
| `1FStJ1nRY0gyC86YQdI1UEIe3gsuSJIRQ` | `MIRROR_game-brief.md` (01 — Authoritative Context) | `mirrors/Docs/Project/game-brief.md` | `c06a62a7dd3fd3fb5281486626abc24b8d6db1e298c16b3b623e515d9be6bed8` | text/markdown | 10401 |
| `1n_lSuRbTnyE55zS3rvoQG_0UpRQkbs-l` | `MIRROR_generate_project_instructions.py` (01 — Authoritative Context) | `mirrors/Tools/ProjectContext/generate_project_instructions.py` | `47a6bf8f70f7e1571dba815a6edd6b887fe1004a326dccedde4eb87fcb9a0433` | text/markdown | 7871 |
| `1yNOgIy_dH1v9ppGup1LEoc2tDyBhtrRq` | `MIRROR_research-index.md` (01 — Authoritative Context) | `mirrors/Docs/Project/research-index.md` | `eb532a546daecdafda0753facd6dd405cc6d5bd724d8cfea0b1c8b80c9038a73` | text/markdown | 25042 |
| `12Snhqp-1Vc6yT2L8pRB84oVd7HEsWt7j` | `MIRROR_shared-project-instructions.md` (01 — Authoritative Context) | `mirrors/Docs/Project/instructions/shared-project-instructions.md` | `251c9ec790c921762a2716eb1d9867665053bf94aad3c7bcbebb69d47fc0265b` | text/markdown | 6129 |
| `1DZM_naGP35s3NlJQoYhaxO-O7zQEDRHx` | `MIRROR_unity-project-overview.md` (01 — Authoritative Context) | `mirrors/Docs/Project/unity-project-overview.md` | `dd28b99520f9a3f8a64d1cef62f80a6f65227aaabb5f6ed7eb3c89c0831f4fdb` | text/markdown | 70291 |
| `1cF_bqhK4tpMJrnEg9F7_5FD3Z6v7SvFQ` | `chatgpt-project-instructions.md` (02 — Project Instructions) | `chatgpt-project-instructions.md` | `50a93899fb8d7e08a7c4b096400c2baaaf53783c89b7e0508e3afba8a738e0a6` | text/markdown | 12023 |
| `1VMycqdd8RD3bzAVU15lLjUiporj7vKnn` | `claude-project-instructions.md` (02 — Project Instructions) | `claude-project-instructions.md` | `0c71c479bc4a736e800c544d0d8fab8f8684a86aff6fb6c9aa74d3d731ee2901` | text/markdown | 9558 |
| `1L5UQW5a2kCaxYw1TcbTZpM7z0tbFZiFw` | `context-manifest.json` (02 — Project Instructions) | `context-manifest.json` | `87bf9d9ce4f5e37955c24477d0958ad5da5e24373ffa742837e6c1fdc0eabde5` | application/json | 3125 |

Notes for the publication lane:

- Existing MIME is recorded as-is. `MIRROR_generate_project_instructions.py` is stored as `text/markdown` with a `.py` extension. Preserve the existing file ID and filename. A raw-content overwrite such as `update_file(fileId, file_uri=…)` is the intended route.
- Both `context-manifest.json` rows carry identical bytes.
- Several rows (`MIRROR_game-brief.md`, `MIRROR_generate_project_instructions.py`, `MIRROR_research-index.md`, `MIRROR_shared-project-instructions.md`) keep the same byte size only because the 40-character SHA stamp is the same length. Their hashes still differ from the 2026-10-27 (`eb218ea`) publication, so all 16 must be replaced.
- After overwrite: download each of the 16 files, compare SHA-256 to the table, confirm both manifests record `d05b27a…` with `preview=false`, and confirm no stale `eb218ea` stamp remains.
- **Project Context Guide** (Google Doc `1sWQn5ZRT6QQ5d-3sotsckSvimIDeQ0LcCRb9pkF5mmo`): not touched. Its factual current-SHA line (`eb218ea…`) must be updated to `d05b27a…` after the mirrors are published, as in WORKFLOW-CAPABILITY-SYNC-01. Do not add a competing authority ladder.

## Platform attachment bundle (prepared)

Directory: `/media/jer/Files/CCF/Verification/ContextRefresh-20261029-ReconciledState/platform-attachments/` (16 files, `SHA256SUMS.txt`). The names mirror the 2026-10-27 set, with the short stamp `d05b27a`:

`CCF-chatgpt-project-instructions-d05b27a.md`, `CCF-claude-project-instructions-d05b27a.md`, `CCF-AgentWorkflow-…`, `CCF-current-milestone-…`, `CCF-decision-log-…`, `CCF-game-brief-…`, `CCF-research-index-…`, `CCF-unity-project-overview-…`, `CCF-shared-project-instructions-…`, `CCF-chatgpt-role-…`, `CCF-claude-role-…`, `CCF-AGENTS-…`, `CCF-CLAUDE-…`, `CCF-context-manifest-d05b27a.json` and `CCF-context-manifest-d05b27a.md` (same content).

Hashes are in `SHA256SUMS.txt`; the two composites match the table above.

- **ChatGPT Game Dev project:** the environment has no ChatGPT project-authoring tool. Status: **not refreshed; user action required.** The composite is 12,019 characters. A previously recorded ChatGPT instruction field limit was 8,000 characters, so the unchanged composite may need to go in as a project source file. It was not hand-trimmed.
- **Claude project:** no Claude project-authoring tool is available. Status: **not refreshed; user action required.**

## Other required fields

| Item | Result |
|---|---|
| Drive file count / byte-identical readback | **0 of 16 replaced; readback NOT performed** (blocked) |
| Project Context Guide update/readback | NOT performed (blocked; see above) |
| Repository cleanliness | Generation worktree clean; the only change is this handoff. The dirty primary checkout was neither used nor modified |
| Dashboard | Not available; no publisher run; generated dashboard-state files not edited |
| Gameplay / Unity / save / schema / mechanic / Decision Log change | None. No Unity run, no Windows build, no account/privacy/billing/permission change |

## Evidence

`/media/jer/Files/CCF/Verification/ContextRefresh-20261029-ReconciledState/`: `generated/` (artifacts), `generated-rerun/` (determinism rerun), `generate.log`, `generate-rerun.log`, `platform-attachments/`, `publication-manifest.{tsv,json}`, `drive-map.tsv` (inherited ID map, unchanged).

## Remaining action

1. The publication-capable lane (the Manager session, if its connector supports raw-content overwrite) overwrites the 16 files by ID, reads each back, compares SHA-256, and updates the Project Context Guide stamp to `d05b27a…`.
2. The user attaches the platform bundle to the ChatGPT Game Dev and Claude projects.
3. Only after step 1's readback passes can the overall token `PROJECT CONTEXT REFRESH COMPLETE — …` be issued.
