# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after Browser Manager Resume 01 #202 completed and local resume #203 started MFM-08 Integration 01 #205. Browser/GitHub Manager ownership remains under #200; non-browser/local/Unity orchestration remains under #201/#203. #199 Option 1 is authorised but cannot become an overlapping production UI writer while #205 is active.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 is the sole primary writer for this file during the current two-lane split.** #201/#203 and child workers refresh/read it but do not become competing writers.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide; fresh-preflight free space, Editor processes and the authoritative lock immediately before acquisition.
- A historical `free` lock receipt never reserves or proves future availability.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` remains **`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`** until #205 is completed and Manager-accepted. MFM-08 is not yet published/integrated there.
- Rack V2 / D-064 remains **VERIFIED for feature scope**. PARITY-08 #126 and S1-D T-05 #151 remain **APPROVED / INTEGRATED / CLOSED**.
- S1-C T-07 #187 remains **MANAGER APPROVED / CLOSED**; genuine v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- #196 MFM-08 MapPlanning Contract Reconciliation is **MANAGER APPROVED / CLOSED**: verification-only `eef1bdf9`; full MapPlanning `168/168`, 8 captures; original five tab names/order retained and V2 `Treatment review` additive. Review: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #198 guided-v2 v24 / D-066 reconciliation is **MANAGER APPROVED / CLOSED**: guided-v2 `535/0/48`; guided-legacy `569/0/62`; first legacy input timing failure retained as non-blocking runner debt. Review: `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #156 MFM-08 is **MANAGER APPROVED / READY FOR INTEGRATION** on exact production candidate `e5fb424635f16182748efcb0539e8a49539495a9`; accepted feature evidence remains 353/353 plus strengthened 373-check conflict gate at both render sizes. Final review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-APPROVAL-20261031.md`.
- **#205 MFM-08 Integration 01 is RUNNING under #203.** Provider/model reported: OpenCode `opencode-go/deepseek-v4.1-flash`; branch `integration/mfm-08-onto-scenario-one-v1`; local lane `/media/jer/ZX20/CCF-U1`; exact start `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`. Worker reports exact approved three-commit range `cfdc93bf3943125d21570bc5749a9338f52d1cce..e5fb424635f16182748efcb0539e8a49539495a9` replayed by cherry-pick with **no conflicts** and byte-equivalent diff (15 files); #196 `eef1bdf9` and #199 `42742a8a` not absorbed. This is returned local execution evidence, not yet Manager integration acceptance. Unity import/compile + focused MFM-08 + full RackV2MapPlanning are the next local phases. Packet: `Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md`.
- #199 Combined HUD / Focus Regression is **OPTION 1 AUTHORISED — WAITING POST-#205 REPIN**. Partial branch `42742a8a` is fixture-only; partial gates combined-v2 `453/10/24`, combined-legacy `440/8/24`. Production direction: restore minimap for ordinary estimate inspection while preserving accepted PARITY-07 estimate information/copy and central-prompt/detail clearance, preserve protected Combined assertions, and repair the real 1920 V2 competitor-list clipping with bounded responsive/reflow work. Decision: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-DISPOSITION-OPTION1-20261031.md`; continuation: `OPTION1-CONTINUATION-PACKET.md`.
- #200 browser umbrella remains **RUNNING**. #202 Browser Manager Resume 01 is **COMPLETE / CLOSED**; result: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-RESUME-RESULT-20261031T1233Z.md`.
- #201 local umbrella remains active; #203 Local Deep Orchestration Resume 02 is **RUNNING** with #205 as its current child.
- Old umbrellas #162/#106 remain SUPERSEDED/CLOSED. Verification Harness Hygiene #193 remains APPROVED/INTEGRATED/CLOSED.
- **Research Source Consolidation Audit #194 remains NO ACTION.** Do not review, approve/reject, merge, update, clean up, reorganise Drive/Project sources or otherwise act on it unless the user explicitly lifts the restriction.
- S1-C T-08 is dependency-cleared on T-07 but not released to Unity yet. It remains the preferred history qualification slot after the current integration step unless live evidence requires reprioritisation.
- S1-D T-08 remains a qualification task previously released on `ce673714...`; after #205 changes the integrated lineage, #200 must determine whether it requires fresh repin before execution. S1-D T-09 waits for T-08 PASS.
- MFM-09 waits for Manager-accepted/integrated MFM-08 via #205, then requires a fresh exact-SHA repin. MFM-10 waits on MFM-09 and must return `SPECIES GUARD QUALIFIED`; PARITY-09 waits on that token; MFM-11 then MFM-12 follow.
- Scenario One Tier-3/player-test candidate remains **NOT RELEASED**. #199 must be green before final Tier-3 while its protected Combined contract remains applicable.
- Dashboard publication beyond this GitHub coordination surface is **not verified in the current browser toolset**. Never manually edit generated dashboard state; if the dashboard publisher is in use, sync remains an explicit operational follow-up.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, coordination, repins, claims, active-tasks writes | **RUNNING — UMBRELLA** |
| **Browser Manager Resume 01** (#202) | browser/GitHub | no local worktree | bounded verdict/release task | **COMPLETE / CLOSED** |
| **Local Deep Orchestration Resume 02** (#203) | non-browser orchestration | child lanes only | local routing + evidence | **RUNNING — #205 CHILD ACTIVE** |
| **MFM-08** (#156) | integration hold | approved candidate `e5fb4246...` | no new feature edits | **MANAGER APPROVED — #205 RUNNING** |
| **MFM-08 Integration 01** (#205) | #203 child: integration + focused Unity | `integration/mfm-08-onto-scenario-one-v1`; `/media/jer/ZX20/CCF-U1` | exact approved MFM replay only | **RUNNING — REPLAY CLEAN; UNITY QUALIFICATION NEXT** |
| **Combined HUD / Focus Repair** (#199) | #203 child after repin | stale partial `42742a8a`; new exact base TBD after #205 | bounded Option-1 presentation/focus repair | **AUTHORISED — WAITING POST-#205 REPIN** |
| **S1-C T-08 final history qualification** | #203 local Tier-2; #200 repins/releases | prepared only | qualification only | **T-07 CLEARED; NOT RELEASED** |
| **S1-D T-08 multi-cycle witness** | #203 local Tier-2 after preferred history slot | prior branch/release | qualification only | **OLD-BASE RELEASE; REPIN CHECK AFTER #205** |
| **S1-D T-09 independent review** | #200 browser/read-only | no production lane | review only | **WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | #200 repin/release then #203 local | prepared only | curriculum/integration | **WAITING ACCEPTED #205** |
| **MFM-10 species-parity guard** | #203 local | prepared only | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | #200 repin then #203 local | TBD | qualification | **BLOCKED ON MFM-10** |
| **MFM-11 calibration impact** | #203 local + independent review | prepared only | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | #203 local + independent review | prepared only | integrated verification | **WAITING MFM-11 + APPLICABLE LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #203 local | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | `verify/parity-03-density-continuity-q10-v1` | verification only | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves the Unity lock is currently free. Every Unity worker must fresh-preflight/acquire through the authoritative tool.

## Single-writer boundaries

### #200 browser Manager
Browser/GitHub only and sole writer of this file. Owns Manager decisions, browser/static review, packets/repins and status. No local C#/Unity implementation or unreturned runtime claims.

### #201 / #203 local orchestration
Substantive local work uses child claims. Read this file before dispatch and return coordination changes to #200. Maintain adaptive-routing evidence. DeepSeek default where capable; Sonnet 5.5 Medium harder-task escalation; Opus only after classified capability/reasoning failure or unusually high-consequence ambiguity.

### #205 MFM-08 Integration 01
Current single writer for its integration surface. Exact replay only; no #196/#199 absorption or new behavior. The clean replay is not enough for acceptance: import/compile and focused post-integration MFM/MapPlanning/compatibility evidence must return before Manager approval. No Windows.

### #199 Option 1
Authorised but no production start until #205 is Manager-accepted and #200 publishes the exact combined-SHA repin. Preserve ordinary-inspection minimap, accepted PARITY-07 estimate/prompt/detail requirements and protected Combined assertions; repair 1920 competitor clipping through bounded reflow. Independent review required. No Rack/MFM/save/schema/anchor/ecology/economy changes. No Windows.

### Research source consolidation
**NO ACTION.** Do not act on #194 unless the user explicitly lifts the restriction.

## Dependency-ready sequence

1. #203 completes #205 local Unity/post-replay qualification and publishes the handoff.
2. #200 reviews #205 immediately. If approved, update exact integration truth, mark/close #156 integration state appropriately, and repin downstream exact-candidate work.
3. Preserve complete-loop critical path: fresh-repin/release S1-C T-08, then S1-D T-08, then browser S1-D T-09, subject to live dependencies/serialization.
4. After accepted/integrated MFM-08, fresh-repin/release MFM-09; then MFM-10 -> `SPECIES GUARD QUALIFIED`.
5. #199 Option 1 is required pre-Tier-3 but starts only after #205 and exact browser repin; avoid overlapping UI ownership.
6. MFM-10 green -> PARITY-09 and MFM-11; MFM-11 accepted -> MFM-12.
7. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 + #199 closure accepted/integrated on one exact candidate -> Tier-3.
8. Tier-3 success -> user personally playtests the same exact candidate.
9. Windows build only after explicit post-playtest user authorisation.

## Durable current records

- Browser resume result: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-RESUME-RESULT-20261031T1233Z.md`.
- #196 review: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #198 review: `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #156 final approval: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-APPROVAL-20261031.md`.
- #205 packet: `Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md`.
- #199 decision/continuation: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-DISPOSITION-OPTION1-20261031.md`, `OPTION1-CONTINUATION-PACKET.md`.
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`.
- S1-D T-08/T-09: `Docs/Handoffs/S1-D-Implementation-08/`, `Docs/Handoffs/S1-D-Implementation-09/`.
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
