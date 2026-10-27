# Scenario Two species-model foundation (Scots pine, downy birch, common alder)

Task `SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-V1`. BASE `8f9ed8025243254fe7378b6d7771a5f1e7efaccc` (the pinned Scenario One
candidate), locked CONTEXT `36cd791b329189e0ae38931d06ae51af1a1173ce`, branch
`feature/scenario-two-species-model-foundation-v1`. Research input: the Scenario Two species parameter synthesis
(`/media/jer/ZX20/CCF-Verification/ScenarioTwoSpeciesParameterSynthesis-20261019T152713Z/`), which is supporting evidence, not
decision authority.

**Status: foundation only.** It makes the three species *representable* without silently inheriting Sitka behaviour. It does
not calibrate them: **growth and juvenile light responses remain on HOLD for all three, browse remains on HOLD for pine, and
the wind kernel remains on HOLD for alder**, so
none of the three species is simulation-ready yet, and none is registered in any scene or save.

## 1. What changed

| Area | Change | Files |
|---|---|---|
| Explicit configuration | `SpeciesParameterState` (Unspecified / Set / Placeholder / Hold), `NaturalSeedDispersal` (Unspecified / None / Wind / WindAndWater), `GrowthAgePhase`, and a pure `SpeciesConfigurationPolicy` that decides whether a definition is valid and simulation-ready | `SpeciesConfigurationPolicy.cs` (new, no UnityEngine) |
| Species definition | Six new serialized fields (`growthState`, `growthAgePhases`, `browseState`, `juvenileLightState`, `naturalSeedDispersal`, `seedKernelState`) and read-only properties. **No existing default was changed.** | `TreeSpeciesDefinition.cs` |
| Registration | Only simulation-ready species are registered (`KnownSpecies`) or resolved (`ResolveSpecies`); an unready species is reported once in `Awake` | `ForestTreeSpawner.cs` |
| Age phases | Adult growth applies age-phase factors to DBH and height increments; the no-phase expression and local-variable layout retain the original exact arithmetic | `ForestEcologyController.cs` (`GrowAdults`), `ForestTreeMarkingManager.cs` (thinning forecast) |
| Seed rain | Planting-only and undeclared species put no seed on the ground (gate after species registration, before any accumulation) | `ForestEcologyController.cs` (`ComputeSpeciesSeedRain`) |
| Site rules | Scenario Two rows appended to the dormant site data (`ScenarioTwoSpeciesSiteResponses`); the Scenario One catalog is untouched | `SpeciesSiteResponse.cs` (additive only) |
| Species data | Three new assets in their own folder | `Species/ScenarioTwo/{ScotsPine,DownyBirch,CommonAlder}.asset` |
| Verification | Offline check and runner, site-rules check, in-Editor runtime checks | `Tools/Verification/ScenarioTwoSpeciesFoundation/`, `Tools/Verification/SiteInfrastructure/` |

Not touched: the Sitka, Oak and Beech assets, `SitkaGrowthModel.cs`, the density model, juvenile rules, the save and
reference-archive code, scenes, packages, project settings. The runner proves this by git blob id against BASE.

## 2. How the safe defaults work

