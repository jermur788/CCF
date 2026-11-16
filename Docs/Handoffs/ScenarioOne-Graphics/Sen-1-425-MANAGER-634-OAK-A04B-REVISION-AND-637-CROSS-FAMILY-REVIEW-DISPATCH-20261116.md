# Sen-1 — Overall Manager verdict on Oak A-04b #634; independent review #637

**Date:** 16 November 2026. **Overall Manager:** GPT-6, browser/GitHub. Inspected [exact #634 GitHub result](https://github.com/jermur788/CCF/blob/docs/sen-1-634-oak-a04b-revision-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-634-OAK-A04B-SCRATCH-REVISION-AND-REPRODUCIBILITY-RESULT-20261116.md), its issue, one-added-Markdown compare against main. No actual Blender source/renders mounted in browser; therefore the author-provided measurements are **reported evidence, not independent Manager measurements**.

## Manager verdict

**APPROVED #634 AS COMPLETED BOUNDED SCRATCH REVISION WORK / ART QUALITY NOT YET REVIEWED.** The original #627 35/35 manifest was reported unchanged; A-04b author reports three OBJ meshes/MTL/stats byte-identical on rebuild, no Unity, zero shared Blender GUI use, and task-specific temp/config. No binaries were committed. Status remains **SCRATCH AUTHORED, not REVIEWED, INTEGRATED, VERIFIED or ACCEPTED**; owner has not selected production stock type.

Reported changes directly address #632's two major and seven minor findings:
- **M-1** provisional **1+1 bare-root** two-season structure: older wood 0–0.24m, scar, current shoot to 0.600m. This is **reversible art reference only**; UK research basis, not binding Irish nursery or CCF gameplay policy.
- **M-2** actual minimum effective visible collar >11mm in all three LODs, derived from polygon cross sections, not nominal radius. UK published minimum does not constitute Irish statutory standard.
- Leaf envelope mean peak 0.535 rather than 0.669, 5–7 shallow lobes, wedge bases, petioles; 29 leaves after clearance. Author's mesh checker reports 0 detached/intersecting leaves across LODs, **6,044 / 1,196 / 311 triangles**; more useful LOD1 and connected low-poly LOD2.
- **Correction to #632 numeric method:** old A-04a floating leaves measured by new vertex-to-triangle method yielded 29/30 detached with up to 13.75mm gap, not old vertex-vertex 45mm; the defect itself was real. Independent reviewer should check method rather than repeat disproved measurement.
- Remaining: LOD1 toothy/zig-zag lobe outline, occasional front orthographic edge-on leaves, no textured venation under D-032, no Unity 720p/1080p evidence or two-sided URP import. These remain explicit, not silently accepted.

**Safety:** reports five fully headless Blender runs with isolated TMPDIR/BLENDER_USER paths, no `/tmp/quit.blend` before/after, no desktop focus, no Unity lease, installs, original scratch mutation or source changes. This addresses earlier observed recovery-file/process risk within **headless** scope; GUI-mode safety still unqualified.

## Next step

**[#637](https://github.com/jermur788/CCF/issues/637), READY / NOT RUNNING**, independent *non-Claude* changed-scope review. **Difficulty MEDIUM:** direct local source/meshes/10 renders, biological and geometric remeasurement, no new broad research. **Best suited:** GPT-6.1 Sol High or Codex High on actual Linux T3 Code as nonauthor/different provider family. **Proposed branch** `review/sen-1-637-oak-a04b-cross-family-delta-20261116-v1` from refreshed main. **Exact GitHub result** `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-637-INDEPENDENT-A04B-OAK-SCRATCH-CHANGED-SCOPE-REVIEW-20261116.md`. Actual local read-only review worktree/branch/start HEAD must be registered by worker; never infer physical ownership.

The task checks all #632 changed findings on actual local A-04a/A-04b art, measures root-collar/leaf attachment with correct geometry and returns **APPROVED / NEEDS CHANGES / REJECTED for SCRATCH QUALITY ONLY**. No asset edits, GUI, Unity Editor/lease, build, binary Git, direct adoption or D-032 approval. Original A-04a/A-04b directories remain immutable, new review receipt separate.

**Paste-ready Linux dispatch:**

Take CCF Scenario One **#637**, full GitHub handoff https://github.com/jermur788/CCF/issues/637 . As an independent **non-Claude** GPT-6.1 Sol High or Codex High Linux T3 Code reviewer, examine #627 A-04a and #634 A-04b original scripts, meshes, checks and 10 renders in local read-only scratch. Recheck #632 two major/seven minor fixes, especially 1+1 reference, root-collar caliper, sessile Oak leaf silhouette and floating LOD2 correction. Register actual model, review branch/worktree/starting HEAD, publish precisely one GitHub Markdown verdict. No Blender GUI, asset changes, Unity import/lease, binaries in Git or Windows build.

**Future adoption boundaries:** Production stock policy/asset D-032/asset-only #619 remain undecided. An approved native Linux Unity lane/lease is required for 720p/1080p gameplay readability and two-sided leaves. No Windows Player build. Dashboard/Drive not verified.
