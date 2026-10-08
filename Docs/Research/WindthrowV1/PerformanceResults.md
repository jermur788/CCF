# Windthrow performance verification

Final simulation gate PASS: 40 cases in 84.27s (earlier run 98.61s), actual ID-roll-derived10/50/100 victims in336/1300/5000-tree synthetic stands, plus1000 victims in5000 trees. One warm-up plus three measured repeats per size; medians below. Growth0 isolates windthrow from ordinary density mortality. Bare synthetic crown transforms make this a simulation/cheap-visual benchmark, not authored crown rendering. All nonempty events have exactly one canopy and one seed rebuild and exactly one record per victim. Zero-victim behavior is independently checked.

| Trees | Victims | Annual ms | Evaluate ms | Mortality ms | Rebuild ms | Records ms (subset) | Visual creation ms |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 336 | 10 | 53.127 | 3.059 | 0.061 | 12.311 | 0.020 | 2.731 |
| 336 | 50 | 57.773 | 2.974 | 0.204 | 10.979 | 0.073 | 12.453 |
| 336 | 100 | 61.418 | 2.958 | 0.434 | 8.532 | 0.169 | 24.073 |
| 1300 | 10 | 216.670 | 12.240 | 0.060 | 48.609 | 0.019 | 2.521 |
| 1300 | 50 | 220.937 | 12.136 | 0.213 | 47.562 | 0.071 | 12.738 |
| 1300 | 100 | 224.557 | 12.209 | 0.452 | 45.643 | 0.145 | 24.382 |
| 5000 | 10 | 1372.196 | 52.882 | 0.066 | 266.444 | 0.020 | 2.953 |
| 5000 | 50 | 1375.715 | 52.907 | 0.250 | 262.019 | 0.077 | 13.200 |
| 5000 | 100 | 1385.256 | 52.968 | 0.478 | 269.637 | 0.153 | 26.512 |
| 5000 | 1000 | 1331.653 | 52.451 | 4.169 | 204.276 | 1.439 | 298.203 |

Record creation time is included in mortality; do not add it twice. Annual time includes the remaining shared ecology path, so these columns are not a sum decomposition. At 5000 trees the final total annual step is about 1.33–1.39s (earlier about 1.65s); it is not suitable to claim a frame-time budget. The bounded batched rebuild holds, but a large future scenario would need separate profiling. The authored336-tree scenario is materially smaller.

Rendered gate PASS:1167 checks in308.04s. Unity6000.6.0f1, Vulkan selected NVIDIA GTX980M (driver580.178.04), GameView1920×1080. Each condition uses exactly the same fixed camera,30 warm-up frames then90 samples, enabled followed by disabled storm visual objects. Main Thread ProfilerRecorder valid in every sample. These are Editor frame wall/Main Thread times, not GPU timers or standalone Player FPS. No repeated randomized order comparison or statistical significance claim.

| Condition | Storm visuals | Total deadwood records | Enabled frame median / p95 ms | Disabled frame median / p95 ms | Main Thread enabled / disabled median ms |
|---|---:|---:|---:|---:|---:|
| fresh | 58 | 58 | 28.73 / 30.01 | 27.74 / 29.58 | 28.80 / 27.86 |
| victims-10 | 10 | 88 | 96.35 / 181.19 | 95.06 / 121.98 | 96.34 / 95.76 |
| victims-50 | 50 | 128 | 99.73 / 100.76 | 99.53 / 100.86 | 99.77 / 99.63 |
| victims-100 | 100 | 178 | 99.80 / 101.14 | 99.82 / 100.81 | 99.82 / 99.62 |
| victims-500 | 500 | 500 | 236.07 / 246.94 | 240.80 / 259.83 | 235.61 / 242.61 |
| century | 47 | 543 | 100.03 / 101.36 | 99.88 / 101.29 | 99.71 / 99.67 |

10/50/100 cases use the authored stand developed20 years, then independently resolved exact ID-roll victims with Growth0 during that event. The500-victim case uses1300 authored Sitka displays with realistic25m/30cm dimensions and Growth0 isolation. Fresh has58 wind victims; century has47 wind victims within543 total deadwood records after normal Growth1+Storm1 over100 years. Selected .05 uniform stress profile is not the proposed default. Both event paths enforce one canopy/seed rebuild; authored event totals97.252/121.631/113.746/451.652ms for10/50/100/500 respectively, recorded alongside substeps.

Performance is a limitation: about100ms developed/century Editor frames even with all storm visuals disabled, and about240ms for the1300-tree fixture. The differences do not support blaming or exonerating a single subsystem. The crown cap limits duplication, but this evidence is not release frame-budget acceptance; wider forest/understorey/deadwood profiling and standalone Player measurement are needed before claiming smooth century play. Separate asset follow-up retains those explicit acceptance gates.

Visual budget:20 fallen crowns within35m, nearest distance then tree-ID tie, one0.5s shared pass. Force an existing LOD no larger than2; smaller LOD sets use their last valid level. Root/log representations remain at distance and after crown decay. Rotated green living crowns are a prototype art gap. Salvaged records must never recreate a crown during budget refresh; the final production guard and focused gate cover this edge case.
