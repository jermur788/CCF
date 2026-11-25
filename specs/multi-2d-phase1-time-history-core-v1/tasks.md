# CCF Multi 2D — Phase 1 Step 1: Time Pacing, History Query & Woodland Structure Descriptors — Bounded Tasks

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-core-V1`
- **Spec status:** `IMPLEMENTING`
- **Task list owner:** Primary Implementer (Gemini 4 Neon)
- **Base commit:** `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43`
- **Last updated:** 2026-11-25

---

## T-01 — Commit 1: Spec quartet
- **Goal:** Create `spec.md`, `plan.md`, `tasks.md`, and unticked `checklists/requirements.md` under `specs/multi-2d-phase1-time-history-core-v1/`.
- **Scope:** `specs/multi-2d-phase1-time-history-core-v1/**`.
- **Verification:** Zero `TBD`; all checklist rows (`P1-TH-01`..`P1-TH-08`, `P1-TH-REG`, `P1-TH-REVIEW`) unchecked `[ ]`; explicit honesty labels, GAMEPLAY CALIBRATION framing, 2D `WorkYear` vs `ObservedYear` timing, 6-value provenance enum, and out-of-scope boundaries included.

## T-02 — Commit 2: Core Modules A, B, C + narrow continuation wiring
- **Goal:** Implement `MultiAreaTimePacing.cs`, `MultiAreaHistoryQuery.cs`, and `WoodlandStructureMetrics.cs`, plus minimal `partial class` wiring on `MultiAreaContinuation.cs`.
- **Scope:**
  - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaTimePacing.cs` (new)
  - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaHistoryQuery.cs` (new)
  - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/WoodlandStructureMetrics.cs` (new)
  - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` (narrow `partial` keyword edit only)
- **Verification:** Clean builds (`0 warnings, 0 errors`) of `MultiRackSitkaP1Lab.sln` and `SitkaP1.Ui.csproj`; zero edits under `src/Ui/`, `src/Core/Phase1/`, or biology/RNG files; zero banned file-I/O API calls (`644 B-01`).

## T-03 — Commit 3: Comprehensive Step 1 Core & Save-continuity tests
- **Goal:** Add `Phase1TimeHistoryTests.cs` (registered via one line in `tests/Core.Tests/Program.cs`) covering `P1-TH-01` through `P1-TH-08`, plus Step 1 SchemaVersion 2 save-continuity cases in `tests/Phase1.Save.Tests/Program.cs` without altering any of the 267 baseline Core cases or 48 baseline Save cases.
- **Scope:**
  - `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs` (new)
  - `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs` (1 registration line only)
  - `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Program.cs` (additive continuity tests only)
- **Verification:** Full Core suite (`267` baseline + new Step 1 tests, `0` failed), Save contract suite (`48` baseline + new Step 1 continuity tests, `0` failed), all 9 frozen anchors exact, clean builds (`0/0`).

## T-04 — Commit 4: Verification gates, implementation handoff, and branch push
- **Goal:** Run all required build and test gates under `~/.dotnet` (8.0.134) with external `TMPDIR`, confirm `.p0ref` hash, protected listeners, scope diff, and clean working tree, write the durable implementation handoff under `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md`, commit, and push `feat/multi-2d-phase1-time-history-core-20261124-v1`.
- **Scope:** `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md`.
- **Verification:** Clean `git status --short`; zero checklist rows ticked; explicit `IMPLEMENTED ON TASK BRANCH — NOT REVIEWED, NOT INTEGRATED, NOT ACCEPTED` statement; pushed to `origin` without force, merge, PR, or tags.
