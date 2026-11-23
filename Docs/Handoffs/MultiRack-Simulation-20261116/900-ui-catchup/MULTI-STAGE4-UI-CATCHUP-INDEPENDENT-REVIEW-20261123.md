# MULTI Stage 4 UI catch-up independent review - 2026-11-23

**Verdict: NEEDS CHANGES (G2 acceptance-artifact discrepancy only).**
**G3 harness change: PASS. No blocking or major gameplay/source defect found.**
The candidate is suitable for a bounded owner playtest, but this review does not waive its literal AC-9 or authorise integration.

## Identity and custody

| Item | Observed value |
|---|---|
| Task | multi-stage4-ui-catchup-20261123 |
| Candidate reviewed | 4d7267d076603833b1b12fff676c6d3161973f39 |
| Source/test head | 541bb898 (candidate adds documentation, browser driver and evidence) |
| Base | 500a50aea4bd049f231f05d73828fabdc0a7a85c |
| Admission | d3a9f20f |
| Implemented branch reviewed | feat/multi-stage4-ui-catchup-20261123-v1 |
| Review branch | docs/multi-stage4-ui-catchup-review-20261123-v1 |
| Only writable checkout | /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-Review-20261123 |
| Reviewer | OpenAI gpt-6.1-sol, high reasoning effort, observed T3 Code/Codex harness; separate context from the Claude author |
| Routing | review decision d-900-ui-catchup-review-1; attempt att-20261123-03 |
| Candidate-local context-source commit observed | 62e39ec621e38a84030c08b35a171e297056c9ed |
| Initial/final pre-report status | clean at candidate; no unrelated tracked/untracked changes |
| Permitted tracked write | This new review Markdown only |
| Independence | No author implementation context adopted as proof; no subagents used |

Repository-relative evidence prefixes below:
- **P** = `Tools/MultiRackSitkaP1Lab`.
- **H** = `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup`.
- **S** = `specs/multi-stage4-ui-catchup-v1`.

I read AgentWorkflow, the project context documents, specs/README, the quartet, admission, implementation handoff, changed source/tests and browser evidence. I obtained the live GitHub active-tasks content with `gh api repos/jermur788/CCF/contents/Docs/Project/coordination/active-tasks.md`; the supplied task/admission remains this review's narrower grant. No coordination file was edited.

**Authority limit:** the original owner packet's complete section 3 A/B/C is not in the candidate's catch-up directory and was not supplied verbatim. I checked the four locked decisions in the review request and the A=test/B=readout+API/C=page deliverables restated by the quartet/handoff. Exact word-for-word comparison with the original packet, including the claimed oversized-image exception, is NOT TESTED. I requested its location while continuing independent work. The two explicitly open owner choices (prune heights and dead-tree drawing) are not defects.

## Checks actually run

Both commands below completed with **0 warnings, 0 errors**, using /home/jer/.dotnet/dotnet 8.0.134:

```text
cd <review-lane>/Tools/MultiRackSitkaP1Lab
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers -p:P1Root=<implementer-lane>/Tools/MultiRackSitkaP1Lab/
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers -p:P1Root=<implementer-lane>/Tools/MultiRackSitkaP1Lab/
```

Here `<implementer-lane>` is /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-20261123. The review lane has no ignored .p0ref directory. This global property is used only for the two P0 reference HintPaths (P/src/Core/SitkaP1.Core.csproj:8; P/tests/Core.Tests/SitkaP1.Core.Tests.csproj:10). All projects, linked Assets source and outputs remained in the review lane. The referenced binary was read only, its normal MSBuild hash guard remained enabled, and its hash matched before and after. No P0 rebuild or copy occurred.

To respect the bin/obj-only generated-write boundary, I did **not** run the top-level Core Program, which creates and deletes fixture-security scratch directories outside the repository (P/tests/Core.Tests/Program.cs:16,452). Instead, a small reflection launcher compiled from stdin into `P/tests/Core.Tests/obj/review-selection.dll` registered the existing, unmodified Model0FingerprintTests, MultiAreaTests, CrownC2Tests, CanopyLightTests, Stage4Tests and Stage4UiCatchupTests, then invoked the existing T.RunAll. These registrations were inspected for writes first. **76/76 passed, 0 failed**, including U-01 through U-04. This is NOT a claim to have independently rerun all 184 tests. The generated output log remains in `P/tests/Core.Tests/obj/review-selection.log`; the material results are copied here so the handoff does not depend on an ignored file.

