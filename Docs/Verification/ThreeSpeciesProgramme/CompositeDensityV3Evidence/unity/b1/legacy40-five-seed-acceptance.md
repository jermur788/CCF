### A_OAK_DENSE: 3/5 seeds pass every R1'-R4 limit

| metric (limit) | default | 101 | 102 | 103 | 104 | median | min | max | seeds passing |
|---|---|---|---|---|---|---|---|---|---|
| R1a abs occupancy slope Y201-300 (<= 0.0025) | 0.0030 **x** | 0.0005 | 0.0011 | 0.0013 | 0.0014 | 0.0013 | 0.0005 | 0.0030 | 4/5 |
| R1b abs occupancy slope Y151-250 (<= 0.0040) | 0.0018 | 0.0005 | 0.0006 | 0.0000 | 0.0007 | 0.0006 | 0.0000 | 0.0018 | 5/5 |
| R2a largest 5-year-block change of annual loss (<= 0.100) | 0.242 **x** | 0.095 | 0.101 **x** | 0.075 | 0.084 | 0.095 | 0.075 | 0.242 | 3/5 |
| R2b largest single-year loss (<= 0.400) | 0.303 | 0.316 | 0.292 | 0.308 | 0.308 | 0.308 | 0.292 | 0.316 | 5/5 |
| R3a worst 25-y dead basal area fraction (<= 4.00%) | 1.64% | 1.44% | 1.79% | 1.43% | 1.40% | 1.44% | 1.40% | 1.79% | 5/5 |
| R3b worst 25-y fraction of stems >= 20 cm dead (<= 5.00%) | 1.74% | 1.61% | 1.51% | 1.63% | 1.53% | 1.61% | 1.51% | 1.74% | 5/5 |
| R4a worst 25-y basal-area ratio (>= 0.500) | 0.742 | 0.814 | 0.761 | 0.895 | 0.880 | 0.814 | 0.742 | 0.895 | 5/5 |
| R4b basal area Y300/Y100 (>= 0.500) | 0.986 | 0.973 | 1.170 | 1.044 | 1.152 | 1.044 | 0.973 | 1.170 | 5/5 |

Misses (value past the limit): R1a seed default: 0.0030 vs 0.0025 (+0.0005, 21% of limit); R2a seed default: 0.242 vs 0.100 (+0.142, 142% of limit); R2a seed 102: 0.101 vs 0.100 (+0.001, 1% of limit)

Trajectory sanity: default: min living 0.12 of start, Y300 66 stems / BA 39 / occ 1.43, years with loss>40%: 0, >25%: 20; 101: min living 0.12 of start, Y300 48 stems / BA 40 / occ 1.45, years with loss>40%: 0, >25%: 24; 102: min living 0.13 of start, Y300 34 stems / BA 45 / occ 1.63, years with loss>40%: 0, >25%: 15; 103: min living 0.12 of start, Y300 36 stems / BA 44 / occ 1.59, years with loss>40%: 0, >25%: 25; 104: min living 0.12 of start, Y300 30 stems / BA 48 / occ 1.73, years with loss>40%: 0, >25%: 27

Reference E80-default: R1a 0.0003, R1b 0.0011, R2a 0.052, R2b 0.270, R3a 1.30%, R3b 1.15%, R4a 0.932, R4b 1.114; Y300 106 stems / BA 46 / occ 1.68

Reference E80-101: R1a 0.0005, R1b 0.0009, R2a 0.049, R2b 0.272, R3a 1.25%, R3b 1.10%, R4a 0.940, R4b 1.085; Y300 103 stems / BA 45 / occ 1.63

Reference E80-102: R1a 0.0005, R1b 0.0008, R2a 0.063, R2b 0.284, R3a 1.31%, R3b 1.05%, R4a 0.906, R4b 1.152; Y300 96 stems / BA 47 / occ 1.72

### B_BEECH_DENSE: 1/5 seeds pass every R1'-R4 limit

