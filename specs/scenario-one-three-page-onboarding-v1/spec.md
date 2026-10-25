<!-- Exact user-supplied specification follows. -->

Implementation gate: ONBOARDING REFERENCES LOCKED — user-approved mapping/treatment, final files/hashes in references/reference-manifest.md.
Source checkpoint inspected: 0805ffb9b6b1153442b3530d7404be5847ce9c41.
Source: T3 thread 0c55a504-715b-46b5-a3a2-e8ef3639bbff, user request 2026-10-25.
The user's approval of the specification is preserved; this gate describes unresolved reference conformance, not revocation of approval.
Status: IMPLEMENTING against the user-approved specification and final reference lock.

# SPEC REQUIRED

Feature ID:

`SCENARIO-ONE-THREE-PAGE-ONBOARDING-V1`

Dependency:

`SCENARIO-ONE-UI-VISUAL-SYSTEM-V2`

Qualified visual-system checkpoint:

`0805ffb`

Status:

`APPROVED FOR DESIGN LOCK + IMPLEMENTATION`

Repository target:

`specs/scenario-one-three-page-onboarding-v1/`

Required files:

- `spec.md`
- `plan.md`
- `tasks.md`
- `checklists/requirements.md`
- `references/`
- `references/reference-manifest.md`

---

# 1. Purpose

Replace the existing Scenario One introductory information popup with a three-page onboarding sequence.

The three pages have distinct educational purposes:

1. **What is Continuous-Cover Forestry?**
2. **How CCF changes a forest over time**
3. **Scenario One — Transforming a Sitka Plantation**

The feature must use the already-qualified Scenario One UI Visual System V2.

Do not create a separate onboarding theme.

---

# 2. Reference-locked implementation

This is a:

`REFERENCE-LOCKED VISUAL IMPLEMENTATION`

Approved onboarding mockups are normative visual authority once frozen in `references/`.

They govern:

- composition;
- information hierarchy;
- image placement;
- panel proportions;
- spacing;
- visual density;
- overall presentation.

The implementation agent may not reinterpret them as loose inspiration.

The exact written copy comes from this specification, not from image OCR.

---

# 3. Mandatory pre-implementation design lock

Before production implementation:

1. prepare the final Page 1, Page 2 and Page 3 reference images;
2. confirm they use Visual System V2;
3. verify forestry imagery represents the intended progression;
4. store them under `references/`;
5. record SHA-256 hashes;
6. create `reference-manifest.md`.

Production implementation may begin only after:

`ONBOARDING REFERENCES LOCKED`

This step exists specifically to prevent implementation drift from the agreed visual design.

---

# 4. Startup sequence

For a new Scenario One game:

`Start New Scenario`
→ Page 1
→ Page 2
→ Page 3
→ enter forest

Back navigation:

Page 2 → Page 1  
Page 3 → Page 2

Continue existing game must not incorrectly replay the onboarding.

The existing old Scenario One information popup and its separate “More detail” section are superseded.

---

# 5. Page 1 — What is Continuous-Cover Forestry?

## Purpose

Explain CCF independently of Scenario One.

Do not mention:

- Scenario One;
- the player's specific plantation;
- oak/beech planting requirements;
- Scenario One objectives.

This page must still make sense if reused later for another scenario.

## Title

`What is Continuous-Cover Forestry?`

## Main definition

Use no more than two concise sentences.

Approved semantic content:

> Continuous-cover forestry keeps a forest continuously wooded while it is managed. Instead of clear-felling the whole stand, trees are removed selectively and new generations establish beneath and between those that remain.

Minor copy editing for readability is allowed only if meaning is preserved.

## Visual progression

The main explanatory visual must show a genuine progression rather than three similar mixed forests.

### Image 1 — starting condition

Must visibly resemble:

`even-aged Sitka plantation`

Characteristics:

- mostly Sitka;
- regular spacing;
- similar age;
- similar height;
- limited vertical structure;
- plantation character.

It must NOT already look like mature mixed CCF.

### Image 2 — transformation underway

Must visibly show:

- retained mature Sitka;
- selective openings;
- greater spacing variation;
- regeneration beginning;
- early structural differentiation.

It should clearly build on Image 1.

### Image 3 — developed CCF structure

Must visibly show:

- retained mature productive trees;
- different age/size classes;
- regeneration;
- mixed structure;
- the additional species represented as actual broadleaf trees/young plants rather than more Sitka seedlings.

The progression must read clearly as:

`uniform plantation`
→ `selective transformation`
→ `structurally varied continuously covered forest`

## Supporting principles

Use a maximum of four short principles:

- Keep forest cover
- Manage repeatedly over time
- Encourage regeneration
- Build varied structure

Do not turn these into long paragraphs.

## Simulator-level meaning

If a short footer is retained, it must remain scenario-independent.

Do NOT use wording such as:

`In this scenario...`

Suitable semantic direction:

`CCF is a long-term process of repeated decisions, observation and adaptation.`

---

# 6. Page 2 — How CCF works over time

## Purpose

