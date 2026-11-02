# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-02 by Overall Manager / OpenAI GPT-5.6 Sol after approving and integrating #224. PARITY-09 #219 and MFM-11 #220 remain Manager-approved/closed; #223 verification debt and #224 Objectives overflow repair are approved/integrated; #225 MFM-12 is now repinned and released for local start. Browser/GitHub Manager ownership remains under #200; local/Unity orchestration remains under #201/#203.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 is the sole primary writer for this file during the current two-lane split.**
- Workers refresh this file and their child claim before asserting ownership/lock availability.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Heavy Unity is serialized machine-wide; a historical `free` receipt proves nothing about current availability.
- Claims track execution presence only; they do not grant ownership.
- No Windows build before the user's personal Scenario One playtest and explicit post-playtest authorisation.
- After meaningful state changes, #200 refreshes `jermur788/Overall-Dev/updates/ccf/latest.md`; CCF remains canonical.

## Current integration truth

Published Scenario One integration:

`integration/scenario-one-player-test-ready-v1` = **`508b3e8124b5d9c477a4d2471cd56ebf6608f197`**.

This integration head includes the approved #223 verification-debt repair and the approved #224 bounded Objectives-window production repair plus returned handoffs and Manager reviews. #224's production delta is limited to `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss`.

Accepted current-lineage state includes Rack V2 / D-064 feature scope VERIFIED; **Sitka/Oak/Beech parity P0 VERIFIED** through PARITY-09 #219; S1-C T-08 accepted/feature-qualified; S1-D T-08 accepted/feature-qualified plus T-09 accepted; MFM-08 and MFM-09 accepted/integrated; MFM-10 #213 `SPECIES GUARD QUALIFIED`; **MFM-11 #220 `MULTI-SCALE CALIBRATION QUALIFIED`**; #199 Combined HUD / Focus Option 1 APPROVED/INTEGRATED/CLOSED; #223 Scenario One verification debt APPROVED/INTEGRATED; and #224 Objectives-window overflow APPROVED/INTEGRATED.

MFM-11's accepted evidence boundary is explicit: the actual post-Lesson-5 extension-state CI comparison is within the accepted gameplay-calibration tolerance; post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**.

#224 closed the final current-lineage guided-v2 product failure. Accepted exact-repair regressions are guided-v2 **535 PASS / 0 FAIL**, Combined HUD **465 PASS / 0 FAIL**, and PARITY-07 rendered **1031 PASS / 0 FAIL**. Do not infer MFM-12 qualification from those targeted repair gates; #225 must still execute its full integrated qualification.

## Live / released tasks

