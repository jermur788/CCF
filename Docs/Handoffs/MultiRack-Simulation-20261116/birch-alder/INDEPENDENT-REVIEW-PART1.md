# Downy Birch and Common Alder V1 — Part 1 Independent Review

## Review identity

| Field | Value |
| --- | --- |
| Reviewer | GPT-6.2 Sol, Codex pool, `reasoningEffort: high` |
| Reviewer thread | Separate delegated child thread, fresh context, no shared history with the implementer |
| Implementer | OpenCode / DeepSeek V4.1 Flash (`opencode-go`), independent lane writer |
| Independence | Different provider and model from the implementer; authored none of the reviewed code |
| Review type | Tier 1 Minimum Necessary Testing, per Delivery Manager directive 2026-11-27 |
| Date | 2026-11-27 |
| Branch reviewed | `feat/multi-2d-birch-alder-engine-v1` |
| Reviewed HEAD | `73ed9fe38ce473068803443e841915b8495ad4dc` |
| Base | `24871551ae906b6911b48bb17c252c023bdea502` (`origin/integration/2d-sim`) |
| Context commit | `bdeb6a324acaf4129c10fb0846faac8b64974360` |
| Verdict | **NEEDS CHANGES** |

## Directive reconciliation

- **Roadmap synchronization is withdrawn from this lane.** The Delivery Manager directed on 2026-11-27 that `2D_Simulator_Strategic_Roadmap.md` v1.20 was superseded by v1.21 and is handled by a separate single-writer GitHub sync under Guardrail 11. The feature branch contains zero roadmap edits, and the reviewed HEAD was deliberately left at `73ed9fe3` rather than adding a docs commit that would move the reviewed artefact. The "Roadmap synchronization (Guardrail 11) — NOT PERFORMED" section of `IMPLEMENTATION-HANDOFF-PART1.md` is therefore **superseded and withdrawn**; no roadmap work is outstanding in this lane.
- **No repair was executed in this publication.** The findings below stand as the reviewer recorded them. The feature branch remains at `73ed9fe3`. A repair would change the pinned digest and the reviewed HEAD, so it needs its own packet and a fresh G2/G3 re-review.

## Verbatim integration decision

```text
INTEGRATION DECISION
Status: NEEDS CHANGES
Gate: G2 / G3 apply to acceptance, calibration and deterministic evidence. G4 remains pending for the single integrator.
Reviewed branch: feat/multi-2d-birch-alder-engine-v1
Reviewed HEAD: 73ed9fe38ce473068803443e841915b8495ad4dc
Context commit: bdeb6a324acaf4129c10fb0846faac8b64974360
SPEC ID / STATUS: SPEC-multi-2d-birch-alder-v1 / APPROVED-IMPLEMENTING
Reasons:
- AC-4 fails because alder regeneration accepts NeutralLegacy without a qualifying moisture class.
- AC-1, AC-2, AC-3, AC-5 and AC-6 pass. Both standalone runs passed 38/38 with identical output and a clean file fence.
Required fixes / remaining gates:
- Restrict alder regeneration to the three approved moisture classes. Add a NeutralLegacy refusal regression and retain bit-exact table identity.
- Repeat the pinned gate and obtain independent G2/G3 re-review at the corrected HEAD before G4.
- AC-7 remains queued behind Playable Milestone 1 and Scots Pine Part 2, with Downy birch before Common alder.
Integration target: origin/integration/2d-sim (NOT merged by this review; single integrator only)
Integrated HEAD: not applicable
Post-integration verification: not applicable
SPEC CONFORMANCE: FAIL
```

## Verification receipts

The reviewer confirmed `git rev-parse HEAD` returned the requested full SHA and `git status --porcelain` was empty before work began. The separate `CCF-Primary` coordination checkout had pre-existing modified and untracked files; the review left them untouched.

From `Tools/MultiRackSitkaP1Lab`, with the pinned SDK `8.0.133`:

```text
dotnet restore tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj
  Restore exit 0: All projects are up-to-date for restore.

dotnet build tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj --no-restore
  Build exit 0: Build succeeded. 0 Warning(s), 0 Error(s).

dotnet run --project tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj --no-build --no-restore   (twice, separate processes)
  SUITE CommonAlder: 9/9 passed, 0 failed
  SUITE Determinism: 1/1 passed, 0 failed
  SUITE DownyBirch: 9/9 passed, 0 failed
  SUITE SiteAndZone: 12/12 passed, 0 failed
  SUITE Timber: 7/7 passed, 0 failed
  TOTAL: 38/38 passed, 0 failed
  Run 1 DIGEST canonical-sha256 e2ce886e80871015d63bf9bd98acafafc1b6d80c59d7b725ae4f0d7abb960298
  Run 2 DIGEST canonical-sha256 e2ce886e80871015d63bf9bd98acafafc1b6d80c59d7b725ae4f0d7abb960298
```

Full stdout was byte-identical between the two runs (`cmp` exit 0, no output).

File fence:

| Command | Output |
| --- | --- |
| `git diff --name-only 24871551..73ed9fe3` | 4 paths under `specs/multi-2d-birch-alder-v1/`, 5 under `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/BirchAlder/`, 8 under `Tools/MultiRackSitkaP1Lab/tests/Core/BirchAlder/`, 1 under `Docs/Handoffs/MultiRack-Simulation-20261116/birch-alder/`. Zero outside. |
| `git diff --stat` | 18 files changed, 1934 insertions(+) |
| `git diff --name-status` | 18 `A` entries; zero existing files modified |
| After all runs | `git status --porcelain` empty; HEAD unchanged at `73ed9fe3` |

Zero changes under `Assets/`, `Packages/`, `ProjectSettings/`, top-level `src/Core/SpeciesWave2/*.cs`, `src/Core/SpeciesWave2/ScotsPine/`, `src/Core/MultiArea/`, `src/Core/RngSalts.cs`, `src/Ui/`, `tests/Core.Tests/` or `build/`. No production callers, save wiring, I/O, clock reads or ambient RNG use were found in the module.

**NOT TESTED**, by the packet's limits: a behavioural mutation of the digest fixture, integrated runtime and save behaviour, Unity, browser/Playwright, `dotnet test`, `2d-gate.sh`, `selftest.sh`.

## Acceptance criteria

| Criterion | Result | Evidence |
| --- | --- | --- |
| AC-1 | **PASS** | `SITE-01a–e`: exact identity bits, all 12 table cells, separate modifiers, bog caution, catalog object reuse. The module calls the foundation directly. |
| AC-2 | **PASS** | Five full-length lanes retained across x; y-only split covers both halves; y = 40 m is `FreshMoistMineral`; invalid samples refused; the value immediately below the boundary is checked as well as the boundary itself. |
| AC-3 | **PASS** | Growth declines with age, loses 70 % at full crowding, applies only the growth multiplier and is unavailable outside the teaching domain. Regeneration rejects age 11, openness 0.64 and cover 0.36 and accepts the exact thresholds 12 / 0.65 / 0.35. Eligible birch on bog returns 0.70 and a caution. |
| AC-4 | **FAIL** | Growth, bog refusal, exact thresholds, mineral ground and the heart-rot sample pass. The moisture gate also makes an otherwise eligible alder gap on `NeutralLegacy` eligible at 1.0, although FR-4 requires one of three named classes. |
| AC-5 | **PASS** | Catalog densities 0.93 and 0.842 t/m³; EUR 42/t and EUR 0.95/sapling for both; both quality DBH cutoffs straddled with heart rot out of play; buyer note and bulk value checked; alder heart rot changes sortment to `PulpChip` at the same bulk price with no mortality path. |
| AC-6 | **PASS** | Pinned restore and build succeeded; both runs 38/38 with identical output; all 18 tracked changes are authorised additions. |
| AC-7 | **NOT TESTED** | Queued behind Playable Milestone 1 and Scots Pine Part 2; Birch first, then Alder. |

## Findings

