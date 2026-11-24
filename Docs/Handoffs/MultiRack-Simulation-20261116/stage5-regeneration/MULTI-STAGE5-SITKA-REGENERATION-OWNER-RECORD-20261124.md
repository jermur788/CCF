# MULTI Stage 5 — owner record and limitation disclosure (documentation-only amendment): handoff

**State: DOCUMENTED — IMPLEMENTED ON TASK BRANCH, DELTA-REVIEWED APPROVED, NUMERICALLY QUALIFIED WITHIN THE AGGREGATE SCOPE OF S5-07A, NOT INTEGRATED.**

This pass changed Markdown only. It records three owner decisions of 2026-11-24 and one measured limitation, so the spec says exactly what `S5-07A` and `S5-02` certify. No production code, test code, `.sln`, `.csproj`, `Assets/`, `*.meta`, `/multi` UI, save schema or `.p0ref` changed.

## 1. Identity and SHAs

| Item | Value |
|---|---|
| Worker | Claude Code (Claude Opus 5.5, `claude-opus-5-5`) in T3 Code on the owner's Windows host; lane in WSL Ubuntu; no subagents |
| Role | Sole writer, documentation-only amendment |
| Base | `5b757d9b4a353d07557db95d3af9227c8d841874` (clean, equal to origin at start); delta review APPROVED at this commit |
| **Spec amendment (docs branch)** | `a13e316500375dfe01a5efe9f5d8222c25d99d8a` on `docs/multi-stage5-sitka-regeneration-spec-20261123-v1`, parent `46c08e1f` |
| Amendment on the feature branch | `639eb2c0`, a cherry-pick `-x` of `a13e3165`. The four spec files have identical blob IDs on both branches |
| Handoff commit | The commit that adds this file, on top of `639eb2c0`; it is the feature branch head |
| Files changed (`5b757d9b..HEAD`) | `specs/multi-stage5-sitka-regeneration-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`, this file, and a short pointer appended to `MULTI-STAGE5-SITKA-REGENERATION-REPAIR-FINAL-20261123.md` |

How the docs commit was made: I built it on `46c08e1f` from the edited spec files, using a temporary Git index (no checkout and no extra worktree), then cherry-picked it onto the feature branch. A first attempt failed in shell quoting before any Git command ran; nothing was committed or stashed by it.

## 2. Decisions recorded (owner, 2026-11-24)

### R1 — `S5-07A` keeps the whole-cell unit

- *The gate.* It covers the matched population: the aligned 5 m blocks represented on both lattices, 101 on MA-1. It uses whole-cell units: in-polygon 2.5 m cell centres × 6.25 m², against 5.0 m cells × 25 m², per hectare. The pooled and the mean ratio, for both metrics in both scenarios, must lie in `[0.75, 1.33]`.
- *Alternatives rejected.* The owner explicitly considered and rejected two other blocking units:
  - each grid's own clipped area;
  - the same true-ground area.
- *Honest reason.* The whole-cell denominator counts **nominal grid area** for 5.0 m, not forest area: 2525 m² nominal against 2270.06 m² clipped. So "per hectare" here means **nominal matched grid area**. This basis is conservative for the promotion result, which is why the owner kept it. It is not conservative for accepted abundance: the other bases give 0.855–0.879, still in tolerance. The spec says so too.
- *Superseded label.* "Per hectare of matched in-polygon area" is kept visible as superseded in spec §9 Q4 and `requirements.md`.

**Measured comparison, recorded verbatim** (pooled 2.5 m / 5.0 m; shared field; matched population; 20 seeds; 30 years):

| Basis | Thinning promo | No work promo | Thinning abundance | No work abundance |
|---|---|---|---|---|
| whole-cell (gate) | 1.247 | 1.181 | 0.956 | 0.952 |
| own clipped area | 1.146 | 1.085 | 0.879 | 0.875 |
| same true ground | 1.120 | 1.061 | 0.859 | 0.855 |

### R2 — Boundary limitation, accepted by the owner

**`S5-07A` certifies the matched population in aggregate only. Matched edge blocks are not equivalent under the Option A rule, and no edge-equivalence claim is made.**

