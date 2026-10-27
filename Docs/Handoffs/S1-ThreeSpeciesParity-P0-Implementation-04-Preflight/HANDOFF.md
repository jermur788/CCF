# PARITY-04-PREFLIGHT — Adult / Species-Data Read-Only Requalification Audit — Handoff

**Task:** `PARITY-04-PREFLIGHT`
**Spec:** `specs/scenario-one-three-species-parity-p0/` (APPROVED; read from `origin/main`, byte-unchanged between the two main heads named below)
**Claim:** GitHub issue #11 (`jermur788/CCF`)
**Date:** 2026-10-27
**Worker / routing:** Claude Sonnet 5.5 (`claude-sonnet-5-5`, Anthropic) via Claude Code in T3 Code. This is **not** the packet's recommended strongest-reasoning/Opus model and it is a **local** read-only session, **not Claude Cloud**; both are recorded as the actual executor.
**Workspace:** none created. The Primary checkout is on an unrelated dirty branch and was not touched. All repository inspection used `git show` / `git grep` / `git diff` against the object store; scratch files lived under `/tmp` outside the repository.
**Unity:** **NOT RUN. Lock not acquired.** **Production writes:** none.

## Verdict

**`PARITY-04 PREFLIGHT COMPLETE — READY FOR MANAGER REVIEW`**

Recommendation for the eventual PARITY-04 production task: **`BOUNDED LABEL/IMPLEMENTATION PATCH REQUIRED`** — a label / documentation / verification-tooling patch only. **No accepted value, no arithmetic, no species-YAML, no scene and no C# production file needs to change** (§7).

This is a preflight. It does **not** claim `ADULT SPECIES PARITY REQUALIFIED`; no Unity or runtime measurement was made.

### Headline findings

1. **No measured or structural defect** was found in the Oak/Beech adult constants, the shared height law, the neutral site seam, shared canopy/light, the shared 8 m Hegyi neighbourhood, the `Ci50` roles, or the protected pure-Sitka path. The Beech height-band miss is a direct arithmetic consequence of the accepted constants (§4, Q11), not an implementation defect.
2. **Documentation is stale against the accepted values, including a canonical Manager-owned doc.** `Docs/Project/unity-project-overview.md:135` still says "Beech/Oak keep Ci50 3 and 0.4 cm/yr", `Docs/BeechSpeciesParameters.md` and `Assets/ForestPrototype/Docs/SessileOakV1.md` carry the superseded 0.4 values, and the Beech doc tags a 40 m maximum as evidence class A/B. These became wrong **on `main`** when Rack V2 core integration merged the three-species lineage (`f23549b`).
3. **A verification contract has drifted and its gate now fails.** `Tools/Calibration/OakBeechGrowth/check_integrated_v1.py` fails on its first assertion at the candidate (and on current `main`) because three of its seven pinned normalized source hashes no longer match. At `8f9ed80` all seven matched. The drift traces to later non-Oak/Beech changes (§4, Q12 and §5 F-5). This is not recorded anywhere I could find, and the Tier-3 static preflight list expects this gate to pass.
4. **The in-Editor adult gate has no recorded Unity result on this lineage** and only *reports*, rather than asserts, the accepted constants. Its header text is stale.
5. **The audit applies to current `main`.** Every audited file is byte-identical between the candidate, the Rack integration tip and `origin/main` at `fe81994` (§1).

---

## 1. Source identity and what was read

`origin/main` advanced during this task (background fetch). The packet, the parity spec/plan/tasks/checklist were read at `ce8a1742cdd768be327dfd0d26bf85485a26d3fa` and re-verified **byte-unchanged** at the current head. Live coordination (`active-tasks.md`) was re-read at the current head and its changes are reflected in §6.

