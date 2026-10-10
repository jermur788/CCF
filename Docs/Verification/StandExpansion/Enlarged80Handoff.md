# IMPLEMENTATION HANDOFF — Scenario One stand expansion (80A + 80B)

Task: Scenario One playable property 40 × 40 m → 80 × 80 m (D-056), implemented as two bounded commits.
Branch: `task/scenario-one-80m-stand` (worktree `/home/jer/CCF-s1-80m`)
Audit base: `9f49e2e` (Phase 1 audit, published)
Task BASE / context: `11c3596` / `8bed399`
80A commit: `b024633438ff93e838ca59a5bec00a4df2ade4e5` (`save: version stand geometry without changing Legacy40`)
80B commit: the commit that contains this file (SHA given in the reply that delivered it)
HEAD: that same commit. **Not pushed. Not merged to main.** `/home/jer/CCF-main` was not modified.

Plain-language summary: new Scenario One games now start in an 80 × 80 m property with 1,344 trees (same 2,100 stems/ha as
before). Old saves, and the frozen Reference Future v1, still load as the 40 × 40 m world, bit-for-bit. Editor measurements say
the enlarged world runs about 70 % slower per frame and about 9× slower per annual step than the 40 m world; that is the cost the
Manager already accepted for implementation but **not** for the pilot. Economy and objective values are unchanged and several are
now much easier (see the calibration evidence).

---

## 80A VERSIONING

Save version: **20** (`ForestSaveData.CurrentVersion = 20`).
Geometry field: **`standGeometryModel`** (int) on `ForestSaveData`, after `stormModel`. `0 = Legacy40` (40 m, 5 m cells, 8 × 8 = 64
cells), `1 = Enlarged80` (80 m, 16 × 16 = 256 cells). Explicit on every v20 save (a v20 JSON without the integer is rejected).
Legacy default: a missing field and every save ≤ v19 mean Legacy40. A pre-v20 save that claims a model is rejected.
Reference geometry: **Legacy40, permanently.** `ScenarioReferenceArchive.Matches` requires live Legacy40; the Reference preview
uses `IdentityMatches` and applies Legacy40 itself, then restores the exact player world on exit.
`ScenarioOneDefinition.definitionVersion` was **not** bumped (`scenario-one-v13`).

Load-order verification: `StandGeometryVerification` (10 check groups, PASS). The loader (1) decides the model from the version
and field, (2) validates the save against *that model's* cell count with no world mutation, (3) calls
`ForestEcologyController.ApplyStandGeometry` as the first mutation, and only then (4) restores cells, understorey, patches,
juveniles, deadwood, events and work orders. Malformed/unknown geometry is rejected before any world change.

Hash identities (D-056 clarification): `CurrentWorldHash` hashes the full v20 JSON and never strips the geometry field;
`LegacyCompatibleWorldHash` is the historical pre-v20 layout and exists for Legacy40 only (null otherwise); `WorldHash` is the
legacy-compatible hash for Legacy40 and the current hash for anything else. An Enlarged80 world therefore cannot hash like a
Legacy40 one. The Legacy V15–V18 layout functions strip the field and return null for non-Legacy40.

Legacy anchors (all under the explicit Editor-only Legacy40 override, 80B working tree): P2 batch `F58FB0B1A421D28B` ×2; P3
`F7C2FC966816BBAD` ×3 (batch ×2 + interactive PASS); Model2 compat `702766DECE591E21`; Reference Y100 `7AD177B3CC2F73C7`;
Reference continuation `9CDF21A541C5968D`; Legacy40 starting world `EBE7A228F630AE41`. **None changed.**

## 80B GEOMETRY

