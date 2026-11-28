# Implementation Handoff — Year-30 Deep-History Payload Compaction (Stage B 512 KiB View-Budget Fix)

- **Branch**: `feat/multi-2d-year30-history-compaction-v1`
- **Base commit**: `f4dee8023d3492a25e61451c7e2a49515e507ed3` (`origin/integration/2d-sim`)
- **Implementation commit**: `54783fc99b77fc67694d695f5894f1e63a5c11a0` (`fix(2d-sim): compact year 0-30 deep-history multi view projection`)
- **D-030 state**: NOT merged to integration/2d-sim or main, origin/main bdeb6a32 untouched, not user-accepted

---

## 1. Root Cause (Measured Byte Attribution)

Before choosing a fix, a disposable out-of-repo probe (`/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/probe/measure_base.py`, `inspect_annual.py`, `measure_lanes.py`) was run against the unmodified base build (`f4dee8023d3492a25e61451c7e2a49515e507ed3`) and cross-checked against the documented base log (`/home/jer/.ccf-orch/evidence/2d-m1-closeout-20261127/base-probe/step2-base.log`).

### Verified finding — the hypothesis that `GET /api/multi/view` alone broke at Year 7/8 was disproven; three concrete coupled defects caused the failure

1. **Primary defect — redundant per-tree `Before`/`After` `AnnualTreeAudit` list inside `POST /api/multi/year/advance` (`MultiCommandPayload.Annual`)**:
   - In `step2-base.log`, `GET /api/multi/view` itself NEVER exceeded 524,288 B on `MA-1` or `MA-1-MIXED` across Years 0–30 (`MA-1` peaked at 517,010 B at Year 27 in Tier 0 and dropped to 401,951–413,191 B in Tier 1 at Years 28–30; `MA-1-MIXED` peaked at 523,424 B at Year 29 in Tier 1 with Tier 2 headroom still unused).
   - What actually returned **HTTP 503 `MultiViewPayloadBudgetExceeded`** (`bytes=524551` at MA-1 Year 8; `bytes=527778` at MA-1-MIXED Year 7; `bytes=530546` in `ma1-full` Year 9; `bytes=652659` in `mixed-play-save` Year 16; up to `bytes=815440` at MA-1-MIXED Year 30) was **`POST /api/multi/year/advance`** (`MultiCommandAsync` in `src/Ui/MultiApi.Commands.cs`), following commit `3a4a0c9a` (`WriteBoundedJsonAsync` enforcing the budget on the full outer response JSON).
   - `MultiCommandPayload` serialized `result.Annual` (`MultiAreaAnnualResult`), whose `Areas[].Report` (`AnnualReport`) carries `IReadOnlyList<AnnualTreeAudit> Trees` — two full `AnnualTree` snapshots (`Before` and `After`) plus per-tree growth diagnostics for every living and dead tree in every area, duplicating the tree roster already present in `payload.view.areas[].trees` in the exact same JSON response:
     - **Year 1 (`MA-1-MIXED` `inspect_annual.py`)**: `annual` = **109,619 B**, of which `report.trees` = **107,663 B** (`77,388 B` MA-A + `30,275 B` MA-B, **98.2%** of `annual`); all stand summaries + `deaths` + metadata combined = **~1,822 B**.
     - **Year 7 (`MA-1` `measure_base.log`)**: `GET /api/multi/view` = `414,386 B` (Tier 0), `POST /api/multi/year/advance` = `521,531 B` (`annual` = `116,433 B`, minus `9,288 B` for omitted `labels`/`reviewPreview`).
     - **Year 8 (`MA-1`)**: `GET /api/multi/view` = `416,874 B` (Tier 0), `POST /api/multi/year/advance` = **`524,551 B > 524,288 B`** -> **HTTP 503** (exact match to `step2-base.log`).
     - **Year 7 (`MA-1-MIXED`)**: `GET /api/multi/view` = `418,946 B` (Tier 0), `POST /api/multi/year/advance` = **`527,778 B > 524,288 B`** -> **HTTP 503** (exact match to `step2-base.log`).
     - **Year 30 (`MA-1-MIXED`)**: `GET /api/multi/view` = `520,631 B` (`view=129,379` [`areas.trees=117,676`, `annualHistory=6,208` (2 yrs), `events=2,394` (2 evs)], `scenes=60,248`, `labels=8,622`, `stage4=272,988` [`ground=168,223`, `trees=27,896`, `deadwood=72,065`], `ecology=48,431`), while `POST /api/multi/year/advance` hit **`815,440 B`** (~295 KB of duplicated `report.trees` for 166 living + 378 dead trees).
     - **Year 30 (`fm-064-sitka-edge-broadleaves-v1`, 0.64 ha 5-lane)**: `GET /api/multi/view` = `1,883,462 B`, while `POST /api/multi/year/advance` was **`2,587,264 B`** (~704 KB of duplicated `report.trees`, only 34 KB below the 2.5 MiB hard ceiling).
   - Zero JavaScript modules under `src/Ui/multi/**` read `payload.annual`; the sole test reference (`tests/ui-smoke-multiarea.sh:355-356`) checks `[r['areaId'] for r in y['annual']['areas']]` and `r['report'] is not None`.

