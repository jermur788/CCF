# MULTI Phase 2 — Sessile Oak and European Beech in the standalone 2D engine

**Status: APPROVED — owner approval 2026-11-24.** This quartet is the Step A deliverable of a two-phase gated task. The six owner decisions in §1 are **locked (6/6 Option A)** and are not re-litigated here. The owner has answered the narrow questions in §14 (recorded in the amendment record below) and authorised Step B (implementation and `P2-01`…`P2-10` qualification). Where the amendment record and the body text below differ, **the amendment record binds**.

## Amendment record — owner approval 2026-11-24 (binding)

This record is appended at approval. The body of the spec is otherwise left as drafted, so the proposal wording that led to each decision stays visible. Every resolution below was confirmed by the owner on 2026-11-24.

**Base.** The quartet was drafted over Stage 5 code head `5b757d9b4a353d07557db95d3af9227c8d841874` and, at approval, rebased onto the Stage 5 **documentation tip** `d1db81b59248c13b4f9682ecdf68e37759b15c65`. `d1db81b5` is a documentation-only amendment over `5b757d9b` (the third Stage 5 amendment record: R1 whole-cell unit, aggregate-only limitation, R3 margin provenance). It changes no `src`, test, constant, tolerance or hash. Stage 5 is formally closed. Step B branches from `d1db81b5`.

| # | Question | Owner resolution (binding) |
|---|---|---|
| Q1 | Version token | **Accepted as proposed.** The species version is appended to `AreaModelVersion` **only when `!= 0`**. All `0/0/0/0` through `1/1/1/1` tokens and hashes stay byte-identical. |
| Q2 | Spot patches and cohort exposure | **Accepted as proposed.** Spot patches affect **exact-point planted-juvenile exposure only**. Natural cohorts keep reading their cell's cover. Unity's area-weighted cohort dilution (`UnderstoreyCompetition.cs`, `CompetitionExposureCache`) is an **intentional, documented non-port**. |
| Q3 | Order shapes | **Accepted as proposed.** `Plant`, `Shelter` and `SpotClear` stay **three separate fenced Work Plan order types**. `Shelter` targets a living `PJ` or a same-batch `Plant` at that point. `SpotClear` may target any in-polygon point. |
| Q4 | Planted-juvenile undergrowth timing | **Accepted as proposed (audit correction A-1).** Planted juveniles read the **same-year stepped** bramble/bracken covers. The packet's "pre-recovery" wording is superseded for bramble/bracken. |
| Q5 | `MA-1-MIXED-v1` | **Accepted, with the share fixed by the owner.** It is a **test-only** fixture. Area A stays **100 % Sitka**. **Area B is split one third Oak, one third Beech and one third Sitka**, with reassigned stems set to **age 60**. This replaces §10's drafted default of "one third each of Oak and Beech" and leaves the remaining third Sitka. The deterministic assignment rule (ordinal `TreeId` based) is fixed in code before any qualification number is measured, and is documented in the Step B handoff. |
| Q6 | Pure-Sitka equivalence | **Accepted as proposed.** `P2-03.8` is **mandatory**: `1/1/1/1/1` on unmodified all-Sitka MA-1 must equal `1/1/1/1/0` **identically** in trees, bands and accounts. The version token is the only permitted difference. |

**Audit qualification A-1 (binding; adopts Q4).** Planted juveniles read the bramble/bracken covers stepped **in the same year** before regeneration growth. This matches Unity `RegenerationModel.Competition`: `PrepareCompetitionForYear` (`ScenarioOneCompetition.cs:15-41`) steps both cell and planting-patch covers in ecology step 5b, before regeneration growth. `AdvanceUnderstorey` (step 6) advances only the habitat groups, which Phase 2 does not model. It also matches the engine's Stage 5 step 5a, which natural cohorts already read. §4's A-1 bullet and §7.4's recovery rule are to be read with this resolution.

**Provenance.** The spec quartet (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) and the pre-flight audit were authored by **Claude Opus 5.5**. The audit facts cited in this quartet (Unity file/line references, sequence order, shelter and clearance rules, and qualification A-1) were **verified read-only by the coordinating model** against the stated commits. That verification was inspection only. It is not Unity, runtime or rendered verification. The owner's approval covers the quartet as amended here. Implementation numbers remain unverified until Step B qualification is observed.

- **Spec ID:** SPEC-multi-phase2-oak-beech-V1
- **Owner:** Overall Manager / project owner
- **Created:** 2026-11-24
- **Last updated:** 2026-11-24
- **Decision log references:** D-059 (three-species parity), D-061 (ordinal species-aware mixed density v1), D-091 (lane admission), D-094 / D-095 (Stage 4 adult/crown approvals, cited at `3238846d`)
- **Milestone:** standalone 2D Sitka Engine (`Tools/MultiRackSitkaP1Lab`) — Phase 2
- **Related evidence:** `Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/PHASE2-PREFLIGHT-AUDIT-20261124.md`; `specs/multi-stage5-sitka-regeneration-v1/`; Stage 5 handoffs under `Docs/Handoffs/MultiRack-Simulation-20261116/stage5-regeneration/`
- **Supersedes / superseded by:** none

