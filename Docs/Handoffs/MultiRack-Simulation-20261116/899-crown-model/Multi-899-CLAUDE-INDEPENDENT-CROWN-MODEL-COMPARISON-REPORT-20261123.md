# CCF #899: Independent comparison of Candidate A (CrownModel 1 / C2) and Candidate B (eight-sector directional crowns)

**Status: INDEPENDENT READ-ONLY REVIEW. NO CODE, FIXTURE, SVG, UNITY OR PARAMETER CHANGES. OWNER G5 DECISION NEEDED.**
**Reviewer:** Claude (Opus 5.5), separate session, 2026-11-23. **Verdict: CANDIDATE A ("LEON") PREFERRED.** Not a hybrid. Two checks belong in the spec before any code is written (§8).

Requested by: [comparison packet](https://github.com/jermur788/CCF/blob/docs/multi-899-claude-leon-vs-directional-crown-review-20261123-v1/Docs/Handoffs/MultiRack-Simulation-20261116/899-crown-model/Multi-899-CLAUDE-INDEPENDENT-LEON-VS-DIRECTIONAL-CROWN-MODEL-COMPARISON-PACKET-20261123.md) (packet commit `aa402bf8`) and the owner's Gemini design-comparison handoff, both supplied in the same session.

---

## 1. Plain-English answer for the owner

- **Why you can't see the ground.** Every tree starts at the full crown width of a tree grown alone in a field. Area A starts with 4.1 times its own area in crowns. The "relax 15% a year" rule then shrinks those crowns for years whether or not anything is felled, so felling barely shows. I reproduced this exactly. Without any felling, Area A's cover drifts from 99.8% down to 91.3% over 10 years. Ten scattered fells leave about 3 m² of bare ground.
- **Both proposals fix that first problem the same way.** Each starts every crown at the size its crowding allows ("Year-0 equilibrium"). After that fix, the two look almost the same on the map. In every felling test I ran, they stay within about 2 percentage points of cover.
- **Candidate A matches real Irish trees with nothing tuned.** The Teagasc Frenchpark thinning trial (Co. Roscommon) measured Sitka crowns in unthinned and thinned plots. A's formula, unchanged, predicts crown radii within +2–7% of those measurements. It also predicts how much wider crowns get after thinning, to within about 2 percentage points. B only gets close after I fit its main coefficient to the same data. Even then, its thinning-response error is three to four times A's. B cannot match both the crowded crown size and the thinning response with one coefficient; A matches both with none.
- **B's extra machinery doesn't show.** B tracks crowns in eight compass directions, but it has to report one circle to the 2D map and the 3D tree mesh. That throws away the directional detail, which is its whole advantage. No data exists yet to pin down B's extra coefficients: changing its reach coefficient or sector count makes no difference once its main coefficient is refitted.
- **B breaks in very dense young stands.** At 10,000 stems/ha its crowns shrink to about 5% of open-grown size, which would show false holes in a regeneration thicket. A keeps crowns at about 0.6 × tree spacing at every density, so dense stands stay closed without any hard cap.
- **What to expect if you choose A.** Felling ten trees together in Area A opens about 38 m² of new bare ground at once, as one gap about 5.5 m across. Neighbours close it slowly, with about 29 m² still open after 10 years. Ten scattered single fells in such a dense stand give only thin slivers, about 17 m² in total, because real crowns there nearly touch. That is honest biology, not a fault.

---

## 2. Sources compared (exact)

| Item | Source | Stamp |
|---|---|---|
| **Candidate A** ("Leon's solution" in the packet) | Owner-supplied Gemini handoff "CrownModel 1 / Option C2", published verbatim on this branch as [Multi-899-CANDIDATE-A-OWNER-SUPPLIED-GEMINI-C2-PROPOSAL-VERBATIM-20261123.md](./Multi-899-CANDIDATE-A-OWNER-SUPPLIED-GEMINI-C2-PROPOSAL-VERBATIM-20261123.md) | file SHA-256 `bb80e53270357067752eacccfe0e3c9124279c0f2b228ca6b29783cba0aeb506` (includes a provenance header) |
| **Candidate B** (GPT-6 directional) | [Original proposal](https://github.com/jermur788/CCF/blob/docs/multi-899-independent-sector-crown-alternative-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/899-crown-model/Multi-899-INDEPENDENT-DIRECTIONAL-CROWN-ALTERNATIVE-CODE-FREE-COMPARISON-20261122.md) | blob `4f93cafa354177432dd014aee477e5dbf6310c4a`, branch head `4669a9ffdc2fe5bf119f705c19f5ac0456e0721d` |
| Lab engine | `Tools/MultiRackSitkaP1Lab` at `feat/multi-885-astra-opus-ui-20261122-v1` head `1e4e02215d373e224fe6a74074ae3f62983db242` | `AnnualRules.cs` `dc299b32…`, `CcfSourcePins.cs` `205d975c…`, `DerivedManifest.cs` `b8d8bd7a…`, `MultiAreaGenerator.cs` `b3827865…`, `MultiAreaExperiment.cs` `79102bb3…`; identical at `a692e1bf` and `a42720bd` |
| Compact30 roster | `Assets/ForestPrototype/ScenarioOne/Compact30/Resources/Compact30Roster.json` at `31fbe46ec5773ab65fee4f2fda68119d2bdc107b` | blob `5a13fc9860a212a4281ede30780568538a4e4264` |
| Unity facts spot-checked | `origin/main` `3a67d0987be0cf40d007892739d2ad295c84f0e0` | see §6 |
| Offline analysis | [`evidence-claude-20261123/crown899.py`](./evidence-claude-20261123/crown899.py), output [`results.json`](./evidence-claude-20261123/results.json) | script SHA-256 `01fa1b2f10aa2903977ffbb659d2170ad68b9aceb56bf56177ad0019987e7e73`; 107 s in pure Python, deterministic |

**Who "Leon" is.** No document called "Leon" exists in the repository or the #899 comments. The [#899 comment of 2026-11-22 23:28](https://github.com/jermur788/CCF/issues/899) names **Gemini Neon** as the Candidate A author. The owner then supplied Gemini's C2 handoff alongside this dispatch. I treat that text as Candidate A. If the owner meant a different "Leon" proposal, the A-side findings here do not apply to it.

**MA-A/MA-B provenance (the packet asked for this).** I traced it. `MultiAreaExperiment.GenesisTrees` initialises both areas with `DerivedManifest.CrownBitsFor(dbhMm)`, which is the open-grown potential with zero competition. That is the same rule as the P1 30 m fixture. Compact30 file crowns also equal open-grown potential (max |difference| 0.0008 m). I regenerated MA-A/MA-B from an integer port of generator v1. The output matches the published figures exactly: MA-A 171 trees, 915 m², 1,869 stems/ha, BA 74.0 m²/ha, relative density 0.80; MA-B 73 trees, 1,500 m², 487 stems/ha, BA 39.5.

---

## 3. What each candidate changes

| | Candidate A (C2) | Candidate B (directional) |
|---|---|---|
| Year-0 crowns | `r0 = target(CI0)` | `R_k,0` = sector targets ("reconstruct"); same idea |
| Target | `r_pot / √(1 + CI/Ci50)`: the existing competition multiplier applied to crown **area** | `R_k = r_pot · exp(−β·C_k)`, eight sector pressures `C_k = Σ max(0,cos Δθ)·(Dj/Di)^p·e^(−d/L)` |
| Annual update | existing relaxation `k` (0.15 Sitka/Beech, 0.12 Oak), both directions | `+min(vMax, γ·max(0, target−R))`, expansion only |
| Output | scalar `CrownRadiusM` | area-equivalent circle `√(ΣR_k²/8)` |
| New parameters | **none** (reuses each species' `Ci50`, `k`) | **p, L, β, γ, vMax, sector count**: all unfitted |
| New saved state | none (a `CrownModel` version flag) | 8–16 floats per tree |
| Proposal's own evidence | offline runs on MA-A/B/Compact30 (reproduced below) | none (concept only, stated honestly) |

---

## 4. Evidence base

| Evidence | What it supports | Class |
|---|---|---|
| Tabbush & White 1988 ([doi](https://doi.org/10.1093/forestry/61.1.23)): `r = 0.9415 + 0.07635·DBH` | Open-grown **ceiling** only (n = 72, DBH 7–80 cm in the regression) | CLOSE TRANSFER (UK) |
| **Frenchpark, Co. Roscommon, Teagasc thinning trial**: Fernandez, G. (2017) *The influence of thinning on tree stability in Sitka spruce*, UCD research MSc ([PDF](https://irishriverproject.com/wp-content/uploads/2025/10/7.-2017_Gonzalo_Fernandez_MScThesis-The-influence-of-thinning-on-tree-stability.pdf)) | **Realised stand-grown crown widths, unthinned vs thinned.** Planted 1995 at 2 × 2 m, yield class 24. First thinning 2010 at age 15 (Table 2): unthinned 2,133, light 1,500, medium 1,230, heavy 1,015 stems/ha. Crowns measured 2015, five seasons later. Trees were the mean-DBH trees, 4 per plot × 3 plots (n = 12 per treatment); the felled crown was laid flat and its maximum width taken. **Table 12 (p. 36) group means: unthinned 2.56 m at DBH 19.5 cm, light 2.90 m at 20.8, medium 3.20 m at 22.2, heavy 3.26 m at 22.1.** The thesis text (p. 16) instead gives sample DBH 19 / 20.5 / 22 / 23.5 cm, and Fig. 11 rounds widths to 2.6 / 2.9 / 3.2 / 3.3 m. I tested both readings. All 60 trees: mean width 2.97 m, SD 0.51, range 1.80–4.10 | **IRISH DIRECT**: the same trial the project evidence composite §5 already uses for DBH |
| Davies & Pommerening 2008 ([doi](https://doi.org/10.1016/j.foreco.2008.03.052)), via composite §8 | Adding *spatial* competition to a DBH-based crown-radius model raised Sitka efficiency only **0.75 → 0.77**. The pooled Hegyi coefficient should not be read causally | CLOSE TRANSFER (Wales) |
| Crookston & Stage 1999 ([RMRS-GTR-24](https://www.fs.usda.gov/fmsc/ftp/fvs/docs/gtr/percancv.pdf)) | Cover ≠ summed crown area. Random-overlap correction `cover = 1 − e^(−Σπr²/A)` | ESTABLISHED METHOD |
| Directional (per-bearing) Sitka crown-radius data | **None located.** No published coefficients for B's p, L, β or lateral-growth limits | GAP |

Frenchpark caveats: one site, one age, 12 trees per treatment, and only trees near the mean DBH. It measures *maximum* width, so half of it is an **upper bound** on a mean projected radius; laying the crown flat may distort it further. The drain/no-drain sub-group widths differ by up to 0.7 m within a treatment. I use this as the best available Irish check, not as a calibration dataset.

---

## 5. Results (offline; same inputs, same fells for every candidate)

**Method.**
- **Engine port.** Pure Python port of `AnnualRules.Grow`.
- **Competition.** Hegyi CI is computed within each area only, with an 8 m cutoff and a 0.5 m distance floor, from pre-growth DBH. Crown potential uses the new DBH.
- **Growth held equal.** DBH growth follows the engine rule (1.2 cm × site 1.0 × size term × `1/(1+CI/5)`), identical for all candidates. Mortality and height growth are off, because crowns feed neither. In MA-A (relative density 0.80) the engine would add about 2% deaths a year, the same for every candidate. Calculations use float64, not engine-exact float32.
- **Cover and gaps.** Cover is the union of crown discs clipped to the stand polygon, on a 0.125 m grid. Gap metrics come from 4-connected components and a Euclidean distance transform; gaps touching the polygon edge are bounded by it.
- **B settings.** p = 1, L = 2 m, γ = 0.15 (A's k), no vMax, eight sectors. **β = 0.443 is fitted to Frenchpark unthinned (Table 12).** `B_matchA` (β = 0.392) is sized to match A in that benchmark, as a same-size control.

### 5.1 Observational test: Frenchpark-like stand

The test stand is a synthetic 60 × 60 m block:
- 2 × 2 m grid with ±0.25 m jitter and periodic boundaries (no edge effect).
- Reduced to 2,133 stems/ha at age 15, DBH mean 16.2 cm, SD 4.
- First thinning reproduced as one rack in seven plus low selection, down to the Table 2 stocking.
- Crowns start at each candidate's Year-0 rule before thinning, then run five years.
- DBH is forced to the measured sample-tree DBH, so only the crown rule is tested.

Values are mean radii of trees within ±1.5 cm of mean DBH (n = 142–213).

| Mean crown radius, m (radius ÷ open-grown potential) | Unthinned | Light | Medium | Heavy | Fitted |
|---|---|---|---|---|---|
| **Measured upper bound** (Table 12 max width ÷ 2) | **1.28** (0.528) | **1.45** (0.572) | **1.60** (0.607) | **1.63** (0.620) | — |
| C0 current (start at potential) | 1.44 | 1.62 | 1.73 | 1.85 | 0 |
| C1 (equilibrium, current target) | 0.82 | 1.00 | 1.10 | 1.21 | 0 |
| **A (C2)** | **1.37** | **1.54** | **1.63** | **1.73** | **0** |
| C3a (exponent 0.65, hand-chosen) | 1.18 | 1.35 | 1.45 | 1.55 | 0 |
| **B** | **1.28** (fit) | **1.51** | **1.63** | **1.75** | **1** |
| B_matchA | 1.37 | 1.59 | 1.70 | 1.82 | 1 |

**Thinning response** (each treatment's radius-to-potential ratio ÷ unthinned):

| | Light | Medium | Heavy | Mean abs. error (Table 12) | Mean abs. error (text reading) |
|---|---|---|---|---|---|
| Measured (Table 12) | +8.3% | +15.0% | +17.4% | — | (+6.4 / +12.3 / +11.0%) |
| **A** | +7.8% | +10.7% | +17.1% | **1.7 pts** | **1.8 pts** |
| C3a | +10.1% | +14.3% | +22.3% | 2.5 pts | 4.1 pts |
| B_matchA | +11.3% | +15.2% | +22.9% | 2.9 pts | — |
| **B** | +13.1% | +17.8% | +26.3% | **5.5 pts** | **6.8 pts** |
| C1 | +15.9% | +23.2% | +36.2% | 11.5 pts | 13.7 pts |

Reading:
- **A (no fitting)** sits +2–7% above the measured upper bound under both readings. A true mean projected radius is likely a little smaller, so **A probably overstates radius by roughly 5–15%**. C3a sits 5–10% below. The truth lies between them, nearer A. The data cannot separate the two with confidence.
- **C1 is 26–36% too small.** That is an empirical basis for Gemini's rejection of C1; Gemini's own reason was map cover.
- **Release.** A has the smallest thinning-response error under both readings. B overstates the response by about 20–60% (up to about 2× on the text reading), an error three to four times A's. Sized to match A's crowded crowns (B_matchA), B's response error falls to 2.9 points, but its radii run 7–12% high. With one coefficient, B can fit either the level or the response, not both.
- **Robustness.** DBH SD of 3 or 5 cm moves either candidate's radius by ≤ 0.05 m.
- **B's coefficients cannot be identified from this data.** L = 3 m (β refitted to 0.226) and 16 sectors (β 0.437) give predictions within 0.002 m of base B. Mean-radius data fixes only one combined scale, so p, L, γ, vMax and sector count remain untested.

### 5.2 Year 0 on the game stands (reproduces Candidate A's table)

| Stand | Candidate | Mean r (q10–q90) m | Σπr²/area | Union cover | Bare m² | Largest gap width m |
|---|---|---|---|---|---|---|
| MA-A | C0 | 2.63 (2.20–3.10) | 4.11 | 99.8% | 2 | 0.75 |
| | **A** | **1.73** (1.29–2.26) | **1.81** | **96.6%** | 32 | 1.58 |
| | B | 1.70 (1.15–2.30) | 1.79 | 96.2% | 35 | 1.58 |
| | C3a / C1 | 1.52 / 1.14 | 1.43 / 0.83 | 91.7% / 69.7% | 76 / 277 | 1.80 / 2.30 |
| MA-B | C0 | 3.34 | 1.75 | 92.4% | 114 | 3.69 |
| | **A** | **2.95** | **1.37** | **88.0%** | 180 | 3.81 |
| | B | 3.07 | 1.49 | 89.9% | 151 | 3.72 |
| Compact30 | C0 | 2.12 | 2.76 | 84.6% | 139 | 8.0 (H1 clearing) |
| | **A** | **1.38** | **1.20** | **70.9%** | 262 | 8.5 (H1 clearing) |
| | B | 1.34 | 1.15 | 69.4% | 275 | 8.3 |
| | C1 | 0.91 | 0.54 | 44.0% | 504 | — |

Candidate A's published Year-0 numbers reproduce to within 0.1 point of cover and 0.01 m of radius (0.2 points for C1 on Compact30: 44.0% vs 43.8%). The Crookston–Stage random-overlap estimate for A on MA-A is 83.6%, against a measured union of 96.6%. That difference is expected for regular plantation spacing, and it shows again that summed crown area is not cover.

### 5.3 Fell scenarios (identical tree IDs for every candidate; IDs listed in `results.json`)

| Scenario | Trees felled | Share of BA |
|---|---|---|
| `A-G10` | 10 nearest the MA-A centroid (MA-A-069, 084, 085, 086, 099, 100, 101, 115, 116, 117) | 6.6% |
| `A-D10` | 10 scattered: MA-A-009, 026, 043 … 162 (every 17th) | 4.5% |
| `A-T20` | One extraction rack (generator row 6), then smallest-first to ≥ 20% BA (50 trees) | 20.4% |
| `B-G10` | 10 nearest the MA-B centroid | 16.8% |
| `C30-G10` | 10 nearest (6, 6) in Compact30 | 4.9% |

Each is felled at Year 0 after initialisation.

**New bare ground caused by the fell** (m²: scenario minus the same candidate's no-work control) **and largest gap width:**

| Scenario | C0 post / Y1 / Y3 / Y10 | **A** post / Y1 / Y3 / Y10 | B (circles) | B (true sector shapes) |
|---|---|---|---|---|
| A-G10 | 14 / 18 / 25 / 26 | **38 / 36 / 34 / 29** | 39 / 37 / 33 / 26 | 40 / 37 / 30 / 18 |
| A-G10 gap width m | 3.3 / 3.8 / 4.5 / 5.5 *(grows)* | **5.5 / 5.4 / 5.3 / 4.8** | 5.5 / 5.4 / 5.2 / 4.6 | 5.5 / 5.2 / 4.8 / 3.8 |
| A-D10 | 1 / 2 / 5 / 13 | **17 / 16 / 14 / 9** | 18 / 16 / 13 / 7 | 17 / 15 / 11 / 4 |
| A-T20 | 22 / 32 / 57 / 77 *(grows)* | **122 / 115 / 104 / 74** | 122 / 111 / 94 / 59 | 124 / 109 / 86 / 40 |
| B-G10 | 130 / 131 / 132 / 123 | **146 / 144 / 139 / 125** | 142 / 140 / 136 / 124 | 144 / 142 / 137 / 121 |
| C30-G10 | 15 / 17 / 21 / 8 | **27 / 26 / 22 / 15** | 24 / 22 / 17 / 7 | 28 / 24 / 19 / 7 |

**Cover in the untouched MA-A control (no felling):** C0 goes 99.8 → 91.3% by Y10 as crowns deflate (the "backwards" behaviour, confirmed). Under A it goes 96.5 → 98.0%, and under B 96.2 → 97.8%, as crowns slowly grow with DBH.

**Gap-edge trees** (survivors within 3.5 m of a felled stem; extra radius vs the same trees in the control):

| Scenario | A Y1 / Y3 / Y10 (m) | B Y1 / Y3 / Y10 (m) | B gap-facing vs opposite sector, Y3 |
|---|---|---|---|
| A-G10 (17 trees) | +0.02 / +0.06 / +0.14 | +0.04 / +0.10 / +0.23 | +0.27 vs +0.02 m |
| A-T20 (97 trees) | +0.02 / +0.06 / +0.15 | +0.03 / +0.09 / +0.22 | +0.17 vs +0.08 m |
| C30-G10 (17 trees) | +0.02 / +0.05 / +0.13 | +0.03 / +0.08 / +0.20 | +0.19 vs +0.02 m |

Reading:
- **C0 is the cause of the owner's symptom.** Ten scattered fells give 1 m² of new bare ground, and gaps *widen* over time even with no felling.
- **On the map, A and B are nearly the same.** They stay within about 2 points of cover in every scenario and year, and their fell-caused bare ground differs by at most 15 m².
- **Most of B's directional effect is lost in the circles.** Drawn as true sector shapes, B closes gaps faster (A-T20 at Y10: 40 m² still open vs A's 74). Drawn as circles, B keeps less than half of that difference (59 m²). At Year 0 in MA-A, circles under-state B's own cover by 1.7 points (97.9% vs 96.2%).
- **B's expansion limit and "no shrinking" rule never come into play here.** Its gap-facing sectors grew about 0.27 m over three years (≈ 0.09 m/yr), so only a vMax below about 0.1 m/yr would limit them. Over 10 untouched years no tree's radius shrinks under either candidate, because with mortality off, DBH growth dominates. B's expansion-only rule would matter only in long runs with suppression.

### 5.4 Density behaviour (equal 15 cm DBH, periodic square grid)

| Spacing m (stems/ha) | 0.8 (15,625) | 1.0 (10,000) | 1.5 (4,444) | 2.0 (2,500) | 3.0 (1,111) | 4.0 (625) | 8.0 (156) |
|---|---|---|---|---|---|---|---|
| A: radius ÷ spacing | 0.66 | 0.65 | 0.63 | 0.59 | 0.52 | 0.44 | 0.25 |
| B: radius ÷ spacing | **0.13** | **0.10** | 0.39 | 0.53 | 0.54 | 0.47 | 0.26 |
| C1: radius ÷ potential | 0.06 | 0.10 | 0.20 | 0.32 | 0.55 | 0.69 | 0.91 |

Why A behaves this way:
- Hegyi CI rises in proportion to stem density, so under A crown **area** ∝ 1/CI ∝ growing space per tree, and radius follows spacing.
- Canopy closure therefore holds at any density without a hard cap, and crowns approach open-grown size as stands open.
- This is a structural argument, not an empirical one, but it is how a space-filling canopy should behave.

B's exponential form, like C1, collapses below 1.5 m spacing. At 10,000 stems/ha B's crowns are 4.9% of potential, which would show false open ground in regeneration thickets. B's proposal allows a different functional form, but that would need fitting too.

### 5.5 Stand-edge effect (affects both candidates)

76% (MA-A), 67% (MA-B) and 84% (Compact30) of trees are within 8 m of their stand boundary. The boundary is treated as open ground, and MA-A and MA-B ignore each other across their shared edge. In MA-A, Year-0 edge trees end up 11% larger than interior trees under A, 16% larger under B, and 23% larger under C1.

---

## 6. Shared-engine fit (2D and 3D)

Unity facts checked on `origin/main` `3a67d098`:

| Fact | Location |
|---|---|
| Crowns start at open-grown potential | `ForestStartingStand.cs:158,319` |
| Annual relaxation lerp | `ForestTree.cs:631–634` |
| Shade reach `CrownRadius × 1 + cellSize/2` | `ForestEcologyController.cs:841,918` |
| Seed potential `clamp01(r/4)` | `ForestEcologyController.cs:1353` |
| Canopy mesh scale `(2r, 2.2r, 2r)` | `ForestTree.cs:280` |
| Pruning multiplier `0.92^lifts` | `ForestTree.cs:185–188` |
| Stand Map draws crowns at `0.62×` | `StandMapDrawPlan.cs:174` |
| Species values as A cites (oak `0.1 + 0.1045·DBH`, Ci50 5, k 0.12; beech power law, Ci50 6.5, k 0.15) | species assets |

| Question | A | B |
|---|---|---|
| One authoritative rule for 2D and 3D | Yes: a scalar in, a scalar out, no contract change | The 2D map and the 3D mesh both take one radius. The sector state is invisible in both unless a new asymmetric-crown contract is designed |
| Saves / determinism | A version flag; same state shape; old saves stay `CrownModel 0` | 8–16 new floats per tree; schema and hash change; migration needs a reconstruction rule |
| Three species (D-059) | Reuses each species' existing `Ci50` and `k`; no new per-species numbers. These are calibration values, not broadleaf crown data | p, L, β (and γ, vMax) would be needed per species, with no data for any of them |
| Light / seed / mesh ripple | **Same for both, because both shrink Sitka crowns about 35% from current.** Seed potential falls (MA-A mean ≈ 0.66 → 0.43; Compact30 ≈ 0.53 → 0.35). Shade reach falls by about 0.9 m. The canopy mesh shrinks about 34%. The Stand Map's 0.62× would shrink crowns twice | same |

Candidate A's Compact30 light figures were **not** reproduced here. I did not run the Unity light raster.

---

## 7. Edge cases and failure modes

| Case | A | B |
|---|---|---|
| Very dense (MA-A, thickets) | Stays closed; radius ≈ 0.6 × spacing | Collapses below about 1.5 m spacing (§5.4) |
| Sparse / open | Approaches open-grown size (correct limit) | Same; slightly larger in MA-B (+4% radius) because of the exponential form |
| Stand edges / plot boundaries | Edge trees +11% (truncated CI); shared A/B edge ignored | Edge-facing sectors get no pressure, +16%; worse |
| Mixed sizes | The Hegyi size ratio handles dominance; suppression is the same on all sides | Directional, but the coefficients are untested |
| Heavy thinning | Matches the Frenchpark 5-year response | Overstates the Frenchpark response by about 20–60% |
| Small DBH | Hegyi's `max(1, DBH)` floor gives a large CI, but the square root keeps radius sensible (illustratively, a 5 cm tree under 30 cm trees ≈ 0.6 m) | exp may hit the 0.1 m floor |
| DBH > 80 cm | Outside the Tabbush regression range (both) | same |
| Closely spaced pairs | Overlap allowed (crowns interlock); no cap | Overlap allowed |
| Long runs | Relaxes both ways | Expansion only; suppressed trees can never shed crown (not exercised in 10 years) |

---

## 8. Recommendation

**Ranked:**
1. **Candidate A (C2), as written: PREFERRED.**
2. C3a: in reserve if more data show that A overstates crowns. It adds one calibrated number.
3. Candidate B: **not supported now.** It would only be worth revisiting if both (a) the 2D/3D contract gains an asymmetric crown shape, and (b) directional Sitka crown measurements are obtained.

No hybrid is needed. A already contains the change that matters most, Year-0 equilibrium, which B also contains. B's real contribution is its comparison rules: identical inputs, union rather than sum, and release measured against a control. Those should carry into the spec's acceptance tests.

**Two checks to settle in the SPEC before any code is written:**
1. **Boundary competition policy.** State whether MA-A, MA-B and Compact30 are islands surrounded by open ground (current behaviour) or embedded in forest (a buffer or assumed surround). Also state whether MA-A and MA-B compete across their shared edge. Most trees are within 8 m of a boundary, so this choice sets Year-0 crown sizes about as much as the choice of model does. The decision is the same for either candidate.
2. **Unity consumer re-pin, before Unity adoption** (lab first, as A proposes). For each reader of `CrownRadius` (light reach, seed potential `r/4`, canopy mesh, Stand Map 0.62×), decide whether it should follow the realised crown. Then run one Compact30 before/after on seed potential and gap light. Removing the 0.62× map factor under `CrownModel 1` is likely, so crowns are not shrunk twice.

Record this as a known bias, not a blocker: against Frenchpark, A probably overstates Sitka crown radius by about 5–15% (§5.1). Do not tune it on one site.

**Model recommendation vs verified accuracy.** This report recommends a model. It does not show that any implementation is ecologically accurate. Nothing was run in Unity or in the lab engine itself. Oak and beech have no stand-grown crown benchmark. Mortality, height growth and regeneration were held fixed. Enlarged80 was not tested. Irish transfer rests on one site.

---

## 9. Not verified / limits of this review

- The analysis is an offline float64 port, not the C# engine; there is no bit-parity claim.
- The Frenchpark stand geometry, DBH spread and thinning selection are synthetic reconstructions. The field data are maximum-width means of 12 mean-DBH trees per treatment, and the thesis reports two inconsistent sample-DBH readings (both tested).
- B was evaluated with p = 1, L = 2 m, an exponential form and γ = 0.15. Other unfitted choices could behave differently, but none can be identified from the available data.
- My fell selections differ from Candidate A's (for example, my 20% BA thinning removes 50 trees vs A's 37), so the scenario numbers are not one-to-one with A's table. The Year-0 table is.
- No game code, fixture, SVG, Unity asset, save or parameter was changed. The only repository write is this docs-only branch.

**Owner action:** choose (G5) Candidate A, C3a, B, or neither. After that, the Manager can issue the SPEC REQUIRED packet with the two checks above.