2. **Secondary defect — envelope-blind budget evaluation (`SerializeBoundedMultiView` vs `WriteBoundedJsonAsync`)**:
   - `SerializeBoundedMultiView` previously evaluated byte budgets against an inner `MultiViewPayload`, ignoring extra envelope fields appended by callers (`MultiCommandPayload` `Execution`/`Annual`/`Refusal`, `MultiSkipPayload` `CompletedYears` up to 5 year records = ~16 KB at Year 30, `MultiCellPayload` `Cell`, `MultiCompetitionPayload` `Competition`, `MultiHitPayload` `Candidates`, `MultiLoadPayload` `Restored`/`ExperimentBadge`/`Recipe`, `MultiWoodlandNewPayload` `ExperimentBadge`/`Recipe`, `MultiEconomySessionPayload` `Refusal`).
   - Whenever the inner `MultiViewPayload` fit inside `budget.MaxViewBytes` (staying in Tier 0 or Tier 1) while the outer wrapper pushed total bytes past `budget.MaxViewBytes`, `WriteBoundedJsonAsync` refused with HTTP 503 without giving `SerializeBoundedMultiView` a chance to step down to Tier 1/Tier 2 or trim history — and for mutating commands, after Core had already committed state.

3. **Tertiary defect — dishonest `historyTruncated` signalling on `stage4.pruneLog` trimming**:
   - Both `MultiApi.View.cs` (`TryFitTier` and Tier 3) and `MultiApi.Stand.cs` (`SerializeBoundedLaneStand`) ORed `pruneTrimmed` (`pruneCap < fullPruneCount`) into `historyTruncated` (and `MultiApi.Stand.cs` hardcoded `histTrunc: true` on overflow). Trimming `stage4.pruneLog` while retaining 100% of `view.events` and `view.annualHistory` (including at Year 0) would falsely set `historyTruncated = true`.

---

## 2. Files Changed

Strictly scoped to UI HTTP projection/compaction (`Tools/MultiRackSitkaP1Lab/src/Ui/**`), targeted regression tests, and this handoff note:

