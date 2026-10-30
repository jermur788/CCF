# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after browser claims #180–#186, Rack V2 G4 final review, S1-C T-07/T-08 and S1-D T-08/T-09 preparation, and release of offline S1-C T-07 claim #187. #151 Unity qualification remains in progress per its latest durable claim comment.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, lock and Editor-process state immediately before acquisition.
- Browser-capable phases should be separated from local implementation/Unity wherever practical; #162 is the browser lane.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- Published Scenario One integration remains `integration/scenario-one-player-test-ready-v1` at **`8fa83bff22ea1b4943689bee1074eed8eae65d2f`**. It contains accepted S1-C T-06, S1-D T-04P/T-04, MFM-08A, save schema v24 and PARITY-08 lineages, but not the in-flight #151/#156 or downstream MFM-09+ work.
- PARITY-08 #126 is **INDEPENDENTLY APPROVED / INTEGRATED / CLOSED**. Reviewed production candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`; exact merge `7d2049e862df5c9311748167423b981e713d1fa9`; docs/integration successor `8fa83bff22ea1b4943689bee1074eed8eae65d2f`. The model-0 completion diagnostic mismatch was reproduced on the accepted base and remains historical D-047/D-048 verification debt, not a PARITY-08 regression; protected anchor `6F84AF319D301F87` remains unchanged.
- Rack V2 / D-064 is now **VERIFIED for feature scope**. Exact integrated Rack candidate `60fd9025e0205e7e8b24203295dd606ffa4a8061` is an ancestor of `8fa83bff...`. Final G4 review: `Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md` at `126ed71fffa259ea955dfbbe6e0e8f57942f9821`; verdict `OBJECTIVE 1 RACK PLANNING & FIELD MARKING V2 QUALIFIED — READY FOR USER TEST`. User acceptance and whole-Scenario-One Tier-3 remain separate. Five rendered polish notes remain non-blocking.
- S1-D T-05 #151 is **OFFLINE/STATIC APPROVED; UNITY QUALIFICATION RUNNING**. Exact start `cfdc93bf3943125d21570bc5749a9338f52d1cce`; production `98ac98b5a33929d30b150dc6e4692d8bdf2b9bc1`; verification `38904535ceb45da1b9112a7695c7c7295462efec`; handed-off head `4ddf4aa535d4ec5267be46d2635679411d595e0f`. Offline review approved 219/219 + 7/7 mutations. Latest durable claim remains `[RUNNING — UNITY]`; no completed Unity handoff, Manager runtime/rendered verdict or integration exists yet. Final Manager checklist: `Docs/Handoffs/S1-D-Implementation-05/FINAL-MANAGER-REVIEW-CHECKLIST-20261030.md`.
- MFM-08 #156 is **STATIC/OFFLINE PRE-REVIEW APPROVED / UNITY TIER-2 PENDING**. Production `d55ec86a3f0bf13820f2080197b6b0f43d832e06`; docs tip `d70cec1781c104794a6f225c8609847c55df1510`; static verdict `APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED`. Final Manager checklist: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/FINAL-MANAGER-REVIEW-CHECKLIST-20261030.md`.
- MFM-03 #39 is **APPROVED/CLOSED** as the static GAMEPLAY CALIBRATION input used by MFM-04+. Its independent-review N1–N6 caveats, especially live post-Lesson-5/cross-year drift, are explicitly carried into MFM-11.
- MFM-09 browser prep #180 is complete/closed but **NOT RELEASED**. Release requires Manager-approved/integrated #151 + #156 and a fresh exact combined integration SHA.
- MFM-10 browser prep #181 is complete/closed but **NOT RELEASED**. Release requires accepted/integrated MFM-09. Qualification-only by default; required token `SPECIES GUARD QUALIFIED`.
- PARITY-09 browser readiness #177 is complete but **NOT READY** because its authoritative start gate requires MFM-10 `SPECIES GUARD QUALIFIED`. Premature #179 was withdrawn without execution.
- MFM-11 browser prep #182 is complete/closed but **NOT RELEASED**. Release requires MFM-10 green; it must resolve the MFM-03 live/cross-year residual and return `MULTI-SCALE CALIBRATION QUALIFIED` or `BLOCKED — CALIBRATION IMPACT`, followed by independent review.
- MFM-12 browser prep #183 is complete/closed but **NOT RELEASED**. It requires the complete MFM requirements matrix, integrated Unity/rendered/save/compatibility/complete-loop evidence and independent architecture/spec review. Required success verdict: `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`.
- S1-C T-07 is a newly confirmed outstanding prerequisite rather than inferred-complete work. Browser readiness `Docs/Handoffs/S1-C-Implementation-07/BROWSER-READINESS-20261030.md`; released packet `Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md` at `8ad07f21daa768e0062c46b27da95f9164db67b4`; claim #187 is **READY — OFFLINE**, not RUNNING. Exact released base `8fa83bff...`; verification/docs only by default; no Unity required.
- S1-C T-08 final feature qualification is browser-prepared but **NOT RELEASED**. It waits for #187 T-07 Manager acceptance and a free serialized Unity lane. Readiness/packet/checklist are under `Docs/Handoffs/S1-C-Implementation-08/`.
- S1-D T-08 multi-cycle witness is browser-prepared but **NOT RELEASED**. Primary remaining semantic gate is #151 T-05 Unity + final Manager approval + integration. Packet/readiness are under `Docs/Handoffs/S1-D-Implementation-08/`.
- S1-D T-09 independent conformance review is browser-prepared but **NOT RELEASED** until T-08 PASS. Its packet has been corrected for D-066: the accepted contract is one bounded v24 historical teaching-completion latch, excluded from world/protected identity hashes, not the stale rule of “no S1-D schema field”. Review checklist is under `Docs/Handoffs/S1-D-Implementation-09/`.
- Scenario One Tier-3 / personal-player-test browser skeleton is prepared under #184 at `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md` and is **NOT RELEASED**. It requires one exact candidate with all required Rack/parity/S1-C/S1-D/MFM-12 gates accepted/integrated.
- No Windows build has occurred or is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **S1-D T-05 previous vs proposed** (#151) | local UI/Unity | `feature/s1-d-t05-previous-proposed-v1`; verification child `verify/s1d-t05-unity-v1` | bounded Work Plan + authorised read-only Annual Review route; verification-only Unity by default | **UNITY QUALIFICATION RUNNING — NO MANAGER RUNTIME VERDICT YET** |
| **S1-C T-07 data-contract stability** (#187) | local offline verification | suggested `verify/s1-c-t07-data-contract-v1`; `/media/jer/Videos/CCF-S1C-T07` | verification/docs only by default | **READY — OFFLINE; NOT YET RUNNING** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **STATIC PRE-REVIEW APPROVED; UNITY AFTER #151** |
| **S1-C T-08 final history qualification** | local Tier-2 after T-07 | prepared only; repin at release | qualification only; no production repair by default | **BROWSER PACKET PREPARED; WAITING #187 ACCEPTANCE + UNITY** |
| **S1-D T-08 multi-cycle witness** | local Tier-2 | prepared only; repin after #151 integration | qualification only; no production repair by default | **BROWSER PACKET PREPARED; WAITING #151 APPROVAL + INTEGRATION** |
| **S1-D T-09 independent conformance review** | browser/read-only independent review | no production lane | review only | **PREPARED; WAITING T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | browser prep then local | prepared only; repin at release | curriculum/integration after accepted #151 + #156 | **BROWSER PACKET PREPARED #180; WAITING ACCEPTED/INTEGRATED #151 + #156** |
| **MFM-10 species-parity guard** | local qualification after MFM-09 | prepared only; repin at release | verification/guard only unless defect separately assigned | **BROWSER PACKET PREPARED #181; WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | browser repin then local qualification | TBD | no production ownership until all start gates true | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | local qualification + independent review | prepared only; repin at release | measurement/qualification only; no retuning by default | **BROWSER PACKET PREPARED #182; WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | local integrated qualification + independent review | prepared only; repin at release | verification/review only unless defect separately assigned | **BROWSER PACKET PREPARED #183; WAITING MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | browser repin then local Tier-3 | prepared skeleton only | final integrated qualification; no Windows | **BROWSER SKELETON PREPARED #184; NOT RELEASED** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight.

## Single-writer boundaries

### #151

Offline/static implementation and independent review are accepted for the current candidate. Unity qualification owns no new production by default. Required runtime/render review includes import/compile, three-cycle presentation at 1280×720 and 1920×1080, Approved / Pending new / Pending carried-over grouping, future-year inconsistency treatment, MFM prescription-created work remaining ordinary Pending before Work Plan approval, exact creation-year Annual Review navigation, read-only history behavior, Executed/unplanned-Fell-mark visibility required by AC-5, and layout/collapsed-foldout readability. Any genuine production defect stops for bounded repair ownership; no save/schema/history-store/MFM-09 work.

### #187 S1-C T-07

Verification/docs only by default. It may run concurrently with #151 because it requires no Unity and must not touch #151-owned UI files. Prove stable event/work-order identities, append-only history meaning, read purity/counter stability, `harvestJobId` as resolution-year label rather than unique job identity, genuine old-save behavior and negative controls. No save/schema/validator/hash/reference/UI/MFM changes. If faithful verification unexpectedly requires Unity or runtime production edits, stop and return to Manager.

### #156

Owns Stand Map plus the smallest MFM-specific/inspection presentation helpers. No Annual Review/history, S1-D generic Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk-confirmation changes. Tier-2 must prove Unity compile/render, live footprint/layer/input behavior, candidate/proposal distinction, `[C]/[N]` interactions, real 3D spot-check, prescription/Work Plan boundary, conflicts, regressions and protected compatibility.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work. Mixed tasks must be split rather than consuming a local lane for browser-only phases.

## Dependency-ready sequence

1. **Parallel now:** finish #151 Unity qualification on exact `4ddf4aa535d4ec5267be46d2635679411d595e0f`; independently, #187 S1-C T-07 may run offline on its released exact base after fresh preflight.
2. When #151 returns, **browser/Manager immediately reviews it** using the prepared final checklist and safely integrates only if APPROVED.
3. After #151 releases the serialized Unity lane, **#156 MFM-08 Tier-2** may run; browser/Manager final review/integration follows if green.
4. After #187 returns, browser/Manager reviews T-07. Once T-07 is accepted and Unity is available, repin/release **S1-C T-08** final feature qualification.
5. After #151 is approved/integrated, repin/release **S1-D T-08** multi-cycle witness. On T-08 PASS, run independent **S1-D T-09** and then Manager feature-scope verification.
6. After accepted/integrated #151 + #156, repin/release **MFM-09**.
7. After accepted/integrated MFM-09, repin/release **MFM-10** and require `SPECIES GUARD QUALIFIED`.
8. Once MFM-10 is durably green, two downstream gates become eligible subject to ownership/Unity serialization: (a) fresh **PARITY-09 browser readiness**, then qualification/integration; (b) **MFM-11 calibration impact**, including the MFM-03 live/cross-year residual and independent review.
9. After MFM-11 and the applicable accepted Rack/parity/S1-C/S1-D lineage are present, release **MFM-12 integrated MFM qualification** and require `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` plus independent architecture/spec review.
10. When Rack V2 G4, PARITY-09, S1-C, S1-D and MFM-12 are all accepted/integrated on one exact candidate, repin/release **Scenario One Tier-3**. Required result: `SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`.
11. User personally playtests the same exact candidate.
12. Windows build only after explicit post-playtest authorisation.

## Durable current records

- Rack G4 final review: `Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md`
- PARITY-08 implementation/review/integration: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/`
- #151 handoff + final-review checklist: `Docs/Handoffs/S1-D-Implementation-05/`
- #156 handoff + browser/final-review checklists: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/`
- PARITY-09 browser readiness: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/BROWSER-READINESS-20261030.md`
- MFM-03 Manager closeout: `Docs/Handoffs/MFM-Implementation-03/MANAGER-REVIEW.md`
- MFM-09 readiness + prepared packet: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/`
- MFM-10 readiness + prepared packet: `Docs/Handoffs/MFM-10-Species-Parity-Guard/`
- MFM-11 readiness + prepared packet: `Docs/Handoffs/MFM-11-Calibration-Impact/`
- MFM-12 readiness + prepared packet: `Docs/Handoffs/MFM-12-Integrated-Verification/`
- S1-C T-07 readiness + released packet: `Docs/Handoffs/S1-C-Implementation-07/`
- S1-C T-08 readiness + prepared packet + review checklist: `Docs/Handoffs/S1-C-Implementation-08/`
- S1-D T-08 readiness + prepared packet: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09 corrected review packet + checklist: `Docs/Handoffs/S1-D-Implementation-09/`
- Tier-3 browser skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
