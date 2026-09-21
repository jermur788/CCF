# European beech species parameters

`BeechSpecies.asset` is the initial individual-tree parameter set for
European beech (`Fagus sylvatica`) in the CCF simulation. Beech is treated as
non-native but naturalised in Ireland: it adds structural and timber
diversity, not native Atlantic woodland restoration credit.

The parameters below follow the Ecology handoff. Evidence classes use the
project convention: A = empirical, B = evidence-derived, C = simulation
abstraction, D = calibration.

| Parameter | Value | Class | Rationale |
| --- | ---: | --- | --- |
| Potential DBH growth | 0.40 cm/year | B/C | Good-site young Beech target within the researched 0.3-0.5 cm/year range. |
| Maximum DBH | 120 cm | B/C | Useful soft simulation maximum; not a biological limit. |
| Potential height growth | 0.40 m/year | B/C | Good-site young/pole target within the researched 0.3-0.5 m/year range. |
| Maximum height | 40 m | A/B | Typical mature height; exceptional trees can exceed this later. |
| CI50 | 3.0 | D | Neutral initial calibration; reuse the existing Hegyi scale. |
| Crown model | `ln(CPA) = 0.05 + 1.01 ln(DBH)` | A/B | Moderately/heavily thinned Beech crown relation. |
| Crown relaxation | 0.15/year | D | Provisional multi-year lateral crown response. |
| Reproductive onset | 40 years | A/B | Open-grown onset; dense stands can be delayed. |
| Full reproductive age | 60 years | B/C | Initial model ramp; suppression can delay effective reproduction. |
| Seed dispersal scale/cutoff | 6 m / 50 m | B/C | Short local Beech seed shadow; reproduction is disabled in this slice. |
| Juvenile light response | 0, 0.15, 0.35, 0.65, 1, 1 at 0, 2, 5, 10, 20, 35% light | A/B/C | Shade-tolerant persistence, but severe deep-shade stagnation. |
| Regeneration support | disabled | Scope | Beech seed production/regeneration is intentionally deferred. |

The asset's regeneration fields remain populated for future work but are not
used while `SupportsRegeneration` is false. Current mixed-species verification
therefore tests only individual-tree growth, competition, canopy/light,
inspection, felling and persistence.

Site interpretation for later regeneration work: fresh/moist, well-drained
mineral soil is preferred; drought and waterlogging should both be strong
penalties. Beech should persist below a Sitka canopy but should not thrive at
arbitrarily deep shade or spread without a Beech seed source.
