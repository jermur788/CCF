# Multi #882 — independent final review

**Verdict: NEEDS CHANGES.** One blocking request-validation defect and one non-blocking keyboard-focus defect. No repairs authored.

Reviewer: OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), Codex/T3 Code, high effort, 2026-11-20. **I am a separate session/context from the implementation author, not the author session or its child/helper.** Same-family separate-session independence is explicitly owner-authorised by the direct task and [routing amendment](https://github.com/jermur788/CCF/issues/882#issuecomment-7049542173). I inspected committed artifacts and reproduced evidence in my own worktree; I did not reuse the author's reasoning or raw runtime evidence as verification.

This is the ONE terminal independent review. It is **not a human owner playtest, G2 owner acceptance, a merge, integration qualification, or PLAYER-TEST-READY**. A repair/re-review needs a new Manager #885 decision.

## Candidate, authority and custody

- **Exact source reviewed and executed: `43bfba8e82adcc31d71dbb13d2b0c00cd682ec4d`.**
- Source branch: `feat/multi-882-competition-unity-ui-firstpass-v1`; base `03547c6d56b6ba05742e71714d0cc2a4431ecf78`.
- `ae01139e73a189d52f2ae48306e2dc45b16aaae8` adds only the author's result Markdown. It is not a different code candidate.
- Physical host verified `DESKTOP-DL1BQI7`, WSL2 Ubuntu, user `jer`. All shell/Git/build/test/HTTP and evidence-file operations ran through WSL bash. Browser inspection used the shared T3 browser against my own WSL listener.
- Own detached worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-Review`. HEAD matched the source SHA; Git status was empty after checkout completed, before tests, and after shutdown. Primary checkout was clean and preserved.
- [Actual read-only RUNNING registration](https://github.com/jermur788/CCF/issues/882#issuecomment-7049638941) preceded deep source inspection/builds.
- Read live #882/all comments, author result, binding dispatch, Crop-first amendment, original approved packet/all four specs, [owner five-path grant](https://github.com/jermur788/CCF/issues/863#issuecomment-7048666134), #879/#880/#881 histories, project workflow/context and live main coordination. Later routing and #885 succession take precedence over the older reviewer/Manager names. Candidate context-source commit inspected: `62e39ec621e38a84030c08b35a171e297056c9ed`.
- No source/test/UI file was edited. Frozen P0 was copied read-only from the qualified `/home/jer/Development/CCF-Review-877/wt-owner/Tools/MultiRackSitkaP1Lab/.p0ref`; never rebuilt.
- My browser process PID `1122824`, port `45425`; my fresh negative-case process PID `1123122`, port `44487`. Both OS-assigned loopback listeners were stopped by exact owned PID and exited normally. My browser tab was closed. No capability URL is published.
- Protected PID `390578` / port `33767` (#867), PID `1750123` / port `46311` (#877), their worktrees/fixtures, and the author's worktree were not reused, changed or stopped; both protected PIDs remained alive at completion.

## Findings

### B1 — BLOCKING: new Crop commands accept incomplete and ambiguous fences/requests

**Locations:** [PlayerApi.cs:83](https://github.com/jermur788/CCF/blob/43bfba8e82adcc31d71dbb13d2b0c00cd682ec4d/Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs#L83), :85, :252, :267 and :288. The new Crop routes call `CommandAsync<TreeRequest>`, which calls `ReadRequestAsync` with duplicate/complete-shape validation disabled. Only the diagnostic route opts into it at :194. JSON deserialisation silently supplies zero for omitted numeric fence members and uses the last duplicate property.

**Independent reproduction:** in a fresh own process at Year 0/work 0/plan revision 0, read the actual view, construct its fence, then omit all four members `fence.planStateRevision`, `fence.source.ecologicalYear`, `fence.source.annualRevision`, `fence.source.workRevision`. POST that incomplete fence and `treeId: P30-045` to `api/crop/mark`. Actual result: **HTTP 200, Crop set [P30-045], plan revision 0 → 1**. Posting a diagnostic request with an omitted ecologicalYear instead returns HTTP 400 / BadRequest. Separately, a Crop body containing `"treeId":"P30-002","treeId":"P30-001"` returned **HTTP 200**, marked P30-001 and bumped revision 4 → 5. Reproductions are preserved in `crop-missing-fence.json` and `crop-duplicate.json`.

**Why blocking:** the newly authorised state mutations require a supplied complete session/full-source/plan-revision fence and strict request validation. The current parser lets malformed or ambiguous requests commit state. Explicit wrong nonzero values still refuse correctly; this is not evidence of a Host/Origin/capability bypass or a changed annual engine. The permissive generic parser predates this task, but these new routes inherit its gap; the new diagnostic's stricter checks do not protect Crop commands.

**Minimal repair direction:** apply complete required-member and recursive duplicate validation to both new Crop routes before invoking Core, preserving their existing guards and 409 stale-value refusals. Add narrowly additive HTTP checks proving missing numeric members and duplicate tree/fence/source members return 400 without mutation, including at zero-valued initial clocks. Do not weaken existing tests or alter protected engine files. Reviewer authors no repair.

### N1 — NON-BLOCKING: selecting a competitor discards keyboard focus

**Locations:** [app.js:1061](https://github.com/jermur788/CCF/blob/43bfba8e82adcc31d71dbb13d2b0c00cd682ec4d/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L1061), :1072 and :1091; existing restoration mechanism :392–408.

Competitor/inspect/return buttons are rebuilt by `renderCompetition`, but lack the stable `data-focus-key` used by `renderAll`. In the actual browser, focus the first competitor button and activate its click: P30-046 is correctly selected and its row is highlighted, but `document.activeElement` becomes **BODY**, losing the position in the long Plan pane. This reproduction used DOM focus/click; the later native Enter probe delivered no key event and is not counted as a native pass/failure.

**Why it matters:** keyboard users lose their place between selecting a competitor and recording the reason or returning to Crop. The workflow remains possible through navigation, so this is non-blocking.

**Minimal repair direction:** give rebuilt diagnostic controls stable focus keys, or explicitly move focus to an appropriate selected-tree/reason control after activation. Use the existing focus-restoration approach. No implementation was made.

## Independently verified facts

### Scope and protected artifacts

- Source diff versus base: **exactly 13 authorised files**, **906 additions / 141 deletions**. Final author-report head versus base: those 13 plus one handoff Markdown. No out-of-grant path.
- `tests/Core.Tests/Program.cs`: exactly two added registration lines.
- `tests/ui-smoke.sh`: exactly one added 102-line block, no removed lines. Removing the added block reconstructs the base file **byte-for-byte**; no original guard changed.
- AnnualRules, AnnualEngine, WorkContract, FixtureAccess, DerivedManifest, TrustTables and ManifestGenerator compared byte-identical to base. The exact-path audit also excludes UI Program, project/package files, fixtures, P0, saves, Unity Assets/Packages/ProjectSettings and unrelated paths from the diff.
- Four-way P0 checked: csproj expected value, actual DLL bytes, .sha256 record and provenance dllSha256 all equal **`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`**. No P0 build.
- Committed derived fixture hash **`bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`**.
- Smoke no-write gates W01–W05 passed. Its P0 tracked-file digest stayed `9e218ca7f29c73895ad4a66dc6ca90cf1a844126bd74e417e0d8cdc0489e30e8`. My separate browser/negative run window preserved **136/136** lab file size/SHA256 entries, including build/reference files. Empty browser stderr, normal “Session ended; nothing was saved.” shutdown, clean tracked status.

### Reproduced commands and counts

From my own detached review worktree root, SDK **8.0.134**, TMPDIR outside the repository:

```bash
TMPDIR=/tmp /home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SitkaP1.Core.Tests.csproj
TMPDIR=/tmp /home/jer/.dotnet/dotnet Tools/MultiRackSitkaP1Lab/tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
TMPDIR=/tmp /home/jer/.dotnet/dotnet build Tools/MultiRackSitkaP1Lab/src/Ui/SitkaP1.Ui.csproj
TMPDIR=/tmp bash Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh /tmp/ccf-882-review-evidence/smoke
```

**Both builds succeeded, 0 warnings / 0 errors. Core: 108/108 passed, 0 failed. Smoke: 178 passed, 0 failed.** These exactly reproduce the author's final counts. The pre-change 92/92 and 140/140 baselines were not separately executed by me; preservation is established by the unchanged original tests/guards and their inclusion in these final runs. No extra review probes are added to the reported suite counts.

### Formula, Crop and security

- Inspected diagnostic arithmetic against the real AnnualRules implementation: same float32 operations, ordinal-ID accumulation, inclusive 8 m cutoff, target DBH floor 1 cm and distance floor 0.5 m.
- Executed PC-01 checks all 100 living fixture trees against actual annual PreGrowthCi float bits and neighbour counts, and sums contributions in annual order. PC-02–08 exercise ordinal ties/ranking, zero neighbours, exact8m/next-float exclusion, coincident centres, DBH/distance floors, dead exclusion, pending/approved/cancel previews, exact post-Execute equality, immutable views/reports and ten-year unchanged annual/RNG reports. These are meaningful source oracles, not hard-coded author totals.
- Diagnostic consumes one immutable view; inspected code has no growth/mortality/RNG invocation, write or independent ecological state. JavaScript renders server numbers and calculates only drawing/navigation geometry.
- Crop lives in the committed WorkRoot as copied read-only session state; same-lock Core commands enforce supplied fence equality, increment plan revision, refuse pending/approved Fell, require explicit unmark before Fell and preserve unrelated approved orders. Execute preserves marks. Year+1 reconciles dead Crop IDs in the same root publish; CR-06 actually exercises natural deaths, CR-07 proves ten-year no biological/RNG change and unchanged picking. CR-01–08 all pass.
- Existing capability, exact Host/Origin, method, JSON content type, body-size and no-CORS guards remain. Smoke reproduced 403/404/405/413/415 and stale409 cases; diagnostics strictly reject missing/duplicate/unknown members and attach the same fenced view. Routes stay in allowed PlayerApi; UI Program/listener unchanged. **Crop shape validation remains B1.**

### Browser observations, explicitly bounded

I launched the exact built candidate against a scratch copy of the committed fixture in my own process. Most actions were DOM click/KeyboardEvent/fetch, not a trusted full user playtest.

- Actual **1280×720**: retained P30-045 server-side, revision 0→1; CI **4.4735703**, 22 neighbours, top IDs P30-035 / P30-046 / P30-044 / P30-056 / P30-036; 94 unrelated crowns dimmed.
- Selecting P30-035 keeps P30-045 as diagnostic target and offers the explicit truthful competition-release intent. Pending preview **3.941147**; approval leaves 100 living/work0; separate confirmation starts on Back and hides raw PlanId/key in ordinary innerText. Execute leaves 99 living/work1; actual CI **3.941147**, exactly preview. Replay returns replayed=true and one event. Year+1 and reload retain surviving Crop, work1, 99 living.
- At approval, Execute bottom **598.421875 px**, Cancel bottom **544.828125 px**; confirm bottom **492.234375 px**, all within 720. After Crop marking, document extent **1280×720**, map **834×414.375 px**, seven navigation controls **46×46 px**; no page scroll required to reach them.
- Pan is real scroll of the viewing window: east/south moves it, repeated presses clamp at **399/786 px**, west/north at 0/0; Fit restores 0/0 and disables all pan directions. Zoom overlay is a sibling of the SVG, so its click cannot bubble through the map's selection listener. One trusted browser click on Zoom in and one trusted Tab event were recorded, moving focus to Zoom out; this narrow result is not a complete native workflow.
- Measured actual1920×1080 / 960×720 / 640×720 / 360×720 after resize **and reload**, not just requested dimensions. Document widths respectively1920/945/625/345; no horizontal page overflow. Wide layout two columns; narrow layouts one column with vertical scroll and 46px controls. This is CSS reflow evidence, not literal200% browser zoom.
- Browser negative checks: stale diagnostic409, missing diagnostic source member400, wrong capability404; retained Crop Fell409, explicit unmark200, stale Crop409, pending Fell blocks Crop409. Approved-Fell conflict/frozen-plan protection was independently reproduced by the Core/smoke suites.
- Normal plan/receipt/history code excludes raw machine IDs; exact identifiers remain in immutable confirmation/API/replay. Unsupported health/form reasons are absent; “Other” requires nonblank custom text and the Core reason gate is unchanged. Light/seed/regeneration explicitly say **NOT MODELLED**.
- Screenshot capture succeeded here. A rendered crop image shows charcoal/slate panels, actual Sitka conifer whorls, dimming and numbered rings. **Captured PNG is only1018×720 while DOM viewport was1280×720**, so the right side is cropped; it does not establish full-screen screenshot coverage. Full-layout reach above comes from DOM measurements.

## Manager judgement calls, separate from defects

1. **Friend/internal compatibility:** PS-18/PS-19 remain byte-unchanged and pass. Internal diagnostics/Crop methods avoid widening the frozen exported-type/public-adapter list; actual mutations still enforce the same Core lock/fence/refusal path. This is an acceptable bounded implementation choice for the granted session-only feature, subject to Manager acknowledgement. **The UI friend relationship is newly added**, not pre-existing: `InternalsVisibleTo("sitkap1-ui")` grants that assembly access to all Core internals, including existing seams. No inspected route uses those seams, but public-facade tests alone do not prove an unchanged assembly trust boundary. Do not describe those tests as covering all future friend-code access.
2. **Visual fidelity:** the panel/text/accent tokens match inspected Unity USS, ground/conifer bases match StandMapSymbols, and new pointed whorls stay within authoritative discs. Pending yellow, approved salmon and selection white remain inherited presentation choices rather than Unity's exact Fell-red/selected-ochre semantics. Manager/owner should decide whether those status colours and the uniform stylised whorls satisfy the requested visual fidelity; no invented broadleaf trees or biological outputs were found. No UI redesign is authorised by this review.
3. **Evidence/readiness:** passing suites do not erase B1. Full native usability, actual full-screen rendered evidence and human aesthetic acceptance remain separate gates. Manager #885 must decide the bounded repair packet and any later qualification; no automatic second review.

## Author-evidence honesty and NOT VERIFIED

The committed author's report explicitly separates author verification from independent review. Its screenshots, complete trusted/native input, touch, screen reader, literal Ctrl+200%, browser-Origin403 and headless-Edge limitations are consistently disclosed in its NOT VERIFIED/NOT RUN paragraph and issue handoff. Its HTTP Origin403 statement is supported by my smoke rerun. I did not independently establish that historical failed attempts or an automatic approval rejection occurred; those remain attributed author reports, not independently reproduced events. My later screenshot/single trusted input successes do not retroactively verify the author's runs. The report's broad malformed/missing/duplicate-body language must be read as **diagnostics coverage**, not a Crop validation pass; B1 demonstrates the gap. I found no committed screenshot/native/zoom result quietly promoted to a passed owner trial.

**NOT VERIFIED in this review:**

- Complete trusted/native Crop→competitor→reason→approve→Execute keyboard/pointer workflow; later Enter/Ctrl probes produced no key events despite tool success responses. DOM-driven functionality and the narrow successful trusted Zoom/Tab events are separately identified.
- Literal native browser200% zoom; Ctrl probe did not establish a change. Narrow CSS reflow is verified.
- Touch and screen-reader operation; comprehensive WCAG/contrast audit.
- Complete1280/1920 screenshots, pixel-identical Unity appearance and human owner aesthetic/playability acceptance. One partial rendered capture was inspected.
- Exact8m/coincident synthetic arrangements and natural-Crop-death notification in the browser; numerical boundary/death reconciliation are verified in real Core tests. Browser zero-neighbour exhaustion was not repeated; Core zero-neighbour case passed.
- Browser-enforced hostile-Origin403; HTTP Origin403 is reproduced. Headless Edge, Windows build, Unity Editor/Player, save/load persistence and Unity numerical parity were not run or claimed.
- Author's historical raw browser/failed-Edge/approval-rejection evidence; only committed claims were audited.
- Dashboard/Drive ingestion and published-state synchronization. I did not edit coordination/generated dashboard files; #885 must reconcile the durable issue/result receipts and operational mirrors.

## Reproduction evidence and disposition

Own external evidence directory: `/tmp/ccf-882-review-evidence` (temporary local storage, not a permanent archive). It contains both build logs, complete Core/smoke logs, browser measurements, partial PNG, request reproductions and before/after file hashes. Runtime stdout contains private capability URLs; do not publish it unredacted.

| Evidence | SHA256 |
|---|---|
| core-run.txt | 1cf1691bc9958cd621cd01636d34f5c8c21c636998b05e3ff5e5922114e690e1 |
| smoke/ui-smoke.txt | 23c173cf6d27431246c14b0d4f3968957c1f9b85eec2c7ab761cf46d9d7191b9 |
| crop-missing-fence.json | 52ddfc14b0b5a84195ed12d38070926347fde238dcd950168d6ba103ed28f4f3 |
| crop-duplicate.json | d351d0ef6c99ae46185f1258fc5572489a0e8912fac8a43768fdf1fba715c4f1 |
| browser-results.json | 14c41ef695fe67deaa2a7af9b994106bdd68ffe90dae95b652242ca46bbf2a01 |
| browser-no-write.txt | a3f2db63aa8869a85b4fb0d72fdcfad6f9a38d041e2a051fc0886aeff521317d |
| crop-partial-1280x720.png | 96a5412ba35c6267552a718e48845ee985725421a6cfc1a4dcc551fb883dbc1f |

Two exploratory browser scripts used wrong DOM IDs, and one post-Execute diagnostic probe raced the UI's execution response and received a null report before a fresh read. Those attempts were not counted as successful assertions; corrected fresh reads established the workflow values reported above. No source was changed to make a check pass.

**Disposition: NEEDS CHANGES for source43bfba8e82adcc31d71dbb13d2b0c00cd682ec4d.** Core108/108 and smoke178/178 reproduced; B1 blocks approval, N1 is non-blocking. Return to Manager #885 for a bounded decision. No source fixes, main merge, second review or G2 acceptance is included.

