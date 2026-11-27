# Downy Birch and Common Alder V1 — Part 1 Implementation Handoff

```text
IMPLEMENTATION HANDOFF
Task: MULTI-2D-BIRCH-ALDER-P1-20261127
Worker: OpenCode / DeepSeek V4.1 Flash (opencode-go), single lane writer
Role: Sole implementation writer (Option A, owner-approved 2026-11-27)
Branch: feat/multi-2d-birch-alder-engine-v1
Base: 24871551ae906b6911b48bb17c252c023bdea502 (origin/integration/2d-sim, live tip at cut time)
Context commit: bdeb6a324acaf4129c10fb0846faac8b64974360 (origin/main)
HEAD: the latest commit touching this file. Code under test is the module and test commits below it.
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/birch-alder/IMPLEMENTATION-HANDOFF-PART1.md
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/Multi2DBirchAlder-20261127/
  (version.txt, restore.txt, build.txt, run1.txt, run2.txt, negative-build.txt, negative-run.txt,
   restore-build.txt, run3-restored.txt)
```

**State:** isolated branch implemented and self-verified; **NOT merged**; **NOT wired**. Parked for a fresh independent review, then for the single integrator to combine one branch at a time after Playable Milestone 1 closes.

## What changed

- A four-file spec quartet, `SPEC-multi-2d-birch-alder-v1`, status APPROVED / IMPLEMENTING. It covers Part 1 (implemented here) and Part 2, which is **QUEUED** behind Playable Milestone 1 **and** Scots Pine Part 2, and is itself sequenced **Downy birch first, then Common alder**.
- A pure .NET 8 module in namespace `SitkaP1.Core.SpeciesWave2.BirchAlder`. It uses only the BCL: no I/O, no clock, no `System.Random`, no RNG-salt file edit.
  - **`BirchAlderSite`** — reads `SpeciesMoistureTable.Cell` directly for the two species and returns the table's own `MoistureCell`/`SiteResponse`. No duplicated moisture vocabulary, no copied card, no copied table row. `NeutralLegacy` is the table's own (1.0, 1.0). Bog always reports `HabitatProtectionCaution`.
  - **`WetValleyAndSlopeMap`** — two-zone helper for the standard 0.64 ha stand: 80 × 80 m, five equal 16 × 80 m lanes. Splits on the **y** midpoint so no lane is cut in half: below is `WetMovingAlluvial` (alder valley), the midpoint and above is `FreshMoistMineral` (Sitka/birch slope). Bounds inclusive; out-of-stand and non-finite samples refused. Exposes `LaneIndexOf` and `LaneBounds` so Part 2 can align mapped trees to lanes.
  - **Growth** — one shared crowding model with per-species constants. Birch: 0.85 → 0.25 cm/yr, up to 70 % crowding reduction. Alder: 0.95 → 0.20 cm/yr, up to 65 %. The site growth modifier is applied once. Both return `null` outside a declared provisional teaching window, because neither card carries a numeric readiness domain; the window is labelled calibration, not an Irish empirical domain.
  - **Local regeneration** — seed tree → good seed year → light → species gates. Birch: seed tree ≥ 12 years, openness ≥ 0.65, undergrowth ≤ 0.35. Alder: seed tree ≥ 10 years, openness ≥ 0.55, non-stagnant moisture (dry refused, bog refused with caution), exposed mineral ground required, undergrowth ≤ 0.40. One good seed year per four-year block (3–5 year gaps) under **per-species salts declared inside this module**. `RngSalts.cs` is untouched.
  - **Alder heart rot** — risk 0 before 50, linear to 0.35 at 60, flat after. A pure per-stem SplitMix64 roll under a dedicated salt; same stem identity and age always agree, no stream consumed, never fires before 50. It flags a downgrade only and never kills a living tree.
  - **Timber** — one provisional bulk price of **EUR 42/t** for both species (band 25–60), read-through card densities (birch 0.93, alder 0.842 t/m³), **EUR 0.95** per sapling for both. Quality stems (birch ≥ 30 cm, alder ≥ 50 cm, mature, straight and sound / well-released clean bole) keep the bulk price, are marked `QualityStemUnpriced` and carry the exact note `Quality stem — no local specialist buyer`. A heart-rot-downgraded alder stem reports `PulpChip` at the single bulk price with a plain reason. No premium price is invented anywhere.
- Test project `tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj`, an Exe. It compiles only `../../../src/Core/SpeciesWave2/**/*.cs` plus its own test files. It runs **38 checks**, prints per-suite and total counts and the canonical SHA-256, and exits non-zero on any failure.

## Files changed (vs base 24871551)

