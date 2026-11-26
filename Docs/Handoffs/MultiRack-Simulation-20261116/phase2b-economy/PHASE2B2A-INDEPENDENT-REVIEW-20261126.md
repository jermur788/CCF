# CCF Multi 2D — Phase 2B-2a Economy UI & `/multi` Routes — Independent Review

| Field | Value |
|---|---|
| Date | 2026-11-26 |
| Candidate branch | `feat/multi-2d-phase2b2a-economy-ui-20261126-v1` |
| Candidate HEAD | `559c0690b910075d243ca17c9737c16a9cc49241` (implementation `e0d5938217f0bf3a3c886594730e2a5348bba0a4` + handoff commit) |
| Base | `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`) |
| Review branch | `review/multi-2d-phase2b2a-economy-ui-20261126-v1` |
| Reviewer identity | requested: `claudeAgent/claude-opus-5-5`, effort=medium · selected: Claude Code agent · resolved model: `claude-opus-5-5` (Opus 5.5), effort=medium |
| Reviewer independence | Did not author the candidate. Read-only on the feature branch; this file is the only write. |
| Reviewer evidence root | `/home/jer/Development/CCF-Multi-2D-Phase2B2a-Evidence-20261126/review/claude/` (outside the repository) |

**Verdict: APPROVED WITH CONDITIONS**

---

## 1. Scope reviewed

- The full diff `6d72c305..559c0690`: 22 files, +2446/−179. I read every hunk:
  - 6 server files: `MultiRoutes.cs`, `PlayerApi.cs`, `MultiApi.{View,Commands,History,Saves}.cs`.
  - 9 frontend files: `multi.html`, `multi.css`, `store.js`, `api.js`, `commands.js`, `main.js`, `panels/work-plan.js`, `panels/time-history.js`, `dialogs/saves.js`.
  - 2 new Edge suite files: `run-economy-edge.sh`, `drive-economy.mjs`.
  - The 4-file spec quartet and the implementation handoff.
- Gate infrastructure I read but did not change:
  - `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh` (S0c branch, lines 1031–1100; stage map for G3–G7).
  - `build/2d-gate/s0a-multi-asset-identity.json`, `s0a-move-table.json` and `protected-paths.txt`.
- Implementer evidence I opened:
  - The `implementer-gate/` directory: `run.log`, `gate.log` and `gate-summary.json` for each suite.
  - The 18 economy-edge screenshots.
- Owner amendments that this candidate is judged against: D1b, D2a, D3, D4. The original bundle does not apply where these amend it.

## 2. Method (commands actually run)

Environment pinned to dotnet `8.0.133` (`$HOME/.dotnet-8.0.133`) with vite-plus on PATH and `TMPDIR` outside the repo. Runner: `review/claude/run-review-gates.sh`. All commands ran in `Tools/MultiRackSitkaP1Lab` of the feature worktree at HEAD `559c0690`.

1. `$DOTNET build MultiRackSitkaP1Lab.sln -c Debug --disable-build-servers`
2. `$DOTNET build tests/Core.Tests/SitkaP1.Core.Tests.csproj …`, then `$DOTNET tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll`
3. `$DOTNET build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj …`, then `$DOTNET tests/Phase1.Save.Tests/bin/Debug/net8.0/Phase1Save.Tests.dll`, plus 8 isolated reruns
4. `$DOTNET build src/Ui/SitkaP1.Ui.csproj …`
5. `DOTNET=$DOTNET tests/ui-smoke-multiarea.sh <ev>`
6. `DOTNET=$DOTNET tests/ui-smoke.sh <ev>`
7. `DOTNET=$DOTNET tests/phase1-save-ui/run-edge-e2e.sh <ev>`
8. `DOTNET=$DOTNET tests/phase1-time-history-ui/run-step2-edge.sh <ev>`
9. `DOTNET=$DOTNET tests/phase2b2a-economy-ui/run-economy-edge.sh <ev>`
10. Canonical gate, run twice: `build/2d-gate.sh --base 6d72c305… --p0ref-dir <evidence>/p0ref-seed --headless-only <ev>`
11. Independent S0c reproduction: compared `git show HEAD:…/multi.css | sha256sum` with the base blob and with the locked `rSha256`.
12. Static checks:
    - Forbidden-path diff.
    - 64-hex token scan.
    - JS module count at base and HEAD.
    - Grep of added JS lines for arithmetic, `Math`, `toFixed`, `parse*`, `Intl` and `*Cents`/`*Minutes` use.
    - Grep for clearfell, override, disposition, `KeepForUse` and retain.
    - Grep for CSP and guard edits.
    - Grep for `<script` and `style=` in `multi.html`.
    - `git ls-remote` for the branch.
13. Base comparison for Save: `git worktree add --detach <evidence>/base-wt 6d72c305`. The build failed with "P0 Core reference missing", which is expected for a fresh worktree with no `.p0ref`. I removed the worktree with `git worktree remove --force` and `prune`.

## 3. PASS / FAIL / NOT TESTED matrix (reviewer's own runs)