Task class: `SPEC REQUIRED` (two new species across adult growth, crowns, mixed density, regeneration, planting, browsing protection and three new Work Plan commands).
Lane: standalone 2D engine, `Tools/MultiRackSitkaP1Lab` only. **Zero** Unity `Assets/`, Compact30/Legacy40/Enlarged80/Reference Future v1, `/multi` UI, save-schema or `ProjectSettings` change.

## 0. Source stamps (read-only audit, 2026-11-24)

| Item | Commit |
|---|---|
| Implementation base (owner-chosen) | `feat/multi-stage5-sitka-regeneration-20261123-v1` @ `5b757d9b4a353d07557db95d3af9227c8d841874` |
| Stage 5 approval (delta-only) | `e8f47e73` |
| `origin/main` at drafting | `37ceb264b5431fe254d67e5702af39bf66cddc1d` |
| Unity behaviour source (pinned formulas) | `Assets/ForestPrototype/**` at `5b757d9b`. The 12 cited Unity files and `Species/` are byte-identical at `37ceb264` (`git diff --stat 5b757d9b 37ceb264 -- <files>` is empty). |

`origin/main` contains neither the engine nor `specs/multi-stage5-sitka-regeneration-v1/`; Phase 2 therefore branches from the Stage 5 feature HEAD, not from `main`.

**Stage 5 clearance on the record.** Stage 5 `5b757d9b` is APPROVED on a delta-only basis (`e8f47e73`). The owner has accepted both outstanding conditions as written: **(R1)** `S5-07A` certifies the matched population in aggregate only — matched edge blocks are not edge-consistent under Option A; **(R3)** the `S5-02` margins stand. Step B's Stage 5 prerequisite is therefore satisfied.

## Product intent

Phase 2 makes the engine's connected woodland a three-species mixed stand. The player can see how Oak and Beech grow, crown, compete, thin, seed, regenerate, get browsed and respond to exact planting with shelters and spot clearance, alongside the existing Sitka. Mixed forestry must produce visible consequences for regeneration, canopy, habitat and productivity. Planting is **not** automatically beneficial: a planted Oak in deep shade, unprotected under browse or smothered by bramble can fail, and the engine must show it.

## Problem

The engine is Sitka-only. Unity already holds Oak/Beech species definitions, a two-part mixed density route (D-061), browse protection and exact planting, but none of it is in the engine. Porting naïvely would (a) silently change the four frozen selections, (b) triple natural regeneration capacity in mixed cells, (c) triple the natural 5 m promotion ceiling, and (d) imply economic parity that does not exist.

## Goals

- A fifth model switch that adds Oak/Beech with **zero** change to every existing selection.
- Oak/Beech adult growth, C2 crowns and two-part mixed density at accepted calibration.
- Natural Oak/Beech regeneration sharing one capacity and one 5 m promotion ceiling with Sitka.
- Exact planting, shelters and selective spot clearance through the existing fenced Work Plan lifecycle.
- Deterministic, rack-order-invariant, idempotent results, qualified by `P2-01`…`P2-10`.

## Non-goals

See §13 Exclusions. In particular: no economy, no stock, no storms, no new species beyond Oak/Beech, no Unity/UI/save change.

## 1. Locked owner decisions (6/6 Option A — not re-litigated)

1. **Fifth model switch.** `SpeciesModelVersion { Off = 0, ThreeSpeciesMixed = 1 }` becomes the fifth field of `MultiAreaModelSelection`. Full Phase 2 is `1/1/1/1/1`. With `Species == Off`, the four existing tuples, all `AreaModelVersion` strings, all RNG streams and all outputs stay byte-identical. Core default `0/0/0/0/0`; `/multi` default untouched; `RequireSupported()` gains the new tuples explicitly.
2. **Shared natural capacity, 1-total 5 m block cap.** Bands keyed by `(SpeciesId, Origin, EstablishYear)`, one combined occupancy capacity per 2.5 m cell (`1.5 · a`). At most **one natural adult per aligned 5 m block per year across all species**, with Stage 5's partial edge-block credit. Tie-break: oldest `EstablishYear` → tallest → `(Column, Row)` → ordinal `SpeciesId`. Exact planted juveniles (`PL-{juvenileId}`) are **exempt** from the block cap.
3. **Exact planting, nursery defaults, optional selective ~1 m² spot clearance.** Fenced `Plant`, `Shelter`, `SpotClear`; zero economy, zero stock. `PJ####` at age 3, 0.6 m; strict in-polygon; `≥ 0.5 m` from plantings; `≥ 0.7 m` from living adult stems. Selective circular clearance, radius `0.564 m`.
4. **Consistent C2 crowns + accepted broadleaf calibration.** `CrownModel 1` for Oak and Beech including new recruits; Oak linear, Beech power-law crowns; growth unchanged; Beech's slight height-band miss accepted.
5. **Exact two-part mixed density (D-061) + species-aware broadleaf pruning.**
6. **Independent mast streams + connected-woodland seed rain.**

## 2. Version switch

```text
SpeciesModelVersion { Off = 0, ThreeSpeciesMixed = 1 }
MultiAreaModelSelection(Adult, Mortality, Crown, Regeneration = Off, Species = Off)
```

