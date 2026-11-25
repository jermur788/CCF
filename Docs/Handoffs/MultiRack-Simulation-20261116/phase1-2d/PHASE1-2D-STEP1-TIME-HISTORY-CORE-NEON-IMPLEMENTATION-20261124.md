# CCF Multi 2D — Phase 1 Step 1 (Time Pacing, History Query & Woodland Structure Core): Implementation and Verification Handoff

**Date:** 2026-11-25 (desktop-local lane, WSL2 Ubuntu).
**Role & sole writer:** Primary Implementer — Gemini 4 Neon (sole writer on the dedicated Step 1 worktree and branch; zero subagents spawned).
**Specification:** [`specs/multi-2d-phase1-time-history-core-v1/spec.md`](../../../../specs/multi-2d-phase1-time-history-core-v1/spec.md) (`SPEC-multi-2d-phase1-time-history-core-V1`).

> **EXPLICIT STATE:** This work is **IMPLEMENTED ON TASK BRANCH** only. It is **NOT REVIEWED**, **NOT INTEGRATED**, and **NOT ACCEPTED**. Every checkbox in `specs/multi-2d-phase1-time-history-core-v1/checklists/requirements.md` remains unchecked `[ ]` pending independent review.

| Label | State |
|---|---|
| IMPLEMENTED | **YES — IMPLEMENTED ON TASK BRANCH** (Spec quartet + Modules A, B, C + `P1-TH-01`..`08`/continuity tests) |
| REVIEWED | **NO — NOT REVIEWED** (independent review pending; `P1-TH-REVIEW` unchecked) |
| INTEGRATED | **NO — NOT INTEGRATED** (task branch only; zero merge to `main`, zero PR, zero tags) |
| VERIFIED (author gates) | **YES for Core Step 1 author gates** (`277/277` Core, `50/50` Save contract, `0/0` warnings/errors on `.sln`, `SitkaP1.Ui.csproj`, and `Phase1Save.Tests.csproj`, all 9 frozen anchors unchanged) |
| ACCEPTED | **NO — NOT ACCEPTED** |

---

## 1. Lane, Base, Toolchain & Commits

- **Repository root:** `/home/jer/Development/CCF-HP`
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistory-20261124`
- **Branch:** `feat/multi-2d-phase1-time-history-core-20261124-v1`
- **Base branch:** `feat/multi-2d-phase1-save-e2e-20261124-v1`
- **Base commit (exact):** `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43` (F-01 Save/Load head accepted as the unmerged baseline)
- **Verified code + spec + test HEAD:** `a85b31cc3fade2715bee304b49924b113eabe2a2` (this handoff commit sits immediately above it, adding only this Markdown file)
- **Toolchain:** `~/.dotnet` SDK `8.0.134` (`global.json` pins `8.0.133` with `rollForward: latestPatch`; system `dotnet` 10.x was NOT used)
- **External `TMPDIR`:** `/home/jer/ccf-p1th-tmp` (outside the repository)
- **Single-writer check:** `git worktree list` confirmed `CCF-Multi-2D-Phase1-TimeHistory-20261124` is the only checkout of `feat/multi-2d-phase1-time-history-core-20261124-v1`, and `ps aux` confirmed no competing writer process on this worktree/branch.

### Commit list (oldest → newest above `5438c94c…`)

| # | SHA | Message |
|---|---|---|
| 1 | `12e9dcd5f9ea886471ad08a21008fc86c299612d` | `spec: add multi-2d phase1 time-history core quartet` |
| 2 | `e3ee9ce076e012a9c52363fa01c07d0b7be9b9f8` | `feat(core): add dual time pacing, read-only history query and woodland structure descriptors` |
| 3 | `318d38febc82943caaf17bcf04995af7ad2d9902` | `test(core): add P1-TH-01..08 and save-continuity gates` |
| 4 | `a85b31cc3fade2715bee304b49924b113eabe2a2` | `test(core): align P1-TH-04 Save.Tests assertion with Samples year 1->2 planted accounts` |
| 5 | *(this handoff commit)* | `docs(phase1-2d): record Step 1 implementation and verification handoff` |

---

## 2. Files Changed & Diff Stat

### `git diff --stat 5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43..a85b31cc3fade2715bee304b49924b113eabe2a2`

```text
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs    |    2 +-
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaHistoryQuery.cs    |  982 +++++++++++++++++
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaTimePacing.cs      |  363 +++++++
 Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/WoodlandStructureMetrics.cs |  251 +++++
 Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs     | 1099 ++++++++++++++++++++
 Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs                    |    1 +
 Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Program.cs             |   36 +
 specs/multi-2d-phase1-time-history-core-v1/checklists/requirements.md    |   76 ++
 specs/multi-2d-phase1-time-history-core-v1/plan.md                       |   69 ++
 specs/multi-2d-phase1-time-history-core-v1/spec.md                       |  258 +++++
 specs/multi-2d-phase1-time-history-core-v1/tasks.md                      |   36 +
 11 files changed, 3172 insertions(+), 1 deletion(-)