1. **MAJOR — Alder regeneration treats unknown moisture as suitable.** The gate rejects `DryFreeDraining` and `StagnantPoorBog`, so `NeutralLegacy` falls through to `Eligible` when the other conditions pass. The foundation defines that class as an unspecified legacy site with identity multipliers. FR-4 requires positive evidence of a suitable moisture class. Allow only the three named classes for alder, keep the bog-specific refusal and caution, and add a regression case with every other gate satisfied. The foundation's neutral cell must still return (1.0, 1.0). `BirchAlderRegeneration.cs` (moisture gate) and `AlderTests.cs` (missing case).
2. **MINOR — No independent age-50 zero assertion.** `ROT-01` starts its monotonic check at `previous = -1.0` and `ROT-02` samples pre-onset at 49. The implementation returns exactly zero at 50, but the suite would not catch a positive age-50 result that preserved the sampled points. Add a bit-exact zero assertion at 50 and include 50 in the no-downgrade sample.
3. **MINOR — The default regeneration result reports eligibility.** `Eligible` is enum value zero, so `default(BirchAlderRegenerationResult).CanRegenerate` is `true` even with no species or valid site. A caller using a default value from a failed lookup can read it as a successful decision. Reserve zero for an unevaluated status. Also specify that `OldestOnMapSeedTreeAgeYears` refers to a seed tree of the requested species.
4. **MINOR — Two provenance comments are misleading.** EUR 42 is the approved calibration value, while the arithmetic midpoint of EUR 25–60 is EUR 42.50. The sapling comment heading says `[Calibration/Transfer]`, although the individual labels correctly identify birch as empirical and alder as calibration. Describe EUR 42 as the selected value near the band midpoint and align the heading with the individual labels.
5. **OBSERVATION — The digest is meaningful for the fields it serialises.** A changed serialised value changes the SHA-256 input and `DET-01` compares against a fixed literal. The heart-rot portion hashes totals, so reassigning flags among stem identities can preserve the digest. Regeneration uses only `WetMovingAlluvial`, which explains why the moisture deviation leaves the pin green. Add the neutral rejection case to the fixture and hash individual sampled rot flags if stem-level identity is part of the pin's contract.

## Spec deviations

FR-4 requires suitable moisture for alder regeneration. The implementation additionally authorises `NeutralLegacy`, which supplies an identity response but establishes no moisture class. The handoff's AC-4 PASS and "deviations: none" claims therefore overstate conformance. The correction fits inside the authorised paths and needs no Part 2 wiring or product decision.

## Proposed bounded repair — NOT EXECUTED

Recorded so the next packet is unambiguous. It is a proposal, not a change, and it needs the Delivery Manager's authorisation:

1. Replace the alder moisture gate with an allow-list: `WetMovingAlluvial`, `FreshMoistMineral`, `SeasonallyWetAerated`. Keep the distinct `BogNotAForestCrop` status and the always-on habitat caution.
2. Add `ALDER-REGEN-01d`: an otherwise fully eligible alder gap on `NeutralLegacy` must be refused, while `SpeciesMoistureTable.Response` still returns exactly (1.0, 1.0) for both species.
3. Add the bit-exact zero at age 50 to `ROT-01` and include 50 in the `ROT-02` no-downgrade sample.
4. Reserve enum zero for `NotEvaluated` (rename or reorder `BirchAlderRegenerationStatus`) and document that the seed-tree age is per requested species.
5. Correct the two provenance comments. Do not reprice: EUR 42 stays the approved value.
6. Add the `NeutralLegacy` case to the determinism fixture. Expect the canonical digest to change; re-pin only after the gate is green and record the new digest.
7. Re-run the pinned gate twice, then obtain a fresh independent G2/G3 re-review at the corrected HEAD before any G4.

## Repository state after this publication

- `feat/multi-2d-birch-alder-engine-v1` remains at `73ed9fe38ce473068803443e841915b8495ad4dc`, unmodified by the reviewer and by this publication.
- This review branch adds only this document. It does not modify the engine, spec, tests or handoff.
- `origin/main` remains at `bdeb6a324acaf4129c10fb0846faac8b64974360`. Nothing was merged.
- `origin/integration/2d-sim` was `24871551ae906b6911b48bb17c252c023bdea502` when this branch was cut and had advanced to `674c7c9e470d0aff0a028bd284fd7510f973e2d5` (Playable Milestone 1 closeout) by publication time. `24871551` remains an ancestor of the new tip, and none of this lane's 18 files collide with anything that landed in between. The integrator should re-base or merge normally at G4. The "after Playable Milestone 1 closes" prerequisite is now satisfied; the Scots Pine Part 2 ordering still stands.
