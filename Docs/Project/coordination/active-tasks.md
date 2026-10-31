# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol under Browser Manager Resume 01 #202. Browser/GitHub Manager work remains under #200; non-browser/local/Unity orchestration remains under #201 with resume #203. #196 and #198 are Manager-approved/closed. MFM-08 #156 is Manager-approved but not integrated; #205 is the exact next local integration action. #199 Option 1 is authorised but must wait for a post-#205 repin before production edits.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **During the current two-lane split, #200 is the sole primary writer for this file. #201/#203 and child workers must refresh/read it but must not become competing writers.**
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- A prior `free` lock receipt is historical evidence only; it never reserves or proves future availability.
- Browser-capable phases belong to #200/#202. Local/worktree/Unity execution orchestration belongs to #201/#203.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` remains **`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`**. MFM-08 is **not integrated** yet.
- Rack V2 / D-064 remains **VERIFIED for feature scope**. #197 did not revoke G4.
- PARITY-08 #126 remains **INDEPENDENTLY APPROVED / INTEGRATED / CLOSED**.
- S1-D T-05 #151 remains **APPROVED / INTEGRATED / CLOSED**.
- S1-C T-07 #187 remains **MANAGER APPROVED / CLOSED**. Accepted genuine v20-y5 SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- OPS Unity-lock repair #189 remains **MANAGER APPROVED / CLOSED**. Any later Unity task must fresh-preflight and acquire normally.
- MFM-08 Geometry Repair 01 #192 remains **MANAGER APPROVED / CLOSED**. Exact repaired production head `e5fb424635f16182748efcb0539e8a49539495a9`.
- **MFM-08 MapPlanning Contract Reconciliation #196 is MANAGER APPROVED / CLOSED.** Verification-only patch `eef1bdf9`; complete authoritative gate `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`. Original five tabs remain asserted by name/order; V2 `Treatment review` is additive; historical/non-V2 five-tab branch remains explicit. Review: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- **Guided-v2 v24 / D-066 Contract Reconciliation #198 is MANAGER APPROVED / CLOSED.** `guided-v2` 535/0 with 48 captures; `guided-legacy` 569/0 with 62 captures. Preserved first legacy real-input timing failure is non-blocking environment/runner debt. Review: `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- **MFM-08 #156 is MANAGER APPROVED / READY FOR INTEGRATION.** Exact approved production candidate `e5fb424635f16182748efcb0539e8a49539495a9`; previously accepted feature evidence remains 353/353 unmodified successor gate and strengthened 373-check open-work-conflict gate at both render sizes. Final review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-APPROVAL-20261031.md`.
- **MFM-08 Integration 01 #205 is READY — NEXT LOCAL ACTION.** Start from exact integration `ce673714...`; replay only the exact approved 3-commit MFM delta `cfdc93bf3943125d21570bc5749a9338f52d1cce..e5fb424635f16182748efcb0539e8a49539495a9`; do not silently absorb #196 verification-only `eef1bdf9` or #199 partial work. Packet: `Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md`.
- Browser overlap review for #205: MFM candidate and integration diverge at `cfdc93bf...`; integration is 29 commits ahead and does not modify the two pre-existing MFM-08 production paths `Assets/ForestPrototype/UI/Map/StandMapView.cs` or `Assets/ForestPrototype/UI/TreeInspectionView.cs`; remaining MFM production paths are additions. This supports release but does not prove local post-replay compile/runtime correctness.
- **Scenario One Combined HUD / Focus Regression #199 is OPTION 1 AUTHORISED — WAITING POST-#205 REPIN.** Partial branch `42742a8a` contains a fixture-only correction; no production `Assets/` change. Observed partial gates: combined-v2 453 pass / 10 fail / 24 captures; combined-legacy 440 pass / 8 fail / 24 captures. Required production direction: restore minimap for ordinary estimate inspection while retaining accepted PARITY-07 estimate information/copy and central-prompt/detail clearance, preserve protected Combined assertions, and repair the exposed 1920 V2 competitor-list clipping through bounded responsive/reflow work. Decision: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-DISPOSITION-OPTION1-20261031.md`; continuation packet: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-CONTINUATION-PACKET.md`.
- **Scenario One Browser / GitHub Manager Lane 03 #200 is RUNNING.** Browser resume #202 is the current bounded Manager execution. #200 remains sole primary writer for this file.
- **Scenario One Long-Running Deep Orchestration #201 / resume #203:** #203 is READY for the next released local task. DeepSeek remains orchestrator/default worker where capable; Claude Sonnet 5.5 Medium is harder-task escalation; Opus only after a classified capability/reasoning failure or unusually high-consequence ambiguity.
- Old umbrellas #162 and #106 remain SUPERSEDED / CLOSED.
- Verification Harness Hygiene #193 remains MANAGER APPROVED / INTEGRATED / CLOSED.
- **Research Source Consolidation Audit #194 remains historical/informational only under the user's explicit NO-ACTION instruction.** Do not review, approve/reject, merge, update, execute cleanup, reorganise Drive/Project sources, or otherwise act on that handoff unless the user explicitly lifts the restriction.
- S1-C T-08 is **DEPENDENCY-CLEARED ON T-07 BUT NOT RELEASED TO UNITY YET**. It remains the preferred history qualification slot after the current MFM integration step unless live evidence requires reprioritisation.
- S1-D T-08 remains **RELEASED** but serialized behind the preferred S1-C T-08 history slot.
- S1-D T-09 remains prepared and waits for S1-D T-08 PASS.
- MFM-09 remains prepared/not released; it waits for Manager-accepted/integrated MFM-08 via #205 and then requires a fresh exact-SHA repin.
- MFM-10 remains prepared/not released; it waits for accepted/integrated MFM-09 and must return `SPECIES GUARD QUALIFIED`.
- PARITY-09 remains blocked on MFM-10 `SPECIES GUARD QUALIFIED`.
- MFM-11 remains prepared/not released; it waits for MFM-10 and must resolve the MFM-03 live/cross-year residual.
- MFM-12 remains prepared/not released; it waits for MFM-11 plus the applicable accepted integrated lineage.
- Scenario One Tier-3/player-test candidate remains browser-prepared/not released. #199 must be green before final Tier-3 while its protected Combined contract remains applicable. #198 is no longer an open blocker.
- Dashboard publication/state beyond this GitHub coordination surface is **not verified in the current browser toolset**. Do not manually edit generated dashboard state; record this as an outstanding operational sync check if the dashboard publisher is in use.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **Browser Manager Resume 01** (#202; parent #200) | browser/GitHub | no local worktree | Manager verdicts, coordination, release | **RUNNING — CURRENT BROWSER TASK** |
| **Local Deep Orchestration Resume 02** (#203; parent #201) | long-running non-browser orchestration | child worktrees only | dispatch/routing of local C#/Unity work; orchestration evidence | **READY — #205 RELEASED AS NEXT LOCAL ACTION** |
| **MFM-08 Stand Map / 3D review UX** (#156) | Manager-approved integration hold | candidate `e5fb4246...` | feature approved; no new feature edits | **MANAGER APPROVED — WAITING #205 INTEGRATION** |
| **MFM-08 Integration 01** (#205) | #203 child: local integration + focused Unity | `integration/mfm-08-onto-scenario-one-v1`; dedicated worktree assigned at start | exact MFM-08 replay/integration only | **READY — FIRST LOCAL ACTION** |
| **Scenario One Combined HUD / Focus Regression** (#199) | #203 child after repin | stale partial `fix/scenario-one-combined-hud-focus-regression-v1` @ `42742a8a`; new exact base TBD after #205 | bounded Option-1 presentation/focus repair only | **OPTION 1 AUTHORISED — WAITING POST-#205 REPIN** |
| **S1-C T-08 final history qualification** | #203 local Tier-2 after current integration slot; #200 repins/releases | prepared only; repin at release | qualification only; no production repair by default | **T-07 CLEARED; NOT RELEASED** |
| **S1-D T-08 multi-cycle witness** | #203 local Tier-2 after preferred S1-C T-08 slot | `verify/s1-d-multicycle-witness-v1`; `/media/jer/Videos/CCF-S1D-T08` | qualification only; no production repair by default | **RELEASED; WAITING SERIALIZED UNITY / MAY REQUIRE REPIN AFTER NEW INTEGRATION** |
| **S1-D T-09 independent conformance review** | #200 browser/read-only independent review | no production lane | review only | **PREPARED; WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | #200 browser repin/release then #203 local | prepared only | curriculum/integration after accepted #205 | **WAITING ACCEPTED/INTEGRATED #205** |
| **MFM-10 species-parity guard** | #203 local qualification after MFM-09 | prepared only | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | #200 repin then #203 local qualification | TBD | qualification | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | #203 local qualification + independent review | prepared only | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | #203 local integrated qualification + independent review | prepared only | verification/review | **WAITING MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #203 local Tier-3 | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 verification when explicitly scheduled | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING; REMAINING PHASE NEEDS UNITY** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight and acquire through the normal authoritative tool.

## Single-writer boundaries

### #200 / #202 Browser Manager

Browser/GitHub only. Sole primary writer for this live coordination file. Owns Manager review/decisions, claims/packets/repins, browser/static diagnosis and browser-safe canonical status maintenance. Owns no local C#/Unity implementation and must not claim local cleanliness, Editor/runtime/rendered evidence or build state without returned local evidence.

### #201 / #203 Local orchestration

Non-browser orchestration at umbrella level; substantive work uses child claims. Read this file before every dispatch and return coordination changes to #200 rather than editing it concurrently. Maintain the adaptive-routing evidence ledger required by the orchestration handoff.

### #205 MFM-08 Integration 01

Bounded integration only. Start from exact `ce673714...`; replay only `cfdc93bf...e5fb4246`. Do not absorb #196 `eef1bdf9`, #199 `42742a8a`, unrelated docs/harness commits or new behavior. Stop on unexpected conflict. Local import/compile and proportionate focused MFM/MapPlanning/save/reference/Legacy evidence are mandatory before Manager integration acceptance. No Windows.

### #199 Option 1

Manager-authorised production layout/focus repair, but blocked from production start until #205 is accepted and #200 republishes the exact combined-SHA repin. Preserve both contracts: ordinary estimate inspection keeps the minimap; accepted PARITY-07 estimate content and central-prompt/detail clearance remain; protected Combined assertions remain semantically unchanged. Repair real 1920 competitor clipping through bounded responsive/reflow work. Independent review required. No Rack/MFM/save/schema/anchor/ecology/economy changes. No Windows.

### Research source consolidation

**NO ACTION.** The user explicitly instructed the Manager not to act on #194 / `Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md` unless the user explicitly lifts the restriction.

### S1-C T-08

Qualification-only when released. T-07 acceptance fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. No production repair authority.

### S1-D T-08

Qualification-only. Existing release was on `ce673714...`; after #205 changes the integration lineage, #200 must decide whether a fresh repin is required before execution rather than assuming the old release is still exact.

## Dependency-ready sequence

1. **#203/#201 next local action:** execute #205 MFM-08 Integration 01 after fresh ownership/worktree/Unity preflight.
2. **#200 next return trigger:** review #205. If approved, update the published integration truth, mark #156 integrated/closed as appropriate, and repin MFM-09 plus any downstream qualification whose exact candidate changed.
3. Preserve the complete-loop critical path: fresh-repin/release S1-C T-08, then S1-D T-08, then browser S1-D T-09, subject to exact live ownership/Unity serialization and any dependency-specific integration requirements.
4. After accepted/integrated MFM-08, fresh-repin/release MFM-09; MFM-09 accepted/integrated -> MFM-10 -> require `SPECIES GUARD QUALIFIED`.
5. **#199 Option 1 remains required pre-Tier-3** but starts only after #205 and exact browser repin. Schedule it without creating an overlapping UI writer or silently displacing the established S1-C/S1-D/MFM critical path unless it directly blocks an upcoming required gate.
6. MFM-10 green -> PARITY-09 and MFM-11 become eligible subject to ownership/Unity serialization.
7. MFM-11 accepted + applicable accepted Rack/parity/S1-C/S1-D lineage -> MFM-12 integrated qualification.
8. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 + #199 closure accepted/integrated on one exact candidate -> Scenario One Tier-3.
9. Tier-3 success -> user personally playtests the same exact candidate.
10. Windows build only after explicit post-playtest authorisation.

## Durable current records

- Browser lane: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-HANDOFF.md` and resume `BROWSER-RESUME-HANDOFF-20261031T1221Z.md`.
- Local orchestration: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/DEEP-ORCHESTRATION-HANDOFF.md` and local resume handoff under #203.
- #196 review: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #198 review: `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/MANAGER-REVIEW-20261031.md`.
- #156 final approval: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-APPROVAL-20261031.md`.
- #205 packet: `Docs/Handoffs/MFM-08-Integration-01/TASK-PACKET.md`.
- #199 decision: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-DISPOSITION-OPTION1-20261031.md`.
- #199 continuation: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-CONTINUATION-PACKET.md`.
- Research Source Consolidation #194: informational only; NO ACTION.
- S1-C T-07/T-08: `Docs/Handoffs/S1-C-Implementation-07/`, `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`.
- S1-D T-08/T-09: `Docs/Handoffs/S1-D-Implementation-08/`, `Docs/Handoffs/S1-D-Implementation-09/`.
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
