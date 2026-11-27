# MULTI-2D-JAY-WINDSTORMS-P1-20261127 — Implementation Handoff (Part 1)

> **Part 1 only. Not wired. Not merged.** Pure .NET 8 BCL engine and standalone offline tests in
> `SitkaP1.Core.NaturalProcesses`. Nothing calls this code yet: no `AnnualPipeline`, `MultiAreaContinuation`,
> `RngSalts`, ForestMaker, Save v3 or `/multi` UI change. No Unity was opened or required. Independent review pending.

- **Branch:** `feat/multi-2d-jay-windstorms-engine-v1` (pushed; no `review/*` branch created)
- **Base:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (`origin/integration/2d-sim`, unchanged after `git fetch origin`)
- **Context:** `origin/main` = `bdeb6a324acaf4129c10fb0846faac8b64974360` (verified; untouched)
- **SDK:** `8.0.133` via `Tools/MultiRackSitkaP1Lab/global.json` (`/usr/bin/dotnet`)
- **Writer:** Claude Opus 5.5 (sole implementation writer). The author must not review this candidate.

## 1. Components built

| File (`Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/`) | What it does |
|---|---|
| `NaturalProcessContracts.cs` | Immutable records/enums for all four engines; append-only `NaturalProcessRefusalCode` |
| `NaturalProcessRandom.cs` | `NaturalProcessStreams` fixed IDs; keyed roll = verbatim `SimulationRandom.Roll` model 1 port; per-call draw counter + test draw log |
| `WoodlandFootprint.cs` | Polygon validity, shoelace hectares, extent, boundary-inclusive containment |
| `JayAcornTransport.cs` | `r = 5 + 195u²` m, uniform bearing, exported when outside (no redraw/clamp/retry) |
| `JayAcornDispersal.cs` | OFF / mast gates (zero draws), 10/ha on-map cap by diversion, source ID kept |
| `NearbyOakWoodland.cs` | Authored source line 1 m outside one edge, 6/ha cap, no fabricated tree ID |
| `StormVulnerability.cs` | Frozen D-050 constants, ReducedProposal V, BoundedRational p, Stable/Watch/Exposed |
| `StormAdapter.cs` | StormModel 0 (zero draws) / 1; one woodland-wide event; `u·360` blow-towards (D-050, Repair 1); glyph = direction; two-pass victims |
| `SalvageAccounting.cs` | Default leave; Work Plan gate; Sell revenue only; Keep retained timber; deadwood-ID guards |
| `NaturalProcessText.cs` | Qualitative labels, convention text, dated non-causal teaching note |

Tests (`tests/Core/NaturalProcesses/`): `SitkaP1.NaturalProcesses.Tests.csproj` (Exe; single
`<Compile Include="../../../src/Core/NaturalProcesses/**/*.cs" />`; zero ProjectReference/PackageReference/P0),
`TestRunner.cs`, `Program.cs`, `Fixtures.cs`, `JayDispersalTests.cs`, `StormAdapterTests.cs`,
`SalvageAccountingTests.cs`, `HygieneAndParityTests.cs`.

Spec quartet: `specs/multi-2d-jay-windstorms-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}`
(JW-REQ-01..23 mapped to named tests).

## 2. Commands and captured output (from `Tools/MultiRackSitkaP1Lab`, clean `bin/obj`)

