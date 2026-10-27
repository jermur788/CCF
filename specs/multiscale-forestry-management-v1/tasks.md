# MULTI-SCALE FORESTRY MANAGEMENT V1 — Tasks

**Spec:** `MULTI-SCALE-FORESTRY-MANAGEMENT-V1`

Each task is bounded. A task's PASS authorises only the next listed gate, not the entire feature.

## MFM-00 — Coordination and authority sync

**Routing:** FREE / Overall Manager / authorised single writer  
**Goal:** record U1–U5 and P1–P4, reference this spec from milestone/context, and publish dashboard state when safe.

### Scope

- Decision Log entry/entries, using next available IDs.
- Explicit amendment relationship to old D-010 spatial wording.
- Relationship to Rack V2/D-064 map exception.
- Current milestone: representative-management slice is pre-external-test.
- Research index: delegated-treatment evidence audit is complete; primary-source spot checks support the framing with qualifications. Record the research result and wording boundaries when the authorised single writer is available.

### Exclusions

Do not touch generated dashboard state manually. Do not overwrite dirty coordination files owned by another lane.

### Gate

`COORDINATION SYNCED — SPEC AUTHORITATIVE`

---

## MFM-01 — Prerequisite and latest-lineage qualification

**Routing:** SOL/CODEX  
**Mode:** read-only qualification

### Goal

Confirm implementation truth after Rack V2 core integration.

### Verify

- persisted Rack V2 geometry exists and is stable;
- world identity/old-save policy is qualified;
- final rack headland/layout can support the representative strip;
- current save schema version;
- current Scenario One teaching sequence and sample code;
- current map/Work Plan ownership;
- no conflicting writer/process.

### Stop

`BLOCKED — RACK V2 PREREQUISITE`

if persisted rack geometry is not yet integration-ready.

### Gate

`MFM IMPLEMENTATION BASE QUALIFIED`

---

## MFM-02 — Rack-served sample and coverage geometry adapter

**Routing:** SOL/CODEX  
**Goal:** re-point the existing teaching footprint to confirmed Rack V2 geometry and derive rack-served coverage units.

### Requirements

- preserve sample teaching intent;
- representative teaching rack = third confirmed rack ordered along the common normal;
- sample anchor = closest point on teaching rack centreline to stand centre, clamped for ±12 m length;
- fixed teaching footprint half-width = 9.0 m;
- preserve 8 m teaching Crop Tree spacing and separate 8 m Hegyi neighbourhood;
- preserve rack corridor exclusion from actual confirmed rack geometry;
- derive half-distance rack-served **coverage** boundaries separately from the fixed teaching footprint;
- define outer remainders explicitly;
- no prescription logic yet.

### Verification

- existing Lessons 2–5 behaviour remains valid;
- live Unity confirms the MFM-01 offline geometry result;
- geometry/rendered inspection;
- historical 69-tree / 1.29490 m² fixtures are relabelled or updated only with explicit qualification, not treated as protected product invariants.

### Gate

`RACK-SERVED SAMPLE GEOMETRY QUALIFIED`

---

## MFM-03 — Coverage calibration measurement

**Routing:** SOL/CODEX or DEEP RESEARCH for evidence-only subwork; implementation/calibration remains SOL/CODEX  
**Goal:** measure Scenario One unit variation and qualify V1 coverage metrics/tolerances.

### Measure

- stem count/density;
- mean DBH;
- mean competition;
- known former-edge dense band;
- outer-lattice/remainder behaviour;
- regeneration/juvenile conflicts;
- access/clearing exclusions;
- dedicated Crop Tree candidate-spacing options and resulting confirmation workload.

### Candidate thresholds

Historical Stage C values may be tested but are not assumed correct.

### Output

- chosen metrics;
- chosen tolerances;
- classification: GAMEPLAY CALIBRATION;
- sensitivity analysis;
- examples of covered/not-covered units.

### Gate

`COVERAGE CALIBRATION QUALIFIED`

---

## MFM-04 — Deterministic prescription core

**Routing:** SOL/CODEX; DeepSeek may implement only if scope is kept pure-code and verification is straightforward  
**Goal:** implement plain deterministic derivation, fit, coverage and proposal-domain logic without UI or work-order mutation.

### Must reuse

- authoritative CompetitionShare/Hegyi implementation;
- accepted sample state.

### Must not implement

- hidden quality ranking;
- broadleaf quantitative release;
- Work Plan mutation;
- TreeMark Fell proposals.

### Verification

Focused offline tests for determinism, fit disagreements, safeguard and coverage.

### Gate

`PRESCRIPTION CORE QUALIFIED`

---

## MFM-05 — Crop Tree candidate system

**Routing:** SOL/CODEX core + Terra UI if cleanly separable  
**Goal:** generate spacing-only Sitka-adult candidates and require player confirmation/rejection.

### Requirements

- spacing-only semantics;
- deterministic candidate ordering;
- candidate copy states that spacing identifies where to inspect; the player judges suitability in 3D;
- no automatic stem-form/branching/vigour quality selector in V1;
- manual override;
- explicit non-quality wording;
- no Oak/Beech auto candidates before parity;
- player confirmation grounded in 3D inspection.

### Verification

