# Storm v1 minimum visual package

**Status:** requirements and production briefs. Nothing created or imported.

## 1. Minimum set

| # | Visual | Source | Classification |
|---|---|---|---|
| 1 | Root plate at the base of each windthrown tree, rotated to the fall bearing and scaled by DBH | **Reuse** `SS_WindthrowBase_Fresh_01` → `_Weathered_01` (switch at DecayClass ≥ 2 or 4 years) | BLOCKER (exists) |
| 2 | Fallen stem from the root plate along the storm direction, length ≈ 0.8 × H | **Reuse** fresh/decayed log, base-anchored. Better: **procedurally chain 2–4 log instances** with hashed roll/scale jitter, to avoid 5× texture stretching | BLOCKER (code); POLISH (segmenting) |
| 3 | Fallen crown for fresh windthrow (years 0–2): green, then grey | **Option A (recommended v1):** reuse the tree's own living display model, rotated about the base (green, years 0–1), then a `MaterialPropertyBlock` tint (needle loss look, years 1–3), then remove the crown, leaving the log. **Option B:** new "fallen crown/top debris" asset (brief §3) | NEEDED (A is a code-only path; B is polish) |
| 4 | Disturbed ground / soil pit beside the root plate | **New:** small decal or low-poly pit mesh (brief §4) | NEEDED |
| 5 | Fresh break / exposed wood | **Already in** the fresh root plate (broken basal stem, exposed wood) | — |
| 6 | Salvage residue | **Reuse** compact brash bundle | — |
| 7 | Deadwood ageing | **Reuse** decay-class switch and tint; fungi/moss dressing already keyed to deadwood | — |
| 8 | Snapped stem / snag | **v1.1 only** (`windsnap`) | deferred |

What can be:

| Approach | Items |
|---|---|
| Reused as-is | root plates, logs, brash, decay tint, fungi/moss dressing |
| Scaled/rotated (deterministic hashes of `treeId`) | root plate (DBH scale 0.7–1.4, mirror), log heading jitter ±20°, segment roll |
| Procedurally varied | crown tint over time; log segment count from height; root-plate tilt toward the pit |
| Created in Blender | disturbed-ground pit; optional long fallen-stem piece; optional fallen-crown debris; v1.1 snag |
| Sourced CC0/free | ground-disturbance decal textures (soil/needle mix) are plausible CC0 candidates. **Licence and provenance must be recorded** per project rules |

## 2. Brief — Fallen Sitka long stem (optional v1 polish)

- **Biological/game state:** windthrown Sitka stem lying on the ground, fresh to early decay. Used with a separate root plate.
- **Visual purpose:** a believable 12–25 m stem without stretched textures; reads at walking distance and from 30–60 m.
- **Scale:** 1 Blender unit = 1 m. Modular: a 6 m mid-stem segment and a tapered 6 m top segment, both with bark-continuous UVs (tiling along length). Diameter 0.35 m nominal (scaled per tree).
- **Variants:** fresh (bark intact, branch stubs) and weathered (bark patches lost, moss-compatible).
- **LOD:** 3 LODs, targets ≤ 1,200 / 600 / 200 tris per segment.
- **Unity:** URP/Lit, ground-level pivot at the butt end, +X along the stem, no collider (a capsule added in Unity if needed).
- **Biological boundary:** no crown; no fungi baked in (dressing is separate).
- **Performance target:** 100 fallen stems × 3–4 segments within the Scenario One frame budget on reference hardware (GTX 980M class).
- **Source/licence:** first-party Blender, the same method as `SS_Log_*`; record provenance.
- **Acceptance:** import → material → prefab with LODGroup → placed in a representative storm gap → rendered review → frame-time check.

## 3. Brief — Fallen crown / top debris (optional, Option B)

- **State:** crown of a windthrown Sitka lying on the ground; green (0–1 yr), red-brown/grey (1–3 yr).
- **Purpose:** makes a fresh windthrow read as a whole tree without rotating a 40k-triangle living model.
- **Scale:** 6–10 m long, 3–5 m wide footprint; pivot at the stem attachment point.
- **Variants:** green, dying (two material states on one mesh).
- **LOD:** ≤ 8k / 3k / 800 tris; alpha-clipped needle cards (0.38, as other Sitka foliage).
- **Unity:** URP/Lit, Render Face Both for cards. No collider.
- **Boundary:** visual only; no habitat or regeneration effect.
- **Acceptance:** as §2.

## 4. Brief — Root-plate soil pit / disturbed ground (NEEDED)

- **State:** the hollow left by an uprooted root plate; fresh soil (0–2 yr) → vegetated/moss (3–10 yr).
- **Purpose:** connects the root plate to the ground; the plate currently "floats" (README: no soil pit, terrain not cut).
- **Scale:** 1.5–3 m oval, depth ≤ 0.4 m (visual).
- **Implementation options:** (a) a decal projector with soil/needle albedo (cheapest); (b) a low-poly pit mesh (≤ 600 tris) with a soil material reusing `Forest_RootPlate_Soil`.
- **Variants:** fresh and greened.
- **LOD:** a decal needs none; a mesh 2 LODs.
- **Biological boundary:** v1 has **no regeneration microsite effect** (pit/mound recruitment is a future ecology integration point, see `DecisionMatrix.md`).
- **Acceptance:** placed with a root plate on sloped terrain without visible floating at walking height.

## 5. Asset / mechanic matrix (packet item 20)

| Storm state | Simulation state | Visual required | Current asset | Gap | Priority |
|---|---|---|---|---|---|
| Windthrown, fresh (0–1 yr) | `mortalityCause=windthrow`, deadwood record DecayClass 0 | fallen stem + green crown + root plate + pit | log (stretched), root plate fresh | crown (A: code reuse), pit, base anchoring | **BLOCKER:** base-anchored directional placement + root plate. **NEEDED:** pit, crown handling |
| Windthrown, dying (1–3 yr) | same, DecayClass 0–1 | stem + grey crown + plate | log, plate | crown tint (code) | NEEDED |
| Windthrown, weathered (4+ yr) | DecayClass ≥ 2 | decayed log + weathered plate + moss | decayed log, weathered plate, moss/fungi dressing | none | — |
| Salvaged | record removed; event | brash + (optional) root plate remains | brash, plate | none | — |
| Storm gap | cell light ↑, `RecentOpening` ↑ | open canopy (emergent) | — | none | — |
| Snapped (v1.1) | `windsnap` + snag record | snag + fallen top | none | snag asset, standing-deadwood state | deferred |
| Stability "exposed" | derived V band | none in world (UI only); optional map layer | — | — | POLISH |
| Long fallen stems | — | segmented stem | log | long-stem asset | POLISH |