```
$ dotnet --version
8.0.133
$ dotnet restore tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj
  Determining projects to restore...
  Restored <worktree>/Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj (in 255 ms).
exit=0
$ dotnet build tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-restore
MSBuild version 17.8.60+293f66545 for .NET
  SitkaP1.NaturalProcesses.Tests -> <worktree>/Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/bin/Debug/net8.0/SitkaP1.Core.NaturalProcesses.Tests.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)
exit=0
$ dotnet run --project tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-build --no-restore
PASS [jay] JW-T01 jay OFF by default consumes zero RNG draws and returns empty results
PASS [jay] JW-T02 Nearby ON with Jay OFF is refused with zero draws
PASS [jay] JW-T03 non-mast year produces zero jay input and zero draws (Jay and Nearby ON)
PASS [jay] JW-T04 pure Sitka with Nearby OFF produces zero jay input
PASS [jay] JW-T05 pure Sitka with Nearby ON (North) gives <= 4 edge-attributed candidates on 0.64 ha
PASS [jay] JW-T05 pure Sitka with Nearby ON (South) gives <= 4 edge-attributed candidates on 0.64 ha
PASS [jay] JW-T05 pure Sitka with Nearby ON (East) gives <= 4 edge-attributed candidates on 0.64 ha
PASS [jay] JW-T05 pure Sitka with Nearby ON (West) gives <= 4 edge-attributed candidates on 0.64 ha
PASS [jay] JW-T06 on-map cap diverts (<= 6 on 0.64 ha), never double counts, keeps source tree ID
PASS [jay] JW-T07 below cap: delivered+exported == supplied, no selection draws
PASS [jay] JW-T08 out-of-bounds acorns are exported, never redrawn or clamped, stable across runs
PASS [jay] JW-T09 transport is r = 5 + 195u^2 (5-200 m) with uniform bearing from the keyed stream
PASS [jay] JW-T10 shuffled on-map input gives the identical diversion and transport
PASS [jay] JW-T11 cap rounding is deterministic nearest integer
PASS [jay] JW-T12 delivered acorns get no microsite bias, no browse immunity, no instant seedlings
PASS [jay] JW-T13 invalid inputs are refused: non-oak, duplicate ID, bad boundary, bad year
PASS [jay] JW-T14 Nearby ON with oaks: both caps apply independently (6 diverted + 4 external)
PASS [storm] JW-T20 StormModel 0 is storm-free with zero draws over 1000 years
PASS [storm] JW-T21 StormModel 0 shifts no other stream (jay output identical either way)
PASS [storm] JW-T22 StormModel 1 event years and severities match frozen Unity parity (seed 12345, years 1-1000)
PASS [storm] JW-T23 one woodland-wide event per year: lane split and tree list never change the event or victims
PASS [storm] JW-T24 a second resolution for the same (or earlier) year is refused with zero draws
PASS [storm] JW-T25 direction is 225 +/- 45 (FROM) and fall glyph is (direction + 180) mod 360
PASS [storm] JW-T26 feedback is qualitative Stable/Watch/Exposed with no numeric hazard or p
PASS [storm] JW-T27 victims: windthrow cause, one fallen deadwood record, one canopy opening, matches independent two-pass roll
PASS [storm] JW-T28 shuffled input ordering yields the same victim set (5 shuffles)
PASS [storm] JW-T29 determinism: repeated 1000-year runs give identical results
PASS [storm] JW-T30 non-neutral site/edge and invalid trees are refused (no invented multipliers)
PASS [salvage] JW-T40 no orders: nothing is salvaged automatically (default LeaveAsFallenDeadwood)
PASS [salvage] JW-T41 SellAndExtract/KeepForUse without an approved Work Plan are refused and change nothing
PASS [salvage] JW-T42 SellAndExtract earns revenue at the unchanged price; pricing echoed unchanged
PASS [salvage] JW-T43 KeepForUse yields retained timber and no revenue
PASS [salvage] JW-T44 no duplicate deadwood after extraction (re-order and re-merge both refused)
PASS [salvage] JW-T45 storm victims merge once into the deadwood stock
PASS [salvage] JW-T46 other refusals: explicit leave, non-windthrow target, duplicate orders, unknown id, bad volume
PASS [salvage] JW-T47 dated non-causal teaching note covers Darragh, Eowyn and the 2026 windblow market
PASS [hygiene-parity] JW-T60 keyed roll is bit-identical to frozen SimulationRandom.Roll model 1
PASS [hygiene-parity] JW-T61 vulnerability and BoundedRational failure chance are bit-identical to frozen StormModel 1
PASS [hygiene-parity] JW-T62 stream ids are fixed, versioned and prefix-free against the existing RngSalts families
PASS [hygiene-parity] JW-T63 static hygiene: no file, network, clock, machine-state or culture-dependent API in src/Core/NaturalProcesses
PASS [hygiene-parity] JW-T64 test project is standalone: one Compile glob, zero ProjectReference/PackageReference
---
SUITE hygiene-parity: 5/5 passed, 0 failed
SUITE jay: 17/17 passed, 0 failed
SUITE salvage: 8/8 passed, 0 failed
SUITE storm: 11/11 passed, 0 failed
TOTAL: 41/41 passed, 0 failed
exit=0
```

