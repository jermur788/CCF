# CCF Multi 2D — Phase 2B-1 Economy, Labour & D-062 Timber Engine — Independent Review

- **Date:** 2026-11-26
- **Reviewer role:** independent review sub-agent (not the implementer)
- **Reviewed SHA (candidate):** `d514f6bbb9b1713c67491ca095b17bf3f0102443` (`origin/feat/multi-2d-phase2b-economy-core-20261126-v1`; implementation commit `204fcdeb` + handoff commit `d514f6bb`)
- **Base:** `5648813d9ccbe8d38e9fbe5df38d62c810af4a22` (`origin/integration/2d-sim`, frozen S0b tip)
- **Diff reviewed:** `git diff 5648813d..d514f6bb`, 23 files, +2563 / −26
- **Binding contract:** `specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md` §1, §2, §4, §5 (read at the candidate SHA)
- **Review branch:** `review/multi-2d-phase2b-economy-core-20261126-v1`

> This is an independent opinion. It does **not** authorise a merge. Integration remains the integrator's decision under the required gates.

---

## Method (what I actually did)

1. Ran the mandatory guard: the review worktree was on the right branch at `5648813d`, the root checkout was on `main`, and the fetched candidate was `d514f6bb…`. Result: `GUARD OK`.
2. Read every changed file at the candidate SHA, using `git show d514f6bb:<path>` into a scratch directory outside the repo. Compared them against the 3D references (`Stage1EconomyDefaults.cs`, `TimberYieldConfiguration.cs`, `ScenarioOneDefinition.cs`, `ScenarioOneEconomyAdapter.cs`) at the same SHA.
3. Cloned the candidate into a scratch directory with `git clone --shared` to `/tmp/rv2b/probe`, outside the repo, and seeded `.p0ref` from the integration worktree. In that clone I rebuilt and re-ran the suites with SDK 8.0.134:
   - Core: **`290/290 passed, 0 failed`**. All 8 `PASS economy:` lines are present. Anchor #7 `776a68df…ce22d` appears exactly once and anchor #9 `93e9abda…9251f` appears exactly once.
   - Save: **`50/50 passed, 0 failed`**, including `PASS golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding`.
4. Added a probe test file in the scratch clone only (never committed) to check the edge cases listed below. With the probe the run was 295/295.
5. Ran the canonical gate's G2 **S0a move-proof sub-check** (`build/2d-gate/move-proof.py`, with the `t0Commit` and table from `s0a-move-table.json`) directly with python3, against both the base and the candidate.
6. Did my own arithmetic for the €/m³ → €/t conversions.

---

## Answers A–L

