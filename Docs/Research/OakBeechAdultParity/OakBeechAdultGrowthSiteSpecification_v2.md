# Oak/Beech adult growth and species × site — evidence-reconciled v2

**Status: RESEARCH RECOMMENDATION; not a production change or adoption decision.** Date: 2026-10-13. Reviewer: Codex, Evidence Reconciliation Reviewer. Source/base: `952bd5bcc339d82915a8b88a5eb9be759f47d032`, `claude/ecstatic-archimedes-a06tac`. Canonical context read independently: `321791b8203bc709c57615fd4d6a19614d78db24`. v1 and all three v1 Evidence files remain unchanged. This replacement covers the proposal's growth/site claims; it does not qualify the separate overnight implementation.

## 1. Executive conclusion

**Final implementation gate: NO — implement only infrastructure and retain current adult-growth behaviour.**

The generic mechanism + species-data architecture survives. The one-dimensional site schema does not. Preserve moisture/drainage, fertility and exposure/climate independently, with rooting constraints reserved. Use neutral numerical responses initially rather than inventing curves.

None of the ten proposed exact growth values is established as a fitted Oak/Beech coefficient by the actual inspected research. They are **CALIBRATION ONLY**, including inherited 120 cm and Ci50 3. Keep proposed changes as experiment candidates; do not freeze 0.65, 0.35/0.50 or 33 as implementation-ready growth parameters. Labelling a fit [C] prevents an empirical claim, but does not supply an independent validation envelope. v1's principal height-age bounds were recalled, and its DBH uplift partly inverts the game's own competition assumptions. That is insufficient to certify the retune as correction of an established biological error.

This is a bounded hold, not a rejection of generic calibrated forestry simulation. It avoids replacing one weakly supported calibration with another presented as evidence-reconciled. D-059 adult-growth parity remains open. The next production packet can implement a neutral multidimensional site/data seam while preserving behavior; a separate growth-calibration packet must first acquire defensible species/site/management validation targets or explicitly authorize an ordinal experimental model without empirical trajectory claims.

**Premium Oak: C — normally beyond Scenario One**, for the newly introduced Oak cohort and ordinary completion times. Rare late/open-grown outcomes are possible hypotheses, not a reason to accelerate growth or promise a premium.

## 2. Evidence used

Labels: [R] inspected repository fact; [A] direct empirical result for its original population/quantity; [T] geographic/species-management transfer with stated limits; [ORD] direction/rank, not magnitude; [C] model/design calibration; [U] unsupported or not recovered. [A] does not mean an Irish coefficient unless the study actually measures that quantity in Ireland.

Seven actual Project PDFs were opened/extracted, with the species matrix visually checked. Exact source provenance is recorded below. Local copies are under `~/Documents/CCF Game/`; PDFs are not added to Git. External extraction/hash manifest and working findings remain under `~/Documents/ChatGPT/Local Game Dev/OakBeech-Reconciliation-Evidence/`.

| ID | Actual file | Pages | SHA-256 |
|---|---|---:|---|
| R1 | CCF Species Evidence Pack v1 — Multi-Species Continuous-Cover Forestry Research Report.pdf | 27 | `8432191f8d09fa2c06954d047cc6f3bcd58b8983986ead4e3029f153b3277206` |
| R2 | CCF Academic Paper Consolidation.pdf | 2 | `a4646d2c4c5a6566b5dce9f5ede5d90a3698f8dc14bcb5ceb86f2e1912d541f1` |
| R3 | CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | 3 | `3e17e40d127f2fb5af53d5529150c5697b5012b8a232c6bcaa14be468aae3d56` |
| R4 | CCF_Primary_Literature_Synthesis_v1.pdf | 12 | `5b7a5261860e2beca9c910a43c0e7c443636db219f334519020a91d232e558f9` |
| R5 | Irish_CCF_ecology.pdf | 11 | `95dcec247817ec9932214c3e0c47846191d6e5d1d7f78ba6ae8edb75de33c6ec` |
| R6 | Species-Specific Density.pdf | 12 | `8a54250e4b729bca8f09c1d9dcc2a2383bdbac1dccda30c8acb7e42b8263210b` |
| R7 | MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | 5 | `b9f801b535162cdb9cfef5717f4ecc1b4ab8eac33777485c554d2617726c29a3` |

