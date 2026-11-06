# #311 — #287 Workbench clone lock-lane registration: result

**Status: BLOCKED — NO CHANGE MADE.** `/media/jer/ZX20/CCF-Ops/lanes.json` was not edited. No backup was needed, no lease was taken, no Editor was started. Date 2026-11-06. This is a worker report for Manager #200, not an acceptance.

**Worker:** Claude Sonnet 5.5, Medium effort, Claude Code in T3 Code, local. Claim [#311](https://github.com/jermur788/CCF/issues/311); packet [MANAGER-287-LOCK-LANE-REGISTRATION-PACKET](MANAGER-287-LOCK-LANE-REGISTRATION-PACKET.md) at main `a1f9851f` (read from `origin/main`).

## Why blocked

A `lanes.json` entry cannot make `unity-lock acquire` succeed for the Workbench clone, because the clone is not in the Git store the lock service requires. The packet says to stop and report if the installed API needs anything beyond the single lane entry; that is the case.

`scripts/unity-lock` (acquire, sha256 `54cf0a95…f048e5`) runs, in order: `resolve_lane` (the error #287 hit), `configured['unity']`, then

```python
top=...git rev-parse --show-toplevel ; common=...git rev-parse --path-format=absolute --git-common-dir
if top!=lane or common!=Path(cfg['repository']).resolve(): raise ValueError('unexpected lane Git store')
...
if not any(Path(c['path']).resolve()==lane for c in store_checkouts(cfg)): raise ValueError('Unity lane is not registered')
```

Observed (read-only) for `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105`:

| Check | Required | Observed |
|---|---|---|
| `--show-toplevel` equals lane | yes | `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105` — PASS |
| `--git-common-dir` equals `cfg['repository']` | `/media/jer/Files/CCF/Repository.git` | `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105/.git` — **FAIL** |
| Listed by `git --git-dir=Repository.git worktree list` | yes | **absent** (every registered lane such as U1, MainInt277, NextActionGuidance is listed) — **FAIL** |

The #284 handoff records why: the canonical store was read-only when the clone was made, so a self-contained clone was used. `Repository.git` is now on an `rw` mount (`/media/jer/Files`, touch test on `Verification/` passed), but the existing clone was never converted.

Adding the lane entry alone would therefore only move the failure from `lane is not configured` to `unexpected lane Git store`, while changing a machine-wide shared file. I did not do that. Fixing it needs one of the actions this packet forbids this task: editing `scripts/unity-lock`/`_ccf.py`, moving or recreating the clone, or copying/pushing local verifier commit `5085e8af`.

## Phase 0 receipts (all read-only)

- `lanes.json` sha256 `5ab2878595aecd30e6bc5fd8a807e5060a4d8684752e304d6d44c3d06ef5abc7`; `_ccf.py` `ab4343ef…de001`; unchanged by this task. Schema 1; storage uses the by-id device path (not touched). Lane entries are `{lane, path, purpose, unity:bool}`; `config_read` requires unique lane names and unique resolved paths, and Unity lanes must resolve under ZX20.
- `unity-lock status --json`: `free`, `metadata: null`, no Editor PIDs. No `6000.6.0f1/Editor/Unity` process. `lane-status --storage-only`: READY/PASS, `/dev/sdc2`, 30.03 GiB free, floor 15 GiB. `Files` backup destination writable.
- Workbench clone: branch `feature/scenario-one-workbench-retirement-20261105`, HEAD `5085e8afd5abb90533e834dd6bbe7444b23973a4` (local-only verifier commit) over `8b1946dc`; tracked/untracked status empty. Left untouched and unpushed.
- Target docs branch `docs/ops-unity-lock-287-registration-20261106-v1` was absent on origin before this report.

## Tests

| Item | Result |
|---|---|
| Installed schema and lane resolution read | PASS |
| Lock free, no Editor, storage healthy, Files rw | PASS |
| Clone Git store matches `cfg['repository']` | **FAIL** |
| Clone listed as a worktree of the configured store | **FAIL** |
| Backup of `lanes.json`, one-entry edit, guard/flock install, post-change resolution | **NOT RUN** (stopped before any write) |
| Scratch-config `--test-state-dir` acquire test | **NOT RUN** (avoids any acquire; the two FAILs above come from the same git commands the tool runs) |

## Options for Manager #200 (recommendation first)

1. **Recommended:** create a proper worktree of `Repository.git` for #287 on ZX20, e.g. `git --git-dir=/media/jer/Files/CCF/Repository.git worktree add /media/jer/ZX20/<NewLane> feature/scenario-one-workbench-retirement-20261105` (the branch is on origin at `8b1946dc`), bring `5085e8af` over from the existing clone with a separately authorised local fetch/bundle, then register that path with a single `lanes.json` entry under this packet's backup/guard rules. The old clone stays as the source until verified. This needs a Manager packet because it moves/copies the verifier commit and creates a cold Unity `Library` (~30 GiB free, 15 GiB floor).
2. Convert the existing clone in place (re-point it at `Repository.git`). Higher risk; not recommended.
3. Change the lock service to accept self-contained clones. Weakens the exclusive-lane safeguards; not recommended.

## Rollback / idempotency

Nothing was changed, so nothing to roll back. A future registration should back up `lanes.json` to `/media/jer/Files/CCF/Verification/UnityLockLane287-20261106/` and compare hashes before and after.

## Not done

No `lanes.json` edit, no lease, no Editor, no clone move, no push of `5085e8af`, no change to #287, #308, #309, main, saves or builds. #287 is not qualified.
