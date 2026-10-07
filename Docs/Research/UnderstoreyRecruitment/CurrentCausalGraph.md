# Current causal graph — source verified

Base/context: StartRecord.md. Runtime reproduction passed for 48 matched cases; the dashed links remain absent in unchanged production.

```mermaid
flowchart LR
 A[Adult crowns] --> L[Cell canopy/light]
 L --> E[Seed-derived establishment]
 S[Seed source and mast] --> E
 U[Establishment suitability] --> E
 E --> R[Regeneration age bands and accumulators]
 L --> G[Juvenile growth and survival]
 B[Browse pressure] --> G
 P[Protection access] --> G
 G --> T[Promotion]
 L --> H[Habitat cover targets]
 U --> H
 O[Recent opening/site/soil] --> H
 H --> C[Persisted cover, annual interpolation]
 C --> V[Habitat visuals and annual reporting]
 W[Clearance] --> D[Regeneration/individual removal]
 D --> R
 W --> Z[Area cover reset or circular treatment patch]
 Z --> C
 Z --> V
 C -. missing direct causal input .-> E
 C -. missing direct causal input .-> G
```

Proof boundaries: ScenarioOneUnderstorey.Target/Advance; ScenarioOneManager.AdvanceUnderstorey and annual order; ScenarioOneClearance.QueryClearance/ApplyClearance/IsVegetationDisplayCleared; ForestEcologyController establishment/age-band growth; JuvenileEcologyRules; ScenarioOneManager.AdvancePlantedJuveniles. Dashed arrows mark missing connections, not implemented effects.

The candidate programme must test identical light/seed/species/browse/age/capacity with differing cover for Sitka/Oak/Beech and natural/exact-planted adapters. Do not conflate the current removal of tree competitors with a future competition relief from understorey alone. Shelters must modify browsing access independently of vegetation response. No-seed recruitment remains forbidden under regeneration model 1.