| Q | Result | One-line evidence |
|---|---|---|
| **A. C-ECON conformance** | **PASS** | `QuoteWorkPlan` (`MultiAreaContinuation.Commands.Economy.cs:38-46`) runs under one `sync` acquisition, is fenced, and only reads. It returns the root's `PlanStateRevision`; a stale fence gets `PlayerPlanStateStale` (test 2). `Approve` (`Commands.Plan.cs:29-69`) recomputes the quote, refuses if it is ineligible, and freezes `EconomyApprovedQuote` in the same root assignment. Nothing can edit it afterwards (`WorkPlanAlreadyApproved`). `BuildSettlement` runs inside `Execute`'s `lock (sync)` and before the single `root = root with {…, Economy = nextEconomy}` (`Commands.Plan.cs:256-281`). Replay returns `executed.Settlement` before the fence check and never re-settles (`:113-118`). A key conflict still gives `WorkExecutionKeyConflict`. The key is the engine-minted `planId + ":execute"` (asserted in test 4). |
| **B. Money and cash** | **PASS** | Money is checked `long` cents everywhere, rounded with `decimal` (`EconomyDefaults.RoundRatio/CeilRatio`). Opening cash is `1_200_000`. The upfront requirement is harvest + non-harvest labour + materials (`EconomyQuoteCalculator.cs:331-332`), checked at Approve (`RequireEligible`) and again at Execute (`:86`). Revenue is never added to the gate. My probe confirmed that test 5 discriminates: cash €2,000 + revenue €2,011.40 ≥ upfront €2,500, and it is still refused. A negative balance is impossible: `cashAfter = cash − cost + revenue ≥ cash − cost ≥ 0`, with an explicit guard at `:96`. |
| **C. Labour** | **PASS** | The allowance is 2,400 min (40.0 h). It resets only in `ReservedPhase2BYearEndSummaryStep` → `EconomyAnnualSummary.BuildYearEnd` (test 6). `Fell → Landowner` is refused in `SetOrderExecutor`, which `SetFellExecutor` delegates to (`Commands.Economy.cs:75-77`). It is also flagged a second time inside `ComputeCore` (`EconomyQuoteCalculator.cs:186-195`) and by `IsLandownerEligible(Fell)=false`. Frozen executors are only copied from the already-validated pending map, and no restore route exists because Save is refused. Landowner minutes produce `labourCents = 0` (`:270-274`). |
| **D. Harvest pricing** | **PASS** | There is one `EconomyHarvestQuote` per `ComputeCore` call, which means one per approved plan. Stage rates are 2000/2300/2000/1500 cents/t, all inside €15–€23. The floor is `max(0, floor − variable)` per plan (`:219-222`). Each `Execute` settles exactly one plan, and only one plan can be approved at a time, so no merge path exists. Test 7 shows two plans in year 0 each paying €2,500. See observation O-1: the stage override can set any plan to €15/t, but it does not bypass the floor. |
| **E. D-062 assortment** | **PASS** | The spec table matches the 3D `TimberYieldConfiguration.cs` exactly. Oak veneer: small end ≥400, mid ≥450, 2.8 m, `Pruned` required. Beam: small end ≥250, mid ≥300, 2.6 m. Fencing: 120–250 small end. Second planking ≥250. Firewood ≥70. Beech has only second planking and firewood. Eligibility comes from size and pruning sections; the price comes from `FindPrice(species, assortment)` alone. Probe: unpruned → no veneer; pruned to 3.0 m → no veneer, because 350 + 2800 > 3000; first veneer at DBH 454 mm (H 28 m), mid ≈450 mm, price unchanged at 25 800. Conversions check out: 230→258, 145→163, 100→112, 60→67, 30→34 €/t (whole-€ rounding, `AwayFromZero`), and the formula is identical to 3D `CentsPerTonneAtProxy`. Sitka prices 38/47/68/95 €/t equal the 3D `Stage1EconomyDefaults` references. Pruning does not change Sitka revenue (test 8). |
| **F. Off-inertness** | **PASS** (one observation) | `Phase2.Economy == Off` by default. `root.Economy` is null at genesis, after Execute and after Advance (test 1). The token gets no suffix. The reserved step returns early. The existing test sources are unchanged apart from the one registration line, and they still pass, so the code-pinned anchor literals hold. I saw #7 and #9 directly in stdout. Money fields read `NOT RECORDED` with `null` cents. **Observation O-2:** with Off, the existing `HistoryYearDetail` readout does change: it gains a 4th `FinancialLedger` disclosure. The root is untouched, but that readout is no longer byte-identical (see the conditions). |
| **G. Persistence** | **PASS** | `CaptureSave`, `SaveExpectation` and `Restore` each refuse with `SaveEconomyRequiresV3` (`MultiAreaContinuation.cs:537,553,578`). These are the only Core save/load entry points; the UI calls them, and nothing calls `Phase1RootMapping.Capture/Restore` directly. The `src/Core/Phase1/**` diff is empty. There is no `SchemaVersion = 3` and no `: IFeatureBlockCodec`. The v2 golden passes. |
| **H. Structural discipline** | **PASS** | `Economy` is the last positional parameter on `MultiAreaRoot` and on `MultiAreaModelSelection`. `AnnualPipeline.Steps` is identical: 6 entries, same order. The ecological code paths only gained `Economy =` assignments, plus the step-6 body. No RNG is used in the simulation path. `RngSalts.cs` is unchanged. Floats appear only where existing ecology already uses them (DBH/height/`StemVolumeM3`, which convert to integers, and the pruning-height comparison). `Math.Sqrt` is followed by integer correction. Every dictionary output is sorted before it is used. |
| **I. Scope ruling** | **PASS (conditional)** | `Stage4Models.cs`: **acceptable minimal opt-in extension**. The selection record lives there, and the change is one trailing optional parameter, two members and one `RequireSupported` line. `AnnualPipeline.cs`: **acceptable as an integrator-adopted patch, not as lane-owned work**. The change fills the slot C-ECON reserves for 2B (contract §2 and §4.5) and adds one `NextEconomy` scratch property. It leaves `Steps` untouched and does nothing when Off. The same applies to the other integrator-owned hunks (`MultiAreaContinuation.cs`, `Commands.{Plan,Fell,Prune,Clear,Planting}.cs`, `MultiAreaContracts.cs`, `MultiAreaHistoryQuery.cs`, `Program.cs`). Contract §4 says the integrator applies shared-file items, so the integrator must review and take ownership of these hunks (condition C2). None of them is an unauthorised behavioural change. |
| **J. Economy on any base** | **FAIL** (design, non-blocking → condition C3) | Probe: `Legacy with { Economy = Enabled }` on the frozen MA-1 experiment builds, executes a Fell and settles (€2,500 cost, €5.58 revenue), and advances under the token `ma1-isolated-per-area-v1+economy-v1`. That matters for three reasons. (1) The frozen MA-1/Stage 3/4/5 paths gain a new, untested supported selection: the support matrix grows from 5 to 10 with no tests on 5 of them. (2) The calibration (the €12,000 / 0.64 ha stand, the Oak/Beech ladders, on-demand broadleaf saplings, broadleaf planting) is defined only for Phase 2. Below Phase 2 every stem is silently priced as Sitka and pruning history does not exist. (3) It turns the frozen `Legacy` reference path into something with a variant, which the frozen-path discipline is meant to prevent. **Recommendation:** accept `Economy = Enabled` only when the base selection is `Phase2`, or the owner records an explicit decision accepting every base. |
| **K. Test adequacy** | **PASS** | All 8 tests drive real engine commands. None is vacuous. Test 5 genuinely discriminates pre-financing (probe numbers in B). Tests 4 and 7 check the exact rule values. Weaker spots: test 3's "re-quote cannot change the frozen quote" is close to tautological, because nothing can reach the frozen record. Test 1 depends on anchors #7/#9 rather than comparing the Off root to a baseline. Test 8's "0.64 ha calibration" is structural only (counts, area, cash) and runs no economic result on the fixture. **No must-have assertion is missing** for the locked rules. The single most valuable addition, if one were allowed, would be a replay after later state changes (for example after `Advance`), to confirm the same receipt comes back and cash does not move. The code handles this correctly. |
| **L. Handoff accuracy** | **FAIL** (minor, non-blocking) | See "Handoff inaccuracies" below. |

**Counts:** PASS 10 (A, B, C, D, E, F, G, H, I, K) · FAIL 2 (J, L; both non-blocking) · NOT TESTED 0. The gate stages are reported separately under LIMITS.

