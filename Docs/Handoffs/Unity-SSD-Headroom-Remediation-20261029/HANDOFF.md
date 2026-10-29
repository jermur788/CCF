# Unity SSD Headroom Remediation — HANDOFF

- Claim: **#152**
- Executor: T3 Code / OpenCode harness, provider `opencode-go/deepseek-v4.1-flash`
- Date (UTC): 2026-10-29
- Task packet: `Docs/Handoffs/Unity-SSD-Headroom-Remediation-20261029/TASK-PACKET.md` (main commit `271d04c25fce637d5b85655a81e190dfff0df3a8`)
- Scope: storage remediation only. No production edits, no Unity launch, no Windows build.

## Result

**UNITY SSD HEADROOM RESTORED — UNITY QUEUE MAY RESUME**

- Starting free: `15,820,521,472` bytes = **14.734 GiB**
- Final free: `22,822,566,640` bytes = **21.255 GiB**
- Freed: `7,001,735,168` bytes = **6.521 GiB** (allocated on disk)
- Manager target 20 GiB (`21,474,836,480` bytes): **MET** (margin 1.255 GiB)
- Mandatory 15 GiB Unity-lock floor (`16,106,127,360` bytes): **clear**

Cleanup stopped immediately after the single deletion that crossed 20 GiB.

## Preflight

- `unity-lock status`: `free`, no metadata, `unity_editor_pids: []`.
- `pgrep -af Unity`: no Unity process.
- Live claims refreshed: `Docs/Project/coordination/active-tasks.md`, GitHub issues #74, #129, #147, #152 and all open RUNNING claims (#91, #106, #94, #97, #146, #39).
- Lock history (`CCF-Ops/state/unity/history.jsonl`) confirmed the only recent ZX20 Unity lanes were U1, Q10 and the historical RackV2-09 worktree.

## Inventory (ZX20, allocated, largest first)

| Path | Size | Classification |
|---|---|---|
| `/media/jer/ZX20/Kinsale` | ~382.5 GB | ACTIVE/PROTECTED — personal media, not CCF |
| `/media/jer/ZX20/Unity` | ~18.1 GB | ACTIVE/PROTECTED — Hub-managed Unity 6000.6.0f1 Editor |
| `/media/jer/ZX20/CCF-U1` | ~10.4 GB (Library 6.7 GB) | ACTIVE/PROTECTED — primary Unity lane; Library last used 21:41Z by claim #147, U1 restored to `c8cfae8` |
| `/media/jer/ZX20/CCF-RackV2-09` | ~8.5 GB (Library 6.7 GB) | DISPOSABLE-GENERATED — clean, inactive/released |
| `/media/jer/ZX20/CCF-Q10` | ~7.6 GB (Library 5.8 GB) | DIRTY/UNKNOWN + retained per packet — #74 blocked lane, 2 modified files |
| `/media/jer/ZX20/test_folder` | ~4.0 GB | ACTIVE/PROTECTED — user data |
| `/media/jer/ZX20/CCF-Integration` | ~2.3 GB | ACTIVE/PROTECTED — integration lane (`unity: false`) |
| Remaining CCF worktrees | ~1.7–1.8 GB each | Source only, no `Library` |

Only three worktrees had Unity caches: U1, RackV2-09 and Q10.

## Removed (exact)

- `/media/jer/ZX20/CCF-RackV2-09/Library` — **6.521 GiB allocated** (`7,001,735,168` bytes; 3.17 GB apparent).
  - Generated Unity import cache only: `PackageCache`, `DataStore`, `Bee`, `BurstCache`, `ShaderCache`, `ScriptAssemblies`, `ArtifactDB`, `SourceAssetDB`, `TempArtifacts`, etc.
  - Not tracked by Git; no `Library.meta`; no source, save, capture or handoff inside it.

### Proof the removed cache was safe

- Worktree is clean including untracked files: `git status --porcelain` returned 0 lines before and after deletion.
- Branch `feature/scenario-one-rack-v2-field-marking-v1` at `04913c02fbd9a37dfbc536dacf02c8e90a3b58cf`; commit and branch both present in `/media/jer/Files/CCF/Repository.git` (`refs/heads/...` and `refs/remotes/origin/...`).
- No open/running claim owns RACK-V2-09. Claims #94 (RACK-V2-10) and #97 (RACK-V2-12) use `/media/jer/Videos/CCF-RackV2-10` and `/media/jer/Videos/CCF-RackV2-12`; #91/#106/#147 use U1.
- Required evidence already preserved outside the cache on the durable store: `/media/jer/Files/CCF/Verification/RackV2-09-20261029` (~61 MB: `field-01..05`, `runtime-01`, `save-01`, logs, restored-side-effects patch, lock release receipt).
- The worktree itself was NOT moved or removed.

## Skipped candidates

- `CCF-U1/Library` (6.712 GB): ACTIVE/PROTECTED primary Unity lane, used by the current RUNNING #91/#106/#147 line; retaining it preserves the warm cache for the resumed queue.
- `CCF-Q10/Library` (5.812 GB): DIRTY/UNKNOWN (2 modified files: `Docs/Handoffs/...Q10/HANDOFF.md`, `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs`) and explicitly to be retained per packet to reduce the next Q10 import cost.
- `Kinsale`, `test_folder`, `Google Drive`, `Music`: user/personal data, never candidates.
- `/media/jer/Videos` worktrees: separate volume and all belong to RUNNING claims; untouched.

## Final state

- `df -B1 /media/jer/ZX20`: `22,822,566,640` available (97% -> 96% used).
- Unity lock: `free`, no metadata, no Editor PIDs.
- No Unity Editor launched during this task.
- No production, branch, commit, worktree, evidence or user file changed by this task.

## Evidence

- `/media/jer/Files/CCF/Verification/Unity-SSD-Headroom-Remediation-20261029/HANDOFF.md` (this file)
- `/media/jer/Files/CCF/Verification/Unity-SSD-Headroom-Remediation-20261029/deletion.log` (before/after `df` and exact target)

DASHBOARD SYNC: claim #152 comment to be posted with this result.
PUBLISHED DASHBOARD REFLECTS THIS TASK: pending comment/readback.
