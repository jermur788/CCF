# Map-First Menu & Lenses UI — Independent Re-Review (Repair Round 1)

- **Reviewer:** Claude Opus 5.5 (independent; did not write the change). Read-only apart from this file.
- **Date:** 2026-11-27
- **Review branch:** `review/multi-2d-map-first-lenses-ui-20261127-v1-r2`
- **Base:** `674c7c9e470d0aff0a028bd284fd7510f973e2d5`
- **Feature HEAD under re-review:** `1cda4a51258b8a35b4ba0ac53533105f2a482e35` (repair `21ad21ba` + handoff `1cda4a51`)
- **Prior review:** `MAP-FIRST-LENSES-UI-REVIEW-20261127.md` on `origin/review/multi-2d-map-first-lenses-ui-20261127-v1` @ `a99b6b4b` (NEEDS CHANGES)
- **Evidence (outside repo):** `/home/jer/Development/CCF-MapFirstLenses-ReviewR2-Evidence-20261127/` (`results.txt`, `results2.txt`, `*.out`, base clone) plus Windows `%TEMP%\ccf-mfr2-edge`, `ccf-mfr2-stepb-head`, `ccf-mfr2-stepb-base`.
- **Environment:** WSL2 Ubuntu, dotnet SDK 8.0.134 (`/home/jer/.dotnet`), node v26 (vite-plus), Windows Edge headless + Windows node for Edge suites. P0 Core DLL sha256 `9a99f781…34b3` (matches pin). Load average 0.2–2.0 during runs.
- **Not run (per brief):** `build/2d-gate/selftest.sh`, full `2d-gate.sh --browser`, Core.Tests, Phase1.Save.Tests. No Unity. No live screen-reader.

## 1. What I ran (exact counts)

| # | Check | Command (abridged) | Result |
|---|---|---|---|
| 1 | Fence | `git diff --name-only 674c7c9e...HEAD` | 22 files, all within `specs/multi-2d-map-first-lenses-ui-v1/**`, `src/Ui/multi/**`, `tests/**`, `Docs/Handoffs/.../map-first-lenses-ui/**`. 0 Core / wwwroot / `*.cs` / csproj / `build/2d-gate` / `Docs/Project` / Assets edits. |
| 2 | Roadmap | `git diff --name-only 674c7c9e HEAD \| grep -i roadmap` | No changes. `2D_Simulator_Strategic_Roadmap.md` still `Version: v1.18`. |
| 3 | Build HEAD | `/home/jer/.dotnet/dotnet build …/SitkaP1.Ui.csproj --nologo -warnaserror -p:P0CorePath=…/p0ref-seed/SitkaLab.Core.dll` | rc=0, **0 Warning(s), 0 Error(s)** |
| 4 | Build base | same, in a `--shared` sparse clone detached at `674c7c9e` (outside repo) | rc=0, 0/0 |
| 5 | **G2 HEAD** | `NODE=node bash tests/ui-smoke-multiarea.sh <evidence>` | Run 1: **FAIL 259/1** (see §4, environmental). Reruns r1, r2, r3: **PASS 363 passed, 0 failed** ×3 |
| 6 | G2 base | same in base clone | **PASS 363/0** ×2 |
| 7 | **Targeted Edge** | `bash tests/map-first-lenses-ui/run-map-first-lenses-edge.sh <win evidence>` | **PASS 56 passed, 0 failed** (1920×1080 24/0; 1280×720 24/0; server Z01/Z02 8/8) |
| 8 | Stage B HEAD | `bash tests/phase1-time-history-ui/run-step2-edge.sh <win evidence>` | FAIL **55 passed, 10 failed** |
| 9 | Stage B base | same in base clone (base driver) | FAIL **55 passed, 10 failed** |
| 10 | Stage B diff | `diff` of FAIL lines, payload-size extraction, normalised PASS lines | FAIL sets identical except CDP `timestamp` values; 24/24 extracted payload sizes byte-identical (incl. `bytes=527778, budget=524288`); PASS line shapes identical (55 = 55). |
| 11 | Cleanup | `/proc` scan for `sitkap1-ui` with my evidence path; `git status --porcelain` | 0 servers of mine left running; worktree clean before writing this file. |

