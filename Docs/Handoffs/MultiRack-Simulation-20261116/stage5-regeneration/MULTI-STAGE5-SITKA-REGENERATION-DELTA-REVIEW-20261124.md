# MULTI Stage 5 — narrow delta re-review (post-Option-A, final bounded repair)

**Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), independent of the implementer (Claude Opus 5.5). I did not write or edit any of the reviewed code. This file is the only thing I publish.
**Scope:** `a3d86aa2..5b757d9b` (7 files). The full review `77b29925` (F1–F11) is background and was not re-derived.
**Method:** I ran the build and suite myself, recomputed the areas independently, and ran four throw-away mutations on copies under `/tmp`. The candidate lane was never modified.

```
INTEGRATION DECISION
Status: APPROVED
        (delta only. §10 lists two owner-acceptance conditions; they are decisions for the owner, not code changes.)
Gate: G2 (acceptance): PASS
        S5-07A judgement: PASS on D1 exactly as written (matched population, per-hectare whole-cell units,
        pooled and mean, both metrics, both scenarios, all inside [0.75, 1.33]; zero out-of-polygon trees;
        5.0 m bit-identical). The pass certifies the matched population IN AGGREGATE. It does NOT establish
        that the grids agree on matched edge blocks (24 of 101 blocks), where the per-hectare promotion ratio
        is 1.90 / 1.84 in every one of 40 seed-runs. See §4.
        S5-02: PASS under D2 (comparative suppression).
Gate: G3 (measurement integrity): PASS
        item 1 population ........ legitimate
        item 2 units ............. legitimate (caveat R2: different denominators, factor 1.1129)
        item 3 new assertions .... legitimate, non-vacuous
        item 4 light-count ....... legitimate, strictly stronger
        item 5 S5-07B disclosure . legitimate (nits R5, R7)
        item 6 S5-02 printout .... legitimate (owner D2)
        item 7 S5-02 definitions . legitimate (caveat R3: provenance of the margins)
        item 8 named constant .... legitimate
        item 9 tolerances ........ legitimate
Reviewed branch: feat/multi-stage5-sitka-regeneration-20261123-v1
Reviewed HEAD: 5b757d9b4a353d07557db95d3af9227c8d841874
Spec: specs/multi-stage5-sitka-regeneration-v1 @ 46c08e1f (APPROVED, amended)
Spec conformance: PASS
Reasons: R1–R8 below (none blocks the delta; R1 and R3 need the owner)
Integration target: none — no merge authority
```

---

## 1. What I ran (all in WSL, never Windows `git.exe`)

| Check | Result |
|---|---|
| Lane HEAD, `git status --short` | `5b757d9b4a353d07557db95d3af9227c8d841874`, clean |
| `git diff a3d86aa2..5b757d9b -- Tools/MultiRackSitkaP1Lab/src` | **0 lines** |
| Files changed in the delta | 4 spec files, `Stage5GridStudy.cs`, `Stage5Tests.cs`, the repair handoff. Nothing else. No `Stage5Support.cs`, `Model0FingerprintTests.cs`, `.p0ref`, `Assets/`, `ProjectSettings/` |
| Clean rebuild, `--no-incremental` (the first build I ran was an incremental no-op on a DLL built after the handoff commit, so I forced a clean one) | **0 warnings, 0 errors** |
| Suite, run twice | **209 / 209 passed, 0 failed**, exit 0, both runs |
| Run-to-run difference | Only `PS-01` / `MA-W10` race counts and `MC runtime` wall-clock lines. Every Stage 5 line is identical across the two runs |
| Spec files on the feature branch vs docs commit `46c08e1f` | All four sha256 pairs identical |
| `.p0ref/SitkaLab.Core.dll` sha256 | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (expected `9a99f781…`) |

Commands: `mkdir -p /tmp/stage5-delta`, then `TMPDIR=/tmp/stage5-delta dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers [--no-incremental]` and `TMPDIR=/tmp/stage5-delta dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll`.

