# Storm calibration results

All 144 worlds completed every 25/50/100-year checkpoint: 18 policy/profile groups, eight seeds each. No early stops or missing horizons. Initial geometry is identical; annual RNG seeds are 20260914 + index × 7919. RNG1/Regeneration2/Growth1 are active; the comparison uses ReducedProposal, BoundedRational, neutral site/edge, intensity .02/.06/.18. Numeric choices are [C] game calibration, not empirical Irish storm probabilities or wind speeds.

Managed policy: 16 largest crops in 10 m blocks; year1 approximately .27 basal-area thinning near crops (first4 Leave, next3 Keep, remainder Sell); year2 eight Oak/eight Beech in protected owner and unprotected contractor groups; year7 three keeps and one protected Oak; year17 approximately .20 basal-area thinning with sale. No adaptive salvage, later recropping or further intervention. Unmanaged controls take no action.

The original process completed 40 worlds before ZX20 disconnected. Its partial 41st world is excluded. An explicit resumed source independently ran the remaining 104 worlds (PASS, 11177 checks, 3506.84 s). The analyzer requires WORLD_DONE and complete rows, detects duplicates, and combines only complete cases. See Evidence/summary.json, summary.csv, complete_world_horizons.csv and complete_world_events.csv; original/resumed logs remain in ignored Build/WindthrowV1.

Completion is a frozen game outcome: all 72 managed worlds completed by year25, while unmanaged worlds completed 0/72. Later current objective failures also occur in storm-free controls and must not be mistaken for reversed completion or attributed solely to storms. Eight successes per profile do not establish a ≥90% population success rate. Unmanaged outcomeYear100 denotes failure, not completion. `original_crops_lost` in the source CSV counts cumulative marked crop casualties specifically from wind, not all natural mortality.