| Gate | Exact command (abbrev.) | Observed summary line | Result |
|---|---|---|---|
| Solution build | `dotnet build MultiRackSitkaP1Lab.sln -c Debug` | `0 Warning(s)` / `0 Error(s)` (RC 0) | **PASS** |
| UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj -c Debug` | `0 Warning(s)` / `0 Error(s)` (RC 0) | **PASS** |
| Core tests | `SitkaP1.Core.Tests.dll` | `291/291 passed, 0 failed` | **PASS** |
| Save tests | `Phase1Save.Tests.dll` (first run) | `49/50 passed, 1 failed` — `FAIL NB1c … expected 1, got 3` | **FAIL (known flake; see note)** |
| Save tests, 8 isolated reruns | same | 7 × `50/50 passed, 0 failed`; 1 × NB1c `49/50` | **PASS 7/8 (flake)** |
| Save tests inside gate run 2 | `2d-gate.sh` G2 | `G2: Phase1.Save.Tests PASS (50/50 passed, 0 failed)` | **PASS** |
| Multiarea smoke | `tests/ui-smoke-multiarea.sh` | `OVERALL: PASS (363 passed, 0 failed)` | **PASS** |
| Single-area smoke | `tests/ui-smoke.sh` | `OVERALL: FAIL (250 passed, 4 failed)`, exactly `L01, L02, L06, J31` | **PASS-BASELINE (known 4 only)** |
| Save Edge | `tests/phase1-save-ui/run-edge-e2e.sh` | `OVERALL: PASS (63 passed, 0 failed)` | **PASS** |
| Step 2 Edge (+G5 sweeps, Money Off) | `tests/phase1-time-history-ui/run-step2-edge.sh` | `OVERALL: PASS (67 passed, 0 failed)`; G5 ma-1 max raw `524279 B`, eco `43320`; ma-1-mixed max raw `523375 B`, eco `49685` | **PASS** |
| Economy Edge | `tests/phase2b2a-economy-ui/run-economy-edge.sh` | `VIEWPORT 1920x1080: 24 passed, 0 failed` / `VIEWPORT 1280x720: 41 passed, 0 failed` / `OVERALL: PASS (71 passed, 0 failed)` | **PASS** |
| Payload budgets, Economy Off | via Step 2 G5 Year 0..30 sweeps (both experiments) | all 31 years ≤ 524288 B raw and ≤ 153600 B eco | **PASS** (the gate's own G5 did NOT run) |
| Canonical gate, run 1 | `build/2d-gate.sh --headless-only --p0ref-dir $SEED` | `ERROR [G2]: Phase1.Save.Tests exited non-zero` → `VERDICT: FAIL (exit=1)` (NB1c flake) | **FAIL (flake)** |
| Canonical gate, run 2 | same | G0 PASS; G1 PASS (seed `9a99f781…34b3`); G2: Core 291/291, Save 50/50, multiarea 363/0, ui-smoke 250/4-known (`PASS-BASELINE-DRIFT`), cli-safety 23/0, build-output-safety PASS (C7/C8 HOST-NA), p0-isolation PASS; then `S0c mode: multi.css no longer matches T0 R_css` → `GATE G2 FAIL`; G3–G7 NOT RUN; `VERDICT: FAIL (exit=1)` | **FAIL (S0c lock only)** |
| Gate G3–G7, B | — | not reached | **NOT TESTED** |
| Forbidden paths | `git diff --name-only base..HEAD` vs fence and `protected-paths.txt` | no hits under `src/Core`, `tests/Core*`, `build/2d-gate*`, `SitkaP1.Ui.csproj`, `wwwroot`, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/` | **PASS** |
| JS module count (D4) | `git ls-tree … src/Ui/multi \| grep -c .js` at base and HEAD | `14` / `14`; `SitkaP1.Ui.csproj` and `MultiPageModules` unchanged | **PASS** |
| Capability tokens | `git diff … \| grep -oE '[0-9a-f]{64}'` | only `9a99f781…34b3` (the public P0 seed provenance hash in the handoff); no `/s/<capability>/` token | **PASS** |
| CSP / guards | grep of diff for CSP, `script-src`, Origin, Host, capability, MaxBodyBytes | 0 changed lines; `multi.html` has 1 `<script` (module) and 0 `style=` | **PASS** |
| D-020 zero JS money arithmetic | grep of added JS lines | only `l.materialCostCents > 0` (a display-only comparison, `work-plan.js:209`) and `Number(year)`; no sums, rounding or unit conversion | **PASS** |

**NB1c note.** `Phase1Save.Tests` references only `src/Core/SitkaP1.Core.csproj`. Neither Core nor the Save test sources changed between base and HEAD, so NB1c behaves the same on base as on HEAD by construction. That matches the documented, already-known NB1c flake (about 7.5%). I could not run the Save tests on base directly: a fresh detached worktree has no `.p0ref`, so the build stops at "P0 Core reference missing". This is not a regression introduced by the candidate.

## 4. Findings by severity

Totals: **BLOCKER 0 · MAJOR 2 · MINOR 6**

### BLOCKER
None. I found no D-020 violation, no forbidden-path edit, no way to assign Fell to Landowner, no clearfell or stage-override path, no disposition selector or `KeepForUse` m³, and no Money toggle that silently loses work.

### MAJOR

**M1 — 2B2a-REG: the canonical gate has not passed, and G3–G7 have never run on this candidate.** (Acceptance: 2B2a-REG)
- `build/2d-gate.sh` exits 1 at G2 sub-check 8: `build/2d-gate.sh:1043-1044` with `build/2d-gate/s0a-multi-asset-identity.json` `"mode": "s0c-css-plus-html-delta"`.
- The cause is gate infrastructure, not behaviour (see §5). But because G2 stops there, G3, G4, G5, G6 and G7 have never run in their canonical form on this candidate.
  - G5 is covered substitutively by the Step 2 sweeps.
  - G7 is covered substitutively by my diff and `protected-paths.txt` check.
  - G3 and G4 are NOT TESTED as canonical stages. I judge both low risk: no Core or Save sources changed, and the G4 golden saves are untouched.
- Spec §5 itself requires `2d-gate.sh --headless-only (exit 0)`. **This is the integration condition C1.**

**M2 — Spec and handoff bookkeeping claim things the artefacts contradict.** (Acceptance: 2B2a-01 quartet consistency; unbacked-claim rule)
- `specs/multi-2d-phase2b-economy-ui-v1/tasks.md:9` (T-06) is ticked `[x]`, but its text includes `build/2d-gate.sh --headless-only (exit 0)`. The gate exited 1 in the implementer's own run and in both of mine.
- `tasks.md:11` (T-08) is ticked `[x]` for "push feature branch to `origin`". Meanwhile the handoff (`PHASE2B2A-IMPLEMENTATION-HANDOFF-20261126.md:5,98,137`) says "LOCAL ONLY — stopped before push".
- `git ls-remote` shows the branch **is** on origin at `559c0690`. So the two documents contradict each other, and T-06 is false as written.
- Fix is documentation only: un-tick T-06 or annotate it as blocked on the S0c lock, and correct the push status. **This is the integration condition C2.**

