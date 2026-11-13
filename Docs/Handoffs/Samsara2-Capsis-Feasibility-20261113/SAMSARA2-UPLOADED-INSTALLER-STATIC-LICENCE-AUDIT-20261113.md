# Samsara2 installer and martelosim source — static feasibility audit

**Date:** 13 November 2026  
**Manager decision:** Browser/static examination only. **#456 remains WAITING / NOT RUNNING. Not a release.**  
**Evidence:** direct read-only inspection of user-uploaded installer, plus public martelosim documentation and package metadata. No software installation, Java/R/CAPSIS execution, game modification, local CCF worktree, Unity lease or test claim.  
**Source location:** user-supplied conversation attachment named `capsis-Samsara2_4.2.7-22961_setup.jar`; binary is **not** published or redistributed in GitHub. The previous research and conditional packet are in this directory.

## 1. Exact installer fingerprint

- File name: `capsis-Samsara2_4.2.7-22961_setup.jar`.
- File size: **194,797,004 bytes** (approximately **185.8 MiB**).
- SHA-256: `cca418b217fc0e36f828990b32d79f27712e0193f2565754883d9aaabd871a13`.
- Java IzPack installer ZIP: **374 ZIP entries**; ZIP CRC test found no corrupt entry.
- Included main packs `packs/pack-Base` and `packs/pack-Data`. Static archive-reading of the serialized IzPack pack entries detected Samsara2 class/source/data file paths.
- **Limits:** ZIP integrity proves internal checksum consistency only. It is not a vendor-signature or malware verdict; binary origin/signature were not independently checked. Installer not executed.

## 2. Licence — material new fact

Both `$INSTALL_PATH/src/samsara2/license_en.txt` and `$INSTALL_PATH/class/samsara2/license_en.txt` are present in `packs/pack-Base`. Their actual embedded text was inspected (not inferred from filename). The Samsara2 model's notice identifies **Benoit Courbaud, 2026** and grants reuse and modification under the **GNU General Public License, either version 3 or (at recipient's option) any later version (GPL-3.0-or-later)**. The corresponding French files are also present.

**Disposition:** previous research correctly cautioned that CAPSIS core LGPL and martelosim MIT cannot substitute for a module-specific licence. This specific uploaded Samsara2 release **does carry an explicit module licence**. The lack-of-licence Gate 1 blocker is therefore resolved *for the Samsara2 module included in this exact installer*. Using, studying and privately running a GPL-licensed binary for independent research does not require an additional proprietary module licence, subject to ordinary GPL terms.

**Important boundaries:**
- GPL terms do **not** mean any code can be pasted into CCF's production Unity project without analysing distribution obligations, attribution, derivative/combined-work issues, and licences of **other** bundled CAPSIS components or datasets.
- A GPL notice is not a binary authenticity certificate. Confirm an official download/source and hash if chain of custody matters.
- This is a technical licence reading, not a formal legal opinion.

## 3. Actual software contents (presence only)

The base pack exposes `$INSTALL_PATH/class/samsara2/model/Samsa2Model.class` and a parallel `src/samsara2` tree, plus licence files. Static file path strings additionally show:

- `samsara2/pgms/flexiblescript/ScriptFlexibleSimuFormat2.class` and `CommandFileReaderForScriptFlexibleSimuFormat2.class`.
- Specific command-line parser classes for `MultiCriteriaThinningLine`, `RectangularGapThinnerLine`, `RectangularGapTargetGThinnerLine`, `TargetDensityThinningLine`, `MarteloscopeFieldMarkingLine`, `SaplingPlantationInCellsLine`, `SaplingPlantationInCirclesLine`, `DisturbanceLine`, `SimulationPartLine` and others.
- Matching intervener classes including `MultiCriteriaThinner`, rectangular gap and target-density thinning.
- Data pack example `data/samsara2/DemoFiles/Prelenfrey/` with inventory, species, climate, command and marking/silviculture files.

**Interpretation:** scientific script support and a richer manipulation vocabulary than the R workshop front end are supported by **module/class existence**. Correct public APIs, example command syntax, successful execution, suitability to CCF, and reproducibility **remain unverified**. Do not equate class presence with operational functionality.

## 4. User-supplied upstream source link

