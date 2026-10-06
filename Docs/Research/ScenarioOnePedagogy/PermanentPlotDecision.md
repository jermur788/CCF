# Whole forest vs sample plot (Workstream E2)

**Status:** decision analysis with a recommendation [INF]. **PRODUCT DECISION REQUIRED** for anything beyond Phase 1.

## Options

**A. Whole-property annual diary.** Automatic. Built from existing saved snapshots, reports and events (`ForestDiaryConcept.md` §1).

**B. One or a few permanent sample plots.** The player places a plot (e.g. one 5 × 5 m cell, or a 3 × 3 cell block). The game records detailed tree and regeneration measurements there at intervals (e.g. every 5 years), like AFI/ANW plots [PRAC §17].

**C. Hybrid.** Automatic annual stand summary (A), plus optional detailed sample plots (B).

## Evaluation

Scores: ● strong, ◐ moderate, ○ weak.

| Criterion | A whole-property | B sample plots only | C hybrid |
|---|---|---|---|
| Gameplay value | ◐ shows trends; little personal ownership | ● the player chooses where to watch; strong place attachment; matches "walk back to the site" | ● both |
| Teaching value | ◐ capital, structure and money over time | ● individual-tree release, regeneration by place, browse outcomes; the practitioner method itself [PRAC §17.2] | ● |
| UI complexity | ○ low (a tab + a timeline) | ◐ plot placement, measurement view, comparison | ◐ (two views; plots optional) |
| Save cost | ● **zero** (Phase 1 derives from existing saves) | ◐ new records: about 30 trees × ~8 fields × up to 20 measurements per plot over 100 years. Small in bytes, but a **schema change** (v17+) | ◐ (only plots add data) |
| Implementation difficulty | ● low: presentation over saved data | ◐ moderate: placement UI, a measurement event, save/validation, determinism of the recorded data | ◐ (two increments) |
| Risk to determinism/Reference | ● none (read-only) | ◐ save-validation and Reference-preview isolation needed | ◐ |
| Overlap with Sol's work | ● none | ○ **save schema is single-writer**; Sol's growth-model versioning may change the save | ◐ (Phase 2 must sequence after Sol) |

## Recommendation: **C, in two phases**

1. **Phase 1 (no save change):** the whole-property timeline, place history and intervention comparison, derived from existing saved data. Ship first. It satisfies most of E1 and E3.
2. **Phase 2 (save change, after Sol's integration):** optional permanent sample plots. Recommended initial scope:
   - at most 3 plots, each one map cell (5 × 5 m);
   - measured automatically every 5 years (and at placement);
   - per tree in the plot: id, species, DBH, height, crown radius, Crop/Fell mark, alive or dead;
   - per plot: regeneration bands by species (density, max height), planted stems' heights and protection, deadwood count/volume;
   - all values copied from authoritative state at the measurement year. **No new ecological variable.**

Why not B alone: without the automatic summary, players who never place a plot get no history at all, and the plot's value depends on choosing it well before they understand what to watch.

Why plots at all: they answer the questions the whole-stand view cannot — "how did *my* Crop Tree respond?", "when did regeneration start *here*?" — and they teach the real AFI/ANW method of repeated measurement plus recorded interventions [PRAC §17.2]. They also cover the missing per-tree growth history (`RepeatedInterventionDesign.md` §3.4) for chosen places, without saving every tree every year.

## Save implication summary

| Phase | Save schema | New fields | Validation | Reference Future v1 |
|---|---|---|---|---|
| 1 | unchanged | none | none | untouched; the diary is not shown in Reference preview (or shows the archive's own history read-only) |
| 2 | **new version** (after Sol's version) | `samplePlots[]` with measurement lists | ForestSaveValidation bounds (≤3 plots, ≤N trees per measurement) | untouched; v12 archive has no plots |

## Open questions for the product owner

1. Should plots be per forest (save) only? **Recommended:** yes.
2. Should the 5-year interval be a scenario setting? **Recommended:** a definition field with default 5, calibration [D], not a forestry rule.
3. May a plot be moved? **Recommended:** no. Re-placing creates a new plot and keeps the old one's history; that is the permanent-plot principle.
