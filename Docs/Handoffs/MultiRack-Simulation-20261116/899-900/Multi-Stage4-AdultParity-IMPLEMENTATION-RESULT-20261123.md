# Multi Stage 4 — adult Sitka parity, natural mortality and fallen deadwood: implementation result

| Field | Value |
|---|---|
| Date | 2026-11-23 |
| Role | Implementation sub-agent, sole writer for this lane |
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4-AdultParity-20261123` |
| Branch | `feat/multi-stage4-adult-parity-20261123-v1` (committed locally, **not pushed**) |
| Base | `4e60c8ab806df56441497b682ccf783badc6e0ea` (Stage 1–3, not merged to `main`) |
| Code HEAD | `ae5a2a568a28279fe01aa0634fe6d0f72dc0e6b5` (this handoff is committed on top of it) |
| Authority | `origin/docs/multi-899-900-approved-spec-20261123-v1`: `specs/multi-stage4-adult-sitka-parity-v1/{spec,plan,tasks}.md`, `checklists/requirements.md`, decision D-095 |
| Tools used | Bash (Git Bash → `wsl.exe`), file read/write/edit. **No Unity or Editor, no browser, no `/multi` visual check.** |
| Statuses | **IMPLEMENTED: yes.** **NUMERICALLY QUALIFIED: yes, against the author's own independent oracles (not yet confirmed by a second party).** **REVIEWED: no.** Not ACCEPTED. |

## 1. Commits

```text
07e424ee feat(multi-stage4): AdultModelVersion/MortalityModelVersion selection, defaults 0, zero drift
40f4960d feat(multi-stage4): woodland-CI adult growth, per-rack mortality with deadwood, fenced Prune
ae5a2a56 test(multi-stage4): growth seams, RD/hazard/boundary oracles, cross-rack, deadwood, pruning, determinism
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
| Per-rack RD / P / S / p / roll / boundary exactly as AnnualRules; rack CI and rack polygon area; never pooled | `AnnualRules.CalculateCore` (now shared by model 0, Stage 1–3 and Stage 4) | S4 M-01..M-04, D-01, R-01 |
| Biological death: cause `DensitySuppression`, current year, idempotent; `LifeState {Living, FelledStump, BiologicallyDead}`; not a stump; no timber; leaves CI, growth, crown union, light | `LifeStates.ApplyBiologicalDeath`, `LifeStates.Of` | S4 M-05, D-01 |
| One `FallenDeadwoodRecord` per death with the 11 specified fields; volume `DBH²·0.00007854·(H·0.5)` float32 | `FallenDeadwoodRecord`, `Advance` | S4 D-01 (every year, every death), M-05 |
| Canopy/light invalidated after mortality | existing cache key on `AnnualRevision`/`WorkRevision` | S4 D-01, P-01 (recompute counter) |
| Pruning state, fenced Prune: Pending only, Approve zero mutation, Execute one lift without advancing the year, canopy invalidated | `AddPrune`, `RemovePrune`, `Approve`, `Execute` | S4 P-01, P-04 |
| Guards (living, lifts<3, ≥5 y, target>base, 0<target<0.6·h) with stable codes | `PruningRules.RefusalCode` (Unity check order) | S4 P-03 |
| Execute `r=max(0.1,r·0.92)`; future target `max(0.1,(r_pot/√(1+CI/5))·0.92^lifts)` | `PruningRules.CutRadiusM`, `CrownModel.PrunedTargetRadiusM` | S4 P-01, P-02 |
| Read-outs and labels without changing existing positional records | separate `ReadStage4View()` and new records | S4 R-01; UI smoke W section unchanged (§6) |

Stable refusal codes added: `Stage4ModelIncomplete`, `Stage4ModelRequired`, `AdultModelUnknown`, `MortalityModelUnknown`, `SiteProductivityInvalid`, `GrowthAgePhasesInvalid`, `BiologicalDeathInvalid`, `PruneTreeNotLiving`, `PruneMaxLiftsReached`, `PruneIntervalTooShort`, `PruneTargetNotAboveCrownBase`, `PruneTargetOutOfRange`. Existing codes reused: `WorkDuplicateTreeId`, `WorkPlanAlreadyApproved`, `UnknownWorkOrder`, `MultiAreaWrongArea`, `UnknownTreeId`, `WorkPendingConsentRequired`, `WorkEmptyPlan`.

