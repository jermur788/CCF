# Implementation Handoff — Year-30 Deep-History Payload Compaction (Stage B and Owner Ruling Option 1)

- **Branch**: `feat/multi-2d-year30-history-compaction-v1`
- **Base commit**: `f4dee8023d3492a25e61451c7e2a49515e507ed3` (`origin/integration/2d-sim` when the feature branch was cut)
- **Implementation commit**: `54783fc99b77fc67694d695f5894f1e63a5c11a0` (`fix(2d-sim): compact year 0-30 deep-history multi view projection`)
- **Repair base reviewed as NEEDS CHANGES**: `9946276a79de55a1cb9a5aead4e2595e97e906fe`
- **Final repair source**: `3b79014c69f90a7a797d8e8af718926813523ced`. The following docs commit carries this handoff on the feature branch.
- **D-030 state**: IMPLEMENTED ON TASK BRANCH. Independent delta review pending. NOT merged to `integration/2d-sim` or `main`. Protected `origin/main` `bdeb6a324acaf4129c10fb0846faac8b64974360` untouched. Not user-accepted.

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
     - **Year 30 (`fm-064-sitka-edge-broadleaves-v1`, 0.64 ha 5-lane)**: `GET /api/multi/view` = `1,883,462 B`, while `POST /api/multi/year/advance` was **`2,587,264 B`** (~704 KB of duplicated `report.trees`, only 34 KB below the original 2.5 MiB hard ceiling).
   - Zero JavaScript modules under `src/Ui/multi/**` read `payload.annual`; the sole test reference (`tests/ui-smoke-multiarea.sh:355-356`) checks `[r['areaId'] for r in y['annual']['areas']]` and `r['report'] is not None`.

2. **Secondary defect — envelope-blind budget evaluation (`SerializeBoundedMultiView` vs `WriteBoundedJsonAsync`)**:
   - `SerializeBoundedMultiView` previously evaluated byte budgets against an inner `MultiViewPayload`, ignoring extra envelope fields appended by callers (`MultiCommandPayload` `Execution`/`Annual`/`Refusal`, `MultiSkipPayload` `CompletedYears` up to 5 year records = ~16 KB at Year 30, `MultiCellPayload` `Cell`, `MultiCompetitionPayload` `Competition`, `MultiHitPayload` `Candidates`, `MultiLoadPayload` `Restored`/`ExperimentBadge`/`Recipe`, `MultiWoodlandNewPayload` `ExperimentBadge`/`Recipe`, `MultiEconomySessionPayload` `Refusal`).
   - Whenever the inner `MultiViewPayload` fit inside `budget.MaxViewBytes` (staying in Tier 0 or Tier 1) while the outer wrapper pushed total bytes past `budget.MaxViewBytes`, `WriteBoundedJsonAsync` refused with HTTP 503 without giving `SerializeBoundedMultiView` a chance to step down to Tier 1/Tier 2 or trim history — and for mutating commands, after Core had already committed state.

3. **Tertiary defect — dishonest `historyTruncated` signalling on `stage4.pruneLog` trimming**:
   - Both `MultiApi.View.cs` (`TryFitTier` and Tier 3) and `MultiApi.Stand.cs` (`SerializeBoundedLaneStand`) ORed `pruneTrimmed` (`pruneCap < fullPruneCount`) into `historyTruncated` (and `MultiApi.Stand.cs` hardcoded `histTrunc: true` on overflow). Trimming `stage4.pruneLog` while retaining 100% of `view.events` and `view.annualHistory` (including at Year 0) would falsely set `historyTruncated = true`.

---

### Owner ruling for the Year-30 follow-up

**LOCKED OWNER RULING OPTION 1 (2026-11-28).** Keep the envelope compaction and separate its soft target from the defensive HTTP 503 ceiling. The soft target is 524,288 B for up to two areas. For three or more areas it is `min(2,621,440, 524,288 × ceil(genesisStems / 300))`. With no test override, the defensive ceiling is `max(softTarget, 4,194,304)` B. A positive `CCF_TEST_MAX_VIEW_BYTES` override applies the legacy strict limit to both targets, `min(scaledSoftTarget, override)`, and can only lower the ecology target. This includes the high-override clamp in the unchanged Phase 2A Edge gate.

The independent review reproduced a Year-30 view failure on MA-1-MIXED after steady thinning with Economy Off. The smallest view projection was 525,092 B with the old overflow flag and exceeded the former 524,288 B refusal limit. See section 4.4 for the same workload under Option 1.

---

## 2. Files Changed

Strictly scoped to UI HTTP projection/compaction (`Tools/MultiRackSitkaP1Lab/src/Ui/**`), targeted regression tests, and this handoff note:

