# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after completing Browser Manager Resume 01 #202. Browser/GitHub Manager ownership remains under #200. Non-browser/local/Unity orchestration remains under #201 with resume #203. #202 is complete/closed. #205 is the exact next local action. #199 Option 1 is authorised but waits for a post-#205 repin before production edits.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 is the sole primary writer for this file during the current two-lane split.** #201/#203 and child workers refresh/read it but do not become competing writers.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Fresh-preflight free space, lock and Editor-process state immediately before acquisition.
- A historical `free` lock receipt never reserves or proves future availability.
- Browser-capable work belongs to #200. Local/worktree/Unity execution belongs to #201/#203 through substantive child claims.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` remains **`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`**. MFM-08 is **not integrated** yet.
- Rack V2 / D-064 remains **VERIFIED for feature scope**.
- PARITY-08 #126 remains **INDEPENDENTLY APPROVED / INTEGRATED / CLOSED**.
- S1-D T-05 #151 remains **APPROVED / INTEGRATED / CLOSED**.
- S1-C T-07 #187 remains **MANAGER APPROVED / CLOSED**. Accepted genuine v20-y5 SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- OPS Unity-lock repair #189 remains **MANAGER APPROVED / CLOSED**. Every later Unity task still fresh-preflights/acquires normally.
- MFM-08 Geometry Repair #192 remains **MANAGER APPROVED / CLOSED**. Exact repaired production head `e5fb424635f16182748efcb0539e8a49539495a9`.
- **#196 MFM-08 MapPlanning Contract Reconciliation is MANAGER APPROVED / CLOSED.** Verification-only patch `eef1bdf9`; full authoritative gate `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`. Original five tabs remain asserted by name/order; V2 `Treatment review` is additive; historical/non-V2 five-tab branch remains explicit. Review: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- **#198 Guided-v2 v24 / D-066 Contract Reconciliation is MANAGER APPROVED / CLOSED.** guided-v2 535/0/48; guided-legacy 569/0/62. Preserved first legacy real-input timing failure is non-blocking environment/runner debt. Review: `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- **#156 MFM-08 is MANAGER APPROVED / READY FOR INTEGRATION, not integrated.** Exact approved production candidate `e5fb424635f16182748efcb0539e8a49539495a9`; accepted feature evidence remains 353/353 plus strengthened 373-check conflict gate at both render sizes. Final review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-APPROVAL-20261031.md`.
- **#205 MFM-08 Integration 01 is READY — NEXT LOCAL ACTION.** Start from exact integration `ce673714...`; replay only approved delta `cfdc93bf3943125d21570bc5749a9338f52d1cce..e5fb424635f16182748efcb0539e8a49539495a9`; do not silently absorb #196 `eef1bdf9` or #199 partial work. Packet: `Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md`.
- Browser overlap review for #205 found the integration-side 29 commits after merge base `cfdc93bf...` do not modify MFM-08's two pre-existing production paths `Assets/ForestPrototype/UI/Map/StandMapView.cs` or `Assets/ForestPrototype/UI/TreeInspectionView.cs`; the remaining MFM production paths are additions. This supports release but does not prove local post-replay compile/runtime correctness.
- **#199 Combined HUD / Focus Regression is OPTION 1 AUTHORISED — WAITING POST-#205 REPIN.** Partial branch `42742a8a` is fixture-only; observed partial gates are combined-v2 453/10/24 and combined-legacy 440/8/24. Required repair: restore minimap for ordinary estimate inspection while preserving accepted PARITY-07 estimate information/copy and central-prompt/detail clearance, preserve protected Combined assertions, and repair the real 1920 V2 competitor-list clipping using bounded responsive/reflow work. Decision: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-DISPOSITION-OPTION1-20261031.md`; continuation: `OPTION1-CONTINUATION-PACKET.md`.
- #200 Browser/GitHub Manager Lane 03 remains **RUNNING** as the browser umbrella. #202 is **COMPLETE / CLOSED** with result `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-RESUME-RESULT-20261031T1233Z.md`.
- #201 Long-Running Deep Orchestration remains the local umbrella; #203 Local Deep Orchestration Resume 02 is **READY** and has received the #205 Manager release.
- Old umbrellas #162/#106 remain SUPERSEDED / CLOSED. Verification Harness Hygiene #193 remains APPROVED / INTEGRATED / CLOSED.
- **Research Source Consolidation Audit #194 remains NO ACTION.** Do not review, approve/reject, merge, update, clean up, reorganise Drive/Project sources or otherwise act on it unless the user explicitly lifts the restriction.
- S1-C T-08 is dependency-cleared on T-07 but not released to Unity yet. It remains the preferred history qualification slot after the current MFM integration step unless live evidence requires reprioritisation.
- S1-D T-08 remains released but was pinned to `ce673714...`; after #205 changes the integration lineage, #200 must determine whether a fresh repin is required before execution.
- S1-D T-09 remains prepared and waits for S1-D T-08 PASS.
- MFM-09 waits for Manager-accepted/integrated MFM-08 via #205 and then requires a fresh exact-SHA repin.
- MFM-10 waits for accepted/integrated MFM-09 and must return `SPECIES GUARD QUALIFIED`.
- PARITY-09 remains blocked on MFM-10 `SPECIES GUARD QUALIFIED`.
- MFM-11 waits for MFM-10 and must resolve the MFM-03 live/cross-year residual.
- MFM-12 waits for MFM-11 plus the applicable accepted integrated lineage.
- Scenario One Tier-3/player-test candidate remains not released. #199 must be green before final Tier-3 while its protected Combined contract remains applicable. #198 is no longer an open blocker.
- Dashboard publication beyond this GitHub coordination surface is **not verified in the current browser toolset**. Never manually edit generated dashboard state; if the publisher is in use, dashboard sync remains an explicit operational follow-up.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, coordination, repins, claims, active-tasks writes | **RUNNING — UMBRELLA** |
| **Browser Manager Resume 01** (#202) | browser/GitHub | no local worktree | bounded verdict/release task | **COMPLETE / CLOSED** |
| **Local Deep Orchestration Resume 02** (#203; parent #201) | long-running non-browser | child worktrees only | dispatch/routing + orchestration evidence | **READY — #205 RELEASED NEXT** |
| **MFM-08** (#156) | integration hold | approved candidate `e5fb4246...` | no new feature edits | **MANAGER APPROVED — WAITING #205** |
| **MFM-08 Integration 01** (#205) | #203 child: local integration + focused Unity | `integration/mfm-08-onto-scenario-one-v1`; dedicated worktree assigned at start | exact approved MFM replay only | **READY — FIRST LOCAL ACTION** |
| **Combined HUD / Focus Repair** (#199) | #203 child after repin | stale partial `42742a8a`; exact new base TBD after #205 | bounded Option-1 presentation/focus repair | **AUTHORISED — WAITING POST-#205 REPIN** |
| **S1-C T-08 final history qualification** | #203 local Tier-2; #200 repins/releases | prepared only | qualification only | **T-07 CLEARED; NOT RELEASED** |
| **S1-D T-08 multi-cycle witness** | #203 local Tier-2 after preferred S1-C slot | existing `verify/s1-d-multicycle-witness-v1` | qualification only | **RELEASED OLD BASE; REPIN CHECK AFTER #205** |
| **S1-D T-09 independent conformance review** | #200 browser/read-only | no production lane | review only | **WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | #200 repin/release then #203 local | prepared only | curriculum/integration | **WAITING ACCEPTED #205** |
| **MFM-10 species-parity guard** | #203 local | prepared only | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | #200 repin then #203 local | TBD | qualification | **BLOCKED ON MFM-10** |
| **MFM-11 calibration impact** | #203 local + independent review | prepared only | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | #203 local + independent review | prepared only | integrated verification | **WAITING MFM-11 + APPLICABLE LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #203 local Tier-3 | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | `verify/parity-03-density-continuity-q10-v1` | verification only | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight and acquire through the authoritative tool.

## Single-writer boundaries

### #200 browser Manager

Browser/GitHub only and sole primary writer of this file. Owns Manager decisions, browser/static review, packets/repins and status. It owns no local C#/Unity implementation and must not claim local cleanliness, Editor/runtime/rendered evidence or build state without returned local evidence.

### #201 / #203 local orchestration

Non-browser orchestration only at umbrella level; substantive work uses child claims. Read this file before every dispatch and return coordination changes to #200. Maintain the required adaptive-routing ledger. DeepSeek is default where capable; Sonnet 5.5 Medium is harder-task escalation; Opus only after classified capability/reasoning failure or unusually high-consequence ambiguity.

### #205 MFM-08 Integration 01

Start from exact `ce673714...`; replay only `cfdc93bf...e5fb4246`. Do not absorb #196 `eef1bdf9`, #199 `42742a8a`, unrelated docs/harness commits or new behavior. Stop on unexpected conflict. Local import/compile and proportionate focused MFM/MapPlanning/save/reference/Legacy evidence are mandatory before Manager integration acceptance. No Windows.

### #199 Option 1

Manager-authorised production repair but no production start until #205 is accepted and #200 publishes the exact combined-SHA repin. Preserve both contracts: ordinary estimate inspection keeps the minimap; accepted PARITY-07 estimate content and central-prompt/detail clearance remain; protected Combined assertions remain semantically unchanged. Repair real 1920 competitor clipping via bounded responsive/reflow work. Independent review required. No Rack/MFM/save/schema/anchor/ecology/economy changes. No Windows.

### Research source consolidation

**NO ACTION.** Do not act on #194 unless the user explicitly lifts the restriction.

### S1-C T-08 / S1-D T-08

Both are qualification-only. S1-C T-08 retains the accepted genuine old-save v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. S1-D T-08's prior release used `ce673714...`; do not assume it remains exact after #205 without browser repin review.

## Dependency-ready sequence

1. #203/#201 executes **#205** after fresh ownership/worktree/Unity preflight.
2. #200 reviews #205 immediately. If approved, update exact integration truth, close/integrate #156 appropriately, and repin downstream exact-candidate work.
3. Preserve complete-loop critical path: fresh-repin/release S1-C T-08, then S1-D T-08, then browser S1-D T-09, subject to live serialization/dependencies.
4. After accepted/integrated MFM-08, fresh-repin/release MFM-09; MFM-09 -> MFM-10 -> require `SPECIES GUARD QUALIFIED`.
5. #199 Option 1 is required pre-Tier-3 but starts only after #205 and exact browser repin; do not create an overlapping UI writer or silently displace the established critical path unless it directly blocks an upcoming gate.
6. MFM-10 green -> PARITY-09 and MFM-11 become eligible.
7. MFM-11 accepted + applicable Rack/parity/S1-C/S1-D lineage -> MFM-12.
8. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 + #199 closure accepted/integrated on one exact candidate -> Tier-3.
9. Tier-3 success -> user personally playtests that same exact candidate.
10. Windows build only after explicit post-playtest user authorisation.

## Durable current records

- Browser lane/resume: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-HANDOFF.md`, `BROWSER-RESUME-HANDOFF-20261031T1221Z.md`, `BROWSER-RESUME-RESULT-20261031T1233Z.md`.
- #196 review: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #198 review: `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #156 final approval: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-APPROVAL-20261031.md`.
- #205 packet: `Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md`.
- #199 decision/continuation: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-DISPOSITION-OPTION1-20261031.md`, `OPTION1-CONTINUATION-PACKET.md`.
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`.
- S1-D T-08/T-09: `Docs/Handoffs/S1-D-Implementation-08/`, `Docs/Handoffs/S1-D-Implementation-09/`.
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
