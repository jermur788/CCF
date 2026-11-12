# Claude A routing and orchestration evidence ledger — research-learning V1

**Status:** TEMPLATE / NOT STARTED. **Owner:** Claude A, one-writer, append-only. **Claim:** https://github.com/jermur788/CCF/issues/390. **V2 contract:** https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/PlayerTestReadiness-Overnight-01/ORCHESTRATION-EVIDENCE-APPENDIX-V2.md.

**Do not backfill.** An entry below is a template, not observed provider capacity, execution, a PASS or an integration result. Observations must be recorded contemporaneously as OBSERVED FACT; model-stated rationales as ORCHESTRATOR CLAIM; later interpretation as RETROSPECTIVE INFERENCE. UNKNOWN is permitted and preferred to guessed usage/quota/reset/energy.

## Starting constraints (user-provided 2026-11-12, not runtime measurements)

- Target: implement complete *approved copy-only* Research-Informed Management Learning V1 against latest UI-improved candidate from #382, preserving its worktree and old user playtest state.
- Claude A / Fable 5.2 orchestrator; GPT-6.1 Sol Medium preferred implementation through authorised Sol account pools; Claude B Sonnet 5.5 independent review, Opus only as justified authorised exception.
- Product authority and active-tasks writer: Manager #200. No overriding locks, safety, user playtest, old-save/frozen Reference or Windows build gate.
- All account quotas, reset windows, context pressure, available tool capabilities, current Unity leases and latest remote refs: **UNKNOWN until personally observed at start**.

## Required starting observation (fill at actual launch)

- Start timestamp / timezone: UNKNOWN
- Actual Claude A provider/model/effort: UNKNOWN
- Orchestrator branch/starting HEAD; one-writer proof: UNKNOWN
- Exact newest UI source candidate and registered Unity Editor lane; clean/owner proof: UNKNOWN
- Current active-tasks/main/issue status: UNKNOWN
- Capacity table per pool (Claude A; Sol account 1; Sol account 2; Claude B): state AVAILABLE/BUSY/COOLDOWN/LOGIN-UNAVAILABLE/UNKNOWN, quota source/confidence/reset, capacity-to-completion, switching costs
- Context limit/used/compactions: UNKNOWN
- Optimisation: correctness and full completion, then minimised rework, tokens/cost/time; no test weakening.
- Prepared child claims/locks and whether #382 user lane is free: UNKNOWN

## Append-only event template

### EVENT — <actual timestamp>

- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE
- **Task / parent / why now:** 
- **Actual provider, model, effort, account pool / capacity and reset source:**
- **Alternatives considered, selected route, risk, exploit/explore and justification:**
- **Context pressure/compaction / switching, retry or checkpoint cost:**
- **Claim [READY]/[RUNNING]/[BLOCKED] / branch / source HEAD / exact worktree / locked file scope / Unity lease:**
- **Inputs and approved product/source boundary:**
- **Outputs, implementation/verification, actual command/test/screenshot evidence, PASS/FAIL/NOT RUN:**
- **Handoff/report URLs and status, accepted/rejected/pending:**
- **Measured calls/tokens/wall time/resources if available; otherwise UNKNOWN:**
- **Failure class, rework, salvage and next action:**

## End-of-run summary (create only from logged events)

Count work attempted/completed/rejected/blocked, accounted actual model+pool usage and switches, token/time where measured, test failures and owner gates, independent review, exact integration and protected-identity evidence, user-required action, dashboard source update and downstream sync/readback separately. Never infer Unity PASS, accepted spec or completed work from an issue title.

---

# LIVE ENTRIES (appended by Claude A from actual start; nothing above this line is an observation)

## Starting observation — 2026-11-12 01:30 UTC

