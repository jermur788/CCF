# Multi #650 — first-year 2D Sitka player flow and mock UI

**Status: DRAFT — browser-only design input for Multi Manager #641; NOT an approved UI specification, implemented application, released annual engine, Unity parity verdict, or empirical Irish forecast.**  
**Prepared:** 16 November 2026, OpenAI ChatGPT GPT-6 (High requested; actual effort selector not observable).  
**Execution:** GitHub-only, independent `docs/multi-p1-650-browser-firstyear-ui-result-20261116-v1` branch. No physical worktree, source mutation, Host, port allocation, Unity, local test run, asset, save, Windows build, or ecological year execution.  
**Only authored file:** this Markdown. **Claim:** [#650](https://github.com/jermur788/CCF/issues/650). **Review owner:** [Multi Manager #641](https://github.com/jermur788/CCF/issues/641).

## 0. Authority, checked evidence, and design labels

The [complete #650 packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-650-browser-firstyear-ui-workflow-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-650-BROWSER-UI-FIRST-YEAR-WORKFLOW-DRAFT-PACKET-20261116.md) is binding for this **one-file documentation result**. The four owner-approved P1 technical documents are the reference for future application behavior: [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md), [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md), [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md), and [requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md).

**Precedence warning:** the four-file `spec.md` *opening current-authority declaration* records the product owner's explicit 16 November four-file approval. Its later historical DRAFT / NOT APPROVED passages preserve the review history; do not demote the current approval or turn its still-open C-2b/C-6 and unperformed acceptance boxes into PASS. The approved design is **permission to implement bounded packets with their separate gates**, not evidence that P1 biology, persistence, UI or the Host presently works.

**Read-only material actually inspected:** live `Docs/Project/coordination/active-tasks.md` (especially #639 exclusive source reservation); issues #639, #641, #649, #650; four-file P1 authority (spec ``0–7, 9; plan ``1–4, 11; tasks ``1–6, 10; requirements A–F/K); and the qualified P0 source lineage `Tools/MultiRackSitkaLab/README.md`, `src/Core/Metrics.cs`, `src/Core/Views.cs`, `src/Core/Models.cs` and `src/Ui/wwwroot/app.js`. Source names below are P0 **observations**, not a P1 implemented API. The existing P0 `StandMetrics` / `MetricsView` counts **eligible living adults** for `NPerHa`, `BaM2`, `BaM2PerHa` and `DqCm`; its `LiveTotal` / `LiveBelowThreshold` are separate counts. P1 must **not** relabel those P0 outputs as P1's all-living biological BA/N/Dq.

**Legend for each proposal:** **[A]** accepted product/spec contract; **[S]** direct read-only source observation from qualified P0; **[D]** UI proposal derived from approved contract, pending #641 review; **[X]** purely illustrative synthetic visual/text choice; **[O]** open technical/product decision requiring separately accepted resolution; **[F]** future implementation/qualification, not present. None of the wireframes are screenshots of a running P1 client.

The frozen original P0 fixture bytes/catalog/100-tree trust manifest (where applicable), their independent hashes, original polygon/IDs/statuses and P0 no-growth behavior remain unchanged. The proposed P1-derived 30m and compiled original trust entries remain **`PendingIndependentReview` until genuine independent evidence and Manager acceptance**. Do not replace these labels with “verified” from documentation alone. #639/#649 control source; this file controls no source path.

## 1. One-page player journey — first session and first year

| Step | Player sees / does | Present F0 state versus future authorized state | Contract |
|---|---|---|---|
| 1. Enter | Choose **Standalone P1 2D experiment** explicitly; never open or silently upgrade P0. Persistent banner: **“CCF adult rules — closed pure-Sitka experimental population. Synthetic inputs. NOT an Irish field forecast.”** Secondary exclusions: **“No recruitment or storms; not Scenario One or Unity.”** | **Current F0:** design mock/empty or read-only provider state; no assumption P1 Host exists. **Future:** domain-labelled entry supplied by a reviewed P1 adapter. | [A] spec ``0, 1, 5; REQ-B01, REQ-F08/F11 |
| 2. Choose domain | Three distinct labelled choices. Default *design selection* is **Derived 30m**, but no origin conversion or import happens implicitly. | `DerivedP1ThirtyMetreExperimentV1`: independent **30×30 m, 900m² / 0.09ha synthetic catalog**; NEVER re-sized original. `OriginalPolygonExploratoryV1`: exact original P0 read-only inspect; no annual/work in F0. `DerivedUnitySquareParityV1`: later source-legal **40m/80m** candidate only after owner C-6 GO and genuine C-2b review, not a 30m parity badge. | [A] D-084 / spec `0 B01, REQ-D05 |
| 3. Overview | Forest map and synchronized accessible tree table; property boundary and stem IDs; source-mode identity; inventory and separate ecological-year labels; count and area. | **Current F0:** “Manifest/trust not accepted — read-only” if pending; never fake a loaded approved 30m catalog. **Future:** actual state supplied by Core, not JavaScript computations. | [A] REQ-D01–D05; [D] screen layout |
| 4. Assess | Compare **all living** (including below-action-threshold stems) with the **actionable living adults (17 cm threshold in current P0 source; read actual threshold from provider)**. Inspect N, BA, N/ha, BA/ha, Dq and separate H diagnostic, plus natural-dead versus work-felled counts only when supported. | **Current:** show `— / unavailable` for unavailable P1 aggregate fields, never substitute P0 `eligibleLiveN` for all-living. **Future:** authoritative two-column metric payload with exact population definition, units and stage provenance. | [A] REQ-D01, REQ-E05, REQ-D06; [S] P0 `Metrics.cs` |
| 5. Inspect tree | Select one stem from map **or keyboard/filterable table**; see ID, species, location, age, current DBH/height/crown, biological status, action eligibility, prior vs current values and supported cause/year. | Before first year, previous-year fields are `not applicable`; pre-inventory dead have **unknown cause/year**, not a fabricated natural-death label. Show source status for every per-ID trace value. | [A] spec `5 and REQ-D03/E01 |
| 6. Read trust/limitations | Read visible banner and an expandable **Source & limits** panel; inspection remains available even if annual work is denied. | **Current:** `PendingIndependentReview` for unaccepted manifests; “P1 annual engine unavailable — no ecological year was run.” Never promise Irish yield, GROWFOR tree forecast, or Unity parity. | [A] C-7 / REQ-F08/F11; [D] panel |
| 7. Review action availability | Context-specific first-year action row: **Inspect** available; **Propose plan / Approve / Queue / Execute** shown as *future workflow*, with permission/status reason; distinguish a mere proposed/approved plan from trees actually felled. | **Current F0:** work command unsupported until accepted source/API gates. **Future:** mark → propose → approve → queue → execute stays distinct; no mortality/felling on selection, proposal or approval; no general forestry advice or automatic action recommendation. | [A] spec `5.2, REQ-E03/E05; [D] arrangement |
| 8. Advance year | A dedicated **Advance one ecological year** control, separate from existing P0 **“Advance experiment step (calendar only — NO BIOLOGICAL GROWTH)”**. An explicit confirmation names source domain, current→next ecological year, committed-work boundary, and limits. | **Current F0:** disabled/refused with reason **Annual engine unavailable; no year advanced**. **Future only after accepted manifest/trust, actual engine and permission:** request an atomic year transaction, show pending response, then server-confirmed year change. Never locally increment year or retry unknown-outcome biology blindly. | [A] spec `5.2; [S] P0 `LabConstants.AdvanceStepLabel`; [D] confirmation |
| 9. Compare and continue | Side-by-side previous/after year: exact year, all-living N/BA, actionable N/BA, Dq, H if diagnostic defined, growth addition, executed work, natural deaths; drill to changed stem IDs and event causes. | **Current:** `No ecological year results yet`. **Future:** render only a verified server-returned pair/ledger and its provenance; absent diagnostic H stays null with flag. Save/replay remain separately gated. | [A] REQ-E01/E02/E05/E06, REQ-D06; [D] compare screen |

The first external game demo remains the **complete 3D Scenario One**, not this standalone lab ([current milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md)); this journey does not alter that schedule.

## 2. Interface map, screen sketches and accessibility

**[D/X] Layout rule:** laptop/desktop first, 1280–1440px useful width, readable at 1024px by stacking the right inspector below the map, no horizontal body scroll; 16px base, visible keyboard focus, labelled controls and numeric units; no color-only mark/state meaning. Map and table select the **same stable tree ID**. Users can complete inspection without any spatial click. Forest metrics and forestry mathematics come from P1 Core; browser does only layout/formatting. Never assign a fixed Host port.

### Screen A — experiment/source launcher, F0 safe

~~~text
+--------------------------------------------------------------------------------+
| STANDALONE P1 2D • CCF adult rules — closed pure-Sitka experimental population  |
| SYNTHETIC INPUTS • NOT AN IRISH FIELD FORECAST • No recruitment or storms       |
+--------------------------------------------------------------------------------+
| Choose what you are inspecting:                                                |
|                                                                                |
| ( ) Derived 30m experiment   30 x 30m / 0.09 ha  [PendingIndependentReview]    |
|     Independent synthetic catalog; source-inspired; NOT Unity parity           |
|                                                                                |
| ( ) Original P0 polygon      Original area / IDs [READ-ONLY]                  |
|     Verified-source import only; NO ANNUAL ADVANCE OR WORK in F0                |
|                                                                                |
| ( ) Unity square candidates  40m / 80m [NOT AVAILABLE — C-6/C-2b]               |
|                                                                                |
| [Open read-only inspection if available]        [Source & limits]              |
| Status: No approved P1 annual engine available in this design mock.            |
+--------------------------------------------------------------------------------+
~~~

**Transition:** choosing a mode only requests a validated domain descriptor. A nonexistent/pending manifest must not be represented as an openable validated 30m stand. Original polygon import preserves original coordinates, area, IDs and bytes; never computes a “30m version” by trim/scale. Invalid tags/geometry reject; no fallback to nearest square. [A: REQ-B02–B05, D05, N01/N05/N13.]

### Screen B — overview and tree inspection (two data definitions exposed)

~~~text
+--------------------------------------------------------------------------------+
| P1 / DerivedP1ThirtyMetreExperimentV1     ECO YEAR: --   INVENTORY: --          |
| SYNTHETIC CLOSED ADULT SITKA • NOT IRISH FORECAST • NOT UNITY NUMERICAL PARITY   |
+--------------------------------------------------------------------------------+
| SOURCE: catalog pending review / no executable year yet   [Why?] [Limits]      |
|                                                                                |
| ALL LIVING — biological denominator        ACTIONABLE — work subset            |
| N: --    N/ha: --    BA: -- m2           N: --    BA: -- m2                   |
| BA/ha: -- m2/ha  Dq: -- cm               DBH threshold: 17 cm*                |
| H (diagnostic): -- m [unverified sampling]  Dead pre-inventory: --             |
| *Show threshold returned by source; 17 cm is current P0 example, not hardcode. |
+----------------------------------------------------+---------------------------+
| MAP: square/polygon perimeter                      | TREE INSPECTOR           |
|  [zoom] [reset] [layer: status] [legend]            | ID: --  Species: Sitka    |
|                                                    | status: --                |
|   o  o  o     selected ID mirrored in list         | DBH: -- cm  H: -- m        |
|     o  [o]         (schematic dots only)           | crown: -- m  age: -- yr   |
|   o        o                                      | XY: -- / -- m             |
|                                                    | Eligible for work: --    |
|                                                    | Previous year: n/a       |
|                                                    | Cause/year: unknown if   |
|                                                    | pre-inventory dead       |
+----------------------------------------------------+---------------------------+
| TREE TABLE [search ID] [status filter] [sort] [keyboard-select]                 |
| ID         Status     DBH(cm)   Height(m)  Age(y)  Actionable?  Source          |
| --         --         --        --         --      --           --             |
+--------------------------------------------------------------------------------+
| [Inspect]   [Plan work — FUTURE]   [Advance year — UNAVAILABLE]  [Save — FUTURE] |
+--------------------------------------------------------------------------------+
~~~

All values are **placeholders**, not invented 30m values. “17 cm” is a read-only P0 lineage example (`AdultThresholdMm` 170 mm) and must be sourced per P1 fixture/runtime, not hardcoded into a biological density denominator. **H** is separately labelled a *synthetic diagnostic* with sample `k` and flag; no assertion of original Broad–Lynch field sampling equivalence [A: spec `5.3]. Hover is not the only discovery path: table row, detail card and focus outline expose the same details; row order is deterministic, ID sorting ordinal when specified.

### Screen C — original polygon inspection and denied actions

~~~text
+-------------------------------------------------------------------------------+
| OriginalPolygonExploratoryV1   [READ-ONLY IN F0]   SYNTHETIC P0 ORIGINAL        |
| Polygon/area/IDs unchanged • NOT scaled to 30m • NOT Unity geometry parity     |
|-------------------------------------------------------------------------------|
| Exact source identifier / catalog trust: [verified | pending | refused]        |
| Original fixture area: from verified source, NEVER derived 0.09ha              |
| [Map original polygon]    [Per-ID original inventory table]                   |
| [Inspect tree]     [Work: unavailable]   [Advance ecological year: unavailable]|
|                                                                               |
| Why no annual? Original polygon growth requires later P1-POLY implementation,  |
| actual review and approval. P0's calendar-only step is not biological growth. |
+-------------------------------------------------------------------------------+
~~~

**F0 is not an empty 30m calendar that can silently advance the original.** The original P0 may be inspectable when its independent trust is valid; pending/invalid trust refuses import rather than showing guessed content. [A: REQ-B02/B04/D05, tasks P1-POLY.]

### Screen D — future work-plan commitment boundary (NOT present F0 capability)

~~~text
+--------------------------------------------------------------------------------+
| WORK PLAN — FUTURE, subject to stable P1 API and independent qualification      |
| Selection: stable tree IDs         Current forest revision: --                 |
| [Preview; NO change to forest]  N/BA removal impact: -- [Core-supplied only]    |
|                                                                                |
| Draft -> Proposed -> Approved -> PendingWork -> Executed                       |
|            \-> Rejected / Cancelled / Stale                                    |
|                                                                                |
| [Propose]  [Approve]  [Queue]  [Execute confirmed work] [Cancel]               |
| All controls unavailable now: P1 work engine/API not approved for this screen. |
| The approval is NOT a felling event. Execute validates current IDs/revision/    |
| session; an invalid set refuses the entire operation.                         |
+--------------------------------------------------------------------------------+
~~~

**[A] Contract:** marking/selection/proposal/approval/queue never changes living N/BA; **ExecuteWork** alone commits eligible felling at current year/DBH before a later ecological advance; natural death is a different cause, not timber work. Loaded open approved/pending plans become stale one per plan, zero load events if none open; this is the owner's first-experiment policy, **not a claim of implemented P1 persistence**. No buttons should suggest approval equals automatic execution. The precise P1 UI command shape, exposed capabilities and which actions are offered in F0 remain **[O/F]**; the diagram illustrates a future lifecycle, not new permissions. [Spec `5.2, REQ-B04/E03/E04.]

### Screen E — future advance confirmation and first-year comparison

~~~text
+----------------------------- YEAR ADVANCE -------------------------------------+
| Proposed action: Advance exactly ONE ecological year                          |
| Domain: DerivedP1ThirtyMetreExperimentV1   From ECO YEAR: -- to: --            |
| Input/manifest: --  Review status: [accepted required]                         |
| Executed work before this year: --    Any reserved/stale plans: --             |
| Synthetic closed Sitka • NOT Irish forecast • No recruitment or storms        |
| [Cancel]                   [Advance one year — only if enabled by Core]        |
+--------------------------------------------------------------------------------+
| RESULT AFTER SERVER-COMMITTED SUCCESS                                          |
| Metric                         Before             After            Change      |
| All-living N                   --                 --               --          |
| All-living BA (m2)             --                 --               --          |
| All-living BA/ha (m2/ha)       --                 --               --          |
| Actionable N / BA              -- / --            -- / --          --          |
| Dq (cm); H* (m)                -- / --            -- / --          --          |
| Executed fell / natural dead   -- / --            -- / --          --          |
| *H diagnostic; not a verified Irish inventory definition                       |
| [Changed tree IDs]  [Year ledger]  [Provenance]  [Previous year]               |
+--------------------------------------------------------------------------------+
~~~

The browser cannot infer that a timed-out request succeeded or failed. On ambiguous response, show **“Outcome unknown — refresh state/command status; do not repeat annual execution”**. It may read state/command history when a future API supports it; no new command-ID policy or retry endpoint is approved here. Never fabricate a completed “Year 1” chart in F0. [A: spec `5.2, REQ-E01/E06/E07.]

### Screen F — refusal, empty, pending-review and future save/replay shells

~~~text
+-------------------- OPERATION DID NOT RUN -------------------------------------+
| Annual advance unavailable                                                    |
| Code/reason: [provider returned category; never made up in browser]           |
| Source: OriginalPolygonExploratoryV1    Ecological year remains unchanged     |
| Details: Original polygon work/year refused until P1-POLY qualified.          |
| [Return to inspection]  [View source and limitations]                         |
+--------------------------------------------------------------------------------+
| EMPTY / NO SELECTION: “No tree selected”; not an error or a fabricated stand. |
| PENDING: “Catalog / trust status PendingIndependentReview — no annual step.”   |
| DENIED: “Action ineligible, stale, or untrusted; no work performed.”           |
| FUTURE SAVE/REPLAY: [Not released] [No file path entry] [No save success claim]|
+--------------------------------------------------------------------------------+
~~~

Refusal is **not** an optimistic mutation followed by an error toast. Display a stable provider code and plain-language explanation, preserve source domain, last-known year/revision, and whether authoritative post-request state was returned. Keep real structural/trust/unsupported-platform/refusal classes distinct. Do not collapse `SaveCommittedDurabilityUnknown` into “failed and old save preserved”; these proposed future semantics require their own source qualification [A: spec ``5.2, 9.9; REQ-E07, currently untested].

## 3. Screen states, commands and refusals (presentation contract)

| State and trigger | Visible affordance and exact truthful result | Side-effect expectation | Authority |
|---|---|---|---|
| `NoSession` / unloaded | Launcher, no stand numbers; “Choose an available source.” | No biological state created by view | [D] |
| `PendingIndependentReview` catalog or compiled trust | Source and type visible; “Not accepted for annual use”; no simulated measurements or run button | No annual/work; no silent override | [A] F0 gate; [D] text |
| `ReadyReadOnly` original P0 | Map/table if verified; **read-only** label; work/year denied | No rescaling, P0 writes or annual events | [A] REQ-B02/B04/D05 |
| `Ready30mF0` first derived catalog, independent gate not yet passed | Inspect **only** if validated to the level required by source API; otherwise pending/refused; work/year absent | No ecological year until manifest + original trust accepted | [A] #639 gate |
| `AnnualEngineUnavailable` in current F0 | Disabled “Advance ecological year” with adjacent explanation; if a command reaches Host, fail closed | No year increment, events, BA/N change | [A] packet and tasks P1-F0 stop |
| `AnnualAvailableFuture` on accepted domain+engine | Deliberate confirmation, server response, then compare | One server-owned transaction, no browser biology | [F] spec `5.2 |
| `UnsupportedSourceMode` / wrong domain for action | Show exact source tag, supported operations, “Switch to an eligible experiment”; no implicit conversion | No fallback to 30m or legal 40/80 | [A] D-084, REQ-D05 |
| `EmptySelection` / zero living stand | Map/table empty state; N=0/BA=0 only when Core says so, Dq/H=null; no plan action | No phantom tree/zero-as-missing confusion | [D] and [A] `5.3 |
| `ActionDenied` ineligible/stale/dead/duplicate/missing ID | Row-level highlight + provider error; plan stage unchanged unless authoritative audited rejection event exists | Atomic refusal; proposal/approval not work | [A] REQ-E03/E07 |
| `SessionLoadedWithOpenPlan` future | “Prior approval is stale; create/approve a new plan.” Show affected plan IDs | One `PlanStale(SessionLoaded)` per open Approved/PendingWork plan; zero otherwise | [A] D-082; [F] engine |
| `SaveReplayUnavailable` current | No file picker, no guessed names, “P1 save/replay not released” | No data write or path access | [D] bounded by REQ-B06/E07 |
| `SaveOrLoadRefused` future | Exact code with unchanged active state and file safety; outcome unknown ≠ rollback | No auto-retry; status must come from Host | [A] REQ-E07; [F] |
| `UnityParityNotQualified` / 40–80 requested | 40/80 mode listed as future; “C-6 owner access and C-2b captured-vector review outstanding” | No parity badge/run or unauthorized Unity | [A] REQ-C01/F01 |
| `ErrorOrUnreachableHost` | “Source unavailable; no update confirmed”; preserve last verified state with **stale-data** marking and refresh control | No optimistic local increment | [D] |

**Refusal vocabulary:** `PendingIndependentReview` is an existing trust-state label from the packet, **not a new HTTP status**. The existing **P0** `OpResult(Code,Message)` and HTTP 409 behavior are [S]; no P1 Host route, code or response contract is claimed. Candidate P1 `PlatformUnverifiable` / `StructuralInvalid` / `UnsupportedVersion` / `SourceOrGenesisIncompatible` / `FingerprintMissingOrInvalid` / `PlatformFingerprintMismatch` / `ReplayMismatch` follow **proposed/unimplemented** spec `9.9 ordering; adoption and exact serialization remain [O]. The UX shall display a returned code and never infer a successful operation from UI intention.

## 4. UI information and DTO/API inventory — requirements, NOT actual P1 interfaces

The following are **semantic data contracts**, not endpoints or a TypeScript/C# schema. Producer is always a **future independently reviewed P1 Core/Host**, except the *qualified existing P0* read-only source fields called out explicitly. UI is a consumer, never a second growth/geometry/BA/CI/RD engine. Status of every row: **[D/F] until implementation and source review**.

| Screen/transition | Fields needed from producer (names indicative; domain values exact) | Source evidence / gap and consumer rule |
|---|---|---|
| Launcher / mode descriptor | `sourceDomainTag` exact enum; `modeDisplayName`; `geometryKind`; `supportedOperations`; `trustReviewStatus`; `manifestVersion`; `fixtureId`; `fixtureDigest`; `sourceSnapshotDigest`; `sourceClass`; `modelVersion`; `rngVersion`; `capabilities`; explicit `reasonUnavailable` | [A] REQ-B01/D05/F08. **No P1 endpoint exists by this document.** Distinguish 30m, original polygon, legal 40/80 from the tag, never dimensions inferred from tag or reverse. |
| Source/limitations panel | `sourceClaim`, `syntheticClassification`, `isIrishForecast=false`, `isUnityNumericalParity`, `isUnityGeometryParity`, `recruitmentExcluded`, `stormsExcluded`, `experimentalOnly`, `evidenceClass`, `independentReviewStatus`, `limitations[]`, `sourceRevision` | [A] spec ``0–2, REQ-F08/F11; parity flags require actual accepted evidence, never default true. |
| Stand geometry/map | authoritative boundary polygon/square vertices and XY origin/axes, unit mm/m, orientation conversion to XZ, elevation policy, area m² and ha plus precision/bits and derived/original provenance, tree positions and ordered stable IDs | [A] REQ-B03/D05; P0 `PointM`/`SessionView.PolygonMetres` [S], but P1 conversion/Float32 bits need a new reviewed contract. No original→30m rescale. |
| Stand aggregate | **All-living** `liveN`, `liveNPerHa`, `liveBaM2`, `liveBaM2PerHa`, `liveDqCm`, density `RD` (if supported), separately **actionable** `eligibleLiveN`, `eligibleBaM2`, `thresholdCm`; `preInventoryDeadN`, `naturalDeadN`, `workFelledN`; `diagnosticH`, `sampleK`, `HMethodFlag`; units/rounding/`notEvaluated` | [A] REQ-D01/D03/D06/E05. Existing P0 `MetricsView` provides eligibility-based `EligibleLiveN/NPerHa/BaM2/BaM2PerHa/DqCm` + `LiveTotal/LiveBelowThreshold` [S], **not** the future P1 all-living BA/Dq. |
| Individual tree / map/table | `treeId`, `species`, `demand`, `stage`, `xMm`, `yMm`, `liveStatus`, `isPreInventoryDead`, `ageYears`, `dbhCm`, `heightM`, `crownRadiusM`, `eligibleForWork`, `currentPlanId/state`, provenance class and field availability | P0 `TreeView` [S] exposes ID, species, XY metres, DBH, H, inventory age, status, mark, threshold/eligibility, plan info; **no P0 crown**. P1 age/crown/status causality requires accepted Core. |
| Individual year trace | `before/afterYear`, `before/afterAge`, `before/afterDbh/height/crown`, `preGrowthCI`, `suppression`, `increments`, `hazardP`, `rollValue`, `rollStatus`, `growthSourceEvidence`, `mortalityCause`, `mortalityYear`, `fellPlanId`, `changeKind` | [A] REQ-E01/E02; private at-evaluation `S/mean/p` and actual source roll-calls **unobserved** in later Tier-1 Unity capture. Mark unavailable/notEvaluated/sourceFunctionRecomputed correctly; do not masquerade reconstructed diagnostic as source observation. |
| Action availability / proposal | `canSelect`, `canPropose`, `canApprove`, `canQueue`, `canExecute`, `canAdvance` with **reason for each denial**; `selectionTreeIds`, `planId`, `planStage`, `baseForestRevision`, `approvalSessionBinding`, `previewBefore/After` (server-computed) | P0 `PlanActions`, `PlanView`, `PreviewView` [S] are reference patterns **not** P1 APIs. P1 exact stages and approval epoch under [A] spec `5.2; exposed P1 capability shape [O]. No browser-side approval or felling. |
| Annual confirmation/request | `expectedSourceDomain`, `expectedEcologicalYear`, `expectedForestRevision`, `manifestId/digest`, `commandId` (if/when protocol adopted), `currentPlanDisposition`; user cancels or submits one intention | [A] spec `5.2 intent, [O] C-1 command-ID and rejection semantics. Do not choose request URL, Host port or optimistic local year update. |
| Committed annual result | `transactionStatus`, `ecologicalYearBefore/After`, `stateRevision`, `pre/postStateHash`, `historyTip` (if adopted), `allLivingBefore/After`, `actionableBefore/After`, `BAAccounting`, `fellIds`, `naturalDeathIds`, `cause/years`, `perIdDeltas`, `sourceClaim` and `manifestDigest` | [A] REQ-E01/E02/E05/E06. For empty result, do not invent deltas. Annual child deaths are not extra replay mutation events. |
| Year compare/history | two committed snapshots/annual ledgers with authoritative population definitions, provenance, nullability, H diagnostic flag, current/previous ecological year and model/RNG/seed identity | [A] REQ-E05/E06; [D] compare layout. No unauthorised persisted history or synthetic trend interpolation. |
| Refusal/transport failure | machine code, human message, `operationAttempted`, `domain`, `stateUnchanged` if authoritative, `currentRevision/year`, `invalidTreeIds`, `planId`, `requiresReview`, retry-safe/outcome-unknown category; source readiness flags | P0 `OpResult` + HTTP 409 [S], P1 categories [O/F]. UI can only assert unchanged if provider confirms or state is re-read. |
| Save/list/load/replay **future** | reviewed P1 format/version/trusted genesis/digests, saved title/name, file identity (not user path), replay result/detached validation, current epoch, stale plan IDs, exact durability outcome, platform fingerprint diagnostics | [A] C-1A conditional staleness; [O] C-1 archive/fingerprint/fsync contract. **No P1 save API or file picker selected.** P0 `.sitkalab.json` and `/api/saves/...` [S] must not be copied as if P1-owned. |
| Export **future** | export class and source-domain tag on every chart/table/report, model/RNG/source/fixture, units, evidence status, excluded mechanics, normalized per-ID and stand result, required synthetic-forecast warning | [A] REQ-F08/F11; [D] display/copy layout. No new export endpoint or saved file written by this task. |

**Data-quality rules [D] for UI implementation:** use `—` only for *unavailable / no response*, `n/a` for definitionally inapplicable, 0 only when Core returns 0, and explicit `notEvaluated` for a nonexecuted biological branch. Render ages as **ecological state age** versus immutable P0 `AgeAtInventoryYears`, and keep `inventoryYear` separate from `ecologicalYear`; P0 `experimentStep` is calendar-only, **never** display it as a biological year. Round for display only (e.g., cm/m) while IDs, bits and authoritative state remain unchanged; exact precision policy is C-2a [O]. Preserve negative zero/failed bits in source traces rather than hiding provenance.

## 5. User stories and UI acceptance criteria — traced, not self-authorized

| ID / category | Story and later acceptance check (UI-only) | Trace to accepted spec / open choice |
|---|---|---|
| U-01 [A/D] | As a player I must recognize this is an **optional standalone experiment**. On launcher, overview, compare and export, the synthetic/no-Irish-forecast/no-recruitment-or-storm label is visible without relying on a tooltip. | REQ-F08/F11; spec ``0–1 |
| U-02 [A/D] | As an investigator I see **exactly which domain** I opened. Fake/swapped tags, a 50m fixture sold as legal Unity, wrong area/axes or original→30m resizing cause a clear refusal, never a silent mode conversion. | REQ-B01–B04/D05; D-084; N01/N05/N13 |
| U-03 [A/D] | As a stand manager I see **all living N/BA/RD** separate from **eligible live adult N/BA**. Verify displayed values exactly match returned Core values; the 17cm example labels the subset, not all-living density. | REQ-D01/E05; P0 Metrics.cs source check |
| U-04 [A/D] | As a keyboard or screen-reader user I inspect every shown tree through a searchable, sortable table; map and table selection remain synchronized by stable ID; status/cause is text-visible not just color. | [D] access proposal supporting spec `5 individual IDs; **review needed** |
| U-05 [A/D] | As a player I inspect an original P0 polygon with its **unaltered** geometry/IDs/source label and cannot propose work or advance a biological year in F0, even through a direct command. | REQ-B02/B04/D05; P1-POLY gate |
| U-06 [A/D] | As a player I never confuse an inactive, pending or unsupported experiment with a successful run. Buttons explain unavailable states and the screen leaves year and counts unchanged after a verified refusal. | REQ-E07 and F0 manifest/trust gate |
| U-07 [A/D/F] | Once separately accepted annual service exists, I deliberately confirm one-year advance. Compare is shown **only after** committed success; ambiguous response triggers readback guidance, not second execution. | REQ-E01/E06/E07; C-1 command retry details [O] |
| U-08 [A/D/F] | On compare screen I can explain each N change as executed fell or natural death, view per-ID deltas and see pre-inventory dead as unknown historical cause rather than a first-year death. | REQ-D03/E01/E02/E05 |
| U-09 [A/D/F] | For future work planning I see Draft→Proposed→Approved→PendingWork→Executed and terminal states; before Execute, authoritative BA/N remain unchanged; invalid ID sets never partially fell. | REQ-E03/E04, spec `5.2; future P1 API [O] |
| U-10 [A/D/F] | On future load, open Approved/PendingWork approvals are shown stale with reason, each exactly once; when none open there is no fabricated load event. The UI requests fresh legal approval before work. | D-082 / REQ-B04/E03; future save/replay [O] |
| U-11 [A/D/F] | A future save/error display differentiates file **precommit refusal**, committed-but-durability-unknown, later list-refresh failure and lost-response uncertainty; never states “previous file retained” after a successful rename with failed directory fsync. | REQ-E07; proposed `9.9 adoption [O] |
| U-12 [A/D] | Mode `DerivedUnitySquareParityV1` remains explicitly **not qualified** until actual legal-square Unity capture and postcapture C-2b acceptance; the 30m UI never inherits a parity badge. | REQ-C01/F01/F03, D-084; C-6 and C-2b OPEN |

**Visual-only pass conditions:** no source/provenance banner missing in any navigable main view; no inferred all-living stand number; no enabled command with a denied capability; no numeric year advance without returned transaction; no misleading 30m→Unity badge; no controls accessible solely via hover or color. These are acceptance **proposals** to be confirmed with Manager and eventual P1-05 implementer; no test performed by this document.

## 6. Focused UI test matrix — to run later, not performed

| Test | Setup and action | Expected user-visible witness | Gate/status |
|---|---|---|---|
| T01 source launcher | Empty/unstarted P1 | Domain identities and full warning; no fake numbers or Year1 | DRAFT / UNRUN |
| T02 pending 30m | Trust `PendingIndependentReview` | Annual/work unavailable, no fictitious acceptance | DRAFT / UNRUN |
| T03 valid original | Independently trusted P0 original loaded for inspection | Original polygon/IDs/status, read-only, no 0.09ha rescale | DRAFT / UNRUN |
| T04 direct original advance attack | Send future annual/work intent on original F0 | Host refuses; UI shows unchanged year, no fallback | DRAFT / UNRUN |
| T05 legal Unity mode | Select 40/80 with no C-6/C-2b | Future-only; no Unity parity badge or run | DRAFT / UNRUN |
| T06 wrong mode/area | Forged tag/geometry, 50m-as-Unity, changed axes/elevation | Explicit refusal; no rendering as accepted mode | DRAFT / UNRUN |
| T07 metrics definition | Fixture with live stems below 17cm threshold | all-living vs eligible BA/N differ correctly and visibly | DRAFT / UNRUN |
| T08 no living | Core returns N=0 | N=0, BA=0; Dq/H n/a, not 0 or fabricated estimate | DRAFT / UNRUN |
| T09 map/list parity | Select same stable ID using mouse, keyboard table, search | Identical inspector fields and focus/selection state | DRAFT / UNRUN |
| T10 pre-inventory dead | P0 Dead without history | “Cause/year not recorded” and no new death event | DRAFT / UNRUN |
| T11 plan staging future | Mark/propose/approve/queue without Execute | No BA/N or status change; stage accurately labelled | FUTURE / UNRUN |
| T12 plan refusal future | Duplicate/missing/below-threshold/dead IDs; stale revision | Atomic denied result, affected IDs shown, zero partial fell | FUTURE / UNRUN |
| T13 annual unavailable now | Click if control present / send direct command | Disabled or refused, ecological year unchanged | DRAFT / UNRUN |
| T14 future one-year success | Reviewed engine; commit one year | one step, authoritative before/after metrics, changed IDs, separate death/fell | FUTURE / UNRUN |
| T15 unknown outcome | Simulated response loss after request | “Outcome unknown”; refresh/readback, never auto-resubmit biology | FUTURE / UNRUN |
| T16 save/load conditional staleness | One open approved/pending plan vs no plan | Per-open-plan staleness exactly once; no event for none; no invented approval persistence | FUTURE / UNRUN |
| T17 malformed/source incompatible file | Wrong P1 format/fingerprint/manifest or P0 masquerading as P1 | Exact refusal, no overwrite, no partial activation or fake successful load | FUTURE / UNRUN |
| T18 post-rename durability uncertain | Directory fsync failure after rename | Correct “committed, durability unknown”, not “old file preserved” | FUTURE / UNRUN |
| T19 responsive/accessibility | 1280, 1024 widths, 200–400% zoom, keyboard-only, reduced motion | No critical horizontal overflow; labels/roles/focus; map-independent inspection | FUTURE / UNRUN |
| T20 P0 unchanged | Run existing P0 Core/Host/browser regressions before and after P1 UI change | P0 calendar-only text, hashes, fixtures and behavior unchanged | FUTURE / UNRUN |
| T21 exported provenance | Future chart/export in all 3 modes | exact domain tag, full scientific warning, null/source flags and unverified parity statuses | FUTURE / UNRUN |

Test implementation must use **real browser** evidence for focus, clicking and visible states, not only headless Core response tests; retain the qualified P0 source's independent regressions and real browser checks. Scope only touched paths, no new full Unity Tier-3 for isolated P1. All 72 checklist boxes and N01–N13 in the approved source specifications remain **unchecked/unrun** by this UI design.

## 7. Smallest-first later UI backlog (proposal, not a dispatch)

1. **UI-00 — read-only contract freeze [D]:** Manager and P1 Core owner review the three-domain/eligibility/stand-metric schema, per-field provenance and capability/refusal DTO. Require real reviewed manifest/trust output. No code until interface ownership and API stable.
2. **UI-01 — safe launcher and read-only inspection [D]:** explicit three domain names, scientific disclosure, geometry map + accessible tree table, inspector, two-population metrics, availability/refusal. For F0, **original inspect-only and 30m pending/no-year** remain truthful. Do not copy P0 calendar-only step into P1 biology.
3. **UI-02 — accepted 30m annual flow [F]:** only after separate #639 manifest/trust independent PASS, Manager acceptance, operating engine, reviewed request/response DTO and source ownership. Confirmation, pending/unknown, before/after comparison, per-ID annual trace. No new mortality calculations in JS.
4. **UI-03 — work-plan boundary [F]:** after P1 event/work API and safe approval binding, actions, whole-set refusal and signed execution result; does not change D-082 conditional stale-on-load.
5. **UI-04 — legal-square and genuine original support [F]:** 40/80 legal Unity mode after C-6 and C-2b; separately original polygon annual after P1-POLY. Each preserves tag/source disclosure and refuses unsupported paths; never lend parity to 30m or original.
6. **UI-05 — save, replay, export and real-browser qualification [F]:** only after C-1 safe storage, durability/fingerprint/replay and P1-03; independent UI review, accessibility and P0 regression. No direct file picker or port assumption.

**Prospective nonoverlapping file-set split — MUST be rechecked by Manager at dispatch:** #639/#649 retains `Tools/MultiRackSitkaP1Lab/**` **now**, including Core/Host/tests/manifest/fixture and any existing UI paths; #650 has **only this result Markdown**, no P1 source claim. After #639 hands off and a stable approved DTO emerges, Manager may grant a **new** P1-05 UI-specific set such as `Tools/MultiRackSitkaP1Lab/src/Ui/**` to one UI writer, with Core/Host owned serially by a separate source owner; if same-files overlap occurs, stop and use a staged single writer or an accepted API handoff. Proposed path is **not a current reservation**. Never edit `Tools/MultiRackSitkaLab/**` (P0), `Assets/**`, `Packages/**`, `ProjectSettings/**` or live coordination from this task. No synthetic fixture/hash, approved spec, physical Ops/Unity lease or Host port is assigned here.

**Dependencies and unresolved decisions:** F0 independent approval of the complete 30m manifest and compiled original P0 trust table; actual adapter and Host read-only DTOs; C-1 canonical archive/identity/commands/security/replay; C-2a numeric fields and presentation; C-2b **postcaptured only**; C-3 all-living synthetic Mature-equivalent population; C-4 initial crown/age/pre-inventory-dead; C-5 polygon/H/SI rules; C-6 separate owner Unity GO for legal 40/80; P1-POLY source/geometry implementation and review; P1-03 save/replay work; P1-04 science comparator. Until accepted, these are **OPEN/UNVERIFIED** and screen capabilities must advertise that truth. No acceptance of new product mechanics, save policy or technical coefficients is requested from this UI document.

## 8. Handoff and evidence integrity

**Deliverable:** one new Markdown on independent docs-only branch; no code. This is a **reviewable draft for #641**, not an APPROVED design, verified browser output or completed P1-05. The owner-approved technical four-file spec remains untouched and authoritative within its gates; any conflict must be corrected through an authorised Manager/spec review rather than implementing from this handoff. The reviewed original P0 100-tree/trust/catalog material and all hashes remain frozen; unaccepted candidates retain `PendingIndependentReview`.

**Requested Manager review:** (1) validate source-mode and current-F0 versus future-engine text; (2) decide whether the candidate UI fields and action boundaries match the approved product intent; (3) distinguish any new product choice for owner review instead of silently approving; (4) allocate future source/Host/UI file ownership only after #639/#649 handoff and actual API stability. Review verdict must be **APPROVED AS DRAFT INPUT / NEEDS CHANGES / REJECTED**; independent functional acceptance is not implied.

**Verification of this task is GitHub-only:** the writer will read back this exact file and compare the doc branch with its inspected `main` parent. No local/browser app tests were run, and dashboard, Drive and physical Ops synchronization were not verified. **User action:** none for this documentation draft; later owner C-6 Unity access is a separate future gate.