Explain the management process and the broader forestry logic without yet introducing the player's specific Scenario One assignment.

This page may contain slightly more information than Page 1 but must remain visually led.

## Core teaching concept

CCF is not one intervention.

It is an iterative cycle:

`observe`
→ `retain/select`
→ `create space/light`
→ `regenerate/establish`
→ `review`
→ `repeat`

## Required concepts

Represent briefly:

- retain productive trees;
- selectively remove competitors;
- create suitable light/space conditions;
- encourage natural regeneration;
- establish other tree species;
- retain continuous cover;
- repeat management over many years;
- adapt decisions to how the stand actually develops.

## Species message

Do not describe species diversification as merely optional.

The page should communicate that managing a productive forest can involve combining:

- retained productive conifers;
- broadleaf diversity;
- regeneration;
- structural diversity.

Do not claim one universal species mixture is scientifically optimal for every Irish site.

## Evidence philosophy

The page itself should not become a scientific disclaimer, but the design must be compatible with the project's principle:

`evidence-constrained, not evidence-confined`

The simulator uses available forestry evidence and transparent modelling to explore transformations for which mature Irish CCF evidence is still limited.

Simulated outcomes are model projections, not empirical proof.

---

# 7. Page 3 — Scenario One: Transforming a Sitka Plantation

## Purpose

Explain the specific player challenge.

Do not repeat the generic definition of CCF.

## Title

`Scenario One — Transforming a Sitka Plantation`

## Starting condition

The player inherits:

- an even-aged Sitka plantation;
- largely one species;
- relatively uniform age/size structure;
- plantation-style spacing/structure.

## Core scenario objective

Scenario One is NOT about deciding whether to diversify.

Diversification is part of the scenario goal.

The player must work out **how to establish both additional available tree species while retaining productive Sitka**.

The long-term target is a forest containing:

- Sitka spruce;
- additional species 1;
- additional species 2;

with:

- all three species present;
- varied ages;
- varied sizes;
- regeneration;
- structural diversity;
- continuous forest cover;
- retained productive Sitka.

Use the actual implemented species names in the final copy.

Do not use vague wording such as:

`other species where appropriate`

or:

`you may introduce other species`

## Recommended central wording

Semantic target:

> You begin with an even-aged Sitka plantation. Your task is to retain the best Sitka, create the conditions for change, and work out how to establish both of the other available tree species so that the future forest contains all three.

A second short sentence may explain that the transformation occurs gradually over repeated management cycles.

## Scenario visual

The before/after visual must be high quality.

### Before

Show:

- dense/even-aged Sitka;
- clear plantation structure;
- limited species/structural variation.

### Transformation

Use a clean, restrained process indicator.

Do NOT recreate the previous overlapping:

`Repeated selective management over many years`

icon/text defect.

### Long-term goal

Show:

- Sitka still clearly present;
- both additional species clearly represented;
- multiple sizes/ages;
- regeneration;
- more irregular spacing;
- continuous canopy/forest cover;
- no suggestion of clear-fell replacement.

The target is NOT:

`Sitka plantation → native woodland`

It is:

`even-aged Sitka monoculture → mixed-species, structurally diverse, productive continuous-cover forest`

---

# 8. Images

The final reference images must use forestry imagery suited to Ireland.

Avoid imagery that reads strongly as:

- Pacific Northwest;
- Canadian wilderness;
- giant old-growth North American conifer forest.

The visual character should be credible for:

- Irish plantation forestry;
- Irish Atlantic woodland context;
- managed mixed forest.

Where young planted species are shown, depict the actual additional species used in Scenario One.

Do not depict generic conifer seedlings when the intended planting species are broadleaf.

---

# 9. Visual system dependency

All pages must use Visual System V2 established at `0805ffb`.

Reuse:

- palette;
- typography;
- panel treatment;
- border treatment;
- buttons;
- navigation;
- spacing conventions;
- image framing.

Do not duplicate theme constants.

Do not reintroduce:

- olive-heavy backgrounds;
- mustard/gold heading text;
- heavy borders;
- old modal styling.

---

# 10. Copy density

The objective is not to fit as much information as possible.

Maintain the clean premium visual presentation.

## Page 1

Maximum:

- title;
- two short explanatory sentences;
- one strong progression visual;
- 3–4 short principles;
- optional single-line footer.

## Page 2

May contain:

- brief introduction;
- one process visual;
- approximately 5–6 concise process statements.

Avoid essay-style paragraphs.

## Page 3

May contain:

- starting-condition summary;
- one concise scenario-goal explanation;
- before/process/goal visual;
- 3–5 concise target-state points.

Detailed controls and mechanics belong in contextual teaching after play begins.

---

# 11. Removed content

Remove from the old introduction:

- generic CCF explanation from the Scenario One-specific box;
- `More detail` section/path;
- duplicated CCF background;
- low-quality old transformation diagram;
- overlapping central transformation icon/text;
- ambiguous generic “other species where appropriate” wording.

---

# 12. Controls

This feature does not redesign the full control scheme.

Any separate control research/remapping work remains outside this feature.

