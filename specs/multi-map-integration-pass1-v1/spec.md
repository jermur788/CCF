# MULTI Map Integration Pass 1 — Read-Only Display & Pure Inline SVG Visual System Specification

- **Spec ID:** SPEC-multi-map-integration-pass1-V1
- **Status:** IMPLEMENTING
- **Owner:** Project owner (product decisions); Primary Implementer custody under D-091
- **Created:** 2026-11-24
- **Last updated:** 2026-11-24
- **Decision log references:** D-020 (presentation authority), D-030 (state honesty), D-061 (mixed density weights), D-091 (single-writer lane custody), D-095 (Stage 4 adult/mortality/prune)
- **Milestone:** MultiRack Sitka 2D standalone simulator (`Tools/MultiRackSitkaP1Lab`) — Pass 1 Map Integration
- **Related evidence:** Base engine `cf6fe7a52aa096e824827c568f43fe05724dcce9` (Phase 2 closed); Stage 4 Owner UX reference `0e7774557dbb2947b29993cf032c084aa3b7227f`
- **Supersedes / superseded by:** Builds on `SPEC-multi-stage4-ui-catchup-V1`, `SPEC-multi-stage5-sitka-regeneration-V1`, `SPEC-multi-phase2-oak-beech-V1` (read-only display integration; does not modify engine biology)

## 1. Product intent

The closed Phase 2 engine (`1/1/1/1/1`) models three species (Sitka spruce, Sessile Oak, European Beech), species-aware pruning, connected-woodland seed rain, natural regeneration cohorts, bramble/bracken undergrowth, browsing, planted juveniles, tree shelters, and ~1 m² circular spot-clear patches. However, the standalone `/multi` browser page still runs on `1/1/1/0/0`, embeds legacy WebP Sitka sprites with elliptical stretch, and exposes none of the Stage 5 / Phase 2 ecological state.

Pass 1 brings `/multi` cleanly onto the closed Phase 2 engine (`1/1/1/1/1`) as a **read-only ecological display integration** while preserving 100 % of the Stage 4 Owner UX polish (`0e777455`), replacing all WebP sprites with a high-detail pure inline SVG `<symbol>`/`<pattern>` visual system, exposing an additive compact `ecology` DTO within hard payload budgets, adding lazy read-only cell inspection via `POST api/multi/cell`, and supporting `--multi-experiment=ma-1|ma-1-mixed`.

## 2. Goals

- Preserve all Stage 4 Owner UX polish from `0e777455` (crown-wins selection, 5 competitor rows + `Add` above the fold at 1280×720 including blocked rows, `Escape` precedence preserving draft Fell ticks, scale bar, Fit framing, legend toggle, narrow dock, compact notice line + Details).
- Default `/multi` to `MultiAreaModelSelection.Phase2` (`1/1/1/1/1`) on synthetic `MA-1` (all Sitka), while keeping Core default `new MultiAreaContinuation(experiment)` at `0/0/0/0/0` and single-area player untouched.
- Support `--multi-experiment=ma-1` (default) and `--multi-experiment=ma-1-mixed` in `sitkap1-ui`, rejecting any other experiment flag with non-zero exit + usage message.
- Provide one atomic read-only composite snapshot in Core taking the lock once so `View`, `Stage4`, `Stage5`, and `Phase2` readouts share one exact `sessionId`, `source`, and `planStateRevision`.
- Expose an additive compact camelCase `ecology` DTO meeting strict raw-byte payload budgets (total `GET api/multi/view` ≤ 512 KiB; incremental `ecology` ≤ 150 KiB).
- Add strictly read-only `POST api/multi/cell` returning full cell detail plus a fresh view/readout, behind all existing `PlayerApi` guards.
- Replace all `data:image/webp;base64,...` sprites with pure inline SVG `<symbol>`/`<pattern>` definitions matching the owner-supplied visual reference boards and locked visual semantics.
- Render strictly circular crowns (`width == height == 2 * radiusM`, no elliptical stretch, zero DBH→radius maths in JS).
- Provide two explicit interaction modes (`Select Tree`, `Inspect Cell`), one dominant ground overlay selector (`Off | Light Guide | Undergrowth | Natural Seedlings`), species-aware prune readouts, and honest caveats.

