# Sitka site / height / adult mortality diagnostics

Disposable; production code is never modified.

1. Copy `SitkaGrowthMortalityDiagnostics.cs` into `Assets/ForestPrototype/`.
2. Run Unity 6000.6.0f1 with `-batchmode -nographics -projectPath <checkout> -executeMethod SitkaGrowthMortalityDiagnostics.Begin -logFile <log>` and the environment:
   - `CCF_DIAG_DIR=<output folder>`;
   - `CCF_SITKA_YEARS=100` (default 100);
   - optionally `CCF_SITKA_MODE=timber` for the timber-impact quotes only.
3. Remove the copy and its `.meta` from `Assets/ForestPrototype/`.
4. Run `python3 height_candidates.py <output folder> <height_candidates.csv>` for the offline height candidates.

Outputs: `stand_development.csv`, `tree_snapshots.csv` (4 MB; SHA-256 `e05d805b…` for the recorded run), `run_hashes.csv`, `performance.csv`, `facts.txt`, `timber_impact.csv`.

The recorded run took about 5 minutes for 26 × 100-year runs. Summaries are in `Docs/Research/SitkaGrowthMortality/Evidence/`.