### MINOR

**m1 — Money mode is inferred from whether the `economy` field is present, so an On session can render as "Money: Off".** (2B2a-03 / honesty)
- `MultiApi.View.cs:524-547`: `TryBuildEconomySnapshot` returns `null` when it catches a `P1Refusal`, or when the economy view's Source or PlanStateRevision differs from the view (a concurrent-mutation race).
- `store.js:102-104`: `isEconomyEnabled()` is simply `Boolean(state.economy && state.economy.enabled)`.
- For an Economy-On session, such a payload would make the UI show `Money: Off`, hide the strip and quote, drop the client-side Approve/Execute cash block, and enable Save.
- Every one of those actions is still refused by the server, so nothing is lost. The fault is that the UI misreports the mode.
- Suggested fix: emit an explicit `economyEnabled` boolean on every payload (it can stay omitted when Off if byte-comparability matters), or fall back to `source.areaModelVersion` containing `+economy-v1`.

**m2 — Economy constants are hard-coded in UI wording instead of being formatted by the server.** (2B2a-03 / D-020 spirit)
- No arithmetic is involved, but these strings would drift if `EconomyOptions` changed:
  - `MultiApi.View.cs:657` ("€2,500 minimum contractor job floor applied", where `MinimumJobFloorCents` is available)
  - `:811-813` (material prices)
  - `:820-821` (`€2,500`, `€45/hr`)
  - `:835` ("40-hour")
  - `commands.js:258` (`€12,000.00`, `40 hrs/year`)
  - `api.js:188` ("40-hour")
  - `multi.html:60` (`40.0 h/yr`)
  - `work-plan.js:199,201` (`€0.00`, `890 kg/m³`)
- They are correct for `EconomyOptions.Default`, the only options this lane can launch.

**m3 — The session swap is torn-frame-safe per request but not fully serialised against concurrent commands.** (2B2a-04 / security & architecture)
- `MultiApi.Commands.cs:73-102`: the swap and the dirty check run under `multiSwapSync`, but mutating commands capture `CurrentMulti` without taking that lock.
- First consequence: a command already running when a swap lands mutates the discarded session. Its response carries the old `sessionId`, and the next fenced call refuses with `PlayerSessionMismatch`. Recovery is honest.
- Second consequence: a command could add work between the dirty check and the swap. The shipped UI always shows the confirmation dialog and sends `confirmReset: true`, so in practice this window only affects raw API callers.
- No torn frames: every handler reads exactly one `MultiSessionPair`. Acceptable for a single-player loopback tool. Record it as a residual risk.

**m4 — Gaps in the Edge suite's assertions.** (2B2a-05/06/08 evidence strength)
- `drive-economy.mjs:454` is `check(true, …)`. It is guarded in practice by the `waitFor` on line 452, which throws on timeout, but it inflates the pass count by 1 for each full-flow viewport.
- `Network.enable` is sent at `:183`, inside `runSuite`, after `Page.navigate` at `:552`. So 404 or 5xx responses during the initial asset load are caught only indirectly, through `Log.entryAdded` console errors (Log is enabled before navigation). The "zero 404" claim is weaker for initial assets.
- `checkLayout` (`:157-174`) checks h-scroll, a single-row top bar, ≤3 columns, the top-action buttons' right edges and the strip's right edge. It does not check that the quote panel, the dialogs or the drawer are unclipped. I inspected screenshots manually instead (m5).

**m5 — The top-bar experiment badge is truncated more at 1280 and at 1920.** (2B2a-02 "zero clipped")
- `multi.css` compact rules narrow `.brand` from `flex 1 1 8.5rem / min-width 8.5rem` to `7rem` to make room for `Money: On/Off`.
- In my screenshots the badge reads "Synthetic MA-1-MIXED demo — Area A Si…" (1280×720) and "…not a surveyed Irish forec…" (1920×1080), with ellipsis and a `title` tooltip.
- It is text, not a control, and no control or overlay is clipped. Still, the honesty caveat "not a surveyed Irish forecast" is no longer readable at 1280 without hovering.
- `#economy-strip-note` also truncates with ellipsis by design.

**m6 — Some of the handoff's evidence citations are imprecise.** (unbacked-claim rule; downgraded because I reproduced them)
- Handoff §3 quotes the gate line `S0c mode: multi.css no longer matches T0 R_css`. That text is not in any implementer artefact: `implementer-gate/2d-gate/gate.log` only has `ERROR [G2]: S0a /multi asset identity failed:` with an empty reason, because the message goes to stderr and is not written into the logs.
- I reproduced the exact line myself (`review/claude/gates/2d-gate-run2.out:20`), so the claim is true. The implementer's artefacts just don't show it.
- Handoff §2 reports the token scan as `NONE`, but the handoff itself adds the 64-hex P0 seed hash. That hash is harmless provenance and not a capability token.
- `gate-summary.json` reports `"networkIsolation": "NOT PROVEN"` while `gate.log` prints `NETWORK ISOLATION: PROVEN`. That inconsistency is in the gate, not the candidate. Recorded for the integrator.

## 5. Independent gate-lock diagnosis

**I agree with the implementer's diagnosis.** The `2d-gate.sh` failure is a stale S0c asset-identity lock. Only the integrator can retire it, and it is caused solely by the intended UI change. It is not a behavioural regression.

- `Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-multi-asset-identity.json` is still active with `"mode": "s0c-css-plus-html-delta"`, `t0Commit 47b27a8d…`, and css `rSha256 f3b2b68a7547acc78444f6389db0f9f39080589950a54417506c4aaf036f00a5`.
- `build/2d-gate.sh:1031-1044`: in that mode the HEAD `multi.css` blob must hash to T0 `R_css`.
- Lines `1046-1064`: `multi.html` must equal T0 except for the single script-tag swap.
- Phase 2B-2a has to change both files: a toggle, strip, quote panel and dialog in HTML, and their styling in CSS. The check is therefore guaranteed to fail for any UI feature lane.
- My reproduction:
  - HEAD `multi.css` sha256 = `ad129dc91162df59c7554b7c72c63c511cef76ca26db4cce1a1d35747024bd28`.
  - Base `6d72c305` = `f3b2b68a…00a5`, which equals the locked `rSha256`.
  - Gate run 2 printed exactly `S0c mode: multi.css no longer matches T0 R_css`.