---

## Blocking findings

**None against the candidate code.** No defect I found requires the implementer to change this SHA before integration.

There is one **integration precondition on the integrator side** that blocks a green gate today. It is not an implementer defect:

- **IP-1: the canonical gate's G2 S0a move-proof fails on this candidate.**
  - **Where:** `Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-move-table.json:4` (`"gateEnforce": true`) and `build/2d-gate.sh:1051-1068`.
  - **What I observed:** `move-proof.py --base 47b27a8d… --head d514f6bb` exits `1` with `FAIL [core]: contiguous block core-cmd-clear (T0 lines 626..673) not found in order in …Commands.Clear.cs`. The same command at `--head 5648813d` exits `0` with `MOVE-PROOF PASS`.
  - **Rule:** contract §6 and §4 require a green gate before integration. `build/2d-gate/**` is integrator-owned, and the lane correctly left it alone.
  - **Why it blocks:** even on a host with a Linux `node`, `2d-gate.sh` would fail at G2. Integration therefore needs a reviewed integrator decision that retires or re-baselines the S0a/S0b move-proof lock, followed by a full green gate run.

---

## Non-blocking findings / observations

- **O-1: the harvest stage override is unconstrained.** `SetHarvestStageOverride` (`Commands.Economy.cs:102-120`) lets the player label any felling plan `Clearfell` (€15/t), including a first thinning, which would otherwise be €20/t. The floor still applies, but it is a free cost reduction for the player. The override is documented in handoff assumption 3, but the spec's rule table does not state it. A related point: an override set while no felling order is pending does not count as unexecuted work, so it survives `Advance` without consent (my probe showed `Clearfell` persisting into the next year). Needs an owner decision: restrict it, or record it as gameplay calibration.
- **O-2: the Off-mode history readout changed.** `MultiAreaHistoryQuery.cs:707-713` appends a `FinancialLedger` disclosure to every Off session, including `Legacy`. My probe counted 4 disclosures where the base has 3. This is root-inert and anchor-inert, but any UI smoke or payload snapshot of `/history` will see different bytes. G2 UI smoke and G5 budgets were not run.
- **O-3: shelter time is rounded per order, not per batch.** Each shelter order is its own line with `CeilRatio(1, 60, 45) = 2 min`, so 45 shelters cost 90 min of owner or contractor time against the 60 min reference. That inflates the cost by about 50%. The spec text says `CeilRatio(count, 60, 45)`. Pruning is unaffected because 60/15 divides exactly.
- **O-4: `ReadEconomyHistoryYear` is not fenced.** It returns `EconomyYearHistory` without `sessionId`, `source` or `planStateRevision`. C-FENCE requires a fence on every per-feature UI readout. There is no UI yet, so this is for the 2B UI lane to fix.
- **O-5: latent UI failure on the slot list.** `src/Ui/MultiApi.Saves.cs:18` (`ReadMultiSavesAsync`) calls `SaveExpectation()` without a guard, and that now throws for Economy sessions. This cannot happen today because the UI only builds `Phase2`. The UI lane must handle it before exposing Economy.
- **O-6: ownership path naming.** The lane files are in `src/Core/MultiArea/Economy/**`, not the §5 pattern `src/Core/<Feature>/**`. The commands file is `src/Core/MultiArea/Economy/MultiAreaContinuation.Commands.Economy.cs`, not `src/Core/MultiArea/MultiAreaContinuation.Commands.Economy.cs`. The test file is `Phase2BEconomyTests.cs`, not `EconomyTests.cs`. No protected path is touched. The integrator can accept or relocate these.
- **O-7: the fixture RNG is not in `RngSalts`.** `EconomyCalibrationFixture` uses its own integer SplitMix64 to generate genesis data. It is deterministic and not part of the simulation's keyed RNG, but §4.6 says to register any new key family. The integrator should decide whether that rule covers fixture generators.
- **O-8: `SameEconomy(economyOptions, null)` is effectively "Economy must be Off".** No saved economy options exist to compare against. This is harmless, because the save is refused earlier, but the handoff's "options equality" wording overstates what the check does.
- **O-9: the drift check compares only three totals.** `EconomyQuoteDrift` compares cost, revenue and owner minutes, not line detail. That is adequate today, because `Execute` already requires `plan.Source == Token(root)`.
- **O-10: no haulage line.** The 3D model has `HaulageCentsPerTonne` (€12/t). 2D omits it, which is consistent with the roadside price basis. Worth stating explicitly in the spec.

## Handoff inaccuracies (L)

1. **"22 changed paths"** (G7 row) was correct for `204fcdeb` (5648813d..204fcdeb). It is wrong for the reviewed SHA `d514f6bb`, which has **23** because the handoff file itself is the 23rd path.
2. **G2 is described as "NOT RUN"** (because G0 failed), and the handoff says the move-proof lock "will" need retiring. In fact the move-proof check **FAILS** on this candidate (IP-1). The handoff anticipates this but does not state it as an observed failure.
3. **"`RequireSameOptions` (`SameEconomy`)"** implies a comparison of economy options. It actually always compares against `null` (O-8).
4. **"IFA combined harvesting+forwarding range (€15–€23/t)"**: the 3D `Stage1EconomyDefaults` range across stages is €14–€24/t. €15–€23 is the span of the *selected* values.
5. The handoff does not mention the Off-mode `HistoryYearDetail` change (O-2), the unconstrained stage override (O-1, beyond assumption 3), or that `Economy = Enabled` is accepted on every base, including `Legacy` (J).

