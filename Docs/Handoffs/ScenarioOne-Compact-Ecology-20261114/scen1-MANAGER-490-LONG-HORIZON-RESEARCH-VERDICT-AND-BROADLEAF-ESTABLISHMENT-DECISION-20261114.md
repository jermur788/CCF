# scen1 — Manager #424 verdict on #490 Year-40 research and broadleaf establishment decision

**Date:** 14 November 2026 UTC. **Manager/reviewer:** OpenAI GPT-6 in connected GitHub source/review lane.  
**Task:** [#490](https://github.com/jermur788/CCF/issues/490) HANDED-OFF / CLOSED, actual local worker **Claude Opus 5.5**.  
**Verdict:** **APPROVED — BOUNDED NATIVE RESEARCH EVIDENCE ONLY.** The claimed true annual 40-year test and exact old native fixture replay are supported by the published result and handoff, but this is not product acceptance, standalone user validation, independent re-execution or an integrated release candidate.  
**Owner decision gate:** should the proposed first compact tutorial rely on **player-led planting and protecting Oak/Beech** as the broadleaf establishment route, without guaranteeing natural Oak/Beech seedlings before Year40; or must *natural broadleaf recruitment during the demo* be a required learning outcome, implying a separately justified donor/seed-policy design? The owner has not explicitly selected either. **Manager recommends player-led planting as the baseline**, with natural regeneration contingent on real modelled donor availability and external hedgerow sources *deferred*, not silently turned on. The open question concerns the **teaching requirement**, not a belief that a small Irish standalone woodland is unrealistic.  
**Naming:** new Scenario One handoff filename and visible title start `scen1`. No old published paths renamed.  
**Dashboard/Drive:** not verified.

## 1. Evidence actually reviewed

Direct-read GitHub:
- [#490 40-year report](https://github.com/jermur788/CCF/blob/research/scen1-three-species-long-horizon-native-20261114-v1/Docs/Research/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-RESULT-20261114.md).
- [#490 40-year handoff](https://github.com/jermur788/CCF/blob/research/scen1-three-species-long-horizon-native-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-HANDOFF-20261114.md).
- [#490 issue registration and closure](https://github.com/jermur788/CCF/issues/490), report of original #460 raw data gates, Unity isolation, screenshots and lease release.
- Current main source `Assets/ForestPrototype/ForestEcologyController.cs` Hegyi/annual density mortality, `Assets/ForestPrototype/ForestTree.cs:547–563` biological death representation, `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` genuine Work Plan/year advancement, and `ScenarioOneManager.History.cs` annual/history source. This source check independently confirms code is capable of the reported paths, not the specific native run or visuals.

Compare the result branch `research/scen1-three-species-long-horizon-native-20261114-v1` with its registered original main start: exactly **two new `scen1` Markdown files; no existing tracked file changes**, safe scope at GitHub level. Worker reports running Unity 6000.6.0f1, exit 0, **142/142 fixture checks PASS**, same 189 Year0 Sitka IDs and matching #460 Arm B early-state trajectory through Year8, both arms 40 genuine individual years, old user/protected worktrees and raw #460 evidence unchanged, temporary CCF-Ops lane restored byte-identical, lease normally released. Worker moved the Unity worktree from planned rotational HDD to ZX20 SSD and reported before running, consistent with active-library placement policy. **Manager did not access the physical host, replay 142 native checks, independently inspect the external PNG pixels, check current physical lease, or run historical save/Reference hashes.** Do not describe any of those as Manager PASS.

## 2. What passed, what did not

| Domain | Research observation and certainty | Disposition |
| --- | --- | --- |
| 30m finite source-generated stand through 40 genuine annual ticks, age20 initial Sitka | Worker-executed unity with exact source/fixture early replay and 142 checks | **PASS for bounded research** |
| Oak and Beech establishment | Year2 planted juveniles (one each) alive/promoted Year10/11 and age42 trees in Year40; Oak 12.28cm DBH /18.10m, Beech 15cm/19.40m in baseline | **Observed in one deterministic case**, not general survival forecast |
| Oak/Beech natural seed recruitment | None through Year40; no adult broadleaf donor initially or outside; both plants began very small seed potential only in Year40 | **Expected for given finite initial conditions**, not proof Irish hedgerows cannot supply seed |
| Sitka natural regeneration | 146/143 abstract natural cohort bands at Year40, plus a few promoted Sitka stems; bands are **not individual seedling stems/ha** | **Observed** |
| Untreated/released crop tree difference | Two Year5 Sitka competitors removed in B; at Year40 P0000 +2.69cm and P1504 +1.34cm DBH relative to A, €2,463 less cash | **Observed conditional response**, not necessarily compelling to novice or proof of optimal thinning |
| Sitka mortality | After shared access, 170/168 original Sitka; Year40 originals 64/68 (living counts include 4/2 promoted Sitka recruits) | **Observed large attrition**; cause by individual not measured |
| 3D evidence | Worker reports actual 720p/1080p Year20/40 captures. Outside view Year40 hides both broadleaves; Arm A eye-level legible, Arm B partly occluded; optional Arm B Year0 image defective | **Worker-described visuals; not independently opened by Manager** |
| Historical save/Reference regression, integrated S1-C/S1-D, two-run save, new compact geometry, human playtime, Windows build | Not run or not built | **NOT VERIFIED/NOT APPROVED** |

## 3. Two distinct product risks exposed

### R1 — broadleaf teaching cannot depend on two planted individuals alone

One Oak and one Beech alive after 40 years establishes that the **code can grow both**, not that the player has a resilient or educationally useful woodland-conversion strategy. If either individual is lost the species story disappears; if both are hidden among Sitka from a typical camera, their change becomes hard to observe. This may be an acceptable *small research witness*, but is a weak **complete multi-year beginner demo**.

**Manager design recommendation, not accepted rule:** use a limited, intentionally selected **small number of real Oak/Beech introductions over management cycles** in suitable opportunities, with actual placement, protection, browsing, local light and cost consequences. Choose count and locations only after inspecting available management area and stand dynamics; do not prescribe arbitrary hectares ratios or guarantee all survive. Consider another valid design where an existing small young broadleaf cohort is part of the authored history (would change starting-stand story and require owner choice). No fake outside donors and no visual-only seedlings used as numerical evidence.

If the desired educational endpoint is **natural Oak/Beech regeneration before Year40**, the current model/starting stand cannot promise it: a real age-eligible internal donor, truthful external species donor with seed-input and survival policy, or a longer horizon would need a **SPEC REQUIRED** decision. A generic hedgerow does not automatically contain fertile Oak/Beech; it may carry other species instead. Any source-based external mechanism needs source identity, spatial relation, seed abundance/mast, year, recruitment conditions and saved provenance; decorative hedge meshes alone do not count.

### R2 — management consequence and forest-health representation need study before first demo

- From initial 170/168 post-access adult Sitka, 106/100 originals disappear by Year40 even without repeated timber operations. Current main `ForestEcologyController.ApplyAdultDensityMortality` calls `ForestTree.ApplyMortality("self-thinning",year)`, and an **aggregate** `AdultMortalityAccount` tracks annual deaths/biological volume. But `ForestTree.ApplyMortality` disables the GameObject and `ForestEcologyController.OnTreeMortality` only invalidates cached competition/canopy/seed, **without a per-tree standing/fallen deadwood resolver**. The worker **did not directly instrument per-tree causes**, so do not relabel every measured loss as an observed tree-specific self-thinning record. This is a real long-horizon presentation/provenance limitation, not automatically a safe one-line bug fix (history/save rules are affected).
- At Year40 the Oak's **0.45m crown radius at 18.1m height** is a source model output; strongly suppressed broadleaves can show narrow crowns, but this very slender geometry deserves an **ecological and visual calibration check**, not automatic enlargement or fabricated realism. Unity selects a 'Mature' interaction/render stage; 'mature-looking' mesh is **not proof of biological crown growth, seed source or a healthy adult**.
- Only **one modest Year5 release**, focused on selected Sitka crop trees, was studied. The response was a small diameter improvement and nearly no difference for the planted broadleaves (Oak 12.28 vs 12.22 cm Year40). This does **not qualify** the owner-proposed **three substantive management reassessments** or a compelling same-start guided/independent comparison. A different genuine intervention near broadleaves could have different results, but **no outcome is proven**.
- The scenario's raw bramble/bracken outputs were exported but **not analysed**; no claim over ground competition's contribution to broadleaf outcomes. No windthrow was enabled and no exterior seed sources modelled; note these distinct limits.

**Prioritisation:** the first external-demo risk is **educational legibility of differentiated decisions and true annual biological history**, not absence of free external broadleaf seed alone. Do not add a seed mechanism as a cosmetic shortcut while the 40-year mixed-stand history mostly looks like Sitka and two broadleaves are hard to see.

## 4. Product options, recommendation and gating

**A — First demo teaches *assisted broadleaf conversion*** (**Manager preference, requires owner confirmation**). The originally all-Sitka small, physically bounded Irish woodland is credible, perhaps surrounded by fields/roads/hedges. The player deliberately plants, protects, inspects and revisits Oak/Beech cohorts across multiple genuine review opportunities; successful regeneration is not scripted, nor is adult natural seeding required by Year40. Continue to support naturally regenerating Sitka and honest unknowns about possible external seed sources. Extra planting and meaningful canopy treatment need an approved compact four-file spec, area/budget/asset qualification and no reliance on exact forced marks.

**B — First demo must show *natural Oak/Beech seedling recruitment*** (separate product goal). Then use an explicitly owned, source-grounded donor strategy (real internal mature species trees or actual external species-specific donors/hedges if grounded), with probabilistic establishment/protection, and source/provenance/truthful non-guarantee. This is **spec-required**, potentially changes initial stand, forest geometry, management timeline and save identity, and must be independently reviewed. A 40-year horizon may still not guarantee recruits.

**Recommendation:** use **A** for the earliest complete-loop experience; if the user wants natural broadleaf regeneration visible by the end, choose B knowingly and plan the additional work. Don't create a new external-hedgerow-spec without a settled requirement. D-059 three-species comparable modelling depth and D-060 complete first external Scenario One remain; initial stand/curriculum, 30m world size, multiple plantings and year40 endpoint are still proposals, not canon.

## 5. Manager next work once owner confirms establishment objective

If A is accepted:
1. Revise the compact same-start-guided vs independent **four-file DRAFT** and design the three non-compulsory review opportunities with actual planting/protection, strategic release/crop selection, annual results and a comparison that does not grade against an alleged perfect guided answer.
2. Make narrow, checkable **separate** biology/visual-quality investigations before implementation: near-broadleaf canopy decision effect, very narrow broadleaf crown under the current model, self-thinning outcome/provenance/habitat effects, and any artistic stage mismatch. Avoid another broad 3×2×4×N factorial study. Produce only evidence directly needed to choose robust tutorial placement and scale.
3. One safe shared save/router design for two independent histories and old-world load rejection **before** any source edits. New compact geometry/saves need approved spec and one writer, preserving current five-rack/Legacy40/Reference Future v1.
4. Schedule local source/Unity research only when bounded, exact context is known and a real exclusive local Editor lease is available. No Windows build until owner personal candidate playtest plus explicit authorisation.

If B is accepted: first write a source-grounded **external/internal broadleaf donor feasibility proposal** with exact species/date/site/provenance, negative controls and no forced natural regeneration; independently review before any DRAFT production spec. Do not introduce non-Sitka donors under an unnamed 'hedgerow' setting.

**No new worker claim** is created by this verdict: the next step is genuinely blocked on the owner's product objective, not more unauthorised testing. #490 remains HANDED-OFF/CLOSED; future work routes through #424.  
**User action needed:** answer whether A planting-led conversion (no natural Oak/Beech recruits promised by Year40) or B natural broadleaf regeneration required in first demo.  
**Dashboard/Drive:** not verified.