## 2. Test-edit integrity (highest risk)

The implementer changed three gate drivers: `tests/ui-smoke-multiarea.sh` (G2), `tests/phase1-time-history-ui/drive-step2.mjs` (Stage B) and the targeted driver. I diffed each against base `674c7c9e` and, for the targeted driver, against the previously reviewed `a99b6b4b`.

### 2.1 `ui-smoke-multiarea.sh` (+6/−5, 4 hunks)

- **Lens-switch insertions (P2 block, L1829 and L1978):** one click on `[data-lens-btn="regeneration"]` before Plant/Spot use, and one back to `woodland` at the end of the block. Locked Decision #2 moves Plant/Spot into the Regeneration lens (`store.js:48` `tools: ['tree','cell','plant','spot']`; `setMapMode` refuses other tools, `modes/interactive.js:31-37`). So this is a real prerequisite that a user would also have to do. No P2 assertion was deleted, and the P2a–P2n checks run unchanged afterwards. Switching lens also resets the tool to Select Tree and clears plant/spot drafts (`interactive.js:16-20`). The insertion runs *before* any Plant draft exists, so it cannot hide a draft-loss problem.
- **J22 (L1767):** `alertText() === ''` became `data-top-tier !== 'blocking' && !alertText().includes('Outcome unconfirmed')`. The lock *behaviour* is still checked in full: "no request but a read can leave the page while locked" (unchanged), the click on the lock's Re-read button, and the `until(!advance.disabled && pendingIds().includes(lostTree))` wait (unchanged). Only the "strip is empty" post-condition was relaxed, because Decision #5 now shows an Attention Required reminder for queued work. The new form still fails if the lock alert, any other blocking alert, or the unconfirmed-outcome text remains. This is **not a bypass**, but it is narrower (see N-B).
- **R3i (L1647):** `alertText() === ''` became `data-top-tier !== 'blocking'`. The behavioural checks (inspector shown, `insp-id` changed from `refA`, announce says "Selected tree") are unchanged. A map-click refusal would raise a blocking `showAlert` (default tier is blocking, `api.js:631`), so the check still catches a spurious refusal. **Not a bypass**, same narrowing as J22.
- **No assertion deleted.** Base and HEAD G2 both report 363 checks.

### 2.2 `drive-step2.mjs` (Stage B NB-2 order 3 and order 2)

- Adds a `screenOf(u,v)` helper (a pure refactor of the existing inline CTM transform) and recomputes the second click's screen point from the same SVG (u,v).
- Adds the Regeneration lens switch (genuine prerequisite, as above) and the switch back to `woodland`.
- **Hold-cell Clear:** queues a temporary `clear/add` on `MA-A ground[0]` so the Attention strip is already open before the two map clicks, then removes it in clean-up (the result is not checked, but the following `pending-total === 'No pending orders'` wait proves the plan is clean). This is a fixture adjustment, not a weakened assertion. All Phase2DuplicateOrder / partial-disclosure / fold assertions are unchanged and pass at both viewports. It does expose a real layout shift (see N-A).
- `findOpenMapPoint` now runs after the strip is open and the lens is set, so its coordinates are taken in the final layout.
- **Stage B "10 failures identical to base": CONFIRMED independently**, not on the implementer's say-so: I ran both commits and diffed the outputs (§1 rows 8–10). All 10 failures come from the pre-existing Year-30 `503 PAYLOAD BUDGET EXCEEDED` (`bytes=527778 > 524288`) and the R2-2 / Year-sweep / console-5xx / Z02-stderr consequences it causes. Base, with its own unmodified driver, gives the same set.

### 2.3 `drive-map-first-lenses.mjs` (targeted; a99b6b4b → HEAD)

