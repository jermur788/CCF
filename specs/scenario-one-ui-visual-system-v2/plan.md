# Technical Plan

> **Document metadata (recorded 2026-10-25, documentation-only; no implementation or verification claimed)**
>
> - Feature ID: `SCENARIO-ONE-UI-VISUAL-SYSTEM-V2`
> - Status: `APPROVED FOR IMPLEMENTATION` (specification); this plan is not evidence of implementation.
> - Source: exact user-provided plan in parent T3 thread `8f9dbb1b-5bb5-4697-8fc4-e93900e805a5` (user message, 2026-10-25)
> - Spec: `spec.md`
> - Base: `13988874cc36413d911243f4d3a6524436237bab`
> - Context: `d5f11f865dc9123ac3ff0bab571e0402ad03d276`

---

## 1. Preflight

Inspect:

- current Scenario One UI branch/candidate;
- UI Toolkit structure;
- USS files;
- shared UI helpers;
- font assets already licensed/available;
- map/minimap implementation;
- HUD implementation;
- Objective/Work Plan/Tree Inspection implementations;
- current shared colour constants.

Identify actual overlap before editing.

Use one registered UI writer/lane.

---

## 2. Freeze references

Copy approved reference files into:

`specs/scenario-one-ui-visual-system-v2/references/`

Record:

- filename;
- SHA-256;
- role;
- approval status.

The map reference is primary visual authority.

---

## 3. Visual inventory

Create a table of current UI surfaces including:

- source files;
- USS selectors;
- hard-coded C# colours;
- fonts;
- border styles;
- old palette dependencies.

Classify each surface:

`ALREADY CLOSE`

`NEEDS TOKEN MIGRATION`

`NEEDS VISUAL REWORK`

Do not modify behaviour during inventory.

---

## 4. Establish shared palette

Sample the approved reference.

Create a restrained canonical token set.

Prefer existing architecture if suitable.

Do not perform an unnecessary theme-framework rewrite.

---

## 5. Typography system

Identify:

- available heading face;
- body face;
- sizes;
- weight hierarchy;
- letter spacing.

Where a suitable serif/display face is unavailable, identify the closest professionally appropriate licensed option already available to the project.

Do not redistribute external font files without authorization/licensing confirmation.

---

## 6. Base components

Update shared:

- panels;
- section headings;
- tabs;
- buttons;
- border/rules;
- selected states;
- locked/disabled states;
- muted labels.

Qualify these before migrating individual screens.

---

## 7. Screen migration

Recommended order:

1. representative menu;
2. Objectives;
3. Work Plan;
4. Tree Inspection;
5. HUD;
6. Controls / Help;
7. Annual Review;
8. remaining supporting panels;
9. map/minimap consistency check only.

This ordering lets the shared visual language stabilize before the HUD is finalized.

---

## 8. Visual iteration

For every major screen:

1. render;
2. compare against primary reference;
3. correct obvious visual mismatches;
4. rerender.

Do not wait until the end to discover the design still looks like the old UI.

---

## 9. Verification

Run focused checks only.

Required:

- Unity compile;
- UI/static/offline checks relevant to changed files;
- navigation smoke;
- render review;
- no-console-error check.

Do not rerun hundreds of gameplay qualification checks solely for styling changes.

If nonvisual production code changes unexpectedly, reassess test scope.

---

## 10. Handoff

Report:

- source checkpoint;
- result checkpoint;
- files changed;
- palette tokens;
- typography decisions;
- visual deviations;
- comparison screenshots;
- test results;
- whether any nonvisual behaviour changed.
