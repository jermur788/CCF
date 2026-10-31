# S1-C T-08 — Release / Exact-Candidate Repin — 2026-10-31

**Execution claim:** #206  
**Parent:** #91 / browser #200 / local orchestration #201/#203  
**Base packet:** `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`  
**Status:** **RELEASED — NEXT SERIALIZED UNITY QUALIFICATION**

This release amendment supersedes the stale status/start-gate text in the base packet that waited for #156/OPS #189 and referenced `ce673714...`. All substantive qualification requirements, protected gates, evidence requirements, stop conditions and success token in the base packet remain binding unless explicitly amended here.

## Exact released candidate

`integration/scenario-one-player-test-ready-v1` = **`34e360f85835afd15f5323f287372e0f2ee83dd9`**.

This exact candidate contains Manager-approved/integrated MFM-08 #156/#205 plus the previously accepted Scenario One lineage. The #205 integration review is `Docs/Handoffs/MFM-08-Integration-01/MANAGER-REVIEW-20261031.md`.

## Cleared prerequisites

- OPS lock-governance repair #189 is approved/closed; normal fresh lock preflight remains mandatory.
- S1-C T-07 #187 is Manager-approved/closed.
- MFM-08 #156 and integration #205 are Manager-approved/integrated/closed.
- The serialized Unity slot is no longer reserved by #205; actual availability must still be observed immediately before acquisition.

Accepted genuine old-save fixture remains v20-y5 SHA-256:
`af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.

Do not substitute the rejected synthetic `v20-records` fixture.

## Ownership and execution

Qualification-only. No production repair authority. Suggested branch/worktree remain:

- `qual/s1-c-review-history-v1`
- `/media/jer/Videos/CCF-S1C-T08`

Before starting, #203 must refresh live `active-tasks.md`, #206, the exact integration ref, worktree cleanliness, free space, Editor processes and the authoritative Unity lock. Mark #206 `[RUNNING]` with actual provider/model, branch/worktree, exact starting SHA and lock receipt only at actual start.

## Required result

Run the complete base-packet Tier-2 matrix at 1280x720 and 1920x1080, including multi-year history, earlier/latest navigation, drill-down, grouped-harvest honesty, no-work year, provenance, guidance, objective history, current-schema save/load, the accepted genuine v20-y5 old save, read-only/nonmutation and protected Reference Future/Legacy compatibility.

Publish `Docs/Handoffs/S1-C-Implementation-08/HANDOFF.md`.

Success token: `S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`.

## Dispatch

`Run S1-C T-08 under claim #206 using Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md plus RELEASE-REPIN-20261031.md. Start only from exact published integration 34e360f85835afd15f5323f287372e0f2ee83dd9 after fresh ownership/free-space/Editor/Unity-lock preflight. Qualification only: run the full multi-year/year-browser/drill-down/grouped-harvest/no-work/provenance/guidance/objective-history/current-save/genuine-v20-y5-old-save/rendered matrix at 1280x720 and 1920x1080. Use old-save SHA af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00. Do not repair production or weaken gates. Publish HANDOFF.md and return the exact success token or blocker. No Windows build.`

User action required: none.
