# Sen-1 — Scenario One Ground-Level Graphics Parity: Audit, Draft Spec and Rollout Plan

**Status:** DOCS-ONLY AUDIT + **DRAFT** spec input. Not approved. Authorises no Unity, scene, asset, material or source change.
**Date:** 2026-11-16
**Worker:** Claude Code (Opus 5.5), local repository read access; **Unity NOT RUN**
**Role:** Primary (docs/audit)
**Branch:** `docs/sen-1-graphics-parity-audit-20261116-v1`
**Base:** `660492c62682f1ab68793ef68fb45bfd6cba2965` (origin/main at task start)
**Context commit:** `7801d9d4f6163f7d196c7d28f6b34a9e7876cf0d`
**Task class (proposed):** the audit is PACKET ONLY. The implementation slice below is **SPEC REQUIRED** in the proposer's view because it is substantial cross-system presentation work. Manager classification is pending.
**Claim status at write time:** dispatched by owner paste as READY / NOT RUNNING. This worker executed it. No `active-tasks.md` registration was written because coordination records belong to the Manager.

---

## 1. Purpose and boundary

Raise Scenario One's **ground-level** presentation to one coherent quality bar. This covers ground vegetation, footpaths, the main track, ground surface materials, path-edge transitions, small ground props and lighting cohesion.

Trees, canopy, UI/HUD, Stand Map, rack geometry, save data and simulation are **out of scope**. They are mentioned only where ground work touches them.

Governing authority (unchanged by this document):

