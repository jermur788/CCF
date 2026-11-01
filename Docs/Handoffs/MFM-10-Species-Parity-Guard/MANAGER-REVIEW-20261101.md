# MFM-10 Species-Parity Guard — Manager Review — 2026-11-01

**Claim reviewed:** #213  
**Browser prerequisite:** #212  
**Local evidence commit:** `43e263708d8f517337181c92d0d78ed442803950`  
**Qualified production candidate:** `6784ab17cac922a16815f4898a30737482101018`  
**Verdict:** **APPROVED — `SPECIES GUARD QUALIFIED`**

## Decision

The combined browser/static and local executable evidence satisfies MFM-10.

Command 1 passed exactly as required: `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0` with byte-identical repeat. This directly confirms the accepted MFM V1 fail-closed species boundary: non-Sitka samples/areas are refused, broadleaf Crop Trees receive no Sitka-calibrated release proposal, and the pure proposal core remains deterministic.

Command 2 returned exit 1, but the raw result is accepted for **MFM-10 scope only** because every MFM-10-relevant Oak/Beech/mixed routing and arithmetic-isolation assertion passed and the only red families are separately attributable:

1. `MaxRemovedBasalShare` is a removal safeguard identifier. The source scanner flags any identifier containing `max`/`maximum` plus `basal`/`sdi`; that heuristic therefore catches a prescription removal-share limit even though it is not a species maximum density, maximum basal area, SDI ceiling or broadleaf capacity claim. The raw scanner failure remains recorded; the verifier is not being relabelled as globally green.
2. The three pure-Beech reduced-simulator R2 misses (`0.423`, `0.407`, `0.417` against unchanged `0.40`) are already recorded by project verification authority as **ACCEPTED KNOWN C-LEVEL LIMITATION**. They remain non-passes in evidence; no threshold, calibration, production value or fixture is changed or waived.

The browser handoff's strict `exit 0` completion wording was therefore over-constrained relative to already-accepted verification state. This review supersedes that completion wording for MFM-10 only. It does **not** change the ThreeSpecies suite, weaken a test, rebaseline a result, or convert the known failures into passes.

## Requirement verdict

- **10a — unsupported species safely limited:** PASS.
- **10b — no Sitka-only coefficient silently applied to Oak/Beech:** PASS.
- **10c — mixed-species tests prove accepted species-aware density while unsupported MFM calibration is refused:** PASS for the MFM-10 contract; raw unrelated/accepted red families remain carried in evidence.
- **Unity/runtime:** no MFM-10-specific Unity run required; no contradictory runtime-only species branch was found.
- **Code/spec mismatch:** none.

Exact gate token:

`SPECIES GUARD QUALIFIED`

## Carry-forward after #199 integration

#199 was reviewed separately and its tested production delta from `6784ab17...` touches only Scenario One UI presentation plus the already-accepted Combined verification fixture. It does not modify MFM prescription/coverage/calibration, species assets, mixed-density arithmetic or the MFM-10 guard paths. Therefore this MFM-10 qualification remains applicable when the Scenario One integration branch advances to #199 production commit `3acdf9cb635cdca79606335801e97f9d7dcce8b0`.

## Residuals

- Keep the ThreeSpecies scanner false-positive behaviour visible as verification debt; do not claim that suite is globally green.
- Keep the three Beech R2 misses visible as accepted C-level limitations with the `0.40` criterion unchanged.
- Any future multi-species quantitative MFM treatment still requires a spec amendment; this review does not authorise Oak/Beech MFM coefficients.

**Manager disposition:** #213 may close completed. PARITY-09 and MFM-11 may now be freshly repinned/released on the current integrated lineage. No Windows build. Research Source Consolidation #194 remains NO ACTION.