| File | Summary of change |
|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.View.cs` | Added `BoundedMultiFrame`, `ProjectAnnualForBudget` + compact annual DTOs (`MultiProjectedAnnualResultDto`/`MultiProjectedAreaReportDto`/`MultiProjectedAnnualReportDto`), optional `envelopeSerializer` on `SerializeBoundedMultiView`, `WriteBoundedBytesAsync`, truthful `HistoryTruncated` separated from `PruneLogTruncated` (omitted via `WhenWritingDefault` when false), and envelope-aware serialization on `ReadMultiCellAsync`, `ReadMultiCompetitionAsync`, `MultiHitTestAsync`. |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Commands.cs` | Projected `result.Annual` via `ProjectAnnualForBudget`, wired exact outer `MultiCommandPayload` and `MultiEconomySessionPayload` envelope serializers into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.History.cs` | Wired exact outer `MultiSkipPayload` envelope serializer (both refusal and completed-years branches) into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs` | Wired exact outer `MultiLoadPayload` envelope serializer into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Woodland.cs` | Wired exact outer `MultiWoodlandNewPayload` envelope serializer into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Stand.cs` | Separated `PruneLogTruncated` (`WhenWritingDefault`) from `HistoryTruncated` (never ORing `pTrim`, never hardcoding `true` on overflow) and added binary-search history tail retention in `SerializeBoundedLaneStand`. |
| `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` | Strengthened Year-1 annual check (`before`/`after`/`deaths` present, `report.trees` omitted); added deep-history envelope checks for Year 15→16 `POST /api/multi/year/advance`, Year 16→20 `POST /api/multi/year/skip`, Year 20 view + Year 0 on-demand history, and check `M15` proving Year-0 `stage4.pruneLog` trimming sets `pruneLogTruncated=true` while keeping `historyTruncated=false`. |
| `Docs/Handoffs/MultiRack-Simulation-20261116/year30-history-compaction/IMPLEMENTATION-HANDOFF.md` | This handoff note. |

**Fence verification**: Zero changes under `src/Core/**`, `src/Core/Phase1/**`, `src/Core/SpeciesWave2/**`, `src/Ui/wwwroot/**`, `src/Ui/multi/**`, `src/Ui/MultiViewBudget.cs`, `Docs/Project/**`, `Assets/**`, `Packages/**`, `ProjectSettings/**`, or `.p0ref/**`.

---

## 3. Compaction / Projection Design & Honesty Guarantees

1. **Lossless/non-redundant annual command projection (`ProjectAnnualForBudget`)**:
   - Retains `yearBefore`, `yearAfter`, and for every area `areaId` + `report.{ yearBefore, yearAfter, before, postGrowth, after, postGrowthDensityPressure, meanVulnerability, deaths }`.
   - Omits only `report.trees` (`IReadOnlyList<AnnualTreeAudit>`), because `view.areas[].trees` in the same HTTP response already supplies the authoritative post-advance tree roster.
   - Reduces `annual` from **109–295 KB** (2-area) / **~707 KB** (0.64 ha 5-lane) down to **1.5–3.9 KB** (~98.7% reduction), with zero Core or save mutation and zero JS changes required.
2. **Envelope-aware tier selection (`SerializeBoundedMultiView`)**:
   - Every view-carrying endpoint passes its exact outer payload serializer (`Func<BoundedMultiFrame, byte[]>`) to `SerializeBoundedMultiView`.
   - Tier 0 → Tier 1 (`ProjectViewForBudget`, `ProjectScenesForBudget`, `ProjectStage4ForBudget(compactGround: false)`) → Tier 2 (`compactGround: true`) → Tier 3 (HTTP 503 `MultiViewPayloadBudgetExceeded`) now measure the exact UTF-8 wire bytes emitted by that route, eliminating double serialization and false 503 refusals while strictly preserving the 524,288 B / 153,600 B budgets (`MultiViewBudget.cs` unchanged) and never emitting over-budget bytes as HTTP 200.
   - On `GET /api/multi/view`, the default `MultiViewPayload` shape is unchanged (preserving R2-2 Tier 0 → Tier 1 transitions and the Phase 2A raise-clamp boundary).
3. **Truthful compaction signalling**:
   - `historyTruncated` is strictly `keepEvents < fullView.Events.Count || keepAnnual < fullView.AnnualHistory.Count` in both `MultiApi.View.cs` and `MultiApi.Stand.cs`. It is NEVER `true` at Year 0 when nothing in `events`/`annualHistory` was omitted, and NEVER triggered by `stage4.pruneLog` trimming or display projection.
   - A separate flag `pruneLogTruncated` (`[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]`) is emitted as `true` if and only if `stage4.pruneLog` entries were omitted.
   - Dead-tree inspector fields (`LifeStates.Of(tr)` distinguishing `BiologicallyDead` vs `FelledStump`, death year/cause/mechanism, pruning-lift fields) and on-demand history (`GET /api/multi/history/index`, `POST /api/multi/history/year`) are completely preserved.

