# MULTI-004 — Sitka Lab P0 file-safety and regression hardening: IMPLEMENTATION HANDOFF

**Date:** 14 November 2026 · **Claim:** [#502](https://github.com/jermur788/CCF/issues/502) · **Packet:** [MULTI-004 packet](https://github.com/jermur788/CCF/blob/docs/multi-004-sitka-lab-p0-hardening-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-004-SITKA-LAB-P0-LOCAL-FILE-SAFETY-AND-REGRESSION-HARDENING-PACKET-20261114.md) · **Source review:** [MULTI-003 verdict](https://github.com/jermur788/CCF/blob/review/multi-003-sitka-2d-lab-p0-independent-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-REVIEW-VERDICT-20261114.md)

## Status: HANDED OFF — fixes F-1…F-5 implemented; NOT independently reviewed, NOT integrated, NOT user-accepted

Everything below is **worker-side evidence**. A focused independent delta review is still required. This is engineering P0 hardening only: no P1 biology, no real data, no Unity or Scenario One, no Windows game build, no ecological claim.

## 1. Worker, lane and what physically ran

| Item | Value |
|---|---|
| Worker | **Anthropic Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code in local T3 Code. Not the packet's recommended Sol/Codex High; the packet allows Sonnet 5.5 with a different independent reviewer. **Same model family as the MULTI-002 author** (separate session, no shared context), so please use Claude Opus 5.5 or Sol/Codex for the delta review, not Sonnet. |
| Registration | [#502 RUNNING comment](https://github.com/jermur788/CCF/issues/502#issuecomment-6879542736), posted before any production edit or local test |
| Host | `jer-laptop`, native Linux 7.0.0-42, Files drive (ext4) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-004-SitkaLab-Hardening-20261114`, created fresh (it was absent). The MULTI-002 and MULTI-003 worktrees were not used, edited or cleaned. The coordination anchor `/media/jer/Files/CCF-Primary` has unrelated dirty files (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`); I left them alone. |
| Branch / starting HEAD | `feature/multi-004-sitka-lab-p0-file-safety-hardening-20261114-v1` @ `f33641f85ec8146ae26b91e4d5ea0b89bfba40f0` (0 ahead / 0 behind at start) |
| **Tested source commit** | **`e9b3b73095139894a3502ce89111937983572dd1`** (all Tier 1/2 results below were produced from this tree; the commit after it adds only this Markdown file) |
| Coordination | Live `active-tasks.md` on `main` (`fc18190b`) had no MULTI-00x, #494/#497/#502 or SitkaLab row, so no board lock was claimed or needed. No `dotnet`, Unity or Chromium process was running at start; none was left running. No Unity was launched. |
| Toolchain | .NET SDK 8.0.133 (pinned by `global.json`), Node v26.13.1, Chromium 156.0.8078.79, Python 3.12.3 |
| Off-repo evidence | `/media/jer/Files/CCF/Verification/MULTI-004-SitkaLab-Hardening-20261114/`, `sha256sums.txt` covers 37 files; SHA-256 of that manifest `7f6a47840b0ba676555779d8eb4702ee48c2472aa550d8d76ca7159c385aa9b7` |

## 2. Exact file scope

`git diff --name-status f33641f8 e9b3b730`: **9 files, all under `Tools/MultiRackSitkaLab/`**, plus this handoff.

| File | Change |
|---|---|
| `src/Core/NoFollowFiles.cs` | **new** — handle-anchored no-follow open (`openat`/`statx` via libc), Linux x64/arm64 only, self-tested, fail-closed |
| `src/Core/LabDataStore.cs` | `ILabFileAccess` reshaped (`Classify`, `ReadRegularFile`); listing/read/write use it; `WriteAtomic` cleans up and throws `WriteFailed` |
| `src/Host/LabHost.cs` | `Deny()` code follows the status; store refusals map to accurate HTTP statuses |
| `tests/Core.Tests/LabDataStoreTests.cs` | rewritten (22 tests; the misleading test replaced) |
| `tests/Core.Tests/LayeredGuardTests.cs` | **new** — forged hash-consistent histories and validator layer (6 tests) |
| `tests/Core.Tests/StateMachineTests.cs` | +1 test (execution-time double-reservation) |
| `tests/Host.Tests/HostTests.cs` | +4 tests (FIFO + concurrent state, controlled save errors, code/status alignment) |
| `tests/run-mutation-controls.py` | M01–M17 unchanged; **M18–M30 added**; summary now also reports the original 17 separately |
| `README.md` | documents the open-time guarantee and the platform restriction |

Not touched: the approved spec, `specs/`, fixtures, schemas, UI files, board/coordination files, root `.gitignore`, `Assets/`, `Packages/`, `ProjectSettings/`, saves, Scenario One. No `.meta` files. Fixtures and schema are unchanged (no fixture or schema path appears in the diff; manifest and regeneration checks below).

## 3. F-1 … F-5: before / after

"Before" is the pristine candidate `f33641f8`, re-attacked on this host with the **reviewer's own MULTI-003 probe** (`reviewer-scripts/probe-Program.cs`, unmodified) and with the live-process smoke in §5. "After" is `e9b3b730`.

| ID | Before (reproduced here) | Fix | After |
|---|---|---|---|
| **F-1** FIFO hang | Probe D1: FIFO in `.lab-data` listed; `ReadSave` blocked. Probe E10: load never returned, concurrent `GET /api/state` timed out behind the session lock. | `Classify` (statx, no open) so only single-link regular files are listed; the read opens with `O_NONBLOCK` and checks the **open handle's** type. | D1 not listed, `NotListed` in <3 s. E10 load → 403 `NotListed`, concurrent state → 200. Live host: 4 concurrent FIFO loads + 12 state reads all answered within 8 s; FIFO untouched; normal save/load still works beside it. |
| **F-2** symlink open race | Probe D2 (8 s race, 55,616 swaps): **6,158 outside-file reads**. Live HTTP race against a *valid* lab file outside the root: **61 loads of the outside file**; a hard link to it **loaded (200)**. | The lab folder is opened `O_NOFOLLOW\|O_DIRECTORY`; the file is opened **relative to that handle** with `O_NOFOLLOW`; type, link count and size come from `statx` on the **open descriptor**; the read is size-capped. Symlink, FIFO, socket, device, folder and `nlink>1` are refused. | D2 (54,835 swaps): **0 outside reads**. Live HTTP race (10,671 swaps): **0 outside loads**, 256 legit loads, 3,663 refusals, 0 other. Hard link → `NotListed`. |
| **F-3** misleading test | `SymlinkAppearingAfterListing_IsStillRefusedBeforeOpen` fired on the **root** check; mutant X08 survived. | Renamed honestly (`LinkReportedOnTheRoot_RefusesTheWholeStore_BeforeAnyOpen`). New tests run a hook **after ListSaves and the per-entry pre-check, immediately before the open**, and swap in a symlink, symlink-to-FIFO, FIFO, folder, hard link, or a symlinked lab folder. Each asserts the earlier checks both saw an ordinary file (so only the open can refuse), the exact refusal code, and that no outside bytes came back. Plus a 2.5 s adversarial stress test with atomic `rename(2)` swapping. | M23 (O_NOFOLLOW removed) is caught by the swap tests **and** the stress test (`mutation-full.json`). |
| **F-4** layered guards | X01, X06, X08, X16, X22 survived (reviewer). | See §4. Nothing was removed or weakened. | M18/M19/M20/M21/M22 caught, each by a dedicated test. |
| **F-5** atomic save | Probe E9: save onto a folder → **500 `Forbidden`**, leaving `*.tmp-<guid>` behind. `Deny()` always said `code:"Forbidden"` (also for 405/413/415/500). | `WriteSave` refuses a folder or special file up front (409 `NotRegularFile`). `WriteAtomic` tracks that its own `CreateNew` succeeded and on any IO failure deletes **only that exact temp path**, then throws `WriteFailed` (500, no path in the message). `Deny()` code = `BadRequest/Forbidden/MethodNotAllowed/PayloadTooLarge/UnsupportedMediaType/InternalError`. | E9 → 409 `NotRegularFile`, 0 temp files. Unwritable folder → 500 `WriteFailed`. 405/413/415/403 carry matching codes. A foreign `*.tmp-decoy` file and the neighbouring save are untouched by a failed write. |
| **F-6** spec wording | — | **Not changed** (approved spec untouched). | See §7. |

## 4. F-4 detail: how each layer is tested on its own

Every forged event has a correct `prevStateHash` and a `postStateHash` recomputed from the forged result, so the chain is intact and only the state-machine guard inside replay can refuse it. The forged file must also **parse** (so a validator cannot be the reason), and the refusal message must name the intended guard. `ForgedHistory_StaleOnAPlan…` includes a control that the same forging technique on a legitimately APPROVED plan **is** accepted.

| Reviewer mutant | New control | Tests that catch it |
|---|---|---|
| X01 PlanStale stage guard | **M18** | forged PlanStale on DRAFT, PROPOSED, EXECUTED, CANCELLED (`LayeredGuardTests`) |
| X06 execute ignores another open plan's reservation | **M19** | `Atomic_TreeAlsoReservedByAnotherOpenPlan_…` via the corrupt-state seam |
| X16 validator double reservation | **M20** | `SnapshotValidator_RefusesATreeInTwoOpenPlans_OnItsOwn` (direct call and through `LabFile.Parse`) |
| X22 listing filter | **M21** | `Listing_OffersOnlySingleLinkRegularFiles`, `ListingFilter_IsAnIndependentLayer_…`, `Classify_…`, the FIFO tests |
| X08 per-entry pre-check | **M22** | `PerEntryPreCheck_IsAnIndependentLayer_…` (scripted `Classify`; asserts the open is never reached) |

Why the seam is used for X06 only: a tree in two open plans cannot be built through the public API (create/propose/approve/queue each refuse it) or through replay, so the corrupt-state seam is the realistic route. This is the same reasoning the reviewer gave in N-2. Also added forged hash-consistent cases for requeue of a PENDING plan (the M17 shape), execution of never-queued DRAFT/PROPOSED/APPROVED plans, approval of a DRAFT, second execution, mark of a dead tree and unmark of a reserved tree.

**Honest limits of F-4:**
- Two things the reviewer listed are still not killed, and I did not manufacture coverage for them: X12 (non-loopback peer, untestable from a loopback-only listener) and X20 (live approval epoch, unreachable because the API always supplies the live epoch). X10 (Sec-Fetch-Site) is still covered by browser test UI-14, not by the dotnet suites. These are outside F-4's five items.
- `NoFollowFiles.ReadRegular` has two size guards (the `statx` size check and the bounded read loop). The second only matters if a file **grows between the check and the read**; I could not construct that deterministically without a test hook inside the libc layer, so the second guard has no independent test.

## 5. Verification

All commands run from `Tools/MultiRackSitkaLab` in the worktree unless noted. All results are **WORKER-SIDE**.

**Tier 1**

| Gate | Command | Result |
|---|---|---|
| Clean build, warnings as errors | `dotnet build MultiRackSitkaLab.sln --no-incremental` after deleting `bin`/`obj` | exit 0, **0 warnings, 0 errors** |
| Core | `dotnet run --no-build --project tests/Core.Tests` | **77 / 77** (baseline 55; +22) |
| Host | `dotnet run --no-build --project tests/Host.Tests` | **12 / 12** (baseline 8; +4) |
| Fixtures | `sha256sum -c fixtures/synthetic/MANIFEST.sha256`; `FixtureTool` regeneration + `cmp` | 3/3 OK; manifest and all 3 fixtures **byte-identical** to the checked-in files |
| Schema | `python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json` | 3 OK, negative controls rejected |
| Mutation controls (scratch copy, once) | `python3 -I -B tests/run-mutation-controls.py <scratch> <json>` | **original M01–M17: 17 / 17 CAUGHT**; new M18–M30: 13 / 13; **30 / 30** |
| Reviewer probe, before vs after | his unmodified `probe-Program.cs`, built against `f33641f8` and `e9b3b730` | before: D1, D2, E9, E10 reproduce (4 defects) · after: all as expected |

**Tier 2 (targeted, not a full requalification)**

| Gate | Result |
|---|---|
| Real headless Chromium, `EVIDENCE_DIR=<ev> node tests/Ui/run-browser-tests.mjs` | **15 / 15**, includes UI-10 save → reset → load → replay and UI-14 security. 2,000-tree selection → inspector → Core preview: **max 162 ms, mean 143 ms** (21 samples; budget 2,000 ms; reviewer measured 157 ms, writer 169 ms) |
| Live-process API/security smoke (`smoke/smoke.py`): the **real host binary** from a scratch copy, attacked over HTTP | after **18 / 18**; the same script on `f33641f8` (FIFO step skipped because it hangs that host) **6 pass / 9 fail** |

The smoke script lives off-repo with the evidence. It covers FIFO plus concurrent state, symlink and hard-link refusal, folder squat, unwritable folder, four `Deny` codes, and the live symlink-swap race against a **valid** lab file outside the root. Two of its checks labelled "session untouched" verify only the HTTP status; the in-process host test `FifoInTheLabFolder_…` asserts the session event count stays 0.

## 6. Platform caveats and residual risk

1. **Linux x64/arm64 only, fail-closed.** Listing, loading and saving use `openat`/`statx` through libc. On first use a self-test in a private temp folder proves the flags work (symlink refused with ELOOP, FIFO opens without blocking, directory flag refuses a file, statx kinds). On any other OS or CPU, or if the self-test fails, the lab throws `PlatformUnsupported` (HTTP 501) rather than using a weaker path-based check. **Behaviour change:** native Windows (never run or qualified by MULTI-002/003) now refuses lab saves instead of using the old racy check. Only **Linux x64** was run. The arm64 flag values (`O_DIRECTORY` 0x4000, `O_NOFOLLOW` 0x8000) come from the kernel headers and are guarded by the self-test, but were **not executed on arm64**. `libc` is resolved by the .NET runtime's probing; musl was not tried.
2. **Writes remain path-based.** `CreateNew` for the temp file and `rename` over the target never follow a final-component symlink, and a squatting folder/FIFO/symlink is refused up front. A same-user process that swaps the **folder itself** during a save is not defended. Reads, which are what can expose data, are. Recorded in the README.
3. **Listing is advisory.** `ListFileNames` still enumerates by path and `Classify` stats relative to a handle. Both can be raced, and by design the authoritative check is the open (M22 and M21 prove each layer separately).
4. **Hard-linked saves are refused** (`nlink > 1`), so a legitimately `cp -l`-duplicated save will not load. An open handle whose name was just replaced reports `nlink 0`, which is **not** treated as a hard link.
5. Opening a FIFO read-only with `O_NONBLOCK` can momentarily release a writer blocked on it; that writer belongs to the same-user adversary, so no new exposure.
6. **Not run:** headed browser, Firefox, Windows, WSL, macOS, arm64, a second Chromium version, any Unity/Tier-3 gate, any P1 behaviour.

## 7. F-6 (spec wording): accepted limitation, for a later Manager-led correction

The approved spec (A3) says "hand-audited" for all fixtures. Only the 3-tree `oracle-quarter-ha` is hand-computed (BA 0.227765 m², 0.911062 m²/ha, N/ha 12, Dq 31.0913 cm; the demo polygon's shoelace area is 5,812.5 m²). The **162- and 2,000-tree stands are formula-generated** by `tools/FixtureTool` (closed-form index arithmetic, no random numbers) and regenerate **byte-identically** (verified above). They are not audited stem by stem. **The spec was not edited.** No synthetic fixture is measured Irish inventory.

## 8. Defects found and fixed during this task (disclosure)

- My first `nlink == 1` rule mislabelled a file as "hard-linked" when the entry was replaced **after** it was opened (the open handle then has link count 0). The concurrent-swap stress test caught it (1,000–3,300 spurious refusals per 2.5 s run). The rule is now `nlink > 1`, and the stress test passed 4 of 4 consecutive runs afterwards. That fix is in the tested commit.
- In the reviewer's own probe, `C4-replay-mark-dead` reports UNEXPECTED on **both** before and after: a forged dead-tree mark is refused one layer earlier, by the snapshot validator at parse time, not by replay. That is unchanged behaviour. The new test forges the event with an unchanged snapshot so replay's own guard is what refuses it.
- The test temp-folder leak the reviewer flagged (N-4): the store tests now delete their folders. The `/tmp` count of older `sitkalab-test-*` folders (198) did not change across my later runs. I did not delete the older ones.

## 9. Reproduce

```bash
cd Tools/MultiRackSitkaLab
dotnet build MultiRackSitkaLab.sln --no-incremental                  # 0 warnings, 0 errors
dotnet run --no-build --project tests/Core.Tests                     # 77/77
dotnet run --no-build --project tests/Host.Tests                     # 12/12
python3 -I -B tests/run-mutation-controls.py "$SCRATCH/mut" results.json   # 30/30 (M01-M17 are the original controls)
dotnet build src/Host && EVIDENCE_DIR=$EV node tests/Ui/run-browser-tests.mjs   # 15/15
```

## 10. Next step

A **focused independent delta review** (not a full re-review), by a reviewer that is not Claude Sonnet 5.5: confirm F-1…F-5 at `e9b3b730` (plus the handoff commit), re-run the probe and the 30 controls, and probe the open-time layer adversarially (for example the folder-swap and the growth-after-fstat case). The Manager decides whether the arm64/Windows caveats need action. Implementation is not integration: this is not a merge, user acceptance, ecological accuracy, field-data import or Unity integration. Dashboard/Drive sync: NOT VERIFIED.
