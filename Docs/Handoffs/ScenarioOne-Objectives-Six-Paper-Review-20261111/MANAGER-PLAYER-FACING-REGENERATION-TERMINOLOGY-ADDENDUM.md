# Scenario One — Player-facing regeneration terminology

**Status:** Proposed wording from direct product-owner feedback, 11 November 2026. Not yet a production UI change or a redefinition of completion rules.

## Problem

“Regenerating cells” exposes the simulation's internal spatial bookkeeping as if it were a real forestry unit. Regeneration itself is a valid forestry concept, but a woodland owner sees young growth, seedlings, saplings, planting locations and areas with regeneration—not grid cells.

The current Scenario One objective's machine-readable identifier is `regeneration`; its current wording is “Maintain regenerating cells”. The evaluator compares `snapshot.occupiedRegenerationCells` to the area's calibrated minimum. In Enlarged80, the configured requirement is 12. Source: `Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs`, `ScenarioOneManager.cs`, and `ScenarioOneDefinition.cs`.

The snapshot's underlying support is an artificial 5 m × 5 m cell lattice. A cell with regeneration cohorts can count; a cell otherwise empty of regeneration but containing a live exact planted juvenile can also count. This is **presence in tracked map sections**, not a measured number of individual saplings, square metres successfully regenerated, or surviving mature trees.

## Preferred player language

- **Objective name:** “Encourage woodland regeneration”.
- **Progress label:** “Areas with young trees” (e.g. “12 of 12”).
- **Explanation:** “These are parts of the woodland where young trees are currently present, either naturally regenerated or planted. Their presence does not guarantee long-term survival.”
- **Annual Review:** “Young trees were recorded in [current] parts of the woodland, compared with [previous] the year before” — *only if equivalent year-stamped states actually support the comparison*. Do not invent a historical difference.
- **Inspection:** say “young Sitka”, “young Oak”, “young Beech”, “naturally regenerated seedlings/saplings”, or “planted young trees” where the game's recorded species, origin and stage justify those exact descriptions.
- **Technical layer:** keep existing `occupiedRegenerationCells`, spatial cell indices, save fields and stable objective IDs, unless a separate approved specification changes their semantics.

## Terms to avoid in the player UI

- “Regenerating cells”, “occupied regeneration cells”, “ecology cell counts”: internal-only.
- “Regeneration coverage”: suggests measured spatial area/continuous cover that a count of occupied grid units does not provide.
- “Established young trees”: implies establishment/survival evidence, not necessarily proven by a present juvenile.
- “Successful regeneration”: requires an explicit success criterion beyond an instantaneous presence count.
- “Regeneration patches”: implies actual coherent spatial groups; neighbouring tracked cells need not represent connected real patches.
- “Regeneration sites”: acceptable only for a deliberately player-selected/site-defined treatment location, not as a blanket synonym for every grid unit.

## Optional improved presentation, not part of this wording-only proposal

Later the report could distinguish **natural regeneration** from **new planting**, and possibly young-tree survival or cohort progress across years. This requires inspection of what historical data are genuinely recorded, avoids double counting and may need an approved spec. Do not imply the existing 12-unit target is an empirically supported Irish silvicultural standard.

## Scope and verification

This proposal changes the **description**, not the live scoring threshold or any world state. A future bounded UI/content task should inventory all strings used in objective screens, history, glossary, work-plan teaching and Century Review, then ensure consistency. Test Legacy40/Enlarged80 and old-save presentation. Do not rename durable objective IDs or fields casually; protect frozen Reference Future v1. Any metric change is a separately authorised product and persistence decision.

**Product choice still open:** whether “Areas with young trees” is the final preferred wording or whether the user wants the objective to measure something different from the current location-presence proxy.

**Dashboard:** no sync/readback verified.
