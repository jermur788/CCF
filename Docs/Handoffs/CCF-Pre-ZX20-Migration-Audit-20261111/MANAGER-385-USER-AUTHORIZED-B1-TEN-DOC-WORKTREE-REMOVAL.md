# #385 — User-authorised removal of EXACTLY ten retired docs-only CCF worktrees (B1)

**User authorisation:** 11 November 2026, 23:47 UTC: “Fifty ten work trains are deprecated. Then delete them all,” immediately clarified: “If those worktrees are not needed anymore, then delete them now.” Read as explicit conditional **approval to remove the ten B1 documentation worktree checkouts** described and individually listed below, **only if a fresh local check establishes each is truly retired, inactive and fully recoverable**. This is not approval to delete 50, 152, other Unity projects, Git branches or the shared repository. User has already approved this exact bounded batch: do NOT demand a second sign-off once the hard safety gates below pass.

**Execution claim:** [#385](https://github.com/jermur788/CCF/issues/385), initially READY / NOT RUNNING. Distinct from [#383](https://github.com/jermur788/CCF/issues/383), which remains OPEN / PARTIAL as the overall audit.  
**Manager:** [#200](https://github.com/jermur788/CCF/issues/200).  
**Difficulty:** MEDIUM–HIGH; ten Git worktrees share a protected object/metadata store and current ownership may differ from the recent inventory.  
**Best local worker:** **Sol / T3 Code (Codex High)** with access to the actual Linux Files mount, Git CLI and CCF-Ops owner registry. No independent second model, no Unity.  
**Exact execution lane:** approved **single Git worktree-registry writer** on the current machine at `/media/jer/Files/CCF/Repository.git`. This path is a **shared Git store, not a place to make a new checkout**; do not create a new worktree, switch any active branch or move other files. Writing only the specific worktree registry entries as a normal side effect of `git worktree remove <exact path>` is allowed **once the live exclusive ownership lock is obtained**. For report-only files use a GitHub docs-only branch, never an active local Unity/source lane. Record actual execution host, agent/provider, exact repo HEAD/branch used for control, and ownership lock ID on #385 before first removal.

## Immutable audit evidence (reference, not a substitute for live proof)

- [Sol B1 residual candidate report](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-RESIDUAL-WORKTREE-BUILD-OWNER-CLOSURE.md)
- [Sol B1 candidate TSV](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-383-RESIDUAL-CANDIDATE-DECISIONS.tsv)
- [Manager B1 conditional governance verdict](https://github.com/jermur788/CCF/blob/docs/manager-383-residual-owner-and-worktree-decision-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-383-RESIDUAL-ADDENDUM-B1-CONDITIONAL-CLEANUP-RULING.md)
- Live `Docs/Project/coordination/active-tasks.md` on `main`, live `CCF-Ops` lane/lease registry, open issues, actual Linux Git/worktree state. **Refresh at execution time.**

The 23:23 UTC audit observed all ten clean with no staged/untracked/ignored files and no process cwd, and each tracked source commit present on GitHub. **This was a point-in-time observation; if any check now differs, SKIP that path rather than assume the user authorised new changes.**

## Exact scope and expected audit identities

All ten paths are under `/media/jer/Files/CCF/Operations`, on **Files ext4**. All must belong to the common Git store `/media/jer/Files/CCF/Repository.git`.

| ID | Exact relative leaf under `/media/jer/Files/CCF/Operations/` | Expected checked-out branch | Expected initial HEAD prefix | Prior allocated bytes |
|---|---|---|---|---:|
| B1-01 | `CCF-AccessSpineDocs` | `docs/access-spine-measure-20261103-v1` | `f7e290cb` | 1,231,380,480 |
| B1-02 | `CCF-MFM-TopologyDocs-20261103` | `docs/mfm-topology-options-20261103-v1` | `feb25453` | 1,231,507,456 |
| B1-03 | `CCF-Overnight-InputSpec-20261104` | `docs/overnight-input-settings-spec-20261104` | `ae7944aa` | 1,231,679,488 |
| B1-04 | `CCF-Overnight-ObjectiveSpec-20261104` | `docs/overnight-objective-guidance-spec-20261104` | `e5fe1761` | 1,231,695,872 |
| B1-05 | `CCF-Overnight-MarkSpec-20261104` | `docs/overnight-mark-language-spec-20261104` | `e13b1c5f` | 1,231,699,968 |
| B1-06 | `CCF-MFM-OptionA-Docs-20261104` | `docs/mfm-option-a-offline-20261104-v1` | `3d8338c7` | 1,231,712,256 |
| B1-07 | `CCF-MFM-A-vs-C-Docs-20261104` | `docs/mfm-a-vs-c-offline-20261104-v1` | `4d0abd1b` | 1,232,138,240 |
| B1-08 | `CCF-Overnight-DPC-20261104` | `docs/v2-routing-continuation-20261104` | `4640eee0` | 1,231,953,920 |
| B1-09 | `CCF-Overnight-GuidanceReview-20261106` | `docs/overnight-guidance-review-20261106-v1` | `bb5bfc1b` | 1,238,474,752 |
| B1-10 | `CCF-D072-SpecReview-20261106` | `docs/d072-independent-spec-review-20261106-v1` | `7c411651` | 1,238,544,384 |

**B1 grand total previously allocated:** **12,330,786,816 bytes** on **Files only**, not a promise of reclaimed/free delta. User approval applies to these ten **worktree registrations and their checked-out directories**, not to branches, refs or shared Git object storage.

## Hard per-path admission before any deletion

1. **Mount and path identity**: `findmnt`/realpath/stat confirm Files is the intended ext4 device and each candidate is exactly the named directory (not a symlink, substituted mount, parent/child of another project or a different worktree). Confirm `git -C <path> rev-parse --git-common-dir` resolves to the intended `Repository.git` and `git worktree list --porcelain` shows that path. Confirm free bytes before/after with `df -B1`. If a path has vanished, **report ALREADY ABSENT**, do not prune or clean metadata to fabricate success.
2. **Exclusive metadata registry ownership:** Refresh current `Docs/Project/coordination/active-tasks.md` **from main** and actual CCF-Ops lane/lease registry. Confirm no agent/process/editor has an active write assignment or held lock for **any target worktree or the shared `Repository.git/worktrees` metadata**; don't assume the absence of a Unix process proves ownership release. Acquire/record one genuinely exclusive CCF Git-worktree registry cleanup writer lease/lock for #385 using the coordination mechanism actually in force; if there is no safe mechanism or another owner is active, **STOP/BLOCK** and return the conflict without editing any worktree. Once admitted, mark #385 [RUNNING] with model/provider, exact execution lane, actual branch/HEAD and lock receipt. This claim by itself confers no lock.
3. **Last source-data check per path**: branch and current HEAD must match **the expected prefix** above; any new/changed commit or branch means **SKIP/STOP that item**. Working tree must be completely clean, including staged/unstaged, all untracked, and **ignored content**; check submodule status if present. All paths must contain no user/personal/local-only/secret/licensed asset/verification/runtime data, and no linked user saves/Prefs. A local worktree being docs-only is insufficient if ignored files now exist. Keep source tree content, current status, per-path size and exact commit as sanitized report evidence.
4. **Reconstruction proof:** ensure the exact full checked-out commit is reachable from a verified remote GitHub ref and the remote branch/containing commit is accessible; note ObjectiveSpec remote branch is ahead of local head, which is acceptable **only if the exact old local commit remains reachable**. Confirm no original worktree has a unique local branch tip, stash, disconnected Git object, submodule changes or local-only evidence; any unknown is BLOCKED, not assumed removable. Branches and refs must not be deleted.
5. **No live consumer:** check nonintrusively whether any process/editor uses the worktree (cwd or open file handles if visible) and confirm no current designated task owner/lease; former docs authors need no individual chat reply **only** under the Manager's conditional waiver, not a blanket bypass of live ownership. For the two directly verified closed claims #282/#298, preserve same checks. If any item is in use, **SKIP**.

## Authorised execution, only after full preflight

Perform work **one path at a time** and capture outcome before proceeding.

- Invoke the normal Git registered-worktree removal from the **verified common Git repository** against the **exact approved absolute directory**: `git worktree remove <exact-absolute-path>` **without `--force`**. Use `git --git-dir=/media/jer/Files/CCF/Repository.git worktree remove <exact-absolute-path>` when verified to point to the same store. Normal Git worktree registry updates for that target **are the only authorised writes to the shared store**.
- **DO NOT** invoke `rm -rf`, `git clean`, `git worktree prune`, `git gc`, `git branch -d/-D`, reset, stash, forced remove, checkout/rebase, delete root `Repository.git` or another worktree, shred/archive anything, modify Unity/Assets/.meta or save data. Never forcibly unlock a worktree. If Git refuses removal, stop **that item** (and stop the entire run if shared-store integrity or lock is uncertain). Leave all other lanes unchanged.
- Check after each path: Git registration absent for that exact target, directory absent if Git completed removal, original remote branch and exact commit still reachable, no unrelated registration changed, and Files volume available-space delta. If a postcheck fails, halt and report potential partial state rather than attempting manual fixes.
- **If a candidate has files after status verification but before removal**, Git should reject; report and preserve. Do not delete ignored files to make it pass.

## Protected exclusions (absolute)

Never touch:
- `/media/jer/ZX20/CCF-U3` (older #361 user personal playable game), `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` (newer #382 UI playtest), or **ANY ZX20 Unity worktree**, including Z1/Z2 dated Libraries.
- CCF Primary, Legacy primary/worktrees, dirty Workbench, MainIntegration original source/assets, game source, licensed ignored assets, frozen Reference Future v1, user saves and `~/.config` / PlayerPrefs.
- `/media/jer/Files/CCF/Builds` (S1B, S1-80m, Legacy evidence), `Bundles`, `Archives`, any unique verification/Runtime evidence, CCF Git object store/branches, current specs/decision log, T3 Fork and dashboard.
- No Windows build, migration, Unity or T3 integration. **T3 Fork Integration HOLD remains binding**.

## Evidence/report/claim closure

**Required local report:** publish a concise new GitHub Markdown and exact-per-path TSV with `REMOVED / SKIPPED / BLOCKED / ALREADY ABSENT`, preflight locks, original full branch/HEAD, before/after Files `df` values, actual freed bytes, all ten path-specific results, unchanged protected refs/other worktrees, any error, and recovery directions. New report-only files under `Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/` on an **isolated docs report branch or GitHub API**, not by checking out or writing a target worktree. Suggested names: `SOL-385-TEN-DOC-WORKTREE-CLEANUP-EXECUTION-RESULT.md` and `SOL-385-PER-PATH-REMOVAL-RESULTS.tsv`. GitHub readback after publication, comment direct links to #385/#383/#200.

Mark #385 [HANDED-OFF] and close **only when execution is complete and source/reporting lock released, even if some paths were safely SKIPPED with explicit blockers**; Manager will then assess which were removed and whether another user decision is needed. If interrupted/blocked during admission, mark [BLOCKED], keep open. #383 remains OPEN/PARTIAL; audit scope not satisfied by this batch.

**Rollback:** Only for approved worktrees: use the recorded branch/full commit and remote objects to recreate an independent *empty-path* worktree through authorised Git worktree-add after requesting appropriate registry ownership; no automatic rollback edits on this turn. This recovers committed docs, not transient warm files. If any unique ignored data is detected, **DO NOT REMOVE** rather than relying on rollback.

## Single copy/paste local Sol dispatch

> Sol / T3 Code (Codex High): **USER HAS EXPLICITLY APPROVED DELETION of exactly ten deprecated docs-only CCF worktrees B1, conditional on fresh proof each is retired and clean.** Execute #385 using this full binding packet: https://github.com/jermur788/CCF/blob/docs/manager-b1-ten-doc-worktrees-cleanup-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-385-USER-AUTHORIZED-B1-TEN-DOC-WORKTREE-REMOVAL.md . On the actual Linux Files drive, refresh live ownership and exclusive shared Git-worktree registry lock, verify each exact path, branch/HEAD, no tracked/staged/untracked/ignored/unique data, no current owner/process/lease, and remote recovery. Then **REMOVE THE VERIFIED TEN NOW**, one-by-one via **`git worktree remove <exact path>` without --force**, updating only registered target metadata. **Skip/block any failing check; no extra user approval needed for the ten listed paths once hard gates pass.** Keep every other worktree, both user Scenario One projects, Unity Libraries, shared Git objects/branches, builds and original saves untouched. No `rm -rf`, gc, prune, reset, stash, Unity, T3 Fork/HOLD change or migration. Record per-path actual removal and Files free delta, publish GitHub execution result, and return #200. #383 remains OPEN/PARTIAL.
