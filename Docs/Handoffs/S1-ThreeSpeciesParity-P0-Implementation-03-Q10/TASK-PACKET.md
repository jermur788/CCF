# PARITY-03 Q10 — TASK PACKET

## Task

**PARITY-03 Q10 — Strengthen mixed-density continuity evidence without changing production behaviour.**

**Parent claim:** #74. Reuse it; do not create a duplicate execution claim.  
**Authority:** approved `specs/scenario-one-three-species-parity-p0/`, especially FR-1/FR-2/FR-11/FR-12, plus the accepted independent PARITY-03 review.  
**Starting main:** current clean `main` containing the Q11 completion and status reconciliation (`08361f06dfec6823538c9062a23a6edfd8bd20b7` or clean descendant after refresh).  
**Recommended branch:** `verify/parity-03-density-continuity-q10-v1`.  
**Target:** OpenAI Codex / strong Sol-class local verification model; Claude Sonnet-class is acceptable.  
**Class:** verification/evidence strengthening only. Existing PARITY-03 production and accepted Unity qualification remain unchanged.

## Reason

The existing offline continuity sweep crosses the pure-Sitka → mixed-route seam, but the accepted independent review found that its fixtures are too homogeneous and partly degenerate to prove a meaningful composition-continuity margin. Current code uses one Enlarged80 area, a High-class replacement only, identical DBH/competition within a fixture, and primarily absolute tolerances. The Unity gate proves route switching with one Oak but does not measure mortality continuity across the 0 → 1 non-Reference transition.

This follow-up strengthens evidence. It is **not** a defect finding and does not invalidate the already-approved PARITY-03 qualification.

## Ownership and sequencing

Static/offline harness work may proceed in parallel with Rack because it owns only the bounded ThreeSpecies density verification files named below and no production files.

Any Unity/controller execution remains behind RACK-V2-08 claim #49 unless the Overall Manager explicitly reroutes the machine-wide lane.

Before edits, refresh current `main`, live `Docs/Project/coordination/active-tasks.md`, claim #74 and this packet. Mark #74 `[RUNNING]` with actual agent/model/provider, branch/worktree and starting HEAD.

## Authorised files

Expected verification-only surface:

- `Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs`;
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs` only for the later controller-level measurement or narrow fixture helpers;
- narrow runner/report files under `Tools/Verification/ThreeSpecies/` if required;
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/HANDOFF.md`.

No production file under `Assets/ForestPrototype/` may change. No species asset/YAML, scene, prefab, save/schema or calibration value may change.

## Required offline strengthening

Extend the continuity evidence so it includes all of the following while preserving the existing continuity assertions rather than weakening them:

1. **Both stand geometries:** Legacy40 (0.16 ha) and Enlarged80 (0.64 ha), or the exact current authoritative areas if implementation truth has changed. Do not infer geometry from stale constants.
2. **Both non-Reference demand classes:** Intermediate (Beech-class demand) and High (Oak-class demand).
3. **Heterogeneous structure:** at least one deterministic low-loss fixture with a realistic spread of DBH and competition index, so CI actually affects competitive allocation. Do not use a fixture where every stem has the same CI.
4. **The exact seam:** compare a pure Reference/Sitka state with an otherwise identical state containing exactly one non-Reference stem. Preserve the existing 1/10/50/100 replacement sweep or an equally strong superset.
5. **Low-loss meaningful margin:** report absolute and relative movement in total loss at the seam. Add a relative criterion appropriate to the low-loss fixture so a several-tenths-of-a-percent absolute jump cannot hide behind the old `0.005` tolerance. The threshold must be justified from numerical continuity/testing sensitivity, **not forestry evidence**; label it `GAMEPLAY CALIBRATION` / verification tolerance where appropriate.
6. **Sitka component preservation:** continue proving the Sitka component at replacement states matches the protected literal/original Sitka block for the Sitka subset.
7. **Determinism:** fixtures and measurement must be deterministic/reproducible. Do not widen tolerances to absorb stochastic noise without explaining the measurement design.

The purpose is to detect a route/composition cliff, not to prove an empirical mixed-stand mortality law.

## Controller-level measurement

Add a bounded real-controller measurement to `MixedDensityVerification` (or the narrowest current equivalent) that demonstrates the 0 → 1 non-Reference transition on the actual controller call path:

- same starting stand/state as far as practical;
- pure Sitka/Reference uses the exact legacy route;
- one Oak **and/or** one Beech switches to the species-aware occupancy route as expected;
- report/compare the realised or expected density-mortality signal/loss across that transition rather than checking route/occupancy only;
- cover both Oak/High and Beech/Intermediate if practical in one fixture; if not, explain the bounded division of coverage;
- do not fabricate deaths or manipulate production state solely to force a green assertion.

This controller measurement requires Unity only if the current harness architecture requires Unity. If so, static implementation may hand off first with the Unity phase explicitly waiting behind #49.

## Protected interpretation

Manager interpretation remains fixed:

- FR-1 preserves exact pure-Sitka controller arithmetic.
- FR-2 species-neutral victim selection and `Ci50` exclusion apply to the **generic mixed component**; they do not delete or rewrite the protected Sitka component.
- No DEC, DMC, mixture bonus, broadleaf density-capacity equation or production retune is authorised.
- Movement of broadleaf-containing model-2 outputs is measurement only unless a separate Manager G3/G5 decision authorises a production change.

## Verification

Run the strengthened offline density/composite suite and its applicable static/type checks. Record:

- exact number of checks;
- old continuity assertion results;
- new geometry/class/heterogeneous-fixture results;
- seam absolute/relative differences;
- negative-control or mutation evidence showing the strengthened test would catch a deliberately introduced discontinuity/fixture defect where practical;
- any known pre-existing pure-Beech R2 misses separately, without rebaselining them.

If Unity is available after Manager release, run the controller-level gate on the exact candidate and preserve the previously accepted PARITY-03 protected checks. If Unity is not released, do not bypass Rack and do not claim the controller measurement verified.

## Stop conditions

Stop and return to Manager if:

- a production arithmetic/value change appears necessary;
- exact pure-Sitka behaviour would need to change;
- a protected test must be weakened/rebaselined;
- the stronger evidence exposes a material discontinuity that could affect accepted anchors;
- another active writer owns the same verification file;
- Unity is owned/prioritised by another task.

For a material production concern return:

`PARITY-03 Q10 CONTINUITY DEFECT FOUND — MANAGER G3/G5 DECISION REQUIRED`

For static completion while Unity remains queued return:

`PARITY-03 Q10 STATIC EVIDENCE STRENGTHENED / CONTROLLER UNITY MEASUREMENT WAITING — RACK UNITY PRIORITY`

## Handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/HANDOFF.md`

Include model/provider, branch/worktree, starting/ending SHA, exact changed files, offline results, numerical seam measurements, mutation/negative-control result, controller-level status, preservation results and repo cleanliness.

If every required static and controller measurement is complete, return:

`PARITY-03 DENSITY CONTINUITY EVIDENCE STRENGTHENED — READY FOR MANAGER REVIEW`

No Windows build.