| metric (limit) | default | 101 | 102 | 103 | 104 | median | min | max | seeds passing |
|---|---|---|---|---|---|---|---|---|---|
| R1a abs occupancy slope Y201-300 (<= 0.0025) | 0.0002 | 0.0013 | 0.0011 | 0.0026 **x** | 0.0014 | 0.0013 | 0.0002 | 0.0026 | 4/5 |
| R1b abs occupancy slope Y151-250 (<= 0.0040) | 0.0015 | 0.0000 | 0.0014 | 0.0011 | 0.0009 | 0.0011 | 0.0000 | 0.0015 | 5/5 |
| R2a largest 5-year-block change of annual loss (<= 0.100) | 0.128 **x** | 0.139 **x** | 0.085 | 0.096 | 0.109 **x** | 0.109 | 0.085 | 0.139 | 2/5 |
| R2b largest single-year loss (<= 0.400) | 0.297 | 0.328 | 0.328 | 0.354 | 0.325 | 0.328 | 0.297 | 0.354 | 5/5 |
| R3a worst 25-y dead basal area fraction (<= 4.00%) | 1.49% | 1.52% | 1.43% | 1.36% | 1.37% | 1.43% | 1.36% | 1.52% | 5/5 |
| R3b worst 25-y fraction of stems >= 20 cm dead (<= 5.00%) | 1.51% | 1.24% | 1.55% | 1.50% | 1.31% | 1.50% | 1.24% | 1.55% | 5/5 |
| R4a worst 25-y basal-area ratio (>= 0.500) | 0.857 | 0.883 | 0.865 | 0.876 | 0.890 | 0.876 | 0.857 | 0.890 | 5/5 |
| R4b basal area Y300/Y100 (>= 0.500) | 1.183 | 1.186 | 1.163 | 1.055 | 1.038 | 1.163 | 1.038 | 1.186 | 5/5 |

Misses (value past the limit): R1a seed 103: 0.0026 vs 0.0025 (+0.0001, 4% of limit); R2a seed default: 0.128 vs 0.100 (+0.028, 28% of limit); R2a seed 101: 0.139 vs 0.100 (+0.039, 39% of limit); R2a seed 104: 0.109 vs 0.100 (+0.009, 9% of limit)

Trajectory sanity: default: min living 0.13 of start, Y300 49 stems / BA 60 / occ 1.64, years with loss>40%: 0, >25%: 39; 101: min living 0.15 of start, Y300 31 stems / BA 66 / occ 1.81, years with loss>40%: 0, >25%: 62; 102: min living 0.14 of start, Y300 60 stems / BA 65 / occ 1.77, years with loss>40%: 0, >25%: 58; 103: min living 0.15 of start, Y300 58 stems / BA 60 / occ 1.65, years with loss>40%: 0, >25%: 82; 104: min living 0.12 of start, Y300 86 stems / BA 58 / occ 1.58, years with loss>40%: 0, >25%: 42

Reference E80-default: R1a 0.0006, R1b 0.0008, R2a 0.073, R2b 0.329, R3a 1.22%, R3b 1.01%, R4a 0.919, R4b 1.206; Y300 169 stems / BA 66 / occ 1.79

Reference E80-101: R1a 0.0008, R1b 0.0000, R2a 0.109, R2b 0.340, R3a 1.26%, R3b 1.12%, R4a 0.884, R4b 1.134; Y300 197 stems / BA 60 / occ 1.65

Reference E80-102: R1a 0.0012, R1b 0.0013, R2a 0.101, R2b 0.326, R3a 1.35%, R3b 1.56%, R4a 0.850, R4b 1.058; Y300 245 stems / BA 61 / occ 1.66

### E_OAK_YOUNG: 4/5 seeds pass every R1'-R4 limit

