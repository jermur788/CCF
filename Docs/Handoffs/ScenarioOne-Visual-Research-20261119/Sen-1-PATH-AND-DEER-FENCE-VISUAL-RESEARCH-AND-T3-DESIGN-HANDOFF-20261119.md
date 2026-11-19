# Sen-1 — Scenario One path and deer-fence visual research, mockups, and T3 design handoff

**Date:** 19 November 2026  
**Status:** **RESEARCH / CONCEPT DESIGN — HANDED OFF FOR SCENARIO ONE MANAGER REVIEW; NOT APPROVED FOR PRODUCTION**  
**Claim:** [#835](https://github.com/jermur788/CCF/issues/835)  
**Owner/provider:** OpenAI GPT-6, browser research + GitHub documentation only  
**Branch:** `docs/sen-1-path-deer-fence-research-20261119-v1` — isolated GitHub documentation lane, no native worktree/Unity/Ops lease  
**Task class:** design research, not an implementation packet or approved feature spec  
**Scope:** Scenario One; propose a *reusable* boundary deer-fence presentation kit, without changing simulation, forestry protection rules, property layout, save or project-wide art authority.

## 1. Research summary and evidence boundaries

| Finding | Evidence type | Design implication |
|---|---|---|
| Irish **Teagasc** forestry-fencing guidance calls for **1.9 m rectangular deer mesh**, or two joined sheep-netting heights; explicitly **no barbed wire on top**. Also requires suitable access and distinguishes deer protection from ordinary stock fencing. | Irish official professional guidance | Tall mesh silhouette with round timber posts, fine galvanised wires, ground contact and optional secure access. Not garden rails or a 1 m stock fence. |
| **UK Forestry Commission / GOV.UK** deer grant specification identifies 1.8 m for red/sika/fallow; timber end/turning posts and bracing, up to 10 m maximum intermediate spacing for those species; fences must follow the ground without deer-sized gaps. | British guidance — **transfer context**, not Irish mandatory dimensional policy | Distinguish intermediate posts from stronger corners/ends. Visual system follows terrain; build with mesh netting/strainers, not continuous timber rails. |
| **Scottish Farm Advisory Service** notes stronger end/corner/slope support, lower mesh treatment where other browsing wildlife or adjacent livestock require it, and gates for practical access. | British practice — transfer | Optional slope, brace and gate modules, not universal compulsory additions. |
| **Teagasc forest-road advice** describes aggregate, camber and drains for vehicle-capable harvesting roads. | Irish professional guidance, **different infrastructure category** | Do not transplant vehicle-road width, deep ditches or machinery ruts onto Compact30's pedestrian-only winding footpath. |
| The supplied product brief and latest **D-083, D-084, D-085 and D-090** call for Compact30, a 1.2 m pedestrian core, ≤0.20 m *visual-only* verge, no internal racks, one continuous ground plane with separate logical masks, and provisional worn-earth/needle-litter look. | Accepted owner direction and provisional authored candidate | This is the **controlling Compact30 design boundary**. Do not widen the collider, move signed-sensitive trees or recast H1 as a gravel haul road. |

**Primary links:** [Teagasc forestry fencing](https://teagasc.ie/crops/forestry/advice/establishment/fencing/); [Forest Research fencing guide](https://forestresearch.gov.uk/publications/forest-fencing/); [UK FG9 deer fencing dimensions](https://www.gov.uk/countryside-stewardship-grants/deer-fencing-fg9); [Farm Advisory Service woodland fences](https://www.fas.scot/article/the-fencing-contractor-fencing-woodlands/); [Teagasc forest-road construction](https://teagasc.ie/crops/forestry/advice/management/construction-of-forest-roads/). Visual-photo reference examples (not licensed production textures): [Tornado high-tensile deer mesh installed with timber bracing](https://www.tornadowire.com/Catalogue/Shop-by-Application/DEER/Wire-Fencing/TorusStiff-Stay/Torus-Stiff-Stay-R1218215-High-Tensile-Deer-100m-RZST121801), [Irish forestry road photo, Forestry Focus](https://www.forestryfocus.ie/social-environmental-aspects/biodiversity-and-nature-conservation/biodiversity-in-forests/plantations/).

**Authority note:** Design sizing and asset counts below are production *proposals*, not surveyed Scenario One measurements, officially certified fence engineering, or accepted product decisions. There is **no evidence that this browser session ran, rendered or inspected the current Unity scene**. Owner-reported visual faults are the problem statement, not an independent render diagnosis.

## 2. Preferred design direction

### A. Pedestrian path — worn earth with restrained grit; not a forest road

Use the currently selected **worn-earth / brown needle-litter** family as the dominant core surface for Compact30; include irregular patches of damp soil, fine grit and leaf/needle fragments. Avoid a uniformly grey, thick gravel roadway; reserve visibly stony treatment for very small naturally walked compacted patches near the H1 arrival if it remains consistent with owner art direction. Keep underlying authored path core continuously walkable at **~1.2 m**, with a narrow ≤0.20 m **visual-only** feather each side. Use texture mask variation and debris clusters at low density, not geometrically broken collision edges. Mild shallow crown/damp breakup is an art choice only; do not create ditches, raised kerbs, unsafe foot-level rocks, broad twin-ruts or extra clearing.

**Priority:** silhouette/continuity > surface material clarity > small ground scatter.

### B. Path-to-planting transition — eliminate geometric discontinuity before decoration

**Diagnose first:** capture overhead, oblique and standing-player screenshots in the exact target scene; identify whether the owner's straight/square line comes from (1) path spline/polygon ends, (2) a planting-clearing logical/visual mask, (3) disconnected material UV/terrain tiles, (4) a real mesh gap, or (5) vegetation spawn bounds. Do **not** assume a texture blend solves an actual hole.

**Correction concept:** use one continuous ground plane (as D-090 directs) and derive the path visual mask along the same reviewed spline/curve as the walkable core. At junctions let the worn-earth core taper and curve continuously rather than terminate in a square rectangular decal; feather only the **visual** margin via distance-based mask (e.g. smoothstep weighted by low-frequency noise), then overlay sparse compatible grass/fern/needle clusters. Rounded edge noise is subordinate to true path geometry. Let planted-area *visual groundcover* overlap into the verge, but **do not move the planting admissibility mask, planted tree coordinates, player collider or signed world geometry to hide the seam**. The authored Compact30 A7 bend and provisional A8 endpoint remain unsigned/unqualified; return those to the authorised world/geometry owner if they are the source of the visible join.

**Acceptance observation:** from standing eye height and 45° oblique and top-down views, no exposed strip, hard 90° segment, sudden surface cut or disconnected join; all path core continuously traversable; no vegetation/guard colliders intrude into the 1.2 m core; visual verge remains non-walkability-authoritative.

### C. Deer fence — convincing and readable perimeter presentation

Use **~1.9 m high galvanised rectangular mesh** stretched between **round treated timber** intermediates, with substantial braced turning/end/strainer assemblies and a top plain tension line. Mesh fine enough to read as real forestry fencing, not a thick architectural grid, and down to the contour without floating gaps. Use a rhythm of posts and a faint silver mesh highlight in normal oblique daylight. Avoid horizontal wooden rails except at functional gates/strainers, avoid ornamental pickets, avoid barbed wire.

**Spacing:** an initial **~6–8 m visual span target** is a proposed game-art implementation envelope to test on the actual perimeter, not an Irish universal spacing prescription. The UK FG9 guide lists up to 10 m for intermediate stakes; the Irish field-day sheet also presents 8 m among its specification figures. Do not adopt 2.5–3 m post spacing simply because a generated image happens to show or label it. Strong anchors and the top/bottom wire help long-distance visual readability. Never scale posts into visually oversized telephone poles solely to force legibility.

**LOD/visibility:** hero near (<10 m) rounded wood grain + discrete knots/staples and mesh grid; mid-distance simplify wire crossings and strengthen only the existing top/bottom wires and post silhouette; far distance collapse netting into unobtrusive transparency/line silhouette with posts still visible. These distance bands are initial *visual QA targets*, not measured Unity LOD values. Test moiré, shimmer and alpha discard under URP 17.6, daytime/overcast, and motion; prefer economical alpha-clipped two-sided mesh or selectively physical wires near gates; thin mesh should not cast a dense, distracting full wire shadow. Use wildlife collision markers only when a site-specific need/design decision warrants them.

**Boundary logic:** the physical fence follows the **actual authored property boundary**, not a decorative convenient polygon; the visuals **must not** independently switch off browsing pressure, block animals, change access, save state, or claim a deer-exclusion mechanic. D-044 defers deer-fencing *gameplay*; proposal is render/presentation only. If an actual boundary fence has to be invented rather than merely replacing an existing visual, seek owner/Manager confirmation first, since the visible boundary treatment must match authored world truth.

## 3. Visual mockups — repository-tracked, original concept schematics

All three files are **original SVG design mockups** authored for this handoff, committed as **text-only documentation assets** (not production media). They are not game captures, not rendered prefab geometry, and not to be treated as measured site plans.

| ID | Visual | Intended reading |
|---|---|---|
| **A — Path and planting edge** | [Open SVG mockup A](https://github.com/jermur788/CCF/blob/docs/sen-1-path-deer-fence-research-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-MOCKUP-A-PATH-BLEND.svg) | Before/after schematic: remove hard square gap, retain 1.2 m continuous path, fade ≤0.20 m visual-only edge and plant groundcover organically. |
| **B — Deer fence in scene** | [Open SVG mockup B](https://github.com/jermur788/CCF/blob/docs/sen-1-path-deer-fence-research-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-MOCKUP-B-DEER-FENCE-IN-SCENE.svg) | Illustrative standing-player viewpoint: clear timber-post sequence, understated silver wire mesh, bracing and natural forest context. |
| **C — Reusable fence kit** | [Open SVG mockup C](https://github.com/jermur788/CCF/blob/docs/sen-1-path-deer-fence-research-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-MOCKUP-C-FENCE-MODULE-KIT.svg) | Straight repeat, corner, braced end, optional gate, slope/terrain-follow segment; technical shape guide not engineering standard. |

Generated **raster concept visualisations** were additionally produced in the originating browser chat as A path, B in-scene fence and C kit crops, plus a combined look-development board. They are **clearly concept output** and remain as downloadable conversation attachments only; they are **not falsely claimed as committed to GitHub**, and their internal text/spacing is illustrative. The three SVGs above are the durable GitHub-linked visual mockups. No third-party reference photography was copied into these repo assets; reference-photo links remain research only.

## 4. Suggested modular T3 asset bill of materials (unapproved)

| Component | Proposed authored variant | Level of detail / reuse |
|---|---|---|
| `Fence_Post_Intermediate` | 1–2 small timber grain/diameter variants | Shared across scenarios, low-poly cylinder/post silhouette; local height normalised, no strict one-to-one real-world material spec. |
| `Fence_Mesh_Span` | Parametric width / height anchored between posts; optional denser lower wire only if design needs it | Repeating tiled galvanised netting; near mesh enough to see, mid/far simplified. Avoid heavy per-wire full geometry on all spans. |
| `Fence_Anchor_End` | Braced terminal with tension wire | Distinct heavier timber, near-camera detail justified. |
| `Fence_Corner` | Turn/strainer variation | Connect angles without floating/wrapped mesh discontinuity. |
| `Fence_Gate_Access` | Optional mesh-and-timber or steel framed secured access | **Only place if existing world entry/access actually supports it**; do not invent a gate solely for concept art. |
| `Fence_Slope_Adjust` | Short span, post-height/mesh fit variant | Follow piecewise slopes without ground gap or distorted mesh raster. |
| `Path_Core_Material` | Worn-earth/needle-litter with muted small grit, roughness variance | Scenario One aesthetic content; can be replaced per biome in future. |
| `Path_Verge_Mask` | Noise-modulated mask along approved curve, width capped to authored visual verge | Doesn't alter walkable core, planting permissions, ecology or ground truth. |
| `Verge_Decals` | Needle litter, moss, sparse small stones and low grass/fern edge patches | Reuse existing licensed first-party decals where allowed; sparse and conditional on actual stand ecology, avoid random ornamental flowers. |

**Material direction:** muted wet Irish woodland earth rather than pale bright clean gravel; green-grey/ochre moss and needle litter, subdued steel-zinc mesh, weathered natural brown timber with restrained variation. Use URP-authored consistent roughness, subtle depth/normal at foot level, no visible repeating 1 m tiles. Authoring sources should be original/procedural with documented authorship and source/editability; **D-032 rights, public-binary storage policy and the graphics spec are not approved by this handoff**.

## 5. Practical production sequence for later T3

This is an **implementation-ready design brief**, **not an implementation start instruction**.

1. **G0 inspection/authority:** T3 checks latest `main`, the current Compact30 graphics four-file DRAFT or later approved status, #688/#785 ownership, actual authored Compact path curve/ground/masks/guards, actual existing fence/boundary hierarchy and source licence policy. Produce three screenshot witnesses of current path, planting join and boundary. Do not assume legacy Enlarged80 pictures are the Compact30 current target. Seek signed world/scene/asset file allowlist and actual branch/worktree/physical lane before writing.
2. **Path-first micro-slice:** make a 1.2 m walkable worn-earth path visual along *approved* curvature and cap the lateral fade to ≤0.2 m visual-only; resolve any real gap or bad polygon seam from ground/mask alignment. No player collision/planting-world changes without separate approved geometry authority. Verify actual H1 connection and A7 bend, not merely an attractive static shot.
3. **Fence vertical slice:** author one straight segment, a visible braced end and one corner, place on a **non-production art test stage** only; verify perspective readability/mesh aliasing/terrain contact. Expand to gate/slope only when functional/art fit is demonstrated and approved.
4. **Scenario placement:** following separate approved graphics/source permit, place only on the verified authored boundary, adjust anchors to boundary corners and elevation, preserve nav/access/collision truth. Use reusable scene-independent prefab/material definitions; no scenario-wide mechanics abstraction.
5. **Qualification:** compare close/normal/distant standing-player views under neutral and overcast lighting, and screenshots at 1280×720/1920×1080; moving camera shimmer/LOD, terrain/vegetation contact, overhead seam and end junction, guard clearance/collision, performance draw calls and material tiling. Preserve old Enlarged80, Legacy40 and frozen Reference Future; don't infer from this document that they were tested.

### Proposed acceptance checks (subject to Manager/spec approval)

- One continuous current Compact pedestrian path; clear readable texture and soft edges; no square cut/gap; ~1.2 m functional core maintained; ≤0.20 m visual-only verge; no new internal extraction rack or machine road.
- Site masks remain truthful: planted Oak/Beech exact positions and guards clear the path; no magic vegetation bridge conceals a geometry error; continuous ground remains intact.
- Boundary fence reads as real ~1.9 m deer netting with correctly spaced and braced posts; no barbed top; clean corner/end/gate treatment when applicable; no visible float-through ground.
- Normal-player perspective and motion show fence without extreme thick mesh, noise/shimmer or lost whole boundary.
- No unintended browsing/economy/save or protection state change, no deviation to signed world/profile/Reference Future; no Windows build.
- Asset source, texture provenance, editable files, rights/storage policy and release authorisation documented, not inferred from a concept image.

## 6. Ownership, routing and next step

**Conflict-aware:** [#824 Scenario One Manager](https://github.com/jermur788/CCF/issues/824) retains Scenario One management and live `active-tasks.md` write authority. [#688](https://github.com/jermur788/CCF/issues/688) / [#785](https://github.com/jermur788/CCF/issues/785) already own/gate Compact path/H1 art authoring, [#691](https://github.com/jermur788/CCF/issues/691) retains DRAFT graphics-spec status, and [#718](https://github.com/jermur788/CCF/issues/718) is the separate native scene gate. This research makes **no claim to those files, assets, locks or implementation**. D-083/D-085/D-090 constrain Compact, D-044 continues to defer deer-fence protection gameplay, D-032 rights/storage remains open. No Multi work.

**Manager handoff decision requested:** inspect the three mockups + guidance; approve or amend preferred visual treatment within current approved/owner-intended graphics direction. If accepted, incorporate into the existing Compact graphics/asset ownership route rather than create a duplicate overlapping #688 production writer. The fence kit can be a **separately approved visual asset slice** if adding physical perimeter fence assets changes authored world content. No implementation is automatically dispatched from this handoff.

**Later worker recommendation:** SOL/Codex with Blender/Unity/URP access; difficulty **Medium** (art/geometry seam investigation, modular mesh LOD and rights/scene constraints). Exact source lane, branch/worktree, physical reservation and worker claim: **NOT ALLOCATED**. Start is blocked until the correct Manager approvals, signed authored world/graphics authority, source assets licence policy and local production lease are obtained.

**Future copy-paste request to Scenario One Manager (NOT a current T3 start permit):**
> Sen-1: review visual research claim #835 and the path/deer-fence handoff; decide the Compact30 worn-earth path and non-causal boundary deer-fence art direction. Align any approved change with #688/#785 and the Compact graphics spec, assign exact source/Unity rights and worktree/lease only when all gates pass; no game mechanics, Multi or Windows.

## 7. Verification, limitations and closure

**Actual work done:** public-web research against Teagasc, UK Forest Research/GOV.UK and FAS; live GitHub Decision Log/current milestone/coordination and #688/#691/#785/#824 inspection; three new original SVG concepts committed to one new isolated docs branch; generated raster lookdev previews produced only as conversation assets.

**Not done:** no live in-Editor scene review, scene/collider/camera measurements, independent art QA, Unity render/compile, native asset build, landscape edit, source-code edit, production scene placement, performance test, gameplay protection test, merge or user art acceptance. Owner reports of current defects are **unverified visually by this agent**. Thumbnail/rendered SVGs are not proof of production suitability.

**Verdict:** **RESEARCH + MOCKUP HANDOFF DELIVERED; PRODUCTION UNAUTHORISED.** User action required for this research: **none**. Scenario One Manager #824 must adjudicate before production. Dashboard sync: **NOT VERIFIED**. Drive context mirrors: **NOT UPDATED** (no canonical context sources changed on this docs branch).
