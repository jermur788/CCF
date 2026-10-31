# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after starting Browser / GitHub Manager Lane 03 under #200. All browser/GitHub Manager work is under running #200; all non-browser/local/Unity execution orchestration remains under #201. #162 and #106 are superseded/closed. #196 remains the first MFM-specific Unity action. #198 and #199 remain prepared pre-Tier-3 debt and do not automatically block MFM-08 #156.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **During the current two-lane split, #200 is the sole primary writer for this file. #201 and child workers must refresh/read it but must not become competing writers.**
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- A prior `free` lock receipt is historical evidence only; it never reserves or proves future availability.
- Browser-capable phases belong to #200. Local/worktree/Unity execution orchestration belongs to #201.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` remains **`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`**. MFM-08 is not integrated.
- Rack V2 / D-064 remains **VERIFIED for feature scope**. #197 did not revoke G4.
- PARITY-08 #126 remains **INDEPENDENTLY APPROVED / INTEGRATED / CLOSED**.
- S1-D T-05 #151 remains **APPROVED / INTEGRATED / CLOSED**.
- S1-C T-07 #187 remains **MANAGER APPROVED / CLOSED**. Accepted genuine v20-y5 SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- OPS Unity-lock repair #189 remains **MANAGER APPROVED / CLOSED**. Any later Unity task must fresh-preflight and acquire normally.
- MFM-08 Geometry Repair 01 #192 remains **MANAGER APPROVED / CLOSED**. Exact repair production head `e5fb424635f16182748efcb0539e8a49539495a9`.
- **MFM-08 #156 is NEEDS CHANGES — VERIFICATION CLOSURE ONLY.** Exact repaired candidate `e5fb424635f16182748efcb0539e8a49539495a9` passed the unmodified successor gate 353/353 and the strengthened open-work-conflict gate 373 PASS at both render sizes. Returned verification head: `qual/mfm-08-unity-v1` @ `d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727`. #195 isolated a stale MapPlanning exact-five-tab contract after the authorised V2 `Treatment review` addition. #156 waits on #196 only for its MFM-specific closure.
- **MFM-08 Tier-2 Closure 01 #195 is CLOSED — NEEDS CHANGES ROUTED.** Manager review: `Docs/Handoffs/MFM-08-Tier2-Closure-01/MANAGER-REVIEW-20261031.md`.
- **MFM-08 MapPlanning Contract Reconciliation 01 #196 is READY — VERIFICATION-ONLY REPAIR + UNITY.** Preserve/assert the original five navigation tabs, assert authorised V2 `Treatment review` as additive, preserve the historical/non-MFM five-tab contract, then run the complete authoritative RackV2MapPlanning gate on exact `e5fb4246...`. No production repair/test weakening authority.
- **Rack V2 Current-Lineage Red-Gate Diagnosis 01 #197 is MANAGER APPROVED / CLOSED.** `guided-v2` is classified as stale v23/pre-D-066 verification contract; `combined-v2` / `combined-legacy` are classified as current generic Scenario One HUD/focus regressions against the protected Combined HUD gate, not MFM-specific Rack failures. Review: `Docs/Handoffs/RackV2-Current-Lineage-Red-Gate-Diagnosis-01/MANAGER-REVIEW-20261031.md`.
- **Guided-v2 v24 / D-066 Contract Reconciliation 01 #198 is READY — VERIFICATION ONLY FIRST.** Exact start `ce673714...`; reconcile stale guided/SequentialObjectives expectations to accepted current v24/D-066 teaching-latch authority without production/schema/anchor changes or assertion weakening. Requires serialized Unity only after the verification patch is reviewable.
- **Scenario One Combined HUD / Focus Regression Repair 01 #199 is READY — BOUNDED PRODUCTION UI REPAIR.** Exact start `ce673714...`; protected Combined HUD post-repair baseline was 450/0 model-0 and 455/0 V2. Current stable failures concern estimate/minimap, competitor presentation and focus 04/06/07. Preserve the protected assertions; no Rack/MFM/schema/ecology/economy changes. Independent review required.
- **Scenario One Browser / GitHub Manager Lane 03 #200 is RUNNING.** Canonical handoff: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-HANDOFF.md` at `b55225d4d385554e5ee70ca33d16d26ec43ef6a7`. It owns browser/GitHub Manager work and this file's writes, not local C#/Unity execution.
- **Scenario One Long-Running Deep Orchestration 01 #201 is READY.** Canonical handoff: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/DEEP-ORCHESTRATION-HANDOFF.md` at `b9e6b57133693cc9033e991c9c1724b7c0937b89`. It orchestrates non-browser execution and adaptive routing evidence; child claims remain mandatory.
- **Old browser umbrella #162 and old local umbrella #106 are SUPERSEDED / CLOSED.** Historical evidence remains valid; no new work should be dispatched under them.
- **Verification Harness Hygiene 01 #193 is MANAGER APPROVED / INTEGRATED / CLOSED.** Reviewed published handoff head `5f9450f4a65e6df8e989a241ffe48d8a3200f6f1`; implementation `b4b68a91d7ab8f1eb24b1956ed53b1e71b416e10`; integration merge `a678fdcfcf3d30541f4bf13066b6feaf2d31dd36`; parent backlog #28 resolved.
- **Research Source Consolidation Audit 01 #194 is historical/informational only under the user's explicit no-action instruction.** Do not review, approve/reject, merge, update, execute cleanup, reorganise Drive/Project sources, or otherwise act on that handoff unless the user explicitly lifts the restriction. Any older issue/review wording does not override this latest direct instruction.
- S1-C T-08 is **DEPENDENCY-CLEARED ON T-07 BUT NOT RELEASED TO UNITY YET**. It remains the preferred next history qualification slot after the MFM-08-specific Unity closure, unless Overall Manager explicitly reprioritises.
- S1-D T-08 remains **RELEASED** but serialized behind the preferred S1-C T-08 history slot.
- S1-D T-09 remains prepared and waits for S1-D T-08 PASS.
- MFM-09 remains prepared/not released; it waits for Manager-approved/integrated MFM-08 #156.
- MFM-10 remains prepared/not released; it waits for accepted/integrated MFM-09.
- PARITY-09 remains blocked on MFM-10 `SPECIES GUARD QUALIFIED`.
- MFM-11 remains prepared/not released; it waits for MFM-10 and must resolve the MFM-03 live/cross-year residual.
- MFM-12 remains prepared/not released; it waits for MFM-11 plus the applicable accepted integrated lineage.
- Scenario One Tier-3/player-test candidate remains browser-prepared/not released. #198/#199 must be closed before final Tier-3 if their protected/current gate contracts remain applicable. No Windows build is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **Scenario One Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, coordination, packets, claims, active-tasks writes | **RUNNING — BROWSER/GITHUB MANAGER ACTIVE** |
| **Scenario One Long-Running Deep Orchestration 01** (#201) | long-running non-browser orchestration | child worktrees only; no umbrella production ownership | dispatch/routing of local C#/Unity work; orchestration evidence | **READY — USER TO START LONG-RUNNING AGENT** |
| **MFM-08 Stand Map / 3D review UX** (#156) | Manager review / integration hold | candidate `e5fb4246...`; returned harness `d88a32f6...` | no production edits pending #196 | **NEEDS CHANGES — WAITING #196** |
| **MFM-08 MapPlanning Contract Reconciliation 01** (#196) | #201 child: bounded verification repair + Unity | `verify/mfm-08-mapplanning-contract-reconcile-v1`; intended registered lane `/media/jer/ZX20/CCF-U1` after fresh preflight | navigation-tab verification contract only; no production | **READY — FIRST MFM UNITY PRIORITY** |
| **Guided-v2 v24 / D-066 Contract Reconciliation 01** (#198) | #201 child: verification repair then serialized Unity | `verify/guided-v2-v24-contract-reconcile-v1`; worktree/lane assigned only at start | guided/SequentialObjectives verification only | **READY — QUEUED, PRE-TIER3 DEBT** |
| **Scenario One Combined HUD / Focus Regression Repair 01** (#199) | #201 child: bounded local UI repair + Unity + independent review | `fix/scenario-one-combined-hud-focus-regression-v1`; dedicated worktree assigned only at start | Scenario One HUD/focus only; no Rack/MFM ownership | **READY — QUEUED, PRE-TIER3 DEBT** |
| **S1-C T-08 final history qualification** | #201 local Tier-2 after MFM-specific closure; #200 repins/releases | prepared only; repin at release | qualification only; no production repair by default | **T-07 CLEARED; NOT RELEASED** |
| **S1-D T-08 multi-cycle witness** | #201 local Tier-2 after preferred S1-C T-08 slot | `verify/s1-d-multicycle-witness-v1`; `/media/jer/Videos/CCF-S1D-T08` | qualification only; no production repair by default | **RELEASED; WAITING SERIALIZED UNITY** |
| **S1-D T-09 independent conformance review** | #200 browser/read-only independent review | no production lane | review only | **PREPARED; WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | #200 browser prep/release then #201 local | prepared only; repin at release | curriculum/integration after accepted #156 | **WAITING ACCEPTED/INTEGRATED #156** |
| **MFM-10 species-parity guard** | #201 local qualification after MFM-09 | prepared only; repin at release | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | #200 repin then #201 local qualification | TBD | qualification | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | #201 local qualification + independent review | prepared only; repin at release | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | #201 local integrated qualification + independent review | prepared only; repin at release | verification/review | **WAITING MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #201 local Tier-3 | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #201 verification when explicitly scheduled | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING; REMAINING PHASE NEEDS UNITY** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight and acquire through the normal authoritative tool.

## Single-writer boundaries

### #200 Browser / GitHub Manager Lane 03

Browser/GitHub only. Sole primary writer for this live coordination file during the split. Owns Manager review/decisions, claims/packets/repins, browser/static diagnosis and browser-safe canonical status maintenance. It owns no local C#/Unity implementation and must not claim local cleanliness, Editor/runtime/rendered evidence or build state without returned local evidence.

### #201 Long-Running Deep Orchestration 01

Non-browser orchestration only at the umbrella level; substantive work still uses child claims. DeepSeek is the default orchestrator/lower-cost worker where capable; Claude Sonnet 5.5 Medium is the main harder-task escalation; Opus 5.5 Low then Medium only for classified capability/high-consequence escalation. Maintain the adaptive-routing evidence ledger required by `DEEP-ORCHESTRATION-HANDOFF.md`. Read this file before every dispatch but return coordination changes to #200 rather than editing it concurrently.

### #156 MFM-08 successor Tier-2

Manager review accepts the feature behavior on exact repaired production head `e5fb424635f16182748efcb0539e8a49539495a9`: 353/353 unmodified gate and 373 PASS with the bounded open-work conflict addition, both render sizes, repaired segment/sample geometry and the accepted MFM/protected regression evidence. No production repair is currently requested.

Integration is held only for #196 MFM-specific MapPlanning closure. Do not modify MFM-08 production under #156 or #196 without a new Manager repair claim.

### #196 MFM-08 MapPlanning Contract Reconciliation 01

Verification-only repair plus full authoritative MapPlanning rerun. The only authorised test change is a state-aware reconciliation of the old exact-five navigation-tab expectation: the original five must remain asserted; authorised V2 `Treatment review` must be additive; non-MFM/historical behavior must preserve the five-tab contract where applicable. Do not blindly change a count, remove assertions, edit production, or weaken unrelated MapPlanning coverage. Start from exact `e5fb4246...`, fresh-preflight and acquire Unity normally, then run the complete gate and return clean. No Windows.

### #198 Guided-v2 v24 / D-066 Contract Reconciliation 01

Verification-only first. Replace only stale v23/pre-D-066 guided/SequentialObjectives expectations with the accepted current v24/D-066 teaching-latch contract. Preserve real-input intent, explicit Rack V2/model-0 split, future-tree/release/review coverage where applicable, save/load determinism and protected Rack assertions. Do not accept arbitrary lesson indices or skip review assertions. If the correctly reconciled gate exposes a production defect, stop for a new Manager repair claim.

### #199 Scenario One Combined HUD / Focus Regression Repair 01

Bounded production UI/focus repair. Protected Combined HUD assertions remain authoritative: do not delete, skip, rebaseline or relax them merely to obtain green. Root-cause estimate/minimap, competitor panel/tags, and FOCUS_04/06/07 separately; make the smallest presentation/focus repair; verify both Rack V2 and legacy modes at 1280×720 and 1920×1080 plus proportionate affected gates. No Rack geometry/marking/persistence, MFM, save/schema/anchor, ecology or economy changes. Independent review required.

### Research source consolidation

**No-action restriction.** The user explicitly instructed the Manager not to act on #194 / `Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md`. Do not resume cleanup, deletion, bundling, archive/Drive reorganisation, Project-source replacement, review or integration from that handoff unless the user explicitly lifts this restriction.

### S1-C T-08

Qualification-only when released. T-07 acceptance fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. No production repair authority.

### S1-D T-08

Released qualification-only task. No production fix authority. Exact released candidate remains `ce673714...` unless Manager explicitly republishes a later inspected descendant.

## Dependency-ready sequence

1. **#201 first local action:** execute #196 after fresh preflight/acquisition. Reconcile only the stale tab contract and run the complete authoritative RackV2MapPlanning gate on exact `e5fb4246...`.
2. **#200 first return trigger:** review #196 handoff and issue `APPROVED / NEEDS CHANGES / REJECTED`. If the complete MapPlanning gate passes under the non-weakened contract and no new MFM-specific failure appears, #156 may be upgraded; #197 found no direct Rack/D-064 reason to hold it.
3. If #156 is upgraded to APPROVED, #200 performs overlap/integration review and releases the exact local integration step to #201; then #200 repins/releases MFM-09 on the accepted combined lineage.
4. Preserve the established full-loop critical path: after the MFM-specific closure releases the serialized lane, #200 fresh-repins/releases S1-C T-08; #201 executes it, then S1-D T-08; #200 handles S1-D T-09 independent browser review.
5. #198 and #199 are required pre-Tier3 cleanup/qualification debt. #200 decides exact scheduling; #201 executes. Do not silently displace S1-C/S1-D unless returned evidence justifies reprioritisation.
6. MFM-09 accepted/integrated -> MFM-10 -> require `SPECIES GUARD QUALIFIED`.
7. MFM-10 green -> PARITY-09 and MFM-11 become eligible subject to ownership/Unity serialization.
8. MFM-11 accepted + applicable accepted Rack/parity/S1-C/S1-D lineage -> MFM-12 integrated qualification.
9. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 + applicable #198/#199 closures accepted/integrated on one exact candidate -> Scenario One Tier-3.
10. Tier-3 success -> user personally playtests the same exact candidate.
11. Windows build only after explicit post-playtest authorisation.

## Durable current records

- Two-lane browser handoff: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/BROWSER-HANDOFF.md` — claim #200.
- Two-lane long-running orchestration handoff: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/DEEP-ORCHESTRATION-HANDOFF.md` — claim #201.
- MFM-08 returned first Tier-2 Manager review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/TIER2-MANAGER-REVIEW-20261030.md`
- MFM-08 repair packet/handoff/review: `Docs/Handoffs/MFM-08-Geometry-Repair-01/`
- MFM-08 successor repin: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/SUCCESSOR-TIER2-REPIN-20261030.md`
- MFM-08 successor Tier-2 Manager review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/SUCCESSOR-TIER2-MANAGER-REVIEW-20261031.md`
- MFM-08 Tier-2 Closure 01 handoff/review: `Docs/Handoffs/MFM-08-Tier2-Closure-01/`
- MFM-08 MapPlanning Contract Reconciliation 01: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/TASK-PACKET.md`
- Rack V2 Current-Lineage Red-Gate Diagnosis 01: `Docs/Handoffs/RackV2-Current-Lineage-Red-Gate-Diagnosis-01/`
- Guided-v2 v24/D-066 reconciliation: `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/TASK-PACKET.md`
- Scenario One Combined HUD/focus repair: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/TASK-PACKET.md`
- Verification Harness Hygiene 01: `Docs/Handoffs/Verification-Harness-Hygiene-01/`
- Research Source Consolidation Audit 01: informational only under current user no-action restriction.
- S1-C T-07: `Docs/Handoffs/S1-C-Implementation-07/`
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`
- OPS Unity lock: `Docs/Handoffs/OPS-UNITY-LOCK-01/`
- S1-D T-08: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09: `Docs/Handoffs/S1-D-Implementation-09/`
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`