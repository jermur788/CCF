# Multi — Sitka P0 accurate post-save HTTP response repair (#567)

**Date:** 16 November 2026. **Programme:** standalone 2D Multi-Rack Sitka laboratory, P0 no-growth lab only. This is **not** Scenario One, P1 growth, Unity or a Windows game build.

**Status:** **HANDED-OFF for independent non-author security review.** This is author-side evidence only. The author may not sign off this change. Manager source acceptance and the P0 re-pin wait for a separately registered non-author reviewer.

## 1. Identity and lane

| Item | Value |
|---|---|
| Author | Anthropic Claude Opus 5.5 (`claude-opus-5-5`) via Claude Code in T3 Code, owner's Claude subscription. Reasoning effort is harness-set and not directly observable to the model (Manager preference: Medium). |
| Host | `jer-laptop`, Linux 7.0.0-42-generic x86_64. This is native Linux, not WSL2. |
| Runtime | .NET SDK **8.0.133**, selected by the lab's `global.json`. System SDK 10.0.114 was also present but not used. Chromium at `/usr/bin/chromium` and Node were used for the Tier-2 browser suite. |
| Branch | `fix/multi-sitka-p0-save-response-r3-20261116-v1` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-567-P0-SaveResponse-20261116` (new, clean at creation, ext4) |
| Starting HEAD | `861b81d3540429d241bba0732a610212626f6828`, the pinned accepted P0 README-only baseline |
| Source commit | `046838b06e500258de45db1ac98bf508421dd96d` |
| Claim | [#567 RUNNING comment](https://github.com/jermur788/CCF/issues/567#issuecomment-6913115730), posted before any source inspection or edit |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-567-P0-SaveResponse-20261116/` |

## 2. Defect (historical #517 R-3)

`POST /api/saves/save` ran `WriteSave(...)` and then the optional, path-based `ListSaves()` inside one `try`/`catch (LabException)`.

- `WriteSave` returns only after `renameat` has committed the file inside the handle-anchored lab folder (`NoFollowFiles.WriteAtomic`; nothing after the rename can throw).
- `ListSaves()` then re-checks the lab root **by path**. If a same-user process had exchanged `.lab-data` with a symlink in the meantime, `CheckRoot` threw `SymlinkRejected` and the client got **HTTP 403 `ok:false`**, even though the save had already committed. A non-`LabException` IO failure in the refresh (for example, the folder replaced by a plain file) fell through to the Guard as a generic **500 `InternalError`**.

This was reproduced RED on the baseline (§5, step 1).

## 3. Change (exact diff summary)

Two tracked files changed. Both are on the Manager allowlist.

**`Tools/MultiRackSitkaLab/src/Host/LabHost.cs`** (+26/−10):

1. **Save handler split into two phases, both inside `st.Gate`:**
   - **Phase 1 (pre-commit):** `WriteSave` in its own `try`. Any `LabException` returns the same `Bad(code, message, StoreStatus(code))` as before, with unchanged status and code mapping. Non-`LabException` failures still propagate to the Guard's generic 500, as before.
   - **Phase 2 (post-commit):** `ListSaves()` in its own `try`. A `LabException` sets `listCode = ex.Code`. Any other exception sets `listCode = "ListFailed"`. The reply is always **200** `{ ok:true, code:"ok", message, fileName, files, listRefreshed, listCode }`. On refresh failure: `files: null`, `listRefreshed: false`, and the message is "saved X; the lab file list could not be refreshed (CODE)". On success: `files: [...]`, `listRefreshed: true`, `listCode: null`. No path or exception text is returned; only a code.
2. **Test seam:** `LabHostOptions.FileAccess` (`ILabFileAccess?`, public `init`) and an overload `LabState(string, ILabFileAccess?)` that passes it to the existing public `LabDataStore(toolRoot, io)` constructor. `Program.cs` never sets it (verified: `new LabHostOptions { ToolRoot = FindToolRoot(), Port = port }`), and no HTTP input can reach it. I first tried an `internal` seam with `[InternalsVisibleTo]`, but that exposes the Host's top-level `Program` class to the test assembly and fails the build with CS0436 under `TreatWarningsAsErrors`. **Reviewer attention:** confirm that a public in-process option is acceptable, or ask for a Manager file allowance for an alternative.

