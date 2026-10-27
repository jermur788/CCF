# MANAGER REVIEW — Scenario One Three-Species Parity P0 / PARITY-02

**Date:** 2026-10-27  
**Reviewer:** Overall Manager (ChatGPT Game Dev)  
**Evidence branch:** `research/scenario-one-three-species-parity-p0-evidence-closure-v1`  
**Reviewed HEAD:** `3142aef`  
**Context basis:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`, superseded for current canonical authority by Git after S1-C/S1-D/parity approvals

## Verdict

**PARITY-02 ACCEPTED — SPECIES PARAMETER MATRIX ACCEPTED.**

The evidence closure is accepted as the P0 evidence/parameter policy for Scenario One three-species parity, subject to the implementation/qualification gates in the approved spec.

No candidate-lineage value is accepted as empirically validated merely because it exists in code. Candidate values retained below remain labelled `GAMEPLAY CALIBRATION`, `ORDINAL REPRESENTATION`, `TRANSFER`, or other stated classes.

## Resolved Manager decisions

### U-01 — Oak vs Beech growing-space class ordering

**Decision: retain Oak = High and Beech = Intermediate for P0, explicitly as a weak provisional ordinal representation.**

Rationale: the architecture is already implemented and testable; accepted evidence supports only ordinal species-aware differentiation, not quantitative broadleaf density equations. The Oak > Beech ordering is not presented as empirical truth. Requalification must demonstrate stable gameplay behaviour and no pure-Sitka regression. A future evidence packet may collapse or reverse the ordering without redesigning the architecture.

Accepted calibrated weights remain:

- Sitka / Reference = 1.0
- Beech / Intermediate = 1.5
- Oak / High = 2.0

These weights are `GAMEPLAY CALIBRATION`; the class ordering is `ORDINAL REPRESENTATION`.

### U-11 — Beech provisional height-band miss

**Decision: accept as a labelled calibration limitation for P0. Do not retune.**

The current Beech adult values remain the best accepted calibration candidate. The provisional height-band miss is not treated as empirical failure and must not be hidden. Any later retuning requires a new evidence/calibration packet and the applicable measurement-integrity gate.

### U-12 — premium Oak reachability / teaching

**Decision: premium Oak may be explained as a possible late, low-competition, pruning-dependent outcome; it is not a Scenario One target, promise, or completion requirement.**

Do not lower thresholds or accelerate Oak growth to make premium timber reliably reachable. Player-facing wording must say that premium size may be reached late under favourable simulated conditions and is not guaranteed within the scenario horizon.

## Accepted P0 deferrals / limitations

The following are explicitly outside P0 and must not be invented to close parity:

- formative shaping and persisted fork/form state;
- species attribution in timber-sale records while selected shared-class prices remain equal;
- species-specific numerical site multipliers;
- species canopy transmissivity / deciduous-season coefficients;
- species-specific wind multipliers or storm activation;
- broadleaf harvest-cost differentiation absent evidence;
- measured Oak/Beech green density — 890 kg/m3 remains a work-equivalent proxy only;
- Beech-specific survival/establishment curves beyond the labelled combined fallback.

## Binding implementation decisions

Accepted from PARITY-02:

- adopt the existing three-species lineage as the implementation baseline; requalify and gap-close rather than rebuild greenfield;
- pure Sitka remains the exact protected path;
- mixed density uses the inherited component architecture with species-neutral victim selection;
- `Ci50` remains a growth/crown/suppression trait and does not drive density victim bias;
- Oak/Beech adult candidate values remain labelled gameplay calibration;
- site response remains neutral 1.0 and dormant in P0;
- canopy/light mechanics remain shared except existing crown geometry/relaxation differences;
- Oak distinct juvenile responses stay; Beech combined juvenile response stays as a conservative labelled fallback;
- existing browsing ordering/escape-height policy stays with transfer/calibration labels;
- Oak five-rung and Beech two-rung timber ladders stay;
- timber quality is size + pruning/crown history only; no claim of modelled fork/defect quality;
- Teagasc quotations are `ManagementGuidance`; selected reference prices and roadside basis are gameplay calibration;
- dormant flat broadleaf value 6,500 c/m3 is prohibited and must be removed/blocked;
- broadleaf pruning uses a separate two-lift high-pruning branch at 3.5 m and 6.0 m, minimum 3 years apart; Sitka pruning stays exact;
- new Scenario One games may use `GrowthModel.SpeciesAware = 2`; old saves retain models 0/1; no parity-specific save field;
- unknown saved species must be rejected, not silently resolved to Sitka;
- wind UI must state storms are off/not modelled rather than imply a species-calibrated wind band;
- Multi-Scale Forestry Management remains Sitka-adult-only unless separately amended.

## Evidence vocabulary

The project research classes are authoritative:

`DIRECT`, `TRANSFER`, `MODEL DERIVATION`, `ORDINAL REPRESENTATION`, `GAMEPLAY CALIBRATION`, `FALLBACK-CONSERVATIVE`, `UNSUPPORTED — NEEDS DECISION`.

Runtime/UI labels are only implementation labels and must map explicitly to the research class. `UNSUPPORTED — NEEDS DECISION` authorises no runtime value.

## Anchor policy

Protected exact anchors include pure-Sitka lifecycle/density, Legacy40, Sitka timber and frozen Reference archive integrity. Broadleaf-containing model-2 completion/economy/start anchors are authorised-change candidates only when measured, explained and explicitly accepted; they are not silent regressions. Reference continuation remains diagnostic under D-042.

## Spec decision

The parity spec may now move from `DRAFT` to `APPROVED` after incorporating PARITY-01 corrections C-01…C-12 and PARITY-02 corrections C-13…C-32 plus the three decisions above.

Production parity work remains subject to task dependencies and live ownership. Shared species YAML/scene surfaces stay frozen while Rack V2 core integration is active.

## Next work

First production task: **PARITY-03 — mixed-density candidate requalification and bounded gap closure**, using the inherited model-2 implementation. It must not edit shared species YAML in its first slice.

`DASHBOARD SYNC: NOT AVAILABLE`  
`Published dashboard reflects this review: NO`