| metric (limit) | default | 101 | 102 | 103 | 104 | median | min | max | seeds passing |
|---|---|---|---|---|---|---|---|---|---|
| R1a abs occupancy slope Y201-300 (<= 0.0025) | 0.0004 | 0.0002 | 0.0015 | 0.0015 | 0.0019 | 0.0015 | 0.0002 | 0.0019 | 5/5 |
| R1b abs occupancy slope Y151-250 (<= 0.0040) | 0.0018 | 0.0016 | 0.0021 | 0.0019 | 0.0016 | 0.0018 | 0.0016 | 0.0021 | 5/5 |
| R2a largest 5-year-block change of annual loss (<= 0.100) | 0.102 **x** | 0.080 | 0.042 | 0.063 | 0.041 | 0.063 | 0.041 | 0.102 | 4/5 |
| R2b largest single-year loss (<= 0.400) | 0.259 | 0.237 | 0.258 | 0.286 | 0.250 | 0.258 | 0.237 | 0.286 | 5/5 |
| R3a worst 25-y dead basal area fraction (<= 4.00%) | 1.63% | 1.61% | 1.57% | 1.62% | 1.57% | 1.61% | 1.57% | 1.63% | 5/5 |
| R3b worst 25-y fraction of stems >= 20 cm dead (<= 5.00%) | 1.41% | 3.23% | 1.49% | 1.46% | 1.79% | 1.49% | 1.41% | 3.23% | 5/5 |
| R4a worst 25-y basal-area ratio (>= 0.500) | 0.882 | 0.870 | 0.948 | 0.939 | 0.965 | 0.939 | 0.870 | 0.965 | 5/5 |
| R4b basal area Y300/Y100 (>= 0.500) | 1.260 | 1.264 | 1.365 | 1.246 | 1.348 | 1.264 | 1.246 | 1.365 | 5/5 |

Misses (value past the limit): R2a seed default: 0.102 vs 0.100 (+0.002, 2% of limit)

Trajectory sanity: default: min living 0.07 of start, Y300 45 stems / BA 42 / occ 1.51, years with loss>40%: 0, >25%: 3; 101: min living 0.07 of start, Y300 41 stems / BA 42 / occ 1.54, years with loss>40%: 0, >25%: 0; 102: min living 0.08 of start, Y300 38 stems / BA 45 / occ 1.65, years with loss>40%: 0, >25%: 3; 103: min living 0.07 of start, Y300 35 stems / BA 41 / occ 1.50, years with loss>40%: 0, >25%: 3; 104: min living 0.07 of start, Y300 29 stems / BA 45 / occ 1.64, years with loss>40%: 0, >25%: 0

Reference E80-default: R1a 0.0012, R1b 0.0015, R2a 0.037, R2b 0.239, R3a 1.49%, R3b 3.70%, R4a 0.935, R4b 1.306; Y300 130 stems / BA 44 / occ 1.61

Reference E80-101: R1a 0.0010, R1b 0.0018, R2a 0.030, R2b 0.220, R3a 1.51%, R3b 1.62%, R4a 0.953, R4b 1.266; Y300 129 stems / BA 43 / occ 1.57

Reference E80-102: R1a 0.0012, R1b 0.0015, R2a 0.037, R2b 0.239, R3a 1.47%, R3b 1.41%, R4a 0.972, R4b 1.326; Y300 119 stems / BA 45 / occ 1.63

### F_BEECH_YOUNG: 5/5 seeds pass every R1'-R4 limit

| metric (limit) | default | 101 | 102 | 103 | 104 | median | min | max | seeds passing |
|---|---|---|---|---|---|---|---|---|---|
| R1a abs occupancy slope Y201-300 (<= 0.0025) | 0.0013 | 0.0014 | 0.0021 | 0.0020 | 0.0016 | 0.0016 | 0.0013 | 0.0021 | 5/5 |
| R1b abs occupancy slope Y151-250 (<= 0.0040) | 0.0024 | 0.0022 | 0.0025 | 0.0023 | 0.0025 | 0.0024 | 0.0022 | 0.0025 | 5/5 |
| R2a largest 5-year-block change of annual loss (<= 0.100) | 0.042 | 0.052 | 0.041 | 0.043 | 0.054 | 0.043 | 0.041 | 0.054 | 5/5 |
| R2b largest single-year loss (<= 0.400) | 0.224 | 0.231 | 0.258 | 0.259 | 0.264 | 0.258 | 0.224 | 0.264 | 5/5 |
| R3a worst 25-y dead basal area fraction (<= 4.00%) | 1.36% | 1.35% | 1.43% | 1.39% | 1.49% | 1.39% | 1.35% | 1.49% | 5/5 |
| R3b worst 25-y fraction of stems >= 20 cm dead (<= 5.00%) | 1.22% | 1.43% | 1.34% | 2.44% | 1.52% | 1.43% | 1.22% | 2.44% | 5/5 |
| R4a worst 25-y basal-area ratio (>= 0.500) | 0.932 | 0.962 | 0.925 | 0.955 | 0.878 | 0.932 | 0.878 | 0.962 | 5/5 |
| R4b basal area Y300/Y100 (>= 0.500) | 1.379 | 1.375 | 1.393 | 1.292 | 1.269 | 1.375 | 1.269 | 1.393 | 5/5 |