## 3. Non-goals (strict exclusions)

- Zero Pass 2 write controls or write endpoints (`Plant`, `SpotClear`, `Clear Undergrowth`, `Shelter` write buttons or stub/disabled controls MUST NOT be rendered).
- Zero biology, growth, mortality, crown, regeneration, mast, browse, or RNG changes; zero anchor hash drift.
- Zero changes to `Assets/**`, `Packages/**`, `ProjectSettings/**`, Unity saves, economy/cash/prices, `Compact30`, `Legacy40`, `Enlarged80`, frozen `Reference Future v1`, `Tools/MultiRackSitkaLab/**`, or carried-forward Stage 5 test cleanups (`S5-07B` / `Stage5GridStudy.cs:544`).
- Zero external packages, npm dependencies, external fonts/URLs, inline scripts, inline styles, or WebP sprites.

## 4. Owner visual reference assets (committed in this spec)

All four owner-supplied 1254×1254 PNG reference boards are committed verbatim (binary, unscaled, un-reencoded) under `specs/multi-map-integration-pass1-v1/assets/`:

| Asset path | Role | SHA-256 |
|---|---|---|
| `specs/multi-map-integration-pass1-v1/assets/Standard_SVG_Asset_Sheet_v2.png` | Binding visual reference for `MI-P1-03` (symbols, markers, states, overlays) | `35a44df40c88abeac58fe7d40469dbad2ddbcfbc017e404cd0872d4c34c0227f` |
| `specs/multi-map-integration-pass1-v1/assets/Spruce_Crown_Assets_TopDown_v2.png` | Crown silhouette & palette reference for `#ccf-sitka` (radial conifer, branch tiers, blue-green) | `1ce74faaaba24f176df59b41f3d0252e9cb095bce9bcc0577ea436e086dcfb26` |
| `specs/multi-map-integration-pass1-v1/assets/Oak_Crown_Assets_TopDown_v2.png` | Crown silhouette & palette reference for `#ccf-oak` (lobed broadleaf, warm green, visible branch structure) | `edc4237f1748c2a981b3772d8fbf42e3d4a67d8382acc162d56508138a39e787` |
| `specs/multi-map-integration-pass1-v1/assets/Beech_Crown_Assets_TopDown_v2.png` | Crown silhouette & palette reference for `#ccf-beech` (rounded/oval broadleaf, dense fine leaf mass, copper-green) | `bd1cfcd6d2e9f167951e78cf33aafebf292ef18e3e277e9658c1f24c322937e2` |

**Binding conflict rule:** Where `Standard_SVG_Asset_Sheet_v2.png` disagrees with the locked textual contract below, **this specification wins**. Specifically, `Standard_SVG_Asset_Sheet_v2.png` illustrates `Expired Shelter` in the Work Plan Action Icons row; per §5.6 rule 4 below, `Expired Shelter` is a **map/legend state only** and MUST NEVER appear as a Work Plan action button. Clean English copy is used throughout (typos on the concept sheet such as "for contrass" are ignored).

## 5. Functional requirements

### 5.1 Default model and CLI (`MI-P1-01`)
- **FR-1.1:** `/multi` MUST default to `MultiAreaModelSelection.Phase2` (`1/1/1/1/1`) on standard synthetic `MA-1` (all Sitka).
- **FR-1.2:** Core default `new MultiAreaContinuation(experiment)` MUST stay `0/0/0/0/0` (`MultiAreaModelSelection.Legacy`); the single-area player (`/`) MUST stay unchanged.
- **FR-1.3:** `Tools/MultiRackSitkaP1Lab/src/Ui/Program.cs` MUST accept `<derivedFixtureDir>` with optional `--multi-experiment=ma-1` (default) or `--multi-experiment=ma-1-mixed`. Any unknown flag, duplicate experiment flag, extra positional argument, or invalid experiment value MUST exit non-zero (exit 64) with a clear usage message. No arbitrary experiment paths and no additional listeners are permitted.

