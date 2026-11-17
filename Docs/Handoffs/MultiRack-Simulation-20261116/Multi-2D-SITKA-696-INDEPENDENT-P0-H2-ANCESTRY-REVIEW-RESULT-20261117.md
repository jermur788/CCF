# Multi #696 — Independent read-only review of #692 P0 H2 ancestry candidate: RESULT

**Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness via T3 Code, host `jer-laptop`. A different provider from the #692 writer (OpenAI Codex GPT-6.2 Sol Medium) and the #687 author (DeepSeek). I did not write or review any part of #692.
**Date:** 2026-11-17, review window 20:11Z–20:20Z.
**Packet:** [#696 custody-gate packet](https://github.com/jermur788/CCF/blob/docs/multi-694-692-independent-review-gate-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-696-INDEPENDENT-CURRENT-MAIN-P0-H2-REVIEW-CUSTODY-GATE-20261117.md).
**Target:** H2 `ce7bfeb6b439d935b2e00094f79338d3b8a658a0`, M `5b858031e2cd79be148864a62692653548e8b4af`, Q `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, P0 tree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`.
**Recommendation: APPROVE.** I checked the narrow ancestry and source-identity claim only. This review is evidence. It is not Manager acceptance. Unity, .NET, P1 and native behaviour were NOT RUN; the packet does not require them.

## 1. Custody and read-only grant readback

- **Gate state at start:** issue #696 and the packet were BLOCKED. No custodian grant had been recorded.
- **Grant:** the user, acting as physical Ops custodian, gave a direct instruction at about 20:10Z (answering an explicit question in this session). It grants this Claude Opus 5.5 session on `jer-laptop` **READ-ONLY** access to:
  - store `/media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/git/CCF-687-Integration.git`;
  - checkout `…/ccf-687-child-a` and `…/receipts/`;
  - evidence `/media/jer/Files/CCF/Verification/CCF-687-P0P1-Integration-20261117/candidate-a/`.
  
  The grant expires **2026-11-18T18:00:00Z**. I recorded and read it back on [#696 comment 6959632119](https://github.com/jermur788/CCF/issues/696#issuecomment-6959632119), together with [RUNNING]. The #692 writer lease was not transferred or borrowed. The #674/#683 leases were not touched.
- **Writer lease readback:**
  - The deployed board row `CCF-687-P0P1-INTEGRATION` is `VERIFY`. Its owner is "OpenAI Codex GPT-6.2 Sol Medium (sole Child A writer; local proof complete; retained for independent review)", with HEAD `ce7bfeb6…`. The lane name `MultiP0P1Integ687` appears once on the board.
  - The receipt `CCF-MULTI-692-custodian-writer-assignment-20261117.json` has sha256 `6eeda450…0b33`. This equals the "receipt" hash in #692's `proof-complete-custody.txt`, so the receipt is unchanged. Its expiry is 2026-11-19T18:00:00Z.
  - The whole board file hash is now `6e9d4c8a…56a6`, against `b428c193…41c4` at #692 proof completion. Other rows changed after #692 (for example the #693/#695 coordination). The #687 row content above is the #692 proof-complete state.
- **Processes:** no git, dotnet or Unity process was running on the lane at start.
- **Non-mutation proof:** before and after the review I took a size+mtime listing of every file under the store, checkout, receipts and evidence (6,002 lines). Pre and post sha256 were both `6b91049529284695a4c2eeadc5b958109652dc64804b9d9b2bca5e15cd222e8f`, so the listings are byte-identical. All Git commands ran with `GIT_OPTIONAL_LOCKS=0 GIT_NO_REPLACE_OBJECTS=1`. I ran no fetch, ref, index, worktree, config, gc, test or build command against the source.

## 2. Results by criterion

`G` = `git --git-dir=<isolated store>`; `C` = `git --git-dir=/media/jer/Files/CCF/Repository.git` (canonical store, read-only).

| # | Criterion | Result |
|---|---|---|
| 1 | Parents ordered M then Q; Q is a genuine ancestor; history is not shallow; no grafts, replace refs or alternates; commit objects are original | **PASS** |
| 2 | L0: zero commits in Q..H2 touch P0. L1: P0 tree pinned, and all 52 blob IDs and modes match between Q and H2 | **PASS** |
| 3 | First-parent diff is exactly 52 `A` rows under P0; the 5,862 other entries are identical in M and H2; the 11 Q-era docs/specs files and P1 are excluded | **PASS** |
| 4 | #692 logs, commands, protected snapshots and `SHA256SUMS` are consistent | **PASS** (with the limitations in §4) |
| 5 | H2 branch is local only; live main drift creates no P0/P1 or authority collision | **PASS** |
| 6 | No unrelated programme rerun | **PASS** (none run) |

### C1 — Parentage and genuine history
```
G cat-file -p H2          → tree 247730da…; parent 5b858031… (1st); parent a7f41983… (2nd)
G rev-list --parents -n1  → ce7bfeb6… 5b858031… a7f41983…
G merge-base --is-ancestor Q H2 → exit 0 ; M H2 → exit 0
G rev-parse --is-shallow-repository → false
info/grafts, objects/info/alternates → absent; G replace -l → empty
G for-each-ref → only refs/heads/integrate/multi-p0-q-current-main-20261117-v1 ce7bfeb6…
G config -l --local → no remote.* entries (no origin)
G cat-file commit X | git hash-object -t commit --stdin → recomputes to M, Q, H2 exactly
G vs C: cat-file commit M and Q → byte-identical to the canonical store
rev-list counts: H2 2039 = |M∪Q| 2038 + 1; M 2012, Q 1941, M∪Q 2038 match canonical; the sorted rev-list of M∪Q is identical to canonical; single root 845accd1…
G fsck --full --strict --no-dangling --no-reflogs → exit 0 (only notice: bare HEAD → unborn main)
```
H2 is absent from the canonical store and from Primary (`cat-file -t` fails), which is consistent with it being local only.

### C2 — L0 and L1
```
G log --oneline Q..H2 -- Tools/MultiRackSitkaLab | wc -l → 0
G rev-parse Q:Tools/MultiRackSitkaLab, H2:Tools/MultiRackSitkaLab → f212c8f6… both
diff <(G ls-tree -r Q -- P0) <(G ls-tree -r H2 -- P0) → identical; 52 entries, all mode 100644 blob
ls-tree listing sha256 daf30c20…e4ed
```

### C3 — Only 52 additions; 5,862 other entries unchanged
```
G diff --no-renames --name-status M H2 → 52 rows, 52 × A, 0 rows outside Tools/MultiRackSitkaLab/
G ls-tree -r --full-tree M → 5862 entries (0 under P0); H2 → 5914 = 5862 + 52
cmp M.tsv H2-minus-P0.tsv → identical (all mode/type/blob/path rows, so all absences match too)
```
- **Q-era exclusion:** `diff merge-base(M,Q)..Q` outside P0 gives exactly 11 `A` rows. These are 6 under `Docs/Handoffs/MultiRack-Simulation-20261114/`, 1 Post-Save HTTP repair handoff under `…-20261116/`, and 4 under `specs/multirack-sitka-2d-reference-lab-v1/`. All 11 are **absent in H2**, and also absent in M. This list matches #692's `excluded-plain-merge.tsv`.
- **P1 exclusion:** `Tools/MultiRackSitkaP1Lab` is absent from H2. The P1 directory tree `2c017956…` appears nowhere in H2's tree, and P1 commit F `e302f15b…` is absent from the isolated store.
- **Independent cross-check:** my own M and H2 non-P0 listings are byte-identical to #692's `5b858031….non-P0.tsv` and `ce7bfeb6….non-P0.tsv`.

### C4 — #692 evidence consistency
- `sha256sum -c SHA256SUMS` in `candidate-a/` passes **46/46 OK**. The 46 entries cover every file in the directory except `SHA256SUMS` itself.
- `construct-and-verify.sh` and its `set -x` trace `construction-and-proof.log` (21,921 B) show this sequence:
  1. Preconditions: RUNNING readback, lease expiry, board row, non-shallow, no replace/grafts/alternates/Git env overrides, merge-base `fc18190b…`, Q tree `dfbf5f9d…`, P0/P1 absent at M, four source pins, `fsck` of M and Q, `rev-list --missing=error`.
  2. `merge-tree` preview, then the 11-row exclusion assertion.
  3. `worktree add -b <branch> … M`.
  4. `merge -s ours --no-ff --no-commit Q`, then `restore --source=Q -- Tools/MultiRackSitkaLab`.
  5. Staged-equals-52-A assertion, then `commit`.
  6. The parent, L0, L1, first-parent, non-P0 and P1-absent checks; `fsck H2`; clean status; protected after-snapshot diff; refs listing; `ls-remote` empty.

  The log contains no error, fatal or warning lines. The writes it records are limited to the authorised isolated-store fetch (`fetch.log`: `fetch.fsckObjects=true --no-write-fetch-head` of exactly M and Q), one worktree, one branch and one commit. No push appears.
- **Order of events:** RUNNING was posted on [#692 comment 6959288923](https://github.com/jermur788/CCF/issues/692#issuecomment-6959288923) (`running-readback.md`, 19:57). The fetch came after it (`fetch.log`, 19:59). The store preflight (`store-preflight.log`) shows 0 objects, 0 refs and no worktree before the fetch, and the branch was absent from origin and from the canonical and legacy stores.
- **Protected state:** `protected-before.txt` and `protected-after.txt` are byte-identical (`cmp`). Both cover the show-ref hash, worktree-list hash and object counts of Repository.git, CCF-Legacy and the #674 store, plus HEAD, status, diff and untracked-file hashes of Primary, the #674 worktree and the #639 worktree.
- **Other evidence values** match my independent results: `parents.txt`, `H2.txt`, `L1-tree.txt`, empty `L0.txt`, empty `replace.txt`, empty `candidate-status.txt`, and `M-source-pins.tsv` (4/4 pins as in the receipt).

### C5 — Local only; live main drift
```
git ls-remote origin refs/heads/integrate/multi-p0-q-current-main-20261117-v1 → none
canonical and CCF-Legacy for-each-ref of that branch → none
origin/main at review = b31f25bf5e00dd85d55d22507c5853b56ef06f44; M is on its first-parent line
git log M..origin/main → 6 commits (7984bcc9, 62fa1cbf, 43684ec8, e80de308, 55422ee4, b31f25bf)
git diff --name-status M origin/main → only "M Docs/Project/coordination/active-tasks.md"
origin/main ls-tree Tools/MultiRackSitkaLab Tools/MultiRackSitkaP1Lab → absent
```
At #692 preflight there were 5 drift commits (`main-drift.json`). The sixth is `b31f25bf`, which records the #692 handoff and the #696 gate. All six touch only `active-tasks.md`. H2 touches only `Tools/MultiRackSitkaLab/`, so the two file sets are disjoint and there is no P0/P1 path collision. M is unchanged and the integration base has not moved. I found no authority collision either: the e80de308 safe-stop record (writer was GPT-6.2, not the assigned GPT-6.1) is superseded by the direct-user reassignment recorded in the receipt and in #692 RUNNING.

## 3. Findings

- **Blocking:** none. **Major:** none.
- **A-1 (advisory, design note):** H2 is a deliberate restricted merge (`-s ours`, then the P0 restore). Git therefore records Q as fully merged and treats the 11 Q-era docs/specs files as intentionally dropped on the main line. Later merges with merge base at or after Q will not re-propose them. If those records are wanted on main, they must be added by a separate explicit docs change. This matches the packet's intent; I am only stating the consequence.
- **A-2 (advisory, for Child B):** P1 commit F `e302f15b…` descends from Q. `Q..F` is 35 additions and 1 modification, and none of them touch P0 or the 11 excluded files. A plain merge of F would bring in every one of those 36 changes, not only the 27-file P1 directory. Child B therefore needs the same restricted-merge technique and its own exact-scope assertions.
- **N-1 (note):** the board file hash has changed since #692's proof-complete record because of unrelated rows. The #687 row and the receipt are unchanged.

## 4. Limitations

- I did not independently audit the whole machine or checksum unrelated worktrees. My protected-state claims for those come only from #692's before/after snapshots and the scope of `protected-snapshot.sh`. That scope does not include ZX20 worktrees or stores other than the three listed.
- My own non-mutation proof compares sizes and mtimes. It does not compare content hashes of every pack file, and it does not cover atime.
- I verified H2's absence from origin by ref name only.
- No .NET, Unity, security, UI, year or P1 tests were run, as the packet requires.

## 5. Verdict

**APPROVE** — #692 H2 `ce7bfeb6b439d935b2e00094f79338d3b8a658a0` genuinely descends from M (first parent) and Q (second parent). It changes exactly the 52 qualified P0 files, which are byte-identical to Q, and changes no other current-M entry. The evidence is complete, hash-consistent and in the right order. The candidate is still local and unpushed. Next step: Manager #694 issues the disposition. P1, current main, default L4 FAIL 38 and first ecological year HOLD are unchanged by this review.