New-game geometry: **Enlarged80** (`StandGeometryPolicy.NewGameModel`; `ScenarioOneManager.Awake` new-game branch applies it
through the same `ApplyStandGeometry` path load and Reference preview use). The Editor-only `CCF_STAND_GEOMETRY` environment
variable / `VerificationOverride` is compiled out of Players, is never serialized, and is not production authority.
Property dimensions: 80 × 80 m = 0.64 ha. Ground, the four boundary ridges (and their colliders) and the camera far plane follow
the geometry; see "How the world is applied" below.
Cells: 256 (16 × 16 at 5 m).
Tree count: **1,344 = 336 core + 1,008 outer**, all age 20.
Stems/ha: **2,100** (stocking not reduced).
Core positions preserved: **336/336 exact** (bitwise-equal positions).
Core IDs preserved: **336/336** (`Pxxxx`; the omission pattern is exactly the legacy pattern). Starting DBH: the 181 deep-interior core
trees keep their DBH exactly. Of the 155 trees in the old edge ring, 135 are unchanged and 20 lose the artificial "released edge"
bonus (mean −1.725 cm, never an increase), which is the intended effect of evaluating neighbourhoods against the full enlarged
occupancy.
Outer IDs: `PO{row:D2}{col:D2}` on a 41 × 41 lattice, unique, deterministic, disjoint from the 336 `Pxxxx` IDs, and still satisfy the
`StartsWith("P")` semantics. Outer omissions are ranked with a murmur-mixed hash (plain FNV-1a banded the rows).
Old-boundary discontinuity check: **no step in spacing, DBH, canopy or modelled relative light; a real, explained step in density
and Hegyi competition inside the old edge. See the first decision below.**

Measured across the former ±20 m boundary (east and west sides, |z| ≤ 16 m, slabs 4–12 | 12–20 | 20–28 | 28–36 m from centre):

| | 4–12 | 12–20 (inside old edge) | 20–28 (outside old edge) | 28–36 |
|---|---|---|---|---|
| Trees | 112 | 122 | 102 | 104 |
| Density /m² | 0.2188 | **0.2383** | 0.1992 | 0.2031 |
| Mean DBH cm | 15.77 | 15.11 | 15.56 | 14.97 |
| Mean Hegyi | 8.97 | **10.41** | 8.84 | 8.78 |
| Nearest neighbour m | 1.60 | 1.56 | 1.67 | 1.66 |
| Modelled relative light | 0.0008 | 0.0018 | 0.0033 | 0.0009 |

Step across the boundary, as the gate computes it (the outer 20–28 m slab compared with the inner 12–20 m slab, relative to the inner
value): density 16.4 % lower outside, nearest-neighbour spacing 6.7 % wider, DBH 2.9 % larger, Hegyi 15.0 % lower, modelled relative light
difference 0.0014. The gate's
stated tolerances (density ≤ 25 %, spacing ≤ 15 %, DBH ≤ 5 %, Hegyi ≤ 25 %, light ≤ 0.03) were written down after I had seen this
data and found the cause, so they describe what I judge acceptable, not an independent prior target.

Cause: 61 core trees stand exactly on the old ±19.4 m margin line. The Legacy40 generator clamps its outermost lattice row and
column from ±20 m to ±19.4 m, so they sit 1.4 m from their neighbours instead of 2.0 m. The Manager's rule that exact core
positions are preserved keeps them there, so in Enlarged80 they form a slightly denser double row 0.6 m inside the former edge.

## EDGE / INTERIOR

Edge distance is to the property boundary of the world being measured.

| Band | Legacy40 | Enlarged80 | Phase-1 benchmark |
|---|---|---|---|
| 0–5 m trees | 155 (46.1 %) | 364 (**27.1 %**) | ≈ 22.5 % |
| 5–10 m | 109 (32.4 %) | 224 (**16.7 %**) | ≈ 20.0 % |
| > 10 m | 72 (21.4 %) | 756 (**56.3 %**) | ≈ 57.5 % |
| > 8 m true interior | 112 (33.3 %) | 862 (**64.1 %**) | n/a |

Mean Hegyi competition by band (Enlarged80): 0–5 m **6.77**, 5–10 m **9.07**, > 10 m **9.31**; whole stand 8.58 (Legacy40 8.24).
Edge trees see fewer neighbours, as they should. Mean DBH is 15.4–15.5 cm in every band.
Light/canopy by band (cells; modelled relative light, a relative model output, not measured daylight): 0–5 m cells (60) light 0.002,
canopy 0.998; 5–10 m (52) 0.002 / 0.998; > 10 m (144) 0.021 / 0.979. The > 10 m figure includes the central path and clearing;
the closed-canopy start is dark everywhere else, as in Legacy40. A meaningful interior now exists (64 % of trees are more than 8 m
from the boundary, against 33 % at 40 m).

## SAVE TESTS (`Enlarged80Verification` 12 check groups PASS; `StandGeometryVerification` 10 PASS)