---

## 4. Before / After Year 0–30 Raw Byte Tables

### 4.1 `MA-1` (2-area, active budget 524,288 B raw / 153,600 B ecology)

On unmodified base (`f4dee8023d3492a25e61451c7e2a49515e507ed3`), the automated `run-step2-edge.sh` sweep aborted at **Year 8** on HTTP 503 (`POST /api/multi/year/advance` = `524,551 B`); Years 8–30 were **unreachable** in the base sweep. The "Before `adv` (probe)" column below was captured by the disposable base probe continuing past HTTP 503 (since Core had mutated prior to the 503 refusal).

| Year | Before `adv` (base) | After `adv` | Before `/view` (base) | After `/view` (G5 sweep) | After `eco` | After `ev`/`an` | After flags (`trunc`/`proj`/`s4c`) |
|---:|---:|---:|---:|---:|---:|:---:|:---:|
| 0 | — | — | 391,107 | 391,107 | 43,320 | 1 / 0 | F / F / F |
| 1 | 498,075 | 390,278 | 397,744 | 397,744 | 42,744 | 1 / 1 | F / F / F |
| 2 | 505,622 | 394,628 | 401,634 | 401,634 | 41,571 | 1 / 2 | F / F / F |
| 3 | 508,051 | 396,230 | 403,783 | 403,783 | 41,080 | 1 / 3 | F / F / F |
| 4 | 509,824 | 397,665 | 405,403 | 405,403 | 40,807 | 1 / 4 | F / F / F |
| 5 | 514,517 | 401,570 | 408,764 | 408,764 | 39,874 | 1 / 5 | F / F / F |
| 6 | 517,141 | 403,508 | 411,071 | 411,071 | 39,426 | 1 / 6 | F / F / F |
| 7 | 521,531 | 407,097 | 414,386 | 414,386 | 38,587 | 1 / 7 | F / F / F |
| 8 | **503 (524,551)** | 409,402 | *unreachable* (416,874) | 416,874 | 38,010 | 1 / 8 | F / F / F |
| 9 | **503 (530,536)** | 412,024 | *unreachable* (419,459) | 419,459 | 37,400 | 1 / 9 | F / F / F |
| 10 | **503 (532,487)** | 413,588 | *unreachable* (422,390) | 422,390 | 37,073 | 2 / 10 | F / F / F |
| 11 | **503 (537,558)** | 417,928 | *unreachable* (425,261) | 425,261 | 36,335 | 2 / 11 | F / F / F |
| 12 | **503 (540,375)** | 420,120 | *unreachable* (427,636) | 427,636 | 35,901 | 2 / 12 | F / F / F |
| 13 | **503 (550,573)** | 429,884 | *unreachable* (437,494) | 437,494 | 37,291 | 2 / 13 | F / F / F |
| 14 | **503 (562,661)** | 438,025 | *unreachable* (445,449) | 445,449 | 37,815 | 2 / 14 | F / F / F |
| 15 | **503 (567,294)** | 439,735 | *unreachable* (447,386) | 447,386 | 36,953 | 2 / 15 | F / F / F |
| 16 | **503 (574,331)** | 445,633 | *unreachable* (452,989) | 452,989 | 36,618 | 2 / 16 | F / F / F |
| 17 | **503 (580,288)** | 449,542 | *unreachable* (457,054) | 457,054 | 36,255 | 2 / 17 | F / F / F |
| 18 | **503 (590,139)** | 457,566 | *unreachable* (464,712) | 464,712 | 35,885 | 2 / 18 | F / F / F |
| 19 | **503 (602,568)** | 466,275 | *unreachable* (473,184) | 473,184 | 35,578 | 2 / 19 | F / F / F |
| 20 | **503 (607,018)** | 467,833 | *unreachable* (476,347) | 476,347 | 34,214 | 3 / 20 | F / F / F |
| 21 | **503 (618,946)** | 478,593 | *unreachable* (486,106) | 486,106 | 35,317 | 3 / 21 | F / F / F |
| 22 | **503 (628,273)** | 483,621 | *unreachable* (490,805) | 490,805 | 34,282 | 3 / 22 | F / F / F |
| 23 | **503 (634,507)** | 488,007 | *unreachable* (495,339) | 495,339 | 33,672 | 3 / 23 | F / F / F |
| 24 | **503 (639,842)** | 491,775 | *unreachable* (499,320) | 499,320 | 33,319 | 3 / 24 | F / F / F |
| 25 | **503 (646,572)** | 496,798 | *unreachable* (504,130) | 504,130 | 32,822 | 3 / 25 | F / F / F |
| 26 | **503 (656,535)** | 505,003 | *unreachable* (512,335) | 512,335 | 33,195 | 3 / 26 | F / F / F |
| 27 | **503 (664,771)** | 509,840 | *unreachable* (517,010) | 517,010 | 32,112 | 3 / 27 | F / F / F |
| 28 | **503 (673,208)** | 516,707 | *unreachable* (401,951) | 401,951 | 32,749 | 3 / 28 | F / T / F |
| 29 | **503 (558,887)** | 399,023 | *unreachable* (406,085) | 406,085 | 32,108 | 3 / 29 | F / T / F |
| 30 | **503 (568,182)** | 404,961 | *unreachable* (413,191) | 413,191 | 32,650 | 4 / 30 | F / T / F |
| **Max** | **503 @ 673,208** | **516,707** | **aborted @ Y08** | **517,010** | **43,320** | — | — |

