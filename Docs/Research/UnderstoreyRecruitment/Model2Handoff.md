> Delivered trial record. Manager accepted the central Model2 v1 calibration on 8 October 2026 (D-049). The clean integration and approved active-model P1 copy supersede the pending decision/status below; see [IntegrationHandoff](../../Verification/RegenerationModel2Integration/IntegrationHandoff.md). Original trial evidence and limitations remain preserved.

# Save18 / regeneration2 manager handoff

**CALIBRATION READY — PARAMETER DECISION REQUIRED**

The approved five-field state implementation works, and the integrated trial demonstrates beneficial, wasted and harmful paid treatment. The remaining decision concerns the unmeasured initial/site target and fixed response/recovery profile. Schema approval did not approve those values. No additional saved field is needed. No main merge or ecological calibration acceptance is claimed.

## Delivery identity

Task: approved-schema understorey competition and recruitment causality trial. Worker: Codex; role: primary ecology implementation/verification worker, with Manager integration review separate.

Implementation/evidence commit: `49b01d8fc90ea0779ebfdc61a5a421af718da1d6`.
Baseline continuation: `7370ea0bb39cf4ec4ee7b75dd9996248c89f93ea`; accepted audit `ed960b7`.
Base and locked canonical context: `a8596df9c52669a36709f09e85dfe6568640af49`.
Branch: `task/understorey-recruitment-causality`.
Owned worktree: `/media/jer/ZX20/CCF-understorey-recruitment`.
Subsequent handoff-only delivery tip is reported in chat. Branch pushed; content clean after delivery under `git -c core.filemode=false status` (ZX20 reports uniform executable bits). Shared Git configuration was not changed. Main, Claude-owned UI, canonical sources/live locks and frozen Reference assets are untouched.

## Implemented scope and compatibility

Production code changed on this trial branch: independent cell and patch bramble/bracken state, deterministic recovery, shared height-limited linear juvenile survival, separate runtime natural/planted cause accounts, geometric local overrides, clearance integration, model2 vegetation display inputs, save18 validation and versioned new-game policy. Existing Beech display-height fix retained. No price, adult growth, seed/request equation, nursery, shelter/browse parameter, biological height, new package, asset family or scene/prefab edit.

Exactly five new saved fields: cell `brambleCover` and `brackenCover`; patch `brambleCover`, `brackenCover`, `competitionUpdatedYear`. Two covers are independent finite fractions [0,1], can overlap and are never normalized. Runtime calibration/cache/ledger objects are not saved. Complete grid order and explicit JSON field presence are checked. Missing, malformed, future-dated, duplicate local-key and out-of-grid-center state is rejected before live-world changes.

New Scenario One is save18/model2. Historical v17 model1 keeps its original biological behaviour; historical/missing-field policies remain intact. Reference Future remains model0. No habitat/mesh inference or silent migration. Historical v15/16/17 byte-layout hash helpers remove only appended fields for appropriate historical models; they cannot substitute for model2 hashes. Actual model2 hashes include all new state.

Patch precedence: most recent created year, then greatest stable `(x,z,radius)` key. List order does not determine outcomes. Natural exposure is the area integral of pointwise max(bramble,bracken), with overlapping area assigned once. A later completed cell treatment masks older patches inside that cell, retaining their cross-border portions outside it. Existing saved work orders provide that mask; no sixth state field. Independent area oracle: production .7306473 versus .7306400109, tolerance .001. Paid cross-border clearance and save/reload passed.

Annual cause-account order is vegetation → light → browse, with unchanged browse growth assessment, followed by establishment/promotion. Exact individuals use the same vegetation hazard with an independent stable random domain. Natural abundance is relative occupancy; it is not priced as physical stems. Planted individuals, promoted trees, clearance removals and cash are recorded separately. Ledgers are runtime diagnostics and retained verification CSVs, not additional saved annual-history fields.

## The narrow decision

Trial response is `strength × max(cover types) × height vulnerability`. Central strength .35; full vulnerability ≤.3m, declining to zero ≥1.5m; shared Sitka/Oak/Beech response. These are [C], not fitted annual field mortality.