**https://forge.inrae.fr/lessem/rpackages/martelosim** is the GitLab upstream repository for the **martelosim R wrapper**, *not* the Samsara2 Java source repository. Its independently reachable documentation and metadata:

- Package website: https://martelosim.lessem.forge.inrae.fr/
- Quick guide: https://martelosim.lessem.forge.inrae.fr/articles/martelosim.html
- API index: https://martelosim.lessem.forge.inrae.fr/reference/index.html
- `write_samsara_cmd()`: https://martelosim.lessem.forge.inrae.fr/reference/write_samsara_cmd.html
- `run_samsara()`: https://martelosim.lessem.forge.inrae.fr/reference/run_samsara.html
- Published R-universe manual: https://lessem-inrae.r-universe.dev/martelosim/doc/manual.html
- Licence: https://martelosim.lessem.forge.inrae.fr/LICENSE.html (MIT).
- Stable installation according to the package website: `install.packages('martelosim', repos=c('https://inrae.r-universe.dev','https://cloud.r-project.org'))`.
- Development branch distribution is published on upstream project as `/-/archive/dev/martelosim-dev.zip` with an explicit warning that APIs can change.
- Public R-universe package metadata reports version **0.1.2**, built **4 October 2026**.
- Direct GitLab source/raw page was **not retrievable with the browser tools in this review**. The R interface facts below are based on the official generated API documentation rather than claiming a full GitLab source-code audit.

### Documented relevant R entry points

| Function | Role / limitation |
|---|---|
| `new_inventory` and `build_samsa2_files` | Inventory and conversion to Samsara2 text input |
| `prepare_samsara` | Generate edge/periphery and virtual saplings; default `memory_perc=75` is configured RAM percentage, **not measured RAM usage** |
| `write_samsara_cmd` | Specify thinning interval, total projection years, 1-year simulation step, `silviculture=auto/expert`; marking group handling marked under development |
| `run_samsara` | Invoke the external model with generated command file; `combineOutputs` option documented as unused |
| `martelo_report` | Generate PDF results; intended for marteloscope marking workshops |

The reference workshop's automatic follow-up interventions derive a strategy from initial field marks. **This is not an independent validation of CCF's repeat-plan, rack access, Work Plan or spatial treatment rules.**

## 5. **Important release-specific version gap**

The published guide explicitly requires CAPSIS **4.2.7-22711** for the stable martelosim, whereas the uploaded package is **4.2.7-22961**. It is newer by build number but may have changed command grammar, script classpaths, output formats or other interfaces.

**Gate for any later local experiment:** before attempting a combined martelosim/Samsara2 run, check the exact installed revision and package compatibility. Prefer *direct* example command execution using the newer Samsara2 flexible script only if documented, supported and separately authorised; do not install the old build automatically or silently edit R source. Compare documented interface against embedded examples and report differences rather than altering production CCF.

## 6. Recommended research lane if Manager releases #456 later

1. Verify checksum against official INRAE/CAPSIS publication if available; document any provenance uncertainty and review other applicable module/data licences. No step implies binary safety verification.
2. Check actual official Java 8 prerequisite and valid install path under isolated low-impact research directory. Do not install yet: this is a future permitted Gate 1.
3. Read the embedded Prelenfrey `CommandFile` and corresponding R source/API for command schema. Determine whether 22961 accepts the older wrapper's generated commands. The broader flexible-script command reader is a candidate for a later independent batch workflow.
4. With an approved local lane and no Objective 1 resource contention, run an authentic no-treatment vs multiple thinning treatment test, recording model version, seeds (if supported), timing/memory measured from the run, source/parameter assumptions, and underlying data exports.
5. Treat outputs as **Alpine forest-model comparisons**; do not import their numbers into the Irish Sitka/Oak/Beech simulation, modify CCF source, approve a new spec, or replace protected saves.

**Current verdict:** **STATIC SOURCE + LICENCE AUDIT COMPLETE. PROGRAM EXECUTION NOT STARTED.** Access concern has substantially narrowed from “module licence unknown” to “version-specific compatibility and runtime behaviour unknown”. The broader script classes make a direct native Samsara2 experimental interface worth examining. Objective 1 remains first; issue #456 remains WAITING pending explicit Manager scheduling/ownership release.

**Dashboard/Drive sync:** not verified; no canonical context authority sources or live coordination updated.
