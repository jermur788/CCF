# Manager #200 — #383 residual addendum verdict and conditional documentation-worktree ownership ruling

**Date:** 11 November 2026, following the local operator's 23:25 read-only return.  
**Verdict:** **APPROVED AS NEW PARTIAL EVIDENCE; NO DELETION APPROVED.**  
**Claim:** [#383](https://github.com/jermur788/CCF/issues/383) remains **OPEN / PARTIAL / NOT RUNNING**.  
**T3 Fork Integration HOLD:** still binding. All CCF personal player-test lanes, original saves/Prefs, licensed ignored assets, frozen Reference and shared Git stores remain protected.

## Verified published evidence and limitations

- [Sol residual owner, Builds and ten-worktree report](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-RESIDUAL-WORKTREE-BUILD-OWNER-CLOSURE.md).
- [Sol exact residual candidate TSV](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-RESIDUAL-CANDIDATE-DECISIONS.tsv).
- [Earlier CCF partial queue](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-CCF-ONLY-CLEANUP-APPROVAL-QUEUE.md) and [Manager disposition](MANAGER-383-PUBLISHED-CCF-PARTIAL-APPROVAL-QUEUE-DISPOSITION.md).

Manager fetched/read both residual GitHub reports and separately checked the latest live coordination and actual #282/#298 task status. The worker reports source-byte GitHub readback; Manager did **not** independently inspect physical disks or run local Git. All on-machine sizes, process and status checks remain worker-observed.

### Measured observations

At 23:23 UTC: Linux root 19,818,078,208 bytes available, Files 182,951,108,608, ZX20 26,788,233,216. No Unity process observed. These observations **do not establish no editor/owner lock**. The space figure that matters for B1 cleanup is the **Files** ext4 volume; it does **not** free ZX20 or Linux root.

## B1 — ten clean docs-only worktrees: Manager ownership ruling

The operator screened **ten distinct documentation worktrees** under `/media/jer/Files/CCF/Operations/`, summing to **12,330,786,816 allocated bytes**, all on Files. At the time of the audit, each had 0 tracked modifications, 0 staged/untracked, 0 ignored, no process cwd in the directory, a resolvable local commit and a GitHub branch containing it. Nine remote heads equal the checkout; the ObjectiveGuidance local head was contained by a later remote head. **This is credible evidence of source-level reconstruction, not a proof of future owner release or a guarantee of exact warm-cache state.**

The ten exact B1 paths are:

| ID | Worktree leaf under `/media/jer/Files/CCF/Operations/` | Observed allocated bytes |
|---|---|---:|
| B1-01 | `CCF-AccessSpineDocs` | 1,231,380,480 |
| B1-02 | `CCF-MFM-TopologyDocs-20261103` | 1,231,507,456 |
| B1-03 | `CCF-Overnight-InputSpec-20261104` | 1,231,679,488 |
| B1-04 | `CCF-Overnight-ObjectiveSpec-20261104` | 1,231,695,872 |
| B1-05 | `CCF-Overnight-MarkSpec-20261104` | 1,231,699,968 |
| B1-06 | `CCF-MFM-OptionA-Docs-20261104` | 1,231,712,256 |
| B1-07 | `CCF-MFM-A-vs-C-Docs-20261104` | 1,232,138,240 |
| B1-08 | `CCF-Overnight-DPC-20261104` | 1,231,953,920 |
| B1-09 | `CCF-Overnight-GuidanceReview-20261106` | 1,238,474,752 |
| B1-10 | `CCF-D072-SpecReview-20261106` | 1,238,544,384 |

**Manager ownership policy for these ten, not for other worktrees:**

- For docs-only worktrees with a **closed/handoff claim and no live single-writer ownership/lease**, Manager **conditionally waives additional individual author acknowledgement**. This is a governance ruling, not a claim that every one of these ten has a positively identified former owner. Manager directly verified [#282](https://github.com/jermur788/CCF/issues/282) and [#298](https://github.com/jermur788/CCF/issues/298) are closed and their corresponding ObjectiveSpec and D072 SpecReview lanes are described as handed off. For the **other eight**, ensure **at the actual future cleanup time** that no current active-task owner, worker session or lock claims the path, and that it remains docs-only and clean; otherwise stop that item for owner confirmation. Absence of a process alone does not waive a lock.
- **Shared Git metadata is protected.** Manager does **not** authorise changes to `Repository.git/worktrees` now. If the user separately approves the **exact B1 paths**, then Manager may dispatch an isolated, single-writer cleanup execution with the narrowly scoped authority to remove **only those approved worktrees** using `git worktree remove <exact path>` **without `--force`** (which necessarily updates registered worktree metadata). No `git prune`, `git gc`, bare Git objects deletion, branch deletion or force cleanup. A final preflight must reverify each worktree identity, current ownership/lock, branch+HEAD availability, remote refs, clean tracked/staged/untracked/ignored, no unique artefacts and restoration recipe. Stop individually on any mismatch; never work around refusal by manual directory deletion.
- **Rollback plan:** branch and exact source commit remain on the remote; recreate a clean docs worktree from its saved branch and remote objects in a new empty path under proper ownership. This restores Git-tracked source, not necessarily transient environment caches. Do not assert exact-byte restoration of a cache without evidence.
- The amount is **a conditional Files-space ceiling**, not guaranteed filesystem reclaim and not **approved-to-delete**. Current approved deletion: **zero**. Files has ample free capacity relative to ZX20, so do not prioritise B1 as an emergency root-space recovery.

**Result:** B1 is **conditionally eligible to be presented for explicit user approval**, but NOT APPROVED FOR EXECUTION. If the live-owner recheck cannot be guaranteed, defer individual rows; nothing authorises the other 142 Main/Legacy worktrees.

## Z1/Z2 older ZX20 Unity Libraries

Z1 `/media/jer/ZX20/CCF-MainIntegration-20261104/Library` and Z2 `/media/jer/ZX20/CCF-UIQualification-20261104/Library` total **14,427,095,040 allocated bytes** on ZX20; no unique recoverable estimate or deletion authority. Closed #275/#277 and historical 4 November Unity lease releases are **not owner retirement**. In particular, live CCF-Ops lanes register Z1 as a **main playtest worktree**, which may still be needed. **OWNER UNCONFIRMED — NO CLEANUP for both**, and do not assume a 7.2 GB reimport is acceptable to user. Only their `Library` folder, not entire source checkout, would ever be considered for future approval. #361 old U3 and #382 newer UI personal testing project including their Libraries remain fully **PROTECTED**.

## Build evidence correction — IMPORTANT

The earlier #383 partial report said the on-disk S1B binary `CCF-S1-20261019-L01-8f9ed80-SMOKE-Linux-x64` was referenced by the stored smoke-summary witnesses. The new targeted audit **corrects that**: six smoke summaries cite **different now-absent builds** and **do not prove the 8f9ed80 executable was the tested one**. The current binary has its own `build-identity.json` and `build-result.txt` reporting Unity 6000.6.0f1 and a successful build with 192 warnings; these are **build receipts, not a smoke playthrough proof**. No separate independent archive exists in reviewed locations; its licensed art pack is not in committed Git. Therefore keep **S1B 1,225,150,464 bytes ARCHIVE BEFORE REMOVAL**, **S1-80m 1,260,412,928 bytes UNCERTAIN**, **Legacy evidence 556,089,344 bytes PROTECTED**. The known archive/backup gap is not solved; no cleanup action.

## User decision and next gates

1. **No automatic deletion.** Exact B1 ten-path user approval is required before Manager creates an execution packet that touches shared Git metadata. The Manager can present an approval choice, but must not infer agreement from "continue cleaning".
2. **ZX20 priorities:** before any Z1/Z2 approval request, responsible current owner/user must state whether each lane remains needed, particularly Z1 as possible main playtest copy; user must accept cold reimport cost and current owner must release cache. A safe rebuild with original licensed assets/ignored dependencies must be documented. No playtest or user save data touched.
3. **Windows migration/backup:** destination drive capacity and source/independent backup still NOT VERIFIED. No move/transfer/migration planned.
4. **Audit status:** #383 OPEN/PARTIAL, not complete; worker has handed off a substantive addendum and reported nothing destructive. No new local scan task unless user opts for a specific cleanup batch or actual new owner evidence arrives.
5. **Dashboard:** GitHub publication and coordination update do not prove dashboard ingestion/readback, still UNVERIFIED.

**Manager verdict:** **APPROVED AS PARTIAL / B1 CONDITIONALLY ELIGIBLE FOR USER APPROVAL / Z1+Z2 OWNER-BLOCKED / BUILDS ARCHIVE-BLOCKED.** No approval to remove any path, edit any protected Git-store metadata, run Unity or lift the T3 Fork Integration HOLD.
