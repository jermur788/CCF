# Batch A execution result — worktree retirement, 2026-11-23

**Outcome: 15 of 15 approved lanes removed, 0 skipped, 0 failed. `--force` never used. No branch or commit deleted.**

| Field | Value |
|---|---|
| Registry entries before | 42 |
| Registry entries after | 27 (21 real lanes + 6 pre-existing dead `C:` rows) |
| Space freed | 16,628 MB (~16.2 GiB) |
| Approval | Owner, 2026-11-23, Batch A only (15 lanes). Leak stop and registry prune were NOT part of this approval. |
| Method | `git worktree remove <path>` per lane, after re-checking clean / pushed / unused |

## Per-lane result

| Lane | Branch / HEAD | Checks | Result |
|---|---|---|---|
| `~/Development/CCF-HP-Worktrees/CCF-885-OwnerUX-Docs-20261122` | `docs/multi-885-owner-ux-d091-admission-20261122-v1` `d8780fd7` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-867-OwnerPlaytest` | `detached` `4c42e201` | dirty=0, pushed=in-origin, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-882-CompetitionUI` | `feat/multi-882-competition-unity-ui-firstpass-v1` `32a3ddc4` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-882-Readiness` | `docs/multi-882-player-test-ready-20261120-v1` `250136e7` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-882-Recheck` | `detached` `e11dd519` | dirty=0, pushed=in-origin, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-882-Review` | `detached` `43bfba8e` | dirty=0, pushed=in-origin, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121` | `integration/multi-885-ma1-two-area-20261121-v1` `4cecf873` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-885-MA1` | `feat/multi-885-ma1-two-area-workplan-20261121-v1` `ac4c7b51` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-885-Review-20261121` | `detached` `58950a9a` | dirty=0, pushed=in-origin, live=no | **REMOVED** |
| `~/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Docs` | `detached` `fed675f5` | dirty=0, pushed=in-origin, live=no | **REMOVED** |
| `~/Development/CCF-Review-877/wt-docs` | `docs/multi-877-owner-g2-launch-result-20261120-v1` `bed4d2c9` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/CCF-Dev/Multi-20261119/_814/docs` | `docs/multi-814-candidate-marking-result-20261119-v1` `7e283c62` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/ccf-worktrees/CCF-MULTI-008-SitkaLab-WSL2-20261114` | `feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1` `2659db99` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/ccf-worktrees/multi-p0-567-wsl2-qualification` | `verify/multi-sitka-p0-567-wsl2-edge-targeted-20261116-v1` `10d40004` | dirty=0, pushed=yes, live=no | **REMOVED** |
| `~/Development/CCF-870-BlindB` | `experiment/multi-870-blind-claude-ui-v1` `e1d07a7d` | dirty=0, pushed=yes, live=no | **REMOVED** |

## Verification after removal

- `git worktree list` no longer contains any of the 15 paths.
- Every non-detached branch still resolves on `origin` at the same commit (spot-checked in the execution log).
- Detached lanes were removed only after confirming their commit was contained in an `origin/*` ref; the commits remain reachable history.
- The current-task lanes, the live `multi-stage4-owner-ux` cluster, the three protected server lanes and the primary checkout were not touched.
- Ignored build outputs (`bin/`, `obj/`, `.p0ref/`) inside the removed lanes went with the directories; they are regenerable and were never tracked.

## Deliberately not done

- No branch was deleted, no `git prune`/`gc`, no `reset`, no `stash`.
- The leaked `/multi` server `pid 4111475` was left running.
- The six dead `C:/Users/hp/.t3/...` registry rows were left in place.
- The loose directories under `/home/jer` (CCF-Dev, dev, Research, migration/qualification sets) were left untouched.
