# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-11-01 by Overall Manager / OpenAI GPT-5.6 Sol after MFM-09 #207 Manager APPROVED / exact fast-forward integration and release of MFM-10 #211. Browser/GitHub Manager ownership remains under #200; non-browser/local/Unity orchestration remains under #201/#203.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 is the sole primary writer for this file during the current two-lane split.** #201/#203 and child workers refresh/read it but do not become competing writers.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Refresh free space, Editor processes and the authoritative lock immediately before acquisition.
- A historical `free` lock receipt never reserves or proves future availability.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.
- **Overall Dev sync:** after meaningful project-state changes, #200/Overall Manager refreshes `jermur788/Overall-Dev/updates/ccf/latest.md` per `Docs/Project/coordination/overall-dev-sync.md`. Keep it concise and coordination-only; CCF remains canonical.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` is now **`6784ab17cac922a16815f4898a30737482101018`**.
- **MFM-09 #207 is MANAGER APPROVED / INTEGRATED / CLOSED.** It was integrated by exact fast-forward from `34e360f8...` to the tested candidate `6784ab17...`; no replay/reconstructed merge. Manager review: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/MANAGER-REVIEW-20261101.md`.
- MFM-09 accepted evidence: four authorised production UI files only; stable `extend-treatment` teaching ID after `response`; D-066 underlying stage meaning preserved; prescription approval remains ordinary Pending; Work Plan approval remains commitment; 29 Unity receipts, MFM-09 261 checks/16 shots, offline 32 checks + 17/17 mutants, protected regressions, clean lane/lock return.
- MFM-09 non-blocking residual: in a no-Covered / never-prescribes path, the extension teaching presentation may remain visible while manual management and underlying accepted progression remain available. This is a UX/playtest residual, not authority for new persistence. Any durable skip/decline/manual-completion state requires a later product/spec decision.
- MFM-08 #156 and integration #205 remain **MANAGER APPROVED / INTEGRATED / CLOSED** and are ancestors of the current integration.
- Rack V2 / D-064 remains **VERIFIED for feature scope**. PARITY-08 #126 and S1-D T-05 #151 remain **APPROVED / INTEGRATED / CLOSED** on the inherited lineage.
- #196 MapPlanning and #198 guided-v2 v24/D-066 reconciliations remain **APPROVED / CLOSED**.
- S1-C T-07 #187 remains **APPROVED / CLOSED**; genuine v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00` remains the accepted old-save fixture.
- S1-C T-08 #206 remains **APPROVED / FEATURE QUALIFIED / CLOSED**; its exact qualified ancestor `34e360f8...` is contained by the current integration and MFM-09 returned the applicable S1-C regressions green.
- S1-D T-08 #208 remains **APPROVED / FEATURE QUALIFIED / CLOSED**; S1-D T-09 #209 independent conformance review remains **APPROVED / CLOSED**. Their exact qualified ancestor `34e360f8...` is contained by the current integration and MFM-09 returned the applicable T-04/T-04P/T-05 regressions green.
- S1-D non-blocking residuals remain: the T-08 D-057 path stayed solvent but thin at €1,336.04 after post-completion work; another harvest is correctly refused by the ordinary €2,500 cash rule. Retain this as a calibration/player-experience risk for playtest, not a defect or coefficient-change authority. Stale T-03/T-06 mutant anchors and the old S1-C T-05 offline v22 pin are verification debt.
- **MFM-10 #211 is RELEASED / READY and is the preferred next local programme task** on exact `6784ab17...`. Required success token: `SPECIES GUARD QUALIFIED`.
- **#199 Combined HUD / Focus Option 1 is REPINNED / READY / QUEUED** on exact `6784ab17...` by `OPTION1-REPIN-20261101.md`; it remains required pre-Tier-3.
- #200 browser umbrella remains **RUNNING**. #201/#203 local orchestration remains **RUNNING**.
- Old umbrellas #162/#106 remain SUPERSEDED/CLOSED. Verification Harness Hygiene #193 remains APPROVED/INTEGRATED/CLOSED.
- **Research Source Consolidation Audit #194 remains NO ACTION.** Do not review, approve/reject, merge, update, clean up, reorganise Drive/Project sources or otherwise act on it unless the user explicitly lifts the restriction.
- PARITY-09 remains blocked until MFM-10 returns `SPECIES GUARD QUALIFIED`; MFM-11 also waits MFM-10, then MFM-12 follows MFM-11 and applicable integrated lineage.
- Scenario One Tier-3/player-test candidate remains **NOT RELEASED**. Required accepted systems must converge on one exact candidate and #199 must be green before final Tier-3.
- **Overall Dev coordination publication is verified:** CCF maintains `jermur788/Overall-Dev/updates/ccf/latest.md`. Actual Dashboard ingestion/rendering of that coordination data is not yet independently verified from this browser lane.

## Live / prepared tasks

