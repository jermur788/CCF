# Multi #730/#841 — exact qualified source published to an isolated review branch (publication receipt)

**19 November 2026 · PUBLISH-ONLY RECEIPT · NO CODE EDIT · NO MERGE · NO REBUILD · REVIEW-ONLY REF**

Publisher: **DeepSeek V4.1 Flash** (`opencode-go/deepseek-v4.1-flash`, variant `high`), T3 Code / OpenCode, Windows host driving WSL2 Ubuntu, **desktop internal disk only**. No ZX20, no external volume, no Unity, no .NET build, no Windows host change.

This receipt exists because #841 and the #730 verdict could only cite Markdown assertions. The exact C# candidate is now fetchable as Git objects so Manager #730 can review real code.

---

## 1. Direct review artefacts

| Item | URL / identity |
|---|---|
| **Review branch** | https://github.com/jermur788/CCF/tree/review/multi-841-g2-qualified-engine-8741a328-20261119-v1 |
| **Exact commit** | https://github.com/jermur788/CCF/commit/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6 |
| Ref name | `review/multi-841-g2-qualified-engine-8741a328-20261119-v1` |
| Commit tree | `a5f5fa313b4781a47db6ba2584b935c95effebfb` |
| Engine subtree | `bf4b74395c3c92ebcaa52ff83d1bfe553b4411c4` |

Clickable source at that exact commit:

* [WorkContract.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/src/Core/WorkContract.cs)
* [AnnualContinuation.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs)
* [AnnualEngine.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/src/Core/AnnualEngine.cs)
* [Program.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs)
* [WorkContractTests.cs](https://github.com/jermur788/CCF/blob/8741a32843b739b0587e4c6f3a3ee7fc890ea7f6/Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs)

There is **no pull request** and **no merge**. The ref is a review transport only.

---

## 2. What was published, and from where

| Item | Value |
|---|---|
| Local lane (read only) | `/home/jer/CCF-Dev/Multi-20261119/g2-fell-writer` |
| Lane branch | `feature/multi-841-g2-atomic-fell-year-20261119-v1` |
| Lane state | clean, `git status --porcelain` empty |
| HEAD **before** push | `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6` |
| HEAD **after** push | `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6` (unchanged) |
| Push target remote | `github` → `https://github.com/jermur788/CCF.git` (verified before writing) |
| Pre-existing ref | **none** — branch was absent, so the update was a create, not a force or overwrite |
| Commits added above the anchor | 2 (`be9b40c5` admission, `8741a328` candidate) |

Nothing was amended, rebased, cherry-picked, reset or rebuilt. The published commit hash is byte-identical to the internally qualified candidate.

---

## 3. Remote readback (from GitHub, not from the local clone)

| Check | Result |
|---|---|
| Branch HEAD via GitHub API | `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6` |
| Commit tree | `a5f5fa313b4781a47db6ba2584b935c95effebfb` |
| `Tools/MultiRackSitkaP1Lab` subtree at that commit | `bf4b74395c3c92ebcaa52ff83d1bfe553b4411c4` |
| Source readable at the ref | yes — `WorkContract.cs` returned 6,050 bytes, blob `79cf27e56d5a289cc6b1598761d88562f8dea9e1` |
| `be9b40c5...8741a328` compare | **5 files, 1 commit, status `ahead`** |

---

## 4. Per-file comparison — admission `be9b40c5` → candidate `8741a328`

Exactly the five expected paths; blob identities returned by GitHub:

| Status | Path | Blob |
|---|---|---|
| modified | `Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs` | `63c41542e875581c7ae75b41746b05e3960d7b54` |
| modified | `Tools/MultiRackSitkaP1Lab/src/Core/AnnualEngine.cs` | `625a3b44a9b1e8c9276ff5b0ee8c7b57c10f8738` |
| added | `Tools/MultiRackSitkaP1Lab/src/Core/WorkContract.cs` | `79cf27e56d5a289cc6b1598761d88562f8dea9e1` |
| modified | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs` | `86952c504dcb64d3bf0da971eaea6b4b557c2a71` |
| added | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs` | `6082a38ecfe2a2b8f6d7c537e48a6a16c7bc6033` |

**Unchanged and confirmed absent from that delta:** `Assets/**` (0 files), `Tools/MultiRackSitkaLab/**` P0 inputs, the derived fixture and its content hash, `AnnualRules.cs`, the ten golden digest literals in `ContinuationTests.cs`, and `.p0ref/**`.

Shared-input hashes, restated for the reviewer:

| Input | Identity |
|---|---|
| `Assets/ForestPrototype/SitkaGrowthModel.cs` | blob `e4c73402d46bb3b75a9cba2e4622857a913fac92` |
| `Assets/ForestPrototype/SimulationRandom.cs` | blob `bb83d7f8468002ab13a6aec6caf548e0bf3ad34b` |
| P0 subtree | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| P0 binary reference | sha256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (not tracked; gitignored) |
| Derived fixture | sha256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` |
| Anchor patch (verification reference only) | sha256 `7791b9ffefaa8f0284450d694bae6f8f7f8c13deae4f3f27a9563860091d07fe` |

---

## 5. Honest disclosure: the branch is **not** only five files relative to today's `main`

The packet required this to be stated rather than glossed.

* The public anchor `a7f41983…` is on a line that **historically diverged from `main`**. The merge base of `main` and the candidate is `fc18190b976f1363f1b5d1b0881810db2f8c6e39`, not the anchor.
* Consequently `main...review-branch` reports **97 files / 28 commits, status `diverged`**.
* Breakdown of the **candidate-side** delta since that merge base: **52** files under `Tools/MultiRackSitkaLab` (the P0 qualification tree), **34** under `Tools/MultiRackSitkaP1Lab` (the #789 engine + the five-file G2 delta), **7** docs and **4** specs. **0** `Assets/` files.
* `main` meanwhile moved **188 commits / 10 files** since that same merge base.

So: the only change attributable to this task is still the five paths in §4. The rest is pre-existing historical public-line content required to make the engine build and its P0 oracles checkable. No `Assets` file is introduced or altered by the branch.

---

## 6. Publication safety checks

* No `.dll`, `.exe`, `.pdb`, archive, key, certificate or credential-shaped content is tracked in the published subtree; `.p0ref/` and `bin/`, `obj/`, `TestResults/`, `evidence/*` remain gitignored.
* A grep for private-key blocks and common token formats across the published subtree returned nothing.
* The branch was created by pushing one exact commit; no other local branch, worktree or dirty file was pushed.
* The user's Unity checkout was not touched.

---

## 7. Status and remaining gates

**Status: PUBLISHED FOR REVIEW ONLY — NOT APPROVED, NOT INTEGRATED.**

* Source publication does **not** clear the Scenario One **V-01** source-owner half. That remains outstanding at [#844](https://github.com/jermur788/CCF/issues/844#issuecomment-7023892548). Publishing already-committed text unchanged to a review ref under the owner's instruction does not authorise reuse, modification or native rebuild.
* **A-1** (legacy `Advance(year, revision)` lacks a work-revision fence) is **not** fixed here and remains a separate PACKET-ONLY repair on a new commit with independent delta verification.
* **#804** remains PARKED / main-BLOCKED. Historical V-01/V-02 variance and **L4 FAIL38** remain unwaived.
* No UI, G1 multi-area, save, economy, Host or Unity grant is implied.
* No merge to `main`, no PR, no deployment.

**Outstanding action:** Manager #730 reviews the code at the ref above and issues a source-visible delta verdict, preserving `8741a328` as the immutable qualified baseline.