Selection order is `Adult / Mortality / Crown / Regeneration / Species`. Supported, explicitly allow-listed in `RequireSupported()` (`src/Core/Stage4Models.cs:77-89`):

| Tuple | Meaning |
|---|---|
| `0/0/0/0/0` | frozen MA-1 (`Legacy`) |
| `0/0/1/0/0` | Stage 1–3 C2 crown |
| `1/1/1/0/0` | Stage 4 |
| `1/1/1/1/0` | Stage 5 |
| `1/1/1/1/1` | **Phase 2 (new)** |

`Species = 1` requires `1/1/1/1`; every other combination with `Species != Off` is refused with a new typed `P1Refusal("SpeciesModelIncomplete", …)`. An undefined enum value is refused with `P1Refusal("SpeciesModelUnknown", …)`. The existing refusal codes and messages for the four old tuples do not change.

**Byte-identity is structural.** A fifth record parameter with default `Off` is exactly how `Regeneration` was added. With `Species == Off` no Phase 2 code path executes, no new RNG key is drawn, and the existing `Legacy`/`Stage3CrownC2`/`Stage4`/`Stage5` static instances compare equal to their five-field forms.

**Token (proposal, Q1).** The species version is appended to `AreaModelVersion` **only when `!= 0`**, mirroring Stage 5's owner Q3, so all four existing tokens stay byte-identical.

Honesty labels: every constant below is `GAMEPLAY CALIBRATION`, `ORDINAL` or `TRANSFER` (Unity gameplay values). **None is `IRISH DIRECT`.** Light is a modelled index, not calibrated PAR.

## 3. Species constants (source-pinned Unity values)

Source: `Assets/ForestPrototype/Species/SessileOak.asset`, `BeechSpecies.asset` at `5b757d9b`, as listed in the task packet and spot-verified (audit §3).

| Constant | Sessile Oak (`sessile-oak`) | European Beech (`beech`) | Class |
|---|---|---|---|
| `Pd` (cm/y) / `Dmax` (cm) | 0.8 / 120 | 0.9 / 120 | GAMEPLAY CALIBRATION |
| `Ph` (m/y) / `Hmax` (m) | 0.65 / 42.5 | 0.75 / 42.5 | GAMEPLAY CALIBRATION |
| `Ci50` (growth) / `k` (crown relax) | 5 / 0.12 | 6.5 / 0.15 | GAMEPLAY CALIBRATION |
| Growing-space weight | 2.0 (class High) | 1.5 (class Intermediate) | ORDINAL (D-061) |
| Crown `r_pot` | `max(0.1, 0.1 + 0.1045·DBH)` | `CPA = exp(0.05 + 1.01·ln(max(0.1, DBH)))`, `r_pot = max(0.1, sqrt(CPA/π))` | TRANSFER |
| Maturity onset / full (yr) | 40 / 60 | 40 / 60 | GAMEPLAY CALIBRATION |
| Seed `S50` | 5 | 5 | GAMEPLAY CALIBRATION |
| Dispersal `λ` / cutoff (m) | 4 / 80 | 6 / 50 | GAMEPLAY CALIBRATION |
| Mast P(good) / P(poor) | 0.15 / 0.35 | 0.15 / 0.35 | GAMEPLAY CALIBRATION |
| Mast multiplier good / normal / poor | 1.0 / 0.3 / 0.075 | 1.0 / 0.4 / 0.1 | GAMEPLAY CALIBRATION |
| Natural initial height (m) | 0.18 | 0.15 | GAMEPLAY CALIBRATION |
| Max juvenile `dH` (m/y) | 0.35 | 0.35 | GAMEPLAY CALIBRATION |
| `PromotionHeight` (m) / `PromotionMinimumLight` | 3.5 / **0.2** | 3.5 / 0 | GAMEPLAY CALIBRATION |
| Browse palatability | 1.0 | 0.5 | GAMEPLAY CALIBRATION |
| Browse full vulnerability ≤ / escape ≥ (m) | 1.2 / 1.8 | 1.2 / 1.8 | GAMEPLAY CALIBRATION |
| `regenDensityPerEstablishment` / `regenDensityMax` | 0.5 / 1.5 | 0.5 / 1.5 | TRANSFER |

Beech's serialized linear crown fields (`crownRadiusIntercept 0.9415`, `crownRadiusPerCmDbh 0.07635`) are **unused** (`usePowerLawCrown: 1`) and must not be read.

### 3.1 Adult growth (broadleaf)

```text
dDBH = Pd · site · clamp01(1 − DBH/Dmax) / (1 + CI/Ci50_species)
dH   = Ph · site · clamp01(1 − H/Hmax)
site = 1.0 (neutral)
```

`CI` is the existing woodland-wide 8 m Hegyi index (species-neutral). `CanopyLightEvaluator` gets **no** species transmissivity term. Oak/Beech growth parameters, including Beech's accepted slight height-band miss, are **not retuned**. Sitka keeps its unchanged Stage 4 adult step.

### 3.2 Crowns (`CrownModel 1`, all species)