### 5.2 `MA-1-MIXED` demo factory and truthful badges (`MI-P1-01`)
- **FR-2.1:** Implement a deterministic, demo-only factory in `src/Core/MultiArea` (never referencing test assemblies):
  - Area A (`MA-A`): pure Sitka (`100% sitka-spruce`), identical starting stems to `MA-1`.
  - Area B (`MA-B`): take all adult `TreeId`s, sort with `StringComparer.Ordinal`, assign round-robin by index `i`: `i % 3 == 0` -> `"sitka-spruce"` (keeps `MA-1` age), `i % 3 == 1` -> `"sessile-oak"` (age set to `60`), `i % 3 == 2` -> `"beech"` (age set to `60`), passing reassigned Oak/Beech through `Phase2Options.GenesisSpecies` so Phase 2 C2 equilibrium crowns are initialised by the engine.
  - Two calls MUST produce byte-identical state and manifests.
  - Existing `MA-1` generator (`MultiAreaGenerator.Generate()`) and test-only `Phase2Support.MixedFixture()` MUST remain unchanged so all frozen hashes hold.
- **FR-2.2:** `GET api/multi/manifest` and the visible `/multi` header badge MUST state truthfully:
  - `ma-1`: `Synthetic MA-1 (all Sitka)`
  - `ma-1-mixed`: `Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)`
  - For `ma-1`, the canonical manifest body and its `contentHash` (`c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95`) MUST remain verifiable and unchanged.

### 5.3 Atomic composite snapshot (`MI-P1-01`)
- **FR-3.1:** Core (`src/Core/MultiArea`) MUST expose one narrow read-only snapshot method (`ReadCompositeUiSnapshot()`) that acquires the engine lock **once**, returning `View`, `Stage4` (and its UI readout), `Stage5` (when `IsStage5`), `Phase2` (when `IsPhase2`), and the compact `Ecology` DTO all built from the single committed root, sharing one exact `sessionId`, `source`, and `planStateRevision`.
- **FR-3.2:** Command, hit-test, and competition responses MUST similarly obtain their paired `(stage4, ecology)` readout under one lock check matching the returned view's `(source, planStateRevision)`, returning `null` for both if the session moved concurrently (zero torn frames).

### 5.4 Additive `ecology` DTO and hard payload budgets (`MI-P1-02`)
- **FR-4.1:** Existing `view`, `scenes`, `labels`, and `stage4` members on `GET api/multi/view` and command/hit/competition payloads MUST remain unbroken.
- **FR-4.2:** Add an optional camelCase `ecology` object (`null` when `SpeciesModelVersion == 0`; never fabricate zeros):
  - `sessionId`, `planStateRevision`, `source`, `models: { adult, mortality, crown, regeneration, species }`, `experimentId`, `gridCellM`.
  - `speciesByTreeId`: sparse map covering living, standing-dead, fallen-dead, and promoted adults; omitted key means `"sitka-spruce"` when `models.species == 1`.
  - `pruneStatusByTreeId`: engine-authoritative species-aware prune eligibility, lifts, max lifts, interval wait, next lift target, next eligible year, refusal code/reason, and plan state. Oak/Beech: max 2 lifts at `3.5 m` and `6.0 m`, 3-year wait; Sitka: max 3 lifts at `2.5 m`, `5.0 m`, `6.5 m`, 5-year wait.
  - `areas[]`: per rack `{ areaId, liveAdultsBySpecies, basalAreaM2BySpecies, deadwoodBySpecies, plantedAliveBySpecies, cellCounts: { total, withNatural, withBramble, withBracken } }`.
  - `cellSchema`: `["areaIndex","col","row","light","bramble","bracken","speciesMask","bandCount"]` where `speciesMask` bitmask is `1=sitka-spruce`, `2=sessile-oak`, `4=beech`.
  - `cellsCompact`: compact numeric rows matching `cellSchema` — zero per-cell contributor IDs, zero duplicate polygon vertices (client reuses `stage4` clipped cell polygons). Never round a positive non-zero value down to `0` (preserve small positive values via a minimum positive rounding floor, e.g., `0.0001` or `1e-4` when rounded value would be `0` while original `> 0`).
  - `planted[]`: exact planted juveniles with map coordinates (`u`, `v`), species, age, height, alive/promoted/death state, and latest browse/veg diagnostics.
  - `shelters[]`: shelters with map coordinates (`u`, `v`), `juvenileId`, `installedYear`, `effectiveYears`, engine-derived `expiryYear` (`installedYear + max(0, effectiveYears)`), and engine-derived `isEffective` (strict `< expiryYear` and not failed).
  - `patches[]`: circular spot-clear patches with map coordinates (`u`, `v`), `radiusM` (`≈ 0.564`), `createdYear`, `updatedYear`, `bramble`, `bracken`.
  - Read-only plan queues: `pendingClear[]`, `approvedClear[]`, `pendingPlanting[]`, `approvedPlanting[]`.
