# Scenario One Inspection HUD Declutter V1

**Status: DRAFT — not approved for implementation.**  
**Owner:** Overall Manager #200 (design/specification lane).  
**Date:** 10 November 2026.

## Authority and objective

Latest user instruction > Decision Log > Current Milestone > Game Brief > approved feature specs. This proposal implements the design direction accepted in [the November 10 Manager Fable UI disposition](../../Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/MANAGER-FABLE-52-HUD-AND-MENU-LAYOUT-DISPOSITION-20261110.md) and the earlier #362 design ruling. It does not supersede accepted rack, Work Plan, marking, MFM, annual review, save or forestry rules.

**Player need:** while inspecting a tree, quickly understand (1) this tree's condition, (2) which neighbours contribute to local competition, and (3) what the player's existing *proposed* plan would change; while in the forest, retain useful visible action buttons/keys at the **bottom of the HUD**.

**Out of scope:** gameplay algorithms, forestry coefficients, MFM promotion/approval semantics, new save schema, road/rack topology, global menu-shell rebuild, completion or annual review history rewrite, automatic Fell advice and replacement of the already corrected #363–#370 UI flows.

## Requirements

### FR-1. One decision-focused inspection display

For a valid selected living tree, show one compact default card containing species, relevant Crop/Fell/ordinary mark state, concise size/condition information and current proposed-plan effects when relevant. When a Crop Tree has competition data, show up to the current ranked-neighbour count (no requirement to hard-code five), each with rank, identifier, distance and share; aggregate remaining neighbours explicitly. Use an appropriate ordinary-tree, harvested-stump, planting or MFM review context, rather than assuming every selection is a Crop Tree.

Do not omit MFM candidate/proposal labels or imply those are Fell marks. Do not overlap the objective checklist or obstruct the selected stem and neighbour highlight. There may be a responsive or paged design if one physical card cannot hold everything at 720p. Avoid hard-coded sizes from the 1920×1080 mock-up.

### FR-2. Scientific and simulation semantics

Use current authoritative model quantities only. A neighbour's **share** is a proportion of its local competition index contribution and is **not** the percentage of the target's diameter growth withheld. A phrase such as “54% of potential diameter growth withheld by competition” must explicitly say *potential diameter growth*, and must reflect the source value. Do not imply that the most competitive neighbour is the most appropriate tree to fell.

Use qualitative status for beginners where appropriate. Retain numeric competition before/after, neighbour DBH, ground light, opening, growth, volume, wind, pruning and source/model caveats under [V] More detail or another clearly reachable detail page. Never silently drop material uncertainty or a blocking wind/opening/commitment warning. Distinguish actual changes from display rounding; “no change” is only valid under a defined tolerance. Model-off does not mean “wind-safe.”

### FR-3. Bottom contextual action buttons — direct user preference

**Keep a bottom HUD action bar of useful, visible, button-shaped affordances.** Each item has an accurate key/gesture and short action label, selected by current state; prioritise a manageable number without accumulating a permanent wall of shortcuts.

State examples *only when genuinely available*:

- Crop Tree inspection: `[1–5] Show neighbour`, `[V] More detail`, `[E] Close`.
- Ordinary walking: `[M] Stand Map`, `[O] Objectives`, `[Tab] Work Plan`, plus one applicable context action if space permits.
- Rack-first planning: Stand Map/rack planning or spray cues, but **no [X] Fell or [C] Crop Tree** while the real gameplay stage suppresses them.
- Other states (planting, MFM review, pending Annual Review, help/modal): the actual commands for that state; never promise an action the source rejects.

When cursor-locked in 3D, button-like keycaps are **displayed actionable key prompts**, not falsely advertised mouse-click targets. If actual clickable buttons are offered in a cursor-released UI state, they must call the same existing authoritative handlers and respect disabled/confirmation guards. Keyboard input must remain fully functional; Tab/Esc/focus flow must not be hijacked. Keep the bar at the bottom rather than replacing it with a top-only instruction.

### FR-4. Progressive disclosure

The existing [V] detailed card is cursor-locked and presently cannot scroll. Do not simply pile all removed facts into an overflowing fixed panel. Design a legible bounded layout, optionally with named pages, and ensure all old information classes remain reachable with keyboard inputs. Provide an explicit current-state indication of More/Less detail. Dismissal [E] and selected competitor [1–5] behaviour must remain coherent.

### FR-5. Proposed effects and warnings

