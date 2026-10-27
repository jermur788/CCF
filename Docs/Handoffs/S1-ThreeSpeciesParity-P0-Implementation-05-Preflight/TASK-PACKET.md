# TASK PACKET — PARITY-05-PREFLIGHT Early-Life / Browse Parameter Mapping Audit

**Feature:** Scenario One Three-Species Parity P0  
**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED  
**Task:** `PARITY-05-PREFLIGHT`  
**Class:** read-only implementation mapping / evidence-to-production architecture audit  
**Status:** READY / DISPATCHED  
**Manager:** ChatGPT Game Dev / Overall Manager

## Purpose

Before any PARITY-05 production edit, map the accepted PARITY-02 early-life/browse evidence policy onto the actual current production architecture. Do **not** infer a numeric conversion where the accepted evidence representation and current runtime representation differ.

The Manager identified two material mapping questions:

1. PARITY-02 accepted simple early-life calibration values (`establishmentRatePerYear`, `establishmentSurvival`, `juvenileAnnualSurvival`) while current production represents recruitment through seed/light/suitability response curves plus density admission and juvenile survival through light-response logic.
2. PARITY-02 accepted browse susceptibility multipliers Sitka 1.0 / Beech 1.8 / Oak 2.2 while current `TreeSpeciesDefinition.browsePalatability` is a 0–1 field and `JuvenileEcologyRules.AssessBrowse` multiplies background pressure by palatability and other exposure terms.

This task resolves the mapping facts and options only. It does not choose or implement a new biological calibration.

## Authority

Read, in order:

1. current `Docs/Project/decision-log.md`, `current-milestone.md`, `game-brief.md`;
2. `specs/scenario-one-three-species-parity-p0/spec.md`, `tasks.md`, and requirements/checklists;
3. `Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md` and Manager review;
4. PARITY-03 and PARITY-04 Manager reviews;
5. live `Docs/Project/coordination/active-tasks.md`.

Accepted PARITY-02 facts to preserve:

- early-life gameplay calibration table:
  - Sitka: establishment rate 0.014, establishment survival 0.62, juvenile annual survival 0.93;
  - Oak: 0.0035, 0.28, 0.88;
  - Beech: 0.0025, 0.24, 0.86;
  - policy intent: broadleaf establishment is not more favourable than Sitka; Beech at least as constrained as Oak; no empirical Irish annual regeneration-rate basis was identified.
- browsing qualitative ordering is evidence-backed: **Oak > Beech > Sitka**.
- accepted gameplay-calibration susceptibility multipliers: Sitka 1.0, Beech 1.8, Oak 2.2.
- accepted protected-browse multiplier: 0.2 for all three species; exact numeric effect remains gameplay calibration.
- Oak uses distinct juvenile curves; Beech uses combined fallback; shared juvenile growth/promotion/density calibration remains unless existing authority says otherwise.
- regeneration substrate remains species-neutral.
- dormant proposed broadleaf age-phase fields remain out of runtime until separately accepted.
- no persisted browse-form state.

## Routing

Target: **Claude subscription 2**, strongest available Claude reasoning model. This is a read-only repository reasoning task; no Unity is needed.

Recommended branch only if needed to publish the handoff:
`research/scenario-one-three-species-parity-p0-05-preflight-v1`

No production worktree ownership is granted. Preserve unrelated dirty work.

## Required inspection

Inspect current production implementation, including at minimum:

- `Assets/ForestPrototype/TreeSpeciesDefinition.cs`
- `Assets/ForestPrototype/JuvenileEcologyRules.cs`
- `Assets/ForestPrototype/ForestEcologyController.cs`
- `Assets/ForestPrototype/BrowsingConditions.cs`
- `Assets/ForestPrototype/Species/SitkaSpruce.asset`
- `Assets/ForestPrototype/Species/SessileOak.asset`
- `Assets/ForestPrototype/Species/BeechSpecies.asset`
- relevant species configuration/validation and existing parity/juvenile/browse verification.

## Questions to answer

### M1 — Early-life parameter representation

For each of Sitka, Oak and Beech, identify the exact current production fields/curves and execution path controlling:

- seed arrival / seed saturation;
- establishment response;
- establishment density admitted;
- annual juvenile survival;
- juvenile height growth;
- promotion thresholds;
- regeneration support / species registration.

Then determine whether the accepted Q07 simple values are:

A. already represented directly;
B. represented indirectly by an existing curve/runtime combination with a defensible derivation;
C. not represented by the current architecture and would require a new explicit production parameter/operation; or
D. obsolete as numeric implementation targets but still valid only as ordinal/conservative policy evidence.

Do not choose B or D without showing the exact derivation/authority. Do not normalize, rescale, or fit values merely to make them fit.

### M2 — Browse susceptibility mapping

Map the current browse probability formula and all species-specific inputs. Explain exactly how the accepted `1.0 / 1.8 / 2.2` susceptibility values relate—or do not relate—to current `browsePalatability` and its `[0,1]` serialized range.

Provide bounded implementation options if a representation change is required. Examples may include a separate non-normalized susceptibility multiplier or an explicit normalization policy, but these are **proposals only** and must state behavioral consequences. Do not implement or silently choose one.

### M3 — Protection factor

Locate how protection currently reduces browse exposure. Determine whether the accepted 0.2 protected multiplier is directly represented, indirectly produced, absent, or contradicted. Identify exact production surface if a change would be needed. Preserve no-fixed-unit-count policy.

### M4 — Existing species policy conformance

Verify, from current bytes:

- Oak distinct juvenile response behavior;
- Beech combined/fallback behavior;
- Sitka baseline behavior;
- species-neutral regeneration substrate;
- no broadleaf dormant age-phase runtime use;
- no persisted browse-form state;
- unknown species / configuration handling relevant to this slice.

### M5 — Exact future patch surface and ownership

List every serialized species asset and C# file that a minimal production implementation would need under each viable mapping option. Refresh live coordination and identify any active writer overlap. Do not edit any such file.

## Required Manager-decision matrix

For every unresolved mapping question provide a table with:

- current production fact;
- accepted evidence/spec intent;
- mismatch or conformance;
- evidence label (evidence-backed ordering, gameplay calibration, implementation fact, unsupported-needs-decision, etc.);
- viable minimal mapping option(s);
- behavioral consequence;
- recommendation;
- whether Manager can decide mechanically from existing authority or whether a product/calibration decision is genuinely required.

## Exclusions

Do **not**:

- edit production, YAML, scenes, prefabs, verification targets, specs or decision log;
- run Unity;
- retune adult growth, density, timber, economy, pruning, site, canopy or Rack/MFM behavior;
- introduce a second species parameter table;
- activate dormant age-phase candidates;
- change save schema or add persisted browse state;
- weaken existing tests or rebaseline anchors;
- claim `EARLY-LIFE SPECIES PARITY REQUALIFIED`.

## Handoff

Publish:
`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05-Preflight/HANDOFF.md`

Include exact inspected main SHA, actual executor/model/provider, files inspected, all M1–M5 findings, Manager-decision matrix, current ownership check, and precise recommended next production packet surface.

## Exit

`PARITY-05 MAPPING PREFLIGHT COMPLETE — READY FOR MANAGER DECISION`

or a precise blocker.
