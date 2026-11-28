# Independent Review — Full Custom Woodland Builder V1, Part 1 (isolated engine)

- **Spec** `SPEC-multi-2d-custom-woodland-builder-v1` (APPROVED / IMPLEMENTING, Part 1)
- **Reviewer** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, independent reviewer; did not author the engine, spec or tests
- **Reviewed feature branch** `feat/multi-2d-custom-woodland-builder-engine-v1`
- **Reviewed SHA** `2a38c274f94b9f175da0f5d8e448068c965713d1`
- **Base SHA** `f4dee8023d3492a25e61451c7e2a49515e507ed3` (`origin/integration/2d-sim` at cut time; verified an ancestor of the reviewed SHA)
- **Review branch** `review/multi-2d-custom-woodland-builder-engine-v1` (this file is the only change)
- **Date** 2026-11-28
- **Unity** NOT RUN (out of scope; pure .NET engine). Core.Tests, Phase1.Save.Tests, browser suites, `build/2d-gate.sh` and `selftest.sh` were deliberately NOT RUN, as the brief instructs.

## Scope read

All four spec files (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`), all seven engine files in `Tools/MultiRackSitkaP1Lab/src/Core/CustomWoodlandBuilder/`, all ten test-project files, and the implementation handoff. I reviewed against FR-1..FR-8 and AC-1..AC-8.

## Commands run (from `Tools/MultiRackSitkaP1Lab`)

| # | Command | Exit | Result |
| --- | --- | --- | --- |
| 0 | `dotnet --version` | 0 | `8.0.133` (selected by `global.json`; machine also has SDK 10.0.114) |
| 0 | `dotnet --info` Host line | 0 | `Host: Version: 10.0.14` (see O-2) |
| 0 | `dotnet --list-runtimes` | 0 | `Microsoft.NETCore.App 8.0.33` and `10.0.14`; the `net8.0` test executable runs on 8.0.33 |
| 1 | `dotnet restore tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj` | 0 | restored |
| 2 | `dotnet build tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj --no-restore` | 0 | **0 Warning(s), 0 Error(s)** (`TreatWarningsAsErrors` on via `Directory.Build.props`) |
| 3 | `dotnet run --project tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj --no-build --no-restore` | 0 | **TOTAL: 42/42 passed, 0 failed**; `DIGEST canonical-sha256 86db0555aaf41405995b4213ecb3cbcadd9c0bab8fa36ac58bc37412dce8cbf5`; stderr empty |
| 4 | same as 3, separate process | 0 | identical: 42/42, same digest |
| 4 | `cmp run1.out run2.out` on raw program stdout (nothing appended) | 0 | **byte-identical**; both stdout files SHA-256 `24540ede348130663b8c32820093630adf4d256eb3f36b650d037d3bf51f03ac` |

Per-suite lines observed include `Mix 4/4`, `Moisture 9/9`, `Stand 8/8`; total 42/42. The digest equals the expected pinned value.

### Reviewer probe (out of tree, read-only)

To check the code against the spec rather than only against its own tests, I built a throwaway console in `/tmp/cwb-probe` (own `global.json` pinning 8.0.133, `net8.0`) that compiles the reviewed `SpeciesWave2/**/*.cs` and `CustomWoodlandBuilder/**/*.cs` by absolute path. Nothing was written inside the repository. Probe results are cited as evidence in the findings below. Key observations:

- Six-species weights `{beech 50, pine 50, others 0}` at 7 stems: table order gives beech 4 / pine 3; the same weights listed pine-first give beech 3 / pine 4. The permuted mix passes `Validate`.
- A `null` entry in `PaintOverrides` makes `CustomWoodlandBuilder.Validate` throw `NullReferenceException` instead of returning an error.
- `LShape(120, 120, 10, 10)` with `WetHollowBogPocket(10)` validates; its area centroid (33.70, 33.70) lies outside the polygon, so 0 bog cells and no caution.
- Edge seed tree distance to the nearest boundary edge: min 1.67–1.94 m, max 4.000 m across Rectangle, TaperedWedge, LShape and AngledStreamEdge; 2–14 trees per stand sit nearer than 3.99 m. All satisfy the 0.5 m clearance.
- Planted stems: 0 of 96,000 (200 seeds, 40 × 40 m at 3,000 stems/ha) lie on the boundary; smallest observed clearance 0.0016 m.
- Cap: 2,880 planted + 120 edge = 3,000 accepted, `NearSafetyCap` true; 2,881 + 120 = 3,001 refused with the cap message; 2,500 living (including 101 edge trees) is near-cap; 2,499 is not.
- Paint: painting one bog cell raises the caution count 0 → 1; bog then dry on the same cell goes back to 0; painting out all 12 hollow bog cells clears `HasHabitatProtectionCaution`. `NeutralLegacy` and an undefined enum value are both refused. No active cell and no tree carried `NeutralLegacy` across 9 pattern × shape builds.
- Lanes: `LShape(120, 120, 7, 10)` at 8 lanes refuses the whole plan: `lane 1 … mean width is below 8.0 m. Lanes are never dropped silently`.
- Mutating the caller's mix list after `Build` changes `build.Recipe.SpeciesMix[0]` (99 after mutation).

## Findings

| ID | Severity | Location | Finding |
| --- | --- | --- | --- |
| M-1 | MINOR | `src/Core/CustomWoodlandBuilder/CustomWoodlandStandGenerator.cs:349-353`; `CustomWoodlandRecipe.cs:179-206`; spec `spec.md:80` | **The largest-remainder tie-break follows the caller's mix order, not the table's species order.** FR-6 says ties are "broken by the table's species order". `AllocateCounts` breaks ties by entry index, and `ValidateSpeciesMix` accepts the six species in any order. Probe: equal 50/50 weights for beech and pine at 7 stems give 4/3 in table order and 3/4 when pine is listed first. The species shuffle also consumes the mix in entry order, so the same weights in a different order produce a different stand. This is deterministic and only moves ±1 stem per tie, but it does not meet FR-6 as written. The fix is one line in either place: require the mix in `SpeciesMoistureTable.SpeciesIds` order, or allocate by table index. The existing MIX-01 test only uses a table-ordered mix, so it cannot catch this. |
| M-2 | MINOR | `CustomWoodlandStandGenerator.cs:243-252`; spec `spec.md:84` | **Edge seed trees are not all on a 4 m inward offset ring.** FR-7 places them "on the polygon's inward offset ring 4 m from the boundary, strictly inside (at least 0.5 m of clearance)". The code offsets samples 4 m from their *own* edge and accepts any point that has 0.5 m clearance. Near convex corners and on acute wedge corners, trees end up 1.67–1.94 m from the adjacent edge (probe). Strict inside-ness and the 0.5 m clearance hold, so this is not a safety defect. It is a gap between the spec text and the code, and GEN-03 only asserts the 0.5 m floor. Either clamp to `DistanceToBoundary ≥ 4 − ε` (which lowers capacity) or issue a spec erratum saying "4 m from the nearest edge segment, at least 0.5 m from every edge". |
| M-3 | MINOR | `MoisturePatternGrid.cs:239-241`; `CustomWoodlandRecipe.cs:143-159` | **`Validate` is not total: a `null` paint-override entry throws `NullReferenceException`.** `MoistureGrid.Build` dereferences `painted.Moisture` without a null guard, and `Validate` catches only `ArgumentException`. The behaviour still fails closed (nothing is built), but it breaks the "returns every problem" contract that a Part 2 picker will rely on. The mix path already handles `null` entries (`CustomWoodlandRecipe.cs:188-192`), so this one is inconsistent. |
| M-4 | MINOR | `CustomWoodlandRecipe.cs:13-23, 26-32`; spec `spec.md:88` | **Requests and results are not fully immutable (FR-8).** `CustomWoodlandRecipe` stores the caller's `IReadOnlyList` references as they are, and `CustomWoodlandBuild.Recipe` exposes the same objects. A caller that keeps the underlying `List<>` can mutate it after `Build`. The probe changed `build.Recipe.SpeciesMix[0].Percent` to 99, while `CanonicalText` and `Stand` still reflect the original mix. `MoistureGrid` already defensively copies its overrides (`MoisturePatternGrid.cs:264`), so the recipe should copy too, either at `Build` or in the record. |
| M-5 | MINOR | `tests/Core/CustomWoodlandBuilder/MixAndStandTests.cs:325-352` | **Cap and near-cap boundaries are untested at their exact edges.** CAP-01 uses 4,320 (far above the cap) and CAP-02 uses exactly 2,500 planted with no edge trees. Nothing asserts that 3,000 is accepted, that 3,001 is refused, that edge seed trees count toward the cap, or that 2,499 is not near-cap. The probe confirms the code is correct on all four (accepted / refused / counted / false), so this is a coverage gap only. |
| N-1 | NIT | `CustomWoodlandStandGenerator.cs:305`; `MixAndStandTests.cs:216` | AC-7 says "every tree strictly inside the polygon". Planted stems are filtered with the boundary-inclusive `ContainsPoint`, and GEN-01 asserts the inclusive test. With continuous jitter this is never hit in practice (0 / 96,000 in the probe), but strictness is not guaranteed by construction or by test. |
| N-2 | NIT | spec `specs/multi-2d-custom-woodland-builder-v1/spec.md:103` | The evidence table labels the 2.5 m grid cell **Empirical**. It is an implementation constant matching `Stage5Options.GridCellM`, not an empirical measurement. Better labels would be "Implementation fact" or "Calibration (existing simulation grid)". The code does not repeat this label. Every other calibration value, including the deer presets `[E]`, is correctly labelled Calibration in both the spec and the code. |
| N-3 | NIT | `CustomWoodlandRecipe.cs:235` | The canonical line `boundary template=<n>` writes the vertex count, not the template. The label is misleading in the frozen canonical format. Rename it only together with a digest re-pin. |
| O-1 | OBSERVATION | `MoisturePatternGrid.cs:354`; spec `spec.md:66` | `WetHollowBogPocket` centres on the area centroid, as specified. For a thin valid L-shape the centroid lies outside the field, so the "bog pocket" pattern silently yields 0 bog cells and no caution. This matches the spec literally. Part 2 (or a spec note) should decide whether to place the hollow at an interior point or warn the player. |
| O-2 | OBSERVATION | environment | The brief expected "host 8.0.134". On this machine `dotnet --info` reports `Host: Version: 10.0.14` (the shared muxer). The SDK is 8.0.133 as pinned, and the `net8.0` executable runs on `Microsoft.NETCore.App 8.0.33`. SDK 10 was not used for build or run. |
| O-3 | OBSERVATION | `CustomWoodlandStandGenerator.cs:198-211` | Edge seed trees have no minimum spacing from planted stems, so a mature edge tree can sit almost on top of a planted stem. The spec does not require spacing. Noting it for Part 2 rendering and competition. |

No BLOCKER or MAJOR findings.

## Spot-check results against the brief's specific concerns

| Concern | Result |
| --- | --- |
| Containment / ordering assumptions | PASS. The polygon is normalised to CCW with a canonical start before any consumer reads it. The inward normal in the edge ring relies on CCW, which is guaranteed. Containment is one shared ray-cast with an on-segment short-circuit. Grid activity uses the cell centre, and trees additionally require `ContainsPoint`, so a tree in a partly outside active cell is still inside the field. Exception: the tie-break ordering assumption (M-1). |
| Rejected lane refuses the whole plan | PASS. `LanePlan.IsValid` is false on the first unusable lane. `Validate` reports the lane index and reason, and `Build` refuses through `Validate`. No lane is dropped, merged or widened (LANE-04/05/06 plus probe). |
| NeutralLegacy never produced or painted | PASS. `Uniform(NeutralLegacy)` is refused. Patterns map only to the five real classes. Painting `NeutralLegacy` or an undefined value is refused. Inactive cells hold `NeutralLegacy` internally, but `ClassAt` throws for them and trees cannot land there. The probe found 0 occurrences. |
| Bog caution recomputed after painting | PASS. `CautionCellCount` is computed after the override loop (`MoisturePatternGrid.cs:254-259`) using `SpeciesMoistureTable.HabitatProtectionCaution`. Probe: 0→1→0 and 12→0. |
| Edge seed trees strictly inside with clearance | PASS for strictly inside with ≥ 0.5 m clearance (`ContainsWithClearance`). See M-2 for the 4 m ring wording. |
| Cap and near-cap semantics | PASS. `planted + edge > 3000` is refused both in `Validate` and again in `Generate`. `NearSafetyCap` is `≥ 2500` on living trees including edge trees. No silent trimming. Untested edges: M-5. |
| Calibration mislabelled as empirical | Code: none. Deer presets carry `EvidenceClass.Calibration` plus the not-a-density note. Spec: N-2 (grid cell labelled Empirical). |
| Purity / determinism (FR-8) | PASS. SplitMix64 only, no `System.Random`, clock or I/O. Floats enter the canonical text as raw bits. Two processes are byte-identical. Immutability gap: M-4. |

## File fence

- `git diff --name-only f4dee802..HEAD` lists 22 files, all under the four authorised prefixes: `specs/multi-2d-custom-woodland-builder-v1/` (4), `Tools/MultiRackSitkaP1Lab/src/Core/CustomWoodlandBuilder/` (7), `Tools/MultiRackSitkaP1Lab/tests/Core/CustomWoodlandBuilder/` (10), `Docs/Handoffs/MultiRack-Simulation-20261116/custom-woodland-builder/` (1). Filtering out the four prefixes leaves **none**.
- All 22 are additions. `git diff --diff-filter=MDR` count is **0**, so no existing file was modified, deleted or renamed.
- Protected paths `Assets/`, `Packages/`, `ProjectSettings/`, `src/Ui/**`, `src/Core/Phase1/**`, `src/Core/ForestMaker/**`, `src/Core/Saves/**`, `src/Core/SpeciesWave2/**`, `**/RngSalts.cs`, `tests/Core.Tests/**`, `build/**`, `**/*.sln`, `Docs/Project/**` and `Docs/Handoffs/MultiRack-Simulation-20261116/2D_Simulator_Strategic_Roadmap.md` have **0** changed files.
- The only `.csproj` in the diff is the new `tests/Core/CustomWoodlandBuilder/SitkaP1.CustomWoodlandBuilder.Tests.csproj`.
- `git status --porcelain` was **clean** (0 lines) before this review document was added.
- **Fence result: PASS.**

`2D_Simulator_Strategic_Roadmap.md` was **not edited** by the feature branch or by this review. Its v1.23/v1.24 sync remains withdrawn to the separate single-writer combine.

## Not verified

- Unity import/compile/runtime, Part 2 wiring (AC-9), save round-trip: out of scope, not run.
- Core.Tests, Phase1.Save.Tests, browser suites, `build/2d-gate.sh`, `selftest.sh`: deliberately not run per the brief.
- Cross-machine or cross-OS digest stability: only verified on this Linux host with SDK 8.0.133 / runtime 8.0.33.
- The handoff's negative-control claim (digest mutation → 41/42): not re-run, because it would require editing a test file.

## Verdict

The engine meets FR-1..FR-5, FR-7's safety semantics and AC-1..AC-8 on the pinned gate, with a clean fence. M-1 (FR-6 tie-break order) and M-2 (FR-7 ring wording) are spec-conformance gaps that change no safety property. Resolve both, by a small code change or a spec erratum, together with M-3/M-4, before the Part 2 picker starts relying on mix order, `Validate` totality or recipe immutability. None of them blocks integrating the isolated, unwired Part 1 engine.

INTEGRATION DECISION: APPROVE WITH NITS — 2a38c274f94b9f175da0f5d8e448068c965713d1