Replace the separate dense text forecast strips with a readable plan summary in/near the card, preferably showing existing Fell/Crop marks once, what changed because of *draft* marks and material wind/opening cautions. A forecast is never an authorised committed operation. Do not suggest the player has spent money, harvested or created work without the Work Plan gate. If the model is disabled or data are unavailable, describe that state instead of green “stable” indicators.

Preserve current Annual Review-first/highest-priority navigation and teaching checklist elsewhere in the HUD. Do not duplicate their prose inside the inspection card.

### FR-6. Optional in-world share overlays

**Experiment only, not mandatory implementation.** While inspecting a Crop Tree and showing ranked neighbour markers, evaluate including the share next to markers. Only show the active selection's values, preserve ranks/1–5 navigation, no label clutter behind tree geometry and no implied Fell recommendation. Check overlap/occlusion from a live Unity camera. Maintain equivalent shares inside the card if labels are disabled. A negative readability verdict may omit overlays without failing core card qualification, but this decision must be recorded.

### FR-7. Compatibility and truthful scope

No changes to underlying ecological calculations, tree marks or IDs, rack planning, prescribed MFM coverage, historical saved data, Legacy40, frozen Reference Future v1 or ordinary Work Plan acceptance. Existing annual review source toggle, first-review acknowledgement semantics and computed preapproval contractor quote are **already implemented** in the inspected candidate; do not redo or replace them under this spec.

### FR-8. Compact top-right global status strip — explicit current-state constraint

The player requests a **small top-right status display** using information already supported by the simulation: **current Year and cash**, with an optional compact **current browsing-pressure band** or game phase if they fit and help. Reconcile existing HUD values rather than creating a second bulky panel. The status strip must be quiet, accurate to current game state, non-duplicative and legible at 720p alongside the objective cue, inspection card and bottom action keycaps.

The player considered a numerical biodiversity-related indicator but explicitly ruled that **nothing speculative or dependent on future modelling belongs in the HUD**. The current game tracks particular ecological facts, not an endorsed all-purpose numerical biodiversity metric. Therefore **do not add biodiversity, habitat, ecological-health or fabricated composite scores or substitute a token numeric proxy**. No blank placeholder is needed: omit the item entirely.

Existing simulation-supported facts about regeneration, deadwood and browsing can remain in the appropriate detailed/contextual views, with their proper quantities, meaning and provenance. The ecology metric question is **deferred outside this HUD iteration**, with **no scoring investigation, model implementation, schema work or dependency for approval**. Reconsider only upon a later, separately grounded decision that a useful metric can be calculated from the then-existing simulation state.

## Accessibility, layout and verification requirements

- Test real Unity renders at **1280×720 and 1920×1080**, colour and grayscale; no hard-coded or stitched screenshot counts as a rendered pass.
- Check cursor-locked operation, keyboard only, reduced-motion/static cues, screen-edge visibility, reasonably long species/ID text, tree and marker occlusion.
- Exercise a valid Rack-first state, an ordinary inspected tree, a Crop Tree with many ranked competitors, a Fell-marked target, MFM candidate/proposal, a stump, changed and unchanged plan, storm model on and off, and real warning/limit conditions.
- No broad full-game Tier 3 pass for a HUD presentation feature. Tier 1 source checks during work; **one focused Tier 2 feature qualification** at handoff, preserving targeted old-save and protected reference compatibility.
- Strict negative cases: no Fell/Crop shortcut prematurely, no automatic felling or commitment, no false share/diameter-growth equivalence, no missing [V] detail, no false stable/safe status, no click-only escape.
- Do not weaken historical verification to make the new UI green. Exact protected candidate and source-diff evidence required.

## Open design decisions before APPROVED

1. Resolve how many ranked neighbour entries fit without overflow at 720p; choose compact rows versus keyboard-paged sections using a real layout proof.
2. Choose button-bar priority and responsive overflow behaviour for the most crowded inspection and wayfinding states, ensuring keys remain truthful.
3. Decide after real Unity rendering whether on-tree share overlays are legible enough to include.
4. Choose which plan effects qualify as “material” and the rounding/tolerance wording; preserve relevant warnings.
5. Confirm whether clickable controls in cursor-released modes add value or whether button-shaped keycaps suffice for this first pass; no new input behaviour can be assumed without explicit approval.

**Implementation gate:** Only after spec approval, exact current-source repin, live file ownership check and a bounded task claim. The unrelated global four-menu shell and exact Annual Review tiles need separate future design scope and should not block this feature.