- **FR-4.3 (Hard payload budgets):** On both `MA-1` and `MA-1-MIXED`, at both Year 0 and Year +5:
  - Raw UTF-8 JSON bytes of `GET api/multi/view` MUST be **≤ 512 KiB** (524,288 bytes).
  - Raw UTF-8 JSON bytes of the incremental `ecology` object MUST be **≤ 150 KiB** (153,600 bytes).
  - Measured in raw UTF-8 bytes, not character counts.

### 5.5 Read-only `POST api/multi/cell` endpoint (`MI-P1-02`)
- **FR-5.1:** Register `POST api/multi/cell` behind all existing `PlayerApi.HandleAsync` guards (Host, capability, route+method, Origin, `Content-Type: application/json`, 16 KiB body limit, duplicate/unknown/null member rejection).
- **FR-5.2:** Strict camelCase request body: `{ fence: { sessionId, source, planStateRevision }, areaId, column, row }`.
- **FR-5.3:** Responses:
  - `200 OK`: full cell detail (`cell`: `areaId`, `column`, `row`, `centreXM`, `centreZM`, `centreU`, `centreV`, `clippedAreaM2`, `polygon[]`, `lightIndex`, `geometricCoveredFraction`, `bramble`, `bracken`, `browsePressure`, `contributingTreeIds[]`, `cohorts[]` with `{ species, origin, establishYear, originYear, heightM, density, lastBrowsedFraction, lastBrowseAssessmentYear }`, `clearState` covering pending/approved/none) PLUS fresh `view`, `scenes`, `stage4`, `ecology`, and `refusal: null`.
  - `400 Bad Request`: malformed JSON, missing/null/duplicate/unknown members, invalid field types or lengths.
  - `409 Conflict`: stale fence (`PlayerSessionMismatch`, `PlayerSourceStale`, `PlayerPlanStateStale`), unknown area (`MultiAreaUnknownArea`), wrong area (`MultiAreaWrongArea`), or invalid/out-of-bounds cell (`CellNotInArea`) — returning a fresh `view`, `scenes`, `stage4`, `ecology`, and `refusal`.
- **FR-5.4:** Strictly read-only: zero state mutation, zero `planStateRevision` or `source` revision increment on either `200` or `409`/`400`.

### 5.6 Pure inline SVG visual system (`MI-P1-03`)
- **FR-6.1:** Remove **all** `data:image/webp;base64,...` Sitka sprites from `PlayerMultiApi.cs`. Zero `data:image/webp` strings may remain in the served page or script.
- **FR-6.2:** Define high-detail pure inline SVG `<symbol>` and `<pattern>` definitions inside `/multi`'s `<defs>`, matching the silhouettes and palette families of the four committed reference boards while remaining CSP-clean (`script-src 'self'; style-src 'self'; img-src data:; connect-src 'self'`; zero inline JS, zero external fonts/URLs).
- **FR-6.3:** Required exact IDs in `<defs>` (all 21 MUST be present):
  `#ccf-sitka`, `#ccf-oak`, `#ccf-beech`, `#ccf-standing-dead`, `#ccf-log`, `#ccf-crop-ring`, `#ccf-sapling-sitka`, `#ccf-sapling-oak`, `#ccf-sapling-beech`, `#ccf-shelter-active`, `#ccf-shelter-expired`, `#ccf-spot-clear`, `#ccf-natural-seedlings`, `#ccf-bramble-hatch`, `#ccf-bracken-hatch`, `#ccf-action-fell`, `#ccf-action-prune`, `#ccf-action-clear`, `#ccf-action-plant`, `#ccf-action-shelter`, `#ccf-action-spot`.
