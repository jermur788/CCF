# Multi #814 — reversible local candidate marking: implementation, independent review and focused qualification result

**19 November 2026. LOCAL CANDIDATE result. Not integrated, not released, no main merge, no L4 waiver.**

| | |
|---|---|
| IMPLEMENTED | **yes** — additive local `REMOVE_CANDIDATE` layer on the approved #811 viewer |
| BASE | exact untouched #811 candidate `896132373542023ccbd0e4ed9d96826202ac4e81` |
| CANDIDATE | local branch only, three commits; see §3 |
| LOCALLY QUALIFIED | **yes** — 75 focused checks, protected/offline gates, two-viewport browser smoke |
| INDEPENDENTLY REVIEWED | **yes** — Claude Opus 5.5, nonauthor changed-lines review and fix re-check (§7) |
| INTEGRATED | **no** — local branch only; no code push, no PR, no main merge |
| ACCEPTED / RELEASED | **no** |
| UNITY / WINDOWS PLAYER | **not run / not built** |
| .NET L4 FAIL38 | **UNWAIVED** |

## 1. The result

A locally runnable, offline, source-bound Year 0–10 history viewer that now accepts **reversible local removal-candidate annotations** on the exact committed source it is displaying. The annotations are a local UI layer: they never fell a stem, never change a biological value, never create a work plan and never persist.

Runnable file (unchanged name, new build):

```
/home/jer/CCF-Dev/Multi-20261119/_814/marking/Tools/MultiRackSitkaHistoryViewer/dist/year0-10-history-viewer.html
```

Windows view: `\\wsl.localhost\Ubuntu\home\jer\CCF-Dev\Multi-20261119\_814\marking\Tools\MultiRackSitkaHistoryViewer\dist\year0-10-history-viewer.html`.

Open it directly (`file://`). No server, install or network is needed. It is one self-contained HTML file, 1,174,745 B, SHA-256 `72c25e86c16614c81e5e65627d575e08e603ad25337f5a1089c8063581041d12`.

## 2. Actors and independence

