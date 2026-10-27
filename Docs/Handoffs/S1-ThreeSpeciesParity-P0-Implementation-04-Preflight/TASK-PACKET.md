# TASK PACKET — PARITY-04-PREFLIGHT Adult / Species-Data Read-Only Requalification Audit

**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED  
**Task:** `PARITY-04-PREFLIGHT`  
**Class:** read-only preflight / independent evidence and implementation audit  
**Status:** READY / DISPATCHED  
**Routing:** Claude Cloud / strongest available reasoning model  
**Unity:** NOT REQUIRED / DO NOT ACQUIRE THE UNITY LOCK  
**Production writes:** FORBIDDEN

## Why this task can run now

Canonical `PARITY-04` production work remains **BLOCKED-READY** on completed PARITY-03 qualification and release of shared species-YAML/scene ownership from Rack V2. This packet does **not** start PARITY-04 implementation.

The purpose is to use an otherwise-free cloud lane to determine, read-only, how much of PARITY-04 already conforms and what exact later production/Unity work is actually required. The result should reduce ambiguity and avoid unnecessary serialized-asset edits once the production gate opens.

## Review targets

Use GitHub as authority. Read:

- approved parity spec/plan/tasks/checklist on current `main`;
- PARITY-02 accepted evidence handoff and Manager review;
- PARITY-03 static handoff `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/HANDOFF.md` published at `b22c75f439e34d5cf5bcd91a1b4303c40ceec2f8`;
- PARITY-03 Manager review;
- production candidate `e1515e165eb40b40b5200992398984d69e0e8e8e` (production tree unchanged from `c6342f79a3e89c040fadd232017a937132779385`).

## Binding PARITY-04 policy to audit

Do not redesign or retune these accepted values/rules:

- Oak: `Pd 0.8`, `Ph 0.65`, `Dmax 120`, `Hmax 42.5`, `Ci50 5`;
- Beech: `Pd 0.9`, `Ph 0.75`, `Dmax 120`, `Hmax 42.5`, `Ci50 6.5`;
- all are `GAMEPLAY CALIBRATION`, not empirical coefficients;
- Oak `High` / Beech `Intermediate` is a weak provisional ordinal representation;
- site response remains neutral identity `1.0` in P0;
- canopy/light remains shared except for already-modelled crown geometry;
- Hegyi cutoff remains shared `8 m`;
- species `Ci50` is growth/crown/suppression calibration and must not bias density victim selection;
- the known Beech provisional-height-band miss is accepted and labelled, not a licence for a new height model;
- no Sitka retune;
- no active species×site multiplier;
- no species canopy transmissivity/deciduous-season model.

## Files / symbols to inspect

At minimum, inspect the exact candidate versions of:

- `Assets/ForestPrototype/Species/SessileOak.asset`;
- `Assets/ForestPrototype/Species/BeechSpecies.asset`;
- `Assets/ForestPrototype/Species/SitkaSpruce.asset` read-only for protected comparison;
- `Assets/ForestPrototype/TreeSpeciesDefinition.cs`;
- `Assets/ForestPrototype/ForestEcologyController.cs` adult growth / competition / canopy paths;
- `Assets/ForestPrototype/ForestTree.cs` where adult dimensions/crown state are applied;
- `Assets/ForestPrototype/SpeciesSiteResponse.cs`;
- `Assets/ForestPrototype/ForestSiteProfile.cs`;
- `Assets/ForestPrototype/ScenarioSiteProfiles.cs`;
- `Assets/ForestPrototype/SitkaGrowthModel.cs` protected Sitka path;
- relevant `Tools/Verification/ThreeSpecies/*` adult/site/canopy gates and historical verification records.

Inspect scene/prefab YAML only as necessary to confirm which species definitions are actually bound. Do not edit it.

## Required questions

For each item, return `CONFORMS`, `LABEL/DOC GAP`, `MEASURED/STRUCTURAL DEFECT`, or `UNITY QUALIFICATION REQUIRED` and cite exact file/symbol evidence:

1. Are Oak/Beech adult constants exactly the accepted calibration values above?
2. Are their evidence labels/comments consistent with `GAMEPLAY CALIBRATION`, with no empirical overclaim?
3. Is Oak High / Beech Intermediate represented only as the accepted weak ordinal class distinction?
4. Does adult growth use the accepted shared legacy asymptotic height law for broadleaves without importing the Sitka age/site envelope?
5. Does the site-response seam remain dormant/neutral at exactly `1.0` for Scenario One P0?
6. Is any species×site multiplier accidentally active in current production?
7. Is canopy/light shared as accepted, with species differences only through existing crown geometry/relaxation rather than invented transmissivity?
8. Is the competition neighbourhood still the shared 8 m Hegyi cutoff?
9. Is `Ci50` confined to its accepted growth/crown/suppression role and absent from density victim selection?
10. Does the current implementation preserve the protected pure-Sitka path?
11. Is the known Beech provisional-height miss merely a calibration limitation, or is there a separate implementation defect?
12. What exact Unity/runtime measurements are still needed to qualify PARITY-04 after the lock/dependency clears?
13. What exact files would require production changes if all currently visible gaps were fixed? Separate serialized YAML from C# and documentation-only changes.
14. Are any of those later write surfaces currently owned by Rack V2, S1-C, another parity task, or another live task?

## Restrictions

- No production, asset, scene, prefab, package, project-setting or canonical-spec edit.
- No Unity execution.
- No parameter retune.
- No rebaselining.
- No Rack V2/S1-C/S1-D/MFM change.
- No change to protected Sitka source or Reference Future material.
- No Windows build.
- Do not claim `ADULT SPECIES PARITY REQUALIFIED`; this is a preflight only.

If a material ambiguity appears between approved policy and inspected implementation, report it to the Manager rather than choosing a new rule.

## Handoff

Publish a GitHub Markdown handoff at:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Preflight/HANDOFF.md`

Include:

- exact source/candidate SHAs inspected;
- files/symbols inspected;
- 14-question verdict matrix;
- current conformance summary;
- exact later patch surface, if any;
- exact Unity gates still required;
- ownership/conflict findings;
- recommendation for the eventual PARITY-04 production task: `ZERO-DIFF REQUALIFICATION LIKELY`, `BOUNDED LABEL/IMPLEMENTATION PATCH REQUIRED`, or `MANAGER DECISION REQUIRED`.

If the cloud environment cannot publish to GitHub, return the complete handoff Markdown and state `PUBLICATION REQUIRED`; do not pretend publication occurred.

## Exit

`PARITY-04 PREFLIGHT COMPLETE — READY FOR MANAGER REVIEW`

or a precise blocker.
