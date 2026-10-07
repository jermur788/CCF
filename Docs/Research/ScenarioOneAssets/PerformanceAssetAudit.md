# Rendering cost audit

Simulation timings are separate in UnderstoreyRecruitment/Performance.md. Imported geometry suggests WATCH/OPTIMIZE SOON targets but no measured GPU bottleneck or release blocker is established.

| Family | Actual inventory finding | Classification / action |
|---|---|---|
| Mature beech | ~529,022 triangles, five material slots, no LOD | OPTIMIZE SOON: profile near/far instances and author LOD chain if confirmed in active path |
| Mature oak | ~607,328 LOD0 source; prefab ~812,946 summed LODs | OPTIMIZE SOON: high-cost foliage; measure active LOD ranges, reduce silhouette-preserving geometry |
| Pruned mature oak | ~989,319 all-LOD triangles / 21 summed slots | WATCH/OPTIMIZE SOON: do not mistake total for per-frame triangles |
| Moss shoot mat | ~59,954 all-LOD triangles, 14 summed slots; cushion placement reuse | WATCH: many ground instances may cost more than distant hero tree |
| Transparent foliage | Many layered canopy/ground meshes | WATCH: overdraw/shadows/material passes need profiler evidence |
| Deadwood accumulation | Thousands of records possible in long simulations; small log meshes | WATCH: visible count/distance/dressing/collision cost, not just record count |
| Sitka initial plantation | Runtime generated/controller variants can import with zero geometry measurement | WATCH: generated geometry must be measured at runtime; zero inventory does not mean free |
| Texture sizes | CSV includes imported dimensions and max-size limits | WATCH where oversized for small ground plants; no blanket downsize without screen-space check |
| Current interaction colliders | Runtime tree colliders separate from art prefab metadata | NO proven issue; measure actual collider counts before optimisation |

Initial runtime has 4,157 Renderer components including disabled LOD children; this is not 4,157 draw calls. Material slots sum hierarchies. No unique-material clone count, instancing/batching pass, GPU overdraw benchmark or profiled populated 100-year forest was completed. Do not classify 'BLOCKER' based solely on a file polycount.

Future benchmark: fixed camera/quality/resolution, warmed shaders, dense plantation/thinned/juvenile/high-ground-cover/100+ deadwood scenes; three repeat samples of CPU/GPU/frame median and tail; enabled LOD triangles/material passes/shadows; compare exact same saved world before/after. Start with beech/oak, reuse shared materials/atlases, disable unnecessary distant ground shadows and keep simple colliders where gameplay permits. No blind mesh/material rewrite now.
