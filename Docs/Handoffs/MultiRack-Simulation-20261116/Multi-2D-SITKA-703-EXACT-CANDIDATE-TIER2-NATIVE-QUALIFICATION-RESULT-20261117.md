# Multi #703 — Exact-candidate Tier-2 native qualification result

**Result: NEEDS CHANGES (narrow, mechanical).** Date 2026-11-17. Claim https://github.com/jermur788/CCF/issues/703, Manager https://github.com/jermur788/CCF/issues/694. Packet: `docs/multi-694-699-acceptance-qualification-gate-20261117-v1` @ `48a0c6b7`.

Compile, tests, the DLL hash, fixture identity, P0 isolation and the CLI guards **PASS** on exact candidate C.

Two accepted-tree guard scripts, `tests/build-output-safety.sh` and `tests/p0-isolation.sh`, **FAIL as committed** with exit 126. The cause is a file-mode defect: `build/build-p0-core-ref.sh` is tracked `100644` and both scripts execute it directly. It came in with the first P1 commit (#639 `21bf97b8`), not with the merge.

A labelled diagnostic copy with only the exec bit set passes build-output safety 8/8 and p0-isolation L0–L3+. Its L4 reproduces **FAIL 38**.

Default-runtime security stays **BLOCKED / UNRESOLVED**. This result grants no waiver.

This document contains no source. Candidate C stays local, with no push and no main merge.

## 1. Authority, custody and worker
- **Gate A physical readback** (custodian, read-only): https://github.com/jermur788/CCF/issues/703#issuecomment-6961271572. Preflight PASS.
- **Lease issued, owner start permit, [RUNNING]**: https://github.com/jermur788/CCF/issues/703#issuecomment-6961328087.
  - Lane `MultiTier2Qual703` is recorded in the live board `/media/jer/ZX20/CCF-Ops` (board/active.md and lanes.json; backups under the lane's `receipts/`). Expiry 2026-11-19T18:00:00Z.
  - **Start permit:** the project owner jermur788 directly granted [APPROVED TO START NATIVE QUALIFICATION] in this session, in place of a separate Manager #694 comment. Manager #694 keeps the acceptance decision.
- **Worker:** Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness inside T3 Code, host `jer-laptop`, kernel 7.0.0-42-generic. Sole worker; the #697 Codex lease was not used or inherited.
- **Environment:** .NET SDK 8.0.133 (selected by both `global.json`, latestPatch); runtimes NETCore and AspNetCore 8.0.33. No NuGet packages are needed.
  - `TMPDIR=/media/jer/Files/CCF/Operations/CCF-703-Tier2-Qual-20261117/tmp` on Files ext4, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`. `/` is 99% full, so `/tmp` was not used.
  - Evidence: `/media/jer/Files/CCF/Verification/CCF-703-Tier2-Qual-20261117/`, plus Gate A at `/media/jer/Files/CCF/Verification/CCF-703-GateA-Preflight-20261117/`.
- **Live main** at start was `8f6bca0b…`; at publish it is `c91fc12f…`. Both were board/docs-only movement and had no effect on the local candidate.

## 2. Source isolation
| Item | Value |
|---|---|
| Source | `/media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/git/CCF-697-Child-B.git`, read only via `git clone --no-hardlinks --no-checkout`. It received no refs, worktrees or objects; its ref digest and worktree count are unchanged. |
| Qualification clone | `.../CCF-703-Tier2-Qual-20261117/ccf-703-qual`, detached at C, origin removed |
| P0 baseline clone | `.../ccf-703-p0`. It's a separate copy so P0 build output never lands in the P1 clone's P0 tree; see §3 for why. |
| Diagnostic clone | `.../ccf-703-diag`. Its only change from C is the working-tree exec bit on 3 `.sh` files; nothing committed. Used for §5 only. |
| C / tree | `719ccbf87f0644a1a3fb9487ce7b991f4b867424` / `5c7919568b4f580fd9ab1edc86f623caddcd6b93` — PASS |
| H2 / F parents | `ce7bfeb6…` / `e302f15b…` — PASS; Q `a7f41983` is an ancestor of C; M `5b858031` present |
| P0 subtree | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`, 52 files — PASS |
| P1 subtree | `2c017956577583d629f9020beafb6c0e757b9b51`, 27 files — PASS |
| 4 CCF pins | SimulationRandom.cs `bb83d7f8`, SitkaGrowthModel.cs `e4c73402`, Species/SitkaSpruce.asset `e638e28e`, TreeSpeciesDefinition.cs `51faaa0d` — 4/4 PASS |
| Symlinks / tracked binaries in subtrees | none / none |

## 3. Step 2 — P0 native baseline: PASS
| Check | Command (cwd `Tools/MultiRackSitkaLab` of the P0 clone) | Result |
|---|---|---|
| Build | `dotnet build MultiRackSitkaLab.sln -nodeReuse:false -p:UseSharedCompilation=false` | 0 warnings, 0 errors (warnings are errors) |
| Core tests | `dotnet run --no-build --project tests/Core.Tests` | **82 passed, 0 failed** |
| Host tests | `dotnet run --no-build --project tests/Host.Tests` | **16 passed, 0 failed**. The concurrent swap test had 0 alternate overwrites. |
| Schema | `python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json` | PASS, 9 OK lines |
| Negative controls | `python3 -I -B tests/run-mutation-controls.py $TMPDIR/p0-mut …` (scratch copy in TMPDIR) | **35/35 caught**, including original M01–M17 17/17 |
| Fixture immutability | `sha256sum -c fixtures/synthetic/MANIFEST.sha256` before and after | 3/3 OK |

**Count note:** the packet's "52 tests" matches the **52 P0 files**. The P0 suites at C actually contain 82 + 16 = **98** tests, and all pass. I treated the packet figure as a file/test conflation, not a missing-test failure.

**Harness observation:** my first attempt used `dotnet build --artifacts-path <external>`. Every P0 test then failed in a type initializer, because `ToolPaths.Find()` locates the tool root by walking up from `AppContext.BaseDirectory`. P0 tests therefore cannot run with output outside a P0 tree. I recorded that attempt (`s2-*-ATTEMPT1-external-artifacts.log`) and moved the baseline to the disposable P0 clone. No original P0 directory was written.

## 4. Steps 3–5 — P0 reference DLL, P1 baseline, fixture
| Check | Result |
|---|---|
| `bash build/build-p0-core-ref.sh` (exit 0; inputs hashed before and after; extraction under TMPDIR) | DLL sha256 **`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` = expected, PASS**. Provenance: commit a7f41983, tree f212c8f6, sdk 8.0.133. Three regular single-link files in git-ignored `.p0ref/`, never tracked. |
| P1 clean build `dotnet build MultiRackSitkaP1Lab.sln` | 0 warnings, 0 errors |
| P1 core tests `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **33/33 passed, 0 failed**. Includes the refusal, trust/identity, P0 blob-pin and source-pointer tests (domain/JSON/manifest/derived/original refusals; symlink, hard-link and FIFO refusal; non-pristine P0 refusal; no file-writing API). |
| `tests/cli-safety.sh` | **OVERALL PASS** (23 PASS lines). P0 tracked-file digest `9e218ca7…30e8` unchanged. |
| `generate-manifest --stdout \| sha256sum` | `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, matching the committed fixture |
| `generate-manifest --check fixtures/derived` | "generator output identical to committed fixture (32567 bytes)", exit 0 |
| `verify-derived p1d-30m-sitka-adult-v1` | TRUSTED; trees=100; belowActionThreshold=12; contentHash `b7dee8a3…6627`; derivedCatalogDigest `143a253a…a6f7` |
| Fixture content (read-only parse) | 30 000 mm square, 900 m², 0.09 ha; 100 unique IDs; 100 unique locations; all live; 12 below 170 mm, 88 at or above |
| `inspect-original … oracle-quarter-ha` | read-only inspection succeeds; annual and work refused |
| `annual …` | `REFUSED AnnualEngineNotImplemented`, exit 2. No ecological year. |
| Fixture after all runs | unchanged; nothing regenerated or overwritten |

## 5. Step 4 guard scripts — FAIL as committed (file-mode defect)
**Defect.** In C, all five P1 scripts are tracked `100644`: `build/build-p0-core-ref.sh`, the three `tests/*.sh` and `tests/l4_analyze.py`. They have been `100644` since the first P1 commit, #639 `21bf97b8`, and remain so in #644 `24805388` and F `e302f15b`.

- `tests/build-output-safety.sh:12,40` executes `"$builder"` directly.
- `tests/p0-isolation.sh:99` executes `build/build-p0-core-ref.sh` directly.

On a fresh checkout both therefore fail with `Permission denied`, exit 126.

| Run | Result |
|---|---|
| `bash tests/build-output-safety.sh <ev>` (as committed) | **FAIL**. C1–C6 all exit 126 (victims unchanged); C7 cold build exit 126. The script moved `.p0ref` back on early stop. |
| `bash tests/p0-isolation.sh <ev>` (as committed) | **FAIL**, exit 126. L0 PASS, L1 PASS, L4 self-test PASS (23/23 synthetic; decoy 13/13; read-only control clean). Stopped at the traced builder call. |
| **Diagnostic only** (`ccf-703-diag`, exec bit set, nothing committed): `tests/build-output-safety.sh` | **8/8 PASS**: C1–C6 refused with exit 4 and victims unchanged; C7 cold and C8 rebuild give the expected DLL hash; P0 digest and status unchanged |
| **Diagnostic only**: `tests/p0-isolation.sh` | L0 PASS · L1 PASS · L2 PASS · L3 PASS (0 ignored files) · L3+ PASS (all 52 tracked P0 files) · P1 tests 33/33 · L4 **FAIL**: succeeded 909, failed 161, protected hits **0**, unresolved **38**, failed protected attempts 0 → OVERALL FAIL |

The diagnostic runs don't count as qualification of C. They show that the exec bit is the only obstacle for these two guards, apart from the separate L4 question.

**Repair for a separate source task, not done here:** `git update-index --chmod=+x` on the 4 shell scripts, or invoke the builder with `bash` inside the two tests. Then re-run §5 on the new exact candidate.

## 6. Step 6 — Security: FAIL 38, BLOCKED / UNRESOLVED
The diagnostic L4 trace of the default runtime reproduces the prior unwaived signature exactly: **19 × `ftruncate` on `memfd:doublemapper` and 19 × `fchmod` on a diagnostic socket (0600) = 38 UNRESOLVED**, with 0 protected hits.

Native tests passing does not make the runtime secure. This task grants **no waiver** and ran no full strace or mutation programme beyond the accepted scripts. A separate read-only fd-provenance review and decision is still required before production L4 can be accepted.

**Observation:** the traced run's build left a VBCSCompiler server under strace for its roughly 10-minute idle timeout. I waited for it to exit naturally rather than killing it, so the trace is complete and unsignalled.

## 7. Step 7 — Postflight
- All three clones: HEAD C, tree `5c791956`, 1 ref each.
  - qual and P0 clones: 0 tracked modifications with fileMode on, and the contents of all 79 tracked files match the step-1 snapshot.
  - diag clone: exactly 3 mode-only diffs, as intended.
- P0 `MANIFEST.sha256` 3/3 OK. P1 fixture sha unchanged. No tracked binaries.
- #697 store ref digest and worktree count unchanged. #697 worktree is at C with 0 status lines, including ignored files. #697 evidence digest is unchanged (`84985042…`).
- Primary is at HEAD 46247486 with the same two user-dirty files and identical hashes. The #639 worktree is at e302f15b and clean. Legacy store ref digest unchanged.
- The canonical store changed only through concurrent Sen-1 sessions and T3 checkpoint refs. No #703 command wrote to it.
- No pushes from any source store, no main merge, no Unity, no Windows. Unity lease free throughout. No #703 dotnet, strace or VBCSCompiler process is left running.
- Lane size 6.4 GB; Files free 44.3 GB. The lease stays open until the custodian releases it. Suggested cleanup: delete the three clones and `tmp/` after the Manager decision, and keep the evidence.

## 8. Verdict for Manager #694
| Area | Status |
|---|---|
| P0 build + 82 Core + 16 Host + schema + 35/35 controls + fixtures | **PASS** |
| P0 reference DLL `9a99f781…` | **PASS** |
| P1 clean build + 33/33 | **PASS** |
| Deterministic fixture and CLI refusals | **PASS** |
| cli-safety | **PASS** |
| build-output-safety, p0-isolation (as committed) | **FAIL**: mode 100644, exit 126 (diagnostic: 8/8 and L0–L3+ PASS) |
| Default-runtime L4 | **FAIL 38 — BLOCKED / UNRESOLVED, no waiver** |

**Recommendation: NEEDS CHANGES.** Make one mechanical source repair (the exec bits) as a separately authorised task, then re-run §5 on the new exact candidate. The L4 decision stays a separate gate. Manager decides APPROVED / NEEDS CHANGES / REJECTED.
