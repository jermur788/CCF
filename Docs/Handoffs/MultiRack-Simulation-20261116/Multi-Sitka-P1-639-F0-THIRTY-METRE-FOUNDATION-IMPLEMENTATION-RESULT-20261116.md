# Multi P1-F0 #639 — 30 m foundation, stage 1 result (manifest and trust candidates for review)

**Date:** 16 November 2026. **Status: IMPLEMENTED (stage 1 scaffold) — NOT REVIEWED, NOT VERIFIED, NOT ACCEPTED.**
**No ecological year has run or can run.** The derived 30 m manifest and the original-P0 trust records are **candidates
pending independent review and Manager acceptance**, as the packet's hard gate requires.

> SYNTHETIC CLOSED ADULT SITKA. NOT AN IRISH FIELD FORECAST. NOT UNITY NUMERICAL OR GEOMETRY PARITY.

**Update, #644 (16 November 2026):** the #643 file-safety findings B-01, M-01 and M-02, and the minors m-01 and m-02,
are repaired in source commit `24805388`; see **§8**. Status: IMPLEMENTED, awaiting independent delta review. The
manifest, catalog and trust records are unchanged and still pending. **NO ECOLOGICAL YEAR RUN.**

**Update, #649 (16 November 2026):** the L4 analyzer now fails closed on undecoded mutating descriptors (source
`9fc3a88b`). Its self-tests pass, but the native cold run surfaced 38 UNRESOLVED .NET runtime kernel-object descriptors.
**#649 is BLOCKED pending a Manager ruling** (see **§9**). Fixture and trust records are unchanged and still pending.

**Update, #651 (16 November 2026):** Manager Option A was carried out as a verification-only comparison (see **§10**),
with no repository source change.
- **Default runtime:** L4 still **FAIL**, 38 UNRESOLVED (preserved).
- **Verification-only switched runtime:** L4 PASS within the traced scope.
- All P1 outputs, refusals, binaries and P0 state **MATCH** between the two.
- **NO ECOLOGICAL YEAR RUN.**

**Erratum, #658 (16 November 2026):** §10 contained a counting error in its thread-name row (now +1, not +2). The four
evidence folders also had checksum-manifest packaging defects; new checksum sidecars were added alongside the preserved
originals. See **§10.5**.

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

## 8. #644 bounded file-safety repair (after #643 NEEDS CHANGES)

**Status: IMPLEMENTED — NOT REVIEWED, NOT ACCEPTED. NO ECOLOGICAL YEAR RUN.** The result is recorded here, in the
already-reserved #639 document, because the packet's new path
`Multi-2D-SITKA-P1-644-F0-FILE-SAFETY-REPAIR-RESULT-20261116.md` is not reserved on the live board (main `eaa245c9`).
That is the packet's stated fallback.

