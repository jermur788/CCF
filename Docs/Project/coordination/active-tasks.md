# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after S1-D T-08 #208 qualification and independent T-09 #209 APPROVED, with Overall Dev cross-project sync enabled. Browser/GitHub Manager ownership remains under #200; non-browser/local/Unity orchestration remains under #201/#203.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 is the sole primary writer for this file during the current two-lane split.** #201/#203 and child workers refresh/read it but do not become competing writers.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, Editor processes and the authoritative lock immediately before acquisition.
- A historical `free` lock receipt never reserves or proves future availability.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.
- **Overall Dev sync:** after meaningful project-state changes, #200/Overall Manager refreshes `jermur788/Overall-Dev/updates/ccf/latest.md` per `Docs/Project/coordination/overall-dev-sync.md`. Keep it concise and coordination-only; CCF remains canonical.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` remains **`34e360f85835afd15f5323f287372e0f2ee83dd9`**.
- MFM-08 #156 and integration #205 remain **MANAGER APPROVED / INTEGRATED / CLOSED**.
- Rack V2 / D-064 remains **VERIFIED for feature scope**. PARITY-08 #126 and S1-D T-05 #151 remain **APPROVED / INTEGRATED / CLOSED**.
- #196 MapPlanning and #198 guided-v2 v24/D-066 reconciliations remain **APPROVED / CLOSED**.
- S1-C T-07 #187 remains **APPROVED / CLOSED**; genuine v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00` remains the accepted old-save fixture.
- S1-C T-08 #206 remains **APPROVED / FEATURE QUALIFIED / CLOSED** on exact `34e360f8...`.
- **S1-D T-08 #208 is APPROVED / FEATURE QUALIFIED / CLOSED** on exact production candidate `34e360f8...`. Returned witness: 4,413 checks / 0 failures, 9 cycle rows, 15 disk save/load checkpoints, 24 captures, cross-session resume PASS, protected anchors unchanged and no production/rebaseline change.
- **S1-D T-09 #209 independent conformance review is APPROVED / CLOSED.** Durable review: `Docs/Handoffs/S1-D-Implementation-09/INDEPENDENT-REVIEW-20261031.md`. S1-D repeated-interventions V1 is feature-scope verified/accepted on `34e360f8...` for the reviewed evidence.
- S1-D non-blocking residuals: the T-08 D-057 path stayed solvent but thin at €1,336.04 after post-completion work; another harvest is correctly refused by the ordinary €2,500 cash rule. Retain this as a calibration/player-experience risk for playtest, not a defect or coefficient-change authority. Stale T-03/T-06 mutant anchors and the old S1-C T-05 offline v22 pin are verification debt.
- **MFM-09 #207 is RELEASED / READY and is now the preferred next local programme task** on exact `34e360f8...`.
- **#199 Combined HUD / Focus Option 1 remains REPINNED / READY / QUEUED** on exact `34e360f8...`; it remains required pre-Tier-3.
- #200 browser umbrella remains **RUNNING**. #201/#203 local orchestration remains **RUNNING**.
- Old umbrellas #162/#106 remain SUPERSEDED/CLOSED. Verification Harness Hygiene #193 remains APPROVED/INTEGRATED/CLOSED.
- **Research Source Consolidation Audit #194 remains NO ACTION.** Do not review, approve/reject, merge, update, clean up, reorganise Drive/Project sources or otherwise act on it unless the user explicitly lifts the restriction.
- MFM-10 waits on accepted/integrated MFM-09 and must return `SPECIES GUARD QUALIFIED`; PARITY-09 waits on that token; MFM-11 then MFM-12 follow.
- Scenario One Tier-3/player-test candidate remains **NOT RELEASED**. Required accepted systems must converge on one exact candidate and #199 must be green before final Tier-3.
- **Overall Dev coordination publication is now verified:** CCF maintains `jermur788/Overall-Dev/updates/ccf/latest.md`. Actual Dashboard ingestion/rendering of that coordination data is not yet independently verified from this browser lane.

## Live / prepared tasks

