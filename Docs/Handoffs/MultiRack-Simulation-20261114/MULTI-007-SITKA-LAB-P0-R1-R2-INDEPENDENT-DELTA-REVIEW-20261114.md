# MULTI-007 — Independent focused R-1/R-2 delta review of the MULTI-006 Sitka Lab P0 write-safety repair

**Date:** 14 November 2026
**Claim:** [CCF #511](https://github.com/jermur788/CCF/issues/511)
**Scope:** the MULTI-006 delta only (R-1 MAJOR handle-anchored saves, R-2 MINOR post-`statx` growth guard). Not a full P0 requalification, not a forestry or game review.

## 0. Verdict

**APPROVED — MULTI-006 engineering P0 write-safety repair only, with one new MINOR finding (R-3) and four non-blocking observations.**

| Item | Result | Basis |
|---|---|---|
| **R-1** same-user folder exchange during a save (MAJOR in MULTI-005) | **PASS** for the contract that was broken: no write outside the opened folder, no orphaned temp file, no false "saved". | REPRODUCED (tests, live host, independent probes, syscall trace, mutants) |
| **R-2** content larger than the `statx` size (MINOR in MULTI-005) | **PASS** | REPRODUCED (test, M35 caught only by the new test) |
| **R-3 (new, MINOR, not a regression)** host can answer `403 SymlinkRejected` for a save that did land in the trusted (moved) folder | Recorded; non-blocking | REPRODUCED twice; pre-existing on MULTI-004 |
| Real **directory substituted before open** | Classified as an honest same-user/local-adversary limit, **not** the repaired race, **not** a reason for NEEDS CHANGES | REPRODUCED (deterministic and racing probes) + approved-spec reading |

This is **not** a source merge, Scenario One or Unity integration, field correctness, P1 ecology, a Windows game build or user acceptance.

## 1. Reviewer identity and source-unchanged attestation

- **Actual reviewer:** Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code in local T3 Code. Non-Opus. It did not author MULTI-006 (Claude Opus 5.5) or review MULTI-005. [RUNNING registration](https://github.com/jermur788/CCF/issues/511#issuecomment-6883830595) was posted on #511 before any source inspection, test or write.
- **Host:** `jer-laptop`, Linux 7.0.0-42-generic x86_64. Tool SDK .NET 8.0.133 (`global.json`, `rollForward: latestPatch`). Node 26.13.1, Python 3, system Chromium.
- **Worktree:** `/media/jer/Files/CCF/Operations/CCF-MULTI-007-SitkaLab-WriteDeltaReview-20261114`, created fresh by this reviewer (path did not exist). **Branch:** `review/multi-007-sitka-lab-p0-write-safety-delta-20261114-v1`. **Starting and candidate HEAD:** `c445f8700610a7e6a835fef9ecec39eb9432128d` (matches the pinned candidate; GitHub compare of review branch vs candidate before this report: `identical`, 0 files).
- **Preflight:** live `main` `Docs/Project/coordination/active-tasks.md` has **no MULTI row** (the only "Sitka" hits are Sitka-spruce mentions in unrelated Scenario One entries). No conflicting worktree or branch for MULTI-007, and no `dotnet`, Chromium or Unity process at start. MULTI-002..006 worktrees were not touched. No Unity lease, no coordination write, no Windows build, no production save.
- **Source unchanged:** no implementation, test, spec, fixture, schema, coordination or Unity file was edited. All attack and mutation work ran on disposable copies under the off-repo evidence folder; the review worktree held only gitignored build output, which was removed (`git clean -fdX`) after the runs. The one tracked change is this Markdown file.

## 2. R-1 — handle-anchored saves: **PASS**

### 2.1 Source inspection (`NoFollowFiles.cs`, `LabDataStore.cs`)

| Step | Code | Handle used |
|---|---|---|
| Folder open, once | `OpenDirectory(dir, forWrite:true)`, `NoFollowFiles.cs:148`, defined `:178-190` — `openat(AT_FDCWD, dir, O_RDONLY\|O_DIRECTORY\|O_NOFOLLOW\|O_CLOEXEC)` | the only path lookup in the save |
| Temp create | `:153` `OpenAtCreate(dfd, tmp, O_WRONLY\|O_CREAT\|O_EXCL\|O_NOFOLLOW\|O_CLOEXEC, 0600)` | `dfd` |
| Write | `:159` / `WriteAll` `:193-207` — EINTR retried, partial writes continued, zero progress is a failure | the temp `fd` |
| Close | `:160` — a non-zero `close` is treated as failure | temp `fd` |
| Rename | `:163` `RenameAt(dfd, tmp, dfd, name)` | `dfd` both sides |
| Cleanup | `:169` `if (!renamed) UnlinkAt(dfd, tmp, 0)` | `dfd`; only this call's GUID temp name |
| Folder close | `:172` `finally { Close(dfd); }` | — |

`grep` over `src/Core` and `src/Host` finds **no** `FileStream`, `File.Move`, `File.Delete`, `File.Open`/`WriteAll*` in the transaction. The only production `AT_FDCWD` use is the single folder open at `:180` (the others are in the self-test). Path-based calls that remain are courtesy pre-checks and listing (`DirectoryExists`, `LinkTarget`, `ListFileNames`, `Directory.CreateDirectory` of a missing root). They never write a save. File names are constrained by `^[A-Za-z0-9_-]{1,64}\.sitkalab\.json$` before any file-system access, and `WriteAtomic` re-checks for a bare name.

**Syscall trace (REPRODUCED, `strace`):** one save produced, inside the save window, `openat(AT_FDCWD, …/.lab-data, O_RDONLY|O_NOFOLLOW|O_CLOEXEC|O_DIRECTORY) = 42` (plus the earlier `Classify` pre-check's own open), then `openat(42, "w.sitkalab.json.tmp-<guid>", O_WRONLY|O_CREAT|O_EXCL|O_NOFOLLOW|O_CLOEXEC, 0600) = 44`, `write(44, …, 3) = 3`, `renameat(42, tmp, 42, "w.sitkalab.json") = 0`. Nothing in the transaction names the folder by path again.

**Descriptor hygiene (REPRODUCED):** 2,000 iterations on each of four paths (success, rename-fails folder squat, read-only folder, missing folder) left the process at 39 open fds before and after, `tmp-left=0`. Source: the directory FD is closed in a `finally`, the temp FD exactly once, no double close.

### 2.2 Deterministic stage-hook attacks (my own, REPRODUCED)

I drove the writer's internal hook seam by reflection with **my own** attack shapes (`probes/src/Program.cs`, mode `hooks`). Each row used a fresh scratch rig with a decoy "alternate" folder holding a victim `w.sitkalab.json`, a temp-like decoy and an unrelated file.

| Attack | Result | Alternate/other folder | Temp orphans |
|---|---|---|---|
| H1 `.lab-data` ⇄ symlink **after the folder FD opened** | `saved`, bytes in the original (moved) folder | identical | 0 / 0 |
| H2 same swap **between temp write and rename**, over an existing save | `saved`, old save replaced in the original folder | identical | 0 / 0 |
| H3 / H3b a **different real folder** exchanged in (after open / before rename) | `saved` in the **original** folder; the substitute is untouched | identical; substitute unchanged | 0 / 0 |
| H4 / H4b the opened folder **deleted** after open / before rename | `WriteFailed` (controlled, no raw exception) | identical | 0 / 0 |
| H5 rename fails (folder squats final name) **and** real-folder swap | `WriteFailed`; the cleanup `unlinkat` hit only the original folder; substitute with its own decoy untouched | identical | 0 / 0 |
| H6 rename fails and symlink swap | `WriteFailed` | identical | 0 / 0 |
| H7 final name → **symlink to a victim** before rename | `saved`; the entry was **replaced** (final is a regular file, not a link); victim unchanged | identical | 0 / 0 |
| H8 final name → **FIFO** before rename | `saved` in 1 ms (no block); FIFO replaced | identical | 0 / 0 |
| H9 final name → **hard link to a victim** before rename | `saved`; victim inode kept its bytes | identical | 0 / 0 |

Late squats at the final name between the early check and `WriteAtomic` (mode `late-squat`): symlink to a victim, FIFO, hard link to a victim and dangling symlink are all replaced and never written through, the victim is unchanged, and a squatting folder gives `WriteFailed` with its contents left alone. Temp count was 0 in every case.

(Two of my first H5/H6 results were `RAW:IOException`. That was a bug in **my** hook, which created a folder over a name that still held a file. I fixed the probe and re-ran; the table above is the corrected run. The candidate was not at fault.)

### 2.3 Concurrent exchange attacks (REPRODUCED, fresh scratch root each time)

| Probe | Repair `c445f870` | Pre-repair MULTI-004 `9ba88b6e` |
|---|---|---|
| MULTI-005 live-host `smoke.py` (read-only copy, sha256 `237a9e6f…`), run 1 | **23/23**; 248,987 swaps; 4,381×`403:SymlinkRejected`, 143×`200:ok`; **0** outside overwrites, **0** orphans | — |
| same, run 2 | **23/23**; 255,164 swaps; 4,258×403, 112×200; **0 / 0** | — |
| same, MULTI-004 baseline | — | **21/23**; **19** outside overwrites (**8 reported as 200 "saved"**), **29** outside temps, **94** inside temps |
| My store-level `writeswap` (4 s), runs 1 / 2 | 431,966 / 434,166 swaps; saved 220 / 211, `SymlinkRejected` 23,582 / 23,492; alternate folder changes **0**; claimed-saved-but-real-folder-differs **0**; claimed-failure-but-real-folder-changed **0**; temp **0** | writer/MULTI-005 baseline counterexamples (not rerun at store level by me) |
| Writer Host test inside `Host.Tests` (my run) | `swaps=326,434 responses=403:SymlinkRejected=2816,200:ok=17 alternate-overwrites=0` | — |

The baseline run proves these probes can fail; the repair's green is therefore not vacuous. The counts differ from the writer's (timing dependent) but the qualitative result is identical: repaired = 0 escapes, 0 orphans, 0 false successes; baseline = both failure classes present.

### 2.4 What can actually be written under same-user directory exchanges

- **During a save (after the folder is opened):** only the opened folder inode, wherever it has been moved to. Writable names: this call's own `<name>.sitkalab.json.tmp-<guid>` and the safe final name `<[A-Za-z0-9_-]{1,64}>.sitkalab.json`. **Nothing** is created, replaced or deleted in the exchange target. 0 alternate-folder changes across all runs.
- **Late squats at the final name:** the directory entry is replaced; it is never written through; a folder there fails safely.
- **Deleted folder:** `WriteFailed`.
- **Rename/cleanup scope:** `unlinkat` only ever removes this call's own GUID-named temp, relative to the same handle.
- A save into a folder the attacker has just moved aside is reported honestly at the store level (R-1 above) and the tests acknowledge this ("the real folder may have been MOVED … that is the inode-anchored guarantee, not a promise about path geography").

### 2.5 Regression coverage validity

The new tests (`FolderSwappedForALink_AfterTheFolderWasOpened_…`, `…_BetweenTempWriteAndRename_…`, `…_AndTheRenameFails_…`, `ConcurrentFolderExchange_DuringSaves_…`, and the Host `SavesDuringAConcurrentLabFolderExchange_…`) exercise the dangerous interval deterministically through the `DirectoryOpened`/`TempWritten` hooks plus a real racing swapper. Individually path-basing each syscall fails them (section 6, M31/M32/M33, plus my X1/X2). None of them can pass vacuously: the deterministic tests assert that the hook ran exactly once and that the lab path really pointed at the alternate folder for the rest of the save, and the racing tests assert that the swapper actually raced (`swaps > 50`) and that both `saved` and refused outcomes occurred.

## 3. R-2 — growth after `statx`: **PASS**

- **Source:** `NoFollowFiles.ReadRegular` rejects `statx` size > limit (`:125-126`); `ReadAll` then enforces a **cumulative** bound after every read (`:241`, `if (ms.Length > maxBytes) throw FileTooLarge`).
- **Test** `ReadRegular_RefusesContentThatOutgrowsTheStatxSize_ProcfsStatus` uses the numeric `/proc/<pid>/status` (not the `/proc/self` symlink). It asserts procfs mounted, `FileInfo.Length == 0`, `Classify == Regular`, positive control (generous limit returns more than 10 bytes containing `Pid:`) and that limit 10 throws `FileTooLarge`. It cannot pass vacuously: the positive control fails if procfs is missing or empty. procfs was present on this host, so no qualification limitation arises.
- **Mutant M35** (removes the second guard) **compiles** and is **CAUGHT by exactly one test: the new procfs test**. The original first-size test still passes under M35, which shows the new test is the effective coverage.
- Status: REPRODUCED.

## 4. R-3 (new, MINOR, non-blocking) — HTTP save can report 403 after the write landed

- **What happens:** `LabHost.cs:215-216` calls `WriteSave(...)` and then builds the 200 response with `files = st.Store.ListSaves()`. `ListSaves` re-resolves the folder by **path** (`LabDataStore.cs:95-106`, `EnsureRoot`/`CheckRoot` `:74-84`, and `Classify` → `OpenDirectory`). If the attacker has swapped the folder in that window the call throws `SymlinkRejected`, which the handler maps to **403**, although the save already landed in the trusted (now moved) folder.
- **Measured (REPRODUCED twice, 5 s each, inode check on the real folder via a held directory FD, `smoke/host-truth.py`):**

  | | `200:ok` landed | `200:ok` **not** landed | `403` but **landed** | `403` not landed | `500` not landed | alternate-folder violations | temp orphans |
  |---|---|---|---|---|---|---|---|
  | Repair run 1 (272,743 swaps) | 57 | **0** | **55** | 3,483 | 0 | **0** | **0** |
  | Repair run 2 (266,879 swaps) | 58 | **0** | **56** | 3,511 | 0 | **0** | **0** |
  | MULTI-004 baseline | 35 | **2 (false success)** | 32 | 3,396 | 203 | present | 55 |

- **Classification:** MINOR. The error direction is the safe one (a false *failure*; the repair has **zero false successes**, every `200` landed). Nothing is written outside the opened folder and nothing is orphaned. It needs a same-user process thrashing the folder at ~50,000 swaps/s. It is **pre-existing** (identical mechanism on MULTI-004) and not introduced by MULTI-006. The writer's host test is named `…AndReportHonestly` but asserts only the outcome whitelist and the no-escape/no-orphan checks, not the correlation between response and real-folder write; the Core test checks that correlation only for `WriteSave` itself. The writer's handoff §7 does not mention it.
- **Smallest repair (not authorised for this review, so not applied):** make the post-write file listing non-fatal (compute `files` inside a try/catch and still return `200` with `fileName`, or return an empty list), then extend the Host test to assert that every `200` landed and no `403` after-write claim is made. One focused review would suffice.

## 5. Real directory substituted **before** open — classification

**Reproduced facts (my probes, `realdir-det`, `realdir-race`, `parent-det`):**

- Deterministic: with a different real folder at `.lab-data` before the save starts, `WriteSave` opens that folder and saves into it; the original folder (moved aside) is untouched. `ListSaves` then lists the substitute's save.
- Racing: 3 s, 295,796 exchanges, 13,669 saves, all `saved`; **7,091** landed in the substitute real folder, 0 temp orphans in either, unrelated files intact. Effect: only `<safe>.sitkalab.json` entries and the call's own temp name, entries replaced (never written through links), mode 0600.
- Ancestor symlink: with the *parent* of `.lab-data` replaced by a symlink to another tool root, the save wrote into that other root's `.lab-data` and replaced its `w.sitkalab.json`. `O_NOFOLLOW` covers only the **last** path component. The README/handoff sentence "links are never followed" is therefore precise only for the final component.

**This is a different thing from the repaired race.** The repaired defect was a path *re-resolved at each step*, so a single save could be split across two folders. After the repair one save is inode-anchored: one folder, start to finish. Substituting a real folder before open means the attacker changed what the fixed path *is*. The save is then correct for the folder it opened.

**Classification: honest same-user / local-adversary limitation, acceptable with the existing README caveat; it does not merit NEEDS CHANGES.** Reasons:

1. The approved spec (`spec.md`, persistence and AC-07) promises a fixed root path, rejection of absolute paths, traversal and **symlink escapes**, and no free-path API. A real directory at the root path is not a symlink escape and is the canonical root path. It does not promise inode identity.
2. A same-user process can already write any file the user can; the lab is a loopback research tool with no privilege boundary. Substitution does not widen reachable data beyond `*.sitkalab.json` entries and a self-named temp.
3. The writer documented it in the README ("same-user process that renames a different **real** folder into `.lab-data` before a save starts has replaced the lab folder itself") and in handoff §7.

**Persistent identity is not provided and must not be claimed.** The guarantee is *transaction-level inode anchoring* (one save, one folder). Pinning the folder's (device, inode) across saves or sessions would be a separate design change and a **separate Manager decision**. I did not require it. My view: not needed for P0; if the Manager wants it later it should also decide whether ancestor-path symlinks are in scope, and it would still not stop a same-user attacker from writing elsewhere directly. I recommend a one-line README wording fix ("final-component links are never followed; ancestor links are resolved normally").

## 6. Mutation coverage

**Focused six (REPRODUCED, `tests/run-mutation-controls.py`, scratch copy, `MUT_ONLY=M28,M31..M35`):** baseline green, **6/6 CAUGHT**, none build-broken, none survived.

| ID | Safeguard broken | Caught by |
|---|---|---|
| M28 | failure cleanup disabled (`if (false) UnlinkAt`) | `FolderSwappedForALink_AndTheRenameFails_…`, `WriteAtomic_FailureAfterTheTempFileExists_…` |
| M31 | temp created by folder path | `ConcurrentFolderExchange_…`, `FolderSwappedForALink_AfterTheFolderWasOpened_…` |
| M32 | rename by folder path | the concurrent test, `…_AfterTheFolderWasOpened_…`, `…_BetweenTempWriteAndRename_…` |
| M33 | cleanup by folder path | `…_AndTheRenameFails_…` |
| M34 | whole save reverted to MULTI-004 `FileStream`/`File.Move`/`File.Delete` | all four swap tests |
| M35 | read-loop size re-check removed | `ReadRegular_RefusesContentThatOutgrowsTheStatxSize_ProcfsStatus` **only** |

**Full run once (REPRODUCED):** **35/35 CAUGHT**, original M01–M17 **17/17**.

**My extra mutants (reviewer-owned, `mutation/reviewer-mutants.py`, scratch copy):**

| ID | Mutation | Result |
|---|---|---|
| X1 | rename new side by path only | **CAUGHT** (5 tests) |
| X2 | rename via a second freshly opened folder handle | **CAUGHT** (3 tests) |
| X6 | save opens the folder without `O_DIRECTORY\|O_NOFOLLOW` | **CAUGHT** (2 tests) |
| X3 | new-file mode 0644 instead of 0600 | **SURVIVED** |
| X4 | temp create without `O_EXCL` | **SURVIVED** |
| X5 | deferred `close` error ignored | **SURVIVED** |

## 7. Observations (non-blocking)

- **O-1** No regression test asserts the 0600 requirement (X3 survived). I verified the actual mode by probe: **0600** under umask 0002, 0022, 0000 and 0077, and 0400 under umask 0277 (the umask is honoured; it can only remove bits). The strace shows the explicit `0600` argument.
- **O-2** X4 and X5 survivors are defence-in-depth (an unpredictable GUID temp name makes a collision negligible; a deferred close error cannot be injected portably). I do not count them as defects.
- **O-3** `fsync` was deliberately not added (writer handoff §3, "Not added: `fsync`"). The save is atomic with respect to visibility, not durable across power loss. That is out of scope here and should not be read as a durability claim.
- **O-4** README states "Only Linux x64 has been run and independently tested". After this review that statement is true for the repaired code on Linux x64 only.

## 8. Other bounds and evidence labels

- **Platform:** Linux x64 file ABI actually run. **Linux arm64 UNTESTED** (values from headers; guarded by the self-test). Native Windows and macOS lab file operations return **HTTP 501 by source** (`StoreStatus`: `PlatformUnsupported` → 501; `RequireSupported()` throws before any I/O); **not run**. Nothing here approves cross-platform or Windows game-build behaviour.
- **Tier 1 (REPRODUCED, review worktree):** `dotnet build MultiRackSitkaLab.sln --no-incremental` exit 0, **0 warnings, 0 errors**, 10.2 s; Core **82/82** exit 0 (7.9 s); Host **13/13** exit 0 (5.9 s); fixture manifest **3/3 OK**; schema checker exit 0 with all negative controls rejected. Worktree stayed clean throughout.
- **Browser (REPRODUCED, scratch copy):** Chromium headless **15/15**; 2,000-tree interaction max wall **158 ms** against the 2,000 ms budget (writer: 186 ms). Chromium needs a short `TMPDIR` (its singleton-socket path limit; my earlier attempts failed on that; the cause was confirmed from Chromium's own `Socket path too long` message, not guessed).
- **Writer-side evidence (accessed read-only):** `MULTI-006-SitkaLab-WriteRepair-20261114/MANIFEST.sha256` **37/37 OK**. The writer's numeric results are **WRITER ONLY** except where reproduced above (build, 82/13/3, schema, 35/35, 15/15, smoke 23/23 vs 21/23).
- **Not tested / not claimed:** other browsers, cross-platform, P1 biology, Unity, production saves, measured field data, user acceptance, long-duration soak, filesystems other than the local ext4 Files volume, behaviour under `ENOSPC` or quota (a `write`/`close` failure path is source-inspected only).

Status labels: R-1 **REPRODUCED**; R-2 **REPRODUCED**; R-3 **REPRODUCED**; real-folder and ancestor-link substitution **REPRODUCED**; arm64 **NOT TESTED**; Windows/macOS **SOURCE INSPECTED** (501 by source); writer numbers **WRITER ONLY** where not listed as reproduced.

## 9. Recommendation to the Manager

1. **Accept** the MULTI-006 write-safety repair as a narrow P0 hardening (this verdict).
2. **Optional, small:** one focused repair for R-3 plus one Host test that correlates response with real-folder write; one focused review. Not required for acceptance in my judgement.
3. **Record a separate decision** on persistent folder identity (and ancestor-link scope) and on the **Linux-only** platform boundary. Neither is authorised or needed to close this repair.
4. README wording fix for "links are never followed" (final component only).

## 10. Evidence, cleanup and publication

- **Off-repo evidence:** `/media/jer/Files/CCF/Verification/MULTI-007-SitkaLab-WriteDeltaReview-20261114/` with `MANIFEST.sha256` (56 entries at creation): `tier1/`, `mutation/` (focused, full, reviewer mutants and runner), `smoke/` (live smoke runs, baseline run, `host-truth.py` and logs), `probes/` (C# probe source, outputs and the `strace` window), `browser/` (log, JSON receipt, screenshots), `environment.txt`. Reviewer-owned scratch copies and probe build output were deleted after the runs.
- **Processes:** I stopped only processes I started (an orphaned scratch host from a failed browser attempt and MSBuild/compiler nodes), by PID, after confirming their start times were after the worktree was created. No dotnet, Chromium or host process of mine remains. The 198 pre-existing `/tmp/sitkalab-test-*` folders (the writer's count) were not touched and none are newer than this task. One short browser temp folder was removed.
- **Publication:** this report is the only tracked change on the review branch; the one-file diff against the candidate and the GitHub readback are recorded on #511. Dashboard/Drive sync **NOT VERIFIED**.
