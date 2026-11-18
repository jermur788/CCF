# Multi #730 / #790 — Recover confined source-writer model routing and enforce external-drive storage

**18 November 2026. READY / NOT RUNNING — ADMISSION REMEDIATION ONLY, NO SOURCE PERMIT.**

## Purpose and exact outcome

Remove the *technical* blockers to the standalone local-only Year0 2D viewer under [#784](https://github.com/jermur788/CCF/issues/784): authenticate the intended GPT-6.1 Sol Core and Claude Sonnet 5.5 Viewer models **inside the existing confined whole-agent launcher**, without exposing credentials, and ensure subsequent substantial work uses **ZX20 or the existing Files worktrees, never the root drive**. Return an empirical two-model PASS/FAIL and storage-policy receipt to Manager #730. This is a bounded recovery, not another general sandbox audit, not viewer development, not acceptance of R1–R4.

**Difficulty MEDIUM–HIGH:** distinct provider credential routing, frozen launcher preservation, evidence without token leakage and exFAT/ext4 storage constraints. **Suggested executor:** native DeepSeek V4.1 Flash (OpenCode/T3) admission-only parent on jer-laptop, with a separate nonauthor GPT-6.1 Sol/Claude witness *only where already available without changing the source boundary*; if intended providers cannot authenticate, an independent DeepSeek synthetic witness can verify what is actually observable. No GPT-6.2 even as fallback.

## Binding evidence and current state

- [#784 S0 read-only admission result](https://github.com/jermur788/CCF/blob/docs/multi-784-year0-viewer-admission-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/784-orchestration/Multi-784-DEEPSEEK-YEAR0-2D-VIEWER-IMPLEMENTATION-PARENT-RESULT-20261118.md) — physical readiness PASS, root capacity concern, models all fail in restricted launcher, no B/F/C/A/Q or live writer leases.
- [#779 actual whole-process confinement result](https://github.com/jermur788/CCF/blob/docs/multi-730-779-deepseek-g1b-confinement-result-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-779-ENFORCED-AGENT-TOOL-CONFINEMENT-RESULT-20261118.md), [Manager's G1b disposition](https://github.com/jermur788/CCF/blob/docs/multi-730-779-manager-g1b-decision-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-779-G1B-MANAGER-VERDICT-AND-VIEWER-REENTRY-20261118.md) — launcher v1.9 SHA-256 64056e1fe167420070ba52953eb87ea8b2c31be0c4d8526b7dccef48906665c6; policy SHA-256 a8a2a5df9f5d568fb1436e62379750eb744333a48d68a22a92be7e4dc7de7f3d; G1b has R1 provider token, R2 destructive worktree tool, R3 symlink bypass, R4 OS network residuals. No final G2 grant.
- [#784 full implementation packet](https://github.com/jermur788/CCF/blob/docs/multi-730-784-year0-viewer-confined-cli-resume-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-784-YEAR0-2D-VIEWER-CONFINED-CLI-IMPLEMENTATION-ORCHESTRATION-PACKET-20261118.md). The owner requested **evaluation**, not affirmative acceptance, of security Option B; this recovery cannot sign on owner's behalf.
- [Owner's binding storage correction on #784](https://github.com/jermur788/CCF/issues/784#issuecomment-6992616861): no root-drive clones/worktrees/source builds/large scratch/cache; prefer ZX20 for capacity with exFAT caution; existing H0-A Core and Viewer worktrees on /media/jer/Files are reserved and must not be moved without authorised physical handoff.
- [#730](https://github.com/jermur788/CCF/issues/730), [#784](https://github.com/jermur788/CCF/issues/784) and live coordination/Ops before any meaningful local change.

At S0 the **actual** confined tests were:
| Route | Observation |
|---|---|
| openai/gpt-6.1-sol | HTTP 401 |
| anthropic/claude-sonnet-5-5 | invalid x-api-key |
| opencode/gpt-6.1-sol and opencode/claude-sonnet-5-5 | Reconnect OpenCode Console |
| opencode-go/deepseek-v4.1-flash | PASS positive control, not an authorised substitute |
The root filesystem was reported at capacity even as Files had 27 GiB and ZX20 45 GiB free. These are S0 observations, not fresh checks.

## Stage 0 — Register, inspect, no secret exposure

1. Register [RUNNING — AUTH/STORAGE RECOVERY ONLY] on [#790](https://github.com/jermur788/CCF/issues/790), actual provider/model/harness/host/session, starting main HEAD, exclusive docs-only branch and one final result Markdown. Parent remains an admission actor, not a source writer. Inspect live coordination and physical Ops **read-only**. Do not modify /media/jer/ZX20/CCF-Ops or active-tasks unless assigned owner.
2. Refresh free space / mount filesystem for root, /media/jer/Files and /media/jer/ZX20. Confirm current H0-A Core and Viewer worktrees are clean and leased to nobody. No clone, no copy of a whole repository and no root temporary spill. If a check itself needs scratch, use a **named, disposable, narrow scratch directory on /media/jer/Files** (POSIX semantics) or ZX20 for non-POSIX large logs; secure it, remove only own scratch after evidence is durably preserved. Do not execute commands that clean someone else's root data.
3. Inspect provider account/session status and **safe metadata only** for OpenAI, Anthropic and OpenCode Console. Never print, commit, log, paste or transmit API keys, provider tokens, auth.json contents, cookies or connection secrets; do not include them in model prompts or GitHub comments. Record only status, provider names, credential source categories, effective isolation, redacted HTTP error details and exact scope. Never use an owner's full host credentials inside an unrestricted agent merely to test.

## Stage 1 — Least-change model authentication repair

**Preferred: restore the originally intended provider routes under the unchanged pinned launcher.** This is a user/provider-session action if authentication needs an interactive sign-in; STOP and ask the user for the exact native application/UI connection step rather than requesting a raw secret in chat. Distinguish expired/rejected key from model entitlement/not available, and verify actual model identifier. Do not overwrite global OpenCode config or log out of an existing shared account without express owner permission. Do not copy general ~/.config/gh, SSH or personal credential stores into the sandbox.

Allowed **without separate host change consent:** read-only diagnostics, safe redacted provider enumeration and a re-run of the existing synthetic positive/negative controls under the frozen policy/launcher using only already-authorised credential paths. A provider-auth **mutation**, re-login affecting a shared profile, credential mount addition, machine-wide config update, changed launcher/policy, new host service or package installation requires specific owner/host approval before execution; explain the proposed exact path and effect.

If OpenAI or Anthropic cannot run under frozen launcher after a legitimate supported reauthentication: publish a decision table, not a silent workaround. Alternatives in priority order:
- (1) Exact owner-performed provider reconnection/re-sign-in then re-test same frozen launcher; preferred.
- (2) New whole-process launcher for existing authenticated Codex CLI / Claude Code, requiring a distinct scope/credential mount decision, independent synthetic security requalification and Manager approval before using as a production writer.
- (3) Specifically named model substitution, shown to work in genuinely confined writer environment and separately approved by Manager/owner. DeepSeek positive control is **not automatic permission** to replace both required writers.
Never invoke unrestricted T3-delegated workers as source writers. Never use GPT-6.2.

## Stage 2 — Enforce the user's external-drive storage boundary

**No new source worktrees, repository clones, large caches, build/test output, downloaded dependencies or heavy scratch on root.** Before invoking any CLI tests, inspect **actual writable mount, permissions, available space, and exFAT semantics**. This applies to process environment and native tools, not only files you explicitly create.

- **Files ext4**: the two already reserved Core and Viewer worktrees remain exactly where they are. Use an isolated new, narrow **Files** POSIX scratch path for credential-sensitive or tools requiring Unix permissions/symlinks; it is not a new source lane or an Ops lease. Do not create new worktrees here unless separately authorised.
- **ZX20 exFAT**: prefer for large non-sensitive evidence/logs/download staging, subject to actual filesystem-compatibility and Ops ownership. A Git worktree, symlinked sandbox, chmod-sensitive auth or Unix test harness must **not** be blindly moved onto exFAT. Under no circumstance write to its live Ops paths without a separate owner.
- **Root**: OS binaries and unavoidable pre-existing small runtime metadata are not automatically forbidden, but do not create *new substantial* repository/build/cache/scratch data there. Redirect TMPDIR, XDG_CACHE_HOME, XDG_CONFIG_HOME **only where safe and without breaking protected auth**, language tool caches, agent archive/log paths, package caches and test temp/output to approved external directories. Verify effective placement with a synthetic small-file probe and runtime inspection, not an environment declaration alone.
- Check that the pinned whole-executor bwrap launcher sees the external scratch/cache destinations through exact permitted mounts. **Do not modify its mounts or policy** as part of this task; if the current launcher cannot use the external scratch destination, record blocker and propose bounded launcher requalification, rather than shifting files to root.
- Root is near capacity; **no cleanup** of third-party files, no moving unrelated work, no stash/reset/clean, no root storage reclamation without explicit approval. If size threshold remains unknown, enforce the strict prohibition on new repo/build scratch and report each unavoidable small root write precisely.

## Stage 3 — Exact acceptance test and handoff

Perform only synthetic no-source, no-fixture checks with **both** intended model routes inside the same unchanged pinned whole-process launcher. For each actor, record PASS/FAIL and:
1. Model genuinely responds through the configured provider while no credentials appear in output.
2. Outside sentinel **read AND write blocked**, inside disposable lane read/write allowed.
3. No shell/webfetch/websearch/Drive/subagent tool present; do not call destructive worktree_remove, but record if it remains listed as known R2.
4. Exact frozen launcher and policy digest still match #779 before and after; all host secret mounts absent except existing model transport credentials; no new credential class or unreviewed route.
5. No additional root worktree/cache/build artifacts created; actual temp/log/cache output locations demonstrated on external mounts.
6. If any model cannot be made to respond, declare **BLOCKED** for that model and specify exactly the user-visible connection step or separate approved alternative. A successful DeepSeek-only control does **not** count as GPT/Claude PASS.

Publish one result at:
Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-790-CONFINED-CLI-MODEL-AUTH-AND-STORAGE-RECOVERY-RESULT-20261118.md
on a separately registered exclusive docs-only branch. Include actual actor identity, observations, redacted diagnostic table, mount/storage receipt, per-model outcome and bounded required owner action. GitHub readback/one-file compare, post [HANDED-OFF] to #790 and #730.

## Gate discipline

**If Stage 3 passes**, #790 is **READY FOR MANAGER RE-ENTRY REVIEW** of #784, NOT automatic writer start. Risk Option B R1–R4 is **not yet explicitly signed**; physical writer leases, B protected #687 pre/post hash, F #639 fixture copy, C Core and subsequent A Viewer/Q grants all require separate deliberate authorisation. If stage fails, stop and provide one precise blocker. Source #710/#725, #687 byte contents, fixture bytes, source worktrees, main, Ops mutations, Host, Unity, Windows, public app, #770/Scenario One outside this recovery. .NET L4 FAIL38 unwaived. Dashboard/Drive synchronization unverified.

**Paste-ready dispatch:** Execute Multi #730 / #790 confined-CLI model-authentication and external-storage recovery from this full GitHub packet. Register actual DeepSeek V4.1 Flash admission-only actor, read #784 S0, test safe provider routes and owner-mandated storage boundary, and attempt only non-mutating diagnostics within frozen #779 launcher/policy. Do not accept R1–R4 for the owner or write any source. If reconnecting provider credentials or modifying mount/config is necessary, STOP with exact owner action instead of copying secrets or changing shared config. Prove actual GPT-6.1 Sol AND Claude Sonnet 5.5 confined synthetic response, outside read/write denied and no new root-drive worktree/cache. Return one redacted GitHub result and request #730 re-entry assessment. No #770/Scenario One, no GPT-6.2, no L4 waiver.
