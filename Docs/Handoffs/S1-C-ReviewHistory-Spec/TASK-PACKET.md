# TASK PACKET — Scenario One Review / Management History Specification

**Project:** CCF  
**Manager:** ChatGPT Game Dev  
**Role:** Primary specification author  
**Task class:** SPEC REQUIRED — specification only  
**Branch:** `spec/scenario-one-review-history-v1`  
**Base:** `3b499aa459b92212f7bb06897836bae5914b5230`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Goal

Create the draft specification for the Scenario One pre-demo review/history system: after management work and time advancement, the player must be able to understand what was done, what happened afterward, and what prior treatment should inform the next decision.

This is specification work only. Do not implement production code.

## Plain-English product outcome

The player should be able to look back at earlier management and answer:

- what work was carried out;
- when it happened;
- what trees/area/work type it affected at an appropriate level;
- what immediate work result occurred;
- what forest/economic consequences are now visible;
- what earlier treatment matters for the next intervention.

The result should support the core loop `decide → advance time → walk → observe → understand why → decide again`, not become a spreadsheet-style analytics product.

## Inspect first

Map current implementation of:

- Annual Review;
- Work Plan results/history;
- work-order completion/result state;
- year advancement;
- intervention/event records;
- economy/history records;
- objective/progression history;
- save fields relevant to management history;
- existing UI panels exposing historical information;
- Reference Future review behaviour;
- stable IDs/keys that later interventions must preserve.

Distinguish verified repository facts from assumptions.

## Canonical deliverables

Create:

- `specs/scenario-one-review-history-v1/spec.md`
- `specs/scenario-one-review-history-v1/plan.md`
- `specs/scenario-one-review-history-v1/tasks.md`
- `specs/scenario-one-review-history-v1/checklists/requirements.md`

Status must remain `DRAFT` until Manager approval.

## Required design constraints

- Prefer extending existing review/history surfaces over adding duplicate systems.
- Historical displays must represent authoritative recorded past events; do not fabricate past facts from current state when provenance is absent.
- Separate recorded historical fact, current observed state, derived comparison, and explanatory teaching.
- Do not claim causal precision the simulation does not contain.
- Preserve Reference Future v1 and old-save compatibility.
- If new durable history state is needed, specify exact schema/default/migration semantics but do not implement them.
- Define what data/API the later repeated-intervention system may rely on.

## Relationship to other active work

- Rack Planning & Field Marking V2 may continue independently.
- Three-species parity may continue independently.
- Multi-Scale Forestry Management may later contribute additional provenance/history, but this spec must not depend on unimplemented Multi-Scale Forestry Management state for the basic repeated-management history requirement.
- Do not implement the second/subsequent intervention loop here; only define the history/review capability it will consume.

## Out of scope

- Production code or Unity asset changes.
- New ecology/economy coefficients.
- Multi-Scale Forestry Management implementation.
- Three-species parity implementation.
- Windows build.

## Open decisions

Do not silently resolve genuine product questions. Mark them in `spec.md` as `OPEN — MANAGER/USER DECISION REQUIRED`, with the narrowest possible decision statement.

## Verification

Documentation/spec review only. Check the draft against Decision Log, Current Milestone, Game Brief, inspected repository truth and existing save/history architecture.

## Required GitHub handoff

Publish:

`Docs/Handoffs/S1-C-ReviewHistory-Spec/HANDOFF.md`

Include implementation facts discovered, proposed boundaries, open decisions, save implications, and why the draft is ready/not ready for Manager approval.

## Exit

`S1-C REVIEW/HISTORY SPEC DRAFT READY FOR MANAGER REVIEW`
