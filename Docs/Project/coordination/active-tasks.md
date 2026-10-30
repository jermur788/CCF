# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after MFM-08 #156 Tier-2 returned a confirmed segment-footprint geometry defect and bounded repair #192 was released.

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
- S1-C T-07 #187 is **MANAGER APPROVED / CLOSED**. It introduced verification/docs only, so it did not move the production integration candidate. Accepted genuine v20-y5 SHA-256: `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`.
- OPS Unity-lock repair #189 is **MANAGER APPROVED / CLOSED**. The live machine-local config repair changed only ZX20 `storage.device` from `/dev/sda2` to `/dev/sdd2`; the normal lock tool returned explicit `free` at that handoff. Any later Unity task must still fresh-preflight and acquire normally.
- **MFM-08 #156 = NEEDS CHANGES.** Static-approved production candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06` compiled and passed most Tier-2 interaction checks, but final Unity qualification confirmed a production geometry defect: Low/Centre/High coverage areas collapse to the same whole-unit polygon/centroid, the source sample is drawn as the whole strip, tags stack and per-area `Inspect in forest` waypoints collapse. Returned verification tip `eefba5eaf35d23e66af9b8ba7304f10b0cb3aa47`; Manager verdict `Docs/Handoffs/MFM-08-Map-3D-Review-UX/TIER2-MANAGER-REVIEW-20261030.md` at main `0ee3e987f7b151002060b06d78a07ed53cfa45af`. Do **not** integrate `d55ec86a...`.
- **MFM-08 Geometry Repair 01 #192 is READY — BOUNDED FIX.** Packet `Docs/Handoffs/MFM-08-Geometry-Repair-01/TASK-PACKET.md` at main `65068743e66f80ed6f160ff417dd605074cc3d0e`; exact start `d55ec86a3f0bf13820f2080197b6b0f43d832e06`.
- S1-C T-08 is **DEPENDENCY-CLEARED ON T-07 BUT NOT RELEASED TO UNITY YET**. Current plan keeps it as the next history qualification slot after MFM-08 is repaired/requalified and releases Unity, unless Overall Manager explicitly reprioritises.
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
| **MFM-08 Geometry Repair 01** (#192) | local bounded fix | `fix/mfm-08-segment-footprints-v1`; `/media/jer/Videos/CCF-MFM08-FIX` suggested | Stand Map treatment-review footprint/waypoint geometry + focused verification | **READY — NOT YET RUNNING** |
| **MFM-08 Stand Map / 3D review UX** (#156) | Unity qualification after #192 | repaired candidate TBD | qualification/integration review only after repair | **NEEDS CHANGES — WAITING #192** |
| **OPS-UNITY-LOCK-01** (#189) | machine-local ops | `/media/jer/ZX20/CCF-Ops` | lock governance repair | **APPROVED / CLOSED** |
| **S1-C T-07 data-contract stability** (#187) | offline verification | `verify/s1-c-t07-data-contract-v1`; `/media/jer/Videos/CCF-S1C-T07` | verification/docs only | **MANAGER APPROVED / CLOSED** |
| **S1-C T-08 final history qualification** | local Tier-2 after MFM-08 release | prepared only; repin at release | qualification only; no production repair by default | **T-07 CLEARED; NOT RELEASED** |
| **S1-D T-08 multi-cycle witness** | local Tier-2 after preferred S1-C T-08 slot | `verify/s1-d-multicycle-witness-v1`; `/media/jer/Videos/CCF-S1D-T08` | qualification only; no production repair by default | **RELEASED; WAITING SERIALIZED UNITY** |
| **S1-D T-09 independent conformance review** | browser/read-only independent review | no production lane | review only | **PREPARED; WAITING S1-D T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | browser prep then local | prepared only; repin at release | curriculum/integration after accepted #156 | **WAITING ACCEPTED/INTEGRATED #156** |
| **MFM-10 species-parity guard** | local qualification after MFM-09 | prepared only; repin at release | verification/guard | **WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | browser repin then local qualification | TBD | qualification | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | local qualification + independent review | prepared only; repin at release | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | local integrated qualification + independent review | prepared only; repin at release | verification/review | **WAITING MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | browser repin then local Tier-3 | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight and acquire through the normal authoritative tool.

## Single-writer boundaries

### #192 MFM-08 Geometry Repair 01

Bounded bug-fix ownership only. Primary production surface is `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs`; one smallest pure helper under `Assets/ForestPrototype/UI/Map/` plus `.meta` is allowed only if genuinely needed. Focused `Tools/Verification/MfmTreatmentReview*` additions/changes are allowed.

Required behavior: derive distinct authoritative Low/Centre/High polygons from `ScenarioOneCoverage.Area.Segment` and the existing rack-axis/anchor segment boundaries; use the actual fixed teaching/sample footprint for the source; make map drawing and `Inspect in forest` consume one shared area-footprint/centre derivation; keep remainders on their authoritative polygons; work for rotated as well as axis-aligned racks.

No coverage classification/calibration/world-adapter changes, no `ScenarioOneRackSampleGeometry` semantic rewrite, no Rack V2 persistence, save/schema/hash/reference, prescription/approval, Work Plan, Annual Review/history, MFM-09, scene/prefab or Windows ownership. If repair requires widening those surfaces, stop and return to Manager.

### #156 MFM-08 qualification

No production repair authority while #192 is active. After #192 handoff and Manager approval, repin #156 to the exact repaired production head and rerun full Tier-2. Keep the returned segment/sample defect assertions; they must become real passes, not be weakened or removed.

The rerun must also cover the deferred Rack V2 map/runtime/field regressions, frozen Reference Future/protected compatibility, applicable MFM-06/07 Unity gates, relevant general UI gate, and an explicit open-work conflict in addition to the already-tested manual-Fell conflict.

### S1-C T-08

Qualification-only when released. T-07 acceptance fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. No production repair authority.

### S1-D T-08

Released qualification-only task. No production fix authority. Exact released candidate remains `ce673714...` unless Manager explicitly republishes a later inspected descendant.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work.

## Dependency-ready sequence

1. **Now:** execute #192 MFM-08 segment-footprint/area-waypoint repair from exact `d55ec86a...`.
2. On #192 handoff, Manager inspects the exact production delta and focused geometry/regression evidence. Verdict `APPROVED / NEEDS CHANGES / REJECTED`.
3. If #192 is APPROVED, repin #156 to the exact repaired production head. Fresh-preflight/acquire Unity, rerun the complete MFM-08 Tier-2 matrix, keep the two geometry defect checks, run deferred regressions/open-work conflict, and return the final MFM-08 handoff.
4. If #156 is APPROVED, safely integrate the repaired MFM-08 production candidate after fresh overlap review. Then release MFM-09 from the exact combined integration lineage.
5. After #156 releases the serialized Unity lane, fresh-repin/release S1-C T-08 on the exact current integration candidate, then proceed to S1-D T-08 and S1-D T-09.
6. MFM-09 accepted/integrated -> MFM-10 -> require `SPECIES GUARD QUALIFIED`.
7. MFM-10 green -> PARITY-09 and MFM-11 become eligible subject to ownership/Unity serialization.
8. MFM-11 accepted + applicable accepted Rack/parity/S1-C/S1-D lineage -> MFM-12 integrated qualification.
9. Rack V2 G4 + PARITY-09 + S1-C + S1-D + MFM-12 accepted/integrated on one exact candidate -> Scenario One Tier-3.
10. Tier-3 success -> user personally playtests the same exact candidate.
11. Windows build only after explicit post-playtest authorisation.

## Durable current records

- MFM-08 Tier-2 Manager review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/TIER2-MANAGER-REVIEW-20261030.md`
- MFM-08 repair packet: `Docs/Handoffs/MFM-08-Geometry-Repair-01/TASK-PACKET.md`
- MFM-08 returned Unity handoff: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md` on `qual/mfm-08-unity-v1` at `eefba5ea...`
- S1-C T-07: `Docs/Handoffs/S1-C-Implementation-07/`
- S1-C T-08: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`
- OPS Unity lock: `Docs/Handoffs/OPS-UNITY-LOCK-01/`
- S1-D T-08: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09: `Docs/Handoffs/S1-D-Implementation-09/`
- MFM-09/10/11/12: corresponding `Docs/Handoffs/MFM-*` folders.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
