# CCF Research Index

## Purpose

Supporting evidence index, not a decision authority. Research recommendations do not become accepted mechanics merely by appearing here.

## Source location

Research sources are held as Game Dev project sources and mirrored under the Drive `CCF — Shared Project Context` folder; `03 — Ecology Research` is now populated. The user's current CCF research/document distribution folder in Google Drive is `Documents/ccf game`; it stores the primary-source audits and the 2006 Irish Forestry issue. Drive copies are distribution/archive copies, not implementation authority: Git remains canonical where a canonical repository source exists. On 2026-10-16/17 the overlapping permanent research attachments were rationalised into four project-facing composites, each carrying per-claim provenance (file, page/section, original reference, evidence class) back to the originals. Research binaries are not committed to Git; preserve exact filenames and do not imply originals were deleted from Drive. See `## Research composites (2026-10-16)` and `## Scenario Two species evidence` below.

## D-023 — Browsing/protection

Primary implementation-oriented source: `Irish_CCF_Browsing_Protection_v1_Report.pdf`.

Browsing & Protection v1 is integrated in the Scenario One baseline (D-043): default browsing 0.2, shared natural/planted juvenile response, shelters/protection and save/load verification. The source report supports the research history; persistent browse/form history, concealment and richer protection mechanics are not thereby implemented.

Topics: background browse pressure, shared natural/planted response, species/stage vulnerability, leader damage/recruitment delay, fencing/individual protection, bramble concealment, persistent browsing/form history.

## D-024 — Understorey competition

Primary implementation-oriented source: `Understorey_Dynamics_Irish_CCF_Report.pdf`.

The same manager assessment/proposal distinction applies. Decision Log remains **Current direction**. The earlier baseline had no causal three-group v1. D-049 now accepts Scenario One model2 for independent bramble/bracken survival competition only; graminoids/concealment remain open. Accepted coefficients are gameplay calibration, not empirical values.

Topics: bramble/bracken/competitive graminoid cover, shared competition for natural/planted juveniles, cover lag/hysteresis, browse concealment, spot vegetation control rather than automatic tree-regeneration deletion.

## Broader evidence

Consolidated on 2026-10-16/17 into the ecology/regeneration and related composites (see `## Research composites (2026-10-16)`); the originals remain in the Drive archive. Status of each original:

- `Irish Atlantic Woodland Ecology for a Continuous-Cover Forestry Game Simulation.pdf` — **retained individual** project source (spatial regeneration/hydrology/species superset).
- `Atlantic Temperate Rainforest Under Continuous-Cover Forestry_ Game-Simulation Mechanisms.pdf` — now held via Composite 1.
- `Irish_Sitka_CCF_Deep_Research_Report.pdf` — now distributed across Composites 3–4.
- `Irish_CCF_ecology.pdf` — now held via Composite 1.
- `Game Mechanics for Atlantic Temperate Rainforest (Continuous-Cover Forestry).pdf` — now held via Composite 1.

The dedicated browsing (`Irish_CCF_Browsing_Protection_v1_Report.pdf`) and understorey (`Understorey_Dynamics_Irish_CCF_Report.pdf`) reports are now held via Composites 1–2; use those composites for those systems where they supersede earlier broad recommendations.

## CCF Primary Literature Synthesis v1

`CCF_Primary_Literature_Synthesis_v1.pdf` (12 pp., 5 October 2026; local copy `~/Documents/CCF Game/`, SHA-256 prefix `5b7a5261860e2bec`; not committed to Git). **Supporting evidence, not decision authority.** Used in the regeneration accounting work (D-047) only to support keeping vegetation competition and browsing as separate causal mechanisms, and treating Sitka leader browsing mainly as height delay and form damage. Its bramble/bracken survival values (English oak) and Irish age-30 top-height anchors are transfer/validation anchors, not adopted coefficients.

As of the 2026-10-16/17 rationalisation this synthesis was distributed across Composites 1–4 (each section retaining its exact provenance) and detached from project sources; the original remains archived in Drive. Its Irish Sitka age-30 anchors (from Lekwadi et al. 2012) are preserved in Composite 3.

## Growth model 1 sources (D-048)

