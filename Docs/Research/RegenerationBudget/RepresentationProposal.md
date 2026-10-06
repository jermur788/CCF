# Regeneration representation — result **B: SAVE CHANGE REQUIRED**

Proposal for Manager decision. Nothing here is implemented; no schema, ecology or calibration change has been made. Evidence: RuntimeReproduction.md. Manager direction applied: hybrid representation (juvenile = relative abundance/occupancy, promoted = exact tree), no stems/ha meaning, separate age bands for biologically different recruitment, no averaging of new recruits into old cohorts.

## 1. Save-format capability audit

| Question | Finding | Evidence |
|---|---|---|
| Can the live model hold more than one cohort per cell and species? | **No.** `ForestEcologyCell.FindCohort` returns the first record for a species; `GetOrCreateCohort` reuses it; establishment, infill, promotion, clearance and display all address the cohort by species. | Source: ForestEcologyCell.cs, ForestEcologyController.cs |
| Can the save file hold it? | The JSON list `cells[].regenerationCohorts[]` could physically contain two records with the same `speciesId`; no field prevents it and `ForestSaveValidation` does not check it. | ForestSaveData.cs (`ForestRegenerationCohortSaveData`: speciesId, density, height, establishYear, origin, originYear) |
| Does the load path preserve it? | **No.** `RestoreCellState` resolves each record through `GetOrCreateCohort(species)`, so the second record overwrites the first. | FX7, runtime: two Sitka records (0.3 / 0.2 m / year 9 and 0.4 / 1.5 m / year 2) restore as **one** cohort (0.4 / 1.5 m / year 2) |
| Can age bands be reconstructed from existing state? | No. One height and one establishment year per species are all that is saved; the age distribution of merged recruits is already lost. | Ledger: 63–81 % of establishment events merge into older cohorts |
| Do transient (unsaved) bands work? | No: they would diverge after save/load, which breaks deterministic continuation. | — |

Conclusion: separate age bands need (a) new semantics for an existing save list, (b) a load-path change and (c) a model flag so legacy saves and frozen Reference v1 keep today's behaviour. That is a save-format change, so per the packet **work stops here pending approval**.

## 2. Minimum schema change (proposed save v16)

**Added persisted state (exactly two items):**