- The old MLUI-05 path used the search miss as the blocking alert. Now the search miss must rank **below** Attention (info tier, the m-1 fix), and a real blocking refusal is triggered (Skip to Review → `WorkPlanOpen`, from the untouched `panels/time-history.js`).
- The MLUI-05 `check(...)` moved later and gained conditions: blocking row `role="alert"`, `#announce` contains `WorkPlanOpen`, focus stays on `#alerts-expand` after keyboard Enter, focus is not `<body>` after Dismiss. It also now clicks the live caller "Open Work Plan" button. Every earlier condition is kept (top tier, order, attention present, no modal, History link). `pressKey` now sends `code` and `text` for Enter so it is a real keyboard activation. Net effect: **strengthened**.
- The `busy`/Approve-ready waits added before Approve are synchronisation only.

**Integrity verdict:** no assertion deleted or made trivially true. The lens insertions are genuine prerequisites. J22/R3i still check the lock and Escape behaviour; only the "alert strip is empty" post-condition was narrowed to "no blocking alert", for a documented decision.

## 3. Finding-by-finding disposition

| ID | Disposition | Evidence |
|---|---|---|
| **B-1** G2 regression (dom.js crash + P2b–P2g, J22, F2e, R3i) | **FIXED** | `dom.js:201` falls back to `document.documentElement` and guards `addEventListener`; `pop.contains` guarded. F2e: `api.js:357-371` ranks the manual blocking alert first and dedups `readout-unavailable`; `commands.js:367` passes `{tier:'blocking', id:'readout-unavailable'}`. P2/J22/R3i: §2.1. G2 HEAD 363/0 ×3 (one environmental first-run failure, §4); base 363/0 ×2. |
| **M-1** cloned caller nodes / dead Open Work Plan | **FIXED** | `api.js:486-488` inserts the real `item.parts` (no `cloneNode`). Targeted Edge clicks `#alert .alert-custom-parts button` (the `time-history.js:119` button with its direct listener) and it opens the Work Plan: PASS at both viewports. |
| **M-2** `#alert` region, refusals not announced | **FIXED** (with nit N-C) | `#alert` remains `role="region"` (`multi.html:110`), but each blocking row gets `role="alert"` (`api.js:504`), and `showAlert` announces the blocking text through the `#announce` polite live region (`api.js:643-646`; `announce` cancels a pending timer, `api.js:655`). The Edge check confirms `role=alert` and the announce text. No live screen-reader test (not available). |
| **M-3** focus lost on re-render; dead `focusTarget` | **FIXED** | No-op re-render skip via `alertSignature` (`api.js:514`). Focus is saved and restored with `data-focus-key` (`api.js:549`, `api.js:599`). Dismiss and expand use `restoreFocus` (`main.js:821`, `dismissAlert`). `focusTarget` is now live because the nodes are no longer cloned (`api.js:648`). Edge: Enter on `#alerts-expand` keeps focus there, and Dismiss focus is not `<body>`. |
| **m-1** everything blocking; duplicate lock/readout items | **FIXED** | Search miss is `tier:'info'` (`main.js:793`). Lock uses `id:'lock'` (`api.js:789`) and is deduped (`api.js:361`); a stale manual lock is hidden once `state.lock` is null (`buildAlertItems` `manual` guard); `readout-unavailable` is deduped (`api.js:371`). The other `showAlert` callers are all real refusals or failures, so their blocking default is appropriate. |
| **m-2** handoff honesty (G2 / Stage B not reported) | **FIXED** | Handoff §4 and §7 now report G2 and Stage B with commands and counts. The Stage B 55/10 claim and the base-parity claim were confirmed by my own runs. The handoff's G2 363/0 claim matches my reruns; it does not mention the transient W999 save timeout I saw on one run, which is consistent with that being environmental. |
| **N-1** wheel "fix" overstated | **FIXED** | Handoff now calls it a proactive containment guard. Confirmed: only `wheel` `stopPropagation` exists (`main.js:968`); `/multi` has no map wheel handler. |
| **N-2** Navigator scope unproven | **FIXED** | Handoff cites `MultiApi.Stand.cs`. Confirmed: `FilterViewToLanes` replaces only `Areas` (`with { Areas = … }`), and ecology filters only Areas/Planted/Shelters/Patches, so Pending/Approved lists stay whole-woodland. |

