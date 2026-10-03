# Scenario 1 — ecology, regeneration and verification completion

Stream: Claude, `task/scenario-one-ecology-completion`. Shared BASE: `integration/scenario-one-base` at `9238e56` (= `bb314a4` + the five browsing commits; tree identical to `4dc6713`). Context commit: `db2b4f1`.

Scope: the ecology, readout and verification side of the minimum Scenario 1 loop.

- No biological rule changed.
- No persisted field was added; save schema stays v14.
- No file outside Claude's ownership was edited. Changes needed elsewhere are listed under **Integration patches**.

Biology anchors after this stream, reproduced by the gates listed under Verification:

| Anchor | Value |
|---|---|
| Neutral lifecycle (browse pressure 0) | `BFC55473C1506067` |
| Normal Scenario One lifecycle (pressure 0.2 [C]) | `3485B6630C9EA448` |
| Ecology calibration measurement | `D48A19525E69DD8A` |
| Reference Future v1 Year 100 | `7AD177B3CC2F73C7` |

## Regeneration diagnosis (Phase C)

`Assets/ForestPrototype/RegenerationDiagnosis.cs` is read-only and static. It uses the existing shared rules: `LightResponse`, `SurvivalResponse`, `CanPromote` and `AssessBrowse`.

`RegenerationDiagnostics.Diagnose(ecology, position, plantedJuveniles, spawner)` picks its subject as follows:
- the nearest living, unpromoted exact planted juvenile within 0.75 m;
- otherwise the tallest cohort in the cell.

It returns species, stage, height, light and light band, browse assessment, escape height, shelter steps remaining, promotion state and a limit. The limit is chosen in this priority order:

| Limit | Condition |
|---|---|
| `Promoting` | `CanPromote` holds now |
| `LightLimited` | Light survival < 1, growth response below the species' poor-light threshold, or light below promotion minimum |
| `AboveBrowseReach` | Height vulnerability is 0 |
| `Protected` | Access is 0 (effective shelter or intact fence) |
| `BrowsedExposed` | p > 0. Reports "N%/yr leader risk (band)", plus expired/failed shelter or breached fence |
| `Growing` | Neither light nor browsing limits |
| `NoneEstablished` | No juvenile at the point |

Further details:

- **Timing.** Browse exposure, protection and shelter steps describe the **upcoming** annual step (`EcologicalYear + 1`). The browse queries gained an optional `year` argument for this. The annual step still uses the current year, so biology is unchanged.
- **Last-year browsing.** This is reported only when recorded by the latest annual step in this session. After a load it is "n/a", because it is not saved.
- **Ground report.** The aimed-ground report keeps its existing line, which ends "browsing low", and adds one diagnosis line above it.
- **Fixtures** (`BrowsingProtectionVerification`, `REGENERATION_DIAGNOSIS_VERIFY_PASS`): dark cohort → light-limited; bright exposed oak → 20%/yr; sheltered oak → protected, 8 steps left (7 after one step); 2.0 m oak → above reach; 3.6 m oak → promoting; empty cell → none established. Also checks that diagnosis is read-only and that last-year browsing reads "n/a" after a load.

## Shelter timing contract (Phase D1)

> **Rule.** When a shelter is installed together with (or in the same resolution as) a planting order, write `installedYear = report.year`. That is the `AdvanceYear` resolution year, `ecology.EcologicalYear + 1` at resolution time, equal to the new juvenile's `plantingYear`. The shelter then protects exactly `effectiveYears` annual steps, starting with the step immediately after resolution.

Fixture (`SHELTER_TIMING_CONTRACT_PASS`), 3-year shelters:

| installedYear | Protected steps |
|---|---|
| resolution year | 1, 2, 3 |
| resolution + 1 | 2, 3, 4 (first step unprotected) |
| resolution − 1 | 1, 2 (one step lost) |

A shelter added to an existing juvenile in a later year's resolution follows the same rule: `installedYear = report.year` of that resolution.

## Restore contract (Phase D2)

> **Rule.** On **every** restore, run `ecology.Browsing.ClearProtection();`, then add the saved shelters, then the saved protected areas. Add copies, never the save object's own lists.

This applies to:
- v1–v14 saves, which have no records, so the result is cleared protection;
- v15 saves;
- Reference preview entry (a frozen v12 world, so no protection during the preview);
- Reference preview exit (`previewReturnData` must carry the v15 records).