- *Edge promotions.* Matched edge promotions are **1.902** (managed thinning) and **1.841** (no work) per nominal hectare. All 40 seed-runs are above 1.33; the per-seed minimums are 1.745 and 1.683, as printed by the test.
- *Edge share.* Edge blocks are 15–24 % of the gated area (343.75 / 2268.75 m² and 600 / 2525 m²). They carry 44–69 % of the promotions (1106 / 2520, 1015 / 2249, 561 / 819, 532 / 772).
- *Mechanism.* A promotion exports only the in-polygon cells' abundance but always creates one whole tree, and the per-block credit rarely binds. The arithmetic: `4/n × raw edge ratio = (600 / 343.75) × (1106 / 1015) = 1.745 × 1.090 = 1.902`; for no work, `1.745 × 561 / 532 = 1.841`.
- *No clean basis.* No area basis makes both edge metrics clean.
  - Whole-cell units make edge abundance look exact (0.99–1.04).
  - Ground-based units bring edge promotions to 1.06–1.30 but push edge abundance to 0.57–0.71.

Nobody may quote the aggregate pass as an edge-equivalence claim.

### R3 — `S5-02` margins adopted by the owner

The owner adopts these as binding `S5-02` margins, from 2026-11-24:
- a 0.315 m height lead;
- dense abundance at most 0.8 × the gap's;
- both compared at years 12 and 15.

The owner adopted them after review confirmed two things:
- *Spec-derived.* 0.35 × 0.90 × 1.0; 1 − 0.2; 0.15 + 11 × 0.315 = 3.615 ≥ 3.5.
- *2–5× away from the observed data.* In year 12 the margins are 4.8× on height and 2.3× on abundance. In year 15 they are 4.1× and 1.8×, which the spec states.

**Provenance disclosure.**
- The implementing worker of the final bounded repair first chose these margins after reading the previous handoff's gap height (3.24 m in year 12) and its 7/12 poor-light census.
- The earlier spec text presented them inside the 2026-11-23 owner-decision record, as if they were an owner record, before the owner had seen them. The final-repair handoff repeated the phrase "before this pass measured anything".
- This amendment makes the margins the owner's from 2026-11-24 only. It does not imply that they were the owner's beforehand.
- The superseded spec sentence is kept visible in a provenance correction, and `plan.md`'s "margins fixed in advance" is kept visible as superseded.

## 3. Label nit

- *Spec text.* The spec's "raw-count matched ratios" is corrected to "same-true-ground ratios", with the old wording kept. Both lattices' matched blocks cover the same ground, so the raw-count ratio is exactly the same-true-ground per-hectare figure (the third row of the table).
- *Open cosmetic item, not fixed.* The test output still prints "raw-count ratio … (not area-normalised, not gated)" (`Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Stage5GridStudy.cs:544`). Fixing it needs a test-code edit, which this pass may not make.

## 4. Verification (this pass)

- **Build.** `TMPDIR=/tmp/stage5-docs2 /home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` gives **0 warnings, 0 errors**.
- **Tests.** `…/SitkaP1.Core.Tests.dll` gives **209/209 passed, 0 failed**, exit 0. The run printed the `1/1/1/1` fingerprint `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35`, unchanged.
- *When the run happened.* It was on the code tree of `5b757d9b`, before the Markdown edits. Docs-only commits do not change the compiled tree, which is the same at HEAD.
- *Source of the edge figures.* The edge figures in R2 (1.902 / 1.841, Σ 1106 / 1015 and 561 / 532, per-seed minimums 1.745 / 1.683, edge abundance 0.991 / 1.036) were read from this run's `S5-07A shared field SPLIT` lines.
- **Diff hygiene.** `git diff --stat 5b757d9b..HEAD` lists only Markdown under `specs/multi-stage5-sitka-regeneration-v1/` and `Docs/Handoffs/…/stage5-regeneration/`. There are zero files under `Tools/`.

## 5. Not verified

- *The ground-based edge ranges (1.06–1.30 for promotions, 0.57–0.71 for abundance).* These come from the owner's task packet and were not reproduced in full by me.
  - From printed pooled totals, same-true-ground edge ratios are 1.090 and 1.054 for promotions, and 0.568 and 0.594 for abundance.
  - The no-work promotion figure, 1.054, sits just below the stated lower bound of 1.06. That may be rounding, or a mean or per-seed basis.
  - The test does not print the own-clipped edge areas, so I could not check the upper ends.
- *The owner's own words* for R1–R3. They were taken from the task packet.
- *Unity runtime, ecological validity, UI runtime and `/multi`.* These are unchanged from the final-repair handoff and were not exercised here.
- *GitHub rendering of the edited Markdown.* Not checked.
