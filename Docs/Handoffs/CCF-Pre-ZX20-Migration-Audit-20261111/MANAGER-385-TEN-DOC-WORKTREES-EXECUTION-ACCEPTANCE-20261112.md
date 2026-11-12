# Manager #200 — #385 ten B1 docs worktree removal review

**Review date:** 12 November 2026.  
**Verdict:** **APPROVED — DELIVERED SCOPE AND OBSERVED OUTCOME**, with **ONE DOCUMENTED PROCESS-GATE EXCEPTION** and **one corrected nonblocking verification mistake**. The task was intentionally limited to ten user-approved B1 docs worktrees on the Files volume. User approval existed before execution. There is no evidence in the published result warranting rollback or repeating destructive tests. **No claim of independent direct local-disk verification by Manager.**

## Evidence directly reviewed on GitHub

- [Claude #385 execution report](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-385-TEN-DOC-WORKTREE-CLEANUP-EXECUTION-RESULT.md).
- [Claude #385 exact per-path result TSV](https://github.com/jermur788/CCF/blob/docs/ccf-pre-zx20-storage-migration-audit-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/SOL-385-PER-PATH-REMOVAL-RESULTS.tsv).
- [Binding Manager task #385](https://github.com/jermur788/CCF/blob/docs/manager-b1-ten-doc-worktrees-cleanup-20261111-v1/Docs/Handoffs/CCF-Pre-ZX20-Migration-Audit-20261111/MANAGER-385-USER-AUTHORIZED-B1-TEN-DOC-WORKTREE-REMOVAL.md).
- [Closed GitHub execution claim #385](https://github.com/jermur788/CCF/issues/385), [parent open cleanup audit #383](https://github.com/jermur788/CCF/issues/383), live `Docs/Project/coordination/active-tasks.md` on `main`.

These GitHub documents exist and their content was independently read by Manager. The operator reports byte-identical readback from GitHub; Manager cannot independently validate local machine state or run the original commands remotely. The execution report's filename uses the legacy `SOL-` deliverable naming convention, but **the executing provider was actually Claude Sonnet 5.5 through Claude Code/T3 Code**, not Sol/Codex. Preserve provider attribution.

## Accepted execution results

| Qualification / outcome | Operator-reported result | Manager treatment |
|---|---|---|
| Exact scope | **10 of 10 B1** named docs-only worktrees | Within user's explicit authorization |
| Files volume available before | **182,950,215,680 bytes** | Reported local `df -B1` |
| Files volume available after | **195,289,886,720 bytes** | Reported local `df -B1` |
| Difference | **12,339,671,040 bytes** gained | ~12.34 decimal GB, measured volume delta; not necessarily solely allocated bytes of removed directories because background activity/overhead can exist |
| Registered worktree count | **107 → 97**, ten named targets absent | Reported no other registration change |
| Preserved Git refs | **2,475 refs**, before/after hash identical | Supports unchanged refs; does not alone prove immutable Git object store, but provider reports original commits readable and no gc/prune |
| Removal mechanism | `git --git-dir=... worktree remove <exact path>` on each, no `--force` | Correct narrow Git-authoritative mechanism |
| Preflight | Paths, realpath/mount, common Git store, branch/exact HEAD, clean staged/tracked/untracked/ignored, remote commit reachability, open handles | Reported all passed immediately before respective removal |
| Protected data | User #361 older playable and #382 new UI projects, Z1/Z2 older dated Libraries, Builds, remaining 72 Files worktrees, T3 HOLD | Provider reports untouched; no direct local cross-check by Manager |

All ten TSV entries show `REMOVED` with before/after volume free bytes and their own branch/full recorded HEAD. No broad cleanup, branch/objects deletion, Windows migration or Unity run.

## Exceptions — record accurately, do not conceal

**E-1: No true exclusive Git worktree-registry mutex (material process exception).** The packet required a genuinely exclusive metadata writer lock before deletions. The user/operator states CCF-Ops only has a **Unity-only lock**, not a Git-worktree-registry lock. The worker substituted refreshed live coordination showing #385 as sole declared registry writer and checked for Git processes, lock files and open handles. These are meaningful collision-risk mitigations but **are not equivalent to an enforceable exclusive lock**. Therefore execution is **not fully gate-compliant** and must not be called a clean procedural PASS. Accept the observed result but record **NEEDS CONTROL IMPROVEMENT for subsequent shared-Git deletions**: define an explicit exclusive owner/lease mechanism or elevate to a separate blocking Manager adjudication before such a task. Do not install a new lock unilaterally or falsify old evidence. No retroactive rerun or post-hoc lock claim.

**E-2: Incorrect direction test on B1-04 (corrected, not an unresolved removal defect).** The first pass skipped the `ObjectiveSpec` docs worktree when a remote branch was ahead by four commits: its checkout commit was reachable as an ancestor, but the reachability guard mistakenly accepted only identical or behind states. Operator corrected only that guard and repeated the identical other validations on this single item; second pass removal succeeded. Remote refs and source reachability remained intact. The packet's relevant requirement was exact checkout commit reachability, not remote-head equality. Record this fix for future read-only validation tools; **do not weaken checks to let unique local source disappear**.

**E-3: Actual provider differs from packet recommendation (nonblocking transparency).** Claude Sonnet 5.5 via Claude Code/T3 Code executed, not the proposed Sol/Codex. This is capability-based routing variance, not evidence of incorrect technical work. The provider is explicitly identified in claim/report.

## Final Manager disposition

**APPROVED scope/outcome** and [#385](https://github.com/jermur788/CCF/issues/385) **closed / handed off**, without pretending the lock gate passed. Do not reopen solely for repeated tests. A single clear follow-up finding **Git worktree-registry ownership lock absent** is recorded for future cleanups; the procedural exception must be considered before any additional Git worktree deletion, including other 72 Files or Legacy checkout. No new engineering task is dispatched solely to build lock tooling in this turn.

**#383** overall CCF pre-ZX20 audit stays **OPEN / PARTIAL**: remaining worktrees and historical evidence/Builds/migration closure, and the two old ZX20 Unity caches **OWNER UNCONFIRMED**. The user-protected #361 and #382 playtest project roots, saves, licensed assets, frozen Reference Future, shared Git object store, Builds and T3 Fork Integration HOLD remain untouched and protected. **No additional cleanup authorised by this acceptance**.

**What this means:** housekeeping reclaimed ~12.34 GB on **Files**, not space on root or ZX20; the user's ZX20 migration target remains a separate capacity/ownership decision. Later action when requested: obtain owner-specific retirement for Z1/Z2 and licensed-data/reimport proof, or complete a separate user-approved cleanup batch with enforceable Git-metadata ownership. Dashboard ingestion and readback remain **UNVERIFIED**.

**User action now:** none for the completed B1 cleanup; do not run or re-run anything. Any new destructive batch needs separate exact-path consent and current safety proof.