| Role | Actual identity |
| --- | --- |
| Orchestrator / source author | DeepSeek V4.1 Flash 1017, `opencode-go/deepseek-v4.1-flash`, native T3 Code + OpenCode Code Mode, `DESKTOP-DL1BQI7` WSL2 Ubuntu, GitHub `jermur788` |
| Independent nonauthor reviewer | Claude Opus 5.5, `claudeAgent/claude-opus-5-5`, distinct provider and session; changed-lines review and bounded fix re-check |
| Manager record | [#730](https://github.com/jermur788/CCF/issues/730) |

No GPT-6.2 was used. The reviewer did not write the implementation and did not edit, build, commit or push.

## 3. Gates, ownership and worktree

- **RUNNING registration:** [#814 comment 7010326382](https://github.com/jermur788/CCF/issues/814#issuecomment-7010326382), posted before the first source edit.
- **New isolated WSL2 ext4 worktree:** `/home/jer/CCF-Dev/Multi-20261119/_814/marking`, branch `feature/multi-814-local-candidate-marking-20261119-v1`.
- **Base:** the exact untouched #811 local candidate `896132373542023ccbd0e4ed9d96826202ac4e81`.
- **Candidate head:** `e8e1d37682c1fc8ace197c189dce9bd2a55a2b0b` (implementation `034fc8d2`, bounded review fixes `de7caef5`, docs-only review-record correction `e8e1d376`; all recorded in §7).
- **Write ceiling:** every Git-tracked change is under `Tools/MultiRackSitkaHistoryViewer/**`. No other path in the repository was written.
- **Local physical ownership:** user `jer`; no foreign worktree lock, no competing `dotnet`/`node`/Unity/Blender process at registration, target path absent, 918 GB free. The external ZX20/Files Ops board was **not mounted**, so the external physical lease was not re-read; no external Ops write was attempted and no external board authority is claimed. The ownership basis for this task is the Manager-granted additive local-ext4 scope plus verified local single-writer custody.
- **Preserved:** the #811 worktree is clean at `89613237`; #784 Core `c4124b63`, Viewer `f3537fe3`, Year0 frozen HTML, fixture, #725 Golden stdout and #789 engine `6f9d2e99` are unchanged (§8.2). The three #810 baseline worktrees were not entered.

## 4. Implemented files

All under `Tools/MultiRackSitkaHistoryViewer/`:

| Path | Change |
| --- | --- |
| `web/candidate-marks.js` | **new** pure candidate state machine: exact-bit eligibility, atomic batch validation, grouped history, epoch stale fence, noncurrent refusal. No DOM, no I/O, no persistence. |
| `web/history-ui.js` | ordered-ID checklist, Mark/Unmark/Undo/Redo/Discard, source badge, stale banner, diagnostic disable, current-versus-proposed table, candidate message status. |
| `web/history-map.js` | dashed candidate ring with `C` glyph; no second hit test; source crown/status/position untouched. Canvas title moved to the bottom-left so the larger legend cannot overlap it. |
| `web/history.css` | candidate panel, checklist, compare table and legend styles. |
| `web/shell.template.html` | candidate panel markup, legend item, updated boundary copy, new script slot. |
| `build/assemble-viewer.py` | inline the candidate module; updated banner. |
| `tests/candidate-suite.js` | **new** 27 focused candidate checks registered into the existing runner. |
| `tests/browser-smoke.js` | **new** committed Windows Edge / `playwright-core` harness that produced the browser evidence and can be re-run with documented env overrides. |
| `tests/run-tests.js` | wire the candidate suite and two static package checks; 75 checks total. |
| `tests/verify-offline-and-protected.sh` | neutral env-var names with #811 fallback; same protected gates. |
| `README.md`, `SOURCE-MAP.md` | candidate contract, refusal codes, stale semantics and known gaps. |
| `evidence/*` | refreshed tests, hashes, browser result, screenshots and evidence note. |
| `dist/year0-10-history-viewer.html` | rebuilt single-file deliverable. |

## 5. What the candidate layer does

- **Ordered, accessible checklist.** All 100 `P30-001…P30-100` IDs in ordinal order as native checkboxes inside a named list. Each checkbox's accessible name contains the ID, the exact eligibility state and whether it is a candidate. A polite live status region announces every action.
- **One A-owned hit test.** The map keeps exactly one nearest stem-centre hit test at 8 CSS px with ordinal tie-break and explicit MISS. The candidate control adds **no** second map hit test and **no** modifier-based map batch.
- **Exact-current-source eligibility.** Marking requires a living row in the currently committed Genesis or `AfterMortality` frame with a valid finite non-negative f32 DBH bit word `≥ 0x41880000` (170 mm). The check is on the source bits, so a value that merely displays as `17.00 cm` from `0x4187fffe` is refused. Dead, unknown-bit, malformed and below-threshold rows are disabled with their reason. Genesis has **88** markable and **12** below-threshold stems.
- **Atomic whole-batch validation.** `Mark selected` / `Unmark selected` validate the ordered batch first. A duplicate ID **inside the same submitted batch**, an unknown ID, an invalid ID or any ineligible element refuses the **entire** batch with zero annotations changed and zero undo entries. No N−1 application and no silent deduplication.
- **Repeated action is a true no-op.** A later independent Mark of already-marked IDs, or Unmark of not-marked IDs, returns `noop` with no annotation change, no history step and no redo invalidation.
- **Grouped undo/redo/discard.** One effective batch is exactly one undo unit; Undo and Redo move whole batches; a new effective edit after Undo clears Redo; Discard clears annotations, history and redo.
- **Stale revision fence.** The draft binds to a monotonic committed-source epoch and an identity key built from domain, fixture id/SHA, content hash, engine commit, seed, model/RNG versions, algorithm, year and revision, plus a 16-hex opaque read token. Switching the active current to another committed year (Genesis or `AfterMortality`) makes the previous draft `STALE_READ_ONLY`: marks stay visible, Mark/Unmark/Undo/Redo are blocked and there is no automatic rebase. Returning to the earlier year does not resurrect it. An empty draft (no marks, no history) may rebind because there is no candidate state to carry — a deliberate, disclosed choice. Discard starts a distinct fresh draft.
- **Diagnostic tabs are not a revision.** `Before` / `Post-growth` display disables editing and does not change the epoch, stale or unfreeze the draft. Returning to the committed tab restores editing on the same draft.
- **Refused replacement.** A refused source update keeps the last accepted frame visible but labels it explicitly noncurrent and refuses every mutation with `SOURCE_NONCURRENT`. No partially admitted frame is published.
- **Current versus proposed.** The comparison table shows the committed source reading beside the proposed local reading. Every biological value is identical — living count, BA, Dq, RD, deaths — and only the candidate count differs. No post-fell crown, stump, timber or altered forest is fabricated. The map keeps the source crown, living status and position and adds one dashed candidate ring.

## 6. Inherited source admission (unchanged from #811)

The package admission pins are unchanged: fixture `bcaaa5c3…` (32,567 B), content hash `b7dee8a3…`, f32 area `3db851ec`, seed 20261116, model 2/RNG 1, transform `p1-square30-corner-to-centred-xz-v1`, engine `6f9d2e99…`, engine mirror `70ddbaf7…`, Year1 golden `574d8746…` (45,980 B), all ten `#789` continuation digests, V-01 inputs and P0 reference DLL. The re-export reproduced every per-year digest, 13 natural deaths and the same detached package SHA-256 `1c5cf768…` as #811, so the genuine history is unchanged.

Refusal order and codes are unchanged from #811. The candidate layer runs only after admission and returns its own separate codes (`STALE_READ_ONLY`, `SOURCE_NONCURRENT`, `NOT_CURRENT_SOURCE`, `INVALID_ACTION`, `EMPTY_BATCH`, `INVALID_ID`, `DUPLICATE_IN_BATCH`, `UNKNOWN_ID`, `INELIGIBLE_DEAD`, `INELIGIBLE_BELOW_THRESHOLD`, `INELIGIBLE_UNKNOWN`).

## 7. Independent nonauthor review and bounded fixes

### 7.1 Initial review — APPROVED with minor findings

Claude Opus 5.5 (different provider and session) reviewed the changed lines from `89613237` to `034fc8d2` read-only. It returned **APPROVED**, confirming all twelve contract points, one commit, every changed path inside `Tools/MultiRackSitkaHistoryViewer/`, no protected-file change, 73/73 tests, the exact f32 eligibility rule across all 1,100 committed rows with zero mismatches, the assembled HTML matching the committed sources with the same CSP and hash, and the unchanged #811 history digest `1c5cf768…`. It found no blocking or major defect.

Minor findings and notes, all closed in `de7caef5`:

| Finding | Disposition |
| --- | --- |
| The evidence README pointed at the #811 review as if it were the #814 sign-off. | Added `evidence/independent-review-814.md`; the README now labels `independent-review.md` as the earlier #811 record. |
| The evidence README named host `jer-laptop`. | Corrected to `DESKTOP-DL1BQI7`, matching `environment.txt`. |
| A stale annotation did not say which year it was made against, and the row's eligibility described the new source. | Each marked row now reads `candidate · Y3` / `stale · Y3`, and the checkbox accessible name says `made against Year 3`. |
| The live status region was rewritten on every render and could re-announce an old message. | Written only when the message text changes. |
| UI wiring had no re-runnable test; the smoke script was uncommitted. | Committed `tests/browser-smoke.js` with documented Windows Edge / `playwright-core` requirements and environment overrides; the qualification run used that committed file. |
| No negative/non-finite DBH-bit test. | Added `c1880000`, `80000000`, `7f800000`, `7fc00000`, `ff800000` cases, all `INELIGIBLE_UNKNOWN`. |
| Discard was disabled with only a Redo history. | Discard now clears it; asserted by the browser smoke. |
| Unused `describeSource`; one brace-line formatting artifact. | Dead code removed and formatting corrected. |

The reviewer accepted the empty-draft rebind as disclosed and non-weakening (it requires both zero marks and empty history, and the epoch still advances), and accepted that contract item 8 (refused replacement → noncurrent) is reachable in the module only because the static build has no live replacement channel.

### 7.2 Fix re-check

The same nonauthor reviewer re-checked `de7caef5`. It confirmed **all nine findings closed**, 75/75 tests, no regression in eligibility, atomic batch, duplicate refusal, no-op, grouped undo/redo/discard, stale epoch, diagnostic non-authority, noncurrent refusal or current-vs-proposed equality, the diff still confined to `Tools/MultiRackSitkaHistoryViewer/**`, and every recorded hash matching.

Its verdict on that re-check was **NEEDS CHANGES for one documentation defect only**: `evidence/independent-review-814.md` had pre-stated an APPROVED re-check before the re-check existed. No code, contract or evidence value was wrong. The paragraph was replaced with the received verdict and committed as docs-only `e8e1d376`; the shipped HTML is unchanged. No third reviewer verdict is claimed. Final source head: `e8e1d376`.

No finding was disputed and no contract test was changed to pass.

## 8. Qualification

### 8.1 Focused JS suite — 75/75 PASS

```bash
cd /home/jer/CCF-Dev/Multi-20261119/_814/marking/Tools/MultiRackSitkaHistoryViewer
/home/jer/.local/share/vite-plus/bin/node tests/run-tests.js
```

Covered: the full #811 history/refusal/map suite (48 checks) plus 27 candidate checks — Genesis 12/88 eligibility, exact-f32 versus rounded display, negative/non-finite/unknown DBH words, dead refusal, grouped batch, duplicate-in-batch atomic refusal, bad-element atomic refusal, unknown/invalid/empty/action refusal, repeated Mark and Unmark no-op, grouped Undo/Redo, redo clearing, Discard, discard of a redo-only history, stale year switch, no auto-unfreeze on return, empty-draft rebind, fresh draft binding, diagnostic non-authority, refused-replacement noncurrent state, deterministic year-specific tokens, source-frame immutability and Genesis editability. Output: `evidence/unit-tests.txt`.

### 8.2 Deterministic offline replay and protected hashes

```bash
MULTI_HISTORY_DOTNET=/home/jer/.dotnet/dotnet \
MULTI_HISTORY_SCRATCH=/home/jer/CCF-Dev/Multi-20261119/_814/scratch \
bash tests/verify-offline-and-protected.sh
```

Full log: `evidence/verify-protected.txt`.

- forced rebuild from committed sources: 0 warnings / 0 errors;
- normal export and `unshare --user --map-current-user --net` export byte-identical, SHA-256 `1c5cf76856197e4c2e9d14b82d30f8c13285bece5f0e662d782a3446a65dfc68`;
- packaged frames equal the replay;
- #784 Year0 Core `01e7e936…`, viewer HTML `3bc38102…`, CSS `f91cb171…`, map `fbb80216…`, UI `17b951ff…`, boot `aa409eca…`, fixture `bcaaa5c3…` and frozen HTML `0b69cc1d…` (127,831 B) unchanged;
- #725 Golden `574d8746…` (45,980 B) unchanged;
- all twelve #789 engine source hashes unchanged;
- P0 reference DLL `9a99f781…` unchanged;
- both V-01 host Assets unchanged.

### 8.3 Real-browser offline smoke

Windows Edge (Chromium) headless through the locally installed `playwright-core`, opening the built file from `file://`:

- requests: the `file://` document only; **zero external requests**; CSP blocked a deliberate external connect probe; zero page/console errors;
- source badge names `Year 1 After mortality · committed revision 1 · read token …` and `LocalCandidateOnly`;
- Genesis shows 88 enabled and 12 disabled below-threshold rows;
- current-versus-proposed table has identical basal area in both columns and differs only in the candidate count;
- marking `P30-001/002/003` creates one grouped edit, the map aria-label reports 3 candidates and the canvas draws the dashed `C` ring;
- repeated Mark reports `No change: every selected ID is already a local candidate.`;
- grouped Undo/Redo moves all three annotations per step; diagnostic tab disables editing without staling; repeated Unmark is a no-op;
- Year 3 dead stem `P30-044` is disabled and labelled `Dead — natural mortality, frozen record`;
- Year 3 → Year 4 stales the draft with `STALE_READ_ONLY`, Year 3 tokens and a per-row `stale · Y3` note, no auto-rebase; returning to Year 3 stays stale; Discard starts a fresh Year 3 draft; a redo-only history can be discarded;
- map hit `P30-001` at 8 CSS px and explicit MISS still work;
- 390×844 has no horizontal overflow and candidate marking still works; 1440×900 shows the ring and panel;
- accessible controls: named ordered list, `aria-live` status, labelled checkbox names.

Raw result and screenshots: `evidence/browser-smoke-results.json`, `evidence/browser-wide-1440x900.png`, `evidence/browser-narrow-390x844.png`.

### 8.4 Artifact hashes

```
72c25e86c16614c81e5e65627d575e08e603ad25337f5a1089c8063581041d12  1174745  Tools/MultiRackSitkaHistoryViewer/dist/year0-10-history-viewer.html
1c5cf76856197e4c2e9d14b82d30f8c13285bece5f0e662d782a3446a65dfc68  2581679  build/out/admitted-frames.json
```

### 8.5 Build environment

- WSL2 Ubuntu 26.04 x64, kernel 6.18.58.1, 7.7 GiB RAM.
- .NET SDK 8.0.134 / runtime 8.0.34 (`global.json` pins 8.0.133 with `rollForward: latestPatch`); the exact 8.0.133 SDK was not installed. Disclosed deviation; every digest and byte gate passed under 8.0.134.
- Node 26.13.1; Python 3.14.4; Windows Edge for the browser smoke. No remote NuGet source; `NuGet.config` clears all sources; the export was re-run under `unshare --net`.
- `build-inputs/p0ref/SitkaLab.Core.dll` is gitignored by design; the #814 worktree copied it task-locally from the untouched #811 worktree and both copies hash to `9a99f781…`.

## 9. Scope and prohibition attestation

No Fell, Work Plan, approval/execute, annual/engine mutation, save, persistence, `localStorage`/`sessionStorage`/`IndexedDB`/cookie, Host, network, Unity/Windows build, public code push, main merge, release, public API or L4 waiver. No edit to #784 Year0 Core/Viewer/fixture/frozen HTML, #725 stdout, #789 engine, `Assets/**`, `ProjectSettings`, packages, CCF-HP or any archived copy. The #811 original worktree and the three #810 baseline worktrees are untouched.

## 10. Known gaps / not run

- No full screen-reader traversal, touch, 200 % zoom or independent user playtest. Only the focused keyboard/ID route, labelled control names, named list, live status and two browser widths were checked.
- Duplicate-in-batch refusal is implemented and unit-tested at the module contract, but the checkbox UI cannot itself produce a duplicate; there is no text-entry batch surface in V0.
- The refused-replacement noncurrent path is implemented and unit-tested, but the shipped single-file viewer has no live replacement channel to trigger it in the UI.
- Nothing persists by design: a refresh discards all annotations.
- No Unity Editor run or Windows Player build.
- External ZX20/Files Ops board not mounted; external physical lease not re-read; no external Ops authority claimed.
- Package integrity is a consistency gate, not a cryptographic signature; the C# adapter and recorded hashes remain the source of truth.
- Terminal zero-live state remains untested and is not claimed.
- `#805`'s 47 R/X/Q items remain unchecked unless actually exercised above. This result covers only the owner-approved local O-02 subset; it does not claim the full four-file Work/Host/save contract, Stage 5 (#812) or an executing rack.

## 11. Handoff to Manager #730

**One outcome:** a locally runnable, source-bound, offline viewer with reversible local removal-candidate annotations, plus its independent review and focused qualification evidence.

- worktree `/home/jer/CCF-Dev/Multi-20261119/_814/marking`
- branch `feature/multi-814-local-candidate-marking-20261119-v1`
- base `896132373542023ccbd0e4ed9d96826202ac4e81`
- head `e8e1d37682c1fc8ace197c189dce9bd2a55a2b0b`
- runnable file `Tools/MultiRackSitkaHistoryViewer/dist/year0-10-history-viewer.html`

Manager decision requested: **accept as LOCAL CANDIDATE**, or return a specific bounded correction. No main integration, product acceptance, release, Unity build or L4 waiver is implied.
