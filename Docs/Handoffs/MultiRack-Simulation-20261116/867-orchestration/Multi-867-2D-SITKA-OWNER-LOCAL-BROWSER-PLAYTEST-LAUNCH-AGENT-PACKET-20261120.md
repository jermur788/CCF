# Multi #867 — launch the existing #849 standalone 2D Sitka simulation for owner playtesting

**Date:** 20 November 2026 UTC  
**Class:** PACKET ONLY — existing qualified standalone feature, no new workflow or source development  
**Difficulty:** LOW–MEDIUM local setup / browser qualification; no scientific research or expensive model needed  
**Manager:** [#863](https://github.com/jermur788/CCF/issues/863), OpenAI GPT-6, GitHub/browser-only  
**Execution/claim issue:** [#867](https://github.com/jermur788/CCF/issues/867) — **READY / NOT RUNNING**, no actual agent/physical worker registered  
**Preferred executor:** one available **Sol/Codex local-capable** agent with real authorised access to the owner's **DESKTOP-DL1BQI7 Windows + WSL2 Ubuntu**. GPT-6.1 Sol is suitable if genuinely available; any comparable local agent that can compile .NET 8 and launch a Windows browser suffices. Do not consume Claude Opus for this simple task. No false claim that this browser manager has access to that machine.  
**Owner instruction:** “Just create a handoff for an agent to do so” — permission to prepare this dispatch, **not a physical machine access grant or completed launch**.

## 1. Binding purpose: smallest genuine demonstration

**Get the owner into the ACTUAL existing #849 simulation in a Windows browser**, without asking them to run technical setup by hand. This is NOT development of a new 2D simulation.

The simulator already has one undivided **30 m × 30 m, 100 synthetic Sitka spruce** forest and real single-area gameplay:

- Select an exact tree ID on the map / table; inspect DBH, height and status.
- Enter the reason and **add to pending Work Plan**.
- **Approve whole plan; confirm zero trees fell.**
- **Execute approved plan** as a separate action, producing a real `fell (manual-work-plan)` event and a visible stump/stand metric change.
- **Advance Year +1** through the actual existing annual ecology from survivors and inspect history.

Owner should perform that interaction themselves; the launcher must **not pre-consume or mutate the owner's in-memory session**. Start from a fresh Year-0 session; automated read-only preflight is fine. The app has **no save, resume, costs or revenue, cross-area G1 comparator, racks, Unity, or Irish field forecast**. Exiting process loses this playtest state.

**SUCCESS** means: exact qualified source and pinned P0 verified, local server starts on `127.0.0.1` with its randomly printed one-time capability URL, the owner's Windows browser can load the real live page with 100 Sitka trees, and **owner receives the URL privately** to begin their playtest. Do not claim OWNER ACCEPTED until the owner actually reports a verdict; the agent can report ready-for-owner separately.

## 2. Exact authoritative source and scope

- **ONLY candidate source for this task:** [#849 exact Git commit `4c42e2012a4c1bee3a422e41a58535343d63ac9f`](https://github.com/jermur788/CCF/commit/4c42e2012a4c1bee3a422e41a58535343d63ac9f), feature branch `feat/multi-standmap-playable2d-20261120-v1`. **NOT on `main`**. Require exact full `git rev-parse HEAD` match; reject newer/older/different local copies, even if visually similar.
- [Exact source README (launch + UI instructions)](https://github.com/jermur788/CCF/blob/4c42e2012a4c1bee3a422e41a58535343d63ac9f/Tools/MultiRackSitkaP1Lab/README.md).
- [G2 Manager acceptance](https://github.com/jermur788/CCF/blob/docs/Multi-849-Manager-G2-Scope-Acceptance-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-849-Manager-G2-Source-and-Standalone-UI-Scope-Acceptance-20261120.md): SOURCE IMPLEMENTED, independent Tier2 REVIEWED, scoped G2 APPROVED, **not on main, not integrated/released, not owner accepted**.
- [Independent final reviewer result](https://github.com/jermur788/CCF/blob/e3c739ddfaf56ebb6b4f63bb6fe1d1f74d2af1f6/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-INDEPENDENT-TIER2-REPAIR2-REVIEW-20261120.md) — previously reported **92/92 Core; 137/137 UI smoke; 39/39 Edge** and ten frozen no-work digests. Those are **historic qualification evidence**, not claims about this new run.
- Live project coordination: [`Docs/Project/coordination/active-tasks.md`](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), freshly inspect before touching any local files. Historical #849 author/source custody has ended; **no new worker automatically inherits that physical lane**. Current `main` at packet creation `2e9fd8cb63a69adfbd76ced054c46e1a84b9412e` (NOT a source-launch ref and subject to change).

**Host/lane/claim (NOT yet physically granted):** target owner's **DESKTOP-DL1BQI7** using WSL2 Ubuntu, as in #849; **do not use jer-laptop** or its Scenario One/Unity worktree. Historical source writer used `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Source` and previously referenced a Windows T3 lane; these are **historical locators, not ownership or clean-checkout evidence**. Suggested *new* isolation lane if actually available and granted: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-867-OwnerPlaytest`; optionally a new local detached worktree rooted at the exact commit, with no branch/source commits. Do not create or write this proposed path until physical preflight and exclusive custody have succeeded. If a local existing pinned clean worktree can safely be reused by its currently authorised owner, avoid duplicate clone for speed. Agent identity, actual branch/worktree/host and any Ops reservation must be recorded on #867 before running.

## 3. Mandatory real-machine preflight and stop conditions

1. Register **actual provider/model, physical hostname + WSL identity, task #867, exact local source HEAD, status and source/build lane** as a `[RUNNING]` comment to #867 **only after** you actually have machine access and the claim is admissible. Check newest `main` coordination board plus **deployed physical Ops registry/leases/locks**, active processes, current Git index and dirty files **on the actual machine**, not a GitHub snapshot. Respect D-091; no lease force-release, arbitrary expiry, killing others or taking another worker's worktree. A 2D .NET/loopback launch **does not require Unity or its Editor lock** and must not take one. Coordinate if any physical host/source/build resource is concurrently occupied.
2. Keep all existing checkouts, including dirty ones, unchanged. If the exact source is only in another worker's dirty/claimed tree, **STOP that approach**; use a separately authorised clean clone/worktree instead. If no free writable build/evidence lane can be assigned, report HELD with the exact blocker. No changes to `main` or feature commit.
3. Require .NET 8 SDK/runtime on WSL. The standalone UI is **not included in the lab solution**, so build its `src/Ui/SitkaP1.Ui.csproj` directly. Use `TMPDIR` outside the repository; build outputs may be generated **only in the newly authorised disposable run checkout and external scratch**, never in the protected P0 lab.
4. **P0 immutable binary barrier:** before building, ensure `Tools/MultiRackSitkaP1Lab/.p0ref/` contains a **previously qualified READ-ONLY sourced** `SitkaLab.Core.dll`, `SitkaLab.Core.dll.sha256`, and `provenance.txt`. If missing, copy them **only from an already-authorised pinned read-only copy** as documented by source README, and compare all four evidence values to:
   ```
   9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
   ```
   The four are: `src/Core/SitkaP1.Core.csproj` `P0CoreExpectedSha256`, DLL SHA-256, matching hash recorded in `.sha256`, and `dllSha256=` in provenance. **Missing/mismatch -> STOP. NEVER run `build/build-p0-core-ref.sh`, change any hash, bypass the check or rebuild P0**.
5. No broad tests/repair detour by default: this is a **single hands-on launch**, not new Tier2 or security audit. If the exact candidate fails to build/serve, report the smallest reproducible blocker and stop that launch action. **No unrequested C#/script/fixture/source changes**, no new framework/network daemon, no package/version rewrites, no tests weakened for green, no G1 or save work, no main merge or Windows/Unity Player build.

## 4. Minimal launch commands — use only after passing §3

On **the qualified exact source checkout on DESKTOP-DL1BQI7 WSL2**, run from the repository root. `SRC` below is intentionally a placeholder: fill it **only with the verified authorised prebuilt P0 reference directory** and do not guess or fetch one from an unknown source.

```bash
# PRECONDITION: exact clean git HEAD 4c42e2012a4c1bee3a422e41a58535343d63ac9f,
# exclusive authorised disposable local build lane, already pinned P0 files present
cd Tools/MultiRackSitkaP1Lab
PIN=9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
# If .p0ref/ missing and authorised SRC is known, follow the exact README copy procedure;
# otherwise STOP. Do not rebuild P0.
test -f .p0ref/SitkaLab.Core.dll
test -f .p0ref/SitkaLab.Core.dll.sha256
test -f .p0ref/provenance.txt
test "$(sha256sum .p0ref/SitkaLab.Core.dll | cut -d' ' -f1)" = "$PIN"
test "$(cut -d' ' -f1 .p0ref/SitkaLab.Core.dll.sha256)" = "$PIN"
test "$(grep '^dllSha256=' .p0ref/provenance.txt | cut -d= -f2)" = "$PIN"
grep -F "<P0CoreExpectedSha256>$PIN</P0CoreExpectedSha256>" src/Core/SitkaP1.Core.csproj
# STOP if any of these tests fails. Do not chain without fail-fast.
```

Then, in a shell with fail-fast enabled and a fresh external scratch folder:

```bash
set -e
mkdir -p /tmp/ccf-multi-867-ui
export TMPDIR=/tmp/ccf-multi-867-ui
~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
~/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived
```

The running process prints:

```text
Synthetic 30 m Sitka experiment. Session only — nothing is saved.
Open http://127.0.0.1:<ephemeral-port>/s/<random-capability>/
Ctrl+C ends the session; nothing is saved.
```

Use the **exact URL printed at this launch**; `127.0.0.1` is deliberate, `localhost` is refused. Do not hardcode the port, use port 5187, replace the IP with a machine LAN IP, bind `0.0.0.0`, open router/firewall/public tunnel, disable Host/Origin guards or upload capability tokens to GitHub. If Windows→WSL loopback forwarding is unavailable, **STOP and report**; do not bypass security. The URL is a **live one-session bearer capability**. Supply it ONLY privately to the owner in the actual local-agent conversation or other explicitly approved local channel, and redact it from all screenshots, GitHub comments, logs and handoffs.

Read-only launch check: in the owner's Windows Edge/Chrome browser open the URL, confirm page loads and reports Year 0, 100 original trees, empty Work Plan, G1 disabled and finances unavailable. If an agent drives a browser to check it, use read-only inspection or a separate disposable session; **leave the owner's fresh Year-0 session unmodified**.

**Keep the server process alive for the owner's interactive playtest** using the agent's authorised terminal/session management; do not claim a background service exists without retaining the actual process/terminal. The owner will see an error if that process ends. After owner finishes, request/receive clear stop signal, send Ctrl+C, verify graceful exit and the in-memory session is gone. Do not terminate another process or force kill. If persistent hosting is unavailable, do not falsely promise the URL will stay usable: communicate the limitation and a command the owner or worker can use to relaunch in their own supervised terminal.

## 5. Owner's manual acceptance path, not an automatic test verdict

1. The owner opens the private URL in Windows Edge/Chrome and checks the full 30×30 map contains 100 Sitka trees at **Year 0**.
2. Owner clicks one individual tree, confirms exact tree ID, DBH and height in inspector.
3. Owner enters a meaningful felling reason and chooses `Add to Work Plan — no tree is felled`.
4. Owner chooses `Approve whole Work Plan — 0 trees felled`; verify living-tree count and crown still unchanged.
5. Owner chooses `Execute approved plan`, confirms correct tree ID and plan, sees actual Fell receipt/stump and change in living count/basal area.
6. Owner chooses `Advance one ecological year`, inspects survivor growth/mortality and history event; years and annual revision are real engine state, not a page animation.
7. Record **the owner's words**: PASS / ISSUE(s) / NOT TESTED, browser/device/zoom, any exact tree ID, screenshot where useful with capability/address bar redacted. Note no save/resume and G1 remains disabled. If an error occurs, preserve error verbatim and the smallest reproduction, but **do not unilaterally fix**.

An agent's own browser smoke or API tests **do not equal owner personal acceptance**. The older 92/137/39 counts remain historical evidence only.

## 6. Handoff and closure — preserve status distinctions

**Before owner opens UI:** post a concise **READY FOR OWNER PLAYTEST** status to #867 with actual commit hash, machine/lane identity, P0 hash 4-way, build result, loopback bind, initial 100-tree state, Windows browser access observed or NOT VERIFIED, whether process is still alive, and **how the owner got the URL privately**, but NEVER the token/port/capability URL. Keep issue OPEN.

**After test/stop:** publish **one new durable GitHub Markdown result** on a separate sole-writer docs branch under `Docs/Handoffs/MultiRack-Simulation-20261116/867-orchestration/`. Include exact source SHA, host/worker, physical admission and build/worktree lane, P0 pin provenance without secrets, commands/exit codes, browser read-only check, process lifecycle, user-observed steps with results, screenshots/evidence locations without bearer URL, error/reproduction/remaining blocks, no-source-diff proof. Verify readback and compare that docs branch to its starting main: only the new Markdown path. Post direct GitHub result URL to #867 and Manager [#863](https://github.com/jermur788/CCF/issues/863). **If an agent cannot publish, explain this explicitly and hand evidence privately to #863 for a separate authorised docs publisher.**

**Allowed labels:** source already IMPLEMENTED; prior scoped G2/independent REVIEWED; new `LAUNCHED` only after real local proof; `OWNER TESTED` only with owner report; `OWNER ACCEPTED` only with actual explicit owner acceptance. **NOT INTEGRATED / NOT RELEASED** remain unchanged.

**Do not independently edit** `Docs/Project/decision-log.md`, `current-milestone.md`, `game-brief.md`, `Docs/Project/coordination/active-tasks.md` or protected simulation sources. Manager handles any later canonical coordination change through the actual required authority and main CAS, not from this packet alone.

## 7. ONE paste-ready message for a separate local-capable agent

> **CCF Multi #867 — LOW–MEDIUM difficulty, single local operator, real owner #849 2D Sitka browser playtest launch; use local Sol/Codex agent capable of DESKTOP-DL1BQI7 WSL2.** **DIRECT BINDING GITHUB HANDOFF:** https://github.com/jermur788/CCF/blob/docs/multi-867-2d-owner-playtest-launch-packet-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/867-orchestration/Multi-867-2D-SITKA-OWNER-LOCAL-BROWSER-PLAYTEST-LAUNCH-AGENT-PACKET-20261120.md . **Claim issue:** https://github.com/jermur788/CCF/issues/867 . **Manager:** https://github.com/jermur788/CCF/issues/863 . Task READY/NOT RUNNING; do not pretend a physical session or exclusive lease exists. Review entire packet, live main coordination board and deployed physical host/dirty checkout/Ops locks before claiming `[RUNNING]`. On authorised owner Windows desktop WSL2 use a separate clean physical run lane and EXACT qualified #849 source commit `4c42e2012a4c1bee3a422e41a58535343d63ac9f` (not main). Verify four P0 prebuilt DLL pin values `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, never rebuild P0 or alter fixtures. Run direct .NET 8 UI project as in §4 with TMPDIR outside repo; confirm loopback-only live browser Year0 /100-tree app; give exact ephemeral URL **PRIVATELY** to owner, never in GitHub, leave fresh live session for owner select→reason→pending→approve 0 Fell→Execute→Year+1→history; ask for actual owner verdict. No Unity, source edits, saves, G1, network exposure, Windows build or main merge; stop on physical/P0/dirty/security blocker without workaround. Publish truthful real-launch and owner-feedback evidence as one verified GitHub docs Markdown handoff to #867 and #863; report NOT TESTED/HELD where applicable. **User action:** start this prompt in one local capable agent session on the correct desktop and open the returned private URL there.

**Packet author verification:** This Manager prepared the handoff using GitHub-only read inspection; the later execution agent must obtain real physical access/claims before any build or launch. The packet/issue do not assert that any owner browser is already running.
