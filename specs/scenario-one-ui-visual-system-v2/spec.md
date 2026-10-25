# Scenario One UI Visual System V2

> **Document metadata (approved and verified 2026-10-25)**
>
> - Feature ID: `SCENARIO-ONE-UI-VISUAL-SYSTEM-V2`
> - Status: `VERIFIED` — user approval: `APPROVED FOR IMPLEMENTATION` (2026-10-25)
> - Repository target: `specs/scenario-one-ui-visual-system-v2/`
> - Source: exact user-provided specification in parent T3 thread `8f9dbb1b-5bb5-4697-8fc4-e93900e805a5` (user message, 2026-10-25)
> - Base: `13988874cc36413d911243f4d3a6524436237bab`
> - Context: `d5f11f865dc9123ac3ff0bab571e0402ad03d276`
> - Primary reference: `references/griffins-peak-ui-reference.png` (immutable; see `references/reference-manifest.md`)
> - Qualified implementation: `36eafce8656cc52c370c383f496ac6433cc5a028`; independent G2/G3 APPROVED. Product requirements below remain unchanged.
> - Verdict: `UI VISUAL SYSTEM V2 QUALIFIED — READY FOR THREE-PAGE ONBOARDING`
> - Evidence and limitations: `implementation-notes.md`; durable worker handoff under `/media/jer/Files/CCF/Verification/ScenarioOneUiVisualSystemV2-20261025/HANDOFF.md`. No G4 integration or human playtest is claimed.

---

## 1. Purpose

Replace the current earthy/olive-heavy Scenario One visual treatment with a substantially more professional, restrained and coherent visual system.

The approved target is the visual language of the uploaded **Griffin’s Peak forestry map UI**.

This is not a loose inspiration exercise.

It is a:

`REFERENCE-LOCKED VISUAL IMPLEMENTATION`

The implementation must reproduce the approved reference's design language as closely as Unity UI Toolkit reasonably allows.

---

# 2. Product problem

The current Scenario One interface is functionally qualified but visually inconsistent.

Problems include:

- excessive olive/forest-green UI colouring;
- mustard/gold used too frequently;
- headings relying heavily on colour rather than typography and hierarchy;
- comparatively heavy panels and borders;
- different parts of the game feeling as though they belong to different UI systems;
- the HUD, objectives, menus, Work Plan and supporting interfaces not achieving the same professional standard as the approved map design;
- an overall tone that feels more like an earthy forestry-themed game UI than a professional forestry simulation interface.

The visual redesign must address these issues without changing accepted gameplay.

---

# 3. Primary visual authority

## 3.1 Approved reference

Primary reference:

`Griffin’s Peak Forestry Map UI`

Source image supplied by the user:

`Griffin’s Peak Forestry Map UI(1)(1).png`

When added to the repository, store an immutable copy as:

`specs/scenario-one-ui-visual-system-v2/references/griffins-peak-ui-reference.png`

This image is the primary visual authority for:

- overall tone;
- colour balance;
- panel treatment;
- border treatment;
- visual density;
- typography hierarchy;
- restrained use of accent colour;
- navigation treatment;
- professional/cartographic character.

## 3.2 Current-state comparison

Current Scenario One reference:

`scenario1 menu.png`

Store as:

`specs/scenario-one-ui-visual-system-v2/references/current-scenario-one-ui.png`

This image is evidence of the visual state being replaced, not a design authority.

## 3.3 Reference hashes

Before implementation begins, record cryptographic hashes for both reference PNGs in:

`references/reference-manifest.md`

The implementation and final review must use those exact files.

Do not silently replace a reference image with a later generated version.

---

# 4. Reference-locked rule

The approved reference is not a mood board.

It is not optional inspiration.

The implementation shall reproduce its:

- visual tone;
- palette;
- hierarchy;
- border weight;
- panel treatment;
- typography character;
- spacing discipline;
- restrained accent usage;
- professional map/field-tool aesthetic;

as closely as technically practical.

## 4.1 No silent reinterpretation

The implementation agent may not:

- substitute a different design language;
- retain the existing earthy theme because it is easier;
- introduce its own palette;
- simplify the design into generic game UI;
- introduce unrelated stylistic embellishments;
- make major layout changes merely because it prefers them.

If a material part of the reference cannot be implemented safely:

`STOP THAT ELEMENT AND RECORD A SPEC DEVIATION`

Do not silently replace it.

---

# 5. Intended visual character

Scenario One should feel like:

- a professional forestry-management simulator;
- a cartographic / field-planning interface;
- a serious educational tool;
- modern but not futuristic;
- polished but not decorative;
- calm, readable and information-dense without feeling cluttered.

It should not feel like:

- a fantasy game UI;
- a rustic/woodland-themed interface;
- an olive-and-gold management game;
- a generic Unity HUD;
- a brightly coloured tutorial system.

