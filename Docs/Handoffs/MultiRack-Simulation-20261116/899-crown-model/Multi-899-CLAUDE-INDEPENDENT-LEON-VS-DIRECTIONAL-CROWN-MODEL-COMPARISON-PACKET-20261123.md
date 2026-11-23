# CCF Multi #899 — Claude independent comparison: Leon's crown proposal vs GPT-6 directional crown model

**Owner instruction, 2026-11-23 00:06 UTC:** “I want Claude to compare your solution with Leon's solution. So create a handoff for Claude to understand what the issue is and what your proposed solution is.”

**Packet status: HANDOFF / READ-ONLY SCIENTIFIC COMPARISON AUTHORISED — NO BIOLOGICAL ALGORITHM APPROVED, NO CODE CHANGES.** This is a comparison assignment, NOT a dispatch to implement or change the map. Difficulty **HIGH** (quantitative forestry, scientific model validation). Provider **Claude** in a new independent comparison session; no production author, Unity user, or D-091 source lease inferred. The latest owner instruction controls: **do not change the existing SVG/map design.** Do not author an implementation until an explicit owner G5 choice.

## A. The issue Claude must decide

The synthetic two-area Sitka spruce **`/multi`** simulation shows a virtually solid crown canopy, reportedly exposing almost no ground after more than ten real Fell trees and three ecological years. Owner believes the **model's crown-size/growth calculation**, rather than SVG rendering, is insufficiently realistic. The desired outcome is the most scientifically defensible *realised crown width and post-thinning expansion* the data can support, not a visually attractive empty patch.

