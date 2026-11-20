# Multi #882 — bounded B1 + N1 repair author result

**Repair implemented and author-tested on the existing task branch. Fresh targeted independent re-check and Manager disposition remain required.** I am the repair author, OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), fresh Codex/T3 Code session, high reasoning effort. This is **not an independent review, owner acceptance, G2 acceptance, integration, merge or PLAYER-TEST-READY**.

## Authority and custody

- Start SHA: **`ae01139e73a189d52f2ae48306e2dc45b16aaae8`**, verified clean before any source edit.
- Final source SHA: **`e11dd5192d7a89abf859613ce92814bdd6338ea2`**, direct child of the start SHA. The later documentation commit adds only this report.
- Same branch: `feat/multi-882-competition-unity-ui-firstpass-v1`.
- Same worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-CompetitionUI`.
- Host `DESKTOP-DL1BQI7`, WSL2 Ubuntu, user `jer`. All shell/Git/build/test/HTTP/file operations through WSL bash; actual browser checks through the shared T3 browser.
- Sole source writer for this repair. Previous author session ended per owner packet; DeepSeek orchestrates and authored no repair code.
- [Truthful RUNNING claim](https://github.com/jermur788/CCF/issues/882#issuecomment-7049968034) preceded source changes.
- Read the independent verdict B1/N1 in full, author result, live #882/#885 return/owner-decision comments, project workflow/context and current GitHub coordination. Context-source commit `62e39ec621e38a84030c08b35a171e297056c9ed`; direct owner repair packet takes precedence over historical routing/status.
- [Independent NEEDS CHANGES verdict](https://github.com/jermur788/CCF/blob/accf5122e02f5bb60490a4867c80e5e443d97bc4/Docs/Handoffs/MultiRack-Simulation-20261116/882-orchestration/Multi-882-INDEPENDENT-REVIEW-RESULT.md); owner bounded repair/re-check decision recorded on [#885](https://github.com/jermur788/CCF/issues/885) and [#882](https://github.com/jermur788/CCF/issues/882).
- Protected #867 PID `390578` / port `33767` and #877 PID `1750123` / port `46311` were never reused or stopped and remained alive at completion. Their worktrees/fixtures and primary checkout were not changed.

## Exact file list

These are the only four repair paths relative to the start SHA:

1. `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs`
2. `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js`
3. `Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh`
4. `Docs/Handoffs/MultiRack-Simulation-20261116/882-orchestration/Multi-882-B1-REPAIR-RESULT.md` (this new file)

Source commit: 3 files, 135 insertions / 12 deletions. Smoke: **116 inserted lines, zero deleted lines**; all **1,190 original lines** remain byte-identical and in order. No original guard/test expectation changed. No Core, index.html, app.css, Core tests, project/package, fixture, P0, save, Assets, Packages, ProjectSettings or Unity source change. No package added, P0 rebuild, Unity run, Windows build or main update.

## What changed

**B1:** both Crop routes explicitly opt into the existing strict diagnostics parser before the Core command. The generic command helper forwards that opt-in. Recursive duplicate rejection and complete required-member checking now protect mark and unmark. The shared shape helper is renamed to describe a tree request, and rejects null required members as malformed. Unknown/case-ambiguous names still fail exact-name deserialization. All 12 source-token members, session ID, plan revision, source, fence and tree ID must be supplied. Numeric fields cannot silently become zero through omission.

Capability, exact Host/Origin, method, content-type, body-size, guard order and Core fence/refusal semantics are unchanged. A structurally complete but stale fence still reaches the existing Core refusal path and returns 409. No fence model or ecological/annual engine change.

**N1:** rebuilt Inspect, competitor and Return controls have stable focus keys. The existing renderAll restoration mechanism restores a competitor by exact tree ID. Returning removes its own button, so focus explicitly lands on the target's enabled Inspect control. Loading appends its status after the rebuilt Inspect control instead of deleting that focused control; focus therefore also survives asynchronous inspection refresh.

## B1 actual before/after reproductions

Fresh owned processes used scratch copies of the committed fixture. Before run used the unmodified start tree and an explicit successful UI build (zero warnings/errors). Initial source clocks were ecological year 0, annual revision 0, work revision 0, plan revision 0.

| Request | Unmodified HTTP / state | Repaired HTTP / state |
|---|---|---|
| Mark P30-045, omit fence.planStateRevision and source.ecologicalYear, annualRevision, workRevision together | **200**, revision **0 → 1**, Crop **[] → [P30-045]** | **400 BadRequest**, revision **0 → 0**, Crop **[] → []**, entire view equal |
| Duplicate treeId: P30-002 then P30-001, current complete fence | **200**, revision **1 → 2**, Crop **[P30-045] → [P30-001, P30-045]** | **400 BadRequest**, revision **0 → 0**, Crop **[] → []**, entire view equal |
| Diagnostics, omit source.ecologicalYear | **400 BadRequest**, revision **2 → 2**, Crop unchanged, entire view equal | **400 BadRequest**, revision **0 → 0**, Crop unchanged, entire view equal |
| Mark with complete fence but planStateRevision + 1 | **409 PlayerPlanStateStale**, revision **2 → 2**, entire view equal | **409 PlayerPlanStateStale**, revision **0 → 0**, entire view equal |

Before owned PID `1515286` / port `33769`; repaired reproduction PID `1517548` / port `38309`. Both stopped by owned PID after the requests; both run windows preserved all **136/136** lab file SHA256 entries, stderr empty.

Additive smoke checks exercise **both mark and unmark at zero clocks**, each omitted required member individually, the combined four-member omission, each required member duplicated (including fence/source nesting and identical-value duplicates), conflicting duplicate tree IDs, null fence/session/source, case ambiguity, complete stale session/source/plan fences, and each route's 403/403/404/405/415/413 guards. Every refusal compares the **whole current view**, including Crop and plan revision. Three additional checks against an actually retained Crop prove malformed unmark requests cannot remove it or bump revision. Original diagnostic missing/duplicate/unknown/guard checks also pass.

## N1 actual browser before/after

Actual T3 browser viewport **1280 × 800** against own WSL listener; real rendered document and app, not the smoke DOM stand-in. Tree selection used a DOM-dispatched quick-ID KeyboardEvent; Crop marking and activation used DOM click/focus.

- **Before:** focus first competitor P30-035, activate its DOM click. Tree P30-035 is correctly selected/highlighted, but activeElement becomes **BODY**, with no focus key.
- **After:** same activation leaves activeElement **BUTTON**, key **competition-tree-P30-035**, aria-pressed **true**.
- **After Return:** activeElement **BUTTON**, key **competition-inspect**, disabled **false**.
- **After Inspect refresh:** focus key **competition-inspect** survives both loading and completed report; five competitor buttons rendered.
- Added DOM-stand-in checks BFOCUS1–3 independently exercise the served app's Inspect refresh, competitor restoration and Return landing as automated regression witnesses. They are not browser/native evidence.

Before browser PID `1516300` / port `34423`; after browser PID `1520605` / port `33439`. Both owned servers stopped, tabs closed. The final browser window preserved **136/136** lab hashes; empty stderr and normal **“Session ended; nothing was saved.”**

**Native input NOT VERIFIED:** Enter and Tab sent through preview_act returned tool success but produced **zero recorded keydown/keyup/click events**. Enter did not select the competitor. Do not count those probes as native workflow passes or application failures. The successful before/after results above are specifically DOM focus/click evidence.

## Gates actually run

From the owned worktree root, SDK **8.0.134**, evidence outside the repository:

```bash
TMPDIR=/tmp /home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SitkaP1.Core.Tests.csproj
TMPDIR=/tmp /home/jer/.dotnet/dotnet Tools/MultiRackSitkaP1Lab/tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
TMPDIR=/tmp /home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj
TMPDIR=/tmp bash Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh /tmp/ccf-882-b1-repair-evidence/smoke
git diff --check
```

| Gate | Actual result |
|---|---|
| Core build | **0 warnings, 0 errors**, exit 0 |
| UI build | **0 warnings, 0 errors**, exit 0 |
| Core executable harness | **108/108**, 0 failed |
| Full UI smoke | **287/287**, 0 failed: **178 original + 109 additive** |
| Additive breakdown | 102 initial-zero/HTTP witnesses + 1 completion/stderr gate + 3 retained-Crop unmark checks + 3 DOM focus checks |
| Original smoke preservation | 116 added / 0 removed lines; original 1,190 lines preserved exactly |
| Scope / committed-byte audit | Only three permitted source/test files; tested bytes equal source commit |
| git diff --check | Clean |
| Smoke W01 | No lab file created/changed/removed, **173 entries**, including bin/obj/.p0ref |
| W02–W05 | ~/.aspnet unchanged; P0 digest unchanged; both labs' Git statuses unchanged; committed fixture unchanged |
| Saves | No forest-save.json exists in own worktree; no save written |
| Own processes / protected processes | Own listeners stopped/tabs closed; protected #867/#877 PIDs alive |

Full gates passed before the source commit against exactly its source bytes. No source changed afterwards; author identity correction changed commit metadata only. This report is a later documentation-only commit.

## Protected hashes

Four-way P0 verification: csproj expected hash, actual .p0ref DLL bytes, .sha256 record and provenance dllSha256 **all equal**:

`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`

Existing lane's pinned reference reused read-only; **never rebuilt**. P0 tracked-file digest before/after smoke:

`9e218ca7f29c73895ad4a66dc6ca90cf1a844126bd74e417e0d8cdc0489e30e8`

Committed derived fixture before/after:

`bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`

## Failed attempts retained

- Initial PowerShell-to-WSL quoting failed a documentation read; a GitHub raw-media request returned base64 JSON instead of decoded Markdown. Corrected with encoded bash input and explicit content decoding; no source change.
- Initial helper used unavailable JavaScript TextEncoder; corrected the wrapper before commands ran.
- First preview open used unsupported about:blank; next HTTP open got connection refused because its launcher process had ended with its tool call. Corrected with a persistent owned launcher; no other listener reused.
- One post-edit build/pin command lacked the lane cd and failed to find project/pin paths. Its subsequent after probe hit the old built binary and correctly failed its 400 assertion. No project build or source write happened in the primary checkout. Build failure saved as failed-wrong-cwd-build.txt; corrected lane build/probe then passed.
- First full smoke: **255 passed / 1 failed**, J31 stopped on a null Inspect control during automatic report loading. Evidence in smoke-failed-async.txt. Added a wait for the automatic report and preserved Inspect during loading (N1 scope); full rerun **287/287**. No original guard changed.
- First smoke-count categorization script classified B100–102 as original and failed its own assertion. Corrected numeric matching; actual count is 178 + 109 = 287.
- The first local, unpublished repair commit inherited the shared Git config's DeepSeek author name. Corrected only that new commit with command-local OpenAI GPT-6.1 Sol identity and existing owner noreply email. Final source is e11dd5192d7a89abf859613ce92814bdd6338ea2; reviewed history and shared Git config unchanged.
- Native Enter/Tab probes delivered no events as disclosed above.

## Evidence and remaining gates

External temporary evidence: `/tmp/ccf-882-b1-repair-evidence`. Runtime stdout contains private capabilities; do not publish those logs unredacted. This tracked report is the durable summary.

| File | SHA256 |
|---|---|
| before-http.json | dd0c978bcc0d1e18dc176d102c9d755793a6c33c9cb96f7d59fe4f8dfa2e7b5e |
| after-http.json | 5505487a43fb2f548833f1440261d61aa6df4e40264e81c30ac79bdb1d680187 |
| core-build.txt | 3a3dae34ca79847186d8ea8ed6e17168bc7e994304170561578e9ba71a81e81a |
| core-run.txt | a1105f33c981c20be02b43a334932c7b68861a98b4063a97f9ae43f67c2bb761 |
| after-ui-build.txt | 8edfe8bf78baf6718a6f2fad13f487f91dff608c672a46cb3bb972bd82775960 |
| smoke-run.txt | 200050839fa62ea86fa4c1d05a06abd4b2c2aa4116fb90c80d2c7b92d9306662 |
| browser-focus.json | 1c38e8367dc7f322c997f09c207a154cc93ecb120443f0a041922d147fb17770 |
| browser-no-write.json | 3f65b42358772c037b34075dcfd567af0dd2e28c5388fe1a3ff963b246d387c6 |

**Still NOT VERIFIED / NOT RUN:** fresh targeted independent re-check; trusted/native keyboard or complete pointer workflow; touch/screen reader, full accessibility, literal browser 200% zoom, new screenshot/visual-fidelity qualification; browser-enforced hostile Origin (HTTP Origin guards passed); Unity/Windows build, save/load persistence, scientific/3D parity, owner G2/aesthetic acceptance; dashboard/Drive ingestion and published-state synchronization. None is inferred from passing author tests. Existing broader feature limitations remain.

For the authorized fresh reviewer: use the exact final source SHA, same pinned hash and committed fixture identity in an independently owned lane. Run the four commands above. In a private own listener, quick-ID P30-045 → Retain as Crop → focus/activate first competitor → verify focus stays on its exact-ID button → Return → verify enabled Inspect focus → Inspect refresh → verify focus survives loading. Distinguish DOM dispatch from actually observed native events. Reproduce the malformed HTTP cases and compare entire views before/after; complete stale fences must remain 409. No Unity steps apply to this HTTP/JavaScript repair.

**Author handoff only:** ready for the owner-authorized targeted independent re-check. No blocking failure remains in the author-run repair gates; acceptance remains with the fresh reviewer/Manager/owner. Dashboard/Drive reconciliation is an outstanding Manager operational item. Push/readback and issue receipts are to be recorded on #882, #881 and #885; no ongoing source writer or live candidate server is left by this handoff.
