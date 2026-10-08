# Player-facing asset gaps that affect comprehension (Workstream W)

**Status:** focused cross-reference. Consumes Sol's audit on `task/understorey-recruitment-causality` (`Docs/Research/ScenarioOneAssets/`: `AssetGapRegister.md`, `AssetMechanicMatrix.md`, `ContinuationPriorities.md`, ten `CreationPackets/`) and the storm asset requirements (`task/windthrow-readiness`). **Does not redo the inventory. No asset was created or inspected in Unity.**

Rule (D-020): a visual may show only authoritative state.

## 1. Comprehension-critical assets against the tutorial progression

Stage IDs from `ObjectiveRedesign.md`.

| Visual need | Stage(s) where comprehension depends on it | Simulation state | Sol's status | Gap for comprehension | Priority |
|---|---|---|---|---|---|
| **Young tree vs woody understorey** | S7 diagnose regeneration; S8 planting; S10 clearance | Regeneration bands, exact juveniles; habitat shrub cover | **P0 validation**: the manager reported tree-vs-shrub confusion; the beech double-height bug is fixed on the M2 branch (not on main) | A beginner cannot "find young trees" if seedlings look like shrubs. Packet 01 (juvenile vs woody legibility) is a **prerequisite for S7** | **BLOCKER for S7/S10 lessons** |
| **Bramble vs bracken** | S10 (Model 2) | M2 `brambleCover`/`brackenCover` (identical trajectories in fresh games) | Meshes exist; "recognizable as authoritative category: **unverified**"; mixed habitat palette not yet driven by competitor state (M2 now drives it on its branch) | The player must recognise *dense competitor cover* vs ordinary ferns/herbs, or clearance teaching fails. Distinguishing bramble from bracken matters less, since the model treats them alike | **IMPORTANT** (with M2) |
| **Deadwood** | Habitat dimension; deadwood choice; self-thinning deaths | Records, decay class | B/C; repetition and silhouette variation (packet 05) | Self-thinning logs and deliberately left logs look the same. The diary/review distinguishes them in text. Visual sameness is acceptable | POLISH |
| **Windthrow** | Storm lessons (after storms) | `windthrow` cause, records, storm events | Root plates exist (fresh/weathered); pit, fallen crown and base-anchored direction needed; "Do not create yet" list includes windthrow sets until mechanics are accepted | Without directional fallen stems and root plates, a storm gap reads as a thinning gap. **Storm teaching depends on it** | BLOCKER *for storms* (not before) |
| **Crop / Fell markings** | S3, S4 | Marks | B; contrast validation (packet 07) | Must read at 20–30 m in shade and in bright clearings. The P2 neighbour tags must not be confused with marks | IMPORTANT |
| **Pruning** | S11 | Lifts, crown base | Branch-state models B; LOD legibility validation | A lift must be visible on the inspected tree from walking distance, or the lesson is text-only | POLISH (pruning is optional) |
| **Shelters** | S9 | Shelter lifecycle | B; visibility contrast check | Expired vs effective shelters are not visually distinct (chip text only) | POLISH |
| **Clearance aftermath** | S10, history revisits | Footprint, `lastUpdated` | B/C; cut-vegetation remnants (packet 04) | In the treatment year the area is bare. In later years recovery is gradual. Without remnants a revisit cannot tell "cleared 3 years ago" from "never vegetated". The text history covers it (MapHistoryDesign) | IMPORTANT (M2) |
| **Light / openings** | S1, S4, S6 | Canopy | Emergent | Fine | — |
| **Sitka life stages** | S6–S12 | Growth Model 1 heights | Packet 02 (stage/scale validation) | Natural Sitka recruits must look like young Sitka, not a different species | IMPORTANT |

## 2. Recommended order (comprehension view)

1. **Packet 01** juvenile vs woody understorey legibility. Before beginner playtest 1, because S7 depends on it. Reuse existing meshes first (Sol's own advice), plus the beech scale fix when M2 integrates.
2. **Packet 07** marking contrast. Before playtest 1 (S3/S4).
3. **Packet 03** competitor categories. With M2 integration.
4. **Packet 04** clearance remnants. With M2.
5. **Packet 02** Sitka life-stage validation. Before the second intervention lands.
6. Storm minimum visual package (`StormAssetRequirements.md`). Within the storm packet.
7. Packets 05, 06, 08, 09, 10. Polish / performance.

## 3. Acceptance for comprehension (not just import)

For each item above, a **recognition check** in the beginner playtest: "Show me a young tree here", "Which plants could smother young trees?", "Which of these trees did you mark to keep?". Passing a numerical harness does not establish recognition (Sol's own caveat).
