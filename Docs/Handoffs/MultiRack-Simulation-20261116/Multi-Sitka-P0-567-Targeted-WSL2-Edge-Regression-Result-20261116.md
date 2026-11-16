# Multi P0 #567 — Targeted Windows Edge / Ubuntu WSL2 delta regression: result (#589)

**Date:** 16 November 2026. **Programme:** standalone 2D Multi-Rack Sitka P0 no-growth lab — **not Scenario One, not Unity, not P1 growth.** **Claim:** [#589](https://github.com/jermur788/CCF/issues/589). **Binding packet:** `docs/multi-sitka-p0-586-manager-acceptance-wsl-delta-20261116-v1` (`Multi-Sitka-P0-586-MANAGER-SOURCE-APPROVAL-AND-WSL-DELTA-PACKET-20261116.md`). **Source under test:** #567 (`fix/multi-sitka-p0-save-response-r3-20261116-v1`), independently source/security reviewed in #586.

## 0. Verdict (worker recommendation; final acceptance is the Manager's)

**APPROVED TARGETED WSL2/EDGE DELTA** — for the exact candidate `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, on the owner's PC (Windows 11 build 26300, Microsoft Edge 157.0.4341.60, Ubuntu 26.04.1 on WSL2 x86_64).

- Host tests **16/16**, schema checks **OK** (3 fixtures, 2 disposable saves, 6 negative controls) on WSL2.
- Real Windows Edge, real OS mouse and keyboard: work flow, three saves through the Save button, host-process restart, page reload, load, integrity comparison, guards. **No unexpected result.** One state-comparison row differed; it is the documented Core rule, explained in §5.
- **Not proven here (see §9):** a *real* post-commit list failure inside the live Edge UI (it cannot be triggered on demand through the UI); `fsync`/power-loss durability; native Windows; Core 82/82 re-run; the independent re-execution of the author's native-Linux numbers.

I did not self-approve Manager acceptance, integration, or P1 base selection.

## 1. Worker and environment (actual)

| Item | Value |
|---|---|
| Worker | Claude Sonnet 5.5 (`claude-sonnet-5-5`), T3 Code / Claude Code harness, local Windows + WSL2 access, owner's own Claude account. Effort: harness numeric reasoning-effort value 10; no named Medium/High selector observed. Not the #567 author (Opus 5.5) or #586 reviewer (GPT-6). |
| Branch | `verify/multi-sitka-p0-567-wsl2-edge-targeted-20261116-v1` |
| Worktree (new, WSL ext4) | `/home/jer/ccf-worktrees/multi-p0-567-wsl2-qualification` (store `/home/jer/Development/CCF-HP`) |
| Starting HEAD | `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`; `git diff` vs the source candidate empty; 0 dirty files |
| `origin/main` at start | `375c40f009616d309d633e9c31fb18a9cd1dec25`; `active-tasks.md` had no #589/#567/#586 entry |
| Windows | Windows 11 Pro 10.0.26300; WSL 3.0.5.0, WSLg 1.0.79, Windows version 10.0.26300.9974 |
| WSL | distro `Ubuntu`, **VERSION 2**, Ubuntu 26.04.1 LTS, kernel `6.18.58.1-microsoft-standard-WSL2` x86_64, worktree on **ext4** (`/dev/sdd`) |
| SDK | per-shell `DOTNET_ROOT=$HOME/.dotnet`, `PATH=$HOME/.dotnet:$PATH`; `dotnet --version` = **8.0.134** (`global.json` 8.0.133 + `rollForward: latestPatch`, not relaxed); runtimes Microsoft.NETCore.App / AspNetCore.App 8.0.34 |
| Edge | **157.0.4341.60** (`Edg/157.0.4341.60`), real Windows Edge in an isolated throwaway profile, remote-debugging used only to *read* page state and to install a passive response recorder; **all interactions were real OS mouse/keyboard input** |
| Constraints kept | no sudo, apt, global install, shell-profile edit, Unity, game build, or WSL shutdown/restart |

## 2. Source integrity and write scope

- Source bytes unchanged: tracked `git status` empty after every phase (§8).
- Build output only in this worktree's gitignored `bin/`/`obj/`; disposable synthetic saves only in this worktree's gitignored `.lab-data/`. No user save, CCF save, Unity file, or other worktree touched. The earlier `CCF-MULTI-008-SitkaLab-WSL2-20261114` worktree was not touched.
- **This handoff is the only tracked file added.**

## 3. Host tests and schema (matrix item 2)

`cd Tools/MultiRackSitkaLab && dotnet run --project tests/Host.Tests` → **`16 passed, 0 failed`** (exit 0, 12.3 s wall including build). Tests that matter for #567:

| Test | Result |
|---|---|
| `SaveThatCommits_ThenMeetsASwappedLabFolder_Reports200WithTheFile_AndSaysTheListWasNotRefreshed` | PASS |
| `SaveThatCommits_ThenCannotListForAnUnexpectedIoReason_StillReports200_WithAGenericListCode` | PASS |
| `SaveThatMeetsASwappedLabFolderBeforeCommit_StillFails403_AndWritesNothingAnywhere` | PASS |
| `SavesDuringAConcurrentLabFolderExchange_…_AndReportHonestly` | PASS — `swaps=887138 responses=403:SymlinkRejected=3165,200:ok=327 alternate-overwrites=0 (reported 200: 0) 200-with-list-not-refreshed=226` |
| `Hostile_HostOriginMethodAndHeaderCases_AreRefused`, `Host_ListensOnLoopbackOnly`, `RequestBodies_AreBoundedAndValidated`, `RefusalCodes_MatchTheirHttpStatus`, `Responses_CannotReadUnrelatedFiles_…`, `FifoInTheLabFolder_…`, `SaveOntoAFolderOrFifo_…`, `SaveThatCannotBeWritten_…`, `Saves_RoundTripOverHttp_…`, `HttpWorkflow_MarkToExecute_…`, UI/source static checks (3) | PASS |

Accuracy note on what these exercise: the two deterministic post-commit tests wrap the **production** `RealLabFileAccess` in a test-only `HookedFileAccess` whose callback swaps the folder *after* the real `renameat` (the injection is test-only and not reachable from HTTP or `Program`). The 887,138-swap stress test uses the **default production file layer without injection** on this WSL2 ext4; its 226 `200`-with-`listRefreshed:false` outcomes are real production-path occurrences of the new behaviour (previously false errors), with zero alternate-folder writes.

`python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json` → OK ×3 fixtures + 6 negative controls rejected, exit 0. The same check passed on both of my Edge-created saves. Core 82/82 was **not** re-run (no Host failure suggested wider impact; Core is byte-identical to the previously qualified baseline per the packet).

## 4. Live Host and real Edge (matrix item 3)

**Host:** one own process, started exactly as the README recipe: `dotnet run --project src/Host` → `Listening on http://127.0.0.1:5187`.
- Linux listener: `LISTEN 127.0.0.1:5187` only (pid 447); no wildcard bind.
- Windows listener on 5187: `127.0.0.1` owned by `wslrelay` only. Windows → both non-loopback Windows IPv4 addresses and → the WSL VM address on :5187 were **unreachable** (`curl` `000`).
- `/api/health` → `{"status":"ok","bind":"127.0.0.1","port":5187,"banner":"SYNTHETIC TEST FOREST"}`.

**Synthetic work in the Edge UI** (fixture `sitka-flat-demo-v1`, reset through the UI): select `T0062` → mark → create plan `P0001` → propose → approve → queue → execute (→ `felled`); select `T0017` → mark → create plan `P0002` → propose → approve (left **APPROVED**). Pre-save Core: events 10, rev 1, N=137, BA 8.88468556194034 m², felled `T0062`, plans `P0001 EXECUTED`, `P0002 APPROVED`.

**Revised save response — what Edge actually received** (recorded by a passive in-page `fetch` wrapper; also confirmed on disk):

| Save (real Save button) | HTTP | Body received |
|---|---|---|
| 1. `p589-primary` (new) | 200 | `{"ok":true,"code":"ok","message":"saved p589-primary.sitkalab.json","fileName":"p589-primary.sitkalab.json","files":["p589-primary.sitkalab.json"],"listRefreshed":true,"listCode":null}` |
| 2. `p589-primary` again (rename over existing) | 200 | same body; list contains the name **once** |
| 3. `p589-secondary` | 200 | `…"fileName":"p589-secondary.sitkalab.json","files":["p589-primary.sitkalab.json","p589-secondary.sitkalab.json"],"listRefreshed":true,"listCode":null}` |

UI after save 1: message `OK: saved p589-primary.sitkalab.json`, class `message ok`, `code=ok`; the Lab-file dropdown gained the file; Core state hash and event count unchanged by the save; **Verify event replay** → `Replay OK: 10 events re-derived from the fixture; final SHA-256 537ef81fe4… matches the current state.`

**On disk (WSL):** two `…sitkalab.json` files, mode `0600`, 1 link each, 29,687 bytes, **0** leftover temp files; the two files are byte-identical (`sha256 08222d29…ef0f8`) because they hold the same state (the file does not embed its name). Header: `{"format":"ccf-sitka-lab","labSchemaVersion":1,"sourceClass":"Synthetic","fixtureId":"sitka-flat-demo-v1",…`.

**Host-process restart (not the distro):** stopped only my own listener (pid 447; guarded by recorded wrapper pid and a `cwd` check in my worktree) → port 5187 free and Windows `curl` `000` → started a second Host (pid 741). Distro stayed `Running` throughout. New process: fresh in-memory default state (0 events, 0 felled, hash `b407c725e655…`) while `GET /api/saves` still listed both files from disk.

**Reload + load in real Edge:** real F5 (page `timeOrigin` changed); the dropdown listed both files; I chose `p589-primary` with the real keyboard and clicked **Load selected** with the real mouse. UI: `OK: loaded; approved and pending plans were made STALE`. Core comparison, saved state vs after restart+load:

| Property | Pre-save | After load |
|---|---|---|
| fixtureId / experimentStep / forestRevision | sitka-flat-demo-v1 / 0 / 1 | same |
| eligible N / BA m² / BA/ha / Dq cm | 137 / 8.88468556194034 / 15.285480536671553 / 28.735304717825375 | identical |
| felled count / ids | 1 / `T0062` | identical |
| plan ids | `P0001`, `P0002` | identical |
| plan states | `P0001 EXECUTED`, `P0002 APPROVED` | `P0001 EXECUTED`, `P0002 STALE` (**stale-approval rule PASS**) |
| marked trees | `T0017:PLAYER_MARKED` | none — see §5 |
| event count / state hash | 10 / `537ef81fe478da0c…` | 11 / `92de403bd647f1eb…` (the load appends a recorded `PlanStale` event) |

Clicking **Execute** on the now-STALE `P0002` refused: `Refused [PlanStale]: plan P0002 is STALE and can no longer be executed — no tree was felled (audit-only event recorded).` Felled stayed 1; events 11→12. **Verify event replay** → `Replay OK: 12 events re-derived from the fixture; final SHA-256 92de403bd6… matches the current state.`

## 4a. Representative guards and the UI's handling of `files:null` (matrix item 4)

| Check | Result |
|---|---|
| **Real Edge UI**, save name `../evil` typed in the Save box | HTTP 400, `{"ok":false,"code":"PathRejected","message":"save name must be 1-64 characters of [A-Za-z0-9_-]",…}`; **no** `fileName`/`files`/`listRefreshed` claimed; UI `Refused [PathRejected]: …` class `message err`; list unchanged; nothing on disk |
| **Real Edge**, `http://localhost:5187/` typed in a new tab | status **403**, body `{"ok":false,"code":"Forbidden","message":"unexpected Host header"}` (screenshot D2) |
| Real Windows `curl.exe` → WSL Host: `GET /api/saves` | 200 |
| `Host: localhost:5187` / `Host: evil.example` | 403 / 403 |
| POST save with `Origin: http://evil.example`; with no Origin; with Origin ok but no `X-Sitka-Lab` | 403 / 403 / 403 (`{"ok":false,"code":"Forbidden","message":"unexpected Origin"}` for the first) |
| POST save `../../escape` (valid headers); POST load `../../etc/passwd` | 400 `PathRejected` / 403 |
| `PUT /api/saves` | 405 |
| After all refusals | state hash unchanged; lab-data hashes identical to right after the saves; nothing named `*evil*`, `*escape*`, `*guard-test*` anywhere under the worktree or its parent |

JSON bodies for `curl.exe` were sent from files (`-d @file`); Windows PowerShell 5.1 strips embedded quotes from inline arguments, which would otherwise let a 400 come from malformed JSON instead of the name check.

**UI handling of the post-commit "list not refreshed" shape — SIMULATED, not a real host fault.** A live post-commit failure needs a same-user folder swap in a microsecond window, so it cannot be triggered on demand through the UI. In real Edge I overrode the page's `fetch` for one request and answered the real Save click with exactly the new shape (200, `ok:true`, `files:null`, `listRefreshed:false`, `listCode:"SymlinkRejected"`, message `saved p589-primary.sitkalab.json; the lab file list could not be refreshed (SymlinkRejected)`). The UI showed `OK: saved …; the lab file list could not be refreshed (SymlinkRejected)` with class `message ok` (**not** a failed save), and the Lab-file list was **retained** (`files:null` does not blank it). As a contrast, a simulated pre-commit 403 showed `Refused [SymlinkRejected]: …` with class `message err`. The simulated requests never reached the Host (no file written); the override was removed afterwards. This verifies the browser code against the response contract; the Host side of the same contract is evidenced by §3.

## 5. The one differing comparison row (explained, not waved away)

My comparison flagged `marked trees`: saved `T0017:PLAYER_MARKED`, after load none. I checked rather than assumed:
- the **saved file bytes** hold `T0017` `markState: PLAYER_MARKED` and `P0002` `APPROVED` (verified by reading the JSON) — persistence is correct;
- on load, Core applies a recorded `PlanStale` event; `schemas/lab-file-v1.md:60` states `APPROVED/PENDING_WORK → STALE on revision change, calendar step, or load — none, trees released`.

So the mark is released from the stale plan by the documented design. #567 changed neither Core nor the schema. I record it as a documented behaviour, not a defect; my own check script labelled the row `DIFFERENT` and is reported here unedited.

## 6. Baseline #514 comparison

Same machine class and same recipe as the accepted #514 qualification (Ubuntu x86_64 on WSL2, user-local SDK 8, Edge, `127.0.0.1:5187`, real mouse/keyboard, host-process restart then load), with the previous results re-used only as context. Observed deltas: Edge 157.0.4341.60 (#514: 157.0.4341.46), SDK 8.0.134, and the new #567 save-response fields. **#514's 63/63 matrix, 35+6 mutation controls and 2,000-tree timing were not repeated, and nothing here claims to re-verify them.**

## 7. Harness honesty (no waivers; all misses disclosed)

My Edge harness is a derived copy of the #514 PowerShell/CDP harness (paths and profile name changed, plus new steps). Faults were all in the harness, not the product:
1. **Attempt 1** of the save step: a PowerShell 5.1 `ConvertFrom-Json` array-count quirk made my wait loop miss a save that **had** succeeded (Edge had received the correct 200 body). I stopped, copied that stray file to `03-aborted-first-attempt/`, deleted it from my own lab folder, and re-ran.
2. **Attempt 2**: the click for Save 3 landed on the map after a layout shift (page log `pointerdown:map`); saves 1–2 had been correct. I made the helper require the Save request itself and retry, deleted my stray file, and re-ran from an empty folder.
3. **Attempt 3** (the evidence reported above) ran clean from an empty `.lab-data`; harness click retries: 1 (the *Confirm reset* click first landed on `MAIN`, retried OK), 0 in the load and guard phases.
4. The second F5 before attempt 3 did not actually reload the page, so the recorder file `A-browser-received-save-responses.json` also holds the two POSTs from attempt 2; **the last three entries are authoritative**.
5. `cmd.exe /c` was mis-invoked once through Git Bash path conversion and left an interactive shell; I stopped that task. One empty cache-index stub of the throwaway Edge profile could not be deleted (no owning process), inside my own scratch folder.

## 8. Cleanup and unrelated work preserved

- Stopped only my own Host (pid-recorded) and only the 10 `msedge.exe` processes whose command line carried my throwaway profile path; the user's **22** other Edge processes were untouched (32 → 22). Debug port 9222 closed; 5187 free in WSL and from Windows; no host/`dotnet run` processes left.
- **WSL distro:** it was `Stopped` when I arrived; I started it by running commands in it and held one `wsl.exe sleep` keep-alive session so it could not idle out mid-test (my own session, stopped at the end). I never ran `wsl --terminate/--shutdown`. After my session ends it may idle-stop on its own, as it was found.
- The earlier MULTI-008 worktree, the user's files, saves, other WSL processes and the primary Windows checkout were not touched. **My Edge harness took the physical mouse and keyboard for roughly seven minutes.**
- Disposable saves in my worktree's `.lab-data/` were copied into the evidence directory and then the whole `.lab-data/` folder (created by my own first Host run) was removed. Ignored `bin/`/`obj/` build output remains in that worktree only.
- Final `git status` in the worktree: tracked diff = this one added Markdown only.

## 9. Not done / limits (what this does and does not prove)

- No real post-commit list failure was induced in the live Edge UI (§4a is simulated); the production-path evidence for that is the in-process Host stress/injection tests (§3).
- No `fsync`/power-loss test; "saved" still means *rename committed*. Not re-run: Core 82/82, the 63/63 security matrix, the 35+6 mutation controls, 2,000-tree timing, native Linux author runs, #586's static assertions. No Windows game build, Unity, native Windows save claim, Chrome, arm64, or WSL distro restart.
- The `files`/`listRefreshed` contract was observed only for the Edge UI and `curl.exe`; no other client was tested.
- Known #586 minors (S-1 test-only provider callback, S-2 same-user path exchange identity, S-3 generic list code) are untouched and remain documented non-blocking limitations.
- Dashboard / Drive synchronisation: **NOT VERIFIED** (not checked).

## 10. Evidence locations (not committed)

External (WSL ext4): `/home/jer/multi589-evidence/` — 39 files, 1.4 MB, `MANIFEST.sha256` (sha256 per file):
`01-identity-tests/` (identity, `host-tests.log`, `schema-check.log`) · `02-live-host/` (host stdout run1/run2 + wrapper pids) · `03-aborted-first-attempt/` (stray save, §7) · `04-disk-after-saves/` (listing, sha256, copies of both saves) · `05-final-checks/` · `06-windows-edge/{screenshots,receipts,harness}` — screenshots A1–A3 (saves, replay), C1–C2 (after restart; after load with felled stump and STALE plan), D1 (bad name refused), D2 (`localhost` 403), D4 (simulated post-commit-list-failure UI); receipts include pre-save/post-load Core JSON and browser-received responses; harness scripts included for inspection. Screenshots show only the Edge window with synthetic data.

## IMPLEMENTATION HANDOFF

```text
Task: CCF #589 — targeted Windows Edge / Ubuntu WSL2 delta regression of #567
Worker: Claude Sonnet 5.5 (claude-sonnet-5-5), T3 Code / Claude Code harness; owner's PC
Role: Primary (verification), no source changes
Branch: verify/multi-sitka-p0-567-wsl2-edge-targeted-20261116-v1
Base: a7f41983c2f2f233b8fe999d1a5b10e72bdd2866 (source candidate, unchanged)
Context commit: per the #586 packet (Scenario One canonical context not involved)
HEAD: <the commit adding this file; see the issue comment>
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-567-Targeted-WSL2-Edge-Regression-Result-20261116.md
GITHUB HANDOFF COMMIT: <see the issue comment>
EXTERNAL EVIDENCE ARCHIVE: /home/jer/multi589-evidence/ (WSL), MANIFEST.sha256
WHAT CHANGED: one added Markdown (this file). No source, fixture, schema, UI, README or Core change.
FILES CHANGED: this Markdown only
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none (standalone lab verification)
SIMULATION / SAVE IMPACT: none to repo; disposable synthetic saves only, removed
MUST-NOT-CHANGE HASHES / ANCHORS: source bytes unchanged (tracked diff vs candidate empty)
INTENTIONALLY CHANGED HASHES / ANCHORS: none
VERIFICATION RUN: Host tests 16/16; schema check; live host loopback checks; real Edge work/save/restart/reload/load/replay/guards; simulated-reply UI check
VERIFICATION RESULTS: all as in §3–§5; one documented-by-design mark-release difference (§5)
SPEC CONFORMANCE: not applicable (PACKET-level verification)
REMAINING VERIFICATION GATES: Manager acceptance of platform delta; separate policies for fsync and portability; P1 base selection (Manager)
KNOWN LIMITATIONS: §9
UNRELATED WORK PRESERVED: yes (§8)
DECISIONS NEEDED: Manager: accept/decline the platform delta and decide the P0 re-pin
WORKER ASSESSMENT: Ready for review: Yes
RECOMMENDED NEXT STEP: Manager reviews; if accepted, mark #567 candidate target-platform verified; P1 approval remains separate
```
