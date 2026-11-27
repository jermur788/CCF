# CCF Multi 2D — Jay Acorn Dispersal, Nearby Oak Woodland and Windstorms V1 — Specification

- **Spec ID:** `SPEC-multi-2d-jay-windstorms-V1`
- **Feature ID:** `multi-2d-jay-windstorms-v1`
- **Task:** `MULTI-2D-JAY-WINDSTORMS-P1-20261127`
- **Status:** `APPROVED / IMPLEMENTING` (owner-approved 2026-11-27, Option A locked). **Part 1 ACTIVE** (pure engine). **Part 2 QUEUED** behind Playable Milestone 1.
- **Base Commit:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`)
- **Canonical Context Commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Branch:** `feat/multi-2d-jay-windstorms-engine-v1`
- **Namespace:** `SitkaP1.Core.NaturalProcesses` — pure .NET 8 BCL only.
- **Scope boundary:** isolated engine + standalone offline tests. **NOT** wired into `AnnualPipeline`, `MultiAreaContinuation`, `/multi` UI or saves. **NOT** merged. **No Unity.**

Evidence labels used below: **[D-050]** frozen accepted decision; **[C]** gameplay calibration (not measured ecology);
**[OWNER]** locked owner decision for this task; **[ENG]** bounded engineering choice made by the implementer.

---

## 1. Locked owner decisions (Option A — not re-opened)

### 1.1 Jay Acorn Dispersal [OWNER]

1. World option `JayAcornDispersal`, **OFF by default**. When OFF the engine makes **zero RNG draws** and returns empty results.
2. Runs only when the caller passes the **existing engine's oak mast outcome** saying this year is an oak mast year. The feature never creates a second jay mast schedule. On-map sources are the caller's existing oak seed candidates, already produced under the existing oak maturity rules.
3. **On-map cap:** up to **10 jay-routed candidate acorns per hectare per mast year** [C], nearest-integer rounding of `rate × hectares` (0.64 ha → **6**). Candidates are **diverted** from the caller's existing on-map oak candidates (taken from that list), never added on top. Zero double counting; source tree ID retained. Never more than `min(cap, supplied count)`.
4. **Transport:** dedicated deterministic stream keyed by `(year, source, candidateIndex)`. `r = 5 + 195·u²` metres (5–200 m) [C]; uniform bearing in `[0, 2π)`. Destination = `source + r·(cos θ, sin θ)`. Outside the woodland polygon → **discarded cleanly and reported as exported seed**: never redrawn, never clamped, no retry.
5. **Zero microsite bias** (no gap, bramble or moss bonus). Every delivered candidate is output for the caller's ordinary ground-suitability, light, bramble/bracken and deer-browsing evaluation. Zero browse immunity. No instant seedlings.

### 1.2 Nearby Oak Woodland [OWNER]

1. World option `NearbyOakWoodland`, **OFF by default**; needs `JayAcornDispersal` ON (jays are the only transport).
2. An authored virtual source line just outside **one** chosen outer edge (North/South/East/West) of the woodland extent.
3. **Off-map cap:** up to **6 external candidate acorns per hectare per mast year** [C], nearest integer (0.64 ha → **4**). Every candidate is attributed to `NearbyOakWoodland` with **no fabricated source tree ID**.
4. Same transport routine; candidates that fail to enter the woodland are discarded as exported seed.

### 1.3 Windstorm adapter — StormModel 1 [OWNER + D-050]

1. `StormModel` 0 is storm-free with **zero draws** and shifts no other stream. StormModel 1 is **OFF by default** (callers pass 0 unless the world option enables it).
2. Frozen D-050 reused verbatim: annual occurrence **0.02**; severities **0.02 / 0.06 / 0.18**, weights **1:1:1**; **ReducedProposal** index; **BoundedRational** transform; neutral site **1**; neutral/absent external edge; one uprooted/fallen outcome only.
3. **One woodland-wide annual event** across all lanes. Never per-lane occurrence, severity or direction. Zero invented lane, exposure or fresh-thinning multipliers.
4. Fixed versioned stream IDs `STORM-OCCURS-v1`, `STORM-SEVERITY-v1`, `STORM-DIRECTION-v1`, per-tree `WINDTHROW-v1-<TreeId>`.
5. `DirectionDegrees = u · 360` (uniform [0, 360)) — the bearing the storm blows **TOWARDS** (0 = north, 90 = east); frozen D-050 semantics.
6. Two-pass victim evaluation (all indices from pre-storm state, then rolls), trees sorted by ordinal `TreeId`:
   `V = Load(H) · Dominance(H/Htop) · Slender(((H/D)/70)²) · Openness(1 + 0.6L) · Recent(1 + 0.8RO) · Site · Edge`,
   with `Load = (H/20)²`, `Dominance = min(1, H / max(1, Htop))²`, D in metres, `Site = 1`, `Edge = 1`.
   Victim when `Roll("WINDTHROW-v1-" + TreeId, year, seed) < p`. Height, DBH, local top height, cell light, recent opening, site hazard and edge facing come **from the caller**; the engine invents no missing ecology.
7. Each victim: mortality cause `"windthrow"`, **one** fallen deadwood record, **one** canopy opening (+1 recent opening at its cell; the caller applies its existing cap). No snapped stems, no snags, no dominoes.
8. Player feedback is qualitative only: **Stable / Watch / Exposed** (thresholds on V: `< 1.5` Stable, `< 3.0` Watch, else Exposed — gameplay calibration carried over from the frozen Unity bands). No hazard percentage or `p` is ever exposed in a result or text.
9. The event exposes `DirectionDegrees` and `FallGlyphOrientationDegrees = DirectionDegrees`, explicitly a **UI convention only**, not a persisted per-tree fall bearing (no per-tree jitter).

**Resolution of a wording conflict in the task packet [ENG, flagged for review].** The packet locks "frozen D-050 reused verbatim … ReducedProposal/BoundedRational" and also quotes the design-document form `p = 1 − exp(−S·V)`. The frozen, integrated StormModel 1 (`Assets/ForestPrototype/StormModel.cs`, D-050) uses **BoundedRational**: `p = S·V / (1 + S·V)`, capped at 0.999999. The exponential form is the pre-calibration design proposal (`Docs/Research/WindthrowV1/StormEventArchitecture.md` §3). Because the governing lock is "D-050 verbatim" and the packet itself names BoundedRational, Part 1 implements **BoundedRational** and does not implement the exponential. The two differ by < 2 % for `S·V < 0.04` and diverge above that. Bit-identical parity with the frozen Unity formula is proven by test JW-T61.

**Direction convention note [OWNER ruling 2026-11-27, D-050].** The design note's proposal of `225 ± 45°` *from* (prevailing south-westerly) is **not adopted**. Part 1 follows the frozen code (`Assets/ForestPrototype/StormModel.cs`): a uniform 0–360° bearing the wind blows *towards*, from the same `STORM-DIRECTION-v1` draw. Site hazard and edge facing remain neutral under StormModel 1, so direction never affects victims either way.

### 1.4 Salvage accounting [OWNER + D-050]

1. Default `LeaveAsFallenDeadwood`. **No automatic salvage, ever**: a deadwood record with no order stays deadwood.
2. `SellAndExtract` and `KeepForUse` only through an approved Work Plan reference (`ApprovedWorkPlanId`).
3. After extraction, no duplicate deadwood (guard on deadwood ID, both for re-orders and for re-merging deadwood).
4. Revenue only for `SellAndExtract`. `KeepForUse` yields retained timber and **no revenue**. Prices and contractor minimum unchanged (multiplier 1.00); the contractor minimum is applied by the existing grouped harvest settlement in Part 2.
5. A dated, explicitly non-causal teaching note constant covers Storm Darragh (December 2024), Storm Éowyn (January 2025, written "Eowyn" in ASCII) and the 2026 windblow market. It is market context, not a forecast or causal model.

### 1.5 Contracts and determinism [OWNER]

Immutable request/result records, enums, stable append-only refusal codes and text constants for Part 2. No wall clock, no machine state, no culture-dependent formatting, no file or network I/O in `src/Core/NaturalProcesses/**`.

---

## 2. Strict file fence

Only these prefixes may be created or modified:

1. `specs/multi-2d-jay-windstorms-v1/**`
2. `Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/**`
3. `Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/**`
4. `Docs/Handoffs/MultiRack-Simulation-20261116/jay-windstorms/**`

Out of scope: `Assets/**`, `Packages/**`, `ProjectSettings/**`, `main`, `src/Core/{MultiArea,SpeciesWave2,WoodlandImport,Phase1}/**`, `RngSalts.cs`, `src/Ui/**`, `tests/Core.Tests/**`, `build/**`, `SitkaP1.Core.csproj`, `*.sln`, `Directory.Build.props`, `Docs/Project/**`, `Docs/Research/**`.

---

## 3. Public contracts (`SitkaP1.Core.NaturalProcesses`)

| Area | Types |
|---|---|
| Shared | `NaturalProcessRefusalCode` (append-only ints), `NaturalProcessRefusal`, `LocalPoint` (+X east, +Y north, metres), `OuterEdge`, `WoodlandFootprint` (polygon, shoelace area, boundary-inclusive containment), `NaturalProcessStreams`, `NaturalProcessText` |
| Jay / Nearby | `JayAcornDispersalSettings` (both OFF by default), `OakMastYearOutcome`, `OnMapOakSeedCandidate`, `JayDispersalRequest`, `JayAcornCandidate`, `JayCandidateOrigin`, `JayCandidateFate`, `JayDispersalResult` (`RequiresOrdinaryEstablishmentEvaluation = true`, `MicrositeBonus = 0`, `BrowseImmune = false`, `CreatesInstantSeedlings = false`), `JayAcornDispersal.Disperse` |
| Storm | `StormModelVersion`, `StormTreeInput`, `StormRequest`, `StormEvent`, `StormExposureBand`, `TreeExposureFeedback`, `WindthrowVictim`, `FallenDeadwoodRecord`, `DeadwoodForm` (Fallen only), `CanopyOpening`, `StormResult`, `StormAdapter.Resolve` / `ExposureBands` |
| Salvage | `SalvageChoice` (default Leave), `SalvageOrder`, `SalvagePricing`, `SalvageRequest`, `SalvageLine`, `SalvageResult`, `DeadwoodMergeResult`, `SalvageAccounting.Apply` / `MergeWindthrowDeadwood` |

All results report `RngDrawCount`. Every refusal is all-or-nothing: no partial result is returned.

---

## 4. Engineering choices made in Part 1 [ENG]

| ID | Choice | Reason |
|---|---|---|
| ENG-01 | Keyed roll = verbatim port of `SimulationRandom.Roll` **model 1** (FNV-1a + splitmix64), the RNG model the 2D lab uses (`DerivedManifest.RngModelVersion = 1`). | The test exe must be standalone; bit parity is proven by JW-T60. |
| ENG-02 | On-map diversion above the cap: one `JAY-DIVERT-v1-<CandidateId>` draw per supplied candidate; lowest rolls win (ties by ordinal ID). At or below the cap all are diverted and no selection draws are made. | Order-independent and unbiased by ID. |
| ENG-03 | On-map transport key: `JAY-ACORN-v1-<SourceTreeId>-<index>` where index is the candidate's ordinal rank among **all supplied** candidates of that tree. Suffixes `/distance`, `/bearing`. | A candidate's path never depends on which others were diverted. |
| ENG-04 | Off-map transport key: `JAY-ACORN-v1-NEARBY-OAK-WOODLAND-<Edge>-<index>`; one extra `/along` draw places the source uniformly along the edge. | Same transport routine; no fabricated tree. |
| ENG-05 | Source line = the chosen side of the woodland's axis-aligned extent, offset **1 m** outward. | "Just outside one outer edge"; presentation geometry, not ecology. |
| ENG-06 | Default `NearbyOakWoodlandEdge = West` (only used when the option is ON). | A value is required; the owner chooses the edge per world. |
| ENG-07 | Cap rounding `Math.Round(rate × ha, MidpointRounding.AwayFromZero)`; hectares from the polygon's shoelace area. | Deterministic nearest integer (6.4 → 6, 3.84 → 4). |
| ENG-08 | Bearing θ is measured anticlockwise from +X (east), as implied by `(cos θ, sin θ)`. | Matches the locked destination formula. |
| ENG-09 | Nearby ON with Jay OFF is refused (`NearbyOakWoodlandRequiresJayDispersal`) with zero draws. | Avoids a silent no-op configuration. |
| ENG-10 | Storm inputs carry `SiteHazard` and `EdgeFacing`; StormModel 1 refuses anything other than 1 and 0. | Inputs come from the caller, but D-050 freezes them neutral; a non-neutral driver needs StormModel 2. |
| ENG-11 | `LastResolvedStormYear` guard refuses a second resolution for the same or an earlier year. | Enforces "at most once per year". |
| ENG-12 | Deadwood ID `DW-WINDTHROW-<year>-<TreeId>`; `MergeWindthrowDeadwood` refuses IDs already present or already extracted. | Stable ID guard against duplicates. |
| ENG-13 | Salvage revenue = `round(volume × unit price × 1.00, 2, away from zero)` in `decimal`; recoverable volume comes from the caller's existing yield calculators. | No new pricing or grading. |
| ENG-14 | Storm tree inputs are `float`, mirroring the frozen `StormTreeContext`. | Bit parity with D-050. |

---

## 5. Part 2 — QUEUED behind Playable Milestone 1

Part 2 integrates Part 1 **without rewriting it**: the contracts, stream IDs, formulas and refusal codes above are fixed. Remaining work:

1. **`AnnualPipeline`:** call `StormAdapter.Resolve` once per year after approved management and before competition/growth (StormEventArchitecture §4); apply victims (mortality cause, one deadwood record, +1 recent opening capped); batch one canopy/seed rebuild; call `JayAcornDispersal.Disperse` with the existing oak mast outcome and oak seed candidates, remove diverted candidates from the ordinary list, and route delivered candidates through ordinary establishment.
2. **`MultiAreaContinuation`:** carry the world options, last resolved storm year, storm event history and extracted deadwood IDs through continuation; salvage orders through the existing Work Plan approval flow and grouped harvest settlement (contractor minimum applied there).
3. **`RngSalts` registration:** add `STORM-OCCURS-v1`, `STORM-SEVERITY-v1`, `STORM-DIRECTION-v1`, `WINDTHROW-v1-`, `JAY-DIVERT-v1-`, `JAY-ACORN-v1-` to the frozen registry and its prefix-freeness tests.
4. **ForestMaker:** world-creation controls for `JayAcornDispersal`, `NearbyOakWoodland` + edge, and StormModel (default 0 / OFF).
5. **Save v3:** persist world options, storm model, resolved storm events `{year, severityLevel, directionDegrees}`, extracted deadwood IDs; older saves load as OFF / StormModel 0.
6. **`/multi` UI:** Stable/Watch/Exposed readout, storm event notice with fall glyph (visual convention), jay/nearby explanations, salvage choices, teaching note.

Part 2 needs its own packet, owner lock state and review.
