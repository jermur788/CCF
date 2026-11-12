# Part 5 — Management Follow-on Readout isolated tests

**#406: SOURCE QUALIFIED OFFLINE; Unity NOT RUN.** See the SOL result handoff for exact source heads, commands and limits. Browser #400 originally supplied uncompiled source. These files are optional test tooling only. Do not place OfflineContractTypes.cs in Assets; its type declarations are intentionally minimal test doubles. The actual source remains in Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs.

## Safe offline checks (future .NET-capable worker)

From repo root:

    python3 -I Tools/Verification/ManagementFollowonReadout/run_offline.py

Requires dotnet SDK (net8.0). The script writes a temporary project **outside the repository**, compiles the actual production helper against the minimal signature-only adapter and executes the C# fixture. A PASS asserts only the readout's logic against the adapter, **not production type compatibility or Unity**. An absent compiler prints NOT RUN, exits nonzero. Do not reinterpret a printed expected token as actual execution without an attached command result and process exit.

The fixture covers: no completed/snapshot year; wrong year; no future observation; exact 5→9 year delta; duplicate completed year; Sitka/Oak/Beech and missing species; distinct natural origin model cell counts/density versus planted cohort cells and exact juveniles; duplicate species IDs; per-year+prescription+coverage groups; succeeded/failed/pending/orphan orders; rack-owned access vs possible non-rack stand felling; planting; cancellation-only event; old no-prescription; duplicate event/order IDs; inconsistent resolvedYear; grouped-harvest money flag with no per-tree allocation; no-work year; stable repeat and no source mutation.

## Required second gate (production signatures, not substitutable by stubs)

Use the established no-Unity source typecheck from a clean C# worktree with a known managed assembly set, adding this new helper automatically because it lives under Assets:

    python3 -I Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src <project-root> --out <disposable-external-dir>

That checks production runtime type bindings, but still does not run Unity. The source-only helper is then available for a targeted Unity/Assembly-CSharp import check during the future qualifying lane after the separate #397 source branch is integrated under Manager authority. Do not run simultaneous Unity Editors, use the protected user checkout, alter existing fixture assertions, or claim full scenario parity from these isolated checks.

## Readout contract and exclusions

Input: already-built IReadOnlyList<YearHistoryView> with exact completed-year semantics; no manager live-state access. Output: copied scalar stand/species observations, exact selected/latest comparison statuses, recorded work with verified/ambiguous/missing historical links, executed coverage groups and advisory stand-fell candidate years. No new save/history/goal, world hash, UI, annual step or managed forest action. Source rack cannot be inferred from area ID text; past per-rack ecology does not exist in annual stand snapshots. The future D-073/D-074/D-076 player features are separate four-file DRAFT specs, not authorised by this runner.

## #406 provenance qualification

The original 37 checks remain; fixtures now supply explicit recorded tree targets and annual settlement metadata for positive claims. The adapter additionally mirrors production Events, target/species, Report and HarvestSettlement signatures. The complete suite has 53 assertions, including collisions with OrderCreated/YearAdvanced, mismatched targets/species, absent historical tree identities and fell/salvage with missing annual report or settlement. No genuine old save is loaded: legacy cases are synthetic missing-field/no-prescription records.

`GroupedHarvestMoney` now means the supplied completed-year view has BOTH its report and grouped harvest settlement, and this is a successful harvest task. It exposes no allocation to an order. Missing tree targets cannot prove a verified tree-work link; available conflicting species are rejected. Non-rack prescription felling remains advisory candidate evidence only.

Reproduce the two bounded guard-removal checks (use a fresh external output directory):

    python3 -I Tools/Verification/ManagementFollowonReadout/run_mutations.py --out /media/jer/Files/CCF/Verification/<new-mutation-output>

Each copied mutant must compile, then fail the expected assertion. The tool retains exact mutation diffs, build logs and process exits outside the repository and never rewrites production source. Mutation success means the broken variants were caught, not that gameplay was qualified. Production source plus fixture also typechecked against actual runtime classes (without OfflineContractTypes.cs); no Unity Editor was used.
