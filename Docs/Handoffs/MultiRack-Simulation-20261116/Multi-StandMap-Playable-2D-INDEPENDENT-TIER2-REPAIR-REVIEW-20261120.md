# CCF #849 — Independent Tier-2 focused repair re-check

**Verdict for the repaired area: NEEDS CHANGES.** D1, D2, D3 and D5 are resolved on **d2c04cbbca380b4a5076807c72f19632bf2543a8**. D4's undersized map targets are corrected, including the stump. **One MINOR residual remains:** A-4 requires a single column at 200% zoom, but a 1920 px browser window at actual native 200% still produces two columns. There is no residual CRITICAL or MAJOR finding established in this focused re-check.

| Field | Record |
|---|---|
| Reviewer | GPT-6.1 Sol, independent nonauthor; no implementing child or additional reviewer used |
| Date / host | 20 November 2026; DESKTOP-DL1BQI7, Windows / WSL2 Ubuntu, user jer |
| Candidate source | `d2c04cbbca380b4a5076807c72f19632bf2543a8` |
| Exact diff under test | `4e95e787e93cdbd1832bc6fa545ef35acb5c2e76..d2c04cbbca380b4a5076807c72f19632bf2543a8` |
| Author documentary addendum | Section 0 at `b7045ecd`, fetched separately; not substituted for source or independent evidence |
| Review branch | `review/multi-849-sol-tier2-repair-20261120-v1`, created directly from the candidate |
| Context | `62e39ec621e38a84030c08b35a171e297056c9ed` |
| Authority | MULTI-STANDMAP-PLAYABLE-2D-V1 at the candidate, including A-4; latest direct re-check packet |
| Gate / scope | Focused G2 review of D1–D5, the eight-path repair and plausible regressions; no G4 integration decision |
| Spec conformance in this scope | D1/D2/D3/D5 PASS; A-4 target size PASS in the measured conditions, single-column clause FAIL at wider native 200% |

