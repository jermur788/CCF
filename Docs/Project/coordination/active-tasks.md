# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-02 by Overall Manager / OpenAI GPT-5.6 Sol after accepting the MFM-12 STOP. #225 is blocked on confirmed fit-replay defect S1. #230 is the released bounded repair. Browser/GitHub Manager ownership remains under #200; local/Unity orchestration remains under #201/#203.

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

`integration/scenario-one-player-test-ready-v1` = **`b4f79c6f4cf8e30a5fc74dd4ce7f8f5f1aa7f789`**.

The exact production bytes remain those of the previously accepted MFM-12 candidate `508b3e8124b5d9c477a4d2471cd56ebf6608f197`; the newer branch head adds only Browser Overall management documentation for the MFM-12 STOP and #230 repair release. No fit-replay production repair has been integrated yet.

Accepted current-lineage state still includes Rack V2 / D-064 feature scope VERIFIED; Sitka/Oak/Beech parity P0 VERIFIED through #219; accepted S1-C/S1-D pre-demo slices; MFM-08/09 accepted/integrated; MFM-10 `SPECIES GUARD QUALIFIED`; MFM-11 `MULTI-SCALE CALIBRATION QUALIFIED`; #199 Combined HUD/Focus approved; #223 verification debt approved/integrated; and #224 Objectives overflow approved/integrated.

MFM-11 evidence boundary remains explicit: extension-state calibration is within accepted GAMEPLAY CALIBRATION tolerance; post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**.

## MFM-12 STOP / defect state

MFM-12 #225 was started on the exact accepted candidate and correctly STOPPED before the full Unity matrix after confirming production/spec defect **S1**:

- `ScenarioOnePrescription.CheckFit` contains the deterministic source-sample replay core;
- the live manager/review flow does not call or expose that fit result;
- Treatment Review does not show the §7.3 agreement/disagreement, differing sample decisions, or Share/removal consequences;
- wider prescription approval can proceed without that required player-visible fit replay.

This fails spec §7.3 and checklist E4/E5/J7. Independent Claude Sonnet 5.5 review returned `NEEDS CHANGES` and confirmed the STOP.

Manager review:

`Docs/Handoffs/MFM-12-Integrated-Qualification/MANAGER-REVIEW-STOP-20261102.md`

Returned handoff/review remain on `qual/mfm-12-integrated-v1`:

- `Docs/Handoffs/MFM-12-Integrated-Qualification/HANDOFF.md`
- `Docs/Handoffs/MFM-12-Integrated-Qualification/INDEPENDENT-REVIEW.md`

The partial qualification package is not a PASS. Eleven offline gates and Unity compile are scoped evidence only. Unity steps 02–17, rendered qualification, exact-candidate save/runtime/provenance/repeated-cycle/full-loop evidence did not run. Mutation controls M14–M25 did not execute.

## Live / released tasks

| Task | Lane | Exact base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, releases, coordination | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | child lanes only | DeepSeek routing + evidence | **RUNNING** |
| **Verification-debt repair** (#223) | #203 local | accepted prior candidate | verification-only repair | **CLOSED — APPROVED / INTEGRATED** |
| **Objectives-window overflow repair** (#224) | #203 local | accepted post-#223 candidate | bounded UI repair | **CLOSED — APPROVED / INTEGRATED** |
| **MFM-12 integrated qualification** (#225) | #203 local | production candidate `508b3e8124b5d9c477a4d2471cd56ebf6608f197` | full integrated qualification | **NEEDS CHANGES — BLOCKED ON #230; DO NOT RESUME** |
| **MFM fit-replay review repair** (#230) | #203 local | current exact integration head at RUNNING preflight | bounded manager/UI/approval repair + focused regressions | **READY — NEXT LOCAL TASK** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 then #203 | not released | full integrated qualification | **NOT RELEASED — WAITS ACCEPTED MFM-12** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | existing verification branch | non-blocking verification | **WAITING — NON-BLOCKING** |

No row proves the Unity lock is currently free. The returned #225 handoff reported the lock free, but every new worker must perform a fresh lock preflight.

## #230 binding repair authority

Packet:

`Docs/Handoffs/MFM-12-Fit-Replay-Repair/TASK-PACKET.md`

Local execution handoff:

`Docs/Handoffs/MFM-12-Fit-Replay-Repair/LOCAL-EXECUTION-HANDOFF-20261102.md`

Classification: **PACKET ONLY — bounded production repair**.

The repair must reuse the existing `CheckFit` core and add the minimum player-visible Treatment Review/approval integration required by spec §7.3:

- agreement/disagreement summary;
- inspectable player-only and rule-only sample decisions;
- relevant Share/removal consequences;
- honest invalid/stale state;
- wider prescription approval blocked when the required current fit replay is unavailable/invalid.

The fit remains an honesty comparison, not a forestry correctness score.

#230 may also mechanically repair the three stale verification-tool items exposed by the stopped MFM-12 where accepted authority is unambiguous: MFM Coverage World save-v23 assertion, MFM mutation M14 v23 target, and the stale MFM-08 treatment-review boundary pin. These are tooling debt, not product defects; do not weaken assertions or guess product intent.

Fresh independent cross-provider review is mandatory for #230.

## Hard exclusions for #230

- no prescription algorithm redesign;
- no coverage/calibration retune;
- no Rack V2 or world-identity change;
- no Sitka/broadleaf scope change;
- no candidate auto-confirm/bulk automation;
- no proposal-to-Fell semantic change;
- no Work Plan commitment-gate change;
- no save-schema change;
- no ecology/economy/timber/deadwood/regeneration change;
- no protected Reference Future/Legacy40 rebaseline;
- no Windows build;
- #194 remains NO ACTION.

## MFM-12 restart rule

After #230 returns:

1. #200 reviews the production diff, focused Unity/rendered evidence and independent review;
2. if APPROVED, integrate #230 onto the Scenario One integration branch;
3. freshly repin #225 to the resulting exact integration candidate;
4. rerun the **full** MFM-12 matrix on that repaired candidate — do not continue from old Unity step 02;
5. only a new full qualification plus independent review may reach `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`.

Tier-3 remains unreleased until that acceptance.

## Durable current records

- MFM-12 STOP Manager review: `Docs/Handoffs/MFM-12-Integrated-Qualification/MANAGER-REVIEW-STOP-20261102.md`.
- #230 repair packet: `Docs/Handoffs/MFM-12-Fit-Replay-Repair/TASK-PACKET.md`.
- #230 local handoff: `Docs/Handoffs/MFM-12-Fit-Replay-Repair/LOCAL-EXECUTION-HANDOFF-20261102.md`.
- MFM-11 Manager review: `Docs/Handoffs/MFM-11-Calibration-Impact/MANAGER-REVIEW-20261101.md`.
- #223 Manager review: `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/MANAGER-REVIEW-20261102.md`.
- #224 Manager review: `Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/MANAGER-REVIEW-20261102.md`.
- MFM-12 original packet/local handoff: `Docs/Handoffs/MFM-12-Integrated-Qualification/`.
- Tier-3 readiness: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-READINESS-20261101.md`.

Frozen Reference Future v1 and Legacy40 compatibility remain protected. Never weaken tests or silently rebaseline protected anchors.