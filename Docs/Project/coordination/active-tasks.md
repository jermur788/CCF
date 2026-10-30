# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-30 by Overall Manager / OpenAI GPT-5.6 Sol after S1-C T-07 #187 Manager approval/closure, S1-C T-08 dependency advance, and OPS Unity-lock blocker #189 creation.

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

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` is now **`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`**. Exact S1-D T-05 production merge is `46fc6d55315bbddcdfa7667f0da490378d584023`; `ce673714...` is the integration-handoff/docs successor and contains no further production change.
- S1-D T-05 #151 is **APPROVED / INTEGRATED / CLOSED**. Exact qualified feature/handoff head `4ddf4aa535d4ec5267be46d2635679411d595e0f`; pushed Unity qualification `643bb39051f58c8e929e86da14f47baf253ec5ad`; durable rendered evidence `c3a448a374eee6b3e2f920f8437fef5b7a288e22`; `render-run4` = `S1D_T05_UNITY_PASS`, 579/579, Unity exit 0, 12 captures. Final Manager review: `Docs/Handoffs/S1-D-Implementation-05/FINAL-MANAGER-REVIEW-20261030.md`; integration handoff: `Docs/Handoffs/S1-D-Implementation-05/INTEGRATION-HANDOFF.md`.
- PARITY-08 #126 is **INDEPENDENTLY APPROVED / INTEGRATED / CLOSED**. Reviewed production candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`; exact merge `7d2049e862df5c9311748167423b981e713d1fa9`; docs/integration successor `8fa83bff22ea1b4943689bee1074eed8eae65d2f`, now an ancestor of `ce673714...`. The model-0 completion diagnostic mismatch remains historical D-047/D-048 verification debt, not a PARITY-08 regression; protected anchor `6F84AF319D301F87` remains unchanged.
- Rack V2 / D-064 is **VERIFIED for feature scope**. Exact integrated Rack candidate `60fd9025e0205e7e8b24203295dd606ffa4a8061` is an ancestor of the current integration. Final G4 review: `Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md` at `126ed71fffa259ea955dfbbe6e0e8f57942f9821`; verdict `OBJECTIVE 1 RACK PLANNING & FIELD MARKING V2 QUALIFIED — READY FOR USER TEST`. User acceptance and whole-Scenario-One Tier-3 remain separate. Five rendered polish notes remain non-blocking.
- MFM-08 #156 is **STATIC/OFFLINE PRE-REVIEW APPROVED; UNITY TIER-2 RELEASED BUT BLOCKED ON LOCK GOVERNANCE #189**. Production `d55ec86a3f0bf13820f2080197b6b0f43d832e06`; docs tip `d70cec1781c104794a6f225c8609847c55df1510`; static verdict `APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED`. Ultra Code preflight stopped before posting `[RUNNING — UNITY]` because the authoritative `unity-lock` tool rejected stale storage/mount configuration. This is an ops blocker, not an MFM-08 product failure.
- OPS #189 is **OPEN / RELEASED** to repair the stale shared Unity-lock configuration after the storage migration. It must restore the existing authoritative lock mechanism without bypass/private-lock substitution, then return an authoritative holder/free state. No Unity launch or gameplay edit is allowed under #189.
- MFM-03 #39 is **APPROVED/CLOSED** as the static GAMEPLAY CALIBRATION input used by MFM-04+. Its independent-review N1–N6 caveats, especially live post-Lesson-5/cross-year drift, are explicitly carried into MFM-11.
- MFM-09 browser prep #180 is complete/closed but **NOT RELEASED**. #151 is now satisfied; remaining release gate is Manager-approved/integrated #156 plus a fresh exact combined integration SHA.
- MFM-10 browser prep #181 is complete/closed but **NOT RELEASED**. Release requires accepted/integrated MFM-09. Qualification-only by default; required token `SPECIES GUARD QUALIFIED`.
- PARITY-09 browser readiness #177 is complete but **NOT READY** because its authoritative start gate requires MFM-10 `SPECIES GUARD QUALIFIED`. Premature #179 was withdrawn without execution.
- MFM-11 browser prep #182 is complete/closed but **NOT RELEASED**. Release requires MFM-10 green; it must resolve the MFM-03 live/cross-year residual and return `MULTI-SCALE CALIBRATION QUALIFIED` or `BLOCKED — CALIBRATION IMPACT`, followed by independent review.
- MFM-12 browser prep #183 is complete/closed but **NOT RELEASED**. It requires the complete MFM requirements matrix, integrated Unity/rendered/save/compatibility/complete-loop evidence and independent architecture/spec review. Required success verdict: `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`.
- S1-C T-07 #187 is **MANAGER APPROVED / CLOSED**. Accepted verification head `e64b337efd3b758fe6bc21636822ad178f0df0ca`; handoff `8228a26651fe01107f68e0aa6f2022d6d26fd3e9`; branch publication tip `d104a3b0b99e36ce66f510d6cdaebf6ba0332f2f`; 304/304 checks and 8/8 mutations caught. Manager review: `Docs/Handoffs/S1-C-Implementation-07/MANAGER-REVIEW-20261030.md` at main `950a620da0933f1ca92b294e763410ec1d6166b2`. This is offline contract acceptance only; no production change was introduced.
- S1-C T-08 final feature qualification is **DEPENDENCY-CLEARED ON T-07 BUT NOT RELEASED**. Its packet is updated on main at `42a535f9111ee735c2c683a8c4bdb53a20ef4ff5`. It waits for #189 lock-governance restoration and #156 completion/lane release, then should receive the next history qualification slot before S1-D T-08 unless Overall Manager explicitly reprioritises.
- S1-D T-08 multi-cycle witness is **RELEASED FOR FRESH PREFLIGHT BUT SERIALIZED BEHIND #156 AND THE PREFERRED POST-#156 S1-C T-08 SLOT**. Exact released integration candidate `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`; repinned packet publication `2bb88a5bac8f2e1d38d40e5a73f3856d08b0a399`; release handoff `Docs/Handoffs/S1-D-Implementation-08/RELEASE-HANDOFF-20261030.md` at `db2eea6c914fb73faaaf08573b17ba044e00e1d8`. No substantive execution claim exists yet; create/mark RUNNING only at actual start after live preflight.
- S1-D T-09 independent conformance review is browser-prepared but **NOT RELEASED** until T-08 PASS. Its packet reflects D-066: the accepted contract is one bounded v24 historical teaching-completion latch, excluded from world/protected identity hashes.
- Scenario One Tier-3 / personal-player-test browser skeleton is prepared under #184 at `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md` and is **NOT RELEASED**. It requires one exact candidate with all required Rack/parity/S1-C/S1-D/MFM-12 gates accepted/integrated.
- No Windows build has occurred or is authorised.