- no candidate becomes Crop Tree automatically;
- confirm/reject survives navigation and required persistence semantics;
- conflicts with existing marks/orders handled safely.

### Gate

`CROP TREE CANDIDATE FLOW QUALIFIED`

---

## MFM-06 — Save / provenance foundation

**Routing:** SOL/CODEX  
**Goal:** add the next available save schema for prescription state and durable provenance.

### Persist

- stable prescription IDs;
- rule/version/source sample;
- accepted player coverage decisions;
- approval state/year;
- work-order provenance;
- history linkage required by spec.

### Compatibility

- missing fields = no prescription;
- no migration from existing marks/orders;
- Reference Future untouched;
- legacy/pre-feature saves preserved.

### Stop

If protected compatibility requires silent rebaselining.

### Gate

`PRESCRIPTION SAVE FOUNDATION QUALIFIED`

---

## MFM-07 — Proposal state and approval boundary

**Routing:** SOL/CODEX  
**Goal:** implement separate proposal state and prescription approval.

### Critical invariants

- proposals are not `TreeMarkType.Fell`;
- opening/approving Work Plan cannot import unapproved proposals;
- prescription approval creates pending ordinary work with provenance;
- Work Plan approval remains required afterward;
- manual player state wins conflicts.

### Verification

Explicit negative tests for Work Plan bypass/leakage.

### Gate

`PRESCRIPTION APPROVAL BOUNDARY QUALIFIED`

---

## MFM-08 — Map / 3D review UX

**Routing:** Terra for bounded UI/reference-driven implementation; SOL/CODEX for integration logic as needed  
**Goal:** make sample, coverage, candidates and proposals legible without implying commitment.

### Map

- source sample;
- coverage states;
- player-facing terminology uses comparable-area/sample-treatment wording rather than presenting `operator selection` as formal Irish terminology;
- candidates;
- proposals;
- reasons/exclusions.

### 3D

- navigate/inspect candidate;
- confirm/reject;
- inspect proposal reason;
- required extrapolated-unit spot check.

### Visual rule

Candidate/proposal presentation must be clearly distinct from Red Fell, Blue Crop Tree and Orange Rack marks.

### Verification

Rendered 720p/1080p review plus interaction qualification.

### Gate

`MULTI-SCALE REVIEW UX QUALIFIED`

---

## MFM-09 — Teaching sequence / Work Plan integration

**Routing:** SOL/CODEX integration; Terra for copy/layout-only follow-up  
**Goal:** insert `Extend your treatment` after existing Lesson 5 and connect approved prescription work to normal Work Plan flow.

### Requirements

- stable existing step IDs preserved;
- new stable IDs added;
- existing Lessons 2–5 behaviour unchanged;
- visible numbering may change, saved keys do not;
- Work Plan groups/explains prescription-generated pending work;
- normal contractor/landowner/economy path used.

### Gate

`SCENARIO ONE EXTENSION FLOW QUALIFIED`

---

## MFM-10 — Species-parity integration guard

**Routing:** SOL/CODEX  
**Goal:** verify the extension remains honest after the accepted Sitka/Oak/Beech parity work lands.

### Required

- Sitka proposal behaviour unchanged unless explicitly revised;
- Oak/Beech not accidentally processed by Sitka-only rules;
- exclusions/reasons accurate;
- if multi-species treatment is desired, STOP and create spec amendment rather than silently adding it.

### Gate

`SPECIES GUARD QUALIFIED`

---

## MFM-11 — Calibration impact qualification

**Routing:** SOL/CODEX; independent review by CLAUDE VALUE  
**Goal:** prove that making wider treatment part of the default teaching flow does not break Scenario One.

### Measure

- removals by unit and whole stand;
- basal area before/after;
- retained/continuous canopy;
- cash/economy;
- deadwood/timber outcomes;
- regeneration/light effects;
- long-run completion;
- protected historical compatibility.

### Compare

- historical sample-only path;
- representative prescription path;
- plausible manual override.

### Stop

`BLOCKED — CALIBRATION IMPACT`

if wider application materially breaks accepted Scenario One outcomes.

### Gate

`MULTI-SCALE CALIBRATION QUALIFIED`

---

## MFM-12 — Integrated verification and independent review

**Routing:** SOL/CODEX integration + CLAUDE VALUE independent review  
**Goal:** qualify spec conformance end-to-end.

### Verify

- requirements checklist;
- save/load;
- map/3D;
- Work Plan boundary;
- old saves;
- full Scenario One completion;
- rendered visuals;
- provenance/history;
- no hidden automation replacing player judgement.

### Verdict

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

Only this gate authorises the user-playtest candidate.

---

## MFM-13 — User playtest and external-test gate

**Owner:** user + Overall Manager  
**Goal:** user personally tests the integrated Scenario One candidate.

### Observe separately

- whether candidate Crop Trees feel like suggestions rather than answers;
- whether coverage/not-covered logic is understandable;
- whether the sample genuinely teaches the later prescription;
- whether spot-checking feels meaningful rather than ceremonial;
- whether prescription approval vs Work Plan approval is clear;
- whether manual alternative remains usable.

### External-test rule

No beginner/external Scenario One test until the integrated candidate is accepted under the project's existing external-demo gates.

No Windows build before the user's explicit build authorisation under the existing project rule.

