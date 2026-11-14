# scen1 — three-species long-horizon native research result (#490, 30 m finite woodland, Year 0–40)

**14 November 2026 — NATIVE RESEARCH RUN COMPLETE / HANDED OFF. Research only; not acceptance, not a product decision.**

| | |
|---|---|
| Worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code in local T3 Code, Linux; no sub-agents |
| Registration | <https://github.com/jermur788/CCF/issues/490#issuecomment-6874806702>; method pre-registered there before any outcome |
| Branch | `research/scen1-three-species-long-horizon-native-20261114-v1` |
| Start | `main` `fc18190b976f1363f1b5d1b0881810db2f8c6e39` |
| Worktree | `/media/jer/ZX20/CCF-scen1-LongHorizon-20261114` (reported alternative to the proposed Files path; see handoff) |
| Engine | Unity **6000.6.0f1**, batch mode in a bubblewrap private namespace |
| Run | Editor exit **0**, 612.7 s including cold import, **142/142** fixture checks PASS |
| Evidence | `/media/jer/Files/CCF/Verification/scen1-ThreeSpeciesLongHorizon490-20261114T085531Z/` (50 MB, 121 files, `evidence-manifest.json` SHA-256 `c4fbcc1b9f7784c051696d5faf760309d48e346520183d904ffc10ec21885c42`) |

**Scope.** This is the authentic #460 30 × 30 m scratch forest. It is a finite bounded woodland with no exterior trees, buffer or hedgerow donors.

Two arms, identical except one Year 5 treatment, both run on genuine `ScenarioOneManager.AdvanceYear` ticks to elapsed **Year 40**. There was:
- no production code, no tracked asset change and no save;
- no skip controller, no forged acknowledgement and no parameter tuning;
- no forced promotion and no browsing change.

Distinctions used throughout:
- **MEASURED** = recorded by the native run.
- **SOURCE DERIVATION** = read from code, not separately instrumented.
- **NOT MEASURED** = absent from the raw data.

## 1. Result in brief

1. **The model ran 40 genuine annual ticks per arm with no exception, no NaN/Infinity value and no blocking gate.** `ReviewYear` is max(25, 100) = 100 and cash stayed positive, so nothing stopped it (SOURCE DERIVATION + MEASURED). The Year-25 completion gate did not trigger, because it needs a reviewed two-cycle management sequence.
2. **The fixture reproduces #460 exactly.**
   - Both arms' initial 189 trees are equal to #460 `C-B-30.json`.
   - Arm B's Year 0–8 recorded states (living, cash, BA, volume, canopy, light, cohort bands, juvenile JSON) are equal to #460's.
   - Arm A's Year 0–4 states are equal to #460's and to Arm B's.
3. **Both planted broadleaves survived 40 years and became trees.**
   - The owner-sheltered **Oak** promoted in **Year 10** (3.75 m). The contractor-planted, unsheltered **Beech** promoted in **Year 11**.
   - At Year 40 both are age 42 (nursery age included): **Oak 12.3 cm DBH / 18.1 m**, **Beech 15.0 cm / 19.4 m**.
   - Their render models changed: Oak `Young_01` → `Mature_02` by Year 30; Beech `Mature_01`.
   - They grew under heavy Sitka competition: CI 14–15 at Year 20, falling to 4–5 by Year 40 as Sitka self-thinned. Crowns stayed narrow (Oak crown radius 0.29–0.45 m).
4. **No natural Oak or Beech regeneration appeared in 40 years, in either arm.**
   - Every one of the 146/143 natural cohort bands at Year 40 is Sitka.
   - The two planted broadleaves only began seed potential at Year 40 (Oak 0.003, Beech 0.010). Oak maturity onset is age 40.
   - In a bounded woodland without external donors, broadleaf continuity in this window depends entirely on the two planted trees reaching maturity. This is a finding, not a failure to fix.
5. **The unmanaged Sitka stand self-thinned heavily** from Year 13.
   - Original planted Sitka fell from 170 to **64** (Arm A) and from 168 to **68** (Arm B); 106 / 100 original stems died.
   - BA plateaued around 7 m², mean Sitka DBH rose to 35 cm and mean canopy fell 0.98 → 0.81–0.85.
   - Deaths are the engine's `"self-thinning"` density mortality (SOURCE DERIVATION). Per-tree cause is NOT MEASURED, because dead trees leave no deadwood record and drop out of scene enumeration.
   - Per source reading (not separately instrumented), growth model 2 moves the stand onto the species-aware occupancy density route once a non-Sitka tree is living (the Oak, from Year 10). The first death came in Year 13.
   - A few natural Sitka cohorts promoted to adult trees (`R39-*`, `R40-*`; 4 in A, 2 in B at Year 40).