| Task | Lane | Exact branch / base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, coordination, repins, claims, this file | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | non-browser orchestration | child lanes only | local routing + evidence | **RUNNING — NEXT CHILD #207** |
| **S1-C T-08 final history qualification** (#206) | completed #203 child | production `34e360f8...` | qualification only | **APPROVED / FEATURE QUALIFIED / CLOSED** |
| **S1-D T-08 multi-cycle witness** (#208) | completed #203 child | production `34e360f8...`; verification branch `verify/s1-d-multicycle-witness-v1` | qualification only | **APPROVED / FEATURE QUALIFIED / CLOSED** |
| **S1-D T-09 independent review** (#209) | #200 browser/read-only | exact candidate `34e360f8...` | independent review only | **APPROVED / CLOSED** |
| **MFM-09 teaching / Work Plan integration** (#207) | #203 local | exact start `34e360f8...`; `feature/mfm-09-teaching-workplan-integration-v1` | teaching/Work Plan integration | **READY — NEXT LOCAL PRIORITY** |
| **Combined HUD / Focus Repair** (#199) | #203 local + independent review | exact start `34e360f8...`; fresh branch from current lineage | bounded Option-1 presentation/focus repair | **REPINNED / READY / QUEUED PRE-TIER3** |
| **MFM-10 species-parity guard** | #203 local | repin after accepted/integrated MFM-09 | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | #200 repin then #203 local | TBD | qualification | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | #203 local + independent review | repin after MFM-10 | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | #203 local + independent review | repin after MFM-11 + applicable lineage | integrated verification | **WAITING MFM-11** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #203 local | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | `verify/parity-03-density-continuity-q10-v1` | verification only | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves the Unity lock is currently free. Every Unity worker must fresh-preflight/acquire through the authoritative tool.

## Single-writer / task boundaries

### #200 browser Manager
Browser/GitHub only and sole writer of this file. Owns Manager decisions, browser/static review, exact-candidate repins and status. No local C#/Unity implementation or unreturned runtime claims.

### #201 / #203 local orchestration
Substantive local work uses child claims. Read this file before dispatch and return coordination changes to #200. Maintain adaptive-routing evidence. DeepSeek is the orchestrator/default worker for bounded/easier work; Sonnet 5.5 Medium is the genuinely harder-task escalation; Opus only after classified capability/reasoning failure or separately justified high-consequence need.

### #207 MFM-09
Start from exact `34e360f8...` only after fresh live ownership/branch/worktree/Editor/free-space/Unity-lock preflight. Preserve all existing teaching IDs, v24 D-066 latch, MFM-08 review UX, Pending-only prescription approval and Work Plan commitment boundary. Scope remains the approved `Extend your treatment` teaching / Work Plan integration; no silent overlap expansion. If fresh inspection requires an unowned MFM-08 review-file or ScenarioOneUiRoot production edit beyond the packet, stop for Manager overlap disposition.

### #199 Option 1
Start fresh from exact current integration when scheduled, not stale partial work. Preserve ordinary-inspection minimap, accepted PARITY-07 estimate/prompt/detail requirements, protected Combined assertions and MFM-08 `TreeInspectionView` behavior; repair 1920 competitor clipping through bounded reflow. Independent review required. No Rack/MFM/save/schema/anchor/reference/ecology/economy change.

### Research source consolidation
**NO ACTION.** Do not act on #194 unless the user explicitly lifts the restriction.

## Dependency-ready sequence

1. #203 executes **MFM-09 #207** next after fresh local preflight; route according to difficulty and the DeepSeek-first orchestration policy.
2. #200 reviews returned #207 evidence and integrates only if exact evidence warrants it.
3. Accepted/integrated MFM-09 -> #200 repins/releases MFM-10 -> require `SPECIES GUARD QUALIFIED`.
4. MFM-10 green -> PARITY-09 and MFM-11 become eligible; MFM-11 accepted -> MFM-12.
5. #199 Option 1 remains exact-SHA ready and required pre-Tier-3; schedule without creating overlapping UI ownership with MFM work.
6. Rack V2 G4 + PARITY-09 + accepted S1-C + accepted S1-D + MFM-12 + #199 closure accepted/integrated on one exact candidate -> Scenario One Tier-3.
7. Tier-3 success -> user personally playtests the same exact candidate.
8. Windows build only after explicit post-playtest user authorisation.

## Durable current records

- Overall Dev sync policy: `Docs/Project/coordination/overall-dev-sync.md`; shared CCF status: `jermur788/Overall-Dev/updates/ccf/latest.md`.
- #205 integration review: `Docs/Handoffs/MFM-08-Integration-01/MANAGER-REVIEW-20261031.md`.
- #206 S1-C T-08 Manager review: `Docs/Handoffs/S1-C-Implementation-08/MANAGER-REVIEW-20261031.md`.
- #208 S1-D T-08 handoff: `Docs/Handoffs/S1-D-Implementation-08/HANDOFF.md` on final docs lineage `605ce465...`.
- #209 S1-D T-09 independent review: `Docs/Handoffs/S1-D-Implementation-09/INDEPENDENT-REVIEW-20261031.md`.
- MFM-09 release: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/RELEASE-REPIN-20261031.md`.
- #199 Option 1 repin: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-REPIN-20261031.md`.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
