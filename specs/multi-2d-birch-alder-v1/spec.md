# Downy Birch and Common Alder V1

- **Spec ID** `SPEC-multi-2d-birch-alder-v1`
- **Status** APPROVED / IMPLEMENTING
- **Owner** Overall Manager
- **Decision** Owner-approved 2026-11-27, Option A on all four choices
- **Task class** SPEC REQUIRED
- **Created** 2026-11-27
- **Last updated** 2026-11-27
- **Milestone** Playable Native Tree Expansion, Part 1 (engine and pricing)
- **Related evidence** `specs/multi-2d-species-wave2-profiles-v1/`, `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/FutureSpeciesCatalog.cs`, `SpeciesMoistureTable.cs`
- **Supersedes / superseded by** none

## Product intent

Give the 2D simulator its two native wet-site broadleaves so that a wet valley is a real silvicultural choice rather than a Sitka spruce stand with a different label. Downy birch is the fast pioneer that loses diameter when it is crowded. Common alder is the alluvial specialist that grows very fast early, then slows sharply and becomes a heart-rot risk from about 50–60 years. Neither is marketed as a premium timber in normal play.

Part 1 delivers callable, offline-tested computation and pricing. Part 2 is **QUEUED** behind Playable Milestone 1 and behind Scots Pine Part 2, and connects these rules to `AnnualPipeline`, `MultiAreaContinuation`, `ForestMaker`, Version 3 saves and `/multi` one species at a time: **Downy birch first, then Common alder**.

## Problem

The integrated Soil Moisture Table and profile cards already describe Downy birch and Common alder, but nothing acts on them. There is no wet-valley layout helper, no growth or local-regeneration rule for either species, and no timber value at all. A player cannot yet see that alder belongs on moving alluvial ground, that alder on stagnant bog is a habitat caution rather than a crop, or that an old alder stem may be worth only firewood. Without that, "add two more native trees" would be a relabelled Sitka stand, which D-059 explicitly rejects.

## Goals

- Make both species computable from the existing Wave 2 foundations with no duplicated vocabulary.
- Provide a two-zone **Wet Valley & Slope** layout helper ready for the Part 2 0.64-hectare preset.
- Make Downy birch growth visibly pioneer-flush then competition-limited.
- Make Common alder visibly fast-then-slow, wet-site dependent, and heart-rot downgraded with age.
- Price both species honestly: one provisional bulk band, quality stems genuinely unpriced.
- Keep the Part 1 API pure, deterministic and free of I/O, clock or ambient randomness.

## Non-goals

- No wiring into the annual loop, `MultiAreaContinuation`, `ForestMaker`, `/multi`, Version 3 saves or any UI.
- No new save fields or migrations.
- No change to `RngSalts.cs`, existing Wave 2 files, the parked Scots Pine module, or any Unity asset.
- No claim of Irish empirical yield, maximum-density or price relationships for either species.
- No premium/veneer market, no buyer simulation, no sawn-product grading, no defect or taper model.
- No tree creation, no dispersal-distance model, no mortality rule for alder heart rot.

## Player workflows

Not applicable in Part 1. Part 2 will expose the workflow: choose the wet-valley preset in the New Woodland picker, see the two moisture zones on the map, plant or retain birch and alder where they belong, read the habitat caution on stagnant ground, and see bulk value plus a `Quality stem — no local specialist buyer` note on inspection. This specification records the intent so Part 2 is not designed from scratch, but Part 1 claims no player-facing behaviour.

## Functional requirements

- **FR-1 — Reuse, do not duplicate.** Reuse `SiteMoistureClass`, `SpeciesMoistureTable` and `FutureSpeciesCatalog.DownyBirch` / `.CommonAlder` directly. Do not copy the moisture classes, the table rows, the profile cards or the crown models. Do not duplicate types owned by the parked Scots Pine lane (`SiteMoistureEngine`, `MoistureArea`, `MoistureSelection`, `SiteMoistureResult`, `DryRidgeAndMoistSlopeMap`); Part 1 reads the table through `SpeciesMoistureTable` and returns the table's own `MoistureCell` and `SiteResponse`. Every result carries an evidence tag. `NeutralLegacy` is bit-exact (1.0, 1.0) for both species. A `StagnantPoorBog` result always carries `HabitatProtectionCaution`.
- **FR-2 — Wet Valley & Slope two-zone layout helper.** Provide a pure layout helper for the standard 0.64-hectare stand: 80 × 80 m, five equal 16 × 80 m lanes. One half is `WetMovingAlluvial` (valley bottom, where alder thrives) and the other half is `FreshMoistMineral` (slope, where Sitka and birch compete). The split is on the **y** midpoint so lanes stay intact: y below the midpoint is `WetMovingAlluvial`; the midpoint and everything above is `FreshMoistMineral`. Bounds are inclusive. Sampling outside the stand and non-finite input are refused. `NeutralLegacy` woodlands are never affected, because the helper only exists where a caller asks for it.
- **FR-3 — Downy birch biology and local seed.** Reuse the Downy birch card unchanged: high light demand, `Light` shade casting, `MediumHigh` deer browse sensitivity and form sensitivity, `AuthoredOnly` external colonisation, local on-map seed allowed, quality-stem inspection at 30–35 cm DBH.
  - **Growth.** A provisional teaching shape: a fast juvenile flush that moderates in adulthood, and a crowding reduction that is stronger than a light-demanding conifer's. Diameter growth is strongly suppressed by competition. Return `null` (unavailable) outside the declared provisional teaching domain, and keep the domain labelled as gameplay calibration, not a readiness domain.
  - **Local regeneration.** Requires an on-map seed tree at least 12 years old, a good seed year, bright opening (canopy openness at least 0.65) and low competing vegetation (undergrowth at most 0.35). Eligible gaps return the site establishment multiplier. Seed rain from outside the map is excluded.
