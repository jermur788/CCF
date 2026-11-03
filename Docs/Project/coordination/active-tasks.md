# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-03 by Overall Manager / OpenAI GPT-6 — user-approved Astra onboarding visual-refresh handoff #243 dispatched as READY, with no implementation lock yet. Prior #232 integrated status remains unchanged. Browser/GitHub Manager ownership remains under #200; local/Unity orchestration remains under #201/#203.

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

The published Scenario One integration on `integration/scenario-one-player-test-ready-v1` is now the **final player-test candidate**. It includes the exact Manager-approved #232 Stand Map species-canopy differentiation by a lease-checked non-forced fast-forward from the exact Tier-3-approved base to the already-qualified direct-child production commit.

Accepted current-lineage state includes Rack V2 / D-064 VERIFIED; Sitka/Oak/Beech parity P0 VERIFIED; S1-C review/history accepted; S1-D repeated-intervention qualification accepted; MFM-08/09 integrated; MFM-10 `SPECIES GUARD QUALIFIED`; MFM-11 `MULTI-SCALE CALIBRATION QUALIFIED`; MFM-12 `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`; #199 Combined HUD/Focus approved; #223 verification debt approved/integrated; #224 Objectives overflow approved/integrated; #230 fit-replay repair approved/integrated; final Tier-3 #231/#240 approved; and #232 Stand Map species-canopy differentiation **APPROVED / INTEGRATED**.

MFM-11 boundary remains explicit: extension-state calibration is within accepted GAMEPLAY CALIBRATION tolerance; post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**. The three pure-Beech reduced-simulator R2 misses remain accepted C-level limitations against the unchanged 0.40 criterion.

## Final Tier-3 and #232 delta

#231 Scenario One final Tier-3 qualification and #240 evidence closure are **APPROVED / CLOSED**. #232 then changed only procedural Stand Map canopy presentation plus durable Stand Map verification assertions.

#232 affected verification accepted by Manager:

- offline Stand Map checks 176/0;
- mixed-species before/after full-map rendering at 1280×720 and 1920×1080;
- unchanged 1,416-tree map-model fingerprint and simulation WorldHash;
- Combined HUD/minimap 465/0;
- Rack map-planning PASS;
- Rack confirm/minimap PASS;
- PARITY-07 rendered PASS;
- compile PASS;
- Scenario One completion PASS with the accepted completion fingerprint;
- no material redraw/performance regression;
- fresh cross-provider OpenAI read-only review: **APPROVED**.

Manager review:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/MANAGER-REVIEW-20261103.md`

Integration handoff:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/INTEGRATION-HANDOFF.md`

Because the integration is the exact already-qualified direct-child commit on its original parent, no Unity or rendered/Tier-3 rerun is required solely for integration.

## Live / released tasks

| Task | Lane | Ownership | Status |
|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | Manager review, releases, coordination | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | local routing + housekeeping | **RUNNING** |
| **Scenario One final Tier-3 qualification** (#231) | #203 local | parent milestone qualification | **APPROVED / CLOSED** |
| **Tier-3 Task 2 evidence closure** (#240) | #203 local | three evidence actions + delta review | **APPROVED / CLOSED** |
| **Stand Map species canopy differentiation** (#232) | #203 local / Unity | bounded player-facing map presentation | **APPROVED / INTEGRATED** |
| **#232 final integration** (#242) | browser/GitHub | exact fast-forward publication | **INTEGRATED / CLOSE** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | non-blocking verification | **WAITING — NON-BLOCKING** |
| **Astra onboarding visual refresh** (#243) | proposed isolated Astra local Unity worktree; not yet acquired | user-approved three screenshot targets, Page 1 exact blue/teal sentence and focused visual qualification; [handoff](../../Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/HANDOFF.md) | **READY FOR ASTRA — NOT RUNNING / NO FILE OR UNITY LOCK**; Astra must refresh live coordination and mark claim RUNNING before edits |

Housekeeping: browser tooling created temporary remote refs `tmp-noop-integration-check` and `noop`, both pointing at the integrated candidate. They do not affect product or candidate state. Local orchestration should delete them when convenient; browser connector has no branch-delete action.

## User / build gate

Browser Overall now issues:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

The user must personally playtest the exact published `integration/scenario-one-player-test-ready-v1` candidate before any Windows build is authorised.

After the personal playtest, Browser Overall records the user-test result. A Windows build remains blocked until the user explicitly authorises it **after** that playtest.

## Hard boundaries

- Research Source Consolidation Audit #194 remains **NO ACTION**.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- Never weaken tests or silently rebaseline protected anchors.
- User-directed #238 test-volume reduction remains binding; do not rerun valid Tier-3 evidence without a new reason.
- The accepted stale economy v14 fixture remains non-blocking verification debt; it is not part of the player-test gate.
