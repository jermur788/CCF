# Species Wave 2 Moisture Table and Native Profile Cards Specification

- **Spec ID:** SPEC-multi-2d-species-wave2-profiles-V1
- **Status:** IMPLEMENTING (author implementation on `feat/multi-2d-species-wave2-profiles-20261126-v1`; awaiting independent review)
- **Owner:** Overall Manager (owner-authorised bounded 2D simulation lane)
- **Created:** 2026-11-26
- **Last updated:** 2026-11-26
- **Decision log references:** none new; follows the task packet for this lane
- **Milestone:** Multi-rack 2D simulation lab (Tools/MultiRackSitkaP1Lab)
- **Related evidence:** `Docs/Handoffs/MultiRack-Simulation-20261116/species-wave2-profiles/`
- **Supersedes / superseded by:** none

## Product intent

Later waves will add Scots pine, downy birch and common alder to the 2D lab and give every species a response to site
moisture. Before anything is wired, the project needs one reviewed, evidence-tagged source of those values so later
wiring cannot quietly reuse Sitka constants or invent ecology.

## Problem

There is no site-moisture vocabulary in the lab and no profile data for the three native Wave 2 species. Without it,
future work would either hard-code values inline or silently copy Sitka spruce constants.

## Goals

- A pure, immutable moisture response table for all six species with separate establishment and growth multipliers.
- Immutable profile cards for Scots pine, downy birch and common alder (colonisation, crown, silviculture, timber metadata).
- An explicit evidence class on every provisional constant, in code and in this spec.

## Non-goals

- No wiring into the annual loop (`AnnualPipeline.cs`, `AnnualEngine.cs`), Phase 1/2 economy, saves, RNG salts or UI.
- No change to Sitka, oak or beech species files, MultiArea, Phase1, Ui, wwwroot or Unity assets.
- No price table: timber bands are read-only metadata, not consumed by the economy.
- No colonisation simulation, no mortality from heart rot, no planting-approval logic on bog.
- No new packages, project or solution edits.

## User / player workflows

Not applicable — this is data-only Core code with no player-facing surface yet.

## Functional requirements

- FR-1: `SiteMoistureClass` MUST have exactly NeutralLegacy, DryFreeDraining, FreshMoistMineral, SeasonallyWetAerated,
  WetMovingAlluvial, StagnantPoorBog.
- FR-2: `SpeciesMoistureTable.Response(speciesId, moisture)` MUST return a `SiteResponse(EstablishmentMultiplier,
  GrowthMultiplier)` whose two values are never pre-multiplied together.
- FR-3: NeutralLegacy MUST return exactly (1.0, 1.0) for all six species.
- FR-4: The 30 authored cells MUST equal the table below, using tier values G=1.00, M=0.70, L=0.35, X=0.10.
- FR-5: `HabitatProtectionCaution(moisture)` MUST be true for StagnantPoorBog only.
- FR-6: Each profile card MUST expose the fields listed in "Profile cards" with the stated values and evidence tags.
- FR-7: `ComputeTargetCrownRadiusM(D, ci) = r_pot(D) / sqrt(1 + ci / ci50)`.
- FR-8: Unknown species ids MUST be rejected with `ArgumentException`.

## Moisture table

Tier defaults: G=1.00, M=0.70, L=0.35, X=0.10 — all **[Calibration]**. Cell tier assignments are the authored design
table — **[Calibration]** pending per-cell source review. Cells are establishment/growth.

| Species | Dry free-draining | Fresh moist mineral | Seasonally wet aerated | Wet moving alluvial | Stagnant poor / bog |
|---|---|---|---|---|---|
| sitka-spruce | M/L | G/G | G/G | M/M | L/L |
| sessile-oak | M/M | G/G | M/M | L/L | X/X |
| european-beech | M/M | G/G | L/L | X/X | X/X |
| scots-pine | G/G | G/G | M/M | M/L | L/L |
| downy-birch | M/M | G/G | G/G | G/M | M/L |
| common-alder | L/L | G/G | G/G | G/G | L/X |

