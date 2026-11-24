# 2D Phase 1 Save/Load — Independent Final-Delta Review (`803bdd83` → `a20640ae`)

**Verdict: NEEDS CHANGES** (one SHOULD finding, bounded to `MultiSaveAsync`; no BLOCKER). Everything else in the delta checks out.  
**Reviewed source SHA:** `a20640ae4fa5f025e68394bffa386236d02106e8` (delta base `803bdd834a0181b7754e411c34381c024fa26f60`).  
**Branch:** `feat/multi-2d-phase1-save-e2e-20261124-v1`, live tip `7e1b1eaa0784560fdb50bce0b9f0b57c8d820f84` (checked via `gh api` at start and end of review). `a20640ae..7e1b1eaa` changes **only** the implementation handoff Markdown, so the final save source is still `a20640ae`.  
**Binding packet:** [Manager verdict, B-01 sign-off, playtest and delta-review packet](https://github.com/jermur788/CCF/blob/docs/multi-2d-save-e2e-manager-verdict-playtest-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-SAVE-E2E-MANAGER-VERDICT-B01-SIGNOFF-PLAYTEST-AND-DELTA-REVIEW-20261124.md) §3.  
**Reviewer:** Claude Opus 5.5 (Claude Code), 2026-11-24.  
**Independence:** a new session that did not take part in implementing `a20640ae` or any earlier repair. Its only inputs were the owner's task message and the binding packet; it then read the implementation handoff at `7e1b1eaa` to compare claims. No nested reviewers were used. Read-only: the shared checkout, the feature worktree, `.p0ref` and P0 files were not modified. Nothing was merged. No Unity and no Windows build.

## 1. Method and custody

- Disposable `git clone --shared --no-checkout` of the local repo at `/tmp/rv-a206`, sparse (P1 lab, P0 lab tree, the 4 linked `Assets/ForestPrototype` files), **detached at `a20640ae`**. `.p0ref/` was copied read-only from the released feature worktree after checking its SHA-256 = pin `9a99f781…34b3`. The pin was checked again after all runs (unchanged in both the copy and the feature worktree). The feature worktree's `git status` was clean.
- `git diff --stat 803bdd83 a20640ae -- Assets Tools/MultiRackSitkaLab` is empty, so the delta has no Unity or P0 changes.
- The delta is 7 files: `Phase1SaveSlots.cs`, `PlayerMultiApi.cs`, `Phase1SaveIntegrationTests.cs`, `Phase1.Save.Tests/Program.cs`, `drive.mjs`, `run-edge-e2e.sh` and the handoff. Each one was read in full in the diff, with its call sites in context.

## 2. Own reproduced results (at `a20640ae`)

| Check | Result |
|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln` + `src/Ui/SitkaP1.Ui.csproj` + `Phase1Save.Tests.csproj` | 0 warnings / 0 errors |
| Core suite (includes the anchor/frozen-digest tests and both `644 B-01` tests) | **267/267 PASS** |
| Save contract suite (`NB1a/b/c`, `R2`, missing/corrupt-primary recovery) | **48/48 PASS** |
| `tests/phase1-save-ui/run-edge-e2e.sh` (real headless Microsoft Edge, 4 server processes, scratch `XDG_DATA_HOME`) | **63/63 PASS**, `OVERALL: PASS`. The `recover` phase is 6/6 (corrupt listing, Load withheld, confirmation names the damaged primary, "Restored the verified backup…", slot "Ready to load", recovered save loads at year 1, no console/CSP error) |
| `ui-smoke.sh`, `ui-smoke-multiarea.sh`, `cli-safety.sh` | NOT RUN (outside the delta; the delta does not touch these surfaces) |
| `build-output-safety.sh`, `p0-isolation.sh` | NOT RUN on purpose (destructive to the P0 pin; not reproduced, per packet) |

### Mutation checks (one exact replacement each, clean rebuild, then the tree restored — final `git status` clean)

| Mutation | Expected detector | Result |
|---|---|---|
| M1 `LockSlot` no longer enters the gate | `NB1c` | **killed**: `NB1c` FAIL, generations `(5,5)` |
| M6 (test) `NB1c` without the `LockSlot` scope, i.e. the old read-then-write interleaving | `NB1c` | **killed**: FAIL `(5,5)`. This reproduces the pre-`a20640ae` duplicate-generation defect |
| M3 broadleaf Prune lift-target check disabled (`Phase2Rules.cs:253`) | `P1-SL-07c` | **killed**: `P1-SL-07c` and `P2-03 M-06` FAIL |
| M5 (test) `P1-SL-07c` target reverted to the old `99f` | the pinned code | **killed**: `expected PruneBroadleafTargetInvalid, got PruneTargetOutOfRange`. The repair now pins the intended cause |
| M4 Stage 4 options ignored in `RequireSameOptions` | `P1-SL-09` | **killed**: `P1-SL-09` FAIL ("expected a refusal, but the call succeeded") on the new Stage 4 branch |
| M2 recovery of a **missing** primary uses the clobbering `rename(2)` | — | **survived** (48/48). The no-clobber path has no test. See N-01 |

## 3. Findings

### F-01 — SHOULD — the slot `Monitor` is held across an `await`, so it can be released from the wrong thread

**Location:** `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs:210-227` (`MultiSaveAsync`), together with `src/Core/Phase1/Phase1SaveSlots.cs:256-274` (`LockSlot`/`SlotScope`).

`LockSlot` enters a `System.Threading.Monitor`, which is thread-affine: only the thread that entered it may exit it. `MultiSaveAsync` does `await WriteJsonAsync(...)` **inside** the `using (saveSlots.LockSlot(slot))` block. If that write does not complete synchronously, the continuation, and so `SlotScope.Dispose → Monitor.Exit`, runs on a different thread-pool thread. C# rejects `await` inside a `lock` statement for exactly this reason. The `IDisposable` wrapper gets past that compile-time check.

**Reachable failure:** the response write completes asynchronously (Kestrel output backpressure, a slow or stalled client). Then:
1. `Monitor.Exit` throws `SynchronizationLockException`. It is not a `P1Refusal`, so it escapes the handler after the slot file was already committed.
2. The gate stays owned by the original thread for the life of the process. Any later Save/Recover/NextGeneration of that slot on any other thread blocks forever and holds a thread-pool thread. On the owner thread it re-enters, so mutual exclusion also stops holding there.

Even without the thread hop, the gate is held during network I/O to the client, which blocks other same-slot requests synchronously.

**Evidence (probe outside the repo, against the `a20640ae` `SitkaP1.Core.dll`):** `using (slots.LockSlot("probe")) { await Task.Yield(); await Task.Delay(20); }`, with `Task.Yield` standing in for an asynchronously completing write. All 3 runs: `enter thread 4, dispose thread 6, fault: SynchronizationLockException`. In 1 of 3 runs, a follow-up `NextGeneration("probe")` on another thread was **BLOCKED > 3 s (gate leaked)**. In the other two it was scheduled on the owner thread and re-entered. I did **not** force an asynchronous Kestrel write end to end, so whether this happens in normal use is not demonstrated. With small (<1 KB) payloads, Kestrel usually completes the write synchronously. The defect sits in the exact guarantee this delta adds, the consequence (a stuck slot until restart, a thread-pool thread lost, a committed save followed by a server error) is serious, and the fix is trivial. That is why I rate it SHOULD rather than NIT.

**Minimal correction (scope-preserving):** build the `MultiSavePayload` inside the scope, and `await WriteJsonAsync` after the `using` block closes. In other words, the scope covers only `NextGeneration → CaptureSave → Encode → Save → Interlocked.Increment`. The `SlotExists` refusal keeps working because it throws out of the scope into the existing `catch`. Optionally, note in `LockSlot`'s XML doc that the scope must not span an `await`. No other files need changing.

### N-01 — NIT — the recovery no-clobber path (R4) is untested

Mutation M2 (always clobber in `RecoverFromBackup`) leaves all suites green. The code is correct by inspection: `Phase1SaveSlots.cs:237-238` uses `AtomicRenameNoReplace` when the primary was absent. In-process, the gate already prevents the race, so only a second process can reach it. That is a disclosed limitation. A fault-hook-driven unit test (create the primary between inspection and rename, expect `SlotExists`) would pin it. Optional.

### N-02 — NIT, pre-existing (not in the delta) — generation resets to 1 over a corrupt primary

`NextGeneration` returns `(true, 1)` when the primary fails to decode (`Phase1SaveSlots.cs:288`). So a save over a corrupt slot whose verified backup carries generation *N* writes generation 1, and the backup is not refreshed from a corrupt primary. The per-slot "unique generation" claim then holds only among saves since the last good primary. No data-integrity effect. A wording or `max(backup)+1` refinement is optional.

### N-03 — NIT — the browser recovery phase does not prove *which* save was loaded

`drive.mjs recoverSlot` checks `ecologicalYear >= 1` and that the map year matches. It does not compare the loaded checksum or label with the backup's. The server-side identity of recovery is covered by `R2` and the slots tests. Optional.

### N-04 — NIT — runner header drift

`run-edge-e2e.sh` lines 5-13 still describe "three independent server processes" and three phases. There are now four (`mixed-c` recovery).

## 4. Specific packet checks

1. **Per-slot gate across generation read → capture → atomic save; distinct generations.** The structure is right. The handler does `NextGeneration`, `CaptureSave` and `Save` in one re-entrant scope. `NB1c` passes, and its old interleaving is killed (M1/M6, `(5,5)`). **There is no lock inversion.** No `src/**` code outside the UI handler touches `Phase1SaveSlots`, and the engine never takes a slot gate, so the order is always slot gate → engine lock. `MultiLoadAsync` evaluates `saveSlots.Load` (ungated) before `multiSession.Restore`, so nothing nests there. **Defect:** F-01, the scope spans an `await`.
2. **Recovery no-clobber; valid newer primary never displaced.** A missing primary is published with `RENAME_NOREPLACE` (or a `link(2)` fallback) and becomes `SlotExists` on a race. A valid primary is refused with `SlotPrimaryValid`, and that is checked server-side under the gate on each request, so a stale listing or stale confirmation cannot bypass it (`R2` passes). Limitation, correctly disclosed in handoff §8: the gate is in-process only. A second process could replace a *corrupt* primary between `Inspect` and `rename(2)`, and recovery would then overwrite that new primary. Untested no-clobber path: N-01.
3. **Load/Restore confirmations and browser phase 4.** `askLoad` and `askRecover` each hide the other confirmation and null the other pending slot. Closing the dialog clears both, and `resetForLoad` clears `loadSlot`. Both handlers check `state.busy` and the pending slot. Recovery success only comes from an `ok` server response, and refusals are reported as "Backup not restored (code)". The real Edge phase passed in my run (6/6, screenshots in my local evidence folder `ccf-delta-review-e2e-a20640ae`). Load still carries the fence, so a stale session is refused (the existing `load` phase, which also passed).
4. **`P1-SL-07c` and `P1-SL-09`.** 07c pins `PruneBroadleafTargetInvalid` with an in-range 4.0 m Oak target. Reverting to 99 m gives `PruneTargetOutOfRange`, which shows the old test was hitting a different rule (M5). 09 now exercises the Stage 4 growth-phase branch and kills M4.
5. **No unrelated drift; anchors; P0.** The UI delta is only the confirmation hide/clear lines. There is no forestry/engine change apart from the save store and handler. Core 267/267 passes, including the anchor tests. No `.p0ref`, P0 tree or `Assets/**` change, and the pin was unchanged after my runs.
6. **Save source after `a20640ae`.** None. Only the handoff changed up to tip `7e1b1eaa`.

### 644 B-01 exception — confirmed exactly one source file

At `a20640ae`, `tests/Core.Tests/Program.cs:428-446` skips only paths ending in `/src/Core/Phase1/Phase1SaveSlots.cs`. Every other `src/**/*.cs` (excluding `bin/obj`) is still scanned for the full banned list (`File.Write/Create/Open/Append/Copy/Move/Delete/Replace/ReadAll`, `FileStream`, `Directory.Create/Delete/Move`, `StreamWriter`), and `T.Eq(1, exempt)` asserts the single exemption. The test passes (so no other file uses a banned token). The descriptor guard test is unchanged. This matches the Manager sign-off. The delta adds no new file I/O outside the exempt store (`LockSlot` is just a `Monitor`).

## 5. Handoff claim comparison

The R3–R7 table in the implementation handoff (lines 147-151 at `7e1b1eaa`) matches the code, with one exception. R3's "two saves of one slot can neither interleave nor record the same generation" holds only while the scope does not span an `await` (F-01). Reported counts (Core 267, Save 48, Edge 63) match my own reruns.

## 6. Verdict and next action

**NEEDS CHANGES**: fix F-01 only. Move `await WriteJsonAsync` out of the `LockSlot` scope in `MultiSaveAsync`. This is a bounded return to the same save feature, not a tooling refactor. After the fix, a re-check only needs to confirm the handler shape and rerun Core 267 / Save 48 / Edge 63. N-01 to N-04 are optional.

G2 (code review) for the delta: NEEDS CHANGES on F-01, everything else supported. G3 (verification): Core, Save and Edge E2E were reproduced independently at `a20640ae`. `ui-smoke*`, `cli-safety`, `build-output-safety` and `p0-isolation` were NOT RUN by this review. This review does not authorise a merge to `main`. The owner playtest can go ahead separately: F-01 is unlikely to show up in single-player local use, but if a Save ever hangs the server, restarting it clears the gate.