```

Plus this handoff file in Commit 5:
- `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md` (new)

**Total files changed relative to `5438c94c…`: 12 files — every file strictly inside the write allowlist.**

| File | Change |
|---|---|
| `specs/multi-2d-phase1-time-history-core-v1/spec.md` | New Step 1 specification (`SPEC-multi-2d-phase1-time-history-core-V1`) |
| `specs/multi-2d-phase1-time-history-core-v1/plan.md` | New implementation plan |
| `specs/multi-2d-phase1-time-history-core-v1/tasks.md` | New bounded task breakdown |
| `specs/multi-2d-phase1-time-history-core-v1/checklists/requirements.md` | New requirements checklist (`P1-TH-01`..`08`, `P1-TH-REG`, `P1-TH-REVIEW`), all `[ ]` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` | Single-line change (`internal sealed class` → `internal sealed partial class`) to wire partial methods without touching any biology, lock, or command logic |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaTimePacing.cs` | New Module A: `TimePacingStopReason` (exact 6 values), `ReviewYearPreview`, `AdvanceYearStepSummary`, `AdvanceYearsResult`, `MultiAreaTimePacing`, and fenced `PreviewNextReviewYear` / `AdvanceYears` on `MultiAreaContinuation` |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaHistoryQuery.cs` | New Module B: 6-value `HistoryProvenance` + `HistoryProvenanceCatalog` (D-073 tiers), newest-first cursor-stable `ReadHistoryIndex`, `WorkYear = Y` vs `ObservedYear = Y + 1` `ReadHistoryYear`, whole-woodland `Phase2PlantedAccount`, `CURRENT`/`DERIVED` comparison, explicit `NOT RECORDED` disclosures, zero economy/euro/weather fields |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/WoodlandStructureMetrics.cs` | New Module C: pure `WoodlandStructureMetrics.Compute(MultiAreaRoot)` + `MultiAreaContinuation.ComputeStructure()`, connected-woodland living-adult $M_i$ and $T_i$ up to $k=3$ with ordinal `TreeId` tie-breaking, exact honesty label |
| `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs` | New 10-case Core verification suite covering `P1-TH-01`..`P1-TH-08` |
| `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs` | Exactly 1 registration line (`Phase1TimeHistoryTests.Register();`); all 267 baseline cases untouched |
| `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Program.cs` | 2 additive SchemaVersion 2 Step 1 continuity cases; all 48 baseline cases untouched |
| `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md` | This durable implementation and verification handoff |

**Forbidden paths verified 100% untouched:**
- `Tools/MultiRackSitkaP1Lab/src/Ui/**` — 0 changes
- `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/**` — 0 changes (`Phase1SaveFormat.SchemaVersion = 2` unchanged)
- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContracts.cs` — 0 changes
- `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs` — 0 changes
- `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/coordination/**`, `.p0ref/**`, `main` — 0 changes

**Working tree status (`git status --short`):** empty (clean).

---

## 3. Exact Build & Test Verification Results

Environment: WSL2 Ubuntu, `PATH="$HOME/.dotnet:$PATH"` (`dotnet --list-sdks` → `8.0.134`), `TMPDIR=/home/jer/ccf-p1th-tmp`.

| Gate | Exact Command | Result |
|---|---|---|
| Solution build | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| Core test suite | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **277/277 passed, 0 failed** (267 baseline + 10 new Step 1 cases) |
| Save contract test suite | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **50/50 passed, 0 failed** (48 baseline + 2 new Step 1 continuity cases) |
| UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |

---

## 4. Acceptance Criteria Status (`P1-TH-01`..`08`, `P1-TH-REG`, `P1-TH-REVIEW`)

| Item | Author Result | Command Used | Evidence Summary |
|---|---|---|---|
| **P1-TH-01** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `PASS P1-TH-01`: Year-0 observation skip (`+4`, `IsObservationPeriod = true`, quiet observation + `GAMEPLAY CALIBRATION` labels), non-Fell work does not set a Fell anchor, post-Fell anchor `Y -> Y + 4` (`MaxTicksPerPress = 5`), overdue `anchor + 4 <= currentYear -> currentYear + 4`, stale `PlanStateRevision`/`Source`/`SessionId` refused via `RequireFence` with zero mutation. |
| **P1-TH-02** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `PASS P1-TH-02`: pre-Fell (0→4), post-Fell (4→8), and 5-year max-press (8→13) `AdvanceYears` match direct single-year `Advance` calls byte-for-byte via canonical `CaptureSave` + `Phase1SaveCodec` encoding on BOTH `MA-1` and `MA-1-MIXED`. |
| **P1-TH-03** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `PASS P1-TH-03a` & `PASS P1-TH-03b`: `WorkPlanOpen` refused with zero mutation for Pending and Approved orders across all 6 families (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`); `InvalidTargetYear` refused (`<= currentYear`, `> currentYear + 5`, overflow boundaries) with zero mutation; `StandCannotAdvance` verified on empty stand; pre-cancelled and mid-run between-step `CancellationToken` returns `CancelledBetweenYears` with exact completed years and zero partial/phantom remainder records. |
| **P1-TH-04** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` and `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | `PASS P1-TH-04` (Core) + 2 `PASS P1-TH-04 contract` cases (Save.Tests): SchemaVersion remains `2`; pre-Fell and post-Fell Save → Decode → Restore into a fresh continuation match an uninterrupted run across `PreviewNextReviewYear`, `AdvanceYears`, `ReadHistoryIndex`, `ReadHistoryYear`, `ComputeStructure`, and final canonical bytes. |
| **P1-TH-05** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `PASS P1-TH-05`: newest-first paging over 17 recorded years (16..0) with cursor stability after appending years 17–18; Year 0 pre-advance work yields `HasAnnualOutcome = false, LiveStemsEndOfYear = null`; `WorkYear = Y` vs `ObservedYear = Y + 1` timing preserved with zero relabelling; all 6 provenance values and D-073 tiers verified; `PastOverheadCanopyCover`, `PastTreeByTreeStructure`, `PerAreaPlantedAccountSplits` marked `NOT RECORDED`; whole-woodland `Phase2PlantedAccount` exposed without per-area fabrication; exact non-causal statement; zero weather claims and zero economy/money/price/objective/euro properties or `€` strings (reflection-checked); zero mutation on read. |
| **P1-TH-06** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `PASS P1-TH-06a` & `PASS P1-TH-06b`: pure Sitka $M_i = 0.0$; analytical 4-tree ($\bar{M} = 5/6, \bar{T} = 11/24$) and 6-tree ($\bar{M}_A = 4/9, \bar{T}_A = 1/2, \bar{M}_B = 7/9, \bar{T}_B = 19/36, \bar{M} = 11/18, \bar{T} = 37/72$) mixed fixtures exact to $10^{-12}$; small counts $N=2,3$ ($k_i = 1,2$, `MaxNeighbourCountK = 3`), $N=0,1$ and zero-DBH $T_i$ return `null`; exact distance ties broken by ordinal `TreeId` regardless of roster order; stumps, standing dead, deadwood, and unpromoted `PJ` saplings excluded; cross-rack neighbours included; zero state mutation. |
| **P1-TH-07** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `PASS P1-TH-07`: Gate G3 assertions verify `GAMEPLAY CALIBRATION` note, quiet observation label, exact non-causal history wording (no weather), exact spatial-structure honesty label, and exact 6-value `TimePacingStopReason` enum with zero unmodelled 3D stop reasons. |
| **P1-TH-08** | **PASS** | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` and `git diff --name-only 5438c94c..HEAD` | `PASS P1-TH-08`: zero tracked or untracked changes under `src/Ui/`, `src/Core/Phase1/`, `Assets/`, `Packages/`, `ProjectSettings/`, or `Docs/Project/coordination/`. |
| **P1-TH-REG** | **PASS** | All four build/test commands + `sha256sum` + `ss -tlnp` | Clean `0/0` builds across `.sln`, `SitkaP1.Ui.csproj`, and `Phase1Save.Tests.csproj`; `277/277` Core (267 baseline + 10 new), `50/50` Save (48 baseline + 2 new); all 9 frozen anchors exact; `.p0ref` hash exact; protected listeners alive. |
| **P1-TH-REVIEW** | **NOT TESTED** | Pending independent reviewer | No self-review permitted; all rows in `specs/multi-2d-phase1-time-history-core-v1/checklists/requirements.md` remain `[ ]`. |

---

## 5. Nine Frozen Anchors & Runtime Invariants

All 9 frozen anchors were re-observed unchanged in the final Core test run and `sha256sum`:

| # | Anchor | Observed SHA-256 | Match |
|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | **EXACT** |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | **EXACT** |
| 3 | Multi-area `0/0/1/0` (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | **EXACT** |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | **EXACT** |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | **EXACT** |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | **EXACT** |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | **EXACT** |
| 8 | P0 Core binary pin (`.p0ref/SitkaLab.Core.dll`) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | **EXACT** |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | **EXACT** |

- **`.p0ref/SitkaLab.Core.dll` sha256 (start & end):** `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (read-only pin untouched; `build/build-p0-core-ref.sh` never run).
- **Protected listeners (verified before and after, untouched):**
  - `127.0.0.1:33767` (`LISTEN`, PID `1750123`)
  - `127.0.0.1:43649` (`LISTEN`, PID `1806361`)
  - `127.0.0.1:46593` (`LISTEN`, PID `839989`)

---

## 6. Deviations & Residual Risks

### Deviations (disclosed transparently)
1. **Follow-up commit (`a85b31cc`) instead of amending Commit 3:** Commit 3 (`318d38fe`) initially failed 1 of 50 cases in `Phase1.Save.Tests` because the new `P1-TH-04` contract assertion checked `Accounts.All(a => a.Planted.Count > 0)` whereas `Samples.Phase2Approved()` intentionally populates `Phase1PlantedAccount` on `Accounts[1]` (year `1→2`) only. Per the no-amend/no-rewrite rule, the assertion was corrected in follow-up commit `a85b31cc` rather than amending `318d38fe`, and the full suite then passed `50/50`.
2. **Test-only between-step hook overload on `AdvanceYears`:** Alongside the required `internal AdvanceYearsResult AdvanceYears(MultiAreaFence fence, int requestedTargetYear, CancellationToken cancellationToken = default)`, an internal 4-parameter overload `AdvanceYears(fence, requestedTargetYear, cancellationToken, Action<int>? afterYearCommitted)` was added so `P1-TH-03b` can deterministically cancel the `CancellationToken` strictly between Year 2 and Year 3 without thread-timing races. It invokes the callback only after a year commits and before the next loop iteration's cancellation check, and never mutates state.
3. **`MultiAreaContracts.cs` left untouched:** All pacing/preview records were placed in `MultiAreaTimePacing.cs` (preferred by the allowlist), leaving `MultiAreaContracts.cs` and `MultiAreaLabels` unchanged.

### Residual Risks & Deferred Work
1. **No UI / HTTP integration yet (by Step 1 scope):** Step 1 is strictly Core-only. HTTP endpoints, `/multi` UI controls, payload budget checks with new DTOs, and browser E2E belong to Step 2.
2. **Synchronous single-lock multi-year loop:** `AdvanceYears` executes 1–5 annual steps synchronously under `MultiAreaContinuation.sync`. For Step 2 HTTP/UI wiring, if interactive client cancellation between years is needed over HTTP, note that holding `lock (sync)` across all 5 steps serializes concurrent reads until the bounded (≤ 5 year) loop completes, while an HTTP request abort token will still be checked between annual steps.
3. **`P1-TH-08` base-commit scope test on future stacked branches:** `P1-TH-08` asserts `git diff --name-only 5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43` has zero edits under `src/Ui/`. That assertion is exact for Step 1; a future Step 2 branch that intentionally adds UI edits will need to retire or branch-scope `P1-TH-08` under Step 2's spec.
4. **Not independently reviewed / not integrated / not accepted:** Independent review (`P1-TH-REVIEW`), integration decision, and owner acceptance remain open.

---

## 7. Next Step

Independent reviewer inspection of `feat/multi-2d-phase1-time-history-core-20261124-v1` against `specs/multi-2d-phase1-time-history-core-v1/` (checking `P1-TH-01`..`P1-TH-08`, `P1-TH-REG`, and `P1-TH-REVIEW`).
