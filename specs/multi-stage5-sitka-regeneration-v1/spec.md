# MULTI Stage 5 — Natural Sitka regeneration, bramble/bracken competition, selective undergrowth clearing and light browsing

**Status: DRAFT — awaiting owner approval. Not approved. No production code, D-091 coding lane or branch is authorised by this document.**
Task class: `SPEC REQUIRED` (new causal regeneration, understorey competition, browsing, promotion, one new Work Plan command).
Lane: standalone 2D Sitka Engine, `Tools/MultiRackSitkaP1Lab` only. **Zero** Unity `Assets/`, Compact30/Legacy40/Enlarged80/Reference Future v1, `/multi` UI, or save-schema change.

## 0. Source stamps (read-only audit, 2026-11-23)

| Item | Commit |
|---|---|
| Implementation base (owner-chosen) | `feat/multi-stage4-ui-catchup-20261123-v1` @ `e7c1a57da519ed1ee03ccfbad240196a66df0905` |
| Stage 4 Core (ancestor of base) | `feat/multi-stage4-adult-parity-20261123-v1` @ `500a50aea4bd049f231f05d73828fabdc0a7a85c` |
| Unity behaviour source (pinned formulas) | `main` @ `5e79e98621f1dff1ed388f6be55f94f78b3d8fe0` |
| `origin/main` at drafting | `bc8829d7de003e879c58c74662af02442516d555` |
| Owner UX R3 (deliberately NOT the base) | `0e777455…` — UI-only, in review |
| D-094 / D-095 approvals | `docs/multi-899-900-approved-spec-20261123-v1` @ `3238846d1dc1e68b16e7e49e24de34490af8ded2` (not yet on `main`) |

`e7c1a57d` and `500a50ae` are **not** ancestors of `origin/main`; Stage 5 therefore branches from `e7c1a57d`, not `main`.

## 1. Locked owner decisions (not re-litigated here)

1. Engine-first: pure-.NET engine + automated tests. `/multi` stays on Stage 4 (`1/1/1/0`); Core default stays `0/0/0/0`. No new UI, no save-schema change.
2. `ClearUndergrowth` is selective: it zeroes bramble and bracken on targeted cells and **keeps existing Sitka cohorts**. This is an intentional divergence from Unity's destructive whole-cell `ScenarioWorkType.RemoveRegeneration`.
3. One `2.5 m` ground-cell grid for light, undergrowth, cohorts and clearing, with an explicit area-consistent scaling policy (§4) proved by `S5-07`.
4. Promotion into `CrownModel 1`, strictly inside the clipped area polygon.
5. Connected-woodland rules: centre-in-polygon cells, adult-empty racks still advance, one shared annual mast roll, cross-rack seed rain, zero seed from outer grassland.
6. Version switch, neutral site, exact Unity browsing rules, atomic immutable state.

## 2. Version switch

```text
MultiAreaRegenerationModelVersion { Off = 0, SitkaCohortCompetitionBrowse = 1 }
```

Selection order is `Adult / Mortality / Crown / Regeneration`. Supported: `0/0/0/0`, `0/0/1/0`, `1/1/1/0`, `1/1/1/1`. Regeneration `1` requires `1/1/1` (it needs the Stage 4 adult step, the refreshed `2.5 m` decoupled light and `CrownModel 1`); every other combination is refused with a typed `P1Refusal`.
Regeneration `0` must produce **byte-identical** output to today's Stage 4 for every supported prior selection.

Honesty labels: all constants below are `GAMEPLAY CALIBRATION` or `TRANSFER` (Unity gameplay values). They are **not** measured Irish establishment probabilities and light is **not** calibrated PAR. Planting or regenerating trees is not automatically beneficial; the outputs are consequences of the stated simulation rules only.

## 3. Annual sequence at `1/1/1/1`

Steps 1–5 are the existing Stage 4 adult step, unchanged (woodland-wide 8 m Hegyi CI → adult growth → `CrownModel 1` relaxation → per-rack Comeau RD mortality and fallen deadwood → refreshed `2.5 m` decoupled light, read from the start-of-year root so area order cannot matter).