No misses.

Trajectory sanity: default: min living 0.12 of start, Y300 54 stems / BA 59 / occ 1.61, years with loss>40%: 0, >25%: 0; 101: min living 0.10 of start, Y300 59 stems / BA 58 / occ 1.59, years with loss>40%: 0, >25%: 0; 102: min living 0.12 of start, Y300 53 stems / BA 60 / occ 1.64, years with loss>40%: 0, >25%: 1; 103: min living 0.11 of start, Y300 66 stems / BA 56 / occ 1.52, years with loss>40%: 0, >25%: 1; 104: min living 0.10 of start, Y300 90 stems / BA 54 / occ 1.47, years with loss>40%: 0, >25%: 1

Reference E80-default: R1a 0.0015, R1b 0.0025, R2a 0.057, R2b 0.249, R3a 1.28%, R3b 1.11%, R4a 0.947, R4b 1.332; Y300 252 stems / BA 57 / occ 1.55

Reference E80-101: R1a 0.0017, R1b 0.0025, R2a 0.041, R2b 0.244, R3a 1.33%, R3b 1.07%, R4a 0.975, R4b 1.414; Y300 234 stems / BA 60 / occ 1.64

Reference E80-102: R1a 0.0023, R1b 0.0025, R2a 0.025, R2b 0.252, R3a 1.26%, R3b 1.04%, R4a 0.978, R4b 1.403; Y300 207 stems / BA 60 / occ 1.64

### Pooled

* fixture-seed runs passing every limit: 13/20
* R1a (abs occupancy slope Y201-300): 18/20 pass
* R1b (abs occupancy slope Y151-250): 20/20 pass
* R2a (largest 5-year-block change of annual loss): 14/20 pass
* R2b (largest single-year loss): 20/20 pass
* R3a (worst 25-y dead basal area fraction): 20/20 pass
* R3b (worst 25-y fraction of stems >= 20 cm dead): 20/20 pass
* R4a (worst 25-y basal-area ratio): 20/20 pass
* R4b (basal area Y300/Y100): 20/20 pass

### When and where the R2' maximum happens (largest 5-year-block change of the all-stem loss fraction)

