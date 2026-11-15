# MULTI-010: independent forestry-evidence and implementation-method review of MULTI-009 / LAB-06 (Sitka P1)

**Date:** 15 November 2026 (UTC; claim registered 2026-11-15T00:00Z)
**Claim:** [#525](https://github.com/jermur788/CCF/issues/525)
**Binding packet:** `docs/multi-010-lab06-sitka-p1-independent-review-packet-20261114-v1`, `Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-010-LAB-06-SITKA-P1-INDEPENDENT-FORESTRY-AND-METHOD-REVIEW-PACKET-20261114.md`
**Study under review:** MULTI-009 / LAB-06 by OpenAI Codex GPT-6.1 Sol, `research/multi-009-sitka-p1-source-evidence-feasibility-20261114-v1` @ `2212544e90fc5c48195f708c525e58be8a8cd936`

## Verdict

**APPROVED AS P1 EVIDENCE PLAN**, judged on study quality only.

This review found **no BLOCKING and no MAJOR defects**. It records **eight MINOR corrections or clarifications** (§6) and **seven conditions** that a future P1 DRAFT has to settle (§7). The study's source mapping is accurate at the pinned snapshot. Every cited line range I checked (about 35) was correct, and all nine arithmetic vectors reproduce independently. Its scientific boundary is honest: GROWFOR/Broad–Lynch is treated as a stand-level B/N/H reference and never as Irish per-tree or CCF forecast validation. Its P0/P1 separation is technically sound.

What the verdict does **not** mean:
- **No P1 spec is approved.** The verdict only allows a separate four-file P1 **DRAFT** for the user and Manager to review.
- **No LAB-07 code is authorised.**
- **No Unity-model parity is established.** No P1 adapter exists and no Unity code was run.
- **No Irish field accuracy is established.**
- **No P1 growth model is scientifically verified.**

The Manager's disposition and the product owner's decision on whether to start the parallel P1 experiment both still apply.

## 1. Reviewer identity, host, worktree and scope

| Item | Value |
|---|---|
| Reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), effort High, running in the Claude Code harness inside local T3 Code. I did not author the study, which was written by OpenAI Codex GPT-6.1 Sol. |
| Host | Linux laptop `jer-laptop` (user `jer`) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-010-SitkaP1-EvidenceReview-20261114`. I created it fresh for this review. Codex's Windows/WSL NTFS worktree was not touched. |
| Branch / starting HEAD | `review/multi-010-lab06-sitka-p1-forestry-method-20261114-v1` @ `2212544e90fc5c48195f708c525e58be8a8cd936`. This equals the pinned study head. Status was clean (0 entries), and the diff against main `fc18190b` was one added Markdown file. |
| CCF source snapshot inspected | main `fc18190b976f1363f1b5d1b0881810db2f8c6e39`. `git diff --quiet fc18190b HEAD -- Assets Tools specs Docs/Research Docs/Project` was empty, so the study head's source equals main. |
| P0 lab snapshot inspected | MULTI-008 `2659db99e317d4aa93b51cf97131a61eab5fdeec`. `git diff --name-only c445f870 2659db99 -- Tools/MultiRackSitkaLab` returned 0 paths, which matches the study's claim. |
| Preflight | #525 was fresh-read as READY / NOT RUNNING. Live `active-tasks.md` on main had no MULTI-009/010/#522/#525 row. No MULTI-010 worktree or evidence folder existed beforehand. No lab or dotnet process was running. I posted the RUNNING registration ([comment](https://github.com/jermur788/CCF/issues/525#issuecomment-6886252359)) before inspecting any source. |
| Off-repo scratch | `/media/jer/Files/CCF/Verification/MULTI-010-SitkaP1-EvidenceReview-20261114/` |
| Primary PDFs | **NOT AVAILABLE.** I searched the laptop's T3 attachments, `~/Downloads` and the `/media/jer/Files/CCF` Research/Documentation/Media trees. The only GROWFOR material found was the tracked extraction (and a Downloads copy of it). The two forestry compendium PDFs among the attachments do not contain Broad & Lynch or GROWFOR. **No primary page was verified by this review.** Every G1/G2a/G2b/G3/G4 page below comes from the extraction. |

Evidence labels follow the packet: **DIRECT SOURCE CHECK**, **REPRODUCED ARITHMETIC**, **PUBLISHED EMPIRICAL** (as represented by the extraction), **DERIVED**, **HISTORICAL TEST ONLY**, **NOT VERIFIED**, and **UNSUPPORTED / NEEDS DECISION**.

## 2. Question 1: evidence-to-source mapping

### 2.1 Line-range audit of study §3 (DIRECT SOURCE CHECK at `fc18190b`)

I opened each cited range. All of them match.

| Study claim | Verified location | Result |
|---|---|---|
| Species parameters | `Species/SitkaSpruce.asset:15–29`; `TreeSpeciesDefinition.cs:137–147,246–255` | ✓ 1.2 / 120 / 0.45 / 35 / Ci50 5 / 0.9415 + 0.07635·D / relax 0.15 / form 0.5. `growingSpaceDemand` is not serialised in the asset, so it takes the field default `Reference` (`TreeSpeciesDefinition.cs:30`). No age phases (`:114,184`). |
| DBH | `ForestEcologyController.GrowAdults:1003–1038`, `ApplyGrowthAgeFactors:1040–1046`, `GetSiteProductivity:1957–1961` | ✓ formula and pre-growth CI. Age increments after growth. Suppression history is diagnostic only. |
| Height | `SitkaGrowthModel.SiteTopHeightM:45–49`, `NextHeight:55–60`, replacement at ecology `:1028–1029` | ✓ Class III, b0 derived from E(30) = 20.4, `Max(1, age)`, float cast, no CI or site multiplier. Applies only to the Sitka species ID, at growth model ≥ 1. |
| Hegyi | `HegyiCutoffMeters:912`, `HegyiTerm:923–926`, `UpdateCompetition:930–999`, `FindTrees:875–886`, `CompareTreeIds:863–866` | ✓ inclusive `distance > 8f` reject, slack prune `1.00002f`, inline term kept for bit-exactness (comment `:990–993`), ordinal ID order, stumps excluded as neighbours and targets. |
| Crown | `RelaxCrowns:1229–1245`; `ForestTree.RelaxCrownRadius:631–636` | ✓ post-growth DBH, pre-growth CI, lerp 0.15, floor 0.1 m. |
| Density input | `StandRelativeDensity:1214–1225`, `StandAreaHectares:73`, `SitkaGrowthModel.RelativeDensity:71–75` | ✓ all living `ForestTree` objects. Area is the **square** `standSizeMeters²/10 000` (see m-5). |
| Hazard and boundary | `SitkaGrowthModel:89–114`; `ApplyAdultDensityMortality:1062–1135` | ✓ including the `count <= 1` guard at `:1126`, which means a universal RD ≤ 1 promise is false (§3.3). |
| Version route | `GrowthModel:14–25`; `UsesSiteClassGrowth:148`, `UsesSpeciesAwareGrowth:151`, route `:1072–1081`, `HasNonReferenceDemand:1160–1169` | ✓ See §2.2. |
| Natural death vs felling | `ForestTree.IsLiving:82`, `Fell:538–545`, `ApplyMortality:551–564`, `RestoreMortality:567–582`; `KillDensityVictims:1138–1158` | ✓ idempotent death (`if (!IsLiving) return false`). Restore raises no event. Felling leaves the biological cause unset. |
| Consequence | `OnTreeFelled:418–427`, `OnTreeMortality:429–437`, `QueueLivingCanopyRebuild:439–452`; `ScenarioOneManager.OnTreeBiologicalDeath:524–548`, `ResolveFelling` deadwood retention `:1628–1664` | ✓ Felling can retain fallen deadwood, so deadwood alone does not show the cause of death. |
| Clamps | `ForestTree.ApplyGrowth:623–629`, `SetAgeYears:638–641` | ✓ |
| Annual order | `ScenarioOneManager.cs:1348–1357`; `AdvanceOneYear:458–499` | ✓ approved work and settlement, then one ecology step, then juveniles, understorey and deadwood. Density mortality runs before the year's promotions. |
| New game / load / validation | `ScenarioOneManager.cs:462–503`; `ForestSaveData.cs:7`; `ForestSaveController.cs:120,495–497`; `ForestSaveValidation.cs:80–81` | ✓ New game uses RNG 1, regeneration 2, growth **2**, storm 0. Saves at version < 17 load as growth 0. Unknown models are rejected. Schema is **24**. |
| Per-tree save fields | `ForestSaveData.cs:196–210` | ✓ All are `float`/`int`. This matters for m-4. |

The study's citations to the P0 lab at `2659db99` also match: `Models.cs:6–19,39–145`, `Engine.cs:168–174,239–253`, `Metrics.cs:30–64`, `LabFile.CheckHeader:94–106`, `Keys:164–176`, `SnapshotValidator.cs:65–75` and `LabSession.AdvanceExperimentStep:247–253`.

### 2.2 Model 2 vs model 1, and where "faithful" has meaning (DIRECT SOURCE CHECK + DERIVED)

Within the ecology annual step, the **only** branch that depends on growth model 2 is the density-mortality route selection at `ForestEcologyController.cs:1075–1076`. I confirmed this with a repo-wide grep for `UsesSpeciesAwareGrowth` and `GrowthModel.SpeciesAware`. The other hit is `ScenarioOneManager.BroadleafMarketActive:508`, which is economy and outside the ecology step.

`MixedDensityModel.SelectRoute:176–179` returns the occupancy route only when growth ≥ 2 **and** at least one living stem has `GrowingSpaceDemand != Reference`. The class-III height replacement depends on the **species ID** being `sitka-spruce` (`:1028`), not on the demand class.

From this, **"faithful CCF adult rules" has a precise domain**:
- every living `ForestTree` is the Sitka asset, with demand class `Reference`, no age phases and no pruning lifts;
- growth model 1 or 2, which are equivalent on this domain at the ecology level (DERIVED from source, not runtime-executed);
- site productivity 1, which is all the current source can produce (see m-2);
- every tree in `ForestTreeStage.Mature` (see m-1);
- no storm (storm model 0), no recruitment, no juveniles.

The adapter must **refuse as out of scope**:
- any non-Sitka stem. A single Oak or Beech recruit flips a model-2 game onto the occupancy route.
- a non-zero storm model;
- pruned trees;
- authored age phases;
- growth model 0. Model 0 may run only as an explicitly labelled diagnostic comparator, never as the target.

The study's route row (§3, "Versioned route") is correct. Its instruction "restrict … explicitly to pure Sitka/model 1; do not advertise it as the entire current game model 2" is the right claim boundary.

### 2.3 Schema 24 vs stale harness assertion 23 (DIRECT SOURCE CHECK + git history)

`Tools/Verification/SitkaGrowthMortality/SitkaGrowthModelVerification.cs:96–98` asserts `original.version == 23` inside `POLICY_NEW_GAME_GROWTH2`. The harness was last edited by `03790ab7` ("MFM-06: save v23 …"). `ForestSaveData.CurrentVersion = 24` was introduced by `129d0ace` ("Persist first guided rack review completion …"), committed later the same day, and `git merge-base --is-ancestor 129d0ace 03790ab7` is false.

The mismatch is therefore **pre-existing test staleness, not a source defect**. The save-version pin was not moved when v24 landed. The study's classification ("do not assume this historical harness is green on current main") is correct. I did not run this harness and did not edit it. On main it would be expected to fail that one policy assertion. **NOT VERIFIED by execution.**

### 2.4 Status of the 14 vectors

| Study label | Count | My finding |
|---|---|---|
| REPRODUCED (Python arithmetic) | 9 (V01–V09) | **All nine reproduced independently** (§3). They are formula arithmetic only and not Unity parity. The study states this correctly at §4 l.75 and l.96. |
| DERIVED | 3 (V10–V12) | V10 and V11 agree with my reading of source (`ForestTree.cs:538–582`; `ForestSaveController.cs:495`; `ForestSaveValidation.cs:80–81`). V12's hashes are **HISTORICAL TEST ONLY**. I did not re-check them. |
| UNAVAILABLE | 2 (V13 Unity/adapter parity, V14 field holdout) | Correctly unavailable. Nothing here changes that. |

The 9 + 3 + 2 classification is accurate. Nowhere does the study let "REPRODUCED" stand for runtime parity.

## 3. Independent recomputation (REPRODUCED ARITHMETIC; not Unity parity)

**Method.** I wrote a new scratch script, `scripts/independent_recompute.py`, directly from the C# methods cited in §2.1. I did not see or copy the Codex script. It runs two modes:
- **b64:** binary64 throughout.
- **f32:** every `float` operation in the source rounded to binary32. Steps the source types as `double` (`SiteTopHeightM`, `RelativeDensity`, hazard) stay binary64.

Command: `python3 -I scripts/independent_recompute.py <fixtures>`, exit 0, Python 3.12.3.
- Script SHA-256 `d61d2e23587070772d182414c9f7aa3bda5b3546616f11f1cc143f1909d45b39`
- Results SHA-256 `64f61711e35758b792e2b96be6173d8de7cadfc13110137d7f4490843f9eb8f4`

The scratch files are not an archive. The values below are the evidence.

| Vector | Study value | My b64 value | My f32 value | Agreement |
|---|---|---|---|---|
| V01 D = 30, CI = 0 | 0.9 | 0.8999999999999999 | 0.9000000357627869 | ✓ |
| V02 D = 30, CI = 10 | 0.3; S = 2/3 | 0.29999999999999993; S = 0.6666… | 0.30000001192092896 | ✓ |
| V03 pair at 8 m | CI 0.125, inc 0.878048780488; coincident term 2; D = 120 gives 0 | 0.125; 0.8780487804878049; 2.0; 0.0 | 0.125; 0.878048837184906; 2.0; 0.0 | ✓ |
| V04 b0 / E(a) | 34.3615853937; E20 14.2057935864 … E100 33.5596227181 | 34.36158539365769; E20 14.205793586373566; E40 24.892424832918607; E60 30.13938399746039; E80 32.51438794826428; E100 33.559622718146365 | (double in source) | ✓ |
| V04 h = 10 m, age 20 | 10.4926393912 before cast | 10.492639391156711 | 10.492639541625977 after `(float)` | ✓ |
| V05 RD | 0.599571734475; 1.0 | 0.5995717344753747; P = 0; 1.0 | (double) | ✓ |
| V06 hazard | P = 0.5; p = 0.02; cap 0.5; mean 0 gives 0 | 0.5000000000000001; 0.02; 0.5; 0.0 | (double) | ✓ |
| V07 felling between years | CI 0.5, inc 0.818181818182, D1 30.8181818182, inc2 0.891818181818, D2 31.71 | 0.5; 0.8181818181818181; 30.818181818181817; 0.8918181818181817; 31.709999999999997 | 0.5; 0.8181818127632141; 30.81818199157715; 0.8918182253837585; 31.71000099182129 | ✓ |
| V08 crown | potential 3.300715, next 2.19510725 | 3.300715; 2.19510725 | 3.3007149696350098; 2.1951072216033936 | ✓ |
| V09 roll, RNG model 1, `ADULT-MORT-T0001`, year 1, seed 20260914 | 0.029643237590789795 | 0.029643237590789795 (exact) | n/a (24-bit value, exact in float) | ✓ |

**Additional independent checks:**
- **V09 under RNG model 0** gives **0.1856723427772522**. The roll depends on `rngModelVersion`, not on the growth model (`SimulationRandom.Roll:41–55`). See m-6.
- **Float area.** `StandAreaHectares` for Legacy40 is the float `0.1599999964237213`. N/ha is `int / float` in float arithmetic (`:1222`, `:1125`). For N = 180 both modes give 1125.0. In general this is a float-precision source step that the study's "double density math" wording blurs (m-4).
- **Tiny-stand boundary.** One 60 cm tree on 0.0016 ha has RD = **2.036**. The boundary loop stops at `count <= 1`, so RD stays above 1. This confirms the study's singleton caveat.

**What the f32 column shows:** single-step f32 and b64 results differ at about 1e-8 relative. Over many years such differences can flip discrete outcomes (`roll < p`, suppression-order ties, the 8 m inclusion edge). This is why the numeric type must be specified (C-2).

## 4. Question 2: scientific claim boundary

### 4.1 GROWFOR / Broad–Lynch representation (checked against extraction V1B; primaries NOT VERIFIED)

Every factual number in study §5 matches `GROWFOR_Technical_Evidence_Extraction_v1.md` at `fc18190b`:
- B/N/H stand state (§4.1, §2 item 1);
- SI = top height at age 30, with global `a = 47.0216, c = 0.4571, t0 = −5, H0 = 0` (§4.5–4.6, §5);
- 1,330 thinned and 540 unthinned trajectories (§6.3);
- the closure state was not retained (§7);
- thinned PSPs start at first thinning, and unthinned plots were first measured at about age 20 (§11.1);
- the manual's `H < 49.3 m` is scoped to the illustrated Sitka-thinned model (§11.1–11.2). The study does not present it as a universal limit.
- 2006b: 44,217 estate subcompartments, 113 nonresearch stands (70/43), and the separation of the two independence fields (§2A.1–2A.2, §13.0);
- 2017 NFI: 200 plots (107/93), 183 with ingrowth; marginal ranges; BA bias −1.96% / RMSE 12.81%; volume −1.41% / 18.80% (§13.2–13.3);
- 2013: model-to-model cumulative volumes 1026/1014 and 1135/995 (§13.1);
- version caveat and the `BroadLynch2006_Sitka` naming (§12).

The study does **not** overstate predictive validity:
- It says explicitly that 2006b is source-data validation and not an untouched final-model holdout.
- It treats the NFI ranges as observed marginal ranges, not joint admissibility bounds.
- It says the NFI RMSE is "never P1 pass thresholds".
- It states that the N state carries no individual or cause-specific hazard.

This review endorses those boundaries. One precision issue remains (m-7).

### 4.2 Separating measured evidence from game calibration

The study's §5 "GAMEPLAY CALIBRATION / transfer" paragraph separates the Irish published anchor from the rest correctly, and it matches source comments and D-048. The only Irish published anchor is E(30) = 20.4 m. Everything else is either transfer or calibration:
- the Comeau slope 2.063 and SDI 1868 are a labelled British transfer (`SitkaGrowthModel.cs:63–65`, D-048);
- onset 0.6, strength 0.08, S⁴, the 0.5 cap, the 8 m cutoff and Ci50 = 5 are [C] calibration choices (`SitkaGrowthModel.cs:89–96`, ecology `:909–912`);
- form factor 0.5 is unvalidated.

**Top-height definition (NOT VERIFIED).** Neither the extraction nor any primary I could access states Broad & Lynch's exact top-height inventory definition. The CCF research notes already flag "100 largest-DBH stems/ha" as unconfirmed (`Docs/Research/SitkaGrowthMortality/IrishHeightValidation.md:53`). The study correctly calls its H convention a model diagnostic awaiting comparison with the source definition.

### 4.3 Is an adult-only closed population a coherent experiment? (DERIVED)

Yes, as a **plantation-like comparator**, which is the domain where GROWFOR has authority. It becomes misleading only if it is presented as CCF-world parity.
- Broad–Lynch N dynamics describe even-aged plantations with no modelled ingrowth (extraction §2A.3–2A.4). A closed population with no recruits is therefore *closer* to that reference than the full CCF world.
- In the full game, `PromoteCohorts` adds `ForestTree` recruits at DBH 2–20 cm (`ForestEcologyController.cs:1733`, `JuvenileEcologyRules.PromotionDbhCm:37`). These recruits immediately join the all-living RD and act as Hegyi neighbours and targets. A closed adult lab cannot reproduce that divergence source.

The study states both sides (§3 annual-order paragraph; §5 l.119). The P1 label should read "CCF adult rules on a closed pure-Sitka population (no recruitment, storms, light or regeneration)".

The most valuable falsifiable use of Option A is to compare the closed CCF adult rules against the `BroadLynch2006_Sitka` **unthinned** N decline. That tests whether the British-SDI onset and strength calibration produces an Irish-plausible self-thinning trajectory. Two warnings apply:
- Tuning the [C] constants to that output and then calling the result "validated" would be circular, because GROWFOR output is model output, not data.
- Any such tuning would be a production model change, which needs its own decision.

## 5. Question 3: P1 architecture and scope

### 5.1 P0 preservation is feasible in principle (DIRECT SOURCE CHECK + DERIVED)

- `LabFile.CheckHeader:94–106` requires `format = "ccf-sitka-lab"` first and `labSchemaVersion == 1` second. `Keys:164–176` rejects unknown or out-of-order keys. So a P1 file with a **distinct `format` string** and its own schema counter will always be refused by the P0 reader, and P0 files and fixtures stay byte-identical. The study's "separate explicitly versioned P1 experiment" is technically sound. I recommend the distinct-`format` route (C-1).
- `Engine.Apply` for `ExperimentStepAdvanced` (`Engine.cs:168–174`) changes calendar and revision only. The study's rule never to overload it with growth is correct.
- `SnapshotValidator.cs:66–74` requires every open-plan tree to satisfy `IsAdult` (live **and** DBH ≥ threshold), and every executed-plan tree to stay `Felled`. Growth only ever adds eligibility, because DBH never decreases, so annual growth cannot invalidate an open plan. A **natural death** of a reserved tree can. The study's demand for atomic cancellation or staling of the plan is correct and necessary.

### 5.2 Population inclusion, quantified on the actual P0 fixtures (REPRODUCED ARITHMETIC)

Fixtures were read from `2659db99`. Area is the polygon shoelace area, threshold is 100 mm, and RD uses the CCF formula.

| Fixture | Area (ha) | Live / P0-Dead | Live below threshold | RD, all living (CCF rule) | RD, action-eligible adults only |
|---|---|---|---|---|---|
| `oracle-quarter-ha` | 0.25 | 3 / 0 | 0 | 0.0101 | 0.0101 |
| `sitka-flat-demo-v1` | 0.58125 (non-square pentagon) | 153 / 9 | 15 (9.8%) | 0.1698 | 0.1689 |
| `sitka-flat-perf-2000-v1` | 2.0 | 1893 / 107 | 182 (9.6%) | **0.6085** | **0.6057** |

What this means:
1. **The demo fixture never exercises density mortality** for decades. Reaching RD = 0.6 at constant N needs Dq of about 50 cm, against 27.4 cm now. This is a DERIVED estimate, not simulated.
2. **The perf fixture sits right at the 0.6 onset.** The inclusion choice moves P from about 0.021 to about 0.014, so first-year expected hazard deaths differ by about 2× in relative terms, though both are tiny in absolute terms.
3. **About 10% of live P0 trees are below threshold.** They act as Hegyi neighbours in CCF and grow into action eligibility over time.

The study rightly says this inclusion needs an approved scope decision. The numbers above show the decision is material at the onset and is not a cosmetic naming choice (C-3).

### 5.3 Natural death, felling and replay

The study's ledger (§6 table) and accounting gates fit the source semantics:
- separate cause and year;
- felling event ID with plan provenance;
- the identity `N_end = N_start − fell − death` with no double counting;
- RNG keyed on the incremented ecological year and the stable ID, never wall-clock time, list index or session epoch;
- reload does not reseed and does not re-fire deaths;
- unknown versions fail closed.

These follow from `ApplyMortality` and `RestoreMortality` (`ForestTree.cs:551–582`) and `Roll(…, "ADULT-MORT-" + id, ecologicalYear, seed)` (`:1103`). The study also correctly forbids inferring cause from deadwood (`ResolveFelling:1628–1664`).

One gap remains: P0 `Dead` trees, which carry no cause or year (m-3).

### 5.4 Staged packets P1-01…P1-05: order and ownership

The order is basically sound: oracle, then adapter, then ledger/save/replay, then B/N/H comparator, then minimal UI. Two dependency gaps should be written into the DRAFT (C-6):
- **P1-01 needs a runtime capture route.** A source-parity oracle needs per-ID outputs from the real current source, and that means an authorised Unity lane. Reasonable alternatives are a reviewed plain-.NET binding of the exact compiled source, or a narrowly extracted static-method harness. The study names "current-source per-ID inputs … to adapter" (gate 5) but does not say who owns that capture or what tooling it uses. The stale v23 assertion (§2.3) needs separately authorised repair before that harness is relied on.
- **Scope decisions C-2…C-5 have to come before P1-01 fixtures**, because the fixture expectations depend on them. P1-04 depends only on P1-02 output and could run in parallel with P1-03.

### 5.5 Existing Broad–Lynch comparator (HISTORICAL TEST, re-run as an offline self-consistency check only)

`Tools/Verification/Growfor/` exists as the study says. I copied it to scratch, so no `__pycache__` was written into the worktree, and ran:
- `test_broad_lynch_2006_sitka.py`: **24 tests OK**
- `test_compare_ccf.py`: **18 tests OK**

These tests only show that the reconstruction is internally consistent with the extraction's coefficients. They prove nothing about field accuracy or about equivalence with the GROWFOR executable. The study's notes on this tool are confirmed:
- README line 13 still says the extraction is "not tracked in Git", which is stale;
- the example JSON is historical;
- SI is inferred from the first row.

I agree there is no reason to duplicate the tool. Requalify and reuse it.

### 5.6 Option B

I agree with the study's deferral. B would have to allocate stand-level ΔN and ΔB to individual IDs with a new and unvalidated allocation rule, and nothing currently available can test such a rule.

Evaluating B becomes worthwhile only when **all** of these are in hand:
1. Licensed Irish (or explicitly labelled transferable) **spatially mapped, repeated-measure individual-tree** Sitka data: stem XY, DBH, height and ideally crown, with at least two remeasurement intervals.
2. Recorded **cause of death and thinning provenance** per stem.
3. Coverage of the target structures, meaning plantation **and** transformation/irregular stands, not only low-thinned plantations.
4. Rights that permit derived-model publication inside CCF.
5. A pre-registered **holdout** split by stand, not by tree.

#479 has supplied none of these. Until then, A or NO-GO are the only honest routes.

## 6. Findings

None is BLOCKING or MAJOR. Each repair is documentation-only and belongs in the P1 DRAFT or in a study erratum if the Manager wants one. No code or spec edit is implied.

| ID | Sev. | Finding (evidence) | Smallest documentation-only repair |
|---|---|---|---|
| m-1 | MINOR | `ForestTree.Height` returns stage-dependent `CurrentHeight`: 1.1 m for Sapling and `min(2.7, h)` for Young (`ForestTree.cs:161,264–275`). Both `NextHeight(tree.Height, …)` and the legacy height law read this value, while `ApplyGrowth` writes `heightMeters`. The study does not mention the stage dependence. It does not matter for Mature trees but must be pinned. | DRAFT scope: every P1 tree is `Mature`-equivalent, and the oracle asserts stage = Mature. Non-Mature stages are out of scope. |
| m-2 | MINOR | The study calls site productivity = 1 an "explicit experimental restriction". In current source nothing writes `SiteProductivity`: the field initializer is 1 (`ForestEcologyCell.cs:48`) and a repo-wide grep finds only reads. So it is **current-game parity**, not a restriction. | Re-label it as "equals the current source; no varying-site rule exists". |
| m-3 | MINOR | P0 fixtures contain `Dead` trees with no cause or year: 9 in demo, 107 in perf. CCF `ApplyMortality` and `RestoreMortality` reject empty causes (`ForestTree.cs:554–555,569–570`). The study does not cover how P0 `Dead` maps into P1. | DRAFT decision: map them as pre-inventory dead (excluded from living, CI and RD) with an explicit synthetic cause such as `pre-inventory` and a year sentinel, and never count them as model deaths. |
| m-4 | MINOR | The study's numeric wording ("float32 Unity operations with double density/height math"; P1 "full-precision" state) blurs four facts: N/ha is computed `int / float` in float (`:1222,1125`); the mortality `ci / Ci50` is evaluated in float before being promoted (`:1092`); `NextHeight` returns `float`; and persisted CCF state is `float` (`ForestSaveData.cs:198–201`). "Full precision" could mean binary64, which cannot be bit-faithful to the source. | DRAFT: declare the authoritative P1 numeric type. For parity, use binary32 with an exact-round-trip serialisation contract. Binary64 is acceptable only with a reviewed tolerance and a stated risk of discrete flips. See C-2. |
| m-5 | MINOR | The study recommends area "from polygon, not a 40 m/80 m guess". The source uses a **square** `standSizeMeters²` (`:73`). Polygon area is an adapter rule, not source parity. The source grid is also centred on the origin (`StandBounds:75`), while P0 coordinates start at a polygon corner. Translating to centred coordinates changes float rounding near the 8 m edge. | DRAFT: use square polygons for parity fixtures. Treat polygon area as a declared adapter extension. Specify the coordinate origin/translation used for float distances. |
| m-6 | MINOR | V09 says "model1" without saying it means **RNG** model 1 (MixedModel). The roll depends on `rngModelVersion`, not on the growth model (`SimulationRandom.cs:41–55`). Under RNG 0 the same inputs give 0.18567. | Write "RNG model 1" and carry the RNG and growth model versions as separate axes in the ledger. The study's ledger already lists the RNG version. |
| m-7 | MINOR | Study §5 "Source-data validation" says 2006a "used those 113 for calibration and adjusted fitting weights" without naming *which* model. Per extraction §2A.1 they calibrated the **volume** model (`validation_independent_of_final_volume_model_calibration = false`), not the B/N/H growth system. Separately, extraction page locators mix PDF-relative G2a pages (pp. 4–29) with printed pages (pp. 68–70). The study notes this, but the mix remains easy to misread. | Say "volume-model calibration" and mark page bases explicitly (PDF-relative vs printed) until a primary-PDF check is done. |
| m-8 | MINOR | The study does not quantify how much the population-inclusion and fixture-domain choices matter. §5.2 shows ~10% of live trees below threshold, the perf fixture RD sitting at the 0.6 onset, and a demo fixture that never reaches density pressure. Fixture heights at age 28 are 9.5–18.05 m against Class III E(28) = 19.30 m, so the P0 fixtures imply SI below 20.4 under any top-height convention. The SI-policy choice is therefore unavoidable for every P0 fixture, not optional. | Add the §5.2 table to the DRAFT and require dedicated density, boundary and SI fixtures in P1-01. The study's case list already includes "dense hazard", "boundary" and "tiny-area". |

## 7. Conditions the P1 DRAFT must settle (owner/Manager decision points)

These are what the approval depends on. None may be filled with a made-up default.

- **C-1 Identity/format.** P1 files use a distinct `format` value plus their own schema version. They reference the immutable P0 fixture by its file SHA-256 and snapshot hash. P0 bytes, hashes and events stay untouched.
- **C-2 Numeric contract.** Choose the authoritative type: binary32 for bit-faithful parity, or binary64 with a reviewed tolerance. Define serialisation round-trip, rounding of exports and display, and mm↔m/cm conversion rules (m-4, m-5).
- **C-3 Population.** Decide whether all living stems are in RD, CI and growth (the CCF rule) or only action-eligible adults. Either way, report both metric sets under separate names (§5.2).
- **C-4 Initial state assumptions.** Crown from DBH potential, all trees Mature, site productivity 1 (parity, per m-2), P0 `Dead` mapping (m-3), RNG model, seed and starting ecological year.
- **C-5 Site/top-height policy.** The CCF Class III curve is fixed. The GROWFOR SI is inferred or declared. State how a fixture whose heights sit below the Class III envelope is compared (m-8), and record the top-height definition as NOT VERIFIED until checked against the primary source.
- **C-6 Packet ownership.** Name the P1-01 oracle capture route and owner (Unity lane or a reviewed alternative). Schedule repair of the stale v23 assertion under separate authority. Settle C-2…C-5 before fixtures are written.
- **C-7 Labels.** Every P1 surface says "CCF adult rules, closed pure-Sitka population, not an Irish forecast". Any B/N/H output is labelled `BroadLynch2006_Sitka` model-to-model context, never accuracy.

## 8. Question 4: product authorisation

**None granted here.** The verdict permits only the separate four-file P1 DRAFT outlined in study §7 (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`), carrying C-1…C-7, for user and Manager review.

