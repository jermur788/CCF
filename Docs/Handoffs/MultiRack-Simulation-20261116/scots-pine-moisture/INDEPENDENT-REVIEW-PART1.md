# Scots Pine and Site Moisture V1 — Part 1 Independent Review

```text
INDEPENDENT REVIEW
Task: MULTI-2D-SCOTS-PINE-MOISTURE-P1-20261127 (Part 1, isolated engine and rules)
Reviewer: Claude Opus 5.5 (claude-opus-5-5), fresh read-only session, medium effort
Spec: SPEC-multi-2d-scots-pine-moisture-V1 (APPROVED / IMPLEMENTING)
Reviewed HEAD: 373725dd46483ea14d5b7e67a88a039bb397eeba (feat/multi-2d-scots-pine-engine-v1)
Base: c82e4e759c2ceef1c13ddb6a033e034097aef77e (origin/integration/2d-sim)
Protected main: bdeb6a324acaf4129c10fb0846faac8b64974360 (observed unchanged)
Verdict: APPROVED FOR PART 1 — 0 blocker / 0 major / 1 minor / 4 nit
Unity: NOT RUN (out of scope). Browser, 2d-gate.sh, dotnet test: NOT RUN (by brief).
Durable evidence: /media/jer/Files/CCF/Verification/scots-pine-moisture-p1-review-20261127/ (SHA256SUMS inside)
```

## Independence

- This session wrote none of the spec, module, tests or handoff. The spec and module are by Codex GPT-6.2 Sol (5b453e30, 08b26003). A separate Claude Opus 5.5 session wrote the tests and handoff (576cafde, 373725dd).
- **Disclosure:** this reviewer is the same model family as the test author, and the local auto-memory index shared across Claude sessions contains a one-line note about that handoff. The reviewer did not open that memory file. The reviewer formed every conclusion below from the Git objects and its own runs. A reader who needs cross-family independence for the tests should treat this review as same-family.

## 1. File fence and isolation — PASS

`git diff --name-status c82e4e75..373725dd` gives 16 entries, all `A` (new). Each falls inside one of the four authorised prefixes:

- 4 × `specs/multi-2d-scots-pine-moisture-v1/**`
- 4 × `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/ScotsPine/**`
- 7 × `Tools/MultiRackSitkaP1Lab/tests/Core/ScotsPine/**`
- 1 × `Docs/Handoffs/MultiRack-Simulation-20261116/scots-pine-moisture/**`

No existing file was modified. Nothing under `Assets/`, `Packages/`, `ProjectSettings/`, `MultiArea/`, `Phase1/`, `RngSalts.cs`, `Ui/`, `Core.Tests/`, `build/`, `Docs/Project/**`, any `.sln` or `Directory.Build.props` changed. The only new `.csproj` is the in-fence test project. The base is an ancestor of HEAD. The new salt `0x1A96C03752AE8D4B` appears nowhere else in the lab tree.

The test project compiles only `src/Core/SpeciesWave2/**/*.cs` plus its own files. It inherits `net8.0`, `TreatWarningsAsErrors`, `Deterministic` and `InvariantGlobalization` from the unchanged lab `Directory.Build.props`.

## 2. Independent build and run — PASS (reproduced)

The reviewer ran from `Tools/MultiRackSitkaP1Lab` in a fresh detached worktree at 373725dd, with no `bin/obj`. The lab `global.json` resolved `dotnet --version` to **8.0.133**, under `/usr/lib/dotnet`. That host also has SDK 10.0.114, which was not used. `~/.dotnet-8.0.133` does not exist on this host.

| Step | Result |
| --- | --- |
| `dotnet restore tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj` | exit 0 |
| `dotnet build … --no-restore` | exit 0, **0 Warning(s), 0 Error(s)** |
| `dotnet run --project … --no-build --no-restore` (run1) | exit 0, **TOTAL: 26/26 passed, 0 failed** |
| second separate process (run2) | exit 0, `cmp run1 run2` byte-identical |
| digest | `96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748`, which matches the handoff pin |

Suites: Determinism 1/1, ScotsPineBiology 9/9, ScotsPineTimber 5/5, SiteMoisture 11/11. Full stdout has SHA-256 `927a34e2…399584f` in both runs. After the mutation probes, a rebuild of the restored tree gave run3, byte-identical to run1.

## 3. Spec conformance by inspection

