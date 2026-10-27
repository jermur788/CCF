# FAIRNESS ORACLE HANDOFF

Date: 2026-10-15. Worker: SOL/CODEX, verification-harness repair. Tier 1 + narrow Tier 2.

- Branch: `fix/density-v3-canopy-pulses-v1`
- Base: `e5cfce3702aa746dab0d4eb66a78aa8c3ec67694`
- Exact HEAD: `e5cfce3702aa746dab0d4eb66a78aa8c3ec67694`
- Context: `c6dc136b7f1f8668d9f67f95f48968d290ad5067`
- Worktree: `/media/jer/Files/CCF-a1-density-e5cfce3`
- Publication state: repaired harness and this receipt are **uncommitted working-tree changes** on that HEAD. HEAD alone does not contain the repair. No commit/push was requested.

## Harness root cause

The previous exclusion predicate was `GenericBudgetPassedOver > 0 || GenericBudgetLimited > 0`. Every one of the 150 trials in each dense design met it. Thus `freeObs`, `freeExp`, and `freeVar` remained zero. The code assigned `pooledZ = 0` when variance was zero and then asserted `abs(pooledZ) < 4`, issuing an invalid green count PASS.

The expected-count model was also stale. It summed `AnnualLossProbability(loss, suppression, meanVulnerability)` and independent per-stem Bernoulli variances. The component planner instead computes `E = genericLossFraction * eligibleCount`, rounds `E` with one keyed fractional Bernoulli, and chooses a weighted sample without replacement via an exponential priority race. Its unbudgeted total count has variance `frac(E) * (1 - frac(E))`, not the sum of independent stem variances. Individual inclusion probabilities are not the old clipped per-stem probabilities. Applying an overall ratio to those probabilities does not repair the species z-test under budgeting.

Filtering samples by *realised* budget binding also conditions on the outcome. Statistical applicability is now determined before the draws: even the largest possible rounded quota of the largest eligible stems must fit the budget.

## Oracle change

`Tools/Verification/ThreeSpecies/Qualification/DensityQualification.cs` now independently reconstructs:

1. Class-weighted occupied space and generic loss, conditional on the existing Sitka component's exclusions.
2. Expected count, fractional quota draw, common species-neutral competitive vulnerability, id-keyed exponential priority and ordinal-id tie break.
3. The priority walk taking fitting stems up to quota; its passed-over count and quota shortfall; the explicit highest-priority never-stall fallback only when no eligible stem fits alone.

It asserts the exact chosen set, budget compliance, quota, counters and priority-maximality. If quota is not filled, no remaining eligible stem may fit the remainder. This is the defined greedy priority-maximal contract, not a maximum-cardinality knapsack guarantee. A fitting higher-priority stem cannot be replaced by a lower-priority one. The independent expected allocation must match every live controller victim position, and therefore each species' accumulated count, over all trials.

Equal-DBH demand labels are cyclically permuted among eligible stems while preserving aggregate occupied space and the fixed Sitka exclusions; exact allocation must be invariant. Both DBH designs also reverse storage order with indices remapped; id-keyed victim identity must be invariant. Species are rotated through positions over the 150 seeded trials. Conditional allocation uses no species/demand weight; demand affects the upstream pressure as intended. Unequal sizes can legitimately affect fitting and therefore species totals: those are checked against the exact conditional contract, not equal-count expectations.

All dense trials receive this hard conditional oracle. Their unbudgeted count statistic is explicitly `QUAL_NOT_APPLICABLE`, with no fabricated z-score. A 30-trial non-binding live-controller control at occupancy approximately 1.05 is added within each requested phase. Its rounded-quota count test must have at least 30 preclassified samples, positive variance and `|z| < 4`; its coverage is a hard gate. These controls reuse the same seeds across FAIRNESS/FAIRNESSHIGH and are not pooled as independent evidence.

`run_qualification.py` adds `--license-dir` to copy a selected licence source into the existing isolated config. The native-config licence expired during the day; the user renewed through Flatpak Unity Hub. Three startup attempts using the old native licence exited 198 before compilation/trials. The successful runs used the renewed Flatpak source and separate new output/config directories.

## Production files changed

**NONE.** `git diff -- Assets/ForestPrototype` is empty after disposable harness cleanup. `GenericAnnualBasalAreaBudgetShare` remains **0.125**. No production, scene, asset, package, ProjectSettings or save-schema change.

An unrelated `.vscode/settings.json` solution-name/trailing-comma normalization was observed before the successful runs and preserved; it is not part of the repair.

## Harness self-tests

Eight hard self-tests PASS in each geometry:

- Correct allocator passes a fixture whose oversized highest-priority stem must be skipped for smaller fitting stems.
- Same-count/same-BA allocator replacing a higher-priority victim with a lower-priority stem fails.
- Allocator ignoring the BA budget fails.
- Allocator always taking the first oversized stem despite a fitting alternative fails.
- Empty allocator fails when valid victims exist.
- Correct never-stall fallback passes when no eligible stem fits alone.
- Zero samples, fewer than 30 samples and zero variance return NOT APPLICABLE, never statistical PASS.
- The count verdict rejects a deliberately biased count and accepts its positive control.

## Runtime results

Unity 6000.6.0f1, batch/nographics, only `CCF_QUAL_PHASES=FAIRNESS,FAIRNESSHIGH`; one Editor at a time. The existing 12 one-year suppression checks inside FAIRNESS also passed (AUC L40 0.628; E80 0.608). No other phase was requested or run.