- **D-020** — art derives from authoritative state; do not invent causal states.
- **D-045** — the integrated presentation baseline (`ec938b0`); ForestFloorV1 was not adopted.
- **D-071 / D-072** — existing path, western headland and southern feeder geometry stay as they are. Topology changes need a separate user decision and spec (#362 lineage).
- **D-032** — binary storage policy is still OPEN. New textures need a reported footprint and owner approval.
- Frozen Reference Future v1, Legacy40, signed save specs and the save-subsystem reservation (#563) are protected. The #443 overlap is FROZEN.
- Game Brief, *Art, audio and authenticity*: "a beautiful, immersive and readable forest … art should make authoritative state visible rather than invent unsupported ecological state."

## 2. Evidence base and how it was obtained

| Kind | What | Caveat |
|---|---|---|
| Source (verified by reading at base) | `StandWorldBoundary.cs`, `StandApproachPath.cs`, `StandApproachRoute.cs`, `ScenarioOneHeadlandPresentation.cs`, `ScenarioOneHeadlandGeometry.cs`, `ScenarioOne/ScenarioHabitatVisuals.cs`, `ForestStartingStand.cs` (clearance), `UI/Map/StandMapSource.cs`, `ScenarioOneCoverageWorld.cs`, `Editor/ForestSceneBuilder.cs` | Read-only. Not executed. |
| Serialized (parsed read-only) | `Assets/Scenes/ForestTest.unity` (Ground, Dirt Path 00–51, Forest Clearing, ridges, light, RenderSettings); `Materials/Forest Ground.mat`, `Forest Path.mat`; `Settings/PC_RPAsset.asset`, `PC_Renderer.asset`, `DefaultVolumeProfile.asset`; `ProjectSettings/QualitySettings.asset` | Parsed with a script. Nothing was opened in Unity. |
| Existing rendered captures (historical, other workers) | `/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/original-hud-current-build-1920x1080.png` (main track, eye level); `/media/jer/Files/CCF/Verification/MFM08-PostIntegration-Unity-R2-20261031T1245Z/evidence/03-3d-candidate-chip-1920x1080.png` (stand floor); `/media/jer/Files/CCF/Verification/ScenarioOneLinuxPlayerSmoke-20261017T073037Z/smoke/loaded-forest.png` (clearing); `/media/jer/Files/CCF/Verification/scen1-ThreeSpeciesLongHorizon490-20261114T085531Z/armA-broadleaf-eye-y20-720p.png` and `armA-stand-oblique-y20-720p.png` (30 m research arena, **not** the Scenario One E80 world) | Different builds, dates and configurations. Used only for qualitative observation. **No fresh capture was taken.** |

## 3. Verified current state (implementation facts)

### 3.1 Ground

- `Ground` is a **scaled primitive cube**: authored at 40 × 1 × 40 m, top at y = 0. `StandWorldBoundary` scales it to 80 × 80 m for Enlarged80 and keeps texel density with a `MaterialPropertyBlock`.
- There is **no Unity Terrain**, height variation or micro-relief. All vegetation is placed at y = 0.012 m.
- `Forest Ground.mat`: URP Lit, base map `Forest_RootPlate_Soil_BaseColor.png` (guid `10e6dd8c…`) tiled 16 × over 40 m (one repeat per 2.5 m), tint (0.92, 0.74, 0.58), **no normal map, no occlusion map**, smoothness 0.05.
- The boundary is four primitive-cube "ridges" (5 m tall, 1 m thick) with `Stone.mat`. Nothing exists beyond them; the code comment says "There is no decorative forest outside the authoritative ecology bounds". Elevated views show flat grey void and default-skybox horizon (490 oblique capture; same mechanism in E80).

### 3.2 Main track, approach, headland, feeder and clearing

- **Main track:** 52 authored `Dirt Path 00–51` objects. Each is a **primitive cube**, 2.6 × 0.012 × ~0.61–0.66 m, on a 0.5 m step. Shadow casting is off. The authored track runs z −14.75 → +10.75 into the clearing.
- **Approach extension (Enlarged80 only):** `StandApproachPath` clones the plank at runtime as `Dirt Path X###` (inside the property) and `Approach Track ###` (outside), down to z −48.5. It adds a cube "cutting" through a gap in the south ridge.
- **Headland and feeder (Rack V2 active):** `ScenarioOneHeadlandPresentation` builds one flat runtime mesh. It has a 2.6 m strip at x −35.4, a feeder strip at z −27.4 to the track at x −2.44, and 48-segment pads of radius 2.2 m at both junction ends. It uses **world-planar UVs (x/2.5, z/2.5)** and the plank material.
- **Clearing:** `Forest Clearing` is a 9 m flat disc at (−2.77, 0.008, 11) using the same path material.
- `Forest Path.mat` uses the **same base texture as the ground** (guid `10e6dd8c…`). It differs only in tiling (3 × 3) and an **HDR tint above 1** (1.6, 1.4, 1.05). It also has no normal or occlusion map.
- Consequences visible in captures:
  - The track reads as a **uniform saturated orange ribbon with a hard, straight-cut edge**. It has no shoulder, verge, ruts, stones or litter transition.
  - The clearing reads as a **hard-edged flat disc**.
- **Texel-density mismatch (code-derived, not yet captured):** plank cube UVs map 3 × 3 repeats onto a 2.6 × 0.66 m face, which gives roughly 4:1 anisotropic stretch per plank. The headland mesh uses about 1 repeat per 0.83 m in both axes. Plank, headland and clearing surfaces therefore cannot match texture scale or orientation at the junctions.
- **Coupling (critical for any redesign):** the plank **GameObjects** are read by name or position by:
  - `ForestStartingStand.CollectClearancePoints` — tree keep-out;
  - `StandMapSource` — map path lines;
  - `ScenarioOneCoverageWorld` — the track corridor;
  - `ScenarioOneManager.PrescriptionApproval` (line ~339);
  - `ScenarioOneHeadlandPresentation` — template and height;
  - `StandApproachPath` — template;
  - `ForestSceneBuilder` (Editor).

  The readers inspected use transforms and active state, not renderers. **Hiding plank renderers while keeping the objects is therefore a plausible non-breaking path; it must be proven by test, not assumed.**

### 3.3 Ground vegetation and props

- `ScenarioHabitatVisuals.VegetationSites` derives sites from each ecology cell's understorey state (D-020 compliant).
  - Each of 7 classes gets `count = round(strength × 4)` instances, at most 4 per cell. Enlarged80 cells are 5 m, about 25 m².
  - Instances are jittered within ±0.42 × cell size around the **cell centre**.
- Results:
  - The cover is **very sparse**: at most about 0.16 instances per m² per class.
  - Placement **clusters on a grid around cell centres**.
  - Cover has **no relationship to the track, headland, clearing or edges**.
- **No exclusion for track, headland or clearing surfaces.** Only `IsVegetationDisplayCleared` (player clearance treatment) suppresses sites. Ferns, bracken and grass growing on the track surface are visible in the 490 arena eye-level capture and in the #362 track capture (tufts on the track edge). Not yet confirmed at a specific E80 pose.
- **Procedural fallback meshes use `Universal Render Pipeline/Unlit`**. Any class without an authored prefab renders unlit and ignores sun, shadow and ambient light, so it cannot sit consistently with lit neighbours.
- Authored ground assets already present:
  - `Ground_Moss_Shoots_03` cushions;
  - Bilberry cover;
  - Woodland forbs;
  - Woodland grass/rush (fixed `rushDressingPositions`);
  - SectionFive fern, bracken and bramble;
  - oak/beech litter (only under observed broadleaf crowns);
  - small deadwood, fungi and moss-on-wood (only on recorded logs);
  - ForestryGround brash and root plates.

  Brash and branch litter read well in captures. There is **no baseline conifer needle/cone/twig/stone floor dressing**: the Year-0 Sitka floor is the bare soil texture.
- Verification couplings: `ScenarioHabitatPresentationVerification` asserts counts and presence: 3 grass catalog entries, `LitterPatchCount == 0` at Year 0 Sitka, `GrassPatchCount > 0` at Y100, plus log/fungi counts. The Overview also records "Year-0 grass: 4 patches in 3 supported cells". **Any density or exclusion change will move these assertions and must update them inside scope.**

### 3.4 Lighting and post-processing

- ForestTest RenderSettings:
  - **fog off**;
  - ambient mode Trilight (sky 0.52/0.65/0.76);
  - **built-in default skybox**;
  - one directional light, colour (1, 0.94, 0.82), intensity 1.6, soft shadows.
- **No scene Volume.** The URP global default profile has **Tonemapping mode 0 (None)** with HDR enabled. HDR is not tonemapped, so tints above 1 (the path material) and sunlit patches clip.
- `PC_RPAsset`:
  - shadow distance **50 m**, 4 cascades, 2048 shadow map;
  - MSAA 1 (off), render scale 1;
  - SSAO renderer feature present on `PC_Renderer`.
- Enlarged80 views across the 80 m stand exceed the shadow distance. Distant stems lose shadows, and without fog nothing hides the transition.
- Recorded performance baseline (Overview, Editor only, GTX 980M / i5-6300HQ): Enlarged80 rendered frame median **~40 ms** (Legacy40 ~24 ms). There is **little headroom**. Any ground upgrade must be budgeted against this.

## 4. Target quality bar (DRAFT — proposed, not accepted)

"Parity" means **every ground-level element seen from the player's 1.7 m eye height meets the same bar**. No single element should look clearly more primitive than its neighbours, the current track being the main example.

| ID | Criterion | Measurable check |
|---|---|---|
| Q-1 Material fidelity | Every walkable ground surface (forest floor, main track, approach, headland/feeder, clearing) uses a material with base colour + normal (+ optional mask/AO) at a declared texel density; **no HDR (>1) albedo tints**. | Material audit list; texel density within ±25 % across surfaces that meet. |
| Q-2 Surface distinction | Forest floor and track/headland are **different texture sets** that a player can tell apart in greyscale. | Greyscale capture at poses C1/C4 shows the track as a distinct band. |
| Q-3 Edge transitions | No straight-cut or perfect-circle edge where a surface meets the forest floor. A blended band 0.3–1.0 m wide (texture blend and/or verge dressing) runs along the track, headland, feeder, pads and clearing. | Visual review at C2/C3/C6; no single-pixel boundary line at 1080p. |
| Q-4 Continuity | Track → approach → feeder pad → feeder → headland read as one constructed network, with no texture-scale jumps or orientation flips at joins. | C4/C5 captures at the junction. |
| Q-5 Vegetation placement | No authoritative-state vegetation instance whose footprint centre lies on a track, headland, feeder, pad or clearing surface. Edge-preferring placement is allowed only as presentation of the same site budget. | Automated count = 0 on-surface instances. |
| Q-6 Ground cover readability | Ground cover forms **natural clusters**, not a cell-centred grid. Visible density stays monotone with authoritative strength (D-020). | Overhead capture shows no 5 m grid; strength-ordering assertion still passes. |
| Q-7 Lighting cohesion | No unlit ground-level material. Ground, track, vegetation and props share one exposure and tonemap. Distance haze/fog hides the shadow-distance falloff and the boundary void at eye level. | Material shader audit; C1 capture shows no shadow pop-line or grey void at eye level. |
| Q-8 Small props | A non-causal baseline floor dressing (needle litter, twigs, cones, small stones near the track) is present at low density. It **does not** imply ecological state (D-020): no ecological meaning, and it is never counted by habitat assertions. | Dressing listed as presentation-only in code comments; habitat assertions unchanged. |
| Q-9 Performance | Enlarged80 eye-level frame median ≤ **baseline + 15 %**, on the same Editor, hardware and pose as the before capture. Texture memory added is within the approved D-032 budget. | Before/after profile at C1 and C6. |
| Q-10 Non-regression | Simulation hashes, save bytes, tree positions for a new game, Stand Map path lines, coverage corridor, Legacy40 world and Reference Future are **unchanged**. | Existing anchor gates, plus a tree-position and map-line equality check. |

Proposed review poses (Enlarged80, Year 0, fixed seed, fixed sun; eye height 1.7 m; 1920 × 1080 and 1280 × 720; HUD hidden). Coordinates derive from the scene and headland constants above; **adjust if the poses do not frame the subject**:

- **C1** spawn (−0.4, 5.25) looking south along the track.
- **C2** (−2.8, 4.0) looking north into the clearing edge.
- **C3** on the track at z −14.75 (the join between authored track and approach extension) looking south.
- **C4** (−2.44, −23.0) looking south-west at the feeder junction pad.
- **C5** on the feeder (−12.0, −27.4) looking east to the junction.
- **C6** in the stand 6 m west of the track at z −5, looking east at the track edge from the forest.
- Plus one documentation oblique overhead of the corridor.

## 5. Worst current gaps (ranked by player impact × confidence)

1. **G-1 Main track / clearing presentation.** These are the same soil texture as the floor with an overbright orange HDR tint and no normal map. Edges are hard-cut. Plank-wise UV stretch is likely. This is the most-seen surface in every play session and the clearest "prototype" signal (captures #362, smoke 2026-10-17).
2. **G-2 No path-edge transition.** Floor and track meet in a single line, and nothing (verge, litter, grass strip) grades between them.
3. **G-3 Vegetation ignores constructed surfaces.** Ferns, bracken and grass can sit on the track or headland (code-confirmed; seen in captures). This also undermines the readability of the access network the lessons teach.
4. **G-4 Lighting/post foundation.** No tonemapping, no fog, default skybox, unlit fallback vegetation. Colour clips and nothing is graded with distance. Every other improvement is judged under this, so it is cheap and high-leverage. It is also global, which makes it a decision (Q3).
5. **G-5 Bare Year-0 forest floor.** One untextured-normal soil map shows obvious 2.5 m repeats at mid distance. There is no needle-litter character for a closed Sitka stand.
6. **G-6 Sparse, gridded ground cover.** At most 4 instances per class per 25 m², clustered on cell centres.
7. **G-7 Network continuity.** Planks, headland mesh and clearing disc use mismatched UV schemes, so the feeder junction (already a #361 comprehension complaint) is also visually discontinuous.
8. **G-8 Boundary and horizon.** Grey cube ridges, flat void and default sky are visible whenever sight lines open: the approach cutting, gaps and elevated views.
9. **G-9 Flat ground.** No micro-relief. It is noticeable, but expensive and risky to fix because every placement assumes y = 0.

## 6. Priority-ordered backlog (proposal)

| Pri | ID | Item | Gaps | Main risk / coupling | Size |
|---|---|---|---|---|---|
| P0 | GP-01 | **Lighting/colour foundation.** Scenario One Volume profile with Neutral or ACES tonemapping, mild exponential fog/haze tuned to hide the 50 m shadow edge, exposure set; remove HDR albedo tints. Use a runtime-added Volume or a new profile asset (no ForestTest scene edit if avoidable). | G-4 | Global look change: all existing screenshot evidence and UI contrast checks shift. **Owner decision Q3.** | S |
| P0 | GP-02 | **Lit fallback vegetation.** Switch `ScenarioHabitatVisuals` procedural meshes from URP Unlit to URP Lit (or Simple Lit) with matched colours. | G-4 | Small source change; habitat vertex counts unchanged. | XS |
| P0 | GP-03 | **Surface exclusion.** Suppress or relocate any vegetation site whose footprint falls on the track, approach, headland, feeder, pads or clearing. Reuse the existing corridor definitions (`ForestStartingStand.PathCorridorRadiusMeters`, `ScenarioOneHeadlandGeometry`). | G-3 | Presentation-only, but moves `ScenarioHabitatPresentationVerification` counts. **Decision Q2**: suppress or relocate. | S |
| P1 | GP-04 | **Separate ground and track material sets.** Forest floor: needle-litter/soil base + normal + macro-variation (second-scale detail or world-space blend) to break the 2.5 m repeat. Track: surface chosen by owner/research (Q1) + normal. Remove the shared texture. | G-1, G-5 | New binary textures: **D-032 footprint report and approval (Q4)**; source and licence provenance required. | M |
| P1 | GP-05 | **Continuous track/network surface.** One runtime presentation mesh per network (track + approach; headland + feeder + pads; clearing). It is generated from the existing plank transforms and headland constants, uses world-space UVs at one texel density and has edge geometry or vertex-alpha for soft blending. Plank **renderers** are hidden; plank **GameObjects, names, transforms and active state stay** for all readers in §3.2. | G-1, G-2, G-7 | Highest technical risk: must prove Stand Map, starting stand, coverage and prescription readers are unchanged; Legacy40 must stay byte-identical in behaviour; old saves without the approach (`SaveHasApproach`) must reconcile. | M–L |
| P1 | GP-06 | **Edge/verge dressing.** Blended band plus sparse verge props (needle drift, twigs, small stones, edge grass where light permits) along network edges. Presentation-only and deterministic from geometry, not from the save. | G-2 | D-020 wording: verge grass must not imply an ecological state the model lacks. Low density only, or tie it to the cell's grass strength. | M |
| P2 | GP-07 | **Clustered ground cover.** Keep the per-site budget from state, but render each site as a natural cluster (several small instances, offsets around a jittered anchor, not the cell centre). Use GPU instancing. | G-6 | Must preserve monotone strength→density (D-020). Performance-sensitive. **Decision Q2.** | M |
| P2 | GP-08 | **Baseline floor props.** Low-density non-causal Sitka floor dressing (cones, twig litter, needle clumps). Flagged presentation-only and excluded from habitat assertions. | G-5 | D-020 boundary; asset sourcing/licence. | S–M |
| P2 | GP-09 | **Boundary/horizon backdrop.** Replace the visible void with a distant tree-line/backdrop and a sky that matches the GP-01 lighting. The ridges stay as colliders. | G-8 | Conflicts with the current design comment "no decorative forest outside the ecology bounds": **decision Q5.** | M |
| P3 | GP-10 | **Ground micro-relief.** Displaced mesh or Terrain. | G-9 | Affects every y = 0 placement, colliders, player movement, saves, map. Separate spec; not before demo-critical items. | L |
| P3 | GP-11 | **Performance/LOD pass.** Vegetation LOD/cull distances, shadow-casting policy for small ground props, SSAO cost review. | Q-9 | Run continuously; formal pass after GP-07. | S–M |

## 7. Recommended benchmark vertical slice

**Slice:** the *spawn-to-feeder corridor* in Enlarged80. It runs from the Forest Clearing (−2.8, 11) south along the authored main track and its approach extension to the feeder junction pad (−2.44, −27.4), plus the first ~10 m of the feeder westward. About 40 m × 12 m, covered by poses C1–C6.

**Why this area:**

- It is the first thing every player sees, from spawn and the clearing.
- It contains every in-scope element type: clearing edge, authored track, the runtime extension join (z −14.75), feeder pad junction, headland-type surface, forest-floor edges and understorey near paths.
- It is the junction the player criticised in #361, so any visual improvement is directly testable against real feedback. It does **not** change topology (D-071/D-072) or pre-empt the #362 topology decision.

**In scope for the slice:** GP-01 (if Q3 approved), GP-02, GP-03, GP-04, GP-05 and GP-06, applied to the corridor networks. GP-05 necessarily covers the entire track mesh, but review and acceptance are scored on the corridor poses.

**Out of scope:** GP-07 to GP-11, trees and canopy, UI, map, rack visuals and markers, any geometry or topology change, any save field, Legacy40 visual change (unless Q6 decides otherwise), Reference Future, Windows build.

**Slice acceptance (proposed G2):**

1. Q-1 to Q-7 and Q-9 to Q-10 pass at C1–C6 at both resolutions, with before and after captures from the same build configuration. Q-8 is only required if GP-06 props are in.
2. Automated:
   - on-surface vegetation count = 0;
   - new-game tree positions identical to the before state;
   - Stand Map path polylines identical;
   - coverage corridor identical;
   - all frozen anchors and the Legacy40 regression unchanged;
   - a pre-approach Enlarged80 save loads without the approach (reconcile path).
3. `ScenarioHabitatPresentationVerification` is updated in scope. Changed expectations are listed with reasons, and none are deleted silently.
4. Profile: frame median ≤ baseline + 15 % at C1 and C6.
5. Owner rendered review PASS. **This is the acceptance gate; agent captures are evidence only.**

**Proposed later lane:** `feature/scenario-one-graphics-parity-vertical-slice-20261116-v1`. It needs Unity, a local worktree with its own Library, the machine-wide Unity lock, and isolated save and preferences (per the #553 physical-board and lease gates). Routing per AgentWorkflow: **SOL/CODEX** or **CLAUDE CLOUD VALUE** with a local Unity runner. An independent non-author review is needed before integration.

**Likely files (indicative; the packet must narrow them):**

- New presentation component(s), for example `Assets/ForestPrototype/ScenarioOne/ScenarioOneGroundPresentation.cs`.
- New material/texture folder, for example `Assets/ForestPrototype/Art/ScenarioOneGround/`.
- `ScenarioOne/ScenarioHabitatVisuals.cs` (GP-02/03).
- Possibly `ScenarioOneHeadlandPresentation.cs` (hide or replace its surface).
- `ScenarioHabitatPresentationVerification.cs`.
- A new Volume profile asset.
- **Avoid editing `ForestTest.unity`, the shared `Forest Ground.mat`/`Forest Path.mat` and the URP global default profile.** Those are high-conflict, shared with Legacy40 and the frozen-reference world, and single-writer. Prefer runtime application gated to Scenario One Enlarged80, following the existing `StandWorldBoundary` pattern.

## 8. Decisions needed (owner / Manager)

- **Q1 — Track surface identity.** What is the main track physically: compacted earth, stone-surfaced forest road, or grass-centred extraction track? Is the headland/feeder the same or different? **Research needed:** Irish conifer-plantation track/ride and headland surface references (first-party photography preferred, per Game Brief). This worker does not assert which is typical.
- **Q2 — Vegetation presentation latitude.** May presentation move a state-derived site off a constructed surface (relocate) or must it drop it (suppress)? May one site render as a multi-instance cluster? Both keep D-020 monotonicity, but they change visible density.
- **Q3 — Global look change.** Approve Scenario One tonemapping, fog and a sky change? This shifts all future captures and may affect HUD contrast checks.
- **Q4 — Binary footprint (D-032).** Approve a budget for new ground textures. The proposed starting point is ≤ 4 texture sets at ≤ 2048², roughly ≤ 60 MB on disk, to be reported precisely before commit. Source and licence provenance are required. Locally licensed, git-ignored packs (for example Ultimate Nature) must **not** become a production dependency without approval.
- **Q5 — Outside-boundary backdrop.** Is decorative, non-simulated scenery beyond the property allowed? This would reverse the current design comment in `StandWorldBoundary`.
- **Q6 — Legacy40 scope.** Enlarged80 only (recommended: smaller blast radius, and Legacy40 is historical/prototype per D-056), or both geometries?
- **Q7 — Spec route.** Does the Manager accept this file as DRAFT spec input and request a 4-file `specs/scenario-one-ground-graphics-parity-v1/` set, or classify the slice as PACKET ONLY?

## 9. Rollout plan (concise)

| Phase | Output | Owner / tools | Gate |
|---|---|---|---|
| 0 — this doc | Audit, bar, backlog, slice, questions | Docs worker (done) | Manager read-back |
| 1 — decide | Q1–Q7 answered; classification; if SPEC REQUIRED, a 4-file spec approved | Owner + Manager (FREE HERE); DEEP RESEARCH for Q1 | Spec APPROVED, or packet issued |
| 2 — baseline | Before captures at C1–C6 × 2 resolutions + profile, same seed and configuration | Unity worker, Tier 1, under the Unity lock | Evidence under `/media/jer/Files/CCF/Verification/…` |
| 3 — slice build | GP-01..06 in the corridor on the feature lane | SOL/CODEX or Claude Cloud + local Unity, Tier 1 iterations | Author self-check against §7 |
| 4 — qualify | Tier 2: §7 acceptance 1–4, after captures, regression anchors | Unity worker | G2 evidence + GitHub handoff |
| 5 — review | Independent non-author review + **owner rendered review** | Reviewer + owner | APPROVED / NEEDS CHANGES |
| 6 — extend | GP-07..09 across the full stand, then GP-11; GP-10 only via a separate spec | New packets | Per-item Tier 1/2; Tier 3 only at the next integrated candidate |

Sequencing constraints:

- Unity work is serialized machine-wide with the active save orchestration (#544/#563) and any personal play-test lease.
- No overlap with the frozen #443 files.
- No main merge without integration gates.

## 10. Standard worker handoff

```text
IMPLEMENTATION HANDOFF
Task: Sen-1 Scenario One ground-level graphics parity audit and spec (docs-only)
Worker: Claude Code, Opus 5.5
Role: Primary (docs)
Branch: docs/sen-1-graphics-parity-audit-20261116-v1
Base: 660492c62682f1ab68793ef68fb45bfd6cba2965
Context commit: 7801d9d4f6163f7d196c7d28f6b34a9e7876cf0d
HEAD: (commit adding this file; see branch)
GITHUB HANDOFF PATH: Docs/Handoffs/ScenarioOne-Graphics/Sen-1-SCENARIO-ONE-GRAPHICS-PARITY-AUDIT-AND-SPEC-HANDOFF.md
EXTERNAL EVIDENCE ARCHIVE: none created; cited historical captures listed in §2
WHAT CHANGED: one new Markdown file
FILES CHANGED: this file only
SERIALIZED UNITY FILES TOUCHED: none (read-only parsing)
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none
MUST-NOT-CHANGE HASHES / ANCHORS: all untouched
VERIFICATION RUN: source/serialized read-only inspection; review of 5 existing captures
VERIFICATION RESULTS: findings in §3; Unity NOT RUN; no fresh captures; E80-specific
  on-track vegetation and plank UV stretch are code-derived/likely, to be confirmed in Phase 2
REMAINING VERIFICATION GATES: Phase 2 baseline captures; all slice gates in §7
KNOWN LIMITATIONS: captures are from older builds and one 30 m research arena; perf baseline
  is the Overview's Editor figure, not re-measured
UNRELATED WORK PRESERVED: yes; separate worktree; CCF-Primary dirty AGENTS.md /
  StorageArchitecture.md untouched
DECISIONS NEEDED: Q1–Q7 (§8)
Ready for review: Yes (docs)
RECOMMENDED NEXT STEP: Manager answers Q3/Q6/Q7 and routes Q1 research; then Phase 2 baseline
```

Dashboard/Drive synchronization: **NOT VERIFIED**. This worker has no dashboard tooling.