---

# 6. Core visual principles

## 6.1 Palette

Move substantially away from:

- olive-heavy panel backgrounds;
- mustard/gold headings;
- earthy green-on-green presentation.

Move toward:

- cool dark charcoal;
- slate;
- restrained blue-green / deep teal;
- soft off-white primary text;
- cool muted secondary text;
- subtle cool-grey/blue borders;
- limited semantic accent colours.

The background interface should be visually quieter than the forestry imagery.

## 6.2 Accent colour

Accent colour must be used semantically rather than decoratively.

Use strong accent primarily for:

- active navigation;
- selected elements;
- actionable controls;
- player/waypoint/state distinctions;
- meaningful warnings.

Do not use accent colour for every section heading.

Gold/yellow must not remain the default heading colour.

## 6.3 Typography

Introduce a stronger hierarchy consistent with the approved map reference.

Major page/panel titles should use a refined display treatment comparable to the map UI.

Body text should remain highly legible.

Required hierarchy:

1. page / major panel title;
2. section heading;
3. body text;
4. secondary/muted text;
5. captions / metadata.

Typography must create hierarchy primarily through:

- typeface;
- weight;
- size;
- spacing;

rather than excessive colour.

## 6.4 Serif/display treatment

Where technically appropriate, major headings may use a serif/display face to reproduce the map reference's cartographic/editorial character.

Do not apply serif styling indiscriminately to dense body text or small controls.

Font licensing and repository availability must be checked.

Do not embed an unsuitable substitute merely to satisfy this requirement.

## 6.5 Panels

Panels should move toward:

- dark neutral/slate surfaces;
- restrained translucency where readability permits;
- fine borders;
- subtle separators;
- low visual mass;
- minimal corner rounding.

Avoid:

- thick bright borders;
- strong olive fills;
- chunky card styling;
- excessive nested boxes.

## 6.6 Lines and borders

Use thin rules similar to the map UI.

Borders should organize content quietly.

They should not dominate it.

## 6.7 Buttons and navigation

Buttons should be flatter, calmer and more compact.

Primary actions should remain obvious without large blocks of gold/green.

Navigation should visually resemble the restrained treatment of the reference map tabs.

## 6.8 Icons

Icons should use a consistent:

- line weight;
- scale;
- colour treatment;
- visual language.

Avoid mixing decorative, skeuomorphic and outline icons in the same UI family.

## 6.9 Relationship to the world

The UI should feel integrated with the forest rather than visually detached from it.

Where appropriate:

- allow subtle environment visibility;
- use restrained translucent panels;
- avoid oversized opaque blocks.

Readability remains mandatory.

---

# 7. Screens in scope

Apply the visual system consistently to the current Scenario One interface.

At minimum inspect and update:

## 7.1 HUD

- normal walking HUD;
- status panels;
- waypoint treatment;
- selected-tree state;
- relevant action hints;
- objective summary widget.

Preserve HUD layout unless a very small styling-driven adjustment is unavoidable.

## 7.2 Objectives

- objective panel;
- active/inactive state;
- completion state;
- explanatory text;
- controls.

## 7.3 Work Plan

- section headers;
- locked/unlocked states;
- cards/panels;
- controls;
- explanatory content.

The term `card` may remain an internal implementation term but should not be exposed unnecessarily in player-facing help.

## 7.4 Tree Inspection

- header;
- selected-tree information;
- competitor information;
- action states;
- badges/markers;
- supporting text.

## 7.5 Controls / Help

- overall panel;
- headings;
- key/action presentation;
- explanations;
- visual grouping.

This task changes visual treatment only.

Detailed control-remapping/content changes belong to a separate task unless trivially required to preserve clarity.

## 7.6 Menus

- Scenario One in-game menu surfaces;
- pause/help surfaces;
- relevant navigation controls.

Do not redesign the three-page startup onboarding in this feature.

## 7.7 Annual Review

Apply the same visual language.

## 7.8 Stand Map / Minimap

The existing professional map design is a primary reference.

Do not degrade it merely for consistency.

Instead, bring the rest of Scenario One closer to it.

## 7.9 Other shared Scenario One panels

Inspect all Scenario One UI surfaces and identify any remaining obvious legacy styling.

The final implementation should not leave isolated major panels in the old earthy theme.

---

# 8. Explicit exclusions

This feature does NOT redesign:

- gameplay mechanics;
- rack rules;
- competition rules;
- forestry models;
- save data;
- scenario progression;
- annual simulation;
- economy;
- reference future;
- map content;
- keyboard bindings;
- onboarding copy;
- onboarding page sequence;
- onboarding forestry illustrations.

The upcoming three-page onboarding feature is separate.

It will consume this visual system after this feature establishes a qualified baseline.

---

