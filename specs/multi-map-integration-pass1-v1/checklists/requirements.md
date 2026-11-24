# MULTI Map Integration Pass 1 — Requirements / Acceptance Checklist

- **Spec ID:** SPEC-multi-map-integration-pass1-V1
- **Spec status:** IMPLEMENTING
- **Checklist owner:** Independent Reviewer / Overall Manager (every box starts unchecked `[ ]`; only an independent reviewer or Manager ticks rows after inspecting evidence)
- **Implementation under test:** `feat/multi-map-integration-pass1-20261124-v1` (base `cf6fe7a52aa096e824827c568f43fe05724dcce9`)
- **Date:** 2026-11-24

> **Rule:** Every row below starts unchecked `[ ]`. The Primary Implementer records observed numbers in the durable handoff (`Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass1/MULTI-MAP-INTEGRATION-PASS1-NEON-IMPLEMENTATION-20261124.md`) and leaves all checkboxes unticked for independent verification.

## Acceptance rows

- [ ] **MI-P1-01 — Default model, CLI, MA-1-MIXED demo factory, truthful badges & atomic snapshot:**
  - `/multi` defaults to `MultiAreaModelSelection.Phase2` (`1/1/1/1/1`) on standard synthetic `MA-1` (all Sitka); Core default `new MultiAreaContinuation(experiment)` stays `0/0/0/0/0`; single-area player unchanged.
  - `Program.cs` accepts `--multi-experiment=ma-1` (default) | `--multi-experiment=ma-1-mixed` and rejects anything else with non-zero exit + usage message; no arbitrary paths, no extra listeners.
  - Demo-only `MA-1-MIXED` factory lives in `src/Core/MultiArea` (no test assembly reference), deterministic, Area A 100 % Sitka, Area B ordinal `TreeId` round-robin `0 -> sitka-spruce`, `1 -> sessile-oak`, `2 -> beech` with Oak/Beech re-initialised at age 60 via `Phase2Options.GenesisSpecies`.
  - `/api/multi/manifest` and visible header badge report truthfully: `Synthetic MA-1 (all Sitka)` for `ma-1`, and `Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)` for `ma-1-mixed`.
  - Single-lock atomic `ReadCompositeUiSnapshot()` guarantees identical `sessionId`, `source`, and `planStateRevision` across `View`, `Stage4`, `Stage5`, and `Phase2` readouts (no torn frames).
  - Evidence:

- [ ] **MI-P1-02 — Additive `ecology` DTO, hard payload budgets & read-only `POST api/multi/cell`:**
  - Existing `view`, `scenes`, `labels`, `stage4` remain unbroken on `GET api/multi/view` and command responses.
  - Additive camelCase `ecology` object is `null` when `SpeciesModelVersion == 0` and never fabricates zeros; includes `speciesByTreeId` (sparse), engine-authoritative species-aware `pruneStatusByTreeId` (Oak/Beech max 2 lifts at 3.5/6.0 m, 3-yr wait; Sitka max 3 lifts, 5-yr wait), `areas[]` summaries & cell counts, `cellSchema` + numeric `cellsCompact` (bitmask `1/2/4`, no contributor IDs, no duplicate polygon vertices, positive non-zero values never rounded to `0`), `planted[]`, `shelters[]` (`expiryYear`, `isEffective`), `patches[]` (`radiusM ≈ 0.564`), read-only `pendingClear[]`/`approvedClear[]`/`pendingPlanting[]`/`approvedPlanting[]`.
  - Hard raw-byte budgets met on both `MA-1` and `MA-1-MIXED` at Year 0 and Year +5: `GET api/multi/view` ≤ 512 KiB; incremental `ecology` ≤ 150 KiB.
  - `POST api/multi/cell` (strict camelCase, 16 KiB limit, behind unchanged guards) returns `200 OK` with full cell detail (polygon, clipped area, light, cover, bramble, bracken, browse pressure, full `cohorts[]` bands, clear state) + fresh composite view; `400` on malformed/unknown/duplicate JSON; `409` with fresh view on stale fence or invalid/out-of-bounds cell; strictly read-only (zero state/revision change).
  - Evidence:

- [ ] **MI-P1-03 — Pure inline SVG visual system, visual references & strict circular crown geometry:**
  - Binding visual reference committed verbatim at `specs/multi-map-integration-pass1-v1/assets/Standard_SVG_Asset_Sheet_v2.png` (sha256 `35a44df40c88abeac58fe7d40469dbad2ddbcfbc017e404cd0872d4c34c0227f`), plus species concept boards `Spruce_Crown_Assets_TopDown_v2.png` (`1ce74faaaba24f176df59b41f3d0252e9cb095bce9bcc0577ea436e086dcfb26`), `Oak_Crown_Assets_TopDown_v2.png` (`edc4237f1748c2a981b3772d8fbf42e3d4a67d8382acc162d56508138a39e787`), and `Beech_Crown_Assets_TopDown_v2.png` (`bd1cfcd6d2e9f167951e78cf33aafebf292ef18e3e277e9658c1f24c322937e2`).
  - Zero `data:image/webp` remaining in served page or script; zero inline JS; zero external fonts/URLs; CSP unchanged.
  - All 21 required IDs present in `<defs>`: `#ccf-sitka`, `#ccf-oak`, `#ccf-beech`, `#ccf-standing-dead`, `#ccf-log`, `#ccf-crop-ring`, `#ccf-sapling-sitka`, `#ccf-sapling-oak`, `#ccf-sapling-beech`, `#ccf-shelter-active`, `#ccf-shelter-expired`, `#ccf-spot-clear`, `#ccf-natural-seedlings`, `#ccf-bramble-hatch`, `#ccf-bracken-hatch`, `#ccf-action-fell`, `#ccf-action-prune`, `#ccf-action-clear`, `#ccf-action-plant`, `#ccf-action-shelter`, `#ccf-action-spot`.
  - Five locked visual-semantics rules satisfied (including brief conflict rule: `Expired Shelter` is a map/legend state only and NEVER a Work Plan action button).
  - Strictly circular crowns (`x = u - r`, `y = v - r`, `width == height == 2 * radiusM`, no elliptical stretch, true-radius hit disc preserved, zero DBH→radius maths in JS).
  - Evidence:

