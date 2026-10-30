# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after browser claims #180–#184, MFM-03 stale-claim closeout, downstream MFM-10/11/12 preparation and Tier-3 preparation; #151 Unity qualification is still in progress per its latest claim record.

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

- Published Scenario One integration branch remains `integration/scenario-one-player-test-ready-v1` at **`8fa83bff22ea1b4943689bee1074eed8eae65d2f`** as of this reconciliation. It contains accepted S1-C T-06, S1-D T-04P / T-04, MFM-08A, save schema v24 and PARITY-08 lineages, but not the in-flight #151/#156 or downstream MFM-09+ work.
- Verification reconciliation #165 is Manager-approved/integrated. Worker tip `508c2d07741f97c47d9ed008306e30eb70666f72`; integration handoff/docs successor `b6b96cb3a51e80bd6b61df7dc73b1c1237950a32`.
- PARITY-08 #126 is **INDEPENDENTLY APPROVED, INTEGRATED AND CLOSED**. Exact reviewed production candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`; independent review `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW.md`; exact two-parent integration merge **`7d2049e862df5c9311748167423b981e713d1fa9`**; integration handoff/docs successor **`8fa83bff22ea1b4943689bee1074eed8eae65d2f`**.
- The PARITY-08 merge adds exactly the six reviewed candidate paths to the prior integration lineage and preserves only already-approved #165 verification/documentation changes from the other parent. No parity-specific schema field, Reference Future change or protected-anchor rebaseline occurred.
- #166 Sitka timber residual is accepted: fixtures 120, assertions 3007, evidence SHA `88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8`.
- #167 baseline discrimination is complete/closed: the added `completion-model0` diagnostic fails identically on accepted base `185200f5` and PARITY-08 candidate `3eb9c954`, including completion hash `046551EB29730CE1`, v16 layout hash `20B85B0152EA747A` and identical v17/v18 hashes. It is recorded as pre-existing D-047/D-048 verification debt, not a #126 regression. Historical anchor `6F84AF319D301F87` remains unchanged. Positive old-model-0 save/load compatibility remains evidenced by the green frozen save-v12 Reference Future load/continuation path.
- S1-D T-05 #151 is **OFFLINE/STATIC APPROVED; UNITY QUALIFICATION RUNNING per the feature claim**. Exact start `cfdc93bf3943125d21570bc5749a9338f52d1cce`; production `98ac98b5a33929d30b150dc6e4692d8bdf2b9bc1`; verification `38904535ceb45da1b9112a7695c7c7295462efec`; handed-off head `4ddf4aa535d4ec5267be46d2635679411d595e0f`. Independent offline review approved 219/219 + 7/7 mutations. Latest durable claim remains `[RUNNING — UNITY]`; no completed Unity handoff, Manager runtime/rendered verdict or integration exists yet. Final Manager checklist is prepared at `Docs/Handoffs/S1-D-Implementation-05/FINAL-MANAGER-REVIEW-CHECKLIST-20261030.md`.
- MFM-08 #156 is **STATIC/OFFLINE PRE-REVIEW APPROVED / UNITY TIER-2 PENDING**. Production `d55ec86a3f0bf13820f2080197b6b0f43d832e06`; docs tip `d70cec1781c104794a6f225c8609847c55df1510`; browser pre-review `Docs/Handoffs/MFM-08-Map-3D-Review-UX/BROWSER-PRE-REVIEW-20261030.md`; MFM-08 offline 106/106 with MFM-04/05/07 unchanged. Not integrated. Final Manager checklist is prepared at `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-REVIEW-CHECKLIST-20261030.md`.
- MFM-03 claim #39 is **APPROVED/CLOSED** as the accepted static GAMEPLAY CALIBRATION input used by MFM-04+. Manager closeout: `Docs/Handoffs/MFM-Implementation-03/MANAGER-REVIEW.md`. The independent-review N1–N6 caveats, especially unmeasured live post-Lesson-5/cross-year drift, are not erased; they are explicitly carried into MFM-11 Calibration Impact Qualification.
- MFM-09 browser preparation #180 is **COMPLETE/CLOSED but NOT RELEASED FOR LOCAL IMPLEMENTATION**. Readiness: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/BROWSER-READINESS-20261030.md`; prepared packet: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/TASK-PACKET.md`. Preparation reference `8fa83bff22ea1b4943689bee1074eed8eae65d2f` is not an implementation start SHA. Release requires Manager-approved/integrated #151 + #156 and a fresh repin to their combined exact integration lineage.
- MFM-10 browser preparation #181 is **COMPLETE/CLOSED but NOT RELEASED**. Readiness and packet are under `Docs/Handoffs/MFM-10-Species-Parity-Guard/`. Release requires accepted/integrated MFM-09 and a fresh exact candidate. Qualification-only by default; exact gate remains `SPECIES GUARD QUALIFIED`.
- MFM-11 browser preparation #182 is **COMPLETE/CLOSED but NOT RELEASED**. Readiness and packet are under `Docs/Handoffs/MFM-11-Calibration-Impact/`. Release requires MFM-10 green on the accepted lineage. It must resolve the MFM-03 live/cross-year residual and returns `MULTI-SCALE CALIBRATION QUALIFIED` or `BLOCKED — CALIBRATION IMPACT`; approved routing also requires an independent Claude review.
- MFM-12 browser preparation #183 is **COMPLETE/CLOSED but NOT RELEASED**. Readiness and packet are under `Docs/Handoffs/MFM-12-Integrated-Verification/`. It requires the complete MFM requirements matrix, integrated Unity/rendered/save/compatibility/complete-loop evidence and independent architecture/spec review. Exact success verdict: `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`.
- PARITY-09 browser readiness #177 is complete: PARITY-03 through PARITY-08 are durably accepted/integrated on the published lineage, but the authoritative PARITY-09 packet also requires **MFM-10 `SPECIES GUARD QUALIFIED`** before local PARITY-09 starts. No MFM-10 qualification exists yet, so PARITY-09 is **NOT READY**. Premature claim #179 was withdrawn/closed without execution.
- Scenario One Tier-3 / personal-player-test browser skeleton is prepared under #184 at `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md` and is **NOT RELEASED**. It requires one exact candidate with all required Rack/parity/S1-C/S1-D/MFM-12 gates accepted/integrated.
- #167 reported the Unity lock released normally, U1 restored clean, no Editor running and host saves unchanged. This is historical evidence only; #151 subsequently acquired the serialized Unity lane. No row/file here proves current lock/process state; every Unity worker must fresh-preflight.
- No Windows build has occurred or is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **S1-D T-05 previous vs proposed** (#151) | local UI/Unity | `feature/s1-d-t05-previous-proposed-v1`; verification child `verify/s1d-t05-unity-v1` | bounded Work Plan + authorised read-only Annual Review route; verification-only Unity by default | **UNITY QUALIFICATION RUNNING — NO MANAGER RUNTIME VERDICT YET** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **STATIC PRE-REVIEW APPROVED; UNITY AFTER #151** |
| **MFM-09 teaching / Work Plan integration** | browser prep then local | prepared only; final branch/worktree repinned at release | curriculum/integration after accepted #151 + #156 | **BROWSER PACKET PREPARED #180; WAITING ON ACCEPTED/INTEGRATED #151 + #156** |
| **MFM-10 species-parity integration guard** | local qualification after MFM-09 | prepared only; repin at release | verification/guard only unless a defect is separately assigned | **BROWSER PACKET PREPARED #181; WAITING ON MFM-09** |
| **PARITY-09 integrated parity qualification** | browser repin then local qualification | TBD | no production ownership until all start gates true | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | local qualification + independent review | prepared only; repin at release | measurement/qualification only; no retuning by default | **BROWSER PACKET PREPARED #182; WAITING ON MFM-10** |
| **MFM-12 integrated MFM qualification** | local Tier-2/integrated qualification + independent review | prepared only; repin at release | verification/review only unless defect separately assigned | **BROWSER PACKET PREPARED #183; WAITING ON MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | browser repin then local Tier-3 | prepared skeleton only | final integrated qualification; no Windows | **BROWSER SKELETON PREPARED #184; NOT RELEASED** |
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

1. Finish **#151 Unity qualification** on exact pushed head `4ddf4aa535d4ec5267be46d2635679411d595e0f`; then use the prepared final Manager checklist for browser rendered/runtime review and safe integration if approved.
2. Run **#156 Unity Tier-2 qualification** only after #151 releases the serialized Unity lane; then use the prepared final Manager checklist and integrate if approved.
3. After accepted/integrated #151 + #156, **release the prepared MFM-09 packet**: re-inspect accepted teaching/Work Plan/provenance surfaces, enumerate existing stable saved teaching IDs, repin the exact combined integration SHA, create the substantive local claim, then implement/qualify/integrate MFM-09.
4. **Release and run MFM-10** on the accepted post-MFM-09 lineage. Require exact success token `SPECIES GUARD QUALIFIED`.
5. Once MFM-10 is durably green, two downstream gates become eligible subject to ownership/Unity serialization: (a) re-run **PARITY-09 browser readiness**, then release/qualify/integrate PARITY-09; (b) release **MFM-11 calibration-impact qualification**, resolve the MFM-03 live/cross-year residual, obtain independent review and require `MULTI-SCALE CALIBRATION QUALIFIED`.
6. After MFM-11 and the applicable accepted integrated parity/S1-C/S1-D/Rack lineage are pinned, release **MFM-12 integrated MFM qualification** and require exact verdict `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` plus independent architecture/spec review.
7. When all required Rack/parity/S1-C/S1-D/MFM-12 work is accepted/integrated on one exact candidate, repin and release **Scenario One Tier-3** using the prepared skeleton. Tier-3 must prove complete-loop reachability/understandability and return parent token `SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`.
8. User personally playtests the same exact candidate.
9. Windows build only after explicit post-playtest authorisation.

## Durable current records

- #126 implementation: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/HANDOFF.md`
- #126 independent review: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INDEPENDENT-REVIEW.md`
- #126 integration: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/INTEGRATION-HANDOFF.md`
- #166 partial residual: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/RESIDUAL-UNITY-HANDOFF.md`
- #167 baseline result: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/MODEL0-BASELINE-HANDOFF.md`
- #151 handoff: `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md`
- #151 final-review checklist: `Docs/Handoffs/S1-D-Implementation-05/FINAL-MANAGER-REVIEW-CHECKLIST-20261030.md`
- #177 PARITY-09 browser readiness: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/BROWSER-READINESS-20261030.md`
- #156 handoff: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`
- #178 MFM-08 browser pre-review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/BROWSER-PRE-REVIEW-20261030.md`
- #156 final-review checklist: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-REVIEW-CHECKLIST-20261030.md`
- MFM-03 Manager closeout: `Docs/Handoffs/MFM-Implementation-03/MANAGER-REVIEW.md`
- #180 MFM-09 readiness + prepared packet: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/`
- #181 MFM-10 readiness + prepared packet: `Docs/Handoffs/MFM-10-Species-Parity-Guard/`
- #182 MFM-11 readiness + prepared packet: `Docs/Handoffs/MFM-11-Calibration-Impact/`
- #183 MFM-12 readiness + prepared packet: `Docs/Handoffs/MFM-12-Integrated-Verification/`
- #184 Tier-3 browser skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
