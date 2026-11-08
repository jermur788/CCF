# Manager #200 — #335 completed handback; exact independent #336 review dispatch

**8 November 2026 | Status: #335 COMPLETED AUTHOR HANDOFF ONLY; #336 READY FOR LOCAL PREFLIGHT, NOT RUNNING.** This is a source-only package provenance review, **not** the adoption of Linux package version 1.1.1, a Unity run, permission to touch any protected worktree, or a release.

## Binding authority and reported evidence

- [Manager #332 Option B proposal-only decision](MANAGER-332-DISPOSITION-AND-NEXT-GATES-20261108.md) and [initial author/reviewer scope](MANAGER-PACKAGE-PROVENANCE-OPTION-B-REVIEW-PACKET-20261108.md).
- [#335 source author handback](https://github.com/jermur788/CCF/blob/d39278ef10001f8536b598fef2a7d68864096c92/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/PACKAGE-OPTION-B-SOURCE-PROPOSAL-RESULT.md), [actual author RUNNING](https://github.com/jermur788/CCF/issues/335#issuecomment-6718938318) and [HANDED-OFF](https://github.com/jermur788/CCF/issues/335#issuecomment-6718982744). Actual Claude Opus 5.5, Anthropic; effort High requested but not independently confirmed. Author reports clean new Files-store source lane and release; browser Manager cannot verify physical cleanliness from GitHub.
- GitHub checked the live published branch and the base-to-publication compare: **two commits, three changed paths**. The source-tested commit changes precisely two package files; the publication commit adds the handoff only. GitHub-side Manager separately parsed old/new JSON and measured **3 direct manifest version changes and 7 lockfile values** (three direct package versions, their three sysroot dependency refs and the transitive sysroot version), all **1.1.0 → 1.1.1**. Other parsed package values unchanged; lock graph retains 65 dependency entries. No `Assets/`, `Tools/Verification/`, `ProjectSettings/`, `.vscode/`, Materials, source baseline or original dirty Workbench file in GitHub diff.
- Author's 31/31 static check, deliberately red no-change control, 7/7 genuine-failure mutants and byte-identical backup/log evidence are **author-reported**, not Manager-reexecuted; independent #336 must review raw evidence and tests. The initial mutant script erroneously counted exceptions as kills; the corrected run must be the only positive evidence.
- Earlier [#333 preserved Workbench environment analysis](https://github.com/jermur788/CCF/blob/f1a501b17609c6c61fda7ccbb09ffe628e66d2e1/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/UNITY-PACKAGE-STORAGE-AND-TREE-DIAGNOSTIC-READINESS-RESULT.md) measured the Editor's first 1.1.0 resolve and second 1.1.1 resolve. **Why the Editor triggers the second resolve is still unknown.** No new Unity evidence is claimed.

## Formal Manager disposition

**#335 author execution: APPROVED AS COMPLETED/HANDED-OFF; close its execution-presence claim.** The package source proposal is reviewable but **NOT yet G2/G3 source approved, not canonical package policy and not merged**.

**#336: RELEASED FOR GENUINELY INDEPENDENT SOURCE-ONLY REVIEW.** The reviewer has no author/write authority on #335. Await #336 verdict, then Manager decides package policy; *only after that* consider a separate warm-lane environmental qualification/B3 witness. No extra author pass or speculative package repair before review.

## Exact pinned candidates

| Dimension | Exact identity |
|---|---|
| Starting source-approved Workbench published head | `03b484d10b36382ab86d5e345ece9acac96c9620` |
| Candidate branch | `proposal/linux-sdk-111-provenance-20261108-v1` |
| **Tested package source** | `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` |
| **Published/report head** | `d39278ef10001f8536b598fef2a7d68864096c92` |
| Authored changes | `Packages/manifest.json` and `Packages/packages-lock.json` ONLY |
| Report-only path | `Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/PACKAGE-OPTION-B-SOURCE-PROPOSAL-RESULT.md` |
| Existing author lane | `/media/jer/Files/CCF/Operations/CCF-PackageOptionB-20261108`, **READ ONLY**, author says clean/released |
| Canonical Git store | `/media/jer/Files/CCF/Repository.git` |

**All exact commit identities above are required for the local reviewer; never substitute `main`, the current source branch head if it moves, or a locally inferred history.**

## #336 review assignment

- **Purpose:** independently judge whether the proposed 1.1.1 manifest/lock is an exact, minimal, auditable reproduction of the Editor-observed package rewrite and is suitable for a later Manager package-policy decision.
- **Difficulty:** MEDIUM–HIGH. Package graphs and offline provenance are tractable, but incorrect lockfiles or weak mutation tests can give false confidence.
- **Best-suited model/provider:** **OpenAI GPT-6.1 Sol High, T3 Code/Codex**, genuinely separate from actual Anthropic Claude Opus 5.5 author. If inaccessible, a comparable genuinely **non-Anthropic** independent local code reviewer may substitute with model/provider/effort disclosed. Do not self-review and do not count an exhausted/quota-blocked attempt as completed.
- **Task claim:** [#336](https://github.com/jermur788/CCF/issues/336), **READY / NOT RUNNING** until true local start; the claim is for execution presence, not a source or Unity lock.
- **Reviewer docs branch (NEW):** `docs/linux-sdk-111-independent-review-20261108-v1`, from freshly verified `origin/main` at reviewer admission, not the package candidate.
- **Reviewer worktree (NEW):** `/media/jer/Files/CCF/Operations/CCF-PackageOptionBReview-20261108` under canonical Files Git store.
- **Reviewer evidence (NEW):** `/media/jer/Files/CCF/Verification/CCF-PackageOptionBIndependent-20261108/`. Reviewer-owned disposable clone at exact tested source for offline checks.
- **Mandatory single result:** `Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/PACKAGE-OPTION-B-INDEPENDENT-REVIEW-RESULT.md` on the new docs branch with explicit **APPROVED / NEEDS CHANGES / REJECTED**, exact evidence, limits and handoff to Manager #200.

### Before starting

Refresh live `Docs/Project/coordination/active-tasks.md` (only Manager #200 may edit), #335/#336 GitHub claims, exact origin branch/head identities, Git common store/branch/worktree registrations, owner conflicts, tracked/untracked/ignored clean states, free space, unrelated dirty primary files, and any competing reviewer processes. The original ZX20 Workbench lane's eight tracked dirty files are protected; do not write there or alter its HEAD, index, Library or package cache. If branch/path exists, candidate moves, or ownership is ambiguous, **STOP** rather than rebase, overwrite, clean, stash or silently repin.

After successful actual preflight, **post #336 [RUNNING]** with actual reviewer provider/model/effort-or-UNKNOWN, own branch/worktree, starting main HEAD and exact reviewed source identities. No reviewer change to live coordination or any source lane.

### Required independent checks — proportionate, no Unity

1. Compare base `03b484d1…` to source `46bc1fd6…`, and source to publication `d39278ef…`, verifying precise two-file source delta and report-only publication. Independently parse both package JSON files and enumerate **exactly 10 changed values**, 65 lock entries, package sources/depths and all unchanged hash/registry refs. Fail if extra package dependency drift or an unlisted modified path is found.
2. Independently verify both proposed files byte-for-byte against the Files-side #318 preserved UPM-written dirty backups, their SHA256SUMS manifest, the original tracked HEAD versions, and the two full 65-package registration blocks in the archived #287 `compile.log`. Confirm the first block equals old input and the second equals proposed output; only the four Linux package versions change. Source evidence location: `/media/jer/Files/CCF/Verification/CCF-PackageOptionB-20261108/`; earlier saved backup in `Workbench287MorningClosure-20261107/dirty-backup/Packages/`. **Do not open or modify the registered ZX20 lane for this check.**
3. Independently inspect the revised source-only validator/proof and reproduce the **31/31** completed controls on reviewer-owned evidence, the deliberately red unchanged-source baseline, and a small high-value subset or all **7 mutations** with true checker FAILs. Distinguish actual negative/failure verdicts from Python exceptions; do not credit the author's superseded run that counted exceptions. Avoid unnecessary broad matrices.
4. Confirm source-approved Workbench verification byte identity including postrun R1 dirty-fail provenance and R324-01 PROVISIONAL/FINAL receipt safeguards; no test or safeguard relaxation. The unchanged 41 runner mocks do not need re-execution merely for a package-json proposal; do not claim they ran here.
5. Assess whether any static check incorrectly implies Unity 1.1.1 import/resolve, Linux Player toolchain compatibility, determinism, package immutability or root cause. Report risk of another patch upgrade, differences across registries, potential Linux sysroot/clang/linker impact, the historical ScenarioTwoSpeciesFoundation stage-specific package scope failure, and requirement for future actual verbose UPM logging/stop-on-drift.
6. Explicit **NOT RUN**: Unity Editor/package installation, builds, material rendering, source-gameplay/native UIs, frozen saves, original registered Workbench C–G, integrated A–O. No package-policy approval or Editor/lane permission by reviewer.

If a material package source-proposal flaw is demonstrated, return **NEEDS CHANGES or REJECTED with a minimal reproducible witness**, no source fix or independent-approval shortcut. If supported, recommend **APPROVED SOURCE PROPOSAL ONLY**, not Unity qualification. Publish the exact Markdown handoff on docs branch, post #336 **[HANDED-OFF]** with full GitHub link and release reviewer lane. **STOP for Manager #200**.

## Hard restrictions and future gates

**No Unity/Editor/lease/new Library; no UPM/network resolve or package download; no original eight dirty Workbench paths or protected ZX20 workspace movement/clean/restore; no material edits; no PR #300/merge/main/integration; no frozen Reference Future v1, Legacy40, user saves or baseline rehash; no Windows or Linux Player build.** Future warm-lane reuse is **not approved**. Measured ZX20 free at last #333 run was 21.622 GiB versus 27 GiB cold-entry, 20 GiB STOP and 15 GiB hard floor. B3 runtime pose remains unproven; integrated A–O 0/15.

## One-line copy/paste dispatch for user to send into local T3 Code

```text
CCF #336 — execute genuinely independent READ-ONLY package-provenance source review from the binding Manager handoff https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-335-HANDBACK-AND-336-INDEPENDENT-REVIEW-RELEASE-20261108.md . Use OpenAI GPT-6.1 Sol High through T3 Code if available (actual reviewer must be independent of #335 Anthropic Claude Opus 5.5); verify latest ownership, canonical Files Git store, unique clean docs branch/worktree, and exact #335 base/tested/published source, then mark #336 RUNNING only after actual admission. Independently check 1.1.0→1.1.1 manifest/lock 10-value delta, preserved UPM-written backups and both 65-package compile-log snapshots, unchanged Workbench checks, and genuine fail-closed proof controls. No Unity/Editor, registered ZX20 lane access/write, package downloads, source fixes, material changes, integration, PR #300, cleanup or builds. Publish the exact PACKAGE-OPTION-B-INDEPENDENT-REVIEW-RESULT.md on the docs branch with explicit source-only verdict, mark #336 HANDED-OFF, release the lane and STOP for Manager #200.
```

**Exact user action:** paste the above into a new independent local T3 Code review session. GitHub publication does not start a worker.