I checked the remaining factual claims and found them accurate: commit identities, file list, 290/290 and 50/50, anchors #7/#9, the 8 test names, the assumption 3 source line `ScenarioOneEconomyAdapter.cs:164`, the recovered constants, and the Save test filename.

---

## INTEGRATION DECISION

**`APPROVED WITH CONDITIONS`**

- **C1.** The integrator retires or re-baselines the S0a/S0b move-proof (`gateEnforce`) through a reviewed decision. The canonical `2d-gate.sh` then runs **to completion and green** (G0–G7, plus B if required) on a host with a Linux `node`. Integrate only on green.
- **C2.** The integrator explicitly reviews and takes ownership of every hunk in integrator-owned or locked files: `AnnualPipeline.cs`, `MultiAreaContinuation.cs`, `Commands.{Plan,Fell,Prune,Clear,Planting}.cs`, `MultiAreaContracts.cs`, `MultiAreaHistoryQuery.cs`, `Stage4Models.cs`, and the `Program.cs` registration line.
- **C3.** Before integration, either restrict `Economy = Enabled` to the `Phase2` base in `RequireSupported`, or record an owner decision accepting it on every base, `Legacy` included (J).
- **C4.** The owner accepts the Off-mode `HistoryYearDetail` readout change (O-2), and G2 UI smoke and G5 payload budgets pass with it.
- **C5.** The owner decides on the unconstrained harvest stage override (O-1): restrict it, or record it as gameplay calibration.

This is an independent review opinion and **not a merge authorisation**.

---

## LIMITS (what I did not verify)

- **The canonical `2d-gate.sh` was not run.** G0–G7 and B are **NOT TESTED** inside the gate runner, because this host has no Linux `node`. I ran only the G2 move-proof sub-check directly (it FAILS, IP-1), plus Core and Save tests outside the gate.
- **NOT TESTED:** UI smoke (`ui-smoke*.sh`), `cli-safety.sh`, `build-output-safety.sh`, `p0-isolation.sh`, G5 payload budgets, browser tier B, and any HTTP/UI/runtime execution. No UI exists for Economy.
- Anchors #1–#6 and #8: not separately recomputed. I relied on the unchanged, passing pinned-literal tests and did not re-hash `.p0ref`.
- No Unity, Blender or rendered verification. None applies to this Core-only change.
- I did not check the 282 baseline test count at `5648813d` by running the base. 290 − 8 = 282 is consistent with the handoff.
- I made no ecological or research judgement on the calibration values beyond checking them against the in-repo 3D sources.
- The probe code lived only in a scratch clone under `/tmp/rv2b/probe` and was never committed. No implementation, spec, test or handoff file was edited. This file is the only one written to the review branch.

---
---

# Delta Re-Review (C5) — 2026-11-26

> Appended section. The findings above (first review of `d514f6bb`) stay as written. This section judges only the bounded delta, plus the invariants the delta must not have broken. It is an independent opinion and **not a merge authorisation**.

- **Reviewed SHA:** `dcba31ca3bd51e0d3f23046cf3da121908529d24` (`origin/feat/multi-2d-phase2b-economy-core-20261126-v1`, single delta commit `dcba31ca`)
- **Delta range:** `d514f6bb..dcba31ca`, 9 files, +149 / −55
- **Base:** `5648813d…` (unchanged). `origin/integration/2d-sim` = `5648813d` (unchanged). `origin/main` = `bdeb6a32…` (not touched by this lane).
- **Guard:** `GUARD OK`, review worktree at `882dd17f`, root checkout on `main`.

## Method

1. Read the whole delta diff, then grepped the candidate tree (`git grep` at `dcba31ca`) for every clearfell and override spelling, every `HarvestThinningStage` and `ExecutedHarvestPlansCount` use, and the floor code.
2. Compared the retained rate constants against the 3D sources at the same SHA: `Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs:31-37,184-189`, `ForestryWorkCalculator.cs:171`, `ScenarioOneEconomyAdapter.cs:164`.
3. Ran `git clone --shared` of the candidate into `/tmp/rv5/probe`, outside the repo, seeded `.p0ref`, and ran the suites with SDK 8.0.134:
   - Core: **`291/291 passed, 0 failed`**. All 9 `PASS economy:` lines are present. Anchor #7 `776a68df…` and anchor #9 `93e9abda…` each appear exactly once.
   - Save: **`50/50 passed, 0 failed`**, including the pinned v2 golden.
4. Added a probe file `ProbeC5Tests.cs` in the scratch clone only (never committed). It ran three cases. Result `294/294`:
   - approve then cancel leaves the count at 0 and the next quote at `FirstThinning`;
   - replaying an `Execute` does not advance the count, `Advance` neither resets nor advances it, the next year's felling plan is `SecondThinning`, and each plan pays ≥ €2,500;
   - an executed non-fell plan (clear undergrowth) has no harvest job and leaves the count at 0.

## D1–D8