### 4.2 `MA-1-MIXED` (2-area, active budget 524,288 B raw / 153,600 B ecology)

On unmodified base, the automated sweep aborted at **Year 7** on HTTP 503 (`POST /api/multi/year/advance` = `527,778 B`); Years 7–30 were **unreachable** in the base sweep.

| Year | Before `adv` (base) | After `adv` | Before `/view` (base) | After `/view` (G5 sweep) | After `eco` | After `ev`/`an` | After flags (`trunc`/`proj`/`s4c`) |
|---:|---:|---:|---:|---:|---:|:---:|:---:|
| 0 | — | — | 393,501 | 393,501 | 45,045 | 1 / 0 | F / F / F |
| 1 | 500,478 | 392,681 | 400,141 | 400,141 | 44,487 | 1 / 1 | F / F / F |
| 2 | 508,500 | 397,384 | 404,285 | 404,285 | 43,209 | 1 / 2 | F / F / F |
| 3 | 511,339 | 399,220 | 406,670 | 406,670 | 42,618 | 1 / 3 | F / F / F |
| 4 | 514,164 | 401,407 | 408,943 | 408,943 | 42,258 | 1 / 4 | F / F / F |
| 5 | 519,511 | 405,649 | 412,640 | 412,640 | 41,018 | 1 / 5 | F / F / F |
| 6 | 522,654 | 407,801 | 415,260 | 415,260 | 40,471 | 1 / 6 | F / F / F |
| 7 | **503 (527,778)** | 411,860 | *unreachable* (418,946) | 418,946 | 39,386 | 1 / 7 | F / F / F |
| 8 | **503 (531,745)** | 414,721 | *unreachable* (421,984) | 421,984 | 38,584 | 1 / 8 | F / F / F |
| 9 | **503 (535,942)** | 417,909 | *unreachable* (425,171) | 425,171 | 37,770 | 1 / 9 | F / F / F |
| 10 | **503 (538,652)** | 419,820 | *unreachable* (429,610) | 429,610 | 37,955 | 2 / 10 | F / F / F |
| 11 | **503 (548,599)** | 428,734 | *unreachable* (436,012) | 436,012 | 37,994 | 2 / 11 | F / F / F |
| 12 | **503 (572,455)** | 450,385 | *unreachable* (457,724) | 457,724 | 41,736 | 2 / 12 | F / F / F |
| 13 | **503 (598,703)** | 468,225 | *unreachable* (475,650) | 475,650 | 44,496 | 2 / 13 | F / F / F |
| 14 | **503 (616,724)** | 479,031 | *unreachable* (486,302) | 486,302 | 45,141 | 2 / 14 | F / F / F |
| 15 | **503 (636,136)** | 494,176 | *unreachable* (501,404) | 501,404 | 46,008 | 2 / 15 | F / F / F |
| 16 | **503 (654,863)** | 505,548 | *unreachable* (512,498) | 512,498 | 44,917 | 2 / 16 | F / F / F |
| 17 | **503 (558,108)** | 524,153 | *unreachable* (409,427) | 409,427 | 46,439 | 2 / 17 | F / T / F |
| 18 | **503 (582,293)** | 418,304 | *unreachable* (424,787) | 424,787 | 48,378 | 2 / 18 | F / T / F |
| 19 | **503 (610,304)** | 434,360 | *unreachable* (440,246) | 440,246 | 49,685 | 2 / 19 | F / T / F |
| 20 | **503 (631,742)** | 443,417 | *unreachable* (451,286) | 451,286 | 49,228 | 3 / 20 | F / T / F |
| 21 | **503 (650,228)** | 453,241 | *unreachable* (459,562) | 459,562 | 48,112 | 3 / 21 | F / T / F |
| 22 | **503 (666,439)** | 461,001 | *unreachable* (466,641) | 466,641 | 45,898 | 3 / 22 | F / T / F |
| 23 | **503 (686,431)** | 473,299 | *unreachable* (479,301) | 479,301 | 46,379 | 3 / 23 | F / T / F |
| 24 | **503 (712,139)** | 488,124 | *unreachable* (493,877) | 493,877 | 46,895 | 3 / 24 | F / T / F |
| 25 | **503 (737,342)** | 500,983 | *unreachable* (506,914) | 506,914 | 47,034 | 3 / 25 | F / T / F |
| 26 | **503 (761,509)** | 513,837 | *unreachable* (519,716) | 519,716 | 46,825 | 3 / 26 | F / T / F |
| 27 | **503 (778,545)** | 523,785 | *unreachable* (522,915) | 522,915 | 47,475 | 3 / 18 | T / T / F |
| 28 | **503 (793,246)** | 523,573 | *unreachable* (522,506) | 522,506 | 46,028 | 3 / 12 | T / T / F |
| 29 | **503 (802,204)** | 522,952 | *unreachable* (523,424) | 523,424 | 46,109 | 3 / 8 | T / T / F |
| 30 | **503 (815,440)** | 522,181 | *unreachable* (520,631) | 520,631 | 48,441 | 2 / 2 | T / T / F |
| **Max** | **503 @ 815,440** | **524,153** | **aborted @ Y07** | **523,424** | **49,685** | — | — |