**Totals:** 41/41 passed, 0 failed; build 0 warnings, 0 errors (warnings-as-errors on); exit code 0.

## 3. Determinism evidence

- Two separate processes produced byte-identical stdout: SHA-256
  `c3ae0982295339b185827d43314e4915f9aa661ba3d4c4a5a9a13ef8a761fb9c` for both runs.
- In-process: JW-T29 (1000-year repeat), JW-T28 (5 shuffles of 400 trees), JW-T10 (5 shuffles of jay input), JW-T08 (40 years run twice).
- **Frozen parity:** `Assets/ForestPrototype/StormModel.cs` + `SimulationRandom.cs` were compiled read-only in `/tmp`
  (SDK 8.0.133, `Mathf` shim) to print vectors. Hard-coded in JW-T60 (15 roll bit patterns), JW-T61 (4 V and 12 p bit
  patterns) and JW-T22 (19 event years/severities for seed 12345, years 1–1000: 28, 39, 108, 194, 219, 227, 229, 283,
  292, 374, 519, 528, 563, 679, 730, 770, 825, 866, 955). All match bit for bit.
- **Mutant checks (temporary copy in `/tmp`, discarded):** recent-opening weight .8 → .81 failed JW-T61; one draw
  added on the OFF path failed JW-T01/T02/T03; clamping jay destinations to the woodland failed JW-T08.

## 4. Fence proof

`git diff --name-only c82e4e75..HEAD` (recorded after commit; the receipt reply repeats it):

```
Docs/Handoffs/MultiRack-Simulation-20261116/jay-windstorms/IMPLEMENTATION-HANDOFF-PART1.md
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/JayAcornDispersal.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/JayAcornTransport.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/NaturalProcessContracts.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/NaturalProcessRandom.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/NaturalProcessText.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/NearbyOakWoodland.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/SalvageAccounting.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/StormAdapter.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/StormVulnerability.cs
Tools/MultiRackSitkaP1Lab/src/Core/NaturalProcesses/WoodlandFootprint.cs
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/Fixtures.cs
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/HygieneAndParityTests.cs
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/JayDispersalTests.cs
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/Program.cs
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/SalvageAccountingTests.cs
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/StormAdapterTests.cs
Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/TestRunner.cs
specs/multi-2d-jay-windstorms-v1/checklists/requirements.md
specs/multi-2d-jay-windstorms-v1/plan.md
specs/multi-2d-jay-windstorms-v1/spec.md
specs/multi-2d-jay-windstorms-v1/tasks.md
```

All 23 paths sit under the four allowed prefixes. The coordination anchor `/media/jer/Files/CCF-Primary` was not modified.

## 5. Decisions and flags for the reviewer

1. **BoundedRational vs `1 − exp(−S·V)` (flag).** The packet locks "D-050 verbatim … BoundedRational" and also
   quotes the exponential design form. The frozen integrated code uses BoundedRational `p = SV/(1+SV)` (cap 0.999999).
   I implemented BoundedRational and proved bit parity (JW-T61). If the owner meant the exponential, that is a new
   StormModel (D-050 rule: material recalibration → StormModel 2), not a Part 1 edit.
2. **Direction convention — RESOLVED by owner, see Repair 1.** The frozen Unity code samples 0–360° *towards*; this task locks `225 ± 45°` *from*. The
   draw and stream ID are the same; with neutral site/edge, direction never affects victims, so victim parity holds.
