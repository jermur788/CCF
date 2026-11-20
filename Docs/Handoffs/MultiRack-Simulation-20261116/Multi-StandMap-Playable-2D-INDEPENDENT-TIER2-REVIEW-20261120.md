# CCF #849 — Independent Tier-2 review, PM-T09

**Verdict: NEEDS CHANGES.** Source review and the requested Tier-2 runs are complete on **1905433fbec69736c4c05ec629aa5941d11952ee**. The engine regression gates pass, but the browser can execute a plan different from the one named in its confirmation, the uncertain-outcome state still permits commands, and the fenced executor accepts keys the engine did not mint. Accessibility is only partially verified.

| Field | Record |
|---|---|
| Reviewer | GPT-6.1 Sol, independent nonauthor, PM-T09 |
| Date / host | 20 November 2026; DESKTOP-DL1BQI7, Windows / WSL2 Ubuntu, user jer |
| Candidate | [1905433fbec69736c4c05ec629aa5941d11952ee][candidate] |
| Candidate branch | feat/multi-standmap-playable2d-20261120-v1 |
| Review branch | review/multi-849-sol-tier2-20261120-v1, created from the candidate |
| Context commit | 62e39ec621e38a84030c08b35a171e297056c9ed |
| Spec | MULTI-STANDMAP-PLAYABLE-2D-V1; the four files at the candidate, with Manager Q-1…Q-6 |
| Spec conformance | FAIL / PARTIAL: D1–D4 below; reader and native zoom untested |
| Gate / scope | Independent G2 acceptance evidence for this isolated source candidate; no G4 integration decision |
| Review change | This Markdown only. No candidate fix, source mutation, checklist tick, merge or release |

The review is **not Manager acceptance, integrated qualification, user acceptance or permission to release**. PM-R15's retrospective physical/source custody evidence remains unverified.

## 1. Independence, authority and scope

I used a fresh independent sparse clone at:

~~~
/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Review
~~~

The checkout was clean at the exact candidate before review work and remained source-clean after the runs. Sparse checkout materialised Tools, Docs and specs, plus these exact tracked inputs required by the existing build/provenance tests: SitkaGrowthModel.cs, SimulationRandom.cs, Species/SitkaSpruce.asset and TreeSpeciesDefinition.cs under Assets/ForestPrototype. None was edited.

I read the project authority/context, the candidate spec/plan/tasks/checklist, the author handoff from Git commit **4e95e787**, and the separately retrieved [Manager final readback][manager]. The final readback is commit **461188c26e7a3bb503f2558c2d3f18adb7d93667**: approved design and conditional source admission, not source acceptance. The live GitHub coordination board was fetched at task start and refreshed before this report; final blob **10e6077245f32730b7de7e5e7eb618d40085da42** still requires native lease checks and names a different registered author checkout from the WSL lane disclosed in the handoff. I do not infer a missing registration from the candidate's older local board.

The frozen P0 reference was copied only from the explicitly authorised .p0ref directory in CCF-Multi-A1-WorkRevision-Fence. DLL hash, csproj pin, .sha256 and provenance.txt all equal:

~~~
9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
~~~

**build-p0-core-ref.sh was never run.** I did not use the author source lane, author evidence directory or author browser drivers. No source edits were made in the author, A-1, docs, T3 Windows or main lanes.

Source review covered the exact added/changed lines through **30fa269c** (including its implementation parent **24d9ef11**) and **30fa269c..1905433f** for UI. The diff from approved spec **fed675f5** has exactly the **15 allowed paths**. I inspected all 15, including all root publications in AnnualContinuation, new facade/map types, PS/MS tests, HTTP guards, page/CSS/JavaScript, smoke and README.

A blob/tree audit against A-1 **4bb809732fb691f15630e1fbc5ea20dca6dbbe2f** found **19 protected paths/trees unchanged**: Assets, ProjectSettings, P0, fixtures, AnnualRules, WorkContract, AnnualEngine, existing Core/test projects, solution, CLI, build files and existing baseline test files. P0 tree is **f212c8f64fb28b83aa870c7b5b7c72e03051e46a**. Tracked lab source is LF; git diff --check passed. Builds created only their normal ignored outputs.

## 2. Defects, ordered by severity

No CRITICAL defect was established. There are **3 MAJOR, 1 MINOR and 1 NIT** findings.

### D1 — MAJOR: confirmation can execute an undisplayed replacement plan

**Location:** [app.js:757][js757], [app.js:800][js800], [app.js:854][js854]; payload replacement at [app.js:135][js135].

The dialog renders a plan's IDs/reasons when opened, but Confirm obtains its PlanId, key and fence from the latest mutable page view. An outstanding Re-read can replace that view without closing or updating the dialog. Core correctly validates the newly substituted plan; the user has confirmed different text.

**Reproduction actually run in headless Edge, with trusted pointer clicks:**

