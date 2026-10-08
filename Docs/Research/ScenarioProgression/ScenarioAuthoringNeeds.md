# Scenario authoring, sandbox and training mode

**Status:** RECOMMENDATION. Covers task Parts 17–19. Implementation facts are from canonical docs and the expansion audit at `341ccbf`; no code was changed or run.

## 1. Relevant implementation facts

- The Scenario One start comes from a hard-coded starting stand (`ForestStartingStand`, per the expansion audit [REPO]); scenario identity is Scenario-One-specific throughout (`ScenarioOneManager`, `ScenarioOneDefinition`, `ScenarioOneObjectives`, Scenario One save data).
- The save captures the full authoritative world (trees, regeneration bands, understorey covers, juveniles, shelters, deadwood, management events, model selectors); save v19 at main. Old saves keep their recorded model versions (RNG, regeneration, growth, storm) — D-046 to D-050.
- `ScenarioReferenceArchive.Matches()` couples the Reference Future to Scenario One's `definitionVersion` (Scenario 1 plan review). Reference Future v1 is frozen (D-042).
- No scenario selection front end exists (`PostScenarioRoadmap.md`).
- Annual step cost grows super-linearly with tree count: ≈ 37 ms (336 trees), 273 ms (1,300), 766 ms (3,000), 1,515 ms (5,000) (expansion `ImplementationWaves.md` §2).

## 2. Minimum architecture for Scenario Two (Part 17)

The expansion proposal's "scenario-start description (stand table by species/size/position rules + site map + bands)" is the right **eventual** format, but Scenario Two does not need it. Scenario Two needs an inherited forest whose history is *causally real*. The cheapest honest way to get that is to let the existing engine produce it.

### 2.1 Scenario package v0

A **scenario package** = a small data asset containing:

| Field | Purpose | Needed for S2? |
|---|---|---|
| `scenarioId`, display name, briefing text | Identity and the opening story ("You have bought…") | Yes |
| **Start snapshot** | A save file at the current schema, produced by an authoring harness (§2.2) | Yes |
| `inheritedHistoryBeforeYear` | Events before this year are shown as "previous owner", not the player's | Yes |
| Definition overrides | Economy config (cash, minimum within accepted ranges), browse pressure, storm model | Yes (values per scenario) |
| Assistance profile | Per-concept tiers (`ProgressiveAssistance.md` §3) | Yes |
| Completion profile id | Which completion rules apply | Yes |
| Previous owner's notes | Short authored text attached to areas/years (story, not state) | Yes (text only; must not contradict the snapshot) |
| Site map, species roster | S3 onward | No |

### 2.2 Authoring by simulation, not by hand

A deterministic **authoring harness** (Tools-side, never shipped) builds a start snapshot by:

