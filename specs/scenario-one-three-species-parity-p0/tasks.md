# Scenario One Three-Species Parity P0 Bounded Tasks

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Spec status:** APPROVED
- **Owner:** Overall Manager
- **Date:** 2026-10-27

Implementation tasks must stay inside their stated scope. Existing inherited code is a candidate baseline, not automatic acceptance.

## PARITY-01 — Current implementation audit

- **Status:** COMPLETE
- **Result:** `CURRENT SPECIES IMPLEMENTATION MAPPED`
- **Handoff:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Audit/HANDOFF.md`

## PARITY-02 — Evidence and parameter closure

- **Status:** COMPLETE / MANAGER ACCEPTED
- **Result:** `SPECIES PARAMETER MATRIX ACCEPTED`
- **Handoff:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md`
- **Manager review:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/MANAGER-REVIEW.md`

## PARITY-03 — Mixed-density candidate requalification and gap closure

- **Status:** VERIFIED — Unity G2/G3 accepted; independent static review #9 remains open
- **Goal:** Requalify the inherited model-2 mixed-density implementation against FR-1/FR-2; patch only demonstrated conformance defects.
- **Scope:** mixed-density source + focused verification; no species YAML/scene edits in the first slice.
- **Required checks:** exact pure-Sitka models 0/1 and model-2 route; living-tree route definition; Reference/Intermediate/High classes; species-neutral victim selection under the model-2 mixed/species-aware route; no new `Ci50` mortality coupling in that route; near-pure route continuity; no DEC/DMC/mixture bonus/broadleaf density equation. Models 0/1 retain legacy behaviour unchanged.
- **Owner / routing:** DeepSeek implementation/qualification when available; Claude independent review. Static/read-only review may proceed earlier if useful.
- **Dependencies:** PARITY-01, PARITY-02, approved spec; live ownership check.
- **Stop:** any protected pure-Sitka change, unsupported retune, or live file overlap.
- **Exit:** `MIXED DENSITY PARITY REQUALIFIED`.

## PARITY-04 — Adult/species-data requalification

- **Status:** VERIFIED — adult Unity G2/G3 accepted
- **Goal:** Requalify accepted Oak/Beech adult values/classes and neutral site/canopy policy; correct labels and verification contracts only unless a newly measured defect is found.
- **Scope:** documentation and focused verification tooling for Oak/Beech adult growth/site/canopy/competition. The 2026-10-27 preflight found the live values/arithmetic structurally conformant; therefore this slice makes **zero serialized species-YAML/scene change and zero production C# change** unless a later measured gate demonstrates a defect and returns to the Manager.
- **Required policy:** Oak 0.8/0.65/120/42.5/5; Beech 0.9/0.75/120/42.5/6.5 as gameplay calibration; Oak High / Beech Intermediate as weak provisional ordinal; site multiplier 1.0; canopy shared; 8 m Hegyi shared; Beech height miss accepted/labelled.
- **Verification authority:** Manager authorises an attributed refresh of `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json` only for the three preflight-identified normalized-source drifts after confirming their changes do not alter the accepted adult arithmetic. `BroadleafAdultGrowthVerification` may be strengthened to assert the accepted live constants/site/canopy/Hegyi facts and record the accepted Beech height miss. No protected anchor may be rebaselined.
- **Exclusions:** no species asset edits; no scene/prefab edits; no value retune; no arithmetic change; no Sitka retune; no active site multiplier; no canopy transmissivity model.
- **Dependencies:** PARITY-03 static conformance is sufficient for the bounded docs/tooling patch; final `ADULT SPECIES PARITY REQUALIFIED` qualification waits for PARITY-03 Unity qualification and the normal machine-wide Unity queue.
- **Exit:** `ADULT SPECIES PARITY REQUALIFIED`.

## PARITY-05 — Regeneration / juvenile / browsing requalification

- **Status:** AUTHORISED after PARITY-03; serialized species edits wait for Rack release
- **Goal:** Requalify early-life and browse behaviour against FR-4/FR-5.
- **Required policy:** Oak distinct juvenile curves; Beech combined fallback; shared juvenile growth/promotion/density calibration; existing dispersal/maturity/mast/light rules; broadleaf stock abstraction label; browse ordering Oak > Beech > Sitka; no persisted browse-form state.
- **Exit:** `EARLY-LIFE SPECIES PARITY REQUALIFIED`.

## PARITY-06 — Broadleaf timber/economy/pruning gap closure

- **Status:** BLOCKED-READY on PARITY-04/ownership where shared assets overlap
- **Goal:** Qualify Oak/Beech timber/economics and close accepted pruning/evidence gaps.
- **Scope:** timber ladders, evidence labels, dormant flat-value removal/block, broadleaf pruning branch, economy integration.
- **Required policy:** Oak five rungs; Beech two rungs; quality = size + pruning/crown history only; Teagasc typical quotations = `ManagementGuidance`; selected references/roadside basis = gameplay calibration; 890 kg/m3 = work proxy only; no cost multiplier; broadleaf pruning 3.5/6.0 m, two lifts, >=3 years apart; Sitka schedule exact.
- **Exclusions:** no fork/form state; no premium-Oak target guarantee; no time-varying price deck.
- **Exit:** `BROADLEAF TIMBER ECONOMY AND PRUNING QUALIFIED`.

## PARITY-07 — Player-facing species truth

- **Status:** BLOCKED on enough of PARITY-04…06 to establish actual behaviour
- **Goal:** Make material species differences and model limitations understandable.
- **Scope:** inspection/teaching/Work Plan/timber wording and bounded UI integration.
- **Required wording:** quality uses size + pruning history; fork/defect not modelled; site/canopy limitations honest; wind says storms off/not modelled; premium Oak possible late/favourable, not promised.
- **Owner:** coordinate with S1-C/S1-D UI writers; one writer per shared file.
- **Verification:** rendered 720p/1080p plus copy review.
- **Exit:** `PLAYER-FACING SPECIES PARITY QUALIFIED`.

## PARITY-08 — Save / deterministic compatibility

- **Status:** AUTHORISED after production slices settle
- **Goal:** Prove model gating/old-save compatibility and reject unknown species safely.
- **Scope:** unknown-species rejection before mutation; old model 0/1 saves; model-2 saves; Legacy40; Reference archive; deterministic anchors.
- **Policy:** no parity-specific save field or schema bump. Species-less sale records stay for P0 while selected shared-class prices do not diverge.
- **Stop:** any legitimate accepted historical fixture contains an unknown species that current authority requires loading; return to Manager before compatibility policy changes.
- **Exit:** `SPECIES PARITY COMPATIBILITY PASS`.

## PARITY-09 — Integrated parity qualification and independent review

- **Status:** BLOCKED on PARITY-03…08
- **Goal:** Qualify combined three-species parity against the milestone/spec.
- **Scope:** pure/mixed density; adult/juvenile/browse/pruning; timber/economy; save/load; player-facing truth; completion/solvency; Multi-Scale Forestry Management Sitka-only guard; protected anchors.
- **Verification:** Tier 2 feature qualification; G2/G3/G4 and G5 only if unexpected persistence work occurred; independent reviewer completes checklist.
- **No Windows build.**
- **Exit:** `THREE-SPECIES PARITY P0 VERIFIED`.

## Task summary

| Task | State | Main dependency |
|---|---|---|
| PARITY-01 | COMPLETE | — |
| PARITY-02 | COMPLETE / ACCEPTED | 01 |
| PARITY-03 | VERIFIED / independent review #9 open | — |
| PARITY-04 | VERIFIED | 03 Unity accepted |
| PARITY-05 | AUTHORISED after 03 | 03 + ownership for YAML |
| PARITY-06 | BLOCKED-READY | 04 + ownership |
| PARITY-07 | BLOCKED | 04–06 sufficiently stable |
| PARITY-08 | BLOCKED-READY | production slices stable |
| PARITY-09 | BLOCKED | 03–08 |