A field left at its C# default is not "neutral": `growingSpaceDemand = Reference` is the Sitka density route, `browsePalatability
= 0` is browse-proof, and the seed kernel, maturity ramp, mast regime, crown envelope and juvenile light arrays are Sitka-like.
The **Sitka asset does not serialize `growingSpaceDemand` at all**, so the Reference default itself must not be changed. The
rules are therefore keyed on the species id.

- **Reference is Sitka-only.** Any other species at Reference is invalid (including Oak or Beech if mis-set).
- **The established three** (`sitka-spruce`, `sessile-oak`, `beech`) are not asked to declare anything: their serialized values are
  the accepted Scenario One behaviour, protected by the deterministic anchors.
- **Every other species must declare**, per area: growth (increments, bounds, Ci50, crown, form), browse, juvenile light, seed
  dispersal and (when it disperses) the seed kernel. `Unspecified` is an error; `Hold` is valid but not ready.
- **Guards against flipping a state without doing the work.** `Set` or `Placeholder` is refused while the area still holds the
  inert sentinels or the Sitka defaults (crown envelope, maturity 20/30, Sitka mast regime, legacy juvenile light response).
- **Fail-safe at runtime.** A species that is not simulation-ready is not registered or resolved. Undeclared or planting-only
  species compute no seed rain.

HOLD values in the three assets are inert **sentinels** (increments 0, size bounds 1, kernel 1/1), chosen so that forgetting to
fill them in fails loudly (no growth, no seed) instead of silently becoming Sitka.

Tags used below. **Use:** SET (entered, label travels with it), RANGE (stand-in inside an evidence bracket, calibration packet
only), ORD (ordinal class or ordering), TARGET (validation anchor, never a coded constant), NEW (new mechanism), HOLD (not set).
**Class:** A direct quantitative evidence, B evidence-bounded inference, C simulation abstraction, OPEN insufficient. **Geo:** IE,
GB, EU, NA. A with Geo other than IE is transfer evidence. "Rechecked" means the source was re-read for this task.

## 3. Common to all three

| Area | Value | Use | Class | Note |
|---|---|---|---|---|
| Density class | `Intermediate` | ORD | B/C | User-accepted ordinal placeholder, **not** a shared measured maximum-density coefficient. Weight 1.5 comes from the existing ordinal model ([ORD/C]). Published Central-European maximum-density lines cover pine, beech and oak only (none for birch or alder). |
| Age-dependent growth | Capability only: `growthAgePhases`, piecewise constant, factors relative to the species' own increments | NEW | C | No species supplies a phase table: no birch or alder equation is held, so none is invented. |
| External colonisation | Authored-only; no map-edge or off-map seed rule exists | — | C | Checked by search of the runtime source and by the in-Editor check. |
| Shared regeneration constants (S50, promotion height, regeneration density, poor-light mortality) | Existing shared values | — | C | Generic/common mechanism, not species evidence. |
| Wind-risk fields | Existing generic defaults | — | C | Diagnostic only. |

## 4. Scots pine (`scots-pine`)

| Area | In the asset / code | Use | Class | Geo | Evidence and note |
|---|---|---|---|---|---|
| Growth increments, bounds, Ci50, crown, form | **HOLD** (sentinels) | HOLD | OPEN | — | No GROWFOR coefficients are held. The 10-60 y / 100-5,000 stems/ha / 10-100 m²/ha / top height ≥ 5 m range is for the model **family**, not pine; use only as an extrapolation guard. |
| Productivity context | Not coded | TARGET | A | GB | Forest Research (rechecked): average ≈ YC 8, 12 or more on fertile sites. Height ≈ 35 m (UCC, rechecked). |
| Natural dispersal | **None: planting-only** | SET | — | — | User decision. Juvenile layer on, so planting works. No pine dispersal evidence exists; the 54 m Finnish figure is a *genetic* estimate and was not found in the EUFORGEN text (lead only). |
| Seed onset | Not coded (not dispersing) | RANGE | A | GB | Forest Research (rechecked): from about 15-20 y. EUFORGEN (rechecked): female flowering 15 y on solitary trees, **25-30 y in closed stands**. |
| Mast | Not coded | RANGE | A | GB | Forest Research (rechecked): good crops at 3-5 year intervals. **No "later / infrequent cone crop" behaviour**: that text belongs to the Douglas fir row of the species pack. |
| Juvenile light response | **HOLD** | HOLD | OPEN | — | Bound only: no more shade-tolerant than alder (~5 %/~20 % of daylight, B, EU/NA). |
| Browse | **HOLD** | RANGE | B / C | GB | Absent from Browsing v1. Class medium-high is an inference; bracket 0.5-0.9 for a calibration packet. 0 in the asset is a sentinel, **not** browse-proof. |
| Site | Waterlogged `Limiting` (ordinal); Fertility `Low` `Tolerated` | ORD | B / A | GB, IE | Waterlogging rests on the species pack alone (the Forest Research page does **not** mention it); fertility on Forest Research and UCC. Moisture and exposure not encoded. |

## 5. Downy birch (`downy-birch`)

| Area | In the asset / code | Use | Class | Geo | Evidence and note |
|---|---|---|---|---|---|
| Growth increments, bounds, Ci50, crown, form | **HOLD** (sentinels) | HOLD | OPEN | — | Needs the age-phase capability plus values. |
| Growth targets | Not coded | TARGET | A | IE | Nieuwenhuis & Barrett abstract (rechecked, abstract only): maximum height increment before age 20, > 1 m/yr in faster trees; maximum radial growth about ages 5-20; fastest tree 25 cm in 32 y; ≈ 200 m³/ha at 42 y ≈ YC 8, YC 10 with thinning. British YC 4-6 (Forest Research, rechecked). Faster/fastest trees are tails, not means. |
| Natural dispersal | **Wind**, kernel `Placeholder` | SET / RANGE | C | GB | One conservative scale. |
| Kernel scale / cutoff | **19 m / 150 m** | RANGE | A anchor, C conversion | GB | Liu & Evans 2021 (*Forests* 12:929, Wytham Woods; abstract rechecked): mean dispersal 65 m open, 38 m in a closed stand; **B. pendula and B. pubescens pooled**; kernel form not readable. Closed-stand anchor chosen as the conservative end; 19 m = 38 m / 2 under the existing 2-D exponential convention (mean = 2 × scale). 150 m is a performance cutoff (about 0.3 % of the kernel beyond it). |
| Seed onset / ramp | **15 y / 15 y** (a step) | RANGE | A / C | GB | Onset: Forest Research (rechecked). Full-fecundity ramp is OPEN: full = onset is the no-ramp placeholder. |
| Mast | **Neutral** (probabilities 0, multipliers 1) | RANGE | C | GB | Forest Research (rechecked): good crops almost every year after onset. Magnitude and variation OPEN; not the Sitka regime. |
| Juvenile light response | **HOLD** | HOLD | OPEN | — | Ordinal light-demanding pioneer only (B). |
| Browse | **0.5**, heights 1.2 / 1.8 m | SET | B / C | GB→IE | Browsing v1 initial value (low-moderate); broadleaf heights are the Browsing v1 abstraction. Palatability is not damage: Irish plantings recorded browsing mortality (no rate). |
| Site | Moisture `Moist` Preferred, `Wetter` Tolerated; `WaterloggedRisk` Tolerated; Fertility `Low`, `Moderate` Tolerated | ORD | A / B | GB, IE | Forest Research (most moist soils; very poor to medium nutrient status, rechecked) and Teagasc (tolerant of wet or waterlogged conditions, heavy clay, wet peatland, unlike silver birch; rechecked). No wetness bonus. Exposure not encoded (single source). |

## 6. Common alder (`common-alder`)

| Area | In the asset / code | Use | Class | Geo | Evidence and note |
|---|---|---|---|---|---|
| Growth increments, bounds, Ci50, crown, form | **HOLD** (sentinels) | HOLD | OPEN | — | No Irish growth curve (Teagasc, rechecked: Irish research limited). |
| Growth targets | Not coded | TARGET | A | EU, GB | Silvics tables (European data, re-read from the page images) and Claessens abstract (rechecked): very fast growth to age 7-10, then slows rapidly. Forest Research (rechecked): sometimes YC 12; full development 30-40 y, rarely beyond about 60 y. |
| Dispersal architecture | **WindAndWater** declared; wind kernel **HOLD**; water propagation **not implemented** | NEW / HOLD | A distances, C | EU, NA | No water or spatial carrier exists, and building one is hydrology. The capability is recorded and exposed (`HasWaterDispersalCapability`); no seed moves by water. Silvics (re-read, p. 107): seed usually not spread more than 30-60 m by wind; **saplings are rarely found more than 20-30 m from the parent where wind is the only vector**; running water and wind drift over standing water are the principal agents. **There is no 30 m cutoff.** |
| Seed onset | Not coded | RANGE | A | EU/NA | Silvics (re-read, p. 106): precocious, some trees flower in the second season and are producing large quantities of seed by years 6-7. The statement is general, with US plantations cited as bearing it out; not an Irish threshold. |
| Juvenile light response | **HOLD** | RANGE | A | EU/NA | Silvics (re-read, p. 107): about 5 % of full daylight essential for first-year establishment, about 20 % required for survival in later years (citing a British experiment). Anchors for a calibration packet; the response curve between them is not set. |
| Browse | **0.2**, heights 1.2 / 1.8 m | SET | B / C | GB→IE | Browsing v1 initial value. Forest Research (rechecked): tolerant of rabbits and hares, suffers less browsing than some native broadleaves. Context-dependent elsewhere. Must not be 0. |
| Site | Moisture `Wetter` Preferred, `Moist` Tolerated, `Drier` Limiting; Fertility `Moderate`, `High` Preferred. **Drainage/aeration: HOLD, no class assessed.** | ORD / HOLD | A / B | EU, GB | Moisture: Forest Research, Silvics, Claessens agree. Fertility: Forest Research (medium to rich nutrient supply, rechecked). **Waterlogging is unresolved (C-13) and is not encoded:** Forest Research itself says both "a high, but not stagnant, water table" and "remarkably tolerant of wet soil conditions"; Silvics says seedlings survive flooding that kills most trees, died when fully submerged, and were hardly hindered when water stood flush with the soil. Exposure not encoded (Forest Research: not tolerant; Teagasc: moderately resistant to salt spray: different stressors, C-17). |
| Plant health | Not modelled | OPEN | — | IE | Teagasc (rechecked): *Phytophthora alni* causes root and collar rot, severe dieback or death, and is "likely to threaten alder within Irish landscapes in the near future". **The earlier synthesis said it was detected in Northern Ireland; the Teagasc pages do not say so.** Scope is a Manager decision. |
| Nitrogen fixation | Not modelled | — | — | — | Real, but no nutrient mechanic exists; no soil-nitrogen effect is implied. |

## 7. Source recheck log

| Source | Rechecked | Result | Consequence |
|---|---|---|---|
| Liu & Evans 2021 (*Forests* 12:929) | Abstract (full text not readable) | 65 m open, 38 m closed confirmed; both birch species pooled; kernel form unknown | Birch scale 19 m is a [C] conversion of a verified anchor |
| Forest Research, downy birch | Page | Onset about 15 y; good crops almost every year; moist soils; very poor to medium nutrient status; exposure Moderate; tallest about 20 m; YC 4-6; rotation 50-60 y. **Does not mention waterlogging** | Coded: onset, mast shape, fertility, moisture. Waterlogging from Teagasc |
| Forest Research, common alder | Page | High but not stagnant water table **and** remarkably tolerant of wet soil; medium to rich nutrients; not tolerant of exposure; tolerant of rabbits and hares; short-lived; YC 12; light-demanding pioneer. **Age of first seed not stated** | Alder drainage left HOLD; seed onset only from Silvics |
| Forest Research, Scots pine | Page | Seed from 15-20 y; good crops 3-5 y; acid to neutral light soils of low fertility; exposure Moderate; average YC 8. **Waterlogging not stated** | Pine waterlogging `Limiting` rests on the species pack alone (ordinal) |
| Teagasc (birch, alder, ExAl) | Pages | Downy birch tolerates wet or waterlogged conditions; alder research limited, self-pruning, moderate salt-spray resistance; *P. alni* severe, not reported detected in Ireland | *P. alni* correction above |
| Claessens et al. review | Abstract | Growth very fast to 7-10 y then slows; rotation 60-70 y maximum if heart rot is to be avoided; early heavy frequent thinning; natural regeneration needs high light and moisture | Growth targets only |
| Nieuwenhuis & Barrett 2002 | Abstract (paywalled) | Values as in section 5 | Targets only; abstract-level |
| UCC Tree Explorers (pine) | Page | "can grow to 35 meters"; **"Ireland's only native pine"** with a reintroduction history | Ceiling check only; provenance question stays open |
| EUFORGEN, Scots pine | PDF text | Female flowering 15 y solitary, 25-30 y in closed stands; mast years relatively frequent; **no 54 m figure found** | Closed-stand delay recorded; 54 m stays an unverified lead |
| USDA Silvics, alder | Scan, pages 106-107 re-read | 5 %/20 % light, 6-7 y precocity, 30-60 m / 20-30 m / water statements, flooding nuance all confirmed | Recorded as RANGE/TARGET, no production constant |

Values left HOLD, RANGE or ORD because they could not be verified at the strength a constant needs: all pine and alder growth,
the alder wind kernel, juvenile light responses, pine browse, ramp lengths, mast magnitude, seed counts, crown geometry.

## 8. Verification

Completion verification is recorded under
`/media/jer/ZX20/CCF-Verification/ScenarioTwoSpeciesFoundationComplete-20261020T100426Z/` (see `HANDOFF.md` for the exact source
and candidate commits, receipts, remaining HOLDs and operational release). The Unity cache is the Manager-authorised linked
root `/media/jer/Files/CCF-UnityCache/U2/project`; source and evidence remain on ZX20. Unity version: 6000.6.0f1.

- Offline: species foundation 83 checks and all 28 protected-file comparisons; Scenario Two site rules 12; existing site
  infrastructure 63. The new site catalog remains dormant.
- Runtime: all 27 species foundation checks passed. The gate checks real asset import, readiness,
  empty-ID default resolution, age phases, planting-only seed behaviour, per-species wind cutoffs, alder wind beyond 30 m, authored-only colonisation and non-zero browsing. Run this
  gate with `CCF_STAND_GEOMETRY=1`: the seed-distance fixture needs Enlarged80; Legacy40 cannot reach its far-distance probe.
- Protected regressions: all 21 simulation/save/economy gates passed on the final source. Supplemental mixed density
  (34 checks), broadleaf adult growth (16 checks), and historical growth-model-1 completion (`702766DECE591E21`) also passed.
  Exact receipts are listed in the handoff. Historical lifecycle checks use
  `CCF_STAND_GEOMETRY=0`; frozen anchors are never rebaselined. No graphical UI, standalone Player or native-Windows claim is made.

**Regression found and fixed during completion.** The first candidate (`ac4f34c`) passed the feature checks but failed eight
legacy/reference gates. A same-environment detached BASE comparison passed all eight. The extra `heightPotential` local and
reordered growth calculations had perturbed the Mono floating-point path. The fix restores the original DBH/height declarations
and their order, and places the optional phase-factor locals in a helper reached only by phased species. Targeted canonical
verification restored `BFC55473C1506067` (neutral) and `3485B6630C9EA448` (normal browsing), with the full growth gate also restoring
`962846D2F517B293` / `FBB8F470D85FF815` (regeneration 1) and `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` (growth 1). Earlier failed receipts
are retained for audit. The current model-2 completion fingerprint also matches the pinned BASE receipt exactly
(`9C77EB4BEE427FB9`); the original candidate had drifted to `FE4AE5F67F45404C` despite passing the round-trip-only completion
assertion. A separate readiness fix rejects an unready default when `ResolveSpecies` receives null or an empty ID.

Production pine, birch and alder remain **not simulation-ready**. These checks qualify the foundation and synthetic fixtures;
they do not supply ecological calibration or release any HOLD.

## 9. To calibrate a species later

Fill the numbers, then change the state from `Hold` to `Placeholder` or `Set`. The policy refuses the flip while sentinels or
Sitka defaults remain. Growth also needs a species crown envelope; juvenile light needs `useDistinctJuvenileLightResponses` and
arrays; a dispersing species needs its own maturity and mast values. Age phases go in `growthAgePhases`. A water-dispersal
mechanism, if ever wanted, needs a spatial carrier first.