1. Approve plan A containing P30-005 and render it in the page.
2. Start a Re-read and delay its fetch. While that read is pending, click Execute. The dialog lists only P30-005.
3. A second client of the **same capability/session** cancels A, adds P30-006, and approves plan B.
4. Let the pending read complete. The page's approved list now contains P30-006, while the still-open dialog lists P30-005.
5. Click the dialog's Execute button.

**Observed:** the successful event contains **only P30-006**. P30-005 is still living, P30-006 is felled, the work revision goes 1 → 2 and live count 99 → 98 in Year 2, with no refusal. Plan-state moved from 14 for displayed A to 17 for B.

This is a same-session asynchronous UI race; the test does not require a second ecological authority or production-source modification. The reviewer wrapper delays the real fetch; it does not replace server data or call the page's confirmation handler. Other-client commands use the existing guarded API.

**Required result:** the confirmation must remain bound to the exact displayed plan and fence, or become invalid when that intent changes. Confirm must never substitute a different current plan. This blocks end-to-end approval of the explicit named-tree Execute workflow (§7.1, R-12 / X-12).

**Evidence:** browser/confirmation-race.cjs, confirmation-race.json and R01–R03 screenshots. R02 visibly shows A's dialog over B's approved plan; R03 and JSON establish which tree was actually felled.

### D2 — MAJOR: “outcome unconfirmed” still permits commands before Re-read

**Location:** [app.js:147][js147] and [app.js:243][js243].

showUnconfirmed changes only the alert. run clears busy before displaying it, and no unresolved-outcome gate disables the remaining page actions. Removing the alert-local retry button did not make the **page state** offer only Re-read.

**Reproduction actually run in Edge:**

1. Select P30-003, type a reason, and activate Add.
2. Allow the real server to commit the Add, then drop its response in the reviewer fetch wrapper.
3. Inspect the alert and controls without allowing the page to re-read.
4. Click the still-enabled Advance button.

**Observed:** actual server plan-state is **11**, pending contains P30-003; displayed plan-state remains **10**, and the page does not optimistically draw the pending order. The alert has only a Re-read button, but **Add and Advance are enabled elsewhere**. Advance sends POST api/year/advance **before any GET api/view**, returning 409 PlayerPlanStateStale.

Core prevented an unwanted mutation in this reproduction, and the no-optimistic-render requirement passed. The explicit recovery requirement did not: a new mutating request was available and submitted while the prior result was unresolved. This contradicts PM-X15, the source comment and the handoff's “only action is Re-read” claim.

**Required result:** until a successful current-state read resolves uncertainty, Re-read must be the only actionable recovery path for engine commands.

**Evidence:** browser/browser-log.json observation checks 14–16; B09 screenshot; browser-review.cjs. The screenshot alone proves only the alert; the enabled-control inspection and captured POST prove the whole-page failure.

### D3 — MAJOR: an unminted execution key can poison a later legitimate plan

**Location:** [AnnualContinuation.cs:390][core390], specifically the pass-through at [AnnualContinuation.cs:411][core411].

The fenced Execute requires a nonblank key and validates prior-key replay, source, session, plan-state and approved PlanId. It never checks that a new key is the approved plan's engine-minted key.

**Independent API reproduction actually run:**

1. Approve a plan at plan-state n. Its shown key is session:plan:n:execute.
2. Execute that current plan with **session:plan:(n+3):execute** instead.
3. Add another tree, then approve it. Execute, Add and Approve move plan-state by three, so this new plan's legitimate minted key is the key consumed in step 2.
4. Execute the new plan with its own displayed minted identity.

**Observed:** step 2 returns 200 and performs a real Fell using the unminted future key. In this run, the current minted key ended in **plan:109:execute**, but the accepted key ended in **plan:112:execute**. The next plan legitimately mints plan:112:execute and its normal Execute returns **409 WorkExecutionKeyConflict**, with full view unchanged. The new approval remains pending execution.

This does not establish duplicate Fell. It establishes acceptance of caller-selected execution identity and obstruction of a later engine-minted identity.

**Independent judgement of the disclosed §6.5 ambiguity:** its ordered algorithm indeed omits an explicit “key equals minted key” step. However, C-5, §6.4, PM-R08 and the public facade contract require engine-minted PlanId/ExecutionKey, and describe callers echoing those identities. An omission in the detailed algorithm does not establish permission to reserve a future plan's key. The concrete poisoning result makes this more than a stylistic reading dispute.

**Required result:** reject an unminted key on the fenced first-execution path without changing state; retain the specified original-source/PlanId/session replay behavior. If the Manager intended caller-chosen keys, that requires an explicit contract disposition addressing future minted-key collision; this report grants no such waiver.

**Evidence:** negative-api.py, negative-api.txt and negative-api.json. The ordinary replay and wrong-PlanId refusal controls passed.

### D4 — MINOR: map targets are below the approved 44 CSS-pixel minimum

**Location:** [app.js:324][js324], map sizing at [app.css:130][css130]; requirement [spec.md:344][spec344].