## 4. Observation: one G2 run failed for environmental reasons

The first G2 run at HEAD failed W999: the Python HTTP workflow witness's `POST api/multi/save` hit its 15 s `urlopen` timeout (W156 onward was skipped, which is why the run shows 259/1). That path is server-only (no `/multi` JS runs). The diff touches no `*.cs` file, so HEAD and base run identical server code. Three back-to-back reruns at HEAD were all 363/0, and base was 363/0 twice. I classify it as a transient I/O or timing stall in this environment, **not a regression**. I am recording it so the gate can be re-run once if it recurs.

## 5. Regression sanity

- **Lens/tool separation:** `setLens` (`modes/interactive.js:11-29`) changes only presentation state, resets the tool to Select Tree (safe), and never calls the API. Selection, Fell ticks, marks and view are kept; plant/spot point drafts are cleared, as before. `setMapMode` refuses tools that the lens does not allow. The repair commit did not change this logic.
- **Hotkeys:** `isTypingContext` (`main.js:563-569`) and the `dialog[open]`/modifier guards (`main.js:~717-719`) are unchanged. No hotkey exists for Approve, Execute, Advance, New Woodland, Load or Money Reset.
- **Work Plan gate, 3-way felling, Save v3:** exercised end to end by the targeted Edge suite (KeepForUse + LeaveAsDeadwood → Approve → Execute → Advance → cross-process Save/Load) at both viewports: PASS. Stage B Save/Restart/Load: PASS.
- **No forestry/financial math in JS:** the only `Math.pow`/`log10` calls are zoom steps and overlay opacity (`store.js:10,486,493`); basal-area values are only formatted with `fixed()`.
- **Modules:** 16 `.js` modules under `src/Ui/multi` (count unchanged; 0 new). The palette and encode-twice paths are not touched by the repair diff (only `api.js`, `commands.js`, `dom.js`, `main.js` changed since `a99b6b4b`).

## 6. New findings

| ID | Severity | Finding |
|---|---|---|
| **N-A** | Minor (non-blocking) | `#alert` sits in normal flow above `#workspace` (`multi.html:109-113`; `.alert.shown` adds margin and padding, `multi.css:123`). Now that Decision #5 shows an Attention strip for queued work, queueing the first order makes the strip appear and pushes the map down mid-interaction. A follow-up click aimed at the same spot lands elsewhere. The Stage B driver had to pre-open the strip with a temporary hold-cell Clear for this reason. Suggest reserving the strip's space or overlaying it in a follow-up. |
| **N-B** | Nit | J22/R3i went from "alert strip empty" to "no blocking tier". This is acceptable, but could be tightened to say the only remaining item is the expected pending-work Attention reminder. |
| **N-C** | Nit | A blocking refusal is now both a `role="alert"` row and a polite `#announce` message, so some screen readers may announce it twice. Needs a manual NVDA/VoiceOver check (not done here). |

No new blocker or major findings.

## 7. Conclusion

B-1, M-1, M-2, M-3, m-1, m-2, N-1 and N-2 are fixed. The mandatory G2 gate is back at base parity (363/0, three clean reruns; one environmental timeout recorded). The targeted Edge suite is 56/0. My own base-versus-HEAD run confirms the Stage B 55/10 result matches base exactly, and the 10 failures come from the pre-existing Year-30 payload-budget defect. The test edits are genuine prerequisites and fixture adjustments, with no deleted or weakened assertions beyond the documented, justified narrowing in J22/R3i. The fence is clean, the roadmap is unchanged at v1.18, and the build is 0/0.

INTEGRATION DECISION: APPROVE FOR INTEGRATION