- Reported **Area A ~171 trees / ~915 m²** and sampled average crown radius ~2.9 m are **preliminary figures**, not independently measured by this packet. The sum of disk areas may be ~5× plot area, but the **clipped union** of overlapping crowns, not the sum, represents projected canopy coverage. Neither measures actual light penetration.
- Exact published code baseline: [`AnnualRules.cs`](https://github.com/jermur788/CCF/blob/a692e1bf5d21304d06e696db1f47d1988ff80ee1/Tools/MultiRackSitkaP1Lab/src/Core/AnnualRules.cs) (GitHub blob `dc299b3271c9dd406f4f580793cd8212b94ac718`): Hegyi-type neighbour index within 8 m; scalar crown response `1/(1+CI/5)`; annual `targetRadius = R_potential × response`; `newRadius = oldRadius + 0.15×(targetRadius-oldRadius)`, floor 0.1m. These are implementation/tuning facts, **not verified empirical crown equations**.
- [`CcfSourcePins.cs`](https://github.com/jermur788/CCF/blob/a692e1bf5d21304d06e696db1f47d1988ff80ee1/Tools/MultiRackSitkaP1Lab/src/Core/CcfSourcePins.cs) (blob `205d975c4680e5b29ab886afad9b40b9956a4981`): `R_potential(m)=0.9415+0.07635×DBH(cm)` from an **open-grown Sitka potential crown** relationship, not crowded stand crown observations.
- [`DerivedManifest.cs`](https://github.com/jermur788/CCF/blob/a692e1bf5d21304d06e696db1f47d1988ff80ee1/Tools/MultiRackSitkaP1Lab/src/Core/DerivedManifest.cs) (blob `b8d8bd7a6eedf8a6f013eea209cc7d3ebd2471d3`) explicitly initialises the **P1 30 m synthetic derived fixture** at zero-competition potential radius `potential-crown-at-ci0-linear-f32-v1`; this is **not proof** that the particular two-area MA-A and MA-B use an identical initialisation route. Trace both before assuming it. Its frozen fixture and existing determinism must remain intact.
- The map renders server-provided `crownRadiusM`, not an independent forest growth model. The owner has explicitly ruled out treating crown overlap as an SVG visual bug or shrinking the tree drawing to manufacture ground openings.

**Distinguish:** potential vs realised crown, crown growth vs diameter growth, crown width vs height/crown length, crown union vs light, overlapping crown shapes vs projected equivalent disks, stand density vs actual coordinates, empirically observed vs simulation tuning.

## B. GPT-6 Candidate B — independent directional competition + gradual release

**FULL PROPOSAL (READ EXACT BEFORE COMPARING):**

https://github.com/jermur788/CCF/blob/docs/multi-899-independent-sector-crown-alternative-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/899-crown-model/Multi-899-INDEPENDENT-DIRECTIONAL-CROWN-ALTERNATIVE-CODE-FREE-COMPARISON-20261122.md

Readback-verified original: blob `4f93cafa354177432dd014aee477e5dbf6310c4a`, branch head `4669a9ffdc2fe5bf119f705c19f5ac0456e0721d`. **Candidate B is a proposal only: no fitted coefficients, offline predictions, or game source modifications.**

**Core idea:** Divide each tree's potential crown into **eight directional sectors** (N, NE, E, SE, S, SW, W, NW), estimating neighbour pressure separately per bearing based on real stem positions, distance and DBH. Unlike one Hegyi-derived scalar multiplier, growth can be more constrained toward close neighbours and less constrained toward a felling gap. Eight is a suggested computational resolution, NOT field-derived biological fact; compare 12–16 sectors offline for sensitivity.

Proposed mathematical skeleton, **unfitted**:

1. Open-grown potential ceiling `Rpot_i = 0.9415 + 0.07635×DBH_i(cm)` in metres, subject to source applicability and age/size.
2. Directional competition pressure:
   `C[i,k] = Σ_j w(k, bearing_ij) × (DBH_j / DBH_i)^p × exp(-distance_ij/L)`,
   for living neighbours using their actual coordinates. Example angular weight `w=max(0,cos(angle difference))`. **p** (size effect) and **L** (m spatial reach) are UNFITTED and not yet validated for Sitka.
3. Proposed realised sector target:
   `Rtarget[i,k] = Rpot_i × exp(-β×C[i,k])`. **β** is UNFITTED; this is a hypothesis, NOT an empirical equation.
4. Year-0 realised sectors must be reconstructed from stand spacing/competition and plausible history; do not assume all mature trees were open-grown, and do not silently change frozen derived fixtures.
5. After Fell, growth toward sector targets should be gradual, bounded by empirically constrained lateral growth `vMax_i` and response `γ_i`:
   `Rnext[i,k] = Rcurrent[i,k] + min(vMax_i, γ_i×max(0, Rtarget[i,k]-Rcurrent[i,k]))`.
   Both `vMax_i` and `γ_i` are UNFITTED. Abrupt contraction, crown dieback and asymmetric physiological persistence would need additional evidence.
6. The current map/API exposes **one scalar `crownRadiusM`**, so proposed output is an **area-equivalent circle**:
   `Req_i = sqrt((Σ_(k=1..8) R[i,k]^2)/8)`.
   This preserves the existing SVG contract if ultimately implemented. **CRITICAL LIMITATION:** an equivalent circle does NOT retain directional canopy gaps or true asymmetric crown geometry. A model with eight internal sectors then drawn as a circle can reproduce crown area but still mispredict the visible gaps that motivated this request. Quantify lost overlap/union accuracy, cost of extra model state, and whether a simpler **scalar spatially calibrated** model might in fact be more defensible.

**Candidate B strengths to assess (not predetermined verdicts):** uses explicit neighbour positions/directions; makes release response directional; permits natural crown overlap; avoids simplistic fixed per-tree growing-space caps; offers a migration path to existing scalar map output.

**Candidate B liabilities to assess:** few/unknown directional Sitka crown datasets; many free parameters and complex initialization; overfitting risk; asymmetry lost in area-equivalent circular rendering; eight-sector discretisation artefacts; time-history state/versioning costs. No measured coefficients, and no demonstrated improvement over simpler scalar calibration so far.

## C. Leon's Candidate A — source not yet supplied / independently verified

The user calls the competing solution **“Leon's solution”** here. The prior conversation called another effort **“Gemini Neon”**. **Do not silently equate Leon with Neon**, or describe Leon's algorithm without his original write-up. At packet creation, no document/branch containing a Leon proposal was located in available #899 comments, searched CCF branches (Leon/crown) or attached project search.

**Claude's FIRST ACTION:** acquire and read Leon's exact complete model proposal from the owner or its directly verifiable GitHub handoff/file; record canonical URL, SHA/version, equations/coefficients, provenance, assumptions, experimental outputs, and whether its author claims fitted field data. If unavailable, **return BLOCKED — CANDIDATE A SOURCE MISSING**, explaining that no meaningful A-vs-B scientific verdict can be given. Claude may critique B on its own while waiting, but must not invent A. If Leon's model arrives as an upload, actually read the upload; do not rely on title/chat summaries. If source is a connected document URL, use the appropriate connected app.

**Question to ask Leon/A's owner if the source omits them:** Does A change initial realised crowns, only annual crown growth, synthetic stand density, crown geometry, or combinations? Does it cite actual crowded Sitka crown-width and thinning-response observations, or tune parameters to make gaps visible? Do its equations preserve existing scalar map rendering and frozen scenario compatibility?

## D. Code-free controlled comparison — identical input, evidence and metrics

**No game code change during comparison.** Read only, publish two independent mathematical/explanatory proposals, and if concrete predictions are needed use separate offline worksheets/notebooks/standalone scratch analyses with approved read access. No committed C#, test harness, frozen fixture, API, Unity, saves, map SVG/CSS, model coefficient or P0 changes. No long-running app, extra worktree or Unity lease presumed. Do not alter the current Opus UI lane or its cleanup status.

**Common dataset/source stamp:** exact immutable MA-A and MA-B tree snapshots: area polygon, individual TreeId, X/Z metres, DBH(cm), height(m), age, living/dead/felled, current crown radius, ecological year and current fixture/source SHA. Verify the provenance of the **current MA-A/B** rather than substitute the unrelated 30m P1 derived fixture. If only the latter can be accessed, label the comparison *fixture proxy*, not a measured MA-1 result. Choose an identical list of at least ten real Fell candidates and a matched no-work control, and keep all other growth variables/scenarios fixed or disclose coupling.

**Time:** initial year; +1 and +3 after documented removals; +10 as a limited plausibility extrapolation (not a massive qualification programme).

**Report, for BOTH candidates, with the same units and inputs:**
- Individual/quantile predicted realised radius and annual lateral growth rate, by age/DBH/stocking and competitor count/CI; fit or expected range versus **independent observed Sitka crowns** including stocked/thinned stands, sample size/domain, residuals and prediction uncertainty.
- True **polygon-clipped union of projected crown footprints** and open-ground area/connected gaps (never infer coverage from summed disk areas); for B compare full-sector approximation with equal-area-circle result to quantify geometry information lost.
- Changes for neighbours nearest removals versus matched unaffected control trees, at year 0/1/3; no enforced visible gap if actual constraints do not warrant one.
- Sensitivity, parameter identifiability, deterministic repeatability, field-transfer limitations (Welsh/British vs Irish), complexity/run cost, save/fixture/model versioning requirements, current scalar `CrownRadiusM` compatibility.
- Distinguish measured evidence, derived statistics, reasonable scientific inference and purely game-tuned choices. Do not present mathematical sophistication as accuracy by itself.

**Evidence starting points (not automatically calibrated coefficients):** Tabbush & White (1988), *Canopy Closure in Sitka Spruce* https://doi.org/10.1093/forestry/61.1.23 (open-grown potential relation); Davies & Pommerening (2008), *The contribution of structural indices to the modelling of Sitka spruce and birch crowns* https://doi.org/10.1016/j.foreco.2008.03.052 (stem-mapped spatial competition; don't use counterintuitive pooled Hegyi coefficient causally); Sitka respacing literature; project research composite `CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md` and [#899 science-first draft](https://github.com/jermur788/CCF/blob/docs/multi-899-sitka-realised-crown-model-spec-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/899-crown-model/Multi-899-MANAGER-EVIDENCE-FIRST-CROWN-MODEL-CORRECTION-DISPATCH-20261122.md). Do not confuse forestry correlations with validated causal rules.

## E. Claude's verdict and output

Publish **ONE readback-verifiable GitHub Markdown independent comparison report** on a new docs-only branch, after checking actual live `Docs/Project/coordination/active-tasks.md` and obtaining nonoverlapping docs publication ownership. It should link both exact original submissions (not summaries), source/fixture SHAs, empirical evidence, a side-by-side table, numerical results only if actually computed, assumptions/unknowns and a **ranked recommendation**.

Evaluate in this priority order: (1) observational Sitka crown fit, (2) realistic year-zero realized crown and post-thinning response, (3) field-data provenance & parameter identifiability, (4) honest actual gap prediction from crown union, (5) compatibility with existing game contracts & protected determinism, (6) complexity/maintenance. Acceptable outcomes: **LEON A PREFERRED**, **DIRECTIONAL B PREFERRED**, **HYBRID WITH EXPLICIT NEW PROPOSAL**, or **NEITHER SUFFICIENTLY SUPPORTED**. Explain why, without forcing a winner. Distinguish *model recommendation* from *implemented and verified ecological accuracy*.

**The owner makes the final G5 model/coefficients/fixture-policy decision AFTER reviewing Claude's report.** No Claude comparison approval may silently change model code. After G5, Manager can approve a full spec quartet and a separately D-091-admitted single Core implementation worker, then proportionate empirical/narrow runtime qualification. Do not overlap the current UI author.

## F. One complete paste-ready CLAUDE dispatch

> **CCF Multi #899 — CLAUDE: INDEPENDENT SCIENTIFIC REVIEW OF LEON'S CROWN MODEL VS GPT-6 DIRECTIONAL CROWN CANDIDATE. DIRECT GITHUB HANDOFF: https://github.com/jermur788/CCF/blob/docs/multi-899-claude-leon-vs-directional-crown-review-20261123-v1/Docs/Handoffs/MultiRack-Simulation-20261116/899-crown-model/Multi-899-CLAUDE-INDEPENDENT-LEON-VS-DIRECTIONAL-CROWN-MODEL-COMPARISON-PACKET-20261123.md**. Difficulty HIGH; provider **Claude** in a separate read-only quantitative-review session. Owner requests a **comparative assessment only; NO production code changes** until owner G5 selection. Problem: synthetic Sitka /multi stand is too visually solid after >10 real fells/3 years, likely current realised-crown computation/initial conditions rather than SVG; keep map design fixed. Actual C# AnnualRules uses scalar Hegyi CI, potential `0.9415+0.07635×DBH_cm`, scalar `1/(1+CI/5)`, 15% annual relaxation; P1 30m derived fixture starts at zero-competition potential, but MA-A/B provenance must be confirmed. Candidate B full original: https://github.com/jermur788/CCF/blob/docs/multi-899-independent-sector-crown-alternative-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/899-crown-model/Multi-899-INDEPENDENT-DIRECTIONAL-CROWN-ALTERNATIVE-CODE-FREE-COMPARISON-20261122.md . B splits local neighbours into eight compass sectors and gives each an unfitted pressure/realised radius, bounded slow post-thinning expansion and area-equivalent scalar CrownRadiusM; note huge risk of underdetermined coefficients and loss of direction when reduced to circle. **FIRST OBTAIN LEON'S ACTUAL COMPLETE PROPOSAL** from owner or verified GitHub/upload; it was NOT available here. Do not conflate user's Leon with previously mentioned Gemini Neon without checking; if missing, report *BLOCKED ON A SOURCE*, don't fabricate A. Compare A/B on identical frozen MA-A/B coordinates/DBH/height/ages/radii, same documented Fell IDs and no-work control; examine empirical crowded Sitka measured crown widths, plausible year0/1/3 realised radii, crown union/gaps and release rates, uncertainty, scalar contract, state/versioning and maintenance cost. Offline/read-only maths only if data and scratch permitted; **NO C#/Core/annual/fixtures/UI/SVG/Unity/P0/save/main edits, new physical leases or full QA**. Publish a durable GitHub Markdown independent review with direct A/B source links, exact stamps, evidence and reasoned **A / B / hybrid / neither** recommendation; direct HANDOFF to owner G5. Separate model recommendation, implementation and field validation. Refresh live active-tasks ownership before docs publication.

**This packet publishes only Claude's review instructions. No Leon model, benchmark run, prediction, verdict, or implementation is claimed.**
