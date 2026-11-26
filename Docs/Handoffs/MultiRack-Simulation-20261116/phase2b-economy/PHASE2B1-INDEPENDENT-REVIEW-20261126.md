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
