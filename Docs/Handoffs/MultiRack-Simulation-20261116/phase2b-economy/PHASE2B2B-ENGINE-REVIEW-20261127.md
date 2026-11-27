# Phase 2B-2b independent engine review

- Candidate and handoff commit: `b298fe1da4b972ba509624f63b0d888a3ca1d355` on `feat/multi-2d-phase2b2b-felling-dispositions-engine-20261127-v1`.
- Engine commit: `1a1ee15193eb59eff72fed1783a12bd6efa9e35a`. Test commit: `fcd7541ff35d6e8d09851f410a263614e116a087`.
- Approved base: `fa69d6d3d8c1789af531b53adefb5f426e90bbed`. The fetched `origin/integration/2d-sim` matched before and after verification. The fetched `origin/main` remained `bdeb6a324acaf4129c10fb0846faac8b64974360`.
- Review branch: `review/multi-2d-phase2b2b-felling-dispositions-engine-20261127-v1`, created in the dedicated clean reviewer checkout.
- Verdict maps to **NEEDS CHANGES** in AgentWorkflow. Two blocking findings, one should-fix finding and two notes.

## Verification commands and results

Ran `git config core.autocrlf false`, `git fetch origin --prune`, `git rev-parse origin/integration/2d-sim origin/main origin/feat/multi-2d-phase2b2b-felling-dispositions-engine-20261127-v1`, and `git switch -c review/multi-2d-phase2b2b-felling-dispositions-engine-20261127-v1 b298fe1da4b972ba509624f63b0d888a3ca1d355`. `git status --short` was empty. `git diff --name-only origin/integration/2d-sim...b298fe1da4b972ba509624f63b0d888a3ca1d355` showed 12 allowed paths: four new disposition engine files, three existing economy files, four focused test files and one handoff. `git diff --check` against the base exited 0 with empty output.

From `Tools/MultiRackSitkaP1Lab`:

| Command | Observed result |
| --- | --- |
| `bash build/build-p0-core-ref.sh` | Exit 0. P0 reference built, SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. |
| `dotnet --version` | `8.0.133`, also confirmed before invoking the build script. |
| `dotnet restore tests/Core/EconomyDispositions/SitkaP1.EconomyDispositions.Tests.csproj` | Exit 0. Restored the focused runner and Core. |
| `dotnet build tests/Core/EconomyDispositions/SitkaP1.EconomyDispositions.Tests.csproj --no-restore` | Exit 0. Build succeeded with 0 warnings and 0 errors. |
| `dotnet run --project tests/Core/EconomyDispositions/SitkaP1.EconomyDispositions.Tests.csproj --no-build --no-restore` | Exit 0. `18/18 passed, 0 failed`. Mixed-plan SHA-256 `d9c7aeddf2eb629b388c6cc3dfa93d5a55bd27c85e6262b323ddcbe6f14498ed`. |
| `dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj` | Exit 0. Build succeeded with 0 warnings and 0 errors. |

### Independent default-path comparison

Ran `git -c core.hooksPath=/dev/null worktree add --detach /tmp/opencode/ccf-2b2b.AnNJsw9b/base fa69d6d3d8c1789af531b53adefb5f426e90bbed`. From the detached base lab directory, ran the pinned-SDK check, `bash build/build-p0-core-ref.sh` and `dotnet build src/Core/SitkaP1.Core.csproj`. The base build exited 0 with 0 warnings and 0 errors.

From `/tmp/opencode/ccf-2b2b.AnNJsw9b/probe`, built the same throwaway probe against each engine DLL with `dotnet build -p:EngineDir=<engine-bin-dir> -o ../base-probe` and `dotnet build -p:EngineDir=<engine-bin-dir> -p:Candidate=true -o ../candidate-probe`. The probe used the existing `SitkaP1.Core.Tests` friend assembly name and called the base and candidate without modifying either engine. It opened the mixed demo with default Economy options, marked MA-B-001, MA-B-002 and MA-B-003, then called Quote, Approve, Execute and Advance. It printed actual engine outputs with no numeric expectations. Ran the two DLLs with `dotnet ../base-probe/SitkaP1.Core.Tests.dll > ../base.txt` and `dotnet ../candidate-probe/SitkaP1.Core.Tests.dll > ../candidate.txt`. `diff -u ../base.txt ../candidate.txt` exited 0 without output. Both output files had SHA-256 `84217807ae0fa257eaf09b10ed377dd3ca114f947832340ff4af9d16ea195120`.