## Live / prepared tasks

| Task | Lane | Branch / worktree | Ownership | Status |
|---|---|---|---|---|
| **PLAYER-TEST-BROWSER-LANE-02** (#162) | browser/GitHub | no local worktree | review/coordination/packets only | **ACTIVE** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | child claims only | **RUNNING umbrella** |
| **OPS-UNITY-LOCK-01** (#189) | local ops/config | existing shared ops/config paths only | authoritative Unity lock config/state repair | **RELEASED — BLOCKS ALL UNITY STARTS UNTIL GREEN** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local UI/Unity | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | Stand Map / bounded inspection UI | **UNITY TIER-2 RELEASED; BLOCKED ON #189 BEFORE RUNNING** |
| **S1-C T-08 final history qualification** | local Tier-2 after #156 | prepared only; repin at release | qualification only; no production repair by default | **T-07 CLEARED; WAITING #189 + #156 LANE RELEASE** |
| **S1-D T-08 multi-cycle witness** | local Tier-2 after preferred S1-C T-08 slot | `verify/s1-d-multicycle-witness-v1`; `/media/jer/Videos/CCF-S1D-T08` | qualification only; no production repair by default | **RELEASED; WAITING SERIALIZED UNITY** |
| **S1-D T-09 independent conformance review** | browser/read-only independent review | no production lane | review only | **PREPARED; WAITING T-08 PASS** |
| **MFM-09 teaching / Work Plan integration** | browser prep then local | prepared only; repin at release | curriculum/integration after accepted #156; #151 satisfied | **BROWSER PACKET PREPARED #180; WAITING ACCEPTED/INTEGRATED #156** |
| **MFM-10 species-parity guard** | local qualification after MFM-09 | prepared only; repin at release | verification/guard only unless defect separately assigned | **BROWSER PACKET PREPARED #181; WAITING MFM-09** |
| **PARITY-09 integrated parity qualification** | browser repin then local qualification | TBD | no production ownership until all start gates true | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | local qualification + independent review | prepared only; repin at release | measurement/qualification only; no retuning by default | **BROWSER PACKET PREPARED #182; WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | local integrated qualification + independent review | prepared only; repin at release | verification/review only unless defect separately assigned | **BROWSER PACKET PREPARED #183; WAITING MFM-11 + APPLICABLE INTEGRATED LINEAGE** |
| **Scenario One Tier-3 / personal-player-test candidate** | browser repin then local Tier-3 | prepared skeleton only | final integrated qualification; no Windows | **BROWSER SKELETON PREPARED #184; NOT RELEASED** |
| **PARITY-03 Q10** (#74) | verification | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only | **WAITING — NON-BLOCKING** |

No row proves that the Unity lock is free. Every Unity worker must fresh-preflight through the restored authoritative mechanism after #189 closes green.

## Single-writer boundaries

### #189 OPS Unity-lock repair

Owns only the smallest shared ops/config correction required to restore the existing authoritative `unity-lock` mechanism after storage migration. Re-confirm mount UUID, authoritative storage README/repo location and the lock tool's intended state root before editing. No alternate/private lock, no Unity launch, no gameplay/project production edits. If the normal tool still cannot establish authoritative holder/free state, stop BLOCKED.

### #156 MFM-08

Owns Stand Map plus the smallest MFM-specific/inspection presentation helpers. No Annual Review/history, S1-D generic Work Plan history, save/schema, MFM-09 curriculum, coverage-world/calibration, proposal→Fell-mark or bulk-confirmation changes. Tier-2 must prove Unity compile/render, live footprint/layer/input behavior, candidate/proposal distinction, `[C]/[N]` interactions, real 3D spot-check, prescription/Work Plan boundary, conflicts, regressions and protected compatibility. Genuine production defect stops for separate ownership; no test weakening. Do not post `[RUNNING — UNITY]` until #189 is green and the full fresh preflight succeeds.

### S1-C T-08

Qualification-only. T-07 identity/data-contract acceptance is satisfied and fixes the genuine old-save source to v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`. Do not start before #189 is green, #156 releases the first restored Unity slot, current integration is freshly repinned and live coordination grants this slot. No production repair authority.

### S1-D T-08

Released qualification-only task. No production fix authority. It must not take the Unity lock while #156, S1-C T-08 or another live owner holds it. Exact released candidate is `ce673714...` unless a later Manager-approved integration descendant is explicitly inspected before start. Run the >=3-cycle/idle-year/carried-order/save-load/history/completion/post-completion/D-057 witness and protected regressions at both required render sizes.

### Browser lane #162

Owns no production files. Use it for pushed-diff review, independent/static review, claim/coordination updates, packet preparation, dependency repinning and safe browser-side integration work. Mixed tasks must be split rather than consuming a local lane for browser-only phases.

## Dependency-ready sequence

1. **Now:** run #189 OPS lock-governance repair. No Unity task may infer the lane free while #189 is unresolved.
2. After #189 returns green, #156 repeats its full fresh preflight, obtains the authoritative Unity lock normally, posts `[RUNNING — UNITY]`, and completes MFM-08 Tier-2.
3. When #156 returns, browser/Manager performs final rendered/runtime review and safe integration only if APPROVED.
4. Because #187 T-07 is now Manager-approved, the preferred next serialized Unity slot after #156 is **S1-C T-08**. At that point repin/release it against the exact current integration lineage, create/mark its substantive claim RUNNING and execute the full history qualification.
5. After S1-C T-08 returns, browser/Manager performs S1-C feature-scope conformance review. If APPROVED, its final history evidence becomes reusable input for S1-D T-08.
6. Then run S1-D T-08 unless live coordination reprioritises for a material reason. On PASS, run independent S1-D T-09 and then Manager feature-scope verification.
7. After accepted/integrated #156, repin/release MFM-09 against the exact combined current integration lineage (#151 is already satisfied).
8. After accepted/integrated MFM-09, repin/release MFM-10 and require `SPECIES GUARD QUALIFIED`.
9. Once MFM-10 is durably green, two downstream gates become eligible subject to ownership/Unity serialization: (a) fresh PARITY-09 browser readiness, then qualification/integration; (b) MFM-11 calibration impact, including the MFM-03 live/cross-year residual and independent review.
10. After MFM-11 and the applicable accepted Rack/parity/S1-C/S1-D lineage are present, release MFM-12 integrated MFM qualification and require `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` plus independent architecture/spec review.
11. When Rack V2 G4, PARITY-09, S1-C, S1-D and MFM-12 are all accepted/integrated on one exact candidate, repin/release Scenario One Tier-3. Required result: `SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`.
12. User personally playtests the same exact candidate.
13. Windows build only after explicit post-playtest authorisation.

## Durable current records

- Rack G4 final review: `Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md`
- PARITY-08 implementation/review/integration: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/`
- #151 final review + integration: `Docs/Handoffs/S1-D-Implementation-05/`
- #156 handoff + browser/final-review checklists: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/`
- #189 lock-governance packet: `Docs/Handoffs/OPS-UNITY-LOCK-01/TASK-PACKET.md`
- PARITY-09 browser readiness: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/BROWSER-READINESS-20261030.md`
- MFM-03 Manager closeout: `Docs/Handoffs/MFM-Implementation-03/MANAGER-REVIEW.md`
- MFM-09 readiness + prepared packet: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/`
- MFM-10 readiness + prepared packet: `Docs/Handoffs/MFM-10-Species-Parity-Guard/`
- MFM-11 readiness + prepared packet: `Docs/Handoffs/MFM-11-Calibration-Impact/`
- MFM-12 readiness + prepared packet: `Docs/Handoffs/MFM-12-Integrated-Verification/`
- S1-C T-07 Manager review: `Docs/Handoffs/S1-C-Implementation-07/MANAGER-REVIEW-20261030.md`
- S1-C T-08 dependency-cleared packet: `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`
- S1-D T-08 readiness + released packet/handoff: `Docs/Handoffs/S1-D-Implementation-08/`
- S1-D T-09 corrected review packet + checklist: `Docs/Handoffs/S1-D-Implementation-09/`
- Tier-3 browser skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