NeutralLegacy = (1.0, 1.0) for every species **[Calibration — design invariant]**, so Scenario One woodlands never shift.
HabitatProtectionCaution: StagnantPoorBog only. Biological survival never implies planting approval on intact bog.

## Profile cards

### Readiness

Structured `ReadinessProfile Readiness` on every card (replaces the earlier free-text status/notes). A null numeric
domain means the sources give no defensible domain, not "unbounded". Metadata only; nothing reads it at runtime.

| Field | Scots pine | Downy birch | Common alder |
|---|---|---|---|
| Status | ReadyForInitialDesign | PartialWithAbstractions | PartialWithAbstractions |
| AgeRangeYears | 10–60 (Irish GROWFOR) | — | — |
| StemsPerHectare | 100–5,000 (Irish GROWFOR) | — | — |
| BasalAreaM2PerHa | 10–100 (Irish GROWFOR) | — | — |
| MinimumTopHeightM | 5.0 (Irish GROWFOR) | — | — |
| IrishYieldClassPotential | — | 8 | — |
| Trajectory | Open-grown crown fit applied to an initial-design even-aged stand within the Irish GROWFOR domain | Fast pioneer flush -> moderated competition-sensitive adult | Very fast early pioneer -> sharp moderation; short-lived with late heart-rot risk |
| Notes | Cites GROWFOR domain; crown fit is a Turkish transfer | Crown fit is a proxy (UK silver birch amenity) | Late heart-rot risk is expressed only via the timber heart-rot inspection flag (50–60 years), never mortality |

### Colonisation (all three)

| Field | Scots pine | Downy birch | Common alder |
|---|---|---|---|
| ExternalColonisation | AuthoredOnly | AuthoredOnly | AuthoredOnly |
| LocalOnMapSeedDispersalAllowed | true | true | true |
| WaterAssistedDispersalEligible | false | false | true |
| Hard wind ceiling | none | none | none (no 30 m ceiling) |

### Crown

| Species | Shade tolerance | r_pot formula (D in cm, m) | Evidence | Ci50 default / band [Calibration] |
|---|---|---|---|---|
| Scots pine | LightModerate | CW = 0.6874 + 0.2358 D − 0.0011 D², r = max(0.10, CW/2), D clamped at 58.6 | Transfer (Turkish open-grown fit) | 4.75 / 4.0–5.5 |
| Downy birch | Light | r = max(0.10, exp(−0.80284) · D^0.64367 · exp(0.04399)) | Inference + Calibration (UK silver birch amenity proxy) | 4.25 / 3.5–5.0 |
| Common alder | Moderate | r = max(0.10, exp(−0.38257) · D^0.52772 · exp(0.05982)) | Empirical + Inference (UK amenity fit) | 4.75 / 4.0–5.5 |

Anchors (m) at D = 10 / 20 / 30 / 45 cm, tolerance ±0.02: pine 1.47 / 2.48 / 3.39 / 4.54; birch 2.06 / 3.22 / 4.18 / 5.43;
alder 2.44 / 3.52 / 4.36 / 5.40. Computed: pine 1.4677 / 2.4817 / 3.3857 / 4.5355; birch 2.0611 / 3.2201 / 4.1804 / 5.4270;
alder 2.4409 / 3.5189 / 4.3585 / 5.3984.

- The pine quadratic is not extrapolated above 58.6 cm (radius 5.3640 m held constant).
- The 0.10 m floor **[Calibration]** guarantees positive juvenile continuity below 3.2 cm (outside the source data). With
  these coefficients it does not bind at D ≥ 0.5 cm; it only protects degenerate inputs (D → 0, negative or non-finite D,
  which are treated as 0).
- Negative Hegyi ci is treated as 0.

### Silviculture

