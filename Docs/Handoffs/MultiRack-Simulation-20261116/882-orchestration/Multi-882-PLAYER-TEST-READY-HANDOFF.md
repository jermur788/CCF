# Multi #882 — PLAYER-TEST-READY handoff for owner G2 (not accepted, not merged)

**Date:** 2026-11-20. **Status: READY FOR OWNER G2 PLAYTEST — recommendation only.** This is **not** owner acceptance, **not** G2 PASS, **not** integration, **not** a merge, and **not** a release. Only the product owner can accept the feature by personally playing it.

Prepared by DeepSeek V4.1 Flash as **orchestrator only**. No source, test or UI line in this task was written by the orchestrator. The cast was: GPT-6.1 Sol sole implementation and repair author, GPT-6.1 Sol in a separate session for the independent review, and GPT-6.1 Sol in a further fresh session for the targeted re-check after the owner-authorised repair.

## 1. Exact candidate under test

| Item | Value |
|---|---|
| Reviewed and re-checked source | `e11dd5192d7a89abf859613ce92814bdd6338ea2` |
| Repair-report head (docs only) | `32a3ddc46e032c92431d113325376adfcbdc0cd0` |
| Original task base | `03547c6d56b6ba05742e71714d0cc2a4431ecf78` |
| Independently reviewed source (pre-repair) | `43bfba8e82adcc31d71dbb13d2b0c00cd682ec4d` |
| Branch | `feat/multi-882-competition-unity-ui-firstpass-v1` |
| Changed source/test paths | 13 authorised paths, all under `Tools/MultiRackSitkaP1Lab/` |
| Docs added by this task | implementation result, independent review, repair result, targeted re-check |

## 2. Evidence chain (each step by a different session)

1. **[Implementation result](Multi-882-FIRSTPASS-IMPLEMENTATION-RESULT.md)** — author verification: Phase A diagnostics/UI and Phase B server-authoritative Crop after the owner grant.
2. **[Independent review](Multi-882-INDEPENDENT-REVIEW-RESULT.md)** — verdict **NEEDS CHANGES**: blocking B1 (Crop routes accepted incomplete fences and duplicate JSON members and still committed state), non-blocking N1 (competitor selection dropped keyboard focus).
3. **[Owner decision on #885](https://github.com/jermur788/CCF/issues/885#issuecomment-7049935573)** — bounded repair authorised, no new file scope, targeted re-check afterwards.
4. **[B1 repair result](Multi-882-B1-REPAIR-RESULT.md)** — repair confined to `PlayerApi.cs`, `app.js`, additive `ui-smoke.sh`.
5. **[Targeted re-check](Multi-882-TARGETED-RECHECK-RESULT.md)** — **TARGETED RECHECK PASS**, fresh session, adversarial: **Core 108/108**, **smoke 287/287** (178 original + 109 additive), **162/162** separate refusal/no-mutation probes, both builds 0 warnings / 0 errors. All four Core files byte-identical to the reviewed source; no original smoke line removed or weakened.

## 3. Verified launcher actually started by the orchestrator

| Item | Verified value |
|---|---|
| Host | physical `DESKTOP-DL1BQI7`, WSL2 Ubuntu, user `jer` |
| Launch worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-Launch`, detached at `e11dd519…` |
| Process | `sitkap1-ui.dll fixtures/derived`, **own PID 1806361** |
| Listener | `127.0.0.1:43649` (own OS-assigned loopback port) |
| Capability page | HTTP **200** with capability; bare `127.0.0.1:43649/` returns **404** |
| P0 reference | four-way check passed; `.p0ref/SitkaLab.Core.dll` = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, never rebuilt |
| Fixture | committed `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` = `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` |
| Build | `dotnet build src/Ui/SitkaP1.Ui.csproj` — 0 warnings, 0 errors |
| Collision check | #867 (PID 390578 / :33767) and #877 (PID 1750123 / :46311) still alive and untouched; separate worktree, port and fixture |

**The capability URL is deliberately NOT published here.** It was given privately to the owner in chat only. Publishing it would hand an editor-capable loopback session to anyone who reads this repository.

## 4. Owner access

1. On `DESKTOP-DL1BQI7`, open the private `http://127.0.0.1:43649/s/<capability>/` address supplied in chat, exactly, in a Windows browser (WSL2 localhost forwarding reaches it).
2. Use `127.0.0.1` only. `localhost` or any other host name is refused by design.
3. Nothing is saved. The session is in memory only; a reload keeps the session, closing the process ends it at Year 0 next time.

## 5. Suggested owner trial flow

1. Load the page; confirm the map shows the whole 30 m area with one undivided management area and no rack claim.
2. Select a living Sitka by crown, by the table, or by exact ID `P30-045`.
3. **Retain as Crop Tree.** Confirm a real server change (plan revision moves), a blue/`C` retained treatment, and that this is not a Fell proposal.
4. Inspect the Crop Tree. Read the top-five competitors: exact IDs, DBH, distance, contribution, share %, strength, current versus after-removal totals.
5. Confirm unrelated crowns dim while the Crop Tree and its ranked competitors stay bright and numbered, and that exiting inspection restores full brightness.
6. Select a listed competitor (for example `P30-035`) and add it to the Work Plan using a competition-release reason. No tree may be felled by that action.
7. Check reasons: no poor-form/damaged presets; `Other` demands real text; nothing is pre-filled for you.
8. Approve the whole Work Plan and confirm it reports **0 trees felled**.
9. Check Execute/Cancel are reachable at 1280×720, and that no huge raw PlanId or execution key appears in the Plan, approval, Execute dialog, receipt or History.
10. Confirm Execute once; confirm the felled count, the stump, and that measured competition after execution matches the earlier preview.
11. Try the refusals: attempt Fell on the retained Crop Tree (must refuse), unmark it first (must then allow), and use a stale page state (must refuse rather than silently change).
12. Advance one genuine ecological year; confirm the Crop Tree survives, living/stump counts move honestly, and a Crop mark on a tree that dies is no longer shown as retained.
13. Try map navigation: `NORTH/WEST/SOUTH/EAST` pan only when zoomed (disabled and stated at full fit), and the separate on-map `+ / − / Fit` zoom does not select a tree underneath.
14. Re-check at 1280×720, a narrow window, and browser zoom: no needless scroll to reach map controls or Work Plan actions.

## 6. Stop instructions (safe, exact)

```bash
kill -TERM 1806361
```

Expect the server to print `Session ended; nothing was saved.` Do **not** use broad `pkill` patterns: PID 390578 (#867) and PID 1750123 (#877) are other people's live playtest lanes.

## 7. What this candidate does NOT do

- No save, load or resume; closing the process loses the session. No schema change.
- Synthetic adult Sitka only, one 30 m area. Light, shade, seed production and regeneration are **NOT MODELLED** and are shown as unavailable, not faked.
- Not Unity/3D numerical or geometry parity; the broad #881 parity programme stays open.
- No multi-rack, corridor, cost/revenue or comparison-extension feature.
- Screenshots, trusted/native input, touch, screen reader, literal 200% zoom and rendered aesthetic judgement remain **NOT VERIFIED** by any automated step; the reviewer and re-check explicitly declined to claim them. Those are exactly what the owner's personal playtest supplies.
- No `main` merge, no Unity work, no Windows build, no owner acceptance.

## 8. Decision this handoff asks for

Owner G2 on the exact candidate above: **accepted**, **needs changes** (with the specific points), or **hold**. A "needs changes" answer returns to Manager #885 for a bounded decision; it does not authorise another automatic repair cycle, and it does not invalidate the recorded review history.