1. `ForestSaveData.regenerationModel` (int). Absent → JsonUtility default 0 = legacy single-cohort behaviour (today's code, unchanged). New games write 1. This follows the D-046 RNG-model pattern.
2. **Semantic change, no new field:** under `regenerationModel = 1`, `regenerationCohorts[]` may hold several records per species. Each record is one **age band**, keyed by `(speciesId, establishYear, origin)`, using the existing `density`, `height`, `establishYear`, `origin` and `originYear` fields.

No other field is needed. Requested and rejected establishment, light/browse/threshold losses and exports are per-year diagnostics (ledger), not state, and stay unsaved.

**Bound:** at most K bands per species per cell (proposal K = 4; 3 species × 64 cells × 4 = 768 records worst case). Validation rejects duplicate band keys and more than K bands.

**Migration:**
- v ≤ 15 saves load exactly as today: one record per species becomes one band, `regenerationModel` = 0, and all legacy rules apply. No values are rewritten.
- Frozen Reference v1 is never resaved; its embedded historical saves load as model 0, so archive integrity, preview and continuation (`9CDF21A541C5968D`) must stay unchanged. Any change is a stop condition.
- There is no automatic upgrade of an existing game from model 0 to 1; a mid-game switch would itself change the timeline.

**Compatibility implications:**
- *Forward:* an older build loading a v16 model-1 save already warns ("newer than supported, best-effort") but would silently collapse bands (last record wins, as FX7 shows). Accept as documented best-effort, or have older builds refuse newer versions (that needs a change to the old build, so it is not available retroactively).
- *Anchors:* model-0 anchors (neutral, normal, completion, Reference) are unchanged by construction and must be re-verified. Model-1 new-game anchors (`2A0B8C32AC0DE113`, `506E8AF6D8514C6C`, `00479F18970F9926`) **will change** and need re-baselining plus tutorial/completion viability re-checks. No threshold or economy recalibration to hide a failure.
- *UI:* a cell can show several juvenile bands; the existing "relative density" wording stays (no stems/ha).

## 3. Proposed band rules under model 1 (for the implementation packet)

- **Establishment:** accepted recruitment in year *y* creates or extends the band with `establishYear = y`, at initial height. It never merges into an older band.
- **Capacity:** shared occupancy across all bands of all species, as today. A request against a full cell is rejected and logged. A request never contracts existing stock: the target is clamped at ≥ its current abundance, fixing FX5B.
- **Age initialisation:** happens only when accepted abundance > 0, fixing FX4.
- **Band cap reached:** **decision needed.**
  - (a) Reject the new recruitment and log it (recommended: keeps ages honest, may slow recruitment in long-occupied cells).
  - (b) Merge the two adjacent *oldest* bands, abundance-weighted. This never ages a new recruit, but blurs old-band history.
- **Growth, survival and browse:** per band, using today's shared `JuvenileEcologyRules` unchanged. Vegetation competition and browsing stay separate mechanisms (synthesis §5; no single mortality coefficient).
- **Infill:** **decision needed.** Under model 1, either remove the seed-independent +0.05 (recruitment then comes only from seed via establishment), or keep it as an explicitly labelled calibration term. The evidence shows it is 10–14 % of all abundance additions and the entire cause of the natural vs planted divergence. Removing it changes model-1 trajectories, so it needs Manager approval like any ecology change.
- **Threshold:** keep the 0.01 representation threshold per band, logged as numerical loss. Optionally lower it under model 1 (decision; it drives the earlier natural die-off at light 0.03).

## 4. Promotion — bounded design proposal (stop condition reached)

Current representation: one species cohort → one exact tree, with the entire abundance consumed and residual 0 (FX6; in-stand median 1.5 units per tree). The current representation **cannot** keep a coherent residual without inventing an abundance-per-tree conversion, so per the packet this is a design proposal, not an implementation.

With age bands, the handoff can be stated without a physical conversion:

| Step | Recorded quantity |
|---|---|
| Juvenile state before promotion | all bands in the cell (abundance, height, establishYear, origin) |
| Exported by promotion | the **promoting band**: the oldest band meeting `CanPromote` (height and light); its whole abundance, logged as a representation handoff |
| Exact tree created | one tree; age from the band's `establishYear`, height from the band, id/origin from the band |
| Residual juvenile state | all other (younger) bands, unchanged |

Properties: younger recruits survive promotion of an older band, which removes most of today's compression in mixed-age cells. The exported band is removed entirely, so no stale origin can label later recruits (fixes FX6B). The UI and docs describe this as a representation handoff, never as N stems becoming one tree.

**Options needing a Manager decision (not selected here):**
- **P1 — whole-band export** (above). No calibration constant. Recommended as the bounded first step.
- **P2 — fixed abundance quantum per exact tree, remainder retained in the band.** Gives within-band residual, but the quantum *is* an abundance-to-tree conversion that needs calibration and a scale bridge for Scenario 2. Out of scope without authority.
- **P3 — promote at most one tree per band per year, with remainder retained.** Repeated exports without a defined quantum; same conversion problem as P2 in disguise. Not recommended.

## 5. Requested decisions

1. Approve save v16 with `regenerationModel` and band semantics as above (or reject, leaving production unchanged).
2. Band-cap policy: (a) reject, or (b) merge oldest.
3. Promotion: P1 whole-band export (recommended), or commission P2.
4. Infill under model 1: remove, or keep as a labelled calibration term.
5. Threshold under model 1: keep 0.01 or lower it.

After approval, the implementation proceeds with the gate list in BeforeAfterRegenerationFunnel.md and produces the AFTER funnel under identical inputs.

## Research use

CCF Primary Literature Synthesis v1 (`~/Documents/CCF Game/CCF_Primary_Literature_Synthesis_v1.pdf`, 12 pp., 5 Oct 2026, SHA-256 prefix `5b7a5261860e2bec`) is **supporting research only**. It was used for:
- keeping vegetation competition and browsing as separate causal mechanisms (§5, citing Harmer & Morgan 2007 and Perrin et al.);
- treating Sitka leader browsing as height-shaped growth delay and form damage rather than a mortality roll (§4, Welch et al. 1991/1992). This matches the observed browse-loss ≪ light-loss pattern.

Its bramble/bracken survival values are English oak transfer and calibration anchors, not Irish coefficients. None is adopted here. The canonical research index is unchanged; adding this synthesis there is a proposal for the Manager.