Observed output:

```text
model0 single-area fingerprint 7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece
model0 multi-area fingerprint a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d
model1 multi-area fingerprint 814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287
S4 stage4 run fingerprint 8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb
MA-F01 manifest 67491 bytes, content hash c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95
76/76 passed, 0 failed
P0 DLL sha256 before/after: 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
```

I also started the reviewer-built Ui DLL twice, using the committed fixture directory read only. Both hosts used the existing Program's OS-assigned loopback port, never the three protected ports; each was stopped by its own subprocess handle with SIGINT. Both exited 0, stderr empty, with "Session ended; nothing was saved." No browser was started.

An in-memory Python HTTP probe observed:

```text
PASS initial view: 343443 bytes, 269.9 ms cold read, models 1/1/1
PASS prune/add AND prune/remove: 400 for unknown, duplicate, null, missing,
     65-character ID and whitespace ID bodies
PASS Pending changes no trees or canopy
PASS old plan-state fence: 409 PlayerPlanStateStale, current unchanged view
PASS remove pending Prune, then add again
PASS Approve: zero tree/ground-cell changes
PASS Execute: radius 3.3890924 -> 3.1179652, year 0; light/ground refreshed
PASS identical replay: receipt replayed, same view, exactly one prune record
PASS altered replay: 409 WorkExecutionKeyConflict
PASS second immediate lift: 409 PruneIntervalTooShort, unchanged fresh view
```

For J01 I streamed the base page and base DOM witness into node, keeping everything in memory and using the fresh candidate host only as the JSON provider. This is a **base-page diagnostic, not a full base-engine smoke run**:
- Original stand-in: 0 table rows; `TypeError: line.cloneNode is not a function` at drawGrid.
- With only the candidate cloneNode method inserted into that in-memory stand-in: 171 rows; no page errors.
- Selecting an A tree on that base page: old competitor buttons 0, current draft checkboxes 5, reason-choice absent, inspector area "Area A".

## Required check matrix

PASS means the bounded claim stated in that row is supported; NOT TESTED is not inferred from the author's count.