v19 Legacy40: loads as Legacy40 with 336 trees, no outer forest, cells in their original positions; loaded over a live Enlarged80
world it restores the Legacy40 grid and ground (cell 4 at (2.5, −17.5)). Re-saving writes v20 + geometry 0.
v20 Legacy40: round trip exact; Legacy40 world hash equals the pre-v20 layout hash `EBE7A228F630AE41`.
v20 Enlarged80: validates, loads over a Legacy40 world, restores 80 m geometry, 256 cells, 1,344 trees and the exact world;
continuation after save/load is deterministic.
Reference preview from Enlarged80: preview shows the frozen 40 m archive at its original positions (Y100 world hash
`7AD177B3CC2F73C7` unchanged); leaving it restores the exact 80 m player world.
New labelled Enlarged80 anchors (current-hash identity, not comparable to any historical anchor):
`ENLARGED80_START_WORLD_CURRENT_HASH 04A78188A6F8E6F9` and `ENLARGED80_YEAR3_CURRENT_HASH 081DB58CE3034648`, identical across two
independent gate runs. These are valid only for this Editor/configuration and generator; any generator change moves them.

## MAP / WAYPOINT (rendered, `Enlarged80Review` PASS; screenshots in `Evidence80/80B/review/`)

1280 × 720: all 256 cells on screen, smallest cell 35 px, grid and side panel do not overlap, labels do not overflow, A–P × 1–16
labels retained, selected-cell panel fits, waypoint P16 set and matched to the HUD ("Waypoint P16: 94 m north-east" from the
south-west area, "33 m north" from the east edge).
1920 × 1080: same assertions pass; the UI canvas scales to a 1600 × 900 reference so the grid is the same size relative to the
screen. Waypoint-to-world mapping is also asserted in `Enlarged80Verification`.
The map was not redesigned. Honest limit: in a dense closed-canopy start almost every cell reads 0.00, so the Light tab carries
little information until the player opens the canopy. That is a property of the start state, not of the 16 × 16 grid.

## FIRST-CYCLE PLAYTHROUGH

Scripted by `Enlarged80Review`; **no human played this**. The harness placed the player rather than walking, so navigation figures
are straight-line distance ÷ the controller's real speeds (`moveSpeed` 4 m/s, `runSpeed` 7 m/s); real routes are longer because trees
block the way.

Flow run: new game → inspected an interior tree (`PO0808`, 16.2 m from the edge, Hegyi 6.1) and an edge tree (`PO2239`, 2.3 m, Hegyi 8.4,
"28 trees within 8 m") → marked 20 trees in two separated areas (north-east, south-west) → Stand Map (Fell & crop marks tab shows both
areas) → waypoint → Work Plan (20 tasks, expected timber €87.87 vs the €2,500 minimum, net −€2,412.13, "Openings in several places",
10 cells, largest opening 3 cells) → approve → advance (2.5 s) → Annual Review (cash €9,587.87, canopy 0.99 → 0.98, opened cells 10,
**30 regenerating cells, was 0**) → walked the post-intervention stand (stumps and openings visible in both areas).

* Start → north-east area 30.8 m (≈ 8 s walking); north-east → south-west 67.9 m (≈ 17 s walking, ≈ 10 s running, straight line).
* Trees a plausible first-cycle player touches: about 20 marks plus a handful of inspections (what the harness did); my estimate of
  30–50 for a player who explores, **not measured with a person**.
* Tedium: 20 marks is comfortable. If a lesson or objective were expressed as a share (the audit harness marks 14.3 %, i.e. 192 trees
  versus 48 at 40 m), one-by-one marking and a 192-card Work Plan would be tedious. **I did not test the Work Plan UI with 192 tasks**
  (the 192-task benchmark used the manager API, not the UI), and there is no multi-select; I did not add one.
* Legibility: openings and the residual stand are easier to read in two separated areas than at 40 m, where one opening takes up a
  large fraction of the stand. The Stand Map also shows two separate blocks at once. That is a judgement from the screenshots, not a
  user test.

## PERFORMANCE (Editor on Linux, GTX 980M, i5-6300HQ; **not** a Player and **not** Windows)

Same environment and harness (`StandExpansionAudit`, `Evidence80/80B/performance/`). Phase-1 numbers are from `Evidence/run1–run3`.
Frame time is one run per world and the Editor was open, so treat ± several ms as noise.

