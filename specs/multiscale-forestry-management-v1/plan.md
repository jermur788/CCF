# MULTI-SCALE FORESTRY MANAGEMENT V1 — Technical Plan

**Spec:** `MULTI-SCALE-FORESTRY-MANAGEMENT-V1`  
**Status:** implementation plan; production coding begins only after prerequisites pass.

## 1. Implementation strategy

Implement the feature as a sequence of bounded layers. Do not build UI first and do not mix Rack V2 state work with prescription state work in one uncontrolled change.

The core architecture is:

`authoritative Rack V2 geometry`
→ `existing qualified sample`
→ `plain deterministic prescription/coverage core`
→ `separate proposal state`
→ `player Crop Tree confirmations`
→ `prescription approval`
→ `ordinary work orders with provenance`
→ `existing Work Plan / execution`
→ `history/review`

## 2. Required pre-implementation inspection

Before changing production code, inspect the latest integrated Scenario One candidate and confirm:

- Rack V2 save/world-identity version and runtime types;
- final rack-plan geometry representation;
- current `ScenarioOneRackBay`/reader implementation after Rack V2;
- current Work Plan import/approval behaviour;
- current save schema version;
- current teaching sequence IDs;
- species-parity state;
- map/minimap ownership and any active single-writer lanes.

Do not implement from the historical `089dbeb` paths alone if production has moved.

## 3. Layer A — geometry/sample adapter

### Goal

Re-point the qualified teaching sample from historical authored rack lines to confirmed Rack V2 rack objects without altering Lessons 2–5 behaviour.

### Requirements

- Identify the representative teaching rack as the third confirmed rack ordered by perpendicular offset along the common rack normal.
- Anchor the sample at the point on that rack centreline closest to stand centre, clamped to preserve the ±12 m length on the segment.
- Use fixed `TeachingHalfWidthM = 9.0` and `HalfLengthM = 12.0`; do not resize the sample from neighbour spacing.
- Preserve the rack corridor exclusion using actual confirmed rack segments.
- Preserve 8 m teaching Crop Tree minimum spacing and the separate 8 m Hegyi competition neighbourhood.
- Produce rack-served coverage strips separately from the fixed teaching sample.
- Handle outer-edge remainder explicitly.

### Verification

MFM-01 has qualified this rule offline. Confirm it live in Unity. Preserve the sample lesson anchors/intent, but treat historical exact tree identities/counts and 1.29490 m² as reference-layout fixtures rather than protected invariants. Any new live fixture values must be explicitly qualified rather than silently rebaselined.

## 4. Layer B — prescription domain model

Prefer plain .NET/domain logic with minimal Unity dependencies.

Suggested conceptual types (names provisional; inspect repository conventions first):

- `ScenarioOnePrescription`
- `PrescriptionRuleVersion`
- `PrescriptionSampleSummary`
- `PrescriptionCoverageUnit`
- `PrescriptionCoverageReason`
- `PrescriptionProposal`
- `PrescriptionReasonCode`
- `CropTreeCandidate`

### Domain responsibilities

- derive rule parameters from executed sample decisions;
- reproduce the rule against the sample for fit analysis;
- classify coverage units;
- generate spacing-only Crop Tree candidates;
- generate release proposals only around player-confirmed Crop Trees;
- enforce per-unit 40% ceiling;
- remain deterministic.

Do not let this layer mutate work orders directly.

## 5. Prescription derivation

For V1, derive only values supported by current mechanics.

Candidate inputs include:

- executed sample removals;
- authoritative Crop Tree identities;
- `CompetitionShare` outputs;
- achieved minimum release Share;
- sample basal-area removal share;
- current sold/deadwood disposition where supported.

Do not infer quality, dominance or hidden biological values.

The prescription's release target must never be below the existing teaching minimum when the sample has passed that lesson.

The per-unit removal ceiling is the lower of the sample-derived removal share and the accepted 40% ceiling, subject to later calibration qualification.

## 6. Fit calculation