| Measured field | Base and candidate |
| --- | --- |
| Revenue / external cost / net cash delta | 15,727 / 250,000 / −234,273 cents |
| Upfront cash required / owner minutes | 250,000 cents / 0 minutes |
| Stage / rate | FirstThinning / 2,100 cents per tonne |
| Variable cost / floor / floor top-up | 4,465 / 250,000 / 245,535 cents |
| Stem / sale / residual volume | 2,389,104 / 2,304,305 / 84,799 cm³ |
| Total green mass | 2,126,303 g |
| Sitka / oak / beech graded stem volume | 1,330,285 / 292,613 / 766,206 cm³ |
| Quote and settlement ledger | 9 lines each. Categories, references, amounts and ordering matched. |
| Cash before / after | 1,200,000 / 965,727 cents |
| Year-end summary | Year 0 → 1. Same cash/cost/revenue/net, 0 owner minutes used, 2,400 remaining and after reset, 1 harvest plan and 1 settlement. |

Ordered ledger in both engines: HarvestingAndForwarding `harvest-variable` −4,465, MinimumJobAdjustment `harvest-minimum-floor` −245,535, then TimberSale entries `beech:SecondPlanking` 1,519, `beech:Firewood` 1,458, `sessile-oak:Fencing` 2,438, `sessile-oak:Firewood` 93, `sitka-spruce:Stake` 166, `sitka-spruce:Pallet` 1,367 and `sitka-spruce:Sawlog` 8,686. Full assortment rows also matched, including unit prices, piece counts, volumes and masses. Removed the temporary base checkout with `git worktree remove /tmp/opencode/ccf-2b2b.AnNJsw9b/base` and confirmed its absence using `git worktree list --porcelain`.

Static review of the full economy diff agrees for valid legacy callers. The tree ordering, D-062 call, full-stem mass, rounding, cost categories, Sell-only ledger order, non-harvest paths, owner allowance and annual summary calculation are preserved. In the default branch, the list given to `Summarize` contains the same yields in the same order (`EconomyQuoteCalculator.cs:248–330`). The unchanged Phase 2B-2a UI maps explicit DTO fields (`src/Ui/MultiApi.View.cs:587–688`), so the additive engine members do not enter those payloads automatically. Full UI execution is **NOT TESTED**.

### Reviewer probes and mutation checks

Ran `dotnet ../candidate-probe/SitkaP1.Core.Tests.dll --edges` from the scratch project. With 249,999 cents, the quote and Approve refused all three dispositions at the 250,000-cent threshold. For each disposition, a fault before publication left the approval, cash, ledger, settlements and retained balance stable. Retry succeeded, and replay with the original source returned the prior receipt with one settlement. Plans marking the whole MA-B area as all Keep or all Leave each refused. Retained oak measured `0.292613 m³` after Advance. Cross-area and mutable-view results are below.

Generated one unchanged compiler input and three substituted inputs under `/tmp/opencode/ccf-2b2b.AnNJsw9b/mut`. The scratch project included the unchanged candidate sources and runner read-only, excluded the tracked calculator, and compiled one scratch calculator input per run. For each variant ran `dotnet build -p:Lab=<review-lab-dir> -p:Variant=<variant> -o <variant>` and `dotnet <variant>/SitkaP1.EconomyDispositions.Tests.dll`. No tracked engine or test source was edited.

| Variant | Observed |
| --- | --- |
| Unchanged control | Exit 0, 18 passed, 0 failed, same mixed-plan SHA-256 as the normal runner. |
| Credit KeepForUse in the revenue accumulator | Exit 1, 8 passed, 10 failed (cases 4–8, 11–13, 15–16). Quote ledger reconciliation detected the added revenue. |
| Remove the live-versus-frozen disposition comparison | Exit 1, 17 passed, 1 failed (case 12). Execute succeeded where `EconomyQuoteDrift` was expected. |
| Exclude LeaveAsDeadwood green mass from the contractor basis | Exit 1, 15 passed, 3 failed (cases 9, 11 and 15). Mixed mass fell from 2,126,303 to 1,444,380 g, and the mixed digest changed to `c5d3e1bc256b3c0931a1e26f55f8bf5d3bfd46641555d089a52d8f5bf057456a`. |