- The C1 integrator delta (`5913cf83`, `45417789`) set `s0a-move-table.json` to `"gateEnforce": false` but left the asset-identity lock untouched. That is consistent with the implementer's account.
- **Functional gates:** in run 2, every functional G2 sub-check that ran passed: Core 291/291, Save 50/50, multiarea 363/0, ui-smoke 250 with only the 4 known failures (`PASS-BASELINE-DRIFT`, G11b PASS), cli-safety 23/0, build-output-safety PASS, p0-isolation PASS.
- **Gate run 1** failed earlier, at Save, because of the pre-existing NB1c flake (§3 note). That flake is not attributable to this candidate.
- **No functional gate failed** because of the candidate. G3–G7 did not run, so they are NOT TESTED in canonical form (M1).

## 6. Acceptance criteria, one by one (amended scope)

| ID | Verdict | Evidence |
|---|---|---|
| **2B2a-01** spec quartet present and consistent | **PASS with M2** | `spec.md`, `plan.md`, `tasks.md` and `checklists/requirements.md` are present. D1b, D2a, D3 and D4 are recorded correctly in spec §1 and acceptance §5. `tasks.md` T-06 and T-08 are ticked falsely (M2). |
| **2B2a-02** theme; 1920×1080 and 1280×720 clean; no h-scroll; ≤3 columns; nothing clipped | **PASS with m5** | My Economy Edge run: `layout (…)` PASS at both viewports, Off, On and Blocked (`1920<=1920`, `1280<=1280`, 3 columns, 1 History button). Screenshots inspected: dark woodland palette kept, no 4th column, controls not clipped. Badge text truncation is m5. |
| **2B2a-03** Money Off by default (MA-1 and MA-1-MIXED); toggle → fresh Year-0 with confirmation; Cash and `X / 40 hrs`; no retained m³ | **PASS with m1, m2** | `Program.cs` is untouched (`MultiAreaModelSelection.Phase2`, no economy). Edge: `Money defaults to Off` on vp1920, vp1280 and ma1-check. `GET api/multi/view omits "economy"` passes. The server refuses an unconfirmed dirty toggle (409 `EconomyResetConfirmationRequired`). The UI dialog lists the progress it will discard (`commands.js:240-272`) and always needs an explicit Confirm click. Stale fence → 409 `PlayerSessionMismatch`. Strip shows `€12,000.00` and `40 / 40 hrs`. `OpenFreshMultiSession` (`MultiRoutes.cs:92-112`) matches the `Program.cs:122-129` genesis exactly. No `KeepForUse` or retained-timber UI (grep, plus the Edge `hasKeepForUse:false`). |
| **2B2a-04** server quotes only; no JS money arithmetic; Fell Contractor-only; no clearfell or override; no disposition selector | **PASS** | All figures are `*Display` strings from `MultiApi.View.cs` formatters; the JS grep found no arithmetic. Fell rows only render the locked `Contractor only` pill (`work-plan.js:30-34`); Landowner buttons render only for non-harvest kinds. Core refuses `EconomyFellContractorOnly` (`MultiAreaContinuation.Commands.Economy.cs:75`); Edge PASS for the 409. Stage comes from `EconomyDefaults.AutoHarvestStage`, with no player input. No clearfell, override or disposition strings in the UI. |
| **2B2a-05** Edge proof: €2,500 floor highlight; cash block with honest reason; landowner deduction and 40 h reset; atomic settlement with no double charge on replay; ledger in existing drawer when On, `NOT RECORDED` when Off | **PASS with m4** | My run at both viewports: `hasFloorHighlight:true`; 409 `EconomyInsufficientCash` plus UI Approve disabled, with the verdict "Expected timber revenue (€26.37) cannot pre-pay work, and no borrowing or overdraft is allowed" (screenshot `vp1920-upfront-cash-blocked`). Hours `39.8 / 40 hrs` after Execute, then `40 / 40 hrs` after Advance. Replay `replayed:true` with cash unchanged at `€9,486.08`. `Financial Ledger — Year 0` card, 14 rows, inside `#history-drawer`, with one History button. Off: `hasFinNotRecorded:true`, `hasEuroGlyph:false`. The Off disclosure is Core's existing `FinancialLedger … NOT RECORDED` text, which contains the word "cash" and no `€`. That is acceptable under the amended spec, and the handoff discloses it. |
| **2B2a-06** Advance, Skip, overlays, inspectors and saves still work; zero console, CSP, 404 or 5xx errors | **PASS** | Step 2 Edge 67/67, Save Edge 63/63, multiarea 363/0. Economy Edge `console=0, csp=0, net=0` on all three runs (with the m4 caveat about initial-load capture). Saves are explicitly blocked while Money is On with `SaveEconomyRequiresV3` (HTTP 200 list plus notice; save button disabled). This is a by-design deferral to 2B-2b, and the toggle dialog warns about it. |
| **2B2a-07** no edits to forbidden paths | **PASS** | §3 forbidden-path row. Only Ui server/frontend files, new tests, specs and the handoff changed. |
| **2B2a-08** (spec) single drawer, financial rows when On, clean audit | **PASS** | As in -05 and -06. |
| **2B2a-REG** | **CONDITIONAL** | Builds 0W/0E, Core 291/291, Save 50/50 (apart from the pre-existing NB1c flake), multiarea 363/0, ui-smoke 250 with known `{L01,L02,L06,J31}`, Save Edge 63/63, Step 2 Edge 67/67, Economy Edge PASS at both viewports, 5/5-equivalent Off payload budgets via the G5 sweeps. **Canonical `2d-gate.sh` exit 0 not achieved (M1).** |