| Q | Result | One-line evidence |
|---|---|---|
| **D1. Clearfell removed** | **PASS** | `git grep -i -E 'clear[ _-]?fell\|clearf\|clear-cut\|clearcut\|1_500L'` over `src/Core/**` and `tests/Core.Tests/**` finds only the test-9 negative assertions (`Phase2BEconomyTests.cs:419,446-447,479`). It also finds one pre-existing string, `"clear-fell probe"` in `PlayerSessionTests.cs:422`, which is identical at base `5648813d` and is a test label, not pricing. The enum has 3 members, there is no `ClearfellCentsPerTonne`, and the switch arm is gone. |
| **D2. Override removed, no back door** | **PASS** | `SetHarvestStageOverride`, `EconomyApprovedQuote.HarvestStageOverride`, and `PendingHarvestStageOverride` on the State and the View are deleted from the records' positional parameters, not just left unused. `ComputeCore` has lost its `harvestStageOverride` parameter. The only `src` uses of `HarvestThinningStage` are the enum, `HarvestRateCentsPerTonne`, `AutoHarvestStage`, and the output field `EconomyHarvestQuote.Stage` (`EconomyRecords.cs:31`). No method, constructor, property or field on the continuation, the state, the view, the approved quote or the options accepts or stores a stage. Test 9 checks this by reflection, and I confirmed it by grep. |
| **D3. Derivation automatic, from history** | **PASS** | `EconomyQuoteCalculator.cs:196`: `stage = EconomyDefaults.AutoHarvestStage(econ.ExecutedHarvestPlansCount)`, with `≤0 → First`, `1 → Second`, `_ → Later` (`EconomyModels.cs:151-156`). The count is written in only one place, `BuildSettlement` (`EconomyQuoteCalculator.cs:136-142`, `+1` only when the frozen quote has a `HarvestJob`), and that is called only from `Execute` (`Commands.Plan.cs:257`). It starts at 0 in genesis, `BuildYearEnd` never touches it, and the probe shows cancel, replay, Advance and non-fell plans do not move it. No caller input reaches it. |
| **D4. Rates = verified non-clearfell 3D tiers** | **FAIL** (one constant; see F-1) | No €19/t tier was invented, and the constants were left unchanged by the delta. But the 3D selected value for first thinning is **€21/t** (`Stage1EconomyDefaults.cs:33` `Harvest(FirstThinning, 2000, 2100, 2200)`, and `Parameter` sets `Selected = reference`, `:187`, which `ForestryWorkCalculator.cs:171` uses). 2D uses `FirstThinningCentsPerTonne = 2_000L` (`EconomyModels.cs:127`), which is the 3D **low** bound. Second (€23) and later (€20) match 3D selected. The reconciliation note honestly records the €23/€19 parenthetical conflict and the choice of the operative clause, but its claim that €20/€23/€20 are the "verified 3D-aligned" tiers is **false for `FirstThinning`**. |
| **D5. €2,500 floor per plan** | **PASS** | `EconomyQuoteCalculator.cs:219-221`: `minAdj = max(0, floor − variable)` inside each `ComputeCore`, which runs once per approved plan. Test 7 still asserts two plans in one year = €5,000 in floors, and my probe asserted ≥ €2,500 on two plans across years. The removed override never touched the floor, and no bypass opened. A pre-existing note (O-11): `EconomyOptions.MinimumHarvestJobCents` is set at session construction and accepts any value ≥ 0. It is not a player command and the delta did not change it. |
| **D6. Prior invariants intact** | **PASS** | The delta touches only `EconomyModels/QuoteCalculator/Records`, `Commands.Economy`, `Commands.Plan` (override lines only), the tests, the spec, the checklist and the handoff. `git diff d514f6bb..dcba31ca` on `MultiAreaContinuation.cs`, `AnnualPipeline.cs`, `src/Core/Phase1/**`, `D062TimberAssortment.cs`, `EconomyCalibrationFixture.cs`, `Commands.Fell.cs`, `MultiAreaHistoryQuery.cs` is empty, and `MultiAreaContracts.cs`/`Stage4Models.cs` are not in the delta. Tests 1–8 pass unchanged except for one swapped line in test 3 (see D7). That covers upfront cash and no debt, contractor-only `Fell`, 40 h and reset, the D-062 ladders, 890 kg/m³, Off-inertness, `SaveEconomyRequiresV3` and the anchors. |
| **D7. Tests non-vacuous** | **PASS** (with notes) | Test 9 drives the real engine: 3 Approve+Execute cycles then a 4th quote, asserting First→Second→Later→Later and rates 2000/2300/2000/2000 on both quotes and settlements. That proves (b). (a) is proved structurally: the enum has 3 members, ordinal 3 is refused, and reflection checks names and types on 7 types. Not tautological. Test 3's replacement `SetOrderExecutor(...)→WorkPlanAlreadyApproved` is a real refusal path, so it keeps the frozen-approval guard covered. **Notes:** (i) the reflection scan does not cover `MultiAreaRoot`, `MultiAreaModelSelection` or `MultiAreaWorkPlanAdapter`, though the grep did. (ii) The message `"uses verified 3D rate"` (`:500`) is wrong for plan 1 (F-1). (iii) The one assertion I would call must-have and missing: **the count does not advance on cancel, replay or a non-fell plan**. My probe proves the code is correct, so this is not blocking. |
| **D8. Documentation accuracy** | **FAIL** (non-blocking except F-1) | Accurate: spec §2 row and §7, checklist P2B-08 and handoff §7 record C3, C4, C5, the removals and the derivation rule correctly, and no restriction was added for C3. Inaccurate: (1) the "verified 3D-aligned FirstThinning = €20/t" claim (spec `:88`, handoff `:165`) is wrong (F-1). (2) The handoff body above §7 still says `SetHarvestStageOverride` exists (`:26`), `Clearfell (€15/t)` is selectable (`:117`), and stage selections are `€20/€23/€20/€15/t` (`:108`), with no "superseded by §7" marker. (3) The handoff still says "Exact 8-case" (`:38`) and `290/290` (`:51`). §7 gives no new count (291/291) and no evidence line for the delta. |

**Counts:** PASS 6 (D1, D2, D3, D5, D6, D7) · FAIL 2 (D4, D8) · NOT TESTED 0.

## Findings

- **F-1: the `FirstThinning` rate is not the 3D-selected tier.** This blocks full C5 closure.
  - **Where:** `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Economy/EconomyModels.cs:127` (`2_000L`), against `Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs:33,187`, where 3D selects `2100`.
  - **What the ruling said:** the owner's operative clause asks for "the exact non-clearfell CCF thinning tiers already verified from 3D".
  - **Why it went unnoticed:** the value predates this delta. My first review checked only that the rates fell inside the €15–€23 range, so it missed this too. The new reconciliation note then asserts alignment that does not exist.
  - **Fix (either one):**
    - (a) set `FirstThinningCentsPerTonne = 2_100L`, and update test 9's `expectedRates[0]`, the test 7/8 values that depend on it, and the spec/handoff/evidence strings (`EconomyModels.cs:202`); or
    - (b) the owner explicitly records €20/t as a deliberate gameplay calibration below the 3D reference, and the "3D-aligned" wording is corrected.
  - **Effect on tests:** none of tests 1–8 or 9 hard-codes a first-thinning *cost* I could see. They assert stage, rate and floor, and small fells sit on the €2,500 floor. I have not run option (a), so its test impact is **not tested**.
- **O-11 (pre-existing, non-blocking):** `EconomyOptions.MinimumHarvestJobCents` can be configured to 0 at construction. If the floor must be fixed, the integrator or owner should pin it. It is not a player path.
- **O-12 (observation, owner question):** `ExecutedHarvestPlansCount` counts across the whole session, not per area. A felling plan in MA-B moves MA-A's next plan to `SecondThinning`. The spec says "on the stand". That is consistent if the session is one stand, but the owner should confirm it for multi-area use. 3D counts `previousInterventions` per scenario stand.
- The D8 items (2) and (3) are documentation-only. The integrator or implementer should add a "superseded by §7" marker and the 291/291 evidence line.

## Status of prior conditions

- **C1: OPEN** (integrator duty). Retire or re-baseline the S0a/S0b move-proof, then get a full green `2d-gate.sh`.
- **C2: OPEN** (integrator duty). Integrator review and ownership of the integrator-owned or locked hunks. The delta adds two removal-only hunks in `Commands.Plan.cs` (`:37-40`, `:63-66`) to that set.
- **C3: CLOSED.** The owner accepted it as-is and the ruling is recorded (spec §7, handoff §7). No restriction was added: `RequireSupported` and `Stage4Models.cs` are unchanged in the delta.
- **C4: CLOSED.** The owner accepted it as-is and it is recorded. The `FinancialLedger: NOT RECORDED` disclosure is retained (`MultiAreaHistoryQuery.cs` is unchanged in the delta, and the `NOT RECORDED` strings are present). The original C4 tail, "G2 UI smoke and G5 budgets pass with it", now falls under C1's green gate.
- **C5: NOT FULLY CLOSED.**
  - **Closed:** no clearfell (D1), no override and no back door (D2), automatic derivation from history (D3), and the per-plan floor (D5).
  - **Open:** the rate-exactness part (D4/F-1), carried below as **C6**.

## INTEGRATION DECISION (delta re-review)

**`APPROVED WITH CONDITIONS`** — C1, C2, C6, C7

- **C1** (integrator): green canonical gate after a reviewed move-proof decision.
- **C2** (integrator): ownership of the integrator-owned hunks, now including the two `Commands.Plan.cs` removal hunks from this delta.
- **C6** (**pre-integration, blocking**): resolve F-1. Either set `FirstThinningCentsPerTonne` to the 3D-selected `2_100L` and update the dependent tests and strings, with Core and Save re-run green, or get an explicit owner record that €20/t is a deliberate calibration and correct the "3D-aligned" wording in spec §7 and handoff §7.
- **C7** (doc, non-blocking): mark the handoff's pre-§7 override and clearfell lines (`:26`, `:108`, `:117`) as superseded, and add the 291/291 delta evidence.

This is an independent review opinion and **not a merge authorisation**.

## LIMITS (delta re-review)

- **The canonical `2d-gate.sh` was not run.** G0–G7 and B are **NOT TESTED**. I did not re-run the move-proof. IP-1 still applies because the delta does not touch `Commands.Clear.cs` or `build/2d-gate/**`.
- **NOT TESTED:** UI smoke, `cli-safety.sh`, `build-output-safety.sh`, `p0-isolation.sh`, G5 payload budgets, browser tier B, and any HTTP/UI/runtime execution.
- I ran only Core and Save outside the gate, plus three probe cases in a scratch clone. Anchors #1–#6 and #8 were not separately recomputed.
- I did not run the F-1 fix (option a) or measure its test impact.
- No Unity, Blender or rendered verification. None applies to this Core-only change.
- I made no ecological or market judgement on which first-thinning rate is "right". I only compared against the in-repo 3D source.
- The probe lived only in `/tmp/rv5/probe` and was never committed. No implementation, spec, test or handoff file was edited. This review file is the only file written to the review branch.

---

# C6 Repair Re-Review — 2026-11-26

- **Reviewer:** the same independent reviewer as the two sections above. This is a focused re-review of the C6/C7 repair, not a fresh full review.
- **Reviewed candidate:** `e97b378072c988c4f1b8157ff0486ddc21349283` on `origin/feat/multi-2d-phase2b-economy-core-20261126-v1`. The base is still `5648813d`.
- **Repair range:** `dcba31ca..e97b3780`: 7 files, +84/−29.
- **Guard:** review worktree on `review/multi-2d-phase2b-economy-core-20261126-v1` at `3fdc0c4d`. The fetched candidate matched `e97b3780`. GUARD OK.

