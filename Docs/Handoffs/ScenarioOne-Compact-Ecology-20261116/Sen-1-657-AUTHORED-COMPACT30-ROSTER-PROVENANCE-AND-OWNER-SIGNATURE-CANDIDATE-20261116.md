# Sen-1 #657 — Authored Compact30 roster: provenance and owner-signature CANDIDATE

**Status: CANDIDATE / UNSIGNED / NOT A SIGNED WORLD / NOT UNITY-VERIFIED / NO IMPLEMENTATION AUTHORITY.**

- **Date:** 16 November 2026 (UTC).
- **Issue:** [#657](https://github.com/jermur788/CCF/issues/657).
- **Worker:** Claude Code, model Claude Opus 5.5 (`claude-opus-5-5`, Anthropic), Linux host `jer-laptop`.
- **Writes:** GitHub-only, to `docs/sen-1-657-compact-authoring-spec-convergence-20261116-v1`.
- **Read-only inputs:** the local research files below, plus `main` at `500ef7acecd39724b8ebd4cec1ebdfc00be45867`. No local research byte was changed. File hashes were taken before and after the work and are identical.

The owner has **not** signed this content. No world/content key is bound to it, and no Compact scene, save or profile exists. This annex supplies what a content owner needs to sign or reject an exact authored roster: authenticated source bytes, an explicit membership rule, a canonical serialisation, a full-content SHA-256, independent geometry checks and the fields still missing. The reserved draft name `Compact30-H1-RouteA-G1-content-v1` (#622 §5) is used only as a label. It is **not** a registered key.

**Evidence labels:**
- **SOURCE BYTES** — read from the local files below.
- **MODEL DERIVATION** — arithmetic on those bytes.
- **OWNER INTENT** — D-083 or D-084, published on `main`.
- **PROPOSED** — the author's authoring choice, which needs review and signature.
- **UNVERIFIED NATIVE** — nothing here was run in Unity.

## 1. Source authentication (SOURCE BYTES, local-only provenance)

| File (local, Files ext4; **not in GitHub**) | Bytes | SHA-256 (full) | Check performed |
| --- | ---: | --- | --- |
| `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/C-B-30.json` | 128,518 | `3e7c37ad010d66382722bbd932db6c93b9de1b78914a38c48b4f1a7e1f9d5a27` | Equals the hash recorded in that run's `evidence-manifest.json`, and `ledger.json` `inputs["C-B-30.json"]`; matches the #626 prefix `3e7c37ad` |
| `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/ledger.json` | 24,746 | `aa10f54f2fac9ccfa23d11a450f29985c39bf966944c665cd822b0ef49b0bcf3` | `sha256sum -c SHA256SUMS` in that folder: **all 9 entries OK** (ledger, `omissions-2.0.json`, 7 scripts); matches the #626 size and prefix |

**Provenance facts read from the bytes:**
- `C-B-30.json` declares `geometryClass = "TEMPORARY RUNTIME DERIVATIVE: NOT A SAVE/PRODUCTION MODEL"`.
- `generatorMethod = "unmodified ForestStartingStand.Generate legacy lattice branch with scratch instance inputs"`.
- `size = 30`, `lattice = 16`, `target = 189`, `seed = 20260914`, `cells = 36`, `rackZ = -0.25`, `rackWidth = 2.4`.
- `harness-provenance.json` records executed-harness SHA-256 `3c0ebae8…9863`, `matches_launch_hash: true`.

**The 19 "rack" stems.** The harness (`executed-harness.cs` line 162) labels each tree `side = "rack"` when `|z − rackZ| ≤ rackWidth/2`, and otherwise `north` or `south`. `rackZ` came from the nearest historical `Dirt Path` plank. **The 19 "rack" rows are an after-the-fact position label, not generator exclusions.** They were living, standing trees at Year 0 in the source. #460 felled them in its Year 1 `access-clearance` trace, which is **not** imported here.

**What else shaped the source stand.** The source stand was generated inside `ForestTest` with the historical clearance objects present. C-B-30 stores 113 `CollectClearancePoints` anchors (104 with radius 1.5 m and 9 with radius 2.2 m; 62 lie inside ±15 m). The ledger's historical track is **52 planks, 27.52 m**, from (−0.93, −14.75) to (−2.71, 10.75). Route A's eight anchors are the after-apron part of that track. The 9 anchors of radius 2.2 m sit at the north end (buildables near z ≈ 9…12.8). #508 reported that 37 lattice slots were excluded by source geometry and 30 by hash-rank mortality (256 → 189). #657 did **not** re-derive that 37/30 split, because it needs the generator, not just the output bytes.

## 2. Source roster facts (SOURCE BYTES)

- **189 rows, 189 unique IDs**: all `sitka-spruce`, all age 20; labels north 99 / south 71 / "rack" 19; all |x|, |z| ≤ 15 m.
- **Fields present per row:** `id, species, side, age, fellable, crop, x, z, dbh (cm), height (m), crown (radius, m), ci, edge, ba, volume, saleVolume`. `ci, edge, ba, volume, saleVolume` are *outputs* of the source model at Year 0, not authored inputs. `crop` marks the harness's two crop picks (P0000, P1504) and is not carried forward.
- **Year-0 totals:** BA 3.6501 m², stem volume 22.2125 m³. Canopy mean recorded 0.97983, recomputed 0.97983 (ledger `canopyControl`).

## 3. Independent recomputation by #657 (MODEL DERIVATION, from the bytes)

- **H1 rectangle** x ∈ [−9, 3], z ∈ [−15, −5] (ledger `H1.rect`). The trees inside it, recomputed from raw x/z, are **exactly** the ledger's 14: P0004, P0005, P0006, P0207, P0306, P0307, P0403, P0404, P0405, P0406, P0407, P0503, P0505, P0507. All 14 are `south`-labelled; none is a "rack" stem. The ledger's rounded coordinates differ from raw by at most 0.0047 m. Their notional sale volume is 1.6173 m³. That volume is **not** sold, harvested or credited.
- **The 1 m outside buffer**, recomputed, is **exactly** the ledger's 8: P0003, P0009, P0103, P0109, P0209, P0504, P0506, P0508. All are retained.
- **Route A**, from the ledger's 8 samples, is 16.671961 m long. The nearest candidate-roster stem centre to the polyline is P1106 at 1.6005 m. The ledger's whole-track figure is 1.592 m, measured against the full 52-plank track. **No stem centre is within 0.8 m** of the polyline. The smallest trunk-edge margin to a 1.2 m core (distance − dbh/2 − 0.6) is **+0.919 m** (P1106).
- **The four G1 sites** (OWNER INTENT, D-084 Geometry A) checked against the 175-row candidate:

| Site | Species | x, z (m) | Cell | Route A d (m), segment | Core / core+0.20 verge margin at r = 0.10 (m) | Nearest candidate stems (m) |
| --- | --- | --- | ---: | --- | --- | --- |
| O1 | sessile-oak | 1.17, −0.47 | 15 | 1.057755, A1–A2 | +0.358 / +0.158 | P0707 2.431; P0807 3.007; P0710 3.670 |
| O2 | sessile-oak | 1.57, −2.73 | 15 | 1.262838, A0–A1 | +0.563 / +0.363 | P0508 2.066; P0707 3.247; P0710 3.620 |
| B1 | beech | 3.43, −0.07 | 15 | 1.222835, A1–A2 | +0.523 / +0.323 | P0710 1.692; P0810 2.244; P0909 3.052 |
| B2 | beech | 3.83, −2.33 | 15 | 1.020976, A0–A1 | +0.321 / **+0.121** | P0710 1.579; P0510 3.063; P0508 3.636 |

  Site-to-site spacing is 2.295 m (O1–O2, O1–B1, O2–B2, B1–B2) and 3.246 m (O1–B2, O2–B1). No site lies in H1. All four clear the source 0.7 m living-stem admission distance. These figures agree with #583/#622. **Note:** P0707 and P0710, the nearest neighbours of O1/B1/B2, are former "rack"-labelled stems. They stand at Year 0 and keep standing in the candidate. Any authoring choice that deleted the 19 would change these neighbour distances and the cell-15 light (D-084 forbids silent subtraction).
- **Ledger `pathDist`** values (1.08 / 1.29 / 1.18 / 1.00 m) differ slightly from the raw-polyline values above, because the ledger measured against the historical plank track. The raw-polyline values are the ones used for the 1.2 m core.
- **Static light:** the ledger records cell-15 `canopyBase = canopyAfter = 0.7789` for all four sites, giving a light proxy of about 0.221. That is a Year-0 static source proxy. It is **not** native Year 1 / Year 2 light, survival or promotion.
- **Candidate totals:** BA 3.3540 m², stem volume 20.3413 m³. Under the floor-grid cell formula `cell = floor((z+15)/5)·6 + floor((x+15)/5)`, which matches the ledger's cell 15 for all four G1 sites, ecology cells 2, 7 and 8 hold **zero** stems (H1 plus the historical clearing) and cell 15 holds 2.

## 4. PROPOSED authored roster rule (author's choice — needs review and owner signature)

1. **Adopt the C-B-30 Year-0 roster verbatim as authored data.** The roster is *intentionally authored from a temporary derived research fixture*. It is **not** the output of a new no-rack generator, and the legacy generator must **not** be rerun to "reproduce" it.
2. **Omit exactly the 14 H1 IDs before Year 0.** This is pre-world history. It creates no event, Fell mark, order, cash or harvest record.
3. **Keep all 175 other stems**, including the **8 buffer stems** and the **19 former "rack"-labelled stems** as ordinary standing Sitka. No rack meaning, corridor or Year-1 clearance comes with them, and the label is dropped from the authored data.
4. **Authored fields:** `id, species, x_m, z_m, age_y, dbh_cm, height_m, crown_m` at the precision below. Model outputs (`ci, edge, ba, volume, saleVolume`) and harness flags (`side, crop, fellable`) are **not** authored. The game recomputes them.
5. **Precision:** x/z rounded to 0.001 m. The largest rounding shift is 0.00047 m, which does not change any distance check above at its reported precision. DBH and height are rounded to 0.01. Crown radius is rounded to 0.001.
6. **The resulting start is 175 living Sitka.** This is an *authored-content* count, not a native generator count. It must be read back from the loaded rows, never assumed.

**Canonical serialisation (v0-UNSIGNED):**
- UTF-8, LF line endings, **no BOM**.
- Line 1 is the exact header shown.
- Then one row per tree, **sorted by `id` ascending** (ASCII). Fields are comma-separated, with no spaces and no quoting.
- Numbers use fixed decimals (x/z 3, dbh 2, height 2, crown 3), `-` for negatives, no `+`, and `.` as the decimal point.
- There is a trailing LF after the last row.
- SHA-256 is taken over the entire byte string.

| Artifact | Rows | Bytes | SHA-256 |
| --- | ---: | ---: | --- |
| **Authored 175-row CANDIDATE** (`ccf-compact30-authored-roster-candidate;v=0-UNSIGNED`) | 175 | 9,481 | **`dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1`** |
| Source 189-row export, with the harness position label (`ccf-c-b-30-source-roster-export;v=1`), reference only | 189 | 11,359 | `e649b2aac5df85230b71372e0977cbf723990f1aabb5881a3914855575cccfbe` |

Both hashes were regenerated twice from the bytes, with identical results. The script that produced them is in §9. The authored candidate hash covers **only the tree roster**. It is **not** a world/content hash. A world-content signature must also cover the geometry in §6 and the guard/G1 data, under one canonical recipe chosen at signing.

## 5. Full authored 175-row CANDIDATE (exact bytes, between the fences)

```text
ccf-compact30-authored-roster-candidate;v=0-UNSIGNED;fields=id,species,x_m,z_m,age_y,dbh_cm,height_m,crown_m;order=id-ascending;lf
P0000,sitka-spruce,-14.400,-14.400,20,22.21,15.90,2.637
P0001,sitka-spruce,-13.270,-14.400,20,20.08,14.06,2.475
P0002,sitka-spruce,-10.751,-14.400,20,14.94,11.21,2.082
P0003,sitka-spruce,-9.136,-14.400,20,12.42,9.13,1.890
P0009,sitka-spruce,3.266,-14.400,20,18.65,13.67,2.366
P0010,sitka-spruce,5.146,-14.400,20,14.06,10.43,2.015
P0011,sitka-spruce,6.628,-14.400,20,11.95,9.64,1.854
P0012,sitka-spruce,9.012,-14.400,20,11.14,8.80,1.792
P0013,sitka-spruce,11.397,-14.400,20,13.86,11.00,2.000
P0014,sitka-spruce,13.415,-14.400,20,18.69,13.66,2.368
P0015,sitka-spruce,14.400,-14.400,20,21.44,15.88,2.578
P0102,sitka-spruce,-10.599,-13.288,20,16.88,12.16,2.230
P0103,sitka-spruce,-9.118,-12.818,20,15.84,12.03,2.151
P0109,sitka-spruce,3.115,-12.621,20,17.95,13.02,2.312
P0110,sitka-spruce,5.235,-12.705,20,14.69,10.94,2.063
P0112,sitka-spruce,9.101,-12.666,20,13.55,10.41,1.976
P0113,sitka-spruce,10.716,-13.139,20,13.83,10.07,1.997
P0209,sitka-spruce,3.231,-11.397,20,15.91,12.01,2.156
P0210,sitka-spruce,5.111,-11.309,20,15.18,11.02,2.100
P0211,sitka-spruce,6.592,-10.839,20,16.07,12.09,2.168
P0214,sitka-spruce,12.843,-11.231,20,14.87,11.68,2.077
P0300,sitka-spruce,-14.400,-8.917,20,12.88,10.51,1.925
P0301,sitka-spruce,-13.288,-9.348,20,13.91,10.46,2.004
P0310,sitka-spruce,5.199,-9.277,20,16.15,11.89,2.175
P0311,sitka-spruce,6.815,-8.843,20,18.53,14.06,2.356
P0312,sitka-spruce,9.334,-9.316,20,18.46,13.15,2.351
P0313,sitka-spruce,10.949,-8.885,20,17.09,12.99,2.246
P0314,sitka-spruce,12.564,-9.355,20,14.45,11.20,2.045
P0315,sitka-spruce,14.400,-8.924,20,11.78,9.03,1.841
P0400,sitka-spruce,-14.400,-6.755,20,10.65,8.07,1.755
P0401,sitka-spruce,-12.889,-7.224,20,11.99,9.59,1.857
P0402,sitka-spruce,-11.274,-6.794,20,14.96,11.77,2.084
P0410,sitka-spruce,5.076,-7.111,20,16.14,11.51,2.174
P0411,sitka-spruce,6.557,-6.638,20,18.82,13.69,2.379
P0412,sitka-spruce,8.942,-7.072,20,19.97,14.07,2.466
P0413,sitka-spruce,11.326,-6.599,20,16.61,12.50,2.210
P0414,sitka-spruce,12.808,-7.034,20,13.53,10.74,1.974
P0415,sitka-spruce,14.400,-6.561,20,10.74,8.32,1.761
P0500,sitka-spruce,-14.400,-4.903,20,10.88,8.72,1.773
P0501,sitka-spruce,-12.871,-5.334,20,11.63,9.69,1.829
P0502,sitka-spruce,-11.122,-4.942,20,14.57,10.82,2.054
P0504,sitka-spruce,-6.621,-4.825,20,19.15,14.20,2.404
P0506,sitka-spruce,-3.390,-4.864,20,15.48,11.75,2.123
P0508,sitka-spruce,1.111,-4.744,20,12.53,9.25,1.898
P0510,sitka-spruce,4.712,-5.263,20,16.47,12.14,2.199
P0511,sitka-spruce,7.231,-4.832,20,19.78,13.68,2.452
P0512,sitka-spruce,8.578,-5.224,20,20.53,14.83,2.509
P0513,sitka-spruce,11.097,-4.790,20,17.76,12.61,2.298
P0514,sitka-spruce,12.981,-5.341,20,14.49,11.27,2.048
P0515,sitka-spruce,14.400,-4.910,20,12.69,9.64,1.910
P0600,sitka-spruce,-14.400,-3.150,20,13.59,10.54,1.979
P0601,sitka-spruce,-13.263,-2.719,20,14.48,10.58,2.047
P0602,sitka-spruce,-11.012,-3.111,20,15.20,11.72,2.102
P0603,sitka-spruce,-9.397,-2.681,20,15.87,11.62,2.153
P0613,sitka-spruce,10.769,-3.263,20,16.78,12.30,2.223
P0614,sitka-spruce,13.422,-2.871,20,15.05,11.25,2.091
P0615,sitka-spruce,14.400,-3.302,20,14.09,11.16,2.017
P0700,sitka-spruce,-14.400,-0.716,20,18.24,13.78,2.334
P0701,sitka-spruce,-12.794,-1.150,20,17.09,12.37,2.246
P0702,sitka-spruce,-11.312,-0.677,20,16.35,11.58,2.190
P0703,sitka-spruce,-8.928,-1.111,20,14.65,11.04,2.060
P0704,sitka-spruce,-7.446,-0.638,20,13.49,9.98,1.971
P0705,sitka-spruce,-5.062,-1.069,20,14.66,11.23,2.061
P0706,sitka-spruce,-2.677,-0.599,20,16.42,11.98,2.195
P0707,sitka-spruce,-1.196,-1.030,20,17.25,13.01,2.259
P0710,sitka-spruce,4.790,-1.076,20,15.28,11.75,2.108
P0711,sitka-spruce,7.309,-0.645,20,13.25,9.98,1.953
P0712,sitka-spruce,8.924,-1.118,20,13.86,10.69,1.999
P0713,sitka-spruce,11.443,-0.684,20,14.83,10.78,2.074
P0714,sitka-spruce,13.425,-0.998,20,16.49,12.14,2.200
P0715,sitka-spruce,14.400,-0.568,20,17.61,12.32,2.286
P0800,sitka-spruce,-14.400,0.861,20,19.44,13.61,2.426
P0801,sitka-spruce,-12.846,1.295,20,18.58,13.77,2.360
P0802,sitka-spruce,-11.231,0.822,20,15.79,12.38,2.147
P0803,sitka-spruce,-8.712,1.256,20,11.47,9.19,1.817
P0804,sitka-spruce,-7.097,0.783,20,10.72,9.05,1.760
P0805,sitka-spruce,-4.578,1.214,20,12.47,9.63,1.894
P0806,sitka-spruce,-2.963,0.744,20,15.41,11.78,2.118
P0807,sitka-spruce,-1.348,1.175,20,18.24,13.02,2.334
P0810,sitka-spruce,5.118,1.408,20,13.93,10.24,2.005
P0811,sitka-spruce,6.599,0.977,20,11.73,9.39,1.837
P0812,sitka-spruce,8.984,1.446,20,10.45,8.26,1.739
P0813,sitka-spruce,11.369,1.016,20,13.70,10.79,1.988
P0814,sitka-spruce,13.386,1.330,20,17.45,12.78,2.274
P0815,sitka-spruce,14.400,0.896,20,18.65,14.03,2.365
P0900,sitka-spruce,-14.400,2.624,20,20.30,14.95,2.491
P0901,sitka-spruce,-12.716,3.097,20,18.68,13.26,2.368
P0902,sitka-spruce,-10.966,2.585,20,15.36,11.71,2.114
P0903,sitka-spruce,-8.582,3.058,20,10.89,8.25,1.773
P0904,sitka-spruce,-6.832,3.450,20,9.94,8.17,1.700
P0905,sitka-spruce,-5.351,3.019,20,10.93,9.29,1.776
P0906,sitka-spruce,-2.698,3.411,20,16.02,11.76,2.165
P0907,sitka-spruce,-1.217,2.977,20,18.79,13.99,2.376
P0909,sitka-spruce,2.917,2.938,20,18.27,13.49,2.337
P0910,sitka-spruce,4.755,3.256,20,14.40,11.25,2.041
P0911,sitka-spruce,7.274,2.783,20,10.17,8.12,1.718
P0912,sitka-spruce,8.621,3.295,20,9.82,8.59,1.691
P0913,sitka-spruce,11.139,2.822,20,12.82,9.94,1.920
P0914,sitka-spruce,13.390,3.334,20,17.49,12.81,2.277
P0915,sitka-spruce,14.400,2.864,20,19.19,13.35,2.407
P1000,sitka-spruce,-14.400,5.196,20,18.81,14.17,2.377
P1001,sitka-spruce,-13.005,4.765,20,18.44,13.25,2.350
P1002,sitka-spruce,-10.621,5.235,20,15.00,10.78,2.087
P1003,sitka-spruce,-9.139,4.804,20,12.74,9.89,1.914
P1004,sitka-spruce,-7.122,5.118,20,11.68,8.90,1.833
P1005,sitka-spruce,-4.737,4.684,20,12.81,10.11,1.919
P1006,sitka-spruce,-3.256,5.157,20,15.14,11.23,2.098
P1009,sitka-spruce,3.362,4.924,20,16.96,12.82,2.236
P1010,sitka-spruce,4.578,4.836,20,14.78,11.63,2.070
P1011,sitka-spruce,7.097,5.270,20,11.94,9.36,1.853
P1012,sitka-spruce,8.712,4.797,20,11.00,9.10,1.781
P1013,sitka-spruce,11.231,5.228,20,13.67,10.25,1.985
P1014,sitka-spruce,12.846,4.758,20,16.02,12.04,2.165
P1015,sitka-spruce,14.400,5.189,20,17.70,12.57,2.293
P1100,sitka-spruce,-14.400,6.762,20,17.26,13.00,2.259
P1101,sitka-spruce,-12.571,7.192,20,15.00,11.09,2.087
P1102,sitka-spruce,-11.224,6.801,20,16.31,12.59,2.187
P1103,sitka-spruce,-8.705,7.235,20,15.93,11.85,2.158
P1104,sitka-spruce,-6.822,6.684,20,14.89,10.69,2.079
P1105,sitka-spruce,-5.206,7.115,20,15.77,11.92,2.145
P1106,sitka-spruce,-2.956,6.723,20,16.21,11.69,2.179
P1108,sitka-spruce,1.446,6.603,20,17.63,12.74,2.287
P1109,sitka-spruce,3.062,7.037,20,15.19,11.92,2.101
P1110,sitka-spruce,5.394,7.309,20,14.80,10.92,2.071
P1111,sitka-spruce,6.875,6.875,20,14.09,10.99,2.018
P1112,sitka-spruce,8.624,7.270,20,14.65,10.65,2.060
P1113,sitka-spruce,11.009,6.836,20,14.42,11.02,2.043
P1114,sitka-spruce,13.125,7.386,20,14.90,11.34,2.079
P1115,sitka-spruce,14.400,6.956,20,15.49,11.02,2.124
P1200,sitka-spruce,-14.400,9.185,20,12.24,10.05,1.876
P1201,sitka-spruce,-13.422,8.751,20,13.16,9.93,1.947
P1202,sitka-spruce,-10.769,9.143,20,15.26,11.74,2.106
P1203,sitka-spruce,-9.288,8.712,20,16.48,11.81,2.200
P1209,sitka-spruce,2.945,8.910,20,13.59,10.68,1.979
P1210,sitka-spruce,5.065,8.822,20,15.62,11.50,2.134
P1211,sitka-spruce,6.681,9.256,20,17.45,13.32,2.274
P1212,sitka-spruce,8.931,8.864,20,17.43,12.80,2.272
P1213,sitka-spruce,11.450,9.295,20,15.94,11.36,2.158
P1214,sitka-spruce,12.797,8.903,20,14.45,11.13,2.044
P1215,sitka-spruce,14.400,9.334,20,12.60,9.47,1.904
P1300,sitka-spruce,-14.400,11.334,20,10.95,9.04,1.777
P1301,sitka-spruce,-13.439,10.861,20,11.80,9.06,1.843
P1302,sitka-spruce,-10.921,11.295,20,15.84,11.90,2.151
P1303,sitka-spruce,-9.305,10.822,20,18.83,13.24,2.379
P1309,sitka-spruce,3.097,10.705,20,13.55,10.86,1.976
P1310,sitka-spruce,4.977,10.790,20,15.94,12.18,2.158
P1311,sitka-spruce,7.362,11.263,20,19.65,13.79,2.442
P1312,sitka-spruce,8.843,10.829,20,19.65,14.66,2.442
P1313,sitka-spruce,11.228,11.302,20,16.85,12.23,2.228
P1314,sitka-spruce,13.245,10.712,20,12.93,9.96,1.929
P1315,sitka-spruce,14.400,11.182,20,10.97,9.25,1.779
P1400,sitka-spruce,-14.400,12.811,20,10.89,9.16,1.773
P1401,sitka-spruce,-12.963,13.284,20,12.24,9.31,1.876
P1402,sitka-spruce,-10.578,12.850,20,16.54,12.84,2.204
P1403,sitka-spruce,-9.097,13.323,20,19.19,13.80,2.407
P1410,sitka-spruce,4.621,13.355,20,15.88,12.22,2.154
P1411,sitka-spruce,7.139,12.882,20,20.17,14.36,2.482
P1412,sitka-spruce,8.755,13.316,20,20.46,14.88,2.504
P1413,sitka-spruce,11.274,12.843,20,17.47,12.51,2.275
P1414,sitka-spruce,13.256,13.432,20,13.55,9.93,1.976
P1415,sitka-spruce,14.400,12.963,20,11.61,9.41,1.828
P1500,sitka-spruce,-14.400,14.400,20,12.79,10.14,1.918
P1501,sitka-spruce,-12.981,14.400,20,12.17,9.24,1.871
P1502,sitka-spruce,-10.730,14.400,20,15.32,11.89,2.111
P1503,sitka-spruce,-9.115,14.400,20,17.69,12.84,2.292
P1504,sitka-spruce,-6.864,14.400,20,20.60,14.62,2.514
P1505,sitka-spruce,-5.249,14.400,20,19.65,14.73,2.442
P1506,sitka-spruce,-2.998,14.400,20,16.41,12.21,2.195
P1507,sitka-spruce,-1.383,14.400,20,15.59,11.18,2.132
P1510,sitka-spruce,4.984,14.400,20,17.55,13.13,2.281
P1511,sitka-spruce,7.369,14.400,20,18.87,13.26,2.382
P1512,sitka-spruce,9.118,14.400,20,18.98,13.84,2.391
P1513,sitka-spruce,10.599,14.400,20,17.60,13.50,2.285
P1514,sitka-spruce,13.252,14.400,20,13.54,10.29,1.975
P1515,sitka-spruce,14.400,14.400,20,13.62,10.85,1.981
```

**The 14 pre-world H1 omissions** (source-export rows, with the harness label; **not** in the authored roster):

```text
P0004,sitka-spruce,-6.617,-14.400,20,12.41,9.46,1.889,south
P0005,sitka-spruce,-5.002,-14.400,20,13.70,10.95,1.987,south
P0006,sitka-spruce,-3.386,-14.400,20,17.68,12.55,2.291,south
P0207,sitka-spruce,-1.171,-11.277,20,17.03,12.04,2.242,south
P0306,sitka-spruce,-2.635,-8.956,20,14.54,11.36,2.052,south
P0307,sitka-spruce,-1.154,-9.386,20,14.29,10.51,2.033,south
P0403,sitka-spruce,-8.755,-7.263,20,19.86,14.29,2.458,south
P0404,sitka-spruce,-6.772,-6.674,20,21.17,14.95,2.558,south
P0405,sitka-spruce,-5.157,-7.146,20,19.76,14.77,2.450,south
P0406,sitka-spruce,-2.638,-6.712,20,14.81,10.82,2.072,south
P0407,sitka-spruce,-1.023,-7.185,20,11.90,9.72,1.850,south
P0503,sitka-spruce,-8.737,-5.376,20,18.58,13.82,2.360,south
P0505,sitka-spruce,-5.139,-5.256,20,18.36,13.02,2.343,south
P0507,sitka-spruce,-1.005,-5.295,20,11.55,8.62,1.823,south
```

## 6. World-geometry candidate (OWNER INTENT where marked; otherwise PROPOSED/SOURCE)

| Item | Value | Class |
| --- | --- | --- |
| Frame | C-B-30 source frame: x east, z north, metres; property bounds x, z ∈ [−15, 15]; 900 m² | OWNER INTENT (30 × 30 m, D-083) + SOURCE |
| Ecology grid | 6 × 6 cells of 5 m, `cell = floor((z+15)/5)·6 + floor((x+15)/5)`; source seed 20260914 recorded for provenance only | SOURCE (formula matches ledger cell 15); production `GetCellIndex` for a new Compact geometry is UNVERIFIED |
| H1 | rectangle x ∈ [−9, 3], z ∈ [−15, −5] (120 m²); historical track entry near (−0.93, −14.75) | OWNER INTENT approximate size (D-083); exact polygon PROPOSED |
| Pre-world omissions | the 14 IDs in §3 | PROPOSED (concept is OWNER INTENT) |
| Route A | (2.99, −4.75) → (2.80, −2.25) → (2.06, 0.25) → (0.90, 2.75) → (−0.43, 5.25) → (−1.68, 7.75) → (−2.59, 10.25) → (−2.71, 10.75); 16.671961 m | SOURCE anchors; the spline/interpolation algorithm is **MISSING** |
| Walking core / verge | 1.2 m symmetric core; optional ≤ 0.20 m **visual-only** verge | OWNER INTENT (D-084), unsigned physical bytes |
| G1 | O1 oak (1.17, −0.47); O2 oak (1.57, −2.73); B1 beech (3.43, −0.07); B2 beech (3.83, −2.33); **order** O1, O2, B1, B2 | OWNER INTENT (D-084 Geometry A) |
| Guard footprint target | every state (effective / expired / failed / with stakes) within r ≤ 0.10 m in plan view; failed state honestly damaged, Compact only | OWNER INTENT (D-084) |
| Historical objects | no 52-plank track, approach clones, headland, rack or Rack V2/MFM in Compact | OWNER INTENT (D-083) / engineering choice GQ-07 open |

## 7. Missing or non-authoritative fields — DATA GATE (not invented)

The bytes do **not** contain the following. Each must be authored, sourced or explicitly declared before signature or implementation.
1. **Terrain:** ground height `y` and slope/normal for trees, H1, the path and the G1 sites. There is no terrain data at all.
2. **Per-tree visual identity:** prefab/LOD variant, yaw/rotation, scale and defect flags.
3. **Per-tree simulation state** beyond age, DBH, height and crown: any internal growth/vigour, mortality RNG or history fields that `ForestTree`/the manager persist.
   - The source `ForestSaveData` tree schema was not compared field-by-field in this task. A save-contract reviewer must list the exact required set.
4. **Year-0 cell ecology:** understorey (bramble/bracken/grass), browse pressure, seed bank, ground vegetation and deadwood for the 36 cells. Only canopy cover and light proxies exist in the ledger.
5. **Path definition:** the spline/interpolation algorithm, terrain projection, per-segment width, the path end condition at A7, and a path-to-boundary rule.
6. **North terminus:** A7 (−2.71, 10.75) lies beside the historical buildables clearing. The 2.2 m anchors at z ≈ 9–12.8 explain a gap in the stand. The new world must author **what is there**, for example a path end or viewpoint, or accept a declared historical gap. The source says nothing about it.
7. **H1 detail:** surface, edge treatment, parking/collection layout, spawn point and orientation, and whether H1 stumps are shown. GQ-05 says "no fresh stumps"; this is not decided.
8. **Guard geometry:** mesh, stake count and positions, and the failed-state shape within r ≤ 0.10 m. None exists yet.
9. **Content key binding:** the registry string, the content version and the full world-content SHA-256 recipe that covers the roster, geometry and G1/guard.
10. **The 37/30 lattice split** (source-geometry versus mortality omissions). It is reported by #508 and not re-derived here. It is not needed to adopt the roster verbatim.

## 8. Signature block — **UNSIGNED**

| Field | Value |
| --- | --- |
| Content revision label | `Compact30-H1-RouteA-G1-content-v1` (reserved draft name only) |
| Roster artifact | §5, SHA-256 `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1` |
| Source evidence | C-B-30 `3e7c37ad…5a27` (128,518 B); ledger `aa10f54f…bcf3` (24,746 B); local Files only |
| Geometry | §6 (H1, Route A anchors, 1.2 m core, ≤ 0.20 m visual verge, G1 order, r ≤ 0.10 m guard target) |
| Open DATA GATE items | §7 items 1–9 |
| Independent document/source review | **NOT DONE** (must be by a non-author) |
| Content-owner signature | **NOT SIGNED** — ______ (name, date, exact SHA) |
| Manager confirmation of APPROVED parent/child/graphics specs | **NOT GIVEN** |
| World/content key registration and save binding | **NOT DONE** |

**Signing order (from #622 §5):**
1. The source custodian captures the bytes (done here, read-only).
2. An independent review of this annex and the spec convergence.
3. The content owner signs the exact revision, or returns an amendment.
4. The Manager confirms the specs are APPROVED.
5. A Linux source actor implements from the immutable bytes.
6. Native qualification and save admission.

After the first integrated Compact writer, any change to the roster, path, G1, guard footprint or cell meaning needs a **new reviewed content version** and a migration/refusal policy (signed SI-10).

## 9. Reproduction recipe (read-only)

Run from an empty scratch directory with **Python 3** and no third-party packages:

```
python3 -I -B roster.py <C-B-30.json> <ledger.json> <outdir>
```

The script reads the two files and writes `source189.csv`, `authored175.csv` and `roster-summary.json` to `<outdir>` only. The CSV bytes must hash to the values in §4.

```python
#!/usr/bin/env python3
"""#657 read-only roster extraction. Args: C-B-30.json ledger.json outdir. Writes only to outdir."""
import json, sys, hashlib, math, os, collections
cb, lg, out = sys.argv[1:4]
raw = open(cb, 'rb').read(); lraw = open(lg, 'rb').read()
d = json.loads(raw); L = json.loads(lraw)
T = d['trees']
res = {'cb_sha256': hashlib.sha256(raw).hexdigest(), 'cb_bytes': len(raw),
       'ledger_sha256': hashlib.sha256(lraw).hexdigest(), 'ledger_bytes': len(lraw),
       'ledger_input_cb_sha_matches': L['inputs']['C-B-30.json'] == hashlib.sha256(raw).hexdigest()}
ids = [t['id'] for t in T]
assert len(ids) == len(set(ids)) == 189
x0, x1, z0, z1 = L['H1']['rect']
def in_h1(t): return x0 <= t['x'] <= x1 and z0 <= t['z'] <= z1
def out_dist(t):
    dx = max(x0 - t['x'], 0, t['x'] - x1); dz = max(z0 - t['z'], 0, t['z'] - z1); return math.hypot(dx, dz)
inside = sorted(t['id'] for t in T if in_h1(t))
buf = sorted(t['id'] for t in T if not in_h1(t) and out_dist(t) <= 1.0)
res['h1_inside_recomputed'] = inside; res['h1_inside_ledger'] = sorted(r[0] for r in L['H1']['inside'])
res['buffer_recomputed'] = buf; res['buffer_ledger'] = sorted(r[0] for r in L['H1']['buffer1m'])
# ledger rounded coords vs source
byid = {t['id']: t for t in T}
res['h1_coord_maxdev'] = max(max(abs(byid[r[0]]['x']-r[1]), abs(byid[r[0]]['z']-r[2])) for r in L['H1']['inside'])
A = [tuple(p) for p in L['H1']['path']['samples']]
def segd(q):
    best = (1e9, None)
    for i in range(len(A)-1):
        (ax, az), (bx, bz) = A[i], A[i+1]; vx, vz = bx-ax, bz-az
        t = max(0, min(1, ((q[0]-ax)*vx + (q[1]-az)*vz)/(vx*vx+vz*vz)))
        dd = math.hypot(q[0]-ax-t*vx, q[1]-az-t*vz)
        if dd < best[0]: best = (dd, i)
    return best
res['routeA_len'] = sum(math.hypot(A[i+1][0]-A[i][0], A[i+1][1]-A[i][1]) for i in range(len(A)-1))
omit = set(inside); roster = [t for t in T if t['id'] not in omit]
res['roster_n'] = len(roster); res['side_counts_roster'] = dict(collections.Counter(t['side'] for t in roster))
res['side_counts_omitted'] = dict(collections.Counter(byid[i]['side'] for i in inside))
dists = sorted((segd((t['x'], t['z']))[0], t['id']) for t in roster)
res['routeA_nearest5'] = [(round(a, 4), b) for a, b in dists[:5]]
res['routeA_within_0.6'] = [b for a, b in dists if a <= 0.6]; res['routeA_within_0.8'] = [b for a, b in dists if a <= 0.8]
# trunk-edge clearance for nearest stem: d - dbh/200 - 0.6
res['routeA_trunk_edge_margin_1.2core_min'] = min((round(segd((t['x'],t['z']))[0]-t['dbh']/200-0.6,4), t['id']) for t in roster)
G1 = {'O1': ('sessile-oak', 1.17, -0.47), 'O2': ('sessile-oak', 1.57, -2.73), 'B1': ('beech', 3.43, -0.07), 'B2': ('beech', 3.83, -2.33)}
def cell(x, z): return int(math.floor((z+15)/5))*6 + int(math.floor((x+15)/5))
g = {}
for k, (sp, x, z) in G1.items():
    dd, seg = segd((x, z)); near = sorted((math.hypot(t['x']-x, t['z']-z), t['id']) for t in roster)[:3]
    g[k] = {'species': sp, 'x': x, 'z': z, 'cell_floorgrid': cell(x, z), 'routeA_d': round(dd, 6), 'seg': f'A{seg}-A{seg+1}',
            'core_margin_r0.10': round(dd-0.6-0.10, 3), 'core_plus_verge_margin': round(dd-0.6-0.10-0.20, 3),
            'nearest3_roster': [(round(a, 3), b, byid[b]['side']) for a, b in near], 'inH1': x0 <= x <= x1 and z0 <= z <= z1}
res['G1'] = g
ks = list(G1); res['G1_pairwise'] = {f'{a}-{b}': round(math.hypot(G1[a][1]-G1[b][1], G1[a][2]-G1[b][2]), 3) for i, a in enumerate(ks) for b in ks[i+1:]}
res['bounds_ok'] = all(abs(t['x']) <= 15 and abs(t['z']) <= 15 for t in T)
res['cells_roster'] = dict(sorted(collections.Counter(cell(t['x'], t['z']) for t in roster).items()))
res['anchors_n'] = len(d['anchors'])
res['ba_roster'] = round(sum(t['ba'] for t in roster), 4); res['vol_roster'] = round(sum(t['volume'] for t in roster), 4)
res['ba_189'] = round(sum(t['ba'] for t in T), 4); res['vol_189'] = round(sum(t['volume'] for t in T), 4)
res['omitted_salevol'] = round(sum(byid[i]['saleVolume'] for i in inside), 5)
res['dbh_range'] = (round(min(t['dbh'] for t in roster), 2), round(max(t['dbh'] for t in roster), 2))
res['h_range'] = (round(min(t['height'] for t in roster), 2), round(max(t['height'] for t in roster), 2))
# canonical serialisation
HDR = {'source189': 'ccf-c-b-30-source-roster-export;v=1;fields=id,species,x_m,z_m,age_y,dbh_cm,height_m,crown_m,harness_position_label;order=id-ascending;lf',
       'authored175': 'ccf-compact30-authored-roster-candidate;v=0-UNSIGNED;fields=id,species,x_m,z_m,age_y,dbh_cm,height_m,crown_m;order=id-ascending;lf'}
def row(t, lab): return f"{t['id']},{t['species']},{t['x']:.3f},{t['z']:.3f},{t['age']},{t['dbh']:.2f},{t['height']:.2f},{t['crown']:.3f}" + (f",{t['side']}" if lab else '')
for name, rows in (('source189', T), ('authored175', roster)):
    body = HDR[name] + '\n' + '\n'.join(row(t, name == 'source189') for t in sorted(rows, key=lambda t: t['id'])) + '\n'
    open(os.path.join(out, f'{name}.csv'), 'w', newline='\n').write(body)
    res[f'{name}_canon_sha256'] = hashlib.sha256(body.encode()).hexdigest(); res[f'{name}_canon_bytes'] = len(body.encode())
# rounding sanity: mm rounding preserves ordering of route distances
res['max_round_shift_m'] = max(max(abs(round(t['x'], 3)-t['x']), abs(round(t['z'], 3)-t['z'])) for t in T)
json.dump(res, open(os.path.join(out, 'roster-summary.json'), 'w'), indent=1)
print(json.dumps(res, indent=1))
```

## 10. Non-claims

This annex does **not** sign a world or content key, compute a Unity world hash, run Unity or the generator, validate terrain, colliders or the walk, or certify vehicle/road safety. It does not assert native Year-1+ ecology, survival or promotion, and it does not create a scene, save, profile or asset. It is a candidate for **independent review and actual content-owner signature**.