| File | Summary of change |
|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiViewBudget.cs` | Separates the scaled soft target from the 4 MiB defensive ceiling and retains strict test override clamping. |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.View.cs` | Added `BoundedMultiFrame`, `ProjectAnnualForBudget` + compact annual DTOs (`MultiProjectedAnnualResultDto`/`MultiProjectedAreaReportDto`/`MultiProjectedAnnualReportDto`), optional `envelopeSerializer` on `SerializeBoundedMultiView`, `WriteBoundedBytesAsync`, truthful `HistoryTruncated` separated from `PruneLogTruncated` (omitted via `WhenWritingDefault` when false), and envelope-aware serialization on `ReadMultiCellAsync`, `ReadMultiCompetitionAsync`, `MultiHitTestAsync`. Option 1 accepts Tier 3 within the hard limit and exposes both limits on bounded responses. |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Commands.cs` | Projected `result.Annual` via `ProjectAnnualForBudget`, wired exact outer `MultiCommandPayload` and `MultiEconomySessionPayload` envelope serializers into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.History.cs` | Wired exact outer `MultiSkipPayload` envelope serializer (both refusal and completed-years branches) into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs` | Wired exact outer `MultiLoadPayload` envelope serializer into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Woodland.cs` | Wired exact outer `MultiWoodlandNewPayload` envelope serializer into `SerializeBoundedMultiView`, and added `PruneLogTruncated` (`WhenWritingDefault`). |
| `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Stand.cs` | Separated `PruneLogTruncated` (`WhenWritingDefault`) from `HistoryTruncated` (never ORing `pTrim`, never hardcoding `true` on overflow) and added binary-search history tail retention in `SerializeBoundedLaneStand`. Option 1 applies the hard limit and shared response headers to overview, lane results and lane refusals. |
| `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` | Strengthened Year-1 annual check (`before`/`after`/`deaths` present, `report.trees` omitted); added deep-history envelope checks for Year 15→16 `POST /api/multi/year/advance`, Year 16→20 `POST /api/multi/year/skip`, Year 20 view + Year 0 on-demand history, and check `M15` proving Year-0 `stage4.pruneLog` trimming sets `pruneLogTruncated=true` while keeping `historyTruncated=false`. Added `M16` for the thinning replay and `M17` for strict hard overflow. |
| `Docs/Handoffs/MultiRack-Simulation-20261116/year30-history-compaction/IMPLEMENTATION-HANDOFF.md` | This handoff note. |

**Fence verification**: Zero changes under `src/Core/**`, `src/Core/Phase1/**`, `src/Core/SpeciesWave2/**`, `src/Ui/wwwroot/**`, `src/Ui/multi/**`, `Docs/Project/**`, `Assets/**`, `Packages/**`, `ProjectSettings/**`, or `.p0ref/**`.

---

## 3. Compaction / Projection Design & Honesty Guarantees

1. **Lossless/non-redundant annual command projection (`ProjectAnnualForBudget`)**:
   - Retains `yearBefore`, `yearAfter`, and for every area `areaId` + `report.{ yearBefore, yearAfter, before, postGrowth, after, postGrowthDensityPressure, meanVulnerability, deaths }`.
   - Omits only `report.trees` (`IReadOnlyList<AnnualTreeAudit>`), because `view.areas[].trees` in the same HTTP response already supplies the authoritative post-advance tree roster.
   - Reduces `annual` from **109–295 KB** (2-area) / **~707 KB** (0.64 ha 5-lane) down to **1.5–3.9 KB** (~98.7% reduction), with zero Core or save mutation and zero JS changes required.
2. **Envelope-aware tier selection (`SerializeBoundedMultiView`)**:
   - Composite view responses serialize their exact outer payload during tier selection. The overview and auxiliary writers apply the ceiling to their serialized response bytes.
   - Tier 0 uses the full projection. Tier 1 compacts the view, scenes and Stage 4 data while retaining detailed ground cells. Tier 2 compacts ground cells. All three measure the exact outer UTF-8 envelope against the soft target and keep the existing ecology target for tier selection.
   - Tier 3 retains the most compact projection and returns the route's normal status while the envelope fits within the defensive ceiling. It keeps `payloadBudgetExceeded=false` on an accepted view. Above the hard ceiling the shared writer reports HTTP 503 `MultiViewPayloadBudgetExceeded` with the active hard limit in `budget` and a stderr diagnostic.
   - Bounded responses carry `X-Multi-View-Soft-Limit` and `X-Multi-View-Hard-Limit` on HTTP 200, 409 and 503. The existing response shape and projection fields are preserved.
