# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-02 by Overall Manager / OpenAI GPT-5.6 Sol after approving and integrating #230. The fit-replay defect is repaired; #225 is freshly released for a full rerun. Browser/GitHub Manager ownership remains under #200; local/Unity orchestration remains under #201/#203.

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

`integration/scenario-one-player-test-ready-v1` = **`e42bb9f437e4fcf20c5b753dacd1136470daf33e`**.

This lineage contains the approved #230 fit-replay repair, its focused verification/tooling repairs, the durable return, Browser Overall Manager approval, the MFM-12 rerun addendum and the fresh local rerun handoff.

The accepted #230 production delta is limited to four files:

- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs`;
- `Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescriptionApproval.cs`;
- `Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs`;
- `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs`.

The authoritative `ScenarioOnePrescription.CheckFit` core itself is unchanged. No save-schema, calibration, Rack V2, species-scope, ecology, economy, Work Plan or protected-anchor semantics changed.

Accepted current-lineage state also includes Rack V2 / D-064 VERIFIED; Sitka/Oak/Beech parity P0 VERIFIED; required S1-C/S1-D pre-demo slices; MFM-08/09 integrated; MFM-10 `SPECIES GUARD QUALIFIED`; MFM-11 `MULTI-SCALE CALIBRATION QUALIFIED`; #199 Combined HUD/Focus approved; #223 verification debt approved/integrated; and #224 Objectives overflow approved/integrated.

MFM-11 boundary remains explicit: extension-state calibration is within accepted GAMEPLAY CALIBRATION tolerance; post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**.

## #230 — fit-replay repair — APPROVED / INTEGRATED / CLOSED

Parent qualification #225 previously stopped correctly because spec §7.3 / checklist E4/E5/J7 were not connected to the player-visible flow.

#230 now satisfies the bounded defect repair:

- live Treatment Review obtains the existing authoritative fit replay;
- player sees agreement/disagreement summary;
- player-only/rule-only sample differences are individually inspectable;
- relevant Crop Tree / Share / basal-area and player-vs-rule consequences are shown;
- invalid/unavailable fit state is explicit;
- wider prescription approval is blocked when required fit is unavailable/invalid;
- fit remains an honesty comparison, not a forestry-correctness score;
- prescription approval still creates Pending work only and Work Plan remains the commitment gate.

Accepted focused evidence includes Unity compile, MFM Treatment Review + fit 412 checks / 29 shots, MFM-07 approval 106, MFM-08A coverage 56, guided-v2 535/0, Combined HUD 465/0, PARITY-07 1031/0, treatment-review offline 312/0, prescription core 240/0, approval 125/0, coverage 102/0, and save mutation controls 25/25 killed. Independent Claude Sonnet 5.5 review: APPROVED, no blocker.

Manager review:

`Docs/Handoffs/MFM-12-Fit-Replay-Repair/MANAGER-REVIEW-20261102.md`

Returned handoff:

`Docs/Handoffs/MFM-12-Fit-Replay-Repair/HANDOFF.md`

Manager token:

`MFM FIT REPLAY REPAIR APPROVED — FULL MFM-12 RERUN RELEASED NEXT`

## Live / released tasks

| Task | Lane | Exact base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, releases, coordination | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | child lanes only | DeepSeek routing + evidence | **RUNNING** |
| **MFM fit-replay repair** (#230) | #203 local | prior integration candidate | bounded fit manager/UI/approval repair | **CLOSED — APPROVED / INTEGRATED** |
| **MFM-12 full integrated qualification rerun** (#225) | #203 local | exact integration head recorded on claim | full A–P qualification + independent review | **READY / REPINNED — NEXT LOCAL GATE** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 then #203 | not released | full integrated qualification | **NOT RELEASED — WAITS ACCEPTED MFM-12** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | existing verification branch | non-blocking verification | **WAITING — NON-BLOCKING** |

No row proves the Unity lock is currently free. The #230 return reported it free; #225 must perform a fresh lock/process preflight immediately before Unity.

## #225 — full MFM-12 rerun authority

Original requirements packet:

`Docs/Handoffs/MFM-12-Integrated-Qualification/TASK-PACKET.md`

Rerun addendum:

`Docs/Handoffs/MFM-12-Integrated-Qualification/RERUN-ADDENDUM-20261102.md`

Fresh local handoff:

`Docs/Handoffs/MFM-12-Integrated-Qualification/LOCAL-EXECUTION-HANDOFF-RERUN-20261102.md`

The exact starting integration head is recorded on issue #225 after all Manager rerun documents are published. Workers must verify that exact head before starting.

This is a **fresh full qualification**. Do not continue the stopped run from Unity step 02. Start the MFM-12 matrix from the beginning on one clean exact candidate.

Fresh-required evidence includes Rack-derived live sample/coverage, fit replay in the live player path, player-judgement/candidate boundaries, proposal/Work Plan boundary, ordinary economy/provenance, save/load and malformed/old-save behavior, Legacy40/Reference Future protection, rendered 720p/1080p UX, current verification gates, repeated-cycle provenance/history, a complete Scenario One loop, and fresh independent cross-provider review.

#225 has no production-repair authority. Any new product/spec defect stops and returns to Browser Overall.

Successful local return:

`MFM-12 QUALIFICATION PASS / INDEPENDENT REVIEW APPROVED / READY FOR MANAGER ACCEPTANCE`

Only Browser Overall may issue:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

## Tier-3 readiness

Tier-3 remains **NOT RELEASED**. Required sequence:

1. execute the fresh #225 MFM-12 rerun on the exact repinned integration candidate;
2. Browser Overall reviews the full return and issues APPROVED / NEEDS CHANGES / REJECTED;
3. only accepted MFM-12 releases Scenario One Tier-3;
4. Tier-3 success hands the exact candidate to the user for personal playtest;
5. Windows build only after explicit post-playtest user authorisation.

## Hard boundaries

- Research Source Consolidation Audit #194 remains **NO ACTION**.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- Never weaken tests or silently rebaseline protected anchors.
- Qualification claims have no production-repair authority.
- Local workers do not edit this file.

## Durable current records

- MFM-12 STOP review: `Docs/Handoffs/MFM-12-Integrated-Qualification/MANAGER-REVIEW-STOP-20261102.md`.
- #230 packet/local handoff/return/review: `Docs/Handoffs/MFM-12-Fit-Replay-Repair/`.
- MFM-12 original packet: `Docs/Handoffs/MFM-12-Integrated-Qualification/TASK-PACKET.md`.
- MFM-12 rerun addendum: `Docs/Handoffs/MFM-12-Integrated-Qualification/RERUN-ADDENDUM-20261102.md`.
- MFM-12 fresh rerun local handoff: `Docs/Handoffs/MFM-12-Integrated-Qualification/LOCAL-EXECUTION-HANDOFF-RERUN-20261102.md`.
- MFM-11 Manager review: `Docs/Handoffs/MFM-11-Calibration-Impact/MANAGER-REVIEW-20261101.md`.
- #223 Manager review: `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/MANAGER-REVIEW-20261102.md`.
- #224 Manager review: `Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/MANAGER-REVIEW-20261102.md`.
- Tier-3 readiness: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-READINESS-20261101.md`.
