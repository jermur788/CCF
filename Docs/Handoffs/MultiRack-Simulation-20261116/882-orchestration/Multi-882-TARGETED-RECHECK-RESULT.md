# Multi #882 — targeted independent repair re-check

**Verdict: TARGETED RECHECK PASS.** B1 is refused before mutation; N1 focus restoration is reproduced with DOM activation in a real browser. Core **108/108**, smoke **287/287** (**178 original + 109 additive**), zero failures. Both builds have **0 warnings / 0 errors**. No source/test fixes authored.

Reviewer: **OpenAI GPT-6.1 Sol (gpt-6.1-sol), fresh Codex/T3 Code session, high reasoning effort, 2026-11-20**. I am **not the repair author and not the original reviewer's session/context**. This is the owner-authorised **targeted re-check**, **not a full re-review, owner playtest, G2 acceptance, integration qualification or merge**. It does not declare PLAYER-TEST-READY or overturn the historical verdict on the unrepaired source.

## Authority, exact candidate and custody

- Read the [original NEEDS CHANGES verdict](https://github.com/jermur788/CCF/blob/accf5122e02f5bb60490a4867c80e5e443d97bc4/Docs/Handoffs/MultiRack-Simulation-20261116/882-orchestration/Multi-882-INDEPENDENT-REVIEW-RESULT.md), [repair result](https://github.com/jermur788/CCF/blob/32a3ddc46e032c92431d113325376adfcbdc0cd0/Docs/Handoffs/MultiRack-Simulation-20261116/882-orchestration/Multi-882-B1-REPAIR-RESULT.md), live #882 comments, #881, latest #885 comments and [binding owner repair/re-check decision](https://github.com/jermur788/CCF/issues/885#issuecomment-7049935573). Project workflow/context and live GitHub main coordination were inspected. Candidate context-source commit: 62e39ec621e38a84030c08b35a171e297056c9ed. The direct owner packet controls this bounded review.
- **Executed source: e11dd5192d7a89abf859613ce92814bdd6338ea2** in own detached worktree **/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-Recheck**. Host verified DESKTOP-DL1BQI7, WSL2 Ubuntu, user jer. All shell/Git/build/test/HTTP/file operations used WSL bash; browser inspection used T3 against my own listener.
- Git independently proves the direct chain: original base **03547c6d56b6ba05742e71714d0cc2a4431ecf78** → reviewed source **43bfba8e82adcc31d71dbb13d2b0c00cd682ec4d** → author-report head **ae01139e73a189d52f2ae48306e2dc45b16aaae8** → repaired source **e11dd5192d7a89abf859613ce92814bdd6338ea2** → repair-report head **32a3ddc46e032c92431d113325376adfcbdc0cd0**. No rewritten reviewed history.
- [Complete truthful RUNNING claim](https://github.com/jermur788/CCF/issues/882#issuecomment-7050284371) preceded source inspection/builds. The first registration comment was accidentally truncated by shell quoting to “RUNNING”; the linked correction supplied model, independence, exact SHA, own lane and read-only scope before substantive checks.
- Inspection worktree was clean before and after tests. No tracked/untracked source/test edits; only ignored build/reference outputs. The clean primary checkout and author's branch/worktree were preserved. No source commit or author-branch push.
- Own fresh server **PID 1681616 / OS-assigned loopback port 36361** served a scratch copy of the committed fixture. It was stopped by exact owned PID using SIGTERM, exited normally with “Session ended; nothing was saved.” and empty stderr. Own browser tab closed. Smoke independently owned/stopped its own ephemeral process.
- Protected **PID 390578 / port 33767 (#867)** and **PID 1750123 / port 46311 (#877)** remained alive; neither listener/worktree/fixture was reused, changed or stopped. Only the already-qualified #877 .p0ref files were read and copied into my ignored reference directory, without changing the originals. No capability URL is published.

## Verified facts: repair scope and preservation

Relative to ae01139e, the **source SHA has exactly three changed files**, 135 additions / 12 deletions:

1. Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs — 11 additions / 8 deletions.
2. Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js — 8 additions / 4 deletions.
3. Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh — **116 additions / 0 deletions**.

The later 32a3ddc4 head adds exactly one fourth path: **Docs/Handoffs/MultiRack-Simulation-20261116/882-orchestration/Multi-882-B1-REPAIR-RESULT.md** (139 lines). No other repair path differs.

All Core files are byte-identical to 43bfba8e, including:

| Core file | Identical Git blob at reviewed and repaired source |
|---|---|
| AnnualContinuation.cs | d94dd5b5bb7da8ba90831392d0bf48a775af8530 |
| PlayerSessionView.cs | 2de0d8a3b6935c3e55ff0d4d00249be5cada9d9b |
| PlayerMapScene.cs | 944f95afd27b52222fff5ee04bf51284f6bfe9f8 |
| PlayerWorkPlanAdapter.cs | 566bc25da4e46babea8e0a9540cecedd4e9c41b2 |

The exact diff excludes index.html, app.css, Core tests, UI Program/listener, project/package files, fixture/P0/save data and Unity Assets/Packages/ProjectSettings. No package/dependency or ecological/fence-model change.

A byte-line comparison of smoke against 43bfba8e permits only equal/insert operations: **all 1,190 original lines preserved exactly and in order**, 116 inserted lines. No original guard/expectation was removed or weakened. Added browser-stand-in setup removes its Crop designation before the original page guard flow; it does not replace those assertions. Counting actual PASS lines independently reproduces **178 original + 109 additive = 287** (102 initial HTTP checks + 1 completion/stderr check + 3 retained-Crop checks + 3 focus checks).

## Verified facts: B1 adversarial reproduction

[PlayerApi.cs at the candidate](https://github.com/jermur788/CCF/blob/e11dd5192d7a89abf859613ce92814bdd6338ea2/Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs#L83): both mark/unmark opt into strict parsing. Lines 291–296 validate recursive unique member names and required members before deserialization/Core invocation; lines 307–324 require the whole fence/source shape, including present non-null numeric members. Exact-name deserialization rejects unknown names. Existing capability/Host/Origin/method/content-type/body-size guard code and Core stale-fence paths remain.

My separate Python standard-library HTTP probe ran against a **fresh initial state: ecologicalYear 0, annualRevision 0, workRevision 0, planStateRevision 0, Crop []**. It did not use the author's probe or raw runtime results. For every refusal it compared fresh **entire views** before and after; all initial cases also matched the original initial view.

| Independent input | Observed / required result |
|---|---|
| Mark P30-045 with fence.planStateRevision and source.ecologicalYear/annualRevision/workRevision all absent | **400**, revision **0 → 0**, Crop **[] → []**, entire view unchanged |
| Same combined omission on unmark; every individual source member or plan revision omitted separately | **400**, entire view unchanged |
| Duplicate treeId, including P30-002 then P30-045; duplicate fence/source objects; each nested source/plan member duplicated; escaped equivalent treeId member | **400**, entire view unchanged |
| Missing/null required objects/IDs; extra unknown property at request/fence/source level; wrong object/array/bool/string/null types; fractional-token/overflow integer values | **400**, entire view unchanged |
| Diagnostics with source.ecologicalYear absent | **400**, initial revision **0 → 0**, entire view unchanged |
| Complete stale session, workRevision, ecologicalYear or planStateRevision | **409**, entire view unchanged |
| Each Crop route: hostile Origin / wrong Host / wrong capability / wrong method / wrong content type / oversized body | **403 / 403 / 404 / 405 / 415 / 413**, entire view unchanged |
| Legitimately current, complete fence with explicit source clock zeros and planStateRevision 0; mark P30-045 | **200**, revision **0 → 1**, Crop **[] → [P30-045]** |
| Reuse that complete revision-0 fence after the mark for unmark | **409**, revision **1 → 1**, Crop preserved |
| Fresh complete revision-1 fence; valid unmark | **200**, revision **1 → 2**, Crop **[P30-045] → []** |

**162/162 separate refusal/no-mutation probes passed**, plus the valid zero-fence mark and valid fresh-fence unmark. These are extra review probes, not additions to the Core/smoke suite totals. The exact generated request bodies, statuses and state comparisons are retained in http-probes.json; executable reproduction is probe.py in my evidence directory below. Smoke separately proves malformed unmark cannot remove an actually retained Crop and preserves diagnostics and existing guard regressions.

**B1 outcome: repaired within the authorised boundary; no bypass reproduced.** Zero is accepted when explicitly supplied and current; absence/ambiguity is rejected. Complete stale requests still reach Core and conflict rather than being misclassified as malformed.

## Verified facts: N1 real-browser DOM focus

[app.js at the candidate](https://github.com/jermur788/CCF/blob/e11dd5192d7a89abf859613ce92814bdd6338ea2/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L1049) adds stable Inspect/competitor/Return focus keys used by renderAll's existing restoration. Return explicitly lands on Inspect when its own button disappears. Loading preserves the Inspect control.

In an actual **1280 × 800 browser**, with **Plan visible**, I selected P30-045 through a DOM-dispatched quick-ID KeyboardEvent, retained it through DOM click, then used DOM focus/click on the real rendered controls:

| Activation | activeElement after activation |
|---|---|
| First competitor P30-035 | **BUTTON**, key **competition-tree-P30-035**, selected P30-035, aria-pressed true, inside **#pane-plan** |
| Return to target P30-045 | **BUTTON**, key **competition-inspect**, enabled, selected P30-045, inside #pane-plan |
| Inspect refresh: immediate loading and completed response | **BUTTON / competition-inspect**, enabled, inside #pane-plan; five competitors rendered |

Recorded click events have **isTrusted=false**. This is real-browser DOM behaviour, not native-input evidence; smoke BFOCUS1–3 are an additional DOM-stand-in regression witness.

A native Enter tool probe after focusing a visible competitor returned tool success but recorded **zero keydown/keyup/click events**, left selection P30-045 and did not activate it. **Native keyboard activation remains NOT VERIFIED**, not a passed workflow or an application failure. An earlier native click timed out while Plan was not visible; final DOM evidence above was repeated with visible Plan. The initial exploratory ancestor check used the wrong selector #plan-panel; the corrected #pane-plan check is true in every final observation.

**N1 outcome: repaired for the independently reproduced DOM activation/redraw path; focus no longer falls to BODY.**

## Reproduced gates and integrity

From the detached candidate root, SDK **8.0.134**, using copied prebuilt pinned .p0ref (never rebuilt):

```bash
TMPDIR=/tmp /home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SitkaP1.Core.Tests.csproj
TMPDIR=/tmp /home/jer/.dotnet/dotnet Tools/MultiRackSitkaP1Lab/tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
TMPDIR=/tmp /home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj
TMPDIR=/tmp bash Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh /tmp/ccf-882-recheck/smoke
git diff ae01139e HEAD --check
```

- Core and UI builds: exit 0, **0 warnings / 0 errors** each.
- Real Core executable: **108/108 passed, 0 failed**.
- Full smoke: **287 passed, 0 failed**; all original guard assertions remain included. W01 preserved **173 filesystem entries**, including bin/obj/.p0ref; W02 ~/.aspnet unchanged; W03 P0 tracked-file digest **9e218ca7f29c73895ad4a66dc6ca90cf1a844126bd74e417e0d8cdc0489e30e8** unchanged; W04 both labs' Git statuses unchanged; W05 fixture unchanged.
- Four-way P0 (csproj expected value, actual DLL bytes, .sha256 record, provenance dllSha256) all equal **9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3**. No P0 build invoked.
- Derived fixture remains **bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4**.
- My independent HTTP/browser window preserved **136/136 lab file SHA256 entries**, including generated/reference files. No forest-save.json exists anywhere in my inspection worktree. Smoke's no-write gates, runtime shutdown and inspection substantiate no save written in these runs.
- git diff --check passed. Final inspection and primary checkout statuses empty. No Unity or generated Unity folder touched.

## Repair-report honesty, Manager judgement and NOT VERIFIED

The repair report's code/scope/count/hash/no-mutation and bounded DOM-focus claims agree with inspected code and my independently reproduced final runs. Its original-guard preservation claim is substantiated by byte comparison and the full smoke run. Its failure history, before-repair executions, past author process identities, historical raw evidence hashes, commit-identity correction and native failed attempts remain **attributed author reports**, not events independently repeated by me. This re-check does not require re-executing the old vulnerable candidate; the supplied original independent verdict remains the historical baseline. I found no final repair claim contradicted by the targeted reproduction.

**Manager judgement remains separate:** disposition of this narrow pass, owner qualification/readiness and any integration release belong to Manager #885/owner. Original review judgement calls about friend/internal compatibility and visual fidelity remain outside this repair and are neither reopened nor waived. No new feature judgement or repair cycle is authorised here.

**NOT VERIFIED:** trusted/native competitor activation or complete keyboard/pointer workflow; touch/screen reader/full accessibility; literal browser 200% zoom and new visual/screenshot/Unity fidelity qualification; browser-enforced hostile Origin (HTTP guards passed); Unity Editor/Player, Windows build, scientific/3D numerical parity, save/load persistence; owner aesthetic/playability/G2 acceptance; full-feature re-review or integrated-candidate gates. Dashboard/Drive ingestion and published-state synchronization remain an outstanding Manager operational item; no coordination/generated dashboard file was edited.

## Evidence and publication

Own external temporary evidence: **/tmp/ccf-882-recheck**, produced by runs from the detached inspection worktree. It is temporary local storage, not a permanent evidence archive. Runtime server.out contains a capability and must not be published unredacted. This one tracked Markdown is the durable result; no source/test evidence file is added to Git.

| Evidence file | SHA256 |
|---|---|
| core-build.txt | e66f50b6426434f5377bd01aef556b7926de8cd6949ef53dfbf97ea879820a9c |
| core-run.txt | 1353d45cf4e4ceffeac852d85bf8b88764d9e2b03166ccb7196e5834c84c2732 |
| ui-build.txt | a5b52e2fced1a7cab0ba5043cc96bb8946042b661b431fe13c33619531946418 |
| smoke-run.txt | 6033b05b0fd60ceb87236968641a15c202e94c58677d91911eb1024b1813924e |
| http-probes.json | 2d6e3b55a011f077a87bc46b22466fbc9588cbac067185f5b0697c6732117285 |
| probe.py | 0b82afa78f9cff393870f34c03f93f01b216df7e15206960a8f28e99c86f18ae |
| browser-focus.json | 10a8b8b909fec89080e8c53c9f3943c62547f81d8f35b2b2f7f8a269ac01d102 |
| integrity.json | 1cf32af8dd90fefd190957352b96a7f66d7bc5760923b7911e8ae43d5b69476b |

Publication lane: new **review/multi-882-sol61-targeted-recheck-20261120-v1**, based on live main **c94f2a2b0dee439cbe2a1269fc47d7421a3e1be2**, exactly this one new Markdown. Publication SHA, GitHub byte-identical readback and receipts to #882/#881/#885 are recorded in the issue comments after push. No original verdict overwritten and no main merge.

**TARGETED RECHECK PASS for e11dd5192d7a89abf859613ce92814bdd6338ea2.** Core 108/108, smoke 287/287, extra refusal probes 162/162; B1 repaired, N1 DOM focus repaired, native input unverified. Return to Manager #885 for disposition.
