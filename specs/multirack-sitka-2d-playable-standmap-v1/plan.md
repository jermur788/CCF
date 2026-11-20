# Multi — Playable 2D Stand Map: implementation and qualification plan (DRAFT)

- **Spec:** [spec.md](spec.md), **DRAFT**. This plan implements that spec and creates no product requirement of its own.
- **Gate:** nothing in Phases II–V starts until **owner G5** (spec §3) and **#730 acceptance** (spec §0.3, §6) are recorded on GitHub. Phase I (docs and environment proof) is complete in this commit.
- **Writer:** one exclusive writer (Claude Opus 5.5) for every path in [tasks.md](tasks.md). Optional Sonnet 5.5 help only on a disjoint, explicitly assigned path, never the Core files. The independent reviewer (GPT-6.1 Sol) never writes the candidate.

## P0. Preconditions (re-checked at each phase start)

1. Live `Docs/Project/coordination/active-tasks.md` and the #730 thread show this task's claim, writer and reviewer, and no overlapping writer on `Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs` (#846 released).
2. The branch `feat/multi-standmap-playable2d-20261120-v1` descends from `4bb80973`. No rebase, cherry-pick to main or force-push.
3. The G5 decision and #730 acceptance are recorded, with any requested changes applied to spec.md first (new commit; earlier commits never rewritten).

## P1. Environment and evidence discipline (from the Phase I findings)

- **Editing lane:** the T3 Windows checkout `C:\Users\hp\.t3\worktrees\CCF-HP\feat-multi-standmap-pla-b40873fbd04c0a28`. Its git admin directory is in WSL (`//wsl.localhost/...`). Windows git shows 563 mode-only artifacts (`100755 → 100644`) and applies `core.autocrlf=true`. Rules:
  - every git command uses `-c safe.directory=* -c core.filemode=false`;
  - stage only explicit paths; never `add -A`, `add .` or `commit -a`;
  - verify the commit with `git show --stat HEAD`;
  - commit with an explicit author identity (`-c user.name=… -c user.email=…`), because the lane's configured identity is the non-author DeepSeek parent;
  - never run `git worktree prune` (WSL lists this worktree as `prunable`).
- **Fast iteration (not evidence):** build and test in the lane from WSL with `GIT_DIR=/home/jer/Development/CCF-HP/.git/worktrees/feat-multi-standmap-pla-b40873fbd04c0a28`. Expect exactly one environment-caused failure: the `CcfSourcePins` working-tree SHA check, caused by CRLF. Any other failure is real.
- **Tier-1 evidence lane:** after each push, run `make-pristine-clone.sh` (Phase I scripts, recorded in the Phase I note) against the **pushed commit SHA** in a fresh WSL-native LF sparse clone under `~/.ccf-orch/standmap-p1-opus/`. Copy `.p0ref` from a pinned copy and verify that the DLL SHA equals the csproj pin, the `.sha256` record and `provenance.txt`. **P0 is never rebuilt.** Then build and test there. Only clone results count as Tier-1.
- **SDK:** `~/.dotnet/dotnet` 8.0.134 (satisfies `global.json` 8.0.133 latestPatch). Set `TMPDIR` outside every repository.

## P2. Phase II — same-lock player session facade (Core only)

1. **`AnnualContinuation.cs` (modify, minimal):**
   - add `SessionId`, plus `WorkRoot.PlanStateRevision` and `WorkRoot.AnnualHistory`;
   - increment the plan-state revision at the single publish point of Add, Remove, Approve, Cancel, Execute and consented-cancel Advance;
   - append an `AnnualYearRecord` in `AdvanceLocked`;
   - add `ReadPlayerView()` and the fenced internal commands of spec §6.4–6.5, implemented by fence-check, then the reentrant legacy call, then the view, all under one `sync`.

   Legacy validation order and refusal codes stay unchanged, as do `ContinuationSnapshot` and `AnnualReport`.
2. **`PlayerSessionView.cs` (new):** public DTO records and `PlayerLabels`.
3. **`PlayerWorkPlanAdapter.cs` (new):** a public facade that opens the trusted fixture and maps `P1Refusal` to `PlayerCommandResult`. It holds no state of its own and exposes no seams.
4. **`PlayerMapScene.cs` (new):** a pure scene builder and hit-test (spec §4.3–4.4, §5).
5. **Tests (new):** `tests/Core.Tests/PlayerSessionTests.cs` and `tests/Core.Tests/PlayerMapSceneTests.cs`, registered by **one added line** in `tests/Core.Tests/Program.cs` (`PlayerSessionTests.Register(manifestPath);`, which also registers the map tests). The dependency-free runner has no test discovery. A `[ModuleInitializer]` alternative was rejected as non-obvious for a learner.
6. **Exit:** lane fast loop green except the known CRLF case; push; Tier-1 clone run of the pushed SHA showing **66 original + new tests passing** and ten digests identical. Record the counts.