Evidence contribution and limits:

- **R1**, species matrix p.5, dossiers p.7 and growth/density discussion pp.9–10: supports species distinctions and transferred/ordinal treatment; explicitly reports no comparable Irish dynamic Oak/Beech model found. This is a synthesis, not an individual-tree coefficient fit.
- **R2/R3 are delivery summaries**, only 2/3 pages, advertising 87/84-page archives. The local files do not contain those full tables/appendices. Their unavailable full archives must not be cited as if inspected. Internal chat-file citation tokens do not identify accessible source measurements.
- **R4**, Irish Sitka site-height and regeneration sections: useful mechanism/evidence-boundary guidance. Sitka top-height and juvenile Oak-survival results do not identify adult Oak/Beech growth coefficients.
- **R5** is a CCF biodiversity/conversion synthesis. It informs ecological caution, not the ten coefficients.
- **R6**, §4 (p.4): explicitly requires moisture/drainage, fertility, exposure/climate and rooting/stability context; supports generic mechanism + species data. Research suggestions do not override current decisions or authorize density changes.
- **R7** records density-data/transfer gaps; no adult-growth fit is recovered from it. Its generic-density suggestions are not adopted here.
- Available originals **Conry & Clinch (1989)** and **Dunbar et al. (2002)** (`2ndPaperSearch/conry1989.pdf`, `dunbar2002.pdf`) were checked for site-study context. Irish soil/yield-class results and the Douglas-fir parent-material/aspect/exposure model support retaining distinct site variables. Stand volume yield class is not individual-tree DBH increment; Douglas-fir coefficients are not Oak/Beech coefficients. Pretzsch & del Río's original density paper was consulted only to distinguish occupancy evidence from growth evidence; no density recommendation is implemented.
- Repository sources: v1 including its arithmetic script; `Docs/BeechSpeciesParameters.md`; species assets and `GrowAdults` at the source base; canonical D-059–D-062 at the locked docs commit. The old repository evidence labels are not independent verification of their uncited primary ranges.

