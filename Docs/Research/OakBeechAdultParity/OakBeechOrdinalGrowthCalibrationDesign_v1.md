# Oak/Beech ordinal growth calibration design v1

## Identity and conclusion

Research base c3fa549e6ce68a8659a781904b50122ed2d968e6; canonical context 321791b8203bc709c57615fd4d6a19614d78db24. Source implementation assessed against the packet's b4894cc/a395e3f generic kernel, not a claim that either branch is accepted. Updated Manager interpretation permits ordinal/calibrated behaviour with validation envelopes, not empirical coefficients. No production changes, fitted parameters or Unity verification.

**Kernel decision A - existing kernel can be calibrated for the supported ordinal diameter-release and coarse height-trajectory targets.** A does not certify the supplied numerical envelopes. Corrected evidence is sufficient to specify calibration validation categories and direction; several age-height ranges remain provisional [C] design envelopes rather than measured British/Irish bands. Do not fit against the erroneous 48-m Beech height or infer a release-height multiplier from it. A height-competition extension is not justified by this report and is not proposed.

## Supplied research provenance

Audited all five pages of local `height and diameter growth of Sessile oak and European beech.pdf`, supplied for the 2026-10-14 work. SHA-256: `4304ba1628ba141305eb81c26d316666cf95b59fa90f75ed931ee72c40a618a5`. This synthesis contains internal citation tokens without a resolvable bibliography. They do not independently establish DIRECT evidence. Primary sources below were recovered independently; untraced claims remain flagged. The synthesis also inconsistently locates the Beech trial in Czechia/Slovakia and misreads its columns.

Categories: **DIRECT** = actual measurements for that study's population/quantity; **TRANSFER** = applying those measurements or published regional guidance elsewhere; **INFERENCE** = derived implication/proposed envelope; **UNSUPPORTED/OVERSTATED** = cited material does not establish the claim. DIRECT in Slovakia is not DIRECT Irish evidence.

## Critical primary-source correction