- `specs/multi-2d-birch-alder-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`
- `Tools/MultiRackSitkaP1Lab/src/Core/SpeciesWave2/BirchAlder/{BirchAlderSite,WetValleyAndSlopeMap,BirchAlderGrowth,BirchAlderRegeneration,BirchAlderTimber}.cs`
- `Tools/MultiRackSitkaP1Lab/tests/Core/BirchAlder/{SitkaP1.BirchAlder.Tests.csproj,Program,TestRunner,SiteAndZoneTests,BirchTests,AlderTests,TimberTests,DeterminismTests}.cs`
- `Docs/Handoffs/MultiRack-Simulation-20261116/birch-alder/IMPLEMENTATION-HANDOFF-PART1.md`

Everything is inside the four authorised prefixes. **No existing file was modified.** `git status` before the commits showed exactly those three untracked directories.

- **Serialized Unity files touched:** none. **Assets added or modified:** none. **Package/ProjectSettings changes:** none.

## Player-facing result

None yet. Part 1 is a callable library the game does not call. Part 2 will make it visible: the wet valley and slope zones on the map, alder where the water moves and a habitat caution where it stagnates, birch colonising bright openings, bulk money, and an honest quality-stem note instead of a premium.

## Simulation / save impact

None. No change to `AnnualPipeline`, `MultiAreaContinuation`, `RngSalts`, `ForestMaker`, `src/Ui`, Version 2 or Version 3 saves, or the existing Wave 2 files. No save fields. `NeutralLegacy` stays bit-exact (1.0, 1.0) for both species.

## Anchors

- **Must-not-change:** Scenario One, `Legacy40`, `Enlarged80`, Reference Future v1, existing P1 save and determinism anchors, `src/Core/SpeciesWave2/*.cs`, the parked `ScotsPine/**` lane. All untouched because no existing source changed.
- **Intentionally changed:** none.
- **New pin:** the Part 1 canonical digest `e2ce886e80871015d63bf9bd98acafafc1b6d80c59d7b725ae4f0d7abb960298`, under SDK 8.0.133.

## Verification run (from `Tools/MultiRackSitkaP1Lab`, `dotnet --version` = 8.0.133)

```text
$ dotnet restore tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj
  Restored .../tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj (in 1.23 sec).      exit 0
$ dotnet build tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj --no-restore
Build succeeded.
    0 Warning(s)
    0 Error(s)                                                                           exit 0
$ dotnet run --project tests/Core/BirchAlder/SitkaP1.BirchAlder.Tests.csproj --no-build --no-restore
SUITE CommonAlder: 9/9 passed, 0 failed
SUITE Determinism: 1/1 passed, 0 failed
SUITE DownyBirch: 9/9 passed, 0 failed
SUITE SiteAndZone: 12/12 passed, 0 failed
SUITE Timber: 7/7 passed, 0 failed
TOTAL: 38/38 passed, 0 failed
DIGEST canonical-sha256 e2ce886e80871015d63bf9bd98acafafc1b6d80c59d7b725ae4f0d7abb960298          exit 0
```

`TreatWarningsAsErrors` is on through `Tools/MultiRackSitkaP1Lab/Directory.Build.props`; the pinned `global.json` selects SDK 8.0.133 even though the machine default is newer.

**Determinism.** A second separate process run gave byte-identical full output (`cmp run1.txt run2.txt`) with the same digest. DET-01 also checks that the canonical text is identical within one process and matches the pinned digest. Doubles enter the canonical text as raw IEEE-754 bits.

**Negative control.** With a one-character change to the pinned digest, the run printed `TOTAL: 37/38 passed, 1 failed` and exited 1. The pin was then restored, rebuilt and re-run at 38/38 with the unchanged digest.

**Not run, by brief:** `dotnet test`, Unity, browser suites and `2d-gate.sh`. Part 1 is not Unity-relevant, so no `CLOUD HANDOFF — NOT UNITY-VERIFIED` item is claimed beyond that statement.

## SPEC-DRIVEN

- **Spec:** `SPEC-multi-2d-birch-alder-v1`, status APPROVED / IMPLEMENTING.
- **Tasks completed:** T-01 to T-06. T-07 (Part 2 wiring) is QUEUED behind Playable Milestone 1 and Scots Pine Part 2.
- **Deviations:** none from the spec. Commits are fence-separated: spec quartet, module, tests, handoff.
- **Unresolved requirements:** AC-7 (Part 2) only.

Acceptance criteria tested:

| AC | Cases | Result |
| --- | --- | --- |
| AC-1 neutral identity, table agreement, separate modifiers, bog caution, no duplicated foundation type | SITE-01a–f | PASS |
| AC-2 standard 0.64 ha layout, y-midpoint split, intact lanes, inclusive bounds, refusals | ZONE-01a–d | PASS |
| AC-3 birch growth shape, crowding, single site modifier, domain, regeneration gates and site multiplier | BIRCH-GROWTH-01/02, BIRCH-REGEN-01/02 | PASS |
| AC-4 alder growth, moisture and mineral gates, bog refusal, heart-rot onset/rate/determinism | ALDER-GROWTH-01/02, ALDER-REGEN-01/02, ROT-01/02 | PASS |
| AC-5 EUR 42/t, card densities, EUR 0.95 sapling, quality-stem flag and note, heart-rot downgrade | TIMBER-01–04, ROT-03 | PASS |
| AC-6 pinned gate, 0 warnings/errors, cross-process digest, file fence | DET-01 + receipts above | PASS |
| AC-7 integrated Part 2 | — | NOT TESTED (queued) |