| Task | Lane | Exact base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, releases, coordination | **RUNNING — UMBRELLA** |
| **Browser Manager Continuation 04** (#221) | browser/GitHub | current integration + returned handoffs | #219/#220 reviews; debt disposition; downstream readiness | **CLOSED — COMPLETED** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | child lanes only | DeepSeek routing + evidence; Mimo authorised | **RUNNING** |
| **Verification-debt repair** (#223) | #203 local | `3acdf9cb635cdca79606335801e97f9d7dcce8b0` | verification-only pins/history/scanner/guided expectations | **CLOSED — APPROVED / INTEGRATED** |
| **Objectives-window overflow repair** (#224) | #203 local | `bbcefe06e0d6a0dce38f6eb01d622dca7677495c` | bounded UI reproduction/repair + rendered regressions | **CLOSED — APPROVED / INTEGRATED** |
| **MFM-12 integrated MFM qualification** (#225) | #203 local | `508b3e8124b5d9c477a4d2471cd56ebf6608f197` | integrated verification + independent review | **RUNNING — local lane `qual/mfm-12-integrated-v1`, worktree `/media/jer/ZX20/CCF-U1`, starting HEAD `508b3e81` (provider/model: orchestrated by `opencode-go/deepseek-v4.1-flash`; Unity execution delegated to `gpt-6.1-sol`); Unity lock checked FREE before start; production-repair authority NONE** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 prep/review then #203 local | not yet released | full integrated qualification | **NOT RELEASED — WAITS MFM-12** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | existing verification branch | non-blocking verification | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves the Unity lock is currently free.

## Manager-accepted PARITY-09 state

#219 is CLOSED / APPROVED on exact `3acdf9cb...`.

Durable review:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/MANAGER-REVIEW-20261101.md`

Token:

`THREE-SPECIES PARITY P0 VERIFIED`

The token does not waive raw verification debt. The required fresh timber/mixed/pruning/history repair coverage was completed and approved under #223. The three Beech reduced-simulator R2 misses remain an accepted C-level limitation with the unchanged 0.40 criterion.

## Manager-accepted MFM-11 state

#220 is CLOSED / APPROVED on exact `3acdf9cb...` production bytes.

Durable review:

`Docs/Handoffs/MFM-11-Calibration-Impact/MANAGER-REVIEW-20261101.md`

Token:

`MULTI-SCALE CALIBRATION QUALIFIED`

Accepted boundary:

- actual extension-state CI delta 4.169829% across 13 current Covered areas, within the accepted 20% GAMEPLAY CALIBRATION;
- post-treatment source-vs-current-Covered CI parity **NOT DEMONSTRATED** because treated areas leave the current Covered class and the later source sample can fail live qualification;
- exact-candidate PARITY-09 Reference/Legacy40 evidence reuse is accepted for the protected compatibility condition, alongside MFM-11 save/load self-consistency;
- no production retune or further mandatory MFM-11 measurement run is required.

## Verification-debt / UI repair disposition

Binding verification-debt record:

`Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/BROWSER-DISPOSITION.md`

### #223 — verification-only — APPROVED / INTEGRATED

Packet:

`Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/TASK-PACKET.md`

Returned handoff:

`Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/HANDOFF.md`

Manager review:

`Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/MANAGER-REVIEW-20261102.md`

Accepted result: mechanically stale v23→v24 verification pins/hashes repaired; history harness compile inputs repaired; `MaxRemovedBasalShare` scanner false positive removed without weakening the biological density guard; guided sequence/rack stale expectations reconciled under accepted authority. Required offline/Unity families pass except the three previously accepted pure-Beech R2 misses against the unchanged 0.40 criterion. No production paths changed.

### #224 — product/UI — APPROVED / INTEGRATED

Packet:

`Docs/Handoffs/ScenarioOne-Objectives-Overflow-Repair/TASK-PACKET.md`

Returned handoff:

`Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/HANDOFF.md`

Manager review:

`Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/MANAGER-REVIEW-20261102.md`

Accepted result: exact-base guided-v2 reproduced 533 PASS / 2 FAIL, both `current objective fits` at 1280×720 and 1920×1080. The bounded one-file USS repair preserved objective content/semantics and the fit assertion. Returned targeted regressions are guided-v2 535/0, Combined HUD 465/0 and PARITY-07 1031/0. Independent Claude Sonnet 5.5 review APPROVED with no blocker. QuickSearch caveat is accepted as Editor-environment evidence, not product failure.

Token:

`SCENARIO ONE OBJECTIVES OVERFLOW REPAIR APPROVED — MFM-12 RELEASED NEXT`

## MFM-12 release state

MFM-12 claim #225 is now **READY / REPINNED / LOCAL START RELEASED**.

Binding packet:

`Docs/Handoffs/MFM-12-Integrated-Qualification/TASK-PACKET.md`

Binding local execution handoff:

`Docs/Handoffs/MFM-12-Integrated-Qualification/LOCAL-EXECUTION-HANDOFF-20261102.md`

Exact candidate:

`508b3e8124b5d9c477a4d2471cd56ebf6608f197`

MFM-12 is qualification-only and has no production-repair authority. It must cover the full accepted requirements matrix, exact-candidate persistence/compatibility, rendered 720p/1080p UX, player-judgement/Work Plan boundaries, provenance/history, repeated-cycle coexistence, complete-loop Scenario One witness and independent cross-provider review.

The accepted MFM-11 post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED** unless separately and explicitly established. Do not upgrade the claim by inference.

Final product token remains Browser Overall authority after the returned handoff is reviewed:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

## Tier-3 readiness

Updated browser readiness:

`Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-READINESS-20261101.md`

Tier-3 remains **NOT RELEASED**. Required sequence is now:

1. execute #225 MFM-12 exact-candidate integrated qualification and independent review;
2. Browser Overall reviews the returned MFM-12 evidence and issues APPROVED / NEEDS CHANGES / REJECTED;
3. only accepted MFM-12 releases Scenario One Tier-3 on one exact production lineage;
4. Tier-3 success -> user personally playtests that exact candidate;
5. Windows build only after explicit post-playtest user authorisation.

## Hard boundaries

- Research Source Consolidation Audit **#194 remains NO ACTION**. Do not inspect/review/approve/reject/merge/update/reorganise it unless the user explicitly lifts the restriction.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- Never weaken tests or silently rebaseline protected anchors.
- Qualification claims have no production-repair authority.
- Local workers do not edit this file.

## Durable current records

- Browser continuation #221: `Docs/Handoffs/ScenarioOne-Browser-Continuation-04/BROWSER-HANDOFF-20261101.md`.
- PARITY-09 Manager review: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/MANAGER-REVIEW-20261101.md`.
- MFM-11 Manager review: `Docs/Handoffs/MFM-11-Calibration-Impact/MANAGER-REVIEW-20261101.md`.
- Verification-debt Manager review: `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/MANAGER-REVIEW-20261102.md`.
- Objectives overflow returned handoff: `Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/HANDOFF.md`.
- Objectives overflow Manager review: `Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/MANAGER-REVIEW-20261102.md`.
- MFM-12 packet/local handoff: `Docs/Handoffs/MFM-12-Integrated-Qualification/`.
- Tier-3 readiness: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-READINESS-20261101.md`.
- Unified local orchestration: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md`.
- MFM-10 Manager review: `Docs/Handoffs/MFM-10-Species-Parity-Guard/MANAGER-REVIEW-20261101.md`.
- #199 Manager review: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-REVIEW-20261101.md`.