| Check | Status | Evidence / limits |
|---|---|---|
| 1a. Locked decision (a): /multi 1/1/1, union canopy, 2.5 m decoupled light; legacy retained | PASS | P/src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs:27; MultiAreaUiReadout.cs:122,152; U-02 and direct API initial view. |
| 1b. Decision (b): natural-death logs, non-colour shape/legend/counts; D distinct from X | PASS | P/src/Ui/PlayerMultiApi.cs:2158,2317; H/evidence/d-thinning-gaps-deadwood-1280x720.png; H/browser-driver/browser-results-20261123.json:272. 17 logs and 3 deaths this year are shown. Drawing both D and a log is the declared open choice. |
| 1c. Decision (c): no twin; both sizes, no sideways scrolling | PASS (inspected author evidence) | Driver scenario.mjs:48 checks document AND body width, dock intersection and stand bounds for 16 states; results JSON:7 onward. Five committed PNGs visually inspected. Also found all five external 1920x1080 images at declared /tmp paths, checked dimensions and byte lengths, visually inspected baseline/light/deadwood. No independent browser/layout rerun. |
| 1d. Decision (d): authoritative display | PASS | See check 3. |
| 1e. Section 3 A/B/C as restated | PASS | A: U-01, lines 21-77. B: MultiAreaUiReadout.cs:29-111; PlayerMultiApi.cs:84-126. C: rack cards, Prune, light inspector and logs in page and evidence. No Stage 5, twin, persistence or package scope added. |
| 1f. Quartet status and authority | PASS / NOT TESTED | All four declare APPROVED; spec.md:15,77 states subordinate task authority, matching specs/README. Commit 18518b97 precedes source edits. Original owner-packet wording/approval provenance beyond the supplied instruction is NOT TESTED. |
| 1g. Literal spec acceptance completeness | FAIL | S/spec.md:105 requires committed images at both resolutions; only five 1280x720 PNGs are tracked. H implementation handoff:174-183,210 honestly declares the exception. F-1 below. AC-2's full 184 count is author-reported, not independently repeated. |
| 2a. Core default ctor/model-0 paths and Stage 1-3 | PASS | MultiAreaContinuation.cs:95 ctor unchanged; diff is new read/cache/scheduled-Prune code plus extraction of the unchanged Stage4ViewOf body. Annual/Execute model-0 algorithms untouched. U-02, MA tests, F-01/F-02, C2 and S4 V-01/V-02 passed. Four fingerprints reproduced above. |
| 2b. Shared evaluator and single-area isolation | PASS (source + selected runtime) | CanopyLightEvaluator diff is one 70-line insertion of internal helpers at line 194; existing Evaluate/Kernel/Cover/Light bodies unchanged. `git diff 500a50ae HEAD -- P/src/Ui/PlayerApi.cs P/src/Ui/Program.cs P/src/Ui/wwwroot` empty (with P expanded). Single-area F-01 passes. Full single-area smoke NOT TESTED. |
| 3. D-020 JavaScript audit | PASS | Only percentage formatting (PlayerMultiApi.cs:1241), display bands (1233), screen geometry (2176), and engine-field formatting (1647,2220,2453) were found. No cover union, light kernel, RD, biological crown-radius or pruning-height/interval arithmetic in page JS. pruneAvailability:2468 gates on engine pruneEligible plus existing plan state; it does not calculate biological eligibility. |
| 3b. Suspicious-looking arithmetic classified | PASS | FOCUS_SCALE and displacement/separation maths at 1742-1781 are pre-existing display-only code, byte-identical in the compared function block. canopyHash/noise and r scaling are artwork. New max/min at 2177 sizes band digits. None supplies engine values or changes hit-test coordinates. |
| 4a. Additive API shape | PASS | Existing payload members retained, new Stage4 appended at PlayerMultiApi.cs:209-221. New labels are additive; manifest hash unchanged. Guard errors and manifest/assets retain their old shapes; "every response" means the four view-bearing payload families, not static/guard responses. |
| 4b. Parsing, guards, revisions, 409/fresh view | PASS | Routes:84-87 reuse MultiTreeMembers and ReadMultiRequestAsync:134; unknown members refused by unchanged PlayerApi.cs:47, duplicates by RequireUniqueMembers, null/missing by :168, IDs by :180. Both routes directly probed for malformed cases; stale fence directly probed. Host/Origin/body-size guard code unchanged; new-route exhaustive hostile HTTP matrix NOT TESTED. |
| 4c. Pending/Approve/Execute/replay/cache | PASS | Direct probes above; continuation:419-439,474-529,569-575,608-618. Pending only changes plan state; Approve cuts nothing; work revision invalidates caches; replay precedes stale-source refusal intentionally and does not prune twice. U-04 passes all three scheduled lifts. |
| 4d. Payload bloat | PASS (functional); NOT TESTED (performance budget) | Measured genesis response 343443 bytes; cold API read 269.9 ms once. It parses/returns successfully, and the 16 KiB constant is a REQUEST limit, not a response limit (PlayerApi.cs:25). Every view-bearing command/diagnostic duplicates the full readout, so this is real bandwidth/allocation overhead. No proven break or stipulated response budget exceeded; no latency distribution/load test. |
| 5. Atomic-refusal test / deliverable A | PASS | Production path through adapter.Execute -> continuation.Execute:519-529, not a mocked guard. U-01:35-66 injects drift in second Prune, then checks all trees, valid first Prune, cross-area Fell, root identity, revisions/events, approval and retry; successful undrifted control :69-77. 76-case run passed it. Reflection is justified for otherwise unreachable defence-in-depth states, not tautological. Details below. |
| 6a. 527a4c02 measurement integrity | PASS | Diff adds cloneNode; changes disclosure expectation from legacy edge exclusion to owner-required connected woodland and adds absence of old text. No check call removed/skipped; no exception swallowed or exit/threshold weakened. MA-W12 remains an exact-member-list assertion with four additive members. |
| 6b. J01 cause and stale J04/J05/J09/J10/J11 | PASS | Base-page replay above demonstrates cloneNode cause and stale selectors. Both git-show base and HEAD contain zero data-competitor and reason-choice occurrences, four data-draft occurrences, one cloneNode call. Old witness lines 656,676,680,696,698 demand the missing buttons or "MA-B"; both pages assign areaName, which renders "Area B". These particular assertions cannot pass on the base page either. |
| 6c. J99 and reported whole-suite totals | PASS (cause); NOT TESTED (full counts) | Shell :783 requires witness exit 0 and empty stderr. Witness :700 dereferences missing reason-choice after timeouts; it cannot finish, so J99 remains FAIL. Base lacked cloneNode and failed earlier; fixing J01 changes which stale checks are reached. No full 107/2 vs 110/6 comparative shell run here; those totals remain author-reported, not green gates. |
| 7a. 73/73 browser evidence honesty | PASS | Parsed JSON: exactly 73 PASS entries, zero FAIL; every result line present in browser-run text. Driver uses real CDP mouse events (cdp.mjs:101), checks server state and records errors, rather than a fake browser. This validates the artifacts' content, not an independent re-execution or cryptographic candidate binding. |
| 7b. Image/publication/token claims | PASS, except AC-9 | Five tracked PNGs: all 1280x720, total 1283847 bytes, exact logged sizes. Five external PNGs: all 1920x1080, exact logged sizes 537973-844777 bytes. H handoff:183 explicitly says external/uncommitted. No literal /s/<64-hex> capability found in any catch-up document/script/PNG bytes; no address bar/token visible in inspected images. Drivers read launch URL from uncommitted scratch and redact it. |
| 8a. Protected tracked boundaries | PASS | `git diff --name-status 500a50ae HEAD`: 26 paths, only granted Core/PlayerMultiApi/tests/spec/handoff paths; no deletions, Assets, Unity files, save/persistence, packages, PlayerApi.cs or Program.cs. Candidate log has no merge commit. Reviewer work left tracked source unchanged. |
| 8b. Main merge/history/second writer | PASS (visible history); NOT TESTED (historical process actions) | Live candidate branch points to requested SHA; live main observed 3a67d0987be0cf40d007892739d2ad295c84f0e0. Local origin/main does not contain candidate. Admission and visible candidate commits support sole source writer. Git trees cannot prove no past force-push or unseen physical writer; no such action performed by reviewer. No worktree created. |
| 9. Misleading owner playtest states | PASS for normal path; minor issues | Correct light/synthetic caveats, D/X/log and scissors/numbers are present. Local reclosure versus falling aggregate cover is explicitly disclosed (handoff:168). No hidden normal-path 5xx observed in evidence or reviewer API probes. Null-readout fallback and oversized text halo are F-2/F-3; tiny 1280 map/status overlap remains disclosed. |

