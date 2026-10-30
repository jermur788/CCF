# Current Development Milestone

## Purpose

Define the current Scenario One milestone, its completion boundary and the work that remains. Verified implementation detail belongs in `Docs/Project/unity-project-overview.md`; accepted product decisions belong in `Docs/Project/decision-log.md`; temporary ownership and execution locks belong in `Docs/Project/coordination/active-tasks.md`.

**Last reconciled:** 2026-10-30.

## Milestone

**Scenario One — Complete-Loop Three-Species Demo Readiness**

The first external demo is the **complete Scenario One** (D-060), not a first-intervention pilot. Before that demo, Sitka spruce, Oak and Beech must be represented to comparable modelling depth (D-059), and the player must be able to assess the stand, plan and execute management, review consequences, make second and later interventions, repeat the management cycle and reach understandable scenario completion.

No external-demo candidate exists yet. No Windows build is authorised before the user personally playtests the Scenario One candidate and explicitly authorises a Windows build.

## Current programme state

### Rack V2 / D-064 — feature-scope verified

Rack V2 is the authoritative Enlarged80 access-planning workflow. Rack geometry is planned on the Stand Map; confirmed entrances are physically marked in the 3D forest; Legacy40 and frozen Reference Future v1 compatibility remain mandatory.

The Rack V2 implementation, persistence, field marking, functional end-to-end and rendered qualification programme is complete for feature scope. The exact integrated Rack candidate `60fd9025e0205e7e8b24203295dd606ffa4a8061` is retained in the current published Scenario One integration ancestry.

The independent G4 review is published at `Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md` and returned:

`OBJECTIVE 1 RACK PLANNING & FIELD MARKING V2 QUALIFIED — READY FOR USER TEST`

That review disposes AC-1…AC-20 and D-064 OPEN-1…OPEN-3 as PASS from the accepted Rack evidence chain. Five rendered polish observations remain non-blocking for the later personal playtest. Rack V2 feature verification is distinct from whole-Scenario-One Tier-3 verification and from user acceptance.

The D-065 production gate requiring Rack V2 persistence/integration has therefore been satisfied. This does not bypass later MFM prerequisites or live ownership/Unity serialization.

### Sitka/Oak/Beech parity P0 — core slices accepted; final integrated parity still pending

The accepted parity programme now includes PARITY-03 through PARITY-08 on the published Scenario One integration lineage. Comparable modelling depth does not imply equal empirical precision; the accepted evidence classes/limitations remain binding and unsupported precision must not be invented.

Current status:

- **PARITY-03 mixed-density parity:** accepted/qualified. Q10 remains non-blocking evidence-strengthening work and does not invalidate the accepted result.
- **PARITY-04 adult Oak/Beech parity:** accepted/qualified.
- **PARITY-05 early-life growth/browsing parity:** accepted/integrated, including the authorised browse-anchor reconciliation.
- **PARITY-06 timber/economy parity:** accepted/qualified/integrated.
- **PARITY-07 rendered species parity:** accepted/qualified/integrated.
- **PARITY-08 persistence/back-compatibility:** independently approved and integrated; exact merge `7d2049e862df5c9311748167423b981e713d1fa9`, docs/integration successor `8fa83bff22ea1b4943689bee1074eed8eae65d2f`.
- **PARITY-09 final integrated parity qualification:** not yet released. Its authoritative start gate requires MFM-10 `SPECIES GUARD QUALIFIED`; after that token exists, PARITY-09 must be freshly repinned to the then-current integration candidate before local qualification.

The existing model-0 completion diagnostic mismatch was reproduced on the accepted baseline and remains pre-existing D-047/D-048 verification debt rather than a PARITY-08 regression. Protected pure-Sitka anchors remain unchanged where required.

### S1-C — review/history for repeated intervention — late-stage qualification

S1-C is pre-demo scope under D-060 and is substantially implemented/integrated.

Accepted/integrated programme state includes T-01 through T-06: read-only management-history query, Annual Review year browsing, intervention drill-down/money honesty, provenance labelling, objective/progression history and prior-treatment guidance.