```text
r_target = max(0.1, r_pot(DBH) / sqrt(1 + CI_woodland / Ci50_species))
r'       = r + k_species · (r_target − r)        (annual relaxation)
```

Applies to every Oak/Beech adult, including newly graduated natural recruits and `PL-*` planted recruits (their initial radius is `r_target` at promotion, not open-grown `r_pot`). **Intentional divergence:** Unity's `AdvancePlantedJuveniles` spawns with `species.PotentialCrownRadiusM(dbh)` (open-grown); Phase 2 uses C2 for consistency with Stage 5 natural promotion (owner decision 4).

## 4. Annual sequence at `1/1/1/1/1`

Mirrors Unity's authoritative sequence (`ScenarioOneManager.cs:1346-1357`, `ForestEcologyController.cs:471-498`), minus storms:

| # | Step | Phase 2 rule |
|---|---|---|
| 1 | Resolve executed orders | `SpotClear` patches and `Plant`/`Shelter` records are applied at Execute, **before** the next ecological year. Zero calendar advance. |
| 2 | Financial settlement | **No-op** (no economy). |
| 3a | Competition → canopy/light → adult DBH+height → crown relaxation | Stage 4 steps, extended per species (§3.1–3.2). |
| 3b | Adult density mortality (batched) | §5. |
| 3c | Canopy refresh | unchanged. |
| 3d | Undergrowth (Stage 5 step 5a) | unchanged cell rule, plus spot-patch recovery (§7.4). |
| 3e | Existing natural regeneration growth/survival | per species (§6.2). |
| 3f | Mast | one independent roll per species (§6.1). |
| 3g | Seed rain → establishment | per species, shared capacity (§6.1, §6.3). |
| 3h | Natural promotion | shared 5 m block cap (§6.4). |
| 3i | Suitability / diagnostics | neutral `1.0`; annual accounts. |
| 4 | `AdvancePlantedJuveniles` | §7.3 — **after** adult mortality and after natural growth **and** promotion. |
| 5 | Planting marker refresh | session state only (no UI). |
| 6 | Understorey habitat | not modelled beyond bramble/bracken (no-op). |
| 7 | Deadwood | Stage 4 fallen deadwood, now with `SpeciesId` (§5.3). |

Consequences reproduced deliberately:
- Planted juveniles read the **post-growth canopy** of the year (canopy refreshed at 3c and not recomputed after natural promotion in the same year, as in Unity).
- Browse is assessed on height **before** that year's growth (Stage 5 Q6, Unity `AssessIndividualBrowse` before `GrowHeight`).
- A `PL-*` promotion inside step 4 spawns the adult but does **not** recompute canopy in the same year.
- **Bramble/bracken timing (audit qualification A-1, Q4).** The packet states planted juveniles use the "pre-recovery understorey". Verified Unity code shows that under `RegenerationModel.Competition` the bramble/bracken covers of cells **and** planting patches are stepped in `PrepareCompetitionForYear` (`ScenarioOneCompetition.cs:15-41`), called from ecology step 5b **before** regeneration growth; `AdvanceUnderstorey` (step 6) only advances the habitat groups (mosses, ferns, grasses, forbs, shrubs, fungi; `ScenarioOneUnderstorey.cs`). The engine's Stage 5 step 5a already corresponds to `PrepareCompetitionForYear`. **Proposal:** planted juveniles read the same-year stepped covers (= Unity Model 2 and = what natural cohorts already read in Stage 5). The "pre-recovery" wording holds only for the habitat groups, which Phase 2 does not model.

## 5. Adult density mortality — exact two-part mixed route (D-061, TRANSFER)

Ported from Unity `MixedDensityModel.cs`, `MixedDensityModel.Composite.cs` (`PlanComponents`) and `SitkaSubsetDensitySignal.cs` at `5b757d9b`. Evaluated **per rack** (`MultiArea` area) with the rack's area in hectares, as Stage 4 already does for Comeau.

### 5.1 Routing

- **100 % Sitka rack** (no living Oak/Beech adult): the unchanged Stage 4 Comeau Sitka path, bit for bit. Nothing below runs, no extra key is drawn.
- **Mixed rack** (≥ 1 living Oak/Beech adult):
  1. **Sitka component.** The Comeau Sitka process runs on the **Sitka stems alone**, with stems per hectare of the whole rack area, drawing `ADULT-MORT-{treeId}` exactly as in a pure Sitka rack. Its victims are Sitka stems; a broadleaf can neither shield a Sitka stem nor pay a Sitka debt. *Step B Stage 1 confirms that the engine's Stage 4 Comeau routine can be called on a subset without changing its pure-rack arithmetic.*
  2. **Generic component**, on the stems still standing after step 1:

```text
space_i          = w_species · (π/4) · DBH_i²          w: Sitka 1.0, Beech 1.5, Oak 2.0   (ORDINAL)
occupancy O      = (Σ space_i / area_ha) / 55                                            (GAMEPLAY CALIBRATION)
G(O)             = 0.5 · (1 − exp(−1.60 · max(0, O − 1)²))                               (GAMEPLAY CALIBRATION)
nonReferenceShare = Σ_{Oak,Beech} space_i / Σ space_i
fraction         = G(O) · nonReferenceShare
expected         = fraction · eligible
quota            = floor(expected + 1e-10) + [roll(quota key) < remainder]
```