### Security and architecture (D3)
- Host, Origin, capability, CSP and body-size guards are untouched (0 diff lines).
- New request records go through the existing `ReadMultiRequestAsync`, which enforces an exact member set and bounded strings. `kind`, `targetKey` and `executor` were added to `RequireBoundedText` (`MultiRoutes.cs:241`).
- Unknown enum values give typed refusals, `EconomyOrderUnknown` and `EconomyExecutorUnknown` (`MultiApi.Commands.cs:149-151`), via `RefusableMulti` with no state change.
- Single-area `/api/*` is unchanged; ui-smoke shows the baseline failures only.
- No external URLs, `eval`, `new Function` or `innerHTML` in the added code.
- Session swap: see m3.

### Hidden scope or dead code
- None material. `resetSessionUiState` is a straight extraction of the former `resetForLoad` body plus the economy fields, and Save Edge 63/63 confirms load still works.
- `ValidateFenceNotice` extraction from `ReadMultiHistoryYearAsync` preserves the same messages.
- The fresh-session genesis in `MultiRoutes.cs` duplicates `Program.cs`, which falls outside the D3 fence. This is a minor maintainability note, not a defect.

## 7. Residual risks

1. G3, G4, G6 and G7 have never run in canonical form on this candidate (M1). G5 and G7 are covered substitutively.
2. Money-On sessions cannot be saved or loaded until Save v3 (2B-2b). Any Money-On progress is lost when the process closes or Money is toggled Off. The dialog and the saves notice disclose this honestly, but it is a real player-facing limitation.
3. m1 mode misreport under a concurrent-mutation race or an unexpected `P1Refusal` inside `TryBuildEconomySnapshot`.
4. m3 swap and command concurrency, for raw API callers.
5. The NB1c Save flake (pre-existing) will keep failing canonical gate runs intermittently, as gate run 1 shows. The integrator should rerun on an NB1c failure rather than treat it as a regression.
6. Each Economy-On view, command and skip payload computes a fresh `QuoteWorkPlan`. Budgets were proven for Money Off only. Money-On payload size and latency at late years are NOT TESTED.

## 8. Conditions for integration

- **C1 (integrator, required):** retire or re-baseline `build/2d-gate/s0a-multi-asset-identity.json` on `integration/2d-sim`, either by moving it to a post-S0c mode or recapturing it against the merged `multi.html`/`multi.css`. Then the full `build/2d-gate.sh` (G0–G7, `--p0ref-dir` seed) must reach exit 0 or the documented `PARTIAL` (5) on the merge candidate. Rerun once if NB1c flakes.
- **C2 (documentation, required):** correct `specs/multi-2d-phase2b-economy-ui-v1/tasks.md` T-06 and T-08, and the handoff's push status, to match reality. This can land in the integration commit or in a single doc-only repair.
- **Recommended, not blocking:** m1 (explicit economy-enabled flag) and m5 (keep the honesty badge readable at 1280), in 2B-2b.

*Original decision at `559c0690` (superseded by the delta re-review below):* APPROVED WITH CONDITIONS — repair cycles allowed for this lane: 1 remaining.

---

## 9. Delta re-review (repair pass R1-R6)

| Field | Value |
|---|---|
| Reviewed-at | `559c0690b910075d243ca17c9737c16a9cc49241` |
| Repaired HEAD | `ff13097e281936961068373c9161b1492fcf1ec4` = code/tests `cf7e559a7f6734bdb3c0302cc8765c053e0f217f` + docs/spec `ff13097e` (docs-only: handoff, `plan.md`, `spec.md`, `tasks.md`) |
| `origin/feat/…-v1` | `ff13097e` (checked with `git fetch` + `rev-parse`) |
| Reviewer | `claude-opus-5-5` (same independent reviewer; did not author the repair) |
| Evidence | `…/CCF-Multi-2D-Phase2B2a-Evidence-20261126/review-delta/` (first run: full gate set, reused) and `…/review-delta2/` (this retry: delta diffs, re-run suites, `scripts/run-retry-suites.sh`) |
| Run context | The first delta run finished every gate suite at `ff13097e`, then hit a provider API error before code review. This retry reuses that evidence, re-executes 2 suites independently, and completes the code-level R1-R6 review. |

### 9.1 Scope of the delta

- `git diff --stat 559c0690..ff13097e`: 16 files, +502/−260. Code delta `559c0690..cf7e559a` touches only:
  - `src/Ui/MultiApi.{Commands,History,Saves,View}.cs`
  - `src/Ui/multi/{api,commands,main,store}.js`, `panels/work-plan.js`, `multi.css`, `multi.html`
  - `tests/phase2b2a-economy-ui/drive-economy.mjs`
- `cf7e559a..ff13097e` touches only the handoff and the three spec files.
- Every hunk maps to one of R1-R6. Nothing outside the six findings changed. The `multi.css` short-desktop spacing tweaks exist only to keep the Stage 4 R3 and NB-2 folds intact after the R6 badge move.
- **Forbidden paths:** `git diff --name-only 6d72c305..ff13097e` has 22 paths, none under `src/Core`, `tests/Core*`, `tests/Phase1.Save.Tests`, `src/Cli`, `build/**`, `SitkaP1.Ui.csproj`, `wwwroot`, `Assets/`, `Packages/`, `ProjectSettings/` or `Docs/Project/`. `git diff --stat 6d72c305 ff13097e -- src/Core tests/Core.Tests tests/Phase1.Save.Tests src/Cli ../MultiRackSitkaLab build` is empty. The P0 tree object is `f212c8f6…` at both commits. **PASS**
- **Modules:** `git ls-tree -r --name-only ff13097e -- src/Ui/multi | grep -c '\.js$'` = `14`. **PASS**
- **Tokens:** the 64-hex scan of `6d72c305..ff13097e` finds only `9a99f781…34b3`, the public P0 seed hash. The new `sed` redaction lines in the Edge runner add no token. **PASS**
- **CSP/guards:** 0 changed code lines matching CSP, `script-src`, Origin, AllowedHost, `MaxBodyBytes` or capability. The only matches are in handoff prose and the existing token-redaction regex. **PASS**
- **No weakened or skipped assertions:** in `drive-economy.mjs`, every changed `check(...)` gains conjuncts and none loses one. `grep -c 'check(true'` = `0`. **PASS**

