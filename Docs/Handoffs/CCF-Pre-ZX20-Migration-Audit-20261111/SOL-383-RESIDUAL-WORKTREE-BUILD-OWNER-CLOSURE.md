# #383 residual closure: owners, worktrees, Builds — PARTIAL (#383 stays OPEN)
Observed 2026-11-11 23:23Z onward. Read-only; nothing deleted, moved, archived, rebuilt, reset or migrated. Unity not launched. #361 (CCF-U3) and #382 projects/Libraries PROTECTED. T3 Fork Integration HOLD unchanged.

**Result: NO APPROVABLE BATCH.** Closest candidate is B1 (worktrees); exact missing decisions are at the end.

## Preflight (verified 23:23Z)
Free bytes: `/` 19,818,078,208; Files 182,951,108,608; ZX20 26,788,233,216. No Unity process running (not itself lease proof).

## Q1 Owners of the two old Libraries (Z1 MainIntegration-20261104, Z2 UIQualification-20261104)
Verified: #275 and #277 are closed with no assignee; `active-tasks.md` on `origin/main` has no active claim naming either lane; the last CCF-Ops Unity lease events for them are releases on 2026-11-04 (Z2 16:05Z, Z1 20:07Z), actor T3-Code-deepseek-v4.1-flash; the latest lease event overall is a Sol381 release 2026-11-11 01:14Z.
Also verified: `CCF-Ops/lanes.json` describes Z1 (MainInt277) as "main playtest worktree after user-directed #277/#243 integration; warm Library", i.e. possibly a user-facing playtest copy of `main`.
**Status: OWNER UNCONFIRMED — NO CLEANUP** for both. Historical closure and released leases are not retirement approval. No owner attestation obtained. Exact questions: (a) Manager/user: is lane MainInt277 still wanted as a playtest copy of `main`? (b) Manager: is UIQual275 retired? (c) user: is a ~7.2 GB reimport per cache an acceptable recovery cost? (Unverified: actual reimport time; no Unity run.)

## Q2 Ten idle worktrees (all docs-only branches under /media/jer/Files/CCF/Operations; Files volume)
Method: picked 10 of 82 Files-side worktrees; `git status --porcelain --ignored` (no refresh writes); remote-ref containment in the local store; branch head on GitHub via API; process-cwd check; `active-tasks.md` mentions.
All 10: 0 modified, 0 staged/untracked, 0 ignored files; HEAD contained in remote refs (and branch head confirmed on GitHub: 9 equal, ObjectiveSpec GitHub head c61bbdd9 is a descendant of local e5fe1761); no process has a cwd in any of them. Two are mentioned in active-tasks as HANDED-OFF/approved (#282 ObjectiveSpec, #298 D072 SpecReview).
Allocated bytes per worktree ≈1.23 GB, total **12,330,786,816** (distinct directories, no overlap). Details in the TSV.
Unverified: whether anyone has an editor open on them; per-lane owner release (only handed-off status in coordination); GitHub objects not checked for completeness beyond the branch head resolving.
Caveat: removing a worktree properly edits metadata in the PROTECTED shared store (`Repository.git/worktrees/<name>`), and none of this is on `/`, which is the tight volume. Files has 183 GB free, so the benefit is housekeeping, not headroom.

## Q3 Builds
- **Correction to the earlier partial report:** the S1B smoke executable on disk (`CCF-S1-20261019-L01-8f9ed80-SMOKE-Linux-x64`, 1,225,150,464 B) is NOT the build cited by `Verification/ScenarioOneLinuxPlayerSmoke-*/smoke-summary.json`. Those six summaries cite `f6c5d3a-L03` and `8942b5c-L01` builds, whose directories are no longer in `Builds/S1B`. No smoke witness for 8f9ed80 was found.
- 8f9ed80 does carry its own `build-identity.json` (git sha 8f9ed8025243254fe7378b6d7771a5f1e7efaccc, Unity 6000.6.0f1, art-pack hash) and `build-result.txt` (Succeeded, 0 errors, 192 warnings). The commit identity is a build witness; the binary is rebuildable only with Unity plus the licensed art pack (not committed to the repository).
- Independent archive: none found. `/media/jer/Files/CCF/Bundles` is empty; `Archives` holds only `UnityAssets`. **S1B: ARCHIVE BEFORE REMOVAL** (unchanged).
- **S1-80m-local-output** (1,260,412,928 B): Unity `Build/` outputs dated 2026-10-10 (largest `Task80` 875,773,952 B). No receipt or owner found in a bounded search. Its S1A/P2/P3/Windthrow/Understorey subfolders are not byte-identical to `S1A-legacy-evidence` counterparts (diff differs/absent). **UNCERTAIN, no duplicate claimed.**
- `S1A-legacy-evidence` (556,089,344 B): PROTECTED, unchanged.
- Windows migration (recommendation only; Windows free space UNKNOWN): HOLD S1B and S1-80m until archived or declared disposable; do not transfer Library caches; transfer no worktrees (rebuildable from Git).

## Classification changes vs partial report
Z1/Z2: unchanged (deferred), now OWNER UNCONFIRMED. S1B: rationale corrected (see above), action unchanged. S1-80m unchanged. B1 added.

## Candidate B1 (not approvable yet)
Ten listed docs worktrees, action would be `git worktree remove <path>` without `--force` (refuses if dirty), ceiling 12,330,786,816 B on Files. Rollback: `git worktree add <path> <branch>` at the recorded head. Missing: (1) Manager decision that handed-off docs lanes need no per-owner attestation; (2) explicit authority to edit shared-store worktree metadata; (3) user approval of each exact path.

## NOT RUN
Other 72 Files worktrees, 15 ZX20 worktrees, Legacy worktrees; owner outreach; Unity; reimport timing; Windows destination free space; archive creation.
