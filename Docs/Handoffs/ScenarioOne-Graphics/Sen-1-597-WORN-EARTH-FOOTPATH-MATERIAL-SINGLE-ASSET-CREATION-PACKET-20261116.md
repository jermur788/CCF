# Sen-1 — Single-asset creation: Compact30 Worn-Earth Footpath Material (review candidate)

**Issued:** 16 November 2026 by Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424), GPT-6, GitHub/browser only.  
**Execution claim:** [#597](https://github.com/jermur788/CCF/issues/597) **READY / NOT RUNNING**. No actual artist is registered or active, and no physical machine/Unity/editor lease is claimed by this packet.
**Difficulty:** **MEDIUM** — isolated, original procedural material plus reproducible provenance and test renders, but no gameplay C#, scene architecture, planting or protective-guard physics.  
**Recommended worker:** **Claude Sonnet 5.5 High in Claude Code**, with local procedural/Blender or other offline materials tooling; Claude Opus 5.5 High is a capable alternative if the local source author needs extensive look development. Actual worker, provider, effort and installed Blender version must be registered and observed rather than assumed.

## 0. Why this is the single best first asset

**Produce one reviewable material asset candidate**, internally named `Sen1_Compact_WornEarth_Footpath_Mat01`: naturally weathered **Irish Sitka woodland pedestrian path** surface — desaturated brown-grey compressed mineral earth, subtle fine grit, pressed-in spruce needles, irregular damp patches and moderate roughness. It should visually remain a **narrow footpath**, not a forestry rack, gravel road, paved parking apron or artificial geometric strip.

**Direct user has APPROVED this W worn-earth appearance** in #593 (along with a separate Compact30 scene, initial ~1.2m walking-core target subject to actual guard clearance, realistic optional mesh/vented guards, no fresh H1 stumps, minimal border scenery, neutral daylight). The material's *look direction* is therefore settled, unlike final path mesh/collider, planted-G1 locations, guard failure pose, native Unity lighting or legal asset storage.

Compared with another new Oak/Beech or guard mesh or full path/apron geometry, an **isolated swatch/tile material** avoids:
- #588/#590 **GQ-07** old 2.6m historical planks overlapping Route A; no new scene required to make or review this one swatch.
- **GQ-02** current failed shelter tilting 28° and translated +0.25 m visually crosses the proposed path at Oak O1; this task does not create or move a shelter or fix the path.
- Forest ecology/source collisions, Work Plan, Stand Map, old save states, 14 Sitka preworld omissions, 2 Oak+2 Beech placement or 1.2m path engineering; no need to author those values here.
- Unverified third-party Ultimate Nature and rejected/unadopted `ForestFloorV1` asset provenance.

This is **higher-probability preproduction art/provenance review**, **NOT** a claim that it will pass a later Unity feature review. The final material cannot be **production approved/imported** while D-032 binary storage/licensing and graphics/parent specs are unapproved.

## 1. Source authority and prior art to read (read-only)

1. [#593 direct-owner ratified graphics decision](https://github.com/jermur788/CCF/blob/docs/sen-1-593-owner-compact-look-scene-ratification-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-593-OWNER-APPROVED-COMPACT-LOOK-SCENE-SPEC-AMENDMENT-RESULT-20261116.md): W path, separate scene, neutral daylight; exact materials not yet visually accepted in Unity.
2. [Latest four-file Compact30 graphics **DRAFT**](https://github.com/jermur788/CCF/blob/docs/sen-1-593-owner-compact-look-scene-ratification-20261116-v1/specs/scenario-one-compact-graphics-v1/spec.md): notably FR-07 surfaces, FR-08 material isolation, FR-12 source/asset provenance, FR-13 compact-only P0, FR-16 owner W style, **FR-18 D-032 OPEN**, AC-03/05/07 and GQ-01/03/07. Spec remains **DRAFT / NOT APPROVED**; do not use it as authority to integrate production assets.
3. [#577 Opus first-party reuse inventory and original Blender scratch W/N/N2 comparisons](https://github.com/jermur788/CCF/blob/docs/sen-1-compact-asset-lookdev-opus55-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-OPUS55-COMPACT-ASSET-PREPRODUCTION-RESULT-20261116.md) §§3–5: W is desaturated brown-grey earth, fine grit, needles; original scratch W ~1.1m worn core and **0.35m soft edge**. **The 0.35m edge is NOT an authorised geometry setting** and is likely too broad near G1, so no path geometry creation from this result. W was reportedly more readable than N/N2 in scratch grayscale, but values were only Blender-derived.
4. [#573 graphics coupling review](https://github.com/jermur788/CCF/blob/review/sen-1-ground-graphics-spec-independent-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-G01-G02-INDEPENDENT-GRAPHICS-SPEC-AND-REFERENCE-REVIEW-20261116.md): the old `Forest Ground.mat`, `Forest Path.mat`, `ScenarioOneFloorPresentationSetup` and historical plank/headland clones are **protected**. Neither material may be edited or used as a mutable base; new asset needs its own original source.
5. `main:Docs/Project/decision-log.md` D-032 is **OPEN**, no accepted LFS/binary/source archiving and no implicit licence or size budget. Old first-party source art without explicit licensing statement must not be assumed redistributable.

## 2. Production boundary (critical)

**Phase A is authorised as SCRATCH ART PREPRODUCTION only**: make a **real new original, editable, testable standalone procedural material** and review renders **outside Git and Unity**. This authorisation arises from the user's direct **request to have one asset created**, but does NOT approve production import, binary commits, Unity execution or release.

- Create offline original material authoring source in a **dedicated personal scratch/evidence folder**. Proposed exact path, contingent on confirming it is free and writable:  
  `/media/jer/Files/CCF/Verification/Sen-1-597-WornEarthMaterial-20261116/`  
  Contents: original editable shader/procedural generator (`.blend` and/or script), baked **review-only** base-colour, normal and roughness/packed maps where feasible, repeat test renders, metadata/provenance and SHA256SUMS. If path conflicts or is not available, use a **new unique path** under that Verification root and record the actual path.
- Optionally reuse the **creative procedural design approach** from #577's original source after inspecting the exact provenance; **do not** copy any image, third-party content, forest-floor candidate, a Unity/shared material, or somebody else's unverified texture. Do not confuse the first #577 Blender illustration with a production texture source.
- **No Git-tracked binary**: absolutely no `.blend`, `.png`, `.jpg`, `.fbx`, `.mat`, `.asset`, `.prefab`, `.unity` or image/texture files in Git. `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, `specs/`, signed save, user Prefs and all original art folders are READ ONLY. No Unity Editor/Library/import/Windows build; no production shader/C# or asset-store content.
- Worker may not set up Git LFS, decide D-032, claim licence approval, or modify any live `CCF-Ops/` or `main:Docs/Project/coordination/active-tasks.md`. This work does not reserve the Unity machine-wide lease.
- Never change historical old `Forest Ground.mat` or `Forest Path.mat`, their shared soil texture, old planks or headland/approach renderer/material templates.
- **This is one material asset only**. Do not add a path mesh, footpath geometry, H1 apron, generic floor, vegetation placement, guard, Oak/Beech, sky/fog or new game UI.

## 3. Worker start, lock and exact Git handoff

1. Read `main:Docs/Project/coordination/active-tasks.md` READ ONLY and GitHub #597, #424, #594, then inspect actual local worktrees/dirty state and machine access. Primary `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` have historically had unrelated local edits — do not touch or clean/discard them. If another owner has the desired scratch path/file set, STOP and coordinate instead of overwriting.
2. Use dedicated docs branch **`docs/sen-1-597-worn-earth-material-artist-result-20261116-v1`**, from the **latest checked `main` HEAD**. Check branch not already occupied. Suggested exclusive local worktree if permitted and empty:  
   `/media/jer/Files/CCF/Operations/CCF-597-Sen1-WornEarthMaterial-20261116`.  
   Do not move another worker's worktree or create second writers in it.
3. **Before making or editing tracked source/evidence**, update GitHub issue #597 to **[RUNNING]** with actual provider/model/effort (if visible), branch, worktree path, exact starting HEAD, actual scratch folder and one tracked write allowlist. If issue-comment capability truly unavailable, **STOP before tracked work and request the Manager to register**; do not silently substitute a later assertion.
4. Only Git-tracked output is ONE NEW Markdown result:  
   `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-597-WORN-EARTH-FOOTPATH-MATERIAL-SCRATCH-ASSET-RESULT-20261116.md`  
   (do not alter this packet, the source/spec or any old docs). Commit only this one result to the dedicated branch, read it back and compare the author starting HEAD to final: exactly **one added Markdown**, no binary and no other tracked diff. If an offline script itself is only in the scratch evidence folder, record its sha and reproducibility command in result.
5. At finish update #597 **[HANDED-OFF]**, comment exact direct GitHub result URL and close it. Notify #424 with the result, honest art/provenance findings and any missing input. A separate nonauthor **source-art review** will be dispatched after the result, not assumed by the artist.

## 4. The actual creative deliverable: one original, repeatable surface material

Build **`Sen1_Compact_WornEarth_Footpath_Mat01`** as a **material-source asset**, not a path mesh:
- **Appearance:** low-saturation brown-grey/olive mineral earth from foot traffic with fine granular soil and occasional **pressed-in Sitka needles**, modest irregular compaction and local damp/dry mottling. Subtle normal variation; avoid large pebbles, gravel road aggregate, railway/rack timbers, tyre ruts, bright orange soil, painted edges or fake footprints. It should complement the pre-existing Sitka stand, not resemble an asphalt path.
- **Procedural/original source:** Blender 4.x shader nodes or offline procedural code with named, editable parameters (seed, grain scale, needle density, colour range, roughness range, normal strength, dampness). **Record actual licence/author/provenance** as newly written original content and list any built-in library dependencies; never claim that a third-party source is licensed without reading its terms. For a **single material** it is acceptable that a baking/export test produces multiple channels of the same set.
- **Maps:** if feasible, bake one review-only tileable set (base colour/albedo, tangent-space normal, roughness), at a source-author-selected **small test resolution, e.g. 1024²**, with consistent UV and neutral physical scale; not an approved byte/material budget. If baking is unavailable, retain a functioning editable procedural source and document the missing map/export as a **review defect**, not a delivered baked asset.
- **No scene dependence:** it must apply to a flat plane in Blender (or an offline swatch viewer) and be **tileable** without requiring the actual Route A spline, 1.2 m collider, H1 polygon, player camera or terrain. Test repeat over a **3×3 tile grid** under neutral diffuse/overcast light to expose seams and repetitive blotches. Do not author a 0.35m or 0.20m wide path edge into a texture; core width/soft fringe belong to the future source geometry/surface shader and cannot be solved by this swatch.
- **Review images:** at minimum one close flat colour tile, one **3×3 repeating tile** with obvious join positions, one angle-view neutral/diffuse lighting shot, one **grayscale comparison** against a clearly labelled **illustrative** dark Sitka needle-litter reference surface (never claim it is actual Unity `Forest Ground.mat` or a source-grounded real frame), and a channel sheet showing base colour/normal/roughness or state why omitted. Render a low-light/wet response **only if cheap and visually useful**, with honest calibration limits.
- **Readable but natural:** W path was chosen because player wayfinding matters. Check grayscale value separation, but **do not declare 1.13 a mandatory physical or perceptual ratio**; previous #577 value was one scratch-screen metric. Minimise obvious periodic details and macro texture repetition in a tiny 30m woodland.
- **Performance/provenance disclosure:** record approximate source bytes/preview map bytes and map resolutions, number of textures/shader nodes, procedural time and reproducibility command, but label Unity VRAM/URP performance **NOT MEASURED**. Do not invent an acceptance budget or renderer compatibility.

## 5. Review gate and pass/fail rubric (preproduction only)

This first review is **P0 source-art/readability/provenance**, not Unity asset integration. A nonauthor graphics reviewer can issue **APPROVED AS SCRATCH MATERIAL CANDIDATE**, **NEEDS CHANGES**, or **REJECTED** based on:
- **Art:** W visibly reads as a naturally walked compact-earth surface, not a vehicle rack/road; restrained Sitka needle/grit/damp variation, coherent with neutral daylight. No false site history or forestry practice claim.
- **Tiling:** 3×3 repeat shows no obvious seams/discontinuities and acceptable repetition under neutral-view and grayscale; source can be rerun for same seed at same software version.
- **Single-asset scope:** no mesh/collider, G1 tree/shelter, H1 apron, global sky/fog/scene, third-party assets or production code changes.
- **Provenance:** source-generator original (with author declaration), real editable files, actual hashes and exact scratch path, no unlicensed external sources, no unexplained copied texture.
- **Technical honesty:** map output list complete or explicitly failed, no fabricated Unity screenshot, performance or legal/storage clearance, no D-032/APPROVED spec claim, no changes to source files.
- **Issue and Git:** actual registered worker, clean branch, GitHub result readback and exactly one tracked Markdown. Distinguish local scratch creation from durable GitHub handoff.

**Reviewer's approval does NOT permit** importing it to Unity, distributing source, committing any binary or marking graphics spec APPROVED. Later production adoption requires accepted D-032, licensed exact source, approved Compact graphics and separate parent scene bridge, signed world/material integration allowance, own Unity lane and native visual/LOD/performance/Legacy40/E80/Reference verification.

## 6. Required result contents

At the exact `Sen-1-597-WORN-EARTH-FOOTPATH-MATERIAL-SCRATCH-ASSET-RESULT-20261116.md` path publish:
1. Actual worker identity/claim registration, branch/worktree/starting head, scratch path, tools and whether Blender was really executed.
2. A table of exact created **offline scratch files** with short purpose, bytes and SHA-256; link local provenance/SHA256SUMS and deterministic build/replay commands; if not actually created, **report BLOCKED/NOT CREATED rather than pretend**.
3. Description plus exact repeat/seam/review render results and failures, including the negative-control grayscale; no future-imagined screenshots.
4. Limits: material only, Blender scratch not native, not on final H1/Route A/Unity, source-first only, protected shared mats unchanged, D-032 OPEN, licences and future source-archiving policy unresolved.
5. Explicit nonauthor next review recommendation and **no production/Unity GO**, with original #577 source references.
6. GitHub diff/readback from own source starting head: one new Markdown only; issue [HANDED-OFF]/closed, dashboard/Drive **NOT VERIFIED**.

## 7. Dispatch text (paste into the local Claude asset worker chat)

> Take **CCF Scenario One #597** as the **single-asset material artist**, using Claude Sonnet 5.5 High in Claude Code with local Blender/procedural tools. Follow the COMPLETE Manager GitHub handoff: https://github.com/jermur788/CCF/blob/docs/sen-1-597-worn-earth-material-scratch-handoff-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-597-WORN-EARTH-FOOTPATH-MATERIAL-SINGLE-ASSET-CREATION-PACKET-20261116.md . The owner approved the **W worn-earth pedestrian path**. Register your actual model, exclusive new documentation branch/worktree, starting HEAD, scratch evidence folder and exact one-result Markdown allowlist on #597 BEFORE editing. Create **ONE REAL, ORIGINAL, EDITABLE REPEATABLE worn-earth footpath MATERIAL** (procedural source, baked review-only maps where possible, seam/grayscale/neutral lighting renders and provenance/hashes) **OUTSIDE GIT**. Publish the exact one-file GitHub result with evidence and mark [HANDED-OFF]/close #597. Do **not** make the footpath mesh, H1 apron, shelters, other assets, Unity files or game code. Do **not** import/commit binary art or edit `Assets/`, shared `Forest Ground.mat`/`Forest Path.mat`, project specs, old scenes, saves or Prefs; **D-032 is OPEN and graphics spec DRAFT**. No Windows build. Return the GitHub result for a separate nonauthor scratch-asset review.

**User action:** paste the dispatch into Claude Code/local art worker. **No further design decision needed for W style**.  
**Dashboard/Drive:** NOT VERIFIED.