- **FR-6.4 (Five locked visual-semantics rules):**
  1. `ClearUndergrowth`: whole `2.5 m` cell dashed highlight sparing natural seedlings inside (`Whole 2.5 m cell · spares seedlings`), never a tree-crown badge.
  2. `SpotClear` (~`1 m²`): circular dashed ring drawn to exact world scale `radius = patch.radiusM ≈ 0.564 m` over bare cleared soil/twigs with **no seedling inside**; rendered in a separate map layer beneath `PJ` saplings.
  3. Work Plan states, never colour-only: `Pending` = dashed gold outline (`#edc774`) + clock/action glyph + text; `Approved` = solid pale-blue filled badge (`#88c5e8`) + checkmark/action glyph + text; `Executed` = removed from active map badges, shown only as a dark-slate `Completed · History Log` record.
  4. Tree shelters: paired solid cyan-blue `#ccf-shelter-active` (< 8 yr) and broken dashed grey `#ccf-shelter-expired` with `X` (8+ yr). **Expired Shelter is a map/legend state only, never a Work Plan action button.**
  5. Retained crop tree: living crown framed by a high-contrast double gold ring + small `C` badge (`#ccf-crop-ring`), never a sawn-lumber icon.
- **FR-6.5 (Strict circular crown geometry):** Remove Stage 4's elliptical stretch (`scale(r*stretch, r/stretch)`). Every living and standing-dead crown MUST render at `x = u - r`, `y = v - r`, `width = 2 * r`, `height = 2 * r` where `r = item.radiusM` in map metres. A deterministic seeded rotation (`0–360°`) around `(u, v)` is permitted. Preserve the transparent true-radius hit disc. Zero DBH→radius maths in browser JS.

### 5.7 Modes, ground overlays, inspectors, and preserved workflows (`MI-P1-04`, `MI-P1-05`)
- **FR-7.1:** Provide two explicit map interaction modes:
  - `Select Tree` (default): crown clicks win and select trees; preserves Stage 4 Owner UX fold geometry at 1280×720 and 1920×1080 (including 5 competitor rows + `Add` above the fold even when rows are blocked).
  - `Inspect Cell`: explicitly swaps the right-hand inspector body to the read-only cell inspector fetched via `POST api/multi/cell` (never stacks above the tree panel). Pressing `Escape` exits cell mode cleanly without clearing unsent draft Fell ticks.
- **FR-7.2:** Render **zero** Pass 2 write controls (no active, disabled, or stub `Plant`, `SpotClear`, `Clear Undergrowth`, or `Shelter` buttons).
- **FR-7.3:** Provide one dominant ground overlay selector:
  - `Off` (default)
  - `Light Guide` — displays visible label `Modelled light · not calibrated PAR`
  - `Undergrowth` — draws `#ccf-bramble-hatch` and `#ccf-bracken-hatch` scaled by cover per cell
  - `Natural Seedlings` — draws cell-level `#ccf-natural-seedlings` presence/glyph + species/band cue only (never fabricated individual stem dots; never present `Density` as a literal stem count)
- **FR-7.4:** Fallen deadwood markers (`#ccf-log`) and tooltips/inspectors MUST show authoritative species from the engine plus the explicit label `direction schematic, not simulated`.
- **FR-7.5:** Existing Stage 4 `Fell`, `Prune` (now showing species-aware engine eligibility, lifts `X/2` for Oak/Beech or `X/3` for Sitka, next target, and wait interval), `Crop`, `Approve`, `Cancel`, `Execute`, and `Year +1` MUST continue working end to end.

## 6. Frozen anchors (must not move)

| Anchor | Expected SHA-256 |
|---|---|
| Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` |
| Multi-area `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` |
| Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` |
| Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` |
| Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` |
| Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` (runtime equality asserted by `Phase2Tests` P2-03 M-08) |
| P0 Core binary pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| MA-1-MIXED 30-y fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` |

## 7. Evidence classification