## 4. Commands and results

All in WSL from `Tools/MultiRackSitkaP1Lab`, `/home/jer/.dotnet/dotnet` 8.0.134, `.p0ref/SitkaLab.Core.dll` sha256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (checked before every run; `build-p0-core-ref.sh` not run).

```bash
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln            # Build succeeded, 0 Warning(s), 0 Error(s)
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
                                                                  # 179/179 passed, 0 failed (baseline 160; +19 Stage 4)
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj           # Build succeeded, 0 Warning(s), 0 Error(s)
DOTNET=/home/jer/.dotnet/dotnet bash tests/ui-smoke-multiarea.sh  # 107 passed, 2 failed (J01, J99) — identical at base, see §6
```

Runtime: the whole Core suite runs in 4.5–5.1 s wall time; the multi-area UI smoke in about 11 s.

## 5. Zero drift and pinned hashes

| Fingerprint | Value | Result |
|---|---|---|
| Model 0 single-area (F-01) | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | unchanged |
| Model 0 MA-1 (F-02, default ctor and explicit 0/0/0) | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | unchanged |
| Stage 1–3 CrownModel 1 MA-1 (adult 0 + crown 1; crown-only ctor and explicit 0/0/1) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | captured on base before any change, now pinned in `Stage4Tests` (S4 V-02); unchanged |
| P0 DLL pin | `9a99f781…34b3` | unchanged |
| Stage 4 fixed script (Fells years 0 and 3, Prunes years 0 and 5, ten years, incl. Stage 4 read-out) | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | identical on two runs (S4 R-02) |

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

| Case | Result |
|---|---|
| RD < 0.6 (16 stems, 0.1 ha) | pressure 0, no probability, no roll, no death |
| 0.6–1.0 (100 stems, 0.04 ha) | RD 0.6371 = oracle exactly, P 0.0926, p 0.000211–0.001240 matches `min(0.5, 0.08·P²·S⁴/mean S⁴)` within 1e-15; p monotone in S; every roll equals `SimulationRandom.Roll(1, "ADULT-MORT-"+id, year, seed)`; 0 realised deaths at these p |
| > 1.0 (100 stems, 0.04 ha) | post-growth RD 2.1438; 46 hazard deaths, then 8 boundary deaths in exact S-desc/TreeId-asc order; final RD 0.9920 ≤ 1.0 and the last victim was necessary |
| Tie (five stems with S = 0) | zero hazard (mean V = 0); boundary kills `MA-T-001..004` in TreeId order; one survivor kept |

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
- Refusals: model 0 and Stage 1–3 sessions `Stage4ModelRequired`; target 0 or −1 `PruneTargetNotAboveCrownBase` (Unity order); target = 0.6·h, NaN, +∞ `PruneTargetOutOfRange`; a lift at years 0 and 4 after a year-0 lift `PruneIntervalTooShort`; year 5 accepted; target ≤ the current base refused; lifts at years 0, 5, 10 then a fourth `PruneMaxLiftsReached`; a naturally dead and a felled tree `PruneTreeNotLiving`; Fell/Prune on the same tree `WorkDuplicateTreeId`; Year+1 with a pending or approved Prune needs consent, and consent cancels it at zero lifts.

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

## 10. How to reproduce

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4-AdultParity-20261123/Tools/MultiRackSitkaP1Lab
sha256sum .p0ref/SitkaLab.Core.dll        # expect 9a99f781…34b3
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll | grep -E 'fingerprint|S4|passed'
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
DOTNET=/home/jer/.dotnet/dotnet bash -lc 'bash tests/ui-smoke-multiarea.sh'   # node must be on PATH (login shell)
```

Expected: 179/179; the four hashes in §5; Ui 0/0; UI smoke 107 pass with only J01/J99 failing, as on base.

## 11. Next step

An independent review by a different provider, against the approved quartet and §9, before owner acceptance. No merge, no push.
