# Geometry counts — SS_Bent_Refined_Set

Final independently reimported exports. Dimensions are authored mesh bounds in metres; targets/labels have no gameplay meaning.

| Stage | LOD | Meshes | Triangles | X × Y × Z (m) |
|---|---:|---:|---:|---|
| Young | 0 | 3 | 24,082 | 3.7841 × 3.5388 × 11.0000 |
| Young | 1 | 3 | 10,822 | 3.7841 × 3.5388 × 11.0000 |
| Young | 2 | 3 | 3,730 | 4.1814 × 3.9986 × 11.0000 |
| Mature | 0 | 3 | 41,182 | 7.7639 × 6.6893 × 26.0000 |
| Mature | 1 | 3 | 18,422 | 7.7639 × 6.6893 × 26.0000 |
| Mature | 2 | 3 | 6,330 | 8.0363 × 7.3807 × 26.0000 |
| PostMature | 0 | 3 | 45,970 | 10.1607 × 8.6644 × 31.0000 |
| PostMature | 1 | 3 | 20,550 | 10.1607 × 8.6644 × 31.0000 |
| PostMature | 2 | 3 | 7,058 | 10.6564 × 9.4842 × 31.0000 |

Each FBX contains three meshes and one root. Bounds include foliage and the authored bend offset where applicable. They exclude all staged preview objects.

LOD1 keeps primary branch identities while reducing branch radial detail and card subdivisions. LOD2 simplifies wood and uses fewer/larger foliage clusters. Alpha-test overdraw and engine cost are not represented by triangle counts.

## Authored bend offsets

| Stage | Leader X offset (m) | Maximum centreline X offset (m) |
|---|---:|---:|
| Young | 0.8113 | 1.1401 |
| Mature | 2.4250 | 3.5208 |
| PostMature | 3.2259 | 4.6745 |

Five common centreline stations and the leader offset passed independent FBX measurements at all LODs, with cross-LOD agreement checked.
