# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after #166 partial qualification and Manager baseline disposition.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- Browser-capable phases must be separated from local implementation/Unity work wherever practical; #162 is the browser lane.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- Scenario One integration includes S1-D T-04P #143 and T-04 #124 at `185200f57140c65ed4d540ff0c4d173451170d14`, save schema v24.
- Verification reconciliation #165 is Manager-approved/integrated. Worker tip `508c2d07741f97c47d9ed008306e30eb70666f72`; integration handoff/docs successor `b6b96cb3a51e80bd6b61df7dc73b1c1237950a32`.
- PARITY-08 #126 exact production candidate is **`3eb9c954c363659a1b012e06ce58a35ab87f4303`**, not integrated. Production diff is limited to pre-mutation unknown-species save validation plus focused verification; no schema/anchor/reference rebaseline.
- #126 clean evidence already covers compile, unknown-species SaveHardening/no-mutation, save/provenance, byte-identical hash dump, frozen Reference Future v1, teaching latch, approval, N3, Legacy40 interaction, progress, model-1 completion, model-2 density/save and protected Sitka timber anchor.
- #166 is **PARTIAL / CLOSED**. Sitka timber anchor PASS on real Editor: fixtures 120, assertions 3007, evidence SHA `88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`. `completion-model0` failed on candidate and was correctly stopped; no production/anchor/harness semantics were changed.
- D-047 records completion anchor `6F84AF319D301F87` for regeneration-model-1 / growth-model-0; D-048 says legacy/growth-0 anchors are retained. No rebaseline or reconstruction change is authorised.
- Positive old-model-0 save/load compatibility is separately evidenced by frozen Reference Future v1: archive save version 12, and production `LoadData` explicitly restores every pre-v17 save with `GrowthModel.Legacy` (0); the Reference contract load/continuation gate is green on #126.
- #128 independent PARITY-08 source/static precheck is complete with no implementation defect found; final verdict waits only for #167 baseline attribution.
- S1-D T-05 #151 remains offline/static approved (`98ac98b5` production / `4ddf4aa5` docs), Unity rendered qualification pending, not integrated.
- MFM-08 #156 remains offline candidate (`d55ec86a` production / `d70cec17` docs), Tier-2 Unity qualification pending, not integrated.
- No Windows build has occurred or is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **PARITY-08 compatibility** (#126) | save/determinism | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | candidate frozen; no production edits while attribution/review runs | **CANDIDATE `3eb9c954`; #167 + #128 pending; NOT INTEGRATED** |
| **PARITY-08 model-0 baseline discrimination** (#167) | verification/Unity | `verify/parity-08-model0-baseline-v1`; `/media/jer/Videos/CCF-Parity08-Model0Baseline` | verification only; exact baseline target `185200f5` | **READY — NEXT CRITICAL UNITY TASK** |
| **PARITY-08 independent review** (#128) | browser-first independent review | exact candidate `3eb9c954` + accepted runtime evidence | read-only | **SOURCE PRECHECK COMPLETE; FINAL VERDICT WAITS ON #167** |
| **S1-D T-05 previous vs proposed** (#151) | local UI/Unity | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | bounded Work Plan/S1-C nav surface | **OFFLINE APPROVED; UNITY PENDING** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **OFFLINE CANDIDATE; UNITY PENDING** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight.

## #167 exact decision boundary

Run the **identical** #166 `completion-model0` verifier semantics against exact accepted pre-PARITY-08 baseline `185200f57140c65ed4d540ff0c4d173451170d14`.

- Baseline PASS / candidate FAIL => candidate regression; #126 returns NEEDS CHANGES.
- Baseline materially identical FAIL => pre-existing D-047/D-048 historical completion verification debt, not introduced by #126. Do not rebaseline; #128 may disposition the extra completion diagnostic while relying on the already-green Reference Future v1 old-model-0 load/continuation evidence for PARITY-08 compatibility.
- Materially different baseline outcome => stop for Manager comparison.

No production, reconstruction, expected-value or anchor changes under #167.

## Dependency-ready sequence

1. **#167** one-gate baseline discrimination on exact `185200f5`.
2. **#128 BROWSER-CAPABLE final review** immediately after #167 returns.
3. If #128 APPROVED, Manager integrate #126 and advance the parity chain toward PARITY-09.
4. Once #167 releases Unity, run **#151 Unity** and **#156 Unity** sequentially by critical priority while browser review/integration runs in parallel.
5. After accepted/integrated #151 + #156, prepare MFM-09 in browser before local implementation.
6. Final Tier-3 on one exact combined candidate; then user personal playtest. Windows build only after explicit post-playtest authorisation.

## Durable current records

- #126 implementation handoff: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/HANDOFF.md`
- #128 source precheck: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW-PRECHECK-20261030.md`
- #166 partial handoff: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/RESIDUAL-UNITY-HANDOFF.md`
- #167 packet: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/MODEL0-BASELINE-DISCRIMINATION-20261030.md`
- #151 handoff: `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md`
- #156 handoff: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`
