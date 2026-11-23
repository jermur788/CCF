# Multi #900 — Canopy cover, growing-space diagnostics and forest-floor light (2D)

**Status: APPROVED — owner decision D-094 (2026-11-23).** SPEC REQUIRED: new ecological output, shared numerical rule, cross-rack interaction and player-facing readout. This pass is **engine-first**: it proves the calculations inside the pure 2D engine and automated tests, and does **not** add a map overlay, heatmap or ground inspector.

## 0. Authority and source readback (2026-11-23)

- 3D kernel [`ForestEcologyController.cs`](https://github.com/jermur788/CCF/blob/main/Assets/ForestPrototype/ForestEcologyController.cs), blob `0290a7beab639e48585f97c6af8cf993959b9f37`. `RecomputeCanopy()` evaluates living tree centre, `CrownRadius` and `Height` on 5 m cell centres: `reach = radius × 1 + cell/2`; `lateral = clamp01(1 − d/reach)`; `vertical = clamp01(h/8)`; `influence = lateral × vertical`; `gap = Π(1 − influence)`; `Canopy = 1 − gap`; `Light = gap`. The 1× reach and `h/8` are documented `k10a10` gameplay calibration, **not calibrated PAR**. `RecomputeCanopy` skips `IsStump`; `FindTrees()` excludes `IsBiologicallyDead`, so only living trees cast.
- Unity annual order (~474–495): competition → pre-growth canopy/light → DBH/height growth → crown relaxation → density mortality → post-growth canopy/light → regeneration. Batch Fell recomputes once after the batch.
- Current 2D engine [`AnnualRules.cs`](https://github.com/jermur788/CCF/blob/a692e1bf5d21304d06e696db1f47d1988ff80ee1/Tools/MultiRackSitkaP1Lab/src/Core/AnnualRules.cs), blob `dc299b32…`: per-tree adult competition/growth and crown updates, no area cell canopy/light state. The multi-area contracts and fenced commands already exist in `src/Core/MultiArea/`.
- `MultiAreaGenerator` places Area A and Area B in **one integer-millimetre world frame** and they share the boundary segment `(36,2)–(40,18)` m. The former draft statement that the areas are fully independent is superseded by D-094.
- The 2D page artistically magnifies/displaces focused crowns. **Never use SVG screen geometry for ecology.** Use authoritative tree X/Z, height, true `CrownRadiusM`, living state and the rack polygon.
- `#899` is now decided (D-094): `CrownModel 1` supplies the realised crown radius fed into this layer.

## 1. Terms and outputs (do not conflate)

1. **Growing-space competition** = the existing Hegyi `CI`, living neighbour count within 8 m and stand density/basal area. It is not a mechanically allocated area per tree and does not itself change growth, mortality or crown rules.
2. **Geometric crown cover** = polygon-clipped **union** of authoritative living crown projections ÷ rack area. The sum of crown areas is not cover. A circular crown projection is an explicit geometric simplification.
3. **Forest-floor light** = a separate 0–1 index sampled at cell centres. It is **not** equal to the uncovered fraction and is **not** measured PAR.
4. **Light field** = area-local sampling of the same physical trees and polygon, in metres and hectares. Rack summaries are per rack; the tree neighbourhood is the connected woodland (§1.5).
5. **Basal area, live density and crown cover** are three different variables and stay separately reported.

### 1.5 Connected-woodland rule (D-094)

- Living trees from **both racks** contribute to competition, crown overlap and shade reach across the shared border; a tree within 8 m of the border must see its cross-border neighbours.
- The outer perimeter of the combined woodland borders **open grassland**: no synthetic neighbours and no ghost trees outside the site.
- Canopy/light output and summaries are computed and reported **per rack polygon**. Do not blend Area A and Area B into one percentage. Area B is not an experimental control for Area A.

## 2. This pass: engine-first proof, no overlay

- Implement a pure, read-only `CanopyLightEvaluator` over immutable engine-authoritative living trees and each rack polygon.
- Demonstrate in automated tests and engine output: a matched duplicate of the initial connected woodland (untreated control), a real Work Plan **Mark → Approve** (assert zero canopy/light change), **Execute** a multi-tree Fell in Area A, then Year +1 and +3.
- Report before/after and control deltas: removed TreeIds, geometric cover %, uncovered m², largest opening width, affected 2.5 m cells, light-index shifts, cross-border cells and measured runtime.
- **No `/multi` heatmap, overlay toggle or ground inspector in this pass.** The existing navy map, SVG, layout and controls are unchanged. A later, separately approved slice may consume these read-outs in the UI.
- Values are session-only, version-stamped and carry **zero save-schema change**.

## 3. Approved light model — `unity-k10a10-decoupled-v1`

For each sample cell centre `x` inside the rack polygon and every **living** tree `i` in the connected woodland:

```text
reach_i     = r_i * 1.0 + 2.5                      metres (fixed physical buffer)
lateral_i   = clamp01(1 - distance(x, p_i) / reach_i)
vertical_i  = clamp01(h_i / 8.0)
influence_i = clamp01(lateral_i * vertical_i)
lightIndex(x) = Π_i (1 - influence_i)               deterministic TreeId order
```

- **Grid-decoupled:** `b = 2.5 m` is fixed and independent of cell width. The former `+ cellWidth/2` is replaced. At a 5.0 m grid this equals Unity's half-cell exactly, which is why 5.0 m is the parity oracle.
- **Default grid 2.5 m**, clipped to the rack polygon, expanded by a trimmed boundary-cell area; area-weight rack summaries by the clipped cell area. **5.0 m mode is retained** as the exact Unity parity oracle.
- Output is finite and bounded `[0,1]`. Empty stand = 1.0.
- Label: **`Modelled Light Index (Unity k10a10 proxy — not calibrated PAR)`**. Version `lightModelVersion = "unity-k10a10-decoupled-v1"`.
- **No calibrated Beer–Lambert/PAR model in this pass.** Do not invent LAI, extinction coefficients, crown opacity or percent PAR.

### Model metadata

`LightModelMetadata` carries: `version`, `kind = "index"`, `calibratedPar = false`, `gridCellM` (2.5 or 5.0), `shadeBufferM = 2.5`, the caveat text, and uncertainties: C2 crown bias (~+5–15 % mean radius), Area A high stocking (~74 m²/ha), open-grassland outer-edge brightening, circular-crown simplification, and non-causal diagnostic status.

## 4. Grid and boundary policy

- 0.25 m micro-grid for the geometric union; 2.5 m default for light; 5.0 m parity mode.
- Cells whose centre is outside the polygon are excluded; boundary cells are clipped and weighted by clipped area. No renormalisation that hides edge loss.
- Two isolated areas are never averaged. Cross-border tree influence is allowed; cross-border *summary* aggregation is not.

## 5. Accuracy checks during development

**A — Provenance/geometry (read-only):** confirm exact source SHA, polygons, tree IDs, metres, living/dead semantics, crown model version and area units. Report radius distribution, basal area, union and gap sizes before coding. Do not use nominal crown sums.

**B — Numerical oracle:** 0/1/2-tree and overlapping cases; empty stand = 1.000; single mature overhead = 0.000; single 4 m young overhead = 0.500; drip-line edge `r = 1.73 m` → 0.409 at both 5.0 m and 2.5 m; narrow two-tree gap ≈ 0.349 with 0 % union overlap; dense lattice shows no 13× grid-size artefact; stump/dead exclusion = 1.000. Exact 5.0 m parity against the Unity equation.

**C — Forestry provenance (not a fit):** cite Hale et al. 2009 (BA and stems/ha both affect transmittance), COFORD Ballinagappoge canopy openness 8.3/11.3 % after thinning and 5.5/7.2 % two years later, and Forest Research light guidance. These are **site-specific openness/transmittance observations**, not a calibration target for this index. Keep `IRISH DIRECT / UK TRANSFER / MODEL ASSUMPTION / GAME TUNING` labels; missing PAR/LAI stays `UNKNOWN`.

**D — Behaviour:** matched control vs actual 10-tree Fell, years 0/1/3, exact IDs, cover, uncovered area, opening dimensions, light deltas, surviving radius changes; Approve alone must change nothing.

**E — Limits:** deterministic repeat; finite 0–1; empty canopy bright; no signal from felled/dead crowns; 2.5/5.0 sensitivity documented; measured runtime on the real 171/73 tree counts.

## 6. Boundary and future dependency

**INCLUDED:** pure evaluator, area-cell geometry, versioned read-only output contracts, recompute after Execute and after annual growth/death, per-tree growing-space diagnostics, canopy cover and gap summaries, targeted numerical qualification.

**NOT INCLUDED:** new recruitment, planting, browsing, seed rain, species curves, forest-floor vegetation causality, LAI/photosynthesis, storms, economics, save/load, Unity code changes, map overlay/heatmap/inspector, or any change to historical Unity ecology.

Frozen P0/Legacy40/Reference Future v1, existing MA-1 baselines, old fixtures, source fences, Work Plan approvals and deterministic replay remain protected. Session-only derived values; explicit version identity in responses; predictable cache invalidation on Execute, natural death and annual crown growth.

## 7. Decision gates

- **G5/Owner — satisfied (D-094):** crown model, light model, grid, buffer and labels fixed above.
- **G4/Manager — satisfied by this APPROVED quartet plus the D-091 lane receipt:** one writable lane, one writer, independent reviewer.
- **D-091 physical custody:** required before the implementation actor starts; no Unity lease, no Editor.
- **G3 calibration:** targeted offline numerical checks only; no broad multi-year/Unity matrix.
- **G2/Owner:** after a real read-out demonstration, judge usefulness and integrity; review ≠ acceptance.

**Source/evidence confidence:** implementation facts re-read from source; scientific studies sourced; MA-A/B light values **not yet computed**; biological fidelity **not established**; no implementation claimed by this document.
