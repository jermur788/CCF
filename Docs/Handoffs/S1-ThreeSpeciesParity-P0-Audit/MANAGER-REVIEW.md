# MANAGER REVIEW — Scenario One Three-Species Parity P0 / PARITY-01

**Date:** 2026-10-27  
**Reviewer:** Overall Manager (ChatGPT Game Dev)  
**Audit branch:** `audit/scenario-one-three-species-parity-p0-current-main`  
**Reviewed audit HEAD:** `33409e6`  
**Audit base:** `c3df69c8e28478993cfd1f9d3304e5f4d453e612`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Verdict

**PARITY-01 ACCEPTED — CURRENT SPECIES IMPLEMENTATION MAPPED**

The audit is accepted as the current-main implementation map. Its branch diff from the spec base is documentation only; no production or Unity state changed.

## Governance decision — existing three-species lineage

**DECISION: ADOPT AS IMPLEMENTATION BASELINE; REQUALIFY AND GAP-CLOSE.**

The existing lineage beginning with `integration/scenario-one-tier3-candidate` is not to be discarded or treated as automatically accepted.

Manager verification shows `integration/scenario-one-tier3-candidate` commit `8f9ed80` is an ancestor of the qualified Rack V2 base `c6342f79a3e89c040fadd232017a937132779385` (the Rack base is 122 commits ahead, zero behind). The current Rack V2 lanes therefore already depend on that three-species lineage. Rebuilding parity greenfield from `main` would duplicate and conflict with the active Scenario One lineage.

Accordingly:

1. the existing density, site-infrastructure, broadleaf timber/economy and Oak/Beech growth work becomes the **candidate implementation baseline** for PARITY-03…PARITY-06;
2. none of its empirical/calibration claims are accepted merely because code exists;
3. existing qualification claims must be re-read/re-run proportionately on the eventual implementation base;
4. weak or unsupported parameters must be corrected or explicitly labelled according to the accepted evidence matrix;
5. pure-Sitka preservation, Reference Future compatibility, save/model gating and milestone completion remain mandatory gates;
6. no production parity task may overwrite the active Rack V2 lanes or independently edit shared single-writer species YAML/assets.

This is **adopt + requalify**, not wholesale acceptance.

## Immediate consequence for the parity plan

The DRAFT parity spec/plan/tasks must no longer describe PARITY-03…PARITY-06 as greenfield implementation. They must become **requalification + gap closure of the inherited candidate lineage**.

PARITY-02 remains the next executable task and is read-only evidence/decision work.

After PARITY-02, the Manager will amend and approve the parity spec before any parity production coding is dispatched.

## Mandatory spec corrections from PARITY-01

The audit corrections C-01…C-12 are accepted as required amendment inputs, with these clarifications:

- C-01: record this adopt/requalify decision and scope “P0 not started” to `main` only.
- C-02: define an explicit mapping between project evidence classes and runtime/UI evidence labels; do not silently conflate them.
- C-04: species behaviour must be gated by an accepted persisted model/version mechanism; extending the existing `growthModel` domain is the leading candidate because the inherited lineage already uses `GrowthModel.SpeciesAware = 2`, but PARITY-02 must confirm the policy before spec approval.
- C-05: the existing pure-Sitka density statements and `SitkaGrowthModel` arithmetic are protected behaviour. PARITY-02 must define operational “100% Sitka”.
- C-06: `Ci50` may not serve simultaneously as an unexamined species-growth trait and density-victim bias; PARITY-02 must decide its role.
- C-07/C-08: broadleaf timber P0 may use only authoritative existing quality/history signals; the dormant flat broadleaf value and Sitka-derived mass proxy are not accepted inputs by default.
- C-09/C-10: broadleaf-containing anchors and existing “unmarketed” assertions are authorised-change candidates, not automatically regressions. Pure-Sitka and frozen Reference integrity remain protected.
- C-11: pruning is included in the species-blind audit surface; Multi-Scale Forestry Management remains Sitka-only unless separately amended.
- C-12: species assets and other shared YAML have one writer during implementation; merge order must follow the active Scenario One lineage after Rack V2 core integration.

## PARITY-02 authority

PARITY-02 must answer Q-01…Q-16 from the audit as far as evidence permits. Every output is classified as one of:

- DIRECT
- TRANSFER
- MODEL DERIVATION
- ORDINAL REPRESENTATION
- GAMEPLAY CALIBRATION
- FALLBACK-CONSERVATIVE
- UNSUPPORTED — NEEDS DECISION

No empirical precision may be invented.

Existing lineage values are candidates to evaluate, not defaults to preserve.

## Implementation authority

**NO parity production implementation is authorised yet.** The parity spec remains DRAFT pending PARITY-02, mandatory amendments, and Manager approval.

## Next step

Execute the PARITY-02 evidence/parameter-closure packet. Once returned, the Manager will resolve remaining choices, amend the DRAFT spec to the inherited-lineage architecture, and either approve it or return narrowly scoped evidence gaps.

## Dashboard

`DASHBOARD SYNC: NOT AVAILABLE`  
`Published dashboard reflects this review: NO`
