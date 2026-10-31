# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-31 by Overall Manager / OpenAI GPT-5.6 Sol after reviewing the returned MFM-08 successor Tier-2. MFM-08 feature behavior is qualified on its exact repaired candidate, but #156 remains **NEEDS CHANGES — VERIFICATION CLOSURE** pending #195. Research Source Consolidation Audit 01 #194 remains independently available as analysis-only work.

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
- **MFM-08 #156 is NEEDS CHANGES — VERIFICATION CLOSURE ONLY.** Exact repaired candidate `e5fb424635f16182748efcb0539e8a49539495a9` passed the unmodified successor gate 353/353 and the strengthened open-work-conflict gate 373 PASS at both render sizes. Returned verification head: `qual/mfm-08-unity-v1` @ `d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727`. Feature behavior is accepted as qualified, but integration is held because `RackV2MapPlanning` was not run and the disclosed Rack red gates were compared only to old feature start `cfdc93bf...`, which is 29 commits behind current integration `ce673714...`.
- **MFM-08 Tier-2 Closure 01 #195 is READY — UNITY VERIFICATION ONLY.** It must run authoritative RackV2MapPlanning on exact `e5fb4246...` and diagnose `guided-v2`, `combined-v2`, and `combined-legacy` on exact current integration `ce673714...`. No production repair/test weakening authority.
- **Verification Harness Hygiene 01 #193 is MANAGER APPROVED / INTEGRATED / CLOSED.** Reviewed published handoff head `5f9450f4a65e6df8e989a241ffe48d8a3200f6f1`; implementation `b4b68a91d7ab8f1eb24b1956ed53b1e71b416e10`; integration merge `a678fdcfcf3d30541f4bf13066b6feaf2d31dd36`; parent backlog #28 resolved.
- **Research Source Consolidation Audit 01 #194 is READY — ANALYSIS ONLY.** It may inventory/hash/compare research sources and propose reversible consolidation/archive options, but it may not delete, move, merge, watermark, recompress or mutate originals.
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
| **MFM-08 Stand Map / 3D review UX** (#156) | Manager review / integration hold | candidate `e5fb4246...`; returned harness `d88a32f6...` | no production edits pending closure | **NEEDS CHANGES — WAITING #195** |
| **MFM-08 Tier-2 Closure 01** (#195) | bounded Unity verification | `verify/mfm-08-tier2-closure-v1`; clean registered Unity lane | Rack map-planning + current-lineage diagnosis only | **READY — FRESH PREFLIGHT REQUIRED** |
| **Research Source Consolidation Audit 01** (#194) | research analysis | `research/source-consolidation-audit-v1`; `/media/jer/Videos/CCF-RESEARCH-CONSOLIDATION-AUDIT` | inventory/plan only; no source mutation | **READY — ANALYSIS ONLY** |
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

Integration is held only for #195 closure. Do not modify MFM-08 production under #156 or #195 without a new Manager repair claim.

### #195 MFM-08 Tier-2 Closure 01

Verification-only. Fresh-preflight and acquire Unity normally. On exact `e5fb4246...`, run authoritative `RackV2MapPlanning`. On exact current integration `ce673714...`, run current `guided-v2`, `combined-v2`, and `combined-legacy` diagnostically. Do not repair production or tests. Preserve accepted #156 MFM evidence; the full 373-check MFM gate does not need repetition unless a new material ambiguity appears. Return clean lane/Editor/lock state. No Windows.

### #194 Research Source Consolidation Audit 01

Analysis-only ownership of its handoff/inventory outputs. It may inspect the Game Dev project-source corpus, `Docs/Research`, accessible local CCF research archives and accessible distribution/archive copies; compute file sizes/hashes and duplication/provenance relationships; and propose consolidation/archive options. It may not delete, move, rename, merge, watermark, recompress, replace or detach any original research/source file. Any later destructive or mutating cleanup requires a separate reviewed execution packet and explicit user/Manager approval. Evidential originals must remain byte-identical in a designated archive under any future plan; visible watermarks/banners may apply only to derivatives.

### S1-C T-08

Qualification-only when released. T-07 acceptance fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. No production repair authority.

### S1-D T-08

Released qualification-only task. No production fix authority. Exact released candidate remains `ce673714...` unless Manager explicitly republishes a later inspected descendant.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work.

## Dependency-ready sequence

1. **Now, Unity critical path:** execute #195 after fresh preflight/acquisition. Run RackV2MapPlanning on `e5fb4246...` and diagnose the three Rack red gates on current integration `ce673714...`.
2. **Now, independent analysis:** #194 may run without Unity and without mutating source/archive files.
3. On #195 handoff, Manager issues `APPROVED / NEEDS CHANGES / REJECTED` for the closure and then upgrades or retains #156 accordingly.
4. If #195 closes the gap and #156 is upgraded to APPROVED, safely integrate the exact MFM-08 production delta onto the exact current Scenario One integration lineage after a fresh overlap review; then release MFM-09 from that combined lineage.
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
- MFM-08 Tier-2 Closure 01: `Docs/Handoffs/MFM-08-Tier2-Closure-01/TASK-PACKET.md`
- Verification Harness Hygiene 01: `Docs/Handoffs/Verification-Harness-Hygiene-01/`
- Research Source Consolidation Audit 01: `Docs/Handoffs/Research-Source-Consolidation-Audit-01/`
- S1-C T-07: `Docs/Handoffs/S1-C-Implementation-07/`
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`
- OPS Unity lock: `Docs/Handoffs/OPS-UNITY-LOCK-01/`
- S1-D T-08: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09: `Docs/Handoffs/S1-D-Implementation-09/`
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`
