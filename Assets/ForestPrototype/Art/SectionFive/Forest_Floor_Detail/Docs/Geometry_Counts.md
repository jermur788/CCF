# Geometry counts — Forest Floor Detail

Final reimport-validated revision: `2026-09-29_reviewed_v2`. Dimensions are mesh bounds in metres, not placement or gameplay rules.

| Asset | LOD | Meshes | Triangles | X × Y × Z (m) |
|---|---:|---:|---:|---|
| Oak_LeafLitter_Patch_01 | 0 | 1 | 900 | 1.4598 × 1.1819 × 0.0201 |
| Oak_LeafLitter_Patch_01 | 1 | 1 | 600 | 1.4598 × 1.1819 × 0.0207 |
| Oak_LeafLitter_Patch_01 | 2 | 1 | 200 | 1.4283 × 1.1926 × 0.0145 |
| Beech_LeafLitter_Patch_01 | 0 | 1 | 900 | 1.4790 × 1.1622 × 0.0199 |
| Beech_LeafLitter_Patch_01 | 1 | 1 | 600 | 1.4790 × 1.1622 × 0.0202 |
| Beech_LeafLitter_Patch_01 | 2 | 1 | 200 | 1.4891 × 1.1426 × 0.0149 |
| Mixed_LeafLitter_Patch_01 | 0 | 1 | 900 | 1.4788 × 1.1310 × 0.0197 |
| Mixed_LeafLitter_Patch_01 | 1 | 1 | 600 | 1.4788 × 1.1310 × 0.0204 |
| Mixed_LeafLitter_Patch_01 | 2 | 1 | 200 | 1.4393 × 1.1404 × 0.0148 |
| Small_Deadwood_Scatter_01 | 0 | 1 | 1,716 | 1.8210 × 1.5725 × 0.0485 |
| Small_Deadwood_Scatter_01 | 1 | 1 | 1,236 | 1.8210 × 1.5726 × 0.0484 |
| Small_Deadwood_Scatter_01 | 2 | 1 | 272 | 1.6369 × 1.3661 × 0.0470 |
| Small_Deadwood_Scatter_02 | 0 | 1 | 1,716 | 1.9606 × 1.7963 × 0.0488 |
| Small_Deadwood_Scatter_02 | 1 | 1 | 1,236 | 1.9606 × 1.7963 × 0.0488 |
| Small_Deadwood_Scatter_02 | 2 | 1 | 272 | 1.9556 × 1.3733 × 0.0425 |

## Totals

- One instance of each of the five assets at LOD0: **6,132 triangles**, five meshes.
- One instance of each of the five assets at LOD1: **4,272 triangles**, five meshes.
- One instance of each of the five assets at LOD2: **1,144 triangles**, five meshes.

Leaf LOD0/1/2 retain 150/150/100 cards. Twig LOD0/1 retain 24 twig systems, two side branches per system and nine bark fragments; LOD2 retains 16 twig systems, one side branch per system and four fragments.

These counts exclude preview ground, text, cameras and lights. Triangle counts do not describe alpha overdraw, draw calls, GPU cost or engine performance. Each export uses one shared material; transparent card area still incurs alpha-test work.
