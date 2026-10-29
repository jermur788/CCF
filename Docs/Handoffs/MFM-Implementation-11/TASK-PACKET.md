# MFM-11 — Calibration Impact Qualification

**Parent:** #91  
**Spec:** `specs/multiscale-forestry-management-v1/` MFM-11  
**Status:** PREPARED / BLOCKED on completed representative-management and repeated-intervention flow  
**Purpose:** measure whether making wider representative treatment part of the default Scenario One teaching path materially changes accepted ecological/economic/completion outcomes.

## Difficulty and routing

- **Difficulty:** High — multi-path Unity measurement across economy, canopy, regeneration and long-run completion; no coefficient may be invented to force a pass.
- **Preferred executor:** Claude Sonnet 5.5 Medium / Anthropic T3 Code lane; escalate to Claude Opus 5.5 Low only if diagnosis becomes materially complex.
- **Mandatory independent reviewer:** OpenAI 6.1 Sol for cross-provider calibration-impact review.

## Start gate / base

Start only after MFM-10, PARITY-09 and the implemented S1-D repeated-intervention flow are accepted on one exact published candidate. S1-C history must be sufficient to compare the paths honestly. Record the exact base SHA before running.

Suggested branch/worktree: `qual/mfm-v1-calibration-impact-v1` / `/media/jer/Videos/CCF-MFM-11`.

Create executor/reviewer claims only when the gate is true.

## Compare three paths

On controlled comparable starts, measure:

1. **historical sample-only path**;
2. **representative prescription path** using the approved MFM flow;
3. **plausible manual override path** demonstrating the manual alternative.

Record by unit and whole stand where applicable:

- stems/removals and basal area before/after;
- accepted 40% per-unit safeguard;
- retained/continuous canopy;
- cash, work cost and timber income;
- deadwood/timber disposition;
- regeneration/light consequences;
- completion timing/reachability;
- solvency;
- protected compatibility/anchors.

Use the existing accepted calibration/economy/ecology models. Do not reinterpret a difference as a defect merely because the paths differ; judge whether it materially breaks accepted Scenario One outcomes.

## Stop condition

Emit `BLOCKED — CALIBRATION IMPACT` and return to the Manager if representative management materially breaks accepted canopy, economy, completion, compatibility or other milestone outcomes.

Do **not** tune thresholds/coefficients inside this qualification to make results green. Any calibration change requires a separate evidence/decision/implementation packet.

## Verification discipline

- exact seeds/scenario inputs and candidate SHA recorded;
- enough repeatability/determinism evidence to distinguish intended path differences from run noise;
- no protected anchor rebaseline;
- Reference Future v1 remains diagnostic/frozen;
- results distinguish direct measurements from gameplay calibration/interpretation;
- independent OpenAI 6.1 Sol review checks methodology, path comparability and conclusion.

No Windows build.

## Handoff

Publish `Docs/Handoffs/MFM-Implementation-11/HANDOFF.md` with path definitions, measurements, deltas, raw evidence references and conclusion.

Independent review: `Docs/Handoffs/MFM-Implementation-11/INDEPENDENT-REVIEW.md` with `APPROVED / NEEDS CHANGES / REJECTED`.

Success token: `MULTI-SCALE CALIBRATION QUALIFIED`

## Copy/paste dispatch line

`Run MFM-11 from Docs/Handoffs/MFM-Implementation-11/TASK-PACKET.md only after MFM-10, PARITY-09 and the implemented S1-D repeated-intervention flow are accepted on one exact published candidate. Use Claude Sonnet 5.5 Medium for the T3 measurement run and OpenAI 6.1 Sol for independent calibration-impact review. Compare sample-only, representative-prescription and plausible-manual-override paths across removals, BA/canopy, economy, deadwood/timber, regeneration/light, completion, solvency and protected anchors. Do not retune to pass; emit BLOCKED — CALIBRATION IMPACT if material accepted outcomes break. No Windows build.`

**User action required now:** none; dependency-gated.
