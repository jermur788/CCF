# Shared Juvenile Ecology and Tree Mortality Foundation

Task branch: `task/juvenile-mortality-foundation`, worktree `/home/jer/CCF-openai`.
Base: `8ae7a25b4d58f152e33cdf6e72c411e2b71e5a8c`.
Locked context: `a2470e2106796176b7597b3af7f64463b5442581`.

## Shared juvenile calculations

`Assets/ForestPrototype/JuvenileEcologyRules.cs` owns juvenile annual height
growth, the survival response, individual survival realisation, height/light
promotion eligibility and existing juvenile-to-tree DBH proportions.

Both `ForestEcologyController.GrowExistingRegeneration` / cohort promotion
and `ScenarioOneManager.AdvancePlantedJuveniles` call these rules. The species
definition keeps its parameters/interpolation; its public float survival API
delegates to the common response. Origin does not enter biological calculations.

No coefficient, probability, light anchor or promotion threshold was tuned.
The species' split versus legacy light-response configuration is retained.
The survival response carries the unrounded legacy complement in double
precision until cohort density is written. Prematurely returning a float
changed the protected lifecycle, even with the same parameters. Exact
individual survival retains the existing float threshold and existing
versioned deterministic roll. This numerical boundary preserves legacy results.

### Intentional storage adapters

- Cohorts retain expected surviving relative density, existing normalized
  shared-cell capacity, infill and the existing extinction cutoff. Every cohort
  now applies the common survival response; population recovery remains the
  existing cohort-only abstraction.
- Exact planted stock retains individual identity/position, age and
  alive/dead outcome. It does not acquire a newly calibrated density/capacity
  weight. Multiple exact plantings in one cell remain supported.
- Cohorts keep establishment-year age and diagnostic Natural/Planted origin;
  exact individuals keep their nursery-stock age and planting history.
- Natural placement retains its existing random offsets and `R` IDs; legacy
  cohort planting retains `PL` IDs; exact promotion retains `PL-<juvenileId>`
  and the original position/link. No additional RNG draws are consumed.
- The annual execution sequence is unchanged: cohort growth precedes seed
  establishment/promotion; the manager advances exact stock after the ecology
  step with the existing cell light/site inputs. No new seasonal timing is
  invented. Early v13 `legacyCohortManaged` records remain inert individual
  records while their original cohort path continues.

Future browsing/understorey/protection must extend the common calculations,
not add a second origin-specific biological implementation. Those mechanisms
and a new population-capacity interpretation are not introduced by this task.

## Explicit biological tree mortality

`ForestTree.ApplyMortality(string cause, int year)` is the authoritative
explicit operation. It records a separate biological-death flag, a stable
non-empty cause identifier and a non-negative year. String cause IDs permit
later extensions without changing enum serialization. Repeat calls are
idempotent and do not overwrite the first recorded death.

Harvesting remains `Fell()` / `ForestTreeStage.Stump` and the `Felled` event.
Biological death leaves the legacy growth/harvest stage intact, emits the
separate `MortalityApplied` event, clears marks and disables inappropriate
felling/pruning. It does not award cash/timber, execute a work order or emit
`Felled`. Restore is state restoration and emits neither event.

`IsLiving` and the ecology enumeration boundary exclude biological deaths.
The death event invalidates competition and removes stale per-tree metrics;
canopy/light and seed rain rebuild immediately or at the outer change-batch
end. Dead seed potential is zero immediately, including inside a batch.
The marking manager clears its cached mark/marker state through the same
eligibility-cleanup handler, not through a fake management execution.

Cause-specific `RecentOpening` increments are not invented: biological death
removes the living canopy but does not apply the management felling exposure
rule. Existing felling exposure/accounting remains unchanged.

### Deadwood and presentation boundary

The body location/disposition is **unspecified**, not implicitly standing or
fallen. The inactive tree object remains an authoritative record with retained
physical data/position and is included by save capture. Its unsupported living
representation/interaction is hidden. No corpse, windthrow/root plate,
standing-dead asset, brash or Scenario One retained-felling deadwood record is
created automatically.

The mortality event and retained physical data are a hook for a later accepted
deadwood resolver. Future suppression or storms can call the explicit API;
this task contains no annual trigger, threshold, storm probability, salvage
operation or use of diagnostic wind risk as a death probability.

## Save compatibility

`ForestSaveData.CurrentVersion` is 14. New `TreeSaveData` fields:
`biologicallyDead`, `mortalityCause`, `mortalityYear`.

- Existing v1–v13 saves restore no biological mortality and clear any death
  from the previous loaded timeline. Harvested stumps remain harvested stumps.
- V14 captures inactive dead records and restores cause/year/state for reused
  or newly spawned tree IDs. Invalid dead cause/year or a simultaneous
  harvested-stump/death combination is rejected before loading.
- Scenario definition remains `scenario-one-v13`; no ScriptableObject or
  historical-reference data is rewritten. The frozen v12 reference still
  loads/previews, and historical continuation writes the current schema.
- RNG model 0 remains supported/default and model 1 round trips unchanged.

## Verification

Explicit focused gate, copied temporarily from `Tools/Verification/`:

```text
<Unity 6000.6.0f1> -batchmode -nographics -projectPath /home/jer/CCF-openai \
  -executeMethod JuvenileMortalityFoundationVerification.Begin -logFile <log>
```

It never writes the on-disk save. `BeginCanonical` runs the existing protected
80-year lifecycle in isolation using the existing hash implementation.
Remove the temporary script and generated meta after either run.

Passed checks include 30 species/light/site fixtures, natural/legacy planted
cohort equality, exact-position identity and survival, shared promotion
boundaries, cohort capacity, all promotion representations, legacy managed
records, model-1 persistence, explicit mortality living-system exclusion,
idempotence, cause/year missing-tree restore, invalid-save rejection, legacy
revival, batch flushing and no automatic death under high suppression history.

Required regression results:

- `SAVE_HARDENING_VERIFY_PASS` (isolated `XDG_CONFIG_HOME` save slot).
- `RNG_MODEL_VERIFY_PASS`.
- `BATCH_RECOMPUTE_VERIFY_PASS`.
- `SCENARIO_ONE_INTERACTION_VERIFY_PASS`.
- `SCENARIO_HABITAT_PRESENTATION_VERIFY_PASS`.
- Canonical lifecycle: `7E39B70A14959FAD`.
- Frozen Year-100 reference: `7AD177B3CC2F73C7`.
- Historical Year-50 v12 continuation reaches Year-100 v14 with the existing
  documented P0601 same-year prune/fell exception, not a rewritten archive.

## Local verification limitations / preserved work

The new worktree has its own Library and no copied shared Library. It does
not include the ignored locally acquired Ultimate Nature pack; Unity reports
16 missing decorative mushroom-prefab instances. Startup also reports an
Editor QuickSearch database `ArgumentOutOfRangeException`, outside runtime
code. Functional gates pass, but a completely clean rendered/player smoke
run with restored local dependency/search setup remains a reviewer/integrator
gate. No scene/prefab/ScriptableObject change is made to address those issues.

Unity's automatic VS Code solution-path rewrite is restored to the tracked
settings. Agent A's harness/report and all other worktrees are preserved;
no calibration data is imported and no adult mortality is activated.
Independent review is required before integration to main.
