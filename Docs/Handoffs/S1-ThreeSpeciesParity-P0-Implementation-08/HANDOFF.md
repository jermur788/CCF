# PARITY-08 — Save / Deterministic Compatibility — Implementation Handoff

Claim: #126 · Parent: #91 · Authority: `RELEASE-AMENDMENT-20261030-v24.md` (on `origin/main`, d5d60498), `TASK-PACKET.md`, spec PARITY-08
Executor: Claude Opus 5.5 (Claude Code), offline/static phase only. **Unity was not run in this phase.**
Branch / worktree: `verify/parity-08-save-compatibility-v1` · `/media/jer/Videos/CCF-Parity08`
Start SHA: `185200f5` (integration, save schema v24) · End SHA: the commit that adds this file (see `git log -1` on the branch / #126 comment)

Verdict (offline/static): **SPECIES PARITY COMPATIBILITY PASS**. The candidate is ready for the serialized Unity gate that #91 schedules. It is not Unity-verified.

## Demonstrated defect (before this change, at 185200f5)

Validation never checked species IDs. `ForestSaveController.LoadData` dealt with unknown species only **after** it had started changing the world (ecology geometry, carried wood, destroyed recruits):

- tree record (v≥7) with an unknown non-empty `speciesId`: logged a warning and **loaded it as the default Sitka** (`ForestSaveController.cs` ~L391–396);
- regeneration cohort (v≥8) with an unknown species: **silently skipped** mid-restore (`ForestEcologyController.RestoreCellState`);
- planted juvenile / pending regeneration order with an unknown species: restored, then quietly skipped or failed on the next annual step.

`CanLoad` reported these saves as loadable.

## Repair (no schema/field change, no validator loosened)

- `Assets/ForestPrototype/ForestSaveValidation.cs`: new scene-free `ValidateSpecies(data, isLoadableSpecies)`. It rejects a non-empty unknown species ID on:
  - trees (v≥7);
  - regeneration cohorts with density > 0 (v≥8, matching the loader, which skips density ≤ 0);
  - planted juveniles (v≥10);
  - Pending/Approved work orders (v≥10).
- `Assets/ForestPrototype/ForestSaveController.cs`: `FindLoadProblem`, which `CanLoad` and `LoadData` share and which runs before the first world change, calls `ValidateSpecies` with `spawner.ResolveSpecies(id) != null`. Registered but not simulation-ready species therefore count as unknown too.
- The old Sitka fallback in `LoadData` is left in place. It can no longer be reached: trees require a spawner, and the spawner-backed check runs first. It was kept to stay inside the minimal-change rule; a reviewer may ask for it to be removed.

Kept unchanged on purpose:

- **Empty IDs** keep their historical meaning (the scene's default species). There are 45 empty-ID fixture records.
- **Versions where the loader ignores the field** are not checked (v<7 trees, v<8 cohorts, v<10 Scenario One).
- **History-only species fields are not treated as authoritative:** Completed/Failed orders, management events, and deadwood records. Deadwood records are already tied to their victim tree's species by `ValidateStorms`.
- **Timber-sale records** carry no species and remain accepted.

## Changed files (vs 185200f5)

- `Assets/ForestPrototype/ForestSaveValidation.cs` (+41)
- `Assets/ForestPrototype/ForestSaveController.cs` (+3)
- `Tools/Verification/SaveHardeningVerification.cs` (+38): Unity-gate cases. For each path, CanLoad and LoadData must both refuse an unknown species, with `AssertUnchanged` checking year and tree count. A resolved-order history case stays accepted.
- `Tools/Verification/Parity08Save/run_offline_check.py`, `Parity08SpeciesOfflineCheck.cs` (new, offline .NET)
- this handoff

Not touched: `ForestSaveData.cs` (`CurrentVersion` stays 24), `ScenarioReferenceArchive.cs`, `ScenarioOne/Resources` (Reference Future v1), hash comparators/pins, scenes, prefabs, packages, ProjectSettings, UI, MFM-08, S1-D, MFM-09. `git diff 185200f5 -- <those paths>` is empty.

## Species-bearing record inventory

| Record | Field | Authoritative? | Check |
|---|---|---|---|
| `TreeSaveData` | `speciesId` (v7+) | yes, builds the tree | rejected if unknown |
| `ForestRegenerationCohortSaveData` | `speciesId` (v8+) | yes, builds the cohort/band | rejected if unknown and density > 0 |
| `ForestCellSaveData.regenDensity…` (v1–7) | implicit Sitka | yes | unchanged (no ID) |
| `PlantedJuvenileSaveData` | `speciesId` | yes, simulated each year | rejected if unknown |
| `ScenarioOneWorkOrder` Pending/Approved | `speciesId` | yes, decides the job | rejected if unknown |
| `ScenarioOneWorkOrder` Completed/Failed | `speciesId` | history | not checked |
| `ScenarioManagementEvent` | `speciesId` | history | not checked |
| `ScenarioDeadwoodRecord` | `speciesId` | must equal the victim tree's (existing storm validator) | covered through the tree |
| `ScenarioTimberSale` | none | — | accepted |

Shipped fixtures/assets: every tracked `.json`/`.txt`/`.csv`/`.bytes` speciesId value is in {"" ×45, `beech` ×29, `sessile-oak` ×29, `sitka-spruce` ×1896}. No accepted fixture uses an unknown ID, so the hard stop is not triggered. Command: `git ls-files | grep -iE '\.(json|txt|csv|bytes)$' | xargs grep -ohE '"speciesId" *: *"[^"]*"' | sort | uniq -c`.

## Evidence (offline). Output stored under `/media/jer/Files/CCF/Verification/PARITY-08/offline-20261030/`

| Command | Candidate | Base 185200f5 |
|---|---|---|
| `python3 -I -B Tools/Verification/Parity08Save/run_offline_check.py --out …/parity08` | **PARITY08_SPECIES_OFFLINE_PASS checks=23** | n/a (new) |
| Mutation: guard on trees / on cohorts disabled | both mutants killed (`FAIL refused: tree species`, `FAIL refused: regeneration cohort species`) | — |
| `python3 -I -B Tools/Verification/MfmSaveProvenance/run_offline_check.py` | **MFM06_SAVE_OFFLINE_PASS checks=172**, `REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_ac876a6` | — |
| `…/MfmSaveProvenance/run_mutation_controls.py` | exit 2 (`M14 mutant text not present`) | **identical** exit 2 / same lines |
| `ThreeSpecies/OfflineLogic/run_offline_check.py --suite timber` | THREE_SPECIES_TIMBER_PASS checks=73 | — |
| `--suite density-isolated` / `density-sitka-isolated` | MIXED_DENSITY_ISOLATED_PASS / SITKA_MODEL_ISOLATED_PASS (relativeDensity=0.811598) | — |
| `--suite density` | 1 FAIL (`MaxRemovedBasalShare` identifier scan on rack/prescription/teaching files) | identical (only the order of the file list differs) |
| `--suite density-composite` | beech long-run R2 FAILs | identical |
| `--suite history` | build fails in `ManagementHistoryText.cs` (`WorkItemDetail`) | identical |
| `CloudTypeCheck/run_typecheck.py --mode prod` | 4 NEW (`Painter2D`, UI only) | same 4 |
| `CloudTypeCheck/run_typecheck.py --mode tools` | 42 NEW | **byte-identical sorted list** |

Log sha256: parity08 run (see `parity08.log`); mfmsave `158813ad…44244a`; ts-timber `b8524f7d…a600f`; ts-density `29b50f14…0c834550`; mfmmut `170af91d…2244244a`.

The offline check also confirms:

- v24 with `firstGuidedRackReviewComplete = true` passes `ValidateSpecies`, and the latch does not move `ScenarioReferenceArchive.WorldHash`;
- `CurrentVersion == 24`;
- every authoritative path refuses an unknown species at its first applicable version (v7/v8/v10);
- history and pre-field records are still accepted.

Mutation ordering, statically: `FindLoadProblem` runs before `ecology.ApplyStandGeometry`, the first world mutation in `LoadData`, and is the same function `CanLoad` calls. CanLoad and LoadData therefore reject the same saves by construction.

## Residuals / not verified here

- **Unity not run.** Still needed on the exact candidate, under the machine-wide lock:
  - import/compile;
  - `SaveHardeningVerification` with the new case 5: CanLoad/LoadData parity and no-mutation;
  - model-0/1/2 positive load;
  - pre-v23/historical fixtures;
  - Legacy40;
  - frozen Reference Future integrity;
  - protected pure-Sitka lifecycle/density and Sitka timber anchors;
  - v24 latch-true round-trip.

  These are expected to be unchanged, because no production path that runs for known-species saves was modified, but that is not proven yet.
- Pre-existing offline failures listed above are identical on the base. They are outside PARITY-08 scope and were not fixed.
- Scenes without a `ForestTreeSpawner` skip the species check, which preserves prior behaviour (such saves must have no trees). Cohort/juvenile restore in such scenes was already a no-op or used the default.
- Policy choice for the reviewer: Pending/Approved orders are treated as authoritative; Completed/Failed orders are treated as history.

## Next action

#91 schedules the serialized Unity gate on this exact end SHA. Then the independent cross-provider review (#128 / reviewer per amendment) before PARITY-09. Not merged or integrated.
