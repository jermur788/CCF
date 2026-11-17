# Multi #699 — Independent read-only review of Child B candidate C (H2 + F) — RESULT

**Recommendation: APPROVE** (source identity/ancestry only; Manager #694 decides). Not a functional qualification: no build, .NET, Unity, test or annual-simulation run was performed or claimed.

- Claim: https://github.com/jermur788/CCF/issues/699 · Manager: https://github.com/jermur788/CCF/issues/694 · Writer: https://github.com/jermur788/CCF/issues/697
- Packet: `docs/multi-699-child-b-independent-review-20261117-v1` @ `6b59402a`
- Reviewer: Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness in **T3 Code**, host **jer-laptop** (`hostname` verified). Different provider from writer #697 (OpenAI Codex) and orchestrator #687 (DeepSeek).
- Review window (UTC): pre-snapshot 2026-11-17T20:49:33Z → post-snapshot 2026-11-17T20:52:13Z (Git checks ran between them; result written afterwards).

## Custody
- **Direct custodian grant:** the user, as physical custodian of jer-laptop, answered "Grant and run review" in this T3 Code session on 2026-11-17 (~20:49Z). That granted READ-ONLY access to the three paths below until **2026-11-19T18:00:00Z**. The grant did not transfer or borrow Codex #697's exclusive write lease on lane `MultiP1ChildB697`.
- Grant + RUNNING recorded on #699 **before any store access**: https://github.com/jermur788/CCF/issues/699#issuecomment-6960605383
- Read-only paths:
  - store `/media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/git/CCF-697-Child-B.git` (bare; checkout is its linked worktree)
  - checkout `/media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/ccf-697-child-b`
  - evidence `/media/jer/Files/CCF/Verification/CCF-697-P1-Child-B-20261117/`
- Collision check: before access and again at the end, `pgrep` showed no process referencing these paths other than this session.
- Environment: `GIT_OPTIONAL_LOCKS=0`, `GIT_NO_REPLACE_OBJECTS=1`. Only `cat-file`, `rev-parse`, `rev-list`, `merge-base`, `log`, `ls-tree`, `diff`, `for-each-ref`, `config --list`, `fsck --full --strict --no-dangling`, `check-ignore --no-index` and `hash-object` (without `-w`) were used. There was no fetch, checkout, status, index, commit, ref, config write, GC or cleanup. The protected #692 H2 store, the #639 worktree and the canonical/legacy stores were not opened, except for read-only `cat-file` on the canonical store (see Check 2). Primary's dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were not touched.

## Results summary

| # | Check | Result |
|---|---|---|
| 1 | #697 evidence integrity: `sha256sum -c SHA256SUMS` | **PASS**. 58/58 OK, and every evidence file except `SHA256SUMS` itself is listed |
| 1b | Local #697 result doc equals GitHub `e066d414` copy | **PASS**. Same sha256 prefix `186e4b800ef1cb07` |
| 2 | C object: tree `5c791956…`, parent1 H2, parent2 F | **PASS** |
| 2b | H2, F, Q, M are ancestors of C; Q is an ancestor of H2 and F | **PASS** |
| 2c | Not shallow; no alternates, grafts or `shallow` file; 0 replace refs; 1 root commit; 2,058 commits | **PASS** |
| 2d | `fsck --full --strict --no-dangling` | **PASS**. Exit 0, no output (14.8 s) |
| 2e | F, Q and M commit objects byte-identical to canonical `Repository.git` | **PASS** |
| 2f | H2 against canonical | **N/A**. H2 is not in canonical (expected; it lives in the protected #692 store, which was not opened). H2 is SHA-1 self-identifying, and its parents M and Q are canonical-identical. |
| 3 | P0 L0: `git log Q..C -- Tools/MultiRackSitkaLab` | **PASS**. 0 commits (see note N-1) |
| 3b | P0 L1 subtree in Q = H2 = F = C = `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` | **PASS** |
| 3c | 52 P0 path/mode/type/blob entries: Q = H2 = C (byte-identical `ls-tree -r`) | **PASS** |
| 4 | P1 subtree in F = C = `2c017956577583d629f9020beafb6c0e757b9b51`; absent in H2 | **PASS** |
| 4b | 27 P1 entries: F = C = #697 published BOM (GitHub `e066d414`) = evidence `approved-f-tree.tsv` | **PASS**. All 27 are `100644 blob` |
| 4c | `diff --no-renames --name-status H2 C`: exactly 27 `A` entries under `Tools/MultiRackSitkaP1Lab/`, 0 others | **PASS** |
| 4d | No generated or ignored P1 file tracked | **PASS**. No bin/obj/TestResults/dll-type paths. `check-ignore --no-index -v` matched only the `!*.sln`, `!*.csproj` and `!evidence/README.md` re-include rules, so none of the 27 is ignored |
| 5 | Full `ls-tree -r` of C minus P1 is byte-identical to H2 (5,914 entries); H2 minus P0 is byte-identical to M (5,862) | **PASS** |
| 5b | 11 Q-era docs/specs `A` (M→Q, outside P0) are absent in C; the 1 `D` and 3 `M` paths are at the M/H2 blob in C | **PASS** |
| 5c | 9 F-era non-P1 changes (8 `A` absent, `decision-log.md` at H2 blob) | **PASS** |
| 5d | Four CCF source pins equal the expected values in H2, C and live main | **PASS** |
| 5e | `diff H2 C -- Assets Packages ProjectSettings Docs specs AGENTS.md CLAUDE.md` | **PASS**. 0 paths |
| 6 | Live main drift vs M | **PASS (coordination-only)**. Main `01fd0482` is 13 ahead and 0 behind M, and changes only `Docs/Project/coordination/active-tasks.md`. Neither P0 nor P1 is on main |
| 6b | C and H2 remain local/unpushed | **PASS**. GitHub returns not-found for both; C is not in canonical |
| 6c | Store refs unchanged: `refs/ccf/697/f`=F, `refs/ccf/697/h2`=H2, `refs/heads/integrate/multi-p1-h2-f-child-b-20261117-v1`=C | **PASS** |
| 6d | Checkout: HEAD = C on that branch; all 5,941 tracked blobs re-hash equal to C; 0 untracked files | **PASS** |
| 7 | Non-mutation inventory (path/type/size/mtime/mode) before vs after, for all three trees | **PASS**. All identical (table below) |

## Key command outputs

```
$ git cat-file -p 719ccbf87f0644a1a3fb9487ce7b991f4b867424
tree 5c7919568b4f580fd9ab1edc86f623caddcd6b93
parent ce7bfeb6b439d935b2e00094f79338d3b8a658a0
parent e302f15bf6188605a2c2cbeddbd973b3d8ac0f66
author jermur788 <…> 1794948095 +0000
committer jermur788 <…> 1794948095 +0000

Integrate the exact Multi-Rack Sitka P1 foundation

$ git rev-list --parents -n1 C
719ccbf8… ce7bfeb6… e302f15b…
$ git rev-list --parents -n1 H2      # H2 = merge(M, Q)
ce7bfeb6… 5b858031… a7f41983…
$ git rev-list --parents -n1 F
e302f15b… a5574924a03e2590d7954e56e8925e2e17714213

merge-base --is-ancestor: H2 YES, F YES, Q YES, M YES (→C); Q→H2 YES; Q→F YES
rev-parse --is-shallow-repository: false; replace -l: 0; rev-list --count C: 2058; roots: 1

P0 tree  Q/H2/F/C: f212c8f64fb28b83aa870c7b5b7c72e03051e46a (×4)
P1 tree  F/C:      2c017956577583d629f9020beafb6c0e757b9b51 (×2); H2: ABSENT

ls-tree -r entry counts: M 5862 | Q 5924 | H2 5914 | F 5959 | C 5941
C minus P1 == H2 (cmp identical); H2 minus P0 == M (cmp identical)
diff --no-renames --name-status H2 C: 27 A, all under Tools/MultiRackSitkaP1Lab/

PIN OK Assets/ForestPrototype/SimulationRandom.cs      bb83d7f8468002ab13a6aec6caf548e0bf3ad34b
PIN OK Assets/ForestPrototype/SitkaGrowthModel.cs      e4c73402d46bb3b75a9cba2e4622857a913fac92
PIN OK Assets/ForestPrototype/Species/SitkaSpruce.asset e638e28ede09afc201d9e01fed938184b7b282f8
PIN OK Assets/ForestPrototype/TreeSpeciesDefinition.cs 51faaa0de0d218886935509ff64cb4f40598dd03
```

### P1 bill of materials in C (equal to F, to the #697 published BOM and to evidence `approved-f-tree.tsv`)
All `100644 blob`, 27 paths under `Tools/MultiRackSitkaP1Lab/`: `.gitignore`, `Directory.Build.props`, `MultiRackSitkaP1Lab.sln`, `README.md`, `build/build-p0-core-ref.sh`, `evidence/README.md`, `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, `global.json`, `src/Cli/Program.cs`, `src/Cli/SitkaP1.Cli.csproj`, `src/Core/{CcfSourcePins,DerivedManifest,Domain,F32,FixtureAccess,ManifestGenerator,NoFollowRead,StrictJson,TrustTables}.cs`, `src/Core/SitkaP1.Core.csproj`, `tests/Core.Tests/{Program,Runner}.cs`, `tests/Core.Tests/SitkaP1.Core.Tests.csproj`, `tests/{build-output-safety,cli-safety,p0-isolation}.sh`, `tests/l4_analyze.py`. The full blob-ID listing is the one in the #697 result at `e066d414`; this review checked it entry by entry (`cmp` identical).

### Q-era and F-era exclusions (all OK: the C blob equals the M/H2 blob, or the path is absent in all three)
- M→Q outside P0: 11 `A` absent in C (7 `Docs/Handoffs/MultiRack-Simulation-2026111{4,6}/` MULTI-001/002/004/006/008 and post-save repair handoffs, plus 4 `specs/multirack-sitka-2d-reference-lab-v1/*`); 1 `D` (`MANAGER-OWNER-CONFIRMED-SITKA-ANNUAL-GROWTH-EXPERIMENT-DECISION-20261115.md`) present at M blob `a197ec5c…`; 3 `M` (`active-tasks.md`, `current-milestone.md`, `decision-log.md`) at M blobs.
- Q→F outside P1: 8 `A` absent in C (4 P1 handoffs, plus 4 `specs/multirack-sitka-p1-adult-rules-comparator-v1/*`); `decision-log.md` at M/H2 blob `9994c609…`.

## Non-mutation inventory
`find <tree> -printf '%P\t%y\t%s\t%T@\t%m'`, sorted, captured before and after:

| Tree | Entries pre/post | Listing sha256 (prefix) pre = post | Result |
|---|---|---|---|
| store (includes worktree admin dir and index) | 40 / 40 | `fe483e9e1910cee0` | IDENTICAL |
| checkout | 6,555 / 6,555 | `e9dcd91093b36647` | IDENTICAL |
| evidence | 60 / 60 | `5ec4cf3bc17dfac4` | IDENTICAL |

## Notes and limitations
- **N-1 (informational, not a defect).** `git log --full-history Q..C -- Tools/MultiRackSitkaLab` lists exactly one commit, H2 itself. That is because H2 = merge(M, Q) brings P0 into the M line: the P0 diff against parent 1 (M) is +52 files, and against parent 2 (Q) it is empty. The command the packet specifies (default history simplification) emits 0, and so does `--simplify-merges`. P0 content is unchanged since Q (Check 3c).
- H2 was not compared against its original #692 store, which is protected and outside the grant. Its identity rests on its SHA-1, its verified parents M and Q (canonical byte-identical), and #692/#696 prior approval.
- The inventory covers path, type, size, mtime and mode only. Access times, inode ctime and machine-wide activity are not covered, and no machine-wide non-mutation claim is made.
- The commit-object listing above masks the author email.
- Out of scope, not run: build, `dotnet`, tests, Unity, Windows, security L4 (default L4 stays FAIL38 with no waiver), and the first ecological year (HOLD).

## Recommendation
**APPROVE** candidate C `719ccbf87f0644a1a3fb9487ce7b991f4b867424` (tree `5c7919568b4f580fd9ab1edc86f623caddcd6b93`) as source-identity-correct. It is exactly H2 plus the 27-file immutable F P1 directory. All 52 P0 entries, all 5,862 M entries, the four CCF source pins and every Q-era/F-era exclusion are preserved, and the history is genuine, unshallow and fsck-clean. Final acceptance and any exact-candidate Tier-2 functional qualification are Manager #694's decision. No direct main merge.
