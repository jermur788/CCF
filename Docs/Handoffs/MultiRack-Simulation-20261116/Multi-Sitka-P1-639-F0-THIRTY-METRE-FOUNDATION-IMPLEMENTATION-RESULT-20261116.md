# Multi P1-F0 #639 — 30 m foundation, stage 1 result (manifest and trust candidates for review)

**Date:** 16 November 2026. **Status: IMPLEMENTED (stage 1 scaffold) — NOT REVIEWED, NOT VERIFIED, NOT ACCEPTED.**
**No ecological year has run or can run.** The derived 30 m manifest and the original-P0 trust records are **candidates
pending independent review and Manager acceptance**, as the packet's hard gate requires.

> SYNTHETIC CLOSED ADULT SITKA. NOT AN IRISH FIELD FORECAST. NOT UNITY NUMERICAL OR GEOMETRY PARITY.

## 1. Worker, lane and source

| Item | Value |
|---|---|
| Worker | Claude Code, Anthropic `claude-opus-5-5` (Opus 5.5), effort low |
| Host | `jer-laptop`, native Ubuntu 24.04 Linux x64 (not WSL2), local ext4 |
| Worktree | `/home/jer/ccf-worktrees/multi-p1-639-f0-30m` (exclusive, per the #425 board reservation on main `5cefe724`) |
| Branch | `feature/multi-p1-639-f0-30m-foundation-20261116-v1` |
| Starting HEAD | `90583811c4fb88508a4bf05c14365732e24a21ac` |
| Source commit | `21bf97b80f182ed53ba8793dc8932ec04b5eaec5` (24 files, all under `Tools/MultiRackSitkaP1Lab/`) |
| Claims | [BLOCKED preflight](https://github.com/jermur788/CCF/issues/639#issuecomment-6926637070), [RUNNING receipt](https://github.com/jermur788/CCF/issues/639#issuecomment-6926938542) |
| P0 pin | #589 commit `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, subtree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| Toolchain | .NET SDK 8.0.133 (P1-owned `global.json`) |

Files changed outside `Tools/MultiRackSitkaP1Lab/**`: **none** besides this result document. `Assets/`, `Packages/`,
`ProjectSettings/`, `Docs/Project/`, `specs/`, P0 and existing saves are unchanged. No Unity process, game build or
Windows build was run.

## 2. What was built

| Area | Outcome |
|---|---|
| Sibling app | `Tools/MultiRackSitkaP1Lab/` with its own `global.json`, `Directory.Build.props` (stops MSBuild's upward search; nothing is inherited from the root or P0), `.gitignore` (re-includes `*.csproj`/`*.sln`; ignores `.p1-data/`, `.p0ref/`, `bin/`, `obj/`), solution, Core library, CLI and tests |
| Domain tags | `DerivedP1ThirtyMetreExperimentV1`, `DerivedUnitySquareParityV1`, `OriginalPolygonExploratoryV1`: exact, case-sensitive and never inferred. Each loader accepts only its own domain |
| Derived manifest | Format `ccf-sitka-p1-derived-fixture` v1, suffix `.sitkap1fx.json`, strict canonical JSON, floats as f32 bit strings only. Deterministic generator (§3) |
| Derived catalog | Compiled record with file length, file SHA-256, content hash, geometry and area bits. **Status `PendingIndependentReview`** |
| Original trust table | `p1-f0-original-trust-v1`: three compiled records (§4). **Status `PendingIndependentReview`** |
| Original inspection | Spec §9.2 T-02 order: table lookup → trust status → no-follow, descriptor-anchored open → `statx` regular / 1 link / exact size → bounded read → SHA-256 → P0 `LabFile.Parse` → P0 `CanonicalJson.SnapshotHash` → pristine-original rules. Read-only; `AnnualAdvanceAllowed=false` always |
| Annual gate | Original → `OriginalAnnualRefused`; Unity 40/80 → `DomainUnavailable`; 30 m pending → `ManifestNotAccepted`; even if accepted → `AnnualEngineNotImplemented` |
| P0 reuse | P0 Core is a **binary** reference. `build/build-p0-core-ref.sh` runs `git archive` on the pinned tree, extracts it outside the repo, builds there and hashes the inputs before and after. The DLL is reproducible: SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` on two independent builds, and the P1 build fails on any other hash. Public API only |
| Scenario One reuse | `Assets/ForestPrototype/SimulationRandom.cs` and `SitkaGrowthModel.cs` compile **unchanged** into P1 (pure `System` code); blob pins are checked by a test. Unity-coupled code (`ForestEcologyController`, `TreeSpeciesDefinition`, `ForestTree`) is not linked, and Scenario One was not refactored |

### Not implemented in this stage (UNAVAILABLE, not claimed)

Annual source-rule adapter (CI, DBH, height, crown, density mortality); Host/UI on a P1 port; `.p1-data/` root, saves,
fsync and post-rename outcomes; work plans, approval, felling, event log and replay; legal 40/80 m catalog; P1-POLY.
"No felling on approval alone", split-run replay, command-ID dedup and Host origin/CSRF tests are therefore **NOT RUN**.

## 3. Candidate 30 × 30 m starting-forest manifest — FOR INDEPENDENT REVIEW

File `Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`:
32,567 bytes, file SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, content hash
`b7dee8a34b4e66b4cc443b533604b5962f5fe4c1d2f28832270a0c2b02ba6627`. Derived catalog digest
`c507763dc779eae37af8df10eb60690b3f6bcc2048867dbe1b1eaf4463c66646`. Two generator runs produce identical bytes.

**CCF source facts** (cited from source pinned at `90583811`) versus **synthetic P1 choices**:

| Field | Value | Class |
|---|---|---|
| Domain / ID / catalog | `DerivedP1ThirtyMetreExperimentV1` / `p1d-30m-sitka-adult-v1` / `p1-f0-derived-catalog-v1` | P1 identity |
| Derivation | `designed`, generator `p1-f0-square30-generator` v1, `originalP0FixtureId=null`. Not a rescale or trim of P0 | Synthetic |
| Local frame | Integer mm; origin (0,0) at the south-west corner; +X east, +Y north; trees in [0,30000)² | Synthetic declaration |
| XZ transform | `p1-square30-corner-to-centred-xz-v1`: `UnityX=(float)((xMm−15000)/1000.0)`, same for Z from Y, `UnityY=0`; int64 subtraction, then binary64 division, then one f32 cast | Synthetic declaration (not Unity geometry) |
| Area | Exactly 900 m²; `(float)(900.0/10000.0)` = f32 bits `3db851ec` (0.0900000035762786865234375 ha). Not the Unity `standSizeMeters²` path | Declared |
| Ecological year / seed | `initialEcologicalYear=0`, first advance year 1, `sourceInventoryYear=null`, `simulationSeed=20261116` | Synthetic |
| Versions | Growth model 2, RNG model 1 (`SimulationRandom.MixedModel`), `p1-f0-adult-sitka-30m-v1` | Source target / P1 |
| Site / stage / species / demand / pruning | Site 1.0f (`3f800000`), MatureEquivalent, Sitka, Reference, 0 | Source fact (site=1) / synthetic declaration |
| Excluded systems | browsing, canopyLight, juvenileLifecycle, mixedSpecies, pruning, recruitment, regeneration, storm | Declared |
| Trees | 100 living stems, IDs `P30-001`…`P30-100`, row-major from the south row; 10 × 10 grid at 3 m pitch from 1.5 m, integer jitter ±600 mm per axis; all inside [900, 29100] mm; positions distinct | Synthetic |
| Age | 30 years for every stem (even-aged choice; source Class III height curve is anchored at 20.4 m at age 30) | Synthetic |
| DBH | 240 + trunc((S−2000)·90/1000) mm, S = sum of four integer uniforms in [0,1000], clamped to [100,400]. Observed 131–349 mm | Synthetic |
| Height | 20400 + (DBH−240)·30 mm (linear size–height choice, not a fitted curve) | Synthetic |
| Initial crown | `MathF.Max(0.1f, 0.9415f + 0.07635f·dbhCm)` in .NET float32: source linear crown potential at CI=0 (`SitkaSpruce.asset`, `usePowerLawCrown: 0`) | Synthetic initialisation using source coefficients; not a Unity observation |
| Action threshold | 170 mm, a label only: 88 actionable, **12 below threshold, still in the biological population** | Synthetic |
| Generator RNG | SplitMix64, integer only, `generatorSeed=6390001`, kept separate from the source mortality RNG and seed | Synthetic |

Indicative density (reviewer arithmetic in binary64 Python, **not a source or P1 engine run**): 1,111 stems/ha,
Dq ≈ 25.30 cm, RD ≈ 0.610. The stand therefore starts **just above** the 0.6 density-pressure onset. This is a deliberate
experimental starting point to be confirmed or changed by the reviewer and Manager; it is not a forestry claim.

## 4. Candidate original-P0 trust records — FOR INDEPENDENT REVIEW

Prepared by this worker from `git cat-file blob a7f41983…:Tools/MultiRackSitkaLab/fixtures/synthetic/<file>`; **not**
copied from `MANIFEST.sha256`. Each snapshot hash was recomputed with P0's own `CanonicalJson.SnapshotHash` and matches.
Trust table version `p1-f0-original-trust-v1`, table digest `38572effa59bbc6df09479ab8de6339b4567b27d8f61ad578b51fe5d5264c5ba`.

| fixtureId | Bytes | File SHA-256 | P0 snapshot hash | Live / dead / live below threshold | Polygon area |
|---|---|---|---|---|---|
| `oracle-quarter-ha` | 969 | `b1691934fa4a41cfd63462f4021811e0871742f0e1f23ecea77fe8da40b5cf62` | `810030c4187c3132cafb22d46ff87c0b8dbfa43bc382a72463c686f8c554152c` | 3 / 0 / 0 | 2500 m² |
| `sitka-flat-demo-v1` | 26675 | `d0eed68979b2d214555e84a59e80ff6866ccd5e7d1774883e7898fcf8fb4f2ed` | `b407c725e655dc80ca5fec22f4ad84540136e89928577ad97b3dce31b234f5b1` | 153 / 9 / 15 | 5812.5 m² (5 vertices) |
| `sitka-flat-perf-2000-v1` | 324929 | `d12db11126de2843ff21538f26feaab09242ccaf6abd04e3f25c8014054790df` | `e4cebf0206c23977abc4b08857d706bdb1c110aa853b08241997081ccfde8726` | 1893 / 107 / 182 | 20000 m² |

The live/dead/below-threshold counts match the spec §5.1 m-8 figures, and the exact shoelace areas match the spec's
original footprints (0.25 / 0.58125 / 2 ha).

### Reviewer checklist (proposed)

1. Recompute the three file hashes and lengths from `a7f41983` blobs, and the snapshot hashes with P0's own code.
2. Regenerate the manifest (`sitkap1 generate-manifest`) and compare bytes; read the generator rules in
   `src/Core/ManifestGenerator.cs`.
3. Judge the synthetic choices in §3 (count, layout, even age 30, DBH/height rules, starting RD ≈ 0.61, threshold 170 mm,
   seed, crown initialisation, XZ transform, f32 area) and the source/synthetic classification.
4. Review `NoFollowRead.cs` and the `FixtureAccess.InspectOriginal` order against spec §9.2 T-02.
5. On acceptance, a **separate reviewed commit** changes the records to `Accepted` and cites the review and Manager
   references. The author must not make that change on their own approval.

## 5. Tests and evidence

Durable evidence: `/media/jer/Files/CCF/Verification/CCF-639-P1-F0-20261116/isolation-run-1/` (checksums in
`../isolation-run-1.SHA256SUMS`). It covers a cold run: P0 Core reference rebuilt, P1 built, tests and CLI run under
`strace -f -y`.

| Check | Result |
|---|---|
| Targeted tests (`tests/Core.Tests`) | **27/27 PASS**: domain tags; f32 bits including NaN, infinity and signed zero; strict JSON; manifest determinism and committed-file equality; about 50 manifest negatives (cross-domain tag, seed, area bits, origin, axis, transform, geometry model, versions, inventory year, disclosure, provenance, ID order and case, status, bounds, derived bits, kind, count, unknown/missing/duplicate key, whitespace, key order, trailing byte, content hash); derived catalog pending and mismatch; original trust values from the pin; pending trust refuses before opening any file; 3 fixtures inspected read-only; derived and unknown IDs refused; altered byte, appended whitespace and a consistent fixture+`MANIFEST.sha256` forgery refused; symlinked file, symlinked directory, hard link, FIFO, directory and missing file refused; canonical but non-pristine P0 (step, mark, felled) refused; annual gate for all domains; source pin identity; `SimulationRandom.Roll` and E(30) function probes |
| Mutation check | 4/4 mutants killed (annual gate ignoring review status; hard-link check removed; file SHA check removed; pending trust accepted). Sources restored byte-identical |
| CLI | `verify-derived` OK (labelled untrusted); `inspect-original` refuses all 3 normally (`OriginalTrustNotAccepted`) and inspects with `--review-evidence`; `annual` refuses all 3 domains |
| L0 history | PASS: no commit in `a7f41983..21bf97b8` touches `Tools/MultiRackSitkaLab` |
| L1 tree | PASS: `HEAD:Tools/MultiRackSitkaLab` = `f212c8f6…` |
| L2 status | PASS: empty before and after (re-checked after commit) |
| L3 ignored snapshot | PASS: 0 ignored files before and after. Extra check: SHA-256 of all 52 tracked P0 files unchanged |
| L4 write trace | PASS: 0 write-intent or mutating calls on the P0 tree. 22 P0 calls, all `O_RDONLY`, among 9,310 traced file calls |
| Mechanism (a) input check | PASS: extracted P0 inputs hash-identical before and after the external build; no `bin/` or `obj/` beside them |

Named build-input tree for L2–L4: the in-repo `Tools/MultiRackSitkaLab` of this worktree. The P0 Core build input is
the external `git archive` extraction, covered by the script's own before/after hashes. L4 limitation: syscalls not in
the traced set, and writes by processes outside the traced tree, are not observed.

## 6. Not run / not claimed

N01–N13 as whole groups remain **UNRUN**; this stage covers only the pieces listed above. Not run: Unity (no C-6 lease),
C-2b numerical parity, the annual step, Host/port/origin/CSRF, saves/fsync/durability, replay, work/felling semantics,
browser/UI, Windows Player build, WSL2 (this host is native Ubuntu), arm64, and Dashboard/Drive. Function-level
`Roll`/`SiteTopHeightM` probes are not Unity observations. The 72 requirement boxes are not ticked by this worker.

## 7. Requested next step

Independent non-author review (not Claude Opus 5.5) of §3 and §4 plus the security-relevant code (`NoFollowRead.cs`,
`FixtureAccess.cs`, `DerivedManifest.cs`), then a Manager decision. Only after acceptance may a separately claimed
step change the records to `Accepted` and start the annual source-rule adapter. #639 stays **OPEN** for Manager
disposition.
