# Multi #849 — Playable 2D Stand Map: PM-T05…PM-T08 implementation result (author Tier-1)

- **Labels (current, after the repair in §0):** **IMPLEMENTED AND REPAIRED (author)** · **author Tier-1 PASS on the repair SHA `d2c04cbb`** · independent Tier-2 of the earlier candidate `1905433f`: **NEEDS CHANGES**; **Tier-2 re-check of `d2c04cbb` NOT YET RUN** · **NOT QUALIFIED** · **NOT INTEGRATED** · **NOT RELEASED** · **NOT USER ACCEPTED**. No Manager acceptance is claimed or implied.
- **Task:** CCF [#849](https://github.com/jermur788/CCF/issues/849), spec `specs/multirack-sitka-2d-playable-standmap-v1/` (MULTI-STANDMAP-PLAYABLE-2D-V1, APPROVED — Manager #730, UI-A), tasks.md rows 8–15 (PM-T05…PM-T08).
- **Author:** Claude Opus 5.5 (single exclusive author for #849; model identity is worker-reported). Commits carry the explicit author identity `Claude Opus 5.5 <noreply@anthropic.com>` because the lane's configured Git identity is the non-author parent.
- **Date / host:** 20 November 2026, DESKTOP-DL1BQI7 (WSL2 Ubuntu `jer`).
- **Lane:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Source` (WSL-native, LF, detached), as named in the task packet. The live board row names the T3 Windows worktree as the registered writer checkout; this run used the WSL lane the task packet designated. Pre-write checks: host name, no `dotnet`/MSBuild process, no `index.lock`, HEAD `30fa269c`.
- **Predecessor:** PM-T01…PM-T04 (Core facade and tests) at `30fa269cc9ec8287a15df364f41fba2bfe48f40f`, parent-verified per the task packet.

## 0. Repair addendum — Tier-2 findings D1–D5 (author, 20 November 2026)

Sections 1–13 below are the original record for `1905433f`. Where this addendum differs, **this addendum is current**.

### 0.1 What this answers

- **Review:** the independent Tier-2 by GPT-6.1 Sol, [`Multi-StandMap-Playable-2D-INDEPENDENT-TIER2-REVIEW-20261120.md` at `7fd3b72c`](https://github.com/jermur788/CCF/blob/7fd3b72cd3e50b0f33574210a6417e651c87312e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-INDEPENDENT-TIER2-REVIEW-20261120.md): **NEEDS CHANGES**, 3 MAJOR (D1–D3), 1 MINOR (D4), 1 NIT (D5).
- **Scope:** one bounded repair cycle on the same candidate. No new scope, no new refusal code, no relaxed ecological or A-4 requirement, no weakened test.
- **Lane:** the same WSL lane, detached at `4e95e787`, clean, no `dotnet` process and no `index.lock` before the first write.
- **Commits:** `d2c04cbbca380b4a5076807c72f19632bf2543a8` (the source repair; pushed as a fast-forward `4e95e787..d2c04cbb`), then this handoff commit. Nothing was forced, amended or rebased.
- **Candidate source SHA for the Tier-2 re-check: `d2c04cbbca380b4a5076807c72f19632bf2543a8`.**

### 0.2 Changed paths, `4e95e787..d2c04cbb` (all under `Tools/MultiRackSitkaP1Lab/`, all on the 15-path list)

| Path | Added / removed | Blob at `d2c04cbb` | Defects |
|---|---|---|---|
| `src/Core/AnnualContinuation.cs` | +10 / −1 | `a8f2aabb462cb1ab91c98e1a5efc9bf6c385519b` | D3 |
| `src/Core/PlayerSessionView.cs` | +2 / −0 | `11be0cdb23352aefcb39ca3c0c1ffa7b3fcf2955` | D5 |
| `src/Ui/wwwroot/app.js` | +302 / −26 | `f3e5f1c309a50aeb7acbb51b72eb774aea9857d1` | D1, D2, D4, D5 |
| `src/Ui/wwwroot/index.html` | +14 / −0 | `005e1e48b841e1cff1f472a8ce96aed105e3358d` | D1, D4 |
| `src/Ui/wwwroot/app.css` | +9 / −1 | `e44ed374b436473f1dbf586e99202a03fcd2f1f0` | D2, D4 |
| `tests/Core.Tests/PlayerSessionTests.cs` | +72 / −3 | `53067f1703ddb172b6ec51ca78779de6f6333959` | D3, D5 |
| `tests/ui-smoke.sh` (mode 100755 kept) | +506 / −4 | `fc511fba6697a4ff004c7e9a5dfc310abdc4bb70` | D1, D2, D3, D4, D5 |
| `README.md` | +23 / −11 | `560c5f67ba2f76a2e77f82956b1fe033a183365c` | all |

Eight paths, 938 lines added, 46 removed. Unchanged since `1905433f`: `PlayerApi.cs`, UI `Program.cs`, `SitkaP1.Ui.csproj`, `PlayerWorkPlanAdapter.cs`, `PlayerMapScene.cs`, `PlayerMapSceneTests.cs` and the test registration `tests/Core.Tests/Program.cs`. No fixture, golden, `.p0ref`, `build/`, other csproj, `.sln`, CLI, existing test, `Assets/**`, Unity or Windows-build change.

### 0.3 Each defect: root cause, change, tests

**D1 (MAJOR) — the confirmation could execute an undisplayed plan.**
- *Root cause:* Confirm read the PlanId, key and fence from the latest page view, which a pending re-read could replace while the dialog stayed open.
- *Change (`app.js`, `index.html`):*
  - Execute opens the dialog on a frozen copy of the displayed plan and fence: session, full source token, plan-state revision, PlanId, minted key, year, and the exact IDs and reasons. The dialog renders only that copy, now including its PlanId.
  - Confirm sends exactly that copy and nothing read from a newer view.
  - Any newly applied view that no longer shows that plan under that fence withdraws the confirmation: the dialog closes, nothing is sent, and the re-read lock below is set, so a further successful re-read is required. The page never substitutes the current plan.
  - Confirm pressed while a request is still on its way sends nothing and leaves the dialog open; the arriving view re-checks it. Back and Escape discard the copy.
- *Tests:* smoke J03–J15 (the reviewer's race with a held re-read and a second client of the same session); real Edge R01–R07.

**D2 (MAJOR) — "outcome unconfirmed" still allowed commands.**
- *Root cause:* only the alert changed; no page-wide state gated the commands.
- *Change (`app.js`, `app.css`):* one re-read lock, set by an unconfirmed outcome or a withdrawn confirmation.
  - Every engine command is disabled: Add, Remove (inspector and list), Approve, Cancel, Execute, the dialog's Execute, Advance and the consent's advance.
  - Each handler refuses first and says "Not sent: …" in the alert.
  - `call()` refuses every POST, including a map hit-test, so no request but a read can leave the page.
  - Only a successful read that began after the latest lock lifts it. A failed read keeps it.
  - Disabled buttons have a dashed, muted style and are skipped by Tab.
- *Tests:* smoke J16–J28 (a committed Add and a committed Execute whose answers are lost; every handler activated, including on disabled buttons; zero requests captured; a failed re-read keeps the lock; a successful re-read lifts it); real Edge U01–U05.

**D3 (MAJOR) — an unminted execution key could poison a later plan.**
- *Root cause:* the fenced first execution accepted any non-blank key.
- *Change (`AnnualContinuation.cs`):* on the first-execution path, after the replay lookup, the fence and the PlanId check, and before any mutation, the key must equal the engine-minted key of the approved plan. Otherwise the result is the existing `WorkExecutionKeyConflict`, with zero mutation.
  - One private helper now mints the key for both the view and this check.
  - The replay rule (same key, PlanId, original source and session) and the wrong-PlanId refusal are unchanged.
  - No new public refusal code: the §6.6 set is unchanged.
- *Tests:* Core **PS-20**; smoke E00, E00b, D02f and D03.

**D4 (MINOR) — map targets below 44 × 44 CSS px. What I chose, and why it meets A-4:**
- *Choice:* a zoom-and-pan map pane (`app.js`, `app.css`, `index.html`).
  - The SVG sits in its own scrolling pane. The pane has `tabindex="-1"`, so it is not a Tab stop (A-2 still skips the map).
  - Seven ordinary buttons zoom and pan: *Zoom out*, *Zoom in*, *Whole area* and four *Pan* buttons. They work by keyboard. Drag and scroll also pan.
  - The map opens at **64 CSS px per metre** when the page is one column (below 900 CSS px wide: 360 px, and 200 % zoom of any window up to 1,799 px) or when the device pixel ratio is 2 or more (200 % zoom of a wider window on a standard display). A two-column desktop at ordinary zoom opens on the whole area. Until the player chooses a zoom, a resize (browser zoom) re-applies that default.
  - Choosing a tree in the table scrolls the zoomed map to it.
  - A visible, polite note states the current smallest drawn target in pixels, or that the whole-area view has targets under 44 px.
- *Why it meets A-4:*
  - At 64 px/m the smallest drawn item, the 0.35 m stump marker, is 0.7 × 64 = 44.8 px. Every crown in this fixture is far larger.
  - Crown radii and the stump constant are unchanged. The server still hit-tests the true discs, and the S-5 drawn-versus-hit cross-check is unchanged. No hit area is enlarged, so a click can name only a tree whose drawn disc is under the pointer.
  - The page itself stays one column at 360 and 640 CSS px, with no page-level horizontal scroll. The core actions are outside the scrolling pane.
  - The table remains the authoritative accessible path.
  - Measured in real Edge: §0.7.
- *Tests:* smoke J02 (the default scale and the stated 44.8 px); real Edge L360, L640, L960, L1280 and K01–K04.

**D5 (NIT) — "1 named trees".**
- *Change (`PlayerSessionView.cs`, `app.js`):* a new Core label `ExecuteConfirmationOneTree` = "Fells exactly {n} named tree in Year {y}, or refuses all". The page uses it when the confirmed plan has exactly one order.
- *Tests:* PS-18 now lists it (the exact label set is still enforced); smoke J04.

### 0.4 New tests and totals

| Where | New or changed | Total on `d2c04cbb` |
|---|---|---|
| Core runner | **New:** `player PS-20: a first Execute must echo the engine-minted key; an unminted or future key is refused, zero mutation`. **Changed:** PS-18's exact expected label set gains `ExecuteConfirmationOneTree` | **92/92** (66 base + 20 PS + 6 MS). All 91 test names that passed on `1905433f` still pass |
| `tests/ui-smoke.sh` | **New:** E00, E00b, D02f (an unminted future key is refused, then the plan minted with it executes) and J01–J31 (the page's own served `app.js`, run by node in a small DOM stand-in against the real process) | **132 passed, 0 failed** (98 earlier + 34) |
| External Edge driver (not committed) | Phases L, K, R and U on the clone build | **43 PASS, 0 FAIL** |

The smoke now needs **node** (any recent version, no packages; `NODE` overrides). The stand-in is **not a browser**: it has no layout, CSS or real focus rules, so target sizes and focus behaviour come from the Edge runs.

### 0.5 Author Tier-1 — exact pushed SHA `d2c04cbb`, fresh WSL-native LF clone

- **Harness:** `~/.ccf-orch/standmap-p1-opus/tier1-repair-run.sh`, outside the repository; sha256 `e48bb0ee6aff571d0ee22b838624c33b72e9e8d44f933a041d6574664e8ea5c9`.
- **Logs:** `~/.ccf-orch/standmap-p1-opus/tier1-logs-d2c04cbb/` on DESKTOP-DL1BQI7. Its `SHA256SUMS` covers 56 files; that manifest's own sha256 is `a23a1e1a26a0ba273d667065935885c6fa09399c72785b70dca24093aba661f3`.
- **Tools:** SDK 8.0.134, ASP.NET Core 8.0.34, node v26.13.1 (WSL).
- **Clone:** partial and sparse, `core.autocrlf=false`; zero tracked lab files contain CR bytes.
- **Result: OVERALL TIER-1 PASS, 40 of 40 gates.**

| Gate | Result |
|---|---|
| D01, D01b | `4e95e787..d2c04cbb` touches exactly the eight paths of §0.2, all modifications, with no mode change |
| D02 | `4bb80973..d2c04cbb`: every lab path is on the 15-path list; everything else is this spec folder or handoff Markdown |
| D03 and unchanged-file gate | The test registration and the six UI and Core files listed in §0.2 are unchanged since `1905433f` |
| Protected | Fixtures, `AnnualRules.cs`, `WorkContract.cs`, `AnnualEngine.cs`, CLI, `.sln` (no UI), both existing csproj, the four base test files, `build/`, `global.json`, `Directory.Build.props`, `.gitignore`, the two linked Assets files and the P0 tree `f212c8f6`: all unchanged since the base |
| P0 reference | Copied from the same authorised pinned copy and verified four ways (`9a99f781…34b3`). **Not rebuilt**; `build/build-p0-core-ref.sh` not run |
| B01, B02 | Solution build with TreatWarningsAsErrors: **0 warnings, 0 errors**; the UI is not built by it |
| T01, T01b | Core runner **92/92**, exit 0. Every name that passed on `1905433f` passes, plus PS-20. Both `644 B-01` scans pass |
| T02 | Ten no-work digests identical to the `30fa269c` record (`digests.txt` sha256 `4e0e47a1…ccee`) |
| R01, R02 | UI restore from an empty local source succeeds; projects only |
| U01, U02 | UI build: **0 warnings, 0 errors**; exactly three embedded page assets. UI DLL `0fb212a990af20d7f4441003870e4449085700de66a8da5f79093e05af00fe7b`, Core DLL `aa5ffc2d50d8bb8de90dc841807d56c38f6de92075aef3a919afa597143e91f2` |
| S01, S02 | `tests/ui-smoke.sh`: **132 passed, 0 failed**, including J01–J31, E00, E00b and D02f |
| W01–W03 | P0 tracked-file digest unchanged; clone `git status` clean; `~/.aspnet` absent |

- **PS-01** in this run: 16 rounds and 1,170 atomic reads, with **Execute won 0, Advance won 16**, so only one race winner was observed in the exact-SHA run. (A pre-commit lane run saw Execute win 1 and Advance 15; that lane run is not evidence.) PS-14 observed 13 natural deaths after the Fell over ten years.

Key log hashes (sha256):

```text
897723a6f85813f1a8441e2a811152b854ded0003676f55b5061add823a5235f  run.txt
a7ccba67661c76f13fd060f507b36a02080759d5a39c29c4a767b5068f1e1647  build.txt
dfa0e2948220305a221789ef46efe52f707dee2f74fa0762c579a03f632e5912  tests.txt
4e0e47a13e6b366b9eadf0d4c47c479da21fa31e8f64f6e07125826dae91ccee  digests.txt
373c39ce068e75ed268f1b0a88376d579c3d0639e04eaf92e1d5a34ad78e743b  ui-restore.txt
8a27708dc05510677bf1b2fd5842eb8a60dfdec08b41a980f68a183198ab3ffe  ui-build.txt
9b2b45f0dbac3b280308f77325bab96de4c8ee0ef1d4765ddf2d7a9a596e9b39  ui-smoke/ui-smoke.txt
```

### 0.6 Negative controls — the new checks fail on the original defects

Each control is a scratch copy of the `d2c04cbb` clone with **one file put back** to `1905433f`:
- **Script:** `negative-controls/repair-negative-controls.sh`, outside the repository.
- **Logs:** `tier1-logs-d2c04cbb/negative-controls/` (`nc-run.txt` sha256 `8ed20cc8b62a1dc6b1c874714439ae9fd820eec4336abe6a2ac1281770c93b9d`).

| Control | What failed (expected) |
|---|---|
| NC-1: `app.js` from `1905433f` | Smoke **FAIL**. J02 (no zoom) and J04 ("Fells exactly 1 named trees") fail. J06, J07, J09 and J11 fail. **J10 captured the old page sending `POST api/plan/execute` for plan B**: the reviewer's D1. The witness then stopped at J12, because the old page has no lock alert to click, so NC-1 does not reach the D2 checks |
| NC-2: `AnnualContinuation.cs` from `1905433f` | Core runner **91/92: only PS-20 fails**. Smoke **FAIL**: **E00 got HTTP 200** (the old engine executed plan 2 under the future key `plan:12`), and **D03 then got 409** for the plan legitimately minted with that key: the reviewer's D3. The later steps diverge from there |
| Edge `unconfirmed` phase on the original `1905433f` build (UI DLL `a01a2194…`) | **U03 FAIL:** all eight engine commands enabled. **U04 FAIL:** a trusted click on Advance **sent `POST api/year/advance`**, and Tab reached Remove, Approve and Advance: the reviewer's D2 (`negative-controls/edge-1905433f/cdp-log.json`, sha256 `7d968d70a85b04899ecd2b5ddd969b8862ad522499d227c29beab57ff72dfea4`) |

### 0.7 Real-browser evidence, Edge 157.0.4341.60 (headless, CDP, trusted input), clone build of `d2c04cbb`

- **Driver:** `edge-repair-driver.mjs`, outside the repository, run with Windows node v26.13.1; sha256 `afa62c0f3186dff7d81abb334711506857065ea9c31dd5b99c6dde611a3f0d62`.
- **Launcher:** `repair-edge-run.sh`. It runs the README launch command from the clone, and the server ends with SIGINT and exit 0, with nothing on stderr.
- **Folder:** `tier1-logs-d2c04cbb/browser/` (`cdp-log.json` sha256 `cdea4e4cdfa2d583f90ec69e4716eb1dc0b458277ea59b8f38a18a66d9e0dcce`). An earlier exact-SHA run with a previous driver version is kept separately in `browser-prelim-earlier-driver/` and is not counted.
- **Viewports and zoom:** set with `Emulation.setDeviceMetricsOverride`. **200 % is emulated** as CSS size plus DPR 2; it is not the browser's zoom control.

| Phase | What was measured or witnessed |
|---|---|
| L360: 360 × 800, mobile, touch, DPR 2 | One column; no page-level horizontal scroll (360 = 360). Map at **64.00 px/m**. **All 98 crowns ≥ 248.53 × 248.53 px; both stumps 44.80 × 44.80 px.** The pane is 294 × 480 px and scrolls internally (2,432 px of map). All 114 visible controls outside the map are ≥ 44 × 44. A trusted **tap** on crown P30-056 selected exactly P30-056. With P30-060 selected by a tap, a trusted tap on the centre of the felled P30-050 **stump** selected exactly P30-050 |
| L640: 640 × 400 at DPR 2 (200 % of 1280 × 800) | One column; no horizontal scroll. 64 px/m. Crowns ≥ 248.53 px and stumps 44.80 px. The pane is 544 × 225 px. Controls ≥ 44. A trusted click selected an exact crown, and then the exact stump |
| L960: 960 × 540 at DPR 2 (200 % of 1920 × 1080) | No horizontal scroll; 64 px/m (the DPR rule). Crowns ≥ 248.53 px and stumps 44.80 px. Controls ≥ 44. **The layout has two columns**: the 900 px breakpoint is unchanged from the reviewed candidate (§0.9) |
| L1280: 1280 × 800 at DPR 1 | Whole area at 18.32 px/m. Smallest crown 70.23 px, **smallest stump 12.66 px** (A-4 does not name this condition; the note says so, and Zoom in gives 44.8 px). Controls ≥ 44 |
| K01–K04 | Enter on Zoom out gives 32 px/m, and Zoom in returns to 64. Enter on Pan east scrolls the pane. Tab from the last pan button skips the pane and the map (A-2). Enter on Whole area fits the map and shows the under-44 px note |
| R01–R07: D1, the reviewer's race | With a held re-read, the dialog listed only plan A (P30-099). A second client of the same session replaced it with plan B (P30-100). A trusted click on the dialog's Execute while the read was pending sent nothing. When plan B arrived, the confirmation was withdrawn, all commands were disabled, and focus moved to the alert. No Execute was sent, and A and B were both living on the server. After a re-read, a fresh dialog listed only B, and **exactly one Execute was sent, for plan B with its minted key** |
| U01–U05: D2 at 360 px | The Add committed on the server and its answer was dropped. The page drew nothing optimistic. All eight engine commands were disabled, focus was in the alert, and Re-read was the only button in it. Trusted clicks on Advance and Add sent **no request**, and 40 Tab stops from the alert reached no engine command. A successful re-read showed the committed Add and lifted the lock |

There was no uncaught page exception and no console error. Screenshots: `L360-360x800.png`, `L360-stump-selected.png`, `L640-640x400.png`, `L640-stump-selected.png`, `L960-960x540.png`, `L1280-1280x800.png`, `R01-dialog-plan-A-read-pending.png`, `R04-confirmation-withdrawn.png`, `R07-plan-B-executed.png`, `U03-unconfirmed-commands-disabled-360.png` and `U05-after-reread-360.png`, all hashed in `browser/SHA256SUMS`. A scan of all the evidence found no capability string.

### 0.8 Author statuses after the repair (not checklist ticks; Tier-2 decides)

| Item | Earlier Tier-2 disposition | Author status on `d2c04cbb` |
|---|---|---|
| PM-R08 | FAIL (D3) | AUTHOR-EVIDENCED: PS-20 and E00/D02f/D03; NC-2 shows the old failure |
| PM-X15 | FAIL (D1, D2) | AUTHOR-EVIDENCED for D1 and D2: J03–J28, R01–R07, U01–U05; NC-1 and Edge NC. Native zoom NOT TESTED |
| PM-R14, R-22, X-21 | PARTIAL / A-4 FAIL (D4) | A-4 targets AUTHOR-EVIDENCED at 360 and 200 % (emulated). **Still PARTIAL** overall: screen reader NOT RUN, native zoom and physical touch NOT TESTED |
| R-12, X-12, X-11 | FAIL / PARTIAL (D1) | AUTHOR-EVIDENCED: the confirmation is bound to the plan it shows, or withdrawn unsent |
| R-15, X-14 | PARTIAL (D3) | AUTHOR-EVIDENCED: first-use identity enforced and replay unchanged |
| R-26 | PARTIAL (D1, D2) | AUTHOR-EVIDENCED: withdrawal and lock alerts with one recovery action |
| PM-R11 | PASS with NIT D5 | AUTHOR-EVIDENCED: singular label (PS-18, J04) |
| All others | as in the review | Unchanged by this repair. PM-R15 and R-25 remain NOT VERIFIED; R-06 and X-05 are not PASS (Q-3); R-07, R-08, X-07 and X-09 are G1-disabled |

### 0.9 Still unverified, limits and open points

1. **Tier-2 re-check of `d2c04cbb` has not run.** This addendum does not claim the review is passed.
2. Screen reader **NOT RUN**. The browser's own 200 % zoom control is **NOT TESTED** (emulated only). Physical touch is **NOT RUN** (emulated). No human player session.
3. At 200 % zoom of a window **1,800 px or wider** (for example 960 CSS px from 1920), the page has two columns, because the 900 px breakpoint is unchanged. Its targets are ≥ 44 px there through the DPR rule. A-4's single-column clause is evidenced at 360 and 640 CSS px only.
4. The DPR rule also opens the map zoomed at 100 % on high-density desktop displays (DPR ≥ 2). *Whole area* gives the overview.
5. The desktop whole-area view (1280 × 800 at DPR 1) draws stumps at 12.66 px. A-4 names only 360 px and 200 %; the page says so and offers zoom.
6. After a withdrawn confirmation, the lock asks for one more re-read even though the arriving view is already current. This follows the repair instruction.
7. The page witness (J checks) runs in a DOM stand-in, not a browser. It stops at the first section that throws: in NC-1 the D1 failure masked the later checks. The smoke still exits non-zero in that case.
8. PS-01 observed only Advance wins in the exact-SHA run.
9. PM-R15 custody, #846's native release and the lane discrepancy are unchanged and not author-verifiable.

## 1. Commits (branch `feat/multi-standmap-playable2d-20261120-v1`, forward-only)

| Commit | Content |
|---|---|
| `02626f1588433bb1536105eef329e194a16ed141` | PM-T05…PM-T08: the eight UI-A paths |
| `1905433fbec69736c4c05ec629aa5941d11952ee` | Follow-up: "outcome unconfirmed" offers only a re-read (PM-X15); smoke covers Remove and Cancel; README sentence |
| this handoff commit | This Markdown file only |

**Candidate source SHA for Tier-2: `1905433fbec69736c4c05ec629aa5941d11952ee`.** Base A-1 `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f`. Nothing was forced, amended or rebased; both pushes were fast-forwards (`30fa269c..02626f15`, `02626f15..1905433f`).

## 2. Changed files, `30fa269c..1905433f` (all under `Tools/MultiRackSitkaP1Lab/`)

| Path | Row | Kind | Added / removed lines | Blob at `1905433f` |
|---|---|---|---|---|
| `src/Ui/SitkaP1.Ui.csproj` | 8 | new | +31 / −0 | `e60142a0231dbd7a9bf9686e0fa007654df54b70` |
| `src/Ui/Program.cs` | 9 | new | +79 / −0 | `b1deb917ffb9855a7ade0cb860a2db7863ca619a` |
| `src/Ui/PlayerApi.cs` | 10 | new | +355 / −0 | `aec7035a0fb1b33f1740e25439180ab0f183f19e` |
| `src/Ui/wwwroot/index.html` | 11 | new | +164 / −0 | `c8a3e452f5eea7433444b1a934241b42ef54749a` |
| `src/Ui/wwwroot/app.css` | 12 | new | +179 / −0 | `9cd4b6950724afca2eb5f806ca94e1947627bbd7` |
| `src/Ui/wwwroot/app.js` | 13 | new | +911 / −0 | `e1edc8865109425892ffaf6fa877b9a45e4a0fd5` |
| `tests/ui-smoke.sh` | 14 | new, mode 100755 | +381 / −0 | `52a87f8c8433a88f67ec2297b77d7a00eff9acfa` |
| `README.md` | 15 | modify (base blob `e3b312b2…`) | +99 / −1 | `421d1f391ec991d8341891f872a1c3f1156426de` |

Eight paths, 2,199 lines added, 1 removed. No other path changed. The UI project is **not** in `MultiRackSitkaP1Lab.sln`. No package, csproj (other than the new UI project), Core, CLI, fixture, test, `.p0ref`, `build/`, `global.json`, `Directory.Build.props`, `Assets/**`, `ProjectSettings/**` or `Tools/MultiRackSitkaLab/**` change.

## 3. Resume record

An earlier attempt at this task died on a provider transport error after writing real work, with nothing pushed. This run inspected every file first:

- **Kept unchanged:** `SitkaP1.Ui.csproj`, `Program.cs`, `index.html`, `app.css`.
- **Kept and repaired:**
  - `PlayerApi.cs`: a hit-test with a missing `u` or `v` is now a 400 instead of silently reading 0.
  - `app.js`: the receipt shows the work revision as w → w+1; the year announcement lists natural deaths by engine ID instead of counting them; the hidden approved-plan and overlap lists are cleared when they no longer apply; "outcome unconfirmed" offers only a re-read (the Execute retry button is removed).
  - `tests/ui-smoke.sh`: added served-asset byte identity, oracle-exact hit-tests at a five-crown overlap and on bare ground inside the square, the missing-point 400, and Remove/Cancel with their refusals.
  - `README.md`: the launch section that the capability table pointed to was missing; it is now written.
- The earlier attempt's out-of-repository screenshots and logs were **not used as evidence**. Every result below was produced in this run.

## 4. What was built (UI-A, spec §3.2, §3.6, §4, §5, §7, §8)

- **Process (`Program.cs`):** one `PlayerWorkPlanAdapter.Open(fixtureDir)` session per process. Kestrel listens on `IPAddress.Loopback` port 0. A fresh 256-bit capability comes from `RandomNumberGenerator`. The bound address is checked (exactly one, `127.0.0.1`, not 5187, not privileged) before the URL is printed. Ctrl+C or SIGTERM stops the host gracefully. Exit codes: 0 after Ctrl+C, 2 fixture refused, 3 listener check failed, 64 usage.
- **Guards and endpoints (`PlayerApi.cs`):** one terminal handler. Guard order: Host (`127.0.0.1:<port>` exactly, else 403 with no body), capability path (`/s/<capability>/…` with a fixed-time compare, else 404 with no body), route and method (404/405), Origin (every POST, and any request that names an origin, must name this page's origin, else 403), then for a POST `Content-Type: application/json` (415), at most 16 KiB (413) and strict JSON (exact camelCase, unknown members refused, 400). There are no CORS headers, a strict CSP, `no-store`, `nosniff`, `no-referrer` and `DENY` framing. The endpoints are `GET api/view` and `POST api/hit`, `api/pending/add`, `api/pending/remove`, `api/plan/approve`, `api/plan/cancel`, `api/plan/execute` and `api/year/advance`. Each is one adapter call. Responses carry `view` and the Core `PlayerMapScene.Build(view)` scene, plus `refusal`, `execution`, `annual` or `candidates`. A refusal is HTTP 409 with the engine code and a fresh view. A non-refusal exception is 500 `OutcomeUnconfirmed`.
- **Page (`wwwroot`, embedded resources served from memory):**
  - The north-up SVG map is drawn from the server scene (U = X + 15, V = 15 − Z, true crown radii, paint order, stumps).
  - Selection uses the exact-ID hit-test with the overlap list, plus the S-5 cross-check of the browser's topmost drawn crown against `candidates[0]`.
  - The semantic tree table has exact case-sensitive search and a status filter.
  - The inspector shows measurements, the recorded reason and the D-093 note.
  - The Work Plan has Remove, Approve (0 felled), Cancel and Execute with a confirmation and the all-live pre-warning. Advance has the consent dialog. History lists `AnnualYearRecord`s and `WorkEvent`s.
  - Alerts use `role="alert"` with focus; announcements use `role="status"`.
  - Every value displayed comes from the latest server view. All fixed labels come from Core `PlayerLabels`.

## 5. Decisions and deviations for the reviewer

| Id | Item | Disposition |
|---|---|---|
| D-1 | Spec §3.2 names `WebApplication.CreateSlimBuilder`; the code uses `CreateEmptyBuilder` | The slim builder adds `appsettings*.json` configuration (file reads plus a reload watcher) and environment URL sources. The empty builder reads no configuration file, so the only runtime file read stays Core's fixture read (spec §3.2). `Listen(IPAddress.Loopback, 0)` and the §3.6 envelope are unchanged. The smoke starts the process with hostile `ASPNETCORE_URLS`, `DOTNET_URLS`, `ASPNETCORE_HTTP_PORTS` and `Kestrel__Endpoints__*` set, and proves that only `127.0.0.1` is bound |
| D-2 | The Execute retry button is removed | PM-X15 says "outcome unconfirmed" offers only a re-read. A re-read shows either the receipt or the plan still approved; Execute then echoes the same engine-minted key (A-2 replay rule) |
| D-3 | Page-authored structural text | Headings, hints and instructions are page text. Every gate, status, action and consequence label comes from `PlayerLabels` or the view (spec §7.2). The only "proposal-like" word on the page is the Q-6 disclosure "Proposed values: UNAVAILABLE / NOT COVERED"; the only "rack" word is Core's "(not a rack)" |
| D-4 | `PlayerLabels.ExecuteConfirmation` renders "Fells exactly 1 named trees…" for n = 1 | Core text, outside this path list; cosmetic only. For the Core owner |
| D-5 | Map discs at 360 CSS px are 24–48 px (stumps 5 px) | Below A-4's 44 px. All controls measured ≥ 44 × 44 CSS px, and the table's Select buttons are the equivalent ≥ 44 px path. This follows from drawing 100 true-radius crowns of a 30 m square in a 279 px map |
| D-6 | UI-only page state | Besides the draft selection, the page keeps the latest overlap candidates and the last Execute's work revisions (fence sent and server result) to label the receipt "w → w+1". None of these is a forestry value or an authority |
| D-7 | Status codes | 503 before the bind check completes. The guard refusals 400, 403, 404, 405, 413 and 415 never reach the adapter |

## 6. Author Tier-1 — exact pushed SHA `1905433f`, fresh WSL-native LF clone

Harness `~/.ccf-orch/standmap-p1-opus/tier1-ui-run.sh` (outside the repository; sha256 `ee99c1f90fcf3832ccfcedfa02e9bff96d6226afff1ac07fc79de1abe70ebe33`). The clone is a partial sparse clone of GitHub at the exact SHA, with `core.autocrlf=false` and zero tracked lab files containing CR bytes. SDK 8.0.134; ASP.NET Core shared framework 8.0.34. Logs are in `~/.ccf-orch/standmap-p1-opus/tier1-logs-1905433f/` on DESKTOP-DL1BQI7.

| Gate | Result |
|---|---|
| P0 reference | Copied from the authorised pinned copy `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-A1-WorkRevision-Fence/Tools/MultiRackSitkaP1Lab/.p0ref` (the same source as the parent-verified 30fa269c run). Four-way equal `9a99f781…34b3` (DLL, csproj pin, `.sha256` record, `provenance.txt`). **Not rebuilt**; `build/build-p0-core-ref.sh` not run |
| D01 diff scope | `30fa269c..1905433f` touches exactly the eight paths of §2 — **PASS** |
| D02 diff scope | `4bb80973..1905433f`: 20 paths. All 15 lab paths are on the tasks.md list; the other 5 are this spec folder or handoff Markdown — **PASS** |
| Protected blobs | Unchanged since the base — **PASS**: fixtures, `AnnualRules.cs`, `WorkContract.cs`, `AnnualEngine.cs`, `src/Cli`, `.sln`, Core and test csproj, `AnnualTests.cs`, `ContinuationTests.cs`, `WorkContractTests.cs`, `Runner.cs`, `build/`, `global.json`, `Directory.Build.props`, `.gitignore`, `Tools/MultiRackSitkaLab` (tree `f212c8f6…`, the qualified P0 tree), and the two linked `Assets/ForestPrototype` files. The `.sln` does not mention the UI |
| B01 solution build | `dotnet build MultiRackSitkaP1Lab.sln -p:TreatWarningsAsErrors=true`: **0 warnings, 0 errors**. The UI is not built by it (B02) |
| T01 Core runner | **91/91 passed** (66 base + 25 PS/MS), exit 0. Both `644 B-01` scans pass; the source scan recurses into `src/Ui` |
| T02 digests | Ten no-work digests **identical** to the 30fa269c Tier-1 record (`digests.txt` sha256 `4e0e47a1…ccee` in both runs) |
| R01/R02 offline restore | `dotnet restore src/Ui/SitkaP1.Ui.csproj --source <empty local folder>` succeeds; `project.assets.json` lists only `project` libraries (framework references `Microsoft.AspNetCore.App`, `Microsoft.NETCore.App`) — **PASS** |
| U01/U02 UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj --no-restore -p:TreatWarningsAsErrors=true`: **0 warnings, 0 errors**. Exactly three embedded page resources; no `wwwroot` file copied to the output. UI DLL sha256 `a01a2194a747a000f5a0452eda268bb840f53742c2b41094ef9314db9082e931` |
| S01 smoke | `tests/ui-smoke.sh` from the clone: **98 passed, 0 failed** |
| W01–W03 | P0 tracked-file digest unchanged (`9e218ca7…`); clone `git status` clean after build, tests and smoke; `~/.aspnet` absent — **PASS** |

**OVERALL author Tier-1: PASS.**

Log hashes (sha256):

```text
344ec8b38ced93e0bbde21fb183ba78aa10add833925dee99a583424db6af749  run.txt
55590e28f48bc9afd6fa005d5d7179ee9b6b754bd3abdbeef078c4fb00fdf826  build.txt
364aca956d9e0f4b92e40b420a4b308ca68e540124fa11f9ed46510ae297d998  tests.txt
4e0e47a13e6b366b9eadf0d4c47c479da21fa31e8f64f6e07125826dae91ccee  digests.txt
c8bfcd92d3e2425d75ff4dac25c4d7f621a45195135d18aba59f9c96a543a980  ui-restore.txt
501c11f58bc627429ef3a8fed7b3466d079620d169acd1aa4e7eaa0e59a8bc1e  ui-build.txt
18e303d090643973080684a1ac56d3e6b8c25939bfef60b6843a7b75e9651e34  ui-smoke/ui-smoke.txt
9f08310cde83920ce1632c8e6c0e7348a275530addab60f53feff1dbe16692dd  diff-from-30fa269c.txt
d91fa0e25faf33d5b980d881800bfa937823ff577c690a5645fdf861b676249e  diff-from-base.txt
```

The same harness also passed on the superseded `02626f15` (smoke 89/89 then; logs in `tier1-logs-02626f15/`).

## 7. `tests/ui-smoke.sh` — what the 98 checks cover

- **Start:** a scratch copy of the fixture (sha256 checked); hostile URL environment; the printed URL is `http://127.0.0.1:<port>/s/<64 hex>/` with a port that is neither 5187 nor privileged.
- **Sockets:** the only LISTEN socket is `0100007F:<port>` in `/proc/net/tcp`, and every TCP socket of the process is on 127.0.0.1.
- **Guards:**
  - 404 with an empty body without the capability, with a wrong capability and with a 63-character prefix;
  - 403 with no body for `Host: localhost` and for a rebinding host;
  - 403 for POSTs without an Origin or with a wrong Origin, and for a GET naming a foreign Origin;
  - 405 with `Allow`; 415 for text/plain and form posts; 413 over 16 KiB;
  - 400 for malformed JSON, a caller-built plan object and a JSON null;
  - security headers checked; no CORS and no Server header; served page, CSS and JS byte-identical to the committed files.
- **Refusals with zero mutation:**
  - for Add: wrong-case ID, blank reason and another session's fence;
  - for Approve: a missing fence and nothing pending;
  - for Execute: no approved plan;
  - stale-fence Add, hit-test and Advance;
  - for Execute: a key conflict;
  - for Add: a felled tree;
  - for Advance: unexecuted work without consent;
  - for Remove: an ID not pending;
  - for Execute: a cancelled plan.
- **Hit-test:**
  - candidates include the tree at its centre, topmost first;
  - at a five-crown overlap point the candidates equal an oracle computed from the scene;
  - bare ground (outside and inside the square) gives no candidate;
  - a stale fence gives no candidate;
  - a missing point is a 400.
- **Work Plan cycle:**
  - Add with zero Fell (plan-state +1);
  - Approve with an engine-minted `…:plan:2` and key, and zero Fell;
  - Execute: one typed `fell`/`manual-work-plan` Fell in the same year, work revision 0 → 1, 100 → 99 living, basal area down, the frame stand equal to the event's after-stand, and the stump at 0.35 m;
  - replay with the original fence (`replayed: true`, no change);
  - Advance refused with the old fence, then committed with a fresh one: the year 0 → 1 started from 99 living, and the felled tree keeps its Fell cause;
  - the consent refusal, then the consented cancellation at zero Fell;
  - a Year 2 decision with Add, Remove, Approve, Cancel (orders not returned to pending), re-add, Approve and Execute.
- **Stop and leave nothing:**
  - SIGINT gives exit 0 and "Session ended; nothing was saved.", with nothing on stderr, and the port then no longer answers;
  - no file is created, changed or removed anywhere in the lab tree (including `bin`, `obj` and `.p0ref`);
  - `~/.aspnet` is unchanged, the P0 digest is unchanged, `git status` is unchanged and the fixture is unchanged.

## 8. Browser evidence

Images are **not committed**. They are in external verification storage on DESKTOP-DL1BQI7 under `~/.ccf-orch/standmap-p1-opus/`, with SHA256SUMS. The capability is redacted to `/s/<capability>/` in every log; a scan of the evidence found no capability string.

### 8.1 Final run — headless Microsoft Edge 157.0.4341.60 over the DevTools protocol, against the Tier-1 clone build of `1905433f`

- **Provenance:** captured 2026-11-20T05:53Z on DESKTOP-DL1BQI7; clone HEAD `1905433f…`; UI DLL `a01a2194…`; launched with the README command from the clone. Folder: `tier1-logs-1905433f/browser/`. Driver `edge-driver-cdp-ui-evidence.mjs` (outside the repository; Node 26, no packages; sha256 `e32fa1d8f80c6af27bbbeafbd756fc4a579c057f4320389387bc7b23163a5ed1`). A throwaway Edge profile was used and removed afterwards.
- **Input method:** keys are trusted `Input.dispatchKeyEvent` events, text entry is `Input.insertText`, a touch tap is `Input.dispatchTouchEvent` with touch emulation, and the viewports come from `Emulation.setDeviceMetricsOverride`. **200 % zoom is emulated** as a 640 × 400 CSS-px viewport at devicePixelRatio 2, which is what 200 % zoom gives in a 1280 × 800 window. It is not the browser's zoom control.
- **Result:** **65 checks, 65 PASS, 0 failures.** No uncaught page exception. The only console entry is the browser's own `ERR_EMPTY_RESPONSE` line for the request deliberately sent after the server was stopped (U02).

| Phase | What was witnessed |
|---|---|
| S0, 1280 × 800 | Full-width capture: title, disclosure, fixture SHA (short and expandable), RNG v1 / growth v2 / algorithm, year 0, annual 0, work 0, plan-state 0, session, 100 living, 1111.1 stems/ha, 55.87 m²/ha, Dq 25.3 cm, the four honesty flags. No page-level horizontal scroll; two columns |
| Keyboard only at 200 % (K01–K26) | Tab → skip link (visible 3 px focus) → Enter → tree table heading → Tab order: search, status filter, table region, Select P30-001, P30-002, P30-003 → Enter selects; focus moves to the inspector heading → reason → Add (zero Fell) → **Remove** (Enter) → Shift+Tab back to Add, re-add → Approve (Space; zero Fell) → **Cancel approved plan** (Enter; orders not returned) → re-add, Approve → Tab to Cancel, then to Execute (separate stops) → Enter opens the confirmation with focus on Back → **Escape** closes it and returns focus, nothing changed → confirm → receipt `Work revision = 0 → 1`, 100 → 99 living, no approved plan left in the page → Advance (no dialog), year 0 → 1 → exact-ID search "P30-004" + Enter → reason → Add → Tab order Remove → Approve → Advance → consent dialog on Keep → **Escape** returns focus → Keep orders changes nothing → Cancel unexecuted work and advance (Space): year 2, cancelled at zero Fell, work revision still 1. Visible focus (`:focus-visible`, 3 px `#ffd166` outline) at every stop; no focus trap |
| Touch at 360 × 800 (mobile emulation, device scale 2) | No page-level horizontal scroll. A tap on the centre of a crown that is topmost on screen (P30-001) produced trusted `pointerdown`/`touchstart`/`pointerup`/`touchend`/`mousedown`/`mouseup`/`click` (`pointerType` touch). The server hit-test selected exactly P30-001 ("2 crowns overlap here"), and nothing was added |
| Contrast | 13 sampled text styles computed against their effective backgrounds: minimum 9.4:1. This is a sample, not a full audit |
| Outcome unconfirmed (real transport failure) | P30-005 selected by keyboard, reason typed, focus on Add. The real session process was then stopped with SIGINT (it exited 0 with "Session ended"). Enter on Add → alert "Outcome unconfirmed — re-read current state"; **the only action is Re-read**; focus is in the alert; nothing is drawn as committed (pending and plan-state unchanged; P30-005 still Living) |

Screenshots (sha256):

```text
c61fd46c66caddb4ea13836fd7ec879bafedfe4ce649219bb84bcc6b5b9572d2  edge/E01-S0-1280x800.png
4da9b6dfd0d54d91776217661eee5560532f0dc9fa917d5fd538a36d8e264c92  edge/K00-200pct-S0.png
cbd97c496c124c47a1792a94c0c0dd5c9677cbec7a5a117cd831ce8dc96b31c5  edge/K01-200pct-skip-link-focus.png
0b4b8f7f06f26618f08444da133e4ea182dc3a0df8e67f9e8aadd54cafc26047  edge/K06-200pct-add-button-focus.png
717aa79c7c14cf9514b7e2d063f10d9737f6a62660f3ebe598e9385be22b28c5  edge/K13-200pct-execute-dialog-focus-back.png
411f0ac7a2a95661d1cdf0e6514053ba88866355f49ff9dd18259a524a9cde6d  edge/K15-200pct-execute-confirm-focus.png
ac6f3b3eeb618d2e0eb843dcd5f8de7b3ecf30f79c39ba9dd7612122db82433b  edge/K16-200pct-receipt.png
27f8c3a03b32b9e76d7c49da38422fb4b5172af3195a640a2f1d31f04ceb2cb7  edge/K23-200pct-consent-dialog.png
6b3aaf18b0e588c9890291264d668f4d8fc46f0abbd40cdbd91e1ad525e3ab9d  edge/T01-360x800-touch-tap-select.png
94523312f4c8751ae22a9c0e03f07bac091377f22740c403ec1a6f9d0bc8807d  edge/U02-outcome-unconfirmed-only-reread-1000x800.png
6ede18a1e0c08ab03d993be038beb4f2ff582b6faeb69bf1b81adfc801f6f54b  edge/cdp-log.json
```

### 8.2 Supplementary — T3 preview browser (pointer and tool-typed text), pre-final lane build

- **Provenance:** captured 2026-11-20T05:24–05:33Z against the lane working tree, built from these blobs: `Program.cs`, `SitkaP1.Ui.csproj`, `index.html`, `app.css` and `PlayerApi.cs` as at `1905433f`. Three blobs differ from the final: `app.js` `235bc2c4…`, which lacks the hidden-list clearing and still had the Execute retry; `tests/ui-smoke.sh` `c44344e9…`; and `README.md` `49de7c39…`. Folder: `resume-browser-1/shots/` (13 PNGs, hashes in that folder).
- **What was witnessed (S0 → S7):**
  - a real map click on P30-001 went through the server hit-test, with no S-5 ambiguity, an overlap list of 2 crowns and the inspector measurements;
  - **refusal 1:** a blank reason gave `WorkReasonRequired` in the alert, with "Nothing changed. Current ecological year 0." and focus moved to the reason field;
  - **refusal 2:** the wrong-case search "p30-001" gave "No exact match — the selection is unchanged";
  - Add (plan-state 1, dashed ring, "Pending Fell (P)");
  - Approve ("0 trees felled", the minted PlanId and key, a double ring);
  - the Execute confirmation, then the receipt (w 0 → 1, 100 → 99, basal area 55.87 → 54.93) and the stump with its canopy gap;
  - Advance (year 0 → 1, natural deaths none);
  - the consent dialog: **deny** (Keep orders, nothing changed, focus back on Advance), then **consent** (year 1 → 2, cancelled at zero Fell);
  - the history tables;
  - at 360 px: single column, no page-level horizontal scroll, all 109 visible controls ≥ 44 × 44 px.
- **Limits:**
  - this tool's captures are cropped to 1016 px wide, so desktop captures were also taken at 1000 × 800 (the same two-column layout);
  - after a page reload, the preview tab stopped producing animation frames (environmental: no input could be delivered), so the keyboard pass moved to the Edge driver of §8.1.

### 8.3 Not run

- **Screen reader** (NVDA or any other): **NOT RUN.**
- **Physical touch device:** not run; touch was emulated.
- **The browser's own zoom control:** not used; 200 % was emulated as described.
- **A human operator's own session:** none; every browser run was agent-driven.
- **A bare-ground click in the browser:** not run. Bare ground was witnessed through the API (smoke H03, H09) and the page code.

## 9. Per-item author status — this delta's checklist (15 PM-R + 16 PM-X)

These are author Tier-1 statuses on `1905433f`. **They are not checklist ticks**: no checkbox was edited, and Tier-2 decides.

| Item | Author status | Evidence |
|---|---|---|
| PM-R01 UI-A honoured | AUTHOR-EVIDENCED | UI-A process; README declares "It is an HTTP server"; §6, §7 |
| PM-R02 #730 acceptance recorded | DOCUMENTARY | The live board row cites the Manager's approved-spec readback at `fed675f5`; the first source commit `24d9ef11` follows it. The acceptance itself is the Manager's |
| PM-R03 map path B | AUTHOR-EVIDENCED | No `StandMapSource` reference; UI csproj has no compile links; linked Assets blobs unchanged |
| PM-R04 single engine | AUTHOR-EVIDENCED | Page renders only server views (code; §8 DOM reads after responses); D-6 |
| PM-R05…R09 | AUTHOR-EVIDENCED | Core PS tests in the 91/91; smoke fences, minted IDs and replay |
| PM-R10 exact identity S-1…S-8 | AUTHOR-EVIDENCED | MS tests; smoke H01–H09; browser S-5 with pointer and touch; table and search by keyboard |
| PM-R11 truthful labels | AUTHOR-EVIDENCED | Smoke V02; E01 |
| PM-R12 no forbidden elements | AUTHOR-EVIDENCED | Scan of UI sources; D-3 negations only |
| PM-R13 protected scope | AUTHOR-EVIDENCED | §6 D01, D02 and protected blobs |
| PM-R14 accessibility A-1…A-7 | PARTIAL | A-1, A-5, A-6 (emulated) and A-2 structure evidenced; A-3 sampled; A-4 except map discs (D-5); **A-7 screen reader NOT RUN** |
| PM-R15 physical/source admission | DOCUMENTARY / NOT AUTHOR-VERIFIABLE | Board registration; author pre-write lane checks; P0 copied and verified, never rebuilt. #846 release "must be rechecked" per the board — not verified by the author |
| PM-X01 base preserved | AUTHOR-EVIDENCED | 91/91, ten digests identical, base test blobs unchanged, fresh LF clone |
| PM-X02…X12 | AUTHOR-EVIDENCED | PS-01…PS-19 and MS-01…MS-06 inside the 91/91 (Core unchanged since 30fa269c) |
| PM-X13 hit-test identity and S-5 | AUTHOR-EVIDENCED | MS-03, MS-04; smoke H01–H09; in-browser S-5 witnessed. The bare-ground negative was witnessed through the API, not by a browser click |
| PM-X14 UI process envelope | AUTHOR-EVIDENCED | Smoke 98/98 from the clone |
| PM-X15 human-operable run | AUTHOR-EVIDENCED | 1280 / 360 / 200 % captures with provenance; keyboard-only pass; refusal announced; nothing optimistic; U02 offers only a re-read |
| PM-X16 independent Tier-2 | **NOT RUN** | PM-T09 (GPT-6.1 Sol), pending |

## 10. Per-item author status — Stage 5 (26 R + 24 X)

All 50 stay unchecked. Under #730 Q-3, **R-06 and X-05 are not PASS from this slice**. R-07, R-08, X-07 and X-09 follow the conditional **N/A — G1 DISABLED** rule, which needs the #730 disposition.

| Item(s) | Author status | Note |
|---|---|---|
| R-01, X-03 | AUTHOR-EVIDENCED | One undivided area label; individually selectable trees; no rack, Confirm or all-tree control |
| R-02, R-03, X-01 | AUTHOR-EVIDENCED | Synthetic 30 m fixture and disclosure; 100 exact IDs; fixture SHA shown; every command fenced. Fixture-tamper negatives are existing Core tests |
| R-04, X-02 | AUTHOR-EVIDENCED (Core) | The UI renders committed frames only |
| R-05, X-04 | PARTIAL | Manual per-ID selection with reasons evidenced; the optional sample is not delivered (Q-3) |
| R-06 | NOT MET BY THIS SLICE | Q-3 deferral |
| R-07, R-08, X-07, X-09 | N/A — G1 DISABLED (needs #730 disposition) | The "Comparison extension: DISABLED — Manual/NotCovered" label is shown; the manual path works |
| X-05 | NOT PASS FROM THIS SLICE | The manual fallback and G1-disabled label are evidenced; the sample is deferred |
| R-09, R-10, X-06 | AUTHOR-EVIDENCED for this UI | No candidate, proposal or bulk path exists; Add and Approve make zero Fell. The #805 candidate is outside this UI |
| R-11, X-08, X-10, X-11 | AUTHOR-EVIDENCED (manual channel) | The prescription channel is N/A because G1 is disabled |
| R-12, X-12 | AUTHOR-EVIDENCED | Approve and Execute are separate; confirmation; one typed Fell |
| R-13, R-26 | AUTHOR-EVIDENCED | Engine revalidation; smoke refusals; alerts |
| R-14, R-15, R-16, X-13, X-14 | AUTHOR-EVIDENCED | Engine event; replay; fault rollback (PS-16) |
| R-17, X-15 | AUTHOR-EVIDENCED | Consent deny and consent; Cancel; refusals |
| R-18, X-16, X-17 | AUTHOR-EVIDENCED | Year+1 from survivors (PS-14); ten digests; consent case |
| R-19, X-18 | AUTHOR-EVIDENCED | Session history read-only; a second decision cycle. No restart claim |
| R-20, X-19 | AUTHOR-EVIDENCED (Core) | The Fell cause is distinct in the inspector and history |
| R-21, X-20 | AUTHOR-EVIDENCED | "UNAVAILABLE (not €0)"; no money number anywhere |
| R-22, X-21 | PARTIAL | Keyboard, emulated touch, 360 and 200 % evidenced; screen reader NOT RUN |
| R-23, X-23 | AUTHOR-EVIDENCED | No save or web storage; smoke W01/W02. This is an HTTP server, allowed only under Q-1 |
| R-24, X-22 | AUTHOR-EVIDENCED | Protected blobs unchanged. Bit-flip negatives are existing Core tests |
| R-25, X-24 | DOCUMENTARY / NOT AUTHOR-VERIFIABLE | Independent qualification and gates are pending |

## 11. Unverified items, risks and blockers

1. **Tier-2 has not run.** There is no independent review, qualification or Manager acceptance.
2. **Screen reader: NOT RUN.** Touch and 200 % were emulated in headless Edge; no physical device and no browser zoom control.
3. **Map disc size at 360 px (D-5)** is below 44 px; the table path is the equivalent.
4. **D-1 builder deviation** needs the reviewer's agreement that it stays within spec §3.2's envelope.
5. **PM-R15 custody:** the board says #846's native lease release "must be rechecked". The author did not verify it.
6. **Lane identity:** the board names the T3 Windows worktree; this run used the WSL lane designated by the task packet.
7. **Cosmetic Core label (D-4)** is for the Core owner.

## 12. Manual steps for a human check (Ubuntu on WSL2, then a Windows browser)

1. From the repository root: `cd Tools/MultiRackSitkaP1Lab`.
2. Copy `.p0ref` from the authorised pinned copy and verify the four values, as in the README "Launch" section. Never run `build/build-p0-core-ref.sh`.
3. Build: `~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj` (expect 0 warnings).
4. Run: `~/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived`.
5. Open the printed `http://127.0.0.1:<port>/s/<capability>/` in Microsoft Edge.
6. Expect S0 to show Year 0, 100 living and 55.87 m²/ha, plus the four flags.
7. Click a crown, or use Tab to reach the table and press Enter on a Select button.
8. Write a reason and choose *Add to Work Plan — no tree is felled* (still 100 living).
9. Choose *Approve whole Work Plan — 0 trees felled* (still 100 living).
10. Choose *Execute approved plan* and confirm. Expect the receipt to show work revision 0 → 1 and 100 → 99 living, and a stump on the map.
11. Choose *Advance one ecological year*. Expect Year 1.
12. Add another tree, then *Advance* again. Expect the consent dialog; try *Keep orders* (nothing changes), then *Cancel unexecuted work and advance*.
13. Ctrl+C in the terminal. Expect "Session ended; nothing was saved."
14. Smoke: `TMPDIR=<outside the repository> tests/ui-smoke.sh <evidence directory outside the repository>`. Expect `OVERALL: PASS (98 passed, 0 failed)`.

## 13. Next

PM-T09 ran on `1905433f` and returned NEEDS CHANGES (§0.1). **Next: the independent Tier-2 re-check of the repair candidate `d2c04cbbca380b4a5076807c72f19632bf2543a8`** on a fresh clone. It should retest the confirmation race, the committed-but-answer-lost recovery, first-use key rejection with normal replay, and the map target sizes at 360 px and 200 %, and should rule on the limits in §0.9.