Fixture (`PROTECTION_RESTORE_CONTRACT_PASS`, production annual step, pressure 0.7, shelters plus a fenced area):

| Continuation | Hash |
|---|---|
| Uninterrupted | `CE7B33BE7F903504` |
| Clear, then restore | `CE7B33BE7F903504` (equal) |
| Clear without restore (negative control) | `0F6172E9654B7F91` (differs) |

Without the Clear, a restore duplicates the records. Biology is unchanged, because the first match wins, but the shelter visuals double (12 instead of 6).

## Shelter visuals (Phase D3)

`Assets/ForestPrototype/ScenarioProtectionVisuals.cs`:

- **Self-bootstrap.** Starts after scene load when a `ForestEcologyController` exists. No scene or prefab edits.
- **Source.** Derived only from `Browsing.Shelters`. It rebuilds when the records or the year change.
- **Tubes.** Plain cylinders, 1.2 m tall and 0.1 m in radius, using a code-created URP Lit material. No colliders or shadows.
- **States**, for the upcoming step:
  - effective: upright, pale green;
  - expired: grey;
  - failed: brown, leaning;
  - not yet installed: hidden.

Fixture (`SHELTER_VISUALS_VERIFY_PASS`): 3 effective + 1 expired + 1 failed + 1 future gives 3 effective visuals, 5 drawn and 5 active children. After `ClearProtection`, there are 0.

## Annual review lines (Phase E)

`Assets/ForestPrototype/ScenarioEcologyReviewLines.cs` is a pure formatter producing up to six lines:
- canopy and light change;
- opened cells (recent opening) and their light;
- regenerating cells by species, with legacy-planted counts;
- planted trees growing / promoted / lost;
- browse band, plus last year's browsing where recorded;
- protected juveniles (sheltered or fenced) and shelters expiring within 2 steps.

It is not called from `ScenarioOneManager`; see the integration patches. Fixture: `ECOLOGY_REVIEW_LINES_VERIFY_PASS`.

## Completion harness (Phase F)

`Tools/Verification/ScenarioOneCompletionVerification.cs` uses the live manager flow on ForestTest. It writes no save and restores the scene world.

| Phase | Result |
|---|---|
| P0 start | 336 Sitka, Year 0, cash 1,200,000 c, pressure 0.2, no shelters |
| P1 inspect | Tree data, ground report shows "browsing low", diagnosis gives a reason |
| P2 plan | 16 crop trees (largest per 10 m block). 72 fellings (27.2% basal area, competitor release); 4 kept as fallen deadwood |
| P3 resolve | Year 1, exactly the marked trees removed, 4 deadwood records |
| P5 plant/protect | 4 oak + 4 beech pairs in the brightest cells. Fixture shelters follow the timing contract; shelters effective in the first step |
| P4 respond | Year 6: thinned cells gained 0.056 light vs 0.053 for unthinned; canopy 0.951 → 0.897 |
| P6 save | In memory, v14 |
| P7 continue vs restore | Year 12, `67C8F4F898DF0CA5` both ways |
| P8 regeneration | Year 6: 8 pairs, sheltered ≥ exposed in every pair, 2 strictly taller (mean 2.13 vs 2.02 m). Year 12: all oak promoted, both groups |
| P9 decades | Second thinning (49 trees, 20.2% basal area) at Year 12. **Completed at Year 25** with every objective met. Year 40 hash `DAC3DBD189785831` |
| P11 negative control | Do nothing to Year 25: Active. Fails deadwood, managed opening and both planted species |
| TODO-INTEGRATION | P2-economy (grouping, contractor/owner choice, minimum charge); P3-economy (single ledger settlement, timber batches); P5-live (shelter work order writing the contract year); P10 (v14→v15 compatibility) |

Final marker: `SCENARIO_ONE_COMPLETION_ENABLED_PHASES_PASS`, listing the TODO phases. `SCENARIO_ONE_COMPLETION_VERIFY_PASS` is reserved for when no TODO remains.

## Ecology viability (Phase G)

`Tools/Verification/ScenarioOneEcologyViabilityDiagnostic.cs` is a diagnostic, not a gate. It runs at pressure 0.2 through the live manager annual step.

| Schedule | Y5 / Y10 / Y20 / Y40 retained originals | Canopy Y10 / Y40 | Regenerating cells Y10 / Y40 | Deadwood m³ Y10 / Y40 | Outcome at Y25 |
|---|---|---|---|---|---|
| A do nothing | 336 / 336 / 336 / 336 | 0.906 / 0.964 | 28 / 29 | 0 / 0 | **Active**: fails deadwood, opening, planting |
| B plan + shelters | 264 / 264 / 215 / 215 | 0.886 / 0.939 | 40 / 45 | 0.53 / 0.21 | **Completed** |
| C plan, no shelters | same as B | same | same | same | **Completed** |

