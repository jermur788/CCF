# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-01 by Overall Manager / OpenAI GPT-5.6 Sol after PARITY-09 #219 returned for Manager review and browser continuation #221 was published. Browser/GitHub Manager ownership remains under #200; local/Unity orchestration remains under #201/#203.

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

`integration/scenario-one-player-test-ready-v1` = **`3acdf9cb635cdca79606335801e97f9d7dcce8b0`**.

Accepted current-lineage state includes Rack V2 / D-064 feature scope VERIFIED; PARITY-08 accepted/integrated; S1-C T-08 accepted/feature-qualified; S1-D T-08 accepted/feature-qualified plus T-09 accepted; MFM-08 and MFM-09 accepted/integrated; MFM-10 #213 APPROVED/CLOSED with `SPECIES GUARD QUALIFIED`; and #199 Combined HUD / Focus Option 1 APPROVED/INTEGRATED/CLOSED at the current integration head.

#199 accepted presentation trade-offs remain playtest observations rather than a universal UI standard. The independently reproduced 11-failure `guided-v2` set remains separate current-lineage verification debt; do not claim that gate globally green.

## Live / released tasks

| Task | Lane | Exact base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, releases, coordination | **RUNNING — UMBRELLA** |
| **Browser Manager Continuation 04** (#221) | browser/GitHub | current integration + returned handoffs | #219 review; verification-debt disposition; MFM-12/Tier-3 prep | **READY — HANDOFF PUBLISHED** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | child lanes only | DeepSeek routing + evidence; Mimo authorised | **RUNNING** |
| **PARITY-09 integrated parity qualification** (#219) | returned from #203 | `3acdf9cb635cdca79606335801e97f9d7dcce8b0` | qualification + independent review | **HANDED OFF — MANAGER REVIEW UNDER #221** |
| **MFM-11 calibration impact** (#220) | #203 local + independent review | `3acdf9cb635cdca79606335801e97f9d7dcce8b0` | quantitative qualification only; no retune | **RUNNING** |
| **MFM-12 integrated MFM qualification** | #200 prep then #203 local | exact candidate TBD after accepted #220 | integrated verification + independent review | **NOT RELEASED — BLOCKED ON MFM-11** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 prep/review then #203 local | not yet released | full integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | existing verification branch | non-blocking verification | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves the Unity lock is currently free.

## PARITY-09 returned state

#219 returned qualification and independent cross-provider review on exact `3acdf9cb...`:

- qualification handoff commit `aa8eb6ff`;
- independent-review commit `87aee15f`;
- independent verdict `APPROVED`;
- new exact-candidate Unity evidence includes adult 30/30 in Legacy40 and Enlarged80, density hard checks/anchors/victim sets, model-2 Legacy40 completion, Reference archive/preview/continuation and SaveHardening;
- PARITY-07 1031/0/36 is reused from #199 on the same exact candidate;
- no production/spec defect was reported.

Reported red/debt items still require Manager classification and must not be silently relabelled green:

1. five stale v23 save-version pins;
2. `MaxRemovedBasalShare` source-scanner false positive;
3. three already-accepted pure-Beech reduced-simulator R2 non-passes;
4. history harness project omission of `ScenarioManagementHistory.cs`;
5. eleven pre-existing guided-v2 failures;
6. no fresh current-candidate Unity timber/pruning assertion because stale v23 pins abort those gates.

Browser claim #221 owns that classification and the PARITY-09 Manager verdict.

## Browser continuation #221

Binding handoff:

`Docs/Handoffs/ScenarioOne-Browser-Continuation-04/BROWSER-HANDOFF-20261101.md`

#221 must:

- review #219 and issue `APPROVED / NEEDS CHANGES / REJECTED`;
- issue `THREE-SPECIES PARITY P0 VERIFIED` only if supported;
- classify each red item as accepted limitation, stale/broken verification asset, real defect, or unresolved;
- publish the smallest verification-only/local repair packet where required;
- prepare MFM-12 browser readiness/task packet but keep it blocked until Manager-accepted MFM-11;
- refresh Tier-3 readiness without releasing Tier-3;
- if #220 returns during the browser run, review it and release MFM-12 only after accepted `MULTI-SCALE CALIBRATION QUALIFIED`.

## Dependency-ready sequence

1. **NOW BROWSER:** #221 reviews returned PARITY-09 #219 and classifies verification debt.
2. **NOW LOCAL:** #220 continues MFM-11 under #203; heavy Unity remains serialized.
3. If #219 is accepted, close/update its claim and record `THREE-SPECIES PARITY P0 VERIFIED` without globally waiving red verification debt.
4. If a bounded verification repair is required before MFM-12/Tier-3, #221 creates the exact local claim/packet; real product defects receive separate repair claims.
5. Accepted #220 -> #221/#200 may freshly repin/release MFM-12.
6. Accepted PARITY-09 + accepted MFM-12 + inherited Rack/S1-C/S1-D requirements + required verification-debt closure on one exact candidate -> #200 may release Scenario One Tier-3.
7. Tier-3 success -> user personally playtests that exact candidate.
8. Windows build only after explicit post-playtest user authorisation.

## Hard boundaries

- Research Source Consolidation Audit **#194 remains NO ACTION**. Do not inspect/review/approve/reject/merge/update/reorganise it unless the user explicitly lifts the restriction.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- Never weaken tests or silently rebaseline protected anchors.
- Qualification claims have no production-repair authority.
- Local workers do not edit this file.

## Durable current records

- Browser continuation #221: `Docs/Handoffs/ScenarioOne-Browser-Continuation-04/BROWSER-HANDOFF-20261101.md`.
- Unified local orchestration: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md`.
- MFM-10 Manager review: `Docs/Handoffs/MFM-10-Species-Parity-Guard/MANAGER-REVIEW-20261101.md`.
- #199 Manager review: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/MANAGER-REVIEW-20261101.md`.
- PARITY-09 release: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/RELEASE-REPIN-20261101.md`.
- MFM-11 release: `Docs/Handoffs/MFM-11-Calibration-Impact/RELEASE-REPIN-20261101.md`.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