| Item | Value |
|---|---|
| Packet | [#644 bounded repair](https://github.com/jermur788/CCF/blob/docs/multi-p1-644-f0-security-repair-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-644-643-FILE-SAFETY-BOUNDED-REPAIR-PACKET-20261116.md); review [#643](https://github.com/jermur788/CCF/blob/review/multi-p1-643-manifest-p0-trust-file-safety-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-643-INDEPENDENT-30M-MANIFEST-P0-TRUST-FILE-SAFETY-RESULT-20261116.md) |
| Worker / host / lane | Same as §1: Claude Code `claude-opus-5-5`, effort low; `jer-laptop` native Ubuntu 24.04 x64; existing worktree and branch |
| Claim | [#644 RUNNING](https://github.com/jermur788/CCF/issues/644#issuecomment-6928456814), posted before any edit |
| Base → source commit | `0636ddc3` → `24805388e88cf9463ff89eb1c1c9ced18660953b` (11 files, all under `Tools/MultiRackSitkaP1Lab/`; +618/−31) |

### 8.1 Repairs, path by path

| Finding | Change |
|---|---|
| **B-01** `generate-manifest` destination | `src/Cli/Program.cs`: the `<out>` path form is removed (it now gives a usage refusal, exit 64). `generate-manifest --stdout` writes only to an inherited stdout that `NoFollowRead.RequireStreamDescriptor` (statx on fd 1) confirms is a pipe, socket or terminal. A regular file or anything else is refused with `OutputNotStream` before any byte is written. `generate-manifest --check <dir>` compares the generator output with the committed fixture through the no-follow reader. **P1 production source now has no file-writing or path-based file API**; a unit test scans `src/` and fails if one appears (`File.Write*/Create/Open/Append/Copy/Move/Delete/ReadAll*`, `FileStream`, `Directory.Create/Delete/Move`, `StreamWriter`). Remaining caller responsibility: a user's own shell or `tee` writing P1's stdout somewhere is outside P1 |
| **M-01** `.p0ref` symlink redirect | `build/build-p0-core-ref.sh` now checks, before any build work and again just before installing:<br>• the P1 root is the real, non-symlinked `Tools/MultiRackSitkaP1Lab` and not inside P0;<br>• `.p0ref` is not a symlink (live or dangling) or a non-directory, is owned by the current user and resolves to itself;<br>• each of the three output names is absent or a regular, single-link, non-symlink file.<br>Outputs are first prepared in the external work dir. Each is then installed by `mktemp` (O_CREAT\|O_EXCL) inside `.p0ref` and `mv -fT` (`rename(2)`), which replaces the name and never writes through a symlink or into a hard-linked inode. The `.p0ref` dev:inode is re-verified after installing. The `git archive` → external build → input-hash check and the DLL SHA `9a99f781…` are unchanged. Residual: a same-user race between check and rename (local script; documented in the README) |
| **M-02** `verify-derived` read | `FixtureAccess.ReadDerived(dir, id, use)` checks ID prefix, catalog record, domain and review status **before opening**. It then uses `NoFollowRead.ReadExact` with the catalog's file name and exact length: no-follow on every path component, statx on the open descriptor (regular, 1 link, exact size), and a bounded read of at most length+1. Only then come SHA-256, strict canonical parse, domain and content hash. The CLI now takes a directory and never a caller file name. Refusal codes are unchanged |
| **m-01** duplicate XY | `DerivedManifest.Parse` refuses two stems at the same `(xMm, yMm)` (`DuplicatePosition`). The committed fixture still parses |
| **m-02** L4 scope | New `tests/l4_analyze.py` replaces the text grep:<br>• tracks each traced process's working directory (`chdir`, `fchdir`, fork/clone inheritance) and resolves legacy relative paths, `*at` dirfds, fd-based mutators and the kernel-resolved path of every write-intent open;<br>• counts any unresolvable relative mutating call as UNRESOLVED, which fails the check;<br>• the traced syscall set is widened (adds `rmdir`, `mknod*`, `ftruncate`, `fallocate`, `fchmod`, `fchown`, `lchown`, `utime*`, `*xattr`, `open_by_handle_at`).<br>`p0-isolation.sh` first runs a **decoy self-test** and only then the cold run. The PASS wording is limited to the traced scope |
| **m-03** density | Deferred as instructed; generator and data unchanged |

A probe on this host showed the reason for m-02: `strace -y` decodes `AT_FDCWD` for `*at` calls but **not** for legacy
calls. On the decoy, the old grep filter caught 9 of 13 writes; it missed three relative `mkdir`s and an `ftruncate`.
The analyzer catches 13 of 13.

### 8.2 Frozen values (verified unchanged against `0636ddc3`)

`git diff --quiet 0636ddc3` is clean for the fixture (still SHA-256 `bcaaa5c3…f757c4`, 32,567 bytes), `TrustTables.cs`
(all four records still `PendingIndependentReview, null, null`), `ManifestGenerator.cs` and `CcfSourcePins.cs`.
`generate-manifest --check` and `--stdout | sha256sum` both reproduce `bcaaa5c3…`. No acceptance reference was added.

### 8.3 Verification on Ubuntu (this host)

Durable evidence: `/media/jer/Files/CCF/Verification/CCF-644-P1-F0-FileSafety-20261116/` (with `SHA256SUMS`).

| Check | Command | Result |
|---|---|---|
| Unit tests | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **33/33 PASS** (27 original + 6 new: duplicate XY; ReadDerived pending/ID; symlink leaf, symlinked ancestor, hard link, FIFO, directory, 1 MiB oversized, undersized, missing, file-as-directory; same-size altered bytes and wrong-domain data under a test catalog → `DomainMismatch`; stream-descriptor guard on a regular file vs a pipe; no-file-API source scan) |
| Mutants | Backup → mutate → test → restore | **3/3 new mutants caught** (duplicate check off; stream guard off; ReadDerived back to `File.ReadAllBytes`), plus the 4 from stage 1. Sources restored byte-identical |
| Builder, hostile root (M-01) | `tests/build-output-safety.sh` | **8/8 PASS**: C1 `.p0ref`→scratch directory at a `Tools/MultiRackSitkaLab` path, C2 dangling symlink, C3 regular file, C4 output-name symlink, C5 output-name hard link, C6 output-name directory → all **exit 4, victim unchanged**; C7 cold build and C8 rebuild → exit 0, DLL `9a99f781…`, 3 regular single-link files, no temp leftovers. P0 digest and status unchanged |
| Same test on the **pre-repair** builder | `0636ddc3` script swapped in temporarily, then restored | **FAIL as expected**: C1 exit 0 with three files written into the scratch stand-in "P0"; C4 wrote through the symlink. Reproduces #643 M-01 on stand-ins only |
| CLI (B-01/M-02) | `tests/cli-safety.sh` | **23/23 PASS**:<br>• `--stdout` pipe hash equals the committed fixture;<br>• `--stdout >>` onto a stand-in P0 fixture and onto a new file → exit 2 `OutputNotStream`, stand-in unchanged, new file 0 bytes;<br>• old path form (stand-in, `..` traversal, new name) and extra argument → exit 64, nothing created;<br>• `--check` → 0;<br>• `verify-derived`: good dir 0; file-path form, symlink leaf, symlinked ancestor, hard link, FIFO (no hang, 30 s timeout), directory, 64 MiB oversized, wrong-domain bytes, original ID and missing → exit 2 with the expected code;<br>• P0 digest unchanged |
| Cold L0–L4 | `tests/p0-isolation.sh` | **OVERALL PASS**:<br>• L0, L1 PASS;<br>• L2 empty before and after;<br>• L3 0 ignored files unchanged; L3+ all 52 tracked P0 files SHA-identical;<br>• **L4 self-test 13/13, read control 0**;<br>• **L4: 1,086 mutating calls resolved, 0 P0 hits, 0 unresolved** (10,968 trace lines);<br>• 33/33 tests; CLI `annual` refuses all three domains; normal `inspect-original` refuses all three pending records |

Notes on the evidence:
- The cold run and the hostile-root and CLI suites ran on the working tree that was then committed as `24805388`. The
  only difference is README wording, edited during the cold run; it has no effect on the build.
- L0/L1/L2 hold at the committed head.
- The P0-isolation run uses the in-repo `Tools/MultiRackSitkaLab` as the named build-input tree; P0 Core itself is built
  from the external `git archive` extraction with before/after input hashes.

### 8.4 Not run, limits and residual risks

- **Not exercised in the decoy:** deletion syscalls (`unlink`, `rmdir`). This session's command guard blocks scripted
  removals, so they are covered only by the analyzer's shared code path, not by an executed decoy case.
- **L4 limits:** syscalls outside the traced set (for example io_uring), processes outside the traced tree, and
  symlinked intermediate components in legacy relative paths are not observed. L2/L3/L3+ are the independent content
  and metadata checks. A PASS is evidence for this run only, not proof that every write is impossible.
- **Builder:** a same-user race between the checks and `rename(2)` remains.
- **CLI refusal code:** `verify-derived` given a file path reports `DirectorySymlinkRefused`, whose message says
  "symlink or not a directory"; the code name is broader than its meaning.
- **Not run:** Host, saves, replay, annual engine, Unity, C-2b, WSL2, the Windows build, and any acceptance-status change.

### 8.5 Requested next step

A narrow, independent changed-lines and security delta check (ideally reviewer #643) of `0636ddc3..24805388` against
B-01, M-01, M-02, m-01 and m-02, then a Manager decision. #639 and #644 stay open. The trust and catalog records stay
`PendingIndependentReview` until a separate reviewed acceptance commit.


## 9. #649 L4 undecoded-descriptor fail-closed repair (after #646 NEEDS CHANGES)

**Status: REPAIR IMPLEMENTED; NATIVE L4 RUN **BLOCKED** (fails closed on 38 genuine non-filesystem descriptors);
awaiting a Manager ruling. NOT REVIEWED. NO ECOLOGICAL YEAR RUN.**

| Item | Value |
|---|---|
| Packet / review | [#649 packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-649-l4-undecoded-fd-repair-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-649-646-UNDECODED-FD-TRACE-FAIL-CLOSED-REPAIR-PACKET-20261116.md); [#646 review](https://github.com/jermur788/CCF/blob/review/multi-p1-646-644-file-safety-delta-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-646-INDEPENDENT-644-FILE-SAFETY-DELTA-RESULT-20261116.md) |
| Worker / host / lane | Claude Code `claude-opus-5-5`, effort low; `jer-laptop` native Ubuntu 24.04 x64; existing worktree and branch |
| Claim | [#649 RUNNING](https://github.com/jermur788/CCF/issues/649#issuecomment-6929702579), posted before any edit |
| Base → source commit | `33c05069` → `9fc3a88b378b0cfff55d87afdd74f28a2dbf411c`. Only `tests/l4_analyze.py` and `tests/p0-isolation.sh` changed (+331/−121) |
| Unchanged (`git diff --quiet 33c05069`) | `fixtures/derived/…sitkap1fx.json` (SHA `bcaaa5c3…`), all of `src/` (incl. `TrustTables.cs`, 4 records `PendingIndependentReview`), `build/`, and the P0 tree `f212c8f6` |

### 9.1 The repair

The #646 MAJOR was that the pre-repair regex silently dropped bare descriptors (`ftruncate(9, 0) = 0` gave no target and
no "unknown" flag, so the run PASSED). `tests/l4_analyze.py` now uses an **explicit argument model per traced mutator**
(`ARG_SPEC`) and a top-level argument splitter:

| Argument kind | Rule |
|---|---|
| fd-only mutators (`ftruncate`, `fallocate`, `fchmod`, `fchown`, `fsetxattr`, `fremovexattr`, `utimensat(fd, NULL, …)`) | Target is the absolute path strace `-y` decodes for the descriptor. A **bare** descriptor (`9`) or a **non-path** decoding (`9<pipe:[…]>`, `8</memfd:…>(deleted)`, `9<socket:[…]>`) → **UNRESOLVED** |
| `*at` calls | Each path string is paired with the dirfd argument immediately before it. A relative path needs that dirfd decoded to an absolute path, or bare `AT_FDCWD` with a known tracked cwd; otherwise UNRESOLVED |
| Legacy path calls | A relative path is joined to the process's tracked cwd; an unknown cwd → UNRESOLVED |
| Write-intent opens | The returned descriptor's decoded path is added (catches routes through symlinks) |
| Catch-all | Any **successful** traced mutator with no reliable pathname → UNRESOLVED |
| Failed calls (`= -1 …`) | Not counted as writes. If aimed at the protected tree → `FAILED_PROTECTED_ATTEMPT` (fatal, reported separately). An outcome-unknown `= ?` is treated as success (fail closed) |

Exit 0 only with no HIT, UNRESOLVED or FAILED_PROTECTED_ATTEMPT. Read-only calls and read-only opens are not mutations.
Existing legacy-cwd and `*at` handling, P0 HIT classification and the declared limitations are kept.

**Self-test, three parts, all wired into `p0-isolation.sh` before the real run:**
1. **Built-in synthetic cases** (`--self-test`): **23/23 PASS**. They cover:
   - the packet's exact cases: bare `ftruncate`, `fallocate` and `fchmod` → UNRESOLVED; `ftruncate(9</scratch/protected/file>)` → HIT; `ftruncate(9</scratch/outside/file>)` → clean; `read(9…)` / `openat(…O_RDONLY)` / `open(…O_RDONLY)` → no mutation;
   - bare `fchown`, `fsetxattr`, `fremovexattr`; a pipe fd; bare `utimensat(fd, NULL)`;
   - relative `*at` with a bare dirfd; a second `rename` operand with a bare dirfd; legacy `mkdir` with an unknown cwd; `open_by_handle_at` with an undecoded return; a mutator with no string path (all UNRESOLVED);
   - a decoded protected dirfd; `chdir` then relative `mkdir`; a write through a symlink (all HIT);
   - a failed protected attempt, a failed write elsewhere, unfinished/resumed bare `ftruncate`, and `= ?`;
   - **two sightings of one operation do not satisfy two expected identities**.
2. **Decoy with unique operation identities:** each of the 13 scripted operations now targets a distinct name (`touch t.txt`, `truncate -s 0 tr.txt`, `chmod r2.txt`, …), giving 13 distinct `(syscall, target)` identities listed in `L4-selftest-expected.txt`. `--expect` requires every identity to match a hit. The count is certified by identity, not by `hits ≥ 13`. Natively: **13/13 identities matched, each sighted exactly once, 0 unresolved.** The only extra identities reported are the source names of the same `mv`/`ln` operations.
3. **Read-only control** in the decoy: clean.

Identities are pinned to this host's coreutils/bash/python syscall choices (Ubuntu 24.04). Deletion calls are still not
in the decoy (this session blocks scripted removals); they use the same rules.

**Demonstration of the defect:** on the same synthetic bare-fd trace, the **pre-repair** analyzer (`33c05069`) exits 0
with "unresolved=0". The repaired analyzer exits 1 with 3 UNRESOLVED. The decoded-protected and benign traces give exit
1 (HIT) and exit 0 respectively.

### 9.2 Native cold L0–L4 run — BLOCKED diagnostic (preserved, not whitelisted)

`tests/p0-isolation.sh` on the committed analyzer:

| Layer | Result |
|---|---|
| L0, L1 | PASS (no P0 commit since `a7f41983`; tree `f212c8f6`) |
| L2 | PASS: P0 status empty before and after |
| L3 / L3+ | PASS: 0 ignored files; all **52 tracked P0 files SHA-identical** |
| L4 self-test | **PASS**: synthetic 23/23; decoy 13/13 distinct identities, 0 unresolved; read control clean |
| **L4 real run** | **FAIL (fail-closed)**: 947 successful mutating calls, 169 failed calls, **0 P0 hits**, **0 failed protected attempts**, **38 UNRESOLVED** |
| P1 tests / CLI | 33/33; `generate-manifest --check` and `--stdout` reproduce `bcaaa5c3…`; `annual` refuses all three domains |

**All 38 UNRESOLVED calls are of exactly two kinds, one of each per `dotnet` process start (19 processes):**

| Count | Raw example | Origin (verified on this host) |
|---|---|---|
| 19 | `ftruncate(8</memfd:doublemapper>(deleted), 2199023255552) = 0` | `memfd_create("doublemapper", MFD_CLOEXEC)`: the .NET runtime's anonymous in-memory W^X double-mapping. Not a filesystem file |
| 19 | `fchmod(9<socket:[1401775]>, 0600) = 0` | The .NET diagnostics IPC socket, followed by `bind` to `/tmp/dotnet-diagnostic-<pid>-…-socket` (outside P0) |

Read-only probe (`dotnet sitkap1.dll annual …`, traced with `memfd_create,ftruncate,fchmod,bind`):
- default settings: memfd 1, socket `fchmod` 1;
- `DOTNET_EnableWriteXorExecute=0`: memfd 0, socket 1;
- `DOTNET_EnableDiagnostics=0`: memfd 1, socket 0.

Every other `ftruncate`/`fchmod` in the trace (192 in total) decodes to a real path (NuGet cache, the external P0-Core
build directory, P1 `obj/`) and resolves outside P0. **The pre-repair #644 run contained the same 19 `memfd` calls and
passed them silently**, which confirms the #646 finding on native data.

Because the packet says to "preserve that diagnostic and report [BLOCKED] rather than whitelist/ignore them to green",
I have **not** changed any classification or runtime setting to get L4 green.

### 9.3 Options for the Manager (choose one; each then needs the narrow independent recheck)

| Option | Change | Trade-off |
|---|---|---|
| **A — runtime switches in the traced run** (recommended) | `p0-isolation.sh` runs the traced command with `DOTNET_EnableWriteXorExecute=0` and `DOTNET_EnableDiagnostics=0`. The analyzer stays fully strict | No analyzer relaxation; probe shows both sources disappear. Cost: the traced runtime differs from the default configuration (JIT memory-mapping mode and the diagnostics endpoint). I don't expect either switch to change P1's file I/O, but that is an assumption, not verified |
| B — narrow, provenance-bound classification | Add `memfd_create` and `socket` to the traced set. Classify an fd-only mutation as NON_FILESYSTEM only when that exact pid/fd was returned by a traced `memfd_create`/`socket` and not since reassigned. Report these separately; bare and other undecoded fds stay UNRESOLVED | Keeps default runtime; more analyzer logic to review; still a relaxation of the fail-closed rule |
| C — keep strict, accept a known L4 FAIL | No change. L4 reported as "FAIL: 38 non-filesystem runtime descriptors, 0 P0 hits"; L2/L3/L3+ remain the P0 integrity gates | No new code; L4 cannot be green on .NET |

### 9.4 Limits

- L4 claims cover only the traced syscall set in the traced process tree for this run.
- Legacy relative paths are joined lexically.
- A `chdir` by one thread is applied to that thread id only.
- Deletion calls are not exercised.
- The builder's same-user race (§8.4) is unchanged and out of scope.
- The decoy identity list is host-specific.

Evidence: `/media/jer/Files/CCF/Verification/CCF-649-P1-F0-L4-FailClosed-20261116/` (with `SHA256SUMS`): cold run,
synthetic traces with pre/post-repair outputs, decoy pre-check, runtime probe traces, self-test log. The copied decoy
keeps its two recorded symlinks, which point into scratch.

**Next:** Manager #641 chooses A, B or C. A or B is then a small follow-up in this lane, followed by the narrow
independent recheck. #639, #644 and #649 stay open; no record is promoted.

## 10. #651 controlled .NET runtime trace comparison (Manager Option A, verification-only)

**Status: VERIFICATION COMPLETED, NOT REVIEWED. NO ECOLOGICAL YEAR RUN. No repository source or test change in #651.**

| Default runtime (production-like) | Verification-only switched runtime |
|---|---|
| **L4 FAIL**: 0 P0 hits, 0 failed protected attempts, **38 UNRESOLVED** (19 `ftruncate` on `memfd:doublemapper`, 19 `fchmod` on 19 distinct diagnostics sockets) | **L4 PASS (traced scope only)**: 0 P0 hits, 0 failed protected attempts, **0 UNRESOLVED** |

The switched result is **conditional on** `DOTNET_EnableWriteXorExecute=0` and `DOTNET_EnableDiagnostics=0`. These
are verification settings only. They are **not** a P1 launch default, not a recommended production configuration, and not
evidence about the default runtime. The default-runtime L4 FAIL stands as recorded.

| Item | Value |
|---|---|
| Packet / claim | [#651 packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-651-controlled-runtime-trace-option-a-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-651-CONTROLLED-DOTNET-TRACE-OPTION-A-VERIFICATION-PACKET-20261116.md); [#651 RUNNING](https://github.com/jermur788/CCF/issues/651#issuecomment-6930412142), posted before any action |
| Worker / host / lane | Claude Code `claude-opus-5-5`, effort low; `jer-laptop` native Ubuntu 24.04 x64; existing worktree and branch |
| Candidate | `342cce6d8db16d8ec0acb11d0a5d7805b5541cc4`. Tracked changes before and after: 0. The only commit added by #651 is this §10 |
| Frozen identities (both runs, `inputs.txt`) | Fixture SHA-256 `bcaaa5c3…f757c4`; `TrustTables.cs` blob `2ecc1a71…` (four `PendingIndependentReview` records); analyzer SHA-256 `35c549f7…`, **identical to `9fc3a88b`**; harness SHA-256 `b2b990a3…`; P0 tree `f212c8f6…` |

### 10.1 Method (no repository change)

An **external** wrapper (`run651-wrapper.sh`, SHA-256 `cb6f34ad…`, kept in the evidence folder and not in the
repository) ran the **unchanged** harness `tests/p0-isolation.sh`, with the **unchanged** strict
`tests/l4_analyze.py`, twice and sequentially on the same head. Each run was a cold build plus tests plus CLI under
`strace`:

- **A (default):** `env -u DOTNET_EnableWriteXorExecute -u DOTNET_EnableDiagnostics tests/p0-isolation.sh …`
- **B (verification-only):** `env -u … DOTNET_EnableWriteXorExecute=0 DOTNET_EnableDiagnostics=0 tests/p0-isolation.sh …`

**Scope of the switches.**
- They are set only in run B's subprocess environment, via `env`.
- That run's only .NET processes are the 19 started inside the traced command. The rest of the harness is git, stat, bash and python, which ignore them.
- The wrapper's own shell had **0** switch lines before and after (`scope-check.txt`).
- `dotnet-env-seen-by-subprocess.txt` records the exact `DOTNET_*` variables per run. The only other one present is the user's pre-existing `DOTNET_BUNDLE_EXTRACT_BASE_DIR`, identical in both runs.

**Confirmation that the switches took effect.** Run B has 0 `memfd` descriptors, 0 socket `fchmod`s and 0
`clr-debug-pipe` FIFOs; run A has 19, 19 and 38 respectively.

### 10.2 Comparison matrix (A default vs B switched)

| Comparison | Result |
|---|---|
| `generate-manifest --check` output | **MATCH** (identical to committed fixture, 32,567 bytes) |
| `generate-manifest --stdout \| sha256sum` | **MATCH** (`bcaaa5c3…`) |
| `verify-derived` output (100 trees, 12 below threshold, content hash, catalog digest, UNTRUSTED label) | **MATCH** |
| `inspect-original` output: 3 review-evidence inspections + 3 `OriginalTrustNotAccepted` refusals | **MATCH** |
| `annual` refusals, all three domains | **MATCH** |
| Core tests: 33 per-test PASS lines | **MATCH** (33/33 both) |
| Built binaries: P0 Core reference `9a99f781…`, `SitkaP1.Core.dll` `502b816d…`, `sitkap1.dll` `b0d96703…`, `SitkaP1.Core.Tests.dll` `2ea2f7fa…` | **MATCH** (byte-identical) |
| Inputs (head, harness, analyzer, fixture, `TrustTables` blob, P0 tree) | **MATCH** |
| P0: 52 tracked-file SHA-256 list (A before vs B after) | **MATCH** |
| P0: L2 status (before/after) and L3 ignored snapshot (before/after; 0 ignored files) | **MATCH** (all empty) |
| L0 / L1 / L2 / L3 / L3+ | PASS in both |
| Analyzer synthetic self-test output | **MATCH** (23/23 both) |
| Decoy: 13/13 distinct identities, each sighted once, 0 unresolved; read-only control clean | **MATCH** |
| L4 real run | **DIFFER, as expected and explained**: A FAIL 38 UNRESOLVED / B PASS 0 UNRESOLVED; 0 P0 hits in both |
| Successful traced mutations (926 vs 812, Δ114) | **DIFFER, fully attributed** (table below); none in P0 or in P1 outputs |
| Host / save / replay / UI / Unity / Windows | **NOT RUN** (out of scope) |

**Attribution of the 114 extra successful mutations in A** (by syscall and target):

| Δ | What | Attribution |
|---|---|---|
| +19 | `fchmod(socket:[…], 0600)` | Diagnostics IPC socket (off in B) |
| +19 | `ftruncate(memfd:doublemapper)` | W^X double-mapping (off in B) |
| +38 / +38 | `mknodat` / `unlink` of `clr-debug-pipe-<pid>-…-in/out` FIFOs in TMPDIR | Debugger transport, part of diagnostics (off in B) |
| +16 | `unlink` of `dotnet-diagnostic-…` socket files in TMPDIR | Diagnostics (off in B) |
| +2 | Writes to `/proc/<pid>/task/<tid>/comm` (thread names) | One fewer runtime thread with diagnostics off; not a filesystem file |
| ±3 | `~/.dotnet/shm` MSBuild session entries | Build-server timing |
| −20 | 4 NuGet HTTP-cache entries (`~/.local/share/NuGet/http-cache/…`) rewritten in **B only** | Cache refresh in the later run. Attributed to cache-expiry timing; **not causally verified**. Outside P0 and P1 |

Both runs also create and remove 24 random-named MSBuild `CASESENSITIVETEST*` probe files (same count, different names).

### 10.3 Negative assurance (analyzer unchanged)

Run in the switched environment against the #649 synthetic traces:
- bare `ftruncate`/`fallocate`/`fchmod` → **exit 1, 3 UNRESOLVED**;
- decoded protected fd → **exit 1, HIT**;
- benign fd plus read-only calls → **exit 0**;
- `--self-test` → **23/23**.

No whitelist, filter or classification change was made. Ambiguous descriptors still fail closed.

### 10.4 Residual restrictions

- The switched-runtime L4 PASS covers only that configuration and the traced syscall set and process tree. For the
  default runtime, P0 integrity rests on L2/L3/L3+ (all PASS) plus an L4 that is correctly FAIL on 38 identified
  non-filesystem runtime descriptors.
- The switches change JIT memory mapping and remove the diagnostics endpoint. This comparison shows identical P1
  outputs, refusals and binaries for this F0 candidate only; it does not show general equivalence.
- The NuGet cache difference is attributed to timing, not proven.
- Earlier limits stand: deletion calls are not in the decoy, legacy relative paths are joined lexically, per-thread cwd,
  and the builder's same-user race (§8.4).

Evidence: `/media/jer/Files/CCF/Verification/CCF-651-P1-F0-ControlledDotnetTrace-20261116/` (with `SHA256SUMS`).
It holds the wrapper; per run `inputs.txt`, `command.txt`, `dotnet-env-seen-by-subprocess.txt`, `exit-code.txt`
(A 1, B 0), `built-binaries.sha256` and the full `iso/` harness output (raw traces, analysis JSON, CLI outputs, P0
before/after lists); `negatives/`; `scope-check.txt`.

**Next:** a narrow, independent non-author review of the switch scoping, the unchanged analyzer and negatives, the
matching P1 results, and the conditional nature of the switched-runtime L4. Then a Manager verdict. #639, #644, #649 and
#651 stay open. All four records stay `PendingIndependentReview`; the first ecological year stays on HOLD.

### 10.5 Erratum and evidence-manifest addenda (#658, added 16 November 2026 — original §10 text above left unchanged)

**Source of the corrections:** the independent #656 native witness (Codex `gpt-6.1-sol`, High), recorded
[here](https://github.com/jermur788/CCF/issues/656#issuecomment-6931891738); Manager #641 NEEDS CHANGES (evidence
packaging and report only); the #658 packet and Manager dispatch. **Authority** for this erratum and the sidecars: the
product owner's direct authorisation, recorded as a one-entry owner-transferred board write on main
[`97ed6a2f`](https://github.com/jermur788/CCF/commit/97ed6a2f3a51a3e91e5d98e1cff503093e7e77b0) (gates A/B/C).
[#658 RUNNING](https://github.com/jermur788/CCF/issues/658#issuecomment-6932218665). **NO ECOLOGICAL YEAR RUN; no
retest, no source change.**

**1. Corrected count.**
- The §10.2 row "+2 | writes to `/proc/<pid>/task/<tid>/comm` … (114 vs 112)" is **wrong**.
- The correct figures are: successful `/proc/.../comm` **write-intent opens**: **A = 117, B = 116, Δ = +1**. My original auxiliary `grep` missed some unfinished/resumed strace records.
- These are opens with write intent (`O_WRONLY`/`O_RDWR`). They are **not proven `write` syscalls**: `write` was not in the traced set.

**2. Reconciliation.**
- The 114 total stands: successful traced mutations A 926 vs B 812.
- Corrected sum: **19 + 19 + 38 + 38 + 16 + 1 + 3 − 20 = 114** (socket `fchmod`, memfd `ftruncate`, `clr-debug-pipe` `mknodat`, their `unlink`s, `dotnet-diagnostic` socket-file `unlink`s, `comm` opens, MSBuild `~/.dotnet/shm`, NuGet cache).
- §10.2's "**fully attributed**" is corrected to **fully count-reconciled; causation partly unproven**. The diagnostics-related identities, the thread-count reason for the `comm` delta, MSBuild timing and NuGet cache expiry are attributions from paths and switch scope, not established causes.

**3. Unchanged conclusions.**
- Default .NET runtime: **L4 FAIL, 38 UNRESOLVED** (not accepted).
- Verification-only switched runtime: **L4 PASS with 0 unresolved, only within the traced scope and that configuration**.
- P0 tracked snapshots, simulator outputs, pins, tests and binaries matched, as independently read by #656.

**4. Evidence-manifest addenda.** Every original was preserved: no historical manifest, trace, binary or receipt was
overwritten, renamed, moved, deleted or re-hashed in place. Exactly two **new** files were created exclusively in each
folder, `SHA256SUMS-CORRECTED-20261116` and `MANIFEST-CORRECTION-20261116.md`.

| Folder (`/media/jer/Files/CCF/Verification/…`) | Historical defect (result preserved) | New manifest: entries / SHA-256 | `sha256sum --strict -c` on the **new** manifest |
|---|---|---|---|
| `CCF-651-P1-F0-ControlledDotnetTrace-20261116/` | `SHA256SUMS` lists itself; #656: 71/72 OK, `./SHA256SUMS: FAILED` | 72 / `8723a7d6…bf2d` | exit 0, 72 OK |
| `CCF-649-P1-F0-L4-FailClosed-20261116/` | same self-entry defect; author check 51/52 OK | 52 / `e61085e1…a8d7`; the 2 decoy symlinks are listed in the note, **not followed or hashed** | exit 0, 52 OK |
| `CCF-644-P1-F0-FileSafety-20261116/` | same self-entry defect; author check 75/76 OK | 76 / `d398062e…29cc5` | exit 0, 76 OK |
| `CCF-639-P1-F0-20261116/` | `isolation-run-1.SHA256SUMS` sits in the parent but its paths are relative to `isolation-run-1/`, so in place: 14 files could not be read; no self-entry | 15 / `0ce1bae6…f214` (`./isolation-run-1/<file>` × 14 + the historical manifest) | exit 0, 15 OK |

**How the new manifests were built.**
- Each lists every regular file under its folder at creation time, sorted by byte value with `./` paths, walked without following symlinks.
- **The historical manifest is included as a payload file**, so its preserved bytes are pinned, while its content is not trusted.
- Each excludes only itself and its note. Its own SHA-256 is recorded in the note, not in itself.
- Inventory check per folder: regular files now = entries + 2.
- They were made by an author script kept outside the repository (`make_sidecars_658.py`, SHA-256 `aae526bc…3042`). The manifests are standard `sha256sum` format and do not need it to verify.

**Provenance boundary.**
- A clean new manifest proves only that the files matched **at creation time** (16 November 2026, about 23:2x UTC).
- It does not prove the files were identical when originally written, and it is not an independent witness of the original runs.
- The historical failures remain the record of what was originally stored.
- **Status: not yet independently re-verified**; the same #656 reviewer is to read the addenda.

**Next:**
1. The independent #656 Codex reviewer re-reads the eight sidecars, the preserved originals and this erratum, then publishes its reserved one-file result (gate C on `97ed6a2f`) with APPROVED / NEEDS CHANGES / REJECTED.
2. Manager #641 issues its verdict.

All four records stay `PendingIndependentReview`; the first year stays on HOLD.