3. **Truthful compaction signalling**:
   - `historyTruncated` is strictly `keepEvents < fullView.Events.Count || keepAnnual < fullView.AnnualHistory.Count` in both `MultiApi.View.cs` and `MultiApi.Stand.cs`. It is NEVER `true` at Year 0 when nothing in `events`/`annualHistory` was omitted, and NEVER triggered by `stage4.pruneLog` trimming or display projection.
   - A separate flag `pruneLogTruncated` (`[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]`) is emitted as `true` if and only if `stage4.pruneLog` entries were omitted.
   - Dead-tree inspector fields (`LifeStates.Of(tr)` distinguishing `BiologicallyDead` vs `FelledStump`, death year/cause/mechanism, pruning-lift fields) and on-demand history (`GET /api/multi/history/index`, `POST /api/multi/history/year`) are completely preserved.

---

## 4. Before / After Year 0–30 Raw Byte Tables

### 4.1 `MA-1` (2-area, soft targets of 524,288 B raw and 153,600 B ecology)

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

### 4.2 `MA-1-MIXED` (2-area, soft targets of 524,288 B raw and 153,600 B ecology)

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

Measured via `measure_lanes.py` before and after the fix (scaled soft target = `min(2,621,440, 524,288 × ceil(stems/300))`):

| Year | Before `adv` | After `adv` | `/view` | `/stand` overview | 3-lane `/stand?lanes=MA-A,MA-B,MA-C` |
|---:|---:|---:|---:|---:|---:|
| 0 | — | — | 1,586,502 | 78,607 | 490,910 |
| 5 | 2,188,551 | 1,614,960 | 1,621,223 | 88,975 | 507,945 |
| 10 | 2,261,790 | 1,638,134 | 1,642,821 | 101,552 | 520,913 |
| 15 | 2,326,842 | 1,688,647 | 1,691,399 | 112,829 | 544,193 |
| 20 | 2,410,214 | 1,749,793 | 1,753,190 | 126,003 | 572,505 |
| 25 | 2,506,650 | 1,821,139 | 1,823,426 | 132,717 | 604,287 |
| 30 | 2,587,264 | **1,879,923** | 1,883,462 | 131,875 | 632,262 |

- `POST /api/multi/year/advance` at Year 30 dropped by **707,341 B** (from 98.7% of the 2.5 MiB soft target down to 71.7%).
- Every lane-filtered 3-lane read stayed well within its scaled soft target across Years 0–30, with honest `historyTruncated` / `pruneLogTruncated` signalling and binary-search tail retention.

---

### 4.4 MA-1-MIXED steady thinning at Year 30

M16 uses the independent review's fixed targets and 501-character fell reason. Each year it queues two fells per area, attempts three prunes per area, approves and executes the plan, advances and reads the view again. Economy is Off. Core refused 57 of the 180 prune requests with `PruneIntervalTooShort`.

| Route | Before Option 1 at `9946276a` | After Option 1 |
|---|---|---|
| `POST /api/multi/year/advance` | HTTP 200, 520,443 B | HTTP 200, 520,443 B |
| `GET /api/multi/view` | HTTP 503, 72 B error body reporting a 525,092 B projection | HTTP 200, 525,093 B compact view |

The accepted view is 805 B above the soft target. Its overflow flag is `false`, which is one byte longer than the rejected projection's `true`. Both Year-30 responses returned `X-Multi-View-Soft-Limit: 524288` and `X-Multi-View-Hard-Limit: 4194304`.

M16 requires HTTP 200 for all 30 advances and view reads, an actively compacted Year-30 view within 600,000 B, truthful Year-0 history signalling and both headers on accepted responses. It also checks the headers on a successful lane read and a stale-fence lane HTTP 409. M17 uses a fresh MA-1-MIXED process with `CCF_TEST_MAX_VIEW_BYTES=3000` to prove that a 284,466 B projection still returns HTTP 503 `MultiViewPayloadBudgetExceeded` with `budget=3000`, both limit headers equal to 3,000 B and the expected stderr diagnostic.

---

## 5. Verification Results (Observed Counts)

### Original implementation at `9946276a`

| Gate / Check | Command | Exact Observed Result |
|---|---|---|
| Clean build (`-warnaserror`) | `$DOTNET build src/Ui/SitkaP1.Ui.csproj --nologo -warnaserror` | **PASS** — `0 Warning(s)`, `0 Error(s)` |
| Core.Tests | `$DOTNET run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug` | **PASS** — `314/314 passed, 0 failed` (all 9 frozen anchors + anchors 10–11 exact, zero drift) |
| UI smoke (`ui-smoke-multiarea.sh`) | `NODE=/home/jer/.local/share/vite-plus/bin/node bash tests/ui-smoke-multiarea.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-g2` | **OVERALL: PASS (`367 passed, 0 failed`)** — includes new Year 15→16 advance, Year 16→20 skip, Year 20 view + Year 0 detail, and `M15` Year-0 `pruneLogTruncated=true`/`historyTruncated=false` |
| Gate Stage B Edge (`run-step2-edge.sh`) | `bash tests/phase1-time-history-ui/run-step2-edge.sh /mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-step2` | **OVERALL: PASS (`67 passed, 0 failed`)** — `ma1-full`: `29 passed, 0 failed`; `mixed-play-save`: `20 passed, 0 failed`; `mixed-restart-load`: `6 passed, 0 failed`; `SWEEP ma-1`: PASS G5 (max raw `517,010 B <= 524,288`, max eco `43,320 B <= 153,600`); `SWEEP ma-1-mixed`: PASS G5 (max raw `523,424 B <= 524,288`, max eco `49,685 B <= 153,600`); zero stderr on every server |
| `module-graph.mjs` & `run-map-first-lenses-edge.sh` | Not run (not applicable — zero files modified under `src/Ui/multi/**`) | **NOT RUN** (no JS/HTML/CSS edits) |
| `build/2d-gate/selftest.sh`, `Phase1.Save.Tests`, `build/2d-gate.sh --browser` | Explicitly skipped per verification diet | **SKIPPED** (per handoff brief) |