This report supersedes **only the affected dispositions listed below** in the [prior review at 7fd3b72cd3e50b0f33574210a6417e651c87312e](https://github.com/jermur788/CCF/blob/7fd3b72cd3e50b0f33574210a6417e651c87312e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-INDEPENDENT-TIER2-REVIEW-20261120.md). All untouched dispositions and deferred gates stand. This is not Manager acceptance, integration, integrated qualification, release or user acceptance.

## Independent lane and preservation

I made a **fresh remote clone**, not the author's lane or the previous review clone:

```text
/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Recheck
```

It was clean at the exact source SHA before builds and source-clean after every run. Sparse checkout materialised Tools, Docs and specs, plus the four exact tracked Assets inputs already required by the existing build/provenance tests: SitkaGrowthModel.cs, SimulationRandom.cs, Species/SitkaSpruce.asset and TreeSpeciesDefinition.cs. All tracked lab files were LF. Pre-build inspection found no competing dotnet/MSBuild process and no Git lock in the new clone.

The live GitHub coordination board was fetched at start and refreshed before publication; its final blob is **10e6077245f32730b7de7e5e7eb618d40085da42**. Its historical author-custody limitations are unchanged. This review does not retrospectively verify the author's pre-first-write reservation, #846 release or the registered/actual lane discrepancy.

I copied P0 only from the authorised A-1 lane's `.p0ref`. The DLL, csproj pin, `.sha256` and provenance each matched:

```text
9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
```

**P0 was never rebuilt.** All **52 P0 source-file checks** and its before/after metadata listing remained identical; the copied DLL still has that hash. The repair audit found exactly **8 modified paths, +938/−46**, no additional source path or mode change. Fifteen protected objects/trees, including Assets, ProjectSettings, P0, fixtures, biology/work engines, CLI, solution, existing projects, test registration and build configuration, are unchanged from the repair parent. No candidate edit or fix was made. The sole tracked review change is this Markdown.

## D1 — resolved: immutable confirmation and original race

Inspection of `app.js` confirms that `confirmationOf` copies the session, full source token, plan-state revision, PlanId, execution key, year and exact ID/reason orders, and recursively freezes the copy. The dialog renders that copy. Confirm sends that copy's fence and identities. `confirmationStillShown` compares the complete source and plan/order identity, and any differing arriving view withdraws the dialog. Back and Escape clear the intent.

I independently restaged the original race in the **real served page**, with trusted mouse input:

1. Approve A for **P30-005**, reason “Reviewer race plan A reason”, at plan-state **8** and work revision **2**.
2. Hold the page's real Re-read fetch before it reaches the server. Open Execute; the dialog shows A's PlanId, P30-005 and its reason.
3. Click the dialog's Execute while that read is pending. **No Execute request leaves the page; the dialog stays open.**
4. A separate client using the same session cancels A, adds **P30-006** with B's different reason, and approves B at plan-state **11**.
5. Release the held real fetch. The arriving B view **closes A's dialog**, focuses the withdrawal alert and locks commands. **Zero Execute requests** have been sent. Both P30-005 and P30-006 remain living; work revision remains **2**.
6. Perform the required further Re-read, open a fresh B confirmation and confirm. Exactly **one** Execute is sent, with **B's PlanId, minted key and original B fence**. Only P30-006 is felled; P30-005 remains living. Work becomes **3**, living count **98→97**, in Year **0**.

The reviewer wrapper only delays a real fetch and records page requests. It replaces no server payload and does not invoke the page's private confirmation handler. This is my own reproduction, not the author's Node witness. Screenshots and browser JSON record A, withdrawal and the successful B payload/event.

## D2 — resolved: unconfirmed outcome blocks commands until Re-read

Inspection confirms three gates: disabled command controls (including dynamically created Remove buttons), refusal in command handlers, and a final POST gate in `call()`, including map hit-test POSTs. Only a successful read that started after the latest lock can unlock commands; a read that itself withdraws a confirmation cannot clear that new lock.

I independently tested this at **360 CSS px** using real browser interactions:

- **Lost committed Add:** P30-007's Add reached the real engine and committed, then my fetch wrapper dropped its response. The server was at plan-state **13** with that pending ID; the page still showed **12**, with no optimistic order/Fell. All eight command IDs were disabled, and focus was in the alert.
- A **trusted pointer click on disabled Advance** sent no request. **40 trusted Tab presses** reached no engine command. Deliberately dispatching events to all eight controls, the Add form and the map also sent **zero requests**; the handler-level lock was effective independently of native disabled-button behavior.
- A deliberately failed Re-read kept the lock. A successful subsequent Re-read resolved the pending Add; the next ordinary Approve succeeded.
- **Lost committed Execute:** Execute for P30-007 then committed at work revision **4**, but its response was dropped. The page retained the approved crown/plan and claimed no Fell. All commands stayed disabled, and forced activation could not send a retry or another command.
- A successful Re-read showed the real stump and cleared approval, with **four total work events / work revision 4**, no duplicate Fell. The only page request during that last locked recovery was **GET api/view**.

A raw API client is outside the page lock, as it was in the original reproduction; the requirement being verified is the page's reachable/sendable command paths. The separate API client was used only to observe actual engine state or construct the specified race.

## D3 — resolved: first-use key policy and unchanged replay

The change at `AnnualContinuation.cs:414` compares a first execution's key with the one helper used to mint the view's key, before mutation. It reuses **WorkExecutionKeyConflict**. The replay lookup remains before current-state staleness validation, with the same session/PlanId/original-source comparison.

My separate standard-library Python API probe completed **18/18 assertions**:

- An arbitrary unminted key and an accurately predicted future key both return **HTTP 409 / WorkExecutionKeyConflict**. Both the returned and freshly read **complete views** equal the pre-call view.
- The legitimate current plan executes exactly P30-001 once.
- After Execute, Add and Approve, the later P30-002 plan is minted with **precisely the future key previously attempted**. It executes normally: no poisoning, no lost approval or blocked legitimate plan.
- Same key + same PlanId + original source + session returns the original receipt with **Replayed=true**, zero full-view mutation.
- Used key + wrong PlanId, or changed original source, returns **WorkExecutionKeyConflict**, zero full-view mutation. Wrong session returns **PlayerSessionMismatch**, zero mutation.
- For an unexecuted key, the wrong current PlanId still refuses **WorkPlanNotApproved**, zero mutation. This ordering was preserved.
- The probe ends with exactly **two events / two work revisions / no annual step**.

PS-20 independently passed in the full Core runner. No new refusal code or protected replay contract was introduced.

## D4 — targets corrected; one MINOR A-4 layout residual

**Residual D4-R (MINOR): wider native 200% remains two-column.** The approved `spec.md:344` says: “At 360 CSS px width and at 200 % zoom: single column, no horizontal scrolling of core actions, long hashes wrap or collapse, targets are at least 44 × 44 CSS px.”

The repair changes map scaling, but `app.css:119–122` still selects two columns at a viewport of at least 900 CSS px. In **Edge 157.0.4341.60**, a **1920×1200 outer window at actual native 200%** produced a **948×554 CSS px viewport**, DPR **2**, and computed main columns **535.5px 357px**. The screenshot visibly shows Map beside Inspector/Work Plan. This violates A-4's single-column clause. It is the same limitation the addendum discloses in §0.9; disclosure does not fulfil or waive the requirement.

**Required follow-up:** meet A-4's single-column condition at wider 200% windows as well, or obtain an explicit authoritative requirement disposition. This review grants no waiver and makes no candidate edit.

The target-size correction itself is independently supported:

| Actual condition | Map scale | Smallest disc, including four 0.35 m-radius stumps | Page horizontal overflow | Main layout |
|---|---:|---:|---:|---|
| Preview, 360×800 CSS px, DPR 1 | 64 px/m | **44.7998×44.7998 px** | **0** (345 = 345) | One column |
| Edge, native 200%, outer 1280×960; viewport 628×434 CSS px, DPR 2 | 64 px/m | **44.7999×44.7999 px** | **0** (620 = 620) | One column |
| Edge, native 200%, outer 1920×1200; viewport 948×554 CSS px, DPR 2 | 64 px/m | **44.7999×44.7999 px** | **0** (940 = 940) | **Two columns — FAIL** |
| Edge, 100%, emulated high-density 1280×800 CSS px, DPR 2 | 64 px/m | **44.7998×44.7998 px** | **0** (1265 = 1265) | Two columns; not the 200% case |
| Edge, native 100%, outer 1280×960; viewport 1256×868 CSS px, DPR 1 | Whole area, 17.94 px/m | 12.5592 px; visible under-44 note | **0** (1241 = 1241) | Two columns; outside A-4's named conditions |

All **100 drawn discs** were measured in each condition. At 360, the **96 remaining crowns** were at least **248.5347 px** across; all **four stumps** were 44.7998 px across. Measured visible ordinary controls were at least **44×44** in the 360 and native-200% checks. Values are CSS bounds, not device-pixel sizes. Long plan identities wrapped in the inspected native-zoom screenshot.

The native-200% rows are **actual browser zoom**, not CSS zoom or viewport/DPR imitation. In a private Edge profile I used the browser's `chrome.settingsPrivate.setDefaultZoom(2)`, verified `getDefaultZoom()==2`, and cleared CDP device metrics overrides. The viewport halves and DPR changes from 1 to 2. Only the explicitly labelled high-density-at-100% row used `Emulation.setDeviceMetricsOverride`. Preview's earlier Control-plus/Control-equals attempts had no effect and are not counted as native zoom.

**Exact identity remains exact.** Scene radii, the 0.35 m stump and Core hit-testing are unchanged. SVG circles use the server's radii, `pointer-events: fill` and the inverse screen matrix; the drawn-versus-server topmost cross-check remains. In the zoomed 360 page, a trusted click in P30-007's stump yielded candidates **[P30-007, P30-017]** and selected P30-007. A second trusted click **25 CSS px to its right**, outside that stump, yielded **[P30-017]** and selected P30-017. There is no enlarged invisible hit area selecting the stump. These are real fenced hit requests, not a nearest-centre substitute.

Enter activated the new Zoom in control (**64→96 px/m**) and Pan east (**scrollLeft 2120→2252**). Tab from the final pan button skipped the map/pane and reached the reason field; the pane has tabindex −1. Escape closed the two-ID Execute dialog and returned focus to Execute.

**Judgement on high-density desktops:** opening zoomed there is an honest, spec-permitted target-size solution. The spec does not require an initial whole-area view or forbid internal map panning; the controls, scale/target-size note, exact table selection and Whole area option disclose the behavior. DPR is a conservative heuristic, not proof that the user selected 200% zoom. It can legitimately choose the accessible map scale at native 100% on a high-density display. This does **not** resolve the separate page-column requirement.

## D5 — resolved in both cases

The real dialog displayed:

- **“Fells exactly 1 named tree in Year 0, or refuses all.”**
- **“Fells exactly 2 named trees in Year 0, or refuses all.”**

The singular/plural choice uses the immutable confirmation's order count and Core's two templates. PS-18 passed with the exact label set. The two-tree dialog named P30-008 and P30-009 with their distinct reasons; it was closed with Escape without executing that plan.

## Regression gates actually run

Working directory: `Tools/MultiRackSitkaP1Lab` in the fresh clone. SDK **8.0.134**, runtime **8.0.34**, Linux Node **v26.13.1** at the packet's explicit path, Python **3.14.4**. Windows Node was also **v26.13.1** for the reviewer-owned Edge driver.

```sh
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln -p:TreatWarningsAsErrors=true
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
/home/jer/.dotnet/dotnet restore src/Ui/SitkaP1.Ui.csproj --source "$E/empty-nuget"
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --no-restore -p:TreatWarningsAsErrors=true
NODE="$HOME/.local/share/vite-plus/bin/node" TMPDIR="$E/tmp" tests/ui-smoke.sh "$E/smoke"
python3 "$E/key-recheck.py"
```

`E` is the external evidence directory below. The real page was served by this clone's built `sitkap1-ui.dll fixtures/derived`. Browser actions used T3 Preview; native zoom used the separately written `edge-native-zoom.cjs`, not an author driver.

| Gate | Exact observed result |
|---|---|
| Solution build | **0 warnings, 0 errors** |
| Core tests, all | **92/92 passed, 0 failed = 66 baseline + 20 PS + 6 MS**, including new PS-20 |
| PS-01 concurrency detail | **16 rounds; 914 atomic reads; Execute won 1, Advance won 15** — both winners observed in this run |
| PS-14 continuation detail | **13 natural deaths** after Fell over ten years; the actual post-Fell biology comparison passed |
| UI offline restore / build | Restore succeeded from the empty local source; build **0 warnings, 0 errors** |
| Inspected committed ui-smoke.sh | **PASS: 132 passed, 0 failed**, including new J checks; this DOM stand-in is not counted as my browser evidence |
| Reviewer API key probe | **18/18 passed**, independent complete-view equality and poisoning/replay controls |
| Ten no-work digests | **10/10 character-identical**; extracted file SHA-256 **4e0e47a13e6b366b9eadf0d4c47c479da21fa31e8f64f6e07125826dae91ccee** |
| P0 / tracked source | Four-way pin verified, **52 file checks** and metadata unchanged; no P0 build; candidate source diff empty after runs |
| Scope / whitespace | Exact eight-path repair; protected-object audit passed; LF audit and `git diff --check` passed |

The smoke verified its ordinary loopback/envelope, full API workflow, no lab/P0/fixture writes, absent unchanged ~/.aspnet, and graceful **SIGINT exit 0**. The separate reviewer server was stopped with **SIGTERM**, returned **exit 0**, and printed “Session ended; nothing was saved.” The private Edge instance and the reviewer Preview tab were closed.

## Affected checklist re-dispositions only

These are independent review dispositions, not edits to the canonical checkboxes. “PASS” below is bounded to the delivered manual channel and the stated requirement. Unlisted prior dispositions stand.

| Item(s) | Focused re-disposition |
|---|---|
| PM-R08 | **PASS** — first-use identities enforced; independent unminted/future-key and later legitimate-plan controls |
| PM-R09; PM-X07 | **PASS reaffirmed for replay** — original tuple replays without mutation; wrong PlanId/source/session refuse as before |
| PM-R11 | **PASS**, D5 NIT removed; both actual dialog labels checked |
| PM-R10; PM-X12; PM-X13 | **PASS retained for exact geometry/selection** through the changed map presentation; radii unchanged, real stump/outside/crown identity check, MS tests |
| PM-R14 | **PARTIAL / A-4 single-column FAIL**; targets PASS in measured conditions; screen reader and physical touch **NOT TESTED** |
| PM-X15 | **D1/D2 recovery and confirmation negatives PASS**; **PARTIAL overall**, because wider native-200% A-4 layout still fails |
| R-12; X-12 | **PASS — named-tree Execute in the manual channel**; displayed A never executed or rebound, fresh displayed B alone executed |
| X-11 | **PASS — manual approval/confirmation**; no pre-Execute Fell and confirmation remains tied to the approved immutable intent |
| R-15; X-14 | **PASS — specified manual execution identity/replay**, including the repaired first-use policy; no comparator/prescription qualification implied |
| R-26 | **PASS — delivered manual refusals/recovery**; changed-intent withdrawal and uncertain-outcome state prevent commands until successful Re-read |
| R-22; X-21 | **PARTIAL / A-4 single-column FAIL**; map targets corrected, native zoom now actually run; reader/physical-touch/human acceptance remain untested |
| PM-X10 | **PASS reaffirmed** by PS-17/18, including the changed label set |
| PM-X14 | **PASS reaffirmed: 132/132** on this candidate |
| PM-X16 | **PASS — independent review performed and published**; verdict NEEDS CHANGES, not candidate approval |

The regression evidence above supports the standing preservation/concurrency dispositions; it does not reopen the remaining untouched checklist. AC-PM-13 is **not met**: the focused verdict is not APPROVED. No blanket Stage 5 PASS is issued. In particular, the prior R-06 / X-05 local-sample deferrals, G1-disabled item dispositions, PM-R15 / R-25 custody limits, X-06 limits and X-24 acceptance gate remain exactly as recorded previously.

## Evidence, limits and handback

Review-owned external evidence:

```text
/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Recheck-Evidence
C:\Users\hp\AppData\Local\Temp\ccf849-sol-repair-20261120
```

The WSL directory contains build/test logs, the independent Python probe and result, **25 browser observations** with recorded requests, five Preview screenshots, four Edge screenshots, the reviewer Edge driver/protocol and preservation audits. `browser-notes.txt` records reviewer setup corrections, including an observation taken before asynchronous Re-read completed and corrected inspector selectors; those are not counted as candidate failures or successful completed reads. The later Execute recovery explicitly waited for the completion announcement.

The evidence manifest has **46 entries**; SHA-256 of `SHA256SUMS` is **3f7dfaa3f971752eac0426d99df73d5a2c92236e8233fc7e758844ad2768988c**. Key evidence hashes:

| Evidence | SHA-256 |
|---|---|
| core-tests.txt | a276461ea42661dc91538c6a300a136da1f5eec3fbbde4063ebe37b61e6b4253 |
| smoke/ui-smoke.txt | a6392c0f39688a6afd3d295a63bca63adec83018a2e942cee68d4d1074222a3f |
| key-recheck.json | 0b912a6584cd34504e1b38086bcdf479586c63734469615a162518c158547ee6 |
| browser-observations.json | 4ee54983e511364ff060f9dab8271d71502faf699ba9e735a3bd90f28232a2ed |
| edge-zoom-observations.json | be7553d5d49304e7b6e9fa6645ec3df2580aa2238c135974da1753f5a7620f0d |

**NOT TESTED:** any real screen reader (including NVDA), physical touch, a human player's review/acceptance, and a full contrast/accessibility audit. The browser's zoom menu/toolbar was not manually operated; the actual native zoom state was tested through its browser setting as described. The DPR-2-at-100% condition was emulated, not measured on a physical high-density panel.

**Not rerun / unchanged limits:** exhaustive untouched Stage 5 permutations, the old external general fault probe, local sample/G1/#805 deferred workflows, historical physical/source custody and author-lane reconciliation. The full current Core runner, including PS-16 fault rollback, did run. Unity, Windows native game builds, persistence/restart, integration and post-integration gates were outside scope and **NOT RUN**.

Dashboard ingestion/audit/published-state synchronization and Drive mirror refresh are **NOT VERIFIED**. The single-review-file remit does not authorise coordination or generated-dashboard writes; operational synchronization remains outstanding under AgentWorkflow.

**Required next step:** a bounded author/authority disposition of residual **D4-R**, then a focused check of that layout only and any changed behavior. D1/D2/D3/D5 need no new repair on the evidence here. No Manager acceptance or integration claim is made.