Candidate target = `.8 × clamp01(siteProductivity × soilStability) × (.1 + .9 × clamp01(light))`. Initial cover is `.25 × target` for both categories. Site potential for both is [I]; numeric target/ramp/floor/initial fraction are [C]. **Fresh trial games give both categories equal trajectories under this shared candidate rule. Separate fields permit differing covers/history, but equality is not an empirical botanical claim.** Deep shade is not an absence rule. No quantitative evidence selects this initialization from alternatives.

Recovery gain/loss .30/.45 remains [D/C]; actual competitor sensitivity tested .15/.25 and .45/.65. Strength .15/.35/.60, escape1/1.5/2.5m and initial fraction .1/.25/.5 were also tested. Default moderate/no-clearance Sitka100yr promotions262; strength low/high294/264; escape short/tall288/260; initial low/high264/262. Vegetation losses vary much more strongly than promotions because existing capacity, bands, promotion and subsequent stand evolution interact. Do not fit mortality coefficients solely to century promotion totals.

Manager should accept or replace the explicit initialization/target rule and fix strength/window/recovery as labelled gameplay calibration before production integration. If accepting a first gameplay calibration, retain the tested central response and require the current shared site/initial-cover assumption to be accepted explicitly. Do not promote it to measured Irish species-specific biology. A replacement target/profile needs rerun model2 cases/anchors; trial saves have no saved profile and must not silently acquire a different biological definition. Future changes to an accepted model2 definition require deliberate version/compatibility review.

Evidence and implementation rationale: [Model2Trial.md](Model2Trial.md). Earlier audit evidence/asset briefs remain valid historical supporting work; they are not relabelled model2 trials.

## Actual gameplay and economy results

39 actual100yr worlds: main matrix33 including two deterministic repeats, targeted economy6. All recorded at10/25/50/100 years; 3,900 annual world steps and11,700 species-year rows. No thinning/light20%/moderate40%/heavy60% distributed thinning; no/early/repeated cell clearance; browse0/.8; actual paid planting12 and optional12 shelters; density/recovery/strength/window/initialization sensitivity. Paid work uses existing purchase, designation, approval and annual resolution. Shelters are existing time-limited individual protection, not a fabricated free permanent fence; natural/cohort protection has independent controlled fixtures. Costs, plant/shelter counts, cash, living adults and basal area are actual model outputs. Mean cover columns are background-cell means; natural loss uses weighted local exposure.

| Required state | Actual observation |
|---|---|
| No clearance needed | Low-target no-clearance world produces304 natural Sitka promotions100yr; zero-competition control306, plus6 planted broadleaf promotions. Seed/light remain necessary. |
| Useful clearance | Dense-opening targeted early clearance:315→344 Sitka promotions100yr for€36, and43→49 living adults. At50yr129→140 promotions. Early10yr vegetation loss20.512→19.765 relative units. |
| Wasted clearance | Zero-competition targeted treatment completes7 eligible cell orders, costs€31.50, and leaves all measured biological outcomes identical at every checkpoint. |
| Harmful clearance | Moderate repeat-year3/every5yr treatment removes7 planted juveniles and established natural occupancy; natural Sitka promotions262→0, clearance cost€7,586.25 by100yr. |
| Repeated management | Background cover recovers between paid treatments and is reset again. Relevance returns as vegetation pressure; this trial does not justify an automatic5yr clearance interval, which destroys existing recruitment. |

Targeting matters: dense opening, blanket64-cell treatment€288 adds2 promotions; up-to8 bright-cell treatment€36 adds29. Moderate targeted treatment adds7 Sitka plus1 Beech promotion for€36. Unthinned/light/heavy central blanket treatment gives fewer cumulative promotions than no clearance. Clearance is therefore not universally optimal, and low competition need not be treated.