- **FR-4 — Common alder biology, local seed and heart rot.** Reuse the alder card unchanged: high light demand, `Moderate` shade casting, `LowMedium` deer browse sensitivity, self-pruning, thinning more important than pruning, water-assisted local dispersal allowed, `AuthoredOnly` external colonisation, heart-rot inspection onset 50–60 years.
  - **Growth.** Very fast early juvenile growth followed by a pronounced adult slowdown, and crowding suppression slightly weaker than birch's because the species is a wet-site pioneer rather than a dry competitor. Domain and availability behave as in FR-3.
  - **Local regeneration.** Requires an on-map seed tree at least 10 years old, a good seed year, adequate light (canopy openness at least 0.55), high moisture (`WetMovingAlluvial`, `FreshMoistMineral` or `SeasonallyWetAerated`; refused on `DryFreeDraining` and `StagnantPoorBog`) and exposed, low-competition mineral ground (caller-supplied `ExposedMineralGround` true and undergrowth at most 0.40). Bog eligibility is refused even though the card's alder persistence on bog is limited rather than impossible, because the table's habitat caution forbids encouraging a bog crop.
  - **Heart rot.** From age 50, a rising deterministic per-stem risk downgrades a felled alder stem from its nominal bulk grade to firewood/pulp. The probability rises linearly from 0 at age 50 to the declared maximum at age 60 and stays there. The roll is a pure hash of a caller-supplied stable stem identity and the stand age under a dedicated salt declared inside this module. It never kills or removes a living tree, and it must not be presented as a mortality rule.
- **FR-5 — Timber grade and pricing.** Both species use one provisional bulk price of **EUR 42 per green tonne** (label `[C]`, a rounded value selected from the archived EUR 25–60/t sensitivity band, whose arithmetic midpoint is EUR 42.50), with the card's green density: Downy birch **0.93 t/m³**, Common alder **0.842 t/m³**. Sapling material cost is **EUR 0.95 per tree** for both; birch is `[E]` (None-So Hardy 2025–26 wholesale) and alder is `[C]` provisional broadleaf parity. A mature, straight, sound Downy birch stem at 30–35+ cm DBH, or a mature well-released clean-bole alder toward 50–60 cm DBH, is reported as a **quality stem**: it earns the bulk price only and carries `Quality stem — no local specialist buyer` with `QualityStemUnpriced` true. Quality stems are never given a premium price. Return sortment, unit price, price band, volume, green tonnes, gross value, flags and a plain reason. Reject negative volume or non-finite inspection input.
- **FR-6 — Integration boundary and sequencing.** Immutable requests and results through a small .NET 8 API. Part 1 has no I/O, ambient randomness or clock, and touches no existing runtime entry point or save payload. Part 2 is sequenced **Downy birch first, then Common alder**, behind Scots Pine Part 2, each with its own implementation packet and integrated acceptance.

## Authority and source of truth

- **Accepted product intent.** The latest owner instruction (2026-11-27, Option A on all four choices), the Decision Log (D-059 evidence boundary; D-062 broadleaf economy), the Current Milestone and the Game Brief, in that order.
- **Implementation facts.** The inspected repository at base `24871551ae906b6911b48bb17c252c023bdea502`: `FutureSpeciesCatalog` already carries both cards; `SpeciesMoistureTable` already carries both rows.
- **Supporting evidence.** Card notes and the Wave 2 profiles specification; the archived EUR 25–60/t broadleaf sensitivity band.
- **Spec authority.** Approved and task-level implementation authority for Part 1 only, subordinate to the project authority hierarchy above.

