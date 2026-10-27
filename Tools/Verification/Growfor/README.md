# BroadLynch2006_Sitka — offline verification benchmark

Isolated, verification-only reimplementation of the Sitka spruce stand model published in
**Broad, L.R. & Lynch, T. (2006), "Growth Models for Sitka Spruce in Ireland"**, plus a
comparator for CCF aggregate yearly values. **Not** a CCF production growth engine; do not
wire it into gameplay, saves, or Reference Future. No Unity required.

Name: **BroadLynch2006_Sitka**. It is *not* "current GROWFOR": whether the distributed
GROWFOR executable uses these exact 2006 coefficients is **unverified** (extraction §12).

## Source authority

- `GROWFOR_Technical_Evidence_Extraction_v1.md` (external research extraction, **not tracked in Git**)
  — coefficients and locators. The canonical Git-side source is this directory.
- Broad & Lynch 2006 PDF (local: `~/Documents/CCF Game/GrowFor/`), per the extraction's G2 locators.
- No coefficient is invented; absent coefficient sets (non-Sitka species) are not implemented.

## Files

| File | Purpose |
|---|---|
| `broad_lynch_2006_sitka.py` | Model: growth matrices, power transform, stand-state transition, height/site, volume, thinning transition, assortment |
| `test_broad_lynch_2006_sitka.py` | Deterministic offline model tests (24) |
| `compare_ccf.py` | Comparator: ingest CCF yearly B/N/H/V and project the 2006 model from the first row |
| `test_compare_ccf.py` | Deterministic offline comparator tests (18) |
| `examples/ccf_growthmodel1_unthinned_sitka.json` | Extracted CCF growth-model-1 pure-Sitka unthinned slice (provenance in `meta`) |

## Equations implemented (with locators)

| Item | Form | Locator |
|---|---|---|
| Height / site (polymorphic Bertalanffy–Richards) | `H(t)=a[1-(1-(H0/a)^c)e^{-b(t-t0)}]^{1/c}`; `b` from `S` and eq. (5); `S=H(30)` | extraction §4.5–4.6, §5 (G2 p.5–6, p.12) |
| Power transform | `x^C = exp(C ln x)` | §4.3 (G2 p.4–5) |
| Transition between thinnings | `x2 = {a + P^-1 exp[Λ b Δt] P (x1^C - a)}^{C^-1}` | §4.8 (G2 p.8) |
| Thinned coefficients | `P, Λ, P^-1, C, C^-1, a=[0,0.620599,5.812518]` | §6.1 (G2 Box 1, p.27) |
| Unthinned coefficients | `P, Λ, P^-1, C, C^-1, a=[0,7.068916,5.812518]` | §6.2 (G2 Box 2, p.28) |
| Stand volume | `V=B(β0+β1H+β2H/√N+β3NH/B+β6B/H)` | §8.2 (G2 pp.20–21) |
| Thinning transition | `lnB=-(1/b)ln[B0^-b-(ab/c)H^d(N^c-N0^c)]` | §9.2–9.3 (G2 p.9, p.21) |
| Assortment (UNML) | `P(Y=j)=e^{η_j}/(1+Σe^{η_k})`, `P(Y=0)=1/(1+Σe^{η_k})` | §10.3–10.4 (G2 pp.22,29) |

Constants: height `a=47.0216, c=0.4571, t0=-5.0, H0=0`; volume `β0=1.0013, β1=0.5295,
β2=-1.6483, β3=-0.0010, β6=-0.2384`; thinning `a=10.0991, b=0.2371, **c=-0.2877, d=-0.5585**`.

### Source-fidelity correction (thinning signs)

The printed Broad & Lynch 2006 thinning coefficients are **`c = -0.2877`** and
**`d = -0.5585`** (both negative; source locator G2 p.21 / extraction §9.3). An earlier
revision of this benchmark transcribed them with positive signs, which flips the sign of
`(N^c - N0^c)` and returns a nonphysical near-zero basal area. With the printed signs the
published reference case **B0=30 m²/ha, N0=1800 stems/ha, H=16 m, N=1200 stems/ha gives
B ≈ 23.7697 m²/ha** (with the old positive signs it gives ≈ 2.25e-7 m²/ha). The benchmark
docs, code comment and tests now use the printed negative signs and independently reproduce
the reference value.

## Initialization policy (explicit, not fitted)

The comparator infers site index `S` from a **single observed** `(age, top height)` pair —
the first row — by inverting the polymorphic height equation (`b = -ln(1-(H/a)^c)/(age-t0)`;
`S = a[1-(1-(H0/a)^c)e^{-b(30-t0)}]^{1/c}`; `S` = top height at age 30). `S` is never fitted
across the trajectory and the model is never calibrated to minimise disagreement. Every output
records the anchor age, the anchor top height, the resulting `S` and the method.

## Tests

```sh
python3 Tools/Verification/Growfor/test_broad_lynch_2006_sitka.py   # 24 tests, OK
python3 Tools/Verification/Growfor/test_compare_ccf.py              # 18 tests, OK
```

