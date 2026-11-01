# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-11-01 by Overall Manager / OpenAI GPT-5.6 Sol after reviewing the unified local return, approving MFM-10, approving/integrating #199, and releasing PARITY-09 #219 plus MFM-11 #220.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 remains the sole primary writer for this file during the current two-lane split.** #201/#203 and child workers refresh/read it but do not become competing writers.
- Workers refresh this file and their claim at task start and before asserting that ownership or the Unity lock is free.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. A historical `free` receipt does not prove current availability.
- No Windows build before the user's personal Scenario One playtest and explicit post-playtest authorisation.
- After meaningful project-state changes, #200/Overall Manager refreshes `jermur788/Overall-Dev/updates/ccf/latest.md`; CCF remains canonical.

## Current integration truth

Published Scenario One integration:

`integration/scenario-one-player-test-ready-v1` = **`3acdf9cb635cdca79606335801e97f9d7dcce8b0`**.

This is exact prior integration `6784ab17...` plus the tested #199 Option-1 production repair. #199 was a one-commit fast-forward; its later handoff/review commits were not moved into the production integration ref.

Accepted current-lineage state includes:

- Rack V2 / D-064 feature scope VERIFIED;
- PARITY-08 accepted/integrated ancestor;
- S1-C T-08 accepted / feature qualified;
- S1-D T-08 accepted / feature qualified and T-09 independent review accepted;
- MFM-08 accepted/integrated;
- MFM-09 accepted/integrated;
- **MFM-10 #213 APPROVED / CLOSED — `SPECIES GUARD QUALIFIED`;**
- **#199 Combined HUD / Focus Option 1 APPROVED / INTEGRATED / CLOSED.**

MFM-10 Manager review: `Docs/Handoffs/MFM-10-Species-Parity-Guard/MANAGER-REVIEW-20261101.md`.

#199 Manager review: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-REVIEW-20261101.md`.

### MFM-10 evidence interpretation

The raw ThreeSpecies density command remains exit 1 and is **not** relabelled globally green. MFM-10 is accepted because all MFM-10 species-guard / Oak-Beech-mixed routing assertions pass and the only red families are outside the MFM-10 defect surface:

- three pure-Beech reduced-simulator R2 misses remain the previously adjudicated **ACCEPTED KNOWN C-LEVEL LIMITATION**, with criterion `0.40` unchanged;
- `MaxRemovedBasalShare` is a prescription removal-share safeguard caught by an over-broad identifier scanner, not a biological species maximum-density / maximum-basal-area claim.

The strict exit-zero completion wording in the earlier browser packet is superseded for MFM-10 only. No test, threshold, fixture, production value or baseline was changed.

### #199 accepted residuals

The repaired protected gates are green. Manager accepted the disclosed presentation trade-offs (estimate 15/14/13 px scale, tighter competitor rows, 8→2 side-panel gap) for this repair; retain them for playtest observation rather than treating them as a universal UI standard.

The independently reproduced `guided-v2` red set remains separate pre-existing debt: 11 identical failures on exact base and repaired candidate, including stale v23 pins, Objectives-window overflow, teaching-sequence indices and the V2 rack lesson expectation. **Do not claim guided-v2 globally green.** Reconcile this debt before any later qualification gate that requires guided-v2 green.

The worker's direct lock-JSON HEAD metadata edit is recorded as a process deviation and must not become normal practice.

## Live / released tasks