- **Evidence class:** OBSERVED FACT unless marked.
- **Start timestamp:** 2026-11-12T01:30:46+00:00 (host clock, UTC).
- **Actual Claude A provider/model/effort:** Anthropic `claude-fable-5-2`, effort `low` (as reported by the harness), Claude Code inside T3 Code, provider instance "Claude A".
- **Orchestrator checkout:** `/media/jer/Files/CCF-Primary` on `ops/workflow-foundation-v1` @ `46247486`, with two pre-existing uncommitted user edits (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`) left untouched. Ledger/report writer worktree created: `/media/jer/Files/CCF/Operations/CCF-390-Orchestration-Docs` on `docs/claude-a-fable-orchestration-research-copy-20261112-v1` @ `95eb5639`. (The handoff expected "no local orchestration worktree"; one was created because the ledger must be committed from somewhere. One writer: Claude A.)
- **Newest UI candidate:** `integration/scenario-one-personal-playtest-candidate-20261111-v1` local = remote = `b62446caf40fc2ac939d55a6bea6e201f41de700`; game source `2aaacad68ed186dbe14531f31ce5bb3a87811441`; `git diff 2aaacad6 b62446ca -- Assets Packages ProjectSettings` empty. Lineage contains #377 `c9c7d010`, #378 `ebe061ba`, #379 `2aaacad6` (post-#373 Sol repairs). Only other remote branches containing `2aaacad6` are the #379 fix branch and `qualify/scenario-one-ui-residual-tier2-20261111-v1` (= `2aaacad6`, no successor commits). No later UI successor found.
- **Registered Unity lane:** `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` on `qualify/scenario-one-tagged-text-379-20261111` @ `2aaacad6`, `git status` clean. Not opened, not modified. No process matching "Unity" running. ZX20 mounted, 25 GiB free. User #382 playtest: issue says USER NOT STARTED; whether the user intends to use the lane imminently is UNKNOWN.
- **main / active-tasks:** `origin/main` @ `20b8c81b`; no row naming #390 as running, no competing writer for the UI copy files. active-tasks.md NOT edited by Claude A (Manager #200 sole writer).
- **Capacity table:** Claude A — AVAILABLE, quota/reset UNKNOWN. Sol pool A (T3 provider "Codex", `gpt-6.1-sol`) — listed AVAILABLE by the harness, quota UNKNOWN. Sol pool B ("ChatGPT - OpenAI B", `gpt-6.1-sol`) — listed AVAILABLE, quota UNKNOWN. Claude B ("Claude B — Pro", `claude-sonnet-5-5`, `claude-opus-5-5`) — listed AVAILABLE, quota UNKNOWN.
- **Context limit/used/compactions:** UNKNOWN / UNKNOWN / 0 so far.
- **Optimisation:** correctness and full completion first, then minimal rework and cost; no test weakening.

## EVENT 1 — 2026-11-12 01:31 UTC — child claim and source lane

- **Evidence class:** OBSERVED FACT.
- **Task:** create C-01 child claim and isolated writer lane (handoff step 2).
- **Outputs:** issue [#391](https://github.com/jermur788/CCF/issues/391) [RUNNING]; worktree `/media/jer/Files/CCF/Operations/CCF-S1-ResearchCopy-V1-Sol`, branch `feature/scenario-one-research-copy-v1-20261112` @ `b62446ca`, clean. Allowed file set and protected list are in #391. #390 retitled [RUNNING] with a preflight comment.
- **Unity lease:** none taken.
- **ORCHESTRATOR CLAIM (source pre-inventory, read-only grep by Claude A):** "Areas with young trees" already ships in `ScenarioOneUiFacts.cs`; residual candidates are `AnnualReviewView.cs` "regeneration cohort(s) removed", curriculum "tallest natural seedling cohort", curriculum topic 2 "Being smaller or suppressed is not by itself a reason to remove a tree", and no existing wind-not-simulated Help sentence. `ManagementHistoryText.cs` parses lines by the prefix "Areas with young regeneration", so those history strings were placed out of bounds.

## EVENT 2 — 2026-11-12 01:33 UTC — implementation dispatch

- **Evidence class:** OBSERVED FACT (dispatch); ORCHESTRATOR CLAIM (rationale).
- **Actual provider/model/effort/pool:** OpenAI `gpt-6.1-sol`, reasoningEffort `medium`, Sol pool A (T3 provider "Codex"), default service tier. Executed model as reported by the provider: recorded at completion.
- **Alternatives considered:** pool B (identical model; held in reserve for a quota/cooldown on A); Claude A self-implementing (rejected: role separation, and it would leave no independent author for Claude B to review against). Exploit, not explore: LOW-difficulty task on the default route.
- **Claim / lane:** #391, single writer, worktree and branch as in Event 1. Unity lease: none; Unity forbidden in the packet.
- **Inputs:** the four-part approved spec on this branch; packet text is in the T3 delegated task and summarised in #391.
- **Outputs:** pending.
- **Measured calls/tokens/wall time:** UNKNOWN.

## EVENT 3 — 2026-11-12 01:37 UTC — Sol implementation returned

- **Evidence class:** OBSERVED FACT for commits/diff; author results are AUTHOR CLAIMS until reviewed.
- **Provider/model:** `gpt-6.1-sol` medium, pool A. No quota message, retry, restart or pool switch observed. Tokens/wall time UNKNOWN.
- **Outputs:** source `02cd3ec9084a0327dddf61075fa5bb49f0907568`, report head `f32be4c5bd7711ab93d92f249f4f02b4f21a3bb1`, pushed (ls-remote matches), worktree clean. [Author report](https://github.com/jermur788/CCF/blob/f32be4c5bd7711ab93d92f249f4f02b4f21a3bb1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-IMPLEMENTATION-RESULT.md).
- **Claude A diff inspection (word-diff):** 8 files: six allowed UI files, one verifier pin (`ScenarioOneTeachingCopyVerification.cs`), the report. No file under `Assets/ForestPrototype/ScenarioOne` changed. One non-literal change: `ScenarioOneUiFacts` adds a display `.Replace("Establish planted ", "Planting completed; species present: ")`.
- **Author claims:** presentation checks 157 PASS baseline and final, 5 mutants killed; offline compile 189 sources 0 errors; Rack help checker keeps pre-existing failures plus an expected scope-whitelist failure. Unity/screenshots/runtime hash NOT RUN.
- **ORCHESTRATOR CLAIM (questions routed to review, not pre-judged):** whether narrowing the "being small/suppressed" lines dropped a teaching point; whether the planting label reads naturally; whether the display Replace is consumed anywhere non-display.

## EVENT 4 — 2026-11-12 01:37 UTC — independent review dispatch

- **Evidence class:** OBSERVED FACT.
- **Provider/model/effort/pool:** Anthropic `claude-sonnet-5-5`, effort medium, provider "Claude B — Pro" (separate account from Claude A). Opus not used; no justification for escalation at LOW difficulty.
- **Claim:** [#392](https://github.com/jermur788/CCF/issues/392) [RUNNING]; harness-isolated checkout; report branch `review/scenario-one-research-copy-v1-claude-b-20261112`. Read-only on source. Unity lease: none.
- **Outputs:** pending.

## EVENT 5 — 2026-11-12 01:41 UTC — Claude B verdict NEEDS CHANGES; rework dispatched

- **Evidence class:** OBSERVED FACT (verdict, push); reviewer findings are REVIEWER CLAIMS accepted by Claude A for rework.
- **Reviewer:** `claude-sonnet-5-5` medium, "Claude B — Pro". No quota event observed. Tokens/time UNKNOWN.
- **Review report:** [6038d85c](https://github.com/jermur788/CCF/blob/6038d85cac7bb6b85c1dde9bf3280a099ea959c8/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-INDEPENDENT-REVIEW.md) on `review/scenario-one-research-copy-v1-claude-b-20261112`.
- **Findings:** 1 major (stale pins in `Tools/Verification/WorkPlanResidualVerification.cs` for changed Work Plan strings, unreported by author); 3 minor (planting label double colon / "completed" on an open line; raw displayName shown in Annual Review "Objective first reached"; a removed Help sentence the spec did not ask to remove); notes (removal count wording, term used before introduced, optional sentence omitted, RackV2HelpTeaching failures pre-existing on baseline). Scope, protected identity and truthfulness judged sound. Reviewer reran presentation checks 157/157; did not rerun the offline compile; Unity NOT RUN, AC-7 NOT VERIFIED.
- **Failure class:** incomplete author grep for pinned strings (verification debt), plus copy polish. Salvage: all work kept.
- **Route:** rework loop 1 to the same author route, `gpt-6.1-sol` medium pool A (context continuity is in the report, cheapest route; pool B still in reserve). Claude A did not fix findings itself (role separation in the #390 handoff).
- **Next:** delta review by Claude B of the rework, then Unity qualification if the lease is free.

## EVENT 6 — 2026-11-12 01:46 UTC — rework loop 1 returned; delta review APPROVED (source only)

- **Evidence class:** OBSERVED FACT (commits, pushes, verdict); check results are AUTHOR/REVIEWER CLAIMS, the presentation run and offline compile were reproduced by the reviewer.
- **Rework:** `gpt-6.1-sol` medium, pool A, second run; no quota event, no pool switch. Source `99af87eeabcdb40b9b36269197198742c944ffb0`, branch head `868643a1311ad79a759dbe18d4353f1b928ce2a7` on `feature/scenario-one-research-copy-v1-20261112`. Diff from baseline: six UI files, two verifier pins (`Tools/Verification/ScenarioOneTeachingCopyVerification.cs`, `Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs`), author report.
- **Delta review:** `claude-sonnet-5-5` medium, "Claude B — Pro", fresh session. Verdict **APPROVED for the source candidate 868643a1**; [delta review 3eeac8db](https://github.com/jermur788/CCF/blob/3eeac8dbbf3aa19159d2157e859ba5912e6f6911/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-DELTA-REVIEW.md). All five earlier findings closed; two non-blocking wording nits (telegraphic objective label; "Share … Share" repetition in curriculum topic 2).
- **Tests:** presentation `run_checks.py` PASS 157, 5 mutants killed (author and reviewer). Offline compile `sources=191 refs=116 errors=0` (author and reviewer). RackV2HelpTeaching offline: FAIL on baseline and candidate (pre-existing S6 + ten step-6 checks; candidate adds S10, that tool's file whitelist predating #391) — not fixed, not rebaselined. `git diff b62446ca 868643a1 -- Assets/ForestPrototype/ScenarioOne` empty.
- **NOT RUN:** Unity Editor, both Unity-hosted verifiers, 1280×720 / 1920×1080 captures (AC-7), runtime world-hash / Reference Future / Legacy40 comparison (AC-6 runtime half; source half is supported by the empty protected diff).
- **Review loops:** 1 NEEDS CHANGES, 1 APPROVED. Opus not used.
- **Gate reached:** Unity qualification needs either a checkout switch of the user's #382 lane or a new ZX20 working set; both are user-owned decisions and have been put to the user. Integration into the candidate branch additionally changes what #382 pins and needs a Manager #200 / user decision. No integration performed.

## EVENT 7 — 2026-11-12 01:53 UTC — user decisions; Unity qualification dispatch

- **Evidence class:** OBSERVED FACT.
- **User decisions (asked by Claude A in session, answered by the user):** (1) Unity lane = **new ZX20 lane**, the #382 playtest lane is not to be reused; (2) integration = **leave on the feature branch** as the revised candidate and return to Manager #200 for the merge decision; the #382 pin `2aaacad6` is unchanged.
- **Dispatch:** Unity qualification to `gpt-6.1-sol` medium, pool A (third Sol run on this pool; no limit observed so far). Rationale (ORCHESTRATOR CLAIM): Sol ran the #376–#381 Unity sessions on this lineage and its launcher/path-guard evidence is reusable; this is author-side runtime evidence, not independent review. Alternative considered: Claude A running Unity directly (rejected: role separation and context cost).
- **Lane:** to be created by the worker at `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`, branch `qualify/scenario-one-research-copy-v1-20261112` @ `868643a1`, fresh Library, ~6.6 GB expected, floor 15 GiB (25 GiB free observed before). Lease: to be acquired per session through the repo unity-lock tool; never forced.
- **Evidence root:** `/media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/`. Report branch: `report/scenario-one-research-copy-v1-unity-qual-20261112`.
- **Outputs:** pending.

## EVENT 8 — 2026-11-12 01:56 UTC — Unity attempt 1 BLOCKED (lane not registered); registry entry added; attempt 2 dispatched

- **Evidence class:** OBSERVED FACT.
- **Attempt 1 result:** `gpt-6.1-sol` medium, pool A. **BLOCKED** before any Editor start: `unity-lock acquire` exit 2, `lane is not configured: /media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`. [Report 452bf98b](https://github.com/jermur788/CCF/blob/452bf98be55737ca31d4861a2a9d08b123c86de6/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-UNITY-QUALIFICATION-RESULT.md). Lane worktree created clean @ `868643a1`; no Library, no licensed assets copied; no lease; lock FREE; ZX20 24.95 → 22.98 GiB free; 37 host custody files unchanged. All Unity checks NOT RUN.
- **Failure class:** operational prerequisite missed in the Claude A packet (lane registration), not a product or provider failure. Cost: one wasted worker run.
- **Action by Claude A:** appended exactly one entry to deployed `/media/jer/ZX20/CCF-Ops/lanes.json` (`ResearchCopyQual391`, `unity: true`), following the UIQual275 / AccessSpineMeasure248 / UIQual376 precedent for a user-authorised lane. Backup `lanes.json.before-391` and `lanes-json-391.diff` are in `/media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/`. No other config touched; no lock forced. `unity-lock status` after: free, no Editor PIDs. **Manager #200 should confirm or revert this registration.**
- **Attempt 2:** same route (`gpt-6.1-sol` medium, pool A), fresh session with the attempt-1 report as input. Pool B still unused.

## EVENT 9 — 2026-11-12 02:11 UTC — Unity attempt 2 PARTIAL; attempt 3 dispatched at higher effort

- **Evidence class:** OBSERVED FACT (report, log, disk); diagnosis lines are ORCHESTRATOR CLAIM.
- **Attempt 2 result:** `gpt-6.1-sol` medium, pool A. **PARTIAL** — [report a962e146](https://github.com/jermur788/CCF/blob/a962e14679fb70d8e4df01989aa9612afd4678f9/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-UNITY-QUALIFICATION-RESULT.md).
  - Real-Editor import + compile: **PASS** (0 C# errors, Play Mode reached).
  - `ScenarioOneTeachingCopyVerification`: **FAIL** `nursery warning visible` (layout-overlap assertion at line 198 on the Work Plan nursery warning; not text changed by this patch). Five 1280 captures all **black**; UI Toolkit render-chain exceptions in log. 1080 NOT RUN.
  - `WorkPlanResidualVerification`, world-hash comparison, Reference/Legacy40: **NOT RUN**.
  - Lease `c72e3e0fce27404b88df38b3f3daa146` acquired and released; lock FREE. Editor PID 113621 hung after shutdown markers and was killed by PID (disclosed). Runner 671 s.
  - 27 licensed ignored assets copied read-only from the #382 lane with matching SHA-256; that lane otherwise untouched. Nine host save/Prefs files unchanged. Lane clean @ `868643a1`.
  - Disk: ZX20 22.98 → 16.45 GiB free (17,661,034,496 bytes; lock floor 16,106,127,360). **Margin ≈ 1.45 GiB.**
- **Failure class:** verification-harness/rendering failure on a cold first import with an old harness; no product defect established, and none excluded for the nursery assertion (UNDETERMINED).
- **ORCHESTRATOR CLAIM:** the worker chose the pre-#376 S1A harness instead of the capture route that produced contentful images in #379/#381; the nursery assertion was evaluated while rendering was faulting. Neither is proven.
- **Route change:** attempt 3 to `gpt-6.1-sol` at **high** effort (same pool A; no quota event observed in five runs). Justification: the remaining work is harness/render diagnosis rather than LOW copy work, and two medium runs returned without usable captures. Pool B not used (same model; no capacity reason to switch). Declared final bounded Unity attempt; if it cannot produce contentful frames or disk falls under the floor, AC-7 is handed back NOT VERIFIED rather than retried indefinitely.

## EVENT 10 — 2026-11-12 02:44 UTC — Unity attempt 3: rendered FAIL on one panel (F391-01); fix + re-verification dispatched

- **Evidence class:** OBSERVED FACT (report, images viewed by Claude A); causation lines are ORCHESTRATOR CLAIM.
- **Attempt 3 result:** `gpt-6.1-sol` **high**, pool A. Verdict **FAIL (one rendered finding)** — [report f757a72e](https://github.com/jermur788/CCF/blob/f757a72e0ca5e1def353c2bc56ba118596221274/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-UNITY-QUALIFICATION-RESULT.md). Five leases acquired and released normally; lock FREE; no Editor killed. Disk end 17,659,592,704 bytes. Nine host save/Prefs hashes unchanged; lane clean @ `868643a1`.
  - Rendered 1280×720 and 1920×1080 **PASS**: Help (Share paragraph, wind sentence, canopy/gap sentence), objectives list, curriculum topic 2 and map.regeneration, Lesson 3 "Why it matters", Annual Review "removal job(s) completed" and "Objective first reached: Areas with young trees".
  - **F391-01 FAIL at both sizes:** Work Plan clearance row — name label "Clearance, square A1" squeezed to ~5 layout units and wrapped one character per line beside the lengthened value. Claude A viewed `clearance-young-1280x720.png` and confirms the defect is real and player-visible.
  - Reference gate **PASS**, anchors equal the #381 record (continuation `EBD49ADEF18C3F36`, legacy layout `BFFF66015A6619DA`). Unmodified P3 `WorkPlanResidualVerification` **PASS** (does not look at the scrolled rows).
  - Black frames of attempt 2: not reproduced warm; cold-run cause unproven. "Render-chain exceptions" reported in attempt 2 were Debug.Log stack frames (attempt-2 statement corrected).
  - **NOT RUN:** baseline-save CurrentWorldHash comparison (no retained fixture with a recorded hash); Oak/Beech first-reached Annual Review variants. Nursery-warning assertion of the old S1A harness: **UNDETERMINED**.
- **ORCHESTRATOR CLAIM:** F391-01 is most likely a regression from this patch's longer value text (48 → 62 characters in a non-shrinking value cell), but no rendered baseline counterfactual exists. This is exactly the plan's "panel clipping" stop condition; it was caught by the rendered gate, not by source review or offline checks (Claude B had marked the Work Plan card NOT VERIFIED).
- **Decision:** fix via the existing `UiKit.Line(..., valueBelow: true)` option on that single call, keeping approved wording. This is a one-argument presentation-layout change, **outside a strict strings-only reading of the spec**; taken under the user's "continue through necessary fixes" instruction, flagged for Manager #200. Alternative considered: shortening the wording to the old length (rejected: not guaranteed to fit and loses the truthful "group portion" wording).
- **Dispatch:** `gpt-6.1-sol` high, pool A — source fix, offline checks, then Unity recapture of the clearance rows and P3 gate on the same lane fast-forwarded to the new head. A Claude B delta review of the final head follows.

## EVENT 11 — 2026-11-12 02:56 UTC — F391-01 fixed and re-verified (author Unity evidence); final delta review dispatched

- **Evidence class:** OBSERVED FACT (commits, diff, image viewed by Claude A); Unity results are AUTHOR-RUN evidence.
- **Fix run:** `gpt-6.1-sol` high, pool A (seventh Sol run; no quota event, restart or pool switch in the whole task). New feature head `a314d308315b5894263d30461a1e8b52ca0f3605`; `git diff 868643a1 a314d308` = one argument (`valueBelow: true`) in `WorkPlanOverview.cs` plus the author report. Unity report [f4bd3a02](https://github.com/jermur788/CCF/blob/f4bd3a02f8f826dd9f0c9787d36b63e5bb428a72/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-UNITY-QUALIFICATION-RESULT.md).
- **Results on a314d308:** offline presentation 157 PASS / 5 mutants killed; offline compile 191 sources 0 errors; real-Editor compile 0 errors, exit 0; unmodified P3 `WorkPlanResidualVerification` PASS; clearance rows 1920×1080 and 1280×720 **PASS** (labels on one line, values beneath). Claude A viewed `attempt4-clearance-young-1280x720.png`: both rows readable, neighbours intact.
- **Carried from 868643a1 by inference, not rerun:** Help, objectives, curriculum, lesson, Annual Review captures; Reference anchors.
- **Custody:** leases `e6b25ec3104a40b9abcd12b15826b178`, `b1b0b566ab9548fc814f952ca9912819` released; lock FREE; qual lane clean @ `a314d308`; ZX20 17,659,592,704 bytes free; nine host save/Prefs hashes unchanged. Claude A check: #382 lane `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` still clean @ `2aaacad6`.
- **Still NOT RUN / UNDETERMINED:** baseline-save CurrentWorldHash comparison; Oak/Beech first-reached variants; old S1A `nursery warning visible` assertion.
- **Dispatch:** Claude B `claude-sonnet-5-5` medium ("Claude B — Pro") final delta review of `a314d308` including an audit of the author screenshots. Opus still not used.

## EVENT 12 — 2026-11-12 03:00 UTC — final Claude B verdict APPROVED; orchestration result published

- **Evidence class:** OBSERVED FACT.
- **Reviewer:** `claude-sonnet-5-5`, "Claude B — Pro". Requested effort medium; **the reviewer's own report says it ran at low effort** — recorded as reported, not corrected. [Final delta review 100a43a2](https://github.com/jermur788/CCF/blob/100a43a252c4d370c4b407b1d6aa5663c710063f/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-FINAL-DELTA-REVIEW.md): **APPROVED for a314d308** with scope limits; screenshots audited; `run_checks.py` 157 PASS rerun; two non-blocking report-consistency notes (duplicate scroll position in density frames; anchors table shows eight lines, prose says nine).
- **Integration:** none performed (user decision, Event 7). Result file: [CLAUDE-A-ORCHESTRATION-IMPLEMENTATION-RESULT.md](CLAUDE-A-ORCHESTRATION-IMPLEMENTATION-RESULT.md).

## End-of-run summary (from logged events only)

- **Work:** 1 implementation task attempted and completed (C-01) with 2 rework loops (review findings; rendered finding F391-01). 0 rejected. Integration deliberately not attempted.
- **Model/pool usage:** Claude A `claude-fable-5-2` (1 session, orchestration only, no source edits; one operational config edit to `lanes.json`). Sol `gpt-6.1-sol` pool A: 7 runs (4 medium: implement, rework 1, Unity attempts 1–2; 3 high: Unity attempt 3, fix + attempt 4 — counted as 2 high runs — see events 9–11; total medium 4, high 2, i.e. **6 runs**; the "seventh" in Event 11 is a miscount, left unedited above per append-only rule). Sol pool B: 0 runs. Claude B `claude-sonnet-5-5`: 3 review runs. Opus: 0. Pool switches: 0. Quota/cooldown/login events observed: 0. Restarts: 0 provider restarts; 3 Unity re-dispatches.
- **Tokens / cost / wall time per run:** UNKNOWN (not exposed to the orchestrator). Whole run wall time ≈ from 01:30 UTC to 2026-11-12 03:00 UTC.
- **Context:** Claude A compactions observed: 0.
- **Review:** Claude B verdicts NEEDS CHANGES (f32be4c5) → APPROVED source (868643a1) → APPROVED (a314d308).
- **Test failures and gates:** stale verifier pins (fixed); Unity lane unregistered (fixed by registry entry); black captures on cold import (not reproduced warm, cause unproven); F391-01 rendered defect (fixed, re-verified); pre-existing RackV2HelpTeaching failures (untouched); nursery-warning assertion UNDETERMINED; baseline-save world-hash load NOT RUN.
- **Protected identity:** protected source diff empty; Reference anchors equal #381 record on 868643a1 (author-run); host save/Prefs hashes unchanged; #382 lane clean @ 2aaacad6.
- **User action required:** none to keep playtesting #382 as pinned. Manager #200 decisions listed in the result file.
- **Dashboard / Drive / active-tasks sync:** NOT DONE by Claude A; NOT VERIFIED.
- **Orchestration errors by Claude A (for learning):** did not check lane registration before the first Unity dispatch (one wasted run); did not require a rendered check before the first "approved" source review, so a layout regression surfaced late.