### 9.2 R1-R6 verification

| ID (orig.) | Verdict | Evidence (HEAD `ff13097e` unless noted) |
|---|---|---|
| **R1 / M2** (+m6) | **FIXED** | <ul><li>`specs/multi-2d-phase2b-economy-ui-v1/tasks.md:9`: T-06 is now `[ ]`, with a true-status sub-bullet: G0/G1 PASS, G2 #1-#7 PASS, #8 S0c FAIL, G3-G7 and B NOT RUN, plus the carried integrator condition.</li><li>`tasks.md:12`: T-08 is ticked with `559c0690` and `cf7e559a` and a fast-forward push. `origin` is at `ff13097e`.</li><li>Handoff `:5` says "pushed to `origin/…`". The old "LOCAL ONLY" text and the "Push held" not-tested line are gone.</li><li>Handoff `:90` cites `implementer-repair/2d-gate-console.log:20-23`. I read that file: line 20 `G2: running S0a /multi asset identity sub-check...`, line 21 `S0c mode: multi.css no longer matches T0 R_css`, line 22 `ERROR [G2]: …failed:`, line 23 `GATE G2 FAIL`. The artefact exists and the citation is exact.</li><li>The token-scan row now discloses the P0 hash.</li></ul> |
| **R2 / m1** | **FIXED** | <ul><li>**Server:** every `/multi` payload record that carries a view now has `bool EconomyEnabled`, which is not null-ignored, so it is always emitted. The covered payloads are view, command, skip, load, cell, hit, competition, economy-session, economy-view and quote.</li><li>The value is read under the swap lock from `pair.Session.SelectedModels.IsEconomyEnabled`, with no inference (e.g. `MultiApi.Commands.cs:19-28`, `MultiApi.View.cs:39-47`).</li><li>`Economy` keeps `JsonIgnore(WhenWritingNull)`, so an Off payload gains only `"economyEnabled":false` and no economy block. Edge confirms this: `GET api/multi/view emits explicit economyEnabled=false and omits "economy"` passes on all 3 runs.</li><li>**Client:** `store.js:103-104` `isEconomyEnabled()` returns `state.economyEnabled === true`. `api.js:41` takes the boolean only from the payload, and `economy` is kept only when it is enabled.</li><li>An On payload with a null snapshot renders `Money: On` with "Unavailable — Re-read" (`main.js:79`) and a "Re-read needed" quote panel. Approve/Execute stay client-blocked (`api.js:271` `!state.economy` ⇒ blocked).</li><li>An On session can no longer render `Money: Off`.</li></ul> |
| **R3 / m2** | **FIXED** | <ul><li>A grep for `€ ?[0-9]`, `40[- ]hour`, `40 hrs`, `40.0 h`, `890 kg`, `2,500`, `12,000` and `45/hr` over `src/Ui/multi/**`, `MultiApi.*.cs` and `MultiRoutes.cs` gives 0 hits, against 13 at `559c0690` (api 1, commands 1, html 1, work-plan 2, View.cs 8).</li><li>Values now come from `EconomyOptions` or quote fields:<ul><li>disclaimer `MultiApi.View.cs:545` (`options.MinimumHarvestJobCents`)</li><li>floor highlight `:698` (`h.MinimumJobFloorCents`)</li><li>material labels `:849` (`line.MaterialCostCents`)</li><li>ledger labels (`options.*`)</li><li>allowance copy `:887`</li><li>new `AnnualLandownerAllowanceDisplay` and `LandownerAllowanceTooltip`</li><li>`work-plan.js` uses `h.timberSaleRevenueDisplay`</li></ul></li><li>Unavailable values are omitted, not hard-coded: the toggle dialog says "the engine's opening cash balance". The static `multi.html` tooltip drops the number. The ledger and allowance fallbacks drop the figure when `options` is null.</li><li>The honest disclaimer survived word for word, apart from the formatted floor.</li><li>Nit n2 below.</li></ul> |
| **R4 / m3** | **FIXED** | <ul><li>`lock (multiSwapSync)` now covers capture, invocation and the synchronous snapshot in `MultiCommandAsync` (`Commands.cs:19-28`). That covers every mutating command, including `SetMultiOrderExecutorAsync`, `Commands.cs:178`.</li><li>It also covers the swap (`:96-135`), quote (`:150-172`), economy view (`:53-80`), skip (`History.cs:59-74`), preview (`:19-43`), economy history year (`:220-260`), view, cell, competition and hit (`View.cs:39,460,484,518`), Save `CaptureSave` (`Saves.cs:92-96`) and Load commit (`Saves.cs:139-166`).</li><li>A brace-matched scan of all 13 lock bodies finds no `await`, `WriteJson*`, `saveSlots`, `File.` or `Encode(`. HTTP request reads happen before the lock, and serialization and HTTP writes happen after it.</li><li>**Deadlock:** the order is always `LockSlot` → `multiSwapSync` → Core engine lock. No path takes `multiSwapSync` and then `LockSlot`, and Core cannot call back into UI locks. There is no cycle.</li><li>**Load:** `saveSlots.Load` (disk) runs outside the lock. The commit then refuses a swapped session with typed `PlayerSessionMismatch` "…nothing changed" (`Saves.cs:142-145`, `ReferenceEquals(activePair, capturedPair)`).</li><li>Unlocked read-only single-pair reads remain: manifest, history index/year, structure and saves list. See nit n3.</li></ul> |
| **R5 / m4** | **FIXED** | <ul><li>`check(true` count is 0. Line 454 is replaced by a real check (`drive-economy.mjs:476-481`): DOM year `1`, `40 / 40 hrs`, and server `remainingLandownerMinutes === annualLandownerAllowanceMinutes` with `usedLandownerMinutesThisYear === 0`. My run observed `server=2400/2400 min`.</li><li>`Network.enable` is now sent before `Page.navigate` (`:578`, next to Page/Runtime/Log).</li><li>The initial-load assertion (`:216-217`) fails on any 404, ≥500 or non-cancelled `loadingFailed`, console error or CSP violation. So a 404'd module fails it.</li><li>Assertion count went from 71 to **74** (25 + 43), counting the new real checks (initial load ×3, the replaced `check(true)` ×2, and other strengthened conjuncts).</li><li>Nit n1 below.</li></ul> |
| **R6 / m5** | **FIXED** | <ul><li>`#experiment-badge` moved to a full-width row under the single-row brand/actions bar (`multi.html:52`, `multi.css:75` `flex: 1 0 100%; white-space: normal; overflow: visible`).</li><li>New `checkLayout` conjuncts (`:167`): `scrollWidth <= clientWidth`, right edge in the viewport, and on ma-1-mixed the text must contain "not a surveyed Irish forecast".</li><li>My screenshots `review-delta2/gates/economy-edge/vp1280-work-plan-quote-pending-1280x720.png` and `vp1920-…-1920x1080.png` show the whole badge "Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)" with no ellipsis.</li><li>Layout passes at both viewports: `zero h-scroll (1280<=1280 / 1920<=1920)`, `3 cols (<=3)`, 1 History button.</li><li>The Step 2 Edge run at `ff13097e` passed `ma1 1280x720 R3 fold preserved (… addBottom 641 … fits:true)` and every NB-2 1280×720 and 1920×1080 fold check.</li></ul> |

