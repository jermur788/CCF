# Accepted integration-input inventory — Tier 1

Canonical context: c6dc136b7f1f8668d9f67f95f48968d290ad5067. Actual pushed main/integration baseline: c1564b857972ee18917e8a461c457533a54cc3f1. Keep context authority separate from the production baseline. Read-only remote snapshot and ancestry checks performed 2026-10-15; exact deltas and containing refs are in the adjacent JSON. No Unity run, merge or silent anchor update.

Density is **awaiting final Tier-2 Queue B verdict** at 90e3496873da4e3b23038993ca5bd2fec3c2e7f1. Earlier v2 B and partial v3 source evidence do not substitute for that verdict. Marking commit is locally retrievable but no pushed containing branch exists in the fetched remote refs or live ls-remote snapshot. Final candidate creation is held.

## Density v3

- Branch: `task/mixed-density-architecture-v3`
- Exact input: `90e3496873da4e3b23038993ca5bd2fec3c2e7f1`
- Pushed branch HEAD: `90e3496873da4e3b23038993ca5bd2fec3c2e7f1`
- Status: CONDITIONAL: awaiting exact-head final Tier-2 Queue B verdict
- Dependencies/anchors: Pure Sitka/Model1 exact; density-owned files only; do not import its inherited timber/history wholesale.

Production delta files:

- `Assets/ForestPrototype/AdultMortalityAccount.cs`
- `Assets/ForestPrototype/ForestEcologyController.cs`
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs`
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs.meta`
- `Assets/ForestPrototype/MixedDensityModel.cs`
- `Assets/ForestPrototype/MixedDensityModel.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneEconomyAdapter.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/SitkaGrowthModel.cs`
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs`
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs.meta`
- `Assets/ForestPrototype/Species/BeechSpecies.asset`
- `Assets/ForestPrototype/Species/SessileOak.asset`
- `Assets/ForestPrototype/TimberYield/TimberYieldCalculator.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldConfiguration.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldValidation.cs`
- `Assets/ForestPrototype/TreeSpeciesDefinition.cs`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`
- `Assets/ForestPrototype/UI/WalkingHudView.cs`
- `Assets/ForestPrototype/UI/WorkPlanView.cs`
- `Assets/ForestPrototype/WorkEconomy/ForestryWorkDomain.cs`
- `Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs`
- `Assets/ForestPrototype/WorkEconomy/TimberAssortmentLabels.cs`
- `Assets/ForestPrototype/WorkEconomy/TimberAssortmentLabels.cs.meta`

Verification-only delta files:

- `Tools/Verification/CloudTypeCheck/InputSystemStub.cs`
- `Tools/Verification/CloudTypeCheck/UnityEditorStub.cs`
- `Tools/Verification/CloudTypeCheck/baseline-prod.txt`
- `Tools/Verification/CloudTypeCheck/baseline-tools.txt`
- `Tools/Verification/CloudTypeCheck/run_typecheck.py`
- `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`
- `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs`
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/BroadleafTimberOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/DensityCalibrationSearch.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/DensityTrajectorySim.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/HistoryOfflineStubs.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/MixedDensityIsolatedCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/MixedDensityOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/SitkaIsolatedCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/SitkaYieldFixture.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/UnityShim.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/recruitment-series-v2.csv`
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_composite_mutations.py`
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_density_trajectories.py`
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py`
- `Tools/Verification/ThreeSpecies/Qualification/DensityProbe.cs`
- `Tools/Verification/ThreeSpecies/Qualification/DensityQualification.cs`
- `Tools/Verification/ThreeSpecies/Qualification/analyze_longrun.py`
- `Tools/Verification/ThreeSpecies/Qualification/analyze_trial2.py`
- `Tools/Verification/ThreeSpecies/Qualification/make_e80_completion_measurement.py`
- `Tools/Verification/ThreeSpecies/Qualification/run_qualification.py`
- `Tools/Verification/ThreeSpecies/Qualification/run_v3_remaining.sh`
- `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs`
- `Tools/Verification/ThreeSpecies/run_unity_gates.py`

## Dormant site

- Branch: `task/oak-beech-site-infrastructure-v1`
- Exact input: `47539847d7e2531824c30f3dfe635d55404699a8`
- Pushed branch HEAD: `47539847d7e2531824c30f3dfe635d55404699a8`
- Status: ACCEPTED DORMANT FOUNDATION
- Dependencies/anchors: Five independent descriptors; numerical multiplier remains bit-exact 1.0; no save/default activation.

Production delta files:

- `Assets/ForestPrototype/ForestSiteProfile.cs`
- `Assets/ForestPrototype/ForestSiteProfile.cs.meta`
- `Assets/ForestPrototype/ScenarioSiteProfiles.cs`
- `Assets/ForestPrototype/ScenarioSiteProfiles.cs.meta`
- `Assets/ForestPrototype/SpeciesSiteResponse.cs`
- `Assets/ForestPrototype/SpeciesSiteResponse.cs.meta`

Verification-only delta files:

- `Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs`
- `Tools/Verification/SiteInfrastructure/run_site_check.py`

## Oak/Beech growth

- Branch: `task/oak-beech-ordinal-production-calibration-v1`
- Exact input: `796da99029190ef71e68d5fa722f25240e696318`
- Pushed branch HEAD: `796da99029190ef71e68d5fa722f25240e696318`
- Status: ACCEPTED ORDINAL INPUT / B caveats; final integrated anchor adjudication pending
- Dependencies/anchors: Only two species rows [C]; no Sitka/equation/site changes. Manager must explicitly accept attributable continuation/completion movements before rebaselining.

Production delta files:

- `Assets/ForestPrototype/Species/BeechSpecies.asset`
- `Assets/ForestPrototype/Species/SessileOak.asset`

Verification-only delta files:

- `Tools/Calibration/OakBeechGrowth/check_production_v1.py`
- `Tools/Calibration/OakBeechGrowth/kernel_controls.py`
- `Tools/Calibration/OakBeechGrowth/production_v1_expected.csv`
- `Tools/Calibration/OakBeechGrowth/production_v1_inputs.json`

## N3 completion

- Branch: `task/scenario-one-two-cycle-completion-implementation-v1`
- Exact input: `eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac`
- Pushed branch HEAD: `eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac`
- Status: ACCEPTED IMPLEMENTATION INPUT; integrated smoke required
- Dependencies/anchors: Save20/event9; eight objectives plus reviewed distinct-year plans; old terminal outcomes retained.

Production delta files:

- `Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`

Verification-only delta files:

- `Tools/Verification/ScenarioOneCompletion/N3/N3CompletionVerification.cs`
- `Tools/Verification/ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs`
- `Tools/Verification/ScenarioOneCompletion/N3/OfflineLogic/SaveNeighbours.cs`
- `Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py`

## U4 guidance

- Branch: `task/scenario-one-two-cycle-guidance-v1`
- Exact input: `a93a7220fd8b4d8246b3613d913360dad06988e8`
- Pushed branch HEAD: `a93a7220fd8b4d8246b3613d913360dad06988e8`
- Status: ACCEPTED BOUNDED SOURCE; offline only
- Dependencies/anchors: Depends N3/history; separate presentation helper leaves rules unchanged. Do not also cherry-pick older guidance 40b0a765.

Production delta files:

- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionGuidance.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionGuidance.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`

Verification-only delta files:

- `Tools/Verification/ScenarioOneCompletion/N3/OfflineLogic/N3OfflineCheck.cs`
- `Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py`
- `Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs`

## UX

- Branch: `ux/scenario-one-small-fixes-84e881a`
- Exact input: `549eae3318f1bb4d01a2853384b58f282f7f4e73`
- Pushed branch HEAD: `None`
- Status: ACCEPTED BOUNDED SOURCE / B review; integrated rendered qualification required
- Dependencies/anchors: Original UX base 84e881a is also needed; follow-up alone is not the complete UX. P3 text AF7405918195F0DE; world EBE7A228F630AE41 unchanged.

Production delta files:

- `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss`
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`
- `Assets/ForestPrototype/UI/UiKit.cs`
- `Assets/ForestPrototype/UI/WorkPlanOverview.cs`

Verification-only delta files:

- `Tools/Verification/S1A_README.md`
- `Tools/Verification/ScenarioOneUx/Qualification/UxQualification.cs`
- `Tools/Verification/ScenarioOneUx/Qualification/make_startup_diagnostic.py`
- `Tools/Verification/ScenarioOneUx/Qualification/run_qualification.py`
- `Tools/Verification/Startup/StartupScreenVerification.cs`
- `Tools/Verification/run_s1a_gates.py`

## Marking optimisation

- Branch: `perf/scenario1-dense-stand-player-v1`
- Exact input: `678283642aaeb60221eda0fe8cb3f97e27958048`
- Pushed branch HEAD: `None`
- Status: BLOCKED PUBLICATION: local commit exists; no containing pushed ref found
- Dependencies/anchors: Containing local branch head e788b09f97958fd0d416cf4f67d63e7ef599df77; preserve Hegyi additions/order/exact cutoff. Owner must publish.

Production delta files:

- `Assets/ForestPrototype/ForestTreeMarkingManager.cs`

Verification-only delta files:

- None

## Fixture fixes

- Branch: `tools/scenario-one-verification-fixture-fixes-v1`
- Exact input: `4c5a8a4b80e2e1df4d343a2aebc21475d6f1401a`
- Pushed branch HEAD: `4c5a8a4b80e2e1df4d343a2aebc21475d6f1401a`
- Status: ACCEPTED VERIFICATION INPUT; integrated representative run required
- Dependencies/anchors: Preserve density-v3 component assertions when resolving gate overlap; observed Enlarged80 values are not canonical replacement anchors.

Production delta files:

- None

Verification-only delta files:

- `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/check_fixture_controls.py`

## feat: add oak and beech timber ladders and prices (pure layer)

- Branch: `claude/stoic-mccarthy-gtzaub`
- Exact input: `8d21e27046fdeae4b2cda710938daf19757fbc3d`
- Pushed branch HEAD: `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a`
- Status: ACCEPTED PRODUCTION INPUT; feature/integrated gates still required
- Dependencies/anchors: Exact cherry-pick delta; do not merge entire overnight branch. Depends Model2 identity/density foundation; history depends timber economy.

Production delta files:

- `Assets/ForestPrototype/TimberYield/TimberYieldCalculator.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldConfiguration.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldValidation.cs`
- `Assets/ForestPrototype/WorkEconomy/ForestryWorkDomain.cs`
- `Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs`
- `Assets/ForestPrototype/WorkEconomy/TimberAssortmentLabels.cs`
- `Assets/ForestPrototype/WorkEconomy/TimberAssortmentLabels.cs.meta`

Verification-only delta files:

- `Tools/Verification/ThreeSpecies/OfflineLogic/BroadleafTimberOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/SitkaYieldFixture.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py`

## feat: add oak and beech timber economy behind growth model 2

- Branch: `claude/stoic-mccarthy-gtzaub`
- Exact input: `855827f96225003c6387b0da43107c509603391a`
- Pushed branch HEAD: `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a`
- Status: ACCEPTED PRODUCTION INPUT; feature/integrated gates still required
- Dependencies/anchors: Exact cherry-pick delta; do not merge entire overnight branch. Depends Model2 identity/density foundation; history depends timber economy.

Production delta files:

- `Assets/ForestPrototype/ScenarioOne/ScenarioOneEconomyAdapter.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`
- `Assets/ForestPrototype/UI/WorkPlanView.cs`

Verification-only delta files:

- `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs`

## feat: add scenario-one management history, year drill-down and completion summary

- Branch: `claude/stoic-mccarthy-gtzaub`
- Exact input: `feb754cbe263c41a8d2f6fe33cd9fc39888c7e60`
- Pushed branch HEAD: `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a`
- Status: ACCEPTED PRODUCTION INPUT; feature/integrated gates still required
- Dependencies/anchors: Exact cherry-pick delta; do not merge entire overnight branch. Depends Model2 identity/density foundation; history depends timber economy.

Production delta files:

- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.History.cs.meta`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`
- `Assets/ForestPrototype/UI/WalkingHudView.cs`
- `Assets/ForestPrototype/UI/WorkPlanView.cs`

Verification-only delta files:

- `Tools/Verification/ThreeSpecies/OfflineLogic/HistoryOfflineStubs.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/UnityShim.cs`
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py`
- `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs`