## Criteria

Engine and test file references below are relative to `Tools/MultiRackSitkaP1Lab` and use candidate line numbers. `EconomyQuoteCalculator.cs`, `EconomyRecords.cs` and `MultiAreaContinuation.Commands.Economy.cs` are under `src/Core/MultiArea/Economy/`. `DispositionTests.cs` is under `tests/Core/EconomyDispositions/`.

| Criterion | Result | Evidence |
| --- | --- | --- |
| A. Values 0/1/2 and default compatibility | **PASS** | Enum at `Economy/Dispositions/FellingDisposition.cs:9–20`. Focused cases 1–2 (`DispositionTests.cs:57–116`) and detached-base probe verified the default economic path. The live Phase 2B-2a UI is **NOT TESTED**. |
| B. Sell economics, D-062 grades, zero retained and deadwood | **PASS** | Detached-base comparison matched all pinned results and ordered ledger. Case 3 (`DispositionTests.cs:119–133`) observed zero retained and zero intents. |
| C. Keep economics and authoritative cumulative balance | **FAIL** | Cases 4–8 (`DispositionTests.cs:135–255`) verified zero revenue and TimberSale lines, exact volume, two settlements, contractor cost and zero intents. `RetainedTimber.cs:39–65,92` builds the tally from engine-graded cm³ and converts by exact decimal division. A read caller can corrupt authoritative per-species volume (B2). |
| D. Leave economics and one deadwood outcome per stem | **PASS** for the engine contract | Cases 9–11 (`DispositionTests.cs:258–372`) verified zero revenue and retained balance, the common contractor cost, conserved grade volume, one intent per stem and duplicate refusal. `Economy/Dispositions/FelledStemDeadwood.cs:30–62` uses the existing biological volume helper, assigns sequential IDs and checks duplicate area/tree pairs within the batch. Live Stage 4 append is **NOT TESTED**. See S1. |
| E. Guards, stage, floor, atomic settlement, cash sufficiency, Money Off | **PASS** | Cases 7, 13, 14 and 16 (`DispositionTests.cs:212–240,416–494,514–539`) covered first/second rates, contractor-only, no clearfell, Money Off and ledger reconciliation. Reviewer probes exercised upfront refusal, fault/retry/replay for all dispositions and all-Keep/all-Leave whole-area refusal. The same full-cost check applies again during settlement at `EconomyQuoteCalculator.cs:98–103`. Money Off kept Economy null, emitted no settlement and read `NOT RECORDED`. |
| F. Disposition and physical outcome contract | **FAIL** | Case 12 covered command refusal, changed live choices and cancel/re-add (`DispositionTests.cs:374–413`). `EconomyRecords.cs:45–63,99–119,139–145,175–183,221–227` adds properties without changing existing positional constructors. Sorted disposition and species keys enumerate deterministically. The approved map and retained species tally expose live mutable data (B1 and B2). |
| G. Engine, focused tests and handoff scope | **PASS** | The 12-path three-dot diff stays within the allowed paths. Core.Tests built. UI, saves/Version 3, Forest Maker, Unity and 3D paths are absent from the diff. |

## Findings

### Blocking

