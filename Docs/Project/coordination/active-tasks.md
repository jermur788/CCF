# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-11-01 by Overall Manager / OpenAI GPT-5.6 Sol after publishing the unified local #203 orchestration handoff for all currently local-ready work. Browser/GitHub Manager ownership remains under #200; non-browser/local/Unity orchestration remains under #201/#203.

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
- **Overall Dev sync:** after meaningful project-state changes, #200/Overall Manager refreshes `jermur788/Overall-Dev/updates/ccf/latest.md` per `Docs/Project/coordination/overall-dev-sync.md`. CCF remains canonical.

## Current integration truth

- Published Scenario One integration `integration/scenario-one-player-test-ready-v1` is **`6784ab17cac922a16815f4898a30737482101018`**.
- **MFM-09 #207 is MANAGER APPROVED / INTEGRATED / CLOSED.** It was integrated by exact fast-forward to the tested candidate `6784ab17...`. Manager review: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/MANAGER-REVIEW-20261101.md`.
- MFM-09 non-blocking residual remains: a no-Covered / never-prescribes path can keep extension teaching presentation visible while manual management and underlying progression remain available. Any durable skip/decline/manual-completion state requires a later product/spec decision.
- MFM-08 #156/#205 remain APPROVED / INTEGRATED / CLOSED ancestors. Rack V2 / D-064 remains feature-scope VERIFIED. PARITY-08 #126, S1-C T-08 #206, S1-D T-08 #208 and S1-D T-09 #209 remain accepted on the inherited lineage/evidence.
- **The original unsplit MFM-10 claim #211 is SUPERSEDED / CLOSED.** User-directed split authority: `Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-SPLIT-HANDOFF-20261101.md` at `a1fc1fa12a1565d08d1c83d05022250ecbdcc919`.
- **#212 browser/static MFM-10 guard prep is HANDED OFF / CLOSED.** Durable result: `Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-HANDOFF-20261101.md` (publication commit `a3372ce5f0c8c2e7fdbff61a74d395c7fff9f2fa`). Static verdict: no code/spec blocker; no MFM-10 Unity requirement; exact residual is two existing deterministic offline runners.
- **#213 is READY for the local-only MFM-10 residual qualification.** It must consume #212's handoff rather than repeat browser-capable review. Required matrix is only the MFM prescription-core offline runner plus the ThreeSpecies density offline suite on exact `6784ab17...`; Unity is not required unless contradictory local evidence invalidates the audited static boundary. Final required token remains `SPECIES GUARD QUALIFIED`.
- **#199 Combined HUD / Focus Option 1 remains REPINNED / READY / QUEUED** on exact `6784ab17...`; it is required pre-Tier-3. Under the user's 2026-11-01 unified-local direction, #203 may schedule it alongside #213 when ownership/worktree separation and Unity serialization are safe.
- **Unified local #203 master handoff is published:** `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md` at `6c77907e8cd4ceaec3ccf2d32a5dbaaff19eca0d`. DeepSeek is the orchestrator. Mimo 2.6 Pro is explicitly authorised as an additional worker/reviewer; the user's report of substantial remaining token capacity is qualitative evidence only until runtime/provider evidence is observed. Attached `ORCHESTRATION-EVIDENCE-APPENDIX-V2.md` and `ORCHESTRATION-LEDGER-20261030.md` define the mandatory routing telemetry/provenance discipline for the run.
- #200 browser umbrella remains RUNNING. #201/#203 local orchestration remains RUNNING.
- **Research Source Consolidation Audit #194 remains NO ACTION.** Do not review, approve/reject, merge, update, clean up, reorganise Drive/Project sources or otherwise act on it unless the user explicitly lifts the restriction.
- PARITY-09 and MFM-11 remain blocked until MFM-10 returns `SPECIES GUARD QUALIFIED` **and #200 accepts/releases the downstream work**; MFM-12 follows accepted MFM-11 on the applicable lineage.
- Scenario One Tier-3/player-test candidate remains **NOT RELEASED**. Required accepted systems must converge on one exact candidate and #199 must be green before final Tier-3.
- Overall Dev coordination publication is refreshed through `jermur788/Overall-Dev` commit `c2f56fd389fe95111a34d193a9674fa44664fc46`; actual Dashboard ingestion/rendering remains unverified from this browser lane.

## Live / prepared tasks