**`Tools/MultiRackSitkaLab/tests/Host.Tests/HostTests.cs`** (+133/−4):

- `TestHost(ILabFileAccess? fileAccess = null)` and `HookedFileAccess`: real `RealLabFileAccess` with one-shot `BeforeWrite` and `AfterWrite` hooks around `WriteAtomic`. Hooks run on the real file system; nothing is simulated.
- New test `SaveThatCommits_ThenMeetsASwappedLabFolder_Reports200WithTheFile_AndSaysTheListWasNotRefreshed`:
  - After the commit, a real `renameat2(RENAME_EXCHANGE)` swaps `.lab-data` with a symlink to an alternate folder.
  - Expects 200, `ok:true`, `code:"ok"`, the correct `fileName`, `listRefreshed:false`, `listCode:"SymlinkRejected"` and `files:null`, with no path or exception text.
  - While the folder is still swapped, `GET /api/saves` must **still return 403 SymlinkRejected**, so the security check is not relaxed.
  - After swapping back: the alternate folder is byte-identical; the lab folder holds exactly the committed file and no temp file; the file loads over HTTP.
- New test `SaveThatCommits_ThenCannotListForAnUnexpectedIoReason_StillReports200_WithAGenericListCode`:
  - After the commit, the folder is moved away and a plain file takes its name, so the refresh fails with an `IOException`.
  - Expects 200, `listCode:"ListFailed"` and no leaked path, exception text or file content. The committed file is present and there is no temp file.
- New test `SaveThatMeetsASwappedLabFolderBeforeCommit_StillFails403_AndWritesNothingAnywhere`:
  - The exchange happens after the name and classification checks but before `WriteAtomic` opens the folder.
  - Expects 403 `SymlinkRejected`, `ok:false`, no `fileName` and no `listRefreshed`. Nothing is written to either folder.
  - Control: an undisturbed save then returns 200 with `listRefreshed:true`, `listCode:null` and the refreshed list.
- Existing race test `SavesDuringAConcurrentLabFolderExchange_…`: kept unchanged, except that every 200 must now name `w.sitkalab.json`, and the INFO line counts 200s whose list refresh failed.

**No change to:** Core (`LabDataStore`, `NoFollowFiles`, the write path), UI (`app.js` already does `if (r.files) fillSaves(r.files)`, so a `null` list keeps the previous list), schemas, fixtures, README, build config, Host/Origin/CSRF/method guards, the loopback port, canonical save identity or any user save. `git diff 861b81d3 HEAD -- schemas fixtures src/Core src/Ui` is empty.

Post-commit SHA-256 values: `LabHost.cs` `6922404feead2889fe4c75fca16f1f5590a7a039686b3ea35cd43a56012711fa`; `HostTests.cs` `fcd8cac16386e1c618b99c8367d22ed8a24659c62ee5a0a331833924df22d2e1`.

## 4. Acceptance mapping

| Manager criterion | Evidence |
|---|---|
| (a) A committed save returns a truthful HTTP result even if the later optional listing fails | The two post-commit tests: 403 → 200 (symlink swap) and 500 → 200 (IO failure). Race test: 64 of 92 successful saves had a refused refresh, and each would have been a false 403 on the baseline. |
| (b) A pre-commit save failure still fails | Pre-commit swap test (403); the existing 409 folder/FIFO, 500 read-only folder and 400 bad-name tests are unchanged and pass; mutant X3 (§6). |
| (c) A same-user folder swap cannot falsify success or weaken security | `GET /api/saves` still refuses the swapped folder; no list is offered from a failed check (mutant X5); race test: 0 alternate-folder overwrites and every 200 names the file. |
| (d) No escape, temp orphan, unintended disk change or schema drift | Exact folder listings in all three new tests; race test temp and alternate checks; schema check passes; Core, schemas and fixtures unchanged; the browser run left the worktree `.lab-data` empty. |
| (e) Proportionate regression and negative controls | Core 82/82, Host 16/16, UI 15/15, existing mutation controls 35/35, fix-specific mutants 6/6. |

## 5. Tests actually run (native Linux, SDK 8.0.133)

