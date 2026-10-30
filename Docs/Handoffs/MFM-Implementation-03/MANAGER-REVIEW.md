# MFM-03 — Coverage Calibration Measurement — Manager Review

**Date:** 2026-10-30  
**Task / claim:** MFM-03 / #39  
**Authority:** approved `specs/multiscale-forestry-management-v1/`, D-065  
**Worker handoff:** `Docs/Handoffs/MFM-Implementation-03/HANDOFF.md` on exact worker tip `60792dc0a70a26af6d43603d9992bc477b9782f5`  
**Independent review:** `Docs/Handoffs/MFM-Implementation-03/INDEPENDENT-REVIEW.md` at `73356e3fa7f9d9b5f2ffc50a7623492559d2f5c5`

## Verdict

`APPROVED — STATIC CALIBRATION ACCEPTED FOR MFM-04+; LIVE/CROSS-YEAR IMPACT RESIDUAL CARRIED TO MFM-11`

This review closes the stale execution claim. It does **not** claim a new Unity run, a live post-Lesson-5 calibration qualification, or a full Multi-Scale Forestry Management V1 calibration-impact pass.

## Accepted result

The independent review reproduced the committed MFM-03 tooling and frozen inputs byte-for-byte, verified the MFM-02 fixture identity used by the measurement, exercised negative controls, found no blocking defect, and returned the exact verdict `MFM-03 CALIBRATION REVIEW PASS — ACCEPT FOR MFM-04`.

The following remain accepted as **GAMEPLAY CALIBRATION** inputs for the deterministic MFM pipeline, rather than empirical Irish/UK forestry thresholds:

- anchor-aligned thirds inside the existing MFM-02 rack-served units;
- density tolerance ±20%;
- mean DBH tolerance ±15%;
- mean authoritative Hegyi competition-index tolerance ±20%;
- minimum 30 stems per compared unit;
- no P90 tail guard;
- spacing-only candidate spacing of 16 m using the qualified deterministic position-only ordering.

These values were already consumed by the subsequent MFM-04+ implementation programme. This closeout does not reopen or retune them.

## Evidence classification and caveats

The independent review's non-blocking findings N1–N6 remain binding context. In particular:

- the reported standard-error separation is only an indicative model derivation because spatial autocorrelation makes the simple SE optimistic;
- the 20% competition tolerance is a defensible gameplay calibration within the measured fixture gap, not a statistically established forestry threshold;
- the clean-interior-window argument is limited by overlapping windows and a small number of qualifying windows;
- the original negative-control transcript's recorded `rc=0` values are superseded by the independent rerun, which correctly observed failing controls with return code 1;
- the pre-access variant wording must be understood as pre-access competition values over post-access stem/area sets;
- most importantly, the offline same-year fixture did **not** establish cross-year robustness at the live post-Lesson-5 state. Growth drift, competition-dependent growth/mortality, natural cohorts/juveniles/shelters/deadwood/marks, whole-stand live state, and live competition-index parity were not established by MFM-03.

No player-facing or design document should upgrade these calibrations into stronger empirical claims.

## Disposition of the live residual

MFM-03's remaining live/cross-year uncertainty is not a blocker to the already-completed deterministic core work. It is a required input to **MFM-11 — Calibration impact qualification**, whose approved task explicitly measures whole-stand treatment impact, canopy, economy, timber/deadwood, regeneration/light, long-run completion and protected compatibility.

MFM-11 must therefore include an exact live-lineage check that the accepted coverage calibration remains reasonable in the actual post-Lesson-5 / extension state. If that check materially fails, use the approved blocker `BLOCKED — CALIBRATION IMPACT`; do not silently retune thresholds or weaken tests.

## Scope / safety

- No production files are changed by this Manager review.
- No save/schema/reference/hash baseline is changed.
- Frozen Reference Future v1 is untouched.
- No Unity/runtime/rendered claim is made here.
- No Windows build is authorised or performed.

## Claim disposition

Claim #39 may close as completed because its static/offline calibration task and independent review are durably complete and were accepted as the versioned calibration input for MFM-04+. The remaining live impact question is tracked by the later approved MFM-11 gate rather than by leaving the historical execution claim falsely RUNNING.
