# Phases 2–5 — Oak and Beech adult parity, species × site / canopy, three-species qualification

**Status: audit complete; no biology changed; two phases stopped on evidence/decision (recorded below); one new Unity gate written (not run); Phase 5 is a runbook, not a result.**
Everything here is code-only. No Unity gate was run.

## Summary

| Phase | Packet goal | Outcome | Why |
|---|---|---|---|
| 2 Oak adult parity | Defensible Oak adult height/DBH under active growth model | **Stopped — evidence.** Nothing changed. Qualification gate written. | No Oak height-by-age or yield-class data exists in the repository, and none could be retrieved in this session (see "Evidence attempted"). Replacing the law would mean inventing parameters. |
| 3 Beech adult parity | Same for Beech; check dormant Sitka-like crown constants | **Stopped — evidence.** Dormant crown constants confirmed unread. | Same. |
| 4 Species × site / canopy-light | Small ordinal species-sensitive response; Sitka exactly 1.0 at the canonical site | **Stopped — design decision.** Nothing changed. | The canonical site has no identity in the data or documents (below). |
| 5 Integrated qualification | Seven-fixture Unity qualification | **Prepared, not executable here.** Runbook below. | Needs Unity. |

None of these stops affects Phase 1 (done) or the later phases, which are independent.

## What the current Oak/Beech adult law is

`ForestEcologyController.GrowAdults`, for any non-Sitka species, under every growth model (0, 1 and 2):

```text
dbhGrowth    = potentialDbh × site × clamp01(1 − DBH/maxDbh) × 1/(1 + CI/Ci50)
heightGrowth = potentialHeight × site × clamp01(1 − H/maxH)          (no competition term)
```

Oak and Beech assets are **identical** for every adult growth parameter: potential DBH 0.4 cm/yr, max DBH 120 cm, potential height 0.4 m/yr, max height 40 m, Ci50 3. (Sitka: 1.2 cm/yr, 120 cm, 0.45 m/yr, 35 m, Ci50 5, and under models ≥ 1 a Class III height envelope instead of the height law.) So the two broadleaves are currently interchangeable in adult growth; they differ in crown (Beech power-law, Oak linear), seed, regeneration, browse and — from Phase 1 — density class.

### What that law implies (restated arithmetic, documentation only)

Promoted at height 3.5 m, DBH 5.25 cm (the promotion rule `DBH = clamp(1.5 × height, 2, 20)`), tree age 12; site 1.0 (the only value used in production):

| CI | Age 30 | Age 50 | Age 80 | Age 100 | Age 112 |
|---:|---|---|---|---|---|
| 0 | H 9.5 D 11.9 | H 15.1 D 18.9 | H 21.6 D 28.6 | H 24.9 D 34.5 | H 26.6 D 37.8 |
| 1 | H 9.5 D 10.3 | H 15.1 D 15.7 | H 21.6 D 23.2 | H 24.9 D 27.9 | H 26.6 D 30.7 |
| 2 | H 9.5 D 9.3 | H 15.1 D 13.7 | H 21.6 D 19.9 | H 24.9 D 23.8 | H 26.6 D 26.1 |
| 4 | H 9.5 D 8.2 | H 15.1 D 11.3 | H 21.6 D 15.9 | H 24.9 D 18.8 | H 26.6 D 20.5 |

Height is independent of CI by construction. Height increment falls linearly with height (0.30 m/yr at 10 m, 0.20 at 20 m, 0.10 at 30 m); 3.5 → 20 m takes about 60 years.

Observations (reading, not measurement against trees):

1. **Height** is a monotone, decelerating law with no sigmoid mid-life peak. The repository's own Beech parameter note calls 0.3–0.5 m/yr a good-site young/pole range and 40 m a typical mature height [A/B]; the law honours those two endpoints but takes far longer to approach 40 m than the "typical mature height" wording suggests (about 25 m at age 100). Whether that matches British or Irish top-height curves **cannot be judged without the data** below.
2. **DBH is very low.** Even with zero competition the stem reaches 37.8 cm at age 112; with moderate competition (CI 1–2) 26–31 cm. The 0.4 cm/yr value is documented as a *good-site young Beech target* (B/C), then multiplied by a size factor and by the Hegyi competition factor, so realised growth sits below the documented target. This looks like a possible double penalty, **flagged as a hypothesis, not changed**: the Hegyi factor was calibrated around Sitka (1.2 cm/yr, Ci50 5).
3. **Consequence for Phase 6.** The Manager's Oak ladder needs ≥ 45 cm mid-log diameter for first-grade veneer and ≥ 30 cm for beam. A 45 cm mid-log needs a DBH above 45 cm; no Oak reaches that within a 112-year-old tree under this law even with zero competition, and beam-size stems (DBH roughly 32–35 cm or more, depending on the taper rule Phase 6 uses) appear only in the oldest, least-competed Oak. Combined with the instruction not to compress Oak maturity, **the upper Oak classes are close to unreachable inside Scenario One as calibrated today**. The supplement calls Oak's unrealised potential "a teaching point, not a defect"; that is the Manager's call, but the Manager should know it is a consequence of the DBH calibration and not only of the commercial ladder.