Startup navigation controls may be implemented as required for:

- Back;
- Continue/Next;
- close/don't-show-again if still part of accepted startup policy.

Do not silently change gameplay key bindings.

---

# 13. Verification

Do not rerun the complete Scenario One simulation suite unless non-UI behaviour changes.

Required:

## Functional

- New Scenario triggers Page 1.
- Page 1 → Page 2.
- Page 2 → Page 3.
- Page 3 → forest.
- Back works correctly.
- existing-save Continue does not incorrectly replay onboarding.
- no startup/session regression.
- no exceptions.

## Visual

Render:

- Page 1 at 1280×720;
- Page 2 at 1280×720;
- Page 3 at 1280×720;
- Page 1 at 1920×1080;
- Page 2 at 1920×1080;
- Page 3 at 1920×1080.

For every page produce:

`APPROVED REFERENCE | ACTUAL UNITY RENDER`

Side-by-side.

## Forestry-image review

Explicitly verify:

### Page 1

- first image = even-aged Sitka plantation;
- second image visibly advances the transformation;
- third image visibly advances again;
- all three are not near-duplicates;
- added species are not represented as generic Sitka/conifer seedlings.

### Page 3

- all three Scenario One species are visibly represented in the goal state;
- Sitka remains part of the productive forest;
- no clear-fell implication;
- target structure visibly differs from starting plantation.

---

# 14. Acceptance rule

Functional correctness alone is insufficient.

The task passes only if the Unity implementation looks substantially like the approved reference images when viewed side-by-side.

If implementation is technically functional but visually different:

`NEEDS CHANGES`

---

# 15. No agent reinterpretation

Implementers may not:

- rewrite the forestry teaching objective;
- decide species diversification is optional;
- replace three pages with two;
- redesign the approved composition;
- substitute new imagery without recording a deviation;
- introduce a different palette;
- simplify the reference into generic Unity panels.

Material deviation requires Manager review.

---

# 16. Tasks

## ONBOARD-01 — Lock final references

- finalize three page mockups;
- inspect forestry correctness;
- store images;
- hash references.

Exit:

`ONBOARDING REFERENCES LOCKED`

## ONBOARD-02 — Exact copy freeze

Write final production copy into spec/source file.

Do not derive production text from image pixels.

Exit:

`COPY LOCKED`

## ONBOARD-03 — Startup flow implementation

Implement Page 1 → Page 2 → Page 3 → forest.

Preserve Continue behaviour.

Exit:

`STARTUP FLOW READY`

## ONBOARD-04 — Page 1 implementation

Exit:

`PAGE 1 REFERENCE MATCH`

## ONBOARD-05 — Page 2 implementation

Exit:

`PAGE 2 REFERENCE MATCH`

## ONBOARD-06 — Page 3 implementation

Exit:

`PAGE 3 REFERENCE MATCH`

## ONBOARD-07 — Functional qualification

Exit:

`ONBOARDING FLOW PASS`

## ONBOARD-08 — Rendered qualification

Exit:

`REFERENCE FIDELITY PASS`

## ONBOARD-09 — Closure

Final verdict:

`THREE-PAGE ONBOARDING QUALIFIED — READY FOR USER PLAYER TEST`

or:

`NEEDS CHANGES`

or:

`NEEDS PRODUCT DECISION`

---

# 17. Handoff requirements

Return:

- source checkpoint (`0805ffb` or verified descendant);
- final checkpoint;
- reference image hashes;
- exact final copy;
- files changed;
- startup-flow implementation summary;
- Page 1/2/3 rendered comparisons;
- forestry-image verification;
- 720p result;
- 1080p result;
- startup/Continue result;
- any deviations;
- confirmation of no gameplay/save/simulation changes;
- dashboard publication status.

# Final success state

`THREE-PAGE ONBOARDING QUALIFIED — READY FOR USER PLAYER TEST`



# Approved clarification — page mapping (2026-10-25)

The user explicitly locked the mapping: Page 1 uses the CCF definition/progression composition; Page 2 uses the five-card/process composition for generic CCF teaching; Page 3 uses the Scenario One before/transformation/long-term-goal composition. The written spec governs page purpose, order and educational content. Correct navigation and copy, removing Scenario One controls, key prompts, rack instructions and mechanics from Page 2. Do not otherwise redesign the supplied compositions. See references/reference-manifest.md for the binding mapping and unchanged original hashes. Palette/forestry conformance remains unresolved; the final-reference lock is not yet satisfied.


# Approved clarification — styling and forestry (2026-10-25)

The user approved preserving composition/layout/hierarchy/proportions/information structure, applying existing V2 charcoal panels, off-white/neutral text, teal accents, qualified borders/rules and existing green primary buttons. No new onboarding colour tokens. Correct conifer planting stock to Oak/Beech; make Oak/Beech identifiable in the goal forest, retain conspicuous Sitka and all three species. No broader redesign. Final reference hashes and forestry inspection are recorded in the manifest. ONBOARDING REFERENCES LOCKED. Exact production copy is production-copy.md; COPY LOCKED.
