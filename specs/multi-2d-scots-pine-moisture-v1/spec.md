# Scots Pine and Site Moisture V1

- **Spec ID** `SPEC-multi-2d-scots-pine-moisture-V1`
- **Status** APPROVED / IMPLEMENTING
- **Owner** Overall Manager
- **Decision** Owner-approved 2026-11-27, Option A locked
- **Task class** SPEC REQUIRED

## Product intent and scope

Give the 2D lab species-specific responses to site moisture and make Scots pine a distinct management choice, from timber quality to local seed in canopy gaps. Part 1 delivers callable computation and offline evidence. Part 2 is **QUEUED behind Playable Milestone 1** and will connect these rules to AnnualPipeline, MultiAreaContinuation, ForestMaker, Version 3 saves and `/multi` UI under a separate packet.

## Option A requirements

- **FR-1 — Site moisture.** Apply the existing Wave 2 table to all six species. Return the establishment and growth modifiers separately. `NeutralLegacy` is exactly (1.0, 1.0) for every species. Select a whole-woodland class by summing area weights per class and taking the class with the greatest total. Use declaration order for equal totals and `NeutralLegacy` for an empty or all-zero input. The Dry Ridge & Moist Slope map splits along the midpoint of the stand's x-axis bounds. The half below the midpoint is `DryFreeDraining`. The midpoint and the half above it are `FreshMoistMineral`. Include the exterior bounds. A selection of `StagnantPoorBog` always carries `HabitatProtectionCaution`.
- **FR-2 — Scots pine timber.** Teaching prices are EUR 68/t for Sawlog (band 50–85), EUR 45/t for Small Roundwood (30–60) and EUR 38/t for Pulp/Chip (25–50). Use 1.02 green t/m³ and EUR 0.45 per sapling for material only. A mature stem qualifies as Sawlog when it reaches the pruning target or has a recorded tight early stocking history. Use the start of the 5–6 m target as the qualifying height. Downgrade other stems to Small Roundwood or Pulp/Chip according to their suitability for Small Roundwood. Return the sortment, unit price, harvested volume, green mass, gross timber value and a plain reason.
- **FR-3 — Scots pine biology and local seed.** Reuse `FutureSpeciesCatalog.ScotsPine` directly. Its crown width is `0.6874 + 0.2358D - 0.0011D²` metres, with radius half the width and diameter capped at 58.6 cm. Retain `Ci50` 4.75, LightModerate shade casting classification, High light demand, MediumHigh deer browse sensitivity, 5–6 m pruning target and `AuthoredOnly` external colonisation. Adult annual DBH growth is a provisional teaching shape within the Irish GROWFOR envelope of 10–60 years, 100–5,000 stems/ha, 10–100 m²/ha basal area and top height at least 5 m. Return unavailable growth outside that domain. Local regeneration requires an on-map seed tree at least 18 years old, a good seed year, canopy openness of at least 0.60 and undergrowth cover up to 0.30. Schedule good seed years at intervals of 3–5 years using an internal dedicated salt and a stable woodland seed. Return the site's establishment modifier when eligible. Seed rain from outside the map is excluded.
- **FR-4 — Integration boundary.** Provide immutable requests and results through a small .NET 8 API. Part 1 has no I/O, ambient randomness or clock. Existing runtime entry points and save payloads receive no Part 1 changes.

## Authority and evidence

The latest owner instruction, Decision Log, Current Milestone and Game Brief govern intent in that order. The checked-out implementation and verified Unity Project Overview govern current behaviour. For this task the approved Option A packet defines its implementation scope. Apply the D-059 evidence boundary.

| Input | Evidence and use |
| --- | --- |
| Moisture tier assignments and multipliers | Existing **[Calibration]** design table. On `DryFreeDraining` Scots pine alone leads both columns. On `FreshMoistMineral` Sitka shares the lead with the five other species in both columns. |
| Crown, competition and silvicultural traits | Reuse the profile's Turkish open-grown fit **[Transfer]**, `Ci50` **[Calibration]** and qualitative traits in the existing card. |
| Adult growth | Irish GROWFOR domain from the research index and profile card. The age and crowding shape and rates are **[Calibration]**. |
| Local regeneration | Local seed and light requirements follow the card. The age, seed schedule and opening/competition thresholds are **[Calibration]**. |
| Timber | Prices, bands, grading threshold use, green-density conversion and sapling cost are used as **[Calibration]** teaching inputs. The underlying Scots pine density entry retains its original Empirical/Inference tag. |

## Save compatibility, write-back and exclusions

Part 1 has no save impact, persistence or external write-back. Part 2 must specify how Version 3 preserves the default `NeutralLegacy` on old saves and protects Scenario One and frozen Reference Future v1. Physical planting approval, empirical yield predictions, seedlings or trees created by the API, changes to existing Wave 2 sources or RNG salts, runtime wiring, packages, Unity, browser verification and P0 are outside Part 1.

## Acceptance criteria

- **AC-1** All six species retain bit-exact neutral identity. Active cells agree with the existing table and preserve separate modifiers. The dry lead and fresh moist tie are visible.
- **AC-2** Weighted selection is deterministic, the two-zone map covers the stand with a documented midpoint rule, and only bog selections carry habitat caution.
- **AC-3** Regeneration is blocked without eligible local seed, a good seed year, enough light or low enough undergrowth. Repeated requests agree, successive good years are 3–5 years apart and eligible gaps return the site multiplier.
- **AC-4** Both mature Sawlog paths and both downgrade outcomes report the Scots pine price, band, mass and value. Material cost uses EUR 0.45 per sapling.
- **AC-5** Biology reads from the existing card. Growth is available only inside the stated domain, with separate moisture-adjusted and potential increments tagged as calibration.
- **AC-6** Pinned .NET 8 console restore, build and execution succeed with zero build warnings/errors, zero failed checks and matching canonical SHA-256 digests across separate runs. All tracked changes fall within the four authorised prefixes.
- **AC-7 (Part 2, queued)** Verify annual growth, regeneration and work/economy outcomes, woodland generation, new and old save compatibility, and feedback in `/multi` on the integrated candidate.

## Stop conditions

Stop for a conflict in product authority, a material ambiguity in a locked behaviour, a need to edit outside the four authorised paths, required Part 2 wiring, unapproved dependencies, a threatened protected anchor or an unresolved failure of the pinned offline gate. No additional Part 1 product decision is required.