- **Lekwadi et al. (2012)**, Irish Sitka site classification and top-height growth, via the CCF Primary Literature Synthesis v1 [A]. Only the age-30 Class III top height (20.4 m) is used as a published anchor; the rounded Table 3 coefficients do not reproduce the published anchors and are not used directly.
- **Comeau, White, Kerr & Hale (2010)**, *Maximum density–size relationships for Sitka spruce and coastal Douglas-fir in Britain and Canada*, Forestry 83(5): 461–468 [B]. GB Sitka slope −2.063, maximum SDI 1,868, Dq 25 cm reference. British transfer evidence: the paper reports regional differences and recommends species/region-specific relationships; it is not Irish validation.
- **Broad & Lynch (2006a, 2006b)**, Irish stand-level Sitka growth model and empirical panel-data validation. These are **not** the production Growth Model 1 engine. D-048's lineage remains Lekwadi-informed Irish height, Comeau British density transfer, existing individual-tree DBH/Hegyi mechanics and calibrated CCF mortality behaviour. Use GROWFOR/Broad & Lynch as a **GROWFOR-informed / benchmarked** aggregate comparison/validation envelope, never as `GROWFOR-based` implementation lineage.

### Distinct Broad & Lynch evidence records

| Field | `BROAD_LYNCH_2006A_GROWTH` | `BROAD_LYNCH_2006B_VALIDATION` |
|---|---|---|
| Citation | Broad, L.R. & Lynch, T. (2006a), “Growth models for Sitka spruce in Ireland”, *Irish Forestry* 63(1&2): 53–79 | Broad, L.R. & Lynch, T. (2006b), “Panel data validation using cross-sectional methods”, *Irish Forestry* 63(1&2): 80–95 |
| Source class | Peer-reviewed primary model-development / calibration study | Peer-reviewed primary empirical source-data / panel-data validation study |
| Species | *Picea sitchensis* (Sitka spruce) | *Picea sitchensis* (Sitka spruce) |
| Geography | Ireland, with British Forestry Commission low-site-index supplementation | Ireland (Coillte estate and research estate; 2003 production-stand survey) |
| Stand structure | Even-aged experimental and production stands | Even-aged experimental and production stands |
| Management scope | Thinned and unthinned model specification and calibration | Thinned and unthinned empirical source-data representativeness/bias assessment |
| Model scale | Stand level | Stand/panel-data audit |
| `validation_role` | Primary **model-development / calibration** source; not a post-fit predictive test | `empirical source-data / panel-data validation` |
| `independent_predictive_validation` | no | no |
| `ccf_direct_validation` | no | no |
| What it establishes | Stand-level `B,N,H` systems; top-height/site-index relationship; thinning relationship; fitted parameter matrices | Estate-versus-research site-index comparison; research-panel representativeness diagnosis; experimental-dependence limitations; cross-sectional research/nonresearch volume-data diagnosis; evidence consistent with sample-tree selection bias |
| What it does not establish | Individual-tree growth; individual mortality hazards; CCF/irregular-stand validation; independent final-model predictive validation | Independent final dynamic-model forecast validation; longitudinal forecast RMSE/MAE; prediction-interval calibration; CCF/irregular-stand validation |
| Key reported results | State vector `(B,N,H)`; separate thinned/unthinned systems; fitted matrices; thinning equation | Estate population 44,217 subcompartments (27,887 thinned, 16,330 unthinned); thinned site-index χ²(7) = 625.5, p < 0.001; unthinned χ²(7) = 1648.2, p < 0.001; 2003 cross-sectional sample 113 stands (70 thinned, 43 unthinned); common-volume relationship F(4,105) = 0.8835, null not rejected |
| Independence | Model development/calibration data; not independent of the final fitted model | `validation_independent_of_original_research_panel = true`; `validation_independent_of_final_volume_model_calibration = false` — the 113 stands were used in 2006a calibration/weight adjustment |
| Evidence verdict | Well-specified calibrated even-aged stand model; predictive validation not demonstrated | Empirical data-quality validation; not validation of final dynamic forecasts |

**Accepted V1B verdict:** `EMPIRICAL SOURCE-DATA VALIDATION CONFIRMED; FINAL GROWTH-MODEL PREDICTIVE VALIDATION NOT DEMONSTRATED`.

**Mortality wording:** stand-level stocking loss/mortality is embedded in the `N` state dynamics, but Broad & Lynch does not provide an independently parameterised individual-tree or cause-specific mortality hazard. Do not use Broad & Lynch as evidence for CCF's suppression-biased individual-tree mortality allocation.