## Integration selection and ownership

Use exact production snapshots/deltas, never whole branch merges that implicitly bring rejected/superseded density. Density owns AdultMortalityAccount.cs, ForestEcologyController.cs, MixedDensityModel*.cs/metas, SitkaSubsetDensitySignal.cs/meta, SitkaGrowthModel.cs, TreeSpeciesDefinition.cs and density-demand fields in the two broadleaf assets. Site owns only ForestSiteProfile/ScenarioSiteProfiles/SpeciesSiteResponse and metas (neutral infrastructure). Timber owns yield/economy/product rows and quoting; history owns derived history, read-only drill-down and summary. N3 owns completion predicate/event recording/save chronology. U4 owns presentation only. UX owns layout, inspection presentation and text. Marking owns the forecast-loop performance implementation.

Recommended order once dependencies clear: qualified density snapshots → four exclusive site commits → 8d21e270/855827f9/feb754cb → calibrated two species rows → eb7fd3d8 → A1 → original UX delta b4894cc..84e881a and follow-up 549eae33 → published marking 67828364 → A2 tool delta. A density-only production import must include Model2 identity/foundation and the exact v3 controller/model snapshots; importing merely its final calibration constant commit is insufficient. Do not select 568ba5cd as a final density implementation or adopt Model2 by default.

Expected overlaps: ScenarioOneManager (timber/history/N3), AnnualReviewView (history/N3/U4/UX), ScenarioInterventionHistory and Manager.History (history/U4), TreeInspectionView/WorkPlanView (timber/UX), Oak/Beech assets (density/growth), MixedDensityVerification (v3/A2), N3 tests/history tests (N3/U4). Semantic ownership is as above; no accepted mechanic wins by “ours/theirs”.

Reported growth qualification at 2052a4ad4a0ac381e039a4a696dece7d19f31395 is B, not unconditional anchor acceptance: Model2 completion 702766DECE591E21 → D99EC9FC2C635064; live Reference continuation 9CDF21A541C5968D → BFFF66015A6619DA; frozen archive remains exact. Existing saves use new species rows on resume. Record Manager acceptance and integrated attribution before updating any reference. U4 further changes completion timing, so growth-only hashes must not be assumed final integrated values. UX accepted P3 change is presentation-derived, independently reviewed at bf1b7e06b030218dbae530df0aed3aa87cb0a280.

Existing older guidance ba48591 adds presentation methods to ScenarioOneCompletionRules and is superseded for this packet by A1's separate helper. Its concurrent qualification work is on a different worktree/source; it cannot qualify A1 automatically. Existing fixture 3a788a52 is the provenance for A2's two gate hunks, not an additional feature to integrate.

STOP final integration for missing exact verdict/source publication; STOP qualification/publication for unexplained anchor movement, any change to save20/event0–9/defaults/eight objectives/frozen Reference, or conflicts requiring a gameplay decision. A partial rehearsal can capture available source interactions; it cannot clear these holds.

## Located Manager anchor policy

Published evidence at tools/verification-fixture-fixes-v1 @ 3a788a52def754edd22e322f79f2faf3c63b6fa1, Docs/Verification/AnchorPolicy/ObservedIntentionalAnchorDeltas.md, records Manager policy of 2026-10-14: retain historical assertions now; establish any new expected values on the integrated candidate; frozen archive is not rebaselined. Accordingly growth is an accepted ordinal implementation input with B caveats, not a demand to pre-approve growth-only final hashes before candidate creation. No hash replacement is authorised by this inventory. Save-resume and promotion caveats remain disclosed for Manager review.
