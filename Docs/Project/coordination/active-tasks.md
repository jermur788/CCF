# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after reviewing MFM-08 Tier-2 Closure 01 #195. #195 is closed with NEEDS CHANGES routed to #196 (MFM-specific MapPlanning contract reconciliation) and #197 (separate Rack V2 current-lineage red-gate diagnosis). MFM-08 feature behavior remains qualified on its exact repaired candidate; #156 remains NEEDS CHANGES pending #196 only for MFM-specific closure.

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
- Rack V2 / D-064 remains **VERIFIED for feature scope**.
- PARITY-08 #126 remains **INDEPENDENTLY APPROVED / INTEGRATED / CLOSED**.
- S1-D T-05 #151 remains **APPROVED / INTEGRATED / CLOSED**.
- S1-C T-07 #187 remains **MANAGER APPROVED / CLOSED**. Accepted genuine v20-y5 SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- OPS Unity-lock repair #189 remains **MANAGER APPROVED / CLOSED**. Any later Unity task must fresh-preflight and acquire normally.
- MFM-08 Geometry Repair 01 #192 remains **MANAGER APPROVED / CLOSED**. Exact repair production head `e5fb424635f16182748efcb0539e8a49539495a9`.
- **MFM-08 #156 is NEEDS CHANGES — VERIFICATION CLOSURE ONLY.** Exact repaired candidate `e5fb424635f16182748efcb0539e8a49539495a9` passed the unmodified successor gate 353/353 and the strengthened open-work-conflict gate 373 PASS at both render sizes. Returned verification head: `qual/mfm-08-unity-v1` @ `d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727`. #195 proved the missing MapPlanning gate is blocked by a stale exact-five-tab verification contract after the authorised V2 `Treatment review` addition. #156 now waits on #196 only for this MFM-specific closure.
- **MFM-08 Tier-2 Closure 01 #195 is CLOSED — NEEDS CHANGES ROUTED.** Manager review: `Docs/Handoffs/MFM-08-Tier2-Closure-01/MANAGER-REVIEW-20261031.md`. It established that the three Rack red gates remain red on current integration and therefore are not MFM-08-specific.
- **MFM-08 MapPlanning Contract Reconciliation 01 #196 is READY — VERIFICATION-ONLY REPAIR + UNITY.** It may change only the stale navigation-tab verification contract: preserve/assert the original five tabs, assert authorised V2 `Treatment review` as additive, and preserve the historical/non-MFM five-tab behavior. Then run the complete authoritative RackV2MapPlanning gate on exact `e5fb4246...`. No production repair/test weakening authority.
- **Rack V2 Current-Lineage Red-Gate Diagnosis 01 #197 is READY — BROWSER/READ-ONLY.** Diagnose/classify `guided-v2`, `combined-v2`, and `combined-legacy` on exact current integration `ce673714...`. It owns no production/test edits and does not automatically block MFM-08 approval unless it finds evidence directly invalidating accepted Rack V2/D-064 behavior required by MFM-08.
- **Verification Harness Hygiene 01 #193 is MANAGER APPROVED / INTEGRATED / CLOSED.** Reviewed published handoff head `5f9450f4a65e6df8e989a241ffe48d8a3200f6f1`; implementation `b4b68a91d7ab8f1eb24b1956ed53b1e71b416e10`; integration merge `a678fdcfcf3d30541f4bf13066b6feaf2d31dd36`; parent backlog #28 resolved.
- **Research Source Consolidation Audit 01 #194 is MANAGER APPROVED / CLOSED — PLAN ONLY.** Reviewed handoff head `60196617fd0afcbfd45642df8c8d74b3a4eac1bd`; canonical review `Docs/Handoffs/Research-Source-Consolidation-Audit-01/MANAGER-REVIEW-20261031.md`. Accepted planning conclusion: preserve byte-identical originals first; broad PDF bundling/master-PDF is not recommended; no cleanup, deletion, move, watermark, recompression, Drive cleanup or source replacement is authorised by this approval.
- S1-C T-08 is **DEPENDENCY-CLEARED ON T-07 BUT NOT RELEASED TO UNITY YET**. It remains the preferred next history qualification slot after MFM-08 releases Unity, unless Overall Manager explicitly reprioritises.
- S1-D T-08 remains **RELEASED** but serialized behind the preferred S1-C T-08 history slot.
- S1-D T-09 remains prepared and waits for S1-D T-08 PASS.
- MFM-09 remains prepared/not released; it waits for Manager-approved/integrated MFM-08 #156.
- MFM-10 remains prepared/not released; it waits for accepted/integrated MFM-09.
- PARITY-09 remains blocked on MFM-10 `SPECIES GUARD QUALIFIED`.
- MFM-11 remains prepared/not released; it waits for MFM-10 and must resolve the MFM-03 live/cross-year residual.
- MFM-12 remains prepared/not released; it waits for MFM-11 plus the applicable accepted integrated lineage.
- Scenario One Tier-3/player-test candidate remains browser-prepared/not released. No Windows build is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **MFM-08 Stand Map / 3D review UX** (#156) | Manager review / integration hold | candidate `e5fb4246...`; returned harness `d88a32f6...` | no production edits pending #196 | **NEEDS CHANGES — WAITING #196** |
| **MFM-08 MapPlanning Contract Reconciliation 01** (#196) | bounded verification repair + Unity | `verify/mfm-08-mapplanning-contract-reconcile-v1`; intended registered lane `/media/jer/ZX20/CCF-U1` after fresh preflight | navigation-tab verification contract only; no production | **READY — FRESH PREFLIGHT REQUIRED** |
| **Rack V2 Current-Lineage Red-Gate Diagnosis 01** (#197) | browser/read-only diagnosis | browser lane #162; no production worktree | diagnosis/classification only | **READY** |
| **S1-C T-08 final history qualification** | local Tier-2 after MFM-08 release | prepared only; repin at release | qualification only; no production repair by default | **T-07 CLEARED; NOT RELEASED** |
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

### #197 Rack V2 Current-Lineage Red-Gate Diagnosis 01

Browser/static/read-only diagnosis first on exact current integration `ce673714...`. Owns no production/test/fixture edits and no Unity lane. Classify every guided/combined failure family with evidence before proposing repair. If fresh Unity becomes necessary, stop and obtain a separately released serialized phase/claim. This work is separate from #196/#156 unless evidence establishes a direct Rack/D-064 dependency that threatens MFM-08.

### Research source consolidation

Audit #194 is complete and grants no live ownership. Any future backup, deletion, move, derivative marking, recompression or archive reorganisation requires a new reviewed execution packet and explicit user/Manager approval. Evidential originals must remain byte-identical in the designated archive under any approved future plan.

### S1-C T-08

Qualification-only when released. T-07 acceptance fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. No production repair authority.

### S1-D T-08

Released qualification-only task. No production fix authority. Exact released candidate remains `ce673714...` unless Manager explicitly republishes a later inspected descendant.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work.

## Dependency-ready sequence

1. **MFM-specific Unity critical path:** run #196 after fresh preflight/acquisition. Reconcile only the stale tab contract and run the full authoritative RackV2MapPlanning gate on exact `e5fb4246...`.
2. **In parallel without consuming Unity:** run #197 browser/static diagnosis of current-lineage Rack red gates on exact `ce673714...`.
3. On #196 handoff, Manager issues `APPROVED / NEEDS CHANGES / REJECTED`. If the full MapPlanning gate passes under the non-weakened contract and no new MFM-specific failure appears, #156 may be upgraded even while #197 continues, unless #197 has found a direct threat to accepted Rack V2/D-064 behavior required by MFM-08.
4. If #156 is upgraded to APPROVED, safely integrate the exact MFM-08 production delta onto the exact current Scenario One integration lineage after a fresh overlap review; then release MFM-09 from that combined lineage.
5. After MFM-08 releases the serialized Unity lane, fresh-repin/release S1-C T-08 on the exact current integration candidate; then proceed to S1-D T-08 and S1-D T-09. PARITY-03 Q10's remaining Unity measurement remains non-blocking and must compete only when the lane is explicitly scheduled.
6. MFM-09 accepted/integrated -> MFM-10 -> require `SPECIES GUARD QUALIFIED`.
7. MFM-10 green -> PARITY-09 and MFM-11 become eligible subject to ownership/Unity serialization.
8. MFM-11 accepted + applicable accepted Rack/parity/S1-C/S1-D lineage -> MFM-12 integrated qualification.
9. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 accepted/integrated on one exact candidate -> Scenario One Tier-3.
10. Tier-3 success -> user personally playtests the same exact candidate.
11. Windows build only after explicit post-playtest authorisation.

## Durable current records

- MFM-08 returned first Tier-2 Manager review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/TIER2-MANAGER-REVIEW-20261030.md`
- MFM-08 repair packet/handoff/review: `Docs/Handoffs/MFM-08-Geometry-Repair-01/`
- MFM-08 successor repin: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/SUCCESSOR-TIER2-REPIN-20261030.md`
- MFM-08 successor Tier-2 Manager review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/SUCCESSOR-TIER2-MANAGER-REVIEW-20261031.md`
- MFM-08 Tier-2 Closure 01 handoff/review: `Docs/Handoffs/MFM-08-Tier2-Closure-01/`
- MFM-08 MapPlanning Contract Reconciliation 01: `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/TASK-PACKET.md`
- Rack V2 Current-Lineage Red-Gate Diagnosis 01: `Docs/Handoffs/RackV2-Current-Lineage-Red-Gate-Diagnosis-01/TASK-PACKET.md`
- Verification Harness Hygiene 01: `Docs/Handoffs/Verification-Harness-Hygiene-01/`
- Research Source Consolidation Audit 01 handoff/review: `Docs/Handoffs/Research-Source-Consolidation-Audit-01/`
- S1-C T-07: `Docs/Handoffs/S1-C-Implementation-07/`
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`
- OPS Unity lock: `Docs/Handoffs/OPS-UNITY-LOCK-01/`
- S1-D T-08: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09: `Docs/Handoffs/S1-D-Implementation-09/`
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`