3. Engineering choices ENG-01..ENG-14 are listed in `spec.md` §4 (diversion selection, stream keys, 1 m source-line
   offset, default West edge, cap rounding, Nearby-needs-Jay refusal, neutral site/edge refusal, year guard, deadwood IDs).

## 6. Residual gaps (not done in Part 1)

- **Not wired** into `AnnualPipeline` / `MultiAreaContinuation`; stream IDs **not registered** in `RngSalts`
  (prefix-freeness checked against a copied list, JW-T62); no ForestMaker option, Save v3 field or `/multi` UI. All
  Part 2, queued behind Playable Milestone 1.
- Local top height, cell light, recent opening (3×3 neighbourhood rule) and recoverable salvage volume are **caller
  inputs**; Part 2 must compute them with the existing engine rules (StormEventArchitecture / RecentThinningAssessment).
- Salvage eligibility by decay class, wind-damage grading, ×1.25 extraction work cost and the €2,500 contractor
  minimum stay in the existing harvest settlement (SalvageDesign §3); Part 1 only passes the price through unchanged.
- Jay rates (10/ha, 6/ha), the 5–200 m kernel and the band thresholds are **gameplay calibration [C]**, not measured
  jay caching or Irish storm hazard. No ecological research claim is made.
- The teaching note's market sentence ("salvage of that windblown timber was part of the 2026 timber market") is
  general context and has not been source-checked against a cited market report; it should be checked before
  player-facing use in Part 2.
- Unity was not run and is not required for this lane. No browser suite or `build/2d-gate.sh` was run (out of scope).

## Repair 1 — restore frozen D-050 storm direction (2026-11-27)

**Owner ruling (flag 2):** the `225 ± 45°` *from* convention is rejected; Part 1 follows the frozen D-050 code literally.

**Change** (parent `7b5c2a07`; the commit containing this section is the Repair 1 commit on
`feat/multi-2d-jay-windstorms-engine-v1`):

- `StormAdapter.cs`: removed `PrevailingFromDegrees` / `DirectionSpreadDegrees`; `DirectionDegrees = u * 360.0` from the
  unchanged `STORM-DIRECTION-v1` draw — uniform [0, 360), the bearing the wind blows **towards** (0 north +Z, 90 east +X),
  as the frozen `StormModel.cs` comment states. `FallGlyphOrientation(d) = d` (no +180, no per-tree ±20° jitter); a UI
  convention only, never a persisted per-tree bearing.
- `NaturalProcessContracts.cs` / `NaturalProcessText.cs`: doc and player text now say "blows towards"; glyphs point in
  the storm's blow-towards bearing; display convention only.
- `spec.md` rules 5 and 9 and the direction note rewritten to record the ruling; `checklists/requirements.md` JW-REQ-13
  and `tasks.md` JW-T08 updated to match (they restated the rejected formula). BoundedRational and its note unchanged.
- `StormAdapterTests.cs` JW-T25: asserts `direction == draw * 360`, range [0, 360), glyph == direction, and that the
  sampled directions over years 1–1000 include at least one < 180 and one > 270. No other test changed; JW-T61 intact.

Victims are unaffected: site hazard and edge facing stay neutral under StormModel 1, so direction never enters the
failure chance. Note: the frozen Unity code multiplies in `float` (`Roll * 360f`); Part 1 multiplies the same float draw
by `360.0` in `double`, as the repair instruction specifies. Reported bearings may differ from Unity in the last float
bits; Part 2 should use the float product if bit-identical persisted bearings are required.

**Verification (SDK 8.0.133, `Tools/MultiRackSitkaP1Lab`):**

```
dotnet build tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-restore   -> 0 Warning(s), 0 Error(s)
dotnet run --project tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj --no-build --no-restore
PASS [storm] JW-T25 direction is u * 360 (blow-towards, D-050) and fall glyph equals direction
PASS [hygiene-parity] JW-T61 vulnerability and BoundedRational failure chance are bit-identical to frozen StormModel 1
TOTAL: 41/41 passed, 0 failed   (exit 0)
```

Unity not run.
