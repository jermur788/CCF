# #312 — #287 registered-worktree migration: result

**Status: COMPLETE FOR OPERATIONS SCOPE — HANDED OFF FOR MANAGER #200 ACCEPTANCE. UNITY NOT RUN. NO LEASE.** A real canonical-store worktree now exists and is registered in the deployed lock registry. This is a worker report, not an acceptance; #287 stays BLOCKED/PAUSED until Manager accepts and a later fresh `unity-lock acquire` succeeds.

**Worker:** Claude Sonnet 5.5, Medium effort, Claude Code in T3 Code, local. Claim [#312](https://github.com/jermur788/CCF/issues/312). Packet: [MANAGER-287-REGISTERED-WORKTREE-MIGRATION-PACKET](MANAGER-287-REGISTERED-WORKTREE-MIGRATION-PACKET.md). Report branch `docs/ops-287-registered-worktree-migration-20261106-v1` (docs lane `/media/jer/Files/CCF/Operations/CCF-Ops287Migration-Docs-20261106`).

## Outcome

| Item | Value |
|---|---|
| New worktree | `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106` |
| Branch / HEAD | `feature/scenario-one-workbench-retirement-20261105` @ `5085e8afd5abb90533e834dd6bbe7444b23973a4` (parent `8b1946dcec00587971d1f7c80004a47ebaa88515`, tree `13d2fac8c9abebd17344e476ee201a351787c897`) |
| `git-common-dir` | `/media/jer/Files/CCF/Repository.git` (the configured store) |
| Lock lane | `WorkbenchTier2Reg287`, `unity: true`, path = the new worktree only |
| `lanes.json` sha256 | before `5ab2878595aecd30e6bc5fd8a807e5060a4d8684752e304d6d44c3d06ef5abc7` → after `8f1aade7a140c8abd55e925838fbaa51cf984bcd8a94d9e6861f199dc9655b0b` |
| Original clone | untouched: HEAD `5085e8af`, status empty, not moved, not deleted, not registered |
| Origin | unchanged: the branch is still `8b1946dc`; `5085e8af` was **not pushed** |

## Phase 0 — admission (read-only, all PASS)

- Original clone `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105`: tracked/untracked/ignored/index clean; HEAD `5085e8af` with the single parent `8b1946dc`; one commit ahead of origin; diff vs `8b1946dc` = exactly four paths, all verifier-only: `M Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs`, `M Tools/Verification/StandGeometry/Enlarged80Verification.cs`, `A Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs`, `A .../run_workbench_retirement.py`. Zero paths outside `Tools/Verification/`.
- Canonical store: 48 worktrees, none on the task branch; before the task only `refs/remotes/origin/<branch>` (`8b1946dc`) existed, no local branch; contains `8b1946dc`, lacked `5085e8af`; no running git writers; target path did not exist.
- `unity-lock` `free`, no Editor/Hub Unity process, storage READY, Files rw, no overlapping ops writer observed. Live coordination read at `origin/main`.
- **Capacity forecast** (bounded, no all-lane `du`): ZX20 30.03 GiB free vs 15 GiB floor. Checkout ≈1.9 GiB (measured 1.9 GiB on the old clone excluding `.git`; actual use 1.8 GiB). Cold `Library` ≈6.8 GiB (U1, MainInt277 and UIQual275 all measure 6.8 GiB). ⇒ about 6.4 GiB above the floor after a first import. Not a breach, but modest: a later ~3 GiB growth per commit switch has been observed in earlier runs.

## Phase 1 — recovery backup (`/media/jer/Files/CCF/Verification/Workbench287StoreRecovery-20261106/`)

`verifier-5085e8af.bundle` (sha256 `d6daa668…2d97`, prerequisite `8b1946dc`), `verifier-5085e8af.patch` (`83d35209…9c54`), `verifier-5085e8af.diff` (`3246ed27…0a5e`), `commit-object.txt` (`7f38c518…485d`), `SHA256SUMS`, `store-refs-before.txt`, `store-worktrees-before.txt`, `lanes.json.before-287-registration-20261107` (`5ab28785…ef5abc7`, byte-equal to the live file by `cmp`).
**Restore test PASS:** a scratch `--no-checkout` clone of the canonical store fetched the bundle and yielded `5085e8af`, parent `8b1946dc`, tree `13d2fac8…`, the same four-file `diff --name-status`, and a `diff --binary` byte-identical to the saved diff. The scratch clone was removed.

## Phase 2 — worktree and local transfer

`git --git-dir=Repository.git worktree add --no-track -b <branch> <new path> refs/remotes/origin/<branch>` (no `--force`, no prune/reset/rebase/cherry-pick), then `git fetch <bundle>` and `git merge --ff-only FETCH_HEAD` inside the new worktree. Verified: HEAD/parent/tree equal the original clone's; `git cat-file commit HEAD` byte-identical to `commit-object.txt`; `--show-toplevel` = the new path; `--git-common-dir` = `Repository.git`; the path is in `git worktree list`; clean including ignored files and index flags; no path outside `Tools/Verification/` differs from `8b1946dc`; `origin`'s branch still `8b1946dc`. The canonical store's refs changed by exactly one line: the new local branch (`diff` against `store-refs-before.txt`). No other worktree or ref was touched. No `Library` exists in the new worktree.

## Phase 3 — registry change

Single appended entry, placed after `MainInt277` by textual insertion so every other byte is unchanged:

```json
    {
      "lane": "WorkbenchTier2Reg287",
      "path": "/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106",
      "purpose": "#287 workbench-retirement Tier-2 qualification; real worktree of the canonical store, registered by #312 for the packet-named path",
      "unity": true
    }
```

Guard: the installed lock service serialises only on `flock` of `state/unity/guard` (the same lock `unity-lock acquire` holds while reading the config); `lanes.json` has no dedicated edit lock. I took that `flock` (opening the guard `a+` like the tool; never modifying or deleting it), re-checked inside it that no `lock.json` existed, no Editor ran and the live hash still equalled the backup, then wrote a same-directory temp file, `fsync`, `os.replace`.
**Staged validation (before install) PASS:** `_ccf.config_read` (schema, storage UUID/by-id, unique names and resolved paths, git_stores); storage PASS; `resolve_lane` finds exactly the new path by name, `CCF-` alias and path; it rejects the old clone, an unknown path and a partial name (`lane is not configured`); the checks `acquire` makes (`top == lane`, `common-dir == repository`, listed in the store's worktree list, no branch conflict, clean) all hold. **Post-install** the same config/resolution checks pass on the live file; the original 12 lane entries and every other top-level field equal the backup.

## Tests

| Item | Result |
|---|---|
| Phase 0 ownership/ancestry/store/branch/capacity checks | PASS |
| Backup + restore test | PASS |
| Worktree identity (HEAD, parent, tree, commit object, store, worktree list, clean) | PASS |
| One-entry delta, existing lanes and scripts unchanged (`unity-lock` `54cf0a95…f048e5`, `_ccf.py` `ab4343ef…de001`) | PASS |
| `unity-lock status --json` after: `free`, no metadata, no Editor PIDs; `lane-status --storage-only` READY 28.20 GiB free | PASS |
| `unity-lock acquire` (also scratch-state) | **NOT RUN by instruction** — registration is not proof a lease will succeed |
| Unity import/Editor, compile, any gate | **NOT RUN** |
| Compilation or correctness of the verifier commit | **NOT RUN** (still unbuilt/untested) |

One process note: I ran the full `lane-status` inventory by mistake after the checks; it started `du` over every lane, which the packet says not to do. I killed it (and its orphaned `du`) within seconds by PID; it only reads and changed nothing. Disk figures above come from `df`, not from it.

## Remaining risks and notes for Manager

- Capacity margin after a first import is about 6 GiB, and later commit switches or evidence runs on ZX20 can erode it; #287 needs its own fresh capacity decision at Unity admission.
- The old clone remains (1.9 GiB + 1.0 GiB `.git`) as a rollback anchor; deleting it was not authorised and would free ~3 GiB.
- The staged scratch copy is at `/media/jer/ZX20/CCF-Temp/287-registration-stage/lanes.json` (small, deletable).
- The new worktree's local branch is ahead of origin by `5085e8af`; whether and when to push it is Manager's call.

## Rollback

1. Config: under the same `flock`, with no lease/Editor, `cp /media/jer/Files/CCF/Verification/Workbench287StoreRecovery-20261106/lanes.json.before-287-registration-20261107` to a temp file beside `lanes.json`, check its sha256 is `5ab28785…ef5abc7`, then `os.replace` it onto `lanes.json`.
2. Worktree: only after Manager approval, `git --git-dir=/media/jer/Files/CCF/Repository.git worktree remove <new path>` and delete the local branch if wanted; the bundle/patch in the backup directory reproduce `5085e8af` from `8b1946dc`.

## Not done

No `unity-lock acquire`, lease, lock file or Unity process; no edit to production source, scene, assets, scripts, other lanes, `active-tasks.md`, #287, #308, #309, main, saves or builds; no push of `5085e8af`; the old clone was not moved, cleaned, deleted or repointed.
