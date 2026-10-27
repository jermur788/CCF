# TASK PACKET — Scenario One Second and Subsequent Interventions Specification

**Project:** CCF  
**Manager:** ChatGPT Game Dev  
**Role:** Primary specification author  
**Task class:** SPEC REQUIRED — specification only  
**Branch:** `spec/scenario-one-repeated-interventions-v1`  
**Base:** `3b499aa459b92212f7bb06897836bae5914b5230`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Goal

Create the draft specification for Scenario One's second and subsequent management interventions so the player can repeat the forestry-management cycle after the first intervention and ultimately reach scenario completion.

This is specification work only. Do not implement production code.

## Plain-English product outcome

The player must be able to repeat:

`assess → decide/mark/plan → Work Plan → execute → advance time → observe/review → decide again`

without resetting the scenario, fabricating a new forest, or treating the first intervention as a one-shot tutorial that permanently blocks later management.

## Inspect first

Map all current first-cycle assumptions in:

- lesson/progression state;
- objective gating;
- tree marking and Crop Tree state;
- Work Plan;
- work orders and execution/result state;
- year advance;
- Annual Review/history;
- planting/regeneration/protection workflows;
- completion criteria;
- save/persistence;
- one-shot flags and stable lesson/objective IDs.

Identify exactly which assumptions prevent clean re-entry into another management cycle.

## Canonical deliverables

Create:

- `specs/scenario-one-repeated-interventions-v1/spec.md`
- `specs/scenario-one-repeated-interventions-v1/plan.md`
- `specs/scenario-one-repeated-interventions-v1/tasks.md`
- `specs/scenario-one-repeated-interventions-v1/checklists/requirements.md`

Status must remain `DRAFT` until Manager approval.

## Required specification questions

The draft must explicitly define or identify as open:

- when the next management cycle becomes available;
- which existing lessons/objectives remain completed and which management actions become reusable;
- whether later interventions are objective-driven, free management, or a bounded hybrid;
- how previous marks/designations/orders/results are distinguished from new decisions;
- what `ready for next intervention` means;
- how completion remains reachable after repeated cycles;
- how saved state/history supports re-entry;
- how the player distinguishes previous work from current proposed work;
- compatibility rules for Legacy40 and Reference Future v1.

Where accepted authority does not settle an item, use:

`OPEN — MANAGER/USER DECISION REQUIRED`

Do not choose a convenient behaviour merely to simplify implementation.

## Relationship to other work

- The Review / Management History specification defines the history the player uses to understand earlier management. This repeated-intervention spec should define its expected interface but may be drafted concurrently.
- Multi-Scale Forestry Management may contribute to later treatment, but it does not by itself satisfy this repeated-intervention requirement.
- Rack Planning & Field Marking V2 is an Objective 1 workflow and should not automatically repeat every management cycle unless accepted design requires it.
- Three-species parity remains a separate pre-demo requirement.

## Out of scope

- Production code or Unity edits.
- New ecology/economy coefficients.
- General Scenario One curriculum redesign beyond what repeated interventions require.
- Multi-Scale Forestry Management implementation.
- Windows build.

## Verification

Documentation/spec review only. Tie every proposed behaviour to accepted authority or mark it explicitly as an open product decision. Preserve stable saved IDs wherever possible; do not assume visible lesson numbering equals persistent identity.

## Required GitHub handoff

Publish:

`Docs/Handoffs/S1-D-RepeatedInterventions-Spec/HANDOFF.md`

Include inspected one-shot assumptions, proposed repeated-cycle architecture, dependencies on Review/History, open product decisions, save implications and readiness for Manager review.

## Exit

`S1-D REPEATED-INTERVENTION SPEC DRAFT READY FOR MANAGER REVIEW`
