# BroadLynch2006_Sitka — offline verification benchmark

Isolated, verification-only reimplementation of the Sitka spruce stand model published in
**Broad, L.R. & Lynch, T. (2006), "Growth Models for Sitka Spruce in Ireland"**, plus a
comparator for CCF aggregate yearly values. **Not** a CCF production growth engine; do not
wire it into gameplay, saves, or Reference Future. No Unity required.

Name: **BroadLynch2006_Sitka**. It is *not* "current GROWFOR": whether the distributed
GROWFOR executable uses these exact 2006 coefficients is **unverified** (extraction §12).

## Source authority

- `GROWFOR_Technical_Evidence_Extraction_v1.md` (supplied) — coefficients and locators.
- Broad & Lynch 2006 PDF (local: `~/Documents/CCF Game/GrowFor/`), per the extraction's G2 locators.
- No coefficient is invented; absent coefficient sets (non-Sitka species) are not implemented.

## Files

| File | Purpose |
|---|---|
| `broad_lynch_2006_sitka.py` | Model: growth matrices, power transform, stand-state transition, height/site, volume, thinning transition, assortment |
| `test_broad_lynch_2006_sitka.py` | Deterministic offline tests (16) |
| `compare_ccf.py` | Comparator: ingest CCF yearly B/N/H/V and project the 2006 model from the first row |
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
β2=-1.6483, β3=-0.0010, β6=-0.2384`; thinning `a=10.0991, b=0.2371, c=0.2877, d=0.5585`.

## Tests

```sh
python3 Tools/Verification/Growfor/test_broad_lynch_2006_sitka.py     # 16 tests, OK
```

Covers: `P·P⁻¹≈I`, `C·C⁻¹≈I` (relative to printed precision); recomputed inverse vs published;
power-transform round trip; `H(30)=S` and inverse round trip; split vs combined transition;
immediate composition of stocking reductions; assortment finite/`[0,1]`/sum≈1 (crop and thinning);
volume/qmd sanity.

Tolerances reflect the fact that the published `P`/`P⁻¹` pair is printed to ~4 decimals, so their
product is `I` only to printed-precision and the split/combined transition residual (~1e-3 relative)
comes from that rounding, not from a fitting error.

## Comparator

```sh
python3 Tools/Verification/Growfor/compare_ccf.py \
  --input <ccf.csv|ccf.json> --out <dir> [--run NAME] [--model unthinned|thinned]
```

**Input contract (Tier-3):** JSON `{"meta":{...},"series":[{"age","N","B","H","V"}]}`, or CSV with
recognised aliases — age (`age|year|stand_age`), N (`stems_ha|stocking|n|trees_ha|living`),
B (`basal_area_m2_ha|basal_area|ba|b`), H (`top_height_m|top_height|h`), optional V
(`volume_m3_ha|volume|v`), optional `run` (filter with `--run`). Units: age yr; N stems/ha;
B m²/ha; H m; V m³/ha. It infers site index from the first row's (age, H), then projects forward.

**Output:** `<dir>/comparison.json` and `<dir>/comparison.md` (per-year CCF vs model B/N/H/V, % diffs,
and mean/max abs %). Comparison only — it never edits CCF.

**Demonstrated comparison** (CCF growth-model-1 pure-Sitka unthinned, ages 20→120; inferred `S≈22.19`):
divergence grows with age — e.g. `H` mean |Δ|≈30.6 % (max 47.5 %), `B`≈11.3 % (max 31.6 %),
`N`≈56 % (max 98 %), `V`≈41.8 % (max 146.6 %). Interpretation is **not** a pass/fail: the two models
are structurally different (spatial individual-tree vs stand-level empirical), CCF's age-20 anchor and
site/productivity framing differ from the polymorphic site-index assumption, and the 2006 volume equation
has **no asymptote** (extraction §12; it misbehaves at very low stocking, N≈5, outside the fit envelope).
This is exactly the systematic-aggregate-divergence evidence the comparator is meant to surface.

## Source gaps / caveats (do not invent)

- The 2006 coefficient set is not proven identical to any later GROWFOR executable.
- The extraction does **not** print the exact **crop (no-thin) assortment** predictor set; the
  implementation drops the thinning-only predictors for a crop assortment and documents this
  interpretation. Thinning assortment requires `D1 ≠ D0`.
- Non-Sitka species coefficient sets are absent from the packet (not implemented).
- No numeric `B,N,H` validity box is published; projections are adequate only within the fit range
  (G2). The comparator is a benchmark, not an acceptance ground truth.
- External context anchors (G3 `1,014` m³/ha no-thin; G4 NFI bias/RMSE) are **not** asserted as tests.

## Verification

Offline tests only; no Unity. Production invariant: `git diff <base>..HEAD -- Assets Packages
ProjectSettings` is EMPTY (this tool touches only `Tools/` and one research-index documentation line).