At **360 CSS px**, measured initial crown target diameters were approximately **30.04–55.80 px**; a later stump measured **5.42 px**. Some crowns and the stump are below 44 × 44. True crown size and topmost occlusion also make some exposed clickable regions smaller than a circle's bounding box. This reproduces the substance of the author's 24–48 px disclosure despite different viewport metrics.

The exact-ID table offers usable ≥44 px Select buttons, and the emulated touch hit-test worked. That is a useful mitigation, but A-4 does not state an exception for map targets. PM-R14 / Stage 5 accessibility cannot receive an unqualified PASS. Meeting A-4 or an explicit Manager disposition remains required; I do not propose changing ecological radii as an implicit fix.

**Reproduction:** render the real page at 360 × 800 CSS px; measure the drawn tree circles' screen bounds; after a Fell include its stump. See browser-log.json and B02/B10.

### D5 — NIT: singular Execute consequence has incorrect grammar

**Location:** [PlayerSessionView.cs:74][labels74].

Approve one tree and open Execute. The dialog reads **“Fells exactly 1 named trees in Year 2, or refuses all.”** The consequence is understandable, but singular wording should be correct.

The label is outside the UI author's eight-path stage, but it is inside the full candidate's Core scope and this review. See R01/R02 and B06.

## 3. What I ran and what it proves

All commands below ran in the independent clone, using /home/jer/.dotnet/dotnet, SDK **8.0.134**, ASP.NET Core **8.0.34**. Browser automation used Windows Node **26.13.1** and Microsoft Edge **157.0.4341.60**, without installing packages.

| Evidence ID | Actual gate / observation | Result |
|---|---|---|
| E1 | Solution build with TreatWarningsAsErrors | **0 warnings, 0 errors** |
| E2 | Existing compiled Core runner | **91/91 passed, 0 failed**: 66 baseline + 19 PS + 6 MS |
| E3 | UI offline restore from an empty local source, then UI build --no-restore with TreatWarningsAsErrors | Restore succeeded; **0 warnings, 0 errors** |
| E4 | Inspected and ran committed tests/ui-smoke.sh | **98 passed, 0 failed** |
| E5 | Reviewer API negative/atomic probe | **18 assertions/checks completed**, including a check deliberately confirming D3's poisoned-key refusal; **two key-policy defect observations**. This is not 18 conformance PASSes |
| E6 | Reviewer compiled fault probe against this clone's DLL | **11/11 assertions passed**; probe build 0 warnings/errors |
| E7 | Real Edge main browser driver | **17/17 observation assertions completed**, including deliberate defect witnesses; no uncaught page errors. This is not a 17-case UX conformance PASS |
| E8 | Separate real Edge confirmation-race driver | D1 reproduced; P30-005 remained live and P30-006 was felled |
| E9 | Exact diff / protected-blob / LF audit | 15 allowed source paths; 19 protected paths/trees unchanged; no tracked source edit |
| E10 | Manager readback / live board / author handoff audit | Documentary evidence only; no retrospective physical-custody PASS |

Primary invocations, from Tools/MultiRackSitkaP1Lab (disposable directory paths abbreviated in angle brackets):

~~~sh
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln -p:TreatWarningsAsErrors=true
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
/home/jer/.dotnet/dotnet restore src/Ui/SitkaP1.Ui.csproj --source <empty-local-directory>
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --no-restore -p:TreatWarningsAsErrors=true
tests/ui-smoke.sh <review-evidence-directory>/smoke
~~~

The first Core attempt was **90/91**, because the initially requested sparse checkout omitted tracked Assets used by an existing source-provenance test. I preserved that log, materialised the exact missing repository files, and reran to **91/91**. This was a review setup correction, not a candidate fix or a waived failure.

PS-01 recorded **16 rounds, 580 atomic reads, Execute won 0 / Advance won 16**. Thus both race-winner outcomes were not observed in the final successful run. Inspection confirms that the same test additionally forces a real Fell and a no-work year every round while readers run; the successful result is not merely 16 cancellations. PS-14 observed **13 natural deaths** after Fell over ten years and checked against AnnualRules.Calculate on the real post-Fell state.

All ten no-work digest lines match the unchanged committed literals. Their extracted file SHA-256 is **4e0e47a13e6b366b9eadf0d4c47c479da21fa31e8f64f6e07125826dae91ccee**.

### Independent negative and fault controls

E5 compares the **complete current view before and after**, not just counts:

- pre-Fell fences after a real Fell refused Add, Remove, Approve, Cancel and Advance; the exact original Execute replay was separately a zero-mutation replay;
- pre-Year+1 fences refused Add, Remove, Approve, Cancel, Execute and Advance;
- same execution key under a different PlanId refused WorkExecutionKeyConflict;
- no-consent Advance refused; consent cancelled approved work at zero additional Fell and advanced one genuine year; the cancelled plan could not execute;
- an all-live approved batch refused WorkWouldLeaveNoLivingStems, with the entire view unchanged and approval retained.