### 9.3 Anomaly resolutions

**A1 — Save 49/50 (NB1c) in the first delta run.**
- The exact failing case is `review-delta/gates/save.log:47`: `FAIL NB1c: a generation read and written inside the slot scope is unique per save — the second save carries the next generation after the first: expected 1, got 3`.
- `tests/Phase1.Save.Tests/Phase1Save.Tests.csproj:12` references only `src/Core/SitkaP1.Core.csproj`.
- `git diff --stat 6d72c305 ff13097e -- src/Core tests/Core.Tests tests/Phase1.Save.Tests` is empty. The branch touches neither Core nor the Save tests, so the compiled test and its dependency are identical to base.
- In the same delta run at the same HEAD, the gate's own G2 Save run passed `50/50` (`review-delta/gates/2d-gate/g2-save-tests.log`).
- Pre-repair, my 8 isolated reruns gave 7 × 50/50 and 1 × NB1c, and the implementer's run gave 50/50.
- The signature matches exactly the documented `LockSlot` ordering race, about 7.5% at T0, and higher under host load. The load average was about 3.8-4.7 during these runs, with other lanes active.
- **Resolution: a base-side flake, not a branch regression.**

**A2 — `build-output-safety` "FAIL C8-rebuild … `.p0ref: DIFFERENT`" against the implementer's "PASS (HOST-NA)".**
- **There is no real discrepancy.** Both runs produced the same raw result and the same gate verdict:
  - My `review-delta/gates/2d-gate/g2-build-output-safety.log:7-9`: `FAIL C7-cold-build: exit 0 sha 6d216099…`, `FAIL C8-rebuild: exit 0 sha 6d216099…`, `previous .p0ref: DIFFERENT`. My `gate.log:17`: `build-output-safety.sh PASS (C1-C6+P0 PASS; HOST-NA: C7-cold-build,C8-rebuild; .p0ref restored & re-verified)`.
  - The implementer's `implementer-repair/2d-gate/g2-build-output-safety.log:7-9` and `implementer-gate/…:7-9` have the same FAIL C7/C8 and DIFFERENT lines, with sha `50e6115a…`, and the same `gate.log:17` PASS (HOST-NA).
  - The earlier comparison set the raw sub-log against the gate summary line.
- **How HOST-NA is decided:**
  - `build/2d-gate.sh:743`: when `P0_MODE` is `PRE-SEEDED*` (the `--p0ref-dir` seed path), the gate backs up the verified seed and runs `tests/build-output-safety.sh`.
  - `:755-761` restores the seed and re-verifies sha256 = `9a99f781…34b3`.
  - `:763-776` requires PASS for C1-C6 and P0, and accepts FAIL only for the labels listed in `build/2d-gate/baseline-counts.json:43-46` `hostNaLabels` = `C7-cold-build`, `C8-rebuild`.
  - `:780` records "frozen 8.0.133 pin not cold-reproducible on this host".
- **Why C7/C8 fail on this host:**
  - `tests/build-output-safety.sh:83-86,92-93` require a cold rebuild of the P0 DLL to hash to the pinned `9a99f781…`.
  - On this host the cold rebuild yields a different hash each run (`50e6115a…` for the implementer, `6d216099…` for me, same worktree path). That makes it a host build-reproducibility property, and `DIFFERENT` (`:97`) is the expected consequence.
- **P0 is untouched by the branch:**
  - The P0 tree is `f212c8f6…` at base and HEAD.
  - The P0 tracked-file digest `9e218ca7…` is unchanged before and after in both runs (`PASS P0` line).
  - The `.p0ref/SitkaLab.Core.dll` now in the feature worktree hashes to `9a99f781…34b3`, the same as the seed.
- **Resolution: (a), a host/P0 reproducibility property unrelated to this branch.**

### 9.4 Regression matrix at `ff13097e`

`R` = re-executed by me in this retry (`review-delta2/run.log`, start 19:57:51Z, `dotnet 8.0.133`, the existing `sitkap1-ui.dll` built 19:27Z at this HEAD, no newer `src/Ui` or `src/Core` source). `U` = reused from my first delta run (`review-delta/run.log`, same HEAD, same runner `review-delta/scripts/run-delta-gates.sh`), with logs re-read.