6. **The one Year 5 release (Arm B) had a small, consistent effect.**
   - It removed P0001 and P1505 (2 stems, 0.079 m² BA) for €2,500 contractor cost and €36.92 timber.
   - By Year 40, crop tree P0000 was 49.99 vs 47.30 cm DBH (+2.69 cm) and P1504 was 44.92 vs 43.58 cm (+1.34).
   - Stand BA was +4.1 % and volume +2.2 %. Arm B ended €2,463.08 poorer.
   - Arm differences after Year 5 combine the treatment with downstream divergence of deterministic-but-state-dependent mortality, so no isolated causal size is claimed.
   - The broadleaves were essentially unaffected (Oak 12.22 vs 12.28 cm).

## 2. Data gate, fixture and protocol

* **#460 raw evidence (read-only, unchanged before/after):**
  - `native-result.json` `58524ad4…de40` and `executed-harness.cs` `3c0ebae8…9863` match the report pins.
  - `C-B-30.json` `3e7c37ad…5a27` matches its manifest entry.
  - It has 189 Sitka, age 20, seed 20260914, models 1/2/2/0, and a Year 0–9 state history.
* **Source.**
  - `ScenarioOneManager.AdvanceYear` runs in this order: settle approved work → `ForestEcologyController.AdvanceOneYear` → planted juveniles → understorey → deadwood → snapshot → `EvaluateProgress`.
  - `CanManage` blocks only at `ReviewYear` (100) or on a Failed outcome.
  - Planted juveniles promote via `JuvenileEcologyRules.CanPromote`; Oak needs 3.5 m height and light ≥ 0.2.
  - Source blobs are those at `fc18190b`. #460 used `6c13a9e1`, and the fixture's identity checks show identical behaviour through Year 8.
* **Harness.** `Scen1LongHorizon490Native.cs`, SHA-256 `c7291f7a2a09afafa324769838b696178b9b00dd62b8e08ee0fbcb81b65126cb`; the copy in evidence is the executed file.
  - It is a mechanical derivative of #460's executed harness. `Build`, `MeasureInitial`, `Harvest`, `ReleaseSet` and `PlantPositions` are verbatim.
  - Only the run sequence, the snapshot export, the fixed cameras and the isolation identifiers changed.
  - Supporting scripts: launcher `run-native.py` `23abd8ab…8a61`, custody `custody.py` `e2c2e662…1959`, offline analysis `analyse.py` `7c5b6a5c…329d`.
  - The harness was untracked, staged only in the isolated worktree, and removed after the run together with its generated `.meta`.
* **Protocol**, as pre-registered:
  - **Shared, Year 1:** access corridor of 19 stems, priced €2,500 with €98.22 revenue, settled exactly. Crop marks P0000/P1504. The #460 zero-cost hypothetical quotes were created and cancelled.
  - **Shared, Year 2:** Oak (owner, sheltered) and Beech (contractor) planted at (1.7, −3.3) and (3.3, −3.3); €15.75 total.
  - **Shared, Years 3–4:** quiet.
  - **Year 5:** Arm B ran #460 `ReleaseSet(crops,1)` through the normal mark → Work Plan → approve → advance, and the quote matched settlement. Arm A was quiet.
  - **Years 6–40:** quiet ticks in both arms. Years 9, 10 and 20 were observation-only.
  - The Year 1 corridor in 30 m is 19 stems; the packet's "13" refers to the 20 m candidate.

## 3. Stand trajectory (MEASURED)

