# D-073 year-comparison text mapper — offline checks

Tooling for issue #415 (task T-03 of `specs/scenario-one-multiyear-decision-outcome-feedback-v2`). **Offline only: Unity is
not started and no save file is read.** A PASS here proves the mapper's wording and rules in isolation. It proves nothing
about Unity compilation or import, rendering, layout, real saves or the Annual Review wiring (task T-04).

The source under test stays in `Assets/ForestPrototype/ScenarioOne/ScenarioOneYearComparisonText.cs`. Nothing in this
folder belongs under `Assets`.

## What is compiled

- the REAL mapper `ScenarioOneYearComparisonText.cs`;
- the REAL read-only helper `ScenarioOneManagementFollowonReadout.cs` (unchanged);
- `YearComparisonVerification.cs` — the fixture and its assertions;
- `OfflineProvenanceTypes.cs` — a test double of the `HistoryProvenance` enum;
- `../ManagementFollowonReadout/OfflineContractTypes.cs` — the helper fixture's test doubles of the history types
  (referenced, not edited).

Histories are built from the doubles, passed through the helper's real `Build`, then through the mapper, and the card is
compared with exact text. A few defensive paths use a hand-made `Result`.

## Run

From the repository root (dotnet SDK with the net8.0 runtime needed; `-I -B` keeps Python from writing `__pycache__`):

    python3 -I -B Tools/Verification/YearComparison/run_offline.py

The launcher writes a throwaway project **outside the repository**, runs `dotnet build`, then runs the built fixture, and
prints both exit codes (`dotnet build exit=…`, `fixture run exit=…`). Its own exit code is the first non-zero of the two.
`--out <dir outside the repo>` keeps the project, `build.log` and `run.log`. `--group <name>` runs one group only.
If dotnet is absent it prints `NOT RUN` and exits non-zero; do not read that as a pass.

Groups: `nocard`, `latest`, `full`, `missing`, `precedence`, `nowork`, `yearfilter`, `ambiguous`, `linkage`, `species`,
`units`, `stable`, `wording`. The `wording` group also sweeps every card the earlier groups produced, so it is most
useful in a full run.

## What the fixture covers

No completed year; latest-year line; full comparison with exact text, order, units and signs (including negative
differences and canopy fraction ×100); selected snapshot missing; latest snapshot missing; both missing; a snapshot
dated for another year; CannotCompare ahead of missing-snapshot lines; no work rows; only viewed-year rows counted;
created/approved orders absent; ambiguous-only, unresolved-only and both on one row; MissingOrder and InconsistentOrder
warning; cancellation without warning; species missing on the selected side and on the latest side; four separate
species measures; data-source label on every row; no currency, money, causal or out-of-scope wording; the helper's
coverage groups and candidate years never shown; repeat calls identical; input unchanged.

## Guard-removal (mutation) checks

    python3 -I -B Tools/Verification/YearComparison/run_mutations.py --out <NEW directory outside the repo>

Each mutation is a copy of the mapper with one rule removed, kept only in the output directory with its diff, build log
and run log. The copy must compile and the fixture must then fail on the assertion that guards the rule. "KILLED" means
the fixture caught the broken copy; it does not mean gameplay was qualified. The production source is never rewritten.

## Second gate: production signatures

The doubles are not the real history types. Real binding is checked by compiling the real runtime sources:

    python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src <project-root> --out <external dir>

That script's own exit code is always 0; read the `exit=` and `errors=` values it prints (the underlying `dotnet build`).
It is a .NET type check against built Player assemblies, **not a Unity compile**.