## Method

1. Read the full `dcba31ca..e97b3780` diff and confirmed which files it touches (`git diff --name-only`).
2. Read the 3D source at the candidate SHA: `Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs:33-36` (`Harvest(...)` rows), `:156-160` (`Harvest` → `Parameter`) and `:184-188` (`Parameter` sets `Selected = reference`). Also read `ForestryWorkCalculator.cs:171`, which consumes `CombinedCentsPerTonne.Selected`. `git diff 5648813d e97b3780 -- Assets` is empty, so the 3D source is unchanged on this branch.
3. Ran `git grep` at `e97b3780` for `€19`, `1_900`, `1900L` and `19/t` across Lab `src`/`tests`, `specs/` and the 3D `WorkEconomy/`. I also grepped `Clearfell|HarvestStageOverride` across `Tools/MultiRackSitkaP1Lab` (non-Markdown files).
4. Read the current handoff, spec, plan, tasks and checklist at `e97b3780` for stale `8-case`, `290`, override, clearfell, `€15` and "3D-aligned" lines.
5. Made a scratch `--shared` clone outside the repo (`/home/jer/tmp/rr-c6-scratch-*`, removed afterwards) at `e97b3780`, with `.p0ref` copied from the implementer worktree (`SitkaLab.Core.dll` SHA-256 `9a99f781…34b3`, matching the pinned value). Built and ran Core and Save with `/home/jer/.dotnet/dotnet`.
6. **Mutation check:** in the scratch clone only, reverted `FirstThinningCentsPerTonne` to `2_000L`, then rebuilt and re-ran Core.

### Observed results (my own runs)

| Check | Result |
|---|---|
| Core build | `0 Warning(s) 0 Error(s)` |
| Core run | **`291/291 passed, 0 failed`** (rc 0); `PASS economy:` = **9**, `FAIL` = 0 |
| Anchor #7 hash `776a68df…e22d` in Core stdout | **1** |
| Anchor #9 hash `93e9abda…251f` in Core stdout | **1** |
| Save build / run | `0 Warning(s) 0 Error(s)`; **`50/50 passed, 0 failed`** (rc 0) |
| Mutation `2_100L → 2_000L` | **`289/291 passed, 2 failed`** (rc 1): Test 7 `expected 2100, got 2000` and Test 9 `plan 1 uses verified 3D rate: expected 2100, got 2000`. The new rate assertions are non-vacuous. |

## R1–R8

| # | Result | Evidence |
|---|---|---|
| **R1. F-1 rate fixed** | **PASS** | `EconomyModels.cs:127` `FirstThinningCentsPerTonne = 2_100L`. In the 3D source, `Stage1EconomyDefaults.cs:33` `Harvest(FirstThinning, 2000, 2100, 2200)` → `Parameter(low, reference, high, …)`, and `:187` sets `Selected = reference`. So **2100 is the selected value**, not just a value inside the range, and `ForestryWorkCalculator.cs:171` prices with `.Selected`. Second is `2_300L` (`:128`), matching 3D `:34` `(2200, 2300, 2400)`. Later is `2_000L` (`:129`), matching 3D `:35` `(2000, 2000, 2000)`. Both are unchanged in the diff. |
| **R2. No invented tier** | **PASS** | The only `€19` hit at `e97b3780` is the spec §7 note quoting the owner parenthetical and saying it was not adopted (the handoff §7/§8 has the same note). There is no `1_900`/`1900L` in any Lab `src`/`tests` file or the 3D `WorkEconomy/`, and no €19 in any pricing table (spec §2, checklist P2B-08, handoff tables). |
| **R3. Note is now true** | **PASS** | Clause by clause, quoting spec `:88` / handoff §7 below. |
| **R4. No silent history rewrite (C7)** | **PASS** | Handoff `:26` strikes ~~`SetHarvestStageOverride`~~ and marks it **[SUPERSEDED / REMOVED by C5 delta]**. `:107`/`:109` strike ~~€15–€23/t~~ and ~~€20/€23/€20/€15/t~~ with SUPERSEDED markers and state the current `€21/€23/€20/t`. `:118` keeps the original item 3 struck through, followed by **[SUPERSEDED / REMOVED by C5 & C6]** and the current rule. `:135` strikes the old €20 rationale and marks it [SUPERSEDED by C6]. plan.md `:15`, `:27`, `:32` and tasks.md `:8-9` carry the same markers. Nothing was deleted, and nothing presents the override or Clearfell-€15/t as current. |
| **R5. Evidence currency** | **PASS** | Handoff `:38`, `:51`, `:65`, `:93`, plan `:27`/`:32` and tasks `:8-9` all show "8-case"/`290` struck through as SUPERSEDED, with current **9 cases** and **`291/291`**. §8 adds a C6 evidence table that matches my own runs. No unmarked "8-case" or `290` remains. |
| **R6. Floor assertion, no new case** | **PASS** | The assertions are inside the existing Test 9 (`Phase2BEconomyTests.cs:410`). For each of the 3 executed plans they check `settlement.HarvestJob.TotalHarvestContractorCostCents >= MinimumHarvestJobCents` and `settlement.TotalExternalCostCents >= MinimumHarvestJobCents`. A post-loop check asserts `ExecutedHarvestPlansCount == 3` and that every `HarvestJob`-bearing settlement is ≥ €2,500. `T.Add("economy: …")` count = **9**, and the run shows `PASS economy:` = 9. |
| **R7. No regression** | **PASS** | The repair changes only two `src` lines: one constant and one evidence string (`git diff dcba31ca e97b3780 --stat -- Tools/MultiRackSitkaP1Lab/src` = `EconomyModels.cs` 2+/2−). `Clearfell`/`HarvestStageOverride` = 0 across Lab non-Markdown files. `AutoHarvestStage` (`EconomyModels.cs:151-156`) is unchanged, so stage is still derived from history only. Test 9's enum/reflection guards are unchanged, and the per-plan floor is still asserted in Test 7. Upfront cash, contractor-only Fell, 40 h, D-062, the 890 proxy, Off-inertness, save refusal, root append-last and pipeline order are in files the repair does not touch. Their covering Tests 1–6 and 8 pass, and Save is 50/50. |
| **R8. Change confinement** | **PASS** | The 7 changed files are the handoff, `EconomyModels.cs`, `Phase2BEconomyTests.cs`, and spec/plan/tasks/checklist, all within the C6/C7 permit. There is no ecological or arithmetic change and no other rate constant changed (`MinimumHarvestJobCents`, the second/later rates and the material/labour constants are untouched). The test-case count stays at 9. `Assets/` is untouched. |