| Task | Lane | Exact base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, repins, claims, this file | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | child lanes only | DeepSeek routing + evidence; Mimo 2.6 Pro authorised worker/reviewer | **RUNNING — NEXT CHILDREN #219 / #220** |
| **MFM-10 local residual** (#213) | completed | `6784ab17...` | qualification only | **APPROVED / CLOSED — `SPECIES GUARD QUALIFIED`** |
| **Combined HUD / Focus Option 1** (#199) | completed + integrated | `6784ab17...` → `3acdf9cb...` | bounded UI presentation/focus repair | **APPROVED / INTEGRATED / CLOSED** |
| **PARITY-09 integrated parity qualification** (#219) | #203 local + independent review | `3acdf9cb635cdca79606335801e97f9d7dcce8b0` | Tier-2 qualification only; no production repair | **RELEASED / READY** |
| **MFM-11 calibration impact** (#220) | #203 local + independent review | `3acdf9cb635cdca79606335801e97f9d7dcce8b0` | quantitative qualification only; no retune | **RELEASED / READY** |
| **MFM-12 integrated MFM qualification** | #200 repin then #203 local | TBD after accepted MFM-11 | integrated verification + independent review | **BLOCKED ON MFM-11** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #203 local | not yet released | full integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | existing verification branch | non-blocking verification | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves the Unity lock is currently free. Every Unity worker must fresh-preflight/acquire through the authoritative tool.

## Released local sequence

### PARITY-09 — #219

Binding release amendment:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/RELEASE-REPIN-20261101.md`

Run the full approved parity Tier-2 checklist on exact `3acdf9cb...`, including protected pure-Sitka/Legacy40/Reference contracts, save/load, player-facing truth, completion/solvency and MFM-10 Sitka-only guard. No production fix authority. Independent cross-provider review required.

### MFM-11 — #220

Binding release amendment:

`Docs/Handoffs/MFM-11-Calibration-Impact/RELEASE-REPIN-20261101.md`

Compare sample-only, representative-prescription/default extension and plausible manual override; resolve the MFM-03 live/cross-year residual; measure forestry/economy/ecology/completion/provenance/save/compatibility impacts without retuning. Independent review required.

### Concurrency

DeepSeek may decompose and route #219/#220 using the unified #203 orchestration handoff and mandatory routing-evidence contract. Mimo 2.6 Pro remains authorised. Non-conflicting offline work may overlap in separate clean worktrees. **Heavy Unity remains serialized machine-wide.**

Neither worker self-accepts its gate. Returned handoffs require #200 Manager review before downstream release.

## Dependency-ready sequence

1. **DONE:** MFM-10 is `SPECIES GUARD QUALIFIED` and #213 is closed.
2. **DONE:** #199 is Manager-approved/integrated; current production integration is `3acdf9cb...`.
3. **NOW LOCAL:** execute released PARITY-09 #219 and MFM-11 #220 under #203, with Unity serialized.
4. #200 reviews each returned result. PARITY-09 may return `THREE-SPECIES PARITY P0 VERIFIED` only after accepted evidence/review. MFM-11 may return `MULTI-SCALE CALIBRATION QUALIFIED` only after accepted evidence/review.
5. Accepted MFM-11 -> #200 repins/releases MFM-12 on the then-current lineage.
6. Resolve any guided-v2 debt required by MFM-12/Tier-3 rather than silently carrying red gates into final qualification.
7. Accepted PARITY-09 + accepted MFM-12 + existing Rack/S1-C/S1-D requirements on one exact integrated candidate -> #200 may release Scenario One Tier-3.
8. Tier-3 success -> user personally playtests that exact candidate.
9. Windows build only after explicit post-playtest user authorisation.

## Hard boundaries

- Research Source Consolidation Audit **#194 remains NO ACTION**. Do not review, approve/reject, merge, update, clean up, reorganise Drive/Project sources or otherwise act on it unless the user explicitly lifts the restriction.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- Never weaken tests or silently rebaseline protected anchors.
- A production defect found by #219/#220 stops for a separate bounded repair claim.
- A material product/spec conflict returns to #200/user.

## Durable current records

- Unified local orchestration: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md`.
- MFM-10 Manager review: `Docs/Handoffs/MFM-10-Species-Parity-Guard/MANAGER-REVIEW-20261101.md`.
- #199 Manager review: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-REVIEW-20261101.md`.
- PARITY-09 release: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/RELEASE-REPIN-20261101.md`.
- MFM-11 release: `Docs/Handoffs/MFM-11-Calibration-Impact/RELEASE-REPIN-20261101.md`.
- Overall Dev sync policy: `Docs/Project/coordination/overall-dev-sync.md`.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