Victims among eligible stems by **species-neutral** suppression: `S_i = CI_i / (CI_i + 5)` (`Ci50 = 5` for every species, never the species' own growth `Ci50`), weight `S_i⁴`, weighted draw without replacement by exponential race on `roll(ADULT-MORT-GENERIC-{treeId})`, ties by ordinal `TreeId`. Pay the quota in priority order within a **0.125 × stand basal area** annual budget, passing over stems that do not fit; if none fits, take the single highest-priority stem (1-stem fallback). Sitka and generic victim sets are disjoint.

**Keys (proposal).** Per-tree: `ADULT-MORT-GENERIC-{treeId}` (owner decision 5). Quota remainder: `ADULT-MORT-GENERIC-QUOTA-{areaId}` (Unity uses the literal `~generic-quota` within its single stand; the area id is added because the engine has several racks). Both are new keys, never drawn when `Species == Off` or in a 100 % Sitka rack.

**Why `Ci50 = 5` for victims but `6.5` for Beech growth.** A species' own `Ci50` already sets its growth response; reusing it for victim choice would double-count it and remove Beech first at equal competition (Unity header comment, `MixedDensityModel.cs`). `P2-03` tests both sides.

### 5.2 Species-aware pruning (TRANSFER, Unity `ForestTree.CanPrune`)

| | Sitka | Oak, Beech |
|---|---|---|
| Max lifts | 3 | **2** |
| Minimum interval | 5 years | **3 years** |
| Target crown base | any valid | **exactly 3.5 m (lift 1), 6.0 m (lift 2)** |
| Cut / target factor | `max(0.1, r·0.92)` / `0.92^lifts` | same |
| Guard | target > current base, > 0, `< 0.6·H` | same |

Uses the existing `MultiAreaPruneOrder`; refusal codes gain a broadleaf target refusal. Sitka pruning is untouched.

### 5.3 Fallen deadwood

Every density death (both components) writes the Stage 4 fallen deadwood record plus `SpeciesId`. Volume `DBH² · 0.00007854 · H · 0.5` for all three species (the existing form-height ratio, `Stage4Records.cs:79-87`; GAMEPLAY CALIBRATION, not a species volume table).

## 6. Natural Oak/Beech regeneration

All Stage 5 rules apply per species unless stated. Sitka keeps its Stage 5 constants and its `STAGE5-MAST` stream untouched.

### 6.1 Mast and seed rain

- **Mast.** One annual `NextDouble()` per species on its own stream: `PHASE2-MAST-sessile-oak`, `PHASE2-MAST-beech` (proposal). Thresholds `r < 0.15 → good`, `r < 0.50 → poor`, else normal, with the species multipliers of §3. Strictly isolated from `STAGE5-MAST` and from every `ADULT-MORT-*` key.
- **Seed rain** per in-polygon cell centre `c` and species `s`: `S_c,s = Σ_i P_i · exp(−d_ic/λ_s)` over living mature adults of `s` in **all** connected racks with `d_ic ≤ cutoff_s`. `P_i = M_s(age) · clamp01(r_i/4.0) · 1.0 · mast_s` with `M_s` linear between onset 40 and full 60 years; `r_i` = realised C2 radius. Outer grassland contributes zero. Adult-empty racks still advance and receive seed.

### 6.2 Juvenile growth, survival and browsing (cohorts)

```text
dH_light  = 0.35 · LR_growth_s(light) · site
survival_light:
   Oak   = LR_survival_oak(light)                                 (distinct tables, useDistinctJuvenileLightResponses = 1)
   Beech = 0.8 if LR_beech(light) < 0.15 else 1.0                 (legacy rule on the shared table)
V_s(h)    = 1 (h ≤ 1.2);  1 − (h − 1.2)/0.6 (1.2 < h < 1.8);  0 (h ≥ 1.8)
p         = min(0.95, pressure · palatability_s · V_s(h) · 1.0 · access)
dH        = dH_light · (1 − 0.90·p)
survival  = survival_light · (1 − 0.04·p) · (1 − p_veg)
p_veg     = clamp01(0.35 · exposure · clamp01((1.5 − h)/1.2))
```

- Oak tables: growth `L=[0,.01,.05,.1,.15,.2,.3,.4,.5,1] → [0,0,.02,.07,.15,.3,.6,.8,1,1]`; survival `L=[0,.01,.05,.1,.15,.2,.3,1] → [.2,.3,.55,.75,.9,1,1,1]`; establishment `L=[0,.01,.05,.1,1] → [.75,.8,.95,1,1]`.
- Beech shared table: `L=[0,.02,.05,.1,.2,.35,1] → [0,0,.15,.35,.65,1,1]` for growth, survival band and establishment.
- `pressure = 0.2` default; `pressure = 0` takes the exact non-browse arithmetic path (bit-identical to light-only). The vegetation-exposure (concealment) factor is fixed at `1.0` because browse concealment is excluded.
- `access` for cohorts is `1.0`: shelters never apply to cohorts (Unity `CohortAccess`), and fences are out of scope (no fence command), so cohort access is unconditionally `1.0` in Phase 2.
- `h` is pre-growth height (Stage 5 Q6).

### 6.3 Establishment and shared capacity

```text
seedFactor_s   = 1 − exp(−S_c,s / 5)
establishment_s = seedFactor_s · LR_establish_s(light_c) · suitability_c          suitability = 1.0
requested_s    = 0.5 · establishment_s · a          (skip if establishment ≤ 0.01)
```

Bands are keyed `(SpeciesId, Origin, EstablishYear)`; Phase 2 uses `Origin = Natural` only (planted juveniles are exact individuals, never bands). **One combined capacity per cell**: `Σ_s density_s / regenDensityMax_s ≤ 1` with capacity `1.5 · a` (`a = (cell/5)²`), so a mixed cell holds no more relative abundance than a pure one. Admission order within a cell is ordinal `SpeciesId` (deterministic; proposal). Band merge/accumulator rules are the Stage 5 port; whether Unity's 4-band + accumulator limit is per cell or per species-in-cell is confirmed read-only in Step B Stage 1 and ported verbatim.

### 6.4 Natural promotion — shared 5 m block cap

- Candidate band: `H ≥ 3.5 m` **and** `light_c ≥ PromotionMinimumLight_s` (Oak 0.2, Beech 0, Sitka 0).
- One shared integer block credit `K` per aligned 5 m block (Stage 5 §4, Option A2, unchanged): **at most one natural promotion per block per year across all species**, with partial edge-block credit.
- Winner: oldest `EstablishYear` → tallest height → lowest `(Column, Row)` → ordinal `SpeciesId`.
- Cohort export (Option A1) matches `(SpeciesId, EstablishYear)`: only same-species, same-year sibling bands are exported.
- New adult: `Age = max(1, year − EstablishYear)`, `Height`, `DBH = clamp(1.5·H, 2, 20)`, `r_0` = C2 target of §3.2 with the species' own `r_pot` and `Ci50`. Position via the existing `STAGE5-PROMO-{areaId}-{col}-{row}-{year}` stream (unique per block-year because the cap is 1 total); strictly in-polygon. `TreeId = R{year}-{areaId}-{col}-{row}` as in Stage 5.

## 7. Exact planting, shelters and spot clearance

**Scope honesty.** These commands are **not** a port of Unity's `ResolvePlanting`, which consumes nursery stock, cash, owner-minutes and a `stockItemId` offer identity. Phase 2 has zero economy and zero stock inventory. The engine borrows Unity's **geometry, nursery defaults and browse rules only**. No parity with Unity's planting economics is claimed.

### 7.1 Orders and lifecycle

Three new order records alongside `MultiAreaPruneOrder` and `MultiAreaClearUndergrowthOrder` (proposal; Q3):

```text
MultiAreaPlantOrder(AreaId, SpeciesId, X, Z)          SpeciesId ∈ {sessile-oak, beech}
MultiAreaShelterOrder(AreaId, X, Z)                   protects the planted juvenile at (X, Z)
MultiAreaSpotClearOrder(AreaId, X, Z)                 ~1 m² circle centred at (X, Z)
```

Lifecycle is the existing fenced one: `Pending → Approve → Execute`, engine-minted execution key, idempotent replay, atomic all-or-nothing revalidation against the fence (`SessionId`, `MultiAreaSourceToken`, `PlanStateRevision`), **zero calendar advance**, zero mutation before Execute, **zero economy, zero stock**. Within one Execute, orders resolve in the fixed order `SpotClear → Plant → Shelter`, then by order id. All three are refused unless `Species == ThreeSpeciesMixed`.

### 7.2 Validation (any failure refuses the whole batch)

| Rule | Value | Provenance |
|---|---|---|
| Point strictly inside the area polygon | — | TRANSFER (Stage 5 polygon rule) |
| Distance to any existing living `PJ` or any other `Plant` in the batch / queue | `≥ 0.5 m` (refuse when `< 0.5`) | **TRANSFER** — `ScenarioOneManager.cs:2198-2203`; tie-break: **lower work-order id wins**, the later order is refused ("Another marker occupies this planting position") |
| Distance to any living adult stem | `≥ 0.7 m` | **NEW OWNER RULE** — does **not** exist anywhere in Unity; not a transfer |
| `Shelter` target | an existing living, unpromoted `PJ` within `0.25 m`, or a `Plant` in the same batch at that point; no other effective shelter within `0.25 m` | TRANSFER (`BrowseShelter.MatchRadiusM = 0.25`) / proposal |
| Unknown area/species, stale fence, duplicate order, non-finite coordinates | refuse | Stage 5 pattern |

### 7.3 Planted juveniles

- Id `PJ####` (4-digit, monotonically minted per session). Nursery defaults: **age 3 years, height 0.6 m** (`ForestEcologyController.cs:31,33`, TRANSFER).
- Annual step (§4 step 4), in ordinal `juvenileId` order, mirroring `ScenarioOneManager.AdvancePlantedJuveniles` (`:1989-2056`):
  1. `age += 1`.
  2. **Vegetation loss**: `p_veg` (§6.2) from the exposure at the exact point (§7.4) and pre-growth height; dies if `roll(PJ…/vegetation) < p_veg`.
  3. **Browse**: `p` (§6.2) with `access` from §7.5 and pre-growth height; browsed if `roll(PJ…/browse) < p` (deterministic Boolean).
  4. Grow height with the containing **2.5 m cell's** light (Unity uses `cell.Light`, not a point sample), browsed fraction `1` or `0`.
  5. **Survival**: `roll(PJ…) < survival_light · (browsed ? 0.96 : 1.0)`.
  6. **Promotion** if `H ≥ 3.5 m` and cell light `≥ PromotionMinimumLight_s`: adult `PL-{juvenileId}` at the **exact X/Z**, `Age = max(1, round(age))`, `DBH = clamp(1.5·H, 2, 20)`, C2 initial radius (§3.2). **Exempt** from the 5 m block cap and block credit. No same-year canopy recompute.
- RNG keys (proposal): `PHASE2-PJ-{juvenileId}/vegetation`, `PHASE2-PJ-{juvenileId}/browse`, `PHASE2-PJ-{juvenileId}` (Unity shape `{id}/vegetation`, `{id}/browse`, `{id}` with a namespace prefix so no key can collide with `ADULT-MORT-*` or `STAGE5-*`).

### 7.4 Selective ~1 m² circular spot clearance

- Footprint: circle radius `sqrt(1/π) = 0.5641896 m`, area exactly `1.0 m²` (`VegetationClearance.cs:14`, TRANSFER). A continuous circle, **not** a 2.5 m cell; it may straddle cells.
- Execute records a patch with `bramble = bracken = 0` (Unity `PlantingClearancePatch`).
- **Recovery.** Each year at step 3d the patch's own covers step toward the target `T` of the cell containing its centre with the Stage 5 gains (0.30 up / 0.45 down), exactly as Unity `PrepareCompetitionForYear` steps patches. A patch created at Execute holds `0` until its first annual step (Unity `competitionUpdatedYear` guard; confirmed in Step B Stage 1).
- **Exposure at a point** = `max(bramble, bracken)` of the newest patch containing the point, else of the containing cell.
- **Selective.** Spot clearance never removes, thins or rescales any natural cohort. **Intentional divergence:** Unity `QueryClearance` (`ScenarioOneClearance.cs:8-40`) removes cohort density in proportion to the cleared area, and Unity's planting auto-clearance is destructive. Phase 2 follows Stage 5's selective `ClearUndergrowth` instead (owner decision 3).
- **Cohort exposure (proposal, Q2).** Natural cohorts keep reading their cell's cover; patches do not dilute it. Unity area-weights patches into `CellPressure` (`UnderstoreyCompetition.cs`, `CompetitionExposureCache`); that is not ported.
- `SpotClear` is optional on planting: a `Plant` without one leaves the stem in full cell exposure.

### 7.5 Shelters

Ported faithfully from `Assets/ForestPrototype/BrowsingConditions.cs` (TRANSFER):

```text
BrowseShelter { ShelterId = "S-J" + juvenileId, X, Z, InstalledYear, EffectiveYears = 8, FailedYear = -1 }
IsEffective(y) = y ≥ InstalledYear && y < InstalledYear + max(0, EffectiveYears) && (FailedYear < 0 || y < FailedYear)
access(point, y) = 0 if an effective shelter lies within 0.25 m, else 1
```

- `access` is **binary** 0/1, never graded.
- Expiry is **strict `<`**: protection covers years `InstalledYear … InstalledYear + 7`; year `InstalledYear + 8` is unprotected. It is **not** `<=`.
- `FailedYear` is ported and **never set** (Unity never sets it: initialised `-1`, changed only by record edit; no stochastic failure exists). **Shelter failure is out of Phase 2 scope.** No failure probability is invented.
- Shelters never apply to cohorts. The id is the literal Unity convention, so `PJ0001` → `S-JPJ0001`.
- `InstalledYear` is the ecological year of the first annual step after Execute (Unity `report.year` semantics; confirmed in Step B Stage 1).

## 8. RNG isolation

`SimulationRandom` (`RngModelVersion = 1`). New keys only, all drawn only when `Species == ThreeSpeciesMixed`: `PHASE2-MAST-sessile-oak`, `PHASE2-MAST-beech`, `ADULT-MORT-GENERIC-{treeId}`, `ADULT-MORT-GENERIC-QUOTA-{areaId}`, `PHASE2-PJ-{id}…`. Phase 2 never draws from, or reorders, `STAGE5-MAST`, `STAGE5-PROMO-*` (other than its existing per-block use), or `ADULT-MORT-{treeId}`. Seed rain, establishment, cohort growth and browse fractions are deterministic functions.

## 9. State and atomic commit

New immutable `MultiAreaPhase2State` as one more optional member of `MultiAreaRoot` (`= null` whenever `Species == Off`), holding: planted juveniles, shelters, spot patches, per-species mast state, pending/approved/executed Plant/Shelter/SpotClear orders, the planting-id counter, and per-species annual accounts. Natural bands gain `SpeciesId` and `Origin` **only inside Phase 2 state paths**; Stage 5 records used by `1/1/1/1/0` keep their current shape and serialisation order so its fingerprint cannot move. One reference assignment publishes the root; a forced pre-commit failure leaves the previous root intact. **Session-only: nothing is saved** (no save-schema change).

## 10. Mixed woodland fixture (proposal, Q5)

MA-1 is all Sitka. Oak/Beech adults that can seed within a 30-year test need ages ≥ 40. Step B adds a **test-only** fixture builder `MA-1-MIXED-v1` that derives a mixed woodland from MA-1 in `Core.Tests` only (no generator, `/multi` or default change): Area A stays 100 % Sitka; in Area B a fixed, documented subset of stems is reassigned to Oak or Beech by ordinal `TreeId` hash, with positions, DBH and height unchanged and age set to 60. The exact rule is fixed before any qualification number is measured.

## 11. Evidence classification

| Part | Class |
|---|---|
| Broadleaf growth, crown, maturity, mast, dispersal, light tables, browse constants | `GAMEPLAY CALIBRATION` / `TRANSFER` from Unity — not measured Irish values |
| Growing-space weights 1.0 / 1.5 / 2.0 | `ORDINAL` (D-061), magnitude `GAMEPLAY CALIBRATION` |
| Occupancy normaliser 55, `G(O)` (0.5, 1.60, exponent 2), BA budget 0.125, victim `Ci50 = 5`, exponent 4 | `GAMEPLAY CALIBRATION` (Unity `[C]`), not an SDI, maximum density or self-thinning law |
| Two-part routing and pure-Sitka exactness | `TRANSFER` (Unity `PlanComponents`, `SitkaSubsetDensitySignal`) |
| Shelter 8-year life, 0.25 m match radius, binary access | `TRANSFER` (Unity `[G]` working assumption, not a measured life) |
| Nursery defaults age 3 / 0.6 m, 1 m² spot circle, 0.5 m spacing | `TRANSFER` |
| 0.7 m from adult stems | **NEW OWNER RULE** (no source) |
| Shared capacity, 1-total block cap, cohort keying, C2 for planted recruits, selective spot clearance | `SIMULATION ABSTRACTION` (owner decisions 2–4) |
| Neutral site 1.0, suitability 1.0 | deliberate neutral default, no soil data |

Qualification claims are limited to the numerical behaviour of the stated rules. No ecological forecast; no claim that planting or mixing is beneficial.

## 12. Save / backward compatibility; security / write-back

- **No save impact** — Phase 2 state is session-only; no save schema, migration or Reference Future change.
- **Write-back:** the only mutations are the fenced Execute of the three new orders; stale, duplicate, out-of-polygon, spacing-violating or unknown targets refuse the whole batch with typed `P1Refusal`s and mutate nothing.

## 13. Exclusions

No cash, prices, contractor fees or timber assortments (Phase 3). No nursery stock inventory. No storms. No DEC, DMC or mixture-productivity bonus. No Scenario Two species (Scots pine, birch, alder). No graminoids or browse concealment. No fences or fence commands. No shelter failure mechanism. No understorey habitat groups. No Unity `Assets/` edit, no `/multi` HTML/JS/CSS edit, no save-schema edit, no `ProjectSettings`, no `.meta`. No use of protected ports `:33767`, `:43649`, `:46593`.

## 14. Open questions for the owner (narrow; recommendation given)

- **Q1 Token.** Append the species version to `AreaModelVersion` only when `!= 0` (as Stage 5 Q3). *Recommend: yes.*
- **Q2 Spot patches and cohort exposure.** Patches affect only exact-point exposure (planted juveniles); cohorts keep cell cover. *Recommend: yes* — simpler, keeps spot clearance strictly a planting aid; Unity's area-weighted dilution is a documented non-port.
- **Q3 Order shapes.** Three separate orders; `Shelter` targets a living `PJ` or a same-batch `Plant` at the point; `SpotClear` may target any in-polygon point. *Recommend: yes.*
- **Q4 Bramble/bracken timing for planted juveniles.** Read the same-year stepped covers (Unity Model 2; audit qualification A-1), not literally "pre-recovery". *Recommend: yes.*
- **Q5 Mixed fixture.** Test-only `MA-1-MIXED-v1` (Area A pure Sitka; Area B partly reassigned to age-60 Oak/Beech). *Recommend: yes; owner may name the reassignment share (default: one third each of Oak and Beech in Area B).*
- **Q6 Pure-Sitka equivalence at `Species = 1`.** Additionally assert that `1/1/1/1/1` on unmodified all-Sitka MA-1 gives the same trees, bands and accounts as `1/1/1/1/0` (tokens aside). *Recommend: yes* — the strongest proof that the Phase 2 path adds nothing to pure Sitka.

## Stop conditions

Any frozen hash moves at `Species = Off`; Phase 2 needs a change to `Assets/`, `/multi` UI or a save schema; the Comeau routine cannot be run on a Sitka subset without changing pure-rack arithmetic; Unity's band limit cannot be ported without changing `1/1/1/1/0` output; another writer on the lab; D-091 not granted. Stop and report the concrete fact; do not retune, widen a tolerance or invent a constant.
