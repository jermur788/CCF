# CCF Research Index

## Purpose

Supporting evidence index, not a decision authority. Research recommendations do not become accepted mechanics merely by appearing here.

## Source location

The Revision 5 source reports these PDFs as Game Dev project/source uploads. Its permanent Drive `03 — Ecology Research` folder was not populated; do not imply the PDFs already live in Drive or Git. Preserve exact filenames when a separately authorised task mirrors research. This migration does not upload research binaries.

## D-023 — Browsing/protection

Primary implementation-oriented source: `Irish_CCF_Browsing_Protection_v1_Report.pdf`.

Browsing & Protection v1 is integrated in the Scenario One baseline (D-043): default browsing 0.2, shared natural/planted juvenile response, shelters/protection and save/load verification. The source report supports the research history; persistent browse/form history, concealment and richer protection mechanics are not thereby implemented.

Topics: background browse pressure, shared natural/planted response, species/stage vulnerability, leader damage/recruitment delay, fencing/individual protection, bramble concealment, persistent browsing/form history.

## D-024 — Understorey competition

Primary implementation-oriented source: `Understorey_Dynamics_Irish_CCF_Report.pdf`.

The same manager assessment/proposal distinction applies. Decision Log remains **Current direction**. The earlier baseline had no causal three-group v1. D-049 now accepts Scenario One model2 for independent bramble/bracken survival competition only; graminoids/concealment remain open. Accepted coefficients are gameplay calibration, not empirical values.

Topics: bramble/bracken/competitive graminoid cover, shared competition for natural/planted juveniles, cover lag/hysteresis, browse concealment, spot vegetation control rather than automatic tree-regeneration deletion.

## Broader evidence

- `Irish Atlantic Woodland Ecology for a Continuous-Cover Forestry Game Simulation.pdf`
- `Atlantic Temperate Rainforest Under Continuous-Cover Forestry_ Game-Simulation Mechanisms.pdf`
- `Irish_Sitka_CCF_Deep_Research_Report.pdf`
- `Irish_CCF_ecology.pdf`
- `Game Mechanics for Atlantic Temperate Rainforest (Continuous-Cover Forestry).pdf`

Use the dedicated browsing/understorey reports for those systems where they supersede earlier broad recommendations.

## CCF Primary Literature Synthesis v1

`CCF_Primary_Literature_Synthesis_v1.pdf` (12 pp., 5 October 2026; local copy `~/Documents/CCF Game/`, SHA-256 prefix `5b7a5261860e2bec`; not committed to Git). **Supporting evidence, not decision authority.** Used in the regeneration accounting work (D-047) only to support keeping vegetation competition and browsing as separate causal mechanisms, and treating Sitka leader browsing mainly as height delay and form damage. Its bramble/bracken survival values (English oak) and Irish age-30 top-height anchors are transfer/validation anchors, not adopted coefficients.

## Growth model 1 sources (D-048)

- **Lekwadi et al. (2012)**, Irish Sitka site classification and top-height growth, via the CCF Primary Literature Synthesis v1 [A]. Only the age-30 Class III top height (20.4 m) is used as a published anchor; the rounded Table 3 coefficients do not reproduce the published anchors and are not used directly.
- **Comeau, White, Kerr & Hale (2010)**, *Maximum density–size relationships for Sitka spruce and coastal Douglas-fir in Britain and Canada*, Forestry 83(5): 461–468 [B]. GB Sitka slope −2.063, maximum SDI 1,868, Dq 25 cm reference. British transfer evidence: the paper reports regional differences and recommends species/region-specific relationships; it is not Irish validation.

## Current multi-species and forestry evidence

The following supporting reports are available locally under `~/Documents/CCF Game/` unless noted. They are not committed research binaries and do not automatically adopt coefficients or mechanics.

- `CCF Species Evidence Pack v1 — Multi-Species Continuous-Cover Forestry Research Report.pdf`: current species-specific evidence for assessing multi-species forestry and the limits of transferring Sitka relationships. Quantitative replacement design remains separate accepted work.
- `CCF Academic Paper Consolidation.pdf` and `CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf`: broader academic consolidation supporting source review and evidence gaps; the comprehensive edition extends the earlier consolidation.
- `Irish Continuous-Cover Forestry Transformation Evidence_ ContinuFOR, TranSSFor, LISS and Related Iri.pdf`: Irish transformation evidence, indexed under its actual local filename.
- `European_Pro_Silva_CCF_Practice_Synthesis.md`: practitioner/website synthesis, distinguished from empirical ecological measurements and Irish numerical calibration.
- `Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf`: underlying source research report, identified by the Manager. Distinguish it from the later specialist-chat handoff “Forestry Economics & Irish Timber Markets — Research Synthesis”; that synthesis is not the source report. The source PDF's local/Drive location has not been verified in this refresh. No prices, profitability targets or economic mechanics are adopted here.

**GROWFOR scope:** five total modelled species — Sitka spruce, Douglas fir, lodgepole pine, Norway spruce and Scots pine. This evidence description does **not** add lodgepole pine to the game roster or claim these five species are implemented in CCF.

D-051–D-054 accept forestry-first direction, independent scenario progression, the primary audience and fruit/nut sequencing. Research-proposed exact curricula and mechanics remain proposals: no six-scenario/two-branch sequence, marteloscope, Bio Tree, forest diary or other suggested feature is canonised by this index.

## General reference

`the-earth-care-manual_-a-permaculture-handbook-for-britain-and-other-temperate-climates-pdfdrive.com-.pdf` is general reference, not authority for CCF forestry/ecology decisions.

## Understorey Model2 v1 (D-049)

`Docs/Research/UnderstoreyRecruitment/Model2Trial.md`, `Model2Handoff.md` and `Evidence/Model2/` retain the original calibration/paid-management evidence; `Docs/Verification/RegenerationModel2Integration/IntegrationHandoff.md` records the accepted clean integration. Field evidence supports plausible competition, while transfer to shared Sitka/Oak/Beech coefficients remains a deliberate abstraction. Initial/target/recovery values are accepted [C/I/D], not measured botanical rates. Do not generalize the tree site-productivity/soil-stability coupling beyond Scenario One without reassessment.

## Storms & Windthrow Model 1 (D-050)

`Docs/Research/WindthrowV1/` retains the readiness hypotheses, vulnerability/transform comparisons, forced and 144-world 25/50/100-year matrices, salvage calibration, save audit, rendered/performance evidence and asset/UI follow-ups. Manager accepted dormant Model1 integration from 857150b on 8 October 2026; post-integration reproduction is in `Docs/Verification/StormsWindthrowIntegration/`.

Annual 2%, severity .02/.06/.18 uniform, ReducedProposal and BoundedRational are frozen [C] gameplay calibration, not measured Irish probabilities or engineering wind-risk constants. New Scenario One still uses storm 0. Evidence supports bounded causal trade-offs for the recorded fixtures/policy/seeds, not a universal win rate, optimum prescription or century ecosystem-health guarantee. Crown/root quality, combined rendering and standalone Player acceptance remain open. Material recalibration requires StormModel 2 or separately approved persisted-profile/version architecture.
