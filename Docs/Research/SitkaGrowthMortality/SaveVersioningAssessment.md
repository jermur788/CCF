# Save / versioning assessment (Phase 12)

## Does mortality need persistent stress history?

**No.** The recommended suppression mortality is a stateless annual hazard. Each year it reads only state the simulation already rebuilds or saves:
- the competition index (recomputed annually);
- tree DBH, height and age (saved);
- a deterministic per-tree roll (`SimulationRandom.Roll`, model 1, keyed by tree ID, year and seed).

"Persistent stress" emerges from repeated annual risk while suppression persists. Deaths use the existing saved fields (`biologicallyDead`, `mortalityCause`, `mortalityYear`; v14+). Deadwood uses the existing `ScenarioDeadwoodRecord` list (saved in `scenarioOne.deadwoodRecords`).

A *consecutive-years-of-stress* counter, or a decaying recent-stress value, would need a new per-tree field. That is not recommended and not needed. The existing `equivalentSuppressedYears` is cumulative, never decays, and is documented as diagnostic-only; it should not be repurposed.

## Does the height model need new persisted state?

**No per-tree state.** Candidate A-III advances each tree as `H ← H · E(age+1)/E(age)`, using the saved height and age. The site class is a scenario-definition value (asset), not per-save state, while a single class applies to a scenario.

## What does need a schema change: the model-version flag

Both the height model and adult mortality change the growth of every adult tree. To keep **legacy saves and Reference Future v1 replayable** (task versioning rule), loads must know which growth/mortality model a save uses. That is the same pattern as `rngModelVersion` (D-046) and `regenerationModel` (D-047).

**Minimum proposed v17 change** (not implemented; production stopped):

| Field | Type | Missing / v ≤ 16 | New Scenario One games |
|---|---|---|---|
| `ForestSaveData.growthModel` | int | 0 = current height law, no adult mortality (legacy) | 1 = selected site-class height envelope + adult suppression mortality |

Migration and compatibility:
- **Old saves:** saves v1–16 load as `growthModel` 0, with no automatic migration. Legacy and model-1 regeneration anchors stay reproducible (JSON hashes compared in the v15/v16 layout via a helper, as for D-047).
- **Reference Future v1:** stays model 0.
- **Forward compatibility:** an older build loading a v17 save would warn ("newer than supported") and replay legacy growth.
- **No speculative storm fields.**

## Conclusion

Production implementation **must stop** for a v17 field decision, in addition to the site-class decision. No per-tree stress state, no new deadwood state and no storm state are required.
