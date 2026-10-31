# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after Manager approval/publication of MFM-08 Integration 01 #205 and downstream exact-SHA repinning. Browser/GitHub Manager ownership remains under #200; non-browser/local/Unity orchestration remains under #201/#203.

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

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` is now **`34e360f85835afd15f5323f287372e0f2ee83dd9`**.
- MFM-08 #156 and MFM-08 Integration 01 #205 are **MANAGER APPROVED / INTEGRATED / CLOSED**. #205 Manager review: `Docs/Handoffs/MFM-08-Integration-01/MANAGER-REVIEW-20261031.md`.
- #205 exact integration was a fast-forward from `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` to `34e360f8...` using the expected old head. Returned local evidence: exact 3-commit approved replay, no conflicts/extraneous production changes, compile clean, MFM-08 `373/373`, MapPlanning `168/168`, PARITY-08 `23` checks, MFM-08 offline `297/0`, clean lane/Editor/lock return. Verification-only stale v23->v24 pin reconciliation was not absorbed into production.
- Rack V2 / D-064 remains **VERIFIED for feature scope**. PARITY-08 #126 and S1-D T-05 #151 remain **APPROVED / INTEGRATED / CLOSED**.
- #196 MapPlanning reconciliation remains **APPROVED / CLOSED**; #198 guided-v2 v24/D-066 reconciliation remains **APPROVED / CLOSED**.
- S1-C T-07 #187 remains **MANAGER APPROVED / CLOSED**. Accepted genuine v20-y5 SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- **#206 S1-C T-08 is RELEASED / READY and is the preferred next serialized Unity action.** Exact start `34e360f8...`. Binding amendment: `Docs/Handoffs/S1-C-Implementation-08/RELEASE-REPIN-20261031.md`. Qualification only; no production repair authority.
- **S1-D T-08 is repinned to `34e360f8...` but waits on #206 Manager acceptance.** Amendment: `Docs/Handoffs/S1-D-Implementation-08/REPIN-20261031.md`. S1-D T-09 remains browser/read-only independent review after T-08 PASS.
- **MFM-09 claim #207 is RELEASED / READY / QUEUED** on exact `34e360f8...`. Binding amendment: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/RELEASE-REPIN-20261031.md`. It must not displace #206's preferred next Unity slot or silently move the published integration lineage ahead of the established S1-C/S1-D qualification sequence without Manager reconciliation.
- **#199 Combined HUD / Focus Option 1 is REPINNED / READY / QUEUED** on exact `34e360f8...`. Amendment: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-REPIN-20261031.md`. MFM-08 changed `TreeInspectionView.cs`, so #199 must start fresh on the new lineage and preserve MFM-08 behavior rather than cherry-picking stale production hunks. #199 remains required pre-Tier-3.
- #200 browser umbrella remains **RUNNING**. #202 Browser Manager Resume 01 is **COMPLETE / CLOSED**.
- #201 local umbrella remains active; #203 Local Deep Orchestration Resume 02 is **RUNNING** and has been instructed to take #206 next.
- Old umbrellas #162/#106 remain SUPERSEDED/CLOSED. Verification Harness Hygiene #193 remains APPROVED/INTEGRATED/CLOSED.
- **Research Source Consolidation Audit #194 remains NO ACTION.** Do not review, approve/reject, merge, update, clean up, reorganise Drive/Project sources or otherwise act on it unless the user explicitly lifts the restriction.
- MFM-10 waits on accepted/integrated MFM-09 and must return `SPECIES GUARD QUALIFIED`; PARITY-09 waits on that token; MFM-11 then MFM-12 follow.
- Scenario One Tier-3/player-test candidate remains **NOT RELEASED**. Required accepted systems must converge on one exact candidate; #199 must be green while the protected Combined contract remains applicable.
- Dashboard publication beyond this GitHub coordination surface is **not verified in the current browser toolset**. Never manually edit generated dashboard state.

## Live / prepared tasks

