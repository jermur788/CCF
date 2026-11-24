# MULTI Map Integration Pass 1 — Bounded Tasks

- **Spec ID:** SPEC-multi-map-integration-pass1-V1
- **Spec status:** IMPLEMENTING
- **Task list owner:** Primary Implementer (Gemini 4 Neon)
- **Last updated:** 2026-11-24

## T-01 — Commit 1: Spec quartet and owner visual reference assets
- **Goal:** Commit self-contained `spec.md`, `plan.md`, `tasks.md`, unticked `checklists/requirements.md`, and all four owner-supplied PNG reference boards under `specs/multi-map-integration-pass1-v1/assets/`.
- **Scope:** `specs/multi-map-integration-pass1-v1/**`.
- **Verification:** All 4 PNG SHA-256 hashes match owner manifest; zero `TBD` in spec; every checklist row starts unchecked `[ ]`.

## T-02 — Commit 2: Stage 4 Owner UX reconciliation (`0e777455` onto `cf6fe7a5`)
- **Goal:** Fold Stage 4 Owner UX code (`src/Ui/PlayerMultiApi.cs`) and smoke witness (`tests/ui-smoke-multiarea.sh`) cleanly onto `cf6fe7a5` without altering Phase 2 biology or bloating the branch with old Stage 4 PNGs.
- **Scope:** `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`, `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh`.
- **Verification:** `.sln` and `SitkaP1.Ui.csproj` build `0 Warning(s) / 0 Error(s)`; full Core test suite `245/245 passed, 0 failed`.

## T-03 — Commit 3: Additive Pass 1 Core additions
- **Goal:** Add deterministic `MA-1-MIXED` demo factory (`src/Core/MultiArea`), single-lock `ReadCompositeUiSnapshot()` + `ReadCompositeUiFor()`, species-aware prune readout helper, compact additive `ecology` DTO, read-only `ReadCellDetail()`, and default `/multi` Phase 2 adapter opener.
- **Scope:** `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/**`, `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/**` (adapter surface allow-list, Pass 1 Core tests).
- **Verification:** Clean build `0/0`; all 245 existing tests + new Pass 1 Core tests pass; all 9 frozen anchors unchanged; payload budgets verified in Core serialization tests.

## T-04 — Commit 4: UI, CLI flag, pure inline SVG system, `POST api/multi/cell`, and tests
- **Goal:** Add `--multi-experiment=ma-1|ma-1-mixed` to `Program.cs`, truthful `/api/multi/manifest` & header badge, `POST api/multi/cell` endpoint behind unchanged guards, full pure inline SVG `<symbol>`/`<pattern>` `<defs>` (21 IDs, zero WebP), strictly circular crowns, `Select Tree` / `Inspect Cell` modes, single ground overlay selector, species-aware Prune + read-only ecology inspectors, zero Pass 2 write controls, and smoke-test additions.
- **Scope:** `Tools/MultiRackSitkaP1Lab/src/Ui/**`, `Tools/MultiRackSitkaP1Lab/tests/**`.
- **Verification:** Clean builds `0/0`; `node --check` on served `multi.js`; zero `data:image/webp`; `ui-smoke-multiarea.sh` green; payload byte measurements ≤ 512 KiB / ≤ 150 KiB on both experiments at Year 0 and Year +5.

## T-05 — Commit 5: Tier 2 verification, real Edge matrix, and durable handoff
- **Goal:** Execute clean-gate builds (`--no-incremental --disable-build-servers`), two identical full Core test runs, anchor hash verification, exact payload measurements (bytes + parse/paint ms), real loopback + headless Edge CDP verification at 1920×1080 and 1280×720 for both `ma-1` and `ma-1-mixed`, verify ephemeral server shutdown and protected listeners `:33767`/`:43649`/`:46593`, write durable implementation handoff, and push task branch.
- **Scope:** `Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass1/MULTI-MAP-INTEGRATION-PASS1-NEON-IMPLEMENTATION-20261124.md`; raw evidence archived externally in `/home/jer/Development/CCF-Multi-MapIntegration-Pass1-Evidence-20261124` (mode 700).
- **Verification:** Clean `git status --short`; branch pushed (`feat/multi-map-integration-pass1-20261124-v1`); zero PR/merge/force-push/tag.
