# Understorey recruitment diagnostics

Base/canonical context: a8596df; audit commit 5c389cc. These are explicit disposable measurement scripts, not production simulation changes. Run from the task checkout with Unity closed. One launcher at a time. Unity 6000.6.0f1 path is configured in the launchers; adjust that path only for the installed Editor on another host.

```bash
python3 Tools/Verification/UnderstoreyRecruitment/run_diagnostics.py
python3 Tools/Verification/UnderstoreyRecruitment/run_diagnostics.py --skip-inventory
python3 Tools/Verification/UnderstoreyRecruitment/run_diagnostics.py --fixtures
python3 Tools/Verification/UnderstoreyRecruitment/run_diagnostics.py --performance-only
python3 Tools/Verification/UnderstoreyRecruitment/run_diagnostics.py --visual
python3 Tools/Verification/UnderstoreyRecruitment/candidate_sweep.py
python3 Tools/Verification/UnderstoreyRecruitment/analyse_evidence.py
python3 Tools/Verification/UnderstoreyRecruitment/run_regression.py
```

First invocation inventories actual imported Assets and runs gap fixtures/32 baseline worlds. `--skip-inventory` reruns biological diagnostics using existing inventory. `--fixtures` covers single-cell accounts, recovery, simulation-only benchmark with juvenile load and 16 mixed juvenile/protection/timing worlds. `--visual` launches an interactive Editor/Game view, stages synthetic patches, captures three resolutions and isolated species/planting/thinning/mortality views. Visual success means capture execution completed; adequacy is assessed manually in the reports. No command implements competition.

The wrapper stages one source under Assets/ForestPrototype and removes that source/meta in finally after the Editor exits. It uses a per-task config, one nonblocking file lock and a systemd user scope with 8G memory / 256M swap limits and two workers. Do not share/copy Library. Results/logs/raw captures are under ignored Build/UnderstoreyRecruitment. `analyse_evidence.py` copies useful CSV evidence, joins inventory metadata and compresses the 43,200 virtual candidate rows. Run it only after a successful inventory and baseline. It deliberately excludes camera PNGs and regression result publication; review those before copying.

The regression launcher retains source SHA256/ref and batch versus interactive mode. Three final verification sources are read from commit 1a36ea9, the gate-contract correction, while production stays a8596df. That commit must be available in local Git objects; no checkout/cherry-pick is performed. Immutable Reference anchors are checked explicitly, and a Reference failure stops the sequence. No PASS means tested candidate ecology: all these gates concern unchanged production. Review individual logs after any failure.

Diagnostics do not write disk save slots. Legacy gates have their own restore contracts; use the isolated config and inspect their result markers. After all Editors exit, inspect Git differences and restore only known Editor-generated tracked material/settings normalisation in the otherwise fresh owned checkout. Never overwrite someone else's changes. Exclude generated caches and staged Assets scripts from commits.

## Manager continuation

`survival_forms.py` writes the reduced four-form survival-only synthetic matrix into Evidence/Continuation. It does not change biology. The continuation regression launcher now retains new outputs separately under Build/UnderstoreyRecruitment/Continuation/regression and appends TimberYield, WorkEconomy, historical canonical lifecycle (BeginCanonical) and the real natural/planted display-height fixture to the original19. Production differs only by renderer-height normalization; no save/model change. Historical19-gate evidence remains untouched.

A targeted rerun uses `run_regression.py --gate JuvenileDisplayHeightVerification`; its separate results file preserves the full-run history. Current corrected fixture measures45 natural height cases plus30 paired Oak/Beech comparisons; Sitka is not stocked by the current planting shop and has no configured planted renderer. The initial unsupported fixture failure is disclosed in ImplementationRecord.md and retained in the continuation evidence.
