# S1-C T-07 — Repin Handoff

**Date:** 2026-10-30  
**Claim:** #187  
**Status:** **READY — OFFLINE; NOT RUNNING**

## Purpose

Repin the already-released S1-C T-07 data-contract/stability verification to the current accepted Scenario One integration lineage after S1-D T-05 #151 integration.

## Exact lineage

- Previous T-07 base: `8fa83bff22ea1b4943689bee1074eed8eae65d2f`.
- Current Scenario One integration/docs successor: **`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`**.
- Delta audit: `8fa83bff...` → `ce673714...` changes only accepted S1-D T-05 UI/presentation files, its focused verification harness and handoff docs. It does not alter the S1-C history query/identity helper or save/schema/validator/hash contract under T-07.
- Repinned task packet: `Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md`, main publication `7a73955725261eb41115e20fea894fb25455764b`.

## Routing

- **Difficulty:** Low/Medium — focused offline identity/read-purity/old-save verification.
- **Best-suited model/provider:** SOL/CODEX high-reasoning verification lane or Claude Sonnet-class Code lane.
- **Branch/worktree:** `verify/s1-c-t07-data-contract-v1` / `/media/jer/Videos/CCF-S1C-T07`.
- **Claim:** #187 remains READY, not RUNNING. At actual start, update it `[RUNNING — OFFLINE]` with model/provider, branch, worktree and exact start SHA.

No Unity is required by default. If faithful verification unexpectedly needs Unity or production edits, stop and return the exact blocker.

## Copy/paste dispatch line

`Run S1-C T-07 from Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md starting from exact ce6737146a7aa9ea953d7db0ca93b103f6c2c996 after fresh active-tasks/claim/worktree preflight. Verification/docs only: stable event/work-order identities, append-only history meaning, harvestJobId resolution-year limitation, genuine supported old-save behaviour, read purity and negative controls. Do not change save/schema/UI or take Unity automatically. Publish HANDOFF.md. No Windows build.`

## User action required

None.
