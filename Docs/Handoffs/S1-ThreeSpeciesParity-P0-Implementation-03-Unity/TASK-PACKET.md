# TASK PACKET — PARITY-03 Unity Tier-2 Qualification

**Task:** `PARITY-03-UNITY-QUALIFICATION`  
**Status:** BLOCKED-READY — wait for machine-wide Unity lock  
**Target:** TTree Code / local Unity-capable verification lane  
**Candidate branch:** `feature/scenario-one-three-species-parity-density-requal-v1`  
**Tested production HEAD:** `e1515e165eb40b40b5200992398984d69e0e8e8e`  
**Static handoff:** `b22c75f439e34d5cf5bcd91a1b4303c40ceec2f8`

## Purpose

Complete the missing focused Tier-2 Unity qualification for PARITY-03 without changing production code unless Unity exposes a demonstrated spec-conformance defect.

The static/offline phase is Manager-accepted as PASS with **production diff NONE**. This packet exists only because the machine-wide Unity lane was unavailable.

## Preflight

Before Unity execution:

1. refresh `Docs/Project/coordination/active-tasks.md`;
2. verify Rack V2 no longer owns the machine-wide Unity lock;
3. respect older queued Unity work, especially S1-C-IMPL-02, unless the Overall Manager changes priority;
4. verify exact branch/worktree/HEAD and a clean verification lane;
5. acquire the machine-wide Unity lock normally; never force-release another owner.

If the exact production HEAD differs, STOP and report identity mismatch unless the Manager explicitly designates a descendant.

## Required gates

On exact `e1515e165eb40b40b5200992398984d69e0e8e8e` production state (documentation-only handoff commits may be present):

- clean Unity import/compile;
- focused `MixedDensityVerification` / ThreeSpecies density gate;
- pure Sitka fixture remains exact;
- near-pure mixed fixtures exercise the route seam;
- Oak/Beech mixed fixtures run under model 2;
- protected Sitka density fingerprints remain exact;
- model 0/1 remain legacy;
- species-neutral density-victim behavior remains consistent with the static proof;
- no save-schema, Rack V2, YAML/scene, economy/pruning/UI or Reference archive change.

Report the known Beech provisional-height limitation honestly; do not retune it or weaken a gate merely to obtain green. PARITY-02 records it as an accepted labelled calibration limitation, not empirical validation.

## Patch policy

Prefer **zero production diff**. Patch only a Unity-demonstrated spec-conformance defect. Any retuning of class weights, response constants, occupancy normalization, annual-loss ceiling, basal-area budget or victim-selection constants requires Manager approval first.

## Handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Unity/HANDOFF.md`

Include exact Unity version/lane/lock record, tested HEAD, production diff, commands/gates, actual results, protected fingerprints, failures/caveats and whether the static handoff remains valid.

Exit:

`MIXED DENSITY PARITY REQUALIFIED — READY FOR MANAGER REVIEW`

or a precise blocker.

No full three-species parity, integration, external-demo readiness or Windows-build claim follows from this packet alone.
