# CCF #329 — exact-candidate independent source review: CTA Roslyn and Workbench receipt — 7 November 2026

**Status:** READY FOR REAL LOCAL PREFLIGHT / NOT RUNNING. **Authority:** [Manager #328 checkpoint and release](MANAGER-328-AUTHOR-HANDOFF-DISPOSITION-AND-329-RELEASE-20261107.md), [Roslyn/receipt decision](MANAGER-327-DISPOSITION-AND-VERIFIER-ARCHITECTURE-20261107.md) and [original bounded author/review contract](MANAGER-327-NEXT-SOURCE-CLOSURE-PACKET-20261107.md).
**Claim:** [#329](https://github.com/jermur788/CCF/issues/329) only; [#328](https://github.com/jermur788/CCF/issues/328) author completion is NOT source approval.
**Role / task class:** independent read-only SOURCE reviewer; PACKET ONLY, exactly two separate G2/G3 judgments.
**Difficulty:** HIGH — compiler semantic identity, fail-closed source analysis, dual-file durable receipt failure under real filesystem errors.
**Best-suited:** **OpenAI GPT-6.1 Sol High in T3 Code** (distinct provider from actual author Claude Sonnet 5.5). If not actually available, choose equally capable **cross-provider** independent local reviewer after disclosing provider and access; never reuse author or claim cross-provider for a same-family substitution. Do not infer model effort; record observable actual setting or UNKNOWN.
**Manager:** #200 is sole live coordination and final approval writer.

## Exact source candidates — DO NOT REPIN TO MAIN

| Review stage | Source branch and new published head | Actual tested source | Governing base |
|---|---|---|---|
| A — CTA | `fix/cta-roslyn-symbol-binding-20261107-v1` at `6adb20318b80e8a81560bc58a63c8bdbfa15d7ca` | `288ba3863793e3775327fb7322dc06af28f7835a` | #325 `c83fae1af4ac1449cba0304e44b0df49b91f8919` |
| B — Workbench | `fix/workbench-receipt-finalization-20261107-v1` at `03b484d10b36382ab86d5e345ece9acac96c9620` | `a4019e6b96b50e03d2b71d92c44fc30a0f7ace29` | #323 `2c4aea1c94a0768c684376bc027ba15c5af5146f` |

All source files at publication heads must match the exact tested commits; only handoff Markdown was added. Do not substitute a different head or treat success at earlier commit as evidence on an altered source.

[CTA author handoff](https://github.com/jermur788/CCF/blob/fix/cta-roslyn-symbol-binding-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/CTA-ROSLYN-PINNED-BINDING-RESULT.md). [Workbench author handoff](https://github.com/jermur788/CCF/blob/fix/workbench-receipt-finalization-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RECEIPT-R2-FINALIZATION-RESULT.md).

## Reviewer lane, ownership and output

**Proposed reviewer-only docs branch:** `docs/verifier-roslyn-receipt-independent-20261107-v1` from currently verified origin/main, never from either source branch.
**Proposed NEW worktree:** `/media/jer/Files/CCF/Operations/CCF-VerifierClosureReview-20261107` using canonical `/media/jer/Files/CCF/Repository.git`. This is conditional, not an existing actual lane.
**Evidence root:** `/media/jer/Files/CCF/Verification/VerifierRoslynReceiptReview-20261107/`, separate disposable clones for the two source heads. All author/source worktrees READ ONLY.
**One exact GitHub result handoff:** `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/VERIFIER-CLOSURE-INDEPENDENT-REVIEW-RESULT.md` on reviewer docs branch, with separate CTA/Workbench verdicts, command/exit inventory, exact refs, scope, limitations and unexecuted Unity gates.

At actual start: read the latest GitHub `Docs/Project/coordination/active-tasks.md` and #329 claim; inspect real branch/HEAD on HTTPS origin, Git common dir, branch/worktree uniqueness, tracked+untracked+ignored cleanliness, unrelated user changes, free space, active owners and no competing review writer. The original registered ZX20 Workbench lane remains protected dirty; no access or changes there. If proposed lane is already claimed or existing branches moved, STOP and report, do not overwrite/rebase/reset/clean anything. Only after real preflight, post #329 `[RUNNING]` receipt with actual model/provider, worktree, branch and starting HEAD. No manager coordination edit by reviewer. Read-only evidence copies from Files are allowed only under ordinary local safety policy; do not mutate retained evidence.

## Stage A — independent CTA Roslyn semantic review

Inspect actual diff from #325 to tested #328 CTA (source additions include C# reporter, Python bridge, reference manifest, tests, README and check 08 binding), rather than relying only on author totals.

Required:
1. Verify actual SDK **8.0.133** pin and SHA-256 of the two Roslyn DLLs, immutable **320-reference manifest** and all copied references; confirm C#9/defines/source set and source-compilation errors are truthfully distinguished from Unity Editor compilation. Check that nonmatching/missing SDK, compiler or refs cannot accidentally become PASS; no hidden network/Unity dependence.
2. Independently inspect semantic resolution, not name-string output: `ScenarioOneUiRoot` base must be metadata `UnityEngine.MonoBehaviour`, the **five** reviewed `Time.frameCount` / `Time.unscaledTime` sites metadata `UnityEngine.Time`, and the **22** reviewed `DisplayStyle.Flex` / `DisplayStyle.None` sites metadata `UnityEngine.UIElements.DisplayStyle` — **27 total**, with fail-closed count and source-identity changes. Check binding checks are actually attached to intended Objective/focus/panel usages rather than an unrelated same-count substitution; flag any meaningful method/site gap, not merely a hypothetical C# parser curiosity.
3. Reconstruct valid C#9 adversarial witnesses **A326-N06**, **A326-N07**, **A326-N08** (tuple field/property/DisplayStyle) and **L326-U01** (otherwise-uncollected global Time) plus **R321-S05/06/07**, run real source checker and assert failure is check **08** for the appropriate bound-symbol or historical lexical reason. Recreate benign valid controls and ensure no over-rejection; invalid C# alternatives do not count as valid false-green tests.
4. Check old 884 negative/112 benign suite, 47+10, historical 6/4/2 red, candidate 10/0/2 green, six genuine SDK8 SnapFar. Reuse valid unchanged evidence only after exact-file comparability; rerun targeted critical checks and the relevant suites once, no unnecessary multiple full matrices.
5. Attempt a small independent set of **high-value** symbol-location/namespace/alias and wrong-SDK/reference/compile-error mutations. Do not open a universal fuzz/parsing project; on a demonstrated novel, in-scope false green return NEEDS CHANGES with exact minimal reproducible witness. Compare the Stage A author's published test-only correction and provenance disclosures; do not treat acknowledged external reference folder as bundled/portable or assert cloud-only PASS when dependency absent.
6. Explicitly assess the known boundary: one player-like Assembly-CSharp model, not Unity import nor Unity assembly graph; tested source set vs all actual production types; static site multiset vs method-specific semantics; runtime behaviour remains NOT RUN.

## Stage B — independent Workbench finalisation review

Review the exact #323-to-#328 source diff of `run_workbench_retirement.py` and `test_runner.py`. Test the protocol against both ordinary valid and faulted destinations without deleting/refactoring author evidence:
1. After initial marker write, surviving file must be conspicuously **PROVISIONAL / INCOMPLETE** with no unqualified bare Editor `_PASS` or clean-status line that could be misread as completed runner PASS.
2. On successful completion require **process exit zero + JSON exit 0/no failures + FINAL marker first line with exactly matching JSON SHA-256**. Check whether helper `authoritative_success` actually rejects absent, stale, mismatched, corrupted or non-final receipts, and whether any deployed consumer relies on the old marker layout. No cross-file atomicity claim.
3. Independently reproduce double-write **simulated ENOSPC** and **real directory/marker permission** failure (use isolated reviewer-owned fixtures; root-permission caveat recorded). Both must produce exit 1 and only an unmistakably incomplete marker, not the #323 false-success marker.
4. Test third/final-write failure: JSON may exist with an earlier successful exit but provisional markers must keep overall result non-authoritative and process return failure. Distinguish raw Editor PASS from runner terminal PASS.
5. Reconfirm earlier **R1** fail-closed after Editor changes to tracked Package/material bytes, **R3** exact terminal PASS/FAIL, **R4** synthetic historical labels, **R5** teaching controls, **13** original runner mocks and **28** projected schema/historical checks. Preserve original frozen/reference/save anchors without running Unity; author-claimed **41/41** is not itself independent evidence. Compare unchanged tests by bytes where possible. The extra `FINAL` and helper-authority controls are within scope.
6. Inspect author's disclosed corrected evidence-driver substitutions: distinguish earlier superseded outputs from final tested source, and do not claim a clean installed Unity runtime or tested actual registered ZX20 lane.

## Verdict and stop rule

Provide **two independent explicit verdicts** using APPROVED / NEEDS CHANGES / REJECTED at **SOURCE-ONLY review level**, and an overall review-execution statement. For each note PASS/FAIL/NOT RUN, exact proof, changed files, old vs new negatives, residual severity, and source-test trust limits. If an in-scope blocker appears, stop for Manager disposition; **do not author a competing fix or a fresh model-specific checker loop**. No manual replacement of the author's source or permission to merge/Unity.

No Unity/Editor/lease/new Library, no editing the protected eight tracked dirty Workbench files, no `Assets/`, `Packages/`, `ProjectSettings/`, `.vscode`, frozen saves/Reference, PR #300/main merge, candidate integration, archive rebaseline or Windows/Player build. #287 and #315 A–O remain blocked regardless of source-review outcome.

At completion publish the exact reviewer handoff on GitHub, post #329 `[HANDED-OFF]` + full link, release reviewer lane, then STOP for Manager #200 G2/G3/G4 decision. A reviewer APPROVED recommendation does not self-approve or integrate either source.

## Copy/paste one-line dispatch — local T3 Code reviewer

```text
CCF independent #329 — review BOTH published #328 source candidates under the exact Manager packet https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-329-EXACT-CANDIDATE-INDEPENDENT-REVIEW-DISPATCH-20261107.md . Act as read-only independent OpenAI GPT-6.1 Sol High reviewer (or disclose actual different cross-provider capable model); inspect live ownership/branches/clean store, mark #329 RUNNING only at real start, verify CTA Roslyn semantics and Workbench receipt finalisation on their exact pinned tested heads, independently adversarial-test with no source edits or Unity, then publish the single GitHub review result VERIFIER-CLOSURE-INDEPENDENT-REVIEW-RESULT.md with TWO separate verdicts, mark #329 HANDED-OFF, and stop for Manager #200. No PR, merge, cleanup, package/save change, Editor, lock or builds.
```

**User action to begin reviewer:** paste the line above in a new local T3 Code session. Publishing this packet alone does not start the reviewer.
