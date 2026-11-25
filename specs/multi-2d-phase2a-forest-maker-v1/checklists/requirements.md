# CCF Multi 2D — Phase 2A: Forest Maker — Requirements Checklist

- **Spec ID:** `SPEC-multi-2d-phase2a-forest-maker-V1`
- **Spec status:** `DRAFT` (awaiting owner approval)
- **Checklist owner:** Independent Reviewer / Overall Manager. Every box starts unchecked `[ ]`, and only an independent reviewer or the Manager ticks a row after inspecting evidence.
- **Implementation under test:** not yet admitted (2A-1 and 2A-2 lanes are assigned after owner approval)
- **Date:** 2026-11-25

> **Rule:** every row stays unchecked `[ ]` until an independent reviewer returns a verdict. Implementers record evidence in their step handoff and never tick boxes.

## Step 2A-1 — Core

- [ ] **P2A-01 — Recipe bounds, cap and typed refusals:**
  - Width/length 30–120 m, lanes 2–8, age 10–60, density 500–3,000/ha, seed 1–100,000,000: each accepted at min and max and refused at min−1 and max+1 with its own code.
  - `MaxLivingStems = 3,000`: 3,000 accepted, 3,001 → `ForestMakerStemCapExceeded`. 120×120 at 3,000/ha refused.
  - Corner matrix: every case generates and validates, or throws a typed `P1Refusal` only.
  - Canonical recipe bytes and `RecipeHash` stable. Non-canonical recipe refused.
  - Evidence:

- [ ] **P2A-02 — Determinism and validity:**
  - Equal recipes → identical manifest bytes and hash. A different seed → different positions, same counts.
  - Unchanged `MultiAreaExperiment.Validate()` passes. Stems strictly inside their lane, ≥ 500 mm from lane edges, no duplicate positions, 4-digit ordinal IDs, unique lane seeds.
  - `ForestMakerLaneGridAlignment` fires for a boundary on a 2.5 m or 0.25 m centre line (e.g. W=30, N=8).
  - Evidence:

- [ ] **P2A-03 — Primary preset `fm-064-sitka-edge-broadleaves-v1` exact:**
  - 80×80 m, lanes at 0/16/32/48/64/80 m, 0.128 ha each. 1,344 stems = 1,336 Sitka (age 20) + 4 Oak + 4 Beech (age 60).
  - Per-lane A 265S+2O+2B; B/C/D 269S; E 264S+2O+2B. Broadleaf positions/species per spec §5.2 rule 3. Every Sitka ≥ 2 m from every broadleaf.
  - 1,024 cells of 2.5 m, 192/224/192/224/192 per lane.
  - Evidence:

- [ ] **P2A-04 — Pure-Sitka preset `fm-064-pure-sitka-v1` exact and anchors 10–11 captured:**
  - 1,344 Sitka, per lane 269/269/269/269/268, empty `GenesisSpecies`, 1,024 cells.
  - Both preset manifest hashes recorded as new frozen anchors 10 and 11.
  - Evidence:

- [ ] **P2A-05 — Existing species constants respected, zero biology change:**
  - Broadleaf genesis crown = `Phase2Crown.TargetRadiusM` over the connected woodland.
  - `Phase2Rules.Maturity(Oak|Beech, 60) == 1` and `Stage5SeedRain.Maturity(20) == 0`, read from constants.
  - Starting DBH/height match spec §5.3. Research/transfer disclosure present in the manifest.
  - Zero diff in biology/RNG files, `MultiAreaGenerator.cs`, `MultiAreaEcology.cs`, `MultiAreaContinuation.cs`.
  - Evidence:

- [ ] **P2A-06 — Cross-lane competition and seeding on 0.64 ha:**
  - MA-B Sitka within 8 m of MA-A: woodland Hegyi CI ≠ lane-only CI.
  - Density mortality/RD per lane on 0.128 ha.
  - Primary: non-zero oak/beech seed input in at least one MA-B cell within 5 years. Pure Sitka: zero everywhere.
  - Both presets advance 15 years under `1/1/1/1/1` without refusal.
  - Evidence:

## Step 2A-2 — UI, Save Schema v3, speed

- [ ] **P2A-07 — Save Schema v3 with full v2 compatibility:**
  - Forest-maker sessions save v3 (recipe + hash + generator note in the header, checksum over header||payload).
  - Fresh-process load reproduces identical canonical bytes and an identical 5-year continuation.
  - Typed zero-mutation refusals: corrupt recipe, recipe hash mismatch, recipe/genesis mismatch, unsupported generator version, schema 4, edited payload.
  - MA-1 / MA-1-MIXED still write v2, byte-identical before and after (SHA-256 of all samples). All 50 existing Save tests pass unchanged. `Phase1SaveSlots.cs` unchanged.
  - Evidence:

- [ ] **P2A-08 — Atomic guarded session replacement:**
  - Refused, unconfirmed or stale-fence requests leave session ID, root, Work Plan and caches unchanged.
  - Success mints a new session ID. Every old fence is refused (Execute/Advance replay included). Work Plan, history and canopy/ground caches belong to the new stand.
  - `WoodlandUnsavedProgress` (409) without `confirmDiscardUnsaved: true` when progress exists.
  - Evidence:

- [ ] **P2A-09 — New Woodland modal and Active Lane + Visible Neighbours view:**
  - Button beside Saved games. No fourth layout column. Accessible dialog (focus trap, Esc, labels, return focus).
  - Preview numbers and refusals come verbatim from Core. Zero client bound/count/geometry math (source grep recorded).
  - 2 lanes at A/E, 3 lanes at B/C/D. Mini-map lane switch (mouse + keyboard). 1-lane toggle. Grassland outside the perimeter.
  - Two-area layout unchanged. 1280×720 and 1920×1080 gates pass with the dialog closed and open.
  - Evidence:

- [ ] **P2A-10 — Measured payload and speed:**
  - M0 table (snapshot ms, raw bytes, parse ms, SVG render ms, annual advance ms) for both presets at Year 0 and Year 15, with environment recorded.
  - MA-1 / MA-1-MIXED ≤ 524,288 B. 5-lane ≤ the frozen scaled budget.
  - Forced-overflow test returns HTTP 503 `MultiViewPayloadBudgetExceeded`, never an over-budget body.
  - Speed targets reported honestly against the approved Q5 values. Any static/dynamic split justified by M0.
  - Evidence:

## Regression

- [ ] **P2A-REG — Builds, suites, anchors and scope:**
  - `0/0` builds (`.sln`, `SitkaP1.Ui.csproj`, `Phase1Save.Tests.csproj`). Core and Save suites = baseline + new, all passing.
  - 9 existing frozen anchors exact. Anchors 10–11 exact from 2A-1 onward. `.p0ref` = `9a99f781…34b3`.
  - Zero diff under `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/coordination/`. 2A-1: zero diff under `src/Ui/**`, `src/Core/Phase1/**`.
  - Protected listeners `:33767`, `:43649`, `:46593` untouched before and after.
  - Evidence:

- [ ] **P2A-REVIEW — Independent review per step (no self-approval):**
  - Evidence:
