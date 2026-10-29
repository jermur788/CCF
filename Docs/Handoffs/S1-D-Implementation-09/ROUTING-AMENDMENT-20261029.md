# S1-D T-09 — Routing Amendment (2026-10-29)

This amendment supersedes only the reviewer-routing section of `TASK-PACKET.md`. Review scope, evidence matrix and verdict rules are unchanged.

## Current independent-review route

Because the current T-08 route is Claude Sonnet 5.5 Medium / Anthropic by default, **T-09 must use OpenAI 6.1 Sol** to preserve cross-provider independence.

- **Reviewer:** OpenAI 6.1 Sol.
- **Mode:** read-only independent repository/evidence review.
- If OpenAI 6.1 Sol is unavailable after T-08 PASS, keep T-09 waiting; do not silently substitute the same-provider Claude executor/reviewer.

No Windows build.

## Updated dispatch line

`Run S1-D T-09 using Docs/Handoffs/S1-D-Implementation-09/TASK-PACKET.md plus ROUTING-AMENDMENT-20261029.md only after T-08 publishes PASS on one exact GitHub-resolvable candidate. Reviewer: OpenAI 6.1 Sol, cross-provider from the Claude T-08 executor. Review only: fully evidence the approved S1-D checklist and issue APPROVED / NEEDS CHANGES / REJECTED. Do not implement production changes and do not build Windows.`
