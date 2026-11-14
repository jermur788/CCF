# MULTI-002 — Sitka 2D Reference Laboratory P0: IMPLEMENTATION HANDOFF

**Date:** 14 November 2026 · **Claim:** [#494](https://github.com/jermur788/CCF/issues/494) · **Status:** **HANDED-OFF for independent review — NOT integrated, NOT user-accepted**
**Scope:** P0 only. Synthetic flat Sitka stand, **NO BIOLOGICAL GROWTH**, standalone C#/.NET Core + loopback browser UI. Nothing here is a measured Irish stand, a validated growth model, a Scenario One feature, a Unity component or a Windows game build.

## 1. Identity, host and ownership receipts

| Item | Value |
|---|---|
| Actual worker | **Anthropic Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code inside local T3 Code. **Not** the packet's recommended Sol/Codex High: the two Codex attempts in this thread failed on a usage limit (resets 2026-11-18) and the owner switched provider and said "try". |
| Host | `jer-laptop`, native Linux 7.0.0-42 (Intel i5-6300HQ, 4 cores, 15 GB). **Not** the proposed "HP WSL" host; `/home/jer/Development/CCF-HP-Worktrees/...` did not exist. |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-002-SitkaLab-P0-20261114` (ext4, 153 GB free). **The owner explicitly approved this substitute** in the T3 session before any edit, in answer to a direct question. |
| Branch | `feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1` |
| Starting HEAD (approved spec) | `c92af201b6366e7e6314d587b6d4071dee352fa3`, worktree clean, 0 ahead of `docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1` |
| Candidate source commit | `2a76260e176023ca19e1ff6b56c1258c63320f20` (all production source, fixtures and tests; this handoff is the only later commit) |
| RUNNING registration | posted on #494 **before** source edits: <https://github.com/jermur788/CCF/issues/494#issuecomment-6875568398> |
| Board / Unity | Live `Docs/Project/coordination/active-tasks.md` on `main` had **no** MULTI-002 or #494 row; no Unity or `dotnet` process was running at preflight. **No board lock was claimed or edited** (no authorised board writer was available and none was needed). No Unity lease; Unity was never launched. |

Preflight was read-only first: issue #494 and comments, the full dispatch packet, the four approved spec files, the live board, `git worktree list`, host, .NET SDKs (8.0.133 and 10.0.114), Node v26.13.1, Chromium 156, disk. Nothing was stashed, reset, cleaned or killed. Two stray host processes I started myself were stopped by their own PID.

## 2. What was built (all under `Tools/MultiRackSitkaLab/`, 50 new files)

* `src/Core` — class library, no Unity/web dependency: exact integer geometry (Int128), stand metrics, plan/tree state machine (`Engine`), canonical JSON writer, strict file reader with header gate, replay, fixture catalog, lab data store with path/symlink safety, display DTOs.
* `src/Host` — ASP.NET Core minimal API bound to `127.0.0.1` only, request guard, static UI server.
* `src/Ui/wwwroot` — SVG map (pan/zoom/scale bar/cursor readout), inspector, Core-fed metrics and previews, plan cards with separate Propose/Approve/Queue/Execute/Reject/Cancel buttons, calendar-step button, fixture reset with in-page confirmation, save/load, replay verification, export.
* `fixtures/synthetic` — `oracle-quarter-ha` (3 trees), `sitka-flat-demo-v1` (162 trees, skewed 5-vertex polygon, dead / below-threshold / on-boundary stems), `sitka-flat-perf-2000-v1` (2,000 trees) + `MANIFEST.sha256`.
* `schemas/lab-file-v1.md` (authoritative) and `lab-file-v1.schema.json`; `README.md`; `tests/` (Core, Host, real-browser driver, schema check, mutation controls); `tools/FixtureTool`.
* **Left incomplete / not done:** nothing in P0 scope is knowingly missing. Not done by design: P1 ecology, real data, Unity/Scenario One, Windows build, CI wiring, Firefox/Windows/WSL runs, a headed (non-headless) browser run.

## 3. Requirement and acceptance status

PASS/FAIL below means **writer-side verification with the evidence listed**. Nothing is independently reviewed yet.

| Req / AC | Result | Evidence |
|---|---|---|
| LAB-01 preflight, no overlap | PASS | §1; #494 comment |
| LAB-02 Core schema, fixtures, polygon, BA/N/Dq | PASS | Core suite (`MetricsAndGeometryTests`, `FileFormatTests`), fixture pins |
| LAB-03 state machine, atomic work, stale tokens, events, saves | PASS | Core suite (`StateMachineTests`, `PersistenceAndReplayTests`, `LabDataStoreTests`, fuzz) |
| LAB-04 2D map + host + Core parity + loopback security | PASS (headless Chromium) | Browser UI-01…UI-15, Host suite |
| LAB-05 Tier 1 + Tier 2 qualification, negatives, 2,000 trees | PASS for the writer's runs; **independent review PENDING** | §5 |
| AC-01 only `Tools/MultiRackSitkaLab/**` | **PASS** | `scope-proof-source.txt`: 50 files, all **A**dded, none outside the subtree, no `.meta`, none under Assets/Packages/ProjectSettings/specs/Docs/Project/ContentReview |
| AC-02 same ID/XY/DBH/status/stage in Core and map | **PASS** | UI-01/02/06: displayed tree-ID set == Core ID set (162 and 2,000), inspector fields == API |
| AC-03 identical canonical bytes / hash | **PASS** | fixtures regenerated by a **separate process** are byte-identical; two fresh sessions serialise to the checked-in bytes; clean save→load→re-save byte-identical; stale-on-load hash difference verified separately (UI-10, Core test). Only the pinned runtime 8.0.33 on one host was exercised |
| AC-04 exact BA/N/area/Dq, dead/stump/duplicate, non-square, boundary, outside, bow-tie, below-threshold | **PASS** | Oracle: BA 0.227765 m², BA/ha 0.911062, N/ha 12, Dq 31.0913 cm, 40 cm removal 0.125664 m²; L-shape 7,500 m², demo 5,812.5 m² vs independent shoelace; 13 invalid-ring cases |
| AC-05 mark→queue change nothing; execute fells all and only | **PASS** | `MarkProposeApproveQueue_ChangeNothingBiological`, `Execute_FellsAllAndOnlyTheListedTrees_Once`, UI-06 |
| AC-06 fail-closed matrix | **PASS** | repeated, stale revision/epoch, dead, felled, below-threshold, duplicate, missing, unreserved, malformed/self-crossing polygon, bad units, overflow/NaN, wrong-format, outside path — each with zero partial felling and one audit-only event where applicable |
| AC-07 versioned isolated saves | **PASS** | format/schema/sourceClass header, `.lab-data` only, symlink/traversal rejected before open (proved with a FIFO), CCF-shaped JSON rejected at the header gate with no write |
| AC-08 UI plotting/selection/Core-only metrics/separate approve-queue-execute/replay/non-colour labels | **PASS** (headless) | UI suite + screenshots; `Browser_CodeContainsNoForestryArithmetic` |
| AC-09 labels; no growth; step is calendar-only | **PASS** | banners; UI-09 and fuzz assert age/DBH/height/position never change |
| AC-10 no real data / replica / extrapolation / Unity | **PASS** | source scan test; README |
| AC-11 negatives, perf, ownership, deps/licences, docs; independent review | **PARTIAL** | all documented here; **independent verdict NOT YET GIVEN** |
| REQ-U09 2,000 trees ≤ 2 s | **PASS** | §5 |

## 4. How it behaves (actual mechanics)

* **One authority:** a `ForestSnapshot` of trees (integer mm), polygon, plans; the map draws a projection of it. Eligible adult = living and `dbhMm >= adultThresholdMm`; below-threshold stems are displayed and inspectable but never counted or actionable.
* **Marks, plans, work:** tree `UNMARKED/PLAYER_MARKED` is separate from plan state (`DRAFT…STALE`). A tree is in at most one open plan; plan ID lists are never edited (cancel/reject, create a new plan ID). Execution validates the whole plan first; any failure fells nothing and appends one **audit-only** `PlanExecutionRejected` event (pre/post hash equal). An unknown plan ID returns an error with no event.
* **Events:** every snapshot-changing action appends exactly one event; staleness cascades are recorded as separate `PlanStale` events; `eventSeq` is gap-free; `prev/postStateHash` chain over canonical snapshot bytes; replay re-derives every hash. Approval epochs come from the event payload on replay, never the live runtime epoch. Load/reset starts a fresh opaque epoch (128 random bits) and stales approved/pending plans by recorded events.
* **UI label:** the calendar button reads exactly the spec A6 text *"Advance experiment step (calendar only — NO BIOLOGICAL GROWTH)"* (the dispatch's shorter wording is a prefix of it).
* **Screenshots** (real headless Chromium, off-repo): `…/browser/desktop-01…10`, `narrow-390x844-selected.png`, `narrow-800x900.png` (see §7).

## 5. Verification actually run (commands and results)

All on `jer-laptop`, .NET SDK 8.0.133 / runtime 8.0.33, Chromium 156.0.8078.79 (headless=new), Node v26.13.1. Run from `Tools/MultiRackSitkaLab` on a clean build of commit `2a76260e`.

| Gate | Command | Result |
|---|---|---|
| Clean build, warnings = errors | `dotnet build MultiRackSitkaLab.sln` | 0 warnings, 0 errors |
| Tier 1 Core | `dotnet run --no-build --project tests/Core.Tests` | **55 passed, 0 failed** |
| Tier 1 Host (real loopback listener, temp lab root) | `dotnet run --no-build --project tests/Host.Tests` | **8 passed, 0 failed** |
| Fixture integrity | `sha256sum -c MANIFEST.sha256`; FixtureTool regenerated into a scratch dir and `cmp`'d | all OK, all IDENTICAL |
| Schema | `python3 -I -B tests/check-schema.py …` on 3 fixtures **and** a real saved history file (13 events) | all OK, 6 negative controls rejected (custom validator, no pip package) |
| Mutation / negative controls | `python3 -I -B tests/run-mutation-controls.py <scratch> <json>` (on a scratch copy) | run 1: **16 of 17 CAUGHT**; **M17 survived** (queue accepted from any state — a second guard hid it for most states but a PENDING_WORK plan could be re-queued). I added the missing assertions (queue twice / queue cancelled / queue executed) and re-ran M17 alone: **CAUGHT**. Both logs are kept; I did not re-run the other 16 after the test edit. |
| Tier 2 browser | `EVIDENCE_DIR=… node tests/Ui/run-browser-tests.mjs` | **15 passed, 0 failed** |

**Performance (REQ-U09), 2,000-tree fixture**, real mouse clicks in headless Chromium, wall-clock from click to inspector + Core preview rendered: **21 samples, max 169 ms, mean 145 ms**, in-page max 88 ms — budget 2,000 ms. Host: Intel i5-6300HQ 4 cores / 15 GB / Linux 7.0.0-42 / .NET SDK 8.0.133 / Chromium 156. Core preview + full view build for 2,000 trees ≈ 1 ms.

**Security results:** only `127.0.0.1:<port>` listening (checked with `ss` and `/proc/net/tcp`); foreign/`localhost` Host, foreign Origin, missing Origin/lab header on POST, wrong content type, cross-site fetch metadata, OPTIONS preflight, PUT/DELETE/PATCH/TRACE, static and API path traversal, absolute and `../` save names, 1.2 MB body — all refused with the expected status and **no CORS allow header**; the page made no non-loopback requests; a CCF-save look-alike next to the tool root was never read or modified; `.lab-data` stayed empty after every hostile request.

## 6. Dependencies, licences, isolation

* **No third-party packages** (no NuGet, npm, CDN). .NET 8 SDK/runtime and in-box `Microsoft.AspNetCore.App` (MIT). UI is hand-written HTML/CSS/JS. Test drivers use Node built-ins and Python 3 stdlib only. SDK pinned by `global.json` (8.0.133, `latestPatch`).
* **Root `.gitignore` hides `*.sln`/`*.csproj`** (Unity-generated). The lab's own `.gitignore` re-includes them (`!*.sln`, `!*.csproj`); `git add -n` shows all 7 project files are tracked. No root file was edited.
* **Saved-game isolation:** no CCF `ForestSaveData`, Unity save, Reference Future or `Assets/` path appears in production source (scan test); the host only writes `Tools/MultiRackSitkaLab/.lab-data/`. The browser run wrote and then deleted only its own `browser-test-<timestamp>` file; `.lab-data` is empty and gitignored.
* **No ecological claim:** every screen says SYNTHETIC TEST FOREST / NO BIOLOGICAL GROWTH and "not a measured Irish stand"; fixtures are index-driven arithmetic patterns, no PRNG, no allometry or stocking meaning.

## 7. Evidence (off-repo, referenced not committed)

`/media/jer/Files/CCF/Verification/MULTI-002-SitkaLab-P0-20261114/` — `tier1/` (build, core/host logs, schema, fixture regeneration, manifest, `mutation/`), `browser/` (`browser-results.json`, run log, 12 PNG screenshots, host console), `sample-save/` (real saved history + host log), `env/tooling-versions.txt`, `scope-proof-source.txt`, `sha256sums.txt` (29 files; SHA-256 of that manifest `d459cffb8637f3ed7f08a85c8cc538c8645b0e534d66d441702f8ca088872c31`).

## 8. Known limitations and honest caveats

1. **Headless, not headed.** Tier 2 used Chromium `--headless=new` driven through DevTools input events (real mouse, wheel, drag; not `element.click()`). The native `<select>` popup cannot be opened that way, so the fixture/lab-file dropdown value was set by script before clicking the real buttons. Firefox, Windows and WSL were not run.
2. **Fixtures are formula-generated.** Only the 3-tree oracle is genuinely hand-computed. The 162- and 2,000-tree files come from `tools/FixtureTool` (index arithmetic); their "audit" is independent recomputation in tests plus pinned hashes, not manual inspection of each stem.
3. **Atomic-rejection tests for dead/duplicate/below-threshold/unreserved/revision/epoch** need an unreachable corrupt plan, so they use an `internal` test seam (`ForTestingUnvalidated`) that injects a deliberately corrupt in-memory state. Through the public API those states cannot arise.
4. **Design choices a reviewer should confirm:** `PlanStale` cascades are separate events; audit events are written only for known plan IDs; a failed mark/plan request appends no event; `Execute` buttons stay clickable on invalid states (greyed/dashed) so the server's refusal and audit event are demonstrable; numeric-safety bounds (|coordinate|, DBH, height ≤ 1,000,000,000 mm) are overflow guards, not biological limits.
5. JSON Schema checked only with a small custom validator; ordering/hash rules live in the markdown and the reader.
6. The mutation re-run after adding tests covered M17 only.
7. SVG redraws every glyph per state change — fine at 2,000, not designed for tens of thousands.
8. Byte-identity was demonstrated only for runtime 8.0.33 on one host.
9. Dashboard/Drive sync: **NOT VERIFIED**.

## 9. Independent review request (new claim needed)

The writer has stopped. Please open a **new claim** for an independent reviewer (Claude Opus or a separate high-reasoning Codex — not this worker) with verdict **APPROVED / NEEDS CHANGES / REJECTED** for **engineering P0 only**. Suggested focus: Engine guards and event/replay semantics; path-safety and header-gate ordering; Host guard completeness; UI/Core parity and screenshots; a headed or second-browser run; the §8 design choices; whether the evidence supports AC-01…AC-11. Implementation existence ≠ integration into Scenario One, a verified ecological model, or user acceptance.

Reviewer quick start: `cd Tools/MultiRackSitkaLab && dotnet build MultiRackSitkaLab.sln && dotnet run --project tests/Core.Tests && dotnet run --project tests/Host.Tests && node tests/Ui/run-browser-tests.mjs` (UI run needs Chromium; set `EVIDENCE_DIR`).

## Appendix A — test inventory (all PASS in the final run)

**Core (55):** FileFormatTests.Fixtures_ArePinnedCanonicalAndMatchManifest; FileFormatTests.Fixtures_AreSyntheticAndNeverClaimMeasuredData; FileFormatTests.HeaderGate_RejectsCcfShapedAndWrongFormatContent_BeforeBodyParsing; FileFormatTests.LoadingTheSameFixtureTwice_GivesIdenticalCanonicalBytesAndMetrics; FileFormatTests.Parse_RejectsBadIdsDuplicatesSpeciesAndOutOfPolygonStems; FileFormatTests.Parse_RejectsBowTieAndOtherInvalidPolygons; FileFormatTests.Parse_RejectsNonIntegerOverflowAndNonPositiveMeasurements; FileFormatTests.Parse_RejectsSchemaDriftDuplicateKeysAndNonCanonicalBytes; LabDataStoreTests.CcfShapedJsonInsideTheLabRoot_FailsAtTheHeaderGate_WithoutWriting; LabDataStoreTests.PathEscapes_AreRejectedByNameShape_BeforeAnyFileSystemAccess; LabDataStoreTests.ProductionCcfSavePathsAreNeverReferencedByTheStore; LabDataStoreTests.SymlinkAppearingAfterListing_IsStillRefusedBeforeOpen; LabDataStoreTests.SymlinkToOutsideFile_IsRefusedBeforeOpen_ProvenWithAFifo; LabDataStoreTests.SymlinkedLabRoot_IsRefused; LabDataStoreTests.UnlistedNames_AreRefused_AndSavesRoundTripThroughTheStore; MetricsAndGeometryTests.Area_IsExactAndIndependentOfWinding; MetricsAndGeometryTests.Area_SkewedAndConcavePolygons_MatchIndependentShoelace; MetricsAndGeometryTests.Boundary_StemOnEdgeAndVertexIsInside_OutsideIsNot; MetricsAndGeometryTests.Dq_IsQuadraticMean_NotArithmeticMean; MetricsAndGeometryTests.ExtremeCoordinates_DoNotOverflow; MetricsAndGeometryTests.InvalidRings_AreRejected; MetricsAndGeometryTests.Metrics_ExcludeDeadFelledAndBelowThreshold_ButCountThemSeparately; MetricsAndGeometryTests.Oracle_BasalAreaDensityDq_MatchHandCalculation; MetricsAndGeometryTests.Oracle_RemovingTheFortyCmStem_BaRemovalMatchesHand; MetricsAndGeometryTests.Preview_AllSubsets_EqualSumOfStems; MetricsAndGeometryTests.Preview_NeverMutatesTheForest; MetricsAndGeometryTests.Preview_RejectsDeadFelledBelowThresholdDuplicateAndMissing; MetricsAndGeometryTests.ZeroEligibleAdults_GivesDefinedNAndNotApplicableDq; OperationFuzzTests.Core_StaysFastOn2000Trees; OperationFuzzTests.RandomOperationSequences_KeepEveryInvariant; PersistenceAndReplayTests.CleanSaveLoad_ReproducesStateHashEventsAndBytes; PersistenceAndReplayTests.EventChain_IsMonotonicHashLinkedAndReplayable; PersistenceAndReplayTests.FixtureReset_StartsFreshEpochAndDropsAllInMemoryPlans; PersistenceAndReplayTests.Load_StalesApprovedAndPendingPlansByRecordedEvent_ChangingTheHash; PersistenceAndReplayTests.ReplayUsesHistoricalApprovalEpoch_NotTheLiveRuntimeEpoch; PersistenceAndReplayTests.Replay_DetectsTamperedHashesSequenceAndHistory; PersistenceAndReplayTests.SaveFile_CarriesFormatMarkerSchemaAndSyntheticSourceClass; StateMachineTests.Atomic_DuplicateIdInPlan_RejectsTheWholePlan; StateMachineTests.Atomic_ForestRevisionMismatch_RejectsTheWholePlan; StateMachineTests.Atomic_OneAlreadyFelledTree_RejectsTheWholePlan; StateMachineTests.Atomic_OneBelowThresholdTree_RejectsTheWholePlan; StateMachineTests.Atomic_OneDeadTree_RejectsTheWholePlan; StateMachineTests.Atomic_OneMissingTree_RejectsTheWholePlan; StateMachineTests.Atomic_SessionEpochMismatch_RejectsTheWholePlan; StateMachineTests.Atomic_TreeNotReservedToThePlan_RejectsTheWholePlan; StateMachineTests.Atomic_ValidCorruptionFreePlan_ExecutesAllFour; StateMachineTests.CalendarStep_StalesApprovedAndPendingPlans_AndNeverGrowsAnything; StateMachineTests.Execute_FellsAllAndOnlyTheListedTrees_Once; StateMachineTests.ExecutingOnePlan_StalesOtherApprovedPlans; StateMachineTests.IllegalTransitions_AreRejectedAndAppendNoStateChangingEvent; StateMachineTests.MarkProposeApproveQueue_ChangeNothingBiological; StateMachineTests.MarkStateIsSeparateFromBiologicalStatusAndPlanState; StateMachineTests.OneOpenPlanPerTree_AndPlansAreNeverEditedInPlace; StateMachineTests.RejectedProposal_ReleasesTreesAndChangesNothing; StateMachineTests.RepeatedExecution_IsRejectedAuditOnly_AndFellsNothingMore.

**Host (8):** HostTests.Browser_CodeContainsNoForestryArithmetic; HostTests.Host_ListensOnLoopbackOnly; HostTests.Host_SourceNeverReferencesProductionSavesUnityOrOutboundNetwork; HostTests.Hostile_HostOriginMethodAndHeaderCases_AreRefused; HostTests.HttpWorkflow_MarkToExecute_MatchesCoreAndIsAtomic; HostTests.RequestBodies_AreBoundedAndValidated; HostTests.Responses_CannotReadUnrelatedFiles_AndNoEndpointTouchesProductionSaves; HostTests.Saves_RoundTripOverHttp_AndWrongFormatContentFailsAtTheHeaderGate.

**Browser:**

| ID | Check | Verdict |
|---|---|---|
| UI-01 | honest labels, Core/UI identity parity of forest, tree IDs and metrics | **PASS** |
| UI-02 | real mouse click selects a real tree; inspector equals Core | **PASS** |
| UI-03 | multi-select (shift-click) shows Core preview identical to the API and changes nothing | **PASS** |
| UI-04 | metre-scale pan/zoom: wheel zoom, drag pan, fit, scale bar and cursor readout | **PASS** |
| UI-05 | explicit tree-ID selection; below-threshold and dead stems are displayed, inspectable and refused for work | **PASS** |
| UI-06 | mark → plan → propose → approve → queue change nothing; execute fells all and only; Core/UI parity at every step | **PASS** |
| UI-07 | duplicate execution is refused: audit-only event, nothing more felled | **PASS** |
| UI-08 | reject and cancel paths release trees; refused transitions explain themselves | **PASS** |
| UI-09 | calendar step makes approved/pending plans STALE, grows nothing, and a stale plan cannot execute | **PASS** |
| UI-10 | save → reset (confirmation) → load → replay verified; approved plan becomes STALE on load | **PASS** |
| UI-11 | export provenance manifest + event log is well-formed and matches Core | **PASS** |
| UI-12 | constrained viewport: stacked layout, no horizontal overflow, controls reachable | **PASS** |
| UI-13 | 2,000-tree fixture: every ID displayed; selection + inspector + Core preview ≤ 2 s | **PASS** |
| UI-14 | security: only 127.0.0.1 is listening; hostile Host/Origin/method/CORS requests refused; no external requests | **PASS** |
| UI-15 | no unexpected console errors or exceptions in the page | **PASS** |

**Mutation controls, run 1 (before the M17 test addition):**

| ID | Safeguard broken | Verdict | Failing tests |
|---|---|---|---|
| M01 | execute no longer rejects a dead tree in the plan | CAUGHT | 2 |
| M02 | basal-area formula wrong by a factor of 4 | CAUGHT | 4 |
| M03 | boundary-on-edge stems no longer count as inside | CAUGHT | 2 |
| M04 | execution silently skips one planned tree (partial felling) | CAUGHT | 6 |
| M05 | calendar step no longer stales approved/pending plans | CAUGHT | 1 |
| M06 | header gate removed: wrong-format content reaches the body parser | CAUGHT | 2 |
| M07 | path-shape check removed from ReadSave | CAUGHT | 1 |
| M08 | calendar step quietly grows DBH | CAUGHT | 2 |
| M09 | out-of-polygon stems accepted | CAUGHT | 1 |
| M10 | replay stops verifying post-state hashes | CAUGHT | 1 |
| M11 | execution ignores a forest-revision mismatch | CAUGHT | 1 |
| M12 | execution ignores a session-epoch mismatch | CAUGHT | 2 |
| M13 | bow-tie / self-intersecting rings accepted | CAUGHT | 1 |
| M14 | Host-header check removed | CAUGHT | 1 |
| M15 | Origin check removed | CAUGHT | 1 |
| M16 | listener bound to all interfaces instead of 127.0.0.1 | CAUGHT | 1 |
| M17 | queue accepted from any plan state | SURVIVED | 0 |

M17 re-run after adding the assertions: **CAUGHT** (1 failing).