A browser audit found that **T-07 — S1-D data contract and stability checks** had not received its own durable qualification and must not be inferred complete merely from later feature success. The existing history helper already documents important identity semantics, including stable ID pass-through and `harvestJobId` as a resolution-year grouping label rather than unique job identity, but T-07 also requires explicit offline stability assertions on current and genuine old-save fixtures.

T-07 is therefore released as bounded **offline verification** under claim #187. It requires no Unity by default and may run in parallel with the current #151 Unity task. It owns verification/docs only unless a separate Manager disposition is required.

**T-08 — final S1-C feature qualification** is browser-prepared but not released. It waits for T-07 Manager acceptance and a free serialized Unity lane. Its final Tier-2 matrix must include multi-year browsing, intervention drill-down, grouped-harvest money honesty, no-work year, provenance classes, prior-treatment guidance, objective history, current save/load, a genuine v20 old-save path, read-only history semantics and rendered 1280×720 / 1920×1080 evidence.

Grouped harvest money remains authoritative at the annual settlement level; successful non-harvest work may show its own recorded contractor/material/owner-time values. Historical browsing must remain read-only with respect to current game state.

### S1-D — repeated interventions — final feature gates underway

S1-D is pre-demo scope under D-060 and is substantially implemented.

Accepted/integrated state includes:

- T-01 repeated-intervention baseline re-verification;
- T-02 advisory cycle/readiness state;
- T-03 teaching-footprint-cap decoupling;
- T-04P durable one-way historical teaching-completion persistence;
- T-04 guide monotonicity;
- T-06 management-event ID integrity/save hardening, including repaired verification fixtures and Unity qualification/integration;
- T-07 broad persisted cycle-state expansion remains intentionally cancelled for V1 by D-066.

**T-05 — previous vs proposed management presentation** is the remaining active implementation/qualification gate. Its exact pushed head `4ddf4aa535d4ec5267be46d2635679411d595e0f` is independently approved offline/static; its Unity qualification is currently running. No Manager runtime/rendered verdict or integration may be inferred until the completed Unity handoff is inspected.

D-066's accepted persistence contract is one bounded v24 boolean-like historical teaching-completion latch written only at the genuine first guided sample/review completion boundary, default false on old saves, one-way false→true, and excluded from protected/world identity hashes. The stale rule that “S1-D adds no schema field” is superseded by that accepted decision. No broader per-cycle/guide/readiness persistence is authorised.

**T-08 — multi-cycle integrated witness** is browser-prepared but not released. It waits primarily for T-05 Unity qualification, final Manager approval and integration. It must prove at least three real management cycles, idle year(s), stale/carried Pending work, save/load, exact Annual Review/history navigation, completion, continued management after completion, D-057 solvent/economy viability and protected regressions.

**T-09 — independent spec-conformance review** follows T-08 PASS and is also browser-prepared. It must use a reviewer independent from the T-08 executor and verify the complete accepted S1-D contract, including D-066.

### Multi-Scale Forestry Management V1 / D-065 — core implemented; final sequence prepared

Multi-Scale Forestry Management V1 is required before beginner/external Scenario One testing. Its approved rule remains: demonstrated sample treatment may be applied to comparable rack-served areas; candidates/proposals do not replace player judgement; proposals are not Fell marks; prescription approval does not itself commit work; ordinary Work Plan approval remains the commitment gate.

The Rack persistence/integration production gate in D-065 is satisfied. MFM-02 through the core deterministic pipeline are present, and MFM-03 static calibration has been independently reproduced/accepted as a **GAMEPLAY CALIBRATION** input. MFM-03's independent-review caveats—especially unmeasured live post-Lesson-5/cross-year drift—remain binding and are explicitly carried into MFM-11 rather than treated as proven away.

Current final MFM sequence:

- **MFM-08 — Stand Map / 3D treatment review UX:** exact production candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06`; browser/static verdict `APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED`. Its Unity gate waits behind #151.
- **MFM-09 — teaching / Work Plan integration:** browser packet prepared, not released. Requires accepted/integrated #151 + #156 and fresh exact-lineage repin.
- **MFM-10 — species-parity integration guard:** browser packet prepared, not released. Requires MFM-09. Exact success token `SPECIES GUARD QUALIFIED`; Oak/Beech must not silently receive Sitka-only quantitative treatment.
- **MFM-11 — calibration impact qualification:** browser packet prepared, not released. Requires MFM-10. It must compare sample-only, representative-prescription and plausible manual paths, resolve the MFM-03 live/cross-year residual and return `MULTI-SCALE CALIBRATION QUALIFIED` or `BLOCKED — CALIBRATION IMPACT`, followed by independent review.
- **MFM-12 — integrated MFM qualification:** browser packet prepared, not released. It requires the complete approved requirements matrix, integrated Unity/rendered/save/compatibility/complete-loop evidence and independent architecture/spec review. Exact success verdict: `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`.

No browser-prepared packet authorises local implementation before its stated dependency/ownership gates are true.

## Pre-demo sequence

The sequence is dependency-driven rather than a licence to bypass live ownership. Current intent is:

1. Finish #151 S1-D T-05 Unity qualification; browser/Manager immediately performs final runtime/rendered review and safe integration only if APPROVED.
2. In parallel, complete S1-C T-07 offline contract/stability verification and Manager review.
3. After #151 releases the Unity lane, run #156 MFM-08 Tier-2 and final Manager review/integration.
4. Complete S1-C T-08 when T-07 is accepted and Unity is available.
5. Complete S1-D T-08 multi-cycle witness after T-05 integration, then T-09 independent review/Manager verification.
6. After accepted/integrated #151 + #156, release/complete MFM-09, then MFM-10.
7. Once MFM-10 returns `SPECIES GUARD QUALIFIED`, freshly repin/complete PARITY-09; MFM-11 may proceed on the accepted lineage as ownership/Unity serialization permits.
8. Complete MFM-12 after its accepted dependencies and applicable Rack/parity/S1-C/S1-D lineages are present.
9. Assemble one exact candidate containing verified Rack V2, final parity, accepted S1-C/S1-D and MFM-12; run Tier-3 full integrated Scenario One qualification.
10. User personally playtests the same qualified Scenario One candidate.
11. Only after explicit user authorisation, create the fresh post-parity/full-loop Windows candidate.
12. Independent/native Windows technical smoke, then beginner external testing of the complete Scenario One.

The live execution order and machine lock are controlled by `Docs/Project/coordination/active-tasks.md` and Manager release, not by this numbered list alone.

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

Historical B06 and other pre-parity/pre-full-loop builds are engineering evidence only, not external-demo candidates. The eventual external-demo Windows candidate must be created fresh from the qualified complete-loop lineage after the user playtest/authorisation gate.

Player/native-Windows text, input and performance remain unqualified until their explicit gates run. Synthetic Editor input evidence does not substitute for native Windows smoke.

## Approved specs currently governing milestone work

- `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` — Rack V2 / D-064.
- `specs/scenario-one-three-species-parity-p0/` — three-species parity / D-059, D-061, D-062.
- `specs/scenario-one-review-history-v1/` — S1-C review/history.
- `specs/scenario-one-repeated-interventions-v1/` — S1-D repeated interventions.
- `specs/multiscale-forestry-management-v1/` — MFM V1 / D-065.

Only APPROVED / IMPLEMENTING / VERIFYING / VERIFIED specs authorise implementation. DRAFT/SUPERSEDED specs do not.

## Explicit exclusions

No Scenario Two implementation, sandbox, multi-scenario product expansion, unapproved yield equations, mixture-productivity bonus, stocking reduction, Reference Future modification, API/pay-as-you-go enablement, or unauthorised Windows build is part of this milestone. Deferred systems do not become scope merely because underlying infrastructure exists.

## Live coordination

`Docs/Project/coordination/active-tasks.md` is the live ownership/lock authority outside the context snapshot. Workers must refresh it and their task claim before substantial edits and before acquiring the machine-wide Unity lock. GitHub state alone does not prove local cleanliness or Unity/runtime state.