| # | Command (from `Tools/MultiRackSitkaLab`) | Result | Evidence file |
|---|---|---|---|
| 1 | Seam and new tests only, fix **not** applied: `dotnet run --project tests/Host.Tests` | **RED as intended:** 13 pass, 3 fail. Post-commit swap 403 `SymlinkRejected`; post-commit IO 500 `InternalError`; pre-commit test reached only the new `listRefreshed` field. | `01-red-seam-only-host-tests.txt` |
| 2 | With the fix: `dotnet run --project tests/Host.Tests` | **16 passed, 0 failed.** Race INFO: swaps=329228, 403:SymlinkRejected=2740, 200:ok=92, alternate-overwrites=0, 200-with-list-not-refreshed=64. | `02-green-host-tests.txt` |
| 3 | `dotnet run --project tests/Core.Tests` | **82 passed, 0 failed** | `03-core-tests.txt` |
| 4 | `python3 -I -B tests/check-schema.py` | exit 0 | `04-schema.txt` |
| 5 | `python3 -I -B tests/run-mutation-controls.py <scratch>` (copies the tree; the real tree is never modified) | **35 of 35 CAUGHT** (M01–M17 17/17) | `05-mutation-controls.{txt,json}` |
| 6 | `python3 -I -B scripts/fix567_mutants.py <lab> <scratch>` | control GREEN; **X1–X6 6/6 CAUGHT** (§6) | `06-fix567-mutants.txt`, `scripts/fix567_mutants.py` |
| 7 | `CHROMIUM=/usr/bin/chromium node tests/Ui/run-browser-tests.mjs` (headless; the worktree's own empty `.lab-data`) | **15 passed, 0 failed**; `.lab-data` empty afterwards | `07-ui-browser-tests.txt`, `07-ui/` |

Race counts are timing-dependent and vary between runs. The deterministic proof is the three hooked tests.

## 6. Fix-specific negative controls (each must turn the Host suite RED)

| ID | Mutant | Caught by |
|---|---|---|
| X1 | Listing `LabException` returned as a failed save again (old behaviour) | post-commit swap test |
| X2 | Generic post-commit `catch (Exception)` removed | post-commit IO test |
| X3 | Pre-commit `LabException` swallowed and reported as success | pre-commit swap test plus 3 existing save-refusal tests |
| X4 | Exception message leaked as `listCode` | post-commit IO test (path leak) |
| X5 | A stale or unfiltered list offered when refresh fails | post-commit swap test |
| X6 | `listRefreshed` always true | both post-commit tests |

## 7. Design notes and limits for the reviewer

- **Catch breadth:** after a committed rename, the refresh is read-only and optional, so every exception there is reported as a list-refresh failure, never as a failed save. Unexpected exceptions get the opaque code `ListFailed`, with no detail. Before the commit, nothing new is caught: non-`LabException` failures still reach the Guard's 500.
- **"Committed" means rename-committed, not `fsync`-durable.** `WriteAtomic` does not `fsync` the file or the folder. The `fsync` policy is an open Manager item and was not changed here. A 200 means the rename succeeded in the handle-anchored folder.
- **The save file is identified by name, not by folder.** After a post-commit swap, the committed file sits in the real lab folder, while the path `.lab-data` may temporarily point elsewhere. The response does not claim the file is visible at that path, and the listing correctly refuses until the swap is undone.
- **API shape change:** the save success reply gains `listRefreshed` (bool) and `listCode` (string or null), and `files` may be `null`. The browser UI handles this already, and the save schema is unaffected.
- **Not run:** Windows Edge / Ubuntu WSL2 runtime (no claim that this repair is WSL-qualified; the earlier #514 acceptance covers the baseline only), Linux arm64, Chrome, `fsync`/power-loss behaviour, Unity, P1. No user save, distro shutdown or SDK install was involved.
- **Hygiene:** `/tmp/sitkalab-*` showed no new leftovers from these runs (before/after lists are in the evidence folder). I stopped only the MSBuild/VBCSCompiler server PIDs started during my build window, by explicit PID. The worktree is clean except for ignored `bin/`/`obj/`.

## 8. Next step

A **separately registered non-author** source/security reviewer should check commit `046838b0` (and this handoff commit) on this branch against §4, re-run steps 1–7, and pay particular attention to the public `FileAccess` seam (§3) and the post-commit catch breadth (§7). Manager disposition and the P0 re-pin follow that review. The reviewer can reuse `scripts/fix567_mutants.py` from the evidence folder.