| Task | Lane | Exact branch / base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, coordination, repins, claims, this file | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | non-browser orchestration | child lanes only | local routing + evidence | **RUNNING — NEXT CHILD #206** |
| **S1-C T-08 final history qualification** (#206) | #203 child; serialized Unity | exact start `34e360f8...`; `qual/s1-c-review-history-v1` suggested | qualification only | **READY — NEXT UNITY PRIORITY** |
| **S1-D T-08 multi-cycle witness** | #203 child after #206 | exact candidate `34e360f8...` | qualification only | **REPINNED — WAITING #206 APPROVAL** |
| **S1-D T-09 independent review** | #200 browser/read-only | no production lane | review only | **WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** (#207) | #203 local | exact start `34e360f8...`; `feature/mfm-09-teaching-workplan-integration-v1` | teaching/Work Plan integration | **READY / QUEUED — DO NOT DISPLACE #206 UNITY SLOT** |
| **Combined HUD / Focus Repair** (#199) | #203 local + independent review | exact start `34e360f8...`; fresh branch from current lineage | bounded Option-1 presentation/focus repair | **REPINNED / READY / QUEUED PRE-TIER3** |
| **MFM-10 species-parity guard** | #203 local | repin after accepted MFM-09 | verification/guard | **WAITING MFM-09** |
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
Substantive local work uses child claims. Read this file before dispatch and return coordination changes to #200. Maintain adaptive-routing evidence. DeepSeek default where capable; Sonnet 5.5 Medium harder-task escalation; Opus only after classified capability/reasoning failure or unusually high-consequence ambiguity.

### #206 S1-C T-08
Qualification-only. Use exact current candidate `34e360f8...` and the accepted genuine v20-y5 fixture. If a production defect appears, stop for a separate repair claim. Preserve Reference Future v1 / Legacy compatibility and history read purity.

### S1-D T-08
Do not start until #206 is Manager-accepted. Qualification-only; no production fix authority. If the published integration head moves before start, #200 must repin again.

### #207 MFM-09
Released on `34e360f8...`, but scheduling remains subordinate to the current S1-C/S1-D critical path and ownership/Unity serialization. Preserve all existing teaching IDs, v24 D-066 latch, MFM-08 review UX, Pending-only prescription approval and Work Plan commitment boundary.

### #199 Option 1
Start fresh from `34e360f8...`, not the stale partial production lineage. Preserve ordinary-inspection minimap, accepted PARITY-07 estimate/prompt/detail requirements, protected Combined assertions and MFM-08 `TreeInspectionView` behavior; repair 1920 competitor clipping through bounded reflow. Independent review required. No Rack/MFM/save/schema/anchor/reference/ecology/economy change.

### Research source consolidation
**NO ACTION.** Do not act on #194 unless the user explicitly lifts the restriction.

## Dependency-ready sequence

1. #203 executes #206 S1-C T-08 after fresh local preflight.
2. #200 immediately reviews #206. If approved, release/start S1-D T-08 on the exact still-current lineage; if the integration SHA has moved, repin first.
3. S1-D T-08 PASS -> #200 independent S1-D T-09 conformance review.
4. MFM-09 #207 is already exact-SHA ready; schedule implementation/qualification without violating S1-C/S1-D ownership/Unity sequencing, then Manager review/integration.
5. #199 Option 1 is exact-SHA ready and remains required pre-Tier-3; schedule when it does not disrupt the critical path.
6. MFM-09 accepted/integrated -> repin/release MFM-10 -> require `SPECIES GUARD QUALIFIED`.
7. MFM-10 green -> PARITY-09 and MFM-11 become eligible; MFM-11 accepted -> MFM-12.
8. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 + #199 closure accepted/integrated on one exact candidate -> Scenario One Tier-3.
9. Tier-3 success -> user personally playtests the same exact candidate.
10. Windows build only after explicit post-playtest user authorisation.

## Durable current records

- #205 integration review: `Docs/Handoffs/MFM-08-Integration-01/MANAGER-REVIEW-20261031.md`.
- #206 release: `Docs/Handoffs/S1-C-Implementation-08/RELEASE-REPIN-20261031.md`.
- S1-D T-08 repin: `Docs/Handoffs/S1-D-Implementation-08/REPIN-20261031.md`.
- MFM-09 release: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/RELEASE-REPIN-20261031.md`.
- #199 Option 1 repin: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-REPIN-20261031.md`.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