Implement a deterministic replay of the derived rule against the original sample.

Expose:

- selected/proposed tree agreement;
- disagreement identities/reasons;
- relevant removal/Share summary.

Do not collapse this to an opaque percentage if the player cannot inspect the mismatches.

## 7. Coverage classification

### Inputs

Use observable/current-state metrics only.

Start with measures already available and inspectable in the current model, such as:

- living stem count/density;
- mean DBH;
- mean competition index;
- species mix;
- regeneration/juvenile state;
- management conflicts;
- access/rack geometry.

### Calibration task

Measure the current Scenario One stand and compare candidate thresholds, including the historical Stage C 20% / 15% / 20% values as test candidates only.

Choose production thresholds by gameplay qualification, label them GAMEPLAY CALIBRATION and record why they are adequate for Scenario One.

No threshold becomes production authority merely because it appeared in an old design note.

## 8. Crop Tree candidate generation

### V1 rule

Generate only spacing-based candidate suggestions among eligible Sitka adults in covered units.

Requirements:

- deterministic ordering/tie-breaking;
- no unsupported quality ranking;
- no automatic Crop Tree designation;
- no Oak/Beech quantitative candidate generation before parity;
- respect player marks/designations/open work;
- explain candidate reason as spacing/structural suggestion only; spacing identifies where to inspect, while the player judges suitability in 3D;
- do not encode automatic form/branching/vigour quality judgement in V1;
- qualify a dedicated candidate-spacing parameter; do not reuse the 8 m teaching spacing or 8 m Hegyi radius by implication.

### Confirmation UX

The player must inspect/confirm or reject candidates. Prefer 3D confirmation with map navigation/summary support. Provide a deterministic candidate queue with Next/Previous and map focus or equivalent. Do not bulk-confirm candidates in V1. Candidate workload must be measured and user-tested; excessive tedium is a stop-for-product-decision condition rather than permission to automate judgement.

Confirmed candidates become ordinary durable Crop Tree state using the existing authoritative Crop Tree mechanism where possible; avoid a second competing designation system.

## 9. Proposal state

Create a distinct proposal representation.

It must not be `TreeMarkType.Fell` and must not be automatically imported by the existing Work Plan.

Proposal state should support:

- tree identity;
- prescription ID;
- coverage-unit ID;
- confirmed Crop Tree link/reason where relevant;
- reason code;
- proposed disposition where applicable;
- preview only until prescription approval.

Prefer recomputation of transient proposals from persisted prescription + confirmed current state where safe. Persist only data required for durable player choices/history.

## 10. Prescription approval

On prescription approval:

1. Revalidate world/rack/sample state.
2. Revalidate coverage and conflicts.
3. Revalidate the per-unit basal-area ceiling.
4. Convert accepted proposals into ordinary pending work orders.
5. Attach prescription provenance to each generated order.
6. Record prescription approval/year/history event.
7. Clear/reconcile transient proposal presentation.

Do not approve the resulting work orders automatically.

The existing Work Plan remains the second approval/commitment step.

## 11. Persistence

Rack V2 is expected to introduce save v21 first. This feature should use the **next available schema version** after inspection; do not hard-code v22 in planning as an authority if another schema change lands first.

Likely persistent structures:

- `nextPrescriptionId`;
- prescription records;
- source sample/rack/year;
- rule version;
- accepted player coverage overrides/exclusions;
- approved state/year;
- provenance ID on generated work orders;
- management event/history linkage.

Potentially persistent only if required by final product behaviour:

- confirmed candidate decisions not already represented by Crop Tree state;
- review inputs used to derive later prescriptions.

Old/missing data = no prescription state. No migration from existing Fell marks/orders into fabricated prescriptions.

## 12. Work Plan integration

Inspect the current automatic Fell-mark import path carefully.

Required invariant:

**Unapproved prescription proposals can never enter pending/approved work through opening or approving the Work Plan.**

After prescription approval, generated work is ordinary pending work and appears in the Work Plan with grouping/provenance sufficient to explain that it came from a prescription.