| Policy | Annual rate | Severity weights | Year | Completed | Current goals | Wind victims mean/max | Wind crop losses mean | Canopy mean | Regeneration mean | Live recruits mean | Min cash € |
|---|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Unmanaged | 0% | 1:1:1 | 25 | 0/8 | 0/8 | 0.000/0 | 0.000 | 0.8829 | 23.670 | 8.625 | 12000.00 |
| Unmanaged | 0% | 1:1:1 | 50 | 0/8 | 0/8 | 0.000/0 | 0.000 | 0.8274 | 39.574 | 7.250 | 12000.00 |
| Unmanaged | 0% | 1:1:1 | 100 | 0/8 | 0/8 | 0.000/0 | 0.000 | 0.7939 | 59.169 | 7.750 | 12000.00 |
| Managed | 0% | 1:1:1 | 25 | 8/8 | 8/8 | 0.000/0 | 0.000 | 0.8881 | 32.771 | 22.500 | 6127.69 |
| Managed | 0% | 1:1:1 | 50 | 8/8 | 0/8 | 0.000/0 | 0.000 | 0.8464 | 39.768 | 9.250 | 6127.69 |
| Managed | 0% | 1:1:1 | 100 | 8/8 | 0/8 | 0.000/0 | 0.000 | 0.7945 | 56.775 | 6.750 | 6127.69 |
| Unmanaged | 1% | 1:1:1 | 25 | 0/8 | 0/8 | 0.375/3 | 0.000 | 0.8827 | 23.723 | 8.625 | 12000.00 |
| Unmanaged | 1% | 1:1:1 | 50 | 0/8 | 0/8 | 3.875/21 | 0.000 | 0.8272 | 40.284 | 8.125 | 12000.00 |
| Unmanaged | 1% | 1:1:1 | 100 | 0/8 | 0/8 | 4.375/21 | 0.000 | 0.7930 | 58.541 | 5.250 | 12000.00 |
| Managed | 1% | 1:1:1 | 25 | 8/8 | 8/8 | 0.500/4 | 0.000 | 0.8880 | 32.785 | 22.500 | 6128.28 |
| Managed | 1% | 1:1:1 | 50 | 8/8 | 0/8 | 3.125/14 | 0.500 | 0.8464 | 40.139 | 9.875 | 6128.28 |
| Managed | 1% | 1:1:1 | 100 | 8/8 | 0/8 | 4.375/14 | 0.750 | 0.7999 | 56.934 | 10.125 | 6128.28 |
| Unmanaged | 1% | 4:2:1 | 25 | 0/8 | 0/8 | 0.375/3 | 0.000 | 0.8827 | 23.723 | 8.625 | 12000.00 |
| Unmanaged | 1% | 4:2:1 | 50 | 0/8 | 0/8 | 2.375/9 | 0.000 | 0.8285 | 39.796 | 7.250 | 12000.00 |
| Unmanaged | 1% | 4:2:1 | 100 | 0/8 | 0/8 | 2.625/9 | 0.000 | 0.7918 | 58.351 | 5.500 | 12000.00 |
| Managed | 1% | 4:2:1 | 25 | 8/8 | 8/8 | 0.500/4 | 0.000 | 0.8880 | 32.785 | 22.500 | 6128.28 |
| Managed | 1% | 4:2:1 | 50 | 8/8 | 0/8 | 2.125/6 | 0.500 | 0.8465 | 39.944 | 10.000 | 6128.28 |
| Managed | 1% | 4:2:1 | 100 | 8/8 | 0/8 | 2.375/6 | 0.750 | 0.7957 | 56.476 | 8.250 | 6128.28 |
| Unmanaged | 2% | 1:1:1 | 25 | 0/8 | 0/8 | 11.875/52 | 0.000 | 0.8809 | 23.649 | 9.375 | 12000.00 |
| Unmanaged | 2% | 1:1:1 | 50 | 0/8 | 0/8 | 22.250/52 | 0.000 | 0.8211 | 40.092 | 9.375 | 12000.00 |
| Unmanaged | 2% | 1:1:1 | 100 | 0/8 | 0/8 | 27.625/52 | 0.000 | 0.7960 | 56.991 | 7.500 | 12000.00 |
| Managed | 2% | 1:1:1 | 25 | 8/8 | 8/8 | 12.750/56 | 1.625 | 0.8718 | 34.569 | 23.875 | 6054.60 |
| Managed | 2% | 1:1:1 | 50 | 8/8 | 0/8 | 20.125/56 | 3.250 | 0.8344 | 40.511 | 12.000 | 6054.60 |
| Managed | 2% | 1:1:1 | 100 | 8/8 | 0/8 | 25.000/56 | 4.375 | 0.8039 | 55.401 | 13.125 | 6054.60 |
| Unmanaged | 2% | 4:2:1 | 25 | 0/8 | 0/8 | 10.000/52 | 0.000 | 0.8818 | 23.608 | 9.125 | 12000.00 |
| Unmanaged | 2% | 4:2:1 | 50 | 0/8 | 0/8 | 15.250/52 | 0.000 | 0.8300 | 39.734 | 9.000 | 12000.00 |
| Unmanaged | 2% | 4:2:1 | 100 | 0/8 | 0/8 | 18.000/52 | 0.000 | 0.7983 | 56.028 | 5.750 | 12000.00 |
| Managed | 2% | 4:2:1 | 25 | 8/8 | 8/8 | 11.125/56 | 1.375 | 0.8755 | 34.379 | 23.875 | 6054.60 |
| Managed | 2% | 4:2:1 | 50 | 8/8 | 0/8 | 15.000/56 | 2.625 | 0.8392 | 40.199 | 11.250 | 6054.60 |
| Managed | 2% | 4:2:1 | 100 | 8/8 | 0/8 | 18.125/56 | 3.625 | 0.7970 | 56.381 | 6.750 | 6054.60 |
| Unmanaged | 3% | 1:1:1 | 25 | 0/8 | 0/8 | 17.500/59 | 0.000 | 0.8818 | 23.461 | 9.500 | 12000.00 |
| Unmanaged | 3% | 1:1:1 | 50 | 0/8 | 0/8 | 33.875/83 | 0.000 | 0.8235 | 39.755 | 9.375 | 12000.00 |
| Unmanaged | 3% | 1:1:1 | 100 | 0/8 | 0/8 | 42.750/90 | 0.000 | 0.7948 | 55.831 | 8.250 | 12000.00 |
| Managed | 3% | 1:1:1 | 25 | 8/8 | 8/8 | 17.500/60 | 2.000 | 0.8690 | 35.484 | 24.875 | 5976.74 |
| Managed | 3% | 1:1:1 | 50 | 8/8 | 0/8 | 32.000/69 | 4.125 | 0.8296 | 40.522 | 12.750 | 5976.74 |
| Managed | 3% | 1:1:1 | 100 | 8/8 | 0/8 | 40.375/77 | 6.125 | 0.8053 | 55.452 | 11.000 | 5976.74 |
| Unmanaged | 3% | 4:2:1 | 25 | 0/8 | 0/8 | 12.125/59 | 0.000 | 0.8829 | 23.284 | 9.250 | 12000.00 |
| Unmanaged | 3% | 4:2:1 | 50 | 0/8 | 0/8 | 23.500/59 | 0.000 | 0.8318 | 39.604 | 9.500 | 12000.00 |
| Unmanaged | 3% | 4:2:1 | 100 | 0/8 | 0/8 | 28.250/59 | 0.000 | 0.8005 | 56.144 | 7.125 | 12000.00 |
| Managed | 3% | 4:2:1 | 25 | 8/8 | 8/8 | 13.125/60 | 1.500 | 0.8752 | 34.676 | 24.125 | 6054.60 |
| Managed | 3% | 4:2:1 | 50 | 8/8 | 0/8 | 23.750/60 | 3.250 | 0.8344 | 39.971 | 11.250 | 6054.60 |
| Managed | 3% | 4:2:1 | 100 | 8/8 | 0/8 | 28.625/61 | 4.875 | 0.7937 | 56.275 | 6.875 | 6054.60 |
| Unmanaged | 5% | 1:1:1 | 25 | 0/8 | 0/8 | 29.250/59 | 0.000 | 0.8840 | 23.235 | 10.125 | 12000.00 |
| Unmanaged | 5% | 1:1:1 | 50 | 0/8 | 0/8 | 57.000/98 | 0.000 | 0.8283 | 38.570 | 11.250 | 12000.00 |
| Unmanaged | 5% | 1:1:1 | 100 | 0/8 | 0/8 | 70.625/119 | 0.000 | 0.7838 | 55.192 | 8.375 | 12000.00 |
| Managed | 5% | 1:1:1 | 25 | 8/8 | 8/8 | 27.875/60 | 3.500 | 0.8607 | 37.279 | 27.875 | 5964.26 |
| Managed | 5% | 1:1:1 | 50 | 8/8 | 1/8 | 50.500/86 | 6.500 | 0.8275 | 40.872 | 15.125 | 5964.26 |
| Managed | 5% | 1:1:1 | 100 | 8/8 | 0/8 | 61.375/96 | 8.625 | 0.7968 | 54.283 | 8.500 | 5964.26 |
| Unmanaged | 5% | 4:2:1 | 25 | 0/8 | 0/8 | 18.625/59 | 0.000 | 0.8848 | 23.082 | 9.750 | 12000.00 |
| Unmanaged | 5% | 4:2:1 | 50 | 0/8 | 0/8 | 37.625/98 | 0.000 | 0.8335 | 39.073 | 10.250 | 12000.00 |
| Unmanaged | 5% | 4:2:1 | 100 | 0/8 | 0/8 | 44.250/103 | 0.000 | 0.7943 | 56.088 | 6.625 | 12000.00 |
| Managed | 5% | 4:2:1 | 25 | 8/8 | 8/8 | 19.375/60 | 3.000 | 0.8667 | 36.033 | 26.750 | 6042.89 |
| Managed | 5% | 4:2:1 | 50 | 8/8 | 1/8 | 34.875/86 | 5.250 | 0.8329 | 40.302 | 13.875 | 6042.89 |
| Managed | 5% | 4:2:1 | 100 | 8/8 | 0/8 | 40.875/88 | 6.750 | 0.7902 | 54.966 | 7.625 | 6042.89 |

Forced comparisons and exact causality gates are reported separately in VulnerabilityComparison.md. This experiment demonstrates bounded response for the specified fixed policy and seeds; it does not demonstrate an optimal adaptive policy or century-long maintenance of all objectives.

Complete horizon CSV also preserves separate bramble/bracken cover, planted survival, regeneration-cell counts and the names of failing current objectives. These are shared Model2 outcomes, not a direct hard-coded storm effect.
