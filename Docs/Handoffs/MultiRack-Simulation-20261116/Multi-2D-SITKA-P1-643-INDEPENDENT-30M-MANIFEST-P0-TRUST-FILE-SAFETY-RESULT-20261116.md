# Multi — #643 independent 30m manifest, original-P0 trust and file-safety review

**Date:** 2026-11-16  
**Verdict: NEEDS CHANGES for the F0 manifest + compiled original-P0 trust + file-safety checkpoint.**  
**Findings:** 1 BLOCKER, 2 MAJOR, 3 MINOR. **No change to production source or trust status is authorised.**  
**Parent Manager:** [#641](https://github.com/jermur788/CCF/issues/641). **Author:** [#639](https://github.com/jermur788/CCF/issues/639). **Reviewer claim:** [#643](https://github.com/jermur788/CCF/issues/643).

## 1. Actual reviewer and independent lane

- **Reviewer:** OpenAI ChatGPT GPT-6, non-author, conducting technical source reasoning at high effort. **This is not represented as an independently launched OpenAI Codex executable or a local Codex/Unity environment.** The #639 implementation author is Claude Code / Anthropic Opus 5.5.
- **Review environment:** connected GitHub browser repository tools only; no local checkout, .NET SDK process, shell trace or physical laptop access. Hash and fixture calculations were independently executed as JavaScript within a separate tool isolate after retrieving the exact GitHub source files. No protected worktree was opened.
- **Source candidate:** `feature/multi-p1-639-f0-30m-foundation-20261116-v1` exactly at `0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5`; parent `90583811c4fb88508a4bf05c14365732e24a21ac`. Original P0 qualified pin `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`.
- **Reviewer branch:** `review/multi-p1-643-manifest-p0-trust-file-safety-20261116-v1` created from the candidate, used for **this result Markdown only**. Independent lane contains no physical worktree or build-output root. Source and review branches both compared identical to the candidate before publication. Live `main` coordination was read, not modified; author retains exclusive `Tools/MultiRackSitkaP1Lab/**` writer custody.
- **Source diff checked independently:** parent-to-candidate 24 *added* files under `Tools/MultiRackSitkaP1Lab/**` and one author-result Markdown. **No tracked modification** to `Tools/MultiRackSitkaLab/**`, `Assets/**`, `Packages/**`, `ProjectSettings/**`, specs or live coordination. This proves committed scope only, not ignored-file or physical-worktree cleanliness.

## 2. Files actually read and work performed

**P1 pinned source read:** `src/Core/{NoFollowRead,FixtureAccess,TrustTables,Domain,F32,DerivedManifest,StrictJson,ManifestGenerator,CcfSourcePins}.cs`; `src/Cli/Program.cs`; `src/Core/SitkaP1.Core.csproj`; `build/build-p0-core-ref.sh`; `tests/p0-isolation.sh`; `tests/Core.Tests/Program.cs`; `Directory.Build.props`; `README.md`; `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`. Also the #639 result, #643 packet, live coordination, #639 comments and accepted P1 spec §9.2.

**Original P0 pinned source read:** `Tools/MultiRackSitkaLab/src/Core/CanonicalJson.cs` and `LabFile.cs`; all three source **fixture files retrieved from the fixed P0 revision**. Scenario One pinned `SitkaGrowthModel.cs`, `TreeSpeciesDefinition.cs`, `Species/SitkaSpruce.asset` and source snippets from `ForestEcologyController.cs` were read to check the density, crown and modelling classifications.

**Independently calculated in this review:** SHA-256 over all four fetched fixture texts; canonical hash over the three original P0 snapshots using the same published field order (independent JSON object serialisation, not the author’s P0 test or `MANIFEST.sha256`); their counts, all-pristine flags and integer shoelace areas; both trust/catalog table digests; 100-tree generation rules independently reproduced with 64-bit BigInt SplitMix64 and float32 rounding; committed-manifest full file and content hashes; stand stem density, Dq, basal area and relative density.

**NOT RUN independently:** `dotnet build`, `LabFile.Parse` or `CanonicalJson.SnapshotHash` as compiled .NET functions, P0 Core DLL reproducibility, `ManifestGenerator.Generate()` compiled byte-for-byte rerun, 27 targeted tests, 4 mutations, L2/L3 physical ignored-file audits, L4 syscall tracing, Linux `statx/openat` negatives, actual P0 symlink-write exploit, Unity, Host/CSRF, save/fsync, ecological-year execution or Windows build. The #639 worker **reported** 27/27 and 4/4 passes, L0–L4 passes, and two matching compiled DLL hashes. Those reports were assessed, not re-executed.

## 3. Independently verified 30 × 30 m manifest — PASS at source/data level

- **Exact file:** 32,567 bytes; SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`.
- **Canonical content SHA-256 (entire object except terminal `contentHash`):** `b7dee8a34b4e66b4cc443b533604b5962f5fe4c1d2f28832270a0c2b02ba6627`; embedded value matches.
- **Compiled catalog digest independently reconstructed:** `c507763dc779eae37af8df10eb60690b3f6bcc2048867dbe1b1eaf4463c66646`; record remains `PendingIndependentReview`.
- Independent integer SplitMix64 row/column/jitter/DBH/height calculations matched **all 100** rows and exact Float32 DBH/height/crown/XZ bit strings (0 mismatches). IDs `P30-001`–`P30-100` are row-major and distinct; **100/100** positions distinct and inside the declared 30m boundary; all are living age-30 Sitka. DBH range **131–349 mm**, **12 below 170 mm**, **88 action eligible**; those 12 remain biologically present in the declared 100-tree population.
- The manifest is explicitly **designed**, never P0-rescaled: `originalP0FixtureId=null`, 10×10 3m grid, independent generator seed `6390001`, ecological seed `20261116`, initial ecological year zero, growth model 2 and RNG model 1, fixed site factor 1.0f, MatureEquivalent/Reference/unpruned, excluded-system list. XY integer mm maps to centred Unity XZ by the stated subtraction/division/cast rule, *not* a Unity 30m geometry claim. Area = 900 m² / 10000 = 0.09 ha, stored `f32 3db851ec`. Format, exact domain tag and disclosing synthetic/non-parity language are present.
- **Independent density arithmetic against pinned source** (Comeau maximum-SDI relationship as implemented by `SitkaGrowthModel.RelativeDensity`): 1,111.111 stems/ha; Dq 25.3033 cm; basal area 55.8729 m²/ha; **RD 0.6097958408**. Thus `DensityPressure=0.0244896` for the current onset 0.6. The initial stand is only just beyond the source-calibrated onset. The source mortality model’s normalized uncapped baseline `0.08×P²` is about `0.00004798` per stem per year before individual weighting; **this is arithmetic on gameplay calibration, not an observed death probability or Irish measurement**.
- **Evidence class:** the Sitka source's maximum-density line and CrownPotential linear coefficients are *source-cited* and transfer/calibration dependent; specific tree positions, age, height–DBH rule, jitter, 170 mm management label, crowns initialized at CI=0, generator/ecological seeds, numerical site factor and 30m boundary are **synthetic P1 choices**. No Irish forecast or Unity numerical/geometry parity claimed.

**Limit of this PASS:** code-path checks and independent numeric recreation match the committed fixture; **compiled C# generation and native file-security behaviour were not independently run**. Manifest acceptance must still wait on the security findings and Manager gate.

## 4. Original-P0 compiled trust — PASS at independent file/hash/structure level

All three original files were retrieved from the exact qualified P0 source revision, **not** the co-located `MANIFEST.sha256`. A separate SHA-256 implementation self-tested against the public `abc` vector, then calculated the complete file hashes and each snapshot’s canonical JSON hash from the original P0 `CanonicalJson.Snapshot` field order.

| Fixture | Bytes | Full file SHA-256 | Independently recomputed P0 snapshot SHA-256 | Living / dead / below threshold | Exact polygon area |
|---|---:|---|---|---|---:|
| `oracle-quarter-ha` | 969 | `b1691934fa4a41cfd63462f4021811e0871742f0e1f23ecea77fe8da40b5cf62` | `810030c4187c3132cafb22d46ff87c0b8dbfa43bc382a72463c686f8c554152c` | 3 / 0 / 0 | 2,500 m² |
| `sitka-flat-demo-v1` | 26,675 | `d0eed68979b2d214555e84a59e80ff6866ccd5e7d1774883e7898fcf8fb4f2ed` | `b407c725e655dc80ca5fec22f4ad84540136e89928577ad97b3dce31b234f5b1` | 153 / 9 / 15 | 5,812.5 m² |
| `sitka-flat-perf-2000-v1` | 324,929 | `d12db11126de2843ff21538f26feaab09242ccaf6abd04e3f25c8014054790df` | `e4cebf0206c23977abc4b08857d706bdb1c110aa853b08241997081ccfde8726` | 1,893 / 107 / 182 | 20,000 m² |

All values match `OriginalTrustTable.Records`, including their embedded snapshot hashes; all snapshots are at experiment step and revision 0, have zero plans/events, no marks/felled trees, and zero elevations. Independently reconstructed full **original trust-table digest:** `38572effa59bbc6df09479ab8de6339b4567b27d8f61ad578b51fe5d5264c5ba`. All three records remain `PendingIndependentReview` and have null review/Manager acceptance refs.

The **source** inspection order in `FixtureAccess.InspectOriginal` matches the accepted §9.2 sequence with the pending-status guard ahead of filesystem opening, no-follow directory chain, no-follow leaf, opened-descriptor regular/single-link/exact-length checks, bounded read, full byte SHA, P0 strict parse/canonical rehash and pristine requirements. The reader never consumes `MANIFEST.sha256`; original inspection has `AnnualAdvanceAllowed=false`. All exposed CLI annual calls end in refusal; no ecological engine is compiled into F0.

The hashes are independently **recalculated**, but a second compiled P0 `.NET` execution is still **NOT RUN**. This difference matters; it is not presented as independent DLL or syscall qualification.

## 5. Findings — corrections required

### B-01 — BLOCKER — arbitrary manifest generation destination can overwrite protected original P0

**Code:** [`src/Cli/Program.cs` lines 15–20](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Tools/MultiRackSitkaP1Lab/src/Cli/Program.cs#L15-L20). `generate-manifest <out>` feeds user-controlled `args[1]` directly into `File.WriteAllBytes`, with no P1-only destination, path-component no-follow, or existing-file refusal. The documented CLI can therefore be passed a path **inside `Tools/MultiRackSitkaLab/**`**, including a checked-in original fixture, and would truncate/replace it. No special filesystem adversary is required; a legitimate but mistaken output destination suffices. The command is a P1 *run* that can mutate protected P0.

**Requirement affected:** accepted spec §9.2 T-01 “a P1 restore, build, test and run write nothing under `Tools/MultiRackSitkaLab`”, and the packet’s protected P0/no-mutation condition. This is a **static call-path proof**, not a destructive reproduction; deliberately **NOT RUN** against P0. It is not resolved by the normal CLI test, because that test does not run `generate-manifest` against a dangerous destination.

**Bounded fix:** confine generation to a validated P1-owned output or a separately controlled scratch root **outside P0**, with no-follow descriptor-relative creation and overwrite refusal; reject symlink/hardlink/path-traversal variants. Add a safe scratch-only negative test demonstrating P0-like protected destinations are refused. Re-run isolation on the exact repaired candidate. Until then, **the unconditional P0 write-isolation contract is not met**.

### M-01 — MAJOR — ignored P0-reference output directory can redirect build writes through a symlink

**Code:** [`build/build-p0-core-ref.sh` lines 19–21, 60–72](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh#L60-L72). `out_dir="$p1_root/.p0ref"` is an ignored, potentially pre-existing path. `mkdir -p "$out_dir"` does not reject a symlink to a directory; subsequent `cp`, `provenance.txt`, and checksum writes follow that symlink. A pre-existing `.p0ref -> ../MultiRackSitkaLab` redirects **three writes into protected P0** when the standalone builder is invoked as documented. The isolation test deletes only the P1-owned `.p0ref` before its cold run, so its passing L0–L4 evidence does not exercise this scenario.

**Requirement affected:** spec §9.2 build-output isolation and complete source/ignored-output preservation. **Static reasoning, no adversarial symlink execution**. Fix by rejecting symlinked P1-owned output root and anchoring file creation to a verified P1-owned descriptor; add a safe simulated-target symlink negative and a direct-build isolation test, without writing or cleaning real P0.

### M-02 — MAJOR — derived CLI verifier reads uncontrolled paths before size/type/no-follow gates

**Code:** [`src/Cli/Program.cs` lines 22–25](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Tools/MultiRackSitkaP1Lab/src/Cli/Program.cs#L22-L25), [`src/Core/FixtureAccess.cs` lines 101–114](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Tools/MultiRackSitkaP1Lab/src/Core/FixtureAccess.cs#L101-L114). The `verify-derived` CLI reads `args[2]` using `File.ReadAllBytes` **before** `LoadDerived` checks catalog length, full SHA and canonical JSON. This follows symlinks, can wait on a FIFO and may allocate an attacker-supplied oversized file, rather than classifying a regular single-link descriptor and rejecting it at the catalog size bound. Original-P0 inspection uses the stronger `NoFollowRead` path, so this finding **does not invalidate the proven original-P0 loader**, but the exposed derived verifier does not share its safeguards.

**Requirement affected:** spec §9.2 T-04 descriptor-checked capped fixture reads and F0 file-safety review. Replace the CLI entry’s path-based read with bounded, no-follow opened-descriptor reading using the derived catalog’s exact file length; test ordinary, symlink, hardlink, FIFO, directory and oversized file in an isolated scratch directory. No Host/persistence implementation is requested.

### m-01 — MINOR — derived parser does not itself enforce distinct XY positions

**Code:** [`DerivedManifest.cs` lines 291–318 and 326–340](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Tools/MultiRackSitkaP1Lab/src/Core/DerivedManifest.cs#L291-L340). `ValidateTree` enforces ordered distinct IDs and bounds, **not duplicate XY coordinates**. The one committed/hashed manifest has 100 independently verified distinct positions, so this is **not a bypass of the current compiled catalog**. Add a duplicate-position check and negative test when hardening the parser, preserving current bytes.

### m-02 — MINOR — L4 trace does not prove absence of every possible write

**Code:** [`tests/p0-isolation.sh` lines 41–42 and 74–83](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh#L74-L83). The observed L4 evidence is filtered by absolute P0 path text in strace output and a fixed syscall list. A process may issue relative `AT_FDCWD` operations after changing cwd, and any untraced syscall is outside that filter. The author correctly disclosed untraced calls. Do not promote “0 matched P0 write calls” to a universal security proof; retain the narrower **reported PASS for the traced benign cold run** and strengthen the test for path-relative writes/coverage if making a stronger claim.

### m-03 — MINOR / gameplay calibration — starting density barely activates mortality

**Code:** [`ManifestGenerator.cs` lines 6–15](https://github.com/jermur788/CCF/blob/0636ddc39cc5e8bc5c31f72ed9c6e81846f3c7f5/Tools/MultiRackSitkaP1Lab/src/Core/ManifestGenerator.cs#L6-L15) and pinned `SitkaGrowthModel.cs` lines 62–105. RD 0.60979584 is just above the 0.6 game-calibrated onset; baseline `0.08×P² ≈ 0.00004798` makes immediate first-year hazard deaths unlikely in this 100-tree synthetic stand absent subsequent growth/density changes. This is a **gameplay-experiment/sensitivity choice**, not an empirical Irish forest density. Preserve the current reviewed manifest unless the Manager intentionally decides to change it; consider a **separate later high-pressure synthetic control** to exercise mortality rather than silently altering the first fixture.

## 6. Passed checks, qualified limitations and disposition

**PASS (independently source/data checked):** three original-P0 immutable fixture lengths/full SHA/canonical snapshot SHA/counts/areas; compiled original trust digest; derived file length/full SHA/content SHA/catalog digest; all 100 generated integer/Float32 tree fields; age/living/threshold/geometry/axes/area/provenance/domain distinctions; current pending-status gate; explicit annual refusal for original/Unity/30m via exposed CLI; tracked candidate diff excludes all protected subtrees.

**PASS with scope qualification (source reasoning, not native execution):** original-P0 `NoFollowRead` and `FixtureAccess` call order and return labels; immutable binary-reference design; independent-source provenance; strict canonical parser. No evidence that source-level review alone qualifies Linux descriptor behaviour, filesystem identity/ownership, native DLL reproducibility or actual strace results.

**FAIL / correction needed:** no unconditional protected-P0 write-refusal for the exposed generator, ignored build-output symlink redirection, and derived CLI unbounded/following read path. The findings are contained to the F0 foundation; they do not justify implementing Host, saves, annual adapter, Unity parity, or polygon annual support here.

**VERDICT: NEEDS CHANGES.** This is **not** a rejection of the deterministic fixture values or the three original trust identities, both independently validated at the source/data level. It is refusal to accept the combined *manifest + trust + file-safety gate* while the documented P1 command/build paths can affect protected P0 or process hostile derived-file paths without the agreed controls.

## 7. Exact next action / ownership

Manager [#641](https://github.com/jermur788/CCF/issues/641) reviews this result and disposes **NEEDS CHANGES**, keeps both derived/original compiled records at `PendingIndependentReview` and maintains the **no ecological year** hold. The exclusive existing #639 source custodian remains the only possible P1 writer unless the Manager expressly changes ownership after a fresh board check. If repairs are authorised, issue **one bounded F0 file-safety repair packet** for B-01, M-01 and M-02 plus proportional isolated negatives; the manifest and original trust values should be retained byte-identical. Then request a **focused independent delta/security review**, not a repeat of the full 100-tree/P0 hash audit unless those inputs change. Only a subsequent Manager-approved review and separate reviewed acceptance-status commit may promote trust records to `Accepted`.

**No author source edits, P0 edits, live coordination edits, original ecological year, Unity operation, Windows build, or filesystem destructive test occurred in this review.** Physical dashboard, local Ops registry, Drive mirror and actual laptop worktree state are **NOT VERIFIED**.
