# Samsara2 / CAPSIS as a scientific foundation for CCF — research feasibility handoff

**Date:** 13 November 2026  
**Task claim:** [#455](https://github.com/jermur788/CCF/issues/455)  
**Status:** RESEARCH HANDED OFF — NON-AUTHORITATIVE; NO PRODUCTION ADOPTION DECISION  
**Worker:** OpenAI GPT-6, browser/GitHub documentation lane (no local worktree, Unity process or CAPSIS executable)  
**Scope:** source and scientific literature review, CCF architecture fit, licences and bounded offline experiment recommendation.  
**Out of scope:** changes to production simulation, C# / Unity / scenes, persisted worlds, accepted specifications, D-073 personal test candidate, Objective 1/2 work, live coordination, Windows builds, or automatic conversion of research into gameplay coefficients.

## Executive verdict

**GO for Samsara2 as an offline comparison/benchmark and source of scientific modelling ideas. NO-GO for using unverified Samsara2 binaries/code or coefficients as CCF's direct in-game simulator at present.** Reevaluate only after a small external executable test, checking licence rights and geographic/species calibration. `martelosim` is an R-based workshop front end around Samsara2, not the growth engine. The main attraction is a spatial individual-tree dynamic system with tree growth, crown/light competition, mortality, seed dispersal, regeneration and repeated selective interventions.

**Recommendation is a modelling inference, not accepted CCF product intent.** It does not replace D-059 parity, D-061 density, D-065 multi-scale management, annual CCF ecology, or frozen Reference Future.

## 1. Verified publicly documented mechanics and workflow

1. **Software lineage:** CAPSIS (Java forest-simulation platform) hosts Samsara2 (spatial individual-tree forest-dynamics model); INRAE's `martelosim` R package transforms marteloscope inventory/marking data into Samsara2 input/commands and standardized reports. Official package: https://martelosim.lessem.forge.inrae.fr/ ; guide: https://martelosim.lessem.forge.inrae.fr/articles/martelosim.html ; CAPSIS projects: https://capsis.cirad.fr/capsis/models .
2. **Forest model:** individual trees have species, DBH, crown dimensions, spatial 3D position; model describes light reaching crowns and ground, seed dispersal, juvenile dynamics, growth and mortality; a ground-cell component interacts with individual trees. Peer-reviewed Courbaud et al. 2015: https://www.sciencedirect.com/science/article/pii/S0304380015002884 ; freely available manuscript text: https://ecology.ghislainv.fr/publications/Courbaud2015-EM.pdf .
3. **Temporal and computational detail:** the 2015 model description uses annual demographic steps, often 5-year recomputation of expensive light; typical 1–4-ha research plots and 5×5-m ground cells; model includes stochastic transitions. These are descriptions of published experiments, not performance guarantees for the current release or CCF. The 2015 paper notes substantial compute cost; the martelosim guide gives one *illustrative* roughly 1-minute/20-year exercise, not a benchmark for the CCF candidate.
4. **Martelosim workflow:** XLSX inventory template, optional supplemental/virtual periphery and saplings, spreadsheet or I+Trainer marking (Cut, Wait, Promote, Retain), `build_samsa2_files()`, `write_samsara_cmd()`, `run_samsara()`, then `martelo_report()`. See full guide and API: https://martelosim.lessem.forge.inrae.fr/reference/index.html .
5. **Command-file controls documented:** `thinning_time`, `evolution_time`, `simulation_year_step` (default 1), `silviculture = auto/expert`, and optional regenerated periphery/saplings. The “auto” workflow derives subsequent thinning strategy from initial marks, not necessarily a literal player-authored repeat schedule. API: https://martelosim.lessem.forge.inrae.fr/reference/write_samsara_cmd.html .
6. **More flexible automation:** general CAPSIS script mode supports Java or Groovy for module-specific experiments, if that model implements requisite classes. It is not evidence that arbitrary Samsara2 scenarios can be fully specified via the narrow martelosim text-command interface: https://capsis.cirad.fr/capsis/documentation/script_mode .
7. **Data and limitations:** inventory expects per-tree location, DBH, and compatible species/attributes; missing Z can be approximated using terrain; missing saplings/peripheral trees can be generated. Such synthetic additions must be tagged and **cannot** be misrepresented as observed CCF state.
8. **Calibration:** Courbaud et al.'s core quantitative application used mainly uneven-aged Norway spruce/silver fir forests in the northern French Alps, with other sources for seedling processes, not Irish Sitka/Oak/Beech. The later Beech demonstration sometimes borrows fir parameters, highlighting the need for species-specific evidence: https://pmc.ncbi.nlm.nih.gov/articles/PMC4801349/ .

## 2. Licensing and distribution gate

Official CAPSIS FAQ (https://capsis.cirad.fr/capsis/documentation/faq): **CAPSIS core LGPL**, individual forestry-model modules on **module-specific licences**; absent module licence means not freely distributable. The **martelosim wrapper is MIT** (https://martelosim.lessem.forge.inrae.fr/), which does **not** confer MIT rights over the Samsara2 module or associated data. The present research did not inspect a Samsara2 module licence file or obtain a licence grant. Consequently:

- **Potentially permissible with current evidence:** reading publications; using a legitimately obtained distributable installation within its terms; independent model comparison; referencing scientific methods with correct attribution.
- **NOT yet authorised:** importing Samsara2 Java classes into Unity, bundling its binaries, copying module source, redistributing its coefficients/data as a game library, or asserting proprietary/commercial/open-source rights.
- **Resolution:** inspect the exact module `LICENSE` distributed with the approved Samsara2 build and, if not explicit, seek permission from the module author/custodian before any source adaptation, embed, sharing or redistribution. Contact listed in the official CAPSIS models catalogue (Benoit Courbaud/INRAE).

## 3. CCF fit and incompatibilities

| CCF subsystem/contract | Samsara2 overlap | Decision implication |
|---|---|---|
| Individual tree growth/diameter | Individual-tree light-driven growth | Strong **method** comparison, numerical transfer unproven |
| Crown/ground light and Hegyi/local competition | Detailed 3D interception; potentially different driver | Benchmark light/release outcomes, do not silently replace CCF competition |
| Seed, juvenile recruitment, local gaps | Spatial seed and ground-light response | Mechanism comparison, not Irish establishment-rate calibration |
| Mortality | Modelled individual mortality | Compare qualitatively with existing protection/storm/annual contracts |
| Repeated thinning | Scripted and automatic thinning | Research analogue for repeated interventions, not CCF's player approval sequence |
| Pure Sitka, Oak/Beech parity | Original fit mainly Alpine spruce/fir, only partial broadleaf transfer | Species-specific calibration remains necessary; no generic replacement |
| Irish browse, understorey, wind, economic and geography | Some aggregate mechanisms, not the accepted CCF full behaviour | Must retain CCF authoritative state and provenance |
| Work Plan, rack planning, MFM, history, Unity player loop | No equivalent gameplay contract evidenced | Keep CCF game architecture |
| Save, Legacy40, frozen Reference Future v1 | Separate model, independent states | No production changes without a new approved compatibility spec |

**Existing CCF authority:** The Game Brief requires a 3D inspect→plan→resolve→advance→review→walk loop and annual ecology. Current approved programme preserves exact pure-Sitka anchors, D-059 Sitka/Oak/Beech parity and ordinal mixed-density rules, work-plan commitment and protected saves. The current production code already has a spatial ecology and regeneration path. Samsara2's value is therefore best viewed as scientific comparison and design analysis, not a replacement of a non-existent simulation subsystem.

**Scientific credibility:** Independent 2015 paper assesses qualitative/quantitative evaluation and sensitivity analysis; published validity does not automatically transfer across species, climate, site, management regime, or model version. The `martelosim` guide itself warns against overinterpreting precise forecasts for a single field site and recommends inspecting relationships and trends.

## 4. Option assessment

- **A. Embed Samsara2 as Unity's runtime engine:** **NO-GO now.** Cross-language Java/R coupling, module licence, CPU/time, deterministic/save identity, data transfer and non-Irish calibration all unresolved. This requires a new substantial accepted spec and native verification if ever reconsidered.
- **B. Run standalone Samsara2 beside CCF for comparable experimental stands:** **RECOMMENDED RESEARCH OPTION.** Clean separation; compare directions and plausible response envelopes without trusting transplanted coefficients. May use the existing Prelenfrey inventory first, then only compatible synthetic/CCF-derived data with honest species equivalence labels.
- **C. Learn from published model architecture and implement verified mechanisms in native C#:** **CONDITIONALLY PROMISING.** No literal code copy or unlicensed coefficients; design/spec plus scientific calibration and frozen-candidate protection before production work.
- **D. Replace CCF with `martelosim`:** **NO-GO.** A workshop-oriented R report generator is not a realtime Unity simulation/game engine, nor an unrestricted scenario editor.

## 5. Proposed bounded offline experiment (not started)

**Purpose:** determine whether a legally obtained, version-pinned Samsara2 installation can reproducibly run and export useful forestry comparisons. First demonstrate a native Prelenfrey sample: inventory→marking→command→simulation→report. No Unity, CCF source writes, protected game saves or current objective code changes.

**Phase P0 — access/legal/software preflight:** identify exact Samsara2 version and module licence; obtain allowed installer; check Java/R/CAPSIS requirements and environment; record module source/data rights; verify module's generalized script capability rather than assuming it. Fail CLOSED on rights or installer uncertainty.

**Phase P1 — reproducible baseline:** run documented Prelenfrey example with fixed inventory, explicit commands, version details and repeat random seeds if accessible; capture outputs, logs, elapsed wall time, peak memory (if observable), and evidence of re-runs. Record whether `martelosim`'s multi-group API limitations matter.

**Phase P2 — controlled interventions:** in a throwaway inventory with independently documented input state, compare (a) no harvest, (b) local competitor-release marking, (c) broader thinning and (d) a repeated-intervention scenario only if script interfaces actually support it. Preserve the same starting stand; compare diameter/BA/stocking, relative light and regeneration direction at selected years. If model's native species differ from Sitka/Oak/Beech, call them **Alpine reference demonstrations**, not Irish-stand validation.

**Phase P3 — compatibility assessment:** document required input/output schema, allowed spatial boundaries, missing-tree assumptions, seed/reproducibility controls, runtime scalability, module version, and ability to operate headless under Linux. Provide a grounded GO / NO-GO verdict specifically for **offline comparison**, not a production integration verdict.

**Stop conditions:** absent module licence/right to use, absent supported binary, implementation requiring unauthorized redistribution/source copying, inability to reproduce the example, unclear input species mappings, or nontrivial interference with Objective 1/2 lanes.

**Acceptance criteria:** two repeatable runs, one meaningful control/treatment comparison, exact tooling/version/seed if available, output manifest, model-applicability limits, observed performance and source of every technical claim. No requirement to reproduce pure-Sitka CCF numbers with Alpine species. Never weaken CCF tests to force agreement.

## 6. Further research questions

1. What exactly does the installed Samsara2 module licence permit for *research*, *publication*, *derivative code* and *distribution*?
2. Can the particular Samsara2 build be called noninteractively on Linux without manual RStudio actions?
3. How are scripts, fixed random seeds, export tables and multi-cycle saved state supported?
4. Which species/parameters are actually included in that binary, and which are demonstrably calibrated?
5. Can it produce comparable canopy/regeneration spatial diagnostics without inventing Irish coefficients?
6. Can the separate Irish GROWFOR evidence provide more defensible *pure-Sitka aggregate* validation, while Samsara2 informs *spatial process* plausibility?

## 7. Delivery/status

- **Delivered:** source-reviewed research assessment, candid licensing/calibration/technical gates and bounded experimental protocol.
- **Not done:** executable installation; licence file inspection; live measurements; CCF source diff; performance/Unity/native test; accepted engine decision.
- **Programme impact:** no milestone or accepted feature scope change; Objective 1 single-writer and Objective 2 unresolved DRAFT gates remain higher priority.
- **User action:** no action for this desk research. An optional separate local experiment requires deliberate scheduling in an isolated non-Unity lane.
- **Dashboard/Drive mirror:** not synced or verified; no canonical context sources changed.

## Sources (independent public documentation)

- Martelosim official guide: https://martelosim.lessem.forge.inrae.fr/articles/martelosim.html
- Martelosim API commands: https://martelosim.lessem.forge.inrae.fr/reference/write_samsara_cmd.html
- Martelosim API execution: https://martelosim.lessem.forge.inrae.fr/reference/run_samsara.html
- Martelosim license: https://martelosim.lessem.forge.inrae.fr/
- CAPSIS model inventory: https://capsis.cirad.fr/capsis/models
- CAPSIS script mode: https://capsis.cirad.fr/capsis/documentation/script_mode
- CAPSIS FAQ (module licensing): https://capsis.cirad.fr/capsis/documentation/faq
- Courbaud et al., Ecological Modelling 314 (2015): https://www.sciencedirect.com/science/article/pii/S0304380015002884
- Beech/light param transfer example: https://pmc.ncbi.nlm.nih.gov/articles/PMC4801349/