**CCF applicability boundary.** Strongest support: pure Sitka, stand level, principally even-aged, conventional thinned/unthinned conditions, aggregate `B`/`N`/`H` comparison. Not validated by these sources: individual-tree growth response, local Hegyi release, regeneration, multiple cohorts, mixed species, strongly irregular CCF, continuous-cover predictive behaviour.

**Scenario One interpretation.** Broad & Lynch is useful as an aggregate plausibility benchmark while the stand remains plantation-like. Its evidential authority decreases as Scenario One becomes structurally irregular, multicohort and mixed-species. This does not declare Scenario One invalid.

### Primary-source audits (V1A/V1B)

- `Primary-Source Audit of the CCF Sitka Spruce Growth Evidence Base` (V1A): earlier audit of the Broad & Lynch 2006a growth evidence, existing GROWFOR extraction, later GROWFOR evaluations/manual and current CCF documentation. At V1A, the 2006b companion paper was not retrievable, so its findings were secondary-only. Preserved as a dated research artifact, not overwritten by V1B.
- `CCF-SITKA-GROWTH-EVIDENCE-AUDIT-V1B` (V1B): later audit that directly examined the uploaded `Irish Forestry, Vol. 63 No. 1&2, 2006`, including Broad & Lynch 2006a (pp. 53–79) and 2006b (pp. 80–95). It establishes the distinct 2006a/2006b roles and the accepted verdict above. Use V1B for current classification; retain V1A as historical evidence.

Both audit reports and the 2006 journal are held under `Documents/ccf game` in Google Drive and as local copies under `~/Documents/CCF Game/`. They are supporting research, not decision authority. The canonical repository records are this index and `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md`.

## Current multi-species and forestry evidence

The following supporting reports are held as Game Dev project sources and/or archived under `~/Documents/CCF Game/` and the Drive `03 — Ecology Research` folder. They are not committed research binaries and do not automatically adopt coefficients or mechanics.

- `CCF Species Evidence Pack v1 — Multi-Species Continuous-Cover Forestry Research Report.pdf` (**retained individual**): current species-specific evidence for assessing multi-species forestry and the limits of transferring Sitka relationships. Quantitative replacement design remains separate accepted work.
- `CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf` (**retained individual**): broader academic consolidation supporting source review and evidence gaps. The ordinary `CCF Academic Paper Consolidation.pdf` was verified fully superseded by the comprehensive edition and detached from project sources on 2026-10-16 (Drive copy retained).
- `GROWFOR_Technical_Evidence_Extraction_v1.md` (**retained individual**; canonical repository copy `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md`): named `BroadLynch2006_Sitka` equations/coefficients, validity limits, assortment coefficients, G3/G4 statistics and the V1B 2006a/2006b evidence-role fields. Deep technical provenance for the GROWFOR/Broad–Lynch evidence; not an implementation specification.
- Held via the four composites (originals in Drive archive): `Irish Continuous-Cover Forestry Transformation Evidence_ ContinuFOR, TranSSFor, LISS and Related Iri.pdf`, `European_Pro_Silva_CCF_Practice_Synthesis.md`, `Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf`, and the ecology/browsing/understorey/Primary-Literature-Synthesis reports.

The earlier note that `Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf` had no verified local/Drive location is superseded: the report is archived in Drive `03 — Ecology Research` and its content is preserved in Composite 4. Distinguish it from the later specialist-chat handoff “Forestry Economics & Irish Timber Markets — Research Synthesis”; that synthesis is not the source report. No prices, profitability targets or economic mechanics are adopted here.

**GROWFOR scope:** the distributed GROWFOR manual's model-selection screen shows thinned/unthinned models for Sitka spruce, Norway spruce, Douglas fir, lodgepole pine, Scots pine and larch, plus an Ash thinned model (7 named species / 13 listed options); COFORD 2012 described completed models for five species and Japanese larch/ash still in production. This description does **not** add any of these to the game roster or claim they are implemented in CCF; see `GROWFOR_Technical_Evidence_Extraction_v1.md` §3, §17.

D-051–D-054 accept forestry-first direction, independent scenario progression, the primary audience and fruit/nut sequencing. Research-proposed exact curricula and mechanics remain proposals: no six-scenario/two-branch sequence, marteloscope, Bio Tree, forest diary or other suggested feature is canonised by this index.

## Research composites (2026-10-16)

Four project-facing composites replace the overlapping permanent research attachments; each opens as supporting research (not decision authority) and carries exact per-claim provenance. Originals are archived in Drive.