1. **B1 — A read caller can change the live frozen disposition.** `EconomyQuoteCalculator.cs:214,459` puts the mutable `SortedDictionary` directly on the quote. `EconomyRecords.cs:106,119` exposes it through an interface, and `MultiAreaContinuation.Commands.Economy.cs:25` returns the exact approved quote from session state. The probe approved Keep for MA-B-002, cast `FrozenFellDispositions` to `IDictionary` and set Leave. A fresh read showed `LeaveAsDeadwood` without a command or revision change, despite the frozen harvest job remaining Keep. Execute refused with `EconomyQuoteDrift`, leaving a valid approved plan unable to execute until the backing map was restored. Return a genuinely read-only map over privately owned data.
2. **B2 — A read caller can change the authoritative retained timber breakdown.** `Economy/Dispositions/RetainedTimber.cs:39–42,62–65` exposes mutable maps, and `MultiAreaContinuation.Commands.Economy.cs:32` returns the session tally by reference. After one retained oak settlement, the probe cast `BySpecies` to `IDictionary` and replaced the oak amount with 1 cm³. A fresh read returned a total of 292,613 cm³ and a species sum of 1 cm³ without a command or settlement. The shared empty tally uses the same mutable pattern. Make the populated and empty maps genuinely read-only before publishing them to views and receipts.

### Should-fix

1. **S1 — The Stage 4 follow-up snippet can silently skip a settled deadwood intent in a supported configuration.** `PHASE2B2B-ENGINE-HANDOFF-20261127.md:138–152` guards the append on `nextStage4 is not null` and lists nullability as open. `src/Core/Stage4Models.cs:93–104` accepts Legacy plus Economy, and `src/Core/MultiArea/MultiAreaContinuation.cs:198` leaves Stage 4 null there. A real Legacy-plus-Economy session settled one Leave intent with Stage 4 null. Document the confirmed case and the policy question for that later pass. The suggested append plus counter increment already preserves `NextDeadwoodNumber == Deadwood.Count + 1` when the invariant holds on entry (`src/Core/Phase1/Phase1SaveValidator.cs:333–342`). The existing validator does not check the death cause there. Version 3 remains outside this review. The handoff otherwise distinguishes offline execution, build-only evidence, source inference and the deferred append, and labels management guidance, simulation abstractions and gameplay calibration.

### Notes

1. **N1 — Disposition choices stay with their original order instance and area.** `Economy/EconomyModels.cs:54–73` keys on work kind, area and target. `Economy/Dispositions/FellingDisposition.cs:77–89` also requires reference identity. Approve copies the order references (`MultiAreaContinuation.Commands.Plan.cs:27–41`). Cancel drops the approved plan but leaves the old choice internally until a later edit, settlement or consented cancellation (`Commands.Plan.cs:74–90`, `Commands.Economy.cs:118–127`, `EconomyQuoteCalculator.cs:175,515`). Case 12 proved that re-adding defaults to Sell. The scratch resolver probe used the same synthetic tree ID in two areas and a value-equal replacement order. Keep stayed on the original area and instance, and both other orders resolved to Sell. A real two-area plan yielded exactly `MA-A/MA-A-001,MA-B/MA-B-002` in order, and supplying the MA-B ID to the MA-A setter refused. A world with duplicate tree IDs is invalid at genesis and on save validation, and Execute rejects duplicate IDs in the plan (`MultiAreaExperiment.cs:109–118`, `Phase1SaveValidator.cs:97`, `Commands.Plan.cs:133–144`).
2. **N2 — The accounting bucket and the biological deadwood record use different representations of the same volume form.** `Economy/D062TimberAssortment.cs:283–289` takes `FallenDeadwoodRecord.StemVolumeM3` and floors the scaled value into D-062 integer cm³. Kept timber converts that integer to decimal m³. `Economy/Dispositions/FelledStemDeadwood.cs:30–36` carries the original float32 biological estimate used by natural-death accounting (`AnnualPipeline.cs:199–206`). `EconomyRecords.cs:47–57` and handoff lines 114–115 and 129–130 identify the bases. For later display or save projection, `DeadwoodStemVolumeCm3` closes the D-062 quote's volume balance, while the intent supplies the biological record's estimate. Case 10 checks the float bits and cases 10–11 check numbering and the grade-volume balance.

## Not tested

Unity, the browser and Phase 2B-2a UI at runtime, the full Core.Tests runner, the frozen simulation anchors, the 2D gate, Version 3 persistence and an integrated Stage 4 append were outside this review. Base parity was exercised on one representative mixed-species sale plan, with source review for the rest of the default path. Cross-platform determinism, a full invalid duplicate-ID world execution and dashboard synchronization were not qualified.

INTEGRATION DECISION: REJECT
