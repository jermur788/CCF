# Deadwood readiness

Growth1 mortality creates one saved deadwood record per biological tree death, using the same falling-log/decay path as retained deadwood; the growth gate verifies once-only creation. Species, original height/diameter, volume, location, fallen year and decay year drive the current representation. Deadwood records are not necessarily simultaneously visible log objects; do not count ledger rows as draw calls.

Actual assets: SS_Log_Fresh_01 (1,004 all-LOD triangles), SS_Log_Decayed_01 (2,329), SS_Stump_01 (976), small scatter variants (3,224), moss dressing and bracket/mushroom clusters. Shared scale/orientation/decay reuse is cheaper than hundreds of unique meshes. Current broadleaf records can use a generic log; a distinctive broadleaf bark/cut end is an improvement, not a necessary new species mechanic.

Tier B functional, C repetition risk. Before creating: stage tens then hundreds of current logs at realistic record sizes, report enabled/rendered object and triangle counts, GPU/CPU frame times, overdraw/shadows/collisions and player visibility. Two or three genuinely different trunk silhouettes (straight/tapered/curved, no storm-uprooted base unless mechanic accepted) plus shared bark/cut-end atlas can materially improve repeated views. Random scale must preserve record diameter/length bounds; variation cannot falsify biological dimensions.

Collision should match existing player interaction and path access, avoiding per-fragment MeshColliders. Keep moss/fungi dressing bounded by count/distance and decay state. Verify fresh→decayed save/load representation and no duplicate visual on rebuild. No hundreds-of-logs render performance pass is claimed by simulation-only benchmarks.