[Štefančík 2013, Table 2](https://jfs.agriculturejournals.cz/pdfs/jfs/2013/06/04.pdf) describes crop-tree groups in Cigánka, Slovakia: four 0.25-ha plots; treatments start at stand age 60. The columns are crop-group diameter `d_g` and mean height `h_g`, not dominant/top height or individual diameter ranges. At age 100:

| Treatment | Crop-group d_g (cm) | Crop-group h_g (m) |
|---|---:|---:|
| Control | 35.0 | 32.2 |
| Crown thinning, 5-year interval (H) | 41.4 | 33.2 |
| Crown thinning, 10-year interval (H2) | 38.2 | 32.4 |
| Heavy thinning from below (C) | 48.2 | 39.5 |

The 48.2 is **diameter**, not height. Relative to control, C's final height is 22.7% higher, H's 3.1%; neither is double. The paper's twofold comparison concerns volume increment, not height. Initial C crop trees were larger, so final differences are not an unconfounded release-effect multiplier. Its favourable management conclusion concerns crown thinning, not a blanket preference for heavier thinning from below. Table image was visually checked against the PDF, not just extracted text.

For diameter, basal area and stem count reproduce `d_g = sqrt(4G/(pi*N))*100` to rounding: e.g. control at age 100 gives about 35 cm. Treat this as crop-group quadratic-mean diameter (Dq), not arithmetic mean DBH or every crop tree's DBH. `h_g` is the paper's crop-group mean height; no conversion to top height is authorised.

## Claim audit

| Supplied claim | Category and finding |
|---|---|
| Oak dominant/top height 20-25 m at age 50 | INFERENCE: no directly verified age-50 table behind the synthesis; retain only as a proposed favourable-site [C] band, not evidence coefficients |
| Oak dominant/top height 30-35 m at age 100 | INFERENCE / regional-transfer plausibility; not established as a universal Atlantic envelope |
| Oak crop-tree DBH 30-50 cm at 50, 40-60/50-60 at 100 | UNSUPPORTED as measured age-DBH bands; log-size requirements do not establish these growth trajectories; defer hard acceptance |
| Beech dominant height 25-30 m at 50, 35-40 m at 100 | INFERENCE: crop-group mean height at 100 does not verify top height, and a trial starting at 60 cannot verify age 50 |
| Beech heavy-thinned height 45-50/48 m | UNSUPPORTED/OVERSTATED: diameter/height-column error; reject |
| Beech crop-tree DBH about 60-70 cm at 100 from that trial | UNSUPPORTED: not in its d_g column; reject that attribution |
| Heavy thinning doubles height growth | UNSUPPORTED/OVERSTATED: reject as a general height or growth-rate multiplier |
| Beech diameter release response | DIRECT for studied crop groups, TRANSFER for Scenario One; direction accepted, effect size not universal |
| Oak positive diameter release response | TRANSFER guidance/ordinal expectation; accept direction, not the guessed height response |
| Oak height increment persists to old age | TRANSFER support; no claim of an identical age curve on every site |
| 50-cm mid-log implies DBH 60-70 cm at 120-150 years | INFERENCE only: bark, taper, log height/length and growth regime needed; not a deterministic conversion |
| All continental observations apply to Britain/Ireland with similar humidity | OVERSTATED: climate similarity alone is insufficient transfer validation |
| Scenario One horizon approximately 50/<80 years | Incorrect project assumption: completion lower bound 25; separate Century Review at 100; tree age differs from scenario year |

## Reviewed validation envelopes

Keep three tiers distinct: study-specific reference envelopes (check matching statistic/population), provisional design bands [C] (manager-reviewed calibration targets, not empirical pass/fail), and deferred unsupported numerical bands.

| Species | Tree/stand age | Site | Management | Quantity | Reviewed envelope/status |
|---|---:|---|---|---|---|
| Beech | 100 | Cigánka site index 28, NW slope, 560 m; no mapping to Scenario One | Control/C/H/H2 started age 60 | Crop-group Dq | 35.0/48.2/41.4/38.2 cm reference points; DIRECT there, TRANSFER here; compare only equivalent selected groups |
| Beech | 100 | Same | Same | Crop-group mean h_g | 32.2/39.5/33.2/32.4 m reference points; not top height |
| Beech | 50 | Favourable mesic transferred-design site; not numerically mapped | Dominant crown cohort, explicitly specified in future fixture | Top height H100 per ha | Proposed 25-30 m [C] plausibility envelope only; no hard evidence acceptance |
| Beech | 100 | Same design category | Same | Top height H100 per ha | Proposed 35-40 m [C] only; reject 45-50 m attribution to Table 2 |
| Sessile Oak | 50 | Favourable mesic transferred-design site; not numerically mapped | Dominant crown cohort | Top height H100 per ha | Proposed 20-25 m [C] only |
| Sessile Oak | 100 | Same | Same | Top height H100 per ha | Proposed 30-35 m [C] only |
| Either species | 50/100 | Any uncalibrated site | Undefined | Stand mean DBH, individual DBH, dominant DBH | No reviewed numeric envelope; do not substitute crop-group Dq |
| Either species | 50/100 | Poor/waterlogged/drought-exposed | Any | Height/diameter | No quantified envelope; ordinal adverse response only, not a multiplier |

Future fixtures must define top height explicitly (e.g. mean height of the 100 largest-DBH living trees/ha) and sample size, dominant-height selection separately if used, arithmetic mean height separately, and crop selection before treatment. Do not apply that proposed H100 definition to a paper whose statistic is h_g. Preserve birth age, planting age and promoted age so simulation year 25 is not adult age 50. A cohort planted late in the minimal-path audit is still juvenile at completion.

For this ordinal calibration design, numerical site response is fixed at **1.0**, as required by the packet; no new numerical site response is introduced. The existing kernel reads `GetSiteProductivity(position)` from cell state: this design boundary must not be misreported as proof that every existing runtime cell already has productivity 1.0. The site categories above specify validation provenance, not production infrastructure or a fitted site index.

## Ordinal targets

| Target | Verdict | Boundary |
|---|---|---|
| Beech reaches canopy sooner than Sessile Oak | WEAK/DEFER as a universal ranking; ACCEPT as explicitly labelled, matched-site [C] design hypothesis | Supplied mature-size anecdotes do not establish common-site juvenile time-to-canopy; test light/browse/competition and initial ages separately |
| Oak sustains height increment longer | ACCEPT sustained later-life Oak increment; WEAK/DEFER comparative lifespan of increment versus Beech | Oak persistence is supported; a strict paired-species ranking needs paired curves |
| Both respond positively to release | ACCEPT diameter direction | Do not imply doubling height or beneficial response to every abrupt exposure |
| Beech crop-tree DBH responds strongly to release | ACCEPT ordinal diameter sensitivity | Different treatments, initial selection and competition confound quantitative multipliers |
| Oak adult light demand | ACCEPT ordinal greater light demand than Beech | No new numeric adult light threshold is inferred |
| Premium Oak normally beyond Scenario One | ACCEPT as [C] expectation for new plantings around normal completion; WEAK/DEFER as an absolute prohibition through year 100 | Existing older trees, long continued play, log quality and taper make an absolute age/size gate unjustified |

## Generic-kernel assessment

Inspected `ForestEcologyController.ApplyGrowth` and species assets: diameter increment is potential times `(1-D/Dmax)` times the Hegyi competition factor `1/(1+CI/Ci50)` times site. Height increment is potential times `(1-H/Hmax)` times site, without competition. Oak and Beech currently share Pd/Ph .4, Dmax120, Hmax40, Ci50 3: [C] values, not verified species-specific evidence.

The diameter kernel already responds continuously to release, and distinct potentials/size brakes can express coarse ordinal species trajectories. Height has an implicit saturating trajectory; it cannot claim explicit age-stage transitions, delayed post-suppression height recovery or a height-release effect. None of those extensions is established as necessary by the corrected evidence. Thus A is bounded to coarse ordinal calibration; if future accepted evidence requires those mechanisms, re-open kernel B explicitly rather than patching them during fitting.

No Pd/Ph/Dmax/Hmax/Ci50 fitted values or search ranges are selected here. Search ranges are optional in the packet and cannot be made safer merely by giving unsupported numbers. Next calibration should preregister candidate ranges as [C], keep Sitka untouched, split juvenile/adult validation, compare matched statistics, retain failed trials, and validate envelopes across management fixtures rather than fit one endpoint. Do not use a diameter-growth calibration to compensate for a density architecture defect; N1 remains separate.

## Sources and evidence boundaries

- [Štefančík 2013](https://jfs.agriculturejournals.cz/pdfs/jfs/2013/06/04.pdf), Table 2 / Fig. 2 and methods. See corrected study-specific quantities above.
- [Duplat & Tran-Ha 1997](https://www.afs-journal.org/articles/forest/abs/1997/07/AFS_0003-4312_1997_54_7_ART0003/AFS_0003-4312_1997_54_7_ART0003.html): French dominant-height reconstruction across 50 stands, sustained later growth and regional differences; does not validate the synthesis's British/Irish age-height bands.
- [Forest Research, Sessile Oak](https://www.forestresearch.gov.uk/tools-and-resources/tree-species-database/131560-sessile-oak-sok/): institutional species guidance; rotations commonly 130-160 years and sawlogs possible around 100 under particular management. This supports a long-horizon expectation, not a unique diameter/age map.
- [Forestry Commission Handbook 9, Growing broadleaves for timber](https://cdn.forestresearch.gov.uk/1993/03/fchb009.pdf), p.66: Beech responds to crown thinning but abrupt release of older neglected trees can cause adverse effects. A release direction is not a universal safety prescription.
- [Nicolescu et al. 2025, Management of sessile oak](https://link.springer.com/article/10.1007/s11676-025-01868-1): review-level transfer support for late Oak height increment and veneer mid-length log diameters 50-55 cm. These refer to logs, not “mid-log DBH”; converting to standing-tree DBH needs taper/measurement conventions. Underlying original references should be traced before a numerical gate uses them.

## Handoff

A research/design deliverable only. New PDF audited; falsely doubled height and column-confused Beech values rejected; exact age-50 growth numbers and standalone premium DBH/age inference are not upgraded to empirical facts. No production or canonical changes; previous v2 reconciliation preserved. No Unity verification, parameter fitting, site multiplier, growth/density/timber change or main integration.
