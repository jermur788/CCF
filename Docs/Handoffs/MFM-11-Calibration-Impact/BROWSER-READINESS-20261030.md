# MFM-11 — Calibration Impact Qualification — Browser Readiness

**Date:** 2026-10-30  
**Browser claim:** #182  
**Parent:** #91 / browser lane #162  
**Authority:** approved `specs/multiscale-forestry-management-v1/` MFM-11; requirements M1–M8  
**Status:** PREPARED — NOT RELEASED FOR LOCAL EXECUTION

## Purpose

Prepare the quantitative impact qualification required before Multi-Scale Forestry Management V1 can be considered safe for the Scenario One player-test candidate.

MFM-11 is not a tuning exercise by default. It is a qualification gate: measure the accepted wider-treatment path on one exact integrated candidate and either prove it remains within accepted Scenario One objectives/compatibility or return `BLOCKED — CALIBRATION IMPACT`.

## Release prerequisites

Do not release until:

1. MFM-09 is Manager-approved and integrated.
2. MFM-10 is green with exact token `SPECIES GUARD QUALIFIED` on the combined accepted lineage.
3. The current exact integration SHA is pinned and contains the accepted MFM, parity, Rack and relevant Scenario One support required by the measurement.
4. Live ownership is refreshed; no production retuning writer is implied by this qualification.
5. A new substantive local claim is created and marked `[RUNNING]` before local execution.

## Required comparisons

Run the approved comparison on the same authoritative initial/candidate context wherever technically valid:

1. **Historical sample-only path** — the accepted teaching/sample management without wider prescription application.
2. **Representative prescription path** — the accepted default MFM extension from demonstrated sample through coverage/candidates/proposals/prescription approval, ordinary Work Plan approval and execution.
3. **Plausible manual override path** — a bounded manual-management alternative allowed by the spec, chosen transparently rather than optimised using hidden future knowledge.

State any unavoidable fixture/branch differences explicitly. Do not present incomparable runs as controlled experiments.

## Required measurements

At minimum capture, by affected unit and whole stand where applicable:

- living removals / retained stems, with species split;
- basal area before and after intervention and over subsequent relevant cycles;
- retained/continuous canopy evidence using the authoritative model outputs available to Scenario One;
- cash/economy effects, contractor/landowner costs and timber revenue through the existing economy path;
- timber and deadwood outcomes;
- regeneration/light outcomes and any material side-effects on juveniles/protection/deadwood;
- completion/repeated-intervention reachability through the required Scenario One horizon;
- prescription-generated Pending/Approved/Executed provenance through Work Plan and Annual Review/history where relevant;
- protected compatibility/fingerprints required by accepted parity/save/Reference gates.

No unsupported ecological precision should be invented merely to fill a table. Label direct model outputs, derived metrics, gameplay calibration and unsupported/unavailable measures honestly.

## MFM-03 residual that must be resolved here

Manager closeout `Docs/Handoffs/MFM-Implementation-03/MANAGER-REVIEW.md` at `cae6e6018aa8a36a2c208dd1631765cadd8eb821` accepted the static calibration for MFM-04+ but explicitly carried the live/cross-year uncertainty into MFM-11.

Therefore MFM-11 must include a live-lineage check of the accepted coverage calibration in the actual extension timing/state, including the material parts of independent-review N1–N6:

- do not treat simple SE separation as independent-spatial statistical proof;
- confirm boundary units/tolerance behaviour is not being misrepresented as empirical precision;
- account for growth drift between the original sample state and the actual post-Lesson-5 extension state;
- observe the live state that static MFM-03 did not establish: natural cohorts/juveniles/shelters/deadwood/marks as applicable, whole-stand state and live competition-index parity;
- if those differences materially invalidate the accepted calibration or default extension outcome, return the blocker rather than silently retuning.

The accepted thresholds remain **GAMEPLAY CALIBRATION** unless a separate accepted decision changes them.

## Required safety assertions

- Each generated unit remains within the accepted basal-area safeguard.
- Wider treatment does not make retained/continuous-canopy objectives unattainable.
- Scenario One economy remains viable through the normal work/economy path; no hidden subsidy or bypass.
- Timber/deadwood outcomes remain coherent with accepted mechanics.
- Long-run Scenario One completion/repeated intervention remains achievable and understandable enough for later integrated qualification.
- Legacy40 / frozen Reference Future / protected Sitka anchors and save compatibility are preserved where governed.
- Oak/Beech are not silently quantified using Sitka-only treatment assumptions; MFM-10 species guard remains green.
- Tests are not weakened or protected anchors silently rebaselined to obtain a pass.

## Evidence / verification design

The local worker should prefer deterministic scripted comparison runs plus the smallest required Unity/runtime observations on the exact candidate. If full long-run measurement is available through authoritative verification harnesses, use them; if a metric only exists in runtime/Unity, serialize that phase normally.

Capture exact commands, seeds/fixtures, candidate SHA, save identities, intervention years, unit IDs, before/after outputs and result hashes sufficient for independent reproduction.

## Independent review

Approved routing requires **independent review by Claude** after the SOL/CODEX qualification result. The reviewer must inspect exact evidence and distinguish model-derived/gameplay-calibration conclusions from empirical claims.

The independent review is separate from implementation/measurement and must return APPROVED / NEEDS CHANGES / REJECTED for the MFM-11 evidence.

## Verdicts

Success: `MULTI-SCALE CALIBRATION QUALIFIED`

Blocking failure: `BLOCKED — CALIBRATION IMPACT`

A blocker does not authorise automatic retuning. Publish the measured failure and return for Manager/product disposition.

No Windows build.
