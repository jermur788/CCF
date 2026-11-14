# MULTI-005: Independent focused delta review of the MULTI-004 Sitka Lab P0 hardening

**Date:** 14 November 2026 · **Claim:** [#504](https://github.com/jermur788/CCF/issues/504) · **Packet:** [MULTI-005 packet](https://github.com/jermur788/CCF/blob/docs/multi-005-sitka-lab-p0-independent-delta-review-packet-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-INDEPENDENT-HARDENING-DELTA-REVIEW-PACKET-20261114.md) · **Reviewed handoff:** [MULTI-004 implementation handoff](https://github.com/jermur788/CCF/blob/feature/multi-004-sitka-lab-p0-file-safety-hardening-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-004-SITKA-LAB-P0-HARDENING-IMPLEMENTATION-HANDOFF-20261114.md) · **Source task:** [#502](https://github.com/jermur788/CCF/issues/502)

## Verdict: **NEEDS CHANGES (narrow)**, for the MULTI-004 engineering P0 hardening delta only

The read-side repairs are sound, and I reproduced them independently: F-1 FIFO, F-2 handle-anchored no-follow open, F-3 honest race tests and F-4 layered-guard coverage. The F-5 error-code and temp-cleanup work is correct when nothing is racing it. Tier 1, mutation 30/30, browser 15/15 and the author's main claims all reproduced.

There is **one MAJOR finding (R-1)**. The save path is still path-based. A same-user process that swaps the `.lab-data` folder during a save makes the lab **write outside the lab root**. It also leaves **orphaned temp files inside `.lab-data`**, and in some cases reports the outcome wrongly. I reproduced this at the store level and through the real HTTP host. The author disclosed the risk, but called it merely "not defended". In practice it breaks the approved write boundary ("persist lab files only under the fixed root", AC-07 "symlink escapes fail") and F-5's own claim that a failed save leaves no temp file. The fix is small: write through the same directory handle the read path already uses.

There is **one MINOR finding (R-2)**: the second size guard has no test, but a deterministic, hook-free test is available. There are **no CRITICAL findings**.

**APPROVED would have meant only** that the bounded repair passes on Linux x64. Even this verdict says nothing about integration, cross-platform behaviour, P1, Irish forestry accuracy, Unity or user acceptance.

## 1. Worker, preflight and candidate attestation

| Item | Value |
|---|---|
| Reviewer | **Anthropic Claude Opus 5.5** (`claude-opus-5-5`), Claude Code in local T3 Code. **Not** the MULTI-004 author (Claude Sonnet 5.5). Separate session; no MULTI-004 session context. |
| Registration | [#504 RUNNING comment](https://github.com/jermur788/CCF/issues/504#issuecomment-6880796339), posted before creating the worktree, inspecting source or running tests |
| Host | `jer-laptop`, native Linux 7.0.0-42 **x86_64**, glibc 2.39, ext4 `/tmp` and Files drive |
| Toolchain | .NET SDK 8.0.133 (selected by `global.json`), Chromium 156.0.8078.79, Node v26.13.1, Python 3 (`-I -B`) |
| Review branch / starting HEAD | `review/multi-005-sitka-lab-p0-hardening-delta-20261114-v1` @ `9ba88b6e3fff3d2ebff10fc88797bf73c0ebc5cc` (0 ahead of the candidate) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-005-SitkaLab-DeltaReview-20261114`, new (confirmed absent before creation) |
| Off-repo evidence | `/media/jer/Files/CCF/Verification/MULTI-005-SitkaLab-DeltaReview-20261114/`, `sha256sums.txt` covers 35 files, manifest SHA-256 `6d673a5c1e89db69b478d5ee310a8ede63389473497fef91c974204a12c60b80` |
| Coordination | Live `origin/main` `active-tasks.md` has no MULTI/Sitka row. Absence of a row is not treated as ownership; the #504 claim tracks presence only. The MULTI-002/003/004 worktrees were not touched. No Unity, `dotnet`, Chromium or lab host process was running at start. The coordination anchor's unrelated dirty files (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`) were left alone. |
| Candidate diff | `git diff --name-status f33641f8 9ba88b6e`: **exactly 10 paths**, 9 under `Tools/MultiRackSitkaLab/` (2 added) plus the MULTI-004 handoff. Zero diff lines under `specs/`, `fixtures/`, `schemas/` and `src/Ui/`. |
| Candidate unchanged | `git ls-remote` before publishing: candidate branch and review branch both at `9ba88b6e…`. The review branch adds **only this Markdown file** (§8). |
| Processes at handoff | My lab hosts, Chromium and MSBuild/VBCSCompiler nodes (started 16:38 UTC by my own build) were stopped. No Unity was used. |

## 2. F-1 … F-6: findings by item

Line numbers refer to the candidate `9ba88b6e`, under `Tools/MultiRackSitkaLab/`.

| ID | Result | Evidence (independent unless marked) |
|---|---|---|
| **F-1** FIFO host hang | **PASS** | `ListSaves` keeps only `Classify == Regular` entries (`src/Core/LabDataStore.cs:110`). `Classify` is a handle-relative `statx(AT_SYMLINK_NOFOLLOW)` that never opens the entry (`src/Core/NoFollowFiles.cs:64-76`). The authoritative open adds `O_NONBLOCK` and refuses any non-regular **descriptor** (`NoFollowFiles.cs:84,98-106`). **Live host:** a real FIFO named `pipe.sitkalab.json` directly in `.lab-data` was not listed. 4 concurrent loads returned `403 NotListed` in 10–24 ms. 12 concurrent `GET /api/state` all returned 200 (max 12 ms), and the whole batch took 0.026 s. The FIFO was left in place. **Race-time FIFO:** `FifoSwappedInAfterEveryPreCheck…` passes, and mutant M25 (drop `O_NONBLOCK`) is caught by 2 tests. |
| **F-2** symlink/open race | **PASS (reads)** | The folder is opened with `O_RDONLY\|O_DIRECTORY\|O_NOFOLLOW` (`NoFollowFiles.cs:117-128`). The file is opened with `openat(dirfd, name, O_NOFOLLOW\|O_NONBLOCK\|O_CLOEXEC)`. Type, `nlink > 1` and size come from `statx(fd, "", AT_EMPTY_PATH)` (`:99-108`). Both fds are closed in `finally` (`:111,113`). strace confirms the syscalls: `openat(AT_FDCWD, ".../.lab-data", O_RDONLY\|O_NOFOLLOW\|O_CLOEXEC\|O_DIRECTORY)`, then `statx(<fd>, "", AT_EMPTY_PATH, …)`. **statx ABI:** offsets mask@0, nlink@16, mode@28 (u16), size@40 and the 256-byte buffer match the kernel `struct statx`. Incomplete masks fail closed (`:134`). **Attacks (reviewer-owned):** (a) store-level `RENAME_EXCHANGE` swap of the whole `.lab-data` with a symlink to an outside folder holding a same-named file, 494,188 swaps in 4 s: **0 outside reads** (1,006 lab reads, 79,659 `SymlinkRejected`). (b) Live HTTP race swapping the entry between a lab file and a symlink to **valid outside lab JSON** (a different fixture), 14,022 swaps in 6 s: **0 outside loads**, 546 inside loads, 1,943 refusals, 0 other. (c) A hard link to the outside valid file was refused (403). Because the type and link checks are made on the descriptor before `read`, and every outside-content attempt was refused, no outside bytes reached the session. |
| **F-3** misleading test | **PASS** | The old test is renamed honestly (`LinkReportedOnTheRoot_RefusesTheWholeStore_BeforeAnyOpen`). The new `HookedAccess.BeforeOpen` runs **inside `ReadRegularFile`**, after `ListSaves` and the per-entry `Classify` (`tests/Core.Tests/LabDataStoreTests.cs:44,56`). `AssertSwapWasInTheDangerousInterval` requires exactly 2 `Classify` calls that both saw `Regular` (`:212-217`), so only the open can refuse. With the authoritative guard disabled in scratch, the tests fail: M23 (no `O_NOFOLLOW` on the file open) is caught by 3 tests including the 2.5 s `rename(2)` stress test, M24 (no descriptor type check) by 3, and M27 (folder followed) by `LabFolderSwappedForASymlink…`. |
| **F-4** layered-guard coverage | **PASS** | M18–M22 (the reviewer's X01/X16/X06/X22/X08) are each **CAUGHT**, independently re-run. M21 is caught by 11 tests and M22 by 7, including `PerEntryPreCheck_IsAnIndependentLayer…`, which scripts `Classify` so that the open is never reached. The forged histories in `tests/Core.Tests/LayeredGuardTests.cs` recompute `postStateHash`, so only the replay guard can refuse, and they include a positive control. M01–M17 strings are **byte-identical** to the baseline script (no `-` lines in the diff of the `MUTATIONS` table). No production guard was removed or weakened: the diff only replaces the old path checks with `Classify` plus the authoritative open. X12/X20 remain unkillable as disclosed. Fabricating tests for them was correctly declined. |
| **F-5** atomic save / error handling | **PARTIAL** | **When nothing is racing, PASS:** folder squat → `409 NotRegularFile` with the folder's content kept; FIFO squat → 409 promptly; symlink squat → `403 SymlinkRejected` with the outside target unchanged; unwritable folder → `500 WriteFailed`; `PUT` → `405 MethodNotAllowed`; `text/plain` → `415 UnsupportedMediaType`; a 1.1 MB body → `413 PayloadTooLarge`. No temp file is left, and no path or stack trace appears in any message (`src/Host/LabHost.cs:130,161-175`). `File.Move(…, overwrite:true)` is a **single `rename(2)`** in strace, with no transient second link. **Under a concurrent folder swap, FAIL:** see R-1. Temp files are orphaned inside `.lab-data`, and `File.Delete(tmp)` (`LabDataStore.cs:56`) re-resolves the path and misses the temp file it created. |
| **F-6** fixture wording | **Accepted documentary limitation** | Spec, fixtures and schema are unchanged (0 diff lines). `sha256sum -c MANIFEST.sha256` passes 3/3 when run **from `fixtures/synthetic/`**; the packet's form, run from the tool root, fails only because the manifest lists bare file names. Only the 3-tree oracle is hand-audited. The 162- and 2,000-tree stands are formula-generated and pinned. This is a wording discrepancy, not a failure to model growth, and P0 has no growth by design. I did not edit the spec. |

## 3. Residual-risk assessments the packet required

### R-1 (MAJOR): folder swap during a save writes outside the lab root and orphans temp files

**Mechanism (verified by strace):** `WriteSave` classifies the name through a handle (`LabDataStore.cs:143`) and then calls `WriteAtomic(full)` with an **absolute path** (`:151`). That becomes `openat(AT_FDCWD, "/…/.lab-data/w.sitkalab.json.tmp-<guid>", O_WRONLY|O_CREAT|O_EXCL)` followed by `rename("/…/.lab-data/…tmp-<guid>", "/…/.lab-data/w.sitkalab.json")` (`:43-52`). The kernel resolves `.lab-data` again on each of those calls and follows it if it has become a symlink. `O_EXCL` and `rename` only protect the **last** path component.

**Reproduced at the store level** (4 s, 484,795 atomic `RENAME_EXCHANGE` swaps between `.lab-data` and a symlink to an outside folder):

- An existing outside `w.sitkalab.json` was **overwritten 1,223 times**.
- **571** `w.sitkalab.json.tmp-*` files were left in the outside folder.
- **5,200** orphaned temp files were left **inside `.lab-data`**.
- Outcomes: 3,478 `ok`, 8,856 `WriteFailed`, 25,274 `SymlinkRejected`. An unrelated outside file was untouched.

**Reproduced through the real host binary over HTTP** (5 s, 290,984 swaps):

- 12 outside overwrites, of which 2 were answered `200 "saved"` and the rest `500 WriteFailed` or `403 SymlinkRejected`. In those 403 cases the handler's post-save `ListSaves` refused after the bytes had already been written.
- 19 temp files outside, 58 orphaned temp files inside.
- The host stayed healthy.

**Threat model and bounds.** The attacker must be a **same-user local process** with rename rights on the lab folder's parent. That process could write these files itself, so there is no privilege escalation and no data disclosure; writes never return outside bytes. The targets are limited to names matching `^[A-Za-z0-9_-]{1,64}\.sitkalab\.json$` and their `.tmp-<32 hex>` siblings. CCF game saves (`forest-save.json`) and the frozen Reference Future **cannot be named**, so no protected file can be overwritten. The content written is always lab-serialised session JSON.

**Why MAJOR rather than MINOR for this delta:**

1. The approved spec's write boundary ("persist lab files only under the fixed root"; AC-07 "symlink escapes fail") is demonstrably false under this race.
2. F-5's guarantee ("removes only its own temp file", no orphan) is false under the same race, and some status codes misreport what happened.
3. MULTI-004's purpose was to close exactly this TOCTOU class. The read side now does, so the write side is the one remaining same-class gap.
4. The fix is small and mirrors code that already exists.

If the Manager prefers to accept it as a documented same-user caveat instead, that is a legitimate decision. It should be recorded explicitly, and the README sentence should then state the concrete consequences above rather than "not defended".

**Suggested narrow repair** (for the Manager to dispatch; not done here):

1. Open the folder once per save with the existing `OpenDirectory`.
2. Create the temp file with `openat(dfd, tmp, O_WRONLY|O_CREAT|O_EXCL|O_NOFOLLOW|O_CLOEXEC, 0600)`, then write.
3. `renameat(dfd, tmp, dfd, final)`.
4. On failure, `unlinkat(dfd, tmp, 0)`.
5. Optionally `fsync` before the rename.
6. Add a reviewer-style `RENAME_EXCHANGE` folder-swap stress test asserting zero outside writes and zero orphans, plus a mutant that restores the path-based write.

### R-2 (MINOR): size growth after `statx`

The bounded read loop does work. Reading `/proc/<pid>/status` through `NoFollowFiles.ReadRegular` is a deterministic case of a file that "grew after inspection": `statx` reports size 0, but 1,539 bytes come back. It was refused `FileTooLarge` at max 10 and 100 bytes, and returned at 64 KiB. Memory is capped at max + 64 KiB.

The guard has **no test**, though. A reviewer mutant (XG) that deletes the loop check (`NoFollowFiles.cs:162`) **survives Core 77/77**, while the `/proc` probe kills it (1,539 bytes returned at max 10). The author said a deterministic test needed a libc hook; the `/proc` file gives one without a hook. Add it in the R-1 repair.

### Linux-only, fail-closed portability

- **Linux x64:** executed, all results above.
- **Linux arm64:** **NOT TESTED.** I checked statically that the constants match the arm64 UAPI headers: `O_DIRECTORY` 0x4000, `O_NOFOLLOW` 0x8000. `O_NONBLOCK`, `O_CLOEXEC`, `AT_*` and the errno values follow the generic headers, and the statx layout is architecture-independent. The self-test would catch wrong flag values, but this remains conditional until it runs on arm64.
- **musl and Linux older than 4.11** (no `statx`) were not tried. They would fail closed through the self-test.
- **Windows and macOS:** by code reading, `RequireSupported` throws `PlatformUnsupported`, which maps to **HTTP 501** for list, load and save (`LabHost.cs:168-175`). The host itself still starts, because `EnsureRoot` does not use `NoFollowFiles`. Not executed.
- **Material product decision for the Manager:** the P0 lab now has **working saves on Linux only**. Native Windows (never qualified by MULTI-002 or MULTI-003) moved from "racy path checks" to "refused". If a Windows-native lab is ever wanted, it needs a separate handle-based implementation (for example `CreateFileW` with `FILE_FLAG_OPEN_REPARSE_POINT` plus by-handle checks). A Linux-only working lab is **not** cross-platform P0.

### Informational notes (no action required for this delta)

- **N-1 Root identity is by name.** Exchanging `.lab-data` with another **real** directory (no symlink) makes reads come from that directory: 9,398 of 18,721 reads in a 2 s probe. This is not a symlink escape; a same-user actor could equally move files in. The R-1 repair does not need to change it.
- **N-2 Hard-link refusal is a heuristic.** Link-then-unlink defeats it in the same way as `mv`. It is useful hygiene, not an isolation boundary.
- **N-3** A load refused as `FileTooLarge` returns HTTP 400 (413 is reserved for request bodies). The code is consistent, so this is acceptable.
- **N-4** The `PlatformUnsupported` message embeds the self-test's exception text, which could include a temp path, but only on unsupported or failing platforms. Low.
- **N-5** There is no `fsync` before the rename (durability, outside P0 scope).
- **N-6** The `/tmp` count of `sitkalab-test-*` folders was 198 before and after all my runs, and no self-test or probe folders were left behind.

## 4. Tests actually executed by this reviewer vs author claims

| Gate | Command (from `Tools/MultiRackSitkaLab` unless noted) | **Reviewer result (independent)** | Author claim |
|---|---|---|---|
| Clean build | `dotnet build MultiRackSitkaLab.sln --no-incremental` (fresh worktree, no `bin`/`obj`) | exit 0, **0 warnings, 0 errors** | same |
| Core | `dotnet run --no-build --project tests/Core.Tests` | **77/77** | 77/77 |
| Host | `dotnet run --no-build --project tests/Host.Tests` | **12/12** | 12/12 |
| Fixtures | `cd fixtures/synthetic && sha256sum -c MANIFEST.sha256` | **3/3 OK** (fails from tool root: path form only) | 3/3 |
| Schema | `python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json` | **3 OK + negative controls rejected** | same |
| Mutation | `python3 -I -B tests/run-mutation-controls.py <scratch> <json>` (script's own scratch copy; baseline green) | **M01–M17: 17/17 CAUGHT; all 30/30 CAUGHT** | 17/17, 30/30 |
| Reviewer mutant XG | delete the read-loop size check in a scratch copy | **SURVIVED** Core 77/77; killed by the `/proc` probe | disclosed as untested |
| Store attack probes | reviewer `Probe.dll growth / readswap / writeswap / dirswapexchange` (scratch copy) | growth guard OK; **0 outside reads**; **write escape (R-1)**; N-1 | read race 0 outside (reproduced); write race not quantified |
| Live host smoke | reviewer `smoke.py` against the real `SitkaLab.Host.dll` from a scratch copy | **21/23**: all F-1/F-2/F-5 checks PASS; the 2 R-1 checks FAIL | 18/18 (author's own script, not re-run) |
| Headless Chromium | `EVIDENCE_DIR=… CHROMIUM=/usr/bin/chromium node tests/Ui/run-browser-tests.mjs` (fresh scratch copy) | **15/15**; 2,000-tree action **max 155 ms, mean 142 ms**, 21 samples, budget 2,000 ms | 15/15, max 162 ms |
| MULTI-004 evidence | `sha256sum -c sha256sums.txt` in `/media/jer/Files/CCF/Verification/MULTI-004-SitkaLab-Hardening-20261114/` (read-only) | **37/37 OK**, manifest hash `7f6a4784…a9b7` matches the handoff | 37 files |

**Not reproduced and still writer-only:** the author's pre-fix "before" numbers on `f33641f8` (the MULTI-003 probe and the 6/15 smoke) and the author's smoke script. **Not tested:** headed browser, Firefox, WSL, macOS, Windows, arm64, musl, P1, Unity.

**Probe method.** All probes ran on off-repo copies under the session scratchpad. They used `renameat2(RENAME_EXCHANGE)` and `rename(2)` swappers. Outside folders were inspected only through their real paths, never through the swapped link. Each probe was time-bounded (2–6 s) and nothing hung. Probe sources, logs, strace extract, mutation JSON and browser screenshots are in the evidence folder (`probes/`, `tier1/`, `tier2/`, `mutation/`).

## 5. Defect summary

| ID | Severity | Finding |
|---|---|---|
| **R-1** | **MAJOR** (same-user integrity; not privilege escalation) | Path-based `WriteAtomic` writes outside `.lab-data`, orphans temp files inside it and can misreport the result when the folder is swapped during a save (`LabDataStore.cs:41-58,151`) |
| **R-2** | MINOR | Post-`statx` growth guard (`NoFollowFiles.cs:162`) works but no test kills its removal; a deterministic `/proc` test is available |
| N-1…N-6 | info | See §3 |

No CRITICAL findings. F-1, F-2 (reads), F-3 and F-4 PASS. F-5 is PARTIAL because of R-1. F-6 is an accepted wording limitation.

## 6. Next step for the Manager

1. Issue a **narrow repair** limited to `src/Core/NoFollowFiles.cs`, `src/Core/LabDataStore.cs` (`WriteAtomic`/`WriteSave`), the store tests, the mutation script and the README sentence. It should cover R-1 (handle-relative `openat`/`renameat`/`unlinkat` save, folder-swap stress test, mutant) and R-2 (`/proc` growth test plus mutant).
2. Follow it with a **focused recheck**, not a new full review: re-run Tier 1, the 30+ controls, and the reviewer write-swap probe and smoke (both in the evidence folder).
3. Separately, decide whether **Linux-only lab saves** are acceptable product scope, and whether an arm64 run is needed.

Alternatively, the Manager may explicitly accept R-1 as a documented same-user caveat and mark the hardening qualified with that exception recorded. That choice belongs to the Manager, not to this reviewer.

**Not claimed:** no source, test, spec, fixture or coordination edits; no merge; no Unity; no P1; no real or Irish data; no production saves; no Windows build; no user acceptance. Dashboard/Drive mirror sync: **NOT VERIFIED**.
