# Sen-1 #637 — Independent A-04b Oak scratch changed-scope review

Date: 16 November 2026. Task: [#637](https://github.com/jermur788/CCF/issues/637). Reviewer: **OpenAI gpt-6.1-sol, high reasoning effort, Codex harness in Linux T3 Code**.

## Verdict

**APPROVED — SCRATCH QUALITY ONLY.** Both major corrections pass. Of the seven minor findings, five pass and two remain explicitly partial: absent venation and residual edge-on leaves. There are **no required asset corrections before Manager review of this scratch baseline**. The toothy LOD1 is acceptable for scratch; it is not a qualified production LOD.

This is an independent non-Claude review of actual local source, six OBJ meshes, measurements and **all ten A-04b/comparison images**. It approves the bounded art revision as a scratch reference, not production stock selection, distribution/licensing, import, gameplay readability, integration or Unity verification. **D-032 remains OPEN; #619 remains DRAFT.** The Manager/owner still decides whether to accept this visual baseline.

The revised collar and attachments check out. One measurement qualification matters: #634 correctly replaces #632's exaggerated vertex-to-vertex gaps with a vertex-to-triangle measurement, but that remains a proxy for the shortest surface gap. Section 5 records the independently measured distinction without changing the revised asset verdict.

## 1. Identity, ownership and starting state

| Item | Actual value |
|---|---|
| Provider/model/effort | OpenAI `gpt-6.1-sol`, `reasoningEffort=high`, read back from this session's T3 configuration. No provider/model switch. Configured Codex provider; billing/quota source not independently exposed. |
| Independence | This session authored neither #627, #632 nor #634. Different model/provider family from their Claude authors. No Claude persistent memory accessed and no delegated reviewer used. |
| Host | `jer-laptop`, Linux `7.0.0-42-generic`. Files storage verified as `/dev/sdb4`, ext4. |
| Branch | `review/sen-1-637-oak-a04b-cross-family-delta-20261116-v1` |
| Actual worktree | `/media/jer/Files/CCF/Operations/CCF-637-Oak-A04b-IndependentReview-20261116` — clean docs-only sparse checkout at registration, not a Unity lane. |
| Starting HEAD | **`2b75ce97d67385d308a298612906ae09371d86ca`**, refreshed `origin/main`. |
| Context-set commit | `7801d9d4f6163f7d196c7d28f6b34a9e7876cf0d`. Live coordination and task/Manager dispatch read separately. PACKET ONLY; reviewer role. |
| Registration | [Actual RUNNING receipt](https://github.com/jermur788/CCF/issues/637#issuecomment-6925592714), posted before writing this Markdown. |
| Physical preflight | `unity-lock status`: FREE, metadata None, no Unity Editor PIDs. No Blender application process; two `blender-mcp` bridge processes were present and unused. No lease acquired. Physical board has no #637 entry; it is Manager-owned and was not edited. Direct user assignment/#637 authorises this read-only review and exclusive report, not any source/Unity lane. |
| Preserved unrelated work | Primary `/media/jer/Files/CCF-Primary`, branch `ops/workflow-foundation-v1`, starting HEAD `46247486b37904f6b36825bb40fb6ed7bea35e88`, dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md`. Neither edited. |
| Only tracked output | This added Markdown. No source, assets, binaries, package/project settings, coordination board, saves or preferences changed. |

## 2. Inputs, custody and reproducibility evidence

Read the exact [#632 report](https://github.com/jermur788/CCF/blob/1a03d3cd8571b3748132873960b58091b30d2ad7/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-632-INDEPENDENT-OAK-JUVENILE-BOTANY-AND-SCRATCH-ART-REVIEW-20261116.md), [#634 report](https://github.com/jermur788/CCF/blob/267f5a26c952dbafb057f7326e107f70ddb7176a/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-634-OAK-A04B-SCRATCH-REVISION-AND-REPRODUCIBILITY-RESULT-20261116.md), their issues and the [binding Manager dispatch](https://github.com/jermur788/CCF/blob/docs/sen-1-425-634-oak-verdict-637-review-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-425-MANAGER-634-OAK-A04B-REVISION-AND-637-CROSS-FAMILY-REVIEW-DISPATCH-20261116.md). GitHub report bytes matched the locally inspected Markdown: SHA-256 `b601d3ad343f089f53990b0e241adecd764a5284e527c629ac076f4cce00c49c` (#632) and `9c4ebe6e00e2c27960fc4e0c7d17eaf0db6a7d8ba7a60ad456828f1c4c63d147` (#634).

Read-only local roots:

- A-04a: `/media/jer/Files/CCF/Verification/Sen-1-627-LinuxOakScratch-ResourceProfile-20261116/`
- A-04b: `/media/jer/Files/CCF/Verification/Sen-1-634-OakA04b-ScratchRevision-20261116/`
- Reviewer-owned receipts: `/media/jer/Files/CCF/Verification/Sen-1-637-OakA04b-IndependentReview-20261116/`

| Check actually performed | Result |
|---|---|
| A-04a manifest | **35/35 hashes match**, manifest SHA-256 `b8d7e81f242dd07367857a7cd85fc3712bfa2e5672d684e131bf2a57b4f1ef0c`. Originals preserved. |
| A-04b manifest | **34/34 hashes match**, manifest SHA-256 `4e9f2a27de2e05523fddb990e1723e7c8b3deaa60bdf6f8a339ce0fa50bd9179`. |
| Actual procedural source | Read `tools/build_oak_juvenile.py` and `tools/build_oak_a04b.py`, both render scripts, `run_iso.sh`, `runs.log`, stats, provenance and mesh checker. Six actual OBJ files parsed. `.blend` files hash-checked, not opened. |
| Existing A-04b rebuild products | All **7/7** OBJ/MTL/stats products in `asset/` match `rebuild-verify/` byte-for-byte. This independently verifies the stored comparison, **not a new independent Blender rebuild**. |
| Authored checker rerun | Inspected before execution; pure Python, reads OBJ, writes only reviewer receipt. All six results reproduced; receipt SHA-256 exactly matches authored `checks/mesh_checks.json`: `aadd3f9968abefaeb76f8522c893b3aa8ae8018c803aa07297b8caab50f0571a`. |
| Additional independent measurements | Separate stdlib OBJ/UV parser, exact polygon calipers, boundary-lobe counts, and triangle-surface clearance including edge-to-edge distances. Known geometric sanity cases passed. Narrow segment/triangle intersection primitive reused from the inspected checker; other distance/measurement logic independently written. |

No Blender was launched. Existing source, isolation wrapper, successful run log and deterministic exported-file comparison answer the bounded rebuild-evidence question; another render/build would add no needed evidence. The wrapper explicitly isolates `TMPDIR` and `BLENDER_USER_CONFIG/SCRIPTS/DATAFILES`, checks `/tmp/quit.blend` and other Blender processes, and uses background mode. This review does not independently prove past process safety, `.blend` semantic equivalence, or GUI isolation. No resource profiling repeated.

## 3. Finding-by-finding disposition

PASS means the requested scratch correction is verified; PARTIAL records a remaining limitation, not a production waiver.

| #632 ID | Result | Evidence and judgment | Smallest next action |
|---|---|---|---|
| **M-1 — stock age** | **PASS** | A-04a treated the leader as one spring/lammas season. A-04b explicitly separates leafless year-1 wood below the 0.240 m previous-terminal scar from year-2 current growth, spring/lammas boundary 0.440 m, and current side shoots. LOD0 has dormant axillary buds and scar bumps; material names state previous/current season. The short shoot originating at 0.175 m on old wood is itself current-year and may bear leaves. The actual front/hero renders show the older basal section and current crown. This is a plausible simplified **provisional 1+1 nursery-summer form**, not proof of nursery history or an Irish game-stock selection. | Retain the provisional label. Manager/owner must choose production stock/state separately. Do not relabel it as first summer after field planting or claim a one-year 0.6 m seedling. |
| **M-2 — root collar** | **PASS** | Independently sliced actual OBJ main-stem triangles at 0/5/10/20 mm height. Every minimum caliper exceeds the cited **UK 11 mm** comparator; lowest is **11.711 mm**, LOD2 at 20 mm. Polygon compensation works, including the triangular LOD2. Full numbers below. | No scratch geometry fix. Preserve measured rather than nominal widths. UK comparator is not an Irish statutory or universal nursery rule. |
| **m-1 — widest leaf point** | **PASS** | Source envelope peak is `1/(1+0.85) = 0.54054`. Actual LOD0 OBJ/UV margin measurement gives **mean 0.534984**, range **0.471346–0.608512**, versus A-04a mean **0.634856**. Envelope and actual lobed-margin peak are different quantities. Top/close-up images show a more central widest point, cuneate base and no auricles. | Retain. Do not describe every leaf as peaking exactly at 0.535 or use juvenile foliage alone as reliable species identification. |
| **m-2 — lobes/petioles** | **PASS** | Independently counted actual LOD0 boundary maxima: **5–7 lobe pairs, mean 5.86207**, versus A-04a 4–5, mean 4.5. Source depth 0.28–0.38 and rounded LOD0 lobes remain shallow. Wedge base and visible petioles persist; source calibrates petioles to 12–22 mm. | No required fix. Dimensions/counts remain art calibration, not measured Irish juvenile averages. |
| **m-3 — attachment/gap correction** | **PASS** | Rerun confirms **29/29 attached assemblies at all three A-04b LODs**. LOD1/2 retain a one-triangle stalk welded to the blade and originating inside their own coarse shoot, eliminating the original omitted-petiole gap. Visual lineup agrees. This is geometric contact, not a claim that the entire plant is one watertight manifold. Old measurements need the qualification in section 5. | No asset fix. Future reports must name the gap metric and tolerance. |
| **m-4 — useful LODs** | **PASS, scratch scope** | Actual tris **6,044 / 1,196 / 311**; LOD1 is **19.79%** of LOD0, versus 42.31% previously. In the actual lineup, LOD1 preserves crown arrangement, age structure and leaf locations. LOD2 keeps attached tapered/obovate blades and overall juvenile silhouette; it is still unlobed and cannot independently identify sessile Oak. The LOD1 sawtooth outline is conspicuous at equal enlarged presentation but acceptable scratch simplification. | **No new modelling cycle required.** If Manager requests one polish, dispatch only a phase-aware extra shoulder sample per lobe, with bounded triangle recount and comparison. No threshold/distance or performance acceptance until a later gameplay-camera gate. |
| **m-5 — fold/shading/venation** | **PARTIAL** | Actual close-up comparison shows smoother shading and a gentler cup; source adds an inner row and reduces fold. No midrib/lateral-vein texture or normal map exists; an internal mesh row is not visible botanical venation. Some polygonal outline remains at extreme close-up. | Defer visible venation to separately authorised D-032/#619 material work. Accept present untextured scratch geometry. |
| **m-6 — edge-on leaves** | **PARTIAL** | Reduced fold and wider elevation/roll variation improve some angles, but front `02` and lineup `05` still show narrow/edge-on blades. Top view reads as foliage. A horizontal leaf naturally narrows in an eye-level view; no gameplay pitch/distance evidence exists to decide player readability. | Keep as a recorded limitation. Judge at the actual future gameplay camera before changing orientation again. No native readability PASS. |
| **m-7 — clearance** | **PASS** | Authored rerun reproduces old LOD0/1 **2** intersecting assembly pairs and revised **0**. Additional blade-surface checks, including edge-to-edge distance and non-overlapping boxes less than 2 mm apart, find **no intersecting or <2 mm blade pairs at any revised LOD**. Top-view layering remains, but projected overlap does not prove physical penetration. | Retain. Centre-distance placement is a heuristic, not a guarantee for future seeds/variants; repeat geometric checks if those change. |

The UK nursery comparator comes from the Leslie (2024)/Morgan (1999) source chain already cited in #632/#634. No new general botanical research was conducted. A 1+1 label describes a two-season nursery assumption; the precise 0.24 m split, colours, leaf size/count, branching and collar target remain calibration. Juvenile sessile/pedunculate identification limits remain as recorded in #632.

## 4. Independent collar measurements

OBJ export is **Y-up**, despite the source's Blender Z-up convention. Slices use OBJ Y for the requested source-height z. The main-stem component is isolated from blades/buds. Triangle/plane intersections form a convex section; the minimum width is calculated at hull-edge normals and maximum width as the hull's diameter. This avoids relying on nominal radii or a sampled orientation that misses the narrowest caliper. OBJ coordinates are rounded to micrometres, so three-decimal-mm display is numerical detail, not empirical precision.

Each entry is **minimum–maximum caliper, mm**:

| Mesh | z = 0 mm | z = 5 mm | z = 10 mm | z = 20 mm |
|---|---:|---:|---:|---:|
| A-04b LOD0 | 12.510–13.549 | 12.361–13.387 | 12.121–13.126 | 11.775–12.756 |
| A-04b LOD1 | 12.180–12.819 | 12.074–12.709 | 11.969–12.598 | 11.759–12.377 |
| A-04b LOD2 | 12.157–14.043 | 12.046–13.915 | 11.934–13.786 | 11.711–13.529 |

All pass the **cited UK 11 mm art comparator**. A-04a independently measured minimum calipers at z=0/5 mm were 9.467/9.430, 8.650/8.483 and 7.419/7.341 mm for LOD0/1/2 respectively. The original problem was real and worsened with low-sided polygons; the revised mesh fixes it.

## 5. Floating-leaf measurement correction, with its limits

The old #632 **7.9 / 17.4 / 45.0 mm** statement came from distances between sparse vertices. Independently recomputing those nearest-vertex distances yields min **7.901**, upper-middle **17.446**, max **44.966 mm**. They do **not** describe the shortest visible surface gaps: the closest point can lie inside a stem triangle or along an edge.

The inspected #634 checker improves this by testing leaf-edge/wood-triangle intersections and then nearest leaf-vertex/wood-triangle distance, using **0.1 mm** as contact tolerance. My rerun reproduces **29/30 floating A-04a LOD2 assemblies; min 0 / upper-middle 9.56 / max 13.75 mm** across all 30 leaves. The zero belongs to the intersecting leaf. These are correctly reproducible **checker-defined proxy values**, not exact surface-to-surface distances. The checker calls the upper middle of an even-sized list its median.

An additional triangle-surface calculation including edge-to-edge distances gives, across those same 30 old blades, **min 0 / conventional median 9.482 / max 12.981 mm**. One other old leaf approaches to **0.0685 mm** at an edge; it is geometrically separated but inside the checker's 0.1 mm tolerance. Therefore **28/30 exceed 0.1 mm under this fuller distance metric**, versus 29/30 under the documented proxy. This is a reporting-method refinement, not a newly introduced revised-mesh defect.

**Disposition:** uphold the old floating-leaf finding and the revised attachment fix. Reject 45 mm as a surface-gap claim. Accept 13.75 mm only when labelled the reproduced vertex-to-triangle proxy; use the fuller **12.981 mm maximum** if claiming the shortest triangle-surface gap for these exact OBJs. No source/evidence was overwritten to make the numbers agree.

The authored near-contact counter also uses vertex proximity and overlapping boxes; it is not by itself a proof of 2 mm surface clearance. The independent expanded-box/triangle-surface check supplies that additional evidence for the revised blades.

## 6. Ten-image inspection record

Viewed every A-04b `renders/01…05` and `compare/cmp_01…05` image directly. Pixel comparisons independently confirm that each comparison's left panel is exactly the original A-04a image and its right panel exactly the revised image, without rescaling or retouching.

| Render and corresponding comparison | Direct observation |
|---|---|
| `01_hero_three_quarter.png` / `cmp_01_hero_three_quarter.png` | Thicker older basal stem and separated current crown read clearly; 0.6 m marker remains. Some leaves still present narrow profiles. |
| `02_front_ortho_scale.png` / `cmp_02_front_ortho_scale.png` | Leafless older leader section, side shoot from old wood and current branching are visible; residual edge-on leaves are obvious. |
| `03_top_plan.png` / `cmp_03_top_plan.png` | More centrally broad, elongated lobed blades; overlapping projected layers remain natural to this view. Geometry checks, rather than this projection, establish clearance. |
| `04_leaf_closeup.png` / `cmp_04_leaf_closeup.png` | Smoother cup and shading, cuneate attachment and visible petioles; no venation. Close-up framing follows each asset's selected leaf, so it is not an identical-leaf photometric comparison. |
| `05_lod_lineup.png` / `cmp_05_lod_lineup.png` | LOD1 teeth are visible; LOD2 preserves attached blades and architecture with simpler outlines. Original low LOD visibly loses attachment quality. No runtime transition was tested. |

Both render scripts have identical executable camera/render logic; only their docstrings differ. Their cameras, adaptive close-up target and evidence sizes are Blender preview conditions, **not 720p/1080p gameplay evidence**.

## 7. Verification receipts and handoff

Reproduce the independent geometry receipt with:

```bash
python3 -I -B /media/jer/Files/CCF/Verification/Sen-1-637-OakA04b-IndependentReview-20261116/independent_mesh_review.py
```

It reads the two evidence roots and writes only its own reviewer receipt/log location. No package installation, Blender, Unity, display or source mutation is involved. Reviewer receipts include:

- `custody.json`: both manifests and 7/7 existing rebuild-file comparison.
- `comparison_pixel_custody.json`: all five exact old/new comparison panels.
- `author_checker_rerun.json`: all six reproduced authored checks.
- `independent_measurements.json`: calipers, actual UV boundary counts, blade clearance and old LOD2 distances. SHA-256 **`d332087bd8de9db82b10b57c02becde0ea6832cd1eeb69d112c22bc915fb0f54`**.
- `independent_mesh_review.py`: reviewer measurement source. SHA-256 **`9a72a88730e4c9b2dd320482746327572ceabb824df6a3bd92e4c43a4335402e`**.

**Not performed:** any asset/model/source revision; Blender rebuild or GUI; Unity launch/lease/import; FBX generation; native rendering or 720p/1080p player tests; performance benchmarks; binary Git upload; packages/downloads; save/preferences changes; Windows build. Existing original/revised evidence remains read-only. No integration/main merge is requested or claimed.

**Remaining production gates:** Manager/owner stock and biological-state choice; D-032 rights/storage policy; approved #619; separately authorised FBX/export/import route; approved Linux Unity lane and actual lease; representative scene, two-sided URP leaf material, camera-distance/LOD-transition checks, rendered 720p/1080p player review and performance/behaviour verification. Source provenance is inspected procedural authorship, not a legal licence determination.

**Dashboard sync: NOT AVAILABLE / published state not verified.** No dashboard publisher/ingestion entry point was found in this repository. Live `active-tasks.md`/physical board are Manager-owned; this task owns only this result and its issue registration/handoff. Outstanding operational follow-up: #424/#425 or the assigned maintainer records the scratch-only review, reconciles the absent physical #637 row, and verifies dashboard audit/public readback. No generated dashboard state was manually edited. Issue publication/closure is not evidence of dashboard synchronization.

**Recommended next step:** Manager reads this independent verdict and decides whether to accept A-04b as the provisional scratch visual baseline. No further scratch correction is required by this review; optional LOD1 polish and all production adoption remain separately scoped decisions.
