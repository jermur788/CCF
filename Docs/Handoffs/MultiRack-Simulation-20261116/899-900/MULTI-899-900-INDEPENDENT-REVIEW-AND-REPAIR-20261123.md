# MULTI #899 / #900 — Independent review, repair and re-review

**Final review status: `APPROVED` — focused G2/G3 repair review.** Not integration, not owner acceptance.

| Item | Value |
|---|---|
| Feature | #899 `CrownModel 1` (C2) + #900 canopy cover and decoupled light index, 2D Sitka engine |
| Base | `1e4e02215d373e224fe6a74074ae3f62983db242` |
| First candidate | `9799914dc55b174c972dfd1b01b6c1c5515fcaf5` |
| Repaired candidate | `4e60c8ab806df56441497b682ccf783badc6e0ea` |
| Branch | `feat/multi-899-900-crown-canopy-light-20261123-v1` |
| Authority | APPROVED #899/#900 quartets + D-094 at `cf05f93e` |
| Implementer | Claude Opus 5.5, effort medium (one writer, new D-091 lane) |
| Reviewer | GPT-6.1 Sol, medium — different provider and context, read-only |
| Orchestrator | opencode-go / DeepSeek V4.1 Flash — custody, verification, publication; no source edits |

## 1. First review — NEEDS CHANGES

**Status: NEEDS CHANGES. Gate G2/G3. Spec conformance: FAIL. No blocking findings.**

| # | Severity | Finding |
|---|---|---|
| 1 | Major | `CanopyLightEvaluator.cs` cover weighted included 0.25 m cells at the full 0.0625 m² and divided by sampled cells. Boundary cells were not clipped and the fraction was not the rack-area fraction. Measured: A 915.0 sampled vs 912.455357 clipped; B 1499.0 vs 1494.331430. |
| 2 | Major | `CanopyLightTests.cs` matched control used a 12-tree Fell; approved C07/R11 require **10**. |
| 3 | Minor | C2 and union tests used implementation-derived oracles and only non-overlapping crowns. |

Reviewer adjudications accepted:

- **13× criterion:** acceptable as a proxy for grid decoupling (1.05 decoupled vs 2.47 legacy reproduced). The original 13× setup stays `UNVERIFIED` and must not be described as reproduced.
- **5.0 m parity:** faithful to the approved equation-parity wording; the transcription matches `RecomputeCanopy` blob `0290a7be`. This is .NET equation parity, **not** Unity runtime equivalence.
- **Matched control:** a genuine ecological clone; the reported 12-tree deltas reproduce, but 12 did not satisfy the written 10-tree gate.

## 2. Repair — `4e60c8ab` (one bounded loop, same writer)

Exactly four files: `CanopyLightEvaluator.cs` (Layer B only), `CanopyLightTests.cs`, `CrownC2Tests.cs`, the implementation handoff.

- Every included micro-cell is weighted by exact polygon-clipped area; covered/overlap/opening areas use those weights; `Uncovered = polygon − covered`; `CoverFraction = covered / polygon area`; `SampledAreaM2` remains visible and is not renormalised.
- New sloped-boundary oracle `O-07` on the triangle `(0,0) (9,1) (2,7)`, hand area **30.5 m²**: covered + uncovered = 30.5 within 1e-9; full-crown fraction **0.972417** with the 0.841 m² edge loss counted as uncovered (the old formula would return 1 and 0).
- `MC-01` now uses exactly **10** trees, asserts the count and the executed removals, and reports Y0/+1/+3 stand, cover, light, removed IDs, crown-radius distributions and annual expansion. `MC-02` is explicitly supplementary 12-tree evidence.
- `C2-00` compares against hand-computed literal radii (DBH 30 at CI 0/5/15; DBH 45 at CI 2.5; one relaxation step) to 1e-6, plus an end-to-end Hegyi case. `O-08`: duplicate crowns preserve union exactly; two overlapping crowns match the analytic union (45.0625 vs 45.493341, 3 %) and lens (10.8125 vs 11.055327, 5 %).

Post-repair genesis cover (0.25 m cover grid):

| Area | Polygon m² | Sampled m² | Covered m² | Uncovered m² | Fraction | Overlapped m² |
|---|---|---|---|---|---|---|
| A | 915 | 912.4554 | 882.2861 | 32.7139 | 0.964247 | 549.5526 |
| B | 1500 | 1494.3314 | 1318.0746 | 181.9254 | 0.878716 | 577.7401 |

## 3. Focused re-review — APPROVED

**APPROVED — focused G2/G3 repair review. Reviewed HEAD `4e60c8ab`, against `9799914d`.**

- Finding 1 **FIXED** — clipped weighting, polygon-area fraction, visible sampled area, independent O-07 that necessarily fails under the old formula.
- Finding 2 **FIXED** — exactly 10 selections, exact executed removals, required radius/expansion evidence, MC-02 supplementary.
- Finding 3 **FIXED** — independent literal oracles; duplicate and overlapping union checks.
- No new blocking or major issue; diff limited to the four files; no public record signature change; UI, Assets and fixtures untouched.
- Reviewer ran the suite independently: **160/160 passed**, build 0 warnings / 0 errors.

## 4. Orchestrator verification (independent of both agents)

| Check | Result |
|---|---|
| Core suite | **160/160 passed**, 0 failed |
| Model 0 fingerprints | `7e3a2a86…` and `a692a6d5…` unchanged |
| Builds | Core / Cli / Ui: 0 warnings, 0 errors |
| P0 pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| `/multi` live | page **200**, `api/multi/view` **200**, both areas, 0 exceptions, listener stopped |
| Scope | 11 implementation files on the branch; no Ui/Assets/fixtures/saves/`main` change |
| Protected listeners | 839989, 1750123, 1806361 untouched |

## 5. Honest state

- **IMPLEMENTED:** yes, on the task branch.
- **NUMERICALLY QUALIFIED:** yes — author, reviewer and orchestrator runs agree; this is offline/model-level evidence.
- **REVIEWED:** yes — independent different-provider review and focused re-review.
- **INTEGRATED:** **no** — no merge to `main` or any integration branch.
- **ACCEPTED:** **no** — owner G2 remains the acceptance route.

Known limits carried forward: Unity runtime parity not run (equation parity only); original 13× setup `UNKNOWN`; no calibrated PAR/LAI; `/multi` still defaults to model 0 by spec; runtime ~29–39 ms per evaluation (Debug, WSL); no UI overlay by decision D-094.

## 6. Next authorised actions

1. Optional owner-visible demonstration: a separately approved slice to select model 1 and consume the read-out in `/multi` (UI change, not covered here).
2. Integration decision by the Manager (no merge has been performed).
3. Any growth-coupling of the woodland CI is a **new** decision; the approved spec freezes DBH/height/mortality.
