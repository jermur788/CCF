# Machine lane and storage audit — 2026-11-23

**Status: READ-ONLY AUDIT. NOTHING DELETED, NOTHING PRUNED, NO PROCESS KILLED.**
Observed 2026-11-23 between 14:00 and 15:45 UTC on the WSL Ubuntu host. Disk `/dev/sdd`: 1007 G total, ~881 G available.

Method: `git worktree list --porcelain`, `git status --porcelain`, `git ls-remote origin <branch>`, `git branch -r --contains`, `du -sm`, `stat`, `ps`/`/proc/*/cwd`, `ss -lntp`, `find -maxdepth 3`. No write operations were performed against any lane or the registry.

## 1. Headline

| Category | Count | Size |
|---|---|---|
| Registered worktrees under `/home/jer` (excluding 6 dead `C:` rows) | 34 | ~44 GB |
| Loose directories directly under `/home/jer` | 20 | ~34 GB |
| Idle `sitkap1-ui` servers still listening | 24 | ~0 CPU each |
| Registry rows marked `prunable` (dead Windows paths) | 6 | 0 (metadata) |

A parallel agent session is **actively working** on the `multi-stage4-owner-ux` feature during this audit: a new lane (`…R3-20261123`) appeared between two audit passes, the spec branch advanced, and one review lane holds uncommitted harness edits. Any cleanup must therefore treat that cluster as live.

## 2. Do not touch

| Lane | Why |
|---|---|
| `/home/jer/Development/CCF-HP` (primary, `main` @ `cbf3a618`) | **Diverged**: 1864 commits ahead / 50 behind `origin/main`. Resolving it is an owner decision, not cleanup. |
| `CCF-Multi-Stage4UiCatchup-20261123` | Current task lane (`e7c1a57d`), approved candidate. |
| `CCF-Multi-Stage4UiCatchup-Review-20261123`, `…-Review2-20261123` | Review evidence lanes (`42885ebf`, `d82f34e8`). |
| 11 `CCF-Multi-Stage4OwnerUX-*` lanes and `CCF-Multi-885-OwnerUX-20261122` | **Live parallel session**; one has uncommitted changes (see §5). |
| `CCF-Multi-Stage4-AdultParity-20261123`, `CCF-Multi-899-900-*` | Immediate predecessors of the shipped work; keep until integration. |
| `CCF-Multi-882-Launch`, `CCF-Multi-885-OwnerUX-20261122`, `CCF-Review-877/wt-owner` | Running `/multi` servers — these are the long-lived listeners `:43649`, `:33767`, `:46593`. |

## 3. Retire candidates (clean, pushed, no process, older than the current work)

All 15 are clean, their own branch (or commit) is on `origin`, and no process is using them. Removing the **worktree** does not delete any branch or commit.

| # | Lane | Branch / HEAD | MB |
|---|---|---|---|
| 1 | `CCF-HP-Worktrees/CCF-885-OwnerUX-Docs-20261122` | `docs/multi-885-owner-ux-d091-admission-…` `d8780fd7` | 1184 |
| 2 | `CCF-HP-Worktrees/CCF-Multi-867-OwnerPlaytest` | detached `4c42e201` (in origin) | 1187 |
| 3 | `CCF-HP-Worktrees/CCF-Multi-882-CompetitionUI` | `feat/multi-882-competition-unity-ui-firstpass-…` `32a3ddc4` | 1189 |
| 4 | `CCF-HP-Worktrees/CCF-Multi-882-Readiness` | `docs/multi-882-player-test-ready-…` `250136e7` | 1184 |
| 5 | `CCF-HP-Worktrees/CCF-Multi-882-Recheck` | detached `e11dd519` | 1189 |
| 6 | `CCF-HP-Worktrees/CCF-Multi-882-Review` | detached `43bfba8e` | 1189 |
| 7 | `CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121` | `integration/multi-885-ma1-two-area-…` `4cecf873` | 1191 |
| 8 | `CCF-HP-Worktrees/CCF-Multi-885-MA1` | `feat/multi-885-ma1-two-area-workplan-…` `ac4c7b51` | 1191 |
| 9 | `CCF-HP-Worktrees/CCF-Multi-885-Review-20261121` | detached `58950a9a` | 1191 |
| 10 | `CCF-HP-Worktrees/CCF-Multi-StandMap2D-Docs` | detached `fed675f5` | 1185 |
| 11 | `CCF-Review-877/wt-docs` | `docs/multi-877-owner-g2-launch-result-…` `bed4d2c9` | 1184 |
| 12 | `/home/jer/CCF-Dev/Multi-20261119/_814/docs` | `docs/multi-814-candidate-marking-result-…` `7e283c62` | 1184 |
| 13 | `/home/jer/ccf-worktrees/CCF-MULTI-008-SitkaLab-WSL2-20261114` | `feature/multi-008-sitka-lab-wsl2-…` `2659db99` | 1188 |
| 14 | `/home/jer/ccf-worktrees/multi-p0-567-wsl2-qualification` | `verify/multi-sitka-p0-567-wsl2-edge-…` `10d40004` | 1186 |
| 15 | `/home/jer/Development/CCF-870-BlindB` | `experiment/multi-870-blind-claude-ui-v1` `e1d07a7d` | 1 |