## Evidence classification

| Input | Class | Note |
| --- | --- | --- |
| Moisture tiers for both species | Calibration | Existing authored table, unchanged. |
| Birch green density 0.93 t/m³ | Empirical / Inference | UK generic birch fresh-felled average. The Irish 2026 birch structural study excluded Downy birch from its primary analysis, so this is not an Irish Downy birch measurement. |
| Alder green density 0.842 t/m³ | Transfer | Latvian fresh-cut average; lower confidence, stated as such. |
| Bulk price EUR 42/t | Calibration | Provisional teaching figure rounded from the EUR 25–60/t band; its arithmetic midpoint is EUR 42.50. Never a Sitka borrowing. |
| Birch sapling EUR 0.95 | Empirical | None-So Hardy 2025–26 wholesale catalogue. |
| Alder sapling EUR 0.95 | Calibration | Provisional broadleaf parity; no Irish alder nursery quote was supplied. |
| Quality-stem inspection sizes | Calibration | 30–35 cm birch, 50–60 cm alder; teaching triggers, not market specifications. |
| Birch pioneer-then-moderate growth shape and crowding strength | Calibration | Provisional teaching shape. Irish YC8 potential is context, not a universal assumption. |
| Alder fast-then-slow growth shape | Calibration | Provisional teaching shape. |
| Seed-tree ages, openness, undergrowth, mineral-ground gate | Calibration | Teaching thresholds. |
| Alder heart-rot onset 50–60 and maximum probability | Calibration | Flag and downgrade only. No mortality, no disease model. |
| Two-zone map split | Calibration | Layout convention for the Part 2 preset. |

No value in this specification is presented as an Irish empirical growth, density or price relationship for these species.

## Save / backward compatibility

No save impact. Part 1 adds no fields, reads no save and changes no schema. Existing woodlands keep `NeutralLegacy` (1.0, 1.0) by default, and Scenario One, `Legacy40`, `Enlarged80` and frozen Reference Future v1 are untouched. Part 2 must specify how Version 3 persists species and moisture state and how old saves and Version 2 saves default to `NeutralLegacy` without fabricating a zone.

## Security / write-back

Not applicable. Part 1 is a pure in-process computation with no input parsing, no file access, no network, no write-back and no destructive operation. Validation is limited to argument guards that refuse negative volumes, out-of-range fractions and non-finite values.

## Acceptance criteria

- **AC-1** Both species keep bit-exact `NeutralLegacy` identity; active cells equal the existing table; a bog selection always reports the habitat caution; no foundation type is duplicated.
- **AC-2** The Wet Valley & Slope helper covers the 80 × 80 m standard stand, splits on the y midpoint with the midpoint in `FreshMoistMineral`, keeps all five 16 × 80 m lanes intact, and refuses out-of-stand and non-finite samples.
- **AC-3** Downy birch growth shows a higher juvenile than adult potential, a strong crowding reduction, and a single application of the site growth multiplier; it is unavailable outside the declared teaching domain. Regeneration is refused without local seed, in a poor seed year, in a dark opening or under high competing vegetation, and returns the site establishment multiplier when eligible.
- **AC-4** Common alder growth shows a higher juvenile than adult potential and a pronounced slowdown; regeneration follows AC-3's rules plus the moisture and exposed-mineral gates, and refuses bog. Heart-rot probability rises from 0 at age 50 to the declared maximum at 60+, is stable for the same stem identity and age in any order, and never appears before age 50.
- **AC-5** Both species return EUR 42/t bulk, their own card density, EUR 0.95 per sapling, bulk mass and value; a quality stem is flagged, unpriced and carries the buyer note; a heart-rot-downgraded alder stem is reported as firewood/pulp with a plain reason and no premium.
- **AC-6** Pinned .NET 8 restore, build and run succeed with zero warnings and zero errors, zero failed checks, and identical canonical SHA-256 digests across separate processes. Every tracked change lies inside the four authorised prefixes.
- **AC-7 (Part 2, queued)** Verify annual growth, regeneration, work and economy outcomes, generation presets, Version 2 and Version 3 save compatibility, and `/multi` feedback on an integrated candidate, Downy birch first and then Common alder.

## Stop conditions

Stop and return to the Manager if:

- product authority conflicts, or a locked behaviour in §Functional requirements is materially ambiguous;
- the work needs an edit outside the four authorised paths;
- Part 1 cannot be completed without wiring Part 2, a new dependency, a package change or a save change;
- a protected anchor (Scenario One, Reference Future v1, existing Wave 2 files, RNG salts) would be threatened;
- the pinned offline gate fails in a way that cannot be repaired inside the authorised paths.

No further Part 1 product decision is required.