## Evidence attempted

| Source | Result |
|---|---|
| Repository | `Docs/BeechSpeciesParameters.md` (rate ranges and maximum height with B/C and A/B labels; no age series); no Oak document; no yield-class or top-height series for either species. The species evidence pack and consolidations named in `research-index.md` are local to the Manager and uncommitted. |
| Web search | Located the right sources — Forestry Commission Booklet 48 (Edwards & Christie 1981, *Yield models for forest management*), its 2016 successor *Forest Yield*, and COFORD *Growing Broadleaves* / *Broadleaf Forestry in Ireland* — but returned no numbers. |
| Direct retrieval | The page-fetch tool could not resolve `cdn.forestresearch.gov.uk` or `www.coford.ie`, and a plain download through the session proxy returned HTTP 403. No attempt was made to bypass the network policy. |

No numbers were taken from memory. **Nothing in the code or this document cites a top-height value for Oak or Beech.**

### Data that would unblock Phases 2 and 3

1. Top (dominant) height by age and yield class for sessile/pedunculate oak and for beech from a British source (FC Booklet 48 or the 2016 *Forest Yield* handbook) and, if it exists, an Irish equivalent; with the base age and top-height definition (a search-result summary, not verified here, says Irish practice uses base age 30 and the 100 largest stems per hectare).
2. Mean/quadratic mean DBH by age at stated stocking for the same yield classes, to settle the DBH hypothesis above.
3. A decision on which yield class stands for the Scenario One site (this is the missing site identity, below).

### What would then be implemented (not implemented now)

A species-keyed relative-height envelope under growth model 2 with the same shape as the Sitka mechanism (each tree keeps its relative height; no competition term), with per-species parameters carried as data on the species asset and labelled [T]/[ORD]/[C]; the Sitka path untouched. Not built now because a mechanism without evidenced data would be dead code or invented numbers.

## Phase 3 addendum — dormant Beech crown constants

`BeechSpecies.asset` carries `crownRadiusIntercept: 0.9415` and `crownRadiusPerCmDbh: 0.07635` — the Sitka linear crown constants — alongside `usePowerLawCrown: 1`. `PotentialCrownRadiusM` takes the power-law branch for Beech and never reads them; the public accessors `CrownRadiusIntercept` and `CrownRadiusPerCmDbh` have no caller anywhere in `Assets/` or `Tools/`. They are **unused and harmless**. Left in place deliberately: editing the asset adds diff and no behaviour. Follow-up for any later asset clean-up task.

## Qualification gap closed (written, not run)

`Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs` is an in-Editor gate that runs Oak and Beech under growth models **0, 1 and 2** (audit finding F: the existing Oak/Beech gates pin `GrowthModel.Legacy`). It asserts and reports the **current** contract so that any later change is deliberate: height follows the species law and ignores competition; DBH follows the Hegyi response so crowding lowers DBH and not height; neither depends on the growth-model number while no density mortality acts; monotone and bounded; deterministic replay; save/load continuation; Oak/Beech shared parameters reported; and Sitka height still on the Class III envelope under model 2. Type-checked; **never executed**.

## Phase 4 — species × site and canopy/light: stopped on a design decision

### Facts

- `SiteProductivity` and `SoilStability` are single floats per cell, both **1.0 everywhere in production** (`ForestEcologyCell`: "[C] simple defaults; no detailed soil chemistry in this milestone"). They are not in the save schema. Only test fixtures vary them. The one scalar multiplies Oak/Beech DBH **and** height potential, Sitka DBH only (Sitka height uses the Class III envelope), juvenile height growth for all species, and the Model 2 understorey target.
- No project document specifies the Scenario One soil, drainage or exposure. "Irish Sitka site Class III (average)" fixes Sitka top height, not drainage. The Beech note states "drought and waterlogging should both be strong penalties", but nothing implements it, and nothing says the canonical site is wet or dry.
- Light: opacity is an implicit 1.0 for every crown (k10a10 calibration); the light equation is anchored in every official hash. The repository holds no species-specific attenuation evidence.

### Consequence