Total recoverable from worktrees: **~16.6 GB**, with no branch or commit loss. Do not use `rm`; each item needs `git worktree remove <path>` (no `--force`), one at a time, after re-checking clean + pushed + unused.

## 4. Loose directories directly under `/home/jer`

| Directory | MB | Newest | What it appears to be |
|---|---|---|---|
| `CCF-Dev` | 7166 | 2026-11-19 | Older checkout root; contains the `#814` docs worktree (#12 above), plus scripts/state |
| `dev` | 5405 | 2026-11-12 | `development-operations-dashboard`, `ipad-mini-2`, `t3code` |
| `Research` | 5311 | 2026-11-13 | `Samsara2-*` research trees |
| `qualification-windows-20261112` | 3524 | 2026-11-12 | Windows qualification copy of the repo |
| `migration-test-qualification-20261112` | 3490 | 2026-11-12 | Migration test qualification copy |
| `b21-offline-reproduction-20261112` | 3268 | 2026-11-12 | Offline reproduction copy |
| `migration-staging` | 2668 | 2026-11-12 | `T3Fork-Migration-20261111-…` |
| `ccf-worktrees` | 2373 | 2026-11-16 | Two registered lanes (#13, #14 above) |
| `migration-evidence` | 1809 | 2026-11-12 | Cutover/restoration reports |
| `_ipad` | 82 | 2026-11-19 | `packet.md` + `repo` |
| `Multi-885-StageB-20261121-evidence` | 4 | 2026-11-21 | Small evidence set |
| `b21-recovery-evidence-20261112` | 3 | 2026-11-12 | Review/handoff evidence |
| `multi008-evidence`, `multi589-evidence`, `orch-scratch` | 3 / 2 / 2 | 2026-11-14/16/21 | Small evidence and orchestration scratch |
| `ccf-workbench` | 1 | 2026-11-23 | `inbox/test-ping.txt`: "rclone round-trip test Mon Nov 23 10:33:51 GMT 2026" |
| `CCF-Restore-810`, `migration-test-evidence-20261112`, `qualification-evidence-20261112`, `toolchain-dl` | 1 each | 2026-11-12/19 | Small scripts and evidence |

Total: **~34 GB**. These are owner decisions, not mechanical cleanup: several are dated evidence sets for finish-line claims.

## 5. Dirty / in-flight lanes (preserve)

- `CCF-Multi-885-Demo-Playable-20261121` — one **untracked** file, `Tools/MultiRackSitkaP1Lab/src/ccf-context.md`. Someone's context note; not yours to discard.
- `CCF-Multi-Stage4OwnerUX-R2Review-F0DB-20261123` — `tests/ui-smoke-multiarea.sh` **modified, +123 lines uncommitted** in a detached review lane. This belongs to the live OwnerUX session and must not be removed, reset or stashed.

## 6. Registry anomalies

Six `prunable` rows point at Windows T3 worktrees under `C:/Users/hp/.t3/worktrees/CCF-HP/…` that no longer exist (`t3-1d59b3f5`, `t3-51cf0f49`, `t3-64f80957`, `t3-8fc40b07`, `t3-d73e44a5`, `t3-dd1bf123`). `git worktree prune` would remove the metadata only, but it is a registry mutation and is therefore proposed, not performed.

## 7. Leaked `/multi` servers

24 `sitkap1-ui` processes are listening, all at ~0% CPU:

- 3 long-lived protected: `:33767` (885-OwnerUX, 31 h), `:43649` (882-Launch, 66 h), `:46593` (Review-877/wt-owner, 69 h).
- 1 leaked from this task's lane: `pid 4111475`, `CCF-Multi-Stage4UiCatchup-20261123`, ~4 h old, port 35803. The run claimed a clean stop; it did not exit.
- 20 from the live OwnerUX session (15 under `t3-8fc40b07`/`t3-51cf0f49` T3 worktrees, 5 under `CCF-Multi-Stage4OwnerUX-Independent-20261123`).

Only `4111475` is safe to stop from this task. The other 20 are another session's.

## 8. Proposed actions, in risk order

1. **Batch A — retire the 15 worktrees in §3.** Per item: re-check `git status` empty, own branch on origin, no process cwd, then `git worktree remove <path>` without `--force`. ~16.6 GB. Branch deletion is *not* proposed.
2. **Batch B — stop `pid 4111475`** (this task's leaked server), then confirm the three protected listeners are still up.
3. **Batch C — `git worktree prune`** for the six dead `C:` rows. Metadata only.
4. **Batch D — loose directories in §4.** Owner decides item by item; `CCF-Dev` (7.2 GB), `dev` (5.4 GB), `Research` (5.3 GB) and the three Nov-12 qualification/migration copies (~10.3 GB) are the large ones. Any dated evidence set should be confirmed as already published before removal.
5. **Batch E — the diverged primary `main`.** Needs a deliberate owner decision about which lineage is authoritative; not a cleanup task.

A single combined pass over A+B+C would take the machine from ~44 GB of lanes to ~27 GB, without losing any branch, commit or document.

## 9. Non-claims

The size figures are `du` allocation on this filesystem at audit time. "Pushed" means the branch ref on `origin` points at the lane's exact HEAD; detached lanes were checked for containment in any `origin/*` ref. Absence of a process for a lane does not prove no other agent will use it; the live OwnerUX cluster is the counter-example.
