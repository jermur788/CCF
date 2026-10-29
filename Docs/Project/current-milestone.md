# Current Development Milestone

## Purpose

Define the current Scenario One milestone, its live completion boundary and the work that remains. Verified implementation detail belongs in `Docs/Project/unity-project-overview.md`; accepted product decisions belong in `Docs/Project/decision-log.md`; temporary ownership and execution locks belong in `Docs/Project/coordination/active-tasks.md`.

**Last reconciled:** 2026-10-29.

## Milestone

**Scenario One — Complete-Loop Three-Species Demo Readiness**

The first external demo is the **complete Scenario One** (D-060), not a first-intervention pilot. Before that demo, Sitka spruce, Oak and Beech must be represented to comparable modelling depth (D-059), and the player must be able to assess the stand, plan and execute management, review consequences, make second and later interventions, repeat the management cycle and reach understandable scenario completion.

No external-demo candidate exists yet. No Windows build is authorised before the user personally playtests the Scenario One candidate and explicitly authorises a Windows build.

## Current programme state

### Rack V2 / D-064 — implementation in progress

Rack V2 is the authoritative Enlarged80 access-planning workflow. Rack geometry is planned on the Stand Map; confirmed entrances are to be physically marked in the 3D forest; Legacy40 and frozen Reference Future v1 compatibility remain mandatory.

The Rack V2 core, map planning/editing/spacing/confirmation/minimap work and the Combined HUD layout repair are integrated. The layout repair was Manager-approved and integrated through PR #80 / merge `b118d8e63f60d54c9f9a3bed6de40af83b35efc1` after exact-candidate Unity qualification.

**RACK-V2-08 — remove competing first-person rack planning and make Stand Map authoritative** is now Manager-approved, feature-qualified and integrated through PR #89 / merge `d925e9591f2fffb9419881a0e77031cefccf9aeb`. Its exact Unity-qualified candidate `0904f0f` passed the required model-0 and Rack V2 Guided/Sequential/Combined-HUD gates, including all seven previously-unwaived real-input assertions; Legacy40 and RACK-V2-07 preservation also passed. No Windows build was made.

