> **DRAFT / SECONDHAND FIGURES / PRIMARY SOURCE NOT VERIFIED / CONFLICTS OPEN.** Provisional docs-only record under the Manager decision D1 in `CCF-860-MANAGER-PUBLICATION-DECISIONS-AND-INDEPENDENT-CLOUD-REVIEW-PACKET-20261120.md`. Not canon, not reviewed, not merged. Desktop writer: Claude Sonnet 5.5. No PDFs or ZIPs are in this branch.

# CCF New-Reports Evidence Addendum (2026-11-20) — DRAFT derivative pack

**Status:** supporting research derivative, not decision authority, not canon. Selective, **not lossless**. Originals unchanged in the desktop archive and Drive.
**Proposed use:** Roster A slot 12. Not yet reviewed by the owner or Manager. Not yet attached anywhere.

## How to read this
- Source IDs are the PDF filenames below. "p." is the PDF page I counted from the extracted text (each report prints its own page number at the foot; they matched in the files I read). Page ranges are to the section, not the sentence.
- **None of the five reports cites an original paper page.** Their footnote markers (for example "39L401-L408") are retrieval-tool pointers into web pages, not page numbers. So every figure below is **second-hand**: "as reported by <report>", not checked against the primary source. Evidence-type labels are the report's own, except where I say otherwise.
- Overlap check: I searched the four composites for these topics. They do not contain the Teagasc "Francepark" thinning-trial figures, the Dowd, Roslin or Dartmoor datasets, or the marteloscope dataset audit. Their only nurse-crop and marteloscope mentions are general (Ecology composite line 390: no hard-coded "birch gives oak +15%" nurse bonus; Practice composite lines 152 and 296). Nothing here contradicts them; the nurse brief is consistent with the Ecology composite.

## Source register
| ID | File | PDF pages | Nature |
|---|---|---|---|
| S853 | Scenario One #853 -- Deep Research Conclusion.pdf | 13 | AI deep-research report; secondary web sources |
| SCALE | Scaling Continuous-Cover Forestry (CCF) Simulation.pdf | 8 | AI report; says no repository code was fetchable |
| MAP2D | Implementation Options for a C#-First 2D Map.pdf | 10 | AI report; framework scoring |
| MART | Deep Research -- Marteloscope datasets as a foundation...pdf | 22 | AI report; most thorough of the five |
| TREMS | Marteloscope (TreMs) dataset contents.pdf | 3 | AI report |
| SITES | Marteloscope sites and datasets (Future scenario candidates).pdf | 2 | AI report |
| E1 | Independent Sitka_Conifer Data Sources (E1) (1).pdf | 5 | AI report; dataset audit |
| MULTI | Multi-Area Sitka Spruce Data Assessment.pdf | 4 | AI report; dataset audit |
| NURSE | CCF_Instant_New_CCF_Nurse_Crop_Research_Summary.pdf | 2 | Short brief dated 12 Nov 2026 |