## 2. Question 4 — is the change test-only? Yes.

| Anchor | Expected | Observed in my run |
|---|---|---|
| Single-area Model 0 | `7e3a2a86…` | `7e3a2a869ec16624…` ✓ |
| Multi-area Model 0 | `a692a6d5…` | `a692a6d5bc374dd0…` ✓ |
| Stage 3 C2 `0/0/1` | `814e8165…` | `814e81650b624285…` ✓ |
| Experiment | `c66d566c…` | `c66d566c9b930ea1…` ✓ |
| P0 pin | `9a99f781…` | sha256 above ✓ |
| `1/1/1/0` full run | `8a926776…` | `8a926776ffa3ca3d…` ✓ (asserted, `Stage5Tests.cs:13`, `:44`) |
| 5.0 m purity | `c67edc6b…` | `c67edc6b7a8b1633…` ✓ (asserted, `Stage5GridStudy.cs:152`) |
| **`1/1/1/1` full run** | `c4b670ed…` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` ✓. This is the same 64-hex string the previous handoff recorded at `a3d86aa2` |

The `1/1/1/1` hash is **printed, not pinned** as a test constant (suite line "S5 1/1/1/1 full-run fingerprint"). That is how the suite has always worked, not something this delta introduced. Its stability here rests on an empty `src` diff, an untouched `Stage5Support.cs`, and my observed value matching the value recorded one commit earlier. The `Stage5Tests.cs` diff has exactly two hunks (`S5-02` and the new `S5-08 P-08`), so no other test moved.

## 3. Question 1 — is the area normalisation internally consistent? No, and it does not change the pass.

**The gate's denominators are different ground.** `Stage5GridStudy.cs:340` (`AreaM2`) multiplies each lattice's own in-polygon cell count by its cell area:

- 2.5 m: 363 cells × 6.25 m² = **2268.75 m²**
- 5.0 m: 101 cells × 25 m² = **2525.00 m²**

Both lattices cover the *same 101 aligned blocks*. A 5.0 m cell is charged a full 25 m² even where much of the block lies outside the polygon. A 2.5 m cell is charged only if its centre is inside.

**I computed the ground independently.** I clipped the MA-1 polygons (A: `(2,2) (36,2) (40,18) (24,32) (3,26)`; B: `(36,2) (90,0) (86,44) (40,18)`, from `MultiAreaGenerator.cs:40` and `:45`) against every block. My code reproduces the test's own printed figures to the cent: 383 / 101 cells, 113 / 101 blocks, 101 matched + 12 unmatched, 77 interior + 24 edge, **2219.88**, **2270.06** and **99.37 m²** clipped, and the 2268.75 / 2525 whole-cell areas.

| Basis for the matched 101 blocks | 2.5 m area | 5.0 m area | Factor on the raw count ratio |
|---|---|---|---|
| **Gate (whole cells)** | 2268.75 m² | 2525.00 m² | **× 1.1129** |
| Each lattice's own clipped cells (the handoff's sensitivity) | 2219.88 m² | 2270.06 m² | × 1.0226 |
| **Same actual ground, both lattices** `(A ∪ B) ∩ matched blocks` | **2294.06 m²** | **2294.06 m²** | **× 1.0000** |

Any common denominator cancels, so the same-ground ratio *is* the raw matched-count ratio. The 5.0 m clipped figure (2270.06) is itself 24.0 m² short of the true ground, because in the four rack-straddling blocks `(7,0) (7,1) (7,2) (7,3)` a 5.0 m cell is owned by one rack and clipped by that rack's polygon alone. The handoff's clipped-basis sensitivity is therefore not a same-ground basis either.

**Result under each basis (pooled; the means agree to ±0.002). All four metrics pass under all three bases.**

| | Gate (whole cells) | Own clipped | **Same true ground** | Limit |
|---|---|---|---|---|
| Thinning, accepted | 0.956 | 0.879 | **0.859** | ≥ 0.75 |
| Thinning, promotions | 1.247 | 1.146 | **1.120** | ≤ 1.33 |
| No work, accepted | 0.952 | 0.875 | **0.855** | ≥ 0.75 |
| No work, promotions | 1.181 | 1.085 | **1.061** | ≤ 1.33 |

Per-seed on the same-ground basis (gate per-seed range ÷ 1.1129): thinning accepted 0.857–0.861, thinning promotions 1.045–1.217, no-work accepted 0.853–0.858, no-work promotions 0.975–1.131. Every one of the 80 per-seed values is inside `[0.75, 1.33]`. On the gate's own basis two thinning seeds are above 1.33 (1.339, 1.355); on the same-ground basis none is.

**Direction of the bias.** The gate's per-hectare ratio equals the same-ground ratio × 1.1129. That is:

- **adverse** to the promotion pass: headroom to 1.33 is 0.083 / 0.149 on the gate basis against 0.210 / 0.269 on the same-ground basis;
- **favourable** to the abundance pass: headroom to 0.75 is 0.206 / 0.202 against 0.109 / 0.105.

The bias is mechanical, it is the owner's literal unit (spec §9 Q4: `A_2.5 = Σ n_block × 6.25`, `A_5.0 = blocks × 25`), and the pass does not depend on it. I record it as **R2**, a labelling issue, not a failure.

## 4. Question 2 — does the pass depend on dilution? My plain answer.

**Is the pass a faithful implementation of D1? Yes.** D1 fixes the population (the 101 blocks represented on both lattices, which *includes* the 24 matched edge blocks), the unit, and the aggregate gate. The worker implemented exactly that and disclosed the edge split in three places: the handoff's opening box, the `S5-07A` SPLIT lines, and `S5-07B`. The phrase "whole stand (interior and edge blocks together)" is Option A's wording; D1 supersedes it only by removing the 12 *unmatched* blocks, not the matched edge. Nothing was excluded to make the number pass.

**Does it conceal a real edge-block scaling defect? The data is not concealed. The gate cannot see the defect by construction, and the defect is real.**

| Matched edge blocks (24), shared field, pooled | Gate unit (whole cells) | Own clipped | Same true ground |
|---|---|---|---|
| Thinning, accepted | 0.991 | **0.676** | **0.568** |
| Thinning, promotions | **1.902** | 1.297 | 1.090 |
| No work, accepted | 1.036 | **0.707** | **0.594** |
| No work, promotions | **1.841** | 1.256 | 1.055 |

(**bold** = outside `[0.75, 1.33]`.)

Four facts, each checked rather than assumed:

1. **It is deterministic and not seed noise.** The edge per-hectare promotion ratio is 1.745–2.122 (thinning) and 1.683–2.025 (no work). All 40 per-seed values are above 1.33.
2. **No area basis makes the edge clean on both metrics.** The whole-cell unit makes edge abundance look exact (0.99–1.04) and edge promotions 1.9×. Both ground-based units make promotions look fine (1.05–1.30) and push edge abundance to 0.57–0.71, below 0.75. The disagreement moves between metrics; it does not go away.
3. **The mechanism is in the owner-approved Option A rule.** In `Stage5Annual.Promote` (lines ~236–247) a winner exports the same cohort's bands from the block's *in-polygon* cells only, but always creates one whole tree. The A2 credit would make the budget area-proportional (`n/4` per year), but it almost never binds. In thinning, realised promotions are 3 % (2.5 m interior) to 13 % (2.5 m edge) of the credit cap. So an `n = 2` block yields as many trees per cohort as an `n = 4` block while representing half the ground. Arithmetic check: mean in-polygon cells per matched edge block = 55 / 24 = 2.29, so `4/n = 1.745`, which equals the whole-cell area ratio `600 / 343.75`. Times the raw edge promotion ratio 1.090 gives **1.902**, the printed value to three places.
4. **Trees per block agree; trees per nominal hectare do not.** Raw edge promotions are 1106 / 1015 (1.09) and 561 / 532 (1.05). The 5.0 m reference charges an edge block 25 m² although its mean clipped polygon area is 16.3 m² (24 edge blocks: 600 m² nominal against 395 m² true ground, × 1.52). The 5.0 m grid, frozen at `a = 1.0` for Unity parity, over-credits its boundary ground. The 2.5 m grid, whose edge credit follows its own in-polygon cell count, does not.

**Why it passes anyway: area weighting.** Edge blocks are 15.2 % (2.5 m) and 23.8 % (5.0 m) of the gated whole-cell area, but carry **44–45 % of matched promotions in thinning and 69 % in no work**. The interior, at 1.146 / 1.075, dominates the per-hectare figure. The whole-population pass is 6.6 % below the limit for thinning promotions (1.247 against 1.33), and 2 of 20 thinning seeds are individually above it.

**My judgement for the owner, stated plainly:**

- Passing the whole-population gate while the edge sub-population sits at about 1.9× is **faithful to D1 and is not manufactured by dilution.** The interior alone passes, the gate's own area factor works *against* the promotion result, and on a same-ground basis the edge promotions agree to 5–9 %.
- It is **not** evidence that the 2.5 m and 5.0 m grids agree on partially covered blocks. It cannot be, because the gate is an aggregate. A real, repeatable, basis-independent edge disagreement exists and is the price of the owner-approved Option A rule on a polygon boundary. Any sentence such as "the two grids are consistent" that goes beyond "the matched population is consistent in aggregate, per nominal hectare" is unsupported by this test.
- I do **not** treat it as grounds to fail `S5-07A`. D1 fixed the population, the unit and the stop rule ("do not adjust the rule, the tolerance, the population or the control"), and none of them was breached. Whether to accept the residual or add an edge sub-population gate is an owner decision (§10, condition 1). I have not made that choice for them.

## 5. Question 3 — is `S5-07B` non-blocking and disclosing? Yes, with two nits.

**Non-blocking.** Lines 158–201 (the whole body) and the print helpers (`Stage5GridStudy.cs:499–600`) contain **0** `T.True` / `T.Eq` / `T.Refuses` / `throw`. I grepped both ranges. Under suite order it reuses the memoised `ModelRun` and `SharedRun` results that `S5-07A` produced, so nothing can fail.

- Nit R7: run on its own, `SharedResult` falls through to `SharedRun`, which contains an invariant `throw` (line ~256) and the `Harness` precondition `T.True`. Those are infrastructure invariants rather than thresholds, but "cannot fail the suite" is strictly true only in suite order. The runner (`Runner.cs:26–36`) is sequential in registration order and fails a test on any thrown exception.

**Disclosing.** Verified against the run output:

| Required disclosure | Printed? |
|---|---|
| Unmatched blocks: count 12, 20 cells, **125.00 m²** (clipped 99.37), promotions and accepted abundance, **both light bases** | Yes, four `UNMATCHED` lines. Model light: 392 / 179 promotions (14.1–15.5 % of the 2.5 m whole stand), accepted 321.0 / 240.2. Shared field: 1023 / 731 promotions (28.9 % / 47.2 %), accepted 532.6 / 444.7. All equal the handoff's table |
| Interior and edge splits, both light bases | Yes (`SPLIT` lines for model light and shared field) |
| Per-seed min–median–max | Yes (`SUMMARY`, `AreaReport` and `SPLIT` lines; per-seed 17–19.5–22 for the unmatched promotions) |
| Non-PAR statement and finer-sampling statement | Yes (`Stage5GridStudy.cs:197–200`) |
| Boundary-ground statement | Yes (`:192–196`); accurate |
| Under model light, matched promotions per hectare | **1.555 / 2.030** pooled (means 1.556 / 2.051), 20 / 20 seeds outside `[0.75, 1.33]`. Accurate |

Nit R5: the printed light note says a higher 2.5 m real-light recruitment "reflects that finer gap-resolution sampling, not a scaling defect". The spec requires this statement. The shared-light control does support the weaker claim that the *scaling rule* is not the main cause, because sharing the light takes the no-work interior promotions from 3.418 to 1.075. But the test cannot separate sampling position from each run's own promoted-adult canopy, and the handoff says so ("Not separated", §4.3) while the printed text does not.

## 6. Question 5 — `S5-02` honesty

**Margins are derived from spec constants, not from the data.** I checked each against the code:

| Margin | Derivation | Evidence |
|---|---|---|
| 0.315 m | `0.35 × 0.90 × 1.0` | `RegenHeightGrowthMPerYear = 0.35f` (`Stage5Models.cs:55`); `LightResponseFactor` maximum 0.9 (`:65`); `SiteProductivity = 1f` (`:114`); growth rule `Stage5Juvenile.cs:60` |
| 0.8 | `1 − 0.2` | `RegenMortalityUnderPoorLight = 0.2f` (`:62`); `LightSurvival` (`Stage5Juvenile.cs:70`) |
| Year 12 | `0.15 + 11 × 0.315 = 3.615 ≥ 3.5`; year 11 gives 3.30 | `RegenInitialHeightM = 0.15f` (`:56`), `PromotionHeightM = 3.5f` (`:57`) |

They are also far from the observed values, which a reverse-fit would not be:

| | Observed | Margin | Headroom |
|---|---|---|---|
| Year 12 gap lead | 1.509 m | ≥ 0.315 m | 4.8× |
| Year 15 gap lead | 1.307 m | ≥ 0.315 m | 4.1× |
| Year 12 dense / gap abundance | 0.349 | ≤ 0.8 | 2.3× |
| Year 15 dense / gap abundance | 0.446 | ≤ 0.8 | 1.8× |

**Prior knowledge (R3).** The worker disclosed in the handoff (§2) that it had read the previous handoff's 3.24 m gap height and 7 / 12 census before choosing margins, and that none was changed after the run. The spec commit (`6bac1e44`, 23:50:24) precedes the test files (23:55, 23:56) and the test commit (23:58), which is consistent with that. But the spec carries these margins as part of a **"binding" owner amendment record** and says they "are fixed here before this pass measured anything", with no prior-knowledge disclosure. The handoff says "two choices are mine". I could not see the owner's own words (§11). The owner should confirm 0.315 / 0.8 / years 12 and 15 as theirs.

**Census is printed, never asserted.** `rows[...].Poor` is read only inside `Console.WriteLine` calls (`Stage5Tests.cs:148–150`); no `T.*` call uses it. The run shows "12/12 … 7/12 … 6/12" with the year-12 line "7/12".

**Would it catch a real suppression failure? Two mutations on copies:**

| Mutation (copy only) | `S5-02` | What it shows |
|---|---|---|
| `RegenMortalityUnderPoorLight` 0.2 → 0 (no poor-light penalty) | **still PASSES** (ratio 0.366 / 0.474) | The test is not a test of that rule; the "0.8" margin is a scale, not a mechanism check. The rule is covered elsewhere |
| Growth response floored at 0.6 in the dark (shade-tolerance bug) | **FAILS**: "the dense interior never reaches promotion height (3.5 m) nor promotes in 15 years". Year 12 gap lead 0.446 m (still above 0.315), abundance ratio 1.000 | It is the promotion-height and abundance criteria that catch it, not the lead margin |

Verdict: it reliably catches a gross failure (no light difference; an interior that grows or fills like the gap), and by design tolerates a moderate one. That is what "materially" with spec-derived margins means.

## 7. Question 6 — the straddling-block test (`S5-08 P-08`, `Stage5Tests.cs:712–795`)

I **derived the expected values by hand** rather than trusting the recorded output. Block `(1,0)` has `n = 4`, so credit 4 per year and one promotion per year. Block `(1,1)` has `n = 2`, so credit 2 per year, and it promotes every second year. Winners are the oldest cohort, then the tallest. That gives:

`17: A(2,0) c3 (+ B c3 exported)` · `18: B(3,1) c5 (+ A c5)` and `B(3,2) c3 (+ A(2,2) c3)` · `19: B(3,0) c7` · `20: A(2,2) c5` · `22: B(3,2) c7`

This equals the asserted string exactly. The withheld counts also derive: block `(1,1)` is withheld in years 17, 19 and 21, booked to the would-be winner's rack, so MA-A gets 1 and MA-B gets 2, matching `T.Eq((1, 2), …)`. Nine bands become six trees plus three exported bands. Final credits `1,0,4,4;1,1,2,4`.

It asserts: genesis credit `0,0,4;0,1,2;1,0,4;1,1,2;2,0,4;2,1,2` (so a straddling block counts both racks), the 5.0 m lattice has no cell in block column 1, the promotion schedule, cohort export across the edge with per-rack booking, `cohortBands == 3`, no bands left, the per-rack abundance identity (`≤ 1e-6`) and booked = recorded (`≤ 1e-9`).

**Mutation check on copies:**

| Mutation | `P-08` |
|---|---|
| A straddling block counts at most 2 in-polygon cells (what per-rack counting gives) | **FAILS** |
| A winner exports only its own rack's cohort bands (no cross-rack export) | **FAILS** |

Both mutations also break other tests, so the evidence is that `P-08` is sensitive to both accounting errors. It does not show `P-08` alone isolates them.

## 8. Question 7 — G3 audit of the nine declared changes

| # | Change | Verdict | Evidence |
|---|---|---|---|
| 1 | `S5-07A` population: whole stand → matched blocks | **Legitimate.** Owner D1, written in the spec before the test run. Whole-stand figures still printed. Superseded wording kept in the spec | `spec.md` Q4; `Stage5GridStudy.cs:135–138` |
| 2 | Units: raw totals → per hectare, whole cells | **Legitimate.** Literal D1 text. Caveat R2: different denominators give a 1.1129 factor and the "raw-count … not area-normalised" label describes the same-ground figure | `:340`, `:529–548`, `:544` |
| 3 | New assertions: 1:1, interior ⊂ matched, areas > 0, 5.0 m has no unmatched ground | **Legitimate and non-vacuous.** The matched set is computed from polygon-centre tests independently of the 5.0 m lattice, then set-compared with it. I reproduced 101 / 12 independently | `:376–389`, `:127–128` |
| 4 | Light-identity count: `≥ 30 × 77` → `= 30 × 101` | **Legitimate; strictly stronger.** 20 × 2 × 30 × 101 = 121,200, which matches the printed count. It reads back the light the cell rules actually used (`cell.Light`) | `:125–126`, `:249–257` |
| 5 | `S5-07B` prints shared-field splits and the unmatched disclosure | **Legitimate.** No assertion in the body; nits R5, R7 | `:158–201`, `:587–599` |
| 6 | `S5-02` absolute poor-light assertion removed → diagnostic | **Legitimate.** Owner D2; the proxy was never an owner criterion | `Stage5Tests.cs:148–150` |
| 7 | `S5-02` comparative criteria and definitions ("tallest reached" counts a promoted cohort; abundance excludes exports) | **Legitimate; stronger than what it replaced.** The earlier strict year-12 `gap > dense` checks are implied. Caveat R3 on provenance | `Stage5Tests.cs:111–113`, `:158–173` |
| 8 | `3.5f` literal → `Stage5Constants.PromotionHeightM` | **Legitimate.** Value is 3.5f (`Stage5Models.cs:57`) | `Stage5Tests.cs:135` |
| 9 | New tolerances `1e-6` / `1e-9` in `P-08` | **Legitimate.** Smallest quantity in play is a 0.1 band, 10⁵ times above 1e-6, so any mis-booked band is caught. Not fitted to a residual | `Stage5Tests.cs:792–793` |

No other assertion was removed. In `Stage5GridStudy.cs` the removed lines containing `T.*`, `throw`, thresholds or seeds are exactly: the old `coincident >= …` check (replaced by the exact count), the old `Gate(...)` calls and function (replaced by `AreaReport` with the same `[0.75, 1.33]`, 20 seeds, 30 years), and the old exception text.

## 9. Findings

| # | Severity | Finding | Evidence |
|---|---|---|---|
| **R1** | **Medium, owner decision** | The aggregate `S5-07A` pass does not establish edge-block consistency. Matched edge blocks sit at 1.90 / 1.84 per nominal hectare in all 40 seed-runs. No area basis cleans both edge metrics (§4). Real, deterministic, a consequence of Option A's cohort-export and non-binding credit on partial blocks. Disclosed, not concealed | `Stage5GridStudy.cs:143`, `:562–584`; `Stage5Annual.cs:236–247` |
| **R2** | Low, labelling | The gate's per-hectare ratio is the same-ground ratio × 1.1129. `AreaReport` labels the same-ground figure "raw-count ratio … (not area-normalised, not gated)". The handoff's clipped basis (2270.06 m²) is 24.0 m² short of true ground (2294.06 m²) because of the four straddling blocks. The pass holds on all three bases | `Stage5GridStudy.cs:340`, `:544`; §3 |
| **R3** | Low–medium, provenance | `S5-02` margins were chosen by the worker after reading the prior handoff's 3.24 m and 7 / 12. The spec presents them inside a "binding" owner record and says they were fixed before any measurement, without the disclosure. Margins are verifiably spec-derived and far from the data, and none changed after the run. The owner should confirm them | `spec.md` second amendment record; handoff §2 |
| R4 | Low | `S5-02` does not exercise the poor-light survival rule (mutation passes) and tolerates a moderate suppression regression. It does catch a gross one (§6) | `Stage5Tests.cs:158–173` |
| R5 | Nit | The printed light note attributes the model-light excess to "finer gap-resolution sampling". The test cannot separate it from each run's own canopy, and the printed text lacks the "not separated" caveat the handoff has | `Stage5GridStudy.cs:197–200` |
| R6 | Nit | The class summary says every number is printed before any assertion. True for the tolerance gate (deferred to `:154–155`), not for the early integrity assertions at `:124–128` | `Stage5GridStudy.cs:34` |
| R7 | Nit | `S5-07B` cannot fail in suite order, but run alone it re-runs `SharedRun`, whose invariant `throw` and `Harness` precondition can fail it | `Stage5GridStudy.cs:236–263`, `:417` |
| R8 | Informational, pre-existing | The `1/1/1/1` hash is printed, not pinned. The `S5-07A` year-loop equals-`Advance` proof covers 2 of 80 runs. Neither was introduced here | suite output; `Stage5GridStudy.cs:84–90` |

There is **no** silently changed criterion, no weakened earlier pass, and no unreachable guard in the delta (Question 8). I looked for each:

- every removed assertion is accounted for in §8;
- the new `T.True(a50.PromotionsBy[Unmatched] == 0 …)` is reachable (it would fail if the 5.0 m lattice had a cell in an unmatched block);
- the `Ratio(0, 0) → 1.0` convention is old and never triggered here (every seed has ≥ 37 matched promotions).

## 10. Owner-acceptance conditions (decisions, not repairs)

1. **Accept or tighten the edge residual (R1).** Either record that `S5-07A` certifies the matched population in aggregate and that matched edge blocks are not edge-consistent under the Option A rule (the honest reading of D1 as written), or decide separately whether an edge sub-population gate or a same-ground unit is wanted. I have not made that decision, and the stop rule forbids the worker from making it.
2. **Confirm the `S5-02` margins (R3):** 0.315 m, 0.8, years 12 and 15 as the owner's, or accept them as the worker's definitions within D2.

Optional test-only wording clean-ups, none required: relabel "raw-count ratio … not area-normalised" at `Stage5GridStudy.cs:544`; add the "not separated" caveat at `:197–200`; correct the comment at `:34`.

## 11. Per-criterion table

| ID | Verdict | Basis |
|---|---|---|
| **S5-01** Frozen paths | **PASS** | Clean rebuild 0 / 0; 209 / 209 twice; all anchors in §2 match; `src` diff empty |
| **S5-02** Dark interior vs gap | **PASS** (D2, comparative) | All criteria hold with headroom (§6); census printed, never asserted; R3, R4 |
| **S5-03** Bramble / bracken | PASS | Not touched by the delta; suite green; not re-reviewed |
| **S5-04** `ClearUndergrowth` | PASS | Same |
| **S5-05** Browsing | PASS | Same |
| **S5-06** Cross-rack seed rain | PASS | Same |
| **S5-07A** Grid consistency (blocking) | **PASS on D1 as written** | Matched population, per-hectare whole-cell unit; pooled and mean, 8 checks, all inside; 121,200 identical-light comparisons; 0 out-of-polygon trees; purity pin. R1 (owner), R2 |
| **S5-07B** Grid diagnostic (non-blocking) | **PASS** | No assertion; all required disclosures printed and accurate; R5, R7 |
| **S5-08** Promotion | **PASS** | Unchanged rule; new straddling test hand-derived and mutation-checked (§7) |
| **S5-09** Adult-empty rack | PASS | Not touched by the delta; suite green |
| **S5-10** Determinism | PASS | Two independent runs identical on every Stage 5 line; the only differences are race-test counts and wall-clock |

Rows marked "not re-reviewed" rest on: an empty `src` diff, a `Stage5Tests.cs` diff limited to `S5-02` and `P-08`, a green suite, and the earlier full review.

## 12. What I did not verify

- **Anything in Unity,** `Assets/`, the `/multi` UI, a browser, a save, or a protected listener. Out of scope and not touched.
- **The owner's own words for D1 and D2.** I saw them only as rendered in the spec and the handoff, so I cannot say whether the owner specified 0.315 and 0.8 or the worker did.
- **Production-code correctness.** I confirmed it is unchanged. I did not re-review it, and I took F1–F11 of the earlier review as background.
- **S5-03 / 04 / 05 / 06 / 09 / 10 in depth,** beyond "untouched and green".
- **The edge mechanism by experiment.** It is derived from reading `Promote` plus arithmetic (4/n × raw ratio reproduces 1.902 to three places). I did not run an isolating experiment.
- **My geometry beyond agreement with the engine's printed areas.** It matches to 0.01 m² on six figures; I did not cross-check it with a second library. The polygon vertices come from `MultiAreaGenerator.cs`.
- **Mutation coverage.** Four single-line mutations on `/tmp` copies (two for `P-08`, two for `S5-02`). The first run of the two `P-08` mutations aborted on the harness rule that `TMPDIR` must be outside the repository, so I reran them with a separate `TMPDIR`; the reported results are from the reruns. They show sensitivity to specific errors, not completeness.
- **Cross-process, cross-OS or CI determinism.** Two runs on this host only.
- **The earlier `S5-07A` proof that the harness loop equals `Advance`** beyond the two runs it covers.
- **GitHub rendering of this file,** and the docs branch beyond byte-equality of its four spec files with the feature branch.

PR none, reviewed head 5b757d9b4a353d07557db95d3af9227c8d841874, delta review verdict APPROVED, integration none
