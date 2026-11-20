# #877 owner G2 private launch — readiness result

**2026-11-20 ~18:15 UTC.** **Status: PRIVATE LAUNCH READY / SERVER LIVE / OWNER PLAYTEST NOT YET PERFORMED.**
This document records a launch, not an acceptance. **No owner verdict is implied and none is made here.**

**Task:** [#877](https://github.com/jermur788/CCF/issues/877) · **Manager:** [#863](https://github.com/jermur788/CCF/issues/863)
**Binding packet:** [Multi #863–#875 independent review acceptance and #877 owner G2 launch](https://github.com/jermur788/CCF/blob/docs/multi-863-875-approved-g2-owner-playtest-route-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/877-orchestration/Multi-863-875-INDEPENDENT-REVIEW-ACCEPTANCE-AND-877-OWNER-G2-LAUNCH-PACKET-20261120.md)
**Reviewed candidate:** `03547c6d56b6ba05742e71714d0cc2a4431ecf78`

## 1. Actor, host and independence

| Field | Value |
|---|---|
| Actor | DeepSeek V4.1 Flash, `opencode-go/deepseek-v4.1-flash`, OpenCode/T3 Code |
| Host | physical owner desktop `DESKTOP-DL1BQI7`, WSL2 Ubuntu (not `jer-laptop`) |
| Relationship to author | **not** the Claude Opus 5.5 #868/#849 author, **not** the #875 review session, no author child or subagent |
| Authority used | launch only: local build/run, live endpoint check, private URL to owner, this document |
| Authority not claimed | no source/test/spec/fixture/P0/Unity/save/main change, no integration, no G2 or product acceptance |

**Model-substitution note for Manager.** The packet preferred "one locally capable Claude Code/other actual machine agent, no DeepSeek/Codex dependency". This is an actual machine agent on the required host and is independent of the implementation author, but it is the same provider family as the #875 reviewer. Because the work is launch-only, no review claim is reused; the Manager may reject the substitution if independence from #875 is required.

## 2. Lane and admission

- Claim registered on #877 before any physical change: [#877 comment](https://github.com/jermur788/CCF/issues/877#issuecomment-7046985977).
- **Read-only candidate lane:** `/home/jer/Development/CCF-Review-877/wt-owner`, clean **detached** worktree at the exact candidate. Not the #868 author lane, not the #875 review lane, not #867.
- **This document's lane:** `/home/jer/Development/CCF-Review-877/wt-docs`, branch `docs/multi-877-owner-g2-launch-result-20261120-v1` from `origin/main@1b1bb5f9`, one new Markdown only.
- **Scratch (outside every repository and save folder):** `/home/jer/Development/CCF-Review-877/scratch/` — fixture copy and `TMPDIR`.
- **Live pre-existing process, untouched:** #867 server **PID 390578** on `127.0.0.1:46311`, still running, not killed, not re-used, no shared port.
- No CCF-Ops board or lease tooling is installed on this host (`~/CCF-Ops`, `/media/jer/*/CCF*` absent). No Unity lease was needed or taken; this is a .NET standalone process only.
- `Docs/Project/coordination/active-tasks.md` on `main` carries no #877 row and no competing Multi 2D launch row; #877 itself is the authority for this task.

## 3. Exact candidate verification

```
git rev-parse HEAD  -> 03547c6d56b6ba05742e71714d0cc2a4431ecf78
tracked changes in lane -> 0 (before and after build)
```

Difference from base `4c42e2012a4c1bee3a422e41a58535343d63ac9f`:

| Change | Path |
|---|---|
| A | `Docs/Handoffs/MultiRack-Simulation-20261116/874-orchestration/Multi-874-SELECTED-2D-UI-IMPLEMENTATION-HANDOFF-20261120.md` |
| M | `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.css` |
| M | `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js` |
| M | `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/index.html` |
| M | `Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh` |

No `src/Core`, API, `.csproj`, fixture, P0, Unity or `main` file differs from base. **Minor documentation discrepancy, non-blocking:** the launch packet describes "two new handoffs" relative to base; this lane observes **one** new handoff file. The reviewed source/test bytes are unaffected, and the candidate SHA is exactly the one the #875 reviewer tested.

## 4. Pinned P0 and fixture

P0 read-only copy taken from the already-qualified pinned `.p0ref` of the #868 author lane (copy only, **never rebuilt**; `build/build-p0-core-ref.sh` not run).

```
1 csproj  : 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
2 dll     : 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
3 record  : 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
4 prov    : 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
```

Four-way match, copied files set read-only. Fixture `p1d-30m-sitka-adult-v1` unchanged in the repository
(`bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`) and served from a private read-only scratch copy with the same SHA-256.

## 5. Build and live readiness (actually executed here)

- `~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj` → **Build succeeded, 0 Warning(s), 0 Error(s)**. SDK 8.0.134 against `global.json` 8.0.133 `latestPatch`; `TMPDIR` outside the repository.
- Launch: `dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll <private scratch fixture dir>`, `setsid nohup`, `127.0.0.1` only, **OS-chosen ephemeral port**, fresh 256-bit capability printed at start.
- **Live process: PID `1750123`**, exact command line
  `dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll /home/jer/Development/CCF-Review-877/scratch/fixture-owner`.
  Port and capability are **deliberately not published here**; both were given to the owner privately in local chat.
- `ss -ltnp` at start: only `127.0.0.1:<ephemeral>` (mine) and `127.0.0.1:46311` (#867, untouched).

Private endpoint checks from inside WSL and **from the Windows side**:

| Check | Result |
|---|---|
| exact capability URL | **200**, 14180 bytes |
| same path without capability | **404** |
| wrong `Host` header (`localhost`) | **403** |
| Windows → WSL `Invoke-WebRequest` on the capability URL | **200**, `title=Stand Map` |
| Windows → WSL without capability | **404** |

Served bytes are the candidate bytes: `index.html` `a7f5fc30…7cfc2`, `app.css` `14db2510…701041`, `app.js` `fc868f34…df6c9` — served SHA-256 identical to the worktree files.

The page is the **#874 revised UI**: `Fit whole area`, `Zoom in`/`Zoom out`, `Pan north/east/south/west`, `Legend`, `Stand Map · one 30 × 30 m area · north up`, `Work Plan`, `Approved plan`, `Execute approved plan`, exact-ID search, reason dropdown with the five recorded reasons plus `Other — specify reason`, `History: actual before and actual after`.

Live session state read over `GET api/view` at launch:

```
ecologicalYear 0 · livingCount 100 · stemsPerHa 1111.11 · basalAreaM2PerHa 55.873
pending 0 · approved none · annualHistory 0 · workRevision 0 · domain DerivedP1ThirtyMetreExperimentV1
fixtureFileSha256 bcaaa5c3…f757c4 · managementScopeId MS-P30-UNDIVIDED-30M-V1 · scene trees 100 (100 Living)
```

## 6. Owner procedure (private browser, this machine)

Open the private capability URL in the Windows browser (given separately; **never** paste it into GitHub).
Start at **1280×720**.

1. Overall look, map prominence, compact `+`/`−`/`Fit`, north-up Sitka-only stand, species-versus-felling-state legend, no page-level body scroll. Compare honestly against the *conceptual* image #1 — the concept art is not a frozen reference asset and the page will not look like it.
2. Select a numbered tree by map click, and again by exact-ID search. Confirm the inspector, the reason dropdown, and that a blank `Other` reason is refused with an inline error.
3. Add two trees to the Work Plan: pending count rises, living count unchanged. Approve: `A` appears, **0 Fell**.
4. **The critical G2 question:** at 1280×720, `Cancel`/`Execute` sit below the **internal Plan-pane viewport** and need a scroll inside that pane. Is that comfortable and obvious? Does the selected tree and reason stay visible while you do it? If not, say so — that becomes a new bounded UI/UX request, not silent acceptance.
5. Execute with the separate confirmation (it names the IDs, the year and the PlanId). Expect a stump and gaps on the map, lower server tree count and basal area, and `Execute` no longer offered. Then `Year+1`. Check History and full source details. Compare at 1920×1080.
6. **Literal** browser Ctrl+`+` to 200 % (Ctrl+`0` to reset): one column, controls usable, no horizontal clipping, focus and scroll sane. This is not the same test as CDP device emulation.
7. Report exactly one of **`ACCEPTED FOR G2`**, **`NEEDS CHANGES`** (with UI location and steps), or **`HOLD`** (cannot complete). The Manager records acceptance; this launch lane cannot.

An optional screen-reader pass is **NOT TESTED** unless the owner runs one; `NaturalDead` status appears only if real state exposes it.

## 7. Stop procedure

Only this exact process, by PID — never a blanket `kill`, `pkill` or port sweep:

```bash
kill -TERM 1750123
```

If it is already gone, `ps -p 1750123` returns nothing and nothing needs doing. **Do not** touch `PID 390578` (#867) or its worktree. Ending the process ends the in-memory session; nothing is saved and the next launch starts again at Year 0.

## 8. Verified versus not verified

**Verified by me here:** exact candidate SHA and clean detached lane; five-file diff versus base; four-way P0 pin; fixture SHA in repo and in the private scratch copy; 0/0 UI build; live loopback endpoint with correct 200/404/403 guards; served bytes identical to candidate bytes; revised-UI feature presence; live Year 0 / 100 Living session; Windows-side reachability; #867 process untouched.

**Not verified here:** I did **not** re-run `tests/ui-smoke.sh` (140/140) or `Core.Tests` (92/92) — those remain #875 evidence on this same commit, not my result. Not verified: human playtest, literal Ctrl+zoom in the owner's browser, screen-reader run, visual fidelity to concept image #1, complete contrast matrix, `NaturalDead` display, and anything at all at 1920×1080 until the owner looks. No Unity, no Windows Player build, no G1, no save/schema, no main/G4 integration.

## 9. Notes for the Manager

- `origin/main` has advanced to `1b1bb5f9` ("#873 native partial outcome and #876 narrow F1 repair scope") since the packet was written against `f852f6ca`. This launch deliberately used the reviewed candidate `03547c6d`, **not** main and not any #876 repair.
- The #875 F5 finding (internal Plan-pane scroll for `Cancel`/`Execute` at 1280×720) is the main subjective question and is expected to be exercised in step 4 above.
- The owner's verdict is the only thing that can close #877 as a G2 decision; this document only proves the interface was launched correctly on the exact reviewed candidate.