| Task | Lane | Exact branch / base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, coordination, repins, claims, this file | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | non-browser orchestration | child lanes only | local routing + evidence | **RUNNING — NEXT CHILD #211** |
| **MFM-09 teaching / Work Plan integration** (#207) | completed #203 child | tested/integrated `6784ab17...` | teaching/Work Plan integration | **APPROVED / INTEGRATED / CLOSED** |
| **MFM-10 species-parity guard** (#211) | #203 local; Unity if required | exact start `6784ab17...`; `verify/mfm-10-species-guard-v1` suggested | qualification only by default | **RELEASED / READY — NEXT LOCAL PRIORITY** |
| **Combined HUD / Focus Repair** (#199) | #203 local + independent review | exact start `6784ab17...`; fresh branch from current lineage | bounded Option-1 presentation/focus repair | **REPINNED / READY / QUEUED PRE-TIER3** |
| **PARITY-09 integrated parity qualification** | #200 repin then #203 local | TBD after MFM-10 | qualification | **BLOCKED ON MFM-10 `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | #203 local + independent review | repin after MFM-10 | measurement/qualification | **WAITING MFM-10** |
| **MFM-12 integrated MFM qualification** | #203 local + independent review | repin after MFM-11 + applicable lineage | integrated verification | **WAITING MFM-11** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #203 local | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | `verify/parity-03-density-continuity-q10-v1` | verification only | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves the Unity lock is currently free. Every Unity worker must fresh-preflight/acquire through the authoritative tool.

## Single-writer / task boundaries

### #200 browser Manager
Browser/GitHub only and sole writer of this file. Owns Manager decisions, browser/static review, exact-candidate repins and status. No local C#/Unity implementation or unreturned runtime claims.

### #201 / #203 local orchestration
Substantive local work uses child claims. Read this file before dispatch and return coordination changes to #200. Maintain adaptive-routing evidence. DeepSeek is the orchestrator/default worker for bounded/easier work; Sonnet 5.5 Medium is the genuinely harder-task escalation; Opus only after classified capability/reasoning failure or separately justified high-consequence need.

### #211 MFM-10
Qualification-only by default. Start from exact `6784ab17...` after fresh live preflight. Prove Sitka control behaviour is unchanged and Oak/Beech are excluded from Sitka-only quantitative candidate/release/proposal rules with explicit honest reasons. Preserve manual-state precedence, Pending-only prescription approval, Work Plan commitment, provenance/save compatibility and protected Reference/Legacy/Sitka anchors. A genuine production defect stops for a separate repair claim. Any desire for multi-species quantitative treatment stops for spec amendment; never generalise silently.

Difficulty is 3/5: DeepSeek is the default executor if capable. Sonnet 5.5 Medium is escalation for a classified harder/capability need; Opus is not justified by environment, lock, quota, genuine gate or authority failures.

### #199 Option 1
Start fresh from exact `6784ab17...` when scheduled, not stale partial work. Preserve ordinary-inspection minimap, accepted PARITY-07 estimate/prompt/detail requirements, protected Combined assertions, MFM-08 `TreeInspectionView` behavior and integrated MFM-09 teaching/Work Plan behavior; repair 1920 competitor clipping through bounded reflow. Independent review required. No Rack/MFM semantics, save/schema/anchor/reference/ecology/economy change.

### Research source consolidation
**NO ACTION.** Do not act on #194 unless the user explicitly lifts the restriction.

## Dependency-ready sequence

1. #203 executes **MFM-10 #211** next after fresh local preflight; DeepSeek should perform the bounded guard if capable.
2. #200 reviews returned #211 evidence. Only an accepted exact token `SPECIES GUARD QUALIFIED` releases downstream parity/calibration work.
3. MFM-10 accepted -> #200 fresh-repins/releases PARITY-09 and MFM-11 as ownership/Unity sequencing permits.
4. MFM-11 accepted -> repin/release MFM-12 on the applicable integrated lineage.
5. #199 Option 1 remains exact-SHA ready and required pre-Tier-3; schedule without creating overlapping UI ownership with MFM work.
6. Rack V2 G4 + PARITY-09 + accepted S1-C + accepted S1-D + MFM-12 + #199 closure accepted/integrated on one exact candidate -> Scenario One Tier-3.
7. Tier-3 success -> user personally playtests the same exact candidate.
8. Windows build only after explicit post-playtest user authorisation.

## Durable current records

- Overall Dev sync policy: `Docs/Project/coordination/overall-dev-sync.md`; shared CCF status: `jermur788/Overall-Dev/updates/ccf/latest.md`.
- #207 MFM-09 Manager review: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/MANAGER-REVIEW-20261101.md`.
- #211 MFM-10 release/repin: `Docs/Handoffs/MFM-10-Species-Parity-Guard/RELEASE-REPIN-20261101.md`.
- #199 Option 1 current repin: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-REPIN-20261101.md`.
- #208 S1-D T-08 handoff: `Docs/Handoffs/S1-D-Implementation-08/HANDOFF.md` on final docs lineage `605ce465...`.
- #209 S1-D T-09 independent review: `Docs/Handoffs/S1-D-Implementation-09/INDEPENDENT-REVIEW-20261031.md`.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
