# Plan — MULTI #899 approved crown model to bounded implementation

**Status: APPROVED (D-094, 2026-11-23).** One D-091 lane and one Core writer after the lane receipt; no Unity, UI artwork or save change.

1. **Source/data baseline — done (read-only, 2026-11-23).** MA-1 A/B genesis uses `DerivedManifest.CrownBitsFor(DBH)` = open-grown potential at CI = 0; positions come from `MultiAreaGenerator` in one shared millimetre frame, and A/B share a real boundary segment. Model 0 is therefore genuinely "open-grown at year 0", which is the defect being corrected.
2. **Model choice — decided (D-094).** Candidate C2 adopted: `r_target = r_pot / sqrt(1 + CI/Ci50)`, `r_0 = r_target(CI_0)`, `k = 0.15`, `Ci50 = 5`. Directional sector model rejected for this pass. Year-0 equilibrium plus bounded relaxation answers the initialisation concern.
3. **Owner G5 approval — recorded (D-094).** The approved values, coefficient provenance, bias caveat and per-rack/connected-woodland boundary policy are in `spec.md`.
4. **Stage 1 (engine readiness, no maths change).** Confirm the existing pure Core has no UI/Unity/HTTP dependency; keep existing immutable views and fenced commands; add `SpeciesProfile` (frozen Sitka values) and neutral `SiteConditions` only if absent. Prove model 0 output equality with an exact before/after hash on fixed runs. No restructure of working code.
5. **Stage 2 (CrownModel 1).** Add `CrownModelVersion`; implement C2; compute `CI_0` and annual `CI` over the connected woodland; keep work plans and summaries per rack; keep model 0 path untouched and selectable. New rule id / derived path; no frozen-fixture rewrite.
6. **Evidence.** Offline model 0 parity, C2 determinism, cross-border neighbour test, outer-perimeter test, radius bounds and annual-change cap; clipped-union cover and gap metrics for A and B at years 0/1/3 with a matched no-work control and a real 10-tree Fell; one targeted `/multi` run.
7. **Handoff.** Committed branch SHA, provenance, exact before/after metrics, separate reviewer verdict, `IMPLEMENTED`/`NUMERICALLY QUALIFIED`/`REVIEWED` kept distinct. No claim of biological accuracy beyond the stated domain; map artwork unchanged.

**Stop conditions:** changing frozen P0/reference/save semantics; rewriting model 0 output; touching Unity, UI presentation or fixtures; overlapping another writer's lane; physical D-091 not granted. Stop the affected step and report the concrete fact.
