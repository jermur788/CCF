# CCF #892 — Manager review of browser redundancy research; Files-first conditional preflight

**Date:** 2026-11-20.  
**Disposition: ACCEPT AS RESEARCH INPUT / RESEARCH COMPLETE / CORRECTIONS NOTED / NO PHYSICAL CLEANUP AUTHORITY.**  
**Reviewer:** ChatGPT GPT-6 via GitHub connector; independent of actual #892 research author.  
**Actual research provider:** DeepSeek V4.1 Flash (T3 Code / OpenCode GitHub connector), disclosed in deliverable.  
**Scope:** read-only source/issue/TSV review, no host filesystem, Unity, Git worktrees, branch integration, deletion or archive.  
**Owner decision:** no exact-path deletion approval in this conversation. This review does not make any candidate SAFE-TO-DELETE.

## 1. Exact material inspected

- [#892 report](https://github.com/jermur788/CCF/blob/docs/ccf-892-browser-redundancy-review-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/892-browser-redundancy-review/CCF-892-BROWSER-REDUNDANCY-EVIDENCE-AND-DISPOSITION.md), fetched blob `dfd9d36796f9188272dec8815d945d2208c2a635`.
- [#892 306-row decision TSV](https://github.com/jermur788/CCF/blob/docs/ccf-892-browser-redundancy-review-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/892-browser-redundancy-review/CCF-892-BROWSER-REDUNDANCY-DECISIONS.tsv), fetched blob `0f0dd8a68a1b528cbc82126d324f6d360627d83b`.
- Historical 306-row snapshot [source register](https://github.com/jermur788/CCF/blob/docs/ccf-all-drives-inventory-report-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/CCF-ALL-DRIVES-INVENTORY-REGISTER-20261120.tsv), original blob `6f6615877ea242ec07930ab87b5b9e41f6aa98ef`, and [source report](https://github.com/jermur788/CCF/blob/docs/ccf-all-drives-inventory-report-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/CCF-ALL-DRIVES-READONLY-INVENTORY-REPORT-20261120.md), blob `1f19c1d88d9f1bf6f8ea9baf108a7d12f0d04f43`.
- Branch `docs/ccf-892-browser-redundancy-review-20261120-v1` compared to its cited main base `e2226a252ff9b0ad016b4aa4c27552a1b33af1d9`: ahead2/behind0, exactly one added TSV (307 lines) and one added Markdown. No game source changes.
- Live issue checks #195, #205, #213, #254, #269, #313, #332, #363 are all **CLOSED**; #287, #382, #447, #652, #802 remain **OPEN**. These are source-task status checks, not evidence of physical path release.

## 2. Accepted findings and boundaries

1. **Date reconciliation:** strict Tier A Files = 12 paths / approximately 14.67 GB using cutoff before 6 Nov; a filter before 7 Nov includes seven 6-Nov paths / ~8.67 GB, giving 19/~23.34 GB. Distinguish age eligibility from safety.
2. Re-evaluation retains/defers five of strict 12, including named `CCF-NonUnity`, `CCF-OpsUnityLock01`, `CCF-ContextSync`, `CCF-S1D-Spec`, `CCF-S1D-T02`. These must not be casually retired.
3. The source-checked *research* shortlist has **51 rows / ~62.74 GB**, comprising **45 Files / 51.27 GB**, **6 ZX20 / 11.48 GB**. TSV category totals exactly 306: 51 RESEARCH-CLEARED/LOCAL-PREFLIGHT-REQUIRED; 129 NEEDS-GITHUB-FOLLOWUP; 72 HOLD; 37 OWNER-DECISION; 17 NOT-ASSESSED. Cleared candidates have no `SAFE-TO-DELETE` claim. The report correctly excludes nested aggregate double-counting.
4. Register issue-regex artifacts are usefully identified. Preserve exact path identities and report all provenance corrections.
5. No deletion, archive, Unity work, local safe preflight, or branch merge was performed by #892. The prior historical #385 exact-ten approval does NOT extend to any current candidate. #802 native one-orchestrator authority remains separate.

## 3. Corrections and qualified confidence

**A. Do not call all 51 paths fully retired in GitHub.** Direct TSV inspection finds **39 of the 51** with `current_issue_status = NO ISSUE NUMBER IN PATH/BRANCH`. These may be legitimately obsolete (some workstreams have closed aggregate issues), but the TSV does not link each one to a closed controlling issue or explicit owner release. **All 51 `closure_evidence_url` values are commit URLs**, with **zero GitHub issue closure URLs**. A reachable commit proves source recovery potential, not task retirement or absence of unique ignored files. Remedy is a small per-workstream closure mapping for any subset intended for native preflight, rather than redoing 306 audits. The 51 count remains a *candidate ceiling*, not an actionable removal batch.

**B. Correct #802 overlap discrepancy.** The TSV flags five rows `overlap_802 = YES`, but **`CCF-INV-0125` (`CCF-OpsLeaseFix-20261115`) is `NEEDS-GITHUB-FOLLOWUP`**, not HOLD; the report §9 says all five are HOLD. This particular row is **not** in the 51 candidates, so no immediate unsafe inclusion occurred. For any physical action it must remain non-removable and be reconciled under #802, preferably mark HOLD until sole #802 orchestrator's exact grant.

**C. Correct the follow-up arithmetic.** Report §7 describes 129 NEEDS-GITHUB-FOLLOWUP as `37 legacy-store + 86 current-week`, summing to **123**, leaving **6 unaccounted** in the prose. The TSV's actual total is 129. Publish a precise component explanation if the categories are used for workload/approval decisions. This is not evidence of deleted/missing data.

**D. Recovery references:** all 51 TSV `remote_recovery_ref` strings include branch and expected HEAD, and commit URLs exist, but a remote commit object is not itself proof that every *named branch* remains remotely available or that the local worktree has no unique ignored/untracked evidence. Native preflight should pin a full reachable SHA and use a safe reconstruction recipe, not assume a branch name alone will restore a particular checkout.

**E. Report timestamp:** the report header claims collection through `22:50 UTC`, which is after the owner's 22:21Z completion message. This is a provenance discrepancy to reconcile with GitHub commit timestamps; it is not a filesystem or cleanup safety finding.

These corrections merit a bounded report erratum before using cohort-wide metrics as a deletion authority; no repeated mass scanning requested.

## 4. First conditional Files preflight target — exact seven only, NOT deletion approval

The first group is 7 paths / approximately 8.62 GB, all under `/media/jer/Files/CCF/Operations`. Full source ID, HEAD and branch are already present in #892 TSV. Their respective closed workstream issues (verified at this review) provide a reasonable case for **checking**, not deleting:

| Source row | Folder basename | Approx GB | Workstream lineage to confirm at native preflight |
|---|---|---:|---|
| `CCF-INV-0128` | `CCF-Overnight-FeedbackVerify-20261104` | 1.232 | Overnight #254/#269/#313/#332/#363 (all closed); prove exact lane closure |
| `CCF-INV-0129` | `CCF-Overnight-ObjectiveCopy-20261104` | 1.233 | Same; prove exact lane closure |
| `CCF-INV-0131` | `CCF-Overnight-RackHandle-20261104` | 1.233 | Same; prove exact lane closure |
| `CCF-INV-0132` | `CCF-Overnight-UIQualification-20261104` | 1.233 | Same; distinguish **different path** from old owner-unconfirmed ZX20 `CCF-UIQualification-20261104`, which stays protected |
| `CCF-INV-0102` | `CCF-MFM08Closure` | 1.230 | #195/#205 closed; prove exact lane closure |
| `CCF-INV-0103` | `CCF-MFM08Qual` | 1.231 | #195/#205 closed; prove exact lane closure |
| `CCF-INV-0104` | `CCF-MFM10Residual-20261101` | 1.232 | #213 closed; prove exact lane closure |

**Native investigation allowed only after separate authorisation of a READ-ONLY PREFLIGHT task**, not assumed by this report. Verify every exact folder and SHA against new live task ownership/board; frozen user/Unity evidence and saved-game data; ignored/untracked content; no cwd/FD/lease/Git writer; exact mount UUID/realpath; canonical remote reconstructibility. No `git worktree remove` or `rm` may occur without an additional **exact-path owner deletion grant**, after seeing that preflight's findings. #802 overlapping scope must be reconciled at that later gate. Unknown = RETAIN. This review gives **zero** physical access or deletion authority.

Next (if first preflight succeeds and owner still wants relief): 7 November-6 review checkouts / 8.67 GB, then only evidence-confirmed ranks 3–4. ZX20 not urgent due substantial reported free space. Do not turn this into a T3, build, evidence-retention, legacy Git or Unity cleanup programme.

## 5. Management verdict and next action

**Verdict: ACCEPT RESEARCH WITH ERRATA; APPROVE FOR A NARROW EXACT-SEVEN READ-ONLY PREFLIGHT PROPOSAL ONLY.** Do not represent the 51 or 62.74 GB as globally SAFE or free-space recovered. No physical worker assigned/started, no new lane/lock claimed. A future separate native worker may be tasked to check seven exact paths after #425/user admission; no deletion today. Avoid slowing current owner-playable Compact30 #890/#891 work with broad process changes.

**Proposed direct handoff** for a subsequent approved worker: https://github.com/jermur788/CCF/blob/docs/manager-892-redundancy-review-verdict-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/CCF-892-MANAGER-RESEARCH-REVIEW-AND-FILES-FIRST-PREFLIGHT-DISPOSITION-20261120.md
