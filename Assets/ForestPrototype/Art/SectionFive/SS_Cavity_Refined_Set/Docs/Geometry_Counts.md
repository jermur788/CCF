# Geometry counts — SS_Cavity_Refined_Set

Final independently reimported exports. Dimensions are authored mesh bounds in metres; targets/labels have no gameplay meaning.

| Stage | LOD | Meshes | Triangles | X × Y × Z (m) |
|---|---:|---:|---:|---|
| Young | 0 | 3 | 23,336 | 3.3925 × 3.5787 × 11.0000 |
| Young | 1 | 3 | 10,664 | 3.3925 × 3.5787 × 11.0000 |
| Young | 2 | 3 | 3,828 | 3.7606 × 3.9267 × 11.0000 |
| Mature | 0 | 3 | 43,868 | 7.0120 × 6.9189 × 26.0000 |
| Mature | 1 | 3 | 19,842 | 7.0120 × 6.9189 × 26.0000 |
| Mature | 2 | 3 | 6,998 | 7.6250 × 7.6090 × 26.0000 |
| PostMature | 0 | 3 | 46,620 | 8.3379 × 8.5807 × 31.0000 |
| PostMature | 1 | 3 | 21,064 | 8.3379 × 8.5807 × 31.0000 |
| PostMature | 2 | 3 | 7,414 | 8.9115 × 9.3914 × 31.0000 |

Each FBX contains three meshes and one root. Bounds include foliage and the authored bend offset where applicable. They exclude all staged preview objects.

LOD1 keeps primary branch identities while reducing branch radial detail and card subdivisions. LOD2 simplifies wood and uses fewer/larger foliage clusters. Alpha-test overdraw and engine cost are not represented by triangle counts.

## Exported cavity centre probes

| Stage | LOD | Recession (m) | Remaining backing (m) |
|---|---:|---:|---:|
| Young | 0 | 0.05500 | 0.14019 |
| Young | 1 | 0.05500 | 0.13927 |
| Young | 2 | 0.05500 | 0.15584 |
| Mature | 0 | 0.16000 | 0.30835 |
| Mature | 1 | 0.16000 | 0.30600 |
| Mature | 2 | 0.16000 | 0.32163 |
| PostMature | 0 | 0.23000 | 0.44566 |
| PostMature | 1 | 0.23000 | 0.44229 |
| PostMature | 2 | 0.23000 | 0.46520 |

Five separate recess probes passed per export. Rear surfaces remain unchanged. The connected cavity-bearing stem component is closed; the raised callus overlay has intentional open borders.