Do not invent per-prescription execution semantics if global approval already handles the work safely.

## 13. UI integration

### Map

Add layers/representation for:

- source sample;
- coverage status;
- candidate Crop Trees;
- proposed removals;
- excluded/manual areas.

Do not visually conflate proposed work with committed Red Fell marks.

### 3D

Support:

- navigation to candidates/proposals;
- candidate confirm/reject;
- inspect "why proposed?";
- required spot-check of an extrapolated unit.

### Work Plan

Group prescription-generated pending work clearly while preserving normal approval/cost/execution behaviour.

### Teaching

Add new stable step IDs after Lesson 5. Do not rename/reuse existing persisted teaching keys.

## 14. History and provenance

Extend work/history data only as much as needed to retain:

- prescription identity;
- source sample;
- approval/execution year;
- generated work linkage;
- reason/provenance necessary for later inspection/history.

Do not claim S1-C/S1-D complete merely because these hooks exist.

## 15. Calibration-impact qualification

Before enabling the extension as the default required Scenario One flow, run an explicit calibration qualification covering:

- total removals generated;
- per-unit basal-area removal;
- stand basal area before/after;
- retained-canopy/continuous-cover objectives;
- cash flow, contractor minimum and timber/deadwood outcomes;
- regeneration/light consequences;
- long-run completion;
- protected fingerprints/hashes where applicable.

Compare:

- sample-only historical flow;
- representative default prescription flow;
- reasonable manual-override variants.

If the extension materially destabilizes Scenario One, fix through bounded gameplay calibration or stop for a genuine design decision. Do not weaken safety gates.

## 16. Species parity integration

The core prescription architecture must be species-aware in its data model even if V1 quantitative proposal generation is Sitka-only.

Until accepted Oak/Beech parity exists:

- broadleaf adult units/trees are excluded with a visible reason;
- no Sitka coefficients are silently transferred;
- no species-preference rule is presented as implemented.

After parity, add multi-species behaviour through an explicit spec/model version amendment.

## 17. Verification strategy

Use layered verification rather than one giant gate.

### Offline/domain

- deterministic derivation;
- deterministic fit;
- coverage classification;
- candidate spacing and tie-breaks;
- proposal generation;
- safeguards;
- provenance.

### Save/compatibility

- new-state round trip;
- old save missing fields;
- malformed data rejection;
- no fabricated prescription;
- Reference/Legacy compatibility.

### Unity/runtime

- teaching sequence insertion;
- map/3D presentation;
- candidate confirmation;
- spot-check requirement;
- approval boundary;
- Work Plan integration.

### Integrated simulation

- economy/canopy/calibration impact;
- completion path;
- save/load continuation;
- later review/history.

### Visual

Rendered review required for all new map/3D proposal/candidate states at supported UI resolutions.

## 18. Ownership / sequencing

Recommended lane sequence:

1. Rack V2 persistence/runtime/headland integration finishes.
2. Geometry/sample adapter qualification.
3. Plain prescription/coverage domain core.
4. Save/provenance layer.
5. Candidate confirmation + proposal state.
6. Map/3D UX.
7. Work Plan/teaching integration.
8. Calibration-impact qualification.
9. Independent review.
10. Full Scenario One integration + user playtest.

Serialize writers for high-conflict files such as ScenarioOneManager, save classes, StandMapView/map model and shared UI USS.



## Evidence-informed terminology

Use `apply your sample treatment to comparable areas` or equivalent in player-facing copy. Treat `delegated marking` as internal shorthand. Do not use `operator selection` as an established Irish technical term. Do not imply a universal contractor-marking workflow.

Coverage/recalibration can cite homogeneous/comparable-condition practice as supporting evidence, but exact coverage metrics/tolerances remain GAMEPLAY CALIBRATION. UK feller-select cab-visibility limits are a design caution: V1 keeps proposal generation and player review separate rather than claiming machine operators can always assess crown-release choices directly.
