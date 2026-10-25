# Requirements Checklist

> **Document metadata (verified 2026-10-25)**
>
> - Feature ID: `SCENARIO-ONE-UI-VISUAL-SYSTEM-V2`
> - Status: `VERIFIED` (specification)
> - Source: exact user-provided checklist in parent T3 thread `8f9dbb1b-5bb5-4697-8fc4-e93900e805a5` (user message, 2026-10-25)
> - Evidence: final source `36eafce8656cc52c370c383f496ac6433cc5a028`; independent G2/G3 APPROVED; 355 main UI checks plus 888 later-stage checks, zero failures; 94 native comparison sheets.
> - Clean candidate refers to committed task source, verified clean before restoration. The user explicitly requires four pre-existing local settings/material edits to be put back; those remain outside the task diff.
> - Verification is Unity Editor/API/callback smoke at 1280×720 and 1920×1080. Human/native-OS input, 4K and Player performance are not claimed.

---

## Reference authority

- [x] Approved map image stored in spec references.
- [x] Current-state screenshot stored separately.
- [x] SHA-256 hashes recorded.
- [x] Implementer confirms reference is normative, not inspirational.

## Palette

- [x] Old olive-heavy appearance no longer dominates.
- [x] Gold/yellow is not the default heading colour.
- [x] Cool charcoal/slate/blue-green base established.
- [x] Primary text uses professional off-white treatment.
- [x] Secondary text is restrained and readable.
- [x] Accent colour is used semantically.
- [x] Shared palette tokens exist.

## Typography

- [x] Major titles have stronger professional/display treatment.
- [x] Section/body/caption hierarchy is clear.
- [x] Dense text remains readable.
- [x] Font licensing/availability verified.
- [x] No arbitrary mixed typography.

## Panels

- [x] Panel borders are thin and restrained.
- [x] Heavy/chunky legacy framing removed where in scope.
- [x] Panel backgrounds are visually quieter.
- [x] Corner treatment matches reference direction.
- [x] Nested cards are minimized.

## Navigation

- [x] Tabs/buttons visually align with map reference.
- [x] Active state is clear.
- [x] Disabled state is clear.
- [x] Primary action remains obvious without excessive accent colour.

## HUD

- [x] Normal HUD aligned.
- [x] Selected-tree HUD aligned.
- [x] Waypoint styling aligned.
- [x] Objective widget aligned.
- [x] No layout regression.

## Objectives

- [x] Aligned to visual system.
- [x] Active/complete/locked states remain clear.

## Work Plan

- [x] Aligned to visual system.
- [x] Locked/unlocked state remains clear.
- [x] No gameplay-state change.

## Tree Inspection

- [x] Aligned to visual system.
- [x] Competitor/selection states remain understandable.

## Help / Controls

- [x] Aligned to visual system.
- [x] Key/action information remains legible.
- [x] No accidental control changes.

## Annual Review

- [x] Aligned to visual system.

## Map / Minimap

- [x] Existing professional treatment preserved.
- [x] No visual regression.
- [x] Rest of UI now clearly belongs beside it.

## Visual comparison

- [x] HUD comparison captured.
- [x] selected-tree comparison captured.
- [x] Objectives comparison captured.
- [x] Work Plan comparison captured.
- [x] Tree Inspection comparison captured.
- [x] Help comparison captured.
- [x] menu comparison captured.
- [x] Annual Review comparison captured.
- [x] Map comparison captured.
- [x] Minimap comparison captured.

## Resolution

- [x] 1280×720 reviewed.
- [x] 1920×1080 reviewed.
- [x] No clipping.
- [x] No overlapping.
- [x] No unreadable text.
- [x] No broken controls.

## Behaviour

- [x] Unity compile PASS.
- [x] UI smoke PASS.
- [x] No new exceptions.
- [x] No gameplay mechanics changed.
- [x] No save/schema changes.
- [x] No simulation changes.

## Spec conformance

- [x] No material silent deviations.
- [x] Deviations documented.
- [x] Reference manifest included.
- [x] Final candidate clean.
- [x] Evidence archived.

# Final gate

PASS only if:

`The implementation visibly looks like the approved professional map-style visual system when viewed side-by-side with the reference.`

Final verdict:

`UI VISUAL SYSTEM V2 QUALIFIED — READY FOR THREE-PAGE ONBOARDING`

or:

`NEEDS CHANGES`

or:

`NEEDS PRODUCT DECISION`