## A. Scenario One forestry consequences (S853) — applies to Scenario One only, not Multi/G1
| Claim (as reported) | Source, page | Evidence type | Caveat |
|---|---|---|---|
| Report finds no evidence-based blocker to the Compact30 demo proceeding; main risks are communication and calibration | S853 p.1 | inference | Based on #490 results "as reported", not re-run (p.12) |
| Teagasc thinning trial: at ~29 y heavy-thinned stand ~0.85 m3 mean tree volume vs 0.44 m3 unthinned; ~616 vs 1762 stems/ha; ~521 vs ~780 m3/ha | S853 pp.3-4, 9-11 | field measured (report's [A]) | Sitka plantation, hectare scale. Primary Teagasc page not in the archive. Rotation lengths quoted inconsistently (27 y on p.9) |
| Thinning income in that trial ~2.7-4.5 k EUR/ha over several thinnings | S853 pp.1, 8, 11 | field/guidance | Quoted as 4.5 k on p.1 and 4.6 k on p.11. Scaled to 0.09 ha gives a few hundred EUR |
| Fixed contractor costs (500-1000 EUR per visit, report's estimate) can exceed tiny-stand thinning income | S853 p.11 | inference [C] | No citation given for the 500-1000 figure |
| Oak/beech planted unprotected are likely lost to deer; natural oak under dense Sitka is rare without a seed source or gaps | S853 pp.2, 5-6, 10 | guidance [B] | Irish field-study claim comes via Teagasc CCF guidance; "Kelly 2002, O'Halloran" named but not retrieved (p.12) |
| CCF afforestation grant ~5,421 EUR/ha plus 912 EUR/ha/yr; game planting charge (~196 EUR for 4 trees) is not realistic | S853 pp.10-11 | guidance | Source is a ProSilva Ireland page the report cites; figures unchecked |
| Deadwood from thinning/mortality must be confirmed as counted with the normal manager (bug #496) | S853 pp.8, 11 | repository-verified result per #490 [D] | A test proposal, not a finding |
| Five proposed checks; only 1-3 (deadwood accounting, thinned vs unthinned crop trees, protected vs unprotected oak/beech) are called critical before playtest | S853 pp.11-12 | game tuning / verification plan | No code was examined by the report |

## B. Simulation engine scaling (SCALE) — hypotheses, none measured
- For fixed-radius neighbour search the report favours a uniform grid or spatial hash (O(n) build, roughly O(1) query per tree) over brute force at 10k+ trees; k-d tree, quadtree, R-tree and Morton ordering are weighed as alternatives. SCALE pp.1-2, 5. Evidence type: inference from general computing references.
- Parallel scaling must preserve determinism: snapshot-and-barrier phases, stable ordering, explicit per-tree RNG seeds, and a warning that parallel float sums can break bitwise reproducibility. SCALE pp.3-4. Inference.
- Eight-gate roadmap and a regression matrix (radius boundary, partition seams, summation order, RNG order, save/load). SCALE pp.6-7. Inference.
- **Limits:** the report states repository details are UNKNOWN and assumes a Hegyi-type radius index (p.1). Memory and timing numbers (e.g. 32 B per tree, ~9M comparisons per year at 1M trees) are the report's own estimates, unmeasured (p.5). The 1M-tree gate of "<1 GB, few minutes per year" is the report's proposal.
- Applicability: engine performance only. No ecological parameter or rule change is implied.

## C. 2D map and UI options (MAP2D) — proposal, separate from verified #849 UI-A
- Report ranks Avalonia with a Skia canvas first (table 4.3; text says ~4.5), MonoGame second (4.0), Blazor/Canvas third, then Stride, Silk.NET, Raylib-cs. MAP2D pp.1-4. Scores are subjective and use weights the report chose.
- Design points: draw trees immediate-mode (not one control per tree), pick by point-in-crown test with a documented tie-break, give keyboard and screen-reader paths, and do not rely on colour alone. MAP2D pp.4-6. Guidance/inference; the WCAG point cites an accessibility blog.
- Engine stays authoritative; UI consumes immutable snapshots and sends commands. MAP2D pp.6-7.
- **Limits:** the "~60 fps at 1000 objects" claim rests on "preliminary testing outside this report" with no data (p.2). The report assumes a .NET 8 desktop app while the project runs Unity 6000.6.0f1 (AGENTS.md). Choosing Avalonia would be a new-dependency and architecture decision needing approval; this report grants nothing. It is a proposal, not verified #849 UI-A behaviour.

## D. Marteloscope data and virtual-marteloscope feasibility (MART, TREMS, SITES) — Scenario One / future G1 benchmark research
- Recommendation: keep Scenario One as the launch world; use real marteloscope data only as a benchmark/teaching fixture, not a replacement. MART pp.1-2, 20-21. Report's inference.
- A marteloscope is a static current-state inventory; the I+ software is not a growth model; future outputs (martelosim/SAMSARA2) are model forecasts, not observations. MART pp.2, 8-10. Source-backed per the report.
- Static marks cannot validate growth, regeneration or storm outcomes ("NO-GO"); the report found no public dataset linking marks to later remeasurement. MART pp.19-20.
- Human marking agreement is only slight-to-fair (36 experiments, 12 sites, British study, Pommerening et al. 2018, in `Rating experiments in forestry.pdf`, which is in the archive and can be checked), so expert marks are not "correct answers". MART pp.2, 10. I have **not** verified this against the paper.
- Rights tiers: CC BY 4.0 GBIF TreMs subset (green); Prelenfrey, Irish Teagasc/Coillte inventories and full I+ economic data (amber, rights unproven); real plots must not be embedded in a shipped game just because the software is MIT (red). MART pp.6-7, 15, 19.
- "80-100" is not verified as a spoken figure in the 2023 CCFG talk. Verified alternatives per the report: height:diameter ratio band (<80 stable, >100 unstable, Forest Research Thinning Practice guide Table 10, p.33), 80-100 F2 trees in a two-storeyed concept (Pommerening et al. 2021), and target DBH. Must not become a game rule. MART pp.10-11.
- The game's 40% per-unit BA ceiling is gameplay calibration, not an empirical Irish threshold; Teagasc Stage 3 guidance cited is no more than 20% BA per intervention. MART pp.11, 14-15, 19.
- **Contradictions between the three reports (unresolved):**
  - Prelenfrey: MART says the martelosim package ships a workbook (access verified, file not downloaded, rights unresolved, pp.4, 15); SITES says its data are "not publicly posted yet" (p.2).
  - Dataset scope: network ">300 sites in 28 countries" (MART p.3) vs TreMs "19 countries" (TREMS p.1) vs "26 countries" (SITES p.1). These may be different scopes and were not reconciled.
  - TREMS says Sitka, oak and beech "appear in the species list" and cites 89 species (TREMS p.2); MART says the archive was **not downloaded or inspected** (p.7, p.20). The species claim is unverified.
  - TREMS and SITES say the GBIF data are ready to use; MART warns the Irish plot name and repeated-measure history cannot be claimed (p.20).

## E. Independent Sitka data for the multi-area (G1) design (E1, MULTI) — Multi G1, not Scenario One
- Both reports reach NO-GO for an empirical multi-area release calibration: no public dataset has treatment-matched sample, target and control areas. E1 pp.1-4; MULTI pp.1-4.
- Datasets named: Dowd & Tobin 2026 (UCD, Zenodo DOI 10.5281/zenodo.19555954, CC BY 4.0, one mature Sitka stand, individual-tree destructive/LiDAR data, no thinning history); Roslin 2026 progeny trials (Edinburgh DataShare DOI 10.7488/ds/8190, 1,341 trees, no treatment); Forest Research Dartmoor 2005 thinning and advance regeneration (OGL v3.0, seedling counts only); Swedish NFI plots (MULTI p.1). E1 pp.1-2; MULTI p.1.
- **Discrepancy:** E1 lists Dowd as having DBH/biomass with no explicit height; MULTI says each tree has DBH, total height, volume and biomass and counts ~10-12 trees (E1 p.2 vs MULTI p.1). Check the dataset itself.
- Both define comparability as UNKNOWN unless mandatory site/stand fields match; no numeric thresholds are published (MULTI p.2). Suggested field trial: two stands, 30x30 m treated and control plots, remeasure after 3-5 years (E1 p.4). Proposals only.
- Fallback named by both: owner-directed synthetic demonstrator, clearly labelled, not calibration (E1 p.4; MULTI p.4).

## F. Nurse-crop establishment (NURSE) — teaching option, not a mechanic
- Simultaneous nurse planting is a trade-off: nurses can shelter or suppress the crop depending on site, species and timing. NURSE p.2. Field/guidance; the brief itself says it is not a universal Irish spacing or thinning formula.
- Sources it names: Guest & Huss 2012 (COFORD, Nursing of Oak in Ireland: pine inadequate on heavy soils, larch/spruce can dominate), Pommerening & Murphy 2004, Pommerening et al. 2024, Martin(i)k et al. 2024 (name garbled in the extracted text) (two-year Czech beech with birch/aspen, conditional effect). Not in the archive, unverified.
- Must not be implemented as a universal growth bonus; the exact webinar segment was not transcribed and the description was clarified by the project owner (NURSE p.2).

## Not covered
- `CCF_Six_Papers_Relevance_Evidence_Extraction.pdf` (24 p) and `CCF_Six_Complete_Forestry_Research_Papers.pdf` (111 p) were **not read**. Nor were the five academic papers (individual-based management, silvicultural systems, Bayesian projection, structure indices, CanopyShotNoise), `perrin2011`, `davies2008`, or the 2025 yearbook.
- No primary source (Teagasc pages, GBIF archive, Zenodo/Edinburgh datasets, Forest Research guide) was opened. Every figure above is as reported.
- No game mechanic, coefficient or spec is changed or proposed for adoption by this pack.
