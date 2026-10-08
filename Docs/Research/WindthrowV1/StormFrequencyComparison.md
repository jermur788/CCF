# Storm occurrence comparison

Complete: 144/144 worlds, eight seeds per managed/unmanaged profile, all horizons reached. Event occurrence is independent of tree vulnerability; events may have zero victims. There is no immunity period. Annual candidates 0/1/2/3/5%, intensity .02/.06/.18, uniform 1:1:1 or light-biased 4:2:1 weights are [C]. Occurrence/severity/direction/victim RNG domains remain the same across profiles.

| Rate | Weights | Mean events at 100 | Zero-event worlds | Managed mean/max victims | Unmanaged mean/max victims | Managed wind crop losses | Managed min cash € |
|---:|---|---:|---:|---:|---:|---:|---:|
| 0% | 1:1:1 | 0.000 | 8/8 | 0.000/0 | 0.000/0 | 0.000 | 6127.69 |
| 1% | 1:1:1 | 0.625 | 4/8 | 4.375/14 | 4.375/21 | 0.750 | 6128.28 |
| 1% | 4:2:1 | 0.625 | 4/8 | 2.375/6 | 2.625/9 | 0.750 | 6128.28 |
| 2% | 1:1:1 | 2.000 | 1/8 | 25.000/56 | 27.625/52 | 4.375 | 6054.60 |
| 2% | 4:2:1 | 2.000 | 1/8 | 18.125/56 | 18.000/52 | 3.625 | 6054.60 |
| 3% | 1:1:1 | 3.500 | 0/8 | 40.375/77 | 42.750/90 | 6.125 | 5976.74 |
| 3% | 4:2:1 | 3.500 | 0/8 | 28.625/61 | 28.250/59 | 4.875 | 6054.60 |
| 5% | 1:1:1 | 6.000 | 0/8 | 61.375/96 | 70.625/119 | 8.625 | 5964.26 |
| 5% | 4:2:1 | 6.000 | 0/8 | 40.875/88 | 44.250/103 | 6.750 | 6042.89 |

Recommend 2% with uniform severity weights as the next reviewed comparison profile: 12.75/20.125/25 mean managed victims at years25/50/100, 1.625/3.25/4.375 wind crop casualties, and all eight managed runs completed by25. At1%, century damage is nearly absent in several seeds; at5%, the same policy loses more crops and61.375 mean stems by100. Light-biased weighting reduces damage without changing event occurrence. This sample supports a moderate disturbance candidate, not a measured return period or an accepted default.

At2% uniform, managed canopy means .871761/.834387/.803903 at25/50/100, regeneration34.5689/40.5114/55.4009, living promoted trees23.875/12/13.125. Corresponding no-storm means are canopy.888062/.846432/.794511, regeneration32.7714/39.7677/56.7748 and living recruits22.5/9.25/6.75. More late canopy or recruits in disturbed worlds does not erase crop losses or establish ecological improvement.

Default remains stormModel0. Recommend a reviewed opt-in StormModel1 scenario option first (B), with a separate activation/default decision. Production profile is immutable runtime configuration; it is not silently persisted as an additional save field. Any future configurable profile selection requires explicit save/schema review.