E6 is reviewer-authored and outside the repository. Reflection accesses only the existing internal test seams. It injects a reached exception before publication in a **two-ID Execute**, verifies the **identical root reference and full serialized view**, then retries and observes two real Fells with exactly one revision/event. Replay creates no second Fell. A separate reached fault during consented Advance leaves the same root and full view; retry cancels at zero additional Fell and commits exactly one year. **Both callbacks were reached.** These retry controls demonstrate that rollback assertions are not passing because the work never reached execution. No production-source mutation or weakened oracle was used.

E4 independently verifies loopback-only sockets, capability path, exact Host, Origin, method, strict JSON, content type, body size, hostile URL environment, no CORS, byte-identical served assets, workflow/Remove/Cancel, no lab/fixture/P0 writes, absent unchanged ~/.aspnet and graceful SIGINT **exit 0**. The later review-owned browser server was stopped and its port confirmed closed.

Compiled reflection also observed exactly the nine expected adapter methods (Open, Read, AddPending, RemovePending, Approve, CancelApproved, Execute, Advance, HitTest), with no public callback, caller-built plan, comparator or sample member.

## 4. Real browser observations and limits

The real candidate page was served from the review clone's built UI. The main Edge run was **06:15:26–06:16:14 UTC**; the separate race run began **06:18:24 UTC**. I inspected the captured desktop/mobile/scale/recovery/race screenshots as well as DOM/API evidence.

| Criterion | What I actually observed | Disposition |
|---|---|---|
| 1280 × 800 CSS px | 100 table rows, two columns, no page horizontal overflow; body text 16 px, clear source/honesty labels | RUN; usable in inspected view |
| 360 × 800 CSS px | One column, no page horizontal overflow; standard visible controls ≥44 px; searchable exact-ID selection and inspector usable | RUN; map target failure D4 |
| 200% scale | 640 × 400 CSS px at DPR 2 for a 1280 × 800 physical equivalent; one column, wrapped facts, readable controls and keyboard workflow | RUN **as layout emulation only** |
| Browser's native 200% zoom control | Not operated; viewport/DPR emulation is not claimed as a native zoom test | **NOT TESTED** |
| Keyboard only | Skip link, table search/filter/Select, inspector, blank-reason refusal/focus, Add/Remove/re-add, Approve/Cancel/reapprove, separate Execute stop, Back initial focus, Escape, confirm/receipt, Year+1, consent Keep and consent Cancel+Advance | RUN with trusted key events; normal path worked |
| Touch | Trusted emulated touch selected exact topmost P30-001, with P30-011 as overlap alternative; real fenced hit endpoint returned 200 | RUN in emulation; physical touch **NOT TESTED** |
| Bare ground | Real pointer click cleared selection rather than choosing the nearest tree | RUN |
| Readability | 13 sampled computed text styles: minimum contrast **9.3996:1**; status names/patterns/text readable. Map grid/axis labels become very small at 360 px | Sample only; no full accessibility/contrast audit |
| Refusal / uncertainty | Blank reason showed WorkReasonRequired and focused reason. Lost committed Add response showed uncertainty without optimistic pending/Fell rendering | RUN; recovery restriction failed D2 |
| Confirmation stability | Pending read plus same-session plan replacement | RUN; different tree executed, D1 |
| Screen reader | No NVDA or other reader was run | **NOT TESTED**; semantic structure alone is not reader PASS |
| Human player review | All input was agent-driven | **NOT RUN by a human player** |

A preliminary T3 Preview session successfully rendered and selected at desktop/mobile widths, but later keyboard commands did not move focus and zoom shortcuts had no effect. I did not count those as successful keyboard or zoom evidence. The independent Edge driver then ran the actual key/touch workflow. Its first attempt needed a corrected Enter text-event sequence; that reviewer-driver-only correction and initial log were retained. Neither correction touched the candidate.

Screenshots B01–B10 and R01–R03 are external evidence. B04 is expressly labelled emulated scale. Static screenshots do not independently prove input, source changes or disabled controls; the companion JSON and request/state checks supply those observations.

## 5. Independent decisions on disclosed deviations

| Disclosure | Reviewer judgement |
|---|---|
| Empty ASP.NET builder instead of spec's slim builder | **Acceptable bounded implementation deviation for this review.** Program.cs explicitly configures Kestrel/loopback and avoids the default configuration sources. E4's hostile-environment/socket/file checks support the Q-1 envelope. No defect was found in this choice. This is not a Manager spec amendment or a claim that the server is absent |
| Sub-44 px map crowns at 360 px, with larger table Select buttons | **Unmet literal A-4; D4.** Equivalent table selection mitigates practical impact but does not itself waive the target requirement |
| “1 named trees” in Core | **NIT D5**, within the whole candidate review even though outside the UI stage's eight paths |
| Executor does not refuse an unminted key; §6.5 is silent | **MAJOR D3.** Considered the silence explicitly; broader minting contract and concrete future-key poisoning preclude an unqualified PM-R08 PASS |
| Remove / Cancel added late | **Independently exercised now.** E4 smoke and real keyboard E7 covered Remove, re-add, Approve, Cancel, absence of returned pending orders, re-add and fresh approval, all before any Fell |
| Removed Execute retry for uncertain outcomes | Alert-local change is present, but the whole-page claim fails **D2** |
| UI-only state | Latest server view, selection/overlap and receipt-display bookkeeping were inspected; I found no independent ecological simulation, client deletion-as-Fell or web-storage authority. D1 is a failure to preserve confirmed intent, despite using real server values |