1. starting from a stand (for S2, Scenario One's existing starting stand or a variant of it at a different seed);
2. running a scripted "previous owner" plan through the real Work Plan and annual steps (good release in one area, thinning from below in another, over-opening in a corner, partial sheltering);
3. stopping at the inherited year and saving.

Benefits: stumps, brash, deadwood, bramble response, browse damage, advance regeneration and the event history are all genuine simulation results (D-020 satisfied); no new ecology; the same harness can produce B1 (with the storm model forced on during authoring) and the Training Stand.

### 2.3 Required small extensions (Scenario Two)

1. **Scenario id in the save** (next free save version; missing field → Scenario One). Needed so a loaded game knows which package's completion and assistance rules apply.
2. **Start-from-package path**: new game for a package = restore its snapshot, then re-stamp it as a new playthrough (year offset, inherited-history boundary). Uses the existing restore path.
3. **Inherited-history boundary** in history/diary presentation.
4. **Per-scenario completion profile**: generalise `ScenarioOneObjectives` just enough for a second rule set (after P6 settles Scenario One's model).
5. **Reference Future isolation**: non-Scenario-One packages hide the Century Review/Reference preview; `Matches()` must keep rejecting them.
6. **Scenario select screen**: minimal list with briefing text.
7. **"No work needed here" record** (a saved decision event; already suggested in the readiness research).

Not needed for S2: site map, species roster, stand-table format, general editor, terrain variation (S2 can reuse the existing ForestTest terrain with a different property boundary or orientation — open decision, since a visually identical place may confuse players into thinking it is the same forest).

### 2.4 Maintenance consequence (important)

A snapshot records the model versions it was authored under, and the project's save rules keep those versions on load. A package authored at growth 1 / regeneration 2 therefore **stays** on those models even after a new default model is accepted. That is stable and deterministic, but every new default model requires a deliberate decision per package: regenerate the snapshot (re-run the authoring harness under the new stack and record new anchors) or keep the old models. Record one anchor per package.

### 2.5 When the general format becomes necessary

| Scenario | Authoring need |
|---|---|
| S2, B1, Training Stand v1 | Package v0 + authoring harness |
| **S3** | Harness can no longer start from Scenario One's stand: needs a **stand generator** (species/size/position rules) and a **site map**. This is when the expansion proposal's description format earns its place, as data consumed by the harness |
| S4–S6, B2 | Same generator with richer inputs (cohorts, deadwood, browse pressure, compartments) |
| Sandbox | The same format exposed through a UI |

Rule: **generalise when the next scenario needs it.** Do not build the S3 generator while S2 is unfinished.

## 3. Sandbox (Part 18)

The sandbox is the scenario package format with a choosing UI: site template, starting-stand template (from the generator), species pool, browse pressure, storms, economy preset. It must reuse scenario machinery and contain no simulation logic of its own.

**Build gate (all):**

| # | Gate |
|---|---|
| SB1 | The stand generator and site map have authored at least three shipped scenarios (S3, S5, S6) |
| SB2 | Species in the pool are calibrated at the depth the sandbox exposes. Species with only ordinal evidence may appear only in roles the model supports (e.g. birch as a pioneer, not as a sawlog crop); uncalibrated species are **absent**, not merely warned about |
| SB3 | Site system active with all three axes |
| SB4 | Economy supports more than one credible strategy (species assortments, or honest "no market" rows) |
| SB5 | **Combination guard**: species × site × starting-structure combinations that produce unmodelled or meaningless states are excluded (e.g. a stand type with no density evidence at that size) |
| SB6 | Performance budget enforced (maximum tree count per area), from measured annual-step cost |
| SB7 | Scenario 6 exists, proving the format can express a realistic multi-compartment holding |

**Player unlock:** after completing Scenario 5 (graduation). Before that, a sandbox would expose a player to unexplained systems without the judgement to use them.

## 4. Marteloscope / training mode (Part 19)

Builds on `MarteloscopeFinalDesign.md` (PROPOSAL): "B-lite now, C later", and its correction that practising on a time-advanced copy of the player's own forest is an oracle under a fixed seed.

| Element | Placement | Notes |
|---|---|---|
| **Plan comparison** (store up to 3 mark sets, compare P2/P3 summaries, no time advance) | **Inside every scenario**, from Scenario 1 (after P3) | Not an assistance feature — real foresters compare marking options. Stays at all tiers |
| **Training Stand mode v1** (authored deterministic stand, five exercises, reset, no score) | **Unlocked after Scenario 1**; built with Scenario Two's package format and authoring harness | Build alongside or immediately after Scenario 2: same enabling work, different content |
| Species/site exercises | After Scenario 3 | Needs W0/W1 |
| Quality/form exercises | After Scenario 5 | Needs quality state |
| Professional/practitioner comparison | Later, optional | Only if real practitioner marking data is collected (playtest group D). Show as "one practitioner group marked…" commentary, never as the correct answer |

Embedding training inside scenarios as compulsory steps is not recommended: it interrupts the core loop and becomes a gate.
