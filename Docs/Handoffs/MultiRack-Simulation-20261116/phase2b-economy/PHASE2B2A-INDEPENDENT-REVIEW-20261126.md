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

---

INTEGRATION DECISION: APPROVED WITH CONDITIONS — The economy UI satisfies the amended D1b/D2a/D3/D4 scope with zero JS money arithmetic, a contractor-only Fell, an honest confirmed Money toggle and green functional suites, but the canonical gate is blocked by the stale S0c asset-identity lock (integrator retirement plus a full G0–G7 pass required) and the task checklist misstates gate and push status — repair cycles allowed for this lane: 1 remaining