| Item | SHA | Note |
|---|---|---|
| `origin/main` at claim / packet read | `ce8a1742cdd768be327dfd0d26bf85485a26d3fa` | claim #11 marked `[RUNNING]` here |
| `origin/main` current at publication | `fe8199441b5ab69c7644797b3e3792cc3c3674eb` | contains Rack V2 core (`f23549b`, PR #12) |
| Candidate audited | `e1515e165eb40b40b5200992398984d69e0e8e8e` | **packet-only** commit above the base; production tree identical to the base |
| Base | `c6342f79a3e89c040fadd232017a937132779385` | ancestor of current `main` |
| Candidate-lineage head PARITY-02 evaluated | `8f9ed80` | ancestor of the candidate and of `main` |
| PARITY-03 static handoff | `b22c75f439e34d5cf5bcd91a1b4303c40ceec2f8` | read |
| Rack V2 integration tip at read | `1147f527bd4adb931152f4d198b3bbe13876a836` | ancestor of `main` |
| Protected Sitka base | `c1564b8` | diffed against |

The candidate commit `e1515e1` itself is not an ancestor of `main`, but its production tree equals `c6342f7`'s (`git diff --stat c6342f7 e1515e1` is the packet file only). **Identity result:** all 22 audited files below are **byte-identical** between `e1515e1`, `origin/integration/scenario-one-rack-v2-v1` (`1147f52`) and `origin/main` (`fe81994`):

`SessileOak.asset`, `BeechSpecies.asset`, `SitkaSpruce.asset`, `TreeSpeciesDefinition.cs`, `ForestEcologyController.cs`, `ForestTree.cs`, `ForestEcologyCell.cs`, `SpeciesSiteResponse.cs`, `ForestSiteProfile.cs`, `ScenarioSiteProfiles.cs`, `SitkaGrowthModel.cs`, `MixedDensityModel.cs`, `MixedDensityModel.Composite.cs`, `SitkaSubsetDensitySignal.cs`, `ForestTreeMarkingManager.cs`, `ForestSaveData.cs`, `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`, `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json`, `Docs/BeechSpeciesParameters.md`, `Assets/ForestPrototype/Docs/SessileOakV1.md`, `Assets/Scenes/ForestTest.unity` (plus `MixedSpeciesTest.unity`, `check_integrated_v1.py`, `source_normalization.py`, `run_site_check.py`).

Relative to `8f9ed80` (what PARITY-02 evaluated), the three species assets, `ForestTree.cs`, `ForestEcologyCell.cs`, the site files, the density files and `SitkaGrowthModel.cs` are byte-identical. The audited files that changed are `TreeSpeciesDefinition.cs`, `ForestEcologyController.cs`, `SpeciesSiteResponse.cs`, `ForestTreeMarkingManager.cs` and `ForestSaveData.cs` (save v21), none in an Oak/Beech adult-arithmetic way (§4 F-5, Q4). PARITY-02 explicitly did not review the Rack V2 descendant line by line; this audit does that for the adult surface.

### Read (authority and context)

Approved spec, plan, tasks, checklist; PARITY-02 handoff and Manager review; PARITY-03 static handoff (`b22c75f`) and Manager review; PARITY-03-R and PARITY-03-UNITY packets; Decision Log D-059 / D-061; AgentWorkflow handoff/lock rules; live `active-tasks.md`; `Docs/Verification/Calibration/OakBeechProductionCalibrationV1Handoff.md`; `CompositeDensityV3Handoff.md`.

### Files / symbols inspected (all at `e1515e1`)

- **Assets:** `SessileOak.asset`, `BeechSpecies.asset`, `SitkaSpruce.asset` (read-only comparison); scene bindings in `Assets/Scenes/ForestTest.unity` and `MixedSpeciesTest.unity` by GUID, and a count of embedded `TreeSpeciesDefinition` MonoBehaviours (0).
- **`TreeSpeciesDefinition.cs`:** provenance legend (3–5), tooltips, `GrowingSpaceDemand`, `PotentialCrownRadiusM`, `HasGrowthAgePhases`.
- **`ForestEcologyController.cs`:** `HegyiCutoffMeters` (912), `UpdateCompetition` (930), `GrowAdults` (1003), `ApplyAdultDensityMortality` (1062), `KillDensityVictims` (1138), `HasNonReferenceDemand` (1160), `ApplySpeciesAwareDensityMortality` (1177), `RelaxCrowns` (1229), `RecomputeCanopy` (822), `GetSiteProductivity` (1957), `UsesSiteClassGrowth` (148), `UsesSpeciesAwareGrowth` (151).
- **`ForestTree.cs`:** `ApplyGrowth`, `RelaxCrownRadius`, `SetAgeYears`, `IsLiving`; **`ForestEcologyCell.cs:48`** `SiteProductivity`.
- **`SpeciesSiteResponse.cs`, `ForestSiteProfile.cs`, `ScenarioSiteProfiles.cs`; `SitkaGrowthModel.cs`; `MixedDensityModel.cs`, `.Composite.cs`; `SitkaSubsetDensitySignal.cs`; `ForestTreeMarkingManager.cs`** (growth forecast and competition cutoff); `SpeciesConfigurationPolicy.GrowthAgePhases`; `JuvenileEcologyRules.PromotionDbhCm`.
- **Verification:** `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`, `run_unity_gates.py`, `SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs` and `run_site_check.py`, `Tools/Calibration/OakBeechGrowth/{check_integrated_v1.py,check_production_v1.py,kernel_controls.py,production_v1_inputs.json,integrated_source_contract.json}`, `Tools/Verification/source_normalization.py`.
- **Docs surfaces:** `Docs/BeechSpeciesParameters.md`, `Assets/ForestPrototype/Docs/SessileOakV1.md`, `Docs/Project/unity-project-overview.md`, `Docs/EcologyCalibrationAdoption.md`.

## 2. What was and was not executed

**Executed (no repository writes):**

- `git show`, `git grep`, `git diff`, `git log`, `git merge-base`, `git archive` on the object store.
- The repo's own `Tools/Verification/source_normalization.py` and the **unmodified** `Tools/Calibration/OakBeechGrowth/check_integrated_v1.py`, run against a `git archive` extract of the candidate and of `8f9ed80` in a scratch directory under `/tmp` (outside the repository), to establish contract drift and the gate's actual failure mode.
- A stdlib-Python recomputation of the Oak/Beech height recurrence, asserting the iterated and closed forms agree to 1e-9.

**Not executed:** Unity (any form); the .NET offline suites (`run_site_check.py`, `ThreeSpecies/OfflineLogic/*`); the CloudTypeCheck approximation; any Python offline density suite; any save load. The PARITY-03 worker's *measured* density numbers (continuity table, hazard-neutrality counts) were **not re-measured**; only the PARITY-03 *source-level* claims were independently re-derived here where they bear on this audit.

---

## 3. 14-question verdict matrix

Verdicts are what the **static** audit can conclude. None of these is Unity-qualified; the runtime proofs still owed are in §5.

| # | Question | Verdict | Evidence (candidate `e1515e1` unless stated) |
|---|---|---|---|
| 1 | Oak/Beech adult constants exactly the accepted values? | **CONFORMS (static)** | `SessileOak.asset:19,21,23,25,27` = Pd 0.8, Dmax 120, Ph 0.65, Hmax 42.5, Ci50 5. `BeechSpecies.asset:19,21,23,25,27` = 0.9, 120, 0.75, 42.5, 6.5. Scenes bind the assets by GUID only (`ForestTest.unity:5808,5839,5864`; `MixedSpeciesTest.unity` same plus `:7007`), with zero embedded `TreeSpeciesDefinition` copies, so these are the live values. Same bytes as `8f9ed80`, the Rack tip and `main`. *Caveat:* the existing automated assertion of these values is red for an unrelated reason (Q12/F-5), so this is confirmed by direct read, not by a passing gate. |
| 2 | Labels consistent with `GAMEPLAY CALIBRATION`, no empirical overclaim? | **LABEL/DOC GAP** | *Live asset/code labels do not overclaim:* each of the five fields in both assets carries `# [C] Ordinal production calibration v1; search-grid centre, not empirical truth.` (asset lines 18–27); C# tooltips say "calibratable"/"not a measured constant". `Dmax` 120 **is** the grid centre (`kernel_controls.py:7`: `[80,120,160]`), so that comment is accurate. *Gaps:* (a) the authoritative class name `GAMEPLAY CALIBRATION` is never written (only the coarse `[C]` tag, which PARITY-02 §7 maps to it); the word "Ordinal" collides with project class `ORDINAL REPRESENTATION` for what are cardinal values; (b) the asset tags `ci50` `[C]` while `TreeSpeciesDefinition.cs:25` tags it `[D]` and its legend (lines 3–5) defines `[C]`="simulation abstraction", `[D]`="gameplay/model calibration" (both map to the same class, so inconsistent rather than wrong); (c) **stale human docs overclaim or contradict** — see F-1…F-3. |
| 3 | Oak High / Beech Intermediate only a weak ordinal class distinction? | **CONFORMS (static)** | `growingSpaceDemand: 2` / `1` (`:28` in each asset). The class is read at exactly two production sites, `ForestEcologyController.cs:1165` (route) and `:1186` (density plan), plus configuration validation; it feeds only `MixedDensityModel.StemOccupiedSpace` (weights 1.0 / 1.5 / 2.0, `MixedDensityModel.cs:138-140`). The "growing space" strings in `UI/TreeInspectionView.cs` refer to the Hegyi competition label, not this class. D-061(3) labelling is met in code (`TreeSpeciesDefinition.cs:29` tooltip; `MixedDensityModel.cs:46-47` "[ORD] rank, [C] magnitude, provisional (D-061)"). *Advisory:* neither asset carries a comment on this field and nothing says specifically that **Oak > Beech** is the weak part (F-4, optional). |
| 4 | Adult growth uses the shared legacy asymptotic height law, without the Sitka age/site envelope? | **CONFORMS (static)** | `GrowAdults` `:1017-1021`: `dbh = Pd·site·clamp01(1−D/Dmax)·1/(1+CI/Ci50)`; `height = Ph·site·clamp01(1−H/Hmax)`. The Sitka envelope is applied only under `UsesSiteClassGrowth && SameSpeciesId(…, "sitka-spruce")` (`:1028-1029`), and `:1029` is the **only** production caller of `SitkaGrowthModel.NextHeight`. The Scenario Two age-phase hook (`:1024-1025`) is skipped because neither Oak nor Beech serializes `growthAgePhases` and `GrowthAgePhases.HasPhases` is false for an empty table (`SpeciesConfigurationPolicy.cs:285`). |
| 5 | Site-response seam dormant/neutral at exactly 1.0 for Scenario One P0? | **CONFORMS (static)** | `SpeciesSiteResponse.GrowthMultiplier` returns the `const float NeutralGrowthMultiplier = 1f` (`:87,115`); `NumericallyActive => false` (`:112`); `SiteGrowthResponse.Resolve` returns 1.0 for every species/site (`:198-204`). The Scenario One site is five Reference/Unknown descriptors, versioned (`ScenarioSiteProfiles.cs:21-26`). **No production file outside the three seam files references the seam** (grep over `Assets` and `Tools`; only offline checks do). The offline check already asserts this (`C_EVERY_SPECIES_SITE_COMBINATION_RETURNS_EXACTLY_1`, `H_NO_PRODUCTION_FILE_REFERENCES_THE_NEW_TYPES_OR_RESOLVER`); it was **not run** here. |
| 6 | Any species×site multiplier accidentally active in production? | **CONFORMS (static)** — none | The only site scalar in the adult path is `cell.SiteProductivity` (`GrowAdults:1014` via `GetSiteProductivity:1957`; also `ForestTreeMarkingManager.cs:426`). It is **never assigned in production** — only the field initializer `= 1f` (`ForestEcologyCell.cs:48`); every other assignment is in `Tools/Verification`. It is identical for all species, so it is not species×site. |
| 7 | Canopy/light shared; species differ only via crown geometry/relaxation? | **CONFORMS (static)** | `RecomputeCanopy` (`:822-851`) reads only position, crown radius and height; no species term. **No** transmissivity / opacity / deciduous / leaf-off identifier exists anywhere in production C# (grep). Species enter via `PotentialCrownRadiusM` (`TreeSpeciesDefinition.cs:246`), `CrownRelaxationPerYear` and the shared `Ci50` crown-target factor in `RelaxCrowns` (`:1229-1245`): Oak linear `0.10+0.1045·DBH`, relax 0.12 (asset `:31,34`); Beech power-law `ln CPA = 0.05+1.01·ln DBH`, relax 0.15 (`:31,34`); Sitka linear, 0.15. Beech's serialized linear crown fields are inert while `usePowerLawCrown: 1`. |
| 8 | Competition neighbourhood still the shared 8 m Hegyi cutoff? | **CONFORMS (static)** | One constant `HegyiCutoffMeters = 8f` (`:912`) used by `UpdateCompetition` (`:937,988`), the marking forecast (`ForestTreeMarkingManager.cs:402`), `ScenarioOne/CompetitionShare.cs:72`, `UI/CropTreeCompetition.cs:169,190`; no species term. *Coupling to note:* Rack-owned `ScenarioOneRackBay.FutureTreeSpacingM` is defined as this constant (`ScenarioOneRackBay.cs:22`; also `:321`), so changing it would silently move Rack V2. |
| 9 | `Ci50` confined to growth/crown/suppression and absent from density victim selection? | **CONFORMS (static) for growth model 2** — with a scoped note (A-1) | *Model-2 mixed route:* `DensityStem` carries no species id and the raw Hegyi index (`ForestEcologyController.cs:1185-1187`); suppression uses the single common `NeutralHalfSaturationCi = 5.0` and exponent 4 (`MixedDensityModel.cs:152,227-231`); the Sitka subset sees only `Reference` stems with the Sitka constant (`SitkaSubsetDensitySignal.cs:37,65`); `.Demand` appears in the composite planner only to total the non-Reference occupancy share (`:155`) and to guard the Sitka component (`:252`), never to rank victims. `Ci50` consumers are growth (`:1019`), the suppression diagnostic (`:1033,1830`), crown target (`:1239`), the marking forecast and UI diagnostics. *Scoped note:* the **legacy** route reads `treeSpecies.Ci50` per tree (`:1092`). It runs only for models 0/1 or model-2 pure Sitka (where every stem is Sitka, Ci50 = 5), as FR-1/FR-11 require. It would use a broadleaf's own `Ci50` only in a model-0/1 world containing a non-Sitka tree. No change recommended; see A-1. |
| 10 | Protected pure-Sitka path preserved? | **CONFORMS (static)** — runtime fingerprints owed to PARITY-03-UNITY | Against `c1564b8`, **all nine hunks in `ForestEcologyController.cs` are pure insertions** (no deleted or modified line; the legacy decision block is untouched). `KillDensityVictims` (`:1138-1158`) is a **verbatim extraction** of the original inline kill loop (`c1564b8:1102-1119`). `SitkaGrowthModel.cs` differs from `c1564b8` only in the `GrowthModel` header comment and enum (`SpeciesAware = 2`, `Latest`); no arithmetic function is touched (the PARITY-03 phrase "unchanged from base" is true of the arithmetic, not of the whole file). `SitkaSpruce.asset` is byte-identical to `c1564b8` and its normalized hash matches the contract. Model 2 satisfies `UsesSiteClassGrowth` (`:148`), so Sitka's height path is model 1's. |
| 11 | Beech provisional-height miss: calibration limitation only, or a separate defect? | **CONFORMS (static)** — calibration limitation only; **no separate implementation defect found** | Independent recomputation from the production recurrence (start: promoted 3.5 m stem, age ≈10): **Beech 23.37 m at age 50 and 34.65 m at 100** against provisional bands 25–30 m and 35–40 m (`kernel_controls.py:70`) → **miss 1.63 m and 0.35 m below the lower edges**. **Oak** 21.45 / 32.76 m sits inside 20–25 / 30–35 m. The numbers match PARITY-02 §5.1 and follow entirely from `Ph 0.75, Hmax 42.5` under the accepted shared law; the gate's `Law()` and the kernel `step` are textually the same formula; height has no competition term and `ApplyGrowth` only clamps to [0.1, 60]. The bands are labelled provisional `[C]` in the kernel. In-engine confirmation is not currently measured at those ages (Q12/U-4). |
| 12 | Exact Unity/runtime measurements still needed? | see §5 | Seven items U-1…U-7. |
| 13 | Exact files that would need production changes if all visible gaps were fixed? | see §4 | **No serialized YAML, no scene, no production C# is required.** Documentation, verification tooling, optional comment-only items. |
| 14 | Are those write surfaces owned by Rack V2, S1-C, another parity task or another live task? | see §6 | **No live owner and no live branch touches any file on the patch surface** (checked against current `main`). One shared-doc coordination point and one dependency/ordering point. |

---

## 4. Findings behind the matrix

### F-1 — Canonical overview doc is stale on `main` *(doc-only; Manager-owned)*
`Docs/Project/unity-project-overview.md:135`: "Beech/Oak keep Ci50 3 and 0.4 cm/yr." The same line is on `origin/main` (`fe81994`) and the Rack integration tip. It was true of the old `main` assets; since `f23549b` merged the lineage, `main`'s assets hold Oak 0.8/0.65/120/42.5/5 and Beech 0.9/0.75/120/42.5/6.5, so it is now wrong on `main`. PARITY-02 designated the overview Manager-owned (C-32). No other `Docs/Project` file states the accepted adult values.

### F-2 — `Docs/BeechSpeciesParameters.md` *(doc-only)*
The table gives Beech Pd 0.40, Ph 0.40, Hmax 40 (tagged **A/B**, "typical mature height"), Ci50 3.0 — all superseded — and says drought/waterlogging "should both be strong penalties", which reads as modelled although P0 site response is neutral 1.0. It also does not mention the accepted Beech height-band miss. The A/B tag on a superseded maximum is the closest thing to an empirical overclaim in the adult label surface.

### F-3 — `Assets/ForestPrototype/Docs/SessileOakV1.md:20-21` *(doc-only)*
"adult potential is 40 m height, 120 cm soft DBH scale, and 0.4 cm/year" — superseded (Hmax 42.5, Pd 0.8, Ph 0.65, Ci50 5). (`Docs/EcologyCalibrationAdoption.md:34` has the same stale figures but is a dated adoption record and was true when written.)

### F-4 — Label wording on the species assets *(optional; serialized)*
See Q2/Q3. If any asset comment is ever changed, **`check_integrated_v1.py:40` asserts the regex `# [C] Ordinal production calibration v1[^\n]*\n  <field>:`** for all five fields, so the prefix and the single-line-before-field layout must be kept (text may be appended on the same line). Unity regenerating an `.asset` on save does not preserve hand-written `#` comments (general Unity behaviour; **not verified in this session**), which makes asset comments a fragile label surface.

### F-5 — Stale verification contract; `check_integrated_v1.py` fails at the candidate *(verification-only; needs Manager authority)*
`integrated_source_contract.json` pins normalized (comment/whitespace-insensitive) hashes of seven files. At the candidate (and current `main`) three no longer match:

| File | contract | actual |
|---|---|---|
| `ForestEcologyController.cs` | `fdcfec2a478f6a04` | `ea1a204324f3c399` |
| `TreeSpeciesDefinition.cs` | `63c9078e7be818d4` | `346647ef4b3f4b5c` |
| `ForestSaveData.cs` | `224173ce7b7fd0ea` | `4734c9d733bf8f84` |

(`SitkaGrowthModel.cs`, `MixedDensityModel.cs`, `SitkaSpruce.asset`, `ScenarioOneObjectives.cs` match.) The unmodified gate stops at its first assertion: `AssertionError: Assets/ForestPrototype/ForestEcologyController.cs normalized source changed`, exit 1. **At `8f9ed80` all seven match**, so the drift entered after it. Attribution (by `git log -S` over `8f9ed80..e1515e1`): `ac4f34c` (Scenario Two species foundation; its arithmetic-preservation follow-up is `95e582e`) → `TreeSpeciesDefinition.cs` +85 lines and, in the controller, the guarded age-phase hook in `GrowAdults` and the `DispersesSeedNaturally` guard in `ComputeSpeciesSeedRain`; `fadc0c7` → `StandApproachPath.Apply`; `4fc05e1` → `StandApproachExpected`; `cf84d24` → `ForestSaveData.cs` (save v20 → v21). **Inspected deltas:** the only change in the adult path is the age-phase hook, inert for Oak/Beech (Q4); the rest is stand geometry, a seed-dispersal guard (regeneration, outside the adult path), and save v21. Docs elsewhere record this gate as passing (`Docs/Verification/ScenarioOneTier3Preflight/StaticPreflightFindings.md:188`, `ScenarioOneVerificationDebtRepair.md:30`), which was true at the older lineage; the drift itself is not recorded. A contract refresh is a deliberate, attributed, Manager-approved verification change — **not performed here** (packet: no rebaselining).

### F-6 — `BroadleafAdultGrowthVerification.cs` *(verification-only)*
- Header lines 21 and 23: "Oak and Beech currently share every adult growth parameter" and "NOT YET RUN IN UNITY" — both stale.
- It **reports** the constants (`OAK_BEECH_ADULT_PARAMETERS_REPORT` is always `Pass(…, true, …)` and prints only Oak's numbers) and **does not assert** them. A drift in an asset value would not fail this gate; the only assertion of the accepted values is `check_integrated_v1.py`, which is red (F-5). **Net: nothing runnable at the candidate currently asserts the accepted constants.**
- Last recorded Unity result is "14/14 on both geometries" at source head `90e3496` (`CompositeDensityV3Handoff.md:204`), a different lineage; `90e3496` is not an ancestor of the candidate. The harness has since changed (+21/−15: fixture now removes every non-grid stem each year) and now has **16** `Pass` sites (13 asserting + 3 informational). I found **no record of the current harness running in Unity**.
- It does not measure Oak/Beech height at ages 50/100 (it runs 60 years from age 12 and prints H20/H40/H60), so the Beech miss is not currently measured in-engine.

### F-7 — Stale "save version 20" text *(comment-only)*
`ScenarioSiteProfiles.cs:5-6` ("version stays 20") and `Tools/Verification/SiteInfrastructure/run_site_check.py:15` ("still 20"). Save is v21 (`ForestSaveData.cs:7`); the offline check itself asserts 21 (`G_SAVE_VERSION_IS_STILL_21`).

### F-8 — Latent duplication *(protected; no action)*
`SitkaSubsetDensitySignal.SitkaCi50 = 5.0` duplicates `SitkaSpruce.asset` `ci50: 5`; equality is pinned by the density offline suite, not at runtime. Irrelevant while Sitka is not retuned.

### Conformance summary

| | Count |
|---|---|
| CONFORMS (static) | 10 of the 11 conformance questions (Q1, Q3–Q11) |
| LABEL/DOC GAP | 1 (Q2) |
| MEASURED/STRUCTURAL DEFECT | **0** |
| UNITY QUALIFICATION REQUIRED | runtime proofs owed for all; none is an open unknown at the source level (§5) |

---

## 5. Unity / runtime gates still required (Q12)

All are owed **under the machine-wide Unity lock, on the exact integrated HEAD**, after the PARITY-03-UNITY / S1-C-IMPL-02 queue (§6). Follow the lane/lock rules in `Docs/Project/AgentWorkflow.md`; the harness headers document the copy → run → remove workflow, and no staged script or generated `.meta` may remain under `Assets/ForestPrototype/`.

| ID | Gate / measurement | Proves | Pass criterion | Exists? |
|---|---|---|---|---|
| U-1 | Clean import/compile on the exact HEAD | prerequisite | 0 `error CS` | existing |
| U-2 | `BroadleafAdultGrowthVerification` (headers: `CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -executeMethod BroadleafAdultGrowthVerification.Begin`; run on both geometries as the v3 run did) | Oak/Beech follow the shared law with CI 0 and site 1; growth is independent of model 0/1/2 without density; monotone and bounded; DBH (not height) falls under competition; replay and save/load continuation; Sitka height still Class III under model 2 | `BROADLEAF_GROWTH_VERIFY_PASS checks=16` by source count (13 asserting + 3 informational). **Do not compare with "14/14"** — different harness version | existing; **no recorded run of the current version** |
| U-3 | **New, verification-only** assertions added to the adult gate: (a) accepted constants read from the live loaded assets for Oak/Beech/Sitka; (b) `SiteGrowthResponse.Resolve(...) == 1f` for all three species at `ScenarioSiteProfiles.ScenarioOneReference` and `GetSiteProductivity == 1` at sampled cells; (c) `RecomputeCanopy` gives bit-identical `cell.Light` for the same crown radius/height/position whichever species owns it; (d) a Hegyi boundary pair (7.99 m counted, 8.01 m excluded) behaves identically across species; (e) `HegyiCutoffMeters == 8f` | Q1, Q5–Q8 at runtime (currently asserted nowhere runnable) | all pass | **proposed — does not exist** |
| U-4 | In-engine Oak/Beech/Sitka height at ages 50 / 75 / 100 from the promotion state (3.5 m, age ≈10), **printed for the record, not pass/fail** | documents the accepted Beech miss; expected ≈ Oak 21.45 / 28.18 / 32.76 m, Beech 23.37 / 30.24 / 34.65 m | recorded and labelled "accepted provisional-band miss, not empirical validation" | **proposed**; needs a longer horizon than the gate's 60 years |
| U-5 | Protected pure-Sitka / model-0/1 proof on the exact HEAD: lifecycle fingerprints `BFC55473C1506067`, `3485B6630C9EA448`, `7E57B9DAEF5BF4D0`, `35E2BF1C2F55C2DE`, Legacy40 start `EBE7A228F630AE41`, Sitka timber fingerprint | Q10 at runtime | exact; **not rebaselined** | owned by PARITY-03-UNITY (#10) / PARITY-09; PARITY-04 consumes it and re-runs only if its own patch could affect it |
| U-6 | Species-neutral density victim behaviour under model 2 (`MixedDensityVerification`) | Q9 at runtime | as PARITY-03-UNITY | owned by #10 |
| U-7 | **Non-Unity:** (i) `check_integrated_v1.py` → `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7` **after** a Manager-approved contract refresh (F-5); (ii) `Tools/Verification/SiteInfrastructure/run_site_check.py` → `SITE_INFRASTRUCTURE_PASS` (needs `dotnet` SDK 8+) | Q1/Q2 constants and labels; Q5/Q6 dormancy | pass | existing (i is red until refreshed; ii not run here) |

Not required for PARITY-04: `ScenarioOneCompletionVerification`, timber/economy gates, rendered review (no UI copy changes in the recommended scope).

---

## 6. Later patch surface (Q13) and ownership (Q14)

### 6.1 Exact later patch surface if every visible gap were fixed

**Serialized YAML — none required.** *Optional only:* `SessileOak.asset` and `BeechSpecies.asset` comment lines (a `growingSpaceDemand` comment; the authoritative class name appended on the existing five `# [C] Ordinal production calibration v1` lines — keep the F-4 regex). **Not recommended:** it touches the shared single-writer YAML surface, comments are fragile under Unity re-serialization, and the same labelling is achievable in docs and `//` comments.

**C# — none required.** *Optional comment-only:* `ScenarioSiteProfiles.cs:5-6` ("version stays 20" → 21; not one of the seven contract files). **Do not** change `TreeSpeciesDefinition.cs` tooltips (attribute strings alter its normalized hash and reopen the contract) or `ForestEcologyController.cs` (PARITY-03 density-writer file and a contract file). No scene, prefab, package, project-setting or save change.

**Documentation-only:**

| File | Change | Owner note |
|---|---|---|
| `Docs/BeechSpeciesParameters.md` | restate the accepted Beech values with project class `GAMEPLAY CALIBRATION`; remove the A/B tag on Hmax; neutral site 1.0; record the accepted height-band miss | no live owner |
| `Assets/ForestPrototype/Docs/SessileOakV1.md:20-21` | restate Oak adult values and class | text only; `.meta` untouched |
| `Docs/Project/unity-project-overview.md:135` | correct the Beech/Oak statement | **Manager-owned canonical doc** |
| `Docs/EcologyCalibrationAdoption.md:34` | optionally annotate as superseded | historical record |

**Verification tooling:**

| File | Change | Authority |
|---|---|---|
| `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs` | fix stale header (F-6); add U-3/U-4 | within PARITY-04 "focused verification" |
| `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json` | refresh the three drifted normalized hashes with the per-file attribution in F-5, then re-run the gate | **needs explicit Manager authority** (rebaselining a source-integrity contract) |
| `Tools/Verification/SiteInfrastructure/run_site_check.py:15` | docstring "still 20" → 21 | comment-only |

### 6.2 Ownership and conflicts (live state re-read at `fe81994`)

| Surface | Finding |
|---|---|
| **Rack V2** | Core integrated: `f23549b` (PR #12), lock-table row "INTEGRATED / MANAGER APPROVED"; repair claim #7 closed; `origin/integration/scenario-one-rack-v2-v1` has **0 branch-only commits** against `main`. No Rack row or branch lists or touches any patch-surface file. The packet/spec dependency "Rack V2 release of shared species-YAML/scene ownership" now appears satisfied in fact, but the live table records **no explicit release statement**, and `tasks.md` still shows PARITY-04 as `BLOCKED-READY` on it. |
| **Species YAML and scenes** | The "Rack owns shared species-YAML/scene" rule exists in the spec, plan, tasks and packet text, **not as an owner in any lock-table row**. No live branch touches `Species/*`, `Assets/Scenes/*` or `TreeSpeciesDefinition.cs`. |
| **PARITY-03** | `STATIC PASS / MANAGER APPROVED STATIC ONLY`; claim #8 closed; independent review #9 dispatched; Unity qualification #10 `[BLOCKED]` behind S1-C. Its density-writer group (`ForestEcologyController.cs`, `MixedDensityModel*`, `SitkaSubsetDensitySignal.cs`, `AdultMortalityAccount.cs`, density harnesses) is **not** on the recommended patch surface; `BroadleafAdultGrowthVerification.cs` is in the same directory as PARITY-03's harnesses but is an adult harness. |
| **S1-C** (`UI/AnnualReviewView.cs`, `ScenarioOneUiRoot.cs`, history files) | no overlap. `feature/s1-c-history-ui-v1` and `-query-v1` touch none of the patch surface. S1-C-IMPL-02 is `READY TO RESUME` and **ahead of PARITY-03 Unity** in the Unity queue. |
| **S1-D-T01, MFM-01** | read-only lanes; no overlap. `RACK-V2-04` (map planning mode) packet names no species, adult, calibration or doc-surface files. |
| **`Docs/Project/unity-project-overview.md`** | no live branch edits it now (the Rack integration branch's earlier edit is merged). Manager-owned. |
| **Unity lock / queue** | PARITY-04's Unity gates queue **behind** S1-C-IMPL-02 → PARITY-03-UNITY (#10). |
| **This task** | the Manager registered a `PARITY-04-PREFLIGHT` row during the run (`READY / DISPATCHED — task claim #11`). It has no write surface; no lock was taken. |
| **Base** | current `main` (descending from `f23549b`) already contains every audited production byte; no rebase risk for the adult surface. |

---

## 7. Ambiguities and decisions for the Manager

**Ambiguities (reported, not resolved):**

- **A-1 — FR-3 `Ci50` wording vs the legacy route.** FR-3 says species `Ci50` "must not drive density-victim selection". In the legacy route (growth models 0/1, and model-2 pure Sitka) `ForestEcologyController.cs:1092` does use the tree's own `Ci50`. Under model 2 the statement holds exactly; under models 0/1 it holds only where every tree is Sitka. FR-1/FR-11 require that legacy behaviour unchanged, so I read FR-3 as scoped to model 2 and recommend **no code change** — only that PARITY-04's verification wording say "under growth model 2".
- **A-2 — Rack release of YAML/scene ownership** is a stated dependency with no recorded release (see §6.2).

**Decisions needed before dispatching the production task:**

- **M-1 —** Authorise (or refuse) the refresh of `integrated_source_contract.json` for the three drifted files (F-5), or name an alternative gate. Without it the only automated assertion of the accepted constants stays red.
- **M-2 —** Who corrects `unity-project-overview.md:135` and when. It is wrong on `main` today (F-1).
- **M-3 —** Re-scope PARITY-04 production to **zero serialized-YAML/scene change** (docs + verification only). Recommended: it removes the Rack-YAML dependency entirely, leaving only PARITY-03 and the Unity queue.
- **M-4 —** Confirm the scoped reading of FR-3 (A-1) so no one "fixes" the legacy route.

## 8. Recommendation

**`BOUNDED LABEL/IMPLEMENTATION PATCH REQUIRED`**

Not `ZERO-DIFF REQUALIFICATION LIKELY`: the spec's own exit for PARITY-04 includes label correction, and the label surface is demonstrably stale (F-1…F-3) with a drifted contract and no runnable assertion of the accepted values (F-5, F-6). Not `MANAGER DECISION REQUIRED` for the policy itself: no accepted value or rule is in doubt and no defect needs a policy choice; M-1…M-4 are authority and sequencing calls on a patch whose content is already determined. The production task should be expected to land a **docs + verification-tooling patch with zero value/arithmetic/serialized-asset diff**, then qualify under U-1…U-7.

## 9. Evidence locations

Raw outputs (git state, SHA-256 of every audited file at the candidate, candidate-vs-Rack-tip and candidate-vs-`main` identity, static greps, protected-Sitka diffs, contract drift and unmodified-gate failure, height recomputation, live-lock diff, branch overlap) are in:

`/media/jer/Files/CCF/Verification/Parity04-Preflight-20261027T194211Z/`

## 10. Standard handoff block

```text
IMPLEMENTATION HANDOFF
Task: PARITY-04-PREFLIGHT — adult / species-data read-only requalification audit
Worker: Claude Sonnet 5.5 (claude-sonnet-5-5) via Claude Code in T3 Code, local session (not Claude Cloud, not Opus)
Role: Reviewer (read-only audit)
Branch: research/scenario-one-three-species-parity-p0-04-preflight-v1
Base: origin/main ce8a1742cdd768be327dfd0d26bf85485a26d3fa (packet read); re-verified at fe8199441b5ab69c7644797b3e3792cc3c3674eb
Context commit: not applicable (read-only; no context regeneration)
HEAD: the commit that adds this file (read back from the remote)
GITHUB HANDOFF PATH: Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Preflight/HANDOFF.md
GITHUB HANDOFF COMMIT: read back from the remote after push
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/Parity04-Preflight-20261027T194211Z/
WHAT CHANGED: one new documentation file (this handoff)
FILES CHANGED: Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Preflight/HANDOFF.md
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none
MUST-NOT-CHANGE HASHES / ANCHORS: not touched; none rebaselined
INTENTIONALLY CHANGED HASHES / ANCHORS: none
VERIFICATION RUN: read-only git inspection; source_normalization + unmodified check_integrated_v1.py on a scratch extract outside the repo; stdlib height recomputation
VERIFICATION RESULTS: 0 defects found; contract drift in 3 of 7 pinned files (check_integrated_v1.py fails at candidate and main); Q1, Q3-Q11 conform statically; Q2 label/doc gap
SPEC CONFORMANCE: NOT VERIFIED at runtime (static audit only)
REMAINING VERIFICATION GATES: U-1 ... U-7 (section 5)
KNOWN LIMITATIONS: no Unity; PARITY-03's measured density numbers not re-measured; offline .NET suites not run; Unity comment-loss on re-serialization not verified
UNRELATED WORK PRESERVED: yes (Primary checkout and its uncommitted changes untouched)
DECISIONS NEEDED: M-1 ... M-4 (section 7)
WORKER ASSESSMENT: Ready for review: Yes
RECOMMENDED NEXT STEP: Manager review; decide M-1 ... M-4; then dispatch a docs + verification PARITY-04 production task queued behind PARITY-03-UNITY
```

`DASHBOARD SYNC: NOT PERFORMED` — this session has no dashboard publisher. `Published dashboard reflects this task: NO` (outstanding operational item for the Manager).

**Exit:** `PARITY-04 PREFLIGHT COMPLETE — READY FOR MANAGER REVIEW`
