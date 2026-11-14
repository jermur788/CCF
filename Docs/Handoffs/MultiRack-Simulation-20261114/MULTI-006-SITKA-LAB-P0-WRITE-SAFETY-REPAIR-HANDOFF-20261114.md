# MULTI-006 — Sitka Lab P0 write-safety repair (R-1 / R-2): implementation handoff

**Date:** 14 November 2026
**Claim:** [CCF #510](https://github.com/jermur788/CCF/issues/510)
**Binding packet:** [MULTI-006 dispatch](https://github.com/jermur788/CCF/blob/docs/multi-006-sitka-lab-p0-write-safety-repair-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-HANDLE-ANCHORED-WRITE-REPAIR-DISPATCH-20261114.md)
**Repairs:** the findings from the independent [MULTI-005 review](https://github.com/jermur788/CCF/blob/review/multi-005-sitka-lab-p0-hardening-delta-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-HARDENING-INDEPENDENT-DELTA-REVIEW-20261114.md): R-1 MAJOR and R-2 MINOR.
**Status:** these are the worker's own results. Nobody has checked them independently yet. The next step is the **MULTI-007 focused delta recheck**, covering R-1 and R-2 only.

## 1. Identity and lineage

| Item | Value |
|---|---|
| Worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), running in Claude Code inside local T3 Code (Claude B — Pro provider). The packet recommended Sol/Codex High; the user's dispatch also allowed a capable local model. |
| Independence note | The implementer is Opus, so **the MULTI-007 recheck should be Sol/Codex or another non-Opus model** |
| Host | `jer-laptop`, Linux Mint 22.3, kernel 7.0.0-42-generic x86_64, i5-6300HQ |
| Toolchain | .NET SDK 8.0.133 (pinned in `global.json`), Node v26.13.1, Chromium 156.0.8078.79 |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-006-SitkaLab-WriteRepair-20261114` (created for this task) |
| Branch | `feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1` |
| Starting HEAD | `9ba88b6e3fff3d2ebff10fc88797bf73c0ebc5cc` (the MULTI-004 handoff HEAD; the branch had no other commits) |
| **Tested source commit** | **`ae05c50a11e1d8cd5145043c3a6341ffd0d9ddb1`**. Every gate below ran on this exact tree. |
| Handoff commit | the commit that adds this file, directly on top of `ae05c50a`. It changes no other file. |
| Coordination | #510 was set to [RUNNING](https://github.com/jermur788/CCF/issues/510#issuecomment-6883008705) before any edit or test. The live `active-tasks.md` has no MULTI/Sitka row, and no board edit was made. No Unity process was started. |

## 2. Files changed (against MULTI-004 `9ba88b6e`)

```
M Tools/MultiRackSitkaLab/README.md                     (+15/-8)
M Tools/MultiRackSitkaLab/src/Core/LabDataStore.cs      (+20/-24)
M Tools/MultiRackSitkaLab/src/Core/NoFollowFiles.cs     (+95/-2)
M Tools/MultiRackSitkaLab/tests/Core.Tests/LabDataStoreTests.cs  (+176/-3)
M Tools/MultiRackSitkaLab/tests/Host.Tests/HostTests.cs          (+52)
M Tools/MultiRackSitkaLab/tests/run-mutation-controls.py         (+14/-3)
A Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-WRITE-SAFETY-REPAIR-HANDOFF-20261114.md
```

Every changed path is on the packet's allowed list.

Untouched: `specs/**`, the fixtures and their manifest, the schema, `Engine`/`LabSession`/biology, the UI, the host source (`LabHost.cs`), `Assets/**`, `Packages/**`, `Docs/Project/**` and `main`.

The full patch is in the evidence folder as `candidate-diff-vs-multi004.patch`.

## 3. R-1: handle-anchored saves

**Before (MULTI-004).** `RealLabFileAccess.WriteAtomic(finalPath, bytes)` did three separate things, each by looking up the folder path again:
- `new FileStream(finalPath + ".tmp-…", CreateNew)`
- `File.Move(tmp, finalPath, overwrite:true)`
- `File.Delete(tmp)` on failure

A same-user `renameat2(RENAME_EXCHANGE)` of `.lab-data` with a symlink could therefore send each step to a different folder.

**After.** There is a new function, `NoFollowFiles.WriteAtomic(dir, name, bytes, stageHook)`, and `RealLabFileAccess.WriteAtomic(dir, fileName, bytes)` now just calls it. The interface seam changed from one absolute path to *(folder, bare name)*. The steps are:

1. **One folder open.** `openat(AT_FDCWD, dir, O_RDONLY|O_DIRECTORY|O_NOFOLLOW|O_CLOEXEC)` opens the folder. This is the existing fail-closed `OpenDirectory`. A link in that position is refused with `SymlinkRejected`, and any other failure to open for a save becomes `WriteFailed`.
2. **Temp file.** `openat(dfd, "<name>.tmp-<guid>", O_WRONLY|O_CREAT|O_EXCL|O_NOFOLLOW|O_CLOEXEC, 0600)` creates it inside that folder handle.
3. **Write.** `write` runs in a loop that retries on EINTR, carries on after partial writes and treats a zero-progress write as failure. A failed `close` is also treated as failure, because it can report a deferred write error.
4. **Rename.** `renameat(dfd, tmp, dfd, name)` uses the same handle on both sides.
5. **Cleanup on any failure after step 2.** `unlinkat(dfd, tmp, 0)` uses the same handle and this call's own unique name, so it can only ever remove this save's temp file.
6. The folder handle is closed in a `finally`.

**Other safeguards:**
- **Bare-name guard.** `WriteAtomic` refuses an empty name, `.`, `..`, or any name containing `/` or NUL (`PathRejected`). This is in addition to the store's existing name-shape check.
- **Self-test extended (fail-closed).** On first use the self-test now also checks four things in its private temp folder:
  - an exclusive create on an existing symlink fails with EEXIST;
  - create and write relative to the folder handle work;
  - `renameat` works;
  - `unlinkat` removes the file.

  If any of these fails, every lab file call returns `PlatformUnsupported` (501). The new O_* values (`O_WRONLY=1`, `O_CREAT=0x40`, `O_EXCL=0x80`) are the same on x64 and arm64.
- **Store.** `LabDataStore.WriteSave` keeps its early `Classify` refusal of symlinks, folders and special files. The store keeps `Resolve` only as a string check; the write itself never uses a joined path.

**Behaviour I deliberately left the same:**
- the allowed name shapes;
- the error codes and HTTP statuses;
- no path leaks in error messages;
- a folder or FIFO squatting the name is refused up front (409);
- a symlink squatting the name is refused up front (403);
- a late folder at the final name makes `renameat` fail, giving `WriteFailed`, with the temp file removed;
- a late symlink or FIFO at the final name is replaced by the rename and never written through (same as MULTI-004);
- an existing regular save can be overwritten;
- the read side is unchanged: no read code was modified, and the F-1/F-2 tests are untouched and pass.

**One small visible change:** new temp files, and therefore new saves, get mode `0600` (minus umask). Before, they got the `FileStream` default (`0666` minus umask). The packet asked for 0600.

**Not added:** `fsync`, any durability redesign, any Windows code.

### R-1 tests added

**Core tests** (`LabDataStoreTests`):
- **`FolderSwappedForALink_AfterTheFolderWasOpened_TempAndRenameStayInTheTrustedFolder`**
  - **Setup.** A deterministic hook runs at `WriteStage.DirectoryOpened` and exchanges `.lab-data` with a link to an "alternate" folder. The alternate folder holds a valid-looking `w.sitkalab.json` sentinel, a temp-like decoy and an unrelated file.
  - **Checks:**
    - the save reports success, truthfully;
    - the bytes are in the real, moved folder;
    - the alternate folder's listing and contents are unchanged;
    - no temp file is left in either folder;
    - the save loads normally once the folder is swapped back.
- **`FolderSwappedForALink_BetweenTempWriteAndRename_TheRenameStaysInTheTrustedFolder`.** The same check with the swap at `WriteStage.TempWritten`, while an existing save is being overwritten.
- **`FolderSwappedForALink_AndTheRenameFails_TheTempIsRemovedFromTheTrustedFolder_AndNothingElse`.** A folder appears at the final name and the lab folder is swapped. The checks:
  - the result is `WriteFailed`, never a success;
  - this save's temp file is removed from the real folder through the same handle;
  - the decoy, the alternate folder, the squatting folder and the neighbouring save are all untouched.
- **`ConcurrentFolderExchange_DuringSaves_…_AndOutcomesAreTruthful`.** This is the reviewer's W attack as a bounded test (2.5 s). It checks:
  - **Outcomes:** only `saved`, `SymlinkRejected` or `WriteFailed` occur.
  - **Alternate folder:** zero changes.
  - **Temp files:** zero orphaned.
  - **Truthful reporting** after every call. The test reads the real folder through a `/proc/self/fd/N` handle, so it works wherever the folder has been moved. After a success the real folder must hold exactly those bytes; after a failure it must still hold the previous save.
- `WriteAtomic_FailureAfterTheTempFileExists_…` (from MULTI-004) was updated only for the new `(dir, name)` signature. Its assertions are unchanged.

**Host test** (`HostTests.SavesDuringAConcurrentLabFolderExchange_…`):
- **Setup.** A real loopback host. A valid lab save from the oracle fixture is used as the sentinel in the alternate folder. HTTP saves run for 3 s while a thread runs `RENAME_EXCHANGE` on the lab folder.
- **Checks:**
  - **Responses:** only `200:ok`, `403:SymlinkRejected` or `500:WriteFailed`.
  - **Alternate folder:** zero overwrites, and nothing new created in it.
  - **Temp files:** none left in `.lab-data`.
  - **Data:** the lab folder holds a demo-fixture save that loads.
  - **Health:** the host is still healthy afterwards.

## 4. R-2: size growth after the statx check

Added **`ReadRegular_RefusesContentThatOutgrowsTheStatxSize_ProcfsStatus`** to `tests/Core.Tests/LabDataStoreTests.cs`. It uses `/proc/<numeric current PID>/status`. The numeric PID folder is needed because `/proc/self` is a symlink, which the no-follow folder open correctly refuses. The test:
- fails explicitly if procfs is missing, so it is never silently skipped;
- asserts that the reported size is 0;
- asserts that `Classify` returns `Regular`;
- **positive control:** reading with a 1 MiB limit returns the real content (more than 10 bytes, and it contains `Pid:`);
- reading with **max = 10** must throw **`FileTooLarge`**.

No production code changed for R-2.

## 5. Mutation controls

`tests/run-mutation-controls.py` changes:
- **M28** is rewritten for the new code. It tests the same safeguard: a failed save must remove its own temp file. The mutant is `if (!renamed) UnlinkAt(...)` → `if (false) ...`.
- **M31–M35** are new:

| ID | Mutation | Verdict (full run) | Caught by |
|---|---|---|---|
| M31 | temp file created by folder **path** (`AT_FDCWD`, `Path.Combine(dir,tmp)`) | **CAUGHT** (2) | concurrent exchange; swap after the folder open |
| M32 | rename by folder **path** | **CAUGHT** (3) | concurrent exchange; both stage-hook swaps |
| M33 | failure cleanup by folder **path** | **CAUGHT** (1) | rename-fails-under-swap (orphan in the real folder) |
| M34 | whole save **reverted to MULTI-004** `FileStream`/`File.Move`/`File.Delete`, with the stage hooks kept so the swap tests still fire | **CAUGHT** (4) | all four store write-swap tests |
| M35 (R-2 "XG") | delete the read loop's **second** size check | **CAUGHT** (1) | the procfs test only. The existing `OversizedFile_IsRefusedBeforeAnyRead` (first `statx` check) still passes under this mutant. |

**Full run** (`mutation/mutation-full.json`):
- baseline Core and Host suites green first;
- **35/35 CAUGHT**: original M01–M17 17/17, MULTI-004 M18–M30 13/13, MULTI-006 M31–M35 5/5;
- 0 SURVIVED, 0 BUILD-BROKEN;
- run on a disposable scratch copy; the tracked tree was not modified.

## 6. Run results (worker-side)

| Gate | Result | Evidence |
|---|---|---|
| Clean build (bin/obj deleted first), warnings are errors | **0 warnings, 0 errors** | `tier1/build.log` |
| Core suite | **82/82 PASS** (MULTI-004: 77; +5 new) | `tier1/core.log` |
| Host suite | **13/13 PASS** (MULTI-004: 12; +1 new). The swap test logged `swaps=316830 responses=403:SymlinkRejected=2422,200:ok=54 alternate-overwrites=0` | `tier1/host.log` |
| Fixture SHA-256 manifest, run from inside `fixtures/synthetic/` | **3/3 OK** | `tier1/fixture-manifest.log` |
| Schema check plus negative controls | **PASS** | `tier1/schema.log` |
| Mutation controls | **35/35 CAUGHT** | `mutation/` |
| Headless Chromium suite | **15/15 PASS**. UI-13 on the 2,000-tree fixture: max wall time 186 ms, mean 147 ms, max in-page 103 ms against a 2 s budget. This ran at the same time as the mutation run. | `tier2/browser-run.log`, `tier2/browser/` |

### Reviewer-owned tools: before and after

I copied the MULTI-005 reviewer's tools from `/media/jer/Files/CCF/Verification/MULTI-005-SitkaLab-DeltaReview-20261114/` without changing them. Their source hashes are in `reviewer-tools-copy/source-sha256.txt`. I ran them against a scratch copy of `9ba88b6e` (before) and of `ae05c50a` (after).

| Reviewer check | Before (`9ba88b6e`) | After (`ae05c50a`) |
|---|---|---|
| Probe `writeswap 3` (store) | 263,723 swaps. **100 outside-victim overwrites**. Temp orphans: **168 outside, 694 inside**. Outcomes: SymlinkRejected 21,695 / WriteFailed 1,702 / ok 2,555 | 263,899 swaps. **0 overwrites, 0 orphans outside, 0 inside**. Outcomes: SymlinkRejected 28,205 / ok 2,732. Unrelated file intact; alternate listing is just `w.sitkalab.json unrelated.txt` |
| Probe `growth` | max 10 and max 100 → FileTooLarge; 64 KiB returns content | same (no production change) |
| Probe `readswap 3` | 0 outside reads | 0 outside reads |
| Probe `dirswapexchange 3` (exchange with a **real** folder) | both contents returned | both contents returned. This is unchanged and expected; see §7 |
| Live host `smoke.py` | **21/23**. 55 outside overwrites (9 of them reported as 200 "saved"); orphans 105 inside, 29 outside | **23/23 on two runs**. Run 1: 282,945 swaps, 0 overwrites, 0 orphans. Run 2: 301,309 swaps, 0 overwrites, 0 orphans. Responses were only 403 SymlinkRejected or 200 ok. |

A note on `smoke.py`: it leaves its `swaplink` behind. My first attempt at run 2 reused the same copy and failed during the script's own setup (`FileExistsError`), before any check ran. I kept that log as `tier2/live-smoke-after-run2-attempt1-setup-error-stale-swaplink.log`, then repeated run 2 on a fresh copy.

## 7. Remaining limits (stated precisely)

- **What the guarantee is.** The guarantee is tied to the folder handle, not to the path. For one save, the folder at `<tool>/.lab-data` is opened once, without following links. Create, write, rename and cleanup then all happen in **that** folder, wherever it is moved during the save. A same-user process that renames the real folder away mid-save finds the save in the moved folder. That is expected, and it is what the tests check.
- **Swapping in a real folder.** A same-user process that renames a different **real** folder into `.lab-data` before the folder is opened has replaced the lab folder itself. That save then goes into the substituted folder. This is the same path-based limit the reviewer's `dirswapexchange` read probe shows. The limits on what can happen there:
  - links are never followed;
  - only a `<name>.sitkalab.json` entry plus our own temp file can be created or replaced there;
  - `RENAME_EXCHANGE` cannot cross filesystems (`EXDEV`).

  Closing this would mean pinning the folder's (device, inode) identity across saves, which is a design change outside this narrow repair. I did not do it; MULTI-007 / the Manager may want to decide on it.
- **Late squats at the final name.** A symlink or FIFO that appears at the final name between the early check and the rename is replaced (it is a directory entry in the real folder, never written through). A folder there gives `WriteFailed`.
- **Same-user tampering with our temp name.** If another process replaced our own GUID-named temp entry with its own file before a failed rename, our cleanup would remove that substitute. It could only happen inside the real folder, under our unique name.
- **Platform.** Only Linux x64 has been run. Linux arm64 is conditional and untested. Native Windows and macOS lab file operations still fail closed with 501. Nothing was added for Windows.

## 8. Scope confirmation

- No change to P0 gameplay, the scientific or biology scope, Engine/LabSession, fixtures, the schema, the UI or the host routing.
- No Unity, no real forest data, no P1 ecology, no protected or CCF saves touched, no spec edits, no Windows build, no coordination board edit.
- Every test used a throw-away temp or scratch tool root.
- Cleanup after the runs:
  - I stopped the MSBuild and VBCSCompiler nodes my builds left running, by PID; all of them started after this task began.
  - I deleted my scratch copies.
  - No host, Chromium or FIFO process is left running.
- Already on the host before this task, and left alone: 198 `/tmp/sitkalab-test-*` folders, none created after 19:20 UTC today.
- The worktree's gitignored `.lab-data/` is empty.

## 9. Evidence

Evidence folder: `/media/jer/Files/CCF/Verification/MULTI-006-SitkaLab-WriteRepair-20261114/` (on the Files drive, not in Git).

- **Manifest:** `MANIFEST.sha256` (37 entries), whose own hash is `bb8e50bca12db8437c0fea8ace386ee2f4260fc2c2401079e31152c9f205ee17`.
- **Key files:**

| File | SHA-256 |
|---|---|
| `tier1/core.log` | `cf22d9ff925a04e8641077f89e7686224f5ce16853ea1fa9666213a571679831` |
| `tier1/host.log` | `c14af5c7aa3aa4e9b596b77cb75cefbd949177e0423dc1f7881accc346a04760` |
| `mutation/mutation-full.json` | `667fffdd95a5beed59684b8f915f653efab847e5261fd571247faea4e9a7b741` |
| `reviewer-probes/probe-after.log` | `bc43a7a7ffff208cb49a7197d585f7bdd8fffcc44c83dbb9ecd1ff0350f96880` |
| `tier2/live-smoke-after-run1.log` | `d2fcc342de1c5b5e1df3b4d584f0cd6ad31aa415c5a18fd03760653ec4e490c6` |
| `tier2/browser/browser-results.json` | `544c71990c7c56fc300985836633f13b1eba1bf2a1bc79ed8deff8d7222b23eb` |

- **Other files:**
  - `environment.txt`
  - `candidate-diff-vs-multi004.patch`
  - `candidate-diff-names.txt`
  - `reviewer-probes/probe-before.log`
  - `tier2/live-smoke-baseline-9ba88b6e.log`
  - `reviewer-tools-copy/`

## 10. For MULTI-007 (focused recheck)

**Re-run these exactly:**
- the reviewer probe modes `writeswap`, `growth` and `dirswapexchange`, plus `tier2/smoke.py`, against `ae05c50a`. Remove `swaplink` between smoke runs.
- `MUT_ONLY=M28,M31,M32,M33,M34,M35 python3 -I -B tests/run-mutation-controls.py <scratch> <out.json>`;
- the Core and Host suites.

**Then decide:** whether the real-folder substitution limit in §7 is acceptable for P0, or needs inode pinning.

**Recheck worker:** non-Opus (Sol/Codex preferred).

The dashboard and Drive mirror have NOT been checked. Nothing has been merged, and the user has not accepted anything.