## Atomic-refusal test assessment

**Deliverable A is genuinely met for its stated invariant.** The reflection setup changes input state; it does not replace Execute or force a return value. Keeping the source/revision unchanged deliberately bypasses earlier stale-source protection to reach the actual production PruningRules.Require call. Max lifts and interval drift reach that guard; non-living drift reaches the production RequireLivingInArea guard immediately before it.

The failing Prune is the second of two; a valid Prune and a Fell in the other area are already approved. Checks compare all tree records/radius bits, the first tree's lift count, no prune-log entries, no work event, unchanged token/year/plan-state, and unchanged published root. That is materially stronger than checking the failing tree alone. The successful control prevents an always-refusing implementation from satisfying the test.

It is coupled to the private root name/shape. A rename/missing field will fail loudly, as will an earlier stale-source refusal because the expected specific guard code would differ. This is not a silent no-op test. It is not a public-command race scenario, does not cover every conceivable eligibility guard (height guard is not injected), and is not a mutation-testing proof against arbitrary future in-place corruption. Those limits do not invalidate the supplied requirement.

## Findings by severity

### F-1 - MINOR, required before final G2 acceptance: screenshot criterion and recorded exception disagree

**Evidence:** S/spec.md:105; S/tasks.md:87; H/MULTI-STAGE4-UI-CATCHUP-IMPLEMENTATION-RESULT-20261123.md:174-183,210.