Primary institutional statements checked directly (supplementary corroboration): [Forest Research Sessile Oak](https://www.forestresearch.gov.uk/tools-and-resources/tree-species-database/131560-sessile-oak-sok/), [Forest Research Beech](https://www.forestresearch.gov.uk/tools-and-resources/tree-species-database/beech-be/), [Ecological Site Classification](https://www.forestresearch.gov.uk/tools-and-resources/fthr/ecological-site-classification/), and [Teagasc hardwood grading](https://teagasc.ie/crops/forestry/advice/timber-harvesting/felling-presentation--grading-of-hardwood-timber/). British species guidance is [T], not a newly fitted Irish law; Teagasc grading is direct Irish trade guidance for the specified log class.

## 3. What v1 got right

Retain: species-keyed parameter rows; a generic state-based mechanism; the existing competition seam; an explicit pure-Sitka bypass; version-controlled behavior; no edits to shared legacy species assets to retune old saves; static site configuration rather than redundant persistence; no growth tuning to force premium timber; separation of juvenile light response from adult site productivity; disclosure of evidence gaps.

The reproduction script correctly exposes how chosen assumptions propagate. Running it again reproduced its deterministic research arithmetic; no Unity or biological validation follows from that result. Its files remain historical, not a v2 acceptance oracle.

## 4. What v1 overstated / got wrong

1. **“Double counting” is a hypothesis, not proven identification.** The repository Beech range is described as young/good-site growth; its original sample/competition conditions are not recovered. Inverting an assumed CI factor cannot identify zero-competition potential. Transferring that argument to Oak adds another unverified step. Poisson equal-neighbor CI estimates are model calculations, not measured Scenario One stand competition.
2. **Recalled top-height bands cannot certify individual-tree height.** v1's Oak H50/H100 11–16/18–25 m and Beech 14–19/22–29 m, paired refit ranges and “Beech height ≥ Oak at every age” are not recovered empirical acceptance envelopes. Site, provenance, management, statistic and tree position matter; dominant/top height is not every-tree height.
3. **Growth scale is not a biological maximum.** In this kernel 120 and 33 are slowdown/asymptote scales. Stature plausibility does not estimate rate or asymptote. Reject the claim that the existing Oak curve is quantitatively wrong by 3–4 m on the basis of recalled band centres.
4. **Preserving an axis is not activating a curve.** Omitting fertility/exposure/climate because their magnitude is unknown is an architecture error. Likewise, exposure is not owned exclusively by storms; recording it does not activate storms or duplicate windthrow.
5. Reject v1's moisture multipliers and refit intervals as ready parameters. A five-category table and round numbers are still numerical assumptions. Neutral reference cannot be called an empirically “optimal” site.
6. Context is corrected: D-060 requires the complete Scenario One; D-062 reopens the broadleaf market deferral. v1's instruction to keep broadleaf markets deferred under D-044 is stale, not a surviving policy. This task neither edits timber nor accepts its implementation.
7. Reject a “completes by Year 25” growth acceptance rule. Twenty-five is the minimum completion year, not a guaranteed outcome; no year-30/harness completion rule is gameplay law.
8. Distance beyond the 8 m Hegyi neighborhood does not establish complete Sitka independence: stand-wide density and other shared processes can couple trees. v1's far-neighbor equality claim needs carefully isolated fixtures.
9. Continuous exponential approximations are not bit-exact discrete annual recurrences. Numerical reproduction is software verification, not evidence of forestry correctness.

## 5. Oak adult-growth evidence

The supported species is **Sessile Oak (Quercus petraea)**, not interchangeable “Oak spp.” Evidence supports a relatively lighter/free-draining niche versus pedunculate oak, light demand in silviculture, and strong form-management dependence. That does not establish a universal dry-site advantage or greater numerical drought tolerance than Beech at every stress level. [R1; Forest Research, T/ORD]

No inspected source supplies the proposed Pd=0.65 or Ph=0.35 in the given kernel. Yield-class and long-rotation guidance constrain interpretation, not those coefficients. Keep British/European trajectories as potential validation data only after recovering exact species/site/management definitions. Existing Oak growth is also weakly evidenced; retaining it temporarily is compatibility preservation, not scientific endorsement.

## 6. Beech adult-growth evidence

Beech's deep, moist, well-aerated soil preference and drought/waterlogging sensitivity are supported transfer directions. Strong shade persistence/release is distinct from universally higher adult productivity. Fertility or exposure advantages require their own evidence; no adult Ci50 can be inferred from a juvenile light-tolerance rank. [R1; Forest Research Beech, T/ORD]

The legacy repository's 0.3–0.5 young-tree ranges lack an inspected primary fit to this kernel. They do not establish Pd=0.65 after algebraic “correction,” nor Ph=0.50 after pair-fitting recalled height bands. Oak/Beech separate rows are warranted even when some initial values remain equal; that equality is calibration/continuity, not measured parity.

## 7. Recommended adult-growth architecture

Keep the existing generic form as a **candidate mechanism**, not an accepted species-specific model:

`ΔD = Pd × oldSiteScalar × newSpeciesSiteResponse × clamp01(1 − D/Dscale) × 1/(1 + CI/Ci50)`

`ΔH = Ph × oldSiteScalar × newSpeciesSiteResponse × clamp01(1 − H/Hscale)`

D is DBH in cm, H height in m; annual increments have corresponding per-year units. Dscale/Hscale are model slowdown scales. Versioned rows are keyed by species, without bespoke OakTree/BeechTree algorithms. Use one parameter/response resolver for simulation and forecasts; keep all existing consumers of Ci50 consistent.

**For the infrastructure packet, do not wire changed growth rows or change the old expressions/order.** Separate configuration/provenance and pure lookup facilities can be introduced dormant. Adult height's lack of competition/light response and the promotion increment discontinuity remain limitations. Do not present age independence as established biological release behavior. Future fit/architecture review must evaluate height/DBH slenderness and delayed promotion rather than silently copying the Sitka top-height rule.

## 8. Recommended site architecture

Retain three independent domains; moisture/drainage and exposure/climate can each contain distinct descriptors rather than conflating unlike mechanisms:

| Domain / minimum data | Initial species-response treatment | Explicit boundary |
|---|---|---|
| Moisture/drainage: water-availability class plus aeration/drainage constraint; both have Unknown and declared Reference states | Supported directions, ordinal descriptors; numerical map neutral/deferred | Moist wet soil is not waterlogging; excessive drainage is not universally good |
| Fertility: nutrient class with Unknown/Reference and provenance | Ordinal suitability descriptors; neutral/deferred numerical response | No shared “site quality” score or guaranteed rich-soil bonus |
| Exposure/climate: exposure class and reserved seasonal drought/frost/temperature context | Ordinal descriptors; neutral/deferred response | No new windthrow or climate model, and no storm ownership monopoly |
| Reserved rooting context: depth/compaction/texture/rockiness | Neutral/deferred; retain schema extension point | Do not fabricate root-depth penalty coefficients |

A descriptor includes whether it is unknown, a reference assumption or a sourced observation. **Unknown must not silently become a known physical reference.** No weighted sum of axes, invented universal site index, fitted response curve, compensating fertility-for-waterlogging rule or assumed product of penalties is authorized. The later numerical aggregation design remains deferred.

Species/axis response classification (direction and numeric status separated):

| Species | Axis | Direction classification | Initial numerical classification |
|---|---|---|---|
| Sessile Oak | Moisture/drainage | DIRECT/TRANSFER EVIDENCE: freer-draining/light soils relative to pedunculate oak; avoid compacted/wet sites | NEUTRAL/DEFERRED; ordinal constraints only |
| Sessile Oak | Fertility | DIRECT/TRANSFER EVIDENCE for tolerance of some poorer mineral soils; ORDINAL preference/suitability, not maximal productivity | NEUTRAL/DEFERRED |
| Sessile Oak | Exposure/climate | DIRECT/TRANSFER EVIDENCE for exposure/form and frost considerations; ORDINAL descriptors | NEUTRAL/DEFERRED |
| Beech | Moisture/drainage | DIRECT/TRANSFER EVIDENCE: moist aerated/deep soils, drought and waterlogging limitations | NEUTRAL/DEFERRED; ordinal constraints only |
| Beech | Fertility | DIRECT/TRANSFER EVIDENCE for site suitability; no universal ranking over Oak | NEUTRAL/DEFERRED |
| Beech | Exposure/climate | DIRECT/TRANSFER EVIDENCE for climatic/exposure considerations; precise response ORDINAL | NEUTRAL/DEFERRED |

Any eventual assigned multiplier is **CALIBRATION**, even if its direction is supported. Active ordinal diagnostics may communicate suitability assumptions, but a neutral numeric placeholder must be disclosed as inactive biology. Shade/light/neighbors remain in their existing domains; do not encode them again as fertility/productivity.

## 9. Parameter-by-parameter adjudication

Each exact proposal has **one** required classification below. “Numeric use” distinguishes continuing an inherited constant from adopting a new calibration. Geography: R means repository-only, T means supporting transferred context rather than a fitted coefficient. No inspected Irish empirical fit exists for these exact quantities.

Validation envelope codes: **V-D** independently measured adult DBH increment/trajectory matched for species, site and management (not recovered here); **V-H** matched individual versus dominant height-age data (not recovered here); **V-S** finite/positive response and documented scale behavior, not a biological maximum; **V-C** existing compatibility plus separate adult competition/release validation (unfitted). Section 14 specifies the implementable tests and data requirements.

| Species | Proposal | Classification | Source / geographic status | Numeric use now | Required validation envelope | Recalibration |
|---|---|---|---|---|---|---|
| Sessile Oak | Pd **0.65 cm/yr** | **CALIBRATION ONLY** | v1 inversion of repository Beech range/assumed CI; R/C, not Oak T coefficient | No new use; research candidate only | V-D; controlled CI/site and promotion age, not premium eligibility | Expected; no validated search interval |
| Sessile Oak | Dscale **120 cm** | **CALIBRATION ONLY** | Existing Oak asset; R/C, no empirical diameter-limit claim | Preserve legacy 120; not certification of a new fitted row | V-S + V-D; initial sizes above scale must not shrink | Review if growth is later fitted |
| Sessile Oak | Ph **0.35 m/yr** | **CALIBRATION ONLY** | Pair-fit to unrecovered recalled UK bands; R/C computation | No new use | V-H; top-height cannot stand for every tree | Expected; fit with scale jointly if justified |
| Sessile Oak | Hscale **33 m** | **CALIBRATION ONLY** | Same recalled fit; no direct/transfer asymptote measurement | No new use; reject biological-maximum interpretation | V-H + V-S; stage, site and form definitions explicit | Expected |
| Sessile Oak | Ci50 **3** | **CALIBRATION ONLY** | Existing asset/game Hegyi calibration; R/C | Preserve unchanged for all current consumers | V-C; consistency across growth, suppression, forecast, density readers | Deferred pending evidence/interface |
| Beech | Pd **0.65 cm/yr** | **CALIBRATION ONLY** | Repository young-tree range plus assumed CI; R/C, primary fit unrecovered | No new use | V-D; a young-stand rate is not zero-CI potential | Expected |
| Beech | Dscale **120 cm** | **CALIBRATION ONLY** | Existing Beech asset soft scale; R/C | Preserve legacy 120 only | V-S + V-D; not empirical maximum | Review with any later fit |
| Beech | Ph **0.50 m/yr** | **CALIBRATION ONLY** | Recalled-height fit and repository young-tree range; R/C | No new use | V-H; release and slenderness diagnostics | Expected |
| Beech | Hscale **33 m** | **CALIBRATION ONLY** | Recalled fit, no recovered asymptote study | No new use; not species-wide stature ceiling | V-H + V-S | Expected |
| Beech | Ci50 **3** | **CALIBRATION ONLY** | Existing asset/game calibration; R/C | Preserve unchanged; no shade-derived adult bonus | V-C; all consumers agree | Deferred |

**None is SUPPORTED or TRANSFER-CALIBRATION RANGE ONLY as an exact kernel coefficient.** This does not reject calibration as a category. It rejects automatic adoption now. The following *claims/other proposals* are **REJECTED** as ready: unrecovered height-age bands and derived paired refit intervals; Pd range 0.5–0.8 as a scientific confidence interval; five moisture response levels 1.0/0.8/0.6/0.4/0.2 and both species' 5-entry tables; cross-species “Beech taller at every age” as a universal biological oracle. They can be explored only as explicitly chosen future calibrations.

## 10. Scenario One reference-site vector

Declare a versioned **compatibility reference**, not a newly discovered soil survey:

- Moisture/drainage = Reference water-availability / Reference aeration.
- Fertility = Reference nutrient state.
- Exposure/climate = Reference exposure / Reference climate context.
- Rooting context = unspecified/reserved, not an invented measured depth.

The new response seam returns **exactly 1.0** for Sitka, Oak and Beech at this reference [C: identity for compatibility]. Initially numerical response for other descriptors is also neutral/deferred; do not label waterlogged Beech favorable because its current multiplier is neutral. Maintain separate ordinal flags/data.

Do not declare this vector to be “optimal class 3,” a brown-earth survey or typical Irish Sitka gley. Distinguish the new identity response from the existing `cell.SiteProductivity` scalar and any soil-stability uses: neither is reset, repurposed or given new effects. Existing calls/order remain unchanged.

## 11. Pure-Sitka compatibility strategy

Keep the actual old Sitka growth, site and density routes untouched, including constants, random domains, order and batching. Sitka has no new broadleaf row. **Bypass** the new resolver in its numerical path; do not add a multiply-by-one refactor and then assume hash equality. The new configuration identity 1.0 does not replace old site behavior.

Keep all old growth-model/save paths and Reference Future replay unchanged. Test exact existing hashes in their original configurations; a comparison excluding a version field is supplementary state evidence, not permission to rebaseline the authoritative full hash. Mixture changes can legitimately influence neighbors, but this packet does not authorize them. A distance-only fixture cannot isolate global density effects.

No model default change or new numeric growth-model ID is selected here. The separate density branch already proposes model 2; reconcile ownership with the Manager before allocating any future version. This research branch is not a merge of that implementation.

## 12. Oak premium-size conclusion

**C — normally beyond Scenario One.** Applies to the pure-Sitka start followed by newly planted/recruited Oak and normal objective-led completion; not to every arbitrary pre-existing mature Oak fixture or an indefinitely extended run.

Teagasc specifies first-grade veneer/planking butts longer than **2.7 m** and **45 cm mid-diameter**, with clean grain and absence of disqualifying defects. Those are log conditions, not a standing-tree DBH threshold. Compute usable diameter at the actual log midpoint with the existing taper/log-position conventions; document bark basis. Do not invent a fixed DBH≈46–52 conversion or certify v1's approximate conversion without its taper/measurement assumptions.

British guidance describes long oak rotations and earlier sawlogs under selected management [T; Forest Research]. This supports caution about young-cohort premiums, not a guaranteed age-to-grade equation. v1's Year-25/100 calculations illustrate consequences of its C values only: minimal-completion-time Oak is young; idealized low-CI century-age trees can be larger. These are not measured growth expectations or proof of first-grade availability. Exceptional late opportunities remain possible, and ordinary sawlog is not automatically premium veneer.

Twenty-five is a minimum completion year; 100 is the separate Century Review, not a mandatory end date. Actual completion remains objective-driven. No growth, assortment threshold, price, objective or horizon may be changed to unlock premium Oak.

## 13. Save/version implications

Infrastructure-only work should require **no save-schema change**. Keep ForestSaveData.CurrentVersion **20**, old saved growth values, definition behavior and legacy/reference replay. Do not add redundant persisted site axes to a static scenario whose existing identity/configuration can determine them.

Lookup must be reproducible for existing saves: immutable configuration/response versions resolved through existing saved identifiers where sufficient. Do not mutate a shared scenario site row and reinterpret old saves. If available identifiers cannot distinguish a genuinely active new behavior, retain neutrality and return to Manager; do not assume a saved profile/model field or schema bump. No new GrowthModel.Latest/default alteration is needed just to store dormant neutral descriptors. Later changed growth requires its own explicit version/compatibility decision.

## 14. Verification envelopes

Separate **software verification** from **biological validation**:

| Envelope | Required test/data | Boundary of claim |
|---|---|---|
| Compatibility | Old growth 0/1, Reference Future, save/load and existing anchors exactly unchanged | Preserved software behavior, not broadleaf ecological correctness |
| Neutral site | Reference vector returns literal 1.0; descriptor round-trip/lookup deterministic; Sitka numerical path bypassed | Identity [C], not optimal-site evidence |
| Schema | Each axis distinguishable; Unknown not substituted as observed Reference; moisture versus aeration separable | Model can express evidence; no fitted curve implied |
| V-S numerical safety | Finite values; nonnegative increments; no shrinkage when initial size exceeds a soft scale; no NaN/overflow; state-based update honors actual site/CI assumptions | Stability, not empirical trajectory validation |
| V-D biological DBH | Recover primary species/site/management/competition data with units, sample and size/age classes; compare increment distributions and longitudinal sizes | No numeric pass band supplied by the inspected set |
| V-H biological height | Recover named trajectory/table and distinguish top/dominant from individual height; validate delayed promotion, suppression/release and height/DBH form | v1's recalled H50/H100 bands are not acceptance targets |
| V-C competition | Shared resolver agrees with all existing consumers; test CI changes separately from site; adult/juvenile evidence kept separate | Ci50 3 remains calibration, not measured shade-tolerance conversion |
| Site direction | Species profiles can retain drought, waterlogging, fertility and exposure constraints independently; no shade/productivity bonus | Ordinal signs/suitability are supported; multiplier magnitudes deferred |
| Replay | Original save/load continuation and deterministic repeat in unchanged modes | No new density/timber/default behavior authorized |
| Scenario | Completion remains Evaluate/SetOutcome-driven; no fixed-year/premium requirement; old calibration retained | Report actual outcome year, do not force Year 25 or 30 |

v1's research oracle can validate implementation of *chosen* equations after reauthorization, but it cannot validate the equations biologically. Its current CSV is not a v2 acceptance baseline. Numerical tolerance values from v1 (1e-3 relative, 0.05 cm/m absolute) are proposed software tolerances [C], not measurement uncertainty; no new acceptance tolerance is frozen here. Report performance separately if later wiring is authorized, without a new target or stocking reduction.

## 15. Implementation-ready architecture

Generic parameter/response interfaces; separate species identities/provenance; three site domains plus rooting extension point; explicit Unknown/Reference; immutable configuration lookup; neutral numeric placeholders; pure-Sitka bypass; one shared forecast/simulation interface; separate numerical and evidence-validation tests. These may be implemented dormant under a bounded infrastructure packet, without claiming active species growth/site parity.

## 16. Implementation-ready parameters

Exact compatibility values permitted now:

| Use | Exact value | Status |
|---|---|---|
| New reference-site response for each current species | **1.0** | [C] identity; not ecological coefficient |
| Oak legacy row retained | Pd **0.40 cm/yr**, Dscale **120 cm**, Ph **0.40 m/yr**, Hscale **40 m**, Ci50 **3** | [R/C] existing behavior only; not a new validated fit |
| Beech legacy row retained | Pd **0.40 cm/yr**, Dscale **120 cm**, Ph **0.40 m/yr**, Hscale **40 m**, Ci50 **3** | [R/C] same boundary |
| Save format | **20** | [R] unchanged compatibility contract |

Preserve actual existing paths/assets; this table does not require migrating them into a new resolver. No newly changed adult-growth row or off-reference numerical curve is implementation-ready. Sitka is not assigned a new row or refitted constant.

## 17. Calibration-only parameters

All ten adjudicated v1 values; v1's moisture levels/tables; assumed CI-at-stocking conversions; recalled-band pair-fits; response aggregation weights; proposed numerical tolerances. They remain hypotheses/design calibrations, not transferred coefficient ranges. Only inherited 120/3 continue through old paths now. Recalibration is expected if primary growth validation is recovered; no Pd/height search interval is frozen merely from plausibility.

## 18. Unsupported/deferred items

Primary fitted adult Oak/Beech DBH/height data for this kernel and site; adult competition tolerance and release response; off-reference numerical site functions and aggregation; species-specific exposure/fertility productivity magnitude; bark/taper conventions needed for a precise DBH-to-log-grade mapping; quantitative promotion/slenderness fixes; long-term senescence; uncertain configuration-version implications of future active site behavior. Recover full advertised consolidation archives if needed, but do not assume they contain missing growth fits.

D-059 Oak adult parity, Beech adult parity and active species×site biology remain open. Neutral infrastructure is necessary groundwork, not completion of D-059. No density rework, timber/economy changes, storm activation, juvenile retune, new species, UI or canonical-doc edits are included.

## 19. Bounded production implementation sequence

1. Manager authorizes infrastructure-only scope and resolves ownership/version interfaces against the separate density work. Do not merge the overnight implementation as part of this task.
2. Add generic data types/provenance and immutable reference configuration with all three site domains retained. All numerical responses neutral; no production growth call-path rewrite, new saved state or default flip.
3. Verify descriptor/lookup determinism and exact old save/reference/anchor preservation. Stop on schema need or unexpected divergence.
4. Recover named, inspectable species/site/management growth trajectories and distinguish individual versus top-height data. Review promotion/competition/slenderness limitations and formulate independent validation envelopes.
5. Return a separate explicitly calibrated growth proposal to Manager. If approved, add versioned Oak/Beech rows dormant in the agreed model/version, run pure-species/mixed/replay/old-anchor tests, and attribute each changed new-mode outcome. Do not treat rebaselines or default adoption as implicit.
6. Only after qualification may the Manager decide subsequent active site responses, integration and eventual default changes in their own packets.

**Final implementation gate: NO — implement only infrastructure and retain current adult-growth behaviour.** Architecture is ready; changed numerical growth rows are not sufficiently independently bounded by the actual source set. This commits a research recommendation only and preserves v1 as the historical proposal.