| Gate | Command (abbrev.) | Observed line | Src | Result |
|---|---|---|---|---|
| SLN build | `$DOTNET build MultiRackSitkaP1Lab.sln -c Debug --disable-build-servers` | `0 Warning(s)` / `0 Error(s)` RC 0 | U | **PASS** |
| UI build | `$DOTNET build src/Ui/SitkaP1.Ui.csproj …` | `0 Warning(s)` / `0 Error(s)` RC 0 | U | **PASS** |
| Core | `SitkaP1.Core.Tests.dll` | `291/291 passed, 0 failed` | U | **PASS** |
| Save (direct) | `Phase1Save.Tests.dll` | `49/50 passed, 1 failed`: NB1c `expected 1, got 3` (`save.log:47`) | U (re-read) | **FAIL — base-side flake (A1)** |
| Save (gate G2, same run) | `2d-gate.sh` G2 | `Phase1.Save.Tests PASS (50/50 passed, 0 failed)` | U | **PASS** |
| Multiarea smoke | `DOTNET=$DOTNET tests/ui-smoke-multiarea.sh <ev>` | `OVERALL: PASS (363 passed, 0 failed)` RC 0 | **R** (and U: same) | **PASS** |
| Single-area smoke | `tests/ui-smoke.sh` | `OVERALL: FAIL (250 passed, 4 failed)`, exactly L01, L02, L06, J31 | U | **PASS-BASELINE** |
| Save Edge | `tests/phase1-save-ui/run-edge-e2e.sh` | `OVERALL: PASS (63 passed, 0 failed)` | U | **PASS** |
| Step 2 Edge (G5 sweeps, R3/NB-2 folds) | `tests/phase1-time-history-ui/run-step2-edge.sh` | `OVERALL: PASS (67 passed, 0 failed)`; ma-1 max raw `516984` / eco `43320`; ma-1-mixed max raw `523398` / eco `49685` | U | **PASS** |
| Economy Edge, both viewports | `DOTNET=$DOTNET tests/phase2b2a-economy-ui/run-economy-edge.sh <ev>` | `VIEWPORT 1920x1080: 25 passed, 0 failed` / `VIEWPORT 1280x720: 43 passed, 0 failed` / `OVERALL: PASS (74 passed, 0 failed)` RC 0 | **R** (and U: same) | **PASS** |
| Canonical gate | `build/2d-gate.sh --base 6d72c305… --p0ref-dir …/p0ref-seed --headless-only <ev>` | G0 PASS; G1 PASS (seed `9a99f781…`); G2: Core 291/291, Save 50/50, multiarea 363/0, ui-smoke `PASS-BASELINE-DRIFT`, cli-safety 23/0, build-output-safety PASS (HOST-NA C7/C8), p0-isolation PASS (13/13); then `S0c mode: multi.css no longer matches T0 R_css` → `GATE G2 FAIL`; G3-G7 and B NOT RUN; `NETWORK ISOLATION: PROVEN`; `VERDICT: FAIL (exit=1)` | U (not re-run, as the brief allowed) | **FAIL (S0c lock only)** |
| G3, G4, G6, G7, B (canonical) | — | not reached | — | **NOT TESTED** |
| Static: forbidden paths / 14 modules / tokens / CSP / `check(true)` | §9.1 | as §9.1 | R | **PASS** |
| Feature worktree untouched by the suites | `git status --short` before and after the retry | empty / empty | R | **PASS** |

### 9.5 Payload-headroom judgement

- Off-mode raw view peaks across Years 0-30 against the 524288 B cap:
  - ma-1: `516984 B`, headroom 7304 B (1.4%)
  - ma-1-mixed: `523398 B`, headroom 890 B (0.17%)
  - Pre-repair: `524279` / `523375`
- The new always-emitted `"economyEnabled":false` adds about 23 B per payload. At ma-1 it pushed the bounded serializer down one projection-compaction tier, so the peak *fell*.
- The cap is enforced by that compaction ladder, not by luck. G5 also asserts ≥1 event and ≥1 annual record retained, and that passes.
- **Judgement: acceptable for integration.** The margin is thin, though, and every additive Off field now costs fidelity at late years.
- Money-On late-year payload size and latency remain **NOT TESTED** (residual risk 6 in §7 carries forward). 2B-2b should add an On-mode G5-style sweep.

### 9.6 Findings after repair

Totals: **BLOCKER 0 · MAJOR 0 · MINOR 0 · NIT 4**. No in-lane defect is open.

- **n1 (nit):** `drive-economy.mjs:217` `initialAssetOkCount >= 16` while 17 assets are observed. A module that is never requested would not trip the count, but any 404 or load failure still trips `badNetwork`. Suggest `>= 17`, or assert the 14 module URLs by name, in 2B-2b.
- **n2 (nit):** `MultiApi.View.cs:602`: the allowance tooltip formats a literal `FormatEuros(0L)` "cash wage". It states a modelling rule (landowner time has no cash wage) and is not a tunable `EconomyOptions` value, because no wage option exists in Core. Acceptable. Move it to a Core-owned constant if a wage option is ever added.
- **n3 (nit):** a few read-only handlers still read `CurrentMulti` once without the swap lock:
  - manifest (`View.cs:15`)
  - history index (`History.cs:138`) and `HistoryYearDetail` (`:159`)
  - structure (`:270`)
  - saves list (`Saves.cs:22`)
  
  Each reads one pair (no torn frame) and mutates nothing, so a racing swap can only return the old session's data, which carries the old `sessionId`. That is outside the R4 brief (commands plus swap) and consistent with it.
- **n4 (nit):** the ma-1-mixed Off headroom of 890 B (§9.5).

### 9.7 Residual integrator conditions

- **C1 (integrator, required, carried):** retire or re-baseline `build/2d-gate/s0a-multi-asset-identity.json` (mode `s0c-css-plus-html-delta`) on `integration/2d-sim`. Then run the full `build/2d-gate.sh` G0-G7 (`--p0ref-dir` seed) on the merge candidate to exit 0 or the documented PARTIAL. G3, G4, G6 and G7 have never run canonically on this candidate.
- **C2 (closed):** the documentation bookkeeping is corrected (R1).
- **Documented base-side flake:** NB1c (A1). Re-run once on that exact signature. Gate with no concurrent load where possible.
- **Host note:** build-output-safety C7/C8 are HOST-NA under the pre-seeded P0 path (A2). This is not a lane condition.

---

INTEGRATION DECISION: APPROVED WITH CONDITIONS — All six repair findings R1-R6 are verified fixed at ff13097e with no in-lane defect remaining; the only open items are the integrator-side retirement of the stale S0c asset-identity lock followed by a full G0-G7 gate pass, and the documented base-side NB1c Save flake — repair cycles remaining for this lane: 0
