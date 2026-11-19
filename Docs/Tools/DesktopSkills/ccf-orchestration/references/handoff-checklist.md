# Durable orchestration packet checklist

For each substantive task, include:

- One-sentence **player or engineering outcome**, reason for doing it now, dependency and acceptance boundary.
- Classification: PACKET ONLY vs SPEC REQUIRED; status and *direct URL* for any required approved spec.
- Links to latest owner/canonical instructions, relevant verified source evidence, unresolved blockers.
- Actual assigned owner/model/provider/harness and capabilities; **not** a permanently assigned model family.
- Exact GitHub issue claim, actual state (READY / RUNNING / BLOCKED / HANDED-OFF), exclusive file set, branch/worktree, observed full HEAD and any required GitHub/physical lease, approval and expiry.
- Negative scope: protected fixtures/world IDs, no-work or save/oracle guards, other owners' dirty files, main/release/Unity/Windows rules.
- Concrete steps, expected positive and negative tests and proportional verification tier; never mark NOT RUN as PASS.
- Specific final GitHub Markdown path, evidence/provenance contents and Manager review recipient.
- Paste-ready agent dispatch line with a permanent GitHub handoff URL.
- Explicit STOP conditions and whether the user must do anything.

When a worker returns: inspect the exact GitHub result and source/verification evidence; issue a scoped APPROVED / NEEDS CHANGES / REJECTED verdict; update claim and produce the next executable packet only if no material blocker remains.
