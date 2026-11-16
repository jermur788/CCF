# Sen-1 — Compact30 Woodland Graphics/Asset V1 — Implementation Plan

- **Spec ID:** SPEC-SEN1-COMPACT-GRAPHICS-V1
- **Spec status:** **DRAFT — NO IMPLEMENTATION AUTHORITY**
- **Plan owner:** Scenario One Manager #424, 2026-11-16
- **Authority:** subordinate to `spec.md`, direct user direction, Project Decision Log/Milestone, signed three-profile save contract and active world/planting specs.
- **Evidence sources:** [#577 Opus first-party inventory and scratch look-dev](https://github.com/jermur788/CCF/blob/docs/sen-1-compact-asset-lookdev-opus55-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-OPUS55-COMPACT-ASSET-PREPRODUCTION-RESULT-20261116.md), [#583 source-only Route A/G1 clearance](https://github.com/jermur788/CCF/blob/docs/sen-1-583-compact-path-g1-clearance-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-583-ROUTE-A-G1-PATH-CLEARANCE-AND-MANIFEST-BOUNDARY-RESULT-20261116.md), [#573 independent E80 graphics finding list](https://github.com/jermur788/CCF/blob/review/sen-1-ground-graphics-spec-independent-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-AND-REFERENCE-REVIEW-20261116.md), [#584 independent compact-doc correction PASS](https://github.com/jermur788/CCF/blob/b2bcb3974650bae429ddd47ccf6bc8c41d560537/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-580-INDEPENDENT-CHANGED-LINES-RECHECK-RESULT-20261116.md).

## Architecture / current-state source findings

| Domain | Source-observed or worker-graded evidence | Implication / caution |
| --- | --- | --- |
| World | Historical `Assets/Scenes/ForestTest.unity`, `StandWorldBoundary`; new 30 m compact playable manifest not created or signed | No path/mesh/collider, new hierarchy or runtime camera baseline can be asserted |
| Ground/path | Shared `Materials/Forest Ground.mat` and `Forest Path.mat`, one base soil texture, different tilings, Editor menu `ScenarioOneFloorPresentationSetup` may rewrite them | Compact needs distinct materials; never mutate shared assets or their editor generator |
| Historical access | #588 source inspection: 32 of 52 root `Dirt Path` planks, **2.6 m wide**, run on the eight Route A waypoint line; another 20 cover H1 approach; `ForestStartingStand`, Stand Map and prescription approval consume old plank identities; `StandApproachPath`/headland clone renderers | **BLOCK GQ-07:** prefer a separately authored Compact scene/hierarchy with no old planks, or independently review a profile-gated `Renderer.enabled`-only exclusion plus gameplay-source reconciliation and exact clone/version/toggle/load lifecycle. Do not treat Compact-only ribbon as sufficient if historic planks still render |
| Ecology/display | `Assets/ForestPrototype/ScenarioOne/ScenarioHabitatVisuals.cs` `VegetationSites` also feeds `ScenarioOneClearance`, `ClearancePreview` and `VegetationClearance` at the same `Assets/ForestPrototype/ScenarioOne/` folder | Display mask remains downstream of generation; preserve actual clear-target and summary IDs/counts, deterministic `Jitter` placement and rush class, and profile-scoped visual+censored verifier accounting |
| Sitka/understorey | #577 source inspected tracked first-party Plantation Sitka 02, fern/bracken/bramble/moss/deadwood, multiple LODs | Reuse but qualify appearance/LOD/perf in actual Compact before relying on source inventory |
| Broadleaf juveniles | `ScenarioHabitatVisuals.PlaceJuveniles` uniform scaling of 2 m sapling to ~0.6 m; `ScenarioProtectionVisuals` upright 1.2 m × 0.2 m diameter cylinder, no active collider, FAILED state tilts 28° and shifts +0.25 m world-x | The current failed visual already intrudes **0.141 m into the 1.2 m O1 walking core**, O2 intrudes 0.155 m with illustrative 0.20 m verge. New real-size transplant and bounded path-aware failed protection visual needed; never change plant growth or browsing mechanics for art |
| Nature pack | `.gitignore` excludes Ultimate Nature; #577 found incomplete local copies of expected prefab GUID set in five local lanes | Never make it a core Compact dependency; state fallback/pack presence in any evidence |
| Camera/URP | Old scene Main Camera `m_ClearFlags: 2` Solid Colour, no serialized additional URP camera data, global `RenderSettings` | Do not assume skybox, tonemapping or fog would work/restore; postpone until isolated feasibility and owner scope |
| Route/G1 | #583 computed raw straight-line G1 source clearances to Route A | 1.2 m core upright-only candidate; minimum **+0.121 m B2** after r=0.10 m upright and v=0.20 m tentative verge, but #588 failed-state O1 is **−0.141 m to core**. 1.8 m B2 only +0.021 m with no verge; 2.4 m puts O1/B2 centres inside path and all 4 upright guards into it. Actual spline/stakes/turning unproven |
| Asset source/provenance | `.blend` largely untracked in local Archives, tracked source provenance and explicit first-party licence lacking; `ForestFloorV1` previously not adopted | D-032 policy/licence explicit before binary/import; keep design/source evidence read-only until then |
| Save | Signed 3-profile policy plus currently blocked Compact ISO-58 and unsigned content identity | Asset work may neither activate Compact nor commit incompatible content behind save slots |

## Proposed technical approach (ONLY after APPROVED spec + owner decisions)

**Phase 0 — blocking scene decision and safe source manifest:** **GQ-07 requires an explicit architecture decision in both Compact parent and graphics specs before signing any path manifest:** (A) preferred DRAFT separate Compact scene/hierarchy with no E80 planks/track consumers, or (B) source-reviewed shared-scene profile-gated `Renderer.enabled`-only exclusion, correct clone creation order and rebuild on approach Version/Rack toggle/Legacy40 load, with source consumers of no-tree corridor, Stand Map, prescription approval and coverage reconciled. Neither is approved yet. #424 then coordinates the compact world manifest author/integrator to pin actual H1 polygon, Route A eight points/spline, intended **test** 1.2 m walkable core and distinct visual-fringe footprints, four G1 species/positions and all current/failed protection envelopes, original 14 omitted and 8 H1 buffer original trees, terrain and world/profile content version. This graphics spec **cannot authorise or generate** that world from old scene objects or Blender preview.

**Phase 1 — native baseline (only with explicit approved gate and shared machine lease):** one physical Unity owner loads exact new Compact state when implemented; captures H1, first bend, interior and G1 before any graphics delta, at 720p and 1080p including HUD, quality tier, LOD, Sun and third-party pack state. For the new not-yet-existing compact scene, baseline creation is separately controlled source/Unity work **not** an exemption to spec approval. Historical E80 40 ms observation is not a baseline.

**Phase 2 — first-party asset acquisition/source control:** inspect original tracked families and provenance, check whether original editable Blender sources may legally be incorporated, document hashes/licence/author and actual byte budget. After D-032 is accepted, use a **new explicitly owned asset directory** (path to be ratified from actual project before source edit). Prefer tracked project-made LODs; external nature pack and unreviewed ForestFloorV1 are not dependencies. Separate Opus/Blender art author from Unity integrator; never let two writers edit same files or Unity scene.

**Phase 3 — compact-only surfaces:** after GQ-07 scene/plank decision, build a narrow winding walkable path from **signed** route/width with distinct path/ground/H1 materials, ensuring **no old 2.6 m E80 plank is visible or gameplay-active in Compact**, while preserving old-world source/renderer objects. Separate walked/collidable core, rendered ribbon, visible verge and display-only vegetation mask. Trial W worn earth versus N2 only as **unapproved** native looks, **re-proportioned/tapered near G1**: #577 W's ~0.35 m soft edge breaches FR-05's tentative 0.20 m limit. Validate upright, failed (current O1 crossing core by 0.141 m) and expired guard/stake envelopes before any visual GO. Do not resolve clipping by moving planting positions or extra Sitka omission.

**Phase 4 — visual ecology and planting:** keep `ScenarioHabitatVisuals.VegetationSites` authoritative and unchanged. Move/suppress instances only downstream for rendering, deterministically using source `Jitter(cell,kind,plant,axis)` or another explicitly verified hash and a per-species footprint radius including **rushes**, within same cell and without inventing cover. For each cell and kind reconcile **rendered + suppressed = source visual instances**, including fully covered cells. The existing `ScenarioHabitatPresentationVerification.VerifyYear0GrassSupport` visible count check (~line 202) needs a **Compact-only** alternative without changing/removing the Legacy40/Reference check. Keep true ecology counts, previews and clearance targets unchanged. Build true-size Oak/Beech transplants and an accepted **bounded/path-safe failed guard**; preserve runtime species/height/protection. Native 2–3 m close species identification versus 4–6 m discoverability via accepted marker/inspection affordance, **not exaggerated leaves**.

**Phase 5 — isolated optional lighting/background:** only if owner explicitly selects it and its dedicated camera/URP feasibility gate succeeds. Inspect that the Compact camera can actually render a sky/tonemap/fog change and restore global scene state; stage owner comparison, never modify shared `DefaultVolumeProfile`, `PC_RPAsset` or `ForestTest` default outside a separately permitted approved scope.

**Phase 6 — independent evidence and Manager/owner acceptance:** compare source tree IDs, planting/Work Plan, ecology, clearance previews, saved world and historical states before/after. Capture real Compact view poses from H1, C2 first bend, G1 unsheltered/sheltered, a later-year state, with 720p/1080p frame/performance data; independent reviewer and user visual acceptance are distinct. Final D-032 storage/provenance record and manifest signature are prerequisites to asset distribution. No Windows build before separately authorised post-personal Compact Editor playtest.

## Integration boundaries and allowed future file sets

**New file paths are illustrative, not an edit allowlist.** Future worker must inspect current repo and task ownership first and receive its own authorised exact paths. Likely compact-only material/surface/asset families under `Assets/ForestPrototype/Art/` or `Assets/ForestPrototype/ScenarioOne/`, content-aware visual assembler and explicit proof under `Tools/Verification/`. Historic `Assets/Scenes/ForestTest.unity` is a shared scene; any modification requires separate Manager-approved scene/serializer ownership, review, and old-state comparisons.

**Read-only/protected unless explicitly separately approved:**
- `Assets/ForestPrototype/ScenarioOne/ScenarioHabitatVisuals.cs` **`VegetationSites` authoritative generation**, `Assets/ForestPrototype/ScenarioOne/ScenarioOneClearance.cs`, `Assets/ForestPrototype/ScenarioOne/ClearancePreview.cs`, `Assets/ForestPrototype/ScenarioOne/VegetationClearance.cs`, `ScenarioOneManager` planting/site/light, the existing profile-sensitive `ScenarioHabitatPresentationVerification` legacy assertion, all ecology and Work Plan state. Existing authorised tests may gain separate **Compact-scoped** expectations only; never weaken old tests to get green.
- `Materials/Forest Ground.mat`, `Materials/Forest Path.mat`, `Editor/ScenarioOneFloorPresentationSetup.cs`; 52 old `Dirt Path ##` (32 route + 20 H1), `Dirt Path X###`, `Approach Track ###`, `ScenarioOneHeadlandPresentation.cs`, `StandApproachPath.cs`, cutting/headland/feeder/racks, old-world Stand Map and corridor/approval consumers. **The Compact scene inclusion/exclusion is UNDECIDED (GQ-07); do not hide/set inactive or clone until accepted**.
- `Assets/Settings/` URP and camera defaults, `ProjectSettings/`, `Packages/`; immutable signed save contract and user saves/Prefs, Legacy40 and Reference Future; `CCF-Ops/` and another owner's dirty Primary.
- Closed #577 scratch previews outside Git; do not copy them blindly or misrepresent them as game assets.

**No source, Unity or asset edits authorised by this DRAFT.** Asset storage/labelling must wait for D-032, exact source provenance and product/Manager acceptance.

## Ownership and concurrency

- #424 Scenario One Manager is single-writer for live `Docs/Project/coordination/active-tasks.md`, publishes safe specs and integration verdicts. Canonical Decision Log/milestone policy on #585 is separate from this graphics spec and must be resolved before implementation supersedes its current status.
- Native world content/scene writer, asset author, vegetation rendering code writer and save/profile integrator cannot overlap without an approved API. One writable lane per owner. A local worker registers actual model/provider, worktree/branch/HEAD, exact path allowlist and claims before any production edit.
- Only one physical heavy Unity Editor/asset import under a genuine CCF-Ops machine lease; never infer lock/Editor/scene status from GitHub. Protect all unrelated dirty files. No source workers currently claimed by this draft.

## Migration and save implications

No save schema changes, persistence extra fields, harvest marks or tree relocation from **visual-only** work. Site-geometry material changes are world-content changes requiring a **reviewed new content manifest and compatible version/refusal policy** before any save writer, not a visual shortcut. Run old v1–v24/Enlarged80/Legacy40/Reference regression with exact physical branch and safe custody. ISO-58 currently blocks Compact profile release; do not sidestep with PlayerPrefs or alternative save names.

## Verification strategy

**Tier 1 FAST — source/offline checks** after spec APPROVED: path G1 2D/mesh-boundary source calculations including actual spline interpolation, same-cell vegetation census/display mask, source asset/LICENCE/byte report, 14 omitted/8 buffer original roster, exact separate old/shared material hash. Tier 1 is not final Unity/physical pass.

**Tier 2 FEATURE QUALIFICATION — proportional native proof** on exact integrated Compact candidate after production: owner-approved baseline H1/C2/G1 and later-year captures; actual player walk/turn/plant/inspect; 720p/1080p HUD/states; native shelter/groundcover/clearance-target invariants; Compact-specific save identity and old-world matrix; repeated same-machine performance sample with warm-up, p50/p95, texture memory and loader impact. Release a single-bounded independent nonauthor handoff reviewing source/Unity evidence; Manager issues APPROVED/NEEDS CHANGES/REJECTED for the feature.

**Tier 3 FULL INTEGRATED QUALIFICATION** reserved for the complete scenario milestone only, after CG-TRIG, finances, world/save spec and true annual management integration. Avoid unnecessary repeat of previously passed #508/#579/#584 source/document checks. Never call scratch Blender or GitHub source inspection a Unity/render PASS.

**Evidence protocol:** pin source branch/content identifier, manifest version, exact world/quality/LOD/HUD/pack state, seed/time, camera/pose, before/after real captures; keep genuine failures, no weakened verifiers, same hardware and separate high/low settings only when approved. Owner visual acceptance must follow actual scene presentation. No hard 15% performance cap is accepted without a compact baseline plus a Manager/owner criterion.

## Risk register

| Risk | Consequence | Preventative gate |
| --- | --- | --- |
| 1.2m path/G1 footprint too tight after spline/real shelter | Obstructed or invisible planted juveniles | Stop and version a remeasured native route/position candidate; never silently move ecology |
| Display mask changes `VegetationSites` gameplay callers | Different clearance counts/marks and cash | Post-generation-only presentation and direct before/after source+Unity count assertions |
| Old shared ground material/plank clones edited | Legacy40/E80 map, approach, headland drift | Distinct Compact materials/objects and old-state hash/visual regression |
| Full-cover cells yield no alternative plant render site | Faked ecological strength/cover | Preserve ecology counts, disclose suppression and per-cell count witness |
| Camera Solid Colour/URP default mismatch | No-op look change or old-world lighting leak | Dedicated compact camera feasibility and global-state restoration gate |
| Licence / untracked asset originals absent | Nonreproducible/rights-risk beta | D-032 and provenance approval; tracked first-party reuse only |
| 2m art uniformly scaled down to 0.6m | Unrealistic leaf/form and invisible planting lesson | Art-only real-transplant variants plus native close/far checks |
| Previous pass statuses read as permission | Source/Unity work on DRAFT | Each production/native task explicitly blocked until spec APPROVED, owner decisions, safe lock |
| Huge test churn | Slow development | Target Tier1/Tier2 on actual deltas; Tier3 only milestone, preserve known failures |

## Implementation sequence mapped to task IDs

1. **T-01:** finish direct owner look/D-032 and manifest dependencies; independent four-file spec review. Do not claim approval from reviewer alone.
2. **T-02:** pin actual Compact world and early native baseline under another approved world/Unity task; qualified source path/G1 footprint and signed content-version proof.
3. **T-03:** produce/record licensed first-party compact H1, path/floor materials and proper Oak/Beech/shelter editable source **after D-032 and full spec APPROVED**; share no binary files into Git without policy.
4. **T-04:** implement isolated compact-only path/ground/arrival presentation, after exact world/spline and one source writer claim.
5. **T-05:** implement visual-only vegetation masks/understorey with invariants, independent gameplay-clearance negative tests.
6. **T-06:** implement real-size transplant and shelter appearance without changing ecology, stocking or protection rules.
7. **T-07:** optional isolated lighting/boundary trial if accepted; otherwise NOOP.
8. **T-08:** native compact captures/LOD/perf and historical before/after regression, independent Tier2.
9. **T-09:** #424 verdict and owner native-look signoff after true review; release asset branch to controlled integration only, not Windows.

No step above has been executed or authorised by this DRAFT.