**Phase II test list (minimum).** Ids are referenced by the checklist:

| Id | Proves |
|---|---|
| PS-01 | the view is atomic under concurrent Execute/Advance readers (spec AC-PM-02) |
| PS-02 | plan-state +1 exactly on Add/Remove/Approve/Cancel/Execute/consented Advance; unchanged on read, refusal, replay and no-work Advance |
| PS-03 | ABA (add X, remove X) leaves the old fence stale although pending content is equal |
| PS-04 | wrong session / stale source / stale plan-state refused for all six commands with zero mutation (full view compared) |
| PS-05 | Add, Remove, Approve and Cancel make zero Fell; token unchanged; Approve mints the PlanId; callers cannot supply one |
| PS-06 | Execute fells exactly the approved IDs once, with cause `fell`/`manual-work-plan`; year and annual revision unchanged; work revision +1; count/BA from `WorkRules.Summarize` |
| PS-07 | same-key replay with the original fence gives `Replayed = true` and zero mutation |
| PS-08 | same key with a different PlanId gives `WorkExecutionKeyConflict` with zero mutation |
| PS-09 | a different session gives `PlayerSessionMismatch` with zero mutation |
| PS-10 | a pre-Fell fence is stale for Add, Approve and Advance after a Fell |
| PS-11 | a pre-advance fence is stale after Year+1 |
| PS-12 | an all-live batch gives `WorkWouldLeaveNoLivingStems` with zero mutation; the approved plan is still present |
| PS-13 | Advance without consent is refused with zero mutation; with consent, orders are cancelled at zero Fell, there is one genuine year, plan-state +1 and an `AnnualYearRecord` with `CancelledUnexecutedWork` |
| PS-14 | after a Fell, Year+1 survivors equal `AnnualRules.Calculate(post-Fell state)`; the felled tree stays dead with its Fell death record; natural deaths keep their own cause |
| PS-15 | a second decision cycle in Year+1 (fresh fence: add, approve, execute) works |
| PS-16 | an injected fault through the internal seams (tests only) leaves zero mutation, including plan-state and history |
| PS-17 | the view's collections are read-only and later commands do not change an earlier view |
| PS-18 | 100 exact original IDs in fixture order, scope `MS-P30-UNDIVIDED-30M-V1`, labels exactly as `PlayerLabels` |
| PS-19 | reflection: the public adapter surface has no proposal/comparator/sample/seam/plan-object member |
| MS-01 | the scene has one item per frame tree, exact IDs, `U = X + 15`, `V = 15 − Z`, crown radius = `CrownRadiusM` |
| MS-02 | paint order is height-ascending then ordinal; stumps after crowns; deterministic across builds |
| MS-03 | hit-test on synthetic overlapping discs gives topmost-first, inclusive boundary, empty on bare ground |
| MS-04 | for every living fixture tree, its centre's candidates include it; `candidates[0]` has the highest paint order containing the point |
| MS-05 | status mapping for pending/approved/felled/natural dead |
| MS-06 | an adapter hit-test with a stale fence is refused after Execute |

## P3. Phase III — playable UI (after G5)

**If UI-A:**

1. `src/Ui/SitkaP1.Ui.csproj`: Exe; `FrameworkReference Microsoft.AspNetCore.App`; Core project reference; `wwwroot/**` embedded. First check `dotnet restore` works **offline**, using only the installed 8.0.34 pack.
2. `src/Ui/Program.cs`: argument parsing (fixture directory only), capability generation, Kestrel loopback port 0, printing the URL, Ctrl+C shutdown.
3. `src/Ui/PlayerApi.cs`: the guards of spec §3.6 and JSON endpoints that are thin calls into the adapter: `GET view`, `POST hit`, `pending/add`, `pending/remove`, `plan/approve`, `plan/cancel`, `plan/execute`, `year/advance`. Each returns `{view, refusal?, execution?, annual?}`.
4. `src/Ui/wwwroot/index.html`, `app.css`, `app.js`: regions, SVG map, table, inspector, Work Plan, dialogs and alerts, per spec §7–8. No framework, no CDN, no web storage.
5. `tests/ui-smoke.sh` (new; bash + curl):
   - bind only to 127.0.0.1 (`/proc/net/tcp`) on a port other than 5187;
   - 404 without the capability; 403 for a wrong Host or Origin; 415 for a wrong content type; 413 for oversize;
   - the full API loop (add, approve with zero Fell, execute, replay, advance) and stale refusals;
   - after the run, `git status` is clean and `~/.aspnet` was not created.