**Spec conformance (Part 1):** PASS, worker-verified only. It has **not** been independently reviewed.

## Remaining verification gates

- A fresh independent review of the spec quartet, module and tests at this HEAD. The reviewer must not be the implementing thread.
- The coordinator creates `review/multi-2d-birch-alder-engine-v1` after that review. This worker did **not** create it.
- All Part 2 gates, in sequence: Downy birch wiring first, then Common alder. Integrated annual loop, generation preset, Version 2 and Version 3 save/load compatibility with old saves defaulting to `NeutralLegacy`, economy outcomes and `/multi` feedback.
- One sequencing risk for the integrator: the parked Scots Pine lane owns `SiteMoistureEngine`, `MoistureArea`, `MoistureSelection`, `SiteMoistureResult` and `DryRidgeAndMoistSlopeMap` inside its own folder. This lane deliberately avoided creating those types. If both lanes are combined, decide whether the thin table accessor here is hoisted into one shared helper rather than merged into two.

## Known limitations and residual risks

- **Calibration, not prediction.** Growth shapes and windows, seed-tree ages, light and undergrowth thresholds, the mineral-ground gate, quality-stem triggers and the heart-rot maximum are provisional teaching values (D-059). Neither card carries a numeric readiness domain, so the teaching windows are this spec's own invention and are labelled as such.
- **One bulk price only.** The heart-rot downgrade is visible as a sortment/reason/flag change but cannot change the money, because Part 1 has exactly one broadleaf bulk price. Part 2 should decide whether firewood needs its own price; inventing one here would have been an unapproved coefficient.
- **Quality stems earn bulk money.** That will feel disappointing in play. Part 2 must keep the buyer note visible rather than hide the flag.
- **Caller-supplied inputs.** Seed-tree age, openness, undergrowth, exposed mineral ground, stem identity, DBH and form come from the caller. There is no dispersal geometry, no defect model and no recruit creation; Part 2 must define how map state supplies them.
- **Layout convention.** The y-midpoint split keeps lanes whole but is not a survey. A real wet valley may not bisect the stand.
- **Alder bog refusal is deliberate.** The card gives alder `L/X` on bog rather than impossible; this lane still refuses regeneration there so that the species is never presented as a bog crop. Birch is allowed to colonise peat but always reports the caution.
- **Copied comparison literals.** The Sitka and Scots pine price literals in TIMBER-02 are copied because this project cannot compile those sources; a later price change would not update them.

## Unrelated work preserved

The worktree was cut clean from the base. Nothing else was touched: no other worktree, branch, review branch, lane, Unity library, save or merge. `/media/jer/Files/CCF-Primary` retains its pre-existing modified and untracked operations files, unabsorbed.

## Decisions needed

None for Part 1. For Part 2, the owner or Manager should confirm (a) whether firewood/pulp needs its own price below the EUR 42/t bulk figure, (b) the exact tie/label behaviour when a stand is a perfect 50/50 valley/slope, and (c) whether the alder quality-stem trigger should stay at 50 cm or move toward the card's missing 50–60 cm band midpoint.

## Roadmap synchronization (Guardrail 11) — NOT PERFORMED

The brief asks for a single-writer docs-only sync of `2D_Simulator_Strategic_Roadmap.md` **v1.20** onto `origin/integration/2d-sim`. The repository copy at the base commit is **v1.18**, and **no v1.19 or v1.20 text exists in any local worktree, evidence tree or Git ref** (`find` across the ZX20 2D lanes and the Files CCF tree). The v1.20 content lives in the CCF Simulator notebook, which this worker cannot read.

This worker did **not** invent strategy text. A strategy document is owner-authored product direction; fabricating v1.20 would create unauthorised project intent. Required follow-up: the designated T3 Code agent or the single integrator commits the real v1.20 file in **one** docs-only change, with `git diff --stat` as its only verification, once the text is supplied. Do not assign it to another parallel lane.

## Worker assessment

Ready for review: Yes. Scope is Part 1, offline .NET only; nothing is merged or wired.

## Recommended next step

Dispatch a fresh, non-author independent review of `feat/multi-2d-birch-alder-engine-v1` against the spec quartet at this HEAD, then let the coordinator create the review branch and park the result until Playable Milestone 1 closes and Scots Pine Part 2 lands.
