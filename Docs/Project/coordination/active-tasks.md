# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after approving and closing Rack V2 Current-Lineage Red-Gate Diagnosis 01 #197. #196 remains the first MFM-specific Unity action. #198 (guided-v2 v24/D-066 contract reconciliation) and #199 (Combined HUD/focus regression repair) are prepared follow-up debt and do not automatically block MFM-08 #156; both must be resolved before final integrated Tier-3 where their protected/current contracts apply.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- A prior `free` lock receipt is historical evidence only; it never reserves or proves future availability.
- Browser-capable phases should be separated from local implementation/Unity wherever practical; #162 is the browser lane.
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
- **Verification Harness Hygiene 01 #193 is MANAGER APPROVED / INTEGRATED / CLOSED.** Reviewed published handoff head `5f9450f4a65e6df8e989a241ffe48d8a3200f6f1`; implementation `b4b68a91d7ab8f1eb24b1956ed53b1e71b416e10`; integration merge `a678fdcfcf3d30541f4bf13066b6feaf2d31dd36`; parent backlog #28 resolved.
- **Research Source Consolidation Audit 01 #194 is MANAGER APPROVED / CLOSED — PLAN ONLY.** Reviewed handoff head `60196617fd0afcbfd45642df8c8d74b3a4eac1bd`; canonical review `Docs/Handoffs/Research-Source-Consolidation-Audit-01/MANAGER-REVIEW-20261031.md`. Accepted planning conclusion: preserve byte-identical originals first; broad PDF bundling/master-PDF is not recommended; no cleanup, deletion, move, watermark, recompression, Drive cleanup or source replacement is authorised by this approval.
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
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **MFM-08 Stand Map / 3D review UX** (#156) | Manager review / integration hold | candidate `e5fb4246...`; returned harness `d88a32f6...` | no production edits pending #196 | **NEEDS CHANGES — WAITING #196** |
| **MFM-08 MapPlanning Contract Reconciliation 01** (#196) | bounded verification repair + Unity | `verify/mfm-08-mapplanning-contract-reconcile-v1`; intended registered lane `/media/jer/ZX20/CCF-U1` after fresh preflight | navigation-tab verification contract only; no production | **READY — FIRST MFM UNITY PRIORITY** |
| **Guided-v2 v24 / D-066 Contract Reconciliation 01** (#198) | verification repair then serialized Unity | `verify/guided-v2-v24-contract-reconcile-v1`; worktree/lane assigned only at start | guided/SequentialObjectives verification only | **READY — QUEUED, PRE-TIER3 DEBT** |
| **Scenario One Combined HUD / Focus Regression Repair 01** (#199) | bounded local UI repair + Unity + independent review | `fix/scenario-one-combined-hud-focus-regression-v1`; dedicated worktree assigned only at start | Scenario One HUD/focus only; no Rack/MFM ownership | **READY — QUEUED, PRE-TIER3 DEBT** |
| **S1-C T-08 final history qualification** | local Tier-2 after MFM-specific closure | prepared only; repin at release | qualification only; no production repair by default | **T-07 CLEARED; NOT RELEASED** |
| **S1-D T-08 multi-cycle witness** | local Tier-2 after preferred S1-C T-08 slot | `verify/s1-d-multicycle-witness-v1`; `/media/jer/Videos/CCF-S1D-T08` | qualification only; no production repair by default | **RELEASED; WAITING SERIALIZED UNITY** |
| **S1-D T-09 independent conformance review** | browser/read-only independent review | no production lane | review only | **PREPARED; WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | browser prep then local | prepared only; repin at release | curriculum/integration after accepted #156 | **WAITING ACCEPTED/INTEGRATED #156** |
| **MFM-10 species-parity guard** | local qualification after MFM-09 | prepared only; repin at release | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | browser repin then local qualification | TBD | qualification | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | local qualification + independent review | prepared only; repin at release | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | local integrated qualification + independent review | prepared only; repin at release | verification/review | **WAITING MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | browser repin then local Tier-3 | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING; REMAINING PHASE NEEDS UNITY** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight and acquire through the normal authoritative tool.

## Single-writer boundaries

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

Audit #194 is complete and grants no live ownership. Any future backup, deletion, move, derivative marking, recompression or archive reorganisation requires a new reviewed execution packet and explicit user/Manager approval. Evidential originals must remain byte-identical in the designated archive under any approved future plan.

### S1-C T-08

Qualification-only when released. T-07 acceptance fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. No production repair authority.

### S1-D T-08

Released qualification-only task. No production fix authority. Exact released candidate remains `ce673714...` unless Manager explicitly republishes a later inspected descendant.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work.

## Dependency-ready sequence

1. **First MFM-specific Unity action:** execute #196 after fresh preflight/acquisition. Reconcile only the stale tab contract and run the complete authoritative RackV2MapPlanning gate on exact `e5fb4246...`.
2. On #196 handoff, Manager issues `APPROVED / NEEDS CHANGES / REJECTED`. If the complete MapPlanning gate passes under the non-weakened contract and no new MFM-specific failure appears, #156 may be upgraded; #197 has already found no direct Rack/D-064 reason to hold it.
3. If #156 is upgraded to APPROVED, safely integrate the exact MFM-08 production delta onto the exact current Scenario One integration lineage after fresh overlap review; then release MFM-09 from that combined lineage.
4. Preserve the established full-loop critical path: after the MFM-specific closure releases the serialized lane, fresh-repin/release S1-C T-08, then proceed to S1-D T-08 and S1-D T-09 unless a returned blocker forces reprioritisation.
5. #198 and #199 are required pre-Tier3 cleanup/qualification debt. Schedule them into serialized Unity without silently displacing S1-C/S1-D; #199 may be pulled earlier if later rendered UI gates demonstrate direct overlap.
6. MFM-09 accepted/integrated -> MFM-10 -> require `SPECIES GUARD QUALIFIED`.
7. MFM-10 green -> PARITY-09 and MFM-11 become eligible subject to ownership/Unity serialization.
8. MFM-11 accepted + applicable accepted Rack/parity/S1-C/S1-D lineage -> MFM-12 integrated qualification.
9. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 + applicable #198/#199 closures accepted/integrated on one exact candidate -> Scenario One Tier-3.
10. Tier-3 success -> user personally playtests the same exact candidate.
11. Windows build only after explicit post-playtest authorisation.

## Durable current records

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
- Research Source Consolidation Audit 01 handoff/review: `Docs/Handoffs/Research-Source-Consolidation-Audit-01/`
- S1-C T-07: `Docs/Handoffs/S1-C-Implementation-07/`
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`
- OPS Unity lock: `Docs/Handoffs/OPS-UNITY-LOCK-01/`
- S1-D T-08: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09: `Docs/Handoffs/S1-D-Implementation-09/`
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`