6. `README.md` (lab): add a "Playable 2D Stand Map (Stage 5 slice)" section with the launch path and its honesty boundaries, and update the stale "Host / UI … Not implemented" row.

**If UI-B:** the same `src/Ui` folder holds `SitkaP1.Ui.csproj` (no FrameworkReference), `Gtk.cs` (interop), `Program.cs` and `MainWindow.cs`. No `wwwroot` and no `ui-smoke.sh`; a manual operation script replaces them. The path list is re-issued to #730 before writing.

**Human-operable evidence (author Tier-1, UI-A):** launch from WSL and operate from a Windows browser at the printed URL (T3 preview browser and/or Microsoft Edge). Capture real screenshots or recordings with provenance (exact SHA, time, viewport) at 1280×800, at 360 CSS px width and at 200 % zoom, covering:

- S0 initial view;
- selection by click and by table, and an overlap list;
- a refusal (blank reason, then wrong-case ID);
- pending, then approved ("0 felled");
- the execute confirmation, then the receipt with canopy gap and count/BA;
- the consent dialog (deny, then consent);
- Year+1 history.

Also run a keyboard-only traversal of the same flow. Screen reader: NVDA, if the owner can run it; otherwise **NOT RUN**.

## P4. Phase IV — qualification

- **Author Tier-1** (pushed SHA, pristine clone):
  - clean build with 0 warnings and 0 errors;
  - 66 base tests unchanged plus the new tests;
  - ten digests identical;
  - `git diff 4bb80973..<sha> --stat` limited to the tasks.md path list;
  - protected blobs unchanged (fixture, `AnnualRules.cs`, `WorkContract.cs`, `AnnualEngine.cs`, CLI, `.sln`, Core and test csproj, `Tools/MultiRackSitkaLab` tree `f212c8f6`, linked Assets blobs);
  - `ui-smoke.sh` green;
  - browser evidence per P3.
- **Mutation spot-checks (author):** temporarily revert the plan-state increment in Execute and confirm PS-02 fails; drop the PlanId check and confirm PS-08 fails. Revert the mutation before committing, and record it.
- **Independent Tier-2 (GPT-6.1 Sol, fresh clone, non-author):** source review of every changed line; rerun of build and tests; negative stale, atomic and replay tests; the real UI path at 360 px and 200 %, keyboard and touch emulation, with provenance of renders; at least one fault or mutation test of the reviewer's choosing. Verdict APPROVED / NEEDS CHANGES / REJECTED. NEEDS CHANGES returns to the author for a new commit on the same branch, followed by a re-review of the changed code.

## P5. Phase V — handback

- One GitHub Markdown handoff under `Docs/Handoffs/MultiRack-Simulation-20261116/`, containing:
  - exact SHA and changed-file blob list;
  - Tier-1 logs summary with external log locations and hashes;
  - screenshots and recordings with provenance;
  - a per-item verdict for 26 R + 24 X (PASS / FAIL / N/A — G1 DISABLED / NOT RUN) and for this delta's checklist;
  - the reviewer verdict and remaining blockers.
- Labels: **IMPLEMENTED / REVIEWED / QUALIFIED (scoped) / NOT INTEGRATED / NOT RELEASED / NOT USER ACCEPTED** unless separate gates say otherwise.

## P6. Risks and mitigations

| Risk | Mitigation |
|---|---|
| A plan-state increment edit changes legacy behaviour | Edits only at publish lines; base 66 tests must pass unchanged; PS-02 plus mutation check |
| Torn view under concurrency | One `sync` acquisition per read or command; PS-01 stress with many readers |
| Replay accepted across plans (A-2) | Fenced path checks PlanId, source and session before staleness; PS-07/08/09 plus mutation check |
| UI shows an optimistic Fell | Page renders only from returned views; no client-side state change before the response; UI evidence reviewed by Sol |
| Loopback exposure (UI-A) | Capability path + Host/Origin + 127.0.0.1-only bind; `ui-smoke.sh`; Sol review |
| CRLF / gitdir lane artifacts | Tier-1 only in the pristine LF clone of the pushed SHA |
| Wrong commit attribution | Per-command author identity; disclosure in the handoff |