| # | Step | Rule |
|---|---|---|
| 5a | Undergrowth | per cell: `T = 0.8·clamp01(siteProductivity·soilStability)·(0.1+0.9·clamp01(light))`; year-0 `bramble = bracken = 0.25·T`; annual `cover' = clamp01(lerp(cover, T, T>cover ? 0.30 : 0.45))`. `siteProductivity = soilStability = 1.0`. |
| 6 | Grow/survive existing cohorts | see §3.2 |
| 7 | Mast | one woodland-wide roll `r = NextDouble()` on stream `STAGE5-MAST` for the year: `r < 0.15 → good, 1.5`; `0.15 ≤ r < 0.50 → poor, 0.4`; `r ≥ 0.50 → normal, 1.0` (verified against Unity `UpdateMast`: good probability 0.15, poor probability 0.35 added cumulatively, else normal). |
| 8 | Seed rain | per in-polygon cell centre `c`: `S_c = Σ_i P_i·exp(−d_ic/20)`, over living mature Sitka `i` in **all** connected racks with `d_ic ≤ 150 m`. `M(age)=0 (≤20), (age−20)/10 (20<age<30), 1 (≥30)`; `P_i = M·clamp01(r_i/4.0)·1.0·mast`, `r_i` = realised `CrownModel 1` radius. Outer grassland contributes zero. |
| 9 | Establishment | `seedFactor = 1−exp(−S_c/10)` (`S50 = 10`, as in `SitkaSpruce.asset`); `establishment = seedFactor·LightResponse(light_c)·suitability_c`; skip if `≤ 0.01`, else `requested = 0.5·establishment` scaled per §4. |
| 10 | Promotion | §3.3 and §4; then refresh canopy/light cache. |
| 11–12 | Neutral suitability / diagnostics | `suitability_c = 1.0` (no opening-decay or soil map is authored; stated, not invented). Emit read-only per-area annual accounts. |

`LightResponse(light)`: piecewise-linear through `(0,0) (0.10,0.10) (0.20,0.30) (0.25,0.50) (0.55,0.90) (1.00,0.80)`.

### 3.1 Bramble and bracken survival penalty

`p_veg = clamp01(0.35 · max(bramble, bracken) · clamp01((1.5 − preGrowthHeight)/(1.5 − 0.3)))` — full at `≤ 0.3 m`, zero at `≥ 1.5 m`. No graminoids, no browse concealment.

### 3.2 Juvenile growth, survival and browsing (exact Unity `JuvenileEcologyRules`)

```text
dH_light      = 0.35 · LightResponse(light) · siteProductivity
survival_light = 0.8 if LightResponse(light) < 0.15 else 1.0
V(h)  = 1 (h ≤ 0.5);  1 − (h − 0.5)/0.3 (0.5 < h < 0.8);  0 (h ≥ 0.8)
p     = min(0.95, pressure · 0.15 · V(h))        (≤ 0.03 at pressure 0.2)
dH    = dH_light · (1 − 0.90·p)
survival = survival_light · (1 − 0.04·p) · (1 − p_veg)     (double precision)
```

`pressure = 0.2` default; `pressure = 0` must take the exact non-browse arithmetic path and be bit-identical to light-only. No stem-form-damage variable, no shelters or fencing in Stage 5. Species constants: initial cohort height `0.15 m`, promotion height `3.5 m`, `regenDensityMax = 1.5`, `RepresentationThreshold = 0.01`, max 4 represented age bands plus one sub-threshold accumulator per cell (band merge/accumulator rules are **ported verbatim** from Unity `ForestEcologyController` @ `5e79e986`, not re-designed).

### 3.3 Promotion into `CrownModel 1`

The oldest qualifying represented band (`height ≥ 3.5 m`) becomes **one** adult Sitka and the band is removed:

```text
Age    = max(1, ecologicalYear − establishYear)
Height = cohort.Height
DBH    = clamp(1.5 · Height, 2, 20) cm
r_0    = max(0.1, r_pot(DBH) / sqrt(1 + CI_woodland/5))        CrownModel 1, NOT open-grown r_pot
```

`CI_woodland` is the connected-woodland Hegyi index (8 m) evaluated at the promoted stem. The light gate `PromotionMinimumLight` exists in Unity code but is **not serialised in `SitkaSpruce.asset`** (reads as 0, i.e. inactive); Stage 5 therefore applies no light gate — **Open question Q2**.
`TreeId = "R{year}-{areaId}-{col}-{row}"`, asserted unique against all existing IDs. Position: deterministic jitter inside the source cell from stream `STAGE5-PROMO-{areaId}-{col}-{row}-{year}`; if the point is outside the clipped area polygon, retry a fixed number of times, then fall back to the cell centre (centre-in-polygon is guaranteed by cell membership). A tree is never placed outside the polygon.

## 4. Area-consistent `2.5 m` scaling policy (PROPOSED — owner sign-off required)

**Source facts (verified):** Unity cohort `Density` is an *abstract abundance per cell*, not a stem count ("Not a stem-count conversion", `ForestEcologyController`). Occupancy is `Density / RegenDensityMax`, shared capacity per cell is 1 (`ForestEcologyCell.AdmitDensity`). Promotion exports a whole band to **one** tree, at most one band per cell per year. All of this is defined for a `5 m` cell (`25 m²`). Copied unchanged onto `2.5 m` cells (`6.25 m²`), the same woodland would admit 4× the abundance per hectare and could promote 4× the trees per hectare per year.