AC-9 requires committed images covering both resolutions, each approximately <=400 KB and the set <=3 MB. The actual tracked set is five 1280x720 images. The 1920x1080 files exist and match the recorded dimensions/byte lengths, but remain at temporary external paths, all above the per-image bound. The handoff is candid about this, so this is **not fabrication or a demonstrated rendering failure**. Nevertheless the reviewer cannot mark the literal approved criterion PASS or silently approve an exception. T-07 is marked DONE despite the mismatch.

**Required correction:** fulfil AC-9, or have the authorised spec owner reconcile AC-9/T-07 with the original packet's external-evidence exception and identify a retained external archive. No gameplay change or broad retesting is requested. A documentation correction is sufficient if the claimed owner exception is confirmed. This is the sole reason for the overall NEEDS CHANGES verdict.

### F-2 - MINOR, nonblocking: unavailable readout is treated as legacy/empty Prunes

**Evidence:** P/src/Ui/PlayerMultiApi.cs:117-126,1230-1232,1550-1555,1625,2993-3005,3294-3309.

The API intentionally returns stage4:null when the root moved. The rack card says "read-out unavailable", but isStage4 then returns false: the disclosure/details revert to model-0 claims ("edge neighbours excluded", "Light ... NOT MODELLED"). approvedPrunes returns [], and samePrunes accepts an empty frozen list with no readout, despite its "cannot confirm" comment. A prune-only approved view can consequently offer an empty "Confirm felling" dialog until re-read.

**Why it matters:** an explicitly handled concurrency state produces false model/plan presentation. **It does not permit unseen ecological execution** in the current server: ReadoutFor is null only after a monotonic revision changed, making the response's fence stale; Execute therefore refuses. No production race was dynamically forced here; this conclusion is from the complete source path, not a claimed browser reproduction.

**Suggested correction:** represent unavailable separately from legacy, keep model provenance honest, and disable/withdraw Execute until a paired snapshot is re-read.

### F-3 - MINOR, nonblocking visual issue: band-number halo scales with map metres

**Evidence:** P/src/Ui/PlayerMultiApi.cs:937,2175-2184; external /tmp/ui900/evidence/b-light-guide-ground-1920x1080.png; committed H/evidence/b-light-guide-ground-1280x720.png.

The digit font is converted to map metres to stay 6-10 screen pixels, but its stroke-width:2px has no non-scaling-stroke. In the inspected 1920 image the dark halos become large disks covering much of each coloured light cell. At 1280 the digits are very small. Numbers remain present and correct, so this does not violate the non-colour cue requirement; it reduces the usefulness of the colour ramp.

**Suggested correction:** keep the halo at a small screen-pixel width and verify the light overlay at the two existing sizes. This is presentation only.

### F-4 - OBSERVATION: a matching Stage 4 hash is printed, not hard-pinned by R-02

**Evidence:** P/tests/Core.Tests/Stage4BehaviourTests.cs:548-553.

R-02 checks repeatability and difference from legacy/Stage3, then prints the fingerprint. It does not assert equality with 8a926776.... That test is unchanged from the base; the reviewer observed the expected hash and is not alleging drift or requesting an unauthorised harness change. Keep the external anchor comparison in the integration gate; "the suite passed" alone would not prove this specific anchor.

### F-5 - OBSERVATION: browser console snapshots differ because the intentional 409 occurs later

**Evidence:** H/browser-driver/scenario.mjs:352-373; browser-results-20261123.json:287-309.

The earlier PASS line reports 100 successful responses and zero errors. facts.console holds a reference to the mutable log, so the saved JSON later contains 101 HTTP 200 plus one intentional 409 and its network error. This is explained by the final refusal probe, not a hidden application/CSP failure. The 73-pass claim is internally consistent with that sequencing.

## Checks not run / unproven claims

