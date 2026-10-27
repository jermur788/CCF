# TASK PACKET — Scenario One Three-Species Parity P0 Current-Main Audit

**Project:** CCF  
**Manager:** ChatGPT Game Dev  
**Role:** Primary read-only auditor  
**Task class:** SPEC REQUIRED — pre-implementation audit only  
**Spec:** `specs/scenario-one-three-species-parity-p0/` — DRAFT  
**Task:** `PARITY-01 — Current-main implementation audit`  
**Branch:** `audit/scenario-one-three-species-parity-p0-current-main`  
**Base:** `c3df69c8e28478993cfd1f9d3304e5f4d453e612`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Goal

Re-audit the current implementation and produce the exact code/data/UI inventory needed to turn the draft three-species parity specification into approved implementation authority. No production code changes.

## Plain-English scope

The external Scenario One demo must model Sitka spruce, Oak and Beech to comparable depth. This audit identifies every place where the game currently treats those species differently, identically, or incompletely, so later implementation does not miss hidden Sitka-only assumptions.

Inspect at least:

- adult growth, density and self-thinning;
- canopy, light, crown and site traits;
- competition/Hegyi behaviour;
- regeneration, juveniles, planting and promotion;
- browsing/protection behaviour;
- timber assortment, yield and roadside value;
- Work Plan/economy settlement paths;
- inspection, teaching and player-facing species wording;
- save/model-version fields and species-sensitive persistence;
- deterministic/reference verification that parity work could affect.

For each relevant symbol/system, record current Sitka/Oak/Beech behaviour and classify it as:

- already species-specific and adequate for P0;
- species-specific but evidence/calibration needs closure;
- shared intentionally and defensibly;
- shared incorrectly / Sitka-specific inheritance;
- absent for Oak/Beech;
- uncertain — requires Manager/spec decision.

## Accepted boundaries to test against

- Pure Sitka must retain the existing exact calibrated density/self-thinning path.
- No invented Oak/Beech SDI or maximum-density equation.
- Mixed density is intended to use ordinal species-aware growing-space-demand classes.
- No DEC, DMC or mixture-productivity bonus in this P0 scope.
- Oak/Beech timber assortment/economics must not simply copy Sitka thresholds/prices.
- Evidence, transfer, ordinal representation and gameplay calibration must remain distinguishable.
- Reference Future v1 is frozen.
- Legacy40/old-save compatibility remains mandatory.

## In scope

Repository inspection, symbol/file mapping, implementation-truth table, identification of hidden cross-system dependencies, and recommendations for the subsequent evidence/parameter-closure task.

## Out of scope

- No production source edits.
- No Unity scene/prefab/asset edits.
- No new coefficients or mechanics.
- No approval of the draft spec.
- No Scenario Two implementation.
- No Rack V2 or Multi-Scale Forestry Management implementation.

## Verification

Read-only repository evidence with exact file/symbol references. Use Unity only if a read-only diagnostic is genuinely needed to resolve implementation truth; do not run a broad qualification suite.

## Stop and report if

- current implementation contradicts a premise of D-059/D-061/D-062;
- a substantial additional system must enter the P0 scope;
- a required behaviour cannot be determined from repository/verified state;
- work would require production edits.

## Required GitHub handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Audit/HANDOFF.md`

Include:

- exact inspected HEAD;
- files/symbols inspected;
- Sitka/Oak/Beech current-behaviour matrix;
- confirmed gaps;
- hidden dependencies;
- evidence/parameter questions for PARITY-02;
- any spec corrections required;
- verification actually performed.

## Exit

`CURRENT SPECIES IMPLEMENTATION MAPPED`
