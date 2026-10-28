# PARITY-03 Q11 — mixed-density verification record corrections

**Status:** AUTHORITATIVE CORRECTION NOTE for the non-canonical working records in this folder.  
**Date:** 2026-10-28  
**Authority:** approved `specs/scenario-one-three-species-parity-p0/`, especially FR-10 and FR-12; accepted independent review `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Review/HANDOFF.md`.

This folder contains historical working records from earlier three-species development. Those records are useful evidence, but they are not current qualification authority. Where an older statement conflicts with the approved parity specification, current source, or later accepted qualification, this correction note controls the interpretation of that statement. Historical measurements are not rewritten.

## 1. Growth-model default statements are historical

The following old statements are **superseded historical status**, not current product state:

- `Phase1-DensityRework.md`: the statement that growth model 1 remains the new-game default / model 2 is not adopted.
- `CompositeDensityV3Handoff.md`: `Not merged. Growth Model 2 is still not the default.`

Current authority is FR-10 of the approved parity specification. Current `ScenarioOneManager.NewGameGrowthModel` is `GrowthModel.SpeciesAware` (model 2). Existing saves on models 0/1 retain their saved behaviour; frozen Reference Future v1 remains model 0. The older statements remain valid only as descriptions of the repository state at the time those working records were written.

## 2. Earlier Unity figures are historical evidence, not current requalification evidence

Unity measurements recorded in `CompositeDensityV3Handoff.md` were produced on the older density branch/base identified by that record, including source head `90e3496` for the quoted V3 runs. They may be cited as **historical development evidence** for that exact tested lineage.

They must **not** be presented as current PARITY-03 requalification evidence for the later Rack/integration base. Current PARITY-03 qualification is the accepted Tier-2 handoff and Manager review under `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Unity/` plus the accepted independent static review. A historical numeric result such as the old continuity measurement is not silently promoted to a current-base measurement.

## 3. FR-12 evidence vocabulary controls

FR-12 authoritative evidence classes are:

- `DIRECT`
- `TRANSFER`
- `MODEL DERIVATION`
- `ORDINAL REPRESENTATION`
- `GAMEPLAY CALIBRATION`
- `FALLBACK-CONSERVATIVE`
- `UNSUPPORTED — NEEDS DECISION`

Legacy working tags in this folder are coarse shorthand only. They do not create additional evidence classes.

| Legacy tag | FR-12 interpretation for these density records | Rule |
|---|---|---|
| `[ORD]` | `ORDINAL REPRESENTATION` | Use for rank/order claims such as Reference < Intermediate < High. It does not imply measured spacing, carrying capacity, SDI or a biological maximum. |
| `[C]` | `GAMEPLAY CALIBRATION` | Use for selected simulation values such as occupancy normalization, loss-scale values, numerical class magnitudes and victim-allocation tuning when they are not empirical coefficients. |
| `[B]` | `TRANSFER` **only where the record explicitly means transferred British Sitka evidence** | `[B]` is not a universal project class. In the density records it describes British Sitka evidence transferred into the accepted Sitka route. Do not reuse `[B]` without the explicit source/context. |
| `[D]` | **No universal automatic mapping** | This is a legacy working tag. Reclassify each occurrence from its stated source and claim. Do not silently treat `[D]` as `DIRECT`, `MODEL DERIVATION`, or any other FR-12 class merely from the letter. |

Additional rules:

- Geometry/algebra that follows mechanically from implemented values may be described as **model derivation** where that is the actual claim, but the underlying calibrated values remain `GAMEPLAY CALIBRATION` or `ORDINAL REPRESENTATION` as appropriate.
- A direct source range does not turn a selected game reference value into `DIRECT`; selected game values remain `GAMEPLAY CALIBRATION` unless the implementation is genuinely using the direct guidance itself as the represented value/rule.
- `UNSUPPORTED — NEEDS DECISION` authorises no runtime value.

### Density-specific mapping

The old `Phase1-DensityRework.md` evidence table should therefore be read as:

- **Route A, British Sitka size-density line:** `TRANSFER` for the British Sitka evidence plus `GAMEPLAY CALIBRATION` for the accepted simulation calibration around it; protected Sitka implementation remains governed by its own accepted evidence record.
- **Route B growing-space class rank:** `ORDINAL REPRESENTATION`.
- **Route B numerical weights, occupancy normalization, bounded-loss parameters and victim-allocation constants:** `GAMEPLAY CALIBRATION`.
- **Stem cross-section calculation:** model geometry/derivation, not empirical species density evidence.
- **Hegyi index use:** inherited model structure; any empirical/source claim must be taken from its own accepted evidence record rather than inferred from this shorthand table.

The core honesty claims remain valid: Route B is not an Oak/Beech SDI or maximum-density equation; there is no DEC/DMC/mixture-productivity bonus; and the numerical occupancy weights are not biological m²/ha capacities.

## 4. README check counts are historical snapshots

The command comments and phase-table check counts in this folder's `README.md` describe the earlier programme snapshot. They are **not current suite totals or current qualification status**. Later parity work expanded and changed the verification suites; for example, the later composite-density offline suite alone reached 107 checks in the independent review snapshot.

Use current task handoffs and current verification tooling output for present check totals. Do not compare a current run against the README's `density 35 / timber 71 / history 70` line as though those are still acceptance totals.

## 5. What this correction does not do

This note does **not**:

- change any production behaviour or parameter;
- rebaseline any anchor or historical measurement;
- claim new Unity verification;
- resolve PARITY-03 Q10 continuity-strengthening;
- repair verification-scanner policy (`NoBiologicalDensityClaim` scope / density-file allow-list);
- change Rack, MFM, S1-C, S1-D or save/schema behaviour.

PARITY-03 Q10 remains a separate verification-strength task: broaden the continuity sweep across Legacy40 and Enlarged80, Intermediate and High replacements, heterogeneous DBH/CI in a realistic low-loss regime, use a meaningful relative tolerance, and add a controller-level 0→1 non-Reference continuity measurement.
