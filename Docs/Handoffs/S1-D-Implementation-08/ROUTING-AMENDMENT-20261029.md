# S1-D T-08 — Routing Amendment (2026-10-29)

This amendment supersedes only the executor/reviewer routing assumption in `TASK-PACKET.md`. Scope, start gates, witness, anchors and verification rules are unchanged.

## Current routing

Per the user's current programme routing:

- **T-08 executor:** Claude Sonnet 5.5 Medium / Anthropic by default.
- **Escalate executor to Claude Opus 5.5 Low** only if the integrated multi-cycle diagnosis is materially complex and the reason is recorded.
- **T-09 independent reviewer:** OpenAI 6.1 Sol, cross-provider from the Claude T-08 executor.

If OpenAI 6.1 Sol is unavailable when T-08 returns PASS, T-09 waits rather than silently using the same provider for the mandatory independent review.

No Windows build.

## Updated dispatch line

`Run S1-D T-08 using Docs/Handoffs/S1-D-Implementation-08/TASK-PACKET.md plus ROUTING-AMENDMENT-20261029.md only after T-02..T-06 are accepted on one exact current integration candidate. Use Claude Sonnet 5.5 Medium by default; escalate to Opus 5.5 Low only with a recorded reason. Run the three-cycle/idle-year/stale-order/save-load/completion/post-completion/D-057 witness and protected regressions. Verification-only; no production fixes, no rebaseline, no Windows build.`