Model coverage: `P·P⁻¹≈I`, `C·C⁻¹≈I` (relative to printed precision); recomputed inverse vs
published; power-transform round trip; `H(30)=S` and inverse round trip; **published-orientation
transition** vs an independent 3×3 product and the **swapped-orientation mutant killed**;
split vs combined transition (residual is printed-coefficient rounding, not exact semigroup);
**numeric thinning reference 23.7697**, `N==N0` returns `B0`, moderate thinning plausible,
successive reductions compose, and the **positive-sign mutant killed**; assortment finite/`[0,1]`/
sum≈1 for crop and thinning plus invalid-domain rejection; volume/qmd sanity and **raw negative
low-stocking volume not clamped**.

Comparator coverage: canonical schema accepted, ambiguous short names / unknown columns /
duplicate-dimension columns rejected; run selection, unknown run, multiple runs, run without a
run column; non-finite, duplicate-age, non-monotonic, vacuous and insufficient series rejected;
explicit initialization metadata; **zero/absent denominator percentages left undefined (never 0)**;
**negative model volume flagged `OUTSIDE_USEFUL_DOMAIN`, not clamped**.

Tolerances reflect that the published `P`/`P⁻¹` pair is printed to ~4 decimals; residuals are
printed-precision effects, not fitting errors. Repeated-step/split discrepancies are bounded by
that precision and are **not** claimed to be exact semigroup behaviour.

## Comparator

```sh
python3 Tools/Verification/Growfor/compare_ccf.py \
  --input <ccf.csv|ccf.json> --out <dir> [--run NAME] [--model unthinned|thinned]
```

**Input contract (Tier-3):** canonical, unit-bearing names only —
`age` (years), `stems_ha` (stems/ha), `basal_area_m2_ha` (m²/ha), `top_height_m` (m),
optional `volume_m3_ha` (m³/ha), optional `run`. Unambiguous synonyms are accepted
(`age_years`/`stand_age`; `stems_per_ha`/`trees_ha`/`stocking_ha`; `basal_area`;
`top_height`/`height_m`; `volume`). Ambiguous short names (`n`, `b`, `h`, `v`, `ba`,
`stocking`), unknown columns and duplicate-dimension columns are **rejected**. `--run NAME`
must select a series: an unknown run, multiple runs without `--run`, or `--run` with no run
column fail. Non-finite values, duplicate ages, non-monotonic ages, and empty/insufficient
series are rejected. JSON uses the same rules (`{"meta":{"run":...},"series":[...]}`).

**Output:** `<dir>/comparison.json` and `<dir>/comparison.md` — initialization metadata, per-year
CCF vs model B/N/H/V, % differences with an explicit denominator, undefined percentages reported
(never zero), `domain` flags, and a mean/max abs % summary over defined in-domain rows only.

**Demonstrated comparison** (CCF growth-model-1 pure-Sitka unthinned, ages 20→120; inferred
`S≈22.19334`): divergence grows with age — `B` mean |Δ|≈7.9 % (max 20.7 %), `N`≈49.1 % (max 91.0 %),
`H`≈27.8 % (max 44.0 %), `V`≈24.4 % (max 39.1 %). The final age-120 model row reaches
`N≈4.69 stems/ha` and `V≈-567.69 m³/ha`; it is **flagged `OUTSIDE_USEFUL_DOMAIN` and excluded from
the summary**, not clamped (the published volume equation has no low-stock asymptote). These
numbers are recorded observations, **not** golden pass/fail values, and divergence is **not**
evidence that CCF is wrong. The primary eventual use is final-model-2 comparison, not model-1
calibration.

## Volume definition and domain

- Broad & Lynch volume is the **published stand-volume equation** using the source's own
  small-end/utilisation convention (**7 cm small-end diameter**, per review). It is **not
  asserted identical to CCF's biological stem-volume measure**; `CCF V` vs `BL V` is a
  diagnostic comparison of related but differently-defined quantities.
- The 2006 volume equation has **no protective low-stock asymptote**: below the fit range
  (e.g. N≈5 stems/ha) it returns nonphysical (often negative) volume. The comparator retains the
  raw prediction and flags the row `OUTSIDE_USEFUL_DOMAIN`; it excludes or separately identifies
  percentage summaries whose interpretation becomes misleading.

## Source gaps / caveats (do not invent)

- The 2006 coefficient set is not proven identical to any later GROWFOR executable; executable
  equivalence to later GROWFOR versions is **unverified**.
- Broad & Lynch 2006 Sitka specification only: **no Oak or Beech coefficients**, no
  mixed-species validation, no empirical irregular/CCF validation established here.
- Early-growth and data-range limitations apply; **no supported numeric `B/N/H` validity box**
  is published, and projections are adequate only within the source fit range.
- Volume definition differs from CCF biological volume (7 cm small-end convention).
- Low-stocking equation behaviour can become nonphysical.
- The extraction does **not** print the exact **crop (no-thin) assortment** predictor set; the
  implementation drops the thinning-only predictors for a crop assortment and documents this
  interpretation. Thinning assortment requires `D1 ≠ D0` and positive `B1`,`N1`.
- The comparator is a diagnostic envelope, **not a production engine** and not an acceptance
  ground truth. External context anchors (G3 `1,014` m³/ha no-thin; G4 NFI bias/RMSE) are not
  asserted as tests.

## Verification

Offline tests only; no Unity. Production invariant: `git diff <base>..HEAD -- Assets Packages
ProjectSettings` is EMPTY (this tool touches only `Tools/`). The GROWFOR 7-species/13-option
wording is a **canonical research-index correction held separately** for the next G4/context
refresh; it is deliberately **not** carried on this tooling branch.
