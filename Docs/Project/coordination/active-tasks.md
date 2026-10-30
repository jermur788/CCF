# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after browser claims #177/#178; #151 Unity qualification is in progress per its latest claim record.

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

- Scenario One integration contains the accepted S1-C T-06, S1-D T-04P / T-04, MFM-08A and save schema v24 lineages.
- Verification reconciliation #165 is Manager-approved/integrated. Worker tip `508c2d07741f97c47d9ed008306e30eb70666f72`; integration handoff/docs successor `b6b96cb3a51e80bd6b61df7dc73b1c1237950a32`.
- PARITY-08 #126 is **INDEPENDENTLY APPROVED, INTEGRATED AND CLOSED**. Exact reviewed production candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`; independent review `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW.md`; exact two-parent integration merge **`7d2049e862df5c9311748167423b981e713d1fa9`**; integration handoff/docs successor **`8fa83bff22ea1b4943689bee1074eed8eae65d2f`**.
- The PARITY-08 merge adds exactly the six reviewed candidate paths to the prior integration lineage and preserves only already-approved #165 verification/documentation changes from the other parent. No parity-specific schema field, Reference Future change or protected-anchor rebaseline occurred.
- #166 Sitka timber residual is accepted: fixtures 120, assertions 3007, evidence SHA `88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`.
- #167 baseline discrimination is complete/closed: the added `completion-model0` diagnostic fails identically on accepted base `185200f5` and PARITY-08 candidate `3eb9c954`, including completion hash `046551EB29730CE1`, v16 layout hash `20B85B0152EA747A` and identical v17/v18 hashes. It is recorded as pre-existing D-047/D-048 verification debt, not a #126 regression. Historical anchor `6F84AF319D301F87` remains unchanged. Positive old-model-0 save/load compatibility remains evidenced by the green frozen save-v12 Reference Future load/continuation path.
- S1-D T-05 #151 is **OFFLINE/STATIC APPROVED; UNITY QUALIFICATION RUNNING per the feature claim**. Exact start `cfdc93bf3943125d21570bc5749a9338f52d1cce`; production `98ac98b5a33929d30b150dc6e4692d8bdf2b9bc1`; verification `38904535ceb45da1b9112a7695c7c7295462efec`; handed-off head `4ddf4aa535d4ec5267be46d2635679411d595e0f`. Independent offline review approved 219/219 + 7/7 mutations. No Manager runtime/rendered verdict and no integration yet.
- MFM-08 #156 is **STATIC/OFFLINE PRE-REVIEW APPROVED / UNITY TIER-2 PENDING**. Production `d55ec86a3f0bf13820f2080197b6b0f43d832e06`; docs tip `d70cec1781c104794a6f225c8609847c55df1510`; browser pre-review `Docs/Handoffs/MFM-08-Map-3D-Review-UX/BROWSER-PRE-REVIEW-20261030.md`; MFM-08 offline 106/106 with MFM-04/05/07 unchanged. Not integrated.
- PARITY-09 browser readiness #177 is complete: PARITY-03 through PARITY-08 are durably accepted/integrated on the published lineage, but the authoritative PARITY-09 packet also requires **MFM-10 `SPECIES GUARD QUALIFIED`** before local PARITY-09 starts. No MFM-10 qualification exists yet, so PARITY-09 is **NOT READY**. Premature claim #179 was withdrawn/closed without execution.
- #167 reported the Unity lock released normally, U1 restored clean, no Editor running and host saves unchanged. This is historical evidence only; the next Unity task must still fresh-preflight the machine.
- No Windows build has occurred or is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **S1-D T-05 previous vs proposed** (#151) | local UI/Unity | `feature/s1-d-t05-previous-proposed-v1`; verification child `verify/s1d-t05-unity-v1` | bounded Work Plan + authorised read-only Annual Review route; verification-only Unity by default | **UNITY QUALIFICATION RUNNING — NO MANAGER RUNTIME VERDICT YET** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **STATIC PRE-REVIEW APPROVED; UNITY AFTER #151** |
| **MFM-09** | browser prep then local | TBD | curriculum/integration after accepted #151 + #156 | **WAITING ON #151 + #156** |
| **MFM-10 species-parity integration guard** | local qualification after MFM-09 | TBD | verification/guard only unless a defect is separately assigned | **WAITING ON MFM-09** |
| **PARITY-09 integrated parity qualification** | browser repin then local qualification | TBD | no production ownership until all start gates true | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight.

## Single-writer boundaries

### #151

Offline/static implementation and independent review are accepted for the current candidate. Unity qualification owns no new production by default: test exact pushed lineage first. Required runtime/render review includes import/compile, three-cycle presentation at 1280×720 and 1920×1080, Approved / Pending new / Pending carried-over grouping, future-year inconsistency treatment, MFM prescription-created work remaining ordinary Pending before Work Plan approval, exact creation-year Annual Review navigation, read-only history behavior, Executed/unplanned-Fell-mark visibility required by AC-5, and layout/collapsed-foldout readability. Any genuine production defect stops for bounded repair ownership; no save/schema/history-store/MFM-09 work.

### #156

Owns Stand Map plus the smallest MFM-specific/inspection presentation helpers. No Annual Review/history, S1-D generic Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk-confirmation changes.

Browser #178 accepted the exact source/static candidate only. Coverage classification remains exclusively through `ScenarioOneManager.ReadCoverage`; rack-sample polygons and living-tree facts are accepted as presentation/current-state reads rather than alternate coverage/biology authority. No API/product blocker was found. Tier-2 Unity qualification remains required at 1280×720 and 1920×1080 to prove `Painter2D`/scroll compile-render behavior, live footprint alignment, layer/input ordering, candidate/proposal distinction from Fell/Crop/Rack marks, `[C]/[N]` interactions, spot-check/approval/Work Plan boundary, conflicts, regressions and Reference/protected compatibility.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work. Mixed tasks must be split rather than consuming a local lane for browser-only phases.

## Dependency-ready sequence

1. Finish **#151 Unity qualification** on exact pushed head `4ddf4aa535d4ec5267be46d2635679411d595e0f`; then BROWSER-CAPABLE final Manager rendered/runtime review and safe integration if approved.
2. Run **#156 Unity Tier-2 qualification** only after #151 releases the serialized Unity lane; browser/static pre-review is already complete, then final Manager review/integration if green.
3. After accepted/integrated #151 + #156, prepare/release and complete **MFM-09**.
4. Run **MFM-10 species-parity integration guard** on the accepted combined lineage and require exact success token `SPECIES GUARD QUALIFIED`.
5. Re-run **PARITY-09 browser readiness** against the then-current exact integration SHA. Only after the MFM-10 gate is durably green may a substantive local PARITY-09 claim be created/released.
6. Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM work integrated on one exact candidate; then user personal playtest.
7. Windows build only after explicit post-playtest authorisation.

## Durable current records

- #126 implementation: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/HANDOFF.md`
- #126 independent review: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW.md`
- #126 integration: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INTEGRATION-HANDOFF.md`
- #166 partial residual: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/RESIDUAL-UNITY-HANDOFF.md`
- #167 baseline result: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/MODEL0-BASELINE-HANDOFF.md`
- #151 handoff: `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md`
- #177 PARITY-09 browser readiness: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/BROWSER-READINESS-20261030.md`
- #156 handoff: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`
- #178 MFM-08 browser pre-review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/BROWSER-PRE-REVIEW-20261030.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
