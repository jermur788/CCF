# Phase 1 2D Save E2E — F-01 repair re-review (2026-11-24)

**Reviewer:** Claude Opus 5.5 (`claude-opus-5-5`, Anthropic), working as an independent, read-only review sub-agent in a fresh thread.
**Branch reviewed:** `feat/multi-2d-phase1-save-e2e-20261124-v1`
**Reviewed SHA:** `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43`. I re-read the live tip through `gh api` before starting and again before publishing; it had not changed.
**Delta:** `7e1b1eaa0784560fdb50bce0b9f0b57c8d820f84..5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43` (one commit: `5438c94c fix(multi-2d-phase1): release the save slot lock before awaiting the response (F-01)`)
**Finding under repair:** F-01 (SHOULD) in
[`PHASE1-2D-SAVE-E2E-FINAL-DELTA-REVIEW-20261124.md`](https://github.com/jermur788/CCF/blob/b2743df4c666194e09bf88558286533b8fea2129/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-SAVE-E2E-FINAL-DELTA-REVIEW-20261124.md)

## Freshness and independence

- **Inputs:** a task brief from the orchestrating agent. The brief named the branch, the delta range, the F-01 review link, the scope (a)–(d), the environment and gate commands, and the expected baselines (Core 267, Save 48, Edge 63). I did not use any implementer verdict as evidence. I read the code and the diff from Git objects myself, and I reran every gate myself in a disposable clone.
- I did not edit the feature branch, the implementer worktree, `main`, `.p0ref`, P0 files or Unity `Assets/`. This document is the only write, on its own docs-only branch.
- **Scope:** F-01 and this delta only. I did not re-review the rest of Phase 1 and did not reopen N-01..N-04.

## Delta read

`git diff --name-status 7e1b1eaa..5438c94c` lists exactly two paths:

| Path | Change |
|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` | +7 / −4, all inside `MultiSaveAsync` |
| `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-SAVE-E2E-IMPLEMENTATION-AND-VERIFICATION-20261124.md` | +37, new §12 appended |

This is `MultiSaveAsync` at `5438c94c`, lines 206–235:

```csharp
try
{
    MultiSavePayload payload;
    using (saveSlots.LockSlot(slot))
    {
        var (exists, generation) = saveSlots.NextGeneration(slot);
        if (exists && !request.ReplaceExisting) throw new P1Refusal("SlotExists", ...);
        var envelope = multiSession.CaptureSave(request.Fence!, label, generation);
        byte[] bytes = Phase1SaveCodec.Encode(envelope);
        var result = saveSlots.Save(slot, bytes, request.ReplaceExisting);
        Interlocked.Increment(ref savesWritten);
        var header = envelope.Header;
        payload = new MultiSavePayload(new MultiSaveOutcome(...same 9 fields...), null);
    }
    await WriteJsonAsync(context, StatusCodes.Status200OK, payload);
}
catch (P1Refusal refusal)
{
    await WriteJsonAsync(context, StatusCodes.Status409Conflict, new MultiSavePayload(null, new Notice(refusal.Code, refusal.Message)));
}
```

### (a) Repair correctness

- **Lock released before any `await`:** yes. The `using` body has no `await`. Its only `await` used to be the 200 write, which now runs after the scope closes. `SlotScope.Dispose()` (`Phase1SaveSlots.cs`, around line 268) calls `Monitor.Exit` on the thread that entered the lock.
- **Generation read, capture and write in one scope:** yes. `NextGeneration`, the `SlotExists` check, `CaptureSave`, `Encode`, `Save` and `Interlocked.Increment` all stay inside the same `using (saveSlots.LockSlot(slot))`, in the same order as before. `NextGeneration` and `Save` take `lock (GateFor(slot))` on the same re-entrant gate, so behaviour is unchanged.
- **`SlotExists` 409 path:** kept. The `P1Refusal` is thrown inside the `using`, so the scope is disposed synchronously on the acquiring thread before the exception reaches `catch (P1Refusal)`. The catch block, and its awaited 409 write, are byte-identical to `7e1b1eaa`. `P1Refusal`s from `CaptureSave`, `Encode` and `Save` follow the same path.
- **Response shape:** unchanged. The 200 body is built with the same `MultiSavePayload(new MultiSaveOutcome(SlotName, status word, BackupUpdated, Checksum, Bytes, SaveGeneration, Label, EcologicalYear, SavedUtc), null)` arguments. The 409 body is unchanged.
- **Definite assignment:** `payload` is assigned on every non-throwing path out of the `using`, so the code compiles with 0 warnings.
- **Ordering semantics:** unchanged. As before, the save is durably written and counted before the response is sent. A client abort during the response write still leaves a committed save, which is the same behaviour as before.

### (b) Sibling handlers

- **Search:** `git grep -n -E 'LockSlot|Monitor\.|lock ?\(|SemaphoreSlim' 5438c94c -- Tools/MultiRackSitkaP1Lab/src/Ui` returns only `PlayerMultiApi.cs:211` (the comment) and `:212` (the `using` in `MultiSaveAsync`).
- **`ReadMultiSavesAsync`** (line 154): not `async`. It runs `ListSlots`, `Inspect` and `Load` synchronously and returns `WriteJsonAsync(...)`. No lock is held when it returns.
- **`MultiLoadAsync`** (line 242): calls `multiSession.Restore(..., saveSlots.Load(...))` and `ReadCompositeUiSnapshot()` synchronously, then awaits the write. No UI-held lock.
- **`MultiRecoverAsync`** (line 263): calls `saveSlots.RecoverFromBackup(...)` synchronously, then awaits the 200 or 409 write. No UI-held lock.
- **Other handlers in the file** (`ReadMultiCellAsync`, `ReadMultiCompetitionAsync`, `MultiHitTestAsync`, `MultiCommandAsync`, `ReadMultiRequestAsync`): all of their awaits are body reads or response writes, and none holds a lock.
- **Core gates:** the remaining gate use in `Phase1SaveSlots.cs` is C# `lock` statements. The compiler forbids `await` inside those (CS1996), so they cannot span an await.
- **Result:** no sibling handler holds `LockSlot`, `Monitor` or `lock` across an `await`.

### (c) Delta boundary and handoff §12

- **Boundary:** the delta touches only `MultiSaveAsync` and the handoff, as the brief says.
- **Handoff accuracy:** §12 describes the code accurately. That covers the defect, the fix sequence, the 409 path, the unchanged response shape, and the sibling audit (`ReadMultiSavesAsync` non-`async`; the load and recover handlers synchronous before their awaits).
- **Status claims:** §12's gate numbers (267/48/63, 0 warnings) match my independent rerun. Its status line ("not INTEGRATED, not ACCEPTED") is appropriately conservative.

### Out-of-repo lock probe (optional evidence)

- **Setup:** a disposable `/tmp/rr-probe` console project that references the `SitkaP1.Core.dll` built at `5438c94c` (and the `.p0ref` P0 dll) by HintPath. Each pattern runs on a dedicated non-pool thread, so any continuation after a real `await` resumes on a different thread.
- **Pattern A** (pre-repair shape, `await Task.Delay` inside `using (LockSlot)`): throws `SynchronizationLockException`, and the gate stays **leaked** (another thread cannot acquire the slot within 2 s). This confirms that F-01 was real.
- **Pattern B** (repaired shape: synchronous `NextGeneration` inside the scope, `await` after it): `ok`, and the gate is free for other threads.
- **Pattern C** (refusal thrown inside the scope, `await` in `catch`, i.e. the `SlotExists`/409 path): `ok`, and the gate is free.
- **Result:** `PROBE PASS`, exit 0. The probe project was deleted afterwards and was never in the repo.

## Findings

**None.** No BLOCKER, SHOULD or NIT findings against this delta.

F-01 is **resolved**.

Non-finding observation: no in-repo test pins the "no await inside `LockSlot`" shape. The existing NB1b/NB1c save tests exercise serialisation and generation uniqueness, not thread affinity. This is optional future hardening and does not affect this verdict.

## Independent gate results (at `5438c94c`, disposable sparse clone `/tmp/rr-5438`)

- **Clone:** `git clone --shared --no-checkout` from the main repo, sparse paths exactly as the brief lists, detached at `5438c94c` with a clean tree.
- **`.p0ref`:** copied read-only from the implementer worktree after checking `SitkaLab.Core.dll` sha256 = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. It was unchanged after the run in both the clone and the worktree.
- **Toolchain:** .NET SDK 8.0.134.

| # | Gate | Result | Exit |
|---|---|---|---|
| G1 | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers --no-incremental -nologo` | Build succeeded, 0 Warning(s), 0 Error(s) | 0 |
| G2 | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers -nologo` | Build succeeded, 0 Warning(s), 0 Error(s) | 0 |
| G3 | `dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --disable-build-servers -nologo` | Build succeeded, 0 Warning(s), 0 Error(s) | 0 |
| G4 | Core suite `SitkaP1.Core.Tests.dll` | **267/267 passed, 0 failed** | 0 |
| G5 | Save suite `Phase1Save.Tests.dll` | **48/48 passed, 0 failed** (scratch removed: True) | 0 |
| G6 | Edge E2E `tests/phase1-save-ui/run-edge-e2e.sh` | **OVERALL: PASS (63 passed, 0 failed)** | 0 |

- **G6 evidence:** `/mnt/c/Users/hp/AppData/Local/Temp/ccf-f01-rereview-e2e-20261124T235021Z`. The script's own servers stopped cleanly on SIGINT (Z01/Z02 PASS) and wrote nothing to stderr.

## Listener status

The protected listeners were identical before and after the Edge run, with the same PIDs. I did not touch them.

```
LISTEN 127.0.0.1:43649  dotnet pid=1806361
LISTEN 127.0.0.1:33767  dotnet pid=1750123
LISTEN 127.0.0.1:46593  dotnet pid=839989
```

After the run, `pgrep` showed no `sitkap1-ui.dll` process started from my clone. All remaining `sitkap1-ui.dll` processes predate this review (started 20–23 Nov, with cwd in other worktrees). I did not touch them.

## Not run

- **Excluded by the brief:** `build-output-safety.sh`, `p0-isolation.sh` and `build-p0-core-ref.sh`.
- **No Unity, Windows or rendered-player verification:** none applies to this tool-only delta.
- **No merge, PR, tag or force push.**
- **Outside scope:** the rest of Phase 1 and N-01..N-04.

## INTEGRATION DECISION

**APPROVED**

The F-01 repair releases the thread-affine slot `Monitor` before any `await`. The generation read, capture and write stay in one locked scope, and the 409 path and response shape are unchanged.
No sibling handler in `PlayerMultiApi.cs` holds `LockSlot`, `Monitor` or `lock` across an `await`. The delta is limited to `MultiSaveAsync` and an accurate handoff §12.
My independent gates passed: builds 0/0/0 warnings, Core 267/267, Save 48/48, Edge 63/63, all exit 0.
This approves the F-01 repair for the bounded save branch only. It is not permission to merge to `main`, and the branch remains not INTEGRATED and not ACCEPTED.
