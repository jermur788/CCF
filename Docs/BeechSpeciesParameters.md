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
| Seed dispersal scale/cutoff | 6 m / 50 m | B/C | Short local Beech seed shadow; short scale means most seed lands near the parent, but the long cutoff allows occasional longer dispersal. |
| Juvenile light response | 0, 0, 0.15, 0.35, 0.65, 1, 1 at 0, 2, 5, 10, 20, 35, 100% light | A/B/C | Shade-tolerant persistence, but severe deep-shade stagnation; strong response from ~20% light upwards. |
| Regeneration support | enabled (v1) | Scope | Beech seed production, regeneration and promotion are active and tested. |

The asset's regeneration fields are used by the live ecology.
Mixed-species verification tests natural regeneration, shared-space capacity,
light-driven juvenile growth, promotion and persistence as well as
individual-tree competition, canopy, felling and save/load.

Site interpretation: fresh/moist, well-drained mineral soil is preferred;
drought and waterlogging should both be strong penalties. Beech persists below
a Sitka canopy but does not thrive at arbitrarily deep shade or spread without
a Beech seed source.
