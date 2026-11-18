# Multi #729: independent review of the #725 Year 1 Sitka implementation (five files, source and numbers)

**Recommendation: APPROVED.** This covers the independent changed-lines and numerical source review only. It does not qualify a native run, make L4 release-safe, integrate the source or show Unity parity. Default .NET L4 is still **FAIL38** and is not waived.

- **Date:** 2026-11-18
- **Claim:** [#729](https://github.com/jermur788/CCF/issues/729). [RUNNING comment](https://github.com/jermur788/CCF/issues/729#issuecomment-6977203917).
- **Returned to:** Manager #730, which issued the [start permit](https://github.com/jermur788/CCF/issues/729#issuecomment-6976747800).
- **Grant:** [physical read-only grant](https://github.com/jermur788/CCF/issues/729#issuecomment-6976628939), which expires 2026-11-19T18:00Z.

## 1. Reviewer and permissions

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code inside native Linux T3 Code on `jer-laptop`. Session `0ba3b24b-d77a-464b-8fbe-2a907f6b5cb1`.
- **Independence:** this session is not the custodian session (`51663961-…`) or the #725 Codex author session (`01a1ed7e-…`). It is the session that earlier stopped read-only on the repeated #725 prompt, which the packet allows. It never wrote #725 source.
- **Effort setting:** the harness does not expose the reasoning-effort setting to me, so I cannot confirm "High". I disclosed this in the RUNNING comment.
- **Actions in the #725 lane:** only `GIT_OPTIONAL_LOCKS=0` `rev-parse`, `show`, `diff`, `ls-tree`, `grep` (on the object store) and `--no-optional-locks status`. `git grep` is not on the grant's list, but it is read-only in the same way as `show` and `diff`.
- **Things I did not do:** no dotnet, build, test or trace, and nothing run from the lane. No edit, no Git write, and no change to the board, lanes or lease.
- **My own arithmetic:** a standalone Python 3 model (`python3 -I`, standard library only), run in my scratchpad and saved under the reviewer evidence folder (§8).

## 2. Identity and scope (verified)

| Check | Result |
|---|---|
| HEAD / parent / tree | `df467dcc302eca564343e6da40f2fb88529a5d16` / `35d26f5e64a00a400c700871927ccc2173370568` (R) / `4237ece5c4efcd167857c1cb29e16bbcc2279ad6` |
| Worktree status | clean (`--no-optional-locks status` printed nothing) |
| R..HEAD diff | **exactly 5 paths**, +463/−11: M `src/Cli/Program.cs`, A `src/Core/AnnualEngine.cs`, A `src/Core/AnnualRules.cs`, A `tests/Core.Tests/AnnualTests.cs`, M `tests/Core.Tests/Program.cs` (all under `Tools/MultiRackSitkaP1Lab/`) |
| P0 tree `Tools/MultiRackSitkaLab` | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` at HEAD (same as the R pin) |
| Frozen fixture blob | `8bf81a43…` at both R and HEAD. Bytes sha256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` |
| Four CCF pins at HEAD | SimulationRandom `bb83d7f8…`, SitkaGrowthModel `e4c73402…`, SitkaSpruce.asset `e638e28e…`, TreeSpeciesDefinition `51faaa0d…`, all matching the packet |
| TrustTables / AnnualGate / analyzer / `.csproj` | not in the diff, so unchanged |
| #725 evidence folder | 79 files. My sorted-file manifest sha256 `4b9595be4e75fe274b874c76fc952bccd9b9f5af332bc816fc3214bb13575cd3` matches the custodian's. `handoff/SHA256SUMS.txt` 13/13 OK |
| Published #725 report vs local copy | byte-identical (raw GitHub at `6e51dcb6` compared with `cmp`) |

## 3. Source faithfulness: CCF controller vs adapter

I compared `AnnualRules.cs` line by line with `ForestEcologyController.cs` at HEAD (`AdvanceOneYear` 457–499, `UpdateCompetition` 930–999, `GrowAdults` 1003–1036, `ApplyAdultDensityMortality` 1062–1132, `StandRelativeDensity` 1214–1225, `RelaxCrowns` 1229–1244). I also checked `ForestTree.ApplyGrowth`/`RelaxCrownRadius` (623–636), `TreeSpeciesDefinition.PotentialCrownRadiusM` (246–255), `SitkaSpruce.asset` and the linked, unchanged `SitkaGrowthModel`/`SimulationRandom`.

| Rule | Source | Adapter | Verdict |
|---|---|---|---|
| Chronology | `ecologicalYear++` → CI → (canopy) → GrowAdults → RelaxCrowns → density mortality, all on the **pre-growth** `competitionIndex` | year = state+1; CI from `before`; Grow (DBH, height, crown) on that CI; post-growth stand; hazard on pre-growth CI; boundary | match. Storm, canopy and regeneration steps are excluded as the fixture's `excludedSystems` declares |
| Order | `FindTrees` ordinal ID sort | `OrderBy(TreeId, Ordinal)`. The manifest enforces unique, ordinal IDs (`DerivedManifest.cs:332`) | match |
| Hegyi | float `dx,dz`; `Vector2.Distance` = `(float)Math.Sqrt(dx*dx+dz*dz)`; `> 8` skip (inclusive 8 m); `(Dj/max(1,Dt))/max(0.5,dist)` float sum | same operations and storage points. The squared-distance prune is left out, which is equivalent because the source's slack band never rejects a pair it would accept | match |
| DBH | `1.2·site·Clamp01(1−D/120)·1/(1+CI/5)`, `ApplyGrowth` clamp [1,200] | same; site = 1.0f (`SiteProductivityBits`); asset has no growth-age phases | match |
| Height | `NextHeight(H, age, 1) − H` then `H + Δ`, clamp [0.1,60] | same | match |
| Crown | `max(0.1, 0.9415+0.07635·D_post)·response`, `Lerp(R, target, 0.15)`, floor 0.1 | `R + (target−R)·0.15f` = Unity `Mathf.Lerp`, same floor; post-growth DBH | match |
| Stand / RD | `count / StandAreaHectares` (int/float → float), binary64 Σ D², `Math.Sqrt`, `RelativeDensity` | same; area `3db851ec` = `900f/10000f` for a 30 m side | match |
| Hazard | `S = 1−1/(1+CI/5)` (float CI/5, then double), mean of S⁴ over living, `p>0 && Roll(...)<p`, key `ADULT-MORT-`+ID, post-increment year | same; roll skipped when p = 0 or pressure ≤ 0 | match |
| Boundary | survivors only; RD > 1; sort by S descending, then ordinal ID; running sums; stop at RD ≤ 1 or count ≤ 1 | `OrderByDescending(S).ThenBy(index-in-ordinal-order)`; same loop guards | match |

No production formula differs from the source. The extra `Mechanism` label (`hazard`/`boundary`) on top of the source cause `self-thinning` is additive reporting only.

## 4. Engine, CLI and admission

- **Admission.** `AnnualEngine.Open` first runs the unchanged `AnnualGate` and only consumes the `AnnualEngineNotImplemented` sentinel. Original P0, the Unity square, unknown or mismatched records and pending records keep their existing refusals. It then calls the existing `FixtureAccess.ReadDerived(..., RequireAccepted)`: a catalog-bound name and length, a no-follow read, SHA-256 and a strict parse. Unknown enum values are refused (`UnknownDomain`).
- **Transaction.** One `lock` covers the year check, calculation, the optional pre-commit hook and the single reference swap. State is held in immutable records over `ReadOnlyCollection` arrays, so a failure before the commit leaves the identical Year 0 object in place. A second advance is refused with `AnnualAlreadyAdvanced`. By inspection, concurrent callers serialise and only one commits.
- **CLI.** The CLI loads a fresh trusted genesis for each run and keeps nothing (no save, Host or network), so it cannot re-apply a year to persisted state. The output carries the full `P1Disclosure.Derived30` text ("NOT AN IRISH FIELD FORECAST. NOT UNITY NUMERICAL OR GEOMETRY PARITY."). It contains no paths. The JSON is deterministic: author receipts `annual-1`/`annual-2` both have sha256 `574d8746…`.
- **CLI refusals (author receipts, not re-run).** Original explicit/implicit → `OriginalAnnualRefused`. Unity → `DomainUnavailable`. Bad domain → `UnknownDomain`. Bad ID → `UnknownDerivedFixture`. Altered bytes → `FileHashMismatch`. Three arguments → usage 64, after the domain/catalog refusal has been checked.
- **Existing tests.** `tests/Core.Tests/Program.cs` still has 33 `T.Add` registrations, the same as R. The only edits are comments, assertion labels and the `AnnualTests.Register` call. All the original gate assertions (codes and cases) are kept. The B-01 banned-file-API scan covers the new `src/Core/Annual*.cs` files.

## 5. Numerical review

**Independent recomputation.** I wrote a separate float32-per-operation model of the CCF source (`model729.py`). It does not use the author's `compare-oracle.py` or #723's script. I ran it on the frozen fixture bytes and compared it **bit for bit** with the author's CLI output `tests/annual-1.json`:

- **700 per-tree values agree exactly, 0 mismatches.** These are DBH₁, H₁, crown₁ and CI as binary32, S and p as binary64, and the roll. Neighbour counts, age 30→31 and living status agree for all 100 trees too.
- **All 100 RNG numerators match.** My integer FNV + SplitMix implementation agrees with the author's CSV on all 100. G10 P30-001 = **2780209** and the model-0 control = **14399697**, both exact to #723.
- **Stand values are identical to the CLI:**

| Value | Result |
|---|---|
| Initial RD | 0.609795828454288 |
| BA/ha (initial / post-growth) | 55.87290918844456 / 58.33954510260964 |
| Post-growth Dq | 25.855791791844318 |
| Post-growth RD | 0.6375836886879149 |
| Pressure | 0.09395922171978738 |
| Mean vulnerability | 0.03541018395366311 |
| Σp | 0.07062668276950534 |
| Max p | 0.003097551888186786 |
| Max CI | P30-053 = 8.432238578796387 |
| Directed pairs | 1622 |
| Stems below 17 cm (included) | 12 |
| Deaths | 0 |

**Against #723.**
- RD, pressure, pairs, max CI and the G10/G11 numerators are exact.
- Mean vulnerability, max p and Σp differ by −2, +3 and +2 binary64 ULPs. That is within #723 §5.5's stated accumulation drift, and my model reproduces the adapter's last bits, not #723's.
- The author's "800/800 within display rounding" claim is credible. My exact values reproduce the same per-ID rows that `per-id-comparison.csv` holds.

**BA/ha expected-value change (packet focus).**
- The fixture `areaHa` bits are `3db851ec` = 0.0900000035762786865234375. That equals both `(float)(900.0/10000.0)` and the controller's `900f/10000f`.
- Σ D² × π/40000 divided by that float area gives **55.87290918844456**, the adapter's value. Divided by exact 0.09 it gives **55.8729114086345**, which is #723's 55.872911409.
- So #723 used the exact decimal area, and the adapter correctly uses the declared binary32 area, as the controller does for N/ha.
- The diff contains no production formula change; only the test's reference was rescaled (`AnnualTests.cs:30`).
- The rescaled expectation 55.872909188810056 is 3.7e-10 from the actual value, inside the 1e-9 tolerance. A regression to the exact-0.09 area would be 2.2e-6 away, about 2,200× the tolerance, so the test **still detects that regression**.

## 6. Test quality (controlled in-memory vectors)

I recomputed every synthetic vector with my own model (`synthetic-vectors.txt`). Every hard-coded discrete expectation follows from the source rules, not from echoing the code under test.

| Test | What it really exercises | Reviewer recomputation |
|---|---|---|
| 8 m / floor (`:123`) | target sees edge at exactly 8.0 (included); `BitIncrement(8f)` excluded; 0.25 m → 0.5 floor; CI = 2.125 exactly; pressure 0 so no hazard work | neighbours 2, CI 2.125, P = 0 ✓ |
| Cap + boundary + tie (`:156`) | 4 equal stems, CI exactly 6 each (equal S). Post-growth RD 1.736. Every p = 0.5 cap. P30-001 roll 0.1657 < 0.5 → hazard. Survivors RD > 1 → P30-002 removed by ordinal tie-break → RD 0.868 | ✓. It would fail under descending-ID tie order or if hazard victims were counted in the boundary |
| All hazard (`:175`) | both rolls (0.1657, 0.1355) under the cap. Empty stand, no boundary | ✓ |
| Priority / last survivor (`:184`) | B is more suppressed (S 0.48 vs 0.15) **and** the later ID, so suppression-first ordering is actually tested. The singleton keeps RD 1.51 > 1 | ✓ |
| Zero-p skip (`:192`) | singleton: S = 0 → mean vulnerability 0 → p = 0, roll null, under pressure 96 | ✓ |
| Atomicity / retry / double-advance (`:82`) | `internal Advance(beforeCommit)` throws after calculation and before commit. `ReferenceEquals` Year 0 snapshot. Retry equals a clean run. Second call refused | correct by inspection |
| Perturbations (`:63`) | seed and year change the roll; DBH changes CI and RD but not the roll key | correct |
| Frozen stand (`:16`) | all 100 rolled (p > 0) and survived; 12 below threshold; per-tree commit equality | matches §5 |

The vectors use only the `internal` `AnnualRules.Calculate` and `Grow`, through `InternalsVisibleTo`. The public API has no route that bypasses trust or turns a synthetic vector into a production fixture.

## 7. Findings

**No blocking or major defects.** The minor items below need no source repair before the Manager decision. M-1 is a precondition for any future Year 2 work.

- **M-1 (minor, latent; `AnnualRules.cs:16–18, 60`).** `Calculate` does not filter `IsLiving`. A dead tree in the input would still compete, grow and enter `Stand(before)`. The source excludes biologically dead trees in `FindTrees` and checks `IsLiving` in mortality. This cannot happen now, because the genesis has 100 living trees and `Advance` refuses any year other than 0. **Fix when Year 2 is scoped:** filter to living trees before competition, growth and stand totals, and add a vector containing a dead tree.
- **M-2 (minor test strength; `AnnualTests.cs:95–107`).** `Parallel.Invoke` with two actions may run them one after the other, so the test does not guarantee the calls overlap. The lock design is correct by inspection. **Optional:** start both calls through a `Barrier`.
- **M-3 (minor coverage gap).** Every synthetic hazard death happens at the 0.5 cap. There is no vector with 0 < p < 0.5 and roll < p, and none with RD exactly 1.0 at the boundary. Both branches are source-equivalent by inspection (`roll < probability`, `> MaximumRelativeDensity`) but are **NOT VERIFIED** by a test. **Optional:** one uncapped-hazard vector.
- **M-4 (minor wording; #725 report §"Numerical comparison", first paragraph).** "Exact comparisons cover … the 24-bit RNG numerators" sits in the #723 comparison. #723 publishes only the G10/G11 numerators exactly, plus 6-decimal rolls. The other 98 numerators were checked against the author's own integer transcription (`compare-oracle.py:11, 48–49`). The report's later paragraph says this correctly, and I confirmed all 100 independently. No action beyond being precise in any future summary.
- **M-5 (provenance note).** `core-tests-initial-ba-expectation.txt` lists 42 tests and is missing "all hazard victims leave an empty stand". It was therefore produced from an intermediate, uncommitted test file. The committed source's 43/43 log (`core-tests.txt`) is the relevant receipt. No action.
- **Note (no action).** `annual` writes its report to stdout without the `RequireStreamDescriptor(1)` check used by `generate-manifest --stdout`. That B-01 guard protects fixture bytes. A report sent to stdout and redirected by the caller's shell is not a P1 file write. `InternalsVisibleTo("SitkaP1.Core.Tests")` has no strong name, which is acceptable for a local, non-release lab.

## 8. What was verified, and how

| Claim | Status |
|---|---|
| Five-file scope, P0 tree, fixture blob, four pins | **Verified by reviewer** from Git objects |
| Source-faithful rules and chronology | **Verified by reviewer** by line-by-line inspection (§3) |
| 100-tree Year 1 numbers and zero deaths | **Verified by reviewer** with an independent model, bit-exact against author CLI output (§5) |
| BA/ha rescale correct, production unchanged, regression still detected | **Verified by reviewer** (§5) |
| Synthetic vector expectations | **Verified by reviewer** by recomputation (§6) |
| Build 0/0, 43/43 tests, two-run byte identity, CLI refusal exits | **Author receipts only**; not re-run (grant forbids it) |
| P0 L0–L3+ guard, Primary unchanged | **Author receipts only**, consistent with the Git object checks above |
| L4 | **FAIL38, unwaived.** The author's single annual trace (`guards/annual-strict-analysis.*`: exit 1, 0 protected hits, 2 unresolved: `ftruncate` memfd doublemapper and `fchmod` socket, the known #720 classes) is **not** an L4 pass. The diff contains no runtime switch, whitelist or analyzer change |
| Mono/Unity float parity, cross-platform, Year 2, save/load | **Not in scope / NOT VERIFIED** |

**Reviewer evidence:** `/media/jer/Files/CCF/Verification/CCF-729-Year1Review-20261118/reviewer/` (`SHA256SUMS`):
- `model729.py` `32efa3da…`
- `cmp729.py` `4dd5fd68…`
- `synth729.py` `d976411a…`
- `ba729.py` `e3c1b8e4…`
- `mine.csv` `a2e45946…`
- `model-output.txt` `f2e128f4…`
- `cli-vs-reviewer.txt` `b85c9858…`
- `synthetic-vectors.txt` `aec8ead2…`
- `ba-area.txt` `fcd1f20f…`

Nothing in the #725 lane, board, lanes or lease was changed by this review.

## 9. Next action

Manager #730 decides on #725 using this review. If accepted, M-1 becomes a written precondition for any Year 2 scope. M-2 and M-3 are optional test hardening that can go in a later bounded change. No repair is required for the Year 1 feature as scoped.