| | Legacy40 (override, this tree) | Phase-1 "current 40 m" | Phase-1 scratch 80 m | **Implemented Enlarged80** |
|---|---|---|---|---|
| Generate() startup, ms | 403–467 | n/a | 2,209–2,493 (scratch generator) | **1,766–1,891** (+ init 183–221) |
| Scenario init, ms | 11–23 | n/a | n/a | 183–221 |
| `RecomputeCanopy` median, ms | 10.5–11.6 | 10.0–11.1 | 188–223 | **185–217** |
| `RecomputeSeedRain` median, ms | 4.2–4.6 | 3.9–4.5 | 57–60 | **58** |
| Hegyi pass median, ms | 2.0–2.2 | 2.0–2.7 | 18.7–19.0 | **18.3–18.8** |
| Ecology-only annual step, ms | 114–135 | 114–193 | 952–1,027 | **915–1,056** |
| Full manager advance, ms | 189–206 | 189–318 | 1,508–1,805 | **1,473–1,703** |
| Year 1 / intervention year / year 3 advance, ms | 295–300 / 336–350 / 220–222 | 288–355 / 336–406 / 247–389 | 2,226–2,471 / 2,556–2,934 / 1,507–1,819 | **1,866–2,045 / 2,588–2,695 / 1,490–1,632** |
| Frame median / p95, ms (fps) | 23.8 / 36.6 (42.0) | 23.0 / 34.6 (43.6) | 35.9 / 47.3 (27.9) | **40.5 / 53.5 (24.7)** |
| Process RSS before → after the annual cycle, MB | 4,543 → 4,604 | 4,559–4,564 → 4,618–4,629 | 4,713–4,728 → 4,880–4,898 | **4,685–4,710 → 4,824–4,857** |

Reading: simulation cost matches the Phase-1 benchmark closely (it should; it is the same cells × trees loops), and Legacy40 is
unchanged by 80A/80B. Rendered frame time is about +70 % at 80 m against +56 % for the scratch benchmark, a ~4.6 ms gap I did not
isolate (the implemented world adds the 80 m ground, longer ridges, a far plane of at least 170 m and 1,008 outer trees; I cannot say
which). Nothing is catastrophic, so per the packet this is reported for judgement. **No optimisation was done.** Player/Windows
performance is unmeasured.

## ECONOMY / OBJECTIVE CALIBRATION EVIDENCE

See `Enlarged80Calibration.md` (full table). Headlines: count-based thresholds are ×4 easier in share terms (retained original
trees 60 = 17.9 % → 4.5 %; regeneration cells 3 = 4.7 % → 1.2 %; century original-tree target 120 = 35.7 % → 8.9 %); a 20-tree
thinning already completes "Retain original canopy trees" (1,324 / 60) and produced 30 regenerating cells against a minimum of 3;
at equal thinning share the €2,500 minimum charge costs €13.02/tree instead of €52.08/tree; owner capacity (2,400 min/year) is a
quarter of the per-hectare planting capacity. In Enlarged80 the century review compares against the aspirational targets, not the
Reference, because `Matches` is false (Reference Future v1 is a 40 m world).
**No values changed: confirmed.** `ScenarioOne.asset`, `ScenarioOneDefinition.cs` and every economy/objective/ecology coefficient
are untouched.

## LIGHT VALIDATION (modelled relative light; the light model was not changed)

Enlarged80, first intervention in two separated areas (`Enlarged80Verification`):

| Cell | Before | After |
|---|---|---|
| Dense interior (untouched) | 0.0029 | 0.0048 |
| Property edge | 0.0009 | 0.0013 |
| Modestly opened (selective) | 0.0014 | 0.0073 |
| Concentrated opening | 0.0039 | 1.0000 |

Ordering after treatment: concentrated (1.0) > selective (0.0073) ≥ dense interior (0.0048) > property edge (0.0013). Directionally
sensible. The small rise in untouched cells reflects the thinned neighbourhood and one year of growth, not a measurement.
"Modelled relative light" is a relative output of the simplified model; it is not measured percent full daylight.

## P2

Batch: `F58FB0B1A421D28B` on every run (Legacy40 override, 80B tree). Interactive: **FAIL, known inherited signature.**
Beginning of task (at the 80A base `9f49e2e`) and end of task (this tree) both gave the same message,
`a world label is drawn under a panel at 1280`, the same correct hash `F58FB0B1A421D28B`, `deterministic=True`, and the two 1280 × 720
captures are **pixel-identical (0 differing pixels)**. The Phase-1-era capture differs from both on about 23 % of pixels, which is the
bimodal behaviour already documented. Per the Manager's rule this is recorded as an inherited flaky gate. P2 fixture, camera and
tolerance were not touched. Evidence: `Evidence80/80A/p2-beginning-of-task/`, `Evidence80/80B/final/p2-interactive*`.