- Full top-level Core 184-case suite; reviewer ran the 76 relevant existing cases instead.
- Full multi-area and single-area shell smokes, and a complete base-engine comparative run; their aggregate counts remain author claims. The base-page cloneNode/selector diagnostic is explicitly narrower.
- p0-isolation.sh, build-output-safety.sh, cli-safety.sh, build-p0-core-ref.sh.
- A fresh real Edge/CDP run, native owner playtest, independent dynamic layout/zoom matrix, or forced null-readout race.
- Unity, asset import, save/load, Windows build, package installation, external services or load/performance qualification.
- Original full owner packet verification; historical absence of force-push/second writer; dashboard publication/synchronization.
- G4 integration, a main merge, post-integration tests or owner acceptance. None is implied by this source review.

The external 1920 Prune images were checked for file size/dimensions but not visually opened; the three other 1920 states and all five committed 1280 states were visually opened. No image or driver was regenerated.

## INTEGRATION DECISION

```text
INTEGRATION DECISION
Status: NEEDS CHANGES
Gate: G2 ACCEPTANCE (AC-9 evidence/spec reconciliation outstanding);
      G3 MEASUREMENT INTEGRITY PASS for 527a4c02 and the inspected evidence;
      G4 INTEGRATION/CANON NOT PERFORMED / NOT AUTHORISED by this review.
Reviewed branch: feat/multi-stage4-ui-catchup-20261123-v1
Reviewed HEAD: 4d7267d076603833b1b12fff676c6d3161973f39
Source/test HEAD: 541bb898
Context commit: candidate-local 62e39ec621e38a84030c08b35a171e297056c9ed;
                latest direct review packet is controlling task authority.
SPEC:
- ID/status: SPEC-multi-stage4-ui-catchup-V1 / APPROVED
- Conformance: FAIL literal AC-9; functional FR-1..FR-9 supported as scoped above;
               original complete owner-packet comparison NOT TESTED.
Reasons:
- No blocking/major source or gameplay defect found.
- 76 selected existing cases and reviewer API probes pass; all five anchors match.
- Deliverable A is genuine production-guard and whole-plan atomic-refusal coverage.
- Harness alignment does not weaken assertions; stale J selectors predate candidate.
- Literal committed screenshot requirement has not been met or formally reconciled.
Required fixes / remaining gates:
- Resolve F-1 through required evidence or an authorised spec/retention correction.
- F-2/F-3 are nonblocking improvement recommendations, not invented engine blockers.
- Preserve declared smoke failures; no blanket green-smoke or full-184 reviewer claim.
- Owner retains prune-height/dead-tree drawing choices and personal acceptance.
Integration target: designated authorised Multi integration candidate, then main
                    only under its separate G4 grant; reviewer does not merge.
Integrated HEAD: none.
Post-integration verification:
- Build solution and Ui, run full Core suite and compare all four fingerprints plus
  P0 SHA before/after on the exact integrated SHA.
- Run/triage both UI smokes without weakening assertions.
- Bounded browser check of Prune Pending/Approve/Execute/refusal, local/cross-area
  light and deadwood/counters; verify both viewport layouts if presentation changes.
- Confirm protected files/manifest untouched, publish durable handoff, and complete
  Manager dashboard synchronization (not verified by this reviewer).
```

## Owner-facing summary

- No blocking or major gameplay defect found; the candidate can receive a bounded owner playtest.
- NEEDS CHANGES concerns one evidence/spec mismatch: AC-9 requires committed screenshots at both sizes.
- The external 1920 images exist, but their exception/retention needs authoritative reconciliation.
- Model 0 and Stage 1-3 fingerprints, Stage 4 fingerprint and P0 binary hash match.
- 76 selected tests and direct Prune API/replay/refusal checks passed.
- Execute atomic-refusal deliverable A is genuinely met.
- The six known multi-smoke failures reflect stale base-page witness controls; they remain failures.
- Minor UI issues: unavailable readout falls back to legacy wording; light-number halos obscure colours.
- Prune heights and standing-dead-plus-log drawing remain your open choices.
- No Unity, full 184-case rerun, fresh browser qualification, integration or owner acceptance is claimed.