### R3: clause-by-clause judgement

Quoted from spec §7 `:88` (handoff §7 `:164-171` is equivalent):

- (a) *"**First thinning = €21/t** (`2,100` cents/t; 3D selected reference within the €20–22 range)"*. **True.** It matches `Stage1EconomyDefaults.cs:33`/`:187`.
- (b) *"**€19/t appears in no source** and was not adopted."* **True** per the R2 grep.
- (c) *"Prior Phase 2B-1 revisions (commits `204fcdeb` and `dcba31ca`) used €20/t (the low bound of the €20–22 first-thinning range) and **incorrectly** described `€20/€23/€20` as the "3D-aligned" tiers; this C6 repair corrects both the value and the wording."* **True and explicit.** It names the commits, the value and the incorrect wording.
- (d) *"Provenance note: these are calibration tiers derived from a single published IFA combined harvest+forward range, not an empirical national tariff."* **Does not overstate.** It is if anything more conservative than the 3D source, which tags `SelectedEvidence` as `Empirical` (`:160`). Making a conservative relabel is acceptable.
- The note no longer contains the false "3D-aligned €20" claim.

### Non-blocking observations (no action required for integration)

- **N-1:** Handoff §8 cites `Stage1EconomyDefaults.cs:33` as the place where "`Parameter(...)` sets `Selected = reference`". The `Harvest` row is `:33`, but the `Selected = reference` assignment is at `:187`. This is a cosmetic citation slip and the substance is correct.
- **N-2:** The handoff EMPIRICAL row (`:107`) now calls €20–€24/t "the single published IFA combined harvesting+forwarding range". That is the union of the three tier ranges in the 3D source. The wording is acceptable, but a reader should not take it to mean a single IFA quote of €20–24.
- **N-3 (carried, unchanged):** the D7 notes from the C5 re-review (reflection scope excludes `MultiAreaRoot`/`MultiAreaModelSelection`/`MultiAreaWorkPlanAdapter`, and there is no assertion that the count does not advance on cancel, replay or a non-fell plan) remain optional hardening. The repair corrected the "uses verified 3D rate" message (D7 note ii), which is now true for plan 1.

## Condition status

- **C6: CLOSED.** `FirstThinning = 2_100L` equals the 3D selected value. Dependent tests and strings were updated, and Core 291/291 and Save 50/50 reproduced independently. The mutation proves the rate is pinned by the tests, and the rate note is now accurate.
- **C7: CLOSED.** The `:26`, `:108`, `:117` region lines (now `:26`, `:107`, `:109`, `:118`) and the 8-case/`290` lines are marked SUPERSEDED without deleting history, and 291/291 delta evidence is present (§8).
- **C1: OPEN, integrator duty, unchanged.** Canonical gate stages must still be run on the integration tree.
- **C2: OPEN, integrator duty, unchanged.** The integrator owns the shared-file hunks, including the `Commands.Plan.cs` removal hunks from C5.

## INTEGRATION DECISION (C6 repair re-review)

**`INTEGRATION DECISION: APPROVED WITH CONDITIONS — C1, C2 (integrator duties; no remaining implementer-side blocking condition)`**

This is an independent review opinion and **not a merge authorisation**. Integration remains the integrator's and owner's decision, subject to C1 and C2.

## LIMITS (C6 repair re-review)

- **The canonical `build/2d-gate.sh` was not run.** G0–G7 and browser tier B are **NOT TESTED** (still blocked by the missing Linux `node` on the WSL PATH, per the handoff). IP-1 from earlier sections still applies.
- **NOT TESTED:** UI smoke, `cli-safety.sh`, `build-output-safety.sh`, `p0-isolation.sh`, G5 payload budgets, and any HTTP/UI/runtime/browser execution.
- I ran only Core and Save outside the gate, plus one mutation, in a scratch clone outside the repo that was removed afterwards. **Anchors #1–#6 and #8 were not recomputed.** Only #7 and #9 were checked, by hash count in Core stdout.
- No Unity Editor, Blender or rendered verification. None applies, and `Assets/` is untouched.
- I made no ecological or market judgement on whether €21/t is the "right" first-thinning rate. I only checked it against the in-repo 3D source's selected value.
- No merge, rebase, cherry-pick or force push. No implementation, spec, test or handoff file was edited. This review file is the only file written to the review branch.