These results support a **conditional paid ecological tradeoff**, not measured timber profit or a universally best plan. In the useful dense target case, century cash is€36 lower and basal area12.579 versus13.641m² despite more surviving trees. No subsequent harvesting is imposed, so no extra realised timber income is claimed. Additional cumulative promotions are model-created trees over time, not29 independently identified rescued field seedlings. Stock prices are not multiplied by relative abundance or used to invent avoided planting cost. Main matrix unthinned cash€11,931 without area treatment,€11,643 with blanket early treatment; pricing is unchanged.

Broadleaf seed sources initially come from planted stock; century native regeneration counts are therefore not a fair empirical species competition ranking. Existing browse/seed/light effects remain independently visible. Preparing the area before already spot-prepared planting adds no planting immunity. Clearance can remove existing planted/natural juveniles; future seed arrival recolonises under existing rules.

## Measured storage and performance

Actual UTF8 pretty-printed save files,64 cells plus1 local patch:306,526 bytes with the five fields versus299,528 for the same world with only those fields removed: **+6,998 bytes (~2.34%)**. Compact serialization:173,129 versus168,489, +4,640. This isolates field overhead; it is not a generic worst-case file size estimate. Saved float formatting/history and patch count vary. No LFS/storage policy change.

Paired fresh loaded simulation worlds:192 natural bands,30 exact juveniles; adults336/1300/3000/5000;0/12 local patches. Four ordered repetitions, first repetition excluded as declared warm-up; three measured samples/model/workload. Both advance existing habitat as well as actual annual ecology and planted juveniles. Final medians:

| Adults | Patches | Model1 ms | Model2 ms | Observed extra ms |
|---:|---:|---:|---:|---:|
|336|0|37.148|37.506|0.357|
|336|12|36.456|37.736|1.280|
|1300|0|271.857|273.476|1.619|
|1300|12|274.464|275.164|0.700|
|3000|0|766.240|766.563|0.324|
|3000|12|764.771|773.773|9.002|
|5000|0|1515.209|1524.937|9.728|
|5000|12|1526.445|1535.863|9.418|

Observed median overhead0.04–3.51%; distributions/ranges are retained. Small replicated sample and synthetic dense stands do not establish rendering FPS or a universal overhead bound. Vegetation display resolves its manager once per enumeration; no repeated scene lookup per plant.

Patch histories100/1000/5000, dispersed and heavily overlapping, are measured separately. Build uses clipped cell buckets plus strip event sweeps and sorted active priorities; exact queries use a balanced spatial tree with priority and conservative center-distance pruning. No juvenile×global-patch nested scan. Before the measured corner-case correction,10k queries/5k overlapping patches visited12,917,779 nodes and took~410ms; final118,603 nodes and~10.14ms, independent exact winners preserved. Final5000 overlapping cache build~530ms: large histories still cost real annual time. Worst-case geometric traversal is not claimed theoretically logarithmic; both concentrated and distributed scaling are disclosed. No new pairwise ecology algorithm.

## Verification and anchors

Final full applicable suite: **23/23 PASS** after the immediate-reset save fix. This includes Growth, Regeneration, Reference, Completion, Economy, Browsing, Planting, Pruning, Deadwood, Progress, SaveHardening, RNG, RNGPolicy, Interaction, EcologyCalibration, P0 Integration, Removal, Clearance, MenuTutorial, TimberYield, WorkEconomy, canonical JuvenileMortality and DisplayHeight. Forced historical model1 completion passes separately.

State gate **1,396 checks PASS**, including immediate-reset save/reload, malformed covers/presence/timestamps/grid/centers, atomic rejection, patch capture/restore, overlapping area/list-order/ties,961 independent exact-point comparisons, paid cross-border cell masks/reload,72 natural cause cases,24 matched natural/exact survival cases with256 exact individuals each,27 rising/falling recovery profiles,8→9yr save continuation and repeated model2 anchors. Exact/natural survival comparison uses a declared5σ+2 individual tolerance; realised individuals are not forced to equal expected abundance.

New model2 save18 anchors, RNG1/growth1 and browse explicitly .2: start `FA855239CDDA32D8`, year1 `02334804F65C0234`. Eight repeated main-matrix checkpoint hashes match at10/25/50/100. Historical model1 anchors are separate and retained; no model2 hash is rewritten to model1.

