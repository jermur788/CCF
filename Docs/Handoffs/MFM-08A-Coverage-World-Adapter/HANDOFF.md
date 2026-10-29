# MFM-08A — authoritative coverage-world adapter: HANDOFF

**Claim:** #129 · **Executor:** Anthropic Claude Sonnet 5.5 via Claude Code (local lane) · **Status:** COMPLETE, gates run
**Branch / worktree:** `feature/mfm-v1-coverage-world-adapter-v1` / `/media/jer/Videos/CCF-MFM-08A`
**Exact base:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5` (post-MFM-07, save schema v23)
**Candidate (gated):** `716486a21588e76a482d3eaea4fed12069f75ffd` — this HANDOFF is a docs-only commit on top of it.
Commit chain (all preserved, none rebased): `c8cfae8c` → `a9d6cc6b` first adapter → `aff488a5` Manager disposition (protected areas fail closed, shelters by existence) → `bd79c78a` gate diagnostics → `333b0ab4` headland radii as `ContainsStemCentre` computes them (float) → `716486a2` gate asserts juvenile outcome either way.

## What changed (production)
- `ForestStartingStand.cs`: two read-only accessors, `PathCorridorRadiusMeters` / `ClearingRadiusMeters`, over the existing serialized fields (unchanged). No reflection, no copied literals.
- `ScenarioOneCoverageWorld.cs` (new, pure, no UnityEngine): `Source` → `ScenarioOneCoverage` masks + features. Refuses (null + reason) on a missing/zero radius, a missing headland, or any stored protected-area polygon.
- `ScenarioOneManager.PrescriptionApproval.cs`: `TryReadPrescriptionWorld` no longer passes empty arrays; new private `ReadCoverageWorld` gathers the Source; public `ReadCoverage(out problem, manualUnitIds)` is a thin side-effect-free view over the same `TryReadPrescriptionWorld` + `Classify` path (no cache, no state).
- Not touched: `ScenarioOneCoverage.cs`, calibration, save/schema/validator/hash, UI, teaching, Work Plan, scenes, ProjectSettings.

## Source → mask / feature mapping
| Output | Authoritative source | Notes |
|---|---|---|
| Mask `Headland` (4) | `ScenarioOneHeadlandGeometry` consts | strip + feeder strip + 2 pads; radii computed in float exactly as `ContainsStemCentre` does |
| Mask `Track` | every active `Dirt Path*` Transform (incl. Enlarged80 `Dirt Path X…`) | circle of `PathCorridorRadiusMeters`; same objects `ForestStartingStand.CollectClearancePoints` used |
| Mask `Clearing` | `Forest Clearing` position | circle of `ClearingRadiusMeters` (not visual scale); none if the object is absent |
| Mask `Site` | every `ForestBuildable` (incl. inactive) position | circle of `ClearingRadiusMeters`. **Player start / live player position is not a source** (Manager decision; no spawn persistence) |
| `PlantedJuvenile` | `plantedJuveniles`, alive and not promoted | id `juvenileId` |
| `Shelter` | `ecology.Browsing.Shelters` with `installedYear <= EcologicalYear+1` | physical tube per `ScenarioProtectionVisuals`; effectiveness/expiry ignored; id `shelterId` |
| `ClearancePatch` | `clearancePatches` | no authoritative id exists; evidence id is `clearance-<createdYear>-<x>-<z>` (deterministic) |
| `ProtectedArea` | `ecology.Browsing.ProtectedAreas` | **zero features emitted**; if any record exists the whole coverage read fails closed with an explicit problem. No producer exists in Scenario One |
| `DeadwoodRecord` | `deadwoodRecords` | id `deadwoodId` |
| `ActiveManagementWork` | open orders of every type except `FellTree` | id `order-<workOrderId>`; felling orders already reach coverage as `PlayerOpenWork` through their stems |
| `NaturalRegeneration` | ecology cells with a natural-origin cohort, density > 0 | id `regen-cell-<index>`, position = cell centre |

## Intentionally unsupported (STOP dispositions)
- Player start / moving player as a `Site`: excluded by Manager decision #129.
- Protected-area polygons: fail closed, never sampled or omitted.
- No mask or feature was invented; nothing is read by reflection.

## Verification
Offline (plain .NET, pinned MFM-03 fixtures, each run twice byte-identical), on `716486a2`:
- new `Tools/Verification/MfmCoverageWorld`: **102/102** — per-kind mapping, exact reasons, evidence ids, moving/removing a source changes the result, mapped masks equal the accepted reference construction, no mutation, deterministic and order-independent, refuses on bad radii / missing headland / protected area, static guards (no empty arrays, no reflection, no player, no writes in the reader).
- MFM-04 core **240/240**, MFM-05 candidates **95/95**, MFM-07 approval boundary **125/125**.

Unity 6000.6.0f1, lane `/media/jer/ZX20/CCF-U1` detached at the candidate under lock `MFM-08A` (released; U1 returned to `c8cfae8c`), receipts in `/media/jer/Files/CCF/Verification/MFM08A-Unity-20261029/` (`01-compile` … `06-reference`, each `process-result.json` pass:true, no lane changes remaining):
| Gate | Result |
|---|---|
| compile | `MFM06_COMPILE_PASS` |
| coverage-world (new) | `MFM08A_UNITY_PASS checks=56` |
| MFM-07 approval | `MFM07_UNITY_PASS checks=106` (gate now plans with production `ReadCoverage`) |
| MFM-06 save | `MFM06_UNITY_PASS checks=159` |
| hash-dump | `MFM06_HASHDUMP_PASS lines=18`; the 18 `MFM06_HASH` lines are **identical** to the MFM-06 base dump (`MFM-06-Unity/run2/04-base-hash-dump`) |
| reference | `REFERENCE_FUTURE_V1_PASS` |

coverage-world witness (live Enlarged80 Rack V2 world): production masks equal an independent construction from the live scene objects (areas, stems, net areas) and differ from empty masks; removing each of Track / Clearing / Site / Headland individually changes the production result (track=104, clearing=1, site=7, headland=4); moving the player changes nothing; reads mutate nothing (mark/order/event/id/cash/world hash) and are deterministic; natural regeneration (51 cells) flags 11 areas without excluding; an open planting order excludes its area as `ActiveManagementWork` with `order-<id>` evidence; after execution the shelter and clearance patch each exclude their area with their own ids, the completed order no longer does.

## Residuals / honest limits
1. In the Unity run the planted sapling did not survive the year, so the "standing juvenile → `PlantedJuvenile`" branch was exercised offline only; Unity asserted the dead-juvenile case (not mapped). Deadwood records and a runtime-built structure as `Site` are likewise offline/static witnessed, not Unity-created.
2. Protected-area fail-closed is covered offline and statically, not by a Unity-created polygon (no producer exists).
3. `NaturalRegeneration` is a point at the 5 m cell centre; a cell straddling an area boundary flags only the area holding its centre.
4. Shelter "still exists" follows the visual rule; nothing in the code removes shelter records, so a shelter on a dead juvenile still counts.
5. Dirt Path planks are found by name prefix, as the starting stand did; Legacy40 worlds lack the `X` approach planks, matching their generation.
6. Added beyond the packet surface (Manager-approved): public `ReadCoverage`; MFM-07 gate edit; `coverage-world` gate registered in `MfmApprovalBoundary/run_unity_gate.py`.
7. Unity run notes: the first compile attempt hung 30 min on stale `Temp/__Backupscenes` + `UnityLockfile` left in U1 (removed; second attempt 65–90 s). The first coverage-world attempt exposed a ~1e-5 m² difference from double-vs-float headland radii (fixed in production, `333b0ab4`).
8. Live `active-tasks.md` on `main` was not edited by this worker. No Windows build. No Reference Future anchor, hash or calibration change; no rebaseline.

## State
Worktree clean at handoff; branch pushed; Unity lock released, no Editor running; ZX20 ~22 GB free at start.

MFM COVERAGE-WORLD ADAPTER COMPLETE — MFM-08 UX MAY RELY ON PRODUCTION COVERAGE
