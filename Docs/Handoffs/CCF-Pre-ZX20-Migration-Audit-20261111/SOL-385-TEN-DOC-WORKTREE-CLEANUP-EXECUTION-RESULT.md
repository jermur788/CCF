# #385 B1 execution result: 10 of 10 REMOVED
Executed 2026-11-12 ~00:05–00:15 UTC by Claude (Sonnet 5.5, Claude Code via T3 Code; not Sol/Codex) on the local Linux host. Control checkout `/media/jer/Files/CCF-Primary` (not written). #383 stays OPEN/PARTIAL. T3 Fork Integration HOLD unchanged.

## Lock receipt (honest)
No machine-level mutex exists for the Git worktree registry (CCF-Ops `unity-lock` covers Unity only). Exclusivity rested on: live `active-tasks.md` on `origin/main` (Manager-owned single-writer authority) showing #385 as the only claim touching this registry; other RUNNING claims (#91, #200) are Manager lanes; no git process, no `Repository.git/*.lock`, no `worktrees/*/locked`, no process cwd/open fd in any target. This is weaker than a true exclusive lease.

## Gates per path (all passed before each removal)
realpath equals path, not a symlink, on Files `/dev/sdb4`; git common dir = `Repository.git`; registered in `git worktree list`; branch and full HEAD match the packet; `status --porcelain --ignored` empty; no submodules; not locked; HEAD on remote-tracking refs; GitHub compare confirms the commit is reachable from the branch; no process handle.
Removal: `git --git-dir=/media/jer/Files/CCF/Repository.git worktree remove <path>`, no `--force`. No rm, prune, gc, clean, reset, stash, or branch deletion.

## Results
All ten REMOVED (per-path table with full HEADs and byte deltas in the TSV). One incident: B1-04 (ObjectiveSpec) was SKIPPED in pass 1 because my own reachability gate was wrong-direction (accepted only `identical|behind`; the remote branch is 4 commits ahead, 0 behind, so the old commit is an ancestor). I corrected the gate, re-ran only B1-04 with every other check unchanged, and it was removed. No other item was re-run.

## Space (Files, `df -B1` available)
Before: 182,950,215,680. After pass 1 (9 removed): 194,057,289,728. After B1-04: 195,289,886,720.
**Total recovered: 12,339,671,040 bytes** (prior allocated estimate 12,330,786,816; difference is rounding and background changes on the volume).

## Post-checks
Registered worktrees 107 -> 97 (exactly the ten targets gone; no other registration changed). `git for-each-ref` hash unchanged across both passes (2475 refs, no branch deleted). Commits readable. ZX20 projects/Libraries (#361, #382, Z1, Z2) and `Builds` paths still present and untouched.

## Recovery
For any removed worktree: `git --git-dir=/media/jer/Files/CCF/Repository.git worktree add <empty-path> <branch>` at the recorded full HEAD (branches and objects intact; GitHub also holds them). Only committed content is recoverable; no ignored/untracked data existed.

## Not done
Nothing else was touched. Remaining 72 Files worktrees, ZX20 Libraries, Builds and Windows migration are still open under #383.