---

### Option 1 repair at `3b79014c`

| Gate | Command | Observed result |
|---|---|---|
| Solution build | `~/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/MultiRackSitkaP1Lab.sln --nologo -warnaserror --no-incremental` | PASS, 0 warnings, 0 errors |
| UI build | `~/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj --nologo -warnaserror --no-incremental` | PASS, 0 warnings, 0 errors |
| Core.Tests | `~/.dotnet/dotnet run --project Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug --no-build` | 314/314 passed, 0 failed. All nine frozen anchors had zero drift. All 26 output lines containing 64-character hashes matched the independent review log. |
| UI smoke | `bash tests/ui-smoke-multiarea.sh <external-evidence-dir>` | OVERALL PASS, 377 passed, 0 failed. M15, M16 and M17 passed. |
| Stage B Edge | `bash tests/phase1-time-history-ui/run-step2-edge.sh <external-evidence-dir>` | OVERALL PASS, 67 passed, 0 failed on the first run. Year 0–30 maxima for MA-1 were 517,010 B view (Year 27) and 43,320 B ecology. MA-1-MIXED reached 523,424 B view (Year 29) and 49,685 B ecology. All five servers had empty stderr. |
| Phase 2A Edge | `DOTNET=~/.dotnet/dotnet bash tests/phase2a-ui/run-phase2a-edge.sh <external-evidence-dir>` | Override checks passed at 400,000 and 9,999,999 B. The full gate reported OVERALL FAIL, 65 passed, 1 counted failure, plus a fatal timeout. The unmodified repair base reproduced both failures and the same counts. |
| P0 reference | `sha256sum Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| Skipped | Requested verification scope | `build/2d-gate/selftest.sh`, Phase1Save.Tests, `run-map-first-lenses-edge.sh`, `build/2d-gate/module-graph.mjs`, full `build/2d-gate.sh --browser` and Unity. |

Toolchain was .NET SDK 8.0.134 and Node v26.13.1. The browser gates used Windows Node v26.13.1 and Microsoft Edge 158.0.4370.33.

Phase 2A's 400,000 B override refused the over-limit view and command routes, including lane reads and stale submissions carrying oversized snapshots. The 9,999,999 B override clamped both the 3-area and 2-area witness limits to 524,288 B and returned the required HTTP 503s. The full gate expected an Economy-enabled Forest Maker save to return HTTP 409 `SaveEconomyRequiresV3` even though the repair base already supports Economy in Forest Maker Save v3. It also timed out waiting for the 2,880-stem interior Lane C render at 1280×720 after the three-lane HTTP read passed. The same gate reproduced the assertion, timeout and pass counts on a disposable archive of `9946276a`. All run servers stopped on SIGINT.

---

## 6. Evidence Paths (Outside Repo)

Option 1 evidence:

- `/home/jer/.ccf-orch/evidence/2d-year30-option1-20261128/` contains both build logs, Core test and hash output, and UI smoke evidence.
- `/mnt/c/Users/hp/AppData/Local/Temp/ccf-y30-option1-20261128/` contains the Step 2 and Phase 2A Edge logs and screenshots, including `phase2a-base-console.log` for the unmodified base.
- Before-repair thinning evidence is in `/home/jer/.ccf-orch/evidence/2d-year30-review-opus-20261128/logs/probe2-mixed-long-2f3p-rerun.log`.

Original implementation evidence:

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

1. **Verification scope.** The full verification skips are listed above. The 503 guard was exercised with lowered hard limits. Payloads exceeding the production 4 MiB ceiling and horizons beyond Year 30 were outside the measured workloads.
2. **External callers of `POST /api/multi/year/advance`.** The post-advance roster is available in `view.areas[].trees`. Callers that need per-tree `AnnualTreeAudit` diffs can obtain them from Core.
3. **Downstream status.** Independent delta review, integration and owner acceptance are pending. Project dashboard synchronization remains unverified by this worktree.