| Year | Arm | Living trees (Sitka / Oak / Beech) | Mean Sitka DBH cm | Mean Sitka height m | BA m² | Biological volume m³ | Mean canopy | Mean light (min–max) | Natural cohort bands (Sitka density) | Cash € |
|---:|---|---|---:|---:|---:|---:|---:|---|---|---:|
| 0 | A | 189 (189 / 0 / 0) | 15.43 | 11.57 | 3.650 | 22.21 | 0.980 | 0.020 (0.000–0.221) | 0 (0.00) | 12,000.00 |
| 0 | B | 189 (189 / 0 / 0) | 15.43 | 11.57 | 3.650 | 22.21 | 0.980 | 0.020 (0.000–0.221) | 0 (0.00) | 12,000.00 |
| 5 | A | 170 (170 / 0 / 0) | 17.70 | 14.24 | 4.304 | 32.22 | 0.929 | 0.071 (0.001–0.583) | 42 (2.65) | 9,582.47 |
| 5 | B | 168 (168 / 0 / 0) | 17.64 | 14.20 | 4.222 | 31.49 | 0.925 | 0.075 (0.001–0.583) | 42 (2.69) | 7,119.39 |
| 9 | A | 170 (170 / 0 / 0) | 19.51 | 16.14 | 5.214 | 44.10 | 0.913 | 0.087 (0.001–0.633) | 65 (7.23) | 9,582.47 |
| 9 | B | 168 (168 / 0 / 0) | 19.45 | 16.09 | 5.120 | 43.15 | 0.909 | 0.091 (0.001–0.633) | 66 (7.56) | 7,119.39 |
| 10 | A | 171 (170 / 1 / 0) | 19.95 | 16.58 | 5.455 | 47.35 | 0.910 | 0.090 (0.001–0.639) | 66 (7.87) | 9,582.47 |
| 10 | B | 169 (168 / 1 / 0) | 19.90 | 16.53 | 5.358 | 46.33 | 0.907 | 0.093 (0.001–0.639) | 66 (8.24) | 7,119.39 |
| 20 | A | 149 (147 / 1 / 1) | 24.16 | 19.87 | 7.010 | 74.11 | 0.912 | 0.088 (0.002–0.459) | 97 (10.70) | 9,582.47 |
| 20 | B | 148 (146 / 1 / 1) | 24.07 | 19.79 | 6.907 | 72.67 | 0.917 | 0.083 (0.002–0.459) | 98 (10.48) | 7,119.39 |
| 25 | A | 121 (119 / 1 / 1) | 27.07 | 21.62 | 7.061 | 80.29 | 0.887 | 0.113 (0.002–0.489) | 128 (13.96) | 9,582.47 |
| 25 | B | 121 (119 / 1 / 1) | 26.75 | 21.36 | 6.934 | 78.39 | 0.888 | 0.112 (0.002–0.397) | 126 (14.05) | 7,119.39 |
| 30 | A | 100 (98 / 1 / 1) | 29.94 | 23.03 | 7.119 | 86.05 | 0.868 | 0.132 (0.001–0.545) | 140 (16.58) | 9,582.47 |
| 30 | B | 101 (99 / 1 / 1) | 29.60 | 22.70 | 7.075 | 84.90 | 0.871 | 0.129 (0.001–0.545) | 136 (16.93) | 7,119.39 |
| 40 | A | 70 (68 / 1 / 1) | 35.00 | 24.24 | 6.942 | 90.89 | 0.812 | 0.188 (0.008–1.000) | 146 (18.02) | 9,582.47 |
| 40 | B | 72 (70 / 1 / 1) | 35.59 | 24.41 | 7.228 | 92.92 | 0.848 | 0.152 (0.009–0.568) | 143 (18.86) | 7,119.39 |

Annual mortality, 5-year sums from Years 6–10 onward:
- **A:** 0, 10, 17, 32, 25, 25, 19
- **B:** 0, 7, 19, 31, 24, 26, 15

Mast years are recorded per year in `arm*-years.json`. Storms are off (storm model 0).

## 4. Planted broadleaves (MEASURED, Arm A; Arm B equal to two decimals except where noted)

| Year | Oak (PJ0001 → PL-PJ0001) | Beech (PJ0002 → PL-PJ0002) |
|---:|---|---|
| 5 | planted juvenile, age 7, height 2.00 m, alive True | planted juvenile, age 7, height 1.69 m, alive True |
| 9 | planted juvenile, age 11, height 3.40 m, alive True | planted juvenile, age 11, height 3.08 m, alive True |
| 10 | tree, age 12, DBH 5.62 cm, height 3.75 m, crown radius 0.69 m, CI 0.00, seed potential 0.000; render `Sessile_Oak_Young_01_Unpruned` | planted juvenile, age 12, height 3.43 m, alive True |
| 20 | tree, age 22, DBH 7.46 cm, height 9.28 m, crown radius 0.33 m, CI 15.03, seed potential 0.000; render `Sessile_Oak_Young_01_Unpruned` | tree, age 22, DBH 7.92 cm, height 9.52 m, crown radius 0.68 m, CI 14.11, seed potential 0.000; render `Beech_Mature_01_Bark` |
| 25 | tree, age 27, DBH 8.41 cm, height 11.75 m, crown radius 0.29 m, CI 13.06, seed potential 0.000; render `Sessile_Oak_Young_01_Unpruned` | tree, age 27, DBH 9.31 cm, height 12.33 m, crown radius 0.63 m, CI 11.39, seed potential 0.000; render `Beech_Mature_01_Unpruned` |
| 30 | tree, age 32, DBH 9.54 cm, height 14.03 m, crown radius 0.31 m, CI 10.20, seed potential 0.000; render `Sessile_Oak_Mature_02_Unpruned` | tree, age 32, DBH 10.98 cm, height 14.90 m, crown radius 0.71 m, CI 8.21, seed potential 0.000; render `Beech_Mature_01_Unpruned` |
| 40 | tree, age 42, DBH 12.28 cm, height 18.10 m, crown radius 0.45 m, CI 4.90, seed potential 0.003; render `Sessile_Oak_Mature_02_Unpruned` | tree, age 42, DBH 15.00 cm, height 19.40 m, crown radius 1.04 m, CI 3.92, seed potential 0.010; render `Beech_Mature_01_Unpruned` |