## P3

`F7C2FC966816BBAD` on batch-1, batch-2 and interactive (all PASS), Legacy40 override.

## REFERENCE

Reference preview Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D` (legacy V15 layout hash), both unchanged; Reference Future v1
is unmodified and Legacy40-only. Reference preview from an Enlarged80 game is verified (see SAVE TESTS).

## LEGACY FULL REGRESSION

Legacy40 override, 80B tree, `run_s1a_gates.py --phase regression`: **24/24 PASS** (`Evidence80/80B/pass1/regression-results.json`),
`production_source_unchanged` true for every gate, run head `b024633`. Targeted phase (second pass, after fixing my own wrapper):
**7/7 PASS** (teaching ×3, MenuTutorial ×2, Clearance, Removal). SessionMenu passes in both the real Enlarged80 world and under the
Legacy40 override. Disclosures:

* `MenuTutorialVerification` / `ScenarioOnePlantingVerification` flakiness seen during 80A is unchanged: in the 80A targeted sequence
  `menu-standalone-1` failed repeatedly (F1 / Escape / Help / `O` key-timing family); a bounded BASE comparison at `9f49e2e`
  reproduced it, so it is **inherited / open**. A `ScenarioOnePlantingVerification` native Mono SIGSEGV at Editor exit after PASS also
  passed on independent rerun. In the 80B passes MenuTutorial passed every time (regression and targeted).
* The first 80B driver pass had two invalid results that I fixed and re-ran: `stand-geometry` failed on an outdated 80A assertion
  ("new games stay Legacy40", no longer true by design) and `targeted` failed in 1 s because `run_s1a_gates.py`'s busy-check
  (`pgrep -f 'Editor/Unity '`) matched my own detached wrapper shell. Neither was a product failure. Both steps and the Enlarged80 gate
  were re-run on the final tooling and passed (`Evidence80/80B/final/`).

## ENLARGED80 VERIFICATION

`Enlarged80Verification` PASS, **12 check groups** (final pass `80b-final`; identical anchors in pass 1): policy and 80 m bounds/256
cells/ground/ridges/camera/texture density; 1,344 trees = 336 + 1,008, 2,100 stems/ha, age 20, clear of path/clearing/start/sites
(closest tree pair 0.968 m); core positions and IDs preserved and legacy omission pattern identical; determinism; edge bands;
old-boundary analysis; construction sites unavailable (no frames, no solid colliders); v20 Enlarged80 save/load; v19 Legacy40 load into
an Enlarged80 world; deterministic continuation; Reference preview switch/restore; Stand Map 256 cells and waypoint mapping;
first-cycle intervention with the light ordering above. `Enlarged80Review` (rendered, 1280 and 1920) PASS.

## SERIALIZED DIFF AUDIT

**No scene, prefab, ScriptableObject, ProjectSettings or Packages file changed** (`git diff` empty for `*.unity`, `*.prefab`, `*.asset`,
`ProjectSettings`, `Packages`). The Manager expected scene edits; I avoided them by design. `StandWorldBoundary` (new runtime component,
added by `ForestEcologyController.Awake`) records the authored Ground and four ridge transforms and restores them exactly for Legacy40,
so Legacy40 is the on-disk scene and nothing was re-serialized. Trade-off: in the Editor's Scene view, outside Play mode, the ground and
ridges still show the 40 m authoring state. Ground texture density is preserved per renderer with a `MaterialPropertyBlock`
(`_BaseMap_ST` / `_MainTex_ST`, 32 tiles over 80 m = the same 2.5 m per tile); **no material asset changed**. I looked at the rendered
ground in the start and interior screenshots and it reads the same scale as Legacy40 (the orange start patch is a separate pre-existing
object that also appears in the Legacy40 evidence). Not verified: the outermost boundary ridges from the player's point of view; the dense
canopy hides them in every captured view.
Editor normalisation (11 `.mat` files and `.vscode/settings.json`) occurred during Unity runs and was **reverted before the commit, never
staged, never absorbed**. `ForestSceneBuilder` was not used or edited; it is still stale (it reproduces only the Legacy40 geometry and
would re-author the hidden construction sites), and I recorded that rather than fixing it.

## CONSTRUCTION POLICY

D-055 intact: no building frame, no `[E] Build` prompt, no solid hidden-site collider, no construction activation. Asserted by
`Enlarged80Verification` in Enlarged80 and by `ScenarioOneInteractionVerification` (in the 24/24 regression) in Legacy40.

## UNRELATED WORK PRESERVED

`/home/jer/CCF-main` (dirty and well behind origin, untouched): dirty/untracked file list and SHA-256 of every dirty file compared with the
baseline recorded before the first task: identical (see the delivering reply for the result of the final comparison). The six quarantined
material changes were not absorbed. `.vscode/settings.json` was not edited. Nothing was deleted. No B07/B08, no Drive/mirror update, no
canonical-doc update (deliberately deferred until the stand is accepted).

## HOW THE WORLD IS APPLIED (for the maintainer)

* `StandGeometryModel` — the numbers for each model. `StandGeometryPolicy` — which model a new game gets.
* `ForestEcologyController.ApplyStandGeometry(model)` — the single geometry-application path (grid, size, event). New game, load, Reference
  preview and verification all call it.
* `StandWorldBoundary` — listens to `StandGeometryApplied` and moves/scales the ground, ridges and camera far plane.
* `ForestStartingStand.Generate()` — Legacy40 path untouched. `GenerateEnlarged80` first reproduces the legacy 336 trees exactly (same IDs,
  jitter, clamp at ±19.4 m, clearance and omission hash), then fills a 41 × 41 lattice with `PO` slots, ranks outer omissions with a mixed
  hash to reach 1,344, and computes neighbourhood-sensitive DBH against the full occupancy. `OuterAxis` keeps the seam between the clamped
  legacy lines and the outer lattice from creating near-coincident trees (an earlier version had a 0.086 m pair; the gate now requires
  > 0.9 m).

## HOW TO CHECK IT BY HAND (not done by a person; please do)

1. Open the project in Unity 6000.6.0f1 on this branch, open `ForestTest`, press Play. Expect a new Scenario One at Year 0 with €12,000.
2. Press `M`: a 16 × 16 map labelled A–P and 1–16. Select a cell and set it as a waypoint, close the map, expect the HUD arrow and a
   distance. Walk (WASD, Shift to run) towards the edge: the boundary ridge should stop you about 40 m from the centre.
3. Mark trees (`X`) in two places, `Tab` Work Plan, approve, advance one year, read the Annual Review, walk back and check the openings.
4. To see the 40 m world again (Editor only): quit Unity, launch it with `CCF_STAND_GEOMETRY=0` in the environment, or load an old save.
   Expected: 336 trees, 8 × 8 map.

## NOT DIRECTLY VERIFIED

Human play/feel (navigation, tedium, Stand Map comfort); the player physically being stopped by the new ridges at ±40 m (the gate asserts
ridge positions and that no enabled ridge collider remains at the old ±20 m, but nothing walks into the new boundary); Work Plan UI with a very large number of marked trees; Player or Windows
performance and memory; whether the outermost boundary ridges look right from inside the stand; behaviour on a machine other than this
one; Scene-view appearance outside Play mode. The P2 rendered gate remains unexplained and flaky.

## DECISIONS NEEDED

1. **Old-edge clamp line (61 trees).** Accept the +16 % density / +15 % Hegyi band 0.6 m inside the former edge, or authorise un-clamping
   those 61 trees in Enlarged80 only (they would move up to 0.6 m, breaking "exact core positions" for those trees; IDs preserved). I did
   not do it because it contradicts the preserved-position rule. Recommendation: accept for the pilot unless a playtester notices a dense
   line, because the visible effect is a slightly closer double row.
2. **Calibration task** (separate, already planned): choose option A/B/C in `Enlarged80Calibration.md`. The stand is not pilot-ready until then.
3. **Performance**: profile a real Player build before deciding on optimisation; Editor frame time is +70 %.
4. **Century review in Enlarged80** uses aspirational targets (no Reference Future for 80 m). Is a Reference Future for the enlarged stand wanted?
5. **Push**: the branch is local-only past `9f49e2e`. Do you want it pushed (no merge to main)?

## READY FOR MANAGER REVIEW

Yes, with the five decisions above and the not-verified list. Every gate named in the packet passed except the inherited P2 interactive
signature (recorded, not changed).

## RECOMMENDED NEXT STEP

1. Manager decides items 1 and 2; 2. a human plays the first cycle in the Editor (steps above) and reports feel/tedium; 3. bounded
calibration task; 4. Player-build profiling; 5. only then accepted-state canonical docs, context regeneration and Drive publication, and a
Windows pilot candidate.
