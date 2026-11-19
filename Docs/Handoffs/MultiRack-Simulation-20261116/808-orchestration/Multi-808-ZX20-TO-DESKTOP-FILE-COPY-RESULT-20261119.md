# Multi #808 — ZX20 → desktop verified file copy — **PASS**

**19 November 2026.** Result of [the #808 handoff](https://github.com/jermur788/CCF/blob/docs/multi-730-808-simple-desktop-copy-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-808-SIMPLE-ZX20-TO-DESKTOP-COPY-HANDOFF-20261119.md). Copy only: nothing extracted, cloned, built, merged, deleted or integrated.

## Actual paths and machine

| Item | Actual value |
|---|---|
| Machine | `DESKTOP-DL1BQI7`, Microsoft Windows 11 Pro 10.0.26300 (build 26300) |
| Operator | T3 Code desktop agent session, authenticated `jermur788` GitHub, live local filesystem access |
| Source (untouched) | `D:\CCF-Transfers\Multi-20261119` |
| Source volume | `D:` label `ZX20`, exFAT, volume serial **18D6-896E** (matches the #789 reported exFAT UUID), 476.91 GiB total, 341.61 GiB free |
| Destination (new) | `C:\CCF-Archives\Multi-20261119` |
| Destination volume | `C:` NTFS fixed local disk, 237.42 GiB total |
| Destination free space | 96.808 GiB before copy, 95.657 GiB after (≈1.15 GiB consumed; the required ≥3 GiB headroom was present before the copy and still remains) |
| Method | `robocopy /E /COPY:DAT /DCOPY:DAT /R:2 /W:2` — copy only, no move/cut, no `/MIR`, no deletion, no overwrite of pre-existing data (destination folder did not exist) |

## Copy result

| Measure | Source | Destination |
|---|---:|---:|
| Files | 10 | 10 |
| Directories (incl. root) | 4 | 4 |
| Total bytes | 1,237,496,210 (1.153 GiB) | 1,237,496,210 (1.153 GiB) |
| Robocopy exit | — | `1` (files copied successfully; < 8 = success) |
| Elapsed | — | ~8 s (≈141.9 MB/s) |

Contents copied in full: `viewer/` (3 files), `engine/` (3 files), `engine/evidence/` (1 file), plus `SHA256SUMS`, `MANIFEST.txt` and `RESTORE.md`. No file, folder or byte omitted, including both Git bundles and all evidence `.tar` files.

## Verification

1. **Source integrity first** — `SHA256SUMS` validated against the ZX20 files before copying: **9/9 OK**.
2. **Relative file list and sizes** — recursive source vs destination comparison of every file's relative path and byte length: **IDENTICAL** (10/10 files, both 1,237,496,210 bytes). Directory structure: **IDENTICAL** (`engine`, `engine\evidence`, `viewer`). No extra or missing destination files.
3. **All nine SHA256SUMS entries against the destination files** — **9/9 OK**:

| Entry | Result |
|---|---|
| `./engine/ccf-y2-10-engine-20261119.gitbundle` | OK |
| `./engine/ccf-y2-10-evidence-20261119.tar` | OK |
| `./engine/ccf-y2-10-lane-snapshot-20261119.tar` | OK |
| `./engine/evidence/HANDOFF.md` | OK |
| `./MANIFEST.txt` | OK |
| `./RESTORE.md` | OK |
| `./viewer/ccf-year0-viewer-20261119.gitbundle` | OK |
| `./viewer/ccf-year0-viewer-evidence-20261119.tar` | OK |
| `./viewer/ccf-year0-viewer-snapshot-20261119.tar` | OK |

4. **Timestamps preserved** — all 10 destination `LastWriteTimeUtc` values equal their source values (original `2026-11-19T09:19Z`–`09:30Z` range).

## Copy completeness and source safety

- **Copy fully complete: YES.** Destination is a byte-identical, structure-identical copy of the final #789 archive set. The 1,202,316,983-byte corrected full-history viewer bundle (`9e6e2904…`) is the one present; no failed/old shallow bundle exists in the set.
- **Source untouched:** ZX20 was only read, never written or renamed. Post-copy the source still holds 10 files / 1,237,496,210 bytes with unchanged timestamps and the same 341.61 GiB free space.
- **No failures, retries or partial copies** occurred; no cleanup or deletion was performed at either end.

## Not done, by design

No archive extraction or `tar` unpack, no `git bundle` verify/clone, no build or test, no WSL2 setup, no Unity/.NET/Node, no Windows game build, no main merge or integration, no change to #784/#789 source or fixtures, no Host/save attempt, no physical Ops board or L4 status change. No deletion of ZX20 contents or laptop originals. Restoration into working development repositories remains a separate step per the [#801 desktop guide](https://github.com/jermur788/CCF/blob/docs/multi-801-gpt6-desktop-restore-dossier-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-801-DESKTOP-RESTORE-AND-INDEPENDENT-QUALIFICATION-ACCEPTANCE-DOSSIER-20261119.md), run on ext4/WSL2 — never from exFAT and never from this NTFS copy for Git work.

**Verdict: PASS — verified desktop archive copy only.**