- **Arm B at Year 40:** Oak 12.22 cm, CI 7.41; Beech 14.91 cm, CI 5.59. Heights are identical to Arm A.
- **Juvenile ages.** `PlantedJuvenile.ageYears` stops at promotion (12 and 13); the tree age carries on.
- **What "stage" means.** All living trees report game stage `Mature`, which is an interaction stage, not a biological stage. The biological signals are the age, size and render-model columns.
- **Model note, not validated.** The very narrow Oak crown (radius about 0.3–0.45 m at 14–18 m height) follows from the shared crown law under suppression. It is recorded as a model observation, not judged against field data.

## 5. Crop trees and the treatment (MEASURED)

| Year | P0000 DBH cm A / B | P0000 CI A / B | P1504 DBH cm A / B | P1504 CI A / B |
|---:|---|---|---|---|
| 5 | 25.72 / 25.81 | 1.88 / 1.07 | 23.52 / 23.56 | 3.43 / 2.83 |
| 9 | 28.43 / 28.87 | 1.90 / 1.09 | 25.78 / 26.00 | 3.45 / 2.84 |
| 10 | 29.09 / 29.62 | 1.91 / 1.09 | 26.34 / 26.60 | 3.46 / 2.84 |
| 20 | 35.48 / 36.81 | 1.84 / 1.01 | 32.02 / 32.68 | 2.45 / 1.90 |
| 25 | 38.52 / 40.21 | 1.86 / 1.02 | 34.94 / 35.85 | 2.27 / 1.63 |
| 30 | 41.51 / 43.56 | 1.61 / 0.77 | 37.90 / 38.97 | 2.06 / 1.65 |
| 40 | 47.30 / 49.99 | 1.51 / 0.66 | 43.58 / 44.92 | 1.66 / 1.22 |

## 6. Regeneration, seed and light

- **Natural cohorts are Sitka only in both arms**, origin `Natural`. They grew from 10 bands at Year 1 to 146 (A) and 143 (B) at Year 40; the maximum cohort height stayed around 3.4–4.3 m.
- **Sitka seed potential** was above zero from Year 5 (age 25) and peaked around 1.0–1.26 at Years 25–30.
- **Planted broadleaves:** zero seed potential until Year 40 (0.003 / 0.010).
- **No Oak or Beech cohort ever appeared.** No donor was invented. Hedgerow or exterior seed supply was explicitly excluded by the owner clarification.
- **Light:** mean cell light rose from 0.02 to 0.19 (A) and 0.15 (B). One Arm A cell reached light 1.0 at Year 40, an opening created by self-thinning.
- **Understorey, deadwood and full scenario state** were exported at Years 9, 10, 20, 25, 30 and 40 (`arm*-scenario-state-y*.json`). Bramble and bracken figures are in that raw data but were not analysed here: NOT ANALYSED, not zero.

## 7. Rendered evidence (actual Unity captures; not field validation or a player test)

Fixed cameras:
- **Stand-oblique:** from (0, 26, −40) looking at (0, 7, 0).
- **Broadleaf eye-level:** 1.7 m high, 9 m south of the two planted trees.
- Both at 1280×720 and 1920×1080, for both arms, at Years 20 and 40. Year 0/9 overhead references are 1200×1200.