### 4.3 0.64 ha 5-lane Preset (`fm-064-sitka-edge-broadleaves-v1`, Years 0–30)

Measured via `measure_lanes.py` before and after the fix (active scaled budget = `min(2,621,440, 524,288 × ceil(stems/300))`):

| Year | Before `adv` | After `adv` | `/view` | `/stand` overview | 3-lane `/stand?lanes=MA-A,MA-B,MA-C` |
|---:|---:|---:|---:|---:|---:|
| 0 | — | — | 1,586,502 | 78,607 | 490,910 |
| 5 | 2,188,551 | 1,614,960 | 1,621,223 | 88,975 | 507,945 |
| 10 | 2,261,790 | 1,638,134 | 1,642,821 | 101,552 | 520,913 |
| 15 | 2,326,842 | 1,688,647 | 1,691,399 | 112,829 | 544,193 |
| 20 | 2,410,214 | 1,749,793 | 1,753,190 | 126,003 | 572,505 |
| 25 | 2,506,650 | 1,821,139 | 1,823,426 | 132,717 | 604,287 |
| 30 | 2,587,264 | **1,879,923** | 1,883,462 | 131,875 | 632,262 |

- `POST /api/multi/year/advance` at Year 30 dropped by **707,341 B** (from 98.7% of the 2.5 MiB ceiling down to 71.7%).
- Every lane-filtered 3-lane read stayed well within its active scaled budget across Years 0–30, with honest `historyTruncated` / `pruneLogTruncated` signalling and binary-search tail retention.