- [ ] **MI-P1-04 — Modes, overlays, inspectors & Stage 4 workflows preserved:**
  - Two modes (`Select Tree` default crown-wins; `Inspect Cell` swaps right-hand inspector body without stacking above tree panel; `Escape` exits cell mode preserving draft Fell ticks).
  - Zero Pass 2 write controls (no active, disabled, or stub `Plant`, `SpotClear`, `Clear Undergrowth`, or `Shelter` buttons).
  - Single dominant ground overlay selector (`Off | Light Guide | Undergrowth | Natural Seedlings`); `Modelled light · not calibrated PAR` visible; `Natural Seedlings` cell-level presence glyph only (no fabricated stem dots, `Density` never called a stem count).
  - Fallen deadwood shows authoritative species + `direction schematic, not simulated`.
  - Existing Stage 4 `Fell`, `Prune` (species-aware readout), `Crop`, `Approve`, `Cancel`, `Execute`, `Year +1` work end to end.
  - Evidence:

- [ ] **MI-P1-05 — Real Edge browser matrix (1920×1080 & 1280×720, `MA-1` & `MA-1-MIXED`):**
  - Real loopback server + real headless Microsoft Edge over CDP at both `1920×1080` and `1280×720` on both `ma-1` and `ma-1-mixed`:
    - Zero horizontal scroll (`scrollWidth <= innerWidth`).
    - Stage 4 R3 gate: all 5 competitor rows + `Add` reachable above the fold in the loaded blocked-competitor state.
    - `Inspect Cell` swaps inspector body cleanly; `POST api/multi/cell` returns `200` on valid cell and `409` on stale/invalid cell.
    - `Escape` preserves draft Fell ticks; keyboard focus works.
    - Zero console errors, zero CSP violations, zero HTTP 5xx.
    - Crowns strictly circular (`width == height == 2 * radiusM`); zero `data:image/webp`.
    - `Modelled light · not calibrated PAR` and `direction schematic, not simulated` visible.
  - Evidence:

- [ ] **MI-P1-06 — Clean build, tests, runtime hygiene & durable handoff:**
  - Clean builds (`--no-incremental --disable-build-servers`, external `TMPDIR`): `0 Warning(s) / 0 Error(s)` for both `.sln` and `SitkaP1.Ui.csproj`.
  - Full Core suite: `245/245` existing + all new Pass 1 tests passing (`0` failures), identical PASS set across two consecutive runs.
  - `ui-smoke-multiarea.sh` green with Pass 1 additions.
  - Ephemeral server stopped; protected listeners `127.0.0.1:33767`, `127.0.0.1:43649`, `127.0.0.1:46593` confirmed alive.
  - Durable implementation handoff committed; `git status --short` clean; task branch pushed only (no PR, merge, force push, or tag).
  - Evidence:

- [ ] **MI-REG — Frozen anchors and scope integrity:**
  - All 9 frozen anchors re-observed unchanged: Single-area Model 0 (`7e3a2a86…1ece`), Multi-area `0/0/0/0` (`a692a6d5…233d`), Multi-area `0/0/1/0` (`814e8165…6287`), Stage 4 `1/1/1/0` (`8a926776…87fb`), Stage 5 `1/1/1/1` (`c4b670ed…dc35`), Stage 5 5.0 m purity (`c67edc6b…53d8`), Phase 2 Q6 (`776a68df…e22d` runtime equality), P0 Core binary pin (`9a99f781…34b3`), `MA-1-MIXED` 30-y fingerprint (`93e9abda…251f`).
  - Zero edits to forbidden scopes (`Assets/**`, `Packages/**`, `ProjectSettings/**`, saves, economy, `Tools/MultiRackSitkaLab/**`, `Stage5GridStudy.cs:544`, `main`).
  - Evidence:

- [ ] **MI-P1-REVIEW — Independent review and verdict:**
  - Independent reviewer verification complete; no self-review accepted.
  - Evidence:

## Verdict

- **Spec conformance:** NOT VERIFIED (pending independent review)
- **Implementation correctness:** NOT VERIFIED (pending independent review)
- **State:** `IMPLEMENTING` (at implementation completion: `IMPLEMENTED ON TASK BRANCH — READY FOR INDEPENDENT REVIEW — NOT INTEGRATED, NOT ACCEPTED`)
