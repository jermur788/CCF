# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-03 by Overall Manager / GPT-6 after #243 returned a 288/289 formal FAIL and #244 independent waiver/visual review was released. Browser/GitHub Manager coordination sole writer #200; local orchestration #201/#203.

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
| **Astra onboarding visual refresh** (#243) | isolated `feature/scenario-one-intro-astra-visual-refresh-20261103` / `/media/jer/ZX20/CCF-Intro-Astra-20261103` | Sole production-file writer (existing allocation remains until final Manager disposition) | **[HANDED-OFF] — IMPLEMENTED; TWO UNITY RUNS FORMALLY FAIL 288/289 (UnityEditor.Search startup exception), INDEPENDENT REVIEW PENDING; NOT INTEGRATED.** Unity lock reportedly still HELD by #243 owner; exact-owner local release requested. ZX20 ~14 GB free, below 15 GiB floor. No Unity may start. |
| **Independent intro visuals / exception review** (#244) | Claude Sonnet 5.5 Medium, **read-only** inspection of existing verification lane and raw evidence | Verify both startup exception traces, all six screenshots, source diff, accepted image reference and safe waiver conditions | **READY / DISPATCHED; NOT YET REVIEWED** — no Unity, no code edits, no additional disk-heavy worktree. |

Housekeeping: browser tooling created temporary remote refs `tmp-noop-integration-check` and `noop`, both pointing at the integrated candidate. They do not affect product or candidate state. Local orchestration should delete them when convenient; browser connector has no branch-delete action.

## Astra #243 ownership and Unity lease boundary — 2026-11-03

- Manager #200 has **assigned exclusive onboarding production-file ownership to Astra #243** for the file set listed in the live table above. Other lanes must not edit those files while the claim remains active; this is ownership assignment, not proof that a worktree exists.
- The local branch/worktree names are authorised for isolated use **only if available**. Production must branch from the published player-test base, never write to `main` or the published integration checkout; preserve unrelated/untracked `Scenario1V1/` content reported in an existing checkout.
- **Unity slot:** the Manager authorises #243 as the next bounded Unity user for this task, but cannot confirm the machine is idle remotely. A real Editor/process/lock/user-playtest preflight and machine-wide lease acquisition by the local executor are mandatory before Unity starts. If the user's personal Unity playtest or another owner is active, remain waiting rather than claiming/replacing the lease. No competing Unity use permitted.
- Astra's latest claim comment reports the execution chat is attached to a **read-only Game Dev reference mirror**, not the real CCF repository/workspace. This is an environment blocker independent of file ownership and user approval. Reopen/attach the real CCF repository as a writable code-workspace root in the Astra environment before attempting managed `create_worktree`; do not bypass the workspace system or modify the existing checkout.
- Latest user-approved three images **in Astra's own execution chat** are reported to differ from earlier handoff ZIP hashes. Per latest direct user direction they are the visual source; obtain hashes from actual local originals and preserve both provenance sets. Added Page 1 sentence stays byte-exact and teal/blue.
- No Windows build, Player build, accepted-test rebaseline, or merge is permitted from this assignment; existing approved Tier-3, #232 and personal playtest remain intact.

## #243 returned onboarding implementation — Manager checkpoint 2026-11-03

The user-approved 3-page intro artwork and copy have been implemented on the isolated feature branch; **not integrated** into the published player-test candidate. Handoff: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/IMPLEMENTATION-HANDOFF.md` (feature branch). Returned evidence: two focused Unity Editor runs, **288 checks passed and 1 failed** each, 6 screenshots each at 1280×720 and 1920×1080. The single `NO_EXCEPTIONS_OR_ERRORS` failure reports a UnityEditor.Search indexer `ArgumentOutOfRangeException` at Editor startup. The formal gate result remains **FAIL** (exit 1); no current product defect is demonstrated but environmental causation is not independently established. Screenshot visual quality is worker-reviewed only; Browser Manager has not directly inspected the externally stored raw captures.

**Manager verdict: NEEDS MORE EVIDENCE**, not an approval. Select an **exact-exception documented waiver pathway** only if independent cross-provider reviewer #244 confirms both raw traces are Unity Editor startup-only, no CCF runtime effect or other errors exist, and all six real Unity renders satisfy accepted visual requirements. Do **not** filter the test, label the original gate green, or rerun a full accepted test matrix. Packet: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/REVIEW-PACKET-20261103.md`.

**Operational block:** #243 worker reports lock remains held under its own owner ID and the lane is clean; Manager has not independently verified machine state. Only that **same owner**, after checking the deployed lock service/Editor processes and matching exact lease, may release the lock. No remote forced unlock, process kill, arbitrary cleanup, or fresh Unity while ZX20 is under its 15 GiB free-space floor (reported ~14 GB). Record local release proof on #243. Review #244 can run without Unity.

The previously issued player-test-ready token applies solely to the already-published integration candidate, unchanged by this unintegrated feature. User personal playtest is **in progress**, not accepted. No Windows build until personal playtest completion and explicit user authorisation.

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
