# Adult density mortality — calibration record (growth model 1)

Manager decisions: keep the local Hegyi competition index; add a **separate** stand-density signal from the British Sitka maximum size–density relationship. That signal drives self-thinning mortality only, never DBH growth. Drop the unsourced 80–90 m²/ha ceiling.

## Density evidence

**Comeau, P.G., White, M., Kerr, G. & Hale, S.E. (2010)**, *Maximum density–size relationships for Sitka spruce and coastal Douglas-fir in Britain and Canada*, Forestry 83(5): 461–468.

From the published abstract:
- the British Sitka slope of log N on log Dq is **−2.063**, steeper than Reineke's −1.605;
- the maximum SDI is **1,868**.

Site quality shifted the line's position slightly but not its slope (abstract, and the authors' conference summary). The full equation table was not accessible here. The intercept is therefore expressed through the published maximum SDI at the standard Reineke reference diameter of 25 cm; check this against the paper before integration.

Equation used (`SitkaGrowthModel.RelativeDensity`):

```
RD = N_ha × (Dq_cm / 25)^2.063 / 1868        (dimensionless)
```

- **Inputs:** N_ha is living stems per hectare over the whole stand area, all species. Dq is the quadratic mean DBH of living trees, in cm.
- **RD = 1** is the British Sitka maximum size–density line.

Transfer limitation [B]:
- The relationship is British Forestry Commission data, not Irish, so treat 1,868 as a validation and maximum-density starting point.
- It is fitted for Sitka. The rare broadleaf recruits are counted in N and Dq without species weighting.

Forest Yield (Forest Research) unthinned Sitka outputs were **not obtained**: the tool's outputs are not available as published tables. They remain a recommended validation source.

**Starting stand:** RD 0.435 (2,100 stems/ha, Dq 15.8 cm).

## Candidates tested

Each candidate ran in the diagnostics harness as a prototype over 100 years × 4 regimes, on unchanged production growth (DBH dynamics identical under either height model). Data: `Evidence/calibration_round*_*.csv`, `*_facts.txt`.

| Round | Rule | Grid | Result |
|---|---|---|---|
| 1 | Hazard `m·P²·S²` per tree, unnormalised | onset 0.5 / 0.6 / 0.7 × m 0.05 / 0.10 / 0.20 | **Rejected:** RD overshoots to 1.21–1.64; basal area 106–143 m²/ha at 120; peaks to 13 %/yr. Deaths fall almost only on small suppressed trees, while large-tree DBH growth (not density-dependent, by decision) keeps raising SDI |
| 2 | Vulnerability normalised across the stand: `m·P²·S²/mean(S²)` | Same grid | Better (basal area 77–102 m²/ha), but RD still exceeds 1 late (to 1.18) and single-year rates reach 13–21 % |
| 3 | Round 2 plus a **maximum-line boundary**: if RD would exceed 1, the most suppressed survivors die until RD ≤ 1 | onset 0.5 / 0.6 / 0.7 × m 0.02 / 0.04 / 0.08 | All 36 runs stay at RD ≤ about 1.03 and basal area settles at 82–90 m²/ha (what the line implies at Dq ≈ 60 cm, not an imposed number). Average losses are 1–4 %/yr; thinning lowers deaths |

**Single-year spikes** in round 3 (22–50 % of all stems) were traced year by year: they are **regeneration recruit pulses**.
- Example: in the unthinned run at year 84, 19 of 22 recent recruits died against 1 of 48 planted trees.
- Small recruits under a closed canopy at the maximum line are the most suppressed and are removed first.
- The planted cohort's worst single year was 7–10 %; its typical loss is 1–3 %/yr.

## Selected [C]

| Parameter | Value | Basis |
|---|---|---|
| Onset RD | 0.6 | Middle of the tested range; density mortality is negligible below it. The starting stand (0.44) is below it; unthinned stands reach it at about age 26 |
| Strength m | 0.08 | The hazard does most of the thinning (not only the boundary), with the clearest thinning response (unthinned 596 vs thinned 400–509 prototype deaths) |
| Pressure | `P = max(0, (RD − 0.6)/0.4)`; probability uses `P²` | Smooth onset, steepening toward the line |
| Vulnerability | `S⁴ / mean(S⁴)`, S = current Hegyi suppression | Calibration used S². Production uses S⁴ after fixture testing showed S² let even-sized dominants die at about 2 %/yr. Normalisation keeps the expected stand rate (`m·P²`) unchanged; long-run production runs confirm the density outcome |
| Per-tree cap | 0.5/yr | Numerical bound |
| Boundary | RD ≤ 1 | [B] maximum line |

**Not used:**
- a universal basal-area cap;
- density in DBH growth;
- per-tree stress state (the hazard is stateless and recomputed from the current competition index).

**Existing suppression state:** `equivalentSuppressedYears` is persisted (v6+), but it is a cumulative diagnostic that never decreases after release. It is not authoritative for current vulnerability and is not used.

## Production check (growth model 1, `Evidence/growth_model1_stand_development.csv`)

See `SitkaGrowthModel1.md` for the full tables.
- **Unthinned:** RD 0.435 → 0.69 (age 30) → 0.91 (60) → 0.97–0.99 (80–120). Stems 2,100 → 256/ha at 120; basal area 83.8 m²/ha; volume 1,219 m³/ha (old model: 416 m²/ha and 5,937 m³/ha).
- **Deaths over 100 years:** unthinned 609, light 562, moderate 520, heavy/selective 473.
- **Fixture (RD ≈ 0.9, 10 years):** the most-suppressed quarter lost 65 %, the least-suppressed quarter 14 %. Thinning 30 % of stems cut deaths from 73 to 22.