Let `cell = 2.5` or `5.0` m and `a = (cell/5.0)²` (so `a = 0.25` at 2.5 m, `a = 1` at 5.0 m — the 5.0 m run therefore reproduces Unity's own scaling exactly).

| Quantity | Policy | Why |
|---|---|---|
| Requested density per event | `requested · a` | density is extensive (per cell) |
| Cell capacity | `RegenDensityMax · a` (1.5 → 0.375) | keeps **relative** occupancy (0–1) and abundance per hectare equal |
| Representation threshold | `0.01 · a` | a band of the same relative size stays represented |
| Promotion budget | **≤ 1 promotion per aligned 5.0 m block per year**, blocks = `floor(x/5), floor(z/5)` in the world frame | caps trees/ha/yr at Unity's ceiling without fractional trees |
| Block winner | greatest height, then oldest, then lowest `(col,row)` | deterministic; non-winners stay and can promote next year |
| Growth, survival, seed rain, light, undergrowth | unchanged (all intensive) | per-point or per-plant rates |

Known approximation: an edge `5 m` block with fewer than four in-polygon cell centres still gets the full budget of 1. `S5-07` measures this; it is not hidden.
Supported grids remain `{2.5, 5.0}` only (the existing `CanopyLightEvaluator` rule).

## 5. `ClearUndergrowth` Work Plan command

- Order = `(AreaId, Column, Row)` of one existing in-polygon `2.5 m` ground cell. New order record alongside the Stage 4 `MultiAreaPruneOrder` pattern; the existing tree `WorkAction`/`MultiAreaWorkOrder` shapes do not change.
- Lifecycle is the existing fenced one: `Pending → Approve → Execute`, engine-minted execution key, idempotent replay, atomic all-or-nothing revalidation against the fence (`SessionId`, `MultiAreaSourceToken`, `PlanStateRevision`), zero calendar advance, zero mutation before Execute.
- Execute sets `bramble = bracken = 0` on the targeted cell(s) and leaves every Sitka cohort untouched. No cost: there is no economy in this stage.
- Stale, duplicate, out-of-polygon, unknown-area or non-existent-cell targets refuse the whole batch.
- **Caveat stated to the player model:** clearing is temporary. Next year step 5a regrows cover toward `T` at rate `0.30`; the window is one growing step of reduced `p_veg`.

## 6. RNG isolation

`SimulationRandom` (`RngModelVersion = 1`) with new keys only: `STAGE5-MAST` (woodland, per year), `STAGE5-PROMO-{areaId}-{col}-{row}-{year}`. Stage 5 must never draw from, or reorder, `"ADULT-MORT-" + TreeId`. Seed rain, establishment, growth and browsing are deterministic functions (no RNG). Exact `SimulationRandom.Roll` call shape is confirmed in plan Stage 1.

## 7. State and atomic commit

New immutable `MultiAreaStage5State` carried as one more optional member of `MultiAreaRoot` (`= null` on every model-0/Stage 1–4 root, the same pattern as `Stage4`). It holds per-area cells (undergrowth covers, cohorts, sub-threshold accumulator), pending/approved/executed clear orders, annual accounts and the Stage 5 options (`BackgroundBrowsePressure`, grid). One reference assignment publishes it with the rest of the root; a simulated pre-commit failure must leave the previous root intact. Session-only: nothing here is saved. How the regeneration version enters `MultiAreaSourceToken` without changing earlier tokens is **Open question Q3**.

## 8. Adult-empty racks

An area with 0 living adults still: advances its ecological year, receives cross-rack seed, updates undergrowth, and grows/promotes its cohorts. Whether Stage 4 currently short-circuits such an area is verified in plan Stage 1; if it does, the fix is confined to `Regeneration = 1` paths.

## 9. Open questions for the owner

- **Q1 Mast bands — resolved from source, please confirm.** The brief's `0.15 / 0.35 / 0.50` are probabilities/cumulative edges, not three sampled rolls: Unity gives good = 15 % (×1.5), poor = 35 % (×0.4, roll in `[0.15, 0.50)`), normal = 50 % (×1.0). Expected multiplier = 0.225 + 0.14 + 0.5 = 0.865. Unity rolls mast per species with the stand RNG; Stage 5 uses the isolated `STAGE5-MAST` stream per year instead (intentional, §6).
- **Q2 Promotion light gate.** No gate (Unity asset leaves it at 0). Confirm.
- **Q3 Token.** Proposal: add the regeneration version to the existing `AreaModelVersion` string only when `≠ 0`, leaving the record shape and all earlier tokens byte-identical.
- **Q4 `S5-07` tolerance.** Proposal: over ≥ 20 deterministic woodland-mast seeds, ratio of 2.5 m / 5.0 m for (a) mean relative occupancy, (b) cumulative promotions per hectare over 10 years lies in `[0.75, 1.33]`; an unscaled policy would sit near 4. Zero out-of-polygon trees is an exact requirement.
- **Q5 Block cap.** Proposal in §4 versus a fractional-promotion accumulator. I recommend the block cap (simpler, deterministic, learner-readable).
- **Q6 Browse height.** Browse `V(h)` and `p_veg` use pre-growth height (as stated for `p_veg`); confirm against `JuvenileEcologyRules` during implementation.

## 10. Exclusions

No Unity `Assets/`, Compact30, Legacy40, Enlarged80, Reference Future v1, historical saves, P0 pin (`9a99f781…`), or `/multi` HTML/JS/CSS. No Stage 5 UI controls/overlays, Oak/Beech, planting/shelters, economy, storms, graminoids, browse concealment, stem-form damage or save-schema change.

## 11. Evidence classification

| Part | Class |
|---|---|
| Seed-kernel, LightResponse, mast, browse, bramble/bracken constants | `GAMEPLAY CALIBRATION` / `TRANSFER` from Unity — not measured Irish values |
| Grid scaling policy §4 | `SIMULATION ABSTRACTION` (new, needs sign-off) |
| `CrownModel 1` initial radius on promotion | `SIMULATION ABSTRACTION` per D-094 |
| Neutral site (1.0) | deliberate neutral default, no soil data |

Qualification claims are limited to: numerical behaviour of the stated rules. No ecological forecast and no claim that regeneration is beneficial.
