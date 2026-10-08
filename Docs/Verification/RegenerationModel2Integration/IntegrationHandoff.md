# Regeneration Model 2 integration handoff

Manager accepted the central Scenario One v1 calibration on 8 October 2026. The source branch was reproduced before integration, then fast-forwarded into a fresh clean ZX20 checkout. No calibration parameters or biological height rules changed during integration.

| Item | Result |
| --- | --- |
| Source | task/understorey-recruitment-causality @ 902903ff6d9790c02bdd01e8cc6f0aab8c2f0774 |
| Source implementation/evidence | 49b01d8fc90ea0779ebfdc61a5a421af718da1d6 |
| Pre-merge main | a8596df9c52669a36709f09e85dfe6568640af49; fetched and identical to source base, no intervening ecology divergence |
| Integration implementation | 62593b28e14a5b5818c414450f168c333df683d7 |
| Context | 62593b28e14a5b5818c414450f168c333df683d7 |
| Merge method | Fast-forward source into integration/regeneration-model2, followed by bounded integration fixes, verification evidence and canonical commits; normal fast-forward publication to origin/main |
| Save/model | Save18; new Scenario One regeneration2; existing v17 regeneration1; immutable Reference0. Exactly five approved added fields |
| Calibration | Linear shared Sitka/Oak/Beech survival loss, strength .35 [C], max(bramble,bracken), full vulnerability through .3m and zero at 1.5m [C]; initial .25 × target [C]; target .8 × clamp01(siteProductivity × soilStability) × (.1 + .9 × clamp01(light)) [C/I]; bounded gain .30/loss .45 per year [C/D] |
| Beech display | Accepted renderer fix retained; authoritative .6m natural and planted heights display approximately .6m without biological-height changes; display gate PASS |
| Model2 anchors | Start FA855239CDDA32D8; year1 02334804F65C0234 |
| Historical anchors | BFC55473C1506067 / 3485B6630C9EA448; regeneration1/growth0 962846D2F517B293 / FBB8F470D85FF815; historical v17 model1 completion D7C4DDD36B53FCCE |
| Reference | PASS; immutable archive year100 7AD177B3CC2F73C7 and continuation historical-v15-layout 9CDF21A541C5968D unchanged |
| Completion | New-game model2 702766DECE591E21 unchanged; separate forced legacy model1 completion PASS |
| Economy | Integration economy gate PASS (120 checks), prices/rules unchanged; reviewed 39 paid century worlds retained as source evidence |
| Performance | Fresh clean-integration annual and patch-history measurements PASS; see retained CSVs and measured limits below |
| Regression | Pushed source critical preflight: state1,396 plus seven gates PASS. Clean integration: state1,398 and all24 regression gates PASS |
| Pedagogy | Approved concise clearance meaning rendered/readable at1280×720,1600×900,1920×1080. Older models retain no-competition truth; model switch refresh works without lesson progress change; forest/lesson read-only checks PASS |
| Canonical | Accepted D-049; current milestone, project overview, research index and live coordination updated. Source trial records retain historical provenance with acceptance notice |
| Project context | Twelve source mirrors and two role composites generated from one committed full context SHA; manifest verified and retained. Outputs Build/ProjectContext |
| Drive | Pending; platform attachments pending |
| Dirty CCF-main | Preserved at402a2b41108dc9aa91e225db9047956b0a91a97e; all11 existing changed/untracked file hashes and status unchanged |
| Worktree | /media/jer/ZX20/CCF-integration-model2, content-clean after final commits; disposable verification source/meta removed |
| Status | REGENERATION MODEL 2 INTEGRATED; final publication tip recorded in delivery message |

The learning panel now states: “Dense bramble or bracken can reduce the survival of small young trees. Clearing can reduce that competition, but it also removes young trees already growing inside the treatment area, and the vegetation can return.” Models0/1 instead explicitly state that clearance does not alter young-tree survival/growth in that saved forest. The layout uses the existing scrollable topic and preserves objective IDs and persistence.