# 9. Layout policy

This is primarily a visual-system change, not a wholesale layout redesign.

Preserve existing functional layouts where they are already effective.

Small adjustments are allowed when necessary to reproduce the approved visual system, for example:

- padding;
- whitespace;
- title spacing;
- border placement;
- tab height;
- control density.

Material information-architecture changes are out of scope.

---

# 10. Shared design tokens

Centralize the visual system as far as practical without performing a broad UI architecture rewrite.

At minimum establish or reconcile shared tokens for:

- primary panel background;
- secondary panel background;
- overlay/translucent background;
- primary text;
- secondary/muted text;
- border/rule;
- active state;
- selection state;
- warning/error;
- disabled/locked state;
- semantic forestry green where needed;
- player/waypoint colours where needed.

Prefer USS variables/shared helpers over duplicated literals.

C# UI drawing outside USS should mirror the same canonical tokens.

Avoid creating independent palettes in individual views.

---

# 11. Colour extraction

The agent must inspect the approved map reference and derive a coherent palette from it.

Do not simply reuse current colours.

Record the final palette in the implementation handoff with:

- token name;
- RGB/RGBA or hex value;
- intended role;
- nearest reference-image region from which it was derived.

The exact values may differ slightly where needed for accessibility/contrast.

---

# 12. Accessibility and readability

The visual reference does not override basic readability.

Required:

- body text remains readable at supported resolutions;
- important active states are distinguishable without relying solely on colour where practical;
- disabled states remain visible;
- contrast is sufficient for normal play;
- no colour change may make forestry/game state ambiguous.

If reference fidelity and readability conflict materially:

record the deviation.

---

# 13. Supported resolutions

Mandatory visual qualification at:

`1280 × 720`

and:

`1920 × 1080`

Also confirm the UI remains sane under the existing higher-resolution/scale configurations used by the project where practical.

No new responsive-layout architecture is required unless current behaviour is broken.

---

# 14. Visual fidelity acceptance

A screen does not pass merely because:

- Unity compiles;
- tests pass;
- all controls remain functional.

The final rendered UI must visually resemble the approved design language.

## 14.1 Required comparison format

For every major reviewed screen, produce:

`APPROVED REFERENCE | ACTUAL UNITY RENDER`

side-by-side.

Where useful also produce:

- overlay;
- difference image;
- sampled-colour comparison.

Pixel-perfect equality is not required.

Strong visual similarity is required.

## 14.2 Mandatory reviewed screens

At minimum:

- HUD — normal;
- HUD — tree selected;
- Objectives;
- Work Plan;
- Tree Inspection;
- Controls / Help;
- representative menu;
- Annual Review;
- Stand Map;
- Minimap.

Review both supported resolutions for representative/high-risk surfaces.

## 14.3 Failure criterion

If a reasonable side-by-side review concludes:

`This still looks substantially like the old UI rather than the approved map visual language`

the task is:

`FAIL`

regardless of functional-test results.

---

# 15. Forbidden substitutions

Do not:

- preserve olive backgrounds as the dominant UI colour;
- use yellow/gold for most headings;
- use heavy borders inconsistent with the reference;
- replace the approved hierarchy with generic Unity styling;
- introduce large rounded mobile-app cards;
- replace the display-heading concept with the old typography without documenting why;
- alter the map to make it match legacy UI;
- treat the reference as optional.

---

# 16. Spec deviation rule

Any material deviation must be recorded as:

- requirement;
- reason exact implementation was not possible;
- implementation chosen;
- visual impact;
- whether the deviation needs Manager/user review.

Minor platform/font rasterization differences do not require escalation.

A materially different layout/palette/design does.

---

# 17. Behaviour preservation

No player-facing gameplay behaviour should change.

The following must remain functionally equivalent:

- menu navigation;
- HUD data;
- objectives state;
- Work Plan state;
- Tree Inspection actions;
- map/minimap state;
- annual review;
- help/navigation actions.

---

# 18. Acceptance criteria

The feature passes only if:

1. shared Scenario One visual tokens are established;
2. major Scenario One UI surfaces adopt them;
3. old olive/gold styling is no longer dominant;
4. gold/yellow is no longer the normal heading colour;
5. panel framing visibly resembles the map reference;
6. typography hierarchy is substantially closer to the map reference;
7. HUD, Objectives, Work Plan and Tree Inspection clearly belong to the same visual family as the map;
8. no significant clipping/overlap regression exists;
9. visual review passes at 1280×720;
10. visual review passes at 1920×1080;
11. representative functional smoke passes;
12. no gameplay/save/simulation code changed;
13. visual comparison evidence is preserved;
14. all deviations are documented.

Successful verdict:

`UI VISUAL SYSTEM V2 QUALIFIED — READY FOR THREE-PAGE ONBOARDING`