| Image | Size | Pixel std-dev (R, G, B) | Visual verdict |
|---|---|---|---|
| `armA-C-B-30-overhead-y00.png` | 1200×1200 | 19.0, 22.2, 13.2 | inspected: valid overhead, full canopy and corridor |
| `armA-C-B-30-overhead-y09.png` | 1200×1200 | 32.8, 21.3, 13.2 | non-black by statistics; not individually inspected |
| `armA-broadleaf-eye-y20-1080p.png` | 1920×1080 | 21.9, 19.9, 20.0 | non-black by statistics; not individually inspected |
| `armA-broadleaf-eye-y20-720p.png` | 1280×720 | 21.9, 19.9, 20.2 | inspected: two young broadleaves (green markers) legible under Sitka canopy |
| `armA-broadleaf-eye-y40-1080p.png` | 1920×1080 | 33.6, 34.2, 38.3 | non-black by statistics; not individually inspected |
| `armA-broadleaf-eye-y40-720p.png` | 1280×720 | 33.9, 34.5, 38.9 | inspected: broadleaf legible, spreading mature-form crown among tall Sitka |
| `armA-stand-oblique-y20-1080p.png` | 1920×1080 | 53.7, 53.1, 57.0 | non-black by statistics; not individually inspected |
| `armA-stand-oblique-y20-720p.png` | 1280×720 | 53.8, 53.3, 57.1 | non-black by statistics; not individually inspected |
| `armA-stand-oblique-y40-1080p.png` | 1920×1080 | 48.2, 45.4, 48.1 | non-black by statistics; not individually inspected |
| `armA-stand-oblique-y40-720p.png` | 1280×720 | 48.5, 45.7, 48.5 | inspected: tall closed Sitka stand, access path visible; broadleaves hidden inside |
| `armB-C-B-30-overhead-y00.png` | 1200×1200 | 16.6, 2.0, 4.3 | **DEFECTIVE** — bare ground, no trees rendered (first frame after the second rebuild; biology unaffected, identity checks pass) |
| `armB-C-B-30-overhead-y09.png` | 1200×1200 | 29.2, 20.8, 12.5 | non-black by statistics; not individually inspected |
| `armB-broadleaf-eye-y20-1080p.png` | 1920×1080 | 15.1, 12.6, 12.1 | non-black by statistics; not individually inspected |
| `armB-broadleaf-eye-y20-720p.png` | 1280×720 | 15.2, 12.6, 12.2 | inspected via 1080p twin statistics; partly occluded by a near Sitka crown |
| `armB-broadleaf-eye-y40-1080p.png` | 1920×1080 | 15.0, 11.8, 11.7 | non-black by statistics; not individually inspected |
| `armB-broadleaf-eye-y40-720p.png` | 1280×720 | 15.1, 11.8, 11.9 | inspected: broadleaf trunk visible but view partly occluded by a near Sitka crown |
| `armB-stand-oblique-y20-1080p.png` | 1920×1080 | 53.8, 53.0, 57.0 | non-black by statistics; not individually inspected |
| `armB-stand-oblique-y20-720p.png` | 1280×720 | 54.0, 53.1, 57.1 | non-black by statistics; not individually inspected |
| `armB-stand-oblique-y40-1080p.png` | 1920×1080 | 48.9, 46.0, 49.0 | non-black by statistics; not individually inspected |
| `armB-stand-oblique-y40-720p.png` | 1280×720 | 49.2, 46.4, 49.4 | non-black by statistics; not individually inspected |

**Visual reading.**
- At Year 40 the oblique view shows a tall, closed, single-storey Sitka block. The broadleaves are **not visible from outside**.
- At eye level in Arm A, the Beech/Oak is legible as a spreading broadleaf trunk and crown among Sitka stems. In Arm B the same view is partly blocked by a Sitka crown that survived there; this is an arm-specific camera occlusion, not a biological difference.
- Orange and green ground rings are source marker visuals and were not interpreted.

## 8. What this does and does not establish

- **Establishes, for this fixture only:**
  - the current engine runs a 30 m finite three-species woodland stably through 40 annual ticks;
  - planted Oak and Beech can survive and promote under Sitka cover;
  - natural broadleaf regeneration does not arise within 40 years without external donors;
  - unmanaged Sitka self-thins strongly;
  - one modest release has a small positive crop-tree effect at real cost.
- **Does not establish:**
  - Irish field accuracy, or mature-broadleaf form and seed beyond Year 40;
  - learner value of a 30 m world, or a product choice of 30 m;
  - two-versus-three species policy, or hedgerow/exterior mechanics;
  - any Year 40 promise.
- **D-059/D-060** remain untouched.
- **Not run:** old-save and Reference Future replay hashes, the integrated suite, and any Windows build.
- **Next authority:** Manager #424 research verdict, then owner/product decisions (see handoff).