Planted trees, same positions in B and C. "Promoted" counts trees that have joined the canopy layer.

| Group | Y5 height (m) | Y10 promoted | Browse events (cumulative) |
|---|---|---|---|
| Oak, sheltered (B) | 1.96 | 4/4 | 0 |
| Oak, same positions unsheltered (C) | 1.58 | 1/4 | 7 |
| Beech, sheltered (B) | 1.66 | 2/4 | 0 |
| Beech, unsheltered (C) | 1.51 | 1/4 | 2 |

All planted trees survive, and all have promoted by Year 20.

Findings:
- **Do-nothing fails.** It fails only on management-action objectives (deadwood, managed opening, planting). The ecological objectives are already met without management.
- **The plan completes at Year 25**, with or without shelters.
- **Shelters help planted oak at 0.2**, but the benefit is earlier recruitment (about 5 years), not survival.

**Calibration/product observations.** These are reported, not retuned.

1. "Maintain regenerating cells (≥3)" is met by natural Sitka regeneration under a closed canopy (34 cells at Year 25 doing nothing).
2. "Keep continuous canopy (≥0.35)" is met everywhere: canopy stays around 0.9.
3. A 27% basal-area thinning raises mean stand light only from about 0.05 to 0.10. Planted broadleaves in the brightest cells still recruit quickly.

So completion currently tests *whether the player acted*, not ecological quality. Whether to tighten the objectives (for example broadleaf recruitment, or regeneration excluding Sitka) is a **product decision**.

## OpenCode cross-review (Phase H)

`task/scenario-one-economy-loop` did not exist when this stream was handed off. The preliminary review below covers the foundation chain `66bdd4d → 5af31df → 5eb508d` against the live-integration criteria.

**MUST-FIX** (to verify in the live adapter): none found in the foundation.