Historical0 canonical anchors `BFC55473C1506067` / `3485B6630C9EA448`; Reference immutable replay `7AD177B3CC2F73C7` / `9CDF21A541C5968D`. Historical model1 growth0 anchors `962846D2F517B293` / `FBB8F470D85FF815`; model1 current-growth completion v17-layout `D7C4DDD36B53FCCE`. Reference resources are unchanged.

Source SHA/mode/time/markers are retained per gate. Harness contracts changed only for approved current save18/regeneration2 policy, explicit old-version fixtures and extra historical layout logging. Growth/survival/calibration checks were not weakened. Final actual-new-game completion anchor: `702766DECE591E21`.

Initial fixture error: patch instance alias retained a deliberately future timestamp; fixture reset fixed it. An early one-year diagnostic had leftover high browsing after unit cases; final new-game anchor explicitly sets .2. Save bounds initially used a missing extension method; replaced with List.Exists, failed compile record retained, then state/full-suite rerun. The first full suite exposed dated save/model fixtures and a browse comparison with unequal competition inputs; the corrected economy rerun then exposed a real immediately-after-reset missing-grid save. Model2 capture now ensures that initial grid, and four explicit checks cover saving/reloading before any annual/map operation. The final suite was rerun after this production fix. Pre-distance-prune performance measurements are retained separately. These are disclosed corrections, not accepted red gates.

Beech display fixture: 45 natural cases and30 paired cases across5 heights/3 refreshes PASS; maximum absolute error1.7e-7m, tolerance.002m, authoritative state unchanged. Earlier authoritative.6m → natural.6m/planted.6m fix is retained; biological heights unchanged.

## Integration and player review

Claude-owned UI is untouched. Main model1 teaching remains truthful. On accepted model2 integration, the current sentence “In this version, clearing does not change how well seedlings survive or grow.” becomes stale; use the exact replacement in [TutorialHandoff.md](TutorialHandoff.md) before release. Distinguish botanical pressure from general habitat and adult-tree crowding; no compulsory first-year treatment or fixed repeat interval. Existing [asset audit](../ScenarioOneAssets/UnderstoreyAssetAudit.md) and [creation backlog](../ScenarioOneAssets/AssetCreationBacklog.md), including the ten creation briefs, remain retained; no asset replacement is required for the Beech fix. Independent state feeds existing bramble/bracken art, but botanical recognition still needs player review and is not assumed from passing numerical tests.

Reproduce the listed launchers in Model2Trial.md with Unity closed and only one Editor. For player review, open the owned branch, start a fresh Scenario One, use M/map and compare bright-cell targets before approving clearance. Verify previews include existing juvenile removals/costs, cleared footprint and later regrowth; save/reload should preserve local recovery. The new biological field values can be inspected in the manager's understorey-cell list. Automated fixtures verify effects; no new human acceptance of model2 visuals is claimed.

Retained evidence: `Evidence/Model2/` state/overlap/paired/recovery/save-size/performance/39-world annual/horizon/stand-summary/clearance-pair CSV/JSON and23-gate plus historical completion results. Full logs and sample saves remain ignored under Build. No disposable verification script/meta remains in Assets after delivery. No additional schema approval is requested; the remaining gate is the initial target/profile parameter decision and independent Manager integration review.

## Ownership

Ecology write owner: this task, per the explicit Manager confirmation. Concurrent ecology writers encountered: none in this owned checkout. ScenarioOneManager modified: yes, for the approved model policy, five-field state capture/restore, local preparation, annual exact-juvenile survival/accounting and immediately-after-reset save correction. Save schema modified: yes, save18 with exactly the five approved fields. Ownership conflicts: none. Claude-owned pedagogy/UI and canonical coordination files were preserved.

Recommended next step: Manager accepts or replaces the explicit initial/site target and central response/recovery profile, then independently reviews/integrates the branch and conditional P1 copy. The user’s subsequent storms/windthrow packet must start only after this branch is integrated and current main is clean; no storm implementation has started.