## 6. Audit of all 31 delta items

These are **review dispositions**, not edits to the authoritative checklist. PASS means the named bounded requirement was supported by inspected code and/or my own run; it never promotes all Stage 5 acceptance. PARTIAL, FAIL, DEFERRED and NOT VERIFIED must not be converted to PASS by grouping. E1–E10 refer to §3.

| Item | Independent disposition | Evidence / limit |
|---|---|---|
| PM-R01 | PASS — UI-A boundary | One in-process engine/session, explicit HTTP presentation listener; E3/E4; builder disposition above |
| PM-R02 | DOCUMENTARY / PARTIAL | Exact approved spec and Manager readback precede first source commit in Git history; actual first local write timing is not independently proved. Not a physical-admission PASS |
| PM-R03 | PASS | New local map; no Scenario One compile/copy/link or StandMapSource use; E9 and project/source inspection |
| PM-R04 | PASS | Server PlayerSessionView drives actual values; no independent biology/clock/RNG/storage authority in page or adapter; E7 and source review |
| PM-R05 | PASS | ReadPlayerView and command success views use one sync/root; E2 PS-01/17, E6 |
| PM-R06 | PASS — engine fence checks | Session/full source/plan-state before mutation and hit-test; E2 PS-04/10/11, MS-06; E5. This does not cure D1's substitution of a different valid fence |
| PM-R07 | PASS | All root publications inspected; PS-02/03 plus zero-delta faults/replays, E5/E6 |
| PM-R08 | **FAIL — D3** | PlanId is engine-minted and plan objects excluded; first-execution key is not enforced as engine-minted |
| PM-R09 | PASS — specified replay tuple | Original key/PlanId/source/session replay; mismatch refusals; E2 PS-07/08/09 and E5. D3 concerns first-use identity |
| PM-R10 | PASS — identity mapping | MS-01…06, E4 overlap oracle, E7 touch/topmost/table/bare-ground; no nearest-centre identity |
| PM-R11 | PASS with NIT D5 | Core labels, actual fixture/year/revisions, G1/money/session disclosures inspected and rendered |
| PM-R12 | PASS within Q boundaries | No forbidden feature/control path; only permitted negative disclosures and Q-6 unavailable values; source scan and E7 |
| PM-R13 | PASS | Exact 15-path diff; protected blobs/trees unchanged, E9 |
| PM-R14 | **PARTIAL / A-4 FAIL** | E7 keyboard/emulated touch/scale; D4; reader and native zoom NOT TESTED |
| PM-R15 | **NOT VERIFIED** | P0 handling verified; pre-first-write custody, #846 native release and lane discrepancy not retrospectively established, E10 |
| PM-X01 | PASS | Exact candidate, independent LF clone, 4-way copied P0, unchanged baseline test blobs and ten digest literals; E1/E2/E9 |
| PM-X02 | PASS with race limit | PS-01: 16 rounds/580 reads, no torn view; only Advance race wins observed; forced real Fell cycles also read, E2 |
| PM-X03 | PASS | PS-02/03 exact revisions and ABA refusal; E2 |
| PM-X04 | PASS | PS-04/10/11, MS-06; E5 stale-source full-view equality after Fell/Year+1 |
| PM-X05 | PASS | PS-05, E4 and keyboard Remove/Cancel: zero pre-Execute Fell/token change |
| PM-X06 | PASS — engine result | PS-06 and E4/E6 real exact-ID typed Fell, work +1, no annual step, authoritative before/after |
| PM-X07 | PASS | PS-07/08/09 and E5: PlanId/source/session mismatches and no second Fell |
| PM-X08 | PASS | PS-12/16 plus E5 all-live full equality and E6 reached faults/retry controls |
| PM-X09 | PASS | PS-13/14/15, E5/E6 consent, genuine year and second cycle; post-Fell AnnualRules comparison |
| PM-X10 | PASS | PS-17/18 read-only copies and 100 exact IDs/one scope; E2 |
| PM-X11 | PASS | PS-19 and independent compiled adapter reflection; no forbidden public seam or plan-object API |
| PM-X12 | PASS | MS-01/02/05, scene code, real rendered crown/ring/stump states |
| PM-X13 | PASS | MS-03/04/06; E4 hit oracle; E7 real topmost cross-check and bare-ground click |
| PM-X14 | PASS | E4 **98/98** process/envelope checks on this candidate |
| PM-X15 | **FAIL — D1/D2; partial scale evidence** | Normal keyboard/360/1280 path ran, but uncertain state permits commands and confirmation can rebind; native zoom untested |
| PM-X16 | PASS — review performed | This independent exact-SHA review and durable report; verdict is NEEDS CHANGES, not candidate approval |