| Field | Scots pine | Downy birch | Common alder |
|---|---|---|---|
| Light demand | High | High | High |
| Browse sensitivity | MediumHigh | MediumHigh | LowMedium |
| Form sensitivity | — | MediumHigh | — |
| Browse recovery | not specified | High | High |
| Pruning relevance | High, target 5–6 m | Moderate (quality-stem path only) | Low (self-pruning) |
| ThinningMoreImportantThanPruning | false | false | true |

Alder "Low" pruning relevance is an implementer reading of "self-pruning" **[Inference]**.

### Timber metadata (read-only, NOT wired to the economy)

| Field | Scots pine | Downy birch | Common alder |
|---|---|---|---|
| Role | PrimaryCommercialSoftwood | SecondaryCommercial | SecondaryCommercial |
| Green density t/m³ | 1.02 [Empirical + Inference] | 0.93 [Empirical + Inference] | 0.842 [Transfer] |
| Density sensitivity | 0.85–1.05 | 0.80–1.10 | 0.75–0.95 |
| Sortment bands EUR/t | Sawlog 50–85, SmallRoundwood 30–60, PulpChip 25–50 [Calibration — provisional Irish] | Bulk 25–60 [Calibration] | Bulk 25–60 [Calibration] |
| PremiumBuyerRequired | false | true | true |
| Inspection trigger | — | quality stem 30–35 cm DBH | heart-rot risk onset 50–60 years (flag/downgrade only, never deterministic death) |

## Authority / source of truth

- **Accepted product intent:** owner-authorised task packet for this lane (2026-11-26).
- **Implementation facts:** `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/*.cs`.
- **Supporting evidence:** formulas and anchors as supplied by the task packet; sources named per row above.
- **Spec authority:** when `APPROVED`, task-level implementation authority for this scope.

## Evidence classification

See tags in every table. Provisional constants are never unlabelled. The Irish pine sortment bands were given as
"provisional" without a class; they are tagged **Calibration** here (see Open product decisions).

## Save / backward compatibility

No save impact — nothing in this module is serialised or read by save code.

## Security / write-back

Not applicable — no I/O, no write-back, pure in-memory immutable data.

## Acceptance criteria

- AC-1: All 30 cells and NeutralLegacy rows match exactly (test W2-01).
- AC-2: HabitatProtectionCaution true only on StagnantPoorBog (W2-02).
- AC-3: Crown anchors within ±0.02 m (W2-03).
- AC-4: Pine clamp at 58.6 cm, non-decreasing above (W2-04).
- AC-5: Juvenile radii finite, positive, non-decreasing at 0.5/1/2/3 cm (W2-05).
- AC-6: Target radius identities at ci = 0 and ci = ci50 within 1e-12 (W2-06).
- AC-7: AuthoredOnly colonisation; premium buyer on birch/alder only; water dispersal on alder only (W2-07).
- AC-8: No Sitka reuse (Ci50 ≠ 5.0), crown evidence classes as specified, sortment bands non-empty (W2-08).
- AC-9: Solution builds with 0 warnings/0 errors; Core tests ≥ 291 passed, 0 failed; 2D gate G0–G7 PASS with 9/9 frozen anchors.
- AC-10: Readiness records match the Readiness table: pine ReadyForInitialDesign with all four GROWFOR numeric domains
  and minimum top height 5 m; birch PartialWithAbstractions, YC 8, pioneer -> competition-sensitive trajectory; alder
  PartialWithAbstractions, pioneer / short-lived trajectory; all three still AuthoredOnly (W2-09).

## Stop conditions

- Any need to edit a forbidden path (csproj, sln, Program.cs, gate data, existing species files).
- Any frozen anchor change or Core test-count drop below 291.

## Open product decisions

- Per-cell moisture tiers are authored design values; a research review should upgrade individual cells from Calibration.
- Evidence class for the provisional Irish pine sortment bands (tagged Calibration pending owner confirmation).
- Pine browse recovery is unspecified.
- Whether NeutralLegacy should remain a permanent class or be migrated once sites are classified.
