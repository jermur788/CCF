# Multi Stage 4 — adult Sitka parity, natural mortality and fallen deadwood: implementation result

| Field | Value |
|---|---|
| Date | 2026-11-23 |
| Role | Implementation sub-agent, sole writer for this lane |
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4-AdultParity-20261123` |
| Branch | `feat/multi-stage4-adult-parity-20261123-v1` (committed locally, **not pushed**) |
| Base | `4e60c8ab806df56441497b682ccf783badc6e0ea` (Stage 1–3, not merged to `main`) |
| Code HEAD | `ae5a2a568a28279fe01aa0634fe6d0f72dc0e6b5` (engine; unchanged by revision 2) |
| Revision | 2 — test-qualification repair after independent review NEEDS CHANGES (tests + this handoff only, one commit on top of `592dc7bd`) |
| Authority | `origin/docs/multi-899-900-approved-spec-20261123-v1`: `specs/multi-stage4-adult-sitka-parity-v1/{spec,plan,tasks}.md`, `checklists/requirements.md`, decision D-095 |
| Tools used | Bash (Git Bash → `wsl.exe`), file read/write/edit. **No Unity or Editor, no browser, no `/multi` visual check.** |
| Statuses | **IMPLEMENTED: yes.** **NUMERICALLY QUALIFIED: yes, against the author's own independent oracles (not yet confirmed by a second party).** **REVIEWED: no.** Not ACCEPTED. |

## 0. Revision 2 — qualification repair (review NEEDS CHANGES: two major, one minor; no production defect)

Only `tests/Core.Tests/Stage4BehaviourTests.cs` and this handoff changed. **No `src/**` change; no production defect was found** while doing the repair: every engine value matched the new independent oracles bit for bit.

| Review finding | Repair | Evidence |
|---|---|---|
| Major 1 — boundary deaths never reached the deadwood integration assertions (M-03 used `AnnualRules` directly; D-01 stays below RD 1.0) | New **S4 D-02**: an overstocked two-rack Stage 4 *session* (§7.3a) with 18 boundary and 63 hazard victims over three years, every victim checked end to end | §7.3a |
| Major 2 — M-02/M-03 built expectations from `report.PreGrowthCi` / `report.Deaths` | M-02 and M-03 rewritten on a new independent oracle `ExpectMortality` built from the *input trees only*; compared to the report afterwards. `report.PreGrowthCi`, `.Suppression`, `.DeathProbability`, `.MortalityRoll` and `.Deaths` are never read to build an expectation | §7.2 |
| Minor — P-04 overclaimed "Execute revalidates and refuses all" | Renamed to what it proves: **"consented Year+1 cancels an approved Prune at zero lifts; Execute of the cancelled plan is refused"**. Execute-time guard revalidation / whole-plan atomic refusal is **not** claimed as tested (§9 item 12) | §7.5 |

Test count: **180/180** (was 179; +1 = D-02; M-02, M-03 and P-04 rewritten/renamed in place).

## 1. Commits

```text
07e424ee feat(multi-stage4): AdultModelVersion/MortalityModelVersion selection, defaults 0, zero drift
40f4960d feat(multi-stage4): woodland-CI adult growth, per-rack mortality with deadwood, fenced Prune
ae5a2a56 test(multi-stage4): growth seams, RD/hazard/boundary oracles, cross-rack, deadwood, pruning, determinism
592dc7bd docs(multi-stage4): Stage 4 engine implementation result handoff
(rev 2)  test(multi-stage4): independent mortality oracle, boundary-death session deadwood, P-04 claim — the commit carrying this revision
```

## 2. Changed files (base..code HEAD)

```text
Tools/MultiRackSitkaP1Lab/src/Core/AnnualRules.cs                    modified: shared CalculateCore, CalculateStage4, GrowStage4
Tools/MultiRackSitkaP1Lab/src/Core/CrownModel.cs                     modified: pruned C2 target / step overloads (additive)
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs modified: model selection ctor, Stage 4 root state,
                                                                       AddPrune/RemovePrune, Approve/Execute/Advance, ReadStage4View
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaExperiment.cs   modified: three new Stage 4 identity constants only
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaStage4.cs       new: Stage 4 root state, Prune order/record, read-out records
Tools/MultiRackSitkaP1Lab/src/Core/Stage4Growth.cs                    new: site-productivity field, GrowthAgePhase(s), Stage4Options
Tools/MultiRackSitkaP1Lab/src/Core/Stage4Models.cs                    new: AdultModelVersion, MortalityModelVersion, LifeState,
                                                                       MultiAreaModelSelection, LifeStates, Stage4Evidence
Tools/MultiRackSitkaP1Lab/src/Core/Stage4Records.cs                   new: TreePruningState, PruningRules, FallenDeadwoodRecord
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Model0FingerprintTests.cs  modified: recipe split into MultiAreaFingerprintOf(session)
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs                 modified: one registration line
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Stage4Tests.cs             new: zero-drift and selection tests
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Stage4BehaviourTests.cs    new: behaviour oracles
Docs/Handoffs/MultiRack-Simulation-20261116/899-900/<this file>       new
```

Not touched: `src/Ui/**`, `Assets/**`, fixtures, `.sln`, `global.json`, build scripts, `.p0ref`, saves, `main`, the Stage 1–3 branch, the shared repo checkout. No package, schema or save change. Every new type is `internal` (the existing "public #849 surface unchanged" test still passes).

## 3. Spec conformance (D-095 / spec §2–§4)

| Requirement | Where | Evidence |
|---|---|---|
| `AdultModelVersion {Legacy=0, ConnectedWoodland=1}`, `MortalityModelVersion {Legacy=0, RecordedDeadwood=1}`; Stage 4 only with 1/1/1; every default 0/0/0 | `Stage4Models.cs`, `MultiAreaContinuation` ctor | S4 V-01, V-03. Partial selections refused `Stage4ModelIncomplete` (see §9 item 4). |
| Model 0 and Stage 1–3 (adult 0 + crown 1) byte-identical; no new per-tree fields in the model 0 fingerprint | `AnnualTree` record unchanged; Stage 4 state lives in the internal root | §5 hashes; S4 V-01, V-02 |
| DBH: `1.2·site·clamp01(1−DBH/120)` with woodland CI, clamp [1,200] | `AnnualRules.GrowStage4` | S4 G-01 (bit-equal to the frozen law fed with woodland CI), X-01 |
| Height: `(NextHeight(h,age)−h)·site`; age +1 | `GrowStage4` | S4 G-01, G-02 |
| `ApplyGrowthAgeFactors` seam; Sitka 1.0/1.0; synthetic table proves it | `GrowthAgePhases`, `Stage4Options` | S4 G-03 |
| CI_woodland = 8 m Hegyi over all living trees, ordinal TreeId | `CrownModel.Woodland` + `AnnualRules.Competition` (existing) | S4 G-01, X-01, P-02 (independent Hegyi oracle) |
| Per-rack RD / P / S / p / roll / boundary exactly as AnnualRules; rack CI and rack polygon area; never pooled | `AnnualRules.CalculateCore` (now shared by model 0, Stage 1–3 and Stage 4) | S4 M-01..M-04 (M-02/M-03 on the independent oracle), D-01, D-02, R-01 |
| Biological death: cause `DensitySuppression`, current year, idempotent; `LifeState {Living, FelledStump, BiologicallyDead}`; not a stump; no timber; leaves CI, growth, crown union, light | `LifeStates.ApplyBiologicalDeath`, `LifeStates.Of` | S4 M-05, D-01, D-02 (incl. boundary victims; canopy/light compared with victims deleted) |
| One `FallenDeadwoodRecord` per death with the 11 specified fields; volume `DBH²·0.00007854·(H·0.5)` float32 | `FallenDeadwoodRecord`, `Advance` | S4 D-01 (hazard deaths), **D-02 (hazard + boundary victims, fields from the independent oracle)**, M-05 |
| Canopy/light invalidated after mortality | existing cache key on `AnnualRevision`/`WorkRevision` | S4 D-01, P-01 (recompute counter) |
| Pruning state, fenced Prune: Pending only, Approve zero mutation, Execute one lift without advancing the year, canopy invalidated | `AddPrune`, `RemovePrune`, `Approve`, `Execute` | S4 P-01; P-04 (consented cancellation at zero lifts and refusal of the cancelled plan only) |
| Guards (living, lifts<3, ≥5 y, target>base, 0<target<0.6·h) with stable codes | `PruningRules.RefusalCode` (Unity check order) | S4 P-03 |
| Execute `r=max(0.1,r·0.92)`; future target `max(0.1,(r_pot/√(1+CI/5))·0.92^lifts)` | `PruningRules.CutRadiusM`, `CrownModel.PrunedTargetRadiusM` | S4 P-01, P-02 |
| Read-outs and labels without changing existing positional records | separate `ReadStage4View()` and new records | S4 R-01; UI smoke W section unchanged (§6) |

Stable refusal codes added: `Stage4ModelIncomplete`, `Stage4ModelRequired`, `AdultModelUnknown`, `MortalityModelUnknown`, `SiteProductivityInvalid`, `GrowthAgePhasesInvalid`, `BiologicalDeathInvalid`, `PruneTreeNotLiving`, `PruneMaxLiftsReached`, `PruneIntervalTooShort`, `PruneTargetNotAboveCrownBase`, `PruneTargetOutOfRange`. Existing codes reused: `WorkDuplicateTreeId`, `WorkPlanAlreadyApproved`, `UnknownWorkOrder`, `MultiAreaWrongArea`, `UnknownTreeId`, `WorkPendingConsentRequired`, `WorkEmptyPlan`.

## 4. Commands and results

All in WSL from `Tools/MultiRackSitkaP1Lab`, `/home/jer/.dotnet/dotnet` 8.0.134, `.p0ref/SitkaLab.Core.dll` sha256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (checked before every run; `build-p0-core-ref.sh` not run).

```bash
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln            # Build succeeded, 0 Warning(s), 0 Error(s)
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
                                                                  # rev 2: 180/180 passed, 0 failed (baseline 160; +20 Stage 4)
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj           # Build succeeded, 0 Warning(s), 0 Error(s)
DOTNET=/home/jer/.dotnet/dotnet bash tests/ui-smoke-multiarea.sh  # rev 1 only: 107 passed, 2 failed (J01, J99) — identical at base, see §6
```

Rev 2 reran the solution build (Core, Cli, Core.Tests: 0/0), the Core suite (180/180) and the Ui build (0/0), with `TMPDIR=/tmp/s4fix/tmp` outside the repo. The UI smoke and `build-p0-core-ref.sh` were **not** run in rev 2 (instructed; no `src/` change).

Runtime: the whole Core suite runs in 4.5–5.1 s wall time; the multi-area UI smoke in about 11 s.

## 5. Zero drift and pinned hashes

| Fingerprint | Value | Result |
|---|---|---|
| Model 0 single-area (F-01) | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | unchanged |
| Model 0 MA-1 (F-02, default ctor and explicit 0/0/0) | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | unchanged |
| Stage 1–3 CrownModel 1 MA-1 (adult 0 + crown 1; crown-only ctor and explicit 0/0/1) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | captured on base before any change, now pinned in `Stage4Tests` (S4 V-02); unchanged |
| P0 DLL pin | `9a99f781…34b3` | unchanged |
| Stage 4 fixed script (Fells years 0 and 3, Prunes years 0 and 5, ten years, incl. Stage 4 read-out) | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | identical on two runs (S4 R-02) |

Rev 2: all five values above re-observed in the 180/180 run, unchanged (F-01, F-02, V-01/V-02 pins, R-02 print, `sha256sum .p0ref/SitkaLab.Core.dll` before and after).

The model 0 refactor (one shared `CalculateCore` taking a growth function and a cause string) is proven bit-neutral by F-01, F-02 and V-02.

## 6. UI / `/multi` status

- `src/Ui` was not edited, builds 0/0 and the default page stays on model 0 (no adapter method was added; Stage 4 is reachable only from code and tests).
- `ui-smoke-multiarea.sh` on code HEAD: 107 pass. The M (headers, pinned manifest hash), G (guards) and W (full JSON workflow, every route 200/expected refusal) sections pass; Z02 "server wrote nothing to stderr", Z03 "no file changed in the lab tree", Z04 "git status unchanged" pass.
- **Pre-existing failure, not introduced here:** J01 "timed out waiting for the first view (Area A, 171 rows)" and J99 (its consequence). I reran the same script twice on base `4e60c8ab` (temporary detached checkout of this clean worktree, then restored): identical 107/2 both times. The Stage 1–3 handoff records that no UI smoke was run there. Cause UNKNOWN; investigating it would need UI/test-harness edits, which are out of my scope.
- The three live listeners (PIDs 839989, 1750123, 1806361) were not touched; the smoke binds its own OS-assigned port and kills only its own PID. They were still listening after every run.

## 7. Evidence tables

### 7.1 Cross-rack growth (S4 X-01)

Two touching 20 × 20 m racks; `MA-A-002` (DBH 60 cm) is 4 m from the Area B border tree `MA-B-001`; `MA-B-003` is 11 m from it. One year after felling `MA-A-002` versus not felling:

| Tree | Woodland CI keep → fell | DBH increment keep → fell (cm) | Stage 1–3 (adult 0) increment keep → fell |
|---|---|---|---|
| MA-B-001 (4 m) | 0.892857 → 0.142857 | 0.848484 → 0.972221 | 0.972221 → 0.972221 |
| MA-B-003 (11 m) | 0.142857 → 0.142857 | 0.972221 → 0.972221 (bit-identical) | 0.972221 → 0.972221 |

### 7.2 RD / hazard / boundary oracles (S4 M-01..M-04)

**Independent mortality oracle (rev 2, `ExpectMortality` / `AssertMortality`).** The expectation is computed from the input start-of-year trees only, before or without reading the report:

1. rack CI = the test's own 8 m Hegyi (`HegyiOracle`, ordinal TreeId, 0.5 m floor, `max(1, DBH_target)`) over the rack's living trees; woodland CI = the same over the connected woodland;
2. grown DBH = `clamp(DBH + 1.2·1·clamp01(1−DBH/120)·(1/(1+CI_woodland/5)), 1, 200)`; grown H = `clamp(H + (NextHeight(H, age) − H)·1, 0.1, 60)` (the frozen `SitkaGrowthModel.NextHeight` law is the only shared growth helper);
3. post-growth RD = `(float)(N/areaHa)·(Dq/25)^2.063/1868` on the grown stems; `P = max(0, (RD−0.6)/(1−0.6))`;
4. `S = 1 − 1/(1 + CI/5)`, `V = clamp01(S)^4`, mean V over the rack, `p = min(0.5, 0.08·P·P·V/meanV)` (0 when mean V = 0);
5. hazard victims: `SimulationRandom.Roll(RngModelVersion, "ADULT-MORT-"+TreeId, year, seed) < p`, in TreeId order (rolls only where p > 0);
6. boundary victims: the oracle's own hazard survivors, sorted S desc then TreeId asc, killed while `count > 1` and RD > 1.0 (running Σ DBH² as written);
7. final RD from the oracle survivors.

`AssertMortality` then compares, bit for bit: the living roster, post-growth RD, P, mean V, and per tree rack CI, grown DBH, grown height, S, p, roll (presence and value) and life state; then the **complete ordered victim list** (hazard in TreeId order, then boundary in kill order, with mechanism) and the final RD. No `report.PreGrowthCi`, `.Suppression`, `.DeathProbability`, `.MortalityRoll` or `.Deaths` value feeds the expectation.

| Case | Result |
|---|---|
| RD < 0.6 (16 stems, 0.1 ha) | pressure 0, no probability, no roll, no death |
| 0.6–1.0 (M-02, 100 stems, 0.04 ha, seed 4242) | oracle RD 0.6371, P 0.0926, p 0.000211–0.001240 (all > 0); report equal to the oracle bit for bit on every field above; p monotone in the oracle S; 0 hazard expected, 0 reported; no boundary |
| > 1.0 (M-03, 100 stems, 0.04 ha, seed 777) | oracle post-growth RD 2.1438; oracle predicts 46 hazard then 8 boundary victims, final RD 0.9920 ≤ 1.0, RD before the last boundary kill > 1.0; the report's ordered death list, values and final RD are identical |
| Tie (five stems with S = 0) | zero hazard (mean V = 0); boundary kills `MA-T-001..004` in TreeId order; one survivor kept |

### 7.3a Boundary-death session (S4 D-02, rev 2)

Synthetic two-rack Stage 4 session `mad-s4-boundary-test` (not MA-1): two touching 20 × 20 m racks (0.04 ha each). **MA-A**: 10 × 10 grid at 2 m, age 40, DBH 40–46 cm, H 25 m, seed 601 — deliberately extreme so that, with p capped at 0.5, hazard alone cannot bring RD under 1.0 and boundary victims are certain in year 1 (a first attempt at the M-03 stocking, RD ≈ 2.14, lost 56 stems to hazard and fell to RD 0.936 with no boundary kill, so it was replaced). **MA-B**: 4 × 4 grid at 5 m, DBH 20 cm, seed 602; its west column is 3.5 m from MA-A's east column, so woodland CI crosses the border. No work, three Year+1 steps.

| Year | Area | Post-growth RD | Hazard | Boundary | Final RD | New deadwood |
|---|---|---|---|---|---|---|
| 1 | MA-A | 4.1653 | 58 | 18 | 0.9998 | 76 (DW0001..DW0076) |
| 1 | MA-B | 0.1457 | 0 | 0 | 0.1457 | 0 |
| 2 | MA-A | 1.0279 | 2 | 0 | 0.9460 | 2 (DW0077..DW0078) |
| 2 | MA-B | 0.1573 | 0 | 0 | 0.1573 | 0 |
| 3 | MA-A | 0.9725 | 3 | 0 | 0.8363 | 3 (DW0079..DW0081) |
| 3 | MA-B | 0.1693 | 0 | 0 | 0.1693 | 0 |

Totals: 63 hazard + 18 boundary = 81 victims = 81 deadwood records, IDs DW0001..DW0081 contiguous. Year 2 is above RD 1.0 after growth but hazard alone brings it to 0.946, so the boundary correctly does nothing.

First and last boundary victims (year 1):

| DeadwoodId | TreeId | X, Z (m) | Cause | DBH (cm) | H (m) | Volume (m³) | S |
|---|---|---|---|---|---|---|---|
| DW0059 | MA-A-056 | 11.000, 11.000 | DensitySuppression | 42.248 | 25.369 | 1.778229 | 0.681488 |
| DW0076 | MA-A-049 | 17.000, 9.000 | DensitySuppression | 42.287 | 25.369 | 1.781485 | 0.627622 |

Checked every year, for every rack and every victim (hazard and boundary):

- the engine report equals the independent oracle (§7.2) on every field, including the complete ordered victim set;
- exactly one new `FallenDeadwoodRecord` per victim, in area order then hazard (TreeId) then boundary (kill order); `DeadwoodId` = the next sequential `DWnnnn`; TreeId, AreaId, `sitka-spruce`, X/Z (bits equal to the start tree), death year, `DensitySuppression`, DBH and height bit-equal to the **oracle's** grown values, and the float32 volume `DBH·DBH·0.00007854f·(H·0.5f)`;
- earlier records are unchanged (record equality), IDs unique across the session and never reused after further annual steps; one record per tree ever; no record without a victim; total records = total victims;
- the victim is `BiologicallyDead` with cause and year in the Stage 4 read-out, never `FelledStump`, has no Crop mark; the session has no work event and no Fell record (the multi-area contracts have no timber/value field outside a Fell receipt, so no harvest or timber value can exist for a victim);
- next year's woodland CI set excludes every victim, and each living tree's woodland CI equals the oracle Hegyi over the roster with the victims deleted outright;
- the session's canopy/light read-out (`ReadCanopyLight`) equals `CanopyLightEvaluator.Evaluate` on the same racks with the victims **deleted from the input** (every growing-space row, stand, cover record and every light cell); and evaluating with this year's victims revived (`Death = null`) changes cover or light, so the comparison is sensitive.

### 7.3 Overstocked vs thinned Area A, ten years (S4 D-01, MA-1 experiment, Stage 4)

"Thinned" fells every second living Area A stem in year 0. RD is each rack's own living stems over its own polygon area.

| Year | A RD / deaths (overstocked) | A RD / deaths (thinned) | B RD / deaths (overstocked) | B RD / deaths (thinned) |
|---|---|---|---|---|
| 1 | 0.808 / 5 | 0.425 / 0 | 0.456 / 0 | 0.457 / 0 |
| 2 | 0.796 / 10 | 0.448 / 0 | 0.476 / 0 | 0.476 / 0 |
| 3 | 0.808 / 4 | 0.472 / 0 | 0.495 / 0 | 0.495 / 0 |
| 4 | 0.834 / 2 | 0.496 / 0 | 0.514 / 0 | 0.515 / 0 |
| 5 | 0.822 / 8 | 0.521 / 0 | 0.534 / 0 | 0.535 / 0 |
| 6 | 0.832 / 4 | 0.546 / 0 | 0.554 / 0 | 0.555 / 0 |
| 7 | 0.826 / 8 | 0.571 / 0 | 0.575 / 0 | 0.575 / 0 |
| 8 | 0.833 / 5 | 0.597 / 0 | 0.596 / 0 | 0.596 / 0 |
| 9 | 0.835 / 5 | 0.623 / 0 | 0.616 / 0 | 0.617 / 0 |
| 10 | 0.857 / 2 | 0.650 / 0 | 0.637 / 0 | 0.638 / 0 |

Overstocked Area A: 53 natural deaths, 53 deadwood records. Thinned Area A: 0. Area B's RD differs slightly between runs only because Area B border trees grow on the woodland CI (Area A thinning releases them); Area B's RD is computed from Area B alone every year (checked against an independent oracle and against the pooled A+B value).

Each year, for every death, the test checks: exactly one deadwood record; `BiologicallyDead`, cause `DensitySuppression`, death year; record fields equal the dead stem's ID, area, species, position, DBH and height; float32 volume formula; no Fell record and no Crop mark for it; it is absent from the next year's woodland CI set and from the canopy/growing-space read-out; and the year adds no work event.

### 7.4 Deadwood records (first three of 53, overstocked run)

| DeadwoodId | TreeId | Area | Species | X, Z (m) | Year | Cause | DBH (cm) | H (m) | Volume (m³) |
|---|---|---|---|---|---|---|---|---|---|
| DW0001 | MA-A-043 | MA-A | sitka-spruce | 32.435, 7.840 | 1 | DensitySuppression | 16.761 | 16.128 | 0.177927 |
| DW0002 | MA-A-073 | MA-A | sitka-spruce | 30.341, 11.683 | 1 | DensitySuppression | 23.372 | 17.840 | 0.382669 |
| DW0003 | MA-A-119 | MA-A | sitka-spruce | 27.469, 19.227 | 1 | DensitySuppression | 23.786 | 17.943 | 0.398671 |

### 7.5 Pruning (S4 P-01..P-04)

- `MA-B-073` (lowest woodland CI in Area B, H 21.275 m): Pending and Approve leave every tree, the year, the work revision and the canopy cache unchanged; Execute gives r 2.961405 → 2.724493 m (= `max(0.1, r·0.92f)` bit-exact), lifts 1, crown base 3 m, last year 0, ecological year still 0, annual revision unchanged, work revision +1, canopy recomputed once on the next read; all other trees identical; an identical retry is a replay with no second cut.
- Next year's radius equals `max(0.1, r + 0.15·(max(0.1, r_pot/√(1+CI/5)·0.92¹) − r))` with an independent Hegyi oracle, bit-exact.
- Refusals: model 0 and Stage 1–3 sessions `Stage4ModelRequired`; target 0 or −1 `PruneTargetNotAboveCrownBase` (Unity order); target = 0.6·h, NaN, +∞ `PruneTargetOutOfRange`; a lift at years 0 and 4 after a year-0 lift `PruneIntervalTooShort`; year 5 accepted; target ≤ the current base refused; lifts at years 0, 5, 10 then a fourth `PruneMaxLiftsReached`; a naturally dead and a felled tree `PruneTreeNotLiving`; Fell/Prune on the same tree `WorkDuplicateTreeId`; Year+1 with a pending Prune needs consent (P-03).
- P-04 (rev 2 wording): Year+1 with an *approved* Prune is refused without consent (`WorkPendingConsentRequired`); with consent it cancels the plan with zero lifts, no pending/approved order and no prune record; a later Execute of the cancelled plan's ID/key is refused `WorkPlanNotApproved`. P-04 does **not** test Execute-time guard revalidation or whole-plan atomic refusal (§9 item 12).

## 8. Determinism

Identical seed and commands give a bit-identical ten-year Stage 4 output, including Fells, Prunes, deaths, deadwood and the read-out (S4 R-02, hash in §5). G-01 also shows an explicit uniform 1.0 site field is bit-identical to the default.

## 9. Known limits, choices and UNKNOWNs (reviewer attention)

1. **Age-phase seam order versus Unity.** Unity `GrowAdults` applies `ApplyGrowthAgeFactors` before replacing Sitka height growth with `NextHeight`, so in Unity a height factor would be discarded for Sitka. The approved spec (§2.2 step 3) lists the seam after the height increment, and I followed the spec: the factors scale both increments. Production Sitka has no phases, so this is inert today; it only matters if a Sitka phase table is ever authored.
2. **DBH response form.** I wrote `dbhPotential · (1/(1+CI/5))` (the Unity and frozen engine form) rather than the literal `dbhPotential / (1+CI/5)`. They are mathematically equal; the chosen form keeps bit parity with Unity and with the frozen rule (G-01).
3. **Height × site** is the D-095 2D extension. Unity's Sitka height ignores site. At 1.0 they match.
4. **Partial selections refused.** The spec defines Stage 4 only at 1/1/1, so 1/0/x, 0/1/x and 1/1/0 are refused (`Stage4ModelIncomplete`) rather than given an unspecified behaviour.
5. **Prune order fields.** `{AreaId, TreeId, TargetCrownBaseHeightM}`, with no reason (the spec lists TreeId + target; Fell still requires a reason). One order per tree per plan (Fell xor Prune). A retained Crop Tree may be pruned. Guards are checked at Mark (clean early refusal) and again at Execute against the current year; any failure refuses the whole plan.
6. **Deadwood IDs and values.** `"DW" + 4-digit session counter` (Unity `ScenarioOneManager` numbering), assigned in area order, then in the order the rules record the deaths (hazard in TreeId order, then boundary victims). DBH and height are the post-growth values of the death year (Unity kills after growth).
7. **Read-out wiring.** `ReadStage4View()` is a separate engine read. It is not exposed through `MultiAreaWorkPlanAdapter` or HTTP, because the UI must stay on model 0 and `src/Ui` is read-only. On model 0 / Stage 1–3 sessions it reports life states and RD with no pruning or deadwood; model 0 natural deaths ("self-thinning") read as `BiologicallyDead`.
8. **Single-area engine.** Stage 4 is wired into the multi-area session only; `AnnualEngine`/`AnnualContinuation` are unchanged.
9. **Mid-range realised deaths.** In the 0.6–1.0 oracle stand the probabilities are tiny (≤ 0.00124), so the suppression bias is shown on probabilities, not realised deaths. Realised hazard deaths come from MA-1 Area A (RD 0.80–0.86).
10. **Not run:** `tests/p0-isolation.sh` (its cold rebuild could rebuild `.p0ref` and break the pin), `build-output-safety.sh`, `cli-safety.sh`, single-area `ui-smoke.sh`. No browser, no rendered `/multi` review, no Unity.
11. **Pre-existing UI smoke J01/J99 failure** (§6): cause UNKNOWN; present on base.
12. **Execute-time Prune revalidation is implemented but not directly tested** (rev 2 correction). The code re-checks Prune guards at Execute and refuses the whole plan on any failure (item 5), but no test constructs a plan whose guard becomes false between Approve and Execute (Year+1 cancels approved work, so the natural route is unavailable). Status: UNTESTED.
13. **D-02 stocking is deliberately unrealistic** (40–46 cm stems at 2 m, RD ≈ 4). It is a qualification fixture for the boundary path, not an ecological scenario.
14. **Oracle sharing (rev 2).** The independent mortality oracle shares only `SitkaGrowthModel.NextHeight` (the frozen height law, needed for the victim's recorded height) and `SimulationRandom.Roll` (the prescribed RNG) with the engine. It mirrors the written float32/double expression order on purpose, so bit equality is meaningful; a reviewer may still prefer a second-party oracle.

## 10. How to reproduce

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4-AdultParity-20261123/Tools/MultiRackSitkaP1Lab
sha256sum .p0ref/SitkaLab.Core.dll        # expect 9a99f781…34b3
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll | grep -E 'fingerprint|S4|passed'
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
DOTNET=/home/jer/.dotnet/dotnet bash -lc 'bash tests/ui-smoke-multiarea.sh'   # node must be on PATH (login shell)
```

Expected: 180/180 (rev 2); the four hashes in §5; Ui 0/0; UI smoke 107 pass with only J01/J99 failing, as on base.

## 11. Next step

An independent review by a different provider, against the approved quartet and §9, before owner acceptance. No merge, no push.