The author correctly left PM-R14 partial, PM-R15 documentary/unverified and PM-X16 pending in the author handoff. The grouped PM-R05…09 claim needs the PM-R08 exception above. PM-X15's unqualified “only a re-read” assertion is contradicted by E7; it must not be promoted to PASS.

## 7. Audit of all 50 Stage 5 items

Authority remains the [26 R + 24 X checklist][stage5]. Q-3 expressly prevents R-06 or X-05 PASS from this slice. The four G1-disabled items below are **not PASS**; the disabled technical condition and manual fallback are positively evidenced, while formal case disposition remains the Manager's.

| Item | Independent disposition | Evidence / boundary |
|---|---|---|
| R-01 | PARTIAL — one-area slice | Exact undivided 30 m / 100-tree area and no rack workflow verified; separate near-term multi-area obligation not delivered |
| R-02 | PASS | Exact synthetic domain/fixture and disclosures, no Unity/Reference parity claim; E2/E7/E9 |
| R-03 | PASS — admitted source | Strict catalog/fixture/ID/f32 tests, committed frame/token fields and exact-source fences; E2/E4/E9 |
| R-04 | PASS | No diagnostic source-tab/edit/rebase route; facade opens committed continuation only; E2 and code/API inspection |
| R-05 | PARTIAL | Exact manual IDs and per-tree written reasons work, including multi-ID E6; local representative sample deferred by Q-3 |
| R-06 | **NOT MET / DEFERRED** | No local sample implementation/evidence; not PASS from this slice |
| R-07 | **N/A — G1 DISABLED** | No generation API; visible Manual/NotCovered and successful manual path; E2 PS-19/E7; Manager item disposition still needed |
| R-08 | **N/A — G1 DISABLED** | No suggestions to qualify; positive disabled/manual evidence as R-07; not proposal traceability PASS |
| R-09 | PASS — manual channel | Individual selection/reason/remove/cancel; no automatic quality/bulk confirmation path; E4/E7 |
| R-10 | PASS — UI boundary | No candidate/proposal/sample import path; selection/Add/Approve do not Fell; E2/E4/E7. Does not qualify #805 Undo itself |
| R-11 | PASS — manual channel | Ordinary source-bound pending orders and no pre-Execute biology; E2/E4/E7; proposed prescription route disabled |
| R-12 | **FAIL — D1** | Separate approval/Execute normal flow works, but a stale confirmation can execute a different batch |
| R-13 | PASS — engine revalidation | Full fenced Add/Approve/Execute and exact live-ID checks; E2/E5. UI display-intent failure remains D1 |
| R-14 | PASS — engine writer | Typed actual Fell, source roster/count/BA/event from one engine; E2/E4/E6 |
| R-15 | PARTIAL | Exactly-once and conflicting replay controls pass; unminted/future-key identity defect D3 prevents full identity qualification |
| R-16 | PASS | Single root publication inspected; reached Execute/Advance faults leave identical root/full view; E2/E6 |
| R-17 | PASS — specified manual flow | Approved edits refuse until explicit Cancel/re-add/reapprove; consent Keep/cancel-and-advance at zero Fell verified; E2/E4/E5/E7 |
| R-18 | PASS | No-work creates no work; survivors feed AnnualRules for genuine next year; E2 PS-13/14, E5/E6 |
| R-19 | PASS within Q-6 | Read-only work/death/year records and repeat cycles; PS-15/17 and E4/E7. No proposed/sample ledger or durable history claimed |
| R-20 | PASS | Fell vs natural causes, both sides of 170 mm, all-live refusal/last-live/annual-zero-live and unchanged no-work oracle; E2/E5 |
| R-21 | PASS | No new economics; unavailable money labels and no monetary calculations/settlement; E2/E4/E7/source review |
| R-22 | **PARTIAL / target FAIL** | E7 keyboard/emulated touch/layout; D4; screen reader and native zoom NOT TESTED |
| R-23 | PASS within Q-1 | Session-only process; no persistence/cross-process claim/storage authority; HTTP server disclosed, E4/source review |
| R-24 | PASS — source preservation | E9 protected-tree comparison, no #805/fixture/Unity/main source change. Physical authority is separately unverified |
| R-25 | **NOT VERIFIED / NOT SATISFIED for acceptance** | Design readback exists; pre-write custody, native release, final Manager gate, integration and multi-area obligation not established |
| R-26 | PARTIAL | Normal refusals are exact, visible and zero mutation, E2/E4/E5/E7; D1 lacks a refusal for changed displayed intent, D2 violates recovery restriction |

