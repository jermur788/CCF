# CCF Storage Architecture

Status: operational policy, applied 2026-10-21 by task `CCF-STORAGE-ARCHITECTURE-MIGRATION-V1`.
This is not a gameplay decision or a canonical-context commit.

## Verified storage devices

| Role | Mount | Device | Filesystem | Size |
|---|---|---|---|---|
| Active-work SSD | `/media/jer/ZX20` | `/dev/sda2` (label `ZX20`) | exFAT | 477 G |
| Durable storage HDD | `/media/jer/Files` | `/dev/sdc4` (label `Files`) | ext4 | 465 G |

`FILES_ROOT=/media/jer/Files`

`ZX20` is exFAT. Symlink creation on it fails with `Operation not permitted`, and
passwordless `sudo` is unavailable, so transparent bind mounts are not used.
Historical `/media/jer/ZX20/CCF-*` evidence paths are therefore not transparently
valid after migration; translate them with the path map below.

## Policy

Keep on ZX20 only material whose active use materially benefits from SSD I/O:

- at most two authorised active Unity worktrees (currently U1 and U2);
- their `Assets/`, `Packages/`, `ProjectSettings/` and warm `Library/`;
- temporary performance-sensitive Player profiling;
- the Unity Editor installation managed by Unity Hub.

Put on the Files HDD:

- verification evidence and accepted receipts;
- research material and generated mockups;
- Player builds and historical build evidence;
- archives, Git bundles and recovery material;
- standalone documentation/reference assets;
- operational records and inactive/non-performance-sensitive checkouts.

Delete rather than archive:

- inactive Unity worktree caches (`Library/`, `Temp/`, `obj/`, `Logs/`);
- disposable review/trial/preflight worktrees once their refs are preserved;
- scratch verification material and compiler intermediates;
- superseded build caches.

Do not keep a path on ZX20 merely because an old handoff records its absolute
path. Update the new default or translate the old path instead.

## Canonical Git store

The shared bare repository now lives at:

`/media/jer/Files/CCF/Repository.git`

Linked worktrees remain on ZX20. The move was verified with `git fsck`,
586 refs preserved, worktree registrations repaired, and identical
`git status` output for CCF-Primary, CCF-U1, CCF-U2 and CCF-Integration
before and after cutover.

## Operational tooling

- `CCF-Ops/lanes.json` now points `repository` at `/media/jer/Files/CCF/Repository.git`.
- Unity lanes U1/U2 remain on ZX20 and are the only lanes the Unity lock may acquire.
- The non-Unity lane was recreated on the Files HDD at
  `/media/jer/Files/CCF/Operations/CCF-NonUnity`; it is never Unity-capable.
- `CCF-Ops/environment.sh` validates the empty ZX20 scratch directory
  `/media/jer/ZX20/CCF-Temp` and the durable Files directories
  `Verification/` and `Builds/`, and exports `CCF_VERIFICATION_ROOT`.
- `CCF-Ops` itself stays on ZX20 because it holds the live machine-local Unity
  lock/board state and is only 7 MB.

## Path map

| Previous ZX20 path | Current path |
|---|---|
| `/media/jer/ZX20/CCF-Verification/` | `/media/jer/Files/CCF/Verification/` |
| `/media/jer/ZX20/CCF-Research/` | `/media/jer/Files/CCF/Research/` |
| `/media/jer/ZX20/CCF-Builds/` | `/media/jer/Files/CCF/Builds/` |
| `/media/jer/ZX20/CCF-s1-80m-local-output/` | `/media/jer/Files/CCF/Builds/S1-80m-local-output/` |
| `/media/jer/ZX20/CCF-p2-bimodal-local-output/` | `/media/jer/Files/CCF/Builds/P2-bimodal-local-output/` |
| `/media/jer/ZX20/CCF-integration-s1a/Build/` | `/media/jer/Files/CCF/Builds/S1A-legacy-evidence/` |
| `/media/jer/ZX20/CCF-Media/` | `/media/jer/Files/CCF/Media/` |
| `/media/jer/ZX20/Unity Assets/` | `/media/jer/Files/CCF/Archives/UnityAssets/` |
| `/media/jer/ZX20/CCF-Repository.git/` | `/media/jer/Files/CCF/Repository.git/` |

Migration receipts, before/after status, hash spot checks and inventories:

`/media/jer/Files/CCF/Operations/StorageMigration-20261021/`

Dirty material archived before legacy checkout removal:

`/media/jer/Files/CCF/LegacyPatches/`

## Current ZX20 CCF paths

| Path | Reason retained on SSD |
|---|---|
| `/media/jer/ZX20/CCF-U1` | active Unity lane with warm Library |
| `/media/jer/ZX20/CCF-U2` | active Unity lane; warm Library restored to SSD |
| `/media/jer/ZX20/CCF-Integration` | frozen Scenario One integration lane; preserved untracked `Scenario1V1/` |
| `/media/jer/ZX20/CCF-Primary` | live coordination anchor and current T3 workspace; move after session handoff |
| `/media/jer/ZX20/CCF-Ops` | live machine-local Unity lock/board state (7 MB) |
| `/media/jer/ZX20/Unity/6000.6.0f1` | Hub-managed Unity Editor installation |

## Future creation rule

Before writing a new CCF path on ZX20 ask: does this materially benefit from
SSD I/O?

- Yes: ZX20 may be appropriate (active Unity working set, Library, profiling).
- No: write under `/media/jer/Files/CCF/`.

New verification tooling should default durable output to
`/media/jer/Files/CCF/Verification` and may honour an explicit
`CCF_VERIFICATION_ROOT` override. Explicit caller-supplied output paths remain
valid.

## Migration result (2026-10-21)

- Task brief baseline: 5.10 GiB free.
- Measured at execution start: 16.14 GiB free (earlier partial cleanup/state difference).
- Measured after migration: 54.06 GiB free.
- Net gain during this task: 37.92 GiB (48.96 GiB against the brief baseline).
- Moved: verification (19,647 files, 10.48 GB), research, builds (including S1-80m and S1A legacy evidence), media (2.60 GB), Unity asset sources (3.54 GB), and the shared bare Git store (3.97 GB).
- Deleted: five legacy ZX20 checkouts, seven retired registered worktrees (one recreated on the Files HDD as the non-Unity lane), one standalone clone, scratch/temp, and compiler intermediates. Branch refs and approved candidate commits were preserved in the bare store.
- Restored: U2's warm Library (3.15 GB / 28,921 files) from the temporary Files cache back to ZX20; the duplicate HDD cache was retired.

## Out of scope

`/media/jer/ZX20/Kinsale` (~382 GB of video editing media) is not CCF material
and was not moved or deleted. It remains the largest consumer of ZX20 and
should be relocated separately if the user wants the SSD fully dedicated to
active development.