- `CCF_Ecology_and_Regeneration_Evidence_Composite.md` — light/canopy, regeneration/establishment, understorey competition, deadwood/veterans, epiphytes, invasives, hydrology/site, biodiversity/monitoring, succession.
- `CCF_Browsing_Protection_and_Management_Evidence_Composite.md` — browser set, pressure vs damage, life-stage vulnerability, species palatability/recovery, browse algorithm, fencing/shelters, deer law/policy, parameter table.
- `CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md` (canonical repository copy `Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`) — density architecture and evidence, Pretzsch DEC/DMC, Irish Sitka site/height, individual-tree growth, crown geometry, oak/beech envelopes, GROWFOR summary anchors and the V1B validation scoring (`source_data_empirical_validation_score` 2; `independent_predictive_validation_score` 0; `ccf_transfer_validation_score` 0).
- `CCF_CCF_Practice_Economics_and_Transformation_Evidence_Composite.md` — Irish transformation trials, positive selection, wind/stability, Pro Silva practice, timber economics, labour/protection/pruning, salvage and timber quality.

The rationalisation evidence package (inventories, overlap matrix, removal-safety, provenance audit and handoff) is held outside the repository at `/media/jer/ZX20/CCF-Verification/ProjectSourceRationalisation-20261016T232710Z/`.

## Scenario Two species evidence

Scenario Two additions are **Sitka spruce, oak, beech, Scots pine, downy birch and common alder** (accepted roster). Two Scenario Two research sources were added to the Drive `03 — Ecology Research` library on 2026-10-17; both are supporting research, not decision authority.

- `Scenario Two Species Evidence Closure.pdf` (11 pp., Drive ID `1qubNMuSMFw_ilSreOic_ktJp3XfNLA_k`; **held as a project source**): closure for the three Scenario Two additions — Scots pine READY FOR INITIAL PARAMETER DESIGN (Irish GROWFOR external; recommended domain ages 10–60, 100–5,000 stems/ha, 10–100 m²/ha BA, top height ≥5 m); downy birch and common alder PARTIAL (design possible with explicit abstractions). Self-reports overall PARTIALLY READY with 5 unresolved contradictions and an audit limitation; no implementation blockers. Common-alder commercial role = secondary commercial crop.
- `Scenario Two Species Selection for Irish Continuous-Cover Forestry.pdf` (Drive ID `1WROZSOypheA1DwPU4fu8gYCsEoxOvu2Y`; **Drive archive only, not a permanent project source**): recommends Douglas fir + common alder. Its Douglas-fir recommendation is **superseded** by the accepted roster (which uses downy birch, not Douglas fir). Retained as archived context/history; do not treat its species recommendation as current authority.

## General reference

`the-earth-care-manual_-a-permaculture-handbook-for-britain-and-other-temperate-climates-pdfdrive.com-.pdf` is general reference, not authority for CCF forestry/ecology decisions.

## Understorey Model2 v1 (D-049)

`Docs/Research/UnderstoreyRecruitment/Model2Trial.md`, `Model2Handoff.md` and `Evidence/Model2/` retain the original calibration/paid-management evidence; `Docs/Verification/RegenerationModel2Integration/IntegrationHandoff.md` records the accepted clean integration. Field evidence supports plausible competition, while transfer to shared Sitka/Oak/Beech coefficients remains a deliberate abstraction. Initial/target/recovery values are accepted [C/I/D], not measured botanical rates. Do not generalize the tree site-productivity/soil-stability coupling beyond Scenario One without reassessment.

## Storms & Windthrow Model 1 (D-050)

`Docs/Research/WindthrowV1/` retains the readiness hypotheses, vulnerability/transform comparisons, forced and 144-world 25/50/100-year matrices, salvage calibration, save audit, rendered/performance evidence and asset/UI follow-ups. Manager accepted dormant Model1 integration from 857150b on 8 October 2026; post-integration reproduction is in `Docs/Verification/StormsWindthrowIntegration/`.

Annual 2%, severity .02/.06/.18 uniform, ReducedProposal and BoundedRational are frozen [C] gameplay calibration, not measured Irish probabilities or engineering wind-risk constants. New Scenario One still uses storm 0. Evidence supports bounded causal trade-offs for the recorded fixtures/policy/seeds, not a universal win rate, optimum prescription or century ecosystem-health guarantee. Crown/root quality, combined rendering and standalone Player acceptance remain open. Material recalibration requires StormModel 2 or separately approved persisted-profile/version architecture.