| Item | Independent disposition | Evidence / boundary |
|---|---|---|
| X-01 | PARTIAL — broad source negatives | 100 exact IDs, fixture/digests and protected HTML/source verified; E2 runs tamper/wrong-ID/strict-count negatives. I did not separately construct every enumerated 99/101 and mixed-fixture variant |
| X-02 | PASS — exposed current-source path | Genesis/committed annual frames only; stale revisions/session/source and every source-token field perturbation refused in E2/E5; no Before/PostGrowth/aborted report import endpoint exists |
| X-03 | PASS — one-area UI | Actual one-area rendering and per-ID selection at 1280/360; no rack/entrance/clearing/all-tree shortcut; E7/E9 |
| X-04 | PASS — manual ID legality | E2 exact/wrong-case/missing/duplicate/dead-ID negatives and multi-ID orders; E6 exact two-ID Fell; E7 selection never implicitly adds stand |
| X-05 | **NOT PASS / DEFERRED** | Manual fallback and disabled label verified; no local representative sample, Q-3 |
| X-06 | **NOT VERIFIED in full** | No #805 candidate route and no #805 edits in this slice; zero pre-Execute biology verified. Actual #805 local Undo/Discard and stale-candidate import workflow not run |
| X-07 | **N/A — G1 DISABLED** | Positive manual-only/disabled evidence; no comparator predicate/proposal claimed; Manager disposition remains |
| X-08 | PASS — manual pending | Reason/ID/fence/duplicate checks and zero Fell before Execute; E2/E4/E5/E7 |
| X-09 | **N/A — G1 DISABLED** | No prescription materialisation API; no unsupported pending route; Manager disposition remains |
| X-10 | PASS — delivered manual channel | One pending/approved batch, duplicate/conflict refusal, no hidden second channel; E2/E4 and source inspection; optional proposals disabled |
| X-11 | PARTIAL | Explicit immutable approval at zero Fell works, E2/E7; D1 lets the later execution confirmation represent a different approval |
| X-12 | **FAIL end-to-end — D1** | Engine's genuine exact-ID execution/refusal controls pass, but browser confirmation can execute the unshown replacement plan |
| X-13 | PASS — commit-boundary fault | E2 PS-16 plus E6 reached two-ID Execute fault and retry controls: identical root/full view, no partial event/revision, then actual successful work |
| X-14 | PARTIAL | Identical original replay and conflicting PlanId/source/session tests pass, E2/E5/E6; first-use key poisoning D3 prevents full execution-identity qualification |
| X-15 | PASS — normal edit/consent contract | Explicit Remove/Cancel/reapproval and Keep/cancel-and-advance exercised; cancelled Execute/no-consent refusals zero mutation; E2/E4/E5/E7. D1 separately blocks confirmation stability |
| X-16 | PASS | Actual post-Fell survivors independently compared with AnnualRules.Calculate, not report relabel; E2 PS-14/E5/E6 |
| X-17 | PASS | Unchanged baseline oracle and all ten digests; no-work year invariants; pending consent/cancel tests, E2/E5 |
| X-18 | PASS within Q-6 | Fresh cycles, earlier views immutable, history retained and replay no-op; E2 PS-15/17, E4/E7; no restart guarantee |
| X-19 | PASS | Typed Fell/natural cause and BA/count, all-live whole refusal, last-live and annual zero-live controls; E2/E5 |
| X-20 | PASS | Unavailable amounts, no prices/affordability/charge/replay settlement; E4/E7 and inspected code |
| X-21 | **PARTIAL / target FAIL** | Real keyboard and emulated touch/layout evidenced; D4; sample/proposal not delivered, reader/native zoom NOT TESTED |
| X-22 | PASS — bounded source preservation | E9 exact lineage/protected blobs plus E2's executed altered-byte/strict-hash/source-pin negatives. No original fixture/golden weakened or rebaselined |
| X-23 | PASS within Q-1/Q-6 | No persistence/rack/economy/Unity/Reference changes; process writes none in E4; session-only disclosures and no hidden save authority |
| X-24 | **NOT SATISFIED** | This independent review exists but finds defects; custody/release evidence and integrated/Manager acceptance absent. No release acceptance |

The handoff's careful sample/G1/accessibility/custody limitations are retained. Its grouped “AUTHOR-EVIDENCED” wording is not automatically an official PASS. In particular, X-06 only has **absence-of-route** evidence here, X-01 has substantial but not every enumerated independent variant, and R-12/X-12 plus execution-identity/recovery items need the exceptions recorded above.

## 8. Evidence custody and reproducibility

External evidence is review-owned, not author evidence:

~~~
/home/jer/.ccf-orch/standmap-p1-sol-tier2/
C:\Users\hp\AppData\Local\Temp\ccf849-sol-tier2
~~~

The persistent WSL directory holds primary build/test logs, all probes/drivers, scope audit, downloaded documentary evidence, browser JSON and 13 PNGs. SHA256SUMS contains **54 evidence entries** (probe bin/obj excluded); its hash is:

~~~
ac983dff012e4ccb543815b8e8adbeeb6691e8af859c2a05f9a9d72f9fdaf5ab
~~~

A frozen external archive contains that manifest and its entries:

~~~
/home/jer/.ccf-orch/standmap-p1-sol-tier2-evidence-1905433f.tar.gz
size: 2312881 bytes
SHA-256: 61f4cdebff5fd9d2a3e86fe9dacdea82fcaae4efae167f7f29f03edff74e6d55
~~~