| fixture | run | max change | blocks (years) | mean loss before -> after | living stems at the later block | blocks over 0.10 |
|---|---|---|---|---|---|---|
| A_OAK_DENSE | default | 0.242 | Y261-265 -> Y266-270 | 0.279 -> 0.037 | 29 | 2 |
| A_OAK_DENSE | 101 | 0.095 | Y281-285 -> Y286-290 | 0.270 -> 0.175 | 22 | 0 |
| A_OAK_DENSE | 102 | 0.101 | Y181-185 -> Y186-190 | 0.154 -> 0.054 | 54 | 1 |
| A_OAK_DENSE | 103 | 0.075 | Y206-210 -> Y211-215 | 0.247 -> 0.172 | 27 | 0 |
| A_OAK_DENSE | 104 | 0.084 | Y231-235 -> Y236-240 | 0.261 -> 0.176 | 28 | 0 |
| A_OAK_DENSE | E80-default | 0.052 | Y261-265 -> Y266-270 | 0.248 -> 0.196 | 93 | 0 |
| A_OAK_DENSE | E80-101 | 0.049 | Y256-260 -> Y261-265 | 0.255 -> 0.206 | 93 | 0 |
| A_OAK_DENSE | E80-102 | 0.063 | Y256-260 -> Y261-265 | 0.254 -> 0.191 | 90 | 0 |
| B_BEECH_DENSE | default | 0.128 | Y171-175 -> Y176-180 | 0.248 -> 0.120 | 34 | 1 |
| B_BEECH_DENSE | 101 | 0.139 | Y231-235 -> Y236-240 | 0.304 -> 0.165 | 32 | 1 |
| B_BEECH_DENSE | 102 | 0.085 | Y171-175 -> Y176-180 | 0.189 -> 0.104 | 66 | 0 |
| B_BEECH_DENSE | 103 | 0.096 | Y191-195 -> Y196-200 | 0.277 -> 0.181 | 37 | 0 |
| B_BEECH_DENSE | 104 | 0.109 | Y221-225 -> Y226-230 | 0.269 -> 0.160 | 34 | 1 |
| B_BEECH_DENSE | E80-default | 0.073 | Y211-215 -> Y216-220 | 0.280 -> 0.207 | 225 | 0 |
| B_BEECH_DENSE | E80-101 | 0.109 | Y276-280 -> Y281-285 | 0.307 -> 0.197 | 218 | 1 |
| B_BEECH_DENSE | E80-102 | 0.101 | Y266-270 -> Y271-275 | 0.279 -> 0.178 | 238 | 1 |
| E_OAK_YOUNG | default | 0.102 | Y251-255 -> Y256-260 | 0.221 -> 0.119 | 38 | 1 |
| E_OAK_YOUNG | 101 | 0.080 | Y271-275 -> Y276-280 | 0.225 -> 0.145 | 34 | 0 |
| E_OAK_YOUNG | 102 | 0.042 | Y201-205 -> Y206-210 | 0.126 -> 0.168 | 49 | 0 |
| E_OAK_YOUNG | 103 | 0.063 | Y291-295 -> Y296-300 | 0.248 -> 0.185 | 28 | 0 |
| E_OAK_YOUNG | 104 | 0.041 | Y181-185 -> Y186-190 | 0.160 -> 0.118 | 57 | 0 |
| E_OAK_YOUNG | E80-default | 0.037 | Y276-280 -> Y281-285 | 0.222 -> 0.185 | 126 | 0 |
| E_OAK_YOUNG | E80-101 | 0.030 | Y181-185 -> Y186-190 | 0.152 -> 0.122 | 219 | 0 |
| E_OAK_YOUNG | E80-102 | 0.037 | Y231-235 -> Y236-240 | 0.184 -> 0.146 | 163 | 0 |
| F_BEECH_YOUNG | default | 0.042 | Y271-275 -> Y276-280 | 0.192 -> 0.150 | 46 | 0 |
| F_BEECH_YOUNG | 101 | 0.052 | Y276-280 -> Y281-285 | 0.190 -> 0.138 | 72 | 0 |
| F_BEECH_YOUNG | 102 | 0.041 | Y266-270 -> Y271-275 | 0.195 -> 0.154 | 73 | 0 |
| F_BEECH_YOUNG | 103 | 0.043 | Y281-285 -> Y286-290 | 0.223 -> 0.179 | 62 | 0 |
| F_BEECH_YOUNG | 104 | 0.054 | Y291-295 -> Y296-300 | 0.190 -> 0.136 | 64 | 0 |
| F_BEECH_YOUNG | E80-default | 0.057 | Y266-270 -> Y271-275 | 0.211 -> 0.153 | 219 | 0 |
| F_BEECH_YOUNG | E80-101 | 0.041 | Y271-275 -> Y276-280 | 0.231 -> 0.190 | 207 | 0 |
| F_BEECH_YOUNG | E80-102 | 0.025 | Y291-295 -> Y296-300 | 0.215 -> 0.240 | 249 | 0 |

### Small-stand baseline: D_SITKA_DENSE_MODEL2 (legacy route, unchanged by v3), same geometry

| seed | R2a | R2b | R1a | R1b | R3a | R3b |
|---|---|---|---|---|---|---|
| default | 0.195 | 0.809 | 0.0003 | 0.0001 | 2.31% | 4.01% |
| 101 | 0.187 | 0.815 | 0.0002 | 0.0000 | 2.15% | 4.53% |
| 102 | 0.235 | 0.818 | 0.0000 | 0.0001 | 2.09% | 3.92% |
| 103 | 0.190 | 0.800 | 0.0002 | 0.0000 | 2.14% | 3.89% |
| 104 | 0.199 | 0.803 | 0.0003 | 0.0000 | 2.13% | 4.96% |
| (reference E80-default) | 0.170 | 0.761 | 0.0001 | 0.0000 | 1.99% | 2.09% |