Each dense row below evaluates **150/150** allocations, excludes **0**, has **150 budget-binding** trials, and reports unbudgeted count statistics **N/A 150/150**. All per-position residuals are exactly zero; species arrays are Sitka/Beech/Oak and equal independent conditional expectations exactly.

| Phase | Geometry | Dense design | Generic victims | Actual = expected by species | Allocation |
|---|---|---|---:|---|---|
| FAIRNESS | Legacy40 | Equal DBH | 2,400 | 805 / 783 / 812 | PASS |
| FAIRNESS | Legacy40 | Unequal DBH | 2,791 | 945 / 893 / 953 | PASS |
| FAIRNESS | Enlarged80 | Equal DBH | 9,900 | 3,305 / 3,229 / 3,366 | PASS |
| FAIRNESS | Enlarged80 | Unequal DBH | 11,058 | 3,717 / 3,603 / 3,738 | PASS |
| FAIRNESSHIGH | Legacy40 | Equal DBH | 4,650 | 1,545 / 1,503 / 1,602 | PASS |
| FAIRNESSHIGH | Legacy40 | Unequal DBH | 5,113 | 1,689 / 1,648 / 1,776 | PASS |
| FAIRNESSHIGH | Enlarged80 | Equal DBH | 18,600 | 6,213 / 6,094 / 6,293 | PASS |
| FAIRNESSHIGH | Enlarged80 | Unequal DBH | 20,443 | 6,843 / 6,774 / 6,826 | PASS |

Each phase/geometry additionally evaluates **30/30** non-binding live trials, excludes **0**, count N/A **0**, and passes exact conditional allocation, label/storage-order invariance and rounded-quota count:

| Phase | Geometry | Deaths | Expected | Rounding variance | z | Count |
|---|---|---:|---:|---:|---:|---|
| FAIRNESS | Legacy40 | 4 | 5.9735 | 4.7689 | -0.90 | PASS |
| FAIRNESS | Enlarged80 | 14 | 17.0809 | 7.3450 | -1.14 | PASS |
| FAIRNESSHIGH | Legacy40 | 4 | 5.9735 | 4.7689 | -0.90 | PASS |
| FAIRNESSHIGH | Enlarged80 | 14 | 17.0809 | 7.3450 | -1.14 | PASS |

Overall allocation coverage: **1,320 evaluated, 0 excluded**, including 1,200 binding and 120 non-binding executions. Non-binding controls repeat across phases; independent sample size is 30 per geometry, not 60. No fallback occurred in dense runtime trials; fallback was exercised by the self-test.

Both geometry processes exited **0**, emitted `DENSITY_QUAL_PASS`, passed **37 hard checks each**, had zero compiler errors and zero unexpected tree changes. Elapsed wall times: Legacy40 **185.0 s**, Enlarged80 **699.9 s**.

Residual warnings: existing obsolete Unity object-finding APIs; non-blocking access-token refresh message (renewed offline entitlement resolved successfully); one `UnityEditor.Search.SearchDatabase.EnumerateAll` startup `ArgumentOutOfRangeException` per process, matching the earlier A1 `hard-e80.log` signature. No fairness failures or new runtime exception signature.

## Reproduction and evidence identity

From the worktree, run this command once per geometry, substituting `0`/`1` and a fresh output directory/label:

```sh
python3 Tools/Verification/ThreeSpecies/Qualification/run_qualification.py --label fairness-l40-valid --out /tmp/opencode/ccf-fairness-repair/l40-valid --license-dir /home/jer/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses --method DensityQualification.Begin --file Tools/Verification/ThreeSpecies/Qualification/DensityQualification.cs --file Tools/Verification/ThreeSpecies/Qualification/DensityProbe.cs --marker DENSITY_QUAL_PASS --env CCF_STAND_GEOMETRY=0 --env CCF_QUAL_PHASES=FAIRNESS,FAIRNESSHIGH --timeout 1500
```

Successful local raw logs, marker receipts and result JSON:

- `/tmp/opencode/ccf-fairness-repair/l40-valid/`
- `/tmp/opencode/ccf-fairness-repair/e80-valid/`

Both result JSON files record run HEAD `e5cfce3702aa746dab0d4eb66a78aa8c3ec67694` and identical staged source SHA-256:

- `DensityQualification.cs`: `67554a877b437fed97d7f2e33d3821b2161e587f76fe8ca4dfd5235b2c225fb3`
- `DensityProbe.cs` (unchanged): `69881656c5a3c8682fd0fbaa464fc4a6ab4949d0422ce93af86bb5e2f571fbe2`
- Runner SHA-256: `2579856015801a7945360cb95c128be23b588ad0cbb0b95b2985c101215d3532`

The source hash, rather than unchanged HEAD alone, identifies the tested uncommitted harness. `git diff --check` passes. Disposable scripts and their `.meta` files were removed by the runner.

## Verdict

- Zero-sample false-pass prevented: **YES**.
- Overall: **A** — fairness is substantively validated under the inactive and binding budget contracts; no production change required.
- Density production recommendation: **ACCEPT FOR INTEGRATION**.

Prior known-good A1 evidence remains valid: the production bytes are unchanged. The Beech pulse reduction, all four 300-year BA-budget/flattening/no-collapse fixtures, pure-Sitka exactness, shielding, continuity and save/load results are reused without repeating those gates. The density fix is ready for integration together with this tested harness repair. The repair remains in the named working tree pending commit/publication by the authorised owner.