| Task | Lane | Exact branch / base | Ownership | Status |
|---|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | no local worktree | Manager review, coordination, repins, claims, this file | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | non-browser orchestration | child lanes only | DeepSeek routing + evidence; Mimo 2.6 Pro authorised worker/reviewer | **RUNNING — UNIFIED MASTER HANDOFF; #213 + #199 LOCALLY RUNNABLE AS SAFE** |
| **MFM-10 browser/static guard prep** (#212) | browser/GitHub | exact candidate `6784ab17...`; no worktree | static/spec/repository audit + residual local matrix | **HANDED OFF / CLOSED** |
| **MFM-10 local residual qualification** (#213) | #203 local; no Unity expected from browser matrix | exact candidate `6784ab17...`; `verify/mfm-10-species-guard-v1` suggested | two residual executable offline suites only | **READY — CRITICAL-PATH GATE** |
| **MFM-10 original unsplit claim** (#211) | none | superseded | no execution authority | **SUPERSEDED / CLOSED** |
| **Combined HUD / Focus Repair** (#199) | #203 local + independent review | exact start `6784ab17...`; fresh branch from current lineage | bounded Option-1 presentation/focus repair | **REPINNED / READY — MAY RUN WITH #213 IF OWNERSHIP/UNITY SAFE** |
| **PARITY-09 integrated parity qualification** | #200 repin then #203 local | TBD after MFM-10 | qualification | **BLOCKED ON #200 ACCEPTANCE OF `SPECIES GUARD QUALIFIED`** |
| **MFM-11 calibration impact** | #203 local + independent review | repin after MFM-10 | measurement/qualification | **BLOCKED ON #200 RELEASE AFTER MFM-10** |
| **MFM-12 integrated MFM qualification** | #203 local + independent review | repin after MFM-11 + applicable lineage | integrated verification | **WAITING MFM-11** |
| **Scenario One Tier-3 / personal-player-test candidate** | #200 repin/review + #203 local | prepared skeleton only | final integrated qualification | **NOT RELEASED** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | `verify/parity-03-density-continuity-q10-v1` | verification only | **WAITING — NON-BLOCKING; UNITY REMAINS** |

No row proves the Unity lock is currently free. Every Unity worker must fresh-preflight/acquire through the authoritative tool.

## Unified local orchestration boundaries

The binding local orchestration packet is `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md`.

DeepSeek may decompose, sequence and parallelise currently released local tasks. Mimo 2.6 Pro may be used as a worker or independent reviewer when DeepSeek records the task-fit/capacity/independence reason. The orchestration run must collect routing telemetry using the attached evidence appendix and historical ledger pattern; unknown capacity/context values remain UNKNOWN. Local workers do not write this coordination file and do not self-release blocked downstream tasks.

### #213 local residual phase — ready

Consume #212's handoff rather than repeating it. After fresh local preflight on exact `6784ab17...`, execute only:

1. `python3 -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/01-prescription-core`
2. `python3 -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/02-three-species-density`

No Unity/Editor run is required if both pass. A genuine defect stops for a separate repair claim; a desire for multi-species quantitative treatment stops for spec amendment. Final durable output remains `Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md`; only accepted combined browser + local evidence may return `SPECIES GUARD QUALIFIED`.

### #199 Option 1 — ready for local scheduling

Start fresh from exact `6784ab17...`. Preserve ordinary-inspection minimap, accepted PARITY-07 estimate/prompt/detail requirements, protected Combined assertions, MFM-08 `TreeInspectionView` behavior and integrated MFM-09 teaching/Work Plan behavior. Independent review required. No Rack/MFM semantics, save/schema/anchor/reference/ecology/economy change. Heavy Unity is serialized and must use fresh lock preflight/acquisition.

### Research source consolidation

**NO ACTION.** Do not act on #194 unless the user explicitly lifts the restriction.

## Dependency-ready sequence

1. **DONE:** #212 browser/static MFM-10 guard prep completed and durable handoff published.
2. **DONE:** #200 reviewed the browser result, retained exact candidate `6784ab17...`, and authorised the two-command residual matrix with no Unity requirement.
3. **NOW LOCAL:** #203 executes **#213 local-only residual qualification**; #199 may also be scheduled/decomposed in parallel or sequence when its separate worktree/file ownership and serialized Unity requirements are safe.
4. #200 reviews returned #213. Only accepted combined evidence and exact token `SPECIES GUARD QUALIFIED` releases downstream parity/calibration work.
5. MFM-10 accepted -> #200 fresh repin/release PARITY-09 and MFM-11; #203 may then execute the released local children without needing a new umbrella handoff.
6. MFM-11 accepted -> #200 repin/release MFM-12 on the applicable integrated lineage; #203 executes the released local qualification.
7. #199 must return green, independent-review evidence and Manager disposition before Tier-3; local worker success alone is not integration/acceptance.
8. Rack V2 G4 + PARITY-09 + accepted S1-C + accepted S1-D + MFM-12 + #199 closure accepted/integrated on one exact candidate -> #200 may release Scenario One Tier-3.
9. Released Tier-3 local qualification may run under #203; success then requires the user to personally playtest the same exact candidate.
10. Windows build only after explicit post-playtest user authorisation.

## Durable current records

- Unified local orchestration master handoff: `Docs/Handoffs/ScenarioOne-Two-Lane-Continuation-20261031/LOCAL-MASTER-HANDOFF-20261101.md` at `6c77907e8cd4ceaec3ccf2d32a5dbaaff19eca0d`.
- MFM-10 browser/local split: `Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-SPLIT-HANDOFF-20261101.md`.
- MFM-10 browser/static result: `Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-HANDOFF-20261101.md`.
- #207 MFM-09 Manager review: `Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/MANAGER-REVIEW-20261101.md`.
- #199 Option 1 current repin: `Docs/Handoffs/ScenarioOne-Combined-HUD-Focus-Regression-01/OPTION1-REPIN-20261101.md`.
- Overall Dev sync policy: `Docs/Project/coordination/overall-dev-sync.md`; shared CCF status: `jermur788/Overall-Dev/updates/ccf/latest.md`.
- Tier-3 skeleton: `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/BROWSER-PREPARATION-20261030.md`.
