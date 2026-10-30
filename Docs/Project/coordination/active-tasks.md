# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after review/integration of #165 and explicit PARITY-08 residual disposition.

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

- MFM-07 remains integrated/verified through `c8cfae8c4025cec6e960a050edf4bfde2a981de5` (save schema v23).
- MFM-08A #129 is Manager-approved/integrated; reviewed production `716486a21588e76a482d3eaea4fed12069f75ffd`, integration handoff successor `182c2c480465a34b269a07ba654d98669c8d8436`.
- S1-C T-06 #147 is Manager-approved/integrated/closed; exact S1-C integration merge `625e0adf5e7fd71184cb074e85e1901b417ec02d`, docs successor `cfdc93bf3943125d21570bc5749a9338f52d1cce`.
- S1-D T-04P #143 is independently reviewed and integrated at production merge `2f476a52` / docs `bccd2b90b3eafc3728a9f29ac421b2719d042d72`; save schema v24. No hash/reference rebaseline.
- S1-D T-04 guide monotonicity #124 is accepted/integrated at `185200f57140c65ed4d540ff0c4d173451170d14`; #164 verification reconciliation is included there. Production unchanged by #124/#164.
- S1-D v24 legacy gate reconciliation wave 2 #165 is **Manager-APPROVED, INTEGRATED AND CLOSED**. Verification-only worker tip `508c2d07741f97c47d9ed008306e30eb70666f72` was a direct descendant of the previous umbrella and was fast-forward integrated; integration handoff docs successor is **`b6b96cb3a51e80bd6b61df7dc73b1c1237950a32`**. Accepted: progress, completion, completion-model1 and density-save all have clean exit-0 receipts on exact PARITY-08 candidate `3eb9c954...`; `progress` and `density-save` isolated-save allowances are accepted because those gates intentionally call `ForestSaveController.Save()` while host-save/cleanup protections remain unchanged.
- PARITY-08 #126 exact production candidate is **`3eb9c954c363659a1b012e06ce58a35ab87f4303`**, not integrated. Offline/static PASS; clean Unity evidence covers compile, unknown-species SaveHardening/no-mutation, save/provenance, byte-identical 18-line hash dump, Reference Future v1, v24 teaching latch, approval, N3, Legacy40 interaction, progress, model-1 completion, model-2 completion/density-save and protected Legacy40 density anchors.
- PARITY-08 still lacks two mandatory Unity evidence items from its binding packet: positive **model-0 compatibility/save-load replay** and protected **Sitka timber anchors**. These are explicit claim #166; they are not waived.
- #128 independent PARITY-08 review is read-only and may perform static/browser precheck now; final `APPROVED / NEEDS CHANGES / REJECTED` waits for #166. Precheck record: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW-PRECHECK-20261030.md`.
- S1-D T-05 #151 is offline/static approved: production `98ac98b5`, verification/docs successor `4ddf4aa5`; rendered three-cycle Unity qualification remains pending. Not integrated.
- MFM-08 #156 is offline candidate: production `d55ec86a`, docs `d70cec17`; Tier-2 Unity import/render/interaction remains pending. Not integrated.
- Storage remediation #152 is historical complete. Overnight Unity work observed roughly 19.5–20 GiB free on ZX20, but that is not current proof. Every Unity acquisition must fresh-check >=15 GiB mandatory and prefer >=20 GiB headroom.
- Intermittent Unity Editor exit hangs after PASS have occurred; a hung receipt is never accepted and must be rerun cleanly.
- No Windows build has occurred or is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | orchestration/integration | orchestration + child lanes | orchestration only | **RUNNING / overnight result published; programme continues** |
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **PARITY-08 compatibility** (#126) | save/determinism | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | candidate frozen; no new production edits while residual verification/review runs | **CANDIDATE `3eb9c954`; #166 + #128 pending; NOT INTEGRATED** |
| **PARITY-08 residual Unity evidence** (#166) | verification/Unity | suggested `verify/parity-08-residual-unity-v1`; `/media/jer/Videos/CCF-Parity08-Residual` | verification-only model-0 + Sitka timber harness/runner | **READY — NEXT CRITICAL UNITY TASK** |
| **PARITY-08 independent review** (#128) | browser-first independent review | exact candidate `3eb9c954` + accepted #165 + future #166 receipts | read-only | **STATIC PRECHECK READY; FINAL VERDICT WAITS ON #166** |
| **S1-D T-05 previous vs proposed** (#151) | local UI/Unity | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | bounded Work Plan/S1-C nav surface | **OFFLINE APPROVED; UNITY PENDING** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **OFFLINE CANDIDATE; UNITY PENDING** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight.

## Single-writer boundaries

### #126 / #166 / #128

#126 production candidate `3eb9c954...` is frozen for qualification/review. #166 may change only verification runner/harness plumbing needed to drive model-0 compatibility and existing Sitka timber-anchor checks in Unity; it must not change production, save/schema/validator/hash/reference behavior, fixtures, anchors, economics or calibration. A genuine production/anchor failure returns to Manager. #128 is read-only; repairs return to the owning implementation claim/new repair claim.

### #151

Primary ownership is `WorkPlanView.cs` plus the smallest already-reviewed S1-C exact-year navigation hook. No save/schema/history-store duplication or MFM-09 curriculum ownership. Unity qualification must use the exact pushed candidate and render 1280×720 and 1920×1080 three-cycle states.

### #156

Owns Stand Map plus the smallest MFM-specific/inspection presentation helpers. No Annual Review/history, S1-D generic Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk-confirmation changes. Tier-2 Unity qualification remains required.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work. Mixed tasks must be split rather than consuming a local lane for browser-only phases.

## Dependency-ready sequence

1. **#166 Unity residual qualification** on exact PARITY-08 candidate `3eb9c954...`.
2. In parallel, **#128 browser/static precheck** may continue; final verdict immediately after #166 returns.
3. If #166 green + #128 APPROVED, Manager final review/integrate #126, then continue parity dependency toward PARITY-09.
4. Once #166 releases Unity, run **#151 Unity** and **#156 Unity** sequentially by critical priority, while browser review/integration work runs in parallel.
5. After accepted/integrated #151 + #156 and accepted S1-D T-04, prepare MFM-09 in browser, then local implementation/qualification.
6. Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM work integrated on one exact candidate; then user personal playtest.
7. Windows build only after explicit post-playtest authorisation.

## Durable current records

- Overnight result: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-RESULT-20261030.md`
- Overnight orchestration ledger: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/ORCHESTRATION-LEDGER-20261030.md`
- #165 worker handoff: `Docs/Handoffs/S1-D-v24-Legacy-Gate-Reconciliation-2/HANDOFF.md`
- #165 integration handoff: `Docs/Handoffs/S1-D-v24-Legacy-Gate-Reconciliation-2/INTEGRATION-HANDOFF.md`
- #126 implementation handoff: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/HANDOFF.md`
- #166 packet: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/RESIDUAL-UNITY-QUALIFICATION-20261030.md`
- #128 precheck: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW-PRECHECK-20261030.md`
- #151 handoff: `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md`
- #156 handoff: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