The binding next Rack task is **RACK-V2-09 — physically mark confirmed rack entrance trees in orange** (claim #90). Its RACK-V2-07/RACK-V2-08 functional prerequisites are satisfied and the task packet is published, but execution is currently **storage-blocked**: the latest Unity handoff reported `/media/jer/ZX20` at approximately 11.5 GiB free, below the 15 GiB machine Unity-lock floor. No Unity task may launch until a fresh preflight proves that floor is restored.

Later Rack V2 help/teaching, persistence and full functional/rendered qualification tasks remain pending. Rack V2 is therefore **not yet complete**. MFM production remains gated by the Rack persistence/integration boundary in D-065.

### Sitka/Oak/Beech parity P0 — underway

Three-species parity work has started and several bounded slices are already integrated and/or Unity-qualified. The old statement that parity P0 had not started is superseded.

Current accepted state includes:
- **PARITY-03 — mixed-density parity:** Manager-approved and Unity-qualified; independent static review accepted. Q11 documentation/evidence-record repair is complete. Q10 remains a non-blocking evidence-strengthening follow-up and does not invalidate the accepted qualification.
- **PARITY-04 — adult Oak/Beech parity:** bounded documentation/verification alignment is integrated and focused adult Unity qualification is Manager-approved.
- **PARITY-05 — early-life growth and browsing parity:** implementation/docs/verification patch is Manager-reviewed and integrated through PR #69; focused Unity qualification remains pending under claim #70.

Other approved parity tasks remain before the whole three-species P0 can be marked complete. Comparable modelling depth does not imply equal empirical precision. Accepted evidence classes and limitations in `specs/scenario-one-three-species-parity-p0/` remain binding; unsupported precision must not be invented.

### S1-C — review/history for repeated intervention — underway

S1-C is pre-demo scope under D-060 and is no longer unimplemented.

- T-01 per-year query/history foundation: integrated.
- T-02 Annual Review year browsing/query wiring: integrated and qualified.
- **T-03 intervention drill-down and money honesty:** static implementation and offline qualification are Manager-accepted, but the change is **not integrated, not Unity-verified and not accepted as a complete slice**. Rendered Tier-2 qualification remains under claim #19 and waits behind the Rack critical path unless the Manager explicitly reroutes the Unity lane.
- Later S1-C tasks remain subject to the approved spec and dependencies.

Grouped harvest money remains authoritative at the annual settlement level; successful non-harvest work may show its own recorded contractor/material/owner-time values. Historical browsing must remain read-only with respect to current game state.

### S1-D — repeated interventions — underway

S1-D is pre-demo scope under D-060 and is no longer unimplemented.

- T-00 product decisions: complete.
- T-01 repeated-intervention baseline/assumption re-verification: Manager-accepted and integrated.
- T-02 advisory cycle/readiness state: Manager-accepted bounded slice and integrated; its new script/meta still needs real Unity import/compile observation during a later applicable S1-C/S1-D Unity gate.
- T-03 teaching-footprint-cap decoupling, T-04 guide monotonicity and T-05 previous-vs-proposed management remain.
- **T-06 management-event ID integrity:** static implementation is complete. Independent source review by GPT-5.6 Sol / OpenAI passed, and OpenAI gpt-6.1-sol independently reproduced the executable evidence on exact reviewed candidate `29bb6cb`: T-06 75 PASS + 7/7 mutants, N3 88 PASS, T-02 93 PASS + 7/7 mutants, with focused method-body negative-control evidence. The independent review gate is complete. T-06 is **not yet Unity-verified, integrated or accepted as a complete slice**; claim #88 owns its queued runtime save-load/no-mutation qualification.
- T-07 persisted cycle-state expansion is cancelled for V1.
- T-08 multi-cycle witness remains blocked until the required implementation tasks are complete; T-09 independent spec review follows T-08.

The machine storage-floor blocker currently prevents T-06's Unity qualification as well as other queued Unity work.

### Multi-Scale Forestry Management V1 / D-065 — planning approved, production gated

Multi-Scale Forestry Management V1 is required before beginner/external Scenario One testing. Its approved product rule remains: demonstrated sample treatment may be applied to comparable rack-served areas; proposals are not Fell marks; player judgement remains required; Work Plan approval is the commitment gate.

MFM-02 rack-served sample/coverage geometry is integrated. **MFM-03 — measure and calibrate representative-management coverage** remains authorised measurement work, but claim #39 is waiting behind the current Rack critical path for any Unity execution. Production coding for the management workflow remains gated by Rack V2 persistence/integration as specified by D-065.

## Pre-demo sequence

The sequence is dependency-driven rather than a licence to bypass live ownership. Current intent is:

1. Finish the Rack V2 critical path required by D-064 and the MFM dependency.
2. Complete and qualify the remaining Sitka/Oak/Beech parity P0 slices.
3. Complete S1-C review/history work.
4. Complete S1-D second-and-later-intervention support.
5. Complete and qualify Multi-Scale Forestry Management V1 before beginner/external testing.
6. Prove Scenario One completion is reachable and understandable across the complete loop.
7. Run Tier-3 full integrated Scenario One qualification.
8. User personally playtests the qualified Scenario One candidate.
9. Only after explicit user authorisation, create the fresh post-parity/full-loop Windows candidate.
10. Independent review plus native Windows technical smoke, then beginner external testing of the complete Scenario One.

The live Unity execution order is controlled by `Docs/Project/coordination/active-tasks.md` and Manager release, not by this numbered list alone.

## Completion checks

Before the first external Scenario One demo:

- the player can start from the initial forest, assess it, make an intervention, advance time, review consequences, make second and later interventions, adapt, repeat and reach scenario completion;
- Sitka spruce, Oak and Beech have comparable modelling depth with evidence quality labelled honestly;
- pure-Sitka protected arithmetic and anchors remain exact where required by D-061 and the parity spec;
- Oak and Beech do not silently inherit materially inappropriate Sitka-specific assumptions;
- mixed density uses the accepted ordinal species-aware growing-space architecture with no DEC, DMC or mixture-productivity bonus;
- Oak/Beech timber/economy follows the accepted parity evidence and does not fabricate form/site precision;
- Rack V2 uses Stand Map planning for Enlarged80, preserves compatibility and provides the approved physical entrance-marking behaviour;
- Multi-Scale Forestry Management V1 preserves player judgement and Work Plan commitment semantics;
- S1-C/S1-D history, advisory state and repeated-intervention behaviour are coherent across multiple cycles;
- malformed/ambiguous save data required by approved hardening tasks is rejected before mutation rather than silently repaired;
- frozen Reference Future v1 remains unchanged;
- full integrated qualification is green before a release candidate is considered;
- no Windows build occurs before the user's personal playtest and explicit authorisation.

## Compatibility and evidence constraints

- Preserve Legacy40 compatibility where the governing task/spec requires it.
- Preserve frozen Reference Future v1 bytes/semantics; do not resize, regenerate, migrate or reinterpret it.
- Preserve unrelated dirty work; one writer per owned subsystem/file set.
- Do not weaken verification merely to obtain a green result.
- Distinguish IMPLEMENTED, REVIEWED, INTEGRATED, VERIFIED and ACCEPTED.
- Use the authoritative evidence classes defined by the approved parity/evidence work. Game abstractions and gameplay calibrations must not be presented as measured Irish/UK forestry practice.
- Research guidance becomes a mechanic only when accepted by Decision Log/spec.

## Build candidate policy

Historical B06 evidence is not an external candidate. Earlier pre-parity or pre-full-loop builds are engineering evidence only. The eventual external-demo Windows candidate must be created fresh from the qualified complete-loop lineage after the user playtest/authorisation gate.

Player/native-Windows text, input and performance remain unqualified until their explicit gates run. Synthetic Editor input evidence does not substitute for native Windows smoke.

## Approved specs currently governing milestone work

- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` — Rack V2 / D-064.
- `specs/scenario-one-three-species-parity-p0/` — three-species parity / D-059, D-061, D-062.
- `specs/scenario-one-review-history-v1/` — S1-C review/history.
- `specs/scenario-one-repeated-interventions-v1/` — S1-D repeated interventions.
- `specs/multiscale-forestry-management-v1/` — MFM V1 / D-065; production coding remains gated as specified.

Only APPROVED / IMPLEMENTING / VERIFYING / VERIFIED specs authorise implementation. DRAFT/SUPERSEDED specs do not.

## Explicit exclusions

No Scenario Two implementation, sandbox, multi-scenario product expansion, unapproved yield equations, mixture-productivity bonus, stocking reduction, Reference Future modification, API/pay-as-you-go enablement, or unauthorised Windows build is part of this milestone. Deferred systems do not become scope merely because underlying infrastructure exists.

## Live coordination

`Docs/Project/coordination/active-tasks.md` is the live ownership/lock authority outside the context snapshot. Workers must refresh it and their task claim before substantial edits and before acquiring the machine-wide Unity lock. GitHub state alone does not prove local cleanliness or Unity/runtime state.
