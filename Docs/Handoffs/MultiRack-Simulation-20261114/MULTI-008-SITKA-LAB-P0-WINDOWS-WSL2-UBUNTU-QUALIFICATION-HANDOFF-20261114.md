# MULTI-008 — Sitka 2D Lab P0 on Windows through the owner's Ubuntu WSL2: qualification handoff

**Claim:** [CCF #514](https://github.com/jermur788/CCF/issues/514) · **Packet:** [MULTI-008 compatibility qualification packet](https://github.com/jermur788/CCF/blob/docs/multi-008-windows-wsl2-ubuntu-qualification-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-UBUNTU-WSL2-COMPATIBILITY-QUALIFICATION-PACKET-20261114.md)
**Date:** 14 November 2026 (test window 22:35–23:04 UTC) · **Scope:** standalone Sitka 2D lab only — **NO BIOLOGICAL GROWTH**, no Unity, no Windows game build, no P1, no production saves.

## Verdict: **WSL WINDOWS-USABLE PASS** (with the stated conditions and limits)

The lab, built and hosted inside the owner's existing **Ubuntu (WSL2)**, was driven from a **real Microsoft Edge on the owner's Windows PC** at `http://127.0.0.1:5187/` and passed the full tree-management workflow, save/load/replay across a host restart, the security checks and the 2,000-tree performance check. **No source file was changed.** Conditions, none hidden:

1. **A .NET SDK 8 was not present and was installed with the owner's approval** (user-local, `~/.dotnet`, no sudo, no system change). `global.json` pins SDK 8.0.133 (`latestPatch`) and Ubuntu 26.04 here had only SDK/runtime 10.0.x, so `dotnet build` could not start before that. See §7 for what the Manager should decide about it.
2. **A restart of the WSL distro itself was NOT TESTED** (an idle interactive shell belonging to the user was open; I did not terminate their distro unasked). Host-process restart persistence **was** tested.
3. **Edge only**; Chrome was not tested. One physical Windows machine. Linux arm64 and native Windows remain untested/unsupported as before.
4. Independent confirmation is **not** needed for source (there is no delta), but this is a single-worker qualification; the Manager may still want a second look at the evidence.

## 1. Environment, ownership and dependency

| Item | Actual value (read live) |
|---|---|
| Windows machine | `DESKTOP-DL1BQI7`, OS build **10.0.26300** x64 (reported product name "Windows 10 Pro"; WSL reports Windows 10.0.26300.9974), primary screen 1920×1080 @100% |
| WSL | WSL **3.0.5.0**, kernel **6.18.58.1-microsoft-standard-WSL2**; Windows PowerShell `wsl.exe --list --verbose`: **`Ubuntu`, Running, VERSION 2** (default distro; WSL1 unsupported on this machine) |
| Ubuntu | **26.04.1 LTS (resolute)**, x86_64, glibc 2.43, root fs **ext4** (`/dev/sdd`, 927 GB free), systemd on, `/proc/self` present; `/etc/wsl.conf` = systemd + default user only; **no** `.wslconfig` (read-only inspection, nothing changed) |
| Toolchain | system SDK 10.0.115 / runtime 10.0.15 (cannot satisfy `global.json`); **SDK 8.0.134 / ASP.NET+NETCore 8.0.34 installed user-local** (§7), Python 3.14.4, git 2.53.0 |
| Windows browser | **Microsoft Edge 157.0.4341.46** (Windows), launched with an isolated throwaway profile (deleted afterwards) |
| Worker | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code inside local T3 Code on this PC |
| Worktree / branch | `/home/jer/ccf-worktrees/CCF-MULTI-008-SitkaLab-WSL2-20261114` (new, ext4, not `/mnt/c`), `feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1` |
| Starting / source HEAD | **`c445f8700610a7e6a835fef9ecec39eb9432128d`** (= accepted MULTI-006 candidate), clean at start (0 changes) and clean at end |
| Dependency | #511 closed HANDED-OFF **APPROVED** and Manager-accepted (22:05Z); #514 released READY. Registered **[RUNNING]** on #514 **before any test**: [comment](https://github.com/jermur788/CCF/issues/514#issuecomment-6885316823) |
| Other lanes on this host | board shows #461 Samsara2 research in a different worktree; no overlapping files/branch, no Unity process; no conflicting lock found |

Owner approvals used: user-local SDK 8 install (asked and approved in-session). **Not** done: any `wsl --set-version`, `.wslconfig`, firewall, default-distro, networking or Windows setting change; no `sudo`; no apt/OS package change.

## 2. Results (actual PASS / FAIL / NOT TESTED)

### A. Ubuntu Core/Host — **PASS** (reproduces the writer baseline)

```bash
cd Tools/MultiRackSitkaLab
dotnet build MultiRackSitkaLab.sln --no-incremental                 # 0 warnings, 0 errors
dotnet run --no-build --project tests/Core.Tests                    # 82 passed, 0 failed
dotnet run --no-build --project tests/Host.Tests                    # 13 passed, 0 failed
(cd fixtures/synthetic && sha256sum -c MANIFEST.sha256)             # 3/3 OK
python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json   # OK (+6 negative controls rejected)
```

| Check | Result |
|---|---|
| Clean build | PASS (0 warn / 0 err, with SDK 8.0.134) |
| Core 82 / Host 13 | **PASS / PASS** (82/82, 13/13) |
| Fixtures 3/3, schema | PASS / PASS |
| 35 mutation controls | NOT RERUN (packet allows this: same candidate passed #511, no source change) |
| Headless Chromium Tier-2 UI tests | NOT RUN (no Chromium in this Ubuntu; replaced by the real Windows Edge run below, which is the acceptance criterion anyway) |

Host.Tests' folder-exchange race printed `alternate-overwrites=0 (reported 200: 0)` with ~3,500 `403 SymlinkRejected` — that is the known minor **R-3** ([#517](https://github.com/jermur788/CCF/issues/517), zero false successes), unchanged and not a WSL difference.

### B. WSL safe persistence — **PASS** (live API against the running host, 63/63 checks)

Script and raw console are preserved (`B-persistence-security/`). Highlights, all on the live host at `127.0.0.1:5187`:

- New save → **HTTP 200, no 501**: the `openat`/`statx` handle-anchored transaction works on the WSL2 kernel.
- Save file lands only in `Tools/MultiRackSitkaLab/.lab-data/` on **ext4**, mode **0600**, link count 1; nothing appears anywhere under `/mnt/c`; no `*.sitkalab.json`/`.lab-data` found on `C:`.
- list → reload → replay canonical hash `matchesCurrentState` → overwrite (content changes, no temp left) → reload → replay: PASS.
- Approved plan saved then reloaded is explicitly **STALE**; Execute on it → `409 PlanStale`, nothing felled; cancel handled.
- Name/path attacks refused (`../escape`, `a/b`, `..`, `..\win`, `/etc/passwd`, NUL, 200-char, `con:`, `%2e%2e%2f`, empty): 400 `PathRejected`; load of `../x`, `/etc/passwd`, unlisted names: 403.
- Planted **symlink → outside decoy**: save refused 403 `SymlinkRejected`, decoy hash unchanged; load refused; symlinked name not offered in the list. **FIFO**: load refused in 0.00 s (host not hung), save → 409 `NotRegularFile`. **Hard link** refused. Malformed / tampered-content / 12 MB files refused (400). Planted items removed afterwards; no leftovers.
- Hostile requests (Host `localhost`/`evil.example`/wrong port, foreign Origin, POST without Origin or `X-Sitka-Lab`, wrong content type, `Sec-Fetch-Site: cross-site`, PUT/DELETE, raw and encoded path traversal): all refused (403/415/405/404); lab state unchanged.
- No file written outside `.lab-data` (find-after-marker), host process has no non-loopback connections, UI source references no external host (only the SVG XML namespace constant).
- **Host process restart** (stop → start): see C-4. **WSL distro restart: NOT TESTED** (see condition 2; steps in §6).

### C. Windows Edge ↔ Ubuntu host — **PASS**

Bind: Ubuntu `ss -ltnp` shows **only `127.0.0.1:5187`** (no `0.0.0.0`, no `[::]`); health `bind":"127.0.0.1"`. On Windows the only listener on 5187 is **`wslrelay.exe`** on **127.0.0.1** (WSL's localhost forwarding), and Windows `curl.exe http://127.0.0.1:5187/api/health` → 200.

Input method (be exact): Edge was launched visibly with an isolated temp profile; **all interactions were real Windows OS input** (`SetCursorPos` + `mouse_event` clicks/wheel/drag, `SendKeys` typing, real F5), positioned using page geometry; the DevTools protocol was used **only to read state, evaluate read-only checks and compare to Core**, never to click. The calibration check showed the page's own cursor-readout reacting to the real mouse (48.11 m, 37.13 m at plot centre). The harness verified each click's post-condition in Core and counted retries: **one** OS click was lost before the page received any pointer event and was retried; every other action took effect on the first applied click. **Disclosure:** in my first two or three attempts, before the harness checked post-conditions or logged page events, a few clicks (tree select, "Create plan", the reset confirm) appeared not to act; I aborted and redid those runs and could not prove the cause (the logged evidence of the later runs points to input-delivery timing in my harness, since every click the page received acted correctly and no instrumented click reached the page without acting). Those aborted runs are not part of the evidence above, and I did not see an app-side dropped-handler in ~60 instrumented clicks — but treat "no intermittent click loss in the app" as observed, not proven.

| # | Packet item | Result | Evidence |
|---|---|---|---|
| 1 | 162-tree stand + banner **SYNTHETIC TEST FOREST — NO BIOLOGICAL GROWTH** | **PASS** (162 glyphs, N 138, area 5812.5 m², both banners) | shot 01, 02 |
| 2 | Select a real tree → inspector matches Core; clear; zoom/pan | **PASS** — T0062: Core x 39 / y 27 / DBH 23.1 cm / h 13.775 m = inspector (`x 39.000 m, y 27.000 m`, `23.1 cm`); preview BA −0.0419 m² = π(0.1155)² by hand; Clear empties inspector; wheel ×3 scale bar 20 m→10 m; drag pans; Fit restores | 02, 02b, 03 |
| 3 | Mark → propose → approve → queue → execute | **PASS** — after Mark/Create/Propose/Approve/Queue Core stayed **felled=0, N=138, BA 8.926595**; Execute: T0062 felled, **N 138→137, BA 8.884686** (hand check 8.926595−π·0.1155² = 8.884686), plan EXECUTED; 6 chained events; **2nd Execute = audit-only `PlanAlreadyExecuted`**, nothing more felled | 04, 05, 05b |
| 4 | Save, restart, load, replay; expected staleness explicit | **PASS** — typed name with real keyboard, saved `win-edge-session` (ext4, 0600, snapshotHash c1b54c39…); **host stopped and restarted**, real F5: fresh session empty (hash b407c725…); load restored **hash c1b54c39…, events 7, felled 1, N 137**; *Verify event replay*: "Replay OK: 7 events … final SHA-256 c1b54c3930… matches". Approved plan reloaded as **STALE**; Queue on it refused | 06, 07, 07b, 08 |
| 5 | Invalid file / hostile Host/Origin; no external requests, no LAN listener, no hidden writes | **PASS** — malformed lab file via UI → `Refused [WrongFormat]`, state unchanged; from Windows `curl.exe`: Host `localhost`/`evil.example` 403, foreign Origin 403, no Origin 403, no CSRF header 403, traversal name 400, raw traversal 404, PUT 405; **Windows → LAN `192.168.68.115`, vEthernet `172.20.176.1`, and WSL VM `172.20.190.222` on :5187 all unreachable**; Edge's resource records show only `127.0.0.1:5187` | 10 |
| 6 | Advance experiment step | **PASS** — step 0→1, forestRevision 0→1, event `ExperimentStepAdvanced`; **all 162 trees' DBH/height/age/status and all stand metrics unchanged**; STALE plan stays STALE | 09 |
| 7 | Genuine Windows screenshots + host stdout + versions | **PASS** — 17 full-window desktop screenshots (Edge chrome visible), host stdout, versions above; narrow window (≈490 px) also works with no horizontal page scroll | 13, 13b |

`http://localhost:5187/` is refused with **HTTP 403** by the Host guard, exactly as designed; only the literal `127.0.0.1` is the supported URL (see §6).

### D. 2,000-tree performance on the Windows browser — **PASS**

`sitka-flat-perf-2000-v1` loaded through the UI (2,000 glyphs in the DOM, banner correct). **30 real mouse clicks** on trees spread across the stand, plus 5 keyboard select-by-ID repetitions. Metric: the page's own selection → inspector → Core-preview time (`#perf-line`), on real Windows Edge:

| Samples | min | median | p95 | **max** | Target |
|---|---|---|---|---|---|
| 30 clicks | 44 ms | 51.3 ms | 59.8 ms | **62 ms** | ≤ 2000 ms |
| 5 ID-box selects | 41 ms | — | — | **60.9 ms** | ≤ 2000 ms |

Edge's network records for `POST /api/preview` (resource timing, n=30): **1.8–2.8 ms**. The inspector matched Core for all 30. This is a Windows result; the earlier laptop Chromium figure was **not** substituted. **Observation (not a failure, not Windows-specific):** at fit zoom the 2,000-tree stand's hit-targets overlap, so in 4 of 30 clicks the glyph on top (a neighbour) was selected instead; the inspector and preview correctly described the tree that was actually selected, and zooming in avoids it.

## 3. Supported route

**Windows browser (Edge verified; Chrome not tested) → Ubuntu WSL2 host on `http://127.0.0.1:5187/`** (WSL localhost forwarding via `wslrelay.exe`, no setting changed). **Not supported / not tested:** native Windows .NET (lab saves fail closed HTTP 501 by design, unchanged); Linux arm64 (untested); WSL1 (not available here); distro-restart persistence (not tested); Chrome on Windows (not tested). Why literal `127.0.0.1`: the Host guard requires the host header `127.0.0.1:<port>`; `localhost` is refused with 403 on purpose and is **not** weakened. Windows still reaches the guest at the IPv4 literal because WSL forwards Windows loopback to the guest's loopback; the guest binds only its own loopback, so the LAN/VM addresses cannot connect (shown above).

## 4. Changes made

- **Source / docs in the repo: none, except this one new Markdown handoff.** Worktree at the end: HEAD `c445f870…`, **0 tracked changes, 0 untracked, 0 commits ahead** of the candidate before this report commit; nothing under `Assets/`, `Packages/`, `ProjectSettings/`, `specs/`; main checkout `CCF-HP` untouched (0 status lines; only a `git fetch` of this branch and the new worktree registration).
- **Machine changes (approved):** user-local .NET SDK 8.0.134 in `~/.dotnet` (installer script from `https://dot.net/v1/dotnet-install.sh`, sha256 `082f7685e156738a1b2e2ed8381a621870d4ce8e8c59278034556f05c186eb2e`, reviewed first: MIT, Microsoft hosts only, no sudo). `~/.bashrc`/PATH **not** edited. Evidence directory `~/multi008-evidence/` created.
- **Cleaned up:** my host processes stopped by PID; my throwaway Edge profile and debug port closed/deleted (the owner's other 19 Edge processes untouched); planted test links/FIFOs/bad files removed. Five synthetic saves remain in the lab's gitignored `.lab-data/` (`win-*`, `wsl-*`).
- **Source/no-Unity/no-production-save integrity:** no Unity process; no `forest-save.json` touched; no `/mnt/c` write by the lab; no real or archived forest save read or written.

## 5. Evidence (off-repo, preserved raw)

`/home/jer/multi008-evidence/` = `\\wsl.localhost\Ubuntu\home\jer\multi008-evidence\`, 53 files, **`MANIFEST.sha256` sha256 `592e7360b523c74de43a8275fd475670fb023d0bd5696179170861445f31571f`**.

| Folder | Contents |
|---|---|
| `A-linux-core-host/` | build, Core, Host, fixture, schema logs; `dotnet --info`; source HEAD |
| `B-persistence-security/` | `sectionB.py`, `sectionB-console.txt`, `sectionB-results.json` (63 checks), saved-file hash |
| `C-host/` | `ss-listeners.txt`, `health-inside.txt`, `host-stdout-run2.log` (raw), `host-stdout-run1-TRANSCRIBED-not-raw.txt` (my stop script overwrote the raw run-1 copy; the three lines were transcribed from console output and are labelled as such) |
| `D-windows-browser/` | `screenshots/` (17 PNG), `windows-environment.txt`, `perf-2000-windows-edge.csv`, `pre-save-state.json`, `step3.out`, and the PowerShell/bash `harness/` that drove the run |

Screenshot sha256 (first 16 hex): 01 `2e5a6128…` · 02 `9d345db1…` · 04 `c26e0c67…` · 05 `898cbbf0…` · 06 `ed1084ad…` · 07 `ff2342a4…` · 08 `a6ea5c83…` · 09 `2235de80…` · 10 `e5d974a4…` · 11 `6cb5405e…` · 12 `2557f4c8…` · 13 `7bb757ff…` (full list in the manifest). **Limit:** console transcripts of steps 4–8 and the Windows hostile-request run were printed to the session, not separately saved as raw files; their outcomes are reproduced in this report and the saved artefacts (screenshots, CSV, JSON, save files) corroborate them. Evidence is **not committed** to the repository (binary-footprint rule); it can be added to the branch if the Manager wants it tracked.

## 6. User-start recipe for the already-installed Ubuntu (no passwords, no system changes)

One-time, only if the Ubuntu has no SDK 8 (this one did not; the installer is the official Microsoft script, installs into your home folder, no `sudo`):

```bash
curl -fsSL https://dot.net/v1/dotnet-install.sh -o ~/dotnet-install.sh
bash ~/dotnet-install.sh --channel 8.0.1xx --install-dir "$HOME/.dotnet" --no-path
```

Each time:

```powershell
# Windows PowerShell
wsl.exe -d Ubuntu
```
```bash
# inside Ubuntu
export DOTNET_ROOT="$HOME/.dotnet" PATH="$HOME/.dotnet:$PATH"
cd ~/ccf-worktrees/CCF-MULTI-008-SitkaLab-WSL2-20261114/Tools/MultiRackSitkaLab   # or your own lab checkout
dotnet run --project src/Host
# prints: Listening on http://127.0.0.1:5187  (loopback only; Ctrl+C to stop)
```

Then open **Edge or Chrome on Windows** at **`http://127.0.0.1:5187/`** (not `localhost`). Stop with **Ctrl+C** in the Ubuntu window. Saves go to `…/Tools/MultiRackSitkaLab/.lab-data/` inside Ubuntu (synthetic only). To confirm the saves survive a WSL restart yourself: from PowerShell run `wsl.exe --terminate Ubuntu`, start again as above, open the lab and choose *Load selected*. If the port is busy: `dotnet run --project src/Host -- --port 5200` and use `http://127.0.0.1:5200/`.

## 7. Findings and decisions for the Manager (none blocking this verdict)

1. **SDK pin vs. Ubuntu 26.04 (real usability gap).** `global.json` = SDK 8.0.133 `latestPatch` and `net8.0`; a stock Ubuntu 26.04 here has only .NET 10, so `dotnet build/run` fails until SDK 8 is installed. Options: **(a)** README-only prerequisite + the install snippet above (allowed by the packet once ownership is established; I did **not** make it so this verdict stays a zero-change PASS) — proposed replacement for the README line *"Windows/WSL users run the same commands"*: *"Windows: supported route is a Windows browser at `http://127.0.0.1:5187/` talking to the lab running inside Ubuntu on **WSL2 (x86_64)**; install .NET SDK 8 (user-local is fine) and run the Quick-start commands inside Ubuntu. Native Windows/macOS cannot save lab files (HTTP 501 by design). Linux arm64 untested."*; **(b)** relax `global.json`/target framework — a source change needing the focused delta review, not done.
2. **`localhost` gives a bare 403.** Correct by design (Host guard), but a user who types `localhost` or follows autocomplete sees a refusal. A friendlier message would touch host source; not done and the guard must not be loosened.
3. **Selection at fit zoom on the 2,000-tree stand** can pick the neighbouring glyph (4/30 clicks). Cosmetic P0 observation; inspector/preview are correct for what was selected.
4. **R-3 (#517)** unchanged and non-blocking; **no change to #511 conclusions**. The persistent-folder-identity/ancestor-link limit remains a bounded same-user P0 limitation.
5. **WSL distro restart** persistence is the one packet item not exercised; run the three-line check in §6 or authorise me to run `wsl --terminate Ubuntu` (it would close any open Ubuntu terminals).
6. Dashboard/Drive sync **NOT VERIFIED**. No Unity, Windows game build, P1 ecology, real data, protected saves, approved spec or live-board edit.

## 8. Status

**WSL WINDOWS-USABLE PASS.** #514 to be marked HANDED-OFF after GitHub readback of this file; Manager and #511 notified there. No source delta, so no delta review is required for this verdict.
