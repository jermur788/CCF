# Multi #710 — Four-shell mode repair and targeted guard qualification result

**Result: REPAIR COMPLETE. Targeted guards PASS within packet expectations. Default L4 still FAIL38 / BLOCKED.** 2026-11-17. Claim https://github.com/jermur788/CCF/issues/710, Manager https://github.com/jermur788/CCF/issues/694. Packet: `docs/multi-694-703-verdict-mode-fix-20261117-v1`.

New local candidate **R `35d26f5e64a00a400c700871927ccc2173370568`** has a single parent, C, and is mode-only. Results on R as committed:
- `build-output-safety.sh` **8/8 PASS**.
- `p0-isolation.sh` L0, L1, L2, L3 and L3+ **PASS**, with the embedded P1 tests 33/33.
- The default-runtime **L4 FAIL38** signature is unchanged (19 memfd `ftruncate` + 19 socket `fchmod`, 0 protected hits), so the script reports OVERALL FAIL by design. No waiver.

R stays local. No push, no main merge. This document contains no source.

## 1. Custody, permit, worker
- **Physical grant, owner start permit and [RUNNING]** (2026-11-17 ~21:55Z): https://github.com/jermur788/CCF/issues/710#issuecomment-6962057856.
  - Lane `MultiP1ModeRepair710` is recorded in `/media/jer/ZX20/CCF-Ops` board/active.md and lanes.json. Backups are under the lane's `receipts/`. Expiry 2026-11-19T18:00:00Z.
  - The project owner jermur788 directly granted [APPROVED TO START #710] in this session, in place of a separate Manager comment. Manager #694 keeps the result decision.
- **Worker:** Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, host `jer-laptop`. Not inherited from #697 or #703.
- **Board-lock incident (disclosed):** my 21:52:34Z board/lanes write took no flock, while Sen-1 #707 held an exclusive flock on `/media/jer/ZX20/CCF-Ops/board`. No data was lost. Confirmation and the corrective commitment (`flock -x -n` on the board directory before any future write, stopping if it is held) are at https://github.com/jermur788/CCF/issues/710#issuecomment-6962349394.
- **Environment:** SDK 8.0.133 (global.json) and runtime 8.0.33. `TMPDIR=/media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/tmp`. Files ext4 UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`. Evidence: `/media/jer/Files/CCF/Verification/CCF-710-ModeRepair-20261117/` (SHA256SUMS.txt).
- **Live main:** `93b3e10a` at the preflight snapshot, `d7014d4c` at postflight (other sessions' docs/board work). Neither touches the local candidate.

## 2. Workspace
| Item | Value |
|---|---|
| New isolated store | `.../CCF-710-ModeRepair-20261117/git/CCF-710-ModeRepair.git`: a `git clone --bare --no-hardlinks` of the #697 store, origin removed, `ccf.taskId=710`. No alternates, not shallow, no grafts or replace refs. |
| Source worktree | `.../ccf-710-src`, branch `repair/multi-p1-shell-modes-710-20261117-v1` (absent on origin). After commit and tests: clean, 0 status lines including ignored. |
| Disposable test clone | `.../ccf-710-test` (`--no-hardlinks` from the #710 store, detached at R, `core.fileMode=true`). All build and test output went here and to TMPDIR. |
| #697 source | read only; ref digest, worktree count, worktree state and evidence unchanged |

## 3. Repair R (exact)
Method: confirm all four paths are tracked `100644`, then `git update-index --chmod=+x -- <4 paths>`, then commit. No content edit; the `l4_analyze.py` mode was not changed.

| Path (under `Tools/MultiRackSitkaP1Lab/`) | Old mode | New mode | Blob (identical before and after) |
|---|---|---|---|
| `build/build-p0-core-ref.sh` | 100644 | 100755 | `0daee7d4d316a04806d59a37df4677a00017eff2` |
| `tests/build-output-safety.sh` | 100644 | 100755 | `c1e8371e76171d3295b924220609ab81ee6e69b9` |
| `tests/cli-safety.sh` | 100644 | 100755 | `a8fa3489940ad0152c3b4cc4d331aea5941718e3` |
| `tests/p0-isolation.sh` | 100644 | 100755 | `3f0befe8d3a98b31c09f956a02a32ff435259cff` |

Proofs (`r-structural-proof.txt`, `r-fsck.txt`):
- `git rev-list --parents -n1 R` gives `35d26f5e… 719ccbf8…`: a single parent, C.
- `git diff --raw C R`: exactly 4 rows, `:100644 100755`, same blob IDs.
- `--name-status`: 4 × `M`. `--numstat`: 0/0 for each. Content-diff lines: 0.
- **P0 L0:** `git log Q..R -- Tools/MultiRackSitkaLab` is empty. **P0 L1:** `R:Tools/MultiRackSitkaLab` = `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`; 52 files.
- **P1:** 27 files, no new paths. All 27 path+blob pairs are identical to F `e302f15b`. The 23 unchanged modes are identical to C. `tests/l4_analyze.py` stays `100644`.
  - **New P1 subtree `fd0861adea74f4f2bc2473375de5a9e82cd2f390`**, which differs from `2c017956` only through the 4 modes. The historical pin `2c017956` is not relaxed; R is a separate mode-only successor.
- The other 5,937 whole-tree entries (R has 5,941) are identical to C in path, mode and blob. That includes all Docs, specs, Assets, Packages and ProjectSettings, and M still has 5,862 entries.
- R tree is `33f84db2e7fdac527c343a758775757f49687c42`.
- 4 CCF pins: `bb83d7f8`, `e4c73402`, `e638e28e`, `51faaa0d` — 4/4 unchanged.
- `git fsck --strict --full --no-dangling`: clean, exit 0.

## 4. Targeted requalification on R (as committed, in the disposable clone)
| Run | Expected | Observed |
|---|---|---|
| `bash tests/build-output-safety.sh <ev>` | 8/8 PASS | **8/8 PASS**. C1–C6 refused with exit 4 and victims unchanged. C7 cold build exit 0, DLL `9a99f781…`, 3 regular single-link files. C8 rebuild gives the same hash. P0 digest `9e218ca7…30e8` unchanged. OVERALL PASS. |
| `bash tests/p0-isolation.sh <ev>` | L0–L3+ PASS, 33/33, L4 FAIL38 | **L0 PASS · L1 PASS · L4 self-test PASS** (23/23 synthetic, decoy 13/13, read-only control clean) · **L2 PASS · L3 PASS** (0 ignored files) · **L3+ PASS** (all 52 tracked P0 files) · **P1 tests 33/33**. L4 FAIL: succeeded 909, failed 160, protected hits **0**, unresolved **38**. Script exit 1 (OVERALL FAIL by design). No `Permission denied`. |
| L4 signature (from `L4-analysis.json`) | 19 + 19 | **19 × `ftruncate` on `memfd:doublemapper` + 19 × `fchmod` on a socket (0600)** — identical to #703 |
| P0 reference DLL | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | match (C7, C8 and the p0-isolation cold build) |
| Fixture / trust | unchanged | `generate-manifest --check` byte-identical (32 567 B, `bcaaa5c3…`); `verify-derived` TRUSTED, 100 trees, 12 below threshold, contentHash `b7dee8a3…`; P0 `MANIFEST.sha256` 3/3 OK; P1 src and fixtures and the P0 tree have no diff from C |

The overall p0-isolation script result is **not** a PASS, and L4 was not disabled or altered. As the packet directs, I didn't re-run the broad P0 98 and P1 33 suites separately. The #703 evidence stands for them, and the only change is 4 mode bits.

## 5. Security
Default-runtime L4 stays **FAIL38 — BLOCKED / UNRESOLVED**, with 0 protected hits. No waiver, no runtime flags, and no change to the security script. The fd-provenance review and decision remain a separate gate.

## 6. Postflight (`protected-before.txt` / `protected-after.txt`)
All identical before and after:
- C modes and identity (C still has 100644 everywhere).
- Primary: two dirty files, same hashes.
- #639 worktree: e302f15b, clean.
- Legacy store refs.
- #697: store refs, 2 worktrees, worktree at C with 0 status lines, evidence digest.
- #692/#687 and #674 store refs.
- #703 evidence (SHA256SUMS verified) and the three #703 clones (still at C).

Also confirmed:
- No `*710*` branch on origin other than this docs output.
- No #710 dotnet, strace or VBCSCompiler process left; the remaining processes belong to Sen-1 #707.
- Unity lease free throughout; no Unity, no Windows, no ecological year.
- The Sen-1 A0 lane, branch and row were not touched.
- Lane size 4.3 GB. Lease open until the custodian releases it. Suggested cleanup after the Manager decision: remove `ccf-710-test` and `tmp/`; keep the store, `ccf-710-src` (R) and evidence.

## 7. Decision request
Mode repair R is exact and minimal. Its targeted guards meet every packet expectation. **Worker recommendation: APPROVED as mode-only successor R**, with default L4 FAIL38 still a separate open gate. Manager #694 decides APPROVED / NEEDS CHANGES / REJECTED and whether a proportional independent review is needed.
