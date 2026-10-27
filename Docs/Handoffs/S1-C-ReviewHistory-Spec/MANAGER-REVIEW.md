# MANAGER REVIEW — S1-C Scenario One Review / Management History Specification

**Date:** 2026-10-27  
**Reviewer:** Overall Manager (ChatGPT Game Dev)  
**Reviewed branch:** `spec/scenario-one-review-history-v1`  
**Reviewed worker HEAD:** `9b96f57`  
**Base:** `3b499aa459b92212f7bb06897836bae5914b5230`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Verdict

**NEEDS CHANGES — NARROW MANAGER AMENDMENT ONLY**

The product direction, architecture, requirements, acceptance criteria and S1-D compatibility contract are accepted in substance. No further discovery/design cycle is required.

The draft must be amended before it becomes implementation authority because its proposed save-version bump collides with the qualified Rack Planning & Field Marking V2 persistence foundation, which already owns the v20 → v21 transition. S1-C must use the next available schema version on its eventual integration base rather than independently claiming v21.

## Manager decisions on OPEN-1…OPEN-4

### OPEN-1 — Pre-feature objective-achievement dates

**DECISION:** Use a labelled derived achievement year **only where the exact first-achievement year can be recomputed solely from persisted, year-tagged L1 records with the objective's accepted logic**. Where exact reconstruction is not possible because historical open-order/session state is absent, display **“not recorded”**. Never present an approximation as an exact historical fact.

Rationale: preserves the project's no-fabricated-history rule while still using durable evidence where it is sufficient.

### OPEN-2 — Session-only browse / DBH-growth facts

**DECISION:** Do **not** add persistence for these facts in S1-C. Keep them session-only and label them honestly after load. Revisit only if later playtesting demonstrates a player need strong enough to justify extra durable state.

### OPEN-3 — Intervention detail depth

**DECISION:** **Aggregate-first with drill-down to recorded targets.** Do not show full per-tree target dumps by default.

### OPEN-4 — Decision provenance

**DECISION:** The existing commitment/result trail is sufficient for S1-C. A complete historical trail of mark application/removal and retain/release decisions is **not required** for this feature. Preserve an extension point for later repeated-intervention and Multi-Scale Forestry Management provenance.

## Required save-version correction

Replace every fixed S1-C proposal of **save version 21** with:

> Use the **next available save-schema version on the integration base**. Do not collide with another accepted/qualified feature's schema version. With the currently qualified Rack V2 persistence foundation reserving v21, S1-C objective-achievement persistence is expected to use **v22 if Rack V2 integrates first**.

Implementation must inspect the actual integration base before choosing the number. No save version may be silently reused.

The additive objective-achievement record itself is accepted, subject to the above sequencing and normal save/reference gates.

## Required amendments

Update `spec.md`, `plan.md`, `tasks.md`, and `checklists/requirements.md` so that:

1. status becomes `APPROVED` after all amendments are applied;
2. OPEN-1…OPEN-4 are replaced by the decisions above (or retained only as a resolved decision register);
3. T-03/T-05/T-06 no longer depend on unresolved OPEN entries;
4. fixed `v21` language is removed from S1-C save work;
5. T-05 explicitly requires checking the integrated save version immediately before implementation and using the next available version;
6. Rack V2 v21 ownership is noted as the current collision to avoid;
7. the checklist tests the conditional old-save objective-history rule: exact labelled derivation only where reconstruction is evidence-complete, otherwise `not recorded`;
8. the original worker handoff receives an addendum recording this Manager review and the amended/approved HEAD.

## Review scope / verification

Manager compared `3b499aa...` → `9b96f57`: branch is two commits ahead, zero behind, with only the task packet, S1-C spec package and handoff added. No production source, Unity asset, save, scene, prefab, package or settings changes are present.

No Unity run is required for this specification review.

## Implementation authority

**NOT YET.** The branch remains DRAFT until the narrow amendment packet is completed and the resulting spec files say `APPROVED`.

After that amendment is published, no further product review is required unless the worker discovers a new material ambiguity or contradiction.

## Next step

Execute `Docs/Handoffs/S1-C-ReviewHistory-Spec/AMENDMENT-PACKET.md` on this same branch, publish the amended HEAD and return it to the Manager. If the amendments are exact, the Manager may integrate the approved specification and dispatch implementation tasks immediately.

## Dashboard

`DASHBOARD SYNC: NOT AVAILABLE`  
`Published dashboard reflects this review: NO`
