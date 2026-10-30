# OPS-UNITY-LOCK-01 — Handoff

**Claim:** #189 · **Date:** 2026-10-30 · **Executor:** Claude Code (Sonnet 5.5)
**Result:** `UNITY LOCK GOVERNANCE RESTORED — #156 MAY RE-PREFLIGHT`

This handoff does not start #156 and holds no lock. #156 must repeat its full fresh preflight after Manager acceptance.

## Root cause

One stale field. The live lock tool's `storage.device` was `/dev/sda2`; the same ZX20 filesystem is now enumerated as `/dev/sdd2` (kernel device letters moved). UUID, filesystem, mount point, repository path, lanes and state root were already correct. The repository path, `Files-canonical` git store and Files-HDD NonUnity lane had been migrated on 2026-10-21.

The earlier #156 preflight inspected the tracked copy under `CCF-Primary/CCF-Ops`. That copy is not the production tool. The production tool, config and state are under `/media/jer/ZX20/CCF-Ops` (StorageArchitecture.md: "CCF-Ops stays on ZX20 ... live machine-local Unity lock/board state"; CCF-Ops/README.md: fixed state path `/media/jer/ZX20/CCF-Ops/state/unity`).

## Evidence

- Live mount (`findmnt -T /media/jer/ZX20`): `/dev/sdd2  /media/jer/ZX20  exfat  UUID 18D6-896E`.
- Storage README (`/media/jer/ZX20/CCF-STORAGE-README.txt`) and `Docs/Operations/StorageArchitecture.md`: canonical bare repo `/media/jer/Files/CCF/Repository.git`. The `lanes.json` `repository` and `git_stores` already matched. `git --git-dir=... fetch` and `cat-file` work against it.
- Before: `python3 -B scripts/unity-lock status` -> `ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdd2', ... 'uuid': '18D6-896E'}`.
- Tool schema: `config_read` -> `storage_check(root, uuid, fstype, device)` compares all four to `findmnt`. `device` comes from `lanes.json` (the hard-coded default in `storage_check()` is unused on this path). `ROOT` is hard-coded `/media/jer/ZX20`. State root is `ROOT/CCF-Ops/state/unity` (`DEFAULT`), fixed, no override.

## Change (single field)

File: `/media/jer/ZX20/CCF-Ops/lanes.json` (live, untracked; not in any git repo).

| Field | Before | After |
|---|---|---|
| `storage.device` | `/dev/sda2` | `/dev/sdd2` |

Nothing else was edited: no code, no state files, no history, no other config, and the tracked copy under `CCF-Primary` was left alone (it is stale and dirty under another owner).

## Verification (normal tool, no bypass flags)

```
$ cd /media/jer/ZX20/CCF-Ops && python3 -B scripts/unity-lock status
Unity lock: free
  metadata: None
  unity_editor_pids: []          (rc=0)

$ python3 -B scripts/unity-lock --json status
{"status": "free", "metadata": null, "unity_editor_pids": []}

$ python3 -B scripts/lane-status --storage-only
READY / PASS | device=/dev/sdd2 rw=True | free=20922892288 bytes (19.49 GiB) threshold=15.00 GiB
```

- State root used: `/media/jer/ZX20/CCF-Ops/state/unity` (contains `guard`, `history.jsonl`; no `lock.json`).
- Last history event: `release` by `Claude-S1D-T05` (lock `99b03d5c...`, 2026-10-30T13:14Z). This is consistent with the explicit free state.
- Independent process check: no `Editor/Unity` processes.
- Self-test: **not run** (not needed). No acquire/release occurred under #189; the final state is not owned by OPS.

## Notes for the Manager

- **Fragile by design:** the check pins a kernel device name. It will break again whenever the SSD enumerates differently after a reboot or re-plug. If you want durability, a separate approved change could make the check UUID-only (code change; deliberately not done here).
- **Space:** ZX20 free is 19.49 GiB against the 15 GiB threshold.
- **Unrelated stale doc:** `StorageArchitecture.md` still lists ZX20 as `/dev/sda2`.

## Rollback

`cp -p /media/jer/Files/CCF/Operations/UnityLockRepair-20261030/lanes.json.before /media/jer/ZX20/CCF-Ops/lanes.json` (this restores the broken state). The status receipt is `status-after.json` in the same directory.

## Boundaries

No Unity launch, no Windows build, no gameplay/project/test edits. The #156 worktree `/media/jer/Videos/CCF-MFM08` was not touched (read-only `git status`/`rev-parse` only, clean at `d70cec17`).