At a uniform site of 1.0, any response written as a function of the existing scalar is inert (1.0^k = 1). A response that is not inert needs either (a) a statement of what the canonical site *is* for each species, or (b) a new site axis in the data. Both are design or save-architecture decisions (stop conditions 4, 8 and 11), and any magnitude would be invented.

### Options for the Manager

| Option | What it is | Effect at the canonical site | Save / anchor impact |
|---|---|---|---|
| S0 | Do nothing for Scenario One; record that species × site fidelity is deferred until a second site exists | None | None |
| S1 | Species site-demand exponent on the existing scalar | None while the scalar is 1.0 | None; zero visible effect, so low value now |
| S2 | A per-species **canonical-site suitability** class (ordinal Good / Moderate / Poor; Sitka exactly 1.0), applied under growth model 2 | Oak/Beech grow below Sitka's reference at the canonical site; magnitude is [C] | No schema change; moves Oak/Beech growth, so mixed-stand anchors under model 2 |
| S3 | Real site axes (moisture/drainage) in cell state and the save | Per-cell species response | Schema change; scene authoring; largest |

Recommendation: S2 only after the Manager states what the site is (for example "imperfectly drained mineral gley"), because without that sentence the factor has no referent; S0 otherwise. Canopy/light: leave unchanged until evidence exists; adding an opacity field defaulted to 1.0 would be inert.

## Phase 5 — three-species qualification runbook (for a Unity-capable session)

Run in this order. Each run in its own Unity process with an isolated `XDG_CONFIG_HOME`, as the regression runners do, `CCF_STAND_GEOMETRY=0` for the historical anchors unless noted.

1. **Compile/import** with Unity 6000.6.0f1: zero C# errors; only the known Editor Search importer signature allowed.
2. **Unchanged-defaults regression:** the 24-gate list in `run_regression.py`. Because `NewGameGrowthModel` is still 1, **every anchor in `Phase0-Baseline.md` must be exactly reproduced**. Any difference is a defect in this programme's changes, not a rebaseline.
3. **New gates:** `MixedDensityVerification.Begin`, `BroadleafAdultGrowthVerification.Begin` (copy into `Assets/ForestPrototype`, run, remove copy and `.meta`).
4. **Measure model 2 on the scenario fixtures** (a measurement, not a pass/fail gate — this is where the packet's mixed-stand rebaseline question is answered):

   ```text
   CCF_GROWTH_MODEL=2 <UNITY> ... -executeMethod ScenarioOneCompletionVerification.Begin   # Legacy40
   CCF_GROWTH_MODEL=2 CCF_STAND_GEOMETRY=1 ... same                                       # Enlarged80
   ```

   Record for each: old anchor (`702766DECE591E21` Legacy40 Model2; the Enlarged80 completion witness), new `SCENARIO_ONE_COMPLETION_HASH`, completion year, minimum cash, and the **first divergence** (the year and the first changed tree set, from `LastAdultMortality.SpeciesAwareRoute` first becoming true and the first species-aware death). State whether the previous result encoded the defect (it did if the divergence appears in the first mixed-density year) and stop for the Manager's rebaseline decision before changing any default.
5. **Fixtures requested by the packet**, to be built as scenario variants of the harness stand (all are synthetic adult stands; the gate `MixedDensityVerification` already covers pure Sitka / Oak / Beech, Sitka+Oak, Sitka+Beech, Oak+Beech and all three, thresholds, replay and save/load): extend only with natural regeneration and juvenile promotion by running the existing `RegenerationModelVerification` and `CCFBeechVerification`/`CCFOakVerification` under `CCF_GROWTH_MODEL=2` once they accept an override (today they pin Legacy and would need that one-line change in a copy).
6. **Companion checks for the unchanged systems:** Reference Future (`7AD177B3CC2F73C7`, `9CDF21A541C5968D`), save hardening and cell-index hardening, understorey/Model2, browse, planting, pruning, Enlarged80 (`04A78188A6F8E6F9`, `081DB58CE3034648`), deadwood, completion.
7. **Performance (informational):** one annual step on Enlarged80 under models 1 and 2 with a mixed stand. The species-aware route adds a tally pass and a list build per year (about 1,300 stems), expected small against the ~1 s annual step; not measured.

## Canonical updates required (not made)

- `unity-project-overview.md`: Oak and Beech adult growth parameters are identical; the law and its implied trajectory; `SiteProductivity` uniform 1.0 and unsaved; the missing site identity.
- `decision-log.md`: record "Phases 2–4 stopped on evidence/decision" with the data list and options S0–S3, and the DBH-calibration observation, as open items.
