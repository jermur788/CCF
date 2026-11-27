# CCF Multi 2D: Map-First Menu & Lenses Upgrade, Independent Review

- **Date:** 2026-11-27
- **Reviewer:** Claude Opus 5.5 (independent, read-only; did not write the change)
- **Review branch:** `review/multi-2d-map-first-lenses-ui-20261127-v1`
- **Feature under review:** `feat/multi-2d-map-first-lenses-ui-20261127-v1` @ `78dd6432f0283d56ab89e434b92b14a15e67ab5a` (the same as `origin/feat/...` at review time)
- **Base:** `origin/integration/2d-sim` @ `674c7c9e470d0aff0a028bd284fd7510f973e2d5` (the merge-base, confirmed)
- **Commits reviewed:** `e1556e3b` (spec quartet), `e17ca43a` (implementation + Edge suite), `f3aadc7c` (handoff), `78dd6432` (Item 9 deferral note)
- **Reviewer evidence (outside the repo):** `C:\Users\hp\AppData\Local\Temp\ccf-mfl-review-opus-20261127\` (`edge/`, `smoke/`, `smoke-base/`, `smoke-probe/`, plus `*.console` files)

## 1. Scope reviewed

- The handoff and the spec quartet (`spec.md`, `tasks.md`; `plan.md` and the checklist skimmed).
- The full diff `674c7c9e...HEAD` (19 files, +2676/−203). I read every hunk of `store.js`, `main.js`, `modes/interactive.js`, `api.js`, `dom.js` and `panels/work-plan.js`, and the `multi.html` diff for script and inline-handler additions.
- How existing callers interact with the new alert strip (`showAlert` callers in `api.js`, `commands.js`, `main.js`, `dialogs/saves.js`, `modes/interactive.js`, `panels/time-history.js`).
- The existing gate suites that drive `/multi`: `tests/ui-smoke-multiarea.sh` (gate **G2**) and `tests/phase1-time-history-ui/drive-step2.mjs` (gate **Stage B**).

## 2. Commands run and results

| # | Check | Command / method | Result |
|---|---|---|---|
| 1 | File fence | `git diff --name-only 674c7c9e...HEAD` filtered against the 4 allowed roots | **PASS**: nothing outside `specs/multi-2d-map-first-lenses-ui-v1/**`, `src/Ui/multi/**`, `tests/**`, `Docs/Handoffs/.../map-first-lenses-ui/**`. No Core, wwwroot, `*.cs`, csproj, `build/2d-gate/**`, `Docs/Project/**`, Assets, Packages or ProjectSettings edits. No mode changes. |
| 2 | Roadmap untouched | `git diff --quiet 674c7c9e HEAD -- .../2D_Simulator_Strategic_Roadmap.md` | **PASS**: byte-identical, still `v1.18`. |
| 3 | `map/contract.js`, `panels/time-history.js` untouched | `git diff --quiet` | **PASS**: byte-identical. |
| 4 | Module rule | `ls-tree` .js lists base vs HEAD; `multi.html` diff | **PASS**: the same 16 modules, 1 `<script type="module">`, no new inline JS or `on*=` handlers, no package.json or bundler. |
| 5 | Module graph | `node Tools/MultiRackSitkaP1Lab/build/2d-gate/module-graph.mjs` | **PASS**: 16 modules, acyclic, 1 module script tag. |
| 6 | Build | `/home/jer/.dotnet/dotnet build .../SitkaP1.Ui.csproj --nologo -warnaserror -p:P0CorePath=.../p0ref-seed/SitkaLab.Core.dll` (SDK 8.0.134; seed sha256 `9a99f781…`) | **PASS**: 0 warnings, 0 errors. |
| 7 | Targeted Edge suite (re-run by me) | `bash tests/map-first-lenses-ui/run-map-first-lenses-edge.sh <evidence outside repo>` | **PASS**: `OVERALL: PASS (56 passed, 0 failed)`, 24/24 at each viewport, 8/8 server lifecycle checks. This matches the implementer's claim. |
| 8 | **G2 gate suite `ui-smoke-multiarea.sh` at HEAD** | `NODE=$(command -v node) bash tests/ui-smoke-multiarea.sh <ev>` | **FAIL**: `OVERALL: FAIL (285 passed, 1 failed)`. `J99` page witness crashed: `TypeError: document.addEventListener is not a function at initExplainPopover (multi/dom.js:203) at multi/main.js:1105`. The entire J/P/R/F page-witness section did not run. |
| 9 | G2 control at base | The same suite in a `--shared` sparse clone detached at `674c7c9e`, built the same way | **PASS**: `OVERALL: PASS (363 passed, 0 failed)`. Item 8 is therefore a regression introduced by this change. |
| 10 | G2 probe (diagnostic only) | The HEAD clone with a 2-line no-op `addEventListener/removeEventListener` added to the stand-in's `FakeDocument` (in the disposable clone only; reverted) | **FAIL**: `333 passed, 10 failed`: `F2e`, `R3i`, `J22`, `P2b`–`P2g`, and `J99` (null `[data-spot-order]`). See B-1. |
| 11 | Lens switch makes no mutation | Code inspection of `setLens` and the hotkey handler | **PASS (inspection)**: see §3. |
| 12 | Save v3 cross-process round-trip | Covered inside my re-run of item 7 (`vp1920/load`, `vp1280/load`) | **PASS** within that suite (cash €9,505.23, retained 0.126 m³, DW0001 after restart). |
| 13 | Stage B `run-step2-edge.sh` | — | **NOT TESTED** (time-boxed). It clicks `#mode-spot` / `#mode-plant` without a lens switch (`drive-step2.mjs:308,317,390,415`), which this change hides in the default lens, so I expect it to fail. |
| 14 | `phase1-save-ui`, `s0c-parity` Edge suites | — | **NOT TESTED**. |
| 15 | Excluded by brief | `build/2d-gate/selftest.sh`, full `2d-gate.sh --browser`, Core.Tests, Phase1.Save.Tests | **NOT RUN** (excluded). Ports :33767, :43649, :46593 were not touched. |
| 16 | Implementer evidence dir | `ccf-map-first-lenses-edge-20261127T211116Z` | Listed: run.log, 2 expected JSONs, 10 screenshots, 4 redacted server outs. I relied on my own re-run (item 7) rather than reading those files in detail, and did not visually review the screenshots. |
| 17 | Screen-reader / AT | — | **NOT TESTED** (and not claimed by the implementer). |

All servers my runs started were stopped by the suite scripts (no `sitkap1-ui.dll` process left from my runs). The disposable clone was deleted. The worktree was clean before this commit.

## 3. Lens/tool separation (critical safety property)

Verified by inspection at HEAD:

- `modes/interactive.js:11-29` `setLens()` only assigns `state.lens`, `state.overlay`, `state.mapMode='tree'`, bumps `cellPick`, clears `cellLoading`/`plantDraft`/`spotDraft`, calls `hooks.renderAll()` and `announce()`.
  - It makes **no** `call()`/`fetch()` and no approve or execute. It does not touch `state.selected`, `state.drafts`, `areaId`, `singleLaneMode`, `zoom` or `centre`.
  - Crop marks are server state, so they are unaffected.
  - Opening Regeneration lands on `tree`, not Plant.
- Hotkeys (`main.js:679` `onKeyDown`) map only to `setLens`, `cycleLane`, History toggle, `showPane('plan')`, `toggleSingleLaneMode` and the shortcuts dialog.
  - They are guarded by modifiers, an open `<dialog>`, and the input/textarea/select/contenteditable check.
  - No key reaches Approve, Execute, Advance, Skip, New Woodland, Load or Reset.
  - `M` performs only GET lane reads (`refreshMultiStandFrame`).
- **Result: PASS.** Caveat: the side effect of this property is B-1. Plant, Spot Clear and Inspect Cell become unreachable outside their lenses (`main.js:243`, `interactive.js:34`).

## 4. Findings

### B-1 BLOCKER: mandatory gate G2 (`ui-smoke-multiarea.sh`) regresses from PASS to FAIL

- **Evidence:** item 8 vs item 9. At HEAD the page witness crashes at `dom.js:203` (`initExplainPopover`, called unconditionally from `main.js:1105`), so 77 page checks never run. Base passes 363/0.
- The diagnostic probe (item 10) shows the crash is not the only problem:
  - **`P2b`–`P2g`:** the existing Plant/Spot Clear workflows fail. In the default Woodland lens the `#mode-plant`/`#mode-spot` buttons are hidden (`main.js:243`) and `setMapMode` refuses them (`interactive.js:34`).
  - **`J22`:** "a successful re-read lifts the lock and clears the alert" fails. The alert text remains after re-read; this links to M-1.
  - **`F2e`:** the "Execute needs a re-read" message is not shown. The strip shows only the built-in "Engine read-out unavailable" primary plus `Blocking (2)` (the duplicate-alert issue in m-1).
  - **`R3i`:** the dialog-only Escape check fails. I did not root-cause this one.
- `tests/**` was inside the file fence, so the implementer could have updated the suites deliberately where behaviour changed by design. They left them failing and did not run them.
- Stage B's `drive-step2.mjs` also clicks the now-hidden mode buttons (item 13).
- **Required:** make G2 (and Stage B) pass. Either keep the existing tool entry points reachable, or update the gate suites with owner-approved behaviour changes and give the reasons. Fix the alert regressions below, and show a passing gate run.

### M-1 MAJOR: the alert strip clones caller-supplied nodes, which drops their event listeners and rebuilds them on every render

- `api.js:466` renders `item.parts.map((p) => p.cloneNode(true))`. `cloneNode` does not copy `addEventListener` handlers.
- **Concrete break:** the Skip refusal's **Open Work Plan** button (`panels/time-history.js:119-120`, listener added directly, no `data-alert-action`) becomes a dead button in the strip.
- `rereadButton()` happens to survive only because it was given `data-alert-action="reread"` and is handled by delegation.
- `renderAll()` calls `renderAlertStrip()` on every render (`main.js:26`). That replaces the alert DOM each time, so keyboard focus placed on a control inside the alert (`showAlert(..., focusTarget)`, `api.js:559-572`) is lost on the next render.
- The cleanup after a lock is also unreliable (`J22`): the lock's manual blocking alert (`showLockAlert`, `api.js:696`) persists independently of `state.lock`.

### M-2 MAJOR: refusals are no longer announced to assistive technology

- `multi.html:110` changed `#alert` from `role="alert"` to `role="region"`.
- Server refusals (`showRefusal`, `showGuardRefusal`, lock alerts) do not call `announce()`. Before this change the `role="alert"` live region announced them; now nothing is spoken automatically.
- The change claims WCAG 2.2 work, so this regression should be fixed. Either keep a live region for blocking/refusal content, or announce it explicitly.

### m-1 MINOR: alert tiering and duplication

- `showAlert` defaults every caller to tier `blocking` (`api.js:561`), including benign messages such as the search miss "No exact match — the selection is unchanged" (`main.js:792`).
- The lock appears twice: once as the built-in `id:'lock'` item (`api.js:346`) and again as the manual blocking alert from `showLockAlert`.
- The readout-unavailable state has the same pattern (F2e shows `Blocking (2)`). The specific corrective message sits behind "All alerts".

### m-2 MINOR: the handoff omits existing gate suites it affects

- §4 of the handoff says the older Edge suites were "not re-run" because of scope. It does not mention `ui-smoke-multiarea.sh`, which is a headless **G2** gate check rather than a browser suite, and it is broken by this change.
- The claim "Playable Milestone 1 core flows ... directly verified" is true only for the new targeted driver. Existing Plant/Spot Clear regression coverage now fails.

### N-1 NON-BLOCKING: the wheel-isolation claim is vacuous

- The only `wheel` listener in `/multi` is the new `stopPropagation` at `main.js:968`. The map itself has no wheel zoom or pan handler. MLUI-08 holds, but trivially; the handoff describes a guard mechanism that has nothing to guard against.

### N-2 NON-BLOCKING / NOT VERIFIED: "Unfinished Work: whole woodland" scope

- `navigatorSnapshot()` (`panels/work-plan.js`) reads `view.pending`/`view.approved`.
- On ≥3-lane stands, `view` is the lane-filtered payload (`applyComposedStandFrame` in `api.js`).
- I did not verify that the lane-filtered payload carries whole-woodland orders. If it does not, the "Whole woodland" badge overclaims. The implementer should confirm this or relabel the badge.

### Guardrails checked (no finding)

- **Palette:** tokens unchanged per spec.
- **Encode-twice:** alert pills carry icon + badge text + colour (`api.js` `renderAlertRow`). Navigator pills use `◷/✓` + text.
- **No forestry or financial math added:** new JS only counts and labels server values.
- **Part 2 slots:** declarative only (`store.js` `LENSES[*].extensionSlots`, `state.extensionAlerts` empty, nothing wired).
- **CSP:** the popover is positioned via CSSOM (`dom.js` `pop.style.left/top`), not inline `style=` attributes. This is CSP-compatible.

## 5. Summary

The file fence, module rule, build, module graph, the lens/tool safety property and the implementer's own 56-check Edge suite all reproduce. However, the change breaks the mandatory G2 gate suite, which passes at base. With the crash bypassed, that suite still fails 10 checks: existing Plant/Spot Clear flows, lock-alert clearing and the Execute-re-read alert. Two alert-strip defects (dead cloned buttons and lost refusal announcements) are real behaviour regressions. This cannot be integrated until the gate is green again.

INTEGRATION DECISION: NEEDS CHANGES