Key evidence hashes:

| Relative evidence file | SHA-256 |
|---|---|
| solution-build.txt | 491a24f80f9b1f2e9f6f7b75481be350ce52ea37f37e73ef48be5358af54c35f |
| core-tests.txt | f79c31cd976adb23af0b2cb9aa0db54d3af3e0625579bfa5bd432dab6e0fef54 |
| ui-build.txt | 00a9817dd8495fa0bff6586476554dccbf3247e77d63e10e14776524daf91b66 |
| smoke/ui-smoke.txt | 9197acf4f93391af152f61d0866d3233b73c904a29318c0fa72cbbcf4f6119c9 |
| negative-api.py | 1428fcba0c7e93dbc6ec90ceed9bed2e7788703f782f1afce1dcf92891df432e |
| negative-api.json | c90987e2a1b9c29a41717db078a1e44b541cb6715aedb57d31a32e2b89757f33 |
| fault-probe/Program.cs | da81a90c8d61de7a5cce5be2b0e053c0c10115666e438d0b3f94830b3680a20b |
| fault-probe.txt | deaddf7be7af541dd2e4a87d083c7267504a0f6b142a7c13a47b92228e9a1107 |
| browser/browser-review.cjs | b1e012691eeddae5e4049a1e852949ccb371cd84e9494008bb0db245607192f9 |
| browser/browser-log.json | 9d5eab56aedd7caa7d1f32838608a7323317ecafd9a28df5db8f5aa2c9aadcbd |
| browser/confirmation-race.cjs | bc35db18414b3fdde424ebe2d9e504a4439d9c7b76044d4a98d85c76465528b1 |
| browser/confirmation-race.json | 568807432344a2baa50f8a1cec41a0f1619387a7d16800ce08c917ecbd26e77d |

Built review DLL SHA-256 values:

- Core: ec4f75b20fdf22735ddcf241173583191e681ff598c2cbfdbb219757f99d0df6
- UI: af69c8bd9b0d63f69deca756ab91e9faff4c83db2563955cf00c2ab30ca7dee0

These are this lane's Debug outputs; differing author Debug hashes alone do not prove a source defect.

To repeat reviewer-only probes after building and verifying the pin, run negative-api.py with Python, and build/run the external fault-probe/Probe.csproj with the recorded SDK. For browser drivers, start this candidate UI DLL with fixtures/derived, set REVIEW_URL to its freshly printed capability URL, and invoke the corresponding external .cjs with Node on Windows. These drivers deliberately exercise disposable in-memory sessions. The detailed reproductions above are sufficient to inspect each defect without adopting an author's harness.

## 9. Remaining gates and return

The review has made **no candidate fix**. Required follow-up is an author revision addressing D1–D3, a disposition of D4, and fresh bounded verification on the revised exact SHA. Retest the race, committed-but-response-lost recovery, first-use key rejection plus normal replay, and the changed browser controls. Preserve the frozen source/oracle boundaries.

Still **NOT TESTED / NOT VERIFIED**:

- native browser 200% zoom, any actual screen reader, physical touch and a human player's readability/acceptance session;
- a complete contrast/accessibility audit beyond the sampled checks;
- every original Stage 5 fixture-negative permutation, the deferred local sample, optional comparator/proposal/prescription channels and actual #805 Undo/Discard workflow;
- both scheduler winner outcomes in the final PS-01 race run;
- actual pre-first-write native custody/#846 release, the disclosed author checkout discrepancy and original on-machine timing; a GitHub row or commit order cannot retrospectively prove these;
- Unity, Windows native builds, persistence/restart, main integration, post-integration gates or release, all outside this review's authorised candidate scope;
- dashboard ingestion/audit/published-state synchronization and Drive mirror refresh. Per AgentWorkflow, this review can hand off, but operational synchronization remains outstanding; the single-document remit does not include editing those records.

**Return:** independent Tier-2 performed; source/engine gates passed as recorded; UI behavior and contract conformance **NEEDS CHANGES**. All 31 delta and 50 Stage 5 items are individually accounted for above. No Manager acceptance or integration is claimed.

[candidate]: https://github.com/jermur788/CCF/commit/1905433fbec69736c4c05ec629aa5941d11952ee
[manager]: https://github.com/jermur788/CCF/blob/docs/Multi-849-FED675-Manager-Final-Spec-Readback-and-Source-Admission-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-849-FED675-Manager-Approved-Spec-Readback-and-Desktop-Lane-Admission-20261120.md
[stage5]: https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/checklists/requirements.md
[js135]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L135
[js147]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L147
[js243]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L243
[js324]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L324
[js757]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L757
[js800]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L800
[js854]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js#L854
[core390]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs#L390
[core411]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs#L411
[labels74]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Core/PlayerSessionView.cs#L74
[css130]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.css#L130
[spec344]: https://github.com/jermur788/CCF/blob/1905433fbec69736c4c05ec629aa5941d11952ee/specs/multirack-sitka-2d-playable-standmap-v1/spec.md#L344