---

## 5. Verification Results (Observed Counts)

| Gate / Check | Command | Exact Observed Result |
|---|---|---|
| Clean build (`-warnaserror`) | `$DOTNET build src/Ui/SitkaP1.Ui.csproj --nologo -warnaserror` | **PASS** — `0 Warning(s)`, `0 Error(s)` |
| Core.Tests | `$DOTNET run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug` | **PASS** — `314/314 passed, 0 failed` (all 9 frozen anchors + anchors 10–11 exact, zero drift) |
| UI smoke (`ui-smoke-multiarea.sh`) | `NODE=/home/jer/.local/share/vite-plus/bin/node bash tests/ui-smoke-multiarea.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-g2` | **OVERALL: PASS (`367 passed, 0 failed`)** — includes new Year 15→16 advance, Year 16→20 skip, Year 20 view + Year 0 detail, and `M15` Year-0 `pruneLogTruncated=true`/`historyTruncated=false` |
| Gate Stage B Edge (`run-step2-edge.sh`) | `bash tests/phase1-time-history-ui/run-step2-edge.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-step2` | **OVERALL: PASS (`67 passed, 0 failed`)** — `ma1-full`: `29 passed, 0 failed`; `mixed-play-save`: `20 passed, 0 failed`; `mixed-restart-load`: `6 passed, 0 failed`; `SWEEP ma-1`: PASS G5 (max raw `517,010 B <= 524,288`, max eco `43,320 B <= 153,600`); `SWEEP ma-1-mixed`: PASS G5 (max raw `523,424 B <= 524,288`, max eco `49,685 B <= 153,600`); zero stderr on every server |
| `module-graph.mjs` & `run-map-first-lenses-edge.sh` | Not run (not applicable — zero files modified under `src/Ui/multi/**`) | **NOT RUN** (no JS/HTML/CSS edits) |
| `build/2d-gate/selftest.sh`, `Phase1.Save.Tests`, `build/2d-gate.sh --browser` | Explicitly skipped per verification diet | **SKIPPED** (per handoff brief) |

---

## 6. Evidence Paths (Outside Repo)

- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/build.log`
- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/core-tests.log`
- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/ui-smoke-multiarea.log`
- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/step2-edge.log`
- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/step2-sweep-ma-1.json`
- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/step2-sweep-ma-1-mixed.json`
- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/probe/measure_base.log`
- `/home/jer/.ccf-orch/evidence/2d-year30-compaction-20261128/probe/measure_after.log`
- `/mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-g2` (ui-smoke evidence)
- `/mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-step2` (Stage B Edge screenshots & logs)

---

## 7. Residual Risks

1. **Unverified Phase 2A Edge suite (`run-phase2a-edge.sh`) and `Phase1.Save.Tests`**: Deliberately skipped under the verification diet; `GET /api/multi/view` Tier 0–2 byte layout was preserved intentionally so the Phase 2A raise-clamp proof (`scaledClampRecipe` at ~526,886 B > 524,288 B) remains intact, and `src/Core/**` / save codecs were untouched. Recommend running Phase 2A Edge during integration qualification.
2. **Extreme beyond-Year-30 horizons (e.g. Year 40–60 on dense mixed stands)**: At Year 30 on `MA-1-MIXED`, Tier 1 (`s4c=False`) reaches 520–523 KB with ~60 KB of Tier 2 (`compactGround=true`) reserve remaining; beyond ~500+ deadwood logs / 700+ total dead trees on a 2-area stand, Tier 2 could eventually saturate 512 KiB unless dead-tree/deadwood list projection or compact ground rows are added in a future milestone.
3. **External callers of `POST /api/multi/year/advance` expecting `annual.areas[].report.trees`**: No in-repo UI code or test reads `annual.areas[].report.trees`, and `view.areas[].trees` carries the authoritative post-advance roster in the same response. Any external script expecting per-tree `AnnualTreeAudit` diffs from the HTTP advance endpoint would need to read Core directly instead.