A focused integration review found that historical hash-field stripping did not consume a negative scientific exponent (e.g.1E-10). The retained failing test demonstrates the issue; the corrected complete JSON-number pattern now passes two additional checks. This affects historical hash normalization only, does not alter saved state or ecology, and leaves all accepted hashes unchanged. State check source hashes describe the disposable harness; the separate production-source SHA256 manifest records the compiled production files.

The original39×100-year worlds,11,700 species rows, storage measurements, sensitivity work and ecological clearance pairs were already reviewed and are preserved in Docs/Research/UnderstoreyRecruitment/Evidence/Model2. They are not relabelled as fresh integration runs. The clean integration repeats critical gates, complete regression, state, performance and rendered pedagogy. Four P0 harnesses use the exact1a36ea97c724b3a026e76a0b5432349b46e6a298 sources; their synthetic v5 fixture adjustment is disclosed in regression JSON. No verification script is delivered under Assets.

Clearance remains a paid conditional action: it can improve recruitment, waste money or destroy useful juveniles. Targeted clearance can outperform blanket clearance. The useful dense target trial cost€36 and produced29 extra cumulative Sitka promotions; century cash was€36 lower and basal area lower, so increased recruitment is not claimed as timber profit. No fixed treatment interval is recommended.

These are accepted gameplay calibrations, not measured Irish annual mortality/recovery or validated botanical productivity equations. Equal category trajectories are an abstraction. The target is limited to Scenario One; it must be reassessed before other soils, site classes, climates, scenarios or vegetation types. Separate vegetation/light/browse cause accounts remain intact. Total habitat cover is non-causal.

Open, nonblocking: independent botanical bramble/bracken responses; bramble browsing concealment; competitive graminoids; source/neighbor spread; stronger site/moisture calibration; target generalization beyond Scenario One; player visual recognition; human clearance-decision playtest. Rendered automated UI review does not constitute human acceptance of plant recognition.

To reproduce, close Unity and use this integration checkout. Run python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py --gate Model2Verification; python3 Tools/Verification/UnderstoreyRecruitment/run_regression.py; python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py --gate Model2Performance. Set CCF_REGEN_MODEL=1 for the separately recorded ScenarioOneCompletionVerification gate. The launchers use isolated verification configuration and one Editor at a time. Play a fresh Scenario One and open O→Vegetation clearance to review the approved explanation; load an older model1 save to review its accurate legacy explanation.

## Clean integration timings

Simulation-only annual medians, three measured repetitions after the initial warm-up; each workload has192 natural bands and30 exact planted juveniles. Negative differences are timing variation, not evidence that competition improves performance.

| Adults | Patches | Model1 ms | Model2 ms | Difference |
| --- | --- | --- | --- | --- |
| 336 | 0 | 36.45 | 37.10 | +1.78% |
| 336 | 12 | 36.64 | 36.87 | +0.62% |
| 1300 | 0 | 266.21 | 268.91 | +1.01% |
| 1300 | 12 | 267.38 | 264.39 | -1.12% |
| 3000 | 0 | 732.36 | 741.12 | +1.20% |
| 3000 | 12 | 738.04 | 740.48 | +0.33% |
| 5000 | 0 | 1457.02 | 1462.95 | +0.41% |
| 5000 | 12 | 1450.82 | 1473.19 | +1.54% |

| Patch layout | Patches | Median cache build ms | Median10k queries ms | Nodes visited |
| --- | --- | --- | --- | --- |
| distributed | 100 | 5.21 | 1.56 | 5795 |
| distributed | 1000 | 31.29 | 4.27 | 36266 |
| distributed | 5000 | 308.52 | 12.57 | 127961 |
| overlapping | 100 | 6.22 | 5.86 | 67246 |
| overlapping | 1000 | 92.92 | 7.84 | 92587 |
| overlapping | 5000 | 517.51 | 10.11 | 118603 |

Large overlapping histories still have a measurable cache-build cost. No theoretical logarithmic worst-case or frame-rate guarantee is claimed. Reviewed storage measurements remain unchanged: actual64-cell/one-patch pretty save adds6,998 bytes (2.34%); compact adds4,640 bytes. No new schema fields were added at integration.