**SHOULD-FIX**
- **Grouping before the minimum.** `ForestryWorkCalculator` applies `MinimumHarvestJobCents` per `ForestryTask` (`MinimumJobAdjustmentCents = max(0, minimum − variable)` when no harvest quote). The live adapter must build **one** `ForestryTask` per commissioned harvest job (all of a year's approved felling orders together), never one per `ScenarioOneWorkOrder`. The completion harness P2 plan (72 fellings) is a ready fixture.
- **One financial settlement.** `ScenarioOneManager` currently changes `cashCents` per order during `AdvanceYear`, adding `estimatedCostCents` and timber revenue. The live adapter must replace that path, not add a ledger settlement on top. P3-economy will assert this once the API exists.

**NOTE**
- The foundation adds only new files under `WorkEconomy/`, `TimberYield/`, `Docs/` and `Tools/Verification/`. It has no `Fell()`, no `cashCents`, and no ecology or browsing references. No ecology rule is duplicated.
- Yield must read the measurement snapshot **before** the authoritative `Fell()` in the felling resolution. There must be exactly one `Fell()` path, the existing one.
- All-residual stems return no batch by design. Charging a zero-revenue felling job needs the explicit work-only harvest charge rule (packet risk 4).
- `definitionVersion` must stay `scenario-one-v13`. The v15 restore must follow the restore contract above.

## Integration patches

Exact changes for files Claude must not edit.

**1. Annual review lines** (`ScenarioOneManager.DrawAnnualReview`). After the `foreach (ScenarioSpeciesOutcome species in current.species)` block and before `GUILayout.Space(8f); GUILayout.Label("SCENARIO OBJECTIVES", headingStyle);`, insert:

```csharp
        ScenarioEcologicalSnapshot previousYear = ecologicalSnapshots.Count > 1
            ? ecologicalSnapshots[ecologicalSnapshots.Count - 2] : null;
        foreach (string line in ScenarioEcologyReviewLines.Lines(previousYear, current, ecology, plantedJuveniles))
            GUILayout.Label(line, bodyStyle);
```

**2. Shelter installation** (OpenCode's protection or plant-with-shelter work resolution, inside `AdvanceYear` before `ecology.AdvanceOneYear()`):

```csharp
        ecology.Browsing.Shelters.Add(new BrowseShelter
        {
            shelterId = "SH-" + juvenile.juvenileId,          // stable, unique
            position = new Vector2(juvenile.position.x, juvenile.position.z),
            installedYear = report.year,                      // timing contract
            effectiveYears = 8                                // [G] 6-8 yr working assumption
        });
```

**3. v15 capture/restore** (OpenCode, wherever the v15 records live). Capture:

```csharp
        shelters = ecology.Browsing.Shelters.Select(s => JsonUtility.FromJson<BrowseShelter>(JsonUtility.ToJson(s))).ToList(),
        protectedAreas = ecology.Browsing.ProtectedAreas.Select(a => JsonUtility.FromJson<BrowseProtectedArea>(JsonUtility.ToJson(a))).ToList(),
```

Restore: the first statement of `ScenarioOneManager.RestoreSaveData`, **before** the `data == null` early return, so v1–v9 saves also clear:

```csharp
        if (ecology != null)
        {
            ecology.Browsing.ClearProtection();
            if (data?.shelters != null)
                ecology.Browsing.Shelters.AddRange(data.shelters.Where(s => s != null)
                    .Select(s => JsonUtility.FromJson<BrowseShelter>(JsonUtility.ToJson(s))));
            if (data?.protectedAreas != null)
                ecology.Browsing.ProtectedAreas.AddRange(data.protectedAreas.Where(a => a != null)
                    .Select(a => JsonUtility.FromJson<BrowseProtectedArea>(JsonUtility.ToJson(a))));
        }
```

Also clear protection in `InitializeNewScenario`. Reference preview entry and exit both go through `LoadData` → `RestoreSaveData`, so the rule covers them as long as `previewReturnData` (`CaptureData`) includes the v15 records.

`BrowseShelter` and `BrowseProtectedArea` serialized fields are frozen for v15.

## Verification

Unity 6000.6.0f1 batch mode, one isolated-config process per gate, on `task/scenario-one-ecology-completion`. Results are in the handoff.

## Stretch delivered

- **S1** `BrowseProtectionGeometry`: rectangle around a planting group with a 2 m margin, clipped to `ForestEcologyController.StandBounds`, with perimeter and area. It produces the `BrowseProtectedArea` a fencing job would add, using `installedYear` per the timing contract. Fixture `FENCE_GEOMETRY_VERIFY_PASS`: a group gives 26 m perimeter and 42 m²; a half-cell fence gives cohort access 0.6 under the 5×5 sampling [S]. There is no fencing gameplay.
- **S2** Inspection card (text only): the origin line (original plantation / natural recruit year / planted cohort / planted juvenile) and the shelter status at the stem.
- **S3** Human smoke checklist (below).
- **S4** `Docs/BrowseHistoryFormDamageV16Proposal.md`: proposal only.

## Human smoke checklist (ForestTest, Play mode)

1. **Start.** Year 0, 336 Sitka. Aim at open ground and confirm the bottom report ends "browsing low". With no regeneration, there is no diagnosis line.
2. **Inspect a tree.** The card shows "Origin: original plantation". There is no protection line.
3. **Thin.** Mark crop trees (C) and fellings (M), add them to the Work Plan, approve and advance a year. Marked trees are gone. The aimed-ground report over a felled cell shows higher light within a few years.
4. **Plant.** Plant oak in a bright opened cell and advance. Aim within about 0.7 m of the planted stem and confirm the diagnosis line reads, for example, "Sessile oak planted PJ0001 0.6x m · exposed to browsing: 20%/yr leader risk (low)".
5. **Shelters** (after the live adapter, or with a test fixture). A pale-green 1.2 m tube appears at the stem. The diagnosis reads "protected by shelter (N yr left)". After its working life the tube turns grey and the diagnosis returns to "exposed" while the oak is below 1.8 m.
6. **Dark ground.** Aim at regeneration under closed canopy: "light-limited: deep shade".
7. **Tall juveniles.** A juvenile above 1.8 m reads "above browse reach".
8. **Save/load** (F5/F9 on a scratch save only). The diagnosis "last year browsed" reads "n/a" after loading. Everything else is unchanged.
9. **Annual review** (after integration patch 1). It shows up to six ecology lines: canopy change, opened cells, regeneration, planted trees, browsing band and protection.
10. **Reference preview** (Tab). No shelters during the preview. After returning, the player's shelters reappear unchanged (requires the v15 restore contract).

## Blockers

None. Open product decisions:
- objective strictness (see Viability);
- Scenario One pressure once protection is live.
