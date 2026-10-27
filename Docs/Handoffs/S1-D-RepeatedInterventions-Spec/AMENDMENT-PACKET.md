# AMENDMENT PACKET — S1-D Scenario One Second and Subsequent Interventions

**Manager verdict:** `NEEDS CHANGES — NARROW MANAGER AMENDMENT ONLY`  
**Authoritative review:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/MANAGER-REVIEW.md`  
**Branch:** `spec/scenario-one-repeated-interventions-v1`  
**Reviewed worker HEAD:** `666cb67cfc3e17fc22df5607ac4043c8fbd7ee7d`  
**Manager review commit:** `457461b9b92da2e96101f5b0eb9940b4e92fec0a`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Goal

Apply the Overall Manager decisions in `MANAGER-REVIEW.md` exactly and convert the S1-D package from `DRAFT` to an implementation-ready `APPROVED` specification.

This is a **documentation/spec amendment only**. Do not edit production source, Unity assets, saves, scenes, prefabs, settings, packages, Rack V2 files, S1-C files or Multi-Scale Forestry Management files.

## Required changes

Amend all four S1-D spec artifacts:

- `specs/scenario-one-repeated-interventions-v1/spec.md`
- `specs/scenario-one-repeated-interventions-v1/plan.md`
- `specs/scenario-one-repeated-interventions-v1/tasks.md`
- `specs/scenario-one-repeated-interventions-v1/checklists/requirements.md`

and append/update:

- `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/HANDOFF.md`

### 1. Resolve the eleven OPEN decisions

Replace OPEN-1…OPEN-11 with a resolved decision register reflecting `MANAGER-REVIEW.md` exactly:

- candidate lineage = partial implementation baseline, not feature completion;
- later-cycle mode = bounded hybrid;
- recurring Annual Review acknowledgement = advisory, not a hard gate;
- readiness = advisory component/reason state, no universal interval;
- teaching 40% footprint cap = hard only during its owning teaching activity, later advisory/non-blocking for ordinary manual work;
- previous/new provenance = existing orders/events/history only; no Crop Tree designation-year field or dated map layer in V1;
- guide authority = persisted-history/frozen teaching-sample facts; no new S1-D save progress field; no PlayerPrefs/session-only authority after historical lesson completion;
- completion = preserve protected two-reviewed-cycle rule; management may continue after Completed;
- rack reopening = delegated to Rack V2; Objective 1 teaching must not automatically repeat;
- Legacy40 = geometry-independent presentation may apply where records exist; no V2 curriculum migration;
- second Multi-Scale prescription = outside S1-D V1.

### 2. Reconcile S1-C interface

Rewrite §9 / EI-1…EI-7 so it consumes the S1-C contract rather than assuming the pre-draft interface.

Use these accepted boundaries:

- one read-only history capability over authoritative existing records;
- stable IDs and append-only/read-only history surfaces;
- distinguish recorded fact / current observed state / derived comparison / teaching;
- aggregate-first intervention detail with drill-down;
- absent provenance is not invented;
- session-only browse/DBH facts remain session-only;
- exact labelled derivation of old objective dates only when evidence-complete; otherwise `not recorded`;
- full historic mark/retain provenance is not required.

S1-D may add a small read-only derived cycle-state adapter. It may not add a competing history store or duplicate S1-C UI.

### 3. Update task statuses/dependencies

- T-00 becomes **COMPLETE — Manager decisions recorded in MANAGER-REVIEW.md**.
- T-01 remains the mandatory first execution task and must run on the actual integrated Rack V2 + approved S1-C contract base.
- T-02 becomes unblocked by product decisions but remains dependent on T-01/integration base.
- T-03 becomes unblocked by product decisions but remains dependent on T-01 and live ownership.
- T-04 becomes unblocked by product decisions but remains dependent on T-01 and curriculum ownership/sequencing.
- T-05 remains dependent on S1-C implementation/UI ownership and T-02.
- T-06 is approved in principle with **reject-before-mutation**, not repair, for duplicate/non-positive management-event IDs. If an accepted shipped fixture is ambiguous, STOP.
- T-07 = **CANCELLED for S1-D V1**.
- T-08/T-09 remain downstream qualification/review.

Do not dispatch production implementation from this amendment lane.

### 4. Preserve protected behaviour

The amended spec must explicitly retain:

- `ScenarioOneCompletionRules` normalised source/hash unchanged;
- eight objective predicates/targets unchanged;
- economy/ecology coefficients unchanged;
- Reference Future v1 frozen;
- no Windows build;
- no fixed intervention interval;
- no automatic treatment choice or hidden automation replacing player judgement.

### 5. Save policy

S1-D V1 adds **no new persisted fields** under the Manager decisions.

Retain the rule that any future schema work must use the next available version on the actual integration base. Do not name a fixed future version.

### 6. Approval status

When and only when all amendments above are applied consistently:

- change the spec package status to `APPROVED`;
- make tasks implementation-authorising only within their stated dependencies;
- update the checklist to test all resolved decisions;
- add a handoff addendum containing the amended HEAD and a statement that no production files changed.

## Verification

Documentation-only checks:

1. `git diff` from `666cb67` contains only the S1-D spec package, handoff/review/amendment docs;
2. no unresolved blocking `OPEN-*` remains;
3. no fixed future save-version number remains for S1-D;
4. T-07 is cancelled;
5. S1-C interface wording matches the Manager review;
6. completion rule/hash is explicitly protected;
7. status is consistently `APPROVED` across spec/plan/tasks/checklist/handoff.

No Unity, compile or runtime gate is required for this amendment.

## Exit

`S1-D REPEATED-INTERVENTION SPEC APPROVED — READY FOR T-01 BASELINE RE-VERIFICATION AFTER RACK V2 CORE INTEGRATION / S1-C CONTRACT APPROVAL`

Return the amended HEAD to the Overall Manager. Do not start T-01 from this lane unless separately dispatched.