| Req | Finding |
| --- | --- |
| FR-1 | `ForSpecies` returns `SpeciesMoistureTable.Cell` unchanged, so the establishment and growth modifiers stay separate. The table guarantees `NeutralLegacy` (1.0, 1.0). `SelectDominant` sums decimal areas per class, scans in declaration order with a strict `>`, and starts from `NeutralLegacy`/0. Ties therefore go to the earlier class, and empty or all-zero input gives `NeutralLegacy`. The map is closed on all four bounds, with `x < MidpointX` as Dry and the rest as Fresh, and it has an overflow-safe midpoint. The bog caution is derived from the class. The reviewer checked the table: pine alone is G/G on Dry, and all six species are G/G on Fresh, which confirms the spec's "dry lead / six-way fresh tie". **Conforms.** |
| FR-2 | 68/45/38 EUR/t, 1.02 t/m³ read from the card and confirmed exact as decimal, EUR 0.45 per sapling. A mature stem is Sawlog when pruned to at least `target.Low` (5.0 m) or grown under tight early stocking. Other stems are Small Roundwood or Pulp/Chip depending on their Small Roundwood suitability. The result returns sortment, price, band, volume, green tonnes, gross value and a reason, all in decimal and unrounded. **Conforms.** |
| FR-3 | The module reuses `FutureSpeciesCatalog.ScotsPine` by reference. Growth is null outside the age, stems, basal-area or ≥ 5 m top-height domain, and also for non-finite input. The growth modifier is applied once. Regeneration gates are applied in order: seed tree ≥ 18, good year, openness ≥ 0.60, undergrowth ≤ 0.30. Bounds are inclusive. The seed year is SplitMix64(seed ^ private salt + block·φ) & 1, giving offset 0 or 1 within each 4-year block. Consecutive good years are 4 + o₂ − o₁ apart, which is always 3, 4 or 5 years. That holds by construction and is independent of the query order. No off-map seed is used. **Conforms.** |
| FR-4 | Requests and results are `readonly record struct` types, the map is a sealed class with get-only state, and the module uses only the BCL. There is no I/O, clock or `System.Random`, and no existing entry point or save changed. **Conforms.** |
| D-059 | Every tuning value carries an `EvidenceTag` of Calibration or a `[Calibration]` comment. There is no pseudo-empirical claim, and the card's density keeps its Empirical/Inference tag. **Conforms.** |

## 4. Mutation probes (reviewer-run, reverted)

Each probe changed one line of source in the disposable worktree, rebuilt the project and ran the full console suite. The probe was then reverted with `git checkout`, and the final tree was verified at HEAD.

| Mutant | Result |
| --- | --- |
| seed-year offset `& 1` → `& 3`, which allows gaps outside 3–5 years | killed, REGEN-02a |
| Sawlog threshold `target.Low` → `target.High` | killed, TIMBER-01a |
| map midpoint `x < Mid` → `x <= Mid` | killed, MOISTURE-03c/03d |
| tie rule, so the later class wins | killed, MOISTURE-03b |
| undergrowth bound made exclusive | killed, REGEN-01b |
| regeneration returns the *growth* multiplier | killed, REGEN-01c |
| **growth applies the *establishment* multiplier** | **SURVIVED, 26/26 and the digest unchanged** |

## Findings

### m-1 (minor, test coverage) — no check covers which modifier growth uses

`ScotsPineGrowth.cs:53` correctly uses `GrowthMultiplier`. But every growth case in GROWTH-01 and in the DET-01 grid uses a class where Scots pine's establishment and growth tiers are equal: Dry G/G, Bog L/L or Neutral. So a swap to `EstablishmentMultiplier` passes all 26 checks and does not change the pinned digest. AC-5 ("separate moisture-adjusted and potential increments") and FR-1 ("separate modifiers") are therefore met by the code but not guarded by the tests.

**Suggested fix (next writer, not blocking Part 1):** add a GROWTH case on `WetMovingAlluvial` (pine M/L) asserting adjusted = potential × 0.35. Optionally add that class to the DET-01 growth grid, which would require a deliberate re-pin of the digest.

### n-1 (nit) — AC-4 field checks are not uniform across all four outcomes

TIMBER-01b, the tight-stocking Sawlog path, asserts sortment, value and reason but not price, band or green tonnes. The Pulp/Chip case does not assert `GreenTonnes`. The asserted gross values imply the missing fields, so the risk is low.

### n-2 (nit) — test name mismatch

The name of TIMBER-01c says "unpruned open-grown", but its Small Roundwood case uses a 4.99 m pruned stem. The behaviour is correct.

### n-3 (nit) — decimal overflow in `SelectDominant`

A pathological total area (≈7.9e28 m²) raises `OverflowException` instead of the documented `ArgumentOutOfRangeException`. This cannot happen at map scale.

### n-4 (nit, process) — handoff evidence is volatile

The handoff's evidence archive is `/tmp/scotspine-gate/` (local, volatile). The receipts are summarised inline. The storage policy puts durable verification evidence under `/media/jer/Files/CCF/`. This review's receipts are stored there, at the path above.

## Observations (no action for Part 1)

- **Post-target commit.** `64ec5f7b` ("Note Sitka joint leadership…") sits on the feature branch after the reviewed HEAD. It changes only the handoff Markdown: it adds a leader-wording note and updates the HEAD/COMMIT wording. The reviewer read it and found it accurate, since all six species are G/G on Fresh. It is not part of the reviewed code. The integrator should take 373725dd, or 64ec5f7b knowing its only extra change is that doc.
- **Review branch ref.** Before this review, `origin/review/multi-2d-scots-pine-engine-v1` already existed and pointed at `64ec5f7b`, the feature tip, not a review. The handoff states that the worker did not create it. This review replaced that ref with the docs-only review commit described below, using `--force-with-lease` against `64ec5f7b`. Nothing was lost, because `64ec5f7b` remains on `feat/multi-2d-scots-pine-engine-v1`.
- The handoff's own residual risks are accurate and remain Part 2 decisions: the 50/50 tie label, the EUR 0.45 sapling scale versus EUR 4.50 for Beech and EUR 5.50 for Oak, caller-supplied regeneration inputs, and copied Sitka literals.

## Verdict

**APPROVED FOR PART 1.** The fence is clean. The pinned gate reproduces exactly: 26/26 passing, 0 warnings, and a stable cross-process digest. The module conforms to FR-1 to FR-4 and D-059. m-1 is a test-coverage gap, not a behaviour defect. It should be closed by the next writer before or with Part 2 and does not block Part 1 integration readiness. This review is not integration. Merging into `integration/2d-sim` remains the single integrator's step, after Playable Milestone 1.