The owner still decides two things: whether a parallel standalone P1 experiment is worth doing alongside the Scenario One first-demo obligation, and whether a "faithful CCF adult rules, not field forecast" label meets its purpose. LAB-07 coding stays **NO-GO** until a DRAFT is approved.

## 9. Why P0, Scenario One and production state remain untouched

This review only read source. Its only tracked output is this file.

Not done:
- no change to `Assets/**`, `Tools/**`, `specs/**`, `Docs/Research/**`, `Docs/Project/**` (including the coordination board), the frozen Reference Future, or any save;
- no Unity Editor, compiler, lab host, browser, game build or Windows Player;
- no field-data download, organisation contact or #479 data use;
- no context or Drive mirror regeneration.

Done, outside the worktree:
- the Growfor tests ran from a scratch copy;
- fixtures were exported with `git show` into scratch.

The review worktree status stayed at 0 entries before this file was added.

**Dashboard/Drive sync: NOT VERIFIED.**

## 10. Scratch evidence (off-repo, not an archive)

`/media/jer/Files/CCF/Verification/MULTI-010-SitkaP1-EvidenceReview-20261114/`:

| File | SHA-256 |
|---|---|
| `scripts/independent_recompute.py` | `d61d2e23587070772d182414c9f7aa3bda5b3546616f11f1cc143f1909d45b39` |
| `results.json` | `64f61711e35758b792e2b96be6173d8de7cadfc13110137d7f4490843f9eb8f4` |
| `fixtures/oracle-quarter-ha.json` (from `2659db99`) | `b1691934fa4a41cfd63462f4021811e0871742f0e1f23ecea77fe8da40b5cf62` |
| `fixtures/sitka-flat-demo-v1.json` | `d0eed68979b2d214555e84a59e80ff6866ccd5e7d1774883e7898fcf8fb4f2ed` |
| `fixtures/sitka-flat-perf-2000-v1.json` | `d12db11126de2843ff21538f26feaab09242ccaf6abd04e3f25c8014054790df` |
| `growfor-copy/` | scratch copy of `Tools/Verification/Growfor` at `fc18190b`, used for the 24 + 18 test run |

Publication proof (GitHub readback and the one-added-file diff against `2212544e`) is reported in the #525 HANDED-OFF comment. This file cannot prove its own publication.
