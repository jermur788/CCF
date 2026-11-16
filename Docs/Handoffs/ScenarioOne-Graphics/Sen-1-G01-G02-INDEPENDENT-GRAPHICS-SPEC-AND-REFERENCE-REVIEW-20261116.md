# Sen-1 G-01/G-02 — Independent graphics-spec and Irish forest-track reference review

**VERDICT: NEEDS CHANGES (DRAFT QUALITY).** 0 BLOCKER · 7 MAJOR · 9 MINOR · 2 NIT. This is an independent DRAFT-quality review. It is **not** product approval, visual acceptance, a Tier 2 result or an implementation release. **Unity NOT RUN.**

The four-file DRAFT is honest about its evidence limits and keeps Q1, Q3, Q4 and Q5 with the owner. The corrections below are small enough for Manager #424 to apply directly as docs-only edits. The two most important are M-1 (one proposed change would silently alter clearance gameplay) and M-4 (the lighting mechanism may not work as written).

| Field | Value |
|---|---|
| Task | Sen-1 G-01/G-02, execution claim [#573](https://github.com/jermur788/CCF/issues/573), parent Manager [#424](https://github.com/jermur788/CCF/issues/424) |
| Actual worker | Claude Code (Anthropic), **Claude Sonnet 5.5** (`claude-sonnet-5-5`). Effort level is not exposed to the session and is not asserted here. |
| Independence | Not the OpenAI GPT-6 Manager who wrote the DRAFT. Not the Claude Opus 5.5 session that wrote the source audit. Same provider family as the audit author, no shared context. |
| Tools actually used | Local shell, git, `gh`, `pdftotext`, WebFetch/WebSearch, image viewing. No Unity, no Editor, no save/Prefs access, no GitHub write other than #573 comments and this branch. |
| Branch / worktree | `review/sen-1-ground-graphics-spec-independent-20261116-v1` at `/media/jer/Files/CCF/Operations/CCF-573-SEN1-GraphicsSpecReview-20261116` (isolated, docs-only) |
| Starting HEAD | `effeebb55727908cc03a91f7d7e6398c5b051dd1` (DRAFT tip). Audit `f23ab2d5`. Main at review `584d383f`. |
| Source tree check | `git diff --stat 660492c6 HEAD -- Assets ProjectSettings Packages` and the same against `origin/main` are both **empty**. The audit's base, the DRAFT tip and main share identical Unity source and settings, so every source reference below applies to all three. |
| Written | Only this file. No spec, source, scene, asset, board, #544, #443, save or Reference file was touched. |

---

## 1. Source-graded audit claims (check 1)

Grades: **SRC** = verified by me in source or serialized files at the DRAFT tip. **HIST** = rests on a historical screenshot or a prior report. **PROV** = provisional extrapolation (code-derived, never rendered). **UNPROVEN** = not established by any evidence.

"DRAFT treats as native?" is **No** for every row. `spec.md:6` and `checklists/requirements.md:3` state that no native Unity run or current before images exist. **No original claim is promoted to measured native Unity.** The one place the DRAFT drops a caveat is the pose list (see m-4).

| # | Audit claim | Grade | Evidence / notes |
|---|---|---|---|
| 1 | Ground is a 40×1×40 primitive cube, scaled to 80 m for E80, texel density kept by a `MaterialPropertyBlock`. No Terrain. | SRC | `ForestTest.unity` Ground `m_LocalScale 40,1,40`, BoxCollider. `StandWorldBoundary.cs:81` (scale), `:127-128` (`_BaseMap_ST`). `grep -c Terrain ForestTest.unity` = 0. |
| 2 | `Forest Ground.mat` and `Forest Path.mat` share one texture (`10e6dd8c…`). Tilings 16 and 3. No normal map. Path tint HDR 1.6/1.4/1.05. | SRC | `Forest Ground.mat:29-30,121`; `Forest Path.mat:29-30,121`; `_BumpMap` empty in both. The tint is set by a menu tool, `ScenarioOneFloorPresentationSetup.cs:57`, which the audit does not list (see M-3). |
| 3 | Track is 52 primitive-cube planks, 2.6 × 0.012 × ≈0.66 m, z −14.75 → +10.75. | SRC | Scene: 52 `Dirt Path NN` objects. Plank 00 scale `2.6, 0.012, 0.662`, mesh fileID 10202 (Cube). I verified plank 00 only. The "0.61–0.66" range is not individually checked. |
| 3a | *(not in audit)* Planks and the clearing have **no collider**, only Transform, MeshFilter and MeshRenderer. | SRC (new) | Parsed scene components. Hiding renderers cannot change physics. This supports the DRAFT's exclusion of colliders. |
| 4 | `Forest Clearing` is a 9 m flat disc. | SRC | Scale `9, 0.008, 9`, mesh 10206 (Cylinder), so diameter 9 m, centre (−2.77, 11). |
| 5 | Headland constants: x −35.4, 2.6 m strip, feeder z −27.4, pads r 2.2, 48 segments, world-planar UVs. | SRC | `ScenarioOneHeadlandGeometry.cs:7-13`; `ScenarioOneHeadlandPresentation.cs:64` (`x / 2.5f`), `:80` (48). |
| 6 | Plank UV ≈ 4:1 anisotropic stretch; headland ≈ 1 repeat per 0.83 m; so scale/orientation cannot match at joins. | PROV | The arithmetic is right (3×3 repeats on a 2.6 × 0.66 face; `x/2.5` × material tiling 3). Never rendered. The DRAFT correctly keeps it as an AC-02 check, not a fact. |
| 7 | Exactly **seven** consumers of the plank GameObjects. | SRC | `ForestStartingStand.cs:330`; `StandMapSource.cs:45`; `ScenarioOneCoverageWorld.cs:9,74`; `ScenarioOneManager.PrescriptionApproval.cs:339`; `ScenarioOneHeadlandPresentation.cs:30`; `StandApproachPath.cs:26,62`; `Editor/ForestSceneBuilder.cs:513`. Renderer dependence is analysed in M-2. |
| 8 | Vegetation: ≤4 per class per cell, `round(strength × 4)`, jitter ±0.42 × cell around cell centre. | SRC, imprecise | `ScenarioHabitatVisuals.cs:202` is `strength < .025 ? 0 : Max(1, RoundToInt(strength × 4))`, so there is a **minimum of 1**. Jitter at `:205-206`. Cells are 5 m, so the jitter region is ±2.1 m. |
| 9 | No exclusion for track, headland or clearing. | SRC | The only skip is `IsVegetationDisplayCleared` (`:114`, `:271`). |
| 10 | Procedural fallback meshes are URP Unlit. | SRC, scope unclear | `ScenarioHabitatVisuals.cs:99`. It applies only where `PrefabFor(...)` returns null. Which classes hit it depends on whether the local Asset Store pack is present (M-7). |
| 11 | Fog off; ambient Trilight; default skybox; one directional light. | SRC | `ForestTest.unity` `m_Fog: 0`, `m_AmbientMode: 1`, skybox fileID 10304. **But** the Main Camera `m_ClearFlags: 2` (Solid Colour, 0.52/0.66/0.77), so the skybox is not drawn (M-4). |
| 12 | Default volume profile Tonemapping mode 0 (None). | SRC, effect UNPROVEN | `DefaultVolumeProfile.asset` Tonemapping `m_Value: 0`. The scene's Main Camera has no serialized URP additional camera data (M-4), so whether this profile is applied is unproven. |
| 13 | `PC_RPAsset`: shadow distance 50, MSAA off, render scale 1. SSAO present. | SRC | `PC_RPAsset.asset:26-29,57`; `PC_Renderer.asset:71`. `QualitySettings.asset` references **two** RP assets (lines 51 and 105). Record the active tier at baseline (m-5). |
| 14 | E80 frame median ≈40 ms, Legacy40 ≈24 ms. | HIST | `unity-project-overview.md:345`, Editor only, GTX 980M. Both the audit and the DRAFT correctly refuse it as a baseline. |
| 15 | Five historical captures exist. | SRC (files) / HIST (content) | All five exist under `/media/jer/Files/CCF/Verification/`. I viewed two. `#362 original-hud…png` shows a uniform warm-orange track with tufts on its edge. `ScenarioOneLinuxPlayerSmoke…/loaded-forest.png` shows a large orange-brown disc with ferns standing on it. The smoke date (17 Oct) is after D-056 (10 Oct), so it may be E80; the build is unrecorded. Neither is a controlled E80 baseline. |
| 16 | The 30 m arena images are not E80. | HIST | The audit and DRAFT both say so (`spec.md:6,33`; plan G-03). |
| 17 | Plants sit on the track or clearing at E80. | HIST + PROV | Seen in two older captures. Code allows it. `unity-project-overview.md:199` records "grass appeared only at the bright clearing/road edge", which is also consistent. Not confirmed at an E80 pose. |
| 18 | "Little headroom" in frame time. | UNPROVEN | An inference from a 40 ms Editor median. Not measured under any budget. |
| 19 | Poses C1–C6 coordinates. | PROV | The audit itself says "adjust if the poses do not frame the subject". Spawn position not verified by me. |

No substitute 30 m arena photo is used as an E80 baseline anywhere in the four files (AC-01, plan G-03, GR-05).

---

## 2. Findings

Severity: **BLOCKER** stops approval; **MAJOR** must be fixed before the spec can be APPROVED; **MINOR** should be fixed; **NIT** is cosmetic. "Fix" names a concrete docs-only change.

### MAJOR

**M-1 — The planned vegetation relocation/suppression sits in a function that also drives clearance gameplay.**
Evidence: the audit (§6 GP-03) and DRAFT (GP-03, plan G-05) treat the change as presentation-only and put the work in `ScenarioHabitatVisuals.cs`. But `ScenarioHabitatVisuals.VegetationSites(...)` (`:187`) is also called by `ScenarioOneClearance.cs:53-56`, which builds `ClearanceTargets.GroundPlants`. `ClearancePreview.cs:45` pins those positions for the player. `VegetationClearance.cs:42-48` uses the count in `HasTargets` and `Summary` ("N ground patches").
Failure scenario: a worker adds the surface mask or relocation inside `VegetationSites`. A spot-clearing preview in the spawn clearing then shows fewer or moved "ground patches", and `HasTargets` or `Summary` text changes. That is a player-visible gameplay change that none of AC-03/AC-04/GR-10/GR-11 would catch. They protect "tree IDs, Stand Map, coverage, prescriptions" but never `GroundPlants` or the clearance preview.
Fix: add to GP-03 and plan G-05 that the mask or relocation **must be applied only in the display loop** (`ScenarioHabitatVisuals.cs:111-135` and `PlaceRushDressing :269`), leaving `VegetationSites` output unchanged. Add an AC/GR item: `ClearanceTargets.GroundPlants` count and positions are identical before and after for a fixed spot and area footprint, and the clearance-preview summary string is unchanged.

**M-2 — The "hide plank renderers" mechanism has concrete dependencies the DRAFT leaves as "not assumed safe".**
The packet asked for a recheck of the possible renderer dependence of the 7 consumers. Result:

| Consumer | Reads | Renderer/material dependence |
|---|---|---|
| `ForestStartingStand.CollectClearancePoints` (`:326-335`) | position, **active state** (`FindObjectsInactive.Exclude`) | None. `SetActive(false)` would break it. |
| `StandMapSource` (`:45`, `:369`), `ScenarioOneCoverageWorld` (`:9,74`), `PrescriptionApproval` (`:339`) | name, position, active | None. |
| `ScenarioOneHeadlandPresentation.CreateSurface` (`:30-31,57`) | `GameObject.Find("Dirt Path 00")`, requires `GetComponent<Renderer>() != null`, **copies `sharedMaterial`** | **Yes.** Destroying the renderer makes it return null (no headland). Swapping the template's material re-skins the headland. |
| `StandApproachPath` (`:26,62`) | `Instantiate(template)` for every runtime plank | **Yes.** Clones inherit renderer enabled state and material. Hiding before the clone hides the whole approach. It also runs on every save-load reconcile. |
| `Editor/ForestSceneBuilder.cs:513` | root name and position | None (Editor only). |

The protected set is wider than "52". `StartsWith("Dirt Path")` also matches the runtime `Dirt Path X###` clones. `Approach Track ###` clones and the cutting also derive from the template.
Fix: replace the hypothesis sentence in GP-05, plan G-06 and GR-11 with a requirement list: (a) hide via `Renderer.enabled = false` only, never `SetActive` and never Destroy; (b) define hide order relative to the `StandApproachPath` clone and the headland `CreateSurface`; (c) do not change the template's `sharedMaterial` (see M-3); (d) rebuild the continuous mesh when `StandApproachPath.Version` changes (`:33,71,204`), when `RackPlanningV2Active` toggles, and on Legacy40 load; (e) extend the protected-object set to the 52 authored objects **plus** `Dirt Path X###`, `Approach Track ###` and the cutting.

**M-3 — Shared material assets and an Editor tool that regenerates them are unprotected.**
Evidence: the audit §7 says to avoid editing `ForestTest.unity`, the shared `Forest Ground.mat` / `Forest Path.mat` and the URP default profile. The DRAFT keeps only the URP and scene part (GR-13, plan G-04). It never forbids editing the two `.mat` files.
Why it matters: both materials are shared with Legacy40 (same scene), and `Headland` takes `sharedMaterial` from the plank (M-2). `ScenarioOneFloorPresentationSetup.BuildAndWire` (`:21`) and `.TextureTrack` (`:49-57`) are menu tools that **rewrite these two materials** (HDR tint, tilings). An in-place edit would be overwritten by anyone running the menu and would change Legacy40 and the headland silently. The audit did not list this tool among its couplings.
Fix: add to GP-04 and GR-13: "E80 ground and track use new material instances or assets. `Forest Ground.mat` and `Forest Path.mat` are not edited, and the Editor setup tool is not changed or run." Add a Legacy40 before/after render or material-hash check to G-07.

**M-4 — The GP-01 lighting mechanism is unproven and may not do what the DRAFT assumes.**
Evidence: the ForestTest Main Camera GameObject has only Transform, Camera and AudioListener. **No URP additional camera data is serialized.** Its `m_ClearFlags` is 2 (Solid Colour).
Consequences, to be proven or refuted in Unity (my reading of URP defaults, not verified here):
1. A camera without additional data normally runs with post-processing off. In that case "Tonemapping None" in the default volume is moot, and a runtime Volume has no effect until a camera flag is enabled. That is a camera change shared with Legacy40.
2. With Solid Colour clear, the default skybox is not drawn, so a "sky grade" via skybox does nothing without changing the clear flags. The audit's "default-skybox horizon" description is source-backed for the skybox asset but misdescribes what the player sees.
3. `RenderSettings.fog` and ambient are scene-global statics, so "E80-only" fog needs set and restore on Legacy40 load and on scene reload.
GP-01 ("compare … do not change global URP defaults or the shared ForestTest scene") may therefore be unsatisfiable as written.
Fix: add a G-04 feasibility sub-gate that records whether the camera flag, clear flags or `RenderSettings` must change, and states that these count as global blast radius under Q3. Require an E80-only gate with explicit restore. The Q3 owner brief should say that "tone, fog and sky" includes camera-level changes.

**M-5 — Suppression cannot honestly keep "strength ordering" and "counts", and cells with no free area exist.**
The packet asked whether relocation inside the same cell can keep ecological strength ordering in cells with no free area. It cannot, and I measured how often that happens.
Offline census (Appendix A; no Unity): the authored surfaces are the 52 planks as oriented rectangles, the 9 m clearing disc, headland strip, feeder and pads, rasterised at 0.1 m over the 16 × 16 grid of 5 m cells. Runtime approach planks (z < −14.75) are **excluded**, so these are lower bounds.

| Margin around surfaces | Cells touched (of 256) | ≥50% covered | ≥90% covered | Candidate region (centre ±2.1 m) ≥90% covered |
|---|---|---|---|---|
| 0 m | 53 | 11 | **1** | 1 |
| 0.6 m | 59 | 15 | **3** | 9 |
| 1.5 m | 75 | 37 | **11** | 11 |

The single fully covered cell at 0 m is centred (−2.5, 12.5), the spawn clearing, directly in the C1/C2 views. Under the 1.5 m margin the feeder row of cells at z = −27.5 (x from −32.5 to −7.5, at least six cells) is fully covered. Overview line 199 records Year-0 grass "only at the bright clearing/road edge", so the state-derived instances are concentrated in exactly the cells that will be suppressed.
Two inconsistencies follow:
- plan G-05 says "preserve strength ordering, gameplay/ecology counts and verifiers", while AC-03 says "update focused verification assertions when visual anchors differ". `ScenarioHabitatPresentationVerification.cs:186-223` asserts that Year-0 grass instances equal `GrassPatchCount` and are supported per cell, so rendered counts will move.
- The anchor is a point, but footprints are not. Moss cushions are offset up to ±0.9 m with scale up to ≈1.15 (`:117-128`), and `RushSites` are fixed authored anchors, so "within the same cell" is undefined for rush.

Fix: state the invariant at **state level** (`VegetationSites` output unchanged, see M-1). State that **rendered** per-cell order is not guaranteed in cells touching constructed surfaces. Require G-05 to report per cell and class: state-derived n, rendered n, suppressed n, and the cell list. Require a per-class footprint radius for the mask. Require deterministic relocation reusing the existing `Jitter(cell, kind, plant, axis)` hash (`:361`), not `UnityEngine.Random`, so visuals do not change across save and load. Define rush behaviour. Rewrite the plan G-05 clause so it does not contradict AC-03. Add the census numbers (or a re-run) as G-03 input.

**M-6 — Unity and task gating is internally inconsistent.**
`spec.md:43` says source or Unity work waits for an APPROVED spec. `tasks.md` G-03 (local Unity baseline) has the gate "Real clean/owned lane and machine lease", with no spec-approval gate. Only G-04 lists G-02 as a gate (G-05 and G-06 list no spec gate), and G-02's own output is "Published APPROVED spec if genuinely accepted". A reviewer cannot approve a spec, and a review being finished is not the same as approval. `tasks.md` says "Only G-00 and drafting may progress before spec approval" while also dispatching G-03 (Unity). `AgentWorkflow.md:99` says only APPROVED or later status authorises implementation.
Fix: add to every Unity or source task (G-03..G-08) the gate "spec.md status APPROVED by Manager, with Q-decisions recorded where needed". Say explicitly whether the G-03 before-baseline needs approval, or is allowed under DRAFT as a read-only capture with a named lease. Reword G-02's output as "independent review verdict". Manager approval is a separate event.

**M-7 — Asset provenance and baseline reproducibility depend on a git-ignored local Asset Store pack.**
Evidence: GP-04 says "use existing licensed repo assets". The ground and understorey visuals come from *Ultimate Nature – Starter*, which is **not in the repository**. `.gitignore:82-86` ignores `/Assets/InnerverseInteractive/`, and `UltimateNatureSetup.md` says it is a locally acquired dependency that must never be committed. The audit warned about this in Q4. The DRAFT dropped the warning.
Consequences: (a) "existing repo assets" is ambiguous, and the only in-repo ground-detail art is `Art/ForestryGround`; (b) a G-03 baseline taken on a machine without the pack would show Unlit fallback plants and different cover, so GP-02's scope and AC-03's counts would not be comparable to a later machine that has it; (c) `unity-project-overview.md:199` already notes that the rendered smoke "used … Ultimate Nature present locally".
Fix: add to GP-04: "existing in-repo assets only, locally acquired packs are not production dependencies for new ground art". Require the G-03 evidence to record whether the pack is present, its GUID checklist result, and which habitat classes rendered via prefab versus Unlit fallback. State that the before and after captures must use the same pack state.

### MINOR

**m-1 — Q1: the compacted-earth lead is a labelled hypothesis (correct), but the evidence does not favour it over a stone road.** See §3.
- The DRAFT labels it an "evidence-informed aesthetic option … not a verified diagnosis" (`spec.md:29`). That is correct and sufficient for Q1 OPEN.
- Cosmetic gap: "with modest stones" is not supported for tracks. Forest Research says tracks are "usually without stone reinforcing" and COFORD says "normally only unsurfaced formations". Stone appears only on locally strengthened sections.
- COFORD places engineered tracks mainly on mountainous or hill-slope ground with good bearing capacity. The Scenario One property is flat plantation, so the mapping of the game's main track to "engineered track" is itself a hypothesis.
- Fix: present A/B/C of §3 with equal standing in G-00, and drop "modest stones" or mark it as strengthened sections.

**m-2 — The Geograph reference is unverified and cannot be a production source.** WebFetch returned HTTP 403, so I could not confirm that photo 3441512 is a conifer-track image. Geograph photos are generally CC BY-SA 2.0, and the ShareAlike clause would encumber a texture derived from one. The COFORD manual is "© COFORD 2004 … all rights reserved". Fix: mark the link "reference only, subject and licence not verified by the Manager or the reviewer". Keep it out of any import path.

**m-3 — D-045 accepted the current track texture; the DRAFT calls it a defect without citing that.** `decision-log.md` D-045 records "Final presentation acceptance PASS … (ten-state rendered review, track texture, HUD readability)". The HDR tint was a deliberate choice: `ScenarioOneFloorPresentationSetup.cs:54-57` says "lighter tint to keep access distinct from the dark needle-litter floor". `spec.md:6` calls D-045 a "historical baseline" only. Removing the tint reopens the access-versus-floor distinction it solved. Fix: add D-045 to the references with one line stating the owner's Q1 and GR-02 choice supersedes the accepted track look. Reserve a decision-log entry for the V1 graphics decisions. AC-02's greyscale-distinct test is the right guard and should stay.

**m-4 — Poses and baseline state are under-specified.**
- `spec.md:11` calls C1–C6 "fixed source-audit poses". The audit calls them provisional and says to adjust them. Fix: G-03 may correct the poses once, record the final coordinates, and freeze them before the before-capture.
- The audit renders the poses with HUD hidden. AC-06 needs HUD-on variants at C1 and C4 and a state with rack marks, or readability is never tested. In `#362` the orange and blue world marks sit near the orange ground, so tint and tonemap changes interact.
- The feeder and headland exist only when `RackPlanningV2Active` (E80 and `rackPlanningModel == V2`, `ScenarioOneManager.RackPlan.cs:37-40`). AC-01 says only "E80 Year 0". Record the rack-planning model, objective state, seed, sun, pack presence (M-7) and active quality tier and RP asset.

**m-5 — Performance acceptance lacks a method.** AC-05, GR-15 and plan G-07 are right on principle (fresh, same machine, +15%). Missing: warm-up and sample count, repeat runs to measure baseline noise, whether HUD is on, Editor versus Player, vsync, and the quality tier. The +15% cap applies at C1/C6 only, but fill-rate cost differs by resolution, so state the cap at both 720p and 1080p, plus the oblique. Add frame p95 or hitches, load-time (the E80 new-game baseline is 1.8 s, Overview line 345), and texture memory. If baseline run-to-run noise exceeds about 5%, a 15% cap is weak.

**m-6 — The verifier inventory is incomplete, and "no red tests" cannot be satisfied.** `ScenarioHabitatPresentationVerification.cs` lives under `Assets/ForestPrototype/`. Five disposable harnesses under `Tools/Verification/` also reference habitat visuals: `ClearanceVerification.cs`, `ScenarioOnePresentationReview.cs`, `RackV2FieldMarking/RackV2FieldMarkingVerification.cs`, `UnderstoreyRecruitment/UnderstoreyVisualAudit.cs`, `UnderstoreyRecruitment/JuvenileDisplayHeightVerification.cs`. Plan G-07 requires "no skipped/red tests", but Overview line 331 records an existing open failing gate. Fix: list the six files as the verifier-update scope, and change the requirement to "no new red against the recorded baseline".

**m-7 — Template conformance and traceability.**
- `spec.md` lacks the template header (Spec ID, Owner, Created and Last updated, Decision-log refs, Milestone, Supersedes). It uses GP-xx instead of numbered MUST/SHOULD FR-n. Stop conditions, Open decisions and Save compatibility are inline, not separate sections.
- `tasks.md` is a table, not the T-NN structure (Status, Dependencies, Verification, Stop conditions, Completion state).
- `plan.md` lacks Architecture findings, Affected files, Verification strategy and Risks.
- Three ID schemes (AC-01..07, GR-01..18, GP-01..06) have no mapping. The audit's **G-1..G-9 are gap IDs**, while the DRAFT's **G-00..G-09 are task IDs**, which is a collision.
- Fix: add the header, a short "Stop conditions" and "Open product decisions" section, a GP→AC→GR trace table, and a note that audit G-n means gap and task G-nn means task. This is required before APPROVED under `specs/README.md`. Substance is present, so it is MINOR.

**m-8 — The protected-object definition omits world states.** There are three states: Legacy40; E80 saved before the approach existed (`SaveHasApproach`, `StandApproachPath.cs:112`, reconciled in `ForestSaveController.cs:355`); E80 with the approach, with Rack V2 on or off. The headland and approach exist in only some of them. Fix: add a small state matrix to AC-04 or G-07 (rendered presentation present or absent, protected anchors unchanged) rather than "Legacy40 and pre-approach E80 retain behaviour". No authored scene object, path, terrain or save field may change, which the DRAFT already states.

**m-9 — "Authoritative … geometry" for the mask is ambiguous.** Gameplay corridors differ from rendered surfaces: `PathCorridorRadiusMeters` is 1.5 m, but rendered planks are 2.6 m wide. The clearing's gameplay radius is not its 9 m visual disc (`ScenarioOneCoverageWorld.cs:10`). Fix: state that the mask uses the **rendered footprint plus a per-class margin** (the census margins in M-5 are a starting range), and name the source for each surface.

### NIT

- **n-1** `plan.md` phases G-00..G-09 do not map one to one with `tasks.md` (plan folds G-01/G-02 into G-00).
- **n-2** The headline "18 unchecked gates" is accurate (GR-01..18). Keep it in the verdict wording.

---

## 3. Q1 — Irish and UK forest-access surface references (check 2)

### 3.1 Citations verified by me

I opened the PDFs and read the text with `pdftotext`.
- **COFORD, *Forest Road Manual* (Ryan, Phillips, Ramsay, Dempsey, 2004).** Glossary: an **engineered track** is "an engineered, level, cambered formation (foundation) of 4 m width, capable of supporting harvesters and forwarders without causing rutting". §B.6 (manual p. 36): "These should not be mistaken for silvicultural extraction 'racks'". Guideline 4: "Engineered tracks will normally be only unsurfaced formations". "Generally then tracks are only allowable on solid, hard ground that is prevalent on hill slopes." A later "Tracks" passage (near manual p. 48; I did not confirm its section number) defines the second category as "harvest tracks or racks" (previously undisturbed ground, unplanted clearances, felled rows), and says engineered tracks "do not get a completion metalling". Roads: minimum carriageway 3.4 m on straight sections. Roads receive stone completion (Part C, §C.5 and §C.7, p. 96–99).
- **Forest Research, *Operational Experience of CCF: UK Case Studies*, IPIN 13/06 (Duncan Ireland, Sept 2006), Appendix 1 "Standards of Access" (p. 49).** Four grades:
  - Forest Road: stone surfaced, to FC "category 1 A".
  - Secondary Forest Road: stone surfaced, lower specification.
  - Track: links road and rack networks, lower specification "usually without stone reinforcing", with some earthworks ("removing or compacting topsoil, creating a durable surface").
  - Rack: "the lowest specified access routes … simply unsurfaced corridors", often reinforced with brash.

This is a UK (Forestry Commission) source, not Irish. The DRAFT labels the two sources correctly.
- The Manager's summary is accurate. COFORD's formal "engineered track" and "harvest track/rack" definitions are as described. The FR classes are as described.

### 3.2 Is the compacted-earth main track a labelled visual hypothesis? Yes, with caveats

`spec.md:29` and the Manager verdict both say it is an evidence-informed option, not a statement that the existing 2.6 m geometry matches a standard. That is correct. The geometry should not be described as a standard: 2.6 m is narrower than both the COFORD 3.4 m minimum carriageway and the 4 m engineered-track formation. The DRAFT does not claim otherwise. See m-1 for the caveats.

### 3.3 Low-cost faithful surface alternatives (reference only; nothing imported)

All three keep the existing geometry. None is chosen; the owner chooses under Q1. I did not view the candidate texture pages' images, so suitability for Irish Sitka plantations is **not verified**. Licence summaries come from the sites' own licence pages as returned by search and must be re-read before any import.

| Option | Reads as | Evidence | Free texture candidates (CC0, check before use) |
|---|---|---|---|
| **A. Stone-surfaced secondary forest road** | Grey-brown angular crushed stone, compacted wheel lines, grassed or mossy verge, stone "bell-mouth" at the junction | FR App. 1 "Secondary Forest Road" (p. 49) and the CCF case-study text on stone bell-mouths; COFORD Part C stone completion | Poly Haven [rocky_trail](https://polyhaven.com/a/rocky_trail), [rocks_ground_02](https://polyhaven.com/a/rocks_ground_02); ambientCG gravel category |
| **B. Unsurfaced compacted-earth track** (the Manager's hypothesis) | Worn mineral soil, darker and less saturated than the floor, central wear bands, sparse embedded stones only | COFORD §B.6 guideline 4; FR App. 1 "Track" | Poly Haven [dirt_floor](https://polyhaven.com/a/dirt_floor); ambientCG ground category |
| **C. Brash/needle-reinforced rack-style surface** (suits the headland and feeder, "lower spec") | Dark litter-covered brash corridor with needle drift, no mineral surfacing | FR App. 1 "Rack"; FR Figures 9 and 31 (brash key route and access track) | Poly Haven [forest_ground_04](https://polyhaven.com/a/forest_ground_04), [leaves_forest_ground](https://polyhaven.com/a/leaves_forest_ground) |
| **D. Zero-binary baseline** (fits Q4 default) | A re-tinted version of the existing soil texture in LDR, with separate tiling, world-space UVs and vertex-alpha edges | Q4 "zero unapproved new texture binaries" | None needed; new `.mat` / profile assets only (M-3) |

Primary-source links: [COFORD Forest Road Manual (PDF)](https://limerickandtipperarywoodlandowners.ie/wp-content/uploads/2024/04/ForestRoadManual-1-.pdf); [Forest Research IPIN 13/06 (PDF)](https://cdn.forestresearch.gov.uk/2022/02/ipin_1306_ccf_case_studies.pdf). The best faithful source remains the **owner's own first-party photography** of Irish tracks (Game Brief, *Art, audio and authenticity*).

### 3.4 Licensing before any asset use

- **Geograph photos** (including 3441512): generally CC BY-SA 2.0. Attribution is required, and a derivative must be released under a compatible BY-SA licence. That would contaminate a derived texture. Reference viewing only. I could not open the page (HTTP 403), so the image and its exact licence are unverified.
- **COFORD manual and FR report text, figures and photos:** not offered for reuse. COFORD states all rights reserved. The Forest Research reuse licence was not checked. Use as references only.
- **Poly Haven and ambientCG:** both state CC0 on their licence pages. Download, hash and record the licence URL and date in provenance at the time of import.
- **Any import** still needs a D-032 footprint report and owner approval (Q4), and a tracked provenance entry. Locally acquired Asset Store packs are not a production dependency (M-7).

---

## 4. Remaining packet checks

| Check | Result |
|---|---|
| **3. Non-authoritative illustrative material vs game rules** | The DRAFT keeps appearance separate from game state: Q1 is a look choice, GP-06 says dressing must not misrepresent cover or completed work, and D-020 is cited. Issues found: M-1 (shared function with clearance gameplay), M-2 (renderer and clone coupling), m-8 (world-state matrix), m-9 (mask source). The "7 consumers" are confirmed and enumerated. |
| **4. Same-cell relocation and honest suppression** | Honest suppression is required (`spec.md:18`, GR-09). The count wording needs fixing (M-5). Existing verifier edits are bounded ("retaining their intent", none deleted) but the inventory is incomplete (m-6). |
| **5. Performance, resolutions, nonauthor Tier 2, owner visual gate, lighting scope** | Fresh E80 before, same hardware, +15% at C1/C6, both resolutions, nonauthor Tier 2 and an explicit owner visual gate are present (AC-01/05/07, GR-14..17). Gaps: method (m-5), the mechanism for E80-only lighting (M-4), HUD-on and rack-mark readability captures (m-4). The DRAFT does protect HUD, rack marks and old-world presentation in principle (AC-06, GR-07, GR-12). |
| **6. Lifecycle, consistency, D-032, authority** | Status is correctly DRAFT in all four files. Q1/Q3/Q4/Q5 are owner or deferred (`spec.md:23-29,41`). Q2 is a "Manager drafting default" that changes visible density, so label it "pending owner visual review (G-08)". D-032 is correctly OPEN with the 4 × 2048² / 60 MB figure explicitly not accepted. New file names and paths are indicative, and the DRAFT does not name a Unity lease as granted. Issues: M-6, M-7, m-7. |
| **7. Benchmark focus and footpath scope** | Correct. The benchmark is the spawn clearing, main track and southern feeder junction (`spec.md:11`). "No new footpath topology may be invented" and "visual reconstruction" of the connected path are both stated. Excluded items include topology, 52 objects, racks, terrain and Stand Map. Note the feeder exists only with Rack V2 on E80 (m-4). |

---

## 5. What is right in the DRAFT

- DRAFT status and "NO IMPLEMENTATION AUTHORITY" are stated in every file. No task gets production authority from DRAFT text, apart from the G-03 gating gap (M-6).
- No audit claim is promoted to native-Unity fact. All 18 gates are unchecked.
- E80-only scope with a Legacy40/Reference regression (Q6), consistent with D-056.
- The performance cap is anchored to a fresh same-machine baseline, not the Overview's ≈40 ms.
- Binary budget, exterior scenery and the global look are explicitly deferred to the owner.
- Stop conditions for ownership, uncovered path consumers and >15% performance are present.

---

## 6. Recommended next steps for Manager #424

1. Apply the docs-only corrections to the four files: M-1..M-7 first, then m-1..m-9. All are text changes. No new decision is needed except the owner items below.
2. Re-run a bounded delta check on M-1, M-2, M-3, M-4, M-5 and M-6 after the edits. A different reviewer is not required unless M-4's feasibility changes the Q3 owner brief.
3. Owner items remain open and are unchanged by this review: Q1 surface choice (use §3.3 A/B/C, with D as the zero-binary option), Q3 camera-level lighting scope, Q4 binary budget and licence, Q5 exterior scenery. The owner should also be told that V1 replaces the D-045 accepted track look (m-3).
4. Do not release G-03 until M-6 is resolved. G-03 will need an owned Editor lane, the machine-wide Unity lease and isolated save and Prefs.
5. No Windows build, as already stated by the Manager.

---

## 7. Limits of this review

- **Unity NOT RUN.** Nothing here proves rendered appearance, frame time, URP camera defaults (M-4 reasoning is from URP behaviour as I understand it, not run), or any gameplay outcome.
- I viewed 2 of the 5 historical captures. I did not run the audit's own parsing script. My own parsing and census used Python on the committed `ForestTest.unity`.
- The census (Appendix A) approximates plank footprints as oriented rectangles, the clearing as a 9 m disc and the headland per the published constants. It **excludes the runtime approach planks** and uses assumed margins. It is a sizing estimate, not a measurement of what will render.
- The Geograph page returned HTTP 403, so its content and exact licence are unverified. Texture candidates were not visually inspected.
- Dashboard and Drive synchronisation: **NOT VERIFIED**; this review did not touch either.

```text
REVIEW HANDOFF
Task: Sen-1 G-01/G-02 independent graphics-spec and reference review (#573)
Worker: Claude Code, Claude Sonnet 5.5 (effort not exposed)
Role: Reviewer (independent, docs-only)
Branch: review/sen-1-ground-graphics-spec-independent-20261116-v1
Start HEAD: effeebb55727908cc03a91f7d7e6398c5b051dd1
GITHUB HANDOFF PATH: Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-AND-REFERENCE-REVIEW-20261116.md
FILES CHANGED: this file only
SERIALIZED UNITY FILES TOUCHED: none
VERDICT: NEEDS CHANGES (DRAFT QUALITY) - 0 BLOCKER / 7 MAJOR / 9 MINOR / 2 NIT
UNITY: NOT RUN
```

---

## Appendix A — Offline cell-coverage census script

Run from the repository root as `python3 -I -B census.py Assets/Scenes/ForestTest.unity`. Reads the scene read-only. Output is summarised in M-5.

```python
import re, math, sys
t = open(sys.argv[1]).read()
docs = re.split(r'\n(?=--- !u!)', t)
by = {}
for d in docs:
    m = re.match(r'--- !u!(\d+) &(\d+)', d)
    if m: by[m.group(2)] = (m.group(1), d)
planks = []; clearing = None
for i,(k,d) in by.items():
    if k != '1': continue
    n = re.search(r'\n  m_Name: (.*)', d).group(1)
    if not (re.fullmatch(r'Dirt Path \d\d', n) or n == 'Forest Clearing'): continue
    for c in re.findall(r'component: \{fileID: (\d+)\}', d):
        k2, d2 = by[c]
        if k2 == '4':
            p = list(map(float, re.search(r'm_LocalPosition: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+)\}', d2).groups()))
            q = list(map(float, re.search(r'm_LocalRotation: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+), w: ([-\d.e]+)\}', d2).groups()))
            s = list(map(float, re.search(r'm_LocalScale: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+)\}', d2).groups()))
            yaw = math.atan2(2*(q[3]*q[1]+q[0]*q[2]), 1-2*(q[1]*q[1]+q[2]*q[2]))
            rec = (n, p[0], p[2], yaw, s[0], s[2])
            if n == 'Forest Clearing': clearing = rec
            else: planks.append(rec)
HX=-35.4; W=2.6; PAD=2.2; FZ=-27.4; PX=-2.436166; SZ=-37.7; NZ=37.7
def covered(x, z, buf):
    for (_,px,pz,yaw,sx,sz) in planks:
        dx=x-px; dz=z-pz; c=math.cos(yaw); s=math.sin(yaw)
        if abs(dx*c-dz*s) <= sx/2+buf and abs(dx*s+dz*c) <= sz/2+buf: return True
    _,cx,cz,_,sx,_ = clearing
    if (x-cx)**2+(z-cz)**2 <= (sx/2+buf)**2: return True
    if HX-W/2-buf <= x <= HX+W/2+buf and SZ-buf <= z <= NZ+buf: return True
    if HX-buf <= x <= PX+buf and FZ-W/2-buf <= z <= FZ+W/2+buf: return True
    return any((x-ax)**2+(z-az)**2 <= (PAD+buf)**2 for ax,az in ((HX,SZ),(HX,NZ),(HX,FZ),(PX,FZ)))
for buf in (0.0, 0.6, 1.5):
    cells=[]
    for cj in range(16):
        for ci in range(16):
            x0=-40+ci*5; z0=-40+cj*5; tot=cov=tj=cj_=0
            for a in range(50):
                for b in range(50):
                    x=x0+(a+.5)*.1; z=z0+(b+.5)*.1; h=covered(x,z,buf)
                    tot+=1; cov+=h
                    if abs(x-(x0+2.5))<=2.1 and abs(z-(z0+2.5))<=2.1: tj+=1; cj_+=h
            cells.append((ci,cj,cov/tot,cj_/tj))
    print(buf, sum(c[2]>0 for c in cells), sum(c[2]>=.5 for c in cells), sum(c[2]>=.9 for c in cells), sum(c[3]>=.9 for c in cells))
```