- Adult growth, C2 crowns, mixed density, pruning lift schedules/guards, juvenile light/browse/undergrowth parameters, and shelter life: **Gameplay Calibration / Transfer / Ordinal / Simulation Abstraction** from Unity (none is `IRISH DIRECT`).
- Modelled light index: **Simulation Abstraction / Gameplay Calibration** (Unity `k10a10` proxy — `Modelled light · not calibrated PAR`).
- `MA-1` and `MA-1-MIXED`: **Synthetic Experimental Demonstrations**, not surveyed Irish forecasts.
- Fallen deadwood glyph orientation: **Visual Schematic** (`direction schematic, not simulated`).
- Natural seedling glyph: **Cell-Level Presence Abstraction** (`Density` is abstract occupancy/abundance, never a literal stem count).

## 8. Save / backward compatibility & security

- **Save impact:** None. `/multi` is strictly in-memory and session-only; nothing is written to disk.
- **Security / write-back:** `POST api/multi/cell` passes every unchanged `PlayerApi.HandleAsync` guard (Host, 256-bit capability, route+method, Origin, JSON content-type, 16 KiB limit, strict member validation) and is strictly read-only. No capability URL is ever written into Git.

## 9. Acceptance rows

- **MI-P1-01:** Phase 2 default (`1/1/1/1/1`), Core default `0/0/0/0/0`, `--multi-experiment=ma-1|ma-1-mixed` CLI validation, deterministic `MA-1-MIXED` demo factory, truthful manifest & header badges, single-lock atomic composite snapshot.
- **MI-P1-02:** Additive camelCase `ecology` DTO (with `null` when `species == 0`, non-zero floor, species-aware prune status, compact cells), hard byte budgets (`≤ 512 KiB` view, `≤ 150 KiB` `ecology` on both experiments at Year 0 and Year +5), read-only `POST api/multi/cell` (`200`, `400`, `409`, zero mutation).
- **MI-P1-03:** Zero `data:image/webp` in served page/script; all 21 required inline SVG `<symbol>`/`<pattern>` IDs present in `<defs>`; visual alignment with committed `Standard_SVG_Asset_Sheet_v2.png` (`35a44df4…227f`) and three species boards (with brief conflict rule on Expired Shelter); all 5 locked visual-semantics rules satisfied; strictly circular crowns (`width == height == 2 * radiusM`).
- **MI-P1-04:** Two modes (`Select Tree`, `Inspect Cell`), inspector body swap in cell mode, `Escape` preserves draft Fell ticks, single ground overlay selector (`Off | Light Guide | Undergrowth | Natural Seedlings`), zero Pass 2 write controls, honest caveats (`Modelled light · not calibrated PAR`, `direction schematic, not simulated`), species-aware Prune + existing Stage 4 Fell/Crop/Approve/Cancel/Execute/Year+1 intact.
- **MI-P1-05:** Real loopback + real headless Edge browser verification at 1920×1080 and 1280×720 on both `MA-1` and `MA-1-MIXED`: zero horizontal scroll, Stage 4 R3 blocked-competitor 5-row + `Add` fold gate, cell inspector swap, `Escape` draft preservation, keyboard focus, zero console/CSP/5xx errors, circular crown DOM check, `POST api/multi/cell` 200 & 409 live verification.
- **MI-P1-06:** Clean builds (`0/0` warnings/errors), full Core test suite (`245/245` existing + all new Pass 1 tests, identical PASS set across two runs), `ui-smoke-multiarea.sh` no regression + Pass 1 checks, protected listeners intact, clean `git status --short`, durable handoff committed and pushed on task branch only.
- **MI-REG:** All 9 frozen anchors (§6) re-observed unchanged; zero forbidden-scope edits.
- **MI-P1-REVIEW:** Independent reviewer verification and checklist sign-off (unticked until independent review).

## 10. Stop conditions

Stop immediately and report (without improvising or rebaselining) if:
1. `.p0ref/SitkaLab.Core.dll` is missing or fails sha256 pin `9a99f781…34b3`.
2. Any frozen anchor in §6 moves.
3. `GET api/multi/view` cannot meet `≤ 512 KiB` or `ecology` cannot meet `≤ 150 KiB` even with compact encoding + lazy cell detail.
4. Reconciling `0e777455` onto `cf6fe7a5` would require modifying Phase 2 biology.
5. Real loopback server or real Microsoft Edge cannot be driven.
