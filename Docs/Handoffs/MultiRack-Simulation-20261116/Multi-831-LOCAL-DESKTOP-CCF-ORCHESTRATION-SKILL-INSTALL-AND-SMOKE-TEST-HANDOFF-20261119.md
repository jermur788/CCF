# Multi #831 — Install CCF Orchestration skill on the actual desktop; verify activation

**19 November 2026 | READY / NOT RUNNING | LOCAL PERSONAL SKILL INSTALL ONLY — NO CCF SOURCE OR PHYSICAL OPS WORK**

## 1. Task, actor and done condition

The owner explicitly requests **a desktop agent to install the CCF Orchestration skill**. The Manager authored the skill package in the ChatGPT environment, but its prior ZIP **was not installed or verified on the owner's desktop**. This handoff supplies permanent GitHub text sources so a new local agent is not dependent on an unshareable `/mnt/data` or sandbox link.

**Task:** verify the actual host, intended desktop product and supported skill directory, install the vetted four-file `ccf-orchestration` skill into the proper **personal/user-level** skill location without overwriting anything, then demonstrate skill discovery and a *read-only* invocation. Report actual evidence.

**Difficulty:** LOW–MEDIUM: no CCF code or Unity; moderate installation host/permissions/app-version ambiguity and preserving preexisting configuration. **Best provider/model:** local **Codex GPT-6 High or GPT-6.2 High** desktop agent with *real local user-filesystem access* (Claude local is acceptable if it can legitimately manage the user's intended Codex skill location). No GitHub-only/browser agent can claim local installation.

**Claim:** [#831](https://github.com/jermur788/CCF/issues/831), **READY / NOT RUNNING**. Current dedicated Multi Manager #730 retains task intake and review. The actual desktop worker must post **[RUNNING]** on #831 with truthful model/provider/desktop product/OS/host (redacting secrets), observed starting GitHub source HEAD, installation target and exact file allowlist *before any write*. No model/host is registered yet; do not backdate it.

## 2. Permanent source package (four text files)

All four texts were published on isolated branch `docs/multi-831-desktop-orchestration-skill-install-packet-20261119-v1` **under `Docs/Tools/DesktopSkills/ccf-orchestration/`**, not integrated into main. Fetch this branch's latest verified exact contents:

| Destination relative to skill folder | GitHub package source | Git blob ID for local byte verification |
|---|---|---|
| `SKILL.md` | [SKILL.md](https://github.com/jermur788/CCF/blob/docs/multi-831-desktop-orchestration-skill-install-packet-20261119-v1/Docs/Tools/DesktopSkills/ccf-orchestration/SKILL.md) | `cd650a1f9023792c0994007e42a59f6f21c633b6` |
| `agents/openai.yaml` | [agents/openai.yaml](https://github.com/jermur788/CCF/blob/docs/multi-831-desktop-orchestration-skill-install-packet-20261119-v1/Docs/Tools/DesktopSkills/ccf-orchestration/agents/openai.yaml) | `9701c5d6435a0f694b64eaa8b1f2c05919d0b38b` |
| `references/handoff-checklist.md` | [handoff-checklist.md](https://github.com/jermur788/CCF/blob/docs/multi-831-desktop-orchestration-skill-install-packet-20261119-v1/Docs/Tools/DesktopSkills/ccf-orchestration/references/handoff-checklist.md) | `f568c7bab92790c63f01129443b0c561178c0d25` |
| `references/source-map.md` | [source-map.md](https://github.com/jermur788/CCF/blob/docs/multi-831-desktop-orchestration-skill-install-packet-20261119-v1/Docs/Tools/DesktopSkills/ccf-orchestration/references/source-map.md) | `a8e319ed0d0851ef17c57c0e24d803733a216151` |

These are text source files **staged in GitHub for install**; the separate chat ZIP is an alternative source, not independently proven to match these four staged blobs byte-for-byte. For a verifiable install use the **four GitHub source files above** as one coherent package. Do not mix file revisions.

### Source verification and security
- Inspect `SKILL.md` frontmatter for name `ccf-orchestration`, intended scope, and ensure none of the files asks to execute scripts, send secrets, bypass approvals or alter worktrees.
- The above blob IDs are Git SHA-1 object IDs of *complete content* (not raw SHA-256 hashes). Where Git is available, `git hash-object <file>` should equal the corresponding blob after download. Alternatively compare independently fetched raw content byte-for-byte before and after installation. GitHub readback by Manager verified the branch texts, but **not** the agent's downloaded desktop bytes.
- Preserve this directory tree exactly. Do not execute arbitrary bundled scripts; package is four text files only.

## 3. Exact local installation sequence

**Preflight:**
1. Determine whether the user's target is **Codex desktop/CLI personal skills** or a separate ChatGPT desktop skill-import mechanism. Check *installed product/version/support* and the actual skill discovery path. Do **not** assume a ChatGPT Desktop ZIP upload button exists. Prefer the supported Codex user skills directory `${HOME}/.agents/skills/ccf-orchestration/` *only if confirmed supported by the actual local agent*. On Windows equivalent is `%USERPROFILE%\.agents\skills\ccf-orchestration\`, again subject to actual product support.
2. Read current destination/parents **without modifying**. Check for an existing `ccf-orchestration` folder, symlink, ownership conflict, protected/dirty CCF repo, other agent using that path, insufficient user-level permissions or unsupported skills feature. **STOP and ask owner for choice** rather than overwrite, delete, force-install or use sudo.
3. Use actual **user-personal** skills config, not a CCF source worktree or `Assets/` directory; never install on a system-root development worktree or edit project `AGENTS.md` as a workaround. CCF project development output may not be placed on the OS root partition; use ephemeral staging in an approved existing data-volume location when required. Do not create new CCF development worktrees.
4. Retrieve only those four published plain-text files from the GitHub package, preserving relative directories and no path traversal. Verify download content/blob identities and follow local permissions policy.
5. Atomically stage then install as `ccf-orchestration/` in the confirmed personal skills directory; do **not** overwrite preexisting personal skill files. No elevated privilege unless owner explicitly authorises it. Record actual destination generically without exposing a home path or username in GitHub result.

**Functional smoke test:**
6. Confirm files exist with correct contents and readback hashes; frontmatter parses and `agents/openai.yaml` is supported/ignored without runtime failure.
7. In a **fresh or refreshed local desktop agent session**, list available skills or invoke `$ccf-orchestration` using the product's verified mechanism. Ask only a **read-only** CCF question such as "identify which live canonical sources and current locks to consult before assigning a Multi G2 read-only source audit"; it should refer to latest `AGENTS.md`, `AgentWorkflow.md`, `active-tasks.md`, #730/#827 and separate G1 DRAFT; it must NOT write source/claim a physical grant or begin #827.
8. If the desktop client has no support for user skill folders or no permission to install, mark **BLOCKED / NOT INSTALLED**, report actual supported options and exact user UI action. Never claim "installed" based only on the file being downloaded or on a skill existing in GitHub/ChatGPT conversation.

**STOP immediately** for existing conflicting skill, lack of filesystem access, invalid package/hash, unexpected executable, unknown destination, insufficient permissions, source/worktree/physical overlap or an app requiring ungranted privilege.

## 4. Exact custody and evidence

- **Desktop local lane:** the existing user's personal skills configuration, `ccf-orchestration/` only, *no project worktree*. Actual OS/host and supported full destination resolved by the desktop worker, **not invented** here. GitHub source branch `docs/multi-831-desktop-orchestration-skill-install-packet-20261119-v1`, read-only.
- **GitHub result lane:** a *new* unique docs-only branch from freshly observed main (example `docs/multi-831-desktop-skill-install-result-20261119-v1`), exactly ONE new result `Docs/Handoffs/MultiRack-Simulation-20261116/831-orchestration/Multi-831-DESKTOP-CCF-ORCHESTRATION-SKILL-INSTALL-AND-SMOKE-TEST-RESULT-20261119.md`. Register real writer and branch before result write; one new Markdown file, fresh compare/readback and #831 completion comment. No main merge.
- **Allowed physical/user config edits:** only new files under confirmed personal `ccf-orchestration` location (assuming absent). No changes to any other skill, existing agent config, CCF repo, active tasks, Ops board/lease, source/fixtures, Unity, .NET, Windows build, security policy, dashboard or Drive.
- **Positive evidence:** intended app supports skill, exact install path, four verified file blob IDs, local discovery/invocation result, no unexpected side effects. **Negative evidence:** existing path protection, wrong ZIP, absent discovery, unsupported UI, stale source, no local access; report actual FAIL/BLOCKED honestly.
- **Handoff and review:** actual result with sanitized readback/proof and action taken, mark claim [HANDED-OFF] then close only on successful complete result or clearly documented BLOCKED if the installation cannot be completed. Manager #730 checks reported host evidence before declaring READY TO USE. Dashboard/Drive synchronization remains NOT VERIFIED.

## 5. Copy/paste dispatch — direct task for local agent

> **Task: Install and verify the CCF Orchestration skill on my desktop (user-level only).** Full GitHub handoff: https://github.com/jermur788/CCF/blob/docs/multi-831-desktop-orchestration-skill-install-packet-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-831-LOCAL-DESKTOP-CCF-ORCHESTRATION-SKILL-INSTALL-AND-SMOKE-TEST-HANDOFF-20261119.md. Read all four skill files from the GitHub package branch referenced there, inspect the real desktop product and supported personal skill path, check that no existing skill would be overwritten, register #831 RUNNING with actual provider/host/destination and safe scope, install the four text files with their verified blob hashes and demonstrate discovery/invocation in a fresh desktop session. Publish the exact one GitHub Markdown result and hand it back to Multi Manager #730. If skills are unsupported or any path is occupied, STOP and return BLOCKED with exact reason. No game source, worktree, Unity, .NET, physical Ops, main merge or other agent config changes.

**Owner action:** paste this handoff into a **local desktop agent with filesystem access**. If the agent cannot access GitHub or its local user config, user intervention may be required. Do not treat this manager packet as a performed installation.
